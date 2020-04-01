using System;
using System.Web;

namespace Our.Umbraco.ContentList.Web.Models.ContentModels
{
    public partial interface IListableContent : Our.Umbraco.ContentList.IListableContent
    {

    }

    public partial class ListableContent
    {
        public string ListImageUrl => ListImage?.Url;
        public string ContentTypeName => ContentType.Alias;
        public DateTime SortDate => CreateDate;
    }
}