using Rac.TestAutomation.Common;
using System;
using OpenQA.Selenium;
using System.Threading;

namespace UIDriver.Pages.B2C
{
    public class KnockoutDialog : BasePage
    {
        #region XPATHS
        private const string BASE              = "/html/body/div[starts-with(@class,'k-widget k-window')]";
        private const string XP_HEADING        = BASE + "/div/span[@class='k-window-title']";
        private const string XP_MESSAGE        = BASE + "/div[contains(@id, '-dialog')]";
        private const string XP_CLOSE_BTN      = BASE + "//div[@class='cluetip-close'][last()]/a/i";
        private const string TXT_HEADING       = "more information required";
        private const string TXT_MESSAGE       = "we are unable to complete this transaction online, please call us on 13 17 03.";
        #endregion

        public KnockoutDialog(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_HEADING);
                var message = GetElement(XP_MESSAGE);

                if (!heading.Text.ToLower().Equals(TXT_HEADING) || !message.Text.ToLower().Equals(TXT_MESSAGE))
                {
                    Reporting.Log($"Incorrect knock out text for heading/message. Actual heading: {heading.Text} | Actual message: {message.Text}");
                    return false;
                }
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return true;
        }

        public void CloseKnockoutDialog()
        {
            ClickControl(XP_CLOSE_BTN, skipJSScrollLogic: true);
            Thread.Sleep(2000);
        }
    }
}