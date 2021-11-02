using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Models
{
    public class ContentListDataSource
    {
        public string Type { get; set; }
        public List<DataSourceParameterValue> Parameters { get; set; } = new List<DataSourceParameterValue>();

    }
}
