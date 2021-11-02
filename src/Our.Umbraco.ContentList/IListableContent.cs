using System;
using Microsoft.AspNetCore.Html;
using Umbraco.Cms.Core.Strings;

namespace Our.Umbraco.ContentList
{
    public interface IListableContent
    {
        public string ContentTypeName { get; }
        public string ListHeading { get; }
        public IHtmlEncodedString ListSummary { get; }
        public string ListImageUrl { get; }
        public string ReadMoreText { get; }

        public string Url { get; }
        int SortOrder { get; }
        DateTime SortDate { get; }
    }
}
