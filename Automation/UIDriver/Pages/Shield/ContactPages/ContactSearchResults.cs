using OpenQA.Selenium;
using Rac.TestAutomation.Common;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    /// <summary>
    /// Handles displayed results from a contact search is made
    /// from the "ShieldSearchPage".
    /// </summary>
    public class ContactSearchResults : BaseShieldPage
    {
        #region XPATHS
        private const string XP_SEARCH_RESULTS_TABLE    = "id('idit-grid-table-searchResult_pipe_')";
        private const string XP_FIRST_SEARCH_RESULT_ROW = "id('idit-grid-table-searchResult_pipe_')//tr[2]/td";

        private const string XP_SELECT_RESULT_BUTTON = "id('selectButtonSearch')";
        #endregion

        #region Settable properties and controls
        #endregion

        public ContactSearchResults(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_SEARCH_RESULTS_TABLE);
                GetElement(XP_SELECT_RESULT_BUTTON);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Selects the first match. An exception returns if
        /// there are no results at all.
        /// </summary>
        public void SelectFirstResult()
        {
            WaitForPage(WaitTimes.T30SEC);
            ClickControl(XP_FIRST_SEARCH_RESULT_ROW);
            ClickControl(XP_SELECT_RESULT_BUTTON);
        }
    }
}
