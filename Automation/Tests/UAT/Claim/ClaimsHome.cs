using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using Rac.TestAutomation.Common.TestData.Claim;
using Tests.ActionsAndValidations;
using System;
using System.Collections.Generic;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.General;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.PolicyHome;


namespace Tests.UAT.Claim
{
    [TestFixture]
    [Property("UAT", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class ClaimsHome : BaseUITest
    {
        private List<HomePolicy> _homePoliciesForClaims;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C UAT tests for online claims against Home insurance policies.");
            _homePoliciesForClaims = ShieldHomeClaimDB.ReturnHomeOwnerPoliciesSuitableForClaims();
        }

        [Test, Category(TestCategory.UAT), Category(TestCategory.Home), 
            Category(TestCategory.Claim), Category(TestCategory.Mock_Member_Central_Support)]
        public void UAT_Claims_Home_EscapeOfLiquid()
        {
            var claim = TestDataEscapeOfLiquid();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            _browser.LaunchPageBeginNewAnonymousHomeClaim();
            ActionsClaimHome.TriageHomeClaim(_browser, claim);

            ActionsClaimHome.ClickOnContinueAndFieldValidation(_browser);
            ActionsClaimHome.HomeClaimCompletePrelimDetails(_browser, claim, findPolicyByPolicyNumber: true);
            ActionsClaimHome.HomeClaimCompletePage2Details(_browser, claim);

            var claimNumber = VerifyClaimHome.VerifyDetailsOnClaimConfirmationPage(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claimNumber);
            VerifyClaimHome.VerifyHomeClaimInShield(claim, claimNumber);
            VerifyClaimHome.VerifyClaimsQuestionnairesInShield(claimNumber, claim);
        }

        [Test, Category(TestCategory.UAT), Category(TestCategory.Home), 
            Category(TestCategory.Claim), Category(TestCategory.Mock_Member_Central_Support)]
        public void UAT_Claims_Home_TheftBreakIn()
        {
            var claim = TestDataTheftBreakIn();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            _browser.LaunchPageBeginNewAnonymousHomeClaim();

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionsClaimHome.HomeClaimCompletePrelimDetails(_browser, claim, findPolicyByPolicyNumber: true);
            ActionsClaimHome.HomeClaimCompletePage2Details(_browser, claim);
            ActionsClaimHome.HomeClaimCompleteWitnessAndThirdPartyDetails(_browser, claim);
            var claimNumber = VerifyClaimHome.VerifyDetailsOnClaimConfirmationPage(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claimNumber);
            VerifyClaimHome.VerifyHomeClaimInShield(claim, claimNumber);
            VerifyClaimHome.VerifyClaimsQuestionnairesInShield(claimNumber, claim);
        }

        [Test, Category(TestCategory.UAT), Category(TestCategory.Home),
            Category(TestCategory.Claim), Category(TestCategory.Mock_Member_Central_Support)]
        public void UAT_Claims_Home_Specified_PersonalValuables_AccidentalDamage()
        {
            var claim = TestDataSpecifiedPersonalValuables();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            _browser.LaunchPageBeginNewPersonalValuablesClaim();

            ActionsClaimHome.HomeClaimCompletePrelimDetails(_browser, claim, findPolicyByPolicyNumber: true);
            ActionsClaimHome.HomeClaimCompletePage2Details(_browser, claim);

            var claimNumber = VerifyClaimHome.VerifyDetailsOnClaimConfirmationPage(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claimNumber);
            // TODO: AUNT-190 add verification for shield check
        }

        private ClaimHome TestDataEscapeOfLiquid()
        {
            ClaimHome claim = null;
            foreach (var policy in _homePoliciesForClaims)
            {
                var claimant = policy.PolicyHolders.PickRandom();
                claimant = DataHelper.MapPolicyContactWithPersonAPI(claimant.Id, claimant.ExternalContactNumber, claimant.ContactRoles.FirstOrDefault());

                if (claimant != null && ShieldClaimDB.GetOpenClaimCountForPolicy(policy.PolicyNumber) == 0)
                {
                    var eventDateAndTime = DateTime.Now.Date.AddDays(-DataHelper.RandomNumber(1, 60)).RandomClaimTimeOfDay();

                    claim = new HomeClaimBuilder()
                               .InitialiseHomeClaimWithBasicData(policy.PolicyNumber, claimant, HomeClaimDamageType.EscapeOfLiquid)
                               .WithAffectedCover(AffectedCovers.BuildingAndContents)
                               .WithEventDateAndTime(eventDateAndTime)
                               .WithAccountOfEvent("How Damage Occurred")
                               .WithIsHomeInhabitable(isHomeInhabitable: true)
                               .WithRandomTimberLaminateFlooringDamage()
                               .Build();
                    break;
                }
            }
            Reporting.IsNotNull(claim, "suitable test data has been found");
            return claim;
        }

        private ClaimHome TestDataTheftBreakIn()
        {           
            ClaimHome claim = null;
            foreach ( var policy in _homePoliciesForClaims ) 
            {
                var claimant = policy.PolicyHolders.PickRandom();
                claimant = DataHelper.MapPolicyContactWithPersonAPI(claimant.Id, claimant.ExternalContactNumber, claimant.ContactRoles.FirstOrDefault());

                if (claimant != null && ShieldClaimDB.GetOpenClaimCountForPolicy(policy.PolicyNumber) == 0)
                {                    
                    var eventDateAndTime = DateTime.Now.Date.AddDays(-DataHelper.RandomNumber(1, 60)).RandomClaimTimeOfDay();

                     claim = new HomeClaimBuilder()                       
                       .InitialiseHomeClaimWithBasicData(policy.PolicyNumber, claimant, HomeClaimDamageType.Theft)
                       .WithAffectedCover(AffectedCovers.ContentsOnly)
                       .WithEventDateAndTime(eventDateAndTime)
                       .WithAccountOfEvent("How Damage Occurred")
                       .WithTheftDamage(stolenItemsLocation: StolenItemsLocation.Inside)
                       .AddStolenItem("Xbox one", 200)
                       .AddStolenItem("iPhone", 500)
                       .WithRandomPoliceReport()
                       .Build();
                    
                    break;
                }                
            }
            Reporting.IsNotNull(claim, "suitable test data has been found");
            return claim;
        }

        private ClaimHome TestDataSpecifiedPersonalValuables()
        {
            ClaimHome claim = null;
            var candidates = ShieldHomeClaimDB.ReturnHomeOwnerPoliciesSuitableForClaimsPersonalValuables();

            foreach (var policy in candidates)
            {
                var claimant = policy.PolicyHolders.PickRandom();
                claimant = DataHelper.MapPolicyContactWithPersonAPI(claimant.Id, claimant.ExternalContactNumber, claimant.ContactRoles.FirstOrDefault());

                if (claimant != null)
                {
                    var eventDateAndTime = DateTime.Now.Date.AddDays(-DataHelper.RandomNumber(1, 60)).RandomClaimTimeOfDay();

                    claim = new HomeClaimBuilder()
                      .InitialiseHomeClaimWithBasicData(policy.PolicyNumber, claimant, HomeClaimDamageType.AccidentalDamage)
                      .WithEventDateAndTime(eventDateAndTime)
                      .WithAffectedCover(AffectedCovers.SpecifiedPersonalValuablesOnly)
                      .WithAccountOfEvent("Hit by Godzilla")
                      .Build();

                    break;
                }
            }
            Reporting.IsNotNull(claim, "suitable test data has been found");
            return claim;
        }
    }
}
