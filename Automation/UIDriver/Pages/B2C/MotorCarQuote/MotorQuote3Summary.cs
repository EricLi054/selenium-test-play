using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace UIDriver.Pages.B2C
{
    public class MotorQuote3Summary : BasePage
    {
        #region XPATHS
        // XP key references:
        private const string BASE    = "/html/body/div[@id='wrapper']";
        private const string PANEL   = BASE + "//form//div[contains(@class,'accordion-panel')]";
        private const string BLOCK_X = PANEL + "//div[@class='summary-block']";
        private const string I_VEH   = "[1]";
        private const string XP_HEADING = BASE + "//span[@class='action-heading']/span";

        // Insurance items:
        private const string XPR_VEHICLE    = "//div[@class='vehicle-description']";
        private const string XPR_COVER_TYPE = "//div[@id='Summary_ProductType_Answer']";
        private const string XPR_AGREED_VAL = "//div[@id='Summary_AgreedValue_Answer']";
        private const string XPR_EXCESS_VAL = "//div[@id='Summary_BasicExcess_Answer']";
        private const string XPR_HIRE_CAR   = "//div[@id='Summary_HireCar_Answer']";
        private const string XP_NCB_PROTECTION_LABEL = "//label[@for='Summary_ProtectNoClaimBonus']";
        private const string XPR_NCB_PROTECTION_ANSWER   = "//div[@id='Summary_ProtectNoClaimBonus_Answer']";
        private const string XPR_START_DATE = "//div[@id='Summary_PolicyStartDate_Answer']";

        // Driver X items:
        private const string XPR_DRIVER_NAME = "//div[@id='Summary_Contacts_{0}__Name_Answer']";
        private const string XPR_IS_HOLDER   = "//div[@id='Summary_Contacts_{0}__IsPolicyHolder_Answer']";
        private const string XPR_ADDRESS     = "//div[@id='Summary_Contacts_{0}__MailingAddress_Answer']";
        private const string XPR_PHONE_MOB   = "//div[@id='Summary_Contacts_{0}__MobilePhoneNumber_Answer']";
        private const string XPR_PHONE_HOME  = "//div[@id='Summary_Contacts_{0}__HomePhoneNumber_Answer']";
        private const string XPR_EMAIL       = "//div[@id='Summary_Contacts_{0}__EmailAddress_Answer']";

        private const string XP_CONTINUE_BTN = PANEL + "//button[@id='accordion_0_submit-action']";
        #endregion

        #region Settable properties and controls
        public string InsuredVehicle
        {
            get
            {
                var element = GetElement(BLOCK_X + I_VEH + XPR_VEHICLE);
                return element.Text;
            }
        }

        public MotorCovers CoverType => MotorCoverNameMappings.First(
                                            x => x.Value.TextB2C == GetInnerText(BLOCK_X + I_VEH + XPR_COVER_TYPE)).Key;


        public string ValueAgreed
        {
            get
            {
                var element = GetElement(BLOCK_X + I_VEH + XPR_AGREED_VAL);
                return element.Text;
            }
        }

        public string ValueExcess
        {
            get
            {
                var element = GetElement(BLOCK_X + I_VEH + XPR_EXCESS_VAL);
                return element.Text;
            }
        }

        /// <summary>
        /// Fetch row indicating the "Hire Car After Accident" cover for the Full Cover policy.
        /// </summary>
        public string HireCarStatus
        {
            get
            {
                var element = GetElement(BLOCK_X + I_VEH + XPR_HIRE_CAR);
                return element.Text;
            }
        }

        /// <summary>
        /// Fetch row indicating the "No Claim Bonus Protection" cover for the Full Cover policy.
        /// </summary>
        public string NoClaimBonusProtectionStatus
        {
            get
            {
                var element = GetElement(BLOCK_X + I_VEH + XPR_NCB_PROTECTION_ANSWER);
                return element.Text;
            }
        }

        public string StartDate
        {
            get
            {
                var element = GetElement(BLOCK_X + I_VEH + XPR_START_DATE);
                return element.Text;
            }
        }
        #endregion

        public MotorQuote3Summary(Browser browser) : base(browser) { }

        override public bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_HEADING);
                if (!heading.Text.ToLower().StartsWith("your quote:"))
                {
                    Reporting.Log("Wrong heading text for second page of Motor Quote process.");
                    return false;
                }
                GetElement(BLOCK_X + I_VEH + XPR_VEHICLE);
                GetElement(BLOCK_X + I_VEH + XPR_START_DATE);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Motor Quote - Quote summary page");
            return true;
        }

        /// <summary>
        /// This method supports exceptions for PrePopulated Quotes as well as Motor Risk Address, as both
        /// those cases complicate the automation tests verifying the address(es) shown.
        /// 
        /// For a PPQ or risk address case, the main driver mailing address is not verified.
        /// Shield DB verifications will still occur in other methods.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="drivers"></param>
        /// <param name="mainDriverAddress"></param>
        /// <param name="isPPQ">Because PPQ mangles the address on this page, we'll skip verification of it.</param>
        /// <returns></returns>
        public bool VerifyDriverDetails(int index, List<Driver> drivers, Address mainDriverAddress, bool isPPQ = false)
        {
            var result = true;
            var driver = drivers.FirstOrDefault(x => x.Details.GetFullTitleAndName().SanitizeString().Equals(GetInnerText(string.Format(BLOCK_X + XPR_DRIVER_NAME, index)), StringComparison.InvariantCultureIgnoreCase));

            if (driver == null)
            { Reporting.Error($"Displayed driver ({GetInnerText(string.Format(BLOCK_X + XPR_DRIVER_NAME, index))}) could not be found in test case data."); }

            /****************************************************************************
             * Create sanitised assertions for Name Fields for the Policy Summary Details
                 * As per B2C-2707
                 * Before checking driver details input with — & ’ must set assertion 
                   to expect - & ' in their place.
                 * See SanitizeString in DataHelper.cs
            *****************************************************************************/
            var field = GetElement(string.Format(BLOCK_X + XPR_DRIVER_NAME, index));
            Reporting.Log("Updating expectations for em-dash and apostrophes in contact Full Title and Names for Policy Summary screen.");
            string sanitizedName = driver.Details.GetFullTitleAndName().SanitizeString();
            if (!string.Equals(field.Text, sanitizedName, StringComparison.InvariantCultureIgnoreCase))
            {
                Reporting.Log($"Driver ({index}) name ({field.Text}) in policy summary was not as expected ({driver.Details.GetFullTitleAndName()})");
                return false;
            }

            field = GetElement(string.Format(BLOCK_X + XPR_IS_HOLDER, index));
            if (string.IsNullOrEmpty(field.Text))
            {
                Reporting.Log($"Driver ({index}) had no value for policy holder status");
                return false;
            }
            if ((driver.IsPolicyHolderDriver && field.Text.ToLower() != "yes") ||
                (!driver.IsPolicyHolderDriver && field.Text.ToLower() != "no"))
            {
                Reporting.Log($"Driver ({index}) had mismatched value ({field.Text}) for policy holder status");
                return false;
            }

            if (!isPPQ && !Config.Get().IsMotorRiskAddressEnabled())
            {
                var expectedAddress = driver.Details.MailingAddress == null ? mainDriverAddress : driver.Details.MailingAddress;
                field = GetElement(string.Format(BLOCK_X + XPR_ADDRESS, index));
                var actualAddress = Address.ParseString(field.Text);

                if (!expectedAddress.IsEqualIgnorePostcode(actualAddress))
                {
                    Reporting.Log($"Driver ({index}) address ({field.Text}) in policy summary was not as expected ({expectedAddress.StreetSuburbState()})");
                    return false;
                }
            }

            // TODO: Resolve issue around Benang providing a different email
            if (driver.IsPolicyHolderDriver && !isPPQ)
            {
                field = GetElement(string.Format(BLOCK_X + XPR_EMAIL, index));
                if (!field.Text.Equals(driver.Details.GetEmail(), StringComparison.InvariantCultureIgnoreCase))
                {
                    Reporting.Log($"Driver ({index}) email ({field.Text}) in policy summary was not as expected ({driver.Details.GetEmail()})");
                    return false;
                }
            }

            var xpr_element = driver.Details.GetPhone().StartsWith("04") ? XPR_PHONE_MOB : XPR_PHONE_HOME;
            field = GetElement(string.Format(BLOCK_X + xpr_element, index));
            if (field.Text != driver.Details.GetPhone())
            {
                Reporting.Log($"Driver ({index}) Phone ({field.Text}) in policy summary was not as expected ({driver.Details.GetPhone()})");
                return false;
            }

            return result;
        }

        /// <summary>
        /// Verifies that the NCB protection label and answer are not shown any longer
        /// it is no longer supported by RAC. Removed from insurance policies from
        /// August 2024.
        /// </summary>
        public void VerifyNCBProtectionIsNotDisplayed()
        {
            Reporting.IsFalse(_driver.TryFindElement(By.XPath(XP_NCB_PROTECTION_LABEL), out IWebElement noClaimBonusProtectionCheckbox), 
                "No Claim Bonus Protection label should not be shown for Comprehensive cover type as it is no longer supported");
            Reporting.IsFalse(_driver.TryFindElement(By.XPath(XPR_NCB_PROTECTION_ANSWER), out IWebElement noClaimBonusProtectionAnswer), 
                "No Claim Bonus Protection answer should not be shown for Comprehensive cover type as it is no longer supported");
        }

        public void ClickContinue()
        {
            ClickControl(XP_CONTINUE_BTN);
        }
    }
}
