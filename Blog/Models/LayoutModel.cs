using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    [Serializable()]
    public class LayoutModel
    {
        public List<string> Messages { get; } = new List<string>();
    }
}
