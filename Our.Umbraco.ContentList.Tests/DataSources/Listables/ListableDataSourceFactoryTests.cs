using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.DataSources.Listables;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.Tests.DataSources.Listables
{
    [TestFixture]
    public class ListableDataSourceFactoryTests
    {
        [Test]
        public void Finds_Metadata_For_Data_Source()
        {
            var expectedParams = new List<DataSourceParameterDefinition>
            {
                ListableSorting.Parameter
            };

            var parameters = ListableDataSourceFactory.CreateParameters(typeof (ListableChildrenDataSource));

            Assert.AreEqual(JsonConvert.SerializeObject(expectedParams), JsonConvert.SerializeObject(parameters));
        }

        [Test]
        public void Has_List_Of_DataSources_With_Metadata()
        {
            var datasources = ListableDataSourceFactory.GetDataSources();
            Assert.AreNotEqual(0, datasources.Count);
            Console.WriteLine(JsonConvert.SerializeObject(datasources, Formatting.Indented));
        }

        [Test]
        public void Creates_Datasource()
        {
            var contentListParams = new ContentListParameters
            {
                DataSource = typeof (ListableChildrenDataSource).FullName,
                DataSourceParameters = new List<DataSourceParameterValue>
                {
                    new DataSourceParameterValue
                    {
                        Key = "sort",
                        Value = "dateasc"
                    }
                }
            };
            var content = Mock.Of<IPublishedContent>();

            var datasource = new ListableDataSourceFactory().Create(contentListParams.DataSource, new QueryParameters(content, contentListParams.DataSourceParameters));

            Assert.IsInstanceOf<ListableChildrenDataSource>(datasource);
        }
    }
}
