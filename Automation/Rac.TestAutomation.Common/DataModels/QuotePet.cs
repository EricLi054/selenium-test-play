using Rac.TestAutomation.Common;
using System;
using System.Text;

using static Rac.TestAutomation.Common.Constants.PolicyPet;

namespace Rac.TestAutomation.Common
{
    public class QuotePet : Pet
    {
        public Address ResidingAddress { get; set; }
        public bool    IsInsured 
        {
            get 
            {
                return !string.IsNullOrEmpty(CurrentInsurer);
            }
        }
        public string  CurrentInsurer { get; set; }
        public Contact PolicyHolder { get; set; }
        public Payment PayMethod { get; set; }
        public DateTime StartDate { get; set; }
        public bool AddTlc { get; set; }
        /// <summary>
        /// The desired excess for the requested policy.
        /// If a null value, then just used the Shield provided default.
        /// </summary>
        public string Excess { get; set; }

        public string GetPetAge()
        {
            string ageString = null;

            var years = DateTime.Now.Year - DateOfBirth.Year;
            if (DateOfBirth.DayOfYear > DateTime.Now.DayOfYear) { years--; }
            if (years == 0) 
            {
                var months = DateTime.Now.Month - DateOfBirth.Month;
                if (DateOfBirth.Day > DateTime.Now.Day) { months--; }

                // Correction to month count if Pet's DoB was last year.
                // Example: DoB Dec 2020 and we're in Jan 2021
                //      Jan (1) - Dec (12) = -11 months
                //      We add 12 to correct to 1 month.
                if (DateOfBirth.Year < DateTime.Now.Year) 
                { months += 12; }

                // B2C rounds pet age up to 2 months, if it is under.
                if (months < 2)
                { months = 2; }

                ageString = $"{months} months";
            }
            else
            {
                ageString = years > 1 ? $"{years} years" : $"{years} year";
            }
            return ageString;
        }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Pet quote data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Pet:        {Type.GetDescription()}, {Gender.GetDescription()}, {Breed}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Pet Name:   {Name}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Pet DoB:    {DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_HYPHENS)}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    TLC cover:  {AddTlc}{Reporting.HTML_NEWLINE}");
            formattedString.Append($"--- Policy holders:{Reporting.HTML_NEWLINE}");
            formattedString.Append(PolicyHolder.ToString());
            return formattedString.ToString();
        }
    }
}
