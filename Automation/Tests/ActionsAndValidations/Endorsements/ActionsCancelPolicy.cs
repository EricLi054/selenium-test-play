using Rac.TestAutomation.Common;
using System;
using System.Threading;
using UIDriver.Pages;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.Endorsements;
using static Rac.TestAutomation.Common.Constants.General;

namespace Tests.ActionsAndValidations.Endorsements
{

    public static class ActionsCancelPolicy
    {
        /// <summary>
        /// Updates reason and email address.  These are compulsory fields.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="contact"></param>
        /// <param name="cancellationReason"></param>
        public static void CompleteCancellationDetails(Browser browser, Contact contact, string cancellationReason)
        {
            UpdateCancellationReason(browser, cancellationReason);
            UpdateEmailAddress(browser, contact.GetEmail());
            Reporting.Log($"Email address updated and reason changed to \"{cancellationReason}\"", browser.Driver.TakeSnapshot());
        }

        /// <summary>
        /// Update cancellation reason and allow time for processing.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="cancellationReason">Reason to select in cancellation reason picker</param>
        private static void UpdateCancellationReason(Browser browser, string cancellationReason)
        {
            using (var cancellationDetailsPage = new CancellationDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                cancellationDetailsPage.SetReason(cancellationReason);
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T60SEC);
            }
        }

        /// <summary>
        /// Fills in the fields for the Cancellation details page
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="contact">Contact </param>
        /// <param name="cancellationReason">Reason for cancalling the policy</param>
        public static void UpdateCancellationForSellingHouseAndCheckExtraInformationDisplayed(Browser browser)
        {
            using (var cancellationDetailsPage = new CancellationDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                cancellationDetailsPage.ChooseHouseSold();
                spinner.WaitForSpinnerToFinish(WaitTimes.T150SEC);
                VerifyCancelPolicy.ValidateExtraInformationHouseSold(browser);
            }
        }

        public static void UpdateCancellationForHouseDemolishedAndCheckExtraInformationDisplayed(Browser browser)
        {
            using (var cancellationDetailsPage = new CancellationDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                cancellationDetailsPage.ChooseHouseDemolished();
                spinner.WaitForSpinnerToFinish();
                VerifyCancelPolicy.ValidateExtraInformationHouseDemolished(browser);
            }
        }

        public static void UpdateCancellationForFinancialHardshipAndCheckExtraInformationDisplayed(Browser browser)
        {
            using (var cancellationDetailsPage = new CancellationDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                cancellationDetailsPage.ChooseFinancialHarship();
                spinner.WaitForSpinnerToFinish();
                VerifyCancelPolicy.ValidateExtraInformationFinancialHardship(browser);
            }
        }

        public static void UpdateLastDayOfCoverDate(Browser browser, DateTime endOfCoverDate)
        {
            using (var cancellationDetailsPage = new CancellationDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                cancellationDetailsPage.LastDayOfCover = endOfCoverDate;
                spinner.WaitForSpinnerToFinish();
                VerifyCancelPolicy.VerifyCancellationLastDayOfCoverIsCorrectDate(browser, endOfCoverDate);
            }
        }

        public static void UpdateEmailAddress(Browser browser, string emailAddress)
        {
            using (var cancellationDetailsPage = new CancellationDetailsPage(browser))
            {
                cancellationDetailsPage.EmailAddress = emailAddress;
                Thread.Sleep(SleepTimes.T2SEC);
            }
        }

        /// <summary>
        /// Check that when responses are invalid, that the 
        /// correct error messages are displayed
        /// </summary>
        /// <param name="browser"></param>
        public static void SubmitCancellationWithErrorsAndCheckFieldValidationMessages(Browser browser)
        {
            using (var cancellationDetailsPage = new CancellationDetailsPage(browser))
            {
                cancellationDetailsPage.EmailAddress = string.Empty;
                // Open and close reason list without selecting a reason
                cancellationDetailsPage.OpenReasonList();
                cancellationDetailsPage.CloseReasonList();

                // As error message will not display until focus has moved from field
                // this is repeated for the occasion where the member has no existing email address
                cancellationDetailsPage.EmailAddress = string.Empty;
                VerifyCancelPolicy.VerifyErrorMessagesOnCancellationDetailsPage(browser);
            }
        }

        /// <summary>
        /// A confirmation modal is shown to the member to make sure
        /// the member wishes to proceed with the cancellation.
        /// 
        /// Check that the correct information is displayed in the modal.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="productDescription">Type of insurance</param>
        /// <param name="policyNumber"></param>
        public static void CheckConfirmationModalAndThenConfirmCancellation(Browser browser, string productDescription,
            string policyNumber, DateTime policyLastDayOfCover)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                VerifyCancelPolicy.VerifyConfirmationModalContents(browser, productDescription, policyNumber, policyLastDayOfCover);
                cancellationPage.ConfirmPolicyCancellation();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// A confirmation modal is shown to the member to make sure
        /// the member wishes to proceed with the cancellation.  In this 
        /// this instance, the member decides not to proceed.
        /// 
        /// Check that the correct information is displayed in the modal.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="productDescription">Type of insurance</param>
        /// <param name="policyNumber"></param>
        public static void CheckConfirmationModalAndThenRejectCancellation(Browser browser, string productDescription,
            string policyNumber, DateTime policyLastDayOfCover)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                VerifyCancelPolicy.VerifyConfirmationModalContents(browser, productDescription, policyNumber, policyLastDayOfCover);
                cancellationPage.RejectPolicyCancellation();
                Thread.Sleep(SleepTimes.T10SEC);
            }
        }

        public static void SubmitCancellationRequest(Browser browser)
        {
            using (var cancellationDetailsPage = new CancellationDetailsPage(browser))
            {
                cancellationDetailsPage.SubmitCancellation();
            }
        }
    }
}
