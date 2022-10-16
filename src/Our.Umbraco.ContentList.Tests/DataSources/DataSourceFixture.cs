using System.Threading.Tasks;
using Our.Umbraco.ContentList.Controllers;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Tests.Support;
using Umbraco.Cms.Core.Models.PublishedContent;
using VerifyNUnit;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    public class DataSourceFixture : FixtureAbstraction
    {
        private UmbracoSupport support;

        public DataSourceFixture(UmbracoSupport support)
        {
            this.support = support;
        }

        public override async Task<object> Execute(ContentListConfiguration configuration, IPublishedContent currentPage)
        {
            var queryHandler = support.GetService<ContentListQueryHandler>();
            var query = queryHandler.CreateQuery(configuration, currentPage);
            var dataSource = queryHandler.CreateDataSource(configuration);
            var total = await dataSource.Count(query, configuration.Skip);
            var queryPaging = queryHandler.CreateQueryPaging(configuration, total);
            var result = await dataSource.Query(query, queryPaging);
            return result;

        }

        public override async Task VerifyResult(object resultObject, string scenario = null)
        {
            var verifyTask = Verifier.Verify(target: resultObject.ToJson(true), extension: "json");
            if (scenario != null)
                verifyTask = verifyTask.UseParameters(scenario);
            await verifyTask;
        }
    }
}