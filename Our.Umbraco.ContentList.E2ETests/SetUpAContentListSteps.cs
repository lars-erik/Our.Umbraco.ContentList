using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Our.Umbraco.ContentList.E2ETests
{
    [Binding]
    public class SetUpAContentListSteps : BrowserSteps
    {
        [Given("I log into the backoffice")]
        [Given("I am logged in to the backoffice")]
        public void GivenILogIntoTheBackoffice()
        {
            if (Driver.FindElementsByClassName("umb-avatar").Any())
                return;

            Driver.Url = "http://localhost:2763/umbraco";
            Driver.Navigate();

            if (Driver.FindElementsByClassName("umb-avatar").Any())
                return;

            var usernameBox = Driver.FindElement(By.CssSelector("[type='text']"));
            var passwordBox = Driver.FindElement(By.CssSelector("[type='password']"));
            var button = Driver.FindElement(By.CssSelector("[type='submit']"));

            usernameBox.SendKeys("admin@admin.com");
            passwordBox.SendKeys("adminadmin");
            button.Click();

            Wait.Until(d => d.FindElement(By.ClassName("umb-avatar")));
        }

        [Given("I create a new TextPage")]
        public void GivenICreateANewTextPage()
        {
            Driver.Url = "http://localhost:2763/umbraco/#/content/content/edit/1064?doctype=TextPage&create=true";
            Driver.Navigate();

            var templatesPreview = Driver.FindElements(By.ClassName("templates-preview"))[1];
            var articleWide = templatesPreview.FindElement(By.XPath(".//*[text()='Article Wide']/.."));
            articleWide.Click();
        }

        [Given("I add a Content List")]
        [When("I add a Content List")]
        public void WhenIAddAContentList()
        {
            var addButton = Driver.FindElement(By.ClassName("umb-cell-placeholder"));
            addButton.Click();

            var contentList = Driver.FindElement(By.LinkText("Content List"));
            contentList.Click();
        }

        [Then("a content list editor is added")]
        public void TheAContentListEditorIsAdded()
        {
            var editorHeading = Driver.FindElement(By.ClassName("umb-control-title"));
            Assert.That(editorHeading.Text, Is.EqualTo("Content List"));
        }

        [Then("the settings dialog is shown")]
        public void ThenTheSettingsDialogIsShown()
        {
            var dialog = Driver.FindElement(By.ClassName("umb-overlay__title"));
            Assert.That(dialog.Displayed);
        }

        [Then(@"the preview shows ""(.*)""")]
        public void ThenThePreviewShows(string expectedText)
        {
            var editor = Driver.FindElement(By.ClassName("umb-control"));
            Assert.That(editor.Text, Does.Contain(expectedText));
        }

        [Given(@"I have added a content list to a new page")]
        public void GivenIHaveAddedAContentListToANewPage()
        {
            GivenICreateANewTextPage();
            WhenIAddAContentList();
        }

        [When(@"I click the preview")]
        public void WhenIClickThePreview()
        {
            var editor = Driver.FindElementByClassName("our-content-list");
            editor.Click();
        }

    }
}
