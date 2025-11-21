using Rac.TestAutomation.Common;
using System.Threading;
using UIDriver.Helpers;
using UIDriver.Pages.B2C;
using UIDriver.Pages.Claims;
using UIDriver.Pages.MicrosoftAD;
using UIDriver.Pages.PCM;
using UIDriver.Pages.Shield;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.Claim.Triage;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.General;

namespace Tests.ActionsAndValidations
{
    public static class ActionsClaimMotor
    {

        /// <summary>
        /// Triage the Motor claim based on damage types
        /// </summary>
        /// <param name="browser"></param>        
        /// <param name="damageType"></param>
        public static void TriageMotorClaim(Browser browser, MotorClaimDamageType damageType)
        {
            using (var b2cSpinner = new RACSpinner(browser))
            using (var authPage = new Authentication(browser))
            using (var triageMotorClaim = new CarInsuranceClaim(browser))
            using (var carAccidentType = new OtherVehicle(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                b2cSpinner.WaitForSpinnerToFinish();
                authPage.WaitForADLoginPageOrDesiredLandingPage(triageMotorClaim);

                switch (damageType)
                {
                    case MotorClaimDamageType.WindscreenGlassDamage:
                        triageMotorClaim.ClickWindowGlassDamage();
                        triageMotorClaim.ClickNext();
                        break;
                    case MotorClaimDamageType.SingleVehicleCollision:
                    case MotorClaimDamageType.MultipleVehicleCollision:
                        triageMotorClaim.ClickCrashOrAccident();
                        triageMotorClaim.ClickNext();                        
                        break;
                    default:
                        triageMotorClaim.ClickOtherDamage();
                        triageMotorClaim.ClickNext();                        
                        break;
                }

                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Complete the Preliminary claim questions for a mtor
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claim"></param>
        /// <param name="findPolicyByPolicyNumber">flag to indicate if we're finding policy by policy number, or by asset details. Ignored if claim started from PCM</param>
        public static void MotorClaimCompletePrelimDetails(Browser browser, ClaimCar claim, bool findPolicyByPolicyNumber)
        {
            using (var prelimPage = new ClaimPrelimMotor(browser))
            using (var page2 = new ClaimMotorPage2DamageEventDetails(browser))
            {
                prelimPage.WaitForPage();
                prelimPage.FillFormForFindMyPolicy(claim, findPolicyByPolicyNumber);

                prelimPage.WaitForDamageTypeQuestionsToBecomeVisible();

                prelimPage.FillFormForDamageTypeAndInitialEventDetails(claim);
                prelimPage.SubmitPreliminaryDetailsAndNavigateDetailsOverrideWarning(nextPage: page2);
            }
        }

        public static void MotorClaimCompleteCommonQuestionnaires(Browser browser, ClaimCar claim)
        {
            MotorClaimCompletePage2Details(browser, claim);
            MotorClaimCompletePage3Details(browser, claim);
            MotorClaimCompletePage4Details(browser, claim);
        }

        public static void MotorClaimCompletePage2Details(Browser browser, ClaimCar claim)
        {
            using (var spinner = new RACSpinner(browser))
            using (var page2 = new ClaimMotorPage2DamageEventDetails(browser))
            using (var page3 = new ClaimMotorPage3DriverDetails(browser))           
            {
                page2.FillTheftDetailsAndSubmit(claim);

                Reporting.Log("Vehicle event details completed.", browser.Driver.TakeSnapshot());
                page2.SubmitPage2ClaimForm();

                spinner.WaitForSpinnerToFinish(nextPage: page3);              
            }
        }

        public static void MotorClaimCompletePage3Details(Browser browser, ClaimCar claim)
        {
            using (var spinner = new RACSpinner(browser))
            using (var page3 = new ClaimMotorPage3DriverDetails(browser))
            using (var page4 = new ClaimMotorPage4OtherParties(browser))
            {
                page3.CompleteDetailsOfDriver(claim);
                Reporting.Log("Claimant driver details completed.", browser.Driver.TakeSnapshot());
                spinner.WaitForSpinnerToFinish(nextPage: page4);
            }
        }

        public static void MotorClaimCompletePage4Details(Browser browser, ClaimCar claim)
        {
            using (var spinner = new RACSpinner(browser))
            using (var page4 = new ClaimMotorPage4OtherParties(browser))
            using (var confirmation = new BaseClaimsConfirmation(browser))
            {
                page4.EnterWitnessInformation(claim);
                Thread.Sleep(SleepTimes.T2SEC);

                page4.EnterThirdPartyInformation(claim);
                Thread.Sleep(SleepTimes.T2SEC);
                
                page4.UploadDocumentationAndSubmitClaim(claim);
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC, nextPage: confirmation);

                Reporting.Log("Witness and Third Party details completed.", browser.Driver.TakeSnapshot());

                Reporting.Log($"Final claim number is {confirmation.ClaimNumber}", browser.Driver.TakeSnapshot());
            }
        }

        /// <summary>
        /// Opens an existing claim in Shield UI and adds a liability reserve to
        /// as though to authorise a quote (even though there may be no quote
        /// actually attached to the claim). This is to move the claim into the
        /// REPAIR claim status.
        /// </summary>
        /// <param name="browser"></param>        
        /// <param name="claimNumber"></param>
        public static void MotorClaimShieldAuthoriseQuote(Browser browser, string claimNumber, MotorClaimDamageType damageType)
        {
            browser.LoginToShieldAndFindClaim(claimNumber);

            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))
            using (var pageDamageDetails = new DamageDetails(browser))
            using (var pageNewLiabilityReserve = new AddLiabilityReserve(browser))
            {
                pageClaimDetails.ClickUpdateClaim();

                pageClaimDetails.DependenciesTree.EditAssetDamageTypeByName(MotorClaimDamageCodeAndScenarioNames[damageType].TextShield);
                pageDamageDetails.WaitForPage();

                pageDamageDetails.AddLiabilityReserve();
                pageNewLiabilityReserve.WaitForPage();

                pageNewLiabilityReserve.Reason = AddLiabilityReserve.AUTHORISED_QUOTE;
                pageNewLiabilityReserve.ClickOK();
                pageDamageDetails.WaitForPage();

                pageDamageDetails.ClickOK();
                pageClaimDetails.DependenciesTree.WaitForUpdateClaimMode();
                pageClaimDetails.ClickFinish();
                pageClaimDetails.WaitForFinishButtonToDisappear();

                pageClaimDetails.WaitForPage();
            }
            browser.LogoutShieldAndCloseBrowser();
        }

        /// <summary>
        /// Opens an existing claim in Shield UI, and assigns a claim handler.
        /// Essentially moves a basic motor claim from LODGE to ASSESS claim states.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testConfig"></param>
        /// <param name="claimNumber"></param>
        public static void MotorClaimShieldAssignClaimsHandler(Browser browser, string claimNumber)
        {
            browser.LoginToShieldAndFindClaim(claimNumber);

            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))
            {
                pageClaimDetails.ClickUpdateClaim();

                // NOTE: The 'Administrator' user is chosen as it is known to always be present
                // in Shield instances. While there are many contacts in Shield who actually have
                // the role of claim handler, it is thought to be risky to rely on them always being there.
                pageClaimDetails.DependenciesTree.ClaimHandler = "Claims Assist";
                pageClaimDetails.ClickFinish();
                pageClaimDetails.WaitForFinishButtonToDisappear();
                pageClaimDetails.WaitForPage();
            }
            browser.LogoutShieldAndCloseBrowser();
        }

        /// <summary>
        /// Opens an existing claim in Shield UI and under its agenda, marks it
        /// as invoiced received. This is to emulate completed repairs on the
        /// claimant's vehicle. Essentially moves the claim to COMPLETE stage.
        /// </summary>
        /// <param name="browser"></param>      
        /// <param name="claimNumber"></param>
        public static void MotorClaimShieldAcceptRepairerInvoice(Browser browser, string claimNumber)
        {
            browser.LoginToShieldAndFindClaim(claimNumber);

            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))
            using (var pageDriverDetails = new TreeItemDriverDetails(browser))
            using (var pageDamagesAndAgenda = new TreeItemDamagesQuestionnairesAgenda(browser))
            {
                // Edit vehicle from the dependency tree
                pageClaimDetails.DependenciesTree.ActOnPolicyholdersInsuredAsset(DependenciesTree.Action.Edit);
                pageDriverDetails.WaitForPage();

                // Click Next from Driver Details (ideally we should also be checking disclosures here
                pageDriverDetails.ClickNext();
                pageDamagesAndAgenda.WaitForPage();

                // Deal with questionnaire?
                pageDamagesAndAgenda.CurrentTab = TreeItemDamagesQuestionnairesAgenda.CLAIM_TABS.Questionnaires;
                if (pageDamagesAndAgenda.IsQuestionShownActionNeededFor21DayHold())
                {
                    Reporting.Log($"Capturing state of Questionnaires tab on arrival, expecting 'Action needed for 21 day hold?' " +
                        $"question will not be answered", browser.Driver.TakeSnapshot());
                    pageDamagesAndAgenda.AnswerActionNeededFor21DayHold("Not Required");
                }

                // Update Agenda
                pageDamagesAndAgenda.CurrentTab = TreeItemDamagesQuestionnairesAgenda.CLAIM_TABS.Agenda;
                pageDamagesAndAgenda.EditAgendaStep(TreeItemDamagesQuestionnairesAgenda.AGENDA_STEPS.InvoiceReceived,
                                                    AgendaStepStatus.Done,
                                                    TreeItemDamagesQuestionnairesAgenda.AGENDA_STATUS_REASON.StepCompleted);
                pageDamagesAndAgenda.WaitForPage(WaitTimes.T5SEC);
                pageDamagesAndAgenda.ClickEndDamages();
                pageDamagesAndAgenda.WaitForEndDamagesButtonToDisappear();

                pageClaimDetails.WaitForPage();
            }

            browser.LogoutShieldAndCloseBrowser();
        }

    }
}
