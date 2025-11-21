using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class StormFixturesAndFittingsDamageToInvestmentProperty : SparkBasePage
    {
        #region Constants
        private class Constants
        {
            public static readonly string ActiveStepperLabel    = "Fixtures and fittings";
            public static readonly string Header                = "Because this is an investment property...";
            public static readonly string NoSelection           = "Please select Yes or No";
            public class DamageQuestion
            {
                public static readonly string Label = "Did the storm cause any damage to your fixtures and fittings?";
                public class AdviseUser
                {
                    public static readonly string ExamplesTitle     = "Typical examples of fixtures and fittings";
                    public static readonly string ExamplesBody      = "Stoves\r\nBuilt-in aircon\r\nLight fittings\r\nWindow furnishings\r\nCarpet and floor rugs";
                    public static readonly string AssessYourDamage  = "Once you submit your claim, we'll get a repairer to contact you to assess your damage";
                }
            }
        }
        #endregion
        private class XPath
        {
            public static readonly string ActiveStepper = "//button[@aria-selected='true']";
            public static readonly string Header = "id('header')";
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";
            public class DamageQuestion
            {
                public static readonly string Label = "id('label-yesNoButtonGroup')"; //TODO SPK-6269 ask for better ids
                public static readonly string NoSelection = "id('helper-text-yesNoButtonGroup')"; //TODO SPK-6269 ask for better ids
                public class Button
                {
                    public static readonly string Yes = "//button[text()='Yes']";
                    public static readonly string No = "//button[text()='No']";
                }
            }
            public static readonly string NextButton = "id('submit-button')";
        }

        public StormFixturesAndFittingsDamageToInvestmentProperty(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                isDisplayed = string.Equals(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
                GetElement(XPath.DamageQuestion.Label);
                GetElement(XPath.NextButton);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            if (isDisplayed)
            {
                Reporting.LogPageChange("Spark Storm Claim - Fixtures and Fittings");
                Reporting.Log($"Capturing screenshot of page as shown on arrival", _driver.TakeSnapshot());
            }
            return isDisplayed;
        }

        /// <summary>
        /// Answer the question regarding damage to fixtures and fittings caused by the storm for investment
        /// property policies.
        /// Presently I've not seen a need to introduce a unique test data element for this, using the 
        /// ContentsDamage.IsWaterDamagedCarpets property to determine the answer to the question.
        /// </summary>
        public void WaterDamageFixturesAndFittings(ClaimHome claimHome)
        {
            if (claimHome.ContentsDamage.IsWaterDamagedCarpets)
            {
                ClickControl(XPath.DamageQuestion.Button.Yes);
            }
            else
            {
                ClickControl(XPath.DamageQuestion.Button.No);
            }
            Reporting.Log($"Capturing screenshot of page before continuing", _driver.TakeSnapshot());
        }

        public void DetailedUiChecksFixturesAndFittings()
        {
            ClickControl(XPath.NextButton);

            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
            Reporting.AreEqual(Constants.Header, GetInnerText(XPath.Header));
            Reporting.AreEqual(Constants.NoSelection, GetInnerText(XPath.DamageQuestion.NoSelection),
                "expected validation error text with value displayed");

            Reporting.AreEqual(Constants.DamageQuestion.Label, GetInnerText(XPath.DamageQuestion.Label));
        }
        public void ClickToContinue()
        {
            ClickControl(XPath.NextButton);
        }
    }
}