using Rac.TestAutomation.Common;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace UIDriver.Pages.B2C
{
    public class MotorQuote2Quote : BaseQuotePage2
    {
        public enum COVER_TAB {
            [Description("Full Cover")]
            COVER_COMP,
            [Description("Third Party Fire and Theft")]
            COVER_TPFT,
            [Description("Third Party Only")]
            COVER_TPP };

        #region XPATHS
        private const string QUOTE_BASE = BASE + "//form//div[@id='productTabStrip']";

        // XP key references:
        private const string XP_TAB_COMP     = QUOTE_BASE + "/ul/li[@id='MFCO']";
        private const string XP_TAB_TPFT     = QUOTE_BASE + "/ul/li[@id='MTFT']";
        private const string XP_TAB_TPP      = QUOTE_BASE + "/ul/li[@id='MTPO']";

        private const string ACTIVE_PANEL_X     = QUOTE_BASE + "/div[@id='productTabStrip-{0}']";
        new private const string XPR_ANNUAL_PRICE   = "//div[@class='your-quote']/div[@class='roadside-annual-premium-container']//div[contains(@class,'price-amount')]";
        new private const string XPR_MONTHLY_PRICE  = "//div[@class='your-quote']/div[@class='table-padding']//td[contains(@class,'right-align')]";
        new private const string XPR_BUY_ONLINE_BTN = "//div[@class='your-quote']//button[@class='btn primary buy-button animate-chevron-right']";
        new private const string XPR_START_DATE     = "//div[@class='customise-your-quote-container']//input[contains(@id,'CoverStartDate')]";

        private const string XPR_ROADSIDE_PRICE = "//div[@class='your-quote']//tr[@class='roadside-annual-price']/td[2]/div[1]";
        private const string XPR_COMBINED_PRICE = "//div[@class='your-quote']/div[@id='combined-online-price-container']//span[@class='data-price-amount']";

        private const string XPR_EXCESS         = "//div[@class='customise-your-quote-container']//span[contains(@aria-owns,'__ExcessData_ExcessLevelVORefId_listbox')]";
        private const string XP_EXCESS_DROPDOWN = "/html/body/div[@class='k-animation-container' and contains(@style,'visible')]//ul[contains(@id,'ExcessLevelVORefId')]/li";
        private const string XP_AGREED_VALUE = "//div[contains(@data-wrapper-for,'__SumInsured')]//div[contains(@class,'answer')]";

        private const string XP_INCLUDE_HIRECAR_AFTER_ACCIDENT = "//input[@id='MotorQuoteProducts_0__HireCarAfterAccident']";
        private const string XP_INCLUDE_NO_CLAIM_BONUS_PROTECTION = "//input[@id='MotorQuoteProducts_0__ProtectNoClaimBonus']";
        private const string XPR_INCLUDE_ROADSIDE = "//input[contains(@id,'Roadside_')]";

        private const string XP_NO_CLAIM_BONUS_TEXT = "//div[@class='no-claim-bonus-text']";
        #endregion

        #region Settable properties and controls
        new public decimal QuotePriceAnnual => decimal.Parse(
                                                   GetInnerText(GetActivePanelXPath() + 
                                                       XPR_ANNUAL_PRICE).StripMoneyNotations());

        new public decimal QuotePriceMonthly => decimal.Parse(
                                                    GetInnerText(GetActivePanelXPath() + 
                                                        XPR_MONTHLY_PRICE).StripMoneyNotations());

        /// <summary>
        /// boolean to indicate if premium quote is showing line-item
        /// for Roadside assistance charge
        /// </summary>
        public bool IsRoadsidePriceShown
        {
            get
            {
                var isShown = false;
                IWebElement element = null;
                if (_driver.TryFindElement(By.XPath(GetActivePanelXPath() + XPR_ROADSIDE_PRICE), out element))
                {
                    isShown = (element != null);
                }

                return isShown;
            }
        }

        /// <summary>
        /// Fetch the price being charged for included roadside assistance
        /// </summary>
        public decimal QuoteRoadsidePrice
        {
            get => decimal.Parse(GetInnerText(GetActivePanelXPath() + XPR_ROADSIDE_PRICE).StripMoneyNotations());
        }

        /// <summary>
        /// If Roadside assistance membership is added to quote, then
        /// this will fetch the combined price which has that value 
        /// added to policy premium.
        /// </summary>
        public decimal QuotePriceCombined
        {
            get => decimal.Parse(GetInnerText(GetActivePanelXPath() + XPR_COMBINED_PRICE).StripMoneyNotations());
        }

        public new DateTime QuoteStartDate
        {
            get
            {
                var dateJsonString = GetAttribute(GetActivePanelXPath() + XPR_START_DATE, "data-currentvalue");
                var dateValue = JsonConvert.DeserializeObject<DateNumeric>(dateJsonString);
                return dateValue.ToDateTime();
            }
            set
            {
                var endtime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
                var success = false;

                do
                {
                    try
                    {
                        if (!_driver.TryWaitForElementToBeVisible(By.XPath(CALENDAR_BASE), WaitTimes.T5SEC, out IWebElement datePicker))
                        {
                            ClickControl($"{GetActivePanelXPath()}{XPR_START_DATE}", skipJSScrollLogic: true);
                            _driver.WaitForElementToBeVisible(By.XPath(CALENDAR_BASE), WaitTimes.T5SEC);
                        }

                        CalendarPickMonthYear(value);
                        CalendarPickDay(value);
                        success = true;
                    }
                    catch (NoSuchElementException ex) { Reporting.Log($"Unable to find element: {ex}"); }
                    Thread.Sleep(500);
                } while (DateTime.Now < endtime && !success);

                if (!success)
                { Reporting.Error("Failed to set desired Start Date."); }

                // Wait for the property to refresh, to show it has been selected properly.
                endtime = DateTime.Now.AddSeconds(WaitTimes.T10SEC);
                do
                {
                    try
                    {
                        if (QuoteStartDate.Date == value.Date) break;
                    }
                    catch (NoSuchElementException ex) { Reporting.Log($"Unable to find element: {ex}"); }
                    Thread.Sleep(500);
                } while (DateTime.Now < endtime);
            }
        }

        public string QuoteExcess
        {
            get
            {
                return GetInnerText($"{GetActivePanelXPath()}{XPR_EXCESS}{XPEXT_DROPDOWN_VALUE}");
            }
            set
            {
                var root = GetActivePanelXPath();

                WaitForSelectableAndPickFromDropdown($"{root}{XPR_EXCESS}", XP_EXCESS_DROPDOWN, DataHelper.AddCurrencyPrefixToAmount(amount: value, currencyPrefix: "$"));
            }
        }

        #endregion

        public MotorQuote2Quote(Browser browser) : base(browser) { }

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
                GetElement(XP_TAB_TPP);
                GetElement(GetActivePanelXPath() + XPR_ANNUAL_PRICE);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Motor Quote page 2 - Premium quote");
            return true;
        }

        override public string GetActiveTabXPath()
        {
            return QUOTE_BASE;
        }

        public void ClickTab(MotorCovers cover)
        {
            var element = string.Empty;
            switch(cover)
            {
                case MotorCovers.MFCO:
                    element = XP_TAB_COMP;
                    break;
                case MotorCovers.TFT:
                    element = XP_TAB_TPFT;
                    break;
                case MotorCovers.TPO:
                    element = XP_TAB_TPP;
                    break;
                default:
                    break;
            }

            var tabControl = GetElement(element);
            if (!tabControl.GetAttribute("class").Contains("k-state-active"))
            {
                var endtime = DateTime.Now.AddSeconds(WaitTimes.T60SEC);

                do
                {
                    tabControl.Click();
                    Thread.Sleep(2000);
                } while (!tabControl.GetAttribute("class").Contains("k-state-active") && DateTime.Now <= endtime);

                if (DateTime.Now > endtime) Reporting.Error($"Clicked {MotorCoverNameMappings[cover].TextB2C} tab, but did not see UI respond within a reasonable time.");
            }
            // Brief sleep to ensure previous tab content has cleared, before
            // we begin wait for new tab content to load. This is because
            // not all tabs may have gotten their rating information before
            // we attempt to change selected cover.
            Thread.Sleep(1000);
            WaitForPage();
        }

        public COVER_TAB CurrentCover()
        {
            var tab = COVER_TAB.COVER_COMP;
            IWebElement element;

            if (_driver.TryFindElement(By.XPath(XP_TAB_COMP), out element) && element.GetAttribute("class").Contains("k-state-active"))
            {
                tab = COVER_TAB.COVER_COMP;
            }
            else if (_driver.TryFindElement(By.XPath(XP_TAB_TPFT), out element) && element.GetAttribute("class").Contains("k-state-active"))
            {
                tab = COVER_TAB.COVER_TPFT;
            }
            else if (_driver.TryFindElement(By.XPath(XP_TAB_TPP), out element) && element.GetAttribute("class").Contains("k-state-active"))
            {
                tab = COVER_TAB.COVER_TPP;
            }

            return tab;
        }

        new public void ClickBuyOnlineButton()
        {
            Reporting.Log($"active quote panel XPath: {GetActivePanelXPath()}");
            ClickControl($"{GetActivePanelXPath()}{XPR_BUY_ONLINE_BTN}");
        }

        /// <summary>
        /// This method attempts to set the Sum Insured amount to match the requested value,
        /// using CoverType to determine which level of Motor Vehicle Insurance is involved
        /// and so which tab is expected active.
        /// </summary>
        /// <param name="InsuredVariance"></param>
        public void ActionSumInsuredVariance(int InsuredVariance)
        {
            try
            {
                Reporting.Log($"Amending Sum Insured");
                var xPathSupportControl = $"{GetActivePanelXPath()}{XP_AGREED_VALUE}";
                var visibleControl = _driver.WaitForElementToBeVisible(By.XPath(xPathSupportControl + "/.."), WaitTimes.T30SEC);
                var supportControl = GetElement(xPathSupportControl);
                var ExistingSumInsured = int.Parse(GetAttribute($"{xPathSupportControl}//input[@type='text']", "aria-valuenow"));
                Reporting.Log($"ExistingSumInsured = {ExistingSumInsured}");
                var desiredSumToInsure = (int)((1 + ((decimal)InsuredVariance / 100)) * ExistingSumInsured);
                Reporting.Log($"Calculated desiredSumToInsure as {desiredSumToInsure}, attempting to set that sum insured");
                // NOTE - Below is same as implementation in ChangeWhereIKeepMyCar.cs
                // This block is quite messy, as B2C uses two input elements here, and swaps them when the user
                // clicks on them. So the first one is read-only and shows the current value. When the user clicks
                // the second one is displayed and is editable. Hence the logic below to send a click to the base DIV
                // and to then send the keypresses to a specific INPUT object.
                ClickControl(xPathSupportControl);
                Thread.Sleep(2000);
                SendKeyPressesAfterClearingExistingTextInField($"{xPathSupportControl}//input[@data-role='numerictextbox']", $"{desiredSumToInsure}{Keys.Return}");
            }
            catch (NoSuchElementException)
            {
                Reporting.Error("Failed to drive Change to Sum Insured value.");
            }
        }

        /// <summary>
        /// This method attempts to set the Roadside Assistance checkbox to match the requested
        /// value, using CoverType to determine which level of Motor Vehicle Insurance is involved
        /// and so which tab is expected active.
        /// </summary>
        /// <param name="setAsChecked">Should the checkbox be checked?</param>
        /// <param name="cover">The Motor Vehicle Cover Type specified for this quote/policy.</param>
        public void ClickRoadsideAssistance(bool setAsChecked)
        {
            try
            {
                var xPathSupportControl = $"{GetActivePanelXPath()}{XPR_INCLUDE_ROADSIDE}";
                var visibleControl = _driver.WaitForElementToBeVisible(By.XPath(xPathSupportControl + "/.."), WaitTimes.T30SEC);
                var supportControl = GetElement(xPathSupportControl);
                var checkAttr = !String.IsNullOrEmpty(supportControl.GetAttribute("checked"));
                if (setAsChecked != checkAttr)
                {
                    visibleControl.Click();
                    Reporting.Log("Selecting Roadside Assistance Bundling checkbox to activate it.", _browser.Driver.TakeSnapshot());
                }
            }
            catch
            {
                Reporting.Error("Failed to drive Roadside assistance checkbox.");
            }
        }

        /// <summary>
        /// Verifies that the NCB protection label and answer are not shown when ExcessAndNcb toggle is enabled.
        /// </summary>
        public void VerifyAddNCBProtectionIsNotDisplayed()
        {
                Reporting.IsFalse(_driver.TryFindElement(By.XPath(XP_INCLUDE_NO_CLAIM_BONUS_PROTECTION), out IWebElement noClaimBonusProtectionCheckbox),
                    "No Claim Bonus Protection Checkbox not be shown for Comprehensive cover type when the Excess changes are enabled");
            Reporting.IsFalse(_driver.TryFindElement(By.XPath(XP_NO_CLAIM_BONUS_TEXT), out IWebElement noClaimBonusText),
                "No Claim Bonus text not be shown for Comprehensive cover type when the Excess changes are enabled");
         }

        /// <summary>
        /// Assuming that the user is on Full Comprehensive cover tab, this method
        /// attempts to set the Hire Car After Accident checkbox to match the requested
        /// value.
        /// </summary>
        /// <param name="setAsChecked"></param>
        public void ClickHireCarAfterAccident(bool setAsChecked)
        {
            try
            {
                var visibleControl = _driver.WaitForElementToBeVisible(By.XPath(XP_INCLUDE_HIRECAR_AFTER_ACCIDENT + "/.."), WaitTimes.T30SEC);
                var supportControl = GetElement(XP_INCLUDE_HIRECAR_AFTER_ACCIDENT);
                var checkAttr = !String.IsNullOrEmpty(supportControl.GetAttribute("checked"));
                if (setAsChecked != checkAttr)
                {
                    visibleControl.Click();
                    Reporting.Log("Selecting Hire Car After Accident checkbox to activate it.", _browser.Driver.TakeSnapshot());
                }
            }
            catch
            {
                Reporting.Error("Failed to drive Hire Car After Accident checkbox.");
            }
        }
        private string GetActivePanelXPath()
        {
            var         xp   = string.Empty;
            int         iTab = 0;
            var         found = false;
            IWebElement element;

            if (_driver.TryFindElement(By.XPath(XP_TAB_COMP), out element)) {
                iTab++;
                if (element.GetAttribute("class").Contains("k-state-active"))  found = true;
            }
            if (!found && _driver.TryFindElement(By.XPath(XP_TAB_TPFT), out element)) {
                iTab++;
                if (element.GetAttribute("class").Contains("k-state-active")) found = true;
            }
            if (!found && _driver.TryFindElement(By.XPath(XP_TAB_TPP), out element))
            {
                iTab++;
                if (element.GetAttribute("class").Contains("k-state-active")) found = true;
            }

            if (found) xp = string.Format(ACTIVE_PANEL_X, iTab);
            return xp;
        }
    }
}
