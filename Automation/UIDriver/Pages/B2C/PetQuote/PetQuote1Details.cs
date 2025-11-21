using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyPet;

namespace UIDriver.Pages.B2C
{
    public class PetQuote1Details : BasePage
    {
        #region XPATHS
        // Segments
        private const string BASE            = "/html/body/div[@id='wrapper']";
        private const string FORM            = BASE + "//form[@class='insuranceForm questionForm formTop']";

        // General
        private const string XP_PAGE_HEADING = BASE + "//span[@class='action-heading']/span";

        // Pet details
        private const string XP_PET_ACCORDION_HEADER = FORM + "//div[@data-accordion-id='Question_Pet']";
        private const string XP_PET_TYPE_CAT_DOG  = "id('Question_Pet_Type')";
        private const string XPR_CAT              = "//span[text()='Cat']/..";
        private const string XPR_DOG              = "//span[text()='Dog']/..";
        private const string XP_PET_BREED         = FORM + "//span[@aria-owns='Question_Pet_BreedId_listbox']";
        private const string XP_PET_BREED_OPTION  = "id('Question_Pet_BreedId_listbox')/li";
        private const string XP_PET_NAME          = "id('Question_Pet_Name')";
        private const string XP_PET_DOB_DAY       = "id('Question_Pet_Dob_Day')";
        private const string XP_PET_DOB_MONTH     = "id('Question_Pet_Dob_Month')";
        private const string XP_PET_DOB_YEAR      = "id('Question_Pet_Dob_Year')";
        private const string XP_PET_SUBURB        = "id('Question_Pet_Suburb_Suburb')";
        private const string XP_CONTINUE_ACDN_1_BTN = "id('accordion_0')";

        // PH details
        private const string XP_DOB_DAY           = "id('Question_PetContact_DateOfBirth_Day')";
        private const string XP_DOB_MONTH         = "id('Question_PetContact_DateOfBirth_Month')";
        private const string XP_DOB_YEAR          = "id('Question_PetContact_DateOfBirth_Year')";
        private const string XP_IS_MEMBER_YN      = "id('Question_PetContact_IsMember')";
        private const string XPR_YES              = "//span[text()='Yes']/..";
        private const string XPR_NO               = "//span[text()='No']/..";

        private const string XP_MEMBERSHIP_LEVEL  = FORM + "//span[@aria-owns='Question_PetContact_MembershipLevel_listbox']";
        private const string XP_EMAIL_ADDRESS        = "id('Question_PetContact_Email')";
        private const string XP_CONTINUE_QUOTE_BTN   = "id('accordion_1_submit-action')";
        #endregion

        #region Settable properties and controls
        public PetType PetType
        {
            get
            {
                var isDog = GetBinaryToggleState(XP_PET_TYPE_CAT_DOG, XPR_DOG, XPR_CAT);
                return isDog ? PetType.Dog : PetType.Cat;
            }
            set => ClickBinaryToggle(XP_PET_TYPE_CAT_DOG, XPR_DOG, XPR_CAT, value == PetType.Dog);
        }

        public string PetBreed
        {
            get => GetInnerText(XP_PET_BREED);
            set => WaitForSelectableAndPickFromDropdown(XP_PET_BREED, XP_PET_BREED_OPTION, value.ToUpper());
        }

        public string PetName
        {
            get => throw new NotImplementedException("TODO: Implement getter for Pet Name.");
            set => WaitForTextFieldAndEnterText(XP_PET_NAME, value);
        }

        public string PetSuburb
        {
            get => GetValue(XP_PET_SUBURB);
            set => WaitForTextFieldAndEnterText(XP_PET_SUBURB, value);

        }

        public DateTime PolicyHolderBirthdate
        {
            get => throw new NotImplementedException("TODO: Implement getter for PH DoB.");

            set
            {
                WaitForTextFieldAndEnterText(XP_DOB_DAY,   value.Day.ToString(), false);
                WaitForTextFieldAndEnterText(XP_DOB_MONTH, value.Month.ToString(), false);
                WaitForTextFieldAndEnterText(XP_DOB_YEAR,  value.Year.ToString(), false);
            }
        }

        public bool IsARACMember
        {
            get => GetBinaryToggleState(XP_IS_MEMBER_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_IS_MEMBER_YN, XPR_YES, XPR_NO, value);
        }

        public MembershipTier MembershipLevel
        {
            get => throw new NotImplementedException("TODO: Implement getter for RAC membership level");
            set => WaitForSelectableAndPickByTyping(XP_MEMBERSHIP_LEVEL, MembershipTierText[value]);
        }

        public string PolicyHolderEmail
        {
            get => GetValue(XP_EMAIL_ADDRESS);
            set => WaitForTextFieldAndEnterText(XP_EMAIL_ADDRESS, value);
        }
        #endregion

        public PetQuote1Details(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_PAGE_HEADING);
                if (!heading.Text.ToLower().Equals("pet quote"))
                {
                    Reporting.Log("Wrong heading text for Pet Quote page.");
                    return false;
                }
                GetElement(XP_PET_NAME);
                GetElement(XP_PET_SUBURB);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Pet Quote page 1 - Rating values");
            return true;
        }

        /// <summary>
        /// Fills the first accordion of page 1 of Pet Quote.
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <returns>Text of vehicle matched from rego/search</returns>
        public void FillQuoteDetailsFirstAccordion(QuotePet quoteDetails)
        {
            PetType   = quoteDetails.Type;
            PetName   = quoteDetails.Name;
            SetPetDateOfBirth(quoteDetails.DateOfBirth);
            PetSuburb = quoteDetails.ResidingAddress.SuburbAndCode();

            // Static sleep as form needs to fetch reference data for chosen pet type.
            Thread.Sleep(2000);
            PetBreed  = quoteDetails.Breed;

            ClickContinueFromPetDetails();
        }

        /// <summary>
        /// Fills the second accordion of page 1 of Pet Quote.
        /// Will add additional drivers as defined.
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <param name="submit"></param>
        public void FillQuoteDetailsSecondAccordion(QuotePet quoteDetails)
        {
            PolicyHolderBirthdate = quoteDetails.PolicyHolder.DateOfBirth;

            IsARACMember = quoteDetails.PolicyHolder.IsRACMember;

            if (IsARACMember)
            {
                MembershipLevel = quoteDetails.PolicyHolder.MembershipTier;
            }

            PolicyHolderEmail = quoteDetails.PolicyHolder.GetEmail();
        }

        /// <summary>
        /// Wrapper around FillQuoteDetails..() methods for first
        /// and second accordions.
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <param name="submit"></param>
        /// <returns></returns>
        public void FillQuoteDetails(QuotePet quoteDetails)
        {
            FillQuoteDetailsFirstAccordion(quoteDetails);
            Thread.Sleep(1000);
            FillQuoteDetailsSecondAccordion(quoteDetails);
        }

        /// <summary>
        /// Click continue to complete entered pet details
        /// </summary>
        public void ClickContinueFromPetDetails()
        {
            ClickControl(XP_CONTINUE_ACDN_1_BTN);
            
            // Animation from Pet Details panel to PolicyHolder Details is hard to detect. Using fixed sleep.
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Submit page 1 details for quote.
        /// </summary>
        /// <param name="driverIndex"></param>
        public void ClickContinueFromPolicyHolderDetails()
        {
            ClickControl(XP_CONTINUE_QUOTE_BTN);
        }

        private void SetPetDateOfBirth(DateTime birthdate)
        {
            WaitForTextFieldAndEnterText(XP_PET_DOB_DAY,   birthdate.Day.ToString(), hasTypeAhead: false);
            WaitForTextFieldAndEnterText(XP_PET_DOB_MONTH, birthdate.Month.ToString(), hasTypeAhead: false);
            WaitForTextFieldAndEnterText(XP_PET_DOB_YEAR,  birthdate.Year.ToString(), hasTypeAhead: false);
        }
    }
}
