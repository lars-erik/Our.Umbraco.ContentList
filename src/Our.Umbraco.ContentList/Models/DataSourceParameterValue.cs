using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Models
{
    public class DataSourceParameterValue : IParameterValue
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}