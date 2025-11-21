using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.General;


namespace UIDriver.Pages.MyRAC
{
    public class MyRACLogin : SparkBasePage
    {
        #region Constants
        public class Constants
        {
            public class Headings
            {
                public static readonly string CredentialsPanel = "Log in to RAC WA";
            }
            public class FieldLabels
            {
                public static readonly string Email = "Email";
                public static readonly string Password = "Password";
            }
            public class Buttons
            {
                public static readonly string EditDetails = "Log in";
                public static readonly string Register = "Register now";
                public static readonly string ForgotPassword = "Forgot your password?";
            }
            public class AdviseUser
            {
                public static readonly string InvalidCredentials = "The email or password provided is invalid.";
            }
        }
        #endregion
        #region XPATHS
        public class XPath
        {
            public class Headings
            {
                public static readonly string CredentialsPanel = "//div[@class='sign-up-step']/h2";
            }
            public class Fields
            {
                public static readonly string Email     = "//input[@id='signInName']";
                public static readonly string Password  = "//input[@id='password']";
            }
            public class Buttons
            {
                public static readonly string LogIn             = "//button[@id='next']";
                public static readonly string ForgotPassword    = "//a[@id='forgotPassword']";
                public static readonly string Register          = "//a[@id='createAccount']";
                public static readonly string UserMenu          = "//button[@aria-label='user-menu-toggle']";
                public static readonly string LogOut            = "//button[normalize-space()='Log out']";
            }
            public class AdviseUser
            {
                public static readonly string InvalidCredentials = "//p[contains(text(), 'provided is invalid.')]";
            }
        }
        #endregion
        public MyRACLogin(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                isDisplayed = string.Equals(Constants.Headings.CredentialsPanel, GetInnerText(XPath.Headings.CredentialsPanel));
                GetElement(XPath.Buttons.Register);
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

        public void InputCredentials(string email, string password, bool detailedUiChecking = false)
        {
            WaitForTextFieldAndEnterText(XPath.Fields.Email, email, false);

            // MS does some fancy forms transitions and that trips up Selenium
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Fields.Password), WaitTimes.T30SEC);
            WaitForTextFieldAndEnterText(XPath.Fields.Password, password, false);
        }

        public void SubmitCredentials()
        {
            Reporting.Log($"Capturing the state of this page before submitting credentials", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Buttons.LogIn);
        }

        /// <summary>
        /// Check for the invalid credentials error message on the page. 
        /// If true this indicates that we need to attempt to register a myRAC account.
        /// </summary>
        /// <returns>True if the invalid credentials error is detected, false otherwise.</returns>
        public bool DetectUnsuccessfulLogin()
        {
            IWebElement invalidCredentialsError = null;
            bool visibleError = _driver.TryWaitForElementToBeVisible(By.XPath(XPath.AdviseUser.InvalidCredentials), WaitTimes.T10SEC, out invalidCredentialsError);
            
            if (visibleError)
            {
                Reporting.Log($"Invalid credentials error detected, capturing the state of this page after failing to login.", _browser.Driver.TakeSnapshot());
                Reporting.AreEqual(Constants.AdviseUser.InvalidCredentials, GetInnerText(XPath.AdviseUser.InvalidCredentials),
                    "expected copy for invalid credentials error with the value displayed");
            }
            else
            {
                Reporting.Log($"Invalid credentials error not detected, will attempt to verify successful login.");
            }
            return visibleError;
        }

        /// <summary>
        /// Confirm that we have reached the myRAC landing page and check whether the policy card for the policy number is displayed
        /// with a 'Make a claim' button.
        /// 
        /// If there's not a 'Make a claim' button, we select the 'Manage' button and look for a control there. 
        /// </summary>
        /// <param name="policyNumber">The policynumber to claim against, which we use to build expected URLs</param>
        public void VerifySuccessfulLogin(string policyNumber)
        {
            string policyCard = $"//span[text()='{policyNumber}']";
            string makeClaimForPolicyXPath = $"//a[contains(@href, '{policyNumber}')]";
            string managePolicyXPath = $"//div[contains(@class, 'MuiGrid-root') and .//span[text()='{policyNumber}']]//button[contains(@class, 'MuiButton-containedInfo') and text()='Manage']";

            //The myRAC landing page with policy cards can be pretty slow to load so we have to be generous here.
            _driver.WaitForElementToBeVisible(By.XPath(policyCard), WaitTimes.T150SEC); 
            Reporting.Log($"Capturing the state of the page.", _browser.Driver.TakeSnapshot());

            ScrollElementIntoView(policyCard);
            Reporting.Log($"Capturing snapshot after scrolling the policy card for {policyNumber} into view", _browser.Driver.TakeSnapshot());

            bool makeClaimButton = _driver.TryWaitForElementToBeVisible(By.XPath(makeClaimForPolicyXPath), WaitTimes.T5SEC, out IWebElement policyCardElement);
            if (makeClaimButton)
            {
                Reporting.AreEqual("Make a claim", GetInnerText(makeClaimForPolicyXPath),
                    $"make a claim button exists for the policy selected on this test ('{policyNumber}') with expected text ");
            }
            else
            {
                ClickControl(managePolicyXPath);
                Reporting.Log($"The 'Make a claim' button for policy '{policyNumber}' was not found on the page. Checking for " +
                    $"'Make another claim' under the 'Manage' button.", _browser.Driver.TakeSnapshot());
                makeClaimButton = _driver.TryWaitForElementToBeVisible(By.XPath(makeClaimForPolicyXPath), WaitTimes.T5SEC, out IWebElement managePolicyMakeClaim);
                if (makeClaimButton)
                {
                    Reporting.AreEqual("Make another claim", GetInnerText(makeClaimForPolicyXPath),
                        $"make another claim button exists for the policy selected on this test ('{policyNumber}') with expected text ");
                }
                else
                {
                    Reporting.Error($"The 'Make another claim' button for policy '{policyNumber}' was not found on the page. Please investigate.");
                }
            }
        }

        public void ReportHomeStormClaimFromMyRAC(string policyNumber, ClaimHome claim)
        {
            Reporting.LogMinorSectionHeading($"Attempting to report claim from myRAC policy card");
            string makeClaimForPolicyXPath = $"//a[contains(@href, '{policyNumber}')]";

            string makeAClaimUrl = GetElement(makeClaimForPolicyXPath).GetAttribute("href");
            bool bypassReportViaMyRAC = false;

            //TODO DED-519 - When this issue is resolved we can remove the known bad URL check and partial logging below
            if (makeAClaimUrl == $"https://racwa-dcc-web-b2c-int2.azureapp.ractest.com.au/claims/home/building-and-contents?policyNumber={policyNumber}")
            {
                Reporting.LogMinorSectionHeading($"URL for 'Make a claim' button for {policyNumber} is '{makeAClaimUrl}' which will not work " +
                    $"(see <a href=\"https://rac-wa.atlassian.net/browse/DED-519\">DED-519</a>) so instead we will open the Home Claim Triage page as if " +
                    $"using the Test Launch page.");
                bypassReportViaMyRAC = true;
                LaunchPage.OpenHomeTriageClaimURL(_browser, claim);
            }
            else if (makeAClaimUrl == $"https://ctr-sit.racinsurance.ractest.com.au/claims/home/building-and-contents?policyNumber={policyNumber}")
            {
                Reporting.Log($"URL for 'Make a claim' button for {policyNumber} is '{makeAClaimUrl}' which appears to be valid for " +
                    $"SIT Home Triage page, clicking the button." +
                    $"<b>NOTE: This implies that <a href=\"https://rac-wa.atlassian.net/browse/DED-519\">DED-519</a> has been resolved, " +
                    $"and we should remove code related to that issue including this part of the log message.</b>");
            }
            else if (makeAClaimUrl == $"https://b2cuat6.ractest.com.au/claims/home/building-and-contents?policyNumber={policyNumber}")
            {
                Reporting.Log($"URL for 'Make a claim' button for {policyNumber} is '{makeAClaimUrl}' which appears to be valid for UAT Home Triage page, clicking the button.");
            }
            else
            {
                Reporting.Error($"The URL on the 'Make a claim' button for {policyNumber} is '{makeAClaimUrl}' which is not one of the options accounted for in our test framework. Please investigate.");
            }
            
            if (!bypassReportViaMyRAC)
            {
                ClickControl(makeClaimForPolicyXPath);
            }
        }

        public void LogOutMyRAC(Browser browser)
            {
            Reporting.LogMinorSectionHeading($"Opening main myRAC page to log out");

            string environmentMyRACURL = null;
            var environment = Config.Get().Shield.Environment;

            if (environment == "shieldint2")
            {
                Reporting.Log($"Setting login URL for {environment}.");
                environmentMyRACURL = MyRACURLs.LoginSIT;
            }
            else if (environment == "shielduat6")
            {
                Reporting.Log($"Setting login URL for {environment}.");
                environmentMyRACURL = MyRACURLs.LoginUAT;
            }
            else
            {
                Reporting.Error($"Environment {environment} is not supported for this test, " +
                    $"must be integrated with Member Central NPE.");
            }

            browser.OpenUrl(environmentMyRACURL);

            bool UserMenuAvailable = _driver.TryWaitForElementToBeVisible(By.XPath(XPath.Buttons.UserMenu), WaitTimes.T60SEC, out IWebElement userMenu);

            if (UserMenuAvailable)
            {
                ClickControl(XPath.Buttons.UserMenu);
                Reporting.Log($"Capturing page after activating user menu but before selecting 'Log out' button.", browser.Driver.TakeSnapshot());
                ClickControl(XPath.Buttons.LogOut);
            }
            else
            {
                Reporting.Error($"User menu button was not found on the page, unable to log out of myRAC.");
            }
        }
    }
}
