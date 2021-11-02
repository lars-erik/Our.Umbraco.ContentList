using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public class PagedUrlBaseBuilder
    {
        private readonly Uri uri;
        private readonly Dictionary<string, IEnumerable<string>> queryString;

        public PagedUrlBaseBuilder(Uri uri, Dictionary<string, IEnumerable<string>> queryString)
        {
            this.uri = uri;
            this.queryString = queryString;
        }

        public string Build(ContentListModel model)
        {
            var hash = model.Configuration.CreateHash();
            var otherParams = String.Join("&", queryString.Keys.Where(x => x != hash).Select(x => x + "=" + queryString[x]));

            var newQuery = otherParams.Length > 0 ? "?" + otherParams + "&" : "?";

            var urlBuilder = new UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath, newQuery + hash + "=");

            var baseUrl = urlBuilder.ToString();

            return baseUrl;
        }
    }
}
