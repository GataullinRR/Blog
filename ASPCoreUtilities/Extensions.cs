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
            return controllerNameOf.Remove("Controller");
        }

        public static string GetHandler(this string handlerNameof)
        {
            return handlerNameof.Remove("OnGet").Remove("OnPost");
        }

        public static string GetPage(this string pageModelNameof)
        {
            return pageModelNameof.Remove("_Pages_").Remove("Model");
        }
    }
}
