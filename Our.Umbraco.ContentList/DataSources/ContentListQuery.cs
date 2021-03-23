using System.Collections.Generic;
using System.Linq;
using System.Web;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.DataSources
{
    public class ContentListQuery
    {
        public ContentListQuery()
            : this(null, new IParameterValue[0])
        {
        }

        public ContentListQuery(IPublishedContent contextContent, IEnumerable<IParameterValue> customParameters)
            : this(contextContent, customParameters?.ToDictionary(x => x.Key, x => x.Value))
        {
        }

        public ContentListQuery(IPublishedContent contextContent)
            : this(contextContent, new IParameterValue[0])
        {
        }

        public ContentListQuery(IDictionary<string, object> customParameters)
            : this(null, customParameters)
        {
        }

        public ContentListQuery(IPublishedContent contextContent, IDictionary<string, object> customParameters)
        {
            ContextContent = contextContent;
            CustomParameters = customParameters;
        }

        public IPublishedContent ContextContent { get; set; }

        public HttpContextBase HttpContext { get; set; }

        public IDictionary<string, object> CustomParameters { get; set; }

        public T CustomParameter<T>(string key)
        {
            try
            {
                return (T)CustomParameters[key];
            }
            catch
            {
                return default;
            }
        }
    }
}