using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Our.Umbraco.ContentList.Rss
{
    public partial class item : IListableContent, IPublicationMetadata
    {
        public string ListHeading => title;
        public string ListImageUrl => null;
        public IHtmlString ListSummary => new HtmlString(description);
        public string ReadMoreText => null;
        public string ContentTypeName => "Rss";
        public string Url => link;
        public int SortOrder => 0;
        public DateTime SortDate
        {
            get
            {
                DateTime.TryParse(pubDate, out var date);
                return date;
            }
        }

        public string Author => author;

        public DateTime Date => SortDate;
    }
}
