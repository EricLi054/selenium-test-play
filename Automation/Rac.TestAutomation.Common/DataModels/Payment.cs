using Rac.TestAutomation.Common.DatabaseCalls;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace Rac.TestAutomation.Common
{
    public class Payment
    {
        public PaymentFrequency PaymentFrequency { get; set; }

        public Contact Payer { get; set; }

        public CreditCard CreditCardDetails => Payer.CreditCards[0];

        /// <summary>
        /// Return the Payment scenario indicative of type
        /// and frequency given in this instance.
        /// </summary>
        public PaymentScenario Scenario
        {
            get
            {
                if (IsPaymentByBankAccount)
                {
                    // Bank account variants
                    return IsAnnual ? PaymentScenario.AnnualBank :
                                      PaymentScenario.MonthlyBank;
                }
                // Credit card variants (inc Annual Cash)
                return IsAnnual ? PaymentScenario.AnnualCash :
                                  PaymentScenario.MonthlyCard;
            }
        }

        /// <summary>
        /// If TRUE, then select bank account/direct debit options
        /// If FALSE, then select card/credit card payment options
        /// </summary>
        public bool IsPaymentByBankAccount { get; set; }

        public Payment(Contact payer = null)
        {
            Payer = payer;
            RandomPaymentFrequency();
            RandomPaymentMethod();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drivers"></param>
        public Payment(List<Driver> drivers)
        {
            if (drivers?.Any() == false)
            {
                Reporting.Error("No drivers provided to determine a payment method.");
            }
            Payer = drivers[Randomiser.Get.Next(drivers.Count)].Details;
            RandomPaymentFrequency();
            RandomPaymentMethod();
        }

        public Payment(List<Contact> contacts)
        {
            if (contacts?.Any() == false)
            {
                Reporting.Error("No contacts provided to determine a payment method.");
            }
            Payer = contacts[Randomiser.Get.Next(contacts.Count)];
            RandomPaymentFrequency();
            RandomPaymentMethod();
        }

        public Payment Annual()
        {
            PaymentFrequency = PaymentFrequency.Annual;
            return this;
        }

        public Payment Monthly()
        {
            PaymentFrequency = PaymentFrequency.Monthly;
            return this;
        }

        public Payment BankAccount()
        {
            IsPaymentByBankAccount = true;
            return this;
        }

        public Payment CreditCard()
        {
            IsPaymentByBankAccount = false;
            return this;
        }

        /// <summary>
        /// Returns true if payment frequency is Annual
        /// </summary>
        /// <param name="paymentFrequency"></param>
        /// <returns></returns>
        public bool IsAnnual => PaymentFrequency.Equals(PaymentFrequency.Annual);

        /// <summary>
        /// Returns true if payment frequency is Monthly
        /// </summary>
        /// <param name="paymentFrequency"></param>
        /// <returns></returns>
        public bool IsMonthly => !IsAnnual;

        /// <summary>
        /// Returns number of payments/instalments by payment frequency
        /// </summary>
        public int NumberOfPayments => IsAnnual ? 1 : 12;

        // TODO: RAI-260 Separate WestpacPaymentDetails to a new class and convert to properties
        public class WestpacPaymentDetails
        {
            public string paymentStatus { get; set; }
            public decimal amount { get; set; }
            public string receiptNumber { get; set; }
            public string responseCode { get; set; }
            public string responseDescription { get; set; }
            public string cardHolder { get; set; }
            public string roadsideProduct { get; set; }
            public string roadsideAmount { get; set; }
            public bool isQuickStreamApi { get; set; }
            public string westpacCustomerRefNumber { get; set; }
            public string westpacHttpStatus { get; set; }
        }

        /// <summary>
        /// Verify WestpacQuickStream details are correct for annual cash payments
        /// </summary>
        /// <typeparam name="T">Test data quote class for relevant product</typeparam>
        /// <param name="cardDetails">Credit card details used in payment</param>
        /// <param name="policyNumber">Issued policy number to lookup payment details in Shield</param>
        /// <param name="expectedPrice">Installment value charged on payment</param>
        /// <param name="expectedReceiptNumber">Receipt number from payment confirmation screen</param>
        /// <param name="isMotorQuoteIncludingRoadside">Only relevant for motor quotes, indicates if RSA was purchased with policy</param>
        public static void VerifyWestpacQuickStreamDetailsAreCorrect<T>(CreditCard cardDetails,
                                                                        string policyNumber,
                                                                        decimal expectedPrice,
                                                                        string expectedReceiptNumber,
                                                                        bool isMotorQuoteIncludingRoadside = false)
        {
            Reporting.Log($"Begin verify WestpacQuickStream details");
            var westpacPaymentDetails = FetchWestpacCreditCardPaymentDetailsOnPolicy(policyNumber);

            Reporting.AreEqual(Status.Processed.GetDescription(), westpacPaymentDetails.paymentStatus, true);
            Reporting.AreEqual(expectedPrice, westpacPaymentDetails.amount, "Amount as expected");
            Reporting.AreEqual(expectedReceiptNumber, westpacPaymentDetails.receiptNumber, "Receipt number as expected");

            /* Westpac test cards are identified on their site: 
             *   https://quickstream.westpac.com.au/docs/general/test-account-numbers/#australian-merchants
             * Based on the last 2 digits, we will either get a "08 - Honour with identification" response
             * or a "00 - Approved" response. (This part of automation only deals with valid test card numbers).
             */
            var lastDigits = int.Parse(cardDetails.CardNumber.Substring(cardDetails.CardNumber.Length - 2, 2));
            var expectedResponseCode        = lastDigits <= 44 ? "08" : "00";
            var expectedResponseDescription = lastDigits <= 44 ? "Honour with identification" : "Approved or completed successfully";
            Reporting.AreEqual(expectedResponseCode, westpacPaymentDetails.responseCode, "Response code as expected");
            Reporting.AreEqual(expectedResponseDescription, westpacPaymentDetails.responseDescription, "Response description as expected");

            Reporting.AreEqual(cardDetails.CardholderName, westpacPaymentDetails.cardHolder, "Card holder as expected");

            if ((typeof(T) == typeof(QuoteCar)) && isMotorQuoteIncludingRoadside)
            {
                Reporting.AreEqual("classic", westpacPaymentDetails.roadsideProduct, "With RSA as expected");
                Reporting.AreEqual(MOTOR_ROADSIDE_AMOUNT, decimal.Parse(westpacPaymentDetails.roadsideAmount), "With RSA amount as expected");
            }
            else
            {
                Reporting.IsNull(westpacPaymentDetails.roadsideAmount, "Without RSA amount as expected");
                Reporting.IsTrue(string.IsNullOrEmpty(westpacPaymentDetails.roadsideProduct), "Without RSA as expected");
            }

            Reporting.IsTrue(westpacPaymentDetails.isQuickStreamApi, "Payment via QuickStream as expected");
            Reporting.AreEqual(policyNumber, westpacPaymentDetails.westpacCustomerRefNumber, "Customer ref number as expected");
            Reporting.AreEqual(WestpacHttpStatusCodes.Created.GetDescription(), westpacPaymentDetails.westpacHttpStatus, "Westpac http status as expected");
        }

        private static WestpacPaymentDetails FetchWestpacCreditCardPaymentDetailsOnPolicy(string policyNum)
        {
            var cardDetails = new WestpacPaymentDetails();

            try
            {
                string query    = ShieldDB.ReadSQLFromFile("Policies\\FetchWestpacQuickStreamDetails.sql");
                var queryPolNum = new Dictionary<string, string>()
                {
                    { "policynumber", policyNum }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);

                    while (reader.Read())
                    {
                        cardDetails.paymentStatus            = reader.GetDbValue(0);
                        cardDetails.amount                   = decimal.Parse(reader.GetDbValue(1));
                        cardDetails.receiptNumber            = reader.GetDbValue(2);
                        cardDetails.responseCode             = reader.GetDbValue(3);
                        cardDetails.responseDescription      = reader.GetDbValue(4);
                        cardDetails.cardHolder               = reader.GetDbValue(5);
                        cardDetails.roadsideProduct          = reader.GetDbValue(6);
                        cardDetails.roadsideAmount           = reader.GetDbValue(7);
                        cardDetails.isQuickStreamApi         = bool.Parse(reader.GetDbValue(8));
                        cardDetails.westpacCustomerRefNumber = reader.GetDbValue(9);
                        cardDetails.westpacHttpStatus        = reader.GetDbValue(10);
                        break;
                    }
                }
            }
            catch (SqlException e)
            {
                Reporting.Error($"SQL error encountered: {e.Message}");
            }

            return cardDetails;
        }

        private void RandomPaymentMethod() => IsPaymentByBankAccount = Randomiser.Get.Next(2) == 0;

        private void RandomPaymentFrequency() => PaymentFrequency
            = DataHelper.GetRandomEnum<PaymentFrequency>(startIndex: (int)PaymentFrequency.Monthly);
    }
}