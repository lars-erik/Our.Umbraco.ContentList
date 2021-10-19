using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.ContentList.Models
{
    public class ContentListPaging
    {
        public long Page { get; set; }
        public long PageSize { get; set; }
        public long From { get; set; }
        public long To { get; set; }
        public long Total { get; set; }
        public bool ShowPaging { get; set; }

        public long Pages
        {
            get
            {
                var pages = Total / PageSize;
                pages += Total % PageSize > 0 ? 1 : 0;
                return pages;
            }
        }
    }
}
