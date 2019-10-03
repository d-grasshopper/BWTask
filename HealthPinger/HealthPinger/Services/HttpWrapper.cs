using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HealthPinger.Services
{
    public class HttpWrapper : IHttpWrapper
    {
        private readonly HttpClient _httpClient;

        public HttpWrapper()
        {
            _httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            var result = await _httpClient.GetAsync(requestUri);

            return result;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
        {
            var result = await _httpClient.SendAsync(httpRequestMessage);

            return result;
        }

        public void SetBaseAddress(string baseAddress)
        {
            var uri = new Uri("http://" + baseAddress);
            _httpClient.BaseAddress = uri;
        }

        public void SetBasicAuth(string basicAuth)
        {
            var bytearray = Encoding.ASCII.GetBytes(basicAuth);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytearray));
        }
    }
}
