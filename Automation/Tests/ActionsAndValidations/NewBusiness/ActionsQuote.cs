using Rac.TestAutomation.Common;
using UIDriver.Pages;
using UIDriver.Pages.B2C;
using UIDriver.Pages.Spark;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Tests.ActionsAndValidations
{
    class ActionsQuote
    {
        public static void RetrieveB2CQuote(Browser browser, Address riskAddress, string quoteNumber, ShieldProductType productType)
        {
            OpenB2CRetrieveQuote(browser: browser);

            BaseQuotePage2 quotePage = null;
            switch(productType)
            {
                case ShieldProductType.MGP:
                    quotePage = new MotorQuote2Quote(browser);
                    break;
                case ShieldProductType.HGP:
                    quotePage = new HomeQuote2Quote(browser);
                    break;
                case ShieldProductType.PET:
                    quotePage = new PetQuote2Quote(browser);
                    break;
                default:
                    Reporting.Error($"We haven't implemented retrieve for quotes of type {productType.GetDescription()}");
                    break;
            }

            using (var retrieveQuotePage = new RetrieveQuote(browser))
            using (var spinner = new RACSpinner(browser))
            {
                retrieveQuotePage.RequestQuoteToRetrieve(quoteNumber, riskAddress.SuburbAndCode());
                Reporting.Log($"Submitted Retrieve Quote request for {quoteNumber}", browser.Driver.TakeSnapshot());
                spinner.WaitForSpinnerToFinish(nextPage: quotePage);
                Reporting.AreEqual(quoteNumber, quotePage.QuoteReference, true);
                Reporting.IsTrue(quotePage.IsDisplayed(), $"Quote {quoteNumber} was successfully retrieved");
            }
        }
        private static void OpenB2CRetrieveQuote(Browser browser)
        {
            LaunchPage.OpenB2CLandingPageAndFeatureToggles(browser);
            Reporting.Log($"Opening Retrieve Quote.", browser.Driver.TakeSnapshot());
            using (var launchPage = new LaunchPage(browser))
            {
                launchPage.ClickRetrieveQuote();
            }

            using (var retrieveQuotePage = new RetrieveQuote(browser))
            {
                retrieveQuotePage.WaitForPage();
            }
        }

        public static void RetrieveSparkQuoteFromEmail(Browser browser, Address assetSuburb, string emailAddress, ShieldProductType productType)
        {
            ActionsQuoteCaravan.CloseBrowserAndWait(browser);
            switch (productType)
            {
                case ShieldProductType.MGV:
                    var retrieveQuoteLink = DataHelper.GetRetrieveQuoteLinkFromEmail(emailAddress);
                    LaunchPage.OpenCaravanRetrieveQuoteFromEmail(browser, retrieveQuoteLink);
                    break;
                default:
                    Reporting.Error($"We haven't implemented retrieve for quotes of type {productType.GetDescription()}");
                    break;
            }
            using (var retrieveQuote = new SparkRetrieveQuote(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish();
                    retrieveQuote.RequestQuoteToRetrieve(assetSuburb.SuburbAndCode());                    
                }
            }
        }

        public static void RetrieveSparkQuoteByQuoteNumber(Browser browser, Address assetSuburb, string quoteNumber, ShieldProductType productType)
        {
            ActionsQuoteCaravan.CloseBrowserAndWait(browser);
            switch (productType)
            {
                case ShieldProductType.MGV:
                    LaunchPage.OpenCaravanRetrieveQuoteLandingPage(browser, ShieldProductType.MGV);
                    break;
                default:
                    Reporting.Error($"We haven't implemented retrieve for quotes of type {productType.GetDescription()}");
                    break;
            }
            using (var retrieveQuote = new SparkRetrieveQuote(browser))
            {
                using (var spinner = new SparkSpinner(browser))
                {
                    spinner.WaitForSpinnerToFinish();
                    retrieveQuote.RequestQuoteToRetrieve(assetSuburb.SuburbAndCode(), quoteNumber);                    
                }
            }
        }
    }
}
