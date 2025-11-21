using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using Rac.TestAutomation.Common.TestData.Endorsements;
using System;
using Tests.ActionsAndValidations;
using Tests.ActionsAndValidations.Endorsements;
using static Rac.TestAutomation.Common.Constants;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace Spark.Endorsements
{
    [Property("Functional", "Spark Caravan Endorsement")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class CaravanEndorsement : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Spark Caravan Endorsement");
        }

        #region TestCases

        /// <summary>
        /// Endorse Caravan policy
        /// Payment plan: Annual Cash
        /// Premium change: None
        /// Endorsement effective: Now
        /// Change Make/Model: False
        /// Provide new random registration: True
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan)]
        [Test(Description = "Annual Cash Caravan Policy for Endorsement with No change in Premium")]
        public void INSU_T53_CaravanEndorsement_AnnualCash_NoPremiumChange()
        {
            var testData = BuildCaravanEndorsement_NoPremiumChange(PaymentScenario.AnnualCash);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanEndorseFlow(_browser, testData);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPaymentDetailsInShield(testData);
        }

        /// <summary>
        /// Endorse Caravan policy
        /// Payment plan: Monthly Installment
        /// Premium change: None
        /// Endorsement effective: Within 14 days
        /// Change Make/Model: False
        /// Provide new random registration: False
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan)]
        [Test(Description = "Monthly Installment Caravan Policy for Endorsement with No change in Premium")]
        public void INSU_T64_CaravanEndorsement_MonthlyInstallment_NoPremiumChange()
        {
            var testData = BuildCaravanEndorsement_NoPremiumChange(PaymentScenario.MonthlyBank);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanEndorseFlow(_browser, testData);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPaymentDetailsInShield(testData);
        }

        /// <summary>
        /// Endorse Caravan policy
        /// Payment plan: Annual Installment
        /// Premium change: None
        /// Endorsement effective: Within 14 days
        /// Change Make/Model: False
        /// Provide new random registration: True
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan)]
        [Test(Description = "Annual Installment Caravan Policy for Endorsement with No change in Premium")]
        public void INSU_T60_CaravanEndorsement_AnnualInstallment_NoPremiumChange()
        {
            var testData = BuildCaravanEndorsement_NoPremiumChange(PaymentScenario.AnnualBank);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanEndorseFlow(_browser, testData);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPaymentDetailsInShield(testData);
        }

        /// <summary>
        /// Endorse Caravan policy
        /// Payment plan: Annual Installment
        /// Premium change: None
        /// Endorsement effective: Within 14 days
        /// Change Make/Model: Yes
        /// Provide new random registration & Park Location: True
        /// Payment : Existing BankAccount
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan)]
        [Test(Description = "Annual Installment Caravan Policy for Endorsement with Increase in Premium and Pay from Existing BankAccount")]
        public void INSU_T61_CaravanEndorsement_AnnualInstallment_PremiumIncrease_PayWithExistingBankAccount()
        {
            var paymentAccount = new PaymentV2().BankAccount(new BankAccount().InitWithRandomValues()).Annual().PaymentTiming(PaymentOptionsSpark.DirectDebit);
            var testData = BuildCaravanEndorsement_IncreaseInPremium(PaymentScenario.AnnualBank, paymentAccount);

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanEndorseFlow(_browser, testData);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPaymentDetailsInShield(testData);
        }

        /// <summary>
        /// Endorse Caravan policy
        /// Payment plan: Annual Cash
        /// Premium change: None
        /// Endorsement effective: Within 14 days
        /// Change Make/Model: Yes
        /// Provide new random registration & Park Location: True
        /// Payment : new CC
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan)]
        [Test(Description = "Annual Cash Caravan Policy for Endorsement with Increase in Premium and Pay With new CC")]
        public void INSU_T57_CaravanEndorsement_AnnualCash_PremiumIncrease_PayWithNewCC()
        {
            var paymentAccount = new PaymentV2().CreditCard(DataHelper.RandomCreditCard()).Annual().PaymentTiming(PaymentOptionsSpark.AnnualCash);
            var testData = BuildCaravanEndorsement_IncreaseInPremium(PaymentScenario.AnnualCash, paymentAccount, SparkCommonConstants.RefundToSource.RefundToUnknownCreditCard);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanEndorseFlow(_browser, testData);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPaymentDetailsInShield(testData);
        }

        /// <summary>
        /// Endorse Caravan policy
        /// Payment plan: Monthly Installment
        /// Premium change: Increase
        /// Endorsement effective: Within 14 days
        /// Change Make/Model: Yes
        /// Provide new random registration & Park Location: True
        /// Payment : New BankAccount
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan)]
        [Test(Description = "Monthly Installment Caravan Policy for Endorsement with Increase in Premium and Pay from new BankAccount")]
        public void INSU_T62_CaravanEndorsement_MonthlyInstallment_PremiumIncrease_PayNewBankAccount()
        {
            var paymentAccount = new PaymentV2().BankAccount(new BankAccount().InitWithRandomValues()).Monthly().PaymentTiming(PaymentOptionsSpark.DirectDebit);
            var testData = BuildCaravanEndorsement_IncreaseInPremium(PaymentScenario.MonthlyBank, paymentAccount, RefundToSource.RefundToBankAccount);

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanEndorseFlow(_browser, testData);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPaymentDetailsInShield(testData);
        }

        /// <summary>
        /// Endorse Caravan policy
        /// Payment plan: Monthly Installment
        /// Premium change: Refund
        /// Endorsement effective: Within 14 days
        /// Change Make/Model: Yes
        /// Provide new random registration & Park Location: True/False
        /// Payment : New CC Added
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan)]
        [Test(Description = "Monthly Installment Caravan Policy for Endorsement with Refund and Pay with new CC")]
        public void INSU_T63_CaravanEndorsement_MonthlyInstallment_Refund_PayNewCC()
        {           
            var testData = BuildCaravanEndorsement_PayNewCC();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanEndorseFlow(_browser, testData);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPaymentDetailsInShield(testData);
        }

        /// <summary>
        /// Endorse Caravan policy
        /// Payment plan: Annual Cash
        /// Premium change: Refund
        /// Endorsement effective: Within 14 days
        /// Change Make/Model: Yes
        /// Provide new random registration & Park Location: True/False
        /// Payment : Refund to UnknownCC
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan)]
        [Test(Description = "Annual Cash Caravan Policy for Endorsement with Refund and Pay to Unknown CC")]
        public void INSU_T54_CaravanEndorsement_AnnualCash_Refund_UnknownCC()
        {
            var testData = BuildCaravanEndorsement_Refund(PaymentScenario.AnnualCash,null,RefundToSource.RefundToUnknownCreditCard);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanEndorseFlow(_browser, testData);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPaymentDetailsInShield(testData);
        }

        /// <summary>
        /// Endorse Caravan policy
        /// Payment plan: Annual Cash
        /// Premium change: Refund
        /// Endorsement effective: Within 14 days
        /// Change Make/Model: Yes
        /// Provide new random registration & Park Location: True/False
        /// Payment : Refund to knownCC
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan)]
        [Test(Description = "Annual Cash Caravan Policy for Endorsement with Refund and Pay to known CC")]
        public void INSU_T56_CaravanEndorsement_AnnualCash_Refund_knownCC()
        {
            var testData = BuildCaravanEndorsement_Refund(PaymentScenario.AnnualCash,null, RefundToSource.RefundToKnownCreditCard);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanEndorseFlow(_browser, testData);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPaymentDetailsInShield(testData);
        }

        /// <summary>
        /// Endorse Caravan policy
        /// Payment plan: Annual Installment
        /// Premium change: Refund
        /// Endorsement effective: Within 14 days
        /// Change Make/Model: Yes
        /// Provide new random registration & Park Location: True/False
        /// Payment : Refund to knownCC
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan)]
        [Test(Description = "Annual Installment Caravan Policy for Endorsement with Refund and Pay to Known CC")]
        public void INSU_T59_CaravanEndorsement_AnnualInstallment_Refund_KnownCC()
        {
            var testData = BuildCaravanEndorsement_Refund(PaymentScenario.AnnualCash, null, RefundToSource.RefundToKnownCreditCard);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanEndorseFlow(_browser, testData);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPaymentDetailsInShield(testData);
        }

        /// <summary>
        /// Endorse Caravan policy
        /// Payment plan: Annual Installment
        /// Premium change: Refund
        /// Endorsement effective: Within 14 days
        /// Change Make/Model: Yes
        /// Provide new random registration & Park Location: True/False
        /// Payment : Refund to known BankAccount
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan)]
        [Test(Description = "Annual Installment Caravan Policy for Endorsement with Refund and Pay to Known BankAccount")]
        public void INSU_T58_CaravanEndorsement_AnnualInstallment_Refund_KnownBankAccount()
        {
            var testData = BuildCaravanEndorsement_Refund(PaymentScenario.AnnualBank, null, RefundToSource.RefundToBankAccount);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanEndorseFlow(_browser, testData);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPaymentDetailsInShield(testData);
        }

        /// <summary>
        /// Endorse Caravan policy with detailed ui verification
        /// Payment plan: Annual Cash
        /// Premium change: Refund
        /// Endorsement effective: Within 14 days
        /// Change Make/Model: Yes
        /// Provide new random registration & Park Location: True/False
        /// Payment : Pay with new bank account
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.Caravan)]
        [Test(Description = "Annual Cash Caravan Policy for Endorsement with Refund and Pay new bank account")]
        public void INSU_T55_CaravanEndorsement_AnnualCash_Refund_AddNewBankAccount()
        {
            var paymentAccount = new PaymentV2()
                        .BankAccount(new BankAccount().InitWithRandomValues())
                        .Annual()
                        .PaymentTiming(PaymentOptionsSpark.DirectDebit);

            var testData = BuildCaravanEndorsement_Refund(PaymentScenario.AnnualCash, paymentAccount, RefundToSource.RefundToBankAccount);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsCaravanEndorsement.CaravanEndorseFlow(_browser, testData, detailUiChecking: true);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseCaravan.VerifyPolicyPaymentDetailsInShield(testData);
        }

        #endregion

        #region Test Case Helper Methods
        //TODO: AUNT-189-handling of future obfuscated member central data
        private EndorseCaravan BuildCaravanEndorsement_NoPremiumChange(PaymentScenario paymentScenario)
        {
            var testData = ShieldCaravanDB.FindCaravanPolicyForEndorsement(SparkCommonConstants.RefundToSource.None, ShieldCaravanDB.ProgramType.Caravan, valueMin: 0, valueMax: 200000, paymentScenario);

            var paymentAccount = new PaymentV2().PaymentTiming(PaymentOptionsSpark.DirectDebit);

            return new CaravanEndorsementBuilder()
                .InitialiseCaravanWithDefaultData(testData.PolicyNumber)
                .WithEndorsementStartDate(DataHelper.GenerateRandomDate(DateTime.Now, DateTime.Now.AddDays(13)))
                .WithRefundDestination(SparkCommonConstants.RefundToSource.None)
                .WithRandomCaravanRego()
                .WithExpectedImpactOnPremium(PremiumChange.NoChange)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        private EndorseCaravan BuildCaravanEndorsement_IncreaseInPremium(
            PaymentScenario paymentScenario, 
            PaymentV2 paymentAccount, 
            SparkCommonConstants.RefundToSource payment = SparkCommonConstants.RefundToSource.None)
        {
            var testData = ShieldCaravanDB.FindCaravanPolicyForEndorsement(payment, ShieldCaravanDB.ProgramType.Caravan, valueMin: 0, valueMax: 5000, paymentScenario);

            return new CaravanEndorsementBuilder()
                .InitialiseCaravanWithDefaultData(testData.PolicyNumber)
                .WithEndorsementStartDate(DataHelper.GenerateRandomDate(DateTime.Now, DateTime.Now.AddDays(13)))
                .WithRandomNewCaravan(minValue:6000)
                .WithRefundDestination(payment)
                .WithRandomCaravanRego()
                .WithExpectedImpactOnPremium(PremiumChange.PremiumIncrease)
                .WithContentCover("$5000")
                .WithRandomParkLocation()
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        private EndorseCaravan BuildCaravanEndorsement_PayNewCC()
        {
            
            var testData = ShieldCaravanDB.FindCaravanPolicyForEndorsement(SparkCommonConstants.RefundToSource.None, ShieldCaravanDB.ProgramType.Caravan, valueMin: 11000, valueMax: 150000, PaymentScenario.MonthlyCard);
            var paymentAccount = new PaymentV2().CreditCard(DataHelper.RandomCreditCard()).Monthly().PaymentTiming(PaymentOptionsSpark.DirectDebit);

            return new CaravanEndorsementBuilder()
                .InitialiseCaravanWithDefaultData(testData.PolicyNumber)
                .WithEndorsementStartDate(DataHelper.GenerateRandomDate(DateTime.Now, DateTime.Now.AddDays(13)))
                .WithRandomNewCaravan(minValue: 1000, maxValue: 10000)
                .WithRefundDestination(RefundToSource.RefundToKnownCreditCard)
                .WithRandomCaravanRego()
                .WithExpectedImpactOnPremium(PremiumChange.PremiumDecrease)
                .WithRandomParkLocation()
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        private EndorseCaravan BuildCaravanEndorsement_Refund(
                PaymentScenario paymentScenario,
                PaymentV2 paymentAccount,
                SparkCommonConstants.RefundToSource payment
                )
        {
            var testData = ShieldCaravanDB.FindCaravanPolicyForEndorsement(payment, ShieldCaravanDB.ProgramType.Caravan, valueMin: 11000, valueMax: 150000, paymentScenario);

            return new CaravanEndorsementBuilder()
                .InitialiseCaravanWithDefaultData(testData.PolicyNumber)
                .WithEndorsementStartDate(DataHelper.GenerateRandomDate(DateTime.Now, DateTime.Now.AddDays(13)))
                .WithRandomNewCaravan(minValue: 1000, maxValue: 10000)
                .WithRefundDestination(payment)
                .WithRandomCaravanRego()
                .WithExpectedImpactOnPremium(PremiumChange.PremiumDecrease)
                .WithRandomParkLocation()
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }
        #endregion
    }
}
