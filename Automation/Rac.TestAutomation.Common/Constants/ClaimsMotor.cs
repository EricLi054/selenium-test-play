using System.Collections.Generic;
using System.ComponentModel;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class ClaimsMotor
        {
            public enum MotorClaimDamageType
            {
                SingleVehicleCollision,
                MultipleVehicleCollision,
                Theft,
                WindscreenGlassDamage,
                MaliciousDamage,
                Fire,
                Storm,
                Earthquake,
                Flood,
                Other
            };

            public readonly static IReadOnlyDictionary<MotorClaimDamageType, IdDescriptions> MotorClaimDamageTypeNames = new Dictionary<MotorClaimDamageType, IdDescriptions>()
            {
                { MotorClaimDamageType.SingleVehicleCollision,
                    new IdDescriptions() {
                        TextB2C    = "Single Vehicle Collision",
                        TextShield = "Collision - Single"
                    }
                },
                { MotorClaimDamageType.MultipleVehicleCollision,
                    new IdDescriptions() {
                        TextB2C    = "Multiple Vehicle Collision",
                        TextShield = "Collision - Multi"
                    }
                },
                { MotorClaimDamageType.Theft,
                    new IdDescriptions() {
                        TextB2C    = "Theft",
                        TextShield = "Theft or Malicious Damage"
                    }
                },
                { MotorClaimDamageType.WindscreenGlassDamage,
                    new IdDescriptions() {
                        TextB2C    = "Windscreen/Glass Damage",
                        TextShield = "Glass"
                    }
                },
                { MotorClaimDamageType.MaliciousDamage,
                    new IdDescriptions() {
                        TextB2C    = "Malicious Damage",
                        TextShield = "Malicious Damage"
                    }
                },
                { MotorClaimDamageType.Fire,
                    new IdDescriptions() {
                        TextB2C    = "Fire",
                        TextShield = "Fire"
                    }
                },
                { MotorClaimDamageType.Storm,
                    new IdDescriptions() {
                        TextB2C    = "Storm",
                        TextShield = "Storm"
                    }
                },
                { MotorClaimDamageType.Earthquake,
                    new IdDescriptions() {
                        TextB2C    = "Earthquake",
                        TextShield = "Earthquake"
                    }
                },
                { MotorClaimDamageType.Flood,
                    new IdDescriptions() {
                        TextB2C    = "Flood",
                        TextShield = "Flood"
                    }
                },
                { MotorClaimDamageType.Other,
                    new IdDescriptions() {
                        TextB2C    = "Other",
                        TextShield = "Other"
                    }
                }
            };

            public enum MotorClaimTowedFrom
            {
                AccidentLocation = 1000014,
                HomeAddress = 1000015,
                Other = 1000016
            };

            public enum MotorClaimTowedTo
            {
                None,
                [Description("Holding yard")]
                HoldingYard,
                [Description("Home address")]
                HomeAddress,
                [Description("Repairer")]
                Repairer,
                [Description("Other")]
                Other,
                [Description("I don't know")]
                Unknown
            };

            public readonly static IReadOnlyDictionary<MotorClaimTowedTo, SparkShieldQuestionnaireDescriptions> SparkMotorTowedToText = new Dictionary<MotorClaimTowedTo, SparkShieldQuestionnaireDescriptions>()
            {
                { MotorClaimTowedTo.HoldingYard,
                    new SparkShieldQuestionnaireDescriptions() {
                        TextSpark = "A tow truck holding yard",
                        TextShield = "HoldingYard",
                        ShieldAnswerId = "1000018"
                    }
                },
                { MotorClaimTowedTo.HomeAddress,
                    new SparkShieldQuestionnaireDescriptions() {
                        TextSpark = "Your home",
                        TextShield = "HomeAddress",
                        ShieldAnswerId = "1000020"
                    }
                },
                { MotorClaimTowedTo.Repairer,
                    new SparkShieldQuestionnaireDescriptions() {
                        TextSpark = "A repairer",
                        TextShield = "Repairer",
                        ShieldAnswerId = "1000017"
                    }
                },
                { MotorClaimTowedTo.Other,
                    new SparkShieldQuestionnaireDescriptions() {
                        TextSpark = "Other",
                        TextShield = "Other",
                        ShieldAnswerId = "1000021"
                    }
                },
                { MotorClaimTowedTo.Unknown,
                    new SparkShieldQuestionnaireDescriptions() {
                        TextSpark = "I don't know",
                        TextShield = "Other",
                        ShieldAnswerId = "1000021"
                    }
                }
            };

            public enum MotorClaimTheftDetails
            {
                [Description("Keys")]
                Keys,
                [Description("Vehicle (Recovered)")]
                VehicleRecovered,
                [Description("Vehicle (Unrecovered)")]
                VehicleUnrecovered,
                [Description("Trailer")]
                Trailer,
                [Description("Vehicle Parts")]
                VehicleParts
            };

            public enum TravelDirection
            {
                [Description("Forward")]
                Forward = 1000077,
                [Description("Reversing")]
                Reversing = 1000078,
                [Description("Stationary")]
                Stationary = 1000075,
                [Description("Parked")]
                Parked = 1000076
            }

            public readonly static IReadOnlyDictionary<TravelDirection, IdDescriptions> SparkTravelDirectionText = new Dictionary<TravelDirection, IdDescriptions>()
            {
                { TravelDirection.Forward,
                    new IdDescriptions() {
                        TextSpark = "Driving forward"
                    }
                },
                { TravelDirection.Reversing,
                    new IdDescriptions() {
                        TextSpark = "Reversing"
                    }
                },
                { TravelDirection.Stationary,
                    new IdDescriptions() {
                        TextSpark = "Stationary"
                    }
                },
                { TravelDirection.Parked,
                    new IdDescriptions() {
                        TextSpark = "Parked"
                    }
                }
            };

            public enum MotorClaimQuestionnaire
            {
                [Description("Have repairs been organised or completed")]
                HaveRepairsBeenOrganised = 3000033,
                [Description("Is the damage to the front windscreen only")]
                GlassIsFrontWindscreenOnly = 1000040,
                [Description("What is the damage - chip or crack")]
                GlassDamageType = 1000041,
                [Description("Does the member need to provide their EFT details")]
                MemberNeedsToProvideEFT = 4000041,
                [Description("Repairs to be completed in WA")]
                RepairsToCompleteInWA = 3000046,
                [Description("Is PH vehicle driveable")]
                IsVehicleDriveable = 1000189,
                [Description("Was PH vehicle towed")]
                WasVehicleTowed = 1000190,
                [Description("From (where towed from)")]
                TowedFrom = 1000192,
                [Description("Destination type (Type of place towed to)")]
                TowedToPlaceType = 1000193,
                [Description("Destination details (Place towed to)")]
                TowedToDetails = 1001557,
                [Description("TP known to PH (hit while parked)")]
                ThirdPartyKnownToPHHitWhileParked = 1000100,
                [Description("TP known to PH (all other multi vehicle collisions)")]
                ThirdPartyKnownToPH = 1001612,
                [Description("Type of road the accident occurred on")]
                TypeOfRoad = 1000058,
                [Description("Anyone changing lane")]
                AnyoneChangingLanes = 1000059,
                [Description("Which Vehicle was in front at the point of merge")]
                WhoWasInFrontDuringMerge = 1000060,
                [Description("Controlled Intersection")]
                ControlledIntersection = 1000061,
                [Description("Traffic Signals type on PH side")]
                TrafficSignalsForPH = 1000062,
                [Description("Traffic Signals type on TP side")]
                TrafficSignalsForTP = 1000063,
                [Description("PH Direction of Travel")]
                DirectionOfTravel = 1000067,
                [Description("Were the vehicle keys stolen")]
                WereKeysStolen = 3000034,
                [Description("Where were the keys stolen from")]
                WhereWereKeysStolenFrom = 3000035,
                [Description("Is the vehicle subject to finance")]
                IsVehicleFinanced = 2000004,
                [Description("Have you tried to sell the vehicle")]
                WasVehicleForSale = 2000008,
                [Description("Has the vehicle been recovered")]
                IsVehicleRecovered = 2000009,
                [Description("Is the member getting their own repairer quote?")]
                IsMemberGettingOwnQuote = 68000025
            }
            public enum MotorCollisionNumberOfVehiclesInvolved
            {
                [Description("1 other vehicle")]
                OneOtherVehicle,
                [Description("2 other vehicles")]
                TwoOtherVehicles,
                [Description("3 or more other vehicles")]
                ThreeOrMoreOtherVehicles,
                [Description("None, no other vehicles involved")]
                NoOtherVehiclesInvolved,
                [Description("I'm not sure")]
                ImNotSure
            }   
           
            public enum MotorClaimScenario
            {
                NotApplicable,                
                // Single Vehicle
                SingleVehicleCollision,
                [Description("Your own property")]
                AccidentWithYourOwnProperty,
                [Description("Someone else's property")]
                AccidentWithSomeoneElseProperty,
                [Description("Kangaroo or wildlife")]
                AccidentWithWildlife,
                [Description("Another person's pet or animal")]
                AccidentWithSomeonesPet,
                [Description("Something else")]
                AccidentWithSomethingElse,
                // Multi vehicle
                WhileDrivingOtherVehicleHitRearOfMyCar,
                WhileDrivingOtherVehicleHitMyCarWhenChangingLanes,
                WhileDrivingOtherVehicleFailToGiveWayAndHitMyCar,
                WhileDrivingOtherVehicleHitMyCarSomethingElseHappened,
                WhileDrivingMyCarHitAnotherCarWhenChangingLanes,
                WhileDrivingMyCarHitRearOfAnotherCar,
                WhileDrivingMyCarHitAParkedCar,
                WhileDrivingMyCarHitAnotherCarFailToGiveWay,
                WhileDrivingMyCarHitAnotherCarSomethingElseHappened,
                WhileDrivingOurCarHitOneAnotherCarWhenChangingLanes,
                WhileDrivingOurCarHitOneAnotherCarBothFailedToGiveWay,
                WhileDrivingOurCarHitOneAnotherCarSomethingElseHappened,
                WhileDrivingSomethingElseHappened,
                WhileReversingHitParkedCar,
                WhileReversingHitAnotherCar,
                WhileReversingHitByAnotherCar,
                WhileReversingHitByAnotherReversingCar,
                WhileReversingSomethingElseHappened,
                WhileParkedAnotherCarHitMyCar,
                WhileParkedSomethingElseHappened,
                WhileStationaryAnotherCarHitRearOfMyCar,
                WhileStationaryAnotherCarReversedIntoMyCar,
                WhileStationarySomethingElseHappened,
                //Glass Damage
                GlassDamageNotFixed,
                GlassDamageRepairsBooked,
                GlassDamageAlreadyFixed
            };
            
            public enum ShieldClaimScenario
            {
                [Description("CollisionWithAnimal")]
                CollisionWithAnimal,
                [Description("ImpactedByObject")]
                VehicleImpactedbyObject,
                [Description("SingleCollision")]
                SingleCollision,
                [Description("MultipleCollision")]
                MultiCollision,
                [Description("HitWhilstParked")]
                HitWhilstParked,
                [Description("UninsuredMotoristExtension")]
                UninsuredMotoristExtension,
                [Description("LiabilityOnly")]
                LiabilityOnly,
                [Description("CollisionWithAnotherVehicle")]
                CollisionWithAnotherVehicle,
                [Description("CollisionWithProperty")]
                CollisionWithProperty,
                [Description("TPOtherRecovery")]
                TPOtherRecovery
            }

            public enum ShieldClaimType
            {
                [Description("CollisionSingle")]
                CollisionSingle,
                [Description("CollisionMulti")]
                CollisionMulti
            }
            

            /// <summary>
            /// Mapping between Shield claim type and shield damage code           
            /// </summary>
            public readonly static IReadOnlyDictionary<ShieldClaimScenario, ShieldApi> ShieldAPIDamageCode = new Dictionary<ShieldClaimScenario, ShieldApi>()
            {
                { ShieldClaimScenario.CollisionWithAnimal,
                    new ShieldApi() {
                        Code = "MCWA"
                    }
                },
                { ShieldClaimScenario.VehicleImpactedbyObject,
                    new ShieldApi() {
                        Code = "MVIO"
                    }
                },
                { ShieldClaimScenario.SingleCollision,
                    new ShieldApi() {
                        Code = "MSVC"
                    }
                },
                { ShieldClaimScenario.MultiCollision,
                    new ShieldApi() {
                        Code = "MMVC"
                    }
                },
                { ShieldClaimScenario.HitWhilstParked,
                    new ShieldApi() {
                        Code = "MHWP"
                    }
                },
                { ShieldClaimScenario.UninsuredMotoristExtension,
                    new ShieldApi() {
                        Code = "MUME"
                    }
                },
                { ShieldClaimScenario.LiabilityOnly,
                    new ShieldApi() {
                        Code = "MLIO"
                    }
                },
                { ShieldClaimScenario.CollisionWithAnotherVehicle,
                    new ShieldApi() {
                        Code = "MCAV"
                    }
                },
                { ShieldClaimScenario.CollisionWithProperty,
                    new ShieldApi() {
                        Code = "MCPR"
                    }
                },
                { ShieldClaimScenario.TPOtherRecovery,
                    new ShieldApi() {
                        Code = "HTPR"
                    }
                },
            };

            /// <summary>
            // Mapping claim scenarios with the Shield API damage type
            /// </summary>
            public readonly static Dictionary<MotorClaimScenario, ShieldApi> MotorClaimScenarioNamesInShield = new Dictionary<MotorClaimScenario, ShieldApi>
            {
                { MotorClaimScenario.WhileDrivingOtherVehicleHitRearOfMyCar,
                    new ShieldApi() {
                        Code = "TPHitPHInRear"
                    }
                },
                { MotorClaimScenario.WhileDrivingOtherVehicleHitMyCarWhenChangingLanes,
                    new ShieldApi() {
                        Code = "TPChangedLanesIntoPH"
                    }
                },
                { MotorClaimScenario.WhileDrivingOtherVehicleFailToGiveWayAndHitMyCar,
                    new ShieldApi() {
                        Code = "TPFailedToGiveWay"
                    }
                },
                { MotorClaimScenario.WhileDrivingOtherVehicleHitMyCarSomethingElseHappened,
                    new ShieldApi() {
                        Code = "OtherMultiCollision"
                    }
                },
                { MotorClaimScenario.WhileDrivingMyCarHitAnotherCarWhenChangingLanes,
                    new ShieldApi() {
                        Code = "PHChangedLanesIntoTP"
                    }
                },
                { MotorClaimScenario.WhileDrivingMyCarHitRearOfAnotherCar,
                    new ShieldApi() {
                        Code = "PHHitTPInRear"
                    }
                },
                { MotorClaimScenario.WhileDrivingMyCarHitAParkedCar,
                    new ShieldApi() {
                        Code = "PHHitTPWhilstParked"
                    }
                },
                { MotorClaimScenario.WhileDrivingMyCarHitAnotherCarFailToGiveWay,
                    new ShieldApi() {
                        Code = "PHFailedToGiveWay"
                    }
                },
                { MotorClaimScenario.WhileDrivingMyCarHitAnotherCarSomethingElseHappened,
                    new ShieldApi() {
                        Code = "OtherMultiCollision"
                    }
                },
                { MotorClaimScenario.WhileDrivingOurCarHitOneAnotherCarWhenChangingLanes,
                    new ShieldApi() {
                        Code = "BothChangingLanes"
                    }
                },
                { MotorClaimScenario.WhileDrivingOurCarHitOneAnotherCarBothFailedToGiveWay,
                    new ShieldApi() {
                        Code = "BothFailedToGiveWay"
                    }
                },
                { MotorClaimScenario.WhileDrivingOurCarHitOneAnotherCarSomethingElseHappened,
                    new ShieldApi() {
                        Code = "OtherMultiCollision"
                    }
                },
                { MotorClaimScenario.WhileDrivingSomethingElseHappened,
                    new ShieldApi() {
                        Code = "OtherMultiCollision"
                    }
                },
                { MotorClaimScenario.WhileReversingHitParkedCar,
                    new ShieldApi() {
                        Code = "PHHitTPWhilstParked"
                    }
                },
                { MotorClaimScenario.WhileReversingHitAnotherCar,
                    new ShieldApi() {
                        Code = "PHReversedIntoTP"
                    }
                },
                { MotorClaimScenario.WhileReversingHitByAnotherCar,
                    new ShieldApi() {
                        Code = "TPHitPHInRear"
                    }
                },
                { MotorClaimScenario.WhileReversingHitByAnotherReversingCar,
                    new ShieldApi() {
                        Code = "BothReversing"
                    }
                },
                { MotorClaimScenario.WhileReversingSomethingElseHappened,
                    new ShieldApi() {
                        Code = "OtherMultiCollision"
                    }
                },
                { MotorClaimScenario.WhileParkedAnotherCarHitMyCar,
                    new ShieldApi() {
                        Code = "TPHitPHWhilstParked"
                    }
                },
                { MotorClaimScenario.WhileParkedSomethingElseHappened,
                    new ShieldApi() {
                        Code = "OtherMultiCollision"
                    }
                },
                { MotorClaimScenario.WhileStationaryAnotherCarHitRearOfMyCar,
                    new ShieldApi() {
                        Code = "TPHitPHInRear"
                    }
                },
                { MotorClaimScenario.WhileStationaryAnotherCarReversedIntoMyCar,
                    new ShieldApi() {
                        Code = "TPReversedIntoPH"
                    }
                },
                { MotorClaimScenario.WhileStationarySomethingElseHappened,
                    new ShieldApi() {
                        Code = "OtherMultiCollision"
                    }
                }
            };

            /// <summary>
            /// When fetching Agenda step from Shield DB and verifying the allocated damage code
            /// we're aligning it based on the initial entered Damage Type.
            /// </summary>
            public readonly static IReadOnlyDictionary<MotorClaimDamageType, IdDescriptions> MotorClaimDamageCodeAndScenarioNames = new Dictionary<MotorClaimDamageType, IdDescriptions>()
            {
                { MotorClaimDamageType.MultipleVehicleCollision,
                    new IdDescriptions() {
                        TextShield = "Multiple Vehicle Collision"
                    }
                },
                { MotorClaimDamageType.SingleVehicleCollision,
                    new IdDescriptions() {
                        TextShield = "Single Vehicle Collision"
                    }
                },
                { MotorClaimDamageType.Theft,
                    new IdDescriptions() {
                        TextShield = "Theft of Vehicle"
                    }
                },
                { MotorClaimDamageType.WindscreenGlassDamage,
                    new IdDescriptions() {
                        TextShield = "Glass Damage"
                    }
                },
                { MotorClaimDamageType.MaliciousDamage,
                    new IdDescriptions() {
                        TextShield = "Malicious Damage"
                    }
                },
                { MotorClaimDamageType.Fire,
                    new IdDescriptions() {
                        TextShield = "Fire"
                    }
                },
                { MotorClaimDamageType.Storm,
                    new IdDescriptions() {
                        TextShield = "Storm"
                    }
                },
                { MotorClaimDamageType.Earthquake,
                    new IdDescriptions() {
                        TextShield = "Earthquake"
                    }
                },
                { MotorClaimDamageType.Flood,
                    new IdDescriptions() {
                        TextShield = "Flood"
                    }
                },
                { MotorClaimDamageType.Other,
                    new IdDescriptions() {
                        TextShield = "Other"
                    }
                }
            };

            public enum GlassDamageType
            {
                Chip = 1000022,
                Crack = 1000023,
                Shattered = 1000024
            };

            public enum MotorGlassRepairer
            {
                [Description("O'Brien Autoglass")]
                OBrien,
                [Description("Novus Autoglass")]
                Novus,
                [Description("Instant Windscreens")]
                Instant
            }

            /// <summary>
            /// The potential options available for selection if more than one
            /// repairer is available in the area provided by the member.
            /// </summary>
            public enum RepairerOption
            {
                None,
                First,
                Second,
                GetQuote
            }


            public enum ShieldEvent
            {
                [Description("Create Claimant Asset Action")]
                CreateClaimantAssetAction,
                [Description("New Damages Created")]
                NewDamagesCreated,
                [Description("Refer claimant asset to service provider")]
                ReferClaimantAssetToServiceProvider,
                [Description("Glazier Automatically Allocated")]
                GlazierAutomaticallyAllocated,
                [Description("Automatic motor glass lodgement")]
                AutomaticMotorGlassLodgement,
                [Description("Online Claim Opened")]
                OnlineClaimOpened,
                [Description("Incomplete Online Claim Follow up Closed")]
                IncompleteOnlineClaimFollowUpClosed,
                [Description("Invoice Documents Received")]
                InvoiceDocumentsReceived
            };

            public static readonly string SingleVehicleCollisionEvent = "Online Claim Lodgement - Motor Single Vehicle Collision";
            public static readonly string MultiVehicleCollisionEvent = "Online Claim Lodgement - Motor Multiple Vehicle Collision";
        }
    }
}
