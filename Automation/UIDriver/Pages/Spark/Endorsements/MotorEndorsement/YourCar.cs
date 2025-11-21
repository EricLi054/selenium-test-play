using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Text.RegularExpressions;

namespace UIDriver.Pages.Spark.Endorsements
{
    public class YourCar : SparkBasePage
    {
        #region CONSTANTS
        private class Constants
        {
            public const string PageHeading = "Your car";
            public const string ConfirmRightMakeModelLabel = "Confirm this is the right make and model";
            public const string WarningRightMakeModelText = "Please confirm the make and model";

            public class Notification
            {
                public const string Title = "If you've got the same car";
                public const string Content = "You can update your registration and other car details next.";
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public const string PageHeading = "//h2[text()='" + Constants.PageHeading +"']";
            public const string ConfirmRightMakeModelLabel = "//label[text()='" + Constants.ConfirmRightMakeModelLabel + "']";
            public const string ConfirmRightMakeModelBtnGrp = "id('yesNoButtonGroup')";
                     
            public class Button
            {
                public const string Yes = "//button[@id='yesNoButtonGroup-true']";
                public const string No = "//button[@id='yesNoButtonGroup-false']";
                public const string Next = "id('your-car-next-button')";
            }

            public class Warning
            {
                public const string RightMakeAndModel = "//div[@id='yesNoButtonGroup']/../p";
            }

            public class Card
            {
                public class CarDetails
                {
                    public const string Container = "id('policy-card-your-car')";
                    public const string YearMake = "id('policy-card-content-policy-details-header-title-your-car')";
                    public const string Model = "id('policy-card-content-policy-details-header-subtitle-your-car')";
                    public const string Registration = "id('policy-card-content-policy-details-property-0-registration-your-car')";
                }

                public class Notification
                {
                    public const string Icon = "id('notification-card-icon')";
                    public const string Title = "id('notification-card-title')";
                    public const string Content = "id('notification-card-content')";
                }
            }
        }
        #endregion

        public YourCar(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PageHeading);
                GetElement(XPath.Card.CarDetails.Container);
                Reporting.Log($"Page 1: Your Car Details card is Displayed");
                GetElement(XPath.ConfirmRightMakeModelLabel);
                GetElement(XPath.Button.Yes);
                GetElement(XPath.Button.No);
                Reporting.Log($"Page 1: Confirm this is the right make and model Buttons are Displayed");
                GetElement(XPath.Button.Next);
                Reporting.Log($"Page 1: Next Button is Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Your Car Page");
            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// Select the "Next" button to navigate to the next page in the flow.
        /// If "Yes, it is" has been selected the user will be navigated to the "Your car details" step.
        /// If "No, it isn't" has been selected then the user will be diverted to the "Lets update your car" page instead.
        /// </summary>
        public void ClickNext() => ClickControl(XPath.Button.Next);

        /// <summary>
        /// Select the "Yes, it is" button on the Confirm this is the right make and model field.
        /// Select the "No, it isn't" button on the Confirm this is the right make and model field.
        /// </summary>
        public void ClickConfirmOrUpdateMakeandModel(bool confirmCarMakeAndModel = false)
        {
            ClickBinaryToggle(XPath.ConfirmRightMakeModelBtnGrp, XPath.Button.Yes, XPath.Button.No, confirmCarMakeAndModel);
        }

        /// <summary>
        /// Verify Car details card Line 1 = Year 
        /// Make Line 2 = Full model description
        /// Line 3 = registration number 
        /// </summary>
        /// <param name="car"></param>
        public void VerifyCarDetailsCard(Car car)
        {
            Reporting.AreEqual($"{car.Year} {car.Make}", GetInnerText(XPath.Card.CarDetails.YearMake), $"expected Year and Make of vehicle against display");
            Reporting.AreEqual(car.Model, GetInnerText(XPath.Card.CarDetails.Model), $"expected Model of vehicle against display");
            if (DataHelper.IsRegistrationNumberConsideredValid(car.Registration))
            {
                Reporting.AreEqual($"Registration: {car.Registration.SanitizeVehicleRegistrationString()}", GetInnerText(XPath.Card.CarDetails.Registration), $"expected Registration of vehicle against display");
            }
        }

        /// <summary>
        /// Verify Card notification to display at all times telling members if they just want to change rego, they can do that next
        /// </summary>
        public void VerifyCarNotificationCard()
        {
            Reporting.IsTrue(IsControlDisplayed(XPath.Card.Notification.Icon), "Notification Icon Should be displayed");
            Reporting.AreEqual(Constants.Notification.Title, GetInnerText(XPath.Card.Notification.Title), $"expected title of notification panel against actual");
            Reporting.AreEqual(Constants.Notification.Content, GetInnerText(XPath.Card.Notification.Content), $"expected content of notification panel against actual");
        }

        public void VerifyWarningMessage()
        {
            Reporting.Log($"Page 1: Capture screenshot for warning message.", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual(Constants.WarningRightMakeModelText, GetInnerText(XPath.Warning.RightMakeAndModel), $"Right make and model against display");
        }
    }
}
