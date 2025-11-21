using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.Claim.Triage
{
    public class OtherVehicle : SparkBasePage
    {
        private static readonly string XP_ONLY_MY_CAR = "//div[contains(@id,'cardgroup') and contains(@id,'SingleVehicle')]/div/div";
        private static readonly string XP_OTHER_VEHICLE = "//div[contains(@id,'cardgroup') and contains(@id,'MultiVehicle')]/div/div";
        private static readonly string XP_DONT_KNOW = "//div[contains(@id,'cardgroup') and contains(@id,'DontKnow')]/div/div";

        private static readonly string XP_BTN_NEXT = "//button[@data-testid='other-vehicles-claim-button']";

        public OtherVehicle(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XP_ONLY_MY_CAR);
                GetElement(XP_OTHER_VEHICLE);
                GetElement(XP_BTN_NEXT);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Motor Claim - Other vehicles");
            return true;
        }

        public void ClickSingleVehicleCollisionDamage()
        {
            ClickControl(XP_ONLY_MY_CAR);
        }
        public void ClickMultiVehicleCollisionDamage()
        {
            ClickControl(XP_OTHER_VEHICLE);
        }
        public void ClickDontKnow()
        {
            ClickControl(XP_DONT_KNOW);
        }
        public void ClickNext()
        {
            Reporting.Log("Motor Claim - Other vehicles", _browser.Driver.TakeSnapshot());
            ClickControl(XP_BTN_NEXT);
        }

    }
}
