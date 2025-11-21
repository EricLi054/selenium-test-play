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
using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace Tests.ActionsAndValidations
{
    public static class ActionsClaimHome
    {

        /// <summary>
        /// Triage the home claim based on damage types and what has been damaged
        /// </summary>
        /// <param name="detailedUiChecking">Flag to indicate if we should check the text on UI elements during this test</param>
        public static void TriageHomeClaim(Browser browser, ClaimHome claim, bool detailedUiChecking = false)
        {
            using (var b2cSpinner = new RACSpinner(browser))
            using (var authPage = new Authentication(browser))
            using (var homeTriageApp = new BuildingAndContents(browser))
            {
                b2cSpinner.WaitForSpinnerToFinish();
                authPage.WaitForADLoginPageOrDesiredLandingPage(homeTriageApp);
                browser.PercyScreenCheck(DividingFenceClaim.TriageEventType);
                Reporting.Log("Capturing Triage Home Claim 'What are you claiming for?' state before continuing.", browser.Driver.TakeSnapshot());

                if (detailedUiChecking)
                {
                    homeTriageApp.DetailedUiChecking();
                }

                switch (claim.DamageType)
                {
                    case HomeClaimDamageType.StormAndTempest:
                    case HomeClaimDamageType.StormDamageToFenceOnly:
                        homeTriageApp.ClickStormDamage();
                        TriageStormClaim(browser, claim, detailedUiChecking);
                        break;
                    default:
                        homeTriageApp.ClickOtherDamage();
                        break;
                }
            }
        }

        /// <summary>
        /// Triage the storm damage claim based on damage types.
        /// Please note that these pages are actually part of the Home General Claims application 
        /// rather than the Triage application.
        /// </summary>
        /// <param name="detailedUiChecking">Flag to indicate if we should check the text on UI elements during this test</param>
        public static void TriageStormClaim(Browser browser, ClaimHome claim, bool detailedUiChecking = false)
        {
            using (var stormDamageClaim = new StormDamageClaim(browser))
            using (var otherSideofDamageFence = new OtherSideOfDamageFence(browser))
            using (var sparkSpinner = new SparkSpinner(browser))
            {
                sparkSpinner.WaitForSpinnerToFinish();

                if (detailedUiChecking)
                {
                    stormDamageClaim.DetailedUiChecking();
                }

                if ((claim.DamageType == HomeClaimDamageType.StormDamageToFenceOnly
                  || claim.DamageType == HomeClaimDamageType.StormAndTempest)
                 && 
                    (claim.DamagedCovers == AffectedCovers.FenceOnly
                  || claim.DamagedCovers == AffectedCovers.BuildingOnly
                  || claim.DamagedCovers == AffectedCovers.BuildingAndFence
                  || claim.DamagedCovers == AffectedCovers.ContentsOnly
                  || claim.DamagedCovers == AffectedCovers.ContentsAndFence
                  || claim.DamagedCovers == AffectedCovers.BuildingAndContents
                  || claim.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence))
                {
                    {
                        switch (claim.DamagedCovers)
                        {
                            case AffectedCovers.FenceOnly:
                                stormDamageClaim.CheckMyFenceCheckbox();
                                break;
                            case AffectedCovers.BuildingOnly:
                                stormDamageClaim.CheckMyBuildingCheckbox();
                                break;
                            case AffectedCovers.ContentsOnly:
                                stormDamageClaim.CheckMyContentsCheckbox();
                                break;
                            case AffectedCovers.BuildingAndContents:
                                stormDamageClaim.CheckMyBuildingCheckbox();
                                stormDamageClaim.CheckMyContentsCheckbox();
                                break;
                            case AffectedCovers.BuildingAndFence:
                                stormDamageClaim.CheckMyBuildingCheckbox();
                                stormDamageClaim.CheckMyFenceCheckbox();
                                break;
                            case AffectedCovers.ContentsAndFence:
                                stormDamageClaim.CheckMyContentsCheckbox();
                                stormDamageClaim.CheckMyFenceCheckbox();
                                break;
                            case AffectedCovers.BuildingAndContentsAndFence:
                                stormDamageClaim.CheckMyBuildingCheckbox();
                                stormDamageClaim.CheckMyContentsCheckbox();
                                stormDamageClaim.CheckMyFenceCheckbox();
                                break;
                            default:
                                Reporting.Error($"Invalid Damaged Covers value for {claim.DamagedCovers.GetDescription()}");
                                break;
                        }
                    browser.PercyScreenCheck(DividingFenceClaim.TriageFenceOnly);
                    Reporting.Log("Capturing Triage Storm Claim 'Tell us what's been damaged' state before continuing.", browser.Driver.TakeSnapshot());
                    stormDamageClaim.ClickNextButton();
                    sparkSpinner.WaitForSpinnerToFinish();
                    
                    if (claim.DamagedCovers == AffectedCovers.FenceOnly
                     || claim.DamagedCovers == AffectedCovers.BuildingAndFence
                     || claim.DamagedCovers == AffectedCovers.ContentsAndFence
                     || claim.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
                        {
                            if (claim.FenceDamage.IsDividingFence)
                            {
                                otherSideofDamageFence.ClickCheckboxForDividingFence();
                                browser.PercyScreenCheck(DividingFenceClaim.TriageSharedFence);
                            }
                            else
                            {
                                otherSideofDamageFence.ClickCheckboxForNonDividingFence();
                                browser.PercyScreenCheck(DividingFenceClaim.TriageNonSharedFence);
                            }
                            Reporting.Log("Capturing Triage Storm Claim 'What sort of fence has been been damaged?' state before continuing.", browser.Driver.TakeSnapshot());
                            otherSideofDamageFence.ClickNextButton();
                            sparkSpinner.WaitForSpinnerToFinish();
                        }
                    }
                }
            }
        } 


        /// <summary>
        /// Complete the Preliminary claim questions for a home claim.
        /// Sets event date time, handles PH verification and
        /// selects the damage type being claimed for.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claim"></param>
        /// <param name="findPolicyByPolicyNumber">flag to indicate if we're finding policy by policy number, or by asset details. Ignored if claim started from PCM</param>
        public static void HomeClaimCompletePrelimDetails(Browser browser, ClaimHome claim, bool findPolicyByPolicyNumber)
        {
            using (var prelimPage = new ClaimPrelimHome(browser))
            using (var page2      = new ClaimHomePage2DamageEventDetails(browser))
            {
                prelimPage.WaitForPage();
                prelimPage.FillFormForFindMyPolicy(claim, findPolicyByPolicyNumber);

                prelimPage.WaitForDamageTypeQuestionsToBecomeVisible();

                prelimPage.FillFormForDamageTypeAndInitialEventDetails(claim);
                prelimPage.SubmitPreliminaryDetailsAndNavigateDetailsOverrideWarning(nextPage: page2);
            }
        }

        /// <summary>
        /// This method is to support triggering a submit of the first accordion without filling any values 
        /// to ensure that all the field validations are available
        /// </summary>
        /// <param name="browser"></param>
        public static void ClickOnContinueAndFieldValidation(Browser browser)
        {
            using (var prelimPage = new ClaimPrelimHome(browser))
            {
                prelimPage.WaitForPage();
                prelimPage.ClickContinueButton();
            }
            VerifyClaimHome.VerifyErrorMessages(browser);
        }

        public static void HomeClaimCompletePage2Details(Browser browser, ClaimHome claim)
        {
            using (var spinner = new RACSpinner(browser))
            using (var page2 = new ClaimHomePage2DamageEventDetails(browser))
            using (var confirmationPage = new ClaimHomePageConfirmation(browser))
            {
                BasePage expectedNextPage = null;  // Unknown as it depends on claim scenario
                page2.FillEventDetails(claim);
                Reporting.Log("damage and event details completed.", browser.Driver.TakeSnapshot());

                if (claim.DamageType != HomeClaimDamageType.StormDamageToFenceOnly)
                {
                    // B2C-4493 - For fence only claim the upload accordion is removed therefore instead of 
                    // continue button displayed it is the submit claim 
                    page2.ClickContinueBtn();
                }
 

                if (claim.DamageType != HomeClaimDamageType.ImpactOfVehicle &&
                    claim.DamageType != HomeClaimDamageType.MaliciousDamage &&
                    claim.DamageType != HomeClaimDamageType.Theft)
                {
                    // We'll be navigating the upload documents accordion
                    // for simpler home claim types without 3rd parties.
                    Thread.Sleep(SleepTimes.T2SEC);
                    page2.SubmitPage2ClaimForm();

                    // Fence only claims will go through their own
                    // series of pages, so will leave it to them to
                    // detect if their expected next page is present.
                    if (claim.DamageType != HomeClaimDamageType.StormDamageToFenceOnly)
                    {
                        expectedNextPage = confirmationPage;
                    }
                }

                // If "expectedNextPage" is null, then We just
                // wait for the spinner to clear, as
                // the next page will either be repair costs,
                // confirmation, or the 3rd party details page
                spinner.WaitForSpinnerToFinish(nextPage: expectedNextPage);
            }
        }

        public static void HomeClaimCompleteWitnessAndThirdPartyDetails(Browser browser, ClaimHome claim)
        {
            using (var spinner = new RACSpinner(browser))
            using (var page3   = new ClaimHomePage3OtherParties(browser))
            using (var confirmation = new ClaimHomePageConfirmation(browser))
            {
                page3.WaitForPage(WaitTimes.T5SEC);
                page3.EnterWitnessInformation(claim);
                page3.EnterThirdPartyInformation(claim);
                Reporting.Log("Witness and Third Party details completed.");

                page3.UploadDocumentationAndSubmitClaim(claim);
                spinner.WaitForSpinnerToFinish(nextPage: confirmation);
            }
        }

        //TODO: AUNT-163 - This method can be removed.
        /// <summary>
        /// Applicable for Fence-Only claims. Requires the calling test
        /// logic to determine whether it should be called.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claim"></param>
        public static void HomeClaimProcessOnlineSettlement(Browser browser, ClaimHome claim)
        {
            using (var spinner = new RACSpinner(browser))
            using (var page3 = new ClaimHomePage3OnlineSettlement(browser))
            using (var pageConfirmation = new ClaimHomePageConfirmation(browser))
            {
                page3.WaitForPage(WaitTimes.T5SEC);
                page3.ProcessOfferedSettlement(claim);
                spinner.WaitForSpinnerToFinish(nextPage: pageConfirmation);
            }
        }

        /// <summary>
        /// Click the "My Insurance" button from the claim confirmation screen.
        /// If the user is logged in, will take them to the claims status of
        /// the just lodged claim.
        /// </summary>
        /// <param name="browser"></param>
        public static void HomeClaimReturnToMyInsurance(Browser browser)
        {
            using (var pageConfirmation = new ClaimHomePageConfirmation(browser))
            using (var pagePCM = new PortfolioSummary(browser))
            {
                pageConfirmation.ClickMyInsuranceButton();
                pagePCM.WaitForPage();
            }
        }

        /// <summary>
        /// Opens an existing home claim in Shield UI, and add damage
        /// for a specified contents item. Only applicable for claims
        /// still in Lodged state.
        /// </summary>
        /// <param name="homeGeneralClaimsToggle">The state of the Home General Claims toggle in this Shield environment will define whether some new fields in the claims questionnaires will require setting.</param>
        public static void HomeClaimShieldAddSCPVDamage(Browser browser, string claimNumber, string homeGeneralClaimsToggle = "false")
        {
            browser.OpenShieldAndLogin();

            using (var pageHome = new ShieldSearchPage(browser))
            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))            
            using (var pageDamagesAndAgenda = new TreeItemDamagesQuestionnairesAgenda(browser))
            {
                // TODO AUNT-224 are we able to expect/support QuickSearch in Shield? (i.e. pageHome.QuickSearch(ShieldSearchPage.QuickSearchType.ClaimByClaimNumber, claimNumber);)
                pageHome.SlowSearchByClaimNumber(claimNumber);
                pageClaimDetails.WaitForPage();

                pageClaimDetails.ClickUpdateClaim();

                // Edit Home claim from the dependency tree
                pageClaimDetails.DependenciesTree.ActOnPolicyholdersInsuredAsset(DependenciesTree.Action.Edit);

                pageClaimDetails.DependenciesTree.ClickAddDamagedItemsFromPolicy();
                pageClaimDetails.AcknowledgeValidationErrorDialog();
                pageDamagesAndAgenda.WaitForPage();

                pageDamagesAndAgenda.CurrentTab = TreeItemDamagesQuestionnairesAgenda.CLAIM_TABS.Questionnaires;
                Reporting.Log($"Capturing state of Questionnaires tab on arrival, expecting 'Is the member providing their own report?' " +
                    $"question will not be answered", browser.Driver.TakeSnapshot());
                pageDamagesAndAgenda.SelectMemberProvidingOwnReport(No);
                
                Reporting.Log("Shield: About to click 'Next' after providing missing questionnaire answers, capturing screen.", browser.Driver.TakeSnapshot());
                pageDamagesAndAgenda.ClickEndDamages();
                pageClaimDetails.DependenciesTree.WaitForUpdateClaimMode();

                pageClaimDetails.DependenciesTree.ActOnPolicyholdersInsuredAsset(DependenciesTree.Action.Edit);
                pageClaimDetails.DependenciesTree.ClickAddDamagedItemsFromPolicy();
                pageClaimDetails.DependenciesTree.SelectDamagedItem();
                pageClaimDetails.ClickSelect();

                Reporting.Log("Shield: About to click 'End Damages' after adding damaged items.", browser.Driver.TakeSnapshot());
                pageDamagesAndAgenda.ClickEndDamages();
                
                pageClaimDetails.FinishChangesAndDismissHomeClaimsClassificationDialog();
                pageClaimDetails.WaitForFinishButtonToDisappear();
                pageClaimDetails.WaitForPage();
            }

            browser.LogoutShieldAndCloseBrowser();
        }
        
        /// <summary>
        /// Opens an existing home claim in Shield UI, and assigns a claim handler.
        /// Essentially moves a basic motor claim from LODGE to ASSESS claim states.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testConfig"></param>
        /// <param name="claimNumber"></param>
        public static void HomeClaimShieldAssignClaimsHandler(Browser browser, string claimNumber)
        {
            browser.OpenShieldAndLogin();

            using (var pageHome = new ShieldSearchPage(browser))
            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))
            {
                // TODO AUNT-224 are we able to expect/support QuickSearch in Shield? 
                pageHome.SlowSearchByClaimNumber(claimNumber);
                pageClaimDetails.WaitForPage();

                pageClaimDetails.ClickUpdateClaim();

                // NOTE: The 'Administrator' user is chosen as it is known to always be present
                // in Shield instances. While there are many contacts in Shield who actually have
                // the role of claim handler, it is thought to be risky to rely on them always being there.
                pageClaimDetails.DependenciesTree.ClaimHandler = "Claims Assist";
                Reporting.Log("Shield: About to click 'Finish Update' after updating Claim Handler.", browser.Driver.TakeSnapshot());
                pageClaimDetails.FinishChangesAndDismissHomeClaimsClassificationDialog();
                pageClaimDetails.WaitForFinishButtonToDisappear();

                pageClaimDetails.WaitForPage();
            }

            browser.LogoutShieldAndCloseBrowser();
        }
        
        /// <summary>
        /// Opens an existing home claim in Shield UI and adds a liability reserve to
        /// as though to authorise a quote. This is to move the claim into the
        /// REPAIR claim status.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testConfig"></param>
        /// <param name="claimNumber"></param>
        public static void HomeClaimShieldAuthoriseQuote(Browser browser, string claimNumber)
        {
            browser.OpenShieldAndLogin();

            using (var pageHome = new ShieldSearchPage(browser))
            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))
            {
                // TODO AUNT-224 are we able to expect/support QuickSearch in Shield? 
                pageHome.SlowSearchByClaimNumber(claimNumber);
                pageClaimDetails.WaitForPage();

                pageClaimDetails.ClickUpdateClaim();

                // Need to change to use an index (or fetch a list of text) as the IWebElement goes stale.
                var damageCoversRowNames = pageClaimDetails.DependenciesTree.GetListOfDamageCoversForProperty(HomeClaimDamageType.StormAndTempest);
                foreach (var damageCoverName in damageCoversRowNames)
                {
                    pageClaimDetails.DependenciesTree.EditAssetDamageTypeByName(damageCoverName);
                    AddLiabilityCover(browser, claimNumber);
                    pageClaimDetails.DependenciesTree.WaitForUpdateClaimMode();
                }
                pageClaimDetails.ClickFinish();
                pageClaimDetails.WaitForFinishButtonToDisappear();

                // Log out of Shield
                pageClaimDetails.WaitForPage();
            }

            browser.LogoutShieldAndCloseBrowser();
        }

        /// <summary>
        /// Opens an existing home claim in Shield UI and under its agenda, marks it
        /// as invoiced received. This is to emulate completed repairs on the
        /// home and content. Essentially moves the claim to COMPLETE stage.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testConfig"></param>
        /// <param name="claimNumber"></param>
        public static void HomeClaimShieldAcceptRepairerInvoice(Browser browser, string claimNumber)
        {
            browser.OpenShieldAndLogin();

            using (var pageHome = new ShieldSearchPage(browser))
            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))
            using (var pageDamagesAndAgenda = new TreeItemDamagesQuestionnairesAgenda(browser))
            {
                // TODO AUNT-224 are we able to expect/support QuickSearch in Shield? 
                pageHome.SlowSearchByClaimNumber(claimNumber);
                pageClaimDetails.WaitForPage();

                // Edit home damages from the dependency tree
                pageClaimDetails.CurrentTab = ShieldClaimDetailsPage.CLAIM_TABS.DependenciesTree;
                pageClaimDetails.DependenciesTree.ActOnPolicyholdersInsuredAsset(DependenciesTree.Action.Edit);
                pageDamagesAndAgenda.WaitForPage();

                pageDamagesAndAgenda.CurrentTab = TreeItemDamagesQuestionnairesAgenda.CLAIM_TABS.Agenda;
                pageDamagesAndAgenda.WaitForPage();

                // Update Agenda
                pageDamagesAndAgenda.CurrentTab = TreeItemDamagesQuestionnairesAgenda.CLAIM_TABS.Agenda;
                Reporting.Log("Shield: Logging screenshot of Agenda Steps before they are manually updated.", browser.Driver.TakeSnapshot());
                pageDamagesAndAgenda.EditAgendaStep(TreeItemDamagesQuestionnairesAgenda.AGENDA_STEPS.ItemsReplacedOrPaidOut,
                                                    AgendaStepStatus.Done,
                                                    TreeItemDamagesQuestionnairesAgenda.AGENDA_STATUS_REASON.StepCompleted);
                Thread.Sleep(SleepTimes.T5SEC);
                pageDamagesAndAgenda.EditAgendaStep(TreeItemDamagesQuestionnairesAgenda.AGENDA_STEPS.RepairsComplete,
                                                    AgendaStepStatus.Done,
                                                    TreeItemDamagesQuestionnairesAgenda.AGENDA_STATUS_REASON.StepCompleted);
                pageDamagesAndAgenda.WaitForPage(WaitTimes.T5SEC);
                Reporting.Log("Shield: About to click 'End Damages' after updating agenda steps.", browser.Driver.TakeSnapshot());
                pageDamagesAndAgenda.ClickEndDamages();
                pageDamagesAndAgenda.WaitForEndDamagesButtonToDisappear();

                pageClaimDetails.WaitForPage();
            }

            browser.LogoutShieldAndCloseBrowser();
        }

        private static void AddLiabilityCover(Browser browser, string claimNumber)
        {
            using (var pageDamageDetails = new DamageDetails(browser))
            using (var pageNewLiabilityReserve = new AddLiabilityReserve(browser))
            {
                pageDamageDetails.WaitForPage();

                pageDamageDetails.AddLiabilityReserve();
                pageNewLiabilityReserve.WaitForPage();

                pageNewLiabilityReserve.Reason = AddLiabilityReserve.AUTHORISED_QUOTE;
                pageNewLiabilityReserve.ClickOK();
                pageDamageDetails.WaitForPage();

                pageDamageDetails.ClickOK();
            }
        }
    }
}
