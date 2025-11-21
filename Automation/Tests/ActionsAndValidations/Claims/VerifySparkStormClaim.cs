using Rac.TestAutomation.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using Rac.TestAutomation.Common.API;
using UIDriver.Pages.Spark.Claim.Home;

namespace Tests.ActionsAndValidations
{
    public static class VerifySparkStormClaim
    {
        public class Constants
        {
            public class ClaimantEventDescription
            {
                public static readonly string Uninhabitable = "^^UNINHABITABLE^^";
            }
            public class ReferralEmail
            {
                public static readonly string CannotMeasureFence                = "Trouble measuring my fence";
                public static readonly string ClaimWithinTwelveMonths           = "Claim in the last 12 months";
                public static readonly string ContactMemberRequested            = "Member requested contact";
                public static readonly string FenceTypeMoreThanOne              = "Fence type selected - More than one type of fence";
                public static readonly string FenceTypeOther                    = "Fence type selected - Other";
                public static readonly string FenceTypeUnsure                   = "Fence type selected - I'm not sure";
                public static readonly string PaymentBlock                      = "Payment block";
                public static readonly string ProcessOnlineClaimAsNormal        = "Please process online claim as per current business rules";
                public static readonly string ServiceProviderNotAllocated       = "Check claimant event description and view events";
                public static readonly string TempFenceRequiredLabel            = "Temporary Fence Required:";
            }
        }
        
        /// <summary>
        /// Verify claim details in Shield 
        /// </summary>
        public static void VerifyStormClaimDetailsInShield(ClaimHome claim, Browser browser)
        {
            if(claim.DamagedCovers == AffectedCovers.FenceOnly
            || claim.DamagedCovers == AffectedCovers.BuildingAndFence
            || claim.DamagedCovers == AffectedCovers.ContentsAndFence
            || claim.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
            {
                if (claim.ExpectedOutcome != ExpectedClaimOutcome.RepairsCompleted && claim.ExpectedOutcome != ExpectedClaimOutcome.AlreadyHaveRepairQuote)
                {
                    VerifyFenceClaimsQuestionnairesInShield(claim);
                }
            }
            
            VerifyUpdatedContactDetailsInMemberCentral(claim);
            VerifyBankDetails(claim);
            VerifyIncidentQuestionnairesInShield(claim);
            VerifyOnlineSettlementEventInShield(claim);
            VerifyClaimGeneral.VerifyClaimantEventDescription(claim);
            VerifyReferralEmail(claim);
            VerifyHomeStormClaimAgenda(claim, browser);
        }

        /// <summary>
        /// If member provided new mobile number or email, verify that the new details are recorded 
        /// in the integrated Member Central environment. Otherwise, confirm that the details 
        /// remain unchanged.
        /// </summary>
        private static void VerifyUpdatedContactDetailsInMemberCentral(ClaimHome claimData)
        {
            string shieldEnvironment = Config.Get().Shield.Environment;
            bool useMyRACLogin = Config.Get().MyRAC.IsMyRACSupportExpected();
            API_MemberCentralPersonV2Response response;
            
            response = MemberCentral.GetInstance().GET_PersonByPersonId(claimData.Claimant.PersonId).GetAwaiter().GetResult();
            //TODO: DED-958 - restore the "shieldint2 OR shielduat6" condition for the list of environments where we can expected changed details.
            if (useMyRACLogin &&
                (shieldEnvironment == "NoSuchEnvironmentExistsForNow"))
            {
                if (claimData.IsEmailAddressChanged)
                {
                    Reporting.AreEqual(claimData.Claimant.PrivateEmail?.NewAddress, response.PersonalEmailAddress, ignoreCase: true,
                        "expected new email address against the value found in Member Central NPE");
                }
                else
                {
                    Reporting.AreEqual(claimData.Claimant.PrivateEmail?.Address, response.PersonalEmailAddress, ignoreCase: true, 
                        "Personal Email address remains unchanged as expected");
                }
                
                if (claimData.IsMobileNumberChanged)
                {
                    Reporting.AreEqual(claimData.Claimant.NewMobilePhoneNumber, response.MobilePhone, 
                        "expected new mobile number against the value found in Member Central NPE");
                }
                else
                {
                    Reporting.AreEqual(claimData.Claimant.MobilePhoneNumber, response.MobilePhone,
                        "Mobile telephone number remains unchanged as expected");
                }
            }
            else
            {
                Reporting.Log($"Configuration of 'Use myRAC Login' = '{useMyRACLogin}' when testing against Shield environment '{shieldEnvironment} means " +
                    $"updating contact details was not available for this test, we don't expect any change to mobile telephone or email.");
                Reporting.AreEqual(claimData.Claimant.PrivateEmail?.Address, response.PersonalEmailAddress, ignoreCase: true,
                        "Personal Email address remains unchanged as expected");
                Reporting.AreEqual(claimData.Claimant.MobilePhoneNumber, response.MobilePhone,
                       "Mobile telephone number remains unchanged as expected");
            }
        }


        /// <summary>
        /// If member accepted the online settlement and
        /// provided the bank details then only it's applicable
        /// Verify correct bank details added in the Shield
        /// </summary>
        /// <param name="claimData"></param>
        private static void VerifyBankDetails(ClaimHome claimData)
        {
            if (claimData.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails)
            {
                Reporting.LogMinorSectionHeading("Verifying Bank Details in Shield");

                var contactDetails = DataHelper.GetContactDetailsViaContactId(claimData.Claimant.Id);

                var result = contactDetails.BankAccounts.Any(bankDetails => bankDetails.Bsb == claimData.AccountForSettlement.Bsb
                         && bankDetails.AccountNumber == claimData.AccountForSettlement.AccountNumber
                         && bankDetails.AccountName == claimData.AccountForSettlement.AccountName);

                Reporting.IsTrue(result, "Bank Account details added in Shield");

            }
        }


        /// <summary>
        /// Validate fence claim questionnaires in Shield
        /// using the Get Claim API
        /// </summary>
        public static void VerifyFenceClaimsQuestionnairesInShield(ClaimHome claim)
        {
            Reporting.LogMinorSectionHeading("Verifying Fence claim questionnaires in Shield");

            var shieldClaimRecord = DataHelper.GetClaimDetails(claim.ClaimNumber);

            var actualFenceMaterialCode = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.FenceType).AnswerId;
            Reporting.AreEqual(FenceTypeNames[claim.FenceDamage.FenceMaterial].ShieldAnswerId, actualFenceMaterialCode, HomeClaimQuestionnaire.FenceType.GetDescription());

            var actualFenceMetresDamaged = Convert.ToDecimal(shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.FenceMetresDamaged).AnswerValue);                        
            if (claim.FenceDamage.MetresPanelsDamaged != null)
            {
                Reporting.AreEqual(claim.FenceDamage.MetresPanelsDamaged, actualFenceMetresDamaged, HomeClaimQuestionnaire.FenceMetresDamaged.GetDescription());
            }
            
            var actualFenceMetresPainted = Convert.ToDecimal(shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.FenceMetresPainted).AnswerValue);                        
            Reporting.AreEqual(claim.FenceDamage.MetresPanelsPainted, actualFenceMetresPainted, HomeClaimQuestionnaire.FenceMetresPainted.GetDescription());

            if (claim.ExpectedOutcome != ExpectedClaimOutcome.RepairsCompleted)
            { 
                var actualIsTemporaryFenceRequired = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.FenceRequireTemporary).GetAnswerIDAsMappedString();
                var expectedIsTemporaryFenceRequired = claim.FenceDamage.IsAreaSafe ? No : Yes;
                Reporting.AreEqual(expectedIsTemporaryFenceRequired, actualIsTemporaryFenceRequired, HomeClaimQuestionnaire.FenceRequireTemporary.GetDescription());
            }
            
            var actualAffectedFenceBoundaries = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.FenceAffectedSides).AnswerValue;

            Reporting.AreEqual(claim.FenceDamage.AffectedBoundaryLeft, actualAffectedFenceBoundaries.Contains("Left"), $"{HomeClaimQuestionnaire.FenceAffectedSides.GetDescription()} - Left");
            Reporting.AreEqual(claim.FenceDamage.AffectedBoundaryRight, actualAffectedFenceBoundaries.Contains("Right"), $"{HomeClaimQuestionnaire.FenceAffectedSides.GetDescription()} - Right");
            Reporting.AreEqual(claim.FenceDamage.AffectedBoundaryFront, actualAffectedFenceBoundaries.Contains("Front"), $"{HomeClaimQuestionnaire.FenceAffectedSides.GetDescription()} - Front");
            Reporting.AreEqual(claim.FenceDamage.AffectedBoundaryRear, actualAffectedFenceBoundaries.Contains("Rear"), $"{HomeClaimQuestionnaire.FenceAffectedSides.GetDescription()} - Rear");
        }

        /// <summary>
        /// The incident questionnaires in Shield does not get updated
        /// if member already repaired the fence
        /// Validate fence incident circumstances questionnaires in Shield
        /// using the Get Claim API
        /// </summary>
        private static void VerifyIncidentQuestionnairesInShield(ClaimHome claim)
        {
            Reporting.LogMinorSectionHeading("Verifying incident circumstances questionnaires in Shield");

            var shieldClaimRecord = DataHelper.GetClaimDetails(claim.ClaimNumber);

            var expectedCashsettlementAnswer = No;
            var expectedEFTDetailsPending = No;

            

            if (claim.DamagedCovers == AffectedCovers.FenceOnly
            || claim.DamagedCovers == AffectedCovers.BuildingAndFence
            || claim.DamagedCovers == AffectedCovers.ContentsAndFence
            || claim.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
            {
                if (claim.ExpectedOutcome != ExpectedClaimOutcome.RepairsCompleted)
                {
                    switch (claim.ExpectedOutcome)
                    {
                        case ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails:
                        case ExpectedClaimOutcome.OnlineSettlementAcceptWithExistingBankDetails:
                            expectedCashsettlementAnswer = Yes;
                            break;
                        case ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails:
                            expectedCashsettlementAnswer = Yes;
                            expectedEFTDetailsPending = EFT;
                            break;
                        case ExpectedClaimOutcome.GetRepairQuoteFirst:
                        case ExpectedClaimOutcome.OnlineSettlementTakeMoreTimeToDecide:
                        case ExpectedClaimOutcome.RepairsCompleted:
                        case ExpectedClaimOutcome.AlreadyHaveRepairQuote:
                            expectedEFTDetailsPending = EFT;
                            break;
                        default:
                            break;
                    }

                    var actualCashsettlementAnswer = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.AreWeCashSettlingToday).GetAnswerIDAsMappedString();
                    Reporting.AreEqual(expectedCashsettlementAnswer, actualCashsettlementAnswer, HomeClaimQuestionnaire.AreWeCashSettlingToday.GetDescription());

                    var uninhabitableLineId = HomeClaimQuestionnaire.IsHomeUninhabitableNotStormDamage;
                    var isHomeUninhabitable = shieldClaimRecord.GetQuestionnaireLine((int)uninhabitableLineId).GetAnswerIDAsMappedString();
                    Reporting.AreEqual(No, isHomeUninhabitable, uninhabitableLineId.GetDescription());
                    
                    var isHomeAssessorRequired = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.IsHomeAssessorRequired).GetAnswerIDAsMappedString();
                    Reporting.AreEqual(No, isHomeAssessorRequired, HomeClaimQuestionnaire.IsHomeAssessorRequired.GetDescription());
                    
                    var isEFTDetailsRequired = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.MemberNeedsToProvideEFTOrPAF).GetAnswerIDAsMappedString();
                    Reporting.AreEqual(expectedEFTDetailsPending, isEFTDetailsRequired, HomeClaimQuestionnaire.MemberNeedsToProvideEFTOrPAF.GetDescription());
                }


            }
        }

        /// <summary>
        /// A Referral Email will be sent in a variety of scenarios as laid out in the SettleFenceOnline
        /// options below. Most of these are ineligible for Online Settlement.
        /// 
        /// This method confirms the Referral email exists when it is expected, and key elements of the 
        /// content of that email.
        /// </summary>
        private static void VerifyReferralEmail(ClaimHome claimData)
        {
            if (claimData.EligibilityForOnlineSettlement == SettleFenceOnline.IneligibleCannotMeasureFence ||
                claimData.EligibilityForOnlineSettlement == SettleFenceOnline.IneligibleClaimWithinTwelveMonths ||
                claimData.EligibilityForOnlineSettlement == SettleFenceOnline.IneligibleFenceTypeMoreThanOne ||
                claimData.EligibilityForOnlineSettlement == SettleFenceOnline.IneligibleFenceTypeOther ||
                claimData.EligibilityForOnlineSettlement == SettleFenceOnline.IneligibleFenceTypeUnsure ||
                claimData.EligibilityForOnlineSettlement == SettleFenceOnline.IneligiblePaymentBlock ||
                claimData.ExpectedOutcome == ExpectedClaimOutcome.FailureToAllocateServiceProvider ||
                claimData.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementContactMe)
            {
                Reporting.LogMinorSectionHeading("Verifying Referral email");

                var emailHandler = new MailosaurEmailHandler();
                var email = Task.Run(() => emailHandler.FindEmailBySubject($"New Claim {claimData.ClaimNumber}")).GetAwaiter().GetResult();
                var message = email.Text.Body.ToString();
                string expectedEmailContent = null;
                
                switch (claimData.EligibilityForOnlineSettlement)
                {
                    case SettleFenceOnline.IneligibleCannotMeasureFence:
                        expectedEmailContent = Constants.ReferralEmail.CannotMeasureFence; 
                        break;
                    case SettleFenceOnline.IneligibleClaimWithinTwelveMonths:
                        expectedEmailContent = Constants.ReferralEmail.ClaimWithinTwelveMonths;
                        break;
                    case SettleFenceOnline.IneligibleFenceTypeMoreThanOne:
                        expectedEmailContent = Constants.ReferralEmail.FenceTypeMoreThanOne;
                        break;
                    case SettleFenceOnline.IneligibleFenceTypeOther:
                        expectedEmailContent = Constants.ReferralEmail.FenceTypeOther;
                        break;
                    case SettleFenceOnline.IneligibleFenceTypeUnsure:
                        expectedEmailContent = Constants.ReferralEmail.FenceTypeUnsure;
                        break;
                    case SettleFenceOnline.IneligiblePaymentBlock:
                        expectedEmailContent = Constants.ReferralEmail.PaymentBlock;
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(expectedEmailContent))
                {
                    Reporting.IsTrue(message.Contains(expectedEmailContent), 
                        $"Referral email body contains \"{expectedEmailContent}\"");
                }

                switch (claimData.ExpectedOutcome)
                {
                    case ExpectedClaimOutcome.FailureToAllocateServiceProvider:
                        expectedEmailContent = Constants.ReferralEmail.ServiceProviderNotAllocated;
                        break;
                    case ExpectedClaimOutcome.OnlineSettlementContactMe:
                        expectedEmailContent = Constants.ReferralEmail.ContactMemberRequested; 
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(expectedEmailContent))
                {
                    Reporting.IsTrue(message.Contains(expectedEmailContent), 
                        $"Referral email body contains \"{expectedEmailContent}\"");
                }

                Reporting.IsFalse(message.Contains(Constants.ReferralEmail.TempFenceRequiredLabel), 
                    $"Referral Email body should never contain {Constants.ReferralEmail.TempFenceRequiredLabel}");

                Reporting.IsTrue(message.Contains(Constants.ReferralEmail.ProcessOnlineClaimAsNormal),
                            $"Referral Email body contains \"{Constants.ReferralEmail.ProcessOnlineClaimAsNormal}\" as expected");
            }
        }

        /// <summary>
        /// Applicable for Spark Fence-Only home claims, where we wish to verify that
        /// Shield has correctly recorded online settlement events
        /// </summary>      
        /// <param name="claimData"></param>
        private static void VerifyOnlineSettlementEventInShield(ClaimHome claimData)
        {
            if (claimData.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails ||
               claimData.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails ||
               claimData.ExpectedOutcome == ExpectedClaimOutcome.GetRepairQuoteFirst)
            {
                Reporting.LogMinorSectionHeading("Verifying Settlement Event in Shield");

                var shieldEvent = ShieldClaimDB.GetClaimEvents(claimData.ClaimNumber);
                Reporting.IsTrue(shieldEvent.Contains(ShieldEvent.CompleteOnlineLodgement.GetDescription()), "Shield event displayed: Complete Online Claim Lodgement");
                Reporting.IsTrue(shieldEvent.Contains(ShieldEvent.SettlementOfferDisplayed.GetDescription()), "Shield event displayed: Online settlement offer displayed");

                switch (claimData.ExpectedOutcome)
                {
                    case ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails:
                    case ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails:
                        Reporting.IsTrue(shieldEvent.Contains(ShieldEvent.SettlementOfferAccepted.GetDescription()), "Shield event displayed: Online settlement offer accepted");
                        break;
                    case ExpectedClaimOutcome.GetRepairQuoteFirst:
                        Reporting.IsTrue(shieldEvent.Contains(ShieldEvent.SettlementOfferDeclined.GetDescription()), "Shield event displayed: Online settlement offer declined");
                        break;
                    default:
                        break;
                }
            }
        }

         
        /// <summary>
        /// Verify claim agenda status in Shield for Home Storm claims.
        /// </summary>
        private static void VerifyHomeStormClaimAgenda(ClaimHome claimData, Browser browser)
        {
            var agendaStatus = ShieldHomeClaimDB.GetHomeStormClaimAgendaStatus(claimData.ClaimNumber);

            var claimLodgedAgendaStatus         = Status.Current.GetDescription();
            var buildingQuoteAuthorisedStatus   = Status.Pending.GetDescription();
            var contentsQuoteAuthorisedStatus   = Status.Pending.GetDescription();
            var buildingRepairsCompleteStatus   = Status.Pending.GetDescription();
            var contentsReplacedOrPaidOutStatus = Status.Pending.GetDescription();
            if (claimData.DamagedCovers == AffectedCovers.FenceOnly)
            {
                Reporting.LogMinorSectionHeading($"Verifying claim agenda status in Shield for {claimData.DamagedCovers}");
                
                if (claimData.EligibilityForOnlineSettlement == SettleFenceOnline.Eligible ||
                    claimData.EligibilityForOnlineSettlement == SettleFenceOnline.IneligibleExcessMoreThanRepairs ||
                    claimData.EligibilityForOnlineSettlement == SettleFenceOnline.IneligibleFenceTypeBrick ||
                    claimData.EligibilityForOnlineSettlement == SettleFenceOnline.IneligibleFenceTypeWood ||
                    claimData.EligibilityForOnlineSettlement == SettleFenceOnline.IneligibleOverThirtyMetres ||
                    claimData.EligibilityForOnlineSettlement == SettleFenceOnline.IneligibleRepairsAlreadyQuoted ||
                    claimData.EligibilityForOnlineSettlement == SettleFenceOnline.IneligibleRepairsAlreadyCompleted
                    )
                {
                    claimLodgedAgendaStatus         = Status.Done.GetDescription();
                    buildingQuoteAuthorisedStatus   = Status.Current.GetDescription();
                    contentsQuoteAuthorisedStatus   = Status.NotApplicable.GetDescription();
                }
                else
                {
                    claimLodgedAgendaStatus         = Status.Current.GetDescription();
                    buildingQuoteAuthorisedStatus   = Status.Pending.GetDescription();
                    contentsQuoteAuthorisedStatus   = Status.NotApplicable.GetDescription();
                }
            }

            if (claimData.DamagedCovers == AffectedCovers.ContentsOnly)
            {
                Reporting.LogMinorSectionHeading($"Verifying claim agenda status in Shield for {claimData.DamagedCovers}");
                if(!claimData.ContentsDamage.IsCarpetTooWet)
                {
                    using (var contentStormDamage = new DamagedContents(browser))
                    {
                        Reporting.Log($"Answering '{DamagedContents.Constants.Field.Label.IsCarpetTooWet}' = FALSE " +
                            $"regarding damage to contents means that the Shield flag 'isClaimLodgementDone' = FALSE" +
                            $"so the Claims Agenda should not have moved on from the `Lodged` step. " +
                            $"Refer to <a href=\"https://rac-wa.atlassian.net/wiki/spaces/ISP/pages/3333194010/CXOne+-+Building+only+Contents+only+all+combo+flows+-+Claims+referral+email\">documentation here</a>");
                    }
                    claimLodgedAgendaStatus = Status.Current.GetDescription();
                    buildingQuoteAuthorisedStatus = Status.NotApplicable.GetDescription();
                    contentsQuoteAuthorisedStatus = Status.Pending.GetDescription();
                    buildingRepairsCompleteStatus = Status.NotApplicable.GetDescription();
                    contentsReplacedOrPaidOutStatus = Status.Pending.GetDescription();
                }
                else
                {
                    Reporting.Log($"Because '{DamagedContents.Constants.Field.Label.IsCarpetTooWet}' = TRUE regarding damage to contents it should mean that the " +
                            $"Shield flag 'isClaimLodgementDone' = TRUE and so the status of the 'Lodged' Claims Agenda step should have moved to 'Done'. " +
                            $"NOTE! If test fails below because 'Lodged' agenda step status is 'Current', then check whether Allocation of Service Provider " +
                            $"has failed, as this will prevent the Lodged step from changing to 'Done'. " +
                            $"Refer to <a href=\"https://rac-wa.atlassian.net/wiki/spaces/ISP/pages/3333194010/CXOne+-+Building+only+Contents+only+all+combo+flows+-+Claims+referral+email\">documentation here</a>");
                    claimLodgedAgendaStatus = Status.Done.GetDescription();
                    buildingQuoteAuthorisedStatus = Status.NotApplicable.GetDescription();
                    contentsQuoteAuthorisedStatus = Status.Current.GetDescription();
                    buildingRepairsCompleteStatus = Status.NotApplicable.GetDescription();
                    contentsReplacedOrPaidOutStatus = Status.Pending.GetDescription();
                }
            }

            if (claimData.DamagedCovers == AffectedCovers.BuildingOnly
             || claimData.DamagedCovers == AffectedCovers.BuildingAndContents
             || claimData.DamagedCovers == AffectedCovers.BuildingAndFence
             || claimData.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
            {
                Reporting.LogMinorSectionHeading($"Verifying claim agenda status in Shield for {claimData.DamagedCovers}");
                Reporting.Log($"NOTE! If test fails because Lodged agenda status is 'Current' instead of 'Done' then check whether Allocation of " +
                            $"Service Provider has failed, as this will prevent the Lodged step from changing to 'Done'. " +
                            $"Refer to <a href=\"https://rac-wa.atlassian.net/wiki/spaces/ISP/pages/3333194010/CXOne+-+Building+only+Contents+only+all+combo+flows+-+Claims+referral+email\">this documentation</a>.");
                claimLodgedAgendaStatus = Status.Done.GetDescription();
                buildingQuoteAuthorisedStatus = Status.Current.GetDescription();
                contentsQuoteAuthorisedStatus = Status.Current.GetDescription();
                buildingRepairsCompleteStatus = Status.Pending.GetDescription();
                contentsReplacedOrPaidOutStatus = Status.Pending.GetDescription();

                string claimantEventDescription = ShieldHomeClaimDB.GetClaimantEventDescriptionFromShield(claimData.ClaimNumber);

                // When the Building damage indicates an uninhabitable state, we expect an Internal Home Assessor (IHA) to be allocated.
                // This section of code deals with the possible outcomes by first checking for events indicating that an IHA has not been
                // allocated, setting expectations for Agenda accordingly. If those events don't exist then we we assert that an IHA should
                // have been allocated to the claim.
                // I've tried to make the details of this explicit in the logging to help when reviewing a test run rather than here.
                if (!claimData.IsHomeInhabitable
                 && claimantEventDescription.Contains(Constants.ClaimantEventDescription.Uninhabitable))
                {
                    Reporting.LogMinorSectionHeading($"NOTE: As Property is uninhabitable, an Internal Home Assessor should be allocated " +
                        $"for properties inside the metropolitan area. If allocation of the IHA fails, then it will prevent the status of " +
                        $"the Lodged step from being updated to 'Done', leaving it set as 'Current'.");

                    var shieldEvent = ShieldClaimDB.GetClaimEvents(claimData.ClaimNumber);
                    var shieldClaimRecord = DataHelper.GetClaimDetails(claimData.ClaimNumber);

                    // We would not usually base our expected results on the Events displayed in Shield, but this is presently our
                    // best option. A future PR may provide a better method, depending on what Sapiens are able to advise.
                    if (shieldEvent.Contains(ShieldEvent.HomeAssessorReferral.GetDescription()))
                    {
                        Reporting.LogMinorSectionHeading($"Shield Event '{ShieldEvent.HomeAssessorReferral.GetDescription()}' exists, " +
                            $"indicating that no IHA has been assigned, probably because allocation has been " +
                            $"exhausted for the region this claim falls within.{Reporting.HTML_NEWLINE}" +
                            $"As a result, we will expect the status of the Lodged step in the Claim Agenda to be 'Current' " +
                            $"instead of 'Done'.");

                        claimLodgedAgendaStatus = Status.Current.GetDescription();
                        buildingQuoteAuthorisedStatus = Status.Pending.GetDescription();
                        contentsQuoteAuthorisedStatus = Status.Pending.GetDescription();
                        buildingRepairsCompleteStatus = Status.Pending.GetDescription();
                        contentsReplacedOrPaidOutStatus = Status.Pending.GetDescription();
                    }
                    else if (shieldEvent.Contains(ShieldEvent.NonMetroHomeAssessorReferral.GetDescription()))
                    {
                        Reporting.LogMinorSectionHeading($"Shield Event '{ShieldEvent.NonMetroHomeAssessorReferral.GetDescription()}' exists, " +
                            $"indicating that no IHA has been assigned <i>due to the insured property being in a non-metropolitan area</i>.{Reporting.HTML_NEWLINE}" +
                            $"In this situation a 'Home Assessor Referral - FU' follow up is created against the policy for claims staff to handle.{Reporting.HTML_NEWLINE}" +
                            $"This should NOT prevent Service Providers or the Claim Handler from being assigned and the status of the Lodged step " +
                            $"in the Claim Agenda should be 'Done' as it would be in the metro area with an IHA assigned.");
                        
                        claimLodgedAgendaStatus = Status.Done.GetDescription();
                        buildingQuoteAuthorisedStatus = Status.Current.GetDescription();
                        contentsQuoteAuthorisedStatus = Status.Current.GetDescription();
                        buildingRepairsCompleteStatus = Status.Pending.GetDescription();
                        contentsReplacedOrPaidOutStatus = Status.Pending.GetDescription();
                    }
                    else if (!shieldEvent.Contains(ShieldEvent.HomeAssessorReferral.GetDescription())
                          && !shieldEvent.Contains(ShieldEvent.NonMetroHomeAssessorReferral.GetDescription()))
                    {
                        Reporting.Log($"Shield Events do not include a '{ShieldEvent.HomeAssessorReferral.GetDescription()}' " +
                            $"or '{ShieldEvent.NonMetroHomeAssessorReferral.GetDescription()}' event. We should have an IHA allocated; checking now.");
                        if (!string.IsNullOrEmpty(shieldClaimRecord.ReturnInternalHomeAssessor()))
                        {
                            var internalHomeAssessor = shieldClaimRecord.ReturnInternalHomeAssessor();
                            
                            Reporting.Log($"IHA found: {internalHomeAssessor}");
                            
                            claimLodgedAgendaStatus = Status.Done.GetDescription();
                            buildingQuoteAuthorisedStatus = Status.Current.GetDescription();
                            contentsQuoteAuthorisedStatus = Status.Current.GetDescription();
                            buildingRepairsCompleteStatus = Status.Pending.GetDescription();
                            contentsReplacedOrPaidOutStatus = Status.Pending.GetDescription();
                        }
                        else
                        {
                            Reporting.Error($"Could not find an Internal Home Assessor related to this claim when we expected one! Panic and freak out!");
                        }
                    }
                }
                else if (!claimData.IsHomeInhabitable
                      && !claimantEventDescription.Contains(Constants.ClaimantEventDescription.Uninhabitable))
                {
                    Reporting.Error($"Test data flag indicates that this claim should be flagged uninhabitable but " +
                        $"Claimant Event Description for claim {claimData.ClaimNumber} does not contain '{Constants.ClaimantEventDescription.Uninhabitable}'. " +
                        $"Investigation required!");
                }
                else if (claimData.IsHomeInhabitable
                      && !claimantEventDescription.Contains(Constants.ClaimantEventDescription.Uninhabitable))
                {
                    Reporting.Log($"Test data IsHomeInhabitable flag = {claimData.IsHomeInhabitable} and Claimant Event Description does not contain " +
                        $"'{Constants.ClaimantEventDescription.Uninhabitable}' so not checking for Internal Claims Assessor allocation.");
                }
                else 
                {
                    Reporting.Error($"Unanticipated scenario has been encountered, please investigate this test.");
                }
            }

            if (claimData.DamagedCovers == AffectedCovers.BuildingOnly
             || claimData.DamagedCovers == AffectedCovers.FenceOnly
             || claimData.DamagedCovers == AffectedCovers.BuildingAndFence)
            {
                Reporting.LogMinorSectionHeading($"Damage to Contents not included so setting assertions for those steps to be 'Not Applicable'");
                contentsQuoteAuthorisedStatus = Status.NotApplicable.GetDescription();
                contentsReplacedOrPaidOutStatus = Status.NotApplicable.GetDescription();
            }

            if (claimData.DamagedCovers == AffectedCovers.ContentsOnly)
            {
                Reporting.LogMinorSectionHeading($"Damage to Building not included so setting assertions for those steps to be 'Not Applicable'");
                buildingQuoteAuthorisedStatus = Status.NotApplicable.GetDescription();
                buildingRepairsCompleteStatus = Status.NotApplicable.GetDescription();
            }

            if (claimData.DamagedCovers == AffectedCovers.BuildingOnly
             || claimData.DamagedCovers == AffectedCovers.BuildingAndContents
             || claimData.DamagedCovers == AffectedCovers.BuildingAndFence
             || claimData.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
            {
                if (claimData.BuildingDamage.StormDamagedBuildingSpecifics.Contains(StormDamagedItemTypes.OtherItems))
                {
                    Reporting.LogMinorSectionHeading($"As 'Other items' has been selected, the status of the Lodged step in Claim Agenda will remain current.");
                    claimLodgedAgendaStatus         = Status.Current.GetDescription();
                
                    if (claimData.DamagedCovers == AffectedCovers.BuildingOnly
                     || claimData.DamagedCovers == AffectedCovers.BuildingAndContents
                     || claimData.DamagedCovers == AffectedCovers.BuildingAndFence
                     || claimData.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
                    {
                        buildingQuoteAuthorisedStatus = Status.Pending.GetDescription();
                    }

                    if (claimData.DamagedCovers == AffectedCovers.BuildingAndContents
                     || claimData.DamagedCovers == AffectedCovers.ContentsAndFence
                     || claimData.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
                    {
                        contentsQuoteAuthorisedStatus = Status.Pending.GetDescription();
                    }
                }
            }

            Reporting.AreEqual(claimLodgedAgendaStatus, agendaStatus[AgendaStepNames.ClaimLodged.GetDescription()], false, $"expected status of Agenda step '{AgendaStepNames.ClaimLodged.GetDescription()}' matches actual status");
            Reporting.AreEqual(buildingQuoteAuthorisedStatus, agendaStatus[AgendaStepNames.BuildingQuoteAuthorised.GetDescription()], false, $"expected status of Agenda step '{AgendaStepNames.BuildingQuoteAuthorised.GetDescription()}' matches actual status");
            Reporting.AreEqual(contentsQuoteAuthorisedStatus, agendaStatus[AgendaStepNames.ContentsQuoteAuthorised.GetDescription()], false, $"expected status of Agenda step '{AgendaStepNames.ContentsQuoteAuthorised.GetDescription()}' matches actual status");
            Reporting.AreEqual(buildingRepairsCompleteStatus, agendaStatus[AgendaStepNames.RepairsComplete.GetDescription()], false, $"expected status of Agenda step '{AgendaStepNames.RepairsComplete.GetDescription()}' matches actual status");
            Reporting.AreEqual(contentsReplacedOrPaidOutStatus, agendaStatus[AgendaStepNames.ContentsReplacedOrPaidOut.GetDescription()], false, $"expected status of Agenda step '{AgendaStepNames.ContentsReplacedOrPaidOut.GetDescription()}' matches actual status");
            Reporting.AreEqual(Status.Pending.GetDescription(), agendaStatus[AgendaStepNames.PaymentsSettled.GetDescription()], false, $"expected status of Agenda step '{AgendaStepNames.PaymentsSettled.GetDescription()}' matches actual status");
        }
    }
}
