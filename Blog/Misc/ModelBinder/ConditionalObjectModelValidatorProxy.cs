using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Blog.Misc.ModelBinder
{
    public class ConditionalObjectModelValidatorProxy : ObjectModelValidatorProxyBase
    {
        public ConditionalObjectModelValidatorProxy(IObjectModelValidator @base) : base(@base)
        {

        }

        public override void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model)
        {
            if (model != null && typeof(IQueryable).IsAssignableFrom(model.GetType()))
            {
                return;
            }
            else
            {
                base.Validate(actionContext, validationState, prefix, model);
            }
        }
    }
}
