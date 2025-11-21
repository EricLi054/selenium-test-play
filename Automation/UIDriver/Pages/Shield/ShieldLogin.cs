using Rac.TestAutomation.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics.CodeAnalysis;

namespace UIDriver.Pages.Shield
{
    public class ShieldLogin : BasePage
    {
        #region XPATHS
        private const string XP_INPUT_USERNAME = "id('UserName')";
        [SuppressMessage("SonarLint", "S2068", Justification = "Codacy worries that this is a hard coded credential, but it is not.")]
        private const string XP_INPUT_PASSWORD = "id('Password')";
        private const string XP_BUTTON_LOGIN = "id('Login')";
        [SuppressMessage("SonarLint", "S2068", Justification = "Codacy worries that this is a hard coded credential, but it is not.")]
        private const string XP_LINK_CHANGE_PASSWORD = "id('login_change')";
        private const string XP_TEXT_VERSION = "//div[@class='footer-info']";


        private const string XP_FRAME_ERROR_NOTICE = "id('login_error')";
        private const string XP_TEXT_ERROR_MESSAGE = "id('login_error_span')";
        #endregion

        #region Settable properties and controls
        public bool IsErrorDialogShown
        {
            get
            {
                IWebElement errorDialog;
                return _driver.TryFindElement(By.Id(XP_FRAME_ERROR_NOTICE), out errorDialog);
            }
        }

        public string Username
        {
            get => throw new NotImplementedException("Not yet implemented access to shadow DOM.");
            set => WaitForTextFieldAndEnterText(XP_INPUT_USERNAME, value, false);
        }

        public string Password
        {
            get => throw new NotImplementedException("Not yet implemented access to shadow DOM.");
            set => WaitForTextFieldAndEnterText(XP_INPUT_PASSWORD, value, false);
        }
        #endregion

        public ShieldLogin(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_INPUT_USERNAME);
                GetElement(XP_INPUT_PASSWORD);
                GetElement(XP_BUTTON_LOGIN);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        public void ClickLoginButton()
        {
            ClickControl(XP_BUTTON_LOGIN);
        }

        public void LoginUser(string username, string password)
        {
            Reporting.Log($"Attempt login with user {username}");
            Username = username;
            Password = password;
            ClickLoginButton();
        }
    }
}
