using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Website.Controllers;

namespace Our.Umbraco.ContentList.Controllers
{
    [PluginController("OurContentList")]
    [IgnoreAntiforgeryToken]
    public class ContentListController : SurfaceController
    {
        private readonly IPublishedSnapshotAccessor snapshotAccessor;
        private readonly IRazorViewEngine viewEngine;
        private readonly IServiceProvider provider;

        public ContentListController(IServiceProvider provider, IPublishedSnapshotAccessor snapshotAccessor, IRazorViewEngine viewEngine, IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            this.snapshotAccessor = snapshotAccessor;
            this.viewEngine = viewEngine;
            this.provider = provider;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        [HttpGet]
        public ActionResult GetEcho(string input)
        {
            return Ok(input);
        }

        [HttpPost]
        public ActionResult PostEcho([FromBody]string input)
        {
            return Ok(input);
        }

        [HttpPost]
        public ActionResult Preview(PreviewContentListConfiguration configuration)
        {
            try
            {
                snapshotAccessor.TryGetPublishedSnapshot(out var snapshot);
                var contextContent = snapshot.Content.GetById(configuration.ContentId);
                return List(configuration, contextContent);
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

            return new OkObjectResult(new object());
        }

        public ActionResult Count(PreviewContentListConfiguration configuration)
        {
            snapshotAccessor.TryGetPublishedSnapshot(out var snapshot);
            var contextContent = snapshot.Content.GetById(configuration.ContentId);
            var query = CreateQuery(configuration, contextContent);
            var datasource = CreateDataSource(configuration);
            var total = datasource.Count(query, configuration.Skip);
            return Json(total);
        }

        private ViewResult List(ContentListConfiguration configuration, IPublishedContent contextContent)
        {
            var datasource = CreateDataSource(configuration);

            var query = CreateQuery(configuration, contextContent);
            var total = datasource.Count(query, configuration.Skip);
            var queryPaging = CreateQueryPaging(configuration, total);

            var data = datasource.Query(query, queryPaging);

            var model = new ContentListModel
            {
                Items = data,
                Query = query,
                Configuration = configuration,
                ColumnStyling = new ContentListColumnStyling(configuration.Columns),
                Paging = CreatePagingModel(queryPaging, configuration, total),
                Hash = configuration.CreateHash()
            };

            var viewName = FindView(configuration);
            return View(viewName, model);
        }

        private QueryPaging CreateQueryPaging(ContentListConfiguration configuration, long total)
        {
            var currentPage = FindPageParameter(configuration);
            var maxPage = total / Math.Max(configuration.PageSize, 1);
            var zerobasePage = Math.Min(Math.Max(currentPage - 1, 0), maxPage);
            var querySkip = zerobasePage * configuration.PageSize;
            var pagingParameter = new QueryPaging(querySkip, configuration.PageSize, configuration.Skip);
            return pagingParameter;
        }

        private string FindView(ContentListConfiguration configuration)
        {
            var name = configuration.View;
            ViewEngineResult foundView = null;
            string path;

            path = "~/Views/Partials/ContentList/" + name + "/List.cshtml";

            foundView = viewEngine.FindView(ControllerContext, path, false);

            if (foundView == null)
            {
                path = null;
            }

            if (path == null)
            {
                // TODO: No test validates this
                path = "~/App_Plugins/Our.Umbraco.ContentList/Views/ContentList/ListViews/" + name + ".cshtml";
                foundView = viewEngine.FindView(ControllerContext, path, false);
            }

            if (foundView == null)
            {
                throw new Exception("No content list view called " + configuration.View + " found");
            }

            return path;
        }

        private ContentListPaging CreatePagingModel(
            QueryPaging queryPaging,
            ContentListConfiguration configuration,
            long total)
        {
            if (!configuration.ShowPaging)
            {
                return new ContentListPaging
                {
                    From = 1,
                    Page = 1,
                    PageSize = configuration.PageSize,
                    To = configuration.PageSize,
                    Total = total
                };
            }

            return new ContentListPaging
            {
                From = queryPaging.Skip + 1,
                To = Math.Min(queryPaging.Skip + configuration.PageSize, total),
                Page = (queryPaging.Skip / configuration.PageSize) + 1,
                PageSize = configuration.PageSize,
                Total = total,
                ShowPaging = configuration.ShowPaging
            };
        }

        private IListableDataSource CreateDataSource(ContentListConfiguration configuration)
        {
            var typeName = configuration.DataSource.Type;
            var type = Type.GetType(typeName);
            var dataSource = (IListableDataSource)provider.GetService(type);
            return dataSource;
        }

        private ContentListQuery CreateQuery(ContentListConfiguration configuration, IPublishedContent contextContent)
        {
            return new ContentListQuery(contextContent, configuration.DataSource.Parameters)
            {
                HttpContext = HttpContext
            };
        }

        private int FindPageParameter(ContentListConfiguration configuration)
        {
            int page;
            var hash = configuration.CreateHash();
            if (!Int32.TryParse(Request.Query[hash], out page))
                page = 1;
            return page;
        }
    }
}
