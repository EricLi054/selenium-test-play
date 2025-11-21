using Rac.TestAutomation.Common;
using OpenQA.Selenium;

namespace UIDriver.Pages.Shield
{
    /// <summary>
    /// The Maintain Event page displays on closing out changes to
    /// an existing claim.
    /// </summary>
    public class MaintainEvent : BaseShieldPage
    {
        #region XPATHS
        private const string XP_TEXT_HEADER = "//div[contains(@id,'ScreenTitle')]";

        private const string XP_EMAIL_ELLIPSIS = "id('dispatchDataObject@dispatchDestinationVOGoButtonGo')/i[contains(@class,'ellipsis')]";
        private const string XP_BUTTON_APPLY = "id('appliesToAddressee')";

        private const string XP_BUTTON_FINISH = "id('Finish')";

        #endregion

        #region Settable properties and controls

        #endregion

        public MaintainEvent(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                var title = GetInnerText(XP_TEXT_HEADER);
                GetElement(XP_BUTTON_FINISH);

                isDisplayed = title.Equals("Maintain Event");
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Click the "Finish" button which should complete the changes to the
        /// claim and trigger the claim update success notification.
        /// </summary>
        public void ClickFinishUpdate()
        {
            ClickControl(XP_BUTTON_FINISH);
        }

        public void ClickApply()
        {
            ClickControl(XP_BUTTON_APPLY);
        }

        public void ClickEmailEllipsis()
        {
            ClickControl(XP_EMAIL_ELLIPSIS);
        }
    }
}
