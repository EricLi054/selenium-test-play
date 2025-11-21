using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Claims
{
    public class ClaimHomePage2DamageEventDetails : BasePage
    {
        #region XPATHS
        private const string XP_HEADING      = "//span[@class='action-heading']";
        private const string XP_CLAIM_NUMBER = "id('quote-number')";
        private const string XPR_YES         = "//span[text()='Yes']/..";
        private const string XPR_NO          = "//span[text()='No']/..";
        private const string XPR_CHECKMARK   = "//i[@class='icon-check']";

        // XPath components shared between stolen and storm damaged contents item entry
        private const string XPR_ITEM_DESC           = "//div[contains(@class,'item-description')]//input";
        private const string XPR_ITEM_VALUE_STATIC   = "//div[contains(@data-wrapper-for,'__ItemValue')]";
        private const string XPR_ITEM_VALUE_EDITABLE = "//input[@data-role='numerictextbox']";

        // Fence damage details:
        private const string XP_DAMAGE_FENCE_YN = "id('HomeDamageDetails_FenceAndGarageDetails_IsDamageAtFence')";

        private const string XP_DAMAGE_DESCRIPTION = "id('HomeDamageDetails_ClaimDetails_DamageOccurredHow')";
        private const string XP_FENCE_TYPE         = "//span[@aria-owns='HomeDamageDetails_FenceAndGarageDetails_FenceTypeDamaged_listbox']";
        private const string XP_FENCE_TYPE_OPTIONS = "id('HomeDamageDetails_FenceAndGarageDetails_FenceTypeDamaged_listbox')/li";

        private const string XP_FENCE_SIDE_LEFT  = "id('HomeDamageDetails_FenceAndGarageDetails_FenceSidePropertyFacing_0_Label')";
        private const string XP_FENCE_SIDE_RIGHT = "id('HomeDamageDetails_FenceAndGarageDetails_FenceSidePropertyFacing_1_Label')";
        private const string XP_FENCE_SIDE_FRONT = "id('HomeDamageDetails_FenceAndGarageDetails_FenceSidePropertyFacing_2_Label')";
        private const string XP_FENCE_SIDE_REAR  = "id('HomeDamageDetails_FenceAndGarageDetails_FenceSidePropertyFacing_3_Label')";

        private const string XP_AREA_PROTECTED_YN     = "id('HomeDamageDetails_FenceAndGarageDetails_IsAreaSafeAndProtected')";
        private const string XP_REQUIRE_TEMP_FENCE_YN = "id('HomeDamageDetails_FenceAndGarageDetails_IsTemporaryFenceRequired')";
        private const string XP_FENCE_DIVIDING_YN     = "id('HomeDamageDetails_FenceAndGarageDetails_IsFenceDividingPropertyFromAnother')";
        private const string XP_TEMP_FENCE_REASON     = "id('HomeDamageDetails_FenceAndGarageDetails_PleaseSpecifyWhy')";
        private const string XP_FENCE_LENGTH_METRES   = "id('HomeDamageDetails_FenceAndGarageDetails_FenceMetresDamaged')";
        private const string XP_FENCE_LENGTH_PANELS   = "id('HomeDamageDetails_FenceAndGarageDetails_FencePanelsDamaged')";
        private const string XP_FENCE_PAINTED_PANELS  = "id('HomeDamageDetails_FenceAndGarageDetails_FencePanelsPainted')";

        private const string XP_FENCE_OTHER_DESCRIPTION = "id('HomeDamageDetails_FenceAndGarageDetails_FenceTypeDamagedOther')";

        // Theft damage details
        private const string XP_LOCATION_ITEMS_STOLEN_FROM = "id('HomeDamageDetails_ClaimDetails_TheftItemsLocation')//span[text()='{0}']";
        private const string XP_ENTRY_POINT_KNOWN_YN       = "id('HomeDamageDetails_DamageDetails_OffenderEntryPoint')";
        private const string XP_ENTRY_POINT_SECURE_YN      = "id('HomeDamageDetails_DamageDetails_OffenderEntryPointRepaired')";
        private const string XP_ENTRY_METHOD_TEXT          = "id('HomeDamageDetails_DamageDetails_OffenderHomeEntryMethod')";
        private const string XP_STOLEN_ITEMS_ADD_ROW_BTN   = "id('addItemButton')";
        private const string XP_STOLEN_ITEM_ROW            = "id('item_{0}_row')";

        // Home damage details
        private const string XP_HOME_INHABITABLE_YN = "id('HomeDamageDetails_ClaimDetails_IsHomeInhabitable')";
        private const string XP_GLASS_DAMAGE_YN = "id('HomeDamageDetails_DamageDetails_IsThereGlassDamage')";
        private const string XP_FLOORING_DAMAGED_YN = "id('HomeDamageDetails_DamageDetails_IsTimberOrLaminateFlooringDamaged')";
        private const string XP_DAMAGE_GARAGE_DOOR_YN = "id('HomeDamageDetails_FenceAndGarageDetails_IsDamageAtGarageDoor')";

        // Storm damaged contents
        private const string XP_DAMAGED_ITEMS_ADD_ROW_BTN = "id('addItemStorm')";
        private const string XP_DAMAGED_ITEM_ROW          = "id('stormItemsTable')/div[@id='item_{0}_row']";

        // Personal Valuables items
        public static class XPath
        {
            public static class PersonalValuables
            {
                public static readonly string AddItemButton = "id('add-specified-valuables-button')";
                public static readonly string ItemRow = "//div[contains(@id,'_UnspecifiedPersonalValuables') and contains(@class,'row')]";

                public static readonly string SelectCategory = "//span[@aria-owns = 'HomeDamageDetails_ClaimDetails_PersonalValuables_UnspecifiedPersonalValuables_0__ValuablesCategoryId_listbox']";
                public static readonly string DropdownOption = "//ul[contains(@id,'__ValuablesCategoryId_listbox') and @aria-hidden='false']/li";
                public static readonly string EnterItemDescription = "//input[@placeholder = 'Enter item description']";
                public static readonly string EnterMarketPrice = "//input[@placeholder = '$0'][1]";

                public static readonly string CheckBox = "//span[@class='cb-text']";
                public static readonly string ItemUnspecifiedField = "//label[@for='ShowUnspecifiedContainer']";
            }
        }

        private const string XP_CONTINUE_PAGE_BTN = "//button[starts-with(@id,'accordion_0')]";

        // Upload documents:
        private const string XP_UPLOAD_FILE_DROP = "id('UploadSupportingDocsFileUploadInfo')";
        private const string XP_SUBMIT_PAGE_BTN  = "//button[contains(@id,'submit-action')]";
        #endregion

        #region Settable properties and controls

        private FenceType FenceMaterial
        {
            get => DataHelper.GetValueFromDescription<FenceType>(GetInnerText($"{XP_FENCE_TYPE}"));
            set => WaitForSelectableAndPickFromDropdown(XP_FENCE_TYPE, XP_FENCE_TYPE_OPTIONS, FenceTypeNames[value].TextB2C);
        }

        private bool IsDividingFence
        {
            get => GetBinaryToggleState(XP_FENCE_DIVIDING_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_FENCE_DIVIDING_YN, XPR_YES, XPR_NO, value);
        }

        private bool IsAreaSafeAndProtected
        {
            get => GetBinaryToggleState(XP_AREA_PROTECTED_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_AREA_PROTECTED_YN, XPR_YES, XPR_NO, value);
        }

        private string TemporaryFenceReason
        {
            get
            {
                throw new NotImplementedException("Have not implemented reading of shadown DOM to fetch reason text.");
            }
            set
            {
                ClickBinaryToggle(XP_REQUIRE_TEMP_FENCE_YN, XPR_YES, XPR_NO, !string.IsNullOrEmpty(value));
                if (!string.IsNullOrEmpty(value))
                    WaitForTextFieldAndEnterText(XP_TEMP_FENCE_REASON, value, false);
            }
        }

        private bool IsHomeInhabitable
        {
            get => GetBinaryToggleState(XP_HOME_INHABITABLE_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_HOME_INHABITABLE_YN, XPR_YES, XPR_NO, value);
        }

        private GlassDamage IsGlassDamaged
        {
            get
            {
                if (GetBinaryToggleState(XP_GLASS_DAMAGE_YN, XPR_YES, XPR_NO))
                    throw new NotImplementedException("Placeholder for glass damage TRUE.");
                return GlassDamage.NoDamage;
            }
            set
            {
                if (value != GlassDamage.NoDamage)
                    throw new NotImplementedException("Placeholder for glass damage is TRUE.");
                ClickBinaryToggle(XP_GLASS_DAMAGE_YN, XPR_YES, XPR_NO, value != GlassDamage.NoDamage);
            }
        }

        private bool IsTimberOrLaminateFlooringDamaged
        {
            get => GetBinaryToggleState(XP_FLOORING_DAMAGED_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_FLOORING_DAMAGED_YN, XPR_YES, XPR_NO, value);
        }

        private GarageDoorDamage IsGarageDoorDamaged
        {
            get
            {
                if (GetBinaryToggleState(XP_DAMAGE_GARAGE_DOOR_YN, XPR_YES, XPR_NO))
                {
                    throw new NotImplementedException("Placeholder for garage door damage TRUE.");
                }
                return GarageDoorDamage.NoDamage;
            }
            set
            {
                if (value != GarageDoorDamage.NoDamage)
                    throw new NotImplementedException("Placeholder for garage door damage is TRUE.");
                ClickBinaryToggle(XP_DAMAGE_GARAGE_DOOR_YN, XPR_YES, XPR_NO, value != GarageDoorDamage.NoDamage);
            }
        }

        #endregion

        public ClaimHomePage2DamageEventDetails(Browser browser) : base(browser)
        {
        }

        public override bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XP_HEADING);
                GetElement(XP_CLAIM_NUMBER);
                // First accordion button.
                // No other first accordion controls are listed here as they
                // vary significantly based on the Damage Type that the member
                // is claiming for.
                GetElement(XP_CONTINUE_PAGE_BTN);

                Reporting.LogPageChange("Home claim event damage details page");
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }

        public void FillEventDetails(ClaimHome claimData)
        {
            WaitForTextFieldAndEnterText(XP_DAMAGE_DESCRIPTION, claimData.AccountOfEvent, false);            
            FillHomeDamageDetails(claimData);
            FillFenceDamageDetails(claimData);
            FillTheftDamageDetails(claimData);
        }

        public void SubmitPage2ClaimForm()
        {
            ClickControl(XP_SUBMIT_PAGE_BTN);
        }

        private void SetLengthOfFenceDamage(ClaimHome claimData)
        {
            switch(claimData.FenceDamage.FenceMaterial)
            {
                case FenceType.Colorbond:
                    WaitForTextFieldAndEnterText(XP_FENCE_LENGTH_METRES, claimData.FenceDamage.MetresPanelsDamaged.ToString(), false);
                    break;
                case FenceType.Asbestos:
                case FenceType.Hardifence:
                case FenceType.SuperSix:
                    WaitForTextFieldAndEnterText(XP_FENCE_LENGTH_PANELS, claimData.FenceDamage.MetresPanelsDamaged.ToString(), false);
                    WaitForTextFieldAndEnterText(XP_FENCE_PAINTED_PANELS, claimData.FenceDamage.MetresPanelsPainted.ToString(), false);
                    break;
                default:
                    break;
            }
        }

        private void FillFenceDamageDetails(ClaimHome claimData)
        {
            // These claims do not get fence questionnaires even for Building damage
            // And if the claim is for contents only, there also will not be a prompt
            // for fence damages
            if (claimData.DamageType == HomeClaimDamageType.EscapeOfLiquid ||
                claimData.DamageType == HomeClaimDamageType.ElectricMotorBurnout ||
                claimData.DamageType == HomeClaimDamageType.SpoiltFood ||
                claimData.DamageType == HomeClaimDamageType.Glass ||
                claimData.DamageType == HomeClaimDamageType.AccidentalDamage ||
                claimData.DamagedCovers == AffectedCovers.ContentsOnly)
            {
                return;
            }

            ClickBinaryToggle(XP_DAMAGE_FENCE_YN, XPR_YES, XPR_NO, claimData.FenceDamage != null);

            if (claimData.FenceDamage != null)
            {
                FenceMaterial = claimData.FenceDamage.FenceMaterial;
                IsDividingFence = claimData.FenceDamage.IsDividingFence;
                SetLengthOfFenceDamage(claimData);

                SetCheckBox(XP_FENCE_SIDE_LEFT, claimData.FenceDamage.AffectedBoundaryLeft);
                SetCheckBox(XP_FENCE_SIDE_RIGHT, claimData.FenceDamage.AffectedBoundaryRight);
                SetCheckBox(XP_FENCE_SIDE_FRONT, claimData.FenceDamage.AffectedBoundaryFront);
                SetCheckBox(XP_FENCE_SIDE_REAR, claimData.FenceDamage.AffectedBoundaryRear);

                IsAreaSafeAndProtected = claimData.FenceDamage.IsAreaSafe;
                if (!IsAreaSafeAndProtected)
                    TemporaryFenceReason = claimData.FenceDamage.TemporaryFenceRequired;
            }
            Reporting.Log("Fence damage details entered", _driver.TakeSnapshot());
        }

        private void FillHomeDamageDetails(ClaimHome claimData)
        {
            switch (claimData.DamageType)
            {
                case HomeClaimDamageType.StormAndTempest:
                    if (claimData.DamagedCovers != AffectedCovers.ContentsOnly)
                    {
                        IsHomeInhabitable = claimData.IsHomeInhabitable;
                        IsGlassDamaged = claimData.IsGlassDamaged;
                        IsTimberOrLaminateFlooringDamaged = false;
                        IsGarageDoorDamaged = claimData.IsGarageDoorDamaged;
                    }
                    if (claimData.DamagedCovers != AffectedCovers.BuildingOnly)
                    {
                        FillStormDamagedItems(claimData.ContentsDamage.StormDamagedItems);
                    }
                    break;
                case HomeClaimDamageType.EscapeOfLiquid:
                    if (claimData.DamagedCovers != AffectedCovers.ContentsOnly)
                    {
                        IsHomeInhabitable = claimData.IsHomeInhabitable;
                        IsTimberOrLaminateFlooringDamaged = false;
                    }
                    break;
                case HomeClaimDamageType.Theft:
                    if (claimData.DamagedCovers != AffectedCovers.ContentsOnly)
                    {
                        IsHomeInhabitable = claimData.IsHomeInhabitable;
                        IsGlassDamaged = claimData.IsGlassDamaged;
                        IsTimberOrLaminateFlooringDamaged = false;
                        IsGarageDoorDamaged = claimData.IsGarageDoorDamaged;
                    }
                    break;
                case HomeClaimDamageType.AccidentalDamage:
                    if (claimData.DamagedCovers == AffectedCovers.SpecifiedPersonalValuablesOnly)
                    {
                        FillSpecifiedPersonalValuables(claimData);
                    }
                    else if (claimData.DamagedCovers == AffectedCovers.UnspecifiedPersonalValuablesOnly)
                    {
                        FillUnspecifiedPersonalValuables(claimData);
                    }
                    break;
                default:
                    Reporting.Error($"Test data error: A damage type was used that needs to be added to this page object for handling home damage questionnaires.");
                    break;
            }
        }
        
        private void FillTheftDamageDetails(ClaimHome claimData)
        {
            if (claimData.TheftDamage == null || claimData.DamageType == HomeClaimDamageType.AccidentalDamage)
            {
                return;
            }

            ClickControl(string.Format(XP_LOCATION_ITEMS_STOLEN_FROM, claimData.TheftDamage.LocationOfStolenItems.GetDescription()));

            for(int i = 0; i < claimData.TheftDamage.StolenItems.Count; i++)
            {
                if (i > 0)
                {
                    ClickControl(XP_STOLEN_ITEMS_ADD_ROW_BTN);
                }
                var rowXPath = string.Format(XP_STOLEN_ITEM_ROW, i.ToString());
                WaitForTextFieldAndEnterText($"{rowXPath}{XPR_ITEM_DESC}", claimData.TheftDamage.StolenItems[i].Description, hasTypeAhead: false);
                // This block is quite messy, as B2C uses two input elements here, and swaps them when the user
                // clicks on them. So the first one is read-only and shows the current value. When the user clicks
                // the second one is displayed and is editable. Hence the logic below to send a click to the base DIV
                // and to then send the keypresses to a specific INPUT object.
                ClickControl($"{rowXPath}{XPR_ITEM_VALUE_STATIC}");
                Thread.Sleep(1000);
                SendKeyPressesToField($"{rowXPath}{XPR_ITEM_VALUE_EDITABLE}", claimData.TheftDamage.StolenItems[i].Value.ToString());

            }

            ClickBinaryToggle(XP_ENTRY_POINT_KNOWN_YN, XPR_YES, XPR_NO, claimData.TheftDamage.IsEntryPointKnown);
            if (claimData.TheftDamage.IsEntryPointKnown)
            {
                WaitForTextFieldAndEnterText(XP_ENTRY_METHOD_TEXT, claimData.TheftDamage.OffenderEntryDescription, hasTypeAhead: false);
                ClickBinaryToggle(XP_ENTRY_POINT_SECURE_YN, XPR_YES, XPR_NO, claimData.TheftDamage.IsEntryPointSecuredOrRepaired);
            }

            Reporting.Log("Theft damage details entered", _driver.TakeSnapshot());
        }

        private void FillStormDamagedItems(List<ContentItem> items)
        {
            Reporting.IsNotNull(items, "For storm damage affecting contents, the claimant must enter some contents items.");
            for (int i = 0; i < items.Count; i++)
            {
                if (i > 0)
                {
                    ClickControl(XP_DAMAGED_ITEMS_ADD_ROW_BTN);
                }
                var rowXPath = string.Format(XP_DAMAGED_ITEM_ROW, i.ToString());
                WaitForTextFieldAndEnterText($"{rowXPath}{XPR_ITEM_DESC}", items[i].Description, hasTypeAhead: false);
                // This block is quite messy, as B2C uses two input elements here, and swaps them when the user
                // clicks on them. So the first one is read-only and shows the current value. When the user clicks
                // the second one is displayed and is editable. Hence the logic below to send a click to the base DIV
                // and to then send the keypresses to a specific INPUT object.
                ClickControl($"{rowXPath}{XPR_ITEM_VALUE_STATIC}");
                Thread.Sleep(1000);
                SendKeyPressesToField($"{rowXPath}{XPR_ITEM_VALUE_EDITABLE}", items[i].Value.ToString());
            }
        }

        private void FillSpecifiedPersonalValuables(ClaimHome claimHome)
        {
            if (claimHome.DamagedCovers == AffectedCovers.SpecifiedPersonalValuablesOnly)
            {
                string policyNumber = claimHome.PolicyDetails.PolicyNumber;
                var assetItems = claimHome.GetAllPersonalValuables();
                var damageItems = assetItems.PickRandom();

                string dynamicXPath = $"{XPath.PersonalValuables.CheckBox}[contains(normalize-space(), '{damageItems.Description}')]";
                ClickControl(dynamicXPath);
            }
            else
            {
                Reporting.Error($"The policy does not have {AffectedCovers.SpecifiedPersonalValuablesOnly.GetDescription()} cover");
            }
        }

        private void FillUnspecifiedPersonalValuables(ClaimHome claimData)
        {
            List<ContentItem> valuablesToAdd = claimData.UnspecifiedValuablesOutside;

            if (valuablesToAdd != null)
            {
                if (IsControlDisplayed(XPath.PersonalValuables.AddItemButton))
                {
                    ClickControl(XPath.PersonalValuables.AddItemButton);
                }

                foreach (var valuable in valuablesToAdd)
                {
                    var rowIndex = AddNewSpecifiedPersonalValuablesRow();
                    WaitForSelectableAndPickFromDropdown($"{XPath.PersonalValuables.ItemRow}[{rowIndex}]{XPath.PersonalValuables.SelectCategory}",
                                                         XPath.PersonalValuables.DropdownOption,
                                                         valuable.CategoryB2CStringForSpecifiedValuable);

                    WaitForTextFieldAndEnterText($"{XPath.PersonalValuables.ItemRow}[{rowIndex}]{XPath.PersonalValuables.EnterItemDescription}", valuable.Description);
                    ClickControl($"{XPath.PersonalValuables.ItemRow}[{rowIndex}]//div[contains(@data-wrapper-for,'__ItemValue')]");
                    Thread.Sleep(2000);
                    SendKeyPressesToField($"{XPath.PersonalValuables.ItemRow}[{rowIndex}]//input[@data-role='numerictextbox']", valuable.Value.ToString());
                }
            }
        }

        private int AddNewSpecifiedPersonalValuablesRow()
        {
            var startingRowCount = GetSpecifiedPersonalValuablesRowCount();
            var rowCountAfterAdd = 0;

            ClickControl(XPath.PersonalValuables.AddItemButton);

            var endTime = DateTime.Now.AddSeconds(WaitTimes.T5SEC);
            do
            {
                try
                {
                    if (GetSpecifiedPersonalValuablesRowCount() > startingRowCount)
                    {
                        var newRowCategoryDropDown = GetElement($"{XPath.PersonalValuables.ItemRow}[{startingRowCount + 1}]{XPath.PersonalValuables.SelectCategory}");
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

        private int GetSpecifiedPersonalValuablesRowCount()
        {
            return _driver.FindElements(By.XPath(XPath.PersonalValuables.ItemRow)).Count;
        }

        private void SetCheckBox(string xpath, bool setAsChecked)
        {
            var checkboxTick = GetElement($"{xpath}{XPR_CHECKMARK}");
            Reporting.Log($"D:{checkboxTick.Displayed} E:{checkboxTick.Enabled}");
            if (checkboxTick.Displayed != setAsChecked)
                ClickControl($"{xpath}/span[1]"); // @class='cb-checkbox']
        }

        public void ClickContinueBtn()
        {
            ClickControl(XP_CONTINUE_PAGE_BTN);
        }
    }
}
