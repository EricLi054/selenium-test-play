using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace UIDriver.Pages.Spark.Endorsements
{
    public class UpdateHowYouPayDetailsPage : SparkPaymentPage
    {
        #region CONSTANTS

        private class Constants
        {
            public const string Title = "Update payment details";

            public class DateChangeMessage
            {
                public const string Heading = "You're not done yet!";
                public const string Paragraph = "Select 'Confirm' to change your next payment " + "date. After this, payments will go back to the original date.";
            }

            public class AccountSelectorPreamble
            {
                public const string Heading = "Choose an account or card";
                public const string ParagraphOne = "We'll use this for all future payments.";
                public const string ParagraphTwo = "If your credit card is expired, you'll need to add a new credit card.";
            }

            public class ValidationMessage
            {
                public const string Email = "Please enter a valid email";
                public const string AccountOption = "Please select an option";

                public const string AccountName = "Please enter a valid account name";
                public const string AccountBSB = "Please enter a valid BSB";
                public const string AccountNumber = "Please enter a valid account number";
            }
        }

        #endregion CONSTANTS

        #region XPATHS

        private class XPath
        {
            public const string Title = "//h2[text() = '" + Constants.Title + "']";

            public class HowOften
            {
                public const string Label = "id('Frequency-label')";
                public const string Value = "id('Frequency-value')";
            }

            public class Amount
            {
                public const string Label = "id('Amount-label')";
                public const string Value = "id('Amount-value')";
            }

            public class NextPaymentDate
            {
                public const string Label = "id('label-cmp-instalment-due-date')";
                public const string CalendarPickerButton = "id('cmp-instalment-due-date')//following-sibling::div/button";
                public const string Value = "id('cmp-instalment-due-date')";

                public class Tooltip
                {
                    public const string OpenButton = "id('tooltip-cmp-instalment-due-dateButton')";
                    // TODO Update How You Pay: Add once dev work complete
                }
            }

            public class AfterDateChangePromptToConfirm
            {
                private const string Container = "id('payment-date-notification-card')";
                public const string WarningIcon = Container + "//div[@id='payment-date-notification-card-icon']/*[local-name()='svg' and @data-icon='triangle-exclamation']";
                public const string Heading = Container + "//p[@id='payment-date-notification-card-title']";
                public const string Paragraph = Container + "//p[@id='payment-date-notification-card-content']";
            }

            /// <summary>
            /// Section for member to choose an account or card
            /// </summary>
            public class AccountSelector
            {
                public class Preamble
                {
                    public const string ParentDiv = "id('cmp-select-payment-method-select-account-preamble')";
                    public const string Heading = ParentDiv + "/h3";
                    public const string ParagraphOne = ParentDiv + "/p[1]";
                    public const string ParagraphTwo = ParentDiv + "/p[2]";
                }

                // Options presented to the member are to use an existing credit card or bank acoount
                // or add a new one
                public const string RadioGroup = "id('select-payment-account-radio-group')";

                public const string AddPaymentMethod = "//div[@id='cmp-select-payment-method-add-payment-method-radio-container']/label";

                public const string NewCardOption = "id('cmp-add-payment-method-input-payment-method-Card')";
                public const string NewBankAccountOption = "//button[@id='cmp-add-payment-method-input-payment-method-Bank account']";

                // For each of the banking details found for a member, they are put into a group with a
                // reference number (index). These details are grouped into the class 'Source'
                public class PaymentOptions
                {
                    private static string Root(string index) => $"//div[@id='payment-account-option-{index}']";

                    public static string Input(string index) => Root(index) + "//div/label//input";

                    // Credit Card variables
                    public static string Number(string index) => Root(index) + $"//span[@id='payment-account-option-{index}-label']";

                    public static string ExpiryDate(string index) => Root(index) + $"//p[@id='payment-account-option-{index}-sublabel']";

                    public static string Icon(string index, string issuer) => $"//div[@id='payment-account-option-{index}-icon']" +
                            $"//*[local-name()='svg' and @id='payment-account-option-{index}-{issuer}']";

                    // Bank Account variables
                    public static string AccountBsbAndNumber(string index) => Root(index) + $"//span[@id='payment-account-option-{index}-label']";

                    public static string AccountHolder(string index) => Root(index) + $"//p[@id='payment-account-option-{index}-sublabel']";
                }

                public class SelectedPaymentOption
                {
                    private static string Root = "//input[@checked='']//ancestor::div[contains(@id,'payment-account-option')]";

                    // When credit card
                    public static string Number = Root + $"//span[contains(@id,'payment-account-option')]";

                    public static string ExpiryDate => Root + $"//p[@data-testid='sublabel']";

                    public static string Icon(string issuer) => Root +
                            $"//*[local-name()='svg' and starts-with(@data-testid,'payment-account-option') and contains(@data-testid,'{issuer}')]";

                    // When it is bank account
                    public static string AccountBsbAndNumber = Root + $"//span[contains(@id,'payment-account-option')]";

                    public static string AccountHolder = Root + Root + $"//p[@data-testid='sublabel']";
                }

                public class NewBankAccount
                {
                    public static string Name = "//input[@id='cmp-bank-account-accountName']";
                    public static string Bsb = "//input[@id='cmp-bank-account-bsb']";
                    public static string Number = "//input[@id='cmp-bank-account-accountNumber']";
                    public static string BSBDetails = "id('cmp-bank-account-bsb-message')";
                    public static string BSBChecking = "//p[@id='cmp-bank-account-bsb-message' and contains(text(),'Checking BSB')]";
                }
                public class EmptyBankAccountWarning
                {
                    public static string Bsb = "id('cmp-bank-account-bsb-message')";
                    public static string Number = "//div[@data-testid='cmp-bank-account-accountNumber']/../p";
                    public static string Name = "id('helper-text-cmp-bank-account-accountName')";
                }

            }

            public class Email
            {
                public const string Label = "//label[@for='email']";
                public const string InputField = "id('email')";
                public const string ValidationMessage = "id('helper-text-email')";                
                public class ToolTip
                {
                    private const string OpenButton = "//button[@aria-describedby ='tooltip-email'])";
                    // TODO Update How You Pay: Add once dev work complete
                }
            }

            public class Confirm
            {
                public class Preamble
                {
                    public const string Title = "id('cmp-debit-details-disclaimer-information-title')";
                    public const string Paragraph = "id('cmp-debit-details-disclaimer-information-paragraph')";
                }

                public class TermsAndConditionsAuthorisation
                {
                    public const string CheckBox = "//input[@id='payment-disclaimer-input']";
                    public const string Text = "id('payment-disclaimer-label')";
                    public const string Link = "id('payment-disclaimer-Link')";
                    public const string AuthorisationCard = "//div[@data-testid='accountAuthorisationCard']";
                }

                public const string Button = "id('cmp-hip-submit')";
            }
        }

        #endregion XPATHS

        #region Settable properties and controls

        public string PageTitle => GetInnerText(XPath.Title);

        public string Frequency => GetInnerText(XPath.HowOften.Value);

        public string AuthorisationText => GetInnerText(XPath.Confirm.TermsAndConditionsAuthorisation.AuthorisationCard).StripLineFeedAndCarriageReturns();

        public string ScheduledAmount => GetInnerText(XPath.Amount.Value);

        public string EmailAddress
        {
            get => GetValue(XPath.Email.InputField);
            set => WaitForTextFieldAndEnterText(XPath.Email.InputField, value, false);
        }

        public DateTime NextPaymentDate
        {
            get => DateTime.ParseExact(GetValue(XPath.NextPaymentDate.Value),
                                       DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH_WITHSPACE,
                                       CultureInfo.InvariantCulture);
            set => SelectDateFromCalendar(dateFieldXPath: null, calendarBtnXPath: XPath.NextPaymentDate.CalendarPickerButton, desiredDate: value);
        }

        public string PaymentDateUpdatedHeading => GetInnerText(XPath.AfterDateChangePromptToConfirm.Heading);
        public string PaymentDateUpdatedParagraph => GetInnerText(XPath.AfterDateChangePromptToConfirm.Paragraph);

        public bool IsPaymentDateUpdatedWarningIconShown => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.AfterDateChangePromptToConfirm.WarningIcon),
                                                    WaitTimes.T5SEC, out IWebElement element);

        public string AccountSelectorPreambleHeading => GetInnerText(XPath.AccountSelector.Preamble.Heading);
        public string AccountSelectorPreambleParagraphOne => GetInnerText(XPath.AccountSelector.Preamble.ParagraphOne);
        public string AccountSelectorPreambleParagraphTwo => GetInnerText(XPath.AccountSelector.Preamble.ParagraphTwo);
        public string CreditCardNumber => GetInnerText(XPath.AccountSelector.SelectedPaymentOption.Number);
        public string CreditCardExpiry => GetInnerText(XPath.AccountSelector.SelectedPaymentOption.ExpiryDate);

        public bool IsCardIconDisplayed(string issuer) => _driver.TryWaitForElementToBeVisible(By.XPath(XPath.AccountSelector.SelectedPaymentOption.Icon(issuer)),
            WaitTimes.T5SEC, out IWebElement _);

        public string BankAccountBsbAndNumber => GetInnerText(XPath.AccountSelector.SelectedPaymentOption.AccountBsbAndNumber);
        public string BankAccountName => GetInnerText(XPath.AccountSelector.SelectedPaymentOption.AccountHolder);

        public string ConfirmUpdateHeading => GetInnerText(XPath.Confirm.Preamble.Title);
        public string ConfirmUpdateParagraph => GetInnerText(XPath.Confirm.Preamble.Paragraph);

        public string TermsAndConditionsText => GetInnerText(XPath.Confirm.TermsAndConditionsAuthorisation.Text);

        private string NewBankAccountBsb
        {
            get => GetValue(XPath.AccountSelector.NewBankAccount.Bsb);
            set => WaitForTextFieldAndEnterText(XPath.AccountSelector.NewBankAccount.Bsb, value, false);
        }

        private string NewBankAccountNumber
        {
            get => GetValue(XPath.AccountSelector.NewBankAccount.Number);
            set => WaitForTextFieldAndEnterText(XPath.AccountSelector.NewBankAccount.Number, value, false);
        }

        private string NewBankAccountName
        {
            get => GetValue(XPath.AccountSelector.NewBankAccount.Name);
            set => WaitForTextFieldAndEnterText(XPath.AccountSelector.NewBankAccount.Name, value, false);
        }

        #endregion Settable properties and controls

        public UpdateHowYouPayDetailsPage(Browser browser) : base(browser)
        {
        }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Title);
                Reporting.Log($"Found Page Title.");
                GetElement(XPath.HowOften.Label);
                Reporting.Log($"Found labels for collection details.");
                GetElement(XPath.Confirm.Button);
                Reporting.Log($"Found Confirm button.");
                Reporting.LogPageChange("Spark Update How You Pay - Policy Payment Details page");
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void ConfirmUpdate()
        {
            ClickControl(XPath.Confirm.Button);
        }

        public void AcceptPaymentTermsAndConditions()
        {
            var checkboxTick = GetElement(XPath.Confirm.TermsAndConditionsAuthorisation.CheckBox);
            if (!checkboxTick.Selected)
            {
                checkboxTick.Click();
            }
            Reporting.Log($"Terms and Conditions Checkbox marked as selected: {checkboxTick.Selected}");
        }

        public void AddNewAccount(BankAccount bankDetails)
        {
            ClickControl(XPath.AccountSelector.AddPaymentMethod);
            ClickControl(XPath.AccountSelector.NewBankAccountOption);

            EnterBankDetails(bankDetails);
        }

        /// <summary>
        /// Overrides `SparkPaymentPage` VerifyBSBDetails() as
        /// Change How I Pay, uses different controls.
        /// </summary>
        new public void VerifyBSBDetails(BankAccount bankDetails)
        {
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.AccountSelector.NewBankAccount.BSBChecking), WaitTimes.T30SEC);
            Reporting.AreEqual(bankDetails.BankBranchState, GetInnerText(XPath.AccountSelector.NewBankAccount.BSBDetails), $"BSB details matches the expected text '{bankDetails.BankBranchState}' for BSB number {bankDetails.Bsb}.");
        }

        /// <summary>
        /// Fills in invalid and no match BSB number to validate error message.
        /// </summary>
        public void EnterInvalidNoMatchBSBAndCheckErrorMessage()
        {
            ClickControl(XPath.AccountSelector.AddPaymentMethod);
            ClickControl(XPath.AccountSelector.NewBankAccountOption);

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
            foreach (var bankAccount in bankAccounts)
            {
                EnterBankDetails(bankAccount);
                VerifyBSBDetails(bankAccount);
            }
        }

        public void AddNewCreditCard(CreditCard creditCard)
        {
            ClickControl(XPath.AccountSelector.AddPaymentMethod);
            ClickControl(XPath.AccountSelector.NewCardOption);
            FillCreditCardDetailsIn(creditCard, false);
        }

        public new void EnterBankDetails(BankAccount bankDetails)
        {
            NewBankAccountBsb = bankDetails.Bsb;
            NewBankAccountNumber = bankDetails.AccountNumber;
            NewBankAccountName = bankDetails.AccountName;
        }

        /// <summary>
        /// Gets the count of options for choosing Bank Account / Credit Card
        /// </summary>
        /// <returns></returns>
        private int GetLengthOfRadioGroup()
        {
            var radioGroup = GetElement(XPath.AccountSelector.RadioGroup);
            return radioGroup.FindElements(By.XPath(".//div[contains(@id,'payment-account-option')]")).Count;
        }

        /// <summary>
        /// Move banking details into view
        /// </summary>
        private void ScrollPaymentSourceDetailsIntoView()
        {
            ScrollElementIntoView(XPath.AccountSelector.RadioGroup);
        }

        /// <summary>
        /// Move the credit card and direct debit authorisation term and conditions
        /// section into view
        /// </summary>
        public void ScrollTermsAndConditionsIntoView()
        {
            ScrollElementIntoView(XPath.Confirm.TermsAndConditionsAuthorisation.Text);
        }

        /// <summary>
        /// Checks that current credit card details for a policy installment
        /// are correct and is the selected radio button.
        /// </summary>
        /// <param name="paymentSource"></param>
        public void VerifySelectedCreditCard(CreditCard paymentSource)
        {
            ScrollPaymentSourceDetailsIntoView();
            Reporting.Log("Checking selected credit card", _browser.Driver.TakeSnapshot());
            // AMEX cards despite been 15 character longs will still have 12 * and the last four digits for masking
            Reporting.AreEqual("**** **** **** " + paymentSource.CardNumber.Substring(paymentSource.CardNumber.Length - 4), CreditCardNumber,
                        "credit card masked with correct last four digits");
            Reporting.AreEqual("Exp: " + paymentSource.CardExpiryDate.ToString(DataFormats.DATE_MONTH_YEAR_FORWARDSLASH, CultureInfo.InvariantCulture),
                CreditCardExpiry, "expiry date is displayed.");
            Reporting.IsTrue(IsCardIconDisplayed(paymentSource.CardIssuer.GetDescription().ToLower()), "correct card icon is displayed");
        }

        /// <summary>
        /// Checks that the correct bank account is selected as the source for the next payment
        /// </summary>
        /// <param name="paymentSource"></param>
        public void VerifySelectedBankAccount(BankAccount paymentSource)
        {
            ScrollPaymentSourceDetailsIntoView();
            Reporting.Log("Checking selected bank account", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual("BSB " + paymentSource.Bsb + " ACC *** " + paymentSource.AccountNumber.Substring(paymentSource.AccountNumber.Length - 3), BankAccountBsbAndNumber,
                        "bank account masked with correct last three digits.");
            Reporting.AreEqual(paymentSource.AccountName, BankAccountName, "account holder is displayed");
        }

        /// <summary>
        /// Checks copy that informs members about how updates to bank accounts and credit cards will work.
        /// </summary>
        public void VerifyAccountSelectorPreamble()
        {
            Reporting.AreEqual(Constants.AccountSelectorPreamble.Heading, AccountSelectorPreambleHeading,
                "heading for instructions on changing payment method");
            Reporting.AreEqual(Constants.AccountSelectorPreamble.ParagraphOne, AccountSelectorPreambleParagraphOne,
                "first paragraph for instructions on changing method");
            Reporting.AreEqual(Constants.AccountSelectorPreamble.ParagraphTwo, AccountSelectorPreambleParagraphTwo,
                "second paragraph for instructions on changing payment method");
        }

        /// <summary>
        /// Checks that after changing the next installment date,
        /// a prompt to the member to click the confirm button is shown.
        /// </summary>
        public void VerifyPaymentUpdatedPromptToConfirm()
        {
            Reporting.IsTrue(IsPaymentDateUpdatedWarningIconShown,
                "warning icon for prompt to confirm after updating instalment date is displayed");
            Reporting.AreEqual(Constants.DateChangeMessage.Heading, PaymentDateUpdatedHeading,
                "heading for prompt to confirm after changing date is displayed");
            Reporting.AreEqual(Constants.DateChangeMessage.Paragraph, PaymentDateUpdatedParagraph,
                "paragraph for prompt to confirm after changing date is displayed");
        }

        /// <summary>
        /// Checks copy that terms and condition for bank direct debit and credit card payments are shown.
        /// In the initial state, the acceptance checkbox should not be selected.
        /// </summary>
        public void VerifyTermsAndConditionsInitialState()
        {
            ScrollTermsAndConditionsIntoView();
            Reporting.Log("Checking acceptance of terms and conditions is not selected and Confirm button disabled", _browser.Driver.TakeSnapshot());
            Reporting.IsTrue(IsControlEnabled(XPath.Confirm.TermsAndConditionsAuthorisation.CheckBox), "The terms and condition checkbox is selected by default");
            var directDebitAuthorisationRegex = new Regex(FixedTextRegex.DIRECT_DEBIT_AUTHORISATION_TERMS_AGREE_REGEX);
            Match match = directDebitAuthorisationRegex.Match(TermsAndConditionsText);
            Reporting.IsTrue(match.Success, $"correct text is shown for debit terms and conditions checkbox. Actual Result: {TermsAndConditionsText}");
        }

        /// <summary>
        /// Check the expected warning message is displayed if no email is entered
        /// </summary>
        public void VerifyEmailWarning()
        {
            ScrollTermsAndConditionsIntoView();
            Reporting.AreEqual(Constants.ValidationMessage.Email, GetInnerText(XPath.Email.ValidationMessage), 
                "the correct warning message is displayed when no email address is added.");
        }

        /// <summary>
        /// Check the expected warning message is displayed if no BSB details are entered
        /// </summary>
        public void VerifyEmptyBSBDetailsWarning()
        {
            ClickControl(XPath.AccountSelector.AddPaymentMethod);
            ClickControl(XPath.AccountSelector.NewBankAccountOption);
            ConfirmUpdate();

            Reporting.AreEqual(Constants.ValidationMessage.AccountBSB, GetInnerText(XPath.AccountSelector.EmptyBankAccountWarning.Bsb),
                "the correct BSB warning message is displayed when no BSB is provided.");
            Reporting.AreEqual(Constants.ValidationMessage.AccountNumber, GetInnerText(XPath.AccountSelector.EmptyBankAccountWarning.Number),
                "the correct Account number warning message is displayed when no account number is provided.");
            Reporting.AreEqual(Constants.ValidationMessage.AccountBSB, GetInnerText(XPath.AccountSelector.EmptyBankAccountWarning.Bsb),
                "the correct account name warning message is displayed when no account name is provided.");
            Reporting.Log("Empty account warning ", _browser.Driver.TakeSnapshot());
        }
    }
}