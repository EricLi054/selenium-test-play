using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rac.TestAutomation.Common.API;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace Rac.TestAutomation.Common
{
    public class ClaimHome
    {
        // Shield scenario name constants
        private const string SCENARIO_THEFT_ITEMS_OUTSIDE   = "Theft of items outside the House";
        private const string SCENARIO_THEFT_ITEMS_INSIDE    = "Theft of items inside the House";
        private const string SCENARIO_ATTEMPTED_THEFT       = "Housebreaking Damage to Building";
        private const string SCENARIO_ACCIDENTAL_DAMAGE     = "Damage_AccidDamage";
        private const string SCENARIO_ACCIDENTAL_LOSS       = "Damage_ADDamage_Loss";

        // Shield damage code constants
        private const string DAMAGE_CODE_THEFT_ITEMS_OUTSIDE    = "Theft of Contents outside the House";
        private const string DAMAGE_CODE_THEFT_ITEMS_INSIDE     = "Theft of Contents inside the House";
        private const string DAMAGE_CODE_ATTEMPTED_THEFT        = "Housebreaking Damage to Building";
        public static class DamageCode
        {
            public static readonly string SpecifiedPersonalValuables = "Accidental Damage to Specified Personal Valuables";
            public static readonly string UnspecifiedPersonalValuables = "Accidental Damage to Unspecified Personal Valuables";
        }

        public string ClaimNumber { get; set; }
        public GetQuotePolicy_Response PolicyDetails { get; set; }
        /// <summary>
        /// PolicyHolder / Representative who will lodge claim.
        /// </summary>
        public PolicyContactDB      Claimant { get; set; }       
        public DateTime             EventDateTime { get; set; }
        public HomeClaimDamageType  DamageType { get; set; }
        public AffectedCovers       DamagedCovers { get; set; }
        public List<ContentItem> UnspecifiedValuablesOutside { get; set; }
        public List<ContentItem> SpecifiedValuablesOutside { get; set; }
        public StormWaterDamageCheckboxes StormWaterDamageCheckboxes { get; set; }
        public bool                 IsHomeInhabitable { get; set; }
        public StormSafetyCheckboxes StormSafetyCheckboxes { get; set; }
        public GlassDamage          IsGlassDamaged { get; set; }
        public bool                 IsTimberOrLaminateFlooringDamaged { get; set; }
        public GarageDoorDamage     IsGarageDoorDamaged { get; set; }
        public LoginWith            LoginWith { get; set; }
        /// <summary>
        /// Used in "More About Your Damage" in Spark Home Claims
        /// Used in "How Damage Occurred" field in B2C Home Claims
        /// </summary>
        public string               AccountOfEvent { get; set; }
        /// <summary>
        /// To set no police involvement, leave this null
        /// </summary>
        public string       PoliceReportNumber { get; set; }
        public DateTime     PoliceReportDate { get; set; }

        public FenceDamageDetails       FenceDamage { get; set; }
        
        public BuildingDamageDetails    BuildingDamage { get; set; }
        public ContentsDamageDetails    ContentsDamage { get; set; }
        public TheftDamageDetails       TheftDamage { get; set; }
        public List<Contact>            ThirdParty { get; set; }
        public List<Contact>            Witness { get; set; }
        public ExpectedClaimOutcome     ExpectedOutcome { get; set; }
        public SettleFenceOnline        EligibilityForOnlineSettlement { get; set; }
        public BankAccount              AccountForSettlement { get; set; }
        public List<PolicyDetail>       LinkedHomePolicies { get; set; }
        public FenceSettlementBreakdown FenceSettlementBreakdown { get; set; }
        /// <summary>
        /// Returns a list of expected damage codes which would be
        /// found in Shield for this claim data. The damage code is
        /// comprised of the declared damage type and affected cover.
        /// </summary>
        /// <returns></returns>
        public List<ClaimHomeDamageDetails> ExpectedAffectedCoversForThisClaim()
        {
            // Theft/Break-in is quite unique so providing specialised handling.
            if (DamageType == HomeClaimDamageType.Theft)
                return ExpectedAffectedCoversForTheftScenario();

            var expectedAffectedCovers = new List<ClaimHomeDamageDetails>();
            var coverNames = new List<string>();
            var baseDamageDetail = HomeClaimDamageTypeNames[DamageType].TextShield;

            if (DamageType == HomeClaimDamageType.StormDamageToFenceOnly)
            {
                coverNames.Add(SHIELD_CLAIM_COVER_FENCE);
            }
            else
            {

                if (DamagedCovers == AffectedCovers.ContentsOnly ||
                    DamagedCovers == AffectedCovers.BuildingAndContents)
                {
                    coverNames.Add(SHIELD_CLAIM_COVER_CONTENTS);
                }

                if (DamagedCovers == AffectedCovers.BuildingOnly ||
                    DamagedCovers == AffectedCovers.BuildingAndContents)
                {
                    coverNames.Add(SHIELD_CLAIM_COVER_BUILDING);
                    if (FenceDamage != null)
                        coverNames.Add(SHIELD_CLAIM_COVER_FENCE);
                }
            }

            foreach (var coverName in coverNames)
            {
                expectedAffectedCovers.Add(new ClaimHomeDamageDetails()
                {
                    DamageCode = $"{baseDamageDetail} Damage to {coverName}"
                });
            }

            return expectedAffectedCovers;
        }

        public List<string> ExpectedDamageCodesForPersonalValuables()
        {
            var expectedDamageCodes = new List<string>();
            if (DamageType == HomeClaimDamageType.AccidentalDamage)
            {
                if (PolicyDetails.HasOVSCover())
                {
                    expectedDamageCodes.Add(DamageCode.SpecifiedPersonalValuables);
                }
                if (PolicyDetails.HasOVUCover())
                {
                    expectedDamageCodes.Add(DamageCode.UnspecifiedPersonalValuables);
                }
            }
            return expectedDamageCodes;
        }

        /// <summary>
        /// Shield uses a single string as a summary of the claim scenario. This
        /// is generally a simple value based on the damage type, but for theft
        /// cases it can be quite unique based on the affected covers.
        /// </summary>
        /// <returns></returns>
        public string ExpectedShieldScenarioName()
        {
            var expectedScenarioName = HomeClaimTypeAndScenarioName[DamageType];
            if (DamageType == HomeClaimDamageType.Theft)
            {
                // Shield recorded scenario is based on a descending order:
                if (DamagedCovers != AffectedCovers.BuildingOnly)
                {
                    expectedScenarioName = TheftDamage.LocationOfStolenItems == StolenItemsLocation.Outside ?
                        SCENARIO_THEFT_ITEMS_OUTSIDE :
                        SCENARIO_THEFT_ITEMS_INSIDE;
                }
                else
                {
                    expectedScenarioName = SCENARIO_ATTEMPTED_THEFT;
                }
            }
            if (DamageType == HomeClaimDamageType.AccidentalDamage)
            {
                expectedScenarioName = SCENARIO_ACCIDENTAL_DAMAGE;
            }
            return expectedScenarioName;
        }

        /// <summary>
        /// Retrieves a list of asset items for a given policy number.
        /// If no assets are found, returns null instead of throwing an error.
        /// This allows the calling method to handle the absence of assets appropriately.
        /// </summary>
        public List<AssetItem> GetAllPersonalValuables()
        {
            if (PolicyDetails.HomeAsset.AssetItems != null)
            {
                return PolicyDetails.HomeAsset.AssetItems.FindAll(x => x.AssetItemType != 1000004);
            }
            return null;
        }

        public override string ToString()
        {
            var config = Config.Get();
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine($"Counted a total of {LinkedHomePolicies.Count} Home policies which qualify for a Storm claim and are related to the claimant {Claimant.Id}.{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            if (config.IsMyRACLoginEnabled() &&
                (config.Shield.Environment != "shieldint2" || config.Shield.Environment != "shielduat6"))
            {
                if (Claimant.NewMobilePhoneNumber != null || Claimant.PrivateEmail.NewAddress != null)
                {

                    formattedString.AppendLine($"--- Changing contact details{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($"New mobile telephone number: {Claimant.NewMobilePhoneNumber}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($"New private email address: {Claimant.PrivateEmail.NewAddress}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                }
            }
            else
            {
                formattedString.AppendLine($"Because this test will not be able to update contact information via myRAC, any changes which would " +
                    $"otherwise be made to the contact email address or mobile telephone number will not be performed.{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            }
            formattedString.AppendLine($"--- Home Claim data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Policy number:  {PolicyDetails.PolicyNumber}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Claimant Id:    {Claimant.Id}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Claim type:     {HomeClaimDamageTypeNames[DamageType].TextB2C}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Event datetime: {EventDateTime.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH)} {EventDateTime.ToString(DataFormats.TIME_FORMAT_24HR)}{Reporting.HTML_NEWLINE}");
            if (DamageType == HomeClaimDamageType.StormAndTempest &&
                (DamagedCovers == AffectedCovers.BuildingOnly ||
                 DamagedCovers == AffectedCovers.BuildingAndFence ||
                 DamagedCovers == AffectedCovers.BuildingAndContents ||
                 DamagedCovers == AffectedCovers.BuildingAndContentsAndFence))
            {
                formattedString.AppendLine($"--- Storm Damage (Building):{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Level 1- Specific Damages:     {BuildingDamage.IsSpecificItemsDamaged.ToString()}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Level 2- General Damages:      {BuildingDamage.IsHomeBadlyDamaged.ToString()}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Level 3- Building is Unsafe:   {BuildingDamage.IsHomeUnsafe.ToString()}{Reporting.HTML_NEWLINE}");
                if (BuildingDamage.StormDamagedBuildingSpecifics.Contains(StormDamagedItemTypes.OtherItems)
                    && BuildingDamage.IsSpecificItemsDamaged
                    && !BuildingDamage.IsHomeBadlyDamaged
                    && !BuildingDamage.IsHomeUnsafe)
                {
                    formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                    formattedString.AppendLine($"Specific Storm Damage categories: {Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($"'Other items' has been specified for this claim scenario so will be the only selection made.{Reporting.HTML_NEWLINE}");
                }
                else
                {
                    if (BuildingDamage.IsSpecificItemsDamaged
                        && !BuildingDamage.IsHomeBadlyDamaged
                        && !BuildingDamage.IsHomeUnsafe)
                    {
                        formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                        formattedString.AppendLine($"Specific Storm Damage categories: {Reporting.HTML_NEWLINE}");
                        formattedString.AppendLine($"Number of categories: {BuildingDamage.StormDamagedBuildingSpecifics.Count}{Reporting.HTML_NEWLINE}");
                        var itemsInClaim = BuildingDamage.StormDamagedBuildingSpecifics;
                        foreach (var itemInClaim in itemsInClaim)
                        {
                            formattedString.AppendLine($"Damage type: {itemInClaim}{Reporting.HTML_NEWLINE}");
                        }
                        formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                    }
                }
                if (StormSafetyCheckboxes != null)
                {
                    formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                    formattedString.AppendLine($"--- Storm Safety Checks:{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($"    {StormSafetyCheckOptions.Insecure.GetDescription()}            : {StormSafetyCheckboxes.Insecure}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($"    {StormSafetyCheckOptions.DangerousLooseItems.GetDescription()} : {StormSafetyCheckboxes.DangerousLooseItems}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($"    {StormSafetyCheckOptions.NoPower.GetDescription()}             : {StormSafetyCheckboxes.NoPower}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($"    {StormSafetyCheckOptions.NoWater.GetDescription()}             : {StormSafetyCheckboxes.NoWater}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($"    {StormSafetyCheckOptions.NoAccessKitchenBath.GetDescription()} : {StormSafetyCheckboxes.NoAccessKitchenBath}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($"    {StormSafetyCheckOptions.NoneOfThese.GetDescription()}         : {StormSafetyCheckboxes.NoneOfThese}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                }
                if (DamagedCovers == AffectedCovers.BuildingOnly
                   || DamagedCovers == AffectedCovers.BuildingAndContents
                   || DamagedCovers == AffectedCovers.BuildingAndFence
                   || DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
                {
                    // TODO: INSU-818: Once gone live, we can remove the toggle and treat as always true.
                    if (Config.Get().IsClaimHomeMoreAboutYourDamageScreenEnabled())
                    {
                        formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                        formattedString.AppendLine($"--- More about your damage:{Reporting.HTML_NEWLINE}");
                        formattedString.AppendLine($" Describe the situation and damage =                                 {AccountOfEvent}{Reporting.HTML_NEWLINE}");
                    }
                }
                if (StormWaterDamageCheckboxes != null)
                {
                    formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                    formattedString.AppendLine($"--- Water Damage checkboxes:{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($" No water damage =                                 {StormWaterDamageCheckboxes.NoWaterDamage}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($" Damp patches or dripping =                        {StormWaterDamageCheckboxes.DampPatchesOrDripping}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($" Solid timber flooring is wet =                    {StormWaterDamageCheckboxes.SolidTimberFloorIsWet}{Reporting.HTML_NEWLINE}");
                    if (DamagedCovers == AffectedCovers.BuildingAndContents
                        || DamagedCovers == AffectedCovers.BuildingAndContentsAndFence
                        || PolicyDetails.HomeAsset.OccupancyType == "I" 
                        || PolicyDetails.HomeAsset.OccupancyType == "T")
                    {
                        formattedString.AppendLine($" Carpet is so badly soaked that you can't dry it = {StormWaterDamageCheckboxes.BadlySoakedCarpets}{Reporting.HTML_NEWLINE}");
                    }
                    else
                    {
                        formattedString.AppendLine($" 'Carpet is so badly soaked that you can't dry it' checkbox will NOT be displayed " +
                            $"if Contents cover isn't involved in combination claim and property is Owner Occupied.{Reporting.HTML_NEWLINE}");
                    }
                    formattedString.AppendLine($" House is flooded =                                {StormWaterDamageCheckboxes.HouseIsFlooded}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($" There's sewage or drain water in the house =      {StormWaterDamageCheckboxes.SewageOrDrainWaterInTheHouse}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($" Water in the electrics =                          {StormWaterDamageCheckboxes.WaterInTheElectrics}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($" Other water damage =                              {StormWaterDamageCheckboxes.OtherWaterDamage}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                }
            }
            if (DamageType == HomeClaimDamageType.StormAndTempest && 
                (DamagedCovers == AffectedCovers.ContentsOnly ||
                 DamagedCovers == AffectedCovers.ContentsAndFence ||
                 DamagedCovers == AffectedCovers.BuildingAndContents ||
                 DamagedCovers == AffectedCovers.BuildingAndContentsAndFence))
            {
                formattedString.AppendLine($"--- Storm Damage (Contents):{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Water damage to carpet:        {ContentsDamage.IsWaterDamagedCarpets.ToString()}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Member unable to dry carpet (if False we may select 'I'm not sure' instead of 'No'):   {ContentsDamage.IsCarpetTooWet.ToString()}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Damage to other contents:      {ContentsDamage.IsOtherStormDamagedContents.ToString()}{Reporting.HTML_NEWLINE}");
                if (ContentsDamage.IsOtherStormDamagedContents)
                {
                    formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                    formattedString.AppendLine($"Number of storm damaged items: {ContentsDamage.StormDamagedItems.Count}{Reporting.HTML_NEWLINE}");
                    var itemsInClaim = ContentsDamage.StormDamagedItems;
                    foreach (var itemInClaim in itemsInClaim)
                    {
                        formattedString.AppendLine($"Damaged item value: {itemInClaim.Value} Damaged item description: {itemInClaim.Description}{Reporting.HTML_NEWLINE}");
                    }
                    formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                }
            }
            if (FenceDamage != null &&
                (DamagedCovers == AffectedCovers.FenceOnly ||
                 DamagedCovers == AffectedCovers.ContentsAndFence ||
                 DamagedCovers == AffectedCovers.BuildingAndFence ||
                 DamagedCovers == AffectedCovers.BuildingAndContentsAndFence))
            {
                formattedString.AppendLine($"--- Fence damage:{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Eligibility for online settlement: {EligibilityForOnlineSettlement}{Reporting.HTML_NEWLINE}");
                if(FenceDamage.FenceMaterial == FenceType.BrickWall
                || FenceDamage.FenceMaterial == FenceType.Glass
                || FenceDamage.FenceMaterial == FenceType.MoreThanOneFence
                || FenceDamage.FenceMaterial == FenceType.NotSure
                || FenceDamage.FenceMaterial == FenceType.Other
                || FenceDamage.FenceMaterial == FenceType.Wooden)
                {
                    formattedString.AppendLine($"    Fence type:     {FenceTypeNames[FenceDamage.FenceMaterial].TextSpark} (length will not be captured and '0' will be reported to Shield - ref SPK-6882){Reporting.HTML_NEWLINE}");
                }
                else
                {
                    formattedString.AppendLine($"    Fence type:     {FenceTypeNames[FenceDamage.FenceMaterial].TextSpark} ({FenceDamage.MetresPanelsDamaged} metres/panels){Reporting.HTML_NEWLINE}");
                }
                formattedString.AppendLine($"    Affected sides: Front: {FenceDamage.AffectedBoundaryFront}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"                     Rear: {FenceDamage.AffectedBoundaryRear}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"                    Right: {FenceDamage.AffectedBoundaryRight}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"                     Left: {FenceDamage.AffectedBoundaryLeft}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"                     Pool: {FenceDamage.AffectedPoolFence}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"           Dividing fence: {FenceDamage.IsDividingFence}{Reporting.HTML_NEWLINE}");
            }
            var thirdPartyCount = ThirdParty != null ? ThirdParty.Count : 0;
            formattedString.AppendLine($"--- Third Party: {thirdPartyCount} individuals{Reporting.HTML_NEWLINE}");

            var witnessCount = Witness != null ? Witness.Count : 0;
            formattedString.AppendLine($"--- Witnesses:   {witnessCount} individuals{Reporting.HTML_NEWLINE}");

            return formattedString.ToString();
        }

        private List<ClaimHomeDamageDetails> ExpectedAffectedCoversForTheftScenario()
        {
            var expectedAffectedCovers = new List<ClaimHomeDamageDetails>();
            if (DamagedCovers != AffectedCovers.ContentsOnly)
            {
                expectedAffectedCovers.Add(new ClaimHomeDamageDetails()
                {
                    DamageCode = DAMAGE_CODE_ATTEMPTED_THEFT
                });
            }
            if (DamagedCovers != AffectedCovers.BuildingOnly)
            {
                if (TheftDamage.LocationOfStolenItems == StolenItemsLocation.Inside ||
                    TheftDamage.LocationOfStolenItems == StolenItemsLocation.Both)
                {
                    expectedAffectedCovers.Add(new ClaimHomeDamageDetails()
                    {
                        DamageCode = DAMAGE_CODE_THEFT_ITEMS_INSIDE
                    });
                }
                if (TheftDamage.LocationOfStolenItems == StolenItemsLocation.Outside ||
                    TheftDamage.LocationOfStolenItems == StolenItemsLocation.Both)
                {
                    expectedAffectedCovers.Add(new ClaimHomeDamageDetails()
                    {
                        DamageCode = DAMAGE_CODE_THEFT_ITEMS_OUTSIDE
                    });
                }
            }
            return expectedAffectedCovers;
        }

        /// <summary>
        /// Evaluates whether the covers we wish to exist on this policy are included.
        /// </summary>
        /// <param name="policyToUse">The candidate policy to examine</param>
        /// <param name="includedCoversOnPolicy">The covers we want to be included on the policy for this test scenario.</param>
        /// <returns></returns>
        public static bool PolicyHasAppropriateCoversForClaimScenario(HomePolicy policyToUse, AffectedCovers includedCoversOnPolicy)
        {
            bool result = false;
            if (includedCoversOnPolicy == AffectedCovers.BuildingAndContents)
            {
                if ((policyToUse.Covers.Any(c => c.CoverCode == HomeCoverCodes.HB) || policyToUse.Covers.Any(c => c.CoverCode == HomeCoverCodes.LB))
                 && (policyToUse.Covers.Any(c => c.CoverCode == HomeCoverCodes.HCN) || policyToUse.Covers.Any(c => c.CoverCode == HomeCoverCodes.LCN)))
                {
                    Reporting.Log($"Covers for Building and Contents exist on {policyToUse.PolicyNumber}, proceeding to next stage of candidate evaluation.");
                    result = true;
                }
                else
                {
                    Reporting.Log($"Policy {policyToUse.PolicyNumber} doesn't contain all required cover types, moving on to next candidate.");
                    result = false;
                }
            }
            else
            {
                Reporting.Log($"This test only requires Building cover so we know all candidate policies will have that if they qualified for a Fence claim.");
                result = true;
            }
            return result;
        }

        /// <summary>
        /// For Home claims, its check if new mobile number provided
        /// </summary>
        public bool IsMobileNumberChanged => !string.IsNullOrEmpty(Claimant.NewMobilePhoneNumber) || string.IsNullOrEmpty(Claimant.MobilePhoneNumber);

        /// <summary>
        /// For Home claims, its check if new email provided
        /// </summary>
        public bool IsEmailAddressChanged => !string.IsNullOrEmpty(Claimant.PrivateEmail?.NewAddress) || string.IsNullOrEmpty(Claimant.PrivateEmail?.Address);

        /// <summary>
        /// For Home claims, check if excess is more than repair cost
        /// </summary>
        public bool IsRepairCostLessThanExcess { get; set; }
    }

    public class BuildingDamageDetails
    {
        public bool IsSpecificItemsDamaged { get; set; }
        public bool IsHomeBadlyDamaged { get; set; }
        public bool IsHomeUnsafe { get; set; }
        /// <summary>
        /// For storm claims where ONLY the first level of Building damage is reported
        /// at least one category of damaged items should be selected.
        /// </summary>
        public List<StormDamagedItemTypes> StormDamagedBuildingSpecifics { get; set; }
    }

    public class ContentsDamageDetails
    {
        public bool IsWaterDamagedCarpets       { get; set; }
        public bool IsCarpetTooWet              { get; set; }
        public bool IsOtherStormDamagedContents { get; set; }
        /// <summary>
        /// For storm claims which impact specific contents rather than just carpet, 
        /// this should contain at least one item.
        /// </summary>
        public List<ContentItem> StormDamagedItems { get; set; }
    }

    public class FenceDamageDetails
    {
        public FenceType FenceMaterial { get; set; }
        public bool IsDividingFence { get; set; }
        //making MetresPanelsDamaged as null able to driver Having trouble messaging checkbox
        public decimal?  MetresPanelsDamaged { get; set; }
        public decimal MetresPanelsPainted { get; set; }
        public bool AffectedBoundaryFront { get; set; }
        public bool AffectedBoundaryRear { get; set; }
        public bool AffectedBoundaryLeft { get; set; }
        public bool AffectedBoundaryRight { get; set; }
        public bool AffectedPoolFence { get; set; }
        public bool IsAreaSafe { get; set; }
        /// <summary>
        /// If NULL no temp fence required.
        /// If has a value, then YES will be answered for temp
        /// fence and string entered for justification
        /// </summary>
        public string TemporaryFenceRequired { get; set; }
    }

    public class StormWaterDamageCheckboxes
    {
        public bool NoWaterDamage { get; set; }
        public bool DampPatchesOrDripping { get; set; }
        public bool SolidTimberFloorIsWet { get; set; }
        public bool BadlySoakedCarpets { get; set; }
        public bool HouseIsFlooded { get; set; }
        public bool SewageOrDrainWaterInTheHouse { get; set; }
        public bool WaterInTheElectrics { get; set; }
        public bool OtherWaterDamage { get; set; }
    }

    public class StormSafetyCheckboxes
    {
        public bool Insecure { get; set; }
        public bool DangerousLooseItems { get; set; }
        public bool NoPower { get; set; }
        public bool NoWater { get; set; }
        public bool NoAccessKitchenBath { get; set; }
        public bool NoneOfThese { get; set; }
    }

    public class FenceSettlementBreakdown
    {
        public decimal NumberOfMetresClaimed { get; set; }
        public decimal TotalRepairCost { get; set; }
        public decimal RepairCostBeforeExcess { get; set; }
        public decimal RepairCostBeforeAdjustments { get; set; }
        public decimal CurrentExcess { get; set; }
        /// <summary>
        /// !!! IMPORTANT !!!
        /// DisposalContribution is no longer expected to be utilised as it was effectively replaced 
        /// by asbestosInspectionFee. It remains here because it IS still included in the API.
        /// </summary>
        public decimal DisposalContribution { get; set; }
        /// <summary>
        /// The amount paid for an inspection to be carried out to ensure asbestos fencing has been 
        /// removed properly. It only applies if the total length of damaged fence is 5m or more.
        /// The cost of actual DISPOSAL for asbestos fencing is included in the total repair cost.
        /// </summary>
        public decimal AsbestosInspectionFee { get; set; }
        /// <summary>
        /// If the fence is a Dividing Fence then RAC pays half of the calculated cost, as the 
        /// owner/insurer of the neighbouring property is responsible for the other half.
        /// </summary>
        public decimal DividingFenceAdjustment { get; set; }
        public decimal PaintingCost { get; set; }
    }

    public class TheftDamageDetails
    {
        public bool   IsEntryPointKnown { get; set; }
        public string OffenderEntryDescription { get; set; }
        public bool   IsEntryPointSecuredOrRepaired { get; set; }
        public StolenItemsLocation LocationOfStolenItems { get; set; }
        public List<ContentItem>             StolenItems { get; set; }
    }
}
