using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Our.Umbraco.ContentList.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Our.Umbraco.ContentList.PropertyEditors
{
    public class QueryConverter : PropertyValueConverterBase
    {
        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias == "our.umbraco.contentlist.query";
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            return JsonConvert.DeserializeObject<ContentListConfiguration>(source?.ToString() ?? "{}");
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            return typeof(ContentListConfiguration);
        }
    }
}
