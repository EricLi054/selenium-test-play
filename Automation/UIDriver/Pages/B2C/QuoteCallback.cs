using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;

namespace UIDriver.Pages.B2C
{
    public class QuoteCallback : BasePage
    {
        #region XPATHS
        private const string BASE  = "/html/body/div[starts-with(@class,'k-widget k-window')]";
        private const string PANEL = BASE + "/div[@id='callback-dialog']";

        private const string XP_HEADING        = BASE + "/div/span[@class='k-window-title']";
        private const string XP_GIVENNAME_TEXT = PANEL + "//input[@id='FirstName']";
        private const string XP_SURNAME_TEXT   = PANEL + "//input[@id='LastName']";
        private const string XP_PHONE_TEXT     = PANEL + "//input[@id='TelephoneNumber']";
        private const string XP_CONTACT_TIME   = PANEL + "//span[@aria-owns='PreferredContactTime_listbox']";
        private const string XP_CONTINUE_BTN   = PANEL + "//button[@class='btn primary animate-chevron-right dialog-button']";

        private const string XP_CLOSE_BTN      = BASE + "//div[@class='cluetip-close']";
        #endregion

        #region Settable properties and controls
        public string PreferredContactTime
        {
            get
            {
                var element = GetElement(XP_CONTACT_TIME + XPEXT_DROPDOWN_VALUE);
                return element.Text;
            }
            set => WaitForSelectableAndPickByTyping(XP_CONTACT_TIME, value);
        }

        public string FirstName
        {
            get => GetValue(XP_GIVENNAME_TEXT);
            set => WaitForTextFieldAndEnterText(XP_GIVENNAME_TEXT, value);
        }

        public string LastName
        {
            get => GetValue(XP_SURNAME_TEXT);
            set => WaitForTextFieldAndEnterText(XP_SURNAME_TEXT, value);
        }

        public string PhoneNumber
        {
            get => GetValue(XP_PHONE_TEXT);
            set => WaitForTextFieldAndEnterText(XP_PHONE_TEXT, value);
        }
        #endregion

        public QuoteCallback(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_HEADING);
                if (!heading.Text.ToLower().Equals("more information required"))
                {
                    Reporting.Log("Wrong heading text for expected driving history popup.");
                    return false;
                }
                GetElement(XP_GIVENNAME_TEXT);
                GetElement(XP_SURNAME_TEXT);
                GetElement(XP_PHONE_TEXT);
                GetElement(XP_CONTINUE_BTN);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Verifies that callback prompts have been pre-populated.
        /// It is encumbent on the calling test to know whether the
        /// current context should have this dialog pre-populated,
        /// as this class cannot determine that on its own.
        /// 
        /// If any value is incorrect, this will fail the asserts
        /// and cause the current test to end.
        /// </summary>
        public void VerifyIfHasPrepopulatedWithContactDetails(Contact contact)
        {
            Reporting.AreEqual(contact.FirstName, FirstName, ignoreCase: true);
            Reporting.AreEqual(contact.Surname, LastName, ignoreCase: true);
            Reporting.AreEqual(contact.GetPhone(), PhoneNumber);
        }

        public void ClickContinueButton()
        {
            ClickControl(XP_CONTINUE_BTN);
        }

        public void DismissCallbackDialog()
        {
            ClickControl(XP_CLOSE_BTN, skipJSScrollLogic: true);
        }
    }
}
