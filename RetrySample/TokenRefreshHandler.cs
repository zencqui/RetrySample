using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RetrySample
{
    public class TokenRefreshHandler : DelegatingHandler
    {
        private int _retryCount = 0;
        private int _maxRetryCount = 3;
        private TimeSpan _retryInterval = TimeSpan.FromSeconds(2);

        public TokenRefreshHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {

        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            for (_retryCount = 0; _retryCount < _maxRetryCount; _retryCount++)
            {
                response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    break;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Renew the token and update the request header
                    string newToken = await RenewTokenAsync();
                    request.Headers.Remove("Authorization");
                    request.Headers.Add("Authorization", "Bearer " + newToken);
                }
                else
                {
                    break;
                }

                await Task.Delay(_retryInterval, cancellationToken);
            }

            return response;
        }

        private async Task<string> RenewTokenAsync()
        {
            // Renew the token here
            // ...

            return "newToken";
        }
    }
}
