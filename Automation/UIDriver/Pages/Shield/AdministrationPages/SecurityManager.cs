using OpenQA.Selenium;
using Rac.TestAutomation.Common;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    /// <summary>
    /// Base Security Manager page
    /// </summary>
    public class SecurityManager : BaseShieldPage
    {
        private const string CONTEXT_BAR_STRING = "Security Manager";

        #region XPATHS
        private const string XP_SETUP_PROMOTION_TABLE    = "id('idit-grid-table-setupProjectList_pipe_')";
        private const string XP_TEST_PROMOTION_PROJECT   = "//td[@aria-describedby='idit-grid-table-setupProjectList_pipe__desc' and @title='Test']";
        private const string XP_NEXT_BUTTON              = "id('Next')";
        private const string XP_RETURN_BUTTON            = "id('Return')";
        #endregion

        public SecurityManager(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_SETUP_PROMOTION_TABLE);
                GetElement(XP_NEXT_BUTTON);
                isDisplayed = ContextBarText.Equals(CONTEXT_BAR_STRING);
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Choose the Test (NPE) security manager option to be able
        /// to modify the organisation structure for test uers.
        /// </summary>
        public void SelectTestPromotionAndClickNext()
        {
            WaitForPage(WaitTimes.T30SEC);
            ClickControl(XP_TEST_PROMOTION_PROJECT);
            ClickControl(XP_NEXT_BUTTON);
        }

        /// <summary>
        /// To be used when the maintain organistional structure
        /// workflow has completed and the user is finalising the event.
        /// </summary>
        public void ReturnToPreviousPage()
        {
            ClickControl(XP_RETURN_BUTTON, waitTimeSeconds: WaitTimes.T30SEC);
        }
    }
}
