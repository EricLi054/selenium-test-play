using Rac.TestAutomation.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UIDriver.Pages.Shield
{
    public class AddLiabilityReserve : BaseShieldPage
    {
        // Static string to use for reason dropdown when adding a liability reserve.
        public const string AUTHORISED_QUOTE = "Authorised Quote";
        public const string CASH_SETTLEMENT_CREDIT_AMOUNT = "1000";
        public const string CASH_SETTLEMENT_REMARKS = "CSFS";

        private class XPath
        {
            public const string Header = "//div[contains(@id,'ScreenTitle')]";
            public class Input
            {
                public const string ReserveReason = "id('s2id_claimReserveReason')";
                public const string CashSettlementCredit = "id('IDITForm@reserveLineList|3@creditAmountVO@amount')";
                public const string CashSettlementRemarks = "id('IDITForm@reserveLineList|3@remarks')";
            }
            public class Dropdown
            {
                public const string ReserveReasonOptions = "id('select2-results-2')/li/div";
            }
            public class BreakdownTable
            {
                private const string RowByDescription = "id('idit-grid-table-IDITForm_at_reserveLineList_pipe_')//tr/td[@title='{0}']";

                public const string RowCreditValue = RowByDescription + "/following-sibling::td[@aria-describedby='idit-grid-table-IDITForm_at_reserveLineList_pipe__creditAmountVO@amount']/input";
                public const string RowDebitValue = RowByDescription + "/following-sibling::td[@aria-describedby='idit-grid-table-IDITForm_at_reserveLineList_pipe__debitAmountVO@amount']/input";
            }
            public class Button
            {
                public const string OK = "id('OK')";
                public const string Return = "id('Return')";
            }
        }

        #region Settable properties and controls
        public string Reason
        {
            get => GetInnerText($"{XPath.Input.ReserveReason}//span[@class='select2-chosen']");
            set => WaitForSelectableAndPickFromDropdown(XPath.Input.ReserveReason, XPath.Dropdown.ReserveReasonOptions, value);
        }

        public string CashSettlementAmount
        {
            get => GetInnerText(XPath.Input.CashSettlementCredit);
            set
            {
                GetElement(XPath.Input.CashSettlementCredit).SendKeys(value);
                GetElement(XPath.Input.CashSettlementCredit).SendKeys(Keys.Tab);
            }
        }

        #endregion

        public AddLiabilityReserve(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.Input.ReserveReason);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }        

        /// <summary>
        /// Click the 'OK' button to complete edits to Liability Reserves
        /// </summary>
        public void ClickOK() => ClickControl(XPath.Button.OK);
        
    }
}
