using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.Web.Models
{
    public partial class Page
    {
        public string Url => this.Url();
        public DateTime SortDate => this.CreateDate;
    }
}
