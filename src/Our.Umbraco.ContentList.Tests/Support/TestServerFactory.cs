using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Our.Umbraco.ContentList.Tests.Support
{
    public class TestServerFactory : WebApplicationFactory<TestServerStartup>
    {
        public TService GetRequiredService<TService>()
        {
            if (Server == null)
            {
                CreateDefaultClient();
            }

            return Server.Host.Services.GetRequiredService<TService>();
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var hostBuilder = new WebHostBuilder();
            // Solution fixing the problem:
            // https://github.com/dotnet/aspnetcore/issues/17655#issuecomment-581418168
            hostBuilder.ConfigureAppConfiguration((context, b) =>
            {
                context.HostingEnvironment.ApplicationName = typeof(Web.Program).Assembly.GetName().Name;
            });
            return hostBuilder.UseStartup<TestServerStartup>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSolutionRelativeContentRoot("Our.Umbraco.ContentList.Web");
        }
    }
}
