using System;
using System.Collections.Generic;
using System.Text;
using HealthPinger.Controllers;
using HealthPinger.Core;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace HealthPinger.Tests.ControllerTests
{
    [TestFixture]
    class StartControllerTests
    {
        private ICheckHealthAndPost _healthChecker;

        [SetUp]
        public void SetUp()
        {
            _healthChecker = Substitute.For<ICheckHealthAndPost>();
        }

        [Test]
        public void Get_CallsStartChecking()
        {
            var controller = new StartController(_healthChecker);

            var result = controller.Get();

            _healthChecker.Received(1).StartChecking();
        }
    }
}
