using System;
using System.IO;
using System.Web.Security;
using Moq;
using NUnit.Framework;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core;
using Umbraco.Core.Dictionary;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web.Composing;
using Umbraco.Tests.TestHelpers;
using Umbraco.Tests.Testing;
using Umbraco.UnitTesting.Adapter;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using Umbraco.Web.Security;

namespace Our.Umbraco.ContentList.Tests.Web
{
    [TestFixture]
    [UmbracoTest(WithApplication=true)]
    public class ContentListApiControllerTests
    {
        private string samplePath;
        private UmbracoSupport UmbracoSupport;

        [SetUp]
        public void Setup()
        {
            UmbracoSupport = new UmbracoSupport();
            UmbracoSupport.SetupUmbraco();

            samplePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Our.Umbraco.ContentList.Web\App_Plugins\Our.Umbraco.ContentList\Views\ContentList\ListViews");
        }

        [TearDown]
        public void TearDown()
        {
            UmbracoSupport.DisposeUmbraco();
        }

        [Test]
        public void Lists_Sample_If_Theme_Path_Not_Found()
        {
            var expected = new[] {"Sample"};
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"\..\..\..\Our.Umbraco.ContentList.Web\Views\Partials\NotFoundContentList");
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
            var controller = new ContentListApiController(
                Path.GetFullPath(themePath), 
                Path.GetFullPath(samplePath),
                Current.Configs.Global(),
                Current.UmbracoContextAccessor,
                Current.SqlContext,
                Current.Services,
                Current.AppCaches,
                Current.ProfilingLogger,
                Current.RuntimeState,
                new UmbracoHelper(
                    Mock.Of<IPublishedContent>(),
                    Mock.Of<ITagQuery>(),
                    Mock.Of<ICultureDictionaryFactory>(),
                    Mock.Of<IUmbracoComponentRenderer>(),
                    Mock.Of<IPublishedContentQuery>(),
                    new MembershipHelper(
                        Current.UmbracoContext.HttpContext,
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
            var templates = controller.ListTemplates();
            Assert.That(templates, Is.EquivalentTo(expected));
        }
    }
}
