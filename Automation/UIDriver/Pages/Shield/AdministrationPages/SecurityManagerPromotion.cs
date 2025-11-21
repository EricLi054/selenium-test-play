using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    /// <summary>
    /// Shield refers to this page as the "Promotion" page where
    /// a number of organizational changes can be made, including
    /// adding employees to agencies.
    /// </summary>
    public class SecurityManagerPromotion : BaseShieldPage
    {
        #region XPATHS
        private const string XP_SIDEBAR_ORG_STRUCTURE = "id('navigationTree')//a[@title='Organization Structure']";

        private const string XP_ORG_ELEMENT_TYPE        = "id('s2id_IDITForm@contactRoleId')";
        private const string XP_ORG_ELEMENT_TYPE_OPTION = "id('select2-results-1')/li/div";
        private const string XP_ORG_ELEMENT_NAME        = "id('IDITForm@orgUnitName')";
        private const string XP_ORG_SEARCH_BUTTON       = "id('searchForOrganizations')";

        private const string XP_SEARCH_RESULTS_TABLE = "id('idit-grid-table-flattendListfilterOrganizationVOList_pipe_')";
        private const string XP_FOUND_AGENCY_RESULT  = "//td[@aria-describedby='idit-grid-table-flattendListfilterOrganizationVOList_pipe__innerVO@contactRoleId' and @title='Agency']";
        private const string XP_ADD_EMPLOYEE_BUTTON  = "id('flattendListfilterOrganizationVOList|addEmployee')";
        private const string XP_RETURN_BUTTON        = "id('Return')";
        #endregion

        public SecurityManagerPromotion(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_SIDEBAR_ORG_STRUCTURE);
                GetElement(XP_RETURN_BUTTON);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Searches for an organizational agency by the given type
        /// and name. And then initiates the workflow to add a new
        /// employee.
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="elementName"></param>
        public void FindAgencyAndBeginAddEmployeeProcess(string elementType, string elementName)
        {
            WaitForPage(WaitTimes.T30SEC);

            ClickControl(XP_SIDEBAR_ORG_STRUCTURE);
            WaitForAgencySearchFields();

            WaitForSelectableAndPickFromDropdown(XP_ORG_ELEMENT_TYPE, XP_ORG_ELEMENT_TYPE_OPTION, elementType);
            WaitForTextFieldAndEnterText(XP_ORG_ELEMENT_NAME, elementName, false);
            ClickControl(XP_ORG_SEARCH_BUTTON);

            WaitForAgencySearchResult();
            ClickControl(XP_FOUND_AGENCY_RESULT);
            ClickControl(XP_ADD_EMPLOYEE_BUTTON);
        }

        public void ReturnToPreviousPage()
        {
            // Sewing in wait as Shield may be still saving from the
            // changes we'd made to the organizational structure.
            WaitForPage(WaitTimes.T30SEC);
            ClickControl(XP_RETURN_BUTTON);
        }

        private void WaitForAgencySearchFields()
        {
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
            var success = false;
            do
            {
                success = IsControlDisplayed(XP_ORG_ELEMENT_TYPE) &&
                          IsControlDisplayed(XP_ORG_ELEMENT_NAME) &&
                          IsControlDisplayed(XP_ORG_SEARCH_BUTTON);
                if (success)
                    break;

                Thread.Sleep(1000);
            } while (DateTime.Now < endTime);

            if (!success)
            { Reporting.Error("We did not see the agency search fields display in the expected time."); }
        }

        private void WaitForAgencySearchResult()
        {
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
            var success = false;
            do
            {
                success = IsControlDisplayed(XP_SEARCH_RESULTS_TABLE) &&
                          IsControlDisplayed(XP_FOUND_AGENCY_RESULT);
                if (success)
                    break;

                Thread.Sleep(1000);
            } while (DateTime.Now < endTime);

            if (!success)
            { Reporting.Error("We did find any matching agency within the expected time."); }
        }
    }
}
