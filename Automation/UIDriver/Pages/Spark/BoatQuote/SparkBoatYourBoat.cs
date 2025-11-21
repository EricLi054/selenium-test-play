using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Globalization;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;


namespace UIDriver.Pages.Spark.BoatQuote
{
    public class SparkBoatYourBoat : BaseBoatPage
    {
        #region Constants
        private class Constants
        {
            public const string PageHeader = "Your boat";
            public const string ActiveStepperLabel = "Your boat";
            public class AdviseUser
            {
                public class Helptext
                {
                    public const string ValueTitle      = "Value of boat";
                    public const string ValueMessage    = "This includes:\r\n" +
                                                        "The hull, motors, masts, spars, rigging, sails, trailer and accessories.\r\n" +
                                                        "Accessories: safety equipment, anchors, oars, paddles, detachable canopies, " +
                                                        "spare propeller, two-way radios, depth sounders, GPS devices, covers and equipment " +
                                                        "that's normally demountable. This excludes personal effects, fishing tackle and " +
                                                        "waterski equipment.";
                }
                public class FieldValidation
                {
                    public const string Make        = "Please select an option";
                    public const string Year        = "Please enter a valid year";
                    public const string Financier   = "Please enter a financier";
                }
                public class DeclinedCover
                {
                    public const string ValueTooHighTitle   = "Please enter a value under $150,000";
                    public const string ValueTooHighBody    = "If your boat is valued over $150,000, please call us on 13 17 03.";
                    public const string ValueTooHighLink    = "tel:131703";
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
                public const string Make                = "id('boat-make-autocomplete')";
                public const string OtherMake           = "id('other-boat-make-autocomplete')";
                public const string Year                = "id('boat-year')";
                public const string HullControl         = "id('your-boat-hull-select')";
                public const string HullOptions         = "//ul[@role='listbox']" + "//li";
                public const string Value               = "id('agreedValue')";
                public const string Financier           = "id('boat-financier-autocomplete')";
            }
            public class Button
            {
                public const string ValueHelpText       = "//button[@data-testid='value-boat-tooltip']";
                public const string ValueHelpTextClose  = "id('value-boat-tooltip-close')";
                public const string Finance             = "id('finance-yes-no-group')";
                public const string Yes                 = "//button[@aria-label='Yes']";
                public const string No                  = "//button[@aria-label='No']";
                public const string NextPage            = "//button[@type='submit']";
            }
            public class AdviseUser
            {
                public class Helptext
                {
                    public const string ValueTitle      = "id('value-boat-tooltip-title')";
                    public const string ValueMessage    = "id('value-boat-tooltip-message')";
                }
                public class FieldValidation
                {
                    public const string Make        = "//p[contains(text(),'an option')]";
                    public const string Year        = "//p[contains(text(),'a valid year')]";
                    public const string Hull        = "//p[contains(text(),'select one')]";
                    public const string Value       = "//p[contains(text(),'enter a value')]";
                    public const string Financed    = "//p[contains(text(),'select Yes or No')]";
                    public const string Financier   = "//p[contains(text(),'enter a financier')]";
                }
                public class DeclinedCover
                {
                    public const string ValueTooHighTitle   = "id('amount-notification-title')";
                    public const string ValueTooHighBody    = "id('amount-notification-body-1')";
                    public const string ValueTooHighLink    = "id('link-1')";
                }
            }
        }
        #endregion

        #region Settable properties and controls
        public string Make
        {
            get => GetElement(XPath.Field.Make).GetAttribute("value");
            set => WaitForTextFieldAndEnterText(XPath.Field.Make, value, true);
        }

        public string OtherMake
        {
            get => GetElement(XPath.Field.OtherMake).GetAttribute("value");
            set => WaitForTextFieldAndEnterText(XPath.Field.OtherMake, value, false);
        }

        public string Year
        {
            get => GetElement(XPath.Field.Year).GetAttribute("value");
            set => WaitForTextFieldAndEnterText(XPath.Field.Year, value, true);
        }

        public string Hull
       {
            get => GetSelectedTextFromDropDown(XPath.Field.HullControl);
            set => WaitForSelectableAndPickFromDropdown(XPath.Field.HullControl, XPath.Field.HullOptions, value);
       }

        public string Value
        {
            get => GetElement(XPath.Field.Value).GetAttribute("value");
            set => SendKeyPressesAfterClearingExistingTextInField(XPath.Field.Value, value);
        }

        public bool IsFinanced
        {
            get => GetBinaryToggleState(XPath.Button.Finance, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.Button.Finance, XPath.Button.Yes, XPath.Button.No, value);
        }
        public string Financier
        {
            get => GetElement(XPath.Field.Financier).GetAttribute("value");
            set => WaitForTextFieldAndEnterText(XPath.Field.Financier, value, true);
        }
        #endregion
        public SparkBoatYourBoat(Browser browser) : base(browser)
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
            { Reporting.LogPageChange("Spark Boat Quote page ? - YourBoat"); }

            return isDisplayed;
        }

        /// <summary>
        /// Invokes worker methods to exercise assertions against the page.
        /// Only invoked when detailedUiChecking = true.
        /// </summary>
        /// <param name="insuredAmount">Used to restore the Sum Insured to the desired value after triggering validation errors in FieldValidationInvalidValue</param>
        public void VerifyPageContent(int insuredAmount)
        {
            Reporting.AreEqual(Constants.PageHeader,
                GetElement(XPath.PageHeader).Text, "expected page header with actual.");

            Reporting.AreEqual(Sidebar.Link.PdsUrl,
                GetElement(XPaths.Sidebar.PdsLink).GetAttribute("href"), "NPE Sidebar PDS URL");

            VerifyStandardHeaderAndFooterContent();
            VerifyHelpText();
            VerifyFieldValidation(insuredAmount);

            ScrollElementIntoView(XPathBaseBoat.AdviseUser.BoatFaqCard.Body);
            Reporting.Log($"Capturing snapshot of Boat FAQ card.", _browser.Driver.TakeSnapshot());
            VerifyBoatFAQContent();
        }

        private void VerifyHelpText()
        {
            Reporting.LogMinorSectionHeading($"Selecting help icon for Value field to display help text.");
            ClickControl(XPath.Button.ValueHelpText);
            _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.Helptext.ValueTitle), WaitTimes.T5SEC);
            
            Reporting.AreEqual(Constants.AdviseUser.Helptext.ValueTitle,
                GetInnerText(XPath.AdviseUser.Helptext.ValueTitle), "expected title for Value help text against actual");

            Reporting.AreEqual(Constants.AdviseUser.Helptext.ValueMessage,
                GetInnerText(XPath.AdviseUser.Helptext.ValueMessage), "expected message for Value help text against actual");

            Reporting.Log($"Capturing visual of {Constants.AdviseUser.Helptext.ValueTitle} help text", _browser.Driver.TakeSnapshot());

            ClickControl(XPath.Button.ValueHelpTextClose);
        }
        /// <summary>
        /// Verify field validation error messages on the Your quote page.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="insuredAmount">Used to restore the Sum Insured to the desired value after triggering validation errors in FieldValidationInvalidValue</param>
        private void VerifyFieldValidation(int insuredAmount)
        {
            VerifyEmptyFieldValidation();
            FieldValidationInvalidValue(insuredAmount);
        }

        /// <summary>
        /// Check expected values for empty field validation errors.
        /// </summary>
        /// <param name="browser"></param>
        private void VerifyEmptyFieldValidation()
        {
            ClickControl(XPath.Button.NextPage);

            Reporting.AreEqual(AdviseUser.FieldValidation.YesNoToggle,
                GetElement(XPath.AdviseUser.FieldValidation.Financed).Text, "expected vs actual field validation text for 'Finance on your boat' field.");

            Reporting.Log($"Capturing screen before setting Financed = Yes then selecting Next trigger field validation for the Financier field", _browser.Driver.TakeSnapshot());
            IsFinanced = true;
            ClickControl(XPath.Button.NextPage);

            Reporting.AreEqual(Constants.AdviseUser.FieldValidation.Make,
                GetElement(XPath.AdviseUser.FieldValidation.Make).Text, "expected vs actual field validation text for `Make` field.");
            
            Reporting.AreEqual(Constants.AdviseUser.FieldValidation.Year,
                GetElement(XPath.AdviseUser.FieldValidation.Year).Text, "expected vs actual field validation text for `Year` field.");

            Reporting.AreEqual(AdviseUser.FieldValidation.SelectOne,
                GetElement(XPath.AdviseUser.FieldValidation.Hull).Text, "expected vs actual field validation text for `Hull construction` field.");

            Reporting.AreEqual(AdviseUser.FieldValidation.EnterAValue,
                GetElement(XPath.AdviseUser.FieldValidation.Value).Text, "expected vs actual field validation text for `Value of boat` field.");

            Reporting.AreEqual(Constants.AdviseUser.FieldValidation.Financier,
                GetElement(XPath.AdviseUser.FieldValidation.Financier).Text, "expected vs actual field validation text for Financier field.");

            ScrollElementIntoView(XPath.AdviseUser.FieldValidation.Financier);
            Reporting.Log($"Scrolling screen so 'Financier' field validation is visible and capturing screen state before continuing", _browser.Driver.TakeSnapshot());
            
            Year = DateTime.Now.Year.ToString();
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.FieldValidation.Year), WaitTimes.T5SEC);
            Reporting.Log("Dismissing empty field validation error for Year before beginning invalid value validation error checks", _browser.Driver.TakeSnapshot());
        }

        /// <summary>
        /// Verify the field validation error for the Agreed Value field is displayed as expected
        /// if the user populates it with a value less than $1.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="insuredAmount">Used to restore the Sum Insured to the desired value after triggering validation errors</param>
        private void FieldValidationInvalidValue(int insuredAmount)
        {
            Reporting.Log($"Setting Sum Insured to $1");
            Value = "1";

            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.FieldValidation.Value), WaitTimes.T5SEC);
            Reporting.Log("Capturing snapshot showing field validation error has been dismissed once a positive value has been input.", _browser.Driver.TakeSnapshot());

            Reporting.Log($"Setting Sum Insured to $0"); 
            Value = "0";

            _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.FieldValidation.Value), WaitTimes.T5SEC);
            Reporting.Log("Capturing snapshot of field validation error for 'Agreed value' = 0.", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(AdviseUser.FieldValidation.EnterAValue,
                GetInnerText(XPath.AdviseUser.FieldValidation.Value), "element displayed to advise user we need a positive sum insured");

            VerifyDeclineCover();
            
            Reporting.Log($"Setting Sum Insured back to value generated for this test: Value = {insuredAmount}");
            Value = insuredAmount.ToString(CultureInfo.InvariantCulture);
            Reporting.Log("Capturing snapshot showing value for this test has been restored.", _browser.Driver.TakeSnapshot());

            Reporting.LogMinorSectionHeading($"Setting invalid year to trigger field validation");
            Year = DateTime.Now.AddYears(1).Year.ToString();
            Reporting.AreEqual(Constants.AdviseUser.FieldValidation.Year,
                GetElement(XPath.AdviseUser.FieldValidation.Year).Text, "expected vs actual field validation text for `Year` field.");
            Year = DateTime.Now.Year.ToString();
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.FieldValidation.Year), WaitTimes.T5SEC);
            Reporting.Log("Capturing snapshot showing field validation disappears when a valid value is input.", _browser.Driver.TakeSnapshot());
        }

        private void VerifyDeclineCover()
        {
            Reporting.LogMinorSectionHeading("Begin Decline Cover verification");
            double moreThanMax = (BOAT_MAXIMUM_INSURED_VALUE_ONLINE + 1);
            Reporting.Log($"Setting Sum Insured to {moreThanMax}");
            Value = moreThanMax.ToString(CultureInfo.InvariantCulture);

            _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.DeclinedCover.ValueTooHighTitle), WaitTimes.T5SEC);
            Reporting.Log("Capturing snapshot of advice to user that they must call to discuss a quote for a high value boat.", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.ValueTooHighTitle,
                GetInnerText(XPath.AdviseUser.DeclinedCover.ValueTooHighTitle), "title of element displayed to advise must call for quote due to high value boat");
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.ValueTooHighBody,
                GetInnerText(XPath.AdviseUser.DeclinedCover.ValueTooHighBody), "body of element displayed to advise must call for quote due to high value boat");
            Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.ValueTooHighLink,
                GetElement(XPath.AdviseUser.DeclinedCover.ValueTooHighLink).GetAttribute("href"), "URL of link on the telephone number provided when we advise the user to call us");

            Reporting.Log($"Setting Sum Insured to {BOAT_MAXIMUM_INSURED_VALUE_ONLINE} exactly");
            Value = BOAT_MAXIMUM_INSURED_VALUE_ONLINE.ToString(CultureInfo.InvariantCulture);

            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.DeclinedCover.ValueTooHighTitle), WaitTimes.T5SEC);
            Reporting.Log("Capturing snapshot to show message has been dismissed.", _browser.Driver.TakeSnapshot());
        }

        public void PopulateFields(QuoteBoat quoteBoat)
        {
            Make = quoteBoat.BoatMake.GetDescription();
            if (quoteBoat.BoatMake == BoatMake.Other)
            {
                OtherMake = UnlistedInputs.OtherBoatMake;
            }
            Reporting.Log($"Capturing screen state after input of Make.", _browser.Driver.TakeSnapshot());
            Year = quoteBoat.BoatYearBuilt.ToString();
            Hull = quoteBoat.SparkBoatHull.GetDescription();
            Value = quoteBoat.InsuredAmount.ToString();
            
            Reporting.Log($"IsFinanced = {quoteBoat.IsFinanced}");
            IsFinanced = quoteBoat.IsFinanced;
            if (IsFinanced)
            {
                Reporting.Log($"Financier = {quoteBoat.Financier}");
                Financier = quoteBoat.Financier;
            }

            Reporting.Log($"Capturing screen state after input.", _browser.Driver.TakeSnapshot());
        }
        public void ContinueToMoreBoat()
        {
            ClickControl(XPath.Button.NextPage);
        }
    }
}