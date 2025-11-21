using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;

namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class WheresYourCar : SparkBasePage
    {
        #region CONSTANTS
        private class Constants
        {
            public static readonly string ActiveStepperLabel = "Where's your car";
            public static readonly string HeaderText = "Where's your car";
            public static string ClaimNumberText(string claimNumber) => $"Your claim number is {claimNumber}";
            public class Label
            {
                public static readonly string YourCarAt = "Your car is at...";
                
                public static readonly string TellUsWhereYourCar = "Please tell us where your car is";
                public static readonly string ProvideDetails = "Please provide any details that may help us locate your car (if known)Any information is helpful.";
                public static readonly string YourCarLocation = "Your car's location (if known)";

                public class RadioButton
                {
                    public static readonly string TowTruck = "A tow truck holding yard";
                    public static readonly string Repairers = "A repairer";
                    public static readonly string YourHome = "Your home";
                    public static readonly string Other = "Other";
                    public static readonly string DontKnow = "I don't know";
                }

                public class BusinessDeatils
                {
                    public static readonly string HoldingYard = "Holding yard details";
                    public static readonly string Repairer = "Repairer details";
                    public static readonly string Name = "Business name (if known)";
                    public static readonly string ContactNumber = "Contact number (if known)";
                    public static readonly string Address = "Address or suburb (if known)Any information is helpful.";
                }
            }
        }

        #endregion

        #region XPATHS

        private class XPath
        {
            public static readonly string Header = "id('wheres-your-car-header')";
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";
            public static readonly string SubHeader = "id('car-location-label')";

            public class Label
            {
                public class TowDestination
                {
                    public static readonly string TowTruck = "id('car-location-a-tow-truck-holding-yard-label')";
                    public static readonly string Repairers = "id('car-location-a-repairer-label')";                   
                    public static readonly string YourHome = "id('car-location-your-home-label')";
                    public static readonly string Other = "id('car-location-other-label')";
                    public static readonly string DontKnow = "id('car-location-i-dont-know-label')";
                }

                public class BusinessDeatils
                {
                    public static readonly string Heading = "id('business-details-heading')";
                    public static readonly string Name = "//label[@for='business-name']";
                    public static readonly string ContactNumber = "//label[@for='contactNumber']";
                    public static readonly string Location = "//label[@for='car-location-details']";
                }
            }

            public class Field
            {
                public static readonly string Location = "id('car-location-details')";
                public static readonly string BusinessName = "id('business-name')";
                public static readonly string ContacdtNumber = "id('contactNumber')";
                public static readonly string HomeAddress = "id('car-location-your-home-sublabel')";
            }

            public class Button
            {
                public static readonly string Next = "id('submit')";
            }
            public class RadioButton
            {
                public static readonly string TowTruck = "//input[@value='A tow truck holding yard']";
                public static readonly string Repairers = "//input[@value='A repairer']";
                public static readonly string YourHome = "//input[@value='Your home']";
                public static readonly string Other = "//input[@value='Other']";
                public static readonly string DontKnow = "//input[@value=\"I don't know\"]";
            }
           
        }

        #endregion

        #region Settable properties and controls

        private string Header => GetInnerText(XPath.Header);
        private string ClaimNumber => GetInnerText(XPath.ClaimNumber);
        
        private string BusinessName
        {
            get => GetInnerText(XPath.Field.BusinessName);
            set => WaitForTextFieldAndEnterText(XPath.Field.BusinessName, value, false);
        }
        private string ContactNumber
        {
            get => GetInnerText(XPath.Field.ContacdtNumber);
            set => WaitForTextFieldAndEnterText(XPath.Field.ContacdtNumber, value, false);
        }
        private string Address
        {
            get => GetInnerText(XPath.Field.Location);
            set => WaitForTextFieldAndEnterText(XPath.Field.Location, value, false);
        }
        #endregion


        public WheresYourCar(Browser browser) : base(browser)
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

            Reporting.LogPageChange("Where's your car");
            Reporting.Log("Where's your car after loading confirmed", _driver.TakeSnapshot());
            return true;
        }

        public void DetailedUiChecking(ClaimCar claim)
        {
            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");

            if (claim.TowedVehicleDetails.WasVehicleTowed == true)
            {
                Reporting.AreEqual(Constants.Label.YourCarAt, GetInnerText(XPath.SubHeader), "Your car is at question label");
                Reporting.AreEqual(Constants.Label.RadioButton.TowTruck, GetInnerText(XPath.Label.TowDestination.TowTruck), "A tow truck holding yard radio button label");
                Reporting.AreEqual(Constants.Label.RadioButton.Repairers, GetInnerText(XPath.Label.TowDestination.Repairers), "A repairer's radio button label");
                //Your Home radio button only be displayed when risk address is full and not only suburb name
                if (claim.Policy.RiskAddress.StreetOrPOBox != null)
                {
                    Reporting.AreEqual(Constants.Label.RadioButton.YourHome, GetInnerText(XPath.Label.TowDestination.YourHome), "Your home radio button label");
                }                
                Reporting.AreEqual(Constants.Label.RadioButton.Other, GetInnerText(XPath.Label.TowDestination.Other), "Other radio button label");
                Reporting.AreEqual(Constants.Label.RadioButton.DontKnow, GetInnerText(XPath.Label.TowDestination.DontKnow), "I don't know radio button label");

                GetElement(XPath.RadioButton.TowTruck).Click();
                Reporting.AreEqual(Constants.Label.BusinessDeatils.HoldingYard, GetInnerText(XPath.Label.BusinessDeatils.Heading), "Holding yard details header");
                VerifyBusinessDetailLabels();

                GetElement(XPath.RadioButton.Repairers).Click();
                Reporting.AreEqual(Constants.Label.BusinessDeatils.Repairer, GetInnerText(XPath.Label.BusinessDeatils.Heading), "Repairer details header");
                VerifyBusinessDetailLabels();

                GetElement(XPath.RadioButton.Other).Click();
                Reporting.AreEqual(Constants.Label.TellUsWhereYourCar, GetInnerText(XPath.Label.BusinessDeatils.Location), "Please tell us where your car is label");

                GetElement(XPath.RadioButton.DontKnow).Click();
                Reporting.AreEqual(Constants.Label.ProvideDetails, GetInnerText(XPath.Label.BusinessDeatils.Location).StripLineFeedAndCarriageReturns(false), "Please provide any details that may help us locate your car label");

            }
            else
            {
                Reporting.AreEqual(Constants.Label.YourCarLocation, GetInnerText(XPath.Label.BusinessDeatils.Location).StripLineFeedAndCarriageReturns(false), "Your cars location label");
            }
        }

        private void VerifyBusinessDetailLabels()
        {
            Reporting.AreEqual(Constants.Label.BusinessDeatils.Name, GetInnerText(XPath.Label.BusinessDeatils.Name), "Business name label");
            Reporting.AreEqual(Constants.Label.BusinessDeatils.ContactNumber, GetInnerText(XPath.Label.BusinessDeatils.ContactNumber), "Contact number label");
            Reporting.AreEqual(Constants.Label.BusinessDeatils.Address, GetInnerText(XPath.Label.BusinessDeatils.Location).StripLineFeedAndCarriageReturns(false), "Address label");
        }

        public void EnterWhereYourCarDetails(ClaimCar claimCar)
        {
            Reporting.AreEqual(Constants.HeaderText, Header, "Page header");
            Reporting.AreEqual(Constants.ClaimNumberText(claimCar.ClaimNumber), ClaimNumber, "Claim number");
            

            if (claimCar.TowedVehicleDetails.WasVehicleTowed == true)
            {
                if (claimCar.Policy.RiskAddress.StreetOrPOBox != null)
                {
                    if (claimCar.Policy.RiskAddress.State != null)
                    {
                        Reporting.AreEqual(claimCar.Policy.RiskAddress.StreetSuburbStatePostcode().Trim(), GetInnerText(XPath.Field.HomeAddress), ignoreCase: true, "Your home address");
                    }
                    else
                    {
                        Reporting.AreEqual(claimCar.Policy.RiskAddress.StreetSuburbPostcode(), GetInnerText(XPath.Field.HomeAddress), ignoreCase: true, "Your home address");
                    }                    
                }

                switch (claimCar.TowedVehicleDetails.TowedTo)
                {
                    case MotorClaimTowedTo.HoldingYard:
                        GetElement(XPath.RadioButton.TowTruck).Click();
                        EnterBusinessDetails(claimCar.TowedVehicleDetails.BusinessDetails);
                        break;
                    case MotorClaimTowedTo.Repairer:
                        GetElement(XPath.RadioButton.Repairers).Click();
                        EnterBusinessDetails(claimCar.TowedVehicleDetails.BusinessDetails);
                        break;
                    case MotorClaimTowedTo.HomeAddress:
                        GetElement(XPath.RadioButton.YourHome).Click();
                        break;
                    case MotorClaimTowedTo.Other:
                        GetElement(XPath.RadioButton.Other).Click();
                        WaitForTextFieldAndEnterText(XPath.Field.Location, claimCar.TowedVehicleDetails.CarLocation);
                        break;
                    case MotorClaimTowedTo.Unknown:
                        GetElement(XPath.RadioButton.DontKnow).Click();
                        WaitForTextFieldAndEnterText(XPath.Field.Location, claimCar.TowedVehicleDetails.CarLocation);
                        break;
                    default:
                        throw new NotSupportedException($"{claimCar.TowedVehicleDetails.TowedTo.GetDescription()} is not supported");
                }
            }
            else
            {
                WaitForTextFieldAndEnterText(XPath.Field.Location, claimCar.TowedVehicleDetails.CarLocation);
            }

            Reporting.Log("About your car - Before clicking Next Button", _driver.TakeSnapshot());
            ClickNext();
        }
        private void EnterBusinessDetails(BusinessDetails businessDetails)
        {
            BusinessName = businessDetails.BusinessName;
            ContactNumber = businessDetails.ContactNumber;
            Address = businessDetails.Address;
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }

        public List<string> GetPercyIgnoreCSS() =>
         new List<string>
         {
               "#claimNumberDisplay span"
         };

    }
}
