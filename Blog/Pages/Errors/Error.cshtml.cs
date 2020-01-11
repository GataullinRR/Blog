using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Pages.Errors
{
    public class ErrorModel : PageModel
    {
        public int HttpStatus { get; private set; }

        public ErrorModel(ServicesLocator services)
        {

        }

        public IActionResult OnGet([Required]int code)
        {
            if (ModelState.IsValid)
            {
                HttpStatus = code;

                return Page();
            }
            else
            {
                return RedirectToPage("/Index");
            }
            //var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            //if (exceptionHandlerPathFeature?.Error is UnauthorizedAccessException)
            //{
            //    Message = "You have no access";
            //}
            //if (exceptionHandlerPathFeature?.Path == "/index")
            //{
            //    ExceptionMessage += " from home page";
            //}

        }
    }
}