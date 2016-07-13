using System.Web;

namespace Our.Umbraco.ContentList
{
    public interface IPublicationMetadata
    {
        IHtmlString Author
        {
            get;
        }
        System.DateTime Date
        {
            get;
        }

    }
}
