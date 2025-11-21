using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.EFT
{
    public class OneTimePasscodeScreen: SparkBasePage
    {
        #region XPATHS
        private static class XPath
        {
            public static class OTPInput
            {
                public const string FirstDigit = "//input[@aria-label='One time pass code input box 1']";
                public const string SecondDigit = "//input[@aria-label='One time pass code input box 2']";
                public const string ThirdDigit = "//input[@aria-label='One time pass code input box 3']";
                public const string FourthDigit = "//input[@aria-label='One time pass code input box 4']";
                public const string FifthDigit = "//input[@aria-label='One time pass code input box 5']";
                public const string SixthDigit = "//input[@aria-label='One time pass code input box 6']";
            }

            public static class Warning
            {
                public const string OTPCodeDoesNotMatch = "//p[text()=\"Sorry, that code didn't match. Please try again\"]";
                public const string OTPCodeExpired      = "//p[text()=\"Sorry, your code has expired. Please request a new code.\"]";
            }

            public static class Button
            {
                public const string Verify = "//button[@data-testid='submit']";
                public const string RequestNewCode = "//a[@data-testid='sms-send-code-button']";
            }

        }
        #endregion XPATHS

        public OneTimePasscodeScreen(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.OTPInput.FirstDigit);
                GetElement(XPath.Button.Verify);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Inputs the One Time Password and selects the Verify button.
        /// Checks for known validation error messages so we have the 
        /// option to try again.
        /// </summary>
        /// <param name="oneTimePasscode"></param>
        /// <returns>isOTPSuccessful (In so far as we did not detect any known OTP errors which is why we return !knownOTPError)</returns>
        public bool EnterOneTimePasscodeAndVerify(string oneTimePasscode)
        {
            bool knownOTPError = false;
            char[] otpDigits = oneTimePasscode.ToCharArray();
            WaitForTextFieldAndEnterText(XPath.OTPInput.FirstDigit, otpDigits[0].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.OTPInput.SecondDigit, otpDigits[1].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.OTPInput.ThirdDigit, otpDigits[2].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.OTPInput.FourthDigit, otpDigits[3].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.OTPInput.FifthDigit, otpDigits[4].ToString(), false);
            WaitForTextFieldAndEnterText(XPath.OTPInput.SixthDigit, otpDigits[5].ToString(), false);
            Reporting.Log("Capturing OTP Page before pressing Verify", _driver.TakeSnapshot());
            ClickControl(XPath.Button.Verify);

            if (IsOTPCodeRejected() ||
                IsOTPCodeExpired())
            {
                Reporting.Log($"OTP not accepted, error returned.", _driver.TakeSnapshot());
                knownOTPError = true;
            }
            else
            {
                Reporting.Log($"No known OTP errors were detected");
            }

            return !knownOTPError;
        }

        public bool IsOTPCodeRejected() =>
            _driver.TryWaitForElementToBeVisible(By.XPath(XPath.Warning.OTPCodeDoesNotMatch), Constants.General.WaitTimes.T5SEC, out IWebElement element);

        public bool IsOTPCodeExpired() =>
            _driver.TryWaitForElementToBeVisible(By.XPath(XPath.Warning.OTPCodeExpired), Constants.General.WaitTimes.T5SEC, out IWebElement element);

        public void RequestNewOTPCode() => ClickControl(XPath.Button.RequestNewCode);
    }
}
