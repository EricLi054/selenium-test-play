using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Collections.Generic;

namespace UIDriver.Pages.Spark.MotorcycleQuote
{
    public class PaymentDetails : SparkPaymentPage
    {
        #region XPATHS
        private static class XPath
        {
            public static class YourBike
            {
                public const string Description = "//p[@data-testid='bikeModelDescription']";
                public const string SumInsured = "//p[@data-testid='insuredFor']";
                public const string Excess = "//p[@data-testid='excessSelected']";
            }
        }
        #endregion

        #region Settable properties and controls

        public string MotorCycleModelDescription => GetInnerText(XPath.YourBike.Description);

        public string SumInsured => DataHelper.GetMonetaryValueFromString(GetInnerText(XPath.YourBike.SumInsured)).ToString();

        public string Excess => DataHelper.GetMonetaryValueFromString(GetInnerText(XPath.YourBike.Excess)).ToString();

        #endregion

        public PaymentDetails(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPathPayment.Button.Submit);
                GetElement(XP_PAYMENT_FREQUENCY_ANNUAL);
                GetElement(XP_PAYMENT_FREQUENCY_MONTHLY);
                GetElement(XPathPayment.Detail.MethodLabel);
                GetElement(XPathPayment.Button.MethodCard);
                GetElement(XPathPayment.Button.MethodBankAccount);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Motorcycle Quote page - Payment");
            return true;
        }

        public void VerifyPaymentAmounts(QuoteMotorcycle quoteMotorcycle)
        {
            //Verify if Payment Amounts are same as what were displayed on 'Here's your quote' page
            //OR in the 'Your quote has been updated' modal window
            if (quoteMotorcycle.PayMethod.IsAnnual)
            {
                Reporting.Log($"quoteMotorcycle.PayMethod.IsAnnual = {quoteMotorcycle.PayMethod.IsAnnual}", _browser.Driver.TakeSnapshot());
                ClickControl(XP_PAYMENT_FREQUENCY_ANNUAL);
                Reporting.AreEqual(quoteMotorcycle.PremiumAnnualFromQuotePage, decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText($"{XP_PAYMENT_AMOUNT_PAYMENT}"))), "Annual Premium in Payment Details page matches the Annual Premium quoted earlier");
            }
            else
            {
                Reporting.Log($"quoteMotorcycle.PayMethod.IsMonthly = {quoteMotorcycle.PayMethod.IsMonthly}", _browser.Driver.TakeSnapshot());
                ClickControl(XP_PAYMENT_FREQUENCY_MONTHLY);
                Reporting.AreEqual(quoteMotorcycle.PremiumMonthlyFromQuotePage, decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText($"{XP_PAYMENT_AMOUNT_PAYMENT}"))), "Monthly Premium in Payment Details page matches the Monthly Premium quoted earlier");
            }
        }

        /// <summary>
        /// Ignore CSS from visual testing
        /// </summary>
        public List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
              "p[data-testid='bikeModelDescription']", 
              "p[data-testid='insuredFor']",
              "p[data-testid='excessSelected']", 
              "#label-payment-amount",
              "[data-testid='paymentFrequency-savings-information-text']",
              "#bsb",
              "#accountNumber",
              "#accountName",
          };
    }
}
