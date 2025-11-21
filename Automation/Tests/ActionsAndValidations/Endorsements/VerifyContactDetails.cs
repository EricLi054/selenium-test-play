using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Contacts;
using UIDriver.Pages.PCM;

namespace Tests.ActionsAndValidations
{
    public static class VerifyContactDetails
    {
        /// <summary>
        /// Verify the expected Contact Details- Event Type and Username 
        /// with Shield Contact Details.
        /// </summary>
        /// <param name="contactDetails"></param>
        public static void VerifyUpdatedContactDetailsFromShield(ChangeContactDetails contactDetails)
        {            
            var contactEvent = ShieldContactDetailsDB.GetLatestContactShieldEventForContactID(contactDetails.Contact.Id);
            Reporting.LogMinorSectionHeading($"Verify Update contact Event Type & check Username of Update User");
            Reporting.AreEqual("Update contact", contactEvent.EventType, "the expected event name from Shield against the actual value");
            Reporting.AreEqual("B2CTestUser", contactEvent.Username, "the expected User against the actual user on the Shield event");
            
            var shieldUpdatedContactDetails = ShieldContactDetailsDB.GetContactByContactID(contactDetails.Contact.Id);
            Reporting.LogMinorSectionHeading($"Verify updated Preferred Delivery Method from Shield Database");
            Reporting.AreEqual(contactDetails.Contact.Id, shieldUpdatedContactDetails.Id, true, "ContactId matches.");

            if (string.IsNullOrEmpty(contactDetails.Contact.PrivateEmail.Address))
            {
                Reporting.AreEqual("Print", shieldUpdatedContactDetails.PreferredDeliveryMethod, "expected delivery method is 'Print'");
            }
            else
            {
                Reporting.AreEqual("Email", shieldUpdatedContactDetails.PreferredDeliveryMethod, "expected delivery method is 'Email'");
            }
        }
       
    }
}
