using System.Collections.Generic;
using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using UIDriver.Pages;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;

namespace Spark.NewBusiness
{
    [Property("Functional", "Boat policy tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class BoatAnonymous : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Mandatory regression tests for Boat new business implemented in Spark.");
        }

        #region Test Cases

        /// <summary>
        /// Test currently simply opens the Important information page of the Boat application and confirms 
        /// that the expected content is rendered and attempts to move on from the page.
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Boat), Category(TestCategory.Spark), Category(TestCategory.Smoke)]
        [Test(Description = "Boat: Open Application and check for Important Information page")]
        public void INSU_T314_Boat_Important_information()
        {
            var quoteBoat = BuildTestDataRandomIndividualsForBoat();

            Reporting.LogTestStart();
            LaunchPage.OpenSparkBoatLandingPageAndSetConfig(_browser);
            
            ActionsQuoteBoat.SmokeTestUniqueContentOnly(_browser);
        }

        /// <summary>
        /// Test opens the Important information page of the Boat application and confirms 
        /// that the expected content is rendered including Header and Footer content,
        /// then attempts to move on from the page.
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Boat), Category(TestCategory.Spark), Category(TestCategory.Smoke)]
        [Test(Description = "Boat: Open Application and check the for Important Information page")]
        public void INSU_T315_Boat_Footer_Links_Important_information()
        {
            Reporting.LogTestStart();
            LaunchPage.OpenSparkBoatLandingPageAndSetConfig(_browser);
            //Page 0: Important information
            ActionsQuoteBoat.ImportantInformationHeaderAndFooterContent(_browser);
            //Page 1: Let's start
        }


        /// <summary>
        /// This test case is similar to INSU_T196 but bypasses much of the detailed UI validation in favour of
        /// arriving at the Payment screen much faster.
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Boat), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.VisualTest)]
        [Test(Description = "Boat: Open Application and proceed with a policy purchase as far as we can go, ignoring UI validation.")]
        public void INSU_T197_Boat_NewBusiness_UI_MVP_WIP_Pseudo_Random_Test_Data_NoFinance()
        {
            var quoteBoat = BuildTestDataRandomIndividualsForBoatNoFinance();

            Reporting.LogTestStart();
            LaunchPage.OpenSparkBoatLandingPageAndSetConfig(_browser);

            ActionsQuoteBoat.QuoteStepsEndToEnd(_browser, quoteBoat);
        }

        /// <summary>
        /// This test case is similar to SPK_T1458 but is able to select either Boat Type as it isn't concerned with displaying
        /// every field.
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Boat), Category(TestCategory.Spark), Category(TestCategory.Regression)]
        [Test(Description = "Boat: Open Application and proceed with a policy purchase as far as we can go without limiting boat type.")]
        public void INSU_T198_Boat_NewBusiness_UI_WIP_Pseudo_Random_Test_Data_Existing_Financier()
        {
            var quoteBoat = BuildTestDataRandomIndividualsForBoat();

            Reporting.LogTestStart();
            LaunchPage.OpenSparkBoatLandingPageAndSetConfig(_browser);

            ActionsQuoteBoat.QuoteStepsEndToEnd(_browser, quoteBoat);
        }

 
        #endregion

        #region Test cases helper methods

        private QuoteBoat BuildTestDataRandomIndividualsForBoat()
        {
            var candidatePolicyHolder = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true)
                                .WithRandomDateOfBirth(MIN_PH_AGE_VEHICLES, MAX_PH_AGE)
                                .Build();
            var boat = new BoatBuilder().InitialiseBoatWithRandomData(new List<Contact>() { candidatePolicyHolder })
                                .WithClaimsHistory(0) //While claim disclosure remains out of scope for automation
                                .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, boat.ToString());

            return boat;
        }


        private QuoteBoat BuildTestDataRandomIndividualsForBoatNoFinance()
        {
            var candidatePolicyHolder = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true)
                                .WithRandomDateOfBirth(MIN_PH_AGE_VEHICLES, MAX_PH_AGE)
                                .Build();
            var builder = new BoatBuilder().InitialiseBoatWithRandomData(new List<Contact>() { candidatePolicyHolder })
                                .WithoutFinancier()
                                .WithClaimsHistory(0); //While claim disclosure remains out of scope for automation

            // Force sailboat to ensure we get racing extension on "Your Quote" for Percy test
            if (Config.Get().IsVisualTestingEnabled)
            { builder.WithType(SparkBoatTypeExternalCode.L); }
            var boat = builder.Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, boat.ToString());
            return boat;
        }
        #endregion
    }
}
