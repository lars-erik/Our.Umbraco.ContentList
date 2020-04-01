using System;
using System.Web;

namespace Our.Umbraco.ContentList.Web.Models.ContentModels
{
    public partial class Page
    {
        public string ListImageUrl => ListImage?.Url;
        public string ContentTypeName => ContentType.Alias;
        public DateTime SortDate => CreateDate;
    }
}