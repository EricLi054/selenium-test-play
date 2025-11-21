using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class ClaimsHome
        {
            public const string SHIELD_CLAIM_COVER_BUILDING = "Building";
            public const string SHIELD_CLAIM_COVER_CONTENTS = "Contents";
            public const string SHIELD_CLAIM_COVER_FENCE = "Fence Only";
            public static class ClaimCovers
            {
                public static readonly string ShieldSpecifiedPersonalValuables = "Specified Personal Valuables";
                public static readonly string ShieldUnpecifiedPersonalValuables = "Unspecified Personal Valuables";

            }

            public class FenceLengths
            {
                /// <summary>
                /// Set to 30 in recent B2C-4412. Lengths over this amount
                /// will not qualify for online settlement.
                /// </summary>
                public const int MaxForOnlineSettlement = 30;
            }

            public enum ExpectedClaimOutcome
            {
                /// <summary>
                /// Claim is lodged successfully with claim number provided (All Claims)
                /// </summary>
                ClaimLodged,
                /// <summary>
                /// Attempt to allocate one or more Service Providers for claim is unsuccessful.
                /// NOTE: This may be because there was no NEED for a service provider to be allocated,
                /// but Shield doesn't provide that level of detail to us, any claim which doesn't have
                /// a service provider allocated is effectively communicated as though it should have but
                /// failed.
                /// </summary>
                FailureToAllocateServiceProvider,
                /// <summary>
                /// Claim is not eligible for online settlment
                /// </summary>
                NotEligibleForOnlineSettlement,
                /// <summary>
                /// Accept online fence settlement and input bank details.
                /// </summary>
                OnlineSettlementAcceptWithBankDetails,
                /// <summary>
                /// Accept online fence settlement and use existing bank details on record.
                /// </summary>
                OnlineSettlementAcceptWithExistingBankDetails,
                /// <summary>
                /// Accept online fence settlment but don't provide bank details.
                /// </summary>
                OnlineSettlementAcceptWithOutBankDetails,
                /// <summary>
                /// Applicable only for Spark Dividing fence claims; user is presented with online settlement details but 
                /// selects the "I'll get a repair quote" button instead.
                /// </summary>
                GetRepairQuoteFirst,
                /// <summary>
                /// Applicable only for Spark, User select repairs already been done
                /// </summary>
                RepairsCompleted,
                /// <summary>
                /// Applicable only for Spark, User select already have a repair quote
                /// </summary>
                AlreadyHaveRepairQuote,
                /// <summary>
                /// Eligible for online settlement but selects "Choose RAC Repairer"
                /// </summary>
                OnlineSettlementRepairByRAC,
                /// <summary>
                /// Eligible for online settlement but selects "Choose a call from RAC"
                /// </summary>
                OnlineSettlementContactMe,
                /// <summary>
                /// Eligible for online settlement but selects "Take time to decide"
                /// </summary>
                OnlineSettlementTakeMoreTimeToDecide,
                /// <summary>
                /// Member did not provide consent for CSFS 
                /// </summary>
                OnlineSettlementNoConsent
            }

            public enum SettleFenceOnline
            {
                Eligible,
                IneligibleClaimNotFenceOnly,
                IneligibleClaimWithinTwelveMonths,
                IneligibleCannotMeasureFence,
                IneligibleExcessMoreThanRepairs,
                IneligibleFenceTypeBrick,
                /// <summary>
                /// Overrides IneligibleOverThirtyMetres.
                /// </summary>
                IneligibleFenceTypeMoreThanOne,
                /// <summary>
                /// Overrides IneligibleOverThirtyMetres.
                /// </summary>
                IneligibleFenceTypeOther,
                /// <summary>
                /// Overrides IneligibleOverThirtyMetres.
                /// </summary>
                IneligibleFenceTypeUnsure,
                IneligibleFenceTypeWood,
                /// <summary>
                /// IneligibleOverThirtyMetres is irrelevant if FenceTypeMoreThanOne, FenceTypeOther or FenceTypeUnsure applies, use that instead.
                /// </summary>
                IneligibleOverThirtyMetres,
                IneligiblePaymentBlock,
                IneligibleRepairsAlreadyQuoted,
                IneligibleRepairsAlreadyCompleted
            }

            public enum HomeClaimDamageType
            {
                StormDamageToFenceOnly,
                StormAndTempest,
                EscapeOfLiquid,
                Theft,
                MaliciousDamage,
                Glass,
                ElectricMotorBurnout,
                ImpactOfVehicle,
                Fire,
                SpoiltFood,
                Flood,
                Earthquake,
                AccidentalDamage
            };

            public readonly static IReadOnlyDictionary<HomeClaimDamageType, IdDescriptions> HomeClaimDamageTypeNames = new Dictionary<HomeClaimDamageType, IdDescriptions>()
            {
                { HomeClaimDamageType.StormDamageToFenceOnly,
                    new IdDescriptions() {
                        TextB2C    = "Storm Damage to Fence Only",
                        TextShield = "Storm & Tempest"
                    }
                },
                { HomeClaimDamageType.StormAndTempest,
                    new IdDescriptions() {
                        TextB2C    = "Storm and Tempest",
                        TextShield = "Storm & Tempest"
                    }
                },
                { HomeClaimDamageType.EscapeOfLiquid,
                    new IdDescriptions() {
                        TextB2C    = "Escape of Liquid",
                        TextShield = "Escape of Liquid"
                    }
                },
                { HomeClaimDamageType.Theft,
                    new IdDescriptions() {
                        TextB2C    = "Theft/House Break-In",
                        TextShield = "Theft/House Break-In"
                    }
                },
                { HomeClaimDamageType.MaliciousDamage,
                    new IdDescriptions() {
                        TextB2C    = "Malicious Damage",
                        TextShield = "Malicious Damage"
                    }
                },
                { HomeClaimDamageType.Glass,
                    new IdDescriptions() {
                        TextB2C    = "Glass",
                        TextShield = "Glass"
                    }
                },
                { HomeClaimDamageType.ElectricMotorBurnout,
                    new IdDescriptions() {
                        TextB2C    = "Electric Motor Burnout",
                        TextShield = "Electric Motor Burnout"
                    }
                },
                { HomeClaimDamageType.ImpactOfVehicle,
                    new IdDescriptions() {
                        TextB2C    = "Impact of Vehicle",
                        TextShield = "Impact of Vehicle"
                    }
                },
                { HomeClaimDamageType.Fire,
                    new IdDescriptions() {
                        TextB2C    = "Fire/Explosion",
                        TextShield = "Fire/Explosion"
                    }
                },
                { HomeClaimDamageType.SpoiltFood,
                    new IdDescriptions() {
                        TextB2C    = "Spoilt Food",
                        TextShield = "Spoilt Food"
                    }
                },
                { HomeClaimDamageType.Flood,
                    new IdDescriptions() {
                        TextB2C    = "Flood",
                        TextShield = "Flood"
                    }
                },
                { HomeClaimDamageType.Earthquake,
                    new IdDescriptions() {
                        TextB2C    = "Earthquake",
                        TextShield = "Earthquake"
                    }
                },
                { HomeClaimDamageType.AccidentalDamage,
                    new IdDescriptions() {
                        TextB2C    = "Accidental Damage",
                        TextShield = "Accidental"
                    }
                }
            };

            public readonly static IReadOnlyDictionary<HomeClaimDamageType, string> HomeClaimTypeAndScenarioName = new Dictionary<HomeClaimDamageType, string>()
            {
                { HomeClaimDamageType.StormDamageToFenceOnly, "Storm" },
                { HomeClaimDamageType.StormAndTempest,        "Storm" },
                { HomeClaimDamageType.EscapeOfLiquid,         "Escape of Liquid" },
                { HomeClaimDamageType.Theft,                  "Theft or Malicious Damage" },
                { HomeClaimDamageType.MaliciousDamage,        "Malicious Damage" },
                { HomeClaimDamageType.Glass,                  "Glass" },
                { HomeClaimDamageType.ElectricMotorBurnout,   "Electric Motor Burnout" },
                { HomeClaimDamageType.ImpactOfVehicle,        "Impact" },
                { HomeClaimDamageType.Fire,                   "Fire" },
                { HomeClaimDamageType.SpoiltFood,             "Other" },
                { HomeClaimDamageType.Flood,                  "Flood" },
                { HomeClaimDamageType.Earthquake,             "Earthquake" },
                { HomeClaimDamageType.AccidentalDamage,       "Accidental Damage or Loss" }
            };

            public enum FenceType
            {
                Hardifence,
                SuperSix,
                Colorbond,
                Wooden,
                BrickWall,
                Asbestos,
                Other,
                MoreThanOneFence,
                Glass,
                NotSure
            };

            public readonly static IReadOnlyDictionary<FenceType, FenceDescriptions> FenceTypeNames = new Dictionary<FenceType, FenceDescriptions>()
            {
                { FenceType.Colorbond,
                    new FenceDescriptions() {
                        TextB2C    = "Colourbond",
                        TextSpark = "Colorbond",
                        ShieldAnswerId = "1000114"
                    }
                },
                { FenceType.SuperSix,
                    new FenceDescriptions() {
                        TextB2C    = "Super Six",
                        TextSpark = "Super Six",
                        ShieldAnswerId = "1000112"
                    }
                },
                { FenceType.Hardifence,
                    new FenceDescriptions() {
                        TextB2C    = "Hardifence",
                        TextSpark = "Hardifence",
                        ShieldAnswerId = "1000111"
                    }
                },
                { FenceType.Asbestos,
                    new FenceDescriptions() {
                        TextB2C    = "Asbestos",
                        TextSpark = "Asbestos",
                        ShieldAnswerId = "1000116"
                    }
                },
                { FenceType.BrickWall,
                    new FenceDescriptions() {
                        TextB2C    = "Brick wall",
                        TextSpark = "Brick",
                        ShieldAnswerId = "1000115"
                    }
                },
                { FenceType.Wooden,
                    new FenceDescriptions() {
                        TextB2C    = "Wooden including picket",
                        TextSpark = "Wooden",
                        ShieldAnswerId = "1000113"
                    }
                },                 
                { FenceType.Other,
                    new FenceDescriptions() {
                        TextB2C    = "Other",
                        TextSpark = "Other",
                        ShieldAnswerId = "1000117"
                    }
                },
                { FenceType.MoreThanOneFence,
                    new FenceDescriptions() {
                        TextSpark = "More than one type of fence",
                        ShieldAnswerId = "1000117"
                    }
                },
                {FenceType.Glass,
                    new FenceDescriptions()
                    {
                        TextSpark = "Glass",
                        ShieldAnswerId = "68000000"
                    }
                },
                { FenceType.NotSure,
                    new FenceDescriptions() {
                        TextSpark = "I'm not sure",
                        ShieldAnswerId = "1000117"
                    }
                }
            };

            public enum GlassDamage
            {
                NoDamage = 3000006,
                Leadlight = 3000007,
                ShowerScreen = 3000008,
                Other = 3000009
            }

            public enum GarageDoorDamage
            {
                NoDamage = 0,
                Damage = 1
            }

            public enum StolenItemsLocation
            {
                Inside,
                Outside,
                Both
            }

            /// <summary>
            /// Codes to be used in home claims test data to indicate the desired covers
            /// to be flagged as affected in the test scenario.
            /// </summary>
            public enum AffectedCovers
            {
                [Description("Building")]
                BuildingOnly,
                [Description("Contents")]
                ContentsOnly,
                [Description("Fence Only")]
                FenceOnly,
                [Description("Building and Contents")]
                BuildingAndContents,
                [Description("Building and Fence")]
                BuildingAndFence,
                [Description("Contents and Fence")]
                ContentsAndFence,
                [Description("Building, Contents & Fence")]
                BuildingAndContentsAndFence,
                [Description("Specified Personal Valuables Only")]
                SpecifiedPersonalValuablesOnly,
                [Description("Unspecified Personal Valuables Only")]
                UnspecifiedPersonalValuablesOnly
            }

            public enum ShieldEvent
            {
                [Description("Online settlement offer displayed")]
                SettlementOfferDisplayed,
                [Description("Online Settlement Offer Accepted")]
                SettlementOfferAccepted,
                [Description("Online Settlement Offer Declined")]
                SettlementOfferDeclined,
                [Description("Complete Online Claim Lodgement")]
                CompleteOnlineLodgement,
                [Description("Cash Settlement Accepted")]
                CashSettlementAccepted,
                [Description("Member decision pending - Awaiting bank account details")]
                CashSettlementAwaitingBankAccount,
                [Description("Cash Settlement  Rejected - Repairs")]
                SettlementRepairs,
                [Description("Member decision pending - Contact member")]
                SettlementContactMember,
                [Description("Member decision pending - Need more time")]
                SettlementNeedMoreTime,
                /// <summary>
                /// Remarks: Non-Metro home assessor may be required. 
                /// Please review for manual home assessor allocation
                /// </summary>
                [Description("Non-Metro Home Assessor Referral")]
                NonMetroHomeAssessorReferral,
                /// <summary>
                /// Remarks: There are no Internal Home Assessors available for automatic allocation. 
                /// Please review for manual Internal Home Assessor allocation
                /// </summary>
                [Description("Home Assessor Referral")]
                HomeAssessorReferral
            };

            public enum HomeClaimQuestionnaire
            {
                [Description("Are we cash settling the member today")]
                AreWeCashSettlingToday = 4000063,
                [Description("Is the home uninhabitable after damage (non-storm)")]
                IsHomeUninhabitableNotStormDamage = 1000141,
                [Description("Is the home uninhabitable after damage (storm)")]
                IsHomeUninhabitableStormDamage = 2000024,
                [Description("Home Assessor is required")]
                IsHomeAssessorRequired = 4000021,
                [Description("Does the member need to provide EFT details or Payment authority form (PAF)")]
                MemberNeedsToProvideEFTOrPAF = 4000042,
                [Description("What type of fence is damaged")]
                FenceType = 3000015,
                [Description("How many metres have been damaged")]
                FenceMetresDamaged = 3000016,
                [Description("Number of metres painted on PH side")]
                FenceMetresPainted = 3000023,
                [Description("Is the fence dividing")]
                FenceIsDividing = 3000021,
                [Description("If you are outside the property facing the front door, which fence has been damaged")]
                FenceAffectedSides = 3000025,
                [Description("Is a temporary fence required")]
                FenceRequireTemporary = 3000026,
                [Description("Please specify why (temp fence required)")]
                FenceRequireTemporaryReason = 3000027,
                [Description("What type of glass is damaged, if any")]
                GlassDamage = 3000028,
                [Description("Is there any damage to garage door or motor?")]
                GarageDoorDamage = 68000023,
                [Description("Point of entry known")]
                TheftIsEntryPointKnown = 1000177,
                [Description("How did the offender enter your home")]
                TheftHowDidTheyEnter = 1000178
            }
            public enum StormDamagedItemTypes
            {
                [Description("Flooring")]
                Flooring,
                [Description("Solar panels")]
                SolarPanels,
                [Description("Garage door or motor")]
                GarageDoorOrMotor,
                [Description("TV aerial")]
                TvAerial,
                [Description("Clothesline")]
                ClothesLine,
                [Description("Security system")]
                SecuritySystem,
                [Description("Glass")]
                Glass,
                [Description("Leadlight")]
                LeadLight,
                [Description("Other items")]
                OtherItems
            }

            public enum StormWaterDamageCheckboxesOptions
            {
                [Description("No water damage")]
                NoWaterDamage,
                [Description("Damp patches or dripping")]
                DampPatchesOrDripping,
                [Description("Solid timber flooring is wet")]
                SolidTimberFloorIsWet,
                [Description("Carpet is so badly soaked that you can't dry it")]
                BadlySoakedCarpets,
                [Description("House is flooded")]
                HouseIsFlooded,
                [Description("There's sewage or drain water in the house")]
                SewageOrDrainWaterInTheHouse,
                [Description("Water in the electrics")]
                WaterInTheElectrics,
                [Description("Other water damage")]
                OtherWaterDamage
            }

            public enum StormSafetyCheckOptions
            {
                [Description("Can't secure my home")]
                Insecure,
                [Description("Dangerous or loose items that could cause injury")]
                DangerousLooseItems,
                [Description("No power to the property")]
                NoPower,
                [Description("No water supply to the property")]
                NoWater,
                [Description("No access to kitchen or bathroom")]
                NoAccessKitchenBath,
                [Description("None of these")]
                NoneOfThese
            }
        }
    }
}
