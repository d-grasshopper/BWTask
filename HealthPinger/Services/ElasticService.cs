using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HealthPinger.Services
{
    public class ElasticService : IPostToElastic
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly Uri _elasticUri;
        private readonly string _elasticAuth;

        public ElasticService(string elasticUri, string elasticAuth)
        {
            
            _elasticUri = new Uri("http://" + elasticUri);
            _elasticAuth = elasticAuth;
            var bytearray = Encoding.ASCII.GetBytes(_elasticAuth);
            _httpClient.BaseAddress = _elasticUri;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytearray));
        }

        public async Task<bool> PostToElastic(IEnumerable<KeyValuePair<string, int>> results)
        {

            var request =
                JsonConvert.SerializeObject(new
                {
                    index = new
                    {
                        _index = "healthchecks",
                        _type = "snapshot"
                    }
                }, Formatting.None).ToString() +
                JsonConvert.SerializeObject(results, Formatting.None).ToString();


            request = string.Join("\n", request) + "\n";

            var stringContent = new StringContent(request, Encoding.UTF8, "application/json");
            var httpContent = new HttpRequestMessage(HttpMethod.Post, _elasticUri);
            httpContent.Content = stringContent;
            var response = await _httpClient.SendAsync(httpContent);

            var successful = response.IsSuccessStatusCode;

            return successful;
        }
    }
}
