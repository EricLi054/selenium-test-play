using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using Rac.TestAutomation.Common.TestData.Endorsements;
using System;
using System.Collections.Generic;
using Tests.ActionsAndValidations.Endorsements;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static UIDriver.Pages.Spark.Endorsements.PayYourRenewalPremium.Constants;

namespace Spark.Endorsements
{
    [Property("Functional", "Spark Make A Payment")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class MakePayment : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Spark Make A Payment");
        }

        #region TestCases
        [Category(TestCategory.MakePayment),Category(TestCategory.Regression),Category(TestCategory.Spark),Category(TestCategory.Endorsement),Category(TestCategory.RejectedInstalment)]
        [Test(Description = "Motor Renewal Or Midterm Or NB Annual DD Policy having Rejected Installment")]
        public void INSU_T65_Motor_RenewalOrMidtermOrNB_AnnualDD_RejectedInstallment()
        {
            var testData = BuildTestDataForPayPolicy_Motor(MakePaymentScenarioType.ANY, null, 
                new PaymentV2().CreditCard(DataHelper.RandomCreditCard()), isRejectedPayment: true);

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMakePayment.MakePayment(_browser, testData, detailUICheck: true,isFailedPayment:false);

            VerifyMakePayment.VerifyPolicyEligibilityForMakePaymentInPCM(_browser, testData.ActivePolicyHolder.Id, testData.PolicyNumber, isFailedPayment: false);
            VerifyMakePayment.VerifyShieldAfterMakePayment(testData, isFailedPayment: false);
        }

        [Category(TestCategory.MakePayment), Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement)]
        [Test(Description = "Motor Mid Term Annual Cash Policy having Submitted Installment")]
        public void INSU_T66_Motor_MidTerm_AnnualCash_SubmittedInstallment()
        {
            var testData = BuildTestDataForPayPolicy_Motor(MakePaymentScenarioType.MID_TERM,
                    new List<InstallmentStatus>
                    {
                        InstallmentStatus.Submitted
                    },
                    new PaymentV2().CreditCard(DataHelper.RandomCreditCard())
            );

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMakePayment.MakePayment(_browser, testData, detailUICheck:false, isFailedPayment: false);

            VerifyMakePayment.VerifyPolicyEligibilityForMakePaymentInPCM(_browser, testData.ActivePolicyHolder.Id, testData.PolicyNumber, isFailedPayment: false);
            VerifyMakePayment.VerifyShieldAfterMakePayment(testData, isFailedPayment: false);
        }

        [Category(TestCategory.MakePayment), Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement)]
        [Test(Description = "Home Renewal Annual Cash Policy having Pending Installment")]
        public void INSU_T67_Home_Renewal_AnnualCash_PendingInstallment()
        {
            var testData = BuildTestDataForPayPolicy_Home(MakePaymentScenarioType.RENEWAL,
                    new List<InstallmentStatus>
                    {
                        InstallmentStatus.Pending,
                    }
            );

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMakePayment.MakePayment(_browser, testData, detailUICheck: true, isFailedPayment: false);

            VerifyMakePayment.VerifyPolicyEligibilityForMakePaymentInPCM(_browser, testData.ActivePolicyHolder.Id, testData.PolicyNumber, isFailedPayment: false);
            VerifyMakePayment.VerifyShieldAfterMakePayment(testData, isFailedPayment: false);
        }

        [Category(TestCategory.MakePayment), Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement), Category(TestCategory.RejectedInstalment)]
        [Test(Description = "Home Mid Term Annual Cash Policy having Submitted Installment")]
        public void INSU_T68_Home_RenewalOrMidtermOrNB_AnnualCC_RejectedInstallment()
        {
            var testData = BuildTestDataForPayPolicy_Home(MakePaymentScenarioType.ANY, installmentStatus: null, isRejectedPayment: true);

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMakePayment.MakePayment(_browser, testData, detailUICheck: false, isFailedPayment: false);

            VerifyMakePayment.VerifyPolicyEligibilityForMakePaymentInPCM(_browser, testData.ActivePolicyHolder.Id, testData.PolicyNumber, isFailedPayment: false);
            VerifyMakePayment.VerifyShieldAfterMakePayment(testData, isFailedPayment: false);
        }

        [Category(TestCategory.MakePayment), Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement)]
        [Test(Description = "MotorCycle Renewal Or Midterm Or NB Annual Cash Policy having Pending Or Submitted Installment")]
        public void INSU_T69_MotorCycle_RenewalOrMidtermOrNB_AnnualCash_PendingOrSubmittedInstallment()
        {
            var testData = BuildTestDataForPayPolicy_MotorCycle_RenewalOrMidTermOrNB_SubmittedOrPending();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMakePayment.MakePayment(_browser, testData, detailUICheck: false, isFailedPayment: false);

            VerifyMakePayment.VerifyPolicyEligibilityForMakePaymentInPCM(_browser, testData.ActivePolicyHolder.Id, testData.PolicyNumber, isFailedPayment: false);
            VerifyMakePayment.VerifyShieldAfterMakePayment(testData, isFailedPayment: false);
        }

        [Category(TestCategory.MakePayment), Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement)]
        [Test(Description = "Pet Renewal Or Midterm Or NB Annual Cash Policy having Pending Or Submitted Installment")]
        public void INSU_T73_Pet_RenewalOrMidtermOrNB_AnnualCash_PendingOrSubmittedInstallment()
        {
            var testData = BuildTestDataForPayPolicy_Pet_RenewalOrMidTermOrNB_SubmittedOrPending();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMakePayment.MakePayment(_browser, testData, detailUICheck: true, isFailedPayment: false);

            VerifyMakePayment.VerifyPolicyEligibilityForMakePaymentInPCM(_browser, testData.ActivePolicyHolder.Id, testData.PolicyNumber, isFailedPayment: false);
            VerifyMakePayment.VerifyShieldAfterMakePayment(testData, isFailedPayment: false);
        }

        [Category(TestCategory.MakePayment), Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement)]
        [Test(Description = "Caravan Renewal Or Midterm Or NB Annual Cash Policy having Pending Or Submitted Installment")]
        public void INSU_T70_Caravan_RenewalOrMidtermOrNB_AnnualCash_PendingOrSubmittedInstallment()
        {
            var testData = BuildTestDataForPayPolicy_Caravan_RenewalOrMidTermOrNB_SubmittedOrPending();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMakePayment.MakePayment(_browser, testData, detailUICheck: true, isFailedPayment: false);

            VerifyMakePayment.VerifyPolicyEligibilityForMakePaymentInPCM(_browser, testData.ActivePolicyHolder.Id, testData.PolicyNumber, isFailedPayment: false);
            VerifyMakePayment.VerifyShieldAfterMakePayment(testData, isFailedPayment: false);
        }

        [Category(TestCategory.MakePayment), Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement)]
        [Test(Description = "Motor Renewal Annual Cash Policy having Pending Or Submitted Installment for Payment Fail and Retry")]
        public void INSU_T71_Motor_Renewal_PendingOrSubmittedInstallment_AnnualCash_PaymentFailAndRetry()
        {
            var testData = BuildTestDataForPayPolicy_Motor(MakePaymentScenarioType.RENEWAL, new List<InstallmentStatus>
                    {
                        InstallmentStatus.Pending,
                        InstallmentStatus.Submitted
                    },
                    new PaymentV2().CreditCard(DataHelper.RandomCreditCard(isNotSufficientFundCard: true))
            );

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMakePayment.MakePayment(_browser, testData, detailUICheck: false, isFailedPayment:true);

            VerifyMakePayment.VerifyPolicyEligibilityForMakePaymentInPCM(_browser, testData.ActivePolicyHolder.Id, testData.PolicyNumber,isFailedPayment:true);
            VerifyMakePayment.VerifyShieldAfterMakePayment(testData, isFailedPayment: true);
        }

        [Category(TestCategory.MakePayment), Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement)]
        [Test(Description = "Boat Renewal Or Midterm Or NB Annual Cash Policy having Pending Or Submitted Installment")]
        public void INSU_T72_Boat_RenewalOrMidtermOrNB_AnnualCash_PendingOrSubmittedInstallment()
        {
            var testData = BuildTestDataForPayPolicy_Boat_RenewalOrMidTermOrNB_SubmittedOrPending();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMakePayment.MakePayment(_browser, testData, detailUICheck: true, isFailedPayment: false);

            VerifyMakePayment.VerifyPolicyEligibilityForMakePaymentInPCM(_browser, testData.ActivePolicyHolder.Id, testData.PolicyNumber, isFailedPayment: false);
            VerifyMakePayment.VerifyShieldAfterMakePayment(testData, isFailedPayment: false);
        }

        [Category(TestCategory.MakePayment), Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.Endorsement)]
        [Test(Description = "Electric Mobility Renewal Or Midterm Or NB Annual Cash Policy having Pending Or Submitted Installment")]
        public void INSU_T74_ElectricMobility_RenewalOrMidtermOrNB_AnnualCash_PendingOrSubmittedInstallment()
        {
            var testData = BuildTestDataForPayPolicy_ElectricMobility_RenewalOrMidTermOrNB_SubmittedOrPending();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionsMakePayment.MakePayment(_browser, testData, detailUICheck: false, isFailedPayment: false);

            VerifyMakePayment.VerifyPolicyEligibilityForMakePaymentInPCM(_browser, testData.ActivePolicyHolder.Id, testData.PolicyNumber, isFailedPayment: false);
            VerifyMakePayment.VerifyShieldAfterMakePayment(testData, isFailedPayment: false);
        }

        #endregion

        #region Test Case Helper Methods
        private EndorseHome BuildTestDataForPayPolicy_Home(MakePaymentScenarioType makePaymentScenarioType, List<InstallmentStatus> installmentStatus,bool isRejectedPayment = false)
        {
            EndorseHome testData = null;
            List<string> policies=null;

            if (isRejectedPayment)
            {
                policies = ShieldPolicyDB.FindRejectedInstallmentForPayNow(ShieldProductType.HGP, PaymentScenario.AnnualCard);
                foreach (var policy in policies)
                {
                    var policyDetails = DataHelper.GetPolicyDetails(policy);

                    if (!policyDetails.RealTimePaymentDetails.IsEligibleForRealTime)
                    {
                        Reporting.Log($"Shield indicates that Policy {policy} is NOT eligible for real time payments. Skipping.");
                        continue;
                    }

                    if (ShieldPolicyDB.PolicyHasPendingInstallmentsRelativeToDate(policyDetails.PolicyNumber, DateTime.Now))
                    { continue; }

                    var contact = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);
                    if (contact != null)
                    {
                        testData = new HomeEndorsementBuilder().InitialiseHomeWithDefaultData(policy, contact)
                                  .WithSparkPaymentChoice(new PaymentV2().CreditCard(DataHelper.RandomCreditCard()))
                                  .Build();
                        break;
                    }
                }
            }
            else
            {
                policies = ShieldPolicyDB.FindPolicyForPayNow(ShieldProductType.HGP, makePaymentScenarioType, installmentStatus);
                foreach (var policy in policies)
                {
                    var policyDetails = DataHelper.GetPolicyDetails(policy);
                    var contact = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                    if (contact != null && !ShieldPolicyDB.PolicyHasBadInstallments(policy) &&
                        !ShieldPolicyDB.PolicyHasPendingInstallmentsRelativeToDate(policy, DateTime.Now))
                    {
                        testData = new HomeEndorsementBuilder().InitialiseHomeWithDefaultData(policy, contact)
                                  .WithSparkPaymentChoice(new PaymentV2().CreditCard(DataHelper.RandomCreditCard()))
                                  .Build();
                        break;
                    }

                }
            }

            Reporting.IsNotNull(testData, "that we found test data. If null it means Shield does not have any policies in the required conditions");
            return testData;
        }
        
        private EndorseCar BuildTestDataForPayPolicy_Motor(MakePaymentScenarioType makePaymentScenarioType, List<InstallmentStatus> installmentStatus,PaymentV2 payment, bool isRejectedPayment=false)
        {
            EndorseCar testData = null;
            List<string> policies = null;

            if (isRejectedPayment)
            {
                policies = ShieldPolicyDB.FindRejectedInstallmentForPayNow(ShieldProductType.MGP, PaymentScenario.AnnualBank);

                foreach (var policy in policies)
                {
                    var policyDetails = DataHelper.GetPolicyDetails(policy);

                    if (!policyDetails.RealTimePaymentDetails.IsEligibleForRealTime)
                    {
                        Reporting.Log($"Shield indicates that Policy {policy} is NOT eligible for real time payments. Skipping.");
                        continue;
                    }

                    if (ShieldPolicyDB.PolicyHasPendingInstallmentsRelativeToDate(policyDetails.PolicyNumber, DateTime.Now))
                    { continue; }

                    var contact = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);
                    if (contact != null)
                    {
                        testData = new MotorEndorsementBuilder().InitialiseMotorCarWithDefaultData(policy, contact)
                                  .WithSparkPaymentChoice(payment)
                                  .Build();
                        break;
                    }
                }
            }
            else
            {
                policies = ShieldPolicyDB.FindPolicyForPayNow(ShieldProductType.MGP, makePaymentScenarioType, installmentStatus);

                foreach (var policy in policies)
                {
                    var policyDetails = DataHelper.GetPolicyDetails(policy);
                    var contact = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                    if (contact != null && !ShieldPolicyDB.PolicyHasBadInstallments(policy) &&
                        (installmentStatus == null || installmentStatus.Contains(InstallmentStatus.Pending) ||
                        !ShieldPolicyDB.PolicyHasPendingInstallmentsRelativeToDate(policy, DateTime.Now)))
                    {
                        testData = new MotorEndorsementBuilder().InitialiseMotorCarWithDefaultData(policy, contact)
                                  .WithSparkPaymentChoice(payment)
                                  .Build();
                        break;
                    }
                }
            }

            Reporting.IsNotNull(testData, "that we found test data. If null it means Shield does not have any policies in the required conditions");
            return testData;
        }

        private EndorseCaravan BuildTestDataForPayPolicy_Caravan_RenewalOrMidTermOrNB_SubmittedOrPending()
        {
            EndorseCaravan testData = null;
            var policies = ShieldPolicyDB.FindPolicyForPayNow(ShieldProductType.MGV, MakePaymentScenarioType.ANY,
                    new List<InstallmentStatus>
                    {
                        InstallmentStatus.Pending,
                        InstallmentStatus.Submitted
                    }
                );

            foreach (var policy in policies)
            {
                var policyDetails = DataHelper.GetPolicyDetails(policy);
                var contact = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                if (contact != null && !ShieldPolicyDB.PolicyHasBadInstallments(policy) &&
                    !ShieldPolicyDB.PolicyHasPendingInstallmentsRelativeToDate(policy, DateTime.Now))
                {
                    testData = new CaravanEndorsementBuilder().InitialiseCaravanWithDefaultData(policy, contact)
                              .WithSparkPaymentChoice(new PaymentV2().CreditCard(DataHelper.RandomCreditCard()))
                              .Build();
                    break;
                }
                
            }

            return testData;
        }

        private EndorsePet BuildTestDataForPayPolicy_Pet_RenewalOrMidTermOrNB_SubmittedOrPending()
        {
            EndorsePet testData = null;
            var policies = ShieldPolicyDB.FindPolicyForPayNow(ShieldProductType.PET, MakePaymentScenarioType.ANY,
                    new List<InstallmentStatus>
                    {
                        InstallmentStatus.Pending,
                        InstallmentStatus.Submitted
                    }
                );

            foreach (var policy in policies)
            {
                var policyDetails = DataHelper.GetPolicyDetails(policy);
                var contact = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                if (contact != null && !ShieldPolicyDB.PolicyHasBadInstallments(policy) &&
                    !ShieldPolicyDB.PolicyHasPendingInstallmentsRelativeToDate(policy, DateTime.Now))
                {
                    testData = new PetEndorsementBuilder().InitialisePetWithDefaultData(policy, contact)
                              .WithSparkPaymentChoice(new PaymentV2().CreditCard(DataHelper.RandomCreditCard()))
                              .Build();
                    break;
                }

            }
            return testData;
        }

        private EndorseElectricMobility BuildTestDataForPayPolicy_ElectricMobility_RenewalOrMidTermOrNB_SubmittedOrPending()
        {
            EndorseElectricMobility testData = null;
            var policies = ShieldPolicyDB.FindPolicyForPayNow(ShieldProductType.MGE, MakePaymentScenarioType.ANY,
                    new List<InstallmentStatus>
                    {
                        InstallmentStatus.Pending,
                        InstallmentStatus.Submitted
                    }
                );

            foreach (var policy in policies)
            {
                var policyDetails = DataHelper.GetPolicyDetails(policy);
                var contact = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                if (contact != null && !ShieldPolicyDB.PolicyHasBadInstallments(policy) &&
                    !ShieldPolicyDB.PolicyHasPendingInstallmentsRelativeToDate(policy, DateTime.Now))
                {
                    testData = new ElectricMobilityEndorsementBuilder().InitialisePetWithDefaultData(policy, contact)
                              .WithSparkPaymentChoice(new PaymentV2().CreditCard(DataHelper.RandomCreditCard()))
                              .Build();
                    break;
                }
            }

            return testData;
        }

        private EndorseMotorCycle BuildTestDataForPayPolicy_MotorCycle_RenewalOrMidTermOrNB_SubmittedOrPending()
        {
            EndorseMotorCycle testData = null;
            var policies = ShieldPolicyDB.FindPolicyForPayNow(ShieldProductType.MGC, MakePaymentScenarioType.ANY,
                    new List<InstallmentStatus>
                    {
                        InstallmentStatus.Pending,
                        InstallmentStatus.Submitted
                    }
                );

            foreach (var policy in policies)
            {
                var policyDetails = DataHelper.GetPolicyDetails(policy);
                var contact = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                if (contact != null && !ShieldPolicyDB.PolicyHasBadInstallments(policy) &&
                    !ShieldPolicyDB.PolicyHasPendingInstallmentsRelativeToDate(policy, DateTime.Now))
                {
                    testData = new MotorCycleEndorsementBuilder().InitialiseMotorCycleWithDefaultData(policy, contact)
                              .WithSparkPaymentChoice(new PaymentV2().CreditCard(DataHelper.RandomCreditCard()))
                              .Build();
                    break;
                }
            }

            return testData;
        }

        private EndorseBoat BuildTestDataForPayPolicy_Boat_RenewalOrMidTermOrNB_SubmittedOrPending()
        {
            EndorseBoat testData = null;
            var policies = ShieldPolicyDB.FindPolicyForPayNow(ShieldProductType.BGP, MakePaymentScenarioType.ANY,
                    new List<InstallmentStatus>
                    {
                        InstallmentStatus.Pending,
                        InstallmentStatus.Submitted
                    }
                );

            foreach (var policy in policies)
            {
                var policyDetails = DataHelper.GetPolicyDetails(policy);
                var contact = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                if (contact != null && !ShieldPolicyDB.PolicyHasBadInstallments(policy) &&
                    !ShieldPolicyDB.PolicyHasPendingInstallmentsRelativeToDate(policy, DateTime.Now))
                {
                    testData = new BoatEndorsementBuilder().InitialiseBoatWithDefaultData(policy, contact)
                              .WithSparkPaymentChoice(new PaymentV2().CreditCard(DataHelper.RandomCreditCard()))
                              .Build();
                    break;
                }
            }

            return testData;
        }
        #endregion
    }
}