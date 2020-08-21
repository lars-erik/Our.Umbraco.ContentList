using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.DataSources.Listables;
using Our.Umbraco.ContentList.Web;
using Umbraco.Web.Composing;
using Umbraco.Core;
using Umbraco.Core.Dictionary;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Tests.Testing;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Security;

namespace Our.Umbraco.ContentList.Tests.Web
{
    [TestFixture]
    [UmbracoTest(WithApplication = true)]
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

            // First call for backwards compatible templates is not found in tests
            StubNoTemplateFound();

            // Second call for OSS version template is found
            StubTemplateFound();
        }

        private void StubTemplateFound()
        {
            viewEngineMock
                .Setup(v => v.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new ViewEngineResult(Mock.Of<IView>(), viewEngineMock.Object));
        }

        private void StubNoTemplateFound()
        {
            viewEngineMock
                .Setup(v => v.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new ViewEngineResult(new string[0]));
        }

        [Test]
        public void Returns_View_With_Result_From_DataSource()
        {
            var controller = CreateController(emptyResults);
            var parameters = CreateConfiguration();

            var result = controller.List(parameters);
            Assert.That(((ContentListModel)result.Model).Items, Is.EquivalentTo(emptyResults));
        }

        [Test]
        [TestCase("Default")]
        [TestCase("Custom")]
        [Ignore("Should so really fake the filesystem. Gotta test this manually. :/")]
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
            var args = CreateConfiguration();
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
            var args = CreateConfiguration();
            args.View = expectedViewName;

            var result = controller.List(args);
            Assert.AreSame(view, result.View);
        }

        [Test]
        public void Hashes_Query_For_Paging_Identification()
        {
            var controller = CreateController(emptyResults);
            var parameters = CreateConfiguration(true);
            parameters.PageSize = 3;

            var result = controller.List(parameters);

            Assert.AreEqual("6oa2bs/RmP5oMC06vaLh8A", ((ContentListModel)result.Model).Hash);
        }

        [Test]
        public void Pages_Result()
        {
            var expectedData = new List<IPublishedContent>();
            for (var i = 0; i < 20; i++)
                expectedData.Add(StubListableContent().Object);

            var controller = CreateController(expectedData, "6oa2bs/RmP5oMC06vaLh8A=2");
            var parameters = CreateConfiguration(true);
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
            Mock.Get(factory).Setup(f => f.Create(It.IsAny<string>())).Returns(dataSource);
            Mock.Get(dataSource).Setup(s => s.Count(It.IsAny<ContentListQuery>(), 0)).Returns(children.Count);

            var configuration = CreateConfiguration(true);
            configuration.PageSize = 5;
            var controller = CreateController(children, configuration.CreateHash() + "=2", factory);

            controller.List(configuration);

            Mock.Get(dataSource).Verify(s => s.Query(It.IsAny<ContentListQuery>(), Match.Create<QueryPaging>(p => ValidatePagingParameter(p, expectedSkip, expectedPageSize))));
        }

        [Test]
        public void Forwards_Preskip_To_Datasource_Count()
        {
            const int expectedPreSkip = 5;

            var factory = Mock.Of<ListableDataSourceFactory>();
            var dataSource = Mock.Of<IListableDataSource>();
            Mock.Get(factory).Setup(f => f.Create(It.IsAny<string>())).Returns(dataSource);

            var controller = CreateController(new List<IPublishedContent>(), "1", factory);
            var parameters = CreateConfiguration(true);
            parameters.Skip = expectedPreSkip;

            controller.List(parameters);

            Mock.Get(dataSource).Verify(s => s.Count(It.IsAny<ContentListQuery>(), expectedPreSkip));
        }

        [Test]
        public void Forwards_Preskip_To_Datasource_Query()
        {
            const int expectedPreSkip = 5;

            var factory = Mock.Of<ListableDataSourceFactory>();
            var dataSource = Mock.Of<IListableDataSource>();
            Mock.Get(factory).Setup(f => f.Create(It.IsAny<string>())).Returns(dataSource);

            var controller = CreateController(new List<IPublishedContent>(), "1", factory);
            var parameters = CreateConfiguration(true);
            parameters.Skip = expectedPreSkip;

            controller.List(parameters);

            Mock.Get(dataSource).Verify(s => s.Query(It.IsAny<ContentListQuery>(), Match.Create<QueryPaging>(p => p.PreSkip == 5)));
        }

        [Test]
        public void Handles_Incomplete_Configuration_Well()
        {
            Assert.Inconclusive("Really don't - throw nullrefs etc. JSON will (hopefully) 'always' have values.");
        }

        [Test]
        public void Returns_Error_View_When_DataSource_Is_Blank()
        {
            var controller = CreateController(new List<IPublishedContent>(), "1", new ListableDataSourceFactory());
            var parameters = new ContentListConfiguration() { DataSource = new ContentListDataSource { Type = "" } };

            var result = controller.List(parameters);

            Assert.That(result.ViewName, Contains.Substring("Errors.cshtml"));
            Assert.That(result.Model, Is.EquivalentTo(new[]
            {
                "Couldn't find listable datasource ''"
            }));
        }

        [Test]
        public void Returns_Error_View_When_Template_Not_Found()
        {
            viewEngineMock.Reset();
            StubNoTemplateFound();

            var controller = CreateController(new List<IPublishedContent>(), "1", new ListableDataSourceFactory());
            var parameters = new ContentListConfiguration() { DataSource = new ContentListDataSource { Type = typeof(ListableChildrenDataSource).AssemblyQualifiedName } };

            var result = controller.List(parameters);

            Assert.That(result.ViewName, Contains.Substring("Errors.cshtml"));
            Assert.That(result.Model, Is.EquivalentTo(new[]
            {
                "No content list view called  found"
            }));
        }

        private static Mock<IPublishedContent> StubListableContent()
        {
            var listableContentStub = new Mock<IListableContent>().As<IPublishedContent>();
            listableContentStub.Setup(x => x.ContentType).Returns(Mock.Of<IPublishedContentType>());
            return listableContentStub;
        }

        private static bool ValidatePagingParameter(QueryPaging p, int expectedSkip, int expectedTake)
        {
            Console.WriteLine(JsonConvert.SerializeObject(p));
            return p.Skip == expectedSkip && p.Take == expectedTake;
        }

        private ContentListModel CreatePagedModel(int page)
        {
            var children = CreateChildren();

            var query = CreateConfiguration(true);
            query.PageSize = 3;
            var controller = CreateController(children, query.CreateHash() + "=" + page);

            var result = controller.List(query);
            var model = (ContentListModel)result.Model;
            return model;
        }

        private static List<IPublishedContent> CreateChildren()
        {
            var expectedData = new List<IPublishedContent>();
            for (var i = 0; i < 20; i++)
                expectedData.Add(new Mock<IListableContent>().As<IPublishedContent>().Object);
            return expectedData;
        }

        private static ContentListConfiguration CreateConfiguration(bool showPaging = false)
        {
            var parameters = new ContentListConfiguration
            {
                DataSource = new ContentListDataSource
                {
                    Type = typeof(ListableChildrenDataSource).FullName,
                    Parameters = new List<DataSourceParameterValue>()
                },
                PageSize = 10,
                ShowPaging = showPaging,
            };
            return parameters;
        }

        private ContentListController CreateController(List<IPublishedContent> children, string pageParam = "", ListableDataSourceFactory dataSourceFactory = null)
        {
            var publishedContentMock = CreateContent(children);
            var umbracoContext = SetupPublishedRequest(publishedContentMock, "/?" + pageParam);
            var controller = new ContentListController(
                dataSourceFactory ?? new ListableDataSourceFactory(),
                Current.UmbracoContextAccessor,
                Current.Factory.GetInstance<IUmbracoDatabaseFactory>(),
                Current.Services,
                Current.AppCaches,
                Current.Logger,
                Current.ProfilingLogger,
                new UmbracoHelper(
                    publishedContentMock.Object,
                    Mock.Of<ITagQuery>(),
                    Mock.Of<ICultureDictionaryFactory>(),
                    Mock.Of<IUmbracoComponentRenderer>(),
                    Mock.Of<IPublishedContentQuery>(),
                    new MembershipHelper(
                        umbracoContext.HttpContext,
                        Mock.Of<IPublishedMemberCache>(),
                        Mock.Of<MembershipProvider>(),
                        Mock.Of<RoleProvider>(),
                        Mock.Of<IMemberService>(),
                        Mock.Of<IMemberTypeService>(),
                        Mock.Of<IUserService>(),
                        Mock.Of<IPublicAccessService>(),
                        Current.AppCaches,
                        Current.Logger
                    )
                )
            );
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
