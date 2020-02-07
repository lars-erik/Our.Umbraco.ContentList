using System;
using System.Web;
using Our.Umbraco.ContentList;
using Umbraco.Core.Models.PublishedContent;

namespace Umbraco.Web.PublishedContentModels
{

    public partial class TextPage : IListableContent
    {
        public IHtmlString ListHeading => new HtmlString(this.Name);
        public string ListImageUrl => string.Empty;
        public IHtmlString ListSummary => new HtmlString("her finner du et summary");
        public IHtmlString ReadMoreText => new HtmlString("les mer");
    }
}