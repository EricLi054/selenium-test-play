using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Collections.Generic;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;

namespace UIDriver.Pages.Spark.CaravanQuote
{
    public class TellUsMoreAboutYourCaravan : SparkBasePage
    {
        #region XPATHS
        private class XPath
        {
            public static class General
            {
                public const string Header          = FORM + "//h2[contains(text(),'Tell us more about your')]";
                public const string RoadBlockNotice = "/../../following-sibling::div/div[@data-testid='roadblockInfoCard']";

                public const string Yes = "//button[text()='Yes']";
                public const string No  = "//button[text()='No']";
            }
            public static class BusinessOrCommercial
            {
                public const string Label  = FORM + "//legend[@id='label-isBusinessOrCommercialUsage']";
                public const string Toggle = "id('input-isBusinessOrCommercialUsage')";
                public static class Help
                {
                    public const string TextHeader  = "id('businessOrCommercialUsageTooltip-title')";
                    public const string TextBody    = "id('businessOrCommercialUsageTooltip-message')";
                    public const string OpenButton  = FORM + "//button[@data-testid='businessOrCommercialUsageTooltipButton']";
                    public const string CloseButton = "id('businessOrCommercialUsageTooltip-close')";
                }
            }
            public static class Suburb
            {
                public const string Label = FORM + "//label[@for='input-suburb']";
                public const string Input = "id('input-suburb')";
                public const string Options = "//ul[@id='input-suburb-listbox']/li";
            }
            public static class Overnight
            {
                public const string Label   = FORM + "//label[@for='select-overnight-parking']";
                public const string Input   = "id('select-overnight-parking')";
                public const string Options = "//ul[@role='listbox']/li";
            }
            public static class Button
            {
                public const string Next = FORM + "//button[@data-testid='submit']";
            }
        }

        #endregion

        #region text
        private const string ONSITE_26TH_PARALLEL_KNOCKOUT_TEXT     = "Sorry, we've hit a roadblock\r\nWe're unable to insure on-site caravans in this suburb. If you'd like to discuss this with us, including why we made this decision, you can call us on 13 17 03.\r\n\r\nTo help you find appropriate cover, Find an Insurer (from the Insurance Council of Australia) has a list of insurance providers offering general insurance.";
        private const string BUSINESS_OR_COMMERCIAL_KNOCKOUT_TEXT   = "Sorry, we've hit a roadblock\r\nWe're unable to insure caravans used for business or commercial purposes. If you'd like to discuss this with us, including why we made this decision, you can call us on 13 17 03.\r\n\r\nTo help you find appropriate cover, Find an Insurer (from the Insurance Council of Australia) has a list of insurance providers offering general insurance.";
        private const string HELP_TEXT_HEADER                       = "Business or commercial use";
        private const string HELP_TEXT_BODY                         = "We only cover caravans for personal use such as holidays or permanent accommodation.\r\n\r\nWe don't cover caravans for any business or commercial use such as hiring out, catering, food or parcel delivery, carrying paying passengers or as portable office space.";

        //Label Text
        private const string LABEL_BUSINESS_OR_COMMERCIAL           = "Do you use your caravan for business or commercial purposes?";
        private const string LABEL_PARKED_SUBURB                    = "What suburb is your caravan usually kept in?";
        private const string LABEL_OVERNIGHT_PARKING                = "Where is your caravan usually parked?";
        private const string LABEL_HEADER                           = "Tell us more about your ";
        #endregion


        public TellUsMoreAboutYourCaravan(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.Header);
                GetElement(XPath.BusinessOrCommercial.Toggle);
                GetElement(XPath.Suburb.Input);
                GetElement(XPath.Overnight.Input);
                GetElement(XPath.Button.Next);
                GetElement(XPath.BusinessOrCommercial.Help.OpenButton);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Caravan Quote page - Tell Us More About Your Caravan");
            return true;
        }

        #region Settable properties and controls
        public bool IsForBusinessOrCommercialUse
        {
            get => GetBinaryToggleState(XPath.BusinessOrCommercial.Toggle, XPath.General.Yes, XPath.General.No);
            set => ClickBinaryToggle(XPath.BusinessOrCommercial.Toggle, XPath.General.Yes, XPath.General.No, value);
        }

        public string ParkedSuburb
        {
            get => GetValue(XPath.Suburb.Input);
            set => WaitForSelectableFieldToSearchAndPickFromDropdown(XPath.Suburb.Input, XPath.Suburb.Options, value);
        }

        public string Parking
        {
            get => GetInnerText(XPath.Overnight.Input);
            set => WaitForSelectableAndPickFromDropdown(XPath.Overnight.Input, XPath.Overnight.Options, value);
        }
        #endregion

        /// <summary>
        /// Supports Spark version of caravan B2C
        /// Fills details about the caravan usage, whether it's financed, overnight park location and suburb
        /// </summary>
        /// <param name="quoteCaravan"></param>
        public void FillAdditionalInformationAndVerifyKnockouts(QuoteCaravan quoteCaravan)
        {
            VerifyHelpText();
            VerifyPageLabels(quoteCaravan);

            ParkedSuburb = quoteCaravan.ParkingAddress.SuburbAndCode();

            Parking = quoteCaravan.ParkLocation.GetDescription();

            //Verify 26th Parallel Knockout. Check if knockout text is present
            //after selecting 'ParkedSuburb' and 'ParkLocation'
            var inError = (quoteCaravan.ParkLocation == CaravanParkLocation.OnSite) &&
                           DataHelper.IsPostcodeAbove26thParallel(quoteCaravan.ParkingAddress.PostCode);
            VerifyRelativeErrorDialogText($"{XPath.Overnight.Input}/..",
                                          inError,
                                          ONSITE_26TH_PARALLEL_KNOCKOUT_TEXT,
                                          "26th parallel onsite case");

            // We're doing this last, because answering "business" will lock out other input fields.
            IsForBusinessOrCommercialUse = quoteCaravan.IsForBusinessOrCommercialUse;
            //Verify Business Or Commercial Use Knockout. Check if knockout text is present
            //after selecting 'BusinessOrCommercialUse'.
            VerifyRelativeErrorDialogText(XPath.BusinessOrCommercial.Toggle,
                                          quoteCaravan.IsForBusinessOrCommercialUse,
                                          BUSINESS_OR_COMMERCIAL_KNOCKOUT_TEXT,
                                          "Business/Commercial question");

        }

        private void VerifyRelativeErrorDialogText(string triggeringXPath, bool isKnockoutExpected, string expectedText, string purpose)
        {
            bool knockoutTextFound;
            try
            {
                knockoutTextFound = GetElement($"{triggeringXPath}{XPath.General.RoadBlockNotice}").Displayed;
            }
            catch (NoSuchElementException)
            {
                knockoutTextFound = false;
            }

            if (isKnockoutExpected)
            {
                Reporting.IsTrue(knockoutTextFound, $"Knockout test message was seen for {purpose}.");
                Reporting.AreEqual(expectedText, GetInnerText($"{triggeringXPath}{XPath.General.RoadBlockNotice}"), "knockout text message has correct value");
                Reporting.Log("Knockout text appeared", _browser.Driver.TakeSnapshot());
            }
            else
            {
                Reporting.IsFalse(knockoutTextFound, $"Knockout test message should not have been shown for {purpose}.");
            }
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }

        private void VerifyHelpText()
        {
            ClickControl(XPath.BusinessOrCommercial.Help.OpenButton);
            Reporting.AreEqual(HELP_TEXT_HEADER, GetInnerText(XPath.BusinessOrCommercial.Help.TextHeader), "Tell us More about you: Help Text Header is incorrect it should be: " + XPath.BusinessOrCommercial.Help.TextHeader);
            Reporting.AreEqual(HELP_TEXT_BODY, GetInnerText(XPath.BusinessOrCommercial.Help.TextBody), "Tell us More about you: Help text Body has incorrect text");
            ClickControl(XPath.BusinessOrCommercial.Help.CloseButton);
        }

        private void VerifyPageLabels(QuoteCaravan quoteCaravan)
        {
            //Verify Header with Make
            Reporting.IsTrue(LABEL_HEADER + quoteCaravan.Make == GetElement(XPath.General.Header).Text, $"EXPECTED:{LABEL_HEADER} Actual: {GetElement(XPath.General.Header).Text}");
            //Verify Do you use your caravan for business or commercial purposes? label
            Reporting.AreEqual(LABEL_BUSINESS_OR_COMMERCIAL, GetInnerText(XPath.BusinessOrCommercial.Label), "use of Caravan text");
            //Verify What suburb is your caravan usually kept in? Label 
            Reporting.AreEqual(LABEL_PARKED_SUBURB, GetInnerText(XPath.Suburb.Label), "Suburb your Caravan usually kept in text");
            //Verify Where is your caravan usually parked? Label
            Reporting.AreEqual(LABEL_OVERNIGHT_PARKING, GetInnerText(XPath.Overnight.Label), "where your Caravan usually parked text");
        }

        /// <summary>
        /// The dynamic CSS that needs to be ignored from visual testing.
        /// </summary>
        public List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
              "#header" // Due to embedded caravan manufacturer
          };
    }
}