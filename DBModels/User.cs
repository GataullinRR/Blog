using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class User : IdentityUser
    {
        [Required]
        public DateTime RegistrationDate { get; set; }

        public List<Post> Posts { get; set; }
        public List<Commentary> Commentaries { get; set; }
    }
}
