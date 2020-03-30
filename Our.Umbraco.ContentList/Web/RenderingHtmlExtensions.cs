using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Our.Umbraco.ContentList.Web
{
    public static class RenderingHtmlExtensions
    {
        public static IHtmlString ContentListItem(this HtmlHelper helper, IListableContent contentListItem)
        {
            var contentListModel = helper.ViewContext.ViewData.Model as ContentListModel;
            if (contentListModel == null)
                throw new Exception("the ContentListItem extension can only be used from a view that has ContentListModel as Model");

            var result = FindItemView(helper, contentListModel, contentListItem);

            if (result.View != null)
                return RenderView(helper, result, contentListItem);

            throw new Exception("Couldn't find list item view for " + contentListItem.Name + " in template " + contentListModel.Configuration.View);
        }

        private static ViewEngineResult FindItemView(HtmlHelper helper, ContentListModel contentListModel, IListableContent contentListItem)
        {
            var paths = new Func<string, string, string>[]
            {
                (t, a) => "~/Views/Partials/ContentList/" + t + "/" + a + ".cshtml",
                (t, a) => "~/Views/Partials/ContentList/" + t + "/ListItem.cshtml"
            };

            var templateName = contentListModel.Configuration.View;
            var docTypeAlias = contentListItem.DocumentTypeAlias;
            ViewEngineResult result = new ViewEngineResult(new string[0]);

            for (var i = 0; i < paths.Length && result.View == null; i++)
                result = ViewEngines.Engines.FindPartialView(helper.ViewContext, paths[i](templateName, docTypeAlias));

            return result;
        }

        private static IHtmlString RenderView(HtmlHelper helper, ViewEngineResult result, IListableContent contentListItem)
        {
            using (var writer = new StringWriter())
            {
                var ctx = new ViewContext(helper.ViewContext, result.View, new ViewDataDictionary {Model = contentListItem}, helper.ViewContext.TempData, writer);
                result.View.Render(ctx, writer);
                return MvcHtmlString.Create(writer.ToString());
            }
        }
    }
}
