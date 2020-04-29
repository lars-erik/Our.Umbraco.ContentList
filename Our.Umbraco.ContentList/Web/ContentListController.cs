using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.DataSources.Listables;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.ContentList.Web
{
    [PluginController("OurContentList")]
    public class ContentListController : SurfaceController
    {
        private readonly ListableDataSourceFactory datasourceFactory;

        #region constructors

        public ContentListController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            ILogger logger,
            IProfilingLogger profilingLogger,
            UmbracoHelper umbracoHelper
        ) : this(new ListableDataSourceFactory(), umbracoContextAccessor, databaseFactory, services, appCaches, logger, profilingLogger, umbracoHelper)
        {
        }

        public ContentListController(
            ListableDataSourceFactory datasourceFactory,
            IUmbracoContextAccessor umbracoContextAccessor, 
            IUmbracoDatabaseFactory databaseFactory, 
            ServiceContext services, 
            AppCaches appCaches, 
            ILogger logger, 
            IProfilingLogger profilingLogger, 
            UmbracoHelper umbracoHelper
        ) : base(umbracoContextAccessor, databaseFactory, services, appCaches, logger, profilingLogger, umbracoHelper)
        { 
            this.datasourceFactory = datasourceFactory;
        }

        #endregion

        public ViewResult List(ContentListConfiguration configuration)
        {
            var contextContent = UmbracoContext.PublishedRequest.PublishedContent;
            return List(configuration, contextContent);
        }

        public ActionResult Preview(PreviewContentListConfiguration configuration)
        {
            try
            {
                var contextContent = Umbraco.Content(configuration.ContentId);
                return List(configuration, contextContent);
            }
            catch (Exception ex)
            {
                try
                {
                    Response.StatusCode = 500;
                    var result = Content(JsonConvert.SerializeObject(ex, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore}), "application/json");
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
            var contextContent = Umbraco.Content(configuration.ContentId);
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
            var maxPage = total / configuration.PageSize;
            var zerobasePage = Math.Min(Math.Max(currentPage - 1, 0), maxPage);
            var querySkip = zerobasePage * configuration.PageSize;
            var pagingParameter = new QueryPaging(querySkip, configuration.PageSize, configuration.Skip);
            return pagingParameter;
        }

        private IView FindView(ContentListConfiguration configuration)
        {
            var name = configuration.View;
            ViewEngineResult foundView = new ViewEngineResult(new string[0]);

            if (foundView.View == null)
            {
                var customPath = "~/Views/Partials/ContentList/" + name + "/List.cshtml";
                foundView = ViewEngineCollection.FindPartialView(ControllerContext, customPath);
            }

            if (foundView.View == null)
            {
                // TODO: No test validates this
                var pluginPath = "~/App_Plugins/Our.Umbraco.ContentList/Views/ContentList/ListViews/" + name + ".cshtml";
                foundView = ViewEngineCollection.FindPartialView(ControllerContext, pluginPath);
            }

            if (foundView.View == null)
            {
                throw new Exception("No content list view called " + configuration.View + " found");
            }

            return foundView.View;
        }

        private ContentListPaging CreatePagingModel(
            QueryPaging queryPaging,
            ContentListConfiguration configuration, 
            long total)
        {
            if (!configuration.ShowPaging) { 
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
            return datasourceFactory.Create(configuration.DataSource.Type);
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
            if (!Int32.TryParse(Request.QueryString[hash], out page))
                page = 1;
            return page;
        }
    }
}
