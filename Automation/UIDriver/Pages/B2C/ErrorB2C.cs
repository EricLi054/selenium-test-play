using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.B2C
{
    public class ErrorB2C : BasePage
    {
        private class XPath
        {
            public class Text
            {
                public const string Error = "//p[@class='text-heading' and contains(text(),'Sorry, the system is unavailable at this time')]";
            }
            public class Links
            {
                public const string MyInsurance = "//div[contains(@class,'callback-confirmation-link-box')]/div[text()='My Insurance']";
                public const string CarQuote    = "//div[contains(@class,'callback-confirmation-link-box')]/div[text()='Car Insurance Quote']";
                public const string HomeQuote   = "//div[contains(@class,'callback-confirmation-link-box')]/div[text()='Home Insurance Quote']";
                public const string MakeClaim   = "//div[contains(@class,'callback-confirmation-link-box')]/div[text()='Make a Claim']";
            }
        }

        public ErrorB2C(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = true;
            try
            {
                GetElement(XPath.Text.Error);
                GetElement(XPath.Links.MyInsurance);
                GetElement(XPath.Links.CarQuote);
                GetElement(XPath.Links.HomeQuote);
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            catch (StaleElementReferenceException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }
    }
}