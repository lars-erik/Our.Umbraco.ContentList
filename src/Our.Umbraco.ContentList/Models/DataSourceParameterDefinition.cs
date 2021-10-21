using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Models
{
    public class DataSourceParameterDefinition
    {
        public string Label { get; set; }

        public string Key { get; set; }

        public string View { get; set; }

        public DataSourceConfig Config { get; set; }
    }

    public class DataSourceConfig
    {

        public IList<DataSourceParameterOption> Items { get; set; }

    }

    public class DataSourceParameterOption
    {
        public string Id { get; set; }

        public string Value { get; set; }
    }
}
