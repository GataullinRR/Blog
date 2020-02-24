using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Blog.Misc.ModelBinder
{
    public abstract class ObjectModelValidatorProxyBase : IObjectModelValidator
    {
        readonly IObjectModelValidator _base;

        protected ObjectModelValidatorProxyBase(IObjectModelValidator @base)
        {
            _base = @base ?? throw new ArgumentNullException(nameof(@base));
        }

        public virtual void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model)
        {
            _base.Validate(actionContext, validationState, prefix, model);
        }
    }
}
