using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Our.Umbraco.ContentList.Composition;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Parameters;
using Our.Umbraco.ContentList.Tests.Support;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    public class DataSourceTestBase
    {
        protected string IntegrationMode;
        protected FixtureAbstraction Fixture;
        protected UmbracoSupport Support;
        protected IUmbracoContext UmbracoContext;

        protected static readonly List<DataSourceParameterValue> DefaultParameters = new()
        {
            new() {Key = ListableSorting.Parameter.Key, Value = "sortorder"}
        };

        protected DataSourceTestBase(string integrationMode)
        {
            IntegrationMode = integrationMode;
        }

        [SetUp]
        public void Setup()
        {
            var cacheSupport = new ContentCacheSupport();
            var tree = cacheSupport.GetFromJsonResource(GetType().FullName);

            Support = new UmbracoSupport(
                tree, 
                Tests.Setup.ContentTypes,
                services =>
                {
                    services.AddContentListServices();
                    Fixture.Setup(services);
                }, 
                builder =>
                {
                    builder.AddContentListDataSources();
                });

            Fixture = IntegrationMode switch
            {
                "dataSource" => new DataSourceFixture(Support),
                "component" => new ComponentFixture(Support),
                _ => throw new NotImplementedException()
            };

            Support.Setup();
            UmbracoContext = Support.GetUmbracoContext();
        }

        [TearDown]
        public void TearDown()
        {
            Support.TearDownUmbraco();
        }

        protected static ContentListConfiguration CreateConfiguration(Type dataSourceType, string themeName, List<DataSourceParameterValue> parameters = null)
        {
            var configuration = new ContentListConfiguration
            {
                DataSource =
                {
                    Type = dataSourceType.GetFullNameWithAssembly(),
                    Parameters = parameters ?? DefaultParameters
                },
                Columns = {Large = 3, Medium = 2, Small = 1},
                PageSize = 10,
                ShowPaging = false,
                Skip = 0,
                View = themeName
            };
            return configuration;
        }

        protected static List<DataSourceParameterValue> CreateDefaultParameters()
        {
            return new List<DataSourceParameterValue>(DefaultParameters);
        }

        protected async Task<object> ExecuteSimpleTheme(Type dataSourceType, List<DataSourceParameterValue> parameters)
        {
            var configuration = CreateConfiguration(dataSourceType, "SimpleTheme", parameters);
            var result = await Fixture.Execute(configuration, UmbracoContext.Content.GetById(1000));
            return result;
        }
    }
}