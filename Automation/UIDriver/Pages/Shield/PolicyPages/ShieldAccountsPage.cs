using Rac.TestAutomation.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.ComponentModel;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;
using System.Data;

namespace UIDriver.Pages.Shield
{
    public class ShieldAccountsPage : BaseShieldPage
    {
        public enum ACCOUNTS_TABS
        {
            [Description("Account Dashboard")]
            AccountDashboard,
            [Description("Account Installments")]
            AccountInstallments,
            [Description("Installments Dunning Steps")]
            InstallmentsDunningSteps
        }

        private class XPath
        {
            public class Ribbon
            {
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
            public class Dashboard
            {
                public const string AccountBalance   = "id('accountBalanceVO@accountBalanceFormatted')";
                public const string TransactionTable = "id('idit-grid-table-filteredDetailsList_pipe_')";
                public const string AllPaidTransactions     = "id('idit-grid-table-filteredDetailsList_pipe_')//tr[@role='row']/td[@aria-describedby='idit-grid-table-filteredDetailsList_pipe__transactionStatusVO' and text()='Paid']/..";
                public const string AllRejectedTransactions = "id('idit-grid-table-filteredDetailsList_pipe_')//tr[@role='row']/td[@aria-describedby='idit-grid-table-filteredDetailsList_pipe__transactionStatusVO' and text()='Counter']/..";
            }
            public class ShieldContextMenu
            {
                public const string RejectPayment = "id('lifilteredDetailsList|rejectPayment')";
            }
        }

        #region Settable properties and controls
        public ACCOUNTS_TABS CurrentTab
        {
            get => DataHelper.GetValueFromDescription<ACCOUNTS_TABS>(GetElement(XPath.Tabs.Current).GetAttribute("title"));
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

        public ShieldAccountsPage(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XPath.Ribbon.ActionsMenu);
                GetElement(XPath.Tabs.Current);
                GetElement(XPath.Dashboard.AccountBalance);
                GetElement(XPath.Dashboard.TransactionTable);
                var breadcrumbs = GetInnerText(XPath.Ribbon.BreadCrumbs);
                isDisplayed = breadcrumbs.Contains("Account Policy ");
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
            var isDisplayed = GetInnerText(XPath.Ribbon.BreadCrumbs).Contains($"Account Policy {policyNumber} AU");
            return isDisplayed;
        }

        public void RejectFirstPaidPayment()
        {
            if (!_driver.TryFindElement(By.XPath(XPath.Dashboard.AllPaidTransactions), out IWebElement firstPaidRow))
            {
                Reporting.Error("This policy is not suitable as we didn't see any paid transactions we can reject.");
            }
            firstPaidRow.Click();
            Actions actions = new Actions(_driver);
            actions.ContextClick(firstPaidRow).Perform();

            _driver.WaitForElementToBeVisible(By.XPath(XPath.ShieldContextMenu.RejectPayment), WaitTimes.T5SEC);
            ClickControl(XPath.ShieldContextMenu.RejectPayment);

            _driver.WaitForElementToBeVisible(By.XPath(XPath.Dashboard.AllRejectedTransactions), WaitTimes.T30SEC);
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
