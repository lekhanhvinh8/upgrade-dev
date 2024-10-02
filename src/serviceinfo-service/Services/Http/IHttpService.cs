namespace ServiceInfoService.Sevices.Http
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> SendFormRequestAsync(string url, Dictionary<string, string> formData);
        Task<HttpResponseMessage> SendJsonRequestAsync(string url, object data, IDictionary<string, string>? headers = null);
    }

}