using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UIDriver.Pages.Claims;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using UIDriver.Pages.PCM;


namespace Tests.ActionsAndValidations
{
    public static class VerifyClaimMotor
    {
        /// <summary>
        /// Verify key information on the B2C claim confirmation screen. If a repairer
        /// is assigned for certain conditions, then we'll expect the repairer
        /// information to be shown on screen.
        /// </summary>
        /// <param name="claimData">test data used to guide test expectations.</param>
        public static string VerifyClaimConfirmationScreen(Browser browser, ClaimCar claimData)
        {
            return VerifyClaimConfirmationScreen(browser, claimData, isRepairerAssertMandatory: true);
        }

        /// <summary>
        /// Verify key information on the B2C claim confirmation screen. If a repairer
        /// is assigned for certain conditions, then we'll expect the repairer
        /// information to be shown on screen.
        /// 
        /// Some cases though won't get a repairer assigned if the quotas in Shield
        /// NPE have been exceeded through heavy testing, and so we allow a flag for
        /// those tests to indicate that they are not concerned if a repairer was not
        /// set.
        /// </summary>
        /// <param name="claimData">test data used to guide test expectations.</param>
        /// <param name="isRepairerAssertMandatory">if false, we will check if Shield assigned a repairer and base our UI assertions on that. If true, we will use rules coded in this method to determine whether we expect a repairer to be allocated.</param>
        public static string VerifyClaimConfirmationScreen(Browser browser, ClaimCar claimData, bool isRepairerAssertMandatory)
        {
            var claimNumber = FetchClaimNumberFromB2CConfirmationScreen(browser);          
            Reporting.Log($"Confirmation page shown.", browser.Driver.TakeSnapshot());

            VerifyClaimConfirmationPageNoRepairer(browser, claimData);

            return claimNumber;
        }

        private static string VerifyClaimConfirmationPageNoRepairer(Browser browser, ClaimCar claimData)
        {
            string claimNumber = null;

            using (var confirmation = new BaseClaimsConfirmation(browser))
            {
                claimNumber = confirmation.ClaimNumber;

                Reporting.AreEqual(claimData.Claimant.FirstName, confirmation.DetailsPHFirstName, ignoreCase: true);
                Reporting.AreEqual(claimData.Claimant.Surname,   confirmation.DetailsPHLastName,  ignoreCase: true);
                Reporting.AreEqual(claimData.Policy.PolicyNumber, confirmation.DetailsPolicyNumber, ignoreCase: false);
            }
            return claimNumber;
        }

        private static string FetchClaimNumberFromB2CConfirmationScreen(Browser browser)
        {
            string claimNumber = null;

            using (var confirmation = new BaseClaimsConfirmation(browser))
            { claimNumber = confirmation.ClaimNumber; }

            return claimNumber;
        }

        public static void VerifyMotorClaimInshield(string claimNumber, ClaimCar claim)
        {
            Reporting.Log($"Verify details of claim {claimNumber} in Shield database.");
            VerifyMotorClaimAgendaStatus(claimNumber, claim);
            VerifyClaimGeneral.VerifyClaimWitnessDetails(claimNumber, claim.Witness);
            VerifyMotorClaimThirdPartyDetails(claimNumber, claim);
            VerifyMotorClaimScenarioDetails(claimNumber, claim);
        }

        /// <summary>
        /// Verifies the 4 agenda steps for a new B2C motor claim. Assumes that the claim
        /// has not yet been updated in Shield, and hence only in the lodged state.
        /// 
        /// Currently this method also assumes that the claim was allocated with a
        /// Authorised Repair centre (hence "Quote Authorised" = Not Applicable).
        /// 
        /// This may need to be reworked as we bring across other claims scenarios.
        /// </summary>
        /// <param name="claimNumber"></param>
        public static void VerifyMotorClaimAgendaStatus(string claimNumber, ClaimCar claim)
        {
            Reporting.Log("================================================================");
            Reporting.Log($"Verify Agenda Step Claim Lodged for Claim: {claimNumber}");
            var agendaStatus = ShieldMotorClaimDB.GetClaimAgendaStatus(claimNumber, AgendaStepNames.ClaimLodged);
            var expectedDamageCode = GetDamageCodeFromTypeAndScenario(claim);

            Reporting.AreEqual(expectedDamageCode, agendaStatus.DamageCode);
            Reporting.AreEqual(AgendaStepNames.ClaimLodged.GetDescription(), agendaStatus.Step);
            Reporting.AreEqual(AgendaStepStatus.Current.GetDescription(), agendaStatus.Status);

            Reporting.Log("================================================================");
            Reporting.Log($"Verify Agenda Step Quote Authorised for Claim: {claimNumber}");
            agendaStatus = ShieldMotorClaimDB.GetClaimAgendaStatus(claimNumber, AgendaStepNames.MotorQuoteAuthorised);

            Reporting.AreEqual(expectedDamageCode, agendaStatus.DamageCode, "expected damage code");
            Reporting.AreEqual(AgendaStepNames.MotorQuoteAuthorised.GetDescription(), agendaStatus.Step, "expected status of Agenda step");
            Reporting.AreEqual(AgendaStepStatus.Pending.GetDescription(), agendaStatus.Status, "expected status of Agenda step");

            Reporting.Log("================================================================");
            Reporting.Log($"Verify Agenda Step Invoice Received for Claim: {claimNumber}");
            agendaStatus = ShieldMotorClaimDB.GetClaimAgendaStatus(claimNumber, AgendaStepNames.InvoiceReceived);

            Reporting.AreEqual(expectedDamageCode, agendaStatus.DamageCode, "expected damage code");
            Reporting.AreEqual("Motor - Invoice Received or Claim Paid Out", agendaStatus.Step, "expected Agenda step name");
            Reporting.AreEqual(AgendaStepStatus.Pending.GetDescription(), agendaStatus.Status, "expected status of Agenda step");           

            Reporting.Log("================================================================");
            Reporting.Log($"Verify Agenda Step Payments Settled for Claim: {claimNumber}");
            agendaStatus = ShieldMotorClaimDB.GetClaimAgendaStatus(claimNumber, AgendaStepNames.PaymentsSettled);

            Reporting.AreEqual(expectedDamageCode, agendaStatus.DamageCode, "expected damage code");
            Reporting.AreEqual(AgendaStepNames.PaymentsSettled.GetDescription(), agendaStatus.Step, "expected name of Agenda step");
            Reporting.AreEqual(AgendaStepStatus.Pending.GetDescription(), agendaStatus.Status, "expected status of Agenda step");
        }

        /// <summary>
        /// Retrieves all the contacts attached to the claim as a third party and
        /// verifies that their names match.
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <param name="claim"></param>
        private static void VerifyMotorClaimThirdPartyDetails(string claimNumber, ClaimCar claim)
        {
            List<ContactClaimDB> dbThirdParties;
            dbThirdParties = ShieldClaimDB.GetClaimOffenderDetails(claimNumber);

            // For motor, third parties are 'ContactClaimMotorTP' class which derives from Contact
            // but with added declarations. For this validation, we just need the base Contact properties.
            List<Contact> expectedThirdParties = claim.ThirdParty != null ? claim.ThirdParty.Select(x => (Contact)x).ToList() : null;
            VerifyClaimGeneral.VerifyClaimThirdPartyDetails(claimNumber, expectedThirdParties, dbThirdParties);
        }

        /// <summary>
        /// Verify the expected damage type and claim scenario have been recorded for
        /// this claim in Shield.
        /// </summary>
        /// <param name="claimNumber"></param>
        private static void VerifyMotorClaimScenarioDetails(string claimNumber, ClaimCar claim)
        {
            var claimDetails = ShieldClaimDB.GetClaimScenario(claimNumber);

            Reporting.Log("================================================================");
            Reporting.Log($"Verify Type and Scenario details for Claim: {claimNumber}");
            Reporting.AreEqual(MotorClaimDamageTypeNames[claim.DamageType].TextShield, claimDetails.ClaimType);
            Reporting.AreEqual(GetScenarioNameFromTypeAndScenario(claim), claimDetails.ClaimScenario);
            Reporting.AreEqual(claim.EventDateTime, claimDetails.EventDateAndTime, "event date and time have been saved correctly");

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

        private static string GetDamageCodeFromTypeAndScenario(ClaimCar claim)
        {
            string damageCodeFromScenario = MotorClaimDamageCodeAndScenarioNames[claim.DamageType].TextShield;
            return damageCodeFromScenario;
        }

        private static string GetScenarioNameFromTypeAndScenario(ClaimCar claim)
        {
            string scenarioName = MotorClaimDamageCodeAndScenarioNames[claim.DamageType].TextShield;
            return scenarioName;
        }
        
        /// <summary>
        /// Verify that Shield has correctly recorded claims questionnaire
        /// responses from claimant.
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <param name="claimData"></param>
        public static void VerifyClaimsQuestionnairesInShield(string claimNumber, ClaimCar claimData)
        {
            Reporting.LogMinorSectionHeading("Verifying motor claim questionnaires");

            var claimDetails = DataHelper.GetClaimDetails(claimNumber);

            ValidateGeneralQuestionnaires(claimData, claimDetails);
            ValidateMotorTheftQuestionnaires(claimData, claimDetails);
        }

        private static void ValidateGeneralQuestionnaires(ClaimCar claimData, GetClaimResponse shieldClaimRecord)
        {
            var actualMemberNeedsToProvideEFT = shieldClaimRecord.GetQuestionnaireLine((int)MotorClaimQuestionnaire.MemberNeedsToProvideEFT).GetAnswerIDAsMappedString();
            Reporting.AreEqual(No, actualMemberNeedsToProvideEFT, MotorClaimQuestionnaire.MemberNeedsToProvideEFT.GetDescription());

            var actualRepairsInWA = shieldClaimRecord.GetQuestionnaireLine((int)MotorClaimQuestionnaire.RepairsToCompleteInWA).GetAnswerIDAsMappedString();
            // Tests are always using suburbs in WA.
            Reporting.AreEqual(Yes, actualRepairsInWA, MotorClaimQuestionnaire.HaveRepairsBeenOrganised.GetDescription());

            var actualIsVehicleDriveable = shieldClaimRecord.GetQuestionnaireLine((int)MotorClaimQuestionnaire.IsVehicleDriveable).GetAnswerIDAsMappedString();
            var expectedIsVehicleDriveable = DataHelper.BooleanToStringYesNo(claimData.IsVehicleDriveable == true);
            if (claimData.DamageType == MotorClaimDamageType.Theft && claimData.TheftDetails.WhatWasStolen == MotorClaimTheftDetails.VehicleUnrecovered)
            { expectedIsVehicleDriveable = Unrecovered; }
            Reporting.AreEqual(expectedIsVehicleDriveable, actualIsVehicleDriveable, MotorClaimQuestionnaire.IsVehicleDriveable.GetDescription());
        }        

        private static void ValidateMotorTheftQuestionnaires(ClaimCar claimData, GetClaimResponse shieldClaimRecord)
        {
            var actualWereKeysStolen     = shieldClaimRecord.GetQuestionnaireLine((int)MotorClaimQuestionnaire.WereKeysStolen).GetAnswerIDAsMappedString();
            var actualIsVehicleFinanced  = shieldClaimRecord.GetQuestionnaireLine((int)MotorClaimQuestionnaire.IsVehicleFinanced).GetAnswerIDAsMappedString();
            var actualWasVehicleForSale  = shieldClaimRecord.GetQuestionnaireLine((int)MotorClaimQuestionnaire.WasVehicleForSale).GetAnswerIDAsMappedString();
            var actualIsVehicleRecovered = shieldClaimRecord.GetQuestionnaireLine((int)MotorClaimQuestionnaire.IsVehicleRecovered).GetAnswerIDAsMappedString();

            var expectedWereKeysStolen = DataHelper.BooleanToStringYesNo(claimData.TheftDetails.WereKeysStolen);
            Reporting.AreEqual(expectedWereKeysStolen, actualWereKeysStolen, MotorClaimQuestionnaire.WereKeysStolen.GetDescription());

            var expectedIsVehicleFinanced = DataHelper.BooleanToStringYesNo(claimData.TheftDetails.WasFinanceOwing);
            Reporting.AreEqual(expectedIsVehicleFinanced, actualIsVehicleFinanced, MotorClaimQuestionnaire.IsVehicleFinanced.GetDescription());

            var expectedWasVehicleForSale = DataHelper.BooleanToStringYesNo(claimData.TheftDetails.WasVehicleForSale);
            Reporting.AreEqual(expectedWasVehicleForSale, actualWasVehicleForSale, MotorClaimQuestionnaire.WasVehicleForSale.GetDescription());

            var expectedIsVehicleRecovered = DataHelper.BooleanToStringYesNo(claimData.TheftDetails.WhatWasStolen != MotorClaimTheftDetails.VehicleUnrecovered);
            Reporting.AreEqual(expectedIsVehicleRecovered, actualIsVehicleRecovered, MotorClaimQuestionnaire.IsVehicleRecovered.GetDescription());
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
                    helpTextThird = PortfolioSummary.CLAIM_PROGRESS_STATE.Repair;
                    break;
                case PortfolioSummary.CLAIM_PROGRESS_STATE.Assess:
                    helpTextSecond = PortfolioSummary.CLAIM_PROGRESS_STATE.Lodge;
                    helpTextThird = PortfolioSummary.CLAIM_PROGRESS_STATE.Repair;
                    break;
                case PortfolioSummary.CLAIM_PROGRESS_STATE.Repair:
                    helpTextSecond = PortfolioSummary.CLAIM_PROGRESS_STATE.Lodge;
                    helpTextThird = PortfolioSummary.CLAIM_PROGRESS_STATE.Assess;
                    break;
                case PortfolioSummary.CLAIM_PROGRESS_STATE.Complete:
                    helpTextFirst = PortfolioSummary.CLAIM_PROGRESS_STATE.Lodge;
                    helpTextSecond = PortfolioSummary.CLAIM_PROGRESS_STATE.Assess;
                    helpTextThird = PortfolioSummary.CLAIM_PROGRESS_STATE.Repair;
                    break;
                default:
                    break;
            }

            browser.LoginMemberToPCM(contactId);
            ActionsPCM.ViewExistingClaim(browser, claimNumber);

            Reporting.Log($"Checking claim status details for: {claimNumber}.");
            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                Thread.Sleep(SleepTimes.T10SEC);
                Reporting.AreEqual(expectedClaimState, pcmHomePage.ClaimCurrentStateIcon, "the selected claim Icon is the expected current claim state");

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
                    expectedText = "After lodging your claim you can obtain a quote from your allocated repairer. We’ll advise you once we have received and reviewed the quote.";
                    break;
                case PortfolioSummary.CLAIM_PROGRESS_STATE.Repair:
                    expectedText = "Once we’ve approved the quote, you can arrange a date for repairs to your vehicle with your allocated repairer. They’ll advise you when the repairs are complete.";
                    break;
                default:
                    break;
            }

            Reporting.AreEqual(expectedState, actualState, "Checking which claim Icon is pointed to by the help text bubble.");
            Reporting.AreEqual(expectedText, actualHelpText);
        }
    }
}
