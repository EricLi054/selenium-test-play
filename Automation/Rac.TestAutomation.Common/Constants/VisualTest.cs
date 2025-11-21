
namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class VisualTest
        {
            public class BoatNewBusiness
            {
                private const string _Prefix                    = "BoatNB - ";
                public const string ImportantInformation        = _Prefix + "Important information";
                public const string LetsStart                   = _Prefix + "Let's start";
                public const string LetsStartDeclinedCover      = _Prefix + "Let's start - Declined Cover";
                public const string AboutYou                    = _Prefix + "About you";
                public const string BoatType                    = _Prefix + "Boat type";
                public const string YourBoat                    = _Prefix + "Your boat";
                public const string MoreBoat                    = _Prefix + "More boat";
                public const string YourQuote                   = _Prefix + "Here's your quote";
                public const string StartDate                   = _Prefix + "Start date";
                public const string YourDetails                 = _Prefix + "Your details";
                public const string YourRegistration            = _Prefix + "Your registration";
                public const string Payment                     = _Prefix + "Payment";
                public const string ConfirmationAnnualCash      = _Prefix + "Confirmation - Annual Cash paid via RealTime Credit Card";
                public const string ConfirmationDirectDebit     = _Prefix + "Confirmation - Direct Debit payments";
            }
            public class DividingFenceClaim
            {
                private const string _Prefix = "FenceClaim - ";
                public const string TriageEventType = _Prefix + "Triage event type";
                public const string TriageFenceOnly = _Prefix + "Triage Fence only";
                public const string TriageSharedFence = _Prefix + "Triage - Shared Fence";
                public const string TriageNonSharedFence = _Prefix + "Triage - Non-Shared Fence";
                public const string BeforeYouStart = _Prefix + "Before You start";
                public const string FindYou = _Prefix + "Let's find you";
                public const string FindYouErrorPage = _Prefix + "Let's find you error page";
                public const string FindYourPolicy = _Prefix + "Let's find your policy";
                public const string FindYourPolicyErrorPage = _Prefix + "Let's find your policy error page";
                public const string ContactDetails = _Prefix + "Contact details";
                public const string StartYourClaim = _Prefix + "Let's start your claim";
                public const string StartYourClaimErrorPage = _Prefix + "Let's start your claim error page";
                public const string YourDamageFence = _Prefix + "Your damage fence";
                public const string YourDamageFenceErrorPage = _Prefix + "Your damage fence error page";
                public const string SafetyAndSecurity = _Prefix + "Your safety and security";
                public const string SafetyAndSecurityErrorPage = _Prefix + "Your safety and security error page";
                public const string YourSettlementOptions = _Prefix + "Choose your claim settlement option";
                public const string YourSettlementOptionsErrorPage = _Prefix + "Choose your claim settlement option error page";
                public const string YourBankDetails = _Prefix + "Enter your bank details";
                public const string YourBankDetailsErrorPage = _Prefix + "Enter your bank details error page";
                public const string Confirmation = _Prefix + "Confirmation";
            }


            public class ClaimMotorGlass
            {
                private const string _Prefix = "ClaimMotorGlass - ";
                public const string Triage = _Prefix + "Car insurance claim";
                public const string BeforeYouStart = _Prefix + "Before You start";
                public const string ContactDetails = _Prefix + "Contact details";
                public const string YourPolicy = _Prefix + "Your policy";
                public const string YourPolicyErrorPage = _Prefix + "Your policy mandatory field validation";
                public const string StartYourClaim = _Prefix + "Let's start your claim";
                public const string StartYourClaimErrorPage = _Prefix + "Let's start your claim mandatory field validation";
                public const string YourGlassRepairs = _Prefix + "Your glass repairs";
                public const string YourGlassRepairsErrorPage = _Prefix + "Your glass repairs mandatory field validation";
                public const string YourGlassRepairs_NotFixed = _Prefix + "Your damage glass not fixed";
                public const string YourGlassRepairs_AlreadyFixed = _Prefix + "Your damage glass already fixed";
                public const string YourGlassRepairs_RepairBooked = _Prefix + "Your damage glass repair booked";
                public const string ReviewClaim_GlassNotFixed = _Prefix + "Review and submit your claim - glass not fixed";
                public const string ReviewClaim_GlassAlreadyFixed = _Prefix + "Review and submit your claim - glass already fixed";
                public const string ReviewClaim_GlassRepairBooked = _Prefix + "Review and submit your claim - glass repair booked";
                public const string Confirmation_GlassNotFixed = _Prefix + "Confirmation - glass not fixed";
                public const string Confirmation_GlassAlreadyFixed = _Prefix + "Confirmation - glass already fixed";
                public const string Confirmation_GlassRepairBooked = _Prefix + "Confirmation - glass repair booked";
                public const string Confirmation_PaymentBlock = _Prefix + "Confirmation - Payment Block";
            }

            public class DocumentUpload
            {
                private const string _Prefix = "DocumentUpload - ";
                public const string UploadAndSubmit = _Prefix + "Upload and submit page";
                public const string NoFileUploadedErrorMessage = UploadAndSubmit + " - No file uploaded error message";
                public const string AfterUploadFinished = UploadAndSubmit + " - Uploaded successfully";
                public const string Confirmation = _Prefix + "Confirmation page";
                public const string FileLimitReachedErrorPage = _Prefix + "File Limit Reached error page";
                public const string LinkExpiredErrorPage = _Prefix + "Link Expired error page";
                public const string SomethingWentWrongErrorPage = _Prefix + "Something went wrong error page";
                public const string SessionTimeOutErrorPage = _Prefix + "Session time out error page";
                public static string RemainingFileMessage(int remainingFileCount) => $"{_Prefix} You can upload {remainingFileCount} more files";
                public const string UnsupportedFileErrorMessage = UploadAndSubmit + " - Unsupported file error message";
                public const string ExceedsMaximumFileSizeErrorMessage = UploadAndSubmit + " - Exceeds file size error message";
            }

            public class ClaimsEFT
            {
                private const string _Prefix = "Claims EFT - ";
                public const string ChoosePreferredOptions = _Prefix + "Choose your preffered option";
                public const string ChoosePreferredOptionsFieldValidation = _Prefix + "Choose your preffered option - mandatory field validation";
                public const string AcceptCashSettlement = _Prefix + "Accept Cash Settlement - Selected";
                public const string DeclineCashSettlement = _Prefix + "Decline Cash Settlement - Selected";
                public const string VerifyItsYou = _Prefix + " - Let's verify it's you";
                public const string EnterYourBankDetails = _Prefix + " - Enter Your Bank Details";
                public const string EnterYourBankDetailsFieldValidation = _Prefix + " - Enter Your Bank Details - mandatory field validation";
                public const string AcceptCashSettlementConfirmation = _Prefix + "Accept CashSettlement Confirmation page";
                public const string DeclineCashSettlementConfirmation = _Prefix + "Decline CashSettlement Confirmation page";
                public const string EFTConfirmation = _Prefix + "New EFT Confirmation page";
                public const string Uhoh = _Prefix + "Error page";
            }

            public class ClaimMotorCollision
            {
                private const string _Prefix = "ClaimMotorCollision - ";
                public const string Triage = _Prefix + "Car insurance claim";
                public const string BeforeYouStart = _Prefix + "Before You start";
                public const string ContactDetails = _Prefix + "Contact details";
                public const string YourPolicy = _Prefix + "Your policy";
                public const string YourPolicyMandatoryFieldsValidation = _Prefix + "Your policy mandatory fields validation";               
                public const string StartYourClaim = _Prefix + "Let's start your claim";
                public const string StartYourClaimMandatoryFieldsValidation = _Prefix + "Let's start your claim mandatory fields validation";               
                public const string AboutTheAccident = _Prefix + "About the accident";
                public const string AboutTheAccidentMandatoryFieldsValidation = _Prefix + "About the accident mandatory fields validation";
                public const string MoreAboutTheAccident = _Prefix + "More about the accident";
                public const string MoreAboutTheAccidentMandatoryFieldsValidation = _Prefix + "More about the accident mandatory fields validation";
                public const string WhereAndHow = _Prefix + "Where and how it happened";
                public const string WhereAndHowFieldsValidation = _Prefix + "Where and how it happened mandatory fields validation";
                public const string DriverOfYourCar = _Prefix + "Driver of your car";
                public const string DriverOfYourCarFieldsValidation = _Prefix + "Driver of your car mandatory fields validation";
                public const string DriverHistory = _Prefix + "Driver history";
                public const string DriverHistoryFieldsValidation = _Prefix + "Driver history mandatory fields validation";

            }

            public class MotorcycleNewBusiness
            {
                private const string _Prefix = "MotorcycleNB - ";
                public const string SelectRACMember = _Prefix + "Select RAC member";
                public const string LetsStartWithYourBike = _Prefix + "Let's start with your bike";
                public const string TellUsMoreAboutYourBike = _Prefix + "Tell us more about your bike";
                public const string NowABitAboutYou = _Prefix + "Now a bit about you";
                public const string HeresYourQuote = _Prefix + "Here's your quote";
                public const string LetsClarifyFewMoreDetails = _Prefix + "Let's clarify few more details";
                public const string EnterMemberDetails = _Prefix + "Enter member details";
                public const string EnterPaymentDetailsAndPurchasePolicy = _Prefix + "Enter payment details and purchase policy";
                public const string ConfirmationPage = _Prefix + "Confirmation page";
            }

            public class CaravanNewBusiness
            {
                private const string _Prefix = "CaravanNB - ";
                public const string SelectRACMember = _Prefix + "Select RAC member";
                public const string LetsStartWithYourCaravan = _Prefix + "Let's start with your caravan";
                public const string TellUsMoreAboutYourCaravan = _Prefix + "Tell us more about your caravan";
                public const string NowABitAboutThePolicyholder = _Prefix + "Now A Bit About The Policyholder";
                public const string HeresYourQuote = _Prefix + "Here's your quote";
                public const string SetStartDate = _Prefix + "Set Start Date";
                public const string TellUsMoreAboutYou = _Prefix + "Tell Us More About You";
                public const string EnterPaymentDetailsAndPurchasePolicy = _Prefix + "Enter payment details and purchase policy";
                public const string ConfirmationPage = _Prefix + "Confirmation page";
            }
        }
    }
}
