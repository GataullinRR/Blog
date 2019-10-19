using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Models
{
    public interface ILayoutModelProvider
    {
        LayoutModel LayoutModel { get; }
    }

    public class ExtendedPageModel : PageModel, ILayoutModelProvider
    {
        public LayoutModel LayoutModel { get; private set; } = new LayoutModel();
        public bool PersistLayoutModel { get; set; } = false;

        public override void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            LayoutModel = HttpContext.Session.Keys.Contains(nameof(LayoutModel)) 
                ? HttpContext.Session.GetString(nameof(LayoutModel)).FromBase64().Deserialize<LayoutModel>() 
                : LayoutModel;

            base.OnPageHandlerSelected(context);
        }

        public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            if (PersistLayoutModel)
            {
                HttpContext.Session.SetString(nameof(LayoutModel), LayoutModel.Serialize().ToBase64());
            }
            else
            {
                HttpContext.Session.Remove(nameof(LayoutModel));
            }

            base.OnPageHandlerExecuted(context);
        }
    }

    public class ExtendedController : Controller, ILayoutModelProvider
    {
        public LayoutModel LayoutModel { get; private set; } = new LayoutModel();
        public bool PersistLayoutModel { get; set; } = false;

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (PersistLayoutModel)
            {
                HttpContext.Session.SetString(nameof(LayoutModel), LayoutModel.Serialize().ToBase64());
            }
            else
            {
                HttpContext.Session.Remove(nameof(LayoutModel));
            }

            base.OnActionExecuted(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            LayoutModel = HttpContext.Session.Keys.Contains(nameof(LayoutModel))
                ? HttpContext.Session.GetString(nameof(LayoutModel)).FromBase64().Deserialize<LayoutModel>()
                : LayoutModel;

            base.OnActionExecuting(context);
        }
    }
}
