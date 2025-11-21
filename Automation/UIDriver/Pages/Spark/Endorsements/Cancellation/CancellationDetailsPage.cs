using Rac.TestAutomation.Common;
using System;
using System.Globalization;
using System.Threading;
using OpenQA.Selenium;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.Endorsements;

namespace UIDriver.Pages.Spark.Endorsements
{
    /// <summary>
    /// The cancellation details page will help guide a member through the cancellation 
    /// process. It will allow them to select a date (from allowed dates) for 
    /// the cancellation, a reason for cancellation and a chance to confirm 
    /// their email address for the cancellation notice.
    /// </summary>
    public class CancellationDetailsPage : SparkBasePage
    {
        #region CONSTANTS
        public class Constants
        {
            public class ValidationMessage
            {
                public const string Email = "Please enter a valid email";
                public const string Reason = "Please select a reason for cancelling";
            }

            public class AdditionalInformationIdPrefix
            {
                public const string DemolishedHouse = "demolishedHouse";
                public const string FinancialHardship = "hardship";
                public const string SoldHouse = "soldHouse";
            }

            public class AdditionalInformationText
            {
                public const string DemolishedHouse = "You can update your policy to only cover your contents by calling us on 13 17 03.";
                public const string FinancialHardship = "We may be able to help you with financial hardship support. Find out more.";
                public const string SoldHouse = "If you're moving house in WA, you can transfer your policy instead. Simply change your home details.";
            }

            public class AdditionalInformationTitle
            {
                public const string DemolishedHouse = "Did you know?";
                public const string FinancialHardship = "Before you cancel";
                public const string SoldHouse = "You might not need to cancel";
            }

            public class RefundProcessing
            {
                public const string PartOne = "We will process your refund once you cancel your policy, as per our";
                public const string PartTwo = "We'll refund your money into the account or card you made payments from.";
                public const string LinkText = "Product Disclosure Statement";
            }
         }
        #endregion

        #region XPATHS
        private class XPath
        {
            public class PageInformation
            {
                public const string Title = "id('header')";
                public const string SubTitle = "id('subtitle')";
            }

            public class Cancellation {
               
                public class LastDayOfCover
                {
                    public const string CalendarPickerButton = Input + "/following-sibling::div/button";
                    public const string Label = "id('label-select-cancellation-date')";
                    public const string Input = "id('select-cancellation-date')";
                }

                public class Reason
                {
                    public const string ListInvoker = "id('cancellationReasonDetail')";
                    public const string List = "id('cancellationReasonDetail-listbox')/li/ul/li";
                    public const string Label = "//label[@for='cancellationReasonDetail']";
                    public const string ValidationMessage = "//p[contains(@class, 'Mui-error') and text() = '" +
                        Constants.ValidationMessage.Reason + "']";
                    public const string ListOpener = "//button[contains(@title,'Open')]";
                    public const string ListCloser = "//button[contains(@title,'Close')]";
                }

                // Extra information is displayed for some cancellation reasons
                public class ExtraInfo
                {
                    public static string Card(string situation, string suffix) => $"id('{situation}ContentTitle{suffix}')";
                    public static string Title(string situation, string suffix) => $"id('{situation}ContentTitle{suffix}-title')";
                    public static string ParagraphOne(string situation, string suffix) => $"id('{situation}ContentParagraph{suffix}')";
                }

                public class Email
                {
                    public const string InputField = "id('email')";
                    public const string ValidationMessage = "//p[contains(@class, 'Mui-error') and text() = '" +
                        Constants.ValidationMessage.Email + "']";
                }

                public class Button
                {
                    public const string CancelPolicy = "id('submit')";
                }
            }

            public class ConfirmationModal
            {
                public const string Root = "id('cancel-confirmation-dialog')";
                public const string Title = "id('cancel-confirmation-dialog-title')/h2";
                public const string Subtitle = "id('cancel-confirmation-dialog-content-subtitle')";
                public const string AgreementText = "id('cancel-confirmation-dialog-content-disclaimer')";

                public class Button
                {
                    public const string ConfirmPolicyCancellation = "id('cancel-confirmation-dialog-actions-confirm-cancellation-button')";
                    public const string Back = "id('cancel-confirmation-dialog-actions-back-button')";
                    public const string CloseDialog = "id('cancel-confirmation-dialog-title')/button";
                }
            }

            public class DatePicker
            {
                public static string CalendarInvocationButton(string calendarBase) => $"{calendarBase}//button[@aria-label='change date']";
                // Date Picker is then displayed in a Mui dialog at the page root
                public const string CalendarHeader = "//div[@class='MuiPickersCalendarHeader-switchHeader']//p[contains(@class,'MuiTypography-body1')]";
                public const string CalendarMonthButtons = "//button[contains(@class,'MuiButtonBase-root MuiIconButton-root MuiPickersCalendarHeader-iconButton')]";
                public const string CalendarPrecedingMonth = "//div[contains(@class,'MuiPickersSlideTransition-transitionContainer MuiPickersCalendarHeader-transitionContainer')]/preceding-sibling::button";
                public const string CalendarFollowingMonth = "//div[contains(@class,'MuiPickersSlideTransition-transitionContainer MuiPickersCalendarHeader-transitionContainer')]/following-sibling::button";
                public const string CalendarDate = "//div[@class='MuiPickersCalendar-week']/div/button[not(contains(@class,'dayDisabled')) and not(contains(@class,'Day-hidden'))]";
            }

            public class ProcessingInformation
            {
                public const string Paragraph1 = "//p/b[contains(text(),\"" + Constants.RefundProcessing.PartOne + "\")]";
                public const string Paragraph2 = "//p/b[contains(text(),\"" + Constants.RefundProcessing.PartTwo + "\")]";
                public const string ProductDisclosureLink = Paragraph1 + "/a[contains(@href,'products/insurance/policy-documents') and contains(text(),\"" +
                    Constants.RefundProcessing.LinkText + "\")]";
            }
        }
        #endregion

           
        public string PageTitle => GetInnerText(XPath.PageInformation.Title);
        public string PageSubTitle => GetInnerText(XPath.PageInformation.SubTitle);
        
        // Last day of cover for member.  In some circumstance different dates can be selected
        public DateTime LastDayOfCover
        {
            get => DateTime.ParseExact(GetValue(XPath.Cancellation.LastDayOfCover.Input),
                                       DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH_WITHSPACE,
                                       CultureInfo.InvariantCulture);
            set => SelectDateFromCalendar(dateFieldXPath: null, calendarBtnXPath: XPath.Cancellation.LastDayOfCover.CalendarPickerButton, desiredDate: value);
        }
 
        public string EmailAddress
        {
            get => GetValue(XPath.Cancellation.Email.InputField);
            set => WaitForTextFieldAndEnterText(XPath.Cancellation.Email.InputField, value, false);
        }

        public string ConfirmationModalTitle 
            => GetInnerText(XPath.ConfirmationModal.Title);
        public string ConfirmationModalSubTitle 
            => GetInnerText(XPath.ConfirmationModal.Subtitle);
        public string ConfirmationModalAgreementText 
            => GetInnerText(XPath.ConfirmationModal.AgreementText);

        // For a small set of reasons, important extra information is to be displayed to the member
        // House Demolished
        public bool IsExtraInformationDemolishedHouseDisplayed => _driver.TryWaitForElementToBeVisible(
            By.XPath(XPath.Cancellation.ExtraInfo.Card(Constants.AdditionalInformationIdPrefix.DemolishedHouse, String.Empty)), WaitTimes.T5SEC, out IWebElement element);
        public string HouseDemolishedExtraInfoTitle => GetInnerText(XPath.Cancellation.ExtraInfo.Title(Constants.AdditionalInformationIdPrefix.DemolishedHouse,String.Empty));
        public string HouseDemolishedExtraInfoText => GetInnerText(XPath.Cancellation.ExtraInfo.ParagraphOne(Constants.AdditionalInformationIdPrefix.DemolishedHouse, String.Empty));

        // Financial Hardship
        public bool IsExtraInformationFinancialHardshipDisplayed => _driver.TryWaitForElementToBeVisible(
            By.XPath(XPath.Cancellation.ExtraInfo.Card(Constants.AdditionalInformationIdPrefix.FinancialHardship, String.Empty)), WaitTimes.T5SEC, out IWebElement element);
        public string FinancialHardshipExtraInfoTitle => GetInnerText(XPath.Cancellation.ExtraInfo.Title(Constants.AdditionalInformationIdPrefix.FinancialHardship, String.Empty));
        public string FinancialHardshipExtraInfoText => GetInnerText(XPath.Cancellation.ExtraInfo.ParagraphOne(Constants.AdditionalInformationIdPrefix.FinancialHardship, "One"));

        // House Sold
        public bool IsExtraInformationSoldHouseDisplayed => _driver.TryWaitForElementToBeVisible(
    By.XPath(XPath.Cancellation.ExtraInfo.Card(Constants.AdditionalInformationIdPrefix.SoldHouse, "Id")), WaitTimes.T5SEC, out IWebElement element);
        public string HouseSoldExtraInfoTitle => GetInnerText(XPath.Cancellation.ExtraInfo.Title(Constants.AdditionalInformationIdPrefix.SoldHouse, "Id"));
        public string HouseSoldExtraInfoText
            => GetInnerText(XPath.Cancellation.ExtraInfo.ParagraphOne(Constants.AdditionalInformationIdPrefix.SoldHouse, String.Empty));

        // Before processing the cancellation, we present a modal for the member
        // to confirm their intention to cancel.  The following are
        // to check for the modals presence and has the necessary elements.
        public bool IsConfirmationModalDisplayed => 
            _driver.TryWaitForElementToBeVisible(By.XPath(XPath.ConfirmationModal.Root),
                                                 WaitTimes.T10SEC, out IWebElement element);
        public bool IsConfirmationModalBackButtonPresent =>
            IsControlDisplayed(XPath.ConfirmationModal.Button.Back);
        public bool IsConfirmationModalCloseButtonPresent =>
            IsControlDisplayed(XPath.ConfirmationModal.Button.CloseDialog);
        public bool IsConfirmationModalConfirmCancellationButtonPresent => 
            IsControlDisplayed(XPath.ConfirmationModal.Button.ConfirmPolicyCancellation);

        // Date picker enablement is dependent on circumstances
        public bool IsDatePickerEnabled => IsControlDisplayed(XPath.Cancellation.LastDayOfCover.CalendarPickerButton) && IsControlEnabled(XPath.Cancellation.LastDayOfCover.CalendarPickerButton);

        // Processing Refund Information
        public bool IsProcessingInformationFirstParagraphPresent => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.ProcessingInformation.Paragraph1),
            WaitTimes.T5SEC, out IWebElement element);
        public string GetProcessingInformationFirstParagraph => GetInnerText(XPath.ProcessingInformation.Paragraph1);
        public bool IsProcessingInformationSecondParagraphPresent => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.ProcessingInformation.Paragraph2),
            WaitTimes.T5SEC, out IWebElement element);
        public bool IsProcessingInformationLinkPresent => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.ProcessingInformation.ProductDisclosureLink),
            WaitTimes.T5SEC, out IWebElement element); 
 
        // Checks for error messages on invalid input
        public bool IsReasonValidationMessagePresent =>
            IsControlDisplayed(XPath.Cancellation.Reason.ValidationMessage);
        public bool IsEmailValidationMessagePresent =>
            IsControlDisplayed(XPath.Cancellation.Email.ValidationMessage);

        public CancellationDetailsPage(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XPath.PageInformation.Title);
                GetElement(XPath.PageInformation.SubTitle);
                GetElement(XPath.Cancellation.Reason.ListInvoker);
                GetElement(XPath.Cancellation.LastDayOfCover.Label);
                GetElement(XPath.Cancellation.LastDayOfCover.Input);
                GetElement(XPath.Cancellation.Email.InputField);
                GetElement(XPath.Cancellation.Button.CancelPolicy);
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }

        public void SetEmailAddress(string newEmailAddress)
        {
            WaitForTextFieldAndEnterText(XPath.Cancellation.Email.InputField, newEmailAddress, false);
        }

        public void SetReason(string reasonForCancellation)
        {
            WaitForSelectableAndPickFromDropdown(XPath.Cancellation.Reason.ListInvoker,
                XPath.Cancellation.Reason.List, reasonForCancellation);
        }

        public void OpenReasonList()
        {
            ClickControl(XPath.Cancellation.Reason.ListOpener, WaitTimes.T10SEC);
        }

        public void CloseReasonList()
        {
            ClickControl(XPath.Cancellation.Reason.ListCloser, WaitTimes.T10SEC);
        }

        public void ChooseFinancialHarship()
        {
            WaitForSelectableAndPickFromDropdown(XPath.Cancellation.Reason.ListInvoker,
               XPath.Cancellation.Reason.List, Cancellations.Reason.FinanacialHardship.Text);
        }

        public void ChooseHouseDemolished()
        {
            WaitForSelectableAndPickFromDropdown(XPath.Cancellation.Reason.ListInvoker,
               XPath.Cancellation.Reason.List, Cancellations.Reason.HouseDemolished.Text);
        }

        public void ChooseHouseSold()
        {
            WaitForSelectableAndPickFromDropdown(XPath.Cancellation.Reason.ListInvoker,
               XPath.Cancellation.Reason.List, Cancellations.Reason.HouseSold.Text);
        }

        public void SubmitCancellation()
        {
            ClickControl(XPath.Cancellation.Button.CancelPolicy);
            WaitForPage(WaitTimes.T5SEC);
        }

        public void ConfirmPolicyCancellation()
        {
            ClickControl(XPath.ConfirmationModal.Button.ConfirmPolicyCancellation);
        }

        public void RejectPolicyCancellation()
        {
            ClickControl(XPath.ConfirmationModal.Button.Back);
        }

    }
}
