using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Web
{
    public class ListTemplate
    {
        public ListTemplate(string name)
        {
            Name = name;
            CompatibleSources = new string[0];
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("compatibleSources")]
        public string[] CompatibleSources { get; set; }

        [JsonProperty("disableColumnsSetting")]
        public bool DisableColumnsSetting { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}
