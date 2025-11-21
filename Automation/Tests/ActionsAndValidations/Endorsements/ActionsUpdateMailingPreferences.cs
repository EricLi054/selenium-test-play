using Rac.TestAutomation.Common;
using System.Text.RegularExpressions;
using UIDriver.Pages.PCM.ContactDetails;

namespace Tests.ActionsAndValidations
{
    public static class ActionsUpdateMailingPreferences
    {
        /// <summary>
        /// Login to PCM Application and view policy carousel.
        /// Select Mail Preferences tab and update the preferred delivery method 
        /// for policy documents.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="contactDetails"></param>
        /// <returns></returns>
        public static void PerformUpdateToMailingPreferences(Browser browser, ChangeContactDetails contactDetails)
        {
            browser.LoginMemberToPCM(contactDetails.Contact.Id);
            
            UpdateExistingPolicyWithNewMailingPreferences(browser, contactDetails);
        }

        private static void UpdateExistingPolicyWithNewMailingPreferences(Browser browser, ChangeContactDetails contactDetails)
        {
            ActionsPCM.ViewMailPreferences(browser);
            
            using (var changeMyContactDetails = new UpdateMailingPreferences(browser))
            {
                Reporting.Log($"Capturing page state on arrival", browser.Driver.TakeSnapshot());
                changeMyContactDetails.UpdateMailingPreference(contactDetails);
            }
        }
        public static void VerificationOfConfirmationMessage(Browser browser)
        {
            using (var changeMyContactDetails = new UpdateMailingPreferences(browser))
            { 
                changeMyContactDetails.VerifyConfirmationMessage();
            }
        }
       
    }
       
}
