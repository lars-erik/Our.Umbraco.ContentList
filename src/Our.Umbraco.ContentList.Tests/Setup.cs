using ApprovalTests.Reporters;
using NUnit.Framework;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Tests.Common.Builders;
using Umbraco.Cms.Tests.Common.Builders.Extensions;
using Umbraco.Cms.Tests.Common.Testing;

[assembly:UseReporter(typeof(VisualStudioReporter))]

namespace Our.Umbraco.ContentList.Tests
{
    public class Setup
    {
        public static void ContentTypes(IDataTypeService dataTypeService, IContentTypeService contentTypeService)
        {
            var textDataType = new DataTypeBuilder()
                .WithId(1)
                .WithDatabaseType(ValueStorageType.Nvarchar)
                .Build();
            dataTypeService.Save(textDataType);

            var rteTypeBuilder = new DataTypeBuilder()
                .WithId(2);
            rteTypeBuilder
                .AddEditor()
                .WithAlias("Umbraco.TinyMCE");
            var rteType = rteTypeBuilder.Build();

            dataTypeService.Save(rteType);

            var contentTypeBuilder = new ContentTypeBuilder();

            var listableContentBuilder = contentTypeBuilder
                .WithAlias("listableContent")
                .WithId(1058);
            listableContentBuilder
                .AddPropertyType()
                .WithAlias("listHeading")
                .WithDataTypeId(1);
            listableContentBuilder
                .AddPropertyType()
                .WithAlias("listSummary")
                .WithDataTypeId(2);
            var listableContentType = listableContentBuilder.Build();

            var pageBuilder = contentTypeBuilder
                .WithAlias("page")
                .WithId(1057);
            pageBuilder
                .WithParentContentType(listableContentType);
            pageBuilder
                .AddAllowedContentType()
                .WithAlias("page");

            var pageType = pageBuilder.Build();

            contentTypeService.Save(listableContentType);
            contentTypeService.Save(pageType);
        }
    }
}
