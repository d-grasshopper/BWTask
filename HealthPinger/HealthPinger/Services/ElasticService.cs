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
        private readonly IHttpWrapper _httpClient;
        private readonly Uri _elasticUri;

        public ElasticService(IHttpWrapper httpWrapper, string elasticUri, string elasticAuth)
        {
            _elasticUri = new Uri("http://" + elasticUri);
            _httpClient = httpWrapper;
            _httpClient.SetBaseAddress(elasticUri);
            _httpClient.SetBasicAuth(elasticAuth);
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
