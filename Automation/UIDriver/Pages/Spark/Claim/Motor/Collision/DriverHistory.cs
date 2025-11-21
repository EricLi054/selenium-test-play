using Rac.TestAutomation.Common;
using System.Collections.Generic;

namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class DriverHistory : BaseMotorClaimPage
    {
        #region CONSTANTS
        private class Constants
        {
            public static string HeaderText(string driverName) => $"{driverName} driver history";
            public static string ClaimNumberText(string claimNumber) => $"Your claim number is {claimNumber}";
            public static readonly string ActiveStepperLabel = "Driver history";

            public class Label
            {
                public class ClaimantDriver
                {
                    public static readonly string DriverUnderTheInfluence = $"Were you under the influence of alcohol or drugs at the time of the incident?";
                    public static readonly string LicenceMoreThan2Years = $"Have you held a driver's licence for more than 2 years?";
                    public static readonly string LicenceSuspendedInLast3Years = $"Has your driver's licence been suspended or cancelled in the last 3 years?";
                }
                public class SomeoneElseDriver
                {
                    public static string DriverUnderTheInfluence(string driverName) => $"Was {driverName} under the influence of alcohol or drugs at the time of the incident?";
                    public static string LicenceMoreThan2Years(string driverName) => $"Has {driverName} held a driver's licence for more than 2 years?";
                    public static string LicenceSuspendedInLast3Years(string driverName) => $"Has {driverName}'s driver's licence been suspended or cancelled in the last 3 years?";
                }

            }
        }

        #endregion

        #region XPATHS

        private class XPath
        {
            public static readonly string Header = "id('your-driver-history-header')";
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";

            public class Button
            {
                public static readonly string Yes = "//button[@aria-label='Yes']";
                public static readonly string No = "//button[@aria-label='No']";
                public static readonly string Unknown = "//button[@aria-label='Unknown']";
                public static readonly string Next = "id('submit-button')";
            }

            public class Toggle
            {
                public static readonly string DriverUnderTheInfluence = "id('driver-was-under-the-influence')";
                public static readonly string LicenceMoreThan2Years = "id('driver-has-valid-licence')";
                public static readonly string LicenceSuspendedInLast3Years = "id('driver-has-licence-suspension-or-cancellation')";
            }
            public class Label
            {
                public static readonly string DriverUnderTheInfluence = "//label[@for='driver-was-under-the-influence']";
                public static readonly string LicenceMoreThan2Years = "//label[@for='driver-has-valid-licence']";
                public static readonly string LicenceSuspendedInLast3Years = "//label[@for='driver-has-licence-suspension-or-cancellation']";
            }
        }

        #endregion

        #region Settable properties and controls

        private string Header => GetInnerText(XPath.Header);
        private string ClaimNumber => GetInnerText(XPath.ClaimNumber);
        private void DriverUnderTheInfluence (bool isClaimantDriver, bool value)
        {
            if(isClaimantDriver)
            {
                ClickBinaryToggle(XPath.Toggle.DriverUnderTheInfluence, XPath.Button.Yes, XPath.Button.No,value);
            }
            else
            { 
                ClickTriStateToggleWithNullableInput(XPath.Toggle.DriverUnderTheInfluence, XPath.Button.Yes, XPath.Button.No, XPath.Button.Unknown, value);
            }
        }
        private void LicenceValid (bool isClaimantDriver, bool value)
        {
            if (isClaimantDriver)
            {
                ClickBinaryToggle(XPath.Toggle.LicenceMoreThan2Years, XPath.Button.Yes, XPath.Button.No, value);
            }
            else
            {
                ClickTriStateToggleWithNullableInput(XPath.Toggle.LicenceMoreThan2Years, XPath.Button.Yes, XPath.Button.No, XPath.Button.Unknown, value);
            }
        }
        private void LicenceSuspended (bool isClaimantDriver, bool value)
        {
            if (isClaimantDriver)
            {
                ClickBinaryToggle(XPath.Toggle.LicenceSuspendedInLast3Years, XPath.Button.Yes, XPath.Button.No, value);
            }
            else
            {
                ClickTriStateToggleWithNullableInput(XPath.Toggle.LicenceSuspendedInLast3Years, XPath.Button.Yes, XPath.Button.No, XPath.Button.Unknown, value);
            }
        }

        #endregion


        public DriverHistory(Browser browser) : base(browser)
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

            Reporting.LogPageChange("Your driver history");
            Reporting.Log("Your driver history immediately after loading confirmed", _driver.TakeSnapshot());
            return true;
        }

        public void DetailedUiChecking(string driverName, bool isClaimantDriver)
        {
            var driverUnderInfluenceLabel = isClaimantDriver ? Constants.Label.ClaimantDriver.DriverUnderTheInfluence : Constants.Label.SomeoneElseDriver.DriverUnderTheInfluence(driverName);
            var licenceValidLabel = isClaimantDriver ? Constants.Label.ClaimantDriver.LicenceMoreThan2Years : Constants.Label.SomeoneElseDriver.LicenceMoreThan2Years(driverName);
            var licenceSuspendedLabel = isClaimantDriver ? Constants.Label.ClaimantDriver.LicenceSuspendedInLast3Years : Constants.Label.SomeoneElseDriver.LicenceSuspendedInLast3Years(driverName);

            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            Reporting.AreEqual(driverUnderInfluenceLabel, GetInnerText(XPath.Label.DriverUnderTheInfluence), "Driver under the influence question label");
            Reporting.IsFalse(IsControlDisplayed(XPath.Label.LicenceMoreThan2Years),"'Driver license more than 2 years' field should not be displayed");
            Reporting.AreEqual(licenceSuspendedLabel, GetInnerText(XPath.Label.LicenceSuspendedInLast3Years), "Licence suspended in last 3 years question label");
        }

        public void EnterDriverHistory(ClaimCar claimCar)
        {            
            string driverName = claimCar.IsClaimantDriver ? "Your" : $"{claimCar.Driver.DriverDetails.FirstName}'s";

            Reporting.AreEqual(Constants.HeaderText(driverName), Header, "Page header");
            Reporting.AreEqual(Constants.ClaimNumberText(claimCar.ClaimNumber), ClaimNumber, "Claim number");

            DriverUnderTheInfluence(claimCar.IsClaimantDriver, claimCar.Driver.WasDriverDrunk);
            LicenceSuspended(claimCar.IsClaimantDriver, claimCar.Driver.WasDriverLicenceSuspended);

            Reporting.Log("Your driver history - Before clicking Next Button", _driver.TakeSnapshot());
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
