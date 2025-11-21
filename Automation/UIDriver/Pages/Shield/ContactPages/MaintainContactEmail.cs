using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Text;
using System.Threading.Tasks;

namespace UIDriver.Pages.Shield
{
    public class MaintainContactEmail : BaseShieldPage
    {
        #region XPATHS
        private const string XP_BUTTON_SELECT = "id('deliverMethod')";
        private const string XP_PRIVATE_EMAIL_ADDRESS = "//tr[contains(.,'Private')]/td[contains(@aria-describedby,'contactActiveEmail_pipe__email')]/input[contains(@id,'@contactActiveEmail')]";

        #endregion

        #region Settable properties and controls
        public string PrivateEmailAddress
        {
            get => GetInnerText(XP_PRIVATE_EMAIL_ADDRESS);

            set => WaitForTextFieldAndEnterText(XP_PRIVATE_EMAIL_ADDRESS, value, false);
        }

        #endregion

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_BUTTON_SELECT);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        public MaintainContactEmail(Browser browser) : base(browser) { }


        public void ClickSelect()
        {
            ClickControl(XP_BUTTON_SELECT);
        }
    }
}
