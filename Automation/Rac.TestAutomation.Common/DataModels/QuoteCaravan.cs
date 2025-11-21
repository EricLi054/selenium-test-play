using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Rac.TestAutomation.Common
{
    public class QuoteCaravan : Caravan
    {
        public Address ParkingAddress { get; set; }
        public List<Contact> PolicyHolders { get; set; }
        public Payment PayMethod { get; set; }
        public DateTime StartDate { get; set; }
        public bool CoverMyAnnexe { get; set; }
        /// <summary>
        /// The desired excess for the requested policy.
        /// If a null value, then just used the Shield provided default.
        /// </summary>
        public int? Excess { get; set; }
        public bool IsForBusinessOrCommercialUse { get; set; }
        public int SumInsuredValue { get; set; }
        //Positive/Negative % increase/decrease from the Market Value
        //This is used to calculate the user defined Caravan Sum Insured value.
        public int InsuredVariance { get; set; }
        public int ContentsSumInsured { get; set; }

        //Quote details from the 'Here's your quote' page
        public QuoteData QuoteData { get; set; }

        public RetrieveQuoteType? RetrieveQuote { get; set; }

        /// <summary>
        /// Supports Spark version of caravan
        /// When there are two policyholders in a quote, we use the highest tier out of the two,
        /// to determine the quote discount regardless of who the main policyholder is.
        /// </summary>
        /// <param name="quoteCaravan"></param>
        /// <returns></returns>
        public MembershipTier GetHighestTier()
        {
            var highestTier = MembershipTier.None;
            foreach (var ph in PolicyHolders)
            {
                if ((int)ph.MembershipTier > (int)highestTier)
                    highestTier = ph.MembershipTier;
            }
            return highestTier;
        }

        public override string ToString()
        {
            var type = Type == CaravanType.Caravan ? CaravanType.Caravan : CaravanType.Trailer;
            var paymentMethod = PayMethod.IsPaymentByBankAccount ? "Bank Account" : "Credit Card";
            var paymentFrequency = PayMethod.IsAnnual ? "Annual" : "Monthly";
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Caravan quote data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Caravan Type:    {type}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Make        :    {Make}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Year        :    {Year}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Model       :    {Model}{Reporting.HTML_NEWLINE}");
            if (ParkingAddress != null)
                formattedString.AppendLine($"    PostCode    :    {ParkingAddress.PostCode}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    ParkLocation:    {ParkLocation}{Reporting.HTML_NEWLINE}");
            formattedString.Append($"--- Payment Details  :{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"    Payment Method:    {paymentMethod}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Payment Frequency:    {paymentFrequency}{Reporting.HTML_NEWLINE}");
            if (PayMethod.IsPaymentByBankAccount)
            {
                formattedString.AppendLine($"--- Bank Details:{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    BSB:    {PayMethod.Payer.BankAccounts.FirstOrDefault().Bsb}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Account Number        :    {PayMethod.Payer.BankAccounts.FirstOrDefault().AccountNumber}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Account Name        :    {PayMethod.Payer.BankAccounts.FirstOrDefault().AccountName}{Reporting.HTML_NEWLINE}");
            }
            else
            {
                formattedString.AppendLine($"--- Credit Card Details:{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Card Number:    {PayMethod.Payer.CreditCards.FirstOrDefault().CardNumber}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Expiry Date       :    {PayMethod.Payer.CreditCards.FirstOrDefault().CardExpiryDate}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    CVN Number        :    {PayMethod.Payer.CreditCards.FirstOrDefault().CVNNumber}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Name        :    {PayMethod.Payer.CreditCards.FirstOrDefault().CardholderName}{Reporting.HTML_NEWLINE}");
            }
            formattedString.Append($"--- Policy holders  :{Reporting.HTML_NEWLINE}");
            foreach (var policyholder in PolicyHolders)
            {
                formattedString.Append(policyholder.ToString());
            }
            return formattedString.ToString();
        }
    }
}
