using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using System.Collections.Generic;
using UIDriver.Pages;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;

namespace Spark.NewBusiness
{
    [Property("Functional", "Caravan tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class CaravanMiscellaneous : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Mandatory regression tests for Spark Caravan new business.");
        }

        #region Test Cases

        /// <summary>
        /// Verification of Spark 'Page 1: Before we get started' page content
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark)]
        [Test(Description = "Caravan: Page Verification: Page 1: Before we get started")]
        public void INSU_T319_Caravan_PageVerification_Before_We_Get_Started()
        {
            ActionsQuoteCaravan.PageVerificationBeforeWeGetStarted(_browser);

            Reporting.Log("Page Verification: Page 1: Before we get started", _browser.Driver.TakeSnapshot());
        }

        /// <summary>
        /// 26th Parallel knockout: Tests the scenario where RAC
        /// refuses to continue with the quote, if the parking suburb is
        /// above the 26th parallel and caravan parked OnSite.
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark)]
        [Test(Description = "Caravan: Knockout: Knockout when OnSite parking suburb is above the 26th Parallel")]
        public void INSU_T320_Caravan_Knockout_OnSite_Parking_Suburb_Above_26th_Parallel()
        {
            var quoteInputs = BuildTestDataKnockoutOnSiteParkingSuburbAbove26thParallel();

            LaunchPage.OpenSparkCaravanLandingPage(_browser);

            //Page 1: Before we get started
            ActionsQuoteCaravan.SelectRACMember(_browser, quoteInputs.PolicyHolders[0]);

            //Page 2: Let's start with your caravan
            ActionsQuoteCaravan.LetsStartWithYourCaravan(_browser, quoteInputs);
            ActionsQuoteCaravan.ProvideValueOfCaravan(_browser, quoteInputs);

            //Page 3: Tell us more about your <Caravan>
            ActionsQuoteCaravan.TellUsMoreAboutYourCaravan(_browser, quoteInputs);
        }

        /// <summary>
        /// Tests the following:
        /// Test 1: Page 1: Let's start with your caravan
        ///         Verify 'Value of caravan' prompt display when
        ///         RAC is not able to assess the value of the caravan
        /// Test 2: Verify error message displayed when value is below the allowed limit
        /// Test 3: Verify error message displayed when value is above the allowed limit
        /// Test 4: Allow user to proceed when a valid market value is entered
        /// Test 5: Page 3: Tell us more about your <Caravan>
        ///         Verify Business Or Commercial use knockout
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark)]
        [Test(Description = "Caravan: Value of caravan and Business Or Commercial use knockout")]
        public void Caravan_Value_Of_Caravan_And_Business_Or_Commercial_Use_Knockout()
        {
            var quoteInputs = BuildTestDataValueOfCaravanAndKnockoutBusinessOrCommUse();

            LaunchPage.OpenSparkCaravanLandingPage(_browser);

            //Page 1: Before we get started
            ActionsQuoteCaravan.SelectRACMember(_browser, quoteInputs.PolicyHolders[0]);

            //Page 2: Let's start with your caravan
            ActionsQuoteCaravan.LetsStartWithYourCaravan(_browser, quoteInputs);

            //Verify 'Value of caravan' prompt display and ability to enter only a valid value
            ActionsQuoteCaravan.ProvideValueOfCaravanValidAndInvalid(_browser, quoteInputs);

            //Page 3: Tell us more about your <Caravan>
            //Verify Business Or Commercial use knockout
            ActionsQuoteCaravan.TellUsMoreAboutYourCaravan(_browser, quoteInputs);
        }

        #endregion

        #region Test cases helper methods

        private QuoteCaravan BuildTestDataKnockoutOnSiteParkingSuburbAbove26thParallel()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true).Build();
            var caravan = new CaravanBuilder().InitialiseCaravanWithRandomData(new List<Contact>() { mainPH })
                .WithParkingSuburb("BROCKMAN", "6701")
                .WithParkLocation(CaravanParkLocation.OnSite)
                .Build();

            LogTestDataInToTestReport(caravan);

            return caravan;
        }

        private QuoteCaravan BuildTestDataValueOfCaravanAndKnockoutBusinessOrCommUse()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual().Build();
            var caravan = new CaravanBuilder().InitialiseCaravanWithRandomData(new List<Contact>() { mainPH })
                .WithRandomCaravan(0, CARAVAN_MIN_SUM_INSURED_VALUE-1)
                .WithAgreedSumInsured(CARAVAN_MIN_SUM_INSURED_VALUE)
                .WithUseOfBusinessOrCommercial(true)
                .Build();

            LogTestDataInToTestReport(caravan);

            return caravan;
        }

        /// <summary>
        /// Logging out test data for easier debugging and auditing.
        /// </summary>
        private static void LogTestDataInToTestReport(QuoteCaravan caravan)
        {
            foreach (var ph in caravan.PolicyHolders)
            {
                Reporting.Log(ph.ToString());
            }           
            Reporting.Log(caravan.ToString());
        }

        #endregion
    }
}
