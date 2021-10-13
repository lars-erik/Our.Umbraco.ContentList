using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTest.Net.Collections;
using Newtonsoft.Json;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Tests.Support;
using Our.Umbraco.ContentList.Web.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Tests.Common.Builders;
using Umbraco.Cms.Tests.Common.Builders.Extensions;

namespace Our.Umbraco.ContentList.Tests
{
    [TestFixture]
    public class When_Listing_Children
    {
        private UmbracoSupport support;

        [SetUp]
        public void Setup()
        {
            var cacheSupport = new ContentCacheSupport();
            var tree = cacheSupport.GetFromJsonResource("Our.Umbraco.ContentList.Tests.site.json");
            support = new UmbracoSupport(tree, Tests.Setup.ContentTypes);
            support.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            support.TearDownUmbraco();
        }

        [Test]
        public void Data_Source_Lists_Children_Of_Requested_Page()
        {
            var ctx = support.GetUmbracoContext();
            var home = ctx.Content.GetById(new Guid("64d01018-3dce-4efb-809f-a9705e166bbd"));

            var dataSource = new ListableChildrenDataSource();
            var result = dataSource.Query(
                new ContentListQuery
                {
                    ContextContent = home
                },
                new QueryPaging(10)
            );

            Console.WriteLine(ToJson(result));

            Assert.That(result.ToList(), Has.Count.EqualTo(2));
            Assert.That(result.First(), Is.TypeOf<Page>());
        }

        [Test]
        public void Gets_Right_Metadata()
        {
            Assert.Inconclusive("Not done yet. Must move to DI.");
        }

        private static string ToJson(IQueryable<IListableContent> result)
        {
            return JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }
    }
}
