using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace Rac.TestAutomation.Common
{
    abstract public class SparkPaymentPage : SparkBasePage
    {
        #region XPATHS
        public static class XPathPayment
        {
            public static class Detail
            {
                public static readonly string PolicyType                    = "//div[@id='racwaContainer']//h3";
                public static readonly string FrequencyLabel                = FORM + "//label[text()='Payment frequency']";
                public static readonly string MethodLabel                   = "//label[@data-testid='paymentMethodSelection']";
                public static readonly string AuthorisationCheckbox         = "//span[@data-test-id='payment-disclaimer-input']";
                public static readonly string AuthorisationText             = "//div[@data-testid='accountAuthorisationCard']";
                public static readonly string AuthorisationAcknowledgement  = "//div[@data-testid='payment-disclaimer-label']";
                public static readonly string ExistingAccountOption1        = "id('payment-account-option-0')";
                public static readonly string ExistingAccountOption2        = "id('payment-account-option-1')";
            }
            public static class Bank
            {
                public static readonly string AddBankAccount                = "id('select-payment-method-add-payment-method')";
                public static readonly string Bsb                           = "id('bsb')";
                public static readonly string BsbDetails                    = "id('bsb-message')";
                public static readonly string BsbDetailsChecking            = "//p[@id='bsb-message' and contains(text(),'Checking BSB')]";
                public static readonly string AccountNumber                 = "//input[@name='accountNumber']";
                public static readonly string AccountName                   = "id('accountName')";
            }
            public static class CreditCard
            {
                public static readonly string PaymentIframe                 = "//iframe[@id='trustedFrame-creditCard']";
                public static readonly string NameOnCard                    = "id('cardholderName')";
                public static readonly string CardNumber                    = "id('cardNumber')";
                public static readonly string ExpiryMonth                   = "id('expiryDateMonth')";
                public static readonly string MonthOptions                  = "id('expiryDateMonth')/option";
                public static readonly string ExpiryYear                    = "id('expiryDateYear')";
                public static readonly string YearOptions                   = "id('expiryDateYear')/option";
                public static readonly string Cvc                           = "id('cvn')";
            }
            public static class Button
            {
                public static readonly string Submit                        = "//button[@data-testid='submit' or @id='submit-button' or @id='submit']";
                public static readonly string MethodCard                    = "//button[text()='Card' or @id='input-payment-method-toggle-card']";
                public static readonly string MethodBankAccount             = "//button[text()='Bank account' or @id='input-payment-method-toggle-bank-account']";
            }
        }
        
        #endregion XPATHS

        private const string PAYMENT_METHOD_BANK_AC = "Bank Account";
        private const string PAYMENT_METHOD_CREDIT_CARD = "Credit Card";

        public SparkPaymentPage(Browser browser) : base(browser)
        {
        }

        #region Payment page

        public string PolicyType => GetInnerText(XPathPayment.Detail.PolicyType);

        public string BSBDetails => GetInnerText(XPathPayment.Bank.BsbDetails);

        public decimal PremiumAnnual => DataHelper.GetMonetaryValueFromString(GetInnerText(XP_PAYMENT_AMOUNT_PAYMENT));

        public decimal PremiumMonthly => DataHelper.GetMonetaryValueFromString(GetInnerText(XP_PAYMENT_AMOUNT_PAYMENT));

        private string PaymentMethod
        {
            get
            {
                string paymethod = string.Empty;

                if (IsChecked(XPathPayment.Button.MethodCard))
                {
                    paymethod = PAYMENT_METHOD_CREDIT_CARD;
                }
                else if (IsChecked(XPathPayment.Button.MethodBankAccount))
                {
                    paymethod = PAYMENT_METHOD_BANK_AC;
                }

                return paymethod;
            }

            set
            {
                var buttonOption = value.Equals(PAYMENT_METHOD_CREDIT_CARD) ? XPathPayment.Button.MethodCard : XPathPayment.Button.MethodBankAccount;
                //Clicking the PaymentMethod if it not already selected. 
                if(GetElement(buttonOption).GetAttribute("aria-pressed") == "false")
                {
                    ClickControl(buttonOption);
                }

            }
        }

        private string BSB
        {
            get => GetValue(XPathPayment.Bank.Bsb);

            set => WaitForTextFieldAndEnterText(XPathPayment.Bank.Bsb, value, false);
        }

        private string AccountNumber
        {
            get => GetValue(XPathPayment.Bank.AccountNumber);

            set => WaitForTextFieldAndEnterText(XPathPayment.Bank.AccountNumber, value, false);
        }

        private string AccountName
        {
            get => GetValue(XPathPayment.Bank.AccountName);

            set => WaitForTextFieldAndEnterText(XPathPayment.Bank.AccountName, value, false);
        }

        private string CreditCardName
        {
            get => GetValue(XPathPayment.CreditCard.NameOnCard);
            set => WaitForTextFieldAndEnterText(XPathPayment.CreditCard.NameOnCard, value, false);
        }

        private string CreditCardNumber 
        {
            get => GetValue(XPathPayment.CreditCard.CardNumber);
            set => WaitForTextFieldAndEnterText(XPathPayment.CreditCard.CardNumber, value, false);
        }

        private string CreditCardExpiryDateMonth
        {
            get => GetValue(XPathPayment.CreditCard.ExpiryMonth);
            set => WaitForSelectableAndPickFromDropdown(XPathPayment.CreditCard.ExpiryMonth, XPathPayment.CreditCard.MonthOptions, value);
        }

        private string CreditCardExpiryDateYear
        {
            get => GetValue(XPathPayment.CreditCard.ExpiryYear);
            set => WaitForSelectableAndPickFromDropdown(XPathPayment.CreditCard.ExpiryYear, XPathPayment.CreditCard.YearOptions, value);
        }

        private string CreditCardCvc
        {
            get => GetValue(XPathPayment.CreditCard.Cvc);
            set => WaitForTextFieldAndEnterText(XPathPayment.CreditCard.Cvc, value, false);
        }

        public string PaymentAuthorisationText => GetInnerText(XPathPayment.Detail.AuthorisationText).StripLineFeedAndCarriageReturns();

        public string PaymentAcknowledgementtext => GetInnerText(XPathPayment.Detail.AuthorisationAcknowledgement);


        /// <summary>
        /// Completes the payment details. Can be via Credit Card or Bank Account.
        /// The method includes submitting the form to proceed to the next page.
        /// </summary>
        /// <param name="payMethod"></param>
        public void EnterPaymentDetailsAndPurchasePolicy(Vehicle vehicle, Payment payMethod)
        {
            Reporting.LogMinorSectionHeading("Entering Payment Details");
            switch (vehicle)
            {
                case Vehicle.Caravan:
                    //For Caravan we decide the Payment Frequency on the 'Here's your quote' page.
                    //So on the 'Payment Details' page we only need to verify if the original selection is retained or not.
                    Reporting.AreEqual(payMethod.PaymentFrequency, PaymentFrequency, "Payment Frequency matches the user's original selection");
                    break;

                case Vehicle.Boat:
                    //For Boat we can set the Payment Frequencey on the 'Here's your quote' page as with Caravan.
                    //So on the Payment step we should only need to verify that the original selection has been retained.
                        Reporting.AreEqual(payMethod.PaymentFrequency, PaymentFrequency,
                        $"Payment Frequency matches the user's original selection of {PaymentFrequency} from the Your Quote step");
                    break;

                case Vehicle.Motorcycle:
                    //For Motorcycle the Payment Frequency cannot be selected on the 'Here are your quotes' page.
                    //So the user must select the Payment Frequency on 'Payment Details' page.
                    PaymentFrequency = payMethod.PaymentFrequency;
                    break;

                default:
                    break;
            }

            if (payMethod.IsPaymentByBankAccount)
            {
                PaymentMethod = PAYMENT_METHOD_BANK_AC;
                EnterBankDetails(payMethod.Payer.BankAccounts.First());
                VerifyBSBDetails(payMethod.Payer.BankAccounts.First());
            }
            else
            {
                PaymentMethod = PAYMENT_METHOD_CREDIT_CARD;
                EnterCreditCardDetails(payMethod);
            }

            var directDebitTermsRegex = payMethod.IsAnnual && !payMethod.IsPaymentByBankAccount ?
                          new Regex(FixedTextRegex.ANNUAL_POLICY_ENDORSEMENT_CARD_PAYMENT_AUTHORISATION_REGEX) :
                           new Regex(FixedTextRegex.PAYMENT_ANNUAL_BANK_MONTHLY_CREDIT_CARD_BANK_AUTHORISATION_REGEX);

            Reporting.Log($"GetInnerText(XP_AUTHORISATION_TEXT).StripLineFeedAndCarriageReturns() = {GetInnerText(XPathPayment.Detail.AuthorisationText).StripLineFeedAndCarriageReturns()}");
            Match matchTerms = directDebitTermsRegex.Match(PaymentAuthorisationText);
            Reporting.IsTrue(matchTerms.Success, $"authorisation terms for credit card payment are present. Actual Result: {PaymentAuthorisationText}");


            // This authorisation text is displayed only for Annual-Credit Card
            if (!(payMethod.IsAnnual && !payMethod.IsPaymentByBankAccount))
            {
                var directDebitAuthorisationRegex = payMethod.IsAnnual && !payMethod.IsPaymentByBankAccount ?
                                     new Regex(FixedTextRegex.CARD_PAYMENT_AUTHORISATION_TERMS_AGREE_REGEX) :
                                       new Regex(FixedTextRegex.DIRECT_DEBIT_AUTHORISATION_TERMS_AGREE_REGEX);

                Reporting.Log($"GetInnerText(XP_AUTHORISATION_ACKNOWLEDGEMENT) = {GetInnerText(XPathPayment.Detail.AuthorisationAcknowledgement)}");
                Match matchAuthorisation = directDebitAuthorisationRegex.Match(PaymentAcknowledgementtext);
                Reporting.IsTrue(matchAuthorisation.Success, $"acknowledgement text for checkbox is present for credit card payment. Actual Result: {PaymentAcknowledgementtext}");
              
            }
            Reporting.Log($"Capturing card details and Authorisation Text Card", _browser.Driver.TakeSnapshot());

            ClickReadAgreeAuthorisationTerms();

            ClickSubmitButton();
        }

        /// <summary>
        /// Completes the EFT Enter Bank details page.
        /// Click Submit.
        /// </summary>
        /// <param name="bankDetails"></param>
        public void EnterBankDetailsAndClickSubmit(BankAccount bankDetails)
        {
            EnterBankDetails(bankDetails);
            Reporting.Log("Bank Details Page", _driver.TakeSnapshot());
            VerifyBSBDetails(bankDetails);
            ClickSubmitButton();
        }

        /// <summary>
        /// Input the BSB, Account Number and Account Name for a one off or 
        /// recurring payment.
        /// If there is an Add Bank Account radio button (e.g. we're displaying
        /// bank accounts on record to select from but the member has the option
        /// of providing details for a different account) then it is selected
        /// first to display the fields for input.
        /// </summary>
        public void EnterBankDetails(BankAccount bankDetails)
        {
            if (IsControlDisplayed(XPathPayment.Bank.AddBankAccount))
            {
                Reporting.Log($"Selecting Add Bank Account to display account detail fields.");
                ClickControl(XPathPayment.Bank.AddBankAccount);
            }
            BSB = bankDetails.Bsb;
            AccountNumber = bankDetails.AccountNumber;
            AccountName = bankDetails.AccountName;
        }

        /// <summary>
        /// After capturing the initial state, trigger the mandatory field validation
        /// errors and take the follow up snapshot
        /// 
        /// Now that we sometimes display existing bank account details for selection
        /// on this page, it is not safe to assume that hitting Submit will trigger 
        /// the validation as an account may already be selected by default.
        /// 
        /// So we have to ensure the Add new details fields are displayed first, then 
        /// select Submit.
        /// </summary>
        public void TriggerFieldValidation(Browser browser)
        {
            if (IsControlDisplayed(XPathPayment.Bank.AddBankAccount))
            {
                Reporting.Log($"Selecting Add Bank Account to display account detail fields.");
                ClickControl(XPathPayment.Bank.AddBankAccount);
            }
            _driver.WaitForElementToBeVisible(By.XPath(XPathPayment.Bank.Bsb), WaitTimes.T5SEC);
            Reporting.Log($"After confirming new account fields are visible, selecting the Submit button to trigger mandatory field validation errors.", _browser.Driver.TakeSnapshot());
            ClickSubmitButton();

            //TODO - We could always allow Percy.io checking if we implement ignoring the bank account elements via XPath
            if (IsControlDisplayed(XPathPayment.Detail.ExistingAccountOption1))
            {
                Reporting.Log($"Bypassing 'PercyScreenCheck(DividingFenceClaim.YourBankDetailsErrorPage)' because existing account " +
                              $"details presented on page are not accounted for.");
            }
            else
            {
                browser.PercyScreenCheck(DividingFenceClaim.YourBankDetailsErrorPage);
            }
        }

        public void SelectBankDetailOnRecord(BankAccount bankDetails)
        {
            if(IsControlDisplayed(XPathPayment.Detail.ExistingAccountOption1))
            {
                Reporting.Log($"Existing account information is available for selection.", _browser.Driver.TakeSnapshot());
                
                if (IsControlDisplayed(XPathPayment.Detail.ExistingAccountOption2))
                {
                    ClickControl(XPathPayment.Detail.ExistingAccountOption2);
                    Reporting.Log($"A second bank account is available, so selected that option.", _browser.Driver.TakeSnapshot());
                }
                else
                {
                    ClickControl(XPathPayment.Detail.ExistingAccountOption1);
                    Reporting.Log($"Only one bank account is available for selection, so selected that option.", _browser.Driver.TakeSnapshot());
                }
            }
        }

        /// <summary>
        /// Fills in invalid and no match BSB number to validate error message.
        /// 
        /// If there is an Add Bank Account radio button (e.g. we're displaying
        /// bank accounts on record to select from but the member has the option
        /// of providing details for a different account) then it is selected
        /// first to display the fields for input.
        /// </summary>
        /// <param name="payMethod">Payment object to define desired frequency. NULL-skip over setting the payment method and frequency. Relevant for insurance payouts.</param>
        public void EnterInvalidNoMatchBSBAndCheckErrorMessage(Payment payMethod)
        {
            if (payMethod!=null)
            {
                PaymentFrequency = payMethod.PaymentFrequency;
                PaymentMethod = PAYMENT_METHOD_BANK_AC;
            }

            if (IsControlDisplayed(XPathPayment.Bank.AddBankAccount))
            {
                ClickControl(XPathPayment.Bank.AddBankAccount);
            }
            
            var invalidBSBAccount = new BankAccount().InitWithRandomValues();
            invalidBSBAccount.Bsb = BSBValidation.InvalidBSB.InvalidBSBNumber;
            invalidBSBAccount.BankBranchState = BSBValidation.InvalidBSB.InvalidBSBWarningText;

            var noMatchBSBAccount = new BankAccount().InitWithRandomValues();
            noMatchBSBAccount.Bsb = BSBValidation.NoMatchBSB.NoMatchBSBNumber;
            noMatchBSBAccount.BankBranchState = BSBValidation.NoMatchBSB.NoMatchBSBWarningText;

            var bankAccounts = new List<BankAccount>(){
                invalidBSBAccount,
                noMatchBSBAccount
            };
            foreach(var bankAccount in bankAccounts)
            {
                EnterBankDetails(bankAccount);
                VerifyBSBDetails(bankAccount);
            }
        }

        public void VerifyBSBDetails(BankAccount bankDetails)
        {             
            _driver.WaitForElementToBeInvisible(By.XPath(XPathPayment.Bank.BsbDetailsChecking), WaitTimes.T30SEC);
            Reporting.AreEqual(bankDetails.BankBranchState, BSBDetails, $"BSB details matches the expected text '{bankDetails.BankBranchState}' for the BSB nuumber '{bankDetails.Bsb}'. Actual Result:'{BSBDetails}'");
        }

        public void EnterCreditCardDetails(Payment payment)
        {
            var cc = payment.CreditCardDetails;
            cc.CardholderName = payment.Payer.GetFullTitleAndName();

            FillCreditCardDetailsIn(cc, payment.IsAnnual);   
        }

        /// <summary>
        /// Fills in the details for a credit card in the Westpac iFrame.  When making a payment
        /// for an Annual Cash amount CVN will be required.  Registering a card for direct debit does not require the CVN to 
        /// be entered.
        /// </summary>
        /// <param name="cc">Credit Card details</param>
        /// <param name="isCvnRequired">Flag whether the CVN Number is required (default: false)</param>
        protected void FillCreditCardDetailsIn(CreditCard cc, bool isCvnRequired = false)
        {
            _driver.WaitForElementToBeVisible(By.XPath(XPathPayment.CreditCard.PaymentIframe), WaitTimes.T30SEC);

            _driver.SwitchTo().Frame(_driver.FindElement(By.XPath(XPathPayment.CreditCard.PaymentIframe)));  //Allows Selenium to access the objects inside the Credit Card payment iFrame
            
            CreditCardName = cc.CardholderName;
            CreditCardNumber = cc.CardNumber;
            CreditCardExpiryDateMonth = cc.CardExpiryDate.ToString("MM");
            CreditCardExpiryDateYear = cc.CardExpiryDate.ToString("yyyy");

            if (isCvnRequired)
            {
                Reporting.Log($"CreditCardCvc = '{cc.CVNNumber}'");
                CreditCardCvc = cc.CVNNumber;
            }
            Reporting.Log($"CreditCardName = {cc.CardholderName}, CreditCardNumber = '{cc.CardNumber}', " +
                $"CreditCardExpiryDateMonth = '{cc.CardExpiryDate.ToString("MM")}', CreditCardExpiryDateYear = '{cc.CardExpiryDate.ToString("yyyy")}", _browser.Driver.TakeSnapshot());
            _driver.SwitchTo().ParentFrame();
        }

        /// <summary>
        /// Attempt to click the "Submit" button.
        /// </summary>
        /// <exception cref="ReadOnlyException">Thrown if button is present but disabled.</exception>
        public void ClickSubmitButton()
        {
            Reporting.Log("Capturing 'Payment screen' before selecting Submit:", _browser.Driver.TakeSnapshot());
            if (IsControlEnabled(XPathPayment.Button.Submit))
            { ClickControl(XPathPayment.Button.Submit); }
            else
            { throw new ReadOnlyException("Button is currently disabled and not clickable. Check input values."); }
        }

        public void ClickReadAgreeAuthorisationTerms()
        {
            if (IsControlEnabled(XPathPayment.Detail.AuthorisationCheckbox))
            { ClickControl(XPathPayment.Detail.AuthorisationCheckbox); }
            else
            { Reporting.Error("Button is currently disabled and not clickable. Check input values or enter payment details."); }
        }

        /// <summary>
        /// Check the payment display is correct. Making allowances for the special case when are .00, 
        /// for which only the whole dollar amount should be shown. Otherwise include cents.
        /// </summary>
        /// <param name="expectedAmount">Expected amount as decimal</param>
        /// <param name="actualAmount">Amount displayed</param>
        /// <param name="message">Message to be suppliec to Reporting.AreEqual</param>
        public void VerifyAmountWithTruncationWhenWholeDollar(decimal expectedAmount, string actualAmount, string message)
        {
            if (expectedAmount % 1 == 0)
            {
                Reporting.AreEqual(String.Format("${0:0}", expectedAmount), actualAmount, $"{message} (whole dollar only)");
            }
            else
            {
                Reporting.AreEqual(String.Format("${0:0.00}", expectedAmount), actualAmount, message);
            }
        }
        #endregion Payment page
    }
}