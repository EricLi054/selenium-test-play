using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Endorsements;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using Tests.ActionsAndValidations;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.SparkPersonalInformationPage.XPathPersonalInfo;

namespace B2C.MemberServices
{
    [Property("Functional", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class CertificateOfCurrency : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C UAT and Regression tests - Retrieving certificates of currency.");
        }

        /// <summary>
        /// UAT test scenario of retrieving the certificate of currency for a current Motor Policy.
        /// </summary>
        [Test, Category(TestCategory.UAT), Category(TestCategory.Motor), 
               Category(TestCategory.Endorsement), Category(TestCategory.B2CPCM)]
        public void UAT_MotorPolicy_GetCertficateOfCurrency()
        {
            var testData = BuildTestDataForMotorPolicyCertificateOfCurrency();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            ActionsPCM.FetchCertificateOfCurrencyForPolicy(_browser, testData.ActivePolicyHolder, testData.PolicyNumber);

            VerifyPolicy.VerifyEmailedCertificateOfCurrency(testData.PolicyNumber, testData.ActivePolicyHolder.GetEmail());
        }

        private EndorseCar BuildTestDataForMotorPolicyCertificateOfCurrency()
        {
            EndorseCar testData = null;
            var policies = ShieldMotorClaimDB.FindMotorPolicyWithNoExistingClaimsNoHireCar();

            foreach (var policy in policies)
            {
                var policyDetails = DataHelper.GetPolicyDetails(policy.PolicyNumber);
                var policyHolders =  DataHelper.FetchPolicyContacts(policyDetails);

                if (policyHolders == null ||
                    !ShieldPolicyDB.IsPolicySuitableForClaims(policy.PolicyNumber))
                {
                    continue;
                }

                // We pick a random policyholder Contact and change the email to make sure we get
                // a Insurance accessible Mailosaur email address.
                var activePolicyHolder = new ContactBuilder(policyHolders.PickRandom())
                                         .WithPrivateEmailAddressFromName()
                                         .Build();

                testData = new MotorEndorsementBuilder().InitialiseMotorCarWithDefaultData(policy.PolicyNumber, activePolicyHolder)
                                                        .Build();
                break;
            }

            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;
        }

        /// <summary>
        /// UAT test scenario of retrieving the certificate of currency for a current Home Policy.
        /// </summary>
        [Test, Category(TestCategory.UAT), Category(TestCategory.Home), 
               Category(TestCategory.Endorsement), Category(TestCategory.B2CPCM)]
        public void UAT_HomePolicy_GetCertficateOfCurrency()
        {
            var testData = BuildTestDataForHomePolicyCertificateOfCurrency();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            ActionsPCM.FetchCertificateOfCurrencyForPolicy(_browser, testData.ActivePolicyHolder, testData.PolicyNumber);

            VerifyPolicy.VerifyEmailedCertificateOfCurrency(testData.PolicyNumber, testData.ActivePolicyHolder.GetEmail());
        }

        private EndorseHome BuildTestDataForHomePolicyCertificateOfCurrency()
        {
            EndorseHome testData = null;
            var policies = ShieldHomeDB.FindPolicyHomeActiveBuildingCoverForCertificateOfCurrency();
            
            foreach (var policy in policies)
            {
                var policyDetails = DataHelper.GetPolicyDetails(policy);
                var policyHolder = DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber);
                // We re-create Contact with changed email to make sure we get
                // a Insurance accessible Mailosaur email address.
                var updatedPolicyHolder = new ContactBuilder(policyHolder)
                                         .WithPrivateEmailAddressFromName()
                                         .Build();

                if (policyHolder != null)
                {
                    testData = new HomeEndorsementBuilder().InitialiseHomeWithDefaultData(policy, updatedPolicyHolder).Build();
                    break;
                }
            }

            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;
        }            
    }
}
