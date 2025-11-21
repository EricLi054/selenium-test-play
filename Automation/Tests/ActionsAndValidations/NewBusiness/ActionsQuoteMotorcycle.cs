using Rac.TestAutomation.Common;
using System.Threading;
using UIDriver.Pages;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.MotorcycleQuote;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace Tests.ActionsAndValidations
{
    public static class ActionsQuoteMotorcycle
    {
        /// <summary>
        /// Works through the full flow of opening a web browser to the
        /// motorcycle quote page, obtaining a quote, and then completing
        /// payment to issue a new policy.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testConfig"></param>
        /// <param name="quote">test data defining all input parameters</param>
        /// <returns></returns>
        public static string PurchaseMotorcyclePolicy(Browser browser, QuoteMotorcycle quote)
        {
            ActionsQuoteMotorcycle.FetchNewMotorCycleQuote(browser, quote);
            return ProceedWithQuoteToPurchase(browser, quote);
        }

        /// <summary>
        /// Opens a new web browser and proceeds to obtain a
        /// motorcycle quote. No modifications are made to
        /// the initial covers, agreed value or excess
        /// presented in the quote.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testConfig"></param>
        /// <param name="quote">test data defining all input parameters</param>
        public static void FetchNewMotorCycleQuote(Browser browser, QuoteMotorcycle quote)
        {
            /***********************************************************
             * Open SPARK launch page.
             ***********************************************************/
            LaunchPage.OpenSparkMotorcycleLandingPage(browser);

            /***********************************************************
             * Select RAC Member asNo
             ***********************************************************/
            ActionsQuoteMotorcycle.SelectRACMember(browser, quote.Drivers[0].Details);
            Reporting.Log($"Capturing Screenshot of Select RAC Member", browser.Driver.TakeSnapshot());

            /***********************************************************
             * Complete Page 1 details
             ***********************************************************/
            ActionsQuoteMotorcycle.LetsStartWithYourBike(browser, quote);
            Reporting.Log($"Capturing Screenshot of Lets Start With Your Bike", browser.Driver.TakeSnapshot());

            // Dirty hack, otherwise we move too fast and motorcycle online
            // javascript can't keep up.
            System.Threading.Thread.Sleep(3000);

            /***********************************************************
             * Complete Page Tell Us more about your bike
             ***********************************************************/
            ActionsQuoteMotorcycle.TellUsMoreAboutYourBike(browser, quote);
            Reporting.Log($"Capturing Screenshot of Tell Us More About Your Bike", browser.Driver.TakeSnapshot());

            /***********************************************************
             * Ammend Rider details and experience
             ***********************************************************/
            ActionsQuoteMotorcycle.NowABitAboutYou(browser, quote);

            using (var heresYourQuote = new HeresYourQuote(browser))
            {
                heresYourQuote.WaitForPage(WaitTimes.T90SEC);
                Reporting.Log($"Capturing Screenshot on landing at Here's Your Quote", browser.Driver.TakeSnapshot());
                browser.PercyScreenCheck(MotorcycleNewBusiness.HeresYourQuote, heresYourQuote.GetPercyIgnoreCSS());
            }
        }

        /// <summary>
        /// Proceeds to issue a new motorcycle policy from the
        /// "Here's Your Quote" page by navigating the remaining
        /// pages and finalising payment. Fulfills the cover,
        /// excess and agreed value identified in the test data.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="quote"></param>
        /// <param name="detailUIChecking"></param>
        /// <returns></returns>
        public static string ProceedWithQuoteToPurchase(Browser browser, QuoteMotorcycle quote, bool detailUIChecking=false)
        {
            /***********************************************************
            * Select the cover type and proceed with the quote
            ***********************************************************/
            ActionsQuoteMotorcycle.AdjustQuoteParametersAndBeginPurchase(browser, quote);
            Reporting.Log($"Capturing Screenshot when cover option selected", browser.Driver.TakeSnapshot());
            /***********************************************************
             * Select policy start date and driver details
             ***********************************************************/
            ActionsQuoteMotorcycle.LetsClarifyFewMoreDetails(browser, quote);
            Reporting.Log($"Capturing Screenshot of Confirm Policy Details", browser.Driver.TakeSnapshot());
            /***********************************************************
            * Enter member details 
            ***********************************************************/
            ActionsQuoteMotorcycle.TellUsMoreAboutYou(browser, quote);
            Reporting.Log($"Capturing Screenshot of Tell Us More About You", browser.Driver.TakeSnapshot());

            /***********************************************************
           * Enter payment details and purchase policy
           ***********************************************************/
            VerifyQuoteMotorcycle.VerifyQuoteDetailsOnPaymentPage(browser, quote);

            if (detailUIChecking)
            {
                using (var paymentDetails = new PaymentDetails(browser))
                {
                    Reporting.Log($"Capturing Screenshot of Enter Payment Detail", browser.Driver.TakeSnapshot());
                    browser.PercyScreenCheck(MotorcycleNewBusiness.EnterPaymentDetailsAndPurchasePolicy, paymentDetails.GetPercyIgnoreCSS());
                    paymentDetails.EnterInvalidNoMatchBSBAndCheckErrorMessage(quote.PayMethod);
                }
            }

            return ActionsQuoteMotorcycle.EnterPaymentDetailAndPurchasePolicy(browser, quote);
        }

        /// <summary>
        /// Answers the "Are you a member?" question.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="contact"></param>
        public static void SelectRACMember(Browser browser, Contact contact)
        {
            using (var beforeWeGetStarted = new BeforeWeGetStarted(browser))
            using (var progressBar = new MotorcycleProgressBar(browser))
            {
                beforeWeGetStarted.WaitForPage();
                Reporting.IsTrue(progressBar.CurrentStepDuringQuoteStages() == MotorcycleProgressBar.PAGE_QUOTE.MemberDetails,
                                 $"Progress bar state, {progressBar.CurrentStepDuringQuoteStages()} to be {MotorcycleProgressBar.PAGE_QUOTE.MemberDetails}");

                // Static sleep because first time loads seem to cause this page to render slowly
                // and we often miss that first click.
                Thread.Sleep(5000);
                browser.PercyScreenCheck(MotorcycleNewBusiness.SelectRACMember);
                beforeWeGetStarted.SelectAreYouAnRACMember(contact);
            }
        }

        // SearchForMotorcycle
        private static void LetsStartWithYourBike(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var letsStartWithYourBike = new LetsStartWithYourBike(browser))
            using (var progressBar = new MotorcycleProgressBar(browser))
            {
                letsStartWithYourBike.WaitForPage();
                Reporting.IsTrue(progressBar.CurrentStepDuringQuoteStages() == MotorcycleProgressBar.PAGE_QUOTE.YourBike,
                                 $"Progress bar state, {progressBar.CurrentStepDuringQuoteStages()} to be {MotorcycleProgressBar.PAGE_QUOTE.YourBike}");
                browser.PercyScreenCheck(MotorcycleNewBusiness.LetsStartWithYourBike);
                letsStartWithYourBike.SearchForMotorcycle(quoteMotorcycle);
                letsStartWithYourBike.ClickNext();
            }
        }

        private static void TellUsMoreAboutYourBike(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var tellUsMoreAboutYourBike = new TellUsMoreAboutYourBike(browser))
            using (var progressBar = new MotorcycleProgressBar(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: tellUsMoreAboutYourBike);
                }
                Reporting.IsTrue(progressBar.CurrentStepDuringQuoteStages() == MotorcycleProgressBar.PAGE_QUOTE.BikeUsage,
                                 $"Progress bar state, {progressBar.CurrentStepDuringQuoteStages()} to be {MotorcycleProgressBar.PAGE_QUOTE.BikeUsage}");

                browser.PercyScreenCheck(MotorcycleNewBusiness.TellUsMoreAboutYourBike, tellUsMoreAboutYourBike.GetPercyIgnoreCSS());
                tellUsMoreAboutYourBike.FillQuoteDetails(quoteMotorcycle);           
            }
        }

        private static void NowABitAboutYou(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var progressBar = new MotorcycleProgressBar(browser))
            using (var nowABitAboutYou = new NowABitAboutYou(browser))
            {
                nowABitAboutYou.WaitForPage();
                Reporting.IsTrue(progressBar.CurrentStepDuringQuoteStages() == MotorcycleProgressBar.PAGE_QUOTE.AboutYou,
                                 $"Progress bar state, {progressBar.CurrentStepDuringQuoteStages()} to be {MotorcycleProgressBar.PAGE_QUOTE.AboutYou}");
                browser.PercyScreenCheck(MotorcycleNewBusiness.NowABitAboutYou);
                nowABitAboutYou.FillAboutYou(quoteMotorcycle);
            }
        }

        /// <summary>
        /// This method does the following:
        /// -Save initial quote details such as Excess and Sum Insured
        /// -Update the default Excess to a desired value
        /// -Capture updated premiums after the Excess is modified
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="quoteMotorcycle"></param>
        public static void AdjustQuoteParametersRemainOnQuotePage(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var heresYourQuote = new HeresYourQuote(browser))
            using (var progressBar = new MotorcycleProgressBar(browser))
            {
                Reporting.IsTrue(progressBar.CurrentStepDuringPolicyStages() == MotorcycleProgressBar.PAGE_POLICY.YourQuote,
                                 $"Progress bar state, {progressBar.CurrentStepDuringPolicyStages()} to be {MotorcycleProgressBar.PAGE_POLICY.YourQuote}");

                if (quoteMotorcycle.Excess == null)
                    quoteMotorcycle.Excess = heresYourQuote.GetExcess(quoteMotorcycle.CoverType);
                else
                {
                    heresYourQuote.UpdateDesiredExcessValues(quoteMotorcycle);
                    using (var spinner = new SparkSpinner(browser))
                    {
                        spinner.WaitForSpinnerToFinish();
                    }
                }

                if (quoteMotorcycle.CoverType != MotorCovers.TPO) {
                    quoteMotorcycle.SumInsuredFromQuotePage = heresYourQuote.GetSumInsured(quoteMotorcycle.CoverType); }

                Reporting.Log($"Our quote number is: {heresYourQuote.QuoteNumber}. Market value of bike is {quoteMotorcycle.SumInsuredFromQuotePage}. Initial premium ${quoteMotorcycle.PremiumAnnualFromQuotePage} annual, and ${quoteMotorcycle.PremiumMonthlyFromQuotePage} monthly", browser.Driver.TakeSnapshot());

                Reporting.AreEqual(heresYourQuote.GetSumInsured(MotorCovers.MFCO), heresYourQuote.GetSumInsured(MotorCovers.TFT), "default sum insured values across different covers.");

                if (quoteMotorcycle.InsuredVariance != 0)
                {
                    var desiredSumToInsure = (int)((1 + ((decimal)quoteMotorcycle.InsuredVariance / 100)) * quoteMotorcycle.SumInsuredFromQuotePage);
                    heresYourQuote.SetSumInsured(quoteMotorcycle.CoverType, desiredSumToInsure);
                    using (var spinner = new SparkSpinner(browser))
                    {
                        spinner.WaitForSpinnerToFinish();
                    }
                }

                quoteMotorcycle.PremiumAnnualFromQuotePage = heresYourQuote.GetPremiumAnnual(quoteMotorcycle.CoverType);
                quoteMotorcycle.PremiumMonthlyFromQuotePage = heresYourQuote.GetPremiumMonthly(quoteMotorcycle.CoverType);

                Reporting.Log($"Updated premiums are: ${quoteMotorcycle.PremiumAnnualFromQuotePage} annual, and ${quoteMotorcycle.PremiumMonthlyFromQuotePage} monthly", browser.Driver.TakeSnapshot());
            }
        }

        public static QuoteMotorcycle AdjustQuoteParametersAndBeginPurchase(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            AdjustQuoteParametersRemainOnQuotePage(browser, quoteMotorcycle);

            using (var heresYourQuote = new HeresYourQuote(browser))
            {
                heresYourQuote.ClickGetPolicy(quoteMotorcycle.CoverType);
            }
            return quoteMotorcycle;
        }

        private static void LetsClarifyFewMoreDetails(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var letsClarifyFewMoreDetails = new LetsClarifyFewMoreDetails(browser))
            using (var progressBar = new MotorcycleProgressBar(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: letsClarifyFewMoreDetails);
                }
                letsClarifyFewMoreDetails.WaitForPage();
                Reporting.IsTrue(progressBar.CurrentStepDuringPolicyStages() == MotorcycleProgressBar.PAGE_POLICY.ConfirmDetails,
                                 $"Progress bar state, {progressBar.CurrentStepDuringPolicyStages()} to be {MotorcycleProgressBar.PAGE_POLICY.ConfirmDetails}");
                browser.PercyScreenCheck(MotorcycleNewBusiness.LetsClarifyFewMoreDetails, letsClarifyFewMoreDetails.GetPercyIgnoreCSS());
                letsClarifyFewMoreDetails.FillClarifyFewDetails(quoteMotorcycle);
            }
        }

        private static void TellUsMoreAboutYou(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var tellUsMoreAboutYou = new TellUsMoreAboutYou(browser))
            using (var progressBar = new MotorcycleProgressBar(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: tellUsMoreAboutYou);
                }
                tellUsMoreAboutYou.WaitForPage();
                Reporting.IsTrue(progressBar.CurrentStepDuringPolicyStages() == MotorcycleProgressBar.PAGE_POLICY.PersonalInformation,
                                 $"Progress bar state, {progressBar.CurrentStepDuringPolicyStages()} to be {MotorcycleProgressBar.PAGE_POLICY.PersonalInformation}");
                browser.PercyScreenCheck(MotorcycleNewBusiness.EnterMemberDetails);
                tellUsMoreAboutYou.FillTellUsMoreAboutYou(quoteMotorcycle);

                if (quoteMotorcycle.IsPremiumChangeExpected)
                {
                    VerifyAnyPremiumChangePopup(browser, quoteMotorcycle);
                }
                else
                {
                    tellUsMoreAboutYou.VerifyNoPremiumPopupIsDisplayed();
                }
            }            
        }

        public static string VerifyQuoteBreakdownTextAndGetQuoteNumber(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var heresYourQuote = new HeresYourQuote(browser))
            {
                var quoteNumber = SaveQuotePremiumDetailsAndGetQuoteNumber(browser, quoteMotorcycle);

                heresYourQuote.ClickSeeQuoteBreakdown(quoteMotorcycle.CoverType);
                Reporting.Log("Quote Breakdown texts display", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(heresYourQuote.AreAllQuoteBreakdownTextsDisplayed(quoteMotorcycle.CoverType), "Quote Breakdown Texts are displayed");

                return quoteNumber;
            }
        }

        public static string VerifyCoverHelpTipAndGetQuoteNumber(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var heresYourQuote = new HeresYourQuote(browser))
            {
                var quoteNumber = SaveQuotePremiumDetailsAndGetQuoteNumber(browser, quoteMotorcycle);

                heresYourQuote.VerifyMemberDiscountIsDisplayedOrNot(quoteMotorcycle.Drivers[0].Details,  quoteMotorcycle);
               
                heresYourQuote.VerifyOnlineDiscountIsNotDisplayed();

                heresYourQuote.ClickHelpTip(quoteMotorcycle.CoverType);
                Reporting.Log("Help Dialogue display", browser.Driver.TakeSnapshot());
                if (quoteMotorcycle.CoverType == MotorCovers.MFCO)
                {
                    Reporting.IsTrue(heresYourQuote.IsMostPopularRibbonDisplayed(), "'Most Popular' Ribbon is displayed");
                }
                Reporting.IsTrue(heresYourQuote.IsBannerTextDisplayed(quoteMotorcycle.CoverType),
                    $"banner description of cover is as expected for {quoteMotorcycle.CoverType}");
                heresYourQuote.CloseHelpTip();

                return quoteNumber;
            }
        }
        
        public static void SelectCover(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var heresYourQuote = new HeresYourQuote(browser))
            {                
                heresYourQuote.ClickGetPolicy(quoteMotorcycle.CoverType);
                using (var letsClarifyFewMoreDetails = new LetsClarifyFewMoreDetails(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: letsClarifyFewMoreDetails);
                }
            }
        }

        public static string SaveQuotePremiumDetailsAndGetQuoteNumber(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var heresYourQuote = new HeresYourQuote(browser))
            {
                if (quoteMotorcycle.CoverType != MotorCovers.TPO)
                {
                    quoteMotorcycle.SumInsuredFromQuotePage = heresYourQuote.GetSumInsured(quoteMotorcycle.CoverType);
                }

                quoteMotorcycle.PremiumAnnualFromQuotePage = heresYourQuote.GetPremiumAnnual(quoteMotorcycle.CoverType);
                quoteMotorcycle.PremiumMonthlyFromQuotePage = heresYourQuote.GetPremiumMonthly(quoteMotorcycle.CoverType);

                Reporting.Log($"Our quote number is: {heresYourQuote.QuoteNumber}. Market value of bike is {quoteMotorcycle.SumInsuredFromQuotePage}. Initial premium ${quoteMotorcycle.PremiumAnnualFromQuotePage} annual, and ${quoteMotorcycle.PremiumMonthlyFromQuotePage} monthly", browser.Driver.TakeSnapshot());

                return heresYourQuote.QuoteNumber;
            }
        }

        private static string EnterPaymentDetailAndPurchasePolicy(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            string policyNumber = null;
            using (var paymentDetails = new PaymentDetails(browser))
            using (var progressBar = new MotorcycleProgressBar(browser))
            {
                Reporting.IsTrue(progressBar.CurrentStepDuringPolicyStages() == MotorcycleProgressBar.PAGE_POLICY.Payment,
                                $"Progress bar state, {progressBar.CurrentStepDuringPolicyStages()} to be {MotorcycleProgressBar.PAGE_POLICY.Payment}");
                
                paymentDetails.VerifyPaymentAmounts(quoteMotorcycle);

                paymentDetails.EnterPaymentDetailsAndPurchasePolicy(Vehicle.Motorcycle, quoteMotorcycle.PayMethod);
            }
            
            using (var confirmationPage = new ConfirmationPage(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC,nextPage: confirmationPage);
                }
                confirmationPage.WaitForPage();
                policyNumber = confirmationPage.PolicyNumber;
                Reporting.Log($"Policy Number is {policyNumber}", browser.Driver.TakeSnapshot());
                Reporting.AreEqual(quoteMotorcycle.Drivers.First().Details.FirstName, confirmationPage.FirstName, true, "Driver's First name");
                Reporting.Log("Policy purchase Confirmation screen:", browser.Driver.TakeSnapshot());
                browser.PercyScreenCheck(MotorcycleNewBusiness.ConfirmationPage, confirmationPage.GetPercyIgnoreCSS());
            }

            return policyNumber;
        }

        private static void VerifyAnyPremiumChangePopup(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var premiumChangePopup = new PremiumChangePopup(browser))
            {
                try
                {
                    premiumChangePopup.WaitForPage();
                    premiumChangePopup.VerifyPopupContent(quoteMotorcycle);
                    premiumChangePopup.VerifyPremiumChange(browser, quoteMotorcycle, SparkBasePage.QuoteStage.AFTER_PERSONAL_INFO);
                }
                catch
                {
                    Reporting.Error("Premium change pop up is expected on this scenario");
                }
            }
        }
    }
}
