using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Data;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace UIDriver.Pages.Spark.MotorcycleQuote
{
    public class TellUsMoreAboutYourBike : SparkBasePage
    {
        #region XPATHS
        private class XPath
        {
            public static class General
            {
                public const string Header = FORM + "//h2[contains(text(),'Tell us more about your')]";
            }
            public static class MotorCycle
            {
                public static class AnnualDistance
                {
                    public const string Input = "//div[@id='mui-component-select-selectedAnnualKmsOption']";
                    public const string DropdownOptions = "//div[@id='menu-selectedAnnualKmsOption']//li";
                }
                public static class Suburb
                {
                    public const string Input = "//div[@data-testid='textfield-suburb-selection']//input[@placeholder='Enter suburb']";
                    public const string DropdownOptions = "//ul[contains(@class,'MuiAutocomplete-listbox') and contains(@id,'-listbox')]/li";
                }
                public static class Toggles
                {
                    public const string IsGaraged = "//div[@data-testid='isGaragedTest']";
                    public const string UseACamera = "//div[@data-testid='isDashcamTest']";
                    public const string IsFinanced = "//div[@data-testid='isFinancedTest']";
                }
                public static class CheckBox
                {
                    public const string Immobiliser = "//span[@data-testid='securityOptionTestImmobiliser']";
                    public const string Tracker = "//span[@data-testid='securityOptionTestTrackingDevice']";
                }
                public static class Financier
                {
                    public const string Input = "//div[@data-testid='textField-Financier-Selection']//input";
                }
            }
            public static class Button
            {
                public const string Yes = "//button[@aria-label='Yes']";
                public const string No = "//button[@aria-label='No']";
                public const string Next = FORM + "//button[@data-testid='submit']";
            }
        }
        #endregion

        #region Settable properties and controls
        public AnnualKms AnnualKm
        {
            get
            {
                // We get the text value but remove trailing " km" so that it aligns with
                // Annual KM strings from general motor.
                var dropdownText = GetInnerText(XPath.MotorCycle.AnnualDistance.Input).Replace(" km","");
                return DataHelper.GetValueFromDescription<AnnualKms>(dropdownText);
            }
            set
            {
                // Motorcycle uses an appended " km", compared to general motor vehicle prompts.
                string valueText = $"{value.GetDescription()} km";
                WaitForSelectableAndPickFromDropdown(XPath.MotorCycle.AnnualDistance.Input, XPath.MotorCycle.AnnualDistance.DropdownOptions, valueText);
            }
        }

        public bool HasImmobiliser
        {
            get => GetClass(XPath.MotorCycle.CheckBox.Immobiliser).Contains("Mui-checked");

            set
            {
                if (value != HasImmobiliser)
                {
                    ClickControl(XPath.MotorCycle.CheckBox.Immobiliser);
                }
            }
        }

        public bool HasTracker
        {
            get => GetClass(XPath.MotorCycle.CheckBox.Tracker).Contains("Mui-checked");

            set
            {
                if (value != HasTracker)
                {
                    ClickControl(XPath.MotorCycle.CheckBox.Tracker);
                }
            }
        }

        public string ParkedSuburbAndCode
        {
            get => GetValue(XPath.MotorCycle.Suburb.Input);
            set => WaitForSelectableFieldToSearchAndPickFromDropdown(XPath.MotorCycle.Suburb.Input, XPath.MotorCycle.Suburb.DropdownOptions, value);
        }

        public bool IsFinanced
        {
            get => GetBinaryToggleState(XPath.MotorCycle.Toggles.IsFinanced, XPath.Button.Yes, XPath.Button.No);

            set => ClickBinaryToggle(XPath.MotorCycle.Toggles.IsFinanced, XPath.Button.Yes, XPath.Button.No, value);
        }

        public string FinancierName
        {
            get => GetValue(XPath.MotorCycle.Financier.Input);

            set => WaitForTextFieldAndEnterText(XPath.MotorCycle.Financier.Input, value);
        }

        public bool HasDashcam
        {
            get => GetBinaryToggleState(XPath.MotorCycle.Toggles.UseACamera, XPath.Button.Yes, XPath.Button.No);

            set => ClickBinaryToggle(XPath.MotorCycle.Toggles.UseACamera, XPath.Button.Yes, XPath.Button.No, value);
        }

        public bool IsGaraged
        {
            get => GetBinaryToggleState(XPath.MotorCycle.Toggles.IsGaraged, XPath.Button.Yes, XPath.Button.No);

            set => ClickBinaryToggle(XPath.MotorCycle.Toggles.IsGaraged, XPath.Button.Yes, XPath.Button.No, value);
        }
        #endregion

        public TellUsMoreAboutYourBike(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.Header);
                GetElement(XPath.MotorCycle.AnnualDistance.Input);
                GetElement(XPath.MotorCycle.Suburb.Input);
                GetElement(XPath.Button.Next);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Motorcycle Quote page - Tell Us More About Your Bike");
            return true;
        }

        /// <summary>
        /// Completes all fields on how member uses their bike and desired insured value.
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <param name="submit"></param>
        /// <returns>The market value of the vehicle which may be different from the agreed value from the test data.</returns>
        public void FillQuoteDetails(QuoteMotorcycle quoteDetails, bool submit = true)
        {
            AnnualKm       = quoteDetails.AnnualKm;
            IsGaraged      = quoteDetails.IsGaraged;
            ParkedSuburbAndCode   = quoteDetails.ParkingAddress.SuburbAndCode();
            HasImmobiliser = quoteDetails.HasImmobiliser;
            HasDashcam     = quoteDetails.HasDashcam;
            IsFinanced     = quoteDetails.IsFinanced;
            if (IsFinanced)
            {
                FinancierName = quoteDetails.Financier;
            }
            Reporting.Log($"Capturing Screenshot of 'Bike usage' page", _browser.Driver.TakeSnapshot());
            ClickNext();
        }

        /// <summary>
        /// Click Next to submit the current form.
        /// </summary>
        /// <exception cref="ReadOnlyException">Thrown if button is present but disabled.</exception>
        public void ClickNext()
        {
            if (IsControlEnabled(XPath.Button.Next))
            {
                ClickControl(XPath.Button.Next);
            }
            else
            {
                throw new ReadOnlyException("Button is currently disabled and not clickable. Check input values.");
            }
        }

        /// <summary>
        /// Ignore CSS from visual testing
        /// </summary>
        public List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
              "[data-testid='header']"
          };
    }
}
