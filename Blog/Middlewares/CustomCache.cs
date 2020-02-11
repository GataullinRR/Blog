﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Middlewares
{
    public static class CustomCache
    {
        public const string CACHE_MODEL_TAG = "script";
        public const string CACHE_MODEL_TAG_ATTRIBUTE = "cache-model";
        internal const string CACHE_MODEL_SECTION_START = "<" + CACHE_MODEL_TAG + " " + CACHE_MODEL_TAG_ATTRIBUTE + ">";
        internal const string CACHE_MODEL_SECTION_END = "</" + CACHE_MODEL_TAG + ">";

        /// <summary>
        /// Marks place to where client model object will 
        /// Usage: var model = <see cref="CLIENT_MODEL_PLACEHOLDER"/>;
        /// </summary>
        public const string CLIENT_MODEL_PLACEHOLDER = "=>client model<=";
    }
}