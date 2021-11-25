using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Parameters;
using Our.Umbraco.ContentList.Tests.Support;
using Our.Umbraco.ContentList.Web.Models;
using Umbraco.Cms.Core.PublishedCache;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    [TestFixture]
    class When_Listing_Children_Of_Selected_Nodes
    {
        private UmbracoSupport support;

        [SetUp]
        public void Setup()
        {
            var cacheSupport = new ContentCacheSupport();
            var tree = cacheSupport.GetFromJsonResource(GetType().FullName);
            support = new UmbracoSupport(tree, Tests.Setup.ContentTypes);
            support.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            support.TearDownUmbraco();
        }

        [Test]
        public void All_Published_Children_Are_Listed()
        {
            var ctx = support.GetUmbracoContext();
            var home = ctx.Content.GetById(1000);

            var dataSource = new ChildrenOfMultipleDataSource(support.GetService<IPublishedSnapshotAccessor>());
            var result = dataSource.Query(
                new ContentListQuery
                {
                    ContextContent = home,
                    CustomParameters =
                    {
                        { ListableSorting.Parameter.Key, ListableSorting.Parameter.Config.Items[0].Value },
                        { "nodes", "1001,1002" }
                    }
                },
                new QueryPaging(10)
            );

            Assert.That(result.ToList(), Has.Count.EqualTo(4));
            Assert.That(result.First(), Is.TypeOf<Page>());

            Approvals.VerifyJson(result.ToJson());

        }

    }
}
