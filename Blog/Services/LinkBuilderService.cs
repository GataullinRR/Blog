using ASPCoreUtilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class LinkBuilderService : ServiceBase
    {
        public LinkBuilderService(ServiceLocator services) : base(services)
        {

        }

        public string GenerateLink(string controller, string controllerAction, object values)
        {
            return S.UrlHelper.Action(
                controllerAction, 
                controller.GetController(), 
                values, 
                S.HttpContext.Request.Scheme);
        }
    }
}
