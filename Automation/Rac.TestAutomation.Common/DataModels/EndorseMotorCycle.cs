using Rac.TestAutomation.Common.API;
using System.Text;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Rac.TestAutomation.Common
{
    public class EndorseMotorCycle : EndorseVehicle
    {
        public EndorseMotorCycle() : base()
        {
        }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            var motorCycle = DataHelper.GetVehicleDetails(OriginalPolicyData.MotorcycleAsset.VehicleId)?.Vehicles[0];

            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Endorsement data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Policy/Contact:  {PolicyNumber} / {ActivePolicyHolder.Id}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Current Premium:  ${OriginalPolicyData.AnnualPremium.Total}{Reporting.HTML_NEWLINE}");
            if (OriginalPolicyData.NextPendingInstallment() != null)
            { formattedString.AppendLine($"Current Instalment:  ${OriginalPolicyData.NextPendingInstallment().Amount.Total}{Reporting.HTML_NEWLINE}"); }
            if (ParkingAddress != null)
            { formattedString.AppendLine($"           Address:  {ParkingAddress.Suburb}{Reporting.HTML_NEWLINE}"); }
            if (motorCycle != null)
            { formattedString.AppendLine($"           MotorCycle:  {motorCycle.MakeDescription} {motorCycle.ModelYear} {motorCycle.ModelFamily}{Reporting.HTML_NEWLINE}"); }
            if (!string.IsNullOrEmpty(Excess))
            { formattedString.AppendLine($"            Excess:  ${Excess}{Reporting.HTML_NEWLINE}"); }
            if (InsuredVariance != 0)
            { formattedString.AppendLine($"         Change SI:  by {InsuredVariance}%{Reporting.HTML_NEWLINE}"); }
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            return formattedString.ToString();
        }
    }
}
