using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.Contacts;

namespace UIDriver.Pages.B2C
{
    public class PetQuote3Policy : BasePage
    {
        #region XPATHS
        private const string BASE = "/html/body/div[@id='wrapper']";

        private const string XP_PAGE_HEADING           = BASE + "//span[@class='action-heading']/span";

        private const string XP_PET_HEADING            = BASE + "//div/div[@data-accordion-id='Policy']";
        private const string XP_PANEL_BODY_X           = BASE + "//div/div[starts-with(@class,'accordion-panel')]";

        private const string XPR_MALE                    = "//span[text()='Male']/..";
        private const string XPR_FEMALE                  = "//span[text()='Female']/..";
        private const string XPR_YES                     = "//span[text()='Yes']/..";
        private const string XPR_NO                      = "//span[text()='No']/..";

        private const string XP_PET_GENDER_MF            = "id('Policy_PolicyPetDetails_PetGender')";
        private const string XP_STERILISED_YN            = "id('Policy_PolicyPetDetails_IsSterilised')";
        private const string XP_PREEXISTING_INJURIES_YN  = "id('Policy_PolicyPetDetails_PreExistingIllness')";
        private const string XP_WORKING_ANIMAL_YN        = "id('Policy_PolicyPetDetails_PetUsageProhibited')";
        private const string XP_OWNED_BY_BUSINESS_YN     = "id('Policy_PolicyPetDetails_OwnedByBusinessOrTrust')";
        private const string XP_REGULAR_VET_YN           = "id('Policy_PolicyPetDetails_HasRegularVet')";
        private const string XP_VET_NAME                 = "//input[@id='Policy_PolicyPetDetails_VetName']";
        private const string XP_VET_ADDRESS              = "//input[@id='Policy_PolicyPetDetails_VetAddress']";
        private const string XP_CURRENTLY_INSURED_YN     = "id('Policy_CurrentlyInsured')";
        private const string XP_CURRENT_INSURER          = "//span[@aria-owns='Policy_CurrentInsurer_listbox']";
        private const string XP_CURRENT_INSURER_OPTION   = "id('Policy_CurrentInsurer_listbox')/li";

        private const string XP_PET_PDS_LINK             = "id('pds-url')";
        private const string XP_ACCEPT_PREEXISTING_TERMS = "id('Policy_NoPreexistingConditions')";

        private const string XP_DISCLOSURE_INSURANCE_YN  = "id('Policy_InsurerRefuseCancelVoidDisclosure')";
        private const string XP_DISCLOSURE_OFFENCES_YN   = "id('Policy_CriminalOffenceDisclosure')";

        private const string XP_PH_TITLE           = "//span[@aria-owns='Policy_Contacts_0__Name_Title_listbox']";
        private const string XP_PH_FIRSTNAME       = "//input[@id='Policy_Contacts_0__Name_FirstName']";
        private const string XP_PH_MIDDLENAME      = "//input[@id='Policy_Contacts_0__Name_MiddleName']";
        private const string XP_PH_SURNAME         = "//input[@id='Policy_Contacts_0__Name_LastName']";
        private const string XP_PH_DOB             = "//div[@data-wrapper-for='Policy_Contacts_0__Dob']//div[@class='display-answer']";
        private const string XP_MAILINGADDRESS     = "//input[@id='Policy_Contacts_0__MailingAddress_qasautocomplete']";
        private const string XP_ADDR_SUGGESTION    = "//div[@id='Policy_Contacts_0__MailingAddress']//table[@class='address-find-table']//tr/td[1]";
        private const string XP_PH_PHONE           = "//input[@id='Policy_Contacts_0__PhoneNumber']";
        private const string XP_PH_EMAIL           = "//input[@id='Policy_Contacts_0__EmailAddress']";
        private const string XP_PH_GENDER_MF       = "id('Policy_Contacts_0__Gender')";

        // Relative control, to be used in conjunction with XP_PANEL_BODY_X
        private const string XPR_CONTINUE_BTN       = "//button[contains(@class,'accordion-button')]";
        // PolicyHolder accordion is nested, breaking the find pattern we were using
        private const string XP_PH_CONTINUE_BUTTON  = "//button[@id='accordion_1']";
        #endregion

        #region Settable properties and controls
        public Gender PetGender
        {
            get => GetBinaryToggleState(XP_PET_GENDER_MF, XPR_FEMALE, XPR_MALE) ?
                       Gender.Female :
                       Gender.Male;

            set => ClickBinaryToggle(XP_PET_GENDER_MF,
                                     XPR_FEMALE,
                                     XPR_MALE, 
                                     value == Gender.Female);
        }

        public bool IsSterilised
        {
            get => GetBinaryToggleState(XP_STERILISED_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_STERILISED_YN, XPR_YES, XPR_NO, value);

        }

        public bool HasPreExistingInjuries
        {
            get => GetBinaryToggleState(XP_PREEXISTING_INJURIES_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_PREEXISTING_INJURIES_YN, XPR_YES, XPR_NO, value);
        }

        public bool IsWorkingAnimal
        {
            get => GetBinaryToggleState(XP_WORKING_ANIMAL_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_WORKING_ANIMAL_YN, XPR_YES, XPR_NO, value);
        }

        public bool IsOwnedByABusiness
        {
            get => GetBinaryToggleState(XP_OWNED_BY_BUSINESS_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_OWNED_BY_BUSINESS_YN, XPR_YES, XPR_NO, value);
        }

        public string CurrentRecentInsurer
        {
            get
            {
                string currentInsurer = null;
                var hasInsurer = GetBinaryToggleState(XP_CURRENTLY_INSURED_YN, XPR_YES, XPR_NO);
                if (hasInsurer)
                {
                    currentInsurer = GetInnerText(XP_CURRENT_INSURER);
                }
                return currentInsurer;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    ClickControl($"{XP_CURRENTLY_INSURED_YN}{XPR_NO}");
                else
                {
                    ClickControl($"{XP_CURRENTLY_INSURED_YN}{XPR_YES}");

                    // Allow time for dropdown to render.
                    Thread.Sleep(2000);

                    WaitForSelectableAndPickFromDropdown(XP_CURRENT_INSURER, XP_CURRENT_INSURER_OPTION, value.ToUpper());
                }
            }
        }
        #endregion

        public PetQuote3Policy(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_PAGE_HEADING);
                if (!heading.Text.ToLower().StartsWith("your quote:"))
                {
                    Reporting.Log("Wrong heading text for third page of Pet Quote process.");
                    return false;
                }
                GetElement(XP_PET_HEADING);
                // Pet panel is first accordion
                GetElement(XP_PET_GENDER_MF);
                GetElement(XP_STERILISED_YN);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Pet Quote page 3 - additional pet and policyholder details");
            return true;
        }

        /// <summary>
        /// Sets the vet details, including the related Yes/No toggle buttons.
        /// </summary>
        /// <returns></returns>
        public void SetRegularVet(string vetName, string vetAddress)
        {
            if (string.IsNullOrEmpty(vetName) && string.IsNullOrEmpty(vetAddress))
            {
                ClickControl($"{XP_REGULAR_VET_YN}{XPR_NO}");
            }
            else
            {
                ClickControl($"{XP_REGULAR_VET_YN}{XPR_YES}");

                WaitForTextFieldAndEnterText(XP_VET_NAME, vetName, false);
                WaitForTextFieldAndEnterText(XP_VET_ADDRESS, vetAddress, false);
            }
        }

        /// <summary>
        /// Helper method to populate the additional pet details fields
        /// </summary>
        /// <param name="petDetails"></param>
        public void FillInPetDetails(QuotePet petDetails)
        {
            PetGender              = petDetails.Gender;
            IsSterilised           = petDetails.IsSterilised;

            Reporting.IsFalse(IsControlDisplayed(XP_PREEXISTING_INJURIES_YN), $"Pre-existing illness question should not be displayed");
            Reporting.IsTrue(IsControlDisplayed(XP_PET_PDS_LINK), "PDS link for pre-existing illness disclosure is shown");
            ClickControl(XP_ACCEPT_PREEXISTING_TERMS);
            Reporting.Log("Acceptance of pre-existing illness disclosure requirements", _driver.TakeSnapshot());

            IsWorkingAnimal        = petDetails.IsWorkingAnimal;
            IsOwnedByABusiness     = petDetails.IsBusinessOwned;
            SetRegularVet(petDetails.VetName, petDetails.VetAddress);
            CurrentRecentInsurer   = petDetails.CurrentInsurer;
        }

        /// <summary>
        /// Fill in the policyholder details
        /// </summary>
        /// <param name="policyholderDetails"></param>
        public void FillInPolicyholderDetails(Contact policyholderDetails)
        {
            WaitForSelectableAndPickByTyping(XP_PH_TITLE, policyholderDetails.Title.GetDescription());
            WaitForTextFieldAndEnterText(XP_PH_FIRSTNAME, policyholderDetails.FirstName);
            if (!string.IsNullOrEmpty(policyholderDetails.MiddleName))
                WaitForTextFieldAndEnterText(XP_PH_MIDDLENAME, policyholderDetails.MiddleName);
            WaitForTextFieldAndEnterText(XP_PH_SURNAME, policyholderDetails.Surname);

            var dobText = GetElement(XP_PH_DOB).Text;
            Reporting.AreEqual(policyholderDetails.DateOfBirth.ToString("d MMM yyyy"), dobText);

            ClickBinaryToggle(XP_PH_GENDER_MF, XPR_MALE, XPR_FEMALE, policyholderDetails.Gender == Gender.Male);

            QASSearchForAddress(XP_MAILINGADDRESS, XP_ADDR_SUGGESTION, policyholderDetails.MailingAddress.StreetSuburbState());

            WaitForTextFieldAndEnterText(XP_PH_PHONE, policyholderDetails.GetPhone());
            WaitForTextFieldAndEnterText(XP_PH_EMAIL, policyholderDetails.GetEmail());
        }

        /// <summary>
        /// Hardcoded to answer "NO" for all disclosures.
        /// </summary>
        public void FillInDisclosureAndSubmit()
        {
            ClickControl($"{XP_DISCLOSURE_INSURANCE_YN}{XPR_NO}");
            ClickControl($"{XP_DISCLOSURE_OFFENCES_YN}{XPR_NO}");
            ClickDisclosuresContinueButton();
        }

        public void ClickPetDetailsContinueButton()
        {
            ClickControl($"{XP_PANEL_BODY_X}[1]{XPR_CONTINUE_BTN}");

            // Animation from Car Details panel to Membership/Driver Details is hard to detect. Using fixed sleep.
            Thread.Sleep(2000);
        }

        public void ClickPolicyholderDetailsContinueButton()
        {
            ClickControl(XP_PH_CONTINUE_BUTTON);

            // Animation from Membership Details panel to Driver Details is hard to detect. Using fixed sleep.
            Thread.Sleep(2000);
        }

        public void ClickDisclosuresContinueButton()
        {
            ClickControl($"{XP_PANEL_BODY_X}[2]{XPR_CONTINUE_BTN}");
        }
    }
}
