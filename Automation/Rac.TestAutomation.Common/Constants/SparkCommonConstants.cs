
namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class SparkCommonConstants
        {
            /// <summary>
            /// Used to denote Refund destination on the SQL to search for.
            /// Also used to drive the test action to identify the expected condition
            /// </summary>
            public enum RefundToSource
            {
                None,
                RefundToUnknownCreditCard,
                RefundToKnownCreditCard,
                RefundToBankAccount
            };

            public class Header
            {
                public class Link
                {
                    public readonly static string RACI_TELEPHONE_NUMBER   = "tel:131703";
                }
            }
            public class Sidebar
            {
                public class Link
                {
                    public readonly static string PdsUrl                  = "https://cdvnetd.ractest.com.au/products/insurance/policy-documents/boat-insurance";
                }
            }
            public class Footer
            {
                public class Link
                {
                    public readonly static string PRIVACY_URL             = "https://cdvnetd.ractest.com.au/about-rac/site-info/privacy";
                    public readonly static string DISCLAIMER_URL          = "https://cdvnetd.ractest.com.au/about-rac/site-info/disclaimer";
                    public readonly static string SECURITY_URL            = "https://cdvnetd.ractest.com.au/about-rac/site-info/security";
                    public readonly static string ACCESSIBILITY_URL       = "https://cdvnetd.ractest.com.au/about-rac/site-info/accessibility";
                }
            }
            public class UnlistedInputs
            {
                public readonly static string FinancierNotFound = "Sterling Cooper Marine Finance";
                public readonly static string OtherBoatMake     = "Boat name - allows 33 characters!";
            }
            public class AdviseUser
            {
                public class FieldValidation
                {
                    public readonly static string SelectOne       = "Please select one";
                    public readonly static string EnterAValue     = "Please enter a value";
                    public readonly static string YesNoToggle     = "Please select Yes or No";
                }
                public class BoatFaqCard
                {
                    public readonly static string Head = "Frequently asked questions";
                    public readonly static string Body = "See our FAQs on boat insurance.";
                    public readonly static string Link = "https://cdvnetd.ractest.com.au/home-life/boat-insurance#faqs";
                }
            }
            public class UpdateUser
            {
                public readonly static string UserName = "SVC.INSSprkTstUs";
            }

            public class BSBValidation
            {
                public class InvalidBSB
                {
                    public readonly static string InvalidBSBNumber = "00000";
                    public readonly static string InvalidBSBWarningText = "Please enter a valid BSB";
                }
                public class NoMatchBSB
                {
                    public readonly static string NoMatchBSBNumber = "000001";
                    public readonly static string NoMatchBSBWarningText = "BSB not found. Please try a different BSB or call us";
                }
            }

            public static class MultiFactorAuthentication
            {
                public static class BypassOTP
                {
                    public readonly static string VerificationCode = "000000";
                }
                public static class IncorrectOTP
                {
                    public readonly static int AddToCurrentOTP = 1000;
                }
            }
        }
    }
}
