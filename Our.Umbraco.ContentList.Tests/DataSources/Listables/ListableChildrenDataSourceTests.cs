using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.DataSources.Listables;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Tests.TestHelpers;

namespace Our.Umbraco.ContentList.Tests.DataSources.Listables
{
    [TestFixture]
    public class ListableChildrenDataSourceTests : BaseRoutingTest
    {
        [Test]
        public void Returns_Children_Of_Current_Content_Implementing_IListableContent()
        {
            var contextContentMock = new Mock<IPublishedContent>();
            var listableChildMock = new Mock<IListableContent>();
            var children = new List<IPublishedContent>
            {
                listableChildMock.As<IPublishedContent>().Object,
                Mock.Of<IPublishedContent>()
            };
            contextContentMock.Setup(c => c.Children).Returns(children);

            var dataSource = new ListableChildrenDataSource(new QueryParameters(contextContentMock.Object));

            Assert.That(dataSource.Query(new PagingParameter()), Is.EquivalentTo(new[] { listableChildMock.Object }));
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
                listableChildMock.As<IPublishedContent>().Object,
                listableChild2Mock.As<IPublishedContent>().Object
            };

            contextContentMock.Setup(c => c.Children).Returns(children);
            listableChildMock.Setup(c => c.Name).Returns("b");
            listableChild2Mock.Setup(c => c.Name).Returns("a");
            listableChildMock.Setup(c => c.SortOrder).Returns(1);
            listableChild2Mock.Setup(c => c.SortOrder).Returns(2);
            listableChildMock.Setup(c => c.CreateDate).Returns(DateTime.Now.AddHours(-1));
            listableChild2Mock.Setup(c => c.CreateDate).Returns(DateTime.Now);

            #endregion

            var dataSource = new ListableChildrenDataSource(new QueryParameters(contextContentMock.Object, new Dictionary<string, string> { { "sort", parameter } }));
            var result = dataSource.Query(new PagingParameter());

            Assert.That(result.First().Name, Is.EqualTo(expectedFirstTitle));
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

            var dataSource = new ListableChildrenDataSource(new QueryParameters(contextContent));
            
            Assert.AreEqual(2, dataSource.Count(0));
        }
    }
}
