using Umbraco.Cms.Core.PropertyEditors;

namespace Our.Umbraco.ContentList.Composition;

[DataEditor(
    "our.umbraco.contentlist.query",
    EditorType.PropertyValue,
    "[ContentList] Query",
    "~/App_Plugins/Our.Umbraco.ContentList/propertyeditors/query/contentlist.query.html",
    ValueType = "JSON"
)]
public class ContentListQueryEditor : DataEditor
{
    public ContentListQueryEditor(IDataValueEditorFactory dataValueEditorFactory, EditorType type = EditorType.PropertyValue) : base(dataValueEditorFactory, type)
    {
    }
}