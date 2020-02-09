using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace ASPCoreUtilities
{
    public static class Extensions
    {
        public static IQueryable<T> AsAsyncQuerable<T>(this IEnumerable<T> sequence)
        {
            return new TestAsyncEnumerable<T>(sequence).AsQueryable();
        }

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

        /// <summary>
        /// Render a partial view to string.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="controller"></param>
        /// <param name="viewNamePath"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<string> RenderViewToStringAsync<TModel>(this Controller controller, string viewNamePath, TModel model)
        {
            controller.ViewData.Model = model;

            using (StringWriter writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

                ViewEngineResult viewResult = null;

                if (viewNamePath.EndsWith(".cshtml"))
                {
                    viewResult = viewEngine.GetView(viewNamePath, viewNamePath, false);
                }
                else
                {
                    viewResult = viewEngine.FindView(controller.ControllerContext, viewNamePath, false);
                }

                if (!viewResult.Success)
                {
                    throw new Exception($"A view with the name '{viewNamePath}' could not be found");
                }

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
