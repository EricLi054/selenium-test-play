using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace UIDriver.Pages.B2C
{
    public class CallbackConfirmation : BasePage
    {
        #region XPATHS
        private const string XP_NOTICE_HEADING = "//p[@class='text-heading']";
        private const string XP_NOTICE_TEXT    = "//p[@class='text-body']";
        #endregion

        #region Settable properties and controls
        public string NoticeHeading
        {
            get
            {
                var element = GetElement(XP_NOTICE_HEADING);
                return element.Text;
            }
        }

        public string NoticeText
        {
            get
            {
                var element = GetElement(XP_NOTICE_TEXT);
                return element.Text;
            }
        }
        #endregion

        public CallbackConfirmation(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                if (_browser.PageTitle != "RAC Insurance - CallbackConfirmation")
                {
                    Reporting.Log("Not expected page title.");
                    return false;
                }

                GetElement(XP_NOTICE_HEADING);
                GetElement(XP_NOTICE_TEXT);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }
    }
}
