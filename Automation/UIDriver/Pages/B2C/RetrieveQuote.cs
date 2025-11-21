using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.B2C
{
    public class RetrieveQuote : BasePage
    {
        private const string XP_QUOTE_NUMBER_FIELD = "id('PolicyNumber')";
        private const string XP_SUBURB_FIELD       = "id('Suburb_Suburb')";
        private const string XP_CONTINUE_BUTTON    = "id('accordion_0_submit-action')";

        public RetrieveQuote(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_QUOTE_NUMBER_FIELD);
                GetElement(XP_SUBURB_FIELD);
                GetElement(XP_CONTINUE_BUTTON);

                Reporting.LogPageChange("Retrieve quote page");
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        public void RequestQuoteToRetrieve(string quoteNumber, string suburbPostcode)
        {
            WaitForTextFieldAndEnterText(XP_QUOTE_NUMBER_FIELD, quoteNumber);
            WaitForSelectableAndPickByTyping(XP_SUBURB_FIELD, suburbPostcode);

            Reporting.Log($"Retrieving {quoteNumber}", _driver.TakeSnapshot());

            ClickControl(XP_CONTINUE_BUTTON);
        }
    }
}
