using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Globalization;

namespace UIDriver.Pages.Spark.Endorsements
{
    public class ChangePolicyStartDate : SparkBasePage
    {
        #region CONSTANTS
        private class Constants
        {
            public const string PageHeading = "Start date";
            public const string ChooseStartDateChangeLabel= "Choose a start date (AWST) for any changes";

            public class NotificationCard
            {
                public const string Title = "Your start date";
                public const string Content = "You won't be able to make more changes online until after this date.";
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public const string PageHeading = "//h2[text()='" + Constants.PageHeading +"']";
            public const string ChooseStartDateChangeLabel = "//label[text()='" + Constants.ChooseStartDateChangeLabel + "']";
            public const string DatePicker = "id('meo-mid-term-endorsement-start-date')";

            public class NotificationCard
            {
                public const string Title = "id('notification-card-title')";
                public const string Content = "id('notification-card-content')";
            }

            public class Button
            {
                public const string Next = "id('start-date-next-button')";
                public const string Back = "id('pcm-back-link')";
            }
        }
        #endregion

        public ChangePolicyStartDate(Browser browser) : base(browser) { }


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
        /// If "Yes, it is" has been selected the user will be navigated to the "Your car details" step.
        /// If "No, it isn't" has been selected then the user will be diverted to the "Lets update your car" page instead.
        /// </summary>
        public void ClickNext() => ClickControl(XPath.Button.Next);

        /// <summary>
        /// Select Start Date for the endorsement 
        /// </summary>
        public void SelectStartDate(EndorseCar testdata)
        {
            PolicyStartDate = testdata.StartDate;
            Reporting.Log($"Selected the start date as '{testdata.StartDate}'", _browser.Driver.TakeSnapshot());
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
