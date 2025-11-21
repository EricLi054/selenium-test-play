using Rac.TestAutomation.Common;
using System;
using System.Threading;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;
using static Rac.TestAutomation.Common.Constants.Contacts;
using OpenQA.Selenium;
using UIDriver.Pages.Spark.BoatQuote;

public class SparkBoatAboutYou : BaseBoatPage
{
    #region Constants
    private class Constants
    {
        public const string PageHeader = "About you";
        public const string ActiveStepperLabel = "About you";
        public class AdviseUser
        {
            public class FieldValidation
            {
                public const string DateOfBirthEmpty        = "Please enter your date of birth";
                public const string DateOfBirthInvalid      = "You must be aged between 16 and 100";
            }
            public class DeclinedCover
            {
                public const string Title                       = "Sorry, we can't insure you";
                public const string MessageBody1                = "You don't meet our eligibility requirements.";
                public const string MessageBody2                = "If you have any questions about this, please call us on 13 17 03.";
                public const string MessageBody3                = "Visit findaninsurer.com.au to find insurance for your boat.";
                public const string FindAnInsurerCallToAction   = "https://findaninsurer.com.au/";
                public const string PhoneNumberCallToAction     = "tel:131703";
            }
        }
        
    }
    #endregion

    #region XPATHS
    private class XPath
    {
        public const string PageHeader = "//*[@id='header']";
        public const string ActiveStepper = "//button[@aria-selected='true']";
        public class Field
        {
            public const string DateOfBirth         = "id('about-you-dob')";
            public const string SkippersTicket      = "id('about-you-skippers-select')";
            public const string ClaimsHistory       = "id('claims-history-number-of-claims-select')";

            public class Options
            {
                public const string Selector        = "//ul[@role='listbox']";
                public const string SelectorOptions = XPath.Field.Options.Selector + "/li";
            }
        }
        public class Button
        {
            public const string NextPage = "//button[@type='submit']";
        }
        public class AdviseUser
        {
            public class FieldValidation
            {
                public const string DateOfBirth             = "//p[contains(text(),'your date of birth')]";
                public const string DateOfBirthInvalid      = "//p[contains(text(),'16 and 100')]";
                public const string PleaseSelectOneEmpty    = "//div[contains(text(),'Please select one')]";
            }
            public class DeclinedCover
            {
                public const string Title                       = "id('claims-history-cover-declined-notification-title')";
                public const string MessageBody1                = "id('claims-history-cover-declined-notification-body-1')";
                public const string MessageBody2                = "id('claims-history-cover-declined-notification-body-2')";
                public const string MessageBody3                = "id('claims-history-cover-declined-notification-body-3')";
                public const string FindAnInsurerCallToAction   = "id('claims-history-cover-declined-notification-link-insurer')";
                public const string PhoneNumberCallToAction     = "id('claims-history-cover-declined-notification-link-phone')";
            }
        }
    }
    #endregion

    #region Settable properties and controls

    #endregion
    public SparkBoatAboutYou(Browser browser) : base(browser)
    { }

    public override bool IsDisplayed()
    {
        var isDisplayed = false;
        try
        {
            isDisplayed = string.Equals(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
            GetElement(XPath.Field.DateOfBirth);
        }
        catch (NoSuchElementException)
        {
            return false;
        }
        if (isDisplayed)
        { Reporting.LogPageChange("Spark Boat Quote page ? - About you"); }

        return isDisplayed;
    }

    /// <summary>
    /// Triggers detailed verification of page content, only invoked when detailedUiChecking = true.
    /// </summary>
    public void VerifyPageContent()
    {
        VerifyFieldValidation();
        VerifyBoatFAQContent();
    }

    /// <summary>
    /// Checks the Field Validation messages for each field on this page, as 
    /// well as cycling through the expected options for the drop-down fields.
    /// Note: is only invoked when detailedUiChecking = true.
    /// </summary>
    /// <param name="browser"></param>
    public void VerifyFieldValidation()
    {
        Reporting.AreEqual(Constants.PageHeader,
            GetInnerText(XPath.PageHeader), "page header content against expected value");

        Reporting.AreEqual(Sidebar.Link.PdsUrl,
            GetElement(XPaths.Sidebar.PdsLink).GetAttribute("href"), "NPE Sidebar PDS URL");
        VerifyStandardHeaderAndFooterContent();

        ContinueToTypeOfBoat();
        
        VerifyEmptyFieldValidationErrors();
        
        VerifySkippersTicketOptions();
        
        VerifyDeclinedCoverClaimHistory();
        
        ExpectToFailToProgress();
        
        VerifyClaimHistoryOptions();

        VerifyAgeTooYoungValidation();

        VerifyLimitedSkippersTicketOptions();
    }
    private void VerifyEmptyFieldValidationErrors()
    { 
        Reporting.Log($"Capturing screenshot after attempting to continue without inputting anything.", _browser.Driver.TakeSnapshot());
        Reporting.AreEqual(Constants.AdviseUser.FieldValidation.DateOfBirthEmpty,
            GetInnerText(XPath.AdviseUser.FieldValidation.DateOfBirth), "Date of Birth empty field validation text");
        Reporting.AreEqual(AdviseUser.FieldValidation.SelectOne,
            GetInnerText(XPath.AdviseUser.FieldValidation.PleaseSelectOneEmpty), "Skippers Ticket empty field validation text");
        Reporting.AreEqual(AdviseUser.FieldValidation.SelectOne,
            GetInnerText(XPath.AdviseUser.FieldValidation.PleaseSelectOneEmpty), "Claims History empty field validation text");
    }

    private void VerifySkippersTicketOptions()
    {
        InputSkippersTicket = SkippersTicketYearsHeld.Noskippersticket.GetDescription();
        InputSkippersTicket = SkippersTicketYearsHeld.Lessthan1.GetDescription();
        InputSkippersTicket = SkippersTicketYearsHeld.LessThanTwo.GetDescription();
        InputSkippersTicket = SkippersTicketYearsHeld.LessThanThree.GetDescription();
        InputSkippersTicket = SkippersTicketYearsHeld.MoreThanThree.GetDescription();
        Reporting.Log($"Have validated options available in Skippers Ticket and now Claims History empty field validation text can be verified", _browser.Driver.TakeSnapshot());
    }

    private void VerifyDeclinedCoverClaimHistory()
    {
        Reporting.LogMinorSectionHeading("Triggering Declined Cover with Claims History");
        InputClaimsHistoryCount = "6+";
        Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.Title,
            GetInnerText(XPath.AdviseUser.DeclinedCover.Title), "title of Declined Cover notice");
        Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.MessageBody1,
            GetInnerText(XPath.AdviseUser.DeclinedCover.MessageBody1), "first part of Declined Cover message");
        Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.MessageBody2,
            GetInnerText(XPath.AdviseUser.DeclinedCover.MessageBody2), "second part of Declined Cover message");
        Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.MessageBody3,
            GetInnerText(XPath.AdviseUser.DeclinedCover.MessageBody3), "third part of Declined Cover message");
        Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.FindAnInsurerCallToAction,
            GetElement(XPath.AdviseUser.DeclinedCover.FindAnInsurerCallToAction).GetAttribute("href"), "findaninsurer.com.au call to action URL");
        Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.PhoneNumberCallToAction,
            GetElement(XPath.AdviseUser.DeclinedCover.PhoneNumberCallToAction).GetAttribute("href"), "phone number link in call to action");
        Reporting.Log($"Capturing snapshot of Declined Cover. before cycling other options in descending order and setting value " +
            $"from test data.", _browser.Driver.TakeSnapshot());
    }

    private void VerifyClaimHistoryOptions()
    {
        InputClaimsHistoryCount = "5";
        InputClaimsHistoryCount = "4";
        InputClaimsHistoryCount = "3";
        InputClaimsHistoryCount = "2";
        InputClaimsHistoryCount = "1";
        InputClaimsHistoryCount = "0";
        Reporting.Log($"Cycle of all dropdown options for Claims History count completed successfully.");
    }

    private void VerifyAgeTooYoungValidation() 
    { 
        ClickControl(XPath.Field.DateOfBirth);
        InputDateOfBirth = DateTime.Now.AddYears(-MIN_PH_AGE_VEHICLES).AddDays(1).ToString(DateTimeTextFormat.ddMMyyyy);
        Reporting.Log($"Capturing screenshot after inputting a Date Of Birth value which would make policyholder less than 16 years old.", _browser.Driver.TakeSnapshot());
        Reporting.AreEqual(Constants.AdviseUser.FieldValidation.DateOfBirthInvalid,
            GetInnerText(XPath.AdviseUser.FieldValidation.DateOfBirthInvalid), "Date of Birth invalid value validation text");
    }

    /// <summary>
    /// Check that when the user selects the Next button which APPEARS to be enabled, that the user is actually unable
    /// to proceed to the next page. Only to be invoked when there is some blocking condition in place such as incomplete
    /// fields, Decline Cover messages displayed etc.
    /// </summary>
    private void ExpectToFailToProgress()
    {
        Reporting.Log($"Selecting '{GetInnerText(XPath.Button.NextPage)}' despite blocker to check whether progress is blocked.");
        ClickControl(XPath.Button.NextPage);
        Thread.Sleep(SleepTimes.T5SEC);
        Reporting.AreEqual(Constants.ActiveStepperLabel,
                GetElement(XPath.ActiveStepper).Text, " value of active step is still 'About you' and user has not been able to " +
                "progress to the next page.");
    }

    private void VerifyLimitedSkippersTicketOptions()
    {
        ClickControl(XPath.Field.DateOfBirth);
        InputDateOfBirth = DateTime.Now.AddYears(-MIN_PH_AGE_VEHICLES).ToString(DateTimeTextFormat.ddMMyyyy);
        Reporting.Log($"Setting Date of Birth to exactly {MIN_PH_AGE_VEHICLES} years old before checking " +
            $"available Skipper's Ticket options which should no longer include '3+'.", _browser.Driver.TakeSnapshot());

        ClickControl(XPath.Field.SkippersTicket);

        Reporting.IsFalse(IsControlDisplayed($"{XPath.Field.Options.SelectorOptions}" + "[@data-value='3+']"), 
            "that '3+' is no longer available");
        
        Reporting.IsTrue(IsControlDisplayed($"{XPath.Field.Options.SelectorOptions}" + "[@data-value='2-3']"), 
            "that value '2-3' still exists and selecting it");
        ClickControl(XPath.Field.Options.SelectorOptions + "[@data-value='2-3']");
    }

    public void EnterAboutYouDetails(Browser browser, QuoteBoat testData)
    {
        Reporting.LogMinorSectionHeading("Enter Test Data");

        ClickControl(XPath.Field.DateOfBirth);
        InputDateOfBirth = testData.CandidatePolicyHolders[0].DateOfBirth.ToString(DateTimeTextFormat.ddMMyyyy);
        
        if (testData.CandidatePolicyHolders[0].DateOfBirth > DateTime.Now.AddYears(-MIN_PH_AGE_BOAT_SKIPPERTICKET_MORETHANTHREE)) 
        {
            Reporting.Log($"Because generated date of birth makes age less than " +
                $"{MIN_PH_AGE_BOAT_SKIPPERTICKET_MORETHANTHREE} years old we may " +
                $"need to adjust planned Skipper's Ticket value.");
            if (testData.SkippersTicketHeld.GetDescription().Equals("3+"))
            {
                Reporting.Log($"Test Data value for Skipper's Ticket is '{testData.SkippersTicketHeld.GetDescription()}' which won't be available under " +
                    $"{MIN_PH_AGE_BOAT_SKIPPERTICKET_MORETHANTHREE} years old, so overwriting quote data to " +
                    $"'{SkippersTicketYearsHeld.LessThanThree.GetDescription()}' instead.");
                testData.SkippersTicketHeld = SkippersTicketYearsHeld.LessThanThree;
                InputSkippersTicket = testData.SkippersTicketHeld.GetDescription();
            }
            else
            {
                Reporting.Log($"Test Data value for Skipper's Ticket is '{testData.SkippersTicketHeld.GetDescription()}' which is available for " +
                    $"Policyholders under {MIN_PH_AGE_BOAT_SKIPPERTICKET_MORETHANTHREE} years old, " +
                    $"so we can input that value.");
                InputSkippersTicket = testData.SkippersTicketHeld.GetDescription();
            }
        }
        else
        {
            Reporting.Log($"Because Date of Birth makes this user over " +
                    $"{MIN_PH_AGE_BOAT_SKIPPERTICKET_MORETHANTHREE} years old, " +
                    $"all Skipper's Ticket options should be available and we'll input the Test Data value without further consideration.");
            InputSkippersTicket = testData.SkippersTicketHeld.GetDescription();
        }
        
        InputClaimsHistoryCount = testData.HistoricBoatClaims.GetDescription();
        Reporting.Log($"Capturing screenshot of completed fields.", _browser.Driver.TakeSnapshot());
    }
    public string InputDateOfBirth
    {
        get => GetInnerText(XPath.Field.DateOfBirth);

        set => SendKeyPressesAfterClearingExistingTextInField(XPath.Field.DateOfBirth, value);
    }

    public string InputSkippersTicket
    {
        get => GetInnerText(XPath.Field.SkippersTicket);

        set => WaitForSelectableAndPickFromDropdown(XPath.Field.SkippersTicket, XPath.Field.Options.SelectorOptions, value);
    }
    public string InputClaimsHistoryCount
    {
        get => GetInnerText(XPath.Field.ClaimsHistory);

        set => WaitForSelectableAndPickFromDropdown(XPath.Field.ClaimsHistory, XPath.Field.Options.SelectorOptions, value);
    }

    public void ContinueToTypeOfBoat()
    {
        ClickControl(XPath.Button.NextPage);
    }
}
