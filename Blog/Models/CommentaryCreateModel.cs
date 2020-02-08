using Blog.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class CommentaryCreateModel
    {
        [BindProperty, Required, BindRequired]
        public int PostId { get; set; }
        [BindProperty, Required, BindRequired,
            MinLength(CommentaryController.MIN_COMMENTARY_LENGTH), 
            MaxLength(CommentaryController.MAX_COMMENTARY_LENGTH)]
        public string Body { get; set; }
    }
}
