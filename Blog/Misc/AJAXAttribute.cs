using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Misc
{
    /// <summary>
    /// Removes generation of error page
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AJAXAttribute : Attribute
    {

    }
}
