using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading;
using UIDriver.Pages.B2C;
using UIDriver.Pages.MicrosoftAD;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.BoatQuote;
using UIDriver.Pages.Spark.Claim;
using UIDriver.Pages.Spark.Claim.Triage;
using UIDriver.Pages.Spark.Claim.UploadInvoice;
using UIDriver.Pages.Spark.EFT;
using UIDriver.Pages.Spark.Endorsements;
using UIDriver.Pages.Spark.Endorsements.MakePayment;
using UIDriver.Pages.Spark.Endorsements.UpdateHowYouPay;
using UIDriver.Pages.Spark.MemberRefund;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace UIDriver.Pages
{
    public class LaunchPage : BasePage
    {
        private bool _IsSpark = false;

        #region Constant

        private class Constants
        {
            public class UriPath
            {
                public const string SparkRetrieveQuote = "/RetrieveQuote";
                public const string InvoiceUpload = "/claims/servicing/invoice-or-quote";
                public const string SessionTimeOut = "/408";
            }

            public class Parameter
            {
                public const string PersonId          = "crmId";
                public const string ShieldEnvironment = "shieldEnvironment";
                public const string FeatureToggles    = "featureToggles";
                public const string ClaimNumber       = "claimNumber";
                public const string PolicyNumber      = "policyNumber";
                public const string SourceUrl         = "sourceUrl";
                public const string OverrideToNumber  = "overrideToNumber";
            }
            
            public class FeatureToggleHeader
            {
                public const string MCMockHeaderName    = "Feature_UseMCMock";
                public const string BenangHeaderName    = "Feature_UseBenang";
                public const string BypassOTPHeaderName = "Feature_BypassOtp";
                public const string PersonV3HeaderName  = "Feature_PersonV3";
            }
        }
        #endregion

        #region XPATHS

        private class XPath
        {
            private const string OpenPanel = "//div[@id='wrapper']//div[@class='accordion-panel opened']";
            public class Header
            {
                public const string SparkHeader = "//h2[contains(text(),'Before we get started')]";
            }

            public class Button
            {
                public const string RetrieveQuote = OpenPanel + "//a[@href='/Quote']";
                public const string NewMotorQuote = OpenPanel + "//a[@href='/QuoteMotor']";
                public const string NewHomeQuote = OpenPanel + "//a[@href='/QuoteHome']";
                public const string NewPetQuote = OpenPanel + "//a[@href='/QuotePet']";
                public const string MakeClaim = OpenPanel + "//a[@href='/Claim2']";
                public const string FeatureToggles = OpenPanel + "//a[@href='/LaunchPage/Features']";
                public const string PCMLogon = OpenPanel + "//a[contains(@href,'LaunchPage/Logon?Length=10')]";
            }
            public class Accordion
            {
                public const string SecureAccoordion = "//div[@id='wrapper']//div[@data-accordion-id='Secure']";
            }
        }

        #endregion       

        /// <summary>
        /// Log a message and screenshot capturing the initial state 
        /// of the browser (which will show any Toggles selected if 
        /// applicable) at the start of the test.
        /// </summary>
        /// <param name="browser"></param>
        static void LogScreenshotLaunchPageState(Browser browser) => Reporting.Log("Capturing screenshot of Launchpage (and toggles if applicable)", browser.Driver.TakeSnapshot());

        /// <summary>
        /// Will open to the B2C landing page and set the feature
        /// toggles if any defined in the test config.json.
        /// </summary>
        /// <param name="browser"></param>
        public static void OpenB2CLandingPageAndFeatureToggles(Browser browser)
        {
            browser.OpenUrl(Config.Get().GetUrlB2C());

            using (var launchPage = new LaunchPage(browser))
            using (var authPage = new Authentication(browser))
            {
                authPage.WaitForADLoginPageOrDesiredLandingPage(launchPage);
            }

            SetB2CFeatureToggle(browser);
            LogScreenshotLaunchPageState(browser);
        }

        /// <summary>
        /// Open the upload Invoice URL
        /// </summary>
        public static void OpenInvoiceUploadURL(Browser browser, ClaimUploadFile claim)
        {
            OpenInvoiceUploadURL(browser, claim, browserName: Config.Get().GetBrowserType());
        }

        /// <summary>
        /// Open the upload Invoice URL
        /// </summary>
        public static void OpenInvoiceUploadURL(Browser browser, ClaimUploadFile claim,
            TargetBrowser browserName, TargetDevice device = TargetDevice.Windows11)
        {
            var invoiceUploadURL = SparkURLBuilder(sparkAppURL: Config.Get().Spark.Applications.ClaimsServicing, uriPath: Constants.UriPath.InvoiceUpload, personId: claim.PersonId, claimNumber: claim.ClaimNumber);
            browser.OpenUrl(browserName, invoiceUploadURL, device, incognito: true);
            
            using (var authPage = new Authentication(browser))
            using (var spinner = new SparkSpinner(browser))
            using (var uploadAndSubmit = new UploadAndSubmit(browser))
            {
                authPage.WaitForADLoginPageOrDesiredLandingPage(uploadAndSubmit);
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
            }
            SetSparkEnvironmentBannerToggle(browser);
        }

        /// <summary>
        /// Open the upload Invoice URL where you expect an error page will be displayed
        /// </summary>
        public static void OpenInvoiceUploadErrorPage(Browser browser, ClaimUploadFile claim)
        {
            var invoiceUploadURL = SparkURLBuilder(sparkAppURL: Config.Get().Spark.Applications.ClaimsServicing, uriPath: Constants.UriPath.InvoiceUpload, personId: claim.PersonId, claimNumber: claim.ClaimNumber);
            browser.OpenUrl(invoiceUploadURL);

            using (var authPage = new Authentication(browser))
            using (var spinner = new SparkSpinner(browser))
            using (var errorPage = new ErrorPage(browser))
            {
                authPage.WaitForADLoginPageOrDesiredLandingPage(errorPage);
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
            }
            SetSparkEnvironmentBannerToggle(browser);
        }

        /// <summary>
        /// After opening the upload Invoice URL
        /// Change the URL to /408, this will trigger the session time out error
        /// </summary>
        public static void NavigateToInvoiceUploadSessionTimeOutPage(Browser browser)
        {
            var invoiceUpload408URL = $"{Config.Get().Spark.Applications.ClaimsServicing}{Constants.UriPath.InvoiceUpload}{Constants.UriPath.SessionTimeOut}";
            browser.Driver.Navigate().GoToUrl(invoiceUpload408URL);

            using (var spinner = new SparkSpinner(browser))
            using (var errorPage = new ErrorPage(browser))
            {                
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
            }
        }

        /// <summary>
        /// Open the Claim Motor Glass URL      
        /// </summary>
        public static void OpenClaimMotorGlassURL(Browser browser, ClaimCar claim)
        {            
            OpenSparkClaimAppURL(browser, Config.Get().Spark.Applications.ClaimsMotorGlass, claim.LoginWith, claim.Claimant.PersonId, claim.Policy.PolicyNumber);

            SetSparkEnvironmentBannerToggle(browser);
        }

        /// <summary>
        /// Open the Claim Motor Collision URL
        /// </summary>
        public static void OpenClaimMotorCollisionURL(Browser browser, ClaimCar claim)
        {
            OpenSparkClaimAppURL(browser, Config.Get().Spark.Applications.ClaimsMotorCollision, claim.LoginWith, claim.Claimant.PersonId, claim.Policy.PolicyNumber);
            if (!Config.Get().IsCrossBrowserDeviceTestingEnabled)
            {
                using (var beforeYouStart = new SparkBeforeYouStart(browser))
                {
                    beforeYouStart.WaitForShieldHealthStatusCheck();
                    beforeYouStart.LogSparkApplicationVersion();
                }
            }
            SetSparkEnvironmentBannerToggle(browser);
        }

        /// <summary>
        /// Open the Spark claim application URL
        /// If LoginWith = PolicyNumber then the Policy Number will be included
        /// in the URL built by SparkURLBuilder. If not then the Policy Number
        /// will not be included.
        /// </summary>
        public static void OpenSparkClaimAppURL(Browser browser, string sparkURL, LoginWith loginType, string personId, string PolicyNumber)
        {
            string sparkAppURL = null;

            if (loginType == LoginWith.ContactId)
            {
                sparkAppURL = SparkURLBuilder(sparkAppURL: sparkURL, personId: personId);
            }
            else
            {
                sparkAppURL = SparkURLBuilder(sparkAppURL: sparkURL, personId: personId, policyNumber: PolicyNumber);
            }

            browser.OpenUrl(sparkAppURL);

            using (var beforeYouStart = new SparkBeforeYouStart(browser))
            {
                using (var authPage = new Authentication(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(beforeYouStart);
                    spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                    beforeYouStart.WaitForShieldHealthStatusCheck();
                }
            }            
        }

        public static void OpenMemberRefundOnlineURL(Browser browser)
        {
            browser.OpenUrl(Config.Get().Spark.Applications.MemberRefund);

            using (var letsGetSomeInformation = new LetsGetSomeInformation(browser))
            {
                using (var authPage = new Authentication(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(letsGetSomeInformation);
                    spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                    SetSparkShieldEnvironmentAndFeatureToggles(browser, Config.Get().Spark.Applications.MemberRefund);
                    letsGetSomeInformation.WaitForShieldHealthStatusCheck();
                }
            }
        }

        /// <summary>
        /// Open Cancel Policy URL
        /// </summary>
        public static void OpenCancelPolicyURL(Browser browser, EndorsementBase endorsementDetails)
        {
            var policyNumber = endorsementDetails.PolicyNumber;
            var policyholderPersonId = endorsementDetails.ActivePolicyHolder.PersonId;

            string cancelPolicyURL = SparkURLBuilder(sparkAppURL: Config.Get().Spark.Applications.CancelPolicy, policyNumber: policyNumber, personId: policyholderPersonId);

            browser.OpenUrl(cancelPolicyURL);

            using (var cancellationPage = new CancellationDetailsPage(browser)) {
                using (var authPage = new Authentication(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(cancellationPage);
                    spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                }
            }
            SetSparkEnvironmentBannerToggle(browser);
        }

        /// <summary>
        /// Open the Update How You Pay application directly by
        /// URL, bypassing the test launch page. Will set toggles
        /// and config values via inline parameters.
        /// </summary>
        public static void OpenUpdateHowYouPayByURL(Browser browser, EndorsementBase endorsementDetails)
        {
            var policyNumber = endorsementDetails.PolicyNumber;
            var policyholderPersonId = DataHelper.GetPersonFromMemberCentralByContactId(endorsementDetails.ActivePolicyHolder.Id).PersonId;

            string howYouPayURL = SparkURLBuilder(sparkAppURL: Config.Get().Spark.Applications.UpdateHowYouPay, policyNumber: policyNumber, personId: policyholderPersonId);

            browser.OpenUrl(howYouPayURL);

            using (var howYouPayPage = new UpdateHowYouPayDetailsPage(browser))
            using (var itsRenewalTime = new ItsRenewalTime(browser))
            using (var errorPage = new UhohErrorPage(browser))
            {
                using (var authPage = new Authentication(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    //If the policy already in renewal then it will display the It's renewal page
                    if (endorsementDetails.OriginalPolicyData.IsInRenewal)
                    {
                        //If the product type is Motor then it will redirect to the renewal page otherwise
                        //for other product type it will display the error page.
                        SparkBasePage landingPage = endorsementDetails.OriginalPolicyData.ProductType == ShieldProductType.MGP ?
                            itsRenewalTime : errorPage;                        
                        authPage.WaitForADLoginPageOrDesiredLandingPage(landingPage);
                    }
                    else
                    {
                        authPage.WaitForADLoginPageOrDesiredLandingPage(howYouPayPage);
                    }                    
                    spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                }
            }
            SetSparkEnvironmentBannerToggle(browser);
        }

        /// <summary>
        /// Open the Make A Payment application directly by
        /// URL, bypassing the test launch page. Will set toggles
        /// and config values via inline parameters.
        /// </summary>
        public static void OpenMakeAPaymentByURL(Browser browser, EndorsementBase endorsementDetails)
        {
            var policyNumber = endorsementDetails.PolicyNumber;
            var policyholderPersonId = DataHelper.GetPersonFromMemberCentralByContactId(endorsementDetails.ActivePolicyHolder.Id).PersonId;

            string makePaymentURL = SparkURLBuilder(sparkAppURL: Config.Get().Spark.Applications.MakeAPayment, policyNumber: policyNumber,personId:policyholderPersonId);

            browser.OpenUrl(makePaymentURL);

            using (var paymentPage = new PaymentPage(browser))
            using (var authPage = new Authentication(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                authPage.WaitForADLoginPageOrDesiredLandingPage(paymentPage);
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                spinner.WaitForShieldHealthStatusCheck();
            }
            
            SetSparkEnvironmentBannerToggle(browser);
        }

        /// <summary>
        /// Open the Home Claim Triage URL    
        /// </summary>
        public static void OpenHomeTriageClaimURL(Browser browser, ClaimHome claim)
        {
            string sparkAppURL = null;

            if (claim.LoginWith == LoginWith.ContactId)
            {
                sparkAppURL = SparkURLBuilder(sparkAppURL: Config.Get().Spark.Applications.TriageHome, personId: claim.Claimant.PersonId);
            }
            else
            {
                sparkAppURL = SparkURLBuilder(sparkAppURL: Config.Get().Spark.Applications.TriageHome, personId: claim.Claimant.PersonId, policyNumber: claim.PolicyDetails.PolicyNumber);
            }

            browser.OpenUrl(sparkAppURL);

            using (var homeInsuranceClaim = new BuildingAndContents(browser))
            {
                using (var authPage = new Authentication(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(homeInsuranceClaim);
                    spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                }
            }
            SetSparkEnvironmentBannerToggle(browser);
        }

        /// <summary>
        /// Open the Motor Triage URL    
        /// </summary>
        public static void OpenMotorTriageClaimURL(Browser browser, ClaimCar claim)
        {
            OpenMotorTriageClaimURL(browser, claim, Config.Get().GetBrowserType(), TargetDevice.Windows11);
        }

        /// <summary>
        /// Open the Motor Triage URL    
        /// </summary>
        public static void OpenMotorTriageClaimURL(Browser browser, ClaimCar claim,
            TargetBrowser browserName, TargetDevice device)
        {
            string sparkAppURL = null;

            if (claim.LoginWith == LoginWith.ContactId)
            {
                sparkAppURL = SparkURLBuilder(sparkAppURL: Config.Get().Spark.Applications.TriageMotor, personId: claim.Claimant.PersonId);
            }
            else
            {
                sparkAppURL = SparkURLBuilder(sparkAppURL: Config.Get().Spark.Applications.TriageMotor, personId: claim.Claimant.PersonId, policyNumber: claim.Policy.PolicyNumber);
            }

            browser.OpenUrl(browserName, sparkAppURL, device);

            using (var carInsuranceClaim = new CarInsuranceClaim(browser))
            {
                using (var authPage = new Authentication(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(carInsuranceClaim);
                    spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                }
            }
            SetSparkEnvironmentBannerToggle(browser);
        }

        /// <summary>
        /// Open the Motor Endorsement application directly by
        /// URL, bypassing the test launch page. Will set toggles
        /// and config values via inline parameters.
        /// </summary>
        /// <param name="isRenewal">flag to drive renewal or endorsement test flow</param>
        public static void OpenMotorEndorsementByURL(Browser browser, EndorseCar endorsementDetails, bool isRenewal)
        {
            var policyNumber = endorsementDetails.PolicyNumber;
            var policyholderPersonId = DataHelper.GetPersonFromMemberCentralByContactId(endorsementDetails.ActivePolicyHolder.Id).PersonId;

            string motorEndorsementURL = SparkURLBuilder(sparkAppURL: Config.Get().Spark.Applications.MotorEndorsement, policyNumber: policyNumber, personId: policyholderPersonId);

            browser.OpenUrl(motorEndorsementURL);

            using (var yourCarPage = new YourCar(browser))
            using (var startDate = new ChangePolicyStartDate(browser))
            {
                using (var authPage = new Authentication(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    if (isRenewal)
                    {
                        authPage.WaitForADLoginPageOrDesiredLandingPage(yourCarPage);
                    }
                    else
                    {
                        authPage.WaitForADLoginPageOrDesiredLandingPage(startDate);
                    }
                    
                    spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                }
            }
            SetSparkEnvironmentBannerToggle(browser);
        }

        /// <summary>
        /// Open the Caravan Endorsement application directly by
        /// URL, bypassing the test launch page. Will set toggles
        /// and config values via inline parameters.
        /// </summary>
        /// <param name="isRenewal">flag to drive renewal or endorsement test flow</param>
        public static void OpenCaravanEndorsementByURL(Browser browser, EndorseCaravan endorsementDetails, bool isRenewal)
        {
            var policyNumber = endorsementDetails.PolicyNumber;
            var policyholderPersonId = DataHelper.GetPersonFromMemberCentralByContactId(endorsementDetails.ActivePolicyHolder.Id).PersonId;

            string caravanEndorsementURL = SparkURLBuilder(sparkAppURL: Config.Get().Spark.Applications.CaravanEndorsement, policyNumber: policyNumber, personId: policyholderPersonId);

            browser.OpenUrl(caravanEndorsementURL);

            using (var yourCaravanOrTrailer = new YourCaravanOrTrailer(browser))
            using (var startDate = new SetPolicyStartDate(browser))
            using (var authPage = new Authentication(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                if (isRenewal)
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(yourCaravanOrTrailer);
                }
                else
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(startDate);
                }
               
               spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
               SetSparkEnvironmentBannerToggle(browser);
            }
        }

        /// <summary>
        /// The method will build a Spark application URL to enable it opening that application
        /// with the feature toggles set and any required parameters for that application to
        /// begin (e.g. Member Person ID and Claim number to begin claim invoicing).
        /// 
        /// Current items are set:
        /// * Feature toggles (obtained from config)
        /// * Shield environment (obtained from config)
        /// * optional URL parameters
        /// 
        /// The intent is to not require the Spark landing page, but enter straight into the
        /// application and eliminate time setting values in the configuration panel.
        /// 
        /// This method is a work in progress and is intended to be updated as more Spark
        /// applications use it.
        /// 
        /// More information can be found in:
        /// https://rac-wa.atlassian.net/wiki/spaces/ISP/pages/3051356643/Test+Launch+Page+Implementation
        /// </summary>
        /// <param name="sparkAppURL">Primary URL for the Spark application being tested. This field should not be null or empty.</param>
        /// <param name="uriPath">Optional: relative URL path for supplementary app features, such as retrieve quote or invoice upload</param>   
        /// <param name="personId">Optional: Member Central person ID to emulate being logged in as</param>   
        /// <param name="claimNumber">Optional: claim number parameter</param>
        /// <param name="policyNumber">Optional: policynumber number parameter</param>

        public static string SparkURLBuilder(string sparkAppURL, string uriPath = null, string personId = null, string claimNumber = null, string policyNumber = null)
        {
            var config = Config.Get();
            var toggles = new List<SparkFeatureToggle>();

            if (string.IsNullOrEmpty(sparkAppURL))
            { Reporting.Error("No Spark app URL provided. Unable to open web application to test."); }

            foreach (var toggle in config.Spark.FeatureToggles)
            {
                var featureToggle = new SparkFeatureToggle();
                featureToggle.DisplayName = toggle.Key;
                
                if (toggle.Key == SparkFeatureToggles.UseMCMock.GetDescription())
                {
                    featureToggle.HeaderName = Constants.FeatureToggleHeader.MCMockHeaderName;
                }
                else if (toggle.Key == SparkFeatureToggles.UseBypassOTP.GetDescription())
                {
                    featureToggle.HeaderName = Constants.FeatureToggleHeader.BypassOTPHeaderName;
                }
                else if (toggle.Key == SparkFeatureToggles.UsePersonV3.GetDescription())
                {
                    featureToggle.HeaderName = Constants.FeatureToggleHeader.PersonV3HeaderName;
                }
                else
                {
                    Reporting.Log($"There are no headers to amend for {toggle.Key} as that isn't supported by the application.");
                    continue;
                }

                featureToggle.Enabled = toggle.Value;
                toggles.Add(featureToggle);
            }

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(toggles);
            var encodedFeatureToggles = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(jsonString));

            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            AddNameValueIfNotNull(queryString, Constants.Parameter.ClaimNumber, claimNumber);
            AddNameValueIfNotNull(queryString, Constants.Parameter.PolicyNumber, policyNumber);
            AddNameValueIfNotNull(queryString, Constants.Parameter.PersonId, personId);
            AddNameValueIfNotNull(queryString, Constants.Parameter.ShieldEnvironment, config.Shield.Environment?.ToUpper());
            if (!config.IsBypassOTPEnabled())
            {
                queryString.Add(Constants.Parameter.OverrideToNumber, config.Telephone.OverrideOTPNumber);
                Reporting.LogMinorSectionHeading($"Telephone number for One Time Password will be OVERRIDDEN in header to send codes to " +
                    $"{config.Telephone.OverrideOTPNumber} for any Multi-Factor Authentication prompts.");
            }
            else 
            {
                Reporting.LogMinorSectionHeading($"IsBypassOTPEnabled = '{config.IsBypassOTPEnabled()}' so we will bypass any " +
                    $"Multi-Factor Authentication prompts with default code '{MultiFactorAuthentication.BypassOTP.VerificationCode}' instead of searching mailosaur for a code.");
            }
            
            queryString.Add(Constants.Parameter.SourceUrl, config.Spark.Applications.LaunchPage);
            queryString.Add(Constants.Parameter.FeatureToggles, encodedFeatureToggles);
            
            var baseUri = new Uri(sparkAppURL);
            var requestedUri = string.IsNullOrEmpty(uriPath) ? baseUri : new Uri(baseUri, uriPath);
            var urlBuilder = new UriBuilder(requestedUri)
            {
                Query = queryString.ToString()
            };
            
            return urlBuilder.ToString();
        }

        public static void OpenSparkBoatLandingPageAndSetConfig(Browser browser)
        {
            browser.OpenUrl(Config.Get().Spark.Applications.Boat);

            using (var boatStartPage = new Spark.BoatQuote.SparkQuoteImportantInformation(browser))
            {
                using (var authPage = new Authentication(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(boatStartPage);
                    spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                }
                SetSparkShieldEnvironmentAndFeatureToggles(browser, Config.Get().Spark.Applications.Boat);
                boatStartPage.WaitForShieldHealthStatusCheck();
            }
        }

        public static void OpenSparkBoatRetrieveQuoteAndSetConfig(Browser browser)
        {
            var url = $"{Config.Get().Spark.Applications.Boat.TrimEnd('/')}/retrieve-quote"; //TODO AUNT-11 if Caravan URI changes to kebab-case we should update this to refer to Constants.UriPath.SparkRetrieveQuote
            Reporting.Log($"url = {url}");
            browser.OpenUrl(url);

            using (var boatRetrieveQuote = new SparkBoatRetrieveQuote(browser))
            {
                using (var authPage = new Authentication(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(boatRetrieveQuote);
                    spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                }
                SetSparkShieldEnvironmentAndFeatureToggles(browser, url);
            }
        }

        public static void OpenSparkMotorcycleLandingPage(Browser browser)
        {
            browser.OpenUrl(Config.Get().Spark.Applications.Motorcycle);

            using (var motorcycleStartPage = new Spark.MotorcycleQuote.BeforeWeGetStarted(browser))
            {
                using (var authPage = new Authentication(browser))
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(motorcycleStartPage);
                }
                SetSparkShieldEnvironmentAndFeatureToggles(browser, Config.Get().Spark.Applications.Motorcycle);
                motorcycleStartPage.WaitForShieldHealthStatusCheck();
            }
        }

        public static void OpenSparkCaravanLandingPage(Browser browser)
        {
            browser.OpenUrl(Config.Get().Spark.Applications.Caravan);

            using (var caravanStartPage = new Spark.CaravanQuote.BeforeWeGetStarted(browser))
            {
                using (var authPage = new Authentication(browser))
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(caravanStartPage);
                }
                SetSparkShieldEnvironmentAndFeatureToggles(browser, Config.Get().Spark.Applications.Caravan);
                caravanStartPage.WaitForShieldHealthStatusCheck();
            }
        }

        public static void OpenSparkTestLaunchPage(Browser browser, string externalSiteName)
        {
            browser.OpenUrl(Config.Get().Spark.Applications.LaunchPage);

            using (var sparkTestLaunchPage = new SparkTestLaunchPage(browser))
            using (var authPage = new Authentication(browser))
            {
                authPage.WaitForADLoginPageOrDesiredLandingPage(sparkTestLaunchPage);

                sparkTestLaunchPage.SetExternalSite(externalSiteName);
                sparkTestLaunchPage.SetSparkShieldEnvironment(Config.Get().Shield.Environment);
                sparkTestLaunchPage.SetSparkFeatureToggleState(Config.Get().Spark.FeatureToggles);
            }
        }

        /// <summary>
        /// Opens the Caravan retrieve quote page.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="productType"></param>
        public static void OpenCaravanRetrieveQuoteLandingPage(Browser browser, ShieldProductType productType)
        {
            if (productType == ShieldProductType.MGV)
            {
                // Avoid cases of where people may or may not have trailing "/" in their base Caravan URL.
                var url = Path.Combine($"{Config.Get().Spark.Applications.Caravan}{Constants.UriPath.SparkRetrieveQuote}");

                browser.OpenUrl(url);
                using (var caravanRetrieveStartPage = new SparkRetrieveQuote(browser))
                {
                    using (var authPage = new Authentication(browser))
                    {
                        authPage.WaitForADLoginPageOrDesiredLandingPage(caravanRetrieveStartPage);
                    }
                    SetSparkShieldEnvironmentAndFeatureToggles(browser, Config.Get().Spark.Applications.Caravan);
                    LogScreenshotLaunchPageState(browser);
                    caravanRetrieveStartPage.WaitForShieldHealthStatusCheck();
                }
            }
            else
            {
                Reporting.Error($"We haven't implemented retrieve for quotes of type {productType.GetDescription()}");
            }
        }

        public static void OpenCaravanRetrieveQuoteFromEmail(Browser browser, string retrieveQuoteLink)
        {
            browser.OpenUrl(retrieveQuoteLink);
            using (var caravanRetrieveStartPage = new SparkRetrieveQuote(browser))
            {
                using (var authPage = new Authentication(browser))
                {
                    authPage.WaitForADLoginPageOrDesiredLandingPage(caravanRetrieveStartPage);
                }
                SetSparkShieldEnvironmentAndFeatureToggles(browser, retrieveQuoteLink);
                LogScreenshotLaunchPageState(browser);
                caravanRetrieveStartPage.WaitForShieldHealthStatusCheck();
            }
        }

        /// <summary>
        /// Open the provided Cash Settlement link
        /// </summary>
        public static void OpenEFTLink(Browser browser, string cashSettlementUrl, ClaimContact claimData)
        {
            var config = Config.Get();
            var toggles = new List<SparkFeatureToggle>();

            if (string.IsNullOrEmpty(cashSettlementUrl))
            { Reporting.Error("No URL provided. Unable to open web application to test."); }

            foreach (var toggle in config.Spark.FeatureToggles)
            {
                var featureToggle = new SparkFeatureToggle();
                featureToggle.DisplayName = toggle.Key;

                if (toggle.Key == SparkFeatureToggles.UseBypassOTP.GetDescription())
                {
                    featureToggle.HeaderName = Constants.FeatureToggleHeader.BypassOTPHeaderName;
                    featureToggle.Enabled = toggle.Value;
                    toggles.Add(featureToggle);
                }
                
            }

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(toggles);
            var encodedFeatureToggles = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(jsonString));

            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            AddNameValueIfNotNull(queryString, Constants.Parameter.ShieldEnvironment, config.Shield.Environment?.ToUpper());
            queryString.Add(Constants.Parameter.FeatureToggles, encodedFeatureToggles);
            queryString.Add(System.Web.HttpUtility.ParseQueryString(new System.Uri(cashSettlementUrl).Query));

            var baseUri = new Uri(cashSettlementUrl);           
            var urlBuilder = new UriBuilder(baseUri)
            {
                Query = queryString.ToString()
            };
           
            browser.OpenUrl(urlBuilder.ToString());

            SparkBasePage landingPage = (claimData.ExpectedOutcome == ClaimContact.ExpectedCSFSOutcome.EFT) ?
                (SparkBasePage)new OneTimePasscodeScreen(browser) : (SparkBasePage)new ChooseSettlement(browser);

            using (landingPage)
            using (var authPage = new Authentication(browser))
            {
                authPage.WaitForADLoginPageOrDesiredLandingPage(landingPage);
            }
            SetSparkEnvironmentBannerToggle(browser);

            Reporting.Log($"Please note actual URL used logged above has had headers for shieldEnvironment and featureToggles added");
            Reporting.Log($"Original EFT URL: <a href=\"{cashSettlementUrl}\">{cashSettlementUrl}</a>", browser.Driver.TakeSnapshot());
        }

        public LaunchPage(Browser browser, bool isSpark = false) : base(browser)
        {
            _IsSpark = isSpark;
        }

        override public bool IsDisplayed()
        {
            try
            {
                if (_IsSpark)
                {
                    GetElement(XPath.Header.SparkHeader);
                }
                else
                {
                    GetElement(XPath.Button.MakeClaim);
                    GetElement(XPath.Button.NewHomeQuote);
                    GetElement(XPath.Button.NewMotorQuote);
                    GetElement(XPath.Button.NewPetQuote);
                    GetElement(XPath.Button.RetrieveQuote);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void ClickRetrieveQuote()
        {
            ClickControl(XPath.Button.RetrieveQuote);
        }

        public void ClickNewMotorQuote()
        {
            ClickControl(XPath.Button.NewMotorQuote);
        }

        public void ClickNewHomeQuote()
        {
            ClickControl(XPath.Button.NewHomeQuote);
        }

        public void ClickNewPetQuote()
        {
            ClickControl(XPath.Button.NewPetQuote);
        }

        public void ClickNewAnonymousClaim()
        {
            ClickControl(XPath.Button.MakeClaim);
        }

        public void ClickPreviewFeatures()
        {
            ClickControl(XPath.Button.FeatureToggles);
        }

        /// <summary>
        /// Expands the test page PCM accordion options, then enters
        /// the PCM login option
        /// </summary>
        public void GotoPCMLoginOptions()
        {
            if (GetClass(XPath.Accordion.SecureAccoordion).ToLower().Contains("closed"))
            {
                ClickControl(XPath.Accordion.SecureAccoordion);
            }
            Thread.Sleep(1000); // Fixed sleep for accordian transition.
            ClickControl(XPath.Button.PCMLogon, waitTimeSeconds: WaitTimes.T30SEC);
        }

        private static void SetB2CFeatureToggle(Browser browser)
        {
            var b2cConfig = Config.Get().B2C;

            Thread.Sleep(2000);
            Reporting.LogFeatureToggle(b2cConfig.FeatureToggles);

            if (b2cConfig.HasFeatureTogglesDefined)
            {
                using (var launchPage = new LaunchPage(browser))
                {
                    launchPage.ClickPreviewFeatures();
                }

                FeatureTogglePage.ProcessB2CFeatureToggles(browser, b2cConfig.FeatureToggles);
            }
        }

        public static void SetSparkShieldEnvironmentAndFeatureToggles(Browser browser, string url)
        {
            var sparkConfig = Config.Get().Spark;

            Thread.Sleep(2000);
            Reporting.LogFeatureToggle(sparkConfig.FeatureToggles);
            if (!Config.Get().Shield.IsUatEnvironment())
            {
                Reporting.Log($"Checking & setting spark Feature Toggle state as {url} isn't a UAT environment.");
                using (var configurationPanel = new ConfigurationPanel(browser))
                {
                    configurationPanel.OpenConfigFrame();
                    configurationPanel.SetEnvironmentBannerToggle(sparkConfig.BannerVisibleToggle);
                    configurationPanel.SetSparkShieldEnvironment(Config.Get().Shield.Environment);
                    configurationPanel.SetSparkFeatureToggleState(sparkConfig.FeatureToggles);
                    Reporting.Log("Capturing ConfigFrame State", browser.Driver.TakeSnapshot());
                    configurationPanel.CloseConfigFrame();
                }
            }
            else
            {
                Reporting.Log($"Bypassing setting spark Feature Toggle state as evaluation of Shield settings indicate that we're targeting a UAT Environment and Toggles should be pre-configured.");
            }
        }

        public static void SetSparkEnvironmentBannerToggle(Browser browser)
        {
            var sparkConfig = Config.Get().Spark;

            //The NPE banner toggle will be On by default
            //only time you need to change when you are setting the toggle value to false
            if (!sparkConfig.BannerVisibleToggle)
            {
                Thread.Sleep(2000);
                Reporting.LogFeatureToggle(sparkConfig.FeatureToggles);
                using (var configurationPanel = new ConfigurationPanel(browser))
                {
                    configurationPanel.OpenConfigFrame();
                    configurationPanel.SetEnvironmentBannerToggle(sparkConfig.BannerVisibleToggle);
                    Reporting.Log("Capturing ConfigFrame State", browser.Driver.TakeSnapshot());
                    configurationPanel.CloseConfigFrame();
                }
            }
        }

        /// <summary>
        /// Opens the Spark NPE configuration panel and sets the OTP
        /// override phone number. Does nothing if OTP Bypass is 
        /// enabled, or if the config doesn't define a phone number
        /// for the OTP override.
        /// </summary>
        public static void SetNumberOfOTPOverride(Browser browser)
        {
            var config = Config.Get();

            if (!config.IsBypassOTPEnabled() && !string.IsNullOrEmpty(config.Telephone.OverrideOTPNumber))
            {
                using (var configurationPanel = new ConfigurationPanel(browser))
                {
                    configurationPanel.OpenConfigFrame();
                    configurationPanel.SetOTPOverride(config.Telephone.OverrideOTPNumber);
                    Reporting.Log("Capturing ConfigFrame State", browser.Driver.TakeSnapshot());
                    configurationPanel.CloseConfigFrame();
                }
            }
        }

        private static void AddNameValueIfNotNull(NameValueCollection collection, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                collection.Add(name, value);
            }
        }
    }
}
