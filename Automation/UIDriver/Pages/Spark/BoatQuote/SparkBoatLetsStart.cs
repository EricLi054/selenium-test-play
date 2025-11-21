using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Threading;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace UIDriver.Pages.Spark.BoatQuote
{
    public class SparkBoatLetsStart : BaseBoatPage
    {
        #region XPATHS
        private class XPath
        {
            public const string PageHeader                              = "//*[@id='header']";
            public const string ActiveStepper                           = "//button[@aria-selected='true']";
            public const string SubHeader                               = "id('subheader')";
            public const string StoredInWaterControl                    = "id('waterStored')";
            public const string BusinessOrCommercialControl             = "id('commercialUse')";
            public const string HouseboatOrPersonalWatercraftControl    = "id('personalWatercraft')";
            public const string HighValueControl                        = "id('highValue')";

            public class Button
            {
                public const string StoredInWaterHelpTextButton             = "//button[@id='tooltip-waterStored-show-button']";
                public const string StoredInWaterHelpTextClose              = "//button[@id='tooltip-waterStored-close']";
                public const string BusinessOrCommercialHelpTextButton      = "//button[@id='tooltip-commercialUse-show-button']";
                public const string BusinessOrCommercialHelpTextClose       = "//button[@id='tooltip-commercialUse-close']";
                public const string Yes                                     = "//button[@aria-label='Yes']";
                public const string No                                      = "//button[@aria-label='No']";
                public const string NextPage                                = "//button[@type='submit']";
            }
            public class AdviseUser
            {
                public class Helptext
                {
                    public const string StoredInWaterHelpTextTitle              = "id('tooltip-waterStored-title')";
                    public const string StoredInWaterHelpTextMessage            = "id('tooltip-waterStored-message')";
                    public const string BusinessOrCommercialHelpTextTitle       = "id('tooltip-commercialUse-title')";
                    public const string BusinessOrCommercialHelpTextMessage     = "id('tooltip-commercialUse-message')";
                }
                public class FieldValidation
                {
                    public const string WaterStored         = "//div[@id='waterStored']" +
                        "/following-sibling::p[contains(text(), 'select Yes or No')]";
                    public const string CommercialUse       = "//div[@id='commercialUse']" +
                        "/following-sibling::p[contains(text(), 'select Yes or No')]";
                    public const string PersonalWatercraft  = "//div[@id='personalWatercraft']" +
                        "/following-sibling::p[contains(text(), 'select Yes or No')]";
                    public const string HighValue           = "//div[@id='highValue']" +
                        "/following-sibling::p[contains(text(), 'select Yes or No')]";
                }
                public class DeclinedCover
                {
                    public const string StoredInWaterTitle                      = "id('water-stored-notification-title')";
                    public const string StoredInWaterMessageBody1               = "id('water-stored-notification-body-1')";
                    public const string StoredInWaterMessageBody2               = "id('water-stored-notification-body-2')";
                    public const string StoredInWaterMessageBody3               = "id('water-stored-notification-body-3')";
                    public const string FindAnInsurerText                       = "//p[contains(text(),'findaninsurer')]";
                    public const string FindAnInsurerCallToActionLink           = "//a[contains(@href,'https://findaninsurer.com.au')]";
                    public const string BusinessOrCommercialTitle               = "id('commercial-use-notification-title')";
                    public const string BusinessOrCommercialMessageBody1        = "id('commercial-use-notification-body-1')";
                    public const string BusinessOrCommercialMessageBody2        = "id('commercial-use-notification-body-2')";
                    public const string BusinessOrCommercialMessageBody3        = "id('commercial-use-notification-body-3')";
                    public const string HouseboatOrPersonalWatercraftTitle      = "id('personal-watercraft-notification-title')";
                    public const string HouseboatOrPersonalWatercraftMsgBody1   = "id('personal-watercraft-notification-body-1')";
                    public const string HouseboatOrPersonalWatercraftMsgBody2   = "id('personal-watercraft-notification-body-2')";
                    public const string HouseboatOrPersonalWatercraftMsgBody3   = "id('personal-watercraft-notification-body-3')";
                    public const string HighValueTitle                          = "id('high-value-notification-title')";
                    public const string HighValueMessage                        = "id('high-value-notification-body-1')";
                    public const string PhoneNumberCallToAction                 = "//a[contains(text(),'13 17 03')]";
                }
            }
        }
        #endregion

        #region Constants
        private class Constants
        {
            public const string PageHeader          = "Let's start your quote";
            public const string ActiveStepperLabel  = "Let's start";
            public const string SubHeader           = "Is your boat...";
            public class AdviseUser
            {
                public class Helptext
                {
                    public const string StoredInWaterHelpTextTitle          = "Stored in the water";
                    public const string StoredInWaterHelpTextMessage        = "This is when your boat is secured to a fixed object " +
                                                                                "such as a marina, jetty, quay, wharf or anchor buoy. " +
                                                                                "It includes boats that are penned or in dry storage " +
                                                                                "for part of the year.";
                    public const string BusinessOrCommercialHelpTextTitle   = "Business or commercial use";
                    public const string BusinessOrCommercialHelpTextMessage = "This includes boats that are hired out or that " +
                                                                                "deliver goods or carry passengers for a fee.";
                }
                public class DeclinedCover
                {
                    public const string StoredInWaterTitle                      = "Sorry, we can't insure you";
                    public const string StoredInWaterMessageBody1               = "We don't insure boats that are stored in the water.";
                    public const string StoredInWaterMessageBody2               = "If you have any questions about this, please call us on 13 17 03.";
                    public const string StoredInWaterMessageBody3               = "Visit findaninsurer.com.au to find insurance for your boat.";
                    public const string FindAnInsurerCallToActionUrl            = "https://findaninsurer.com.au/";
                    public const string BusinessOrCommercialTitle               = "Sorry, we can't insure you";
                    public const string BusinessOrCommercialMessageBody1        = "We don't insure boats used for business or commercial purposes.";
                    public const string BusinessOrCommercialMessageBody2        = "If you have any questions about this, please call us on 13 17 03.";
                    public const string BusinessOrCommercialMessageBody3        = "Visit findaninsurer.com.au to find insurance for your boat.";
                    public const string HouseboatOrPersonalWatercraftTitle      = "Sorry, we can't insure you";
                    public const string HouseboatOrPersonalWatercraftMsgBody1   = "We don't insure houseboats or personal watercrafts.";
                    public const string HouseboatOrPersonalWatercraftMsgBody2   = "If you have any questions about this, please call us on 13 17 03.";
                    public const string HouseboatOrPersonalWatercraftMsgBody3   = "Visit findaninsurer.com.au to find insurance for your boat.";
                    public const string HighValueTitle                          = "Sorry, we can't help you online";
                    public const string HighValueMessage                        = "Please call us on 13 17 03 to discuss your quote.";
                    public const string PhoneNumberCallToAction                 = "tel:131703";
                }
            }
        }
        #endregion

        #region Settable properties and controls
        public bool AnswerIsStoredInWater
        {
            get => GetBinaryToggleState(XPath.StoredInWaterControl, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.StoredInWaterControl, XPath.Button.Yes, XPath.Button.No, value);
        }
        public bool AnswerIsForBusinessOrCommercialUse
        {
            get => GetBinaryToggleState(XPath.BusinessOrCommercialControl, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.BusinessOrCommercialControl, XPath.Button.Yes, XPath.Button.No, value);
        }
        public bool AnswerIsHouseboatOrPersonalWatercraft
        {
            get => GetBinaryToggleState(XPath.HouseboatOrPersonalWatercraftControl, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.HouseboatOrPersonalWatercraftControl, XPath.Button.Yes, XPath.Button.No, value);
        }
        public bool AnswerIsHighValue
        {
            get => GetBinaryToggleState(XPath.HighValueControl, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.HighValueControl, XPath.Button.Yes, XPath.Button.No, value);
        }
        public bool DoesNextPageButtonAppearEnabled => IsControlEnabled(XPath.Button.NextPage);
        #endregion
        public SparkBoatLetsStart(Browser browser) : base(browser)
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
            { Reporting.LogPageChange("Spark Boat Quote page 1 - Let's start"); }

            return isDisplayed;
        }

        /// <summary>
        /// Triggers detailed verification of page content, only invoked when detailedUiChecking = true.
        /// </summary>
        public void VerifyPageContent()
        {
            Reporting.AreEqual(Constants.PageHeader,
                GetElement(XPath.PageHeader).Text, "expected page header with actual.");

            Reporting.AreEqual(Constants.SubHeader,
                GetElement(XPath.SubHeader).Text, "expected page sub-heading with actual.");

            Reporting.AreEqual(Sidebar.Link.PdsUrl,
                    GetElement(XPaths.Sidebar.PdsLink).GetAttribute("href"), "NPE Sidebar PDS URL");

            VerifyStandardHeaderAndFooterContent();
            VerifyFieldValidation();
            VerifyHelpText();
            VerifyDeclineCoverContent();
            
            ScrollElementIntoView(XPathBaseBoat.AdviseUser.BoatFaqCard.Body);
            Reporting.Log($"Capturing snapshot of Boat FAQ card.", _browser.Driver.TakeSnapshot());
            VerifyBoatFAQContent();
        }
        /// <summary>
        /// Trigger the empty field verification errors for each of the fields on this page, then confirm they are 
        /// dismissed as each field receives input.
        /// </summary>
        public void VerifyFieldValidation()
        {
            Reporting.LogMinorSectionHeading("Verify Unanswered Field Validation Errors");
            
            Reporting.Log($"Selecting Next to trigger Field Validation errors.");
            ClickControl(XPath.Button.NextPage);
            _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.FieldValidation.WaterStored), WaitTimes.T5SEC);
            Reporting.Log($"Capturing image of Field Validation errors.", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(AdviseUser.FieldValidation.YesNoToggle, 
                GetInnerText(XPath.AdviseUser.FieldValidation.WaterStored), "expected field validation error for the 'Usually stored in the water?' field");
            AnswerIsStoredInWater = false;
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.FieldValidation.WaterStored), WaitTimes.T5SEC);
            Reporting.Log($"Capturing image confirming validation error for the 'Usually stored in the water?' field has been dismissed.", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(AdviseUser.FieldValidation.YesNoToggle,
                GetInnerText(XPath.AdviseUser.FieldValidation.CommercialUse), "expected field validation error for the 'Used for business or commercial purposes?' field");
            AnswerIsForBusinessOrCommercialUse = false;
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.FieldValidation.CommercialUse), WaitTimes.T5SEC);
            Reporting.Log($"Capturing image confirming validation error for the 'Used for business or commercial purposes?' field has been dismissed.", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(AdviseUser.FieldValidation.YesNoToggle,
                GetInnerText(XPath.AdviseUser.FieldValidation.PersonalWatercraft), "expected field validation error for the 'A houseboat or personal watercraft, e.g. jet ski?' field");
            AnswerIsHouseboatOrPersonalWatercraft = false;
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.FieldValidation.PersonalWatercraft), WaitTimes.T5SEC);
            Reporting.Log($"Capturing image confirming validation error for the 'A houseboat or personal watercraft, e.g. jet ski?' field has been dismissed.", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(AdviseUser.FieldValidation.YesNoToggle,
                GetInnerText(XPath.AdviseUser.FieldValidation.HighValue), "expected field validation error for the 'Valued at more than $150,000?' field");
            AnswerIsHighValue = false;
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.FieldValidation.HighValue), WaitTimes.T5SEC);
            Reporting.Log($"Capturing image confirming validation error for the 'Valued at more than $150,000?' field has been dismissed.", _browser.Driver.TakeSnapshot());
        }
        private void VerifyHelpText()
        {
            ClickControl(XPath.Button.StoredInWaterHelpTextButton);
            Reporting.LogMinorSectionHeading($"Selecting help icon for Stored in water field to display help text.");
            _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.Helptext.StoredInWaterHelpTextTitle), WaitTimes.T5SEC);
            
            Reporting.AreEqual(Constants.AdviseUser.Helptext.StoredInWaterHelpTextTitle, 
                                GetInnerText(XPath.AdviseUser.Helptext.StoredInWaterHelpTextTitle), "Stored in water help text title");
            
            Reporting.AreEqual(Constants.AdviseUser.Helptext.StoredInWaterHelpTextMessage, 
                                GetInnerText(XPath.AdviseUser.Helptext.StoredInWaterHelpTextMessage), "Stored in water help text message");
            
            Reporting.Log($"Capturing Stored in water help text before closing it.", _browser.Driver.TakeSnapshot());
            
            ClickControl(XPath.Button.StoredInWaterHelpTextClose);
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.Helptext.StoredInWaterHelpTextTitle), WaitTimes.T5SEC);

            ClickControl(XPath.Button.BusinessOrCommercialHelpTextButton);
            Reporting.LogMinorSectionHeading($"Selecting help icon for Business or Commercial Use field to display help text.");
            _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.Helptext.BusinessOrCommercialHelpTextTitle), WaitTimes.T5SEC);
            
            Reporting.AreEqual(Constants.AdviseUser.Helptext.BusinessOrCommercialHelpTextTitle,
                                GetInnerText(XPath.AdviseUser.Helptext.BusinessOrCommercialHelpTextTitle), "Business or Commercial Use help text title");

            Reporting.AreEqual(Constants.AdviseUser.Helptext.BusinessOrCommercialHelpTextMessage,
                                GetInnerText(XPath.AdviseUser.Helptext.BusinessOrCommercialHelpTextMessage), "Business or Commercial Use help text message");
            
            Reporting.Log($"Capturing Business or Commercial Use help text before closing it.", _browser.Driver.TakeSnapshot());
            
            ClickControl(XPath.Button.BusinessOrCommercialHelpTextClose);
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.Helptext.BusinessOrCommercialHelpTextTitle), WaitTimes.T5SEC);

            Reporting.LogMinorSectionHeading("Confirming help text is dismissed if user clicks elsewhere.");
            
            ClickControl(XPath.Button.StoredInWaterHelpTextButton);
            _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.Helptext.StoredInWaterHelpTextTitle), WaitTimes.T5SEC);
            Reporting.IsTrue(GetElement(XPath.AdviseUser.Helptext.StoredInWaterHelpTextTitle).Displayed, 
                "Helptext for Stored in water field should be visible after selecting it again");

            Reporting.Log("Selecting the Help Text button for Business or Commercial Use to dismiss the help text for the Stored in water field " +
                "and display the Help Text for the Business or Commercial Use field instead");
            
            ClickControl(XPath.Button.BusinessOrCommercialHelpTextButton);
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.Helptext.StoredInWaterHelpTextTitle), WaitTimes.T5SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.Helptext.BusinessOrCommercialHelpTextTitle), WaitTimes.T5SEC);
            
            Reporting.IsTrue(GetElement(XPath.AdviseUser.Helptext.BusinessOrCommercialHelpTextTitle).Displayed, 
                "Helptext for Business or Commercial Use field should be visible");
            
            Reporting.Log("Selecting the active 'Let's start' Stepper to dismiss the help text for Business or Commercial Use field");
            ClickControl(XPath.ActiveStepper);
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.Helptext.BusinessOrCommercialHelpTextTitle), WaitTimes.T5SEC);
        }
        private void VerifyDeclineCoverContent()
        {
            Reporting.LogMinorSectionHeading("Begin verification of DECLINED COVER content");
            Reporting.IsTrue(DoesNextPageButtonAppearEnabled, "Next page button is available for selection prior to any user input to the Let's start fields.");

            DeclinedCoverContentStoredInWater();
            
            DeclinedCoverContentBusinessOrCommercialUse();
            
            DeclinedCoverContentHouseboatOrPersonalWatercraft();
            
            DeclinedCoverContentHighValue();

            Reporting.LogMinorSectionHeading($"Selecting Yes to all questions to render all Declined Cover messaging for screenshots");
            AnswerIsHighValue = true;
            AnswerIsHouseboatOrPersonalWatercraft = true;
            AnswerIsForBusinessOrCommercialUse = true;
            AnswerIsStoredInWater = true;
            
            ScrollElementIntoView(XPath.HouseboatOrPersonalWatercraftControl);
            Reporting.Log("Scrolling down page to maximise Declined Cover messaging content visible for snapshot and possible percy.io check.", _browser.Driver.TakeSnapshot());
            _browser.PercyScreenCheck(BoatNewBusiness.LetsStartDeclinedCover);

            Reporting.IsTrue(DoesNextPageButtonAppearEnabled, "Next page button should appear to be enabled even when Decline Cover is present as per SPK-3461");
            ExpectToFailToProgress();

            Reporting.LogMinorSectionHeading($"Setting all questions to No to hide all declined Cover messaging content and allow progress through flow");
            AnswerIsStoredInWater = false;
            AnswerIsForBusinessOrCommercialUse = false;
            AnswerIsHouseboatOrPersonalWatercraft = false;
            AnswerIsHighValue = false;
            
            Reporting.IsTrue(DoesNextPageButtonAppearEnabled, "Next page button should still appear to be available");
        }

        private void DeclinedCoverContentStoredInWater()
        {
            AnswerIsStoredInWater = true;
            Reporting.LogMinorSectionHeading("Selected Stored in water = Yes to show Declined Cover messaging");
            
            Reporting.IsTrue(DoesNextPageButtonAppearEnabled, "Next page button should appear to be enabled even when Decline Cover is present as per SPK-3461");
            ExpectToFailToProgress();

            _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.DeclinedCover.StoredInWaterTitle), WaitTimes.T5SEC);
            Reporting.Log("Snapshot of Declined Cover messaging content before verification.", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.StoredInWaterTitle,
                GetElement(XPath.AdviseUser.DeclinedCover.StoredInWaterTitle).Text, "Stored in water Declined Cover title");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.StoredInWaterMessageBody1,
                GetElement(XPath.AdviseUser.DeclinedCover.StoredInWaterMessageBody1).Text, "Stored in water Declined Cover first paragraph");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.StoredInWaterMessageBody2,
                GetElement(XPath.AdviseUser.DeclinedCover.StoredInWaterMessageBody2).Text, "Stored in water Declined Cover second paragraph");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.StoredInWaterMessageBody3,
                GetElement(XPath.AdviseUser.DeclinedCover.StoredInWaterMessageBody3).Text, "Stored in water Declined Cover third paragraph");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.PhoneNumberCallToAction,
                GetElement(XPath.AdviseUser.DeclinedCover.PhoneNumberCallToAction).GetAttribute("href"), "phone number link in call to action for Stored in water");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.FindAnInsurerCallToActionUrl,
                GetElement(XPath.AdviseUser.DeclinedCover.FindAnInsurerCallToActionLink).GetAttribute("href"), "findaninsurer.com.au call to action URL for Stored in water");
            
            AnswerIsStoredInWater = false;
            Reporting.LogMinorSectionHeading($"Selected Stored in water = No to hide Declined Cover messaging");
            Reporting.IsTrue(DoesNextPageButtonAppearEnabled, "Next page button should still appear to be available");
        }

        private void DeclinedCoverContentBusinessOrCommercialUse()
        {
            AnswerIsForBusinessOrCommercialUse = true;
            Reporting.LogMinorSectionHeading("Selected Business or commercial use = Yes to show Declined Cover messaging");
            
            Reporting.IsTrue(DoesNextPageButtonAppearEnabled, "Next page button should appear to be enabled even when Decline Cover is present as per SPK-3461");
            ExpectToFailToProgress();

            _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.DeclinedCover.BusinessOrCommercialTitle), WaitTimes.T5SEC);
            Reporting.Log("Snapshot of Declined Cover messaging content before verification.", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.BusinessOrCommercialTitle,
                GetElement(XPath.AdviseUser.DeclinedCover.BusinessOrCommercialTitle).Text, "Business or commercial use Declined Cover title");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.BusinessOrCommercialMessageBody1,
                GetElement(XPath.AdviseUser.DeclinedCover.BusinessOrCommercialMessageBody1).Text, "Business or commercial use Declined Cover first paragraph");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.BusinessOrCommercialMessageBody2,
                GetElement(XPath.AdviseUser.DeclinedCover.BusinessOrCommercialMessageBody2).Text, "Business or commercial use Declined Cover second paragraph");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.BusinessOrCommercialMessageBody3,
                GetElement(XPath.AdviseUser.DeclinedCover.BusinessOrCommercialMessageBody3).Text, "Business or commercial use Declined Cover third paragraph");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.PhoneNumberCallToAction,
                GetElement(XPath.AdviseUser.DeclinedCover.PhoneNumberCallToAction).GetAttribute("href"), "phone number link in call to action for Business or commercial use");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.FindAnInsurerCallToActionUrl,
                GetElement(XPath.AdviseUser.DeclinedCover.FindAnInsurerCallToActionLink).GetAttribute("href"), "findaninsurer.com.au call to action URL for Business or commercial use");
            
            AnswerIsForBusinessOrCommercialUse = false;
            Reporting.LogMinorSectionHeading($"Selected Business or commercial use = No to hide Declined Cover messaging");
            Reporting.IsTrue(DoesNextPageButtonAppearEnabled, "Next page button should still appear to be available");
        }

        private void DeclinedCoverContentHouseboatOrPersonalWatercraft()
        {
            AnswerIsHouseboatOrPersonalWatercraft = true;
            Reporting.LogMinorSectionHeading($"Selected Houseboat or Personal Watercraft = Yes to show Declined Cover messaging");
            
            Reporting.IsTrue(DoesNextPageButtonAppearEnabled, "Next page button should appear to be enabled even when Decline Cover is present as per SPK-3461");
            ExpectToFailToProgress();

            _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.DeclinedCover.HouseboatOrPersonalWatercraftTitle), WaitTimes.T5SEC);
            Reporting.Log("Snapshot of Declined Cover messaging content before verification.", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.HouseboatOrPersonalWatercraftTitle,
                GetElement(XPath.AdviseUser.DeclinedCover.HouseboatOrPersonalWatercraftTitle).Text, "Houseboat or Personal Watercraft Declined Cover title");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.HouseboatOrPersonalWatercraftMsgBody1,
                GetElement(XPath.AdviseUser.DeclinedCover.HouseboatOrPersonalWatercraftMsgBody1).Text, "Houseboat or Personal Watercraft Declined Cover message first paragraph");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.HouseboatOrPersonalWatercraftMsgBody2,
                GetElement(XPath.AdviseUser.DeclinedCover.HouseboatOrPersonalWatercraftMsgBody2).Text, "Houseboat or Personal Watercraft Declined Cover message second paragraph");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.HouseboatOrPersonalWatercraftMsgBody3,
                GetElement(XPath.AdviseUser.DeclinedCover.HouseboatOrPersonalWatercraftMsgBody3).Text, "Houseboat or Personal Watercraft Declined Cover message third paragraph");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.PhoneNumberCallToAction,
                GetElement(XPath.AdviseUser.DeclinedCover.PhoneNumberCallToAction).GetAttribute("href"), "phone number link in call to action for Houseboat or Personal Watercraft");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.FindAnInsurerCallToActionUrl,
                GetElement(XPath.AdviseUser.DeclinedCover.FindAnInsurerCallToActionLink).GetAttribute("href"), "findaninsurer.com.au call to action URL for Houseboat or Personal Watercraft");
            
            AnswerIsHouseboatOrPersonalWatercraft = false;
            Reporting.LogMinorSectionHeading($"Selected Houseboat or Personal Watercraft = No to hide Declined Cover messaging");
            Reporting.IsTrue(DoesNextPageButtonAppearEnabled, "Next page button should still appear to be available");
        }

        private void DeclinedCoverContentHighValue()
        {
            AnswerIsHighValue = true;
            Reporting.LogMinorSectionHeading($"Selected High Value = Yes to show Declined Cover messaging");
            
            Reporting.IsTrue(DoesNextPageButtonAppearEnabled, "Next page button should appear to be enabled even when Decline Cover is present as per SPK-3461");
            ExpectToFailToProgress();

            _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.DeclinedCover.HighValueTitle), WaitTimes.T5SEC);
            Reporting.Log("Snapshot of Declined Cover messaging content before verification.", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.HighValueTitle,
                GetElement(XPath.AdviseUser.DeclinedCover.HighValueTitle).Text, "High value Declined Cover title");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.HighValueMessage,
                GetElement(XPath.AdviseUser.DeclinedCover.HighValueMessage).Text, "High value Declined Cover message");
            
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.PhoneNumberCallToAction,
                GetElement(XPath.AdviseUser.DeclinedCover.PhoneNumberCallToAction).GetAttribute("href"), "phone number link in call to action for High value");

            AnswerIsHighValue = false;
            Reporting.LogMinorSectionHeading($"Selected High Value = No to hide Declined Cover messaging");
            Reporting.IsTrue(DoesNextPageButtonAppearEnabled, "Next page button should still appear to be available");
        }

        /// <summary>
        /// Check that when the user selects the Next button which APPEARS to be enabled, that the user is actually unable
        /// to proceed to the next page. Only to be invoked when there is some blocking condition in place such as incomplete
        /// fields, Decline Cover messages displayed etc.
        /// </summary>
        public void ExpectToFailToProgress()
        {
            Reporting.Log($"Selecting '{GetInnerText(XPath.Button.NextPage)}' despite blocker to check whether progress is blocked.");
            ClickControl(XPath.Button.NextPage);
            Thread.Sleep(SleepTimes.T5SEC);
            Reporting.AreEqual(Constants.ActiveStepperLabel,
                    GetElement(XPath.ActiveStepper).Text, " value of active step is still 'Let's start' and user has not been able to " +
                    "progress to the next page.");
        }

        public void ContinueToAboutYou()
        {
            AnswerIsStoredInWater = false;
            AnswerIsForBusinessOrCommercialUse = false;
            AnswerIsHouseboatOrPersonalWatercraft = false;
            AnswerIsHighValue = false;
            Reporting.Log("Snapshot capturing screen state as we move on.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.NextPage);
        }
    }
}
