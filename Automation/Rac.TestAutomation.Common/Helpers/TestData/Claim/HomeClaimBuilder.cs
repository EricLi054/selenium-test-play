using Rac.TestAutomation.Common.API;
using System;
using System.Collections.Generic;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;

namespace Rac.TestAutomation.Common.TestData.Claim
{
    public class HomeClaimBuilder : EntityBuilder<ClaimHome, HomeClaimBuilder>
    {
        private Config _config;

        public HomeClaimBuilder()
        {
            _config = Config.Get();
        }

        /// <summary>
        /// For setting the claimant, picks randomly from the list
        /// provided in the `policyDetails` which we'd normally
        /// expect to have been filled from a SQL query.
        /// </summary>       
        public HomeClaimBuilder WithPolicyDetails(string policyNumber, PolicyContactDB claimant)
        {
            var policyDetails = DataHelper.GetPolicyDetails(policyNumber);
            /// Note that this does not pick up any policies related to other Shield Contacts linked 
            /// via Member Central, as the spark application does not either (ref AUNT-128).
            var linkedHomePolicies = DataHelper.GetAllPoliciesOfTypeForPolicyHolder(claimant.Id, "Home");

            Set(x => x.PolicyDetails, policyDetails);
            Set(x => x.Claimant, claimant);
            Set(x => x.LinkedHomePolicies, linkedHomePolicies);

            return this;
        }

        public HomeClaimBuilder LoginWith(LoginWith contactOrPolicy)
        {
            Set(x => x.LoginWith, contactOrPolicy);
            return this;
        }

        /// <summary>
        /// Invokes PopulateMockMemberCentralWithLatestContactIdInfo to ensure MC Mock record exists, and if
        /// email or mobile telephone (or both) are to be updated generates the new value to be provided.
        /// </summary>
        /// <param name="contact">Shield Contact ID of the Claimant to be used</param>
        /// <param name="changeEmailAddress">If true, update email address from current value.</param>
        /// <param name="changeMobileNumber">If true, update telephone number from current value.</param>
        /// <returns></returns>
        public HomeClaimBuilder WithNewContactDetailsForClaimant(PolicyContactDB contact, bool changeEmailAddress = false, bool changeMobileNumber = false)
        {
            MemberCentral.PopulateMockMemberCentralWithLatestContactIdInfo(contact.Id);

            var config = Config.Get();

            if (config.IsMyRACLoginEnabled() &&
                   (config.Shield.Environment != "shieldint2" || config.Shield.Environment != "shielduat6"))
            {
                if (string.IsNullOrEmpty(contact.PrivateEmail?.Address))
                {
                    contact.PrivateEmail = DataHelper.RandomEmail(contact.FirstName, contact.Surname, domain: _config.Email?.Domain);
                }
                else if (changeEmailAddress)
                {
                    contact.PrivateEmail.NewAddress = DataHelper.RandomEmail(contact.FirstName, contact.Surname, domain: _config.Email?.Domain).Address;
                }

                if (changeMobileNumber || string.IsNullOrEmpty(contact.MobilePhoneNumber))
                {
                    contact.NewMobilePhoneNumber = DataHelper.RandomMobileNumber();
                }

                Reporting.Log($"Generating a new email address based on member name to use registering a myRAC account because " +
                    $"as we need an email in the '{_config.Email?.Domain}' domain if we need to retrieve a verification code." +
                    $"This email must not be based on Private Email etc as it will not change.");
                contact.LoginEmail = DataHelper.LoginEmail(contact.FirstName, contact.Surname, domain: _config.Email?.Domain);
                Reporting.Log($"Login Email generated: {contact.LoginEmail}");
            }
           

            Set(x => x.Claimant, contact);

            return this;
        }

        public HomeClaimBuilder WithEventDateAndTime(DateTime dateAndTimeOfDamage)
        {
            Set(x => x.EventDateTime, dateAndTimeOfDamage);
            return this;
        }

        public HomeClaimBuilder WithDamageType(HomeClaimDamageType damageType)
        {
            Set(x => x.DamageType, damageType);
            return this;
        }

        public HomeClaimBuilder WithRandomAccountOfEvent()
        {
            Set(x => x.AccountOfEvent, GetRandomMultiLineText());
            return this;
        }

        public HomeClaimBuilder WithAccountOfEvent(string eventDescription)
        {
            Set(x => x.AccountOfEvent, eventDescription);
            return this;
        }

        public HomeClaimBuilder WithNoPoliceReport()
        {
            Set(x => x.PoliceReportNumber, null);
            return this;
        }

        public HomeClaimBuilder WithPoliceReport(string reportNumber, DateTime reportDate)
        {
            Set(x => x.PoliceReportNumber, reportNumber);
            Set(x => x.PoliceReportDate, reportDate);
            return this;
        }

        public HomeClaimBuilder WithRandomPoliceReport()
        {
            var eventDate = Get(x => x.EventDateTime);
            var daysRangeForPoliceReport = DateTime.Now.Subtract(eventDate.Date).Days;


            Set(x => x.PoliceReportNumber, DataHelper.RandomAlphanumerics(5, 15));
            Set(x => x.PoliceReportDate, DateTime.Now.AddDays(-DataHelper.RandomNumber(0,daysRangeForPoliceReport)));
            return this;
        }

        public HomeClaimBuilder WithFenceDamage(FenceDamageDetails damageDetails)
        {
            Set(x => x.FenceDamage, damageDetails);
            return this;
        }

        public HomeClaimBuilder WithFenceDamage(FenceType fenceType,
                                                decimal? lengthDamaged,
                                                decimal lengthPainted = 0,
                                                bool isDividing = false,
                                                bool isAreaSafe = true,
                                                bool affectedLeftSide = false,
                                                bool affectedRightSide = false,
                                                bool affectedFrontSide = false,
                                                bool affectedRearSide = false,
                                                bool affectedPoolFence = false,
                                                string tempFenceReason = null)
        {
            var damageDetails = new FenceDamageDetails()
            {
                FenceMaterial           = fenceType,
                IsDividingFence         = isDividing,
                MetresPanelsDamaged     = lengthDamaged,
                MetresPanelsPainted     = lengthPainted,
                IsAreaSafe              = isAreaSafe,
                TemporaryFenceRequired  = tempFenceReason,
                AffectedBoundaryLeft    = affectedLeftSide,
                AffectedBoundaryRight   = affectedRightSide,
                AffectedBoundaryFront   = affectedFrontSide,
                AffectedBoundaryRear    = affectedRearSide,
                AffectedPoolFence       = affectedPoolFence
            };
            
            if (damageDetails.FenceMaterial == FenceType.BrickWall
                || damageDetails.FenceMaterial == FenceType.Glass
                || damageDetails.FenceMaterial == FenceType.MoreThanOneFence
                || damageDetails.FenceMaterial == FenceType.NotSure
                || damageDetails.FenceMaterial == FenceType.Other
                || damageDetails.FenceMaterial == FenceType.Wooden)
            {
                Reporting.Log($"Overriding length of damaged fence to 0 because Fence Material = '{damageDetails.FenceMaterial}'");
                damageDetails.MetresPanelsDamaged = 0;
                damageDetails.MetresPanelsPainted = 0;
            }
            return WithFenceDamage(damageDetails);
        }

        public HomeClaimBuilder WithFenceSettlementBreakdownCalculator()
        {
            Set(x => x.FenceSettlementBreakdown, new FenceSettlementBreakdown());
            return this;
        }

        public HomeClaimBuilder WithTheftDamage(StolenItemsLocation stolenItemsLocation,
                                                bool isEntryPointKnown = false,
                                                bool isEntryPointSecure = false)
        {
            var theftDamage = new TheftDamageDetails()
            {
                LocationOfStolenItems = stolenItemsLocation,
                IsEntryPointKnown = isEntryPointKnown,
                IsEntryPointSecuredOrRepaired = isEntryPointSecure,
                OffenderEntryDescription = GetRandomMultiLineText(),
                StolenItems = new List<ContentItem>()
            };

            Set(x => x.TheftDamage, theftDamage);
            return this;
        }

        public HomeClaimBuilder AddStolenItem(string description, int amount)
        {
            var theftDetails = GetOrDefault(x => x.TheftDamage);
            if (theftDetails == null)
            {
                Reporting.Error($"Must call {nameof(WithTheftDamage)} first to initialise theft damage details, before adding stolen items.");
            }

            theftDetails.StolenItems.Add(new ContentItem() { Description = description, Value = amount });

            Set(x => x.TheftDamage, theftDetails);
            return this;
        }

        /// <summary>
        /// Generates Building Damage for the test scenario, determining which levels of damage are selected and
        /// populating a list of specific types of damage to be input if only the first level is selected.
        /// 
        /// We only include 'Other items' in the specific damaged items if it is flagged for a particular test, 
        /// using isBuildingOnlyOtherItems.
        /// Selecting this option will prevent allocation of service providers and the claim agenda steps to 
        /// progress as normal.
        /// </summary>
        /// <param name="stormDamagedItemTypes">A list of damaged item types passed in.</param>
        /// <param name="isSpecificItemsDamaged">If TRUE, we may be providing the stormDamagedItemTypes information.</param>
        /// <param name="isBuildingOnlyOtherItems">If this is TRUE we will ONLY select 'Other items' from the damage types if given the opportunity. Otherwise we avoid selecting this.</param>
        /// <param name="isHomeBadlyDamaged">If TRUE then the second level of damage to building is selected during the claim process, and we will not be invited to provide stormDamagedItemTypes.</param>
        /// <param name="isHomeUnsafe">If TRUE then the third level of damage to building is selected during the claim process, and we will not be invited to provide stormDamagedItemTypes.</param>
        /// <returns></returns>
        public HomeClaimBuilder WithBuildingDamage(List<StormDamagedItemTypes> stormDamagedItemTypes,
                                                    bool isSpecificItemsDamaged = true,
                                                    bool isBuildingOnlyOtherItems = false,
                                                    bool isHomeBadlyDamaged = true,
                                                    bool isHomeUnsafe = true)
        {
            var damagedBuilding = GetOrDefault(x => x.BuildingDamage, new BuildingDamageDetails());

            damagedBuilding.IsSpecificItemsDamaged = isSpecificItemsDamaged;
            damagedBuilding.IsHomeBadlyDamaged = isHomeBadlyDamaged;
            damagedBuilding.IsHomeUnsafe = isHomeUnsafe;

            if (stormDamagedItemTypes == null)
            { Reporting.Error("No details of specific storm damage to building were provided."); }

            if (damagedBuilding.StormDamagedBuildingSpecifics == null)
            { damagedBuilding.StormDamagedBuildingSpecifics = new List<StormDamagedItemTypes>(); }

            foreach (var item in stormDamagedItemTypes)
            { damagedBuilding.StormDamagedBuildingSpecifics.Add(item); }

            if (isBuildingOnlyOtherItems)
            {
                damagedBuilding.StormDamagedBuildingSpecifics.Add(StormDamagedItemTypes.OtherItems);
            }

            Set(x => x.BuildingDamage, damagedBuilding);

            return this;
        }

        public HomeClaimBuilder AddStormDamagedBuildingSpecifics(StormDamagedItemTypes stormDamagedBuildingSpecifics)
        {
            var buildingDamage = GetOrDefault(x => x.BuildingDamage, new BuildingDamageDetails());
            if (buildingDamage.StormDamagedBuildingSpecifics == null)
            { buildingDamage.StormDamagedBuildingSpecifics = new List<StormDamagedItemTypes>(); }

            buildingDamage.StormDamagedBuildingSpecifics.Add(new StormDamagedItemTypes());

            Set(x => x.BuildingDamage, buildingDamage);
            return this;
        }

        public HomeClaimBuilder WithContentsDamage(List<ContentItem> stormDamagedItems,
                                                    bool isCarpetWaterDamaged = true,
                                                    bool isCarpetTooWet = true,
                                                    bool isOtherStormDamagedContents = true)
        {
            var damagedContents = GetOrDefault(x => x.ContentsDamage, new ContentsDamageDetails());

            damagedContents.IsWaterDamagedCarpets       = isCarpetWaterDamaged;
            damagedContents.IsCarpetTooWet              = isCarpetTooWet;
            damagedContents.IsOtherStormDamagedContents = isOtherStormDamagedContents;

            if (stormDamagedItems == null)
            { Reporting.Error("Please give me some storm damaged contents."); }

            if (damagedContents.StormDamagedItems == null)
            { damagedContents.StormDamagedItems = new List<ContentItem>(); }

            foreach(var item in stormDamagedItems)
            { damagedContents.StormDamagedItems.Add(item); }

            Set(x => x.ContentsDamage, damagedContents);

            return this;
        }

        public HomeClaimBuilder AddStormDamagedItem(string description, int amount)
        {
            var damagedContents = GetOrDefault(x => x.ContentsDamage, new ContentsDamageDetails());
            if (damagedContents.StormDamagedItems == null)
            { damagedContents.StormDamagedItems = new List<ContentItem>(); }

            damagedContents.StormDamagedItems.Add(new ContentItem(description, amount));

            Set(x => x.ContentsDamage, damagedContents);
            return this;
        }

        public HomeClaimBuilder WithIsHomeInhabitable(bool isHomeInhabitable)
        {
            Set(x => x.IsHomeInhabitable, isHomeInhabitable);
            return this;
        }

        /// <summary>
        /// Generate random answers for the 'Safety checks' step, ensuring that the resulting claim will 
        /// have the appropriate Inhabitable/Uninhabitable status. It always starts with Insecure = true 
        /// to avoid having ALL values returned false and so nothing to input.
        /// Property is expected to be flagged Uninhabitable if either: 
        /// - there is no access to the Kitchen or Bathroom
        /// - there is no supply of both Power AND Water (if either supply exists, not flagged)
        /// 
        /// If 'None of these' is true then all other checkboxes would be cleared when it is selected.
        ///
        /// If shouldHomeBeInhabitable = FALSE then 'None of these' is overridden to be false to avoid clearing
        /// them.
        /// </summary>
        /// <param name="shouldHomeBeInhabitable">Value passed in as isHomeInhabitable flag, defaulted false in BuildTestDataSparkStormClaimNotFenceOnly</param>
        /// <param name="forceNoSafetyChecks">If both shouldHomeBeInhabitable and this flag are true we enforce 'None of these' as the only checkbox selected</param>
        /// <returns></returns>
        public HomeClaimBuilder WithStormSafetyCheckOptions(bool shouldHomeBeInhabitable, bool forceNoSafetyChecks)
        {
            var randomSafetyCheckList = GetOrDefault(x => x.StormSafetyCheckboxes, new StormSafetyCheckboxes()
            {
                Insecure            = true,
                DangerousLooseItems = DataHelper.RandomBoolean(),
                NoPower             = DataHelper.RandomBoolean(),
                NoWater             = DataHelper.RandomBoolean(),
                NoAccessKitchenBath = DataHelper.RandomBoolean(),
                NoneOfThese         = DataHelper.RandomBoolean()
            });
            
            if (!forceNoSafetyChecks)
            {
                if (shouldHomeBeInhabitable)
                {
                    randomSafetyCheckList.NoAccessKitchenBath  = false;
                    randomSafetyCheckList.NoWater              = false;
                }
                else
                {
                    bool uninhabitableNoUtilities = DataHelper.RandomBoolean();

                    if (uninhabitableNoUtilities)
                    {
                        randomSafetyCheckList.NoPower = true;
                        randomSafetyCheckList.NoWater = true;
                    }
                    else
                    {
                        randomSafetyCheckList.NoAccessKitchenBath = true;
                    }
                    randomSafetyCheckList.NoneOfThese = false;
                }
            }
            else
            {
                randomSafetyCheckList.NoneOfThese = true;
            }

            if (randomSafetyCheckList.NoneOfThese
             && shouldHomeBeInhabitable)
            {
                randomSafetyCheckList.Insecure              = false;
                randomSafetyCheckList.DangerousLooseItems   = false;
                randomSafetyCheckList.NoPower               = false;
                randomSafetyCheckList.NoWater               = false;
                randomSafetyCheckList.NoAccessKitchenBath   = false;
            }

            Set(x => x.StormSafetyCheckboxes, randomSafetyCheckList);
            return this;
        }

        public HomeClaimBuilder WithGlassDamage(GlassDamage isGlassDamaged)
        {
            Set(x => x.IsGlassDamaged, isGlassDamaged);
            return this;
        }

        public HomeClaimBuilder WithTimberLaminateFlooringDamage(bool isTimberOrLaminateFlooringDamaged)
        {
            Set(x => x.IsTimberOrLaminateFlooringDamaged, isTimberOrLaminateFlooringDamaged);
            return this;
        }

        public HomeClaimBuilder WithRandomTimberLaminateFlooringDamage()
        {
            Set(x => x.IsTimberOrLaminateFlooringDamaged, Randomiser.Get.Next(2) == 0);
            return this;
        }

        public HomeClaimBuilder WithGarageDoorDamage(GarageDoorDamage isGarageDoorDamaged)
        {
            Set(x => x.IsGarageDoorDamaged, isGarageDoorDamaged);
            return this;
        }

        public HomeClaimBuilder WithAffectedCover(AffectedCovers affectedCovers)
        {
            var policyInfo = Get(x => x.PolicyDetails);
            switch(affectedCovers)
            {
                case AffectedCovers.FenceOnly:
                case AffectedCovers.BuildingOnly:
                case AffectedCovers.BuildingAndFence:
                    if (!policyInfo.HasBuildingCover())
                    { Reporting.Error("Cannot set Building cover as damaged, as policy does not have appropriate cover."); }
                    break;
                case AffectedCovers.ContentsOnly:
                    if (!policyInfo.HasContentsCover())
                    { Reporting.Error("Cannot set Contents cover as damaged, as policy does not have appropriate cover."); }
                    break;
                case AffectedCovers.BuildingAndContents:
                case AffectedCovers.ContentsAndFence:
                case AffectedCovers.BuildingAndContentsAndFence:
                    if (!policyInfo.HasContentsCover() || !policyInfo.HasBuildingCover())
                    { Reporting.Error($"Cannot set Building and Contents covers as damaged, as policy does not have both covers."); }
                    break;

                // TODO AUNT-190 to include scenario for UnspecifiedPersonalValuablesOnly
                case AffectedCovers.SpecifiedPersonalValuablesOnly:
                case AffectedCovers.UnspecifiedPersonalValuablesOnly:
                    if (!policyInfo.HasOVSCover())
                    { Reporting.Error($"Cannot set Specified Personal Valuables covers as damaged, as policy does not have both covers."); }
                    break;
                default:
                    Reporting.Error($"Unknown affected cover type: {affectedCovers.GetDescription()}");
                    break;
            }
            Set(x => x.DamagedCovers, affectedCovers);
            return this;
        }

        /// <summary>
        /// Generate random values for the various Storm Water Damage checkboxes with three important caveats. 
        /// - The 'BadlySoakedCarpets' checkbox is only displayed in certain flows, and the value is determined by 
        /// isCarpetTooWet flag. If isCarpetTooWet is true, it takes precedence over 'NoWaterDamage' and 
        /// the avoidRestorerAllocation flag.
        /// 
        /// - If avoidRestorerAllocation = true, we force a 'false' value on the remaining checkboxes that would allocate a 
        ///   restorer if they were true. As above, isCarpetTooWet must ALREADY be false if you wish to use this, so it
        ///   does not set that value.
        /// 
        /// - If the 'NoWaterDamage' checkbox = true, ALL other checkboxes = false.
        /// </summary>
        /// <param name="isCarpetTooWet">Defines the answer to the 'Carpet is so badly soaked that you can't dry it' checkbox (if displayed)</param>
        /// <param name="avoidRestorerAllocation">If a test passes this flag as 'true' we force 'false' values for the checkboxes which can trigger a restorer to be allocated and would preclude the member providing a detailed list of damaged contents. INCOMPATIBLE WITH isCarpetTooWet = true</param>
        /// <returns></returns>
        public HomeClaimBuilder WithRandomStormWaterDamage(bool isCarpetTooWet, bool avoidRestorerAllocation)
        {
            var waterDamagedList = GetOrDefault(x => x.StormWaterDamageCheckboxes, new StormWaterDamageCheckboxes()
            {
                NoWaterDamage                = DataHelper.RandomBoolean(),
                DampPatchesOrDripping        = true,
                SolidTimberFloorIsWet        = DataHelper.RandomBoolean(),
                BadlySoakedCarpets           = isCarpetTooWet,
                HouseIsFlooded               = DataHelper.RandomBoolean(),
                SewageOrDrainWaterInTheHouse = DataHelper.RandomBoolean(),
                WaterInTheElectrics          = DataHelper.RandomBoolean(),
                OtherWaterDamage             = DataHelper.RandomBoolean()
            });

            if (!isCarpetTooWet 
             && avoidRestorerAllocation)
            {
                waterDamagedList.HouseIsFlooded                 = false;
                waterDamagedList.SewageOrDrainWaterInTheHouse   = false;
            }

            if (!isCarpetTooWet 
             && waterDamagedList.NoWaterDamage)
            {
                waterDamagedList.DampPatchesOrDripping          = false;
                waterDamagedList.SolidTimberFloorIsWet          = false;
                waterDamagedList.BadlySoakedCarpets             = false;
                waterDamagedList.HouseIsFlooded                 = false;
                waterDamagedList.SewageOrDrainWaterInTheHouse   = false;
                waterDamagedList.WaterInTheElectrics            = false;
                waterDamagedList.OtherWaterDamage               = false;
            }
            else
            {
                waterDamagedList.NoWaterDamage = false;
            }

            Set(x => x.StormWaterDamageCheckboxes, waterDamagedList);

            return this;
        }

        public HomeClaimBuilder AddWitness(Contact contact)
        {
            var witnesses = GetOrDefault(x => x.Witness, new List<Contact>()).ToList();

            witnesses.Add(contact);

            Set(x => x.Witness, witnesses);
            return this;
        }

        public HomeClaimBuilder WithNoWitnesses()
        {
            Set(x => x.Witness, null);
            return this;
        }

        public HomeClaimBuilder AddRandomWitness(int witnessCount = 1)
        {
            var witnesses = GetOrDefault(x => x.Witness, new List<Contact>()).ToList();

            for (int i = 0; i < witnessCount; i++)
            {
                var newWitness = new ContactBuilder().InitialiseRandomIndividual().Build();
                // B2C does not recognise the Dr title for witnesses.
                newWitness.Title = DataHelper.GetRandomTitleForGender(newWitness.Gender, excludeDrTitle: true);

                witnesses.Add(newWitness);
            }

            Set(x => x.Witness, witnesses);
            return this;
        }

        public HomeClaimBuilder WithNoThirdParty()
        {
            Set(x => x.ThirdParty, null);
            return this;
        }

        public HomeClaimBuilder AddRandomThirdParty(int thirdPartyCount = 1)
        {
            var thirdParty = GetOrDefault(x => x.ThirdParty, new List<Contact>()).ToList();

            for (int i = 0; i < thirdPartyCount; i++)
            {
                var newThirdParty = new ContactBuilder().InitialiseRandomIndividual().Build();
                // B2C does not recognise the Dr title for third parties and offenders.
                newThirdParty.Title = DataHelper.GetRandomTitleForGender(newThirdParty.Gender, excludeDrTitle: true);

                thirdParty.Add(newThirdParty);
            }

            Set(x => x.ThirdParty, thirdParty);
            return this;
        }

        public HomeClaimBuilder WithAccountDetailsForOnlineSettlement(BankAccount account)
        {
            Set(x => x.AccountForSettlement, account);
            return this;
        }

        public HomeClaimBuilder WithExpectedOutcomeForTest(ExpectedClaimOutcome expectedOutcome)
        {
            Set(x => x.ExpectedOutcome, expectedOutcome);
            return this;
        }

        public HomeClaimBuilder WithEligibilityForOnlineSettlement(SettleFenceOnline eligibilityForOnlineSettlement)
        {
            Set(x => x.EligibilityForOnlineSettlement, eligibilityForOnlineSettlement);
            return this;
        }

        public HomeClaimBuilder InitialiseHomeClaimWithBasicData(string policyNumber, PolicyContactDB claimant,HomeClaimDamageType damageType)
        {
            var HomeClaimBuilder = WithPolicyDetails(policyNumber, claimant)
                                   .WithEventDateAndTime(DateTime.Now.Date)
                                   .WithNewContactDetailsForClaimant(claimant, false, false)
                                   .WithDamageType(damageType)
                                   .WithRandomAccountOfEvent()
                                   .WithNoPoliceReport()
                                   .WithNoWitnesses()
                                   .WithNoThirdParty()
                                   .WithAccountDetailsForOnlineSettlement(null)
                                   .WithGlassDamage(isGlassDamaged: GlassDamage.NoDamage)
                                   .WithGarageDoorDamage(isGarageDoorDamaged: GarageDoorDamage.NoDamage)
                                   .WithTimberLaminateFlooringDamage(false)
                                   .WithExpectedOutcomeForTest(ExpectedClaimOutcome.ClaimLodged);
            return HomeClaimBuilder;
        }

        protected override ClaimHome BuildEntity()
        {
            return new ClaimHome
            {
                PolicyDetails  = GetOrDefault(x => x.PolicyDetails),
                Claimant       = GetOrDefault(x => x.Claimant),
                EventDateTime  = GetOrDefault(x => x.EventDateTime),
                BuildingDamage = GetOrDefault(x => x.BuildingDamage),
                ContentsDamage = GetOrDefault(x => x.ContentsDamage),
                DamageType     = GetOrDefault(x => x.DamageType),
                DamagedCovers  = GetOrDefault(x => x.DamagedCovers),
                FenceDamage    = GetOrDefault(x => x.FenceDamage),
                TheftDamage    = GetOrDefault(x => x.TheftDamage),
                AccountOfEvent = GetOrDefault(x => x.AccountOfEvent),
                ThirdParty     = GetOrDefault(x => x.ThirdParty),
                Witness        = GetOrDefault(x => x.Witness),
                PoliceReportNumber   = GetOrDefault(x => x.PoliceReportNumber),
                PoliceReportDate     = GetOrDefault(x => x.PoliceReportDate),
                AccountForSettlement = GetOrDefault(x => x.AccountForSettlement),
                ExpectedOutcome      = GetOrDefault(x => x.ExpectedOutcome),
                EligibilityForOnlineSettlement = GetOrDefault(x => x.EligibilityForOnlineSettlement),
                IsHomeInhabitable    = GetOrDefault(x => x.IsHomeInhabitable),
                IsGlassDamaged       = GetOrDefault(x => x.IsGlassDamaged),
                IsGarageDoorDamaged  = GetOrDefault(x => x.IsGarageDoorDamaged),
                LoginWith            = GetOrDefault(x => x.LoginWith),
                LinkedHomePolicies   = GetOrDefault(x => x.LinkedHomePolicies),
                IsTimberOrLaminateFlooringDamaged = GetOrDefault(x => x.IsTimberOrLaminateFlooringDamaged),
                FenceSettlementBreakdown          = GetOrDefault(x => x.FenceSettlementBreakdown),
                StormWaterDamageCheckboxes        = GetOrDefault(x => x.StormWaterDamageCheckboxes),
                StormSafetyCheckboxes             = GetOrDefault(x => x.StormSafetyCheckboxes),
            };
        }

        private string GetRandomMultiLineText()
        {
            var description = new System.Text.StringBuilder();
            int max = Randomiser.Get.Next(1, 8);

            for (int i = 0; i < max; i++)
            {
                description.Append($"{DataHelper.RandomAlphanumerics(10, 80)}.\r\n");
            }
            return description.ToString();
        }
    }
}
