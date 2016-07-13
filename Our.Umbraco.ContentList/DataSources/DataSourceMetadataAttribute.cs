using System;

namespace Our.Umbraco.ContentList.DataSources
{
    public class DataSourceMetadataAttribute : Attribute
    {
        public Type MetadataType { get; set; }

        public DataSourceMetadataAttribute(Type metadataType)
        {
            MetadataType = metadataType;
        }
    }
}
