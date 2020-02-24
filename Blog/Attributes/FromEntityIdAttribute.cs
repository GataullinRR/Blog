using System;

namespace Blog.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromEntityIdAttribute : Attribute
    {
        public string IdParameterName { get; set; }

        public FromEntityIdAttribute() : this("id")
        {

        }
        public FromEntityIdAttribute(string idParameterName)
        {
            IdParameterName = idParameterName ?? throw new ArgumentNullException(nameof(idParameterName));
        }
    }
}
