using System.Net.Http;
using System.Threading.Tasks;

namespace HealthPinger.Services
{
    public interface IHttpWrapper
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage);

        void SetBaseAddress(string baseAddress);

        void SetBasicAuth(string basicAuth);
    }
}
