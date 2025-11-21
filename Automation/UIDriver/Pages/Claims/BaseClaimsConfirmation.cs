using Rac.TestAutomation.Common;
using System;
using System.Threading;
using OpenQA.Selenium;

namespace UIDriver.Pages.Claims
{
    public class BaseClaimsConfirmation : BasePage
    {
        #region XPATHS
        protected const string XP_HEADING               = "//span[@class='action-heading']";
        protected const string XP_CLAIM_NUMBER          = "id('quote-number')";

        // Left panel:
        protected const string XP_CLAIMANT_FIRST_NAME   = "id('marketing-first-name')";
        protected const string XP_CLAIMANT_CLAIM_NUMBER = "id('marketing-claim-number')";
        protected const string XP_RAC_CLAIM_INFO        = "id('marketing-claim-response')";
        protected const string XP_RAC_CLAIM_FAQ_TEXT    = "//div[@class='confirmationMessage']/p[contains(.,'FAQ')]";
        protected const string XP_MY_INSURANCE_BUTTON   = "id('HappensNextMyInsuranceButton')";
        

        // Right panel - claim summary:
        protected const string XP_POLICY_NUMBER = "id('Details_PolicyNumber_Answer')";
        protected const string XP_EVENT_DATE    = "id('Details_EventDate_Answer')";
        protected const string XP_EVENT_TIME    = "id('Details_EventTime_Answer')";
        protected const string XP_DAMAGE_TYPE   = "id('Details_DamageTypeDescription_Answer')";
        protected const string XP_PH_NAME_FIRST = "id('Details_FirstName_Answer')";
        protected const string XP_PH_NAME_LAST  = "id('Details_Surname_Answer')";
        protected const string XP_PH_DOB        = "id('Details_DateOfBirth_Answer')";
        #endregion

        #region Settable properties and controls
        public string ClaimNumber => GetElement(XP_CLAIM_NUMBER).Text;

        /// <summary>
        /// Left panel, conditional paragraph text that
        /// provides additional information to the claimant.
        /// </summary>
        public string GeneralClaimInformationParagraph => GetInnerText(XP_RAC_CLAIM_INFO).StripLineFeedAndCarriageReturns();

        /// <summary>
        /// Left panel, paragraph text that provides
        /// guidance on where to find claims FAQs and
        /// claim current status.
        /// </summary>
        public string ClaimFAQTextParagraph => GetInnerText(XP_RAC_CLAIM_FAQ_TEXT).StripLineFeedAndCarriageReturns(); 

        public string DetailsPolicyNumber => GetInnerText(XP_POLICY_NUMBER);

        // Date is always d MMMM yyyy format in this view, which is parsable by DateTime.
        public DateTime DetailsEventDate => DateTime.Parse(GetInnerText(XP_EVENT_DATE));

        // Time is always "H:mm tt" format in this view, which is parsable by DateTime.
        public DateTime DetailsEventTime => DateTime.Parse(GetInnerText(XP_EVENT_TIME));

        public string DetailsPHFirstName => GetInnerText(XP_PH_NAME_FIRST);

        public string DetailsPHLastName => GetInnerText(XP_PH_NAME_LAST);

        public DateTime DetailsPHDateOfBirth => DateTime.Parse(GetInnerText(XP_PH_DOB));

        #endregion

        public BaseClaimsConfirmation(Browser browser) : base(browser) { }

        override public bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XP_HEADING);
                GetElement(XP_CLAIM_NUMBER);
                GetElement(XP_CLAIMANT_FIRST_NAME);

                Reporting.LogPageChange($"Claims confirmation page for claim: {ClaimNumber}");
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }

        public void ClickMyInsuranceButton()
        {
            ClickControl(XP_MY_INSURANCE_BUTTON);
        }
    }
}
