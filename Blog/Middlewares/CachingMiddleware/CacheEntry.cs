﻿using Blog.Attributes;
using Blog.Middlewares.CachingMiddleware.Policies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Blog.Middlewares
{
    class CacheEntry
    {
        public ServerResponseCacheAttribute CachingInfo { get; }
        public byte[] ResponseBody { get; }
        public int StatusCode { get; }
        public IHeaderDictionary Headers { get; }
        public DateTime ServerCacheExpirationTime { get; }
        public bool IsRequestDataSet { get; }
        public object RequestData { get; }
        public object PolicyMetadata { get; }
        public ICachePolicy Policy { get; }
        public KeyValuePair<string, object>[] RouteData { get; }

        public int ApproximateSizeInMemory { get; set; }

        public CacheEntry(ServerResponseCacheAttribute cachingInfo, byte[] responseBody, int statusCode,
                          IHeaderDictionary headers, bool isRequestDataSet, object requestData,
                          KeyValuePair<string, object>[] routeData, ICachePolicy policy, object policyMetadata)
        {
            CachingInfo = cachingInfo ?? throw new ArgumentNullException(nameof(cachingInfo));
            ResponseBody = responseBody ?? throw new ArgumentNullException(nameof(responseBody));
            StatusCode = statusCode;
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            ServerCacheExpirationTime = DateTime.UtcNow.AddSeconds(CachingInfo.CacheDuration);
            IsRequestDataSet = isRequestDataSet;
            RequestData = requestData;
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            RouteData = routeData ?? throw new ArgumentNullException(nameof(routeData));
            PolicyMetadata = policyMetadata;

            ApproximateSizeInMemory = ResponseBody.Length + 100;
        }
    }
}
