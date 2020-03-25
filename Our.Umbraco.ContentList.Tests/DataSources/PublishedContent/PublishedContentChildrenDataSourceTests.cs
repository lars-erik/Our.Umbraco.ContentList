using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.DataSources.Listables;
using Our.Umbraco.ContentList.DataSources.PublishedContent;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.Tests.DataSources.PublishedContent
{
    [TestFixture]
    public class PublishedContentChildrenDataSourceTests
    {
        [Test]
        public void Returns_Children_Of_Current_Content()
        {
            var contextContentMock = new Mock<IPublishedContent>();
            var children = new[] {
                StubPublishedContent(),
                Mock.Of<IPublishedContent>()
            };
            contextContentMock.Setup(c => c.Children).Returns(children);

            var dataSource = new PublishedContentChildrenDataSource(new QueryParameters(contextContentMock.Object));

            Assert.That(dataSource.Query(new PagingParameter()), Is.EquivalentTo(children));
        }

        [Test]
        [TestCase("sortorder", "b")]
        [TestCase("dateasc", "b")]
        [TestCase("datedesc", "a")]
        public void Orders_Result_By_Parameter(string parameter, string expectedFirstTitle)
        {
            #region mocks

            var contextContentMock = new Mock<IPublishedContent>();
            var child1Mock = new Mock<IPublishedContent>();
            var child2Mock = new Mock<IPublishedContent>();
            var children = new List<IPublishedContent>
            {
                child1Mock.Object,
                child2Mock.Object
            };

            contextContentMock.Setup(c => c.Children).Returns(children);
            child1Mock.Setup(c => c.Name).Returns("b");
            child2Mock.Setup(c => c.Name).Returns("a");
            child1Mock.Setup(c => c.SortOrder).Returns(1);
            child2Mock.Setup(c => c.SortOrder).Returns(2);
            child1Mock.Setup(c => c.CreateDate).Returns(DateTime.Now.AddHours(-1));
            child2Mock.Setup(c => c.CreateDate).Returns(DateTime.Now);

            #endregion

            var dataSource = new PublishedContentChildrenDataSource(new QueryParameters(contextContentMock.Object, new Dictionary<string, string> { { "sort", parameter } }));
            var result = dataSource.Query(new PagingParameter());

            Assert.That(result.First().Name, Is.EqualTo(expectedFirstTitle));
        }

        [Test]
        public void Counts_Children()
        {
            var contextContent = Mock.Of<IPublishedContent>();
            Mock.Get(contextContent).Setup(c => c.Children).Returns(new[]
            {
                Mock.Of<IPublishedContent>(),
                Mock.Of<IPublishedContent>()
            });

            var dataSource = new PublishedContentChildrenDataSource(new QueryParameters(contextContent));

            Assert.AreEqual(2, dataSource.Count(0));
        }

        private static IPublishedContent StubPublishedContent()
        {
            var content = Mock.Of<IPublishedContent>();
            Mock.Get(content).Setup(x => x.ContentType).Returns(Mock.Of<IPublishedContentType>());
            return content;
        }
    }
}
