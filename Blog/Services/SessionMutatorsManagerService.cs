using Blog.Attributes;
using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extensions;
using ASPCoreUtilities.Types;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class SessionMutatorsManagerService : ServiceBase
    {
        public SessionMutatorsManagerService(ServiceLocator services) : base(services)
        {

        }

        public string RegistrationRole
        {
            get => S.HttpContext.Session.GetUTF8StringOrDefault(nameof(RegistrationRole), Roles.USER);
            set
            {
                S.HttpContext.Session.Set(nameof(RegistrationRole), value.ToByteArray(Encoding.UTF8));
            }
        }

        public void Reset(string mutatorPropertyName)
        {
            S.HttpContext.Session.Remove(mutatorPropertyName);
        }
    }
}
