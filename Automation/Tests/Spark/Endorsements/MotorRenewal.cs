using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using Rac.TestAutomation.Common.TestData.Endorsements;
using Tests.ActionsAndValidations;
using Tests.ActionsAndValidations.Endorsements;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace Spark.Endorsements
{
    [Property("Functional", "Spark Motor Renewal")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class MotorRenewal : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Spark Motor Renewal");
        }

        #region TestCases
        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.MotorRenewal)]
        [Test(Description = "Full Cover Credit Card payment with insufficient balance")]
        public void INSU_T103_FullCoverMotorRenewal_RightMakeModel_PayCreditCardFromCash_FailedPayment()
        {
            var testData = BuildMotorRenewalOneOffCCFailedPaymentMFCO();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorRenewalFlow(_browser, testData, isFailedPayment:true);            
            ActionsMotorEndorsement.VerifyConfirmationPage(_browser, testData, isFailedPayment: true);

            Reporting.LogTestShieldValidations("policy renewal", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyDetailsInShield(testData, isFailedPayment: true);

            //Verifying renewal details on PCM
            Reporting.Log("PCM Verifications for Renewals");
            VerifyEndorseMotor.VerifyPolicyStateInPcmAfterRenewal(_browser, testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.MotorRenewal)]
        [Test(Description = "TFT Policy with Right make-model and new payment method changed from Cash to BPAY")]
        public void INSU_T99_TFTMotorRenewal_RightMakeModel_PayWithBPayFromAnnualCash()
        {
            var testData = BuildMotorRenewalBPayTFT();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorRenewalFlow(_browser, testData);
            ActionsMotorEndorsement.VerifyConfirmationPage(_browser, testData);

            Reporting.LogTestShieldValidations("policy renewal", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyDetailsInShield(testData);

            //Verifying renewal details on PCM
            Reporting.Log("PCM Verifications for Renewals");
            VerifyEndorseMotor.VerifyPolicyStateInPcmAfterRenewal(_browser, testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.MotorRenewal)]
        [Test(Description = "TPO Right make & model, update annual KM, add modifications, hire car and payment method changed from Cash to Pay later")]
        public void INSU_T100_TPOMotorRenewal_RightMakeModel_PayWithPayLaterFromAnnualCash()
        {
            var testData = BuildMotorRenewalPayLaterTPO();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorRenewalFlow(_browser, testData, detailUiChecking: true);
            ActionsMotorEndorsement.VerifyConfirmationPage(_browser, testData);

            Reporting.LogTestShieldValidations("policy renewal", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyDetailsInShield(testData);

            //Verifying renewal details on PCM
            Reporting.Log("PCM Verifications for Renewals");
            VerifyEndorseMotor.VerifyPolicyStateInPcmAfterRenewal(_browser, testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.MotorRenewal), Category(TestCategory.MultiFactorAuthentication)]
        [Test(Description = "Full Cover NOT Right make-model, search using make-model, add modifications & finance, business use, hire car & NCB and payment method changed from monthly CC to monthly Direct Debit  with field, tool tip and bsb validation")]
        public void INSU_T101_FullCoverMotorRenewal_NewMakeModel_PayWithMonthlyDirectDebitFromMonthlyCard_WithFieldValidations()
        {
            var testData = BuildMotorRenewalFromMonthlyCardMfcoToConvertToAnnualCash();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorRenewalFlow(_browser, testData, detailUiChecking: true);

            //MFA for bank account input
            bool expectBankMFA = Config.Get().IsSparkEndorsementBankMultiFactorAuthenticationExpected();
            if (expectBankMFA)
            {
                ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber, detailUiChecking: true);
            }
            else
            {
                Reporting.Log($"{SparkFeatureToggles.SparkEndorsementsBankExpectMultiFactorAuthentication.GetDescription()} = {expectBankMFA} so we won't expect it here.");
            }
            
            ActionsMotorEndorsement.VerifyConfirmationPage(_browser, testData);

            Reporting.LogTestShieldValidations("policy renewal", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyDetailsInShield(testData);

            //Verifying renewal details on PCM
            Reporting.Log("PCM Verifications for Renewals");
            VerifyEndorseMotor.VerifyPolicyStateInPcmAfterRenewal(_browser, testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.MotorRenewal)]
        [Test(Description = "Full Cover NOT Right make-model, search using Make, no modifications & with finance, rideshare use, no hire car & NCB and payment method changed from monthly CC to annual CC")]
        public void INSU_T102_FullCoverMotorRenewal_NewMakeModel_PayWithAnnualCreditCardFromMonthlyCreditCard()
        {
            var testData = BuildMotorRenewalOneOffCCPaymentMFCOBusiness();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorRenewalFlow(_browser, testData);            
            ActionsMotorEndorsement.VerifyConfirmationPage(_browser, testData);

            Reporting.LogTestShieldValidations("policy renewal", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyDetailsInShield(testData);

            //Verifying renewal details on PCM
            Reporting.Log("PCM Verifications for Renewals");
            VerifyEndorseMotor.VerifyPolicyStateInPcmAfterRenewal(_browser, testData);
        }

        #endregion

        #region Test Case Helper Methods
        private EndorseCar BuildMotorRenewalBPayTFT()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForRenewal(motorCover: MotorCovers.TFT, hasRiskAddress: true, paymentScenarios: PaymentScenario.AnnualCash);


            var paymentAccount = new PaymentV2()
                .Annual()
                .PaymentTiming(PaymentOptionsSpark.BPay);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(false)
                .WithRandomNewCar()
                // TODO: SPK-6704 Remove the IsMotorPolicyWithExcessChanges check and the $500 option when the story is actioned as all the TFT policies will be on version 46 with default excess as $700
                .WithExcess(testData.IsMotorPolicyWithExcessChanges() ? "$700" : "$500")
                .WithUsage(usage: VehicleUsage.Private)
                .WithAnnualKms(AnnualKms.LessThan5000)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        private EndorseCar BuildMotorRenewalPayLaterTPO()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForRenewal(motorCover: MotorCovers.TPO, hasRiskAddress: true, paymentScenarios: PaymentScenario.AnnualCash);

            var paymentAccount = new PaymentV2()
                .Annual()
                .PaymentTiming(PaymentOptionsSpark.PayLater);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(false)
                .WithRandomNewCar()
                // TODO: SPK-6704 Remove the IsMotorPolicyWithExcessChanges check and $0 option when this story is actioned as all TPO policies will be on version 46 with $700 as the only excess option
                .WithExcess(testData.IsMotorPolicyWithExcessChanges() ? "$700" : "$0")
                .WithUsage(usage: VehicleUsage.Private)
                .WithAnnualKms(AnnualKms.MoreThan20000)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        private EndorseCar BuildMotorRenewalOneOffCCFailedPaymentMFCO()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForRenewal(motorCover: MotorCovers.MFCO, hasRiskAddress: true, paymentScenarios: PaymentScenario.AnnualCash);

            var paymentAccount = new PaymentV2()
                .CreditCard(DataHelper.RandomCreditCard(isNotSufficientFundCard:true))
                .Annual()
                .PaymentTiming(PaymentOptionsSpark.AnnualCash);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(false)
                .WithRandomNewCar()
                .WithRandomNewCarRego()
                .WithExcess("$1000")
                .WithAnnualKms(AnnualKms.UpTo10000)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        private EndorseCar BuildMotorRenewalFromMonthlyCardMfcoToConvertToAnnualCash()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForRenewal(motorCover: MotorCovers.MFCO, hasRiskAddress: true, paymentScenarios: PaymentScenario.MonthlyCard);

            var paymentAccount = new PaymentV2()
                .BankAccount(new BankAccount().InitWithRandomValues())
                .Monthly()
                .PaymentTiming(PaymentOptionsSpark.DirectDebit);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(true)
                .WithRandomNewCar()
                .WithRandomNewCarRego()
                .WithExcess("$1500")
                .WithAnnualKms(AnnualKms.UpTo10000)
                .WithRandomFinancier()
                .WithUsage(usage: VehicleUsage.Business)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        private EndorseCar BuildMotorRenewalOneOffCCPaymentMFCOBusiness()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForRenewal(motorCover: MotorCovers.MFCO, hasRiskAddress: true, paymentScenarios: PaymentScenario.AnnualCash);

            var paymentAccount = new PaymentV2()
                .CreditCard(DataHelper.RandomCreditCard())
                .Annual()
                .PaymentTiming(PaymentOptionsSpark.AnnualCash);

            return new MotorEndorsementBuilder() 
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(true)
                .WithRandomNewCar()
                .WithRandomNewCarRego()
                .WithRandomFinancier()
                .WithExcess("$2000")
                .WithUsage(usage: VehicleUsage.Ridesharing)
                .WithAnnualKms(AnnualKms.UpTo15000)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }
        #endregion
    }
}
