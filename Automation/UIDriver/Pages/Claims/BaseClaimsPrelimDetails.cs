using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Claims
{
    /// <summary>
    /// Some of the prelim question fields are common between Motor and Home,
    /// so we've put them here to share.
    /// </summary>
    public class BaseClaimsPrelimDetails : BasePage
    {
        protected class Constants
        {
            public const string OverrideWarning = "You are about to replace your existing contact information.";
        }

        protected class BaseXPath
        {
            public const string Heading = "//span[@class='action-heading']";
            public class Toggle
            {
                public const string KnowPolicyNumberYN = "id('PrelimDetails_PolicyDetails_PolicyNumberKnown')";
                public const string PoliceReportLodgedYN = "id('PrelimDetails_ContactDetails_WasPoliceInvolved')";

                public const string Yes = "//span[text()='Yes']/..";
                public const string No  = "//span[text()='No']/..";
            }
            public class Inputs
            {
                public const string FindByPolicyNumber = "id('PrelimDetails_PolicyDetails_PolicyNumber')";
                public const string FindBySurname = "id('PrelimDetails_PolicyDetails_FindMyPolicySurname')";
                public const string ConfirmSurname = "id('PrelimDetails_ContactDetails_Surname')";
                public const string Email = "id('PrelimDetails_ContactDetails_EmailAddress')";
                public const string Phone = "id('PrelimDetails_ContactDetails_TelephoneNumber_PhoneNumber')";
                public const string PoliceReportNumber = "id('PrelimDetails_ContactDetails_PoliceReportNumber')";
            }
            public class Date
            {
                public class FindByBirth
                {
                    public const string Day   = "id('PrelimDetails_PolicyDetails_FindMyPolicyDateOfBirth_Day')";
                    public const string Month = "id('PrelimDetails_PolicyDetails_FindMyPolicyDateOfBirth_Month')";
                    public const string Year  = "id('PrelimDetails_PolicyDetails_FindMyPolicyDateOfBirth_Year')";
                }
                public class Event
                {
                    public const string Day   = "id('PrelimDetails_PolicyDetails_DateOfIncident_Day')";
                    public const string Month = "id('PrelimDetails_PolicyDetails_DateOfIncident_Month')";
                    public const string Year  = "id('PrelimDetails_PolicyDetails_DateOfIncident_Year')";
                    public class Hours
                    {
                        public const string Dropdown = "//span[@aria-owns='PrelimDetails_PolicyDetails_TimeOfIncident_Hour_listbox']";
                        public const string Options  = "//ul[@id='PrelimDetails_PolicyDetails_TimeOfIncident_Hour_listbox' and @aria-hidden='false']/li";
                    }
                    public class Minutes
                    {
                        public const string Dropdown = "//span[@aria-owns='PrelimDetails_PolicyDetails_TimeOfIncident_Minute_listbox']";
                        public const string Options  = "//ul[@id='PrelimDetails_PolicyDetails_TimeOfIncident_Minute_listbox' and @aria-hidden='false']/li";
                    }
                    public class Meridiem
                    {
                        public const string Dropdown = "//span[@aria-owns='PrelimDetails_PolicyDetails_TimeOfIncident_Meridiem_listbox']";
                        public const string Options  = "//ul[@id='PrelimDetails_PolicyDetails_TimeOfIncident_Meridiem_listbox' and @aria-hidden='false']/li";
                    }
                }
                public class ConfirmBirth
                {
                    public const string Day   = "id('PrelimDetails_ContactDetails_DateOfBirth_Day')";
                    public const string Month = "id('PrelimDetails_ContactDetails_DateOfBirth_Month')";
                    public const string Year  = "id('PrelimDetails_ContactDetails_DateOfBirth_Year')";
                }
                public class PoliceReport
                {
                    public const string Day   = "id('PrelimDetails_ContactDetails_PoliceReportDate_Day')";
                    public const string Month = "id('PrelimDetails_ContactDetails_PoliceReportDate_Month')";
                    public const string Year  = "id('PrelimDetails_ContactDetails_PoliceReportDate_Year')";
                }
            }
            public class Button
            {
                public const string ContinueFirstAccordion = "id('accordion_0_submit-action')";
                public const string ContinueSecondAccordion = "id('accordion_1_submit-action')";
            }
            public class DamageType
            {
                public const string Dropdown = "//span[@aria-owns='PrelimDetails_ContactDetails_DamageType_listbox']";
                public const string Options = "id('PrelimDetails_ContactDetails_DamageType_listbox')/li";

            }
            public class Dialog
            {
                public const string Frame = "//div[@id='simple-dialog']/..";
                public class Text
                {
                    public const string Body = Frame + "//div[@class='cluetip-inner']";
                }
                public class Button
                {
                    public const string Yes = Frame + "//span[.='Yes']";
                    public const string No = Frame + "//span[.='No']";
                }
            }
        }

        #region Settable properties and controls
        public bool KnowsPolicyNumber
        {
            get => GetBinaryToggleState(BaseXPath.Toggle.KnowPolicyNumberYN, BaseXPath.Toggle.Yes, BaseXPath.Toggle.No);
            set => ClickBinaryToggle(BaseXPath.Toggle.KnowPolicyNumberYN, BaseXPath.Toggle.Yes, BaseXPath.Toggle.No, value);
        }

        public string PolicyNumber
        {
            get => throw new NotImplementedException("Need to implement means to navigate shadow DOM to get this value.");
            set => WaitForTextFieldAndEnterText(BaseXPath.Inputs.FindByPolicyNumber, value, false);
        }

        public DateTime EventDate
        {
            get => throw new NotImplementedException("TODO: Implement getter for event date.");
            set
            {
                WaitForTextFieldAndEnterText(BaseXPath.Date.Event.Day, value.ToString("dd"), false);
                WaitForTextFieldAndEnterText(BaseXPath.Date.Event.Month, value.ToString("MM"), false);
                WaitForTextFieldAndEnterText(BaseXPath.Date.Event.Year, value.ToString("yyyy"), false);
            }
        }

        public DateTime EventTime
        {
            get => throw new NotImplementedException("TODO: Implement getter for event time.");
            set
            {
                var meridiem = value.Hour >= 12 ? "PM" : "AM";

                WaitForSelectableAndPickFromDropdown(BaseXPath.Date.Event.Hours.Dropdown, BaseXPath.Date.Event.Hours.Options, value.ToString("hh"));
                WaitForSelectableAndPickFromDropdown(BaseXPath.Date.Event.Minutes.Dropdown, BaseXPath.Date.Event.Minutes.Options, value.ToString("mm"));
                WaitForSelectableAndPickFromDropdown(BaseXPath.Date.Event.Meridiem.Dropdown, BaseXPath.Date.Event.Meridiem.Options, meridiem);
            }
        }

        public string FindPolicySurname
        {
            get => throw new NotImplementedException("Need to implement means to navigate shadow DOM to get this value.");
            set => WaitForTextFieldAndEnterText(BaseXPath.Inputs.FindBySurname, value, false);
        }

        public DateTime FindPolicyDateOfBirth
        {
            get => throw new NotImplementedException("Need to implement means to navigate shadow DOM to get this value.");
            set
            {
                WaitForTextFieldAndEnterText(BaseXPath.Date.FindByBirth.Day,   value.Day.ToString(),   false);
                WaitForTextFieldAndEnterText(BaseXPath.Date.FindByBirth.Month, value.Month.ToString(), false);
                WaitForTextFieldAndEnterText(BaseXPath.Date.FindByBirth.Year,  value.Year.ToString(),  false);
            }
        }

        public string ConfirmSurname
        {
            get => throw new NotImplementedException("Need to implement means to navigate shadow DOM to get this value.");
            set => WaitForTextFieldAndEnterText(BaseXPath.Inputs.ConfirmSurname, value, false);
        }

        public DateTime ConfirmDateOfBirth
        {
            get => throw new NotImplementedException("Need to implement means to navigate shadow DOM to get this value.");
            set
            {
                WaitForTextFieldAndEnterText(BaseXPath.Date.ConfirmBirth.Day, value.Day.ToString(), false);
                WaitForTextFieldAndEnterText(BaseXPath.Date.ConfirmBirth.Month, value.Month.ToString(), false);
                WaitForTextFieldAndEnterText(BaseXPath.Date.ConfirmBirth.Year, value.Year.ToString(), false);
            }
        }

        public string Email
        {
            get => throw new NotImplementedException("Need to implement means to navigate shadow DOM to get this value.");
            set => WaitForTextFieldAndEnterText(BaseXPath.Inputs.Email, value, false);
        }

        public string PhoneNumber
        {
            get => throw new NotImplementedException("Need to implement means to navigate shadow DOM to get this value.");
            set => WaitForTextFieldAndEnterText(BaseXPath.Inputs.Phone, value, false);
        }
        #endregion

        public BaseClaimsPrelimDetails(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(BaseXPath.Date.Event.Day);
                GetElement(BaseXPath.Date.Event.Hours.Dropdown);
                GetElement(BaseXPath.Button.ContinueFirstAccordion);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Claim preliminary details page");
            return true;
        }

        public void SetPoliceReportDetails(string reportNumber, DateTime reportDate)
        {
            var havePoliceReport = !string.IsNullOrEmpty(reportNumber);
            ClickBinaryToggle(BaseXPath.Toggle.PoliceReportLodgedYN, BaseXPath.Toggle.Yes, BaseXPath.Toggle.No, havePoliceReport);

            if (havePoliceReport)
            {
                WaitForTextFieldAndEnterText(BaseXPath.Inputs.PoliceReportNumber, reportNumber);
                WaitForTextFieldAndEnterText(BaseXPath.Date.PoliceReport.Day, reportDate.Day.ToString(), false);
                WaitForTextFieldAndEnterText(BaseXPath.Date.PoliceReport.Month, reportDate.Month.ToString(), false);
                WaitForTextFieldAndEnterText(BaseXPath.Date.PoliceReport.Year, reportDate.Year.ToString(), false);
            }
        }

        public void SubmitPreliminaryDetailsAndNavigateDetailsOverrideWarning(BasePage nextPage, int waitTimeSeconds = WaitTimes.T90SEC)
        {
            Reporting.Log("Initial damage type details completed.", _browser.Driver.TakeSnapshot());
            ClickControl(BaseXPath.Button.ContinueSecondAccordion);

            using (var spinner = new B2C.RACSpinner(_browser))
            {
                spinner.WaitForSpinnerToFinish();

                if (nextPage.IsDisplayed())
                { return; }

                if (IsContactDetailsOverrideWarningPresent())
                {
                    // We'll accept the change and proceed.
                    ClickControl(BaseXPath.Dialog.Button.Yes);
                    spinner.WaitForSpinnerToFinish(nextPage: nextPage);
                }
                else
                {
                    Reporting.Error("Prelim details form has been submitted, but appear to have neither progressed to the next page, nor encountered 'contact details override' notice.");
                }
            }
        }

        /// <summary>
        /// Returns TRUE or FALSE to indicate if the "Override Contact Details"
        /// warning dialog is currently displayed on screen.
        /// </summary>
        /// <returns></returns>
        private bool IsContactDetailsOverrideWarningPresent()
        {
            var generalDialogShown = IsControlDisplayed(BaseXPath.Dialog.Frame);
            if (generalDialogShown)
            {
                if (GetInnerText(BaseXPath.Dialog.Text.Body).Contains(Constants.OverrideWarning))
                {
                    return true;
                }
                else
                {
                    Reporting.Error("We encountered a general dialog with an unexpected message when lodging preliminary questions for a new claim.");
                }
            }
            return false;
        }
    }
}
