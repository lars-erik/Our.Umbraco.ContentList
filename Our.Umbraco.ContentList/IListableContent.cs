using System;
using System.Web;

namespace Our.Umbraco.ContentList
{
    public interface IListableContent
    {
        IHtmlString ListHeading
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

        IHtmlString ReadMoreText { get; }

        // ....

        string DocumentTypeAlias { get; }

        string Url { get; }

        string Name { get; }

        int SortOrder { get; }

        DateTime CreateDate { get; }

        string CreatorName { get; }
    }

    public interface IListableContentVisibility
    {
        bool IsVisible();
    }
}
