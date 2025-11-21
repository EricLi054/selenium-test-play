using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;

namespace UIDriver.Pages.Spark.CaravanQuote
{
    public class LetsStartWithYourCaravan : SparkBasePage
    {
        #region XPATHS
        private static class XPath
        {
            public static class General
            {
                public const string Header = FORM + "//h2[contains(text(),'start with your caravan')]";
            }
            public static class Button
            {
                public const string Next = FORM + "//button[@data-testid='submit']";
            }
            public static class Caravan
            {
                public static class Make
                {
                    private const string Dropdown = "id('input-manufacturer-listbox')";
                    public const string Input = "id('input-manufacturer')";
                    public const string Options = Dropdown + "//li";
                }
                public static class Year
                {
                    private const string Dropdown = "id('input-year-listbox')";
                    public const string Input = "id('input-year')";
                    public const string Options = Dropdown + "//li";
                }
                public static class Model
                {
                    private const string Dropdown = "id('input-model-listbox')";
                    public const string Input = "id('input-model')";
                    public const string Options = Dropdown + "//li";
                }
                public static class Value
                {
                    public const string Input = FORM + "//input[@id='input-marketValue']";
                    public const string Label = Input + "/../../legend[1]/p";
                    public const string Error = "//*[@data-testid='input-marketValue-error-message']";
                }
                public static class Results
                {
                    public const string Selected = "//div[@data-testid='input-caravan-container']//span[contains(@class,'Mui-checked ')]/following-sibling::span";
                    public static string OptionById(string vehicleId) => $"//input[@type='radio' and @value='{vehicleId}']/../..";
                }
            }
        }
        #endregion

        #region Constants
        private const string VALUE_OF_CARAVAN   = "Value of caravan";
        private const string CARAVAN_VALUE_MSG = "Consider what your caravan, including accessories, would sell for in today's market.";
        private const string CALL_CARAVAN_VALUE = "We need some more information\r\nPlease call us on 13 17 03 to insure a caravan valued over {0}.";
        #endregion

        #region Settable properties and controls
        public string Make
        {
            get => GetInnerText(XPath.Caravan.Make.Input);
            set
            {
                Thread.Sleep(SleepTimes.T2SEC);
                WaitForSelectableAndPickFromDropdown(XPath.Caravan.Make.Input, XPath.Caravan.Make.Options, value);
            }
        }

        public string Model
        {
            get => GetValue(XPath.Caravan.Model.Input);

            set
            {
                try
                {
                    _driver.WaitForElementToBeEnabled(By.XPath(XPath.Caravan.Model.Input));
                    Thread.Sleep(SleepTimes.T2SEC);
                    WaitForSelectableAndPickFromDropdown(XPath.Caravan.Model.Input, XPath.Caravan.Model.Options, value);
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

        public string ModelSelected => GetInnerText(XPath.Caravan.Results.Selected);

        /// <summary>
        /// Supports Spark Caravan
        /// Used to select the caravan model description.
        /// The vehicle id (being unique) is used here to select the model description
        /// </summary>
        private void SelectCaravanFromListByVehicleID(string vehicleId)
        {
            var desiredModelRadioControl = XPath.Caravan.Results.OptionById(vehicleId);

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

        public decimal Year
        {
            get => decimal.Parse(GetValue(XPath.Caravan.Year.Input));

            set
            {
                // Manufacturers like "Viper" only have one year
                var yearControl = GetElement(XPath.Caravan.Year.Input);
                if (yearControl.Enabled)
                {
                    Thread.Sleep(SleepTimes.T2SEC);
                    WaitForSelectableAndPickFromDropdown(XPath.Caravan.Year.Input, XPath.Caravan.Year.Options, value.ToString());
                }
                else
                {
                    if (value != Year)
                    {
                        Reporting.Error($"Unable to set Year field, and its value of {Year} differs from expected {value}");
                    }
                }
            }
        }

        public string SetCaravanValue
        {
            set => WaitForTextFieldAndEnterText(XPath.Caravan.Value.Input, value);
        }
        #endregion

        public LetsStartWithYourCaravan(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.Header);
                GetElement(XPath.Caravan.Make.Input);
                GetElement(XPath.Caravan.Year.Input);
                GetElement(XPath.Caravan.Model.Input);
                GetElement(XPath.Button.Next);
            }
            catch (NoSuchElementException)
            {
                Reporting.Log("Element not found when checking if Caravan Quote page is displayed.");
                return false;
            }

            Reporting.LogPageChange("Caravan Quote page - Lets Start With Your Caravan");
            return true;
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Fills all the caravan fields such as Make, Year, Model
        /// and the model description.
        /// To identify the model description, the vehicle id is used
        /// </summary>
        /// <param name="caravan"></param>
        public void SearchForCaravan(Caravan caravan)
        {
            Make = caravan.Make;
            Year = caravan.Year;
            Model = caravan.Model != null ? caravan.Model : caravan.ModelDescription;
            //VehicleId is used to identify the vehicle Model Description.
            //The value of the model Description radio button contains the vehicle Id
            SelectCaravanFromListByVehicleID(caravan.VehicleId);
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Verifies the two messages (Information and Validation)
        /// that gets displayed when user does not enter a valid value for the caravan
        /// </summary>
        public void VerifyValueOfCaravanValidationMessages()
        {
            Reporting.AreEqual(CARAVAN_VALUE_MSG, GetInnerText(XPath.Caravan.Value.Label), $"'{VALUE_OF_CARAVAN}' text is visible");
            Reporting.AreEqual($"Please enter a value between ${CARAVAN_MIN_SUM_INSURED_VALUE} and {DataHelper.ConvertIntToMonetaryString(CARAVAN_MAX_SUM_INSURED_VALUE, minValueForThousandsSeparator: 10000)}", GetInnerText(XPath.Caravan.Value.Error), $"'{VALUE_OF_CARAVAN}' validation text is visible");
        }

        /// <summary>
        /// Supports Spark Caravan
        /// Verifies the 'Please call us to continue' message that gets displayed,
        /// when user enters a value, higher than the maximum limit for the caravan
        /// </summary>
        public void VerifyValueOfCaravanCallUsToContinueMessage()
        {
            string callCaravanMsg = string.Format(CALL_CARAVAN_VALUE, DataHelper.ConvertIntToMonetaryString(CARAVAN_MAX_SUM_INSURED_VALUE, minValueForThousandsSeparator: 10000));
            Reporting.AreEqual(callCaravanMsg, GetInnerText(XPath.Caravan.Value.Error), $"'{VALUE_OF_CARAVAN}' 'Please call us' text is visible");
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
        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }
    }
}