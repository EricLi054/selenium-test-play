using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace UIDriver.Pages.Spark.CaravanQuote
{
    public class PremiumChangePopup : SparkBasePage
    {
        #region XPATHS
        private class XPath
        {
            public const string Dialog = "//div[@role='dialog' and @aria-labelledby='premium-changed-title']";
            public static class Button
            {
                public const string Close = Dialog + "//button[@id='button-premium-changed-next']";
            }
            public static class Contents
            {
                public const string Title   = Dialog + "//h2[@id='premium-changed-title']/h1";
                public const string Message = Dialog + "//p[@id='premium-changed-content-text']";

                public static class NewPremium
                {
                    /// <summary>
                    /// This is the label for the new premium based on the user's
                    /// selected payment frequency.
                    /// </summary>
                    public const string LabelForPrimary = Dialog + "//div[@data-testid='main-cost-container']/div/h4";
                    public const string Annual          = Dialog + "//h4[@id='premium-changed-annual-cost']//strong";
                    public const string Monthly         = Dialog + "//h4[@id='premium-changed-monthly-cost']//strong";
                }

                public static class Breakdown
                {
                    public const string ShowHideLink = Dialog + "//a[@data-testid='link-caravan-premium-breakdown']";
                    public const string BasicPremium = "//span[@id='caravan-premium-breakdown-basic']";
                    public const string StampDuty    = "//span[@id='caravan-premium-breakdown-government-charges']";
                    public const string GST          = "//span[@id='caravan-premium-breakdown-gst']";
                }
            }
        }

        private static readonly string QUOTE_UPDATE_TITLE_TEXT_AGE_BASED = "We've updated your quote";
        private static readonly string QUOTE_UPDATE_MESSAGE_TEXT_AGE_BASED = "We've updated some things since your quote was created and your price has changed.";
        private static readonly string QUOTE_UPDATE_TITLE_TEXT_PERSONAL_DETAILS_MATCHED = "Good news";
        private static readonly string QUOTE_UPDATE_MESSAGE_TEXT_QUALIFIED_FOR_DISCOUNT = "A member discount has been applied.";
        private static readonly string ANNUAL_PRICE_TEXT = "New annual price";
        private static readonly string MONTHLY_PRICE_TEXT = "New monthly price";
        private static readonly string QUOTE_BREAKDOWN_TEXT = "See quote breakdown";


        #endregion
        //Rate change thresholds for 'Trailed Caravan', based on the 'Version 12' of 'Caravan Driver Age Factor (ID 1000220)' table.
        public readonly static int DRIVER_AGE_FACTOR_RATE_GROUP2_MIN_AGE = 21;
        public readonly static int DRIVER_AGE_FACTOR_RATE_GROUP3_MIN_AGE = 24;
        public readonly static int DRIVER_AGE_FACTOR_RATE_GROUP4_MIN_AGE = 50;
        public readonly static int DRIVER_AGE_FACTOR_RATE_GROUP5_MIN_AGE = 71;
        public readonly static int DRIVER_AGE_FACTOR_RATE_GROUP6_MIN_AGE = 76;

        #region Settable properties and controls 

        #endregion

        public PremiumChangePopup(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Dialog);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }

        public void ClickPremiumBreakdownLink()
        {
            ClickControl(XPath.Contents.Breakdown.ShowHideLink);
        }

        /// <summary>
        /// Used to get Basic premium for any payment frequency
        /// </summary>
        /// <returns></returns>
        public decimal GetPremiumBreakdownBasic()
        {
            return decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.Contents.Breakdown.BasicPremium)));
        }

        /// <summary>
        /// Used to get Stamp duty value for any payment frequency
        /// </summary>
        /// <returns></returns>
        public decimal GetPremiumBreakdownStamp()
        {
            return decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.Contents.Breakdown.StampDuty)));
        }

        /// <summary>
        /// Used to get GST value for any payment frequency
        /// </summary>
        /// <returns></returns>
        public decimal GetPremiumBreakdownGST()
        {
            return decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.Contents.Breakdown.GST)));
        }

        /// <summary>
        /// Premium Change Popup can appear due to one or more of the following reasons:
        /// 1. Main Policyholder's age based rate, can change based on the age at the Policy start date.
        ///    This can happen on the 'Great Let's Set A Start Date' page when we set the policy start date to a future date.
        /// 2. A discount eligible member who skipped declaring the membership level, or answered 'No' on the 'Are you an RAC member' page,
        ///    gets Single Matched on 'Tell us more about you', 'Tell us more about your joint policyholder'or 'Membership Level' page.
        /// </summary>
        public void WaitForPremiumChangePopup(PremiumChangeTrigger premiumChangeTrigger, QuoteCaravan quoteData)
        {

            var mainPHFirstName = quoteData.PolicyHolders[0].FirstName;
            WaitForPage(waitTimeSeconds: WaitTimes.T5SEC);

            switch (premiumChangeTrigger)
            {
                case PremiumChangeTrigger.POLICY_START_DATE:
                    Reporting.AreEqual(QUOTE_UPDATE_TITLE_TEXT_AGE_BASED, GetInnerText(XPath.Contents.Title));
                    break;
                case PremiumChangeTrigger.MEMBER_MATCH:
                    if (mainPHFirstName.Length > 12)
                    {
                        Reporting.AreEqual($"{QUOTE_UPDATE_TITLE_TEXT_PERSONAL_DETAILS_MATCHED}", GetInnerText(XPath.Contents.Title));
                    }
                    else
                    {
                        Reporting.AreEqual($"{QUOTE_UPDATE_TITLE_TEXT_PERSONAL_DETAILS_MATCHED} {mainPHFirstName}", GetInnerText(XPath.Contents.Title), true, "title of premium price reduced modal pop-up is as expected:");
                    }
                    break;
                default:
                    Reporting.Error($"'{premiumChangeTrigger}' Premium Change Trigger is not yet implemented");
                    break;
            }
        }

         public void VerifyPopupContent(PremiumChangeTrigger premiumChangeTrigger, QuoteCaravan quoteData)
         {
            var paymentFrequency = quoteData.PayMethod.PaymentFrequency;

            var pHCount = quoteData.PolicyHolders.Count;

            switch (premiumChangeTrigger)
            {
                case PremiumChangeTrigger.POLICY_START_DATE:
                    Reporting.AreEqual(QUOTE_UPDATE_MESSAGE_TEXT_AGE_BASED, GetInnerText(XPath.Contents.Message));
                    break;
                case PremiumChangeTrigger.MEMBER_MATCH:
                    Reporting.AreEqual(QUOTE_UPDATE_MESSAGE_TEXT_QUALIFIED_FOR_DISCOUNT, GetInnerText(XPath.Contents.Message));
                    break;
                default:
                    Reporting.Error($"'{premiumChangeTrigger}' Premium Change Trigger is not yet implemented");
                    break;
            }

            var expectedLabelText = paymentFrequency == PaymentFrequency.Annual ?
                                    ANNUAL_PRICE_TEXT : MONTHLY_PRICE_TEXT;
            Reporting.AreEqual(expectedLabelText, GetInnerText(XPath.Contents.NewPremium.LabelForPrimary));

            Reporting.AreEqual(QUOTE_BREAKDOWN_TEXT, GetInnerText(XPath.Contents.Breakdown.ShowHideLink));
        }

        public void VerifyPremiumChange(Browser browser, QuoteCaravan quoteCaravan, SparkBasePage.QuoteStage quoteStage)
        {
            decimal originalPremiumValue;
            string xp_NewPremium;

            if (quoteCaravan.PayMethod.PaymentFrequency == PaymentFrequency.Annual)
            {
                originalPremiumValue = quoteCaravan.QuoteData.AnnualPremium;
                quoteCaravan.QuoteData.AnnualPremium = decimal.Parse(GetInnerText(XPath.Contents.NewPremium.Annual).StripMoneyNotations());
                xp_NewPremium = XPath.Contents.NewPremium.Annual;
            }
            else
            {
                originalPremiumValue = quoteCaravan.QuoteData.MonthlyPremium;
                quoteCaravan.QuoteData.MonthlyPremium = decimal.Parse(GetInnerText(XPath.Contents.NewPremium.Monthly).StripMoneyNotations());
                xp_NewPremium = XPath.Contents.NewPremium.Monthly;
            }

            decimal newPremiumValue = decimal.Parse((GetInnerText(xp_NewPremium).StripMoneyNotations()));

            if (GetExpectedImpactOnPremium(quoteCaravan, quoteStage) == PremiumChange.PremiumIncrease)
            { Reporting.IsTrue(newPremiumValue > originalPremiumValue, $"Premium {newPremiumValue} has increased as expected over {originalPremiumValue}"); }
            else
            { Reporting.IsTrue(newPremiumValue < originalPremiumValue, $"Premium {newPremiumValue} has decreased as expected from {originalPremiumValue}"); }

            using (var premiumChangePopup = new PremiumChangePopup(browser))
            {
                premiumChangePopup.ClickPremiumBreakdownLink();
                quoteCaravan.QuoteData.PremiumBreakdownBasic = premiumChangePopup.GetPremiumBreakdownBasic();
                quoteCaravan.QuoteData.PremiumBreakdownStamp = premiumChangePopup.GetPremiumBreakdownStamp();
                quoteCaravan.QuoteData.PremiumBreakdownGST = premiumChangePopup.GetPremiumBreakdownGST();
            }

            Reporting.Log($"Updated premium breakdowns are: ${quoteCaravan.QuoteData.PremiumBreakdownBasic} basic, ${quoteCaravan.QuoteData.PremiumBreakdownStamp} stamp and ${quoteCaravan.QuoteData.PremiumBreakdownGST} GST", browser.Driver.TakeSnapshot());

            ClickControl(XPath.Button.Close);

            using (var spinner = new SparkSpinner(_browser))
            { spinner.WaitForSpinnerToFinish(); }
        }

        /// <summary>
        /// A premium change can happen due to the following reasons:
        ///
        /// 1. Policyholder's age based rate, can change based on the age at the Policy start date.
        ///     -This can happen on the 'Great Let's Set A Start Date' page when we set the policy start date to a future date.
        ///     -In this sceanrio, we calculate if we are supposed to get a premium increase or decrease based on the:
        ///         1.1. Policyholders date of birth
        ///         1.2. Today's date
        ///         1.3. Policy start date
        ///      for spark caravan based on the 'Version 12' of 'Caravan Driver Age Factor (ID 1000220)' table.
        ///
        /// 2. A discount eligible member who skipped declaring the membership level, or answered 'No' on the 'Are you an RAC member' page,
        ///    gets Single Matched on 'Tell us more about you' page.
        ///    In this scenario the premium is expected to decrease.
        /// </summary>
        /// <param name="quoteData"></param>
        /// <returns></returns>
        public PremiumChange GetExpectedImpactOnPremium(QuoteCaravan quoteData, SparkBasePage.QuoteStage quoteStage)
        {
            var mainPH = quoteData.PolicyHolders[0];

            if (quoteStage == SparkBasePage.QuoteStage.AFTER_PERSONAL_INFO)
            { return PremiumChange.PremiumDecrease; }
            else if (quoteStage == SparkBasePage.QuoteStage.AFTER_QUOTE)
            {
                var ageAtStartDate = quoteData.StartDate.Year - mainPH.DateOfBirth.Year;
                if (mainPH.DateOfBirth.Date > quoteData.StartDate.AddYears(-ageAtStartDate)) ageAtStartDate--;

                if (mainPH.GetContactAge() < DRIVER_AGE_FACTOR_RATE_GROUP6_MIN_AGE && ageAtStartDate >= DRIVER_AGE_FACTOR_RATE_GROUP6_MIN_AGE)
                { return PremiumChange.PremiumIncrease; }
                else
                { return PremiumChange.PremiumDecrease; }
            }
            else
                return PremiumChange.NoChange;
        }
    }
}