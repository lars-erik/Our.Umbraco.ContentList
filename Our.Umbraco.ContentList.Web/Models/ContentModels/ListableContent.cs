using System.Web;

namespace Our.Umbraco.ContentList.Web.Models.ContentModels
{
    public partial interface IListableContent : Our.Umbraco.ContentList.IListableContent
    {

    }

    public partial class ListableContent
    {
        IHtmlString ContentList.IListableContent.ListHeading => new HtmlString(ListHeading);
        public string ListImageUrl => ListImage?.Url;
        public IHtmlString ReadMoreText => new HtmlString(ReadMoreLinkText);
        public string DocumentTypeAlias => ContentType.Alias;
    }
}