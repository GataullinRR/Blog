﻿using Microsoft.AspNetCore.Mvc;
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
        [BindProperty(), Required(), MinLength(6), MaxLength(1000), BindRequired]
        public string Body { get; set; }
    }
}
