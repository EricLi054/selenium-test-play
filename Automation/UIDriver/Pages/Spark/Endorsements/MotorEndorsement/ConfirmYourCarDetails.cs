using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Linq;

namespace UIDriver.Pages.Spark.Endorsements
{
    /// <summary>
    /// Presents information about a member's car.  Shows car registration, engine performance modifications status and 
    /// and finance status (finance company where applicable).
    /// </summary>
    public class ConfirmYourCarDetails : SparkBasePage
    {
        #region CONSTANTS
        private class Constants
        {
            public const string PageTitle = "Confirm your car details";

            public class Registration
            {
                public const string Heading = "Car registration";
                public const string SubHeading = "Don't forget your registration";
                public const string InfoText = "It's better if we have your rego but you can continue without it.";
            }

            public class WarningText
            {
                public const string ModificationWarning = "Please select Yes or No";
                public const string FinanceWarning = "Please select Yes or No";
                public const string FinancierWarning = "Please enter a valid financier";
            }

            public class Notification
            {
                public const string Title = "Don't forget your registration";
                public const string Content = "It's better if we have your rego but you can continue without it.";
            }

            public class ToolTip
            {
                public class Title
                {
                    public const string CarModification = "Car modifications";
                    public const string Finance = "Car finance";
                }

                public class Content
                {
                    public const string CarModification = "This includes changes to your car to improve performance that haven't been installed by the manufacturer, such as:";
                    public const string CarModificationPointOne = "Computer-enhanced performance chips";
                    public const string CarModificationPointTwo = "Super chargers";
                    public const string CarModificationPointThree = "Turbo chargers";
                    public const string CarModificationPointFour = "Any other modification to increase engine capacity";
                    public const string Finance = "Your car is financed if you have a loan with a financial institution, a hire purchase agreement or a lease agreement.";
                }
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public const string PageTitle = "//h2[text()='" + Constants.PageTitle + "']";

            public class Registration
            {
                public const string Label = "//label[@for='vehicle-registration']";
                public const string Field = "id('vehicle-registration')";
                public const string InfoHeading = "id('notification-card-title')";
                public const string InfoText = "id('notification-card-content')";
            }

            public class Finance
            {
                public const string Label = "//label[@for='car-finance-question']";
                public const string ButtonGroup = "id('car-finance-question')";
                public const string Yes = "//button[@id='car-finance-question-true']";
                public const string No = "//button[@id='car-finance-question-false']";
                public const string Dropdown = "id('financier')";
            }

            public class Modifications
            {
                public const string ButtonGroup = "//*[@id=\'car-modifications-question\']";
                public const string Yes = "//*[@id=\'car-modifications-question-true\']";
                public const string No = "//*[@id=\'car-modifications-question-false\']";

            }

            public class Button
            {
                public const string Next = "id('your-car-details-next-button')";
            }

            public class WarningField
            {
                public const string ModificationWarning = "//div[@id='car-modifications-question']/../p";
                public const string FinanceWarning = "//div[@id='car-finance-question']/../p";
                public const string FinancierWarning = "//label[@for='financier']/../p";
            }

            public class Notification
            {
                public const string Icon = "id('notification-card-icon')";
                public const string Title = "id('notification-card-title')";
                public const string Content = "id('notification-card-content')";
            }

            public class ToolTip
            {
                public class ShowButton
                {
                    public const string CarModification = "id('tooltip-car-modifications-question-show-button')";
                    public const string Finance = "id('tooltip-car-finance-question-show-button')";
                }
                public class Title
                {
                    public const string CarModification = "id('tooltip-car-modifications-question-title')";
                    public const string Finance = "id('tooltip-car-finance-question-title')";
                }

                public class Content
                {
                    public const string CarModification = "//div[@id='tooltip-car-modifications-question-message']/p";
                    public const string CarModificationPointOne = "//div[@id='tooltip-car-modifications-question-message']/ul/li[1]";
                    public const string CarModificationPointTwo = "//div[@id='tooltip-car-modifications-question-message']/ul/li[2]";
                    public const string CarModificationPointThree = "//div[@id='tooltip-car-modifications-question-message']/ul/li[3]";
                    public const string CarModificationPointFour = "//div[@id='tooltip-car-modifications-question-message']/ul/li[4]";
                    public const string FinanceContent = "id('tooltip-car-finance-question-message')";
                }

                public class CloseButton
                {
                    public const string CarModification = "id('tooltip-car-modifications-question-close')";
                    public const string Finance = "id('tooltip-car-finance-question-close')";
                }
            }
        }


            #endregion

        #region Settable properties and controls

        public string CarRegistration
        {
            get => GetValue(XPath.Registration.Field);
            set => WaitForTextFieldAndEnterText(XPath.Registration.Field, value, false);
        }
        public bool CarModification
        {
            get => GetBinaryToggleState(XPath.Modifications.ButtonGroup, XPath.Modifications.Yes, XPath.Modifications.No);
            set => ClickBinaryToggle(XPath.Modifications.ButtonGroup, XPath.Modifications.Yes, XPath.Modifications.No, value);
        }
        public bool Finance
        {
            get => GetBinaryToggleState(XPath.Finance.ButtonGroup, XPath.Finance.Yes, XPath.Finance.No);
            set => ClickBinaryToggle(XPath.Finance.ButtonGroup, XPath.Finance.Yes, XPath.Finance.No, value);
        }
        public string Financier
        {
            get => GetValue(XPath.Finance.Dropdown);
            set => WaitForTextFieldAndEnterText(XPath.Finance.Dropdown, value);
        }

        #endregion

        public ConfirmYourCarDetails(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PageTitle);
                GetElement(XPath.Finance.Label);
                Reporting.Log($"Page 1: Finance question is visible");
                GetElement(XPath.Button.Next);
                Reporting.Log($"Page 1: Next Button is Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Confirm Your Car Details page");
            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }


        /// <summary>
        /// Check the presentation of the vehicle registration and associated information.
        /// When the registration is consider interim (eg TBA), then the value will be empty.
        /// Also, no special characters will be displayed.  So a registration with a hyphen will 
        /// get the hyphen removed.  eg RAC-123 becomes RAC123.
        /// </summary>
        public void VerifyExistingCarDetails(EndorseCar endorseCar)
        {
            if (DataHelper.IsRegistrationNumberConsideredValid(endorseCar.OriginalPolicyData.MotorAsset.RegistrationNumber))
            {
                Reporting.AreEqual(endorseCar.OriginalPolicyData.MotorAsset.RegistrationNumber.SanitizeVehicleRegistrationString(), CarRegistration, "car registration number populated");
            }
            else
            {
                Reporting.AreEqual("", CarRegistration, "car registration number left blank when registration considered temporary or otherwise invalid");
            }
            Reporting.AreEqual(endorseCar.OriginalPolicyData.MotorAsset.IsVehicleModified(), CarModification, "Modifications to your car that improve engine performance");
            Reporting.AreEqual(endorseCar.OriginalPolicyData.MotorAsset.IsFinanced, Finance, "Finance on your car");

            if (Finance)
            {
                Reporting.AreEqual(endorseCar.OriginalPolicyData.GetFinancierNameViaShieldAPI(), Financier, ignoreCase: true, $"Expected financier name against the actual name displayed");
            }
        }

        /// <summary>
        /// Verify Card notification in case registration number is not displayed
        /// </summary>
        public void VerifyCarRegistrationNotificationCard(EndorseCar endorse)
        {
            if (string.IsNullOrEmpty(endorse.OriginalPolicyData.MotorAsset.RegistrationNumber) || endorse.ChangeMakeAndModel)
            {
                Reporting.IsTrue(IsControlDisplayed(XPath.Notification.Icon), "Notification Icon Should be displayed");
                Reporting.AreEqual(Constants.Notification.Title, GetInnerText(XPath.Notification.Title), $"expected title of notification panel against actual");
                Reporting.AreEqual(Constants.Notification.Content, GetInnerText(XPath.Notification.Content), $"expected content of notification panel against actual");
            }        
        }

        /// <summary>
        /// Open, verify and close finance tool tip
        /// </summary>
        public void VerifyFinanceToolTip()
        {
            ClickControl(XPath.ToolTip.ShowButton.Finance);
            Reporting.AreEqual(Constants.ToolTip.Title.Finance, GetInnerText(XPath.ToolTip.Title.Finance), $"expected title of finance tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.Finance, GetInnerText(XPath.ToolTip.Content.FinanceContent), $"expected content of finance tool tip against actual");
            ClickControl(XPath.ToolTip.CloseButton.Finance);
        }

        /// <summary>
        /// Open, verify and close car modification tool tip
        /// </summary>
        public void VerifyCarModificationToolTip()
        {
            ClickControl(XPath.ToolTip.ShowButton.CarModification);
            Reporting.AreEqual(Constants.ToolTip.Title.CarModification, GetInnerText(XPath.ToolTip.Title.CarModification), $"expected title of car modification tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.CarModification, GetInnerText(XPath.ToolTip.Content.CarModification), $"expected content of car modification tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.CarModificationPointOne, GetInnerText(XPath.ToolTip.Content.CarModificationPointOne), $"expected bullet point 1 text of car modification tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.CarModificationPointTwo, GetInnerText(XPath.ToolTip.Content.CarModificationPointTwo), $"expected bullet point 2 text of car modification tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.CarModificationPointThree, GetInnerText(XPath.ToolTip.Content.CarModificationPointThree), $"expected bullet point 3 text of car modification tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.CarModificationPointFour, GetInnerText(XPath.ToolTip.Content.CarModificationPointFour), $"expected bullet point 4 text of car modification tool tip against actual");
            ClickControl(XPath.ToolTip.CloseButton.CarModification);
        }

        public void EnterNewCarDetails(EndorseCar endorseCar)
        {
            if (endorseCar.ChangeMakeAndModel)
            {
                CarRegistration = endorseCar.NewInsuredAsset.Registration;
                CarModification = endorseCar.NewInsuredAsset.IsModified;
                Finance = endorseCar.NewInsuredAsset.IsFinanced;
                if (Finance)
                {
                    using (var spinner = new SparkSpinner(_browser))
                    {
                        spinner.WaitForSpinnerToFinish();
                        Financier = endorseCar.NewInsuredAsset.Financier;
                    }                    
                }
            }

            // This will update only the Rego of the Insured car. Usefull in a scenario where there is no
            // change to the make and model but only rego is changed
            if (!string.IsNullOrEmpty(endorseCar.NewInsuredAsset.Registration) && !endorseCar.ChangeMakeAndModel)
            {
                CarRegistration = endorseCar.NewInsuredAsset.Registration;
            }
        }

        /// <summary>
        /// Verifying the warning message for modification, finance and financier
        /// Setting and Resetting the Financier to check the warning message
        /// </summary>
        public void VerifyConfirmYourCarDetailsWarning()
        {
            using (var spinner = new SparkSpinner(_browser))
            {
                ClickNext();
                Reporting.Log($"Page 3: Capture screenshot for warning message.", _browser.Driver.TakeSnapshot());
                Reporting.AreEqual(Constants.WarningText.ModificationWarning, GetInnerText(XPath.WarningField.ModificationWarning), $"'Modifications to your car that improve engine performance' warning message against display");
                Reporting.AreEqual(Constants.WarningText.FinanceWarning, GetInnerText(XPath.WarningField.FinanceWarning), $"'Finance on your car' warning message against display");
                Finance = true;//Setting the Is Finance to Yes to check the warning 
                spinner.WaitForSpinnerToFinish();
                ClickNext();
                Reporting.Log($"Page 3: Capture screenshot for warning message.", _browser.Driver.TakeSnapshot());
                Reporting.AreEqual(Constants.WarningText.FinancierWarning, GetInnerText(XPath.WarningField.FinancierWarning), $"'Who your car is financed with' warning message against display");
                Finance = false;//Resetting the Is Finance to default state
            }
        }
    }
}
