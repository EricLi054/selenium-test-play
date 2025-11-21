using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Text.RegularExpressions;
using UIDriver.Pages.B2C;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace UIDriver.Pages.PCM
{
    abstract public class BaseEndorsementPage : BasePage
    {
        #region XPATHS
        // Payment details accordion
        private const string XP_PAYMENT_AMOUNT                = "id('PaymentAmount_Answer')";
        private const string XP_SUBMIT_PAYMENT_BUTTON         = "id('accordion_1_submit-action')";
        private const string XP_ANNUAL_CC_AUTHORISATION       = "//div[@id='PayNowContainer']/div/div[contains(@class, 'payment-information')]";

        // Confirmation accordion
        private const string XP_CONFIRMATION_ACCORDION_ACTIVE = "//div[@data-accordion-id='ConfirmationViewModel' and contains(@class,'opened')]";
        private const string XP_CONFIRMATION_TEXT             = "//div[@data-accordion-panel-id='ConfirmationViewModel' and contains(@class,'opened')]//div[@class='pcm-content-text']";
        private const string XP_RETURN_TO_POLICY_BUTTON       = "//div[contains(@class,'confirm-continue-btn')]/a";
        private const string XP_CONFIRMATION_SUMMARY          = "//div[@class='RenewalsConfirmationSummaryTable']";
        private const string XP_POLICY_NUM                    = XP_CONFIRMATION_SUMMARY + "//div[@id='PolicyNumber_Answer']";
        private const string XP_PAYMENT_FREQ                  = XP_CONFIRMATION_SUMMARY + "//div[@id='PaymentFrequency_Answer']";
        private const string XP_PAYMENT_AMNT                  = XP_CONFIRMATION_SUMMARY + "//div[@id='PaymentAmount_Answer']";
        private const string XP_RECEIPT_NUMBER                = XP_CONFIRMATION_SUMMARY + "//div[@id='ReceiptNumber_Answer']";
        #endregion

        #region Settable properties and controls
        public bool IsPayNowButtonEnabled => IsControlEnabled(XP_SUBMIT_PAYMENT_BUTTON);

        /// <summary>
        /// Gets the whole text of the Authorisation message for annual cash payments
        /// </summary>
        public string AnnualCreditCardAuthorisationText => GetInnerText(XP_ANNUAL_CC_AUTHORISATION);

        /// <summary>
        /// Returns the Policy Number from Confimation accordion
        /// </summary>
        public string GetPolicyNumber()
        {
            _driver.WaitForElementToBeVisible(By.XPath(XP_POLICY_NUM), WaitTimes.T30SEC);
            return GetInnerText(XP_POLICY_NUM);
        }

        /// <summary>
        /// Returns the Payment Frequency from Confimation accordion
        /// </summary>
        public PaymentFrequency PaymentFrequency
        {
            get
            {
                var element = GetElement(XP_PAYMENT_FREQ);
                if (element.Text.Equals(PaymentFrequency.Annual.GetDescription()))
                { return PaymentFrequency.Annual; }

                // If annual is not selected, then we expect monthly to have been selected.
                if (!element.Text.Equals(PaymentFrequency.Monthly.GetDescription()))
                { Reporting.Error($"String for payment frequency ({element.Text}) is an unrecognised value."); }
                
                return PaymentFrequency.Monthly;
            }
        }

        /// <summary>
        /// Returns the Amount from Confimation accordion
        /// </summary>
        public decimal Amount
        {
            get
            {
                var element = GetElement(XP_PAYMENT_AMNT);
                var price = decimal.Parse(element.Text.StripMoneyNotations());
                return price;
            }
        }

        /// <summary>
        /// Returns the Receipt Number from Confimation accordion
        /// </summary>
        public string ReceiptNumber => GetInnerText(XP_RECEIPT_NUMBER);

        /// <summary>
        /// Gets the Amount from Payment details/Confimation accordion
        /// </summary>
        public decimal GetAmount(string xPath) => decimal.Parse(GetElement(xPath).Text.StripMoneyNotations());
        #endregion

        public BaseEndorsementPage(Browser browser) : base(browser)
        {
        }

        /// <summary>
        /// Verifies that the confirmation message is observed and then returns
        /// to portfolio summary view.
        /// </summary>
        /// <returns>Confirmation Text</returns>
        public string WaitForConfirmationAndReturnToPolicyView()
        {
            _driver.WaitForElementToBeVisible(By.XPath(XP_CONFIRMATION_ACCORDION_ACTIVE), WaitTimes.T30SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_RETURN_TO_POLICY_BUTTON), WaitTimes.T5SEC);

            string confirmationText = GetInnerText(XP_CONFIRMATION_TEXT).StripLineFeedAndCarriageReturns();

            using (var spinner = new RACSpinner(_browser))
            {
                ClickControl(XP_RETURN_TO_POLICY_BUTTON);
                spinner.WaitForSpinnerToFinish();
            }
                
            return confirmationText;
        }

        public decimal VerifyUIElementsInPaymentDetailsAccordionAreCorrect(decimal expectedAmount)
        {
            var actualAmount = GetAmount(XP_PAYMENT_AMOUNT);
            Reporting.AreEqual(expectedAmount, actualAmount, "Amount is as expected");
            Reporting.IsTrue(IsPayNowButtonEnabled, "Pay now button".IsEnabled());
            VerifyPaymentAuthorisationMessage();

            return actualAmount;
        }

        public string VerifyConfirmationAccordionDetails(Payment paymentDetails, decimal expectedAmount, string expectedPolicyNumber, bool isRenewalScenario = false)
        {
            if (isRenewalScenario)
            {
                Reporting.AreEqual(expectedPolicyNumber, GetPolicyNumber(), "Policy Number is as expected.");
                Reporting.AreEqual(paymentDetails.PaymentFrequency, PaymentFrequency, "Payment Frequency is as expected.");
            }

            Reporting.AreEqual(expectedAmount, Amount, "Amount is as expected.");

            Regex receiptNumberRegex = new Regex(@"(?=1)\d{10}");
            var actualReceiptNumber  = ReceiptNumber;
            Match matchReceiptNumber = receiptNumberRegex.Match(actualReceiptNumber);
            Reporting.IsTrue(matchReceiptNumber.Success, "Receipt number generated");
            Reporting.Log($"Receipt number: {actualReceiptNumber}", _browser.Driver.TakeSnapshot());

            return actualReceiptNumber;
        }

        private void VerifyPaymentAuthorisationMessage()
        {
            Regex authorisationMessageRegex = new Regex(FixedTextRegex.PAYMENT_ANNUAL_CREDIT_CARD_AUTHORISATION_PCM_REGEX);
            var authorisationMessageText    = AnnualCreditCardAuthorisationText;
            Match match                     = authorisationMessageRegex.Match(authorisationMessageText);

            if (!match.Success)
            { Reporting.Error($"Authorisation message is incorrect. Actual: {authorisationMessageText}"); }
        }
    }
}