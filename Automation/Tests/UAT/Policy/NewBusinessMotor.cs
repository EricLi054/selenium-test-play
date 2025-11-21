using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using System;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace Tests.UAT.Policy
{
    [TestFixture]
    [Property("UAT", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class NewBusinessMotor : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C UAT tests for new business for Motor insurance policies.");
        }

        //Motor Comprehensive with Roadside - Quote to Purchase - Multi Driver - Single Match in MC -  Declined due to Driver History 
        //then history amended and purchase completed with RealTime Credit Card including roadside confirmation
        [Test, Category(TestCategory.UAT), Category(TestCategory.Motor), 
            Category(TestCategory.New_Business)]
        public void UAT_Policy_Motor_MFCOWithRSAPurchaseMultipleDrivers()
        {
            var vehicleQuote = TestData_MFCOWithRSAPurchaseMultiplePHMultipleDrivers();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, vehicleQuote.ToString());
            
            Reporting.LogTestStart();
            string policyNumber = ActionsQuoteMotor.PurchaseMotorVehiclePolicy(_browser, vehicleQuote, includeRequestEmailQuote: true);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteMotor.VerifyMotorVehiclePolicyInShield(vehicleQuote, policyNumber);
        }

        private QuoteCar TestData_MFCOWithRSAPurchaseMultiplePHMultipleDrivers()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual()
                                                        .WithDateOfBirth(DataHelper.RandomDoB(22,24))
                                                        .WithGender(Gender.Male)
                                                        .WithRandomTitleFromGender()
                                                        .Build();
            mainPH.ChangeNameCase(Contact.CaseOptions.TitleCase);

            var secondPH = new ContactBuilder(DataHelper.CreateMemberMatchContact(
                                                             maximumAge: 24,
                                                             minimumAge: 22,
                                                             gender: Gender.Female,
                                                             membershipTiers: new MembershipTier[] {
                                                                 MembershipTier.None
                                                             })).Build();

            var thirdPH = new ContactBuilder(DataHelper.CreateMemberMatchContact(
                                                             maximumAge: 24,
                                                             minimumAge: 22,
                                                             gender: Gender.Male,
                                                             membershipTiers: new MembershipTier[] {
                                                                 MembershipTier.None
                                                             })).Build();
            mainPH.ChangeNameCase(Contact.CaseOptions.UpperCase);

            var vehicle = new MotorCarBuilder().InitialiseMotorCarWithRandomData(mainPH, true)
                                             // TODO: INSU-286 Remove the check for "IsExcessChangesEnabled" in WithAdditionalDriverLicenseTime when the Excess&NCB toggle is removed from the UI
                                             .WithDriver(secondPH, true) 
                                             .WithDriver(thirdPH, true)
                                             .WithCover(MotorCovers.MFCO) // The default is MFCO, but explicitly setting for clarity.
                                             .WithRandomFinancier()
                                             .WithUsage(VehicleUsage.Private) // To get RSA offer.
                                             .WithAnnualKms(AnnualKms.UpTo10000)
                                             .WithRandomDriverConvictions(driverIndex: 1, min: 6, max: 7) // max is not inclusive
                                             .WithRandomDriverAccidentHistory(driverIndex: 1, min: 6, max: 7) // max is not inclusive
                                             .WithDisclosureDeclineNoticeThenDismiss(true)
                                             .WithPaymentMethod(new Payment(mainPH).CreditCard().Annual())
                                             .WithMainDriverLicenseTime("5 years")
                                             .WithPurchaseRoadsideAssistanceMembershipBundle(true)
                                             .WithHireCarAfterAccidentCover(true)
                                             .WithIsModified(true)
                                             .WithExcess("$1,000")
                                             .WithPercentageInsuredValueChangeFromMarketValue(10)
                                             .Build();

            return vehicle;
        }

        //Motor Comprehensive no Roadside - Quote to Purchase - Single Driver - Single Match in MC -  Declined due to Courier use
        //then usage amended and purchase completed with Annual Direct Debit from Bank Account
        [Test, Category(TestCategory.UAT), Category(TestCategory.Motor),
               Category(TestCategory.New_Business)]
        public void UAT_Policy_Motor_MFCOBusinessUsageSinglePHSingleDriver()
        {
            // Setup test data
            var vehicleQuote = TestData_MFCOBusinessUsageSinglePHSingleDriver();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, vehicleQuote.ToString());

            Reporting.LogTestStart();
            string policyNumber = ActionsQuoteMotor.PurchaseMotorVehiclePolicy(_browser, vehicleQuote);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteMotor.VerifyMotorVehiclePolicyInShield(vehicleQuote, policyNumber);
        }

        //Motor Third Party Fire & Theft - Create and Retrieve Then Purchase - Single Driver - Multiple Match in MC resulting in Create New
        [Test, Category(TestCategory.UAT), Category(TestCategory.Motor), 
               Category(TestCategory.New_Business)]
        public void UAT_Policy_Motor_MTFTWithRSATierSinglePHSingleDriver()
        {
            // Setup test data
            var vehicleQuote = TestData_MTFTWithRSATierSinglePHSingleDriver();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, vehicleQuote.ToString());
            
            Reporting.LogTestStart();
            string policyNumber = ActionsQuoteMotor.PurchaseMotorVehiclePolicy(_browser, vehicleQuote, includeRetrieveQuote: true);
            
            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteMotor.VerifyMotorVehiclePolicyInShield(vehicleQuote, policyNumber);

        }

        private QuoteCar TestData_MTFTWithRSATierSinglePHSingleDriver()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual()
                                             .WithMultiMatchContact(gender: Gender.Female, minimumAge: 25, maximumAge: 50)
                                             .WithoutDeclaringMembership(true)
                                             .Build();

            var vehicle = new MotorCarBuilder().InitialiseMotorCarWithRandomData(mainPH, true)
                                 .WithCover(MotorCovers.TFT)
                                 .WithRandomFinancier()
                                 .WithUsage(VehicleUsage.Private)
                                 .WithAnnualKms(AnnualKms.UpTo15000)
                                 .WithPaymentMethod(new Payment(mainPH).BankAccount().Monthly())
                                 .WithPurchaseRoadsideAssistanceMembershipBundle(true)
                                 .WithPercentageInsuredValueChangeFromMarketValue(5)
                                 .WithPolicyStartDate(DateTime.Now.AddDays(5))
                                 .Build();

            return vehicle;
        }

        //Motor Third Party Only - Create and Retrieve Then Purchase - Multiple Driver - Main Driver Tiered Single Match in MC - Additional Driver Not Found - Create New
        [Test, Category(TestCategory.UAT), Category(TestCategory.Motor), 
               Category(TestCategory.New_Business)]
        public void UAT_Policy_Motor_MTPOWithRSATierMultiplePHMultipleDrivers()
        {
            var vehicleQuote = TestData_MTPOWithRSATierMultiplePHMultipleDrivers();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, vehicleQuote.ToString());

            Reporting.LogTestStart();
            string policyNumber = ActionsQuoteMotor.PurchaseMotorVehiclePolicy(_browser, vehicleQuote, includeRetrieveQuote: true, includeRequestEmailQuote: true);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteMotor.VerifyMotorVehiclePolicyInShield(vehicleQuote, policyNumber);
        }

        private QuoteCar TestData_MTPOWithRSATierMultiplePHMultipleDrivers()
        {
            var mainPH = DataHelper.CreateMemberMatchContact(maximumAge: 30,
                                                             minimumAge: 30,
                                                             gender: Gender.Male,
                                                             declareExistingMembership: true,
                                                             membershipTiers: new MembershipTier[] {
                                                                 MembershipTier.Silver
                                                             }
                                                             );
            mainPH.ChangeNameCase(Contact.CaseOptions.CamelCase);

            var secondPH = new ContactBuilder().InitialiseRandomIndividual().Build();
            var thirdPH  = new ContactBuilder().InitialiseRandomIndividual().Build();

            var vehicle = new MotorCarBuilder().InitialiseMotorCarWithRandomData(mainPH, true)
                                 // TODO: INSU-286 Remove the check for "IsExcessChangesEnabled" in WithAdditionalDriverLicenseTime when the Excess&NCB toggle is removed from the UI
                                 .WithDriver(secondPH, true)
                                 .WithDriver(thirdPH, false)
                                 .WithRandomDriverAccidentHistory(2,1,2)
                                 .WithCover(MotorCovers.TPO)
                                 .WithoutFinancier()
                                 .WithUsage(VehicleUsage.Private)
                                 .WithAnnualKms(AnnualKms.UpTo20000)
                                 .WithPaymentMethod(new Payment(mainPH).CreditCard().Monthly())
                                 .WithPolicyStartDate(DateTime.Now.AddDays(21))
                                 .WithIsModified(true)
                                 .WithPayer(thirdPH)
                                 .Build();
            return vehicle;
        }

        private QuoteCar TestData_MFCOBusinessUsageSinglePHSingleDriver()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual()
                                             .WithGender(Gender.Female)
                                             .WithRandomTitleFromGender()
                                             .WithRandomDateOfBirth(70)
                                             .Build();

            var vehicle = new MotorCarBuilder().InitialiseMotorCarWithRandomData(mainPH, true)
                                 .WithCover(MotorCovers.MFCO)
                                 .WithoutFinancier()
                                 .WithMainDriverLicenseTime("5 years")
                                 .WithUsage(VehicleUsage.CourierOrDelivery)
                                 .WithAnnualKms(AnnualKms.LessThan5000)
                                 .WithVehicleUsageDeclineThenDismiss(true)
                                 .WithPaymentMethod(new Payment(mainPH).BankAccount().Annual())
                                 .WithExcess("$800")
                                 .Build();

            return vehicle;
        }
    }
}
