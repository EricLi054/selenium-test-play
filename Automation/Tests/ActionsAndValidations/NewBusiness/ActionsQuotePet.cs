using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using UIDriver.Helpers;
using UIDriver.Pages;
using UIDriver.Pages.B2C;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Tests.ActionsAndValidations
{
    public static class ActionsQuotePet
    {
        public static string PurchasePetPolicy(Browser browser, QuotePet petQuote, bool detailUIChecking=false)
        {
            decimal agreedFirstPayment = 0;
            string policyNumber        = null;
            string receiptNumber       = string.Empty;

            OpenBaseB2CLandingPage(browser: browser);

            SubmitInitialPetQuoteRatingValues(browser: browser, petQuote: petQuote);

            UpdateAndSubmitInitialQuotePage(browser: browser, petQuote: petQuote, agreedQuotePrice: out agreedFirstPayment);

            UpdateAndSubmitAdditionalQuoteDetailsPage(browser: browser, petQuote: petQuote);

            VerifyQuotePet.VerifyQuoteSummaryPage(browser: browser,
                                                   petQuote: petQuote);
            AcceptQuoteSummary(browser);

            agreedFirstPayment = SubmitPayment(browser: browser, petQuote: petQuote, expectedPrice: agreedFirstPayment, detailUIChecking);

            policyNumber = VerifyQuotePet.VerifyPetConfirmationPage(browser: browser,
                                                                     petQuote: petQuote,
                                                                     expectedPrice: agreedFirstPayment, receiptNumber: out receiptNumber);

            if (petQuote.PayMethod.Scenario == PaymentScenario.AnnualCash)
                Payment.VerifyWestpacQuickStreamDetailsAreCorrect<QuotePet>(cardDetails: petQuote.PayMethod.CreditCardDetails,
                                                            policyNumber: policyNumber,
                                                            expectedPrice: agreedFirstPayment,
                                                            expectedReceiptNumber: receiptNumber);

            return policyNumber;
        }

        public static void OpenBaseB2CLandingPage(Browser browser)
        {
            LaunchPage.OpenB2CLandingPageAndFeatureToggles(browser);
            browser.LaunchPageBeginNewPetQuote();
        }

        /// <summary>
        /// Deals with "page 1" of the pet policy purchase flow. 
        /// Enters all the initial values for Shield to provide a 
        /// quote price for the user.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="petQuote"></param>
        /// <returns></returns>
        public static void SubmitInitialPetQuoteRatingValues(Browser browser, QuotePet petQuote)
        {
            using (var quotePage1 = new PetQuote1Details(browser))
            using (var quotePage2 = new PetQuote2Quote(browser))
            using (var spinner = new RACSpinner(browser))
            {
                quotePage1.FillQuoteDetails(petQuote);
                quotePage1.ClickContinueFromPolicyHolderDetails();
                spinner.WaitForSpinnerToFinish(nextPage: quotePage2);
            }
        }

        /// <summary>
        /// Deals with "page 2" of the pet policy purchase flow.
        /// Makes changes based on test data in "petQuote".
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="petQuote">test data</param>
        /// <param name="agreedQuotePrice">This choice of either annual/monthly price is derived from values in "petQuote"</param>
        /// <returns>Quote reference code</returns>
        public static string UpdateAndSubmitInitialQuotePage(Browser browser, QuotePet petQuote, out decimal agreedQuotePrice)
        {
            string quoteNumber = string.Empty;

            agreedQuotePrice = 0;

            using (var quotePage2 = new PetQuote2Quote(browser))
            using (var quotePage3 = new PetQuote3Policy(browser))
            using (var spinner = new RACSpinner(browser))
            {
                if (!string.IsNullOrEmpty(petQuote.Excess))
                {
                    quotePage2.QuoteExcess = petQuote.Excess;
                    spinner.WaitForSpinnerToFinish();
                }

                if (petQuote.StartDate.Date != DateTime.Now.Date)
                {
                    quotePage2.QuoteStartDate = petQuote.StartDate;
                    spinner.WaitForSpinnerToFinish();
                }

                if (petQuote.AddTlc)
                {
                    quotePage2.ClickTLCCoverCheckbox(petQuote.AddTlc);
                    spinner.WaitForSpinnerToFinish();
                }

                // Buffer details:
                quoteNumber      = quotePage2.QuoteReference;
                agreedQuotePrice = petQuote.PayMethod.IsAnnual ? quotePage2.QuotePriceAnnual : quotePage2.QuotePriceMonthly;
                petQuote.Excess  = string.IsNullOrEmpty(petQuote.Excess) ?
                                        quotePage2.QuoteExcess.StripMoneyNotations() :
                                        petQuote.Excess;
                var quoteDate = quotePage2.QuoteStartDate.ToString(DataFormats.DATE_ABBREVIATED_MONTH_FORWARD_FORWARDSLASH);

                Reporting.Log($"Quote ({quoteNumber}) successfully generated. Quoted price: {agreedQuotePrice} Set Excess: {petQuote.Excess} Start Date: {quoteDate}");

                quotePage2.ClickBuyOnlineButton();
                spinner.WaitForSpinnerToFinish(nextPage: quotePage3);
            }

            return quoteNumber;
        }

        public static void UpdateAndSubmitAdditionalQuoteDetailsPage(Browser browser, QuotePet petQuote)
        {
            using (var quotePage3   = new PetQuote3Policy(browser))
            using (var quoteSummary = new PetQuote3Summary(browser))
            using (var spinner      = new RACSpinner(browser))
            {
                // Complete the additional car details
                quotePage3.FillInPetDetails(petQuote);
                quotePage3.ClickPetDetailsContinueButton();

                // Allow time for accordion transition
                System.Threading.Thread.Sleep(2000);

                quotePage3.FillInPolicyholderDetails(petQuote.PolicyHolder);
                quotePage3.ClickPolicyholderDetailsContinueButton();

                // Allow time for accordion transition
                System.Threading.Thread.Sleep(2000);

                quotePage3.FillInDisclosureAndSubmit();

                spinner.WaitForSpinnerToFinish(nextPage: quoteSummary);
            }
        }

        /// <summary>
        /// Deals with "page 4" of the pet policy purchase flow.
        /// </summary>
        /// <param name="browser"></param>
        public static void AcceptQuoteSummary(Browser browser)
        {
            using (var quotePage3  = new PetQuote3Summary(browser))
            using (var paymentPage = new QuotePayments(browser))
            using (var spinner     = new RACSpinner(browser))
            {
                quotePage3.ClickContinue();
                spinner.WaitForSpinnerToFinish(nextPage: paymentPage);
            }
        }

        /// <summary>
        /// Deals with the payment page of the general motor vehicle
        /// policy purchase flow.
        /// </summary>
        /// <param name="browser"></param>
        public static decimal SubmitPayment(Browser browser, QuotePet petQuote, decimal expectedPrice, bool detailUIChecking = false)
        {
            decimal nextInstallment = 0;

            using (var paymentPage   = new QuotePayments(browser))
            using (var confirmation  = new PetQuoteConfirmation(browser))
            using (var spinner       = new RACSpinner(browser))
            using (var westpacIframe = new WestpacQuickStream(browser))
            {
                Reporting.AreEqual(expectedPrice, paymentPage.GetNextInstallmentAmountForChosenFrequency(petQuote.PayMethod.PaymentFrequency),
                    "Expected amount is displayed correctly");

                Reporting.IsTrue(paymentPage.VerifyUIElementsInPaymentDetailsPageAreCorrect(), "UI elements in payment details page are as expected");

                if (detailUIChecking)
                {
                    paymentPage.EnterInvalidNoMatchBSBAndCheckErrorMessage(petQuote.PayMethod);
                }

                nextInstallment = paymentPage.FillPayment(petQuote.PayMethod);

                Reporting.IsTrue(paymentPage.IsPayNowButtonEnabled, "Pay now button".IsEnabled());

                if (!DataHelper.WouldAnyPolicyholdersTriggerRevisedPricing(new List<Contact>() { petQuote.PolicyHolder }))
                    Reporting.AreEqual(expectedPrice, nextInstallment, "premium from quote page matches premium presented at payment.");

                paymentPage.VerifyAuthorisationMessageIsCorrect();

                if (!petQuote.PayMethod.IsPaymentByBankAccount)
                {
                    westpacIframe.EnterCardDetails(petQuote.PayMethod);
                }

                paymentPage.VerifySubmitButtonTextIsCorrect<QuotePet>(petQuote.PayMethod);

                //Terms and condition checkbox is displayed for all payments except Annual Credit Card
                if (!paymentPage.HasAnnualCreditCardPaymentMethodBeenEntered())
                {
                    paymentPage.ClickReadAgreeAuthorisationTerms();
                }

                paymentPage.ClickSubmit();
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC, nextPage: confirmation);
            }
            return nextInstallment;
        }
    }
}