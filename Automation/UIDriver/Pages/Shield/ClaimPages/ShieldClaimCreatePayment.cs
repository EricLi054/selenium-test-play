using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    public class ShieldClaimCreatePayment : BaseShieldPage
    {

        public const string CLAIM_PAYMENT_TYPE = "Cash Settlement";
        public const string CLAIM_PAY_TO= "Insured";
        public const string CSFS_REQUIRED = "Yes";
        public const string CONTROL_AMOUNT = "500";
        public const string PH_ITCE_PERCENTAGE = "0";
        public const string CLAIM_TRANSACTION_STATUS = "Approved";   

        #region XPATHS

        private const string XP_BUTTON_OK = "id('OK')";

        private const string XP_DIALOG = "id('BasicNotificationDialog')";
        private const string XP_BUTTON_DIALOG_OK = "id('DialogOK')";

        private const string XP_PAYMENT_TYPE = "id('s2id_claimTransactionTypeVO')";
        private const string XP_PAYMENT_TYPE_OPTION = "id('select2-results-2')/li/div";

        private const string XP_PAY_TO = "id('s2id_beneficiaryTypeVO')";
        private const string XP_PAY_TO_OPTION = "id('select2-results-3')/li/div";

        private const string XP_CSFS_REQUIRED = "id('s2id_csfsRequired')";
        private const string XP_CSFS_REQUIRED_OPTION = "id('select2-results-4')/li/div";

        private const string XP_CHECKBOX_CASH_SETTLEMENT_VALUE = "id('cashSettlement')";
        private const string XP_CHECKBOX_CASH_SETTLEMENT = "id('cashSettlementLabel')";

        private const string XP_CHECKBOX_REPAIRS_VALUE = "id('repairs')";
        private const string XP_CHECKBOX_REPAIRS = "id('repairsLabel')";

        private const string XP_CONTROL_AMOUNT = "id('controlAmount')";
        private const string XP_PH_ITCE_PERCENTAGE = "id('phItcePercentage')";

        private const string XP_BUTTON_GET_BREAKDOWN = "id('getBreakdown')";

        private const string XP_CLAIM_REMAINING_EXCESS = "id('totalAmountBeforeWriteOffWithoutDebtOffsetPresentationValue')";
        private const string XP_FENCING_PAYMENT = "//tr[contains(.,'Fencing')]/td/input[contains(@name,'creditAmount')]";

        private const string XP_CLAIM_TRANSACTION_STATUS = "id('s2id_claimTransactionStatusList')";
        private const string XP_CLAIM_TRANSACTION_STATUS_OPTION = "id('select2-drop')//li/div";
        
        private const string XP_BASIC_NOTIFICATION_DIALOG = "id('BasicNotificationDialog')";
        private const string XP_BASIC_NOTIFICATION_DIALOG_OK_BTN = "id('DialogOK')";

        private const string XP_CSFS_REASON = "id('s2id_csfsRequiredReason')";
        private const string XP_CSFS_REASON_OPTION = "id('select2-results-5')/li/div";
        
        private const string XP_BANK_ACCOUNT = "id('s2id_beneficiaryBankAccountVO')";
        private const string XP_BANK_ACCOUNT_OPTION = "id('select2-results-9')/li/div";

        private const string XP_CHECKBOX_EFT_CONFIRMATION_REQUIRED = "id('isEftConfirmationRequired')";
        private const string XP_CHECKBOX_EFT_CONFIRMATION_REQ_LABEL = "id('isEftConfirmationRequiredLabel')";

        #endregion

        #region Settable properties and controls

        public string PaymentType
        {
            get => GetInnerText($"{XP_PAYMENT_TYPE}//span[@class='select2-chosen']");

            set => WaitForSelectableAndPickFromDropdown(XP_PAYMENT_TYPE, XP_PAYMENT_TYPE_OPTION, value);
        }

        public string PayTo
        {
            get => GetInnerText($"{XP_PAY_TO}//span[@class='select2-chosen']");

            set => WaitForSelectableAndPickFromDropdown(XP_PAY_TO, XP_PAY_TO_OPTION, value);
        }

        public string CSFSRequired
        {
            get => GetInnerText($"{XP_CSFS_REQUIRED}//span[@class='select2-chosen']");

            set => WaitForSelectableAndPickFromDropdown(XP_CSFS_REQUIRED, XP_CSFS_REQUIRED_OPTION, value);
        }

        public bool CashSettlement
        {
            get => GetElement(XP_CHECKBOX_CASH_SETTLEMENT_VALUE).Selected;
        }

        public bool Repairs
        {
            get => GetElement(XP_CHECKBOX_REPAIRS_VALUE).Selected;
        }

        public string ControlAmount
        {
            get => GetElement(XP_CONTROL_AMOUNT).GetAttribute("Value");

            set => WaitForTextFieldAndEnterText(XP_CONTROL_AMOUNT, value, false);
        }

        public string PolicyHolderITCEPercentage
        {
            get => GetElement(XP_PH_ITCE_PERCENTAGE).GetAttribute("Value");

            set => WaitForTextFieldAndEnterText(XP_PH_ITCE_PERCENTAGE, value, false);
        }

        public string ClaimRemainingExcess
        {
            get => GetElement(XP_CLAIM_REMAINING_EXCESS).GetAttribute("Value").Replace("-","");
        }

        public string ClaimTransactionStatus
        {
            get => GetInnerText($"{XP_CLAIM_TRANSACTION_STATUS}//span[@class='select2-chosen']");

            set => WaitForSelectableAndPickFromDropdown(XP_CLAIM_TRANSACTION_STATUS, XP_CLAIM_TRANSACTION_STATUS_OPTION, value);
        }

        public string CSFSReasons
        {
            get => GetInnerText($"{XP_CSFS_REASON}//span[@class='select2-chosen']");

            set => WaitForSelectableAndPickFromDropdown(XP_CSFS_REASON, XP_CSFS_REASON_OPTION, value);
        }

        public string BankAccount
        {
            get => GetInnerText($"{XP_BANK_ACCOUNT}//span[@class='select2-chosen']");

            set => WaitForSelectableAndPickFromDropdown(XP_BANK_ACCOUNT, XP_BANK_ACCOUNT_OPTION, value);
        }

        public bool EFTConfirmationRequired
        { 
            get => GetElement(XP_CHECKBOX_EFT_CONFIRMATION_REQUIRED).Selected;
        }
        #endregion

        public ShieldClaimCreatePayment(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_BUTTON_OK);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        public void SelectCashSettlement()
        {
            if (!CashSettlement)
            {
                ClickControl(XP_CHECKBOX_CASH_SETTLEMENT);
            }
        }

        public void SelectRepairs()
        {
            if (!Repairs)
            {
                ClickControl(XP_CHECKBOX_REPAIRS);
            }
        }

        public void SelectEFTConfirmationRequired()
        {
            if (!EFTConfirmationRequired)
            {
                ClickControl(XP_CHECKBOX_EFT_CONFIRMATION_REQ_LABEL);
            }
        }

        public void ClickNext()
        {
            ClickControl(XP_BUTTON_OK);
            ClickDialogOk();
        }

        public void ClickDialogOk()
        {
            var dialog = _driver.WaitForElement(By.XPath(XP_DIALOG), WaitTimes.T30SEC);
            if (dialog.Displayed)
            {
                ClickControl(XP_BUTTON_DIALOG_OK);
            }
        }

        /// <summary>
        /// Calculate and the claim payment
        /// </summary>
        public void CalculateAndEnterClaimPaymentAmount()
        {
            var Amount = Convert.ToDecimal(ClaimRemainingExcess);
            var PaymentAmount = Convert.ToDecimal(CONTROL_AMOUNT);

            var TotalPayment = Convert.ToString(Amount + PaymentAmount);

            WaitForTextFieldAndEnterText(XP_FENCING_PAYMENT, TotalPayment, false);

        }

    }

    
}
