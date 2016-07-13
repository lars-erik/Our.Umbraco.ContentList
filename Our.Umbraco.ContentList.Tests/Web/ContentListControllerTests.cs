using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources.Listables;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core.Models;

namespace Our.Umbraco.ContentList.Tests.Web
{
    [TestFixture]
    public class ContentListControllerTests : BaseControllerTest
    {
        private readonly List<IPublishedContent> emptyResults = new List<IPublishedContent>();
        private Mock<IViewEngine> viewEngineMock;

        [SetUp]
        public void Setup()
        {
            viewEngineMock = new Mock<IViewEngine>();
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(viewEngineMock.Object);
            viewEngineMock
                .Setup(v => v.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>()))
               .Returns(new ViewEngineResult(new string[0]));

            viewEngineMock
                .Setup(v => v.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new ViewEngineResult(Mock.Of<IView>(), viewEngineMock.Object));
        }

        [Test]
        public void Returns_View_With_Result_From_DataSource()
        {
            var controller = CreateController(emptyResults);
            var parameters = CreateParameters();

            var result = controller.List(parameters);
            Assert.That(((ContentListModel)result.Model).Items, Is.EquivalentTo(emptyResults));
        }

        [Test]
        [TestCase("Default")]
        [TestCase("Custom")]
        public void When_Specified_View_Exists_In_Plugin_Folder_Returns_View(string expectedViewName)
        {
            var view = Mock.Of<IView>();

            viewEngineMock.Reset();

            viewEngineMock
                .Setup(v => v.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new ViewEngineResult(new string[0]));

            viewEngineMock.Setup(v => v.FindPartialView(It.IsAny<ControllerContext>(), "~/App_Plugins/Our.Umbraco.ContentList/Views/ListViews/" + expectedViewName, It.IsAny<bool>()))
                .Returns(new ViewEngineResult(view, viewEngineMock.Object));

            var controller = CreateController(emptyResults);
            var args = CreateParameters();
            args.View = expectedViewName;

            var result = controller.List(args);
            Assert.AreSame(view, result.View);
        }

        [Test]
        [TestCase("Default")]
        [TestCase("Custom")]
        public void When_Specified_View_Exists_In_Views_Partials_Folder_Returns_View_And_Ignores_Plugin_Folder(string expectedViewName)
        {
            var pluginFolderView = Mock.Of<IView>();
            var view = Mock.Of<IView>();

            viewEngineMock.Reset();

            viewEngineMock
                .Setup(v => v.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new ViewEngineResult(new string[0]));

            viewEngineMock.Setup(v => v.FindPartialView(It.IsAny<ControllerContext>(), "~/App_Plugins/Our.Umbraco.ContentList/Views/ListViews/" + expectedViewName, It.IsAny<bool>()))
                .Returns(new ViewEngineResult(pluginFolderView, viewEngineMock.Object));

            viewEngineMock.Setup(v => v.FindPartialView(It.IsAny<ControllerContext>(), "~/Views/Partials/ContentList/" + expectedViewName + "/List.cshtml", It.IsAny<bool>()))
                .Returns(new ViewEngineResult(view, viewEngineMock.Object));

            var controller = CreateController(emptyResults);
            var args = CreateParameters();
            args.View = expectedViewName;

            var result = controller.List(args);
            Assert.AreSame(view, result.View);
        }

        [Test]
        public void Pages_Result()
        {
            var expectedData = new List<IPublishedContent>();
            for(var i = 0; i<20; i++)
                expectedData.Add(new Mock<IListableContent>().As<IPublishedContent>().Object);

            var controller = CreateController(expectedData);
            var parameters = CreateParameters(true);
            parameters.PageSize = 3;

            var result = controller.List(parameters);
            var model = (ContentListModel)result.Model;
            Assert.That(model.Items, Is.EquivalentTo(expectedData.Skip(3).Take(3)));
        }

        [Test]
        public void Adds_Paging_Model()
        {
            var model = CreatePagedModel(2);

            var expectedPaging = new ContentListPaging
            {
                Page = 2,
                PageSize = 3,
                From = 4,
                To = 6,
                Total = 20,
                ShowPaging = true
            };

            Assert.AreEqual(JsonConvert.SerializeObject(expectedPaging), JsonConvert.SerializeObject(model.Paging));            
        }

        [Test]
        public void Adds_Paging_Model_With_To_Below_Or_Equal_To_Total()
        {
            var model = CreatePagedModel(7);

            var expectedPaging = new ContentListPaging
            {
                Page = 7,
                PageSize = 3,
                From = 19,
                To = 20,
                Total = 20,
                ShowPaging = true
            };

            Assert.AreEqual(JsonConvert.SerializeObject(expectedPaging), JsonConvert.SerializeObject(model.Paging));            
        }

        [Test]
        public void Adds_Paging_Model_With_Page_Below_Or_Equal_To_Last_Page()
        {
            var model = CreatePagedModel(8);

            var expectedPaging = new ContentListPaging
            {
                Page = 7,
                PageSize = 3,
                From = 19,
                To = 20,
                Total = 20,
                ShowPaging = true
            };

            Assert.AreEqual(JsonConvert.SerializeObject(expectedPaging), JsonConvert.SerializeObject(model.Paging));            
        }

        [Test]
        public void Forwards_Count_Of_Items_To_Skip_And_Take_To_DataSource()
        {
            const int expectedSkip = 5;
            const int expectedPageSize = 5;

            var children = CreateChildren();
            var factory = Mock.Of<ListableDataSourceFactory>();
            var dataSource = Mock.Of<IListableDataSource>();
            Mock.Get(factory).Setup(f => f.Create(It.IsAny<string>(), It.IsAny<QueryParameters>())).Returns(dataSource);
            Mock.Get(dataSource).Setup(s => s.Count(0)).Returns(children.Count);

            var controller = CreateController(children, "2", factory);
            var parameters = CreateParameters(true);
            parameters.PageSize = 5;

            controller.List(parameters);
            
            Mock.Get(dataSource).Verify(s => s.Query(Match.Create<PagingParameter>(p => ValidatePagingParameter(p, expectedSkip, expectedPageSize))));
        }

        [Test]
        public void Forwards_Preskip_To_Datasource_Count()
        {
            const int expectedPreSkip = 5;

            var factory = Mock.Of<ListableDataSourceFactory>();
            var dataSource = Mock.Of<IListableDataSource>();
            Mock.Get(factory).Setup(f => f.Create(It.IsAny<string>(), It.IsAny<QueryParameters>())).Returns(dataSource);

            var controller = CreateController(new List<IPublishedContent>(), "1", factory);
            var parameters = CreateParameters(true);
            parameters.Skip = expectedPreSkip;

            controller.List(parameters);

            Mock.Get(dataSource).Verify(s => s.Count(expectedPreSkip));
        }

        [Test]
        public void Forwards_Preskip_To_Datasource_Query()
        {
            const int expectedPreSkip = 5;

            var factory = Mock.Of<ListableDataSourceFactory>();
            var dataSource = Mock.Of<IListableDataSource>();
            Mock.Get(factory).Setup(f => f.Create(It.IsAny<string>(), It.IsAny<QueryParameters>())).Returns(dataSource);

            var controller = CreateController(new List<IPublishedContent>(), "1", factory);
            var parameters = CreateParameters(true);
            parameters.Skip = expectedPreSkip;

            controller.List(parameters);

            Mock.Get(dataSource).Verify(s => s.Query(Match.Create<PagingParameter>(p => p.PreSkip == 5)));
        }

        private static bool ValidatePagingParameter(PagingParameter p, int expectedSkip, int expectedTake)
        {
            Console.WriteLine(JsonConvert.SerializeObject(p));
            return p.Skip == expectedSkip && p.Take == expectedTake;
        }

        private ContentListModel CreatePagedModel(int page)
        {
            var children = CreateChildren();

            var controller = CreateController(children, page.ToString());
            var parameters = CreateParameters(true);
            parameters.PageSize = 3;

            var result = controller.List(parameters);
            var model = (ContentListModel) result.Model;
            return model;
        }

        private static List<IPublishedContent> CreateChildren()
        {
            var expectedData = new List<IPublishedContent>();
            for (var i = 0; i < 20; i++)
                expectedData.Add(new Mock<IListableContent>().As<IPublishedContent>().Object);
            return expectedData;
        }

        private static ContentListParameters CreateParameters(bool showPaging = false)
        {
            var parameters = new ContentListParameters
            {
                DataSource = typeof(ListableChildrenDataSource).FullName,
                PageSize = 10,
                ShowPaging = showPaging,
                DataSourceParameters = new List<DataSourceParameterValue>()
            };
            return parameters;
        }

        private ContentListController CreateController(List<IPublishedContent> children, string page = "2", ListableDataSourceFactory factory = null)
        {
            var publishedContentMock = CreateContent(children);
            var umbracoContext = SetupPublishedRequest(publishedContentMock, "/?p=" + page);
            var controller = new ContentListController(umbracoContext, factory ?? new ListableDataSourceFactory());
            controller.ControllerContext = new ControllerContext(umbracoContext.HttpContext, new RouteData(), controller);
            return controller;
        }

        private static Mock<IPublishedContent> CreateContent(List<IPublishedContent> children)
        {
            var publishedContentMock = new Mock<IPublishedContent>();
            publishedContentMock.Setup(c => c.Children).Returns(children);
            return publishedContentMock;
        }
    }
}
