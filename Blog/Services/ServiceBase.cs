using DBModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class ServiceBase
    {
        protected ServiceLocator S { get; }

        public ServiceBase(ServiceLocator services)
        {
            S = services;
        }
    }
}
