using Rac.TestAutomation.Common;
using System;
using System.Threading;
using UIDriver.Pages;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.Endorsements;
using UIDriver.Pages.Spark.Endorsements.UpdateHowYouPay;
using static Rac.TestAutomation.Common.Constants.General;

namespace Tests.ActionsAndValidations.Endorsements
{

    public static class ActionsUpdateHowYouPay
    {
        /// <summary>
        /// Verify the It's Renewal time page content
        /// and click the Review and renew button
        /// </summary>       
        public static void ReviewAndRenew(Browser browser, EndorsementBase endoresement)
        {
            using (var itsRenewalTime = new ItsRenewalTime(browser))
            {
                itsRenewalTime.VerifyPageContent(endoresement.OriginalPolicyData.EndorsementStartDate);
                itsRenewalTime.ClickReviewAndRenewButton();
            }

            // We do the following as the Update How I Pay handover to Motor Endorsement Renewal doesn't
            // pass the One Time Password override value in NPE.
            Reporting.Log($"We have jumped to the Motor Endorsement Online renewal flow, so refresheshing the One Time Password override");
            using (var yourCar = new YourCar(browser))
            {
                yourCar.WaitForPage(waitTimeSeconds: WaitTimes.T90SEC);
                if (!Config.Get().IsBypassOTPEnabled())
                { LaunchPage.SetNumberOfOTPOverride(browser); }
            }
        }

        /// <summary>
        /// Adds the supplied bank account as a new source bank account
        /// for payments.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="bankAccountDetails">Bank account object with bsb, number and name</param>
        /// <param name="detailUIChecking">Set to true for bsb validation scenarios</param>
        public static void AddNewBankAccount(Browser browser, BankAccount bankAccountDetails, bool detailUIChecking=false)
        {
            using (var updateHowYouPayDetails = new UpdateHowYouPayDetailsPage(browser))
            {
                if (detailUIChecking)
                {
                    updateHowYouPayDetails.VerifyEmptyBSBDetailsWarning();
                    updateHowYouPayDetails.EnterInvalidNoMatchBSBAndCheckErrorMessage();
                }
                updateHowYouPayDetails.AddNewAccount(bankAccountDetails);
                updateHowYouPayDetails.VerifyBSBDetails(bankAccountDetails);
            }
        }

        /// <summary>
        /// Adds the supplied credit as the new payment source for a policy
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="creditCard"></param>
        public static void AddNewCreditCard(Browser browser, CreditCard creditCard)
        {
            using (var updateHowYouPayDetails = new UpdateHowYouPayDetailsPage(browser))
            {
                updateHowYouPayDetails.AddNewCreditCard(creditCard);
            }
        }

        /// <summary>
        /// Updates the next payment date with the date supplied
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="DateToSelect"></param>
        public static void UpdateNextPaymentDate(Browser browser, DateTime DateToSelect)
        {
            using (var updateHowYouPayDetails = new UpdateHowYouPayDetailsPage(browser))
            {
                updateHowYouPayDetails.NextPaymentDate = DateToSelect;
                Thread.Sleep(SleepTimes.T500MS);
                Reporting.Log("Next instalment date changed", browser.Driver.TakeSnapshot());
                updateHowYouPayDetails.VerifyPaymentUpdatedPromptToConfirm();
            }
        }

        /// <summary>
        /// Updates the form with the supplied email address.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="contact"></param>
        public static void UpdateEmailAddress(Browser browser, Contact contact, bool detailUIChecking=false)
        {
            using (var updateHowYouPayDetails = new UpdateHowYouPayDetailsPage(browser))
            {
                if (detailUIChecking)
                {
                    updateHowYouPayDetails.EmailAddress = "";
                    updateHowYouPayDetails.VerifyEmailWarning();
                    Reporting.Log("Warning message when email is not provided", browser.Driver.TakeSnapshot());
                }
                //Generate a random email address if member don't have one previously
                if (string.IsNullOrEmpty(contact.GetEmail()))
                {
                    updateHowYouPayDetails.EmailAddress = DataHelper.RandomEmail(contact.FirstName, contact.Surname, Config.Get().Email.Domain).Address;
                }
                else
                {
                    updateHowYouPayDetails.EmailAddress = contact.GetEmail();
                }                
            }
        }

        /// <summary>
        /// Accepts the terms and conditions for credit card and bank debit payments
        /// then confirms
        /// </summary>
        /// <param name="browser"></param>
        public static void AcceptTermsAndConditionsThenConfirm(Browser browser)
        {
            using (var updateHowYouPayDetails = new UpdateHowYouPayDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                
                updateHowYouPayDetails.ScrollTermsAndConditionsIntoView();
                updateHowYouPayDetails.AcceptPaymentTermsAndConditions();
                Reporting.Log("Terms and conditions for payment accepted", browser.Driver.TakeSnapshot());
                updateHowYouPayDetails.ConfirmUpdate();
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T90SEC);
            }
        }
    }
}
