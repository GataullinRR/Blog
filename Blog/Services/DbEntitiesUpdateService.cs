using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Services
{
    public class DbEntitiesUpdateService : ServiceBase
    {
        public DbEntitiesUpdateService(ServicesLocator serviceProvider) : base(serviceProvider)
        {

        }

        public async Task UpdateViewStatisticAsync(User currentUser, ViewStatistic viewStatistic)
        {
            if (currentUser != null)
            {
                viewStatistic.RegistredUserViews++;
            }
            viewStatistic.TotalViews++;
        }
    }
}
