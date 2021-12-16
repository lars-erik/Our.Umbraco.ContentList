using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Our.Umbraco.ContentList.Web.Models.ContentModels
{
    public partial class ContentPage : IListableContent
    {
        public string ListHeading => this.Value<string>("pageTitle", fallback:Fallback.ToLanguage);
        public IPublishedContent ListImage => null;
        public string ListImageUrl => null;
        public IHtmlString ListSummary => new HtmlString(SeoMetaDescription);
        public string ReadMoreText => "Read more";
        public string ContentTypeName => ContentType.Alias;
        public DateTime SortDate => CreateDate;
    }
}