using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;


namespace UIDriver.Pages.Spark.Endorsements.UpdateHowYouPay
{
    public class ItsRenewalTime : SparkBasePage
    {
        #region CONSTANTS
        private static class Constant
        {
            public static readonly string Header = "It's renewal time";

            public static readonly string Title = "Review, update and renew";
            public static readonly string Subtitle = "You can update how you pay if you review and renew your policy.";
            public static string Line1(string startDate) => $"Your policy auto-renews on {startDate}. Any changes you make will only apply from that date.";
            public static readonly string Line2 = "If you want your changes to apply before then, call us on 13 17 03.";
        }

        #endregion

        #region XPATHS

        private static class XPath
        {
            public static readonly string Header = "//h1[text()=\"It's renewal time\"]";

            public static readonly string Title = "id('error-page-title')";
            public static readonly string Subtitle = "id('error-page-subtitle')";
            public static readonly string Line1 = "id('renewal-time-date-message')";
            public static readonly string Line2 = "id('renewal-time-phone-message')";            

            public static class Button
            {
                public static readonly string ReviewAndRenewButton = "id('renewal-time-button')";
            }
        }

        #endregion

        public ItsRenewalTime(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Button.ReviewAndRenewButton);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("It's renewal time page");
            Reporting.Log("It's renewal time page", _browser.Driver.TakeSnapshot());
            return true;
        }

        public void VerifyPageContent(DateTime renewalDate)
        {
            Reporting.AreEqual(Constant.Header, GetInnerText(XPath.Header), "Header text");
            Reporting.AreEqual(Constant.Title, GetInnerText(XPath.Title), "Page title");
            Reporting.AreEqual(Constant.Subtitle, GetInnerText(XPath.Subtitle), "Page sub title");
            Reporting.AreEqual(Constant.Line1(renewalDate.ToString("dd/MM/yyyy")), GetInnerText(XPath.Line1), "Renewal time date message");
            Reporting.AreEqual(Constant.Line2, GetInnerText(XPath.Line2), "Renewal time phone message");
        }

        public void ClickReviewAndRenewButton()
        {
            ClickControl(XPath.Button.ReviewAndRenewButton);
        }
    }
}
