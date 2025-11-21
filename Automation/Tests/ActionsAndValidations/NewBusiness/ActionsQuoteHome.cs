using Rac.TestAutomation.Common;
using System;
using System.Threading;
using UIDriver.Helpers;
using UIDriver.Pages;
using UIDriver.Pages.B2C;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace Tests.ActionsAndValidations
{
    public static class ActionsQuoteHome
    {
        /// <summary>
        /// Performs the complete new business flow of a home policy, from getting
        /// a quote, through to completing purchase.
        /// </summary>
        /// <returns>Policy number of new policy.</returns>
        public static string PurchaseHomePolicy(Browser browser, QuoteHome quoteDetails, bool detailUIChecking = false)
        {
            GetNewHomeQuote(browser, quoteDetails, out QuoteData premiumValues);
            VerifyExcessDetails(browser, quoteDetails);
            return ProceedWithThePurchaseOfQuote(browser, quoteDetails, premiumValues, detailUIChecking);
        }

        /// <summary>
        /// Open B2C and proceed to a new Home Quote (page 2), including
        /// updating any desired changes to excess, additional covers
        /// and sum insured.
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <param name="premiumValues">Returning the buffered premium, excess and SI values</param>
        /// <returns>quote number</returns>
        public static string GetNewHomeQuote(Browser browser, QuoteHome quoteDetails, out QuoteData premiumValues)
        {
            OpenB2CBeginHomeQuote(browser: browser);
            SubmitInitialHomeQuoteRatingValues(browser: browser, quoteDetails: quoteDetails);

            var quoteNumber = UpdateInitialQuotePage(browser: browser, quoteDetails: quoteDetails);
            premiumValues = GetPremiumsAndCoverValues(browser, quoteDetails);

            return quoteNumber;
        }

        /// <summary>
        /// Completes the purchase of the home policy from the quote page,
        /// through to completing payment.
        /// </summary>
        /// <param name="quoteDetails">Input test data to drive responses to UI forms</param>
        /// <param name="premiumValues">A record of the premiums from the quote page which is used for verifying billing on the purchase page.</param>
        /// <returns>Returns the policy number generated from the purchase.</returns>
        public static string ProceedWithThePurchaseOfQuote(Browser browser, QuoteHome quoteDetails, QuoteData premiumValues, bool detailUIChecking = false)
        {
            HomeQuoteClickBuyPage2(browser);
            HomeQuoteAddedDetailsPage3(browser, quoteDetails);
            
            VerifyQuoteHome.VerifyQuoteSummaryPage(browser: browser, quoteDetails: quoteDetails);
            
            return AcceptQuoteSummaryAndPurchase(browser, quoteDetails, premiumValues, detailUIChecking);
        }

        public static void HomeQuoteClickBuyPage2(Browser browser)
        {
            using (var quotePage2 = new HomeQuote2Quote(browser))
            using (var quotePage3 = new HomeQuote3Policy(browser))
            using (var spinner = new RACSpinner(browser))
            {
                quotePage2.ClickBuyOnlineButton();
                spinner.WaitForSpinnerToFinish(nextPage: quotePage3);
            }
        }

        /// <summary>
        /// Completes all details on page 3 of the home quote flow
        /// * Building details
        /// * Home details disclosure
        /// * policyholder details
        /// </summary>
        public static void HomeQuoteAddedDetailsPage3(Browser browser, QuoteHome quoteDetails)
        {
            using (var quotePage3 = new HomeQuote3Policy(browser))
            using (var quoteSummary = new HomeQuote3Summary(browser))
            using (var spinner = new RACSpinner(browser))
            using (var callback = new QuoteCallback(browser))
            {
                quotePage3.FillInBuildingDetails(quoteDetails);
                quotePage3.FillInHomeDetails(quoteDetails);
                quotePage3.FillInPolicyholderDetails(quoteDetails);

                if (quoteDetails.IsHomeUsageUnacceptable)
                {
                    spinner.WaitForSpinnerToFinish(nextPage: callback);
                    callback.VerifyIfHasPrepopulatedWithContactDetails(quoteDetails.PolicyHolders[0]);
                    callback.DismissCallbackDialog();

                    quotePage3.ClearHomeDetailsDisclosureAndSubmitPage(quoteDetails);
                }
                spinner.WaitForSpinnerToFinish(nextPage: quoteSummary);
            }
        }

        /// <summary>
        /// Supporting the case where the quote has been retrieved and
        /// page 3 had been previously submitted. This method will verify
        /// that the previously entered values have been retained. Mailing
        /// addresses need to be re-entered, then the page is submitted.
        /// </summary>
        public static void HomeQuoteAddedDetailsPage3VerifyAndSubmit(this Browser browser, QuoteHome quoteDetails)
        {
            using (var quotePage3 = new HomeQuote3Policy(browser))
            using (var quoteSummary = new HomeQuote3Summary(browser))
            using (var spinner = new RACSpinner(browser))
            {
                quotePage3.VerifyPopulatedBuildingDetails(quoteDetails);
                quotePage3.VerifyPopulatedHomeDetails(quoteDetails);
                quotePage3.VerifyPolicyholderDetails(quoteDetails);

                spinner.WaitForSpinnerToFinish(nextPage: quoteSummary);
            }
        }

        public static string AcceptQuoteSummaryAndPurchase(Browser browser, QuoteHome quoteDetails, QuoteData premiumValues, bool detailUIChecking = false)
        {
            AcceptQuoteSummary(browser, quoteDetails, premiumValues);

            SubmitPayment(browser: browser, quoteDetails: quoteDetails, expectedPremiumValues: premiumValues, detailUIChecking);

            var policyNumber = VerifyQuoteHome.VerifyHomeConfirmationPage(browser: browser, quoteDetails: quoteDetails, expectedPremiumValues: premiumValues, receiptNumber: out string receiptNumber);

            // CoC only applicable where building cover is involved.
            if (quoteDetails.BuildingValue.HasValue)
                RequestCertificateOfCurrency(browser);

            if (quoteDetails.PayMethod.Scenario == PaymentScenario.AnnualCash)
            {
                Payment.VerifyWestpacQuickStreamDetailsAreCorrect<QuoteHome>(cardDetails: quoteDetails.PayMethod.CreditCardDetails,
                                                            policyNumber: policyNumber,
                                                            expectedPrice: premiumValues.AnnualPremium,
                                                            expectedReceiptNumber: receiptNumber);
            }

            return policyNumber;
        }
        
        public static QuoteData GetPremiumsAndCoverValues(Browser browser, QuoteHome quoteDetails)
        {
            QuoteData premiumsAndCoverValues;
            using (var quotePage2 = new HomeQuote2Quote(browser))
            {
                premiumsAndCoverValues = quotePage2.GetQuoteCoverValuesAndPremium(quoteDetails);
            }
            return premiumsAndCoverValues;
        }

        private static void OpenB2CBeginHomeQuote(Browser browser)
        {
            LaunchPage.OpenB2CLandingPageAndFeatureToggles(browser);
            browser.LaunchPageBeginNewHomeQuote();
        }

        /// <summary>
        /// Deals with "page 1" of the home policy purchase flow.
        /// Enters all the initial values for Shield to provide
        /// a quote price for the user.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="quoteDetails"></param>
        /// <returns></returns>
        private static void SubmitInitialHomeQuoteRatingValues(Browser browser, QuoteHome quoteDetails)
        {
            using (var quotePage1 = new HomeQuote1Details(browser))
            using (var quotePage2 = new HomeQuote2Quote(browser))
            using (var callback = new QuoteCallback(browser))
            using (var spinner = new RACSpinner(browser))
            {
                // Because the page is actually not always ready for us to start
                // clicking stuff even though the controls are rendered.
                Thread.Sleep(2000);

                quotePage1.FillQuoteDetails(quoteDetails);

                if (quoteDetails.IsContentsAboveSecurityLimit())
                {
                    spinner.WaitForSpinnerToFinish(nextPage: callback);
                    callback.DismissCallbackDialog();

                    // Revise sum insured below threshold and proceed.
                    quoteDetails.ContentsValue = DataHelper.RandomNumber(minValue: HOME_CONTENTS_SI_MIN, maxValue: HOME_CONTENTS_SI_MAX_NO_SECURITY);
                    Reporting.Log($"Callback dialog navigated and contents SI revised down to ${quoteDetails.ContentsValue.Value} to enable test to proceed.");

                    quotePage1.ContentsSumInsured = quoteDetails.ContentsValue.Value;

                    quotePage1.SubmitForm();
                }
                spinner.WaitForSpinnerToFinish(nextPage: quotePage2);
            }
        }

        /// <summary>
        /// Deals with "page 2" of the home policy purchase flow.
        /// Makes changes based on test data in "quoteDetails".
        /// Does not select the 'Buy online' button. Allows for the
        /// test to do further test activities.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="quoteDetails">test data</param>
        /// <returns>Quote reference code</returns>
        private static string UpdateInitialQuotePage(Browser browser, QuoteHome quoteDetails)
        {
            string quoteNumber = string.Empty;

            HomeQuoteEditCoverDetails(browser, quoteDetails);

            using (var spinner = new RACSpinner(browser))
            using (var quotePage2 = new HomeQuote2Quote(browser))
            {
                // Buffer details:
                quoteNumber = quotePage2.QuoteReference;

                // We buffer excess values if were relying on default, so we can verify against it later.
                if (quoteDetails.BuildingValue.HasValue)
                {
                    quoteDetails.ExcessBuilding = string.IsNullOrEmpty(quoteDetails.ExcessBuilding) ?
                                                  quotePage2.QuoteBuildingExcess :
                                                  quoteDetails.ExcessBuilding;
                }

                if (quoteDetails.ContentsValue.HasValue)
                {
                    quoteDetails.ExcessContents = string.IsNullOrEmpty(quoteDetails.ExcessContents) ?
                                                  quotePage2.QuoteContentsExcess :
                                                  quoteDetails.ExcessContents;
                }
            }
            return quoteNumber;
        }

        private static void HomeQuoteEditCoverDetails(Browser browser, QuoteHome quoteDetails)
        {
            using (var quotePage2 = new HomeQuote2Quote(browser))
            using (var spinner = new RACSpinner(browser))
            {
                Reporting.Log($"Beginning update of new home quote: {quotePage2.QuoteReference}");

                if (!string.IsNullOrEmpty(quoteDetails.ExcessBuilding))
                {
                    Reporting.Log("UPDATE Building Excess.");
                    quotePage2.QuoteBuildingExcess = quoteDetails.ExcessBuilding;
                    spinner.WaitForSpinnerToFinish();
                }

                if (!string.IsNullOrEmpty(quoteDetails.ExcessContents))
                {
                    Reporting.Log("UPDATE Contents Excess.");
                    quotePage2.QuoteContentsExcess = quoteDetails.ExcessContents;
                    spinner.WaitForSpinnerToFinish();
                }

                // For most home quotes, the sum insured values get set on the first page of the quote
                // flow. The exception is Renters, which can only be modified on page 2.
                if (quoteDetails.Occupancy == HomeOccupancy.Tenant)
                {
                    quotePage2.QuoteContentsSumInsured = quoteDetails.ContentsValue.Value;
                    spinner.WaitForSpinnerToFinish();
                }

                if (quoteDetails.StartDate.Date != DateTime.Now.Date)
                {
                    Reporting.Log("UPDATE Policy start date.");
                    quotePage2.QuoteStartDate = quoteDetails.StartDate;
                    spinner.WaitForSpinnerToFinish();
                }

                // Purchasing Accidental Damage cover is currently available for Owner occupied - Building & contents or Owner occupied - Contents only
                if (quoteDetails.IsEligibleForAccidentalDamage && quoteDetails.AddAccidentalDamage)
                {
                    Reporting.Log("Add Accidental Damage.");
                    quotePage2.TickAccidentalDamage(true);
                    spinner.WaitForSpinnerToFinish();
                    Reporting.Log($"Accidental Damage added.", browser.Driver.TakeSnapshot());
                }

                if (quoteDetails.SpecifiedValuablesOutside != null)
                {
                    quotePage2.AddSpecifiedPersonalValuables(quoteDetails.SpecifiedValuablesOutside);
                    spinner.WaitForSpinnerToFinish();
                }

                if (quoteDetails.UnspecifiedValuablesInsuredAmount != UnspecifiedPersonalValuables.None)
                {
                    quotePage2.UnspecifiedValuablesCover = quoteDetails.UnspecifiedValuablesInsuredAmount;
                    spinner.WaitForSpinnerToFinish();
                }

                if (quoteDetails.SpecifiedValuablesInside != null)
                {
                    quotePage2.AddSpecifiedContents(quoteDetails.SpecifiedValuablesInside);
                    spinner.WaitForSpinnerToFinish();
                }

                quotePage2.SendQuoteByEmail(quoteDetails.PolicyHolders[0].GetEmail(), isPrepopulated: true);
                spinner.WaitForSpinnerToFinish();

                // Buffer details:
                var quoteNumber = quotePage2.QuoteReference;
                var quotePrice = quoteDetails.PayMethod.IsAnnual ? quotePage2.QuotePriceAnnual : quotePage2.QuotePriceMonthly;
                var quoteDate = quotePage2.QuoteStartDate.ToString(DataFormats.DATE_ABBREVIATED_MONTH_FORWARD_FORWARDSLASH);

                Reporting.Log($"Quote ({quoteNumber}) successfully generated. Quoted price: {quotePrice} Set Start Date: {quoteDate}", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Progresses past the Quote Summary page. Will check the premium header
        /// in case there had been a change in premiums due to member match.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="quoteDetails"></param>
        /// <param name="expectedPremiums"></param>
        private static void AcceptQuoteSummary(Browser browser, QuoteHome quoteDetails, QuoteData expectedPremiums)
        {
            using (var quotePage3  = new HomeQuote3Summary(browser))
            using (var paymentPage = new QuotePayments(browser))
            using (var spinner     = new RACSpinner(browser))
            {
                quotePage3.WaitForPage();

                if (!DataHelper.WouldAnyPolicyholdersTriggerRevisedPricing(quoteDetails.PolicyHolders))
                {
                    expectedPremiums.AnnualPremium = quotePage3.HeaderQuotePriceAnnual;
                    expectedPremiums.MonthlyPremium = quotePage3.HeaderQuotePriceMonthly;
                }
                
                quotePage3.ClickContinue();
                spinner.WaitForSpinnerToFinish(nextPage: paymentPage);
            }
        }

        /// <summary>
        /// Deals with the payment page of the home policy purchase flow.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="quoteDetails">test data input values</param>
        /// <param name="expectedPremiumValues">contains expected premium values to verify against payment screen.</param>
        private static void SubmitPayment(Browser browser, QuoteHome quoteDetails, QuoteData expectedPremiumValues, bool detailUIChecking = false)
        {
            decimal nextInstallment = 0;

            using (var paymentPage   = new QuotePayments(browser))
            using (var confirmation  = new HomeQuoteConfirmation(browser))
            using (var spinner       = new RACSpinner(browser))
            using (var westpacIframe = new WestpacQuickStream(browser))
            {
                Reporting.IsTrue(paymentPage.VerifyUIElementsInPaymentDetailsPageAreCorrect(), "UI elements in payment details page are as expected");

                if (detailUIChecking)
                {
                    paymentPage.EnterInvalidNoMatchBSBAndCheckErrorMessage(quoteDetails.PayMethod);                   
                }

                nextInstallment = paymentPage.FillPayment(quoteDetails.PayMethod);

                var expectedPrice = quoteDetails.PayMethod.IsAnnual ? expectedPremiumValues.AnnualPremium : expectedPremiumValues.MonthlyPremium;
                Reporting.AreEqual(expectedPrice, nextInstallment, "premium from quote page matches premium presented at payment.");

                Reporting.IsTrue(paymentPage.IsPayNowButtonEnabled, "Pay now button".IsEnabled());

                paymentPage.VerifyAuthorisationMessageIsCorrect();

                if (!quoteDetails.PayMethod.IsPaymentByBankAccount)
                {
                    westpacIframe.EnterCardDetails(quoteDetails.PayMethod);
                }

                paymentPage.VerifySubmitButtonTextIsCorrect<QuoteHome>(quoteDetails.PayMethod);

                //Terms and condition checkbox is displayed for all payments except Annual Credit Card
                if (!paymentPage.HasAnnualCreditCardPaymentMethodBeenEntered())
                {
                    paymentPage.ClickReadAgreeAuthorisationTerms();
                }

                paymentPage.ClickSubmit();
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC, nextPage: confirmation);
            }
        }

        private static void RequestCertificateOfCurrency(Browser browser)
        {
            using (var confirmation = new HomeQuoteConfirmation(browser))
            {
                confirmation.EmailCertificateOfCurrency();
            }
        }

        public static void VerifyExcessDetails(Browser browser, QuoteHome quoteDetails)
        {
            using (var quotePage2 = new HomeQuote2Quote(browser))
            {
                // TODO: B2C-5274 to include Tenant tooltip check when the ticket is fixed
                if (quoteDetails.Occupancy != HomeOccupancy.Tenant)
                {
                    quotePage2.VerifyExcessMessage(quoteDetails);
                }
                else
                {
                    // Do nothing. leave this for now until B2C-5274 is fixed
                }
            }
        }
    }
}