using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using static Rac.TestAutomation.Common.Constants;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.DatabaseCalls.Policies.ShieldMotorDB;

namespace UIDriver.Pages.Spark.Endorsements
{
    public class RenewalConfirmation : SparkPaymentPage
    {
        #region CONSTANTS
        private static class Constants
        {
            public static readonly string YoureAllSet                                   = "You're all set, ";
            public static readonly string Payment                                       = "Payment:";
            public static readonly string PolicyRenewsOn                                = "Policy renews on:";
            public static readonly string PolicyNumber                                  = "Policy number:";
            public static readonly string ReceiptNumber                                 = "Receipt number:";
            public static readonly string AnnualPayment                                 = "Annual payment";
            public static readonly string MonthlyPayment                                = "Monthly payment";

            public static class BPay
            {
                public static readonly string Name                                      = @"BPAY";
                public static readonly string Title                                     = @"Don't forget to pay!";
                public static readonly string SubTitle                                  = @"Pay by phone, online or mobile banking.";
                public static readonly string BlurbPart1a                               = "Please pay by ";
                public static readonly string BlurbPart1b                               = " to stay covered.";
                public static readonly string BlurbPart2                                = "You'll receive an email with your renewal documents and details on how to pay shortly.";
                public static readonly string BillerCode                                = @"Biller code:";
                public static readonly string Reference                                 = @"Reference:";
            }

            public static class PayLater
            {
                public static readonly string Title                                     = "Don't forget to pay!";
                public static readonly string BlurbPart1a                               = "Please pay by ";
                public static readonly string BlurbPart1b                               = " to stay covered.";
                public static readonly string BlurbPart2                                = @"You'll receive an email with your renewal documents and details on how to pay shortly.";
            }

            public static class AnnualCash
            {
                public static readonly string PaidMessage                               = @"You've renewed your policy and paid.";
                public static readonly string RecordsMessage                            = @"Please save a copy of this screen for your records.";
            }

            public static class DirectDebit
            {
                public static readonly string ConfirmationAnnualDirectDebit             = "You've renewed your policy and have chosen to pay annually. " +
                                                                                            "You'll receive an email with your renewal documents shortly.";
                public const string ConfirmationMonthlyDirectDebit                      = "You've renewed your policy and have chosen to pay monthly. " +
                                                                                            "You'll receive an email with your renewal documents shortly.";
                public static readonly string DebitDateInformation                      = "We'll debit your account on this date";
                public static readonly string NextPaymentDue                            = "Next payment date: ";
            }

            public static class PaymentFailed
            {
                public static readonly string CallUsToPlay                          = "Please call us to pay";
                public static readonly string ParagraphOne                          = "We couldn't process your payment.";
                public static readonly string ParagraphTwo                          = "But we renewed your policy and emailed your renewal documents.";
                public static readonly string ParagraphThreeDueDate                 = "Your payment is due ";
                public static readonly string ParagraphThreeDueDatePolicyCancelled  = " If you forget to pay, your policy will be cancelled.";
            }

            public static class Monthlypayment
            {
                public static readonly string MonthlyPaymentReceivedTitle           = "Monthly payment";
                public static readonly string IncreasePaymentMethodCC               = "Monthly payment: Credit card";
                public static readonly string IncreasePaymentMethodBankAccount      = "Monthly payment: Bank account";
                public static readonly string Paragraph                             = "You've updated your policy. You'll receive an email with your policy documents shortly.";
            }

            public static class NoChangePremium
            {
                public static readonly string ParagraphOne                          = "You've updated your policy. Start date for this change is ";
                public static readonly string ParagraphTwo                          = "You'll receive an email with your policy documents shortly.";
            }

            public static class CreditCardRefund
            {
                public static readonly string CreditCardRefundTitle                 = "Your refund";
                public static readonly string CreditCardRefundParagraph             = "You've updated your policy and you'll get a refund. You'll receive an email with your policy documents shortly.";
                public static readonly string CreditCardRefundDestination           = "Refund to: Credit card";
                public static readonly string BankRefundDestination                 = "Refund to: Bank account";
                public static readonly string ProcessingTime                        = "Processing time: 3-7 business days from your change date";
            }

            public static class IncreasePremiumCreditCard
            {
                public static readonly string IncreasePremiumParagraph              = "You've updated your policy and paid. You'll receive an email with your policy documents shortly.";
                public static readonly string PaymentReceivedTitle                  = "Payment received";
                public static readonly string IncreasePaymentMethod                 = "Payment: Credit card";
                public static readonly  string ReceiptFooter                        = "Please save a copy of this screen for your records.";
            }

            public static class IncreasePremiumInstallments
            {
                public static readonly  string IncreasePremiumParagraph             = "You've updated your policy. You'll receive an email with your policy documents shortly.";
                public static readonly string PaymentReceivedTitle                  = "Payment";
                public static readonly string IncreasePaymentMethodCC               = "Payment: Credit card";
                public static readonly string IncreasePaymentMethodBankAccount      = "Payment: Bank account";
            }
        }
        #endregion

        #region XPATHS
        private static class XPath
        {
            public static class AnnualCash
            {
                public static readonly string Title                 = "id('renewal-annual-policy-paid-title')";
                public static readonly string SubTitle              = "id('renewal-annual-policy-paid-paragraph')";
                public static readonly string Type                  = "id('renewal-annual-policy-paid-card-title')";
                public static readonly string Amount                = "id('renewal-annual-policy-paid-card-amount')";
                public static readonly string Method                = "id('renewal-annual-policy-paid-payment-method')";
                public static readonly string RenewalDate           = "id('renewal-annual-policy-paid-renews-on')";
                public static readonly string PolicyNumber          = "id('renewal-annual-policy-paid-policy-number')";
                public static readonly string ReceiptNumber         = "id('renewal-annual-policy-paid-receipt-number')";
                public static readonly string RecordRetentionNotice = "id('renewal-annual-policy-paid-receipt-footer')";
            }

            public static class DirectDebit
            {
                public static readonly string Title                 = "id('direct-debit-title')";
                public static readonly string Paragraph             = "id('direct-debit-paragraph')";
                public static readonly string Type                  = "id('direct-debit-card-title')";
                public static readonly string Amount                = "id('direct-debit-card-amount')";
                public static readonly string Method                = "id('direct-debit-payment-method')";
                public static readonly string RenewalDate           = "id('direct-debit-renewal-date')";
                public static readonly string PaymentDue            = "id('direct-debit-payment-due')";
                public static readonly string DebitDateInfo         = "id('direct-debit-debit-date')";
                public static readonly string PolicyNumber          = "id('direct-debit-policy-number')";
            }

            public static class BPay
            {
                public static readonly string Title                 = "id('renewal-annual-pay-later-title')";
                public static readonly string ParagraphOne          = "id('renewal-annual-pay-later-paragraph-one')";
                public static readonly string ParagraphTwo          = "id('renewal-annual-pay-later-paragraph-two')";
                public static readonly string Warning               = "id('renewal-annual-pay-later-warning-title')";
                public static string Subtitle(string policyNumber) => $"id('renewal-annual-pay-later-bpay-{policyNumber}-subtitle')";
                public static readonly string Logo                  = "id('BPAY_Logo')";
                public static string BPayName(string policyNumber) => $"id('renewal-annual-pay-later-bpay-{policyNumber}-title')";
                public static string Code(string policyNumber)      => $"id('renewal-annual-pay-later-bpay-{policyNumber}-biller-code')";
                public static string Reference(string policyNumber) => $"id('renewal-annual-pay-later-bpay-{policyNumber}-reference')";
                public static readonly string PolicyNumber              = "id('renewal-annual-pay-later-policy-number')";
            }

            public static class PayLater
            {
                public static readonly string Title                 = "id('pay-later-title')";
                public static readonly string ParagraphOne          = "id('pay-later-paragraph-one')";
                public static readonly string ParagraphTwo          = "id('pay-later-paragraph-two')";
                public static readonly string KeyInformation        = "id('pay-later-warning-title')";
                public static readonly string PolicyNumber          = "id('pay-later-policy-number')";
            }

            public static class Button
            {
                public static readonly string BackToMyPolicy        = "//a[text()='Back to my policy']";
            }

            public static class PaymentFailed
            {
                public static readonly string ErrorTitle            = "id('realtime-payment-error-title')";
                public static readonly string ParagraphOne          = "id('realtime-payment-error-paragraph-one')";
                public static readonly string ParagraphTwo          = "id('realtime-payment-error-paragraph-two')";
                public static readonly string ErrorPolicyNumber     = "id('realtime-payment-error-policy-number')";
                public static readonly string ParagraphThree        = "id('realtime-payment-error-paragraph-three')";
                public static readonly string CallBackButton        = "id('realtime-payment-error-phone-button')";
            }

            public static class NoChangePremium
            {
                public static readonly string Title                     = "id('midterm-no-premium-change-title')";
                public static readonly string NoChangePremiumParagraph  = "id('midterm-no-premium-change-paragraph')";
                public static readonly string PolicyNumber              = "id('midterm-no-premium-change-policy-number')";
            }

            public static class RefundCreditCard
            {
                public static readonly string RefundAllSet              = "id('midterm-refund-paid-title')";
                public static readonly string RefundParagraph           = "id('midterm-refund-paid-paragraph')";
                public static readonly string RefundCardTitle           = "id('midterm-refund-paid-card-title')";
                public static readonly string Amount                    = "id('midterm-refund-paid-card-amount')";
                public static readonly string RefundDestination         = "id('midterm-refund-paid-refund-destination')";
                public static readonly string ProcessingTime            = "id('midterm-refund-paid-processing-time')";
                public static readonly string StartDate                 = "id('midterm-refund-paid-endorsement-start-date')";
                public static readonly string PolicyNumber              = "id('midterm-refund-paid-policy-number')";
                public static readonly string BackToMyPoliciesButton    = "id('midterm-refund-paid-back-to-my-policy')";
            }

            public static class IncreasePremiumCreditCard
            {
                public static readonly string IncreasePremiumAllSet         = "id('midterm-policy-paid-title')";
                public static readonly string IncreasePremiumParagraph      = "id('midterm-policy-paid-paragraph')";
                public static readonly string PaymentReceivedTitle          = "id('midterm-policy-paid-card-title')";
                public static readonly string Amount                        = "id('midterm-policy-paid-card-amount')";
                public static readonly string PaymentMethod                 = "id('midterm-policy-paid-payment-method')";
                public static readonly string StartDate                     = "id('midterm-policy-paid-endorsement-start-date')";
                public static readonly string PolicyNumber                  = "id('midterm-policy-paid-policy-number')";
                public static readonly string ReceiptNumber                 = "id('midterm-policy-paid-receipt-number')";
                public static readonly string ReceiptFooter                 = "id('midterm-policy-paid-receipt-footer')";
                public static readonly string BackToMyPoliciesButton        = "id('midterm-policy-paid-back-to-my-policy')";
            }

            public static class MonthlyInstallment
            {
                public static readonly string IncreasePremiumAllSet             = "id('midterm-monthly-instalment-title')";
                public static readonly string Paragraph                         = "id('midterm-monthly-instalment-paragraph')";
                public static readonly string CardTitle                         = "id('midterm-monthly-instalment-card-title')";
                public static readonly string InstallmentPaymentMethod          = "id('midterm-monthly-instalment-payment-method')";
                public static readonly string StartDate                         = "id('midterm-monthly-instalment-endorsement-start-date')";
                public static readonly string NextPaymentDate                   = "id('midterm-monthly-instalment-collection-date')";
                public static readonly string InstallmentAmount                 = "id('midterm-monthly-instalment-card-amount')";
                public static readonly string PolicyNumber                      = "id('midterm-monthly-instalment-policy-number')";
                public static readonly string BackToMyPolicy                    = "id('midterm-monthly-instalment-back-to-my-policy')";
            }
        }
        #endregion

        #region Settable properties and controls

        private string AnnualCashTitle => GetInnerText(XPath.AnnualCash.Title);
        private string AnnualCashSubTitle => GetInnerText(XPath.AnnualCash.SubTitle);
        private string AnnualCashType => GetInnerText(XPath.AnnualCash.Type);
        private string AnnualCashAmount => GetInnerText(XPath.AnnualCash.Amount);
        private string AnnualCashMethod => GetInnerText(XPath.AnnualCash.Method);
        private string AnnualCashRenewalDate => GetInnerText(XPath.AnnualCash.RenewalDate);
        private string AnnualCashPolicyNumber => GetInnerText(XPath.AnnualCash.PolicyNumber);
        private string AnnualCashReceiptNumber => GetInnerText(XPath.AnnualCash.ReceiptNumber);
        private string AnnualCashRecordRetention => GetInnerText(XPath.AnnualCash.RecordRetentionNotice);

        private string BPayTitle => GetInnerText(XPath.BPay.Title);
        private string BPayWarning => GetInnerText(XPath.BPay.Warning);
        private string BPayPolicyNumber => GetInnerText(XPath.BPay.PolicyNumber);
        private string BPayParagraphOne => GetInnerText(XPath.BPay.ParagraphOne);
        private string BPayParagraphTwo => GetInnerText(XPath.BPay.ParagraphTwo);
        private string BPaySubtitle(string policyNumber) => GetInnerText(XPath.BPay.Subtitle(policyNumber));
        private bool IsBPayLogoDisplayed => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.BPay.Logo),WaitTimes.T5SEC, out IWebElement _);
        private string BPayName(string policyNumber) => GetInnerText(XPath.BPay.BPayName(policyNumber));
        private string BPayCode(string policyNumber) => GetInnerText(XPath.BPay.Code(policyNumber));
        private string BPayReference(string policyNumber) => GetInnerText(XPath.BPay.Reference(policyNumber));

        private string PayLaterTitle => GetInnerText(XPath.PayLater.Title);
        private string PayLaterParagraphOne => GetInnerText(XPath.PayLater.ParagraphOne);
        private string PayLaterParagraphTwo => GetInnerText(XPath.PayLater.ParagraphTwo);
        private string PayLaterKeyInformation => GetInnerText(XPath.PayLater.KeyInformation);
        private string PayLaterPolicyNumber => GetInnerText(XPath.PayLater.PolicyNumber);

        // TODO: SPK-4270: Improvement: Check content for each container instead of every single line.
        // TODO: SPK-4270: Reference: https://github.com/racwa/raci-test-automation-selenium/pull/275#pullrequestreview-1417773085
        // TODO: SPK-4270: Start
        private string DirectDebitTitle => GetInnerText(XPath.DirectDebit.Title);
        private string DirectDebitType => GetInnerText(XPath.DirectDebit.Type);
        private string DirectDebitAmount => GetInnerText(XPath.DirectDebit.Amount);
        private string DirectDebitParagraph => GetInnerText(XPath.DirectDebit.Paragraph);
        private string DirectDebitMethod => GetInnerText(XPath.DirectDebit.Method);
        private string DirectDebitRenewalDate => GetInnerText(XPath.DirectDebit.RenewalDate);
        private string DirectDebitDebitDateInfo => GetInnerText(XPath.DirectDebit.DebitDateInfo);
        private string DirectDebitPaymentDue => GetInnerText(XPath.DirectDebit.PaymentDue); 
        private string DirectDebitPolicyNumber => GetInnerText(XPath.DirectDebit.PolicyNumber);
        // TODO: SPK-4270: End

        private string AnnualCashErrorTitle => GetInnerText(XPath.PaymentFailed.ErrorTitle);
        private string FailedPaymentParagraphOne => GetInnerText(XPath.PaymentFailed.ParagraphOne);
        private string FailedPaymentParagraphTwo => GetInnerText(XPath.PaymentFailed.ParagraphTwo);
        private string FailedPaymentParagraphThree => GetInnerText(XPath.PaymentFailed.ParagraphThree);
        private string NoPremiumChangePolicyTitle => GetInnerText(XPath.NoChangePremium.Title);
        private string NoPremiumChangeParagraph => GetInnerText(XPath.NoChangePremium.NoChangePremiumParagraph);
        private string NoPremiumChangePolicyNumber => GetInnerText(XPath.NoChangePremium.PolicyNumber);

        //Known and UnKnowm Credit Card Refund
        private string RefundAllSet => GetInnerText(XPath.RefundCreditCard.RefundAllSet);
        private string RefundParagraph => GetInnerText(XPath.RefundCreditCard.RefundParagraph);
        private string RefundCardTitle => GetInnerText(XPath.RefundCreditCard.RefundCardTitle);
        private string Amount => GetInnerText(XPath.RefundCreditCard.Amount);
        private string RefundDestination => GetInnerText(XPath.RefundCreditCard.RefundDestination);
        private string ProcessingTime => GetInnerText(XPath.RefundCreditCard.ProcessingTime);
        private string StartDate => GetInnerText(XPath.RefundCreditCard.StartDate);
        private string PolicyNumber => GetInnerText(XPath.RefundCreditCard.PolicyNumber);

        // Monthly Installment
        private string AllSet => GetInnerText(XPath.MonthlyInstallment.IncreasePremiumAllSet);
        private string Paragraph => GetInnerText(XPath.MonthlyInstallment.Paragraph);
        private string CardTitle => GetInnerText(XPath.MonthlyInstallment.CardTitle);
        private string InstallmentPaymentMethod => GetInnerText(XPath.MonthlyInstallment.InstallmentPaymentMethod);
        private string ChangeStartDate => GetInnerText(XPath.MonthlyInstallment.StartDate);
        private string NextPaymentDate => GetInnerText(XPath.MonthlyInstallment.NextPaymentDate);
        private string InstallmentAmount => GetInnerText(XPath.MonthlyInstallment.InstallmentAmount);
        private string PolicyNumberEndorsed => GetInnerText(XPath.MonthlyInstallment.PolicyNumber);

        //Increase in Premium - Pay via Credit Card
        private string IncreasePremiumAllSet => GetInnerText(XPath.IncreasePremiumCreditCard.IncreasePremiumAllSet);
        private string IncreasePremiumParagraph => GetInnerText(XPath.IncreasePremiumCreditCard.IncreasePremiumParagraph);
        private string PaymentReceivedTitle => GetInnerText(XPath.IncreasePremiumCreditCard.PaymentReceivedTitle);
        private string IncreaseAmount => GetInnerText(XPath.IncreasePremiumCreditCard.Amount);
        private string IncreasePaymentMethod => GetInnerText(XPath.IncreasePremiumCreditCard.PaymentMethod);
        private string EndorseStartDate => GetInnerText(XPath.IncreasePremiumCreditCard.StartDate);
        private string EndorsedPolicyNumber => GetInnerText(XPath.IncreasePremiumCreditCard.PolicyNumber);
        private string ReceiptNumber => GetInnerText(XPath.IncreasePremiumCreditCard.ReceiptNumber);
        private string ReceiptFooter => GetInnerText(XPath.IncreasePremiumCreditCard.ReceiptFooter);

        #endregion

        public RenewalConfirmation(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Button.BackToMyPolicy);
                Reporting.Log($"Button : {GetInnerText(XPath.Button.BackToMyPolicy)} is Displayed");
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// When an Annual Cash payment is done by credit card. Ensure the correct details are shown.
        /// </summary>
        public void VerifyPageAnnualCash(EndorseCaravan endorseCaravan)
        {
            Reporting.Log($"Annual Cash confirmation page when policy paid by one off credit card payment", _browser.Driver.TakeSnapshot());

            var updatedPolicyData = DataHelper.GetPolicyDetails(endorseCaravan.PolicyNumber);

            Reporting.AreEqual($"{Constants.YoureAllSet}{endorseCaravan.ActivePolicyHolder.FirstName}!", AnnualCashTitle,
                "page confirmation title for annual cash");

            Reporting.AreEqual(Constants.AnnualCash.PaidMessage, AnnualCashSubTitle, "policy paid confirmation is present");

            Reporting.AreEqual(Constants.AnnualPayment, AnnualCashType, "frequency of payment given correctly");

            var expectedAmount = updatedPolicyData.NextPendingInstallment().Amount.Total;
            VerifyAmountWithTruncationWhenWholeDollar(expectedAmount, AnnualCashAmount, "payment amount is displayed correctly");

            Reporting.AreEqual(expectedAmount, endorseCaravan.PremiumChangesAfterEndorsement.Total, "premium payment ammount matches proposed amount");
            Reporting.IsTrue(updatedPolicyData.IsPaidInFull, "policy API has recorded the policy as paid");

            Reporting.AreEqual($"{Constants.Payment} {CREDIT_CARD}", AnnualCashMethod, $"{CREDIT_CARD} is shown as the payment type");
            Reporting.AreEqual($"{Constants.PolicyRenewsOn} {updatedPolicyData.PolicyStartDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH, CultureInfo.InvariantCulture)}",
                AnnualCashRenewalDate, "policy renewal date information is shown");
            Reporting.AreEqual($"{Constants.PolicyNumber} {endorseCaravan.PolicyNumber}", AnnualCashPolicyNumber, "policy number is shown");

            var expectedTextRegex = new Regex($"^({Constants.ReceiptNumber}\\s*)(\\d+)$");
            var match = expectedTextRegex.Match(AnnualCashReceiptNumber);
            if (match.Success && match.Groups.Count == 3)
            {
               Payment.VerifyWestpacQuickStreamDetailsAreCorrect<EndorseCar>(cardDetails: endorseCaravan.SparkExpandedPayment.CreditCardDetails,
                                                    policyNumber: endorseCaravan.PolicyNumber,
                                                    expectedPrice: updatedPolicyData.TotalInstallmentPremium.Total,
                                                    expectedReceiptNumber: match.Groups[2].Value);
            }
            else
            {
                Reporting.Error("Problem with displaying the Westpac receipt");
            }

            Reporting.AreEqual(Constants.AnnualCash.RecordsMessage, AnnualCashRecordRetention, "advice to member to retain a copy for their records");
        }

        /// <summary>
        /// Confirm the Failed payment confirmation page.
        /// Verifying the policy number, policy due date, buttons and other text elements
        /// </summary>
        public void VerifyFailedPayment(EndorseCaravan endorseCaravan)
        {
            Reporting.Log($"Annual Cash confirmation page when policy payment with credit card is failed", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(Constants.PaymentFailed.CallUsToPlay, AnnualCashErrorTitle, $"{Constants.PaymentFailed.CallUsToPlay} is displayed");
            Reporting.AreEqual(Constants.PaymentFailed.ParagraphOne, FailedPaymentParagraphOne, $"{Constants.PaymentFailed.ParagraphOne} is displayed");
            Reporting.AreEqual(Constants.PaymentFailed.ParagraphTwo, FailedPaymentParagraphTwo, $"{Constants.PaymentFailed.ParagraphTwo} is displayed");
            Reporting.AreEqual($"{Constants.PaymentFailed.ParagraphThreeDueDate}{DataHelper.AddDaysToDate(endorseCaravan.OriginalPolicyData.PolicyStartDate, 28)}.{Constants.PaymentFailed.ParagraphThreeDueDatePolicyCancelled}",
                DataHelper.StripLineFeedAndCarriageReturns(GetInnerText(XPath.PaymentFailed.ParagraphThree), replaceWithWhiteSpace: false), "due date and text is displayed");
            Reporting.AreEqual($"Policy number: {endorseCaravan.OriginalPolicyData.PolicyNumber}", GetInnerText(XPath.PaymentFailed.ErrorPolicyNumber),"policy number is displayed");
            Reporting.IsTrue(IsControlDisplayed(XPath.PaymentFailed.CallBackButton), "'Call Back' button is displayed");
            Reporting.IsTrue(IsControlDisplayed(XPath.Button.BackToMyPolicy), "'Back to my Policy' button is displayed");
        }

        /// <summary>
        /// Confirm the content of Annual Cash BPay confirmation page.
        /// </summary>
        public void VerifyBpay(EndorseCaravan endorseCaravan)
        {
            Reporting.Log($"BPay", _browser.Driver.TakeSnapshot());
            var updatedPolicyData = DataHelper.GetPolicyDetails(endorseCaravan.PolicyNumber);
            var renewalDateString = endorseCaravan.OriginalPolicyData.PolicyStartDate.ToString(DateTimeTextFormat.ddMMyyyy);

            Reporting.AreEqual(Constants.BPay.Title, BPayTitle, "BPay title is displayed");

            Reporting.AreEqual($"{Constants.BPay.BlurbPart1a}{renewalDateString}{Constants.BPay.BlurbPart1b}",
                BPayParagraphOne, "message that policy is renewed, renewal date and pay by date is correct.");
            Reporting.AreEqual($"{Constants.BPay.BlurbPart2}", BPayParagraphTwo, "message about emailing renewal documents");

            // Payment amount display can be different. When cents are .00, then only whole dollar amount to be shown, otherwise include cents.
            var collectionAmount = updatedPolicyData.NextPendingInstallment().Amount.Total;
            var expectedAmount = String.Format("${0:0.00}", collectionAmount);
            if (collectionAmount % 1 == 0)
            {
                expectedAmount = String.Format("${0:0}", collectionAmount);
            }
            Reporting.AreEqual($"{expectedAmount} due by {renewalDateString}", BPayWarning, "BPay amount and due date is displayed");
            Reporting.IsFalse(updatedPolicyData.IsPaidInFull, "policy API has NOT recorded the policy as paid");
            Reporting.AreEqual(collectionAmount, endorseCaravan.PremiumChangesAfterEndorsement.Total, "premium payment ammount matches proposed amount");

            Reporting.AreEqual(Constants.BPay.Name, BPayName(endorseCaravan.PolicyNumber), "BPay name is displayed");
            Reporting.AreEqual(Constants.BPay.SubTitle, BPaySubtitle(endorseCaravan.PolicyNumber), "BPay information is displayed");
            Reporting.IsTrue(IsBPayLogoDisplayed, "BPay icon is displayed");
            Reporting.AreEqual($"{Constants.BPay.BillerCode} {BPAY_BILLER_CODE}", BPayCode(endorseCaravan.PolicyNumber), "BPay biller code is displayed");
            Reporting.AreEqual($"{Constants.BPay.Reference} {endorseCaravan.PolicyNumber.Substring(3).PadLeft(9,'0')}", BPayReference(endorseCaravan.PolicyNumber),
                "BPay reference uses policy number without first three letters and is padded with leading 0s to make a 9 digit number");
            Reporting.AreEqual($"{Constants.PolicyNumber} {endorseCaravan.PolicyNumber}", BPayPolicyNumber, "full policy number is shown");
        }


        /// <summary>
        /// Confirm the content of Pay Later confirmation page
        /// </summary>
        public void VerifyPayLater(EndorseCaravan endorseCaravan)
        {
            Reporting.Log($"Pay Later", _browser.Driver.TakeSnapshot());
            var updatedPolicyData = DataHelper.GetPolicyDetails(endorseCaravan.PolicyNumber);
            var renewalDateString = endorseCaravan.OriginalPolicyData.PolicyStartDate.ToString(DateTimeTextFormat.ddMMyyyy);

            Reporting.AreEqual(Constants.PayLater.Title, PayLaterTitle, "Pay Later title is displayed");

            Reporting.AreEqual($"{Constants.PayLater.BlurbPart1a}{renewalDateString}{Constants.PayLater.BlurbPart1b}",
                PayLaterParagraphOne, "message for pay by date is correct.");
            Reporting.AreEqual($"{Constants.PayLater.BlurbPart2}", PayLaterParagraphTwo, "message about emailing renewal documents");

            // Payment amount display can be different. When cents are .00, then only whole dollar amount to be shown, otherwise include cents.
            var outstandingAmount = updatedPolicyData.NextPendingInstallment().Amount.Total;
            var expectedAmount = String.Format("${0:0.00}", outstandingAmount);
            if (outstandingAmount % 1 == 0)
            {
                expectedAmount = String.Format("${0:0}", outstandingAmount);
            }
            Reporting.IsFalse(updatedPolicyData.IsPaidInFull, "policy API has NOT recorded the policy as paid");
            Reporting.AreEqual(outstandingAmount, endorseCaravan.PremiumChangesAfterEndorsement.Total, "premium payment ammount matches proposed amount");

            Reporting.AreEqual($"{expectedAmount} due by {renewalDateString}", PayLaterKeyInformation, "Pay later amount and due date is displayed");

            Reporting.AreEqual($"{Constants.PolicyNumber} {endorseCaravan.PolicyNumber}", PayLaterPolicyNumber, "full policy number is shown");
        }

        /// <summary>
        /// Confirm the content when a direct debit is chosen by the member. This can be by credit card
        /// or bank account. The frequency can be monthly or annually.
        /// </summary>
        public void VerifyDirectDebit(EndorseCaravan endorseCaravan) 
        {
            Reporting.Log($"Direct Debit", _browser.Driver.TakeSnapshot());
            var updatedPolicyData = DataHelper.GetPolicyDetails(endorseCaravan.PolicyNumber);

            Reporting.AreEqual($"{Constants.YoureAllSet}{endorseCaravan.ActivePolicyHolder.FirstName}!", DirectDebitTitle,
                "page confirmation title for direct debit");

            if (endorseCaravan.SparkExpandedPayment.IsAnnual)
            {
                Reporting.AreEqual(Constants.DirectDebit.ConfirmationAnnualDirectDebit, DirectDebitParagraph,"message for paying annually and emailing documents is shown");
                Reporting.AreEqual(Constants.AnnualPayment, DirectDebitType,"shows annual payment for title");
                Reporting.AreEqual(Constants.DirectDebit.DebitDateInformation, DirectDebitDebitDateInfo, "explanation for debit date");
            }
            else
            {
                Reporting.AreEqual(Constants.DirectDebit.ConfirmationMonthlyDirectDebit, DirectDebitParagraph, "message for paying monthly and emailing documents is shown");
                Reporting.AreEqual(Constants.MonthlyPayment, DirectDebitType, "shows monthly payment for title");
                var collectionDateString = updatedPolicyData.NextPendingInstallment().CollectionDate.ToString(DateTimeTextFormat.ddMMyyyy);
                Reporting.AreEqual($"{Constants.DirectDebit.NextPaymentDue}{collectionDateString}", DirectDebitPaymentDue, "explanation on next payment date");
            }

            var expectedAmount = updatedPolicyData.NextPendingInstallment().Amount.Total;
            VerifyAmountWithTruncationWhenWholeDollar(updatedPolicyData.NextPendingInstallment().Amount.Total, DirectDebitAmount,
                "payment amount is displayed correctly");
            Reporting.AreEqual(expectedAmount, endorseCaravan.PremiumChangesAfterEndorsement.Total, "premium payment ammount matches proposed amount");

            var methodDescription = (endorseCaravan.SparkExpandedPayment.CreditCardDetails != null) ? CREDIT_CARD : BANK_ACCOUNT ;
            Reporting.AreEqual($"{Constants.Payment} {methodDescription}", DirectDebitMethod, "payment type is displayed correctly");

            var renewalDateString = updatedPolicyData.PolicyStartDate.ToString(DateTimeTextFormat.ddMMyyyy);
            Reporting.AreEqual($"{Constants.PolicyRenewsOn} {renewalDateString}", DirectDebitRenewalDate, "policy renewal date is correct.");

            Reporting.AreEqual($"{Constants.PolicyNumber} {endorseCaravan.PolicyNumber}", DirectDebitPolicyNumber, "full policy number is shown");
        }

        /// <summary>
        /// Verification for Confirmation page for Monthly Installment
        /// </summary>
        public void VerifyConfirmationForMonthlyInstallment(EndorseCaravan endorseCaravan)
        {
            var payment = endorseCaravan.PayMethod.IsPaymentByBankAccount ?
                            Constants.Monthlypayment.IncreasePaymentMethodBankAccount : Constants.Monthlypayment.IncreasePaymentMethodCC;

            Reporting.Log($"Increase Premium Payment", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual($"{Constants.YoureAllSet}{endorseCaravan.ActivePolicyHolder.FirstName}!", AllSet, ignoreCase:true, 
                "expected page confirmation title for endorsement against value displayed on page");
            Reporting.AreEqual(Constants.Monthlypayment.Paragraph, Paragraph, "expected 'you've updated your policy' paragraph against value displayed on page");

            Reporting.AreEqual(Constants.Monthlypayment.MonthlyPaymentReceivedTitle, CardTitle, "expected Title for the Monthly Payment card against value displayed on page");
            Reporting.AreEqual($"${DataHelper.GetCorrectedPremium(endorseCaravan.PremiumChangesAfterEndorsement.TotalPremiumMonthly)}", InstallmentAmount,
                "expected new premium against value displayed on page");            

            Reporting.AreEqual(payment, InstallmentPaymentMethod, ignoreCase: true, "expected payment method against value displayed on page");
            Reporting.AreEqual($"Start date for this change: {endorseCaravan.StartDate.ToString(DateTimeTextFormat.ddMMyyyy)}", ChangeStartDate, 
                "expected start date value displayed on page");
            Reporting.AreEqual($"Next payment date: {endorseCaravan.OriginalPolicyData.NextPayableInstallment.CollectionDate.ToString(DateTimeTextFormat.ddMMyyyy)}", NextPaymentDate,
                "expected next payment due date, against value displayed on page");

            Reporting.AreEqual($"Policy number: {endorseCaravan.PolicyNumber}", PolicyNumberEndorsed, "expected policy number against value displayed on page");

            Reporting.IsTrue(IsControlDisplayed(XPath.MonthlyInstallment.BackToMyPolicy), "'Back to my policies' button is displayed");
        }

        /// <summary>
        /// Verification for increase premium Confirmation page for Annual Installment
        /// </summary>
        public void VerifyIncreasePremiumConfirmationForAnnualInstallment(EndorseCaravan endorseCar)
        {
            Reporting.Log($"Increase Premium Payment", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual($"{Constants.YoureAllSet}{endorseCar.ActivePolicyHolder.FirstName}!", IncreasePremiumAllSet,
                "expected page confirmation title for endorsement against value displayed on page");
            Reporting.AreEqual(Constants.IncreasePremiumInstallments.IncreasePremiumParagraph, IncreasePremiumParagraph,
                "expected paragraph against value displayed on page");
            Reporting.AreEqual(Constants.IncreasePremiumInstallments.PaymentReceivedTitle, PaymentReceivedTitle,
                "expected card title against value displayed on page");
            Reporting.AreEqual($"${DataHelper.GetCorrectedPremium(endorseCar.PremiumChangesAfterEndorsement.Total)}", IncreaseAmount,
                "expected new premium against value displayed on page");
            var payment = endorseCar.SparkExpandedPayment.PaymentOption.Equals(PaymentOptionsSpark.DirectDebit) ?
                Constants.IncreasePremiumInstallments.IncreasePaymentMethodBankAccount : Constants.IncreasePremiumInstallments.IncreasePaymentMethodCC;
            Reporting.AreEqual(payment, IncreasePaymentMethod, ignoreCase: true, "expected payement method against value displayed on page");
            Reporting.AreEqual($"Start date for this change: {endorseCar.StartDate.ToString(DateTimeTextFormat.ddMMyyyy)}", EndorseStartDate,
                "expected start date value displayed on page");
            Reporting.AreEqual($"Policy number: {endorseCar.PolicyNumber}", EndorsedPolicyNumber, "expected policy number against value displayed on page");
            Reporting.IsTrue(IsControlDisplayed(XPath.IncreasePremiumCreditCard.BackToMyPoliciesButton), "'Back to my policies' button is displayed");
        }

        /// <summary>
        /// Verification for increase premium Confirmation page for Annual Cash
        /// </summary>
        public void VerifyIncreasePremiumConfirmationForAnnualCashPayment(EndorseCaravan endorseCar)
        {
            Reporting.Log($"Increase Premium Payment", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual($"{Constants.YoureAllSet}{endorseCar.ActivePolicyHolder.FirstName}!", IncreasePremiumAllSet, ignoreCase: true,
                "expected page confirmation title for endorsement against value displayed on page");
            Reporting.AreEqual(Constants.IncreasePremiumCreditCard.IncreasePremiumParagraph, IncreasePremiumParagraph,
                "expected paragraph against value displayed on page");
            Reporting.AreEqual(Constants.IncreasePremiumCreditCard.PaymentReceivedTitle, PaymentReceivedTitle,
                "expected card title against value displayed on page");
            Reporting.AreEqual($"${DataHelper.GetCorrectedPremium(endorseCar.PremiumChangesAfterEndorsement.Total)}", IncreaseAmount,
                "expected amount increase premium against value displayed on page");
            Reporting.AreEqual(Constants.IncreasePremiumCreditCard.IncreasePaymentMethod, IncreasePaymentMethod,
                "expected payement method against value displayed on page");
            Reporting.AreEqual($"Start date for this change: {endorseCar.StartDate.ToString(DateTimeTextFormat.ddMMyyyy)}", EndorseStartDate,
                "expected start date value displayed on page");
            Reporting.AreEqual($"Policy number: {endorseCar.PolicyNumber}", EndorsedPolicyNumber, "expected policy number against value displayed on page");
            Reporting.AreEqual(Constants.IncreasePremiumCreditCard.ReceiptFooter, ReceiptFooter, "expected receipt footer against value displayed on page");
            Reporting.IsTrue(IsControlDisplayed(XPath.IncreasePremiumCreditCard.BackToMyPoliciesButton), "'Back to my policies' button is displayed");
        }

        /// <summary>
        /// Verification for refund Confirmation page for Known/Unknown CC and Bank account
        /// </summary>
        /// <param name="refundDestination">Set this for the expected refund destination</param>
        public void VerifyCreditCardRefundConfirmation(EndorseCaravan endorseCar, SparkCommonConstants.RefundToSource refundDestination)
        {
            Reporting.Log($"Refund CreditCard and BankAccount", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual($"{Constants.YoureAllSet}{endorseCar.ActivePolicyHolder.FirstName}!", RefundAllSet,
                "expected confirmation page title for credit card refund endorsement against value displayed on page");
            Reporting.AreEqual(Constants.CreditCardRefund.CreditCardRefundParagraph, RefundParagraph,
                "expected refund paragraph against value displayed on page");
            Reporting.AreEqual(Constants.CreditCardRefund.CreditCardRefundTitle, RefundCardTitle,
                "expected refund card title against value displayed on page");
            Reporting.AreEqual($"${DataHelper.GetCorrectedPremium(endorseCar.PremiumChangesAfterEndorsement.Total)}", Amount,
                "expected refund amount difference against value displayed on page");
            if (refundDestination.Equals(SparkCommonConstants.RefundToSource.RefundToKnownCreditCard) || refundDestination.Equals(SparkCommonConstants.RefundToSource.RefundToUnknownCreditCard))
            {
                Reporting.AreEqual(Constants.CreditCardRefund.CreditCardRefundDestination, RefundDestination,
                    "expected refund destination against value displayed on page");
            }

            if (refundDestination.Equals(SparkCommonConstants.RefundToSource.RefundToBankAccount))
            {
                Reporting.AreEqual(Constants.CreditCardRefund.BankRefundDestination, RefundDestination,
                    "expected refund destination against value displayed on page");
            }
            Reporting.AreEqual(Constants.CreditCardRefund.ProcessingTime, ProcessingTime, "expected processing time against value displayed on page");
            Reporting.AreEqual($"Start date for this change: {endorseCar.StartDate.ToString(DateTimeTextFormat.ddMMyyyy)}", StartDate,
                "expected start date value displayed on page");
            Reporting.AreEqual($"Policy number: {endorseCar.PolicyNumber}", PolicyNumber, "expected policy number against value displayed on page");
            Reporting.IsTrue(IsControlDisplayed(XPath.RefundCreditCard.BackToMyPoliciesButton), "'Back to my policies' button is displayed");
        }

        public void VerifyNoChangePremiumConfirmation(EndorseCaravan endorseCar)
        {
            Reporting.Log($"No Change in Premium", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual($"{Constants.YoureAllSet}{endorseCar.ActivePolicyHolder.FirstName}!", NoPremiumChangePolicyTitle,
              "expected page confirmation title for No premium change endorsement against value displayed on page");
            Reporting.AreEqual($"{Constants.NoChangePremium.ParagraphOne}{endorseCar.StartDate.ToString(DateTimeTextFormat.ddMMyyyy)}. {Constants.NoChangePremium.ParagraphTwo}", NoPremiumChangeParagraph,
             " expected confirmation text for No premium change endorsement against value displayed on page");
            Reporting.AreEqual($"Policy number: {endorseCar.PolicyNumber}", NoPremiumChangePolicyNumber,
             "expected policy number against value displayed on page");
        }
    }
}
