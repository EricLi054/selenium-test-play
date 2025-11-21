using Rac.TestAutomation.Common;
using System;
using System.Threading;
using System.Text.RegularExpressions;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using System.Collections.Generic;
using OpenQA.Selenium;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace UIDriver.Pages.B2C
{
    public class QuotePayments : BasePage
    {
        #region XPATHS
        private const string BASE    = "/html/body/div[@id='wrapper']";
        private const string CONTENT = BASE + "//form";

        private const string XP_POL_START_DATE = CONTENT + "//div[@id='PaymentDetails_PolicyStartDate_Answer']";

        private const string XP_RADIO_ANNUAL   = CONTENT + "//div[@id='PaymentDetails_PaymentFrequency_Y_Label']/span[contains(@class,'rb-radio')]";
        private const string XP_RADIO_MONTHLY  = CONTENT + "//div[@id='PaymentDetails_PaymentFrequency_M_Label']/span[contains(@class,'rb-radio')]";
        private const string XP_AMOUNT_ANNUAL  = CONTENT + "//div[@id='PaymentDetails_PaymentFrequency_Y_Label']/span[contains(@class,'rb-text')]";
        private const string XP_AMOUNT_MONTHLY = CONTENT + "//div[@id='PaymentDetails_PaymentFrequency_M_Label']/span[contains(@class,'rb-text')]";

        private const string XP_RADIO_CC = CONTENT + "//div[@id='PaymentDetails_PaymentMethod_4_Label']/span[contains(@class,'rb-radio')]";
        private const string XP_RADIO_DD = CONTENT + "//div[@id='PaymentDetails_PaymentMethod_2_Label']/span[contains(@class,'rb-radio')]";

        // Direct Debit
        private const string XP_DD_BSB  = CONTENT + "//input[@id='PaymentDetails_DirectDebit_Bsb']";
        private const string XP_DD_ACC  = CONTENT + "//input[@id='PaymentDetails_DirectDebit_AccountNumber']";
        private const string XP_DD_NAME = CONTENT + "//input[@id='PaymentDetails_DirectDebit_AccountName']";
        private const string XP_BANK_BSB_DETAILS_CHECKING = "//div[@id='div-payment-details-bsb-lookup-bankdetails' and contains(text(),'Checking BSB')]";
        private const string XP_BANK_BSB_DETAILS = "//div[@id='div-payment-details-bsb-lookup-bankdetails']";
        private const string XP_BANK_BSB_DETAILS_WARNING = "//span[@id='payment-details-direct-debit']//span[@class='field-validation-error']";

        // Common
        private const string XP_PAYER_NAME     = CONTENT + "//span[@aria-owns='PaymentDetails_PolicyPaidBy_listbox']";
        private const string XP_PAYER_DROPDOWN = "//ul[contains(@id,'PaymentDetails_PolicyPaidBy_listbox')]/li";

        private const string XP_BTN_BACK   = CONTENT + "//button[contains(@class,'back-button')]";
        private const string XP_AUTHORISATION_CHECKBOX = "//input[@id='PaymentDetails_TermsClicked']";
        private const string XP_BTN_SUBMIT = CONTENT + "/*//button[@id='accordion_0_submit-action']";

        // Roadside
        private const string XP_STEP_1_AMOUNT                          = CONTENT + "//div[contains(@class,'row collapse') and contains(@class,'showQuestion')]//div[@id='roadsideStep1']/div[@class='amount']";
        private const string XP_STEP_2_TEXT                            = CONTENT + "//div[contains(@class,'showQuestion') and contains(@id,'payment-details-roadside')]//div[@id='roadsideStep2']";
        private const string XP_CAR_INSURANCE_PREMIUM_AMOUNT           = CONTENT + "//div[contains(@class,'row collapse') and contains(@class,'showQuestion')]//div[text()='Car Insurance premium']/following-sibling::div[1]";
        private const string XP_ROADSIDE_ASSISTANCE_AMOUNT             = CONTENT + "//div[contains(@class,'row collapse') and contains(@class,'showQuestion')]//div[text()='Classic Roadside Assistance']/following-sibling::div[1]";
        private const string XP_CARINSURANCE_AND_ROADSIDE_TOTAL_AMOUNT = CONTENT + "//div[contains(@class,'row collapse') and contains(@class,'showQuestion')]//div[text()='Total']/following-sibling::div[1]";

        // CC Icons
        private const string XP_ICONS             = "//div[@class='payment-icons on-payment']/div[@class='payment-icon-set payment-page']";
        private const string XP_MASTER_CARD_ICON  = XP_ICONS + "/div[contains(@class, 'mastercard')]";
        private const string XP_VISA_ICON         = XP_ICONS + "/div[contains(@class, 'visa')]";
        private const string XP_DIRECT_DEBIT_ICON = XP_ICONS + "/div[contains(@class, 'direct-debit')]";
        private const string XP_AMEX_ICON         = XP_ICONS + "/div[contains(@class, 'amex')]";

        // Confirmation messages
        private const string XP_PAYNOW_WARNING_BANNER    = "//div[@id='pay-now-confirmation-message']/div/div[@class='warning-banner']";
        private const string XP_AUTHORISATION_TEXT = "//div[contains(@class,'showQuestion') and contains(@id,'payment-details-authorisation')]/div/div[contains(@Class, 'payment-information')]";

        #endregion

        #region Settable properties and controls
        public string DDBsb
        {
            get => GetInnerText(XP_DD_BSB);
            set => WaitForTextFieldAndEnterText(XP_DD_BSB, value, false);
        }

        public string BSBDetails => GetInnerText(XP_BANK_BSB_DETAILS);

        public string BSBDetailsWarning => GetInnerText(XP_BANK_BSB_DETAILS_WARNING);

        public string DDName
        {
            get => GetInnerText(XP_DD_NAME);
            set => WaitForTextFieldAndEnterText(XP_DD_NAME, value, false);
        }

        public string DDNumber
        {
            get => GetInnerText(XP_DD_ACC);
            set => WaitForTextFieldAndEnterText(XP_DD_ACC, value, false);
        }

        public string Payer
        {
            get => GetInnerText(XP_PAYER_NAME + XPEXT_DROPDOWN_VALUE);
            set
            {
                try
                {
                    WaitForSelectableAndPickFromDropdown(XP_PAYER_NAME, XP_PAYER_DROPDOWN, value);
                }
                catch (NoSuchElementException)
                {
                    Reporting.Error($"Element not found when trying to select Payer ({value}) from dropdown.");
                }
            }
        }

        /// <summary>
        /// Get the amount that member will be paying in their first installment.
        /// Applicable for Motor Quotes Only.
        /// </summary>
        public decimal RoadsideStep1Price
        {
            get
            {
                var element = GetElement(XP_STEP_1_AMOUNT);
                var price = decimal.Parse(element.Text.StripMoneyNotations());
                return price;
            }
        }

        /// <summary>
        /// Get the text from the information dialog for step 2.
        /// Applicable for Motor Quotes Only.
        /// </summary>
        public string RoadsideStep2Text
        {
            get
            {
                var element = GetElement(XP_STEP_2_TEXT);
                return element.Text.Trim().StripLineFeedAndCarriageReturns();
            }
        }

        /// <summary>
        /// Get the Car Insurance premium amount that member will be paying
        /// Applicable for Motor Quotes with Roadside only, Annual - Credit Card payments
        /// </summary>
        public decimal CarInsurancePremiumPrice
        {
            get
            {
                var element = GetElement(XP_CAR_INSURANCE_PREMIUM_AMOUNT);
                var price = decimal.Parse(element.Text.StripMoneyNotations());
                return price;
            }
        }

        /// <summary>
        /// Get the Classic Roadside Assistance amount that member will be paying
        /// Applicable for Motor Quotes with Roadside only, Annual - Credit Card payments
        /// </summary>
        public decimal ClassicRoadsideAssistancePrice
        {
            get
            {
                var element = GetElement(XP_ROADSIDE_ASSISTANCE_AMOUNT);
                var price = decimal.Parse(element.Text.StripMoneyNotations());
                return price;
            }
        }

        /// <summary>
        /// Get the Total amount of Car Insurance premium and Classic Roadside Assistance that member will be paying
        /// Applicable for Motor Quotes with Roadside only, Annual - Credit Card payments
        /// </summary>
        public decimal CarInsuranceAndRoadsidePrice
        {
            get
            {
                var element = GetElement(XP_CARINSURANCE_AND_ROADSIDE_TOTAL_AMOUNT);
                var price = decimal.Parse(element.Text.StripMoneyNotations());
                return price;
            }
        }

        public string SubmitButtonText => GetInnerText(XP_BTN_SUBMIT);

        /// <summary>
        /// Gets the whole text of the Authorisation message displayed for different payment frequency and payment method combination
        /// </summary>
        public string AuthorisationText => GetInnerText(XP_AUTHORISATION_TEXT);

        /// <summary>
        /// Returns if Annual Payment Frequency radio button is ticked
        /// </summary>
        public bool IsAnnualPaymentFrequencySelected => GetClass(XP_RADIO_ANNUAL).Contains("checked");

        /// <summary>
        /// Returns if Monthly Payment Frequency radio button is ticked
        /// </summary>
        public bool IsMonthlyPaymentFrequencySelected => GetClass(XP_RADIO_MONTHLY).Contains("checked");

        /// <summary>
        /// Returns if Credit Card Payment Method radio button is ticked
        /// </summary>
        public bool IsCreditCardPaymentMethodSelected => GetClass(XP_RADIO_CC).Contains("checked");

        /// <summary>
        /// Returns if Credit Card Payment Method radio button is ticked
        /// </summary>
        public bool IsDirectDebitPaymentMethodSelected => GetClass(XP_RADIO_DD).Contains("checked");

        /// <summary>
        /// Returns if 'Pay now' button is enabled
        /// </summary>
        public bool IsPayNowButtonEnabled => IsControlEnabled(XP_BTN_SUBMIT);

        /// <summary>
        /// Gets amount displayed for Annual payment frequency
        /// </summary>
        public string GetAnnualAmount => GetAmountFromPaymentFrequency(XP_AMOUNT_ANNUAL);

        /// <summary>
        /// Gets amount displayed for Monthly payment frequency
        /// </summary>
        public string GetMonthlyAmount => GetAmountFromPaymentFrequency(XP_AMOUNT_MONTHLY);

        private string GetAmountFromPaymentFrequency(string xPath) => DataHelper.SplitStringAndReturnAnElementFromArray(GetInnerText(xPath), char.Parse("$"), 1).StripMoneyNotations();
        #endregion

        public QuotePayments(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XP_POL_START_DATE);
                GetElement(XP_RADIO_ANNUAL);
                GetElement(XP_RADIO_CC);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("New business payment page");
            return true;
        }

        /// <summary>
        /// Completes all payment info, but does not submit form.
        /// Useful where verification of form details is desired.
        /// </summary>
        /// <param name="paymentInfo"></param>
        /// <param name="payer"></param>
        public decimal FillPayment(Payment paymentInfo)
        {
            if (paymentInfo == null || paymentInfo.Payer == null)
            { Reporting.Error("Missing payment info. Cannot proceed."); }

            if (paymentInfo.IsPaymentByBankAccount &&
                paymentInfo.Payer.BankAccounts.Count < 1)
            { Reporting.Error("Provided contact does not have bank account details defined."); }

            if (!paymentInfo.IsPaymentByBankAccount &&
                paymentInfo.Payer.CreditCards.Count < 1)
            { Reporting.Error("Provided contact does not have credit card details defined."); }

            var xPathPaymentFrequency = paymentInfo.IsAnnual ?
                                        XP_RADIO_ANNUAL : XP_RADIO_MONTHLY;
            ClickControl(xPathPaymentFrequency);

            if (paymentInfo.IsPaymentByBankAccount)
            {
                CompleteBankAccountPrompts(paymentInfo.Payer, isWarningExpected:false);
            }
            else
            {
                CompleteCreditCardPrompts(paymentInfo);
            }

            return GetNextInstallmentAmountForChosenFrequency(paymentInfo.PaymentFrequency);
        }

        /// <summary>
        /// Check to See the payment is by Annual Credit Card
        /// </summary>
        public bool HasAnnualCreditCardPaymentMethodBeenEntered()
        {
            return IsAnnualPaymentFrequencySelected && IsCreditCardPaymentMethodSelected?true: false;
        }

        /// <summary>
        /// Clicking the 'Complete Step 1' button for Motor Policy having roadside 
        /// Or Click Purchase Policy/Paynow for all other type
        /// </summary>
        public void ClickSubmit()
        {
            ClickControl(XP_BTN_SUBMIT);
        }

        private void WaitForDirectDebitFields()
        {
            var found = true;
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
            do
            {
                try
                {
                    found = GetElement(XP_DD_BSB).Displayed &
                            GetElement(XP_DD_ACC).Displayed &
                            GetElement(XP_DD_NAME).Displayed;
                    Thread.Sleep(2000);
                }
                catch (NoSuchElementException)
                {
                    found = false;
                }
            } while (DateTime.Now < endTime && !found);

            if (!found)
            { Reporting.Error("Direct Debit payment fields were not found or visible in time. See " + _driver.TakeSnapshot()); }
        }

        private void CompleteBankAccountPrompts(Contact payer,bool isWarningExpected)
        {
            CompleteBankAccountPromptsWithSpecificBankAccount(payer, payer.BankAccounts[0], isWarningExpected);
        }

        private void CompleteCreditCardPrompts(Payment paymentInfo)
        {
            ClickControl(XP_RADIO_CC);

            if (!IsAuthorisationMessageShown())
            {
                ClickControl(XP_RADIO_DD);
                ClickControl(XP_RADIO_CC);
                Reporting.Log($"We failed to observe authorisation text, so toggled CC payment off/on and tried again, which resulted in visibility of authorisation message; {IsAuthorisationMessageShown()}");
            }

            Thread.Sleep(2000); // Need a sleep. Clicking too fast breaks javascript sometimes.

            if (paymentInfo.IsMonthly)
            {
                Payer = $"{paymentInfo.Payer.FirstName} {paymentInfo.Payer.Surname}";
            }
        }

        public decimal GetNextInstallmentAmountForChosenFrequency(PaymentFrequency frequency)
        {
            var xPathInstallmentText = frequency == PaymentFrequency.Annual ?
                                        XP_AMOUNT_ANNUAL : XP_AMOUNT_MONTHLY;

            return DataHelper.GetMonetaryValueFromString(GetInnerText(xPathInstallmentText));
        }

        /// <summary>
        /// Verifies the submit button text in payment page is correct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paymentInfo"></param>
        /// <param name="isMotorQuoteIncludingRoadside"></param>
        public void VerifySubmitButtonTextIsCorrect<T>(Payment paymentInfo, bool isMotorQuoteIncludingRoadside = false)
        {

            if ((typeof(T) == typeof(QuoteCar)) && isMotorQuoteIncludingRoadside && paymentInfo.Scenario != PaymentScenario.AnnualCash)
                Reporting.AreEqual("Complete Step 1", SubmitButtonText, "Complete Step 1 button text is as expected");
            else
            {
                if (paymentInfo.IsAnnual && !paymentInfo.IsPaymentByBankAccount)
                    Reporting.AreEqual("Pay now", SubmitButtonText, "Pay now button text is as expected");
                else
                    Reporting.AreEqual("Purchase Policy", SubmitButtonText, "Purchase Policy button text is as expected");
            }

        }

        /// <summary>
        /// Verifies the Authorisation message on the Payment screen
        /// </summary>
        public void VerifyAuthorisationMessageIsCorrect()
        {
            ScrollElementIntoView(XP_BTN_SUBMIT);
            Regex authorisationMessageRegex = HasAnnualCreditCardPaymentMethodBeenEntered() ? 
                new Regex(FixedTextRegex.PAYMENT_ANNUAL_CREDIT_CARD_AUTHORISATION_REGEX) : new Regex(FixedTextRegex.PAYMENT_ANNUAL_BANK_MONTHLY_CREDIT_CARD_BANK_AUTHORISATION_REGEX);
            var authorisationMessageText = AuthorisationText;
            Match match = authorisationMessageRegex.Match(authorisationMessageText);

            Reporting.IsTrue(match.Success, $"the Authorisation message is correct. Actual: {authorisationMessageText}");
        }

        public bool VerifyUIElementsInPaymentDetailsPageAreCorrect()
        {
            Reporting.IsFalse(GetClass(XP_RADIO_ANNUAL).Contains("checked"),
                                "Annual".IsNotSelected());
            Reporting.IsFalse(GetClass(XP_RADIO_MONTHLY).Contains("checked"),
                                "Monthly".IsNotSelected());
            Reporting.IsFalse(GetClass(XP_RADIO_CC).Contains("checked"),
                                "Credit card".IsNotSelected());
            Reporting.IsFalse(GetClass(XP_RADIO_DD).Contains("checked"),
                                "Direct Debit".IsNotSelected());
            Reporting.IsTrue(IsControlEnabled(XP_BTN_SUBMIT),
                                "Purchase Policy".IsEnabled());
            Reporting.IsFalse(IsControlDisplayed(XP_AUTHORISATION_TEXT),
                                "Annual credit card authorisation message".IsNotDisplayed());
            Reporting.IsFalse(IsControlDisplayed(XP_MASTER_CARD_ICON),
                                "Mastercard payment method icon".IsNotDisplayed());
            Reporting.IsFalse(IsControlDisplayed(XP_VISA_ICON),
                                "Visa payment method icon".IsNotDisplayed());
            Reporting.IsFalse(IsControlDisplayed(XP_DIRECT_DEBIT_ICON),
                                "Direct debit payment method icon".IsNotDisplayed());
            Reporting.IsFalse(IsControlDisplayed(XP_AMEX_ICON),
                                "Amex icon".IsNotDisplayed());
            Reporting.IsFalse(IsControlDisplayed(XP_PAYNOW_WARNING_BANNER),
                                "Pay now message".IsNotDisplayed());

            return true;
        }

        /// <summary>
        /// Fills in invalid and no match BSB number to validate error message.
        /// </summary>
        /// <param name="payMethod">Payment object to define desired frequency.</param>
        public void EnterInvalidNoMatchBSBAndCheckErrorMessage(Payment payMethod)
        {
            ClickControl(payMethod.IsAnnual ? XP_RADIO_ANNUAL : XP_RADIO_MONTHLY);

            var invalidBSBAccount = new BankAccount().InitWithRandomValues();
            invalidBSBAccount.Bsb = BSBValidation.InvalidBSB.InvalidBSBNumber;
            invalidBSBAccount.BankBranchState = BSBValidation.InvalidBSB.InvalidBSBWarningText+".";

            var noMatchBSBAccount = new BankAccount().InitWithRandomValues();
            noMatchBSBAccount.Bsb = BSBValidation.NoMatchBSB.NoMatchBSBNumber;
            noMatchBSBAccount.BankBranchState = BSBValidation.NoMatchBSB.NoMatchBSBWarningText+".";

            var bankAccounts = new List<BankAccount>(){
                invalidBSBAccount,
                noMatchBSBAccount
            };

            foreach (var bankAccount in bankAccounts)
            {
                CompleteBankAccountPromptsWithSpecificBankAccount(payMethod.Payer, bankAccount, isWarningExpected:true);            
            }
        }

        private void CompleteBankAccountPromptsWithSpecificBankAccount(Contact payer, BankAccount bankDetails, bool isWarningExpected)
        {
            Reporting.Log("Payment via direct debit.");
            ClickControl(XP_RADIO_DD);

            if (!IsAuthorisationMessageShown())
            {
                ClickControl(XP_RADIO_CC);
                ClickControl(XP_RADIO_DD);
                Reporting.Log($"We failed to observe authorisation text, so toggled DD payment off/on and tried again, which resulted in visibility of authorisation message; {IsAuthorisationMessageShown()}");
            }

            WaitForDirectDebitFields();
            Thread.Sleep(2000); // Need a sleep. As some of the JS is slow.

            DDBsb = bankDetails.Bsb;
            DDNumber = bankDetails.AccountNumber;
            DDName = bankDetails.AccountName;
            Thread.Sleep(2000);

            Payer = $"{payer.FirstName} {payer.Surname}";

            VerifyBSBDetails(bankDetails, isWarningExpected);
        }

        public void VerifyBSBDetails(BankAccount bankDetails, bool isWarningExpected)
        {
            _driver.WaitForElementToBeInvisible(By.XPath(XP_BANK_BSB_DETAILS_CHECKING), WaitTimes.T30SEC);
            if (isWarningExpected)
            {
                Reporting.AreEqual(bankDetails.BankBranchState, BSBDetailsWarning, $"BSB details warning message for the BSB number '{bankDetails.Bsb}'");
            }
            else
            {
                Reporting.AreEqual(bankDetails.BankBranchState, BSBDetails, $"BSB details for the BSB number '{bankDetails.Bsb}'");
            }
        }

        /// <summary>
        /// Waits (maximum of 10 seconds) for the 'I've read and agree to the direct debit authorisation terms.' checkbox 
        /// to display for payment and clicks it if it is visible.
        /// If the checkbox is not found, throws an error.
        /// </summary>
        public void ClickReadAgreeAuthorisationTerms()
        {
            if (!_driver.TryWaitForElementToBeVisible(By.XPath(XP_AUTHORISATION_CHECKBOX), WaitTimes.T10SEC, out IWebElement authorisationCheckbox))
            {
                Reporting.Error("Button is currently disabled and not clickable. Check input values or enter payment details."); 
            }
            else
            {
                ClickControl(XP_AUTHORISATION_CHECKBOX);
            }
        }

        private bool IsAuthorisationMessageShown()
        {
            ScrollElementIntoView(XP_BTN_SUBMIT);

            Regex authorisationMessageRegex = HasAnnualCreditCardPaymentMethodBeenEntered() ?
    new Regex(FixedTextRegex.PAYMENT_ANNUAL_CREDIT_CARD_AUTHORISATION_REGEX) : new Regex(FixedTextRegex.PAYMENT_ANNUAL_BANK_MONTHLY_CREDIT_CARD_BANK_AUTHORISATION_REGEX);
            var isFound = _driver.TryWaitForElementToBeVisible(By.XPath(XP_AUTHORISATION_TEXT), WaitTimes.T5SEC, out IWebElement authTextElement);

            Reporting.Log($"Auth message box found is {isFound}", _driver.TakeSnapshot());

            return isFound;
        }
    }
}