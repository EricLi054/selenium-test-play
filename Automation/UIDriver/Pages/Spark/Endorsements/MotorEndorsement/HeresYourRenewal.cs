using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace UIDriver.Pages.Spark.Endorsements
{
    public class HeresYourRenewal : BaseYourPremium
    {
        #region CONSTANTS
        private class Constants
        {
            public const string Title = "Here's your renewal";
          
            public class PaymentFrequencySection
            {
                public const string Title = "Your renewal amount";
            }

            public class AdjustHowYouPaySection
            {
                public const string Title = "Adjust the amount you pay";
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public const string Title = "//h2[text()=" + "\"" + Constants.Title + "\"" + "]";

            public class Button
            {
                public const string Yes = "//button[@aria-label='Yes']";
                public const string No = "//button[@aria-label='No']";
                public const string Confirm = "id('your-premium-next-button')";
            }

            public class PaymentFrequencySection
            {
                public const string Title = "//h3[text()='" + Constants.PaymentFrequencySection.Title + "']";
                public const string SavingsInfoText = "//p[@id='motor-renewal-savings-information-text']";

                public const string OptionSelected = "//div[@id='motor-renewal-payment-frequency']/button[@aria-pressed,'true')]";
                public class FrequencyButton
                {
                    public const string Annual = "//button[@id='motor-renewal-payment-frequency-toggle-payment-frequency-annual']";
                    public const string Monthly = "//button[@id='motor-renewal-payment-frequency-toggle-payment-frequency-monthly']";
                }

                public const string RenewalAmount = "//h3[@id='motor-renewal-label-payment-amount']";
                public const string AnnualPremiumMonthlyFrequency = "id('motor-renewal-savings-information-text')";
                public const string PremiumBreakDownLink = "//a[@id='motor-renewal-premium-breakdown']";
                public const string PremiumBreakdownBasic = "//p[@id='motor-renewal-premium-breakdown-basic']";
                public const string PremiumBreakdownGovCharges = "//p[@id='motor-renewal-premium-breakdown-government-charges']";
                public const string PremiumBreakdownGST = "//p[@id='motor-renewal-premium-breakdown-gst']";
            }

            public class AdjustHowYouPaySection
            {
                public const string Title = "//h3[text()='" + Constants.AdjustHowYouPaySection.Title + "']";
                public const string Excess = "id('excess-dropdown')";
                public const string ExcessOptions = "//ul[@role='listbox']//li";
                public const string AgreedValue = "id('agreed-value-input')";
                public const string HireCar = "id('hire-car-after-accident-toggle-button')";
            }
        }
        #endregion

        #region Settable properties and controls
        private Decimal RenewalAmount => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PaymentFrequencySection.RenewalAmount)));
      

        private PaymentFrequency Frequency
        {
            get => DataHelper.GetValueFromDescription<PaymentFrequency>(GetInnerText(XPath.PaymentFrequencySection.OptionSelected));
            set
            {
                switch (value)
                {
                    case PaymentFrequency.Annual:
                        ClickControl(XPath.PaymentFrequencySection.FrequencyButton.Annual);
                        break;
                    case PaymentFrequency.Monthly:
                        ClickControl(XPath.PaymentFrequencySection.FrequencyButton.Monthly);
                        break;
                    case PaymentFrequency.SemiAnnual:
                        throw new ArgumentException("Semi annual payments are not supported for online motor renewal");
                    default:
                        throw new ArgumentException("Select a valid value for choose how to pay");
                }
            }
        }

        private Decimal PremiumBasic => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PaymentFrequencySection.PremiumBreakdownBasic)));
        private Decimal PremiumGovernmentCharges => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PaymentFrequencySection.PremiumBreakdownGovCharges)));
        private Decimal PremiumGST => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PaymentFrequencySection.PremiumBreakdownGST)));
        private Decimal AnnualPremiumMonthyFrequency => decimal.Parse(DataHelper.StripMoneyNotations(DataHelper.GetAnnualPremiumForMonthlyFrquency(GetInnerText(XPath.PaymentFrequencySection.AnnualPremiumMonthlyFrequency))));
        #endregion

        public HeresYourRenewal(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Title);
                Reporting.Log($"Page 1: {GetInnerText(XPath.Title)} is Displayed");
                GetElement(XPath.PaymentFrequencySection.Title);
                GetElement(XP_PAYMENT_FREQUENCY_ANNUAL);
                GetElement(XP_PAYMENT_FREQUENCY_MONTHLY);
                GetElement(XPath.PaymentFrequencySection.PremiumBreakDownLink);
                GetElement(XPath.PaymentFrequencySection.SavingsInfoText);
                GetElement(XPath.PaymentFrequencySection.RenewalAmount);
                Reporting.Log($"Page 1: Payment Frequency Section is Displayed");
                GetElement(XPath.AdjustHowYouPaySection.Title);
                GetElement(XPath.AdjustHowYouPaySection.Excess);
                Reporting.Log($"Page 1: Adjust the amount you pay Section is Displayed");
                GetElement(XPath.Button.Confirm);
                Reporting.Log($"Page 1: Confirm Button is Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Here's your renewal page");
            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        public void ClickConfirm()
        {
            ClickControl(XPath.Button.Confirm);
        }

        /// <summary>
        /// Choose between Annual or Monthly payments
        /// </summary>
        public void UpdateFrequencySelection(EndorseCar endorseCar)
        {
            endorseCar.PremiumChangesAfterEndorsement = new Rac.TestAutomation.Common.API.PremiumDetails();

            Frequency = endorseCar.SparkExpandedPayment.PaymentFrequency;

            ClickControl(XPath.PaymentFrequencySection.PremiumBreakDownLink);
            _driver.WaitForElementToBeVisible(By.XPath(XPath.PaymentFrequencySection.PremiumBreakdownGST), WaitTimes.T5SEC);
            Reporting.Log($"Capturing screenshot of expanded premium breakdown.", _browser.Driver.TakeSnapshot());

            if (endorseCar.SparkExpandedPayment.PaymentFrequency == PaymentFrequency.Monthly)
            {
                endorseCar.PremiumChangesAfterEndorsement.TotalPremiumMonthly = AnnualPremiumMonthyFrequency;
            }

            endorseCar.PremiumChangesAfterEndorsement.Total = RenewalAmount;
            endorseCar.PremiumChangesAfterEndorsement.BaseAmount = PremiumBasic;
            endorseCar.PremiumChangesAfterEndorsement.StampDuty = PremiumGovernmentCharges;
            endorseCar.PremiumChangesAfterEndorsement.Gst = PremiumGST;

            // If this is not defined, then we haven't modified the excess.
            // We buffer it, to make sure the displayed Excess is what is saved
            // at the end of the test.
            if (string.IsNullOrEmpty(endorseCar.Excess))
            {
                endorseCar.Excess = Excess;
            }
        }
    }
}
