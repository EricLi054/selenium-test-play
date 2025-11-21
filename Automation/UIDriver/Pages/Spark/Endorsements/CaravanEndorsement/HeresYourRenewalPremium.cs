using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace UIDriver.Pages.Spark.Endorsements
{
    public class HeresYourRenewalPremium : CaravanBaseYourPremium
    {
        #region CONSTANTS
        public static class Constants
        {
            public static readonly string PageTitle  = "Here's your renewal";
          
            public static class PaymentFrequencySection
            {
                public static readonly string Title  = "Your renewal amount";
            }           
        }
        #endregion

        #region XPATHS
        public static class XPath
        {
            public static readonly string PageTitle                 = "//h2[text()=" + "\"" + Constants.PageTitle + "\"" + "]";

            public static class PaymentFrequencySection
            {
                public static readonly string Title                 = "//h3[text()='" + Constants.PaymentFrequencySection.Title + "']";
                public static readonly string SavingsInfoText       = "//p[@id='caravan-renewal-savings-information-text']";
                public static readonly string OptionSelected        = "//div[@id='caravan-renewal-payment-frequency']/button[@aria-pressed,'true')]";

                public static class FrequencyButton
                {
                    public static readonly string Annual            = "//button[@id='caravan-renewal-payment-frequency-toggle-payment-frequency-annual']";
                    public static readonly string Monthly           = "//button[@id='caravan-renewal-payment-frequency-toggle-payment-frequency-monthly']";
                }

                public static readonly string RenewalAmount                 = "//h3[@id='caravan-renewal-label-payment-amount']";
                public static readonly string AnnualPremiumMonthlyFrequency = "id('caravan-renewal-savings-information-text')";
                public static readonly string PremiumBreakDownLink          = "//a[@id='caravan-renewal-premium-breakdown']";
                public static readonly string PremiumBreakdownBasic         = "//p[@id='caravan-renewal-premium-breakdown-basic']";
                public static readonly string PremiumBreakdownGovCharges    = "//p[@id='caravan-renewal-premium-breakdown-government-charges']";
                public static readonly string PremiumBreakdownGST           = "//p[@id='caravan-renewal-premium-breakdown-gst']";
            }
            public static readonly string NextButton = "id('your-premium-next-button')";
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
                        throw new NotImplementedException($"Invalid value: '{value}'. Semi-annual payments are not supported and so have not been implemented for online renewal");
                    default:
                        throw new NotImplementedException($"Support for the PaymentFrequency '{value}' has not been implemented for online renewal");
                }
            }
        }
       

        private Decimal PremiumBasic => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PaymentFrequencySection.PremiumBreakdownBasic)));
        private Decimal PremiumGovernmentCharges => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PaymentFrequencySection.PremiumBreakdownGovCharges)));
        private Decimal PremiumGST => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PaymentFrequencySection.PremiumBreakdownGST)));
        private Decimal AnnualPremiumMonthyFrequency => decimal.Parse(DataHelper.StripMoneyNotations(DataHelper.GetAnnualPremiumForMonthlyFrquency(GetInnerText(XPath.PaymentFrequencySection.AnnualPremiumMonthlyFrequency))));
        #endregion

        public HeresYourRenewalPremium(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PageTitle);
                Reporting.Log($"Page 1: {GetInnerText(XPath.PageTitle)} is Displayed");
                GetElement(XPath.PaymentFrequencySection.Title);
                GetElement(XP_PAYMENT_FREQUENCY_ANNUAL);
                GetElement(XP_PAYMENT_FREQUENCY_MONTHLY);
                GetElement(XPath.PaymentFrequencySection.PremiumBreakDownLink);
                GetElement(XPath.PaymentFrequencySection.SavingsInfoText);
                GetElement(XPath.PaymentFrequencySection.RenewalAmount);
                Reporting.Log($"Page 1: Payment Frequency Section is Displayed");       
                GetElement(XPath.NextButton);
                Reporting.Log($"Page 1: Next Button is Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Here's your renewal page");
            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        public void ClickNext()
        {
            ClickControl(XPath.NextButton);
        }

        /// <summary>
        /// Choose between Annual or Monthly payments
        /// </summary>
        public void UpdateFrequencySelection(EndorseCaravan endorseCaravan)
        {
            endorseCaravan.PremiumChangesAfterEndorsement = new Rac.TestAutomation.Common.API.PremiumDetails();

            Frequency = endorseCaravan.SparkExpandedPayment.PaymentFrequency;

            ClickControl(XPath.PaymentFrequencySection.PremiumBreakDownLink);
            _driver.WaitForElementToBeVisible(By.XPath(XPath.PaymentFrequencySection.PremiumBreakdownGST), WaitTimes.T5SEC);
            Reporting.Log($"Capturing screenshot of expanded premium breakdown.", _browser.Driver.TakeSnapshot());

            if (endorseCaravan.SparkExpandedPayment.PaymentFrequency == PaymentFrequency.Monthly)
            {
                endorseCaravan.PremiumChangesAfterEndorsement.TotalPremiumMonthly = AnnualPremiumMonthyFrequency;
            }

            endorseCaravan.PremiumChangesAfterEndorsement.Total = RenewalAmount;
            endorseCaravan.PremiumChangesAfterEndorsement.BaseAmount = PremiumBasic;
            endorseCaravan.PremiumChangesAfterEndorsement.StampDuty = PremiumGovernmentCharges;
            endorseCaravan.PremiumChangesAfterEndorsement.Gst = PremiumGST;
        }
    }
}
