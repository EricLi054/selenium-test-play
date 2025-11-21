using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace UIDriver.Pages.B2C
{
    public class HomeQuote2Quote : BaseQuotePage2
    {
        #region XPATHS
        private const string QUOTE_BASE = BASE + "//form//div[contains(@id,'productTabStrip') and contains(@class,'k-state-active')]";

        // XP key references:
        private const string XP_BUILDING_SI               = QUOTE_BASE + "//input[contains(@id,'_BuildingSumInsured_Value')]";
        private const string XP_CONTENTS_SI               = QUOTE_BASE + "//input[contains(@id,'_ContentsSumInsured_Value')]";
        private const string XP_RENTERS_CONTENTS          = QUOTE_BASE + "//span[contains(@aria-owns,'HomeQuoteProducts_1__RentersContentSumInsured_listbox')]";
        private const string XP_RENTERS_CONTENTS_DROPDOWN = "id('HomeQuoteProducts_1__RentersContentSumInsured_listbox')/li";

        private const string XP_BUILDING_EXCESS          = QUOTE_BASE + "//span[contains(@aria-owns,'_BuildingExcess_ExcessLevelVORefId_listbox')]";

        private const string XP_BUILDING_EXCESS_DROPDOWN = "//ul[contains(@id,'_BuildingExcess_ExcessLevelVORefId_listbox')]/li";
        private const string XP_CONTENTS_EXCESS_SETTABLE = QUOTE_BASE + "//span[contains(@aria-owns,'_ContentsExcess_ExcessLevelVORefId_listbox')]";
        private const string XP_CONTENTS_EXCESS_FIXED    = "id('HomeQuoteProducts_1__ContentsExcess_ExcessLevelIdValues_0__Excess_Answer')";
        private const string XP_CONTENTS_EXCESS_DROPDOWN = "//ul[contains(@id,'_ContentsExcess_ExcessLevelVORefId_listbox') and (@aria-hidden='false')]/li";
        
        // Unspecified and specified personal valuables and contents:
        private const string XP_ADD_PERSONAL_VALUABLES_GROUP            = "id('add-personal-valuables-button')";
        private const string XP_ADD_UNSPECIFIED_PERSONAL_COVER          = "//span[contains(@aria-owns,'__UnspecifiedItemsId_listbox')]";
        private const string XP_ADD_UNSPECIFIED_PERSONAL_COVER_DROPDOWN = "//ul[contains(@id,'__UnspecifiedItemsId_listbox') and (@aria-hidden='false')]/li";

        private const string XP_ADD_SPECIFIED_PERSONAL_ITEM = "id('add-specified-valuables-button')";
        private const string XP_SPECIFIED_ITEM_ROW          = "//div[contains(@id,'__PersonalValuables') and contains(@class,'row')]";
        private const string XP_SPECIFIED_ITEM_TOTAL        = "id('specified-valuables-total')";

        private const string XP_ADD_SPECIFIED_CONTENTS_GROUP      = "id('add-specified-contents-section-button')";
        private const string XP_ADD_SPECIFIED_CONTENTS_ITEM       = "id('add-specified-contents-button')";
        private const string XP_SPECIFIED_CONTENTS_ROW            = "//div[contains(@id,'__SpecifiedContents_') and contains(@class,'row')]";
        private const string XP_SPECIFIED_CONTENTS_TOTAL          = "id('specified-contents-total')";

        private const string XPR_VALUABLE_CATEGORY          = "//span[contains(@aria-owns,'__ValuablesCategory_listbox')]";
        private const string XP_VALUABLE_CATEGORY_DROPDOWN  = "//ul[contains(@id,'__ValuablesCategory_listbox') and @aria-hidden='false']/li";
        private const string XPR_VALUABLE_DESCRIPTION       = "//input[@data-role='textbox']";
        private const string XPR_VALUABLE_VALUE             = "//div[contains(@data-wrapper-for,'__ItemValue')]/..";

        private const string XP_PERSONAL_VALUABLE_UPDATE_QUOTE    = "id('pv-update-quote-button')";
        private const string XP_SPECIFIED_CONTENTS_UPDATE_QUOTE   = "id('sc-update-quote-button')";

        private const string XP_ADD_ACCIDENTAL_DAMAGE = "//input[contains(@id, '_AccidentalDamageIsSelected')]";
        private static class XPATHS
        {
            public static class Tooltips
            {
                public static readonly string Heading               = "//*[@class='cluetip-title ui-widget-header ui-cluetip-header']";
                public static readonly string Message               = "//*[@id='cluetip']/div[@class='cluetip-outer']/div";
                public static readonly string Close                 = "//div[@class='cluetip-close']/a/i[@class='icon-remove']";
                public static readonly string BuildingIcon          = QUOTE_BASE + "//a[@rev='Building excess']";
                public static readonly string ContentsIcon          = QUOTE_BASE + "//a[@rev='Contents excess']";
            }
            public static class QuoteInfoBox
            {
                public static readonly string Text                  = QUOTE_BASE + "//div[@class='info-box quote home-excess-info-box']";
            }
            public static class ExcessSection
            {
                public static readonly string Section               = BASE + "//*[@id='home-excess-container']";
                public static readonly string Heading               = Section + "//div[@class='body-text-heading']";

                public static readonly string StandardExcess        = Section + "//div[@class='row summary-excess-container']/div";
                public static readonly string Conditional           = Section + "//div[@class='row hhbac-excess-summary summary-excess']//span[@data-restrict='forRenters']";
                public static readonly string IfOtherExcess         = Section + "//div[@class='row fine-print-space'][1]";
                public static readonly string Ending                = Section + "//div[@class='row fine-print-space'][2]";

                public static class AdditionalExcess
                {
                    public static readonly string Row1              = Section + "//div[@class='row']";
                    public static readonly string Earthquake        = Section + "//div[@class='row building-excess summary-excess'][1]";
                    public static readonly string Flood             = Section + "//div[@class='row building-excess summary-excess'][2]";
                    public static readonly string AccidentalDamage  = Section + "//div[@class='row building-excess summary-excess'][3]";
                    public static readonly string PersonalValuables = Section + "//div[@class='row pv-excess summary-excess']";
                    public static readonly string MaliciousDamage   = Section + "//div[@class='row landlord-excess summary-excess']";
                }
            }
        }
        #endregion

        #region Constants
        private static class Constants
        {
            public static class Tooltips
            {
                public static class Excess
                {
                    public static readonly string Message               = "The excess is the amount you will need to pay towards settlement of any claim. Read the Excesses section for more information.";
                    public static readonly string BuildingTitle         = "Building excess";
                    public static readonly string ContentsTitle         = "Contents excess";
                }
            }
            // This is the field below the SI and excess dropdowns
            public static class QuoteInfoBox
            {
                public static readonly string GeneralMessage            = "Read Excesses section for more information.";
                public static readonly string LandLord                  = "The standard basic excess is $500 for building and $500 for contents.\r\n\r\n\r\nIf you accept an excess other than the standard basic excess, you may pay more at claim time.\r\n\r\nRead Excesses section for more information.";
                public static readonly string ContentsOnly              = "Not sure of the sum insured? Try our calculator.\r\n\r\nRead Excesses section for more information.";
                public static readonly string TenantContents            = "Not sure of the sum insured? Try our calculator.\r\n\r\nNeed a higher sum insured? View the Contents tab at the top of the page.\r\n\r\nThere's a fixed $200 excess for Basic Contents.\r\n\r\nRead Excesses section for more information.";
            }
            // This is the Excesses section near product disclosures
            public static class ExcessSection
            {
                public static class General
                {
                    public static readonly string StandardExcess        = "The excess is the amount you will need to pay towards settlement of any claim.\r\n\r\nIf you make a combined building and contents claim, the higher of the two excesses will apply.\r\n\r\nIf you adjust your excess, your premium will change.";
                    public static readonly string Conditional           = "If you make a combined building and contents claim, the higher of the two excesses will apply.";
                }
                public static class LandLord
                {
                    public static readonly string StandardExcess        = "The excess is the amount you will need to pay towards settlement of any claim.\r\n\r\nIf you make a combined building and contents claim, the higher of the two excesses will apply.\r\n\r\nIf you adjust your excess, your premium will change.";
                    public static readonly string Conditional           = "If you make a combined building and contents claim, the higher of the two excesses will apply.";
                }
                public static class ContentsOnly
                {
                    public static readonly string StandardExcess        = "The standard basic excess is $500 per policy. This is the standard but you may have changed it in your quote.";
                }
                public static class Additional
                {
                    public static readonly string Row1                  = "Extra excesses may apply:";
                    public static readonly string Earthquake            = "Earthquake excess$500";
                    public static readonly string Flood                 = "Flood excess$200";
                    public static readonly string AccidentalDamage      = "Accidental Damage excess$500";
                    public static readonly string MaliciousDamage       = "Malicious damage/theft excess$300";
                    public static readonly string PersonalValuables     = "Personal valuables excess$200";
                }

                public static readonly string IfOtherExcess             = "Special excess: will be stated in your policy documents if applicable.";
                public static readonly string Ending                    = "See the Premium, Excess and Discount Guide for more information.";
            }
        }
        #endregion
        #region Settable properties and controls
        public bool IsBuildingSumInsuredDisplayed => _driver.TryFindElement(
                                                          By.XPath(XP_BUILDING_SI),
                                                          out IWebElement control);

        public bool IsContentsSumInsuredDisplayed => _driver.TryFindElement(
                                                          By.XPath(XP_CONTENTS_SI),
                                                          out IWebElement control);

        public bool IsBuildingExcessDisplayed => _driver.TryFindElement(
                                                      By.XPath(XP_BUILDING_EXCESS),
                                                      out IWebElement control);

        public bool IsContentsExcessDisplayed => _driver.TryFindElement(
                                                      By.XPath(XP_CONTENTS_EXCESS_SETTABLE),
                                                      out IWebElement controlVariable) ||
                                                 _driver.TryFindElement(
                                                      By.XPath(XP_CONTENTS_EXCESS_FIXED), 
                                                     out IWebElement controlStatic);


        public bool ArePersonalValuablesDisplayed => _driver.TryFindElement(
                                                          By.XPath(XP_ADD_PERSONAL_VALUABLES_GROUP),
                                                                   out IWebElement personalValuablesCollapsible);

        public bool IsPersonalValuablesAdded(QuoteHome quoteDetails)
        {
            return quoteDetails.SpecifiedValuablesOutside != null && quoteDetails.SpecifiedValuablesOutside.Count > 0;
        }

        public bool AreSpecifiedContentsDisplayed => _driver.TryFindElement(
                                                          By.XPath(XP_ADD_SPECIFIED_CONTENTS_GROUP),
                                                          out IWebElement specifiedContentsControl);

        public int QuoteBuildingSumInsured
        {
            get => int.Parse(GetValue(XP_BUILDING_SI));
            set => WaitForTextFieldAndEnterText(XP_BUILDING_SI, value.ToString(), false);

        }

        public int QuoteContentsSumInsured
        {
            get
            {
                // Most home policy occupancy types allow free text entry
                // of contents value.
                IWebElement contentsFreeText = null;
                if (_driver.TryFindElement(By.XPath(XP_CONTENTS_SI), out contentsFreeText))
                    return int.Parse(GetValue(XP_CONTENTS_SI));
                // But Tenants are given a dropdown selection fixed values to pick from.
                else
                {
                    return DataHelper.ConvertMonetaryStringToInt(GetInnerText(XP_RENTERS_CONTENTS));
                }
            }
            set
            {
                // Most home policy occupancy types allow free text entry
                // of contents value.
                IWebElement contentsFreeText = null;
                if (_driver.TryFindElement(By.XPath(XP_CONTENTS_SI), out contentsFreeText))
                    WaitForTextFieldAndEnterText(XP_CONTENTS_SI, value.ToString(), false);

                // But Tenants are given a dropdown selection fixed values to pick from.
                else
                    WaitForSelectableAndPickByTyping(XP_RENTERS_CONTENTS, DataHelper.ConvertIntToMonetaryString(value));
            }
        }

        public string QuoteBuildingExcess
        {
            get => GetInnerText($"{XP_BUILDING_EXCESS}{XPEXT_DROPDOWN_VALUE}").StripMoneyNotations();
            set => WaitForSelectableAndPickFromDropdown(XP_BUILDING_EXCESS, 
                                                        XP_BUILDING_EXCESS_DROPDOWN, 
                                                        DataHelper.ConvertIntToMonetaryString(
                                                            amount: int.Parse(value)));
        }

        public string QuoteContentsExcess
        {
            get
            {
                // Most home policy occupancy types use a dropdown control for
                // contents excess
                IWebElement contentsExcessSettable = null;
                var excessValue = "";
                if (_driver.TryFindElement(By.XPath(XP_CONTENTS_EXCESS_SETTABLE), out contentsExcessSettable))
                    excessValue = GetInnerText($"{XP_CONTENTS_EXCESS_SETTABLE}{XPEXT_DROPDOWN_VALUE}");

                // But Tenants have a fixed excess that can't be changed, and the control is different.
                else
                    excessValue = GetInnerText(XP_CONTENTS_EXCESS_FIXED);

                return excessValue.StripMoneyNotations();
            }
            set
            {
                WaitForSelectableAndPickFromDropdown(XP_CONTENTS_EXCESS_SETTABLE, XP_CONTENTS_EXCESS_DROPDOWN, DataHelper.ConvertIntToMonetaryString(amount: int.Parse(value)));
            }
        }

        public void VerifyExcessMessage(QuoteHome quoteDetails)
        {
            HomeCover homeCover = quoteDetails.HasBuildingAndContentsCover ? HomeCover.BuildingAndContents : HomeCover.ContentsOnly;

            VerifyTooltip(quoteDetails);
            VerifyQuoteInfoBox(quoteDetails, homeCover);
            VerifyExcessSection(quoteDetails, homeCover);
        }

        private void VerifyTooltip(QuoteHome quoteDetails)
        {
            if (quoteDetails.HasContentsCover)
            {
                ClickControl(XPATHS.Tooltips.ContentsIcon);
                Reporting.AreEqual(Constants.Tooltips.Excess.ContentsTitle, GetInnerText(XPATHS.Tooltips.Heading), "Tooltip heading is correct");
                Reporting.AreEqual(Constants.Tooltips.Excess.Message, GetInnerText(XPATHS.Tooltips.Message), $"Tooltip message contains the expected text");
                ClickControl(XPATHS.Tooltips.Close);
            }
            if (!quoteDetails.HasContentsOnlyCover)
            {
                ClickControl(XPATHS.Tooltips.BuildingIcon);
                Reporting.AreEqual(Constants.Tooltips.Excess.BuildingTitle, GetInnerText(XPATHS.Tooltips.Heading), "Tooltip heading is correct");
                Reporting.AreEqual(Constants.Tooltips.Excess.Message, GetInnerText(XPATHS.Tooltips.Message), $"Tooltip message contains the expected text");
                ClickControl(XPATHS.Tooltips.Close);
            }
        }

        private void VerifyQuoteInfoBox(QuoteHome quoteDetails, HomeCover homeCover)
        {
            if (IsControlDisplayed(XPATHS.QuoteInfoBox.Text))
            {
                switch (quoteDetails.Occupancy)
                {
                    case HomeOccupancy.HolidayHome:
                        // Handle the case for HolidayHome when a test scenario becomes available
                        break;
                    case HomeOccupancy.InvestmentProperty:
                        if (homeCover == HomeCover.LandlordsBuildingAndContents)
                        {
                            Reporting.AreEqual(Constants.QuoteInfoBox.LandLord, GetInnerText(XPATHS.QuoteInfoBox.Text), "Message of Quote excess box contains the expected text");
                        }
                        break;
                    case HomeOccupancy.OwnerOccupied:
                        if (homeCover == HomeCover.BuildingAndContents)
                        {
                            Reporting.AreEqual(Constants.QuoteInfoBox.GeneralMessage, GetInnerText(XPATHS.QuoteInfoBox.Text), "Message of Quote excess box contains the expected text");
                        }
                        if (homeCover == HomeCover.ContentsOnly)
                        {
                            Reporting.AreEqual(Constants.QuoteInfoBox.ContentsOnly, GetInnerText(XPATHS.QuoteInfoBox.Text), "Message of Quote excess box contains the expected text");
                        }
                        break;
                    case HomeOccupancy.Tenant:
                        if (homeCover == HomeCover.ContentsOnly)
                        {
                            Reporting.AreEqual(Constants.QuoteInfoBox.TenantContents, GetInnerText(XPATHS.QuoteInfoBox.Text), "Message of Quote excess box contains the expected text");
                        }
                        break;
                    default:
                        Reporting.Error("Unknown occupancy type");
                        break;
                }
            }
        }

        private void VerifyExcessSection(QuoteHome quoteDetails, HomeCover homeCover)
        {
            switch (quoteDetails.Occupancy)
            {
                case HomeOccupancy.HolidayHome:
                    // Handle the case for HolidayHome when a test scenario becomes available
                    break;
                case HomeOccupancy.InvestmentProperty:
                    if (homeCover == HomeCover.LandlordsBuildingAndContents)
                    {
                        Reporting.AreEqual(Constants.ExcessSection.General.StandardExcess, GetInnerText(XPATHS.ExcessSection.StandardExcess), "Standard excess contains the expected text");
                        Reporting.AreEqual(Constants.ExcessSection.General.Conditional, GetInnerText(XPATHS.ExcessSection.Conditional), "Conditional excess contains the expected text");
                        Reporting.AreEqual(Constants.ExcessSection.Additional.Row1, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.Row1), "Additional row1 contains the expected text");
                        AssertText(Constants.ExcessSection.Additional.Earthquake, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.Earthquake), "Earthquake excess contains the expected text");
                        AssertText(Constants.ExcessSection.Additional.Flood, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.Flood), "Flood excess contains the expected text");
                        if (IsPersonalValuablesAdded(quoteDetails))
                        { AssertText(Constants.ExcessSection.Additional.PersonalValuables, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.PersonalValuables), "Personal valuables excess contains the expected text"); }
                        AssertText(Constants.ExcessSection.Additional.MaliciousDamage, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.MaliciousDamage), "Malicious damage excess contains the expected text");
                        Reporting.AreEqual(Constants.ExcessSection.IfOtherExcess, GetInnerText(XPATHS.ExcessSection.IfOtherExcess), "If other excess contains the expected text");
                        Reporting.AreEqual(Constants.ExcessSection.Ending, GetInnerText(XPATHS.ExcessSection.Ending), "Ending contains the expected text");
                    }
                    break;
                case HomeOccupancy.OwnerOccupied:
                    if (homeCover == HomeCover.BuildingAndContents)
                    {
                        Reporting.AreEqual(Constants.ExcessSection.General.StandardExcess, GetInnerText(XPATHS.ExcessSection.StandardExcess), "Standard excess contains the expected text");
                        Reporting.AreEqual(Constants.ExcessSection.Additional.Row1, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.Row1), "Additional row1 contains the expected text");
                        AssertText(Constants.ExcessSection.Additional.Earthquake, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.Earthquake), "Earthquake excess contains the expected text");
                        AssertText(Constants.ExcessSection.Additional.Flood, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.Flood), "Flood excess contains the expected text");
                        AssertText(Constants.ExcessSection.Additional.AccidentalDamage, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.AccidentalDamage), "Accidental damage excess contains the expected text");
                        if (IsPersonalValuablesAdded(quoteDetails))
                        { AssertText(Constants.ExcessSection.Additional.PersonalValuables, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.PersonalValuables), "Personal valuables excess contains the expected text"); }
                        Reporting.AreEqual(Constants.ExcessSection.IfOtherExcess, GetInnerText(XPATHS.ExcessSection.IfOtherExcess), "If other excess contains the expected text");
                        Reporting.AreEqual(Constants.ExcessSection.Ending, GetInnerText(XPATHS.ExcessSection.Ending), "Ending contains the expected text");
                    }
                    if (homeCover == HomeCover.ContentsOnly)
                    {
                        Reporting.AreEqual(Constants.ExcessSection.General.StandardExcess, GetInnerText(XPATHS.ExcessSection.StandardExcess), "Standard excess contains the expected text");
                        Reporting.AreEqual(Constants.ExcessSection.Additional.Row1, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.Row1), "Additional row1 contains the expected text");
                        AssertText(Constants.ExcessSection.Additional.Earthquake, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.Earthquake), "Earthquake excess contains the expected text");
                        AssertText(Constants.ExcessSection.Additional.Flood, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.Flood), "Flood excess contains the expected text");
                        AssertText(Constants.ExcessSection.Additional.AccidentalDamage, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.AccidentalDamage), "Accidental damage excess contains the expected text");
                        Reporting.AreEqual(Constants.ExcessSection.IfOtherExcess, GetInnerText(XPATHS.ExcessSection.IfOtherExcess), "If other excess contains the expected text");
                        Reporting.AreEqual(Constants.ExcessSection.Ending, GetInnerText(XPATHS.ExcessSection.Ending), "Ending contains the expected text");
                    }
                    break;
                case HomeOccupancy.Tenant:
                    if (homeCover == HomeCover.ContentsOnly)
                    {
                        Reporting.AreEqual(Constants.ExcessSection.ContentsOnly.StandardExcess, GetInnerText(XPATHS.ExcessSection.StandardExcess), "Standard excess contains the expected text");
                        Reporting.AreEqual(Constants.ExcessSection.Additional.Row1, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.Row1), "Additional row1 contains the expected text");
                        if (IsPersonalValuablesAdded(quoteDetails))
                        { AssertText(Constants.ExcessSection.Additional.PersonalValuables, GetInnerText(XPATHS.ExcessSection.AdditionalExcess.PersonalValuables), "Personal valuables excess contains the expected text"); }
                        Reporting.AreEqual(Constants.ExcessSection.IfOtherExcess, GetInnerText(XPATHS.ExcessSection.IfOtherExcess), "If other excess contains the expected text");
                        Reporting.AreEqual(Constants.ExcessSection.Ending, GetInnerText(XPATHS.ExcessSection.Ending), "Ending contains the expected text");
                    }
                    break;
                default:
                    Reporting.Error("Unknown occupancy type");
                    break;
            }
        }

        public UnspecifiedPersonalValuables UnspecifiedValuablesCover
        {
            get
            {
                var cover = UnspecifiedPersonalValuables.None;

                if (IsControlDisplayed(XP_ADD_PERSONAL_VALUABLES_GROUP))
                    return cover;

                var dropdownText = GetInnerText(XP_ADD_UNSPECIFIED_PERSONAL_COVER);

                try
                {
                    cover = DataHelper.GetValueFromDescription<UnspecifiedPersonalValuables>(dropdownText);
                }
                catch (ArgumentException ex)
                {
                    Reporting.Error($"Error while parsing dropdown text: {ex}");
                }

                return cover;
            }
            set
            {
                if (IsControlDisplayed(XP_ADD_PERSONAL_VALUABLES_GROUP))
                    ClickControl(XP_ADD_PERSONAL_VALUABLES_GROUP);

                WaitForSelectableAndPickFromDropdown(XP_ADD_UNSPECIFIED_PERSONAL_COVER,
                                                     XP_ADD_UNSPECIFIED_PERSONAL_COVER_DROPDOWN,
                                                     value.GetDescription());

                ClickControl(XP_PERSONAL_VALUABLE_UPDATE_QUOTE);
            }
        }
        #endregion

        public HomeQuote2Quote(Browser browser) : base(browser) { }

        override public bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_PAGE_HEADING);
                if (!heading.Text.ToLower().StartsWith("your quote:"))
                {
                    Reporting.Log("Wrong heading text for second page of Home Quote process.");
                    return false;
                }
                GetElement($"{GetActiveTabXPath()}{XPR_ANNUAL_PRICE}");
                GetElement($"{GetActiveTabXPath()}{XPR_BUY_ONLINE_BTN}");
            }
            catch (NoSuchElementException)
            {
                Reporting.Log("Element not found when checking if Home Quote page 2 is displayed.");
                return false;
            }
            Reporting.LogPageChange("Home Quote page 2 - Premium quote");
            return true;
        }

        override public string GetActiveTabXPath()
        {
            return QUOTE_BASE;
        }


        public void AddSpecifiedPersonalValuables(List<ContentItem> specifiedValuables)
        {
            if (IsControlDisplayed(XP_ADD_PERSONAL_VALUABLES_GROUP))
                ClickControl(XP_ADD_PERSONAL_VALUABLES_GROUP);

            foreach (var valuable in specifiedValuables)
            {
                var rowIndex = AddNewSpecifiedPersonalValuablesRow();

                WaitForSelectableAndPickFromDropdown($"{XP_SPECIFIED_ITEM_ROW}[{rowIndex}]{XPR_VALUABLE_CATEGORY}",
                                                     XP_VALUABLE_CATEGORY_DROPDOWN,
                                                     valuable.CategoryB2CStringForSpecifiedValuable);

                WaitForTextFieldAndEnterText($"{XP_SPECIFIED_ITEM_ROW}[{rowIndex}]{XPR_VALUABLE_DESCRIPTION}", valuable.Description);

                // This block is quite messy, as B2C uses two input elements here, and swaps them when the user
                // clicks on them. So the first one is read-only and shows the current value. When the user clicks
                // the second one is displayed and is editable. Hence the logic below to send a click to the base DIV
                // and to then send the keypresses to a specific INPUT object.
                ClickControl($"{XP_SPECIFIED_ITEM_ROW}[{rowIndex}]//div[contains(@data-wrapper-for,'__ItemValue')]");
                Thread.Sleep(2000);
                SendKeyPressesToField($"{XP_SPECIFIED_ITEM_ROW}[{rowIndex}]//input[@data-role='numerictextbox']", valuable.Value.ToString());
            }

            ClickControl(XP_PERSONAL_VALUABLE_UPDATE_QUOTE);
        }

        public void AddSpecifiedContents(List<ContentItem> specifiedContents)
        {
            foreach (var valuable in specifiedContents)
            {
                var rowIndex = AddNewSpecifiedContentsRow();

                WaitForSelectableAndPickFromDropdown($"{XP_SPECIFIED_CONTENTS_ROW}[{rowIndex}]{XPR_VALUABLE_CATEGORY}",
                                                     XP_VALUABLE_CATEGORY_DROPDOWN,
                                                     valuable.CategoryB2CStringForSpecifiedContents);

                WaitForTextFieldAndEnterText($"{XP_SPECIFIED_CONTENTS_ROW}[{rowIndex}]{XPR_VALUABLE_DESCRIPTION}", valuable.Description);

                // This block is quite messy, as B2C uses two input elements here, and swaps them when the user
                // clicks on them. So the first one is read-only and shows the current value. When the user clicks
                // the second one is displayed and is editable. Hence the logic below to send a click to the base DIV
                // and to then send the keypresses to a specific INPUT object.
                ClickControl($"{XP_SPECIFIED_CONTENTS_ROW}[{rowIndex}]//div[contains(@data-wrapper-for,'__ItemValue')]");
                Thread.Sleep(2000);
                SendKeyPressesToField($"{XP_SPECIFIED_CONTENTS_ROW}[{rowIndex}]//input[@data-role='numerictextbox']", valuable.Value.ToString());
            }

            ClickControl(XP_SPECIFIED_CONTENTS_UPDATE_QUOTE);
        }

        public List<ContentItem> GetSpecifiedContentItems()
        {
            var specifiedContents = new List<ContentItem>();

            if (IsControlDisplayed(XP_ADD_SPECIFIED_CONTENTS_GROUP))
            {
                // Then no content items have been entered.
                return specifiedContents;
            }

            var uiSpecifiedContentsRowCount = GetSpecifiedContentsRowCount();

            for (int rowIndex = 1; rowIndex <= uiSpecifiedContentsRowCount; rowIndex++)
            {
                var categoryText = GetInnerText($"{XP_SPECIFIED_CONTENTS_ROW}[{rowIndex}]{XPR_VALUABLE_CATEGORY}");

                var item = new ContentItem()
                {
                    Category = (int)SpecifiedContentsDisplayedText.First(x => x.Value.TextB2C == categoryText).Key,
                    Description = GetValue($"{XP_SPECIFIED_CONTENTS_ROW}[{rowIndex}]{XPR_VALUABLE_DESCRIPTION}"),
                    Value = int.Parse(GetElement($"{XP_SPECIFIED_CONTENTS_ROW}[{rowIndex}]//div[contains(@data-wrapper-for,'__ItemValue')]//input[@type='text']").GetAttribute("aria-valuenow"))
                };
                specifiedContents.Add(item);
            }

            return specifiedContents;
        }

        public int GetSpecifiedContentsTotalValue()
        {
            return int.Parse(GetInnerText(XP_SPECIFIED_CONTENTS_TOTAL).StripMoneyNotations());
        }

        public QuoteData GetQuoteCoverValuesAndPremium(QuoteHome testData)
        {
            var quoteValues = new QuoteData()
            {
                QuoteNumber = QuoteReference,
                AnnualPremium = QuotePriceAnnual,
                MonthlyPremium = QuotePriceMonthly,
                HomeInsuredValueAndExcess = new HomeQuoteCoverValues()
            };

            if (testData.BuildingValue.HasValue)
            {
                quoteValues.HomeInsuredValueAndExcess.ExcessBuilding = QuoteBuildingExcess;
                quoteValues.HomeInsuredValueAndExcess.SumInsuredBuilding = QuoteBuildingSumInsured;
            }
            if (testData.ContentsValue.HasValue)
            {
                quoteValues.HomeInsuredValueAndExcess.ExcessContents = QuoteContentsExcess;
                quoteValues.HomeInsuredValueAndExcess.SumInsuredContents = QuoteContentsSumInsured;
            }

            return quoteValues;
        }

        /// <summary>
        /// This method attempts to set the "Add Accidental Damage?" checkbox to match the requested value.
        /// </summary>
        /// <param name="setAsChecked"></param>
        public void TickAccidentalDamage(bool setAsChecked)
        {
            try
            {
                var visibleControl = _driver.WaitForElementToBeVisible(By.XPath(XP_ADD_ACCIDENTAL_DAMAGE + "/.."), WaitTimes.T30SEC);
                var supportControl = GetElement(XP_ADD_ACCIDENTAL_DAMAGE);
                var checkAttr = !string.IsNullOrEmpty(supportControl.GetAttribute("checked"));
                if (setAsChecked != checkAttr)
                {
                    visibleControl.Click();
                    Reporting.Log("\"Add Accidental Damage?\" checkbox has been selected.", _browser.Driver.TakeSnapshot());
                }
            }
            catch 
            { Reporting.Error("Failed to drive \"Add Accidental Damage?\" checkbox."); }
        }

        private int GetSpecifiedPersonalValuablesRowCount()
        {
            return _driver.FindElements(By.XPath(XP_SPECIFIED_ITEM_ROW)).Count;
        }

        private int GetSpecifiedContentsRowCount()
        {
            return _driver.FindElements(By.XPath(XP_SPECIFIED_CONTENTS_ROW)).Count;
        }

        private int AddNewSpecifiedPersonalValuablesRow()
        {
            var startingRowCount = GetSpecifiedPersonalValuablesRowCount();
            var rowCountAfterAdd = 0;

            ClickControl(XP_ADD_SPECIFIED_PERSONAL_ITEM);

            var endTime = DateTime.Now.AddSeconds(WaitTimes.T5SEC);
            do
            {
                try
                {
                    if (GetSpecifiedPersonalValuablesRowCount() > startingRowCount)
                    {
                        var newRowCategoryDropDown = GetElement($"{XP_SPECIFIED_ITEM_ROW}[{startingRowCount + 1}]{XPR_VALUABLE_CATEGORY}");
                        if (newRowCategoryDropDown.Displayed)
                        {
                            rowCountAfterAdd = GetSpecifiedPersonalValuablesRowCount();
                        }
                    }
                }
                catch (NoSuchElementException ex) { Reporting.Log($"Unable to find element: {ex}"); }
                Thread.Sleep(500);
            } while ((DateTime.Now < endTime) && (rowCountAfterAdd == 0));

            Reporting.IsTrue(rowCountAfterAdd > 0, "that new Specified Personal Valuables row has rendered on screen.");

            return rowCountAfterAdd;
        }

        private int AddNewSpecifiedContentsRow()
        {
            var startingRowCount = GetSpecifiedContentsRowCount();
            var rowCountAfterAdd = 0;

            // For the very first item use the group control
            if (IsControlDisplayed(XP_ADD_SPECIFIED_CONTENTS_GROUP))
                ClickControl(XP_ADD_SPECIFIED_CONTENTS_GROUP);
            // Only need to use "add item" after the first row.
            else
                ClickControl(XP_ADD_SPECIFIED_CONTENTS_ITEM);

            var endTime = DateTime.Now.AddSeconds(WaitTimes.T5SEC);
            do
            {
                try
                {
                    if (GetSpecifiedContentsRowCount() > startingRowCount)
                    {
                        var newRowCategoryDropDown = GetElement($"{XP_SPECIFIED_CONTENTS_ROW}[{startingRowCount + 1}]{XPR_VALUABLE_CATEGORY}");
                        if (newRowCategoryDropDown.Displayed)
                        {
                            rowCountAfterAdd = GetSpecifiedContentsRowCount();
                        }
                    }
                }
                catch (NoSuchElementException)
                {
                    Reporting.Log("Element not found when trying to add new Specified Contents row.");
                }
                Thread.Sleep(500);
            } while ((DateTime.Now < endTime) && (rowCountAfterAdd == 0));

            Reporting.IsTrue(rowCountAfterAdd > 0, "that new Specified Contents row has rendered on screen.");

            return rowCountAfterAdd;
        }

        private void AssertText(string expected, string actual, string logMessage)
        {
            Reporting.AreEqual(expected.StripLineFeedAndCarriageReturns(replaceWithWhiteSpace: false).Replace(" ", ""),
                               actual.StripLineFeedAndCarriageReturns(replaceWithWhiteSpace: false).Replace(" ", ""),
                               logMessage);
        }
    }
}
