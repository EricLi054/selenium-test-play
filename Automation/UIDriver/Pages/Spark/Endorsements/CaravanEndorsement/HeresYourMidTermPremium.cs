using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using System;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace UIDriver.Pages.Spark.Endorsements
{
    public class HeresYourMidTermPremium : CaravanBaseYourPremium
    {
        #region CONSTANTS
        private static class Constants
        {
            public static readonly string Title = "Here's your premium";

            public static class Premium
            {
                public static readonly string NoPremiumChangeHeader = "There's no change to your premium";
                public static readonly string NoPremium = "$0.00";
                public static readonly string DecreasePremiumChangeHeader = "Your premium will decrease by";
                public static readonly string DecreasePremiumChangeMessage = "You'll receive a one-off refund";
                public static readonly string IncreasePremiumChangeHeader = "Your premium will increase by";
                public static readonly string IncreasePremiumChangeMessage = "You'll need to make a one-off payment to cover this change";

                public static readonly string MonthlyNextInstallment = "Your next instalment will be $";
                public static readonly string MonthlyPremiumChangeMessage = "We'll spread this amount across your remaining instalments";
            }

            public static readonly string AgreedValueRange = "You can insure your caravan or trailer for between ";
        }
        #endregion

        #region XPATHS
        private static class XPath
        {
            public static readonly string Title = "//h2[text()=" + "\"" + Constants.Title + "\"" + "]";

            public static class Button
            {
                public static readonly string Next = "id('your-premium-next-button')";
                public static readonly string Back = "id('back-link')";
            }

            public static class Premium
            {
                public static readonly string PremiumChangeHeader = "id('premium-change-header')";
                public static readonly string PremiumAmountChange = "id('premium-change-difference')";
                public static readonly string PremiumChangeMessage = "id('premium-change-message')";

                public static readonly string MonthlyNextInstallment = "id('premium-change-monthly-next-instalment')";
                public static readonly string MonthlyPremiumChangeMessage = "id('premium-change-message')";
            }

            public static readonly string AgreedValueRange = "//p[starts-with(text()," + "\"" + Constants.AgreedValueRange + "\"" + ")]";
            public static readonly string AgreedValueInput = "id('agreed-value-input')";
            public static readonly string ContentCoverDropdownBase = "id('contents-cover-dropdown')";
            public static readonly string ContentCoverDropdown = "id('contents-cover-dropdown-list')/li";

        }
        #endregion

        #region Settable properties and controls
        private Decimal DifferenceAmount => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.Premium.PremiumAmountChange)));
        public string ContentCover
        {
            get => GetInnerText(XPath.ContentCoverDropdownBase);
            set => WaitForSelectableAndPickFromDropdown(XPath.ContentCoverDropdownBase, XPath.ContentCoverDropdown, value);
        }
        public string AgreedValue
        {
            get => GetValue(XPath.AgreedValueInput);
            set => WaitForTextFieldAndEnterText(XPath.AgreedValueInput, value+Keys.Tab, false);
        }
        #endregion

        public HeresYourMidTermPremium(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Title);
                Reporting.Log($"Page 1: {GetInnerText(XPath.Title)} is Displayed");
                GetElement(XPath.Button.Next);
                GetElement(XPath.Button.Back);

            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Here's your premium page");
            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// Automation captures the resulting change in premium as
        /// well as the excess (if the automation hadn't set a
        /// specified value).
        /// </summary>
        public void CapturePremiumChangeAndExcessValue(EndorseCaravan endorseCaravan)
        {
            endorseCaravan.PremiumChangesAfterEndorsement = new PremiumDetails();

            using (var heresYourPremium = new HeresYourMidTermPremium(_browser))
            {
                if (endorseCaravan.PayMethod.IsMonthly)
                {
                    var newMonthlyPremium = decimal.Parse(GetInnerText(XPath.Premium.MonthlyNextInstallment).Replace(Constants.Premium.MonthlyNextInstallment, "").Trim());
                    endorseCaravan.PremiumChangesAfterEndorsement.TotalPremiumMonthly = newMonthlyPremium;
                }
                endorseCaravan.PremiumChangesAfterEndorsement.Total = heresYourPremium.DifferenceAmount;
                heresYourPremium.VerifyPremiumChangeText(endorseCaravan);
            }

            // If this is not defined, then we haven't modified the excess.
            // We buffer it, to make sure the displayed Excess is what is saved
            // at the end of the test.
            if (string.IsNullOrEmpty(endorseCaravan.Excess) && IsEligibleForExcess(endorseCaravan))
            {
                endorseCaravan.Excess = Excess;
            }

            if (string.IsNullOrEmpty(endorseCaravan.ContentCover) && endorseCaravan.OriginalPolicyData.Covers[1].CoverTypeDescription.Equals("Caravan Contents"))
            {
                endorseCaravan.ContentCover = ContentCover;
            }

            endorseCaravan.NewAgreedValue = AgreedValue;
        }

        public void VerifyPremiumChangeText(EndorseCaravan caravan)
        {
            switch (caravan.ExpectedImpactOnPremium)
            {
                case PremiumChange.NoChange:
                    Reporting.AreEqual(Constants.Premium.NoPremiumChangeHeader, GetInnerText(XPath.Premium.PremiumChangeHeader), $"no premium change heading against actual");
                    Reporting.AreEqual(Constants.Premium.NoPremium, GetInnerText(XPath.Premium.PremiumAmountChange), $"premium change amount is displayed as {Constants.Premium.NoPremium} against actual");                
                    break;
                case PremiumChange.PremiumDecrease:
                    Reporting.AreEqual(Constants.Premium.DecreasePremiumChangeHeader, GetInnerText(XPath.Premium.PremiumChangeHeader), $"no premium change heading against actual");
                    if (caravan.PayMethod.IsAnnual)
                    {
                        Reporting.AreEqual(Constants.Premium.DecreasePremiumChangeMessage, GetInnerText(XPath.Premium.PremiumChangeMessage), $"premium change message is displayedas {Constants.Premium.DecreasePremiumChangeMessage} against actual");
                    }
                    break;
                case PremiumChange.PremiumIncrease:
                    Reporting.AreEqual(Constants.Premium.IncreasePremiumChangeHeader, GetInnerText(XPath.Premium.PremiumChangeHeader), $"no premium change heading against actual");
                    if (caravan.PayMethod.IsAnnual)
                    {
                        Reporting.AreEqual(Constants.Premium.IncreasePremiumChangeMessage, GetInnerText(XPath.Premium.PremiumChangeMessage), $"premium change message is displayed as {Constants.Premium.IncreasePremiumChangeMessage} against actual");
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            // All of the below verification is only for Monthly payment scenarios
            if (caravan.PayMethod.IsMonthly)
            {
                if (!caravan.ExpectedImpactOnPremium.Equals(PremiumChange.NoChange))
                {
                    Reporting.AreEqual(Constants.Premium.MonthlyPremiumChangeMessage, GetInnerText(XPath.Premium.MonthlyPremiumChangeMessage), $"premium change heading against actual");
                }

                if (caravan.ExpectedImpactOnPremium.Equals(PremiumChange.NoChange))
                {
                    // API returns this as a double, we convert it to a decimal here
                    var expectedNextInstalment = Convert.ToDecimal(caravan.OriginalPolicyData.NextPayableInstallment.OutstandingAmount);
                    var onscreenNextInstalment = DataHelper.GetMonetaryValueFromString(GetInnerText(XPath.Premium.MonthlyNextInstallment));
                    Reporting.AreEqual(expectedNextInstalment, onscreenNextInstalment, "the expected text and amount is displayed.");
                }
                else
                {
                    Reporting.IsTrue(GetInnerText(XPath.Premium.MonthlyNextInstallment).StartsWith(Constants.Premium.MonthlyNextInstallment), "the expected monthly next installment text is displayed.");
                }
            }
        }

        public void ClickNext()
        {
            Reporting.Log($" Premium page", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.Next);
        }

        /// <summary>
        /// Updating the Agreed value, Excess and Content cover
        /// </summary>
        public void UpdateAgreedValueContentCoverAndExcess(EndorseCaravan endorseCaravan)
        {
            using (var spinner = new SparkSpinner(_browser))
            {                                    
                //Updating the Excess value
                if (!string.IsNullOrEmpty(endorseCaravan.Excess) && IsEligibleForExcess(endorseCaravan))
                {
                    Excess = endorseCaravan.Excess;
                    spinner.WaitForSpinnerToFinish();
                }

                //Updating the Agreed value
                var agreedValueRangeAmount = GetInnerText(XPath.AgreedValueRange).Replace(Constants.AgreedValueRange, "").Split(new[] { " and " }, StringSplitOptions.None);

                if (endorseCaravan.ExpectedImpactOnPremium.Equals(PremiumChange.PremiumIncrease))
                {
                    //Update agreed value to higher value to increase the premium
                    AgreedValue = agreedValueRangeAmount[1];
                    spinner.WaitForSpinnerToFinish();
                }
                else if (endorseCaravan.ExpectedImpactOnPremium.Equals(PremiumChange.PremiumDecrease))
                {
                    //Update agreed value to lower value to decrease the premium
                    AgreedValue = agreedValueRangeAmount[0];
                    spinner.WaitForSpinnerToFinish();
                }

                //Updating the Content Cover value
                if (!string.IsNullOrEmpty(endorseCaravan.ContentCover) && !endorseCaravan.OriginalPolicyData.Covers.First().CoverTypeDescription.Equals("Trailer"))
                {
                    ContentCover = endorseCaravan.ContentCover;
                    spinner.WaitForSpinnerToFinish();
                }                
            }
        }      
    }
}
