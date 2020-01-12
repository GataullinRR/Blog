using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class ChartJSDatasetModel<T>
    {
        public string Label;
        public T[] Data;
        public double BorderWidth = 2;
        public string[] BorderColor = new string[] { "rgba(0, 0, 255, 0.5)" };
        public string[] BackgroundColor = new string[] { "rgba(0, 0, 255, 0.15)" };
    }
}
