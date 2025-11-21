using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Text;
using System.Threading.Tasks;

namespace UIDriver.Pages.Shield
{
    public class ShieldClaimManualAction : BaseShieldPage
    {

        #region XPATHS

        private const string XP_BUTTON_NEXT = "id('Next')";

        private const string XP_CLAIMS = "//td[@title='Claims']/div/div";       
        private const string XP_CLAIMS_COMMUNICATION = "//td[@title='Claims Communications']/div/div";
        private const string XP_SETTLEMENT = "//td[@title='Settlement']/div/div";
        private const string XP_EFT_FORM = "//span[contains(.,'EFT form')]";

        #endregion

        public ShieldClaimManualAction(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_CLAIMS);
                GetElement(XP_BUTTON_NEXT);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }


        /// <summary>
        /// Select the Claim Communication
        /// Send the EFT settlement form
        /// </summary>
        public void SelectEFTForm()
        {
            ClickControl(XP_CLAIMS);
            ClickControl(XP_CLAIMS_COMMUNICATION);
            ClickControl(XP_SETTLEMENT);
            ClickControl(XP_EFT_FORM);
            ClickControl(XP_BUTTON_NEXT);
            ClickControl(XP_BUTTON_NEXT);
        }
    }
}
