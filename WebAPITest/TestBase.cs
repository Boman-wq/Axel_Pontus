using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace WebAPITest
{
    public class TestBase
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public TestContext TestContext { get; set; }

        [TestCleanup]
        public void TestCleanup()
        {
            string testName = $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}";
            UnitTestOutcome currentTestOutcome = TestContext.CurrentTestOutcome;
            string message = $"Test '{testName}' {currentTestOutcome.ToString().ToUpperInvariant()}";
            if (currentTestOutcome != UnitTestOutcome.Passed)
            {
                Logger.Error(message);
            }
            else
            {
                Logger.Info(message);
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            string testName = $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}";
            Logger.Info($"Started with test '{testName}'");
        }
    }
}