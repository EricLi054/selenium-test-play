using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Spark.Endorsements.CaravanEndorsement
{
    public class LetsUpdateYourCaravanOrTrailer : SparkBasePage
    {
        #region CONSTANTS
        private static class Constants
        {
            public static readonly string PageHeading         = "Let's update your caravan or trailer";

            public static class WarningMessages
            {
                public static readonly string Make            = "Please select a make";
                public static readonly string Year            = "Please select a year";
                public static readonly string Model           = "Please select a model";

                public static readonly string SelectCaravanModel = "Please select your caravan or trailer";

                public static readonly string ValueOfCaravan  = "Please enter a value between $1000 and $150,000";

                public static readonly string CantFindCaravan = "Can't find your caravan or trailer?";
                public static readonly string Call            = "Please call us on 13 17 03";
            }
          
        }
        #endregion

        #region XPATHS
        private static class XPath
        {
            public static readonly string PageHeading         = "//h2[text()=" + "\"" + Constants.PageHeading + "\"" + "]";

            public static class Make
            {
                private static readonly string Dropdown       = "id('manufacturer-listbox')";
                public static readonly string Input           = "id('manufacturer')";
                public static readonly string Options         = Dropdown + "//li";
                public static readonly string Error           = "id('helper-text-manufacturer')";
            }
            public static class Year
            {
                private static readonly string Dropdown       = "id('year-listbox')";
                public static readonly string Input           = "id('year')";
                public static readonly string Options         = Dropdown + "//li";
                public static readonly string Error           = "id('helper-text-year')";
            }
            public static class Model
            {
                private static readonly string Dropdown       = "id('model-listbox')";
                public static readonly string Input           = "id('model')";
                public static readonly string Options         = Dropdown + "//li";
                public static readonly string Error           = "id('helper-text-model')";
            }

            public static class Value
            {
                public static readonly string Input           = FORM + "//input[@id='agreed-value-input']";
                public static readonly string Label           = Input + "/../../legend[1]/p";
                public static readonly string Error           = "id('agreed-value-error')";
            }
            public static class Results
            {
                public static readonly string Selected        = "//div[@data-testid='caravan-container']//span[contains(@class,'Mui-checked ')]/following-sibling::span";
                public static string OptionById(string vehicleId) => $"//input[@type='radio' and @value='{vehicleId}']/../..";
                public static readonly string Error = "id('select-vehicle-error')";
            }

            public static class Button
            {
                public static readonly string Confirm         = "id('update-your-caravan-next-button')";
                public static readonly string Back            = "id('back-link')";
            }

            public static class UnableToFind
            {
                public static readonly string CantFindCaravan = "id('select-vehicle-card-title')";
                public static readonly string Call            = "//a[@id='select-vehicle-card-call-us']/..";
            }

           
        }
        #endregion

        #region Settable properties and controls
        private string Make
        {
            get => GetInnerText(XPath.Make.Input);
            set
            {
                _driver.WaitForElementToBeEnabled(By.XPath(XPath.Make.Input));
                Thread.Sleep(SleepTimes.T2SEC);
                WaitForSelectableAndPickFromDropdown(XPath.Make.Input, XPath.Make.Options, value);
            }
        }

        private string Model
        {
            get => GetValue(XPath.Model.Input);

            set
            {
                try
                {
                    _driver.WaitForElementToBeEnabled(By.XPath(XPath.Model.Input));
                    Thread.Sleep(SleepTimes.T2SEC);
                    WaitForSelectableAndPickFromDropdown(XPath.Model.Input, XPath.Model.Options, value);
                }
                catch (NoSuchElementException)
                {
                    if (!string.Equals(value, Model, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Reporting.Error($"Unable to set Model Family field, and its value of {Model} differs from expected {value}");
                    }
                }
            }
        }

        private string ModelSelected => GetInnerText(XPath.Results.Selected);

        /// <summary>
        /// Supports Spark Caravan
        /// Used to select the caravan model description.
        /// The vehicle id (being unique) is used here to select the model description
        /// </summary>
        private void SelectCaravanFromListByVehicleID(string vehicleId)
        {
            var desiredModelRadioControl = XPath.Results.OptionById(vehicleId);

            if (!_driver.TryWaitForElementToBeVisible(By.XPath(desiredModelRadioControl),
                                                      WaitTimes.T5SEC,
                                                      out IWebElement field))
            {
                Reporting.Error("Model description radio button is not visible");
            }
            if (!field.Enabled)
            { Reporting.Error("Unable to select the radio button"); }

            if (!(field.GetAttribute("class").Contains("checked")))
            { ClickControl(desiredModelRadioControl); }
        }

        private decimal Year
        {
            get => decimal.Parse(GetValue(XPath.Year.Input));

            set
            {
                Thread.Sleep(SleepTimes.T2SEC);
                // Manufacturers like "Viper" only have one year
                var yearInput = GetElement(XPath.Year.Input);
                if (yearInput.Enabled)
                { WaitForSelectableAndPickFromDropdown(XPath.Year.Input, XPath.Year.Options, value.ToString()); }
                else
                {
                    if (value != Year)
                    {
                        Reporting.Error($"Unable to set Year field, and its value of {Year} differs from expected {value}");
                    }
                }
            }
        }

        private string SetCaravanValue
        {
            set => WaitForTextFieldAndEnterText(XPath.Value.Input, value,false);
        }

        private string GetCaravanValue
        {
            set => WaitForTextFieldAndEnterText(XPath.Value.Input, value);
        }

        #endregion

        public LetsUpdateYourCaravanOrTrailer(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                Reporting.Log($"Page 2: Checking that 'Let's update your caravan or trailer' header is displayed");
                GetElement(XPath.PageHeading);
                Reporting.Log($"Page 2: Checking that the 'Confirm' button is displayed");
                GetElement(XPath.Button.Confirm);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Let's update your caravan or trailer");
            Reporting.Log($"Page 2: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Fills all the caravan fields such as Make, Year, Model
        /// and the model description.
        /// To identify the model description, the vehicle id is used
        /// </summary>
        public void SearchForCaravan(EndorseCaravan endorseCaravan, bool detailUiChecking)
        {
            Make = endorseCaravan.NewInsuredAsset.Make;
            Year = endorseCaravan.NewInsuredAsset.Year;
            Model = endorseCaravan.NewInsuredAsset.Model != null ? endorseCaravan.NewInsuredAsset.Model : endorseCaravan.NewInsuredAsset.ModelDescription;

            if (detailUiChecking)
            {
                ClickConfirm();
                VerifySelectYourCaravanTrailerWarningMessages();
            }

            //VehicleId is used to identify the vehicle Model Description.
            //The value of the model Description radio button contains the vehicle Id
            SelectCaravanFromListByVehicleID(endorseCaravan.NewInsuredAsset.VehicleId);

            if (IsControlDisplayed(XPath.Value.Input))
            {
                if (detailUiChecking)
                {
                    ClickConfirm();
                    VerifyAgreedValueWarningMessages();
                }
                SetCaravanValue = endorseCaravan.NewInsuredAsset.MarketValue.ToString();
            }
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Enter the value of caravan
        /// </summary>
        public void EnterValueOfCaravan(decimal marketValue)
        {
            SetCaravanValue = marketValue.ToString();
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Submit page1 form.
        /// </summary>
        public void ClickConfirm()
        {
            ClickControl(XPath.Button.Confirm);
        }

        /// <summary>
        /// Verify the warning message displayed for Make, Year and Model
        /// </summary>
        public  void VerifyFieldWarningMessages()
        {
            Reporting.AreEqual(Constants.WarningMessages.Make, GetInnerText(XPath.Make.Error), "the error message displayed for the Make");
            Reporting.AreEqual(Constants.WarningMessages.Year, GetInnerText(XPath.Year.Error), "the error message displayed for the Year");
            Reporting.AreEqual(Constants.WarningMessages.Model, GetInnerText(XPath.Model.Error), "the error message displayed for the Model");
        }

        /// <summary>
        /// Verify the unable to find warning messages
        /// </summary>
        public void VerifyCantFindWarningMessages()
        {
            Reporting.AreEqual(Constants.WarningMessages.CantFindCaravan, GetInnerText(XPath.UnableToFind.CantFindCaravan), "the error message displayed for Cant find caravan");
            Reporting.AreEqual(Constants.WarningMessages.Call, GetInnerText(XPath.UnableToFind.Call), "phone number displayed");
        }

        /// <summary>
        /// Verify the 'Please select your caravan or trailer' warning messages
        /// </summary>
        public void VerifySelectYourCaravanTrailerWarningMessages()
        {
            Reporting.AreEqual(Constants.WarningMessages.SelectCaravanModel, GetInnerText(XPath.Results.Error), "the error message displayed for 'Please select your caravan or trailer' field");
        }

        /// <summary>
        /// Verify the 'Value of caravan or trailer' warning messages
        /// </summary>
        public void VerifyAgreedValueWarningMessages()
        {
            Reporting.AreEqual(Constants.WarningMessages.ValueOfCaravan, GetInnerText(XPath.Value.Error), "the error message displayed for 'Value of caravan or trailer' field");
        }

        public void EnterNewCaravanValue(int marketValue)
        {
            if (IsControlDisplayed(XPath.Value.Input))
            {
                SetCaravanValue = marketValue.ToString();
            }
        }
    }
}
