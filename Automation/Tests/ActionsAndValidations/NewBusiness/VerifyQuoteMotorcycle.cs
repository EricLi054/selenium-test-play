using Rac.TestAutomation.Common;
using UIDriver.Pages.Spark.MotorcycleQuote;

using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;

namespace Tests.ActionsAndValidations
{
    public static class VerifyQuoteMotorcycle
    {
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
    }
}
