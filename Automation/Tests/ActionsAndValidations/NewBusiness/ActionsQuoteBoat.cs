using Rac.TestAutomation.Common;
using UIDriver.Pages;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.BoatQuote;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.VisualTest;
using static Rac.TestAutomation.Common.Constants.General;

namespace Tests.ActionsAndValidations
{
    class ActionsQuoteBoat
    {
        /// <summary>
        /// Supports Spark Boat
        /// Will create a new boat quote on spark boat and set the quote number and premium values,
        /// into the test data object (under child property QuoteData),
        /// which will be visible to caller after this method exits.
        /// </summary>
        public static void CreateNewBoatQuote(Browser browser, QuoteBoat quote)
        {
            Reporting.Log("Begin input of values to build boat quote.");
        }
        /// <summary>
        /// Reusable generic steps used by boat quote to purchase tests.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void QuoteStepsEndToEnd(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false)
        {
            //Page 0: Important information
            if (detailedUiChecking)
            {
                ImportantInformationUniqueContent(browser);
            }    
            ContinueToLetsStart(browser);
            //Page 1: Let's start
            ArriveLetsStart(browser, detailedUiChecking);
            ContinueToAboutYou(browser);
            //Page 2: About you
            ArriveAboutYou(browser, quoteBoat, detailedUiChecking);
            ContinueToTypeOfBoat(browser);
            //Page 3: Boat type
            ArriveBoatType(browser, quoteBoat, detailedUiChecking);
            ContinueToYourBoat(browser);
            //Page 4: Your boat
            ArriveYourBoat(browser, quoteBoat, detailedUiChecking);
            ContinueToMoreBoat(browser);
            //Page 5: More boat
            ArriveMoreBoat(browser, quoteBoat, detailedUiChecking);
            ContinueToYourQuote(browser);
            //Page 6: Here's your quote
            ArriveYourQuote(browser, quoteBoat, detailedUiChecking);
            ContinueToStartDate(browser);
            //Page 7: Start date
            ArrivePolicyStartDate(browser, quoteBoat, detailedUiChecking);
            ContinueToYourDetails(browser, quoteBoat);
            //Page 8: Your details
            ArriveYourDetails(browser, detailedUiChecking);
            ProvidePersonalInformationMainPH(browser, quoteBoat);
            ContinueToYourRegistration(browser);
            //Page 9: Your registration
            ArriveYourRegistration(browser, quoteBoat, detailedUiChecking);
            ContinueToPayment(browser);
            //Page 10: Payment
            ArrivePayment(browser, quoteBoat, detailedUiChecking);
            ContinueToConfirmation(browser, quoteBoat);
            //Page 11: Confirmation
            ArriveConfirmation(browser, quoteBoat, detailedUiChecking);
        }

        /// <summary>
        /// Reusable generic steps used to generate a quote without finalising the purchase, for use in testing Retrieve Quote.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void QuoteWithoutPurchase(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false)
        {
            //Page 0: Important information
            if (detailedUiChecking)
            {
                ImportantInformationUniqueContent(browser);
            }
            ContinueToLetsStart(browser);
            //Page 1: Let's start
            ArriveLetsStart(browser, detailedUiChecking);
            ContinueToAboutYou(browser);
            //Page 2: About you
            ArriveAboutYou(browser, quoteBoat, detailedUiChecking);
            ContinueToTypeOfBoat(browser);
            //Page 3: Boat type
            ArriveBoatType(browser, quoteBoat, detailedUiChecking);
            ContinueToYourBoat(browser);
            //Page 4: Your boat
            ArriveYourBoat(browser, quoteBoat, detailedUiChecking);
            ContinueToMoreBoat(browser);
            //Page 5: More boat
            ArriveMoreBoat(browser, quoteBoat, detailedUiChecking);
            ContinueToYourQuote(browser);
            //Page 6: Here's your quote
            ArriveYourQuote(browser, quoteBoat, detailedUiChecking);
            ContinueToStartDate(browser);
            //Page 7: Start date
            ArrivePolicyStartDate(browser, quoteBoat, detailedUiChecking);
            ContinueToYourDetails(browser, quoteBoat);
            //Page 8: Your details
            ArriveYourDetails(browser, detailedUiChecking);
            ProvidePersonalInformationMainPH(browser, quoteBoat);
            ContinueToYourRegistration(browser);
            //Page 9: Your registration
            ArriveYourRegistration(browser, quoteBoat, detailedUiChecking);
            ContinueToPayment(browser);
        }
            /// <summary>
            /// Reusable generic steps used by boat Retrieve quote tests flowing through to purchase.
            /// </summary>
            /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
            public static void RetrieveQuoteStepsEndToEnd(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false, bool changesAfterRetrieve = false)
        {
            ContinueToYourRetrievedQuote(browser, quoteBoat);
            //Page 6: Here's your quote
            ArriveYourRetrievedQuote(browser, quoteBoat, detailedUiChecking, changesAfterRetrieve);
            ContinueToStartDate(browser);
            //Page 7: Start date
            ArrivePolicyStartDate(browser, quoteBoat, detailedUiChecking);
            ContinueToYourDetails(browser, quoteBoat);
            //Page 8: Your details
            ArriveYourDetails(browser, detailedUiChecking);
            ProvidePersonalInformationMainPH(browser, quoteBoat);
            ContinueToYourRegistration(browser);
            //Page 9: Your registration
            ArriveYourRegistration(browser, quoteBoat, detailedUiChecking);
            ContinueToPayment(browser);
            //Page 10: Payment
            ArrivePayment(browser, quoteBoat, detailedUiChecking);
            ContinueToConfirmation(browser, quoteBoat);
            //Page 11: Confirmation
            ArriveConfirmation(browser, quoteBoat, detailedUiChecking);
        }

        /// <summary>
        /// Short run alternative to QuoteStepsEndToEnd for smoke testing, only checking the unique content 
        /// on the Important Information page.
        /// </summary>
        public static void SmokeTestUniqueContentOnly(Browser browser)
        {
            ActionsQuoteBoat.ImportantInformationUniqueContent(browser);
            //Page 1: Let's start
            ActionsQuoteBoat.ContinueToLetsStart(browser);
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Checks the unique content on the "Important information" preamble page.
        /// </summary>
        public static void ImportantInformationUniqueContent(Browser browser)
        {
            using (var importantInformationPage = new SparkQuoteImportantInformation(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: importantInformationPage);
                }
                Reporting.Log("Important information screen:", browser.Driver.TakeSnapshot());
                browser.PercyScreenCheck(BoatNewBusiness.ImportantInformation);
                importantInformationPage.VerifyImportantInformationPageContent();
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Checks the common Header and Footer elements of the 
        /// "Important information" preamble page including.
        /// </summary>
        public static void ImportantInformationHeaderAndFooterContent(Browser browser)
        {
            using (var importantInformationPage = new SparkQuoteImportantInformation(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: importantInformationPage);
                }
                Reporting.Log("Important information screen:", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Continue to the Let's start page of the Boat New Business flow.
        /// </summary>
        public static void ContinueToLetsStart(Browser browser)
        {
            using (var importantInformationPage = new SparkQuoteImportantInformation(browser))
            {
                importantInformationPage.ContinueToLetsStart();
                Reporting.LogMinorSectionHeading("Selecting 'Next' to navigate to 'Let's start' page.");
            }
            using (var boatLetsStart = new SparkBoatLetsStart(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: boatLetsStart);
                }
                boatLetsStart.WaitForPage();
                browser.PercyScreenCheck(BoatNewBusiness.LetsStart);
                Reporting.Log("Let's start screen:", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Checks the unique content on the "Let's start" page.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ArriveLetsStart(Browser browser, bool detailedUiChecking = false)
        {
            using (var letsStartPage = new SparkBoatLetsStart(browser))
            {
                if (detailedUiChecking)
                {
                    letsStartPage.VerifyPageContent();
                }
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Set appropriate values in "Let's start" page to allow the user to continue to the About you 
        /// page of the Boat New Business flow and then does so.
        /// </summary>
        public static void ContinueToAboutYou(Browser browser)
        {
            using (var boatLetsStart = new SparkBoatLetsStart(browser))
            {
                boatLetsStart.ContinueToAboutYou();
                Reporting.LogMinorSectionHeading("Selecting 'Next' to navigate to 'About you' page.");
            }
            using (var boatAboutYou = new SparkBoatAboutYou(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: boatAboutYou);
                }
                
                boatAboutYou.WaitForPage();
                browser.PercyScreenCheck(BoatNewBusiness.AboutYou);
                Reporting.Log("About you screen:", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Updates the "About you" page fields. If detailedUiChecking flag = true 
        /// then it checks the page content as well.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ArriveAboutYou(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false)
        {
            using (var aboutYouPage = new SparkBoatAboutYou(browser))
            {
                if (detailedUiChecking)
                {
                    aboutYouPage.VerifyPageContent();
                }
                aboutYouPage.EnterAboutYouDetails(browser, quoteBoat);
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Continue to the "Type of boat" page of the Boat New Business flow.
        /// </summary>
        public static void ContinueToTypeOfBoat(Browser browser)
        {
            using (var boatAboutYou = new SparkBoatAboutYou(browser))
            {
                boatAboutYou.ContinueToTypeOfBoat();
                Reporting.LogMinorSectionHeading("Selecting 'Next' to navigate to 'Boat type' page.");
            }
            using (var boatTypeOfBoat = new SparkBoatTypeOfBoat(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: boatTypeOfBoat);
                }

                boatTypeOfBoat.WaitForPage();
                browser.PercyScreenCheck(BoatNewBusiness.BoatType);
                Reporting.Log("Boat type screen:", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Selects a boat type from the "Type of boat" page. If detailedUiChecking flag = true 
        /// then it checks the page content as well.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ArriveBoatType(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false)
        {
            using (var boatTypeOfBoat = new SparkBoatTypeOfBoat(browser))
            {
                if (detailedUiChecking)
                {
                    boatTypeOfBoat.VerifyPageContent();
                }
                boatTypeOfBoat.SelectBoatType(quoteBoat);
            }
        }
        
        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Continue to the "Your boat" page of the Boat New Business flow.
        /// </summary>
        public static void ContinueToYourBoat(Browser browser)
        {
            using (var boatType = new SparkBoatTypeOfBoat(browser))
            {
                boatType.ContinueToYourBoat();
                Reporting.LogMinorSectionHeading("Selecting 'Next' to navigate to 'Your boat' page.");
            }
            using (var boatYourBoat = new SparkBoatYourBoat(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: boatYourBoat);
                }
                boatYourBoat.WaitForPage();
                browser.PercyScreenCheck(BoatNewBusiness.YourBoat);
                Reporting.Log("Your boat screen:", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Updates the "Your boat" page fields. If detailedUiChecking flag = true 
        /// then it checks the page content as well.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ArriveYourBoat(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false)
        {
            using (var yourBoatPage = new SparkBoatYourBoat(browser))
            {
                if (detailedUiChecking)
                {
                    yourBoatPage.VerifyPageContent(quoteBoat.InsuredAmount);
                }
                yourBoatPage.PopulateFields(quoteBoat);
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Continue to the "More boat" page of the Boat New Business flow.
        /// </summary>
        public static void ContinueToMoreBoat(Browser browser)
        {
            using (var boatYourBoat = new SparkBoatYourBoat(browser))
            {
                boatYourBoat.ContinueToMoreBoat();
                Reporting.LogMinorSectionHeading("Selecting 'Next' to navigate to 'More boat' page.");
            }
            using (var boatMoreBoat = new SparkBoatMoreBoat(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: boatMoreBoat);
                }
                boatMoreBoat.WaitForPage();
                browser.PercyScreenCheck(BoatNewBusiness.MoreBoat);
                Reporting.Log("More boat screen:", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Updates the "More boat" page fields. If detailedUiChecking flag = true 
        /// then it checks the page content as well.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ArriveMoreBoat(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false)
        {
            using (var moreBoatPage = new SparkBoatMoreBoat(browser))
            {
                if (detailedUiChecking)
                {
                    moreBoatPage.VerifyPageContent();
                }
                moreBoatPage.PopulateFields(browser, quoteBoat);
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Continue to the "Your quote" page of the Boat New Business flow.
        /// </summary>
        public static void ContinueToYourQuote(Browser browser)
        {
            using (var boatMoreBoat = new SparkBoatMoreBoat(browser))
            {
                boatMoreBoat.ContinueToYourQuote();
                Reporting.LogMinorSectionHeading("Selecting 'View quote' to navigate to 'Here's your quote' page.");
            }
            using (var boatYourQuote = new SparkBoatYourQuote(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: boatYourQuote);
                }
                boatYourQuote.WaitForPage();
                boatYourQuote.CloseFormotivPopupIfDisplayed();
                browser.PercyScreenCheck(BoatNewBusiness.YourQuote);
                Reporting.Log("Here's your quote screen:", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Updates the "Your quote" page fields. If detailedUiChecking flag = true 
        /// then it checks the page content as well.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ArriveYourQuote(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false)
        {
            using (var yourQuotePage = new SparkBoatYourQuote(browser))
            {
                yourQuotePage.YourQuoteComparisonDetails(quoteBoat);
                yourQuotePage.VerifyAgreedValueMatch(browser, quoteBoat.InsuredAmount);
                if (detailedUiChecking)
                {
                    yourQuotePage.VerifyPageContent(quoteBoat);
                }
                yourQuotePage.UpdatePageFields(quoteBoat);
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Continue to the in 'Great, let's set a start date.' page 
        /// of the Boat New Business flow.
        /// </summary>
        public static void ContinueToStartDate(Browser browser)
        {
            using (var boatYourQuote = new SparkBoatYourQuote(browser))
            {
                boatYourQuote.ContinueToStartDate();
                Reporting.LogMinorSectionHeading("Selecting 'Next' to navigate to 'Start date' page.");
            }
            using (var boatStartDate = new SparkBoatStartDate(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: boatStartDate);
                }
                boatStartDate.WaitForPage();
                browser.PercyScreenCheck(BoatNewBusiness.StartDate);
                Reporting.Log("Start date screen:", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Supports Spark Boat
        /// Answers 'When would you like your policy to start?' question
        /// in 'Great, let's set a start date.' page.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ArrivePolicyStartDate(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false)
        {
            using (var greatLetsSetAStartDate = new SparkBoatStartDate(browser))
            {
                if (detailedUiChecking)
                {
                    greatLetsSetAStartDate.VerifyPageContent();
                }
                greatLetsSetAStartDate.SetPolicyStartDate = quoteBoat.PolicyStartDate;
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Continue to the "Your details" page of the Boat New Business flow.
        /// </summary>
        public static void ContinueToYourDetails(Browser browser, QuoteBoat quoteBoat)
        {
            using (var boatStartDate = new SparkBoatStartDate(browser))
            {
                boatStartDate.ContinueToYourDetails(quoteBoat);
                Reporting.LogMinorSectionHeading("Selecting 'Next' to navigate to 'Your details' page.");
            }
            using (var boatYourDetails = new SparkBoatYourDetails(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: boatYourDetails);
                }
                boatYourDetails.WaitForPage();
                browser.PercyScreenCheck(BoatNewBusiness.YourDetails);
                Reporting.Log("Your details screen:", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// If detailedUiChecking flag = true checks the page content 
        /// of the "Your details" page.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ArriveYourDetails(Browser browser, bool detailedUiChecking = false)
        {
            using (var sparkBoatYourDetails = new SparkBoatYourDetails(browser))
            {
                if (detailedUiChecking)
                {
                    sparkBoatYourDetails.VerifyPageContent();
                }
            }
        }

        /// <summary>
        /// Supports Spark Boat
        /// Answers personal information questions in 'Your details' page for the Policyholder.
        /// </summary>
        public static void ProvidePersonalInformationMainPH(Browser browser, QuoteBoat quoteBoat)
        {
            using (var sparkBoatYourDetails = new SparkBoatYourDetails(browser))
            { 
                var mainPH = quoteBoat.CandidatePolicyHolders[0];
                sparkBoatYourDetails.FillPersonalInformation(mainPH);
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Continue to the "Your registration" page of the Boat New Business flow.
        /// </summary>
        public static void ContinueToYourRegistration(Browser browser)
        {
            using (var boatYourDetails = new SparkBoatYourDetails(browser))
            {
                boatYourDetails.ContinueToYourRegistration();
                Reporting.LogMinorSectionHeading("Selecting 'Next' to navigate to 'Your Registration' page.");
            }
            using (var boatYourRegistration = new SparkBoatYourRegistration(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: boatYourRegistration);
                }
                boatYourRegistration.WaitForPage();
                browser.PercyScreenCheck(BoatNewBusiness.YourRegistration);
                Reporting.Log("Your Registration screen:", browser.Driver.TakeSnapshot());
            }
        }
        
        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Updates the "Your registration" page fields. If detailedUiChecking flag = true 
        /// then it checks the page content as well.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ArriveYourRegistration(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false)
        {
            using (var sparkBoatYourRegistration = new SparkBoatYourRegistration(browser))
            {
                if (detailedUiChecking)
                {
                    sparkBoatYourRegistration.VerifyPageContent();
                }
                sparkBoatYourRegistration.InputRegistrationInformation(browser, quoteBoat);
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Continue to the "Payment" page of the Boat New Business flow.
        /// </summary>
        public static void ContinueToPayment(Browser browser)
        {
            using (var boatYourRegistration = new SparkBoatYourRegistration(browser))
            {
                boatYourRegistration.ContinueToPayment();
                Reporting.LogMinorSectionHeading("Selecting 'Next' to navigate to the 'Payment' page.");
            }
            using (var boatPayment = new SparkBoatPayment(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: boatPayment);
                }
                boatPayment.WaitForPage();
                browser.PercyScreenCheck(BoatNewBusiness.Payment);
                Reporting.Log("Payment screen:", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Updates the "Payment" page fields. If detailedUiChecking flag = true 
        /// then it checks the page content as well.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ArrivePayment(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false)
        {          
            using (var sparkBoatPayment = new SparkBoatPayment(browser))
            {
                if (detailedUiChecking)
                {
                    sparkBoatPayment.CheckPaymentFieldValidation(browser);
                    sparkBoatPayment.VerifyStandardHeaderAndFooterContent();
                }
                sparkBoatPayment.PremiumCheck(quoteBoat);
                sparkBoatPayment.CheckSummaryContent(browser, quoteBoat);
                using (var sparkBoatQuote = new SparkBoatYourQuote(browser))
                {
                    SparkBoatYourQuote.VerifyQuoteDetailsInShield(quoteBoat, isYourQuoteScreenUpdated: true, isStartDateProvided: true, isPersonalInfoProvided: true, isRegistrationProvided: true);
                }
                sparkBoatPayment.InputPaymentInformation(browser, quoteBoat, detailedUiChecking);
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Continue to the "Confirmation" page of the Boat New Business flow.
        /// </summary>
        public static void ContinueToConfirmation(Browser browser, QuoteBoat quoteBoat)
        {
            using (var sparkBoatPayment = new SparkBoatPayment(browser))
            {
                Reporting.LogMinorSectionHeading("Selected 'Purchase policy' to navigate to the 'Confirmation' page.");
            }
            using (var boatConfirmation = new SparkBoatConfirmation(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(WaitTimes.T150SEC, nextPage: boatConfirmation);
                }
                boatConfirmation.WaitForPage();
                if(!quoteBoat.PayMethod.IsPaymentByBankAccount && quoteBoat.PayMethod.IsAnnual)
                {
                    browser.PercyScreenCheck(BoatNewBusiness.ConfirmationAnnualCash, boatConfirmation.GetPercyIgnoreCSS());
                }
                else
                {
                    browser.PercyScreenCheck(BoatNewBusiness.ConfirmationDirectDebit, boatConfirmation.GetPercyIgnoreCSS());
                }
                
                Reporting.Log("Policy purchase Confirmation screen:", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Updates the "Confirmation" page fields. If detailedUiChecking flag = true 
        /// then it checks the page content as well.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ArriveConfirmation(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false)
        {
            using (var boatConfirmation = new SparkBoatConfirmation(browser))
            {
                if (detailedUiChecking)
                {
                    boatConfirmation.VerifyPageContent(browser, quoteBoat);
                }
                boatConfirmation.CapturePolicyNumber(browser, quoteBoat);
                boatConfirmation.VerifyPolicyDetailsInShield(browser, quoteBoat);
            }
        }
        
        /// <summary>
        /// Arrive at the "Retrieve quote" page and update the fields to progress to Here's your quote' page.
        /// </summary>
        public static void ContinueToYourRetrievedQuote(Browser browser, QuoteBoat quoteBoat)
        {
            using (var boatRetrieveQuote = new SparkBoatRetrieveQuote(browser))
            {
                Reporting.LogMinorSectionHeading("Retrieving Quote");
                boatRetrieveQuote.VerifyHeaderAndFooterContent();
                boatRetrieveQuote.VerifyPageContent();
                boatRetrieveQuote.InputQuoteDetails(quoteBoat);
            }
            using (var boatYourQuote = new SparkBoatYourQuote(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish(nextPage: boatYourQuote);
                }
                boatYourQuote.WaitForPage();
                boatYourQuote.CloseFormotivPopupIfDisplayed();
                browser.PercyScreenCheck(BoatNewBusiness.YourQuote);
                Reporting.Log("Here's your quote screen:", browser.Driver.TakeSnapshot());
            }
        }
        
        /// <summary>
        /// Applicable only for the Spark version of B2C
        /// Arrive at "Your quote" page after retrieving a quote. 
        /// If detailedUiChecking flag = true then will provide detailed reporting of UI.
        /// If changeAfterRetrieve = true then generates/sets a new value for Excess and Agreed Value.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        /// <param name="changeAfterRetrieve">Optional parameter, if set to true will alter some values during the flow to purchase.</param>
        public static void ArriveYourRetrievedQuote(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false, bool changeAfterRetrieve = false)
        {
            using (var yourQuotePage = new SparkBoatYourQuote(browser))
            {
                yourQuotePage.YourQuoteNumber(quoteBoat);
                yourQuotePage.PremiumBreakdown(quoteBoat);
            Reporting.Log($"About to invoke SparkBoatYourQuote.VerifyQuoteDetailsInShield(quoteBoat, isStartDateProvided: true, isPersonalInfoProvided: true, isRegistrationProvided: true);");
                SparkBoatYourQuote.VerifyQuoteDetailsInShield(quoteBoat, isYourQuoteScreenUpdated: true, isStartDateProvided: true, isPersonalInfoProvided: true, isRegistrationProvided: true);

                yourQuotePage.VerifyAgreedValueMatch(browser, quoteBoat.InsuredAmount);
                if (detailedUiChecking)
                {
                    yourQuotePage.VerifyPageContent(quoteBoat);
                }
                if (changeAfterRetrieve)
                {
                    Reporting.LogMinorSectionHeading("Generating new values for input before purchase");
                    quoteBoat.BasicExcess = DataHelper.RandomNumber(0, 21) * 100;
                    Reporting.Log($"New Basic Excess value = {quoteBoat.BasicExcess}");
                    quoteBoat.InsuredAmount = DataHelper.RandomNumber(3000, BOAT_MAXIMUM_INSURED_VALUE_ONLINE);
                    Reporting.Log($"New Insured Amount value = {quoteBoat.InsuredAmount}");
                    quoteBoat.BoatRego = null;
                    Reporting.Log($"New Boat Registration value = {quoteBoat.BoatRego}");
                    quoteBoat.BoatTrailerRego = DataHelper.RandomAlphanumerics(8, 10);
                    Reporting.Log($"New Trailer Registration value = {quoteBoat.BoatTrailerRego}");

                    Reporting.Log($"Setting Agreed Value = {quoteBoat.InsuredAmount}");
                    yourQuotePage.Value = quoteBoat.InsuredAmount.ToString();
                    using (var spinner = new SparkSpinner(browser))
                    {
                        spinner.WaitForSpinnerToFinish();
                    }
                    yourQuotePage.UpdatePageFields(quoteBoat);
                }
            }
        }
    }
}
