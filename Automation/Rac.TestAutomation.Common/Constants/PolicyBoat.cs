using System.Collections.Generic;
using System.ComponentModel;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class PolicyBoat
        {
            public static readonly int BOAT_MAXIMUM_INSURED_VALUE_ONLINE = 150000;
            public enum BoatType
            {
                [Description("Monohull Yacht")]
                MonohullYacht,
                [Description("Multihull Yacht")]
                MultihullYacht,
                [Description("Powerboat")]
                PowerBoat,
                [Description("Runabout")]
                Runabout,
                [Description("Ski Boat")]
                SkiBoat,
                [Description("Windsurfer/Sailboard")]
                WindsurferSailboard,
                [Description("Sailboat")]
                Sailboat
            }

            public enum SparkBoatTypeExternalCode
            {
                [Description("Powerboat")]
                P,
                [Description("Sailboat")]
                L
            }

            /// <summary>
            /// This enum is used to answer the Spark Boat question
            /// around where the boat is stored), currently wireframe
            /// suggests we're just going to ask "Boat kept in a garage?"
            /// with a Yes/No response but I believe this will not 
            /// suffice for excluding new business Moored/Penned at
            /// a Marina.
            /// </summary>
            public enum BoatRiskLocation
            {
                [Description("Garage")]
                Garage,
                [Description("Carport")]
                Carport,
                [Description("Driveway")]
                Driveway,
                [Description("Street or Verge")]
                StreetOrVerge,
                [Description("Communal Carpark")]
                CommunalCarpark,
                [Description("Moored or Penned at a Boat Club or Marina")] // Expect this will be retained as a knock-out option only.
                Marina
            };

            /// <summary>
            /// This text used for to verify where the Boat is kept text in the Policy summary page
            /// </summary>
            public readonly static IReadOnlyDictionary<BoatRiskLocation, string> BoatParkedText = new Dictionary<BoatRiskLocation, string>()
            {
                { BoatRiskLocation.StreetOrVerge, "Parked on street or verge"},
                { BoatRiskLocation.Garage, "Parked in garage"},
                { BoatRiskLocation.Carport, "Parked in carport"},
                { BoatRiskLocation.Driveway, "Parked in driveway"},
                { BoatRiskLocation.CommunalCarpark, "Parked in communal carpark"},
                { BoatRiskLocation.Marina, "Moored or Penned at a Boat Club or Marina"} // Expect this will be retained as a knock-out option only.
            };
            public enum SkippersTicketYearsHeld
            {
                [Description("I don't have a skipper's ticket")]
                Noskippersticket,
                [Description("Less than 1 year")]
                Lessthan1,
                [Description("1-2")]
                LessThanTwo,
                [Description("2-3")]
                LessThanThree,
                [Description("3+")]
                MoreThanThree
            }
            public enum BoatClaimsInLastThreeYears
            {
                [Description("0")]
                Zero,
                [Description("1")]
                One,
                [Description("2")]
                Two,
                [Description("3")]
                Three,
                [Description("4")]
                Four,
                [Description("5")]
                Five,
                [Description("6+")]
                SixOrMore
            }
            public enum BoatMake
            {
                [Description("Achilles")]
                Achilles,
                [Description("Alumarine")]
                Alumarine,
                [Description("Barracuda")]
                Barracuda,
                [Description("Galeforce")]
                Galeforce,
                [Description("Haines Hunter")]
                HainesHunter,
                [Description("Haines Signature")]
                HainesSignature,
                [Description("Kingcraft")]
                Kingcraft,
                [Description("Other")]
                Other,
                [Description("Quintrex")]
                Quintrex,
                [Description("Scorpion")]
                Scorpion,
                [Description("Sea Breeze")]
                SeaBreeze,
                [Description("Sea Change")]
                SeaChange
            }
            public enum BoatHullMaterial
            {
                [Description("Aluminium")]
                N,
                [Description("Fibreglass")]
                I,
                [Description("Glass reinforced plastic")]
                G,
                [Description("Plastic")]
                P,
                [Description("Steel")]
                L,
                [Description("Wood")]
                W
            }
            public enum MotorType
            {
                [Description("Inboard")]
                Inboard,
                [Description("Outboard")]
                Outboard,
                [Description("No motor")]
                NoMotor
            }
        }
    }
}
