using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Nickname { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string EMail { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }
        [Required]
        public Role Role { get; set; }

        public List<Post> Posts { get; set; }
        public List<Commentary> Commentaries { get; set; }
    }
}
