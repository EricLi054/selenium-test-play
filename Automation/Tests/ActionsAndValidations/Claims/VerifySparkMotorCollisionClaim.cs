using System;
using System.Linq;
using System.Collections.Generic;
using UIDriver.Pages.Spark.Claim.Motor.Collision;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using UIDriver.Pages.Spark;

namespace Tests.ActionsAndValidations
{
    public static class VerifySparkMotorCollisionClaim
    {
        /// <summary>
        /// Verify motor policy is not active error message on the Start your claim page
        /// </summary>       
        public static void VerifyPolicyNotActiveErrorMessage(Browser browser)
        {
            using (var startYourClaim = new StartYourClaim(browser))
            {
                startYourClaim.VerifyPolicyNotActiveErrorMessage();
            }

        }

        /// <summary>
        /// Verify sorry you can't claim online error message on the Start your claim page
        /// </summary>       
        public static void VerifyCantClaimOnlineErrorMessageOnStartYourClaimPage(Browser browser)
        {
            using (var startYourClaim = new StartYourClaim(browser))
            {
                startYourClaim.VerifyCantClaimOnlineErrorMessage();
            }

        }

        /// <summary>
        /// Verify duplicate Claim error message on the Start your claim page
        /// </summary>       
        public static void VerifyDuplicateClaimErrorMessage(Browser browser, string previousClaimNumber)
        {
            using (var startYourClaim = new StartYourClaim(browser))
            {
                startYourClaim.VerifyDuplicateClaimErrorMessage(previousClaimNumber);
            }

        }

        /// <summary>
        /// Verify similar Claim message on the Start your claim page
        /// </summary>       
        public static void VerifySimilarClaimDialogMessage(Browser browser, string date, string previousClaimNumber)
        {
            using (var startYourClaim = new StartYourClaim(browser))
            {
                startYourClaim.VerifySimilarClaimErrorMessage(date, previousClaimNumber);
            }

        }

        /// <summary>
        /// Verify Can't Claim Online error message on the about the accident page
        /// </summary>       
        public static void VerifyCantClaimOnlineErrorMessageOnAboutTheAccidentPage(Browser browser)
        {
            using (var aboutTheAccident = new AboutTheAccident(browser))
            {
                aboutTheAccident.VerifyCantClaimOnlineErrorMessage();
            }

        }

        /// <summary>
        /// Verify Can't Claim Online error message on the Property or pet owner page
        /// </summary>       
        public static void VerifyCantClaimOnlineErrorMessageOnThirdPartyDetailsPage(Browser browser)
        {
            using (var propertyOrPetOwner = new ThirdPartyDetails(browser))
            {
                propertyOrPetOwner.VerifyCantClaimOnlineErrorMessage();
            }

        }

        /// <summary>
        /// Verify claim schenario, damage type, damage code and policy contatcts with Shield Claim API
        /// Here is the detail confluence page for mapping
        /// https://rac-wa.atlassian.net/wiki/spaces/ISP/pages/3310354690/SPK-5840+Create+claim+scenarios+CMC+Slice+2
        /// </summary>       
        public static void VerifyInitialClaimDetailsInShield(ClaimCar claim)
        {
            Reporting.LogTestShieldValidations("intitial claim details", $"Claim Number: {claim.ClaimNumber}");
            var claimResponse = DataHelper.GetClaimDetails(claim.ClaimNumber);

            Reporting.LogMinorSectionHeading("Verifying clain type, scenario and damage code in Shield");
            if (claim.DamageType == MotorClaimDamageType.SingleVehicleCollision)
            {
                VerifySingleVehicleCollisionDamageCode(claim, claimResponse);
            }
            else if (claim.DamageType == MotorClaimDamageType.MultipleVehicleCollision)
            {
                VerifyMultiVehicleCollisionDamageCode(claim, claimResponse);
            }

            Reporting.LogMinorSectionHeading("Verifying all the contact roles in the claim dependency tree in Shield");
            var actualPolicyHolderName = claimResponse.ClaimContacts.Find(x => x.ClaimContactRole == ClaimContactRole.PolicyHolder).Name.RemoveRoundBrackets();
            var expectedPolicyHolderName = claim.Policy.PolicyHolders.Find(x => x.ContactRoles.FirstOrDefault() == ContactRole.PolicyHolder).GetFullName();
            Reporting.AreEqual(expectedPolicyHolderName, actualPolicyHolderName, ignoreCase: true, "Policy holder details in Shield claim depencency tree");

            if (claim.Policy.PolicyHolders.FindAll(x => x.ContactRoles.FirstOrDefault() == ContactRole.CoPolicyHolder).Count() >= 1)
            {

                List<string> actualCoPolicyOwnersName = new List<string>();
                foreach (var coPolicyOwner in claimResponse.ClaimContacts.FindAll(x => x.ClaimContactRole == ClaimContactRole.PolicyCoOwner))
                {
                    actualCoPolicyOwnersName.Add(coPolicyOwner.Name.RemoveRoundBrackets());
                }

                List<string> expectedCoPolicyOwnersName = new List<string>();
                foreach (var coPolicyOwner in claim.Policy.PolicyHolders.FindAll(x => x.ContactRoles.FirstOrDefault() == ContactRole.CoPolicyHolder))
                {
                    expectedCoPolicyOwnersName.Add(coPolicyOwner.GetFullName());
                }

                Reporting.IsTrue(expectedCoPolicyOwnersName.All(actualCoPolicyOwnersName.Contains) && expectedCoPolicyOwnersName.Count == actualCoPolicyOwnersName.Count, $"CoPolicy owner details in Shield claim depencency tree are matching {string.Join(", ", expectedCoPolicyOwnersName)}");
            }

            var actualInformantName = claimResponse.ClaimContacts.Find(x => x.ClaimContactRole == ClaimContactRole.Informant).Name.RemoveRoundBrackets();
            var expectedInformantName = claim.Claimant.GetFullName();
            Reporting.AreEqual(expectedInformantName, actualInformantName, "Informant details in Shield claim depencency tree");

            Reporting.LogMinorSectionHeading("Verifying the evemt date and time in Shield");
            Reporting.AreEqual(claim.EventDateTime, claimResponse.EventDate, "Event date and time in Shield");
        }


        /// <summary>
        /// Verify claim type, claim scenario and damage code for single vehicle collision in Shield
        /// </summary>  
        private static void VerifySingleVehicleCollisionDamageCode(ClaimCar claim, GetClaimResponse claimResponse)
        {
            var PHClaimContact = claimResponse.ClaimContacts.Find(x => x.ClaimantSide == ClaimContactRole.PolicyHolder);
            Reporting.AreEqual(ShieldClaimType.CollisionSingle.GetDescription(), claimResponse.ClaimType, "Claim type in Shield");

            //If TP property damaged and only claiming for TP damage
            if (claim.IsTPPropertyDamage == true && claim.OnlyClaimDamageToTP)
            {
                Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.LiabilityOnly].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                switch (claim.ClaimScenario)
                {
                    case MotorClaimScenario.AccidentWithSomeoneElseProperty:
                    case MotorClaimScenario.AccidentWithSomethingElse:
                        Reporting.AreEqual(ShieldClaimScenario.SingleCollision.GetDescription(), claimResponse.ClaimScenario, "Claim scenario in Shield");
                        break;
                    case MotorClaimScenario.AccidentWithSomeonesPet:
                        Reporting.AreEqual(ShieldClaimScenario.CollisionWithAnimal.GetDescription(), claimResponse.ClaimScenario, "Claim scenario in Shield");
                        break;
                    default:
                        throw new NotSupportedException($"{claim.ClaimScenario.GetDescription()} is not supported");
                }
            }
            ////If TP property damaged and claiming for all parties
            else if (claim.IsTPPropertyDamage == true && !claim.OnlyClaimDamageToTP)
            {
                switch (claim.ClaimScenario)
                {
                    case MotorClaimScenario.AccidentWithSomeoneElseProperty when (claim.DirectionBeingTravelled == TravelDirection.Forward || claim.DirectionBeingTravelled == TravelDirection.Reversing):
                    case MotorClaimScenario.AccidentWithSomethingElse when (claim.DirectionBeingTravelled == TravelDirection.Forward || claim.DirectionBeingTravelled == TravelDirection.Reversing):
                        Reporting.AreEqual(ShieldClaimScenario.SingleCollision.GetDescription(), claimResponse.ClaimScenario, "Claim scenario in Shield");
                        Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.SingleCollision].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                        claim.ShieldDamageType = ShieldClaimScenario.SingleCollision;
                        break;
                    case MotorClaimScenario.AccidentWithSomeoneElseProperty when (claim.DirectionBeingTravelled == TravelDirection.Parked || claim.DirectionBeingTravelled == TravelDirection.Stationary):
                    case MotorClaimScenario.AccidentWithSomethingElse when (claim.DirectionBeingTravelled == TravelDirection.Parked || claim.DirectionBeingTravelled == TravelDirection.Stationary):
                        Reporting.AreEqual(ShieldClaimScenario.VehicleImpactedbyObject.GetDescription(), claimResponse.ClaimScenario, "Claim scenario in Shield");
                        Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.VehicleImpactedbyObject].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                        claim.ShieldDamageType = ShieldClaimScenario.VehicleImpactedbyObject;
                        break;
                    case MotorClaimScenario.AccidentWithSomeonesPet:
                        Reporting.AreEqual(ShieldClaimScenario.CollisionWithAnimal.GetDescription(), claimResponse.ClaimScenario, "Claim scenario in Shield");
                        Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.CollisionWithAnimal].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                        claim.ShieldDamageType = ShieldClaimScenario.CollisionWithAnimal;
                        break;
                    default:
                        throw new NotSupportedException($"{claim.ClaimScenario.GetDescription()} is not supported");
                }
            }
            //if TP property not damaged
            else
            {
                switch (claim.ClaimScenario)
                {
                    case MotorClaimScenario.AccidentWithYourOwnProperty:
                    case MotorClaimScenario.AccidentWithSomeoneElseProperty when (claim.DirectionBeingTravelled == TravelDirection.Forward || claim.DirectionBeingTravelled == TravelDirection.Reversing):
                    case MotorClaimScenario.AccidentWithSomethingElse when (claim.DirectionBeingTravelled == TravelDirection.Forward || claim.DirectionBeingTravelled == TravelDirection.Reversing):
                        Reporting.AreEqual(ShieldClaimScenario.SingleCollision.GetDescription(), claimResponse.ClaimScenario, "Claim scenario in Shield");
                        Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.SingleCollision].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                        claim.ShieldDamageType = ShieldClaimScenario.SingleCollision;
                        break;
                    case MotorClaimScenario.AccidentWithSomeoneElseProperty when (claim.DirectionBeingTravelled == TravelDirection.Parked || claim.DirectionBeingTravelled == TravelDirection.Stationary):
                    case MotorClaimScenario.AccidentWithSomethingElse when (claim.DirectionBeingTravelled == TravelDirection.Parked || claim.DirectionBeingTravelled == TravelDirection.Stationary):
                        Reporting.AreEqual(ShieldClaimScenario.VehicleImpactedbyObject.GetDescription(), claimResponse.ClaimScenario, "Claim scenario in Shield");
                        Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.VehicleImpactedbyObject].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                        claim.ShieldDamageType = ShieldClaimScenario.VehicleImpactedbyObject;
                        break;
                    case MotorClaimScenario.AccidentWithWildlife:
                    case MotorClaimScenario.AccidentWithSomeonesPet:
                        Reporting.AreEqual(ShieldClaimScenario.CollisionWithAnimal.GetDescription(), claimResponse.ClaimScenario, "Claim scenario in Shield");
                        Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.CollisionWithAnimal].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                        claim.ShieldDamageType = ShieldClaimScenario.CollisionWithAnimal;
                        break;
                    default:
                        throw new NotSupportedException($"{claim.ClaimScenario.GetDescription()} is not supported");
                }
            }
        }

        /// <summary>
        /// Verify claim type, claim scenario and damage code for multi vehicle collision in Shield
        /// </summary>        
        private static void VerifyMultiVehicleCollisionDamageCode(ClaimCar claim, GetClaimResponse claimResponse)
        {
            var PHClaimContact = claimResponse.ClaimContacts.Find(x => x.ClaimantSide == ClaimContactRole.PolicyHolder);

            Reporting.AreEqual(ShieldClaimType.CollisionMulti.GetDescription(), claimResponse.ClaimType, "Claim type in Shield");

            //If cover type is full cover and claiming for all parties
            if (claim.Policy.GetCoverType() == MotorCovers.MFCO && !claim.OnlyClaimDamageToTP)
            {
                Reporting.AreEqual(MotorClaimScenarioNamesInShield[claim.ClaimScenario].Code, claimResponse.ClaimScenario, "Claim scenario in Shield");
                if (claim.ClaimScenario == MotorClaimScenario.WhileParkedAnotherCarHitMyCar)
                {
                    Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.HitWhilstParked].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                    claim.ShieldDamageType = ShieldClaimScenario.HitWhilstParked;
                }
                else
                {
                    Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.MultiCollision].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                    claim.ShieldDamageType = ShieldClaimScenario.MultiCollision;
                }
            }
            //If cover type is full cover and claiming for thirdparty only
            else if (claim.Policy.GetCoverType() == MotorCovers.MFCO && claim.OnlyClaimDamageToTP)
            {
                if (claim.DirectionBeingTravelled == TravelDirection.Parked || claim.DirectionBeingTravelled == TravelDirection.Stationary)
                {
                    Reporting.AreEqual("OtherMultiCollision", claimResponse.ClaimScenario, "Claim scenario in Shield");
                }
                else
                {
                    Reporting.AreEqual(MotorClaimScenarioNamesInShield[claim.ClaimScenario].Code, claimResponse.ClaimScenario, "Claim scenario in Shield");
                }
                Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.LiabilityOnly].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                claim.ShieldDamageType = ShieldClaimScenario.LiabilityOnly;
            }
            //If cover type is thirdparty cover
            else if (claim.Policy.GetCoverType() != MotorCovers.MFCO)
            {
                if (claim.DirectionBeingTravelled == TravelDirection.Parked || claim.DirectionBeingTravelled == TravelDirection.Stationary)
                {
                    Reporting.AreEqual("OtherMultiCollision", claimResponse.ClaimScenario, "Claim scenario in Shield");
                    Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.UninsuredMotoristExtension].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                    claim.ShieldDamageType = ShieldClaimScenario.UninsuredMotoristExtension;                    
                }
                else
                {
                    switch (claim.ClaimScenario)
                    {
                        case MotorClaimScenario.WhileDrivingMyCarHitAnotherCarWhenChangingLanes:
                        case MotorClaimScenario.WhileDrivingMyCarHitRearOfAnotherCar:
                        case MotorClaimScenario.WhileDrivingMyCarHitAParkedCar:
                        case MotorClaimScenario.WhileDrivingMyCarHitAnotherCarFailToGiveWay:
                        case MotorClaimScenario.WhileReversingHitParkedCar:
                        case MotorClaimScenario.WhileReversingHitAnotherCar:
                            Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.LiabilityOnly].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                            claim.ShieldDamageType = ShieldClaimScenario.LiabilityOnly;
                            break;
                        case MotorClaimScenario.WhileDrivingMyCarHitAnotherCarSomethingElseHappened:
                        case MotorClaimScenario.WhileDrivingSomethingElseHappened:
                        case MotorClaimScenario.WhileReversingSomethingElseHappened:
                            Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.UninsuredMotoristExtension].Code, PHClaimContact.DamagedAssets.FirstOrDefault().Damages.FirstOrDefault().DamageCode, "Policy holder claim damage code in Shield");
                            claim.ShieldDamageType = ShieldClaimScenario.UninsuredMotoristExtension;                           
                            break;
                        default:
                            throw new NotSupportedException($"{claim.ClaimScenario.GetDescription()} is not supported");
                    }
                }
            }
            else
            {
                throw new NotSupportedException($"{claim.DamageType} with {claim.ClaimScenario} is not valid claim scenatio for online claim");
            }
        }

        /// <summary>
        /// Confirmation page
        /// </summary>
        public static GetClaimResponse VerifyConfirmationPage(Browser browser, ClaimCar claim)
        {
            using (var confirmation = new Confirmation(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                confirmation.WaitForPage(WaitTimes.T150SEC);
                return confirmation.VerifyConfirmationPage(claim);
            }
        }

        /// <summary>
        /// At the end of a Spark Motor Collision Claim, this method
        /// checks that the confirmation page has successfully displayed
        /// and verifies key claim information in Shield.
        /// </summary>
        public static void VerifyConfirmationPageAndDetailsInShield(Browser browser, ClaimCar claim)
        {
            var claimResponse = VerifyConfirmationPage(browser, claim);
            VerifyClaimDetailsAfterUpdateClaimInShield(claim);
            VerifyClaimDetailsInShield(claim, claimResponse);
        }

        private static void VerifyClaimDetailsAfterUpdateClaimInShield(ClaimCar claim)
        {
            Reporting.LogTestShieldValidations("Updated claim details", $"Claim Number: {claim.ClaimNumber}");

            var claimResponse = DataHelper.GetClaimDetails(claim.ClaimNumber);
            var PHClaimContact = claimResponse.ClaimContacts.Find(x => x.ClaimantSide == ClaimContactRole.PolicyHolder);

            //Check event location
            Reporting.AreEqual(claim.EventLocation, claimResponse.EventLocation.StreetName, "Event location street in Shield");
            Reporting.AreEqual("AUSTRALIA", claimResponse.EventLocation.Country, "Event location country in Shield");

            //Check driver details
            Reporting.AreEqual(claim.Driver.DriverDetails.GetFullName(), claimResponse.ClaimContacts.Find(x => x.ClaimContactRole == ClaimContactRole.Driver).Name.RemoveRoundBrackets(), "Driver name in Shield");
            var driver = PHClaimContact.DamagedAssets.FirstOrDefault().Driver;
            var driverDetails = DataHelper.GetContactDetailsViaExternalContactNumber(driver.ContactExternalNumber);
            Reporting.AreEqual(claim.Driver.DriverDetails.FirstName, driverDetails.FirstName, "Drivers first name in Shield");
            if (claim.Driver.DriverDetails.MiddleName != null)
            {
                Reporting.AreEqual(claim.Driver.DriverDetails.MiddleName, driverDetails.MiddleName, "Drivers middle name in Shield");
            }
            Reporting.AreEqual(claim.Driver.DriverDetails.Surname, driverDetails.Surname, "Drivers surname in Shield");
            Reporting.AreEqual(claim.Driver.DriverDetails.DateOfBirthString, driverDetails.DateOfBirthString, "Drivers date of birth in Shield");

            if (claim.Driver.DriverDetails.MobilePhoneNumber != null)
            {
                if (claim.IsClaimantDriver && claim.IsMobileNumberChanged)
                {
                    Reporting.LogMinorSectionHeading($"We have not updated tests for this claim type to include MFA changes to update this information " +
                        $"via myRAC, so here we confirm that the mobile telephone number has remained the same.");
                    Reporting.AreEqual(claim.Driver.DriverDetails.MobilePhoneNumber, driverDetails.MobilePhoneNumber, 
                        "Drivers mobile number in Shield not expected to change for this claim type");
                }
                else
                {
                    Reporting.AreEqual(claim.Driver.DriverDetails.MobilePhoneNumber, driverDetails.MobilePhoneNumber,
                        "Drivers mobile number in Shield not expected to change for this claim type");
                }
            }

            Reporting.IsTrue(claim.Driver.DriverDetails.MailingAddress.IsEqual(driverDetails.MailingAddress), "Drivers address in Shield");

            //Check driving history
            if (claim.IsQualifiedForDriverHistoryQuestionnaire)
            {
                Reporting.AreEqual(DataHelper.BooleanToStringYesNoAndCustomText(claim.Driver.WasDriverDrunk, Unknown), driver.WasDriverUnderTheInfluence, "Driver was under the influence in Shield");
                Reporting.AreEqual(DataHelper.BooleanToStringYesNoAndCustomText(claim.Driver.WasDriverLicenceSuspended, Unknown), driver.HasLicenceSuspended, "Driving licence is suspended is past in Shield");
            }
            else
            {
                Reporting.IsNull(driver.WasDriverUnderTheInfluence, "Driver was under the influence in Shield");
                Reporting.IsNull(driver.HasLicenceSuspended, "Driving licence is suspended is past in Shield");
            }
            //Check claimant event description
            var claimantEventDescription = $"Event Description: {claim.AccountOfAccident}";
            if (!claim.OnlyClaimDamageToTP)
            {
                claimantEventDescription = $"{claimantEventDescription}Damage: {claim.DamageToPHVehicle}";
            }
            Reporting.AreEqual(claimantEventDescription.StripLineFeedAndCarriageReturns(), PHClaimContact.ClaimantEventDescription.StripLineFeedAndCarriageReturns(), "Claimant event description in Shield");

            string expectedIsVehicleDriveable = null;
            string expectedWasVehicleTowed = null;

            //Check vehicle incident questionire only if the claim type is not liability only claim or uninsured motor extention claim
            if (claim.Policy.GetCoverType() == MotorCovers.MFCO && !claim.OnlyClaimDamageToTP)
            {
                //Check repairer location, Tests are always using suburbs in WA.
                var actualRepairsInWA = claimResponse.GetQuestionnaireLine((int)MotorClaimQuestionnaire.RepairsToCompleteInWA).GetAnswerIDAsMappedString();
                Reporting.AreEqual(Yes, actualRepairsInWA, MotorClaimQuestionnaire.HaveRepairsBeenOrganised.GetDescription());

                //Check vehicle drivable
                var actualIsVehicleDriveable = claimResponse.GetQuestionnaireLine((int)MotorClaimQuestionnaire.IsVehicleDriveable).GetAnswerIDAsMappedString();
                expectedIsVehicleDriveable = claim.IsVehicleDriveable == true ? Yes : No;
                Reporting.AreEqual(expectedIsVehicleDriveable, actualIsVehicleDriveable, MotorClaimQuestionnaire.IsVehicleDriveable.GetDescription());

                //Check vehicle towed
                var actualWasVehicleTowed = claimResponse.GetQuestionnaireLine((int)MotorClaimQuestionnaire.WasVehicleTowed).GetAnswerIDAsMappedString();
                expectedWasVehicleTowed = DataHelper.BooleanToStringYesNoAndCustomText(claim.TowedVehicleDetails.WasVehicleTowed, Unsure);
                Reporting.AreEqual(expectedWasVehicleTowed, actualWasVehicleTowed, MotorClaimQuestionnaire.WasVehicleTowed.GetDescription());

                //Check towing location
                if (claim.TowedVehicleDetails.WasVehicleTowed == true)
                {
                    var actualTowedTo = claimResponse.GetQuestionnaireLine((int)MotorClaimQuestionnaire.TowedToPlaceType).AnswerId;
                    Reporting.AreEqual(SparkMotorTowedToText[claim.TowedVehicleDetails.TowedTo].ShieldAnswerId, actualTowedTo, MotorClaimQuestionnaire.TowedToPlaceType.GetDescription());
                }
            }
            //Check travel direction
            var actualDirectionOfTravel = claimResponse.GetQuestionnaireLine((int)MotorClaimQuestionnaire.DirectionOfTravel).GetAnswerIDAsEnum<TravelDirection>();
            Reporting.AreEqual(claim.DirectionBeingTravelled, actualDirectionOfTravel, MotorClaimQuestionnaire.DirectionOfTravel.GetDescription());


            //Check PH known to TP
            //When TP details is not empty and 
            //it's not an uninsured motor extention claim or vehicle impacted by object claim
            //then Incident Circumstance Questionnaire will be populated
            if (!(claim.ShieldDamageType == ShieldClaimScenario.UninsuredMotoristExtension ||
                claim.ShieldDamageType == ShieldClaimScenario.VehicleImpactedbyObject) &&
                claim.ThirdParty != null && 
                claim.ThirdParty.Count > 0)
            {
                var questionnaireTpKnownTOPH = claim.ClaimScenario == MotorClaimScenario.WhileParkedAnotherCarHitMyCar ?
                    MotorClaimQuestionnaire.ThirdPartyKnownToPHHitWhileParked : MotorClaimQuestionnaire.ThirdPartyKnownToPH;

                var actualTPKnownToPH = claimResponse.GetQuestionnaireLine((int)questionnaireTpKnownTOPH).GetAnswerIDAsMappedString();
                var expectedTPKnownToPH = DataHelper.BooleanToStringYesNo(claim.ThirdParty.First().IsKnownToClaimant);
                Reporting.AreEqual(expectedTPKnownToPH, actualTPKnownToPH, MotorClaimQuestionnaire.ThirdPartyKnownToPH.GetDescription());
            }

            string eventDescription = null;
            var collisionType = claim.DamageType == MotorClaimDamageType.SingleVehicleCollision ? SingleVehicleCollisionEvent : MultiVehicleCollisionEvent;

            //Check the claim type is not liability only claim or uninsured motor extention claim
            if (claim.Policy.GetCoverType() == MotorCovers.MFCO && !claim.OnlyClaimDamageToTP)
            {
                string vehicleLocation = null;
                if (claim.TowedVehicleDetails.WasVehicleTowed == true)
                {
                    if (claim.TowedVehicleDetails.TowedTo == MotorClaimTowedTo.HoldingYard ||
                        claim.TowedVehicleDetails.TowedTo == MotorClaimTowedTo.Repairer)
                    {
                        vehicleLocation = $"{claim.TowedVehicleDetails.TowedTo.GetDescription()} Business Name: {claim.TowedVehicleDetails.BusinessDetails.BusinessName}; Contact Number: {claim.TowedVehicleDetails.BusinessDetails.ContactNumber}; Address or suburb: {claim.TowedVehicleDetails.BusinessDetails.Address}";
                    }
                    else if (claim.TowedVehicleDetails.TowedTo == MotorClaimTowedTo.HomeAddress)
                    {
                        vehicleLocation = "Home address";
                    }
                    else
                    {
                        vehicleLocation = claim.TowedVehicleDetails.CarLocation.StripLineFeedAndCarriageReturns();
                    }
                }
                else if (claim.IsVehicleDriveable != true)
                {
                    vehicleLocation = claim.TowedVehicleDetails.CarLocation.StripLineFeedAndCarriageReturns();
                }

                eventDescription = vehicleLocation == null ? $"{collisionType} Vehicle Driveable: {expectedIsVehicleDriveable} Vehicle Towed: {expectedWasVehicleTowed}" :
                    $"{collisionType} Vehicle Driveable: {expectedIsVehicleDriveable} Vehicle Towed: {expectedWasVehicleTowed} Vehicle Location: {vehicleLocation}";
            }
            else
            {
                eventDescription = collisionType;
            }

            if (claim.Witness != null)
            {
                eventDescription = $"{eventDescription} {claim.Witness.Count()} Witness Details";
            }

            if (claim.ThirdParty != null)
            {
                eventDescription = $"{eventDescription} {claim.ThirdParty.Count()} TP details provided";
            }
            else
            {
                eventDescription = $"{eventDescription} No TP details provided";
            }

            if (!claim.OnlyClaimDamageToTP && 
                !claim.IsRepairerAllocationExhausted &&
                (claim.RepairerOption == RepairerOption.GetQuote))
            {
                eventDescription = $"{eventDescription} Member wants to get their own quote";
            }
            
            Reporting.AreEqual(eventDescription, claimResponse.EventDescription.StripLineFeedAndCarriageReturns(), "Event description in Shield");

            //Verify witness details
            VerifyWitnessDetailsInShield(claim, claimResponse);

            //Verify TP details
            VerifyTPDetailsInShield(claim, claimResponse);

            //Verify the libality percentage
            VerifyLiabilityPercentage(claim, claimResponse);

            //Verify police details
            if (claim.IsPoliceInvolved == true)
            {
                Reporting.AreEqual(true, claimResponse.IsPoliceInvolved, "whether police were involved in the accident");
                if (!string.IsNullOrEmpty(claim.PoliceReportNumber))
                {
                    Reporting.AreEqual(claim.PoliceReportNumber, claimResponse.PoliceReportNumber, "Police report number");
                }
            }
            else
            {
                Reporting.AreEqual(false, claimResponse.IsPoliceInvolved, "whether police were involved in the accident");
            }
        }

        /// <summary>
        /// Verify witness details in Shield
        /// </summary>
        private static void VerifyWitnessDetailsInShield(ClaimCar claim, GetClaimResponse claimResponse)
        {
            if (claim.Witness == null)
            {
                var witness = claimResponse.ClaimContacts.FindAll(x => x.ClaimContactRole == ClaimContactRole.Witness);
                Reporting.IsTrue(witness.Count() == 0, "As expected no witness are added in Shield");
            }
            else
            {
                foreach (var contact in claim.Witness)
                {
                    Reporting.Log($"Verifying witness details in Shield for {contact.GetFullName()}");

                    var claimResponseWitness = claimResponse.ClaimContacts.FirstOrDefault(x => x.ClaimContactRole == ClaimContactRole.Witness && x.Name == $"{contact.FirstName} {contact.Surname}");
                    if (claimResponseWitness != null)
                    {
                        var actualWitnessContact = DataHelper.GetContactDetailsViaExternalContactNumber(claimResponseWitness.ContactExternalNumber);
                        Reporting.AreEqual(contact.FirstName, actualWitnessContact.FirstName, "Witness first name");

                        string expectedWitnessNote = $"First Name: {contact.FirstName}";

                        if (!string.IsNullOrEmpty(contact.Surname))
                        {
                            Reporting.AreEqual(contact.Surname, actualWitnessContact.Surname, "Witness surname");
                            expectedWitnessNote = $"{expectedWitnessNote} Last Name: {contact.Surname}";
                        }

                        if (!string.IsNullOrEmpty(contact.MobilePhoneNumber) || !string.IsNullOrEmpty(contact.HomePhoneNumber))
                        {
                            var contactNumber = string.IsNullOrEmpty(contact.MobilePhoneNumber) ? contact.HomePhoneNumber : contact.MobilePhoneNumber;
                            expectedWitnessNote = $"{expectedWitnessNote} Phone: {contactNumber}";

                            if (!string.IsNullOrEmpty(contact.MobilePhoneNumber))
                            {
                                Reporting.AreEqual(contact.MobilePhoneNumber, actualWitnessContact.MobilePhoneNumber, "Witness phone number");
                            }
                            else
                            {
                                Reporting.AreEqual(contact.HomePhoneNumber, actualWitnessContact.HomePhoneNumber, "Witness phone number");
                            }
                        }

                        if (!string.IsNullOrEmpty(contact.PrivateEmail.Address))
                        {
                            Reporting.AreEqual(contact.GetEmail(), actualWitnessContact.GetEmail(), ignoreCase: true, "Witness email address");
                            expectedWitnessNote = $"{expectedWitnessNote} Email: {contact.GetEmail()}";
                        }

                        Reporting.AreEqual(expectedWitnessNote, claimResponseWitness.WitnessNote.StripLineFeedAndCarriageReturns(), "Witness note in Shield");
                        Reporting.AreEqual("NOT AVAILABLE", claimResponseWitness.WitnessLocationType, "Witness location type in Shield");
                        Reporting.AreEqual("1901-01-01", actualWitnessContact.DateOfBirthString, "Witness date of birth");
                        Reporting.AreEqual(false, actualWitnessContact.IsCrmPreferred, "Witness IsCrmPreferred");
                        Reporting.AreEqual("Witness", actualWitnessContact.Roles.FirstOrDefault().ExternalCode, "Witness contact role");
                    }
                }
            }
        }

        /// <summary>
        /// Verify third party details in Shield
        /// </summary>
        private static void VerifyTPDetailsInShield(ClaimCar claim, GetClaimResponse claimResponse)
        {
            var claimResponseTP = claimResponse.ClaimContacts.FirstOrDefault(x => x.ClaimantSide == ClaimContactRole.ThirdParty);

            if (claim.ThirdParty == null)
            {
                Reporting.IsTrue(claimResponseTP == null, "As expected no third party details are added in Shield");
            }
            else
            {
                var thirdParty = claim.ThirdParty.First();
                Reporting.Log($"Verifying third party details in Shield for {thirdParty.GetFullName()}");
              
                if (claimResponseTP != null)
                {
                    if (claim.DamageType == MotorClaimDamageType.MultipleVehicleCollision)
                    {
                        Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.CollisionWithAnotherVehicle].Code, claimResponseTP.DamagedAssets.First().Damages.First().DamageCode, "Third party damage code");
                    }
                    else
                    {
                        if (claim.ClaimScenario == MotorClaimScenario.AccidentWithSomeoneElseProperty ||
                        claim.ClaimScenario == MotorClaimScenario.AccidentWithSomethingElse)
                        {
                            Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.CollisionWithProperty].Code, claimResponseTP.DamagedAssets.First().Damages.First().DamageCode, "Third party damage code");
                        }
                        else if (claim.ClaimScenario == MotorClaimScenario.AccidentWithSomeoneElseProperty)
                        {
                            Reporting.AreEqual(ShieldAPIDamageCode[ShieldClaimScenario.TPOtherRecovery].Code, claimResponseTP.DamagedAssets.First().Damages.First().DamageCode, "Third party damage code");
                        }
                    }

                    var actualTPContact = DataHelper.GetContactDetailsViaExternalContactNumber(claimResponseTP.ContactExternalNumber);
                    string expectedTPEventDescription = null;

                    if ((claim.DamageType == MotorClaimDamageType.MultipleVehicleCollision) &&
                        (!string.IsNullOrEmpty(thirdParty.Rego)))
                    {
                        expectedTPEventDescription = $"Car Rego: {thirdParty.Rego}";
                    }

                    if (!string.IsNullOrEmpty(thirdParty.FirstName))
                    {
                        Reporting.AreEqual(thirdParty.FirstName, actualTPContact.FirstName, "Third party first name");
                        expectedTPEventDescription = $"{expectedTPEventDescription} First Name: {thirdParty.FirstName}";
                    }
                    else
                    {
                        Reporting.AreEqual(".", actualTPContact.FirstName, "Third party first name should be '.' as first name is null");
                    }

                    if (!string.IsNullOrEmpty(thirdParty.Surname))
                    {
                        Reporting.AreEqual(thirdParty.Surname, actualTPContact.Surname, "Third party surname");
                        expectedTPEventDescription = $"{expectedTPEventDescription} Last Name: {thirdParty.Surname}";
                    }
                    else
                    {
                        Reporting.AreEqual(".", actualTPContact.Surname, "Third party last name should be '.' as last name is null");
                    }


                    if (!string.IsNullOrEmpty(thirdParty.MobilePhoneNumber) || !string.IsNullOrEmpty(thirdParty.HomePhoneNumber))
                    {
                        var contactNumber = string.IsNullOrEmpty(thirdParty.MobilePhoneNumber) ? thirdParty.HomePhoneNumber : thirdParty.MobilePhoneNumber;
                        expectedTPEventDescription = $"{expectedTPEventDescription} Phone: {contactNumber}";

                        if (!string.IsNullOrEmpty(thirdParty.MobilePhoneNumber))
                        {
                            Reporting.AreEqual(thirdParty.MobilePhoneNumber, actualTPContact.MobilePhoneNumber, "Third party phone number");
                        }
                        else
                        {
                            Reporting.AreEqual(thirdParty.HomePhoneNumber, actualTPContact.HomePhoneNumber, "Third party phone number");
                        }
                    }

                    if (!string.IsNullOrEmpty(thirdParty.GetEmail()))
                    {
                        Reporting.AreEqual(thirdParty.GetEmail(), actualTPContact.GetEmail(), ignoreCase: true, "Third party email address");
                        expectedTPEventDescription = $"{expectedTPEventDescription} Email: {thirdParty.GetEmail()}";
                    }

                    if (thirdParty.MailingAddress != null)
                    {
                        Reporting.AreEqual(thirdParty.MailingAddress.StreetSuburbState(), thirdParty.MailingAddress.StreetSuburbState(), $"Third party address {thirdParty.MailingAddress.PCMFormattedAddressString()}");
                        expectedTPEventDescription = $"{expectedTPEventDescription} Address: {thirdParty.MailingAddress.StreetSuburbStatePostcode(false)}";
                    }

                    if (thirdParty.IsKnownToClaimant)
                    {
                        expectedTPEventDescription = $"{expectedTPEventDescription} TP known to PH";
                    }

                    Reporting.AreEqual("1901-01-01", actualTPContact.DateOfBirthString, "Third party date of birth");
                    Reporting.AreEqual(false, actualTPContact.IsCrmPreferred, "Third party IsCrmPreferred");
                    Reporting.AreEqual(false, actualTPContact.IsCrmManaged, "Third party IsCrmManaged");

                    if (claim.DamageType == MotorClaimDamageType.MultipleVehicleCollision)
                    {
                        if (thirdParty.WasDriverTheOwner)
                        {
                            expectedTPEventDescription = $"{expectedTPEventDescription} Driver is the owner of car";
                        }
                        else if (!string.IsNullOrEmpty(thirdParty.AdditionalInfo))
                        {
                            expectedTPEventDescription = $"{expectedTPEventDescription} Name and contact details of owner: {thirdParty.AdditionalInfo}";
                        }
                    }

                    if (thirdParty.Insurer.Name != null)
                    {    
                        Reporting.AreEqual(thirdParty.Insurer.ExternalContactNumber.ToString(), claimResponseTP.DamagedAssets.FirstOrDefault().ClaimantInsurer, "Third party insurance company external contact ID");
                        expectedTPEventDescription = $"{expectedTPEventDescription} Insurer: {thirdParty.Insurer.Name}";
                    }

                    if (thirdParty.ClaimNumber != null)
                    {
                        Reporting.AreEqual(thirdParty.ClaimNumber, claimResponseTP.DamagedAssets.FirstOrDefault().ClaimantClaimNumber, "Third party claim number");
                        expectedTPEventDescription = $"{expectedTPEventDescription} Claim Number: {thirdParty.ClaimNumber}";
                    }

                    if (thirdParty.DescriptionOfDamageToVehicle != null)
                    {
                        expectedTPEventDescription = $"{expectedTPEventDescription} Damage: {thirdParty.DescriptionOfDamageToVehicle}";
                    }                   
                    
                    Reporting.AreEqual(expectedTPEventDescription.Trim().StripLineFeedAndCarriageReturns(), claimResponseTP.ClaimantEventDescription.StripLineFeedAndCarriageReturns(), ignoreCase: true, "Third party event description in Shield");
                }
            }
        }

        /// <summary>
        /// Verify the claim agenda steps in Shield, also
        /// verify the choosen repairer is added in Shield, when user selects the repairer option
        /// and verify referral email is sent to claim assist when complete lodgement failed in Shield
        /// </summary>       
        private static void VerifyClaimDetailsInShield(ClaimCar claim, GetClaimResponse claimResponse)
        {
            Reporting.LogMinorSectionHeading("Verify claim agenda steps, repairer questions and referral email in Shield");
            VerifyMotorClaimAgendaStatus(claim);

            if (!claim.OnlyClaimDamageToTP &&
                !claim.IsRepairerAllocationExhausted &&
                (claim.RepairerOption == RepairerOption.First || claim.RepairerOption == RepairerOption.Second))
            {
                Reporting.AreEqual(claim.AssignedRepairer.BusinessName, claimResponse.ServiceProviderName(), "Assigned repairer name in Shield");
            }

            if (!claim.ExpectCompleteLodgementDone)
            {
                VerifyMotorCollisionReferralEmail(claim.ClaimNumber);
            }

            if (!claim.OnlyClaimDamageToTP &&
                !claim.IsRepairerAllocationExhausted &&
                (claim.RepairerOption == RepairerOption.GetQuote))
            {
                var actualIsMemberGettingOwnQuote = claimResponse.GetQuestionnaireLine((int)MotorClaimQuestionnaire.IsMemberGettingOwnQuote).GetAnswerIDAsMappedString();
                Reporting.AreEqual("Yes", actualIsMemberGettingOwnQuote, MotorClaimQuestionnaire.HaveRepairsBeenOrganised.GetDescription());
            }
        }

        /// <summary>
        /// Verify the expected the referral email is triggered when complete lodgement fail
        /// </summary>
        public static void VerifyMotorCollisionReferralEmail(string claimNumber)
        {
            var emailText = "*Damage Type:* Motor Collisions*Instruction:* Please process online claim as per current business rules";
            var _messageHandler = new MailosaurEmailHandler();
            var email = System.Threading.Tasks.Task.Run(() => _messageHandler.FindEmailBySubject($"New Claim {claimNumber}", retrySeconds: WaitTimes.T150SEC)).GetAwaiter().GetResult();

            if (email != null)
            {
                Reporting.AreEqual(emailText, email.Text.Body.StripLineFeedAndCarriageReturns(false), "Claim assist referal email for motor collision claim");
            }
        }

        /// <summary>
        /// Verifies the 4 agenda steps in Shield for a new spark single vehicle collision motor claim.
        /// </summary>        
        public static void VerifyMotorClaimAgendaStatus(ClaimCar claim)
        {
            bool isRepairerAutoAuthorised = false;
            if (!string.IsNullOrEmpty(claim.AssignedRepairer.BusinessName))
            {
                var repairerProfile = ShieldMotorClaimDB.GetMotorRepairerSkillsViaClaim(claim.ClaimNumber);
                // in Shield, however our claims rules will still automatically authorise those quotes.         
                isRepairerAutoAuthorised = repairerProfile != null ? repairerProfile.IsAutoAuthoriseRepairer : false;
            }
            Reporting.Log($"Verify Agenda Steps for Claim: {claim.ClaimNumber}");

            var agendaStatus = ShieldMotorClaimDB.GetClaimAgendaStatus(claim.ClaimNumber, AgendaStepNames.ClaimLodged);
            Reporting.AreEqual(AgendaStepNames.ClaimLodged.GetDescription(), agendaStatus.Step, "Agenda step name");
            if (claim.ExpectCompleteLodgementDone)
            {
                Reporting.AreEqual(AgendaStepStatus.Done.GetDescription(), agendaStatus.Status, "Claim Lodged Agenda Step status");
            }
            else
            {
                Reporting.AreEqual(AgendaStepStatus.Current.GetDescription(), agendaStatus.Status, "Claim Lodged Agenda Step status");
            }

            //If the claim is for liability only then MotorQuoteAuthorised and InvoiceReceived agenda steps are not applicable
            if (!claim.OnlyClaimDamageToTP)
            {
                agendaStatus = ShieldMotorClaimDB.GetClaimAgendaStatus(claim.ClaimNumber, AgendaStepNames.MotorQuoteAuthorised);
                Reporting.AreEqual(AgendaStepNames.MotorQuoteAuthorised.GetDescription(), agendaStatus.Step, "Agenda step name");
                // Auto authorised repairers will skip the Authorised step(as Not Applicable)
                if (isRepairerAutoAuthorised && claim.ExpectCompleteLodgementDone)
                {
                    Reporting.AreEqual(AgendaStepStatus.NotApplicable.GetDescription(), agendaStatus.Status, "Quote Authorised Agenda Step status");
                }
                else if (claim.ExpectCompleteLodgementDone)
                {
                    Reporting.AreEqual(AgendaStepStatus.Current.GetDescription(), agendaStatus.Status, "Quote Authorised Agenda Step status");
                }
                else
                {
                    Reporting.AreEqual(AgendaStepStatus.Pending.GetDescription(), agendaStatus.Status, "Quote Authorised Agenda Step status");
                }

                agendaStatus = ShieldMotorClaimDB.GetClaimAgendaStatus(claim.ClaimNumber, AgendaStepNames.InvoiceReceived);
                Reporting.AreEqual("Motor - Invoice Received or Claim Paid Out", agendaStatus.Step, "Agenda step name");
                if (isRepairerAutoAuthorised && claim.ExpectCompleteLodgementDone)
                {
                    Reporting.AreEqual(AgendaStepStatus.Current.GetDescription(), agendaStatus.Status, "Invoice Received Agenda Step status");
                }
                else
                {
                    Reporting.AreEqual(AgendaStepStatus.Pending.GetDescription(), agendaStatus.Status, "Invoice Received Agenda Step status");
                }
            }

            agendaStatus = ShieldMotorClaimDB.GetClaimAgendaStatus(claim.ClaimNumber, AgendaStepNames.PaymentsSettled);
            Reporting.AreEqual(AgendaStepNames.PaymentsSettled.GetDescription(), agendaStatus.Step, "Agenda step name");
            if (claim.OnlyClaimDamageToTP && claim.ExpectCompleteLodgementDone)
            {
                Reporting.AreEqual(AgendaStepStatus.Current.GetDescription(), agendaStatus.Status, "Payments Settled Agenda Step status");
            }
            else
            {
                Reporting.AreEqual(AgendaStepStatus.Pending.GetDescription(), agendaStatus.Status, "Payments Settled Agenda Step status");
            }
        }

        /// <summary>
        /// Verifies the libality percentage for both policy holder and third party
        /// </summary>        
        public static void VerifyLiabilityPercentage(ClaimCar claim, GetClaimResponse claimResponse)
        {
            var claimResponsePH = claimResponse.ClaimContacts.Find(x => x.ClaimantSide == ClaimContactRole.PolicyHolder);
            var claimResponseTP = claimResponse.ClaimContacts.FirstOrDefault(x => x.ClaimantSide == ClaimContactRole.ThirdParty);

            if (claim.OnlyClaimDamageToTP && claim.ShieldDamageType != ShieldClaimScenario.UninsuredMotoristExtension)
            {
                Reporting.AreEqual(LiabilityWithOutPercent.Percent100.GetDescription(), claimResponsePH.ClaimantLiability, "Policyholder liability");
                Reporting.AreEqual(LiabilityWithOutPercent.Percent0.GetDescription(), claimResponseTP.ClaimantLiability, "Third party liability");
            }
            else
            {
                switch (claim.DamageType)
                {
                    case MotorClaimDamageType.SingleVehicleCollision:
                        Reporting.AreEqual(LiabilityWithOutPercent.Percent100.GetDescription(), claimResponsePH.ClaimantLiability, "Policyholder liability");
                        if (claim.ThirdParty != null && claim.ThirdParty.Count() > 0)
                        {
                            Reporting.AreEqual(LiabilityWithOutPercent.Percent0.GetDescription(), claimResponseTP.ClaimantLiability, "Third party liability");
                        }
                        break;

                    case MotorClaimDamageType.MultipleVehicleCollision:
                        
                        var scenario = claim.ClaimScenario;

                        if (scenario == MotorClaimScenario.WhileDrivingMyCarHitAParkedCar ||
                            scenario == MotorClaimScenario.WhileReversingHitParkedCar ||
                            scenario == MotorClaimScenario.WhileDrivingMyCarHitRearOfAnotherCar)
                        {
                            if (claim.ThirdParty != null && claim.ThirdParty.Count() > 0)
                            {
                                Reporting.AreEqual(LiabilityWithOutPercent.Percent100.GetDescription(), claimResponsePH.ClaimantLiability, "Policyholder liability");
                                Reporting.AreEqual(LiabilityWithOutPercent.Percent0.GetDescription(), claimResponseTP.ClaimantLiability, "Third party liability");
                            }
                            else
                            {
                                Reporting.AreEqual(LiabilityWithOutPercent.Percent100.GetDescription(), claimResponsePH.ClaimantLiability, "Policyholder liability");                                
                            }
                        }
                        else if (scenario == MotorClaimScenario.WhileParkedAnotherCarHitMyCar ||
                                scenario == MotorClaimScenario.WhileDrivingOtherVehicleHitRearOfMyCar ||
                                scenario == MotorClaimScenario.WhileReversingHitByAnotherCar ||
                                scenario == MotorClaimScenario.WhileStationaryAnotherCarHitRearOfMyCar)
                        {
                            if (claim.ThirdParty != null && claim.ThirdParty.Count() > 0)
                            {
                                Reporting.AreEqual(LiabilityWithOutPercent.Percent0.GetDescription(), claimResponsePH.ClaimantLiability, "Policyholder liability");
                                Reporting.AreEqual(LiabilityWithOutPercent.Percent100.GetDescription(), claimResponseTP.ClaimantLiability, "Third party liability");
                            }
                            else
                            {
                                Reporting.AreEqual(LiabilityWithOutPercent.Percent100.GetDescription(), claimResponsePH.ClaimantLiability, "Policyholder liability");                                
                            }
                        }
                        else
                        {
                            Reporting.AreEqual(LiabilityWithOutPercent.Unknown.GetDescription(), claimResponsePH.ClaimantLiability, ignoreCase: true, "Policyholder liability");
                            if (claim.ThirdParty != null && claim.ThirdParty.Count() > 0)
                            {
                                Reporting.AreEqual(LiabilityWithOutPercent.Unknown.GetDescription(), claimResponseTP.ClaimantLiability, ignoreCase: true, "Third party liability");
                            }                                
                        }
                        break;
                    default:
                        throw new NotSupportedException($"{claim.DamageType.GetDescription()} is not supported");
                }
            }
        }

    }
}

