using System.Collections.Generic;

namespace Our.Umbraco.ContentList.Models
{
    public class ContentListModel
    {
        public ContentListConfiguration Configuration { get; set; }
        public IEnumerable<IListableContent> Items { get; set; }
        public ContentListColumnStyling ColumnStyling { get; set; }
        public ContentListPaging Paging { get; set; }
        public string Hash { get; set; }
        public ContentListQuery Query { get; set; }
    }
}
