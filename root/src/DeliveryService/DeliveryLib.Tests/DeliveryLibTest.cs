using System.Threading;
using NUnit.Framework;
using DeliveryLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.DotNet.InternalAbstractions;

namespace DeliveryLib.Tests
{
    [TestFixture]
    public class DeliveryLibTest
    {
        private DeliveryMessage _service = null;

        [SetUp]
        public void Init()
        {
            _service = new DeliveryMessage(
                "https://api.telegram.org/bot",
                "325834582:AAGbvJQ7oYH2E3HW4z_yqshRvu-jT53GOQc");
        }


        [Test]
        public void TestSpamMessage()
        {
            _service.SendMessageAsync("148460428", "test spam message", default(CancellationToken)).Wait();
        }
    }
}
