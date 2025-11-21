using Rac.TestAutomation.Common;
using OpenQA.Selenium;
using System;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages
{
    public abstract class BaseShieldPage : BasePage
    {
        public enum QuickSearchType
        {
            ClaimByClaimNumber,
            ClaimByPolicyNumber,
            ContactBySurnameCompanyName,
            PolicyByPolicyNumber
        }

        #region XPATHS
        // Context bar is located just below the top Shield ribbon bar.
        private const string XP_CONTEXT_BAR                 = "id('contextBar')/ul[@class='idit-breadcrumbs']";

        private const string XP_TEXT_ENVIRONMENT            = "id('distinguishingBadge')/div";
        private const string XP_INPUT_QUICKSEARCH           = "//div[@id='QuickSearchTextGoogleLikeDiv']//input[@type='text' and @id='additionalInfo(quickSearchTextGoogleLike_autocompleteSelectInputId)']";
        private const string XP_BUTTON_QUICKSEARCH_SELECTOR = "id('QuickSearchButton')";
        private const string XP_LIST_QUICKSEARCH_OPTIONS    = "id('QuickSearchButton')/../ul/li";

        private const string XP_FIND_BY_DROPDOWN            = "id('s2id_finderKeySelect')";
        private const string XP_FIND_BY_TEXT_INPUT          = "//div[@id='select2-drop']//div[@class='select2-search']//input[@type='text' and @id='s2id_autogen2_search']";
        private const string XP_SHOW_ALL_FIELDS_BUTTON      = "id('showHiedAllFieldsButton')";
        private const string XP_SEARCH_CLAIM_DETAILS        = "//header[@title='Search by Claim Details']/span[@class='fields-expander']";
        private const string XP_CLAIM_NR_FIELD              = "id('IDITForm@claimNr')";
        private const string XP_FIND_BUTTON                 = "id('homepageButtonsB_Search')";   

        private const string XP_BUTTON_USER_SELECTOR        = "id('userMenuButton')";
        private const string XP_LIST_USER_OPTIONS           = "id('userMenuButton')/../ul/li/a";

        // General notice, things like losing entered data if exiting a process or logging out.
        private const string XP_FRAME_GENERAL_NOTICE = "//div[@aria-describedby='BasicNotificationDialog']";
        private const string XP_BUTTON_NOTICE_OK     = XP_FRAME_GENERAL_NOTICE + "//button[@id='DialogOK']";
        private const string XP_BUTTON_NOTICE_CANCEL = XP_FRAME_GENERAL_NOTICE + "//button[@id='DialogCancel']";

        // Added in R1 2022, Shield dialog for home claims classification.
        private const string XP_FRAME_CLAIMS_CLASSIFICATION_NOTICE = "//div[@aria-describedby='claimsClassificationSuggestionFloatingDivDiv']";
        private const string XP_CLASSIFICATION_NOTICE_OK = XP_FRAME_CLAIMS_CLASSIFICATION_NOTICE + "//button[@id='OK']";

        // Confirmation notice, things like claim/policy updates saved
        private const string XP_FRAME_CONFIRMATION        = "//div[contains(@class,'ConfirmationPageDiv')]";
        private const string XP_TEXT_CONFIRMATION_MESSAGE = XP_FRAME_CONFIRMATION + "//h2";
        private const string XP_BUTTON_CONFIRMATION_OK    = XP_FRAME_CONFIRMATION + "//button[@id='Ok']";

        // Settings dropdown from top ribbon bar.
        private const string XP_SETTINGS_DROPDOWN_BUTTON      = "id('SettingsMainMenu')";

        // Administration Tools -> Job Scheduler
        private const string XP_SETTINGS_ADMIN_TOOLS_OPTION   = "//div[contains(@class,'SettingsMenu')]//li/a[@title='Administration Tools']";
        private const string XP_SETTINGS_JOB_SCHEDULER_OPTION = "//div[contains(@class,'SettingsMenu')]//li/a[@title='Jobs Scheduler']";

        // Setup -> Security Manager
        private const string XP_SETTINGS_SETUP_OPTION         = "//div[contains(@class,'SettingsMenu')]//li/a[@title='Setup']";
        private const string XP_SETTINGS_SECURITY_MNGR_OPTION = "//div[contains(@class,'SettingsMenu')]//li/a[@title='Security Manager']";

        // Left menu, things like Quotes, Pending Policies, Policies etc.
        private const string XP_POLICY_LNK       = "//a[@title='Policies']";
        private const string XP_POLICY_CHILD_LNK = "/following-sibling::ul[1]";
        #endregion

        #region Settable properties and controls
        public string EnvironmentName => GetInnerText(XP_TEXT_ENVIRONMENT);

        public string ContextBarText => GetInnerText(XP_CONTEXT_BAR);

        #endregion

        public BaseShieldPage(Browser browser) : base(browser) { }

        public void Logout()
        {
            ShieldDropDownPicker(XP_BUTTON_USER_SELECTOR, XP_LIST_USER_OPTIONS, "[@id='LogOff']", hasFailOver: true);

            var endTime = DateTime.Now.AddSeconds(WaitTimes.T5SEC);  // This should be plenty for the purpose of logging out.
            do
            {
                // Is the 'logged in user' button still present?
                IWebElement element;
                if (!_driver.TryFindElement(By.XPath(XP_BUTTON_USER_SELECTOR), out element))
                    // screen is expected to be in the process of logging out of session.
                    break;
                
                // Then check for error dialog and dismiss
                if (IsGeneralNoticeDialogPresent())
                {
                    DismissGeneralNoticeDialog();
                    break;
                }
            } while (DateTime.Now < endTime);
        }

        /// <summary>
        /// Use the main search fields on the Shield home page to search for a claim.
        /// Implemented to work around QuickSearch not working in v19, refer AUNT-224.
        /// </summary>
        /// <param name="searchString">Claim Number to be searched for</param>
        public void SlowSearchByClaimNumber(string searchString)
        {
            Reporting.Log($"Searching for claim via main search fields because Quicksearch is unreliable");
            ClickControl(XP_FIND_BY_DROPDOWN);
            Reporting.Log($"Inputting the 'Claim' as the 'Find By' option");
            WaitForTextFieldAndEnterText(XP_FIND_BY_TEXT_INPUT, "Claim");
            Thread.Sleep(1000);
            Reporting.Log($"Selecting the Show All Fields control");
            ClickControl(XP_SHOW_ALL_FIELDS_BUTTON);
            Thread.Sleep(1000);
            Reporting.Log($"Selecting Serach by Claim Details");
            ClickControl(XP_SEARCH_CLAIM_DETAILS);
            Thread.Sleep(1000);
            Reporting.Log($"Inputting Claim Number");
            WaitForTextFieldAndEnterText(XP_CLAIM_NR_FIELD, searchString);
        }

        // TODO AUNT-224 are we able to expect/support QuickSearch in Shield? 
        /// <summary>
        /// Uses the QuickSearch control in the top right of Shield to search for records.
        /// </summary>
        /// <param name="searchType">The type of record you want to search for (e.g. 'ClaimByPolicyNumber')</param>
        /// <param name="searchString">The string to search with, for example a Policy or Claim number.</param>
        public void QuickSearch(QuickSearchType searchType, string searchString)
        {
            string searchTypeString = "";
            switch(searchType)
            {
                case QuickSearchType.ClaimByClaimNumber:
                    searchTypeString = "Claim by Claim No.";
                    break;
                case QuickSearchType.ClaimByPolicyNumber:
                    searchTypeString = "Claim by Policy No.";
                    break;
                case QuickSearchType.ContactBySurnameCompanyName:
                    searchTypeString = "Contact by last Name/Company";
                    break;
                case QuickSearchType.PolicyByPolicyNumber:
                    searchTypeString = "Policy by Policy No.";
                    break;
                default :
                    Reporting.Error($"QuickSearchType {searchType} is not supported in Shield.");
                    break;
            }
            ShieldDropDownPicker(XP_BUTTON_QUICKSEARCH_SELECTOR, XP_LIST_QUICKSEARCH_OPTIONS, $"[@title='{searchTypeString}']");

            Thread.Sleep(2000);

            WaitForTextFieldAndEnterText(XP_INPUT_QUICKSEARCH, searchString);
        }

        public void DismissGeneralNoticeDialog()
        {
            IWebElement element;
            if (_driver.TryFindElement(By.XPath(XP_FRAME_GENERAL_NOTICE), out element))
            {
                if (element.Displayed)
                {
                    ClickControl(XP_BUTTON_NOTICE_OK);
                }
            }
        }

        public bool IsGeneralNoticeDialogPresent()
        {
            IWebElement element;
            if (_driver.TryFindElement(By.XPath(XP_FRAME_GENERAL_NOTICE), out element))
            {
                return element.Displayed;
            }
            return false;
        }

        public void DismissConfirmationDialog()
        {
            Reporting.Log($"Capturing snapshot of dialog box before dismissing", _browser.Driver.TakeSnapshot());
            ClickControl(XP_BUTTON_CONFIRMATION_OK);
        }

        /// <summary>
        /// Shield does not always display the claims classification
        /// dialog. If it is seen, we Click Ok to dismiss it.
        /// </summary>
        /// <returns>TRUE if seen and dismissed. FALSE if not seen.</returns>
        public bool DismissHomeClaimsClassificationDialog()
        {
            var wasClassificationDialogSeen = false;
            try
            {
                if (_driver.TryWaitForElementToBeVisible(By.XPath(XP_FRAME_CLAIMS_CLASSIFICATION_NOTICE), waitTimeSeconds: WaitTimes.T10SEC, out IWebElement panel))
                {
                    wasClassificationDialogSeen = true;
                    ClickControl(XP_CLASSIFICATION_NOTICE_OK);
                }
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return wasClassificationDialogSeen;
        }

        /// <summary>
        /// Opens the Settings dropdown from the ribbon bar
        /// and navigates through to Administration Tools
        /// to open the Shield batch job scheduler
        /// </summary>
        public void OpenJobScheduler()
        {
            ClickControl(XP_SETTINGS_DROPDOWN_BUTTON);
            ClickControl(XP_SETTINGS_ADMIN_TOOLS_OPTION);
            ClickControl(XP_SETTINGS_JOB_SCHEDULER_OPTION);
        }

        /// <summary>
        /// Opens the Settings dropdown from the ribbon bar
        /// and navigates through to Setup to open the
        /// Security Manager.
        /// </summary>
        public void OpenSecurityManager()
        {
            ClickControl(XP_SETTINGS_DROPDOWN_BUTTON);
            ClickControl(XP_SETTINGS_SETUP_OPTION);
            ClickControl(XP_SETTINGS_SECURITY_MNGR_OPTION);
        }

        /// <summary>
        /// Generic wait for a Shield confirmation dialog generally shown
        /// after an "Update/Create" task.
        /// We capture a snapshot whether or not the dialog is presented/dismissed to 
        /// provide context if a test needs debugging.
        /// </summary>
        /// <param name="textToVerify">Optional field to validate body text of dialog.</param>
        public void WaitForAndClearGenericConfirmationDialog(string textToVerify = null)
        {
            var found = false;
            var endtime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
            do
            {
                Thread.Sleep(1000);
                if (IsConfirmationDialogShown(textToVerify))
                {
                    Thread.Sleep(3000);
                    DismissConfirmationDialog();
                    found = true;
                    break;
                }
            } while (DateTime.Now < endtime);

            if (!found)
            {
                Reporting.Log("NOTICE: We did not observe expected confirmation dialog, but that does not mean it didn't occur, " +
                              "as Shield R9 onwards auto-dismisses the confirmation now. If the test failed, review Shield actions " +
                              "to see if they were successful.", _browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Generic wait for a Shield informational dialog generally shown
        /// after an simple action task (e.g. transmit info to 3rd party)
        /// </summary>
        public void WaitForAndDismissGeneralNotificationDialog()
        {
            var found = false;
            var endtime = DateTime.Now.AddSeconds(WaitTimes.T10SEC);
            do
            {
                Thread.Sleep(SleepTimes.T1SEC);
                if (IsGeneralNoticeDialogPresent())
                {
                    Thread.Sleep(SleepTimes.T3SEC);
                    DismissGeneralNoticeDialog();
                    found = true;
                    break;
                }
            } while (DateTime.Now < endtime);

            if (!found)
                Reporting.Error("Did not observe expected information dialog.");
        }

        /// <summary>
        /// Selects the policy by policy number from the Shield left menu
        /// </summary>
        /// <param name="policyNumber"></param>
        public void SelectPolicy(string policyNumber)
        {
            var policy = $"{XP_POLICY_LNK}{XP_POLICY_CHILD_LNK}//a[@title='{policyNumber}']";
            ClickControl(policy);
        }

        private bool IsConfirmationDialogShown(string expectedText)
        {
            var success = false;
            IWebElement element;
            if (_driver.TryFindElement(By.XPath(XP_FRAME_CONFIRMATION), out element))
            {
                if (!string.IsNullOrEmpty(expectedText))
                {
                    var displayedText = GetInnerText(XP_TEXT_CONFIRMATION_MESSAGE).Replace("\r", "").Replace("\n", "");
                    Reporting.AreEqual(expectedText,
                                       displayedText);
                }
                // If made it here, then we had the expected message, as AreEqual()
                // would cause test to abort if it failed.
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Used for dropdowns with no typing filter.
        /// </summary>
        /// <param name="xPathDropdownTrigger">XPath to the base control to click to trigger dropdown display</param>
        /// <param name="xPathDropDownListRoot">XPath that will return the collection of elements forming the dropdown</param>
        /// <param name="xPathRelativeListIdentifier">test to match to find the specific desired dropdown element</param>
        /// <param name="hasFailOver">Optional - If true, will not report error if control can't be driven</param>
        protected void ShieldDropDownPicker(string xPathDropdownTrigger, string xPathDropDownListRoot, string xPathRelativeListIdentifier, bool hasFailOver = false)
        {
            // Open the dropdown
            ClickControl(xPathDropdownTrigger, hasFailOver: hasFailOver);            

            Thread.Sleep(2000);

            // Click the desired list item.
            var xpathSpecificOption = $"{xPathDropDownListRoot}{xPathRelativeListIdentifier}";
            try
            {
                ClickControl(xpathSpecificOption, waitTimeSeconds: WaitTimes.T30SEC, hasFailOver: hasFailOver, skipJSScrollLogic: true);
            }
            catch (Exception ex) when (ex is StaleElementReferenceException)
            {
                GetElement(xpathSpecificOption).Click();                
            }
        }
    }
}

