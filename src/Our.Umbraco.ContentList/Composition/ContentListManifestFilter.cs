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
                    "/App_Plugins/Our.Umbraco.ContentList/our.umbraco.contentlist.umd.js"
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
