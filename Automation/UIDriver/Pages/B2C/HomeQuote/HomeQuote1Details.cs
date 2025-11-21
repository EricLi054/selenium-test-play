using Rac.TestAutomation.Common;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyHome;
using System.Data;

namespace UIDriver.Pages.B2C
{
    public class HomeQuote1Details : BasePage
    {
        // Max past claims allowed (given longest previous insurance history)
        private const int MAXIMUM_CLAIMS_HISTORY        = 5;
        private const string DECLINE_NOTICE_HEADER_TEXT = "Cover declined";

        #region XPATHS
        // Segments
        private const string BASE = "/html/body/div[@id='wrapper']";
        private const string FORM = BASE + "//form[@class='insuranceForm questionForm formTop']";

        // General
        private const string XP_PAGE_HEADING = BASE + "//span[@class='action-heading']/span";
        private const string XPR_YES = "//span[text()='Yes']/..";
        private const string XPR_NO  = "//span[text()='No']/..";

        // Insurance Type
        private const string XP_OCCUPANCY_STATUS        = FORM + "//span[@aria-owns='Question_Cover_OccupancyStatus_listbox']";
        private const string XP_OCCUPANCY_STATUS_OPTION = "id('Question_Cover_OccupancyStatus_listbox')/li";
        private const string XP_IS_SHORT_STAY_YN        = "id('Question_Cover_IsHomeUsedForShortStay')";
        private const string XP_REQUIRED_COVER          = FORM + "//span[@aria-owns='Question_Cover_CoverTypeId_listbox']";
        private const string XP_REQUIRED_COVER_OPTION   = "id('Question_Cover_CoverTypeId_listbox')/li";
        private const string XP_CONTINUE_ACDN_1_BTN     = "id('accordion_0')";

        // Building details
        private const string XP_BUILDING_TYPE            = FORM + "//span[@aria-owns='Question_Building_Building_DwellingType_listbox']";
        private const string XP_BUILDING_TYPE_OPTION     = "id('Question_Building_Building_DwellingType_listbox')/li";
        private const string XP_BUILDING_MATERIAL        = FORM + "//span[@aria-owns='Question_Building_Building_MainConstructionMaterial_listbox']";
        private const string XP_BUILDING_MATERIAL_OPTION = "id('Question_Building_Building_MainConstructionMaterial_listbox')/li";
        private const string XP_CONSTRUCTION_YEAR        = FORM + "//span[@aria-owns='Question_Building_Building_YearBuilt_listbox']";
        private const string XP_CONSTRUCTION_YEAR_OPTION = "id('Question_Building_Building_YearBuilt_listbox')/li";
        private const string XP_MAILINGADDRESS           = "id('Question_Building_RiskLocation_qasautocomplete')";
        private const string XP_ADDR_SUGGESTION          = "//div[@id='Question_Building_RiskLocation']//table[@class='address-find-table']//tr/td[1]";
        private const string XP_WEEKLY_RENTAL_AMOUNT     = "id('Question_Building_WeeklyRentalAmount')";
        private const string XP_PROPERTY_MANAGER         = FORM + "//span[@aria-owns='Question_Building_PropertyManagerType_listbox']";
        private const string XP_PROPERTY_MANAGER_OPTION  = "id('Question_Building_PropertyManagerType_listbox')/li";
        private const string XP_MORE_THAN_5ACRES_YN      = "id('Question_Building_IsLargeProperty')";
        private const string XP_WINDOWS_SECURED_YN       = "id('Question_Building_HasWindowSecurityFittings')";
        private const string XP_DOORS_SECURED_YN         = "id('Question_Building_HasExternalDoorSecurityFittings')";
        private const string XP_RAISED_ABOVE_GROUND_YN   = "id('Question_Building_IsPropertyElevated')";
        private const string XP_HAS_CYCLONE_SHUTTERS_YN  = "id('Question_Building_HasCycloneShutters')";
        private const string XP_GARAGE_DOORS             = FORM + "//span[@aria-owns='Question_Building_GarageDoorUpgradePost2012_listbox']";
        private const string XP_GARAGE_DOORS_OPTION      = "id('Question_Building_GarageDoorUpgradePost2012_listbox')/li";
        private const string XP_ROOF_REPAIRS             = FORM + "//span[@aria-owns='Question_Building_RoofRepairsPost1982_listbox']";
        private const string XP_ROOF_REPAIRS_OPTION      = "id('Question_Building_RoofRepairsPost1982_listbox')/li";
        private const string XP_ALARM                    = FORM + "//span[@aria-owns='Question_Building_AlarmTypeId_listbox']";
        private const string XP_ALARM_OPTION             = "id('Question_Building_AlarmTypeId_listbox')/li";
        private const string XP_INSURED_YEARS            = FORM + "//span[@aria-owns='Question_Building_ContinuousHomeInsuranceHistory_listbox']";
        private const string XP_INSURED_YEARS_OPTION     = "id('Question_Building_ContinuousHomeInsuranceHistory_listbox')/li";
        private const string XP_CONTINUE_ACDN_2_BTN      = "id('accordion_1')";

        // Declarations
        private const string XP_HAS_CRIMINAL_CHARGES_YN  = "id('Question_PolicyDisclosures_IsConvicted')";
        private const string XP_HAS_HISTORICAL_CLAIMS_YN = "id('Question_PolicyDisclosures_HadAnyClaims')";
        private const string XP_ADD_CLAIM_HISTORY_BTN    = "id('DisclosuresClaim_addButton')";
        private const string XP_CONTINUE_ACDN_3_BTN      = "id('accordion_2')";

        // Cover Details
        private const string XP_BUILDING_SI = "id('Question_CoverDetails_BuildingSumInsured')";
        private const string XP_CONTENTS_SI = "id('Question_CoverDetails_ContentsSumInsured')";
        private const string XP_OLDEST_PH_AGE = "id('Question_CoverDetails_AgeOfOldestPolicyHolder')";
        private const string XP_RAC_MEMBER_YN = "id('Question_CoverDetails_IsMember')";
        private const string XP_EMAIL_ADDRESS = "id('Question_CoverDetails_EmailAddress')";
        private const string XP_MEMBERSHIP_TIER        = "//span[@aria-owns='Question_CoverDetails_MembershipLevel_listbox']";
        private const string XP_MEMBERSHIP_TIER_OPTION = "id('Question_CoverDetails_MembershipLevel_listbox')/li";
        private const string XP_CONTINUE_ACDN_4_BTN = "id('accordion_3_submit-action')";

        // Cover Declined dialog
        private const string XP_DECLINED_NOTICE_HEADER = "id('knockout-dialog_wnd_title')";
        private const string XP_DECLINED_NOTICE_TEXT   = "id('knockout-dialog')";
        private const string XP_DECLINED_NOTICE_DISMISS = XP_DECLINED_NOTICE_HEADER + "/following-sibling::div[@class='cluetip-close']";
        #endregion

        #region Settable properties and controls
        public HomeOccupancy Occupancy
        {
            get => DataHelper.GetValueFromDescription<HomeOccupancy>(GetInnerText(XP_OCCUPANCY_STATUS));
            set => WaitForSelectableAndPickFromDropdown(XP_OCCUPANCY_STATUS,
                                                        XP_OCCUPANCY_STATUS_OPTION,
                                                        value.GetDescription());
        }

        public bool IsUsedForShortStay
        {
            get => GetBinaryToggleState(XP_IS_SHORT_STAY_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_IS_SHORT_STAY_YN, XPR_YES, XPR_NO, value);
        }

        public HomeCover Cover
        {
            get => DataHelper.GetValueFromDescription<HomeCover>(GetInnerText(XP_REQUIRED_COVER));
            set => WaitForSelectableAndPickFromDropdown(XP_REQUIRED_COVER,
                                                     XP_REQUIRED_COVER_OPTION,
                                                     value.GetDescription());
        }

        public HomeType HomeType
        {
            get
            {
                var dropdownText = GetInnerText(XP_BUILDING_TYPE);
                Reporting.IsTrue(HomeTypeDropdownText.Any(x => x.Value.TextB2C == dropdownText), $"for recognised text in dropdown, received: {dropdownText}");
                return HomeTypeDropdownText.First(x => x.Value.TextB2C == dropdownText).Key;
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
            get => int.Parse(GetInnerText(XP_REQUIRED_COVER));
            set => WaitForSelectableAndPickFromDropdown(XP_CONSTRUCTION_YEAR,
                                                        XP_CONSTRUCTION_YEAR_OPTION,
                                                        value.ToString());
        }

        public int WeeklyRental
        {
            get => throw new NotImplementedException("Unable to access shadow DOM for this value.");
            set => WaitForTextFieldAndEnterText(XP_WEEKLY_RENTAL_AMOUNT, value.ToString(), false);
        }

        public HomePropertyManager WhoManagesProperty
        {
            get => DataHelper.GetValueFromDescription<HomePropertyManager>(GetInnerText(XP_PROPERTY_MANAGER));
            set => WaitForSelectableAndPickFromDropdown(XP_PROPERTY_MANAGER,
                                                     XP_PROPERTY_MANAGER_OPTION,
                                                     value.GetDescription());
        }

        public bool IsMoreThan5Acres
        {
            get => GetBinaryToggleState(XP_MORE_THAN_5ACRES_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_MORE_THAN_5ACRES_YN, XPR_YES, XPR_NO, value);
        }

        public bool AreWindowsSecured
        {
            get => GetBinaryToggleState(XP_WINDOWS_SECURED_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_WINDOWS_SECURED_YN, XPR_YES, XPR_NO, value);
        }

        public bool AreDoorsSecured
        {
            get => GetBinaryToggleState(XP_DOORS_SECURED_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_DOORS_SECURED_YN, XPR_YES, XPR_NO, value);
        }
        public bool isRaisedAboveGround 
        {
            get => GetBinaryToggleState(XP_RAISED_ABOVE_GROUND_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_RAISED_ABOVE_GROUND_YN, XPR_YES, XPR_NO, value);
        }
        public bool AreWindowsWithCycloneShutters
        {
            get => GetBinaryToggleState(XP_HAS_CYCLONE_SHUTTERS_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_HAS_CYCLONE_SHUTTERS_YN, XPR_YES, XPR_NO, value);
        }

        public GarageDoorsUpgradeStatus GarageDoorsUpgradeStatus
        {
            get
            {
                var dropdownText = GetInnerText(XP_GARAGE_DOORS);
                try
                {
                    return DataHelper.GetValueFromDescription<GarageDoorsUpgradeStatus>(dropdownText);
                }
                catch
                {
                    throw new DataException($"Unrecognised text in dropdown, received: {dropdownText}");
                }
            }
            set => WaitForSelectableAndPickFromDropdown(XP_GARAGE_DOORS,
                                                        XP_GARAGE_DOORS_OPTION,
                                                        value.GetDescription());
        }
        public RoofImprovementStatus RoofImprovementStatus
        {
            get
            {
                var dropdownText = GetInnerText(XP_ROOF_REPAIRS);
                try
                {
                    return DataHelper.GetValueFromDescription<RoofImprovementStatus>(dropdownText);
                }
                catch
                {
                    throw new DataException($"Unrecognised text in dropdown, received: {dropdownText}");
                }
            }
            set => WaitForSelectableAndPickFromDropdown(XP_ROOF_REPAIRS,
                                                        XP_ROOF_REPAIRS_OPTION,
                                                        value.GetDescription());
        }
        public Alarm Alarm
        {
            get
            {
                var dropdownText = GetInnerText(XP_ALARM);
                return DataHelper.GetValueFromDescription<Alarm>(dropdownText);
            }
            set => WaitForSelectableAndPickFromDropdown(XP_ALARM,
                                                        XP_ALARM_OPTION,
                                                        value.GetDescription());
        }

        public HomePreviousInsuranceTime YearsPreviouslyInsured
        {
            get => DataHelper.GetValueFromDescription<HomePreviousInsuranceTime>(GetInnerText(XP_INSURED_YEARS));
            set => WaitForSelectableAndPickFromDropdown(XP_INSURED_YEARS,
                                                        XP_INSURED_YEARS_OPTION,
                                                        value.GetDescription());
        }

        public bool HasPastCriminalConvictions
        {
            get => GetBinaryToggleState(XP_HAS_CRIMINAL_CHARGES_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_HAS_CRIMINAL_CHARGES_YN, XPR_YES, XPR_NO, value);
        }

        public bool HasPastHomeClaims
        {
            get => GetBinaryToggleState(XP_HAS_HISTORICAL_CLAIMS_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_HAS_HISTORICAL_CLAIMS_YN, XPR_YES, XPR_NO, value);
        }

        public int BuildingSumInsured
        {
            get => int.Parse(GetInnerText(XP_BUILDING_SI));
            set => WaitForTextFieldAndEnterText(XP_BUILDING_SI, value.ToString(), false);
        }

        public int ContentsSumInsured
        {
            get => int.Parse(GetInnerText(XP_CONTENTS_SI));
            set => WaitForTextFieldAndEnterText(XP_CONTENTS_SI, value.ToString(), false);
        }

        public int OldestPHAge
        {
            get => int.Parse(GetInnerText(XP_OLDEST_PH_AGE));
            set => WaitForTextFieldAndEnterText(XP_OLDEST_PH_AGE, value.ToString(), false);
        }

        public bool IsARACMember
        {
            get => GetBinaryToggleState(XP_RAC_MEMBER_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_RAC_MEMBER_YN, XPR_YES, XPR_NO, value);
        }

        public string Email
        {
            get => GetInnerText(XP_EMAIL_ADDRESS);
            set => WaitForTextFieldAndEnterText(XP_EMAIL_ADDRESS, value, false);
        }
        #endregion

        public HomeQuote1Details(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                if (!GetInnerText(XP_PAGE_HEADING).ToLower().Equals("home quote"))
                {
                    Reporting.Log("Wrong heading text for Home Quote page.");
                    return false;
                }
                GetElement(XP_OCCUPANCY_STATUS);
                GetElement(XP_REQUIRED_COVER);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Home Quote page 1 - Rating values");
            return true;
        }

        /// <summary>
        /// Select the occupancy and cover types for the home quote.
        /// Covers are derived from the building/contents sum insured
        /// values.
        /// 
        /// The question about whether the home is used for short stay
        /// is also answered here.
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <returns>Text of vehicle matched from rego/search</returns>
        public void FillQuoteDetailsFirstAccordion(QuoteHome quoteDetails)
        {
            Occupancy = quoteDetails.Occupancy;
            IsUsedForShortStay = quoteDetails.IsUsedForShortStay;

            // Allow time for cover types to updated from choice of occupancy
            Thread.Sleep(2000);

            // Desired cover is determined from whether related SI values have been defined.
            var derivedCover = HomeCover.BuildingAndContents;
            if (!quoteDetails.BuildingValue.HasValue)
                derivedCover = HomeCover.ContentsOnly;
            else if (!quoteDetails.ContentsValue.HasValue)
                derivedCover = HomeCover.BuildingOnly;

            Cover = derivedCover;

            ClickControl(XP_CONTINUE_ACDN_1_BTN);
        }

        /// <summary>
        /// Complete questions on home details and address
        /// </summary>
        /// <param name="quoteDetails"></param>
        public void FillQuoteDetailsSecondAccordion(QuoteHome quoteDetails)
        {
            HomeType = quoteDetails.TypeOfBuilding;

            var heritageListedKnockOutDialog = "heritage listed knock out dialog";

            // Handle B2C-4122 Home Heritage - B2C Knock Out
            if (quoteDetails.TypeOfBuilding == HomeType.HouseHeritage &&
                (quoteDetails.GetRequestedCoverAsString() == HomeCover.BuildingAndContents.GetDescription() || quoteDetails.GetRequestedCoverAsString() == HomeCover.BuildingOnly.GetDescription()))
            {
                using (var knockoutDialog = new KnockoutDialog(_browser))
                {
                    Reporting.IsTrue(knockoutDialog.IsDisplayed(), $"correct {heritageListedKnockOutDialog}".IsDisplayed());
                    Reporting.Log($"B2C Knock Out: B2C-4122 - {heritageListedKnockOutDialog}".IsDisplayed(), _browser.Driver.TakeSnapshot());
                    knockoutDialog.CloseKnockoutDialog();
                    Reporting.Log($"B2C Knock Out: B2C-4122 - {heritageListedKnockOutDialog} was dismissed successfully.", _browser.Driver.TakeSnapshot());
                }

                quoteDetails.TypeOfBuilding = DataHelper.GetRandomEnum<HomeType>(startIndex: 2); // Excluding HouseHeritage to be selected this time
                HomeType                    = quoteDetails.TypeOfBuilding;

                Reporting.Log($"Building type changed to {HomeTypeDropdownText[quoteDetails.TypeOfBuilding].TextB2C} " +
                    $"to allow the test to continue with the quote to policy flow.", _browser.Driver.TakeSnapshot());
            }
            else if(quoteDetails.TypeOfBuilding == HomeType.HouseHeritage)
            {
                using (var knockoutDialog = new KnockoutDialog(_browser))
                {
                    Reporting.IsFalse(knockoutDialog.IsDisplayed(), $"{heritageListedKnockOutDialog}".IsNotDisplayed());
                }
            }

            Material = quoteDetails.WallMaterial;
            YearBuilt = quoteDetails.YearBuilt;

            QASSearchForAddress(XP_MAILINGADDRESS, XP_ADDR_SUGGESTION, quoteDetails.PropertyAddress.StreetSuburbState());

            if (quoteDetails.Occupancy == HomeOccupancy.InvestmentProperty)
            {
                WeeklyRental = quoteDetails.WeeklyRental;
                WhoManagesProperty = quoteDetails.PropertyManager;
            }

            // Always setting to true, otherwise business rules block quote
            IsMoreThan5Acres = false;
            AreWindowsSecured = quoteDetails.SecurityWindowsSecured;
            AreDoorsSecured = quoteDetails.SecurityDoorsSecured;

            if (Config.Get().IsCycloneEnabled() && quoteDetails.IsACycloneAddress)
            {
                isRaisedAboveGround           = quoteDetails.IsPropertyElevated;
                AreWindowsWithCycloneShutters = quoteDetails.IsCycloneShuttersFitted;
                if (quoteDetails.YearBuilt < 2012)
                {
                    GarageDoorsUpgradeStatus  = quoteDetails.GarageDoorsCycloneStatus;

                    if (quoteDetails.YearBuilt < 1982)
                    {
                        RoofImprovementStatus = quoteDetails.RoofImprovementCycloneStatus;
                    }
                }

            }
            Alarm = quoteDetails.AlarmSystem;
            YearsPreviouslyInsured = quoteDetails.PreviousInsuranceTime;

            ClickControl(XP_CONTINUE_ACDN_2_BTN);
        }

        /// <summary>
        /// Answer disclosure questions. NOTE: if the notice for
        /// declined cover is triggered, automation will verify
        /// and then clear it before proceeding with the scenario.
        /// </summary>
        /// <param name="quoteDetails"></param>
        public void FillQuoteDetailsThirdAccordion(QuoteHome quoteDetails)
        {
            if (quoteDetails.HasPastConvictions)
            {
                HasPastCriminalConvictions = true;
                WaitForDeclinedCoverNoticeAndDismiss();
            }
            HasPastCriminalConvictions = false;

            if (quoteDetails.PastClaims == null || quoteDetails.PastClaims.Count == 0)
            {
                HasPastHomeClaims = false;
            }
            else
            {
                HasPastHomeClaims = true;

                for (int i = 0; i < quoteDetails.PastClaims.Count; i++)
                {
                    // The first row (i=0) already displays by default
                    // Only have to add rows for subsequent items.
                    if (i > 0)
                        ClickAddClaimHistoryRow();

                    SetClaimHistory(i, quoteDetails.PastClaims[i]);
                }
            }

            ClickControl(XP_CONTINUE_ACDN_3_BTN);

            if (IsDeclineNoticeExpectedFromClaimsHistory(quoteDetails))
            {
                WaitForDeclinedCoverNoticeAndDismiss();

                Reporting.Log("We encountered declined cover notice as expected. Clearing claims history to enable test to proceed.");
                // Clear some disclosures
                HasPastHomeClaims = false;
                quoteDetails.PastClaims = null;

                ClickControl(XP_CONTINUE_ACDN_3_BTN);
            }
        }

        /// <summary>
        /// Complete sum insured values and oldest policyholder question
        /// then submit for quote.
        /// </summary>
        /// <param name="quoteDetails"></param>
        public void FillQuoteDetailsFourthAccordionAndSubmit(QuoteHome quoteDetails)
        {
            if (quoteDetails.BuildingValue.HasValue)
                BuildingSumInsured = quoteDetails.BuildingValue.Value;

            // Renters do not get prompted for contents insurance SI until quote page.
            if (quoteDetails.Occupancy != HomeOccupancy.Tenant)
            {
                if (quoteDetails.ContentsValue.HasValue)
                    ContentsSumInsured = quoteDetails.ContentsValue.Value;
            }

            var oldestAge = 0;
            var hasRACMembership = false;

            foreach (var contact in quoteDetails.PolicyHolders)
            {
                oldestAge = contact.GetContactAge() > oldestAge ? contact.GetContactAge() : oldestAge;
                hasRACMembership &= contact.IsRACMember;
            }

            IsARACMember = hasRACMembership;
            OldestPHAge  = oldestAge;
            Email        = quoteDetails.PolicyHolders[0].GetEmail();

            SubmitForm();
        }

        /// <summary>
        /// Clicks the 'Get Quote' control to submit the page 1 form to
        /// get a quote created. Will fail if the Cover Details accordion
        /// is not already opened.
        /// </summary>
        public void SubmitForm()
        {
            ClickControl(XP_CONTINUE_ACDN_4_BTN);
        }

        /// <summary>
        /// Wrapper around FillQuoteDetails..() methods for first
        /// and second accordions.
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <param name="submit"></param>
        /// <returns></returns>
        public void FillQuoteDetails(QuoteHome quoteDetails)
        {
            FillQuoteDetailsFirstAccordion(quoteDetails);
            Thread.Sleep(1000);
            FillQuoteDetailsSecondAccordion(quoteDetails);
            Thread.Sleep(1000);
            FillQuoteDetailsThirdAccordion(quoteDetails);
            Thread.Sleep(1000);
            FillQuoteDetailsFourthAccordionAndSubmit(quoteDetails);
        }

        private void SetClaimHistory(int eventIndex, ClaimHistory item)
        {
            var claim_type_controls_identifier = $"'Question_PolicyDisclosures_DisclosureClaims_{eventIndex}__ClaimType_listbox'";
            var xpath_claim_type_input  = $"//span[@aria-owns={claim_type_controls_identifier}]";
            var xpath_claim_type_list   = $"id({claim_type_controls_identifier})/li";
            var claim_year_controls_identifier = $"'Question_PolicyDisclosures_DisclosureClaims_{eventIndex}__ClaimYear_listbox'";
            var xpath_claim_year_input = $"//span[@aria-owns={claim_year_controls_identifier}]";
            var xpath_claim_year_list  = $"id({claim_year_controls_identifier})/li";

            _driver.WaitForElementToBeVisible(By.XPath(xpath_claim_type_input), WaitTimes.T30SEC);

            WaitForSelectableAndPickFromDropdown(xpath_claim_type_input, xpath_claim_type_list, item.ClaimType.GetDescription());
            WaitForSelectableAndPickFromDropdown(xpath_claim_year_input, xpath_claim_year_list, item.Year.ToString());
        }

        private void ClickAddClaimHistoryRow()
        {
            ClickControl(XP_ADD_CLAIM_HISTORY_BTN);

            // Allow new control time to render.
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Table reference for thresholds:
        /// https://rac-wa.atlassian.net/wiki/spaces/SB/pages/882114562/Home+Quotes#Interface-Definition-%E2%80%93-Display-Home-Quote-Questions
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <returns></returns>
        private bool IsDeclineNoticeExpectedFromClaimsHistory(QuoteHome quoteDetails)
        {
            if (quoteDetails.PastClaims == null)
                return false;

            int toleratedClaims = MAXIMUM_CLAIMS_HISTORY;
            switch(quoteDetails.PreviousInsuranceTime)
            {
                case HomePreviousInsuranceTime.Zero:
                case HomePreviousInsuranceTime.LessThan1:
                    toleratedClaims = 1;
                    break;
                case HomePreviousInsuranceTime.One:
                    toleratedClaims = 2;
                    break;
                case HomePreviousInsuranceTime.Two:
                    toleratedClaims = 3;
                    break;
                case HomePreviousInsuranceTime.Three:
                    toleratedClaims = 4;
                    break;
                case HomePreviousInsuranceTime.Four:
                case HomePreviousInsuranceTime.FivePlus:
                    toleratedClaims = MAXIMUM_CLAIMS_HISTORY;
                    break;
                default:
                    Reporting.Error($"Encountered an unexpected case for Home previous insurance time: {quoteDetails.PreviousInsuranceTime}");
                    break;
            }

            return quoteDetails.PastClaims.Count > toleratedClaims;
        }

        private void WaitForDeclinedCoverNoticeAndDismiss()
        {
            _driver.WaitForElementToBeVisible(By.XPath(XP_DECLINED_NOTICE_HEADER), WaitTimes.T5SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_DECLINED_NOTICE_TEXT), WaitTimes.T5SEC);
            Reporting.Log("Cover declined notice triggered as expected.", _driver.TakeSnapshot());
            Reporting.AreEqual(DECLINE_NOTICE_HEADER_TEXT, GetInnerText(XP_DECLINED_NOTICE_HEADER), false);

            var dialogText = GetInnerText(XP_DECLINED_NOTICE_TEXT).StripLineFeedAndCarriageReturns();
            Regex declineDialogRegEx = new Regex(FixedTextRegex.QUOTE_COVER_DECLINED_TEXT);
            Match match = declineDialogRegEx.Match(dialogText);
            Reporting.IsTrue(match.Success, $"Asserting the cover declined dialog text ");

            ClickControl(XP_DECLINED_NOTICE_DISMISS);
        }
    }
}