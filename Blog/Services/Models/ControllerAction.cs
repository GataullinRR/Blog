using ASPCoreUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPCoreUtilities.Extensions;

namespace Blog.Services.Models
{
    public class ControllerAction
    {
        public string ControllerName { get; }
        public string ActionName { get; }

        public ControllerAction(string controllerName, string actionName)
        {
            ControllerName = controllerName?.GetController() ?? throw new ArgumentNullException(nameof(controllerName));
            ActionName = actionName ?? throw new ArgumentNullException(nameof(actionName));
        }

        public string GetURI()
        {
            return $"/{ControllerName}/{ActionName}";
        }
    }
}
