using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyHome;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using UIDriver.Pages.Spark.Endorsements;

namespace UIDriver.Pages.PCM
{
    public class PortfolioSummary : BasePage
    {
        /// <summary>
        /// Four different states that are presented to the member in regards to their
        /// claim. The numbers assigned are reflective of the numeric indexes that B2C
        /// uses to track which
        /// </summary>
        public enum CLAIM_PROGRESS_STATE
        {
            Lodge = 1,
            Assess = 2,
            Repair = 3,
            Complete = 4
        };

        private Dictionary<CLAIM_PROGRESS_STATE, string> _ClaimStatusIconLabels = new Dictionary<CLAIM_PROGRESS_STATE, string>()
        {
            {CLAIM_PROGRESS_STATE.Lodge, "Lodge"},
            {CLAIM_PROGRESS_STATE.Assess, "Assess"},
            {CLAIM_PROGRESS_STATE.Repair, "Repair"},
            {CLAIM_PROGRESS_STATE.Complete, "Complete"}
        };
		
        private Dictionary<CLAIM_PROGRESS_STATE, string> _ClaimStatusIconLabelsHome = new Dictionary<CLAIM_PROGRESS_STATE, string>()
        {
            {CLAIM_PROGRESS_STATE.Lodge, "Lodge"},
            {CLAIM_PROGRESS_STATE.Assess, "Assess"},
            {CLAIM_PROGRESS_STATE.Repair, "Repair or replace"},
            {CLAIM_PROGRESS_STATE.Complete, "Complete"}
        };
		
        public enum PCM_TAB { POLICIES, CLAIMS, MAILPREFERENCES };

        private enum POLICY_TYPE { Home, Motor, Pet, Caravan, Boat, Motorcycle };

        #region XPATHS
        private class XPath
        {
            public class Tabs
            {
                public const string MyPolcies                       = " //div[@id='wrapper']/div[contains(@class,'PCMMenu')]//a[@href='/Secure/PCM']";
                public const string MyClaims                        = " //div[@id='wrapper']/div[contains(@class,'PCMMenu')]//a[@href='/Secure/PCM/Claim']";
                public const string MailPreferences                 = " //div[@id='wrapper']/div[contains(@class,'PCMMenu')]//a[@href='/Secure/PCM/MailPreferences']";
            }
            public class Carousel
            {
                public const string Base                            = " //div[@id='wrapper']/div[@class='Carousel-Wrapper']//div[contains(@class,'owl-carousel')]";
                public const string Items                           = Base + "//div[contains(@class,'owl-item')]/div";
                public const string NextButton                      = " //div[contains(@class,'CarouselNav-Next')]";
                public const string PreviousButton                  = " //div[contains(@class,'CarouselNav-Previous')]";
                public const string ContentBase                     = " //div[@id='wrapper']//div[@id='CarouselContentArea']";
                public const string DriverDetailsSectionBase        = ContentBase + "//div[@class='PCMHeading' and text()='Driver details']/..";
            }
            public class MyPolicies
            {      
                public class PolicyOperations
                {
                public const string ButtonContainer                 = " //div[@class='PCMButtonArea']";
                public const string NoPoliciesContainer             = " //div[@id='wrapper']//div[contains(@class,'noPortfolioSection')]" +
                                                                      "//div[contains(@class,'AlertBox-Content')]";
                    public class Button
                    {
                        public const string CancelPolicy            = ButtonContainer + "//a/span[text()='Cancel my policy']/..";
                        public const string ChangeBankDetails       = ButtonContainer + "//a/span[text()='Change my banking details']/..";
                        public const string ChangeCarDetails        = ButtonContainer + "//a/span[text()='Change my car details']/..";
                        public const string ChangeCarKeptSuburb     = ButtonContainer + "//a/span[text()='Change where I keep my car']/..";
                        public const string ChangeHomeDetails       = ButtonContainer + "//a/span[text()='Change my home details']/..";
                        public const string GetCerticateOfCurrency  = ButtonContainer + "//a/span[text()='Get certificate of currency']/..";
                        public const string MakeClaim               = ButtonContainer + "//button/span[text()='Make a claim']/../..";
                        public const string MakeAPayment            = ButtonContainer + "//a/span[text()='Make a payment']/..";
                        public const string RenewPolicy             = ButtonContainer + "//a/span[text()='Renew my policy']/..";
                        public const string ReschedulePayment       = ButtonContainer + "//a/span[text()='Reschedule my next payment']/..";
                    }
                }
                public class Notifications
                {
                private const string PcmSection                     = " //div[@class='PCMSection']";
                    public const string PolicyInRenewalNotification = PcmSection + "//div[contains(@class,'AlertBox-Content') and contains(text(),'Your policy is currently in renewal and no further changes can be made online')]";
                public const string PolicyRenewalNotification       = " //div[contains(@class,'AlertBox-Content') and contains(text(),'Your policy is due for renewal')]";
                }
                public class PolicyDetails
                {
                    private const string SectionBase                    = Carousel.ContentBase + "//div[@class='PCMHeading' and text()='Policy details']/..";
                    public const string PolicyNumberAnswer              = SectionBase + "//div[@id='PolicyDetails_PolicyNumber_Answer']";
                    public const string CoverTypeAnswer                 = SectionBase + "//div[@id='PolicyDetails_ProductType_Answer']";
                    public const string RenewalDateAnswer               = SectionBase + "//div[@id='PolicyDetails_RenewalDate_Answer']";
                    public const string SumInsuredAnswer                = SectionBase + "//div[@id='PolicyDetails_SumInsured_Answer']";
                    public const string ExcessAnswer                    = SectionBase + "//div[@id='PolicyDetails_Excess_Answer']";
                    public const string BuildingSumInsuredAnswer        = SectionBase + "//div[@id='PolicyDetails_BuildingSumInsured_Answer']";
                    public const string BuildingExcessAnswer            = SectionBase + "//div[@id='PolicyDetails_BuildingExcess_Answer']";
                    public const string ContentsSumInsuredAnswer        = SectionBase + "//div[@id='PolicyDetails_ContentsSumInsured_Answer']";
                    public const string ContentsExcessAnswer            = SectionBase + "//div[@id='PolicyDetails_ContentsExcess_Answer']";
                    public const string AccidentalDamageExcessLabel     = " //label[@for='PolicyDetails_AccidentalDamageExcess']";
                    public const string AccidentalDamageExcessAnswer    = " //div[@id='PolicyDetails_AccidentalDamageExcess_Answer']";
                }
                public class MotorDetails
                {
                    private const string SectionBase                    = Carousel.ContentBase + "//div[@class='PCMHeading' and text()='Car details']/..";
                    public const string ParkingSuburb                   = SectionBase + "//div[@id='AssetDetails_Suburb_Answer']";
                    public const string ParkingRiskAddressAnswer        = " id('AssetDetails_RiskAddress_Answer')";
                    public const string RegistrationAnswer              = " //div[@id='AssetDetails_Registration_Answer']";
                    public const string UsageAnswer                     = " //div[@id='AssetDetails_CarUsage_Answer']";
                    public const string KmPerYearAnswer                 = " //div[@id='AssetDetails_KmPerYear_Answer']";
                    public const string MotorDescriptionAnswer          = " //div[@id='AssetDetails_CarDescription_Answer']";
                    public const string CaravanMailingAddressAnswer     = " //div[@id='PolicyDetails_MailingAddress_Answer']";
                    public const string CaravanMakeAnswer               = " //div[@id='AssetDetails_Make_Answer']";
                    public const string CaravanModelAnswer              = " //div[@id='AssetDetails_Model_Answer']";
                }
                public class HomeDetails
                {
                    public const string HomeAddressAnswer               = " //div[@id='AssetDetails_RiskAddress_Answer']";
                    public const string OccupancyStatusAnswer           = " //div[@id='AssetDetails_OccupancyType_Answer']";
                    public const string BuildingTypeAnswer              = " //div[@id='AssetDetails_BuildingType_Answer']";
                    public const string AlarmSystemAnswer               = " //div[@id='AssetDetails_AlarmSystem_Answer']";
                    public const string FinancierAnswer                 = " //div[@id='AssetDetails_Financier_Answer']";
                    // Home Details Check
                    public const string NoContinueToPolicyButon         = " //a[@id='homeDetailsContinueAction']";
                }
                public class PaymentDetails
                {
                    private const string SectionBase                    = Carousel.ContentBase + "//div[contains(@class,'PCMHeading')]//ancestor-or-self::div[contains(text(),'Payment details')]/../..";
                    public const string AnnualPremiumLabel              = Carousel.ContentBase + "//div[@data-wrapper-for='PaymentDetails_TotalInstallmentPremium']//label[@for='PaymentDetails_TotalInstallmentPremium']";
                    public const string AnnualPremiumAnswer             = SectionBase + "//div[@id='PaymentDetails_TotalInstallmentPremium_Answer']";
                    public const string PremiumFrequencyLabel           = Carousel.ContentBase + "//div[@data-wrapper-for='PaymentDetails_PaymentFrequency']//label[@for='PaymentDetails_PaymentFrequency']";
                    public const string PaymentFrequencyAnswer          = SectionBase + "//div[@id='PaymentDetails_PaymentFrequency_Answer']";
                    public const string PaymentMethodLabel              = Carousel.ContentBase + "//div[@data-wrapper-for='PaymentDetails_PaymentMethod']//label[@for='PaymentDetails_PaymentMethod']";
                    public const string PaymentMethodAnswer             = SectionBase + "//div[@id='PaymentDetails_PaymentMethod_Answer']";
                    public const string BsbLabel                        = Carousel.ContentBase + "//div[@data-wrapper-for='PaymentDetails_InstalmentDetail_BSB']//label[@for='PaymentDetails_InstalmentDetail_BSB']";
                    public const string CreditCardLabel                 = Carousel.ContentBase + "//div[@data-wrapper-for='PaymentDetails_InstalmentDetail_MaskedCreditCardNumber']//label[@for='PaymentDetails_InstalmentDetail_MaskedCreditCardNumber']";
                    public const string BankAccountNumberLabel          = Carousel.ContentBase + "//div[@data-wrapper-for='PaymentDetails_InstalmentDetail_BankAccountNumber']//label[@for='PaymentDetails_InstalmentDetail_BankAccountNumber']";
                    public const string NextInstallmentDateLabel        = Carousel.ContentBase + "//div[@data-wrapper-for='PaymentDetails_InstalmentDetail_NextInstalmentDate']//label[@for='PaymentDetails_InstalmentDetail_NextInstalmentDate']";
                    public const string NextInstallmentDateAnswer       = SectionBase + "//div[@id='PaymentDetails_InstalmentDetail_NextInstalmentDate_Answer']";
                    public const string NextInstallmentAmountLabel      = Carousel.ContentBase + "//div[@data-wrapper-for='PaymentDetails_InstalmentDetail_NextInstalmentAmount']//label[@for='PaymentDetails_InstalmentDetail_NextInstalmentAmount']";
                    public const string NextInstallmentAmountAnswer     = SectionBase + "//div[@id='PaymentDetails_InstalmentDetail_NextInstalmentAmount_Answer']";
                }
                public class BankDetails
                {
                    public const string BsbAnswer                       = " //div[@id='PaymentDetails_InstalmentDetail_BSB_Answer']";
                    public const string AccountNumberAnswer             = " //div[@id='PaymentDetails_InstalmentDetail_BankAccountNumber_Answer']";
                }
                public class ExcessDetails
                {
                    public static readonly string Paragraph1            = " //div[@class='excess-fine-print body-text row']//p[1]";
                }
            }
            public class MyClaims
            {
                public const string SectionBase                     = Carousel.ContentBase + "//div[@class='PCMHeading' and text()='Claim details']/..";
                public class  ClaimsDetails
                {
                    public const string ClaimNumber                 = SectionBase + "//div[@id='ClaimDetails_ClaimNumber_Answer']";
                    public const string PolicyNumber                = SectionBase + "//div[@id='ClaimDetails_PolicyNumber_Answer']";
                }
                public class ProgresBar
                {
                    public const string CurrentIconLabel            = " //div[contains(@class,'progtrckr-inprogress')]/div[@class='step-container']//span";
                    public const string LodgeHelpIcon               = " //div[contains(@class,'pgbar')]//a[contains(@data-content-key,'Lodged')]";
                    public const string AssessHelpIcon              = " //div[contains(@class,'pgbar')]//a[contains(@data-content-key,'Assess')]";
                    public const string RepairHelpIcon              = " //div[contains(@class,'pgbar')]//a[contains(@data-content-key,'Repair')]";
                    public const string BarFrame                    = " //div[contains(@class,'pgbar') and contains(@class,'helptext')]";
                    public const string TextHeader                  = " //div[contains(@class,'inProgressIndicator')]/div[contains(@class,'PCMHeadingText')]";
                    public const string TextBody                    = " //div[contains(@class,'inProgressIndicator')]/div[@class='claim-status-helptext']";
                }
            }
            public class MarketingSiderBar
            {
                public const string GetAQuoteButton                 = " //div[@id='sidebar-assist-marketing']//a[@id='quote_product_button']";
            }
            public class NewQuote
            {
                public class ProductSelectionDialog
                {
                    public const string SectionBase                 = " //div[@id='quoteProductSelectionDialog']";
                    public const string CarInsuranceProduct         = SectionBase + "//div[@class='modal-body']/div[1]/a[text()[contains(.,'Car insurance')]]";
                }
                public class ForYourselfDialog
                {
                    public const string SectionBase                 = " //div[@id='prePopulatedValidationQuestionsDialog']";
                    public const string ForselfYesLabel             = SectionBase + "//div[@id='IsTheQuoteForYourself_True_Label']";
                    public const string ForselfNoLabel              = SectionBase + "//div[@id='IsTheQuoteForYourself_False_Label']";
                    public const string DriverYesLabel              = SectionBase + "//div[@id='WillYouBeTheMainDriver_True_Label']";
                    public const string DriverNoLabel               = SectionBase + "//div[@id='WillYouBeTheMainDriver_False_Label']";
                    public const string ForYourselfErrorBox         = SectionBase + "//div[@data-wrapper-for='IsTheQuoteForYourself']/..//span[@class='field-validation-error']";
                    public const string MainDriverfErrorBox         = SectionBase + "//div[@data-wrapper-for='WillYouBeTheMainDriver']/..//span[@class='field-validation-error']";
                    public const string ErrorBoxes                  = SectionBase + "//span[@class='field-validation-error']";
                    public const string ContinueButton              = SectionBase + "//span[@class='btn-text' and text()='Continue']/..";
                }
                public class QuoteMemberDetailsDialog
                {
                    public const string SectionBase                 = " //div[@id='quoteMemberDetailsDialog']";
                    public const string PrefillNameAnswer           = SectionBase + "//div[@id='FullName_Answer']";
                    public const string PrefillGenderAnswer         = SectionBase + "//div[@id='GenderDescription_Answer']";
                    public const string PrefillDobAnswer            = SectionBase + "//div[@data-wrapper-for='Dob']//div[@class='display-answer']";
                    public const string PrefillMemberAnswer         = SectionBase + "//div[@id='MembershipDescription_Answer']";
                    public const string PrefillAddressAnswer        = SectionBase + "//div[@id='MailingAddress_Answer']";
                    public const string PrefillEmailAnswer          = SectionBase + "//div[@id='EmailAddress_Answer']";
                    public const string PrefillPhoneAnswer          = SectionBase + "//div[@id='PreferredContactNumber_Answer']";
                    public const string PrefillYesButton            = SectionBase + "//span[@class='btn-text' and text()='Yes']/..";
                    public const string PrefillNoButton             = SectionBase + "//span[@class='btn-text' and text()='No']/..";
                }
            }
        }
        #endregion
        #region CONSTANTS
        private class Constant
        {
            /// <summary>
            /// TODO INSU-286 to remove the NCB project toggle 
            /// The toggle will be removed 1 year 28 days after the actual go-live, around August 2025
            /// 46 is the Product Version number when this NCB project online
            /// Here is to check the text before (Pre46) and after (Post46) the new Product Version number
            /// </summary>
            public class Pre46
            {   
                public static readonly string AccidentalDamageText  = "Contents accidental damage excess";
                public static readonly string ExcessMsgOwner        = "The total of your basic excess is specified above. The standard basic excess is $200 per policy.\r\n\r\nIf you have chosen standard basic excesses and make a claim where one event impacts combined policies, then the basic excess is capped at a total of $200. However, if you choose to accept an excess other than the standard basic excess, and make a claim involving both your building and contents policies, then you will need to pay your basic excess for both your building and your contents policy.";
                public static readonly string ExcessMsgInvest       = "The total of your basic excess is specified above. The standard basic excess is $500 per policy.\r\n\r\nIf you have chosen standard basic excesses and make a claim where one event impacts combined policies, then the basic excess is capped at a total of $500. However, if you choose to accept an excess other than the standard basic excess, and make a claim involving both your building and contents policies, then you will need to pay your basic excess for both your building and your contents policy.";
                public static readonly string AccidentalDamageValue = "$350";
            }
            public class Post46
            {
                public static readonly string AccidentalDamageText  = "Contents accidental damage excess";
                public static readonly string ExcessMsg             = "The excess is the amount you will need to pay towards settlement of any claim.\r\n\r\nIf you make a combined building and contents claim, the higher of the two excesses will apply.\r\n\r\nIf you adjust your excess, your premium will change.";
                public static readonly string AccidentalDamageValue = "$500";
            }
        }
        #endregion

        #region General Settable properties and controls
        /// <summary>
        /// returns BSB Number 
        /// from Policy Payment details view.
        /// </summary>
        /// <returns></returns>
        public string GetBSB => GetInnerText(XPath.MyPolicies.BankDetails.BsbAnswer);

        /// <summary>
        /// returns Accoumt Number 
        /// from Policy Payment details view.
        /// </summary>
        /// <returns></returns>
        public string GetAccountNumber => GetInnerText(XPath.MyPolicies.BankDetails.AccountNumberAnswer);

        /// <summary>
        /// Fetch Mailing Address from the "Policy details" section for this policy.
        /// </summary>
        /// <returns></returns>
        public string GetMailingAddressFromPolicySummary => GetInnerText(XPath.MyPolicies.MotorDetails.CaravanMailingAddressAnswer);

        /// <summary>
        /// Fetch Caravan Registraiton.
        /// </summary>
        /// <returns></returns>
        public string GetCaravanRegistration => GetInnerText(XPath.MyPolicies.MotorDetails.RegistrationAnswer);

        /// <summary>
        /// Fetches or changes the current PCM tab that is selected.
        /// </summary>
        /// <exception cref="InvalidSelectorException">Thrown if we don't recognise which tab we're on.</exception>
        public PCM_TAB CurrentPCMTab
        {
            get
            {
                if (GetClass(XPath.Tabs.MyPolcies).Contains("is-selected"))
                { return PCM_TAB.POLICIES; }
                if (GetClass(XPath.Tabs.MyClaims).Contains("is-selected"))
                { return PCM_TAB.CLAIMS; }
                if (GetClass(XPath.Tabs.MailPreferences).Contains("is-selected"))
                { return PCM_TAB.MAILPREFERENCES; }

                throw new InvalidSelectorException("PortfolioSummary was unable to determine which PCM view it is in.");
            }
            set
            {
                string desiredXPath = "";
                switch (value)
                {
                    case PCM_TAB.POLICIES:
                        desiredXPath = XPath.Tabs.MyPolcies;
                        break;
                    case PCM_TAB.CLAIMS:
                        desiredXPath = XPath.Tabs.MyClaims;
                        break;
                    case PCM_TAB.MAILPREFERENCES:
                        desiredXPath = XPath.Tabs.MailPreferences;
                        break;
                    default:
                        throw new InvalidSelectorException("PortfolioSummary was unable to determine which PCM view it is in.");
                }

                if (!GetClass(desiredXPath).Contains("is-selected"))
                    ClickControl(desiredXPath);

                var endTime = DateTime.Now.AddSeconds(WaitTimes.T60SEC);
                var success = false;
                do
                {
                    if (CurrentPCMTab == value)
                    {
                        success = true;
                        break;
                    }
                    Thread.Sleep(1000);
                } while (DateTime.Now < endTime);
                if (!success)
                { Reporting.Error("We did not appear to switch to desired tab within a reasonable period of time."); }
            }
        }

        /// <summary>
        /// For both policy view and claim view, returns an enum
        /// indicating the type of policy that relates to the
        /// currently displayed carousel item.
        /// </summary>
        private POLICY_TYPE CurrentCarouselItemPolicyType
        {
            get
            {
                var carouselControl = _driver.FindElement(By.XPath($"{XPath.Carousel.Items}[contains(@class,'is-selected')]"));
                if (!Enum.TryParse(carouselControl.GetAttribute("data-policy-type"), out POLICY_TYPE policyType))
                { Reporting.Error("Unrecognised type of policy for current carousel item."); }
                return policyType;
            }
        }
        #endregion

        #region Policy Settable properties and controls
        // Value retrived from the currently displayed Policy details
        public string CurrentPolicyPolicyNumber => GetInnerText(XPath.MyPolicies.PolicyDetails.PolicyNumberAnswer);

        public string RenewalDate => GetInnerText(XPath.MyPolicies.PolicyDetails.RenewalDateAnswer);

        public bool IsSumInsuredDisplayed => _driver.FindElements(By.XPath(XPath.MyPolicies.PolicyDetails.SumInsuredAnswer)).Any();
        public int SumInsured => int.Parse(GetInnerText(XPath.MyPolicies.PolicyDetails.SumInsuredAnswer).StripMoneyNotations());

        public int Excess
        {
            get
            {
                var excessText = GetInnerText(XPath.MyPolicies.PolicyDetails.ExcessAnswer).StripMoneyNotations();
                if (excessText.Equals("Removed", StringComparison.InvariantCultureIgnoreCase))
                    return 0;
                return int.Parse(excessText);
            }
        }

        /// <summary>
        /// Returns the enumeration representing a general motor cover
        /// for the current policy.
        /// </summary>
        /// <exception cref="exception">thrown if determines that the current policy is not a motor policy.</exception>
        public MotorCovers MotorCover
        {
            get
            {
                if (!CurrentPolicyPolicyNumber.StartsWith("MGP"))
                { Reporting.Error("Current policy needs to be a motor policy in order to get Motor Cover type."); }

                return MotorCoverNameMappings.First(x => x.Value.TextB2C.Equals(GetInnerText(XPath.MyPolicies.PolicyDetails.CoverTypeAnswer), StringComparison.InvariantCultureIgnoreCase)).Key;
            }
        }

        /// <summary>
        /// Returns whether the renewal notice is current being displayed for the current policy.
        /// The renewal notice is an text block shown below the action buttons which explains
        /// what renewal means and what is involved to the member.
        /// </summary>
        public bool IsRenewalNoticeShown => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.MyPolicies.Notifications.PolicyRenewalNotification), WaitTimes.T5SEC, out IWebElement element);

        /// <summary>
        /// Returns whether the 'Renew My Policy' button is current being displayed for the current policy.
        /// </summary>
        public bool IsRenewalButtonShown => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.MyPolicies.PolicyOperations.Button.RenewPolicy),
                                                WaitTimes.T5SEC, out IWebElement element);

        /// <summary>
        /// Returns whether the 'Pay My Policy' button is current being displayed for the current policy.
        /// </summary>
        public bool IsMakeAPaymentButtonShown => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.MyPolicies.PolicyOperations.Button.MakeAPayment),
                                                    WaitTimes.T5SEC, out IWebElement element);

        /// <summary>
        /// Returns whether the 'Cancel my Policy' button is current being displayed for the current policy.
        /// </summary>
        public bool IsCancelMyPolicyButtonShown => _driver.TryWaitForElementToBeVisible(
                                                       By.XPath(XPath.MyPolicies.PolicyOperations.Button.CancelPolicy),
                                                       WaitTimes.T5SEC, 
                                                       out IWebElement element);

        public string MotorPolicyParkingSuburb => GetInnerText(XPath.MyPolicies.MotorDetails.ParkingSuburb);

        public string MotorPolicyParkingRiskAddress => GetInnerText(XPath.MyPolicies.MotorDetails.ParkingRiskAddressAnswer);

        public decimal PolicyAnnualPremium => decimal.Parse(GetInnerText(XPath.MyPolicies.PaymentDetails.AnnualPremiumAnswer).StripMoneyNotations());

        public decimal PolicyNextInstalmentAmount => decimal.Parse(GetInnerText(XPath.MyPolicies.PaymentDetails.NextInstallmentAmountAnswer).StripMoneyNotations());

        public DateTime PolicyNextInstalmentDate => DateTime.Parse(GetInnerText(XPath.MyPolicies.PaymentDetails.NextInstallmentDateAnswer));
 
        public DateTime PolicyRenewalDate => DateTime.Parse(GetInnerText(XPath.MyPolicies.PolicyDetails.RenewalDateAnswer));

        /// <summary>
        /// Returns the enumeration representing a general home cover for the current policy.
        /// </summary>
        /// <exception cref="exception">thrown if determines that the current policy is not a home policy.</exception>
        public HomeCover HomeCover
        {
            get
            {
                if (!CurrentPolicyPolicyNumber.StartsWith("HGP"))
                { Reporting.Error("Current policy needs to be a home policy in order to get Home Cover type."); }

                return DataHelper.GetValueFromDescription<HomeCover>(GetInnerText(XPath.MyPolicies.PolicyDetails.CoverTypeAnswer));
            }
        }

        public int BuildingSumInsured => int.Parse(GetInnerText(XPath.MyPolicies.PolicyDetails.BuildingSumInsuredAnswer).StripMoneyNotations());

        public string BuildingExcess
        {
            get
            {
                var excessText = GetInnerText(XPath.MyPolicies.PolicyDetails.BuildingExcessAnswer).StripMoneyNotations();

                return excessText.Equals("Removed", StringComparison.InvariantCultureIgnoreCase) ? "0" : excessText;
            }
        }

        public int ContentsSumInsured => int.Parse(GetInnerText(XPath.MyPolicies.PolicyDetails.ContentsSumInsuredAnswer).StripMoneyNotations());

        public string ContentsExcess
        {
            get
            {
                var excessText = GetInnerText(XPath.MyPolicies.PolicyDetails.ContentsExcessAnswer).StripMoneyNotations();

                return excessText.Equals("Removed", StringComparison.InvariantCultureIgnoreCase) ? "0" : excessText;
            }
        }
        #endregion

        /// <summary>
        /// Verifies that the 'Your policy is currently in renewal' message appears in PCM
        /// </summary>
        /// <returns></returns>
        public bool IsRenewalAccepted() => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.MyPolicies.Notifications.PolicyInRenewalNotification), WaitTimes.T30SEC, out IWebElement element);


        #region Claims Settable properties and controls
        public string CurrentClaimPolicyNumber => GetInnerText(XPath.MyClaims.ClaimsDetails.PolicyNumber);

        public string CurrentClaimClaimNumber => GetInnerText(XPath.MyClaims.ClaimsDetails.ClaimNumber);

        /// <summary>
        /// Returns which claim progress state is represented by the vehicle icon, indicating to
        /// the member where their claim is currently at.
        /// </summary>
        public CLAIM_PROGRESS_STATE ClaimCurrentStateIcon
        {
            get
            {
                // As we're unsure of previous actions, we don't know if this will still
                // be getting fetched from Shield. So adding wait to accomodate loading.
                _driver.WaitForElementToBeVisible(By.XPath(XPath.MyClaims.ProgresBar.CurrentIconLabel), WaitTimes.T30SEC);
                var currentIconLabel = GetInnerText(XPath.MyClaims.ProgresBar.CurrentIconLabel);

                return CurrentCarouselItemPolicyType == POLICY_TYPE.Motor ?
                       _ClaimStatusIconLabels.First(x => x.Value == currentIconLabel).Key :
                       _ClaimStatusIconLabelsHome.First(x => x.Value == currentIconLabel).Key;
            }
        }
		
        /// <summary>
        /// Returns which claim progress state has its help text currently showing.
        /// </summary>
        public CLAIM_PROGRESS_STATE ClaimCurrentReferencedHelpIcon
        {
            get
            {
                var classAttributeText = GetClass(XPath.MyClaims.ProgresBar.BarFrame);

                var regex = new Regex(@"large-offset-(\d)");
                Match match = regex.Match(classAttributeText);
                if (match.Success && match.Groups.Count == 2)
                {
                    Reporting.Log($"Detecting dialog {match.Groups[1].Value}", _browser.Driver.TakeSnapshot());
                    return (CLAIM_PROGRESS_STATE)int.Parse(match.Groups[1].Value);
                }

                // If Progress bar exists, but without offset attribute, then we are
                // on final complete step (no Progress/What-to-expect dialog).
                return CLAIM_PROGRESS_STATE.Complete;
            }
            set
            {
                var xPath = string.Empty;
                switch (value)
                {
                    case CLAIM_PROGRESS_STATE.Lodge:
                        xPath = XPath.MyClaims.ProgresBar.LodgeHelpIcon;
                        break;
                    case CLAIM_PROGRESS_STATE.Assess:
                        xPath = XPath.MyClaims.ProgresBar.AssessHelpIcon;
                        break;
                    case CLAIM_PROGRESS_STATE.Repair:
                        xPath = XPath.MyClaims.ProgresBar.RepairHelpIcon;
                        break;
                    case CLAIM_PROGRESS_STATE.Complete:
                    default:
                        Reporting.Error("Only the Lodge/Assess/Repair statuses have a help text icon");
                        break;
                }
                ClickControl(xPath);

                var endTime = DateTime.Now.AddSeconds(WaitTimes.T5SEC);
                var success = false;
                do
                {
                    if (ClaimCurrentReferencedHelpIcon == value)
                    {
                        WaitForClaimStatusHelpTextToLoad();
                        success = true;
                        break;
                    }
                    Thread.Sleep(1000);
                } while (DateTime.Now < endTime);
                if (!success)
                { Reporting.Error("We did not appear to switch to desired help icon within a reasonable period of time."); }

                Thread.Sleep(3000);
            }
        }
        

        public string ClaimStatusHelpTextHeader => GetInnerText(XPath.MyClaims.ProgresBar.TextHeader);

        public string ClaimStatusHelpTextBody => GetInnerText(XPath.MyClaims.ProgresBar.TextBody);
        #endregion

        private enum RENDER_STATE { UNKNOWN, NO_POLICIES, HAS_POLICIES };
        private RENDER_STATE _state;

        /// <summary>
        /// Generic item for returning basic info for what items are in the
        /// PCM carousel. This is used for Policy and Claims views.
        /// </summary>
        private class PCMItemTabState
        {
            /// <summary>
            /// This hosts either the policy number of the claim number
            /// depending on the context in which its called.
            /// </summary>
            public string ReferenceNumber { get; set; }
            public bool   IsVisible { get; set; }
            public bool   IsSelected { get; set; }
        }

        public PortfolioSummary(Browser browser) : base(browser)
        {
            _state = RENDER_STATE.UNKNOWN;
        }

        public override bool IsDisplayed()
        {
            IWebElement element;
            var rendered = false;

            try
            {
                var loggingContext = string.Empty;
                GetElement(XPath.Tabs.MyPolcies);
                GetElement(XPath.Tabs.MyClaims);
                GetElement(XPath.Tabs.MailPreferences);

                if (_driver.TryFindElement(By.XPath(XPath.MyPolicies.PolicyOperations.NoPoliciesContainer), out element))
                {
                    rendered = true;
                    _state = RENDER_STATE.NO_POLICIES;
                }
                else if (_driver.TryFindElement(By.XPath(XPath.Carousel.Base), out element))
                {
                    _state = RENDER_STATE.HAS_POLICIES;
                    if (CurrentPCMTab == PCM_TAB.POLICIES)
                    {
                        GetElement(XPath.Carousel.ContentBase);
                        GetElement(XPath.MyPolicies.PolicyDetails.PolicyNumberAnswer);
                        // If made to here, then the policy details have rendered.

                        loggingContext = $"PCM displaying policy: {CurrentPolicyPolicyNumber}";
                    }
                    else if (CurrentPCMTab == PCM_TAB.CLAIMS)
                    {
                        GetElement(XPath.Carousel.ContentBase);
                        GetElement(XPath.MyClaims.ClaimsDetails.ClaimNumber);
                        GetElement(XPath.MyClaims.ClaimsDetails.PolicyNumber);

                        loggingContext = $"PCM displaying claim: {CurrentClaimClaimNumber}";
                    }

                    Reporting.LogPageChange(loggingContext);
                    rendered = true;
                }
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }

        public void ViewSpecificPolicy(string policyNumber)
        {
            if (CurrentPCMTab != PCM_TAB.POLICIES)
            { Reporting.Error("Not on the policies view, so unable to search for policy."); }

            var configuredPolicyDisplayLimit = GetCarouselPolicyDisplayLimit();
            var allPolicies = GetPolicyCarouselTabs();

            if (allPolicies.FirstOrDefault(x => x.ReferenceNumber == policyNumber) == null)
            { Reporting.Error($"Desired policy of {policyNumber} was not found.", _driver.TakeSnapshot()); }

            // If we're already on policy then return.
            if (policyNumber.Equals(CurrentlySelectedCarouselItem(), StringComparison.InvariantCultureIgnoreCase))
            { return; }

            var desiredPolicyTabXPath = $"{XPath.Carousel.Items}[@data-policy-number='{policyNumber.ToUpper()}']";
            if (allPolicies.Count <= configuredPolicyDisplayLimit)
            {
                ClickControl(desiredPolicyTabXPath);
            }
            else
            {
                var found = false;
                for (int i = 0; i < allPolicies.Count && !found; i++)
                {
                    // Click the "Next" button.
                    ClickControl(XPath.Carousel.NextButton);
                    // Tab updates quite quickly, but allow a brief sleep to be sure.
                    Thread.Sleep(1000);

                    found = policyNumber.Equals(CurrentlySelectedCarouselItem(), StringComparison.InvariantCultureIgnoreCase);
                }
            }

            // Allow for some of the content to render:
            //   - Wait for policy operation buttons to render
            _driver.WaitForElementToBeVisible(By.XPath(XPath.MyPolicies.PolicyOperations.ButtonContainer), WaitTimes.T30SEC);

            var policyNumberFromDetails = _driver.WaitForElementToBeVisible(By.XPath(XPath.MyPolicies.PolicyDetails.PolicyNumberAnswer), WaitTimes.T30SEC).Text.Trim();
            Reporting.AreEqual(policyNumber, policyNumberFromDetails, true);

            return;
        }

        public void VerifyExcessText(EndorseHome testData)
        {
            var policyDetails = DataHelper.GetPolicyDetails(testData.PolicyNumber);

            if (policyDetails.HasAccidentalDamageCover())
            {
                Reporting.AreEqual(Constant.Post46.AccidentalDamageText, GetInnerText(XPath.MyPolicies.PolicyDetails.AccidentalDamageExcessLabel), 
                    "text field displays when the policy has Accidental Damage covered");
                Reporting.AreEqual(Constant.Post46.AccidentalDamageValue, GetInnerText(XPath.MyPolicies.PolicyDetails.AccidentalDamageExcessAnswer), 
                    "Accidental Damage Value displays");
            }
            else
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.MyPolicies.PolicyDetails.AccidentalDamageExcessLabel), 
                    "to confirm Accidental Damage excess text is not displayed when policy does not have that cover.");
                Reporting.IsFalse(IsControlDisplayed(XPath.MyPolicies.PolicyDetails.AccidentalDamageExcessAnswer), 
                    "to confirm Accidental Damage excess value is not displayed when policy does not have that cover.");
            }

            Reporting.AreEqual(Constant.Post46.ExcessMsg, GetInnerText(XPath.MyPolicies.ExcessDetails.Paragraph1), 
                "information in the Excess section displays correctly");
        }
        
        public void ViewSpecificClaim(string claimNumber)
        {
            if (CurrentPCMTab != PCM_TAB.CLAIMS)
            { Reporting.Error("Not on the claims view, so unable to search for claim."); }

            var configuredPolicyDisplayLimit = GetCarouselPolicyDisplayLimit();
            var allPolicies = GetPolicyCarouselTabs();

            if (allPolicies.FirstOrDefault(x => x.ReferenceNumber == claimNumber) == null)
            { Reporting.Error($"Desired claim of {claimNumber} was not found.", _driver.TakeSnapshot()); }

            // If we're not already on claim then navigate.
            if (!claimNumber.Equals(CurrentlySelectedCarouselItem(), StringComparison.InvariantCultureIgnoreCase))
            {

                var desiredPolicyTabXPath = $"{XPath.Carousel.Items}[@data-claim-number='{claimNumber.ToUpper()}']";
                if (allPolicies.Count <= configuredPolicyDisplayLimit)
                {                    
                    ClickControl(desiredPolicyTabXPath);
                }
                else
                {
                    var found = false;
                    for (int i = 0; i < allPolicies.Count && !found; i++)
                    {
                        // Click the "Next" button.
                        ClickControl(XPath.Carousel.NextButton);
                        // Tab updates quite quickly, but allow a brief sleep to be sure.
                        Thread.Sleep(1000);

                        found = claimNumber.Equals(CurrentlySelectedCarouselItem(), StringComparison.InvariantCultureIgnoreCase);
                    }
                }
            }
            // Wait for core claim content to render:
            var claimNumberFromDetails = WaitForClaimProgressDetailsToRender();
            Reporting.AreEqual(claimNumber, claimNumberFromDetails, true);

            return;
        }

        /// <summary>
        /// Clicks the "Get a quote" button to trigger the new quote pop up
        /// which will then allow the user to select a product to begin a new
        /// quote flow.
        /// </summary>
        public void ClickGetAQuote() => ClickControl(XPath.MarketingSiderBar.GetAQuoteButton);

        /// <summary>
        /// Clicks the "Renew My Policy" button to trigger the policy renewal process.
        /// </summary>
        public void ClickRenewMyPolicy() => ClickControl(XPath.MyPolicies.PolicyOperations.Button.RenewPolicy,
                                                         waitTimeSeconds: WaitTimes.T30SEC);

        /// <summary>
        /// Clicks the "Pay My Policy" button to trigger the policy payment.
        /// </summary>
        public void ClickMakeAPayment() => ClickControl(XPath.MyPolicies.PolicyOperations.Button.MakeAPayment,
                                                       waitTimeSeconds: WaitTimes.T30SEC);

        /// <summary>
        /// Clicks the "Reschedule My Next Payment" button to start policy payment reschedule.
        /// </summary>
        public void ClickRescheduleMyNextPayment() => ClickControl(XPath.MyPolicies.PolicyOperations.Button.ReschedulePayment,
                                                                   waitTimeSeconds: WaitTimes.T30SEC);
 
        /// <summary>
        /// Clicks the "No Continue To Policy" button to begin the
        /// related renewal process on the current home policy.
        /// </summary>
        public void ClickNoContinueToPolicy()
        {
            Thread.Sleep(2000);
            try
            {
                ClickControl(XPath.MyPolicies.HomeDetails.NoContinueToPolicyButon, waitTimeSeconds: WaitTimes.T30SEC);
            }
            catch (NoSuchElementException)
            {
                Reporting.Error("No Continue to Policy button was not present at this time.");
            }
        }

        /// <summary>
        /// Clicks the "Make a claim" button to begin lodging a new online
        /// claim for the currently displayed policy.
        /// </summary>
        public void ClickMakeAClaim() => ClickControl(XPath.MyPolicies.PolicyOperations.Button.MakeClaim, 
                                                      waitTimeSeconds: WaitTimes.T30SEC);

        /// <summary>
        /// Clicks the "Change my home details" button to begin the
        /// related endorsement process on the current home policy.
        /// </summary>
        public void ClickChangeMyHomeDetails() => ClickControl(XPath.MyPolicies.PolicyOperations.Button.ChangeHomeDetails,
                                                               waitTimeSeconds: WaitTimes.T30SEC);

        /// <summary>
        /// Handle the scenario of expecting the new quote pop-up
        /// dialog which has a choice of all the different products
        /// that they can get a quote for, and selecting a new car
        /// insurance quote.
        /// </summary>
        public void NewQuotePromptClickCarInsurance()
        {
            var endTime = DateTime.Now.AddSeconds(30);
            var success = false;
            IWebElement button;
            do
            {
                button = null;
                _driver.WaitForElement(By.XPath(XPath.NewQuote.ProductSelectionDialog.SectionBase), WaitTimes.T30SEC);
                if (_driver.TryFindElement(By.XPath(XPath.NewQuote.ProductSelectionDialog.CarInsuranceProduct), out button))
                {
                    var spanElement = GetElement(XPath.NewQuote.ProductSelectionDialog.CarInsuranceProduct + "/span");
                    Reporting.Log($"Button contains " + spanElement.GetAttribute("class") + "  " + button.Text);
                    if (button.Displayed && button.Enabled)
                    {
                        button.Click();
                        success = true;
                        Thread.Sleep(1000);
                    }
                }
            } while (!success && DateTime.Now < endTime);

            if (!success)
            { Reporting.Error("Couldn't find or click the button within a reasonable period of time."); }
        }

        /// <summary>
        /// This method answers the initial questions about who the new
        /// car insurance quote will be for, in order to determine
        /// whether it should perform prefill of details.
        /// </summary>
        /// <param name="forSelf">TRUE/FALSE for whether the quote is for current PCM user. If NULL, automation will not answer this question.</param>
        /// <param name="mainDriver">TRUE/FALSE for whether the main driver will be the current PCM user. If NULL, automation will not answer this question.</param>
        public void NewQuotePromptAnswerRadioButtons(bool? forSelf, bool? mainDriver)
        {
            _driver.WaitForElement(By.XPath(XPath.NewQuote.ForYourselfDialog.SectionBase), WaitTimes.T30SEC);

            if (forSelf.HasValue)
            {
                var radioToClick = forSelf.Value ? XPath.NewQuote.ForYourselfDialog.ForselfYesLabel : XPath.NewQuote.ForYourselfDialog.ForselfNoLabel;
                ClickControl(radioToClick);
            }
            if (mainDriver.HasValue)
            {
                var radioToClick = mainDriver.Value ? XPath.NewQuote.ForYourselfDialog.DriverYesLabel : XPath.NewQuote.ForYourselfDialog.DriverNoLabel;
                ClickControl(radioToClick);
            }

            ClickControl(XPath.NewQuote.ForYourselfDialog.ContinueButton);
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Looks at the radio buttons and the current values, then checks whether
        /// the appropriate validation messages are visibly displayed, with the
        /// assumption that the user had just tried to submit.
        /// </summary>
        /// <returns>true that the validations are as expected, otherwise false</returns>
        public bool VerifyNewQuoteRadioButtonValidations()
        {
            bool success = true;

            try
            {
                // Check 'for yourself' buttons
                IWebElement errorBox = null;

                if (GetClass($"{XPath.NewQuote.ForYourselfDialog.ForselfYesLabel}/span[contains(@class,'rb-radio')]").Contains("checked") ||
                    GetClass($"{XPath.NewQuote.ForYourselfDialog.ForselfNoLabel}/span[contains(@class,'rb-radio')]").Contains("checked"))
                {
                    if (_driver.TryFindElement(By.XPath(XPath.NewQuote.ForYourselfDialog.ForYourselfErrorBox), out errorBox))
                    {
                        success = false;
                    }
                }
                else
                {
                    if (_driver.TryFindElement(By.XPath(XPath.NewQuote.ForYourselfDialog.ForYourselfErrorBox), out errorBox) == false)
                    {
                        success = false;
                    }
                }

                // Check 'main driver' buttons
                if (GetClass($"{XPath.NewQuote.ForYourselfDialog.DriverYesLabel}/span[contains(@class,'rb-radio')]").Contains("checked") ||
                    GetClass($"{XPath.NewQuote.ForYourselfDialog.DriverNoLabel}/span[contains(@class,'rb-radio')]").Contains("checked"))
                {
                    if (_driver.TryFindElement(By.XPath(XPath.NewQuote.ForYourselfDialog.MainDriverfErrorBox), out errorBox))
                    {
                        success = false;
                    }
                }
                else
                {
                    if (_driver.TryFindElement(By.XPath(XPath.NewQuote.ForYourselfDialog.MainDriverfErrorBox), out errorBox) == false)
                    {
                        success = false;
                    }
                }
            }
            catch 
            {
                success = false;
            }
            return success;
        }

        /// <summary>
        /// Performs validation on the Pre-Populated Quote pop-up dialog that prompts the
        /// member to verify that the details are correct before proceeding with the quote.
        /// </summary>
        /// <param name="member">Valid test data of a member with current RAC membership if applicable.</param>
        /// <param name="testConfig">Configuration information to inform feature toggle state.</param>
        /// <returns></returns>
        public bool VerifyPrefillDetails(Contact member)
        {
            var errorMessage = "";
            _driver.WaitForElement(By.XPath(XPath.NewQuote.QuoteMemberDetailsDialog.SectionBase), WaitTimes.T30SEC);

            var match = true;
            try
            {
                 
                match &= member.EqualsFullName(GetInnerText(XPath.NewQuote.QuoteMemberDetailsDialog.PrefillNameAnswer));

                errorMessage = "Gender";
                var expectedGender = member.Gender.GetDescription();
                match &= CheckPrefillStrings(expectedGender, XPath.NewQuote.QuoteMemberDetailsDialog.PrefillGenderAnswer, errorMessage);

                errorMessage = "Date of Birth";
                match &= CheckPrefillStrings(member.DateOfBirth.ToString("d MMM yyyy"), XPath.NewQuote.QuoteMemberDetailsDialog.PrefillDobAnswer, errorMessage);

                errorMessage = "Mailing Address";
                var expectedAddress = member.MailingAddress.QASStreetAddress();
                if (!CheckPrefillStrings(expectedAddress.Trim(), XPath.NewQuote.QuoteMemberDetailsDialog.PrefillAddressAnswer, errorMessage, stripAddressDelimiters: true))
                    // Modifying street type, as non-QAS verified addresses can present unabbreviated street types.
                    expectedAddress = member.MailingAddress.QASStreetAddress(true);
                match &= CheckPrefillStrings(expectedAddress.Trim(), XPath.NewQuote.QuoteMemberDetailsDialog.PrefillAddressAnswer, errorMessage, stripAddressDelimiters: true);

                errorMessage = "Membership Info";
                if (member.IsRACMember)
                    {
                        var expectedMembership = $"{member.MembershipNumber} - {member.MembershipTier}";
                        match &= CheckPrefillStrings(expectedMembership, XPath.NewQuote.QuoteMemberDetailsDialog.PrefillMemberAnswer, errorMessage);
                    }
                else
                    {
                        // We should not see the membership field.
                        IWebElement field = null;
                        match &= !_driver.TryFindElement(By.XPath(XPath.NewQuote.QuoteMemberDetailsDialog.PrefillMemberAnswer), out field);
                    }

                errorMessage = "Email";
                match &= CheckPrefillStrings(member.GetEmail(), XPath.NewQuote.QuoteMemberDetailsDialog.PrefillEmailAnswer, errorMessage);

                errorMessage = "Phone Number";
                match &= CheckPrefillStrings(member.GetPhone(), XPath.NewQuote.QuoteMemberDetailsDialog.PrefillPhoneAnswer, errorMessage);
            }
            catch
            {
                Reporting.Log($"Error occurred in trying to access the UI field for {errorMessage} on the PPQ Prefill details confirmation pop-up.");
                match = false;
            }
            return match;
        }

        public void ClickPrefillDetailsCorrect()
        {
            _driver.WaitForElement(By.XPath(XPath.NewQuote.QuoteMemberDetailsDialog.SectionBase), WaitTimes.T30SEC);
            ClickControl(XPath.NewQuote.QuoteMemberDetailsDialog.PrefillYesButton, skipJSScrollLogic: true);
        }

        public void ClickPrefillDetailsNotCorrect()
        {
            _driver.WaitForElement(By.XPath(XPath.NewQuote.QuoteMemberDetailsDialog.SectionBase), WaitTimes.T30SEC);
            ClickControl(XPath.NewQuote.QuoteMemberDetailsDialog.PrefillNoButton);
        }

        /// <summary>
        /// Supporting PPQ checks of member details fetched from MC which
        /// will be used for pre-fill. This is for use against the pop-up
        /// which prompts the Member to confirm if details are correct.
        /// </summary>
        /// <param name="expectedString">string test is expecting for member</param>
        /// <param name="xpath">XPath of field we'll be checking against</param>
        /// <param name="fieldDescription">a description of that field to use in logging messages</param>
        /// <param name="stripAddressDelimiters">If evaluating an address string, set this parameter to remove address delimiters from the comparison</param>
        /// <returns></returns>
        private bool CheckPrefillStrings(string expectedString, string xpath, string fieldDescription, bool stripAddressDelimiters = false)
        {
            var result = true;
            var foundText = stripAddressDelimiters ? GetElement(xpath).Text.StripAddressDelimiters() : GetElement(xpath).Text;
            if (!expectedString.Equals(foundText, StringComparison.InvariantCultureIgnoreCase))
            {
                Reporting.Log($"Mismatch between expected {fieldDescription}({expectedString}) and presented prefill {fieldDescription} ({foundText})");
                result = false;
            }
            return result;
        }

        private int GetCarouselPolicyDisplayLimit() => int.Parse(_driver.WaitForElement(By.XPath(XPath.Carousel.Base), WaitTimes.T5SEC)
                                                                        .GetAttribute("data-carousel-items-per-page"));

        private List<PCMItemTabState> GetPolicyCarouselTabs()
        {
            var tabItems = new List<PCMItemTabState>();
            var tabs = _driver.FindElements(By.XPath(XPath.Carousel.Items));

            var desiredTabAttribute = string.Empty;
            switch (CurrentPCMTab)
            {
                case PCM_TAB.POLICIES:
                    desiredTabAttribute = "data-policy-number";
                    break;
                case PCM_TAB.CLAIMS:
                    desiredTabAttribute = "data-claim-number";
                    break;
                default:
                    Reporting.Error("Invalid context for querying the carousel. Only supported for Policies and Claims.");
                    break;
            }

            if (tabs != null)
            {
                foreach (var carouselTab in tabs)
                {
                    var tabState = new PCMItemTabState()
                    {
                        ReferenceNumber = carouselTab.GetAttribute(desiredTabAttribute),
                        IsSelected = carouselTab.GetAttribute("class").Contains("is-selected"),
                        IsVisible = carouselTab.Displayed
                    };
                    tabItems.Add(tabState);
                }
            }

            return tabItems;
        }

        /// <summary>
        /// Returns the reference number from the carousel for the currently selected
        /// carousel tab. Usable for both Policy view as well as Claim view.
        /// </summary>
        /// <returns></returns>
        private string CurrentlySelectedCarouselItem()
        {
            var desiredTabAttribute = string.Empty;
            switch (CurrentPCMTab)
            {
                case PCM_TAB.POLICIES:
                    desiredTabAttribute = "data-policy-number";
                    break;
                case PCM_TAB.CLAIMS:
                    desiredTabAttribute = "data-claim-number";
                    break;
                default:
                    Reporting.Error("Invalid context for querying the carousel. Only supported for Policies and Claims.");
                    break;
            }

            var selectedCarouselItemXPath = $"{XPath.Carousel.Items}[contains(@class,'is-selected')]";
            return GetElement(selectedCarouselItemXPath).GetAttribute(desiredTabAttribute);
        }


        /// <summary>
        /// Clicks the "Change my banking details" button to begin the
        /// related change Bank details on the current policy.
        /// </summary>
        public void ClickChangeMyBankingDetails() => ClickControl(XPath.MyPolicies.PolicyOperations.Button.ChangeBankDetails,
                                                                  waitTimeSeconds: WaitTimes.T30SEC);

        public void ClickGetCertificateOfCurrency() => ClickControl(XPath.MyPolicies.PolicyOperations.Button.GetCerticateOfCurrency);

        /// <summary>
        /// Clicks the "Cancel my policy" button to begin the
        /// cancel policy flow.
        /// </summary>
        public void ClickCancelMyPolicy() => ClickControl(XPath.MyPolicies.PolicyOperations.Button.CancelPolicy);

        private string WaitForClaimProgressDetailsToRender(int waitTime = WaitTimes.T60SEC)
        {
            var endTime = DateTime.Now.AddSeconds(waitTime);
            var claimNumber = string.Empty;
            Reporting.Log($"Begin waiting for claim content to render");
            do
            {
                try
                {
                    Thread.Sleep(500);
                    var thing = _driver.WaitForElementToBeVisible(By.XPath(XPath.MyPolicies.PolicyOperations.ButtonContainer), WaitTimes.T30SEC);
                    if (!GetElement(XPath.MyPolicies.PolicyOperations.ButtonContainer).Displayed) { continue; }
                    if (!GetElement(XPath.MyClaims.ProgresBar.LodgeHelpIcon).Displayed) { continue; }
                    if (!GetElement(XPath.MyClaims.ProgresBar.AssessHelpIcon).Displayed) { continue; }
                    if (!GetElement(XPath.MyClaims.ProgresBar.RepairHelpIcon).Displayed) { continue; }

                    if (ClaimCurrentReferencedHelpIcon != CLAIM_PROGRESS_STATE.Complete &&
                        string.IsNullOrEmpty(GetInnerText(XPath.MyClaims.ProgresBar.TextBody)))
                    { continue; }

                    claimNumber = GetInnerText(XPath.MyClaims.ClaimsDetails.ClaimNumber);
                    break;
                }
                catch (NoSuchElementException ex) { /* Do nothing, this can be normal. */ }
            } while (DateTime.Now < endTime);

            if (string.IsNullOrEmpty(claimNumber))
            { Reporting.Error($"Timed out after {waitTime}s while waiting for claim details to render in PCM. Check screenshot to see if page fully loaded."); }

            Reporting.Log($"Claim content for {claimNumber} is not visible.");
            return claimNumber;
        }

        private void WaitForClaimStatusHelpTextToLoad(int waitTime = WaitTimes.T30SEC)
        {
            var endTime = DateTime.Now.AddSeconds(waitTime);

            Reporting.Log($"Begin waiting for claim help text dialog to fetch text");
            do
            {
                try
                {
                    Thread.Sleep(500);

                    if (string.IsNullOrEmpty(GetInnerText(XPath.MyClaims.ProgresBar.TextBody)))
                    { continue; }
                    break;
                }
                catch (NoSuchElementException ex) { Reporting.Log($"Unable to find claim help text element: {ex}"); }
            } while (DateTime.Now < endTime);
                if (DateTime.Now > endTime)
            { Reporting.Error($"Timed out after {waitTime}s while waiting for claim help text dialog to populate."); }

            Reporting.Log($"Claim help text dialog populated.");
        }

        #region Home details Settable properties and controls
        /// <summary>
        /// Returns the Address under Home details
        /// </summary>
        /// <returns></returns>
        public string HomeAddress
        {
            get { return GetInnerText(XPath.MyPolicies.HomeDetails.HomeAddressAnswer); }
        }

        /// <summary>
        /// Returns the Building type under Home details
        /// </summary>
        /// <returns></returns>
        public HomeType BuildingType
        {
            get { return DataHelper.ConvertBuildingTypeStringToEnum(GetInnerText(XPath.MyPolicies.HomeDetails.BuildingTypeAnswer)); }
        }

        /// <summary>
        /// Returns the Alarm system under Home details
        /// </summary>
        /// <returns></returns>
        public Alarm AlarmSystem
        {
            get { return DataHelper.GetValueFromDescription<Alarm>(GetInnerText(XPath.MyPolicies.HomeDetails.AlarmSystemAnswer)); }
        }

        /// <summary>
        /// Returns the Financier under Home details
        /// </summary>
        /// <returns></returns>
        public string Financier
        {
            get { return GetInnerText(XPath.MyPolicies.HomeDetails.FinancierAnswer); }
        }
        #endregion

        #region Motor details Settable properties and controls
        public string Registration => GetInnerText(XPath.MyPolicies.MotorDetails.RegistrationAnswer);        
        public string CarUsage => GetInnerText(XPath.MyPolicies.MotorDetails.UsageAnswer);
        public string AnnualKms => GetInnerText(XPath.MyPolicies.MotorDetails.KmPerYearAnswer);

        public string CarDescription => _driver.FindElement(By.XPath(XPath.MyPolicies.MotorDetails.MotorDescriptionAnswer)).Text;
        #endregion

        #region Caravan details Settable properties and controls
        public string CaravanModel => _driver.FindElement(By.XPath(XPath.MyPolicies.MotorDetails.CaravanModelAnswer)).Text;
        public string CaravanMake => _driver.FindElement(By.XPath(XPath.MyPolicies.MotorDetails.CaravanMakeAnswer)).Text;
        #endregion
    }
}