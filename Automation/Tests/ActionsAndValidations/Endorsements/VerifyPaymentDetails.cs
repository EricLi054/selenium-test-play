using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;

namespace Tests.ActionsAndValidations
{
    public static class VerifyPaymentDetails
    {
        /// <summary>
        /// Verify the payment details recorded in Shield for the given policy.
        /// Current design only supports verifying bank account details.
        /// </summary>
        /// <param name="policyNumber">The policy number to be looked up in Shield </param>
        /// <param name="expectedBankDetails">The details of the bank account that we expect to see recorded</param>
        public static void VerifyPolicyPaymentDetails(string policyNumber, BankAccount expectedBankDetails)
        {
            var updatedBankDetails = ShieldBankDetailsDB.GetDirectDebitDetailsForPolicy(policyNumber);
            Reporting.Log($"Verify updated Bank Details Data for BSB , Account Number and Account Name");

            if (!string.IsNullOrEmpty(expectedBankDetails.Id))
                Reporting.AreEqual(expectedBankDetails.Id, updatedBankDetails.Id);
            
            Reporting.AreEqual(expectedBankDetails.Bsb, updatedBankDetails.Bsb);
            Reporting.AreEqual(expectedBankDetails.AccountNumber, updatedBankDetails.AccountNumber);
            Reporting.AreEqual(expectedBankDetails.AccountName, updatedBankDetails.AccountName);
        }
    }
}
