using Rac.TestAutomation.Common;
using System.Text.RegularExpressions;
using UIDriver.Helpers;
using UIDriver.Pages;
using UIDriver.Pages.B2C;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Tests.ActionsAndValidations
{
    public static class ActionsQuoteMotor
    {
        public static string PurchaseMotorVehiclePolicy(Browser browser, QuoteCar vehicleQuote, bool includeRetrieveQuote = false, bool includeRequestEmailQuote = false, bool detailUIChecking = false)
        {
            decimal agreedFirstPayment = 0;
            string  chosenVehicle      = null;
            string  policyNumber       = null;
            string receiptNumber       = string.Empty;

            OpenBaseB2CLandingPage(browser: browser);

            chosenVehicle = SubmitInitialMotorQuoteRatingValues(browser: browser, vehicleQuote: vehicleQuote);

            var quoteNumber = UpdateInitialQuotePage(browser: browser, vehicleQuote: vehicleQuote, agreedQuotePrice: out agreedFirstPayment, includeRequestEmailQuote: includeRequestEmailQuote);

            if (includeRetrieveQuote)
            {
                browser.CloseBrowser();
                ActionsQuote.RetrieveB2CQuote(browser, vehicleQuote.ParkingAddress, quoteNumber, ShieldProductType.MGP);
                RestoreAnyAdditionalCoversAfterRetrieve(browser, vehicleQuote);
                Reporting.Log($"Capturing image after restoring any reset covers.", browser.Driver.TakeSnapshot());
            }

            browser.MotorQuoteClickBuyPage2();

            browser.MotorQuoteAddedDetailsPage3(vehicleQuote);

            VerifyQuoteMotor.VerifyQuoteSummaryPage(browser: browser,
                                                            vehicleQuote: vehicleQuote,
                                                            insuredVehicle: chosenVehicle);
            AcceptQuoteSummary(browser);

            SubmitPayment(browser: browser, vehicleQuote: vehicleQuote, expectedPrice: agreedFirstPayment, detailUIChecking);

            policyNumber = VerifyQuoteMotor.VerifyMotorVehicleConfirmationPage(browser: browser,
                                                                               vehicleQuote: vehicleQuote,
                                                                               expectedPrice: agreedFirstPayment,
                                                                               insuredVehicle: chosenVehicle, 
                                                                               receiptNumber: out receiptNumber);

            if (vehicleQuote.PayMethod.Scenario == PaymentScenario.AnnualCash)
                Payment.VerifyWestpacQuickStreamDetailsAreCorrect<QuoteCar>(cardDetails: vehicleQuote.PayMethod.CreditCardDetails,
                                                                            policyNumber: policyNumber,
                                                                            expectedPrice: agreedFirstPayment,
                                                                            expectedReceiptNumber: receiptNumber,
                                                                            isMotorQuoteIncludingRoadside: vehicleQuote.AddRoadside);

            return policyNumber;
        }
        
        public static void OpenBaseB2CLandingPage(Browser browser)
        {
            LaunchPage.OpenB2CLandingPageAndFeatureToggles(browser);
            browser.LaunchPageBeginNewMotorQuote();
        }

        /// <summary>
        /// Deals with "page 1" of the general motor vehicle policy
        /// purchase flow. Enters all the initial values for Shield
        /// to provide a quote price for the user.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="vehicleQuote"></param>
        /// <returns></returns>
        public static string SubmitInitialMotorQuoteRatingValues(Browser browser, QuoteCar vehicleQuote)
        {
            string insuredVehicle = string.Empty;
            using (var quotePage1 = new MotorQuote1Details(browser))
            using (var quotePage2 = new MotorQuote2Quote(browser))
            using (var spinner = new RACSpinner(browser))
            {
                insuredVehicle = Regex.Replace(quotePage1.FillQuoteDetails(vehicleQuote, false), ",", "");

                Reporting.Log($"Verify chosen vehicle matching search items: {insuredVehicle}");
                Reporting.IsTrue(insuredVehicle.ToUpper().Contains(vehicleQuote.Year.ToString()), $"Expected {insuredVehicle} to contain {vehicleQuote.Year}");
                Reporting.IsTrue(insuredVehicle.ToUpper().Contains(vehicleQuote.Make.ToUpper()),  $"Expected {insuredVehicle} to contain {vehicleQuote.Make}");
                Reporting.IsTrue(insuredVehicle.ToUpper().Contains(vehicleQuote.Model.ToUpper()), $"Expected {insuredVehicle} to contain {vehicleQuote.Model}");
                Reporting.IsTrue(insuredVehicle.ToUpper().Contains(vehicleQuote.Body.ToUpper()),  $"Expected {insuredVehicle} to contain {vehicleQuote.Body}");
                quotePage1.ClickContinueFromCurrentDriver(vehicleQuote.Drivers.Count);
                if (vehicleQuote.DisclosureDeclineThenDismiss)
                {
                    quotePage1.MotorWaitForDeclinedCoverNoticeAndDismiss(browser, vehicleQuote);
                }
                spinner.WaitForSpinnerToFinish(nextPage: quotePage2);
                
            }

            return insuredVehicle;
        }

        /// <summary>
        /// Deals with "page 2" of the general motor vehicle policy
        /// purchase flow. Makes changes based on test data in "vehicleQuote".
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="vehicleQuote">test data</param>
        /// <param name="agreedQuotePrice">This choice of either annual/monthly price is derived from values in "vehicleQuote"</param>
        /// <returns>Quote reference code</returns>
        public static string UpdateAndSubmitInitialQuotePage(Browser browser, QuoteCar vehicleQuote, out decimal agreedQuotePrice)
        {
            var quoteNumber = UpdateInitialQuotePage(browser, vehicleQuote, out agreedQuotePrice);
            browser.MotorQuoteClickBuyPage2();
            return quoteNumber;
        }

        /// <summary>
        /// Deals with "page 2" of the general motor vehicle policy
        /// purchase flow. Makes changes based on test data in "vehicleQuote".
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="vehicleQuote">test data</param>
        /// <param name="agreedQuotePrice">This choice of either annual/monthly price is derived from values in "vehicleQuote"</param>
        /// <returns>Quote reference code</returns>
        public static string UpdateInitialQuotePage(Browser browser, QuoteCar vehicleQuote, out decimal agreedQuotePrice, bool includeRequestEmailQuote = false)
        {
            string quoteNumber = string.Empty;

            browser.MotorQuoteEditCoverPage2(vehicleQuote);
            using (var spinner = new RACSpinner(browser))
            using (var quotePage2 = new MotorQuote2Quote(browser))
            {
                // Buffer details:
                quoteNumber = quotePage2.QuoteReference;
                if (vehicleQuote.InsuredVariance != 0)
                {
                    quotePage2.ActionSumInsuredVariance(vehicleQuote.InsuredVariance);
                    spinner.WaitForSpinnerToFinish();
                    Reporting.Log($"Sum Insured should have been altered by {vehicleQuote.InsuredVariance}%", browser.Driver.TakeSnapshot());
                }

                if (vehicleQuote.AddRoadside)
                {
                    quotePage2.ClickRoadsideAssistance(true);
                    spinner.WaitForSpinnerToFinish();
                    Reporting.IsTrue(quotePage2.IsRoadsidePriceShown, "that after adding roadside assistance, the price is now added to quote premium on screen");
                    Reporting.Log($"Roadside price should now be shown.", browser.Driver.TakeSnapshot());
                }

                if (vehicleQuote.CoverType == Constants.PolicyMotor.MotorCovers.MFCO)
                {
                    Reporting.Log($"Checking NCB Protection fields when Excess and NCB Changes are enabled.");
                    quotePage2.VerifyAddNCBProtectionIsNotDisplayed();
                }

                if (vehicleQuote.AddHireCarAfterAccident)
                {
                    quotePage2.ClickHireCarAfterAccident(true);
                    spinner.WaitForSpinnerToFinish();
                    Reporting.Log("Hire Car After Accident should now be selected.", browser.Driver.TakeSnapshot());
                }

                agreedQuotePrice    = vehicleQuote.PayMethod.IsAnnual ? quotePage2.QuotePriceAnnual : quotePage2.QuotePriceMonthly;
                vehicleQuote.Excess = string.IsNullOrEmpty(vehicleQuote.Excess) ?
                                        quotePage2.QuoteExcess.StripMoneyNotations() :
                                        vehicleQuote.Excess;

                if (vehicleQuote.AddRoadside)
                {
                    decimal tRoadsidePrice = quotePage2.QuoteRoadsidePrice;
                    Reporting.IsTrue(tRoadsidePrice > 100, "Roadside assistance price should be some value over $100");

                    decimal tPolicyCombinedPrice = quotePage2.QuotePriceCombined;
                    Reporting.AreEqual(tPolicyCombinedPrice, quotePage2.QuotePriceAnnual + tRoadsidePrice, "combined premium with roadside assist should be equal.");
                }
                if (includeRequestEmailQuote)
                {
                    Reporting.Log($"Quote Email Request initiated.");
                    quotePage2.SendQuoteByEmail(vehicleQuote.Drivers[0].Details.GetEmail(), isPrepopulated: true);
                    spinner.WaitForSpinnerToFinish();
                }
            }

            return quoteNumber;
        }

        /// <summary>
        /// Deals with "page 4" of the general motor vehicle policy
        /// purchase flow.
        /// </summary>
        /// <param name="browser"></param>
        public static void AcceptQuoteSummary(Browser browser)
        {
            using (var quotePage3  = new MotorQuote3Summary(browser))
            using (var paymentPage = new QuotePayments(browser))
            using (var spinner = new RACSpinner(browser))
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
        public static void SubmitPayment(Browser browser, QuoteCar vehicleQuote, decimal expectedPrice, bool detailUIChecking = false)
        {
            using (var paymentPage   = new QuotePayments(browser))
            using (var confirmation  = new MotorQuoteConfirmation(browser))
            using (var spinner       = new RACSpinner(browser))
            using (var westpacIframe = new WestpacQuickStream(browser))
            {
                Reporting.AreEqual(expectedPrice, paymentPage.GetNextInstallmentAmountForChosenFrequency(vehicleQuote.PayMethod.PaymentFrequency),
                    "Expected amount is displayed correctly");

                // Verify UI initial state are correct
                Reporting.IsTrue(paymentPage.VerifyUIElementsInPaymentDetailsPageAreCorrect(), "UI elements in payment details page are as expected");

                if (detailUIChecking)
                {
                   paymentPage.EnterInvalidNoMatchBSBAndCheckErrorMessage(vehicleQuote.PayMethod);
                }

                paymentPage.FillPayment(vehicleQuote.PayMethod);
                Reporting.Log("Payment method selected.", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(paymentPage.IsPayNowButtonEnabled, "Pay now button".IsEnabled());

                if (vehicleQuote.AddRoadside)
                {
                    if (vehicleQuote.PayMethod.Scenario != PaymentScenario.AnnualCash)
                    {
                        Reporting.AreEqual(expectedPrice, paymentPage.RoadsideStep1Price, "shown combined premium is as expected");
                        Reporting.AreEqual("Get $50 off your Classic Roadside Assistance.", paymentPage.RoadsideStep2Text, "roadside step 2 text");
                    }
                    else
                    {
                        Reporting.AreEqual(expectedPrice, paymentPage.CarInsurancePremiumPrice, "shown car insurance premium is as expected");
                        Reporting.AreEqual(paymentPage.CarInsurancePremiumPrice + paymentPage.ClassicRoadsideAssistancePrice, paymentPage.CarInsuranceAndRoadsidePrice, "shown combined premium is as expected");
                    }
                }

                paymentPage.VerifyAuthorisationMessageIsCorrect();

                if (!vehicleQuote.PayMethod.IsPaymentByBankAccount)
                {
                    westpacIframe.EnterCardDetails(vehicleQuote.PayMethod);
                }

                //Terms and condition checkbox is displayed for all payments except Annual Credit Card
                if (!paymentPage.HasAnnualCreditCardPaymentMethodBeenEntered())
                {
                    paymentPage.ClickReadAgreeAuthorisationTerms();
                }

                paymentPage.VerifySubmitButtonTextIsCorrect<QuoteCar>(vehicleQuote.PayMethod, vehicleQuote.AddRoadside);
                Reporting.Log("Capturing completed Payment Details fields before attempting to proceed.", browser.Driver.TakeSnapshot());
                paymentPage.ClickSubmit();
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC, nextPage: confirmation);
            }
        }
        /// <summary>
        /// Re-populates RSA Bundling/NCBP/Hire Car After Accident if they are
        /// desired as part of test data and have not been restored by Retrieve
        /// Quote. 
        /// Currently expect to only see this actioned for RSA Bundling.
        /// </summary>
        private static void RestoreAnyAdditionalCoversAfterRetrieve(Browser browser, QuoteCar vehicleQuote)
        {
            Reporting.Log($"Checking Optional Covers after Retrieve Quote and re-applying if necessary from test data. Capturing BEFORE image here.", browser.Driver.TakeSnapshot());
            using (var spinner = new RACSpinner(browser))
            using (var quotePage2 = new MotorQuote2Quote(browser))
            {
                if (vehicleQuote.AddRoadside)
                {
                    Reporting.Log($"Is Roadside Price Shown? (non-failing check - Roadside Bundling not expected to be retained after Retrieve Quote) = {quotePage2.IsRoadsidePriceShown}");
                    quotePage2.ClickRoadsideAssistance(true);
                    spinner.WaitForSpinnerToFinish();
                    Reporting.IsTrue(quotePage2.IsRoadsidePriceShown, "to confirm Roadside Price shown is True?");
                }

                if (vehicleQuote.AddHireCarAfterAccident)
                {
                    quotePage2.ClickHireCarAfterAccident(true);
                    spinner.WaitForSpinnerToFinish();
                }
            }
        }
    }
}