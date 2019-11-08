using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class ObjectReferenceModel
    {
        public ObjectReferenceModel(object @object)
        {
            Object = @object ?? throw new ArgumentNullException(nameof(@object));
        }

        public object Object { get; }
    }
}
