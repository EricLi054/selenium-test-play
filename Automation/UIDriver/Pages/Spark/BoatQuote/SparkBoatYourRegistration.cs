using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;


namespace UIDriver.Pages.Spark.BoatQuote
{
    public class SparkBoatYourRegistration : BaseBoatPage
    {
        #region Constants
        private class Constants
        {
            public const string PageHeader = "Your registration";
            public const string ActiveStepperLabel = "Your rego";
            public class AdviseUser
            { 
                public static class FieldValidation
                {
                    public const string EnterBoatRego       = "Please enter a boat registration";
                    public const string EnterTrailerRego    = "Please enter a trailer registration";
                }
            }
        }

        #endregion

        #region XPATHS
        private class XPath
        {
            public const string PageHeader = "//*[@id='header']";
            public const string ActiveStepper = "//button[@aria-selected='true']";
            public static class Field
            {
                public const string KnowBoatRegistration        = "id('knowBoatRegistration')";
                public const string InputBoatRegistration       = "id('boat-rego')";
                public const string KnowTrailerRegistration     = "id('knowTrailerRegistration')";
                public const string InputTrailerRegistration    = "id('trailer-rego')";
            }
            public static class Button
            {
                public const string Yes = "//button[@aria-label='Yes']";
                public const string No = "//button[@aria-label='No']";
                public const string NextPage = "//button[@type='submit']";
            }
            public class AdviseUser
            {
                public class FieldValidation
                {
                    public const string YesNoToggle     = "//p[contains(text(),'select Yes or No')]";
                    public const string EnterBoatRego   = "//p[contains(text(),'a boat registration')]";
                    public const string EnterTrailerRego= "//p[contains(text(),'a trailer registration')]";
                }
            }
        }
        #endregion

        #region Settable properties and controls
        public bool KnowBoatRegistration
        {
            get => GetBinaryToggleState(XPath.Field.KnowBoatRegistration, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.Field.KnowBoatRegistration, XPath.Button.Yes, XPath.Button.No, value);
        }

        public string BoatRegistration
        {
            get => GetElement(XPath.Field.InputBoatRegistration).GetAttribute("value");
            set => SendKeyPressesAfterClearingExistingTextInField(XPath.Field.InputBoatRegistration, $"{value}{Keys.Tab}");
        }

        public bool KnowTrailerRegistration
        {
            get => GetBinaryToggleState(XPath.Field.KnowTrailerRegistration, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.Field.KnowTrailerRegistration, XPath.Button.Yes, XPath.Button.No, value);
        }
        public string TrailerRegistration
        {
            get => GetElement(XPath.Field.InputTrailerRegistration).GetAttribute("value");
            set => SendKeyPressesAfterClearingExistingTextInField(XPath.Field.InputTrailerRegistration, $"{value}{Keys.Tab}");
        }
        #endregion
        public SparkBoatYourRegistration(Browser browser) : base(browser)
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
            { Reporting.LogPageChange("Spark Boat Quote page ? - YourRegistration"); }

            return isDisplayed;
        }

        public void VerifyPageContent()
        {
            Reporting.AreEqual(Constants.PageHeader,
                GetElement(XPath.PageHeader).Text, "expected page header with actual.");

            Reporting.AreEqual(Sidebar.Link.PdsUrl,
                GetElement(XPaths.Sidebar.PdsLink).GetAttribute("href"), "NPE Sidebar PDS URL");

            VerifyStandardHeaderAndFooterContent();
            VerifyRegistrationFieldValidation();
            
            VerifyBoatFAQContent();
        }
        

        /// <summary>
        /// Check field mandatory field validation for this page.
        /// </summary>
        private void VerifyRegistrationFieldValidation()
        {
            ClickControl(XPath.Button.NextPage);
            Reporting.LogMinorSectionHeading("Selecting 'Next' to trigger field validation for Yes/No toggles.");

            Reporting.AreEqual(AdviseUser.FieldValidation.YesNoToggle, 
                GetElement(XPath.AdviseUser.FieldValidation.YesNoToggle).Text, "expected field validation for no selection on Boat Registration question");

            Reporting.Log($"Answering Know Boat Registration Yes to display input field and allow verification of Trailer Registration question.", _browser.Driver.TakeSnapshot());

            KnowBoatRegistration = true;

            Reporting.AreEqual(AdviseUser.FieldValidation.YesNoToggle,
                GetElement(XPath.AdviseUser.FieldValidation.YesNoToggle).Text, "expected field validation for no selection on Trailer Registration question");

            KnowTrailerRegistration = true;
            ClickControl(XPath.Button.NextPage);
            Reporting.Log($"Answering KnowTrailer Registration Yes to display input field then trigger empty field validation for both text input fields.", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(Constants.AdviseUser.FieldValidation.EnterBoatRego,
                GetElement(XPath.AdviseUser.FieldValidation.EnterBoatRego).Text, "expected field validation message when no value is input for Boat Registration despite 'Yes' answer");

            Reporting.AreEqual(Constants.AdviseUser.FieldValidation.EnterTrailerRego,
                GetElement(XPath.AdviseUser.FieldValidation.EnterTrailerRego).Text, "expected field validation message when no value is input for Trailer Registration despite 'Yes' answer");
        }

        public void InputRegistrationInformation(Browser browser, QuoteBoat quoteBoat)
        {
            Reporting.LogMinorSectionHeading("Inputting field content for 'Your Registration' page");
            if (quoteBoat.BoatRego == null)
            {
                KnowBoatRegistration = false;
            }
            else
            {
                KnowBoatRegistration = true;
                BoatRegistration = quoteBoat.BoatRego;
            }
            if (quoteBoat.BoatTrailerRego == null)
            {
                KnowTrailerRegistration = false;
            }
            else
            {
                KnowTrailerRegistration = true;
                TrailerRegistration = quoteBoat.BoatTrailerRego;
            }

            Reporting.Log($"Capturing page state", _browser.Driver.TakeSnapshot());
        }
        public void ContinueToPayment()
        {
            ClickControl(XPath.Button.NextPage);
        }
    }
}