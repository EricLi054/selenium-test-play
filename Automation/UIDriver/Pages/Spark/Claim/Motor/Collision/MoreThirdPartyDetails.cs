using System.Linq;
using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;

namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class MoreThirdPartyDetails : BaseMotorClaimPage
    {
        #region CONSTANTS
        private static class Constants
        {
            public static readonly string SingleVehicleActiveStepperLabel = "More on the owner";
            public static readonly string MultiVehicleActiveStepperLabel = "Other vehicle owner";
            public static readonly string MultiVehicleHeaderText = "Owner of the other vehicle";
            public static readonly string SingleVehicleHeaderText = "More on the owner";

            public static string ClaimNumberText(string claimNumber) => $"Your claim number is {claimNumber}";
        }
        #endregion

        #region XPATHS

        private static class XPath
        {
            public static readonly string Header = "id('third-party-owner-details-header')";
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";

            public static class Toggle
            {
                public static readonly string WasDriverTheOwner = "id('is-driver-the-owner')";
            }
            public static class Field
            {
                public static readonly string OwnerNameAndContactDetails = "id('owners-name-and-contact-details')";                
                public static readonly string ThirdPartyClaimNumber = "id('third-party-owner-claim-number')";
                public static readonly string DamageDescription = "id('damage-description')";
            }
            public static class Dropdown
            {
                public static readonly string InsuranceCompany = "id('owners-insurance-company')";
                public static readonly string Option = "//ul[@role='listbox']/li";
            }
            public static class Button
            {
                public static readonly string Yes = "//button[@aria-label='Yes']";
                public static readonly string No = "//button[@aria-label='No']";
                public static readonly string Next = "id('submit-button')";
            }
        }
        #endregion

        #region Settable properties and controls

        private string Header => GetInnerText(XPath.Header);
        private bool WasDriverTheOwner
        {
            get => GetBinaryToggleState(XPath.Toggle.WasDriverTheOwner, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.Toggle.WasDriverTheOwner, XPath.Button.Yes, XPath.Button.No, value);
        }
        #endregion

        public MoreThirdPartyDetails(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Button.Next);
            }
            catch(NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("More Third Party details");
            Reporting.Log("More Third Party details", _driver.TakeSnapshot());
            return true;
        }

        public void DetailedUiChecking(ClaimCar claim)
        {
            if (claim.DamageType == MotorClaimDamageType.SingleVehicleCollision)
            {
                Reporting.AreEqual(Constants.SingleVehicleHeaderText, Header, "Page header");
                Reporting.AreEqual(Constants.SingleVehicleActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            }
            else
            {
                Reporting.AreEqual(Constants.MultiVehicleHeaderText, Header, "Page header");
                Reporting.AreEqual(Constants.MultiVehicleActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            }

        }

        public void EnterMoreTPDetails(ClaimCar claim)
        {
            var thirdPartyDetails = claim.ThirdParty.FirstOrDefault();

            if (claim.DamageType == MotorClaimDamageType.MultipleVehicleCollision)
            {
                WasDriverTheOwner = thirdPartyDetails.WasDriverTheOwner;

                if (!thirdPartyDetails.WasDriverTheOwner && !string.IsNullOrEmpty(thirdPartyDetails.AdditionalInfo))
                {
                    WaitForTextFieldAndEnterText(XPath.Field.OwnerNameAndContactDetails, thirdPartyDetails.AdditionalInfo);
                }
            }

            if(!string.IsNullOrEmpty(thirdPartyDetails.Insurer.Name))
            {
                WaitForSelectableFieldToSearchAndPickFromDropdown(XPath.Dropdown.InsuranceCompany, XPath.Dropdown.Option, thirdPartyDetails.Insurer.Name);
            }

            if (!string.IsNullOrEmpty(thirdPartyDetails.ClaimNumber))
            {
                WaitForTextFieldAndEnterText(XPath.Field.ThirdPartyClaimNumber, thirdPartyDetails.ClaimNumber, false);
            }

            if (!string.IsNullOrEmpty(thirdPartyDetails.DescriptionOfDamageToVehicle))
            {
                WaitForTextFieldAndEnterText(XPath.Field.DamageDescription, thirdPartyDetails.DescriptionOfDamageToVehicle, false);
            } 

            Reporting.Log("More about the accident - Before clicking Next Button", _driver.TakeSnapshot());
            ClickNext();
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }
    }
}
