using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class PolicyHome
        {
            public const int HOME_BUILDING_SI_MAX = 2000000;
            public const int HOME_BUILDING_SI_MIN = 200000;
            public const int HOME_CONTENTS_SI_MAX = 300000;
            public const int HOME_CONTENTS_SI_MAX_NO_SECURITY = 200000;
            public const int HOME_CONTENTS_SI_MIN = 30000;

            public const int HOME_RENTAL_MINIMUM = 50;
            public const int HOME_RENTAL_MAXIMUM = 5000;
            public static readonly string HOME_ACCIDENTAL_DAMAGE_EXCESS  = "500"; 
            public const int HOME_ACCIDENTAL_DAMAGE_SI_AMOUNT               = 0; // Fixed AD SI amount, as this is not customisable
            public const string HOME_ACCIDENTAL_DAMAGE_SI_CONTENTS_COVERED  = "Contents covered"; // Displays "Contents covered" in Sum insured if AD is added

            /// <summary>
            /// Home Product Version that Shield introduced support for
            /// Cyclone ReInsurance. This version was introduced on 26th
            /// September 2023 to PROD, and can be removed from automation
            /// after October 2024 (as all home policies will be on this
            /// version (or newer after Oct 2024).
            /// </summary>
            public const int HomeProductVersionCycloneReinsurance = 68000008;

            public enum HomeCover
            {
                [Description("Landlord's building")]
                LandlordsBuilding,
                [Description("Building only")]
                BuildingOnly,
                [Description("Contents only")]
                ContentsOnly,
                [Description("Renters")]
                RentersContents,
                [Description("Building & contents")]
                BuildingAndContents,
                [Description("Landlord's building & contents")]
                LandlordsBuildingAndContents
            };

            public enum HomeCoverCodes
            {
                HB,
                HCN,
                LB,
                LCN,
                RCN,
                OVS,
                OVU,
                AD
            };

            public enum HomeOccupancy
            {
                [Description("Holiday home")]
                HolidayHome,
                [Description("Investment property")]
                InvestmentProperty,
                [Description("Owner occupied")]
                OwnerOccupied,
                [Description("Tenant")]
                Tenant
            };

            public enum HomeType
            {
                Undefined, // Where test case is not setting a value
                StrataPropertyUpToFour,
                StrataPropertyFiveOrMore,
                GrannyFlat,
                GarageOrShed,
                HouseHeritage,
                // Home Types above are excluded from new business
                // Only add ones below that are supported for New Business and Change My Home.
                House,  // This must always be first item of valid B2C values.
                Townhouse,
                Duplex,
                Unit,
                Flat,
                Villa,
                Parkhome,
                Triplex,
                Quadruplex
            };

            public readonly static IReadOnlyDictionary<HomeType, IdDescriptions> HomeTypeDropdownText = new Dictionary<HomeType, IdDescriptions>()
            {
                { HomeType.House,                       new IdDescriptions() { TextB2C = "House", TextShield = "House" } },
                { HomeType.HouseHeritage,               new IdDescriptions() { TextB2C = "House - heritage listed", TextShield = "Heritage" } },
                { HomeType.Townhouse,                   new IdDescriptions() { TextB2C = "Townhouse", TextShield = "Town House" } },
                { HomeType.Duplex,                      new IdDescriptions() { TextB2C = "Duplex", TextShield = "Duplex" } },
                { HomeType.Unit,                        new IdDescriptions() { TextB2C = "Unit", TextShield = "Unit" } },
                { HomeType.Flat,                        new IdDescriptions() { TextB2C = "Flat", TextShield = "Flat" } },
                { HomeType.Villa,                       new IdDescriptions() { TextB2C = "Villa", TextShield = "Villa" } },
                { HomeType.Parkhome,                    new IdDescriptions() { TextB2C = "Park home", TextShield = "Park home" } },
                { HomeType.Triplex,                     new IdDescriptions() { TextB2C = "Triplex", TextShield = "Triplex" } },
                { HomeType.Quadruplex,                  new IdDescriptions() { TextB2C = "Quadruplex", TextShield = "Quadruplex" } },
                { HomeType.StrataPropertyUpToFour,      new IdDescriptions() { TextB2C = "Strata Dwelling (2 to4)", TextShield = "Strata Dwelling (2 to4)" } },
                { HomeType.StrataPropertyFiveOrMore,    new IdDescriptions() { TextB2C = "Strata Dwellings (5 or more)", TextShield = "Strata Dwellings (5 or more)" } },
                { HomeType.GrannyFlat,                  new IdDescriptions() { TextB2C = "Granny Flat", TextShield = "Granny Flat" } },
                { HomeType.GarageOrShed,                new IdDescriptions() { TextB2C = "Garage Or Shed", TextShield = "Garage or Shed" } }
            };

            public enum HomeMaterial
            {
                Undefined, // Where test case is not setting a value
                [Description("Not applicable for Valuables Only")]
                NotApplicable,
                [Description("Asbestos")]
                Asbestos,
                [Description("Brick")]
                Brick,
                [Description("Cement")]
                Cement,
                [Description("Earth/Mud")]
                EarthMud,
                [Description("Fibro")]
                Fibro,
                [Description("Hardiplank")]
                Hardiplank,
                [Description("Insulated Concrete Forms")]
                InsulatedConcreteForms,
                [Description("Metal")]
                Metal,
                [Description("Other")]
                Other,
                [Description("Stone")]
                Stone,
                [Description("Timber")]
                Timber                
            };

            public enum Alarm
            {
                Undefined, // Where test case is not setting a value
                [Description("Monitored alarm")]
                MonitoredAlarm,
                [Description("No alarm")]
                NoAlarm,
                [Description("Non monitored alarm")]
                NonMonitoredAlarm,
                [Description("RAC monitored alarm")]
                RACMonitoredAlarm
            };

            public enum GarageDoorsUpgradeStatus
            {
                [Description("No Information")]
                NoInformation = 0,
                [Description("Yes, they've been replaced to cyclone standards")]
                ReplacedToCyclone,
                [Description("Yes, they've had a bracing upgrade")]
                BracingUpgrade,
                [Description("No upgrades to my garage doors since 2012")]
                NoUpgrade,
                [Description("I don't have garage doors")]
                NoGarageDoor,
                [Description("I'm not sure")]
                NotSure
            };

            public enum RoofImprovementStatus
            {
                [Description("No Information")]
                NoInformation = 0,
                [Description("Yes, a complete roof replacement and tie-down upgrades")]
                CompleteRoofReplacement,
                [Description("Yes, a roof structure tie-down upgrade")]
                TiedownUpgrade,
                [Description("No significant roof improvements")]
                NoImprovement,
                [Description("I'm not sure")]
                NotSure
            };

            public enum HomePreviousInsuranceTime
            {
                [Description("0")]
                Zero,
                [Description("Less than 1")]
                LessThan1,
                [Description("1")]
                One,
                [Description("2")]
                Two,
                [Description("3")]
                Three,
                [Description("4")]
                Four,
                [Description("5+")]
                FivePlus
            };

            public enum HomeRoof
            {
                Undefined, // Where test case is not setting a value
                [Description("Metal")]
                Metal,
                [Description("Other")]
                Other,
                [Description("Tile")]
                Tile
            };

            public enum UnspecifiedPersonalValuables
            {
                [Description("None")]
                None = 0,
                [Description("Max $2,000 / $500 per item")]
                Max2000 = 2000,
                [Description("Max $3,000 / $600 per item")]
                Max3000 = 3000,
                [Description("Max $4,000 / $800 per item")]
                Max4000 = 4000,
                [Description("Max $5,000 / $1,000 per item")]
                Max5000 = 5000
            };

            public enum SpecifiedValuables
            {
                Bicycles,
                DenturesHearingAids,
                DivingEquipment,
                JewelleryWatches,
                Laptop,
                MobilePhone,
                MusicalInstrument,
                PhotographicVideoEquipment,
                PortableDVDPlayers,
                PortableEPIRBAndGPS,
                PrescriptionGlasses,
                SportingEquipment,
                Wheelchair,
                Contents = 1000  // Here to support parsing categories from Shield DB.
            };

            public readonly static IReadOnlyDictionary<SpecifiedValuables, IdDescriptions> SpecifiedPersonalValuablesDisplayedText = new Dictionary<SpecifiedValuables, IdDescriptions>()
            {
                { SpecifiedValuables.Bicycles,
                                new IdDescriptions() { TextB2C = "Bicycles", TextShield = "Bicycles" }
                },
                { SpecifiedValuables.DenturesHearingAids,
                                new IdDescriptions() { TextB2C = "Dentures & Hearing Aids", TextShield = "Dentures & Hearing Aids" }
                },
                { SpecifiedValuables.DivingEquipment,
                                new IdDescriptions() { TextB2C = "Diving Equipment", TextShield = "Diving Equipment" }
                },
                { SpecifiedValuables.JewelleryWatches,
                                new IdDescriptions() { TextB2C = "Jewellery & watches", TextShield = "Jewellery/Watches" }
                },
                { SpecifiedValuables.Laptop,
                                new IdDescriptions() { TextB2C = "Laptop & notebook computer", TextShield = "Lap-Top /Notebook Computer" }
                },
                { SpecifiedValuables.MobilePhone,
                                new IdDescriptions() { TextB2C = "Mobile Phone", TextShield = "Mobile Phone" }
                },
                { SpecifiedValuables.MusicalInstrument,
                                new IdDescriptions() { TextB2C = "Musical Instrument", TextShield = "Musical Instrument" }
                },
                { SpecifiedValuables.PhotographicVideoEquipment,
                                new IdDescriptions() { TextB2C = "Photographic & video equipment", TextShield = "Photographic and Video Equipment" }
                },
                { SpecifiedValuables.PortableDVDPlayers,
                                new IdDescriptions() { TextB2C = "Portable DVD players", TextShield = "Portable Radio/TV/Recorders/DVD Players" }
                },
                { SpecifiedValuables.PortableEPIRBAndGPS,
                                new IdDescriptions() { TextB2C = "Portable EPIRB & GPS", TextShield = "Portable EPIRB and GPS Equipment" }
                },
                { SpecifiedValuables.PrescriptionGlasses,
                                new IdDescriptions() { TextB2C = "Prescription glasses & sunglasses", TextShield = "Prescription Glasses and Sunglasses" }
                },
                { SpecifiedValuables.SportingEquipment,
                                new IdDescriptions() { TextB2C = "Sporting equipment", TextShield = "Sporting Equipment including Golf & Fishing Equipment" }
                },
                { SpecifiedValuables.Wheelchair,
                                new IdDescriptions() { TextB2C = "Wheelchair", TextShield = "Wheelchair" }
                },
                { SpecifiedValuables.Contents,   // Supports Shield DB categorisation of specified contents
                                new IdDescriptions() { TextShield = "Contents" }
                }
            };

            public enum SpecifiedContents
            {
                JewelleryWatches,
                ArtAntiquesFurs,
                AudioVisualCollections
            };

            public readonly static IReadOnlyDictionary<SpecifiedContents, IdDescriptions> SpecifiedContentsDisplayedText = new Dictionary<SpecifiedContents, IdDescriptions>()
            {
                { SpecifiedContents.JewelleryWatches,
                                new IdDescriptions() { TextB2C = "Jewellery & watches", TextShield = "Contents" }
                },
                { SpecifiedContents.ArtAntiquesFurs,
                                new IdDescriptions() { TextB2C = "Works of art, antiques & furs", TextShield = "Contents" }
                },
                { SpecifiedContents.AudioVisualCollections,
                                new IdDescriptions() { TextB2C = "Audio visual collections", TextShield = "Contents" }
                }
            };

            public enum HomePropertyManager
            {
                Undefined, // Where test case is not setting a value
                [Description("Agent")]
                Agent,
                [Description("Yourself")]
                Owner
            };

            public enum ClaimsHistory
            {
                [Description("Burglary or theft")]
                BurglaryOrTheft,
                [Description("Escape of liquid")]
                EscapeOfLiquid,
                [Description("Fire")]
                Fire,
                [Description("Flood")]
                Flood,
                [Description("Loss of rent")]
                LossOfRent,
                [Description("Malicious damage")]
                MaliciousDamage,
                [Description("Other")]
                Other,
                [Description("Storm")]
                Storm
            }
        }
    }
}
