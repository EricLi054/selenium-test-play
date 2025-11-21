using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using UIDriver.Pages.Spark.Endorsements;


namespace UIDriver.Pages.Spark.Claim.Home
{
    public class YourPolicyPicker : SparkBasePage
    {
        #region Constants
        private class Constants
        {
            public static readonly string Header = "Which home was damaged?";
            public static readonly string SubHeader = "You can only choose one policy at a time";
            public class AdviseUser
            {
                public static readonly string NotListedTitle    = "Policy not listed";
                public static readonly string NotListedBody     = "If you'd like to discuss this or if you think there's been a mistake, call us on 13 17 03.";
            }
        }
        #endregion
        #region XPATHS
        private class Xpath
        {
            public static readonly string Header = "id('header')";
            public class Control
            {
                public static readonly string NotListedOpen     = "//a[contains(text(),'listed?')]";
                public static readonly string NotListedClose    = "//button[@aria-label='close']";
                public static readonly string Next = "id('submit-button')";
            }
            public class PolicyCard
            {
                public static string Card(string policyNumber) => $"//div[@data-testid='policyOption-{policyNumber}']//p[contains(@id,'policy-card-content-policy-details-property')]";
                public static readonly string Address       = "id('policy-card-content-policy-details-header-title-policy-0')";
                public static readonly string CoverType     = "id('policy-card-content-policy-details-header-subtitle-policy-0')";
                public static readonly string Occupancy     = "id('policy-card-content-policy-details-property-0-type-policy-0')";
                public static readonly string PolicyNumber  = "id('policy-card-content-policy-details-property-1-policy-number-policy-0')";
            }
            public class AdviseUser
            {
                public static readonly string NotListedTitle = "id('policy-not-listed-dialog-title')";
                public static readonly string NotListedBody  = "//*[@data-testid='policy-not-listed-dialog-content']";
            }
        }

        #endregion


        public YourPolicyPicker(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(Xpath.Header);
                GetElement(Xpath.Control.Next);
            }
            catch (Exception e) when (e is NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Home Claim Page - Policy picker");
            return true;
        }

        /// <summary>
        /// Check the policy card contains the correct details.
        /// </summary>
        public void VerifyHomePolicyCardIsDisplayed(Browser browser, ClaimHome claim)
        {
            using (var policyCard = new PolicyInformationComponent(browser))
            {
                Reporting.Log($"Capturing 'Your policy' page", _browser.Driver.TakeSnapshot());
                Reporting.AreEqual($"{claim.PolicyDetails.HomeAsset.Address.StreetNumber} {claim.PolicyDetails.HomeAsset.Address.StreetOrPOBox.Trim()}", policyCard.PolicyDetailsCardTitle(claim.PolicyDetails.PolicyNumber), 
                    ignoreCase: true, "expected asset address with displayed value");

                //TODO: SPK-5201 - Cover Type for Policy Card

                //TODO: SPK-5201 - Provide assertion for of Occypancy type expected value that isn't just a code
                Reporting.Log($"Occupancy type displayed = {policyCard.PolicyDetailsCardProperty(claim.PolicyDetails.PolicyNumber, 0, "type")}");

                Reporting.AreEqual($"Policy number: {claim.PolicyDetails.PolicyNumber}", policyCard.PolicyDetailsCardProperty(claim.PolicyDetails.PolicyNumber, 1, "policy-number"), 
                    ignoreCase: true, "expected policy number with displayed value");
                

                ClickControl(Xpath.Control.NotListedOpen);
                Reporting.Log($"Screenshot after opening Not Listed dialog", _browser.Driver.TakeSnapshot());
                Reporting.AreEqual(Constants.AdviseUser.NotListedTitle, GetInnerText(Xpath.AdviseUser.NotListedTitle), ignoreCase: true, 
                    "expected not listed title with displayed value");
                Reporting.AreEqual(Constants.AdviseUser.NotListedBody, GetInnerText(Xpath.AdviseUser.NotListedBody), ignoreCase: true, 
                    "expected not listed body with displayed value");
                ClickControl(Xpath.Control.NotListedClose);
            }
        }

        public void SelectPolicy(Browser browser, ClaimHome claim)
        {
            using (var policyCard = new PolicyInformationComponent(browser))
            {
                ClickControl(Xpath.PolicyCard.Card(claim.PolicyDetails.PolicyNumber));
                Reporting.Log($"Policy selected to progress", _browser.Driver.TakeSnapshot());
            }
        }

        public void SelectNext()
        {
            Reporting.Log($"Capturing snapshot of selected policy before attempting to progress to next page.", _browser.Driver.TakeSnapshot());
            ClickControl(Xpath.Control.Next);
        }
    }
}