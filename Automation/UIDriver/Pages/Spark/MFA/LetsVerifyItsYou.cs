using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Spark
{
    public class LetsVerifyItsYou : SparkBasePage
    {
        #region Constants
        private static class Constants
        {
            public static readonly string Header                       = "Let's verify it's you";
            public static string VerificationSMS(string maskedNumber)  => $"We'll send a verification code to {maskedNumber}.";
            public static string VerificationCall(string maskedNumber) => $"We'll phone you on {maskedNumber} with a verification code.";
            public static class Field
            {
                public static readonly string GetCodeViaPhoneCall          = "Get code via phone call";
                public static readonly string GetCodeViaSMS                = "Send code via SMS";
                public static readonly string NeedHelp                     = "Need help? Visit our FAQs";
                public static readonly string NotYourNumber                = "Not your number? Call 13 17 03";
            }
            public static class Link
            {
                public static readonly string LinkPrefixSIT             = "https://cdvnets.ractest.com.au";
                public static readonly string LinkPrefixUAT             = "https://ractest.com.au";
                public static readonly string FrequentlyAskedQuestions  = "/myrac/help";
                public static readonly string NotYourNumber             = "tel:131703";
            }
            public static class Button
            {
                public static readonly string SendCode                     = "Send code";
            }
        }
        #endregion

        #region XPATHS
        public static class XPath
        {
            public static readonly string Header                       = "id('dialog-title')";
            public static readonly string SendCodeToNumber             = "//div[@data-testid='dialog-content']/p";
            public static class Field
            {
                public static readonly string GetCodeViaPhoneCall          = "id('get-code-via-phone-call-link')";
                public static readonly string GetCodeViaSMS                = "id('send-code-via-sms-link')";
                public static readonly string NeedHelp                     = "id('need-help-faq-label')";
                public static readonly string NotYourNumber                = "id('verify-another-way-phone-number-label')";
            }
            public static class Link
            {
                public static readonly string FrequentlyAskedQuestions     = "id('need-help-faq-link')";
                public static readonly string NotYourNumber                = "id('verify-another-way-phone-number-link')";
            }
            public static class Button
            {
                public static readonly string SendCode                      = "//button[text()='Send code']";
            }
        }
        #endregion

        public override bool IsDisplayed()
        {
            try
            {                
                GetElement(XPath.Button.SendCode);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Let's verify it's you page");
            Reporting.Log("Let's verify it's you page", _driver.TakeSnapshot());
            return true;
        }

        public LetsVerifyItsYou(Browser browser) : base(browser)
        { }

        /// <summary>
        /// Test scenarios with the detailedUiChecking flag set to TRUE go into more detail than 
        /// most of our tests to confirm that copy on the page is correct using methods such as 
        /// this one.
        /// 
        /// First we select the 'Get code via phone call' element so the modal is updated to display 
        /// the 'Get code via SMS' element and the "We'll phone you..." verification code text.
        /// Then we select the 'Get code via SMS' element to return to the default state and evaluate 
        /// most of the rest of the UI elements on this modal.
        /// 
        /// The exception is the "We'll send a verification code to..." text, as this is already 
        /// verified in the VerifyAndSendCode method regardless of the detailedUiChecking flag.
        /// </summary>
        /// <param name="mobileNumber">Telephone number belonging to the logged in member which we'll use to provide a One Time Password</param>
        public void DetailedUiChecking(string mobileNumber)
        {
            var maskMobileNumber = DataHelper.MaskPhoneNumber(mobileNumber);
            Reporting.AreEqual(Constants.VerificationSMS(maskMobileNumber), GetInnerText(XPath.SendCodeToNumber),
                "'We'll send a verification code' text with masked phone number");
            Reporting.Log($"Clicking the '{Constants.Field.GetCodeViaPhoneCall}' element so we can verify the " +
                $"'{Constants.Field.GetCodeViaSMS}' element.");
            ClickControl(XPath.Field.GetCodeViaPhoneCall);
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Field.GetCodeViaSMS), WaitTimes.T5SEC);
            Reporting.AreEqual(Constants.VerificationCall(maskMobileNumber), GetInnerText(XPath.SendCodeToNumber), 
                "'We'll phone you' text with masked phone number");
            Reporting.AreEqual(Constants.Field.GetCodeViaSMS, GetInnerText(XPath.Field.GetCodeViaSMS), "Get code via SMS text");
            
            Reporting.Log($"Capturing screenshot of 'We'll phone you' elements before clicking the '{Constants.Field.GetCodeViaSMS}' " +
                $"element so to return to default and evaluate remaining elements", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Field.GetCodeViaSMS);
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Field.GetCodeViaPhoneCall), WaitTimes.T5SEC);
            Reporting.AreEqual(Constants.Header, GetInnerText(XPath.Header), "Let's verify it's you header text");
            Reporting.AreEqual(Constants.Field.GetCodeViaPhoneCall, GetInnerText(XPath.Field.GetCodeViaPhoneCall), 
                "Get code via phone call link text");
            Reporting.AreEqual(Constants.Field.NeedHelp, GetInnerText(XPath.Field.NeedHelp), "'Need help?' text");
            Reporting.AreEqual(Constants.Field.NotYourNumber, GetInnerText(XPath.Field.NotYourNumber), "'Not your number?' text");
            Reporting.AreEqual(Constants.Button.SendCode, GetInnerText(XPath.Button.SendCode), "send code button text");
            Reporting.AreEqual(Constants.Link.NotYourNumber, 
                GetElement(XPath.Link.NotYourNumber).GetAttribute("href"), "'call 13 17 03' URL matches expected value");
            
            CheckFAQLinkForEnvironment();
        }

        public void CheckFAQLinkForEnvironment()
        {
            var config = Config.Get();
            bool isUatEnvironment = config.Shield.IsUatEnvironment();
            
            if (isUatEnvironment)
            {
                Reporting.AreEqual(Constants.Link.LinkPrefixUAT + Constants.Link.FrequentlyAskedQuestions,
                    GetElement(XPath.Link.FrequentlyAskedQuestions).GetAttribute("href"), "frequently asked questions URL matches expected value for UAT");
            }
            else
            {
                Reporting.AreEqual(Constants.Link.LinkPrefixSIT + Constants.Link.FrequentlyAskedQuestions,
                    GetElement(XPath.Link.FrequentlyAskedQuestions).GetAttribute("href"), "frequently asked questions URL matches expected value for SIT");
            }
        }

        public void VerifyAndSendCode(string mobileNumber)
        {
            var maskMobileNumber = DataHelper.MaskPhoneNumber(mobileNumber);
            
            ClickControl(XPath.Button.SendCode);
        }
    }
}
