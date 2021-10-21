using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.ContentList.Composition;

namespace Our.Umbraco.ContentList.DataSources
{
    public class DataSourceMetadataFactory
    {
        private readonly IServiceProvider provider;

        public DataSourceMetadataFactory(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public IEnumerable<IDataSourceMetadata> All()
        {
            return provider.GetServices<IDataSourceMetadata>();
        }
    }
}
