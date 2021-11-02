using System;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Our.Umbraco.ContentList.Models;

namespace Our.Umbraco.ContentList.Components
{
    [ViewComponent(Name="ContentListPager")]
    public class PagerComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ContentListModel model)
        {
            var request = ViewContext.HttpContext.Request;
            var uri = new Uri(request.GetEncodedUrl());
            var queryString = request.Query.ToDictionary(x => x.Key, x => x.Value.AsEnumerable());

            var builder = new PagedUrlBaseBuilder(uri, queryString);

            ViewData.Add("baseUrl", builder.Build(model));
            
            return View("Pager", model.Paging);
        }
    }
}
