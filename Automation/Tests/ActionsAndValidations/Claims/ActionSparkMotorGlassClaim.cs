using Rac.TestAutomation.Common;
using UIDriver.Pages;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.Claim.Motor;
using UIDriver.Pages.Spark.Claim.Motor.Glass;
using UIDriver.Pages.Spark.Claim.Triage;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace Tests.ActionsAndValidations.Claims
{
    public static class ActionSparkMotorGlassClaim
    {

        /// <summary>
        /// Launches the test launch page, sets the environment settings 
        /// Select Claim Triage and Login as the claimaint represented by the ContactId parameter
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claim"></param>
        public static void OpenMotorTriagePageAndLogin(Browser browser, ClaimCar claim)
        {
            LaunchPage.OpenSparkMotorcycleLandingPage(browser);
            LaunchPage.OpenSparkTestLaunchPage(browser, SparkTestLaunchPage.Constants.ExternalSites.TriageMotor);
            using (var sparkTestLaunchPage = new SparkTestLaunchPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                if (claim.LoginWith == LoginWith.ContactId)
                {
                    sparkTestLaunchPage.LoginAsContact(claim.Claimant.Id);
                }
                else
                {
                    sparkTestLaunchPage.LoginWithPolicy(claim.Policy.PolicyNumber, claim.Claimant.Id);
                }
                spinner.WaitForSpinnerToFinish();
            }
        }

        public static void LodgeMotorGlassClaim(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            BeforeYouStart(browser);
            ConfirmContactDetails(browser, claim, detailUiChecking);
            /// Your policy page will be displayed
            /// when user login with the contact id 
            /// and they have more than 1 motor policy
            if (claim.LoginWith == LoginWith.ContactId && claim.LinkedMotorPoliciesForClaimant.Count > 1)
            {
                SelectPolicy(browser, claim);
            }
            StartYourClaim(browser, claim);
            YourGlassRepair(browser, claim);
            ReviewAndSubmitClaim(browser, claim);
            VerifyConfirmationPage(browser, claim);
        }


        /// <summary>
        /// Triage Motor claim
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="damageType"></param>
        public static void TriageMotorClaim(Browser browser, MotorClaimDamageType damageType)
        {
            using (var triageMotorClaim = new CarInsuranceClaim(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: triageMotorClaim);
                browser.PercyScreenCheck(ClaimMotorGlass.Triage);

                if (damageType == MotorClaimDamageType.WindscreenGlassDamage)
                {
                    triageMotorClaim.ClickWindowGlassDamage();
                }
                else
                {
                    triageMotorClaim.ClickOtherDamage();
                }
                
                triageMotorClaim.ClickNext();
                spinner.WaitForSpinnerToFinish();
                LaunchPage.SetSparkEnvironmentBannerToggle(browser);
            }
        }

        /// <summary>
        /// Before You Start page
        /// Click Next
        /// </summary>
        /// <param name="browser"></param>    
        public static void BeforeYouStart(Browser browser)
        {
            using (var beforeYouStart = new BeforeYouStart(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: beforeYouStart);
                browser.PercyScreenCheck(ClaimMotorGlass.BeforeYouStart);
                
                beforeYouStart.VerifyText();
                beforeYouStart.ClickNext();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Select your policy
        /// Click Next
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claim"></param>    
        public static void SelectPolicy(Browser browser, ClaimCar claim)
        {
            using (var yourPolicy = new SparkYourPolicy(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: yourPolicy);
                yourPolicy.SelectPolicy(claim);
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Arrive at the "Contact details" page and verify existing (masked) details for mobile telephone and private email address are correct and then assert 
        /// that they are correct to continue.
        /// 
        /// Please note that this page could display home or work telephone numbers if a mobile number were not available
        /// but if a mobile number is present it is displayed preferentially. The way we gather candidate data for these 
        /// tests means we always expect a mobile number to be present.
        /// </summary>
        public static void ConfirmContactDetails(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            using (var contactDetailsPage = new SparkContactDetails(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: contactDetailsPage);
                if (claim.Claimant.PrivateEmail == null)
                {
                    Reporting.Log($"Informant on this claim does not have a private email address on record so we will " +
                        $"display an alternative version of 'Contact details' advising they must provide it to proceed online." +
                        $"At this point in time, reaching this version of the page means that this test will fail after evaluating" +
                        $"the page details.");
                    SparkEmailContactDetailMissing(browser, claim.Claimant.FirstName);
                }

                Reporting.LogMinorSectionHeading($"Full Multi-factor Authentication for contact detail changes via myRAC have not been implemented " +
                    $"to test automation for this type of claim. This test will opt to continue without any changes to member information.");

                if (detailUiChecking)
                {
                    contactDetailsPage.DetailedUiChecking();
                }
                browser.PercyScreenCheck(ClaimMotorCollision.ContactDetails, contactDetailsPage.GetPercyIgnoreCSS());
                contactDetailsPage.VerifyExistingContactDetails(claim.Claimant.FirstName, claim.Claimant.MobilePhoneNumber, claim.Claimant.PrivateEmail?.Address);

                contactDetailsPage.AssertDetailsAreCorrect();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Arrive at the version of the "Contact details" page displayed when no email is on file with new 
        /// MFA changes so the member has no option but to go to myRAC and provide email details.
        /// </summary>
        /// <param name="firstName">The name of the claimant informant generated for this test.</param>
        public static void SparkEmailContactDetailMissing(Browser browser, string firstName)
        {
            using (var contactDetailsEmailMissing = new SparkEmailContactDetailMissing(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(WaitTimes.T30SEC, nextPage: contactDetailsEmailMissing);
                Reporting.Log($"Capturing the state of this page on arrival", browser.Driver.TakeSnapshot());

                contactDetailsEmailMissing.DetailedUiChecking(firstName);

                contactDetailsEmailMissing.ClickAddEmail();
                spinner.WaitForSpinnerToFinish();
                Reporting.Error("Unable to continue with this test until we introduce logging into myRAC to these tests.");
            }
        }

        /// <summary>
        /// Let's start your claim
        /// Verify the policy details
        /// Select damage date and click Next
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claim"></param>    
        public static void StartYourClaim(Browser browser, ClaimCar claim)
        {
            using (var startYourClaim = new StartYourClaim(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: startYourClaim);
                startYourClaim.CapturePercyScreenShot();
                startYourClaim.VerifyCarModel(claim);
                startYourClaim.SelectEventDate(claim.EventDateTime);
                startYourClaim.ClickNext();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Your Glass Damage
        /// Verify the Claim number
        /// Select damage fixing option and click Next
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claim"></param>    
        public static void YourGlassRepair(Browser browser, ClaimCar claim)
        {
            using (var yourGlassRepair = new YourGlassRepairs(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: yourGlassRepair);
                yourGlassRepair.SelectGlassFixedOption(claim);
                spinner.WaitForSpinnerToFinish();
            }
        }


        /// <summary>
        /// Review and Submit Claim
        /// Verify Claim Details and Submit Claim
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claim"></param>    
        public static void ReviewAndSubmitClaim(Browser browser, ClaimCar claim)
        {
            using (var reviewAndSubmitClaim = new ReviewAndSubmitClaim(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: reviewAndSubmitClaim);
                reviewAndSubmitClaim.VerifyReviewAndSubmitClaim(claim);
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Verify details on Confirmation page        
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claim"></param>    
        public static void VerifyConfirmationPage(Browser browser, ClaimCar claim)
        {
            using (var confirmationPage = new Confirmation(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: confirmationPage);
                confirmationPage.VerifyConfirmationPage(claim);
            }
        }

        /// <summary>
        /// Click Submit Invoice button on Confirmation page    
        /// </summary>        
        public static void ClickSubmitInvoice(Browser browser)
        {
            using (var confirmationPage = new Confirmation(browser))
            {
                confirmationPage.ClickSubmitInvoice();
            }
        }
    }
}
