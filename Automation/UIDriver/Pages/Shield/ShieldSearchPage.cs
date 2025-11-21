using OpenQA.Selenium;
using Rac.TestAutomation.Common;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    public class ShieldSearchPage : BaseShieldPage
    {
        #region XPATHS
        private const string XP_BUTTON_SEARCH_TYPE = "id('s2id_finderKeySelect')";
        private const string XP_LIST_SEARCH_OPTIONS = "id('select2-results-40')/li";

        private const string XP_INPUT_SURNAME = "id('IDITForm@name')";
        private const string XP_INPUT_GIVEN_NAME = "id('IDITForm@firstName')";

        private const string XP_BUTTON_FIND_UPPER = "id('selectButtonSearch')";
        private const string XP_BUTTON_FIND_LOWER = "id('homepageButtonsB_Search')";
        #endregion

        public ShieldSearchPage(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_BUTTON_SEARCH_TYPE);
                GetElement(XP_INPUT_SURNAME);
                GetElement(XP_INPUT_GIVEN_NAME);
                GetElement(XP_BUTTON_FIND_UPPER);
                GetElement(XP_BUTTON_FIND_LOWER);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        public void FindContactByName(Contact contact)
        {
            WaitForPage(WaitTimes.T30SEC);
            if (!string.IsNullOrEmpty(contact.Surname))
            { WaitForTextFieldAndEnterText(XP_INPUT_SURNAME, contact.Surname, false); }

            if (!string.IsNullOrEmpty(contact.FirstName))
            { WaitForTextFieldAndEnterText(XP_INPUT_GIVEN_NAME, contact.FirstName, false); }

            ClickControl(XP_BUTTON_FIND_LOWER);
        }
    }
}
