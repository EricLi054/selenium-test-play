using Rac.TestAutomation.Common;
using System;
using UIDriver.Pages;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.Endorsements;
using UIDriver.Pages.Spark.Endorsements.MotorRenewal;
using static Rac.TestAutomation.Common.Constants;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Tests.ActionsAndValidations.Endorsements
{
    public static class ActionsMotorEndorsement
    {
        /// <summary>
        /// Workflow to drive motor renewal flow for various test data combination
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        /// <param name="isFailedPayment">Optional Parameter, To deal with Try again button displayed on Failed payment & verify the confirmation page specific to failed payment</param>
        public static void MotorRenewalFlow(Browser browser, EndorseCar endorseCar, bool detailUiChecking = false, bool isFailedPayment = false)
        {
            LaunchPage.OpenMotorEndorsementByURL(browser, endorseCar, isRenewal:true);
            MotorRenewalFlowAfterLaunch(browser, endorseCar, detailUiChecking, isFailedPayment);
        }

        public static void MotorRenewalFlowAfterLaunch(Browser browser, EndorseCar endorseCar, bool detailUiChecking = false, bool isFailedPayment = false)
        {
            ConfirmYourCarMakeAndModel(browser, endorseCar, detailUiChecking);
            if (endorseCar.ChangeMakeAndModel)
            {
                UpdateYourNewCardDetails(browser, endorseCar, detailUiChecking);
            }
            ConfirmYourCarDetails(browser, endorseCar, detailUiChecking);
            ConfirmYourCarUsage(browser, endorseCar, detailUiChecking);
            HeresYourRenewal(browser, endorseCar, detailUiChecking);
            FacilitatePayment(browser, endorseCar, isFailedPayment, detailUiChecking);
        }

        /// <summary>
        /// Workflow to drive motor endorsment flow for various test data combination
        /// </summary>
        /// <param name="refundDestination">Optional parameter, To deal with the expected refund destination</param>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void MotorEndorseFlow(Browser browser, EndorseCar endorseCar, SparkCommonConstants.RefundToSource refundDestination= SparkCommonConstants.RefundToSource.None, bool detailUiChecking = false)
        {
            LaunchPage.OpenMotorEndorsementByURL(browser, endorseCar, isRenewal:false);

            SetStartDate(browser, endorseCar);
            ConfirmYourCarMakeAndModel(browser, endorseCar, detailUiChecking);
            if (endorseCar.ChangeMakeAndModel)
            {
                UpdateYourNewCardDetails(browser, endorseCar, detailUiChecking);
            }
            ConfirmYourCarDetails(browser, endorseCar, detailUiChecking);
            ConfirmYourCarUsage(browser, endorseCar,detailUiChecking);
            HeresYourPremium(browser, endorseCar, detailUiChecking);
            ReviewYourPolicy(browser, endorseCar,refundDestination: refundDestination, detailUiChecking);            
        }

        /// <summary>
        /// Workflow to drive motor renewal flow on 'Your Car' Page
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ConfirmYourCarMakeAndModel(Browser browser, EndorseCar endorseCar, bool detailUiChecking = false)
        {
            using (var yourCar = new YourCar(browser))
            using (var spinner = new SparkSpinner(browser))
            {                
                yourCar.WaitForPage(waitTimeSeconds: WaitTimes.T90SEC);
                yourCar.VerifyCarDetailsCard(endorseCar.InsuredAsset);

                if (detailUiChecking)
                {
                    yourCar.ClickNext();
                    yourCar.VerifyWarningMessage();
                }
                yourCar.ClickConfirmOrUpdateMakeandModel(!endorseCar.ChangeMakeAndModel);
                yourCar.VerifyCarNotificationCard();
                yourCar.ClickNext();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Workflow to drive motor renewal flow on 'Update Your Car Details' Page
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void UpdateYourNewCardDetails(Browser browser, EndorseCar endorseCar, bool detailUiChecking = false)
        {
            using (var updateYourCar = new LetsUpdateYourCar(browser))
            using (var spinner = new SparkSpinner(browser))
            {                
                updateYourCar.WaitForPage();
                if (detailUiChecking)
                {
                    updateYourCar.ClickConfirm();
                    updateYourCar.VerifyRegistrationWarning();
                    updateYourCar.ClickMakeAndModel();
                    updateYourCar.ClickConfirm();
                    updateYourCar.VerifyMakeAndModelWarning();
                }

                updateYourCar.SelectNewCar(endorseCar.NewInsuredAsset, detailUiChecking);
                updateYourCar.ClickConfirm();
            }
        }

        /// <summary>
        /// Workflow to drive motor renewal flow on 'Confirm Your Car Details' Page
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ConfirmYourCarDetails(Browser browser, EndorseCar endorseCar, bool detailUiChecking = false)
        {
            using (var confirmYourCarDetailsPage = new ConfirmYourCarDetails(browser))
            using (var confirmYourCarUsagePage = new ConfirmYourCarUsage(browser))
            using (var spinner = new SparkSpinner(browser))
            {               
                confirmYourCarDetailsPage.WaitForPage();
                confirmYourCarDetailsPage.VerifyCarRegistrationNotificationCard(endorseCar);

                if (!endorseCar.ChangeMakeAndModel)
                {
                    confirmYourCarDetailsPage.VerifyExistingCarDetails(endorseCar);
                }
                if (detailUiChecking)
                {
                    confirmYourCarDetailsPage.VerifyCarModificationToolTip();
                    confirmYourCarDetailsPage.VerifyFinanceToolTip();
                    if (endorseCar.ChangeMakeAndModel)
                    {
                        confirmYourCarDetailsPage.VerifyConfirmYourCarDetailsWarning();
                    }
                }
                confirmYourCarDetailsPage.EnterNewCarDetails(endorseCar);
                confirmYourCarDetailsPage.ClickNext();
                spinner.WaitForSpinnerToFinish(nextPage: confirmYourCarUsagePage);
            }
        }

        /// <summary>
        /// Workflow to drive the endorsement flow on 'Confirm Your Car Usage' Page for both
        /// Mid-Term Endorsements and Renewal endorsements.
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void ConfirmYourCarUsage(Browser browser, EndorseCar endorseCar,bool detailUiChecking = false)
        {
            using (var confirmYourCarUsagePage = new ConfirmYourCarUsage(browser))
            using (var spinner = new SparkSpinner(browser))
            {                
                confirmYourCarUsagePage.WaitForPage();
                confirmYourCarUsagePage.VerifyCarInformation(endorseCar.OriginalPolicyData);
                if (detailUiChecking)
                {
                    confirmYourCarUsagePage.VerifyCarUseToolTip();
                    confirmYourCarUsagePage.VerifyBusinessCarUseToolTip(endorseCar);
                    confirmYourCarUsagePage.VerifyAddressToolTip();
                    confirmYourCarUsagePage.VerifyCarUsageWarningMessage();
                }
                confirmYourCarUsagePage.SelectNewCarUsage(newCarUsage: endorseCar.UsageType);
                confirmYourCarUsagePage.SelectRiskAddress(endorseCar);
                confirmYourCarUsagePage.KmDrivenAnnually = endorseCar.AnnualKm.GetDescription();
                confirmYourCarUsagePage.ClickNext();
                spinner.WaitForSpinnerToFinish();                                                
            }
        }

        /// <summary>
        /// Workflow to select start date 'Start Date' Page
        /// </summary>
        public static void SetStartDate(Browser browser, EndorseCar endorseCar)
        {
            using (var startDate = new ChangePolicyStartDate(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                startDate.WaitForPage();
                startDate.SelectStartDate(endorseCar);
                startDate.VerificationOfNotificationCard();
                startDate.ClickNext();
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Workflow to premium change 'Review Your Policy' Page
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        /// <param name="refundDestination">Set this for the expected refund destination</param>
        public static void ReviewYourPolicy(Browser browser, EndorseCar endorseCar, SparkCommonConstants.RefundToSource refundDestination, bool detailUiChecking = false)
        {
            using (var reviewYourPolicy = new ReviewYourPolicy(browser))
            using (var confirmation = new Confirmation(browser))
            using (var spinner = new SparkSpinner(browser))
            using (var payYourRenewals = new PayYourRenewal(browser))
            {
                reviewYourPolicy.WaitForPage();
                reviewYourPolicy.VerifyPolicyCard(endorseCar);
                reviewYourPolicy.VerifyPremiumChange(endorseCar);
                reviewYourPolicy.VerifyYourPolicySummary(endorseCar, isRenewal: false);
                reviewYourPolicy.VerifyPaymentMethodAndMakePayment(endorseCar, refundDestination: refundDestination);
                reviewYourPolicy.UpdateEmail(endorseCar.ActivePolicyHolder);
                // Auth is dispalyed for Premium Increase regardless of payment plan or
                // all monthly payment regardless of premium change or
                // annual installment - no premium change 
                var isAcceptAuthRequired = endorseCar.ExpectedImpactOnPremium == PremiumChange.PremiumIncrease || endorseCar.PayMethod.IsMonthly || (endorseCar.PayMethod.Scenario.Equals(PaymentScenario.AnnualBank) && (endorseCar.ExpectedImpactOnPremium.Equals(PremiumChange.NoChange)));
                if (isAcceptAuthRequired)
                { payYourRenewals.AcceptPaymentAuthorisationTermsWhenRequired(endorseCar.SparkExpandedPayment); }
                reviewYourPolicy.ClickConfirm();
            }
        }


        /// <summary>
        /// Workflow to drive motor renewal flow on 'Here's your renewal' Page
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void HeresYourRenewal(Browser browser, EndorseCar endorseCar, bool detailUiChecking = false)
        {
            using (var heresYourRenewalPage = new HeresYourRenewal(browser))
            {
                VerifyPremiumChangeControlsAndUpdateExcess(heresYourRenewalPage, endorseCar, detailUiChecking);

                heresYourRenewalPage.UpdateFrequencySelection(endorseCar);

                heresYourRenewalPage.ClickConfirm();
            }


            using (var payYourRenewalPage = new PayYourRenewal(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: payYourRenewalPage);
            }
        }

        /// <summary>
        /// Workflow to drive motor endorsement flow on 'Here's your premium' Page
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void HeresYourPremium(Browser browser, EndorseCar endorseCar, bool detailUiChecking = false)
        {
            using (var heresYourPremiumPage = new HeresYourPremium(browser))
            {
                VerifyPremiumChangeControlsAndUpdateExcess(heresYourPremiumPage, endorseCar, detailUiChecking);
                heresYourPremiumPage.CapturePremiumChangeAndExcessValue(endorseCar);
                heresYourPremiumPage.ClickNext();
            }

            using (var reviewYourPolicyPage = new ReviewYourPolicy(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(nextPage: reviewYourPolicyPage);
            }
        }

        /// <summary>
        /// Present options for payment and where requested enter details to pay.
        /// Members have a choice between setting up automatic direct debit, one off credit payments,
        /// or providing information on paying later (eg BPAY or Pay Later)
        /// </summary>
        /// <param name="isFailedPayment">Optional Parameter - to deal with Try again button displayed on Failed payment condition</param>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field validation errors etc</param>
        public static void FacilitatePayment(Browser browser, EndorseCar endorseCar, bool isFailedPayment = false, bool detailUiChecking = false)
        {
            using (var payYourRenewalPage = new PayYourRenewal(browser))
            using (var confirmationPage = new Confirmation(browser))
            using(var reviewPolicy =  new ReviewYourPolicy(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                payYourRenewalPage.WaitForPage();
                payYourRenewalPage.VerifyPolicyCard(endorseCar);
                payYourRenewalPage.VerifyPremiumDetails(endorseCar);
                reviewPolicy.VerifyYourPolicySummary(endorseCar,isRenewal:true);
                payYourRenewalPage.CompletePaymentInputs(endorseCar, detailUiChecking);
                payYourRenewalPage.UpdateEmail(endorseCar.ActivePolicyHolder);
                payYourRenewalPage.AcceptPaymentAuthorisationTermsWhenRequired(endorseCar.SparkExpandedPayment);
                if (isFailedPayment)
                {
                    payYourRenewalPage.CreateFailedPayment();
                }
                else
                {
                    payYourRenewalPage.ClickConfirm();                   
                }
                spinner.WaitForSpinnerToFinish(WaitTimes.T150SEC);
            }
        }

        /// <summary>
        /// Verification for different payment methods (eg BPAY or Pay Later) on the Confirmation page
        /// </summary>
        /// <param name="isFailedPayment">Optional Parameter - to verify the confirmation screen specific to failed payment</param>
        public static void VerifyConfirmationPage(Browser browser, EndorseCar endorseCar, bool isFailedPayment = false)
        {
            using (var confirmationPage = new Confirmation(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                confirmationPage.WaitForPage();
                Reporting.LogPageChange("Confirmation page");
                switch (endorseCar.SparkExpandedPayment.PaymentOption)
                {
                    case PaymentOptionsSpark.AnnualCash:
                        if (isFailedPayment)
                        { confirmationPage.VerifyFailedPayment(endorseCar); }
                        else
                        { confirmationPage.VerifyPageAnnualCash(endorseCar); }
                        break;
                    case PaymentOptionsSpark.BPay:
                        confirmationPage.VerifyBpay(endorseCar);
                        break;
                    case PaymentOptionsSpark.PayLater:
                        confirmationPage.VerifyPayLater(endorseCar);
                        break;
                    case PaymentOptionsSpark.DirectDebit:
                        confirmationPage.VerifyDirectDebit(endorseCar);
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
        public static void VerifyEndorsementConfirmationPage(Browser browser, EndorseCar endorseCar, SparkCommonConstants.RefundToSource refundDestination)
        {
            using (var confirmationPage = new Confirmation(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                confirmationPage.WaitForPage();
                
                switch (endorseCar.ExpectedImpactOnPremium)
                {
                    case PremiumChange.NoChange:
                        if (endorseCar.PayMethod.IsMonthly)
                        {
                            confirmationPage.VerifyConfirmationForMonthlyInstallment(endorseCar);
                        }
                        else
                        {
                            confirmationPage.VerifyNoChangePremiumConfirmation(endorseCar);
                        }                            
                        break;
                    case PremiumChange.PremiumDecrease:
                        if (endorseCar.PayMethod.IsMonthly)
                        {
                            confirmationPage.VerifyConfirmationForMonthlyInstallment(endorseCar);
                        }
                        else
                        {
                            confirmationPage.VerifyCreditCardRefundConfirmation(endorseCar, refundDestination: refundDestination);
                        }
                        break;
                    case PremiumChange.PremiumIncrease:
                        if (endorseCar.PayMethod.Scenario.Equals(PaymentScenario.AnnualCash))
                        {
                            confirmationPage.VerifyIncreasePremiumConfirmationForAnnualCashPayment(endorseCar);
                        }
                        else if (endorseCar.PayMethod.IsMonthly)
                        {
                            confirmationPage.VerifyConfirmationForMonthlyInstallment(endorseCar);
                        }
                        else
                        {
                            confirmationPage.VerifyIncreasePremiumConfirmationForAnnualInstallment(endorseCar);
                        }                      
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private static void VerifyPremiumChangeControlsAndUpdateExcess(BaseYourPremium pageObject, EndorseCar endorseCar, bool detailUiChecking)
        {
            pageObject.WaitForPage();
            pageObject.VerifyUIForCoverType(endorseCar);
            if (detailUiChecking)
            {
                pageObject.VerifyExcessToolTip(endorseCar);
                pageObject.VerifyAgreedValueToolTip(endorseCar);
                pageObject.VerifyHireCarAfterAccidentToolTip(endorseCar);
                if (endorseCar.OriginalPolicyData.MotorAsset.HasNcbProtection)
                {
                    pageObject.VerifyNCBToolTip(endorseCar.IsMotorPolicyWithExcessChanges());
                }
            }

            pageObject.VerifyAdjustYourAmountSection(endorseCar);
            pageObject.Excess = endorseCar.Excess;
        }
    }
}