using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.DataSources.Listables;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.ContentList.Web
{
    [PluginController("OurContentList")]
    public class ContentListController : SurfaceController
    {
        private readonly ListableDataSourceFactory datasourceFactory;

        #region constructors

        public ContentListController()
            : this(new ListableDataSourceFactory())
        {
        }

        public ContentListController(ListableDataSourceFactory datasourceFactory)
        { 
            this.datasourceFactory = datasourceFactory;
        }

        #endregion

        public ViewResult List(ContentListParameters query)
        {
            throw new NotImplementedException("Obsolete methods");

            // TODO: Individuell identifisering vha. string hashcode av query
            //var contextContent = UmbracoContext.PublishedContentRequest.PublishedContent;
            //query.Page = FindPageParameter();
            //return List(query, contextContent);
        }

        public ViewResult Preview(PreviewContentListParameters query)
        {
            throw new NotImplementedException("Obsolete methods");

            //var contextContent = Umbraco.TypedContent(query.ContentId);
            //return List(query, contextContent);
        }

        public ActionResult Count(PreviewContentListParameters query)
        {
            throw new NotImplementedException("Obsolete methods");

            //var contextContent = Umbraco.TypedContent(query.ContentId);
            //var datasource = CreateDataSource(query, contextContent);
            //var total = datasource.Count(query.Skip);
            //return Json(total);
        }

        private ViewResult List(ContentListParameters query, IPublishedContent contextContent)
        {
            var datasource = CreateDataSource(query, contextContent);
            var total = datasource.Count(query.Skip);
            var paging = CreatePagingModel(query, total);
            var data = datasource.Query(new PagingParameter(paging.From-1, paging.PageSize, query.Skip));

            var model = new ContentListModel
            {
                Items = data,
                Parameters = query,
                ColumnStyling = new ContentListColumnStyling(query.Columns),
                Paging = paging
            };

            var viewName = FindView(query);
            return View(viewName, model);
        }

        private IView FindView(ContentListParameters query)
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

            //return "ListViews/" + query.View;
            return foundView.View;
        }

        private ContentListPaging CreatePagingModel(ContentListParameters query, long total)
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

        private IListableDataSource CreateDataSource(ContentListParameters query, IPublishedContent contextContent)
        {
            var queryParameters = ContentListToQueryParameters(query, contextContent);
            return datasourceFactory.Create(query.DataSource, queryParameters);
        }

        private static QueryParameters ContentListToQueryParameters(ContentListParameters query, IPublishedContent contextContent)
        {
            return new QueryParameters(contextContent, query.DataSourceParameters);
        }

        private int FindPageParameter()
        {
            int page;
            if (!Int32.TryParse(Request.QueryString["p"], out page))
                page = 1;
            return page;
        }
    }
}
