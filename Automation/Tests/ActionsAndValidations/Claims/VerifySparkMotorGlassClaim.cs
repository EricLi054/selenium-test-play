using System;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.General;

namespace Tests.ActionsAndValidations
{
    public static class VerifySparkMotorGlassClaim
    {

        private static readonly string emailText = "*Damage Type:* Motor Glass Damage*Instruction:*Please process online claims as per current business rules";

        /// <summary>
        /// Verify claim details in Shield 
        /// </summary>
        public static void VerifyMotorGlassClaimDetailsInShield(ClaimCar claim)
        {
            if (!Config.Get().IsVisualTestingEnabled || !Config.Get().IsCrossBrowserDeviceTestingEnabled)
            {
                VerifyIncidentQuestionnairesInShield(claim);
                VerifyUpdatedContactDetailsInShield(claim);
                VerifyMotorGlassClaimAgendaStatus(claim);
                VerifyMotorGlassClaimEventDetails(claim);
                VerifyMotorGlassClaimScenarioDetails(claim);
            }
            
        }

        /// <summary>
        /// Validate fence incident circumstances questionnaires in Shield
        /// using the Get Claim API
        /// </summary>
        private static void VerifyIncidentQuestionnairesInShield(ClaimCar claim)
        {
            Reporting.LogMinorSectionHeading("Verifying incident circumstances questionnaires in Shield");

            var shieldClaimRecord = DataHelper.GetClaimDetails(claim.ClaimNumber);

            var expectedRepairOrganisedOrCompleted = No;
            var expectedWindscreenOnly = No;
            var expectedMemberNeedsToProvideEFT = No;

            switch (claim.ClaimScenario)
            {
                case MotorClaimScenario.GlassDamageAlreadyFixed:
                case MotorClaimScenario.GlassDamageRepairsBooked:
                    expectedRepairOrganisedOrCompleted = Yes;
                    expectedMemberNeedsToProvideEFT = Yes;
                    break;
                case MotorClaimScenario.GlassDamageNotFixed when claim.GlassDamageDetails.FrontWindscreenOnly == true && claim.GlassDamageDetails.OtherWindowGlass == true:
                case MotorClaimScenario.GlassDamageNotFixed when claim.GlassDamageDetails.OtherWindowGlass == true:
               
                    break;
                case MotorClaimScenario.GlassDamageNotFixed when claim.GlassDamageDetails.FrontWindscreenOnly == true:
                    expectedWindscreenOnly = Yes;                    
                    break;
                default:
                    throw new NotImplementedException($"{claim.ClaimScenario} not valid for motor glass claim");                    
            }

            var actualRepairOrganisedOrCompleted = shieldClaimRecord.GetQuestionnaireLine((int)MotorClaimQuestionnaire.HaveRepairsBeenOrganised).GetAnswerIDAsMappedString();
            var actualWindscreenOnly = shieldClaimRecord.GetQuestionnaireLine((int)MotorClaimQuestionnaire.GlassIsFrontWindscreenOnly).GetAnswerIDAsMappedString();
            var actualMemberNeedsToProvideEFT = shieldClaimRecord.GetQuestionnaireLine((int)MotorClaimQuestionnaire.MemberNeedsToProvideEFT).GetAnswerIDAsMappedString();

            Reporting.AreEqual(expectedMemberNeedsToProvideEFT, actualMemberNeedsToProvideEFT, MotorClaimQuestionnaire.MemberNeedsToProvideEFT.GetDescription());
            Reporting.AreEqual(expectedRepairOrganisedOrCompleted, actualRepairOrganisedOrCompleted, MotorClaimQuestionnaire.HaveRepairsBeenOrganised.GetDescription());            
            Reporting.AreEqual(expectedWindscreenOnly, actualWindscreenOnly, MotorClaimQuestionnaire.GlassIsFrontWindscreenOnly.GetDescription());
        }

        /// <summary>
        /// If member provided new mobile number or email
        /// Verify new details showing in Shield
        /// </summary>
        /// <param name="claim"></param>
        private static void VerifyUpdatedContactDetailsInShield(ClaimCar claim)
        {
            Reporting.LogMinorSectionHeading($"Verify member details details for Claim: {claim.ClaimNumber}");

            var contactDetails = DataHelper.GetContactDetailsViaContactId(claim.Claimant.Id);

            Reporting.AreEqual(claim.Claimant.DateOfBirth.Date, contactDetails.DateOfBirth.Date, "Date Of Birth in Shield");
            Reporting.AreEqual(claim.Claimant.Title, contactDetails.Title, "Title in Shield");
            Reporting.AreEqual(claim.Claimant.FirstName, contactDetails.FirstName, ignoreCase: true, "First Name in Shield (NOT CASE SENSITIVE)");
            Reporting.AreEqual(claim.Claimant.Surname, contactDetails.Surname, ignoreCase: true, "Last Name in Shield (NOT CASE SENSITIVE)");            

            if (claim.Claimant.MailingAddress != null &&
                claim.Claimant.MailingAddress.StreetNumber != null)
            {
                Reporting.IsTrue(claim.Claimant.MailingAddress.IsEqualIgnorePostcode(contactDetails.MailingAddress), $"Mailing address of policy holder ({claim.Claimant.MailingAddress.QASStreetAddress()}) should equal {contactDetails.MailingAddress.QASStreetAddress()}");
                Reporting.AreEqual(claim.Claimant.MailingAddress.State, contactDetails.MailingAddress.State, "Mailing Address:State in Shield");
                Reporting.AreEqual(claim.Claimant.MailingAddress.PostCode, contactDetails.MailingAddress.PostCode, "Mailing Address:HouseNumber in Shield");
            }
            else
            {
                Reporting.AreEqual(claim.Claimant.MailingAddress.Country, contactDetails.MailingAddress.Country, true);
                Reporting.AreEqual(claim.Claimant.MailingAddress.State, contactDetails.MailingAddress.State, true);
            }            

            if (claim.IsEmailAddressChanged)
            {
                Reporting.LogMinorSectionHeading($"We have not updated tests for this claim type to include MFA changes to update this information " +
                    $"via myRAC, so here we confirm that the mobile telephone number has remained the same.");
                Reporting.AreEqual(claim.Claimant.PrivateEmail.Address, contactDetails.PrivateEmail.Address, ignoreCase: true, 
                    "Claimant email address in Shield not expected to change for this claim type");
            }
            else
            {
                Reporting.AreEqual(claim.Claimant.PrivateEmail.Address, contactDetails.PrivateEmail.Address, ignoreCase: true,
                    "Claimant email address in Shield not expected to change for this claim type");
            }

            if (claim.IsMobileNumberChanged)
            {
                Reporting.LogMinorSectionHeading($"We have not updated tests for this claim type to include MFA changes to update this information " +
                       $"via myRAC, so here we confirm that the mobile telephone number has remained the same.");
                Reporting.AreEqual(claim.Claimant.MobilePhoneNumber, contactDetails.MobilePhoneNumber,
                        "Claimant mobile number in Shield not expected to change for this claim type");
            }
            else
            {
                Reporting.AreEqual(claim.Claimant.MobilePhoneNumber, contactDetails.MobilePhoneNumber,
                    "Claimant mobile number in Shield not expected to change for this claim type");
            }
        }


        /// <summary>
        /// If member provided new mobile number or email
        /// Verify new details showing in the member central
        /// </summary>
        /// <param name="claim"></param>
        public static void VerifyUpdatedContactDetailsInMemberCentral(ClaimCar claim)
        {
            API_MemberCentralPersonV2Response response;

            if (Config.Get().IsMCMockEnabled())
            {
                response = DataHelper.GetPersonFromMemberCentralByContactId(claim.Claimant.Id);
            }
            else
            {
                response = MemberCentral.GetInstance().GET_PersonByPersonId(claim.Claimant.PersonId).GetAwaiter().GetResult();
            }

            Reporting.AreEqual(claim.Claimant.DateOfBirth.ToString("yyyy-MM-dd"), response.DateOfBirth, "Date Of Birth in MC");            
            Reporting.AreEqual(claim.Claimant.Title.GetDescription(), response.Title, ignoreCase: true, "Title in MC");
            Reporting.AreEqual(claim.Claimant.FirstName, response.FirstName, ignoreCase: true, "First Name in MC (NOT CASE SENSITIVE)");
            Reporting.AreEqual(claim.Claimant.Surname, response.Surname, ignoreCase: true, "Last Name in MC (NOT CASE SENSITIVE)");           

            if (claim.Claimant.MailingAddress != null &&
                claim.Claimant.MailingAddress.StreetNumber != null)
            {
                Reporting.AreEqual(claim.Claimant.MailingAddress.QASStreetNameOnly(), response.PostalAddress.StreetName, ignoreCase: true, $"Mailing address of policy holder ({claim.Claimant.MailingAddress.QASStreetNameOnly()}) should equal {response.PostalAddress.StreetName}");
                Reporting.AreEqual(claim.Claimant.MailingAddress.State, response.PostalAddress.State, "Mailing Address:State in MC");
                Reporting.AreEqual(claim.Claimant.MailingAddress.PostCode, response.PostalAddress.Postcode, "Mailing Address:HouseNumber in MC");
            }
            else
            {
                Reporting.AreEqual(claim.Claimant.MailingAddress.Country, response.PostalAddress.Country, true);
                Reporting.AreEqual(claim.Claimant.MailingAddress.State, response.PostalAddress.State, true);
            }            

            if (claim.IsEmailAddressChanged)
            {
                Reporting.LogMinorSectionHeading($"We have not updated tests for this claim type to include MFA changes to update this information " +
                    $"via myRAC, so here we confirm that the mobile telephone number has remained the same.");
                Reporting.AreEqual(claim.Claimant.PrivateEmail.Address, response.PersonalEmailAddress, ignoreCase: true,
                    "Claimant email address in Shield not expected to change for this claim type");
            }
            else
            {
                Reporting.AreEqual(claim.Claimant.PrivateEmail.Address, response.PersonalEmailAddress, ignoreCase: true,
                    "Claimant email address in Shield not expected to change for this claim type");
            }

            if (claim.IsMobileNumberChanged)
            {
                Reporting.LogMinorSectionHeading($"We have not updated tests for this claim type to include MFA changes to update this information " +
                       $"via myRAC, so here we confirm that the mobile telephone number has remained the same.");
                Reporting.AreEqual(claim.Claimant.MobilePhoneNumber, response.MobilePhone,
                        "Claimant mobile number in Shield not expected to change for this claim type");
            }
            else
            {
                Reporting.AreEqual(claim.Claimant.MobilePhoneNumber, response.MobilePhone,
                        "Claimant mobile number in Shield not expected to change for this claim type");
            }

        }

        /// <summary>
        /// Verify motor glass claim agenda status
        /// </summary>
        /// <param name="claim"></param>
        private static void VerifyMotorGlassClaimAgendaStatus(ClaimCar claim)
        {
            Reporting.LogMinorSectionHeading($"Verify Agenda Steps for Claim: {claim.ClaimNumber}");

            Reporting.Log("================================================================");
            Reporting.Log($"Verify Agenda Step Claim Lodged for Claim");
            var agendaStatus = ShieldMotorClaimDB.GetClaimAgendaStatus(claim.ClaimNumber, AgendaStepNames.ClaimLodged);
            var expectedDamageCode = MotorClaimDamageCodeAndScenarioNames[claim.DamageType].TextShield;

            Reporting.AreEqual(expectedDamageCode, agendaStatus.DamageCode, "expected damage code");
            Reporting.AreEqual(AgendaStepNames.ClaimLodged.GetDescription(), agendaStatus.Step, "expected agenda step name");
            if (!claim.expectClaimPaymentBlock)
            {
                Reporting.AreEqual(AgendaStepStatus.Done.GetDescription(), agendaStatus.Status, "expected status of Agenda step");
            }
            else
            {
                Reporting.AreEqual(AgendaStepStatus.Current.GetDescription(), agendaStatus.Status, "expected status of Agenda step");
            }
                    

            Reporting.Log("================================================================");
            Reporting.Log($"Verify Agenda Step Invoice Received for Claim");
            agendaStatus = ShieldMotorClaimDB.GetClaimAgendaStatus(claim.ClaimNumber, AgendaStepNames.InvoiceReceived);

            Reporting.AreEqual(expectedDamageCode, agendaStatus.DamageCode, "expected damage code");
            Reporting.AreEqual("Motor - Invoice Received or Claim Paid Out", agendaStatus.Step, "expected agenda step name");
            if (!claim.expectClaimPaymentBlock)
            {
                Reporting.AreEqual(AgendaStepStatus.Current.GetDescription(), agendaStatus.Status, "expected status of Agenda step");
            }
            else
            {
                Reporting.AreEqual(AgendaStepStatus.Pending.GetDescription(), agendaStatus.Status, "expected status of Agenda step");
            }
                      

            Reporting.Log("================================================================");
            Reporting.Log($"Verify Agenda Step Payments Settled for Claim");
            agendaStatus = ShieldMotorClaimDB.GetClaimAgendaStatus(claim.ClaimNumber, AgendaStepNames.PaymentsSettled);

            Reporting.AreEqual(expectedDamageCode, agendaStatus.DamageCode, "expected damage code");
            Reporting.AreEqual(AgendaStepNames.PaymentsSettled.GetDescription(), agendaStatus.Step, "expected name of Agenda step");
            Reporting.AreEqual(AgendaStepStatus.Pending.GetDescription(), agendaStatus.Status, "expected status of Agenda step");
        }

        /// <summary>
        /// Verify shield events for motor glass claim
        /// </summary>
        /// <param name="claim"></param>
        private static void VerifyMotorGlassClaimEventDetails(ClaimCar claim)
        {
            Reporting.LogMinorSectionHeading($"Verify Event Details for Claim: {claim.ClaimNumber}");
            var eventDetails = ShieldClaimDB.GetClaimEvents(claim.ClaimNumber);

            Reporting.IsTrue(eventDetails.Contains(ShieldEvent.CreateClaimantAssetAction.GetDescription()), $"Shield event displayed {ShieldEvent.CreateClaimantAssetAction}");
            Reporting.IsTrue(eventDetails.Contains(ShieldEvent.NewDamagesCreated.GetDescription()), $"Shield event displayed {ShieldEvent.NewDamagesCreated}");            
            Reporting.IsTrue(eventDetails.Contains(ShieldEvent.OnlineClaimOpened.GetDescription()), $"Shield event displayed {ShieldEvent.OnlineClaimOpened}");
            Reporting.IsTrue(eventDetails.Contains(ShieldEvent.IncompleteOnlineClaimFollowUpClosed.GetDescription()), $"Shield event displayed {ShieldEvent.IncompleteOnlineClaimFollowUpClosed}");

            if (claim.ClaimScenario == MotorClaimScenario.GlassDamageNotFixed && !claim.expectClaimPaymentBlock)
            {
                var claimDetails = DataHelper.GetClaimDetails(claim.ClaimNumber);
                Reporting.Log($"Assigned Service Provider is '{claimDetails.ServiceProviderName()}'.");
                //For O'Brien service provider the Refer Claimant Asset To Service Provider event will not be generated
                if (!claimDetails.ServiceProviderName().Contains(MotorGlassRepairer.OBrien.GetDescription()))
                {
                    Reporting.IsTrue(eventDetails.Contains(ShieldEvent.ReferClaimantAssetToServiceProvider.GetDescription()), $"Shield event displayed {ShieldEvent.ReferClaimantAssetToServiceProvider}");
                }
                else
                {
                    Reporting.Log($"No 'ReferClaimantAssetToServiceProvider' event is expected when Assigned Service Provider is '{claimDetails.ServiceProviderName()}'");
                    Reporting.IsFalse(eventDetails.Contains(ShieldEvent.ReferClaimantAssetToServiceProvider.GetDescription()), $"{ShieldEvent.ReferClaimantAssetToServiceProvider} Shield event should NOT be displayed ");
                }
                    Reporting.IsTrue(eventDetails.Contains(ShieldEvent.GlazierAutomaticallyAllocated.GetDescription()), $"Shield event displayed {ShieldEvent.GlazierAutomaticallyAllocated}");                
            }
        }

        /// <summary>
        /// Verify the expected damage type and claim scenario have been recorded for
        /// this claim in Shield.
        /// </summary>
        /// <param name="claim"></param>
        private static void VerifyMotorGlassClaimScenarioDetails(ClaimCar claim)
        {
            var claimDetails = ShieldClaimDB.GetClaimScenario(claim.ClaimNumber);
            
            Reporting.LogMinorSectionHeading($"Verify Type and Scenario details for Claim: {claim.ClaimNumber}");
            Reporting.AreEqual(MotorClaimDamageTypeNames[claim.DamageType].TextShield, claimDetails.ClaimType);
            Reporting.AreEqual(MotorClaimDamageCodeAndScenarioNames[claim.DamageType].TextShield, claimDetails.ClaimScenario);

            //If event date is not today’s date, always defaulted to 3:59PM
            //Or event date is today and the lodgement time is after 3:59PM then also defaulted to 3:59PM
            if (claim.EventDateTime < DateTime.Today || DateTime.Now.TimeOfDay > new TimeSpan(15, 59, 0))
            {
                Reporting.AreEqual(claimDetails.EventDateAndTime, claim.EventDateTime.Date.Add(new TimeSpan(15, 59, 0)), "Event date and time in shield");
            }
            else
            {
                // If claim event is today AND before 3:59pm, then Shield records event time as now.
                var claimStartTimeLower = DateTime.Now.AddMinutes(-5);
                var claimStartTimeUpper = DateTime.Now;
                Reporting.IsTrue(claimDetails.EventDateAndTime > claimStartTimeLower && claimDetails.EventDateAndTime < claimStartTimeUpper,
                                 $"Event date and time in shield is {claimDetails.EventDateAndTime} and we expect within {claimStartTimeLower} and {claimStartTimeUpper}");
            }
        }

        /// <summary>
        /// Verify the expected the referral email is triggered when there is a payment block
        /// </summary>
        public static void VerifyMotorGlassReferralEmail(string claimNumber)
        {
            var _messageHandler = new MailosaurEmailHandler();
            var email = System.Threading.Tasks.Task.Run(() => _messageHandler.FindEmailBySubject($"New Claim {claimNumber}", retrySeconds: WaitTimes.T150SEC)).GetAwaiter().GetResult();

            if (email != null)
            {
                Reporting.AreEqual(emailText, email.Text.Body.StripLineFeedAndCarriageReturns(false), "Claim assist referal email for motor glass claim");
            }
        }


    }
}