using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Spark
{
    public class ConfigurationPanel : SparkBasePage
    {
        #region XPATHS
        public static class XPath
        {
            public static class Configuration
            {
                public static readonly string Label                 = "//h3[contains(text(),'Configuration')]";
                public static readonly string EnvironmentBanner     = "id('inp-npe-toggle-banner-mobile')";
                public static readonly string Environment           = "//div[contains(@id,'inp-npe-select-shield-environment-')]";
                public static readonly string OTPOverride           = "id('override-to-number-selector-input')";
                public static readonly string DropdownOptions       = "//ul[@role='listbox']" + "//li";
                public static readonly string FeatureToggle         = "//span[text()='";
                public static readonly string FeatureTogglesOptions = "']/following-sibling::div";
            }
            public static class Button
            {
                public static readonly string On                    = "/button[@value='true' or @value='On']";
                public static readonly string Off                   = "/button[@value='false' or @value='Off']";
                public static class ConfigPanel
                {
                    public static readonly string Open      = "//button[@id='dialog-npe-app-config-dialog-open-button']";
                    public static readonly string Close     = "//button[@id='dialog-npe-app-config-dialog-close-button']";
                    public static readonly string Ellipsis  = "//button[@id='npe-header-buttons-menu-toggle']";
                }
                
                
            }
        }

        #endregion

        #region Settable properties and controls
        /// <summary>
        /// Drive the control to define the Shield environment the spark application will be pointed to.
        /// "Default" will be based on DevOps configuration.
        /// </summary>
        /// <param name="environment">Desired Shield environment</param>
        public void SetSparkShieldEnvironment(string environment)
        {
            if (string.IsNullOrEmpty(environment)) return;
            WaitForSelectableAndPickFromDropdown(XPath.Configuration.Environment, XPath.Configuration.DropdownOptions, environment.ToUpper());
        }

        /// <summary>
        /// Drive the control to define the Feature Toggle/s for spark-based projects based on toggle settings derived from config.json 
        /// when running via IDE or on variable groups when running via Azure DevOps.
        /// </summary>
        /// <param name="featureToggles">Feature Toggle/s (usually an array containing multiple) and desired state of each toggle.</param>
        public void SetSparkFeatureToggleState(List<KeyValueBool> featureToggles)
        {            
            if (featureToggles?.Any() ?? false)
            {
                SetFeatureToggles(XPath.Configuration.FeatureToggle, XPath.Configuration.FeatureTogglesOptions, featureToggles);
            }
                
        }

        /// <summary>
        /// Sets Environment toggle off
        /// </summary>
        public void SetEnvironmentBannerToggle(bool toggleState)
        {
            if (_driver.TryFindElement(By.XPath(XPath.Configuration.EnvironmentBanner), out IWebElement bannerToggle))
            {
                ClickBinaryToggle(XPath.Configuration.EnvironmentBanner, XPath.Button.On, XPath.Button.Off, toggleState);
            }
        }

        public void SetOTPOverride(string overrideNumber)
        {
            if (string.IsNullOrEmpty(overrideNumber)) return;
            WaitForSelectableAndPickFromDropdown(XPath.Configuration.OTPOverride, XPath.Configuration.DropdownOptions, overrideNumber, partialMatching: true);
        }

        #endregion

        public ConfigurationPanel(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Configuration.Label);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }

        private void SetFeatureToggles(string xPathFeatureToggle, string xPathFeatureToggleOption, List<KeyValueBool> featureToggles)
        {
            foreach (var toggle in featureToggles)
            {
                try
                {
                    var toggleBasePath = $"{xPathFeatureToggle}{toggle.Key}{xPathFeatureToggleOption}";
                    if (_driver.TryFindElement(By.XPath(toggleBasePath), out IWebElement bannerToggle))
                    {
                        ClickBinaryToggle(toggleBasePath, XPath.Button.On, XPath.Button.Off, toggle.Value);
                    }
                }
                catch
                {
                    throw new NotFoundException($"'{toggle.Key}' toggle could not be driven in Configuration -> Feature Toggles.");
                }
            }
        }

        public void OpenConfigFrame()
        {
            //Open the ellipsis for mobile view
            if (_browser.DeviceName == TargetDevice.GalaxyS21 || _browser.DeviceName == TargetDevice.iPhone14)
            {
                ClickControl(XPath.Button.ConfigPanel.Ellipsis);
                Thread.Sleep(1000);
            }

            /* We can't directly detect the design system version in the app we're
             * testing, and not all upgrade to the latest. However we're seeing
             * that in a design system version introduced at the beginning of 2025
             * the Spark Configuraiton panel now has two buttons returned by the
             * XPath for the old version. The first one cannot be interacted with.
             * 
             * So if we see more than 1 returned, we can assume it is the new DS
             * and use the updated XPath instead.
             */ 
            var openButtonResults = _driver.FindElements(By.XPath(XPath.Button.ConfigPanel.Open));
            if (openButtonResults.Count > 1)
            { openButtonResults[1].Click(); }
            else
            { openButtonResults[0].Click(); }
        }

        public void CloseConfigFrame()
        {
            ClickControl(XPath.Button.ConfigPanel.Close);

            //Close the ellipsis for mobile view            
            if(_browser.DeviceName == TargetDevice.GalaxyS21)
            {
                GetElement(XPath.Button.ConfigPanel.Open).SendKeys(Keys.Escape);
            }
        }
    }
}
