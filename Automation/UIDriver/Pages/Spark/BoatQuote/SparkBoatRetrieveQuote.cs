using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace UIDriver.Pages.Spark.BoatQuote
{
    public class SparkBoatRetrieveQuote : SparkBasePage
    {
        #region XPATHS
        private class XPath
        {
            public static readonly string RetrieveQuoteHeader   = "id('header')";
            public static readonly string PleaseNote            = "id('please-note')";
            public class Field
            {
                public static readonly string QuoteNumber   = "id('quoteNumber')";
                public static readonly string Suburb        = "id('suburb')";
                public static readonly string SuburbOptions = "//ul[@role='listbox']" + "//li";
                public class Label
                {
                    public static readonly string QuoteInfo  = "//p[contains(text(),'find this on')]";
                    public static readonly string SuburbInfo = "//p[contains(text(),'where you keep')]";
                }
            }
            public class Button
            {
                public static readonly string RetrieveQuote = "id('btn-retrieve-quote')";
            }
        }
        #endregion

        #region Constants
        private class Constants
        {
            public static readonly string RetrieveQuoteHeader   = "Let's retrieve your quote";
            public static readonly string PleaseNote            = "Please note that your quote may have changed. If there are any updates, " +
                                                                "we'll retrieve your latest quote.";
            public class Field
            {
                public class Label
                {
                    public static readonly string QuoteInfo     = "You'll find this on your quote summary email.";
                    public static readonly string SuburbInfo    = "This is the suburb where you keep your boat.";
                }
            }
            public class Button
            {
                public static readonly string RetrieveQuote = "Retrieve quote";
            }
        }
        #endregion

        #region Settable properties and controls
        public string InputQuoteNumber
        {
            get => GetInnerText(XPath.Field.QuoteNumber);

            set => SendKeyPressesAfterClearingExistingTextInField(XPath.Field.QuoteNumber, value);
        }

        public string InputQuoteRiskSuburb
        {
            get => GetInnerText(XPath.Field.Suburb);

            set => WaitForSelectableAndPickFromDropdown(XPath.Field.Suburb, XPath.Field.SuburbOptions, value);
        }
        #endregion


        public SparkBoatRetrieveQuote(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPaths.Header.PhoneNumber);
                GetElement(XPath.RetrieveQuoteHeader);
            }
            catch
            {
                return false;
            }
            Reporting.LogPageChange("Spark Boat Retrieve Quote page");

            return true;
        }

        /// <summary>
        /// Supports Spark Boat Quotes.
        /// Verify that the RACI telephone number exists in the header 
        /// and that the Footer links are as expected.
        /// </summary>
        public void VerifyHeaderAndFooterContent()
        {
            Reporting.AreEqual(Header.Link.RACI_TELEPHONE_NUMBER, GetElement(XPaths.Header.PhoneNumber).GetAttribute("href"), "Help telephone number at the top right hand corner");
            Reporting.AreEqual(Footer.Link.PRIVACY_URL, GetElement(XPaths.Footer.PrivacyLink).GetAttribute("href"), "NPE Footer Privacy Policy URL");
            Reporting.AreEqual(Footer.Link.DISCLAIMER_URL, GetElement(XPaths.Footer.DisclaimerLink).GetAttribute("href"), "NPE Footer Disclaimer URL");
            Reporting.AreEqual(Footer.Link.SECURITY_URL, GetElement(XPaths.Footer.SecurityLink).GetAttribute("href"), "NPE Footer Security URL");
            Reporting.AreEqual(Footer.Link.ACCESSIBILITY_URL, GetElement(XPaths.Footer.AccessibilityLink).GetAttribute("href"), "NPE Footer Accessibility URL");
        }

        /// <summary>
        /// Supports Spark Boat Quotes.
        /// Verify text and elements found on the Important Information
        /// page.
        /// </summary>
        public void VerifyPageContent()
        {
            VerifyStandardHeaderAndFooterContent();

            Reporting.AreEqual(Constants.RetrieveQuoteHeader, GetInnerText(XPath.RetrieveQuoteHeader), "the expected Page Header against the actual value displayed");

            Reporting.AreEqual(Constants.Field.Label.QuoteInfo, GetInnerText(XPath.Field.Label.QuoteInfo), "the expected information text for the Quote Number field against the actual value displayed");
            Reporting.AreEqual(Constants.Field.Label.SuburbInfo, GetInnerText(XPath.Field.Label.SuburbInfo), "the expected information text for the Suburb field against the actual value displayed");

            Reporting.AreEqual(Constants.Button.RetrieveQuote, GetInnerText(XPath.Button.RetrieveQuote), "the expected text on the button against the actual value displayed");
        }

        /// <summary>
        /// Input the Quote number and select the appropriate Suburb & Postcode input during the last version of a Boat Quote 
        /// to retrieve it for viewing and possible conversion to Policy.
        /// </summary>
        /// <param name="quoteBoat"></param>
        public void InputQuoteDetails(QuoteBoat quoteBoat)
        {
            InputQuoteNumber = quoteBoat.QuoteData.QuoteNumber;
            InputQuoteRiskSuburb = "West Perth - 6005";
            Reporting.Log("Capturing screenshot before selecting 'Retrieve quote' to navigate to 'Here's your quote' page.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.RetrieveQuote);
        }
    }
}
