using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Text.RegularExpressions;

namespace UIDriver.Pages.Spark.Endorsements
{
    public class YourCaravanOrTrailer : SparkBasePage
    {
        #region CONSTANTS
        private class Constants
        {
            public const string PageHeading = "Your caravan or trailer";
            public const string ConfirmRightMakeModelLabel = "Confirm this is the right make and model";
            public const string WarningRightMakeModelText = "Please confirm the make and model";

            public static class Notification
            {
                public const string Title = "If you've got the same caravan or trailer";
                public const string Content = "You can update your registration and other details next.";
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public const string PageHeading = "//h2[text()='" + Constants.PageHeading +"']";
            public const string ConfirmRightMakeModelLabel = "//label[text()='" + Constants.ConfirmRightMakeModelLabel + "']";
            public const string ConfirmRightMakeModelBtnGrp = "id('yesNoButtonGroup')";
                     
            public static class Button
            {
                public const string Yes = "//button[@id='yesNoButtonGroup-true']";
                public const string No = "//button[@id='yesNoButtonGroup-false']";
                public const string Next = "id('your-caravan-next-button')";
            }

            public static class Warning
            {
                public const string RightMakeAndModel = "//div[@id='yesNoButtonGroup']/../p";
            }

            public class Card
            {
                public static class CaravanDetails
                {
                    public const string Container = "id('policy-card-your-caravan')";
                    public const string YearMake = "id('policy-card-content-policy-details-header-title-your-caravan')";
                    public const string Model = "id('policy-card-content-policy-details-header-subtitle-your-caravan')";
                    public const string Registration = "id('policy-card-content-policy-details-property-0-registration-your-caravan')";
                }

                public static class Notification
                {
                    public const string Icon = "id('your-caravan-notification-icon')";
                    public const string Title = "id('your-caravan-notification-title')";
                    public const string Content = "id('your-caravan-notification-content')";
                }
            }
        }
        #endregion

        public YourCaravanOrTrailer(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PageHeading);
                GetElement(XPath.Card.CaravanDetails.Container);
                Reporting.Log($"Page 1: Your caravan or trailer Details card is Displayed");
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

            Reporting.LogPageChange("Your caravan or trailer");
            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// Select the "Next" button to navigate to the next page in the flow if they have answered 
        /// the "Correct Make & Model" question.
        /// If "Yes, it is" has been selected the user will be navigated to the "Storage and use" step.
        /// If "No, it isn't" has been selected then the user will be diverted to the "update make and model" page instead.
        /// If nothing has been selected yet, a field validation error will be displayed.
        /// </summary>
        public void ClickNext() => ClickControl(XPath.Button.Next);

        /// <summary>
        /// Select the "Yes, it is" button on the Confirm this is the right make and model field.
        /// Select the "No, it isn't" button on the Confirm this is the right make and model field.
        /// </summary>
        public void ClickConfirmOrUpdateMakeandModel(bool confirmCarMakeAndModel)
        {
            ClickBinaryToggle(XPath.ConfirmRightMakeModelBtnGrp, XPath.Button.Yes, XPath.Button.No, confirmCarMakeAndModel);
        }

        /// <summary>
        /// Verify Caravan details card 
        /// Line 1 = Year 
        /// Make Line 2 = Full model description
        /// Line 3 = registration number 
        /// </summary>
        public void VerifyCaravanDetailsCard(EndorseCaravan testData)
        {         
             var caravan = DataHelper.GetVehicleDetails(testData.OriginalPolicyData.CaravanAsset.VehicleId).Vehicles[0];

            Reporting.IsTrue(IsControlDisplayed(XPath.Card.CaravanDetails.Container), "Policy card is present");
            Reporting.AreEqual(GetInnerText(XPath.Card.CaravanDetails.YearMake), $"{caravan.ModelYear} {caravan.MakeDescription}", "model year and description is displayed");
            Reporting.AreEqual(GetInnerText(XPath.Card.CaravanDetails.Model), caravan.ModelDescription, " model description is displayed");

            if (DataHelper.IsRegistrationNumberConsideredValid(testData.OriginalPolicyData.CaravanAsset.RegistrationNumber))
            {
                Reporting.AreEqual(GetInnerText(XPath.Card.CaravanDetails.Registration), 
                    $"Registration: {testData.OriginalPolicyData.CaravanAsset.RegistrationNumber}", "caravan/trailer registration is displayed");
            }
        }

        /// <summary>
        /// Verify Card notification to display at all times telling members if they just want to change rego, they can do that next
        /// </summary>
        public void VerifyCaravanNotificationCard()
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
