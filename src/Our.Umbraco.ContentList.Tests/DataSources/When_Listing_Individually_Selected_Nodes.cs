using System.Threading.Tasks;
using ApprovalTests.Namers;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Models;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    [TestFixtureSource(typeof(IntegrationModes))]
    public class When_Listing_Individually_Selected_Nodes : DataSourceTestBase
    {
        public When_Listing_Individually_Selected_Nodes(string integrationMode) : base(integrationMode)
        {
        }

        [Test]
        public async Task Lists_All_Selected_Nodes()
        {
            var result = await ExecuteSimpleTheme<IndividuallySelectedDataSource>(
                CreateParameters(
                    new DataSourceParameterValue("nodes", "1001,1003")
                )
            );
            
            using (ApprovalResults.ForScenario(IntegrationMode))
            { 
                await Fixture.VerifyResult(result);
            }
        }
    }

}
