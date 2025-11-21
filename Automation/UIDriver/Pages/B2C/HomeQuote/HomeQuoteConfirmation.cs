using Rac.TestAutomation.Common;
using System;
using OpenQA.Selenium;
using System.Text.RegularExpressions;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace UIDriver.Pages.B2C
{
    public class HomeQuoteConfirmation : BasePage
    {
        #region XPATHS
        private const string BASE    = "/html/body/div[@id='wrapper']";
        private const string CONTENT = BASE + "//form";

        private const string XP_HEADER = BASE + "//span[@class='action-heading']/span";

        // General Summary fields
        private const string XP_POLICY_NUM       = "id('PolicyNumber_Answer')";
        private const string XP_COVER_TYPE       = "id('CoverTypeDescription_Answer')";
        private const string XP_PROPERTY_ADDRESS = "id('BuildingLocation_Answer')";
        private const string XP_POL_START        = "id('PolicyStartDate_Answer')";
        private const string XP_POL_END          = "id('PolicyEndDate_Answer')";

        // Sum insured and excess values
        private const string XP_SUMINSURED_BUILDING               = "id('BuildingSumInsured_Answer')";
        private const string XP_SUMINSURED_CONTENTS               = "id('ContentsSumInsured_Answer')";
        private const string XP_EXCESS_BUILDING                   = "id('BuildingExcess_Answer')";
        private const string XP_EXCESS_CONTENTS                   = "id('ContentsExcess_Answer')";
        private const string XP_SUMINSURED_ACCIDENTAL_DAMAGE      = "id('AccidentalDamageCover_Answer')";
        private const string XP_EXCESS_ACCIDENTAL_DAMAGE          = "id('AccidentalDamageExcess_Answer')";
        private const string XP_ACCIDENTAL_DAMAGE_LBL             = "//label[@for='AccidentalDamageCover']";
        private const string XP_ACCIDENTAL_DAMAGE_EXCESS_LBL      = "//label[@for='AccidentalDamageExcess']";

        // PolicyHolder(s)
        // No unique ID or other attributes

        // Payment details
        private const string XP_PAYMENT_FREQ_LBL   = "//label[@for='PaymentFrequency']";
        private const string XP_PAYMENT_FREQ       = "id('PaymentFrequency_Answer')";
        private const string XP_PAYMENT_AMNT_LBL   = "//label[@for='AmountPaid']";
        private const string XP_PAYMENT_AMNT       = "id('AmountPaid_Answer')";
        private const string XP_RECEIPT_NUMBER     = "//div[@id='ReceiptNumber_Answer']";
        private const string XP_RECEIPT_NUMBER_LBL = "//label[@for='ReceiptNumber']";

        // Security Quote Popup
        private const string XP_SECURITY_QUOTE_POPUP_TITLE   = "id('email-securityQuote_wnd_title')";
        private const string XP_SECURITY_QUOTE_POPUP_TEXT    = "id('email-securityQuote')/div";
        private const string XP_SECURITY_QUOTE_POPUP_DISMISS = "id('SecurityQuoteCancelAction')";
        private const string XP_SECURITY_QUOTE_POPUP_ACCEPT  = "id('SecurityQuoteSubmitAction')";

        private const string XP_EMAIL_COC                    = "id('email-certOfCurrency')";

        private const string XP_COC_SUCCESS_POPUP_TITLE      = "id('homeCocDialog_wnd_title')";
        private const string XP_COC_SUCCESS_POPUP_TEXT       = "id('currencyCertifcate-confirm')";
        private const string XP_COC_SUCCESS_POPUP_CLOSE      = XP_COC_SUCCESS_POPUP_TITLE + "/../div[@class='cluetip-close']";
        #endregion

        #region Constants
        private const string SECURITY_QUOTE_MESSAGE = "An RAC monitored alarm will save you 25% on your RAC Contents Insurance every year. Would you like a free home security quote?";
        #endregion

        #region Settable properties and controls
        public string PolicyNumber => GetInnerText(XP_POLICY_NUM);

        public string Cover => GetInnerText(XP_COVER_TYPE);

        public string PropertyAddress => GetInnerText(XP_PROPERTY_ADDRESS);

        public string PolicyStartDate => GetInnerText(XP_POL_START);

        public string PolicyEndDate => GetInnerText(XP_POL_END);

        public int SumInsuredBuilding => DataHelper.ConvertMonetaryStringToInt(GetInnerText(XP_SUMINSURED_BUILDING));

        public int SumInsuredContents => DataHelper.ConvertMonetaryStringToInt(GetInnerText(XP_SUMINSURED_CONTENTS));

        public string SumInsuredAccidentalDamage => GetInnerText(XP_SUMINSURED_ACCIDENTAL_DAMAGE);

        public string ExcessBuilding => GetInnerText(XP_EXCESS_BUILDING).StripMoneyNotations();

        public string ExcessContents => GetInnerText(XP_EXCESS_CONTENTS).StripMoneyNotations();

        public string ExcessAccidentalDamage => GetInnerText(XP_EXCESS_ACCIDENTAL_DAMAGE).StripMoneyNotations();

        public PaymentFrequency PaymentFrequency => DataHelper.GetValueFromDescription<PaymentFrequency>(GetInnerText(XP_PAYMENT_FREQ));

        public decimal AmountPaid => decimal.Parse(GetInnerText(XP_PAYMENT_AMNT).StripMoneyNotations());

        public bool IsPaymentFrequencyLabelDisplayed => IsControlDisplayed(XP_PAYMENT_FREQ_LBL);

        public string PaymentFrequencyLabelText => GetInnerText(XP_PAYMENT_FREQ_LBL);

        public bool IsAmountLabelDisplayed => IsControlDisplayed(XP_PAYMENT_AMNT_LBL);

        public string AmountLabelText => GetInnerText(XP_PAYMENT_AMNT_LBL);

        public string ReceiptNumber => GetInnerText(XP_RECEIPT_NUMBER);

        public string ReceiptNumberLabelText => GetInnerText(XP_RECEIPT_NUMBER_LBL);

        public bool IsReceiptNumberLabelDisplayed => IsControlDisplayed(XP_RECEIPT_NUMBER_LBL);

        public bool IsReceiptNumberDisplayed => IsControlDisplayed(XP_RECEIPT_NUMBER);

        public string AccidentalDamageLabelText => GetInnerText(XP_ACCIDENTAL_DAMAGE_LBL);

        public bool IsAccidentalDamageLabelDisplayed => IsControlDisplayed(XP_ACCIDENTAL_DAMAGE_LBL);

        public bool IsAccidentalDamageSumInsuredDisplayed => IsControlDisplayed(XP_SUMINSURED_ACCIDENTAL_DAMAGE);

        public string AccidentalDamageExcessLabelText => GetInnerText(XP_ACCIDENTAL_DAMAGE_EXCESS_LBL);

        public bool IsAccidentalDamageExcessLabelDisplayed => IsControlDisplayed(XP_ACCIDENTAL_DAMAGE_EXCESS_LBL);

        public bool IsAccidentalDamageExcessDisplayed => IsControlDisplayed(XP_EXCESS_ACCIDENTAL_DAMAGE);
        #endregion

        public HomeQuoteConfirmation(Browser browser) : base(browser) { }

        override public bool IsDisplayed()
        {
            try
            {
                if (GetInnerText(XP_HEADER) != "Home insurance policy")
                {
                    Reporting.Log("Not the expected page header.");
                    return false;
                }
                GetElement(XP_POLICY_NUM);
                GetElement(XP_PAYMENT_AMNT);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Home Quote - Policy issued confirmation page");
            return true;
        }

        /// <summary>
        /// Will select the button to email the certificate of
        /// currency to the main policy holder.
        /// NOTE: There is no mechanism to dismiss this dialog due
        /// to layering of DOM elements.
        /// </summary>
        public void EmailCertificateOfCurrency()
        {
            ClickControl(XP_EMAIL_COC);

            // This dialog can take an excessive time to display.
            _driver.WaitForElementToBeVisible(By.XPath(XP_COC_SUCCESS_POPUP_TITLE), WaitTimes.T90SEC);

            System.Threading.Thread.Sleep(2000);
            ClickControl(XP_COC_SUCCESS_POPUP_CLOSE);
        }

        /// <summary>
        /// Under certain conditions, a pop-up will be shown on display of confirmation
        /// page, offering member a chance to request a RAC Home Security quote which
        /// can entitle them to a further discount on their contents insurance.
        /// </summary>
        /// <returns></returns>
        public bool CheckForRACAlarmQuoteOfferAndDismiss()
        {

            IWebElement dialogELement = null;
            bool isDialogFound = _driver.TryWaitForElementToBeVisible(By.XPath(XP_SECURITY_QUOTE_POPUP_TITLE), WaitTimes.T5SEC, out dialogELement);

            if (isDialogFound)
            {
                Reporting.AreEqual(SECURITY_QUOTE_MESSAGE, GetInnerText(XP_SECURITY_QUOTE_POPUP_TEXT).StripLineFeedAndCarriageReturns());

                ClickControl(XP_SECURITY_QUOTE_POPUP_DISMISS);
            }
            return isDialogFound;
        }

        public string VerifyReceiptNumberIsDisplayedCorrectly()
        {
            Regex receiptNumberRegex = new Regex(@"(?=1)\d{10}");
            var actualReceiptNumber  = ReceiptNumber;
            Match match              = receiptNumberRegex.Match(actualReceiptNumber);
            Reporting.IsTrue(match.Success, "Receipt number generated");
            Reporting.AreEqual("Receipt number:", ReceiptNumberLabelText, "Receipt number label as expected");
            Reporting.Log($"Receipt number: {actualReceiptNumber}", _browser.Driver.TakeSnapshot());

            return actualReceiptNumber;
        }
    }
}