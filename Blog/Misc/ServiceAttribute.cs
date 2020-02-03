using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Misc
{
    public enum ServiceType
    {
        SCOPED,
        SINGLETON,
        TRANSIENT
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ServiceAttribute : Attribute
    {
        public ServiceType ServiceType { get;}

        public ServiceAttribute(ServiceType serviceType)
        {
            ServiceType = serviceType;
        }
    }
}
