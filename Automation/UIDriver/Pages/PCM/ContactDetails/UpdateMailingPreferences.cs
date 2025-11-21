using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using UIDriver.Pages.B2C;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.PCM.ContactDetails
{
    public class UpdateMailingPreferences : BasePage
    {
        #region Constants
        public class Constants
        {
            public class MailingPreferences
            {
                public class AdviseUser
                {
                    public static readonly string EmailInfoBox  = "Where possible, we'll email your documents to you. We may need to send some documents by post.";
                    public static readonly string EmailMissing  = "There's no email in your contact details. Please return to myRAC, select 'Edit your contact details' and update your contact email.";
                }
            }
            public class Confirmation
            {
                public static readonly string ConfirmationText  = "You've updated your mail preferences for your policy documents.";
            }
        }
        #endregion
        #region XPATHS
        public class XPath
        {
            public class MailingPreferences
            {
                public static readonly string EmailOption   = "//input[@id='RenewPolicyUsingEmail_True']/following-sibling::span[1]";
                public static readonly string PrintOption   = "//input[@id='RenewPolicyUsingEmail_False']/following-sibling::span[1]";
                public static readonly string Submit        = "id('accordion_0_submit-action')";
                public class AdviseUser
                {
                    public static readonly string EmailInfoBox  = "id('RenewPolicyUsingEmail_InfoBox')";
                    public static readonly string EmailMissing  = "id('no-email-error')";
                }
            }
            public class Confirmation
            {
                public static readonly string MessageContent = "//div[@data-accordion-panel-id='ConfirmationViewModel' and contains(@class,'opened')]//div[@class='pcm-content-text']";
            }
        }
        #endregion

        public UpdateMailingPreferences(Browser browser) : base(browser)
        {
        }

        public override bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XPath.MailingPreferences.EmailOption);
                GetElement(XPath.MailingPreferences.PrintOption);

                Reporting.LogPageChange("Change member mailing preferences PCM page");
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }

        /// <summary>
        /// Select the Email preferred delivery method and confirm the copy of the notification 
        /// panel displayed.
        /// If the member does not have a Private Email address on record, invokes HandleNoEmail 
        /// otherwise submits the email option.
        /// </summary>
        /// <param name="contactData"></param>
        public void UpdateMailingPreference(ChangeContactDetails contactData)
        {
            using (var spinner = new RACSpinner(_browser))
            {
                spinner.WaitForSpinnerToFinish();
            }
            ClickControl(XPath.MailingPreferences.PrintOption);
            Reporting.Log($"Capture screenshot after selecting Print for Preferred Delivery Method.", _browser.Driver.TakeSnapshot());

            ClickControl(XPath.MailingPreferences.EmailOption);
            Reporting.Log("Capture screenshot after selecting Email for Preferred Delivery Method.", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual(Constants.MailingPreferences.AdviseUser.EmailInfoBox, 
                GetInnerText(XPath.MailingPreferences.AdviseUser.EmailInfoBox), "expected Email information box copy matches display");

            HandleSubmitPage(contactData);
        }

        /// <summary>
        /// If the member has no Private Email address on record we will confirm the content of the validation error 
        /// message displayed then select Print resubmit the page.
        /// </summary>
        public void HandleSubmitPage(ChangeContactDetails contactData)
        {
            if (string.IsNullOrEmpty(contactData.Contact.PrivateEmail.Address))
            {

                Reporting.Log($"Capturing snapshot before submitting page expecting to trigger validation error.");
                ClickControl(XPath.MailingPreferences.Submit);
                using (var spinner = new RACSpinner(_browser))
                {
                    spinner.WaitForSpinnerToFinish();
                }

                Reporting.Log($"Capturing snapshot of validation error when we attempt to submit email and no email exists.", _browser.Driver.TakeSnapshot());
                Reporting.AreEqual(Constants.MailingPreferences.AdviseUser.EmailMissing, GetInnerText(XPath.MailingPreferences.AdviseUser.EmailMissing),
                    "expected copy matches displayed copy when no email address is on record");
                ClickControl(XPath.MailingPreferences.PrintOption);
                Reporting.Log($"Capturing snapshot before submitting page with Print option.");
                ClickControl(XPath.MailingPreferences.Submit);

                using (var spinner = new RACSpinner(_browser))
                {
                    spinner.WaitForSpinnerToFinish();
                }
            }
            else
            {
                Reporting.Log($"Capturing snapshot before submitting page.");
                ClickControl(XPath.MailingPreferences.Submit);
                
                using (var spinner = new RACSpinner(_browser))
                {
                    spinner.WaitForSpinnerToFinish();
                }
                Reporting.IsFalse(IsControlDisplayed(XPath.MailingPreferences.AdviseUser.EmailMissing),
                    "'Missing email' verification error should not be displayed when Private Email exists");
            }
        }

        /// <summary>
        /// Verifies that the confirmation message matches the expected copy.
        /// </summary>
        public void VerifyConfirmationMessage()
        {
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Confirmation.MessageContent), WaitTimes.T60SEC);
            Reporting.Log($"Capturing screenshot of confirmation.", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual(Constants.Confirmation.ConfirmationText, GetInnerText(XPath.Confirmation.MessageContent), 
                $"expected confirmation text against value displayed");
        }
    }
}
