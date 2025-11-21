using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UIDriver.Helpers;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;

namespace UIDriver.Pages.Spark.MotorcycleQuote
{
    public class HeresYourQuote : SparkBasePage
    {
        #region XPATHS
        private class XPath
        {
            public static class General
            {
                public const string Header = "//h2[@id='header' and text()='Here are your quotes']";
                public const string QuoteNumber = "//*[@id='quoteNumber']";
                public static class QuoteDiologue
                {
                    public const string Close = "//button[@aria-label='close']";
                }
            }
            public static class Policies
            {
                public static class Comprehensive
                {
                    public static class Help
                    {
                        public const string DialogueTitle = "id('Comprehensive-cover-dialogue')";
                        public const string DialogueRibbon = DialogueTitle + "/following-sibling::div[1]/div[1]/h4";
                    }
                    public static class PaymentFrequency
                    {
                        public const string Annual = "id('comprehensive-payment-frequency-toggle-payment-frequency-annual')";
                        public const string Monthly = "id('comprehensive-payment-frequency-toggle-payment-frequency-monthly')";
                    }
                    public const string Price = "id('comprehensive-label-payment-amount')";
                    public static class Breakdown
                    {
                        public const string Link = "id('comprehensive-premium-breakdown')";
                        public const string Basic = "id('comprehensive-premium-breakdown-basic')";
                        public const string Stamp = "id('comprehensive-premium-breakdown-government-charges')";
                        public const string GST = "id('comprehensive-premium-breakdown-gst')";
                    }
                    public static class Excess
                    {
                        public const string Dropdown = "id('excess-COMP')";
                        public const string Options = "id('menu-excess-COMP')//ul/li";
                    }
                    public static class Input
                    {
                        public const string SumInsured = "id('sumInsured-COMP')";
                    }
                    public static class Button
                    {
                        public const string ChooseThisCover = "id('choose-this-cover-COMP-button')";
                    }

                }
                public static class ThirdParty_FireAndTheft
                {
                    public static class Help
                    {
                        public const string DialogueTitle = "//h2[@id='Third party fire and theft-cover-dialogue']";
                    }
                    public static class PaymentFrequency
                    {
                        public const string Annual = "id('thirdpartyfireandtheft-payment-frequency-toggle-payment-frequency-annual')";
                        public const string Monthly = "id('thirdpartyfireandtheft-payment-frequency-toggle-payment-frequency-monthly')";
                    }
                    public const string Price = "id('thirdpartyfireandtheft-label-payment-amount')";
                    public static class Breakdown
                    {
                        public const string Link = "id('thirdpartyfireandtheft-premium-breakdown')";
                        public const string Basic = "id('thirdpartyfireandtheft-premium-breakdown-basic')";
                        public const string Stamp = "id('thirdpartyfireandtheft-premium-breakdown-government-charges')";
                        public const string GST = "id('thirdpartyfireandtheft-premium-breakdown-gst')";
                    }
                    public static class Excess
                    {
                        public const string Dropdown = "id('excess-TPFT')";
                        public const string Options = "id('menu-excess-TPFT')//ul/li";
                    }
                    public static class Input
                    {
                        public const string SumInsured = "id('sumInsured-TPFT')";
                    }
                    public static class Button
                    {
                        public const string ChooseThisCover = "id('choose-this-cover-TPFT-button')";
                    }
                }
                public static class ThirdParty_PropertyDamage
                {
                    public static class Help
                    {
                        public const string DialogueTitle = "//h2[@id='Third party property damage-cover-dialogue']";
                    }
                    public static class PaymentFrequency
                    {
                        public const string Annual = "id('thirdpartypropertydamage-payment-frequency-toggle-payment-frequency-annual')";
                        public const string Monthly = "id('thirdpartypropertydamage-payment-frequency-toggle-payment-frequency-monthly')";
                    }
                    public const string Price = "id('thirdpartypropertydamage-label-payment-amount')";
                    public static class Breakdown
                    {
                        public const string Link = "id('thirdpartypropertydamage-premium-breakdown')";
                        public const string Basic = "id('thirdpartypropertydamage-premium-breakdown-basic')";
                        public const string Stamp = "id('thirdpartypropertydamage-premium-breakdown-government-charges')";
                        public const string GST = "id('thirdpartypropertydamage-premium-breakdown-gst')";
                    }
                    public static class Excess
                    {
                        public const string Dropdown = "id('excess-TPO')";
                        public const string Options = "id('menu-excess-TPO')//ul/li";
                    }
                    public static class Button
                    {
                        public const string ChooseThisCover = "id('choose-this-cover-TPO-button')";
                    }
                }
            }
        }
        #endregion

        private const string COMP         = "COMP";
        private const string TPFT         = "TPFT";
        private const string TPO          = "TPO";
        public const string MOST_POPULAR  = "Most Popular";
        public const string MEMBER_DISCOUNT = "member discount";

        #region Settable properties and controls
        public string QuoteNumber => GetElement(XPath.General.QuoteNumber).Text;

        #endregion

        public HeresYourQuote(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.Header);
                GetElement(XPath.Policies.Comprehensive.Button.ChooseThisCover);
                GetElement(XPath.Policies.Comprehensive.Price);
                GetElement(XPath.Policies.Comprehensive.Input.SumInsured);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Motorcycle Quote page - Heres Your Quote");
            return true;
        }

        /// <summary>
        /// Fills in the desired quote parameters for the "Here's your quote" form.
        /// NOTE: This method makes a major assumption that the test will only call
        /// this once in each scenario, as it assumes that the prefill in the
        /// sum insured is the default value and hence the market value. It will
        /// then use the "InsuredVariance" from the quoteDetails and that market
        /// value to determine what to set in sum insured.
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <param name="sumInsuredValue"></param>
        public void UpdateDesiredExcessValues(QuoteMotorcycle quoteDetails)
        {
            SetExcess(quoteDetails.CoverType, quoteDetails.Excess);
        }

        public void SetExcess(MotorCovers desiredCover, string excess)
        {
            if (string.IsNullOrEmpty(excess))
            {
                // this is the test data mechanism for just use default.
                return;
            }
            string excessPrimaryCtrl;
            string excessDropDown;
            GetXpathForExcess(desiredCover, out excessPrimaryCtrl, out excessDropDown);

            // Setup excess value to correct value (as we use exacty match for option)
            // Needs to have prefixed $ sign, and comma separator for thousands
            if (!excess.StartsWith("$"))
                excess = $"${excess}";

            if (excess.Length > 4 &&
                excess.IndexOf(',') < 0)
            {
                excess = excess.Insert(excess.Length - 3, ",");
            }

            WaitForSelectableAndPickFromDropdown(excessPrimaryCtrl, excessDropDown, excess);
        }

        /// <summary>
        /// Fetches the currently displayed excess value for the requested cover.
        /// NOTE: strips the "$" and "," characters from the string.
        /// </summary>
        /// <param name="desiredCover"></param>
        /// <returns></returns>
        public string GetExcess(MotorCovers desiredCover)
        {
            string excessPrimaryCtrl;
            string excessDropDown;
            GetXpathForExcess(desiredCover, out excessPrimaryCtrl, out excessDropDown);

            var plainText = GetElement(excessPrimaryCtrl).Text;
            return plainText.StripMoneyNotations();
        }

        /// <summary>
        /// Will attempt to set the suminsured as given. Does not perform
        /// and boundary checks on the provided amount.
        /// </summary>
        /// <param name="desiredCover"></param>
        /// <param name="amount"></param>
        public void SetSumInsured(MotorCovers desiredCover, int amount)
        {
            var xpath = GetXpathForSumInsured(desiredCover);
            WaitForTextFieldAndEnterText(xpath, amount.ToString(), true);
        }

        public int GetSumInsured(MotorCovers desiredCover)
        {
            var xpath = GetXpathForSumInsured(desiredCover);
            var plainText = GetValue(xpath);
            return int.Parse(plainText.StripMoneyNotations());
        }

        public decimal GetPremiumAnnual(MotorCovers desiredCover)
        {
            ClickControl(GetButtonXpathForAnnualPremium(desiredCover));
            var plainText = GetInnerText(GetXpathForPremium(desiredCover));
            return decimal.Parse(plainText.StripMoneyNotations());
        }

        public decimal GetPremiumMonthly(MotorCovers desiredCover)
        {
            ClickControl(GetButtonXpathForMonthlyPremium(desiredCover));
            var plainText = GetInnerText(GetXpathForPremium(desiredCover));
            return decimal.Parse(plainText.StripMoneyNotations());
        }

        /// <summary>
        /// Click the "Get Policy" button for the desired cover.
        /// </summary>
        /// <param name="desiredCover"></param>
        public void ClickGetPolicy(MotorCovers desiredCover)
        {
            var desiredButtonXpath = GetXpathForGetPolicy(desiredCover);

            if (IsControlEnabled(desiredButtonXpath))
                ClickControl(desiredButtonXpath);
            else
            {
                throw new ReadOnlyException("Button is currently disabled and not clickable. Check input values.");
            }
        }

        /// <summary>
        /// Returns whether the field error validations have been triggered for the sumInsured field.
        /// </summary>
        /// <param name="marketValue"></param>
        /// <returns></returns>
        public bool IsSumInsuredValidationTriggered(MotorCovers desiredCover, int marketValue)
        {
            // Lower limit is rounded up
            int lowerLimit = (int)(Math.Ceiling(marketValue * 0.7));
            lowerLimit = lowerLimit < 500 ? 500 : lowerLimit;
            // Upper limit is round down
            int upperLimit = (int)(Math.Floor(marketValue * 1.3));
            upperLimit = upperLimit > 200000 ? 200000 : upperLimit;

            var xpath = GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.Input.SumInsured, XPath.Policies.ThirdParty_FireAndTheft.Input.SumInsured, null);
            var hasErrorHighlight = _driver.IsTextBoxHighlightedError(xpath, "Sum Insured");
            var hasErrorNotice    = _driver.IsSumInsuredErrorMessagePresent(xpath, $"Based on current market value, we will insure you for an amount between {lowerLimit.ToString("C0")} and {upperLimit.ToString("C0")}");

            if (hasErrorHighlight != hasErrorNotice)
            { Reporting.Error($"Inconsistent error state of hasErrorHighlight ({hasErrorHighlight}) and hasErrorNotice ({hasErrorNotice})."); }

            return hasErrorHighlight;
        }

        private string GetButtonXpathForAnnualPremium(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.PaymentFrequency.Annual, XPath.Policies.ThirdParty_FireAndTheft.PaymentFrequency.Annual, XPath.Policies.ThirdParty_PropertyDamage.PaymentFrequency.Annual);
        }

        private string GetButtonXpathForMonthlyPremium(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.PaymentFrequency.Monthly, XPath.Policies.ThirdParty_FireAndTheft.PaymentFrequency.Monthly, XPath.Policies.ThirdParty_PropertyDamage.PaymentFrequency.Monthly);
        }


        private string GetXpathForPremium(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.Price, XPath.Policies.ThirdParty_FireAndTheft.Price, XPath.Policies.ThirdParty_PropertyDamage.Price);
        }

        private string GetXpathForSumInsured(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.Input.SumInsured, XPath.Policies.ThirdParty_FireAndTheft.Input.SumInsured, null);
        }

        private void GetXpathForExcess(MotorCovers desiredCover, out string excessControl, out string excessDropdownList)
        {
            excessControl = GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.Excess.Dropdown, XPath.Policies.ThirdParty_FireAndTheft.Excess.Dropdown, XPath.Policies.ThirdParty_PropertyDamage.Excess.Dropdown);
            excessDropdownList = GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.Excess.Options, XPath.Policies.ThirdParty_FireAndTheft.Excess.Options, XPath.Policies.ThirdParty_PropertyDamage.Excess.Options);
        }

        private string GetXpathForGetPolicy(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.Button.ChooseThisCover, XPath.Policies.ThirdParty_FireAndTheft.Button.ChooseThisCover, XPath.Policies.ThirdParty_PropertyDamage.Button.ChooseThisCover);
        }

        private string GetXpathForSeeBreakdown(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.Breakdown.Link, XPath.Policies.ThirdParty_FireAndTheft.Breakdown.Link, XPath.Policies.ThirdParty_PropertyDamage.Breakdown.Link);
        }

        private string GetXpathForHelpDialogue(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.Help.DialogueTitle, XPath.Policies.ThirdParty_FireAndTheft.Help.DialogueTitle, XPath.Policies.ThirdParty_PropertyDamage.Help.DialogueTitle);
        }

        private string GetXpathForHelpTip(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, GetHelpTipXpath(COMP), GetHelpTipXpath(TPFT), GetHelpTipXpath(TPO));
        }

        private string GetMemberDiscountXpath(string coverType) => $"//div[contains(@class,'MuiCardContent-root') and contains(.,{coverType})]//span[@id='discount-0'][contains(text(),'{MEMBER_DISCOUNT}')]";

        public string GetMemberDiscountText(Contact driverDetails, MotorCovers desiredCover)
        {
            return GetInnerText(GetXpathForQuoteMemberDiscountText(desiredCover));
        }

        private string GetOnlineDiscountXpath(string coverType) => $"//div[contains(@class,'MuiCardContent-root') and contains(.,{coverType})]//span[@id='discount-0'][contains(text(),'{ONLINE_DISCOUNT}')]";

        private string GetHelpDialogueBannerTextXpath(string name) => $"{name}/following-sibling::p[1]";

        private string GetHelpTipXpath(string name) => $"//button[@id='coverTooltipId-{name}-Button']//*[@data-icon='circle-question']";

        private string GetXpathForQuoteBreakdownTextBasic(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.Breakdown.Basic, XPath.Policies.ThirdParty_FireAndTheft.Breakdown.Basic, XPath.Policies.ThirdParty_PropertyDamage.Breakdown.Basic);
        }

        private string GetXpathForQuoteBreakdownTextGov(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.Breakdown.Stamp, XPath.Policies.ThirdParty_FireAndTheft.Breakdown.Stamp, XPath.Policies.ThirdParty_PropertyDamage.Breakdown.Stamp);
        }

        private string GetXpathForQuoteBreakdownTextGST(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, XPath.Policies.Comprehensive.Breakdown.GST, XPath.Policies.ThirdParty_FireAndTheft.Breakdown.GST, XPath.Policies.ThirdParty_PropertyDamage.Breakdown.GST);
        }

        private string GetXpathForQuoteMemberDiscountText(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, GetMemberDiscountXpath(COMP), GetMemberDiscountXpath(TPFT), GetMemberDiscountXpath(TPO));
        }

        private string GetXpathForHelpDialogueBannerText(MotorCovers desiredCover)
        {
            return GetXpathByCover(desiredCover, GetHelpDialogueBannerTextXpath(XPath.Policies.Comprehensive.Help.DialogueTitle), GetHelpDialogueBannerTextXpath(XPath.Policies.ThirdParty_FireAndTheft.Help.DialogueTitle), GetHelpDialogueBannerTextXpath(XPath.Policies.ThirdParty_PropertyDamage.Help.DialogueTitle));
        }

        private string GetXpathByCover(MotorCovers desiredCover, string xpathOmco, string xpathOmtp, string xpathOmto)
        {
            var desiredXpath = xpathOmco;
            switch (desiredCover)
            {
                case MotorCovers.TFT:
                    desiredXpath = xpathOmtp;
                    break;
                case MotorCovers.TPO:
                    desiredXpath = xpathOmto;
                    break;
                default:
                    break;
            }
            return desiredXpath;
        }
        /// <summary>
        /// Motocycle NB - remove online discount (UI) https://rac-wa.atlassian.net/browse/SPK-6099 
        /// Checks if elements showing Online Discount information is not displayed.
        /// </summary>
        public void VerifyOnlineDiscountIsNotDisplayed()
        {
            Reporting.IsFalse(_driver.TryFindElement(By.XPath(GetOnlineDiscountXpath(COMP)),
                               out IWebElement compOnlineDiscount), "Online Discount is not shown for Comprehensive cover type");
            Reporting.IsFalse(_driver.TryFindElement(By.XPath(GetOnlineDiscountXpath(TPFT)),
                           out IWebElement tpftOnlineDiscount), "Online Discount is not shown for Third Party Fire and theft cover type");
            Reporting.IsFalse(_driver.TryFindElement(By.XPath(GetOnlineDiscountXpath(TPO)),
                                out IWebElement tpoOnlineDiscount), "Online Discount is not shown for Third Party cover type");
        }
        
        public void ClickSeeQuoteBreakdown(MotorCovers desiredCover)
        {
            var desiredDropdownXpath = GetXpathForSeeBreakdown(desiredCover);

            if (IsControlEnabled(desiredDropdownXpath))
            {
                ClickControl(desiredDropdownXpath);                
            }
            else
            {
                throw new ReadOnlyException("Dropdown is currently disabled and not clickable. Check input values.");
            }
        }

        public void ClickHelpTip(MotorCovers desiredCover)
        {
            var helpTipXpath = GetXpathForHelpTip(desiredCover);

            if (IsControlEnabled(helpTipXpath))
            {
                ClickControl(helpTipXpath);
                _driver.WaitForElementToBeVisible(By.XPath(GetXpathForHelpDialogue(desiredCover)), WaitTimes.T5SEC);
            }
            else
            {
                throw new ReadOnlyException("Help tip is currently disabled and not clickable. Check input values.");
            }
        }

        public void CloseHelpTip()
        {
            if (IsControlEnabled(XPath.General.QuoteDiologue.Close))
            {
                ClickControl(XPath.General.QuoteDiologue.Close);
            }
            else
            {
                throw new ReadOnlyException("Help tip close button is currently disabled and not clickable.");
            }
        }

        public bool AreAllQuoteBreakdownTextsDisplayed(MotorCovers desiredCover)
        {
            var quoteBreakdownTextBasic = GetXpathForQuoteBreakdownTextBasic(desiredCover);
            var quoteBreakdownTextGov = GetXpathForQuoteBreakdownTextGov(desiredCover);
            var quoteBreakdownTextGST = GetXpathForQuoteBreakdownTextGST(desiredCover);

            bool areQuoteBreakdownTextsDisplayed = IsControlDisplayed(quoteBreakdownTextBasic) && IsControlDisplayed(quoteBreakdownTextGov) && IsControlDisplayed(quoteBreakdownTextGST);

            return areQuoteBreakdownTextsDisplayed;
        }
        /// <summary>
        /// Verifies if the member discount is shown or not.
        /// If the member is eligible for discount(Gold, Silver or Bronze) and does not skip declaring membership then the correct member discount text is displayed.
        /// Otherwise, no member discount is shown.
        /// </summary>
        public void VerifyMemberDiscountIsDisplayedOrNot(Contact driverDetails, QuoteMotorcycle quoteMotorcycle)
        {
            if (IsEligibleForDiscount(driverDetails.MembershipTier) && !driverDetails.SkipDeclaringMembership)
            {
                Reporting.AreEqual(GetMemberDiscountText(quoteMotorcycle.Drivers[0].Details, MotorCovers.MFCO), $"{MemberDiscountMappings[driverDetails.MembershipTier].Motorcycle} " +
                    $"{driverDetails.MembershipTier.ToString()} {MEMBER_DISCOUNT}", $"Quote Member Discount Texts are displayed for {MotorcycleCoverNameMappings[MotorCovers.MFCO].TextB2C}");
                Reporting.AreEqual(GetMemberDiscountText(quoteMotorcycle.Drivers[0].Details, MotorCovers.TPO), $"{MemberDiscountMappings[driverDetails.MembershipTier].Motorcycle} " +
                    $"{driverDetails.MembershipTier.ToString()} {MEMBER_DISCOUNT}", $"Quote Member Discount Texts are displayed for {MotorcycleCoverNameMappings[MotorCovers.TPO].TextB2C}");
                Reporting.AreEqual(GetMemberDiscountText(quoteMotorcycle.Drivers[0].Details, MotorCovers.TFT), $"{MemberDiscountMappings[driverDetails.MembershipTier].Motorcycle} " +
                    $"{driverDetails.MembershipTier.ToString()} {MEMBER_DISCOUNT}", $"Quote Member Discount Texts are displayed for {MotorcycleCoverNameMappings[MotorCovers.TFT].TextB2C}");
            }
            else
            {
                Reporting.IsFalse(_driver.TryFindElement(By.XPath(GetMemberDiscountXpath(COMP)),
                               out IWebElement compMemberDiscount), "Member Discount should not be shown for Comprehensive cover type");
                Reporting.IsFalse(_driver.TryFindElement(By.XPath(GetMemberDiscountXpath(TPFT)),
                               out IWebElement tpftMemberDiscount), "Member Discount should not be shown for Third Party Fire and theft cover type");
                Reporting.IsFalse(_driver.TryFindElement(By.XPath(GetMemberDiscountXpath(TPO)),
                                    out IWebElement tpoMemberDiscount), "Member Discount should not be shown for Third Party cover type");
            }
        }

        public bool IsMostPopularRibbonDisplayed()
        {
            if (_driver.TryFindElement(By.XPath(XPath.Policies.Comprehensive.Help.DialogueRibbon), out IWebElement ribbon))
            {
                return (GetInnerText(XPath.Policies.Comprehensive.Help.DialogueRibbon) == MOST_POPULAR);
            }

            return false;
        }

        public bool IsBannerTextDisplayed(MotorCovers desiredCover)
        {
            Regex quoteHelpDialogBannerTextRegEx = null;
            switch (desiredCover)
            {
                case MotorCovers.MFCO:
                    quoteHelpDialogBannerTextRegEx = new Regex(FixedTextRegex.QUOTE_HELP_DIALOGUE_BANNER_TEXT_OMCO);
                    break;
                case MotorCovers.TPO:
                    quoteHelpDialogBannerTextRegEx = new Regex(FixedTextRegex.QUOTE_HELP_DIALOGUE_BANNER_TEXT_OMTP);
                    break;
                case MotorCovers.TFT:
                    quoteHelpDialogBannerTextRegEx = new Regex(FixedTextRegex.QUOTE_HELP_DIALOGUE_BANNER_TEXT_OMTO);
                    break;
                default:
                    break;
            }
            var quoteHelpDialogBannerText = GetInnerText(GetXpathForHelpDialogueBannerText(desiredCover)).StripLineFeedAndCarriageReturns();
            Reporting.Log($"Banner Text displayed is '{quoteHelpDialogBannerText}'");
            Match match = quoteHelpDialogBannerTextRegEx.Match(quoteHelpDialogBannerText);
            return match.Success;
        }

        /// <summary>
        /// The dynamic CSS that needs to be ignored from visual testing.
        /// </summary>
        public List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
              "#quoteNumber", 
              "#comprehensive-label-payment-amount",
              "[data-testid='comprehensive-savings-information-text']",
              "#excess-COMP", 
              "#sumInsured-COMP",
              "#thirdpartyfireandtheft-label-payment-amount",
              "[data-testid='thirdpartyfireandtheft-savings-information-text']",
              "#excess-TPFT",
              "#sumInsured-TPFT"
          };
    }
}
