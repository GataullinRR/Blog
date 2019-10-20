using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Extensions;

namespace ASPCoreUtilities
{
    public static class Extensions
    {
        public static string GetController(this string controllerNameOf)
        {
            return controllerNameOf.Remove("Controller")
        }
    }
}
