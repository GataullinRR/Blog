using System;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public abstract class EditBase : IDbEntity, IAuthored
    {
        [Key] public int Id { get; set; }
        [Required] public virtual User Author { get; set; }
        [Required] public string Reason { get; set; }
        [Required] public DateTime EditTime { get; set; }
    }
}
