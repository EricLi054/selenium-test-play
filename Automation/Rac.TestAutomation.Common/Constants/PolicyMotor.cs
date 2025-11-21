using System.Collections.Generic;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class PolicyMotor
        {
            public const int MOTOR_COVER_MIN_INSURABLE_VALUE = 1000;
            public const int MOTOR_COVER_MAX_INSURABLE_VALUE = 200000;
            public const int MOTOR_COVER_TFT_MAXVALUE = 15000; // B2C-3148 STRETCH: Motor - TPFT cover is only available where the sum insured selected is $15000 or less.
            public const int MOTOR_COVER_MIN_EXCESS = 0;
            public const int MOTOR_COVER_MFCO_MAX_EXCESS = 2000;
            public const int MOTOR_COVER_TPO_TFT_MAX_EXCESS = 500;
            // TODO: INSU-286 Rename this constant when the story is actioned
            public const string MOTOR_COVER_TPO_TFT_DEFAULT_EXCESS_POST_VERSION_46 = "700";
            public const decimal MOTOR_ROADSIDE_AMOUNT = 160;   // Roadside Price Increase Effective 1 July 2025. INSU-1146

            /// <summary>
            /// This is the product version that the Shield support for motor
            /// risk address starts. So when doing an endorsement, we don't
            /// look for the MRA prompt if the policy is on an earlier version.
            ///
            /// TODO: B2C-4561 This can be removed altogether one older versions have 'aged' out.
            /// </summary>
            public const int MotorRiskAddressStartVersion = 40;

            // TODO: SPK-6704 Remove this constant when this story is actioned. This logic will no longer be required as all the active motor policies will be on product version ID 68000023
            /// <summary>
            /// Motor product version ID that Shield introduced for Excess and NCB changes.
            /// The anticipated release date is 27th of August 2024, and can be removed
            /// from automation after August 2025 (as all motor policies will be on this
            /// version or newer after August 2025).
            /// </summary>
            public const int MotorProductVersionIdWithExcessNcbChanges = 68000023;

            //Stand-in value when Member Number is not returned after Roadside Assistance Bundling purchase
            public const string MEMBER_NUMBER_NOT_RETURNED = "T.B.A.";

            public const string REGISTRATION_NOT_PROVIDED = "To be advised";

            public class MotorRiskAddress
            {
                /// <summary>
                /// Stand-in Street Address information when Mailing Address is PO Box/Locked Bag
                /// </summary>
                public static Address Generic => new Address()
                {
                    StreetNumber = "2351",
                    StreetOrPOBox = "Albany Hwy",
                    Suburb = "Gosnells",
                    PostCode = "6110",
                    State = "WA",
                    Country = "Australia"
                };

                /// <summary>
                /// For motor policy endorsements requiring changing to
                /// a high rick location
                /// </summary>
                public static Address HighRisk => new Address()
                {
                    StreetNumber = "8",
                    StreetOrPOBox = "Kirkcolm Way",
                    Suburb = "Warwick",
                    PostCode = "6024",
                    State = "WA",
                    Country = "Australia"
                };

                /// <summary>
                /// For motor policy endorsements requiring changing to
                /// a low risk location.
                /// </summary>
                public static Address LowRisk => new Address()
                {
                    StreetNumber = "18",
                    StreetOrPOBox = "Jasper Way",
                    Suburb = "Lakelands",
                    PostCode = "6180",
                    State = "WA",
                    Country = "Australia"
                };
            }

            public static class ChildCovers
            {
                public static readonly string HireCarAfterAccident = "MHAO";
            }

            public enum MotorCovers
            {
                MFCO,
                TFT,
                TPO
            }

            public enum NCB
            {
                Yes,
                No,
                NA
            }

            public enum VehicleLookupType
            {
                Registration = 0,
                MakeAndModel = 1
            }

            /// <summary>
            /// These are UI labels applied to covers relevant for general motor (car) insurance
            /// </summary>
            public static Dictionary<MotorCovers, IdDescriptions> MotorCoverNameMappings = new Dictionary<MotorCovers, IdDescriptions>()
            {
                { MotorCovers.MFCO, new IdDescriptions() { TextB2C = "Comprehensive",            TextShield = "Full Cover" } },
                { MotorCovers.TFT,  new IdDescriptions() { TextB2C = "Third Party Fire & Theft", TextShield = "TFT" } },
                { MotorCovers.TPO,  new IdDescriptions() { TextB2C = "Third Party Property",     TextShield = "TPO" } }
            };

            public enum VehicleUsage
            {
                Undefined,
                Private,
                Business,
                CourierOrDelivery,
                Ridesharing,
                HiredOrleasedOutSmall,
                TaxiOrSmallCharter,
                LimoOrChauffeured
            }

            public static Dictionary<VehicleUsage, IdDescriptions> VehicleUsageNameMappings = new Dictionary<VehicleUsage, IdDescriptions>()
            {
                { VehicleUsage.Private,            new IdDescriptions() { TextB2C = "Private",  TextSpark="Private",  TextShield = "Private" } },
                { VehicleUsage.Business,           new IdDescriptions() { TextB2C = "Business", TextSpark="Business", TextShield = "Business" } },
                { VehicleUsage.CourierOrDelivery,  new IdDescriptions() { TextB2C = "Courier or delivery", TextSpark="Courier or delivery", TextShield = "Courier/Delivery" } },
                { VehicleUsage.HiredOrleasedOutSmall, new IdDescriptions() { TextB2C = "Hired or leased out", TextSpark="Hired or leased out", TextShield = "Hired or Leased out" } },
                { VehicleUsage.LimoOrChauffeured,  new IdDescriptions() { TextB2C = "Limo or chauffeured", TextSpark="Limo or chauffeured", TextShield = "Charter/Commercial" } },
                { VehicleUsage.Ridesharing,        new IdDescriptions() { TextB2C = "Ridesharing - part-time (less than 30 hours per week)", TextSpark="Ridesharing", TextShield = "Ridesharing" } },
                { VehicleUsage.TaxiOrSmallCharter, new IdDescriptions() { TextB2C = "Taxi or small charter", TextSpark="Taxi or small charter", TextShield = "Taxi/Limo/Small Charter Vehicle" } }
            };


            /// <summary>
            /// *** B2C and PCM only ***
            /// Derived from 'VehicleUsage' enum, but filtered to
            /// only contain the values that General Motor will accept
            /// in a B2C/PCM quote/endorsement.
            /// </summary>
            public enum VehicleUsageAccepted
            {
                Private = VehicleUsage.Private,
                Business = VehicleUsage.Business,
                RidesharingParttime = VehicleUsage.Ridesharing
            }

            /// <summary>
            /// These are Shield DB Id mappings for motor covers
            /// </summary>
            public static Dictionary<MotorCovers, string> MotorCoverIdMappings = new Dictionary<MotorCovers, string>()
            {
                { MotorCovers.MFCO, "1000013" },
                { MotorCovers.TFT,  "1000017" },
                { MotorCovers.TPO,  "1000018" }
            };


            /// <summary>
            /// List of MotorWeb test vehicles
            /// </summary>
            public static List<Car> MotorWebTestVehicles = new List<Car>()
            {
                new Car() { Make = "HOLDEN", Year = 2007, Model = "STATESMAN", Body = "SEDAN", Transmission = "6 SP AUTO ACTIVE SEQ", VehicleId = "5100092", Registration = "0" },
                new Car() { Make = "LEXUS", Year = 2016, Model = "RX350", Body = "WAGON", Transmission = "8 SP AUTOMATIC", VehicleId = "", Registration = "5272283" },
                new Car() { Make = "TOYOTA", Year = 2014, Model = "LANDCRUISER", Body = "WAGON", Transmission = "5 SP SEQUENTIAL AUTO", VehicleId = "5252983", Registration = "CATHLIN7" },
                new Car() { Make = "HYUNDAI", Year = 2006, Model = "i30", Body = "HATCHBACK", Transmission = "6 SP AUTOMATIC", VehicleId = "5275867", Registration = "1GEH772" },
                new Car() { Make = "RANGE ROVER", Year = 2006, Model = "RANGE ROVER", Body = "WAGON", Transmission = "8 SP AUTOMATIC", VehicleId = "5252830", Registration = "NZ00WA" }
            };

            /// <summary>
            /// Partial list of vehicle manufacturer codes to
            /// support some selective vehicle searches.
            /// </summary>
            public class Manufacturers
            {
                public const string Ford       = "FOR";
                public const string Holden     = "HOL";
                public const string Mazda      = "MAZ";
                public const string Mitsubishi = "MIT";
                public const string Nissan     = "NIS";
                public const string Toyota     = "TOY";
                public const string Audi       = "AUD";
                public const string BMW        = "BMW";
                public const string Citroen    = "CIT";
                public const string Mercedes   = "MER";
                public const string Porsche    = "POR";
            }
        }
    }
}