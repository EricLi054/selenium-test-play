using Rac.TestAutomation.Common;
using System;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Claims
{
    public class ClaimHomePage3OtherParties : BasePage
    {
        #region XPATHS
        private const string XP_HEADING = "//span[@class='action-heading']";
        private const string XP_CLAIM_NUMBER = "id('quote-number')";

        private const string XPR_YES = "//span[text()='Yes']/..";
        private const string XPR_NO = "//span[text()='No']/..";
        private const string XPR_UNKNOWN = "//span[text()='Unknown']/..";

        // Witness Count:
        private const string XP_WERE_THERE_WITNESSES_YN = "id('WitnessDetails_WereThereAnyWitnesses')";
        private const string XP_WITNESS_COUNT = "//span[@aria-owns='WitnessDetails_NumberOfWitnesses_listbox']";
        private const string XP_WITNESS_COUNT_OPTIONS = "id('WitnessDetails_NumberOfWitnesses_listbox')/li";
        private const string XP_WITNESS_COUNT_CONTINUE_BTN = "id('accordion_0')";

        // Witness details #1:
        private const string XP_WITNESS_1_HEADER = "id('IntegratedClaimWitnessDetails_Witness1Header_Wrapper')";
        private const string XP_WITNESS_1_TITLE = "//span[@aria-owns='WitnessDetails_FirstWitnessTitle_listbox']";
        private const string XP_WITNESS_1_TITLE_OPTIONS = "id('WitnessDetails_FirstWitnessTitle_listbox')/li";
        private const string XP_WITNESS_1_NAME_FIRST = "id('WitnessDetails_FirstWitnessFirstName')";
        private const string XP_WITNESS_1_NAME_LAST = "id('WitnessDetails_FirstWitnessLastName')";
        private const string XP_WITNESS_1_PHONENUMBER = "id('WitnessDetails_FirstWitnessTelephoneNumber_PhoneNumber')";
        private const string XP_WITNESS_1_OTHER_INFO = "id('WitnessDetails_FirstWitnessOtherInformation')";
        private const string XP_WITNESS_1_CONTINUE_BTN = "id('accordion_1')";

        // Third Party details:
        private const string XP_THIRDPARTY_COUNT = "//span[@aria-owns='WitnessDetails_NumberOfOtherVehiclesOrParties_listbox']";
        private const string XP_THIRDPARTY_COUNT_OPTIONS = "id('WitnessDetails_NumberOfOtherVehiclesOrParties_listbox')/li";
        private const string XP_DID_KNOW_THIRDPARTY_YN = "id('WitnessDetails_DidYouKnowThirdPartyPriorToAccident')";
        private const string XP_THIRDPARTY_TITLE = "//span[@aria-owns='WitnessDetails_ThirdPartyTitle_listbox']";
        private const string XP_THIRDPARTY_TITLE_OPTIONS = "id('WitnessDetails_ThirdPartyTitle_listbox')/li";
        private const string XP_THIRDPARTY_NAME_FIRST = "id('WitnessDetails_ThirdPartyFirstName')";
        private const string XP_THIRDPARTY_NAME_LAST = "id('WitnessDetails_ThirdPartyLastName')";
        private const string XP_THIRDPARTY_PHONENUMBER = "id('WitnessDetails_ThirdPartyTelephoneNumber_PhoneNumber')";

        // Offender (Theft) details
        private const string XP_OFFENDER_COUNT = "//span[@aria-owns='WitnessDetails_NumberOfOtherPartiesOrOffenders_listbox']";
        private const string XP_OFFENDER_COUNT_OPTIONS = "id('WitnessDetails_NumberOfOtherPartiesOrOffenders_listbox')/li";
        private const string XP_OFFENDER_DETAILS_YN = "id('WitnessDetails_OffenderKnown')";
        private const string XP_OFFENDER_RELATIONSHIP = "id('WitnessDetails_OffenderRelationship')";

        // TODO QAS TP address
        private const string XP_THIRDPARTY_CONTINUE_BTN = "id('accordion_3')";

        // Upload Documents
        // Used partial ID here as id = accordion_4_submit-action if TP is an option,
        // but id = accordion_3_submit-action  when there is no option for TP.
        private const string XP_SUBMIT_CLAIM_BTN = "//button[contains(@id,'submit-action')]";
        #endregion

        #region Settable properties and controls
        private int WitnessCount
        {
            get => throw new NotImplementedException();
            set
            {
                string choiceText = "";
                switch (value)
                {
                    case 0:
                        Reporting.Error("Invalid value. 0 witnesses should not see this dropdown.");
                        break;
                    case 1:
                    case 2:
                        choiceText = value.ToString();
                        break;
                    default:
                        choiceText = "More than 2";
                        break;
                }
                _driver.WaitForElementToBeVisible(By.XPath(XP_WITNESS_COUNT), WaitTimes.T5SEC);
                WaitForSelectableAndPickFromDropdown(XP_WITNESS_COUNT, XP_WITNESS_COUNT_OPTIONS, choiceText);
            }
        }

        private int ThirdPartyCount
        {
            get => throw new NotImplementedException("Not yet implemented.");
            set
            {
                _driver.WaitForElementToBeVisible(By.XPath(XP_THIRDPARTY_COUNT), WaitTimes.T5SEC);
                string choiceText = "";
                switch (value)
                {
                    case 0:
                        // Applicable for Theft cases.
                        choiceText = "Unknown";
                        break;
                    case 1:
                        choiceText = value.ToString();
                        break;
                    default:
                        choiceText = "More than 1";
                        break;

                }
                WaitForSelectableAndPickFromDropdown(XP_THIRDPARTY_COUNT, XP_THIRDPARTY_COUNT_OPTIONS, choiceText);
            }
        }

        private int OffenderCount
        {
            get =>throw new NotImplementedException("Not yet implemented.");
            set
            {
                _driver.WaitForElementToBeVisible(By.XPath(XP_OFFENDER_COUNT), WaitTimes.T5SEC);

                string choiceText = "";
                switch (value)
                {
                    case 0:
                        // Applicable for Theft cases.
                        choiceText = "Unknown";
                        break;
                    case 1:
                        choiceText = value.ToString();
                        break;
                    default:
                        choiceText = "More than 1";
                        break;

                }
                WaitForSelectableAndPickFromDropdown(XP_OFFENDER_COUNT, XP_OFFENDER_COUNT_OPTIONS, choiceText);
            }
        }
        #endregion

        public ClaimHomePage3OtherParties(Browser browser) : base(browser)
        {
        }

        public override bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XP_HEADING);
                GetElement(XP_CLAIM_NUMBER);
                GetElement(XP_WERE_THERE_WITNESSES_YN);
                GetElement(XP_WITNESS_COUNT_CONTINUE_BTN);

                Reporting.LogPageChange("Home claim other parties page");
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }

            return rendered;
        }

        public void EnterWitnessInformation(ClaimHome claimData)
        {
            // Define witness count.
            var witnessCount = claimData.Witness?.Count ?? 0;
            ClickBinaryToggle(XP_WERE_THERE_WITNESSES_YN, XPR_YES, XPR_NO, witnessCount > 0);

            if (witnessCount > 0)
            {
                Thread.Sleep(1000);
                _driver.WaitForElementToBeVisible(By.XPath(XP_WITNESS_COUNT), WaitTimes.T30SEC);
                WitnessCount = witnessCount;

                WaitForWitness1AccordionToRender();
            }

            ClickControl(XP_WITNESS_COUNT_CONTINUE_BTN);

            if (witnessCount > 0)
            {
                // TODO: Implement support for second witness.
                // Not done as yet as current Tosca tests don't cover 2 witnesses.
                if (witnessCount > 1) throw new NotImplementedException("Support for second witness needs to be implemented.");

                WaitForWitness1DetailsToBeVisible();

                SetWitness1Details(claimData.Witness[0]);

                ClickControl(XP_WITNESS_1_CONTINUE_BTN);
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// Should only be called when claim offer options to enter TP details.
        /// </summary>
        /// <param name="claimData"></param>
        public void EnterThirdPartyInformation(ClaimHome claimData)
        {
            SetThirdPartyDetailsTheft(claimData);

            ClickControl(XP_THIRDPARTY_CONTINUE_BTN);
            Thread.Sleep(2000);  // Sleep to allow for accordion transition animation
        }

        public void UploadDocumentationAndSubmitClaim(ClaimHome claimData)
        {
            // TODO: Determine any handling around files to upload.

            // Submit claim.
            ClickControl(XP_SUBMIT_CLAIM_BTN);
        }

        /// <summary>
        /// Enter the contact details of first witness.
        /// 
        /// NOTE: This is currently built to just deal with cases
        /// where there is only one witness. Will need to be extended
        /// to handle when there is a second witness.
        /// </summary>
        /// <param name="contact"></param>
        private void SetWitness1Details(Contact contact)
        {
            if (contact.Title != Title.None)
                WaitForSelectableAndPickFromDropdown(XP_WITNESS_1_TITLE, XP_WITNESS_1_TITLE_OPTIONS, contact.Title.GetDescription());

            if (!string.IsNullOrEmpty(contact.FirstName))
                WaitForTextFieldAndEnterText(XP_WITNESS_1_NAME_FIRST, contact.FirstName, false);

            if (!string.IsNullOrEmpty(contact.Surname))
                WaitForTextFieldAndEnterText(XP_WITNESS_1_NAME_LAST, contact.Surname, false);

            if (!string.IsNullOrEmpty(contact.GetPhone()))
                WaitForTextFieldAndEnterText(XP_WITNESS_1_PHONENUMBER, contact.GetPhone(), false);

        }

        private void SetThirdPartyDetailsTheft(ClaimHome claimData)
        {
            var thirdPartyCount = claimData.ThirdParty == null ? 0 : claimData.ThirdParty.Count;
            OffenderCount = thirdPartyCount;

            if (thirdPartyCount == 0)
                return;

            // We only take details of one offender, even if more were present.
            SetThirdPartyDetailsCommon(claimData.ThirdParty[0]);
        }

        private void SetThirdPartyDetailsCommon(Contact contact)
        {
            WaitForThirdPartyDetailsToBeVisible();

            if (contact.Title != Title.None)
                WaitForSelectableAndPickFromDropdown(XP_THIRDPARTY_TITLE, XP_THIRDPARTY_TITLE_OPTIONS, contact.Title.GetDescription());

            if (!string.IsNullOrEmpty(contact.FirstName))
                WaitForTextFieldAndEnterText(XP_THIRDPARTY_NAME_FIRST, contact.FirstName, false);

            if (!string.IsNullOrEmpty(contact.Surname))
                WaitForTextFieldAndEnterText(XP_THIRDPARTY_NAME_LAST, contact.Surname, false);

            if (!string.IsNullOrEmpty(contact.GetPhone()))
                WaitForTextFieldAndEnterText(XP_THIRDPARTY_PHONENUMBER, contact.GetPhone(), false);
        }

        /// <summary>
        /// This accordion is hidden. If the user selects that there are
        /// witnesses, then automation must wait for this accordion to
        /// change state to render, otherwise it can end up skipping
        /// the section altogether.
        /// </summary>
        private void WaitForWitness1AccordionToRender()
        {
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T10SEC);
            do
            {
                Thread.Sleep(500);
                if (GetElement(XP_WITNESS_1_HEADER).GetAttribute("class").Equals("showQuestion"))
                {
                    break;
                }
            } while (DateTime.Now < endTime);
        }

        /// <summary>
        /// Witness details UI are hidden within an accordion that
        /// has been observed to occassionally take a while to render
        /// once the user selects to open it.
        /// </summary>
        private void WaitForWitness1DetailsToBeVisible()
        {
            _driver.WaitForElementToBeVisible(By.XPath(XP_WITNESS_1_TITLE), WaitTimes.T30SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_WITNESS_1_NAME_FIRST), WaitTimes.T5SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_WITNESS_1_NAME_LAST), WaitTimes.T5SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_WITNESS_1_PHONENUMBER), WaitTimes.T5SEC);

            // Despite the above wait controls. It has been observed that occassionally
            // the UI libraries are still applying styles and layout, which breaks attempts
            // to drive UI controls.
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Third Party details UI controls are hidden by default
        /// and transition to be visible once user defines whether
        /// any TP were involved. The javascript for this can run
        /// slower than Selenium, so we need to check they're
        /// visible before attempting to drive them.
        /// </summary>
        private void WaitForThirdPartyDetailsToBeVisible()
        {
            _driver.WaitForElementToBeVisible(By.XPath(XP_THIRDPARTY_TITLE), WaitTimes.T30SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_THIRDPARTY_NAME_FIRST), WaitTimes.T5SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_THIRDPARTY_NAME_LAST), WaitTimes.T5SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_THIRDPARTY_PHONENUMBER), WaitTimes.T5SEC);
        }
    }
}
