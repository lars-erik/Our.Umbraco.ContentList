using System.Threading.Tasks;
using ApprovalTests;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.ContentList.Components;
using Our.Umbraco.ContentList.Controllers;
using Our.Umbraco.ContentList.Models;
using Our.Umbraco.ContentList.Tests.Support;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    public class ComponentFixture : FixtureAbstraction
    {
        private UmbracoSupport support;

        public ComponentFixture(UmbracoSupport support)
        {
            this.support = support;
        }

        public override void Setup(IServiceCollection services)
        {
            services.AddRenderingSupport();
            services.AddTransient<ContentListViewComponent>();
            services.AddTransient<PagerComponent>();
        }

        public override async Task VerifyResult(object resultObject)
        {
            var viewComponentResult = (ViewViewComponentResult) resultObject;
            var result = await support.GetService<ViewRenderer>().Render(viewComponentResult.ViewName, viewComponentResult.ViewData.Model);
            Approvals.VerifyHtml(result);
        }

        public override async Task<object> Execute(ContentListConfiguration configuration, IPublishedContent currentPage)
        {
            var component = support.GetService<ContentListViewComponent>();
            var componentResult = await component.InvokeAsync(configuration, currentPage);
            return componentResult;
        }
    }
}