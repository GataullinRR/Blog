using Blog.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class CommentaryEditinigModel
    {
        [BindProperty, BindRequired, Required, MaxLength(CommentaryController.MAX_COMMENTARY_LENGTH), MinLength(CommentaryController.MIN_COMMENTARY_LENGTH)]
        public string Body { get; set; }
        [BindProperty, BindRequired, Required, MaxLength(CommentaryController.MAX_EDIT_REASON_LENGTH), MinLength(CommentaryController.MIN_EDIT_REASON_LENGTH)]
        public string Reason { get; set; }

        public string Author { get; set; }
        public string AuthorId { get; set; }
        public int CommentaryId { get; set; }
    }
}
