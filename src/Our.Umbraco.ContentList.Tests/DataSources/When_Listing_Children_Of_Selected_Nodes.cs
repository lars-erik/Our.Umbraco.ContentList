using System.Linq;
using ApprovalTests;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Tests.Support;
using Our.Umbraco.ContentList.Web.Models;

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

            var level1Children = home.Children;
            Approvals.VerifyJson(level1Children.ToJson());
            
            Assert.Fail("Haven't started the data source. 😇");
            //var dataSource = new ListableChildrenDataSource();
            //var result = dataSource.Query(
            //    new ContentListQuery
            //    {
            //        ContextContent = home
            //    },
            //    new QueryPaging(10)
            //);

            //Assert.That(result.ToList(), Has.Count.EqualTo(2));
            //Assert.That(result.First(), Is.TypeOf<Page>());

            //Approvals.VerifyJson(result.ToJson());

        }

    }
}
