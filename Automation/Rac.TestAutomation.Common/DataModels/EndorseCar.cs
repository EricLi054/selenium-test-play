using Rac.TestAutomation.Common.API;
using System.Text;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace Rac.TestAutomation.Common
{
    public class EndorseCar: EndorseVehicle
    {
        public Car InsuredAsset { get; set; }
        public Car NewInsuredAsset { get; set; }
        public MotorCovers CoverType { get; set; }
        public VehicleUsage UsageType { get; set; }

        // TODO: SPK-6704 Remove this method when this story is actioned. This logic will no longer be required as all the active motor policies will be on product version 46
        /// <summary>
        /// Method checks the product version of the motor policy
        /// Excess NCB changes are applicable on product version 46 and onwards
        /// the anticipated release date is 27th of August 2024
        /// </summary>
        public bool IsMotorPolicyWithExcessChanges() => CurrentProductVersionNumber > 45;

        public EndorseCar(): base()
        { }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Endorsement data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Policy Number:  {PolicyNumber}");
            formattedString.AppendLine($"    PH Contact Id:  {ActivePolicyHolder.Id}");
            formattedString.AppendLine($"    Confirm right make and model:  {ChangeMakeAndModel}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Current Premium:  ${OriginalPolicyData.AnnualPremium.Total}{Reporting.HTML_NEWLINE}");
            if (OriginalPolicyData.NextPendingInstallment() != null)
            { formattedString.AppendLine($"Current Instalment:  ${OriginalPolicyData.NextPendingInstallment().Amount.Total}{Reporting.HTML_NEWLINE}"); }
            if (ParkingAddress != null)
            { formattedString.AppendLine($"       New Address:  {ParkingAddress.Suburb}{Reporting.HTML_NEWLINE}"); }
            if (InsuredAsset != null && !string.IsNullOrEmpty(InsuredAsset.Make))
            { formattedString.AppendLine($"       Car Details:  {InsuredAsset.Make} {InsuredAsset.Year} {InsuredAsset.Model} {InsuredAsset.Body} {InsuredAsset.Transmission}{Reporting.HTML_NEWLINE}"); }
            if (NewInsuredAsset != null && !string.IsNullOrEmpty(NewInsuredAsset.Make))
            { formattedString.AppendLine($"   New Car Details:  {NewInsuredAsset.Make} {NewInsuredAsset.Year} {NewInsuredAsset.Model} {NewInsuredAsset.Body} {NewInsuredAsset.Transmission}{Reporting.HTML_NEWLINE}"); }
            if (!string.IsNullOrEmpty(Excess))
            { formattedString.AppendLine($"        New Excess:  ${Excess}{Reporting.HTML_NEWLINE}"); }
            if (InsuredVariance != 0)
            { formattedString.AppendLine($"         Change SI:  by {InsuredVariance}%{Reporting.HTML_NEWLINE}"); }
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            return formattedString.ToString();
        }
    }
}
