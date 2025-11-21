using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UIDriver.Pages.B2C;
using UIDriver.Pages.Claims;
using UIDriver.Pages.PCM;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Tests.ActionsAndValidations
{
    public static class VerifyClaimHome
    {
        /// <summary>
        /// Verifies the details that are common across all home claim
        /// damage types, on the claim confirmation screen.
        ///
        /// Closes browser window on completion.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claimData"></param>
        /// <returns>Claim number displayed on screen.</returns>
        public static string VerifyDetailsOnClaimConfirmationPage(Browser browser, ClaimHome claimData)
        {
            string claimNumber = null;

            using (var confirmation = new ClaimHomePageConfirmation(browser))
            {
                claimNumber = confirmation.ClaimNumber;
                Reporting.Log($"Claim confirmation page shown for {claimNumber}", browser.Driver.TakeSnapshot());

                VerifyConfirmationPageLeftPanel(confirmation, claimData);
                VerifyConfirmationPageRightPanel(confirmation, claimData);
            }

            browser.CloseBrowser();
            return claimNumber;
        }

        /// <summary>
        /// Validate and report Settlement Offer Displayed event in Shield
        /// </summary>
        private static void ValidateSettlementOfferDisplayedEventInShield(List<string> shieldEvent)
        {
            Reporting.IsTrue(shieldEvent.Contains(ShieldEvent.SettlementOfferDisplayed.GetDescription()), "Shield event displayed: Online settlement offer displayed");
        }

        /// <summary>
        /// Validate and report Complete Online Claim Lodgement event in Shield
        /// </summary>
        private static void ValidateCompleteOnlineLodgementEventInShield(List<string> shieldEvent)
        {
            Reporting.IsTrue(shieldEvent.Contains(ShieldEvent.CompleteOnlineLodgement.GetDescription()), "Shield event displayed: Complete Online Claim Lodgement");
        }

        /// <summary>
        /// Verifying general claim information provided to the claimant after
        /// a home claim has been lodged.
        /// </summary>
        /// <param name="confirmation">Reference to the confirmation page object</param>
        private static void VerifyConfirmationPageLeftPanel(ClaimHomePageConfirmation confirmation,
                                                            ClaimHome claimData)
        {
            Reporting.Log("Validating left panel text.");



            switch (claimData.ExpectedOutcome)
            {
                case ExpectedClaimOutcome.GetRepairQuoteFirst:
                    {
                        var expectedTextRegex = new Regex(FixedTextRegex.CLAIMS_CONSULTANT_CONTACT_REGEX);
                        var displayedText = confirmation.GeneralClaimInformationParagraph;
                        var match = expectedTextRegex.Match(displayedText);
                        Reporting.IsTrue(match.Success, $"claims consultant contact text. Received: {displayedText}");

                        expectedTextRegex = new Regex(FixedTextRegex.CLAIMS_FAQ_TEXT_CLAIM_LODGED_REGEX);
                        displayedText = confirmation.ClaimFAQTextParagraph;
                        match = expectedTextRegex.Match(displayedText);
                        Reporting.IsTrue(match.Success, $"claims FAQ text. Received: {displayedText}");

                        break;
                    }
                case ExpectedClaimOutcome.OnlineSettlementRepairByRAC:
                case ExpectedClaimOutcome.OnlineSettlementContactMe:
                case ExpectedClaimOutcome.OnlineSettlementNoConsent:
                    {
                        var expectedTextRegex = new Regex(FixedTextRegex.CLAIMS_CONSULTANT_CONTACT_REGEX);
                        var displayedText = confirmation.GeneralClaimInformationParagraph;
                        var match = expectedTextRegex.Match(displayedText);
                        Reporting.IsTrue(match.Success, $"claims consultant contact text. Received: {displayedText}");

                        break;

                    }
                case ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails:
                case ExpectedClaimOutcome.OnlineSettlementTakeMoreTimeToDecide:
                {
                        var expectedTextRegex = new Regex(FixedTextRegex.CLAIMS_ONLINE_SETTLEMENT_RECEIVE_EMAIL);
                        var displayedText = confirmation.ReceiveEmailParagraph;
                        var match = expectedTextRegex.Match(displayedText);
                        Reporting.IsTrue(match.Success, $"Receive email text. Received: {displayedText}");

                        expectedTextRegex = new Regex(FixedTextRegex.CLAIMS_ONLINE_SETTLEMENT_EMAIL_LINK_EXPIRE);
                        displayedText = confirmation.EmailExpireParagraph;
                        match = expectedTextRegex.Match(displayedText);
                        Reporting.IsTrue(match.Success, $"Link in the email expire. Received: {displayedText}");

                        break;
                    }

                case ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails:
                    {
                        var expectedTextRegex = new Regex(FixedTextRegex.CLAIMS_ONLINE_SETTLEMENT_PROCESSING_TIME_REGEX);
                        var displayedText = confirmation.PaymentProcessingTimeParagraph;
                        var match = expectedTextRegex.Match(displayedText);
                        Reporting.IsTrue(match.Success, $"fence online settlment payment processing time. Received: {displayedText}");
                        break;
                    }
                default:
                    Reporting.Log($"Do nothing.");
                    break;
            }
        }

        /// <summary>
        /// Verifies general event details and claimant information
        /// </summary>
        /// <param name="confirmation"></param>
        /// <param name="claimData"></param>
        private static void VerifyConfirmationPageRightPanel(ClaimHomePageConfirmation confirmation,
                                                             ClaimHome claimData)
        {
            Reporting.Log("Validate event details on confirmation page.");
            Reporting.AreEqual(claimData.PolicyDetails.PolicyNumber, confirmation.DetailsPolicyNumber);
            Reporting.AreEqual(claimData.EventDateTime.ToString("d MMMM yyyy"), confirmation.DetailsEventDate.ToString("d MMMM yyyy"));
            Reporting.AreEqual(claimData.EventDateTime.ToString("h:mm tt"), confirmation.DetailsEventTime.ToString("h:mm tt"));
            Reporting.AreEqual(claimData.DamageType, confirmation.DetailsDamageType, "Damage type (enum) is as expected.");

            if (claimData.DamageType != HomeClaimDamageType.StormDamageToFenceOnly)
            {
                Reporting.Log("Validating right panel claimant details.");

                Reporting.AreEqual(claimData.Claimant.FirstName, confirmation.DetailsPHFirstName, true);
                Reporting.AreEqual(claimData.Claimant.Surname, confirmation.DetailsPHLastName, true);
                Reporting.AreEqual(claimData.Claimant.DateOfBirth.ToString("d MMMM yyyy"),
                                   confirmation.DetailsPHDateOfBirth.ToString("d MMMM yyyy"),
                                   true);
            }
        }

        /// <summary>
        /// Verify that Shield has correctly recorded claims questionnaire
        /// responses from claimant.
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <param name="claimData"></param>
        public static void VerifyClaimsQuestionnairesInShield(string claimNumber, ClaimHome claimData)
        {
            Reporting.LogMinorSectionHeading("Verifying home claim questionnaires");

            var claimDetails = DataHelper.GetClaimDetails(claimNumber);

            if (claimData.FenceDamage != null)
            {
                ValidateFenceSettlementStatus(claimData, claimDetails);
                ValidateFenceDamageAnswers(claimData, claimDetails);
            }
            ValidateHomeGeneralQuestionnaireAnswer(claimData, claimDetails);
        }

        private static void ValidateFenceSettlementStatus(ClaimHome claimData, GetClaimResponse shieldClaimRecord)
        {
            var expectedCashsettlementAnswer = No;
            if (claimData.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails ||
                claimData.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails)
            { expectedCashsettlementAnswer = Yes; }

            var actualCashsettlementAnswer = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.AreWeCashSettlingToday).GetAnswerIDAsMappedString();

            Reporting.AreEqual(expectedCashsettlementAnswer, actualCashsettlementAnswer, HomeClaimQuestionnaire.AreWeCashSettlingToday.GetDescription());
        }

        private static void ValidateFenceDamageAnswers(ClaimHome claimData, GetClaimResponse shieldClaimRecord)
        {
            var actualFenceMaterialCode = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.FenceType).AnswerId;
            Reporting.AreEqual(FenceTypeNames[claimData.FenceDamage.FenceMaterial].ShieldAnswerId, actualFenceMaterialCode, $"expected ID value for '{HomeClaimQuestionnaire.FenceType.GetDescription()}' with actual value in Shield");

            var actualFenceMetresDamaged = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.FenceMetresDamaged).AnswerValue;
            Reporting.AreEqual(claimData.FenceDamage.MetresPanelsDamaged.ToString(), actualFenceMetresDamaged, $"expected value for '{HomeClaimQuestionnaire.FenceMetresDamaged.GetDescription()}' with actual value in Shield");

            var actualFenceMetresPainted = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.FenceMetresPainted).AnswerValue;
            Reporting.AreEqual(claimData.FenceDamage.MetresPanelsPainted.ToString(), actualFenceMetresPainted, $"expected value for '{HomeClaimQuestionnaire.FenceMetresPainted.GetDescription()}' with actual value in Shield");

            var actualIsTemporaryFenceRequired = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.FenceRequireTemporary).GetAnswerIDAsMappedString();
            var expectedIsTemporaryFenceRequired = string.IsNullOrEmpty(claimData.FenceDamage.TemporaryFenceRequired) ? No : Yes;
            Reporting.AreEqual(expectedIsTemporaryFenceRequired, actualIsTemporaryFenceRequired, $"expected value for '{HomeClaimQuestionnaire.FenceRequireTemporary.GetDescription()}' with actual value in Shield");

            if (!string.IsNullOrEmpty(claimData.FenceDamage.TemporaryFenceRequired))
            {
                Reporting.AreEqual(claimData.FenceDamage.TemporaryFenceRequired, shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.FenceRequireTemporaryReason).AnswerValue, $"expected value for '{HomeClaimQuestionnaire.FenceRequireTemporaryReason.GetDescription()}' with actual value in Shield");
            }

            var actualAffectedFenceBoundaries = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.FenceAffectedSides).AnswerValue;
            Reporting.AreEqual(claimData.FenceDamage.AffectedBoundaryLeft,  actualAffectedFenceBoundaries.Contains("Left"), $"expected value for '{HomeClaimQuestionnaire.FenceAffectedSides.GetDescription()} - Left' with actual value in Shield");
            Reporting.AreEqual(claimData.FenceDamage.AffectedBoundaryRight, actualAffectedFenceBoundaries.Contains("Right"), $"expected value for '{HomeClaimQuestionnaire.FenceAffectedSides.GetDescription()} - Right' with actual value in Shield");
            Reporting.AreEqual(claimData.FenceDamage.AffectedBoundaryFront, actualAffectedFenceBoundaries.Contains("Front"), $"expected value for '{HomeClaimQuestionnaire.FenceAffectedSides.GetDescription()} - Front' with actual value in Shield");
            Reporting.AreEqual(claimData.FenceDamage.AffectedBoundaryRear,  actualAffectedFenceBoundaries.Contains("Rear"), $"expected value for '{HomeClaimQuestionnaire.FenceAffectedSides.GetDescription()} - Rear' with actual value in Shield");
        }

        private static void ValidateHomeGeneralQuestionnaireAnswer(ClaimHome claimData, GetClaimResponse shieldClaimRecord)
        {
            var isEFTDetailsRequired   = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.MemberNeedsToProvideEFTOrPAF).GetAnswerIDAsMappedString();

            var expectedEFTDetailsPending = No;
            if (claimData.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails
                || claimData.ExpectedOutcome == ExpectedClaimOutcome.GetRepairQuoteFirst)
            {
                expectedEFTDetailsPending = EFT; 
            }
            
            Reporting.AreEqual(expectedEFTDetailsPending, isEFTDetailsRequired, HomeClaimQuestionnaire.MemberNeedsToProvideEFTOrPAF.GetDescription());

            if (claimData.DamagedCovers != AffectedCovers.ContentsOnly)
            {
                var uninhabitableLineId = claimData.DamageType == HomeClaimDamageType.StormAndTempest ?
                                          HomeClaimQuestionnaire.IsHomeUninhabitableStormDamage :
                                          HomeClaimQuestionnaire.IsHomeUninhabitableNotStormDamage;
                var isHomeUninhabitable = shieldClaimRecord.GetQuestionnaireLine((int)uninhabitableLineId).GetAnswerIDAsMappedString();
                Reporting.AreEqual(No, isHomeUninhabitable, uninhabitableLineId.GetDescription());

                var isHomeAssessorRequired = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.IsHomeAssessorRequired).GetAnswerIDAsMappedString();
                Reporting.AreEqual(No, isHomeAssessorRequired, HomeClaimQuestionnaire.IsHomeAssessorRequired.GetDescription());

                if (claimData.DamageType == HomeClaimDamageType.StormAndTempest ||
                    claimData.DamageType == HomeClaimDamageType.Theft)
                {
                    Reporting.AreEqual(claimData.IsGlassDamaged, 
                                       shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.GlassDamage).GetAnswerIDAsEnum<GlassDamage>(),
                                       HomeClaimQuestionnaire.GlassDamage.GetDescription());

                    if (claimData.IsGarageDoorDamaged.ToString() != null)
                    {
                        Reporting.AreEqual(claimData.IsGarageDoorDamaged,
                                           shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.GarageDoorDamage).GetAnswerIDAsEnum<GarageDoorDamage>(),
                                           HomeClaimQuestionnaire.GarageDoorDamage.GetDescription());
                    }
                }

                if (claimData.DamageType == HomeClaimDamageType.Theft)
                {
                    var isEntryPointKnown = shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.TheftIsEntryPointKnown).GetAnswerIDAsMappedString();
                    var expectedIsEntryPointKnown = DataHelper.BooleanToStringYesNo(claimData.TheftDamage.IsEntryPointKnown);
                    Reporting.AreEqual(expectedIsEntryPointKnown, isEntryPointKnown, HomeClaimQuestionnaire.TheftIsEntryPointKnown.GetDescription());

                    if (claimData.TheftDamage.IsEntryPointKnown)
                    {
                        Reporting.AreEqual(claimData.TheftDamage.OffenderEntryDescription,
                                           shieldClaimRecord.GetQuestionnaireLine((int)HomeClaimQuestionnaire.TheftHowDidTheyEnter).AnswerValue,
                                           HomeClaimQuestionnaire.TheftHowDidTheyEnter.GetDescription());
                    }
                }
            }
        }

        /// <summary>
        /// Supports all core validations in Shield for a recently lodged home
        /// claim. Built to be suitable for all damage types.
        /// </summary>
        public static void VerifyHomeClaimInShield(ClaimHome claimData, string claimNumber)
        {
            VerifyHomeClaimAgenda(claimData, claimNumber);
            VerifyHomeClaimDamageCodeandStatusInShield(claimData, claimNumber);
            VerifyHomeClaimScenarioDetails(claimData, claimNumber);
            VerifyHomeClaimThirdPartyDetails(claimData, claimNumber);
            // TODO AUNT-190 to finalise the validation for Personal Valuables Accidental Damage
            if (claimData.DamageType != HomeClaimDamageType.AccidentalDamage)
            {
                VerifyHomeClaimContentsAndValuables(claimData, claimNumber);
            }
            VerifyClaimGeneral.VerifyClaimWitnessDetails(claimNumber, claimData.Witness);
        }

        /// <summary>
        /// Verify damage code and status for new home claim in Shield.
        /// </summary>
        private static void VerifyHomeClaimDamageCodeandStatusInShield(ClaimHome claimData, string claimNumber)
        {
            var damageDetailsByCover = ShieldHomeClaimDB.GetAffectedCoversAndReserves(claimNumber);
            var expectedAffectedCovers = claimData.ExpectedAffectedCoversForThisClaim();
            var expectedAffectedPersonalValuablesCovers = claimData.ExpectedDamageCodesForPersonalValuables();
            foreach (var shieldDamageLineItem in damageDetailsByCover)
            {
                if (claimData.DamageType == HomeClaimDamageType.AccidentalDamage)
                {
                    var expectedPersonalItem = expectedAffectedPersonalValuablesCovers;
                    Reporting.IsNotNull(expectedPersonalItem, $"{shieldDamageLineItem.DamageCode} was logged in Shield as expected for this claim");
                    Reporting.AreEqual(Status.Open.GetDescription(), shieldDamageLineItem.DamageStatus, false, "home claim damage status");
                    Reporting.IsTrue(int.Parse(shieldDamageLineItem.ReserveAmount) > 0, "Reserve amount is greater than zero");
                }
                else
                {
                    var expectedItem = expectedAffectedCovers.FirstOrDefault(x => x.DamageCode == shieldDamageLineItem.DamageCode);
                    Reporting.IsNotNull(expectedItem, $"{shieldDamageLineItem.DamageCode} was logged in Shield as expected for this claim");
                    Reporting.AreEqual(Status.Open.GetDescription(), shieldDamageLineItem.DamageStatus, false, "home claim damage status");
                    Reporting.IsTrue(int.Parse(shieldDamageLineItem.ReserveAmount) > 0, "Reserve amount is greater than zero");
                    expectedAffectedCovers.Remove(expectedItem);
                }
            }
            if (claimData.DamageType != HomeClaimDamageType.AccidentalDamage)
            {
                Reporting.IsTrue(expectedAffectedCovers.Count == 0, $"there are no expected damaged covers missed from claim in Shield");
            }
        }

        /// <summary>
        /// Verify recorded claim cause and scenario, as well as police
        /// report information.
        /// </summary>
        private static void VerifyHomeClaimScenarioDetails(ClaimHome claim, string claimNumber)
        {
            var claimDetails = ShieldClaimDB.GetClaimScenario(claimNumber);
            Reporting.Log($"Verify Type and Scenario details for Claim:");
            Reporting.AreEqual(HomeClaimTypeAndScenarioName[claim.DamageType], claimDetails.ClaimType);

            var expectedScenarioName = claim.ExpectedShieldScenarioName();
            Reporting.AreEqual(expectedScenarioName, claimDetails.ClaimScenario);

            if (string.IsNullOrEmpty(claim.PoliceReportNumber))
            {
                Reporting.IsTrue(string.IsNullOrEmpty(claimDetails.PoliceReportNumber), "no police report number should be recorded if we did not set it");
            }
            else
            {
                Reporting.AreEqual(claim.PoliceReportNumber, claimDetails.PoliceReportNumber);
                Reporting.AreEqual(claim.PoliceReportDate.Date, claimDetails.PoliceReportDate.Date, "date for police report is preserved as entered by claimant");
            }
        }

        private static void VerifyHomeClaimAgenda(ClaimHome claimData, string claimNumber)
        {
            if (claimData.DamageType == HomeClaimDamageType.AccidentalDamage)
            {
                var expectedPersonalValuablesCovers = claimData.ExpectedDamageCodesForPersonalValuables();
                var coverType = claimData.PolicyDetails.HasOVSCover() ? ClaimCovers.ShieldSpecifiedPersonalValuables : ClaimCovers.ShieldUnpecifiedPersonalValuables;
                var personalValuablesCover = expectedPersonalValuablesCovers.First(x => x.EndsWith(coverType));
                VerifyAgendaStepsForCover(claimData, claimNumber, personalValuablesCover);
            }
            else
            {
                var expectedAffectedCovers = claimData.ExpectedAffectedCoversForThisClaim();

                if (claimData.DamagedCovers == AffectedCovers.ContentsOnly ||
                    claimData.DamagedCovers == AffectedCovers.BuildingAndContents)
                {
                    var contentsDamageCover = expectedAffectedCovers.First(x => x.DamageCode.Contains(SHIELD_CLAIM_COVER_CONTENTS)).DamageCode;
                    VerifyAgendaStepsForCover(claimData, claimNumber, contentsDamageCover);
                }

                if (claimData.DamagedCovers == AffectedCovers.FenceOnly ||
                    claimData.DamagedCovers == AffectedCovers.BuildingOnly ||
                    claimData.DamagedCovers == AffectedCovers.BuildingAndContents)
                {
                    if (claimData.FenceDamage != null)
                    {
                        var fenceDamageCover = expectedAffectedCovers.First(x => x.DamageCode.EndsWith(SHIELD_CLAIM_COVER_FENCE)).DamageCode;
                        VerifyAgendaStepsForCover(claimData, claimNumber, fenceDamageCover);
                    }

                    if (claimData.DamageType != HomeClaimDamageType.StormDamageToFenceOnly)
                    {
                        var buildingDamageCover = expectedAffectedCovers.First(x => x.DamageCode.EndsWith(SHIELD_CLAIM_COVER_BUILDING)).DamageCode;
                        VerifyAgendaStepsForCover(claimData, claimNumber, buildingDamageCover);
                    }
                }
            }
        }

        private static void VerifyAgendaStepsForCover(ClaimHome claimData, string claimNumber, string damageCover)
        {
            VerifyHomeClaimAgendaStatusInShield(claimNumber, AgendaStepNames.ClaimLodged, damageCover, Status.Current.GetDescription());

            VerifyHomeClaimAgendaStatusInShield(claimNumber, AgendaStepNames.ContentsQuoteAuthorised, damageCover, Status.Pending.GetDescription());
            if (claimData.DamageType == HomeClaimDamageType.Theft &&
            claimData.DamagedCovers == AffectedCovers.ContentsOnly)
            {
                VerifyHomeClaimAgendaStatusInShield(claimNumber, AgendaStepNames.ContentsReplacedOrPaidOut, damageCover, Status.Pending.GetDescription());
            }
            else
            {
                VerifyHomeClaimAgendaStatusInShield(claimNumber, AgendaStepNames.RepairsComplete, damageCover, Status.Pending.GetDescription());
            }

            VerifyHomeClaimAgendaStatusInShield(claimNumber, AgendaStepNames.PaymentsSettled, damageCover, Status.Pending.GetDescription());
        }

        private static void VerifyHomeClaimAgendaStatusInShield(string claimNumber, AgendaStepNames step, string damagedCover, string status)
        {
            var agendaStatus = ShieldHomeClaimDB.GetClaimHomeAgendaStatus(claimNumber, step, damagedCover);
            Reporting.IsNotNull(agendaStatus, $"agenda claim status exists for step {step} and damaged cover {damagedCover}");
            Reporting.AreEqual(step.GetDescription(), agendaStatus.Step, false);
            Reporting.AreEqual(status, agendaStatus.Status, false);
        }

        /// <summary>
        /// Retrieves all the contacts attached to the claim as a third party and
        /// verifies that their names match.
        /// </summary>
        private static void VerifyHomeClaimThirdPartyDetails(ClaimHome claim, string claimNumber)
        {
            var dbThirdParties = ShieldClaimDB.GetClaimOffenderDetails(claimNumber);

            VerifyClaimGeneral.VerifyClaimThirdPartyDetails(claimNumber, claim.ThirdParty, dbThirdParties);
        }

        private static void VerifyHomeClaimContentsAndValuables(ClaimHome claim, string claimNumber)
        {
            var itemsInClaim = ShieldHomeClaimDB.GetContentsItemsFromClaim(claimNumber);
           // TODO AUNT-190 finalise the validation on damaged items
           // var expectedItemCount = 0;
            if (claim.TheftDamage != null && claim.TheftDamage.StolenItems != null)
            {
                // TODO AUNT-190 finalise the validation on damaged item
                //expectedItemCount = claim.TheftDamage.StolenItems.Count;
            }
            else if (claim.DamageType == HomeClaimDamageType.StormAndTempest &&
                     claim.ContentsDamage.StormDamagedItems != null)
            {
                // TODO AUNT-190 finalise the validation on damaged item
                //expectedItemCount = claim.ContentsDamage.StormDamagedItems.Count;
            }
            // TODO AUNT-190 finalise the validation on damaged item
            // if claim.DamageType not equal to HomeClaimDamageType.AccidentalDamage and  
            // claim.SpecifiedItemDamage.SpecifiedItems != null
            // add expectedItemCount 

            foreach (var itemInClaim in itemsInClaim)
            {
                ContentItem expectedItem;
                if (claim.DamageType == HomeClaimDamageType.Theft)
                {
                    expectedItem = claim.TheftDamage.StolenItems.First(x => x.Description == itemInClaim.Description);
                }
                else
                {
                    expectedItem = claim.ContentsDamage.StormDamagedItems.First(x => x.Description == itemInClaim.Description);
                }
                Reporting.AreEqual(expectedItem.Value, itemInClaim.Value, $"the value (${expectedItem.Value}) of {expectedItem.Description} matches the value provided when claim was lodged.");
            }

        }

        /// <summary>
        /// verify input field error messages for first accordion prelim claim questions
        /// </summary>
        /// <param name="browser"></param>
        public static void VerifyErrorMessages(Browser browser)
        {
            using (var spinner = new RACSpinner(browser))
            using (var prelimPage = new ClaimPrelimHome(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: prelimPage);
                Reporting.IsTrue(prelimPage.IsSurnameValidationPresent, "Surname must be entered");
                Reporting.IsTrue(prelimPage.IsDateOfBirthValidationPresent, "Please enter a valid Date of birth.");
                Reporting.IsTrue(prelimPage.IsAddressValidationPresent, "Please enter a valid address.");
                Reporting.IsTrue(prelimPage.IsEventDateValidationPresent, $"Please enter a valid event date in the format {DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH}.");
                Reporting.IsTrue(prelimPage.IsEventTimeValidationPresent, "Please enter a valid event time.");
            }
        }

        public static void VerifyClaimStatusInPCM(Browser browser, string claimNumber, string contactId, PortfolioSummary.CLAIM_PROGRESS_STATE expectedClaimState)
        {
            Reporting.Log($"Checking claim status details for: {claimNumber}. Claim status '{expectedClaimState}'.");

            // For Claim status we verify the first 3 status help text.
            var helpTextFirst = expectedClaimState;
            var helpTextSecond = expectedClaimState;
            var helpTextThird = expectedClaimState;
            switch (expectedClaimState)
            {
                case PortfolioSummary.CLAIM_PROGRESS_STATE.Lodge:
                    helpTextSecond = PortfolioSummary.CLAIM_PROGRESS_STATE.Assess;
                    helpTextThird  = PortfolioSummary.CLAIM_PROGRESS_STATE.Repair;
                    break;
                case PortfolioSummary.CLAIM_PROGRESS_STATE.Assess:
                    helpTextSecond = PortfolioSummary.CLAIM_PROGRESS_STATE.Lodge;
                    helpTextThird  = PortfolioSummary.CLAIM_PROGRESS_STATE.Repair;
                    break;
                case PortfolioSummary.CLAIM_PROGRESS_STATE.Repair:
                    helpTextSecond = PortfolioSummary.CLAIM_PROGRESS_STATE.Lodge;
                    helpTextThird  = PortfolioSummary.CLAIM_PROGRESS_STATE.Assess;
                    break;
                case PortfolioSummary.CLAIM_PROGRESS_STATE.Complete:
                    helpTextFirst  = PortfolioSummary.CLAIM_PROGRESS_STATE.Lodge;
                    helpTextSecond = PortfolioSummary.CLAIM_PROGRESS_STATE.Assess;
                    helpTextThird  = PortfolioSummary.CLAIM_PROGRESS_STATE.Repair;
                    break;
                default:
                    Reporting.Log($"Do nothing.");
                    break;
            }

            browser.LoginMemberToPCM(contactId);
            ActionsPCM.ViewExistingClaim(browser, claimNumber);

            Reporting.Log($"Checking claim status details for: {claimNumber}.");
            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                Thread.Sleep(SleepTimes.T2SEC);
                Reporting.Log("B2C: Claim Expected to be in state: " + expectedClaimState, browser.Driver.TakeSnapshot());
                Reporting.AreEqual(expectedClaimState, pcmHomePage.ClaimCurrentStateIcon, "Checking which claim Icon is current");

                // If claim is completed, then no help text will be open by default, will need to select first help text to validate.
                if (expectedClaimState == PortfolioSummary.CLAIM_PROGRESS_STATE.Complete)
                    pcmHomePage.ClaimCurrentReferencedHelpIcon = helpTextFirst;
                ValidateClaimStatusHelpText(helpTextFirst, pcmHomePage.ClaimCurrentReferencedHelpIcon, pcmHomePage.ClaimStatusHelpTextBody);

                pcmHomePage.ClaimCurrentReferencedHelpIcon = helpTextSecond;
                ValidateClaimStatusHelpText(helpTextSecond, pcmHomePage.ClaimCurrentReferencedHelpIcon, pcmHomePage.ClaimStatusHelpTextBody);

                pcmHomePage.ClaimCurrentReferencedHelpIcon = helpTextThird;
                ValidateClaimStatusHelpText(helpTextThird, pcmHomePage.ClaimCurrentReferencedHelpIcon, pcmHomePage.ClaimStatusHelpTextBody);
            }
            browser.CloseBrowser();
        }
        private static void ValidateClaimStatusHelpText(PortfolioSummary.CLAIM_PROGRESS_STATE expectedState, PortfolioSummary.CLAIM_PROGRESS_STATE actualState, string actualHelpText)
        {
            string expectedText = null;
            switch (expectedState)
            {
                case PortfolioSummary.CLAIM_PROGRESS_STATE.Lodge:
                    expectedText = "Once you have lodged your claim, one of our claims specialists will review it to complete the lodgement process.";
                    break;
                case PortfolioSummary.CLAIM_PROGRESS_STATE.Assess:
                    expectedText = "Our service provider will arrange a time with you to review the damage to your building and/or contents. We'll review your contents claim as required. Please ensure all requested documentation has been forwarded to us.";
                    break;
                case PortfolioSummary.CLAIM_PROGRESS_STATE.Repair:
                    expectedText = "Our service provider will arrange a time with you to carry out the repairs. We'll also arrange repair or replacement of your contents as required.";
                    break;
                default:
                    Reporting.Log($"Do nothing.");
                    break;
            }

            Reporting.AreEqual(expectedState, actualState, "which claim Icon is pointed to by the help text bubble.");
            Reporting.AreEqual(expectedText, actualHelpText, "claim status help text");
        }
    }
}
