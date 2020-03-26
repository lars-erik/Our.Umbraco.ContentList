using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        public ViewResult List(ContentListQuery query)
        {
            var contextContent = UmbracoContext.PublishedRequest.PublishedContent;
            query.Page = FindPageParameter(query);
            return List(query, contextContent);
        }

        public ActionResult Preview(PreviewContentListQuery query)
        {
            try
            {
                var contextContent = Umbraco.Content(query.ContentId);
                return List(query, contextContent);
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

        public ActionResult Count(PreviewContentListQuery query)
        {
            var contextContent = Umbraco.Content(query.ContentId);
            var datasource = CreateDataSource(query, contextContent);
            var total = datasource.Count(query.Skip);
            return Json(total);
        }

        private ViewResult List(ContentListQuery query, IPublishedContent contextContent)
        {
            var datasource = CreateDataSource(query, contextContent);
            var total = datasource.Count(query.Skip);
            var paging = CreatePagingModel(query, total);
            var data = datasource.Query(new PagingParameter(paging.From-1, paging.PageSize, query.Skip));

            var model = new ContentListModel
            {
                Items = data,
                Query = query,
                ColumnStyling = new ContentListColumnStyling(query.Columns),
                Paging = paging,
                Hash = query.CreateHash()
            };

            var viewName = FindView(query);
            return View(viewName, model);
        }

        private IView FindView(ContentListQuery query)
        {
            var name = query.View;
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
                throw new Exception("No content list view called " + query.View + " found");
            }

            return foundView.View;
        }

        private ContentListPaging CreatePagingModel(ContentListQuery query, long total)
        {
            if (!query.ShowPaging) { 
                return new ContentListPaging
                {
                    From = 1,
                    Page = 1,
                    PageSize = query.PageSize,
                    To = query.PageSize,
                    Total = total
                };
            }

            var maxPage = total/query.PageSize;
            var zerobasePage = Math.Min(Math.Max(query.Page - 1, 0), maxPage);
            
            var pagingSkip = zerobasePage*query.PageSize;

            return new ContentListPaging
            {
                From = pagingSkip + 1,
                To = Math.Min(pagingSkip + query.PageSize, total),
                Page = zerobasePage + 1,
                PageSize = query.PageSize,
                Total = total,
                ShowPaging = query.ShowPaging
            };
        }

        private IListableDataSource CreateDataSource(ContentListQuery query, IPublishedContent contextContent)
        {
            var queryParameters = ContentListToQueryParameters(query, contextContent);
            return datasourceFactory.Create(query.DataSource.Type, queryParameters);
        }

        private static QueryParameters ContentListToQueryParameters(ContentListQuery query, IPublishedContent contextContent)
        {
            return new QueryParameters(contextContent, query.DataSource.Parameters);
        }

        private int FindPageParameter(ContentListQuery query)
        {
            int page;
            var hash = query.CreateHash();
            if (!Int32.TryParse(Request.QueryString[hash], out page))
                page = 1;
            return page;
        }
    }
}
