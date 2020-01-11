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
        protected ServicesLocator S { get; }

        public ServiceBase(ServicesLocator services)
        {
            S = services;
        }
    }
}
