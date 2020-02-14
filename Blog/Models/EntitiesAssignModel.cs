using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class EntitiesAssignModel
    {
        [Required]
        public string ModeratorId { get; set; }
        [Required, BindProperty, Range(0, 100)]
        public int NumOfEntitiesToAssign { get; set; }
    }
}
