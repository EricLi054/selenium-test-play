using Rac.TestAutomation.Common;
using OpenQA.Selenium;
using System;
using System.ComponentModel;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    public class ShieldPolicyDetailsPage : BaseShieldPage
    {
        public enum POLICY_TABS
        {
            [Description("Dashboard")]
            Dashboard,
            [Description("General Details")]
            GeneralDetails,
            [Description("Policy contacts")]
            PolicyContacts,
            [Description("Payments")]
            Payments,
            [Description("Policy Covers")]
            PolicyCovers
        }

        private class XPath
        {
            public class Ribbon
            {
                public const string GeneralEndorsment = "id('createGeneralChange_Link')";
                public const string ActionsMenu = "id('ActionsLink')";
                public const string BreadCrumbs = "id('contextBar')/ul[@class='idit-breadcrumbs']/li/b";
            }
            public class Tabs
            {
                public const string Current = "//ul[contains(@class,'idit-tabs-nav')]/li[@aria-selected='true']";
            }
            public class Dialog
            {
                public const string Frame = "//*[@aria-describedby='BasicNotificationDialog']";
                public const string ButtonOK = Frame + "//*[@id='DialogOK']";
            }
            public class Footer
            {
                public const string Finish = "//button[@id='Finish' and @title='Finish']";
                public const string Select = "//button[@id='Finish' and @title='Select']";
                public const string Return = "id('Return')";
            }
            public class Sidebar
            {
                public const string AccountListHeader = "//a[@title='Accounts']";
                public static string PolicyAccount(string policyNumber) => $"//a[@id='Policy {policyNumber} AU']";
            }
            public class Dashboard
            {
                public const string PolicyNumber = "id('IDITForm@externalPolicyNr')";
            }
        }

        #region Settable properties and controls
        public POLICY_TABS CurrentTab
        {
            get => DataHelper.GetValueFromDescription<POLICY_TABS>(GetElement(XPath.Tabs.Current).GetAttribute("title"));
            set
            {
                var tabControl = GetElement($"//li[@title='{value.GetDescription()}']");
                if (tabControl.GetAttribute("aria-selected") != "true")
                {
                    tabControl.Click();
                    Thread.Sleep(SleepTimes.T2SEC);  // Allow transition time for tab to begin rendering
                }
            }
        }
        #endregion

        public ShieldPolicyDetailsPage(Browser browser) : base(browser) {}

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XPath.Ribbon.ActionsMenu);
                GetElement(XPath.Tabs.Current);
                var breadcrumbs = GetInnerText(XPath.Ribbon.BreadCrumbs);
                isDisplayed = breadcrumbs.Contains("Policy:");
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Returns whether the desired policy number is displayed in the Dashboard
        /// as well as the page ribbon breadcrumbs.
        /// </summary>
        public bool IsDisplayingDesiredPolicy(string policyNumber)
        {
            var isDisplayed = GetInnerText(XPath.Ribbon.BreadCrumbs).Contains($"Policy: {policyNumber}");
            isDisplayed    &= GetInnerText(XPath.Dashboard.PolicyNumber).Trim().Equals(policyNumber);
            return isDisplayed;
        }

        public void SelectPolicyPaymentAccountFromSidebar(string policyNumber)
        {
            var accountXPath = XPath.Sidebar.PolicyAccount(policyNumber);
            if (!_driver.TryWaitForElementToBeVisible(By.XPath(accountXPath), WaitTimes.T5SEC, out IWebElement accountItem))
            {
                ClickControl(XPath.Sidebar.AccountListHeader);
                _driver.WaitForElementToBeVisible(By.XPath(accountXPath), WaitTimes.T5SEC);
            }
            ClickControl(accountXPath);
        }

        /// <summary>
        /// Click the "Select" button
        /// </summary>
        public void ClickSelect()
        {
            ClickControl(XPath.Footer.Select);
        }

        public bool IsBasicNotificationPresent()
        {
            IWebElement element;
            if (_driver.TryFindElement(By.XPath(XPath.Dialog.Frame), out element))
            {
                return element.Displayed;
            }
            return false;
        }

        public void ClickFinish()
        {
            ClickControl(xpath: XPath.Footer.Finish, waitTimeSeconds: WaitTimes.T10SEC);
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Because we can no longer detect the Shield confirmation pop
        /// up dialogs, this method will monitor the "Finish" button
        /// disappearing as a means to determine that we have exited
        /// Update mode successfully.
        /// </summary>
        public void WaitForFinishButtonToDisappear()
        {
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.Footer.Finish), WaitTimes.T30SEC);
        }

        public void ClickReturn()
        {
            ClickControl(XPath.Footer.Return);
        }
    }
}
