using System;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Our.Umbraco.ContentList.E2ETests
{
    [Binding]
    public class DriverBinding
    {
        private static RemoteWebDriver driver;
        private static WebDriverWait wait;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(2);

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitlyWait(DefaultTimeout);
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, DefaultTimeout);
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            driver.Dispose();
        }

        [BeforeScenario()]
        public void BeforeScenario()
        {
            ScenarioContext.Current.ScenarioContainer.RegisterInstanceAs(driver);
            ScenarioContext.Current.ScenarioContainer.RegisterInstanceAs(wait);
        }
    }
}
