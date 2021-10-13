using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.Models
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

        public HttpContext HttpContext { get; set; }

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
