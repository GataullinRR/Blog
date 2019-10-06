using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace DBModels
{
    public static class Genders
    {
        public const string MALE = "Male";
        public const string FEMALE = "Female";
        public const string ANOTHER = "Another";
    }

    public class User : IdentityUser
    {
        [Required]
        public DateTime RegistrationDate { get; set; }
        public string Gender { get; set; }
        public string ProfileImage { get; set; }
        public string About { get; set; }

        public List<Post> Posts { get; set; }
        public List<Commentary> Commentaries { get; set; }
    }
}
