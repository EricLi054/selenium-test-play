using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;

namespace UIDriver.Pages.Spark.Endorsements.MotorRenewal
{
    public class LetsUpdateYourCar : SparkBasePage
    {
        #region CONSTANTS
        private class Constants
        {
            public const string PageTitle = "Let's update your car";

            public class WarningText
            {
                public const string WarningRegistration = "Please search with your registration";
                public const string WarningMake = "Please select a make";
                public const string WarningYear = "Please select a year";
                public const string WarningModel = "Please select a model";
                public const string WarningBodyType = "Please select a body type";
                public const string WarningTransmission = "Please select a transmission";
                public const string WarningSelectCar = "Please select your car";
            }
           ;
        }

        #endregion

        #region XPATHS

        private static class XPath
        {
            public const string PageTitle = "id('stepper-content')/section/div/h2";
            public const string Label = "id('vehicle-lookup')";

            public static class Toggle
            {
                public const string MakeAndModelButton = "//*[@id=\"vehicle-lookup-Make & model\"]";
                public const string RegistrationButton = "id('vehicle-lookup-Registration')";
            }

            public static class Radio
            {
                public static string SelectCar(string id) => $"id('select-vehicle-item-{id}-radio-container')";
            }

            public static class Field
            {
                public static class Make
                {
                    public const string Input    = "id('manufacturer')";
                    public const string Options  = "id('manufacturer-listbox')//li";
                }
                public static class Year
                {
                    public const string Input   = "id('year')";
                    public const string Options = "id('year-listbox')//li";
                }
                public static class Model
                {
                    public const string Input   = "id('model')";
                    public const string Options = "id('model-listbox')//li";
                }
                public static class BodyType
                {
                    public const string Input = "id('body')";
                    public const string Options = "id('body-listbox')//li";
                }
                public static class TransmissionType
                {
                    public const string Input = "id('transmission')";
                    public const string Options = "id('transmission-listbox')//li";
                }

                public const string RegoSearch = "id('search_searchInput')";
            }

            public class Button
            {
                public const string Confirm = "id('update-your-car-confirm-button')";
                public const string Search = "id('search_searchButton')";
            }

            public class WarningField
            {
                public const string WarningRegistration = "id('select-vehicle-error')";
                public const string WarningMake = "//label[text()='Make']/../p";
                public const string WarningYear = "//label[text()='Year']/../p";
                public const string WarningModel = "//label[text()='Model']/../p";
                public const string WarningBodyType = "//label[text()='Body type']/../p";
                public const string WarningTransmission = "//label[text()='Manual or automatic']/../p";
                public const string WarningSelectCar = "id('select-vehicle-error')";
            }
        }

        #endregion

        #region Settable properties and controls

        public string Make
        {
            get => GetValue(XPath.Field.Make.Input);
            set
            {
                try
                {
                    _driver.WaitForElementToBeEnabled(By.XPath(XPath.Field.Make.Input));
                    WaitForSelectableAndPickFromDropdown(XPath.Field.Make.Input, XPath.Field.Make.Options, value);
                }
                catch (NoSuchElementException)
                {
                    if (!string.Equals(value, Make, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Reporting.Error($"Unable to set Make field, and its value of {Make} differs from expected {value}");
                    }
                }
            }
        }

        public string Year
        {
            get => GetValue(XPath.Field.Year.Input);
            set
            {
                try
                {
                    _driver.WaitForElementToBeEnabled(By.XPath(XPath.Field.Year.Input));
                    WaitForSelectableAndPickFromDropdown(XPath.Field.Year.Input, XPath.Field.Year.Options, value);
                }
                catch (NoSuchElementException)
                {
                    if (!string.Equals(value, Year, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Reporting.Error($"Unable to set Year field, and its value of {Year} differs from expected {value}");
                    }
                }
            }
        }

        public string Model
        {
            get => GetValue(XPath.Field.Model.Input);
            set
            {
                try
                {
                    _driver.WaitForElementToBeEnabled(By.XPath(XPath.Field.Model.Input));
                    WaitForSelectableAndPickFromDropdown(XPath.Field.Model.Input, XPath.Field.Model.Options, value);
                }
                catch (NoSuchElementException)
                {
                    if (!string.Equals(value, Model, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Reporting.Error($"Unable to set Model field, and its value of {Model} differs from expected {value}");
                    }
                }
            }
        }

        public string BodyType
        {
            get => GetValue(XPath.Field.BodyType.Input);
            set
            {
                try
                {
                    _driver.WaitForElementToBeEnabled(By.XPath(XPath.Field.BodyType.Input));
                    WaitForSelectableAndPickFromDropdown(XPath.Field.BodyType.Input, XPath.Field.BodyType.Options, value);
                }
                catch (NoSuchElementException)
                {
                    if (!string.Equals(value, BodyType, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Reporting.Error($"Unable to set Body Type field, and its value of {BodyType} differs from expected {value}");
                    }
                }
            }
        }

        public string TransmissionType
        {
            get => GetValue(XPath.Field.TransmissionType.Input);
            set
            {
                try
                {
                    _driver.WaitForElementToBeEnabled(By.XPath(XPath.Field.TransmissionType.Input));
                    WaitForSelectableAndPickFromDropdown(XPath.Field.TransmissionType.Input, XPath.Field.TransmissionType.Options, value);
                }
                catch (NoSuchElementException)
                {
                    if (!string.Equals(value, TransmissionType, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Reporting.Error($"Unable to set Transmission field, and its value of {TransmissionType} differs from expected {value}");
                    }
                }
            }
        }

        public string RegoSearch
        {
            get => GetValue(XPath.Field.RegoSearch);
            set => WaitForTextFieldAndEnterText(XPath.Field.RegoSearch, value);
        }


        #endregion

        public LetsUpdateYourCar(Browser browser) : base(browser)
        {
        }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PageTitle);
                Reporting.Log($"Page 2 Update my car title is visible");
                GetElement(XPath.Label);
                Reporting.Log($"Page 2: Lookup method question is visible");
                GetElement(XPath.Button.Confirm);
                Reporting.Log($"Page 2: Confirm Button is Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Update Your Car Details page");
            Reporting.Log($"Page 2: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// Add new car by make and model, also perform optional Ui checking for car model
        /// </summary>
        /// <param name="newCar">Passing in values generated for new car to be input, replacing the existing insured vehicle.</param>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public void SelectNewCar(Car newCar, bool detailUiChecking = false)
        {
            ClickMakeAndModel();

            Make = newCar.Make;
            Year = newCar.Year.ToString();
            Model = newCar.Model;
            BodyType = newCar.Body;
            TransmissionType = newCar.Transmission;

            if (detailUiChecking)
            {
                ClickConfirm();
                Reporting.Log($"Page 2: Capture screenshot for warning message.", _browser.Driver.TakeSnapshot());
                Reporting.AreEqual(Constants.WarningText.WarningSelectCar, GetInnerText(XPath.WarningField.WarningSelectCar), $" 'Please select your car' warning message against display");
            }

            ClickControl(XPath.Radio.SelectCar(newCar.VehicleId));
        }

        /// <summary>
        /// Clicking the Make And Model Toggle buttom on the 'Lets Update Your Car' Page to start
        /// entering Make, Year, Model, BodyType and Transmission Type
        /// </summary>
        public void ClickMakeAndModel()
        {
            ClickControl(XPath.Toggle.MakeAndModelButton);
        }

        public void ClickConfirm()
        {
            ClickControl(XPath.Button.Confirm, 10);
        }

        public void VerifyRegistrationWarning()
        {
            Reporting.Log($"Page 2: Capture screenshot for warning message.", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual(Constants.WarningText.WarningRegistration, GetInnerText(XPath.WarningField.WarningRegistration), $" 'Enter your registration to find your car' warning message against display");
        }

        /// <summary>
        /// Verifying the warning message on selecting new car by Make and Model 
        /// </summary>
        public void VerifyMakeAndModelWarning()
        {
            Reporting.Log($"Page 2: Capture screenshot for warning message.", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual(Constants.WarningText.WarningMake, GetInnerText(XPath.WarningField.WarningMake), $" 'Make' warning message against display");
            Reporting.AreEqual(Constants.WarningText.WarningYear, GetInnerText(XPath.WarningField.WarningYear), $" 'Year' warning message against display");
            Reporting.AreEqual(Constants.WarningText.WarningModel, GetInnerText(XPath.WarningField.WarningModel), $" 'Model' warning message against display");
            Reporting.AreEqual(Constants.WarningText.WarningBodyType, GetInnerText(XPath.WarningField.WarningBodyType), $" 'Body Type' warning message against display");
            Reporting.AreEqual(Constants.WarningText.WarningTransmission, GetInnerText(XPath.WarningField.WarningTransmission), $" 'Manual or automatic' warning message against display");
        }
    }
}