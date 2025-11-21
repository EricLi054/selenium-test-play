using Rac.TestAutomation.Common;
using System;
using OpenQA.Selenium;

namespace UIDriver.Pages.B2C
{
    public class PetQuote3Summary : BasePage
    {
        #region XPATHS
        // XP key references:
        private const string BASE    = "/html/body/div[@id='wrapper']";
        private const string PANEL   = BASE + "//form//div[contains(@class,'accordion-panel')]";
        private const string BLOCK_X = PANEL + "//div[@class='summary-block']";
        private const string XP_HEADING = BASE + "//span[@class='action-heading']/span";

        // Insurance items:
        private const string XP_PET_NAME   = "//div[@id='Summary_PetName_Answer']";
        private const string XP_PET_BREED  = "//div[@id='Summary_PetBreed_Answer']";
        private const string XP_PET_AGE    = "//div[@id='Summary_PetAge_Answer']";
        private const string XP_PET_SUBURB = "//div[@id='Summary_PetSuburb_Answer']";

        private const string XP_EXCESS     = "//div[@id='Summary_Excess_Answer']";
        private const string XP_START_DATE = "//div[@id='Summary_PolicyStartDate_Answer']";
        private const string XP_TLC_COVER  = "//div[@id='Summary_TlcInclusionStatus_Answer']";

        // Driver X items:
        private const string XP_PH_NAME = "//div[@id='Summary_Contacts_0__Name_Answer']";
        private const string XP_ADDRESS = "//div[@id='Summary_Contacts_0__MailingAddress_Answer']";
        private const string XP_PHONE   = "//div[@id='Summary_Contacts_0__PrimaryContactNumber_Answer']";
        private const string XP_EMAIL   = "//div[@id='Summary_Contacts_0__EmailAddress_Answer']";

        private const string XP_CONTINUE_BTN = PANEL + "//button[@id='accordion_0_submit-action']";
        #endregion

        #region Settable properties and controls
        public string PetName =>  GetInnerText(XP_PET_NAME);

        public string PetBreed => GetInnerText(XP_PET_BREED);

        public string PetAge => GetInnerText(XP_PET_AGE);

        public string PetSuburbOfResidence => GetInnerText(XP_PET_SUBURB);

        public string ValueExcess => GetInnerText(XP_EXCESS);


        /// <summary>
        /// Fetch row indicating the "Tender Loving Care" cover has been added.
        /// </summary>
        public bool HasTLCCover
        {
            get
            {
                var tlcText = GetElement(XP_TLC_COVER).Text.Trim();
                return tlcText.Equals("Included", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public string StartDate => GetElement(XP_START_DATE).Text.Trim();
        #endregion

        public PetQuote3Summary(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_HEADING);
                if (!heading.Text.ToLower().StartsWith("your quote:"))
                {
                    Reporting.Log("Wrong heading text for quote summary page of Pet Quote process.");
                    return false;
                }
                GetElement(XP_PET_NAME);
                GetElement(XP_PET_AGE);
                GetElement(XP_PET_BREED);
                GetElement(XP_PET_SUBURB);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Pet Quote - Quote summary page");
            return true;
        }

        /// <summary>
        /// Verify that the policyholder's details are correctly displayed in the summary page.
        /// </summary>
        /// <param name="policyholder"></param>
        /// <returns></returns>
        public void VerifyPolicyholderDetails(Contact policyholder)
        {
            Reporting.IsTrue(policyholder.EqualsFullName(GetInnerText(XP_PH_NAME)), $"Verifying {policyholder.GetFullTitleAndName()} matches policy holder name on confirmation screen; {GetInnerText(XP_PH_NAME)}");

            Reporting.IsTrue(policyholder.MailingAddress.IsEqualIgnorePostcode(Address.ParseString(GetInnerText(XP_ADDRESS))),
                                    $"Mailing address of policy holder ({policyholder.MailingAddress.QASStreetAddress()}) shown in summary; {GetInnerText(XP_ADDRESS)}");

            Reporting.AreEqual(policyholder.GetPhone(), GetInnerText(XP_PHONE));

            Reporting.AreEqual(policyholder.GetEmail(), GetInnerText(XP_EMAIL));
        }

        public void ClickContinue()
        {
            ClickControl(XP_CONTINUE_BTN);
        }
    }
}
