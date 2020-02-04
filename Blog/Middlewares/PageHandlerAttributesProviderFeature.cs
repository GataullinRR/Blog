﻿using System;
using System.Collections.Generic;

namespace Blog.Middlewares
{
    public class PageHandlerAttributesProviderFeature
    {
        public IEnumerable<object> Attributes { get; }

        public PageHandlerAttributesProviderFeature(IEnumerable<object> attributes)
        {
            Attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
        }
    }
}
