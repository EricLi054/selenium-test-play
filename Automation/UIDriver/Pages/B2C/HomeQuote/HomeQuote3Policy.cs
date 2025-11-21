using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace UIDriver.Pages.B2C
{
    public class HomeQuote3Policy : BasePage
    {
        // The maximum number of policyholders allowed on a home policy.
        private const int MAX_POLICYHOLDERS = 4;

        #region XPATHS
        // General
        private const string BASE            = "/html/body/div[@id='wrapper']";

        private const string XP_PAGE_HEADING = BASE + "//span[@class='action-heading']/span";
        private const string XP_PANEL_BODY_X = BASE + "//div/div[starts-with(@class,'accordion-panel')]";

        private const string XPR_YES         = "//span[text()='Yes']/..";
        private const string XPR_NO          = "//span[text()='No']/..";
        private const string XPR_EMAIL       = "//span[text()='Email']/..";
        private const string XPR_POST        = "//span[text()='Post']/..";

        // Building details
        private const string XP_ROOF_MATERIAL = "//span[@aria-owns='Policy_RoofConstruction_listbox']";
        private const string XP_ROOF_MATERIAL_OPTION = "id('Policy_RoofConstruction_listbox')/li";
        private const string XP_CURRENT_INSURER        = "//span[@aria-owns='Policy_CurrentInsurer_listbox']";
        private const string XP_CURRENT_INSURER_OPTION = "id('Policy_CurrentInsurer_listbox')/li";
        private const string XP_IS_BUILDING_SOUND_YN   = "id('Policy_IsPropertyStructurallySound')";
        private const string XP_IS_MORTGAGED_YN        = "id('Policy_IsFinanced')";
        private const string XP_FINANCIER              = "//span[@aria-owns='Policy_FinanceCompany_listbox']";
        private const string XP_FINANCIER_OPTION       = "id('Policy_FinanceCompany_listbox')/li";
        private const string XP_BUILDING_DETAILS_CONTINUE_BUTTON = "//button[@id='accordion_0']";

        // Home details
        private const string XP_HOME_DETAILS_ACCORDION = "//div[@data-accordion-id='Policy_IsPropertyFailingOnlineConditions']";
        private const string XP_HOME_USE_DISCLOSURE_YN = "id('Policy_IsPropertyFailingOnlineConditions')";
        private const string XP_HOME_DETAILS_CONTINUE_BUTTON = "//button[@id='accordion_1']";

        // Policyholder details
        // To be populated with zero-base index 
        private const string XP_BASE_POLICYHOLDER_ROW_X = "id('Policy_Contacts_{0}__row')";

        private const string XPR_ACCORDION_HEADING = "/div[contains(@class,'accordion-heading')]";
        private const string XPR_PH_TITLE          = "//span[contains(@aria-owns,'__Name_Title_listbox')]";
        private const string XP_PH_TITLE_DROPDOWN  = "//ul[contains(@id,'_Name_Title_listbox') and @aria-hidden='false']/li";
        private const string XPR_PH_FIRSTNAME      = "//input[contains(@id,'__Name_FirstName')]";
        private const string XPR_PH_MIDDLENAME     = "//input[contains(@id,'__Name_MiddleName')]";
        private const string XPR_PH_SURNAME        = "//input[contains(@id,'__Name_LastName')]";
        private const string XPR_PH_DOB_DAY        = "//input[contains(@id,'__Dob_Day')]";
        private const string XPR_PH_DOB_MONTH      = "//input[contains(@id,'__Dob_Month')]";
        private const string XPR_PH_DOB_YEAR       = "//input[contains(@id,'__Dob_Year')]";
        private const string XPR_IS_ADDRESS_THE_SAME_YN  = "//div[contains(@id,'__IsMailingAddressSameAs')]";
        private const string XPR_MAILINGADDRESS    = "//input[contains(@id,'__MailingAddress_qasautocomplete')]";
        private const string XPR_ADDR_SUGGESTION   = "//div[contains(@id,'__MailingAddress')]//table[@class='address-find-table']//tr/td[1]";
        private const string XPR_PH_PHONE          = "//input[contains(@id,'__PhoneNumber')]";
        private const string XPR_PH_EMAIL          = "//input[contains(@id,'__EmailAddress')]";
        private const string XPR_PH_GENDER         = "//span[contains(@aria-owns,'__Gender_listbox')]";
        private const string XP_PH_GENDER_DROPDOWN = "//ul[contains(@id,'__Gender_listbox') and @aria-hidden='false']/li";
        private const string XPR_DELIVER_BY_EMAIL_YN = "//div[contains(@id,'__RenewPolicyUsingEmail')]";

        private const string XPR_ADD_POLICYHOLDER_BTN = "//div[contains(@class,'addPolicyHolder')]";
        private const string XPR_CONTINUE_BTN         = "//button[contains(@class,'accordion-button')]";
        #endregion

        #region Settable properties and controls
        private HomeRoof Roof
        {
            get => DataHelper.GetValueFromDescription<HomeRoof>(GetInnerText(XP_ROOF_MATERIAL));
            set => WaitForSelectableAndPickFromDropdown(XP_ROOF_MATERIAL,
                                                        XP_ROOF_MATERIAL_OPTION,
                                                        value.GetDescription());
        }

        private string CurrentInsurer
        {
            get => GetInnerText($"{XP_CURRENT_INSURER}{XPEXT_DROPDOWN_VALUE}");
            set => WaitForSelectableAndPickFromDropdown(XP_CURRENT_INSURER, XP_CURRENT_INSURER_OPTION, value);
        }

        private bool IsSoundSecureAndMaintained
        {
            get => GetBinaryToggleState(XP_IS_BUILDING_SOUND_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_IS_BUILDING_SOUND_YN, XPR_YES, XPR_NO, value);
        }

        private string Financier
        {
            get
            {
                string currentFinancier = null;
                if (GetBinaryToggleState(XP_IS_MORTGAGED_YN, XPR_YES, XPR_NO))
                {
                    currentFinancier = GetInnerText($"{XP_FINANCIER}{XPEXT_DROPDOWN_VALUE}");
                }
                return currentFinancier;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    ClickControl($"{XP_IS_MORTGAGED_YN}{XPR_NO}");
                else
                {
                    ClickControl($"{XP_IS_MORTGAGED_YN}{XPR_YES}");

                    // Allow time for dropdown to render.
                    Thread.Sleep(3000);

                    WaitForSelectableAndPickFromDropdown(XP_FINANCIER, XP_FINANCIER_OPTION, value);
                }
            }
        }

        private bool IsHomeUsedInWaysThatProhibitOnlinePurchase
        {
            get => GetBinaryToggleState(XP_HOME_USE_DISCLOSURE_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_HOME_USE_DISCLOSURE_YN, XPR_YES, XPR_NO, value);
        }
        #endregion

        public HomeQuote3Policy(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_PAGE_HEADING);
                if (!heading.Text.ToLower().StartsWith("your quote:"))
                {
                    Reporting.Log("Wrong heading text for third page of Home Quote process.");
                    return false;
                }

                // With Cyclone functionality, new business no longer asks about roof material.
                if (!Config.Get().IsCycloneEnabled()) { GetElement(XP_ROOF_MATERIAL); }

                GetElement(XP_IS_BUILDING_SOUND_YN);
                GetElement(XP_IS_MORTGAGED_YN);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Home Quote page 3 - additional home and policyholder details");
            return true;
        }

        /// <summary>
        /// Fill in first accordion on building questions
        /// </summary>
        /// <param name="quoteDetails"></param>
        public void FillInBuildingDetails(QuoteHome quoteDetails)
        {
            // With Cyclone functionality, roof material is no longer asked.
            if (!Config.Get().IsCycloneEnabled()) { Roof = quoteDetails.RoofMaterial; }

            // This question only occurs if a non-zero previous insurance time was provided
            if (quoteDetails.PreviousInsuranceTime != HomePreviousInsuranceTime.Zero)
                CurrentInsurer = quoteDetails.CurrentInsurer;

            // Mortgage question is only applicable for building cover.
            if (quoteDetails.BuildingValue.HasValue)
                Financier = quoteDetails.Financier;

            // Always setting to true, otherwise business rules block quote
            IsSoundSecureAndMaintained = true;

            ClickControl(XP_BUILDING_DETAILS_CONTINUE_BUTTON);
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Fill in second accordion, which is declaration about
        /// condition of property.
        /// </summary>
        /// <param name="quoteDetails"></param>
        public void FillInHomeDetails(QuoteHome quoteDetails)
        {
            IsHomeUsedInWaysThatProhibitOnlinePurchase = quoteDetails.IsHomeUsageUnacceptable;

            ClickControl(XP_HOME_DETAILS_CONTINUE_BUTTON);
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Fill in the policyholder details
        /// </summary>
        /// <param name="policyholderDetails"></param>
        public void FillInPolicyholderDetails(QuoteHome quoteDetails)
        {
            for (int i = 0; i < quoteDetails.PolicyHolders.Count; i++)
            {
                var policyholderDetails = quoteDetails.PolicyHolders[i];
                var baseXPath = GetBaseXPathForPolicyHolderControls(i);

                WaitForSelectableAndPickFromDropdown($"{baseXPath}{XPR_PH_TITLE}", XP_PH_TITLE_DROPDOWN, policyholderDetails.Title.GetDescription());
                WaitForTextFieldAndEnterText($"{baseXPath}{XPR_PH_FIRSTNAME}", policyholderDetails.FirstName);
                if (!string.IsNullOrEmpty(policyholderDetails.MiddleName))
                    WaitForTextFieldAndEnterText($"{baseXPath}{XPR_PH_MIDDLENAME}", policyholderDetails.MiddleName);
                WaitForTextFieldAndEnterText($"{baseXPath}{XPR_PH_SURNAME}", policyholderDetails.Surname);

                WaitForTextFieldAndEnterText($"{baseXPath}{XPR_PH_DOB_DAY}", policyholderDetails.DateOfBirth.Day.ToString(), false);
                WaitForTextFieldAndEnterText($"{baseXPath}{XPR_PH_DOB_MONTH}", policyholderDetails.DateOfBirth.Month.ToString(), false);
                WaitForTextFieldAndEnterText($"{baseXPath}{XPR_PH_DOB_YEAR}", policyholderDetails.DateOfBirth.Year.ToString(), false);

                WaitForSelectableAndPickFromDropdown($"{baseXPath}{XPR_PH_GENDER}", XP_PH_GENDER_DROPDOWN, policyholderDetails.Gender.GetDescription());

                // Set mailing address:
                // First policy holder is asked if their mailing address is the same as the property
                // (Home Quotes Only) UNLESS the quote has been retrieved, in which case they must
                // enter their mailing address manually to prevent possible data contamination from
                // SHIELD Risk Address. Ref B2C-4382.
                if (i == 0)
                {
                    SetMailingAddress(policyholderDetails.MailingAddress, quoteDetails.PropertyAddress, i,
                                      hasBeenRetrieved: quoteDetails.QuoteHasBeenRetrieved);
                }

                // All co-policy holders are asked if their address is the same as the main policy holder.
                else
                {
                    SetMailingAddress(policyholderDetails.MailingAddress, quoteDetails.PolicyHolders[0].MailingAddress, i);
                }

                WaitForTextFieldAndEnterText($"{baseXPath}{XPR_PH_PHONE}", policyholderDetails.GetPhone());
                WaitForTextFieldAndEnterText($"{baseXPath}{XPR_PH_EMAIL}", policyholderDetails.GetEmail());

                // First policy holder only:
                // Set preferred delivery method for policyholder.
                if (i == 0)
                    ClickBinaryToggle($"{baseXPath}{XPR_DELIVER_BY_EMAIL_YN}", XPR_POST, XPR_EMAIL, policyholderDetails.MailingAddress.IsPreferredDeliveryMethod);

                // Add another contact if there are more, and we're not yet at the max.
                // We add 1 to i, as it's a zero-based counter.
                if ((i+1) < quoteDetails.PolicyHolders.Count && 
                    (i+1) < MAX_POLICYHOLDERS)
                    ClickControl($"{baseXPath}{XPR_ADD_POLICYHOLDER_BTN}");
                else
                    ClickControl($"{baseXPath}{XPR_CONTINUE_BTN}");

                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// For cases where the page 3 details had been previously completed and
        /// the quote has been later retrieved, then the home quote should
        /// retain all values. This method verifies the Building Details
        /// accordion then proceeds to the Home Details accordion.
        /// </summary>
        public void VerifyPopulatedBuildingDetails(QuoteHome quoteDetails)
        {
            if (!Config.Get().IsCycloneEnabled()) { Reporting.AreEqual(quoteDetails.RoofMaterial, Roof, "roof reflects previously populated value"); }
            if (quoteDetails.PreviousInsuranceTime != HomePreviousInsuranceTime.Zero)
                Reporting.AreEqual(quoteDetails.CurrentInsurer, CurrentInsurer, "current insurer reflects previously populated value");

            Reporting.IsTrue(IsSoundSecureAndMaintained, "building is structurally sound disclosure should reflect previously set TRUE value");

            ClickControl(XP_BUILDING_DETAILS_CONTINUE_BUTTON);
            Thread.Sleep(2000);
        }

        /// <summary>
        /// For cases where the page 3 details had been previously completed and
        /// the quote has been later retrieved, then the home quote should
        /// retain all values. This method verifies the Home Details accordion
        /// then proceeds to the policyholder details accordion.
        /// </summary>
        public void VerifyPopulatedHomeDetails(QuoteHome quoteDetails)
        {
            Reporting.AreEqual(quoteDetails.IsHomeUsageUnacceptable, IsHomeUsedInWaysThatProhibitOnlinePurchase, "home usage declaration should retain previously set value");

            ClickControl(XP_HOME_DETAILS_CONTINUE_BUTTON);
            Thread.Sleep(2000);
        }

        /// <summary>
        /// For cases where the page 3 details had been previously completed and
        /// the quote has been later retrieved, then the home quote should
        /// retain all values. This method verifies all previously entered
        /// policyholder details. Mailing addresses are re-entered as they are
        /// not retained.
        /// </summary>
        public void VerifyPolicyholderDetails(QuoteHome quoteDetails)
        {
            for (int i = 0; i < quoteDetails.PolicyHolders.Count; i++)
            {
                var policyholderDetails = quoteDetails.PolicyHolders[i];
                var baseXPath = GetBaseXPathForPolicyHolderControls(i);

                var currentTitle = DataHelper.GetValueFromDescription<Title>(GetInnerText($"{baseXPath}{XPR_PH_TITLE}"));
                Reporting.AreEqual(policyholderDetails.Title, currentTitle, "title has retained previously set value");
                Reporting.AreEqual(policyholderDetails.FirstName, GetValue($"{baseXPath}{XPR_PH_FIRSTNAME}"), false);
                Reporting.AreEqual(policyholderDetails.MiddleName, GetValue($"{baseXPath}{XPR_PH_MIDDLENAME}"), false);
                Reporting.AreEqual(policyholderDetails.Surname, GetValue($"{baseXPath}{XPR_PH_SURNAME}"), false);

                Reporting.AreEqual(policyholderDetails.DateOfBirth.Day.ToString(), GetValue($"{baseXPath}{XPR_PH_DOB_DAY}"), false);
                Reporting.AreEqual(policyholderDetails.DateOfBirth.Month.ToString(), GetValue($"{baseXPath}{XPR_PH_DOB_MONTH}"), false);
                Reporting.AreEqual(policyholderDetails.DateOfBirth.Year.ToString(), GetValue($"{baseXPath}{XPR_PH_DOB_YEAR}"), false);

                var currentGender = DataHelper.GetValueFromDescription<Gender>(GetInnerText($"{baseXPath}{XPR_PH_GENDER}"));
                Reporting.AreEqual(policyholderDetails.Gender, currentGender, "gender has retained previously set value.");

                // Mailing address is not retained with retrieve quote so this needs to be set again.
                // First policy holder is asked if their address is the same as the property
                if (i == 0)
                {
                    SetMailingAddress(policyholderDetails.MailingAddress, quoteDetails.PropertyAddress, i,
                                      hasBeenRetrieved: quoteDetails.QuoteHasBeenRetrieved);
                }

                // All co-policy holders are asked if their address is the same as the main policy holder.
                else
                {
                    SetMailingAddress(policyholderDetails.MailingAddress, quoteDetails.PolicyHolders[0].MailingAddress, i);
                }

                Reporting.AreEqual(policyholderDetails.GetPhone(), GetValue($"{baseXPath}{XPR_PH_PHONE}"), false);
                Reporting.AreEqual(policyholderDetails.GetEmail(), GetValue($"{baseXPath}{XPR_PH_EMAIL}"), ignoreCase: true);

                // First policy holder only:
                // Always setting to email. No test automation scenarios cover postal correspondence.
                if (i == 0)
                {
                    var isPDMEmail = !policyholderDetails.MailingAddress.IsPreferredDeliveryMethod;
                    Reporting.AreEqual(policyholderDetails.MailingAddress.IsPreferredDeliveryMethod, GetBinaryToggleState($"{baseXPath}{XPR_DELIVER_BY_EMAIL_YN}", XPR_POST, XPR_EMAIL), "preferred delivery method should retain previously set value");
                }
                
                ClickControl($"{baseXPath}{XPR_CONTINUE_BTN}");
                // Allows for accordion transition animation and scroll
                Thread.Sleep(2000);
            }
        }


        /// <summary>
        /// Built to handle cases where the callback dialog has been
        /// triggered due to Home Details disclosure (failing online
        /// conditions). We dismiss the dialog, change the disclosure
        /// to an acceptable answer and then resubmit the page.
        /// </summary>
        public void ClearHomeDetailsDisclosureAndSubmitPage(QuoteHome quoteDetails)
        {
            ClickControl(XP_HOME_DETAILS_ACCORDION);

            quoteDetails.IsHomeUsageUnacceptable       = false;
            IsHomeUsedInWaysThatProhibitOnlinePurchase = false;

            // Open last policyholder accordion and click Continue. (-1 as it is zero based index)
            var baseXPath = GetBaseXPathForPolicyHolderControls(quoteDetails.PolicyHolders.Count - 1);
            ClickControl($"{baseXPath}{XPR_ACCORDION_HEADING}");
            Thread.Sleep(2000);  // Allow time for accordion transition animation.
            ClickControl($"{baseXPath}{XPR_CONTINUE_BTN}");
        }

        private string GetBaseXPathForPolicyHolderControls(int index)
        {
            return string.Format(XP_BASE_POLICYHOLDER_ROW_X, index.ToString());
        }

        /// <summary>
        /// Fills in the mailing address for a home PH or coPH, including answering
        /// the question as to whether the mailing address is the same as the home
        /// (or main PH, if the mailing address relates to a coPH).
        /// </summary>
        /// <param name="contactAddress">Address to be entered for the referenced policyholder</param>
        /// <param name="comparisonAddress">Address to be compared against to determine whether to answer Y/N to "is address the same?" question</param>
        /// <param name="index">0-based index, indicating which policyholder we're addressing to support XPaths</param>
        /// <param name="hasBeenRetrieved">Indicates if quote has been retrieved, which changes UI behaviour if B2C contacts QAS directly</param>
        private void SetMailingAddress(Address contactAddress, 
                                       Address comparisonAddress,
                                       int index, bool hasBeenRetrieved = false)
        {
            var baseXPath = GetBaseXPathForPolicyHolderControls(index);
            var isMailingAddressSameAsComparison = comparisonAddress.StreetSuburbPostcode(expandUnitAddresses: true).Equals(contactAddress.StreetSuburbPostcode(expandUnitAddresses: true));

            // If we are in the case where the quote HAS been retrieved and
            // B2C is using qas directly, then the prompt for whether main PH
            // mailing address is same as risk address, is NOT offered.
            if (!(hasBeenRetrieved && Config.Get().IsUseAddressManagementApiEnabled))
            {
                ClickBinaryToggle($"{baseXPath}{XPR_IS_ADDRESS_THE_SAME_YN}", XPR_YES, XPR_NO, isMailingAddressSameAsComparison);
            }
                       
            if (!isMailingAddressSameAsComparison ||
                (hasBeenRetrieved && Config.Get().IsUseAddressManagementApiEnabled))
            {
                QASSearchForAddress($"{baseXPath}{XPR_MAILINGADDRESS}",
                                    $"{baseXPath}{XPR_ADDR_SUGGESTION}",
                                    contactAddress.StreetSuburbState());
            }
            Reporting.Log("End of SetMailingAddress section, taking screenshot.", _browser.Driver.TakeSnapshot());
        }
    }
}
