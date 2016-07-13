using Umbraco.Core.Models;
using Umbraco.Web;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Our.Umbraco.ContentList.Web
{
    public static class ListableContentExtensions
    {
        public static bool IsVisible(this IListableContent content)
        {
            var asPublishedContent = content as IPublishedContent;
            if (asPublishedContent != null)
            {
                return asPublishedContent.IsVisible();
            }
            var asListableContentVisibility = content as IListableContentVisibility;
            if (asListableContentVisibility != null)
            {
                return asListableContentVisibility.IsVisible();
            }
            return true;
        }
    }
}
