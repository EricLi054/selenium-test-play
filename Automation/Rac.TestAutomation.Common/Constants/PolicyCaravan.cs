using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class PolicyCaravan
        {
            //Maximum amount that Spark caravan application allows to insure a caravan, when caravan market value is known.
            public const int CARAVAN_MAX_SUM_INSURED_VALUE = 150000;
            //This is the minimum amount that a user can specify as the 'Value of the caravan' (on 'Let's start with your caravan' page) or 'Choose insured value' (on 'Here's your quote' page)
            public const int CARAVAN_MIN_SUM_INSURED_VALUE = 1000;
            public const int CARAVAN_MIN_EXCESS_VALUE = 0; //The minimum value that can be specified for caravan Excess value
            public const int CARAVAN_DEFAULT_CONTENT_INSURANCE_VALUE = 1000;   //Allowable minimum value (as well as the default value) for the caravan contents insurance.

            public static readonly int MIN_AGE_FOR_EXCESS_WAIVER   = 50;    //The age that either of the Policyholders should be, to get $0 excess.
            public static readonly int MAX_CONTENT_INSURANCE_VALUE = 15000; //Allowable maximum value for the caravan contents insurance.

            public const string CARAVAN_PERSONAL_USAGE_TEXT = "No business or commercial use";    //Caravan usage policy summary text for verification 
            public const string DRIVING_HISTORY_NO_ACCIDENT_TEXT = "No accidents or claims in the past 3 years";   //Diver accident history text policy summary text for verification 
            public const string DRIVING_HISTORY_NO_CANCELLATION_TEXT = "No cancellation, suspension or special conditions applied to drivers licence in the past 3 years";  //Diver licence cancellation history text policy summary text for verification 

            // TODO: SPK-6704 Remove the constant when this story is actioned. This logic will no longer be required as all the active caravan policies will be on product version ID 68000024
            /// <summary>
            /// Caravan product version ID that Shield introduced for Excess and NCB changes.
            /// This has released on 27th of August 2024, and can be removed
            /// from automation after August 2025 (as all Caravan policies will be on this
            /// version or newer after August 2025).
            /// </summary>
            public static readonly int CaravanProductVersionIdWithExcessNcbChanges = 68000024;

            public enum CaravanType
            {
                [Description("Caravan")]
                Caravan,
                [Description("Trailer")]
                Trailer
            };

            /// <summary>
            /// This enum is used to answer the Spark Caravan question
            /// "Where is your caravan usually parked"
            /// </summary>
            public enum CaravanParkLocation
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
                [Description("On-Site")]
                OnSite
            };

            /// <summary>
            /// This text used for to verify Caravan kept text in Policy summary page
            /// </summary>
            public readonly static IReadOnlyDictionary<CaravanParkLocation, string> CaravanParkedText = new Dictionary<CaravanParkLocation, string>()
            {
                { CaravanParkLocation.StreetOrVerge, "Parked on street or verge"},
                { CaravanParkLocation.OnSite, "Parked on-site"},
                { CaravanParkLocation.Garage, "Parked in garage"},
                { CaravanParkLocation.Carport, "Parked in carport"},
                { CaravanParkLocation.Driveway, "Parked in driveway"},
                { CaravanParkLocation.CommunalCarpark, "Parked in communal carpark"}
            };
        }
    }
}
