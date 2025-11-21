using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Collections.Generic;

namespace UIDriver.Pages.Spark
{
    public class SparkEmailContactDetailMissing : SparkBasePage
    {
        #region Constants
        private class Constants
        {
            public static string Header(string firstName) => $"{firstName}, we need a contact email for you";
            public static readonly string CantContinueOnline = "We can't continue online without this.";
            public static readonly string PleaseAddEmail = "Please add an email address to your contact details in myRAC.";
            public static readonly string ActiveStepperLabel = "Contact details";
        }
        #endregion
        #region XPATHS
        public class XPath
        {
            public static readonly string Header = "id('need-contact-email')";
            public static readonly string CantContinueOnline = "id('cant-continue-online')";
            public static readonly string PleaseAddEmail = "id('please-add-email')";
            public class Button
            {
                public static readonly string AddEmail = "id('add-email-button')";
            }
        }
        #endregion

        public SparkEmailContactDetailMissing(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Button.AddEmail);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Contact details page - Missing Email");
            return true;
        }

        /// <summary>
        /// If we encounter this page, we check the copy.
        /// </summary>
        public void DetailedUiChecking(string firstName)
        {
            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            Reporting.AreEqual(Constants.Header(firstName), GetInnerText(XPath.Header), "expected header text with the displayed value");
            Reporting.AreEqual(Constants.CantContinueOnline, GetInnerText(XPath.CantContinueOnline), "expected 'can't continue online' text with the displayed value");
            Reporting.AreEqual(Constants.PleaseAddEmail, GetInnerText(XPath.PleaseAddEmail), "expected 'please add email' text with the displayed value");
        }

        public void ClickAddEmail()
        {
            Reporting.Log($"Capturing 'Contact details' page before attempting to progress myRAC by selecting 'Add email'.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.AddEmail);
        }
    }
}
