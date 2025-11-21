using Rac.TestAutomation.Common.API;
using System.Text;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Rac.TestAutomation.Common
{
    public class EndorseCaravan : EndorseVehicle
    {
        public EndorseCaravan() : base()
        {
        }

        public Caravan InsuredAsset { get; set; }
        public Caravan NewInsuredAsset { get; set; }

        /// <summary>
        /// Member requested insured content cover for the caravan.
        /// This can be used to update the original content cover
        /// </summary>
        public string ContentCover { get; set; }

        /// <summary>
        /// This can be used to store the agreed value updated on the 
        /// endorsement flow
        /// </summary>
        public string NewAgreedValue { get; set; }

        /// <summary>
        /// This is used to set the caravan parked location such as Driverway, Garage etc
        /// </summary>
        public CaravanParkLocation Parked { get; set; }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            var caravan = DataHelper.GetVehicleDetails(OriginalPolicyData.CaravanAsset.VehicleId)?.Vehicles[0];

            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Endorsement data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Policy/Contact:  {PolicyNumber} / {ActivePolicyHolder.Id}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Current Premium:  ${OriginalPolicyData.AnnualPremium.Total}{Reporting.HTML_NEWLINE}");
            if (OriginalPolicyData.NextPendingInstallment() != null)
            { formattedString.AppendLine($"Current Instalment:  ${OriginalPolicyData.NextPendingInstallment().Amount.Total}{Reporting.HTML_NEWLINE}"); }
            if (ParkingAddress != null)
            { formattedString.AppendLine($"           Address:  {ParkingAddress.Suburb}{Reporting.HTML_NEWLINE}"); }
            formattedString.AppendLine($"           Parked:  {Parked}{Reporting.HTML_NEWLINE}");
            if (caravan != null)
            { formattedString.AppendLine($"           Caravan:  {caravan.MakeDescription} {caravan.ModelYear} {caravan.ModelFamily}{Reporting.HTML_NEWLINE}"); }
            if (!string.IsNullOrEmpty(Excess))
            { formattedString.AppendLine($"            Excess:  ${Excess}{Reporting.HTML_NEWLINE}"); }
            if (InsuredVariance != 0)
            { formattedString.AppendLine($"         Change SI:  by {InsuredVariance}%{Reporting.HTML_NEWLINE}"); }
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            if (ChangeMakeAndModel)
            { formattedString.AppendLine($"   New Caravan Details:  {NewInsuredAsset.Make} {NewInsuredAsset.Year} {NewInsuredAsset.Model} {NewInsuredAsset.VehicleId} {NewInsuredAsset.MarketValue}{Reporting.HTML_NEWLINE}"); }

            return formattedString.ToString();
        }
    }
}
