using Rac.TestAutomation.Common.API;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Rac.TestAutomation.Common
{
    abstract public class EndorseVehicle : EndorsementBase
    {
        public AnnualKms   AnnualKm  { get; set; }
        public Address     ParkingAddress { get; set; }
        /// <summary>
        /// The desired excess for the requested policy.
        /// If a null value, then just used the Shield provided default.
        /// </summary>
        public string Excess { get; set; }

        /// <summary>
        /// Property to capture displayed premium pricing changes for assertion
        /// at completion of test.
        /// </summary>
        public PremiumDetails PremiumChangesAfterEndorsement { get; set; }

        /// <summary>
        /// This is a percentage variance from the market value of the
        /// requested vehicle. The percentage is given as a whole integer.
        /// A negative value is a reduction under markefst value, and a
        /// positive integer is an increase from market value.
        /// E.g.: a value of "10" would mean a 10% increase from market
        /// value.
        ///       a value of "-15" would mean a 15% decrease from
        /// market value.
        /// </summary>
        public int InsuredVariance { get; set; }

        /// <summary>
        /// This flag will be used to drive motor and caravan endorsement 
        /// This will help drive test flow when Change of make and model needed on renewal or mid term 
        /// endorosement for motor and caravan
        /// </summary>
        public bool ChangeMakeAndModel { get; set; }

        public EndorseVehicle(): base()
        { }
    }
}