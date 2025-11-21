using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DataModels;
using System;
using UIDriver.Pages;
using UIDriver.Pages.Spark.MemberRefund;

namespace Tests.ActionsAndValidations
{
    public static class ActionMemberRefund
    {
 
        /// <summary>
        /// Navigate to the MRO url
        /// </summary>
        public static void OpenMRO(Browser browser)
        {
            LaunchPage.OpenMemberRefundOnlineURL(browser);
        }

        /// <summary>
        /// Checking the static text, field labels, warning message on Let's get some information page
        /// Entering details required for the page and click next
        /// </summary>
        public static void CompleteRefundDetailsEntry(Browser browser, RefundDetails testData, bool detailUiCheck)
        {
            using (var letsGetSomeInformation = new LetsGetSomeInformation(browser)) 
            {
                letsGetSomeInformation.WaitForPage();
                if (detailUiCheck)
                {
                    letsGetSomeInformation.VerifyUIFieldLabels();
                    letsGetSomeInformation.VerifyStepperName();
                    letsGetSomeInformation.VerifyActiveStepper();
                    letsGetSomeInformation.ClickNext();
                    letsGetSomeInformation.VerifyWarningTextForMandatoryFields();
                }
                letsGetSomeInformation.EnterRefundDetails(testData);
                letsGetSomeInformation.ClickNext();
            }
        }

        /// <summary>
        /// Checking static text, validation messages for no otp,invalid otp
        /// Read and Enter OTP from the Mailosaur account 
        /// </summary>
        public static void EnterAndVerifyOTP(Browser browser, RefundDetails testData, bool detailUiCheck)
        {
            var isOTPSuccessful = false;
            var endTime = DateTime.Now.AddMinutes(5);

            using (var letsVerifyItsYou = new LetsVerifyItsYou(browser))
            {
                letsVerifyItsYou.WaitForPage();

                var oneTimePasscode = Config.Get().IsBypassOTPEnabled() ?
                                      Constants.SparkCommonConstants.MultiFactorAuthentication.BypassOTP.VerificationCode :
                                      DataHelper.GetOTPFromSMS();

                if (detailUiCheck)
                {
                    letsVerifyItsYou.VerifyDetailUIChecking(oneTimePasscode);
                }

                do
                {
                    // If we didn't find an OTP, don't cancel, it may still be pending delivery.
                    if (string.IsNullOrEmpty(oneTimePasscode))
                    { continue; }

                    isOTPSuccessful = letsVerifyItsYou.EnterOTP(oneTimePasscode);
                    if (!isOTPSuccessful)
                    { 
                        letsVerifyItsYou.RequestNewOTPCode();
                        oneTimePasscode = Config.Get().IsBypassOTPEnabled() ?
                                          Constants.SparkCommonConstants.MultiFactorAuthentication.BypassOTP.VerificationCode :
                                          DataHelper.GetOTPFromSMS();

                    }
                }
                while (!isOTPSuccessful && (endTime > DateTime.Now));

                if (!isOTPSuccessful)
                {
                    Reporting.Error("We either never got an OTP, or not the correct one. Verify system config as OTP service may be disabled or throttled.");
                }
                else
                {
                    Reporting.Log("OTP is successful", browser.Driver.TakeSnapshot());
                }
            }
        }
    
        /// <summary>
        /// Checking the static text, field labels, BSB validation and expected refund amount
        /// Enter refund bank details and click on NEXT button
        /// </summary>
        public static void EnterRefundBankDetails(Browser browser, RefundDetails testData, bool detailUiCheck)
        {
            using (var bankAccountDetails = new BankAccountDetails(browser))
            {
                bankAccountDetails.WaitForPage();

                if (detailUiCheck)
                {
                    bankAccountDetails.VerifyDetailsUiCheck();
                    bankAccountDetails.ClickNext();
                    bankAccountDetails.VerifyInputWarningMessages();
                }

                bankAccountDetails.VerifyRefundAmount(testData.RefundAmount);
                bankAccountDetails.EnterBankAccountDetails(testData);
                bankAccountDetails.ClickNext();
            }             
        }

        /// <summary>
        /// Checking the confirmation page for the static text and the 'back to rac homepage' button
        /// </summary>
        public static void VerifyConfirmationPage(Browser browser)
        {
            using (var confirmation = new Confirmation(browser))
            {
                confirmation.WaitForPage();
                confirmation.VerifyActiveStepper();
                confirmation.VerifyUILabels();
            }
        }
    }
}
