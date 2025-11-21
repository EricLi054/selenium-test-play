using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.Claim.Triage
{
    public class CarInsuranceClaim : SparkBasePage
    {
        private static readonly string XP_CRASH_OR_ACCIDENT = "//div[contains(@id,'cardgroup') and contains(@id,'CrashOrAccident')]/div/div";
        private static readonly string XP_WINDOW_GLASS = "//div[contains(@id,'cardgroup') and contains(@id,'WindowGlassOnly')]/div/div";
        private static readonly string XP_OTHER = "//div[contains(@id,'cardgroup') and contains(@id,'Other')]/div/div";

        private static readonly string XP_BTN_NEXT = "//button[@data-testid='car-insurance-claim-button']";

        public CarInsuranceClaim(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XP_WINDOW_GLASS);
                GetElement(XP_OTHER);
                GetElement(XP_BTN_NEXT);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Motor Claim - Car insurance claim");
            return true;
        }

        public void ClickWindowGlassDamage()
        {
            ClickControl(XP_WINDOW_GLASS);
        }
        public void ClickOtherDamage()
        {
            ClickControl(XP_OTHER);
        }
        public void ClickCrashOrAccident()
        {
            ClickControl(XP_CRASH_OR_ACCIDENT);
        }
        public void ClickNext()
        {
            Reporting.Log("Motor Claim - Car insurance claim", _browser.Driver.TakeSnapshot());
            ClickControl(XP_BTN_NEXT);
        }

    }
}
