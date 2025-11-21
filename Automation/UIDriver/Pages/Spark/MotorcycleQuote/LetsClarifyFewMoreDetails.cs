using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace UIDriver.Pages.Spark.MotorcycleQuote
{
    public class LetsClarifyFewMoreDetails : SparkBasePage
    {
        #region XPATH
        private static class XPath
        {
            public static class General
            {
                public const string Header = "//h2[contains(text(),'clarify a few more details')]";
                public const string Disclosure = "//div[@data-testid='dutyOfDisclosureCardTest']/div";
            }
            public static class SetAStartDate
            {
                public const string Input = "id('policyStartDate')";
                public const string Icon = "//button[contains(@aria-label,'Choose date')]";
            }
            public static class Registration
            {
                public const string Input = "//div[@data-testid='bikeRegistrationInput']/input";
            }
            public static class YesOrNo
            {
                public const string Rego = "//div[@data-testid='isRegoKnownYesNoButton']";
                public const string Accident = "//div[@data-testid='isHadAccidentTest']";
                public const string LicenseSuspension = "//div[@data-testid='isLicenceCancelledTest']";
            }
            public const string MoreInfo = "//div[@data-testid='accidentOrCancelledCardTest']/div";
            public static class Button
            {
                public const string Yes = "//button[@aria-label='Yes']";
                public const string No = "//button[@aria-label='No']";
                public const string Next = "//button[@data-testid='submit']";
            }
        }
        /// <summary>
        /// XP_SELECT_DATE is not intended to be used as is, but to be used with an appended
        /// clarifier for a specific DAY. e.g.: $"{XP_SELECT_DATE}[contains(.,'26')]" which
        /// will look for the first enabled control that has '26' in a attribute (or child's
        /// attribute).
        /// </summary>
        #endregion

        #region Constants
        private const string DUTY_OF_DISCLOSURE_MESSAGE = "Important information\r\nWhen answering our questions you have a duty to answer them honestly, accurately and to the best of your knowledge. The duty applies to you and anyone else insured under the policy. If you answer for another person, we will treat your answers as theirs. Your duty continues until we insure you.\r\n\r\nIf you do not meet the duty your policy may be cancelled, or treated as if it never existed and your claim may be rejected or not paid in full.\r\n\r\nThis insurance is a consumer insurance contract.";
        private const string NEED_MORE_INFO_MESSAGE = "We need some more information\r\nPlease call 13 17 03, quoting your reference no. for assistance to complete your policy.";
        #endregion
        #region Settable properties and controls

        /// <summary>
        /// Returns the DAY of the policy start date only. The setter
        /// assumes that the date is always within a month of the
        /// current date. So if the day is equal to today's day or
        /// higher, then the month is the current month.
        /// If the day is less than today, then the month is next month.
        /// </summary>
        public DateTime PolicyStartDate
        {
            get => DateTime.ParseExact(GetValue(XPath.SetAStartDate.Input), 
                                       DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH_WITHSPACE,
                                       CultureInfo.InvariantCulture);
            set => SelectDateFromCalendar(dateFieldXPath: null, calendarBtnXPath: XPath.SetAStartDate.Icon, desiredDate: value);
        }

        public string BikeRego
        {
            get
            {

                var element = GetElement($"{XPath.YesOrNo.Rego}{ XPath.Button.Yes}");
                return (element.GetAttribute("Pressed").Equals(true)) ? GetValue(XPath.Registration.Input) : null;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    ClickControl($"{XPath.YesOrNo.Rego}{XPath.Button.Yes}");

                    Thread.Sleep(1000);

                    WaitForTextFieldAndEnterText(XPath.Registration.Input, value, false);
                }
                else
                {
                    ClickControl($"{XPath.YesOrNo.Rego}{XPath.Button.No}");
                }
               
            }
        }

        public string DutyOfDisclosure => GetElement(XPath.General.Disclosure).Text;

        public bool HadAccident
        {
            get => GetBinaryToggleState(XPath.YesOrNo.Accident, XPath.Button.Yes, XPath.Button.No);

            set => ClickBinaryToggle(XPath.YesOrNo.Accident, XPath.Button.Yes, XPath.Button.No, value);
        }

        public bool LicenseCancelled
        {
            get => GetBinaryToggleState(XPath.YesOrNo.LicenseSuspension, XPath.Button.Yes, XPath.Button.No);

            set => ClickBinaryToggle(XPath.YesOrNo.LicenseSuspension, XPath.Button.Yes, XPath.Button.No, value);
        }

        public string NeedMoreInformation
        {
            get => GetInnerText(XPath.MoreInfo);
        }


        #endregion

        public LetsClarifyFewMoreDetails(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.Header);
                GetElement(XPath.SetAStartDate.Input);
                GetElement(XPath.YesOrNo.Accident);
                GetElement(XPath.YesOrNo.LicenseSuspension);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Motorcycle Quote page - Lets Clarify A Few More Details");
            return true;
        }

        public void FillClarifyFewDetails(QuoteMotorcycle quoteDetails)
        {

            Reporting.AreEqual(DateTime.Now.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH_WITHSPACE), 
                               PolicyStartDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH_WITHSPACE), $"today's date with policy start date");
            PolicyStartDate = quoteDetails.StartDate;

            BikeRego = quoteDetails.Registration;

            Reporting.AreEqual(DUTY_OF_DISCLOSURE_MESSAGE, DutyOfDisclosure, "the displayed duty of disclosure message is correct");

            HadAccident =  (quoteDetails.Drivers.FirstOrDefault().HistoricalAccidents == null) ? false : true;

            LicenseCancelled = (quoteDetails.Drivers.FirstOrDefault().LicenseConvictions == null) ? false : true;

            if (HadAccident.Equals(true) ^ LicenseCancelled.Equals(true))
            {
                Reporting.AreEqual(NEED_MORE_INFO_MESSAGE, NeedMoreInformation, "the Call To Action to contact RACI via telephone");

                Reporting.AreEqual("true", GetElement(XPath.Button.Next).GetAttribute("Disabled"), 
                "the Next button should be disabled");
            }

            ClickNext();
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }

        /// <summary>
        /// Ignore CSS from visual testing
        /// </summary>
        public List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
              "#policyStartDate"
          };
    }
}
