using Rac.TestAutomation.Common;
using System;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.B2C
{
    public class PetQuote2Quote : BaseQuotePage2
    {
        #region XPATHS
        private const string QUOTE_BASE         = BASE + "//form//div[@id='productTabStrip']";

        // XP key references:
        private const string XP_EXCESS          = QUOTE_BASE + "//span[contains(@aria-owns,'PetQuoteProducts_0__ExcessData_ExcessLevelVORefId_listbox')]";
        private const string XP_EXCESS_DROPDOWN = "/html/body/div[@class='k-animation-container' and contains(@style,'visible')]//ul[@id='PetQuoteProducts_0__ExcessData_ExcessLevelVORefId_listbox']/li";

        private const string XP_INCLUDE_TLC  = "id('PetQuoteProducts_0__TenderLovingCare')";
        #endregion

        #region Settable properties and controls
        public string QuoteExcess
        {
            get => GetInnerText($"{XP_EXCESS}{XPEXT_DROPDOWN_VALUE}");
            set => WaitForSelectableAndPickFromDropdown(
                       XP_EXCESS, 
                       XP_EXCESS_DROPDOWN, 
                       DataHelper.AddCurrencyPrefixToAmount(amount: value, currencyPrefix: "$"));
        }
        #endregion

        public PetQuote2Quote(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_PAGE_HEADING);
                if (!heading.Text.ToLower().StartsWith("your quote:"))
                {
                    Reporting.Log("Wrong heading text for second page of Motor Quote process.");
                    return false;
                }
                GetElement($"{GetActiveTabXPath()}{XPR_ANNUAL_PRICE}");
                GetElement($"{GetActiveTabXPath()}{XPR_BUY_ONLINE_BTN}");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Pet Quote page 2 - Premium quote");
            return true;
        }

        override public string GetActiveTabXPath()
        {
            return QUOTE_BASE;
        }

        /// <summary>
        /// This method attempts to set TLC Cover checkbox to match the requested
        /// value.
        /// </summary>
        /// <param name="setAsChecked"></param>
        public void ClickTLCCoverCheckbox(bool setAsChecked)
        {
            try
            {
                var visibleControl = _driver.WaitForElementToBeVisible(By.XPath(XP_INCLUDE_TLC + "/.."), WaitTimes.T30SEC);
                var supportControl = GetElement(XP_INCLUDE_TLC);

                var checkAttr = !String.IsNullOrEmpty(supportControl.GetAttribute("checked"));

                if (setAsChecked != checkAttr)
                    visibleControl.Click();
            }
            catch (NoSuchElementException)
            {
                Reporting.Error("Failed to drive Add TLC Cover checkbox.");
            }
        }
    }
}
