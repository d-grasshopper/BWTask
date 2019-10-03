using System.Collections.Generic;
using System.Threading.Tasks;
using HealthPinger.Core;
using HealthPinger.Services;
using NSubstitute;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace HealthPinger.Tests.CoreTests
{
    [TestFixture]
    class HealthCheckerTests
    {
        private IPingHealthEndpoints _healthPingService;
        private IPostToElastic _elasticService;

        [SetUp]
        public void SetUp()
        {
            _healthPingService = Substitute.For<IPingHealthEndpoints>();
            _healthPingService.CheckHealthOfServices().Returns(FakeResults);

            _elasticService = Substitute.For<IPostToElastic>();
            _elasticService.PostToElastic(Arg.Any<Dictionary<string, int>>()).Returns(true);
        }

        [Test]
        public async Task CheckServicesAndPersists_CallsEndpointsAndElastic()
        {
            var healthChecker = new HealthChecker(_healthPingService, _elasticService);

            await healthChecker.CheckServicesAndPersist();

            await _healthPingService.Received(1).CheckHealthOfServices();

            await _elasticService.Received(1).PostToElastic(Arg.Any<Dictionary<string, int>>());
        }

        private Dictionary<string, int> FakeResults { get { return new Dictionary<string, int>
        {
            { "health0", 1 },
            { "health1", 0 }
        };} }
    }
}
