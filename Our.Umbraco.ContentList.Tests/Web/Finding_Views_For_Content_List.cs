using System.IO;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Our.Umbraco.ContentList.Web;

namespace Our.Umbraco.ContentList.Tests.Web
{
    [TestFixture]
    public class Finding_Views_For_Content_List
    {
        private Mock<IViewEngine> engineMock;
        private IView view;

        [SetUp]
        public void Setup()
        {
            StubViewEngine();

            view = Mock.Of<IView>();
        }

        [Test]
        [TestCase("Default")]
        [TestCase("Custom Template")]
        public void Finds_Default_List_Item_View_In_Template_Folder(string expectedTemplateName)
        {
            StubViewResult("~/Views/Partials/ContentList/" + expectedTemplateName + "/ListItem.cshtml", view);

            var model = new ContentListModel { Query = new ContentListQuery {View = expectedTemplateName} };
            var helper = CreateHtmlHelper(model);

            var listItem = Mock.Of<IListableContent>();

            Mock.Get(view)
                .Setup(v => v.Render(Match.Create<ViewContext>(c => c.ViewData.Model == listItem), It.IsAny<StringWriter>()))
                .Verifiable();

            var result = helper.ContentListItem(listItem);

            Mock.Get(view).VerifyAll();
        }

        [Test]
        [TestCase("Default", "FancyType")]
        [TestCase("Custom Template", "MoreFancyType")]
        public void Finds_Custom_List_Item_View_Of_DocType_Name_In_Template_Folder(string expectedTemplateName, string documentType)
        {
            StubViewResult("~/Views/Partials/ContentList/" + expectedTemplateName + "/" + documentType + ".cshtml", view);

            var model = new ContentListModel { Query = new ContentListQuery {View = expectedTemplateName} };
            var helper = CreateHtmlHelper(model);

            var listItem = Mock.Of<IListableContent>();
            Mock.Get(listItem).Setup(i => i.DocumentTypeAlias).Returns(documentType);

            Mock.Get(view)
                .Setup(v => v.Render(Match.Create<ViewContext>(c => c.ViewData.Model == listItem), It.IsAny<StringWriter>()))
                .Verifiable();

            var result = helper.ContentListItem(listItem);

            Mock.Get(view).VerifyAll();
        }

        private static HtmlHelper CreateHtmlHelper(ContentListModel model)
        {
            var controllerContext = new ControllerContext();
            var parentView = Mock.Of<IView>();
            var parentViewContext = new ViewContext(controllerContext, parentView, new ViewDataDictionary {Model = model}, new TempDataDictionary(), TextWriter.Null);
            var helper = new HtmlHelper(parentViewContext, new ViewPage());
            return helper;
        }

        private void StubViewResult(string viewPath, IView viewToReturn)
        {
            engineMock
                .Setup(e => e.FindPartialView(It.IsAny<ControllerContext>(), viewPath, It.IsAny<bool>()))
                .Returns(new ViewEngineResult(viewToReturn, engineMock.Object));
        }

        private void StubViewEngine()
        {
            engineMock = new Mock<IViewEngine>();

            engineMock
                .Setup(e => e.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new ViewEngineResult(new string[0]));

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(engineMock.Object);
        }
    }
}
