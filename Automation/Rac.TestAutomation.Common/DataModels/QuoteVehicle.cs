using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;

namespace Rac.TestAutomation.Common
{
    public class QuoteCar : Car
    {
        public VehicleUsage  UsageType { get; set; }
        public AnnualKms     AnnualKm  { get; set; }
        public Address            ParkingAddress { get; set; }
        public string             CurrentInsurer { get; set; }
        public List<Driver>              Drivers { get; set; }
        public Payment                 PayMethod { get; set; }
        public DateTime                StartDate { get; set; }
        public MotorCovers   CoverType { get; set; }
        public bool                  AddRoadside { get; set; }
        public bool      AddHireCarAfterAccident { get; set; }
        /// <summary>
        /// In some tests we want to trigger Decline notices for the user
        /// (e.g. Disclosures outside of acceptance for online quotes) then
        /// restore the proposal to an acceptable state so policy purchase
        /// can be completed.
        /// </summary>
        public bool     DisclosureDeclineThenDismiss      { get; set; }
        /// <summary>
        /// In some tests we want to trigger Decline notices for the user
        /// because of unacceptable vehicle usage and then restore the 
        /// proposal to an acceptable state so policy purchase can be completed.
        /// </summary>
        public bool VehicleUsageDeclineThenDismiss { get; set; }
        /// <summary>
        /// The desired excess for the requested policy.
        /// If a null value, then just used the Shield provided default.
        /// </summary>
        public string   Excess         { get; set; }
        /// <summary>
        /// This is a percentage variance from the market value of the
        /// requested vehicle. The percentage is given as a whole integer.
        /// A negative value is a reduction under market value, and a
        /// positive integer is an increase from market value.
        /// E.g.: a value of "10" would mean a 10% increase from market
        /// value.
        ///       a value of "-15" would mean a 15% decrease from
        /// market value.
        /// </summary>
        public int      InsuredVariance { get; set; }
        public bool IsMotorQuoteWithExcessChanges { get; set; }
        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Vehicle quote data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Vehicle:    {Year} {Make} {Model} {Body} {Transmission}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"{nameof(DisclosureDeclineThenDismiss)} = {DisclosureDeclineThenDismiss}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"{nameof(VehicleUsageDeclineThenDismiss)} = {VehicleUsageDeclineThenDismiss}{Reporting.HTML_NEWLINE}");
            for (int i = 0; i < Drivers.Count; i++)
            {
                formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                formattedString = FormatDriverDetails(formattedString, Drivers[i], i + 1);
            }
            return formattedString.ToString();
        }

        private StringBuilder FormatDriverDetails(StringBuilder outputString, Driver driver, int i)
        {
            int age = driver.Details.GetContactAge();
            int countConvictions = driver.LicenseConvictions  != null ? driver.LicenseConvictions.Count : 0;
            int countAccidents   = driver.HistoricalAccidents != null ? driver.HistoricalAccidents.Count : 0;
            var name = driver.Details.GetFullTitleAndName();
            var address = driver.Details.MailingAddress != null ? driver.Details.MailingAddress.StreetSuburbPostcode(expandUnitAddresses: false) : "no address";

            outputString.AppendLine($"    Driver {i}: {name} PH:{driver.IsPolicyHolderDriver}{Reporting.HTML_NEWLINE}");
            outputString.AppendLine($"                {driver.Details.Gender} {age}years (DoB: {driver.Details.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_HYPHENS)}){Reporting.HTML_NEWLINE}");
            outputString.AppendLine($"Driver License Time:{driver.LicenseTime}{Reporting.HTML_NEWLINE}");
            outputString.AppendLine($"                {countAccidents} Accidents {countConvictions} Convictions{Reporting.HTML_NEWLINE}");
            outputString.AppendLine($"                {address}{Reporting.HTML_NEWLINE}");
            outputString.AppendLine($"        Member: {driver.Details.IsRACMember} / {driver.Details.MembershipTier}({driver.Details.MembershipNumber}){Reporting.HTML_NEWLINE}");
            return outputString;
        }
    }

    public class QuoteMotorcycle : Motorcycle
    {
        public MotorcycleUsage UsageType      { get; set; }
        public AnnualKms   AnnualKm { get; set; }
        public Address  ParkingAddress { get; set; }
        public string   CurrentInsurer { get; set; }
        public List<Driver> Drivers    { get; set; }
        public Payment  PayMethod      { get; set; }
        public DateTime StartDate      { get; set; }
        public MotorCovers CoverType { get; set; }
        /// <summary>
        /// The desired excess for the requested policy.
        /// If a null value, then just used the Shield provided default.
        /// </summary>
        public string   Excess         { get; set; }
        /// <summary>
        /// This is a percentage variance from the market value of the
        /// requested vehicle. The percentage is given as a whole integer.
        /// A negative value is a reduction under market value, and a
        /// positive integer is an increase from market value.
        /// E.g.: a value of "10" would mean a 10% increase from market
        /// value.
        ///       a value of "-15" would mean a 15% decrease from
        /// market value.
        /// </summary>
        public int      InsuredVariance { get; set; }
        public decimal PremiumAnnualFromQuotePage { get; set; }
        public decimal PremiumMonthlyFromQuotePage { get; set; }
        public int SumInsuredFromQuotePage { get; set; }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Vehicle quote data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Vehicle:    {Year} {Make} {Model} {EngineCC}{Reporting.HTML_NEWLINE}");
            for (int i = 0; i < Drivers.Count; i++)
            {
                formattedString = FormatDriverDetails(formattedString, Drivers[i], i + 1);
            }
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            return formattedString.ToString();
        }

        private StringBuilder FormatDriverDetails(StringBuilder outputString, Driver driver, int i)
        {
            int age = driver.Details.GetContactAge();
            int countConvictions = driver.LicenseConvictions != null ? driver.LicenseConvictions.Count : 0;
            int countAccidents = driver.HistoricalAccidents != null ? driver.HistoricalAccidents.Count : 0;
            var name = driver.Details.GetFullTitleAndName();

            outputString.AppendLine($"    Driver {i}: {name} PH:{driver.IsPolicyHolderDriver}{Reporting.HTML_NEWLINE}");
            outputString.AppendLine($"                {driver.Details.Gender} {age}years {driver.LicenseTime}{Reporting.HTML_NEWLINE}");
            outputString.AppendLine($"                {countAccidents} Accidents {countConvictions} Convictions{Reporting.HTML_NEWLINE}");
            outputString.AppendLine($"                {driver.Details.MailingAddress.StreetSuburbPostcode(expandUnitAddresses: false)}{Reporting.HTML_NEWLINE}");
            outputString.AppendLine($"        Member: {driver.Details.IsRACMember} / {driver.Details.MembershipTier}({driver.Details.MembershipNumber}){Reporting.HTML_NEWLINE}");
            return outputString;
        }
    }

    public class Driver
    {
        public Contact           Details             { get; set; }
        public string            LicenseTime         { get; set; }
        public List<Declaration> LicenseConvictions  { get; set; }
        public List<Declaration> HistoricalAccidents { get; set; }
        public bool              IsPolicyHolderDriver { get; set; }
        /// Relates to disclosure whether any policyholder has
        /// been previously convicted of theft, drug, fraud, or
        /// criminal damage.
        /// 
        /// If TRUE, Test framework will answer yes to verify
        /// decline pop-up, but then it will answer no to
        /// allow policy purchase to proceeed.
        /// </summary>
        public bool HasPastConvictions { get; set; }
        public List<ClaimHistory> PastClaims { get; set; }
        /// <summary>
    }

    /// <summary>
    /// Used for both traffic conviction declarations, as well
    /// as historical vehicle accident declarations
    /// </summary>
    public class Declaration
    {
        public string Month       { get; set; }
        public string Year        { get; set; }
        public string Description { get; set; }
        public string InsurerAtTime { get; set; }
    }
}
