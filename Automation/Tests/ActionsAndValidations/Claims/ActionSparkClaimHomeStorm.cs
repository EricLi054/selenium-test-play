using Rac.TestAutomation.Common;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.Claim.Home;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.VisualTest;
using UIDriver.Pages.Spark.Claim.Home.DividingFence;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.General;
using UIDriver.Pages.MyRAC;
using UIDriver.Pages.MyRACRegisterAnAccount;
using UIDriver.Pages.MicrosoftAD;
using UIDriver.Pages;

namespace Tests.ActionsAndValidations.Claims
{
    public static class ActionSparkClaimHomeStorm
    {
        /// <summary>
        /// Determine whether to report a claim from the MyRAC login page (and if so facilitate that) 
        /// or jump directly to the Home Triage Claim URL.
        /// 
        /// We can only report a claim from MyRAC if the environment is shieldint2 or shielduat6 and we
        /// don't want to bother with the MyRAC login if the test does not call for a change to contact 
        /// details in myRAC later in the flow.
        /// 
        /// If the member has multiple policies then we will log in to myRAC but then begin the claim flow 
        /// from the triage page via so we can include the 'Policy Picker' page in the test.
        /// </summary>
        public static void ReportClaimFrom(Browser browser, ClaimHome claim)
        {
            if (Config.Get().MyRAC.IsMyRACSupportExpected()
                && (claim.IsEmailAddressChanged || claim.IsMobileNumberChanged))
            {
                OpenMyRACURL(browser);
                AttemptMyRACLogin(browser, claim);
                
                if (claim.LoginWith == LoginWith.ContactId && claim.LinkedHomePolicies.Count > 1)
                {
                    LaunchPage.OpenHomeTriageClaimURL(browser, claim);
                }
                else
                {
                    ReportClaimHomeStormFromMyRAC(browser, claim);
                }
            }
            else
            {
                LaunchPage.OpenHomeTriageClaimURL(browser, claim);
            }
        }

        public static void OpenMyRACURL(Browser browser)
        {
            string environmentMyRACURL = null;
            var environment = Config.Get().Shield.Environment;

            if (environment == "shieldint2")
            {
                Reporting.Log($"Setting login URL for {environment}.");
                environmentMyRACURL = MyRACURLs.LoginSIT;
            }
            else if (environment == "shielduat6")
            {
                Reporting.Log($"Setting login URL for {environment}.");
                environmentMyRACURL = MyRACURLs.LoginUAT;
            }
            else
            {
                Reporting.Error($"Environment {environment} is not supported for this test, " +
                    $"must be integrated with Member Central NPE.");
            }
            
            browser.OpenUrl(environmentMyRACURL);

            using (var myRACLogin = new MyRACLogin(browser))
            {
                using (var authPage = new Authentication(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(myRACLogin);
                    spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                }
            }
        }

        /// <summary>
        /// This method attempts login to myRAC and if login is not successful then it will attempt to register a new myRAC account.
        /// 
        /// Note: Password is pulled from MyRAC.Pwd in the config file/variable groups as we want to have a consistent password when creating
        /// accounts so both automated and manual testers can log in to accounts created, but it is easy to change. 
        /// </summary>
        /// <param name="claim">Contains the Claimant.LoginEmail and other details which may be required for a new myRAC account registration</param>
        /// <param name="detailedUiChecking">Optional flag to determine if detailed UI checks are executed or not</param>
        public static void AttemptMyRACLogin(Browser browser, ClaimHome claim, bool detailedUiChecking = false)
        {
            using (var myRACLogin = new MyRACLogin(browser))
            using (var registerMyRACAccount = new MyRACRegisterAnAccount(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(WaitTimes.T30SEC, nextPage: myRACLogin);
                Reporting.Log($"Capturing the state of this page on arrival", browser.Driver.TakeSnapshot());
                
                string password = Config.Get().MyRAC.Pwd;
                myRACLogin.InputCredentials(claim.Claimant.LoginEmail, password, detailedUiChecking);
                myRACLogin.SubmitCredentials();

                if (myRACLogin.DetectUnsuccessfulLogin())
                {
                    registerMyRACAccount.RegisterAccount(claim.Claimant, claim.PolicyDetails.PolicyNumber, password);
                }
                
                myRACLogin.VerifySuccessfulLogin(claim.PolicyDetails.PolicyNumber);
            }
        }

        public static void ReportClaimHomeStormFromMyRAC(Browser browser, ClaimHome claim, bool detailedUiChecking = false)
        {
            using (var myRACLogin = new MyRACLogin(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                myRACLogin.ReportHomeStormClaimFromMyRAC(claim.PolicyDetails.PolicyNumber, claim);
            }
        }

        /// <summary>
        /// Reusable flow to report a new Storm Damage claim.
        /// 
        /// REGARDING STORM DAMAGE TO FENCE ONLY CLAIMS 
        /// If the member has a quote for repairs: 
        ///  - we don't gather information about the damaged fence as we will see the quote.
        /// 
        /// If the member has already had repairs done: 
        ///  - we don't gather information about the damages fence as we will see the invoice
        ///  - we don't ask about a temporary fence if the repairs are complete there is no need 
        /// for a temporary fence/make-safe.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ReportSparkClaimHomeStorm(Browser browser, ClaimHome claim, bool retryOTP = false, bool detailedUiChecking = false)
        {
            BeforeYouStart(browser);
            string shieldEnvironment = Config.Get().Shield.Environment;
            bool useMyRACLogin = Config.Get().MyRAC.IsMyRACSupportExpected();

            if (string.IsNullOrEmpty(claim.Claimant.PrivateEmail.Address))
            {
                Reporting.Log($"Informant on this claim does not have a private email address on record so we will " +
                    $"display an alternative version of 'Contact details' advising they must provide it to proceed online.");
                SparkEmailContactDetailMissing(browser, claim);
            }
            else
            {
                ConfirmContactDetails(browser, claim);
            }
            
            
            if(claim.LoginWith == LoginWith.ContactId && claim.LinkedHomePolicies.Count > 1)
            {
                Reporting.Log($"As {claim.LinkedHomePolicies.Count} policies exist for this claimant, handling the 'Your policy' selection page.");
                YourPolicyPicker(browser, claim);
            }
            else
            {
                Reporting.Log($"This claimant either logged in with an existing policy context or does not have multiple policies qualifying for Home Storm Claim, so we don't expect to see the 'Your policy' selection page.");
            }
            
            SelectEventDateAndContinue(browser, claim);
            
            ExcessAndExistingRepairs(browser, claim, detailedUiChecking);

            

            if (claim.DamagedCovers == AffectedCovers.BuildingOnly
                || claim.DamagedCovers == AffectedCovers.BuildingAndContents
                || claim.DamagedCovers == AffectedCovers.BuildingAndFence
                || claim.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
            {
                StormDamageToBuilding(browser, claim, detailedUiChecking);

                // The specific damage to building page is only displayed to the user if Level One
                // is the only damage indicated for building on this claim. Otherwise an assessor
                // will cover all of that.
                if (claim.BuildingDamage.IsSpecificItemsDamaged
                && !claim.BuildingDamage.IsHomeBadlyDamaged
                && !claim.BuildingDamage.IsHomeUnsafe)
                {
                    StormDamageBuildingDetails(browser, claim, detailedUiChecking);
                }

                // TODO: INSU-818: Once gone live, we can remove the toggle and treat as always true.
                if (Config.Get().IsClaimHomeMoreAboutYourDamageScreenEnabled())
                { MoreAboutYourDamage(browser, claim, detailedUiChecking); }

                //Note: These are the EXTERNAL_CODE values passed by the Shield API.
                //Relevant ShieldDB Query: SELECT external_code, * FROM T_PROPERTY_OCCUPATION_TYPE WHERE external_code in ('O','I','T','H','C')
                if (claim.PolicyDetails.HomeAsset.OccupancyType == "I"
                || claim.PolicyDetails.HomeAsset.OccupancyType == "T")
                {
                    StormWaterDamageToFixturesAndFittings(browser, claim, detailedUiChecking); 
                }

                StormWaterDamageToHomeownerBuilding(browser, claim, detailedUiChecking); 

                MoreSafetyChecks(browser, claim, detailedUiChecking);
            }

            if (claim.DamagedCovers == AffectedCovers.ContentsOnly
             || claim.DamagedCovers == AffectedCovers.ContentsAndFence)
            {
                StormDamageToContents(browser, claim, detailedUiChecking);
            }

            // The specific damage to Contents page is only displayed to the user if there is
            // no damage to carpets indicated on this claim. Otherwise an assessor will cover
            // all of that.
            if ((claim.DamagedCovers == AffectedCovers.ContentsOnly
                || claim.DamagedCovers == AffectedCovers.ContentsAndFence
                || claim.DamagedCovers == AffectedCovers.BuildingAndContents
                || claim.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
                && claim.ContentsDamage.IsOtherStormDamagedContents)
            {
                StormDamagedContentsList(browser, claim, detailedUiChecking);
            }

            if (claim.DamagedCovers == AffectedCovers.FenceOnly
             || claim.DamagedCovers == AffectedCovers.BuildingAndFence
             || claim.DamagedCovers == AffectedCovers.ContentsAndFence
             || claim.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
            {
                if (claim.DamagedCovers != AffectedCovers.FenceOnly)
                {
                    Reporting.Log($"Now let's look at your fence!");
                    NowLetsLookAtYourFence(browser, claim, detailedUiChecking);
                }
                
                switch (claim.ExpectedOutcome)
                {
                    case ExpectedClaimOutcome.AlreadyHaveRepairQuote:
                    case ExpectedClaimOutcome.RepairsCompleted:
                        break;
                    default:
                        StormDamageToFenceDetails(browser, claim);
                        break;
                }

                if (claim.ExpectedOutcome != ExpectedClaimOutcome.RepairsCompleted)
                {
                    AnswerSafetyAndSecurityQuestion(browser, claim);
                }

                if (claim.ExpectedOutcome == ExpectedClaimOutcome.RepairsCompleted || claim.ExpectedOutcome == ExpectedClaimOutcome.AlreadyHaveRepairQuote)
                {
                    Reporting.Log($"Expected Outcome = {claim.ExpectedOutcome.ToString()} so Upload documents is expected.");
                    UploadDocuments(browser, claim);
                }
            }

            ReviewYourClaim(browser, claim, detailedUiChecking);

            if (claim.DamagedCovers == AffectedCovers.FenceOnly)
            {
                /// <summary>
                /// Call fence settlement calculator API and check the total repair cost
                /// </summary>
                if (claim.EligibilityForOnlineSettlement == SettleFenceOnline.Eligible)
                {
                    var response = DataHelper.GetFenceSettlementBreakdownCost(claim.ClaimNumber);
                    claim.IsRepairCostLessThanExcess = response.TotalRepairCost < 0;

                    claim.FenceSettlementBreakdown.TotalRepairCost = response.TotalRepairCost;
                    claim.FenceSettlementBreakdown.NumberOfMetresClaimed = response.NumberOfMetresClaimed;
                    claim.FenceSettlementBreakdown.RepairCostBeforeExcess = response.SubTotalBeforeExcess;
                    claim.FenceSettlementBreakdown.RepairCostBeforeAdjustments = response.SubTotalBeforeAdjustments;
                    claim.FenceSettlementBreakdown.DisposalContribution = response.DisposalContribution;
                    claim.FenceSettlementBreakdown.AsbestosInspectionFee = response.AsbestosInspectionFee;
                    claim.FenceSettlementBreakdown.PaintingCost = response.PaintingCost;
                    claim.FenceSettlementBreakdown.DividingFenceAdjustment = response.DividingFenceAdjustment;
                    claim.FenceSettlementBreakdown.CurrentExcess = response.CurrentExcess;
                }

                if (claim.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails ||
                    claim.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementRepairByRAC ||
                    claim.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementAcceptWithExistingBankDetails ||
                    claim.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails ||
                    claim.ExpectedOutcome == ExpectedClaimOutcome.GetRepairQuoteFirst ||
                    claim.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementContactMe ||
                    claim.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementTakeMoreTimeToDecide)
                {
                    ClaimSettlementOptions(browser, claim);

                    if (claim.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails)
                    {
                        EnterYourBankDetails(browser, claim, detailedUiChecking);
                        ActionMFA.RequestAndEnterOTP(browser, claim.Claimant.MobilePhoneNumber, retryOTP, detailedUiChecking);
                    }
                    if (claim.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementAcceptWithExistingBankDetails ||
                        claim.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails)
                    {
                        EnterYourBankDetails(browser, claim, detailedUiChecking);
                    }
                }
            }

            VerifyConfirmationMessage(browser, claim);

            Reporting.LogTestShieldValidations($"Home Storm {claim.DamagedCovers.GetDescription()} Claim", claim.ClaimNumber);
            VerifySparkStormClaim.VerifyStormClaimDetailsInShield(claim, browser);

            if (useMyRACLogin 
                && (claim.IsEmailAddressChanged || claim.IsMobileNumberChanged))
                {
                using (var myRACLogin = new MyRACLogin(browser))
                {
                    myRACLogin.LogOutMyRAC(browser);
                }
            }
        }

        /// <summary>
        /// Applicable only for the Spark 
        /// Click Continue button on the Before You Start page
        /// </summary>
        /// <param name="browser"></param>        
        public static void BeforeYouStart(Browser browser)
        {
            using (var beforeYouStart = new BeforeYouStart(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: beforeYouStart);
                LaunchPage.SetSparkEnvironmentBannerToggle(browser);
                browser.PercyScreenCheck(DividingFenceClaim.BeforeYouStart);
                beforeYouStart.ClickNext();
            }
        }

        /// <summary>
        /// Arrive at the "Contact details" page and either:
        /// 1) confirm existing (masked) details for mobile telephone and private email address are correct and then assert 
        /// that they are correct to continue.
        /// 2) select Update to change the telephone number and/or email address on record via myRAC before returning and continuing.
        ///    (OPTION 2 only available in environments with inegrated Member Central NPE).
        /// 
        /// Please note that this page could display home or work telephone numbers if a mobile number were not available
        /// but if a mobile number is present it is displayed preferentially. The way we gather candidate data for these 
        /// tests means we always expect a mobile number to be present.
        /// 
        /// NOTE: Unlike MFA when adding bank account details etc, we don't include the optional retryOTP param in this method as we 
        /// won't intentionally provide an invalid code for MFA when updating contact details in myRAC.
        /// </summary>
        /// <param name="claim">The test data generated for this test.</param> 
        /// <param name="detailUiChecking">Optional parameter; if TRUE we check UI elements like field labels etc rather than focusing only on bare functional test actions</param>
        public static void ConfirmContactDetails(Browser browser, ClaimHome claim, bool detailUiChecking = false)
        {
            using (var contactDetailsPage = new SparkContactDetails(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                string shieldEnvironment = Config.Get().Shield.Environment;
                spinner.WaitForSpinnerToFinish(nextPage: contactDetailsPage);
                Reporting.Log($"Capturing contact details state on arrival", browser.Driver.TakeSnapshot());

                if ((shieldEnvironment == "shieldint2" || shieldEnvironment == "shielduat6")) 
                {
                    //TODO: DED-958 - update this logging to state that we can test this when DED-958 is resolved and remove reference to that story.
                    Reporting.LogMinorSectionHeading($"Full Multi-factor Authentication changes to contact details won't be tested in this run against " +
                        $"{shieldEnvironment} pending resolution of <a href=\"https://rac-wa.atlassian.net/browse/DED-958\">DED-958</a> or similar which " +
                        $"allows us to know the whether OTP is being bypassed or not.");
                }
                else
                {
                    Reporting.LogMinorSectionHeading($"Full Multi-factor Authentication for changes to contact details cannot be tested in this run " +
                        $"as the test environment ('{shieldEnvironment}') cannot support it.");
                }
                
                if (detailUiChecking)
                {
                    contactDetailsPage.DetailedUiChecking();
                }
                contactDetailsPage.VerifyExistingContactDetails(claim.Claimant.FirstName, claim.Claimant.MobilePhoneNumber, claim.Claimant.PrivateEmail?.Address);

                //TODO: DED-958 - restore the "shieldint2 OR shielduat6" condition for the list of environments where we can test this.
                if ((shieldEnvironment == "NoSuchEnvironmentExistsForNow") && 
                   (claim.IsMobileNumberChanged       || claim.IsEmailAddressChanged))
                {
                    contactDetailsPage.UpdateMyDetailsInMyRAC();
                    contactDetailsPage.HandleTelephoneNumberWarningDialog();
                    spinner.WaitForSpinnerToFinish();

                    SparkUpdateContactViaMyRac(browser, claim);
                }

                contactDetailsPage.AssertDetailsAreCorrect();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Arrive at the version of the "Contact details" page displayed when no email is on file with new 
        /// MFA changes so the member has no option but to go to myRAC and provide email details.
        /// </summary>
        /// <param name="claim">The test data generated for this test.</param>
        public static void SparkEmailContactDetailMissing(Browser browser, ClaimHome claim)
        {
            using (var contactDetailsEmailMissing = new SparkEmailContactDetailMissing(browser))
            using (var contactDetailsSpark = new SparkContactDetails(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                string shieldEnvironment = Config.Get().Shield.Environment;
                bool useMyRACLogin = Config.Get().MyRAC.IsMyRACSupportExpected();
                spinner.WaitForSpinnerToFinish(WaitTimes.T30SEC, nextPage: contactDetailsEmailMissing);
                Reporting.Log($"Capturing the state of this page on arrival", browser.Driver.TakeSnapshot());

                contactDetailsEmailMissing.DetailedUiChecking(claim.Claimant.FirstName);
                
                contactDetailsEmailMissing.ClickAddEmail();
                contactDetailsSpark.HandleTelephoneNumberWarningDialog();
                spinner.WaitForSpinnerToFinish();

                if (useMyRACLogin &&
                   (shieldEnvironment == "shieldint2" || shieldEnvironment == "shielduat6"))
                {
                    SparkUpdateContactViaMyRac(browser, claim);
                    ConfirmContactDetails(browser, claim);
                }
                else
                {
                    Reporting.Error($"This contact has no email, but the only way to update it is via myRAC. Configuration of " +
                        $"'Use myRAC Login' = '{useMyRACLogin}' when testing against Shield environment '{shieldEnvironment}' " +
                        $"so test is unable to proceed.");
                }
            }
        }

        /// <summary>
        /// This method drives handling the MFA prompts and accessing the editable contact detail fields
        /// in myRAC.
        /// </summary>
        public static void SparkUpdateContactViaMyRac(Browser browser, ClaimHome claim)
        {
            using (var myRACUpdateContactInformation = new MyRACUpdateContactInformation(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                Reporting.Log($"Capturing the state of this page on arrival", browser.Driver.TakeSnapshot());
                
                myRACUpdateContactInformation.ClickEditDetails();
                ActionMFA.RequestAndEnterOTP(browser, claim.Claimant.MobilePhoneNumber, isMyRACChangeDetails: true);

                myRACUpdateContactInformation.EditMyRACFields(claim);
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Arrive at the "Policy picker" page and select the policy to claim against
        /// Click Next
        /// </summary>
        /// <param name="claim">The test data generated for this test.</param> 
        public static void YourPolicyPicker(Browser browser, ClaimHome claim)
        {
            using (var policyPickerPage = new YourPolicyPicker(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: policyPickerPage);
                
                policyPickerPage.VerifyHomePolicyCardIsDisplayed(browser, claim);
                policyPickerPage.SelectPolicy(browser, claim);
                policyPickerPage.SelectNext();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Verify the property details like address, cover type and policy number
        /// Select the event date and click Continue
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claim"></param>
        public static void SelectEventDateAndContinue(Browser browser, ClaimHome claim)
        {
            using (var startYourClaim = new StartClaimEventDateTime(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                startYourClaim.WaitForPage();

                //Take the snapshot when the page is open
                //Then Click on the Continue button so,
                //the mandatory field validation errors are
                //visible and then take the snapshot again
                browser.PercyScreenCheck(DividingFenceClaim.StartYourClaim);
                startYourClaim.ClickContinueButton();
                browser.PercyScreenCheck(DividingFenceClaim.StartYourClaimErrorPage);

                startYourClaim.VerifyPropertyDetails(browser, claim);
                startYourClaim.SelectEventDate(claim);
                startYourClaim.InputTimeAsText(claim.EventDateTime);

                startYourClaim.ClickContinueButton();
                spinner.WaitForSpinnerToFinish();
                startYourClaim.CheckForExistingClaimContent();
                spinner.WaitForSpinnerToFinish();
            }
            
        }

        /// <summary>
        /// Verify content regarding Excess for the claim and if it is a Storm Damage to Fence Only claim ask
        /// member whether they already have a quote for repairs or an invoice for already-completed repairs.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ExcessAndExistingRepairs(Browser browser, ClaimHome claim, bool detailedUiChecking = false)
        {
            using (var excessAndExistingRepairs = new ExcessAndExistingRepairs(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                excessAndExistingRepairs.WaitForPage();
                excessAndExistingRepairs.RecordClaimNumber(claim);
                if (detailedUiChecking)
                {
                    excessAndExistingRepairs.VerifyDetailedContent(claim);
                }
                if(claim.DamagedCovers == AffectedCovers.FenceOnly)
                {
                    excessAndExistingRepairs.RepairsOrQuote(claim);
                    excessAndExistingRepairs.VerifyNotificationCardContent(claim);
                }
                excessAndExistingRepairs.VerifyExcessContent(claim);
                excessAndExistingRepairs.ClickNext();
            }
        }

        /// <summary>
        /// Ask member whether they already have a quote for repairs or an invoice for already-completed repairs on 
        /// their fence for claims where Fence damage is not the only damage present.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void NowLetsLookAtYourFence(Browser browser, ClaimHome claim, bool detailedUiChecking = false)
        {
            using (var nowLetsLookAtYourFence = new NowLetsLookAtYourFence(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                nowLetsLookAtYourFence.WaitForPage();

                if (detailedUiChecking)
                {
                    nowLetsLookAtYourFence.VerifyDetailedContent(claim);
                }
                nowLetsLookAtYourFence.RepairsOrQuote(claim);
                nowLetsLookAtYourFence.VerifyNotificationCardContent(claim);
                nowLetsLookAtYourFence.ClickNext();
            }
        }

        /// <summary>
        /// Fill the damaged fence details, i.e, fence type, damaged length, sides
        /// and click Continue
        /// </summary>
        /// <param name="browser"></param>       
        /// <param name="claim"></param>
        private static void StormDamageToFenceDetails(Browser browser, ClaimHome claim, bool detailedUiChecking = false)
        {
            using (var yourDamagedFence = new YourDamagedFence(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                yourDamagedFence.WaitForPage();

                //Take the snapshot when the page is open
                //Then Click on the Continue button so,
                //the mandatory field validation errors are
                //visible and then take the snapshot again
                browser.PercyScreenCheck(DividingFenceClaim.YourDamageFence);
                yourDamagedFence.ClickContinueButton();
                browser.PercyScreenCheck(DividingFenceClaim.YourDamageFenceErrorPage);

                yourDamagedFence.FillFenceDamageDetails(claim, detailedUiChecking);
                spinner.WaitForSpinnerToFinish();
            }
        }

        private static void StormDamageToBuilding(Browser browser, ClaimHome claim, bool detailedUiChecking)
        {
            using (var buildingStormDamage = new DamagedBuilding(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: buildingStormDamage);
                buildingStormDamage.WaitForPage();
                if (detailedUiChecking)
                {
                    buildingStormDamage.DetailedUiCheckingOfDamageLevels();
                }
                buildingStormDamage.AnswerBuildingDamageLevel(browser, claim);
                Reporting.Log($"Selecting Next to continue to next page.");
                buildingStormDamage.ClickToContinue();
                spinner.WaitForSpinnerToFinish();
            }
        }

        public static void StormDamageBuildingDetails(Browser browser, ClaimHome claim, bool detailedUiChecking)
        {
            using (var buildingStormDamageDetails = new DamagedBuildingDetails(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                buildingStormDamageDetails.WaitForPage();
                spinner.WaitForSpinnerToFinish(nextPage: buildingStormDamageDetails);
                if (detailedUiChecking)
                {
                    buildingStormDamageDetails.DetailedUiCheckingOfBuildingDamageTypes();
                }
                buildingStormDamageDetails.SelectSpecificDamages(claim);
                Reporting.Log($"Capturing page before selecting Next to continue to next page.", browser.Driver.TakeSnapshot());
                buildingStormDamageDetails.ClickToContinue();
                spinner.WaitForSpinnerToFinish();
            }
        }

        public static void MoreAboutYourDamage(Browser browser, ClaimHome claim, bool detailedUiChecking)
        {
            using (var moreAboutYourDamage = new MoreAboutYourBuildingDamage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                moreAboutYourDamage.WaitForPage();
                if (detailedUiChecking)
                {
                    moreAboutYourDamage.DetailedUiCheckingOfBuildingDamageFreeTextPage(claim.ClaimNumber);
                }
                moreAboutYourDamage.InputFreeText(claim.AccountOfEvent);
                moreAboutYourDamage.ClickToContinue();
                spinner.WaitForSpinnerToFinish();
            }
        }

        public static void StormWaterDamageToHomeownerBuilding(Browser browser, ClaimHome claim, bool detailedUiChecking)
        {
            using(var buildingWaterDamage = new StormWaterDamageToBuilding(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                buildingWaterDamage.WaitForPage();
                if (detailedUiChecking)
                {
                    buildingWaterDamage.DetailedUiChecksOfWaterDamage(claim);
                }
                buildingWaterDamage.WaterDamageBuilding(claim);
                buildingWaterDamage.ClickToContinue();
                spinner.WaitForSpinnerToFinish();
            }
        }

        public static void StormWaterDamageToFixturesAndFittings(Browser browser, ClaimHome claim, bool detailedUiChecking)
        {
            using (var buildingFixtures = new StormFixturesAndFittingsDamageToInvestmentProperty(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                buildingFixtures.WaitForPage();
                if (detailedUiChecking)
                {
                    buildingFixtures.DetailedUiChecksFixturesAndFittings();
                }
                buildingFixtures.WaterDamageFixturesAndFittings(claim);
                buildingFixtures.ClickToContinue();
                spinner.WaitForSpinnerToFinish();
            }
        }

        public static void MoreSafetyChecks(Browser browser, ClaimHome claim, bool detailedUiChecking)
        {
            using (var safetyChecks = new StormSafetyChecks(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                safetyChecks.WaitForPage();
                if (detailedUiChecking)
                {
                    safetyChecks.DetailedUiCheckingSafetyChecks();
                }
                safetyChecks.AnswerSafetyChecks(claim);
                safetyChecks.ClickToContinue();
                spinner.WaitForSpinnerToFinish();
            }
        }

        private static void StormDamageToContents(Browser browser, ClaimHome claim, bool detailedUiChecking)
        {
            using (var contentStormDamage = new DamagedContents(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: contentStormDamage);
                contentStormDamage.WaitForPage();
                contentStormDamage.AnswerContentsDamageQuestions(browser, claim, detailedUiChecking);
                spinner.WaitForSpinnerToFinish();
            }
        }

        public static void StormDamagedContentsList(Browser browser, ClaimHome claim, bool detailedUiChecking)
        {
            using (var contentsList = new DamagedContentsList(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: contentsList);
                contentsList.WaitForPage();
                contentsList.AreAnyOtherItemsDamaged(browser, claim, detailedUiChecking);
                contentsList.ContinueToNextPage();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Answer the safety and security question
        /// and click Continue
        /// </summary>
        /// <param name="browser"></param>       
        /// <param name="claim"></param>
        public static void AnswerSafetyAndSecurityQuestion(Browser browser, ClaimHome claim)
        {
            using (var safetyAndSecurity = new SafetyAndSecurity(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                safetyAndSecurity.WaitForPage();

                //Take the snapshot when the page is open
                //Then Click on the Continue button so,
                //the mandatory field validation errors are
                //visible and then take the snapshot again
                browser.PercyScreenCheck(DividingFenceClaim.SafetyAndSecurity);
                safetyAndSecurity.ClickContinueButton();
                browser.PercyScreenCheck(DividingFenceClaim.SafetyAndSecurityErrorPage);

                safetyAndSecurity.AnswerTemporaryFenceQuestionAndContinue(claim);
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        public static void UploadDocuments(Browser browser, ClaimHome claim, bool detailUiChecking = false)
        {
            using (var uploadDocuments = new UploadDocuments(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                uploadDocuments.WaitForPage();

                if (detailUiChecking)
                {
                    uploadDocuments.VerifyDetailedContent(claim);
                }

                uploadDocuments.ClickNext();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Review the details on the "Review your claim" page
        /// and click Submit Claim to proceed.
        /// </summary>
        public static void ReviewYourClaim(Browser browser, ClaimHome claim, bool detailUiChecking = false)
        {
            using (var reviewYourClaim = new ReviewYourClaim(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                reviewYourClaim.WaitForPage();
                Reporting.Log($"Capture page on landing.", browser.Driver.TakeSnapshot());
                if (detailUiChecking)
                {
                    reviewYourClaim.VerifyDetailedContent(claim);
                }

                reviewYourClaim.ConfirmAndSubmitClaim();
                //TODO - NRIINS-205 - We should return this wait period to the default 90 seconds when environment performance improves.
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
            }
        }

        /// <summary>
        /// Verify fence breakdown cost
        /// and select settlement option
        /// </summary>
        public static void ClaimSettlementOptions(Browser browser, ClaimHome claim)
        {
            using (var yourClaimSettlementOptions = new YourClaimSettlementOptions(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                yourClaimSettlementOptions.WaitForPage();

                //Take the snapshot when the page is open
                browser.PercyScreenCheck(DividingFenceClaim.YourSettlementOptions);

                if(claim.FenceDamage.IsDividingFence)
                {
                    yourClaimSettlementOptions.VerifySettlementBreakdownCostAndSelectAnOption(claim);
                    spinner.WaitForSpinnerToFinish();
                }
                else
                {
                    yourClaimSettlementOptions.VerifyNonDividingSettlementOption(claim);
                    spinner.WaitForSpinnerToFinish();

                    if(claim.ExpectedOutcome != ExpectedClaimOutcome.OnlineSettlementRepairByRAC)
                    {
                        yourClaimSettlementOptions.VerifySettlementBreakdownCostAndSelectAnOption(claim);
                        spinner.WaitForSpinnerToFinish();
                    }
                }
            }
        }

        /// <summary>
        /// Enter the bank details and
        /// Click Submit button
        /// </summary>
        /// <param name="browser"></param>       
        /// <param name="claim"></param>
        public static void EnterYourBankDetails(Browser browser, ClaimHome claim, bool detailUiChecking = false)
        {
            using (var bankDetail = new YourBankDetails(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                bankDetail.WaitForPage();
                Reporting.Log($"Capturing initial screen state", browser.Driver.TakeSnapshot());
                browser.PercyScreenCheck(DividingFenceClaim.YourBankDetails);

                bankDetail.TriggerFieldValidation(browser);

                if (detailUiChecking)
                {
                    bankDetail.EnterInvalidNoMatchBSBAndCheckErrorMessage(payMethod: null);
                }

                bankDetail.VerifySettlementAmountOnBankDetailsPage(claim.FenceSettlementBreakdown.TotalRepairCost);
                switch (claim.ExpectedOutcome)
                {
                    case ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails:

                        bankDetail.EnterBankDetails(claim.AccountForSettlement);
                        bankDetail.VerifyBSBDetails(claim.AccountForSettlement);
                        break;
                    case ExpectedClaimOutcome.OnlineSettlementAcceptWithExistingBankDetails:
                        bankDetail.SelectBankDetailOnRecord(claim.AccountForSettlement);
                        break;
                    case ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails:
                        bankDetail.ClickDontHaveBankDetailsCheckbox();
                        break;
                    default:
                        bankDetail.EnterBankDetails(claim.AccountForSettlement);
                        bankDetail.VerifyBSBDetails(claim.AccountForSettlement);
                        break;
                }
                Reporting.Log("Your Bank Details Page", browser.Driver.TakeSnapshot());
                bankDetail.ClickSubmitButton();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Verify messages on Confirmation Page
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claim"></param>
        public static void VerifyConfirmationMessage(Browser browser, ClaimHome claim)
        {
            using (var confirmation = new Confirmation(browser))            
            {
               confirmation.WaitForPage();

                //Take the snapshot when the page is open
                browser.PercyScreenCheck(DividingFenceClaim.Confirmation);

               confirmation.VerifyConfimationMessage(claim);
            }
        }
    }
}
