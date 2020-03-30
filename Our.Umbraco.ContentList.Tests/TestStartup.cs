using ApprovalTests.Namers;
using NUnit.Framework;
using Umbraco.UnitTesting.Adapter;

[assembly:UseApprovalSubdirectory("Approvals")]

namespace Our.Umbraco.ContentList.Tests
{
    [SetUpFixture]
    public class TestStartup
    {
        [OneTimeSetUp]
        public static void RegisterWithUmbracoTests()
        {
            UmbracoSupport.RegisterForTesting<TestStartup>();
        }
    }
}
