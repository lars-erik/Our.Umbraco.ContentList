using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.DataSources.Listables;
using Our.Umbraco.ContentList.Install;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Tests.TestHelpers;
using Umbraco.Tests.Testing;
using Umbraco.UnitTesting.Adapter;

namespace Our.Umbraco.ContentList.Tests.DataSources.Listables
{
    [TestFixture]
    [UmbracoTest(WithApplication=true)]
    public class ListableChildrenDataSourceTests
    {
        private UmbracoSupport umbracoSupport;

        [SetUp]
        public void Setup()
        {
            umbracoSupport = new UmbracoSupport();
            umbracoSupport.SetupUmbraco(composition =>
            {
                new ListableDataSourcesComposer().Compose(composition);
            });
        }

        [TearDown]
        public void TearDown()
        {
            umbracoSupport.DisposeUmbraco();
        }

        [Test]
        public void Returns_Children_Of_Current_Content_Implementing_IListableContent()
        {
            var contextContentMock = new Mock<IPublishedContent>();
            var listableChildMock = new Mock<IListableContent>();
            var children = new List<IPublishedContent>
            {
                StubPublishedContent(listableChildMock).Object,
                Mock.Of<IPublishedContent>()
            };
            contextContentMock.Setup(c => c.Children).Returns(children);

            var dataSource = new ListableChildrenDataSource();

            Assert.That(dataSource.Query(new ContentListQuery(contextContentMock.Object), new QueryPaging()), Is.EquivalentTo(new[] { listableChildMock.Object }));
        }

        [Test]
        [TestCase("sortorder", "b")]
        [TestCase("dateasc", "b")]
        [TestCase("datedesc", "a")]
        public void Orders_Result_By_Parameter(string parameter, string expectedFirstTitle)
        {
            #region mocks

            var contextContentMock = new Mock<IPublishedContent>();
            var listableChildMock = new Mock<IListableContent>();
            var listableChild2Mock = new Mock<IListableContent>();
            var children = new List<IPublishedContent>
            {
                StubPublishedContent(listableChildMock).Object,
                StubPublishedContent(listableChild2Mock).Object
            };

            contextContentMock.Setup(c => c.Children).Returns(children);
            listableChildMock.Setup(c => c.ListHeading).Returns("b");
            listableChild2Mock.Setup(c => c.ListHeading).Returns("a");
            listableChildMock.Setup(c => c.SortOrder).Returns(1);
            listableChild2Mock.Setup(c => c.SortOrder).Returns(2);
            listableChildMock.Setup(c => c.SortDate).Returns(DateTime.Now.AddHours(-1));
            listableChild2Mock.Setup(c => c.SortDate).Returns(DateTime.Now);

            #endregion

            var dataSource = new ListableChildrenDataSource();
            var result = dataSource.Query(new ContentListQuery(contextContentMock.Object, new Dictionary<string, object> { { "sort", parameter } }), new QueryPaging());

            Assert.That(result.First().ListHeading, Is.EqualTo(expectedFirstTitle));
        }

        [Test]
        public void Counts_Children()
        {
            var contextContent = Mock.Of<IPublishedContent>();
            Mock.Get(contextContent).Setup(c => c.Children).Returns(new[]
            {
                new Mock<IListableContent>().As<IPublishedContent>().Object,
                new Mock<IListableContent>().As<IPublishedContent>().Object
            });

            var dataSource = new ListableChildrenDataSource();
            
            Assert.AreEqual(2, dataSource.Count(new ContentListQuery(contextContent), 0));
        }

        private static Mock<IPublishedContent> StubPublishedContent(Mock<IListableContent> listableChildMock)
        {
            var mock = listableChildMock.As<IPublishedContent>();
            mock.Setup(x => x.ContentType).Returns(Mock.Of<IPublishedContentType>());
            return mock;
        }
    }
}
