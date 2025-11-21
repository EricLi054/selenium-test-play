using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    public class QuoteData
    {
        public string QuoteNumber { get; set; }
        public decimal AnnualPremium { get; set; }
        public decimal MonthlyPremium { get; set; }
        public decimal PremiumBreakdownBasic { get; set; }
        public decimal PremiumBreakdownStamp { get; set; }
        public decimal PremiumBreakdownGST { get; set; }

        public HomeQuoteCoverValues HomeInsuredValueAndExcess { get; set; }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine("-------------------------------------<br>");
            formattedString.AppendLine("--- Caravan Quote data:<br>");
            formattedString.AppendLine($"   Quote Number:    {QuoteNumber}<br>");
            formattedString.AppendLine($"   Annual Premium:    {AnnualPremium}<br>");
            formattedString.AppendLine($"   Monthly Premium:    {MonthlyPremium}<br>");
            return formattedString.ToString();
        }
    }

    public class HomeQuoteCoverValues
    {
        public int SumInsuredBuilding { get; set; }
        public int SumInsuredContents { get; set; }
        public string ExcessBuilding  { get; set; }
        public string ExcessContents  { get; set; }
    }

    public class BoatQuoteValues
    {
        public string PolicyUpdateUser { get; set; }
        public string QuoteNumber { get; set; }
        public string PolicyNumber { get; set; }
        public string Status { get; set; }
        public string BasicExcess { get; set; }
        public string HasWaterSkiingAndFlotationDeviceCover { get; set; }
        public string HasRacingCover { get; set; }
        public string OriginalChannel { get; set; }
        public string Discount { get; set; }
        public string Ncb { get; set; }
        public string OldHasAlarmShouldBeNull { get; set; }
        public string OldHasGpsShouldBeNull { get; set; }
        public string VehicleUsageDscShouldBeNull { get; set; }
    }
}
