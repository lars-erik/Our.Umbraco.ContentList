using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.ContentList
{
    public interface IListableContent
    {
        public string ListHeading { get; }

        public string Url { get; }
        int SortOrder { get; }
        DateTime SortDate { get; }
    }
}
