using Rac.TestAutomation.Common;
using System.Collections.Generic;

namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class AboutYourCar : BaseMotorClaimPage
    {
        #region CONSTANTS
        private class Constants
        {
            public static readonly string ActiveStepperLabel = "About your car";
            public static readonly string HeaderText = "About your car";
            public static string ClaimNumberText(string claimNumber) => $"Your claim number is {claimNumber}";

            public class Label
            {
                public static readonly string DamageDescription = "Please describe the damage to your carWhat parts were damaged and how badly, e.g. right tail-light cracked.";
                public static readonly string WasYourCarTowed = "Was your car towed?";
                public static readonly string DrivedToRepairer = "Can your car be safely driven to the repairer?";
                public static readonly string PreferredSuburb = "Preferred area for repairs";
                public static readonly string RepairerOutsideWA = "I'd like a repairer outside WA.";
            }
        }

        #endregion

        #region XPATHS

        private class XPath
        {
            public static readonly string Header = "id('about-your-car-header')";
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";

            public class Field
            {
                public static readonly string DamageDescription = "id('damage-description')";
            }

            public class Button
            {
                public static readonly string Yes = "//button[@aria-label='Yes']";
                public static readonly string No = "//button[@aria-label='No']";
                public static readonly string NotSure = "//button[@aria-label=\"I'm not sure\"]";
                public static readonly string Next = "id('submit')";
            }
            public class Toggle
            {
                public static readonly string WasYourCarTowed = "id('car-was-towed')";
                public static readonly string CanSafelyDrivenToRepairer = "id('car-is-driveable')";              
            }
            public class Dropdown
            {
                public static readonly string Suburb = "id('suburb')";               
                public static readonly string FirstSuburb = "id('suburb-option-0')";
            }
            public class Checkbox
            {
                public static readonly string RepairerOutsideWA = "id('find-repairer-outside-wa')";
            }
            public class Label
            {
                public static readonly string DamageDescription = "//label[@for='damage-description']";
                public static readonly string WasYourCarTowed = "//label[@for='car-was-towed']";
                public static readonly string DrivedToRepairer = "//label[@for='car-is-driveable']";
                public static readonly string PreferredSuburb = "//label[@for='suburb']";
                public static readonly string RepairOutsideWA = "id('find-repairer-outside-wa-checkbox-label')";
            }
        }

        #endregion

        #region Settable properties and controls

        private string Header => GetInnerText(XPath.Header);
        private string ClaimNumber => GetInnerText(XPath.ClaimNumber);
        private string DamageDescription
        {
            get => GetInnerText(XPath.Field.DamageDescription);
            set => WaitForTextFieldAndEnterText(XPath.Field.DamageDescription, value);
        }
        private bool? CarTowed
        {
            get => GetNullableBinaryForTriStateToggle(XPath.Toggle.WasYourCarTowed, XPath.Button.Yes, XPath.Button.No, XPath.Button.NotSure);
            set => ClickTriStateToggleWithNullableInput(XPath.Toggle.WasYourCarTowed, XPath.Button.Yes, XPath.Button.No, XPath.Button.NotSure, value);
        }
        private bool? CarDrivenToRepairer
        {
            get => GetNullableBinaryForTriStateToggle(XPath.Toggle.CanSafelyDrivenToRepairer, XPath.Button.Yes, XPath.Button.No, XPath.Button.NotSure);
            set => ClickTriStateToggleWithNullableInput(XPath.Toggle.CanSafelyDrivenToRepairer, XPath.Button.Yes, XPath.Button.No, XPath.Button.NotSure, value);
        }

        private string Suburb
        {
            get => GetValue(XPath.Dropdown.Suburb);
            set => QASSearchForAddress(XPath.Dropdown.Suburb, XPath.Dropdown.FirstSuburb, value);
        }

        private bool RepairerOutsideWA
        {
            get => GetValue(XPath.Checkbox.RepairerOutsideWA) == "true";
            set => ClickControl(XPath.Checkbox.RepairerOutsideWA);
        }

        #endregion


        public AboutYourCar(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Button.Next);
            }
            catch
            {
                return false;
            }

            Reporting.LogPageChange("About your car");
            Reporting.Log("About your car immediately after loading confirmed", _driver.TakeSnapshot());
            return true;
        }

        public void DetailedUiChecking()
        {
            CarTowed = false;
            CarDrivenToRepairer = true;

            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            Reporting.AreEqual(Constants.Label.DamageDescription, GetInnerText(XPath.Label.DamageDescription).StripLineFeedAndCarriageReturns(false), "Please describe the damage to your car question label");
            Reporting.AreEqual(Constants.Label.WasYourCarTowed, GetInnerText(XPath.Label.WasYourCarTowed), "Was your car towed question label");
            Reporting.AreEqual(Constants.Label.DrivedToRepairer, GetInnerText(XPath.Label.DrivedToRepairer), "Can your car be safely driven to the repairer question label");
            Reporting.AreEqual(Constants.Label.DrivedToRepairer, GetInnerText(XPath.Label.DrivedToRepairer), "Preferred area for repairs question label");
            Reporting.AreEqual(Constants.Label.RepairerOutsideWA, GetInnerText(XPath.Label.RepairOutsideWA), "Repairer outside WA question label");
        }

        public void EnterCarDamageAndTowedDetails(ClaimCar claimCar)
        {
            Reporting.AreEqual(Constants.HeaderText, Header, "Page header");
            Reporting.AreEqual(Constants.ClaimNumberText(claimCar.ClaimNumber), ClaimNumber, "Claim number");

            DamageDescription = claimCar.DamageToPHVehicle;
            CarTowed = claimCar.TowedVehicleDetails.WasVehicleTowed;

            if (CarTowed == false)
            {
                CarDrivenToRepairer = claimCar.IsVehicleDriveable;

                if (CarDrivenToRepairer == true)
                {
                    Suburb = claimCar.PreferredRepairerSuburb.SuburbAndCode();
                }
            }

            Reporting.Log("About your car - Before clicking Next Button", _driver.TakeSnapshot());
            ClickNext();
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }

        public List<string> GetPercyIgnoreCSS() =>
         new List<string>()
         {
               "#claimNumberDisplay span"
         };

    }
}
