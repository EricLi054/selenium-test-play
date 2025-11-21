using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Collections.Generic;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace UIDriver.Pages.Spark.Claim.Motor
{
    public class SparkYourPolicy : BaseMotorClaimPage
    {
        #region CONSTANTS
        private class Constants
        {
            public static readonly string Header = "Which car was involved?";
            public static readonly string ActiveStepperLabel = "Your policy";
        }

        #endregion

        #region XPATHS

        private class XPath
        {
            public static readonly string Header = "id('header')";
            public static string PolicyCard(string policyNumber) => $"//div[@data-testid='policyOption-{policyNumber}']//p[contains(@id,'policy-card-content-policy-details-property')]";
            public static readonly string PolicyNotListedLink = "//a[text()='Car insurance policy not listed?']";
            public static readonly string NextButton = "//button[text()='Next']";
        }
        
        #endregion


        #region Settable properties and controls

        public string Header => GetInnerText(XPath.Header);

        #endregion


        public SparkYourPolicy(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.NextButton);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Which car was damaged page");
            Reporting.Log("Which car was damaged page", _driver.TakeSnapshot());
            return true;
        }

        public void DetailedUiChecking()
        {
            Reporting.AreEqual(Constants.Header, Header, "Header text");
            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            Reporting.IsTrue(GetElement(XPath.PolicyNotListedLink).Displayed, "Car insurance policy not listed link displayed");
        }

        public void SelectPolicy(ClaimCar claim)
        {
            if (Config.Get().IsVisualTestingEnabled)
            {
                _browser.PercyScreenCheck(ClaimMotorGlass.YourPolicy, GetPercyIgnoreCSS());
                ClickNext();
                _browser.PercyScreenCheck(ClaimMotorGlass.YourPolicyErrorPage, GetPercyIgnoreCSS());
            }
            
            ClickControl(XPath.PolicyCard(claim.Policy.PolicyNumber));
            Reporting.Log("Your policy - before clicking next button", _driver.TakeSnapshot());
            ClickNext();
        }

        private void ClickNext()
        {            
            ClickControl(XPath.NextButton);
        }

        private List<string> GetPercyIgnoreCSS() =>
         new List<string>()
         {
               "#policy-card-content-policy-details-header-title-policy-0",
               "#policy-card-content-policy-details-header-subtitle-policy-0",
               "#policy-card-content-policy-details-property-0-policy-number-policy-0",
               "#policy-card-content-policy-details-header-title-policy-1",
               "#policy-card-content-policy-details-header-subtitle-policy-1",
               "#policy-card-content-policy-details-property-0-policy-number-policy-1"
         };
    }
}
