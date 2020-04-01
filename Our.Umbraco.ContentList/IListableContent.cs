using System;
using System.Web;

namespace Our.Umbraco.ContentList
{
    public interface IListableContent
    {
        string ListHeading
        {
            get;
        }
        string ListImageUrl
        {
            get;
        }
        IHtmlString ListSummary
        {
            get;
        }

        string ReadMoreText { get; }

        // ....

        string ContentTypeName { get; }

        string Url { get; }

        int SortOrder { get; }

        DateTime SortDate { get; }
    }

    public interface IListableContentVisibility
    {
        bool IsVisible();
    }
}
