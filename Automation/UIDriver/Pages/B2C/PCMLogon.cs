using Rac.TestAutomation.Common;
using System;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.B2C
{
    public class PCMLogon : BasePage
    {
        #region XPATHS
        // Segments
        private const string BASE = "/html/body/div[@id='wrapper']";

        // General
        private const string XP_PAGE_HEADING = BASE + "//span[@class='action-heading']";

        // Login by Contact ID
        private const string XP_CONTACT_ID_TEXT = "//input[@id='UserId']";
        private const string XP_LOGIN_PORTFOLIO_BUTTON = "//button[@id='SubmitPolicy']";
        private const string XP_LOGIN_PORTFOLIO_NEWQUOTE_BUTTON = "//button[@id='SubmitPolicyNewQuote']";
        #endregion

        public PCMLogon(Browser browser) : base(browser) { }

        override public bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_PAGE_HEADING);
                if (!heading.Text.ToLower().StartsWith("log on"))
                {
                    Reporting.Log("Wrong heading text for PCM login page.");
                    return false;
                }
                GetElement(XP_CONTACT_ID_TEXT);
                GetElement(XP_LOGIN_PORTFOLIO_BUTTON);
                GetElement(XP_LOGIN_PORTFOLIO_NEWQUOTE_BUTTON);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Only logs the given contact ID into PCM, which should bring test
        /// to the portfolio summary.
        /// </summary>
        /// <param name="contactId"></param>
        public void LogInToPortfolioSummary(string contactId)
        {
            _driver.WaitForElementToBeVisible(By.XPath(XP_CONTACT_ID_TEXT), WaitTimes.T5SEC);
            // Brief sleep as we've had some cases where the input field is not
            // immediately ready for Selenium, even though it's marked as displayed.
            Thread.Sleep(1000);
            WaitForTextFieldAndEnterText(XP_CONTACT_ID_TEXT, contactId, false);
            ClickControl(XP_LOGIN_PORTFOLIO_BUTTON, waitTimeSeconds: WaitTimes.T30SEC);
            Reporting.Log($"Logging into PCM as Contact ID: {contactId}");
        }

        /// <summary>
        /// Uses the NPE "Policies & New quote" button to log into PCM
        /// which logs into PCM and brings up the "Get a quote" pop-up
        /// with options for all online product types .
        /// </summary>
        /// <param name="contactId"></param>
        public void LogInToPortfolioSummaryAndStartNewQuote(string contactId)
        {
            _driver.WaitForElementToBeVisible(By.XPath(XP_CONTACT_ID_TEXT), WaitTimes.T5SEC);
            WaitForTextFieldAndEnterText(XP_CONTACT_ID_TEXT, contactId, false);

            ClickControl(XP_LOGIN_PORTFOLIO_NEWQUOTE_BUTTON, waitTimeSeconds: WaitTimes.T30SEC);
        }
    }
}
