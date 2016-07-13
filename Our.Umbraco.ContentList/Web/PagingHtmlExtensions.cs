using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Web;

namespace Our.Umbraco.ContentList.Web
{
    public static class PagingHtmlExtensions
    {
        public static IHtmlString ContentListPager(
            this HtmlHelper<ContentListModel> helper,
            string pagerClasses = "",
            string itemClasses = "",
            string anchorClasses = "",
            string pagerElement = "div",
            string itemElement = ""
            )
        {
            return ContentListPager(helper, helper.ViewData.Model, pagerClasses, itemClasses, anchorClasses, pagerElement, itemElement);
        }

        public static IHtmlString ContentListPager(
            this HtmlHelper helper, 
            ContentListModel model, 
            string pagerClasses = "", 
            string itemClasses = "", 
            string anchorClasses = "", 
            string pagerElement = "div", 
            string itemElement = ""
        )
        {
            var paging = model.Paging;

            if (DontShow(paging))
                return new MvcHtmlString("");

            var url = GetUrl(helper);

            var builder = CreateOutput(paging.Page, paging.Pages, url, pagerClasses, itemClasses, anchorClasses, pagerElement, itemElement);

            return new MvcHtmlString(builder.ToString());
        }

        private static bool DontShow(ContentListPaging paging)
        {
            return !paging.ShowPaging || paging.Total <= paging.PageSize;
        }

        private static string GetUrl(HtmlHelper helper)
        {
            if (helper.ViewContext == null || 
                helper.ViewContext.RequestContext == null ||
                helper.ViewContext.RequestContext.HttpContext == null ||
                helper.ViewContext.RequestContext.HttpContext.Request == null ||
                helper.ViewContext.RequestContext.HttpContext.Request.Url == null
                )
                return "";
            return helper.ViewContext.RequestContext.HttpContext.Request.Url.GetComponents(UriComponents.AbsoluteUri ^ UriComponents.Query, UriFormat.UriEscaped);
        }

        private static StringBuilder CreateOutput(int currentPage, int pages, string url, string pagerClasses = "", string itemClasses = "", string anchorClasses = "", string pagerElement = "div", string itemElement = "")
        {
            var builder = new StringBuilder(8192);
            var anchorClassAttribute = anchorClasses.IsNullOrWhiteSpace() ? "" : " class=\"" + anchorClasses + "\"";

            builder.AppendFormat("<{0} class=\"pagination {1}\">", pagerElement, pagerClasses);

            for (var i = 0; i < pages; i++)
            {
                var isCurrent = i + 1 == currentPage;

                if (!itemElement.IsNullOrWhiteSpace())
                    builder.AppendFormat("<{0} class=\"{1}\">", itemElement, itemClasses + (isCurrent ? " active" : ""));

                if (!isCurrent && UmbracoContext.Current.IsFrontEndUmbracoRequest)
                    builder.AppendFormat(
                        "<a{2} href=\"{0}?p={1}\">{1}</a>",
                        url,
                        i + 1,
                        anchorClassAttribute
                        );
                else
                    builder.AppendFormat("<span>{0}</span>", i + 1);

                if (!itemElement.IsNullOrWhiteSpace())
                    builder.AppendFormat("</{0}>", itemElement);
            }

            builder.AppendFormat("</{0}>", pagerElement);
            return builder;
        }
    }
}
