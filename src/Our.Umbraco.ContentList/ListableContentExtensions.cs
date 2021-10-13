using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList
{
    public static class ListableContentExtensions
    {
        public static bool IsVisible(this IListableContent content)
        {
            if (content is IPublishedContent asPublishedContent)
            {
                return asPublishedContent.IsVisible();
            }

            // This is for extension with non-Umbraco content, so we want it here.
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (content is IListableContentVisibility asListableContentVisibility)
            {
                return asListableContentVisibility.IsVisible();
            }

            return true;
        }
    }
}