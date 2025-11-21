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
    public class HeresYourPremium : BaseYourPremium
    {
        #region CONSTANTS
        private class Constants
        {
            public const string Title = "Here's your premium";

            public class Premium
            {
                public const string NoPremiumChangeHeader = "There's no change to your premium";
                public const string NoPremium = "$0.00";
                public const string DecreasePremiumChangeHeader = "Your premium will decrease by";
                public const string DecreasePremiumChangeMessage = "You'll receive a one-off refund";
                public const string IncreasePremiumChangeHeader = "Your premium will increase by";
                public const string IncreasePremiumChangeMessage = "You'll need to make a one-off payment to cover this change";
                               
                public const string MonthlyNextInstallment = "Your next instalment will be $";
                public const string MonthlyPremiumChangeMessage = "We'll spread this amount across your remaining instalments";
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public const string Title = "//h2[text()=" + "\"" + Constants.Title + "\"" + "]";

            public class Button
            {
                public const string Next = "id('your-premium-next-button')";
                public const string Back = "id('back-link')";
            }

            public class Premium
            {
                public const string PremiumChangeHeader = "id('premium-change-header')";
                public const string PremiumAmountChange = "id('premium-change-difference')";
                public const string PremiumChangeMessage = "id('premium-change-message')";

                public const string MonthlyNextInstallment = "id('premium-change-monthly-next-instalment')";
                public const string MonthlyPremiumChangeMessage = "id('premium-change-message')";
            }

        }
        #endregion

        #region Settable properties and controls
        private Decimal DifferenceAmount => decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.Premium.PremiumAmountChange)));
        #endregion

        public HeresYourPremium(Browser browser) : base(browser) { }

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
        public void CapturePremiumChangeAndExcessValue(EndorseCar endorseCar)
        {
            endorseCar.PremiumChangesAfterEndorsement = new PremiumDetails();

            using (var heresYourPremium = new HeresYourPremium(_browser))
            {
                if (endorseCar.PayMethod.IsMonthly)
                {
                    var newMonthlyPremium = decimal.Parse(GetInnerText(XPath.Premium.MonthlyNextInstallment).Replace(Constants.Premium.MonthlyNextInstallment, "").Trim());
                    endorseCar.PremiumChangesAfterEndorsement.TotalPremiumMonthly = newMonthlyPremium;
                }
                endorseCar.PremiumChangesAfterEndorsement.Total = heresYourPremium.DifferenceAmount;
                heresYourPremium.VerifyPremiumChangeText(endorseCar);
            }

            // If this is not defined, then we haven't modified the excess.
            // We buffer it, to make sure the displayed Excess is what is saved
            // at the end of the test.
            if (string.IsNullOrEmpty(endorseCar.Excess))
            {
                endorseCar.Excess = Excess;
            }
        }

        public void VerifyPremiumChangeText(EndorseCar car)
        {
            switch (car.ExpectedImpactOnPremium)
            {
                case PremiumChange.NoChange:
                    Reporting.AreEqual(Constants.Premium.NoPremiumChangeHeader, GetInnerText(XPath.Premium.PremiumChangeHeader), $"no premium change heading against actual");
                    Reporting.AreEqual(Constants.Premium.NoPremium, GetInnerText(XPath.Premium.PremiumAmountChange), $"premium change amount is displayed as {Constants.Premium.NoPremium} against actual");                
                    break;
                case PremiumChange.PremiumDecrease:
                    Reporting.AreEqual(Constants.Premium.DecreasePremiumChangeHeader, GetInnerText(XPath.Premium.PremiumChangeHeader), $"no premium change heading against actual");
                    if (car.PayMethod.IsAnnual)
                    {
                        Reporting.AreEqual(Constants.Premium.DecreasePremiumChangeMessage, GetInnerText(XPath.Premium.PremiumChangeMessage), $"premium change message is displayedas {Constants.Premium.DecreasePremiumChangeMessage} against actual");
                    }
                    break;
                case PremiumChange.PremiumIncrease:
                    Reporting.AreEqual(Constants.Premium.IncreasePremiumChangeHeader, GetInnerText(XPath.Premium.PremiumChangeHeader), $"no premium change heading against actual");
                    if (car.PayMethod.IsAnnual)
                    {
                        Reporting.AreEqual(Constants.Premium.IncreasePremiumChangeMessage, GetInnerText(XPath.Premium.PremiumChangeMessage), $"premium change message is displayed as {Constants.Premium.IncreasePremiumChangeMessage} against actual");
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            // All of the below verification is only for Monthly payment scenarios
            if (car.PayMethod.IsMonthly)
            {
                if (!car.ExpectedImpactOnPremium.Equals(PremiumChange.NoChange))
                {
                    Reporting.AreEqual(Constants.Premium.MonthlyPremiumChangeMessage, GetInnerText(XPath.Premium.MonthlyPremiumChangeMessage), $"premium change heading against actual");
                }
                Reporting.IsTrue(GetInnerText(XPath.Premium.MonthlyNextInstallment).StartsWith(Constants.Premium.MonthlyNextInstallment), $"the expected text preceeding the monthly instalment amount with the actual copy displayed on the page.");
            }
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }

    }
}
