using Rac.TestAutomation.Common;
using System.Collections.Generic;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace UIDriver.Pages.Spark.Claim.Motor.Collision
{
    public class ReviewYourClaim : BaseMotorClaimPage
    {
        #region CONSTANTS
        private class Constants
        {
            public static readonly string ActiveStepperLabel = "Review your claim";
            public static readonly string Yes = "Yes";
            public static readonly string No = "No";
            public static readonly string Unknown = "Unknown";
            public static readonly string NotSure = "I'm not sure";

            public static readonly string HeaderText = "Review and confirm your claim";
            public static string ClaimNumberText(string claimNumber) => $"Your claim number is {claimNumber}";
            public static class Label
            {
                public static readonly string YourClaimSummary = "Your claim summary";
                public static readonly string ClaimType = "Claim type";
                public static readonly string MoreClaimDetails = "More claim details";
                public static class ContactDetails
                {
                    public static readonly string ContactDetailsHeader = "Contact details";
                    public static readonly string ContactNumber = "Contact number";
                    public static readonly string Email = "Email";
                }
                public static class WhenHappened
                {
                    public static readonly string WhenHappenedHeader = "When it happened";
                    public static readonly string AccidentDate = "Date of the incident";
                    public static readonly string ApproximateTime = "Approximate time";
                }
                public static class AboutTheAccident
                {
                    public static readonly string WhenHappenedHeader = "About the accident";
                    public static readonly string AccidentWith = "The accident was with...";
                }
                public static class MoreAboutAccident
                {
                    public static readonly string WhenHappenedHeader = "More about the accident";
                    public static readonly string YourCarWas = "Your car was...";
                }
                public static class WhereHowItHappened
                {
                    public static readonly string WhenHappenedHeader = "Where and how it happened";
                    public static readonly string WhereHappened = "Where the incident happened";
                    public static readonly string HowHappened = "Please describe what happened";
                }
                public static class DriverOfYourCar
                {
                    public static readonly string DriverOfYourCarHeader = "Driver of your car";
                    public static readonly string WereYourDriver = "Were you the driver?";
                    public static readonly string WasTheDriver = "Was the driver...";
                    public static readonly string FirstName = "First name";
                    public static readonly string MiddleName = "Middle name";
                    public static readonly string LastName = "Last name";
                    public static readonly string DOB = "Date of birth";
                    public static readonly string ContactNumber = "Contact number";
                    public static readonly string Address = "Address";
                }
                public static class DriverHistory
                {
                    public static string DriverHistoryHeader(bool isClaimantDriver, string driverName) => isClaimantDriver ? "Your driver history" : $"{driverName}'s driver history";
                    public static string UnderTheInfluence(bool isClaimantDriver, string driverName) => isClaimantDriver ?
                        "Were you under the influence of alcohol or drugs at the time of the incident?" :
                        $"Was {driverName} under the influence of alcohol or drugs at the time of the incident?";
                    public static string LicenseMoreThan2Years(bool isClaimantDriver, string driverName) => isClaimantDriver ?
                        "Have you held a driver's licence for more than 2 years?" :
                        $"Has {driverName} held a driver's licence for more than 2 years?";
                    public static string LicenseCancelledInLast3Years(bool isClaimantDriver, string driverName) => isClaimantDriver ?
                        "Has your driver's licence been suspended or cancelled in the last 3 years?" :
                        $"Has {driverName}'s driver's licence been suspended or cancelled in the last 3 years?";
                    public static readonly string ProvideDeatils = "Please provide details";
                }
                public static class AboutYourCar
                {
                    public static readonly string AboutYourCarHeader = "About your car";
                    public static readonly string DaamgeDescription = "Please describe the damage to your car";
                    public static readonly string CarTowed = "Was your car towed?";
                    public static readonly string CarDrivable = "Can your car be safely driven to the repairer?";
                    public static readonly string PreferredArea = "Preferred area for repairs";
                }
                public static class WheresYourCar
                {
                    public static readonly string WheresYourCarHeader = "Where's your car";
                    public static readonly string CarIsAt = "Your car is at...";
                    public static readonly string CarLocation = "Your car's location";
                    public static readonly string BusinessName = "Business name";
                    public static readonly string ContactNumber = "Contact number";
                    public static readonly string Address = "Address or suburb";
                }

                public static readonly string InformationText = "After this, you won't be able to make any changes online";
            }

            public static class Content
            {
                public static readonly string ClaimType = "Car accident";
            }
        }

        #endregion

        #region XPATHS
        private static class XPath
        {
            public static readonly string Header = "id('review-your-claim-header')";
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";
            public class PolicyCard
            {
                public static readonly string CarMakeAndModel = "id('policy-card-content-policy-details-header-title-summary')";
                public static readonly string CarRego = "id('policy-card-content-policy-details-header-subtitle-summary')";
                public static readonly string PolicyNumber = "id('policy-card-content-policy-details-property-0-policy-number-summary')";
            }

            public static class Label
            {
                public static readonly string YourClaimSummary = "id('review-your-claim-title')";
                public static readonly string ClaimType = "id('review-your-claim-claim-type-title')";
                public static readonly string MoreClaimDetails = "id('review-your-claim-expand')";
                public static class ContactDetails
                {
                    public static readonly string ContactDetailsHeader = "id('review-your-claim-contact-details-section-title')";
                    public static readonly string ContactNumber = "id('contact-number-label')";
                    public static readonly string Email = "id('email-label')";
                }
                public static class WhenHappened
                {
                    public static readonly string WhenHappenedHeader = "id('review-your-claim-when-it-happened-section-title')";
                    public static readonly string AccidentDate = "id('date-of-the-incident-label')";
                    public static readonly string ApproximateTime = "id('approximate-time-label')";
                }
                public static class AboutTheAccident
                {
                    public static readonly string AboutTheAccidentHeader = "id('review-your-claim-about-the-accident-title')";
                    public static readonly string AccidentWith = "id('the-accident-was-with-label')";
                }
                public static class MoreAboutAccident
                {
                    public static readonly string MoreAboutAccidentHeader = "id('review-your-claim-more-about-the-accident-title')";
                    public static readonly string YourCarWas = "id('your-car-was-label')";
                }
                public static class WhereHowItHappened
                {
                    public static readonly string WhereHowItHappenedHeader = "id('review-your-claim-where-and-how-title')";
                    public static readonly string WhereHappened = "id('provide-the-location-label')";
                    public static readonly string HowHappened = "id('please-describe-what-happened-label')";
                }
                public static class DriverOfYourCar
                {
                    public static readonly string DriverOfYourCarHeader = "id('review-your-claim-driver-of-your-car-title')";
                    public static readonly string WereYouAreTheDriver = "id('were-you-the-driver-label')";
                    public static readonly string WasTheDriver = "id('was-the-driver-label')";
                    public static readonly string FirstName = "id('other-driver-first-name-label')";
                    public static readonly string MiddleName = "id('middle-name-label')";
                    public static readonly string LastName = "id('other-driver-last-name-label')";
                    public static readonly string DOB = "id('other-driver-date-of-birth-label')";
                    public static readonly string ContactNumber = "id('other-driver-contact-number-label')";
                    public static readonly string Address = "id('other-driver-address-label')";
                }
                public static class DriverHistory
                {
                    public static readonly string DriverHistoryHeader = "id('review-your-claim-driver-history-title')";
                    public static readonly string UnderTheInfluence = "id('driverWasUnderTheInfluence-label')";
                    public static readonly string LicenseMoreThan2Years = "id('driverHasValidLicence-label')";
                    public static readonly string LicenseCancelledInLast3Years = "id('driverHasLicenceSuspensionOrCancellation-label')";
                    public static readonly string ProvideDeatils = "id('please-provide-details-label')";
                }

                public static class Witnesses
                {
                    public static readonly string WitnessesHeader = "id('review-your-claim-witness-details-title')";                    
                }

                public static class AboutYourCar
                {
                    public static readonly string AboutYourCarHeader = "id('review-your-claim-about-your-car-title')";
                    public static readonly string DaamgeDescription = "id('please-describe-the-damage-to-your-car-label')";
                    public static readonly string CarTowed = "id('was-your-car-towed-label')";
                    public static readonly string CarDrivable = "id('can-your-car-be-safely-driven-to-the-repairer-label')";
                    public static readonly string PreferredArea = "id('preferred-area-for-repairs-label')";
                }
                public static class WheresYourCar
                {
                    public static readonly string WheresYourCarHeader = "id('review-your-claim-where-is-your-car-title')";
                    public static readonly string CarIsAtLocation = "id('your-car-is-at-label')";
                    public static readonly string CarLocation = "id('your-cars-location-label')";
                    public static readonly string BusinessName = "id('business-name-label')";
                    public static readonly string ContactNumber = "id('businessContactNumber-label')";
                    public static readonly string Address = "id('address-or-suburb-label')";
                }

                public static readonly string InformationText = "id('notification-card-title')";
            }

            public static class Content
            {
                public static readonly string ClaimType = "id('-content')";
                public static readonly string ClaimantContactNumber = "id('contact-number-content')";
                public static readonly string ClaimantEmail = "id('email-content')";
                public static readonly string AccidentDate = "id('date-of-the-incident-content')";
                public static readonly string AccidentTime = "id('approximate-time-content')";

                public static readonly string NumberOfOtherVehiclesInvolved = "id('number-of-other-vehicles-involved-content')";
                public static readonly string AccidentWasWith = "id('the-accident-was-with-content')";
                public static readonly string WasPetInjured = "id('was-the-pet-or-animal-injured-content')";
                public static readonly string WasPropertyDamaged = "id('was-their-property-damaged-content')";
                public static readonly string WasSomeoneElsePropertyDamaged = "id('was-someone-elses-property-damaged-content')";
                public static readonly string ClaimDamageToYourCar = "id('do-you-want-to-claim-for-the-damage-to-your-car-content')";

                public static readonly string YourCarWas = "id('your-car-was-content')";
                public static readonly string HowItHappened = "id('how-it-happened-content')";
                public static readonly string DidAnotherVehicleHitYourCar = "id('did-another-vehicle-hit-your-car-content')";
                public static readonly string WhatScenarioDdescribesWhatHappened = "id('what-scenario-best-describes-what-happened-content')";
                public static readonly string DidTheOtherVehicle = "id('did-the-other-vehicle-content')";
                public static readonly string DidYourCar = "id('did-your-car-content')";
                public static readonly string TheCars = "id('the-vehicles-content')";

                public static readonly string WhereTheAccidentHappened = "id('where-the-incident-happened-content')";
                public static readonly string HowTheAccidentHappened = "id('please-describe-what-happened-content')";
                public static readonly string ProvideTheLocation = "id('provide-the-location-content')";
                public static readonly string WerePoliceInvolved = "id('were-the-police-involved-content')";
                public static readonly string PoliceReportNumber = "id('police-report-number-content')";

                public static readonly string WasTheDriver = "id('was-the-driver-content')";
                public static readonly string WereYouAreTheDriver = "id('were-you-the-driver-content')";
                public static readonly string DriverFristName = "id('other-driver-first-name-content')";
                public static readonly string DriverMiddleName = "id('middle-name-content')";
                public static readonly string DriverLastName = "id('other-driver-last-name-content')";
                public static readonly string DriverDOB = "id('other-driver-date-of-birth-content')";
                public static readonly string DriversContactNumber = "id('other-driver-contact-number-content')";
                public static readonly string DriversAddress = "id('other-driver-address-content')";

                public static readonly string WasDriverUnderTheInfluence = "id('driverWasUnderTheInfluence-content')";
                public static readonly string HasValidLicence = "id('driverHasValidLicence-content')";
                public static readonly string HasLicenceCancelled = "id('driverHasLicenceSuspensionOrCancellation-content')";

                public static readonly string DoYouHaveTPDetails = "id('do-you-have-any-of-their-details-content')";
                public static readonly string TPCarRego = "id('car-registration-content')";
                public static readonly string TPFirstName = "id('third-party-first-name-content')";
                public static readonly string TPLasttName = "id('third-party-last-name-content')";
                public static readonly string TPContactNumber = "id('third-party-contact-number-content')";
                public static readonly string TPEmail = "id('third-party-email-content')";
                public static readonly string TPAddress = "id('third-party-address-content')";
                public static readonly string TPKnownPH = "id('did-you-know-the-other-party-before-the-accident-content')";

                public static readonly string TPWasDriverTheOwner = "id('was-the-driver-the-owner-content')";
                public static readonly string TPOwnersNameAndContactDetails = "id('owners-name-and-contact-details-content')";
                public static readonly string TPInsuranceCompany = "//p[contains(@id,'insurance-company-content')]";
                public static readonly string TPClaimNumber = "id('their-claim-number-content')";
                public static readonly string TPDamageDescription = "//p[contains(@id,'please-describe-their-damage') and contains(@id,'content')]";

                public static readonly string Witness1 = "id('witness-1-content')";
                public static readonly string Witness2 = "id('witness-2-content')";

                public static readonly string DamageDescription = "id('please-describe-the-damage-to-your-car-content')";
                public static readonly string WasCarTowed = "id('was-your-car-towed-content')";
                public static readonly string CarDrivable = "id('can-your-car-be-safely-driven-to-the-repairer-content')";
                public static readonly string PreferredArea = "id('preferred-area-for-repairs-content')";

                public static readonly string YourCarAt = "id('your-car-is-at-content')";
                public static readonly string YourCarLocation = "id('your-cars-location-label')";
                public static readonly string BusinessName = "id('business-name-content')";
                public static readonly string BusinessContactNumber = "id('businessContactNumber-content')";
                public static readonly string BusinessAddress = "id('address-or-suburb-content')";
                public static readonly string ProvideDetailsToLocateYourCar = "id('please-provide-any-details-that-may-help-us-locate-your-car-content')";
                public static readonly string TellUsWhereYourCar = "id('please-tell-us-where-your-car-is-content')";
                public static readonly string CarLocation = "id('your-cars-location-content')";

            }

            public static class Button
            {
                public static readonly string MoreClaimDetails = "id('review-your-claim-expand')";
                public static readonly string HideClaimDetails = "id('review-your-claim-collapse')";
                public static readonly string SubmitClaim = "id('submit-button')";
            }
        }

        #endregion


        public ReviewYourClaim(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Button.SubmitClaim);
            }
            catch
            {
                return false;
            }

            Reporting.LogPageChange("Review your claim");
            Reporting.Log("Review your claim after loading confirmed", _driver.TakeSnapshot());
            return true;
        }

        public void DetailedUiChecking(ClaimCar claim)
        {
            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            Reporting.AreEqual(Constants.Label.YourClaimSummary, GetInnerText(XPath.Label.YourClaimSummary), "your claim summary label");
            Reporting.AreEqual(Constants.Label.ClaimType, GetInnerText(XPath.Label.ClaimType), "Claim type label");

            Reporting.AreEqual(Constants.Label.ContactDetails.ContactDetailsHeader, GetInnerText(XPath.Label.ContactDetails.ContactDetailsHeader), "Contact details label");
            Reporting.AreEqual(Constants.Label.ContactDetails.ContactNumber, GetInnerText(XPath.Label.ContactDetails.ContactNumber), "Contact number label");
            Reporting.AreEqual(Constants.Label.ContactDetails.Email, GetInnerText(XPath.Label.ContactDetails.Email), "Contact email label");

            Reporting.AreEqual(Constants.Label.WhenHappened.WhenHappenedHeader, GetInnerText(XPath.Label.WhenHappened.WhenHappenedHeader), "When it happened label");
            Reporting.AreEqual(Constants.Label.WhenHappened.AccidentDate, GetInnerText(XPath.Label.WhenHappened.AccidentDate), "Date of the accident label");
            Reporting.AreEqual(Constants.Label.WhenHappened.ApproximateTime, GetInnerText(XPath.Label.WhenHappened.ApproximateTime), "Approximate time label");

            Reporting.AreEqual(Constants.Label.MoreClaimDetails, GetInnerText(XPath.Label.MoreClaimDetails), "More claim details expand label");

            if (claim.DamageType == MotorClaimDamageType.SingleVehicleCollision)
            {
                ClickControl(XPath.Button.MoreClaimDetails);

                ScrollElementIntoView(XPath.Label.WhereHowItHappened.WhereHowItHappenedHeader);
                Reporting.Log("Review your claim page - Where and how it happened", _driver.TakeSnapshot());

                Reporting.AreEqual(Constants.Label.AboutTheAccident.WhenHappenedHeader, GetInnerText(XPath.Label.AboutTheAccident.AboutTheAccidentHeader), "About the accident label");
                Reporting.AreEqual(Constants.Label.AboutTheAccident.AccidentWith, GetInnerText(XPath.Label.AboutTheAccident.AccidentWith), "The accident was with label");

                Reporting.AreEqual(Constants.Label.MoreAboutAccident.WhenHappenedHeader, GetInnerText(XPath.Label.MoreAboutAccident.MoreAboutAccidentHeader), "More about the accident label");
                Reporting.AreEqual(Constants.Label.MoreAboutAccident.YourCarWas, GetInnerText(XPath.Label.MoreAboutAccident.YourCarWas), "Your car was label");

                Reporting.AreEqual(Constants.Label.WhereHowItHappened.WhenHappenedHeader, GetInnerText(XPath.Label.WhereHowItHappened.WhereHowItHappenedHeader), "Where and how it happened label");
                Reporting.AreEqual(Constants.Label.WhereHowItHappened.WhereHappened, GetInnerText(XPath.Label.WhereHowItHappened.WhereHappened), "Where the accident happened label");
                Reporting.AreEqual(Constants.Label.WhereHowItHappened.HowHappened, GetInnerText(XPath.Label.WhereHowItHappened.HowHappened), "Please describe how the accident happened label");

                ScrollElementIntoView(XPath.Label.WhereHowItHappened.WhereHowItHappenedHeader);
                Reporting.Log("Review your claim page - Where it happened", _driver.TakeSnapshot());

                Reporting.AreEqual(Constants.Label.DriverOfYourCar.DriverOfYourCarHeader, GetInnerText(XPath.Label.DriverOfYourCar.DriverOfYourCarHeader), "Driver of your car label");
                Reporting.AreEqual(Constants.Label.DriverOfYourCar.WereYourDriver, GetInnerText(XPath.Label.DriverOfYourCar.WereYouAreTheDriver), "Were your driver label");


                if (!claim.IsClaimantDriver && claim.Driver.expectedClaimDrivers.Count() > 0)
                {
                    Reporting.AreEqual(Constants.Label.DriverOfYourCar.WasTheDriver, GetInnerText(XPath.Label.DriverOfYourCar.WasTheDriver), "Was the driver label");
                }
                else if (claim.Driver.isNewContact())
                {
                    Reporting.AreEqual(Constants.Label.DriverOfYourCar.FirstName, GetInnerText(XPath.Label.DriverOfYourCar.FirstName), "First name label");
                    Reporting.AreEqual(Constants.Label.DriverOfYourCar.MiddleName, GetInnerText(XPath.Label.DriverOfYourCar.MiddleName), "Middle name label");
                    Reporting.AreEqual(Constants.Label.DriverOfYourCar.LastName, GetInnerText(XPath.Label.DriverOfYourCar.LastName), "Last name label");
                    Reporting.AreEqual(Constants.Label.DriverOfYourCar.DOB, GetInnerText(XPath.Label.DriverOfYourCar.DOB), "Date of birth label");
                    Reporting.AreEqual(Constants.Label.DriverOfYourCar.ContactNumber, GetInnerText(XPath.Label.DriverOfYourCar.ContactNumber), "Contact number label");
                    Reporting.AreEqual(Constants.Label.DriverOfYourCar.Address, GetInnerText(XPath.Label.DriverOfYourCar.Address), "Address label");
                }

                Reporting.AreEqual(Constants.Label.DriverHistory.DriverHistoryHeader(claim.IsClaimantDriver, claim.Driver.DriverDetails.FirstName), GetInnerText(XPath.Label.DriverHistory.DriverHistoryHeader), "Driver history label");
                Reporting.AreEqual(Constants.Label.DriverHistory.UnderTheInfluence(claim.IsClaimantDriver, claim.Driver.DriverDetails.FirstName), GetInnerText(XPath.Label.DriverHistory.UnderTheInfluence), "Driver under the influence label");
                Reporting.IsFalse(IsControlDisplayed(XPath.Label.DriverHistory.LicenseMoreThan2Years),"'Driver license more than 2 years' field should not be displayed");
                Reporting.AreEqual(Constants.Label.DriverHistory.LicenseCancelledInLast3Years(claim.IsClaimantDriver, claim.Driver.DriverDetails.FirstName), GetInnerText(XPath.Label.DriverHistory.LicenseCancelledInLast3Years), "Driver license suspended in last 3 years label");

                if (!claim.OnlyClaimDamageToTP)
                {
                    ScrollElementIntoView(XPath.Label.AboutYourCar.AboutYourCarHeader);
                    Reporting.Log("Review your claim page -About your car", _driver.TakeSnapshot());

                    Reporting.AreEqual(Constants.Label.AboutYourCar.AboutYourCarHeader, GetInnerText(XPath.Label.AboutYourCar.AboutYourCarHeader), "About your car label");
                    Reporting.AreEqual(Constants.Label.AboutYourCar.DaamgeDescription, GetInnerText(XPath.Label.AboutYourCar.DaamgeDescription), "Damage description label");
                    Reporting.AreEqual(Constants.Label.AboutYourCar.CarTowed, GetInnerText(XPath.Label.AboutYourCar.CarTowed), "Was your car towed label");
                    if (claim.TowedVehicleDetails.WasVehicleTowed == false)
                    {
                        Reporting.AreEqual(Constants.Label.AboutYourCar.CarDrivable, GetInnerText(XPath.Label.AboutYourCar.CarDrivable), "Car drivable label");
                        if (claim.IsVehicleDriveable == true)
                        {
                            Reporting.AreEqual(Constants.Label.AboutYourCar.PreferredArea, GetInnerText(XPath.Label.AboutYourCar.PreferredArea), "Preferred area for repairs label");
                        }
                    }

                    if (claim.IsVehicleDriveable != true)
                    {
                        Reporting.AreEqual(Constants.Label.WheresYourCar.WheresYourCarHeader, GetInnerText(XPath.Label.WheresYourCar.WheresYourCarHeader), "Where is your car label");

                        if (claim.TowedVehicleDetails.WasVehicleTowed == true)
                        {
                            Reporting.AreEqual(Constants.Label.WheresYourCar.CarIsAt, GetInnerText(XPath.Label.WheresYourCar.CarIsAtLocation), "Your car location label");

                            if (claim.TowedVehicleDetails.TowedTo == MotorClaimTowedTo.HoldingYard ||
                            claim.TowedVehicleDetails.TowedTo == MotorClaimTowedTo.Repairer)
                            {
                                Reporting.AreEqual(Constants.Label.WheresYourCar.BusinessName, GetInnerText(XPath.Label.WheresYourCar.BusinessName), "Business name label");
                                Reporting.AreEqual(Constants.Label.WheresYourCar.ContactNumber, GetInnerText(XPath.Label.WheresYourCar.ContactNumber), "Contact number label");
                                Reporting.AreEqual(Constants.Label.WheresYourCar.Address, GetInnerText(XPath.Label.WheresYourCar.Address), "Address or suburb label");
                            }
                        }
                        else
                        {
                            Reporting.AreEqual(Constants.Label.WheresYourCar.CarLocation, GetInnerText(XPath.Label.WheresYourCar.CarLocation), "Your car is at label");
                        }
                    }
                }

                Reporting.AreEqual(Constants.Label.InformationText, GetInnerText(XPath.Label.InformationText), "Information Text");
                ClickControl(XPath.Button.HideClaimDetails);
            }
        }

        public void VerifyClaimSummaryDetails(ClaimCar claimCar)
        {
            Reporting.AreEqual(Constants.HeaderText, GetInnerText(XPath.Header), "Page header");
            Reporting.AreEqual(Constants.ClaimNumberText(claimCar.ClaimNumber), GetInnerText(XPath.ClaimNumber), "Claim number");

            Reporting.AreEqual($"{claimCar.Policy.Vehicle.Year} {claimCar.Policy.Vehicle.Make}", GetInnerText(XPath.PolicyCard.CarMakeAndModel), "Car Make and Model");

            if (DataHelper.IsRegistrationNumberConsideredValid(claimCar.Policy.Vehicle.Registration))
            {
                Reporting.AreEqual(claimCar.Policy.Vehicle.Registration.Trim(), GetInnerText(XPath.PolicyCard.CarRego), "Car Registration Number");
            }
            Reporting.AreEqual($"Policy number: {claimCar.Policy.PolicyNumber}", GetInnerText(XPath.PolicyCard.PolicyNumber), "Policy number");

            Reporting.AreEqual(Constants.Content.ClaimType, GetInnerText(XPath.Content.ClaimType), "Claim type is Car accident");
            
            Reporting.AreEqual(DataHelper.MaskPhoneNumber(claimCar.Claimant.GetPhone()).Replace(" ", string.Empty), GetInnerText(XPath.Content.ClaimantContactNumber).Replace(" ", string.Empty), "expected masked contact phone number for Informant with displayed content");

            Reporting.AreEqual(DataHelper.MaskEmailAddress(claimCar.Claimant.PrivateEmail.Address), GetInnerText(XPath.Content.ClaimantEmail), 
                ignoreCase: true, "expected masked private email address for Informant with displayed content");

            Reporting.AreEqual(claimCar.EventDateTime.ToString("dd/MM/yyyy"), GetInnerText(XPath.Content.AccidentDate), "Accident date");
            Reporting.AreEqual(claimCar.EventDateTime.ToString("h:mmtt").ToLower(), GetInnerText(XPath.Content.AccidentTime), "Accident time");

            ClickControl(XPath.Button.MoreClaimDetails);

            ScrollElementIntoView(XPath.Label.AboutTheAccident.AboutTheAccidentHeader);
            Reporting.Log("Review your claim page - About the accident", _driver.TakeSnapshot());
            VerifyAboutTheAccidentClaimSummary(claimCar);

            ScrollElementIntoView(XPath.Label.MoreAboutAccident.MoreAboutAccidentHeader);
            Reporting.Log("Review your claim page - More about the accident", _driver.TakeSnapshot());
            VerifyMoreAboutTheAccidentClaimSummary(claimCar);

            ScrollElementIntoView(XPath.Label.WhereHowItHappened.WhereHowItHappenedHeader);
            Reporting.Log("Review your claim page - Where and how it happened", _driver.TakeSnapshot());
            Reporting.AreEqual(claimCar.EventLocation, GetInnerText(XPath.Content.WhereTheAccidentHappened), "Where the accident happened field");
            Reporting.AreEqual(claimCar.AccountOfAccident.StripLineFeedAndCarriageReturns(), GetInnerText(XPath.Content.HowTheAccidentHappened), "How the accident happened field");
            Reporting.AreEqual(DataHelper.BooleanToStringYesNoAndCustomText(claimCar.IsPoliceInvolved, Constants.NotSure), GetInnerText(XPath.Content.WerePoliceInvolved), "Were the police involved");
            if (claimCar.IsPoliceInvolved == true && !string.IsNullOrEmpty(claimCar.PoliceReportNumber))
            {
                Reporting.AreEqual(claimCar.PoliceReportNumber, GetInnerText(XPath.Content.PoliceReportNumber), "Police report number");
            }

            ScrollElementIntoView(XPath.Label.DriverOfYourCar.DriverOfYourCarHeader);
            Reporting.Log("Review your claim page - Driver Your Car", _driver.TakeSnapshot());
            Reporting.AreEqual(DataHelper.BooleanToStringYesNoAndCustomText(claimCar.IsClaimantDriver, Constants.Unknown), GetInnerText(XPath.Content.WereYouAreTheDriver), "Were you the driver?");

            if (!claimCar.IsClaimantDriver)
            {
                if (claimCar.Driver.isNewContact())
                {
                    if (claimCar.Driver.expectedClaimDrivers.Count() > 0)
                    {
                        Reporting.AreEqual("Someone else", GetInnerText(XPath.Content.WasTheDriver), "Was the driver...");
                    }
                    Reporting.AreEqual(claimCar.Driver.DriverDetails.FirstName, GetInnerText(XPath.Content.DriverFristName), "Driver's first name");
                    Reporting.AreEqual(claimCar.Driver.DriverDetails.MiddleName, GetInnerText(XPath.Content.DriverMiddleName), "Driver's middle name");
                    Reporting.AreEqual(claimCar.Driver.DriverDetails.Surname, GetInnerText(XPath.Content.DriverLastName), "Driver's last name");
                    Reporting.AreEqual(claimCar.Driver.DriverDetails.DateOfBirth.ToString("dd/MM/yyyy"), GetInnerText(XPath.Content.DriverDOB), "Driver's date of birth");
                    Reporting.AreEqual(claimCar.Driver.DriverDetails.MobilePhoneNumber, GetInnerText(XPath.Content.DriversContactNumber), "Driver's contact number");
                    Reporting.AreEqual(claimCar.Driver.DriverDetails.MailingAddress.StreetSuburbState(true), GetInnerText(XPath.Content.DriversAddress), ignoreCase: true, "Driver's address");
                }
                else
                {
                    Reporting.AreEqual(claimCar.Driver.DriverDetails.GetFullName(), GetInnerText(XPath.Content.WasTheDriver), "Was the driver...");
                }
            }
            if (claimCar.IsQualifiedForDriverHistoryQuestionnaire)
            {
                ScrollElementIntoView(XPath.Label.DriverHistory.DriverHistoryHeader);
                Reporting.Log("Review your claim page - Driver history", _driver.TakeSnapshot());
                Reporting.AreEqual(DataHelper.BooleanToStringYesNoAndCustomText(claimCar.Driver.WasDriverDrunk, Constants.Unknown), GetInnerText(XPath.Content.WasDriverUnderTheInfluence), "Driver was under the influence of drug ");
                Reporting.IsFalse(IsControlDisplayed(XPath.Content.HasValidLicence), "'Driver's licence is more than 2 years' field should not be displayed");
            }
            else
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.Content.HasValidLicence), "'Driver's licence is more than 2 years' field should not be displayed - refer INSU-21");
                Reporting.IsFalse(IsControlDisplayed(XPath.Content.HasLicenceCancelled), "'Driver's license cancelled in last 3 years' field should not be displayed - refer INSU-21");
            }
            ScrollElementIntoView(XPath.Label.Witnesses.WitnessesHeader);
            Reporting.Log("Review your claim page - Witnesses", _driver.TakeSnapshot());
            if (claimCar.Witness?.Any() == true)
            {
                Reporting.AreEqual(FormatWitnessDetailsToString(claimCar.Witness.FirstOrDefault()), GetInnerText(XPath.Content.Witness1), "first witness details");
                if (claimCar.Witness.Count() > 1)
                {
                    Reporting.AreEqual(FormatWitnessDetailsToString(claimCar.Witness[1]), GetInnerText(XPath.Content.Witness2), "second witness details");
                }
            }

            if (claimCar.IsTPPropertyDamage == true)
            {
                Reporting.AreEqual(DataHelper.BooleanToStringYesNo(claimCar.ThirdParty != null && claimCar.ThirdParty.Count() > 0), GetInnerText(XPath.Content.DoYouHaveTPDetails), "Do you have third party details");
            }

            if (claimCar.ThirdParty != null && claimCar.ThirdParty.Count() > 0)
            {
                var thirdParty = claimCar.ThirdParty.First();

                if (claimCar.DamageType == MotorClaimDamageType.MultipleVehicleCollision)
                {
                    if (!string.IsNullOrEmpty(thirdParty.Rego))
                    {
                        Reporting.AreEqual(thirdParty.Rego, GetInnerText(XPath.Content.TPCarRego), "Third party car rego");
                    }
                    else
                    {
                        Reporting.AreEqual("-", GetInnerText(XPath.Content.TPCarRego), "Third party car rego is '-' because it's not provided");
                    }
                }

                if (!string.IsNullOrEmpty(thirdParty.FirstName))
                {
                    Reporting.AreEqual(thirdParty.FirstName, GetInnerText(XPath.Content.TPFirstName), "Third party first name");
                }
                else
                {
                    Reporting.AreEqual("-", GetInnerText(XPath.Content.TPFirstName), "Third party first name is '-' because it's not provided");
                }

                if (!string.IsNullOrEmpty(thirdParty.Surname))
                {
                    Reporting.AreEqual(thirdParty.Surname, GetInnerText(XPath.Content.TPLasttName), "Third party last name");
                }
                else
                {
                    Reporting.AreEqual("-", GetInnerText(XPath.Content.TPLasttName), "Third party last name is '-' because it's not provided");
                }

                if (!string.IsNullOrEmpty(thirdParty.MobilePhoneNumber) || !string.IsNullOrEmpty(thirdParty.HomePhoneNumber))
                {
                    var phoneNumber = string.IsNullOrEmpty(thirdParty.MobilePhoneNumber) ? thirdParty.HomePhoneNumber : thirdParty.MobilePhoneNumber;
                    Reporting.AreEqual(DataHelper.FormatPhoneNumber(phoneNumber), GetInnerText(XPath.Content.TPContactNumber), "Third party phone name");
                }
                else
                {
                    Reporting.AreEqual("-", GetInnerText(XPath.Content.TPContactNumber), "Third party contact number is '-' because it's not provided");
                }

                if (!string.IsNullOrEmpty(thirdParty.GetEmail()))
                {
                    Reporting.AreEqual(thirdParty.GetEmail(), GetInnerText(XPath.Content.TPEmail), "Third party email");
                }
                else
                {
                    Reporting.AreEqual("-", GetInnerText(XPath.Content.TPEmail), "Third party email is '-' because it's not provided");
                }

                if (thirdParty.MailingAddress != null)
                {
                    Reporting.AreEqual(thirdParty.MailingAddress.StreetSuburbState(true), GetInnerText(XPath.Content.TPAddress), ignoreCase: true, "Third party address");
                }
                else
                {
                    Reporting.AreEqual("-", GetInnerText(XPath.Content.TPAddress), "Third party address is '-' because it's not provided");
                }

                Reporting.AreEqual(DataHelper.BooleanToStringYesNo(thirdParty.IsKnownToClaimant), GetInnerText(XPath.Content.TPKnownPH), "whether the Third Party was known to the PH before the accident");

                if (claimCar.DamageType == MotorClaimDamageType.MultipleVehicleCollision)
                {
                    Reporting.AreEqual(DataHelper.BooleanToStringYesNo(thirdParty.WasDriverTheOwner), GetInnerText(XPath.Content.TPWasDriverTheOwner), "Was the driver the owner");

                    if (!thirdParty.WasDriverTheOwner)
                    {
                        if (!string.IsNullOrEmpty(thirdParty.AdditionalInfo))
                        {
                            Reporting.AreEqual(thirdParty.AdditionalInfo, GetInnerText(XPath.Content.TPOwnersNameAndContactDetails), "Third party owner and contact details");
                        }
                        else
                        {
                            Reporting.AreEqual("-", GetInnerText(XPath.Content.TPOwnersNameAndContactDetails), "Third party insurance company is '-' because it's not provided");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(thirdParty.Insurer.Name))
                {
                    Reporting.AreEqual(thirdParty.Insurer.Name, GetInnerText(XPath.Content.TPInsuranceCompany), ignoreCase: true, "Third party insurance company");                   
                }
                else
                {
                    Reporting.AreEqual("-", GetInnerText(XPath.Content.TPInsuranceCompany), "Third party insurance company is '-' because it's not provided");
                }

                if (!string.IsNullOrEmpty(thirdParty.ClaimNumber))
                {
                    Reporting.AreEqual(thirdParty.ClaimNumber, GetInnerText(XPath.Content.TPClaimNumber), "Third party claim number");
                }
                else
                {
                    Reporting.AreEqual("-", GetInnerText(XPath.Content.TPClaimNumber), "Third party claim number is '-' because it's not provided");
                }

                if (!string.IsNullOrEmpty(thirdParty.DescriptionOfDamageToVehicle))
                {
                    Reporting.AreEqual(thirdParty.DescriptionOfDamageToVehicle.StripLineFeedAndCarriageReturns(), GetInnerText(XPath.Content.TPDamageDescription), "Third party damage description");
                }
                else
                {
                    Reporting.AreEqual("-", GetInnerText(XPath.Content.TPDamageDescription), "Third party damage description is '-' because it's not provided");
                }

            }

            if (!claimCar.OnlyClaimDamageToTP)
            {
                ScrollElementIntoView(XPath.Label.AboutYourCar.AboutYourCarHeader);
                Reporting.Log("Review your claim page - About your car", _driver.TakeSnapshot());
                Reporting.AreEqual(claimCar.DamageToPHVehicle.StripLineFeedAndCarriageReturns(), GetInnerText(XPath.Content.DamageDescription), "Damage description");
                Reporting.AreEqual(DataHelper.BooleanToStringYesNoAndCustomText(claimCar.TowedVehicleDetails.WasVehicleTowed, Constants.NotSure), GetInnerText(XPath.Content.WasCarTowed), "Was your car towed?");
                if (claimCar.TowedVehicleDetails.WasVehicleTowed == false)
                {
                    Reporting.AreEqual(DataHelper.BooleanToStringYesNoAndCustomText(claimCar.IsVehicleDriveable, Constants.NotSure), GetInnerText(XPath.Content.CarDrivable), "Can your car be safely driven to the repairer?");
                    if (claimCar.IsVehicleDriveable == true)
                    {
                        Reporting.AreEqual(claimCar.PreferredRepairerSuburb.SuburbAndCode(), GetInnerText(XPath.Content.PreferredArea), ignoreCase: true, "Preferred area for repairs");
                    }
                }

                if (claimCar.IsVehicleDriveable != true)
                {
                    ScrollElementIntoView(XPath.Label.WheresYourCar.WheresYourCarHeader);
                    Reporting.Log("Review your claim page - Wheres Your Car", _driver.TakeSnapshot());

                    if (claimCar.TowedVehicleDetails.WasVehicleTowed == true)
                    {
                        Reporting.AreEqual(SparkMotorTowedToText[claimCar.TowedVehicleDetails.TowedTo].TextSpark, GetInnerText(XPath.Content.YourCarAt), "expected value for 'Your car is at' field with actual value displayed");

                        if (claimCar.TowedVehicleDetails.TowedTo == MotorClaimTowedTo.HoldingYard ||
                            claimCar.TowedVehicleDetails.TowedTo == MotorClaimTowedTo.Repairer)
                        {
                            Reporting.AreEqual(claimCar.TowedVehicleDetails.BusinessDetails.BusinessName, GetInnerText(XPath.Content.BusinessName), true, "Business name field");
                            Reporting.AreEqual(DataHelper.FormatPhoneNumber(claimCar.TowedVehicleDetails.BusinessDetails.ContactNumber), GetInnerText(XPath.Content.BusinessContactNumber), true, "Business name field");
                            Reporting.AreEqual(claimCar.TowedVehicleDetails.BusinessDetails.Address, GetInnerText(XPath.Content.BusinessAddress), "Address field");
                        }
                        else if (claimCar.TowedVehicleDetails.TowedTo == MotorClaimTowedTo.Other)
                        {
                            Reporting.AreEqual(claimCar.TowedVehicleDetails.CarLocation.StripLineFeedAndCarriageReturns(), GetInnerText(XPath.Content.TellUsWhereYourCar), "Where is your car field");
                        }
                        else if (claimCar.TowedVehicleDetails.TowedTo == MotorClaimTowedTo.Unknown)
                        {
                            Reporting.AreEqual(claimCar.TowedVehicleDetails.CarLocation.StripLineFeedAndCarriageReturns(), GetInnerText(XPath.Content.ProvideDetailsToLocateYourCar), "Where is your car field");
                        }
                    }
                    else
                    {
                        Reporting.AreEqual(claimCar.TowedVehicleDetails.CarLocation.StripLineFeedAndCarriageReturns(), GetInnerText(XPath.Content.CarLocation), "Car location field");
                    }
                }
            }
        }

        /// <summary>
        /// Verify About the accident section on the claim summary page
        /// </summary>        
        private void VerifyAboutTheAccidentClaimSummary(ClaimCar claimCar)
        {
            Reporting.AreEqual(claimCar.NumberOfVehiclesInvolved.GetDescription(), GetInnerText(XPath.Content.NumberOfOtherVehiclesInvolved), "Number of other vehicles involved");

            if (claimCar.DamageType == MotorClaimDamageType.SingleVehicleCollision)
            {
                Reporting.AreEqual(claimCar.ClaimScenario.GetDescription(), GetInnerText(XPath.Content.AccidentWasWith), "The accident was with field");

                if (claimCar.ClaimScenario == MotorClaimScenario.AccidentWithSomeonesPet)
                {
                    if (claimCar.IsTPPropertyDamage == true)
                    {
                        Reporting.AreEqual("Yes", GetInnerText(XPath.Content.WasPetInjured), "Was the pet or animal injured");
                    }
                    else if (claimCar.IsTPPropertyDamage == false)
                    {
                        Reporting.AreEqual("No", GetInnerText(XPath.Content.WasPetInjured), "Was the pet or animal injured");
                    }
                    else
                    {
                        Reporting.AreEqual("Unknown", GetInnerText(XPath.Content.WasPetInjured), "Was the pet or animal injured");
                    }
                }
                else if (claimCar.ClaimScenario == MotorClaimScenario.AccidentWithSomeoneElseProperty)
                {
                    if (claimCar.IsTPPropertyDamage == true)
                    {
                        Reporting.AreEqual("Yes", GetInnerText(XPath.Content.WasPropertyDamaged), "Was their property damaged");
                    }
                    else if (claimCar.IsTPPropertyDamage == false)
                    {
                        Reporting.AreEqual("No", GetInnerText(XPath.Content.WasPropertyDamaged), "Was their property damaged");
                    }
                    else
                    {
                        Reporting.AreEqual("Unknown", GetInnerText(XPath.Content.WasPropertyDamaged), "Was their property damaged");
                    }
                }
                else if (claimCar.ClaimScenario == MotorClaimScenario.AccidentWithSomethingElse)
                {
                    if (claimCar.IsTPPropertyDamage == true)
                    {
                        Reporting.AreEqual("Yes", GetInnerText(XPath.Content.WasSomeoneElsePropertyDamaged), "Was someone else's property damaged");
                    }
                    else if (claimCar.IsTPPropertyDamage == false)
                    {
                        Reporting.AreEqual("No", GetInnerText(XPath.Content.WasSomeoneElsePropertyDamaged), "Was someone else's property damaged");
                    }
                    else
                    {
                        Reporting.AreEqual("Unknown", GetInnerText(XPath.Content.WasSomeoneElsePropertyDamaged), "Was someone else's property damaged");
                    }
                }
            }

            if (claimCar.IsTPPropertyDamage == true && claimCar.OnlyClaimDamageToTP && claimCar.Policy.GetCoverType() == MotorCovers.MFCO)
            {
                Reporting.AreEqual("No", GetInnerText(XPath.Content.ClaimDamageToYourCar), "Do you want to claim for the damage to your car");
            }
            else if (claimCar.IsTPPropertyDamage == true && !claimCar.OnlyClaimDamageToTP && claimCar.Policy.GetCoverType() == MotorCovers.MFCO)
            {
                Reporting.AreEqual("Yes", GetInnerText(XPath.Content.ClaimDamageToYourCar), "Do you want to claim for the damage to your car");
            }
        }


        /// <summary>
        /// Verify More about the accident section on the claim summary page
        /// </summary>
        private void VerifyMoreAboutTheAccidentClaimSummary(ClaimCar claimCar)
        {
            Reporting.AreEqual(SparkTravelDirectionText[claimCar.DirectionBeingTravelled].TextSpark, GetInnerText(XPath.Content.YourCarWas), "expected value for 'Your car was' field with actual value displayed");

            if (claimCar.DamageType == MotorClaimDamageType.MultipleVehicleCollision)
            {
                switch (claimCar.ClaimScenario)
                {
                    case MotorClaimScenario.WhileDrivingOtherVehicleHitRearOfMyCar:
                        Reporting.AreEqual("A vehicle hit your car", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        Reporting.AreEqual("Hit the rear of your car", GetInnerText(XPath.Content.DidTheOtherVehicle), "Did the other vehicle");
                        break;

                    case MotorClaimScenario.WhileDrivingOtherVehicleHitMyCarWhenChangingLanes:
                        Reporting.AreEqual("A vehicle hit your car", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        Reporting.AreEqual("Hit your car when changing lanes", GetInnerText(XPath.Content.DidTheOtherVehicle), "Did the other vehicle");
                        break;

                    case MotorClaimScenario.WhileDrivingOtherVehicleFailToGiveWayAndHitMyCar:
                        Reporting.AreEqual("A vehicle hit your car", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        Reporting.AreEqual("Fail to give way", GetInnerText(XPath.Content.DidTheOtherVehicle), "Did the other vehicle");
                        break;

                    case MotorClaimScenario.WhileDrivingOtherVehicleHitMyCarSomethingElseHappened:
                        Reporting.AreEqual("A vehicle hit your car", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        Reporting.AreEqual("Something else happened", GetInnerText(XPath.Content.DidTheOtherVehicle), "Did the other vehicle");
                        break;

                    case MotorClaimScenario.WhileDrivingMyCarHitAnotherCarWhenChangingLanes:
                        Reporting.AreEqual("Your car hit another vehicle", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        Reporting.AreEqual("Hit another vehicle when changing lanes", GetInnerText(XPath.Content.DidYourCar), "Did your car");
                        break;

                    case MotorClaimScenario.WhileDrivingMyCarHitRearOfAnotherCar:
                        Reporting.AreEqual("Your car hit another vehicle", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        Reporting.AreEqual("Hit the rear of another vehicle", GetInnerText(XPath.Content.DidYourCar), "Did your car");
                        break;

                    case MotorClaimScenario.WhileDrivingMyCarHitAParkedCar:
                        Reporting.AreEqual("Your car hit another vehicle", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        Reporting.AreEqual("Hit a parked vehicle", GetInnerText(XPath.Content.DidYourCar), "Did your car");
                        break;

                    case MotorClaimScenario.WhileDrivingMyCarHitAnotherCarFailToGiveWay:
                        Reporting.AreEqual("Your car hit another vehicle", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        Reporting.AreEqual("Fail to give way", GetInnerText(XPath.Content.DidYourCar), "Did your car");
                        break;

                    case MotorClaimScenario.WhileDrivingMyCarHitAnotherCarSomethingElseHappened:
                        Reporting.AreEqual("Your car hit another vehicle", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        Reporting.AreEqual("Something else happened", GetInnerText(XPath.Content.DidYourCar), "Did your car");
                        break;

                    case MotorClaimScenario.WhileDrivingOurCarHitOneAnotherCarWhenChangingLanes:
                        Reporting.AreEqual("Our vehicles hit one another", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        Reporting.AreEqual("Both changed lanes and hit each other", GetInnerText(XPath.Content.TheCars), "The cars");
                        break;

                    case MotorClaimScenario.WhileDrivingOurCarHitOneAnotherCarBothFailedToGiveWay:
                        Reporting.AreEqual("Our vehicles hit one another", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        Reporting.AreEqual("Both failed to give way", GetInnerText(XPath.Content.TheCars), "The cars");
                        break;

                    case MotorClaimScenario.WhileDrivingOurCarHitOneAnotherCarSomethingElseHappened:
                        Reporting.AreEqual("Our vehicles hit one another", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        Reporting.AreEqual("Something else happened", GetInnerText(XPath.Content.TheCars), "The cars");
                        break;

                    case MotorClaimScenario.WhileDrivingSomethingElseHappened:
                        Reporting.AreEqual("Something else happened", GetInnerText(XPath.Content.HowItHappened), "How it happened");
                        break;

                    case MotorClaimScenario.WhileReversingHitParkedCar:
                        Reporting.AreEqual("Hit a parked vehicle", GetInnerText(XPath.Content.DidYourCar), "Did your car");
                        break;

                    case MotorClaimScenario.WhileReversingHitAnotherCar:
                        Reporting.AreEqual("Hit another vehicle", GetInnerText(XPath.Content.DidYourCar), "Did your car");
                        break;

                    case MotorClaimScenario.WhileReversingHitByAnotherCar:
                        Reporting.AreEqual("Get hit in the rear by another vehicle", GetInnerText(XPath.Content.DidYourCar), "Did your car");
                        break;

                    case MotorClaimScenario.WhileReversingHitByAnotherReversingCar:
                        Reporting.AreEqual("We reversed into each other", GetInnerText(XPath.Content.DidYourCar), "Did your car");
                        break;

                    case MotorClaimScenario.WhileReversingSomethingElseHappened:
                        Reporting.AreEqual("Something else happened", GetInnerText(XPath.Content.DidYourCar), "Did your car");
                        break;

                    case MotorClaimScenario.WhileParkedAnotherCarHitMyCar:
                        Reporting.AreEqual("Yes", GetInnerText(XPath.Content.DidAnotherVehicleHitYourCar), "Did another vehicle hit your car");
                        break;

                    case MotorClaimScenario.WhileParkedSomethingElseHappened:
                        Reporting.AreEqual("Something else happened", GetInnerText(XPath.Content.DidAnotherVehicleHitYourCar), "Did another vehicle hit your car");
                        break;

                    case MotorClaimScenario.WhileStationaryAnotherCarHitRearOfMyCar:
                        Reporting.AreEqual("Another driver hit the rear of my car", GetInnerText(XPath.Content.WhatScenarioDdescribesWhatHappened), "What scenario best describes what happened");
                        break;

                    case MotorClaimScenario.WhileStationaryAnotherCarReversedIntoMyCar:
                        Reporting.AreEqual("Another driver reversed into my car", GetInnerText(XPath.Content.WhatScenarioDdescribesWhatHappened), "What scenario best describes what happened");
                        break;

                    case MotorClaimScenario.WhileStationarySomethingElseHappened:
                        Reporting.AreEqual("Something else happened", GetInnerText(XPath.Content.WhatScenarioDdescribesWhatHappened), "What scenario best describes what happened");
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Format witness details in single string using delimiter as comma(,)
        /// for example, if the contact has firstname, lastname, phone and mobile
        /// then it will returns single string as firstname lastname, phone, email
        /// </summary>      
        private string FormatWitnessDetailsToString(Contact contact)
        {
            string witness = contact.FirstName;

            if (!string.IsNullOrEmpty(contact.Surname))
            {
                witness = $"{witness} {contact.Surname}";
            }

            if (!string.IsNullOrEmpty(contact.MobilePhoneNumber) || !string.IsNullOrEmpty(contact.HomePhoneNumber))
            {
                var contactNumber = string.IsNullOrEmpty(contact.MobilePhoneNumber) ? contact.HomePhoneNumber : contact.MobilePhoneNumber;
                witness = $"{witness}, {DataHelper.FormatPhoneNumber(contactNumber)}";
            }

            if (!string.IsNullOrEmpty(contact.PrivateEmail.Address))
            {
                witness = $"{witness}, {contact.GetEmail()}";
            }

            return witness;
        }

        public void ClickSubmitClaim()
        {
            Reporting.Log("Review your claim - Before clicking Submit Button", _driver.TakeSnapshot());
            ClickControl(XPath.Button.SubmitClaim);
        }

        public List<string> GetPercyIgnoreCSS() =>
        new List<string>()
        {
            "#claimNumberDisplay span"
        };

    }
} 
