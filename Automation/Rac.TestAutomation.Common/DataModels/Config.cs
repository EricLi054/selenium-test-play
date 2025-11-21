using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using static Rac.TestAutomation.Common.Constants.General;
using System.Text.RegularExpressions;


namespace Rac.TestAutomation.Common
{
    public class Credentials
    {
        public string User { get; set; }
        public string Pwd { get; set; }
    }

    public class KeyValueBool
    {
        public string Key { get; set; }
        public bool Value { get; set; }
    }

    public class KeyValueString
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class B2CTarget
    {
        public string Url { get; set; }

        /// <summary>
        /// Represents available feature toggles in B2C/Spark applications
        /// </summary>
        public List<KeyValueBool> FeatureToggles { get; set; }

        /// <summary>
        /// Check whether any feature toggle is defined and return true otherwise return false 
        /// </summary>
        public bool HasFeatureTogglesDefined => FeatureToggles?.Any() ?? false;
    }

    public class SparkApplications
    {
       
        public string Boat            { get; set; }
        public string CancelPolicy { get; set; }
        public string Caravan         { get; set; }
        public string ClaimsHomeGeneral { get; set; }
        public string ClaimsServicing { get; set; }
        public string ClaimsMotorCollision { get; set; }
        public string ClaimsMotorGlass { get; set; }
        public string EFT             { get; set; }
        public string FenceClaim      { get; set; }
        public string LaunchPage      { get; set; }
        public string Motorcycle      { get; set; }
        public string MotorEndorsement { get; set; }
        public string TriageHome      { get; set; }
        public string TriageMotor     { get; set; }
        public string UpdateHowYouPay { get; set; }
        public string MemberRefund { get; set; }
        public string MakeAPayment { get; set; }
        public string CaravanEndorsement { get; set; }
    }

    public class SparkTarget
    {
        public bool BannerVisibleToggle { get; set; }

        public SparkApplications Applications { get; set; }
        /// <summary>
        /// Represents available feature toggles in B2C/Spark applications
        /// </summary>
        public List<KeyValueBool> FeatureToggles { get; set; }
    }

    public class ShieldTarget
    {
        /// <summary>
        /// The name of the Shield environment being referenced. Not
        /// case sensitive, typically of the form shieldint1, shielduat1, etc
        /// This value is used in both DB and API operations to Shield.
        /// </summary>
        public string Environment { get; set; }
        public ShieldWebTarget Web { get; set; }
        public Credentials Database { get; set; }

        /// <summary>
        /// This should be used by code to fetch the 'Env'
        /// value, to ensure that it is recognised.
        /// </summary>
        /// <returns></returns>
        public ENV GetEnvironmentAsEnum() => DataHelper.GetValueFromDescription<ENV>(Environment.ToUpperInvariant());

        /// <summary>
        /// If a Shield name contains this constant we expect that to indicate that it 
        /// is a UAT environment. We may need to use this to bypass things such
        /// as setting the Feature Toggle state, as the controls are not 
        /// rendered in UAT environments but we need to retain them for SIT.
        /// </summary>
        private const string UAT = "uat";

        private const string SIT = "sit";
        private const string DEV = "dev";

        /// <summary>
        /// Check whether an environment is a UAT environment based on the URL
        /// containing the UAT constant.
        /// </summary>
        public bool IsUatEnvironment() => Environment.IndexOf(UAT, StringComparison.OrdinalIgnoreCase) >= 0;

        public bool IsDevEnvironment() => Environment.IndexOf(DEV, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public class ShieldWebTarget : Credentials
    {
        public string Url { get; set; }
    }

    public class MyRACProperties
    {
        public bool IsMyRACSupportExpected()
        {
            return ((Config.Get().Shield.Environment == "shieldint2"
                || Config.Get().Shield.Environment == "shielduat6")
                && (Config.Get().IsMyRACLoginEnabled()));
        }
        public string Pwd { get; set; }
    }
    

    public class AzureEnvironment
    {
        public string APIKey { get; set; }
        /// <summary>
        /// APIM is either dev/sit/uat. May not necessarily align
        /// with the domain of B2C or Shield, hence is defined here.
        /// </summary>
        public string APIEnv { get; set; }
        /// <summary>
        /// Used to specify if the current Environment has MC integration.
        /// Test verifications against MC should only be carried out if the environment supports MC integration.
        /// </summary>
        public bool HasLiveSync { get; set; }
        /// <summary>
        /// Optional config item. Only needed for cases where we need to get
        /// an OAuth token for some of its API operations.
        /// </summary>
        public string APIClientId { get; set; }
        /// <summary>
        /// Optional config item. Only needed for cases where we need to get
        /// an OAuth token for some of its API operations.
        /// </summary>
        public string APISecret { get; set; }
        /// <summary>
        /// Optional config item. Only needed for cases where we need to get
        /// an OAuth token for some of its API operations.
        /// </summary>
        public string APIScope { get; set; }
    }

    public class AzureTable
    {
        public string URI { get; set; }
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
    }

    public class EmailConfiguration
    {
        public string APIKey { get; set; }
        public string ServerID { get; set; }
        public string Domain { get; set; }
    }

    public class PercyConfiguration
    {
        public string Token { get; set; }
    }

    public class AutomateConfiguration
    {
        public string URL { get; set; }
        public string UserName { get; set; }
        public string Key { get; set; }
    }

    public class CloudProperties
    {
        /// <summary>
        /// For Microsoft Online OAuth operations, this
        /// identifies RAC WA.
        /// </summary>
        public string TenantId { get; set; }
        public AzureEnvironment Shield { get; set; }
        public AzureEnvironment MemberCentral { get; set; }
        /// <summary>
        /// Member Central Mock config elements.
        /// </summary>
        public AzureEnvironment MCMock { get; set; }
        public AzureEnvironment RoadsideBundling { get; set; }
        public AzureEnvironment SendGrid { get; set; }
        public AzureEnvironment MemberNotificationService { get; set; }
        public AzureEnvironment ContactService { get; set; }
        public AzureTable StorageMotorClaims { get; set; }
        public AzureTable StorageMemberRefund { get; set; }
    }

    public class BrowserStackConfiguration
    {
        public PercyConfiguration Percy { get; set; }
        public AutomateConfiguration Automate { get; set; }
    }

    public class CCIConfiguration
    {
        public string ClaimMailbox { get; set; }
        public string InsurerMailbox { get; set; }
        public string SupplierMailbox { get; set; }
        public string ProofOfLossMailbox { get; set; }
        public string TCUMailbox { get; set; }
        public string TenantId { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
    }

    public class Config
    {
        private static Config _config;
        public static Config Get() 
        {

            if (_config == null)
            {
                var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string file = string.Format("{0}\\config.json", path);
                if (File.Exists(file))
                {
                   _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(file));
                }
            }
            return _config;
        }

        public string Browser { get; set; }
        public Credentials WindowsAD { get; set; }
        public B2CTarget B2C { get; set; }
        public SparkTarget Spark { get; set; }
        public ShieldTarget Shield { get; set; }
        public MyRACProperties MyRAC { get; set; }
        public CloudProperties Azure { get; set; }
        public EmailConfiguration Email { get; set; }
        public TelephoneConfiguration Telephone { get; set; }
        public BrowserStackConfiguration BrowserStack { get; set; }
        public CCIConfiguration CCI { get; set; }
        public bool IsShieldDatabaseConnectionsSupported()
        {
            return Shield != null &&
                !string.IsNullOrEmpty(Shield.Environment);
        }

        public TargetBrowser GetBrowserType()
        {
            return DataHelper.GetValueFromDescription<TargetBrowser>(Browser);
        }
       

        public string GetUrlB2C()
        {
            // Logic checks the format of URL and that it is a "https" URL.
            Uri uriResult;
            var isValidHttpsURI = Uri.TryCreate(B2C.Url, UriKind.Absolute, out uriResult) &&
                                  uriResult.Scheme == Uri.UriSchemeHttps;

            if (!isValidHttpsURI)
            { throw new ArgumentException("Expecting B2C.Url config parameter to have a valid HTTPS URL. Terminating test."); }

            return B2C.Url;
        }

        /// <summary>
        /// Determines if MC Mock feature toggle is enabled via config for B2C and Spark applications
        /// 
        /// The feature toggle must be explicitly defined as TRUE, and the basic Mock API
        /// inputs must also be set in the config.
        /// 
        /// EXAMPLE BEHAVIOURS DRIVEN BY THIS
        /// - Pre-populating the Member Central Mock at the start of a quote
        /// - Determining wether data for verification is retrieved from the Member Central Mock or 
        /// from an actual Member Central instance.
        /// </summary>
        /// <returns></returns>
        public bool IsMCMockEnabled()
        {
            List<bool> featureTogglesList = new List<bool>
            {
                B2C.FeatureToggles.Any(x => x.Key   == B2CFeatureToggles.UseMemberCentralMock.GetDescription() && x.Value == true),
                Spark.FeatureToggles.Any(x => x.Key == SparkFeatureToggles.UseMCMock.GetDescription() && x.Value == true)
            };

            if (featureTogglesList.Contains(true) &&
                   (Azure.MCMock == null ||
                    string.IsNullOrEmpty(Azure.MCMock.APIKey)))
            {
                Reporting.Error("Tests are turning on MC Mock toggle, but MC Mock config.json is incomplete.");
            }

            return featureTogglesList.Contains(true);
        }

        /// <summary>
        /// Determines if Person V3 support is enabled for Spark applications
        /// </summary>
        public bool IsPersonV3Enabled() => Spark.FeatureToggles.Any(
                x => x.Key == SparkFeatureToggles.UsePersonV3.GetDescription() &&
                x.Value == true);

        /// <summary>
        /// Return true if the Bypass OTP toggle is enabled, otherwise returns false
        /// </summary>
        public bool IsBypassOTPEnabled() => Spark.FeatureToggles.Any(
                x => x.Key == SparkFeatureToggles.UseBypassOTP.GetDescription() &&
                x.Value == true);

        /// <summary>
        /// Return true if "Use myRAC Login" is enabled, otherwise returns false.
        /// 
        /// IMPORTANT: We can only ever use the myRAC login where there is an integrated 
        /// Member Central NPE related to the Shield environment under test as myRAC requires 
        /// a Member Central NPE to function.
        /// </summary>
        public bool IsMyRACLoginEnabled() => Spark.FeatureToggles.Any(
                x => x.Key == SparkFeatureToggles.LoginMyRAC.GetDescription() &&
                x.Value == true);

        /// <summary>
        /// Return true if 'Expect MFA for Spark Endorsements Bank Details' toggle is enabled, otherwise returns false.
        /// If true then when adding new bank account details during an online endorsement we will expect to 
        /// navigate the MFA prompts. 
        /// If false, the MFA prompts will not be expected.
        /// </summary>
        /// <returns></returns>
        public bool IsSparkEndorsementBankMultiFactorAuthenticationExpected() => Spark.FeatureToggles.Any(
            x => x.Key == SparkFeatureToggles.SparkEndorsementsBankExpectMultiFactorAuthentication.GetDescription() &&
            x.Value == true);

        //TODO: Remove this once INSU-818 goes live as this will then be permamently on.
        /// <summary>
        /// Return true if automation expects the "More About Your Damage" screen to be present in the
        /// Spark Claims Home flows. This is only relevant while INSU-818 is underway. Once delivered
        /// this feature will be permamently on, and so automation won't need a toggle any more.
        /// </summary>
        public bool IsClaimHomeMoreAboutYourDamageScreenEnabled() => Spark.FeatureToggles.Any(
            x => x.Key == SparkFeatureToggles.ClaimsHomeMoreAboutYourDamage.GetDescription() &&
            x.Value == true);

        public class TelephoneConfiguration
        {
            private string _overrideOTPNumber;
            public string OverrideOTPNumber 
            {
                get {
                    if (string.IsNullOrEmpty(_overrideOTPNumber))
                    {
                        Reporting.Error("Config issue: OTP override number has not been defined.");
                    }
                    
                    var matches = Regex.Matches(_overrideOTPNumber, "\\+[0-9]{11}", RegexOptions.IgnoreCase);

                    if (matches == null || matches.Count< 1)
                    {
                        Reporting.Error($"Config issue: OTP override number ({_overrideOTPNumber}) doesn't match expected format of beginning with a '+' " +
                            $"and having 11 digits.<BR>" +
                            $"A valid telephone number (e.g. +15405024901) is required if not using OTP Bypass. IsBypassOTPEnabled = '{Config.Get().IsBypassOTPEnabled()}'. <BR>" +
                            $"Please check configuration before running again.");
                    }

                    return _overrideOTPNumber;
                    }
                set { _overrideOTPNumber = value; }
            }

            public List<KeyValueBool> FeatureToggles { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool MNSInMailosaur() => Telephone.FeatureToggles.Any(
            x => x.Key == TelephoneFeatureToggles.SMSForMNSInMailosaur.GetDescription() &&
            x.Value == true);

        /// <summary>
        /// Returns whether the B2C feature toggle for Cyclone Re-Insurance
        /// has been set. If the toggle has not been included in the
        /// config.json, then it is assumed as 'false'.
        /// 
        /// TODO: be removed with https://rac-wa.atlassian.net/browse/B2C-5010.
        /// </summary>
        public bool IsCycloneEnabled() => B2C.FeatureToggles.Any(
                x => x.Key == B2CFeatureToggles.CycloneReInsurance.GetDescription() && 
                x.Value == true);

        /// <summary>
        /// Determines whether UseAddressManagementApi Toggle is enabled.
        /// 
        /// If it is, we stop expecting the "Is the policyholder’s mailing 
        /// address the same as the insured property address?" question to
        /// be displayed on Retrieved quotes, so we have to TRACK whether
        /// the quote is retrieved or not (see QuoteHasBeenRetrieved in 
        /// QuoteHome.cs).
        /// 
        /// JIRA REF: https://rac-wa.atlassian.net/browse/B2C-4382
        /// 
        /// This will always be the logic after Benang go-live.
        /// 
        /// </summary>
        public bool IsUseAddressManagementApiEnabled => B2C.FeatureToggles.Any(
                            x => x.Key == B2CFeatureToggles.UseAddressManagementApi.GetDescription() && 
                            x.Value == true);

        // TODO: B2C-4561 Remove MotorRiskAddress toggle code when removing toggle from B2C/PCM Functional code
        //       including B2CFeatureToggles in General.cs
        /// <summary>
        /// Determines if MotorRiskAddress feature toggle is enabled via config for B2C/PCM
        /// 
        /// The feature toggle must be explicitly defined as TRUE until it becomes
        /// the norm in functional code at which time this should be removed.
        /// 
        /// EXAMPLE BEHAVIOURS DRIVEN BY THIS
        /// - Expect to provide a full QAS-validated Risk Address instead of 
        ///   just a Suburb when obtaining a quote or endorsing a policy.
        /// </summary>
        public bool IsMotorRiskAddressEnabled()
        {
            List<bool> featureTogglesList = new List<bool>
            {
                B2C.FeatureToggles.Any(x => x.Key        == B2CFeatureToggles.MotorRiskAddress.GetDescription() && x.Value == true)
            };

            return featureTogglesList.Contains(true);
        }

        /// <summary>
        /// Returns whether the B2C feature toggle for the 2025 changes to use new 
        /// CXOne web chat Agent for B2C Claims is on or off.
        /// If the toggle is not defined in the config.json, then it 
        /// is assumed as 'false'.
        /// 
        /// TODO: INSU-450 tracks when this toggle is due to be removed.
        /// </summary>
        public bool UseCxOneAgentForClaimsWebChat() => B2C.FeatureToggles.Any(
                x => x.Key == B2CFeatureToggles.UseCxOneAgentForClaimsWebChat.GetDescription() &&
                x.Value == true);

        /// <summary>
        /// Returns whether the B2C feature toggle for the 2025 changes to use new 
        /// CXOne web chat Agent for B2C Policy is on or off.
        /// If the toggle is not defined in the config.json, then it 
        /// is assumed as 'false'.
        /// 
        /// TODO: INSU-450 tracks when this toggle is due to be removed.
        /// </summary>
        public bool UseCxOneAgentForPolicyWebChat() => B2C.FeatureToggles.Any(
                x => x.Key == B2CFeatureToggles.UseCxOneAgentForPolicyWebChat.GetDescription() &&
                x.Value == true);

        /// <summary>
        /// Determines to enable visual testing or not
        /// </summary>
        /// <returns></returns>
        public bool IsVisualTestingEnabled => string.IsNullOrEmpty(BrowserStack?.Percy?.Token) ? false : true;

        /// <summary>
        /// Determines to enable device testing or not
        /// </summary>
        /// <returns></returns>
        public bool IsCrossBrowserDeviceTestingEnabled => string.IsNullOrEmpty(BrowserStack?.Automate?.Key) ? false : true;
    }
}
