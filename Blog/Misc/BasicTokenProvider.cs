using DBModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Misc
{
    public class EMailChangeTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public EMailChangeTokenProviderOptions()
        {
            Name = nameof(BasicTokenProvider);
            TokenLifespan = TimeSpan.FromMinutes(30);
        }
    }

    public class BasicTokenProvider : DataProtectorTokenProvider<User>
    {
        public BasicTokenProvider(
            IDataProtectionProvider dataProtectionProvider,
            IOptions<EMailChangeTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<User>> logger)
            : base(dataProtectionProvider, options, logger)
        {

        }
    }
}
