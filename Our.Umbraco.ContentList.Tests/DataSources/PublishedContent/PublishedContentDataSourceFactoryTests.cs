using System;
using System.Collections.Generic;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources;
using Our.Umbraco.ContentList.DataSources.Listables;
using Our.Umbraco.ContentList.DataSources.PublishedContent;
using Our.Umbraco.ContentList.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.Tests.DataSources.PublishedContent
{
    [TestFixture]
    public class PublishedContentDataSourceFactoryTests
    {
        [Test]
        public void Finds_Metadata_For_Data_Source()
        {
            var expectedParams = new List<DataSourceParameterDefinition>
            {
                PublishedContentSorting.Parameter
            };

            var parameters = PublishedContentDataSourceFactory.CreateParameters(typeof(PublishedContentChildrenDataSource));

            Assert.AreEqual(JsonConvert.SerializeObject(expectedParams), JsonConvert.SerializeObject(parameters));
        }

        [Test]
        public void Has_List_Of_DataSources_With_Metadata()
        {
            var datasources = PublishedContentDataSourceFactory.GetDataSources();
            Assert.AreNotEqual(0, datasources.Count);
            Console.WriteLine(JsonConvert.SerializeObject(datasources, Formatting.Indented));
        }

        [Test]
        public void Creates_Datasource()
        {
            var contentListParams = new ContentListQuery
            {
                DataSource = new ContentListDataSource
                {
                    Type = typeof(PublishedContentChildrenDataSource).FullName,
                    Parameters = new List<DataSourceParameterValue>
                    {
                        new DataSourceParameterValue
                        {
                            Key = "sort",
                            Value = "dateasc"
                        }
                    }
                }
            };
            var content = Mock.Of<IPublishedContent>();

            var datasource = new PublishedContentDataSourceFactory().Create(contentListParams.DataSource.Type, new QueryParameters(content, contentListParams.DataSource.Parameters));

            Assert.IsInstanceOf<PublishedContentChildrenDataSource>(datasource);
        }
    }
}
