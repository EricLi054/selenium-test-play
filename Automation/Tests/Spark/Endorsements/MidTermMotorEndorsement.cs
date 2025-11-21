using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using Rac.TestAutomation.Common.DatabaseCalls.Queries.Environment;
using Rac.TestAutomation.Common.TestData.Endorsements;
using System;
using Tests.ActionsAndValidations;
using Tests.ActionsAndValidations.Endorsements;
using static Rac.TestAutomation.Common.Constants;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace Spark.Endorsements
{
    [Property("Functional", "Spark Midterm Motor Endorsements")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class MidTermMotorEndorsement : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Spark Midterm Motor Endorsements");
        }

        #region TestCases
        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.ChangeMyPolicy), Category(TestCategory.Endorsement),Category(TestCategory.Motor)]
        [Test(Description = "Annual Cash - No change to premium - Review and Confirm")]
        public void INSU_T87_MotorEndorsement_AnnualCash_NoPremiumChange()
        {
            var testData = BuildTestDataForAnnualCashNoPremiumChange();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorEndorseFlow(_browser, testData);
            ActionsMotorEndorsement.VerifyEndorsementConfirmationPage(_browser, testData, refundDestination: RefundToSource.None);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPaymentDetailsInShield(testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.ChangeMyPolicy), Category(TestCategory.Endorsement), Category(TestCategory.Motor)]
        [Test(Description = "Annual Cash - Refund - Option to add Bank Account")]
        public void INSU_T89_MotorEndorsement_AnnualCash_Refund_AddBankAccount()
        {
            var testData = BuildTestDataForAnnualCashRefund_AddBankAccount();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            //Refund to bank account
            ActionsMotorEndorsement.MotorEndorseFlow(_browser, testData, refundDestination: RefundToSource.RefundToBankAccount);
            //MFA for bank account input
            bool expectBankMFA = Config.Get().IsSparkEndorsementBankMultiFactorAuthenticationExpected();
            if (expectBankMFA)
            {
                ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber);
            }
            else
            {
                Reporting.Log($"{SparkFeatureToggles.SparkEndorsementsBankExpectMultiFactorAuthentication.GetDescription()} = {expectBankMFA} so we won't expect it here.");
            }
            
            ActionsMotorEndorsement.VerifyEndorsementConfirmationPage(_browser, testData, refundDestination: RefundToSource.RefundToBankAccount);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPaymentDetailsInShield(testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.ChangeMyPolicy), Category(TestCategory.Endorsement),Category(TestCategory.Motor)]
        [Test(Description = "Annual Cash - Refund - Known Credit Card")]
        public void INSU_T90_MotorEndorsement_AnnualCash_Refund_KnownCreditCard()
        {
            ShieldParametersDB.RequireShieldFeatureToggleForRefundIsTrue();
            var testData = BuildTestDataForAnnualCashRefund_KnownCC();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorEndorseFlow(_browser, testData, refundDestination: RefundToSource.RefundToKnownCreditCard);
            ActionsMotorEndorsement.VerifyEndorsementConfirmationPage(_browser, testData, refundDestination: RefundToSource.RefundToKnownCreditCard);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPaymentDetailsInShield(testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.ChangeMyPolicy), Category(TestCategory.Endorsement), Category(TestCategory.Motor)]
        [Test(Description = "Annual Cash - Refund - UnKnown Credit Card")]
        public void INSU_T88_MotorEndorsement_AnnualCash_Refund_UnKnownCreditCard()
        {
            ShieldParametersDB.RequireShieldFeatureToggleForRefundIsTrue();
            var testData = BuildTestDataForAnnualCashRefund_UnknownCC();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorEndorseFlow(_browser, testData, refundDestination: RefundToSource.RefundToUnknownCreditCard);
            ActionsMotorEndorsement.VerifyEndorsementConfirmationPage(_browser, testData, refundDestination: RefundToSource.RefundToUnknownCreditCard);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPaymentDetailsInShield(testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.ChangeMyPolicy), Category(TestCategory.Endorsement), Category(TestCategory.Motor)]
        [Test(Description = "Annual Cash - Increase in Premium -Pay With CreditCard Only")]
        public void INSU_T91_MotorEndorsement_AnnualCash_IncreasePremium_PayWithCreditCard()
        {
            var testData = BuildTestDataForAnnualCash_IncreasePremium_PayWithCC();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorEndorseFlow(_browser, testData, refundDestination: RefundToSource.RefundToUnknownCreditCard);
            ActionsMotorEndorsement.VerifyEndorsementConfirmationPage(_browser, testData, refundDestination: RefundToSource.RefundToUnknownCreditCard);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPaymentDetailsInShield(testData);
        }
        

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.ChangeMyPolicy), Category(TestCategory.Endorsement), Category(TestCategory.Motor)]
        [Test(Description = "Annual Installment - Refund - Existing Bank Account")]
        public void INSU_T92_MotorEndorsement_AnnualInstallment_Refund_BankAccount()
        {
            var testData = BuildTestDataForAnnualInstallmentRefund_BankAccount();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorEndorseFlow(_browser, testData, refundDestination: RefundToSource.RefundToBankAccount); 
            ActionsMotorEndorsement.VerifyEndorsementConfirmationPage(_browser, testData, refundDestination: RefundToSource.RefundToBankAccount);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPaymentDetailsInShield(testData);
        }


        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.ChangeMyPolicy), Category(TestCategory.Endorsement), Category(TestCategory.Motor)]
        [Test(Description = "Annual Installment - Refund - Existing Credit Card")]
        public void INSU_T93_MotorEndorsement_AnnualInstallment_Refund_KnownCreditCard()
        {
            ShieldParametersDB.RequireShieldFeatureToggleForRefundIsTrue();
            var testData = BuildTestDataForAnnualInstallmentRefund_KnownCreditCard();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorEndorseFlow(_browser, testData, refundDestination: RefundToSource.RefundToKnownCreditCard);
            ActionsMotorEndorsement.VerifyEndorsementConfirmationPage(_browser, testData, refundDestination: RefundToSource.RefundToKnownCreditCard);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPaymentDetailsInShield(testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.ChangeMyPolicy), Category(TestCategory.Endorsement), Category(TestCategory.Motor)]
        [Test(Description = "Annual Installment - No Premium Change")]
        public void INSU_T94_MotorEndorsement_AnnualInstallment_NoPremiumChange()
        {
            var testData = BuildTestDataForAnnualInstallment_NoPremiumChange();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorEndorseFlow(_browser, testData, refundDestination: RefundToSource.None);
            ActionsMotorEndorsement.VerifyEndorsementConfirmationPage(_browser, testData, refundDestination: RefundToSource.None);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPaymentDetailsInShield(testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.ChangeMyPolicy), Category(TestCategory.Endorsement), Category(TestCategory.Motor)]
        [Test(Description = "Monthly Installment - No Premium Change")]
        public void INSU_T98_MotorEndorsement_MonthlyInstallment_NoPremiumChange_SelectExistingAccount()
        {
            var testData = BuildTestDataForMonthlyInstallment_NoPremiumChange();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorEndorseFlow(_browser, testData, refundDestination: RefundToSource.None);
            ActionsMotorEndorsement.VerifyEndorsementConfirmationPage(_browser, testData, refundDestination: RefundToSource.None);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPaymentDetailsInShield(testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.ChangeMyPolicy), 
            Category(TestCategory.Endorsement), Category(TestCategory.Motor), Category(TestCategory.MultiFactorAuthentication)]
        [Test(Description = "Annual Installment - Increase in Premium - Pay with new Direct Debit")]
        public void INSU_T95_MotorEndorsement_AnnualInstallment_IncreasePremium_PayWithDirectDebit()
        {
            var testData = BuildTestDataForAnnualInstallment_IncreasePremium_PayDirectDebitViaBankAccount();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorEndorseFlow(_browser, testData, refundDestination: RefundToSource.RefundToBankAccount);
            //MFA for bank account input
            bool expectBankMFA = Config.Get().IsSparkEndorsementBankMultiFactorAuthenticationExpected();
            if (expectBankMFA)
            {
                ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber, retryOTP: true);
            }
            else
            {
                Reporting.Log($"{SparkFeatureToggles.SparkEndorsementsBankExpectMultiFactorAuthentication.GetDescription()} = {expectBankMFA} so we won't expect it here.");
            }            
            ActionsMotorEndorsement.VerifyEndorsementConfirmationPage(_browser, testData, refundDestination: RefundToSource.RefundToBankAccount);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPaymentDetailsInShield(testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.ChangeMyPolicy), Category(TestCategory.Endorsement), Category(TestCategory.Motor)]
        [Test(Description = "Monthly Installment - Increase in Premium - Add new Bank Account")]
        public void INSU_T96_MotorEndorsement_MonthlyInstallment_IncreaseInPremium_AddBankAccount()
        {
            var testData = BuildTestDataForMonthlyInstallment_IncreaseInPremium();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorEndorseFlow(_browser, testData, refundDestination: RefundToSource.RefundToBankAccount, detailUiChecking:true);
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
           
            ActionsMotorEndorsement.VerifyEndorsementConfirmationPage(_browser, testData, refundDestination: RefundToSource.RefundToBankAccount);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPaymentDetailsInShield(testData);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.ChangeMyPolicy), Category(TestCategory.Endorsement), Category(TestCategory.Motor)]
        [Test(Description = "Monthly Installment - Increase in Premium - Add new Credit Card")]
        public void INSU_T97_MotorEndorsement_MonthlyInstallment_Refund_AddCreditCard()
        {
            var testData = BuildTestDataForMonthlyInstallment_Refund();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMotorEndorsement.MotorEndorseFlow(_browser, testData, refundDestination: RefundToSource.RefundToKnownCreditCard);
            //MFA for bank account input
            bool expectBankMFA = Config.Get().IsSparkEndorsementBankMultiFactorAuthenticationExpected();
            if (expectBankMFA)
            {
                ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber);
            }
            else
            {
                Reporting.Log($"{SparkFeatureToggles.SparkEndorsementsBankExpectMultiFactorAuthentication.GetDescription()} = {expectBankMFA} so we won't expect it here.");
            }           
            ActionsMotorEndorsement.VerifyEndorsementConfirmationPage(_browser, testData, refundDestination: RefundToSource.RefundToKnownCreditCard);

            Reporting.Log("Opening PCM to verify the details of the endorsed policy");
            _browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPremiumAndParkingSuburb(_browser, testData);

            Reporting.LogTestShieldValidations("Policy Endorsment ", testData.PolicyNumber);
            VerifyEndorseMotor.VerifyPolicyPaymentDetailsInShield(testData);
        }

        #endregion

        #region Test Case Helper Methods
        /// <summary>
        /// Test Data setup for No premium change , Annual Cash, Any Cover Type
        /// </summary>
        private EndorseCar BuildTestDataForAnnualCashNoPremiumChange()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForChangeMyCar(requestedScenario: ShieldMotorDB.ChangeMyCarScenario.AnyValueForNoChange,
                                                refundDestination: SparkCommonConstants.RefundToSource.None, qasAddressPolicies: true, PaymentScenario.AnnualCash);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .WithUsage(VehicleUsage.Undefined)
                .WithRandomNewCar()
                .WithNewCarIsModified(isModified: true)
                .WithRandomNewCarRego()
                .WithPolicyStartDate(DateTime.Now)
                .WithAnnualKms(AnnualKms.UpTo10000)
                .WithExpectedImpactOnPremium(PremiumChange.NoChange)
                .Build();
        }

        /// <summary>
        /// Test Data setup for decrease in premium (Refund), Annual Cash, Refund to newly added bank account
        /// </summary>
        private EndorseCar BuildTestDataForAnnualCashRefund_AddBankAccount()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForChangeMyCar(requestedScenario: ShieldMotorDB.ChangeMyCarScenario.HighValueForDecreasePremium, 
                                                    refundDestination: SparkCommonConstants.RefundToSource.RefundToBankAccount, qasAddressPolicies: true, PaymentScenario.AnnualCash);

            var paymentAccount = new PaymentV2()
                                    .BankAccount(new BankAccount().InitWithRandomValues())
                                    .Annual()
                                    .PaymentTiming(PaymentOptionsSpark.DirectDebit);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(true)
                .WithUsage(VehicleUsage.Private)
                .WithRandomLowValueCar()
                .WithRandomNewCarRego()
                .WithPolicyStartDate(DateTime.Now)
                .WithAnnualKms(AnnualKms.LessThan5000)
                .WithExcess("$1500")
                .WithExpectedImpactOnPremium(PremiumChange.PremiumDecrease)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        /// <summary>
        /// Test Data setup for decrease in premium (Refund), Annual Cash, Refund to Known Credit Card
        /// </summary>
        private EndorseCar BuildTestDataForAnnualCashRefund_KnownCC()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForChangeMyCar(requestedScenario: ShieldMotorDB.ChangeMyCarScenario.HighValueForDecreasePremium,
                                                    refundDestination: SparkCommonConstants.RefundToSource.RefundToKnownCreditCard, qasAddressPolicies: true, PaymentScenario.AnnualCash);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(true)
                .WithUsage(VehicleUsage.Private)
                .WithRandomLowValueCar()
                .WithRandomNewCarRego()
                .WithPolicyStartDate(DateTime.Now)
                .WithAnnualKms(AnnualKms.LessThan5000)
                .WithExcess("$2000")
                .WithExpectedImpactOnPremium(PremiumChange.PremiumDecrease)
                .Build();
        }

        /// <summary>
        /// Test Data setup for decrease in premium (Refund), Annual Cash, Refund to Un Known Credit Card
        /// </summary>
        private EndorseCar BuildTestDataForAnnualCashRefund_UnknownCC()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForChangeMyCar(requestedScenario: ShieldMotorDB.ChangeMyCarScenario.HighValueForDecreasePremium,
                                                    refundDestination: SparkCommonConstants.RefundToSource.RefundToUnknownCreditCard, qasAddressPolicies: true, PaymentScenario.AnnualCash);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(true)
                .WithUsage(VehicleUsage.Private)
                .WithRandomLowValueCar()
                .WithRandomNewCarRego()
                .WithPolicyStartDate(DateTime.Now.AddDays(5))
                .WithAnnualKms(AnnualKms.LessThan5000)
                .WithExcess("$2000")
                .WithExpectedImpactOnPremium(PremiumChange.PremiumDecrease)
                .Build();
        }

        /// <summary>
        /// Test Data setup for Increase in Premium, Annual Cash, Pay with CC
        /// </summary>
        private EndorseCar BuildTestDataForAnnualCash_IncreasePremium_PayWithCC()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForChangeMyCar(requestedScenario: ShieldMotorDB.ChangeMyCarScenario.LowValueForIncreasePremium,
                                                    refundDestination: SparkCommonConstants.RefundToSource.None, qasAddressPolicies: true, PaymentScenario.AnnualCash);

            var paymentAccount = new PaymentV2()
               .CreditCard(DataHelper.RandomCreditCard())
               .Annual()
               .PaymentTiming(PaymentOptionsSpark.AnnualCash);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(true)
                .WithUsage(VehicleUsage.Business)
                .WithRandomHighValueCar()
                .WithRandomNewCarFinancier()
                .WithRandomNewCarRego()
                .WithPolicyStartDate(DateTime.Now)
                .WithAnnualKms(AnnualKms.MoreThan20000)
                .WithExcess("$0")
                .WithExpectedImpactOnPremium(PremiumChange.PremiumIncrease)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }        

        /// <summary>
        /// Test Data setup for decrease in premium (Refund), Annual Installment, Refund to existing bank account
        /// </summary>
        private EndorseCar BuildTestDataForAnnualInstallmentRefund_BankAccount()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForChangeMyCar(requestedScenario: ShieldMotorDB.ChangeMyCarScenario.HighValueForDecreasePremium,
                                                    refundDestination: SparkCommonConstants.RefundToSource.RefundToBankAccount, qasAddressPolicies: true, PaymentScenario.AnnualBank);

            var paymentAccount = new PaymentV2()
                                    .BankAccount(new BankAccount().InitWithRandomValues())
                                    .Annual()
                                    .PaymentTiming(PaymentOptionsSpark.DirectDebit);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(true)
                .WithUsage(VehicleUsage.Private)
                .WithRandomLowValueCar()
                .WithRandomNewCarRego()
                .WithPolicyStartDate(DateTime.Now)
                .WithAnnualKms(AnnualKms.LessThan5000)
                .WithExcess("$1500")
                .WithExpectedImpactOnPremium(PremiumChange.PremiumDecrease)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        /// <summary>
        /// Test Data setup for decrease in premium (Refund), Annual Installment, Refund to Known Credit Card
        /// </summary>
        private EndorseCar BuildTestDataForAnnualInstallmentRefund_KnownCreditCard()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForChangeMyCar(requestedScenario: ShieldMotorDB.ChangeMyCarScenario.HighValueForDecreasePremium,
                                                    refundDestination: SparkCommonConstants.RefundToSource.RefundToKnownCreditCard, qasAddressPolicies: true, PaymentScenario.AnnualCash);

            var paymentAccount = new PaymentV2()
                                    .CreditCard(DataHelper.RandomCreditCard())
                                    .Annual()
                                    .PaymentTiming(PaymentOptionsSpark.AnnualCash);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(true)
                .WithUsage(VehicleUsage.Private)
                .WithRandomLowValueCar()
                .WithRandomNewCarRego()
                .WithPolicyStartDate(DateTime.Now)
                .WithAnnualKms(AnnualKms.LessThan5000)
                .WithExcess("$1500")
                .WithExpectedImpactOnPremium(PremiumChange.PremiumDecrease)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        /// <summary>
        /// Test Data setup for no premium change Annual Installment
        /// </summary>
        private EndorseCar BuildTestDataForAnnualInstallment_NoPremiumChange()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForChangeMyCar(requestedScenario: ShieldMotorDB.ChangeMyCarScenario.AnyValueForNoChange,
                                                    refundDestination: SparkCommonConstants.RefundToSource.None, qasAddressPolicies: true, PaymentScenario.AnnualBank);

            var paymentAccount = new PaymentV2().PaymentTiming(PaymentOptionsSpark.DirectDebit);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .WithUsage(VehicleUsage.Undefined)
                .WithRandomNewCar()
                .WithRandomNewCarRego()
                .WithPolicyStartDate(DateTime.Now)
                .WithAnnualKms(AnnualKms.UpTo10000)
                .WithExpectedImpactOnPremium(PremiumChange.NoChange)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        /// <summary>
        /// Test Data setup for no premium change monthly Installment
        /// </summary>
        private EndorseCar BuildTestDataForMonthlyInstallment_NoPremiumChange()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForChangeMyCar(requestedScenario: ShieldMotorDB.ChangeMyCarScenario.AnyValueForNoChange,
                                                    refundDestination: SparkCommonConstants.RefundToSource.None, qasAddressPolicies: true, PaymentScenario.MonthlyCard, PaymentScenario.MonthlyBank);

            var paymentAccount = new PaymentV2().PaymentTiming(PaymentOptionsSpark.DirectDebit);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .WithUsage(VehicleUsage.Undefined)
                .ChangeCarMakeAndModel(false)
                .WithRandomNewCar()
                .WithRandomNewCarRego()
                .WithPolicyStartDate(DateTime.Now)
                .WithAnnualKms(AnnualKms.UpTo10000)
                .WithExpectedImpactOnPremium(PremiumChange.NoChange)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        /// <summary>
        /// Test Data setup for Increase in premium monthly Installment
        /// </summary>
        private EndorseCar BuildTestDataForMonthlyInstallment_IncreaseInPremium()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForChangeMyCar(requestedScenario: ShieldMotorDB.ChangeMyCarScenario.LowValueForIncreasePremium,
                                                    refundDestination: SparkCommonConstants.RefundToSource.None, qasAddressPolicies: true, PaymentScenario.MonthlyBank);

            var paymentAccount = new PaymentV2()
                                   .BankAccount(new BankAccount().InitWithRandomValues())
                                   .Monthly()
                                   .PaymentTiming(PaymentOptionsSpark.DirectDebit);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(true)
                .WithUsage(VehicleUsage.Business)
                .WithRandomHighValueCar()
                .WithRandomNewCarRego()
                .WithPolicyStartDate(DateTime.Now)
                .WithAnnualKms(AnnualKms.UpTo20000)
                .WithExpectedImpactOnPremium(PremiumChange.PremiumIncrease)
                .WithExcess("$0")
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        /// <summary>
        /// Test Data setup for refund monthly Installment
        /// </summary>
        private EndorseCar BuildTestDataForMonthlyInstallment_Refund()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForChangeMyCar(requestedScenario: ShieldMotorDB.ChangeMyCarScenario.HighValueForDecreasePremium,
                                                    refundDestination: SparkCommonConstants.RefundToSource.None, qasAddressPolicies: true, PaymentScenario.MonthlyCard);

            var paymentAccount = new PaymentV2()
                                    .CreditCard(DataHelper.RandomCreditCard())
                                    .Monthly()
                                    .PaymentTiming(PaymentOptionsSpark.DirectDebit);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(true)
                .WithUsage(VehicleUsage.Private)
                .WithRandomLowValueCar()
                .WithRandomNewCarRego()
                .WithPolicyStartDate(DateTime.Now)
                .WithAnnualKms(AnnualKms.LessThan5000)
                .WithExpectedImpactOnPremium(PremiumChange.PremiumDecrease)
                .WithExcess("$2000")
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }

        /// <summary>
        /// Test Data setup for Increase in Premium, Annual Installment, Pay with Bank Account
        /// </summary>
        private EndorseCar BuildTestDataForAnnualInstallment_IncreasePremium_PayDirectDebitViaBankAccount()
        {
            var testData = ShieldMotorDB.FindMotorPolicyForChangeMyCar(requestedScenario: ShieldMotorDB.ChangeMyCarScenario.LowValueForIncreasePremium,
                                                    refundDestination: SparkCommonConstants.RefundToSource.None, qasAddressPolicies: true, PaymentScenario.AnnualBank);

            var paymentAccount = new PaymentV2()
                                    .BankAccount(new BankAccount().InitWithRandomValues())
                                    .Annual()
                                    .PaymentTiming(PaymentOptionsSpark.DirectDebit);

            return new MotorEndorsementBuilder()
                .WithPolicyData(testData)
                .ChangeCarMakeAndModel(true)
                .WithUsage(VehicleUsage.Business)
                .WithRandomHighValueCar()
                .WithRandomNewCarFinancier()
                .WithRandomNewCarRego()
                .WithPolicyStartDate(DateTime.Now.AddDays(13))
                .WithAnnualKms(AnnualKms.MoreThan20000)
                .WithExcess("$0")
                .WithExpectedImpactOnPremium(PremiumChange.PremiumIncrease)
                .WithSparkPaymentChoice(paymentAccount)
                .Build();
        }
        #endregion
    }
}
