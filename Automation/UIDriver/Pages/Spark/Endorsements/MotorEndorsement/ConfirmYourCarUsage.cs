using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace UIDriver.Pages.Spark.Endorsements
{
    public class ConfirmYourCarUsage : SparkBasePage
    {
        #region CONSTANTS
        private class Constants
        {
            public const string Title = "Confirm your car usage";
            public const string KMDrivenAnnuallyLabel = "Kilometres driven annually";
            public const string RiskAddressLabel = "Address where your car is usually kept";
            public const string KnockOutMessageText = "You can't continue online\r\nPlease call us on 13 17 03.";

            public class IncomeDerivative
            {
                public const string BusinessLabel = "Is your car used to make an income?";
                public const string RideshareLabel = "Car used for ridesharing for 30 hours or more per week";
                public const string BusinessText = "Your car is not used to make an income if it's used by a tradesperson or other profession to drive to jobs.";
            }

            public class ToolTip
            {
                public class Title
                {
                    public const string CarUse = "Car use";
                    public const string Address = "Address";
                    public const string BusinessCarUse = "Car used to make an income";

                }
                public class Content
                {
                    public const string CarUsePrivate = "Private use: Your car is used for social or domestic purposes or travelling to work.";
                    public const string CarUseBusiness = "Business use: Your car is registered for business use and used for business. This includes use by a tradesperson. Business use doesn't include using your car for ridesharing or courier or delivery.";
                    public const string CarUseRideshare = "Ridesharing: Your car is used to provide rides to passengers for a fee. Part-time is less than 30 hours a week. Full-time is 30 hours or more.";
                    public const string CarUseDontCoverContentTitle = "What we don't cover";
                    public const string CarUseDontCover = "We don't cover courier, delivery, cars that are hired or leased out, taxis, small charter vehicles, limousines, chauffeured vehicles or full-time ridesharing.";
                    public const string CarUseCourier = "Courier and delivery are where your car is used to deliver food or goods for a fee.";
                    public const string Address = "We only insure cars based in Western Australia. If you now keep your car outside WA, please call us on 13 17 03.";
                    public const string BusinessCarUse = "Your car is used to make an income if it's used for courier, delivery, hiring or leasing out, or as a taxi, small charter vehicle, limousine, chauffered vehicle or for full-time ridesharing.";
                }
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public const string Title = "//h2[text()='" + Constants.Title + "']";
            public const string CarUsage = "id('vehicle-usage-dropdown')";
            public const string Option = "//ul[@role='listbox']" + "//li";

            public class RiskAddress
            {
                public const string Label = "//label[text()='" + Constants.RiskAddressLabel + "']";
                public const string TextInputField = FORM + "//input[@id='risk-address-lookup']";
                public const string Selection = "id('risk-address-lookup-listbox')/li[not(contains(text(),'Loading'))]";
            }

            public class KmDrivenAnnually
            {
                public const string Label = "//label[text()='" + Constants.KMDrivenAnnuallyLabel + "']";
                public const string Dropdown = "id('kilometres-driven-annually')";
                public const string DropdownOptions = "//div[@id='menu-annualKm']//li";
            }

            public class Button
            {
                public const string Next = "id('your-car-usage-next-button')";
                public const string Yes = "//button[@aria-label='Yes']";
                public const string No = "//button[@aria-label='No']";
            }

            public class IncomeDerivative
            {
                public const string BusinessQuestionLabel = "id('income-directly-derived-from-car-label')";
                public const string BusinessQuestionText = "id('income-directly-derived-from-car-sublabel')";
                public const string RideShareQuestionLabel = "id('ridesharing-hours-per-week-label')";
                public const string RideShareQuestionButtonGroup = "id('ridesharing-hours-per-week')";
                public const string BusinessQuestionButtonGroup = "id('income-directly-derived-from-car')";
                public const string IncomeDerivativeKnockOutMsg = "id('cant-continue-notification')";
            }

            public class ToolTip
            {
                public class ShowButton
                {
                    public const string CarUse = "id('vehicle-usage-tooltipButton')";
                    public const string Address = "id('tooltip-riskAddressButton')";
                    public const string BusinessCarUse = "id('income-directly-derived-from-car-tooltipButton')";
                }

                public class Title
                {
                    public const string CarUse = "id('vehicle-usage-dialog-title')";
                    public const string Address = "id('tooltip-riskAddress-title')";
                    public const string BusinessCarUse = "id('income-directly-derived-from-car-tooltip-title')";
                }

                public class Content
                {
                    public const string CarUsePrivateContent = "id('vehicle-usage-dialog-private')";
                    public const string CarUseBusinessContent = "id('vehicle-usage-dialog-business')";
                    public const string CarUseRideshareContent = "id('vehicle-usage-dialog-ridesharing')";
                    public const string CarUseDontCoverContentTitle = "id('vehicle-usage-dialog-uncovered-title')";
                    public const string CarUseDontCoverContent = "id('vehicle-usage-dialog-uncovered')";
                    public const string CarUseCourierContent = "id('vehicle-usage-dialog-courier')";

                    public const string Address = "id('tooltip-riskAddress-message-text')";

                    public const string BusinessCarUse = "id('income-directly-derived-from-car-tooltip-message')";
                }

                public class CloseButton
                {
                    public const string CarUse = "//h2[@id='vehicle-usage-dialog-title']/button";
                    public const string Address = "id('tooltip-riskAddress-close')";
                    public const string BusinessCarUse = "id('income-directly-derived-from-car-tooltip-close')";
                }

            }
        }
        #endregion

        #region Settable properties and controls
        public string CarUsage
        {
            get => GetInnerText(XPath.CarUsage);
            set => WaitForSelectableAndPickFromDropdown(XPath.CarUsage, XPath.Option, value);
        }

        public string RiskAddress
        {
            get => GetValue(XPath.RiskAddress.TextInputField);
            set => QASSearchForAddress(XPath.RiskAddress.TextInputField, XPath.RiskAddress.Selection, value);
        }

        public string KmDrivenAnnually
        {
            get => GetInnerText(XPath.KmDrivenAnnually.Dropdown);
            set => WaitForSelectableAndPickFromDropdown(XPath.KmDrivenAnnually.Dropdown, XPath.KmDrivenAnnually.DropdownOptions, value);
        }

        /// <summary>
        /// This method involves clicking the appropriate element based on the car's usage, whether it is for business or part-time rideshare purposes. 
        /// </summary>
        /// <param name="carUsage">Car Usage from test data, either Business or Ridesharing Parttime</param>
        /// <param name="value">Value to set the question (default = false)</param>
        public void SetIncomeDerivativeQuestion(VehicleUsage carUsage, bool value = false)
        {
            ClickBinaryToggle(carUsage == VehicleUsage.Business ? XPath.IncomeDerivative.BusinessQuestionButtonGroup
                : XPath.IncomeDerivative.RideShareQuestionButtonGroup,
                XPath.Button.Yes, XPath.Button.No, value);
        }

        #endregion

        public ConfirmYourCarUsage(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Title);
                GetElement(XPath.CarUsage);
                Reporting.Log($"Page 1: Car usage options are displayed");
                GetElement(XPath.Button.Next);
                Reporting.Log($"Page 1: Next Button is Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Confirm Your Car Usage page");
            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// This method involves clicking the next button to proceed to the next page.
        /// By doing so, the Motor endorsement will advance to the subsequent stage of the process.
        /// </summary>
        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }

        /// <summary>
        /// This method aims to verify the pre-populated values on the Car Usage page, including car usage, risk address, and kilometers traveled. 
        /// Its purpose is to ensure the accuracy and reliability of the pre-filled information.
        /// </summary>
        public void VerifyCarInformation(GetQuotePolicy_Response expectedTestData)
        {
            VerifyInitialCarUsage(expectedTestData.MotorAsset.GetVehicleUsage());
            if (expectedTestData.MotorAsset.Address != null && !string.IsNullOrEmpty(expectedTestData.MotorAsset.Address.GnafPid))
            {
                // Only addresses with a GNAF will pre-populate.
                Reporting.IsTrue(expectedTestData.MotorAsset.Address.IsEqualToString(RiskAddress), "Verifying Risk Address");
            }
            Reporting.AreEqual(expectedTestData.MotorAsset.GetAnnualKms, KmDrivenAnnually, $"expected Annual KMs Driven vs actual value");
            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
        }

        /// <summary>
        /// This method involves clicking to verify the pre-populated data displayed in the car usage field.
        /// </summary>
        private void VerifyInitialCarUsage(VehicleUsage carUsage)
        {
            var expectedCarUsageText = VehicleUsageNameMappings[carUsage].TextSpark;
            switch (carUsage)
            {
                case VehicleUsage.Private:
                    Reporting.AreEqual(expectedCarUsageText, CarUsage, $"Initial Car Usage: Expected :{carUsage.ToString()} Actual: {CarUsage}");
                    break;
                case VehicleUsage.Business:
                case VehicleUsage.Ridesharing:
                    Reporting.AreEqual(expectedCarUsageText, CarUsage, $"Initial Car Usage: Expected :{carUsage.ToString()} Actual: {CarUsage}");
                    VerifyIncomeDerivativeQuestionDisplayed(carUsage);
                    break;
                default:
                    // Other usages are: Courier or delivery |  Hired or leased out | Taxi or small charter | Limo or Chauffeured 
                    // These usages are unacceptable and are to blocked from proceeding via the web site. 
                    VerifyKnockoutMessage();
                    break;
            }
        }

        /// <summary>
        /// Change vehicle usage.  Revert to the original value for usages that would knockout the member from renewing.
        /// </summary>
        public void SelectNewCarUsage(VehicleUsage newCarUsage)
        {
            // If undefined, we do nothing and skip.
            if (newCarUsage == VehicleUsage.Undefined)
            { return; }

            var originalCarUsage = CarUsage;// Buffering in case we want to revert.

            CarUsage = VehicleUsageNameMappings[newCarUsage].TextSpark;

            switch (newCarUsage)
            {
                case VehicleUsage.Private:
                    break;
                case VehicleUsage.Business:
                case VehicleUsage.Ridesharing:
                    VerifyIncomeDerivativeQuestionDisplayed(newCarUsage);
                    break;
                default:
                    VerifyKnockoutMessage();
                    CarUsage = originalCarUsage;// Returns it to it's original Car Usage.
                    break;
            }
        }

        public void SelectRiskAddress(EndorseCar endorseCar)
        {
            if (string.IsNullOrEmpty(RiskAddress))
            {
                RiskAddress = endorseCar.OriginalPolicyData.MotorAsset.RiskLocation(expandUnitAddresses: true);
            }
        }

        /// <summary>
        /// This method is used to verify the display of all of the different Income derivative question.
        /// The function will leave the dropdown with the original state/value after checking
        /// </summary>
        public void VerifyCarUsageWarningMessage()
        {
            SelectNewCarUsage(VehicleUsage.Business);
            SelectNewCarUsage(VehicleUsage.Ridesharing);
            SelectNewCarUsage(VehicleUsage.CourierOrDelivery);
            SelectNewCarUsage(VehicleUsage.HiredOrleasedOutSmall);
            SelectNewCarUsage(VehicleUsage.TaxiOrSmallCharter);
            SelectNewCarUsage(VehicleUsage.LimoOrChauffeured);
        }

        /// <summary>
        /// Open, verify and close Address tool tip
        /// </summary>
        public void VerifyAddressToolTip()
        {
            ClickControl(XPath.ToolTip.ShowButton.Address);
            Reporting.AreEqual(Constants.ToolTip.Title.Address, GetInnerText(XPath.ToolTip.Title.Address), $"expected title of address tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.Address, GetInnerText(XPath.ToolTip.Content.Address), $"expected content of address tool tip against actual");
            ClickControl(XPath.ToolTip.CloseButton.Address);
        }
        /// <summary>
        /// Open, verify and close car use tool tip
        /// </summary>
        public void VerifyCarUseToolTip()
        {
            ClickControl(XPath.ToolTip.ShowButton.CarUse);
            Reporting.AreEqual(Constants.ToolTip.Title.CarUse, GetInnerText(XPath.ToolTip.Title.CarUse), $"expected title of car use tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.CarUsePrivate, GetInnerText(XPath.ToolTip.Content.CarUsePrivateContent), $"expected private content text on car use tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.CarUseBusiness, GetInnerText(XPath.ToolTip.Content.CarUseBusinessContent), $"expected business content text on car use tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.CarUseDontCoverContentTitle, GetInnerText(XPath.ToolTip.Content.CarUseDontCoverContentTitle), $"expected dont cover title on car use tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.CarUseDontCover, GetInnerText(XPath.ToolTip.Content.CarUseDontCoverContent), $"expected dont cover content text on car use tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.CarUseCourier, GetInnerText(XPath.ToolTip.Content.CarUseCourierContent), $"expected Courier content text on car use tool tip against actual");
            ClickControl(XPath.ToolTip.CloseButton.CarUse);
        }

        /// <summary>
        /// Set the value of Vehicle user to Business, Open, verify and close  tool tip
        /// Set the vehicle usage the defualt value
        /// </summary>
        public void VerifyBusinessCarUseToolTip(EndorseCar endorseCar)
        {
            SelectNewCarUsage(VehicleUsage.Business);
            ClickControl(XPath.ToolTip.ShowButton.BusinessCarUse);
            Reporting.AreEqual(Constants.ToolTip.Title.BusinessCarUse, GetInnerText(XPath.ToolTip.Title.BusinessCarUse), $"expected title of Business car tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.BusinessCarUse, GetInnerText(XPath.ToolTip.Content.BusinessCarUse), $"expected content of Business car tool tip against actual");
            ClickControl(XPath.ToolTip.CloseButton.BusinessCarUse);
            SelectNewCarUsage(endorseCar.OriginalPolicyData.MotorAsset.GetVehicleUsage());
        }

        /// <summary>
        /// This method is used to verify the display of the Income derivative question.
        /// By clicking "Yes," it triggers the knockout message to appear.
        /// After verification, it clicks "No" to allow the test case to proceed.
        /// </summary>
        private void VerifyIncomeDerivativeQuestionDisplayed(VehicleUsage carUsage)
        {
            if (carUsage == VehicleUsage.Business)
            {
                Reporting.AreEqual(Constants.IncomeDerivative.BusinessLabel, GetInnerText(XPath.IncomeDerivative.BusinessQuestionLabel));
                Reporting.AreEqual(Constants.IncomeDerivative.BusinessText, GetInnerText(XPath.IncomeDerivative.BusinessQuestionText));
            }
            else if (carUsage == VehicleUsage.Ridesharing)
            {
                Reporting.AreEqual(Constants.IncomeDerivative.RideshareLabel, GetInnerText(XPath.IncomeDerivative.RideShareQuestionLabel));
            }

            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            if ((carUsage == VehicleUsage.Business || carUsage == VehicleUsage.Ridesharing))
            {
                SetIncomeDerivativeQuestion(carUsage, true); // Clicking "Yes," it triggers the knockout message to appear. 
                VerifyKnockoutMessage();
                SetIncomeDerivativeQuestion(carUsage, false); // Clicks "No" to allow the test case to proceed.
            }

            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
        }


        /// <summary>
        /// This method is used to verify if the knockout message is displayed.
        /// </summary>
        private void VerifyKnockoutMessage()
        {
            Reporting.AreEqual(Constants.KnockOutMessageText, GetInnerText(XPath.IncomeDerivative.IncomeDerivativeKnockOutMsg), $"Knockout Message Should be Equal");
            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
        }


    }
}
