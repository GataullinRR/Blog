using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPCoreUtilities.Types
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
        public Type RegisterAs { get; }

        public ServiceAttribute(ServiceType serviceType)
        {
            ServiceType = serviceType;
        }
        public ServiceAttribute(ServiceType serviceType, Type registerAs) : this(serviceType)
        {
            RegisterAs = registerAs ?? throw new ArgumentNullException(nameof(registerAs));
        }
    }
}
