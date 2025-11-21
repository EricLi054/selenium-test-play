using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace UIDriver.Pages.B2C
{
    public class FeatureTogglePage : BasePage
    {
        #region XPATHS
        private const string XP_PAGE_HEADER          = "//span[@class='action-heading']";

        //Feature Toggle List
        private const string BASE_TABLE           = "//table/tbody";
        private const string XP_GENERIC_ROW       = BASE_TABLE + "//tr";

        private const string XPR_TOGGLE_NAME      = "//td[1]/label/span";
        private const string XPR_ON_BUTTON        = "//div[@class='radioSetAnswer']//div[contains(@class,'first')]";
        private const string XPR_OFF_BUTTON       = "//div[@class='radioSetAnswer']//div[contains(@class,'last')]";

        private const string XP_BTN_SUBMIT = "//button[@id='btnSubmit']";
        private const string XP_BTN_BACK   = "//button[@id='btnBack']";
        #endregion

        public FeatureTogglePage(Browser browser) : base(browser) { }

        /// <summary>
        /// Handles full loading of feature toggle page and setting of toggle values.
        /// Only needs to be done 
        /// </summary>
        /// <param name="hdlBrowser"></param>
        /// <param name="featureToggles"></param>
        public static void ProcessB2CFeatureToggles(Browser hdlBrowser, List<KeyValueBool> featureToggles)
        {
            /***********************************************************
             * Complete Page 1 details
             ***********************************************************/
            using (var featurePage = new FeatureTogglePage(hdlBrowser))
            {
                featurePage.SetFeatureToggles(featureToggles);
                featurePage.SubmitAndReturnToLandingPage();
            }
        }

        override public bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_PAGE_HEADER);
                if (!heading.Text.ToLower().Equals("features"))
                {
                    Reporting.Log("Wrong heading text for Feature Toggle page.");
                    return false;
                }
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Takes a list of toggles to check from config.json, and modifies them.
        /// </summary>
        /// <param name="requestedToggles"></param>
        public void SetFeatureToggles(List<KeyValueBool> requestedToggles)
        {
            if (requestedToggles == null || requestedToggles.Count == 0) return;

            var rows = _driver.FindElements(By.XPath(XP_GENERIC_ROW));

            for(int i = 0; i< rows.Count; i++)
            {
                var toggleName = GetElement($"{XP_GENERIC_ROW}[{i+1}]{XPR_TOGGLE_NAME}").Text;

                foreach(var request in requestedToggles)
                {
                    if (toggleName.Equals(request.Key, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (request.Value)
                            SetToggle(i + 1);
                        else
                            ClearToggle(i + 1);
                        break;
                    }
                }
            }
        }

        public void SubmitAndReturnToLandingPage()
        {
            ClickControl(XP_BTN_SUBMIT);
        }

        /// <summary>
        /// Will enable a toggle if it is not already enabled.
        /// </summary>
        /// <param name="i"></param>
        private void SetToggle(int i)
        {
            var toggle = GetElement($"{XP_GENERIC_ROW}[{i}]{XPR_ON_BUTTON}");
            // If ON button not already checked, then click.
            if (!toggle.GetAttribute("class").ToLower().Contains("checked"))
                toggle.Click();
        }

        /// <summary>
        /// Will disable a toggle if it is not already disabled.
        /// </summary>
        /// <param name="i"></param>
        private void ClearToggle(int i)
        {
            var toggle = GetElement($"{XP_GENERIC_ROW}[{i}]{XPR_OFF_BUTTON}");
            // If OFF button not already checked, then click.
            if (!toggle.GetAttribute("class").ToLower().Contains("checked"))
                toggle.Click();
        }
    }
}
