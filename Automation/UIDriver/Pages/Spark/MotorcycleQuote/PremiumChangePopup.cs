using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;

namespace UIDriver.Pages.Spark.MotorcycleQuote
{
    public class PremiumChangePopup : SparkBasePage
    {
        #region Constants
        private static class XPath
        {
            public static class PremiumChange
            {
                public const string Popup = "//div[@role='dialog' and @aria-labelledby='premium-changed-title']";
                public const string Title = Popup + "//h2[@id='premium-changed-title']/h1";
                public const string Message = Popup + "//p[@id='premium-changed-content-text']";
                public static class QuoteChange
                {
                    public const string Annual = "id('premium-changed-annual-cost')";
                    public const string Monthly = "//span[text()='New monthly price']/../../div[2]/span";
                }
                public static class Button
                {
                    public const string Close = Popup + "//button[@id='button-premium-changed-next']";
                }
            }
        }

        private static class Expected
        {
            public const string QUOTE_UPDATE_TITLE                             = "Your quote has been updated";
            public const string QUOTE_UPDATE_MESSAGE                           = "We've updated your quote based on the information you changed.";
        }
        #endregion

        #region Settable properties and controls
        public decimal PopUpChangedPremiumAnnually => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PremiumChange.QuoteChange.Annual)));

        public decimal PopUpChangedPremiumMonthly => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PremiumChange.QuoteChange.Monthly)));

        #endregion

        public PremiumChangePopup(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PremiumChange.Popup);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }

        public void VerifyPopupContent(QuoteMotorcycle quoteData)
        {
            Reporting.AreEqual(Expected.QUOTE_UPDATE_TITLE, GetInnerText(XPath.PremiumChange.Title));
            Reporting.AreEqual(Expected.QUOTE_UPDATE_MESSAGE, GetInnerText(XPath.PremiumChange.Message));
        }

        public void VerifyPremiumChange(Browser browser, QuoteMotorcycle quoteMotorcycle, SparkBasePage.QuoteStage quoteStage)
        {
            Reporting.Log($"Change premium pop up found. New premiums are: ${PopUpChangedPremiumAnnually} annual, and ${PopUpChangedPremiumMonthly} monthly.", browser.Driver.TakeSnapshot());
            quoteMotorcycle.PremiumAnnualFromQuotePage = PopUpChangedPremiumAnnually;
            quoteMotorcycle.PremiumMonthlyFromQuotePage = PopUpChangedPremiumMonthly;

            ClickControl(XPath.PremiumChange.Button.Close);
        }
    }
}