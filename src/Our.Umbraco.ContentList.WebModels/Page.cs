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
        public string ContentTypeName => ContentType.Alias;
        public string ListImageUrl { get; }
        public string Url => this.Url();
        public DateTime SortDate => this.CreateDate;
    }
}
