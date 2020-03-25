using System.Web;

namespace Our.Umbraco.ContentList.Web.Models.ContentModels
{
    public partial class Page
    {
        IHtmlString ContentList.IListableContent.ListHeading => new HtmlString(ListHeading);
        public string ListImageUrl => ListImage?.Url;
        public IHtmlString ReadMoreText => new HtmlString(ReadMoreLinkText);
        public string DocumentTypeAlias => ContentType.Alias;
    }
}