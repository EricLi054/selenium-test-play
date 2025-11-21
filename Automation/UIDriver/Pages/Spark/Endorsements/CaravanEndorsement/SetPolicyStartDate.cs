using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Globalization;

namespace UIDriver.Pages.Spark.Endorsements
{
    public class SetPolicyStartDate : SparkBasePage
    {
        #region CONSTANTS
        private static class Constants
        {
            public static readonly string PageHeading = "Start date";
            public static readonly string ChooseStartDateChangeLabel= "Choose a start date (AWST) for any changes";

            public static class NotificationCard
            {
                public static readonly string Title = "Your start date";
                public static readonly string Content = "You won't be able to make more changes online until after this date.";
            }
        }
        #endregion

        #region XPATHS
        private static class XPath
        {
            public static readonly string PageHeading = "//h2[text()='" + Constants.PageHeading +"']";
            public static readonly string ChooseStartDateChangeLabel = "//label[text()='" + Constants.ChooseStartDateChangeLabel + "']";
            public static readonly string DatePicker = "id('ceo-midterm-endorsement-start-date')";

            public static class NotificationCard
            {
                public static readonly string Title = "id('midterm-start-date-notification-title')";
                public static readonly string Content = "id('midterm-start-date-notification-content')";
            }

            public static class Button
            {
                public static readonly string Next = "id('midterm-start-date-next-button')";
                public static readonly string Back = "id('pcm-back-link')";
            }
        }
        #endregion

        public SetPolicyStartDate(Browser browser) : base(browser) { }


        private DateTime PolicyStartDate
        {
            get => DateTime.ParseExact(GetValue(XPath.DatePicker), DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH, CultureInfo.InvariantCulture);

            set => SelectDateFromCalendar(dateFieldXPath: null, calendarBtnXPath: XPath.DatePicker, desiredDate: value);
        }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PageHeading);                
                GetElement(XPath.ChooseStartDateChangeLabel);
                Reporting.Log($"Start Date page is Displayed");
                GetElement(XPath.DatePicker);
                GetElement(XPath.Button.Next);
                GetElement(XPath.Button.Back);
                Reporting.Log($"Next and Back buttons are Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Start Date Page");
            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// Select the "Next" button to navigate to the next page in the flow.
        /// If "Yes, it is" has been selected the user will be navigated to the "Your caravan details" step.
        /// If "No, it isn't" has been selected then the user will be diverted to the "Lets update your caravan" page instead.
        /// </summary>
        public void ClickNext() => ClickControl(XPath.Button.Next);

        /// <summary>
        /// Select Start Date for the endorsement 
        /// </summary>
        public void SelectStartDate(DateTime endorsmentStartDate)
        {
            PolicyStartDate = endorsmentStartDate;
            Reporting.Log($"Selected the start date as '{endorsmentStartDate}'", _browser.Driver.TakeSnapshot());
        }

        /// <summary>
        /// Verification of the notification card displayed on Start Date page
        /// </summary>
        public void VerificationOfNotificationCard()
        {
            Reporting.AreEqual(Constants.NotificationCard.Title, GetInnerText(XPath.NotificationCard.Title), "the title of the notification card matches the expected text");
            Reporting.AreEqual(Constants.NotificationCard.Content, GetInnerText(XPath.NotificationCard.Content), "the content of the notification card matches the expected text");
        }
    }
}
