using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;

namespace UIDriver.Pages.Spark.Claim.Motor
{
    public class SparkStartYourClaim : BaseMotorClaimPage
    {
        #region CONSTANTS
        protected class Constants
        {
            public static readonly string HeaderText = "Let's start your claim";
            public static readonly string DateOfAccidentLabel = "Date of the incident";            
        }

        #endregion

        #region XPATHS
        protected class XPath
        {
            public static readonly string Header = "id('start-claim-header')";

            public class Field
            {
                public class Label
                {
                    public static readonly string DateOfAccident = "id('label-start-claim-date-input')";                   
                }
                public class Input
                {
                    public static readonly string EventDate = "id('start-claim-date-input')";
                }
            }

            public class Button
            {
                public static readonly string EvenbtDate = "//button[@aria-label = 'Choose date']";
                public static readonly string Next = "id('submit-button')";
            }
        }
    
        public DateTime EventDate
        {
            get => DateTime.ParseExact(GetValue(XPath.Field.Input.EventDate), DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH, System.Globalization.CultureInfo.InvariantCulture);

            set => SelectDateFromCalendar(dateFieldXPath: XPath.Field.Input.EventDate, calendarBtnXPath: XPath.Button.EvenbtDate, desiredDate: value);
        }

        #endregion

        public SparkStartYourClaim(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Button.Next);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Start Your Claim Page");
            Reporting.Log("Start Your Claim Page", _driver.TakeSnapshot());
            return true;
        }

        public void SelectEventDate(DateTime date)
        {
            EventDate = date;
        }

        public void ClickNext()
        {
            Reporting.Log("Start Your Claim - Before clicking next button", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.Next);
        }

        private List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
               "#policy-card-content-policy-details-header-title-policy-0",
               "#policy-card-content-policy-details-header-subtitle-policy-0",
               "#policy-card-content-policy-details-property-0-policy-number-policy-0"
          };
    }
}