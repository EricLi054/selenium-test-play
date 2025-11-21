using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
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
    public class PayYourRenewal : SparkPaymentPage
    {
        #region CONSTANTS
        private class Constants
        {
            public const string Title = "Pay your renewal";
            public class PolicyCard
            {
                public const string Title = "Car insurance";
            }
            public class InstalmentSummary
            {
                public const string HowOftenLabel = "How often";

                public const string AnnualFrequency = "Annually";
                public const string MonthlyFrequency = "Monthly";

                public const string AmountLabel = "Amount";
                public const string AmountVariation = "Monthly amounts may vary. Check your policy documents for the exact amounts.";

                public const string PolicyRenewsLabel = "Policy renews on";
            }

            public static string RenewalDateInfor(string renewalStartDate) => $"Any changes you make will apply from {renewalStartDate}. If you want your changes to apply sooner, please call us on 13 17 03.";

            public class AnnualDirectDebitOption
            {
                public const string Title = "Would you like to set up annual direct debit?";
                public const string Text = "This means we'll debit your account in future years. " +
                    "We'll send your renewal documents before payment is due so you can make changes.";
            }

            public class PreferredMonthlyPaymentDate
            {
                public const string Title = "Preferred monthly payment date";
                public const string Text = "Choose your payment date for the next 12 months";
                public const string EveryMonth = "of every month";
                public static string DayOfEveryMonth(int day) => $"{DataHelper.AddOrdinal(day)} of every month";
            }

            public class Payment
            {
                public const string Title = "Choose how to pay";

                public class DirectDebitPayments
                {

                    public const string MonthlyDirectDebitText = "We'll direct debit all future payments from this account.";
                    public const string AnnualDirectDebitText = "We'll direct debit this account annually when payment is due.";
                    public const string TextExpiredCard = "If your credit card is expired, you'll need to add a new credit card.";
                }

                public class OneOffPayments
                {
                    public const string TextExplanation = "Pay by card now or pay later using your card, BPAY® or another method.";
                    public const string Subtitle = "Select 'Confirm' to renew";
                    public const string CardPaymentMethod = "This will be a one-off payment";
                    public const string BpayPaymentMethod = "You can BPAY using your online banking";
                    public const string BpayBlurbPart1a = "Your payment is due on ";  // [dd/MM/yyyy] date follows this blurb 
                    public const string BpayBlurbPart1b = ". We'll email your renewal documents.";
                    public const string BpayBlurbPart2 = "The biller code and reference number will be on the next screen.";
                    public const string PayLaterMethod = "You can pay for your insurance later";
                    public const string PayLaterBlurbPart1a = "Your payment is due on "; // [dd/MM/yyyy] date follows this blurb
                    public const string PayLaterBlurbPart1b = ". We'll email your renewal documents and details on how to pay.";
                }
            }


            public class Email
            {
                public const string Title = "Confirm your email address";
                public const string Text = "We'll update your contact details (not your login email) if you provide a new email.";
                public const string Validation = "Please enter a valid email";
            }

        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public const string Title = "//h2[@id='payment-page-title']";
            public class PolicyCard
            {
                public const string PolicyCardTitle = "id('policy-card-content-policy-details-header-title-policy-details')";
                public const string PolicyCover     = "id('policy-card-content-policy-details-header-subtitle-policy-details')";
                public const string CarModel        = "id('policy-card-content-policy-details-properties-policy-details')//p[contains(@id,'model-policy-details')]";
                public const string CarRegistration = "id('policy-card-content-policy-details-properties-policy-details')//p[contains(@id,'registration-policy-details')]";
                public const string PolicyNumber    = "id('policy-card-content-policy-details-properties-policy-details')//p[contains(@id,'policy-number-policy-details')]";
            }

            public class InstalmentSummary
            {
                public const string FrequencyLabel = "id('frequency-label')";
                public const string FrequencyValue = "id('frequency-value')";
                public const string AmountLabel = "id('amount-label')";
                public const string AmountValue = "id('amount-value')";
                public const string AmountExtraInfo = "id('amount-subtitle')";
                public const string RenewDateLabel = "id('start-date-label')";
                public const string RenewDateValue = "id('start-date-value')";
                public const string RenewDateInfo = "id('start-date-subtitle')";
            }

            public class PreferredDate
            {
                public const string Title = "//label[@for='payment-day-text-input']";
                public const string Explanation = Title + "/following-sibling::p";
                public const string Input = "//input[@id='payment-day-text-input']";
            }

            public class FailedPayment
            {
                public const string TryAgainButton = "//button[text()='Try again']";
            }

            public class AnnualDirectDebit
            {
                public const string Information = "//label[@for='direct-debit-opt-in']";
                public const string Blurb = Information + "//p";
                public const string Selection = "//div[@id='direct-debit-opt-in']";

                public class Button
                {
                    public const string Yes = "//button[@id='direct-debit-opt-in-true']";
                    public const string No = "//button[@id='direct-debit-opt-in-false']";
                }
            }

            public class DirectDebitPayment
            {
                public class Preamble
                {
                    public const string Container = "//div[@id='select-payment-method-select-account-preamble']";
                    public static string SectionTitle => $"//b[text()='{Constants.Payment.Title}']";
                    public const string ParagraphOne = Container + "/p[1]";
                    public const string ParagraphTwo = Container + "/p[2]";
                }

                // Options presented to the member are to use an existing credit card or bank acoount
                // or add a new one
                public const string RadioGroup = "id('select-payment-method-add-payment-method')";
                public const string AddPaymentMethod = "//div[@id='select-payment-method-add-payment-method-radio-container']/label";

                public const string NewCardOption = "//button[@id='add-payment-method-input-payment-method-Card']";
                public const string NewBankAccountOption = "//button[@id='add-payment-method-input-payment-method-Bank account']";
            }

            /// <summary>
            /// Information and options for one off payments
            /// </summary>
            public class OneOffPaymentPayment
            {
                public const string Information = "//div[@id='stacked-card-container-content']";
                public const string Blurb = Information + "//h2";
                public const string OptionSelected = "//div[@id='cmp-add-payment-method-input-pay-later-option']/button[@aria-pressed,'true')]";
                public const string PaymentMethod = "id('payment-method-message')";

                public class Button
                {
                    public const string Card = "//button[@id='cmp-add-payment-method-input-pay-later-option-toggle-card']";
                    public const string BPay = "//button[@id='cmp-add-payment-method-input-pay-later-option-toggle-bpay']";
                    public const string PayLater = "//button[@id='cmp-add-payment-method-input-pay-later-option-toggle-paylater']";
                }
                // For Pay Later and BPay
                public class BPayAndPayLaterPayment
                {
                    public const string Icon = "id('notification-card-icon')";
                    public const string DeferredPaymentTitle = "id('notification-card-title')";
                    public const string ParagraphOne = "id('annual-payment-option-notification-part1')";
                    public const string ParagraphTwo = "id('annual-payment-option-notification-part2')";
                }
            }
            public class Email
            {
                public const string Label = "//label[@for='email']";
                public const string InputField = "id('email')";
            }

            public class Confirm
            {
                public const string Button = "id('confirm')";
            }
        }
        #endregion

        #region Settable properties and controls
        private string PageTitleValue => GetInnerText(XPath.Title);
        private string PolicyCardTitle => GetInnerText(XPath.PolicyCard.PolicyCardTitle);
        private string PolicyCardCover => GetInnerText(XPath.PolicyCard.PolicyCover);
        private string PolicyCardModel => GetInnerText(XPath.PolicyCard.CarModel);
        private string PolicyCardRego => GetInnerText(XPath.PolicyCard.CarRegistration);
        private string PolicyNumber => GetInnerText(XPath.PolicyCard.PolicyNumber);
        private string FrequencyLabel => GetInnerText(XPath.InstalmentSummary.FrequencyLabel);
        private string FrequencyValue => GetInnerText(XPath.InstalmentSummary.FrequencyValue);
        private string AmountLabel => GetInnerText(XPath.InstalmentSummary.AmountLabel);
        private string AmountValue => GetInnerText(XPath.InstalmentSummary.AmountValue);
        private string AmountExtraInfo => GetInnerText(XPath.InstalmentSummary.AmountExtraInfo);
        private string RenewalDateLabel => GetInnerText(XPath.InstalmentSummary.RenewDateLabel);
        private string RenewalDateValue => GetInnerText(XPath.InstalmentSummary.RenewDateValue);
        private string RenewalDateInfo => GetInnerText(XPath.InstalmentSummary.RenewDateInfo);
        private string PreferredDateTitle => GetInnerText(XPath.PreferredDate.Title);
        private string PreferredDateExplanation => GetInnerText(XPath.PreferredDate.Explanation);
        private string PreferredDateInput => GetValue(XPath.PreferredDate.Input);
        private string AnnualDirectDebitSelectionLabel => GetInnerText(XPath.AnnualDirectDebit.Information);
        private string AnnualDirectDebitSelectionBlurb => GetInnerText(XPath.AnnualDirectDebit.Blurb);

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

        public PayYourRenewal(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
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

        public void VerifyPolicyCard(EndorseCar endorseCar)
        {

            string policyCover = null;
            Car insuredCar = new Car();
            insuredCar = endorseCar.ChangeMakeAndModel ? endorseCar.NewInsuredAsset : endorseCar.InsuredAsset;

            switch (endorseCar.CoverType)
            {
                case MotorCovers.MFCO:
                    policyCover = "Comprehensive";
                    break;
                case MotorCovers.TFT:
                    policyCover = "Third party fire & theft";
                    break;
                case MotorCovers.TPO:
                    policyCover = "Third party";
                    break;
                default:
                    break;
            }
            Reporting.AreEqual(Constants.PolicyCard.Title, PolicyCardTitle, "Policy card title");
            Reporting.AreEqual(policyCover, PolicyCardCover, "Policy card cover type");
            Reporting.Log($"Expecting the following to be contained in the Model field: '{insuredCar.Make} {insuredCar.Model}'");
            Reporting.IsTrue(PolicyCardModel.Contains($"{insuredCar.Make} {insuredCar.Model}"), "Policy card car make and model");

            if (!string.IsNullOrEmpty(endorseCar.NewInsuredAsset.Registration) && !endorseCar.ChangeMakeAndModel)
            {
                Reporting.AreEqual($"Registration: {endorseCar.NewInsuredAsset.Registration}", PolicyCardRego, "Policy card car registration number");
            }
            else
            {
                Reporting.AreEqual($"Registration: {insuredCar.Registration}", PolicyCardRego, "Policy card car registration number");
            }
            
            Reporting.AreEqual($"Policy number: {endorseCar.PolicyNumber}", PolicyNumber, "Policy card policy number");
        }

        public void VerifyPremiumDetails(EndorseCar endorseCar)
        {
            var paymentFrequency = endorseCar.SparkExpandedPayment.IsAnnual ? Constants.InstalmentSummary.AnnualFrequency : Constants.InstalmentSummary.MonthlyFrequency;
            Reporting.AreEqual(paymentFrequency, FrequencyValue, "How often policy paid");
            VerifyAmountWithTruncationWhenWholeDollar(endorseCar.PremiumChangesAfterEndorsement.Total, AmountValue, "new premium is consistent with proposed");
            if (!endorseCar.SparkExpandedPayment.IsAnnual)
            {
                Reporting.AreEqual(Constants.InstalmentSummary.AmountVariation, AmountExtraInfo, "text for how amounts can vary due to rounding");
            }
            Reporting.AreEqual(endorseCar.OriginalPolicyData.EndorsementStartDate.ToString(DateTimeTextFormat.ddMMyyyy), RenewalDateValue, "renewal date shown correctly");
            Reporting.AreEqual(Constants.RenewalDateInfor(RenewalDateValue), RenewalDateInfo, "Policy Renewal date extra info");
            if (!endorseCar.SparkExpandedPayment.IsAnnual)
            {
                Reporting.AreEqual(Constants.InstalmentSummary.AmountVariation, AmountExtraInfo, "text for how amounts can vary due to rounding");
                Reporting.AreEqual(Constants.PreferredMonthlyPaymentDate.DayOfEveryMonth(endorseCar.OriginalPolicyData.NextPayableInstallment.CollectionDate.Day), PreferredDateInput, "preferred collection day of every month");
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
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate BSB validations</param>
        public void CompletePaymentInputs(EndorseCar endorseCar, bool detailUiChecking = false)
        {
            var userPayment = endorseCar.SparkExpandedPayment;

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
                    if (endorseCar.SparkExpandedPayment.BankAccountDetails != null && detailUiChecking)
                    {
                        ClickControl(XPath.DirectDebitPayment.AddPaymentMethod);
                        ClickControl(XPath.DirectDebitPayment.NewBankAccountOption);
                        EnterInvalidNoMatchBSBAndCheckErrorMessage(null);
                    }
                    AddNewPaymentMethod(endorseCar.SparkExpandedPayment);
                    break;
                case PaymentOptionsSpark.AnnualCash:
                    Reporting.AreEqual($"{Constants.Payment.OneOffPayments.CardPaymentMethod}", GetInnerText(XPath.OneOffPaymentPayment.PaymentMethod), "explantion for one off payment");
                    FillCreditCardDetailsIn(userPayment.CreditCardDetails, isCvnRequired: true);
                    Reporting.Log($"Capturing screen state after credit card input", _browser.Driver.TakeSnapshot());
                    break;
                case PaymentOptionsSpark.BPay:
                    VerifyBPayText(endorseCar.OriginalPolicyData.PolicyStartDate);
                    break;
                case PaymentOptionsSpark.PayLater:
                    VerifyPayLaterText(endorseCar.OriginalPolicyData.PolicyStartDate);
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

        public void UpdateEmail(Contact contact)
        {
            //Generate a random email address if member don't have one previously
            if (string.IsNullOrEmpty(contact.GetEmail()))
            {
                EmailAddress = DataHelper.RandomEmail(contact.FirstName, contact.Surname, Config.Get().Email.Domain).Address;
            }
            else
            {
                EmailAddress = contact.GetEmail();
            }
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
                spinner.WaitForSpinnerToFinish(WaitTimes.T150SEC);
                Reporting.Log("Waiting for the 'Try Again' button for the first time", _browser.Driver.TakeSnapshot());
                ClickTryAgainButton();
                ClickConfirm();
                spinner.WaitForSpinnerToFinish(WaitTimes.T150SEC);
                Reporting.Log("Waiting for the 'Try Again' button for the second time", _browser.Driver.TakeSnapshot());
                ClickTryAgainButton();
                ClickConfirm();
                spinner.WaitForSpinnerToFinish(WaitTimes.T150SEC);
            }

        }
    }
}
