using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.General;
using System.Threading;
using System.Net;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class MoreAboutYourBuildingDamage : SparkBasePage
    {
        #region Constants
        private class Constants
        {
            public static readonly string ActiveStepperLabel        = "More about your damage";
            public static readonly string Header                    = "Please tell us more about your building damage";
            public static readonly string ClaimNumber               = "Your claim number is ";
            public static readonly string TextAreaPlaceHolderText   = "Describe the situation and the damage";
            public static readonly string WarningText               = "Please describe the damage";

            public class InfoSection
            {
                public static readonly string Title                 = "For example";
                public static readonly string BulletPointOne        = "The roof leaked during the recent storm, and now the lounge ceiling is stained near the light fitting.";
                public static readonly string BulletPointTwo        = "Gutters overflowed during heavy rain, and we can see damage to the roof.";

            }

        }
        #endregion

        #region XPaths
        private class XPath
        {
            public static readonly string ActiveStepper     = "//button[@aria-selected='true']";
            public static readonly string Header            = "id('header')";
            public static readonly string ClaimNumber       = "id('claimNumberDisplay')";
            public static readonly string TextInput         = "id('damage-description')";
            public static readonly string NextButton        = "id('submit-button')";
            public static readonly string WarningText       = "id('helper-text-damage-description')";

            public class InfoSection
            {
                public static readonly string Title             = "id('notification-card-title')";
                public static readonly string BulletPointOne    = "//div[@data-testid='notFoundCard']//li[1]";
                public static readonly string BulletPointTwo    = "//div[@data-testid='notFoundCard']//li[2]";
            }
        }
        #endregion
        public MoreAboutYourBuildingDamage(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                isDisplayed = string.Equals(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
                GetElement(XPath.NextButton);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            if (isDisplayed)
            {
                Reporting.LogPageChange("Spark Storm Claim - Specific damage");
                Reporting.Log($"Capturing screenshot of page as shown on arrival", _driver.TakeSnapshot());
            }
            return isDisplayed;
        }

        public void DetailedUiCheckingOfBuildingDamageFreeTextPage(string claimNumber)
        {
            Reporting.LogMinorSectionHeading("Detailed UI Checking begins by triggering page validation error");
            Reporting.AreEqual(Constants.ClaimNumber+ claimNumber, GetInnerText(XPath.ClaimNumber), "claim number text is displayed");
            Reporting.AreEqual(Constants.TextAreaPlaceHolderText, GetAttribute(XPath.TextInput, "placeholder"), "text area place holder text is displayed");
            ClickToContinue();
            Reporting.AreEqual(Constants.WarningText, GetInnerText(XPath.WarningText), "Warning message for the field 'Describe the situation or damage' text area is displayed");
            Reporting.Log($"Warning message", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(Constants.InfoSection.Title, GetInnerText(XPath.InfoSection.Title), "info section title is displayed");
            Reporting.AreEqual(Constants.InfoSection.BulletPointOne, GetInnerText(XPath.InfoSection.BulletPointOne), "info section bullet point one is displayed");
            Reporting.AreEqual(Constants.InfoSection.BulletPointTwo, GetInnerText(XPath.InfoSection.BulletPointTwo), "info section bullet point two is displayed");
        }

        public void InputFreeText(string damageInformationText)
        {
            WaitForTextFieldAndEnterText(XPath.TextInput, damageInformationText, hasTypeAhead: false);
            // Due to the validation on the text field, we can't just click on the submit button
            // while that text field has focus, se sending a tab to shift focus away and left the
            // JS validation complete.fs
            GetElement(XPath.TextInput).SendKeys(Keys.Tab);
            Reporting.Log($"Capturing state of page after inputting description.", _browser.Driver.TakeSnapshot());
            Thread.Sleep(SleepTimes.T500MS);
        }

        public void ClickToContinue()
        {
            ClickControl(XPath.NextButton);
        }
    }
}
