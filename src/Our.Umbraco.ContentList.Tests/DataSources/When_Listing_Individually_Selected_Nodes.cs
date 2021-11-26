using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var parameters = CreateDefaultParameters();
            parameters.Add(new DataSourceParameterValue("nodes", "1001,1003"));
            var result = await ExecuteSimpleTheme(typeof(IndividuallySelectedDataSource), parameters);
            
            using (ApprovalResults.ForScenario(IntegrationMode))
            { 
                await Fixture.VerifyResult(result);
            }
        }
    }

}
