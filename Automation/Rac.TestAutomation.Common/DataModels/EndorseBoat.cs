using System.Text;

using static Rac.TestAutomation.Common.Constants.PolicyBoat;

namespace Rac.TestAutomation.Common
{
    public class EndorseBoat : EndorsementBase
    {
        public EndorseBoat() : base()
        {
        }
        public BoatType Type { get; set; }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Endorsement data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Policy/Contact:  {PolicyNumber} / {ActivePolicyHolder.Id}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            return formattedString.ToString();
        }
    }
}
