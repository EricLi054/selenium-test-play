using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.SparkBasePage;

namespace Tests.ActionsAndValidations
{
    public static class VerifyPolicy
    {
        #region Constants
        private const string EMAIL_SUBJECT = "RAC Insurance Policy Certificate of Currency";
        #endregion
        /// <summary>
        /// Calls the Shield Contact API to retrieve a contact and verify the contact's details
        /// against the test's expected values.
        /// </summary>
        /// <param name="expectedContact">Contact object that the test data is using to input into Insurance</param>
        /// <param name="contactId">The Contact ID that Shield should have recorded this contact against</param>
        /// <param name="includeMailingAddress">TRUE if we also verify mailing address, otherwise skip (such as where we haven't entered PH details yet).</param>
        /// <param name="quoteStage"> The current stage of the Quote
        public static void VerifyPHDetailsWithAPIResponse(Contact expectedContact, string contactId, bool includeMailingAddress = true, QuoteStage quoteStage = QuoteStage.POLICY_ISSUED)
        {
            var contactDetailsFromShieldAPI = DataHelper.GetContactDetailsViaContactId(contactId);

            Reporting.AreEqual(expectedContact.DateOfBirth.Date, contactDetailsFromShieldAPI.DateOfBirth.Date, "Date Of Birth in Shield");
            Reporting.AreEqual(QUOTE_ROLE_IN_SHIELD, contactDetailsFromShieldAPI.Roles[0].ExternalCode, "Role in Shield");

            if (quoteStage == QuoteStage.POLICY_ISSUED)
            {
                Reporting.AreEqual(expectedContact.Title, contactDetailsFromShieldAPI.Title, "Title in Shield");
            }
            
            Reporting.AreEqual(expectedContact.FirstName, contactDetailsFromShieldAPI.FirstName, ignoreCase: true, "First Name in Shield (NOT CASE SENSITIVE)");
            Reporting.AreEqual(expectedContact.Surname, contactDetailsFromShieldAPI.Surname, ignoreCase: true, "Last Name in Shield (NOT CASE SENSITIVE)");
            Reporting.AreEqual(expectedContact.Gender, contactDetailsFromShieldAPI.Gender, "Gender in Shield");

            if (includeMailingAddress && 
                expectedContact.MailingAddress != null && 
                expectedContact.MailingAddress.StreetNumber != null)
            {
                Reporting.IsTrue(expectedContact.MailingAddress.IsEqualIgnorePostcode(contactDetailsFromShieldAPI.MailingAddress), $"Mailing address of policy holder ({expectedContact.MailingAddress.QASStreetAddress()}) should equal {contactDetailsFromShieldAPI.MailingAddress.QASStreetAddress()}");
                Reporting.AreEqual(expectedContact.MailingAddress.State, contactDetailsFromShieldAPI.MailingAddress.State, "Mailing Address:State in Shield");
                Reporting.AreEqual(expectedContact.MailingAddress.PostCode, contactDetailsFromShieldAPI.MailingAddress.PostCode, "Mailing Address:HouseNumber in Shield");
            }else
            {
                Reporting.AreEqual(expectedContact.MailingAddress.Country, contactDetailsFromShieldAPI.MailingAddress.Country, true, "Mailing Address:Country in Shield");
                Reporting.AreEqual(expectedContact.MailingAddress.State, contactDetailsFromShieldAPI.MailingAddress.State, true, "Mailing Address:State in Shield");
            }
            
            if (!string.IsNullOrEmpty(expectedContact.MobilePhoneNumber))
            {
                Reporting.AreEqual(expectedContact.MobilePhoneNumber, contactDetailsFromShieldAPI.MobilePhoneNumber, "Mobile in Shield");
            }
            if (expectedContact.PrivateEmail != null)
            {
                Reporting.AreEqual(expectedContact.PrivateEmail.Address, contactDetailsFromShieldAPI.PrivateEmail.Address, ignoreCase: true, "Private Email address in Shield");
            }
        }

        public static void VerifyNewBusinessEventForAnnualCashPayment(List<string> recentEvents)
        {
            Reporting.IsTrue(recentEvents.Any(e => e.Contains("Paid new business certificate print")), "Paid Certificate in Recent Events".IsFound());
        }

        public static void VerifyNewBusinessEventForDirectDebitInstalmentPayment(List<string> recentEvents)
        {
            Reporting.IsTrue(recentEvents.Any(e => e.Contains("Recurring instalment new business certificate print")), "Recurring instalment new business certificate print in Recent Events".IsFound());
            Reporting.IsTrue(recentEvents.Any(e => e.Equals("Recurring instalment New Business")), "Recurring instalment New Business in Recent Events".IsFound());
        }

        public static void VerifyEmailedCertificateOfCurrency(string policyNumber, string emailAddress)
        {
            var emailHandler = new MailosaurEmailHandler();
            var email = Task.Run(() => emailHandler.FindEmailByRecipient(emailAddress)).GetAwaiter().GetResult();
            Reporting.Log($"Email received: {email.Received.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss")}");

            Reporting.AreEqual(EMAIL_SUBJECT, email.Subject);
            Reporting.IsTrue(email.Attachments != null && email.Attachments.Count == 1, "that there is only one attachment to the Certificate of currency email.");
            Reporting.AreEqual($"CertificateOfCurrency_{policyNumber}.pdf", email.Attachments[0].FileName, ignoreCase: true);
            Reporting.IsTrue(email.Attachments[0].Length > 0, $"the size of the PDF is greater than 0 ({email.Attachments[0].Length} bytes).");
        }

        /// <summary>
        /// Verifying expected status for instalments on a new policy
        /// </summary>
        /// <param name="policyNumber"></param>
        public static void VerifyNewPolicyInstalments(string policyNumber)
        {
            var instalments = ShieldPolicyDB.FetchPaymentStatusOnPolicy(policyNumber);

            var allStatusCorrect = true;

            foreach (var instalment in instalments)
            {
                // A recurring payment scheduled for today would be in a paid state
                // due to billings jobs. So we will only check:
                //  - future payments, or
                //  - non-recurring payments from mid-term endorsements.
                if ((instalment.CollectionDate > DateTime.Now.Date ||
                     !instalment.IsRecurring) &&
                    instalment.Status != Status.Pending)
                {
                    Reporting.Log($"Encountered unexpected instalment {instalment.Status.GetDescription()} of ${instalment.Amount} due {instalment.CollectionDate.ToString(DataFormats.DATE_ABBREVIATED_MONTH_FORWARD_WHITESPACE)}");
                    allStatusCorrect = false;
                    break;
                }
            }

            Reporting.IsTrue(allStatusCorrect, "Payment Status is/are as expected");
        }

        /// <summary>
        /// Verify the status of remaining instalments for the policy after an
        /// endorsement is performed. Shared amongst both renewals as well
        /// as endorsements that update policy details.
        /// </summary>
        /// <param name="testData">base endorsement details for policy number and payment details</param>
        /// <param name="renewalDate">Only valid for renewal cases, to determine if it is a Pay Now scenario</param>
        public static void VerifyPolicyEndorsementInstalments(EndorsementBase testData, DateTime? renewalDate)
        {
            var instalments = ShieldPolicyDB.FetchPaymentStatusOnPolicy(testData.PolicyNumber);

            if (instalments.Count == 0)
            {
                // We do nothing. This situation occurs for endorsements on annual paid policies
                // so the last instalment has passed some time ago, and the endorsement did not
                // trigger any new instalments.
                return;
            }

            var lastinstalment = instalments.Last();

            if (testData.IsExpectedToReceiveRefund())
            {
                // The payment status will be 'Pending' for Future dates endorsement and ''Booked' for immediate/today
                var expectedBillingState = testData.StartDate.Date > DateTime.Now.Date ? Status.Pending : Status.Booked;
                Reporting.AreEqual(expectedBillingState.GetDescription(), lastinstalment.Status.GetDescription(), "expected Payment Status against actual status in Shield");
            }
            else if (testData.IsAnnualCreditCardPaymentMethod() &&
                     (renewalDate != null && renewalDate.Value == DateTime.Now.Date))
            {
                Reporting.AreEqual(Status.Paid.GetDescription(), lastinstalment.Status.GetDescription(), "Payment Status is as expected");
            }
            else if (testData.IsExpectedToMakeOneOffPayment())
            {
                Reporting.AreEqual(Status.Pending.GetDescription(), lastinstalment.Status.GetDescription(), "Payment Status is as expected");
            }
            else
            {
                var instalmentStatusAreCorrect = true;

                foreach (var instalment in instalments)
                {
                    if (instalment.CollectionDate < DateTime.Now.Date && instalment.Status != Status.Paid)
                    {
                        Reporting.Log($"Encountered instalment that should be in PAID state, but was {instalment.Status.GetDescription()} of ${instalment.Amount} due {instalment.CollectionDate.ToString(DataFormats.DATE_ABBREVIATED_MONTH_FORWARD_WHITESPACE)}");
                        instalmentStatusAreCorrect = false;
                    }

                    if (instalment.CollectionDate >= DateTime.Now.Date &&  instalment.Status != Status.Pending)
                    {
                        Reporting.Log($"Encountered instalment that should be in PENDING state, but was {instalment.Status.GetDescription()} of ${instalment.Amount} due {instalment.CollectionDate.ToString(DataFormats.DATE_ABBREVIATED_MONTH_FORWARD_WHITESPACE)}");
                        instalmentStatusAreCorrect = false;
                    }
                }

                Reporting.IsTrue(instalmentStatusAreCorrect, "Payment statuses are as expected (see preceeding messages if failed)");
            }
        }

        public static void VerifyPolicyMultiMatchDetailsInShield(List<Contact> policyHolders, string policyNumber)
        {
            Reporting.LogMinorSectionHeading($"Verify that any existing member details used, they are matched and linked correctly on the policy");
            var policyDetails = DataHelper.GetPolicyDetails(policyNumber);

            if (policyHolders == null || policyHolders.Count < 1)
            { Reporting.Error($"The policy holders are null or the count is equal to zero"); }

            var mainPolicyHolder = policyHolders[0];

            if (mainPolicyHolder.IsMultiMatchRSAMember)
            {
                Reporting.AreNotEqual(mainPolicyHolder.Id, policyDetails.Policyholder.Id.ToString(), true,
                    $"member used for main policyholder with existing Contact ID '{mainPolicyHolder.Id}' " +
                    $"had a new Contact ID created for this policy, so the Policyholder Contact ID does not " +
                    $"match to that existing Contact ID.");
            }

            if (!string.IsNullOrEmpty(mainPolicyHolder.Id) && !mainPolicyHolder.IsMultiMatchRSAMember)
            {
                Reporting.AreEqual(mainPolicyHolder.Id, policyDetails.Policyholder.Id.ToString(),
                    $"member used for main policyholder with existing Contact ID '{mainPolicyHolder.Id}' " +
                    $"was correctly matched and this policy was linked to that existing Contact ID");
            }

            // Work through coPolicyholders
            // The "Skip(1)" gets over the main policyholder.
            var copolicyholders = policyHolders.Skip(1).Where(x => !string.IsNullOrEmpty(x.Id)).ToList();
            foreach (var coph in copolicyholders)
            {
                if (coph.IsMultiMatchRSAMember)
                {
                    Reporting.IsFalse(policyDetails.PolicyCoOwners.Any(x => x.Id.ToString() == coph.Id),
                                      $"existing multimatch coPH({coph.Id}) had new contact record created");
                }
                else
                {
                    Reporting.IsTrue(policyDetails.PolicyCoOwners.Any(x => x.Id.ToString() == coph.Id),
                                   $"existing coPH({coph.Id}) was matched to expected Shield Contact ID");
                }
            }
        }

        public static void VerifyPolicyMultiMatchDetailsInShield(List<Driver> drivers, string policyNumber)
        {
            var policyHolderContacts = drivers.Where(x => x.IsPolicyHolderDriver).Select(x => x.Details).ToList();
            VerifyPolicyMultiMatchDetailsInShield(policyHolderContacts, policyNumber);
        }
    }
}
