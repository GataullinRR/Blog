using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class ObjectReferenceModel
    {
        public ObjectReferenceModel(object @object, bool isActive)
        {
            Object = @object ?? throw new ArgumentNullException(nameof(@object));
            IsActive = isActive;
        }

        public object Object { get; }
        public bool IsActive { get; }
    }
}
