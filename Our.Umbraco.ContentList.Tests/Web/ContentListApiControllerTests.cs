using System.IO;
using NUnit.Framework;
using Our.Umbraco.ContentList.Web;
using Umbraco.Tests.TestHelpers;

namespace Our.Umbraco.ContentList.Tests.Web
{
    [TestFixture]
    public class ContentListApiControllerTests : BaseRoutingTest
    {
        private string samplePath = @"..\..\..\Our.Umbraco.ContentList.Web\App_Plugins\Our.Umbraco.ContentList\Views\ContentList\ListViews";

        [Test]
        public void Lists_Sample_If_Theme_Path_Not_Found()
        {
            var expected = new[] {"Sample"};
            var path = @"..\..\..\Our.Umbraco.ContentList.Web\Views\Partials\NotFoundContentList";
            AssertTemplates(path, expected);
        }

        [Test]
        public void Lists_Sample_If_No_Theme_Folders()
        {
            var expected = new[] {"Sample"};
            var path = @".";
            AssertTemplates(path, expected);
        }

        private void AssertTemplates(string themePath, string[] expected)
        {
            var controller = new ContentListApiController(GetUmbracoContext("/", -1), Path.GetFullPath(themePath), Path.GetFullPath(samplePath));
            var templates = controller.ListTemplates();
            Assert.That(templates, Is.EquivalentTo(expected));
        }
    }
}
