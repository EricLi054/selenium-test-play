using Rac.TestAutomation.Common;
using System;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.General;
using UIDriver.Pages.Spark;

namespace UIDriver.Pages.MicrosoftAD
{
    public class Authentication : BasePage
    {
        private class Constants
        {
            public const int MinValidADPasswordLength = 8;
            public const string ADUserDomain = "@rac.com.au";
        }

        private class XPath
        {
            public class Login
            {
                public class Field
                {
                    public const string Email = "//input[(@type='email') and (@name='loginfmt')]";
                    public const string Password = "//input[(@type='password') and (@name='passwd')]";
                }
                public class Button
                {
                    public const string SignIn = "//input[@type='submit']";
                }
            }
            public class Heading
            {
                public const string StaySignedIn = "//div[(@role='heading') and (text()='Stay signed in?')]";
            }
        }

        public Authentication(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                if (_driver.Url.Contains("login.microsoftonline.com"))
                {
                    IWebElement username = null;
                    IWebElement password = null;
                    IWebElement signInPrompt = null;
                    if (_driver.TryFindElement(By.XPath(XPath.Login.Field.Email), out username) ||
                        _driver.TryFindElement(By.XPath(XPath.Login.Field.Password), out password) ||
                        _driver.TryFindElement(By.XPath(XPath.Login.Button.SignIn), out signInPrompt))
                        isDisplayed = true;
                }
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// For Insurance applications which are behind MS AAD protection, then 
        /// a user might get presented with a MS AD login view when trying to
        /// run their test.
        /// 
        /// This method will attempt to navigate that prompt if it appears.
        /// </summary>
        /// <exception cref="Exception">If failed to navigate AAD or parameters are invalid.</exception>
        public void WaitForADLoginPageOrDesiredLandingPage(BasePage nextPage)
        {
            WaitForADLoginPageOrDesiredLandingPage(nextPage, windowsLogin: null);
        }

        /// <summary>
        /// For Insurance applications which are behind MS AAD protection, then 
        /// a user might get presented with a MS AD login view when trying to
        /// run their test.
        /// 
        /// This method will attempt to navigate that prompt if it appears. Allows
        /// for provision of explicit credentials to be used, otherwise it will
        /// just use the default WindowsAD credentials from config.json.
        /// </summary>
        /// <exception cref="Exception">If failed to navigate AAD or parameters are invalid.</exception>
        public void WaitForADLoginPageOrDesiredLandingPage(BasePage nextPage, Credentials windowsLogin)
        {
            int azureADCounter = 0;
            var credentials    = windowsLogin ?? Config.Get().WindowsAD;

            var endTime        = DateTime.Now.AddSeconds(WaitTimes.T90SEC);
            var authSeen       = false;
            var authCleared    = false;
            var success        = false;

            do
            {
                if (!authSeen)
                {
                    authSeen = IsDisplayed();
                }
                else
                {
                    // Only check AAD username and password because we are about to use it:
                    if (string.IsNullOrWhiteSpace(credentials.User))
                    { throw new ArgumentException("No username has been provided. This should be a RAC Azure AD email address."); }
                    if (string.IsNullOrEmpty(credentials.Pwd) || credentials.Pwd.Length < Constants.MinValidADPasswordLength)
                    { throw new ArgumentException($"It appears that an invalid value has been set for the Active Directory password. It needs to have a value equal to or greater than {Constants.MinValidADPasswordLength} characters."); }

                    try
                    {
                        // The user login should be an email address, so ammend if necessary.
                        // AD is PRD domain accounts, so we can use a constant.
                        NavigateMicrosoftADPage(credentials.User, credentials.Pwd);
                        authCleared = true;
                    }
                    catch (WebDriverException)
                    {
                        // An exception indicates that we ran into an issue
                        // navigating AAD. Reload page and try again.
                        Reporting.Log($"Windows AAD failed, will retry (attempt #{++azureADCounter})", _driver.TakeSnapshot());
                        _driver.Navigate().Refresh();
                        authSeen = false;
                        authCleared = false;
                    }
                }

                // If still not seen MS Auth page, check for our
                // intended page.
                if (!authSeen || authCleared)
                {
                    //BrowserStack Automate takes bit long to load the page
                    if (authCleared)
                    {
                        nextPage.WaitForPage(WaitTimes.T150SEC);
                    }
                    if (nextPage.IsDisplayed())
                    {
                        success = true;
                        break;
                    }

                }
                Thread.Sleep(200);
            } while (DateTime.Now < endTime && !success);

            if (!success && authSeen && !authCleared) { Reporting.Error("We couldn't traverse MS AD Authentication page."); }
            if (!success && !authSeen) { Reporting.Error("Neither MS AD Auth or desired page loaded, check network."); }
        }

        private void SubmitForm()
        {
            ClickControl(XPath.Login.Button.SignIn, skipJSScrollLogic: true);
        }

        private void NavigateMicrosoftADPage(string username, string password)
        {
            WaitForTextFieldAndEnterText(XPath.Login.Field.Email, username, false);
            SubmitForm();

            // MS does some fancy forms transitions and that trips up Selenium
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Login.Field.Password), WaitTimes.T30SEC);
            WaitForTextFieldAndEnterText(XPath.Login.Field.Password, password, false);
            SubmitForm();

            // MS does some fancy forms transitions and that trips up Selenium
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Heading.StaySignedIn), WaitTimes.T30SEC);
            SubmitForm();
        }
    }
}
