using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Spark
{
    public class EnterVerificationCode : SparkBasePage
    {
        #region Constants
        private static class Constants
        {
            public static readonly string Header              = "Enter verification code";
            public static string SentSMS(string maskedNumber) => $"We've sent an SMS to {maskedNumber}. Please enter the code to verify it's you.";
            public static class Field
            {
                public static readonly string SendNewCode         = "Send new code";
                public static readonly string GetCodeViaPhoneCall = "Get code via phone call";
                public static readonly string GetCodeViaSMS       = "Send code via SMS";
                public static readonly string NeedHelp            = "Need help? Visit our FAQs";
                public static readonly string NotYourNumber       = "Not your number? Call 13 17 03";
            }
            public static class Button
            {
                public static readonly string Verify              = "Verify";
            }
            public static class Warning
            {
                public static readonly string OTPCodeDoesNotMatch = "Sorry, that code doesn't match. Please try again or request a new code.";
                public static readonly string OTPCodeHasExpired   = "Sorry, that code has expired. Please request a new code.";
            }
        }
        #endregion

        #region XPATHS
        public static class XPath
        {
            public static readonly string Header              = "id('dialog-title')";
            public static readonly string VerificationCode    = "//*[text()='to verify'] | //*[@id='verificationCode-sms-label']";

            public static class OTPInput
            {
                public static readonly string FirstDigit  = "//*[@id='input-otp-verificationCode-otp-0'] | //*[@id='input-otp-0']";
                public static readonly string SecondDigit = "//*[@id='input-otp-verificationCode-otp-1'] | //*[@id='input-otp-1']";
                public static readonly string ThirdDigit  = "//*[@id='input-otp-verificationCode-otp-2'] | //*[@id='input-otp-2']";
                public static readonly string FourthDigit = "//*[@id='input-otp-verificationCode-otp-3'] | //*[@id='input-otp-3']";
                public static readonly string FifthDigit  = "//*[@id='input-otp-verificationCode-otp-4'] | //*[@id='input-otp-4']";
                public static readonly string SixthDigit  = "//*[@id='input-otp-verificationCode-otp-5'] | //*[@id='input-otp-5']";
            }
            public static class Field
            {
                public static readonly string SendNewCode         = "//*[@id='send-new-code-link']                   | //*[contains(text(), 'Send new code')]";
                public static readonly string GetCodeViaPhoneCall = "//*[id='get-code-via-phone-call-link']          | //*[contains(text(), 'Get code via phone call')]";
                public static readonly string GetCodeViaSMS       = "//*[id='send-code-via-sms-link']                | //*[contains(text(), 'Send code via SMS')]";
                public static readonly string NeedHelp            = "//*[id='need-help-faq-label']                   | //*[contains(text(), 'Need help?')]";
                public static readonly string NotYourNumber       = "//*[id='verify-another-way-phone-number-label'] | //*[contains(text(), 'Not your number?')]";
            }
            public static class Button
            {
                public static readonly string Verify              = "//*[@id='verify-button' or @id='otp-verify-button-verify-initial']";
            }
            public static class Warning
            {
                public static readonly string OTPCodeDoesNotMatch = "//p[contains(text(),'Please try again or request')]";
                public static readonly string OTPCodeHasExpired   = "//p[contains(text(),'Sorry, that code has expired.')]";
            }
        }
        #endregion


        public override bool IsDisplayed()
        {
            try
            {               
                GetElement(XPath.Button.Verify);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Enter verification code page");
            Reporting.Log("Enter verification code page", _driver.TakeSnapshot());
            return true;
        }

        public EnterVerificationCode(Browser browser) : base(browser)
        { }

        /// <summary>
        /// Test scenarios with the detailedUiChecking flag set to TRUE go into more detail than 
        /// most of our tests to confirm that copy on the page is correct using methods such as 
        /// this one.
        /// 
        /// Unlike the first stage of MFA, we don't check the elements displayed for a code 
        /// provided via telephone call. Selecting the "Get code via phone call" link would
        /// reset us back to the "Let's verify it's you" stage.
        /// 
        /// We also don't check the "We've sent an SMS to..." text, as this is already verified in the 
        /// EnterOneTimePasscodeAndVerify method regardless of the detailedUiChecking flag.
        /// </summary>
        public void DetailedUiChecking()
        {
            Reporting.AreEqual(Constants.Header, GetInnerText(XPath.Header), "Let's verify it's you header text");
            Reporting.AreEqual(Constants.Field.SendNewCode, GetInnerText(XPath.Field.SendNewCode), "Send new code");
            Reporting.AreEqual(Constants.Field.GetCodeViaPhoneCall, GetInnerText(XPath.Field.GetCodeViaPhoneCall), 
                "Get code via phone call link text");
            Reporting.AreEqual(Constants.Field.NeedHelp, GetInnerText(XPath.Field.NeedHelp), "'Need help?' text");
            Reporting.AreEqual(Constants.Field.NotYourNumber, GetInnerText(XPath.Field.NotYourNumber), "'Not your number?' text");
            Reporting.AreEqual(Constants.Button.Verify, GetInnerText(XPath.Button.Verify), "submit code button text");
        }

        public void EnterOneTimePasscodeAndVerify(string oneTimePasscode, string mobileNumber, bool detailUiChecking = false)
        {
            if (detailUiChecking)
            {
                Reporting.AreEqual(Constants.SentSMS(DataHelper.MaskPhoneNumber(mobileNumber)), GetInnerText(XPath.VerificationCode),
                "Sent verification code text");
            }
            char[] otpDigits = oneTimePasscode.ToCharArray();
            WaitForTextFieldAndEnterText(XPath.OTPInput.FirstDigit, otpDigits[0].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.OTPInput.SecondDigit, otpDigits[1].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.OTPInput.ThirdDigit, otpDigits[2].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.OTPInput.FourthDigit, otpDigits[3].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.OTPInput.FifthDigit, otpDigits[4].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.OTPInput.SixthDigit, otpDigits[5].ToString(), false);
            Reporting.Log("Enter verification code page before clicking Verify", _driver.TakeSnapshot());
            ClickControl(XPath.Button.Verify);
        }

        public void VerifyOTPErrorMessage()
        {
            var displayed = _driver.TryWaitForElementToBeVisible(By.XPath(XPath.Warning.OTPCodeDoesNotMatch), WaitTimes.T5SEC, out IWebElement element);
            if (displayed)
            {
                Reporting.AreEqual(Constants.Warning.OTPCodeDoesNotMatch, GetInnerText(XPath.Warning.OTPCodeDoesNotMatch), 
                    "Error message when OTP input is invalid");
                Reporting.Log("Capturing snapshot", _driver.TakeSnapshot());
            }
            else
            {
                Reporting.Error("Verification code does not match error message not found", _driver.TakeSnapshot());
            }
        }

        public void RequestNewOTPCode()
        {
            Reporting.Log($"Requesting a new OTP verification code.");
            ClickControl(XPath.Field.SendNewCode);
        }
    }
}
