using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    /// <summary>
    /// Page object for the new employee screen when adding a contact
    /// to the organizational structure.
    /// </summary>
    public class NewEmployee : BaseShieldPage
    {
        // Drop down option to use for Shield automation user accounts' position.
        private const string POSITION_STAFF_MEMBER = "Staff Member";

        // Drop down option to use for Shield automation user accounts' profile.
        private const string PROFILE_COMPANY_LTD = "Company Employee not limited";

        // Drop down option to use for Shield automation user accounts' default printer.
        private const string DEFAULT_PRINTER_HAPPY = "Printer HAPPY  - Discontinued";

        public enum NEW_EMPLOYEE_TABS
        {
            EmployeeCard,
            Roles
        }

        private Dictionary<NEW_EMPLOYEE_TABS, string> _NewEmployeeTabTitles = new Dictionary<NEW_EMPLOYEE_TABS, string>()
        {
            { NEW_EMPLOYEE_TABS.EmployeeCard, "Employee Card"},
            { NEW_EMPLOYEE_TABS.Roles,        "Roles"}
        };

        #region XPATHS
        // Tabs
        private const string XP_TAB_CURRENT_TAB = "//ul[contains(@class,'idit-tabs-nav')]/li[@aria-selected='true']";
        
        // Form controls - Employee Card
        private const string XP_EMPLOYEE_POSITION         = "id('s2id_IDITForm@contactRoleId')";
        private const string XP_EMPLOYEE_POSITION_OPTIONS = "id('select2-results-1')/li/div";
        private const string XP_FIND_CONTACT_BUTTON       = "id('findContact')";
        private const string XP_CREATE_USER_BUTTON        = "id('newPerformSubAction_createUserSubAction__Link')";

        private const string XP_USER_USERNAME_INPUT   = "id('IDITForm@userVO@nameOfUser')";
        private const string XP_USER_PASSWORD_1_INPUT = "id('IDITForm@userVO@userAdditionalDataVO@userPassword')";
        private const string XP_USER_PASSWORD_2_INPUT = "id('additionalInfo(confirmPassword)')";
        private const string XP_USER_PROFILE          = "id('select2-chosen-2')";
        private const string XP_USER_PROFILE_OPTIONS  = "id('select2-results-2')/li/div";
        private const string XP_USER_DEFAULT_PRINTER  = "id('s2id_IDITForm@userVO@userAdditionalDataVO@defaultPrinterVO@id')";
        private const string XP_USER_DEFAULT_PRINTER_OPTIONS = "id('select2-results-6')/li/div";

        // Form controls - Roles
        private const string XP_FIRST_ORG_ROLES_RESULT_ROW = "id('idit-grid-table-flattendListuserRolesList_pipe_')//tr[2]/td";
        private const string XP_ADD_ROLE_BUTTON            = "id('flattendListuserRolesList|addRoleToUserRolesTable')";

        // General footer controls
        private const string XP_OK_BUTTON = "id('OK')";
        #endregion

        #region Settable properties and controls
        public NEW_EMPLOYEE_TABS CurrentTab
        {
            get
            {
                var currentTabTitle = GetElement(XP_TAB_CURRENT_TAB).GetAttribute("title");
                if (_NewEmployeeTabTitles.ContainsValue(currentTabTitle))
                    return _NewEmployeeTabTitles.First(x => x.Value == currentTabTitle).Key;

                throw new NoSuchElementException($"Could not identify current tab. Received tab text of: {currentTabTitle}.");
            }
            set
            {
                if (!_NewEmployeeTabTitles.ContainsKey(value))
                    throw new NotSupportedException("Requested tab is not supported. May not yet be implemented in test framework.");

                string tabTitle = _NewEmployeeTabTitles[value];
                var tabControl = GetElement($"//li[@title='{tabTitle}']");
                if (tabControl.GetAttribute("aria-selected") != "true")
                    tabControl.Click();
            }
        }
        #endregion

        public NewEmployee(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_EMPLOYEE_POSITION);
                GetElement(XP_FIND_CONTACT_BUTTON);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Sets "Position" to Staff Member and launches the
        /// search form to find/create a contact.
        /// </summary>
        public void SetPositionToStaffMemberAndBeginFindContact()
        {
            WaitForPage(WaitTimes.T30SEC);
            WaitForSelectableAndPickFromDropdown(XP_EMPLOYEE_POSITION, XP_EMPLOYEE_POSITION_OPTIONS, POSITION_STAFF_MEMBER);
            ClickControl(XP_FIND_CONTACT_BUTTON);
        }

        /// <summary>
        /// For a selected contact, defines the required Shield user
        /// values including login credentials.
        /// </summary>
        public void CompleteNewShieldUserFields(Contact user, string password)
        {
            ClickControl(XP_CREATE_USER_BUTTON);

            WaitForEmployeeUserDetailsToBeShown();

            WaitForTextFieldAndEnterText(XP_USER_USERNAME_INPUT, $"{user.FirstName}{user.Surname}", false);
            WaitForTextFieldAndEnterText(XP_USER_PASSWORD_1_INPUT, password, false);
            WaitForTextFieldAndEnterText(XP_USER_PASSWORD_2_INPUT, password, false);
            WaitForSelectableAndPickFromDropdown(XP_USER_PROFILE, XP_USER_PROFILE_OPTIONS, PROFILE_COMPANY_LTD);
            WaitForSelectableAndPickFromDropdown(XP_USER_DEFAULT_PRINTER, XP_USER_DEFAULT_PRINTER_OPTIONS, DEFAULT_PRINTER_HAPPY);
        }

        /// <summary>
        /// Ensures that view has switched to the Roles tab, and
        /// then initiates the wokflow to search for, and add,
        /// roles to the new employee.
        /// </summary>
        public void InitiateAddingRoleToUserAccount()
        {
            CurrentTab = NEW_EMPLOYEE_TABS.Roles;

            ClickControl(XP_FIRST_ORG_ROLES_RESULT_ROW);
            ClickControl(XP_ADD_ROLE_BUTTON);
        }

        /// <summary>
        /// Clicking the OK button will save all the values provided
        /// and complete the process of adding a employee.
        /// </summary>
        public void SaveEmployeeDetails()
        {
            ClickControl(XP_OK_BUTTON);
        }

        private bool AreEmployeeUserDetailFieldsShown()
        {
            return IsControlDisplayed(XP_USER_USERNAME_INPUT) &&
                   IsControlDisplayed(XP_USER_PASSWORD_1_INPUT) &&
                   IsControlDisplayed(XP_USER_PASSWORD_2_INPUT) &&
                   IsControlDisplayed(XP_USER_PROFILE) &&
                   IsControlDisplayed(XP_USER_DEFAULT_PRINTER);
        }

        private void WaitForEmployeeUserDetailsToBeShown()
        {
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
            var success = false;
            do
            {
                if (AreEmployeeUserDetailFieldsShown())
                {
                    success = true;
                    break;
                }
                Thread.Sleep(1000);
            } while (DateTime.Now < endTime);

            if (!success)
            { Reporting.Error("Shield did not load the user details form fields within the expected time for creating a new Shield user."); }
        }
    }
}
