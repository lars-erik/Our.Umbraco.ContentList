using System;
using System.Collections.Specialized;
using Moq;
using NUnit.Framework;
using Our.Umbraco.ContentList.Install;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Tests.TestHelpers;
using Umbraco.UnitTesting.Adapter;
using Umbraco.Web;
using Umbraco.Web.Composing;
using Umbraco.Web.Routing;

//using Umbraco.Web;
//using Umbraco.Web.Routing;

namespace Our.Umbraco.ContentList.Tests.Web
{
    public abstract class BaseControllerTest //: BaseWebTest
    {
        protected UmbracoSupport UmbracoSupport;

        [SetUp]
        public void SetUp()
        {
            UmbracoSupport = new UmbracoSupport();
            UmbracoSupport.SetupUmbraco(composition =>
            {
                composition.WithListableDataSources();
            });
        }

        [TearDown]
        public void TearDown()
        {
            UmbracoSupport.DisposeUmbraco();
        }

        public UmbracoContext SetupPublishedRequest<T>(Mock<T> publishedContentMock, string url = "/") // UmbracoContext
            where T : class, IPublishedContent
        {
            UmbracoSupport.GetUmbracoContext(url);
            var request = new FakePublishedRequest(Mock.Of<IPublishedRouter>(), Current.UmbracoContext, new Uri("http://localhost" + url))
            {
                PublishedContent = publishedContentMock.Object
            };
            Current.UmbracoContext.PublishedRequest = request;
            return Current.UmbracoContext;
            //var umbracoSettingsMock = new Mock<IUmbracoSettingsSection>();
            //var routingSectionMock = new Mock<IWebRoutingSection>();
            //routingSectionMock.Setup(s => s.UrlProviderMode).Returns(UrlProviderMode.Auto.ToString());
            //umbracoSettingsMock.Setup(s => s.WebRouting).Returns(routingSectionMock.Object);
            //var publishedContentRequest = new PublishedContentRequest(new Uri("http://localhost" + url),
            //    GetRoutingContext(url, -1, null, false, umbracoSettingsMock.Object), routingSectionMock.Object,
            //    s => new string[0]);
            //GetRoutingContext(url, -1, null, true, umbracoSettingsMock.Object);
            //UmbracoContext.Current.PublishedContentRequest = publishedContentRequest;
            //publishedContentRequest.PublishedContent = publishedContentMock.Object;

            //return UmbracoContext.Current;

            //throw new NotImplementedException("Obsolete altogether");
        }

        protected static Mock<IPublishedContent> MockContent(int id)
        {
            var mock = new Mock<IPublishedContent>();
            mock.Setup(c => c.Id).Returns(id);
            return mock;
        }

        protected static Mock<T> MockContent<T>(int id, int? level = null)
            where T : class, IPublishedContent
        {
            var mock = new Mock<T>();
            mock.Setup(c => c.Id).Returns(id);
            if (level != null)
                mock.Setup(c => c.Level).Returns(level.Value);
            return mock;
        }
    }
}
