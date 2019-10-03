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
    class ElasticServiceTests
    {
        private IHttpWrapper _httpWrapper;
        private Dictionary<string, int> _pingResults;

        [SetUp]
        public void SetUp()
        {
            var successfulResponse = new HttpResponseMessage()
            {
                Content = new StringContent("success"),
                ReasonPhrase = "reasonPhrase",
                StatusCode = HttpStatusCode.OK
            };

            _httpWrapper = Substitute.For<IHttpWrapper>();
            _httpWrapper.SendAsync(Arg.Any<HttpRequestMessage>()).Returns(successfulResponse);

            _pingResults = new Dictionary<string, int>
            {
                {"health0", 1},
                {"health1", 1},
                {"health2", 1},
                {"health3", 0}
            };
        }

        [Test]
        public async Task PostToElastic_FiresOffHttpRequest()
        {
            var elasticService = new ElasticService(_httpWrapper, "elasticUri", "elasticAuth");

            var result = await elasticService.PostToElastic(_pingResults);

            Assert.AreEqual(true, result);

        }

    }
}
