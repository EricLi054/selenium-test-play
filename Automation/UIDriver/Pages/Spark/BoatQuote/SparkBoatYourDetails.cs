using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

public class SparkBoatYourDetails : SparkPersonalInformationPage
{
    #region XPATHS
    private class XPath
    {
        public const string PageHeader = "//*[@id='header']";
        public const string ActiveStepper = "//button[@aria-selected='true']";
        public class Button
        {
            public const string MailingAddressHelpTextButton = "//button[@id='tooltip-mailingAddressButton']";
            public const string MailingAddressHelpTextClose = "//button[@id='tooltip-mailingAddress-close']";
            public const string Yes = "//button[@aria-label='Yes']";
            public const string No = "//button[@aria-label='No']";
            public const string NextPage = FORM + "//button[@type='submit']";
        }
        public class AdviseUser
        {
            public class Helptext
            {
                public const string MailingAddressHelpTextTitle     = "//div[@id='tooltip-mailingAddress-title']";
                public const string MailingAddressHelpTextMessage   = "//div[@id='tooltip-mailingAddress-message']";
                
            }
            public class FieldValidation
            {
                public const string Title           = "//p[contains(text(),'Please select a title')]";
                public const string Gender          = "//p[contains(text(),'Please select one')]";
                public const string FirstName       = "//p[contains(text(),'valid first name')]";
                public const string LastName        = "//p[contains(text(),'valid last name')]";
                public const string Telephone       = "//p[contains(text(),'For a mobile')]";
                public const string Email           = "//p[contains(text(),'valid email')]";
                public const string MailingAddress  = "//p[contains(text(),'valid address')]";
            }
            public class BoatFaqCard
            {
                public readonly static string Head = "id('faq-header')";
                public readonly static string Body = "id('faq-body')";
                public readonly static string Link = "id('faq-link')";
            }
        }
    }
    #endregion

    #region Constants
    private class Constants
    {
        public const string PageHeader = "Your details";
        public const string ActiveStepperLabel = "Your details";
        public class AdviseUser
        {
            public class Helptext
            {
                public const string MailingAddressHelpTextTitle = "Mailing address";
                public const string MailingAddressHelpTextMessage = "Please enter the address where you prefer to receive mail. " +
                                                                    "If it doesn't appear, try entering fewer details (e.g. remove " +
                                                                    "the unit number). If you can't find your address please call us " +
                                                                    "on 13 17 03 with your quote number.";
            }
            public class FieldValidation
            {
                public const string Title           = "Please select a title";
                public const string FirstName       = "Please enter a valid first name";
                public const string LastName        = "Please enter a valid last name";
                public const string Telephone       = "For a mobile, enter your 10 digit number or for a landline, please include your area code";
                public const string Email           = "Please enter a valid email";
                public const string MailingAddress  = "Please enter a valid address";
            }
        }
    }
    #endregion

    #region Settable properties and controls

    #endregion
    public SparkBoatYourDetails(Browser browser) : base(browser)
    { }

    public override bool IsDisplayed()
    {
        var isDisplayed = false;
        try
        {
            isDisplayed = string.Equals(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
        }
        catch (NoSuchElementException)
        {
            return false;
        }

        if (isDisplayed)
        { Reporting.LogPageChange("Spark Boat Quote page ? - YourDetails"); }

        return isDisplayed;
    }

    public void VerifyBoatFAQContent()
    {
        Reporting.AreEqual(AdviseUser.BoatFaqCard.Head, GetInnerText(XPath.AdviseUser.BoatFaqCard.Head),
            "expected FAQ card Header text with actual value on page");
        Reporting.AreEqual(AdviseUser.BoatFaqCard.Body, GetInnerText(XPath.AdviseUser.BoatFaqCard.Body),
            "expected FAQ card Body text with actual value on page");
        Reporting.AreEqual(AdviseUser.BoatFaqCard.Link, GetElement(XPath.AdviseUser.BoatFaqCard.Link).GetAttribute("href"),
            "expected FAQ card link URL with actual value on page");
        Reporting.LogMinorSectionHeading("End Boat FAQ Card verification");
    }

    public void VerifyPageContent()
    {
        Reporting.AreEqual(Constants.PageHeader,
        GetElement(XPath.PageHeader).Text, "expected page header with actual.");

        Reporting.AreEqual(Sidebar.Link.PdsUrl,
            GetElement(XPaths.Sidebar.PdsLink).GetAttribute("href"), "NPE Sidebar PDS URL");

        VerifyStandardHeaderAndFooterContent();
        VerifyFieldValidationErrors();
        VerifyBoatFAQContent();
    }

    /// <summary>
    /// Check field validation for the Your details page.
    /// </summary>
    private void VerifyFieldValidationErrors()
    {
        Reporting.LogMinorSectionHeading("Selecting the Next button to trigger field validation messages");
        ClickControl(XPath.Button.NextPage);
        Reporting.Log("Capturing snapshot before evaluating strings.", _browser.Driver.TakeSnapshot());
        
        Reporting.AreEqual(Constants.AdviseUser.FieldValidation.Title, 
            GetInnerText(XPath.AdviseUser.FieldValidation.Title), "field validation message for Title against expected value");
        Reporting.AreEqual(Constants.AdviseUser.FieldValidation.FirstName, 
            GetInnerText(XPath.AdviseUser.FieldValidation.FirstName), "field validation message for First Name against expected value");
        Reporting.AreEqual(Constants.AdviseUser.FieldValidation.LastName, 
            GetInnerText(XPath.AdviseUser.FieldValidation.LastName), "field validation message for Last Name against expected value");
        Reporting.AreEqual(Constants.AdviseUser.FieldValidation.Telephone, 
            GetInnerText(XPath.AdviseUser.FieldValidation.Telephone), "field validation message for Telephone against expected value");
        Reporting.AreEqual(Constants.AdviseUser.FieldValidation.Email, 
            GetInnerText(XPath.AdviseUser.FieldValidation.Email), "field validation message for Email against expected value");
        Reporting.AreEqual(Constants.AdviseUser.FieldValidation.MailingAddress, 
            GetInnerText(XPath.AdviseUser.FieldValidation.MailingAddress), "field validation message for Mailing Address against expected value");


        DisplayGenderField();
        ClickControl(XPath.Button.NextPage);
        Reporting.Log("Set a title to display Gender field then selected Next again to display field validation for Gender field", _browser.Driver.TakeSnapshot());

        Reporting.AreEqual(AdviseUser.FieldValidation.SelectOne, 
            GetInnerText(XPath.AdviseUser.FieldValidation.Gender), "field validation message for Gender against expected value");

        ClickControl(XPath.Button.MailingAddressHelpTextButton);
        _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.Helptext.MailingAddressHelpTextTitle), WaitTimes.T5SEC);
        
        Reporting.AreEqual(Constants.AdviseUser.Helptext.MailingAddressHelpTextTitle, 
            GetInnerText(XPath.AdviseUser.Helptext.MailingAddressHelpTextTitle), "title of help text for mailing address field against expected value");
        Reporting.AreEqual(Constants.AdviseUser.Helptext.MailingAddressHelpTextMessage, 
            GetInnerText(XPath.AdviseUser.Helptext.MailingAddressHelpTextMessage), "message of help text for mailing address field against expected value");
        Reporting.Log("Capturing snapshot of Help Text for Mailing Address before closing it", _browser.Driver.TakeSnapshot());
        ClickControl(XPath.Button.MailingAddressHelpTextClose);
    }

    /// <summary>
    /// Utilise SparkPersonalInformationPage.cs to complete the personal information
    /// generated in the testData builder for the candidate policyholder.
    /// </summary>
    /// <param name="candidatePolicyHolder">Details of the candidate policyholder</param>
    public void FillPersonalInformation(Contact candidatePolicyHolder)
    {

        SetTitleWithGender(candidatePolicyHolder);
        FirstName = candidatePolicyHolder.FirstName;
        LastName = candidatePolicyHolder.Surname;
        ContactNumber = candidatePolicyHolder.MobilePhoneNumber;
        Email = candidatePolicyHolder.PrivateEmail.Address;

        MailingAddress = candidatePolicyHolder.MailingAddress.StreetSuburbState();
        Reporting.Log("Capturing personal details input", _browser.Driver.TakeSnapshot());
    }

    public void ContinueToYourRegistration()
    {
        ClickControl(XPath.Button.NextPage);
    }
}
