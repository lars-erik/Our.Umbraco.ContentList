using System;
using Microsoft.AspNetCore.Http;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.Controllers;

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