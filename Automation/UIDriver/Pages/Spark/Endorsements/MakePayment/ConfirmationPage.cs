using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Data;
using System.Text.RegularExpressions;

namespace UIDriver.Pages.Spark.Endorsements.MakePayment
{
    public class ConfirmationPage : SparkPaymentPage
    {
        #region CONSTANTS
        private class Constants
        {
            public static readonly string YoureAllSet           = "You're all set, ";
            public static readonly string YoureAllPaid          = "You're all paid up!";

            public static readonly string CardHeading           = "Payment";
            public static readonly string PaymentSource         = "Payment: Credit card";

            public static readonly string FuturePaymentHeading  = "Future payments";
            public static readonly string FuturePaymentContent  = "If you want to pay for future instalments using this payment card, you can update how you pay.";

            public static readonly string PolicyNumber          = "Policy number:";
            public static readonly string ReceiptNumber         = "Receipt number:";

            public static readonly string YourEmail             = "Your email is ";
            public static readonly string UpdateEmail           = "If this isn't your email, you can update it.";

            public static class Stepper
            {
                public static readonly string Payment           = "Payment";
                public static readonly string Confirmation      = "Confirmation";
            }

            public static class FailedPayment
            {
                public static readonly string Title                 = "Please call us to pay";
                public static readonly string SubTitle              = "We couldn't process your payment.";
                public static readonly string PolicyNumberDisplay   = "Policy number: ";
            }

            public class Button
            {
                public static readonly string BackToMyPolicy        = "Back to my policy";
                public static readonly string SendEmail             = "Send email receipt";
                public static readonly string SentEmail             = "Email sent";
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public static class AnnualCash
            {
                public static readonly string YoureAllSet                   = "id('confirmation-successful-annual-cash-title')";
                public static readonly string YoureAllPaid                  = "id('confirmation-successful-annual-cash-subtitle')";

                public static readonly string CardHeading                   = "id('confirmation-successful-annual-cash-payment-amount-card-content-heading-text')";
                public static readonly string PaymentSource                 = "id('confirmation-successful-annual-cash-payment-amount-card-content-subtext')";
                public static readonly string AmountDue                     = "id('confirmation-successful-annual-cash-payment-amount-card-content-heading-amount')";

                public static readonly string PolicyNumber                  = "id('confirmation-successful-annual-cash-policy-number')";
                public static readonly string ReceiptNumber                 = "id('confirmation-successful-annual-cash-receipt-number')";

                public static readonly string YourEmail                     = "id('confirmation-successful-annual-cash-email-line-one')";
                public static readonly string UpdateEmail                   = "id('confirmation-successful-annual-cash-email-line-two')";
            }
            
            public static class DirectDebit
            {
                public static readonly string YoureAllSet                   = "id('confirmation-successful-direct-debit-title')";
                public static readonly string YoureAllPaid                  = "id('confirmation-successful-direct-debit-subtitle')";

                public static readonly string CardHeading                   = "id('confirmation-successful-direct-debit-payment-amount-card-content-heading-text')";
                public static readonly string PaymentSource                 = "id('confirmation-successful-direct-debit-payment-amount-card-content-subtext')";
                public static readonly string AmountDue                     = "id('confirmation-successful-direct-debit-payment-amount-card-content-heading-amount')";

                public static readonly string FuturePaymentHeading          = "id('future-payments-card-heading')";
                public static readonly string FuturePaymentContent          = "id('future-payments-card-content')";

                public static readonly string PolicyNumber                  = "id('confirmation-successful-direct-debit-policy-number')";
                public static readonly string ReceiptNumber                 = "id('confirmation-successful-direct-debit-receipt-number')";
            }

            public static class Stepper
            {
                public static readonly string Payment                       = "id('payment-step')";
                public static readonly string Confirmation                  = "id('confirmation-step')";
            }
            public static class FailedPayment
            {
                public static readonly string Title                         = "id('confirmation-annual-cash-realtime-payment-error-title')";
                public static readonly string SubTitle                      = "id('confirmation-annual-cash-realtime-payment-error-subtitle')";
                public static readonly string PolicyNumberDisplay           = "id('confirmation-annual-cash-realtime-payment-error-policy-number')";
            }

            public class Button
            {
                public static readonly string BackToMyPolicy                = "//button[text()='" + Constants.Button.BackToMyPolicy + "']";
                public static readonly string SendEmailReceipt              = "id('confirmation-successful-annual-cash-send-email-button')";
                public static readonly string PhoneButton                   = "id('confirmation-annual-cash-realtime-payment-error-phone-button')";
            }
        }
        #endregion

        public ConfirmationPage(Browser browser) : base(browser) { }

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
        /// Checking the name & active/inactive stepper on the current page
        /// </summary>
        public void VerifyStepperNameAndState()
        {
            Reporting.AreEqual(Constants.Stepper.Payment, GetInnerText(XPath.Stepper.Payment), $"the expected stepper name '{Constants.Stepper.Payment}' against the value displayed on the page.");
            Reporting.AreEqual(Constants.Stepper.Confirmation, GetInnerText(XPath.Stepper.Confirmation), $"the expected stepper name '{Constants.Stepper.Confirmation}' against the value displayed on the page.");
            Reporting.IsFalse(bool.Parse(GetElement(XPath.Stepper.Payment).GetAttribute("aria-selected")), $"the stepper {Constants.Stepper.Payment} is disabled on the page.");
            Reporting.IsTrue(bool.Parse(GetElement(XPath.Stepper.Confirmation).GetAttribute("aria-selected")), $"the stepper {Constants.Stepper.Confirmation} is enabled on the page.");
        }

        /// <summary>
        /// When an Annual Direct Debit payment is done by credit card. Ensure the correct details are shown.
        /// </summary>
        public void VerifyPageDirectDebit(EndorsementBase testData)
        {
            Reporting.Log($"Direct Debit confirmation page when policy paid by one off credit card payment", _browser.Driver.TakeSnapshot());

            var updatedPolicyData = DataHelper.GetPolicyDetails(testData.PolicyNumber);

            Reporting.AreEqual($"{Constants.YoureAllSet}{testData.ActivePolicyHolder.FirstName}!", GetInnerText(XPath.DirectDebit.YoureAllSet),
                "page confirmation title for direct debit");
            Reporting.AreEqual(Constants.YoureAllPaid, GetInnerText(XPath.DirectDebit.YoureAllPaid),
                "page confirmation sub title for direct debit");

            Reporting.AreEqual(Constants.CardHeading, GetInnerText(XPath.DirectDebit.CardHeading), "policy paid confirmation is present");

            Reporting.AreEqual(Constants.PaymentSource, GetInnerText(XPath.DirectDebit.PaymentSource), "payment source is displayed correctly");

            Reporting.AreEqual(PaymentPage.GetAmountDue(testData), DataHelper.StripMoneyNotations(GetInnerText(XPath.DirectDebit.AmountDue)), $"the Amount due '{PaymentPage.GetAmountDue(testData)}' is displayed");

            Reporting.AreEqual($"{Constants.PolicyNumber} {testData.PolicyNumber}", GetInnerText(XPath.DirectDebit.PolicyNumber), "policy number is shown");

            //Future payment section
            Reporting.AreEqual(Constants.FuturePaymentHeading, GetInnerText(XPath.DirectDebit.FuturePaymentHeading), "future payment heading is present");
            Reporting.AreEqual(Constants.FuturePaymentContent, GetInnerText(XPath.DirectDebit.FuturePaymentContent), "future payment content is present");

            // policynumber and receipt
            var expectedTextRegex = new Regex($"^({Constants.ReceiptNumber}\\s*)(\\d+)$");
            var match = expectedTextRegex.Match(GetInnerText(XPath.DirectDebit.ReceiptNumber));
            if (match.Success && match.Groups.Count == 3)
            {
                Payment.VerifyWestpacQuickStreamDetailsAreCorrect<EndorsementBase>(cardDetails: testData.SparkExpandedPayment.CreditCardDetails,
                                                     policyNumber: testData.PolicyNumber,
                                                     expectedPrice: updatedPolicyData.AnnualPremium.Total,
                                                     expectedReceiptNumber: match.Groups[2].Value);
            }
            else
            {
                Reporting.Error("Problem with displaying the Westpac receipt");
            }
        }

        /// <summary>
        /// When an Annual Cash payment is done by credit card. Ensure the correct details are shown.
        /// </summary>
        public void VerifyPageAnnualCash(EndorsementBase testData)
        {
            Reporting.Log($"Annual Cash confirmation page when policy paid by one off credit card payment", _browser.Driver.TakeSnapshot());

            var updatedPolicyData = DataHelper.GetPolicyDetails(testData.PolicyNumber);
            var memberDetails = DataHelper.GetPersonFromMemberCentralByContactId(testData.OriginalPolicyData.Policyholder.Id.ToString());

            Reporting.AreEqual($"{Constants.YoureAllSet}{testData.ActivePolicyHolder.FirstName}!", GetInnerText(XPath.AnnualCash.YoureAllSet),
                "page confirmation title for annual cash");
            Reporting.AreEqual($"{Constants.YoureAllPaid}", GetInnerText(XPath.AnnualCash.YoureAllPaid),
                "page confirmation sub title for annual cash");

            Reporting.AreEqual(Constants.CardHeading, GetInnerText(XPath.AnnualCash.CardHeading), "policy paid confirmation is present");

            Reporting.AreEqual(Constants.PaymentSource, GetInnerText(XPath.AnnualCash.PaymentSource), "payment source is displayed correctly");

            Reporting.AreEqual(PaymentPage.GetAmountDue(testData), DataHelper.StripMoneyNotations(GetInnerText(XPath.AnnualCash.AmountDue)), $"the Amount due '{PaymentPage.GetAmountDue(testData)}' is displayed");

            Reporting.AreEqual($"{Constants.PolicyNumber} {testData.PolicyNumber}", GetInnerText(XPath.AnnualCash.PolicyNumber), "policy number is shown");

            var expectedTextRegex = new Regex($"^({Constants.ReceiptNumber}\\s*)(\\d+)$");
            var match = expectedTextRegex.Match(GetInnerText(XPath.AnnualCash.ReceiptNumber));
            if (match.Success && match.Groups.Count == 3)
            {
                Payment.VerifyWestpacQuickStreamDetailsAreCorrect<EndorsementBase>(cardDetails: testData.SparkExpandedPayment.CreditCardDetails,
                                                     policyNumber: testData.PolicyNumber,
                                                     expectedPrice: updatedPolicyData.AnnualPremium.Total,
                                                     expectedReceiptNumber: match.Groups[2].Value);
            }
            else
            {
                Reporting.Error("Problem with displaying the Westpac receipt");
            }

            if (memberDetails.PersonalEmailAddress != null)
            {
                Reporting.AreEqual(Constants.Button.SendEmail, GetInnerText(XPath.Button.SendEmailReceipt), "Send email receipt button is displayed");

                ClickSendEmailReceiptButton();
                Reporting.Log($"Capturing button text change aftr clicking 'Send email receipt' button", _browser.Driver.TakeSnapshot());
                Reporting.AreEqual(Constants.Button.SentEmail, GetInnerText(XPath.Button.SendEmailReceipt), "'Send email receipt' button text is changed to 'Email sent'");
            }
            else
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.Button.SendEmailReceipt), "Send email receipt button is NOT displayed");
            }
        }

        /// <summary>
        /// Attempt to click the "Send email receipt" button.
        /// </summary>
        /// <exception cref="ReadOnlyException">Thrown if button is present but disabled.</exception>
        public void ClickSendEmailReceiptButton()
        {
            using (var spinner = new SparkSpinner(_browser))
            {
                if (IsControlEnabled(XPath.Button.SendEmailReceipt))
                {
                    ClickControl(XPath.Button.SendEmailReceipt);
                    Reporting.Log("Clicked on 'Send email receipt' button");
                }
                else
                { throw new ReadOnlyException("Button is currently disabled and not clickable. Check input values."); }
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Checking static text, default buttons and policy number
        /// </summary>
        /// <param name="policyNumber"></param>
        public void VerifyFailedPaymentConfirmation(string policyNumber)
        {
            Reporting.Log($"Confirmation page for Failed payment", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(Constants.FailedPayment.Title, GetInnerText(XPath.FailedPayment.Title),"page confirmation title for failed payment");
            Reporting.AreEqual(Constants.FailedPayment.SubTitle, GetInnerText(XPath.FailedPayment.SubTitle), "page confirmation subtitle for failed payment");
            Reporting.AreEqual($"{Constants.FailedPayment.PolicyNumberDisplay}{policyNumber}", GetInnerText(XPath.FailedPayment.PolicyNumberDisplay), "policy number is shown");
            Reporting.IsTrue(IsControlDisplayed( XPath.Button.BackToMyPolicy), "Back to policy button is present");
            Reporting.IsTrue(IsControlDisplayed(XPath.Button.PhoneButton), "phone number button is present");
        }
    }
}
