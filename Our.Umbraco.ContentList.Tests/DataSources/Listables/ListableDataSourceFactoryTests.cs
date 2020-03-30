using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.Windows;
using NUnit.Framework;
using Our.Umbraco.ContentList.DataSources.Listables;
using Our.Umbraco.ContentList.Install;
using Umbraco.Tests.Testing;

namespace Our.Umbraco.ContentList.Tests.DataSources.Listables
{
    [TestFixture]
    [UmbracoTest]
    [UseReporter(typeof(VisualStudioReporter))]
    public class ListableDataSourceFactoryTests : UmbracoTestBase
    {
        protected override void Compose()
        {
            base.Compose();

            new ListableDataSourcesComposer().Compose(Composition);
        }

        [Test]
        public void Finds_Metadata_For_Data_Source()
        {
            var parameters = ListableDataSourceFactory.CreateParameters(typeof(ListableChildrenDataSource));
            Approvals.VerifyJson(parameters.ToJson());
        }

        [Test]
        public void Has_List_Of_DataSources_With_Metadata()
        {
            var datasources = ListableDataSourceFactory.GetDataSources();
            Assert.AreNotEqual(0, datasources.Count);
            Console.WriteLine(datasources.ToJson(true));
            Approvals.VerifyJson(datasources.ToJson());
        }

        [Test]
        public void Creates_Datasource()
        {
            var fullName = typeof(ListableChildrenDataSource).FullName;

            var datasource = new ListableDataSourceFactory().Create(fullName);

            Assert.IsInstanceOf<ListableChildrenDataSource>(datasource);
        }
    }
}
