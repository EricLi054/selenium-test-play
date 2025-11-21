using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.MyRACRegisterAnAccount
{
    public class MyRACRegisterAnAccount : SparkBasePage
    {
        #region Constants
        public class Constants
        {
            public class Headings
            {
                public static readonly string AlmostThere = "Almost there, lets find your existing products and policies...";
                public static readonly string YourDetails = "We'll use your details to look up your products and policies";
            }
            public class FieldLabels
            {
                public static readonly string FirstName            = "First name";
                public static readonly string LastName             = "Last name";
                public static readonly string DateOfBirth          = "Date of birth";
                public static readonly string MobileNumber         = "Mobile number";
                public static readonly string MemberOrPolicyNumber = "Membership / policy number";
            }
            public class Buttons
            {
                public static readonly string FindMyDetails = "Find my details";
                public static readonly string Exit          = "Exit";
            }
        }
        #endregion
        #region XPATHS
        public class XPath
        {
            public class Headings
            {
                public static readonly string AlmostThere = "//div[@id='pnl-ProductLinkForm']/h3";
                public static readonly string YourDetails = "//div[@id='pnl-ProductLinkForm']/h5";
            }
            public class Fields
            {
                public static readonly string EmailAddress          = "id('emailAddress')";
                public static readonly string VerificationCode      = "id('verificationCode')";
                public static readonly string NewPassword           = "id('newPassword')";
                public static readonly string ReEnterPassword       = "id('reenterPassword')";
                public static readonly string FirstName             = "id('FirstName')";
                public static readonly string LastName              = "id('LastName')";
                public static readonly string DateOfBirth           = "id('DateOfBirth')";
                public static readonly string MobileNumber          = "id('MobileNumber')";
                public static readonly string MemberOrPolicy        = "id('MembershipNumber')";
                public class Labels
                {
                    public static readonly string First                 = "id('lblFirstName')";
                    public static readonly string Last                  = "id('lblLastName')";
                    public static readonly string BirthDate             = "id('lblDateOfBirth')";
                    public static readonly string Mobile                = "id('lblMobileNumber')";
                    public static readonly string MemberOrPolicyNumber  = "id('lblMembershipNumber')";
                }
            }
            public class Buttons
            {
                public static readonly string RegisterNow   = "id('createAccount')";
                public static readonly string SendCode      = "id('emailVerificationControl_but_send_code')";
                public static readonly string ReSendCode    = "id('emailVerificationControl_but_change_claims')";
                public static readonly string VerifyCode    = "id('emailVerificationControl_but_verify_code')";
                public static readonly string RegisterPwd   = "id('continue')";
                public static readonly string TermsOfUse    = "id('extension_TermsOfUse_True')";
                public static readonly string FindMyDetails = "//button[@id='submitForLookup']";
                public static readonly string Exit          = "//a[@data-ng-click='logout()']";
            }
        }
        #endregion
        public MyRACRegisterAnAccount(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                isDisplayed = string.Equals(Constants.Headings.AlmostThere, GetInnerText(XPath.Headings.AlmostThere));
                GetElement(XPath.Buttons.FindMyDetails);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            if (isDisplayed)
            {
                Reporting.LogPageChange("myRAC Login page");
            }
            return isDisplayed;
        }

        /// <summary>
        /// Captures the headings and other copy displayed on the myRAC Login page and 
        /// compares with the expected values.
        /// </summary>
        public void VerifyDetailedContent(ClaimHome claimHome)
        {
            //Maybe not though?
        }

        /// <summary>
        /// This method orchestrates: 
        /// - fetching the verification code from myRAC and inputting that code
        /// - inputting and verifying a password and accepting the terms of use
        /// - inputting member information to match this myRAC account with their Member Central records
        /// </summary>
        /// <param name="contactDetails">Contact object referred to for a LoginEmail value and later in the 'Find my details' step</param>
        /// <param name="policyNumber">Policy number selected for this test which may be used in the 'Find my details' step</param>
        /// <param name="password">The password which will be allocated to this myRAC account</param>
        public void RegisterAccount(Contact contactDetails, string policyNumber, string password)
        {
            ClickControl(XPath.Buttons.RegisterNow);
            Reporting.Log($"Capturing the page on arrival.", _browser.Driver.TakeSnapshot());
            MyRACVerificationCode(contactDetails.LoginEmail);

            SetMyRACPassword(password);

            MyRACFindMyDetails(contactDetails.FirstName, contactDetails.Surname, contactDetails.DateOfBirth.ToString("ddMMyyyy"), contactDetails.MobilePhoneNumber, policyNumber);

            ClickControl(XPath.Buttons.FindMyDetails);
        }

        /// <summary>
        /// Submit an email address to receive a verification code for myRAC account registration.
        /// </summary>
        /// <param name="loginEmail">The email generated to use in retrieving a verification code from mailosaur</param>
        public void MyRACVerificationCode(string loginEmail)
        {
            string oneTimePasscode = null;
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Fields.EmailAddress), WaitTimes.T10SEC);
            WaitForTextFieldAndEnterText(XPath.Fields.EmailAddress, loginEmail, hasTypeAhead: false);
            Reporting.Log($"Capturing the before submitting email '{loginEmail}' for verification code.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Buttons.SendCode);

            _driver.WaitForElementToBeVisible(By.XPath(XPath.Fields.VerificationCode), WaitTimes.T30SEC);

            Reporting.Log($"Capturing screen while attempting to fetch verification code from mailosaur.", _browser.Driver.TakeSnapshot());
            oneTimePasscode = DataHelper.GetMyRACRegistrationCodeFromEmail(loginEmail);

            if (oneTimePasscode == null)
            {
                Reporting.Error("Email was found but passcode returned was null, indicating it was not found in the email");
            }
            else
            {
                _driver.WaitForElementToBeVisible(By.XPath(XPath.Fields.VerificationCode), WaitTimes.T5SEC);
                WaitForTextFieldAndEnterText(XPath.Fields.VerificationCode, oneTimePasscode, hasTypeAhead: false);
                Reporting.Log($"Capturing the screen after entering verification code '{oneTimePasscode}'.", _browser.Driver.TakeSnapshot());
                ClickControl(XPath.Buttons.VerifyCode);
            }
        }

        /// <summary>
        /// Provide and verify a password for this myRAC account, and check the terms of use box before continuing.
        /// </summary>
        /// <param name="password">The password to be linked to this myRAC account</param>
        public void SetMyRACPassword(string password)
        {
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Fields.ReEnterPassword), WaitTimes.T30SEC);

            WaitForTextFieldAndEnterText(XPath.Fields.NewPassword, password, hasTypeAhead: false);
            WaitForTextFieldAndEnterText(XPath.Fields.ReEnterPassword, password, hasTypeAhead: false);

            ClickControl(XPath.Buttons.TermsOfUse);
            Reporting.Log($"Capturing the screen having accepted terms of use to submit password registration.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Buttons.RegisterPwd);
        }

        /// <summary>
        /// Provide information about the member to link their Member Central records with their myRAC account.
        /// 
        /// At time of writing, expectation is that a member without a mobile telephone contact number would 
        /// never be included in a test invoking this flow, but if so a policy number could be provided instead.
        /// </summary>
        /// <param name="firstName">Member first name</param>
        /// <param name="lastName">Member surname</param>
        /// <param name="dateOfBirthddMMyyyy">Member date of Birth in the format 'ddMMyyyy'</param>
        /// <param name="mobileNumber">Member's mobile telephone number</param>
        /// <param name="policyNumber">Policy number for the member - only used if Mobile telephone is unavailable</param>
        public void MyRACFindMyDetails(string firstName, string lastName, string dateOfBirthddMMyyyy, string mobileNumber = null, string policyNumber = null)
        {
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Fields.MemberOrPolicy), WaitTimes.T150SEC);
            WaitForTextFieldAndEnterText(XPath.Fields.FirstName, firstName);
            WaitForTextFieldAndEnterText(XPath.Fields.LastName, lastName);
            Reporting.Log($"Capturing member name inputs", _browser.Driver.TakeSnapshot());
            MyRACDateOfBirthEntry(XPath.Fields.DateOfBirth, dateOfBirthddMMyyyy);

            if (!string.IsNullOrEmpty(mobileNumber))
            {
                WaitForTextFieldAndEnterText(XPath.Fields.MobileNumber, mobileNumber);
            }
            else if (!string.IsNullOrEmpty(policyNumber))
            {
                WaitForTextFieldAndEnterText(XPath.Fields.MemberOrPolicy, policyNumber);
            }
            else
            {
                Reporting.Error($"Both Mobile telephone number and Policy number were null or empty, unable to proceed with " +
                    $"myRAC account registration.");
            }
        }
    }
}