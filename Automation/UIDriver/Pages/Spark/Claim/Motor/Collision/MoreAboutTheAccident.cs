
using Rac.TestAutomation.Common;
using System;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;

namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class MoreAboutTheAccident : BaseMotorClaimPage
    {
        #region CONSTANTS
        private class Constants
        {
            public static readonly string ActiveStepperLabel = "More about the accident";
            public static readonly string HeaderText = "More about the accident";
            public static readonly string SubHeaderText = "Your car was...";

            public class Label
            {
                public static readonly string DrivingForward = "Driving forward";
                public static readonly string Reversing = "Reversing";
                public static readonly string Parked = "ParkedCar engine is off and handbrake is on";
                public static readonly string Stationary = "StationaryCar is not moving, e.g. stopped in traffic";
            }
        }

        #endregion

        #region XPATHS

        private class XPath
        {
            public static readonly string Header = "id('header')";
            public static readonly string SubHeader = "id('direction-of-travel-label')";

            public class Field
            {
                public static readonly string DrivingForwardText = "id('direction-of-travel-0-radio-container')";
                public static readonly string ReversingText = "id('direction-of-travel-1-radio-container')";
                public static readonly string ParkedText = "id('direction-of-travel-2-radio-container')";
                public static readonly string StationaryText = "id('direction-of-travel-3-radio-container')";
            }

            public class RadioButton
            {
                public static readonly string DrivingForward = "//input[@value='Forward']";
                public static readonly string Reversing = "//input[@value='Reversing']";
                public static readonly string Parked = "//input[@value='Parked']";
                public static readonly string Stationary = "//input[@value='Stationary']";

                public static readonly string DrivingForwardAnotherVehicleHitYourCar = "id('forward-scenario')//div[.='A vehicle hit your car']//input";
                public static readonly string DrivingForwardYourCarHitAnotherVehicle = "id('forward-scenario')//div[.='Your car hit another vehicle']//input";
                public static readonly string DrivingForwardOurCarsHitOneAnother = "id('forward-scenario')//div[.='Our vehicles hit one another']//input";
                public static readonly string WhileDrivingSomethingElseHappened = "id('forward-scenario')//div[.='Something else happened']//input";
                public static readonly string ReversingHitAParkedCar = "id('multi-vehicle-collision-scenario')//div[.='Hit a parked vehicle']//input";
                public static readonly string ReversingHitAnotherVehicle = "id('multi-vehicle-collision-scenario')//div[.='Hit another vehicle']//input";
                public static readonly string ReversingGetHitByAnotherCar = "id('multi-vehicle-collision-scenario')//div[.='Get hit in the rear by another vehicle']//input";
                public static readonly string ReversingIntoAnotherReversingVehicle = "id('multi-vehicle-collision-scenario')//div[.='We reversed into each other']//input";
                public static readonly string ParkedAnotherCarHitMyCar = "id('multi-vehicle-collision-scenario')//div[.='Yes']//input";
                public static readonly string StationaryAnotherCarHitRearOfMyCar = "id('multi-vehicle-collision-scenario')//div[.='Another driver hit the rear of my car']//input";
                public static readonly string StationaryAnotherCarReverseIntoMyCar = "id('multi-vehicle-collision-scenario')//div[.='Another driver reversed into my car']//input";
                public static readonly string WhileNotDrivingElseHappened = "id('multi-vehicle-collision-scenario')//div[.='Something else happened']//input";

                public static readonly string AnotherVehicleHitTheRearOfYourCar = "id('multi-vehicle-collision-scenario')//div[.='Hit the rear of your car']//input";
                public static readonly string AnotherVehicleHitYourCarWhenChangingLanes = "id('multi-vehicle-collision-scenario')//div[.='Hit your car when changing lanes']//input";
                public static readonly string AnotherVehicleHitYourCarWhenFailedToGiveWay = "id('multi-vehicle-collision-scenario')//div[.='Fail to give way']//input";
                public static readonly string AnotherVehicleHitYourCarSomethingElseHappened = "id('multi-vehicle-collision-scenario')//div[.='Something else happened']//input";

                public static readonly string YourCarHitAnotherVehicleWhenChangingLanes = "id('multi-vehicle-collision-scenario')//div[.='Hit another vehicle when changing lanes']//input";
                public static readonly string YourCarHitRearOfAnotherVehicle = "id('multi-vehicle-collision-scenario')//div[.='Hit the rear of another vehicle']//input";
                public static readonly string YourCarHitAnotherParkedVehicle = "id('multi-vehicle-collision-scenario')//div[.='Hit a parked vehicle']//input";
                public static readonly string YourCarHitAnotherVehicleWhenFailedToGiveWay = "id('multi-vehicle-collision-scenario')//div[.='Fail to give way']//input";
                public static readonly string YourCarHitAnotherVehicleSomethingElseHappened = "id('multi-vehicle-collision-scenario')//div[.='Something else happened']//input";

                public static readonly string OurCarHitAnotherVehicleWhenBothChangingLanes = "id('multi-vehicle-collision-scenario')//div[.='Both changed lanes and hit each other']//input";
                public static readonly string OurCarHitAnotherVehicleWhenBothFailedToGiveWay = "id('multi-vehicle-collision-scenario')//div[.='Both failed to give way']//input";
                public static readonly string OurCarHitAnotherVehicleSomethingElseHappened = "id('multi-vehicle-collision-scenario')//div[.='Something else happened']//input";
            }

            public class Button
            {
                public static readonly string Next = "id('submit')";
            }
        }

        #endregion

        #region Settable properties and controls

        private string Header => GetInnerText(XPath.Header);
        private string SubHeader => GetInnerText(XPath.SubHeader);
        private string DrivingForwardLabel => GetInnerText(XPath.Field.DrivingForwardText);
        private string ReversingLabel => GetInnerText(XPath.Field.ReversingText);
        private string ParkedLabel => GetInnerText(XPath.Field.ParkedText).StripLineFeedAndCarriageReturns(false);
        private string StationaryLabel => GetInnerText(XPath.Field.StationaryText).StripLineFeedAndCarriageReturns(false);

        #endregion


        public MoreAboutTheAccident(Browser browser) : base(browser)
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

            Reporting.LogPageChange("More about the accident");
            Reporting.Log("More about the accident", _driver.TakeSnapshot());
            return true;
        }

        public void DetailedUiChecking()
        {
            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            Reporting.AreEqual(Constants.HeaderText, Header, "Page header text");
            Reporting.AreEqual(Constants.SubHeaderText, SubHeader, "Page sub header text");
            Reporting.AreEqual(Constants.Label.DrivingForward, DrivingForwardLabel, "Driving forward radio button text");
            Reporting.AreEqual(Constants.Label.Reversing, ReversingLabel, "Reversing radio button text");
            Reporting.AreEqual(Constants.Label.Parked, ParkedLabel, "Parked radio button text");
            Reporting.AreEqual(Constants.Label.Stationary, StationaryLabel, "Stationary radio button text");
            VerifyMotorClaimOnlineNotificationCard();

        }
        /// <summary>
        /// First It select the travel direction of the car,
        /// and if it's a multi vehicle collision then
        /// it select the claim scenario
        /// <summary>
        public void SelectHowTheAccidentHappened(ClaimCar claim)
        {
            SelectYourCarTravelDirection(claim.DirectionBeingTravelled);

            if (claim.DamageType == MotorClaimDamageType.MultipleVehicleCollision)
            {
                switch (claim.ClaimScenario)
                {
                    case MotorClaimScenario.WhileDrivingOtherVehicleHitRearOfMyCar:
                        GetElement(XPath.RadioButton.DrivingForwardAnotherVehicleHitYourCar).Click();
                        GetElement(XPath.RadioButton.AnotherVehicleHitTheRearOfYourCar).Click();
                        break;

                    case MotorClaimScenario.WhileDrivingOtherVehicleHitMyCarWhenChangingLanes:
                        GetElement(XPath.RadioButton.DrivingForwardAnotherVehicleHitYourCar).Click();
                        GetElement(XPath.RadioButton.AnotherVehicleHitYourCarWhenChangingLanes).Click();
                        break;

                    case MotorClaimScenario.WhileDrivingOtherVehicleFailToGiveWayAndHitMyCar:
                        GetElement(XPath.RadioButton.DrivingForwardAnotherVehicleHitYourCar).Click();
                        GetElement(XPath.RadioButton.AnotherVehicleHitYourCarWhenFailedToGiveWay).Click();
                        break;

                    case MotorClaimScenario.WhileDrivingOtherVehicleHitMyCarSomethingElseHappened:
                        GetElement(XPath.RadioButton.DrivingForwardAnotherVehicleHitYourCar).Click();
                        GetElement(XPath.RadioButton.AnotherVehicleHitYourCarSomethingElseHappened).Click();
                        break;

                    case MotorClaimScenario.WhileDrivingMyCarHitAnotherCarWhenChangingLanes:
                        GetElement(XPath.RadioButton.DrivingForwardYourCarHitAnotherVehicle).Click();
                        GetElement(XPath.RadioButton.YourCarHitAnotherVehicleWhenChangingLanes).Click();
                        break;

                    case MotorClaimScenario.WhileDrivingMyCarHitRearOfAnotherCar:
                        GetElement(XPath.RadioButton.DrivingForwardYourCarHitAnotherVehicle).Click();
                        GetElement(XPath.RadioButton.YourCarHitRearOfAnotherVehicle).Click();
                        break;

                    case MotorClaimScenario.WhileDrivingMyCarHitAParkedCar:
                        GetElement(XPath.RadioButton.DrivingForwardYourCarHitAnotherVehicle).Click();
                        GetElement(XPath.RadioButton.YourCarHitAnotherParkedVehicle).Click();
                        break;

                    case MotorClaimScenario.WhileDrivingMyCarHitAnotherCarFailToGiveWay:
                        GetElement(XPath.RadioButton.DrivingForwardYourCarHitAnotherVehicle).Click();
                        GetElement(XPath.RadioButton.YourCarHitAnotherVehicleWhenFailedToGiveWay).Click();
                        break;

                    case MotorClaimScenario.WhileDrivingMyCarHitAnotherCarSomethingElseHappened:
                        GetElement(XPath.RadioButton.DrivingForwardYourCarHitAnotherVehicle).Click();
                        GetElement(XPath.RadioButton.YourCarHitAnotherVehicleSomethingElseHappened).Click();
                        break;

                    case MotorClaimScenario.WhileDrivingOurCarHitOneAnotherCarWhenChangingLanes:
                        GetElement(XPath.RadioButton.DrivingForwardOurCarsHitOneAnother).Click();
                        GetElement(XPath.RadioButton.OurCarHitAnotherVehicleWhenBothChangingLanes).Click();
                        break;

                    case MotorClaimScenario.WhileDrivingOurCarHitOneAnotherCarBothFailedToGiveWay:
                        GetElement(XPath.RadioButton.DrivingForwardOurCarsHitOneAnother).Click();
                        GetElement(XPath.RadioButton.OurCarHitAnotherVehicleWhenBothFailedToGiveWay).Click();
                        break;

                    case MotorClaimScenario.WhileDrivingOurCarHitOneAnotherCarSomethingElseHappened:
                        GetElement(XPath.RadioButton.DrivingForwardOurCarsHitOneAnother).Click();
                        GetElement(XPath.RadioButton.OurCarHitAnotherVehicleSomethingElseHappened).Click();
                        break;

                    case MotorClaimScenario.WhileDrivingSomethingElseHappened:                    
                        GetElement(XPath.RadioButton.WhileDrivingSomethingElseHappened).Click();
                        break;

                    case MotorClaimScenario.WhileParkedSomethingElseHappened:
                    case MotorClaimScenario.WhileStationarySomethingElseHappened:
                    case MotorClaimScenario.WhileReversingSomethingElseHappened:
                        GetElement(XPath.RadioButton.WhileNotDrivingElseHappened).Click();
                        break;

                    case MotorClaimScenario.WhileReversingHitParkedCar:
                        GetElement(XPath.RadioButton.ReversingHitAParkedCar).Click();
                        break;

                    case MotorClaimScenario.WhileReversingHitAnotherCar:
                        GetElement(XPath.RadioButton.ReversingHitAnotherVehicle).Click();
                        break;

                    case MotorClaimScenario.WhileReversingHitByAnotherCar:
                        GetElement(XPath.RadioButton.ReversingGetHitByAnotherCar).Click();
                        break;

                    case MotorClaimScenario.WhileReversingHitByAnotherReversingCar:
                        GetElement(XPath.RadioButton.ReversingIntoAnotherReversingVehicle).Click();
                        break;

                    case MotorClaimScenario.WhileParkedAnotherCarHitMyCar:
                        GetElement(XPath.RadioButton.ParkedAnotherCarHitMyCar).Click();
                        break;

                    case MotorClaimScenario.WhileStationaryAnotherCarHitRearOfMyCar:
                        GetElement(XPath.RadioButton.StationaryAnotherCarHitRearOfMyCar).Click();
                        break;

                    case MotorClaimScenario.WhileStationaryAnotherCarReversedIntoMyCar:
                        GetElement(XPath.RadioButton.StationaryAnotherCarReverseIntoMyCar).Click();
                        break;
                    default:
                        break;
                }
            }

            Reporting.Log("More about the accident - Before clicking Next Button", _driver.TakeSnapshot());
            ClickNext();
        }


        /// <summary>
        /// Select the direction of the car travel
        /// </summary>
        private void SelectYourCarTravelDirection(TravelDirection travelDirection)
        {
            switch (travelDirection)
            {
                case TravelDirection.Forward:
                    GetElement(XPath.RadioButton.DrivingForward).Click();
                    break;
                case TravelDirection.Reversing:
                    GetElement(XPath.RadioButton.Reversing).Click();
                    break;
                case TravelDirection.Stationary:
                    GetElement(XPath.RadioButton.Stationary).Click();
                    break;
                case TravelDirection.Parked:
                    GetElement(XPath.RadioButton.Parked).Click();
                    break;
                default:
                    throw new NotSupportedException($"{travelDirection.GetDescription()} is not supported");
            }
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }

    }
}