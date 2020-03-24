using NUnit.Framework;
using Umbraco.Tests.Testing;
using Umbraco.UnitTesting.Adapter;

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
