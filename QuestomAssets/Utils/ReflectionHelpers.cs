using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace QuestomAssets.Utils
{
    public static class ReflectionHelper
    {
        public static void AddObjectToEnum(ISmartPtr<AssetsObject> toAdd, IEnumerable<ISmartPtr<AssetsObject>> target)
        {
            var addMethod = target.GetType().GetMethod("Add");
            if (addMethod == null)
                throw new Exception($"Add method could not be found on type {target.GetType().Name}!  The passed object should be a List<>!");

            addMethod.Invoke(target, new object[] { toAdd });
        }

        public static void InvokeRemoveAt(object targetObject, int index)
        {
            var removeAtMethod = targetObject.GetType().GetMethod("RemoveAt");
            if (removeAtMethod == null)
                throw new Exception($"RemoveAt method could not be found on type {targetObject.GetType().Name}!  The passed target object should be a List<>!");

            removeAtMethod.Invoke(targetObject, new object[] { index });
        }

        public static void AssignPtrToPropName(object targetObject, string targetPropName, ISmartPtr<AssetsObject> ptr)
        {
            var prop = targetObject.GetType().GetProperty(targetPropName);
            if (prop == null)
            {
                throw new Exception($"Couldn't find property {targetPropName} on type {targetObject.GetType().Name}.");
            }
            prop.SetValue(targetObject, ptr, null);
        }

        public static bool HasPropNameOfType<T>(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            if (prop == null)
                return true;
            if (prop.PropertyType != typeof(T))
                return false;

            return true;
        }


        public static bool IsPropNameAssignableToType(object obj, string propName, Type type)
        {
            var prop = obj.GetType().GetProperty(propName);
            if (prop == null)
            {
                throw new Exception($"Couldn't find property {propName} on type {obj.GetType().Name}.");
            }
            return type.IsAssignableFrom(prop.PropertyType);
        }

        public static ISmartPtr<AssetsObject> GetPtrFromPropName(object obj, string propName)
        {
            var prop = obj.GetType().GetProperty(propName);
            if (prop == null)
            {
                throw new Exception($"Couldn't find property {propName} on type {obj.GetType().Name}.");
            }
            var val = prop.GetValue(obj, null);
            if (val == null)
                return null;

            if (!typeof(ISmartPtr<AssetsObject>).IsAssignableFrom(val.GetType()))
                throw new Exception($"Value of property {propName} on type {obj.GetType().Name} could not be converted to {nameof(ISmartPtr<AssetsObject>)}!");

            return (ISmartPtr<AssetsObject>)val;
        }

        public static ISmartPtr<AssetsObject> MakeTypedPointer(AssetsObject owner, AssetsObject target)
        {
            var fname = owner.ObjectInfo.ParentFile.AssetsFileName;
                var fname2 = owner.ObjectInfo.ParentFile.AssetsFileName;
            var genericInfoType = typeof(SmartPtr<>).MakeGenericType(target.GetType());
            var constructor = genericInfoType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(AssetsObject), target.GetType() }, null);

            if (constructor == null)
                throw new Exception("Unable to find the proper SmartPtr constructor!");
            return (ISmartPtr<AssetsObject>)constructor.Invoke(new object[] { owner, target });
        }
    }
}
