using Rac.TestAutomation.Common;
using UIDriver.Pages.Spark.MotorcycleQuote;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;

namespace Tests.ActionsAndValidations
{
    public static class VerifyQuoteMotorcycle
    {
        #region Constants

        public const string DUPLICATE_ALERT_TITLE = "You may already be insured";
        public const string DUPLICATE_ALERT_CONTENT = "Please call us on 13 17 03 so we can help you.";

        #endregion

        /// <summary>
        /// Verifies that the duplicate policy alert is displayed on the page.
        /// This method checks for the alert visibility and validates its title and content.
        /// </summary>
        /// <param name="browser">The browser instance</param>
        public static void DuplicatePolicyAlert(Browser browser)
        {
            using (var tellUsMoreAboutYou = new TellUsMoreAboutYou(browser))
            {
                Reporting.Log("Verifying duplicate policy alert dialog.", browser.Driver.TakeSnapshot());
                
                if (!tellUsMoreAboutYou.IsDuplicateAlertVisible())
                {
                    System.Threading.Thread.Sleep(2000);
                    Reporting.IsTrue(tellUsMoreAboutYou.IsDuplicateAlertVisible(), "Duplicate policy alert dialog should be visible");
                }
                
                Reporting.AreEqual(DUPLICATE_ALERT_TITLE, tellUsMoreAboutYou.GetDuplicateAlertTitle(), "Duplicate alert title should match expected text");
                Reporting.AreEqual(DUPLICATE_ALERT_CONTENT, tellUsMoreAboutYou.GetDuplicateAlertContent(), "Duplicate alert content should match expected text");
            }
        }

        /// <summary>
        /// Answers the "Are you a member?" question.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="contact"></param>
        public static void QuoteDetailsOnPaymentPage(Browser browser, QuoteMotorcycle quoteDetails)
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

        /// <summary>
        /// If the premium change popup is displayed, verifies it and closes it (updating quote data).
        /// </summary>
        public static void PremiumChangePopupIfDisplayed(Browser browser, QuoteMotorcycle quote)
        {
            Thread.Sleep(2000);
            using (var popup = new PremiumChangePopup(browser))
            {
                if (popup.IsDisplayed())
                    popup.VerifyPremiumChange(browser, quote, SparkBasePage.QuoteStage.AFTER_QUOTE);
            }
        }
    }
}
