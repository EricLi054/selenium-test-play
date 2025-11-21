using Rac.TestAutomation.Common;
using OpenQA.Selenium;
using System;
using System.Threading;
using System.Globalization;

namespace UIDriver.Pages.Spark.MotorcycleQuote
{
    public class LetsStartWithYourBike : SparkBasePage
    {
        #region XPATHS
        private static class XPath
        {
            public static class General
            {
                public const string Header = FORM + "//h2[contains(text(),'start with your bike')]";
            }
            public static class Button
            {
                public const string Next = FORM + "//button[@data-testid='submit']";
            }
            public static class MotorCycle
            {
                public static class Make
                {
                    private const string Dropdown = "id('manufacturer-input-listbox')";
                    public const string Input = "id('manufacturer-input')";
                    public const string Options = Dropdown + "//li";
                }
                public static class Year
                {
                    private const string Dropdown = "id('year-input-listbox')";
                    public const string Input = "id('year-input')";
                    public const string Options = Dropdown + "//li";
                }
                public static class Model
                {
                    private const string Dropdown = "id('model-input-listbox')";
                    public const string Input = "id('model-input')";
                    public const string Options = Dropdown + "//li";
                }
                public static class Engine
                {
                    private const string Dropdown = "id('engineCapacity-input-listbox')";
                    public const string Input = "id('engineCapacity-input')";
                    public const string Options = Dropdown + "//li";
                }
            }
        }
        #endregion

        #region Settable properties and controls
        public string Make
        {
            get => GetElement(XPath.MotorCycle.Make.Input).Text;

            set => WaitForSelectableAndPickFromDropdown(XPath.MotorCycle.Make.Input, XPath.MotorCycle.Make.Options, value);
        }

        public string Model
        {
            get => GetValue(XPath.MotorCycle.Model.Input);

            set
            {
                try
                {
                    _driver.WaitForElementToBeEnabled(By.XPath(XPath.MotorCycle.Model.Input));
                    WaitForSelectableAndPickFromDropdown(XPath.MotorCycle.Model.Input, XPath.MotorCycle.Model.Options, value);
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

        public string EngineCC
        {
            get => GetValue(XPath.MotorCycle.Engine.Input);

            set
            {
                var engineControl = GetElement(XPath.MotorCycle.Engine.Input);
                if (engineControl.Enabled)
                    WaitForSelectableAndPickFromDropdown(XPath.MotorCycle.Engine.Input, XPath.MotorCycle.Engine.Options, value);
                else
                {
                    if (!string.Equals(value, EngineCC, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Reporting.Error($"Unable to set EngineCC field, and its value of {EngineCC} differs from expected {value}");
                    }
                }
            }
        }

        public decimal Year
        {
            get => decimal.Parse(GetValue(XPath.MotorCycle.Year.Input));

            set
            {
                // Manufacturers like "Viper" only have one year
                var engineControl = GetElement(XPath.MotorCycle.Year.Input);
                if (engineControl.Enabled)
                    WaitForSelectableAndPickFromDropdown(XPath.MotorCycle.Year.Input, XPath.MotorCycle.Year.Options, value.ToString(CultureInfo.InvariantCulture));
                else
                {
                    if (value != Year)
                    {
                        Reporting.Error($"Unable to set Year field, and its value of {Year} differs from expected {value}");
                    }
                }
            }
        }
        #endregion

        public LetsStartWithYourBike(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.Header);
                GetElement(XPath.MotorCycle.Make.Input);
                GetElement(XPath.MotorCycle.Model.Input);
                GetElement(XPath.Button.Next);
            }
            catch (NoSuchElementException)
            {
                Reporting.Log("Element not found when checking if Motorcycle Quote page is displayed.");
                return false;
            }

            Reporting.LogPageChange("Motorcycle Quote page - Lets Start With Your Bike");
            return true;
        }

        /// <summary>
        /// Fills all the vehicle fields and should result in a single
        /// motorcycle match?
        /// </summary>
        /// <param name="vehicle"></param>
        public void SearchForMotorcycle(Motorcycle vehicle)
        {
            Make     = vehicle.Make;
            Year     = vehicle.Year;
            //For some makes and Years (e.g:- SUR-RON : 2018) After entering the Year, 
            //the Model text field becomes enabled for a fraction of a second and then it becomes disabled
            //This fools Selenium. To avoid that we to wait here a bit.
            Thread.Sleep(3000);
            Model    = vehicle.Model;
            EngineCC = vehicle.EngineCC; 
        }

        /// <summary>
        /// Submit page1 form.
        /// </summary>
        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }
    }
}
