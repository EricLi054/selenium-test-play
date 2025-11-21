using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;


namespace UIDriver.Pages.Spark.Endorsements.UpdateHowYouPay
{
    public class UhohErrorPage: SparkBasePage
    {
        #region CONSTANTS
        private static class Constant
        {
            public static readonly string Header = "Uh oh!";

            public static readonly string CantDoOnline = "Sorry, you can't do this online right now";
            public static string EndorsementBlockingMessageDueToRenewal(string date) => $"You can't update how you pay online until after your policy renews on {date}. Call us on 13 17 03 to update now.";            
        }

        #endregion

        #region XPATHS

        private static class XPath
        {
            public static readonly string Header = "//h1[text()=\"Uh oh!\"]";
            public static readonly string ErrorTitle = "id('error-page-title')";
            public static readonly string ErrorSubTitle = "id('error-page-subtitle')";
            public static class Button
            {
                public static readonly string BacktomyRAC = "//a[text()='Back to myRAC']";
            }
        }

        #endregion

        public UhohErrorPage(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);                
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Uh oh error page");           
            Reporting.Log("Uh oh error page", _browser.Driver.TakeSnapshot());
            return true;
        }
        /// <summary>
        /// Verify the error page content
        /// </summary>
        /// <param name="date">policy renewal date</param>       
        public void VerifyErrorPage(DateTime date)
        {
            Reporting.AreEqual(Constant.Header, GetInnerText(XPath.Header), "Header text");
            Reporting.AreEqual(Constant.CantDoOnline, GetInnerText(XPath.ErrorTitle), "Error page title");            
            Reporting.AreEqual(Constant.EndorsementBlockingMessageDueToRenewal(date.ToString("dd/MM/yyyy")), GetInnerText(XPath.ErrorSubTitle), "Error message");
        }

    }
}
