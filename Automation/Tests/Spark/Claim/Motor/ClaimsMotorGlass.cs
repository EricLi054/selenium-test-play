using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using Rac.TestAutomation.Common.TestData.Claim;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tests.ActionsAndValidations;
using Tests.ActionsAndValidations.Claims;
using UIDriver.Pages;
using static Rac.TestAutomation.Common.AzureStorage.AzureTableOperation;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace Spark.Claim.Motor
{
    [Property("Functional", "Motor Glass Claim")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class ClaimsMotorGlass : BaseUITest
    {
        private List<MotorPolicyEntity> _motorPoliciesForClaims;
        private readonly string _azureTableName = Config.Get().Shield.Environment + DataType.Policy + DataPurpose.ForClaim + ProductType.Motor;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Motor Glass claim");
            _motorPoliciesForClaims = DataHelper.AzureTableGetAllRecords(_azureTableName)
                                                    .FindAll(x => (x.CoverType == DataHelper.GetDescription(MotorCovers.MFCO)) &&
                                                                                  (!x.IsEV)).PickRandom(25);
        }

        #region Test Cases

        [Category(TestCategory.Regression),Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass)]
        [Test(Description = "CMG_LoginWithPolicyNumber_Policyholder_NotFixed_WindscreenDamaged")]
        public void INSU_T243_CMG_LoginWithPolicyNumber_Policyholder_NotFixed_WindscreenDamaged()
        {
            var claim = BuildTestDataMotorGlassClaim(LoginWith.PolicyNumber, ContactRole.PolicyHolder, MotorClaimScenario.GlassDamageNotFixed, onlyWindscreenDamaged: true,
                otherWindowGlass: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorGlassURL(_browser, claim);         
            ActionSparkMotorGlassClaim.LodgeMotorGlassClaim(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claim.ClaimNumber);
            VerifySparkMotorGlassClaim.VerifyMotorGlassClaimDetailsInShield(claim);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass)]
        [Test(Description = "CMG_LoginWithContactId_CoPolicyOwner_NotFixed_OtherGlassDamaged")]
        public void INSU_T208_CMG_LoginWithContactId_CoPolicyOwner_NotFixed_OtherGlassDamaged()
        {
            var claim = BuildTestDataMotorGlassClaim(LoginWith.ContactId, ContactRole.CoPolicyHolder, MotorClaimScenario.GlassDamageNotFixed, onlyWindscreenDamaged: false,
                otherWindowGlass: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorGlassURL(_browser, claim);            
            ActionSparkMotorGlassClaim.LodgeMotorGlassClaim(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claim.ClaimNumber);
            VerifySparkMotorGlassClaim.VerifyMotorGlassClaimDetailsInShield(claim);            
        }

        [Category(TestCategory.Regression), Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass), Category(TestCategory.VisualTest)]
        [Test(Description = "CMG_LoginWithContactId_CoPolicyOwner_RepairBooked_ChangeEmailAddress"), Category(TestCategory.SparkB2CRegressionForMemberCentralReleases)]
        public void INSU_T210_CMG_LoginWithContactId_CoPolicyOwner_RepairBooked_ChangeEmailAddress()
        {
            var claim = BuildTestDataMotorGlassClaim(LoginWith.ContactId, ContactRole.CoPolicyHolder, MotorClaimScenario.GlassDamageRepairsBooked, false, false
                ,changeEmail: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorGlassURL(_browser, claim);          
            ActionSparkMotorGlassClaim.LodgeMotorGlassClaim(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claim.ClaimNumber);
            VerifySparkMotorGlassClaim.VerifyMotorGlassClaimDetailsInShield(claim);

            Reporting.LogTestMemberCentralValidations("claim", claim.ClaimNumber);
            VerifySparkMotorGlassClaim.VerifyUpdatedContactDetailsInMemberCentral(claim);
        }

        [Category(TestCategory.Regression), Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass)]
        [Test(Description = "CMG_LoginWithPolicyNumber_PolicyHolder_AlreadyFixed")]
        public void INSU_T242_CMG_LoginWithPolicyNumber_PolicyHolder_AlreadyFixed()
        {
            var claim = BuildTestDataMotorGlassClaimAlreadyFixed(LoginWith.PolicyNumber, ContactRole.PolicyHolder, MotorClaimScenario.GlassDamageAlreadyFixed, false, false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorGlassURL(_browser, claim);
            ActionSparkMotorGlassClaim.LodgeMotorGlassClaim(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claim.ClaimNumber);
            VerifySparkMotorGlassClaim.VerifyMotorGlassClaimDetailsInShield(claim);            
        }

        [Category(TestCategory.Regression), Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass), Category(TestCategory.VisualTest), Category(TestCategory.InsuranceContactService)]
        [Test(Description = "CMG_LoginWithContactId_Policyholder_NotFixed_MultipleMotorPolicy_ChangeMobileNumber")]
        public void INSU_T209_CMG_LoginWithContactId_Policyholder_NotFixed_MultipleMotorPolicy_ChangeMobileNumber()
        {
            var claim = BuildTestDataMotorGlassClaim(LoginWith.ContactId, ContactRole.PolicyHolder, MotorClaimScenario.GlassDamageNotFixed, onlyWindscreenDamaged: true,
                otherWindowGlass: false, changeMobileNumber: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenMotorTriageClaimURL(_browser, claim);
            ActionSparkMotorGlassClaim.TriageMotorClaim(_browser, claim.DamageType);
            ActionSparkMotorGlassClaim.LodgeMotorGlassClaim(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claim.ClaimNumber);
            VerifySparkMotorGlassClaim.VerifyMotorGlassClaimDetailsInShield(claim);
        }

        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass)]
        [Test(Description = "CMG_LoginWithContactId_Policyholder_NotFixed_PaymentBlock_ReferralEmail")]
        public void CMG_LoginWithContactId_Policyholder_NotFixed_PaymentBlock_ReferralEmail()
        {
            var claim = BuildTestDataMotorGlassClaimPaymentBlock(LoginWith.ContactId, ContactRole.PolicyHolder, MotorClaimScenario.GlassDamageNotFixed, onlyWindscreenDamaged: true,
                otherWindowGlass: false, changeMobileNumber: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorGlassURL(_browser, claim);
            ActionSparkMotorGlassClaim.LodgeMotorGlassClaim(_browser, claim);
            VerifySparkMotorGlassClaim.VerifyMotorGlassReferralEmail(claim.ClaimNumber);

            Reporting.LogTestShieldValidations("claim", claim.ClaimNumber);
            VerifySparkMotorGlassClaim.VerifyMotorGlassClaimDetailsInShield(claim);
        }

        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass), Category(TestCategory.VisualTest)]
        [Test(Description = "CMG_LoginWithContactId_CoPolicyOwner_AlreadyFixed")]
        public void CMG_LoginWithContactId_CoPolicyOwner_AlreadyFixed()
        {
            var claim = BuildTestDataMotorGlassClaimAlreadyFixed(LoginWith.ContactId, ContactRole.PolicyHolder, MotorClaimScenario.GlassDamageAlreadyFixed, false, false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorGlassURL(_browser, claim);
            ActionSparkMotorGlassClaim.LodgeMotorGlassClaim(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claim.ClaimNumber);
            VerifySparkMotorGlassClaim.VerifyMotorGlassClaimDetailsInShield(claim);
        }

        [Test, TestCaseSource(typeof(CrossBrowserAndDeviceList)), Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.Glass), Category(TestCategory.CrossBrowserAndDeviceTest)]
        public void CrossBrowser_Policyholder_ClaimMotorGlass_NotFixed_WindscreenDamaged(TargetDevice targetDevice, TargetBrowser targetBrowser)
        {
            var claim = BuildTestDataMotorGlassClaim(LoginWith.ContactId, ContactRole.PolicyHolder, MotorClaimScenario.GlassDamageNotFixed, onlyWindscreenDamaged: true,
                otherWindowGlass: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();           
            LaunchPage.OpenMotorTriageClaimURL(_browser, claim, targetBrowser, targetDevice);
            ActionSparkMotorGlassClaim.TriageMotorClaim(_browser, claim.DamageType);
            ActionSparkMotorGlassClaim.LodgeMotorGlassClaim(_browser, claim);

        }

        #endregion

        #region Test cases helper methods
        private class ClaimPolicy
        {
            public MotorPolicy motorPolicy { get; set; }
            public PolicyContactDB claimant { get; set; }
            public Contact driver { get; set; }
            public List<PolicyDetail> linkedMotorPolicies { get; set; }
        }

        /// <summary>
        /// Get an eligible motor policy based on the Contact role,
        /// </summary>    
        /// <returns>Return an eligible motor policy for claim</returns>    
        private ClaimPolicy GetEligiblePolicy(List<MotorPolicyEntity> motorPolicyEntities, ContactRole contactRole)
        {
            ClaimPolicy claimPolicy = new ClaimPolicy();
            foreach (var entity in motorPolicyEntities)
            {
                var policy = DataHelper.GetMotorPolicyDetailsFromEntity(entity, _azureTableName);
                if (policy == null) { continue; }

                // Check policy has desired role.
                var claimant = policy.PolicyHolders.Find(x => x.ContactRoles.Contains(contactRole));
                if (claimant == null) { continue; }

                claimant.UpdateEmailIfNotDefined();

                if (!ShieldPolicyDB.IsPolicySuitableForClaims(policy.PolicyNumber))
                { continue; }

                var linkedMotorPolicies = DataHelper.GetAllPoliciesOfTypeForPolicyHolder(claimant.Id, "Motor");

                //For Percy screencheck we need policies where the claimant has exactly 2 linked motor policies
                //this is to make sure we have a similar comparison against the base run 
                if (_testConfig.IsVisualTestingEnabled && linkedMotorPolicies.Count() != 2)
                { continue; }

                claimPolicy.motorPolicy = policy;
                claimPolicy.claimant = claimant;
                claimPolicy.linkedMotorPolicies = linkedMotorPolicies;
                break;
            }

            Reporting.IsNotNull(claimPolicy.motorPolicy, "suitable test data has been found");
            // Remove the used policy number from the list
            motorPolicyEntities.RemoveAll(x => x.PolicyNumber == claimPolicy.motorPolicy.PolicyNumber);

            return claimPolicy;
        }

        private ClaimCar BuildTestDataMotorGlassClaim(LoginWith loginType, ContactRole contactRole, MotorClaimScenario glassClaimScenario, bool onlyWindscreenDamaged, bool otherWindowGlass,
                            bool changeMobileNumber = false, bool changeEmail = false)
        {
            var candidate = GetEligiblePolicy(_motorPoliciesForClaims, contactRole);

            var testData = new MotorClaimBuilder()
                    .InitialiseMotorClaimWithBasicData(candidate.motorPolicy, MotorClaimDamageType.WindscreenGlassDamage)
                    .LoginWith(loginType)
                    .WithGlassDamageDetails(onlyWindscreenDamaged, otherWindowGlass, GlassDamageType.Chip, false)
                    .WithClaimant(candidate.claimant, changeMobileNumber, changeEmail)
                    .WithRandomEventDateInLast7Days()
                    .WithClaimScenario(glassClaimScenario)
                    .WithLinkedMotorPoliciesForClaimant(candidate.linkedMotorPolicies)
                    .Build();

            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;
        }

        private ClaimCar BuildTestDataMotorGlassClaimPaymentBlock(LoginWith loginType, ContactRole contactRole, MotorClaimScenario glassClaimScenario, bool onlyWindscreenDamaged, bool otherWindowGlass,
                    bool changeMobileNumber = false, bool changeEmail = false)
        {
            ClaimCar testData = null;
            var policiesForGlassClaims = ShieldMotorClaimDB.FindMotorPoliciesHaveUnpaidInstallments();

            foreach (var policy in policiesForGlassClaims)
            {
                var claimant = policy.PolicyHolders.Find(x => x.ContactRoles.Contains(contactRole));
                claimant = DataHelper.MapPolicyContactWithPersonAPI(claimant.Id, claimant.ExternalContactNumber, contactRole);

                if (claimant != null)
                {
                    var linkedMotorPolicies = DataHelper.GetAllPoliciesOfTypeForPolicyHolder(claimant.Id, "Motor");

                    testData = new MotorClaimBuilder()
                        .InitialiseMotorClaimWithBasicData(policy, MotorClaimDamageType.WindscreenGlassDamage)
                        .LoginWith(loginType)
                        .WithGlassDamageDetails(onlyWindscreenDamaged, otherWindowGlass, GlassDamageType.Chip, false)
                        .WithClaimant(claimant, changeMobileNumber, changeEmail)
                        .WithEventDateAndTime(DateTime.Now)
                        .WithClaimScenario(glassClaimScenario)
                        .WithLinkedMotorPoliciesForClaimant(linkedMotorPolicies)
                        .WithPaymentBlock()
                        .Build();

                    break;
                }
            }

            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;
        }


        private ClaimCar BuildTestDataMotorGlassClaimAlreadyFixed(LoginWith loginType, ContactRole contactRole, MotorClaimScenario glassClaimScenario, bool onlyWindscreenDamaged, bool otherWindowGlass,
                   bool changeMobileNumber = false, bool changeEmail = false)
        {
            var candidate = GetEligiblePolicy(_motorPoliciesForClaims, contactRole);

            var testData = new MotorClaimBuilder()
                    .InitialiseMotorClaimWithBasicData(candidate.motorPolicy, MotorClaimDamageType.WindscreenGlassDamage)
                    .LoginWith(loginType)
                    .WithGlassDamageDetails(onlyWindscreenDamaged, otherWindowGlass, GlassDamageType.Chip, false)
                    .WithClaimant(candidate.claimant, changeMobileNumber, changeEmail)
                    .WithEventDateAndTime(DateTime.Now.Date.AddDays(-6).AddHours(15).AddMinutes(15))
                    .WithClaimScenario(glassClaimScenario)
                    .WithLinkedMotorPoliciesForClaimant(candidate.linkedMotorPolicies)
                    .Build();

            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;
        }

        public class CrossBrowserAndDeviceList : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new object[] { TargetDevice.MacBook, TargetBrowser.Safari };
                yield return new object[] { TargetDevice.Windows11, TargetBrowser.Edge };
                yield return new object[] { TargetDevice.iPhone14, TargetBrowser.Safari };
                yield return new object[] { TargetDevice.GalaxyS21, TargetBrowser.Chrome };
                yield return new object[] { TargetDevice.iPad10, TargetBrowser.Safari };
            }
        }

        #endregion
    }
}
