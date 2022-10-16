using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Our.Umbraco.ContentList.Models;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.Controllers;

public class ContentListViewComponent : ViewComponent
{
    private readonly IRazorViewEngine viewEngine;
    private readonly ContentListQueryHandler contentListQueryHandler;
    private string path;

    public ContentListViewComponent(
        IRazorViewEngine viewEngine,
        ContentListQueryHandler contentListQueryHandler,
        IHostingEnvironment env
    )
    {
        this.viewEngine = viewEngine;
        this.contentListQueryHandler = contentListQueryHandler;
        path = env.MapPathContentRoot("~/Views/Partials/ContentList");
    }

    public async Task<IViewComponentResult> InvokeAsync(ContentListConfiguration configuration, IPublishedContent contextContent)
    {
        var model = await Execute(configuration, contextContent);

        var viewName = FindView(configuration);
        var result = View(viewName, model);
        return await Task.FromResult(result);
    }

    private async Task<ContentListModel> Execute(ContentListConfiguration configuration, IPublishedContent contextContent)
    {
        var dataSource = contentListQueryHandler.CreateDataSource(configuration);

        var query = contentListQueryHandler.CreateQuery(configuration, contextContent);
        var total = await dataSource.Count(query, configuration.Skip);
        var queryPaging = contentListQueryHandler.CreateQueryPaging(configuration, total);

        var data = await dataSource.Query(query, queryPaging);

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

        foundView = viewEngine.GetView(null, path, false);

        if (!foundView.Success)
        {
            var rootDir = new DirectoryInfo(path);
            if (name == "Sample" || !rootDir.Exists)
            { 
                path = "~/App_Plugins/Our.Umbraco.ContentList/Views/ListViews/Sample.cshtml";
                foundView = viewEngine.GetView(null, path, false);
            }
        }

        if (!foundView.Success)
        {
            throw new Exception("No content list view called " + configuration.View + " found");
        }

        return foundView.ViewName;
    }

    public static ContentListPaging CreatePagingModel(
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