using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Globalization;
using UIDriver.Pages.Spark.Endorsements;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class StartClaimEventDateTime : SparkBasePage
    {
        #region Constants
        public class Constants
        {
            public static readonly string POLICY_COVER_BUILDING = "Building";
            public static readonly string POLICY_COVER_BUILDING_CONTENTS = "Building & Contents";
            public class HelpText
            {
                public static readonly string SimilarClaimPopupTitle = "You've already made a claim for a similar date";
                public static readonly string SimilarClaimPopupBody1 = "That claim was for storm damage on";
                public static readonly string SimilarClaimPopupBody2 = ". To continue that claim or ask about it, please call 13 17 03 and quote the claim number";
                public static readonly string SimilarClaimPopupButton = "Or make a new claim";
            }
        }
        #endregion

        #region XPATHS
        public class XPath
        {
            public static readonly string EventDate = "id('start-claim-date-input')";
            public static string PolicyCard(string policyNumber) => $"//div[@data-testid='policyOption-{policyNumber}']//p[contains(@id,'policy-card-content-policy-details-property')]";

            public class Button
            {
                public static readonly string PickDate = "id('start-claim-date-input')/..//button";
                public static readonly string Next = "id('submit-button')";
            }
            public class HelpText
            {
                public static readonly string SimilarClaimPopupTitle    = "id('similar-claim-confirmation-dialog-title')";
                public static readonly string SimilarClaimPopupBody     = "//div[@data-testid='similar-claim-confirmation-dialog-content']";
                public static readonly string SimilarClaimPopupButton   = "//button[@data-testid='similar-claim-confirmation-dialog-button']";
            }
        }
        #endregion

        #region Settable properties and controls
        public DateTime EventDate
        {
            get => DateTime.ParseExact(GetValue(XPath.EventDate), DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH, CultureInfo.InvariantCulture);
            set => SelectDateFromCalendar(dateFieldXPath: null, calendarBtnXPath: XPath.Button.PickDate, desiredDate: value);
        }

        /// <summary>
        /// Check whether the Similar Claim popup is displayed so we can handle it.
        /// </summary>
        public bool IsSimilarClaimFound => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.HelpText.SimilarClaimPopupTitle),
            Rac.TestAutomation.Common.Constants.General.WaitTimes.T5SEC, out IWebElement element);
        #endregion

        public StartClaimEventDateTime(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.EventDate);
                GetElement(XPath.Button.PickDate);
            }
            catch (Exception e) when (e is NoSuchElementException)
            {
                return false;
            }
            
            Reporting.LogPageChange("Spark Fence Claim Page 2 - Your Property");
            return true;
        }

        /// <summary>
        /// Verify details about the policy being claimed against from the Policy Card.
        /// </summary>
        public void VerifyPropertyDetails(Browser browser, ClaimHome claim)
        {
            using (var policyCard = new PolicyInformationComponent(browser))
            {
                if(string.IsNullOrEmpty(claim.PolicyDetails.HomeAsset.Address.StreetNumber))
                {
                    VerifyStreetNameOnly(browser, claim);
                }
                else
                {
                    Reporting.AreEqual($"{claim.PolicyDetails.HomeAsset.Address.StreetNumber} {claim.PolicyDetails.HomeAsset.Address.StreetOrPOBox.Trim()}",
                    policyCard.PolicyDetailsCardTitle(claim.PolicyDetails.PolicyNumber), ignoreCase: true, "expected asset address with displayed value");
                }
                
                Reporting.AreEqual($"Policy number: {claim.PolicyDetails.PolicyNumber}", policyCard.PolicyDetailsCardProperty(claim.PolicyDetails.PolicyNumber, 0, "policy-number"),
                    ignoreCase: true, "expected policy number with displayed value");
            }
        }

        /// <summary>
        /// Some addresses returned from Shield don't include the full expected address, for example
        /// addresses with Lot numbers. If the details returned include a null StreetNumber (meaning
        /// there is nothing for the application to display) then we use this method which will verify
        /// the street name only.
        /// </summary>
        public void VerifyStreetNameOnly(Browser browser, ClaimHome claim)
        {
            using (var policyCard = new PolicyInformationComponent(browser))
            {
                Reporting.AreEqual($"{claim.PolicyDetails.HomeAsset.Address.StreetOrPOBox}",
                    policyCard.PolicyDetailsCardTitle(claim.PolicyDetails.PolicyNumber), ignoreCase: true, "expected asset address (street name only as StreetNumber is NULL) with displayed value");
            }
        }

        /// <summary>
        /// Select the event date specified by the test data
        /// </summary>
        public void SelectEventDate(ClaimHome claim)
        {
            EventDate = claim.EventDateTime;
            Reporting.Log($"Event date input, now adding event time.");
        }

        public void ClickContinueButton()
        {
            Reporting.Log("Capturing 'Start your claim' page before continuing", _driver.TakeSnapshot());
            ClickControl(XPath.Button.Next);
        }
        
        /// <summary>
        /// This method checks for the existence of the pop-up that is displayed when another Storm claim with a similar date exists.
        /// If it is found, we validate the title and button text, then select the button to continue to report a new claim.
        /// We do not validate the body text as the value:effort ratio is not sufficient to justify identifying the other storm claim
        /// and fetching those details to provide expected values for the claim event date and claim number.
        /// </summary>
        public void CheckForExistingClaimContent()
        {
            if (IsSimilarClaimFound)
            {
                Reporting.AreEqual(Constants.HelpText.SimilarClaimPopupTitle, GetInnerText(XPath.HelpText.SimilarClaimPopupTitle), ignoreCase: true,
                    "expected title of pop-up displayed when a storm claim with a similar date exists on the policy");
                Reporting.Log($"Body of pop-up is as follows: {GetInnerText(XPath.HelpText.SimilarClaimPopupBody)}", _browser.Driver.TakeSnapshot());
                Reporting.AreEqual(Constants.HelpText.SimilarClaimPopupButton, GetInnerText(XPath.HelpText.SimilarClaimPopupButton), ignoreCase: true,
                    "button text on pop-up");
                ClickControl(XPath.HelpText.SimilarClaimPopupButton);
            }
        }
    }
}
