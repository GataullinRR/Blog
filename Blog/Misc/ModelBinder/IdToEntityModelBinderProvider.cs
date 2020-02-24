using Blog.Attributes;
using DBModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Linq;
using Utilities.Extensions;

namespace Blog.Misc.ModelBinder
{
    public class IdToEntityModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            var metadata = (DefaultModelMetadata)context.Metadata;
            var isFromEntityId = metadata.Attributes?.ParameterAttributes
                ?.Any(a => a.GetType() == typeof(FromEntityIdAttribute)) ?? false;
            if (isFromEntityId)
            {
                return new BinderTypeModelBinder(typeof(IdToEntityBinder));
            }
            else
            {
                return null;
            }
        }
    }
}
