using Rac.TestAutomation.Common;
using System;
using static Rac.TestAutomation.Common.Constants;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;

namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class AboutTheAccident : BaseMotorClaimPage
    {
        #region CONSTANTS
        private static class Constants
        {
            public static readonly string ActiveStepperLabel = "About the accident";
            public static readonly string Header = "About the accident";

            public static class Label
            {
                public static readonly string YourOwnProperty = "Your own propertye.g. garage door, fence, wall, pillar";
                public static readonly string SomeoneElseProperty = "Someone else's propertye.g. power pole, bollard, bus stop, wall, pillar";
                public static readonly string Wildlife = "Kangaroo or wildlife";
                public static readonly string SomeonesPet = "Another person's pet or animale.g. dog, cat, cow";
                public static readonly string SomethingElse = "Something else";
            }

            public class Message
            {
                public static readonly string CantClaimOnlineMessage = "Sorry, you can't claim onlinePlease call us on 13 17 03 so we can help you with your claim.";
            }
        }

        #endregion

        #region XPATHS

        private static class XPath
        {
            public static readonly string Header = "id('header')";
            public static readonly string NumberOfVehiclesInvolved = "id('label-other-vehicles-involved')";

            public static class VehiclesInvolved
            {
                public static readonly string NumberOfVehiclesInvolvedDropDown = "id('other-vehicles-involved')";
                public static readonly string Option = "//ul[@role='listbox']" + "//li";
            }

            public static class AccidentWith
            {
                public static readonly string YourOwnProperty = "id('single-vehicle-collision-scenario-cardgroup-0-HitYourOwnProperty')/div";
                public static readonly string SomeoneElseProperty = "id('single-vehicle-collision-scenario-cardgroup-1-HitSomeoneElsesProperty')/div";
                public static readonly string Wildlife = "id('single-vehicle-collision-scenario-cardgroup-2-HitKangarooOrWildlife')/div";
                public static readonly string SomeonesPet = "id('single-vehicle-collision-scenario-cardgroup-3-HitAnotherPersonsPetOrAnimal')/div";
                public static readonly string SomethingElse = "id('single-vehicle-collision-scenario-cardgroup-4-SomethingElseHappened')/div";
            }
            public static class Toggle
            {
                public static readonly string WantToClaimForDamageToYourCar = "id('claim-for-damage-to-own-car')";
                public static readonly string WasTheirAssetDamaged = "id('property-damaged')";
            }
            public static class Button
            {
                public static readonly string Yes = "//button[@aria-label='Yes']";
                public static readonly string No = "//button[@aria-label='No']";
                public static readonly string Unknown = "//button[@aria-label='Unknown']";
                public static readonly string Next = "id('submit')";
            }
        }

        #endregion

        #region Settable properties and controls

        private string Header => GetInnerText(XPath.Header);
        private string YourOwnProperty => GetInnerText(XPath.AccidentWith.YourOwnProperty).StripLineFeedAndCarriageReturns(false);
        private string SomeoneElseProperty => GetInnerText(XPath.AccidentWith.SomeoneElseProperty).StripLineFeedAndCarriageReturns(false);
        private string KangarooOrWildlife => GetInnerText(XPath.AccidentWith.Wildlife).StripLineFeedAndCarriageReturns(false);
        private string SomeonesPet => GetInnerText(XPath.AccidentWith.SomeonesPet).StripLineFeedAndCarriageReturns(false);
        private string SomethingElse => GetInnerText(XPath.AccidentWith.SomethingElse).StripLineFeedAndCarriageReturns(false);

        private bool WantToClaimForDamageToYourCar
        {
            get => GetBinaryToggleState(XPath.Toggle.WantToClaimForDamageToYourCar, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.Toggle.WantToClaimForDamageToYourCar, XPath.Button.Yes, XPath.Button.No, value);
        }
        private bool? WasTheirAssetDamaged
        {
            get => GetNullableBinaryForTriStateToggle(XPath.Toggle.WasTheirAssetDamaged, XPath.Button.Yes, XPath.Button.No, XPath.Button.Unknown);
            set => ClickTriStateToggleWithNullableInput(XPath.Toggle.WasTheirAssetDamaged, XPath.Button.Yes, XPath.Button.No, XPath.Button.Unknown, value);
        }

        #endregion


        public AboutTheAccident(Browser browser) : base(browser)
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

            Reporting.LogPageChange("About the accident page");
            Reporting.Log("About the accident page", _driver.TakeSnapshot());
            return true;
        }

        public void DetailedUiChecking(ClaimCar claim)
        {
            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            if (claim.DamageType == MotorClaimDamageType.SingleVehicleCollision)
            {
                WaitForSelectableAndPickFromDropdown(XPath.VehiclesInvolved.NumberOfVehiclesInvolvedDropDown, XPath.VehiclesInvolved.Option, MotorCollisionNumberOfVehiclesInvolved.NoOtherVehiclesInvolved.GetDescription());
                Reporting.AreEqual(Constants.Label.YourOwnProperty, YourOwnProperty, "Accident with your own property text");
                Reporting.AreEqual(Constants.Label.SomeoneElseProperty, SomeoneElseProperty, "Accident with someone's else property text");
                Reporting.AreEqual(Constants.Label.Wildlife, KangarooOrWildlife, "Accident with kangaroo or wildlife text");
                Reporting.AreEqual(Constants.Label.SomeonesPet, SomeonesPet, "Accident with another person's pet or animal text");
                Reporting.AreEqual(Constants.Label.SomethingElse, SomethingElse, "Accident with something else text");
            }
            VerifyMotorClaimOnlineNotificationCard();
        }

        /// <summary>
        /// This method sets the number of vehicles involved in the single/multiple motor collision scenarios.
        /// </summary>        
        private void SetNumberOfVehicleInvolved(ClaimCar claimCar)
        {
            switch (claimCar.NumberOfVehiclesInvolved)
            {
                case MotorCollisionNumberOfVehiclesInvolved.OneOtherVehicle:
                    WaitForSelectableAndPickFromDropdown(XPath.VehiclesInvolved.NumberOfVehiclesInvolvedDropDown, XPath.VehiclesInvolved.Option, MotorCollisionNumberOfVehiclesInvolved.OneOtherVehicle.GetDescription());
                    Reporting.AreEqual(MotorCollisionNumberOfVehiclesInvolved.OneOtherVehicle.GetDescription(), claimCar.NumberOfVehiclesInvolved.GetDescription(), "Number of vehicles involved");
                    break;
                case MotorCollisionNumberOfVehiclesInvolved.TwoOtherVehicles:
                    WaitForSelectableAndPickFromDropdown(XPath.VehiclesInvolved.NumberOfVehiclesInvolvedDropDown, XPath.VehiclesInvolved.Option, MotorCollisionNumberOfVehiclesInvolved.TwoOtherVehicles.GetDescription());
                    Reporting.AreEqual(MotorCollisionNumberOfVehiclesInvolved.TwoOtherVehicles.GetDescription(), claimCar.NumberOfVehiclesInvolved.GetDescription(), "Number of vehicles involved");
                    break;
                case MotorCollisionNumberOfVehiclesInvolved.ThreeOrMoreOtherVehicles:
                    WaitForSelectableAndPickFromDropdown(XPath.VehiclesInvolved.NumberOfVehiclesInvolvedDropDown, XPath.VehiclesInvolved.Option, MotorCollisionNumberOfVehiclesInvolved.ThreeOrMoreOtherVehicles.GetDescription());
                    Reporting.AreEqual(MotorCollisionNumberOfVehiclesInvolved.ThreeOrMoreOtherVehicles.GetDescription(), claimCar.NumberOfVehiclesInvolved.GetDescription(), "Number of vehicles involved");
                    break;
                case MotorCollisionNumberOfVehiclesInvolved.NoOtherVehiclesInvolved:
                    WaitForSelectableAndPickFromDropdown(XPath.VehiclesInvolved.NumberOfVehiclesInvolvedDropDown, XPath.VehiclesInvolved.Option, MotorCollisionNumberOfVehiclesInvolved.NoOtherVehiclesInvolved.GetDescription());
                    Reporting.AreEqual(MotorCollisionNumberOfVehiclesInvolved.NoOtherVehiclesInvolved.GetDescription(), claimCar.NumberOfVehiclesInvolved.GetDescription(), "Number of vehicles involved");
                    break;
                // I'm not sure option is only displayed for the motor policies with cover type MFCO
                case MotorCollisionNumberOfVehiclesInvolved.ImNotSure when claimCar.Policy.GetCoverType() == PolicyMotor.MotorCovers.MFCO:
                    WaitForSelectableAndPickFromDropdown(XPath.VehiclesInvolved.NumberOfVehiclesInvolvedDropDown, XPath.VehiclesInvolved.Option, MotorCollisionNumberOfVehiclesInvolved.ImNotSure.GetDescription());
                    Reporting.AreEqual(MotorCollisionNumberOfVehiclesInvolved.ImNotSure.GetDescription(), claimCar.NumberOfVehiclesInvolved.GetDescription(), "Number of vehicles involved");
                    break;
                default:
                    Reporting.Error($"Invalid selected value {claimCar.NumberOfVehiclesInvolved.GetDescription()}");
                    break;
            }
        }

        /// <summary>
        /// Verify the Can't claim online blocking message
        /// </summary>
        public void VerifyCantClaimOnlineErrorMessage()
        {
            Reporting.Log("Verify can't claim online error card", _driver.TakeSnapshot());
            Reporting.AreEqual(Constants.Message.CantClaimOnlineMessage, GetInnerText(XPathBaseMotorClaim.NotificationCard.Body).StripLineFeedAndCarriageReturns(false), "expected can't claim online card content is displayed");
        }


        /// <summary>
        /// First, It select the number of cars involved in the accident
        /// then based on that if it's a single vehicle collision then 
        /// it selects the claim scenario and liability type
        /// </summary>        
        public void SelectAccidentWithAndClickNext(ClaimCar claimCar)
        {
            SetNumberOfVehicleInvolved(claimCar);

            if (claimCar.DamageType == MotorClaimDamageType.SingleVehicleCollision)
            {
                if (claimCar.Policy.GetCoverType() != PolicyMotor.MotorCovers.MFCO)
                {
                    ClickNext();
                    return;
                }
                else
                {
                    switch (claimCar.ClaimScenario)
                    {
                        case MotorClaimScenario.AccidentWithYourOwnProperty:
                            ClickControl(XPath.AccidentWith.YourOwnProperty);
                            break;
                        case MotorClaimScenario.AccidentWithSomeoneElseProperty:
                            ClickControl(XPath.AccidentWith.SomeoneElseProperty);
                            SelectTPPropertyDamageAndClaimType(claimCar.IsTPPropertyDamage, claimCar.OnlyClaimDamageToTP);
                            break;
                        case MotorClaimScenario.AccidentWithWildlife:
                            ClickControl(XPath.AccidentWith.Wildlife);
                            break;
                        case MotorClaimScenario.AccidentWithSomeonesPet:
                            ClickControl(XPath.AccidentWith.SomeonesPet);
                            SelectTPPropertyDamageAndClaimType(claimCar.IsTPPropertyDamage, claimCar.OnlyClaimDamageToTP);
                            break;
                        case MotorClaimScenario.AccidentWithSomethingElse:
                            ClickControl(XPath.AccidentWith.SomethingElse);
                            SelectTPPropertyDamageAndClaimType(claimCar.IsTPPropertyDamage, claimCar.OnlyClaimDamageToTP);
                            break;
                        default:
                            throw new NotImplementedException($"{claimCar.ClaimScenario} is not supported");
                    }
                    Reporting.Log("About the incident page - Before clicking Next Button", _driver.TakeSnapshot());
                    ClickNext();
                }
            }
            else
            {
                if (claimCar.Policy.GetCoverType() == PolicyMotor.MotorCovers.MFCO)
                {
                    WantToClaimForDamageToYourCar = !claimCar.OnlyClaimDamageToTP;
                }
                Reporting.Log("About the incident page - Before clicking Next Button", _driver.TakeSnapshot());
                ClickNext();
            }
        }

        /// <summary>
        /// Answer whether the third party property involved in an accident was damaged, and if their 
        /// property is damaged we also answer whether the member wishes to claim for their own vehicle.
        /// 
        /// NOTE: isOnlyTPClaim is INVERTED because the question the member actually answers is 
        /// "Do you want to claim for the damage to your car?"
        /// </summary>
        /// <param name="isTPAssetDamaged">Is the third party property damaged?</param>
        /// <param name="isOnlyTPClaim">Do we only want to claim for the Third Party property?</param>
        private void SelectTPPropertyDamageAndClaimType(bool? isTPAssetDamaged, bool isOnlyTPClaim)
        {
            WasTheirAssetDamaged = isTPAssetDamaged;
            if (isTPAssetDamaged != false)
            {
                WantToClaimForDamageToYourCar = !isOnlyTPClaim;
            }
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }
    }
}