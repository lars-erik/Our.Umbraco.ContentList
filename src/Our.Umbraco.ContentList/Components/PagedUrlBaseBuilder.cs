using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.ContentList.Models;

namespace Our.Umbraco.ContentList.Components
{
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