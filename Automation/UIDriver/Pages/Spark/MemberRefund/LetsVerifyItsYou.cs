using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UIDriver.Pages.Spark.MemberRefund
{
    public class LetsVerifyItsYou : BaseMemberRefund
    {
        #region CONSTANTS
        private static new class Constants
        {
            public static class Label
            {
                public static readonly string CodeExpiredResendCode = "Didn't get a code or code has expired? Send a new code";
                public static readonly string NoOTPWarningMessage = "Please enter a valid verification code";
                public static readonly string OTPMobileNumberHeading = "^We\'ve sent an SMS with a verification code to mobile \\*{4} \\*{3} \\d{3}\\.\\s*Please enter the code to verify it\'s you\\.$";
                public static readonly string NotYourPhoneNumberWarning = "If this is not your number or you don't have access to this phone, " +
                                                                            "please call us on our dedicated reimbursement line - 1300 657 627.";
            }
        }
        #endregion

        #region XPATHS
        private static new class XPath
        {
            public static readonly string PageHeading = $"//h2[text()=\"Let's verify it's you\"]";

            public static class Label
            {
                public static readonly string OTPMobileNumberHeading = "id('verify-token-subtitle')";
                public static readonly string NoOTPWarningMessage = "//div[@id='verification-code']/p";
                public static readonly string OTPCodeDoesNotMatch = "//p[text()=\"Sorry, that code didn't match. Please try again.\"]";
                public static readonly string CodeExpiredResendCode = "//a[@id='resend-verification-code-link']/..";
                public static readonly string NotYourPhoneNumberWarning = "//a[@id='verify-token-call-us-link']/..";
            }

            public static class Link
            {
                public static readonly string SendANewCode = "id('resend-verification-code-link')";
            }

            public static class Input
            {
                public static readonly string OTPFirstDigit = "id('input-otp-verification-code-0')";
                public static readonly string OTPSecondDigit = "id('input-otp-verification-code-1')";
                public static readonly string OTPThirdDigit = "id('input-otp-verification-code-2')";
                public static readonly string OTPFourthDigit = "id('input-otp-verification-code-3')";
                public static readonly string OTPFifthDigit = "id('input-otp-verification-code-4')";
                public static readonly string OTPSixthDigit = "id('input-otp-verification-code-5')";
            }

            public static class Button
            {
                public static readonly string Verify = "id('verify-token-submit')";
                public const string RequestNewCode = "id('resend-verification-code-link')";
            }
        }
        #endregion

        public LetsVerifyItsYou(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.PageHeading);
                GetElement(XPath.Label.OTPMobileNumberHeading);
                GetElement(XPath.Input.OTPFirstDigit);
                GetElement(XPath.Link.SendANewCode);
                Reporting.Log($"Let's verify it's you page is Displayed");
                GetElement(XPath.Button.Verify);
                Reporting.Log($"Verify buttons is Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Let's verify it's you");
            Reporting.Log($"Page 1: Capture screenshot after confirming page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// Entering the One Time Passcode sent to the member and clicking VERIFY button
        /// </summary>
        /// <param name="oneTimePasscode">One Time Passcode retrieved from the mailosaur inbox to allow the member to authenticate their identity</param>
        /// <returns></returns>
        public bool EnterOTP(string oneTimePasscode)
        {
            char[] otpDigits = oneTimePasscode.ToCharArray();
            Reporting.Log($"Inputting 'OTP Number': {oneTimePasscode}");           
            WaitForTextFieldAndEnterText(XPath.Input.OTPFirstDigit, otpDigits[0].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.Input.OTPSecondDigit, otpDigits[1].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.Input.OTPThirdDigit, otpDigits[2].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.Input.OTPFourthDigit, otpDigits[3].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.Input.OTPFifthDigit, otpDigits[4].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.Input.OTPSixthDigit, otpDigits[5].ToString(), false);
            Reporting.Log("Capturing One Time Password page before selecting VERIFY", _driver.TakeSnapshot());
            ClickVerify();
            return !IsOTPCodeRejected();
        }

        /// <summary>
        /// Verify the error message on clicking Verify button without entering anything.
        /// </summary>
        public void VerifyNoOTPWarning()
        {
            Reporting.AreEqual(Constants.Label.NoOTPWarningMessage, GetInnerText(XPath.Label.NoOTPWarningMessage), 
                $"the expected '{Constants.Label.NoOTPWarningMessage}' warning text matches what's displayed on the page.");
        }

        /// <summary>
        /// Verify the error message on clicking Verify button after entering the wrong passcode.
        /// </summary>
        public void VerifyWrongOTPWarning()
        {
            using (var spinner = new SparkSpinner(_browser))
            {
                spinner.WaitForSpinnerToFinish();
                Reporting.IsTrue(IsControlDisplayed(XPath.Label.OTPCodeDoesNotMatch), $"the 'Sorry, that code didn't match. Please try again' warning text is displayed on the page.");
            }          
        }

        /// <summary>
        /// Select the "Verify" button to attempt to validate the pass code entered and continue.
        /// </summary>
        public void ClickVerify() => ClickControl(XPath.Button.Verify);

        /// <summary>
        /// Waiting for the error message 'Sorry, that code didn't match. Please try again.' to see the OTP code is wrong
        /// </summary>
        public bool IsOTPCodeRejected() =>
           _driver.TryWaitForElementToBeVisible(By.XPath(XPath.Label.OTPCodeDoesNotMatch), Rac.TestAutomation.Common.Constants.General.WaitTimes.T5SEC, out IWebElement element);

        /// <summary>
        /// Click on 'Send a new code' button to request new code
        /// </summary>
        public void RequestNewOTPCode() => ClickControl(XPath.Button.RequestNewCode);

        /// <summary>
        /// Checking the active stepper on the current page
        /// </summary>
        public override void VerifyActiveStepper()
        {
            Reporting.IsFalse(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperEnterRefund).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperEnterRefund} is disabled on the page.");
            Reporting.IsTrue(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperOTP).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperOTP} is enabled on the page.");
            Reporting.IsFalse(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperEnterBankDetails).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperEnterBankDetails} is disabled on the page.");
            Reporting.IsFalse(bool.Parse(GetElement(BaseMemberRefund.XPath.Label.StepperConfirmation).GetAttribute("aria-selected")), $"the stepper {BaseMemberRefund.Constants.Label.StepperConfirmation} is disabled on the page.");
        }

        /// <summary>
        /// Checking the labels on the page
        /// </summary>
        public void VerifyUIFieldLabels()
        {
            var directDebitAuthorisationRegex = new Regex(Constants.Label.OTPMobileNumberHeading);

            Match matchAuthorisation = directDebitAuthorisationRegex.Match(GetInnerText(XPath.Label.OTPMobileNumberHeading));
            Reporting.IsTrue(matchAuthorisation.Success, $"the sub heading is shown as expected");
            Reporting.AreEqual(Constants.Label.CodeExpiredResendCode, GetInnerText(XPath.Label.CodeExpiredResendCode), "the expected 'send a new code' text against the value displayed");
            Reporting.AreEqual(Constants.Label.NotYourPhoneNumberWarning, GetInnerText(XPath.Label.NotYourPhoneNumberWarning), $"the expected '{Constants.Label.NotYourPhoneNumberWarning}' text against the value displayed");
        }

        /// <summary>
        /// Checking all the UI verification for the OTP page
        /// </summary>
        /// <param name="oneTimePasscode">Real OTP code to derive incorrect code from</param>
        public void VerifyDetailUIChecking(string oneTimePasscode)
        {
            // Incrementing by 100 to reduce chance of picking up a OTP
            // from a previous test and accidentally getting a valid one.
            var badOtp = Int32.Parse(oneTimePasscode) + 100;
            using (var spinner = new SparkSpinner(_browser))
            {
                VerifyUIFieldLabels();
                VerifyActiveStepper();
                ClickVerify();
                VerifyNoOTPWarning();
                EnterOTP(badOtp.ToString("D6"));
                spinner.WaitForSpinnerToFinish();
                VerifyWrongOTPWarning();
            }         
        }
     }
}
