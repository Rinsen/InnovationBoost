using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Rinsen.IdentityProvider.Token
{
    public class TokenPostConfigureOptions : IPostConfigureOptions<TokenOptions>
    {
        private readonly IDataProtectionProvider _dp;

        public TokenPostConfigureOptions(IDataProtectionProvider dataProtection)
        {
            _dp = dataProtection;
        }


        public void PostConfigure(string name, TokenOptions options)
        {
            options.DataProtectionProvider = options.DataProtectionProvider ?? _dp;



            if (options.Backchannel == null)
            {
                options.Backchannel = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler());
                options.Backchannel.DefaultRequestHeaders.UserAgent.ParseAdd("Rinsen RemoteTokenHandler");
                options.Backchannel.Timeout = options.BackchannelTimeout;
                options.Backchannel.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB
            }
        }
    }
}
