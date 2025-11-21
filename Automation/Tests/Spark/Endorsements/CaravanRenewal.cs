using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using Rac.TestAutomation.Common.TestData.Endorsements;
using Tests.ActionsAndValidations;
using Tests.ActionsAndValidations.Endorsements;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Spark.Endorsements
{
    [Property("Functional", "Spark Caravan Renewal")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class CaravanRenewal : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Spark Caravan Renewal");
        }

        #region TestCases

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan), Category(TestCategory.CaravanRenewal)]
        [Test(Description = "Caravan Policy with Right make-model and new payment method changed from Cash to BPAY")]
        public void INSU_T48_CaravanRenewal_RightMakeModel_AnnualCashToBPay()
        {
            var testData = INSU_T48_BuildCaravanRenewal_AnnualCashToBPay();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanRenewalFlow(_browser, testData, detailUiChecking:false, isFailedPayment:false, retryOTP:false);

            Reporting.LogTestShieldValidations("policy renewal", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyDetailsInShield(testData, isFailedPayment: false);

            Reporting.Log("PCM Verifications for Renewals");
            VerifyEndorseCaravan.VerifyPolicyStateInPcmAfterRenewal(_browser, testData);
        }


        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan), Category(TestCategory.CaravanRenewal)]
        [Test(Description = "Caravan Policy Change make-model and new payment method changed from Month CC to Annual CC")]
        public void INSU_T49_CaravanRenewal_ChangeMakeModel_MonthlyCCToAnnualCC()
        {
            var testData = INSU_T49_BuildCaravanRenewal_MonthlyCCToAnnualCC();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanRenewalFlow(_browser, testData, detailUiChecking: false, isFailedPayment: false, retryOTP: false);

            Reporting.LogTestShieldValidations("policy renewal", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyDetailsInShield(testData, isFailedPayment: false);

            Reporting.Log("PCM Verifications for Renewals");
            VerifyEndorseCaravan.VerifyPolicyStateInPcmAfterRenewal(_browser, testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan), Category(TestCategory.CaravanRenewal)]
        [Test(Description = "Caravan Policy Right make-model and new payment method changed from Annual Cash to Pay Later")]
        public void INSU_T50_CaravanRenewal_RightMakeModel_AnnualCashToPayLater()
        {
            var testData = INSU_T50_BuildCaravanRenewal_AnnualCashToPayLayer();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanRenewalFlow(_browser, testData, detailUiChecking: false, isFailedPayment: false, retryOTP: false);

            Reporting.LogTestShieldValidations("policy renewal", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyDetailsInShield(testData, isFailedPayment: false);

            Reporting.Log("PCM Verifications for Renewals");
            VerifyEndorseCaravan.VerifyPolicyStateInPcmAfterRenewal(_browser, testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan), Category(TestCategory.CaravanRenewal)]
        [Test(Description = "Caravan Policy change make-model and new payment method changed from Monthly CC to Monthly direct debit with field validation check and MFA")]
        public void INSU_T51_CaravanRenewal_ChangeMakeModel_MonthlyCCToMonthlyDD_DetailUIChecking()
        {
            var testData = INSU_T51_BuildCaravanRenewal_MonthlyCCToMonthlyDD("CUSTOM BUILT", 2024, "18FT CARAVAN", 10000, "70050862");
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanRenewalFlow(_browser, testData, detailUiChecking: true, isFailedPayment: false, retryOTP:true);

            Reporting.LogTestShieldValidations("policy renewal", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyDetailsInShield(testData, isFailedPayment: false);

            Reporting.Log("PCM Verifications for Renewals");
            VerifyEndorseCaravan.VerifyPolicyStateInPcmAfterRenewal(_browser, testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan), Category(TestCategory.CaravanRenewal)]
        [Test(Description = "Caravan Policy right make-model and new payment method changed from Annual Cash to Credit card with failed payment")]
        public void INSU_T52_CaravanRenewal_RightMakeModel_AnnualCashToAnnualCC_FailedPayment()
        {
            var testData = INSU_T52_BuildCaravanRenewal_CashToCreditCard();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanRenewalFlow(_browser, testData, detailUiChecking: false, isFailedPayment: true, retryOTP: false);

            Reporting.LogTestShieldValidations("policy renewal", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyDetailsInShield(testData, isFailedPayment: true);

            Reporting.Log("PCM Verifications for Renewals");
            VerifyEndorseCaravan.VerifyPolicyStateInPcmAfterRenewal(_browser, testData);
        }

        #endregion

        #region Test Case Helper Methods
        private EndorseCaravan INSU_T48_BuildCaravanRenewal_AnnualCashToBPay()
        {
            var testData = ShieldCaravanDB.FindCaravanPolicyForRenewal(paymentScenarios: PaymentScenario.AnnualCash);

            var paymentAccount = new PaymentV2()
                .Annual()
                .PaymentTiming(PaymentOptionsSpark.BPay);

            return new CaravanEndorsementBuilder()
                .InitialiseCaravanWithDefaultData(testData.PolicyNumber)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        private EndorseCaravan INSU_T49_BuildCaravanRenewal_MonthlyCCToAnnualCC()
        {
            var testData = ShieldCaravanDB.FindCaravanPolicyForRenewal(paymentScenarios: PaymentScenario.MonthlyCard);

            var paymentAccount = new PaymentV2()
                .CreditCard(DataHelper.RandomCreditCard())
                .Annual()
                .PaymentTiming(PaymentOptionsSpark.AnnualCash);

            return new CaravanEndorsementBuilder()
                .InitialiseCaravanWithDefaultData(testData.PolicyNumber) 
                .WithRandomNewCaravan()
                .WithRandomCaravanRego()
                .WithSparkPaymentChoice(paymentAccount)
                .WithParked(CaravanParkLocation.Driveway)
                .Build();
        }

        private EndorseCaravan INSU_T50_BuildCaravanRenewal_AnnualCashToPayLayer()
        {
            var testData = ShieldCaravanDB.FindCaravanPolicyForRenewal(paymentScenarios: PaymentScenario.AnnualCash);

            var paymentAccount = new PaymentV2()
                 .Annual()
                 .PaymentTiming(PaymentOptionsSpark.PayLater);

            return new CaravanEndorsementBuilder()
                .InitialiseCaravanWithDefaultData(testData.PolicyNumber)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        /// <summary>
        /// Passing the make/model other details to enable value field to enter caravan market value
        /// </summary>
        private EndorseCaravan INSU_T51_BuildCaravanRenewal_MonthlyCCToMonthlyDD(string make, decimal year, string model, int marketValue, string vehicleId = null)
        {
            var testData = ShieldCaravanDB.FindCaravanPolicyForRenewal(paymentScenarios: PaymentScenario.MonthlyCard);

            var paymentAccount = new PaymentV2()
               .BankAccount(new BankAccount().InitWithRandomValues())
               .Monthly()
               .PaymentTiming(PaymentOptionsSpark.DirectDebit);

            return new CaravanEndorsementBuilder()
                .InitialiseCaravanWithDefaultData(testData.PolicyNumber)
                .WithNewCaravan(make,year,model,marketValue,vehicleId)
                .WithRandomCaravanRego()
                .WithSparkPaymentChoice(paymentAccount)
                .WithParked(CaravanParkLocation.Garage)
                .Build();
        }

        private EndorseCaravan INSU_T52_BuildCaravanRenewal_CashToCreditCard()
        {
            var testData = ShieldCaravanDB.FindCaravanPolicyForRenewal(paymentScenarios: PaymentScenario.AnnualCash);

            var paymentAccount = new PaymentV2()
                        .CreditCard(DataHelper.RandomCreditCard(isNotSufficientFundCard: true))
                        .Annual()
                        .PaymentTiming(PaymentOptionsSpark.AnnualCash);

            return new CaravanEndorsementBuilder()
                .InitialiseCaravanWithDefaultData(testData.PolicyNumber)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }
        #endregion
    }
}
