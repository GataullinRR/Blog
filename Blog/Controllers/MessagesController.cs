using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class MessagesController : Controller
    {
        public IActionResult GetMessageView([Required]string body, string jsActions)
        {
            if (ModelState.IsValid)
            {
                var model = new MessageModel(body);
                if (jsActions != null)
                {
                    model.JSActions.AddRange(JSHandledAction.Parse(jsActions));
                }

                return PartialView("_MessageTemplate", model);
            }
            else
            {
                throw new Exception();
            }
        }
    }
}