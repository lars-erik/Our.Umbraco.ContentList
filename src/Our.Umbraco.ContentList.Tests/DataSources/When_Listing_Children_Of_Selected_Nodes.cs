using System.Threading.Tasks;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Models;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    [TestFixtureSource(typeof(IntegrationModes))]
    class When_Listing_Children_Of_Selected_Nodes : DataSourceTestBase
    {
        public When_Listing_Children_Of_Selected_Nodes(string integrationMode) : base(integrationMode)
        {
        }

        [Test]
        public async Task All_Published_Children_Are_Listed()
        {
            var result = await ExecuteSimpleTheme<ChildrenOfMultipleDataSource>(
                CreateParameters(
                    new DataSourceParameterValue
                    {
                        Key = "nodes",
                        Value = "1001,1002"
                    }
                )
            );

            await Fixture.VerifyResult(result, IntegrationMode);
        }
    }
}
