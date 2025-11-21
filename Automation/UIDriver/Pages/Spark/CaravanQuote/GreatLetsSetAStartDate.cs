using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace UIDriver.Pages.Spark.CaravanQuote
{
    public class GreatLetsSetAStartDate : SparkBasePage
    {
        #region XPATHS
        private class XPath
        {
            public static class General
            {
                public const string Header = FORM + "//h2[contains(text(),\"Great, let's set a start date.\")]";
            }
            public static class Button
            {
                public const string Next = FORM + "//button[@data-testid='submit']";
            }
            public static class StartDate
            {
                public const string Label      = "//label[@id='label-policyStartDate']";
                public const string Input      = "//input[@id='policyStartDate']";
                public const string ChangeIcon = "//button/*[@data-testid='CalendarIcon']/..";
            }
        }

        private const string POLICY_START_DATE_QUESTION_TEXT  = "When would you like your policy to start?";

        #endregion

        #region Settable properties and controls

        private DateTime PolicyStartDate
        {
            get => DateTime.ParseExact(GetValue(XPath.StartDate.Input), DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH, CultureInfo.InvariantCulture);

            set => SelectDateFromCalendar(dateFieldXPath: null, calendarBtnXPath: XPath.StartDate.ChangeIcon, desiredDate: value);
        }

        #endregion

        public GreatLetsSetAStartDate(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.Header);
                GetElement(XPath.StartDate.Label);
                GetElement(XPath.StartDate.Input);
                GetElement(XPath.Button.Next);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Caravan Quote page - Set A Start Date");
            return true;
        }

        public void VerifyPageContent()
        {
            Reporting.AreEqual(POLICY_START_DATE_QUESTION_TEXT, GetInnerText(XPath.StartDate.Label), "Policy start date question label has correct text");
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Set the policy start date, and click 'Next' button
        /// Handles the premium change popup if it appears
        /// </summary>
        /// <param name="caravan"></param>
        public void SetStartDate(QuoteCaravan quoteData)
        {
            if (quoteData.StartDate.Date != DateTime.Now.Date)
            {
                PolicyStartDate = quoteData.StartDate;
            }
            Reporting.Log("Capturing 'Start date' before selecting Next :", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.Next);

            using (var spinner = new SparkSpinner(_browser))
                spinner.WaitForSpinnerToFinish();

            if (quoteData.StartDate.Date != DateTime.Now.Date)
            {
                var mainPH = quoteData.PolicyHolders[0];

                var ageAtStartDate = quoteData.StartDate.Year - mainPH.DateOfBirth.Year;

                if (mainPH.DateOfBirth.Date > quoteData.StartDate.AddYears(-ageAtStartDate)) ageAtStartDate--;

                if ((mainPH.GetContactAge() < PremiumChangePopup.DRIVER_AGE_FACTOR_RATE_GROUP2_MIN_AGE &&
                     ageAtStartDate >= PremiumChangePopup.DRIVER_AGE_FACTOR_RATE_GROUP2_MIN_AGE) ||  // rating age threshold 1
                    (mainPH.GetContactAge() < PremiumChangePopup.DRIVER_AGE_FACTOR_RATE_GROUP3_MIN_AGE &&
                     ageAtStartDate >= PremiumChangePopup.DRIVER_AGE_FACTOR_RATE_GROUP3_MIN_AGE) ||  // rating age threshold 2
                    (mainPH.GetContactAge() < PremiumChangePopup.DRIVER_AGE_FACTOR_RATE_GROUP4_MIN_AGE &&
                     ageAtStartDate >= PremiumChangePopup.DRIVER_AGE_FACTOR_RATE_GROUP4_MIN_AGE) ||  // rating age threshold 3
                    (mainPH.GetContactAge() < PremiumChangePopup.DRIVER_AGE_FACTOR_RATE_GROUP5_MIN_AGE &&
                     ageAtStartDate >= PremiumChangePopup.DRIVER_AGE_FACTOR_RATE_GROUP5_MIN_AGE) ||  // rating age threshold 4
                    (mainPH.GetContactAge() < PremiumChangePopup.DRIVER_AGE_FACTOR_RATE_GROUP6_MIN_AGE &&
                     ageAtStartDate >= PremiumChangePopup.DRIVER_AGE_FACTOR_RATE_GROUP6_MIN_AGE))    // rating age threshold 5
                {
                    // We triggered a premium change assert the pop ups.
                    using (var premiumChangePopup = new PremiumChangePopup(_browser))
                    {
                        premiumChangePopup.WaitForPremiumChangePopup(PremiumChangeTrigger.POLICY_START_DATE, quoteData);
                        premiumChangePopup.VerifyPopupContent(PremiumChangeTrigger.POLICY_START_DATE, quoteData);
                        premiumChangePopup.VerifyPremiumChange(_browser, quoteData, SparkBasePage.QuoteStage.AFTER_QUOTE);
                    }
                }
            }
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