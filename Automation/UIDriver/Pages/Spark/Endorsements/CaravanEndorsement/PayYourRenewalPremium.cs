using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace UIDriver.Pages.Spark.Endorsements
{
    /// <summary>
    /// The Pay Your Renewal page gives the member the chance to review how they pay for their policy.
    /// Options are presented to change the freqency and method of payment.
    /// </summary>
    public class PayYourRenewalPremium : SparkPaymentPage
    {
        #region CONSTANTS
        public static class Constants
        {
            public static readonly string PageTitle = "Pay your renewal";
            public static class PolicyCard
            {
                public static readonly string Title = "Caravan and trailer insurance";
            }
            public static class InstalmentSummary
            {
                public static readonly string HowOftenLabel = "How often";

                public static readonly string AnnualFrequency = "Annually";
                public static readonly string MonthlyFrequency = "Monthly";

                public static readonly string AmountLabel = "Amount";

                public static readonly string PolicyRenewsLabel = "Policy renews on";
            }

            public static string RenewalDateInfor(string renewalStartDate) => $"Any changes you make will apply from {renewalStartDate}. If you want your changes to apply sooner, please call us on 13 17 03.";

            public static class AnnualDirectDebitOption
            {
                public static readonly string Title = "Would you like to set up annual direct debit?";
                public static readonly string Text = "This means we'll debit your account in future years. " +
                    "We'll send your renewal documents before payment is due so you can make changes.";
            }

            public static class PreferredMonthlyPaymentDate
            {
                public static readonly string Title = "Preferred monthly payment date";
                public static readonly string Text = "Choose your payment date for the next 12 months";
                public static readonly string EveryMonth = "of every month";
                public static string DayOfEveryMonth(int day) => $"{DataHelper.AddOrdinal(day)} of every month";
            }

            public static class Payment
            {
                public static readonly string Title = "Choose how to pay";

                public static class DirectDebitPayments
                {

                    public static readonly string MonthlyDirectDebitText = "We'll direct debit all future payments from this account.";
                    public static readonly string AnnualDirectDebitText = "We'll direct debit this account annually when payment is due.";
                    public static readonly string TextExpiredCard = "If your credit card is expired, you'll need to add a new credit card.";
                }

                public static class OneOffPayments
                {
                    public static readonly string TextExplanation = "Pay by card now or pay later using your card, BPAY® or another method.";
                    public static readonly string Subtitle = "Select 'Confirm' to renew";
                    public static readonly string CardPaymentMethod = "This will be a one-off payment";
                    public static readonly string BpayPaymentMethod = "You can BPAY using your online banking";
                    public static readonly string BpayBlurbPart1a = "Your payment is due on ";  // [dd/MM/yyyy] date follows this blurb 
                    public static readonly string BpayBlurbPart1b = ". We'll email your renewal documents.";
                    public static readonly string BpayBlurbPart2 = "The biller code and reference number will be on the next screen.";
                    public static readonly string PayLaterMethod = "You can pay for your insurance later";
                    public static readonly string PayLaterBlurbPart1a = "Your payment is due on "; // [dd/MM/yyyy] date follows this blurb
                    public static readonly string PayLaterBlurbPart1b = ". We'll email your renewal documents and details on how to pay.";
                }
            }

            public static class Email
            {
                public static readonly string Title = "Confirm your email address";
                public static readonly string Text = "We'll update your contact details (not your login email) if you provide a new email.";
                public static readonly string Validation = "Please enter a valid email";
            }


            public static class YourPolicySummary
            {
                public static readonly string Title                         = "Your policy summary";
                public static readonly string TitleFooter                   = "Please ensure all details are correct and that the policy suits your needs.";

                public static readonly string TitleYourCaravanOrTrailer     = "Your caravan or trailer";

                public static readonly string TitleCaravanOrTrailerDetails  = "Storage and use";
                public static readonly string Rego                          = "Caravan or trailer registration: ";
                public static readonly string IsBusinessUse                 = "Your caravan or trailer is used for business purposes or earning income: No";
                public static readonly string Suburb                        = "Suburb where your caravan or trailer is usually kept: ";
                public static readonly string KeptPlace                     = "Your caravan or trailer is kept in one place and isn’t towed: ";
                public static readonly string Parked                        = "Place your caravan or trailer is usually parked: ";

                public static readonly string TitleYourPremium              = "Your premium";
                public static readonly string BasicExcess                   = "Basic excess: ";
                public static readonly string AgreedValue                   = "Agreed value: ";
                public static readonly string ContentCover                  = "Contents cover: ";
            }

        }
        #endregion

        #region XPATHS
        public static class XPath
        {
            public static readonly string PageTitle = "//h2[text()=" + "\"" + Constants.PageTitle + "\"" + "]";
            public static class PolicyCard
            {
                public static readonly string PolicyCardTitle = "id('policy-card-content-policy-details-header-title-policy-details')";
                public static readonly string PolicyCover = "id('policy-card-content-policy-details-header-subtitle-policy-details')";
                public static readonly string CarModel = "id('policy-card-content-policy-details-property-0-model-policy-details')";
                public static readonly string CarRegistration = "//p[contains(@id,'policy-card-content-policy-details') and contains(@id,'registration-policy-details')]";
                public static readonly string PolicyNumber = "//p[contains(@id,'policy-number-policy-details') and contains(@id,'policy-card-content-policy-details-property')]";
            }

            public static class InstalmentSummary
            {
                public static readonly string FrequencyLabel = "id('frequency-label')";
                public static readonly string FrequencyValue = "id('frequency-value')";
                public static readonly string AmountLabel = "id('amount-label')";
                public static readonly string AmountValue = "id('amount-value')";
                public static readonly string AmountExtraInfo = "id('amount-subtitle')";
                public static readonly string RenewDateLabel = "id('start-date-label')";
                public static readonly string RenewDateValue = "id('start-date-value')";
                public static readonly string RenewDateInfo = "id('start-date-subtitle')";
            }

            public static class PreferredDate
            {
                public static readonly string Title = "//label[@for='payment-day-text-input']";
                public static readonly string Explanation = Title + "/following-sibling::p";
                public static readonly string Input = "//input[@id='payment-day-text-input']";
            }

            public static class FailedPayment
            {
                public static readonly string TryAgainButton = "//button[text()='Try again']";
            }

            public static class AnnualDirectDebit
            {
                public static readonly string Information = "//label[@for='direct-debit-opt-in']";
                public static readonly string Blurb = Information + "//p";
                public static readonly string Selection = "//div[@id='direct-debit-opt-in']";

                public static class Button
                {
                    public static readonly string Yes = "//button[@id='direct-debit-opt-in-true']";
                    public static readonly string No = "//button[@id='direct-debit-opt-in-false']";
                }
            }

            public static class DirectDebitPayment
            {
                public static class Preamble
                {
                    public static readonly string Container = "//div[@id='select-payment-method-select-account-preamble']";
                    public static string SectionTitle => $"//b[text()='{Constants.Payment.Title}']";
                    public static readonly string ParagraphOne = Container + "/p[1]";
                    public static readonly string ParagraphTwo = Container + "/p[2]";
                }

                // Options presented to the member are to use an existing credit card or bank acoount
                // or add a new one
                public static readonly string RadioGroup = "id('select-payment-method-add-payment-method')";
                public static readonly string AddPaymentMethod = "//div[@id='select-payment-method-add-payment-method-radio-container']/label";

                public static readonly string NewCardOption = "//button[@id='add-payment-method-input-payment-method-Card']";
                public static readonly string NewBankAccountOption = "//button[@id='add-payment-method-input-payment-method-Bank account']";
            }

            /// <summary>
            /// Information and options for one off payments
            /// </summary>
            public static class OneOffPaymentPayment
            {
                public static readonly string Information = "//div[@id='stacked-card-container-content']";
                public static readonly string Blurb = Information + "//h2";
                public static readonly string OptionSelected = "//div[@id='cmp-add-payment-method-input-pay-later-option']/button[@aria-pressed,'true')]";
                public static readonly string PaymentMethod = "id('payment-method-message')";

                public static class Button
                {
                    public static readonly string Card = "//button[@id='cmp-add-payment-method-input-pay-later-option-toggle-card']";
                    public static readonly string BPay = "//button[@id='cmp-add-payment-method-input-pay-later-option-toggle-bpay']";
                    public static readonly string PayLater = "//button[@id='cmp-add-payment-method-input-pay-later-option-toggle-paylater']";
                }
                // For Pay Later and BPay
                public static class BPayAndPayLaterPayment
                {
                    public static readonly string Icon = "id('notification-card-icon')";
                    public static readonly string DeferredPaymentTitle = "id('notification-card-title')";
                    public static readonly string ParagraphOne = "id('annual-payment-option-notification-part1')";
                    public static readonly string ParagraphTwo = "id('annual-payment-option-notification-part2')";
                }
            }
            public static class Email
            {
                public static readonly string Label = "//label[@for='email']";
                public static readonly string InputField = "id('email')";
            }

            public static class YourPolicySummary
            {
                public static readonly string ShowYourPolicyInfo        = "id('info-summary-expand')";
                public static readonly string Title                     = "id('info-summary-title')";
                public static readonly string TitleFooter               = "id('info-summary-subtitle')";

                public static readonly string TitleYourCaravanOrTrailer = "id('info-summary-your-caravan-section-title')";
                public static readonly string Model                     = "id('info-summary-your-caravan-section-your-caravan-subsection-0')";

                public static readonly string TitleCaravanOrTrailerDetails  = "id('info-summary-caravan-details-title')";
                public static readonly string Rego                          = "id('info-summary-caravan-details-caravan-details-subsection-0')";
                public static readonly string IsBusinessUse                 = "id('info-summary-caravan-details-caravan-details-subsection-1')";
                public static readonly string Suburb                        = "id('info-summary-caravan-details-caravan-details-subsection-2')";
                public static readonly string KeptPlace                     = "id('info-summary-caravan-details-caravan-details-subsection-3')";
                public static readonly string Parked                        = "id('info-summary-caravan-details-caravan-details-subsection-4')";

                public static readonly string TitleYourPremium              = "id('info-summary-your-premium-section-title')";
                public static readonly string BasicExcess                   = "id('info-summary-your-premium-section-premium-subsection-0')";
                public static readonly string AgreedValue                   = "id('info-summary-your-premium-section-premium-subsection-1')";
                public static readonly string ContentCover                  = "id('info-summary-your-premium-section-premium-subsection-2')";
            }

            public static class Confirm
            {
                public static readonly string Button = "id('confirm')";
            }
        }
        #endregion

        #region Settable properties and controls
        private string PageTitleValue => GetInnerText(XPath.PageTitle);
        private string PolicyCardTitle => GetInnerText(XPath.PolicyCard.PolicyCardTitle);
        private string PolicyCardCover => GetInnerText(XPath.PolicyCard.PolicyCover);
        private string PolicyCardModel => GetInnerText(XPath.PolicyCard.CarModel);
        private string PolicyCardRego => GetInnerText(XPath.PolicyCard.CarRegistration);
        private string PolicyNumber => GetInnerText(XPath.PolicyCard.PolicyNumber);
        private string FrequencyLabel => GetInnerText(XPath.InstalmentSummary.FrequencyLabel);
        private string FrequencyValue => GetInnerText(XPath.InstalmentSummary.FrequencyValue);
        private string AmountLabel => GetInnerText(XPath.InstalmentSummary.AmountLabel);
        private string AmountValue => GetInnerText(XPath.InstalmentSummary.AmountValue);
        private string RenewalDateLabel => GetInnerText(XPath.InstalmentSummary.RenewDateLabel);
        private string RenewalDateValue => GetInnerText(XPath.InstalmentSummary.RenewDateValue);
        private string RenewalDateInfo => GetInnerText(XPath.InstalmentSummary.RenewDateInfo);
        private string PreferredDateTitle => GetInnerText(XPath.PreferredDate.Title);
        private string PreferredDateExplanation => GetInnerText(XPath.PreferredDate.Explanation);
        private string PreferredDateInput => GetValue(XPath.PreferredDate.Input);
        private string AnnualDirectDebitSelectionLabel => GetInnerText(XPath.AnnualDirectDebit.Information);
        private string AnnualDirectDebitSelectionBlurb => GetInnerText(XPath.AnnualDirectDebit.Blurb);

        //Policy Summary
        private string Title => GetInnerText(XPath.YourPolicySummary.Title);
        private string TitleFooter => GetInnerText(XPath.YourPolicySummary.TitleFooter);
        private string TitleYourCaravanOrTrailer => GetInnerText(XPath.YourPolicySummary.TitleYourCaravanOrTrailer);
        private string Model => GetInnerText(XPath.YourPolicySummary.Model);
        private string TitleCaravanOrTrailerDetails => GetInnerText(XPath.YourPolicySummary.TitleCaravanOrTrailerDetails);
        private string Rego => GetInnerText(XPath.YourPolicySummary.Rego);
        private string IsBusinessUse => GetInnerText(XPath.YourPolicySummary.IsBusinessUse);
        private string Suburb => GetInnerText(XPath.YourPolicySummary.Suburb);
        private string KeptPlace => GetInnerText(XPath.YourPolicySummary.KeptPlace);
        private string Parked => GetInnerText(XPath.YourPolicySummary.Parked);
        private string TitleYourPremium => GetInnerText(XPath.YourPolicySummary.TitleYourPremium);
        private string BasicExcess => GetInnerText(XPath.YourPolicySummary.BasicExcess);
        private string AgreedValue => GetInnerText(XPath.YourPolicySummary.AgreedValue);
        private string ContentCover => GetInnerText(XPath.YourPolicySummary.ContentCover);

        private bool AnnualDirectDebitSelection
        {
            get => GetBinaryToggleState(XPath.AnnualDirectDebit.Selection, XPath.AnnualDirectDebit.Button.Yes, XPath.AnnualDirectDebit.Button.No);
            set => ClickBinaryToggle(XPath.AnnualDirectDebit.Selection, XPath.AnnualDirectDebit.Button.Yes, XPath.AnnualDirectDebit.Button.No, value);
        }

        private string DeferredPaymentTitle => GetInnerText(XPath.OneOffPaymentPayment.BPayAndPayLaterPayment.DeferredPaymentTitle);
        private string DeferredPaymentParagraphOne => GetInnerText(XPath.OneOffPaymentPayment.BPayAndPayLaterPayment.ParagraphOne);
        private string DeferredPaymentParagraphTwo => GetInnerText(XPath.OneOffPaymentPayment.BPayAndPayLaterPayment.ParagraphTwo);
        private bool IsWarningIconDisplayed => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.OneOffPaymentPayment.BPayAndPayLaterPayment.Icon), WaitTimes.T5SEC, out IWebElement _);

        private PaymentOptionsSpark OneOffPayment
        {
            get => DataHelper.GetValueFromDescription<PaymentOptionsSpark>(GetInnerText(XPath.OneOffPaymentPayment.OptionSelected));
            set
            {
                switch (value)
                {
                    case PaymentOptionsSpark.AnnualCash:  // Pay now with Credit Card
                        ClickControl(XPath.OneOffPaymentPayment.Button.Card);
                        break;
                    case PaymentOptionsSpark.BPay:
                        ClickControl(XPath.OneOffPaymentPayment.Button.BPay);
                        break;
                    case PaymentOptionsSpark.PayLater:
                        ClickControl(XPath.OneOffPaymentPayment.Button.PayLater);
                        break;
                    default:
                        Reporting.Error("Select a valid value for choose how to pay");
                        break;
                }
            }
        }

        private string ChooseHowToPayOneOffTitle => GetInnerText(XPath.OneOffPaymentPayment.Information);
        private string ChooseHowToPayOneOffBlurb => GetInnerText(XPath.OneOffPaymentPayment.Blurb);

        private string ChooseHowToPayAutomaticTitle => GetInnerText(XPath.DirectDebitPayment.Preamble.SectionTitle);
        private string ChooseHowToPayAutomaticBlurb => GetInnerText(XPath.DirectDebitPayment.Preamble.ParagraphOne);
        private string ChooseHowToPayAutomaticExpiredCards => GetInnerText(XPath.DirectDebitPayment.Preamble.ParagraphTwo);

        private string EmailAddress
        {
            get => GetValue(XPath.Email.InputField);
            set => WaitForTextFieldAndEnterText(XPath.Email.InputField, value, false);
        }

        #endregion

        public PayYourRenewalPremium(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PageTitle);
                GetElement(XPath.InstalmentSummary.FrequencyLabel);
                GetElement(XPath.InstalmentSummary.AmountLabel);
                GetElement(XPath.InstalmentSummary.RenewDateLabel);
                GetElement(XPath.Confirm.Button);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Pay your renewal");
            Reporting.Log("Pay your renewal", _browser.Driver.TakeSnapshot());

            return true;
        }

        public void VerifyPolicyCard(EndorseCaravan endorseCaravan)
        {
            Caravan insuredCaravan = new Caravan();
            insuredCaravan = endorseCaravan.ChangeMakeAndModel ? endorseCaravan.NewInsuredAsset : endorseCaravan.InsuredAsset;

            Reporting.AreEqual(Constants.PolicyCard.Title, PolicyCardTitle, "Policy card title");
            Reporting.Log($"Expecting the following to be contained in the Model field: '{insuredCaravan.Model.Trim()}'");
            Reporting.IsTrue(PolicyCardModel.Contains($"{insuredCaravan.Model.Trim()}"), "Policy card caravan make and model");
            if (DataHelper.IsRegistrationNumberConsideredValid(insuredCaravan.Registration))
            {
                Reporting.AreEqual($"Registration: {insuredCaravan.Registration}", PolicyCardRego, "caravan/trailer registration is displayed");
            }         
            Reporting.AreEqual($"Policy number: {endorseCaravan.OriginalPolicyData.PolicyNumber}", PolicyNumber, "Policy card policy number");
        }

        public void VerifyYourPolicySummary(EndorseCaravan endorseCaravan)
        {
            Caravan insuredCaravan = new Caravan();
            insuredCaravan = endorseCaravan.ChangeMakeAndModel ? endorseCaravan.NewInsuredAsset : endorseCaravan.InsuredAsset;

            ClickControl(XPath.YourPolicySummary.ShowYourPolicyInfo);
            Reporting.Log("Your policy summary expanded", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(Constants.YourPolicySummary.Title, Title, "'Your policy summary' title is displayed");
            Reporting.AreEqual(Constants.YourPolicySummary.TitleFooter, TitleFooter, "Your policy summary' sub title is displayed");
            Reporting.AreEqual(Constants.YourPolicySummary.TitleYourCaravanOrTrailer, TitleYourCaravanOrTrailer, $"caravan/trailer details title matches expected value");
            Reporting.AreEqual($"{insuredCaravan.Year} {insuredCaravan.Make}", Model, "caravan model displayed");

            Reporting.AreEqual(Constants.YourPolicySummary.TitleCaravanOrTrailerDetails, TitleCaravanOrTrailerDetails, $"section title '{Constants.YourPolicySummary.TitleCaravanOrTrailerDetails}' displayed");
            if (DataHelper.IsRegistrationNumberConsideredValid(insuredCaravan.Registration))
            {
                Reporting.AreEqual($"{Constants.YourPolicySummary.Rego}{insuredCaravan.Registration}", Rego, "caravan/trailer registration is displayed");
            }
            else
            {
                Reporting.AreEqual($"{Constants.YourPolicySummary.Rego}TBA", Rego, "caravan/trailer registration is displayed as 'TBA' for no rego");
            }
            Reporting.AreEqual(Constants.YourPolicySummary.IsBusinessUse, IsBusinessUse, $"'{Constants.YourPolicySummary.IsBusinessUse}' is displayed");
            Reporting.IsTrue(Suburb.ToLower().StartsWith($"{Constants.YourPolicySummary.Suburb}{endorseCaravan.OriginalPolicyData.CaravanAsset.Suburb}".ToLower()), "Suburb were the caravan is kept is displayed");
            var isKeptInOnePlace = endorseCaravan.OriginalPolicyData.Covers.First().CoverTypeDescription.Contains("On-Site") ? "Yes" : "No";
            if (endorseCaravan.ChangeMakeAndModel)
            {
                isKeptInOnePlace = string.IsNullOrEmpty(endorseCaravan.Parked.GetDescription()) ? "Yes" : "No";
            }
            Reporting.AreEqual($"{Constants.YourPolicySummary.KeptPlace}{isKeptInOnePlace}", KeptPlace, $"'{Constants.YourPolicySummary.KeptPlace}' label is displayed");

            Reporting.AreEqual(Constants.YourPolicySummary.TitleYourPremium, TitleYourPremium, $"title {Constants.YourPolicySummary.TitleYourPremium} is displayed");

            var excess = endorseCaravan.OriginalPolicyData.Covers.First().StandardExcess.ToString().Replace(".0", "").Trim();
            Reporting.AreEqual($"{Constants.YourPolicySummary.BasicExcess}{DataHelper.ConvertIntToMonetaryString(Int32.Parse(excess))}", BasicExcess, "Basic Excess is displayed");

            var agreedValueExpected = endorseCaravan.ChangeMakeAndModel ? endorseCaravan.NewInsuredAsset.MarketValue : endorseCaravan.OriginalPolicyData.Covers.First().SumInsured;
            var cleanedAgreedValue = agreedValueExpected.ToString().Replace(".0", "").Trim();
            Reporting.AreEqual($"{Constants.YourPolicySummary.AgreedValue}{DataHelper.ConvertIntToMonetaryString(Int32.Parse(cleanedAgreedValue),10000)}", AgreedValue, "Agreed Value is displayed");
            
            if (endorseCaravan.OriginalPolicyData.Covers.First().CoverTypeDescription.Equals("Trailer"))
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.YourPolicySummary.ContentCover), "Content cover option is not displayed for Trailer");
            }
            else if (endorseCaravan.OriginalPolicyData.Covers[1].CoverTypeDescription.Equals("Caravan Contents"))
            {
                var contentCoverExpected = (int)Math.Floor(endorseCaravan.OriginalPolicyData.Covers[1].SumInsured);
                Reporting.AreEqual($"{Constants.YourPolicySummary.ContentCover}{DataHelper.ConvertIntToMonetaryString(contentCoverExpected, minValueForThousandsSeparator:10000, applyThousandsSeparator: true)}",
                                   ContentCover, "Content Cover is displayed");
            }
        }

        public void VerifyPremiumDetails(EndorseCaravan endorseCaravan)
        {
            var paymentFrequency = endorseCaravan.SparkExpandedPayment.IsAnnual ? Constants.InstalmentSummary.AnnualFrequency : Constants.InstalmentSummary.MonthlyFrequency;
            Reporting.AreEqual(paymentFrequency, FrequencyValue, "How often policy paid");
            VerifyAmountWithTruncationWhenWholeDollar(endorseCaravan.PremiumChangesAfterEndorsement.Total, AmountValue, "new premium is consistent with proposed");
            Reporting.AreEqual(endorseCaravan.OriginalPolicyData.EndorsementStartDate.ToString(DateTimeTextFormat.ddMMyyyy), RenewalDateValue, "renewal date shown correctly");
            Reporting.AreEqual(Constants.RenewalDateInfor(RenewalDateValue), RenewalDateInfo, "Policy Renewal date extra info");
            if (!endorseCaravan.SparkExpandedPayment.IsAnnual)
            {
                Reporting.AreEqual(Constants.PreferredMonthlyPaymentDate.DayOfEveryMonth(endorseCaravan.OriginalPolicyData.NextPayableInstallment.CollectionDate.Day), PreferredDateInput, "preferred collection day of every month");
            }
        }

        private void VerifyChooseHowToPayTitle()
        {
            Reporting.AreEqual(Constants.Payment.Title, ChooseHowToPayAutomaticTitle, "Choose how to pay title");
        }

        /// <summary>
        /// Handles entering the payment choices for the test case, including
        /// entering payment information.
        /// </summary>    
        /// <param name="detailUiChecking">If set to true will investigate BSB validations</param>
        public void CompletePaymentInputs(EndorseCaravan endorseCaravan, bool detailUiChecking)
        {
            var userPayment = endorseCaravan.SparkExpandedPayment;

            if (userPayment.IsAnnual)
            {
                Reporting.IsTrue(AnnualDirectDebitSelectionLabel.StartsWith(Constants.AnnualDirectDebitOption.Title), "Annual direct debit title");
                Reporting.AreEqual(Constants.AnnualDirectDebitOption.Text, AnnualDirectDebitSelectionBlurb, "Annual direct debit information text");

                AnnualDirectDebitSelection = userPayment.PaymentOption == PaymentOptionsSpark.DirectDebit;
                Thread.Sleep(SleepTimes.T5SEC);
            }

            ScrollElementIntoView(XPath.DirectDebitPayment.Preamble.SectionTitle);
            Reporting.Log($"Capturing screenshot for payments", _browser.Driver.TakeSnapshot());

            VerifyChooseHowToPayTitle();
            if (userPayment.IsAnnual && !AnnualDirectDebitSelection)
            {
                OneOffPayment = userPayment.PaymentOption;
            }

            switch (userPayment.PaymentOption)
            {
                case PaymentOptionsSpark.DirectDebit:
                    if (userPayment.IsAnnual)
                    {
                        Reporting.AreEqual(Constants.Payment.DirectDebitPayments.AnnualDirectDebitText, ChooseHowToPayAutomaticBlurb, "Choose how to pay information text for annual direct debit payment");
                    }
                    else
                    {
                        Reporting.AreEqual(Constants.Payment.DirectDebitPayments.MonthlyDirectDebitText, ChooseHowToPayAutomaticBlurb, "Choose how to pay information text for monthly payment");

                    }
                    Reporting.AreEqual(Constants.Payment.DirectDebitPayments.TextExpiredCard, ChooseHowToPayAutomaticExpiredCards, "Choose how to pay expired card information");
                    if (endorseCaravan.SparkExpandedPayment.BankAccountDetails != null && detailUiChecking)
                    {
                        ClickControl(XPath.DirectDebitPayment.AddPaymentMethod);
                        ClickControl(XPath.DirectDebitPayment.NewBankAccountOption);
                        EnterInvalidNoMatchBSBAndCheckErrorMessage(null);
                    }
                    AddNewPaymentMethod(endorseCaravan.SparkExpandedPayment);
                    break;
                case PaymentOptionsSpark.AnnualCash:
                    Reporting.AreEqual($"{Constants.Payment.OneOffPayments.CardPaymentMethod}", GetInnerText(XPath.OneOffPaymentPayment.PaymentMethod), "explantion for one off payment");
                    FillCreditCardDetailsIn(userPayment.CreditCardDetails, isCvnRequired: true);
                    Reporting.Log($"Capturing screen state after credit card input", _browser.Driver.TakeSnapshot());
                    break;
                case PaymentOptionsSpark.BPay:
                    VerifyBPayText(endorseCaravan.OriginalPolicyData.PolicyStartDate);
                    break;
                case PaymentOptionsSpark.PayLater:
                    VerifyPayLaterText(endorseCaravan.OriginalPolicyData.PolicyStartDate);
                    break;
                default:
                    throw new NotImplementedException("Payment option supplied is not supported.");
            }

        }

        /// <summary>
        /// Choose to make an automatic payment, select bank account or credit card 
        /// according to the available type of payment. Then add payment details for direct debit.
        /// </summary>
        private void AddNewPaymentMethod(PaymentV2 payment)
        {
            ClickControl(XPath.DirectDebitPayment.AddPaymentMethod);
            if (payment.CreditCardDetails != null)
            {
                ClickControl(XPath.DirectDebitPayment.NewCardOption);
                FillCreditCardDetailsIn(payment.CreditCardDetails, false);
            }
            else if (payment.BankAccountDetails != null)
            {
                ClickControl(XPath.DirectDebitPayment.NewBankAccountOption);
                EnterBankDetails(payment.BankAccountDetails);
                VerifyBSBDetails(payment.BankAccountDetails);
            }
        }

        private void VerifyBPayText(DateTime renewalDate)
        {
            ScrollElementIntoView(XPath.OneOffPaymentPayment.Information);
            Reporting.Log($"Capturing screen state after selecting BPay ", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual($"{Constants.Payment.OneOffPayments.BpayPaymentMethod}", GetInnerText(XPath.OneOffPaymentPayment.PaymentMethod), "explantion for BPAY");

            Reporting.IsTrue(IsWarningIconDisplayed, "BPay includes warning icon");
            Reporting.AreEqual($"{Constants.Payment.OneOffPayments.Subtitle}", DeferredPaymentTitle, "BPay title");
            Reporting.AreEqual($"{Constants.Payment.OneOffPayments.BpayBlurbPart1a}{renewalDate.ToString(DateTimeTextFormat.ddMMyyyy)}" +
                $"{Constants.Payment.OneOffPayments.BpayBlurbPart1b}", DeferredPaymentParagraphOne, "BPay paragraph 1");
            Reporting.AreEqual($"{Constants.Payment.OneOffPayments.BpayBlurbPart2}", DeferredPaymentParagraphTwo, "BPay paragraph 2");
        }

        private void VerifyPayLaterText(DateTime renewalDate)
        {
            ScrollElementIntoView(XPath.OneOffPaymentPayment.Information);
            Reporting.Log($"Capturing screen state after selecting Pay Later ", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual($"{Constants.Payment.OneOffPayments.PayLaterMethod}", GetInnerText(XPath.OneOffPaymentPayment.PaymentMethod), "explantion for pay later");

            Reporting.IsTrue(IsWarningIconDisplayed, "Pay Later page includes warning icon");
            Reporting.AreEqual($"{Constants.Payment.OneOffPayments.Subtitle}", DeferredPaymentTitle, "Pay Later title");
            Reporting.AreEqual($"{Constants.Payment.OneOffPayments.PayLaterBlurbPart1a}{renewalDate.ToString(DateTimeTextFormat.ddMMyyyy)}" +
                 $"{Constants.Payment.OneOffPayments.PayLaterBlurbPart1b}", DeferredPaymentParagraphOne, "Pay Later paragraph 2");
        }

        public void UpdateEmail(string newEmailAddress)
        {
            EmailAddress = newEmailAddress;
        }

        /// <summary>
        /// When setting up direct debit or performing a one off payment, terms must be accepted.
        /// Delayed payment options do not require terms to be displayed. 
        /// </summary>
        public void AcceptPaymentAuthorisationTermsWhenRequired(PaymentV2 userPayment)
        {
            if (userPayment.PaymentOption == PaymentOptionsSpark.AnnualCash)
                {
                var directDebitTermsRegex = new Regex(FixedTextRegex.ANNUAL_POLICY_ENDORSEMENT_CARD_PAYMENT_AUTHORISATION_REGEX);
                Match matchTerms = directDebitTermsRegex.Match(PaymentAuthorisationText);
                Reporting.IsTrue(matchTerms.Success, $"authorisation terms for credit card payment are present. Actual Result: {PaymentAuthorisationText}");

                var directDebitAuthorisationRegex = new Regex(FixedTextRegex.CARD_PAYMENT_AUTHORISATION_TERMS_AGREE_REGEX);
                Match matchAuthorisation = directDebitAuthorisationRegex.Match(PaymentAcknowledgementtext);
                Reporting.IsTrue(matchAuthorisation.Success, $"acknowledgement text for checkbox is present for credit card payment. Actual Result: {PaymentAcknowledgementtext}");

                ClickReadAgreeAuthorisationTerms();
            }
            else if (userPayment.PaymentOption == PaymentOptionsSpark.DirectDebit)
            {
                var directDebitTermsRegex = new Regex(FixedTextRegex.ANNUAL_BANK_MONTHLY_CARD_BANK_PAYMENT_AUTHORISATION_REGEX);
                Match matchTerms = directDebitTermsRegex.Match(PaymentAuthorisationText);
                Reporting.IsTrue(matchTerms.Success, $"authorisation terms for direct debit are present. Actual Result: {PaymentAuthorisationText}");

                var directDebitAuthorisationRegex = new Regex(FixedTextRegex.DIRECT_DEBIT_AUTHORISATION_TERMS_AGREE_REGEX);
                Match matchAuthorisation = directDebitAuthorisationRegex.Match(PaymentAcknowledgementtext);
                Reporting.IsTrue(matchAuthorisation.Success, $"acknowledgement text for checkbox is present for direct debit. Actual Result: {PaymentAcknowledgementtext}");

                ClickReadAgreeAuthorisationTerms();
            }
            else
            {
                // Payment terms are not to be displayed for BPay and Pay Later
                // Payment terms are not displayed for endorsement other than Premium Increase
                if (_driver.TryWaitForElementToBeVisible(By.XPath(XPathPayment.Detail.AuthorisationText), WaitTimes.T5SEC, out _))
                {
                    Reporting.Error("Authorisation Terms are only valid for Direct Debit or Card Payments");
                }
            }
        }

        public void ClickConfirm()
        {
            ScrollElementIntoView(XPath.Confirm.Button);
            Reporting.Log($"Capturing screen state before confirming", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Confirm.Button);
        }

        /// <summary>
        /// Clicking Try Again button resulting from failed payment
        /// </summary>
        private void ClickTryAgainButton()
        {
            ClickControl(XPath.FailedPayment.TryAgainButton);
        }

        /// <summary>
        /// Support submitting failed payment by clicking the Confirm button ( three times) and Try Again button (two times)
        /// The Try Again button will be displayed only in the case of Failed Payments
        /// </summary>
        public void CreateFailedPayment()
        {
            using (var spinner = new SparkSpinner(_browser))
            {
                ClickConfirm();
                spinner.WaitForSpinnerToFinish();
                Reporting.Log("Waiting for the 'Try Again' button for the first time", _browser.Driver.TakeSnapshot());
                ClickTryAgainButton();
                ClickConfirm();
                spinner.WaitForSpinnerToFinish();
                Reporting.Log("Waiting for the 'Try Again' button for the second time", _browser.Driver.TakeSnapshot());
                ClickTryAgainButton();
                ClickConfirm();
                spinner.WaitForSpinnerToFinish();
            }

        }
    }
}
