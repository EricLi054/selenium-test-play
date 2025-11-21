using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using System;
using UIDriver.Helpers;
using UIDriver.Pages.B2C;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace B2C.NewBusiness
{
    [TestFixture]
    [Property("Functional", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class Motor : BaseUITest
    {
        #region Constants
        private const string CALLBACK_REQUEST_MESSAGE = "A call back request has been successfully submitted.";
        #endregion
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C mandatory regression tests for motor anonymous new business.");
        }

        /// <summary>
        /// Automated version of mandatory regression test:
        /// "B2C Anonymous - Motor Policy (PRR) - TC02 - Comprehensive Scenario 2"
        /// 
        /// Test emulates the workflow of a anonymous user attempting to obtain
        /// a Motor Quote via B2C, but being requested to a RAC callback due to
        /// the value of their vehicle.
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Motor), Category(TestCategory.New_Business), Category(TestCategory.B2CPCM)]
        public void INSU_T248_Anonymous_MotorPolicy_ComprehensiveScenario2()
        {
            // Setup test data
            var vehicleQuote = BuildTestDataAnonymousMotorPolicyTC02ComprehensiveScenario2();

            ActionsQuoteMotor.OpenBaseB2CLandingPage(browser: _browser);

            /***********************************************************
             * Complete Page 1 details
             ***********************************************************/
            using (var quotePage1 = new MotorQuote1Details(_browser))
            using (var spinner = new RACSpinner(_browser))
            using (var callbackPage = new QuoteCallback(_browser))
            {
                quotePage1.FillQuoteDetails(vehicleQuote);

                spinner.WaitForSpinnerToFinish(nextPage: callbackPage);

                Reporting.Log("Expecting call back prompt.");
                callbackPage.FirstName            = vehicleQuote.Drivers[0].Details.FirstName;
                callbackPage.LastName             = vehicleQuote.Drivers[0].Details.Surname;
                callbackPage.PhoneNumber          = vehicleQuote.Drivers[0].Details.GetPhone();
                callbackPage.PreferredContactTime = "Morning";
                Reporting.Log("Capture state of call back prompt immediately before selecting Continue", _browser.Driver.TakeSnapshot());
                callbackPage.ClickContinueButton();
            }

            using (var confirmation = new CallbackConfirmation(_browser))
            {
                Reporting.Log("Expecting confirmation of callback details received by system.");
                confirmation.WaitForPage();
                Reporting.AreEqual(CALLBACK_REQUEST_MESSAGE, confirmation.NoticeText, "Checking notice text");
                Reporting.Log("Capturing page state after validating notice text", _browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Automated version of mandatory regression test:
        /// "B2C Anonymous - Motor Policy - TC01 - Comprehensive cover with Roadside Assistance".
        /// 
        /// Covers scenario of a new full comprehensive policy being purchased with
        /// additional roadside assistance membership. 3 policyholders are part of
        /// the policy.
        /// Bank Account - Annual
        /// Some policyholders attached to policy will have maximum length names with special characters.
        /// </summary>        
        [Test, Category(TestCategory.Regression), Category(TestCategory.Motor), Category(TestCategory.New_Business), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.B2CPCM), Category(TestCategory.SparkB2CRegressionForMemberCentralReleases)]
        public void INSU_T252_Anonymous_MotorPolicy_ComprehensivePolicyIssueWithRoadside()
        {
            // Setup test data
            var vehicleQuote = BuildTestDataAnonymousMotorPolicyTC01ComprehensiveWithRoadside();

            string tPolicyNumber = ActionsQuoteMotor.PurchaseMotorVehiclePolicy(_browser, vehicleQuote);

            // Verify policy against Shield
            VerifyQuoteMotor.VerifyMotorVehiclePolicyInShield(vehicleQuote, tPolicyNumber);
        }

        /// <summary>
        /// Automated version of mandatory regression test:
        /// "B2C Anonymous - Motor Policy - TC03 - TFT Scenario 1"
        /// 
        /// Test emulates the workflow of an anonymous user getting a Third Party
        /// Fire & Theft coverage for a vehicle where the main driver is high risk
        /// but not a PH, however an additional driver is present who is low risk
        /// and also the policy holder.
        /// Credit Card - Monthly
        /// Policy should be able to be successfully purchased.
        /// </summary>        
        [Test, Category(TestCategory.Regression), Category(TestCategory.Motor), Category(TestCategory.New_Business), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.B2CPCM)]
        public void INSU_T249_Anonymous_MotorPolicy_TFT_Scenario_1()
        {
            /***********************************************************
             * Setup test data
             ***********************************************************/
            var vehicleQuote = BuildTestDataAnonymousMotorPolicyTC03TFTScenario1();

            string tPolicyNumber = ActionsQuoteMotor.PurchaseMotorVehiclePolicy(_browser, vehicleQuote);

            // Verify policy against Shield
            VerifyQuoteMotor.VerifyMotorVehiclePolicyInShield(vehicleQuote, tPolicyNumber);
        }

        /// <summary>
        /// Automated version of mandatory regression test:
        /// "B2C Anonymous - Motor Policy - TC06 - TPO Scenario 2" - B2C-T865
        /// 
        /// Test emulates the workflow of an anonymous user purchasing Third
        /// Party Property only cover for a low value vehicle. Policy should
        /// be purchased successfully.
        /// Bank Account - Annual with BSB validation
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Motor), Category(TestCategory.New_Business), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.B2CPCM)]
        public void INSU_T250_Anonymous_MotorPolicy_TPO_Scenario_2()
        {
            // Setup test data
            var vehicleQuote = BuildTestDataAnonymousMotorPolicyTC06TPOScenario2();

            string tPolicyNumber = ActionsQuoteMotor.PurchaseMotorVehiclePolicy(_browser, vehicleQuote, detailUIChecking: true);

            // Verify policy against Shield
            VerifyQuoteMotor.VerifyMotorVehiclePolicyInShield(vehicleQuote, tPolicyNumber);
        }

        /// <summary>
        /// Regression test relating to B2C-2707 to verify special character and name
        /// character lengths in motor policies.
        /// </summary>        
        [Test, Category(TestCategory.Regression), Category(TestCategory.Motor), Category(TestCategory.New_Business), Category(TestCategory.B2CPCM)]
        public void INSU_T350_Anonymous_MotorPolicy_NameFields()
        {
            // Setup test data
            var vehicleQuote = BuildTestDataAnonymousMotorPolicyNameFields();

            decimal quotePrice; // Not used.

            ActionsQuoteMotor.OpenBaseB2CLandingPage(browser: _browser);

            ActionsQuoteMotor.SubmitInitialMotorQuoteRatingValues(browser: _browser, vehicleQuote: vehicleQuote);

            ActionsQuoteMotor.UpdateAndSubmitInitialQuotePage(browser: _browser, vehicleQuote: vehicleQuote, agreedQuotePrice: out quotePrice);

            /***********************************************************
             * Complete Page 3 details - check validations and end test
             ***********************************************************/
            _browser.MotorQuoteAddedDetailsPage3FieldLengthValidations(vehicleQuote);

        }

        /// <summary>
        /// Regression test relating to B2C-5308 that adds an option of "Unknown"
        /// under the "Number of years you have held your driver's licence" dropdown
        /// list for main driver.
        /// 
        /// To validate the full purchase flow of an anonymous user getting a comprehensive 
        /// motor policy for vehicle where the main driver is a policy holder with number of 
        /// years license held as "unknown" with payment option as credit card and frequency monthly.
        /// </summary>        
        [Test, Category(TestCategory.Regression), Category(TestCategory.Motor), Category(TestCategory.New_Business), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.B2CPCM)]
        public void INSU_T254_Anonymous_MotorPolicy_Comprehensive_SingleDriverWithUnknownLicenseTime()
        {
            // Setup test data
            var vehicleQuote = BuildTestDataAnonymousMotorPolicyComprehensiveUnknownMainDriverLicenseTime();

            string cPolicyNumber = ActionsQuoteMotor.PurchaseMotorVehiclePolicy(_browser, vehicleQuote);

            // Verify policy against Shield
            VerifyQuoteMotor.VerifyMotorVehiclePolicyInShield(vehicleQuote, cPolicyNumber);
        }

        private QuoteCar BuildTestDataAnonymousMotorPolicyTC02ComprehensiveScenario2()
        {
            // Setup test data
            var mainPH = new ContactBuilder().InitialiseRandomIndividual()
                                                        .WithRandomDateOfBirth(minAge: 25, maxAge: MAX_PH_AGE) // higher min age to reduce chance of triggering decline instead of callback
                                                        .Build();

            var parkingAddress = new Address()
            {
                StreetNumber = "45",
                StreetOrPOBox = "Kinross Dr",
                Suburb = "Kinross",
                PostCode = "6028"
            };
            var vehicle = new MotorCarBuilder().InitialiseMotorCarWithRandomData(mainPH, true)
                                             .WithRandomVehicle(minValue: MOTOR_COVER_MAX_INSURABLE_VALUE, maxValue: 500000)  // Need to be over MOTOR_COVER_MAX_INSURABLE_VALUE to trigger callback
                                             .WithParkingAddress(parkingAddress)
                                             .WithoutFinancier()
                                             .WithRandomDriverConvictions(0)     // Random convictions for main driver
                                             .WithRandomDriverAccidentHistory(0) // Random accident history for main driver
                                             .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, vehicle.ToString());

            return vehicle;
        }

        private QuoteCar BuildTestDataAnonymousMotorPolicyTC01ComprehensiveWithRoadside()
        {
            // Setup test data
            // Main driver must be at least 21 to be offered RSA.
            var mainPH = new ContactBuilder().InitialiseRandomIndividual()
                                         .WithRandomDateOfBirth(21, 80)
                                         // TODO: 
                                         // Ideally we should be checking all name fields can
                                         // support each of the Smart Punctuation characters
                                         // present in the First Name here.
                                         // However this would need to be spread across multiple
                                         // tests as currently we trigger an exception in the WAF
                                         // around SQL Injection.
                                         // This change allows some coverage while also avoiding
                                         // that exception.
                                         .WithFirstName("St-'an(da‘rd)—is`ed" + DataHelper.RandomLetters(26)) //Boundary test 45 character limit on First Name field
                                         .WithMiddleName("N-a—m(e`)s" + DataHelper.RandomLetters(40)) //Boundary test 50 character limit on Middle Name field
                                         .WithSurname("F‘i’elds" + DataHelper.RandomLetters(42)) //Boundary test 50 character limit on Middle Name field
                                         .Build();
            var parkingAddress = new Address()
            {
                StreetNumber = "57",
                StreetOrPOBox = "Bashford St",
                Suburb = "Jurien Bay",
                PostCode = "6516"
            };
            
            //Chief reason for leaving one of the PH without "Smart Punctuation" in the name
            //is because the B2C Account Holder Name field has not been updated to accept these
            //in-line with other Name fields. 
            //The Shield database field CN_CONTACT_BANK_ACCOUNT_RACI.CONTACT_BANK_ACCOUNT_NAME 
            //does support brackets, apostrophes and hyphens.
            var additionalPH = new ContactBuilder().InitialiseRandomIndividual().WithoutMailingAddress().Build(); 
            
            var tertiaryPH = new ContactBuilder().InitialiseRandomIndividual()
                                         .WithRandomDateOfBirth(21, 80)
                                         .WithFirstName(DataHelper.RandomLetters(26) + "St-'an(da‘rd)—’is`e") //Boundary test 45 character limit on First Name field
                                         .WithMiddleName(DataHelper.RandomLetters(41) + "N-a—m(e`)") //Boundary test 50 character limit on Middle Name field
                                         .WithSurname(DataHelper.RandomLetters(39) + "`F-i—e(l)ds") //Boundary test 50 character limit on Middle Name field
                                         .Build();

            var vehicle = new MotorCarBuilder().InitialiseMotorCarWithRandomData(mainPH, true)
                                             .WithRandomVehicle(minValue: 5000)  // To avoid getting low value vehicles that might trigger golden rule and prevent MFCO cover
                                             .WithDriver(additionalPH, true)
                                             .WithDriver(tertiaryPH, true)
                                             .WithUsage(VehicleUsage.Private) // To get RSA offer.
                                             .WithPurchaseRoadsideAssistanceMembershipBundle(true)
                                             .WithPolicyStartDate(DateTime.Now.AddDays(5))
                                             .WithCover(MotorCovers.MFCO) // The default is MFCO, but explicitly setting for clarity.
                                             .WithPaymentMethod(new Payment(additionalPH).CreditCard().Annual())
                                             .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, vehicle.ToString());

            return vehicle;
        }

        private QuoteCar BuildTestDataAnonymousMotorPolicyTC03TFTScenario1()
        {
            // Setup test data
            var mainDriver = new ContactBuilder().InitialiseRandomIndividual().Build();
            var secondDriverAndPH = new ContactBuilder().InitialiseRandomIndividual().WithRandomDateOfBirth(24,80).WithoutMailingAddress().Build();
            var quoteData = new MotorCarBuilder().InitialiseMotorCarWithRandomData(mainDriver, false)
                                             // TODO: INSU-286 Remove the check for "IsExcessChangesEnabled" in WithAdditionalDriverLicenseTime when the Excess&NCB toggle is removed from the UI
                                             .WithDriver(secondDriverAndPH, true)
                                             .WithLimitedValidRandomVehicleUsage() // as we are choosing TFT cover
                                             .WithPolicyStartDate(DateTime.Now.AddDays(29))
                                             .WithCover(MotorCovers.TFT)
                                             .WithPaymentMethod(new Payment(secondDriverAndPH).CreditCard().Monthly())
                                             .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, quoteData.ToString());

            return quoteData;
        }

        private QuoteCar BuildTestDataAnonymousMotorPolicyTC06TPOScenario2()
        {
            // Setup test data
            var address = new Address()
            {
                StreetOrPOBox = "PO Box 123",
                Suburb = "Palmyra",
                PostCode = "6957"
            };
            var mainPH = new ContactBuilder().InitialiseRandomIndividual()
                                         .WithMailingAddress(address)
                                         .WithMailingAddressAsTheOnlyPreferredDeliveryMethod()
                                         .Build();
            var parkingAddress = new Address()
            {
                StreetNumber = "57",
                StreetOrPOBox = "Bashford St",
                Suburb = "Jurien Bay",
                PostCode = "6516"
            };
            var vehicle = new MotorCarBuilder().InitialiseMotorCarWithRandomData(mainPH, true)
                                             .WithParkingAddress(parkingAddress)
                                             .WithCover(MotorCovers.TPO)
                                             .WithExcess("0")
                                             .WithPaymentMethod(new Payment(mainPH).BankAccount().Annual())
                                             .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, vehicle.ToString());

            return vehicle;
        }

        private QuoteCar BuildTestDataAnonymousMotorPolicyNameFields()
        {
            // Setup test data to trigger field validation errors 
            //NB: The FirstName/Surname limits are 5 less than Shield/MC as 5 characters are taken up by "_B2C_" prefix.
            var mainPH = new ContactBuilder().InitialiseRandomIndividual()
                                         .WithFirstName("Standardise" + DataHelper.RandomLetters(35)) //Exceed 45 character limit on First Name field
                                         .WithMiddleName("Name" + DataHelper.RandomLetters(47)) //Exceed 50 character limit on Middle Name field
                                         .WithSurname("Fields" + DataHelper.RandomLetters(45)) //Exceed 50 character limit on Middle Name field
                                         .Build();
            var vehicle = new MotorCarBuilder().InitialiseMotorCarWithRandomData(mainPH, true)
                                             .WithPolicyStartDate(DateTime.Now.AddDays(5))
                                             .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, vehicle.ToString());

            return vehicle;
        }

        private QuoteCar BuildTestDataAnonymousMotorPolicyComprehensiveUnknownMainDriverLicenseTime()
        {
            // Setup test data
            var mainPH = new ContactBuilder().InitialiseRandomIndividual()
                                             .Build();
         
            var vehicle = new MotorCarBuilder().InitialiseMotorCarWithRandomData(mainPH, true)
                                             .WithRandomVehicle()
                                             .WithMainDriverLicenseTime("Unknown")
                                             .WithCover(MotorCovers.MFCO)
                                             .WithPaymentMethod(new Payment(mainPH).CreditCard().Monthly())
                                             .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, vehicle.ToString());

            return vehicle;
        }
    }
}