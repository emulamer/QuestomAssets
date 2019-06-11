using QuestomAssets.AssetsChanger;
using QuestomAssets.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace QuestomAssets.Utils
{
    public enum ExclusionMode
    {
        None = 0,
        LeaveRef = 5,
        Remove = 10
    }
    public class CloneExclusion
    {
        public CloneExclusion(ExclusionMode mode)
        { Mode = mode; }

        public CloneExclusion(ExclusionMode mode, string propertyName = null, RawPtr pointer = null, AssetsObject pointerTarget = null, Type type = null)
        {
            Mode = mode;
            PropertyName = propertyName;
            Pointer = pointer;
            Type = type;
            PointerTarget = pointerTarget;
        }

        public ExclusionMode Mode { get; set; }
        //these don't work on enums
        public string PropertyName { get; set; }
        public RawPtr Pointer { get; set; }
        public AssetsObject PointerTarget { get; set; }
        public Type Type { get; set; }

        //should have done filter from the start and the hell with the rest of it.
        //Maybe should make it object instead of pointer, but it's all relevant to pointers
        public Func<ISmartPtr<AssetsObject>, PropertyInfo, bool> Filter { get; set; }

        public bool Matches(object obj, PropertyInfo propInfo)
        {
            bool match = true;
            if (PropertyName != null && propInfo.Name != PropertyName)
            {
                match = false;
            }

            if (Pointer != null)
            {
                if (typeof(ISmartPtr<AssetsObject>).IsAssignableFrom(propInfo.PropertyType))
                {
                    var ptr = ReflectionHelper.GetPtrFromPropName(obj, propInfo.Name);

                    if (ptr == null || ptr.FileID != Pointer.FileID || ptr.PathID != Pointer.PathID)
                    {
                        match = false;
                    }
                }
                else
                {
                    match = false;
                }
            }
            
            if (PointerTarget != null)
            {
                if (typeof(ISmartPtr<AssetsObject>).IsAssignableFrom(propInfo.PropertyType))
                {
                    var ptr = obj as ISmartPtr<AssetsObject>;

                    if (ptr.Target.Object != PointerTarget)
                        match = false;
                }
                else
                {
                    match = false;
                }
            }
            if (Filter != null)
            {
                if (obj is ISmartPtr<AssetsObject>)
                {
                    if (!Filter((ISmartPtr<AssetsObject>)obj, propInfo))
                        match = false;
                }
                else
                {
                    match = false;
                }
            }
            if (Type != null && Type != propInfo.PropertyType)
            {
                match = false;
            }
            return match;
        }
    }
}
