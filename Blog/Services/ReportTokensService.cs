using DBModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class ReportViewConfirmationTokenProvider : DataProtectorTokenProvider<User> 
    {
        public ReportViewConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<ReportViewConfirmationTokenProviderOptions> options) : base(dataProtectionProvider, options)
        {

        }
    }

    public class ReportViewConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public ReportViewConfirmationTokenProviderOptions()
        {
            Name = nameof(ReportViewConfirmationTokenProvider);
            TokenLifespan = TimeSpan.FromHours(24);
        }
    }
}
