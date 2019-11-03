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
        protected ServicesProvider Services { get; }

        public ServiceBase(ServicesProvider services)
        {
            Services = services;
        }
    }
}
