using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.ContentList.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.ContentList.Tests.DataSources
{
    public abstract class FixtureAbstraction
    {
        public abstract Task VerifyResult(object resultObject);
        public abstract Task<object> Execute(ContentListConfiguration configuration, IPublishedContent currentPage);
        public virtual void Setup(IServiceCollection services) {}
    }
}