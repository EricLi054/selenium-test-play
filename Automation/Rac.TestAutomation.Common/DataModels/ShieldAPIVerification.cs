using System;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Rac.TestAutomation.Common
{
    public static class ShieldAPIVerification
    {
        public const string QUOTE_MCO_SHIELDSURNAME     = "_Anon_Motorcycle_Prospect";
        public const string QUOTE_CARAVAN_SHIELDSURNAME = "_Anon_Caravan_Prospect";

        public static Contact BuildExpectedContact(Vehicle vehicle, SparkBasePage.QuoteStage quoteStage, Contact contact)
        {
            var expectedContact = new Contact();
            expectedContact.DateOfBirth = contact.DateOfBirth;
            expectedContact.MailingAddress = new Address();

            if ((contact.SkipDeclaringMembership) && (quoteStage == SparkBasePage.QuoteStage.AFTER_QUOTE))
            {   
                //Member who skipped declaring membership on Page 1, gets a quote but before entering 'Personal Information' on 'Tell us more about you' page.
                expectedContact.FirstName = QUOTE_B2C_IN_SHIELD;
                switch (vehicle)
                {
                    case Vehicle.Caravan:
                        expectedContact.Surname = QUOTE_CARAVAN_SHIELDSURNAME;
                        break;
                    case Vehicle.Motorcycle:
                        expectedContact.Surname = QUOTE_MCO_SHIELDSURNAME;
                        break;
                    default:
                        break;
                }
                expectedContact.Gender = Gender.Female;
                expectedContact.Title = Title.None;
                expectedContact.MailingAddress.Country = contact.MailingAddress.Country;
                expectedContact.MailingAddress.State = contact.MailingAddress.State;
                expectedContact.MailingAddress.Suburb = contact.MailingAddress.Suburb;
                expectedContact.PrivateEmail = null;
                expectedContact.MobilePhoneNumber = null;
            }
            else if (!contact.SkipDeclaringMembership || (contact.SkipDeclaringMembership && (quoteStage == SparkBasePage.QuoteStage.AFTER_PERSONAL_INFO)))
            {
                //Member who did not skip declaring membership on Page 1, OR
                //a member who skipped declaring membership on Page 1, but gets matched entering 'Personal Information' on 'Tell us more about you' page.
                expectedContact.FirstName = $"{QUOTE_B2C_IN_SHIELD}_{contact.FirstName.ToTitleCase()}";
                // Middle name in Shield should equal whatever this quote actually persists. For
                // single-match upfront, skip-declaring-membership, retrieve-quote and no-match
                // flows that is the fixture's middle name (either user input or existing member
                // profile). For multi-match upfront, Shield does not merge into the candidate
                // member record - it creates a new prospect with no middle name - so the
                // expected value must be empty regardless of what the multi-match candidate
                // record carries on the fixture.
                if (!contact.IsMultiMatchRSAMember && !string.IsNullOrEmpty(contact.MiddleName))
                {
                    expectedContact.MiddleName = contact.MiddleName;
                }
                expectedContact.Surname = $"{QUOTE_B2C_IN_SHIELD}_{contact.Surname.ToTitleCase()}";
                expectedContact.Gender = contact.Gender;
                expectedContact.Title = contact.Title;
                expectedContact.MailingAddress = contact.MailingAddress;
                expectedContact.PrivateEmail = contact.PrivateEmail;
                expectedContact.MobilePhoneNumber = contact.MobilePhoneNumber;
            }
            if (quoteStage == SparkBasePage.QuoteStage.POLICY_ISSUED)
            { expectedContact = contact; }

            return expectedContact;

        }
    }
}
