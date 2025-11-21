using System.Collections.Generic;
using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Contacts;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using UIDriver.Pages;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;

namespace Spark.NewBusiness
{
    [Property("Functional", "Boat policy tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class BoatMemberMatch : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Mandatory regression tests for Boat new business implemented in Spark.");
        }

        #region Test Cases

        /// <summary>
        /// This test case is similar to INSU_T197 but the member information provided matches an existing record.
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Boat), Category(TestCategory.Spark), Category(TestCategory.Regression)]
        [Test(Description = "Boat: Make a policy purchase for an existing member, ignoring UI validation.")]
        public void INSU_T432_Boat_NewBusiness_ExistingMember_Pseudo_Random_Test_Data_NoFinance()
        {
            var quoteBoat = BuildTestDataExistingIndividualForBoatNoFinance();

            Reporting.LogTestStart();
            LaunchPage.OpenSparkBoatLandingPageAndSetConfig(_browser);

            ActionsQuoteBoat.QuoteStepsEndToEnd(_browser, quoteBoat);
        }

        /// <summary>
        /// This test case is similar to INSU_T198 but the member information provided matches an existing record.
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Boat), Category(TestCategory.Spark), Category(TestCategory.Regression)]
        [Test(Description = "Boat: Make a policy purchase for an existing member who has finance on the boat, ignoring UI validation.")]
        public void INSU_T431_Boat_NewBusiness_ExistingMember_Pseudo_Random_Test_Data_Existing_Financier()
        {
            var quoteBoat = BuildTestDataExistingIndividualForBoatWithFinance();

            Reporting.LogTestStart();
            LaunchPage.OpenSparkBoatLandingPageAndSetConfig(_browser);

            ActionsQuoteBoat.QuoteStepsEndToEnd(_browser, quoteBoat);
        }

        #endregion
        #region Test cases helper methods
        private QuoteBoat BuildTestDataExistingIndividualForBoatNoFinance()
        {
            var contactCandidate = ShieldContacts.FetchAContactWithRACMembershipTier(membershipTiers: new MembershipTier[] { MembershipTier.Gold,
                                                                                            MembershipTier.Silver,
                                                                                            MembershipTier.Bronze });
            var candidatePolicyHolder = new ContactBuilder(contactCandidate).Build();

            var builder = new BoatBuilder().InitialiseBoatWithRandomData(new List<Contact>() { candidatePolicyHolder })
                                .WithoutFinancier()
                                .WithClaimsHistory(0); 
            var boat = builder.Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, boat.ToString());
            return boat;
        }

        private QuoteBoat BuildTestDataExistingIndividualForBoatWithFinance()
        {
            var contactCandidate = ShieldContacts.FetchAContactWithRACMembershipTier(membershipTiers: new MembershipTier[] { MembershipTier.Gold,
                                                                                            MembershipTier.Silver,
                                                                                            MembershipTier.Bronze });
            var candidatePolicyHolder = new ContactBuilder(contactCandidate).Build();

            var builder = new BoatBuilder().InitialiseBoatWithRandomData(new List<Contact>() { candidatePolicyHolder })
                                .WithoutFinancier()
                                .WithClaimsHistory(0);
            var boat = builder.Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, boat.ToString());
            return boat;
        }
        #endregion
    }
}
