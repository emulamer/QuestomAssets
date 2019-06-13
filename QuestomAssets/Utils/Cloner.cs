using QuestomAssets.AssetsChanger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.Utils
{
    public static class Cloner
    {
        private static void ClonePropsInObj(object curObj, AssetsObject parentObj, Dictionary<AssetsObject, AssetsObject> clonedObjects, AssetsFile toFile, List<AssetsObject> addedObjects, List<CloneExclusion> exclusions)
        {
            var file = parentObj.ObjectInfo.ParentFile.AssetsFileName;
            var updateProps = curObj.GetType().GetProperties().ToList();

            //remove any array properties that are a string or a value type
            updateProps.Where(x => x.PropertyType.IsArray && (x.PropertyType.GetElementType().IsValueType || x.PropertyType.GetElementType() == typeof(string)))
                .ToList().ForEach(x => updateProps.Remove(x));

            //look through any properties that are smart pointers, clone their targets and make a new pointer, then remove them from the list of props to update
            var propsToClone = updateProps.Where(x => typeof(ISmartPtr<AssetsObject>).IsAssignableFrom(x.PropertyType)).ToList();
            foreach (var prop in propsToClone)
            {
                var baseVal = prop.GetValue(curObj, null);
                if (baseVal == null)
                    continue;
                var propPtr = (prop.GetValue(curObj, null) as ISmartPtr<AssetsObject>);
                var propObj = propPtr.Target.Object;
                AssetsObject assignObject = null;

                switch (exclusions.GetExclusionMode(propPtr, prop))
                {
                    case ExclusionMode.LeaveRef:
                        assignObject = propObj;
                        break;
                    case ExclusionMode.Remove:
                        assignObject = null;
                        Log.LogErr($"WARNING: Cloner is leaving the pointer NULL on property '{curObj.GetType().Name}.{prop.Name}'");
                        break;
                    default:
                        assignObject = DeepClone(propObj, toFile, addedObjects, clonedObjects, exclusions);
                        break;
                }
                                                             
                prop.SetValue(curObj, ReflectionHelper.MakeTypedPointer(parentObj, assignObject), null);
            }
            propsToClone.ForEach(x => updateProps.Remove(x));

            //look through any properties that lists of smart pointers, this code isn't ideal because it only actually supports things that have a default indexer
            //  I should clean this up to work better
            var listsToClone = updateProps.Where(x => typeof(IEnumerable<ISmartPtr<AssetsObject>>).IsAssignableFrom(x.PropertyType)).ToList();
            foreach (var listProp in listsToClone)
            {
                var listVal = listProp.GetValue(curObj, null) as IEnumerable<ISmartPtr<AssetsObject>>;

                if (listVal == null)
                    continue;
                if (listProp.PropertyType.IsArray)
                {
                    Array listArr = (Array)listVal;
                    for (int i = 0; i < listArr.Length; i++)
                    {
                        var ptrVal = listArr.GetValue(i) as ISmartPtr<AssetsObject>;
                        ISmartPtr<AssetsObject> clonedObj = null;
                        switch (exclusions.GetExclusionMode(ptrVal, listProp))
                        {
                            case ExclusionMode.LeaveRef:
                                clonedObj = listArr.GetValue(i) as ISmartPtr<AssetsObject>;
                                break;
                            case ExclusionMode.Remove:
                                clonedObj = null;
                                break;
                            default:
                                clonedObj = ReflectionHelper.MakeTypedPointer(DeepClone(ptrVal.Target.Object, toFile, addedObjects, clonedObjects, exclusions), parentObj);
                                break;
                        }
                        if (clonedObj == null)
                        {
                            listArr.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            listArr.SetValue( clonedObj, i);
                        }
                    }
                }
                else
                {
                    var indexerProp = listVal.GetType().GetProperties().Where(x => x.Name == "Item").FirstOrDefault();

                    if (indexerProp == null)
                    {
                        throw new NotSupportedException($"Couldn't find the default indexer property on {curObj.GetType().Name}.{listProp.Name}!");
                    }
                    for (int i = 0; i < listVal.Count(); i++)
                    {
                        var ptrVal = indexerProp.GetValue(listVal, new object[] { i }) as ISmartPtr<AssetsObject>;
                        AssetsObject clonedObj = null;
                        switch (exclusions.GetExclusionMode(ptrVal, listProp))
                        {
                            case ExclusionMode.LeaveRef:
                                clonedObj = ptrVal.Target.Object;
                                break;
                            case ExclusionMode.Remove:
                                clonedObj = null;
                                break;
                            default:
                                clonedObj = DeepClone(ptrVal.Target.Object, toFile, addedObjects, clonedObjects, exclusions);
                                break;
                        }

                        //if the cloned object comes back null, remove it from the list instead of setting it null
                        if (clonedObj == null)
                        {
                            ReflectionHelper.InvokeRemoveAt(listVal, i);
                        }
                        else
                        {
                            indexerProp.SetValue(listVal, ReflectionHelper.MakeTypedPointer(parentObj as AssetsObject, clonedObj), new object[] { i });
                        }
                    }
                }
            }
            listsToClone.ForEach(x => updateProps.Remove(x));

            //look through any objects that are plain old lists of whatever.  this is to catch lists of "structs" that may have pointers in them
            var plainEnumerableToClone = updateProps.Where(x => !x.PropertyType.IsValueType && !(x.PropertyType == typeof(string)) && typeof(IEnumerable).IsAssignableFrom(x.PropertyType)).ToList();
            foreach (var enumProp in plainEnumerableToClone)
            {
                var listVal = enumProp.GetValue(curObj, null) as IEnumerable;

                if (listVal == null)
                    continue;

                foreach (var plainObj in listVal)
                {
                    //pass in the parent AssetsObject that was passed to us since that object will be the "owner", not the struct object
                    ClonePropsInObj(plainObj, parentObj, clonedObjects, toFile, addedObjects, exclusions);
                }
            }
            plainEnumerableToClone.ForEach(x => updateProps.Remove(x));
            //look through any "struct" type properties that may have pointers in them
            var plainObjToClone = updateProps.Where(x => !x.PropertyType.IsValueType && !(x.PropertyType == typeof(string)));
            foreach (var plainProp in plainObjToClone)
            {
                var objVal = plainProp.GetValue(curObj, null) as IEnumerable;
                if (objVal == null)
                    continue;

                foreach (var plainObj in objVal)
                {
                    //pass in the parent AssetsObject that was passed to us since that object will be the "owner", not the struct object
                    ClonePropsInObj(plainObj, parentObj, clonedObjects, toFile, addedObjects, exclusions);
                }
            }
        }

        public static T DeepClone<T>(T source, AssetsFile toFile = null, List<AssetsObject> addedObjects = null, Dictionary<AssetsObject, AssetsObject> clonedObjects = null, List<CloneExclusion> exclusions = null) where T : AssetsObject
        {
            if (exclusions == null)
                exclusions = new List<CloneExclusion>();

            if (clonedObjects == null)
            {
                clonedObjects = new Dictionary<AssetsObject, AssetsObject>();
            }
            else
            {
                if (clonedObjects.ContainsKey(source))
                {
                    return (T)clonedObjects[source];
                }
            }
            if (addedObjects == null)
            {
                addedObjects = new List<AssetsObject>();
            }



            var curObj = source.ObjectInfo.Clone(toFile);

            (toFile ?? source.ObjectInfo.ParentFile).AddObject(curObj, true);

            addedObjects.Add(curObj);

            clonedObjects.Add(source, curObj);
            ClonePropsInObj(curObj, curObj, clonedObjects, toFile, addedObjects, exclusions);
            return (T)curObj;
        }
    }
}
