using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class General
        {
            /// <summary>
            /// These are the Test Categories
            /// </summary>
            public class TestCategory
            {
                public const string Regression = nameof(Regression);
                public const string B2CPCM = nameof(B2CPCM);
                public const string Westpac_Payment = nameof(Westpac_Payment);
                public const string New_Business = nameof(New_Business);
                public const string Endorsement = nameof(Endorsement);
                public const string Claim = nameof(Claim);
                public const string ClaimServicing = nameof(ClaimServicing);
                public const string ClaimStorm = nameof(ClaimStorm);
                public const string Motor = nameof(Motor);
                public const string Home = nameof(Home);
                public const string Caravan = nameof(Caravan);
                public const string Pet = nameof(Pet);
                public const string Motorcycle = nameof(Motorcycle);
                public const string Spark = nameof(Spark);
                public const string EFT = nameof(EFT);
                public const string FenceOnly = nameof(FenceOnly);
                public const string SharedFence = nameof(SharedFence);
                public const string PrivateFence = nameof(PrivateFence);
                public const string Contents = nameof(Contents);
                public const string Building = nameof(Building);
                public const string Glass = nameof(Glass);                
                public const string SingleVehicleCollision = nameof(SingleVehicleCollision);
                public const string MultiVehicleCollision = nameof(MultiVehicleCollision);
                public const string UAT = nameof(UAT);
                public const string Integration = nameof(Integration);
                public const string Mock_Member_Central_Support = nameof(Mock_Member_Central_Support);
                public const string CHaaFS = nameof(CHaaFS);
                public const string PolicyCancellation = nameof(PolicyCancellation);
                public const string UpdateHowYouPay = nameof(UpdateHowYouPay);
                public const string ChangeMyPolicy = nameof(ChangeMyPolicy);
                public const string Smoke = nameof(Smoke);
                public const string Boat = nameof(Boat);
                public const string MotorRenewal = nameof(MotorRenewal);
                public const string CaravanRenewal = nameof(CaravanRenewal);
                public const string MakePayment = nameof(MakePayment);
                public const string RejectedInstalment = nameof(RejectedInstalment);
                public const string VisualTest = nameof(VisualTest);
                public const string CrossBrowserAndDeviceTest = nameof(CrossBrowserAndDeviceTest);
                public const string MemberNotificationServiceEmail = nameof(MemberNotificationServiceEmail);
                public const string ClaimCommunicationInbound = nameof(ClaimCommunicationInbound);
                public const string TCUInboundEmail = nameof(TCUInboundEmail);
                public const string MemberNotificationServiceSms = nameof(MemberNotificationServiceSms);
                public const string InsuranceContactService = nameof(InsuranceContactService);
                public const string SparkB2CRegressionForMemberCentralReleases = nameof(SparkB2CRegressionForMemberCentralReleases);
                public const string MultiFactorAuthentication = nameof(MultiFactorAuthentication);
                public const string CheckAutomationUsers = nameof(CheckAutomationUsers);
            };

            public enum ENV
            {
                SHIELDINT2,
                SHIELDSIT5,
                SHIELDSIT6,
                SHIELDSIT7,
                SHIELDSIT8,
                SHIELDUAT5,
                SHIELDUAT6,
                SHIELDUAT7,
            };

            public class Environment
            {
                public const string dev = nameof(dev);
                public const string sit = nameof(sit);
                public const string uat = nameof(uat);
            };

            #region Feature Toggles
            /// <summary>
            /// B2C Feature Toggles
            /// </summary>
            public enum B2CFeatureToggles
            {
                [Description("UseMemberCentralMock")]
                UseMemberCentralMock,
                [Description("SparkPolicyCancellations")]
                SparkPolicyCancellations,
                [Description("UseAddressManagementApi")]
                UseAddressManagementApi,
                [Description("MotorRiskAddress")]
                MotorRiskAddress,
                [Description("CycloneReInsurance")]
                CycloneReInsurance,
                [Description("UseCxOneAgentForClaimsWebChat")]
                UseCxOneAgentForClaimsWebChat,
                [Description("UseCxOneAgentForPolicyWebChat")]
                UseCxOneAgentForPolicyWebChat
            };

            /// <summary>
            /// Spark Feature Toggles
            /// </summary>
            public enum SparkFeatureToggles
            {
                [Description("Use MC Mock")]
                UseMCMock,               
                [Description("Use OTP Bypass")]
                UseBypassOTP,
                [Description("Use myRAC Login")]
                LoginMyRAC,
                [Description("Expect MFA for Home Storm Claims Change Contact Details")]
                HomeStormClaimsContactDetailsExpectMultiFactorAuthentication,
                [Description("Expect MFA for Motor Glass Claims Change Contact Details")]
                MotorGlassContactDetailsExpectMultiFactorAuthentication,
                [Description("Expect MFA for Motor Collision Claims Change Contact Details")]
                MotorCollisionContactDetailsExpectMultiFactorAuthentication,
                [Description("Expect MFA for Spark Endorsements Bank Details")]
                SparkEndorsementsBankExpectMultiFactorAuthentication,
                [Description("CHG More about your damage screen INSU-818")]
                ClaimsHomeMoreAboutYourDamage,
                [Description("Use Person V3")]
                UsePersonV3
            };

            /// <summary>
            /// Telephone number Feature Toggles
            /// </summary>
            public enum TelephoneFeatureToggles 
            {
                [Description("expect MNS SMS to be retrievable from Mailosaur Inbox")]
                SMSForMNSInMailosaur
            };
            #endregion

            /// <summary>
            /// Supports test framework operations where page waits
            /// are required. All times are in seconds.
            /// </summary>
            public class WaitTimes
            {
                public const int T5SEC = 5;
                public const int T10SEC = 10;
                public const int T30SEC = 30;
                public const int T60SEC = 60;
                public const int T90SEC = 90;
                public const int T150SEC = 150;
                public const int T600SEC = 600;  //10min
            };

            /// <summary>
            /// Intended for use with Thread.Sleep() calls where values are required
            /// in milliseconds.
            /// </summary>
            public class SleepTimes
            {
                public const int T500MS =    500;
                public const int T1SEC  =   1000;
                public const int T2SEC  =   2000;
                public const int T3SEC  =   3000;
                public const int T5SEC  =   5000;
                public const int T10SEC =  10000;
                public const int T30SEC =  30000;
                public const int T1MIN  =  60000;
                public const int T2MIN  = 120000;
                public const int T5MIN  = 300000;
                public const int T10MIN = 600000;
            };


            /// <summary>
            /// Date Time string format
            /// </summary>
            public class DateTimeTextFormat
            {
                public const string ddMMyyyy = "dd/MM/yyyy";
                public const string MMddyyyy = "MM/dd/yyyy";
            };

            public enum TargetBrowser
            {
                [Description("chrome")]
                Chrome,
                [Description("firefox")]
                Firefox,
                [Description("ie")]
                InternetExplorer,
                [Description("msedge")]
                Edge,
                [Description("safari")]
                Safari
            }
                

            public enum TargetDevice
            {
                [Description("Samsung Galaxy S21")]
                GalaxyS21,
                [Description("iPhone 14")]
                iPhone14,
                [Description("iPad 10th")]
                iPad10,
                [Description("MacBook")]
                MacBook,
                [Description("Windows 11")]
                Windows11
            }


            public enum PlatformName
            {
                [Description("Android")]
                Android,
                [Description("iOS")]
                iOS,
                [Description("macOS")]
                Mac,
                [Description("windows")]
                Windows
            };

            public class FileType
            {
                public const string PDFFile = "PDFIInvoice.pdf";
                public const string PNGFile = "PNGInvoice.png";
                public const string JPGFile1 = "Invoice1.jpg";
                public const string JPGFile2 = "Invoice2.jpg";
                public const string JPGFile3 = "Invoice3.jpg";
                public const string JPGFile4 = "Invoice4.jpg";
                public const string WordFile = "WordInvoice.docx";
                public const string PDFFile10MB = "PDFFile10MB.pdf";
            }

            public enum ShieldToggle
            {
                [Description("Toggle for HGC")]
                HomeGeneralClaims,
                [Description("Toogle_for_RefundFW_B2C")]
                Toogle_for_RefundFW_B2C
            }

            public static class MyRACURLs
            {
                public static readonly string LoginSIT = "https://cdvnets.ractest.com.au/api/oidc/SignIn";
                public static readonly string LoginUAT = "https://ractest.com.au/api/oidc/SignIn";
            }
        }
    }
}
