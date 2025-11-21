using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common.DataModels
{
    public class RefundDetails
    {
        public string RefundID { get; set; }
        public DateTime Dob { get; set; }
        public string LastName { get; set; }
        public string RefundAmount { get; set; }
        public BankAccount RefundBankAmount { get; set; }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();

            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Refund data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Refund ID:  {RefundID}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   What is your date of birth?:  {Dob}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Last name:  {LastName}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Bank:  {RefundBankAmount.Bsb}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Bank Branch State:  {RefundBankAmount.BankBranchState}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Account Number:  {RefundBankAmount.AccountNumber}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Account Name:  {RefundBankAmount.AccountName}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Refund Amount:  {RefundAmount}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);

            return formattedString.ToString();
        }
    }
}
