using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Core.PropertyEditors;

namespace Our.Umbraco.ContentList.Composition
{
    public class ContentListManifestFilter : IManifestFilter
    {
        private readonly IDataValueEditorFactory dataValueEditorFactory;

        public ContentListManifestFilter(IDataValueEditorFactory dataValueEditorFactory)
        {
            this.dataValueEditorFactory = dataValueEditorFactory;
        }

        public void Filter(List<PackageManifest> manifests)
        {
            manifests.Add(new PackageManifest
            {
                PackageName = "Content List",
                Stylesheets = new[]
                {
                    "/App_Plugins/Our.Umbraco.ContentList/css/contentlist.css"
                },
                Scripts = new[]
                {
                    "/App_Plugins/Our.Umbraco.ContentList/common/datasource.service.js",
                    "/App_Plugins/Our.Umbraco.ContentList/common/query.properties.js",
                    "/App_Plugins/Our.Umbraco.ContentList/common/state.js",
                    "/App_Plugins/Our.Umbraco.ContentList/common/template.service.js",
                    "/App_Plugins/Our.Umbraco.ContentList/grideditors/contentlist/contentlist.js",
                    "/App_Plugins/Our.Umbraco.ContentList/propertyeditors/columns/columns.controller.js",
                    "/App_Plugins/Our.Umbraco.ContentList/propertyeditors/datasource/blge.datasource.controller.js",
                    "/App_Plugins/Our.Umbraco.ContentList/propertyeditors/datasource/grid.datasource.controller.js",
                    "/App_Plugins/Our.Umbraco.ContentList/propertyeditors/theme/blge.theme.controller.js",
                    "/App_Plugins/Our.Umbraco.ContentList/propertyeditors/theme/grid.theme.controller.js",
                    "/App_Plugins/Our.Umbraco.ContentList/propertyeditors/query/contentlist.query.controller.js"
                },
                GridEditors = new[]
                {
                    new GridEditor
                    {
                        Alias = "content.list",
                        Name = "Content List",
                        View = "/App_Plugins/Our.Umbraco.ContentList/grideditors/contentlist/contentlist.html",
                        Render = "/App_Plugins/Our.Umbraco.ContentList/Views/ContentList.cshtml",
                        Icon = "icon-list",
                        Config = new Dictionary<string, object>
                        {
                            { "settings", new Dictionary<string, object>() }
                        }
                    }
                }
            });
        }
    }
}
