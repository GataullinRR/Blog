using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public interface IViewStatistic : IDbEntity
    {
        int RegistredUserViews { get; set; }
        int TotalViews { get; set; }
    }

    public interface IViewStatistic<T> : IViewStatistic
    {
        T Owner { get; set; }
    }

    public class ViewStatistic<T> : IViewStatistic<T>
    {
        [Key]
        public int Id { get; set; }
        public int? OwnerId { get; set; }
        public T Owner { get; set; }
        public int RegistredUserViews { get; set; }
        public int TotalViews { get; set; }
    }
}
