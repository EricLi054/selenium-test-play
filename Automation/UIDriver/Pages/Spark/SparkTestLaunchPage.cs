using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using Rac.TestAutomation.Common;


namespace UIDriver.Pages.Spark
{
    /// <summary>
    /// The Spark test launch page is intended to be a common site to access new Spark applications.
    /// It provides a means of selecting the Spark site, the Shield environment and applying toggle settings. 
    /// </summary>
    public class SparkTestLaunchPage : SparkBasePage
    {
        #region CONSTANTS
        public class Constants
        {
            public class ExternalSites
            {
                public static readonly string UpdateHowYouPay  = "How You Pay";
                public static readonly string Cancellations    = "Policy Cancellations";
                public static readonly string TriageMotor      = "Triage - Motor";
                public static readonly string MotorEndorsement = "Motor Endorsement";
                public static readonly string MotorGlassClaims = "Motor Glass Claims";
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public class Input
            {
                public const string PolicyNumber = "id('policyNumber')";
                public const string ContactId = "id('contactId')";
            }

            public class Button
            {
                public const string LoginAsContact = "id('loginAsContact')";
                public const string PolicyholderLogin = "id('submit-policyNumber')";
                public const string PolicyFindOwners = "//button[@id='findOwners-policyNumber']/span";
                public static string Login(string contactId) => $"//td[contains(text(),'{contactId}')]/ancestor::tr//button[starts-with(@data-testid,'login')]";
            }

            public class ExternalSite
            {
                public const string Selector = "id('inp-npe-select-external-site')";
                public const string Options = "//ul[@role='listbox']" + "//li";
            }

            public class ShieldEnvironment
            {
                public const string Selector = "id('inp-npe-select-shield-environment-desktop')";
                public const string Options = "//ul[@role='listbox']" + "//li";
            }

            public class FeatureToggle
            {
                public static string OptionBase(string toggleDescription) => $"//span[text()='{toggleDescription}']/following-sibling::div";
                public const string OnButtonSuffix = "/button[1]";
                public const string OffButtonSuffix = "/button[2]";
                public const string NotSetButtonSuffix = "/button[3]";
            }
        }
        #endregion

        public SparkTestLaunchPage(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XPath.Input.PolicyNumber);
                GetElement(XPath.Button.PolicyholderLogin);
                GetElement(XPath.ExternalSite.Selector);
                GetElement(XPath.ShieldEnvironment.Selector);
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }

        /// <summary>
        /// This method will attempt to login with the supplied policy number
        /// if the no Contact id is passed then it will login as PoloicyHolder
        /// otherwise it will login with the provided contact id
        /// The test launch page will find the CRM for policy holder, append it
        /// and the other settings in a query string passed to the selected external site.
        /// </summary>
        /// <param name="policyNumber"></param>
        public void LoginWithPolicy(string policyNumber, string contactId = null)
        {
            SendKeyPressesToField(XPath.Input.PolicyNumber, policyNumber);
            Reporting.Log("Test Launch Page, ready to login", _browser.Driver.TakeSnapshot());
            if (string.IsNullOrEmpty(contactId))
            {
                ClickControl(XPath.Button.PolicyholderLogin);
            }
            else
            {
                using (var spinner = new SparkSpinner(_browser))
                {
                    ClickControl(XPath.Button.PolicyFindOwners);
                    spinner.WaitForSpinnerToFinish();
                    ClickControl(XPath.Button.Login(contactId));
                }
               
            }
        
        }

        /// <summary>
        /// This method will attempt to login with the supplied contact id
        /// </summary>
        /// <param name="contactId"></param>
        public void LoginAsContact(string contactId)
        {
            SendKeyPressesToField(XPath.Input.ContactId, contactId);
            Reporting.Log("Test Launch Page, ready to login", _browser.Driver.TakeSnapshot());
            using (var spinner = new SparkSpinner(_browser))
            {
                ClickControl(XPath.Button.LoginAsContact);
                spinner.WaitForSpinnerToFinish();
            }
                
        }

        /// <summary>
        /// Selects the Spark site to use for testing.  The test launch page contains a drop down menu
        /// containing the base url for the Spark site.
        /// </summary>
        /// <param name="siteName">Site name to select for External Site dropdown</param>
        public void SetExternalSite(string siteName)
        {
            if (string.IsNullOrEmpty(siteName)) return;
            WaitForSelectableAndPickFromDropdown(XPath.ExternalSite.Selector, XPath.ExternalSite.Options, siteName);
        }

        /// <summary>
        /// Select the name of the shield environment to pass to the external site.
        /// </summary>
        /// <param name="environment">Shield Environment name to select</param>
        public void SetSparkShieldEnvironment(string environment)
        {
            if (string.IsNullOrEmpty(environment)) return;
            WaitForSelectableAndPickFromDropdown(XPath.ShieldEnvironment.Selector, XPath.ShieldEnvironment.Options, environment.ToUpper());
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
                SetFeatureToggles(featureToggles);
            }
        }

        /// <summary>
        /// Sets the toggle state for each of the Feature Toggles
        /// </summary>
        private void SetFeatureToggles(List<KeyValueBool> featureToggles)
        {
            foreach (var toggle in featureToggles)
            {
                try
                {
                    if (_driver.TryFindElement(By.XPath(XPath.FeatureToggle.OptionBase(toggle.Key)), out IWebElement bannerToggle))
                    {
                        ClickBinaryToggle(XPath.FeatureToggle.OptionBase(toggle.Key), XPath.FeatureToggle.OnButtonSuffix,
                           XPath.FeatureToggle.OffButtonSuffix, toggle.Value);
                    }
                }
                catch
                {
                    throw new NotFoundException($"'{toggle.Key}' toggle is not found in Configuration -> Feature Toggles." +
                        $"\nMake sure to use the exact feature toggle name displayed with the correct spacing and casing.");
                }
            }
        }
    }
}
