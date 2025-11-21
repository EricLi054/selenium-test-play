using System;

using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyHome;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using Rac.TestAutomation.Common.API;

namespace Rac.TestAutomation.Common
{
    /// <summary>
    /// Support policy endorsement tests that verify change in state.
    /// </summary>
    public class PolicyData
    {
        public string PolicyNumber { get; set; }
        public CoverHome HomeCovers { get; set; }
        public CoverMotor MotorCovers { get; set; }
        public CoverMotorcycle MotorcycleCovers { get; set; }
        public decimal AnnualPremium { get; set; }
        public DateTime RenewalDate { get; set; }
        public string HomeAddress { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public PremiumDetails TotalInstallmentPremium { get; set; }
        public DateTime CollectionDate { get; set; }
    }

    public class Cover
    {
        public int Excess { get; set; }
        public int SumInsured { get; set; }
        public string BuildingExcess { get; set; }
        public int BuildingSumInsured { get; set; }
        public string ContentsExcess { get; set; }
        public int ContentsSumInsured { get; set; }
    }

    public class CoverMotor : Cover
    {
        public MotorcarDetails motorCarDetails { get; set; }
        public MotorCovers CoverType { get; set; }
        public string HireCarAfterAccident { get; set; }
        public NCB NoClaimBonus { get; set; }
        public AnnualKms AnnualKms { get; set; }
    }

    public class CoverMotorcycle : Cover
    {
        public MotorCovers CoverType { get; set; }
    }

    public class CoverHome : Cover
    {
        public HomeCover CoverType { get; set; }
    }

    public class PolicyPaymentData
    {
        public string BSB { get; set; }
        public string BankAccountNumber { get; set; }
        // Collection date for the next instalment
        public DateTime CollectionDate { get; set; }
        // Next instalment amount
        public decimal CollectionAmount { get; set; }
        public decimal AnnualPremium { get; set; }
    }


}
