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
    public class BoatRetrieveQuote : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Mandatory regression tests for Boat new business implemented in Spark.");
        }

        #region Test Cases
        /// <summary>
        /// This test case is similar to INSU_T196 but is able to select either Boat Type as it isn't concerned with displaying
        /// every field and retrieves the quote, changing some values before purchasing.
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Boat), Category(TestCategory.Spark), Category(TestCategory.Regression)]
        [Test(Description = "Boat: Open Application and proceed through to payment screen. Close the quote instead of completing the purchase." +
            "Then retrieve the quote and make some changes to the quote details and complete the purchase.")]
        public void INSU_T199_Boat_Retrieve_Quote_Make_Changes_Before_Purchase()
        {
            var quoteBoat = BuildTestDataRandomIndividualsForRetrieveBoatQuote();

            Reporting.LogTestStart();
            LaunchPage.OpenSparkBoatLandingPageAndSetConfig(_browser);

            ActionsQuoteBoat.QuoteWithoutPurchase(_browser, quoteBoat);

            var quoteData = quoteBoat.QuoteData;
            LaunchPage.OpenSparkBoatRetrieveQuoteAndSetConfig(_browser);
            Reporting.Log($"quoteData.QuoteNumber = {quoteData.QuoteNumber}");

            ActionsQuoteBoat.RetrieveQuoteStepsEndToEnd(_browser, quoteBoat, detailedUiChecking: false, changesAfterRetrieve: true);
        }

        /// <summary>
        /// This test case is similar to INSU_T196 but is able to select either Boat Type as it isn't concerned with displaying
        /// every field and retrieves the quote, not changing any values before purchasing.
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Boat), Category(TestCategory.Spark), Category(TestCategory.Regression)]
        [Test(Description = "Boat: Open Application and proceed through to payment screen. Close the quote instead of completing the purchase." +
            "Then complete the purchase of the retrieved quote without making any changes to the quote details.")]
        public void INSU_T200_Boat_Retrieve_Quote_No_Changes_Before_Purchase()
        {
            var quoteBoat = BuildTestDataRandomIndividualsForRetrieveBoatQuote();

            Reporting.LogTestStart();
            LaunchPage.OpenSparkBoatLandingPageAndSetConfig(_browser);

            ActionsQuoteBoat.QuoteWithoutPurchase(_browser, quoteBoat);

            var quoteData = quoteBoat.QuoteData;
            LaunchPage.OpenSparkBoatRetrieveQuoteAndSetConfig(_browser);
            Reporting.Log($"quoteData.QuoteNumber = {quoteData.QuoteNumber}");

            ActionsQuoteBoat.RetrieveQuoteStepsEndToEnd(_browser, quoteBoat, detailedUiChecking: false, changesAfterRetrieve: false);
        }
        #endregion

        #region Test cases helper methods
        public QuoteBoat BuildTestDataRandomIndividualsForRetrieveBoatQuote()
        {
            var candidatePolicyHolder = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true)
                                .WithRandomDateOfBirth(MIN_PH_AGE_VEHICLES, MAX_PH_AGE)
                                .Build();
            var boat = new BoatBuilder().InitialiseBoatWithRandomData(new List<Contact>() { candidatePolicyHolder })
                                .WithClaimsHistory(0) //While claim disclosure remains out of scope for automation
                                .WithSkippersTicketHeld(SkippersTicketYearsHeld.Noskippersticket)
                                .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, boat.ToString());

            return boat;
        }
        #endregion
    }
}
