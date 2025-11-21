using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;

namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class ThirdPartyDetails : BaseMotorClaimPage
    {
        #region CONSTANTS
        private static class Constants
        {
            public static readonly string SingleVehicleActiveStepperLabel = "Property or pet owner";
            public static readonly string MultiVehicleActiveStepperLabel = "Other vehicle driver";
            public static readonly string SingleVehicleHeaderText = "Owner of the property or pet you collided with";
            public static readonly string MultiVehicleHeaderText = "Other driver in the accident";
            public static string ClaimNumberText(string claimNumber) => $"Your claim number is {claimNumber}";

            public static class Message
            {
                public static readonly string CantClaimOnline = "Sorry, you can't claim onlinePlease call us on 13 17 03 so we can help you with your claim.To speed up the process, please have their details ready before you call.";
            }
        }

        #endregion

        #region XPATHS

        private static class XPath
        {
            public static readonly string Header = "id('third-party-details-header')";
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";
            public static class Toggle
            {
                public static readonly string DoYouHaveTheTPDetails = "id('has-owner-details')";
                public static readonly string DoYouKnowTheTPBeforeAccident = "id('is-third-party-known-to-policy-holder')";
            }
            public static class Field
            {
                public static readonly string CarRego = "id('third-party-car-registration')";
                public static readonly string FirstName = "id('third-party-first-name')";
                public static readonly string LastName = "id('third-party-last-name')";
                public static readonly string ContactNumber = "id('contactNumber')";
                public static readonly string Email = "id('third-party-email')";
                public static readonly string Address = "id('address')";
                public static readonly string FirstAddress = "id('address-listbox')/li[@id = 'address-option-0']";
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
        private string ClaimNumber => GetInnerText(XPath.ClaimNumber);

        private bool DoYouHaveTheTPDetails
        {
            get => GetBinaryToggleState(XPath.Toggle.DoYouHaveTheTPDetails, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.Toggle.DoYouHaveTheTPDetails, XPath.Button.Yes, XPath.Button.No, value);
        }

        private string FirstName 
        {
            get => GetValue(XPath.Field.FirstName);
            set => WaitForTextFieldAndEnterText(XPath.Field.FirstName, value);
        }
        private string LastName
        {
            get => GetValue(XPath.Field.LastName);
            set => WaitForTextFieldAndEnterText(XPath.Field.LastName, value);
        }
        private string ContactNumber
        {
            get => GetValue(XPath.Field.ContactNumber);
            set => WaitForTextFieldAndEnterText(XPath.Field.ContactNumber, value);
        }
        private string Email
        {
            get => GetValue(XPath.Field.Email);
            set => WaitForTextFieldAndEnterText(XPath.Field.Email, value);
        }

        private string Address
        {
            get => GetValue(XPath.Field.Address);
            set => QASSearchForAddress(XPath.Field.Address, XPath.Field.FirstAddress, value);
        }

        private bool DoYouKnowTheTPBeforeAccident
        {
            get => GetBinaryToggleState(XPath.Toggle.DoYouKnowTheTPBeforeAccident, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.Toggle.DoYouKnowTheTPBeforeAccident, XPath.Button.Yes, XPath.Button.No, value);
        }

        #endregion


        public ThirdPartyDetails(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Button.Next);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Owner of the property or pet you collided with");
            Reporting.Log("Owner of the property or pet you collided with", _driver.TakeSnapshot());
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

        public void EnterTPAndPoliceDetails(ClaimCar claim)
        {
            Reporting.AreEqual(Constants.ClaimNumberText(claim.ClaimNumber), ClaimNumber, "Claim number");

            DoYouHaveTheTPDetails = claim.ThirdParty != null;
            
            if (claim.ThirdParty == null && claim.OnlyClaimDamageToTP)
            {
                ClickNext();
                return;
            }

            if (DoYouHaveTheTPDetails)
            {
                var thirdParty = claim.ThirdParty.First();
                if (claim.DamageType == MotorClaimDamageType.MultipleVehicleCollision &&
                    !string.IsNullOrEmpty(thirdParty.Rego))
                {
                    WaitForTextFieldAndEnterText(XPath.Field.CarRego, thirdParty.Rego);
                }

                if (!string.IsNullOrEmpty(thirdParty.FirstName))
                {
                    FirstName = thirdParty.FirstName;
                }

                if (!string.IsNullOrEmpty(thirdParty.Surname))
                {
                    LastName = thirdParty.Surname;
                }

                if (!string.IsNullOrEmpty(thirdParty.MobilePhoneNumber) || !string.IsNullOrEmpty(thirdParty.HomePhoneNumber))
                {
                    var contactNumber = string.IsNullOrEmpty(thirdParty.MobilePhoneNumber) ? thirdParty.HomePhoneNumber : thirdParty.MobilePhoneNumber;
                    ContactNumber = contactNumber;
                }

                if (!string.IsNullOrEmpty(thirdParty.MobilePhoneNumber))
                {
                    ContactNumber = thirdParty.MobilePhoneNumber;
                }

                if (!string.IsNullOrEmpty(thirdParty.GetEmail()))
                {
                    Email = thirdParty.GetEmail();
                }

                if (thirdParty.MailingAddress != null)
                {
                    Address = thirdParty.MailingAddress.StreetSuburbState();
                }

                DoYouKnowTheTPBeforeAccident = thirdParty.IsKnownToClaimant;
            }

            Reporting.Log("Property or pet owner - Before clicking Next Button", _driver.TakeSnapshot());
            ClickNext();           
        }

        /// <summary>
        /// Verify the Can't claim online blocking message
        /// </summary>
        public void VerifyCantClaimOnlineErrorMessage()
        {
            Reporting.Log("Verify can't claim online error card", _driver.TakeSnapshot());
            Reporting.AreEqual(Constants.Message.CantClaimOnline, GetInnerText(XPathBaseMotorClaim.NotificationCard.Body).StripLineFeedAndCarriageReturns(false), "expected can't claim online card content is displayed");
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }

    }
}