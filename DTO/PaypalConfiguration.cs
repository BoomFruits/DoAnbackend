using Microsoft.Extensions.Configuration;
using PayPal.Api;
using System.Collections.Generic;

namespace   DoAn.DTO
{
    public class PaypalConfiguration
    {
        private readonly IConfiguration _configuration;

        public PaypalConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Dictionary<string, string> GetConfig()
        {
            return new Dictionary<string, string>
            {
                { "mode", "sandbox" }, // hoặc "live"
                { "clientId", _configuration["PaypalOptions:AppID"] },
                { "clientSecret", _configuration["PaypalOptions:AppSecret"] }
            };
        }

        public APIContext GetAPIContext()
        {
            string clientId = _configuration["PaypalOptions:AppID"];
            string clientSecret = _configuration["PaypalOptions:AppSecret"];

            string accessToken = new OAuthTokenCredential(clientId, clientSecret, GetConfig()).GetAccessToken();

            var apiContext = new APIContext(accessToken)
            {
                Config = GetConfig()
            };

            return apiContext;
        }
    }
}
