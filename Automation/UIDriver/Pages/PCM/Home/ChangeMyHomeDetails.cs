using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Linq;
using System.Threading;
using UIDriver.Pages.B2C;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace UIDriver.Pages.PCM.Home
{
    public class ChangeMyHomeDetails : BaseEndorsementPage
    {
        #region XPATHS
        // General
        private const string XPR_RADIO_YES                                   = "//span[text()='Yes']";
        private const string XPR_RADIO_NO                                    = "//span[text()='No']";

        // Home details
        private const string XP_BUILDING_TYPE                                = "//span[@aria-owns='Building_DwellingType_listbox']";
        private const string XP_BUILDING_TYPE_OPTION                         = "id('Building_DwellingType_listbox')/li";
        private const string XP_ALARM_TYPE                                   = "//span[@aria-owns='Building_AlarmTypeId_listbox']";
        private const string XP_ALARM_TYPE_OPTION                            = "id('Building_AlarmTypeId_listbox')/li";

        private const string XP_CHANGE_START_DATE                            = "//span[@aria-owns='EffectiveDate_SelectedEffectiveDate_listbox']";
        private const string XP_CHANGE_START_DATE_OPTIONS                    = "id('EffectiveDate_SelectedEffectiveDate_listbox')/li";
        private const string XP_CHANGE_SELECTED_ADDRESS                      = "//a[contains(text(),'Change selected address')]";
        private const string XP_STREETADDRESS                                = "id('RiskLocation_qasautocomplete')";
        private const string XP_ADDR_SUGGESTION                              = "//div[@id='RiskLocation']//table[@class='address-find-table']//tr/td[1]";
        private const string XP_BUILDING_MATERIAL                            = "//span[@aria-owns='Building_MainConstructionMaterial_listbox']";
        private const string XP_BUILDING_MATERIAL_OPTION                     = "id('Building_MainConstructionMaterial_listbox')/li";
        private const string XP_IS_ELEVATED_YN                               = "id('IsPropertyElevated')";
        private const string XP_HAS_CYCLONE_SHUTTERS_YN                      = "id('HasCycloneShutters')";
        private const string XP_GARAGE_DOOR_UPGRADES                         = "//span[@aria-owns='GarageDoorUpgradePost2012_listbox']";
        private const string XP_GARAGE_DOOR_UPGRADES_OPTION                  = "id('GarageDoorUpgradePost2012_listbox')/li";
        private const string XP_ROOF_IMPROVEMENTS                            = "//span[@aria-owns='RoofRepairsPost1982_listbox']";
        private const string XP_ROOF_IMPROVEMENTS_OPTION                     = "id('RoofRepairsPost1982_listbox')/li";
        private const string XP_CONSTRUCTION_YEAR                            = "//span[@aria-owns='Building_YearBuilt_listbox']";
        private const string XP_CONSTRUCTION_YEAR_OPTION                     = "id('Building_YearBuilt_listbox')/li";
        private const string XP_CONSTRUCTION_ROOF                            = "//span[@aria-owns='Building_RoofConstruction_listbox']";
        private const string XP_CONSTRUCTION_ROOF_OPTION                     = "id('Building_RoofConstruction_listbox')/li";
        private const string XP_WINDOWS_SECURED_YN                           = "id('Building_HasWindowSecurityFittings')";
        private const string XP_DOORS_SECURED_YN                             = "id('Building_HasExternalDoorSecurityFittings')";
        private const string XP_ISMORTGAGED_GROUP                            = "id('IsFinanced')";
        private const string XP_FINANCIER                                    = "//span[@aria-owns='FinanceCompany_listbox']";
        private const string XP_FINANCIER_DROPDOWN                           = "id('FinanceCompany_listbox')/li";
        private const string XP_WEEKLY_RENTAL_AMOUNT                         = "id('WeeklyRentalAmount')";
        private const string XP_PROPERTY_MANAGER                             = "//span[@aria-owns='PropertyManagerType_listbox']";
        private const string XP_PROPERTY_MANAGER_OPTION                      = "id('PropertyManagerType_listbox')/li";
        private const string XP_ACCORDION_BUTTON1                            = "id('accordion_0_submit-action')";

        // Changes to your premium
        private const string XP_PREMIUM_CHANGE_LABEL                         = "//div[@class='price-label']";
        private const string XP_PREMIUM_INCREASE                             = "//div[@class='price-amount']";
        private const string XP_CUSTOMISE_YOUR_PREMIUM_LABEL                 = "//div[@class='customise-your-premium-header']";
        private const string XP_BUILDING_EXCESS                              = "//span[@aria-owns='EndorsementDetails_BuildingExcessData_ExcessLevelVORefId_listbox']";
        private const string XP_BUILDING_EXCESS_OPTIONS                      = "id('EndorsementDetails_BuildingExcessData_ExcessLevelVORefId_listbox')/li";
        private const string XP_BUILDING_SUM_INSURED                         = "//div[@data-wrapper-for='EndorsementDetails_BuildingSumInsured']//div[contains(@class,'answer')]";
        private const string XP_CONTENTS_EXCESS                              = "//span[@aria-owns='EndorsementDetails_ContentsExcessData_ExcessLevelVORefId_listbox']";
        private const string XP_CONTENTS_EXCESS_OPTIONS                      = "id('EndorsementDetails_ContentsExcessData_ExcessLevelVORefId_listbox')/li";
        private const string XP_CONTENTS_SUM_INSURED                         = "//div[@data-wrapper-for='EndorsementDetails_ContentSumInsured']//div[contains(@class,'answer')]";
        private const string XP_EMAIL_TEXT                                   = "id('Email')";
        private const string XP_ACCORDION_BUTTON2                            = "id('accordion_1_submit-action')";

        // Payment details
        private const string XP_PAYMENT_AMOUNT                               = "id('PaymentAmount_Answer')";
        private const string XP_SUBMIT_PAYMENT_REFUND_BUTTON                 = "id('accordion_2_submit-action')";
        private const string XP_REFUND_BSB_NUMBER                            = "id('BankAccountDetails_Bsb')";
        private const string XP_REFUND_ACCOUNT_NUMBER                        = "id('BankAccountDetails_AccountNumber')";
        private const string XP_REFUND_ACCOUNT_NAME                          = "id('BankAccountDetails_AccountName')";

        public static class XPath
        {
            public static readonly string ExcessInfoBox = "//div[@class='info-box']";
        }
        #endregion

        #region Constants
        public static class Constant
        {
            /// <summary>
            /// TODO INSU-286 to remove the NCB project toggle 
            /// The toggle will be removed 1 year 28 days after the actual go-live, around August 2025
            /// 46 is the Product Version number when this NCB project online
            /// Here is to check the text before (Pre46) and after (Post46) the new Product Version number
            /// </summary>
            public static class Pre46
            {
                public static readonly string ExcessMsgOwner  = "The standard basic excess is $200 for building and $200 for contents. If you accept an excess other than the standard basic excess you may pay more at claim time.\r\n\r\nRead the Premium excess and discount guide for more information.";
                public static readonly string ExcessMsgInvest = "The standard basic excess is $500 for building and $500 for contents. If you accept an excess other than the standard basic excess you may pay more at claim time.\r\n\r\nRead the Premium excess and discount guide for more information.";
            }
            public static class Post46
            {
                public static readonly string ExcessMsg = "The excess is the amount you will need to pay towards settlement of any claim.\r\n\r\nRead the Premium excess and discount guide for more information.";
            }
        }
        #endregion

        #region Settable properties and controls
        public DateTime StartDate
        {
            get
            {
                var date = DateTime.MinValue;
                var fieldValue = "Pending read of UI date dropdown element.";
                try
                {
                    fieldValue = GetInnerText($"{XP_CHANGE_START_DATE}{XPEXT_DROPDOWN_VALUE}");
                    date = DateTime.Parse(fieldValue);
                }
                catch (NoSuchElementException)
                {
                    Reporting.Error($"Error fetching current start date. Element not found. Attempted to parse text {fieldValue}");
                }
                catch (FormatException)
                {
                    Reporting.Error($"Error fetching current start date. Date format incorrect. Attempted to parse text {fieldValue}");
                }
                return date;
            }
            set => WaitForSelectableAndPickFromDropdown(XP_CHANGE_START_DATE,
                                                        XP_CHANGE_START_DATE_OPTIONS,
                                                        value.ToString("d MMMM yyyy"));
        }

        public HomeType HomeType
        {
            get
            {
                var dropdownText = GetInnerText(XP_BUILDING_TYPE);
                Reporting.IsTrue(HomeTypeDropdownText.Any(x => x.Value.TextB2C.Equals(dropdownText)), $"that home Type shown in drop down ({dropdownText}) was recognised");

                return HomeTypeDropdownText.First(x => x.Value.TextB2C.Equals(dropdownText)).Key;
            }
            set => WaitForSelectableAndPickFromDropdown(XP_BUILDING_TYPE,
                                                        XP_BUILDING_TYPE_OPTION,
                                                        HomeTypeDropdownText[value].TextB2C);
        }

        public HomeMaterial Material
        {
            get
            {
                var dropdownText = GetInnerText(XP_BUILDING_MATERIAL);
                return DataHelper.GetValueFromDescription<HomeMaterial>(dropdownText);
            }
            set => WaitForSelectableAndPickFromDropdown(XP_BUILDING_MATERIAL,
                                                        XP_BUILDING_MATERIAL_OPTION,
                                                        value.GetDescription());
        }

        public int YearBuilt
        {
            get => int.Parse(GetInnerText(XP_CONSTRUCTION_YEAR));
            set => WaitForSelectableAndPickFromDropdown(XP_CONSTRUCTION_YEAR,
                                                        XP_CONSTRUCTION_YEAR_OPTION,
                                                        value.ToString());
        }

        public HomeRoof HomeRoof
        {
            get => DataHelper.GetValueFromDescription<HomeRoof>(GetInnerText(XP_CONSTRUCTION_ROOF));
            set => WaitForSelectableAndPickFromDropdown(XP_CONSTRUCTION_ROOF,
                                                        XP_CONSTRUCTION_ROOF_OPTION,
                                                        value.GetDescription());
        }

        /// <summary>
        /// Question regarding whether property is elevated, conditional based on whether
        /// property is in a cyclone risk area
        /// </summary>
        /// <exception cref="InvalidElementStateException">If attempt to read state, but no option has been selected yet.</exception>
        public bool IsPropertyElevated
        {
            get
            {
                var radioButton = $"{XP_IS_ELEVATED_YN}{XPR_RADIO_YES}/../span[contains(@class,'rb-radio')]";
                if (GetClass(radioButton).Contains("checked"))
                    return true;

                radioButton = $"{XP_IS_ELEVATED_YN}{XPR_RADIO_NO}/../span[contains(@class,'rb-radio')]";
                if (GetClass(radioButton).Contains("checked"))
                    return false;

                throw new InvalidElementStateException("No radio button has been selected for is property elevated question.");
            }
            set => ClickControl(value ? $"{XP_IS_ELEVATED_YN}{XPR_RADIO_YES}" : $"{XP_IS_ELEVATED_YN}{XPR_RADIO_NO}");
        }

        /// <summary>
        /// Question regarding whether property has cyclone shutters, conditional based on 
        /// whether property is in a cyclone risk area
        /// </summary>
        /// <exception cref="InvalidElementStateException">If attempt to read state, but no option has been selected yet.</exception>
        public bool HasCycloneWindowShutters
        {
            get
            {
                var radioButton = $"{XP_HAS_CYCLONE_SHUTTERS_YN}{XPR_RADIO_YES}/../span[contains(@class,'rb-radio')]";
                if (GetClass(radioButton).Contains("checked"))
                    return true;

                radioButton = $"{XP_HAS_CYCLONE_SHUTTERS_YN}{XPR_RADIO_NO}/../span[contains(@class,'rb-radio')]";
                if (GetClass(radioButton).Contains("checked"))
                    return false;

                throw new InvalidElementStateException("No radio button has been selected for window cyclone shutters question.");
            }
            set => ClickControl(value ? $"{XP_HAS_CYCLONE_SHUTTERS_YN}{XPR_RADIO_YES}" : $"{XP_HAS_CYCLONE_SHUTTERS_YN}{XPR_RADIO_NO}");
        }

        /// <summary>
        /// Question regarding whether the garage door has had any upgrades. This question
        /// is a conditional question based on whether the property is in a cyclone risk
        /// area, and was built prior to 2012.
        /// </summary>
        /// <exception cref="InvalidElementStateException">If attempt to read state, but control is not visible or has an unrecognised value.</exception>
        public GarageDoorsUpgradeStatus GarageDoorStatus
        {
            get
            {
                var dropdownText = GetInnerText(XP_GARAGE_DOOR_UPGRADES);
                try
                {
                    return DataHelper.GetValueFromDescription<GarageDoorsUpgradeStatus>(dropdownText);
                }
                catch
                {
                    throw new InvalidElementStateException($"Unrecognised text in dropdown, received: {dropdownText}");
                }
            }
            set => WaitForSelectableAndPickFromDropdown(XP_GARAGE_DOOR_UPGRADES,
                                                       XP_GARAGE_DOOR_UPGRADES_OPTION,
                                                       value.GetDescription());
        }

        /// <summary>
        /// Question regarding whether the roof has had any improvements. This question
        /// is a conditional question based on whether the property is in a cyclone risk
        /// area, and was built prior to 1982.
        /// </summary>
        /// <exception cref="InvalidElementStateException">If attempt to read state, but control is not visible or has an unrecognised value.</exception>
        public RoofImprovementStatus RoofImprovements
        {
            get
            {
                var dropdownText = GetInnerText(XP_ROOF_IMPROVEMENTS);
                try
                {
                    return DataHelper.GetValueFromDescription<RoofImprovementStatus>(dropdownText);
                }
                catch
                {
                    throw new InvalidElementStateException($"Unrecognised text in dropdown, received: {dropdownText}");
                }
            }
            set => WaitForSelectableAndPickFromDropdown(XP_ROOF_IMPROVEMENTS,
                                                       XP_ROOF_IMPROVEMENTS_OPTION,
                                                       value.GetDescription());
        }

        public bool AreWindowsSecured
        {
            get
            {
                var radioButton = $"{XP_WINDOWS_SECURED_YN}{XPR_RADIO_YES}/../span[contains(@class,'rb-radio')]";
                if (GetClass(radioButton).Contains("checked"))
                { return true; }

                radioButton = $"{XP_WINDOWS_SECURED_YN}{XPR_RADIO_NO}/../span[contains(@class,'rb-radio')]";
                // If this second control is NOT checked, then neither of them have been selected.
                if (!GetClass(radioButton).Contains("checked"))
                { Reporting.Error("No radio button has been selected for windows secured question."); }
                
                return false;
            }
            set => ClickControl(value ? $"{XP_WINDOWS_SECURED_YN}{XPR_RADIO_YES}" : $"{XP_WINDOWS_SECURED_YN}{XPR_RADIO_NO}");
        }

        public bool AreDoorsSecured
        {
            get
            {
                var radioButton = $"{XP_DOORS_SECURED_YN}{XPR_RADIO_YES}/../span[contains(@class,'rb-radio')]";
                if (GetClass(radioButton).Contains("checked"))
                { return true; }

                radioButton = $"{XP_DOORS_SECURED_YN}{XPR_RADIO_NO}/../span[contains(@class,'rb-radio')]";
                // If this second control is NOT checked, then neither of them have been selected.
                if (!GetClass(radioButton).Contains("checked"))
                { Reporting.Error("No radio button has been selected for doors secured question."); }
                
                return false;
            }
            set => ClickControl(value ? $"{XP_DOORS_SECURED_YN}{XPR_RADIO_YES}" : $"{XP_DOORS_SECURED_YN}{XPR_RADIO_NO}");
        }

        public Alarm Alarm
        {
            get
            {
                var dropdownText = GetInnerText(XP_ALARM_TYPE);
                return DataHelper.GetValueFromDescription<Alarm>(dropdownText);
            }
            set => WaitForSelectableAndPickFromDropdown(XP_ALARM_TYPE,
                                                       XP_ALARM_TYPE_OPTION,
                                                       value.GetDescription());
        }

        /// <summary>
        /// Returns the name of the financier, if one has been provided,
        /// otherwise return null string if not financed.
        /// </summary>
        public string Financier
        {
            get
            {
                var radioButton = $"{XP_ISMORTGAGED_GROUP}{XPR_RADIO_YES}/../span[contains(@class,'rb-radio')]";
                if (GetClass(radioButton).Contains("checked"))
                { return GetInnerText(XP_FINANCIER); }

                radioButton = $"{XP_ISMORTGAGED_GROUP}{XPR_RADIO_NO}/../span[contains(@class,'rb-radio')]";
                // If this second control is NOT checked, then neither of them have been selected.
                if (!GetClass(radioButton).Contains("checked"))
                { Reporting.Error("No radio button has been selected for financier."); }
                
                return null;
            }
            set
            {
                var desiredXPath = !string.IsNullOrEmpty(value) ? $"{XP_ISMORTGAGED_GROUP}{XPR_RADIO_YES}" : $"{XP_ISMORTGAGED_GROUP}{XPR_RADIO_NO}";
                ClickControl(desiredXPath);

                if (!string.IsNullOrEmpty(value))
                {
                    WaitForSelectableAndPickFromDropdown(XP_FINANCIER, XP_FINANCIER_DROPDOWN, value);
                }
            }
        }

        public string WeeklyRental
        {
            get => throw new NotImplementedException("Unable to access shadow DOM for this value.");
            set => WaitForTextFieldAndEnterText(XP_WEEKLY_RENTAL_AMOUNT, value.ToString(), false);
        }

        public HomePropertyManager WhoManagesProperty
        {
            get => DataHelper.GetValueFromDescription<HomePropertyManager>(GetInnerText(XP_PROPERTY_MANAGER));
            set
            {
                Thread.Sleep(2000);

                WaitForSelectableAndPickFromDropdown(XP_PROPERTY_MANAGER,
                                                     XP_PROPERTY_MANAGER_OPTION,
                                                     value.GetDescription());
            }
        }

        public decimal ChangeToPremium
        {
            get
            {
                var premiumChange = decimal.Parse(GetInnerText(XP_PREMIUM_INCREASE).StripMoneyNotations());
                var premiumLabel = GetInnerText(XP_PREMIUM_CHANGE_LABEL);

                // If reduction in premium, then return premium change as negative value.
                if (premiumLabel.Contains("Your premium has reduced by"))
                {
                    premiumChange = premiumChange > 0 ? -premiumChange : premiumChange;
                }

                return premiumChange;
            }
        }

        public string BuildingExcess
        {
            get => GetInnerText($"{XP_BUILDING_EXCESS}{XPEXT_DROPDOWN_VALUE}").StripMoneyNotations();
            set => WaitForSelectableAndPickFromDropdown(
                       XP_BUILDING_EXCESS,
                       XP_BUILDING_EXCESS_OPTIONS,
                       DataHelper.AddCurrencyPrefixToAmount(amount: value, 
                                                            currencyPrefix: "$"));
        }

        public int BuildingSumInsured
        {
            get => int.Parse(GetElement($"{XP_BUILDING_SUM_INSURED}//input[@type='text']").GetAttribute("aria-valuenow"));
            set
            {
                // No action needed if the value is already desired value.
                if (BuildingSumInsured == value)
                    return;

                // This block is quite messy, as B2C uses two input elements here, and swaps them when the user
                // clicks on them. So the first one is read-only and shows the current value. When the user clicks
                // the second one is displayed and is editable. Hence the logic below to send a click to the base DIV
                // and to then send the keypresses to a specific INPUT object.
                ClickControl(XP_BUILDING_SUM_INSURED);
                Thread.Sleep(2000);
                SendKeyPressesAfterClearingExistingTextInField($"{XP_BUILDING_SUM_INSURED}//input[@data-role='numerictextbox']", $"{value.ToString()}{Keys.Return}");

            }
        }

        public string ContentsExcess
        {
            get => GetInnerText($"{XP_CONTENTS_EXCESS}{XPEXT_DROPDOWN_VALUE}").StripMoneyNotations();
            set => WaitForSelectableAndPickFromDropdown(
                       XP_CONTENTS_EXCESS,
                       XP_CONTENTS_EXCESS_OPTIONS,
                       DataHelper.AddCurrencyPrefixToAmount(amount: value,
                                                            currencyPrefix: "$"));
        }

        public int ContentsSumInsured
        {
            get => int.Parse(GetElement($"{XP_CONTENTS_SUM_INSURED}//input[@type='text']").GetAttribute("aria-valuenow"));
            set
            {
                // No action needed if the value is already desired value.
                if (ContentsSumInsured == value)
                    return;

                // This block is quite messy, as B2C uses two input elements here, and swaps them when the user
                // clicks on them. So the first one is read-only and shows the current value. When the user clicks
                // the second one is displayed and is editable. Hence the logic below to send a click to the base DIV
                // and to then send the keypresses to a specific INPUT object.
                ClickControl(XP_CONTENTS_SUM_INSURED);
                Thread.Sleep(2000);
                SendKeyPressesAfterClearingExistingTextInField($"{XP_CONTENTS_SUM_INSURED}//input[@data-role='numerictextbox']", $"{value.ToString()}{Keys.Return}");
            }
        }

        public string Email
        {
            get => throw new NotImplementedException("Not yet implemented access to shadow DOM.");
            set => WaitForTextFieldAndEnterText(XP_EMAIL_TEXT, value, false);
        }
        #endregion

        public ChangeMyHomeDetails(Browser browser) : base(browser)
        {
        }

        override public bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XP_ACCORDION_BUTTON1);
                GetElement(XP_BUILDING_TYPE);
                GetElement(XP_ALARM_TYPE);
                Reporting.LogPageChange("Home policy change home details page");
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                Reporting.Log("Element not found when checking if Home policy change home details page is displayed.");
                rendered = false;
            }
            return rendered;
        }

        /// <summary>
        /// Update the first accordion in the "Change my home details"
        /// endorsement flow. This includes updating the address with and entering all other mandatory data.
        /// </summary>
        /// <param name="testData"></param>
        public void UpdateMyHomeDetails(EndorseHome testData)
        {
            if (testData.StartDate.Date != DateTime.Now.Date)
            { StartDate = testData.StartDate; }

            var newProperty = testData.NewAssetValues;

            // Changing the address
            if (newProperty.PropertyAddress != null)
            {
                ClickControl(XP_CHANGE_SELECTED_ADDRESS);
                QASSearchForAddress(XP_STREETADDRESS, XP_ADDR_SUGGESTION, testData.NewAssetValues.PropertyAddress.StreetSuburbState());
            }

            if (newProperty.TypeOfBuilding != HomeType.Undefined)
            { HomeType = newProperty.TypeOfBuilding; }

            if (newProperty.WallMaterial != HomeMaterial.Undefined)
            { Material = newProperty.WallMaterial; }

            if (newProperty.YearBuilt != 0)
            { YearBuilt = newProperty.YearBuilt; }

            // Roof material question is only applicable for Home Policies which
            // are pre-Cyclone (less than version ID 68000008), or Cyclone is disabled.
            if (!Config.Get().IsCycloneEnabled() || testData.OriginalPolicyData.ProductVersionAsInteger < HomeProductVersionCycloneReinsurance)
            {
                if (newProperty.RoofMaterial != HomeRoof.Undefined)
                { HomeRoof = newProperty.RoofMaterial; }
            }

            AreWindowsSecured = newProperty.SecurityWindowsSecured;
            AreDoorsSecured   = newProperty.SecurityDoorsSecured;

            if (newProperty.AlarmSystem != Alarm.Undefined)
            { Alarm = newProperty.AlarmSystem; }

            Financier = testData.Financier;

            if (testData.WeeklyRent != null)
            { WeeklyRental = testData.WeeklyRent; }

            if (testData.HomePropertyManager != HomePropertyManager.Undefined)
            { WhoManagesProperty = testData.HomePropertyManager; }

            /* TODO AUNT-112: To look at having this operate when we are changing the home address
             * as well, so that we can implement support for changing to a cyclone address.
             */
            if (Config.Get().IsCycloneEnabled() &&
                newProperty.IsACycloneAddress)
            {
                IsPropertyElevated = newProperty.IsPropertyElevated;
                HasCycloneWindowShutters = newProperty.IsCycloneShuttersFitted;

                if ((newProperty.YearBuilt == 0 && testData.OriginalPolicyData.HomeAsset.ConstructionYear < 2012) ||
                    (newProperty.YearBuilt != 0 && newProperty.YearBuilt < 2012))
                {
                    GarageDoorStatus = newProperty.GarageDoorsCycloneStatus;
                }

                if ((newProperty.YearBuilt == 0 && testData.OriginalPolicyData.HomeAsset.ConstructionYear < 1982) ||
                    (newProperty.YearBuilt != 0 && newProperty.YearBuilt < 1982))
                {
                    RoofImprovements = newProperty.RoofImprovementCycloneStatus;
                }
            }


            Reporting.Log("Endorsement fields complete", _driver.TakeSnapshot());
            ClickControl(XP_ACCORDION_BUTTON1);

            using (var spinner = new RACSpinner(_browser))
            {
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Wait for the second accordion to display with the resulting
        /// change in premium.
        /// </summary>
        /// <returns></returns>
        public decimal WaitForPremiumChangeAccordion()
        {
            _driver.WaitForElementToBeVisible(By.XPath(XP_PREMIUM_CHANGE_LABEL), WaitTimes.T30SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_CUSTOMISE_YOUR_PREMIUM_LABEL), WaitTimes.T5SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_ACCORDION_BUTTON2), WaitTimes.T5SEC);

            Reporting.Log("Change to premium is displayed after endorsement details.", _driver.TakeSnapshot());
            return ChangeToPremium;
        }

        /// <summary>
        /// Update any required values (such as Building & Contents Excess, Building & Contents sum insured)
        /// before entering email address and proceeding with endorsement.
        /// </summary>
        /// <param name="testData"></param>
        /// <returns>The change to premium</returns>
        public decimal UpdatePolicyExcessAndSIAndProceed(EndorseHome testData)
        {
            decimal updatedPremiumChange = 0;

            using (var spinner = new RACSpinner(_browser))
            {
                // Update Building Excess & Sum Insured
                if (testData.NewAssetValues.BuildingValue.HasValue)
                {
                    if (!string.IsNullOrEmpty(testData.ExcessBuilding))
                    {
                        BuildingExcess = testData.ExcessBuilding;
                        spinner.WaitForSpinnerToFinish();
                    }

                    BuildingSumInsured = testData.NewAssetValues.BuildingValue.Value;
                    spinner.WaitForSpinnerToFinish();
                }

                // Update Contents Excess & Sum Insured
                if (testData.NewAssetValues.ContentsValue.HasValue)
                {
                    if (!string.IsNullOrEmpty(testData.ExcessContents))
                    {
                        ContentsExcess = testData.ExcessContents;
                        spinner.WaitForSpinnerToFinish();
                    }

                    ContentsSumInsured = testData.NewAssetValues.ContentsValue.Value;
                    spinner.WaitForSpinnerToFinish();
                }

                Email = testData.PayMethod.Payer.GetEmail();

                updatedPremiumChange = ChangeToPremium;
                Reporting.Log($"Accepted premium changes: ${updatedPremiumChange}", _driver.TakeSnapshot());
                ClickControl(XP_ACCORDION_BUTTON2);
                spinner.WaitForSpinnerToFinish();
            }
            return updatedPremiumChange;
        }

        public void VerifyExcessMsg(EndorseHome testData)
        {
            Reporting.AreEqual(Constant.Post46.ExcessMsg, GetInnerText(XPath.ExcessInfoBox), "Excess Info Box displays correctly");
        }

        /// <summary>
        /// Wait for third accordion to render with payment details
        /// for change in premium.
        /// </summary>
        public void WaitForPaymentDetails(PremiumChange changeType)
        {
            if (changeType == PremiumChange.PremiumIncrease)
            {
                _driver.WaitForElementToBeVisible(By.XPath(XP_PAYMENT_AMOUNT), WaitTimes.T30SEC);
            }

            if (changeType == PremiumChange.PremiumDecrease)
            {
                _driver.WaitForElementToBeVisible(By.XPath(XP_REFUND_BSB_NUMBER), WaitTimes.T30SEC);
                _driver.WaitForElementToBeVisible(By.XPath(XP_REFUND_ACCOUNT_NUMBER), WaitTimes.T5SEC);
                _driver.WaitForElementToBeVisible(By.XPath(XP_REFUND_ACCOUNT_NAME), WaitTimes.T5SEC);
            }

            _driver.WaitForElementToBeVisible(By.XPath(XP_SUBMIT_PAYMENT_REFUND_BUTTON), WaitTimes.T5SEC);
        }

        /// <summary>
        /// Navigates process of one-off payments or refunds relating to endorsement on policy.
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="expectedAmount"></param>
        public void HandlePaymentOrRefundPrompt(EndorseHome testData, decimal expectedAmount)
        {
            using (var spinner = new RACSpinner(_browser))
            {
                if (testData.IsExpectedToReceiveRefund())
                {
                    var bankAccount = testData.PayMethod.Payer.BankAccounts[0];
                    WaitForTextFieldAndEnterText(XP_REFUND_BSB_NUMBER, bankAccount.Bsb);
                    WaitForTextFieldAndEnterText(XP_REFUND_ACCOUNT_NUMBER, bankAccount.AccountNumber);
                    WaitForTextFieldAndEnterText(XP_REFUND_ACCOUNT_NAME, bankAccount.AccountName);
                }
                else
                {
                    using (var westpacIframe = new WestpacQuickStream(_browser))
                    {
                        VerifyUIElementsInPaymentDetailsAccordionAreCorrect(expectedAmount);
                        westpacIframe.EnterCardDetails(testData.PayMethod);
                        Reporting.IsTrue(IsPayNowButtonEnabled, "Pay now button".IsEnabled());
                    }
                }

                ClickControl(XP_SUBMIT_PAYMENT_REFUND_BUTTON);
                spinner.WaitForSpinnerToFinish();
            }
        }
    }
}