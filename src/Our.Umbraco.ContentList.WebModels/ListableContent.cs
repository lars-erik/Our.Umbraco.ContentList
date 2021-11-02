using System;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.Web.Models
{
    public partial interface IListableContent : Our.Umbraco.ContentList.IListableContent
    {

    }

    public partial class ListableContent
    {
        public string ContentTypeName => ContentType.Alias;
        public string ListImageUrl { get; }
        public string Url => this.Url();
        public DateTime SortDate => this.CreateDate;
    }
}
