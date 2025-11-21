using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using UIDriver.Pages.B2C;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Tests.ActionsAndValidations
{
    public static class VerifyQuotePet
    {
        public static void VerifyQuoteSummaryPage(Browser browser, QuotePet petQuote)
        {
            using (var quotePage3  = new PetQuote3Summary(browser))
            using (var paymentPage = new QuotePayments(browser))
            using (var spinner = new RACSpinner(browser))
            {
                Reporting.Log("Begin verify policy vehicle details");
                Reporting.AreEqual(quotePage3.PetBreed, petQuote.Breed, true);
                Reporting.AreEqual(quotePage3.PetName, petQuote.Name);

                Reporting.AreEqual(petQuote.GetPetAge(), quotePage3.PetAge, $"summary of pet age ({quotePage3.PetAge}) to match or contain input age of {petQuote.GetPetAge()} for pet DoB: {petQuote.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_HYPHENS)}");

                quotePage3.VerifyPolicyholderDetails(petQuote.PolicyHolder);
            }
        }

        public static string VerifyPetConfirmationPage(Browser browser, QuotePet petQuote, decimal expectedPrice, out string receiptNumber)
        {
            var policyNumber = string.Empty;
            receiptNumber    = string.Empty;

            using (var confirmation = new PetQuoteConfirmation(browser))
            {
                Reporting.Log("Verifying details on confirmation screen.");

                policyNumber = confirmation.PolicyNumber;
                Reporting.Log($"Policy number is: {policyNumber}", browser.Driver.TakeSnapshot());

                Reporting.AreEqual(petQuote.StartDate.ToString("d MMMM yyyy"), confirmation.PolicyStartDate, "Policy start date on confirmation page");
                Reporting.AreEqual(petQuote.StartDate.AddYears(1).ToString("d MMMM yyyy"), confirmation.PolicyEndDate, "Policy end date on confirmation page");

                Reporting.Log("Policy Purchase Confirmation page, establishing sanitizedName from customer.SanitizeNameValues().");

                Reporting.IsTrue(petQuote.PolicyHolder.EqualsFullName(confirmation.PolicyHolder), $"Verifying {petQuote.PolicyHolder.GetFullTitleAndName()} matches policy holder name on confirmation screen.");

                Reporting.AreEqual($"{petQuote.Name} {petQuote.Breed}", confirmation.PetDetail, true, "Pet detail on confirmation page");

                Reporting.IsTrue(confirmation.IsPaymentFrequencyLabelDisplayed, "Payment frequency label".IsDisplayed());
                Reporting.AreEqual("Payment frequency:", confirmation.PaymentFrequencyLabelText, "Payment frequency:".IsExpectedLabelText());
                Reporting.AreEqual(petQuote.PayMethod.PaymentFrequency, confirmation.PaymentFrequency, "Payment Frequency on confirmation page");

                Reporting.IsTrue(confirmation.IsAmountLabelDisplayed, "Amount label".IsDisplayed());
                Reporting.AreEqual("Amount:", confirmation.AmountLabelText, "Amount:".IsExpectedLabelText());
                Reporting.AreEqual(expectedPrice, confirmation.AmountPaid, "recorded amount as expected");

                if (petQuote.PayMethod.Scenario == PaymentScenario.AnnualCash)
                    receiptNumber = confirmation.VerifyReceiptNumberIsDisplayedCorrectly();
                else
                {
                    Reporting.IsFalse(confirmation.IsReceiptNumberLabelDisplayed, "Receipt number label".IsNotDisplayed());
                    Reporting.IsFalse(confirmation.IsReceiptNumberDisplayed, "Receipt number".IsNotDisplayed());
                }
            }

            return policyNumber;
        }

        public static void VerifyPetPolicyInShield(QuotePet petQuote, string policyNumber)
        {
            VerifyPetPolicyInShieldBasicCoverDetails(petQuote: petQuote, policyNumber: policyNumber);
            VerifyPetPolicyInShieldUIPaymentDetails(quoteDetails: petQuote, policyNumber: policyNumber);
        }

        public static void VerifyPetPolicyInShieldBasicCoverDetails(QuotePet petQuote, string policyNumber)
        {
            // Verify General Policy details
            Reporting.Log($"Begin verify policy details from Shield DB. Param/s = {policyNumber}");
            var policyInfo = ShieldPetDB.FetchPetPolicyDetail(policyNumber);

            Reporting.AreEqual(petQuote.PolicyHolder.GetContactAge(), policyInfo.PolicyholderAge, "policy holder age");
            Reporting.AreEqual(petQuote.PolicyHolder.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
                               policyInfo.PolicyholderDOB.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), "Policy holder age");
            Reporting.IsTrue(petQuote.Type == policyInfo.PetType, $"Pet type {petQuote.Type} matches {policyInfo.PetType}");
            Reporting.AreEqual(petQuote.Breed, policyInfo.PetBreed, "Pet breed");
            Reporting.AreEqual(petQuote.Name, policyInfo.PetName, "Pet name");
            Reporting.AreEqual(petQuote.StartDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
                               policyInfo.PolicyStartDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), "Policy start date");
            Reporting.AreEqual(petQuote.PayMethod.IsPaymentByBankAccount, policyInfo.IsDirectDebit, "payment method by bank account");
            Reporting.AreEqual(petQuote.PayMethod.PaymentFrequency, policyInfo.PaymentFrequency, "Policy Payment Frequency from Shield DB");
            Reporting.AreEqual(petQuote.AddTlc, policyInfo.HasTLCCover, "state of optional TLC cover");
            var expectedInstallments = petQuote.PayMethod.NumberOfPayments;
            Reporting.AreEqual(expectedInstallments, policyInfo.InstallmentCount, "count of payment installments");
            Reporting.IsTrue(policyInfo.HasPreExistIllness == false, $"The pre-existing illness condition is {policyInfo.HasPreExistIllness} as default.");
        }

        /// <summary>
        /// Verifies new business pet policy's payment details against Shield DB
        /// </summary>
        /// <param name="testConfig"></param>
        /// <param name="quoteDetails"></param>
        /// <param name="policyNumber"></param>
        public static void VerifyPetPolicyInShieldUIPaymentDetails(QuotePet quoteDetails, string policyNumber)
        {
            Reporting.Log($"Begin verify payment details from Shield DB. Param/s = {policyNumber}");
            var dbPaymentDetails      = ShieldPolicyDB.FetchPaymentDetailsForPolicy(policyNumber);
            var recentEvents          = ShieldPolicyDB.FetchRecentEventsOnPolicy(policyNumber);

            Reporting.IsTrue(quoteDetails.PayMethod.PaymentFrequency == dbPaymentDetails.PaymentFrequency, "payment frequency is expected value in DB");

            if (quoteDetails.PayMethod.IsPaymentByBankAccount)
                Reporting.AreEqual(DIRECT_DEBIT, dbPaymentDetails.PaymentMethod);
            else
            {
                if (quoteDetails.PayMethod.IsAnnual)
                {
                    Reporting.AreEqual(CASH, dbPaymentDetails.PaymentMethod);
                    VerifyPolicy.VerifyNewBusinessEventForAnnualCashPayment(recentEvents);
                }
                else
                    Reporting.AreEqual(CREDIT_CARD, dbPaymentDetails.PaymentMethod);
            }

            if (quoteDetails.PayMethod.Scenario != PaymentScenario.AnnualCash)
                VerifyPolicy.VerifyNewBusinessEventForDirectDebitInstalmentPayment(recentEvents);

            var expectedPaymentFrequency = quoteDetails.PayMethod.NumberOfPayments;
            Reporting.AreEqual(expectedPaymentFrequency, dbPaymentDetails.PaymentCount, "count of payments in DB is expected value");

            VerifyPolicy.VerifyNewPolicyInstalments(policyNumber);
        }
    }
}
