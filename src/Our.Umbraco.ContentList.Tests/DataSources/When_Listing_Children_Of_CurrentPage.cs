using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ApprovalTests.Namers;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Web.Models;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    [TestFixtureSource(typeof(IntegrationModes))]
    public class When_Listing_Children_Of_CurrentPage : DataSourceTestBase
    {
        public When_Listing_Children_Of_CurrentPage(string integrationMode) : base(integrationMode)
        {
        }

        [Test]
        public async Task All_Published_Children_Are_Listed()
        {
            var configuration = CreateConfiguration(typeof(ChildrenOfCurrentPageDataSource), "SimpleTheme");
            var result = await Fixture.Execute(configuration, UmbracoContext.Content.GetById(1000));

            using (ApprovalResults.ForScenario(IntegrationMode))
            { 
                await Fixture.VerifyResult(result);
            }
        }
    }
}
