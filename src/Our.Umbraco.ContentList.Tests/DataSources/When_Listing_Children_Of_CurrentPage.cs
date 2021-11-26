using System.Threading.Tasks;
using ApprovalTests.Namers;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;

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
            var result = await ExecuteSimpleTheme<ChildrenOfCurrentPageDataSource>();

            using (ApprovalResults.ForScenario(IntegrationMode))
            { 
                await Fixture.VerifyResult(result);
            }
        }
    }
}
