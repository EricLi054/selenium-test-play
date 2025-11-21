using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;
using OpenQA.Selenium;

namespace UIDriver.Pages.Spark.BoatQuote
{
    public class SparkBoatMoreBoat : BaseBoatPage
    {
        #region Constants
        private class Constants
        {
            public const string PageHeader          = "More about your boat";
            public const string ActiveStepperLabel  = "More about your boat";
            public class Field
            {
                public class Label
                {
                    public const string Suburb      = "Suburb where you keep your boat";
                    public const string Garage      = "Boat kept in a garage";
                    public const string Motor       = "Type of motor";
                    public const string Security    = "Security systems on your boat\r\nYou can choose more than one option";
                    public const string SecuritySub = "You can choose more than one option";
                }
            }
            public class AdviseUser
            {
                public class FieldValidation
                {
                    public const string EmptySuburb     = "Please enter a suburb";
                    public const string EmptySecurity   = "Please select all options that apply";
                }
                public class HelpText
                {
                    public static readonly string SecurityTitle     = "Security systems";
                    public static readonly string SecurityBody      = "Trailer security device: A tool that prevents theft of your boat or trailer, e.g. wheel or hitch locks.\r\n\r\n" +
                                                                    "Alarm or GPS tracker: GPS is technology that provides the exact location of your boat.\r\n\r\n" +
                                                                    "nebolink: A boat monitoring and voyage logging system.";
                }
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public const string PageHeader      = "//*[@id='header']";
            public const string ActiveStepper   = "//button[@aria-selected='true']";

            public class Field
            {
                public const string Suburb              = "//div[@data-testid='suburb-container']";
                public const string SuburbOptions       = "//ul[@role='listbox']" + "//li";
                public const string KeptInGarage        = "id('kept-in-garage')";
                public const string TypeOfMotor         = "id('more-boat-motor-type-select')";
                public const string TypeOfMotorOptions  = "//ul[@role='listbox']" + "//li";
                public class Label
                { 
                    public const string Suburb      = "//label[contains(text(),'Suburb')]"; //Suburb where you keep your boat
                    public const string Garage      = "//label[contains(text(),'garage')]"; //Boat kept in a garage
                    public const string Motor       = "//label[contains(text(),'motor')]"; //Type of motor
                    public const string Security    = "id('security-systems-multi-choice-input-label')";
                    public const string SecuritySub = "id('security-systems-multi-choice-input-sublabel')";
                }
            }
            public class Button
            {
                public const string Yes             = "//button[@aria-label='Yes']";
                public const string No              = "//button[@aria-label='No']";
                public const string NeboLink        = "id('security-systems-multi-choice-input-Nebolink')";
                public const string AlarmOrGps      = "id('security-systems-multi-choice-input-Alarm_or_GPS_tracker')";
                public const string TrailerDevice   = "id('security-systems-multi-choice-input-Trailer_security_device')";
                public const string NoSecurity      = "id('security-systems-multi-choice-input-No_security')";
                public const string NextPage        = "//button[@type='submit']";
            }
            public class AdviseUser
            {
                public class FieldValidation
                {
                    public const string EmptySuburb     = "//p[contains(text(),'enter a suburb')]";
                    public const string EmptyGarage     = "//p[contains(text(),'select Yes or No')]";
                    public const string EmptyMotor      = "//p[contains(text(),'select one')]";
                    public const string EmptySecurity   = "//p[contains(text(),'select all options that apply')]";
                }
                public class HelpText
                {
                    public static readonly string SecurityShow      = "id('security-systems-tooltipButton')";
                    public static readonly string SecurityHide      = "id('security-systems-tooltip-close')";
                    public static readonly string SecurityTitle     = "id('security-systems-tooltip-title')";
                    public static readonly string SecurityBody      = "id('security-systems-tooltip-message')";
                    public static readonly string NebolinkUrl       = "id('security-system-tooltip-nebolink-link')";
                }
            }
        }
        #endregion


        #region Settable properties and controls
        private string Suburb
        {
            get => GetValue(XPath.Field.Suburb);
            set => WaitForSelectableAndPickFromDropdown(XPath.Field.Suburb, XPath.Field.SuburbOptions, value);
        }
        public bool KeptInGarage
        {
            get => GetBinaryToggleState(XPath.Field.KeptInGarage, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.Field.KeptInGarage, XPath.Button.Yes, XPath.Button.No, value);
        }

        public string MotorType
        {
            get => GetElement(XPath.Field.TypeOfMotor).GetAttribute("value");
            set => WaitForSelectableAndPickFromDropdown(XPath.Field.TypeOfMotor, XPath.Field.TypeOfMotorOptions, value);
        }
        #endregion

        public SparkBoatMoreBoat(Browser browser) : base(browser)
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
            { Reporting.LogPageChange("Spark Boat Quote page ? - MoreBoat"); }

            return isDisplayed;
        }

        /// <summary>
        /// Invokes worker methods to exercise assertions against the page
        /// and update the fields.
        /// </summary>
        /// <param name="quoteBoat">Data generated for this quote.</param>
        public void VerifyPageContent()
        {
            Reporting.AreEqual(Constants.PageHeader,
                GetElement(XPath.PageHeader).Text, "page header content against expected value");

            Reporting.AreEqual(Sidebar.Link.PdsUrl,
                GetElement(XPaths.Sidebar.PdsLink).GetAttribute("href"), "NPE Sidebar PDS URL");

            VerifyStandardHeaderAndFooterContent();
            VerifyFieldValidations();
            ValidateLabels();
            ValidateHelpText();

            ScrollElementIntoView(XPathBaseBoat.AdviseUser.BoatFaqCard.Body);
            Reporting.Log($"Capturing snapshot of Boat FAQ card.", _browser.Driver.TakeSnapshot());
            VerifyBoatFAQContent();
        }

        private void VerifyFieldValidations()
        {
            Reporting.LogMinorSectionHeading($"Selecting 'View quote' button to trigger display of empty field validation error content.");
            ClickControl(XPath.Button.NextPage);

            Reporting.AreEqual(Constants.AdviseUser.FieldValidation.EmptySuburb,
                GetElement(XPath.AdviseUser.FieldValidation.EmptySuburb).Text, "expected vs actual field validation text when no option is selected for the 'Suburb' field.");

            Reporting.AreEqual(AdviseUser.FieldValidation.YesNoToggle,
                GetElement(XPath.AdviseUser.FieldValidation.EmptyGarage).Text, "expected vs actual field validation text when no option is selected for the 'Garage' field.");

            Reporting.AreEqual(AdviseUser.FieldValidation.SelectOne,
                GetElement(XPath.AdviseUser.FieldValidation.EmptyMotor).Text, "expected vs actual field validation text when no option is selected for the 'Motor' field.");

            Reporting.AreEqual(Constants.AdviseUser.FieldValidation.EmptySecurity,
                GetElement(XPath.AdviseUser.FieldValidation.EmptySecurity).Text, "expected vs actual field validation text when no options are selected for the 'Security' field.");

            Reporting.Log($"Capturing screen state before continuing", _browser.Driver.TakeSnapshot());
        }

        private void ValidateLabels()
        {
            Reporting.AreEqual(Constants.Field.Label.Suburb,
                GetElement(XPath.Field.Label.Suburb).Text, "expected vs actual field title on the 'Suburb' field.");

            Reporting.AreEqual(Constants.Field.Label.Garage,
                GetElement(XPath.Field.Label.Garage).Text, "expected vs actual field title on the 'Garage' field.");

            Reporting.AreEqual(Constants.Field.Label.Motor,
                GetElement(XPath.Field.Label.Motor).Text, "expected vs actual field title on the 'Motor' field.");

            Reporting.AreEqual(Constants.Field.Label.Security,
                GetElement(XPath.Field.Label.Security).Text, "expected vs actual field Title & Sub-Title on the 'Security' field.");

            Reporting.AreEqual(Constants.Field.Label.SecuritySub,
            GetElement(XPath.Field.Label.SecuritySub).Text, "expected vs actual field Sub-Title ONLY on the 'Security' field.");
        }
        private void ValidateHelpText()
        {
            ClickControl(XPath.AdviseUser.HelpText.SecurityShow);

            Reporting.AreEqual(Constants.AdviseUser.HelpText.SecurityTitle, GetInnerText(XPath.AdviseUser.HelpText.SecurityTitle), 
                "expected Title of Security help text against actual");
            Reporting.AreEqual(Constants.AdviseUser.HelpText.SecurityBody, GetInnerText(XPath.AdviseUser.HelpText.SecurityBody),
                "expected Body of Security help text against actual");

            Reporting.Log("Capturing snapshot of Help Text before closing it", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.AdviseUser.HelpText.SecurityHide);
        }

        public void PopulateFields(Browser browser, QuoteBoat quoteBoat)
        {
            Reporting.LogMinorSectionHeading($"Populating fields.");

            var suburb = "West Perth - 6005";
            Suburb = suburb;

            if (quoteBoat.IsGaraged)
            {
                KeptInGarage = true;
            }
            else
            {
                KeptInGarage = false;
            }

            MotorType = quoteBoat.SparkBoatMotorType.GetDescription();

            if (quoteBoat.SecurityAlarmGps)
            {
                ClickControl(XPath.Button.AlarmOrGps);
            }
            
            if (quoteBoat.SecurityNebo)
            { 
                ClickControl(XPath.Button.NeboLink);
            }
            
            if (quoteBoat.SecurityHitch)
            {
            ClickControl(XPath.Button.TrailerDevice);
            }

            if (!quoteBoat.SecurityAlarmGps && !quoteBoat.SecurityNebo && !quoteBoat.SecurityHitch)
            {
                ClickControl(XPath.Button.NoSecurity);
            }
            
            Reporting.Log($"Capturing screen state after input.", _browser.Driver.TakeSnapshot());
        }

        public void ContinueToYourQuote()
        {
            ClickControl(XPath.Button.NextPage);
        }
    }
}