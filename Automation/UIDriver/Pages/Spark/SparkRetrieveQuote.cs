using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark
{
    public class SparkRetrieveQuote : SparkBasePage
    {
        private static class Headers
        {
            public const string XP_H1 = "//h1[text()='Welcome back']";
            public const string XP_H2 = FORM + "//h2[text()=\"Let's retrieve your quote\"])";
        }      
        private static class Textbox
        {
            public const string XP_QUOTENUMBER  = "id('quoteNumberInput')";
            public const string XP_SUBURB       = "id('input-suburb')";
        }
        private static class Button
        {
            public const string XP_RETRIEVE_QUOTE = "id('btn-retrieve-quote')";
        }
        private static class Labels
        {
            public const string XP_QUOTENUMBER    = "//label[text()='Quote number']";
            public const string XP_SUBURB         = "//label[text()='Suburb']";
        }
        private static class Expected
        {
            public const string RETRIEVE_QUOTE          = "Retrieve quote";
            public const string QUOTE_LABEL             = "Quote number";
            public const string SUBURB_LABEL            = "Suburb";
        }

        private string QuoteNumber
        {
            get => GetValue(Textbox.XP_QUOTENUMBER);
            set => WaitForTextFieldAndEnterText(Textbox.XP_QUOTENUMBER, value);
        }

        private string Suburb
        {
            get => GetValue(Textbox.XP_SUBURB);
            set => WaitForSelectableAndPickByTyping(Textbox.XP_SUBURB, value);
        }

        public SparkRetrieveQuote(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(Headers.XP_H1);
                GetElement(Textbox.XP_QUOTENUMBER);
                GetElement(Textbox.XP_SUBURB);
                GetElement(Button.XP_RETRIEVE_QUOTE);
                isDisplayed = true;
                Reporting.LogPageChange("Retrieve quote page");
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }

            return isDisplayed;
        }

        public void RequestQuoteToRetrieve(string suburbPostcode, string quoteNumber = null)
        {
            if (quoteNumber == null && string.IsNullOrEmpty(QuoteNumber))
            {
                Reporting.Log($"we didn't see quote number populate, but we expected it to, with url: {_browser.Driver.Url}. Forcing page reload.", _driver.TakeSnapshot());
                _browser.Driver.Navigate().Refresh();

                WaitForPage();
            }

            ValidateLabels();

            if (quoteNumber != null)
            { QuoteNumber = quoteNumber; }

            Reporting.IsTrue(!string.IsNullOrEmpty(QuoteNumber), "that the quote number field is populated when attempting to retrieve a quote");
            Suburb = suburbPostcode;
            Reporting.Log($"Retrieving {QuoteNumber}", _driver.TakeSnapshot());

            ClickControl(Button.XP_RETRIEVE_QUOTE);
        }

        private void ValidateLabels()
        {
            Reporting.IsTrue(IsControlDisplayed(Labels.XP_QUOTENUMBER), $"RETRIEVE QUOTE: EXPECTED: {Expected.QUOTE_LABEL}");
            Reporting.IsTrue(IsControlDisplayed(Labels.XP_SUBURB), $"RETRIEVE QUOTE: EXPECTED: {Expected.SUBURB_LABEL}");
            Reporting.AreEqual(Expected.RETRIEVE_QUOTE, GetInnerText(Button.XP_RETRIEVE_QUOTE), "RETRIEVE QUOTE");
        }
    }
}
