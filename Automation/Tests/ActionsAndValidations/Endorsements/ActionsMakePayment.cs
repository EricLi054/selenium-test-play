using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIDriver.Pages;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.Endorsements.MakePayment;
using static Rac.TestAutomation.Common.Constants;
using static Rac.TestAutomation.Common.Constants.General;

namespace Tests.ActionsAndValidations.Endorsements
{
    public static class ActionsMakePayment
    {
        /// <summary>
        /// Navigate to the Make A Payment application URL,check payment page, fill Credit card info and Verify Confirmation Page
        /// </summary>
        /// <param name="detailUICheck">Set to true for details UI checking of the page</param>
        /// <param name="isFailedPayment">Set to true for failed payment scenario testing</param>
        public static void MakePayment(Browser browser, EndorsementBase testData, bool detailUICheck, bool isFailedPayment)
        {
            LaunchPage.OpenMakeAPaymentByURL(browser, testData);
            PerformPaymentAndVerify(browser, testData, detailUICheck, isFailedPayment);
            VerifyPaymentConfirmation(browser, testData, detailUICheck,isFailedPayment);
        }

        /// <summary>
        /// Check stepper details, Static UI text, policy card, amount due and authorisation text
        /// Along with entering CC details and submit payment for various products
        /// </summary>
        /// <param name="browser">browser</param>
        /// <param name="detailUICheck">pass true to perform details UI check</param>
        /// <param name="isFailedPayment">Set to true for failed payment scenario testing</param>
        private static void PerformPaymentAndVerify(Browser browser, EndorsementBase testData, bool detailUICheck, bool isFailedPayment)
        {
            using (var payment = new PaymentPage(browser))
            {
                payment.WaitForPage(waitTimeSeconds: WaitTimes.T90SEC);
                if (detailUICheck)
                {
                    payment.VerifyStepperNameAndState();
                    payment.VerifyUILabels();
                }
                payment.VerifyPolicyCard(testData);
                payment.VerifyAmountDue(testData);
                payment.FillCreditCard(testData.SparkExpandedPayment.CreditCardDetails);
                payment.VerifyCurrentEmailAddressAndUpdate(testData);
                payment.VerifyAuthTextAndSubmitPayment();
                if (isFailedPayment)
                {
                    payment.VerifyFailedPaymentPopUpAndTryAgain();
                }
            }
        }

        /// <summary>
        /// Check confirmation page for various product
        /// </summary>
        /// <param name="detailUICheck">pass true to perform details UI check</param>
        /// <param name="isFailedPayment">Set to true for failed payment scenario testing</param>
        private static void VerifyPaymentConfirmation(Browser browser, EndorsementBase testData, bool detailUICheck, bool isFailedPayment)
        {
            using (var confirmation = new ConfirmationPage(browser))
            {
                confirmation.WaitForPage();
                if (detailUICheck)
                {
                    confirmation.VerifyStepperNameAndState();
                }

                if (isFailedPayment)
                {
                    confirmation.VerifyFailedPaymentConfirmation(testData.PolicyNumber);
                }
                else if (testData.OriginalPolicyData.PaymentMethod.Equals(PolicyGeneral.PaymentScenarioIdMappings[PolicyGeneral.PaymentScenario.AnnualCash].CollectionMethod))
                {
                    confirmation.VerifyPageAnnualCash(testData);
                }
                else
                {
                    confirmation.VerifyPageDirectDebit(testData);              
                }
            }
        }
    }
}
