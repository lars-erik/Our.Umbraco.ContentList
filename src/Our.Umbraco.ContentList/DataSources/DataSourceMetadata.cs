using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Our.Umbraco.ContentList.Models;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.DataSources
{
    public interface IDataSourceMetadata
    {
        string Key { get; }
        string Name { get; }
        IEnumerable<DataSourceParameterDefinition> Parameters { get; }
        Type For();
    }

    public abstract class DataSourceMetadata<T> : IDataSourceMetadata
        where T: IListableDataSource
    {
        public static Type For => typeof(T);

        Type IDataSourceMetadata.For() => For;

        public string Key => For.GetFullNameWithAssembly();

        public abstract string Name { get; }

        public virtual IEnumerable<DataSourceParameterDefinition> Parameters =>
            Enumerable.Empty<DataSourceParameterDefinition>();
    }
}
