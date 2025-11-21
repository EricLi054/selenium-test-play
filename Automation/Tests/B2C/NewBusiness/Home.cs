using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using System;
using System.Collections.Generic;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace B2C.NewBusiness
{
    [Property("Functional", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class Home : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C mandatory regression tests for home anonymous new business.");
        }

        /// <summary>
        /// Automated version of mandatory regression test:
        /// "B2C Anonymous - Home Policy - TC01 - Building and Contents Owner Occupied"
        /// 
        /// Test emulates the workflow of a anonymous user purchasing a home
        /// quote for owner occupied, building and contents. With a specified
        /// personal valuable, and a second policyholder.
        /// Bank Account - Monthly with BSB validation
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Home), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.B2CPCM)]
        public void INSU_T14_NewHomePolicy_BuildingContentsOwnerOccupied()
        {
            // Setup test data
            var testData = BuildTestDataBuildingContentsOwnerOccupied();

            string policyNumber = ActionsQuoteHome.PurchaseHomePolicy(_browser, testData, detailUIChecking: true);

            // Verify Policy against Shield database
            VerifyQuoteHome.VerifyHomePolicyInShield(testData, policyNumber);
        }

        /// <summary>
        /// Test purchasing a new home quote where the address is at a cyclone
        /// risk location. This is a supplementary automation test, and verifies
        /// that the additional cyclone questionnaire responses are correctly
        /// sent to Shield from B2C.
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Home), Category(TestCategory.Westpac_Payment), Category(TestCategory.B2CPCM)]
        public void INSU_T306_NewHomePolicy_ContentsOnlyOwnerOccupiedWithCycloneSteps()
        {
            // Setup test data
            var testData = BuildTestDataOwnerOccupiedContentsOnlyWithCyclone();

            string policyNumber = ActionsQuoteHome.PurchaseHomePolicy(_browser, testData);

            // Verify Policy against Shield database
            VerifyQuoteHome.VerifyHomePolicyInShield(testData, policyNumber);
        }
        /// <summary>
        /// "B2C Anonymous - Home Policy - TC03 - Contents Only Renters"
        /// Emulates the workflow of a user purchasing a Limited Contents only
        /// policy as a tenant.
        /// Credit Card - Monthly
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Home), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.B2CPCM)]
        public void INSU_T15_NewHomePolicy_ContentsOnlyRenters()
        {
            // Setup test data
            var testData = BuildTestDataRentersContentsOnly();

            string policyNumber = ActionsQuoteHome.PurchaseHomePolicy(_browser, testData);

            // Verify Policy against Shield database
            VerifyQuoteHome.VerifyHomePolicyInShield(testData, policyNumber);
        }

        /// <summary>
        /// "B2C Anonymous - Home Policy - TC04 - Landlords Building & Contents"
        /// Emulates a user purchasing a landlord's home policy with
        /// building and contents cover.
        /// Bank Account- Annual
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Home), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.B2CPCM)]
        public void INSU_T16_NewHomePolicy_BuildingAndContentsLandlord()
        {
            // Setup test data
            var testData = BuildTestDataLandlordsBuildingAndContents();

            string policyNumber = ActionsQuoteHome.PurchaseHomePolicy(_browser, testData);
            // Verify Policy against Shield database
            VerifyQuoteHome.VerifyHomePolicyInShield(testData, policyNumber);
        }

        /// <summary>
        /// "B2C Anonymous - Retrieve - TC05 - Home Owner Occupied Building and Contents"
        /// Emulates a user retrieving a home quote. Verifies that premiums,
        /// added covers and insured amounts are correctly retrieved.
        /// 
        /// Completes purchase of policy after retrieval.
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Home), Category(TestCategory.B2CPCM)]
        public void INSU_T18_NewHomePolicy_RetrieveQuote_BuildingContentsOwnerOccupied()
        {
            // Setup test data
            var testData = BuildTestDataRetrieveQuoteBuildingContentsOwnerOccupied();

            ActionsQuoteHome.GetNewHomeQuote(_browser, testData, out QuoteData expectedQuoteValues);

            _browser.CloseBrowser();
            System.Threading.Thread.Sleep(5000);

            ActionsQuote.RetrieveB2CQuote(_browser, testData.PropertyAddress, expectedQuoteValues.QuoteNumber, ShieldProductType.HGP);
            testData.QuoteHasBeenRetrieved = true;

            VerifyQuoteHome.VerifyQuoteAfterRetrieve(_browser, testData, expectedQuoteValues);
            string policyNumber = ActionsQuoteHome.ProceedWithThePurchaseOfQuote(_browser, testData, expectedQuoteValues);

            // Verify Policy against Shield database
            VerifyQuoteHome.VerifyHomePolicyInShield(testData, policyNumber);
        }

        /// <summary>
        /// "B2C Anonymous - Home Policy - Owner Occupied - Building and Contents - Purchase Accidental Damage"
        /// Test emulates the workflow of a anonymous user purchasing a home
        /// quote for owner occupied, building and contents with Accidental Damage bundle and a second policyholder.
        /// Annual - Cash
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Home), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.B2CPCM)]
        public void INSU_T36_NewHomePolicy_OwnerOccupiedBuildingAndContentsWithAccidentalDamage()
        {
            // Setup test data
            var testData = BuildTestDataOwnerOccupiedBuildingAndContentsWithAccidentalDamage();

            string policyNumber = ActionsQuoteHome.PurchaseHomePolicy(_browser, testData);

            // Verify Policy against Shield database
            VerifyQuoteHome.VerifyHomePolicyInShield(testData, policyNumber);
        }

        private QuoteHome BuildTestDataBuildingContentsOwnerOccupied()
        {
            // Setup test data
            var firstPolicyHolder = new ContactBuilder().InitialiseRandomIndividual().Build();
            var secondPolicyHolder = new ContactBuilder().InitialiseRandomIndividual().WithMailingAddress(firstPolicyHolder.MailingAddress).Build();
            var testData = new HomeBuilder().InitialiseHomeWithRandomData(HomeOccupancy.OwnerOccupied,
                                                                          new List<Contact>() { firstPolicyHolder, secondPolicyHolder })
                                            .WithAlarmSystem(Alarm.NonMonitoredAlarm)
                                            .WithPolicyStartDate(DateTime.Now.AddDays(29).Date)
                                            .WithInsuranceHistory(HomePreviousInsuranceTime.Three, null)
                                            .WithAreDoorsSecured(true)
                                            .WithAreWindowsSecured(true)
                                            .WithIsUsedForShortStay(true)
                                            .WithRandomFinancier()
                                            .WithExcess("300", null)
                                            .AddSpecifiedPersonalValuable(SpecifiedValuables.JewelleryWatches, "Family Jewells", 2000)
                                            .WithPaymentMethod(new Payment(firstPolicyHolder).BankAccount().Monthly())
                                            .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            return testData;
        }
        /// <summary>
        /// Creating home quote case for a cyclone risk address.
        /// Address is hard coded.
        /// </summary>
        /// <returns></returns>
        private QuoteHome BuildTestDataOwnerOccupiedContentsOnlyWithCyclone()
        {
            // Setup test data
            // Cyclone address
            var propertyAddress = new Address(streetNumber: "1",
                                              streetName: "Hart Ct",
                                              suburb: "Mount Tarcoola",
                                              state: "WA",
                                              postcode: "6530");

            var firstPolicyHolder = new ContactBuilder().InitialiseRandomIndividual().Build();
            var testData = new HomeBuilder().InitialiseHomeWithRandomData(HomeOccupancy.OwnerOccupied,
                                                                          new List<Contact>() { firstPolicyHolder })
                                            .WithPropertyAddress(propertyAddress)
                                            .WithRandomYearBuiltBeforeGivenYear(1982)
                                            .WithCycloneResponses(isElevated: true, hasShutters: true,
                                                                   garageDoorStatus: GarageDoorsUpgradeStatus.BracingUpgrade,
                                                                   roofStatus: RoofImprovementStatus.TiedownUpgrade)
                                            .WithoutBuildingSumInsured()
                                            .WithoutFinancier()
                                            .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            return testData;
        }

        private QuoteHome BuildTestDataRentersContentsOnly()
        {
            // Setup test data
            var firstPolicyHolder = new ContactBuilder().InitialiseRandomIndividual().Build();
            var testData = new HomeBuilder().InitialiseHomeWithRandomData(HomeOccupancy.Tenant,
                                                                          new List<Contact>() { firstPolicyHolder })
                                            .WithPropertyAddress(firstPolicyHolder.MailingAddress)
                                            .WithAlarmSystem(Alarm.NoAlarm)
                                            .WithAreDoorsSecured(false)
                                            .WithAreWindowsSecured(true)
                                            .WithoutBuildingSumInsured()
                                            .WithRandomRentersContentsSumInsured()
                                            .WithInsuranceHistory(HomePreviousInsuranceTime.FivePlus, null)
                                            .WithoutFinancier()
                                            .AddSpecifiedPersonalValuable(SpecifiedValuables.JewelleryWatches, "Family Jewells", 2000)
                                            .WithPaymentMethod(new Payment(firstPolicyHolder).CreditCard().Monthly())
                                            .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            return testData;
        }

        private QuoteHome BuildTestDataLandlordsBuildingAndContents()
        {
            // Setup test data
            var firstPolicyHolder = new ContactBuilder().InitialiseRandomIndividual().Build();
            var secondPolicyHolder = new ContactBuilder().InitialiseRandomIndividual().WithMailingAddress(firstPolicyHolder.MailingAddress).Build();
            var testData = new HomeBuilder().InitialiseHomeWithRandomData(HomeOccupancy.InvestmentProperty,
                                                                          new List<Contact>() { firstPolicyHolder, secondPolicyHolder })
                                            .WithPropertyAddress(firstPolicyHolder.MailingAddress)
                                            .WithAlarmSystem(Alarm.NonMonitoredAlarm)
                                            .WithAreDoorsSecured(true)
                                            .WithAreWindowsSecured(false)
                                            .WithRandomRentalRate()
                                            .WithRandomPropertyManager()
                                            .WithRandomBuildingSumInsured(200000, 1000000)
                                            .WithRandomContentsSumInsured(15000, 100000)
                                            .WithInsuranceHistory(HomePreviousInsuranceTime.LessThan1, null)
                                            .WithoutFinancier()
                                            .WithPaymentMethod(new Payment(firstPolicyHolder).BankAccount().Annual())
                                            .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            return testData;
        }

        private QuoteHome BuildTestDataRetrieveQuoteBuildingContentsOwnerOccupied()
        {
            // Setup test data
            var firstPolicyHolder = new ContactBuilder().InitialiseRandomIndividual()
                                             .WithMembership(MembershipTier.Blue, "12345")
                                             .Build();
            var testData = new HomeBuilder().InitialiseHomeWithRandomData(HomeOccupancy.OwnerOccupied,
                                                                          new List<Contact>() { firstPolicyHolder })
                                            .WithAlarmSystem(Alarm.RACMonitoredAlarm)
                                            .WithInsuranceHistory(HomePreviousInsuranceTime.Zero, null)
                                            .WithAreDoorsSecured(true)
                                            .WithAreWindowsSecured(true)
                                            .WithIsUsedForShortStay(false)
                                            .WithRandomUnspecifiedPersonalValuablesCover()
                                            .AddSpecifiedContentsItem(SpecifiedContents.AudioVisualCollections, "Surveillance Cameras", 200)
                                            .AddSpecifiedContentsItem(SpecifiedContents.ArtAntiquesFurs, "Paintings", 2000)
                                            .WithPaymentMethod(new Payment(firstPolicyHolder).CreditCard().Annual())
                                            .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            return testData;
        }

        private QuoteHome BuildTestDataOwnerOccupiedBuildingAndContentsWithAccidentalDamage()
        {
            // Setup test data
            var firstPolicyHolder   = new ContactBuilder().InitialiseRandomIndividual().Build();
            var secondPolicyHolder     = new ContactBuilder().InitialiseRandomIndividual().WithMailingAddress(firstPolicyHolder.MailingAddress).Build();
            var testData = new HomeBuilder().InitialiseHomeWithRandomData(HomeOccupancy.OwnerOccupied,
                                                                          new List<Contact>() { firstPolicyHolder, secondPolicyHolder })
                                            .WithAlarmSystem(Alarm.NonMonitoredAlarm)
                                            .WithPolicyStartDate(DateTime.Now.AddDays(29).Date)
                                            .WithInsuranceHistory(HomePreviousInsuranceTime.Four, null)
                                            .WithAreDoorsSecured(true)
                                            .WithAreWindowsSecured(true)
                                            .WithIsUsedForShortStay(true)
                                            .WithRandomFinancier()
                                            .WithExcess(null, null)
                                            .WithPaymentMethod(new Payment(firstPolicyHolder).CreditCard().Annual())
                                            .WithPurchaseAccidentalDamageBundle(true)
                                            .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            return testData;
        }
    }
}