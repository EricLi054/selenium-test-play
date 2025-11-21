using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class StormSafetyChecks : SparkBasePage
    {
        #region Constants
        private class Constants
        {
            public static readonly string ActiveStepperLabel    = "Safety checks";
            public static readonly string Header                = "We need to do some more safety checks";
            public class Questions
            {
                public static readonly string AllProblemsLabel  = "Select all the problems your home has.";
                public static readonly string NoSelection       = "Please select all options that apply";
                public class AdviseUser
                {
                    public static readonly string AssessYourDamageTitle = "Your safety is important";
                    public static readonly string AssessYourDamageBody  = "Once you submit your claim, we'll:\r\nContact you to discuss how we can help.\r\nGet a repairer to contact you to assess your damage.";
                }
            }
        }
        #endregion
        private class XPath
        {
            public static readonly string ActiveStepper = "//button[@aria-selected='true']";
            public static readonly string Header        = "id('header')";
            public static readonly string ClaimNumber   = "id('claimNumberDisplay')";
            public class Questions
            {
                public static readonly string AllProblemsLabel  = "id('problems-multi-choice-input-label')";
                public static readonly string NoSelection       = "//p[contains(text(),'that apply')]";
                public class AdviseUser
                {
                    public static readonly string AssessYourDamageTitle = "id('notification-card-title')";
                    public static readonly string AssessYourDamageBody  = "//*[contains(text(),'Once you submit your claim')]";
                }
                public class Checkbox
                {
                    public static readonly string Insecure              = "id('problems-multi-choice-input-CantSecureMyHome')";
                    public static readonly string DangerousLooseItems   = "id('problems-multi-choice-input-DangerousOrLooseItems')";
                    public static readonly string NoPower               = "id('problems-multi-choice-input-NoPower')";
                    public static readonly string NoWater               = "id('problems-multi-choice-input-NoWaterSupply')";
                    public static readonly string NoAccessKitchenBath   = "id('problems-multi-choice-input-NoAccessToKitchenOrBathroom')";
                    public static readonly string NoneOfThese           = "id('problems-multi-choice-input-NoneOfThese')";
                }
            }
            public static readonly string NextButton = "id('submit-button')";
        }

        public StormSafetyChecks(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                isDisplayed = string.Equals(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
                GetElement(XPath.Questions.AllProblemsLabel);
                GetElement(XPath.NextButton);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            if (isDisplayed)
            {
                Reporting.LogPageChange("Spark Storm Claim - Safety checks");
                Reporting.Log($"Capturing screenshot of page as shown on arrival", _driver.TakeSnapshot());
            }
            return isDisplayed;
        }

        public void AnswerSafetyChecks(ClaimHome claimHome)
        {
            if (claimHome.IsHomeInhabitable)
            {
                Reporting.Log($"Claim should not be flagged Uninhabitable, so we expect to leave '{StormSafetyCheckOptions.NoAccessKitchenBath.GetDescription()}' " +
                    $"and at least one of '{StormSafetyCheckOptions.NoPower.GetDescription()}' & '{StormSafetyCheckOptions.NoWater.GetDescription()}' unchecked.");
            }
            else 
            {
                Reporting.Log($"Claim should be flagged Uninhabitable, so we expect to check either '{StormSafetyCheckOptions.NoAccessKitchenBath.GetDescription()}' " +
                    $"or both '{StormSafetyCheckOptions.NoPower.GetDescription()}' & '{StormSafetyCheckOptions.NoWater.GetDescription()}'.");
            }
            
            if (claimHome.StormSafetyCheckboxes.Insecure)
            {
                ClickControl(XPath.Questions.Checkbox.Insecure);
                Reporting.Log($"Selecting '{StormSafetyCheckOptions.Insecure.GetDescription()}' checkbox.");
            }

            if (claimHome.StormSafetyCheckboxes.DangerousLooseItems)
            {
                ClickControl(XPath.Questions.Checkbox.DangerousLooseItems);
                Reporting.Log($"Selecting '{StormSafetyCheckOptions.DangerousLooseItems.GetDescription()}' checkbox.");
            }

            if (claimHome.StormSafetyCheckboxes.NoPower)
            {
                ClickControl(XPath.Questions.Checkbox.NoPower);
                Reporting.Log($"Selecting '{StormSafetyCheckOptions.NoPower.GetDescription()}' checkbox.");
            }

            if (claimHome.StormSafetyCheckboxes.NoWater)
            {
                ClickControl(XPath.Questions.Checkbox.NoWater);
                Reporting.Log($"Selecting '{StormSafetyCheckOptions.NoWater.GetDescription()}' checkbox.");
            }

            if (claimHome.StormSafetyCheckboxes.NoAccessKitchenBath)
            {
                ClickControl(XPath.Questions.Checkbox.NoAccessKitchenBath);
                Reporting.Log($"Selecting '{StormSafetyCheckOptions.NoAccessKitchenBath.GetDescription()}' checkbox.");
            }

            if (claimHome.StormSafetyCheckboxes.NoneOfThese)
            {
                ClickControl(XPath.Questions.Checkbox.NoneOfThese);
                Reporting.Log($"Selecting '{StormSafetyCheckOptions.NoneOfThese.GetDescription()}' checkbox.");
            }

            Reporting.Log($"Capturing screenshot before continuing.", _browser.Driver.TakeSnapshot());
        }

        /// <summary>
        /// This method checks more detail than the average test scenario, and is only invoked if 
        /// the detailedUiChecking flag is true for a given test scenario.
        /// </summary>
        public void DetailedUiCheckingSafetyChecks()
        {
            Reporting.Log($"Triggering validation by selecting Next without any input");
            ClickControl(XPath.NextButton);

            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
            Reporting.AreEqual(Constants.Header, GetInnerText(XPath.Header));
            Reporting.AreEqual(Constants.Questions.NoSelection, GetInnerText(XPath.Questions.NoSelection),
                "expected validation error text with value displayed");

            Reporting.AreEqual(Constants.Questions.AllProblemsLabel,
                GetInnerText(XPath.Questions.AllProblemsLabel));

            Reporting.Log($"Selecting an item to display 'Your safety is important' notification.");
            ClickControl(XPath.Questions.Checkbox.Insecure);

            Reporting.AreEqual(Constants.Questions.AdviseUser.AssessYourDamageTitle, 
                GetInnerText(XPath.Questions.AdviseUser.AssessYourDamageTitle), "Title of 'Your safety is important' notification");
            Reporting.AreEqual(Constants.Questions.AdviseUser.AssessYourDamageBody,
                GetInnerText(XPath.Questions.AdviseUser.AssessYourDamageBody), "Body of 'Your safety is important' notification");

            Reporting.Log($"Selecting 'None of these' to clear selected item before continuing.");
            ClickControl(XPath.Questions.Checkbox.NoneOfThese);
        }
        public void ClickToContinue()
        {
            ClickControl(XPath.NextButton);
        }
    }
}