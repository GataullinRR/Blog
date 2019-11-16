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
        public LinkBuilderService(ServicesProvider services) : base(services)
        {

        }

        public string GenerateLink(string controller, string controllerAction, object values)
        {
            return Services.UrlHelper.Action(
                controllerAction, 
                controller.GetController(), 
                values, 
                Services.HttpContext.Request.Scheme);
        }
    }
}
