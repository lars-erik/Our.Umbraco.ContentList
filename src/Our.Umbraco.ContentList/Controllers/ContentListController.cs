using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Newtonsoft.Json;
using Our.Umbraco.ContentList.Models;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Attributes;

namespace Our.Umbraco.ContentList.Controllers
{
    public class ContentListController : Controller
    {
        private readonly IPublishedSnapshotAccessor snapshotAccessor;
        private readonly IRazorViewEngine viewEngine;
        private readonly IUmbracoContextAccessor umbracoContextAccessor;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ContentListQueryHandler contentListQueryHandler;
        private readonly IServiceProvider provider;

        public ContentListController(
            IServiceProvider provider, 
            IPublishedSnapshotAccessor snapshotAccessor, 
            IRazorViewEngine viewEngine, 
            IUmbracoContextAccessor umbracoContextAccessor, 
            IHttpContextAccessor httpContextAccessor,
            ContentListQueryHandler contentListQueryHandler
        )
        {
            this.snapshotAccessor = snapshotAccessor;
            this.viewEngine = viewEngine;
            this.umbracoContextAccessor = umbracoContextAccessor;
            this.httpContextAccessor = httpContextAccessor;
            this.contentListQueryHandler = contentListQueryHandler;
            this.provider = provider;
        }

        [HttpGet]
        public ActionResult HelloWorld(string input)
        {
            return View("Echo");
        }

        [HttpGet]
        public ActionResult GetEcho(string input)
        {
            return Ok(input);
        }

        [HttpPost]
        public ActionResult PostEcho(string input)
        {
            return Ok(input);
        }

        [HttpPost]
        [LowerCaseInputJson]
        public ActionResult Preview(
            [ModelBinder(typeof(LowerCaseJsonBinder))]
            PreviewContentListConfiguration configuration
        )
        {
            try
            {
                snapshotAccessor.TryGetPublishedSnapshot(out var snapshot);
                var contextContent = snapshot.Content.GetById(configuration.ContentId);
                var viewResult = ViewComponent("ContentList", new { configuration, contextContent });
                return viewResult;
            }
            catch (Exception ex)
            {
                try
                {
                    Response.StatusCode = 500;
                    var result = Content(JsonConvert.SerializeObject(ex, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
                    return result;
                }
                catch (Exception innerEx)
                {
                    throw new AggregateException(
                        innerEx,
                        ex
                    );
                }
            }
        }

        public ActionResult Count(PreviewContentListConfiguration configuration)
        {
            snapshotAccessor.TryGetPublishedSnapshot(out var snapshot);
            var contextContent = snapshot.Content.GetById(configuration.ContentId);
            var query = contentListQueryHandler.CreateQuery(configuration, contextContent);
            var datasource = contentListQueryHandler.CreateDataSource(configuration);
            var total = datasource.Count(query, configuration.Skip);
            return new ObjectResult(total);
        }

    }
}
