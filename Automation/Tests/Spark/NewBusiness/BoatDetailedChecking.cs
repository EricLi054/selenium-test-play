using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using UIDriver.Pages;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace Spark.NewBusiness
{
    [Property("Functional", "Boat policy tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class BoatDetailedUiChecking : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Mandatory regression tests for Boat new business implemented in Spark.");
        }

        #region Test Cases
        /// <summary>
        /// This test case focuses on detailed validation of UI elements such as help text, decline cover content and field validation error 
        /// so other tests can ignore that level of detail.
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Boat), Category(TestCategory.Spark), Category(TestCategory.Regression)]
        [Test(Description = "Boat: Open Application and proceed through policy purchase flow checking details such as help text and field validation.")]
        public void INSU_T196_Boat_NewBusiness_UI_FieldValidation_WIP_Pseudo_Random_Test_Data()
        {
            var quoteBoat = BuildTestDataFieldValidationSemiRandomisedForBoat();

            Reporting.LogTestStart();
            LaunchPage.OpenSparkBoatLandingPageAndSetConfig(_browser);

            ActionsQuoteBoat.QuoteStepsEndToEnd(_browser, quoteBoat, detailedUiChecking: true);
        }
        #endregion

        #region Test cases helper methods

        private QuoteBoat BuildTestDataFieldValidationSemiRandomisedForBoat()
        {
            var candidatePolicyHolder = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true)
                                .WithRandomDateOfBirth(MIN_PH_AGE_VEHICLES, MAX_PH_AGE)
                                .Build();
            var boat = new BoatBuilder().InitialiseBoatWithRandomData(new List<Contact>() { candidatePolicyHolder })
                                .WithFinancier(UnlistedInputs.FinancierNotFound)
                                .WithType(Constants.PolicyBoat.SparkBoatTypeExternalCode.L) //Sailboat to ensure we get racing extension on Your Quote
                                .WithYearBuilt(DateTime.Now.Year)
                                .WithClaimsHistory(0) ////While claim disclosure remains out of scope for automation
                                .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, boat.ToString());

            return boat;
        }
        #endregion
    }
}
