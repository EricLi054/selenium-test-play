using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Text.RegularExpressions;
using UIDriver.Pages.B2C;

namespace UIDriver.Pages.PCM.CertificateOfCurrency
{
    public class RetrieveCertificateOfCurrency : BasePage
    {
        #region XPATHS

        // Change my Contact Details
        private const string XP_DOWNLOAD_COC = "//a/span[text()='Download']";
        private const string XP_EMAIL_COC    = "//a/span[text()='Email']";
        private const string XP_BACK_BUTTON  = "//a[contains(@class,'back-button')]";


        // Email pop-up
        private const string XP_DIALOG_TITLE = "id('currencyCertifcate-dialog_wnd_title')";
        private const string XP_DIALOG_TEXT = "id('currencyCertifcate-dialog')/div[1]";
        private const string XP_EMAIL1_INPUT     = "id('EmailAddress')";
        private const string XP_EMAIL2_INPUT     = "id('EmailAddress2')";
        private const string XP_SUBMIT_EMAILS    = "id('SubmitAction')";
        private const string XP_BACK_TO_POLICIES = "id('SubmitAction')";

        #endregion

        #region Constants
        private const string EmailDialog = "Please enter the email address you want to send the document to.";
        #endregion
        #region Settable properties and controls

        #endregion

        public RetrieveCertificateOfCurrency(Browser browser) : base(browser)
        {
        }

        public override bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XP_DOWNLOAD_COC);
                GetElement(XP_EMAIL_COC);

                Reporting.LogPageChange("Download/Email Certificate of Currency page");
                rendered = true;
            }
            catch (NoSuchElementException ex) { Reporting.Log($"Unable to find element: {ex}"); }
            return rendered;
        }

        /// <summary>
        /// update Contact details
        /// Enter LastName, DOB, Mobile Number,Home Phone Number, Work Phone Number, 
        /// Email Address, Postal Address Name and Preferred delivery method
        /// clicks on save button
        /// </summary>
        /// <param name="contactData"></param>
        public void RequestCertificateOfCurrencyByEmailAndReturnToPortfolioSummary(string emailRecipient)
        {
            Reporting.Log("Requesting Certificate of Currency.", _browser.Driver.TakeSnapshot());
            ClickControl(XP_EMAIL_COC);

            using(var spinner = new RACSpinner(_browser))
            {
                spinner.WaitForSpinnerToFinish();
                Reporting.AreEqual("Send document", GetInnerText(XP_DIALOG_TITLE));
                Reporting.AreEqual(EmailDialog, GetInnerText(XP_DIALOG_TEXT));
                WaitForTextFieldAndEnterText(XP_EMAIL1_INPUT, emailRecipient);
                Reporting.Log("Requested email to {XP_EMAIL1_INPUT}", _browser.Driver.TakeSnapshot());
                ClickControl(XP_SUBMIT_EMAILS);
                spinner.WaitForSpinnerToFinish();

                Reporting.AreEqual("Send document", GetInnerText(XP_DIALOG_TITLE));
                Regex thankYouTextRegex = new Regex(FixedTextRegex.POLICY_EMAIL_CERTIFICATE_OF_CURRENCY_SUCCESS_REGEX);
                var thankYouText = GetInnerText(XP_DIALOG_TEXT);
                Match match = thankYouTextRegex.Match(thankYouText);
                Reporting.Log("Checking dialog text.", _browser.Driver.TakeSnapshot());
                Reporting.IsTrue(match.Success, $"CoC email dialog thank you text. Received: {thankYouText}");
                Reporting.AreEqual(emailRecipient, match.Groups[1].Value);

                ClickControl(XP_BACK_TO_POLICIES);
                spinner.WaitForSpinnerToFinish();
            }
        }
    }
}
