using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Rac.TestAutomation.Common
{
    public class QuoteBoat : Boat
    {
        public QuoteData QuoteData { get; set; }
        public Address ParkingAddress { get; set; }
        public List<Contact> CandidatePolicyHolders { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public SparkBoatTypeExternalCode BoatTypeExternalCode { get; set; }
        public int BasicExcess { get; set; }
        public string BoatRego { get; set; }
        public string BoatTrailerRego { get; set; }
        public Payment PayMethod { get; set; }
        public override string ToString()
        {
            var paymentMethod = PayMethod.IsPaymentByBankAccount ? "Bank Account" : "Credit Card";
            var paymentFrequency = PayMethod.IsAnnual ? "Annual" : "Monthly";
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Boat quote data:{Reporting.HTML_NEWLINE}");

            formattedString.AppendLine($"    Skippers Ticket            :    {SkippersTicketHeld.GetDescription()}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Claims Count               :    {HistoricBoatClaims}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Boat Type                  :    {BoatTypeExternalCode.GetDescription()}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Boat Type External Code    :    {BoatTypeExternalCode}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Boat Make                  :    {BoatMake.GetDescription()}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Hull Material              :    {SparkBoatHull.GetDescription()}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Year Built                 :    {BoatYearBuilt}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Boat Value                 :    {InsuredAmount}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Basic Excess               :    {BasicExcess}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Financier                  :    {Financier}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    WaterskiingCover           :    {HasWaterSkiingAndFlotationDeviceCover}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    RacingCover (Sail only)    :    {HasRacingCover}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Start Date                 :    {PolicyStartDate.ToString("dd/MM/yyyy")}{Reporting.HTML_NEWLINE}");
            if (ParkingAddress != null)
            {
                formattedString.AppendLine($"    PostCode    :    {ParkingAddress.PostCode}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    IsGaraged   :    {IsGaraged}{Reporting.HTML_NEWLINE}");
                
            }
            formattedString.AppendLine($"   Motor Type                  :    {SparkBoatMotorType.GetDescription()}{Reporting.HTML_NEWLINE}");

            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.Append($"--- Security devices :{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"    Alarm/GPS   :    {SecurityAlarmGps}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    neboLink    :    {SecurityNebo}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    TrailerHitch:    {SecurityHitch}{Reporting.HTML_NEWLINE}");

            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.Append($"--- Registration Numbers :{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"   Boat (empty = unknown)      :    {BoatRego}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Trailer (empty = unknown)   :    {BoatTrailerRego}{Reporting.HTML_NEWLINE}");

            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.Append($"--- Payment Method information :{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"    Payment Method:    {paymentMethod}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Payment Frequency:    {paymentFrequency}{Reporting.HTML_NEWLINE}");
            if (PayMethod.IsPaymentByBankAccount)
            {
                formattedString.AppendLine($"--- Bank Details:{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    BSB:    {PayMethod.Payer.BankAccounts.FirstOrDefault().Bsb}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Account Number      :    {PayMethod.Payer.BankAccounts.FirstOrDefault().AccountNumber}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Account Name        :    {PayMethod.Payer.BankAccounts.FirstOrDefault().AccountName}{Reporting.HTML_NEWLINE}");
            }
            else
            {
                formattedString.AppendLine($"--- Credit Card Details:{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Card Number       :    {PayMethod.Payer.CreditCards.FirstOrDefault().CardNumber}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Expiry Date       :    {PayMethod.Payer.CreditCards.FirstOrDefault().CardExpiryDate}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    CVN Number        :    {PayMethod.Payer.CreditCards.FirstOrDefault().CVNNumber}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Name              :    {PayMethod.Payer.CreditCards.FirstOrDefault().CardholderName}{Reporting.HTML_NEWLINE}");
            }
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.Append($"--- Candidate Policyholder information :{Reporting.HTML_NEWLINE}");
            foreach (var candidatePH in CandidatePolicyHolders)
            {
                formattedString.Append(candidatePH.ToString());
            }
            return formattedString.ToString();
        }
    }

    
}
