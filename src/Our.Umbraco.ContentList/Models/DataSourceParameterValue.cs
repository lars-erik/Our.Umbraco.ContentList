using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Models
{
    public class DataSourceParameterValue : IParameterValue
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public DataSourceParameterValue()
        {
        }

        public DataSourceParameterValue(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}