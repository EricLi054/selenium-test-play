using Rac.TestAutomation.Common;
using System;
using UIDriver.Pages.Spark;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace Tests.ActionsAndValidations
{
    public static class ActionMFA
    {
        /// <summary>
        /// This method either fetches the OTP from mailosaur SMS, or uses the default OTP override code if the IsBypassOTPEnabled() 
        /// feature toggle is enabled.
        /// 
        /// If isMyRACChangeDetails is TRUE, then retryOTP and detailUiChecking won't count in this method as we don't aim to cover 
        /// that level of detail in myRAC change details.
        /// 
        /// Note also that isBypassOTPEnabled does not work in the myRAC MFA check and so isMyRACChangeDetails will ensure a live 
        /// NPE OTP is fetched. Not that we expect that toggle on often in INT2 or UAT6.
        /// </summary>
        /// <param name="mobileNumber">The mailosaur mobile telephone number we will send the OTP code to for MFA</param>
        /// <param name="retryOTP">If TRUE then we intentionally provide an invalid code at first, validating the error message before correcting the code</param>
        /// <param name="detailUiChecking">Optional parameter; if TRUE we check UI elements like field labels etc rather than focusing only on bare functional test actions</param>
        /// <param name="isMyRACChangeDetails">Optional parameter; must be FALSE for retryOTP or detailUiChecking to count as TRUE</param>
        public static void RequestAndEnterOTP(Browser browser, string mobileNumber, bool retryOTP = false, bool detailUiChecking = false, bool isMyRACChangeDetails = false)
        {
            using (var letsVerifyItsYou = new LetsVerifyItsYou(browser))
            using (var enterOTP = new EnterVerificationCode(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish();
                letsVerifyItsYou.WaitForPage();
                if (detailUiChecking && !isMyRACChangeDetails)
                {
                    letsVerifyItsYou.DetailedUiChecking(mobileNumber);
                }
                letsVerifyItsYou.VerifyAndSendCode(mobileNumber);
                spinner.WaitForSpinnerToFinish();

                enterOTP.WaitForPage();
                if (detailUiChecking && !isMyRACChangeDetails)
                {
                    enterOTP.DetailedUiChecking();
                }
                ///If the IsBypassOTPEnabled() feature toggle is enabled then we apply a default code. 
                ///If the toggle is not enabled then we retrieve the OTP from mailosaur.
                string oneTimePasscode = null;
                bool isBypassOTPEnabled = Config.Get().IsBypassOTPEnabled();
                if (isBypassOTPEnabled && !isMyRACChangeDetails)
                {
                    Reporting.Log($"Setting OTP to '{MultiFactorAuthentication.BypassOTP.VerificationCode}' as BypassOTPEnabled is '{isBypassOTPEnabled}' and isMyRACChangeDetails is '{isMyRACChangeDetails}'");
                    oneTimePasscode = MultiFactorAuthentication.BypassOTP.VerificationCode;
                }
                else
                {
                    Reporting.Log($"Retrieving OTP from mailosaur inbox. BypassOTPEnabled is '{isBypassOTPEnabled}' and isMyRACChangeDetails is '{isMyRACChangeDetails}'");
                    oneTimePasscode = DataHelper.GetOTPFromSMS();
                }
                
                if (retryOTP && !isMyRACChangeDetails)
                {
                    Reporting.Log($"Test scenario has retryOTP flagged to input incorrect OTP so we can validate error message; " +
                        $"adding {MultiFactorAuthentication.IncorrectOTP.AddToCurrentOTP} to actual OTP code.");
                    var invalidOTP = Int32.Parse(oneTimePasscode) + MultiFactorAuthentication.IncorrectOTP.AddToCurrentOTP;
                    enterOTP.EnterOneTimePasscodeAndVerify(invalidOTP.ToString("D6"), mobileNumber);
                    spinner.WaitForSpinnerToFinish();
                    enterOTP.VerifyOTPErrorMessage();
                }
                enterOTP.EnterOneTimePasscodeAndVerify(oneTimePasscode, mobileNumber);
                spinner.WaitForSpinnerToFinish(WaitTimes.T150SEC);
            }
        }
       
    }
}
