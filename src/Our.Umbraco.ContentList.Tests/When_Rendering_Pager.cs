using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Namers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using Our.Umbraco.ContentList.Controllers;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Tests.Support;

namespace Our.Umbraco.ContentList.Tests
{
    [TestFixture]
    public class When_Rendering_Pager
    {
        private TestServerFactory serverFactory;
        private HttpRequest request;
        private ContentListQueryHandler queryHandler;
        private ViewRenderer viewRenderer;
        private Dictionary<string, StringValues> queryValues;
        private IHttpContextAccessor contextAccessor;

        [SetUp]
        public void SetUp()
        {
            serverFactory = new TestServerFactory();
            viewRenderer = serverFactory.GetRequiredService<ViewRenderer>();
            queryHandler = serverFactory.GetRequiredService<ContentListQueryHandler>();
            contextAccessor = serverFactory.GetRequiredService<IHttpContextAccessor>();

            queryValues = new Dictionary<string, StringValues>{{"unrelated", new StringValues("xyz")}};
            //Mock.Get(request).Setup(x => x.Query).Returns(() => n);
            //Mock.Get(ctx).Setup(x => x.RequestServices).Returns(serverFactory.Services);
            

        }

        private void CreateHttpContext()
        {
            var ctx = new DefaultHttpContext
            {
                Request =
                {
                    Scheme = "https",
                    Host = new HostString("example.com"),
                    Path = new PathString("/a/b"),
                    Query = new QueryCollection(queryValues)
                },
                RequestServices = serverFactory.Services
            };

            Mock.Get(contextAccessor).Setup(x => x.HttpContext).Returns(ctx);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task On_Pages_One_Through_Three_Renders_One_Through_Five(int page)
        {
            await VerifyPager(page);
        }

        [Test]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        public async Task On_Pages_Eight_Through_Ten_Renders_Six_Through_Ten(int page)
        {
            await VerifyPager(page);
        }

        private async Task VerifyPager(int page)
        {
            var config = new ContentListConfiguration
            {
                ShowPaging = true,
                PageSize = 10,
            };

            queryValues.Add(config.CreateHash(), new StringValues(page.ToString()));
            CreateHttpContext();

            var pagerModel = CreatePagingModel(config);
            var model = new ContentListModel
            {
                Configuration = config,
                Paging = pagerModel
            };
            
            var result = await viewRenderer.Render("PagerTest", model);

            Console.WriteLine(result);

            using (ApprovalResults.ForScenario(page))
            {
                Approvals.VerifyHtml(result);
            }
        }

        private ContentListPaging CreatePagingModel(ContentListConfiguration config)
        {
            var queryPaging = queryHandler.CreateQueryPaging(config, 100);
            var pagerModel = ContentListViewComponent.CreatePagingModel(queryPaging, config, 100);
            return pagerModel;
        }
    }
}
