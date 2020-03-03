using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.ComponentModel.DataAnnotations;
using CommonDBModels;

namespace StatisticDBModels
{
    public interface IViewStatistic<T>
    {
        [Key]
        public T Id { get; set; }
        int RegisteredUserViews { get; set; }
        int TotalViews { get; set; }
    }

    public class ViewStatistic<T> : IViewStatistic<T>
    {
        [Key]
        public T Id { get; set; }
        public int RegisteredUserViews { get; set; }
        public int TotalViews { get; set; }

        public ViewStatistic()
        {

        }

        public ViewStatistic(T id)
        {
            Id = id;
        }
    }
}
