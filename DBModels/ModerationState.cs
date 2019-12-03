using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class ModerationInfo : IDbEntity
    {
        [Key]
        public int Id { get; set; }
        public ModerationState State { get; set; }
        public string StateReasoning { get; set; }
    }
}
