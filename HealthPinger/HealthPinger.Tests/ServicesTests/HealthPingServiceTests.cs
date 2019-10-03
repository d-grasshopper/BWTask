using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HealthPinger.Services;
using NSubstitute;
using NUnit.Framework;

namespace HealthPinger.Tests.ServicesTests
{
    [TestFixture]
    class HealthPingServiceTests
    {
        private IHttpWrapper _httpClient;
        private Dictionary<string, string> _locationServices;

        [SetUp]
        public void SetUp()
        {
            _httpClient = Substitute.For<IHttpWrapper>();
            _httpClient
                .GetAsync("http://health0/health")
                .Returns(CreateFakeResponse(HttpStatusCode.OK, SuccessfulContent));

            _httpClient
                .GetAsync("http://health1/health")
                .Returns(CreateFakeResponse(HttpStatusCode.InternalServerError, EmptyContent));

            _locationServices = new Dictionary<string, string>
            {
                {"health0", "health0"},
                {"health1", "health1"}
            };
        }

        [Test]
        public async Task CheckHealthOfServices_ReturnsFilledDict()
        {
            var healthPingService = new HealthPingService(_httpClient, _locationServices);

            var result = await healthPingService.CheckHealthOfServices();

            Assert.AreEqual(result["health0"], 1);
            Assert.AreEqual(result["health1"], 0);
        }

        private HttpResponseMessage CreateFakeResponse(HttpStatusCode statusCode, HttpContent content)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = content
            };

            return response;
        }

        private HttpContent SuccessfulContent { get { return new StringContent("{\"status\": \"up\"}"); } }

        private HttpContent EmptyContent { get { return new StringContent("");} }
    }
}
