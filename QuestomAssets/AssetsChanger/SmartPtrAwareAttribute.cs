using System;
using System.Collections.Generic;
using System.Text;
using Cauldron.Interception;

namespace QuestomAssets.AssetsChanger
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class SmartPtrAwareAttribute : Attribute, IPropertySetterInterceptor
    {
        
        public void OnExit()
        {
        }

        public bool OnSet(PropertyInterceptionInfo propertyInterceptionInfo, object oldValue, object newValue)
        {
            
            if (typeof(ISmartPtr<AssetsObject>).IsAssignableFrom(propertyInterceptionInfo.PropertyType))
            {
                ISmartPtr<AssetsObject> aOld = (ISmartPtr<AssetsObject>)oldValue;
                ISmartPtr<AssetsObject> aNew = (ISmartPtr<AssetsObject>)newValue;
                if (aNew != null)
                    aNew.Owner = ((AssetsObject)propertyInterceptionInfo.Instance);
                if (aOld != null)
                    aOld.Owner = null;
            }
            return false;
        }


        public bool OnException(Exception e)
        {
            throw new NotImplementedException();
        }
    }
}
