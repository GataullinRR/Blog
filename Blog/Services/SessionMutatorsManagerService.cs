using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Services
{
    public class SessionMutatorsManagerService : ServiceBase
    {
        public SessionMutatorsManagerService(ServicesProvider services) : base(services)
        {

        }

        public string RegistrationRole
        {
            get => Services.HttpContext.Session.GetStringOrDefault(nameof(RegistrationRole), Roles.USER);
            set
            {
                Services.HttpContext.Session.Set(nameof(RegistrationRole), value.ToByteArray(Encoding.UTF8));
            }
        }

        public void Reset(string mutatorPropertyName)
        {
            Services.HttpContext.Session.Remove(mutatorPropertyName);
        }
    }
}
