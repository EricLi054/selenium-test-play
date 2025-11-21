using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Endorsements;
using Tests.ActionsAndValidations;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using System;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace B2C.EndorsePolicy
{
    [Property("Functional", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class HomePolicyChangeHomeDetails : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C mandatory regression tests for home endorsements involving changing the home details");
        }

        /// <summary>
        /// Mandatory regression test case "B2C Logged In - Update Home Details - TC01 - Homeowners Premium Increase Instalment"
        /// Covers the scenario of a homeowner changing their home details 
        /// (address primarily) of their policy to a new address with a higher risk rating, resulting in an increase in premium.
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Endorsement), Category(TestCategory.Home), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.Mock_Member_Central_Support), Category(TestCategory.B2CPCM)]
        public void INSU_T23_B2CLoggedIn_ChangeMyHomeDetails_TC01_HomeOwnersIncreasePremium()
        {
            var testData = BuildTestDataForTC01ChangeMyHomeDetails();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            Reporting.LogTestStart();
            var originalpolicyValues = ActionsEndorseHome.PerformChangeMyHomeDetailsEndorsement(_browser, testData);

            Reporting.LogTestShieldValidations("home policy after endorsement", testData.PolicyNumber);
            VerifyEndorseHome.VerifyPolicyPremiumAndHomeDetailsAfterEndorsement(_browser, testData, originalpolicyValues);
            VerifyEndorseHome.VerifyPolicyAndHomeDetailsInShield(testData, originalpolicyValues);
            VerifyEndorseHome.VerifyPolicyPaymentDetailsInShield(testData, originalpolicyValues);
        }

        /// <summary>
        /// Mandatory regression test case "Update Home Details - TC14 - Change Combo - Landlord - Building Only - Refund Premium - DD - Monthly"
        /// Covers the scenario of a landlord's building cover type changing their home details 
        /// (address primarily) of their policy to a new address with a lower risk rating, resulting in a decrease in premium.
        /// As the policy is paid in installments, then there should be no prompt for any additional immediate payment.
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Endorsement), Category(TestCategory.Home), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.Mock_Member_Central_Support), Category(TestCategory.B2CPCM)]
        public void INSU_T25_B2CLoggedIn_ChangeMyHomeDetails_TC14_LandlordsBuildingDecreasePremium()
        {
            var testData = BuildTestDataForTC14ChangeMyHomeDetails();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            Reporting.LogTestStart();
            var originalpolicyValues = ActionsEndorseHome.PerformChangeMyHomeDetailsEndorsement(_browser, testData);

            Reporting.LogTestShieldValidations("home policy after endorsement", testData.PolicyNumber);
            VerifyEndorseHome.VerifyPolicyPremiumAndHomeDetailsAfterEndorsement(_browser, testData, originalpolicyValues);
            VerifyEndorseHome.VerifyPolicyAndHomeDetailsInShield(testData, originalpolicyValues);
            VerifyEndorseHome.VerifyPolicyPaymentDetailsInShield(testData, originalpolicyValues);
        }

        private EndorseHome BuildTestDataForTC01ChangeMyHomeDetails()
        {
            EndorseHome testData = null;
            var policies = ShieldHomeDB.FindHomePolicyForChangeMyHomeDetails(ShieldHomeDB.ChangeMyHomeDetailsScenario.LowRiskForIncreasePremium);

            foreach (var policy in policies)
            {
                if (!ShieldPolicyDB.IsPolicySuitableForEndorsements(policy))
                { continue; }

                var policyDetails = DataHelper.GetPolicyDetails(policy);
                var policyHolder = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                if (policyHolder != null)
                {
                    // TODO: RAI-167 Tech debt: Instead of using a static high/low risk address, look at dynamically using high/low risk address
                    var highRiskAddress = new Address(streetNumber: "81", streetName: "York Rd", suburb: "Carrabin", postcode: "6423");

                    testData = new HomeEndorsementBuilder().InitialiseHomeWithDefaultData(policy, policyHolder)
                                                    .WithPropertyAddress(highRiskAddress)
                                                    .WithAlarmSystem(Alarm.NoAlarm)
                                                    .WithRandomFinancier()
                                                    .WithExcess("200", null) // Changing building excess $200, leaving contents excess as is
                                                    .WithExpectedImpactOnPremium(PremiumChange.PremiumIncrease)
                                                    .WithRandomBuildingSumInsured()
                                                    .WithRandomContentsSumInsured()
                                                    .Build();
                    break;
                }
            }

            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;            

        }

        private EndorseHome BuildTestDataForTC14ChangeMyHomeDetails()
        {
            EndorseHome testData = null;
            var policies = ShieldHomeDB.FindHomePolicyForChangeMyHomeDetails(ShieldHomeDB.ChangeMyHomeDetailsScenario.HighRiskForDecreasePremium);

            foreach (var policy in policies)
            {
                if (!ShieldPolicyDB.IsPolicySuitableForEndorsements(policy))
                { continue; }

                var policyDetails = DataHelper.GetPolicyDetails(policy);
                var policyHolder = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                if (policyHolder != null)
                {
                    // TODO: RAI-167 Tech debt: Instead of using a static high/low risk address, look at dynamically using high/low risk address
                    var lowRiskAddress = new Address(streetNumber: "46", streetName: "Mortimer Rd", suburb: "Guilderton", postcode: "6041");

                    // testData is replaced here with a complete set for the current scenario.
                    // previous values were used by the HomeEndorsementBuilder to create the final set.
                    testData = new HomeEndorsementBuilder().InitialiseHomeWithDefaultData(policy, policyHolder)
                                                    .WithPropertyAddress(lowRiskAddress)
                                                    .WithYearBuilt(DateTime.Now.Year - 3)
                                                    .WithAreWindowsSecured(true)
                                                    .WithAreDoorsSecured(true)
                                                    .WithAlarmSystem(Alarm.RACMonitoredAlarm)
                                                    .WithRandomBuildingSumInsured(99999, 111111)
                                                    .WithRandomRentalRate()
                                                    .WithRandomPropertyManager()
                                                    .WithExpectedImpactOnPremium(PremiumChange.PremiumDecrease)
                                                    .Build();
                    break;
                }
            }
            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;
        }
    }
}