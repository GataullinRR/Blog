using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class ViewStatistic
    {
        [Key]
        public int Id { get; set; }
        public int RegistredUserViews { get; set; }
        public int TotalViews { get; set; }
    }
}
