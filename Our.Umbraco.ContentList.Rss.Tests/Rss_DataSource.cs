using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Reporters;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using StackExchange.Profiling.Internal;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.Rss.Tests
{
    [TestFixture]
    [UseReporter(typeof(VisualStudioReporter))]
    public class Rss_DataSource
    {
        private readonly ContentListQuery query = new ContentListQuery(
            Mock.Of<IPublishedContent>(), new Dictionary<string, object>
            {
                {"url", "https://blog.aabech.no/rss"}
            }
        );

        private RssDataSource dataSource;

        [SetUp]
        public void Setup()
        {
            var handler = Mock.Of<HttpClientHandler>();
            Mock.Get(handler)
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync<HttpRequestMessage, CancellationToken, HttpClientHandler, HttpResponseMessage>((r, c) =>
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(RssTestData.GetRssStream())
                    }
                );
            var client = new HttpClient(handler);
            dataSource = new RssDataSource(client);
        }

        [Test]
        public void Transforms_Rss_To_ListableContent()
        {
            var items = dataSource.Query(query, new QueryPaging(0, 10));

            Assert.That(items.Count(), Is.EqualTo(10));
            Approvals.VerifyJson(items.ToJson());
        }

        [Test]
        public void Counts_Items_In_Feed()
        {
            var count = dataSource.Count(query, 0);

            Assert.That(count, Is.EqualTo(25));
        }
    }
}
