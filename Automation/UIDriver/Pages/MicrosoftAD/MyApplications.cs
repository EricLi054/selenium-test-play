using Rac.TestAutomation.Common;
using OpenQA.Selenium;

namespace UIDriver.Pages.MicrosoftAD
{
    public class MyApplications : BasePage
    {
        private class XPath
        {
            public const string UserAvatar = "id('mectrl_headerPicture')";
        }

        public MyApplications(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            return _driver.Url.Contains("myapplications.microsoft.com");
        }
    }
}
