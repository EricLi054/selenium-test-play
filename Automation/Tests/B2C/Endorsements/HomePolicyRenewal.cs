using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using Rac.TestAutomation.Common.TestData.Endorsements;
using Tests.ActionsAndValidations;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace B2C.EndorsePolicy
{
    [Property("Functional", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class HomePolicyRenewal : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C mandatory regression tests for home endorsements involving policy renewal.");
        }

        /// <summary>
        /// Mandatory regression test case "B2C Logged In - Renew My Policy - TC01 - Renew an Annual Cash paid Home Policy"
        /// Covers the scenario of a member who has a current Landlord’s policy paid by Annual Cash, renewing it via PCM
        /// </summary>
        [Test(Description = "B2C Logged In - Renew My Policy - TC01 - Renew an Annual Cash paid Home Policy")]
        [Category(TestCategory.Regression), Category(TestCategory.Endorsement), Category(TestCategory.Home), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.Mock_Member_Central_Support), Category(TestCategory.B2CPCM)]
        public void INSU_T27_B2CLoggedIn_HomePolicyRenewal_TC01_AnnualCashToGateway()
        {
            var testData = BuildTestDataForTC01HomeRenewal();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            Reporting.LogTestStart();
            var policyValuesFromRenewal = ActionsEndorseHome.PerformHomePolicyRenewal(_browser, testData);

            Reporting.LogTestShieldValidations("home policy after endorsement", testData.PolicyNumber);
            VerifyEndorseHome.VerifyPolicyStateAfterRenewal(_browser, testData);
            VerifyEndorseHome.VerifyPolicyDetailsInShieldAfterRenewal(_browser, testData, policyValuesFromRenewal.AnnualPremium);
            VerifyEndorseHome.VerifyPolicyPaymentDetailsInShield(testData, policyValuesFromRenewal);
        }

        /// <summary>
        /// Mandatory regression test case "B2C Logged In - Renew My Policy - TC02 - Renew a Home Policy paid in Instalments"
        /// Covers the scenario of a member who has a current Landlord’s policy paid by Instalments, renewing it via PCM
        /// </summary>
        [Test(Description = "B2C Logged In - Renew My Policy - TC02 - Renew a Home Policy paid by Instalments")]
        [Category(TestCategory.Regression), Category(TestCategory.Endorsement), Category(TestCategory.Home), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.Mock_Member_Central_Support), Category(TestCategory.B2CPCM)]
        public void INSU_T28_B2CLoggedIn_HomePolicyRenewal_TC02_Instalments()
        {
            var testData = BuildTestDataForTC02HomeRenewal();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            Reporting.LogTestStart();
            var policyValuesFromRenewal = ActionsEndorseHome.PerformHomePolicyRenewal(_browser, testData);

            Reporting.LogTestShieldValidations("home policy after endorsement", testData.PolicyNumber);
            VerifyEndorseHome.VerifyPolicyStateAfterRenewal(_browser, testData);
            VerifyEndorseHome.VerifyPolicyDetailsInShieldAfterRenewal(_browser, testData, policyValuesFromRenewal.AnnualPremium);
            VerifyEndorseHome.VerifyPolicyPaymentDetailsInShield(testData, policyValuesFromRenewal);
        }

        private EndorseHome BuildTestDataForTC01HomeRenewal()
        {
            EndorseHome testData = null;
            var policies = ShieldHomeDB.FindHomePolicyForRenewal(PaymentFrequency.Annual);
            foreach (var policy in policies)
            {
                if (!ShieldPolicyDB.IsPolicySuitableForEndorsements(policy))
                { continue; }

                var policyDetails = DataHelper.GetPolicyDetails(policy);
                var policyHolder = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                if (policyHolder != null)
                {
                    testData = new HomeEndorsementBuilder()
                                .InitialiseHomeWithDefaultData(policy, policyHolder)
                                .WithExpectedImpactOnPremium(PremiumChange.NotApplicable)
                                .WithWeeklyRentalRate("400")
                                .WithPropertyManager(HomePropertyManager.Agent)
                                .Build();
                    break;
                }
            }

            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;
        }

        private EndorseHome BuildTestDataForTC02HomeRenewal()
        {
            EndorseHome testData = null;
            var policies = ShieldHomeDB.FindHomePolicyForRenewal(PaymentFrequency.Monthly);

            foreach (var policy in policies)
            {
                if (!ShieldPolicyDB.IsPolicySuitableForEndorsements(policy))
                { continue; }

                var policyDetails = DataHelper.GetPolicyDetails(policy);
                var policyHolder = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);

                if (policyHolder != null)
                {
                    testData = new HomeEndorsementBuilder()
                                .InitialiseHomeWithDefaultData(policy, policyHolder)
                                .WithExpectedImpactOnPremium(PremiumChange.NotApplicable)
                                .WithWeeklyRentalRate("400")
                                .WithPropertyManager(HomePropertyManager.Agent)
                                .Build();
                    break;
                }
            }

            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;
        }
    }
}