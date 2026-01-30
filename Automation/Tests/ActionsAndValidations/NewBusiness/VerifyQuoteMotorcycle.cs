using Rac.TestAutomation.Common;
using UIDriver.Pages.Spark.MotorcycleQuote;

using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;

namespace Tests.ActionsAndValidations
{
    public static class VerifyQuoteMotorcycle
    {
        #region Constants

        public const string DUPLICATE_ALERT_TITLE   = "You may already be insured";
        public const string DUPLICATE_ALERT_CONTENT = "Please call us so we can help you.";

        #endregion
        /// <summary>
        /// Verifies that the duplicate policy alert is displayed on the page.
        /// This method checks for the alert visibility and validates its title and content.
        /// </summary>
        /// <param name="browser">The browser instance</param>
        public static void VerifyDuplicatePolicyAlert(Browser browser)
        {
            using (var tellUsMoreAboutYou = new TellUsMoreAboutYou(browser))
            {
                Reporting.Log("Verifying duplicate policy alert dialog.", browser.Driver.TakeSnapshot());

                if (!tellUsMoreAboutYou.IsDuplicateAlertVisible())
                {
                    Reporting.IsTrue(tellUsMoreAboutYou.IsDuplicateAlertVisible(), "Duplicate policy alert dialog should be visible");
                }

                Reporting.AreEqual(DUPLICATE_ALERT_TITLE, tellUsMoreAboutYou.GetDuplicateAlertTitle(), "Duplicate alert title should match expected text");
                Reporting.AreEqual(DUPLICATE_ALERT_CONTENT, tellUsMoreAboutYou.GetDuplicateAlertContent(), "Duplicate alert content should match expected text");
            }
        }

        public static void VerifyPremiumChangePopup(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var tellUsMoreAboutYou = new TellUsMoreAboutYou(browser))
            using (var progressBar = new MotorcycleProgressBar(browser))
            {
                if (quoteMotorcycle.IsPremiumChangeExpected)
                {
                    VerifyAnyPremiumChangePopup(browser, quoteMotorcycle);
                }
                else
                {
                    tellUsMoreAboutYou.VerifyNoPremiumPopupIsDisplayed();
                }
            }
        }

        /// <summary>
        /// Answers the "Are you a member?" question.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="contact"></param>
        public static void VerifyQuoteDetailsOnPaymentPage(Browser browser, QuoteMotorcycle quoteDetails)
        {
            using (var page = new PaymentDetails(browser))
            {
                page.WaitForPage();

                Reporting.AreEqual(MotorcycleCoverNameMappings[quoteDetails.CoverType].TextB2C, page.PolicyType, true, "Motocycle policy type");

                var shownExcess = page.Excess.StripMoneyNotations();
                if (quoteDetails.Excess != null)
                {
                    var expectedExcess = quoteDetails.Excess.StripMoneyNotations();
                    Reporting.AreEqual(expectedExcess, shownExcess, "Excess value ");
                }
                else
                {
                    Reporting.Log($"Test did not override excess, so accepted given excess {shownExcess}");
                }
                Reporting.Log($"Capturing Screenshot of Payment screen", browser.Driver.TakeSnapshot());          

                var shownVehicle = page.MotorCycleModelDescription;
                var expectedVehicle = quoteDetails.GetFullMotorcycleName();

                Reporting.AreEqual(expectedVehicle, shownVehicle, true, "Vehicle description");

                if (quoteDetails.CoverType != MotorCovers.TPO)
                {
                    Reporting.AreEqual(quoteDetails.SumInsuredFromQuotePage, int.Parse(page.SumInsured), "Sum Insured in payment details page matched the original Sum Insured in Policy Details page");
                }
                else
                {
                    Reporting.Log($"Motor Cover = {MotorCovers.TPO} so there is no Sum Insured to compare");
                }
            }
        }

        private static void VerifyAnyPremiumChangePopup(Browser browser, QuoteMotorcycle quoteMotorcycle)
        {
            using (var premiumChangePopup = new PremiumChangePopup(browser))
            {
                try
                {
                    premiumChangePopup.WaitForPage();
                    premiumChangePopup.VerifyPopupContent(quoteMotorcycle);
                    premiumChangePopup.VerifyPremiumChange(browser, quoteMotorcycle, SparkBasePage.QuoteStage.AFTER_PERSONAL_INFO);
                }
                catch
                {
                    Reporting.Error("Premium change pop up is expected on this scenario");
                }
            }
        }
    }
}
