using System;

namespace Our.Umbraco.ContentList.DataSources
{
    public class SimpleDataSourceMetadata : DataSourceMetadata
    {
        public SimpleDataSourceMetadata()
        {
        }

        public SimpleDataSourceMetadata(Type type)
        {
            Key = type.AssemblyQualifiedName;
            Name = type.Name;
        }
    }
}
