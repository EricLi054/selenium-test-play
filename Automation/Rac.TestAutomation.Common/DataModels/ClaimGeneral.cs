using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    /// <summary>
    /// Class for a Contact (Witness/Driver/TP) retrieved
    /// from the Shield DB in relation to a claim.
    /// </summary>
    public class ContactClaimDB : Contact
    {
        public ContactClaimDB() { }
        /// <summary>
        /// Set by Shield DB when retrieving TP from lodged claim
        /// </summary>
        public string DBTPRole { get; set; }
        public string DBTPEMailStatus { get; set; }
        public string DBTPPhoneStatus { get; set; }
        public string DBTPAddressStatus { get; set; }
    }

    /// <summary>
    /// Class for upload file for claim
    /// </summary>
    public class ClaimUploadFile
    {
        public string ClaimNumber { get; set; }
        public string PersonId { get; set; }
        public string ClaimantFirstName { get; set; }
        public List<string> File { get; set; }

        public ClaimUploadFile(string claimNumber, string persoonId, string claimantFirstName, List<string> file)
        {
            ClaimNumber = claimNumber;
            PersonId = persoonId;
            ClaimantFirstName = claimantFirstName;
            File = file;
        }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Claim data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Claim Number:          {ClaimNumber}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Claimant Person Id:    {PersonId}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Claimant First Name:   {ClaimantFirstName}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Document Name :        {string.Join(", ", File)}{Reporting.HTML_NEWLINE}");
            return formattedString.ToString();
        }
    }
}
