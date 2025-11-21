using Rac.TestAutomation.Common;
using System;
using System.Threading;
using OpenQA.Selenium;

namespace UIDriver.Pages.Claims
{
    public class ClaimPreamble : BasePage
    {
        #region XPATHS
        private const string XP_HEADING             = "//span[@class='action-heading']";

        // Online Claim buttons:
        private const string XP_CLAIM_MOTOR_GENERAL = "//button[@id='PreambleCarClaim']";
        private const string XP_CLAIM_MOTORCYCLE    = "//button[@id='PreambleMotorbikeClaim']";
        private const string XP_CLAIM_BOAT          = "//button[@id='PreambleBoatClaim']";
        private const string XP_CLAIM_HOME_B_AND_C  = "//button[@id='PreambleHomeContentsClaim']";
        private const string XP_CLAIM_PERSONAL_VAL  = "//button[@id='PreamblePersonalValuablesClaim']";
        private const string XP_CLAIM_CARAVAN       = "//button[@id='PreambleCaravanClaim']";

        // Call Centre Claim buttons:
        private const string XP_CLAIM_RENTERS       = "//button[@id='PreambleRentersClaim']";
        private const string XP_CLAIM_LANDLORDS     = "//button[@id='PreambleLandlordsClaim']";
        #endregion

        public ClaimPreamble(Browser browser) : base(browser)
        {
        }

        public override bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XP_HEADING);
                GetElement(XP_CLAIM_MOTOR_GENERAL);
                GetElement(XP_CLAIM_MOTORCYCLE);
                GetElement(XP_CLAIM_BOAT);
                GetElement(XP_CLAIM_HOME_B_AND_C);
                GetElement(XP_CLAIM_PERSONAL_VAL);
                GetElement(XP_CLAIM_CARAVAN);

                Reporting.LogPageChange("Claim preamble page for anonymous claim flow");
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }

        public void ClickNewMotorCarClaim()
        {
            ClickControl(XP_CLAIM_MOTOR_GENERAL);
        }

        public void ClickNewMotorCycleClaim()
        {
            ClickControl(XP_CLAIM_MOTORCYCLE);
        }

        public void ClickNewBoatClaim()
        {
            ClickControl(XP_CLAIM_BOAT);
        }

        public void ClickNewHomeAndContentsClaim()
        {
            ClickControl(XP_CLAIM_HOME_B_AND_C);
        }

        public void ClickNewPersonalValuablesClaim()
        {
            ClickControl(XP_CLAIM_PERSONAL_VAL);
        }

        public void ClickNewCaravanClaim()
        {
            ClickControl(XP_CLAIM_CARAVAN);
        }

        public void ClickNewRentersClaim()
        {
            ClickControl(XP_CLAIM_RENTERS);
        }

        public void ClickNewLandlordClaim()
        {
            ClickControl(XP_CLAIM_LANDLORDS);
        }
    }
}
