using Rac.TestAutomation.Common;
using System;
using System.Globalization;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;
using OpenQA.Selenium;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Spark.BoatQuote
{
    public class SparkBoatStartDate : BaseBoatPage
    {
        #region XPATHS
        private class XPath
        {
            public static readonly string PageHeader = "//*[contains(text(),'set a start') and @id='header']";
            public static readonly string ActiveStepper = "//button[@aria-selected='true']";
            public class Button
            {
                public static readonly string NextPage = FORM + "//button[@type='submit']";
                public static readonly string PolicyStartDate = "//button[contains(@aria-label,'Choose date')]";
            }
            public class Field
            {
                public static readonly string PolicyStartDateControl = "id('policyStartDate')";
            }
        }
        #endregion

        #region Constants
        private class Constants
        {
             public static readonly string PageHeader           = "Great, let's set a start date";
             public static readonly string ActiveStepperLabel   = "Start date";
        }
        #endregion

        #region Settable properties and controls

        #endregion
        public SparkBoatStartDate(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                isDisplayed = string.Equals(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            if (isDisplayed)
            { Reporting.LogPageChange("Spark Boat Quote page ? - StartDate"); }

            return isDisplayed;
        }

        public void VerifyPageContent()
        {
            Reporting.AreEqual(Constants.PageHeader,
                GetElement(XPath.PageHeader).Text, "expected page header with actual.");

            Reporting.AreEqual(Sidebar.Link.PdsUrl,
                GetElement(XPaths.Sidebar.PdsLink).GetAttribute("href"), "NPE Sidebar PDS URL");

            VerifyStandardHeaderAndFooterContent();

            VerifyBoatFAQContent();
        }

        public DateTime SetPolicyStartDate
        {
            get => DateTime.ParseExact(GetValue(XPath.Field.PolicyStartDateControl),
                                       DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH,
                                       CultureInfo.InvariantCulture);
            set {
                if (value.Date != DateTime.Today.Date)
                {
                    SelectDateFromCalendar(dateFieldXPath: XPath.Field.PolicyStartDateControl, calendarBtnXPath: XPath.Button.PolicyStartDate, desiredDate: value);
                    Reporting.Log($"Capturing Start Date set", _browser.Driver.TakeSnapshot());
                }
        }
        }
        
        public void ContinueToYourDetails(QuoteBoat quoteBoat)
        {
            ClickControl(XPath.Button.NextPage);

            using (var spinner = new SparkSpinner(_browser))
                spinner.WaitForSpinnerToFinish();

            HandlePremiumChangePopUp(quoteBoat);
        }
        
        public void HandlePremiumChangePopUp(QuoteBoat quoteBoat)
        {
            if (!_driver.TryWaitForElementToBeVisible(By.XPath(PremiumChangePopup.XPath.PremiumChangeTitle), WaitTimes.T5SEC, out IWebElement loggedInUser))
            {
                // If cannot find Premium Change popup title, proceed as usual.
                Reporting.Log("Premium change pop-up not encountered; continuing.");
                return;
            }
            else
            {
                Reporting.Log("Expanding Breakdown accordion");
                ClickControl(PremiumChangePopup.XPath.Button.BreakdownAccordion);

                Reporting.AreEqual(PremiumChangePopup.Constants.PremiumChangeTitle,
                    GetInnerText(PremiumChangePopup.XPath.PremiumChangeTitle), 
                    "expected Title of the premium change dialog against the actual Title");
                Reporting.AreEqual(PremiumChangePopup.Constants.PremiumChangeBody,
                    GetInnerText(PremiumChangePopup.XPath.PremiumChangeBody),
                    "expected Body text of the premium change dialog against the actual Body text");
                if (GetInnerText(PremiumChangePopup.XPath.FrequencyText) == "New monthly price")
                {
                    quoteBoat.QuoteData.MonthlyPremium = decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(PremiumChangePopup.XPath.TotalPrice)));
                    Reporting.Log($"New Monthly Premium = {quoteBoat.QuoteData.MonthlyPremium}");
                    quoteBoat.QuoteData.AnnualPremium = decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(PremiumChangePopup.XPath.TotalAnnualComparison)));
                    Reporting.Log($"New Annual Premium = {quoteBoat.QuoteData.AnnualPremium}");

                    Reporting.Log($"New Basic premium = {decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(PremiumChangePopup.XPath.PremiumBreakdown.Basic)))}");
                    Reporting.Log($"New Government charges = {decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(PremiumChangePopup.XPath.PremiumBreakdown.GovernmentCharges)))}");
                    Reporting.Log($"New GST = {decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(PremiumChangePopup.XPath.PremiumBreakdown.GST)))}");
                }
                else
                {
                    quoteBoat.QuoteData.AnnualPremium = decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(PremiumChangePopup.XPath.TotalPrice)));
                    Reporting.Log($"New Annual Premium = {quoteBoat.QuoteData.AnnualPremium}");

                    quoteBoat.QuoteData.PremiumBreakdownBasic = decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(PremiumChangePopup.XPath.PremiumBreakdown.Basic)));
                    Reporting.Log($"New Basic premium = {quoteBoat.QuoteData.PremiumBreakdownBasic}");

                    quoteBoat.QuoteData.PremiumBreakdownStamp = decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(PremiumChangePopup.XPath.PremiumBreakdown.GovernmentCharges)));
                    Reporting.Log($"New Government charges = {quoteBoat.QuoteData.PremiumBreakdownStamp}");

                    quoteBoat.QuoteData.PremiumBreakdownGST = decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(PremiumChangePopup.XPath.PremiumBreakdown.GST)));
                    Reporting.Log($"New GST = {quoteBoat.QuoteData.PremiumBreakdownGST}");
                }
                Reporting.Log("Capturing premium change pop-up.", _browser.Driver.TakeSnapshot());
                ClickControl(PremiumChangePopup.XPath.Button.ClosePopup);
            }
        }

}
}