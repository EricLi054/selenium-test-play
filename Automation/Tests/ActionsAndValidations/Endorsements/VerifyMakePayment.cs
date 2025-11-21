using Rac.TestAutomation.Common;
using System.Linq;
using UIDriver.Pages.Spark.Endorsements.MakePayment;

namespace Tests.ActionsAndValidations.Endorsements
{
    public static class VerifyMakePayment
    {
        /// <summary>
        /// Verifying whether the Policy is eligible for Make a payment on PCM 
        /// </summary>
        /// <param name="contactId">Contact id to login in PCM</param>
        /// <param name="policyNumber">Policy number to look for in PCM Home</param>
        /// <param name="isFailedPayment">Set to true for failed payment scenario testing</param>
        public static void VerifyPolicyEligibilityForMakePaymentInPCM(Browser browser, string contactId, string policyNumber, bool isFailedPayment)
        {
            Reporting.Log("Opening PCM to verify the details of the policy we have paid in this test");
            browser.LoginMemberToPCMAndDisplayPolicy(contactId, policyNumber);
            ActionsPCM.CheckMakePaymentEligibility(browser,isFailedPayment);
        }

        /// <summary>
        /// Verifying the Policy details on Shield 
        /// </summary>
        /// <param name="isFailedPayment">Set to true for failed payment scenario testing</param>
        public static void VerifyShieldAfterMakePayment(EndorsementBase testData,bool isFailedPayment)
        {
            var updatedPolicyData = DataHelper.GetPolicyDetails(testData.PolicyNumber);
            var amountDueBeforePayment = testData.OriginalPolicyData.NextPayableInstallment is null ? testData.OriginalPolicyData.AnnualPremium.Total.ToString() : testData.OriginalPolicyData.NextPayableInstallment.OutstandingAmount.ToString();
            var amountDueAfterPayment = updatedPolicyData.NextPayableInstallment is null ? updatedPolicyData.AnnualPremium.Total.ToString() : updatedPolicyData.NextPayableInstallment.OutstandingAmount.ToString();

            //Checking the installment Status is turned to Paid in Shield for Submitted status payment
            if (testData.OriginalPolicyData.Installments.First().Status.Equals("Submitted"))
            {
                Reporting.AreEqual("Paid", updatedPolicyData.Installments.First().Status, " the Shield payment status is turned to 'Paid' after 'Make A Payment'");
            }

            if (isFailedPayment)
            {
                Reporting.IsFalse(updatedPolicyData.IsPaidInFull, " the Shield payment amount is NOT equals to '0.00' after 'Make A Payment'");
                Reporting.AreEqual(amountDueBeforePayment, amountDueAfterPayment, " the Amount Due before and after payment failure. ");
            }
            else
            {
                Reporting.IsTrue(updatedPolicyData.IsPaidInFull, " the Shield payment amount is equals to '0.00' after 'Make A Payment'");
            }

            var contactDetails = DataHelper.GetContactDetailsViaExternalContactNumber(testData.OriginalPolicyData.Policyholder.ContactExternalNumber);
            Reporting.AreEqual(testData.ActivePolicyHolder.PrivateEmail.Address, contactDetails.GetEmail(), true, $" the current email address is matching with Shield.");
        }
    }
}
