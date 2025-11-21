using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Linq;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class YourBankDetails : SparkPaymentPage
    {
        #region XPATHS
        public class XPath
        {
            public static readonly string SettlementAmount = "id('settlementSubTitle')";
            public static readonly string ChkDontHaveBankDetails = "//span[@data-testid='dontHaveBankDetails-checkbox']";
        }
        #endregion
        
        /// <summary>
        /// Take the Settlement Amount displayed on the Your bank details page
        /// at the end of the sub-title text and convert it ToDecimal for 
        /// comparison with the value returned by Shield Settlement Breakdown 
        /// Calculator.
        /// 
        /// If there is a trailing period after the amount, we remove it to allow
        /// the ToDecimal conversion.
        /// </summary>
        /// <returns>Settlement amount from text as a Decimal value</returns>
        public decimal SettlementAmountDisplayedOnPage
        {
            get
            {
                string settlementAmount = new String(GetElement(XPath.SettlementAmount).Text.
                    Where(x => x == '.' || Char.IsDigit(x)).ToArray());
                settlementAmount = settlementAmount.TrimEnd('.');
                return Convert.ToDecimal(settlementAmount);
            }
        }

        public YourBankDetails(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.SettlementAmount);
                GetElement(XPathPayment.Button.Submit);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Spark Fence Claim Page 6 - Your Bank Details");
            return true;
        }

        public void VerifySettlementAmountOnBankDetailsPage(decimal settlementAmount)
        {
            Reporting.AreEqual(settlementAmount, SettlementAmountDisplayedOnPage, 
                "expected value for Settlement Amount with displayed value on the Bank Details Page (as a decimal value)");
        }

        public void ClickDontHaveBankDetailsCheckbox()
        {
            ClickControl(XPath.ChkDontHaveBankDetails);
        }
    }
}
