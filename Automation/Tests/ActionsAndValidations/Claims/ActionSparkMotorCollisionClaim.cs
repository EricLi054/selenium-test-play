using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using System;
using System.Collections.Generic;
using System.Linq;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.Claim.Motor;
using UIDriver.Pages.Spark.Claim.Motor.Collision;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace Tests.ActionsAndValidations.Claims
{
    public static class ActionSparkMotorCollisionClaim
    {
        public static void LodgeMotorCollisionClaim(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            BeforeYouStart(browser, detailUiChecking);
            ConfirmContactDetails(browser, claim, detailUiChecking);
            /// Your policy page will be displayed
            /// when user login with the contact id 
            /// and they have more than 1 motor policy
            if (claim.LoginWith == LoginWith.ContactId && claim.LinkedMotorPoliciesForClaimant.Count > 1)
            {
                SelectPolicy(browser, claim, detailUiChecking);
            }
            StartYourClaim(browser, claim, detailUiChecking);
            AboutTheAccident(browser, claim, detailUiChecking);
            MoreAboutTheAccident(browser, claim, detailUiChecking);
            EnterWhereAndHowAccidentHappened(browser, claim, detailUiChecking);
            VerifySparkMotorCollisionClaim.VerifyInitialClaimDetailsInShield(claim);
            ChooseDriverOfYourCar(browser, claim, detailUiChecking);
            if (claim.IsQualifiedForDriverHistoryQuestionnaire) 
            { 
                EnterDriverHistory(browser, claim, detailUiChecking);
            }
            EnterThirdPartyAndPoliceDetails(browser, claim, detailUiChecking);
            EnterMoreThirdPartyDetails(browser, claim, detailUiChecking);
            EnterWitnessDetails(browser, claim, detailUiChecking);

            //About you car and where's your car page only displayed if it's not an thirdparty only claim
            if (!claim.OnlyClaimDamageToTP)
            {
                EnterCarDamageAndTowedDetails(browser, claim, detailUiChecking);
                if (claim.IsVehicleDriveable != true)
                {
                    EnterWhereYourCarDetails(browser, claim, detailUiChecking);
                }
            }

            ReviewClaimSummary(browser, claim, detailUiChecking);
        }

        /// <summary>
        /// This method fetches the available preferred repairers from Shield using the ClaimNumber and PreferredRepairerSuburb.
        /// We only display the Repairer Options page if one or more repairer is returned by the GetServiceProviders call.
        /// Otherwise we don't display the reapirer option
        /// </summary>       
        public static void AssignServiceProvider(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            // We do a general spinner wait because we don't know if we'll be prompted
            // for a choice of service providers or not. But we need to wait for the
            // update claim to complete, before we can do an API query to determine
            // which page we should be at.
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish();
            }

            var getServiceProviders = DataHelper.GetServiceProviders(claim.ClaimNumber, claim.PreferredRepairerSuburb.Suburb);

            if (claim.IsRepairerAllocationExhausted || !getServiceProviders.ServiceTypes.Any())
            {
                claim.IsRepairerAllocationExhausted = true;
                Reporting.Log($"Shield didn't returns any repairers for Claim number: {claim.ClaimNumber} and Suburb: {claim.PreferredRepairerSuburb.Suburb}");
            }
            else
            {
                var serviceProviders = getServiceProviders.ServiceTypes?.FirstOrDefault().ServiceProviders;
                RepairerOptions(browser, claim, serviceProviders, detailUiChecking);
            }
        }

        /// <summary>
        /// Before You Start page
        /// Click Next
        /// </summary>       
        public static void BeforeYouStart(Browser browser, bool detailUiChecking = false)
        {
            using (var beforeYouStart = new BeforeYouStart(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                beforeYouStart.WaitForPage();
                browser.PercyScreenCheck(ClaimMotorCollision.BeforeYouStart);
                if (detailUiChecking)
                {
                    beforeYouStart.DetailedUiChecking();
                }
                beforeYouStart.ClickNext();
                beforeYouStart.ClickStartClaimDialogBox();
            }
        }

        /// <summary>
        /// Select your policy
        /// Click Next
        /// </summary>       
        public static void SelectPolicy(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            using (var yourPolicy = new SparkYourPolicy(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                yourPolicy.WaitForPage(waitTimeSeconds: WaitTimes.T150SEC);
                if (detailUiChecking)
                {
                    yourPolicy.DetailedUiChecking();
                }
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
                Reporting.LogMinorSectionHeading($"Full Multi-factor Authentication for contact detail changes via myRAC have not been implemented " +
                    $"to test automation for this type of claim. This test will opt to continue without any changes to member information.");

                if (detailUiChecking)
                {
                    contactDetailsPage.DetailedUiChecking();
                }
                browser.PercyScreenCheck(ClaimMotorCollision.ContactDetails, contactDetailsPage.GetPercyIgnoreCSS());
                contactDetailsPage.VerifyExistingContactDetails(claim.Claimant.FirstName, claim.Claimant.GetPhone(), claim.Claimant.PrivateEmail?.Address);

                contactDetailsPage.AssertDetailsAreCorrect();
            }
        }

        /// <summary>
        /// Let's start your claim
        /// Verify the policy details
        /// Select damage date and click Next
        /// </summary>        
        public static void StartYourClaim(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            using (var startYourClaim = new StartYourClaim(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                startYourClaim.WaitForPage();
                if (detailUiChecking)
                {
                    startYourClaim.DetailedUiChecking();
                }
                if (Config.Get().IsVisualTestingEnabled)
                {
                    browser.PercyScreenCheck(ClaimMotorCollision.YourPolicy, startYourClaim.GetPercyIgnoreCSS());
                    startYourClaim.ClickNext();
                    browser.PercyScreenCheck(ClaimMotorCollision.YourPolicyMandatoryFieldsValidation, startYourClaim.GetPercyIgnoreCSS());
                }
                startYourClaim.VerifyCarModel(claim);
                startYourClaim.SelectEventDate(claim.EventDateTime);
                switch (browser.DeviceName)
                {
                    case TargetDevice.MacBook:
                    case TargetDevice.Windows11:
                        startYourClaim.SelectEventTime(claim.EventDateTime);
                        break;
                    case TargetDevice.iPad10:
                        startYourClaim.SelectTimeFromTimePicker(claim.EventDateTime);
                        break;
                    case TargetDevice.GalaxyS21:
                    case TargetDevice.iPhone14:
                        startYourClaim.SelectMobileTimeClock();
                        break;
                    default:
                        throw new NotSupportedException($"{browser.DeviceName.GetDescription()} device is not supported");
                }
                startYourClaim.ClickNext();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Let's start your claim page       
        /// Select collision type and click Next
        /// </summary>        
        public static void AboutTheAccident(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            using (var aboutTheAccident = new AboutTheAccident(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                aboutTheAccident.WaitForPage();
                if (detailUiChecking)
                {
                    aboutTheAccident.DetailedUiChecking(claim);
                }
                if (Config.Get().IsVisualTestingEnabled)
                {
                    browser.PercyScreenCheck(ClaimMotorCollision.AboutTheAccident);
                    aboutTheAccident.ClickNext();
                    browser.PercyScreenCheck(ClaimMotorCollision.AboutTheAccidentMandatoryFieldsValidation);
                }
                aboutTheAccident.SelectAccidentWithAndClickNext(claim);
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// More about the accident page        
        /// Select your car travel direction and click Next
        /// </summary>        
        public static void MoreAboutTheAccident(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            using (var moreAboutTheAccident = new MoreAboutTheAccident(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                moreAboutTheAccident.WaitForPage();
                if (detailUiChecking)
                {
                    moreAboutTheAccident.DetailedUiChecking();
                }
                if (Config.Get().IsVisualTestingEnabled)
                {
                    browser.PercyScreenCheck(ClaimMotorCollision.MoreAboutTheAccident);
                    moreAboutTheAccident.ClickNext();
                    browser.PercyScreenCheck(ClaimMotorCollision.MoreAboutTheAccidentMandatoryFieldsValidation);
                }
                moreAboutTheAccident.SelectHowTheAccidentHappened(claim);
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Where and how it happened       
        /// Enter where and how accident happened and click Next
        /// </summary>        
        public static void EnterWhereAndHowAccidentHappened(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            using (var whereAndHowItHappened = new WhereAndHowItHappened(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                whereAndHowItHappened.WaitForPage();
                if (detailUiChecking)
                {
                    whereAndHowItHappened.DetailedUiChecking(claim.ClaimNumber);
                }
                if (Config.Get().IsVisualTestingEnabled)
                {
                    browser.PercyScreenCheck(ClaimMotorCollision.WhereAndHow, whereAndHowItHappened.GetPercyIgnoreCSS());
                    whereAndHowItHappened.ClickNext();
                    browser.PercyScreenCheck(ClaimMotorCollision.WhereAndHowFieldsValidation, whereAndHowItHappened.GetPercyIgnoreCSS());
                }
                whereAndHowItHappened.EnterWhereAndHowAccidentHappened(claim);
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Driver of your Car
        /// Select who was driving your car
        /// </summary>        
        public static void ChooseDriverOfYourCar(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            using (var driverOfYourCar = new DriverOfYourCar(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                driverOfYourCar.WaitForPage();
                if (detailUiChecking)
                {
                    driverOfYourCar.DetailedUiChecking();
                }
                if (Config.Get().IsVisualTestingEnabled)
                {
                    browser.PercyScreenCheck(ClaimMotorCollision.DriverOfYourCar, driverOfYourCar.GetPercyIgnoreCSS());
                    driverOfYourCar.ClickNext();
                    browser.PercyScreenCheck(ClaimMotorCollision.DriverOfYourCarFieldsValidation, driverOfYourCar.GetPercyIgnoreCSS());
                }
                driverOfYourCar.ChooseDriverOfYourCar(claim);
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Enter driving history
        /// </summary>        
        public static void EnterDriverHistory(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            using (var driverHistory = new DriverHistory(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                driverHistory.WaitForPage();
                if (detailUiChecking)
                {
                    driverHistory.DetailedUiChecking(claim.Driver.DriverDetails.FirstName, claim.IsClaimantDriver);
                }
                if (Config.Get().IsVisualTestingEnabled)
                {
                    browser.PercyScreenCheck(ClaimMotorCollision.DriverHistory, driverHistory.GetPercyIgnoreCSS());
                    driverHistory.ClickNext();
                    browser.PercyScreenCheck(ClaimMotorCollision.DriverHistoryFieldsValidation, driverHistory.GetPercyIgnoreCSS());
                }
                driverHistory.EnterDriverHistory(claim);
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Enter Third party and police details
        /// </summary>
        public static void EnterThirdPartyAndPoliceDetails(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            if ((claim.DamageType == MotorClaimDamageType.SingleVehicleCollision && claim.IsTPPropertyDamage == true) ||
                (claim.DamageType == MotorClaimDamageType.MultipleVehicleCollision))
            {
                using (var propertyOrPetOwner = new ThirdPartyDetails(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    propertyOrPetOwner.WaitForPage();
                    if (detailUiChecking)
                    {
                        propertyOrPetOwner.DetailedUiChecking(claim);
                    }
                    propertyOrPetOwner.EnterTPAndPoliceDetails(claim);
                }
            }
        }

        /// <summary>
        /// Enter More TP details, including TP insuranc company, TP claim number and TP damage details
        /// </summary>
        public static void EnterMoreThirdPartyDetails(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            if (claim.ThirdParty != null)
            {
                using (var moreThirdPartyDetails = new MoreThirdPartyDetails(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    moreThirdPartyDetails.WaitForPage();
                    if (detailUiChecking)
                    {
                        moreThirdPartyDetails.DetailedUiChecking(claim);
                    }
                    moreThirdPartyDetails.EnterMoreTPDetails(claim);
                }
            }
        }

        /// <summary>
        /// Enter witnesses details
        /// </summary>
        public static void EnterWitnessDetails(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            using (var witnesses = new Witnesses(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                witnesses.WaitForPage();
                if (detailUiChecking)
                {
                    witnesses.DetailedUiChecking();
                }
                witnesses.AddWitnesses(claim);

            }
        }

        /// <summary>
        /// About your Car
        /// </summary>        
        public static void EnterCarDamageAndTowedDetails(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            using (var aboutYourCar = new AboutYourCar(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                aboutYourCar.WaitForPage();
                if (detailUiChecking)
                {
                    aboutYourCar.DetailedUiChecking();
                }
                aboutYourCar.EnterCarDamageAndTowedDetails(claim);
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Where's your Car
        /// </summary>        
        public static void EnterWhereYourCarDetails(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            using (var wheresYourCar = new WheresYourCar(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                wheresYourCar.WaitForPage();
                if (detailUiChecking)
                {
                    wheresYourCar.DetailedUiChecking(claim);
                }
                wheresYourCar.EnterWhereYourCarDetails(claim);
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Review your Car
        /// </summary>        
        public static void ReviewClaimSummary(Browser browser, ClaimCar claim, bool detailUiChecking = false)
        {
            using (var reviewYourClaim = new ReviewYourClaim(browser))
            {
                reviewYourClaim.WaitForPage();
                //Review claim page is not completed for multi vehicle collision claim
                if (detailUiChecking)
                {
                    reviewYourClaim.DetailedUiChecking(claim);
                }
                reviewYourClaim.VerifyClaimSummaryDetails(claim);                
                reviewYourClaim.ClickSubmitClaim();
            }
        }

        /// <summary>
        /// Repairer options
        /// </summary>
        public static void RepairerOptions(Browser browser, ClaimCar claim, List<ServiceProvider> serviceProviders, bool detailUiChecking = false)
        {
            using (var repairerOptions = new RepairerOptions(browser))
            {
                repairerOptions.WaitForPage(waitTimeSeconds: WaitTimes.T5SEC);
                if (detailUiChecking)
                {
                    repairerOptions.DetailedUiChecking();
                }
                repairerOptions.VerifyRepairerDetails(claim, serviceProviders);
                repairerOptions.SelectRepairer(claim, serviceProviders);
            }
        }
    }
}