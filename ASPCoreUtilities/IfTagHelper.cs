using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ASPCoreUtilities
{
    [HtmlTargetElement("*")]
    public class IfTagHelper : TagHelper
    {
        public override int Order => -1000;

        [HtmlAttributeName("include-if")]
        public bool Include { get; set; } = true;

        [HtmlAttributeName("exclude-if")]
        public bool Exclude { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Always strip the outer tag name as we never want <if> to render
            //output.TagName = null;
            output.Attributes.RemoveAll("include-if");
            output.Attributes.RemoveAll("exclude-if");

            if (Include && !Exclude)
            {
                return;
            }

            output.SuppressOutput();
        }
    }
}
