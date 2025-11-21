using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Linq;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class SafetyAndSecurity : SparkBasePage
    {

        #region XPATHS
        public class XPath
        {
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";
            public static readonly string PropertyAddress = "id('isSafetyIssue')";
            public class Button
            {
                public static readonly string Yes = "//button[text()='Yes']";
                public static readonly string No = "//button[text()='No']";
                public static readonly string Continue = "id('submit-button')";
            }
        }
        #endregion

        #region Settable properties and controls

        public string ClaimNumber
        {
            get
            {
                var claimNumber = new String(GetElement(XPath.ClaimNumber).Text.
                    Where(x => Char.IsDigit(x)).ToArray());
                return claimNumber;
            }
        }

        public bool isTemporaryFenceRequired
        {
            get => GetBinaryToggleState(XPath.PropertyAddress, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.PropertyAddress, XPath.Button.Yes, XPath.Button.No, value);
        }


        #endregion

        public SafetyAndSecurity(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PropertyAddress);
                GetElement(XPath.Button.Continue);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Spark Fence Claim Page 4 - Safety and Security");
            return true;
        }

        public void ClickContinueButton()
        {
            ClickControl(XPath.Button.Continue);
        }

        public void AnswerTemporaryFenceQuestionAndContinue(ClaimHome claim)
        {
            if (claim.ExpectedOutcome == Constants.ClaimsHome.ExpectedClaimOutcome.AlreadyHaveRepairQuote)
            {
                claim.ClaimNumber = ClaimNumber;
            }

            isTemporaryFenceRequired = !claim.FenceDamage.IsAreaSafe;
            Reporting.Log("Safety and Security:", _driver.TakeSnapshot());
            ClickContinueButton();
        }

    }
}
