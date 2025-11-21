using Rac.TestAutomation.Common;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.B2C
{
    abstract public class BaseQuotePage2 : BasePage
    {
        #region XPATHS
        // XP key references:
        protected const string BASE       = "/html/body/div[@id='wrapper']";
        protected const string FINE_PRINT = BASE + "//form//div[@id='fine-print-section']";

        protected const string XP_PAGE_HEADING    = BASE + "//span[@class='action-heading']/span";
        protected const string XP_QUOTE_REFERENCE = XP_PAGE_HEADING + "/span[@id='quote-number']";

        protected const string XPR_ANNUAL_PRICE    = "//div[@class='annual-premium-container']//span[contains(@class,'data-price-amount')]";
        protected const string XPR_MONTHLY_PRICE   = "//td[contains(@class,'data-monthly-amount')]";
        protected const string XPR_BUY_ONLINE_BTN  = "//button[contains(@class,'buy-button')]";
        protected const string XPR_START_DATE      = "//input[contains(@id,'__CoverStartDate')]";

        protected const string CALENDAR_BASE               = "/html/body/div[@class='k-animation-container']/div[contains(@id,'CoverStartDate_dateview')]";
        protected const string XP_CAL_CUR_MONTH            = CALENDAR_BASE + "//div[@class='k-header']/a[2]";
        protected const string XP_MONTH_UP                 = CALENDAR_BASE + "//div[@class='k-header']/a[3]/span";
        protected const string XPM_CAL_DAYS                = CALENDAR_BASE + "//table/tbody//a[@class='k-link']";

        // Email quote prompt
        protected const string XP_EMAIL_QUOTE_BUTTON        = "id('email-quote')/span";
        protected const string XP_EMAIL_POPUP_HEADER        = "id('email-dialog_wnd_title')";
        protected const string XP_EMAIL_POPUP_ADDRESS_FIELD = "id('EmailAddress')";
        protected const string XP_EMAIL_POPUP_SEND_BUTTON   = "id('SubmitAction')";
        #endregion

        #region Settable properties and controls
        public string QuoteReference
        {
            get => GetInnerText(XP_QUOTE_REFERENCE);
        }

        public decimal QuotePriceAnnual
        {
            get => decimal.Parse(GetInnerText($"{GetActiveTabXPath()}{XPR_ANNUAL_PRICE}").StripMoneyNotations());
        }

        public decimal QuotePriceMonthly
        {
            get => decimal.Parse(GetInnerText($"{GetActiveTabXPath()}{XPR_MONTHLY_PRICE}").StripMoneyNotations());
        }

        public DateTime QuoteStartDate
        {
            get
            {
                var dateJsonString = GetAttribute($"{GetActiveTabXPath()}{XPR_START_DATE}", "data-currentvalue");
                var dateValue = JsonConvert.DeserializeObject<DateNumeric>(dateJsonString);
                return dateValue.ToDateTime();
            }
            set
            {
                ClickControl($"{GetActiveTabXPath()}{XPR_START_DATE}");

                var endtime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
                do
                {
                    var cal = GetElement(CALENDAR_BASE);
                    if (cal.Displayed) break;
                    Thread.Sleep(500);
                } while (DateTime.Now < endtime);
                CalendarPickMonthYear(value);
                CalendarPickDay(value);
            }
        }
        #endregion

        public BaseQuotePage2(Browser browser) : base(browser) { }

        public void ClickBuyOnlineButton() => ClickControl($"{GetActiveTabXPath()}{XPR_BUY_ONLINE_BTN}");

        /// <summary>
        /// Handles the flow of requesting an emailed copy of the quote.
        /// Where we expect the email to have already been provided, then
        /// the prepopulation of the email address is asserted.
        /// </summary>
        /// <param name="email">email address for quote</param>
        /// <param name="isPrepopulated">flag to indicate whether the email address should be pre-populated</param>
        public void SendQuoteByEmail(string email, bool isPrepopulated = false)
        {
            ClickControl(XP_EMAIL_QUOTE_BUTTON);

            // This dialog can take an excessive time to display.
            _driver.WaitForElementToBeVisible(By.XPath(XP_EMAIL_POPUP_HEADER), WaitTimes.T90SEC);

            // We only assert that an email value is here. We don't check it matches our record as
            // test data may have a modified email to support Mailosaur or email whitelisting rules.
            if (isPrepopulated)
            { Reporting.IsFalse(string.IsNullOrEmpty(GetValue(XP_EMAIL_POPUP_ADDRESS_FIELD)), "Email field is already populated with member's email."); }
            
            WaitForTextFieldAndEnterText(XP_EMAIL_POPUP_ADDRESS_FIELD, email);
            Reporting.Log($"Submitting request for Quote Email to {email}, see following screenshot.", _browser.Driver.TakeSnapshot());
            ClickControl(XP_EMAIL_POPUP_SEND_BUTTON);
        }

        abstract public string GetActiveTabXPath();

        protected void CalendarPickMonthYear(DateTime date)
        {
            var desiredMonYear = date.ToString("MMMM yyyy");
            var found = false;
            bool isAtMaximumMonth = false;

            do
            {
                try
                {
                    var currentMonth = GetInnerText(XP_CAL_CUR_MONTH);
                    if (currentMonth == desiredMonYear)
                    {
                        found = true;
                        break;
                    }
                    ClickControl(XP_MONTH_UP, skipJSScrollLogic: true);
                    Thread.Sleep(SleepTimes.T2SEC);
                }
                catch (NoSuchElementException)
                {
                    isAtMaximumMonth=true;
                }
            } while (!isAtMaximumMonth);
            if (!found)
            { Reporting.Error("Could not drive calendar to desired date of " + desiredMonYear + ". See " + _driver.TakeSnapshot()); }
        }

        protected void CalendarPickDay(DateTime date)
        {
            var desiredDate = date.ToString(DataFormats.DATE_CALENDAR_PICKER_B2C); ;
            var dayOptions = _driver.FindElements(By.XPath(XPM_CAL_DAYS));

            var result = dayOptions.FirstOrDefault(x => desiredDate.Equals(x.GetAttribute("title")));

            if (result == null)
            { Reporting.Error($"Could not find calendar selectable for {desiredDate}. See {_driver.TakeSnapshot()}"); }
            else
            { result.Click(); }
        }
    }
}
