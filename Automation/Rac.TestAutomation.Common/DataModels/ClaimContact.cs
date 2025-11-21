using Rac.TestAutomation.Common.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    public class ClaimContact
    {
        public enum ExpectedCSFSOutcome
        {
            [Description("Cash Settlement Accepted")]
            Accepted,
            [Description("Cash Settlement Offer Rejected")]
            Declined,
            [Description("EFT Confirmation Received")]
            EFT
        }

        public string ClaimNumber { get; set; }
        public Contact Beneficiary { get; set; }

        public ExpectedCSFSOutcome ExpectedOutcome { get; set; }

        public ClaimsEFTFLow ClaimsEFTFlowType { get; set; }

        public ClaimContact(string claimNumber, Contact beneficiary)
        {
            this.ClaimNumber = claimNumber;
            this.Beneficiary = beneficiary;           
        }

        public ClaimContact WithExpectedOutcomeForTest(ExpectedCSFSOutcome expectedOutcome)
        {
            this.ExpectedOutcome = expectedOutcome;
            return this;
        }

        public override string ToString()        {
          
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Claim data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Claim Number:              {ClaimNumber}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Beneficiary Contact Id:    {Beneficiary.Id}{Reporting.HTML_NEWLINE}");
            return formattedString.ToString();
        }

        public enum ClaimsEFTFLow
        {
            [Description("CSFS")]
            CSFS,
            [Description("EFT")]
            EFT
        }

        public ClaimContact WithClaimsEFTFlow(ClaimsEFTFLow claimsEFTFLow)
        {
            this.ClaimsEFTFlowType = claimsEFTFLow;
            return this;
        }
    }
}
