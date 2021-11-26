using System.Collections.Generic;
using System.Threading.Tasks;
using ApprovalTests.Namers;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Models;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    [TestFixtureSource(typeof(TestFixtureModes))]
    class When_Listing_Children_Of_Selected_Nodes : DataSourceTestBase
    {
        public When_Listing_Children_Of_Selected_Nodes(string integrationMode) : base(integrationMode)
        {
        }

        [Test]
        public async Task All_Published_Children_Are_Listed()
        {
            var parameters = new List<DataSourceParameterValue>(DefaultParameters);
            parameters.Add(new DataSourceParameterValue
            {
                Key = "nodes",
                Value = "1001,1002"
            });
            var configuration = CreateConfiguration(typeof(ChildrenOfMultipleDataSource), "SimpleTheme", parameters);
            var result = await Fixture.Execute(configuration, UmbracoContext.Content.GetById(1000));

            using (ApprovalResults.ForScenario(IntegrationMode))
            {
                await Fixture.VerifyResult(result);
            }
        }
    }
}
