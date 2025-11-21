using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Rac.TestAutomation.Common
{
    public class PaymentV2
    {
        public PaymentFrequency PaymentFrequency { get; private set; }

        public PaymentOptionsSpark PaymentOption { get; private set; }

        public CreditCard CreditCardDetails { get; private set; }

        public BankAccount BankAccountDetails { get; private set; }

        public PaymentV2()
        {
        }

        public PaymentV2 BankAccount(BankAccount bankAccount)
        {
            BankAccountDetails = bankAccount;
            CreditCardDetails  = null;
            PaymentOption      = PaymentOptionsSpark.DirectDebit;
            return this;
        }

        public PaymentV2 CreditCard(CreditCard creditCard)
        {
            BankAccountDetails = null;
            CreditCardDetails  = creditCard;
            return this;
        }

        public PaymentV2 Monthly()
        {
            PaymentFrequency = PaymentFrequency.Monthly;
            PaymentOption    = PaymentOptionsSpark.DirectDebit;
            return this;
        }

        public PaymentV2 Annual()
        {
            PaymentFrequency = PaymentFrequency.Annual;
            return this;
        }

        public PaymentV2 PaymentTiming(PaymentOptionsSpark paymentOption)
        {
            PaymentOption = paymentOption;
            return this;
        }

        public bool IsAnnual => PaymentFrequency.Equals(PaymentFrequency.Annual);

        public bool IsMonthly => !IsAnnual;

        /// <summary>
        /// Returns number of payments/instalments by payment frequency
        /// </summary>
        public int NumberOfPayments => IsAnnual ? 1 : 12;
    }
}
