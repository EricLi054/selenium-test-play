using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using Rac.TestAutomation.Common.TestData.Claim;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.ActionsAndValidations;
using Tests.ActionsAndValidations.Claims;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;

namespace Spark.Claim.Home
{
    [Property("Functional", "Spark Non-dividing Fence Claim")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class HomeNonDividingFenceClaim : BaseUITest
    {
        private List<string> _homePoliciesForSparkClaims;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Spark tests for Non-dividing fence claim");
            _homePoliciesForSparkClaims = ShieldHomeClaimDB.ReturnLandlordAndHomeOwnerPoliciesSuitableForClaims();
        }

        #region Test Cases
        /// <summary>
        /// Lodge a non-dividing fence claim with below details
        /// Policy Cover : Home Owner Building and Content
        /// Claimant Role : Policy Holder
        /// Fence Type : Asbestos
        /// Damaged Fence Length : 12
        /// Damage Painted Fence Length : 5
        /// Damage Sides : Left; Rear
        /// Temporary Fence Required : Yes
        /// Online Settlement: Eligible - Accept with bank details (includes Asbestos removal element in Settlement Breakdown).
        /// Provide Bank Details for Direct Debit; test includes BSB validation.
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), 
            Category(TestCategory.Regression), Category(TestCategory.FenceOnly), Category(TestCategory.PrivateFence), Category(TestCategory.VisualTest), 
            Category(TestCategory.InsuranceContactService), Category(TestCategory.MultiFactorAuthentication)]
        [Test(Description = "NonSharedFence HomeBuildingContent PolicyHolder Asbestos TempFence AcceptWithBankDetails ChangeEmailAddress")]
        public void INSU_T326_NonSharedFence_HomeBuildingContent_PolicyHolder_Asbestos_TempFence_AcceptWithBankDetails_ChangeEmailAddress()
        {            
            var claim = BuildTestDataSparkPrivateFenceClaim_ChangeEmailAddress(ContactRole.PolicyHolder, _homePoliciesForSparkClaims,
                FenceType.Asbestos, 12, 5, tempFenceRequired: true, leftSide: true, rearSide: true,
                expectedClaimOutcome: ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails,
                eligibilityForOnlineSettlement: SettleFenceOnline.Eligible);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim, retryOTP: false, detailedUiChecking: true);

        }

        /// <summary>
        /// Lodge a non-dividing fence claim with below details
        /// Policy Cover : Home Owner Building and Content
        /// Claimant Role : Policy Holder
        /// Fence Type : Colorbond
        /// Damaged Fence Length : 15
        /// Damage Painted Fence Length : 0
        /// Damage Sides : Left; Right
        /// Temporary Fence Required : No
        /// Online Settlement : Not Eligible - Repairs Completed, so we want their invoice
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.FenceOnly), Category(TestCategory.PrivateFence)]
        [Test(Description = "NonSharedFence HomeBuildingContent PolicyHolder RepairsCompleted")]
        public void INSU_T331_NonSharedFence_HomeBuildingContent_PolicyHolder_RepairsCompleted()
        {
            var claim = BuildTestDataSparkPrivateFenceClaim(ContactRole.PolicyHolder, _homePoliciesForSparkClaims,
                FenceType.Colorbond, 15, 0, leftSide: true, rightSide: true, tempFenceRequired: false,
                expectedClaimOutcome: ExpectedClaimOutcome.RepairsCompleted,
                eligibilityForOnlineSettlement: SettleFenceOnline.IneligibleRepairsAlreadyCompleted);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim);

        }
        
        /// <summary>
        /// Lodge a non-dividing fence claim with below details
        /// Policy Cover : Home Owner Building and Content
        /// Claimant Role : Policy Holder
        /// Fence Type : Wooden
        /// Damaged Fence Length : 12
        /// Damage Painted Fence Length : 0
        /// Damage Sides : Right; Front
        /// Temporary Fence Required : Yes
        /// Online Settlement : Not Eligible - Wooden fence
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.FenceOnly), Category(TestCategory.PrivateFence)]
        [Test(Description = "NonSharedFence HomeBuildingContent PolicyHolder Wooden TempFence NotEligible")]
        public void INSU_T330_NonSharedFence_HomeBuildingContent_PolicyHolder_Wooden_TempFence_NotEligible()
        {
            var claim = BuildTestDataSparkPrivateFenceClaim(ContactRole.PolicyHolder, _homePoliciesForSparkClaims,
                FenceType.Wooden, 12, 0, rightSide: true, frontSide: true, tempFenceRequired: true,
                expectedClaimOutcome: ExpectedClaimOutcome.NotEligibleForOnlineSettlement,
                eligibilityForOnlineSettlement: SettleFenceOnline.IneligibleFenceTypeWood);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim);

        }

        /// <summary>
        /// Lodge a non-dividing fence claim with below details
        /// Policy Cover : HomeOwner Building
        /// Claimant Role : Co Policy Owner
        /// Fence Type : Hardifence
        /// Damaged Fence Length : HavingTroubleMeasuringFence
        /// Damage Painted Fence Length : 0
        /// Damage Sides : Left; Right
        /// Temporary Fence Required : No
        /// Online Settlement : Not Eligible - they can't tell us how much of their fence is damaged
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.FenceOnly), Category(TestCategory.PrivateFence)]
        [Test(Description = "NonSharedFence HomeBuilding CoPolicyOwner Hardifence HavingTroubleMeasuringFence")]
        public void INSU_T329_NonSharedFence_HomeBuilding_CoPolicyOwner_Hardifence_HavingTroubleMeasuringFence_ChangeMobileNumber()
        {
            var claim = BuildTestDataSparkPrivateFenceClaim_TroubleMeasuringFence_ChangeMobileNumber(ContactRole.CoPolicyHolder, _homePoliciesForSparkClaims,
                FenceType.Hardifence, lengthDamaged: null, 0, leftSide: true, rightSide: true,
               expectedClaimOutcome: ExpectedClaimOutcome.NotEligibleForOnlineSettlement,
               eligibilityForOnlineSettlement: SettleFenceOnline.IneligibleCannotMeasureFence,
               includedCoversOnPolicy: AffectedCovers.BuildingOnly);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim);

        }

        /// <summary>
        /// Lodge a non-dividing fence claim with below details
        /// Policy Cover : HomeOwner Building
        /// Claimant Role : Co PolicyOwner
        /// Fence Type : Other
        /// Damaged Fence Length : 19
        /// Damage Painted Fence Length : 0
        /// Damage Sides : Left; Right; Rear
        /// Temporary Fence Required : Yes
        /// Online Settlement : Not Eligible - member already has a repair quote so we want that
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.FenceOnly), Category(TestCategory.PrivateFence)]
        [Test(Description = "NonSharedFence HomeBuilding CoPolicyOwner Other AlreadyHaveRepairQuote")]
        public void INSU_T334_NonSharedFence_HomeBuilding_CoPolicyOwner_Other_AlreadyHaveRepairQuote()
        {
            var claim = BuildTestDataSparkPrivateFenceClaim(ContactRole.CoPolicyHolder, _homePoliciesForSparkClaims,
                FenceType.Other, lengthDamaged: 19, 0, leftSide: true, rightSide: true, rearSide: true, tempFenceRequired: true,
                expectedClaimOutcome: ExpectedClaimOutcome.AlreadyHaveRepairQuote,
                eligibilityForOnlineSettlement: SettleFenceOnline.IneligibleRepairsAlreadyQuoted,
                includedCoversOnPolicy: AffectedCovers.BuildingOnly);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim);

        }

        /// <summary>
        /// Lodge a non-dividing fence claim with below details
        /// Policy Cover : HomeOwner Building and Content
        /// Claimant Role : Policy Holder
        /// Fence Type : Super Six
        /// Damaged Fence Length : 11
        /// Damage Painted Fence Length : 11
        /// Damage Sides : Right; Rear
        /// Temporary Fence Required : Yes
        /// Online Settlement : Eligible - Select Take Time To Decide on Cash Settlement Fact Sheet
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.FenceOnly), Category(TestCategory.PrivateFence)]
        [Test(Description = "NonSharedFence HomeBuildingContent PolicyHolder SuperSix TempFence TakeTimeToDecide")]
        public void INSU_T328_NonSharedFence_HomeOwner_PolicyHolder_SuperSix_TempFence_TakeTimeToDecide()
        {
            var claim = BuildTestDataSparkPrivateFenceClaim(ContactRole.PolicyHolder, _homePoliciesForSparkClaims,
                FenceType.SuperSix, 11, 11, rightSide: true, rearSide: true, tempFenceRequired: true,
                expectedClaimOutcome: ExpectedClaimOutcome.OnlineSettlementTakeMoreTimeToDecide, 
                eligibilityForOnlineSettlement: SettleFenceOnline.Eligible,
                includedCoversOnPolicy: AffectedCovers.BuildingAndContents);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim);
        }

        /// <summary>
        /// Lodge a non-dividing fence claim with below details
        /// Policy Cover : HomeOwner Building and Content
        /// Claimant Role : Co Policy Owner
        /// Fence Type : Colorbond
        /// Damaged Fence Length : 20
        /// Damage Painted Fence Length : 0
        /// Damage Sides : Left
        /// Temporary Fence Required : No
        /// Online Settlement : Eligible - Accept Without Bank Details
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.FenceOnly), Category(TestCategory.PrivateFence)]
        [Test(Description = "NonSharedFence HomeBuildingContent CoPolicyOwner Colorbond AcceptWithOutBankDetails")]
        public void INSU_T327_NonSharedFence_HomeBuildingContent_CoPolicyOwner_Colorbond_AcceptWithOutBankDetails()
        {
            var claim = BuildTestDataSparkPrivateFenceClaim(ContactRole.CoPolicyHolder, _homePoliciesForSparkClaims,
                FenceType.Colorbond, 20, 0, leftSide: true,
                expectedClaimOutcome: ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails,
                eligibilityForOnlineSettlement: SettleFenceOnline.Eligible,
                includedCoversOnPolicy: AffectedCovers.BuildingAndContents);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());


            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim);
        }

        /// <summary>
        /// Lodge a non-dividing fence claim with below details
        /// Policy Cover : HomeOwner Building and Content
        /// Claimant Role : Policy Co-owner
        /// Fence Type : Super Six
        /// Damaged Fence Length : 12
        /// Damage Painted Fence Length : 12
        /// Damage Sides : Left
        /// Temporary Fence Required : Yes
        /// Online Settlement : Eligible - Member selects Repair by RAC on Settlement Options
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.FenceOnly), Category(TestCategory.PrivateFence)]
        [Test(Description = "NonSharedFence HomeBuildingContent CoPolicyOwner SuperSix TempFence SettlementOptionsRepairByRac")]
        public void INSU_T332_NonSharedFence_HomeBuildingContent_CoPolicyOwner_SuperSix_TempFence_SettlementOptionsRepairByRac()
        {
            var claim = BuildTestDataSparkPrivateFenceClaim(ContactRole.CoPolicyHolder, _homePoliciesForSparkClaims,
                FenceType.SuperSix, 12, 12, leftSide: true, tempFenceRequired: true,
                expectedClaimOutcome: ExpectedClaimOutcome.OnlineSettlementRepairByRAC,
                eligibilityForOnlineSettlement: SettleFenceOnline.Eligible,
                includedCoversOnPolicy: AffectedCovers.BuildingAndContents);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim);
        }

        /// <summary>
        /// Lodge a non-dividing fence claim with below details
        /// Policy Cover : Home Owner Building and Contents
        /// Claimant Role : Co-Owner
        /// Claim Event: 3 months ago
        /// Fence Type : Hardifence
        /// Damaged Fence Length : 17
        /// Damage Painted Fence Length : 10
        /// Damage Sides : Front; Right
        /// Temporary Fence Required : Yes
        /// Online Settlement: Eligible - Select existing Bank Details on record for online settlement; 
        /// test includes BSB validation before existing Bank Account is selected.
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.FenceOnly), Category(TestCategory.PrivateFence), Category(TestCategory.InsuranceContactService)]
        [Test(Description = "NonSharedFence HomeBuildingContent PolicyHolder Hardifence TempFence AcceptWithExistingBankDetails ChangeContactDetails")]
        public void INSU_T333_NonSharedFence_HomeBuildingContent_PolicyHolder_Hardifence_TempFence_AcceptWithExistingBankDetails_ChangeEmailAddress()
        {
            var claim = BuildTestDataSparkPrivateFenceClaim_ChangeContactInformation_SelectExistingBankAccount(ContactRole.CoPolicyHolder, 
                _homePoliciesForSparkClaims, FenceType.Hardifence, 17, 10, tempFenceRequired: true, 
                frontSide: true, rightSide: true, expectedClaimOutcome: ExpectedClaimOutcome.OnlineSettlementAcceptWithExistingBankDetails, 
                eligibilityForOnlineSettlement: SettleFenceOnline.Eligible,
                includedCoversOnPolicy: AffectedCovers.BuildingAndContents);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim);
        }

        /// <summary>
        /// Lodge a non-dividing fence claim with below details
        /// Policy Cover : HomeOwner Building and Content
        /// Claimant Role : Policy Holder
        /// Fence Type : Super Six
        /// Damaged Fence Length : 11
        /// Damage Painted Fence Length : 11
        /// Damage Sides : Right; Rear
        /// Temporary Fence Required : No
        /// Online Settlement : Eligible - Select "Choose a call from RAC" on the Cash Settlement Fact Sheet
        /// Note: Based off INSU_T328 with some changes (e.g. no temp fence) to cover "Choose a call from RAC"
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.FenceOnly), Category(TestCategory.PrivateFence)]
        [Test(Description = "NonSharedFence HomeOwner PolicyHolder SuperSix EligibleForSettlementSelectsContactMe")]
        public void INSU_T335_NonSharedFence_HomeOwner_PolicyHolder_SuperSix_EligibleForSettlementSelectsContactMe()
        {
            var claim = BuildTestDataSparkPrivateFenceClaim(ContactRole.PolicyHolder, _homePoliciesForSparkClaims,
                FenceType.SuperSix, 13, 13, leftSide: true, frontSide: true, tempFenceRequired: false,
                expectedClaimOutcome: ExpectedClaimOutcome.OnlineSettlementContactMe, 
                eligibilityForOnlineSettlement: SettleFenceOnline.Eligible,
                includedCoversOnPolicy: AffectedCovers.BuildingAndContents);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim);
        }
        #endregion

        #region Test cases helper methods

        private ClaimHome BuildTestDataSparkPrivateFenceClaim(ContactRole contactRole, List<string> policiesForSparkClaims, FenceType fenceType,
                   decimal? lengthDamaged = null, decimal lengthPainted = 0, bool leftSide = false, bool rightSide = false, bool rearSide = false, 
                   bool frontSide = false, bool isPoolFence = false, bool tempFenceRequired = false,
                 ExpectedClaimOutcome expectedClaimOutcome = ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails,
                 SettleFenceOnline eligibilityForOnlineSettlement = SettleFenceOnline.Eligible,
                 AffectedCovers includedCoversOnPolicy = AffectedCovers.BuildingAndContents)
        {
            ClaimHome testData = null;

            foreach (var candidate in policiesForSparkClaims)
            {
                Reporting.Log($"Begin examination of candidate policy {candidate} in detail (This log message is here for the purposes of checking timing).");
                var policyToUse = ShieldHomeDB.FetchHomePolicyDetailsForClaim(candidate);

                if (!ClaimHome.PolicyHasAppropriateCoversForClaimScenario(policyToUse, includedCoversOnPolicy))
                { continue; }

                if (!policyToUse.PolicyHolders.Any(ContactRole => ContactRole.ContactRoles.Contains(contactRole)))
                {
                    Reporting.Log($"Policy {candidate} doesn't have a contact with the role '{contactRole}' to act as the informant so we're " +
                        $"moving on to the next candidate.");
                    continue;
                }

                var claimant = policyToUse.PolicyHolders.Find(x => x.ContactRoles.Contains(contactRole));
                claimant = DataHelper.MapPolicyContactWithPersonAPI(claimant.Id, claimant.ExternalContactNumber, contactRole);

                if (claimant == null || claimant.MobilePhoneNumber == null || claimant.PrivateEmail?.Address == null
                       || !ShieldPolicyDB.IsPolicySuitableForFenceClaims(policyToUse.PolicyNumber))
                { continue; }

                testData = new HomeClaimBuilder()
                       .InitialiseHomeClaimWithBasicData(policyToUse.PolicyNumber, claimant, HomeClaimDamageType.StormDamageToFenceOnly)                       
                       .WithAffectedCover(AffectedCovers.FenceOnly)
                       .WithAccountDetailsForOnlineSettlement(new BankAccount().InitWithRandomValues())
                       .WithEventDateAndTime(DateTime.Now)
                       .WithFenceDamage(fenceType: fenceType,
                                        lengthDamaged: lengthDamaged,
                                        lengthPainted: lengthPainted,
                                        affectedLeftSide: leftSide,
                                        affectedRightSide: rightSide,
                                        affectedRearSide: rearSide,
                                        affectedFrontSide: frontSide,
                                        affectedPoolFence: isPoolFence,
                                        isAreaSafe: !tempFenceRequired,
                                        isDividing: false)
                       .WithFenceSettlementBreakdownCalculator()
                       .WithExpectedOutcomeForTest(expectedClaimOutcome)
                       .WithEligibilityForOnlineSettlement(eligibilityForOnlineSettlement)
                       .Build();
                break;
            }
            Reporting.IsNotNull(testData, "suitable test data has been found, removing number from list");
            policiesForSparkClaims.RemoveAll(x => x == testData.PolicyDetails.PolicyNumber);
            return testData;
        }


        private ClaimHome BuildTestDataSparkPrivateFenceClaim_TroubleMeasuringFence_ChangeMobileNumber(ContactRole contactRole, List<string> policiesForSparkClaims, FenceType fenceType,
                   decimal? lengthDamaged, decimal lengthPainted = 0, bool leftSide = false, bool rightSide = false, bool rearSide = false, 
                   bool frontSide = false, bool isPoolFence = false, bool tempFenceRequired = false,
                 ExpectedClaimOutcome expectedClaimOutcome = ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails,
                 SettleFenceOnline eligibilityForOnlineSettlement = SettleFenceOnline.Eligible,
                 AffectedCovers includedCoversOnPolicy = AffectedCovers.BuildingAndContents)
        {
            ClaimHome testData = null;

            foreach (var candidate in policiesForSparkClaims)
            {
                Reporting.Log($"Begin examination of candidate policy {candidate} in detail (This log message is here for the purposes of checking timing).");
                var policyToUse = ShieldHomeDB.FetchHomePolicyDetailsForClaim(candidate);

                if (!ClaimHome.PolicyHasAppropriateCoversForClaimScenario(policyToUse, includedCoversOnPolicy))
                { continue; }

                if (!policyToUse.PolicyHolders.Any(ContactRole => ContactRole.ContactRoles.Contains(contactRole)))
                {
                    Reporting.Log($"Policy {candidate} doesn't have a contact with the role '{contactRole}' to act as the informant so we're " +
                        $"moving on to the next candidate.");
                    continue;
                }

                var claimant = policyToUse.PolicyHolders.Find(x => x.ContactRoles.Contains(contactRole));
                claimant = DataHelper.MapPolicyContactWithPersonAPI(claimant.Id, claimant.ExternalContactNumber, contactRole);

                if (claimant == null || claimant.MobilePhoneNumber == null || claimant.PrivateEmail?.Address == null
                       || !ShieldPolicyDB.IsPolicySuitableForFenceClaims(policyToUse.PolicyNumber))
                { continue; }

                testData = new HomeClaimBuilder()
                       .InitialiseHomeClaimWithBasicData(policyToUse.PolicyNumber, claimant, HomeClaimDamageType.StormDamageToFenceOnly)
                       .WithNewContactDetailsForClaimant(claimant, changeEmailAddress: false, changeMobileNumber: true)
                       .WithAffectedCover(AffectedCovers.FenceOnly)
                       .WithAccountDetailsForOnlineSettlement(new BankAccount().InitWithRandomValues())
                       .WithEventDateAndTime(DateTime.Now)
                       .WithFenceDamage(fenceType: fenceType,
                                        lengthDamaged: lengthDamaged,
                                        lengthPainted: lengthPainted,
                                        affectedLeftSide: leftSide,
                                        affectedRightSide: rightSide,
                                        affectedRearSide: rearSide,
                                        affectedFrontSide: frontSide,
                                        affectedPoolFence: isPoolFence,
                                        isAreaSafe: !tempFenceRequired,
                                        isDividing: false)
                       .WithFenceSettlementBreakdownCalculator()
                       .WithExpectedOutcomeForTest(expectedClaimOutcome)
                       .WithEligibilityForOnlineSettlement(eligibilityForOnlineSettlement)
                       .Build();
                break;
            }
            Reporting.IsNotNull(testData, "suitable test data has been found, removing number from list");
            policiesForSparkClaims.RemoveAll(x => x == testData.PolicyDetails.PolicyNumber);
            return testData;
        }

        private ClaimHome BuildTestDataSparkPrivateFenceClaim_ChangeEmailAddress(ContactRole contactRole, List<string> policiesForSparkClaims, FenceType fenceType,
                   decimal? lengthDamaged, decimal lengthPainted = 0, bool leftSide = false, bool rightSide = false, bool rearSide = false, 
                   bool frontSide = false, bool isPoolFence = false, bool tempFenceRequired = false,
                    ExpectedClaimOutcome expectedClaimOutcome = ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails,
                    SettleFenceOnline eligibilityForOnlineSettlement = SettleFenceOnline.Eligible,
                    AffectedCovers includedCoversOnPolicy = AffectedCovers.BuildingAndContents)
        {
            ClaimHome testData = null;

            foreach (var candidate in policiesForSparkClaims)
            {
                Reporting.Log($"Begin examination of candidate policy {candidate} in detail (This log message is here for the purposes of checking timing).");
                var policyToUse = ShieldHomeDB.FetchHomePolicyDetailsForClaim(candidate);

                if (!ClaimHome.PolicyHasAppropriateCoversForClaimScenario(policyToUse, includedCoversOnPolicy))
                { continue; }

                if (!policyToUse.PolicyHolders.Any(ContactRole => ContactRole.ContactRoles.Contains(contactRole)))
                {
                    Reporting.Log($"Policy {candidate} doesn't have a contact with the role '{contactRole}' to act as the informant so we're " +
                        $"moving on to the next candidate.");
                    continue;
                }

                var claimant = policyToUse.PolicyHolders.Find(x => x.ContactRoles.Contains(contactRole));
                claimant = DataHelper.MapPolicyContactWithPersonAPI(claimant.Id, claimant.ExternalContactNumber, contactRole);

                if (claimant == null || claimant.MobilePhoneNumber == null || claimant.PrivateEmail?.Address == null
                       || !ShieldPolicyDB.IsPolicySuitableForFenceClaims(policyToUse.PolicyNumber))
                { continue; }

                testData = new HomeClaimBuilder()
                       .InitialiseHomeClaimWithBasicData(policyToUse.PolicyNumber, claimant, HomeClaimDamageType.StormDamageToFenceOnly)
                       .WithNewContactDetailsForClaimant(claimant, changeEmailAddress: true, changeMobileNumber: false)
                       .WithAffectedCover(AffectedCovers.FenceOnly)
                       .WithAccountDetailsForOnlineSettlement(new BankAccount().InitWithRandomValues())
                       .LoginWith(LoginWith.ContactId)
                       .WithEventDateAndTime(DateTime.Now)
                       .WithFenceDamage(fenceType: fenceType,
                                        lengthDamaged: lengthDamaged,
                                        lengthPainted: lengthPainted,
                                        affectedLeftSide: leftSide,
                                        affectedRightSide: rightSide,
                                        affectedRearSide: rearSide,
                                        affectedFrontSide: frontSide,
                                        isAreaSafe: !tempFenceRequired,
                                        isDividing: false)
                       .WithFenceSettlementBreakdownCalculator()
                       .WithExpectedOutcomeForTest(expectedClaimOutcome)
                       .WithEligibilityForOnlineSettlement(eligibilityForOnlineSettlement)
                       .Build();
                break;
            }
            Reporting.IsNotNull(testData, "suitable test data has been found, removing number from list");
            policiesForSparkClaims.RemoveAll(x => x == testData.PolicyDetails.PolicyNumber);
            return testData;
        }

        private ClaimHome BuildTestDataSparkPrivateFenceClaim_ChangeContactInformation_SelectExistingBankAccount(ContactRole contactRole, List<string> policiesForSparkClaims,
            FenceType fenceType, decimal? lengthDamaged, decimal lengthPainted = 0, bool leftSide = false, bool rightSide = false, bool rearSide = false, 
            bool frontSide = false, bool isPoolFence = false, bool tempFenceRequired = false,
            ExpectedClaimOutcome expectedClaimOutcome = ExpectedClaimOutcome.OnlineSettlementAcceptWithExistingBankDetails,
            SettleFenceOnline eligibilityForOnlineSettlement = SettleFenceOnline.Eligible,
            AffectedCovers includedCoversOnPolicy = AffectedCovers.BuildingAndContents)
        {
            ClaimHome testData = null;

            foreach (var candidate in policiesForSparkClaims)
            {
                Reporting.Log($"Begin examination of candidate policy {candidate} in detail (This log message is here for the purposes of checking timing).");
                var policyToUse = ShieldHomeDB.FetchHomePolicyDetailsForClaim(candidate);

                if (!ClaimHome.PolicyHasAppropriateCoversForClaimScenario(policyToUse, includedCoversOnPolicy))
                { continue; }

                if (!policyToUse.PolicyHolders.Any(ContactRole => ContactRole.ContactRoles.Contains(contactRole)))
                {
                    Reporting.Log($"Policy {candidate} doesn't have a contact with the role '{contactRole}' to act as the informant so we're " +
                        $"moving on to the next candidate.");
                    continue;
                }

                var claimant = policyToUse.PolicyHolders.Find(x => x.ContactRoles.Contains(contactRole));
                claimant = DataHelper.MapPolicyContactWithPersonAPI(claimant.Id, claimant.ExternalContactNumber, contactRole);

                if (ShieldPolicyDB.ClaimantHasActiveBankAccount(claimant, policyToUse.PolicyNumber))
                {
                    if (claimant == null || claimant.MobilePhoneNumber == null || claimant.PrivateEmail?.Address == null 
                        || !ShieldPolicyDB.IsPolicySuitableForFenceClaims(policyToUse.PolicyNumber))
                    { continue; }                   

                    testData = new HomeClaimBuilder()
                            .InitialiseHomeClaimWithBasicData(policyToUse.PolicyNumber, claimant, HomeClaimDamageType.StormDamageToFenceOnly)
                            .WithNewContactDetailsForClaimant(claimant, changeEmailAddress: true, changeMobileNumber: true)
                            .WithAffectedCover(AffectedCovers.FenceOnly)
                            .LoginWith(LoginWith.ContactId)
                            //Adding random event time because we're always using Now in other tests
                            .WithEventDateAndTime(DataHelper.GenerateRandomDate(DateTime.Now, DateTime.Now.AddMonths(-3)))
                            .WithFenceDamage(fenceType: fenceType,
                                            lengthDamaged: lengthDamaged,
                                            lengthPainted: lengthPainted,
                                            affectedLeftSide: leftSide,
                                            affectedRightSide: rightSide,
                                            affectedRearSide: rearSide,
                                            affectedFrontSide: frontSide,
                                            isAreaSafe: !tempFenceRequired,
                                            isDividing: false)
                            .WithFenceSettlementBreakdownCalculator()
                            .WithExpectedOutcomeForTest(expectedClaimOutcome)
                            .WithEligibilityForOnlineSettlement(eligibilityForOnlineSettlement)
                            .Build();
                    break;
                }
            }
            Reporting.IsNotNull(testData, "suitable test data has been found, removing number from list");
            policiesForSparkClaims.RemoveAll(x => x == testData.PolicyDetails.PolicyNumber);
            return testData;
        }

        #endregion
    }
}
