using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Linq;
using static Rac.TestAutomation.Common.Constants;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace UIDriver.Pages.Spark.Endorsements
{
    abstract public class CaravanBaseYourPremium : SparkBasePage
    {
        #region CONSTANTS
        private static class Constants
        {
            public static class AdjustHowYouPaySection
            {
                public static readonly string Title = "Adjust the amount you pay";
            }
            public static class ToolTip
            {
                public static class Title
                {
                    public static readonly string AgreedValue = "Agreed value";                  
                    public static readonly string ContentCover = "Contents cover";
                    public static readonly string Excess = "Excess";
                }

                public static class Content
                {
                    public static readonly string AgreedValue = "The agreed value is the amount we agree to insure your caravan or trailer for. It includes GST, registration, on-road costs and accessories fitted to your caravan.";

                    public static class ContentCover
                    {
                        public static readonly string ParagraphOne = "Your policy includes $1000 of contents cover against loss or damage caused by things like fire, storm, collision, theft or malicious damage.";
                        public static readonly string ParagraphTwo = "Contents includes clothing, personal belongings, furnishings, bedding, portable fridges and electrical appliances contained in your caravan or annexe.";
                        public static readonly string ParagraphThree = "If you increase your cover, you'll pay more.";
                    }

                    public static class ContentExcess
                    {
                        public static readonly string ParagraphOne = "The excess is the amount you may need to pay towards settlement of any claim.";
                        public static readonly string ParagraphTwo = "If you adjust your excess, your premium will change.";
                        public static readonly string ExtraExcessHeading = "Extra excesses may apply:";
                        public static readonly string ExtraExcessOptionOne = "Driver under 19: $650";
                        public static readonly string ExtraExcessOptionTwo = "Driver under 21: $550";
                        public static readonly string ExtraExcessOptionThree = "Driver under 24: $450";
                        public static readonly string ExtraExcessOptionFour = "Driver under 26: $300";
                        public static readonly string ExtraExcessOptionFive = "Special excess: will be stated in your policy documents if applicable";
                        public static readonly string ParagraphThree = "See the Premium, Excess and Discount Guide for more information.";
                    }
                }
            }
        }
        #endregion

        #region XPATHS
        private static class XPath
        {

            public static class ToolTip
            {
                public static class Excess
                {
                    public static class Button
                    {
                        public static readonly string Show = "id('excess-tooltip-idButton')";
                        public static readonly string Close = "//button[@aria-label='close']";
                    }
                    public static readonly string Title = "id('excess-dialog-title')";
                    public static readonly string ParagraphOne = "id('excess-dialog-paragraph-one')";
                    public static readonly string ParagraphTwo = "id('excess-dialog-paragraph-two')";
                    public static readonly string ExtraExcessHeading = "id('excess-dialog-extra-excesses')";
                    public static readonly string ExtraExcessOptionOne = "id('excess-dialog-extra-excesses-one')";
                    public static readonly string ExtraExcessOptionTwo = "id('excess-dialog-extra-excesses-two')";
                    public static readonly string ExtraExcessOptionThree = "id('excess-dialog-extra-excesses-three')";
                    public static readonly string ExtraExcessOptionFour = "id('excess-dialog-extra-excesses-four')";
                    public static readonly string ExtraExcessOptionFive = "id('excess-dialog-extra-excesses-five')";
                    public static readonly string ParagraphThree = "id('excess-dialog-paragraph-four')";
                }

                public static class AgreedValue
                {
                    public static class Button
                    {
                        public static readonly string Show = "id('agreed-value-tooltipButton')";
                        public static readonly string Close = "id('agreed-value-tooltip-close')";
                    }
                    public static readonly string Title = "id('agreed-value-tooltip-title')";
                    public static readonly string Content = "id('agreed-value-tooltip-message-text')";
                }

                public static class ContentCover
                {
                    public static class Button
                    {
                        public static readonly string Show = "id('contents-cover-tooltip-idButton')";
                        public static readonly string Close = "id('contents-cover-tooltip-id-close')";
                    }
                    public static readonly string Title = "id('contents-cover-tooltip-id-title')";
                    public static readonly string ParagraphOne = "//div[@id='contents-cover-tooltip-id-message']//p[1]";
                    public static readonly string ParagraphTwo = "//div[@id='contents-cover-tooltip-id-message']//p[2]";
                    public static readonly string ParagraphThree = "//div[@id='contents-cover-tooltip-id-message']//p[3]";
                }
            }

            public static class AdjustHowYouPaySection
            {
                public static readonly string Title = "//h3[text()='" + Constants.AdjustHowYouPaySection.Title + "']";
                public static readonly string Excess = "id('excess-dropdown')";
                public static readonly string ExcessOptions = "//ul[@role='listbox']//li";
                public static readonly string AgreedValue = "id('agreed-value-input')";
                public static readonly string ContentCover = "id('contents-cover-dropdown')";
                public static readonly string ContentCoverOptions = "//ul[@role='listbox']//li";
            }
        }
        #endregion

        #region Settable properties and controls


        public string Excess
        {
            get => DataHelper.StripMoneyNotations(GetInnerText(XPath.AdjustHowYouPaySection.Excess));
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    WaitForSelectableAndPickFromDropdown(XPath.AdjustHowYouPaySection.Excess, XPath.AdjustHowYouPaySection.ExcessOptions, value);

                    using (var spinner = new SparkSpinner(_browser))
                    {
                        spinner.WaitForSpinnerToFinish(WaitTimes.T150SEC);
                    }
                }
            }
        }

        private int AgreedValue
        {
            get => DataHelper.ConvertMonetaryStringToInt(GetValue(XPath.AdjustHowYouPaySection.AgreedValue));
            set => WaitForTextFieldAndEnterText(XPath.AdjustHowYouPaySection.AgreedValue, value.ToString("0"), false);
        }


        #endregion

        protected CaravanBaseYourPremium(Browser browser) : base(browser) { }


        /// <summary>
        /// The Agreed Value from Shield is decimal but as per SPK-4141
        /// Agreed Value presented in Spark will have the cents truncated
        /// </summary>
        private void VerifyAgreedValue(Decimal expectedAgreedValue)
        {
            var expected = (int)Math.Floor(expectedAgreedValue);
            Reporting.AreEqual(expected, AgreedValue, $"expected Agreed Value matches actual value displayed.");
        }

        /// <summary>
        /// Open, verify and close Excess tool tip
        /// </summary>
        public void VerifyExcessToolTip(EndorseCaravan endorseCaravan)
        {
            if (IsEligibleForExcess(endorseCaravan))
            {
                ClickControl(XPath.ToolTip.Excess.Button.Show);
                Reporting.AreEqual(Constants.ToolTip.Title.Excess, GetInnerText(XPath.ToolTip.Excess.Title), $"expected title of Excess tool tip against actual");
                Reporting.AreEqual(Constants.ToolTip.Content.ContentExcess.ParagraphOne, GetInnerText(XPath.ToolTip.Excess.ParagraphOne), $"expected content of Excess tool tip against actual");
                Reporting.AreEqual(Constants.ToolTip.Content.ContentExcess.ParagraphTwo, GetInnerText(XPath.ToolTip.Excess.ParagraphTwo), $"expected content of Excess tool tip against actual");

                Reporting.AreEqual(Constants.ToolTip.Content.ContentExcess.ExtraExcessHeading, GetInnerText(XPath.ToolTip.Excess.ExtraExcessHeading), $"expected content of Excess tool tip against actual");
                Reporting.AreEqual(Constants.ToolTip.Content.ContentExcess.ExtraExcessOptionOne, GetInnerText(XPath.ToolTip.Excess.ExtraExcessOptionOne), $"expected content of Excess tool tip against actual");
                Reporting.AreEqual(Constants.ToolTip.Content.ContentExcess.ExtraExcessOptionTwo, GetInnerText(XPath.ToolTip.Excess.ExtraExcessOptionTwo), $"expected content of Excess tool tip against actual");
                Reporting.AreEqual(Constants.ToolTip.Content.ContentExcess.ExtraExcessOptionThree, GetInnerText(XPath.ToolTip.Excess.ExtraExcessOptionThree), $"expected content of Excess tool tip against actual");
                Reporting.AreEqual(Constants.ToolTip.Content.ContentExcess.ExtraExcessOptionFour, GetInnerText(XPath.ToolTip.Excess.ExtraExcessOptionFour), $"expected content of Excess tool tip against actual");
                Reporting.AreEqual(Constants.ToolTip.Content.ContentExcess.ExtraExcessOptionFive, GetInnerText(XPath.ToolTip.Excess.ExtraExcessOptionFive), $"expected content of Excess tool tip against actual");

                Reporting.AreEqual(Constants.ToolTip.Content.ContentExcess.ParagraphThree, GetInnerText(XPath.ToolTip.Excess.ParagraphThree), $"expected content of Excess tool tip against actual");
                ClickControl(XPath.ToolTip.Excess.Button.Close);
            }
        }

        /// <summary>
        /// Verified the Content Cover displayed
        /// </summary>
        public void VerifyContentCover(EndorseCaravan endorseCaravan)
        {
            if (endorseCaravan.OriginalPolicyData.Covers.First().CoverTypeDescription.Equals("Trailer"))
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.AdjustHowYouPaySection.ContentCover), "Content cover option is not displayed for Trailer");
            }
            else if (endorseCaravan.OriginalPolicyData.Covers[1].CoverTypeDescription.Equals("Caravan Contents"))
            {
                var contentCoverExpected = endorseCaravan.OriginalPolicyData.Covers[1].SumInsured;
                Reporting.AreEqual(contentCoverExpected.ToString().Replace(".0", "").Trim(), DataHelper.StripMoneyNotations(GetInnerText(XPath.AdjustHowYouPaySection.ContentCover)), "expected value on content cover");
            }
        }

        /// <summary>
        /// Verify the agreed value displayed
        /// </summary>
        public void VerifyAgreedValue(EndorseCaravan endorseCaravan)
        {
            var agreedValueExpected = endorseCaravan.ChangeMakeAndModel ? endorseCaravan.NewInsuredAsset.MarketValue : endorseCaravan.OriginalPolicyData.Covers.First().SumInsured;
            Reporting.AreEqual(agreedValueExpected.ToString().Replace(".0", "").Trim(), DataHelper.StripMoneyNotations(GetValue(XPath.AdjustHowYouPaySection.AgreedValue)), $"expected agreed value against actual");
        }

        /// <summary>
        /// Verify the Excess value displayed
        /// </summary>
        public void VerifyExcessValue(EndorseCaravan endorseCaravan)
        {

            if (IsEligibleForExcess(endorseCaravan))
            {
                Reporting.AreEqual(DataHelper.StripMoneyNotations(endorseCaravan.OriginalPolicyData.Covers.First().StandardExcess.ToString().Replace(".0", "").Trim()),
                    DataHelper.StripMoneyNotations(GetInnerText(XPath.AdjustHowYouPaySection.Excess)), $"expected excess value against actual");
            }
            else
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.AdjustHowYouPaySection.Excess), "Excess is not displayed as the age of the policy holder is more than 50");
            }
        }

        /// <summary>
        /// Open, verify and close Agreed value tool tip
        /// </summary>
        public void VerifyAgreedValueToolTip()
        {
            ClickControl(XPath.ToolTip.AgreedValue.Button.Show);
            Reporting.AreEqual(Constants.ToolTip.Title.AgreedValue, GetInnerText(XPath.ToolTip.AgreedValue.Title), $"expected title of Agreed value tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.AgreedValue, GetInnerText(XPath.ToolTip.AgreedValue.Content), $"expected content of a agreed value tool tip against actual");
            ClickControl(XPath.ToolTip.AgreedValue.Button.Close);
        }

        /// <summary>
        /// Open, verify and close Content Cover tool tip
        /// </summary>
        public void VerifyContentCoverToolTip()
        {
            ClickControl(XPath.ToolTip.ContentCover.Button.Show);
            Reporting.AreEqual(Constants.ToolTip.Title.ContentCover, GetInnerText(XPath.ToolTip.ContentCover.Title), $"expected title of content cover tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.ContentCover.ParagraphOne, GetInnerText(XPath.ToolTip.ContentCover.ParagraphOne), $"expected content of a agreed value tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.ContentCover.ParagraphTwo, GetInnerText(XPath.ToolTip.ContentCover.ParagraphTwo), $"expected content of a agreed value tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.ContentCover.ParagraphThree, GetInnerText(XPath.ToolTip.ContentCover.ParagraphThree), $"expected content of a agreed value tool tip against actual");
            ClickControl(XPath.ToolTip.ContentCover.Button.Close);
        }

        /// <summary>
        /// Check the policy is eligible to display excess related info on the policy
        /// </summary>
        public bool IsEligibleForExcess(EndorseCaravan endorseCaravan)
        {
            var basicExcess = DataHelper.StripMoneyNotations(endorseCaravan.OriginalPolicyData.Covers.First().StandardExcess.ToString()).Replace(".0", "").Trim();
            // TODO: SPK-6704 Remove the if check when this story is actioned. This logic will no longer be required as all the active caravan policies will be on product version ID 68000024
            if (endorseCaravan.OriginalPolicyData.ProductVersionAsInteger >= PolicyCaravan.CaravanProductVersionIdWithExcessNcbChanges)
            {
                return true;
            }
            else
            {
                return endorseCaravan.ActivePolicyHolder.GetContactAge() > 50 ? false : true;
            }
        }
    }
}
