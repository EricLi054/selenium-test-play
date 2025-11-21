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

namespace Tests.UAT.Policy
{
    [TestFixture]
    [Property("UAT", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class NewBusinessHome : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C UAT tests for new business for Home insurance policies.");
        }

        [Test, Category(TestCategory.UAT), Category(TestCategory.Home), 
               Category(TestCategory.New_Business)]
        public void UAT_Policy_Home_OwnerOccupiedBuildingContentsRSADiscount()
        {
            var testData = TestData_OwnerOccupiedBuildingContentsMembershipDiscount();

            var policyNumber = ActionsQuoteHome.PurchaseHomePolicy(_browser, testData);

            // Verify Policy against Shield database
            VerifyQuoteHome.VerifyHomePolicyInShield(testData, policyNumber);
        }

        [Test, Category(TestCategory.UAT), Category(TestCategory.Home), 
               Category(TestCategory.New_Business)]
        public void UAT_Policy_Home_LandlordBuildingContentsMultiMatch()
        {
            var testData = TestData_LandlordBuildingContentsMultiMatch();

            var quoteNumber = ActionsQuoteHome.GetNewHomeQuote(browser: _browser, quoteDetails: testData, premiumValues: out QuoteData expectedQuoteValues);
            _browser.CloseBrowser();

            ActionsQuote.RetrieveB2CQuote(_browser, testData.PropertyAddress, quoteNumber, ShieldProductType.HGP);
            if (_testConfig.IsUseAddressManagementApiEnabled)
            {
                testData.QuoteHasBeenRetrieved = true;
            }
            VerifyQuoteHome.VerifyQuoteAfterRetrieve(_browser, testData, expectedQuoteValues);

            ActionsQuoteHome.HomeQuoteClickBuyPage2(_browser);
            ActionsQuoteHome.HomeQuoteAddedDetailsPage3(_browser, testData);
            var policyNumber = ActionsQuoteHome.AcceptQuoteSummaryAndPurchase(_browser, testData, expectedQuoteValues);

            // Verify Policy against Shield database
            VerifyQuoteHome.VerifyHomePolicyInShield(testData, policyNumber);
        }

        [Test, Category(TestCategory.UAT), Category(TestCategory.Home), 
               Category(TestCategory.New_Business)]
        public void UAT_Policy_Home_Renters()
        {
            var testData = TestData_RentersAnonymous();

            var quoteNumber = ActionsQuoteHome.GetNewHomeQuote(browser: _browser, quoteDetails: testData, premiumValues: out QuoteData expectedQuoteValues);
            ActionsQuoteHome.HomeQuoteClickBuyPage2(_browser);
            ActionsQuoteHome.HomeQuoteAddedDetailsPage3(_browser, testData);
            _browser.CloseBrowser();

            ActionsQuote.RetrieveB2CQuote(_browser, testData.PropertyAddress, quoteNumber, ShieldProductType.HGP);
            if (_testConfig.IsUseAddressManagementApiEnabled)
            {
                testData.QuoteHasBeenRetrieved = true;
            }
            VerifyQuoteHome.VerifyQuoteAfterRetrieve(_browser, testData, expectedQuoteValues);

            ActionsQuoteHome.HomeQuoteClickBuyPage2(_browser);
            ActionsQuoteHome.HomeQuoteAddedDetailsPage3VerifyAndSubmit(_browser, testData);
            var policyNumber = ActionsQuoteHome.AcceptQuoteSummaryAndPurchase(_browser, testData, expectedQuoteValues);

            // Verify Policy against Shield database
            VerifyQuoteHome.VerifyHomePolicyInShield(testData, policyNumber);
        }

        [Test, Category(TestCategory.UAT), Category(TestCategory.Home), 
               Category(TestCategory.New_Business)]
        public void UAT_Policy_Home_HolidayHomeBuildingContents()
        {
            var testData = TestData_HolidayHomeBuildingContents();

            var quoteNumber = ActionsQuoteHome.GetNewHomeQuote(browser: _browser, quoteDetails: testData, premiumValues: out QuoteData expectedQuoteValues);
            _browser.CloseBrowser();

            ActionsQuote.RetrieveB2CQuote(_browser, testData.PropertyAddress, quoteNumber, ShieldProductType.HGP);
            if (_testConfig.IsUseAddressManagementApiEnabled)
            {
                testData.QuoteHasBeenRetrieved = true; 
            }
            VerifyQuoteHome.VerifyQuoteAfterRetrieve(_browser, testData, expectedQuoteValues);

            ActionsQuoteHome.HomeQuoteClickBuyPage2(_browser);
            ActionsQuoteHome.HomeQuoteAddedDetailsPage3(_browser, testData);
            var policyNumber = ActionsQuoteHome.AcceptQuoteSummaryAndPurchase(_browser, testData, expectedQuoteValues);

            // Verify Policy against Shield database
            VerifyQuoteHome.VerifyHomePolicyInShield(testData, policyNumber);
        }

        private QuoteHome TestData_OwnerOccupiedBuildingContentsMembershipDiscount()
        {
            var mainPH = new ContactBuilder(DataHelper.CreateMemberMatchContact(
                                                             membershipTiers: new MembershipTier[] { MembershipTier.Gold,
                                                                                                     MembershipTier.Silver,
                                                                                                     MembershipTier.Bronze}))
                                .WithPrivateEmailAsPreferredDeliveryMethod(true)
                                .Build();
            mainPH.ChangeNameCase(Contact.CaseOptions.TitleCase);

            var secondPH = new ContactBuilder().InitialiseRandomIndividual()
                                               .WithMailingAddress(mainPH.MailingAddress)
                                               .WithPrivateEmailAsTheOnlyPreferredDeliveryMethod()
                                               .Build();
            var thirdPH  = new ContactBuilder().InitialiseRandomIndividual()
                                               .WithMailingAddress(mainPH.MailingAddress)
                                               .WithPrivateEmailAsTheOnlyPreferredDeliveryMethod()
                                               .Build();


            var testData = new HomeBuilder().InitialiseHomeWithRandomData(HomeOccupancy.OwnerOccupied,
                                                                          new List<Contact>() { mainPH, secondPH, thirdPH })
                                            .WithAlarmSystem(Alarm.NonMonitoredAlarm)
                                            .WithIsUsedForShortStay(true)
                                            .WithInsuranceHistory(HomePreviousInsuranceTime.Zero)
                                            .WithBuildingType(HomeType.House)
                                            .WithWallMaterial(HomeMaterial.Brick)
                                            .WithRandomFinancier()
                                            .WithPaymentMethod(new Payment(mainPH).BankAccount().Monthly())
                                            .WithPolicyStartDate(DateTime.Now.AddDays(29).Date)
                                            .WithAreDoorsSecured(true)
                                            .WithAreWindowsSecured(true)
                                            .WithRandomClaimsHistory(3)
                                            .IsHomeUsageUnacceptable(true)
                                            .WithExcess("300", "300")
                                            .AddSpecifiedPersonalValuable(SpecifiedValuables.JewelleryWatches, "Family Jewells", 3456)
                                            .AddSpecifiedContentsItem(SpecifiedContents.ArtAntiquesFurs, "Pictures and antiques", 6000)
                                            .WithRandomUnspecifiedPersonalValuablesCover()
                                            .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            return testData;
        }

        private QuoteHome TestData_LandlordBuildingContentsMultiMatch()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual()
                                             .WithMultiMatchContact()
                                             .WithMailingAddressAsTheOnlyPreferredDeliveryMethod()
                                             .WithoutMembership()
                                             .Build();
            mainPH.ChangeNameCase(Contact.CaseOptions.CamelCase);

            var secondPH = new ContactBuilder().InitialiseRandomIndividual()
                                               .WithMultiMatchContact()
                                               .WithMailingAddress(mainPH.MailingAddress)
                                               .WithMailingAddressAsTheOnlyPreferredDeliveryMethod()
                                               .Build();
            secondPH.ChangeNameCase(Contact.CaseOptions.CamelCase);

            var testData = new HomeBuilder().InitialiseHomeWithRandomData(HomeOccupancy.InvestmentProperty,
                                                                          new List<Contact>() { mainPH, secondPH })
                                            .WithAlarmSystem(Alarm.MonitoredAlarm)
                                            .WithInsuranceHistory(HomePreviousInsuranceTime.One)
                                            .WithBuildingType(HomeType.Unit)
                                            .WithWallMaterial(HomeMaterial.Fibro)
                                            .WithRandomFinancier()
                                            .WithPaymentMethod(new Payment(secondPH).CreditCard().Monthly())
                                            .WithAreDoorsSecured(false)
                                            .WithAreWindowsSecured(false)
                                            .WithRandomContentsSumInsured(minValue:(HOME_CONTENTS_SI_MAX_NO_SECURITY + 1))  // triggers callback due not having minimum security
                                            .WithRandomPropertyManager()
                                            .WithRandomRentalRate()
                                            .WithExcess("200", "200")
                                            .Build();

            Reporting.IsTrue(testData.IsContentsAboveSecurityLimit(), "test data has the values to trigger desired callback case.");

            // Logging out test data for easier debugging and auditing.
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            return testData;
        }

        private QuoteHome TestData_RentersAnonymous()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual()
                                             .WithPrivateEmailAsTheOnlyPreferredDeliveryMethod()
                                             .WithMembership(MembershipTier.Silver, "123456")
                                             .Build();
            mainPH.ChangeNameCase(Contact.CaseOptions.UpperCase);

            var testData = new HomeBuilder().InitialiseHomeWithRandomData(HomeOccupancy.Tenant,
                                                                          new List<Contact>() { mainPH })
                                            .WithAlarmSystem(Alarm.NoAlarm)
                                            .WithoutBuildingSumInsured()
                                            .WithRandomRentersContentsSumInsured()
                                            .WithInsuranceHistory(HomePreviousInsuranceTime.Two)
                                            .WithBuildingType(HomeType.Flat)
                                            .WithWallMaterial(HomeMaterial.Cement)
                                            .WithPolicyholderHasPastConvictions()
                                            .WithPaymentMethod(new Payment(mainPH).BankAccount().Annual())
                                            .WithAreDoorsSecured(true)
                                            .WithAreWindowsSecured(true)
                                            .WithRandomUnspecifiedPersonalValuablesCover()
                                            .WithoutFinancier()  // No mortgage questions for contents only.
                                            .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            return testData;
        }

        private QuoteHome TestData_HolidayHomeBuildingContents()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual()
                                             .Build();


            var testData = new HomeBuilder().InitialiseHomeWithRandomData(HomeOccupancy.HolidayHome,
                                                                          new List<Contact>() { mainPH })
                                            .WithAlarmSystem(Alarm.RACMonitoredAlarm)
                                            .WithInsuranceHistory(HomePreviousInsuranceTime.Three)
                                            .WithBuildingType(HomeType.Villa)
                                            .WithWallMaterial(HomeMaterial.Timber)
                                            .WithPaymentMethod(new Payment(mainPH).CreditCard().Annual())
                                            .WithPolicyStartDate(DateTime.Now.AddDays(10).Date)
                                            .WithAreDoorsSecured(true)
                                            .WithAreWindowsSecured(true)
                                            .WithExcess("1000", "1000")
                                            .AddSpecifiedContentsItem(SpecifiedContents.AudioVisualCollections, "VHS and music", 5432)
                                            .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            return testData;
        }
    }
}
