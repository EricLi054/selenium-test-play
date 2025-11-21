using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Tests.ActionsAndValidations
{
    public static class VerifyEndorsePolicy
    {
        public static void VerifyEndorsementEventForAnnualCashPayment(List<string> recentEvents, PremiumChange endorsementChange = PremiumChange.NotApplicable, DateTime? renewalDate = null)
        {
            // Only applicable for Home renewal which is due/overdue (Pay now) scenario.
            // Note that this won't work correctly if renewals and billing aren't being run in Shield.
            // If test fails here, check that the NPE batch jobs are scheduled/running.
            // See https://rac-wa.atlassian.net/wiki/spaces/SS/pages/2198471435/NPE+Batch+Jobs
            if (renewalDate == DateTime.Now.Date)
                Reporting.IsTrue(recentEvents.Any(e => e.Equals("Instalment status is Paid")), "for 'Instalment status is Paid' event in Recent Events " +
                    "(NOTE: Will only exist for appropriately billed renewal policies; Instalment Status before Renewal logged at start of test should " +
                    "have been 'Submitted'. Check Collections Export - Initiator job.).".IsFound());
            else
                IsPolicyEndorsedSuccessfully(recentEvents);

            if (endorsementChange == PremiumChange.PremiumIncrease
                || endorsementChange == PremiumChange.NotApplicable)
                Reporting.IsTrue(recentEvents.Any(e => e.Equals("Endorsement receipt print")), "Endorsement receipt print in Recent Events".IsFound());
            else
                Reporting.IsFalse(recentEvents.Any(e => e.Equals("Endorsement receipt print")), "Endorsement receipt print should not be in Recent Events".IsNotFound());
        }

        public static void VerifyEndorsementEventForNonAnnualCashPayment(List<string> recentEvents)
        {
            IsPolicyEndorsedSuccessfully(recentEvents);
        }

        private static void IsPolicyEndorsedSuccessfully(List<string> recentEvents)
        {
            Reporting.IsTrue(recentEvents.Any(e => e.Equals("General change")), "General change in Recent Events".IsFound());
            Reporting.IsTrue(recentEvents.Any(e => e.Equals("Policy Endorsement Certificate Print")), "Policy Endorsement Certificate Print in Recent Events".IsFound());
        }

        public static void VerifyUpdateOccursInShieldWithCreditCardAsSource(string policyNumber, CreditCard creditCard)
        {
            var policyInstalmentDetails = ShieldPolicyDB.FetchNextInstalmentDetails(policyNumber);

            Reporting.AreEqual(creditCard.CardIssuer,
                               policyInstalmentDetails.CreditCard.CardIssuer,
                               "card issuer is correct");

            Reporting.AreEqual(creditCard.CardNumber.Substring(creditCard.CardNumber.Length - 4),
                               policyInstalmentDetails.CreditCard.CardNumber.Substring(policyInstalmentDetails.CreditCard.CardNumber.Length - 4),
                               "card token is correct");

            Reporting.AreEqual(creditCard.CardholderName,
                               policyInstalmentDetails.CreditCard.CardholderName,
                               "card holder name is correct");

            Reporting.AreEqual(creditCard.CardExpiryDate.ToString(DataFormats.DATE_MONTH_YEAR_FORWARDSLASH, CultureInfo.InvariantCulture),
                               policyInstalmentDetails.CreditCard.CardExpiryDate.ToString(DataFormats.DATE_MONTH_YEAR_FORWARDSLASH, CultureInfo.InvariantCulture),
                               "credit card expiry date is correct");
        }

        public static void VerifyUpdateOccursInShieldWithBankAccountAsSource(string policyNumber, BankAccount bankAccount)
        {
            var policyInstalmentDetails = ShieldPolicyDB.FetchNextInstalmentDetails(policyNumber);

            Reporting.AreEqual(bankAccount.Bsb,
                               policyInstalmentDetails.BankAccount.Bsb,
                               "BSB updated");

            Reporting.AreEqual(bankAccount.AccountNumber,
                               policyInstalmentDetails.BankAccount.AccountNumber,
                               "account number updated");

            Reporting.AreEqual(bankAccount.AccountName,
                               policyInstalmentDetails.BankAccount.AccountName,
                               "account name updated");
        }
    }
}
