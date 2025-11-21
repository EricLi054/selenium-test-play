using OpenQA.Selenium;
using System;
using System.Globalization;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.DataFormats;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace Rac.TestAutomation.Common
{
    abstract public class SparkBasePage : BasePage
    {
        #region CONSTANTS  
        
        #endregion

        #region XPATHS
        public class XPaths
        {
            public static readonly string ActiveStepper = "//button[@aria-selected='true']";
            public class Header
            {
                public readonly static string PhoneNumber               = "//a[@id='header-phone-button']";
                public readonly static string ShieldStatusIcon          = "id('shield-status-button')";
                public readonly static string HealthySvg                = "//*[(@class='svg-inline--fa fa-heart ')]";
                public readonly static string HealthyOld                = "//*[(@class='svg-inline--fa fa-shield-heart ')]";
                public readonly static string SpinnerSvg                = "//*[(@class='svg-inline--fa fa-spinner fa-spin ')]";
            }
            public class Banner
            {
                public readonly static string WebAppVersion = "//a[@id='label-npe-banner-app-version-url']";
                public readonly static string BFFAppVersion = "//a[@id='label-npe-banner-bff-version-url']";
            }

            public class Sidebar
            {
                public const string PdsLink             = "id('pdsLink')";
            }
            public class Footer
            {
                public const string PrivacyLink         = "//a[text() = 'Privacy']";
                public const string DisclaimerLink      = "//a[text() = 'Disclaimer']";
                public const string SecurityLink        = "//a[text() = 'Security']";
                public const string AccessibilityLink   = "//a[text() = 'Accessibility']";
            }
            public class DatePicker
            {
                public const string Base = "//div[starts-with(@class,'MuiPickers') and contains(@class,'root')]";
                public class Button
                {
                    public const string PrevMonth = "//button[@title='Previous month']";
                    public const string NextMonth = "//button[@title='Next month']";
                    public const string DesiredDay = "//div[contains(@class,'MuiDayCalendar-monthContainer')]//button[not(contains(@class,'disabled')) and not(contains(@class,'hiddenDay')) and not(contains(@data-testid,'damage-date-disabled')) and not(contains(@class,'dayOutsideMonth')) and (text()=";
                }
                public const string HeaderText = "//div[contains(@class,'MuiPickersCalendarHeader-label') and contains(@id,'grid-label')]";
                public const string OK = "//button[text() = 'OK']";
            }

            public class TimePicker
            {
                public const string ClockIcon = "//*[@data-testid='ClockIcon']";
                public static string SelectHour(string Hour) => $"//ul[@aria-label='Select hours']//li[text()='{Hour}']";
                public static string SelectMinute(string Minute) => $"//ul[@aria-label='Select minutes']//li[text()='{Minute}']";
                public static string SelectMeridiem(string Meridiem) => $"//ul[@aria-label='Select meridiem']//li[text()='{Meridiem}']";
                public class Button
                {
                    public const string OK = "//button[text()='OK']";
                }
            }
            public class TimeField
            {
                public static readonly string TextInput = "id('approximate-time-textinputfield')";
            }
        }
        // Segments
        protected const string BASE = "/html/body/div[@id='root']";
        protected const string FORM = BASE + "//form";

        //Here's your quote page and Payment page
        protected const string XP_PAYMENT_AMOUNT_QUOTE      = "//h3[contains(@id,'label-payment-amount')]";
        protected const string XP_PAYMENT_AMOUNT_PAYMENT    = "//h3[contains(@id,'label-payment-amount')]";
        private const string XP_PAYMENT_FREQUENCY_SELECTED  = "//div[contains(@id,'-payment-frequency')]/button[@aria-pressed='true']";
        protected const string XP_PAYMENT_FREQUENCY_ANNUAL  = "//button[@data-testid='input-toggle-payment-frequency-annual' or @value='Annual']";
        protected const string XP_PAYMENT_FREQUENCY_MONTHLY = "//button[@data-testid='input-toggle-payment-frequency-monthly' or @value='Monthly']";

        #endregion

        public SparkBasePage(Browser browser) : base(browser) { }

        /// <summary>
        /// This is used to identify what triggers the Premium Change:
        /// 1. Policy Start date: Policy start date can make a user transition from one age based rate group to another.
        /// 2. Member Match : This is member match at 'Tell us more about you page' when a member who fails to declare their membership
        /// on the first page (Are you an RAC member) gets matched on the 'Personal Details' page.
        /// </summary>
        public enum PremiumChangeTrigger
        {
            POLICY_START_DATE,
            MEMBER_MATCH
        }

        /// <summary>
        /// Used to denote at which stage of the application flow, we are creating a particular quote version in Shield.
        /// This is used to identify the right quote version to be used for verifying the quote values in Shield.
        /// </summary>
        public enum QuoteStage
        {
            AFTER_QUOTE,
            AFTER_PERSONAL_INFO,
            POLICY_ISSUED
        }

        public bool IsChecked(string elementPath) => GetElement(elementPath).GetAttribute("aria-pressed").Contains("true");

        public string GetElementValue(string elementPath) => GetElement(elementPath).GetAttribute("value");

        /// <summary>
        /// Selects a Date from a Spark Calendar control.
        /// This method will try to click on the calendar icon button, if it's
        /// ruuning on the mobile devices otherwise it will click on calendar field 
        /// From there, it will then set the given date and return.
        /// </summary>
        /// <param name="dateFieldXPath">XPath to the overall date field, usually the editable text field.</param>
        /// <param name="calendarBtnXPath">XPath to the calendar icon button.</param>
        /// <param name="desiredDate">The date to set in the calendar. This method will attempt to navigate to that date and select it.</param>
        /// <exception cref="Exception">If no control is operable, or the date is unable to be set.</exception>
        protected void SelectDateFromCalendar(string dateFieldXPath, string calendarBtnXPath, DateTime desiredDate)
        {
            //Clalendar button not visible for mobile devices
            if (_browser.DeviceName == TargetDevice.GalaxyS21 || _browser.DeviceName == TargetDevice.iPhone14)
            {
                ClickControl(dateFieldXPath);
            }
            else
            {
                ClickControl(calendarBtnXPath);
            }           

            _driver.WaitForElementToBeVisible(By.XPath(XPaths.DatePicker.Base), WaitTimes.T5SEC);

            var desiredMonth = desiredDate.ToString("MMMM yyyy");
            var currentMonthYear = GetElement(XPaths.DatePicker.HeaderText).Text;

            if (!desiredMonth.Equals(currentMonthYear))
            {
                string monthController = desiredDate < DateTime.Now.Date ? XPaths.DatePicker.Button.PrevMonth : XPaths.DatePicker.Button.NextMonth;

                do
                {
                    // XP_CHANGE_MONTH will pick up the active chevron to either take us forward or backward.
                    if (!IsControlEnabled(monthController))
                    {
                        Reporting.Error($"Calendar month controller is disabled and not allowing us to get to desired date {desiredDate.ToString(DateTimeTextFormat.ddMMyyyy)}");
                    }
                    ClickControl(monthController);
                    Thread.Sleep(1000);
                    currentMonthYear = GetElement(XPaths.DatePicker.HeaderText).Text;
                } while (!desiredMonth.Equals(currentMonthYear));
            }

            //Click desired date.           
            ClickControl($"{XPaths.DatePicker.Button.DesiredDay}'{desiredDate.Day}')]");

            // Some calendar control cases will present the `OK` button to
            // dismiss the picker dialog. This has become indeterminate
            // with the introduction of MUI5.
            ClickControl(XPaths.DatePicker.OK, hasFailOver: true);
            
            //For samsung galaxy device we need press the Tab key
            //to move out the focus from date picker field
            if (_browser.DeviceName == TargetDevice.GalaxyS21)
            {
                GetElement(dateFieldXPath).SendKeys(Keys.Tab);
            }
        }

        /// <summary>
        /// Selects a time from the MUI time picker control
        /// <summary>
        public void SelectTimeFromTimePicker(DateTime time)
        {
            string hour = time.ToString("hh");
            string minute = time.ToString("mm");
            string meridiem = time.ToString("tt", CultureInfo.InvariantCulture);

            ClickControl(XPaths.TimePicker.ClockIcon);            
            _driver.TryWaitForElementToBeVisible(By.XPath(XPaths.TimePicker.Button.OK), WaitTimes.T60SEC, out IWebElement OkButton);            
          
            ScrollElementIntoView(XPaths.TimePicker.SelectHour(hour));
            ClickControl(XPaths.TimePicker.SelectHour(hour));
            ClickControl(XPaths.TimePicker.SelectMinute(minute));
            ClickControl(XPaths.TimePicker.SelectMeridiem(meridiem));            
        }

        /// <summary>
        /// Re-usable option to input the time as text (e.g. claims event time).
        /// </summary>
        public void InputTimeAsText(DateTime dateTime)
        {
            var eventTime = dateTime.ToString(TIME_FORMAT_12HR);
            WaitForTextFieldAndEnterText(XPaths.TimeField.TextInput , eventTime, hasTypeAhead: false);
        }

        /// <summary>
        /// For mobile devices it display the analog clock
        /// currently we not able to select a desired time from the clock because of UI restriction        
        /// TO DO:: https://rac-wa.atlassian.net/browse/SPK-5068 is raised to handel it
        /// </summary>
        public void SelectMobileTimeClock()
        {
            ClickControl("id('start-claim-time-input')", skipJSScrollLogic: true);
            _driver.TryWaitForElementToBeVisible(By.XPath("//button[text()='OK']"), WaitTimes.T60SEC, out IWebElement OkButton);

            ClickControl("//div[contains(@class,'MuiClock-squareMask')]");
            Thread.Sleep(1000);
            ClickControl("//div[contains(@class,'MuiClock-squareMask')]");
            Thread.Sleep(1000);

            ClickControl("//button[text()='OK']");
        }

        public PaymentFrequency PaymentFrequency
        {
            get => DataHelper.GetValueFromDescription<PaymentFrequency>(GetValue(XP_PAYMENT_FREQUENCY_SELECTED));
            set => ClickBinaryToggle(null, XP_PAYMENT_FREQUENCY_ANNUAL, XP_PAYMENT_FREQUENCY_MONTHLY, value == PaymentFrequency.Annual);
        }

        public decimal QuoteAmount => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XP_PAYMENT_AMOUNT_QUOTE)));
        
        public decimal PaymentAmount => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XP_PAYMENT_AMOUNT_PAYMENT)));

        public bool IsEligibleForDiscount(MembershipTier membershipTier)
        {
            return (membershipTier == MembershipTier.Gold) || (membershipTier == MembershipTier.Silver) || (membershipTier == MembershipTier.Bronze);
        }

        public void VerifyStandardHeaderAndFooterContent()
        {
                Reporting.AreEqual(Header.Link.RACI_TELEPHONE_NUMBER,
                    GetAttribute(XPaths.Header.PhoneNumber, "href"), "Help telephone number at the top right hand corner");
                Reporting.AreEqual(Footer.Link.PRIVACY_URL,
                    GetAttribute(XPaths.Footer.PrivacyLink, "href"), "NPE Footer Privacy Policy URL");
                Reporting.AreEqual(Footer.Link.DISCLAIMER_URL,
                    GetAttribute(XPaths.Footer.DisclaimerLink, "href"), "NPE Footer Disclaimer URL");
                Reporting.AreEqual(Footer.Link.SECURITY_URL,
                    GetAttribute(XPaths.Footer.SecurityLink, "href"), "NPE Footer Security URL");
                Reporting.AreEqual(Footer.Link.ACCESSIBILITY_URL,
                    GetAttribute(XPaths.Footer.AccessibilityLink, "href"), "NPE Footer Accessibility URL");
        }

        /// <summary>
        /// Specific for DOB field, returning whether the expected error message is present
        /// on a given DOB input box.
        /// </summary>
        public bool IsTextBoxErrorMessagePresentDob(string xpath, string expectedErrorText)
        {
            IWebElement field;
            var hasErrorNotice = _driver.TryFindElement(By.XPath($"{xpath}/../../../p[contains(@class,'Mui-error')]"), out field);
            if (hasErrorNotice)
            {
                Reporting.AreEqual(expectedErrorText, field.Text, $"expected Date of Birth field validation text with actual display");
            }

            return hasErrorNotice;
        }

        /// <summary>
        /// Checks for either extant version of the healthy status indicator.
        /// 
        /// If one is found, return true.
        /// Otherwise return false.
        /// 
        /// TODO: Could be simplified to "return IsControlDisplayed(XPaths.Header.HealthySvg);"
        /// once the older status indicator is replaced (e.g. on Caravan).
        /// </summary>
        public bool CheckNpeStatusIndicator()
        {
            return IsControlDisplayed(XPaths.Header.HealthySvg) || IsControlDisplayed(XPaths.Header.HealthyOld);
        }

        /// <summary>
        /// Checks the Shield Status Icon in the upper-right corner of NPE Spark applications looking
        /// for an indication that Shield is healthy and it's worth proceeding with the test.
        /// 
        /// If after a maximum of 60 seconds it has not seen indication of a healthy test environment 
        /// it will log an error, aborting the test early and logging the reason.
        /// 
        /// If the icon is not found at all, then we skip the check entirely as it is hidden in UAT and 
        /// some Spark applications (e.g. Motorcycle) have not implemented this feature at time of writing.
        /// </summary>
        public void WaitForShieldHealthStatusCheck()
        {
            if (!IsControlDisplayed(XPaths.Header.ShieldStatusIcon))
            {
                Reporting.Log($"Couldn't find Shield/API status indicator so assuming UAT environment " +
                    $"or status indicator not implemented for this application and bypassing check.");
                return;
            }

            var healthyState = false;
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T60SEC);
            do
            {
                healthyState = CheckNpeStatusIndicator();
                Thread.Sleep(SleepTimes.T500MS);
            } while (!healthyState && DateTime.Now < endTime);

            if (!healthyState)
            {
                Reporting.Log($"Healthy state not found, check for spinner SVG = {IsControlDisplayed(XPaths.Header.SpinnerSvg)}");
                Reporting.Error($"Test aborted as healthy Shield/API status indicator was not found after applying config.");
            }
            else
            {
                Reporting.Log($"Healthy Shield/API status indicator was found after applying config, can proceed.");
            }
        }


        /// <summary>
        /// Capture the Web and BFF version in Report
        /// <summary>
        public void LogSparkApplicationVersion()
        {
            Reporting.LogMinorSectionHeading("Spark Application Version");
            if (IsControlDisplayed(XPaths.Banner.WebAppVersion))
            {
                Reporting.Log($"Web Version: {GetInnerText(XPaths.Banner.WebAppVersion)}");
            }
            else
            {
                Reporting.Log($"Couldn't find spark web app version indicator so assuming UAT environment " +
                   $"or status indicator not implemented for this application and bypassing check.");
            }
            if (IsControlDisplayed(XPaths.Banner.BFFAppVersion))
            {
                Reporting.Log($"Web Version: {GetInnerText(XPaths.Banner.BFFAppVersion)}");
            }
            else
            {
                Reporting.Log($"Couldn't find spark bff app version indicator so assuming UAT environment " +
                   $"or status indicator not implemented for this application and bypassing check.");
            }

        }
    }
}
