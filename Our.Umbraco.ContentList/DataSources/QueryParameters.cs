using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core.Models;

namespace Our.Umbraco.ContentList.DataSources
{
    public class QueryParameters
    {
        public QueryParameters()
            : this(null, new IParameterValue[0])
        {
        }

        public QueryParameters(IPublishedContent contextContent, IEnumerable<IParameterValue> customParameters)
            : this(contextContent, customParameters.ToDictionary(x => x.Key, x => x.Value))
        {
        }

        public QueryParameters(IPublishedContent contextContent)
            : this(contextContent, new IParameterValue[0])
        {
        }

        public QueryParameters(IDictionary<string, string> customParameters)
            : this(null, customParameters)
        {
        }

        public QueryParameters(IPublishedContent contextContent, IDictionary<string, string> customParameters)
        {
            ContextContent = contextContent;
            CustomParameters = customParameters;
        }

        public IPublishedContent ContextContent { get; set; }

        public IDictionary<string, string> CustomParameters { get; set; }
    }
}