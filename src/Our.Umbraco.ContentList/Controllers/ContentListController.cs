using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Web;

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

            return new OkObjectResult(new object());
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

    public class ContentListQueryHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IServiceProvider provider;

        public ContentListQueryHandler(IHttpContextAccessor httpContextAccessor, IServiceProvider provider)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.provider = provider;
        }

        public QueryPaging CreateQueryPaging(ContentListConfiguration configuration, long total)
        {
            var currentPage = FindPageParameter(configuration);
            var maxPage = total / Math.Max(configuration.PageSize, 1);
            var zerobasePage = Math.Min(Math.Max(currentPage - 1, 0), maxPage);
            var querySkip = zerobasePage * configuration.PageSize;
            var pagingParameter = new QueryPaging(querySkip, configuration.PageSize, configuration.Skip);
            return pagingParameter;
        }

        public IListableDataSource CreateDataSource(ContentListConfiguration configuration)
        {
            var typeName = configuration.DataSource.Type;
            var type = Type.GetType(typeName);
            var dataSource = (IListableDataSource)provider.GetService(type);
            return dataSource;
        }

        public ContentListQuery CreateQuery(ContentListConfiguration configuration, IPublishedContent contextContent)
        {
            return new ContentListQuery(contextContent, configuration.DataSource.Parameters)
            {
                HttpContext = httpContextAccessor.HttpContext
            };
        }

        private int FindPageParameter(ContentListConfiguration configuration)
        {
            int page;
            var hash = configuration.CreateHash();
            var request = httpContextAccessor.HttpContext?.Request;
            if (!Int32.TryParse(request?.Query?[hash], out page))
                page = 1;
            return page;
        }
    }

    public class ContentListViewComponent : ViewComponent
    {
        private readonly IRazorViewEngine viewEngine;
        private readonly ContentListQueryHandler contentListQueryHandler;

        public ContentListViewComponent(
            IRazorViewEngine viewEngine,
            ContentListQueryHandler contentListQueryHandler
        )
        {
            this.viewEngine = viewEngine;
            this.contentListQueryHandler = contentListQueryHandler;
        }

        public async Task<IViewComponentResult> InvokeAsync(ContentListConfiguration configuration, IPublishedContent contextContent)
        {
            var model = Execute(configuration, contextContent);

            var viewName = FindView(configuration);
            var result = View(viewName, model);
            return await Task.FromResult(result);
        }

        private ContentListModel Execute(ContentListConfiguration configuration, IPublishedContent contextContent)
        {
            var datasource = contentListQueryHandler.CreateDataSource(configuration);

            var query = contentListQueryHandler.CreateQuery(configuration, contextContent);
            var total = datasource.Count(query, configuration.Skip);
            var queryPaging = contentListQueryHandler.CreateQueryPaging(configuration, total);

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
            return model;
        }

        private string FindView(ContentListConfiguration configuration)
        {
            var name = configuration.View;
            ViewEngineResult foundView = null;
            string path;

            path = "~/Views/Partials/ContentList/" + name + "/List.cshtml";

            foundView = viewEngine.FindView(ViewContext, path, false);

            if (foundView == null)
            {
                path = null;
            }

            if (path == null)
            {
                // TODO: No test validates this
                path = "~/App_Plugins/Our.Umbraco.ContentList/Views/ContentList/ListViews/" + name + ".cshtml";
                foundView = viewEngine.FindView(ViewContext, path, false);
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
    }

    public class LowerCaseInputJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            if (request.ContentType == "application/json")
            {
                request.EnableBuffering();
            }
        }
    }


    public class LowerCaseJsonBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var paramName = bindingContext.ModelName;
            var providedValue = bindingContext.ValueProvider.GetValue(paramName);
            if (providedValue.FirstValue != null &&
                providedValue.FirstValue.GetType() == bindingContext.ModelType)
                return;

            var request = bindingContext.HttpContext.Request;

            var stream = request.Body;
            var readStream = new StreamReader(stream, Encoding.UTF8);
            var json = await readStream.ReadToEndAsync();
            bindingContext.Result = ModelBindingResult.Success(JsonConvert.DeserializeObject(json, bindingContext.ModelType));
        }
    }

}
