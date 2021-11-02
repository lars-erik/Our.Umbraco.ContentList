using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Namers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using Our.Umbraco.ContentList.Components;
using Our.Umbraco.ContentList.Controllers;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Tests.Support;

namespace Our.Umbraco.ContentList.Tests
{
    [TestFixture]
    public class When_Rendering_Pager
    {
        private UmbracoSupport support;

        [SetUp]
        public void SetUp()
        {
            support = new UmbracoSupport(configureServices: services =>
            {
                services.AddTransient<ContentListQueryHandler>();
                services.AddTransient<PagerComponent>();

                services.AddRenderingSupport();
            });
            support.Setup();

        }

        [TearDown]
        public void TearDown()
        {
            support?.TearDownUmbraco();
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

            SetupRequestPath(page, config);

            var model = new ContentListModel
            {
                Configuration = config,
                Paging = CreatePagingModel(config)
            };
            
            var result = await support.GetService<ViewRenderer>().Render("PagerTest", model);

            Console.WriteLine(result);

            using (ApprovalResults.ForScenario(page))
            {
                Approvals.VerifyHtml(result);
            }
        }

        private void SetupRequestPath(int page, ContentListConfiguration config)
        {
            support.ReplaceHttpRequest(
                "/a/b",
                new Dictionary<string, StringValues>
                {
                    {"unrelated", new StringValues("xyz")},
                    {config.CreateHash(), new StringValues(page.ToString())}
                }
            );
        }

        private ContentListPaging CreatePagingModel(ContentListConfiguration config)
        {
            var queryPaging = support.GetService<ContentListQueryHandler>().CreateQueryPaging(config, 100);
            var pagerModel = ContentListViewComponent.CreatePagingModel(queryPaging, config, 100);
            return pagerModel;
        }
    }
}
