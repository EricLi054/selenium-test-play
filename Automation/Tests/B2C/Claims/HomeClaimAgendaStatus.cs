using NUnit.Framework;
using Rac.TestAutomation.Common.DatabaseCalls.Queries.Environment;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.General;
using Tests.ActionsAndValidations.Claims;
using Tests.ActionsAndValidations;
using UIDriver.Pages.PCM;
using UIDriver.Pages;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using Rac.TestAutomation.Common.TestData.Claim;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.Contacts;

namespace B2C.Claims
{
    [Property("Functional", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class HomeClaimAgendaStatus : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C mandatory regression tests for verifying claim status details in PCM as a claim progresses through its lifecycle.");
        }

        /// <summary>
        /// Mandatory regression test case "B2C Logged In - Claim Status - Home"
        /// Covers the scenario of a Home Storm claim reported online via Spark, 
        /// creating a Lodged = Current status and then tracking the Claim Agenda Status 
        /// in PCM as the claim is updated through its life cycle via Shield.
        /// 
        /// Note: Though this test utilises Spark to create the initial test and so has a 
        /// Spark TestCategory flagged, the main focus of the test is on the Claim Agenda in 
        /// Shield and the accurate representation of that status in PCM.
        /// 
        /// Test emulates the following workflow of a member from Spark and Shield perspectives:
        ///     1. Lodge
        ///         1.1 Spark - Claim setup in Spark HGC application
        ///         1.2 Shield UI - Add SCPV damage from Sheild
        ///         1.3 B2C PCM - Verify lodge status
        ///     2. Assess
        ///         2.1 Shield UI - Update claims handler
        ///         2.2 B2C PCM - Verify assess status
        ///     3. Repair
        ///         3.1 Shield UI - Update liability reserve
        ///         3.2 B2C PCM - Verify repair status
        ///     4. Complete
        ///         4.1 Shield UI - Update invoice/payment
        ///         4.2 B2C PCM - Verify complete status
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Claim), Category(TestCategory.Home),
        Category(TestCategory.Mock_Member_Central_Support),Category(TestCategory.B2CPCM)]
        public void INSU_T37_B2CLoggedIn_ClaimStatus_Home()
        {
            var claim = BuildTestDataForClaimStatus();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());
            var shieldHgcToggle = ShieldParametersDB.FetchShieldToggleForEnvironment(ShieldToggle.HomeGeneralClaims.GetDescription());
            Reporting.LogMinorSectionHeading($"Shield Toggle '{shieldHgcToggle.Param_desc}' value = '{shieldHgcToggle.Value}'");
            Reporting.LogTestStart();

            LaunchPage.OpenHomeTriageClaimURL(_browser, claim);
            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim, detailedUiChecking: false);

            var claimNumber = claim.ClaimNumber;

            Reporting.LogTestShieldValidations("claim", claimNumber);
            VerifyClaimHome.VerifyClaimStatusInPCM(_browser, claimNumber, claim.Claimant.Id, PortfolioSummary.CLAIM_PROGRESS_STATE.Lodge);

            ActionsClaimHome.HomeClaimShieldAssignClaimsHandler(_browser, claimNumber);
            VerifyClaimHome.VerifyClaimStatusInPCM(_browser, claimNumber, claim.Claimant.Id, PortfolioSummary.CLAIM_PROGRESS_STATE.Assess);

            VerifyClaimHome.VerifyClaimsQuestionnairesInShield(claimNumber, claim);
            ActionsClaimHome.HomeClaimShieldAddSCPVDamage(_browser, claimNumber, shieldHgcToggle.Value);

            ActionsClaimHome.HomeClaimShieldAuthoriseQuote(_browser, claimNumber);
            VerifyClaimHome.VerifyClaimStatusInPCM(_browser, claimNumber, claim.Claimant.Id, PortfolioSummary.CLAIM_PROGRESS_STATE.Repair);

            ActionsClaimHome.HomeClaimShieldAcceptRepairerInvoice(_browser, claimNumber);
            VerifyClaimHome.VerifyClaimStatusInPCM(_browser, claimNumber, claim.Claimant.Id, PortfolioSummary.CLAIM_PROGRESS_STATE.Complete);
        }

        /// <summary>
        /// Builds test data specifically for the Home Claim Agenda Status test.
        /// Please note that it is important that StormDamagedItemTypes.OtherItems be included
        /// as this ensures that the status of the Lodged claim agenda step will be Current after
        /// the claim is reported.
        /// </summary>
        private ClaimHome BuildTestDataForClaimStatus()
        {
            var policyNumber = ShieldHomeDB.FindHomePersonalValuablesPolicyForHomeClaimAgendaTest();

            ClaimHome testData = null;
            var policyDetails = ShieldHomeDB.FetchHomePolicyDetailsForClaim(policyNumber);
            var claimantCandidate = policyDetails.PolicyHolders.PickRandom();

            var claimant = DataHelper.MapPolicyContactWithPersonAPI(claimantCandidate.Id, claimantCandidate.ExternalContactNumber, claimantCandidate.ContactRoles.FirstOrDefault());

            Reporting.Log($"Policy selected for claim is {policyNumber.ToString()}");
            var someBrokenStuff = new List<ContentItem>()
            { new ContentItem("Persian rugs", 1234),
                new ContentItem("Kitchen cabinets", 15500),
                new ContentItem("playstation 5", 630)};

            var stormDamagedBuildingSpecifics = new List<StormDamagedItemTypes>()
            {
                StormDamagedItemTypes.OtherItems //If OtherItems is selected, the Lodged claim agenda step will be Current
            };

            testData = new HomeClaimBuilder()
                    .InitialiseHomeClaimWithBasicData(policyDetails.PolicyNumber, claimant, HomeClaimDamageType.StormAndTempest)                    
                    .WithAffectedCover(AffectedCovers.BuildingAndContents)
                    .WithIsHomeInhabitable(isHomeInhabitable: true)
                    .WithStormSafetyCheckOptions(shouldHomeBeInhabitable: true, forceNoSafetyChecks: true)
                    .LoginWith(LoginWith.PolicyNumber)
                    .WithEventDateAndTime(DataHelper.GenerateRandomDate(DateTime.Now, DateTime.Now.AddMonths(-1)))
                    .WithBuildingDamage(stormDamagedBuildingSpecifics, isSpecificItemsDamaged: true, isBuildingOnlyOtherItems: true, isHomeBadlyDamaged: false, isHomeUnsafe: false)
                    .WithRandomStormWaterDamage(isCarpetTooWet: false, avoidRestorerAllocation: true)
                    .WithContentsDamage(someBrokenStuff, isCarpetWaterDamaged: false, isCarpetTooWet: false, isOtherStormDamagedContents: true)
                    .WithExpectedOutcomeForTest(ExpectedClaimOutcome.ClaimLodged)
                    .Build();

            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;
        }
    }
}
