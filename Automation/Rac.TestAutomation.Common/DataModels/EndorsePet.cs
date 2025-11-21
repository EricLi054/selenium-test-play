using System.Text;

namespace Rac.TestAutomation.Common
{
    public class EndorsePet : EndorsementBase
    {
        public EndorsePet() : base()
        {
        }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();

            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Endorsement data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Policy/Contact:  {PolicyNumber} / {ActivePolicyHolder.Id}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Current Premium:  ${OriginalPolicyData.AnnualPremium.Total}{Reporting.HTML_NEWLINE}");
            if (OriginalPolicyData.NextPendingInstallment() != null)
            { formattedString.AppendLine($"Current Instalment:  ${OriginalPolicyData.NextPendingInstallment().Amount.Total}{Reporting.HTML_NEWLINE}"); }               
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            return formattedString.ToString();
        }
    }
}
