using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    /// <summary>
    /// Page object related to the Employee Card page (when adding
    /// contacts to the organizational structure). This page relates
    /// to finding roles to grant to the related employee.
    /// 
    /// NOTE: roles can only be found, if the Shield login you are
    /// currently using has the authority to grant them.
    /// </summary>
    public class FindEmployeeRoles : BaseShieldPage
    {
        #region XPATHS
        private const string XP_ROLE_SEARCH_INPUT  = "id('additionalInfo(addNewUserRoleFilter)')";
        private const string XP_ROLE_SEARCH_BUTTON = "id('searchForRoles')";

        private const string XP_FIRST_ROLE_RESULT    = "id('idit-grid-table-currentUserRolesPVOListToAdd_pipe_')//tr[2]/td";
        private const string XPR_IS_GRANTED_CHECKBOX = "//label[contains(@for,'isGranted')]";

        private const string XP_OK_BUTTON = "id('OK')";
        #endregion

        public FindEmployeeRoles(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_ROLE_SEARCH_INPUT);
                GetElement(XP_ROLE_SEARCH_BUTTON);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Searches for the first role to match the search string provided.
        /// Method will select to grant the role to the current employee,
        /// but not give authority for that user to grant it to others.
        /// </summary>
        /// <param name="roleName"></param>
        /// <exception cref="Exception">If no matches are found at all for the given string.</exception>
        public void GrantRequestedRole(string roleName)
        {
            WaitForPage(WaitTimes.T30SEC);

            WaitForTextFieldAndEnterText(XP_ROLE_SEARCH_INPUT, roleName, false);
            ClickControl(XP_ROLE_SEARCH_BUTTON);

            WaitForRoleSearchResult();

            ClickControl(XP_FIRST_ROLE_RESULT);
            // If role is not already granted, then set.
            if (!GetElement($"{XP_FIRST_ROLE_RESULT}{XPR_IS_GRANTED_CHECKBOX}").GetAttribute("title").Equals("true"))
            {
                ClickControl($"{XP_FIRST_ROLE_RESULT}{XPR_IS_GRANTED_CHECKBOX}");
            }

            ClickControl(XP_OK_BUTTON);
        }

        private void WaitForRoleSearchResult()
        {
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
            var success = false;
            do
            {
                success = IsControlDisplayed(XP_FIRST_ROLE_RESULT);
                if (success)
                    break;

                Thread.Sleep(1000);
            } while (DateTime.Now < endTime);

            if (!success)
            { Reporting.Error("We did not see any role search results display in the expected time."); }
        }
    }
}
