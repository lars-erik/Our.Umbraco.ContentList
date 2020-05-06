using System.Web;

namespace Our.Umbraco.ContentList
{
    public interface IPublicationMetadata
    {
        string Author
        {
            get;
        }
        System.DateTime Date
        {
            get;
        }

    }
}
