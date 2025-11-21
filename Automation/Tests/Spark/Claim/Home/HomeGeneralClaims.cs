using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using Rac.TestAutomation.Common.TestData.Claim;

using Tests.ActionsAndValidations;
using Tests.ActionsAndValidations.Claims;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyHome;
using Rac.TestAutomation.Common.DatabaseCalls.Queries.Environment;


namespace Spark.Claim.Home
{
    [Property("Functional", "Spark Home General Claims application")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class HomeGeneralClaims : BaseUITest
    {
        private List<string> _homePoliciesForSparkClaims;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Spark tests for dividing fence claim");
            _homePoliciesForSparkClaims = ShieldHomeClaimDB.ReturnLandlordAndHomeOwnerPoliciesSuitableForClaims();
        }

        #region Test Cases
        /// <summary>
        /// Lodge a Home Storm Claim claim with below details
        /// Policy Covers claimed against : Contents Only
        /// Claimant Role : Policy Holder
        /// Damage requires assessor: Yes (so we skip the page asking member for detailed list of damage) (isCarpetWaterDamaged flag)
        /// Damage requires restorer: Yes (isCarpetTooWet flag)
        /// Contents beside carpet damaged: No (isOtherStormDamagedContents flag)
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Contents)]
        [Test(Description = "SparkStormClaim HomeContentsOnly CarpetTooWet PolicyHolder ChangeContactDetails")]
        public void SparkStormClaim_HomeContentsOnly_CarpetTooWet_PolicyHolder_ChangeContactDetails()
        {
            var claim = BuildTestDataSparkStormClaimNotFenceOnly(ContactRole.PolicyHolder, _homePoliciesForSparkClaims,
                FenceType.Asbestos, 0,
                isCarpetWaterDamaged: true, isCarpetTooWet: true, isOtherStormDamagedContents: false,
                withAffectedCover: AffectedCovers.ContentsOnly);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());
            
            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim, detailedUiChecking: false);

        }

        /// <summary>
        /// Lodge a Home Storm Claim claim with below details
        /// Policy Covers claimed against : Contents Only
        /// Claimant Role : Policy Co-owner
        /// Damage requires assessor: No (so we WILL ask the member for a detailed list of damage) (isCarpetWaterDamaged flag)
        /// Damage requires restorer: No (isCarpetTooWet flag)
        /// Contents beside carpet damaged: Yes (isOtherStormDamagedContents flag)
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Contents)]
        [Test(Description = "SparkStormClaim HomeContentsOnly SpecificContents PolicyHolder ChangeContactDetails")]
        public void SparkStormClaim_HomeContentsOnly_SpecificContents_PolicyHolder_ChangeContactDetails()
        {
            var claim = BuildTestDataSparkStormClaimNotFenceOnly(ContactRole.CoPolicyHolder, _homePoliciesForSparkClaims,
                FenceType.Asbestos, 0,
                isCarpetWaterDamaged: false, isCarpetTooWet: false, isOtherStormDamagedContents: true,
                withAffectedCover: AffectedCovers.ContentsOnly);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim, detailedUiChecking: true);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim, detailedUiChecking: true);

        }

        /// <summary>
        /// Lodge a Home Storm Claim claim with below details
        /// Policy Covers claimed against : Building Only
        /// Claimant Role : Policy Holder
        /// Damage requires assessor: Yes (so we skip the page asking member for detailed list of damage) (isCarpetWaterDamaged flag)
        /// Damage to fixtures and fittings: Yes
        /// - NOTE: If the insured property is an investment property we use the isCarpetWaterDamaged flag to determine
        ///           the answer to the question regarding damage to fixtures and fittings caused by the storm.
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Building)]
        [Test(Description = "SparkStormClaim HomeBuildingOnly MultiLevelDamage PolicyHolder ChangeContactDetails")]
        public void SparkStormClaim_HomeBuildingOnly_MultiLevelDamage_PolicyHolder_ChangeContactDetails()
        {
            var claim = BuildTestDataSparkStormClaimNotFenceOnly(ContactRole.PolicyHolder, _homePoliciesForSparkClaims,
                FenceType.Asbestos, 0,
                isCarpetWaterDamaged: true,
                isSpecificBuildingItemsDamaged: true, isHomeBadlyDamaged: true, isHomeUnsafe: true,
                withAffectedCover: AffectedCovers.BuildingOnly);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim, detailedUiChecking: true);

        }

        /// <summary>
        /// Lodge a Home Storm Claim claim with below details
        /// Policy Covers claimed against : Building & Contents damage.
        /// Claimant Role : Policy Holder
        /// Damage requires assessor: No (so we WON'T SKIP the page asking member for specific types of damage to the building)
        /// Water Damaged Carpets: No (so we WON'T SKIP the page asking member for detailed list of damaged contents on those grounds)
        /// No Water Damage: Yes (setting all of the rest of the Water Damage checkboxes to false so we don't skip the page asking for 
        ///                     a detailed list of damaged contents, by use of the noWaterDamage flag)
        ///   - NOTE: If the insured property is an investment property we use the same isCarpetWaterDamaged flag to determine
        ///         the answer to the question regarding damage to fixtures and fittings caused by the storm.
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Building), Category(TestCategory.Contents)]
        [Test(Description = "SparkStormClaim HomeBuildingAndContents SingleLevelDamage PolicyHolder_ChangeContactDetails")]
        public void SparkStormClaim_HomeBuildingAndContents_SingleLevelDamage_PolicyHolder_ChangeContactDetails()
        {
            var claim = BuildTestDataSparkStormClaimNotFenceOnly(ContactRole.PolicyHolder, _homePoliciesForSparkClaims,
                FenceType.Asbestos, 0,
                isSpecificBuildingItemsDamaged: true, isHomeBadlyDamaged: false, isHomeUnsafe: false,
                isCarpetWaterDamaged: false, isCarpetTooWet: false, isOtherStormDamagedContents: true,
                avoidRestorerAllocation: true,
                withAffectedCover: AffectedCovers.BuildingAndContents);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim, detailedUiChecking: false);
        }

        /// <summary>
        /// Lodge a Home Storm Claim claim with below details
        /// Policy Covers claimed against : Building, Contents & Fence damage.
        /// Claimant Role : Policy Holder
        /// Damage requires assessor: No (so we WON'T SKIP the page asking member for specific types of damage to the building)
        /// Water Damaged Carpets: No (so we WON'T SKIP the page asking member for detailed list of damaged contents on those grounds)
        ///   - NOTE: If the insured property is an investment property we use the same isCarpetWaterDamaged flag to determine
        ///         the answer to the question regarding damage to fixtures and fittings caused by the storm.
        /// Avoid Restorer Allocation: Yes avoidRestorerAllocation = true (so we don't skip the page asking member for detailed list of damaged contents) 
        /// Fence Type : Hardifence
        /// Damaged Fence Length : 11
        /// Damage Painted Fence Length : 9
        /// Damage Sides : Right
        /// Temporary Fence Required : Yes
        /// No Online Settlement Eligibility with combination claims.
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Building), Category(TestCategory.Contents), Category(TestCategory.SharedFence)]
        [Test(Description = "SparkStormClaim_HomeBuildingContentsAndSharedFence_SingleLevelDamage_PolicyHolder_ChangeContactDetails")]
        public void SparkStormClaim_HomeBuildingContentsAndSharedFence_SingleLevelDamage_PolicyHolder_ChangeContactDetails()
        {
            var claim = BuildTestDataSparkStormClaimNotFenceOnly(ContactRole.PolicyHolder, _homePoliciesForSparkClaims,
                FenceType.Hardifence, 11, 9, rightSide: true, tempFenceRequired: true, sharedFence: true,
                isSpecificBuildingItemsDamaged: true, isHomeBadlyDamaged: false, isHomeUnsafe: false,
                isCarpetWaterDamaged: false, isCarpetTooWet: false, isOtherStormDamagedContents: true,
                avoidRestorerAllocation: true,
                withAffectedCover: AffectedCovers.BuildingAndContentsAndFence);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim, retryOTP: true, detailedUiChecking: true);
        }

        /// <summary>
        /// Lodge a Home Storm Claim claim with below details.
        /// NOTE: This test case diverges from the manual regression test in that:
        ///    i. We don't REQUIRE multiple home policies related to the claimant. 
        ///       As with all automated Home Storm test cases, if multiple home 
        ///       policies DO exist we detect it and handle selection of our chosen 
        ///       policy from the 'Your policy' page.
        ///   ii. We do not currently perform as much validation around the Claim 
        ///       Questionnaires as the manual test.
        ///
        /// Policy Covers claimed against : Building, Contents & Shared Fence damage.
        /// Claimant Role : Policy Holder
        /// Damage requires assessor: Yes - Level 3 (isHomeUnsafe) only
        /// Water Damaged Carpets: No (isCarpetWaterDamaged = false) so we WON'T SKIP the page asking member for detailed list of damaged contents)
        ///   - NOTE: If the insured property is an investment property we use the same isCarpetWaterDamaged flag to determine
        ///         the answer to the question regarding damage to fixtures and fittings caused by the storm.
        /// Avoid Restorer Allocation: Yes (avoidRestorerAllocation = true so we don't set any value which will skip the page asking member for 
        ///                            a detailed list of damaged contents) 
        /// Uninhabitable Home : Yes (isHomeInhabitable: false) 
        ///                     'No access to kitchen or bathroom' 
        ///                     OR BOTH 'No power to the property' AND 'No water supply to the property' 
        /// Fence Type : Glass
        /// Damage Sides : Pool fence
        /// Temporary Fence Required : Yes
        /// No Online Settlement Eligibility with combination claims (SettleFenceOnline.IneligibleClaimNotFenceOnly)
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.Building), Category(TestCategory.Contents), Category(TestCategory.PrivateFence)]
        [Test(Description = "BuildingContentsNonSharedFence Uninhabitable SingleLevelDamage PolicyHolder ChangeContactDetails")]
        public void INSU_T338_BuildingContentsNonSharedFence_Uninhabitable_SingleLevelDamage_PolicyHolder_ChangeContactDetails()
        {
            var claim = BuildTestDataSparkStormClaimNotFenceOnly(ContactRole.PolicyHolder, _homePoliciesForSparkClaims,
                FenceType.Glass, 0, isPoolFence: true, tempFenceRequired: true, sharedFence: false,
                isSpecificBuildingItemsDamaged: false, isHomeBadlyDamaged: false, isHomeUnsafe: true,
                isCarpetWaterDamaged: false, isCarpetTooWet: false, isOtherStormDamagedContents: true,
                avoidRestorerAllocation: true, isHomeInhabitable: false,
                eligibilityForOnlineSettlement: SettleFenceOnline.IneligibleClaimNotFenceOnly,
                withAffectedCover: AffectedCovers.BuildingAndContentsAndFence);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim, detailedUiChecking: true);
        }

        /// <summary>
        /// Lodge a Home Storm Claim claim with below details.
        /// NOTE: This test case diverges from the manual regression test in that:
        ///    i. We do not force the use of a Landlord policy. Our automated test 
        ///       scenarios should complete successfully whether the policy 
        ///       returned is Landlord or Homeowner).
        ///   ii. Fence material is set to always be BrickWall whereas the manual 
        ///       test allows the analyst to choose BrickWall or Wooden. We could 
        ///       add a randomiser but it is considered low value.
        ///  iii. We do not currently perform as much validation around the Claim 
        ///       Questionnaires as the manual test.
        ///
        ///         /// NOTE: This test also checks that the Shield 'Toggle for HGC' value is 'true' as a 
        ///       'smoke test' confirming the Shield environment configuration is as we expect.
        /// 
        /// Policy Covers claimed against : Building & Non-Shared Fence damage.
        /// Claimant Role : Policyholder
        /// Damage requires assessor: No - Level 1 only (isSpecificBuildingItemsDamaged = true)
        /// Specific Damage to Building must include: Other items
        /// Is Carpet Badly Soaked: Yes (isCarpetTooWet = true; we want a restorer to be required)
        /// Avoid Restorer Allocation: No (avoidRestorerAllocation = false; we expect a restorer to be required)
        /// Uninhabitable Home : No (isHomeInhabitable = true)
        /// Safety Checks: None of these (forceNoSafetyChecks = true)
        /// Fence Type : Brick
        /// Damage Sides : Front
        /// Temporary Fence Required : No
        /// No Online Settlement Eligibility with combination claims (SettleFenceOnline.IneligibleClaimNotFenceOnly)
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.Building), Category(TestCategory.PrivateFence)]
        [Test(Description = "BuildingNonSharedFence SpecificItemsOther LevelOneDamageOnly PolicyHolder ForceNoSafetyChecks ChangeContactDetails")]
        public void INSU_T336_BuildingNonSharedFence_SpecificItemsOther_LevelOneDamageOnly_PolicyHolder_ForceNoSafetyChecks_ChangeContactDetails()
        {
            var shieldHgcToggle = ShieldParametersDB.FetchShieldToggleForEnvironment(ShieldToggle.HomeGeneralClaims.GetDescription());
            if (shieldHgcToggle.Value == "true")
            {
                Reporting.Log($"Shield Toggle '{shieldHgcToggle.Param_desc}' value = '{shieldHgcToggle.Value}'");
            }
            else
            {
                Reporting.Error($"Shield Toggle '{shieldHgcToggle.Param_desc}' value should always be 'true' after delivery of " +
                    $"HGC project, but value returned is '{shieldHgcToggle.Value}'." +
                    $"Please check configuration of Shield environment before re-attempting execution!");
            }
            var claim = BuildTestDataSparkStormClaimNotFenceOnly(ContactRole.PolicyHolder, _homePoliciesForSparkClaims,
                FenceType.BrickWall, 0, isPoolFence: false, frontSide: true, tempFenceRequired: false, sharedFence: false,
                isCarpetTooWet: true,
                isSpecificBuildingItemsDamaged: true, isBuildingOnlyOtherItems: true, isHomeBadlyDamaged: false, isHomeUnsafe: false,
                avoidRestorerAllocation: false, isHomeInhabitable: true, forceNoSafetyChecks: true,
                eligibilityForOnlineSettlement: SettleFenceOnline.IneligibleClaimNotFenceOnly,
                withAffectedCover: AffectedCovers.BuildingAndFence);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim, detailedUiChecking: false);
        }

        /// <summary>
        /// Lodge a Home Storm Claim claim with below details.
        /// NOTE: This test case diverges from the manual regression test in that:
        ///       i. We may select either "No" instead of "I'm not sure" for 'Are the carpets so badly soaked that you can't dry them?' 
        ///          on the Contents damage page as they are functionally the same. Analysts running the manual tests are directed to 
        ///          select 'I'm not sure'.
        ///      ii. Attempting to prevent allocation of Service Provider due to being outside of the metropolitan area is non-trivial 
        ///          and may not work. Gathering data for a solution to this.
        ///     iii. Currently we make changes to the policyholder telephone/email infomation when the manual test does not (until MFA gets involved).
        ///      iv. We do not currently perform as much validation around the Claim Questionnaires as the manual test.
        ///       v. We do not currently upload an invoice and check for the resulting 'Invoice Documents Received' event in Shield.
        ///      vi. We do not currently check for the damaged item details provided in the Contents List in Shield.
        ///     vii. We do not currently check for 'Claim unable to be assigned by staff allocation tool' 
        ///     
        /// Policy Covers claimed against : Contents & Shared Fence damage.
        /// Claimant Role : Policyholder
        /// Is Carpet Water Damaged: Yes 
        /// Is Carpet Badly Soaked: No/Unsure (isCarpetTooWet = false)
        /// Specific contents damaged: Yes (isOtherStormDamagedContents = true)
        /// Avoid Restorer Allocation: No (avoidRestorerAllocation = false; we expect a restorer to be required)
        /// Uninhabitable Home : No (isHomeInhabitable = true)
        /// Safety Checks: None of these (forceNoSafetyChecks = true)
        /// Shared Fence Repairs Already Complete (FenceType.NotSure, 0 set strictly for the test framework)
        /// Temporary Fence Required : No
        /// No Online Settlement Eligibility with combination claims (SettleFenceOnline.IneligibleClaimNotFenceOnly)
        ///       
        [Category(TestCategory.Claim), Category(TestCategory.ClaimStorm), Category(TestCategory.Home), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.Contents), Category(TestCategory.SharedFence)]
        [Test(Description = "ContentsAndSharedFence SpecificContentsItem UploadInvoice WaterDamagedCarpet PolicyHolder NoChangeToContactDetails")]
        public void INSU_T337_ContentsAndSharedFence_SpecificContentsItem_UploadInvoice_WaterDamagedCarpet_PolicyHolder_NoChangeToContactDetails()
        {
            var claim = BuildTestDataSparkStormClaimNotFenceOnly(ContactRole.PolicyHolder, _homePoliciesForSparkClaims,
                FenceType.NotSure, 0, sharedFence: true,
                isCarpetWaterDamaged: true, isCarpetTooWet: false, isOtherStormDamagedContents: true,
                expectedClaimOutcome: ExpectedClaimOutcome.RepairsCompleted,
                eligibilityForOnlineSettlement: SettleFenceOnline.IneligibleClaimNotFenceOnly,
                withAffectedCover: AffectedCovers.ContentsAndFence);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            ActionSparkClaimHomeStorm.ReportClaimFrom(_browser, claim);

            ActionsClaimHome.TriageHomeClaim(_browser, claim);
            ActionSparkClaimHomeStorm.ReportSparkClaimHomeStorm(_browser, claim, detailedUiChecking: false);
        }
        #endregion

        #region Test cases helper methods
        private ClaimHome BuildTestDataSparkStormClaimNotFenceOnly(ContactRole contactRole, List<string> policiesForSparkClaims,
                    FenceType fenceType, decimal? lengthDamaged, decimal lengthPainted = 0, bool leftSide = false, bool rightSide = false, 
                    bool rearSide = false, bool frontSide = false, bool isPoolFence = false, bool tempFenceRequired = false, bool sharedFence = true,
                    bool isSpecificBuildingItemsDamaged = true, bool isBuildingOnlyOtherItems = false, bool isHomeBadlyDamaged = true, bool isHomeUnsafe = true,
                    bool isCarpetWaterDamaged = true, bool isCarpetTooWet = true, bool isOtherStormDamagedContents = true, 
                    bool avoidRestorerAllocation = false, bool isHomeInhabitable = true, bool forceNoSafetyChecks = false,
                    ExpectedClaimOutcome expectedClaimOutcome = ExpectedClaimOutcome.NotEligibleForOnlineSettlement,
                    SettleFenceOnline eligibilityForOnlineSettlement = SettleFenceOnline.IneligibleClaimNotFenceOnly,
                    AffectedCovers withAffectedCover = AffectedCovers.ContentsOnly)
        {
            ClaimHome testData = null;
            foreach (var candidate in policiesForSparkClaims)
            {
                Reporting.Log($"Begin examination of candidate policy {candidate} in detail (This log message is here for the purposes of checking timing).");
                var policyToUse = ShieldHomeDB.FetchHomePolicyDetailsForClaim(candidate);

                if (withAffectedCover == AffectedCovers.ContentsOnly ||
                    withAffectedCover == AffectedCovers.ContentsAndFence ||
                    withAffectedCover == AffectedCovers.BuildingAndContents ||
                    withAffectedCover == AffectedCovers.BuildingAndContentsAndFence)
                {
                    if (policyToUse.Covers.Any(c => c.CoverCode == HomeCoverCodes.HCN) || policyToUse.Covers.Any(c => c.CoverCode == HomeCoverCodes.LCN))
                    {
                        Reporting.Log($"Cover for Contents exists on {candidate}, proceeding to next stage of candidate evaluation.");
                    }
                    else
                    {
                        Reporting.Log($"Policy {candidate} doesn't contain Contents cover required for this test scenario, moving on to next candidate.");
                        continue;
                    }
                }

                if (!policyToUse.PolicyHolders.Any(ContactRole => ContactRole.ContactRoles.Contains(contactRole)))
                {
                    Reporting.Log($"Policy {candidate} doesn't have a contact with the role '{contactRole}' to act as the informant so we're " +
                        $"moving on to the next candidate.");
                    continue;
                }


                var claimant = policyToUse.PolicyHolders.Find(x => x.ContactRoles.Contains(contactRole));
                claimant = DataHelper.MapPolicyContactWithPersonAPI(claimant.Id, claimant.ExternalContactNumber, contactRole);

                //TODO: Might consider a different set of validations for other storm claim scenarios, but
                //      for now using IsPolicySuitableForFenceClaims works fine.
                if (claimant == null || claimant.MobilePhoneNumber == null || claimant.PrivateEmail?.Address == null
                       || !ShieldPolicyDB.IsPolicySuitableForFenceClaims(policyToUse.PolicyNumber))
                { continue; }

                var someBrokenStuff = new List<ContentItem>()
                { new ContentItem("iFone", 1750),
                  new ContentItem("Couch", 5000),
                  new ContentItem("Television", 2000)};

                //TODO SPK-6267 - Dynamic specific damage types per test
                var stormDamagedBuildingSpecifics = new List<StormDamagedItemTypes>()
                {
                    StormDamagedItemTypes.Flooring,
                    StormDamagedItemTypes.SolarPanels,
                    StormDamagedItemTypes.GarageDoorOrMotor,
                    StormDamagedItemTypes.TvAerial,
                    StormDamagedItemTypes.ClothesLine,
                    StormDamagedItemTypes.SecuritySystem,
                    StormDamagedItemTypes.Glass,
                    StormDamagedItemTypes.LeadLight
                };


                testData = new HomeClaimBuilder()
                       .InitialiseHomeClaimWithBasicData(policyToUse.PolicyNumber, claimant, HomeClaimDamageType.StormAndTempest)
                       .WithNewContactDetailsForClaimant(claimant, changeEmailAddress: true, changeMobileNumber: true)
                       .WithAffectedCover(withAffectedCover)
                       .WithIsHomeInhabitable(isHomeInhabitable)
                       .WithStormSafetyCheckOptions(isHomeInhabitable, forceNoSafetyChecks)
                       .LoginWith(LoginWith.ContactId)
                       .WithEventDateAndTime(DataHelper.GenerateRandomDate(DateTime.Now, DateTime.Now.AddDays(-13)))
                       .WithBuildingDamage(stormDamagedBuildingSpecifics, isSpecificBuildingItemsDamaged, isBuildingOnlyOtherItems, isHomeBadlyDamaged, isHomeUnsafe)
                       .WithContentsDamage(someBrokenStuff, isCarpetWaterDamaged, isCarpetTooWet, isOtherStormDamagedContents)
                       .WithRandomStormWaterDamage(isCarpetTooWet, avoidRestorerAllocation)
                       .WithFenceDamage(fenceType:         fenceType,
                                        lengthDamaged:     lengthDamaged,
                                        lengthPainted:     lengthPainted,
                                        affectedLeftSide:  leftSide,
                                        affectedRightSide: rightSide,
                                        affectedRearSide:  rearSide,
                                        affectedFrontSide: frontSide,
                                        affectedPoolFence: isPoolFence,
                                        isAreaSafe:       !tempFenceRequired,
                                        isDividing:        sharedFence)
                       .WithExpectedOutcomeForTest(expectedClaimOutcome)
                       .WithEligibilityForOnlineSettlement(eligibilityForOnlineSettlement)
                       .Build();
                break;
            }
            Reporting.IsNotNull(testData, $"suitable test data has been found, removing {testData.PolicyDetails.PolicyNumber} from list");
            policiesForSparkClaims.RemoveAll(x => x == testData.PolicyDetails.PolicyNumber);
            return testData;
        }
        #endregion
    }
}
