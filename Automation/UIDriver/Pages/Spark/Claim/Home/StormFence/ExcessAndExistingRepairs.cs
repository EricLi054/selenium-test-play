using System;
using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class ExcessAndExistingRepairs : SparkBasePage
    {
        #region Constants
        private class Constants
        {
            public static readonly string HeaderFence               = "Before we look at your damage";
            public static readonly string HeaderBuildingOrContents  = "Your excess";
            internal class AdviseUser
            {
                public static readonly string InvoiceNotification   = "We'll ask you to upload your invoice shortly";
                public static readonly string QuoteNotification     = "We'll ask you to upload your quote shortly";
                public class Excess
                {
                    internal static readonly string NoExcessApplies             = "You don't have an excess\r\nSo you won't need to pay anything towards the settlement of your claim.";
                    internal static readonly string Amount                      = "Your excess is $";
                    internal static readonly string BodyFenceShared             = "The excess is the amount you need to pay towards settlement of your claim.\r\nWe'll deduct your excess from your cash settlement.";
                    internal static readonly string BodyFenceNonShared          = "The excess is the amount you need to pay towards settlement of your claim.\r\nWe can't start repairs until you pay your excess. You can pay us or the repairer.";
                    internal static readonly string BodyBuildingOrContents      = "The excess is the amount you need to pay towards settlement of your claim.\r\nOnce we've assessed your claim, we'll let you know how to pay your excess.";
                }
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
            public static readonly string Header                = "id('header')";
            public static readonly string ClaimNumberDisplay    = "id('claimNumberDisplay')";
            public class Field
            {
                public static readonly string RepairsComplete   = "//div[@data-testid='repairsCompletedQuestion']";
                public static readonly string QuoteReceived     = "//div[@data-testid='repairQuoteQuestion']";
            }
            public class Button
            {
                public static readonly string Next   = "id('submit-button')";
                public static readonly string Yes    = "//button[text()='Yes']";
                public static readonly string No     = "//button[text()='No']";
            }
            public class AdviseUser
            {
                public static readonly string ExcessAdvice          = "//div[@data-testid='your-excess-container']";
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

        public ExcessAndExistingRepairs(Browser browser) : base(browser)
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
            Reporting.LogPageChange("Spark Storm Claim Page - Excess (plus Invoice/quote if Fence Only)");
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

        public void RecordClaimNumber(ClaimHome claim)
        {
            claim.ClaimNumber = ClaimNumber;
            Reporting.Log($"ClaimNumber noted as {claim.ClaimNumber}");
        }

        /// <summary>
        /// Check the content of the Excess info box advising the excess amount and
        /// explaining what the excess is.
        /// </summary>
        public void VerifyExcessContent(ClaimHome claim)
        {
            if (claim.DamagedCovers == AffectedCovers.ContentsOnly
                && claim.DamageType == HomeClaimDamageType.StormAndTempest)
            {
                var excessValueContents = claim.PolicyDetails.Excess().Find(x => x.Key == "HCN").Value;

                if (excessValueContents < 1)
                {
                    VerifyNoExcessCopy();
                }
                else
                {
                    Reporting.AreEqual($"{Constants.AdviseUser.Excess.Amount}{excessValueContents}\r\n{Constants.AdviseUser.Excess.BodyBuildingOrContents}",
                        GetInnerText(XPath.AdviseUser.ExcessAdvice), ignoreCase: true, "expected excess information content for a non-zero excess for a Contents only claim");
                }
            }
            else if (claim.DamagedCovers == AffectedCovers.BuildingOnly
                    && claim.DamageType == HomeClaimDamageType.StormAndTempest)
            {
                var excessValueBuilding = claim.PolicyDetails.Excess().Find(x => x.Key == "HB").Value;

                if (excessValueBuilding < 1)
                {
                    VerifyNoExcessCopy();
                }
                else
                {
                    Reporting.AreEqual($"{Constants.AdviseUser.Excess.Amount}{excessValueBuilding}\r\n{Constants.AdviseUser.Excess.BodyBuildingOrContents}", 
                        GetInnerText(XPath.AdviseUser.ExcessAdvice), ignoreCase: true, "expected excess information content for a non-zero excess for a Building only claim");
                }
            }
            else if (claim.DamagedCovers == AffectedCovers.BuildingOnly
                    && claim.DamageType == HomeClaimDamageType.StormDamageToFenceOnly
                    && claim.FenceDamage.IsDividingFence)
            {
                var excessValueBuildingForFence = claim.PolicyDetails.Excess().FirstOrDefault().Value;
                if (excessValueBuildingForFence < 1)
                {
                    VerifyNoExcessCopy();
                }
                else
                {
                    Reporting.AreEqual($"{Constants.AdviseUser.Excess.Amount}{excessValueBuildingForFence}\r\n{Constants.AdviseUser.Excess.BodyFenceShared}",
                        GetInnerText(XPath.AdviseUser.ExcessAdvice), ignoreCase: true, "expected excess information content for a non-zero excess for a Shared fence only claim");
                }
            }
            else if (claim.DamagedCovers == AffectedCovers.BuildingOnly
                        && claim.DamageType == HomeClaimDamageType.StormDamageToFenceOnly
                        && !claim.FenceDamage.IsDividingFence)
            {
                var excessValueBuildingForFence = claim.PolicyDetails.Excess().FirstOrDefault().Value;
                if (excessValueBuildingForFence < 1)
                {
                    VerifyNoExcessCopy();
                }
                else
                {
                    Reporting.AreEqual($"{Constants.AdviseUser.Excess.Amount}{excessValueBuildingForFence}\r\n{Constants.AdviseUser.Excess.BodyFenceNonShared}",
                        GetInnerText(XPath.AdviseUser.ExcessAdvice), ignoreCase: true, "expected excess information content for a non-zero excess for a Non-Shared fence only claim");
                }
            }
        }

        private void VerifyNoExcessCopy()
        {
            Reporting.AreEqual($"{Constants.AdviseUser.Excess.NoExcessApplies}", GetInnerText(XPath.AdviseUser.ExcessAdvice), ignoreCase: true, 
                "expected excess information copy when excess has been removed");
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

                //TODO: SPK-4899 - SPK-4967  Introduction of "We'll ask you to upload your {document} shortly notification box to be added here
                //      when Yes is selected for one of these questions.
            }
            if(claim.DamagedCovers == AffectedCovers.ContentsOnly
            || claim.DamagedCovers == AffectedCovers.BuildingOnly)
            {
                Reporting.AreEqual(Constants.HeaderBuildingOrContents, GetInnerText(XPath.Header),
                    $"expected page heading for {claim.DamagedCovers}");
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
            if(claim.DamagedCovers == AffectedCovers.BuildingOnly)
            {
                if(claim.ExpectedOutcome == ExpectedClaimOutcome.RepairsCompleted)
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
            Reporting.Log("Capturing Home Storm Claim - Excess page before continuing.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.Next);
        }
    }
}
