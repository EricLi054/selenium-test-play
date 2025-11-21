using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Globalization;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace UIDriver.Pages.Spark.Endorsements
{
    /// <summary>
    /// Presents information about a member's caravan.  Shows car registration, business purpose
    /// </summary>
    public class StorageAndUse : SparkBasePage
    {
        #region CONSTANTS
        private static class Constants
        {
            public static readonly string PageTitle = "Storage and use";

            public static class WarningText
            {
                public static readonly string BusinessUseWarning = "Please select Yes or No";
                public static readonly string ParkingSuburbWarning = "Please enter a valid suburb";
                public static readonly string PlaceKeptWarning = "Please select Yes or No";
                public static readonly string KeptLocationWarning = "Please select the place your caravan or trailer is usually parked";
            }

            public static class Notification
            {
                public static readonly string Title = "Don't forget your registration";
                public static readonly string Content = "It's better if we have your rego but you can continue without it.";
            }

            public static class KnockOut
            {
                public static readonly string Title = "You can't continue online";
                public static readonly string Content = "Please call us on 13 17 03.";
            }

            public static class ToolTip
            {
                public static class Title
                {
                    public static readonly string BusinessUse = "Business use";
                }

                public static class Content
                {
                    public static readonly string BusinessUsePointOne = "We only cover caravans and trailers for personal use such as holidays or permanent accommodation.";
                    public static readonly string BusinessUsePointTwo = "We don't cover them for any business use, for earning income or any other use.";
                }
            }
        }
        #endregion

        #region XPATHS
        private static class XPath
        {
            public static readonly string PageTitle = "//h2[text()='" + Constants.PageTitle + "']";

            public static class Registration
            {
                public static readonly string Label = "id('label-input-registration-number')";
                public static readonly string Field = "id('input-registration-number')";
                public static readonly string Icon = "id('notification-card-icon')";
                public static readonly string InfoHeading = "id('notification-card-title')";
                public static readonly string InfoText = "id('notification-card-content')";
            }

            public static class BusinessUse
            {
                public static readonly string Label = "id('label-caravan-purpose')";
                public static readonly string ButtonGroup = "id('caravan-purpose')";
                public static readonly string Yes = "//button[@id='caravan-purpose-true']";
                public static readonly string No = "//button[@id='caravan-purpose-false']";
            }

            public static class ParkingSuburb
            {
                public static readonly string Label = "id('label-select-suburb')";
                public static readonly string Dropdown = "id('select-suburb')";
                public static readonly string Option = "id('select-suburb-listbox')/li";
                public static readonly string Warning = "id('helper-text-select-suburb')";
            }

            public static class PlaceKept
            {
                public static readonly string Label = "id('label-is-on-site')";
                public static readonly string ButtonGroup = "id('is-on-site')";
                public static readonly string Yes = "//button[@id='is-on-site-true']";
                public static readonly string No = "//button[@id='is-on-site-false']";
            }

            public static class PlaceParked
            {
                public static readonly string Label = "id('label-kept-location')";
                public static readonly string DropdownBase = "id('kept-location')";
                public static readonly string Dropdown = "//div[@id='menu-keptLocation']//ul//li";
                public static readonly string Warning = "id('helper-text-kept-location')";
            }

            public static class Button
            {
                public static readonly string Next = "id('your-caravan-usage-next-button')";
            }

            public static class WarningField
            {
                public static readonly string BusinessUseWarning = "id('helper-text-caravan-purpose')";
                public static readonly string PlaceKeptWarning = "id('helper-text-kept-place')";
            }

            public static class Notification
            {
                public static readonly string Icon = "id('cant-continue-notification-icon')";
                public static readonly string Title = "id('cant-continue-notification-title')";
                public static readonly string Content = "//a[@id='notification-phone-number']/..";
            }

            public static class KnockOut
            {
                public static readonly string Title = "id('caravan-purpose-business-purposes-title')";
                public static readonly string Content = "//a[@id='caravan-purpose-business-purposes-phone-number-link']/..";
            }

            public static class ToolTip
            {
                public static readonly string ButtonBusinessUseShow = "id('tooltip-caravan-purpose-show-button')";
                public static readonly string ButtonBusinessUseClose = "id('tooltip-caravan-purpose-close')";

                public static class Content
                {
                    public static readonly string BusinessUseTitle = "id('tooltip-caravan-purpose-title')";
                    public static readonly string BusinessUsePointOne = "//div[@id='tooltip-caravan-purpose-message']/p[1]";
                    public static readonly string BusinessUsePointTwo = "//div[@id='tooltip-caravan-purpose-message']/p[2]";
                }
            }
        }
        #endregion

        #region Settable properties and controls
        public string CaravanRegistration
        {
            get => GetValue(XPath.Registration.Field);
            set => WaitForTextFieldAndEnterText(XPath.Registration.Field, value, false);
        }
        public bool UsedForBusinessOrIncome
        {
            get => GetBinaryToggleState(XPath.BusinessUse.ButtonGroup, XPath.BusinessUse.Yes, XPath.BusinessUse.No);
            set => ClickBinaryToggle(XPath.BusinessUse.ButtonGroup, XPath.BusinessUse.Yes, XPath.BusinessUse.No, value);
        }
        public string ParkingSuburb
        {
            get => GetValue(XPath.ParkingSuburb.Dropdown);
            set => WaitForSelectableFieldToSearchAndPickFromDropdown(XPath.ParkingSuburb.Dropdown, XPath.ParkingSuburb.Option, value);
        }

        /// <summary>
        /// Control for question about whether caravan/trailer is
        /// kept in one place, or is it towed. TRUE means "Yes" it
        /// is kept in one place, like an on-site caravan. FALSE
        /// means "No" it is towed, such as a caravan that is
        /// parked at home and towed places for use.
        /// </summary>
        public bool PlaceKept
        {
            get => GetBinaryToggleState(XPath.PlaceKept.ButtonGroup, XPath.PlaceKept.Yes, XPath.PlaceKept.No);
            set => ClickBinaryToggle(XPath.PlaceKept.ButtonGroup, XPath.PlaceKept.Yes, XPath.PlaceKept.No, value);
        }

        public string Parked
        {
            get => GetValue(XPath.PlaceParked.Dropdown);
            set => WaitForSelectableAndPickFromDropdown(XPath.PlaceParked.DropdownBase, XPath.PlaceParked.Dropdown, value);
        }


        #endregion

        public StorageAndUse(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PageTitle);
                GetElement(XPath.BusinessUse.Label);
                GetElement(XPath.ParkingSuburb.Label);
                GetElement(XPath.PlaceKept.Label);
                Reporting.Log($"Page 2: Business Purpose/Parking suburb/Place Kept question is visible");
                GetElement(XPath.Button.Next);
                Reporting.Log($"Page 2: Next Button is Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Storage and use page");
            Reporting.Log($"Page 2: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        public void ClickNext()
        {
            Reporting.Log($"Screen capture of 'Storage and Use' Page before clicking NEXT button", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.Next);
        }

        /// <summary>
        /// Check the presentation of the vehicle registration and associated information.
        /// When the registration is consider interim (eg TBA), then the value will be empty.
        /// Also, no special characters will be displayed.  So a registration with a hyphen will 
        /// get the hyphen removed.  eg RAC-123 becomes RAC123.
        /// </summary>
        public void VerifyExistingCaravanDetails(EndorseCaravan endorseCaravan)
        {
            string caravanParked = ParkingSuburb.ToUpper(CultureInfo.InvariantCulture);

            if (DataHelper.IsRegistrationNumberConsideredValid(endorseCaravan.OriginalPolicyData.CaravanAsset.RegistrationNumber))
            {
                Reporting.AreEqual(endorseCaravan.OriginalPolicyData.CaravanAsset.RegistrationNumber.SanitizeVehicleRegistrationString(), CaravanRegistration, "caravan registration number populated");
            }
            else
            {
                Reporting.AreEqual("", CaravanRegistration, "caravan registration number left blank when registration considered temporary or otherwise invalid");
            }

            // Checking the 'Your caravan or trailer is used for business purposes or for earning income' is defaulted to 'No'
            Reporting.IsFalse(UsedForBusinessOrIncome, "Business Use of the caravan is defaulted to 'No'");
          
            if (endorseCaravan.ChangeMakeAndModel)
            {
                Reporting.AreEqual(string.Empty, caravanParked, "Suburb were the caravan is kept is displayed as blank for change in make and model");
            }
            else
            {
                Reporting.IsTrue(caravanParked.StartsWith(endorseCaravan.OriginalPolicyData.CaravanAsset.Suburb), $"Suburb were the caravan is kept is displayed with '{endorseCaravan.OriginalPolicyData.CaravanAsset.Suburb}' for no change in make and model");
            }

            if (endorseCaravan.OriginalPolicyData.Covers.First().CoverTypeDescription.Contains("On-Site"))
            {
                Reporting.IsTrue(PlaceKept, "'Your caravan or trailer is kept in one place, like a caravan park, and isn't towed' is set as 'Yes' for On-site parked Caravan");
                Reporting.IsFalse(IsControlDisplayed(XPath.PlaceParked.Label), "'Place your caravan or trailer is usually parked' label is not displayed");
                Reporting.IsFalse(IsControlDisplayed(XPath.PlaceParked.DropdownBase), "'Place your caravan or trailer is usually parked' dropdown is not displayed");
            }
            else
            {
                Reporting.IsFalse(PlaceKept, "'Your caravan or trailer is kept in one place, like a caravan park, and isn't towed' is set as 'No' for On-site parked Caravan");
                Reporting.IsTrue(IsControlDisplayed(XPath.PlaceParked.Label), "'Place your caravan or trailer is usually parked' label is displayed");
                Reporting.IsTrue(IsControlDisplayed(XPath.PlaceParked.DropdownBase), "'Place your caravan or trailer is usually parked' dropdown is displayed");
            }
        }

        /// <summary>
        /// Verify Card notification in case registration number is not displayed
        /// </summary>
        public void VerifyCarRegistrationNotificationCard(EndorseCaravan endorseCaravan)
        {
            if (string.IsNullOrEmpty(endorseCaravan.OriginalPolicyData.CaravanAsset.RegistrationNumber) || 
                (endorseCaravan.OriginalPolicyData.CaravanAsset.RegistrationNumber.Equals("TBA")) || 
                endorseCaravan.ChangeMakeAndModel)
            {
                Reporting.IsTrue(IsControlDisplayed(XPath.Registration.Icon), "registration notification info box is displayed");
                Reporting.AreEqual(Constants.Notification.Title, GetInnerText(XPath.Registration.InfoHeading), $"expected title of notification panel against actual");
                Reporting.AreEqual(Constants.Notification.Content, GetInnerText(XPath.Registration.InfoText), $"expected content of notification panel against actual");
            }
            else
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.Registration.Icon), "registration notification info box is NOT displayed");
            }       
        }

        /// <summary>
        /// Open, verify and close car Business Use tool tip
        /// </summary>
        public void VerifyBusinessUseToolTip()
        {
            ClickControl(XPath.ToolTip.ButtonBusinessUseShow);
            Reporting.AreEqual(Constants.ToolTip.Title.BusinessUse, GetInnerText(XPath.ToolTip.Content.BusinessUseTitle), $"expected title of caravan used for tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.BusinessUsePointOne, GetInnerText(XPath.ToolTip.Content.BusinessUsePointOne), $"expected bullet point 1 text of caravan Business Use tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.BusinessUsePointTwo, GetInnerText(XPath.ToolTip.Content.BusinessUsePointTwo), $"expected bullet point 2 text of caravan Business Use tool tip against actual");
            Reporting.Log($"Cravan Business Use.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.ToolTip.ButtonBusinessUseClose);
        }

        /// <summary>
        /// Entering new caravan details on the storage and use page
        /// </summary>
        /// <param name="endorseCaravan"></param>
        public void EnterNewCaravanDetails(EndorseCaravan endorseCaravan)
        {
            if (endorseCaravan.ChangeMakeAndModel)
            {
                CaravanRegistration = endorseCaravan.NewInsuredAsset.Registration;
                // Setting the 'Your caravan or trailer is used for business purposes or for earning income' to 'No'
                // We only do business for Private use
                UsedForBusinessOrIncome = false;

                // If we have the postcode, apply it in the search, as some suburbs have similar names
                // such as "Manning" and "Manmanning" and we want to make sure we get the correct one. 
                var caravanAsset = endorseCaravan.OriginalPolicyData.CaravanAsset;
                var suburbSearchString = $"{caravanAsset.Suburb} - {caravanAsset.Postcode}";
                ParkingSuburb = suburbSearchString;
                if (!string.IsNullOrEmpty(endorseCaravan.Parked.GetDescription()))
                {
                    PlaceKept = false;
                    Parked = endorseCaravan.Parked.GetDescription();
                }
                else
                {
                    PlaceKept = true;
                }
            }

            // This will update only the Rego of the Insured caravan. Usefull in a scenario where there is no
            // change to the make and model but only rego is changed
            if (!string.IsNullOrEmpty(endorseCaravan.NewInsuredAsset?.Registration) && !endorseCaravan.ChangeMakeAndModel)
            {
                CaravanRegistration = endorseCaravan.NewInsuredAsset.Registration;
            }

            // This will update only the parking location of the Insured caravan. Used for scenarios where there is no
            // change to the make and model but only parked is changed
            if (!string.IsNullOrEmpty(endorseCaravan.NewInsuredAsset?.ParkLocation.GetDescription()) 
                && !endorseCaravan.ChangeMakeAndModel 
                && !endorseCaravan.ExpectedImpactOnPremium.Equals(PremiumChange.NoChange))
            {
                var ParkLoc = endorseCaravan.NewInsuredAsset.ParkLocation.GetDescription();
                var xx = ParkLoc.Equals(CaravanParkLocation.OnSite) || ParkLoc.Equals(CaravanParkLocation.CommunalCarpark) ? "Other" : ParkLoc;
                Parked = xx;
            }
        }

        /// <summary>
        /// Verifying the Knock out message resulted from Selecting Yes to Caravan purpose question
        /// </summary>
        public void VerifyBusinessUseKnockOutMessage()
        {
            //Setting the Business Use field to Yes to check the Knockout message
            UsedForBusinessOrIncome = true;
            Reporting.AreEqual(Constants.KnockOut.Title, GetInnerText(XPath.KnockOut.Title), $"expected title of knockout message against actual");
            Reporting.AreEqual(Constants.KnockOut.Content, GetInnerText(XPath.KnockOut.Content), $"expected content of knockout message against actual");
            Reporting.Log($"Knock Out message.", _browser.Driver.TakeSnapshot());
        }

        public void VerifyEmptySuburbValidationError()
        {
            ClickNext();
            Reporting.AreEqual(Constants.WarningText.ParkingSuburbWarning, GetInnerText(XPath.ParkingSuburb.Warning), $"expected warning message against actual");
        }
    }
}
