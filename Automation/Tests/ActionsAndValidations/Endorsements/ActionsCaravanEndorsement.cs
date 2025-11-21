using Rac.TestAutomation.Common;
using System;
using UIDriver.Pages;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.Endorsements;
using UIDriver.Pages.Spark.Endorsements.CaravanEndorsement;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace Tests.ActionsAndValidations.Endorsements
{
    public static class ActionsCaravanEndorsement
    {
        /// <summary>
        /// Workflow to drive caravan renewal flow for various test data combination
        /// </summary>
        /// <param name="detailUiChecking">if set to true will verify copy text of field validation errors etc</param>
        /// <param name="isFailedPayment">To deal with Try again button displayed on Failed payment & verify the confirmation page specific to failed payment</param>
        /// <param name="retryOTP">If TRUE we will input an incorrect One-Time Password initially to test that behaviour, then provide the correct code to proceed</param>
        public static void CaravanRenewalFlow(Browser browser, EndorseCaravan endorseCaravan, bool detailUiChecking, bool isFailedPayment, bool retryOTP)
        {
            LaunchPage.OpenCaravanEndorsementByURL(browser, endorseCaravan, isRenewal:true);
            ConfirmYourCaravanMakeAndModel(browser, endorseCaravan, detailUiChecking);
            if (endorseCaravan.ChangeMakeAndModel)
            {
                UpdateYourNewCaravanDetails(browser, endorseCaravan, detailUiChecking);
            }
            ConfirmStorageAndUse(browser, endorseCaravan, detailUiChecking);
            HeresYourRenewal(browser, endorseCaravan, detailUiChecking);
            FacilitatePayment(browser, endorseCaravan, isFailedPayment, detailUiChecking);
            if (endorseCaravan.SparkExpandedPayment.PaymentOption.Equals(PaymentOptionsSpark.DirectDebit))
            {
                ActionMFA.RequestAndEnterOTP(browser, endorseCaravan.ActivePolicyHolder.MobilePhoneNumber, retryOTP , detailUiChecking);
            }
            VerifyConfirmationPage(browser, endorseCaravan, isFailedPayment);
        }

        /// <summary>
        /// Workflow to drive caravan endorsment flow for various test data combination
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void CaravanEndorseFlow(Browser browser, EndorseCaravan endorseCaravan, bool detailUiChecking = false)
        {
            LaunchPage.OpenCaravanEndorsementByURL(browser, endorseCaravan, isRenewal:false);
            SetEndorsementStartDate(browser, endorseCaravan.StartDate);
            ConfirmYourCaravanMakeAndModel(browser, endorseCaravan, detailUiChecking);
            if (endorseCaravan.ChangeMakeAndModel)
            {
                UpdateYourNewCaravanDetails(browser, endorseCaravan, detailUiChecking);
            }
            ConfirmStorageAndUse(browser, endorseCaravan, detailUiChecking);
            HeresYourPremium(browser, endorseCaravan, detailUiChecking);
            ReviewYourPolicy(browser, endorseCaravan, detailUiChecking);
            if (IsMFAExpectedOnMidTermEndorsement(endorseCaravan))
            {
                ActionMFA.RequestAndEnterOTP(browser, endorseCaravan.ActivePolicyHolder.MobilePhoneNumber, retryOTP: true, detailUiChecking);
            }
            VerifyEndorsementConfirmationPage(browser, endorseCaravan);
        }


        /// <summary>
        /// Workflow to select start date for the endorsement on 'Start Date' Page
        /// </summary>
        public static void SetEndorsementStartDate(Browser browser, DateTime endorsmentStartDate)
        {
            using (var startDate = new SetPolicyStartDate(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                startDate.SelectStartDate(endorsmentStartDate);
                startDate.VerificationOfNotificationCard();
                startDate.ClickNext();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Workflow to drive caravan renewal flow on 'Your caravan or trailer' Page
        /// </summary>
        /// <param name="detailUiChecking">if set to true will investigate field validation errors etc</param>
        public static void ConfirmYourCaravanMakeAndModel(Browser browser, EndorseCaravan endorseCaravan, bool detailUiChecking)
        {
            using (var yourCaravanOrTrailer = new YourCaravanOrTrailer(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                yourCaravanOrTrailer.WaitForPage(waitTimeSeconds: WaitTimes.T90SEC);
                yourCaravanOrTrailer.VerifyCaravanDetailsCard(endorseCaravan);

                if (detailUiChecking)
                {
                    yourCaravanOrTrailer.ClickNext();
                    yourCaravanOrTrailer.VerifyWarningMessage();
                }

                yourCaravanOrTrailer.ClickConfirmOrUpdateMakeandModel(!endorseCaravan.ChangeMakeAndModel);
                yourCaravanOrTrailer.VerifyCaravanNotificationCard();
                yourCaravanOrTrailer.ClickNext();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Workflow to drive caravan renewal flow on 'Update Your Caravan Details' Page
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void UpdateYourNewCaravanDetails(Browser browser, EndorseCaravan endorseCaravan, bool detailUiChecking)
        {
            using (var updateYourCaravan = new LetsUpdateYourCaravanOrTrailer(browser))
            using (var storageAndUse = new StorageAndUse(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                updateYourCaravan.WaitForPage();
                if (detailUiChecking)
                {
                    updateYourCaravan.ClickConfirm();
                    updateYourCaravan.VerifyFieldWarningMessages();
                }

                updateYourCaravan.SearchForCaravan(endorseCaravan, detailUiChecking);

                if (detailUiChecking)
                {
                    updateYourCaravan.VerifyCantFindWarningMessages();
                }
                updateYourCaravan.ClickConfirm();
                spinner.WaitForSpinnerToFinish(nextPage: storageAndUse);
            }
        }

        /// <summary>
        /// Workflow to drive caravan renewal flow on 'Storage and use' Page
        /// </summary>
        /// <param name="detailUiChecking">if set to true will investigate field validation errors etc</param>
        public static void ConfirmStorageAndUse(Browser browser, EndorseCaravan endorseCaravan, bool detailUiChecking)
        {
            using (var storageAndUse = new StorageAndUse(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                storageAndUse.WaitForPage();
                storageAndUse.VerifyCarRegistrationNotificationCard(endorseCaravan);

                if (!endorseCaravan.ChangeMakeAndModel)
                {
                    storageAndUse.VerifyExistingCaravanDetails(endorseCaravan);
                }
                if (detailUiChecking)
                {
                    storageAndUse.VerifyBusinessUseToolTip();
                    storageAndUse.VerifyBusinessUseKnockOutMessage();
                    storageAndUse.VerifyEmptySuburbValidationError();
                }
                storageAndUse.EnterNewCaravanDetails(endorseCaravan);
                storageAndUse.ClickNext();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Workflow to drive caravan endorsement flow on 'Here's your premium' Page
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void HeresYourPremium(Browser browser, EndorseCaravan endorseCaravan, bool detailUiChecking = false)
        {
            using (var heresYourPremiumPage = new HeresYourMidTermPremium(browser))
            {
                Reporting.IsTrue(heresYourPremiumPage.IsDisplayed(), "Here's Your Premium page is displayed");
                VerifyPremiumChangeControls(heresYourPremiumPage, endorseCaravan, detailUiChecking);
                heresYourPremiumPage.UpdateAgreedValueContentCoverAndExcess(endorseCaravan);
                heresYourPremiumPage.CapturePremiumChangeAndExcessValue(endorseCaravan);
                heresYourPremiumPage.ClickNext();
            }
            using (var reviewYourPolicyPage = new ReviewYourPolicy(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: reviewYourPolicyPage);
            }
        }

        /// <summary>
        /// Workflow to drive caravan renewal flow on 'Here's your renewal' Page
        /// </summary>
        /// <param name="detailUiChecking"> if set to true will investigate field validation errors etc</param>
        public static void HeresYourRenewal(Browser browser, EndorseCaravan endorseCaravan, bool detailUiChecking = false)
        {
            using (var heresYourRenewalPage = new HeresYourRenewalPremium(browser))
            {
                Reporting.IsTrue(heresYourRenewalPage.IsDisplayed(), "Here's Your Renewal page is displayed");
                VerifyPremiumChangeControls(heresYourRenewalPage, endorseCaravan, detailUiChecking);
                heresYourRenewalPage.UpdateFrequencySelection(endorseCaravan);
                heresYourRenewalPage.ClickNext();             
            }

            using (var payYourRenewalPage = new PayYourRenewal(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: payYourRenewalPage);
            }
        }

        /// <summary>
        /// Present options for payment and where requested enter details to pay.
        /// Members have a choice between setting up automatic direct debit, one off credit payments,
        /// or providing information on paying later (eg BPAY or Pay Later)
        /// </summary>
        /// <param name="isFailedPayment">To deal with Try again button displayed on Failed payment condition</param>
        /// <param name="detailUiChecking">if set to true will investigate field validation errors etc</param>
        public static void FacilitatePayment(Browser browser, EndorseCaravan endorseCaravan, bool isFailedPayment, bool detailUiChecking)
        {
            using (var payYourRenewalPage = new PayYourRenewalPremium(browser))
            using (var confirmationPage = new Confirmation(browser))
            using (var reviewPolicy = new ReviewYourPolicy(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                payYourRenewalPage.WaitForPage();
                payYourRenewalPage.VerifyPolicyCard(endorseCaravan);
                payYourRenewalPage.VerifyPremiumDetails(endorseCaravan);
                payYourRenewalPage.VerifyYourPolicySummary(endorseCaravan);
                payYourRenewalPage.CompletePaymentInputs(endorseCaravan, detailUiChecking);
                payYourRenewalPage.UpdateEmail(endorseCaravan.ActivePolicyHolder.GetEmail());
                payYourRenewalPage.AcceptPaymentAuthorisationTermsWhenRequired(endorseCaravan.SparkExpandedPayment);
                if (isFailedPayment)
                {
                    payYourRenewalPage.CreateFailedPayment();
                }
                else
                {
                    payYourRenewalPage.ClickConfirm();
                }
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Workflow to premium change 'Review Your Policy' Page
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        /// <param name="refundDestination">Set this for the expected refund destination</param>
        public static void ReviewYourPolicy(Browser browser, EndorseCaravan endorseCaravan, bool detailUiChecking = false)
        {
            using (var reviewYourPolicy = new ReviewYourCaravanPolicy(browser))
            using (var spinner = new SparkSpinner(browser))
            using (var payYourRenewals = new PayYourRenewal(browser))
            {
                reviewYourPolicy.WaitForPage();
                reviewYourPolicy.VerifyPolicyCard(endorseCaravan);
                reviewYourPolicy.VerifyPremiumChange(endorseCaravan);
                reviewYourPolicy.VerifyYourPolicySummary(endorseCaravan);
                reviewYourPolicy.VerifyPaymentMethodAndMakePayment(endorseCaravan);
                reviewYourPolicy.UpdateEmail(endorseCaravan.ActivePolicyHolder.GetEmail());

                if (IsAuthRequired(endorseCaravan))
                { payYourRenewals.AcceptPaymentAuthorisationTermsWhenRequired(endorseCaravan.SparkExpandedPayment); }
                reviewYourPolicy.ClickConfirm();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Verification for different payment methods (eg BPAY or Pay Later) on the Confirmation page
        /// </summary>
        /// <param name="isFailedPayment">Verify the confirmation screen specific to failed payment</param>
        public static void VerifyConfirmationPage(Browser browser, EndorseCaravan endorseCaravan, bool isFailedPayment)
        {
            using (var confirmationPage = new RenewalConfirmation(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                confirmationPage.WaitForPage();
                Reporting.LogPageChange("Confirmation page");
                switch (endorseCaravan.SparkExpandedPayment.PaymentOption)
                {
                    case PaymentOptionsSpark.AnnualCash:
                        if (isFailedPayment)
                        { confirmationPage.VerifyFailedPayment(endorseCaravan); }
                        else
                        { confirmationPage.VerifyPageAnnualCash(endorseCaravan); }
                        break;
                    case PaymentOptionsSpark.BPay:
                        confirmationPage.VerifyBpay(endorseCaravan);
                        break;
                    case PaymentOptionsSpark.PayLater:
                        confirmationPage.VerifyBpay(endorseCaravan);
                        break;
                    case PaymentOptionsSpark.DirectDebit:
                        confirmationPage.VerifyDirectDebit(endorseCaravan);
                        break;
                    default:
                        throw new NotImplementedException("A valid Payment Option value was not supplied");
                }
            }
        }

        /// <summary>
        /// Verification for different payment methods on the Confirmation page
        /// </summary>
        /// <param name="refundDestination">Set this for the expected refund destination</param>
        public static void VerifyEndorsementConfirmationPage(Browser browser, EndorseCaravan endorseCar)
        {
            using (var confirmationPage = new RenewalConfirmation(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                confirmationPage.WaitForPage();

                if (endorseCar.PayMethod.IsMonthly)
                { confirmationPage.VerifyConfirmationForMonthlyInstallment(endorseCar); }

                else // all the annual payment handling
                {
                    switch (endorseCar.ExpectedImpactOnPremium)
                    {
                        case PremiumChange.NoChange:
                            confirmationPage.VerifyNoChangePremiumConfirmation(endorseCar);
                            break;
                        case PremiumChange.PremiumDecrease:
                            confirmationPage.VerifyCreditCardRefundConfirmation(endorseCar, endorseCar.RefundDestination);
                            break;
                        case PremiumChange.PremiumIncrease:
                            if (endorseCar.PayMethod.Scenario.Equals(PaymentScenario.AnnualCash))
                            {
                                confirmationPage.VerifyIncreasePremiumConfirmationForAnnualCashPayment(endorseCar);
                            }
                            else // Annual instalments
                            {
                                confirmationPage.VerifyIncreasePremiumConfirmationForAnnualInstallment(endorseCar);
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        private static void VerifyPremiumChangeControls(CaravanBaseYourPremium pageObject, EndorseCaravan endorseCaravan, bool detailUiChecking)
        {
             if (detailUiChecking)
             {
                pageObject.WaitForPage();
                pageObject.VerifyExcessToolTip(endorseCaravan);
                pageObject.VerifyAgreedValueToolTip();
                pageObject.VerifyContentCoverToolTip();
             }
             pageObject.VerifyExcessValue(endorseCaravan);
             pageObject.VerifyAgreedValue(endorseCaravan);
             pageObject.VerifyContentCover(endorseCaravan);
        }

        /// <summary>
        /// Auth is dispalyed for Premium Increase regardless of payment plan or
        /// all monthly payment regardless of premium change or
        /// annual installment - no premium change 
        /// </summary>
        /// <param name="endorseCaravan"></param>
        /// <returns></returns>
        private static bool IsAuthRequired(EndorseCaravan endorseCaravan)
        {
            var isAcceptAuthRequired = endorseCaravan.ExpectedImpactOnPremium == PremiumChange.PremiumIncrease || 
                                        endorseCaravan.PayMethod.IsMonthly || 
                                           (endorseCaravan.PayMethod.Scenario.Equals(PaymentScenario.AnnualBank) && 
                                           (endorseCaravan.ExpectedImpactOnPremium.Equals(PremiumChange.NoChange)));
            return isAcceptAuthRequired;
        }

        /// <summary>
        ///Multi-factor authentication (MFA) is required whenever you add a new bank account or credit card and it's not a one-time payment
        ///if the payment type is an existing bank account or credit card then SparkExpandedPayment should be Null and it will return False
        ///Otherwise if the payment type is a new bank account or credit card(RefundToSource is not None) and it's not a one-time payment(Annual Cash), then will return True
        ///Else always return False
        /// </summary>
        /// <returns></returns>
        private static bool IsMFAExpectedOnMidTermEndorsement(EndorseCaravan endorseCaravan)
        {
            // MFA is not displayed if no expected payment is set on test builder
            if (endorseCaravan.SparkExpandedPayment == null)
            {
                return false;
            }
            else if (!endorseCaravan.SparkExpandedPayment.PaymentOption.Equals(PaymentOptionsSpark.AnnualCash) && !endorseCaravan.RefundDestination.Equals(RefundToSource.None))
            {
                return true;
            }
            return false;
        }
    }
}