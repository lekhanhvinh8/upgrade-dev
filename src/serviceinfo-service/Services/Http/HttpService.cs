using System.Text;

namespace ServiceInfoService.Sevices.Http
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendFormRequestAsync(string url, Dictionary<string, string> formData)
        {
            var content = new FormUrlEncodedContent(formData);

            var response = await _httpClient.PostAsync(url, content);
            return response;
        }

        public async Task<HttpResponseMessage> SendJsonRequestAsync(string url, object data, IDictionary<string, string>? headers = null)
        {
            // Set additional headers, if provided
             _httpClient.DefaultRequestHeaders.Clear();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            // Serialize the data to JSON
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            // Create the request content with JSON data
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Send the POST request asynchronously
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            return response;
        }
    }

}