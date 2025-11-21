using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;

using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace UIDriver.Pages.B2C
{
    public class HomeQuote3Summary : BasePage
    {
        #region XPATHS
        // XP key references:
        private const string BASE       = "/html/body/div[@id='wrapper']";
        private const string XP_HEADING = BASE + "//span[@class='action-heading']/span";

        // Insurance items:
        private const string XP_START_DATE       = "id('HomeSummary_PolicyStartDate_Answer')";
        private const string XP_PROPERTY_ADDRESS = "id('HomeSummary_BuildingLocation_Answer')";
        private const string XP_COVER            = "id('HomeSummary_CoverTypeDescription_Answer')";

        // Sum Insured
        private const string XP_SUM_INSURED_BUILDING          = "id('HomeSummary_BuildingSumInsured_Answer')";
        private const string XP_SUM_INSURED_CONTENTS          = "id('HomeSummary_ContentsSumInsured_Answer')";
        private const string XP_SUM_INSURED_ACCIDENTAL_DAMAGE = "id('HomeSummary_AccidentalDamageCover_Answer')";
        private const string XP_ACCIDENTAL_DAMAGE_LBL         = "//label[@for='HomeSummary_AccidentalDamageCover']";

        // Excess
        private const string XP_EXCESS_BUILDING              = "id('HomeSummary_BuildingExcess_Answer')";
        private const string XP_EXCESS_CONTENTS              = "id('HomeSummary_ContentsExcess_Answer')";
        private const string XP_EXCESS_ACCIDENTAL_DAMAGE     = "id('HomeSummary_AccidentalDamageExcess_Answer')";
        private const string XP_ACCIDENTAL_DAMAGE_EXCESS_LBL = "//label[@for='HomeSummary_AccidentalDamageExcess']";

        // Driver X items:
        private const string XP_PH_NAME = "//div[@id='HomeSummary_Contacts_0__Name_Answer']";
        private const string XP_ADDRESS = "//div[@id='HomeSummary_Contacts_0__MailingAddress_Answer']";
        private const string XP_PHONE   = "//div[@id='HomeSummary_Contacts_0__MobilePhoneNumber_Answer']";
        private const string XP_EMAIL   = "//div[@id='HomeSummary_Contacts_0__EmailAddress_Answer']";

        private const string XP_CONTINUE_BTN = "id('accordion_0_submit-action')";

        // Quote premium values header
        private const string XP_ANNUAL_PRICE = "//span[@class='price']";
        private const string XP_MONTHLY_PRICE = "//span[@class='monthly-price small']";
        #endregion

        #region Settable properties and controls
        public string StartDate => GetInnerText(XP_START_DATE);

        public string InsuredPropertyAddress => GetInnerText(XP_PROPERTY_ADDRESS);

        public string Cover => GetInnerText(XP_COVER);

        public int SIBuilding => DataHelper.ConvertMonetaryStringToInt(GetInnerText(XP_SUM_INSURED_BUILDING));

        public int SIContents => DataHelper.ConvertMonetaryStringToInt(GetInnerText(XP_SUM_INSURED_CONTENTS));

        public string ExcessBuilding => DataHelper.StripMoneyNotations(GetInnerText(XP_EXCESS_BUILDING));

        public string ExcessContents => DataHelper.StripMoneyNotations(GetInnerText(XP_EXCESS_CONTENTS));

        public decimal HeaderQuotePriceAnnual => decimal.Parse(GetInnerText(XP_ANNUAL_PRICE));

        public decimal HeaderQuotePriceMonthly => decimal.Parse(GetInnerText(XP_MONTHLY_PRICE));

        public string SIAccidentalDamage => GetInnerText(XP_SUM_INSURED_ACCIDENTAL_DAMAGE);

        public string ExcessAccidentalDamage => DataHelper.StripMoneyNotations(GetInnerText(XP_EXCESS_ACCIDENTAL_DAMAGE));

        public string AccidentalDamageLabelText => GetInnerText(XP_ACCIDENTAL_DAMAGE_LBL);

        public bool IsAccidentalDamageLabelDisplayed => IsControlDisplayed(XP_ACCIDENTAL_DAMAGE_LBL);

        public bool IsAccidentalDamageSumInsuredDisplayed => IsControlDisplayed(XP_SUM_INSURED_ACCIDENTAL_DAMAGE);

        public string AccidentalDamageExcessLabelText => GetInnerText(XP_ACCIDENTAL_DAMAGE_EXCESS_LBL);

        public bool IsAccidentalDamageExcessLabelDisplayed => IsControlDisplayed(XP_ACCIDENTAL_DAMAGE_EXCESS_LBL);

        public bool IsAccidentalDamageExcessDisplayed => IsControlDisplayed(XP_EXCESS_ACCIDENTAL_DAMAGE);
        #endregion

        public HomeQuote3Summary(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_HEADING);
                if (!heading.Text.ToLower().StartsWith("your quote:"))
                {
                    Reporting.Log("Wrong heading text for quote summary page of Home Quote process.");
                    return false;
                }
                GetElement(XP_START_DATE);
                GetElement(XP_PROPERTY_ADDRESS);
                GetElement(XP_COVER);
                GetElement(XP_PH_NAME);
                GetElement(XP_ADDRESS);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Home Quote - Quote summary page");
            return true;
        }

        /// <summary>
        /// Verify that the policyholder's details are correctly displayed in the summary page.
        /// </summary>
        /// <param name="policyholder"></param>
        /// <returns></returns>
        public void VerifyPolicyholderDetails(Contact policyholder)
        {
            Reporting.IsTrue(policyholder.EqualsFullName(GetInnerText(XP_PH_NAME)), $"Verifying {policyholder.GetFullTitleAndName()} matches policy holder name on confirmation screen.");

            // TODO: Add address validation here. (Not present in Tosca tests).

            Reporting.AreEqual(policyholder.GetPhone(), GetInnerText(XP_PHONE));

            Reporting.AreEqual(policyholder.GetEmail(), GetInnerText(XP_EMAIL));
        }

        /// <summary>
        /// Verify that the Sum insured and Excess details are correctly displayed in the policy summary page.
        /// </summary>
        public void VerifySumInsuredAndExcessDetails(QuoteHome quoteDetails)
        {
            Reporting.Log($"Sum insured and Excess sections", _browser.Driver.TakeSnapshot());

            if (quoteDetails.BuildingValue.HasValue)
            {
                Reporting.AreEqual(quoteDetails.BuildingValue.Value, SIBuilding, "building sum insured value");
                Reporting.AreEqual(quoteDetails.ExcessBuilding, ExcessBuilding, "building excess");
            }

            if (quoteDetails.ContentsValue.HasValue)
            {
                Reporting.AreEqual(quoteDetails.ContentsValue.Value, SIContents, "contents sum insured value");
                Reporting.AreEqual(quoteDetails.ExcessContents, ExcessContents, "contents excess");
            }

            if (quoteDetails.IsEligibleForAccidentalDamage && quoteDetails.AddAccidentalDamage)
            {
                Reporting.IsTrue(IsAccidentalDamageLabelDisplayed, "Accidental damage label".IsDisplayed());
                Reporting.AreEqual("Accidental Damage:", AccidentalDamageLabelText, "Accidental Damage:".IsExpectedLabelText());
                Reporting.IsTrue(IsAccidentalDamageSumInsuredDisplayed, "Accidental damage sum insured".IsDisplayed());
                Reporting.AreEqual(HOME_ACCIDENTAL_DAMAGE_SI_CONTENTS_COVERED, SIAccidentalDamage, "accidental damage contents covered");

                Reporting.IsTrue(IsAccidentalDamageExcessLabelDisplayed, "Accidental damage excess label".IsDisplayed());
                Reporting.AreEqual("Accidental Damage:", AccidentalDamageExcessLabelText, "Accidental Damage:".IsExpectedLabelText());
                Reporting.IsTrue(IsAccidentalDamageExcessDisplayed, "Accidental damage excess".IsDisplayed());
                Reporting.AreEqual(HOME_ACCIDENTAL_DAMAGE_EXCESS, ExcessAccidentalDamage, "accidental damage excess");
            }
            else 
            {
                Reporting.IsFalse(IsAccidentalDamageLabelDisplayed, "Accidental damage label".IsNotDisplayed());
                Reporting.IsFalse(IsAccidentalDamageSumInsuredDisplayed, "Accidental damage sum insured".IsNotDisplayed());

                Reporting.IsFalse(IsAccidentalDamageExcessLabelDisplayed, "Accidental damage excess label".IsNotDisplayed());
                Reporting.IsFalse(IsAccidentalDamageExcessDisplayed, "Accidental damage excess".IsNotDisplayed());
            }
        }

        public void ClickContinue()
        {
            ClickControl(XP_CONTINUE_BTN);
        }
    }
}
