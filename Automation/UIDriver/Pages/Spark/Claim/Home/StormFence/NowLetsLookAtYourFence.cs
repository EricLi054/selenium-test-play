using System;
using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class NowLetsLookAtYourFence : SparkBasePage
    {
        #region Constants
        private class Constants
        {
            public static readonly string HeaderFence = "Now, let's look at your fence";
            internal class AdviseUser
            {
                public static readonly string InvoiceNotification   = "We'll ask you to upload your invoice shortly";
                public static readonly string QuoteNotification     = "We'll ask you to upload your quote shortly";
                public class FieldValidation
                {
                    public static readonly string RepairsCompleted  = "Please select Yes or No";
                    public static readonly string QuoteReceived     = "Please select Yes or No";
                }
            }

        }
        #endregion
        #region XPATHS
        private class XPath
        {
            public static readonly string Header = "id('header')";
            public static readonly string ClaimNumberDisplay = "id('claimNumberDisplay')";
            public class Field
            {
                public static readonly string RepairsComplete = "//div[@data-testid='repairsCompletedQuestion']";
                public static readonly string QuoteReceived = "//div[@data-testid='repairQuoteQuestion']";
            }
            public class Button
            {
                public static readonly string Next = "id('submit-button')";
                public static readonly string Yes = "//button[text()='Yes']";
                public static readonly string No = "//button[text()='No']";
            }
            public class AdviseUser
            {
                public static readonly string NotificationCardTitle = "id('notification-card-title')";
                public class FieldValidation
                {
                    public static readonly string BinaryToggleFieldValidation = "//p[contains(text(),'Yes or No')]";
                }
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

        public bool AreRepairsComplete
        {
            get => GetBinaryToggleState(XPath.Field.RepairsComplete, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.Field.RepairsComplete, XPath.Button.Yes, XPath.Button.No, value);
        }

        public bool IsQuoteReceived
        {
            get => GetBinaryToggleState(XPath.Field.QuoteReceived, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.Field.QuoteReceived, XPath.Button.Yes, XPath.Button.No, value);
        }
        #endregion

        public NowLetsLookAtYourFence (Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Button.Next);
            }
            catch (Exception e) when (e is NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Spark Storm Claim Page - Invoice/quote questions for combination fence claims");
            Reporting.Log($"Claim Number : {ClaimNumber} - capturing screenshot of page as shown on arrival", _driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// This method handles setting the answers regarding completed repairs and 
        /// quotes received depending on the ExpectedClaimOutcome value set in the 
        /// test data.
        /// We also obtain the Claim Number here as it's our first guaranteed 
        /// opportunity to do so.
        /// </summary>
        public void RepairsOrQuote(ClaimHome claim)
        {
            claim.ClaimNumber = ClaimNumber;

            switch (claim.ExpectedOutcome)
            {
                case ExpectedClaimOutcome.RepairsCompleted:
                    AreRepairsComplete = true;
                    break;
                case ExpectedClaimOutcome.AlreadyHaveRepairQuote:
                    AreRepairsComplete = false;
                    IsQuoteReceived = true;
                    break;
                default:
                    AreRepairsComplete = false;
                    IsQuoteReceived = false;
                    break;
            }
        }

        /// <summary>
        /// Verify detailed copy on this page if detailedUiChecking is true.
        /// Most tests won't invoke this level of detail.
        /// </summary>
        public void VerifyDetailedContent(ClaimHome claim)
        {
            if (claim.DamagedCovers == AffectedCovers.FenceOnly)
            {
                Reporting.AreEqual(Constants.HeaderFence, GetInnerText(XPath.Header),
                    "expected page heading for Fence Only claims");

                ClickControl(XPath.Button.Next);
                Reporting.Log($"Selected Next button to trigger display of field validation error for Repairs Complete", _browser.Driver.TakeSnapshot());

                Reporting.AreEqual(Constants.AdviseUser.FieldValidation.RepairsCompleted,
                    GetInnerText(XPath.AdviseUser.FieldValidation.BinaryToggleFieldValidation),
                    "expected empty field validation message against value displayed on page for Repairs Complete field");

                AreRepairsComplete = false;
                ClickControl(XPath.Button.Next);
                Reporting.Log($"Set Repairs Complete = No then selected Next button to trigger display of field validation error for Quote received", _browser.Driver.TakeSnapshot());

                Reporting.AreEqual(Constants.AdviseUser.FieldValidation.QuoteReceived,
                    GetInnerText(XPath.AdviseUser.FieldValidation.BinaryToggleFieldValidation),
                    "expected empty field validation message against value displayed on page for Quote Received field");
            }
        }

        /// <summary>
        /// Verify the copy on the notification card displayed if a user has indicated that they have either
        /// completed their repairs already, or have a quote for repairs.
        /// 
        /// If no card should be displayed, it checks to confirm that no card is displayed.
        /// 
        /// This is not behind the "detailUiChecking" flag because we only answer Yes to these questions on 
        /// a couple of scenarios where that flag is set to false. It's not worth adding detailed checking to 
        /// every screen just to include these.
        /// </summary>
        public void VerifyNotificationCardContent(ClaimHome claim)
        {
            if (claim.DamagedCovers == AffectedCovers.BuildingOnly)
            {
                if (claim.ExpectedOutcome == ExpectedClaimOutcome.RepairsCompleted)
                {
                    Reporting.AreEqual(Constants.AdviseUser.InvoiceNotification, GetInnerText(XPath.AdviseUser.NotificationCardTitle),
                        "expected copy for notification card advising we'll ask for the Invoice from repairs against the actual value displayed");
                }
                else if (claim.ExpectedOutcome == ExpectedClaimOutcome.AlreadyHaveRepairQuote)
                {
                    Reporting.AreEqual(Constants.AdviseUser.QuoteNotification, GetInnerText(XPath.AdviseUser.NotificationCardTitle),
                        "expected copy for notification card advising we'll ask for the Quote for repairs against the actual value displayed");
                }
                else
                {
                    Reporting.IsFalse(IsControlDisplayed(XPath.AdviseUser.NotificationCardTitle),
                        "that Notification card regarding invoices or quotes".IsNotDisplayed());
                }
            }
        }

        /// <summary>
        /// Capture a screenshot of the page for Extent Report, then select the button to confirm details
        /// and progress to the next page.
        /// </summary>
        public void ClickNext()
        {
            Reporting.Log("Capturing Home Storm Claim - Your fence repairs page before continuing.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.Next);
        }
    }
}