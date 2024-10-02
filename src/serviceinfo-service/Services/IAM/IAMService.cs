using System.Text.Json.Serialization;
using Newtonsoft.Json;
using ServiceInfoService.Sevices.IAM;

namespace ServiceInfoService.Sevices.Http
{
    public class IAMService : IIAMService
    {
        private static DateTime CacheTime = DateTime.Now;
        private static string? CacheToken = null;
        private static int IAMCacheSecond = 0;

        private readonly IHttpService _httpService;

        public IAMService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<string?> GetCachedToken()
        {
            if(CacheToken != null && CacheTime.AddSeconds(IAMCacheSecond + 20) >= DateTime.Now)
            {
                return CacheToken;
            }

            var response = await _httpService.SendFormRequestAsync("https://localhost:5144/connect/token", new Dictionary<string, string>() 
            {
                { "grant_type", "client_credentials"},
                { "client_id", "mym2mclient"},
                { "client_secret", "secret"},
            });

            if(response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var responseObj = JsonConvert.DeserializeObject<TokenResponse>(responseData);

                if(responseObj != null)
                {
                    CacheTime = DateTime.Now;
                    CacheToken = responseObj.access_token;
                    IAMCacheSecond = (int)responseObj.expires_in!;

                    return CacheToken;
                }
            }

            return null;
        }

        public class TokenResponse
        {
            public string? access_token { get; set; }
            public int? expires_in { get; set; }
            public string? token_type { get; set; }
            public string? scope { get; set; }
        }
    }
}