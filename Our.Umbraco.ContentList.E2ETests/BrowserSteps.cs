using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Our.Umbraco.ContentList.E2ETests
{
    public abstract class BrowserSteps : Steps
    {
        protected RemoteWebDriver Driver { get; }
        protected WebDriverWait Wait { get; }

        protected BrowserSteps()
        {
            Driver = ScenarioContext.Current.ScenarioContainer.Resolve<RemoteWebDriver>();
            Wait = ScenarioContext.Current.ScenarioContainer.Resolve<WebDriverWait>();
        }
    }
}