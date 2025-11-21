using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Linq;
using UIDriver.Pages.Spark.Claim.UploadInvoice;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class UploadDocuments : UploadAndSubmit
    {
        #region Constants
        public class Constants
        {
            public class Quote
            {
                public static readonly string InputLabel = "Upload your quote";
            }
            public class Invoice
            {
                public static readonly string InputLabel = "Upload your invoice";
            }
            public class NotificationCard
            {
                public static readonly string Title = "Or you can upload later";
                public static readonly string Paragraph = "We'll email you to explain how.";
            }
        }
        #endregion
        #region XPATHS
        public class XPath
        {
            public static readonly string Header = "id('header')";
            public static readonly string ClaimNumberDisplay = "id('claimNumberDisplay')";
            public static readonly string UploadInputLabel = "id('file-upload-input-label')";
            public class NotificationCard
            {
                public static readonly string Title       = "id('notification-card-title')";
                public static readonly string Paragraph   = "id('notification-card-paragraph-0')";
            }
            public class Button
            {
                public static readonly string Next = "//button[@type='submit']";
            }
        }
        #endregion
        #region Settable properties and controls
        public string ClaimNumber
        {
            get
            {
                var claimNumber = new String(GetElement(XPath.ClaimNumberDisplay).Text.
                    Where(x => Char.IsDigit(x)).ToArray());
                return claimNumber;
            }
        }
        #endregion

        public UploadDocuments(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.UploadInputLabel);
                GetElement(XPath.Button.Next);
            }
            catch (Exception e) when (e is NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Spark Fence Claim Page - Upload");
            Reporting.Log($"Claim Number : {ClaimNumber} - capturing screenshot of page as shown on arrival", _driver.TakeSnapshot());
            return true;
        }

        public void VerifyDetailedContent(ClaimHome claim)
        {
            if(claim.ExpectedOutcome.ToString() == "RepairsCompleted")
            {
                Reporting.AreEqual(Constants.Invoice.InputLabel, GetInnerText(XPath.UploadInputLabel),
                        "the expected input label for this page with the value displayed");
            }
            else
            {
                Reporting.AreEqual(Constants.Quote.InputLabel, GetInnerText(XPath.UploadInputLabel),
                        "the expected input label for this page with the value displayed");
            }

            Reporting.AreEqual(Constants.NotificationCard.Title, GetInnerText(XPath.NotificationCard.Title), 
                "expected copy for the Notification Card Title against the actual value displayed on the page");
            Reporting.AreEqual(Constants.NotificationCard.Paragraph, GetInnerText(XPath.NotificationCard.Paragraph),
                "expected copy for the Notification Card Paragraph against the actual value displayed on the page");
        }

        /// <summary>
        /// Capture a screenshot of the page for Extent Report, then select the button to confirm details
        /// and progress to the next page.
        /// </summary>
        public void ClickNext()
        {
            Reporting.Log("Capturing Home Claim - Upload page before continuing.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.Next);
        }
    }
}