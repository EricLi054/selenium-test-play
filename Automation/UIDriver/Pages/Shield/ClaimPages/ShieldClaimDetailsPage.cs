using Rac.TestAutomation.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    public class ShieldClaimDetailsPage : BaseShieldPage
    {
        // References to tab specific page objects.
        public Dashboard        Dashboard        { get; set; }
        public DependenciesTree DependenciesTree { get; set; }
        public RelatedPolicy    RelatedPolicy    { get; set; }

        public enum CLAIM_TABS
        {
            [Description("Dashboard")]
            Dashboard,
            [Description("Claim Primary Details")]
            PrimaryDetails,
            [Description("Claim Dependencies Tree")]
            DependenciesTree,
            [Description("Related Policy")]
            RelatedPolicy,
            [Description("Claim History")]
            History,
            [Description("Agenda")]
            Agenda
        }

        private class XPath
        {
            public class Ribbon
            {
                public const string UpdateClaim = "id('updateClaim_Link')";
                public const string ActionsMenu = "id('ActionsLink')";
                public const string MenuViewEvents = "//ul[@class='idit-actions-menu']//a[@id='ViewEntityEvents_Link']";
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
        }

        #region Settable properties and controls
        public CLAIM_TABS CurrentTab
        {
            get => DataHelper.GetValueFromDescription<CLAIM_TABS>(GetElement(XPath.Tabs.Current).GetAttribute("title"));
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

        public ShieldClaimDetailsPage(Browser browser) : base(browser)
        {
            Dashboard        = new Dashboard(browser);
            DependenciesTree = new DependenciesTree(browser);
            RelatedPolicy    = new RelatedPolicy(browser);
        }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XPath.Ribbon.ActionsMenu);
                GetElement(XPath.Tabs.Current);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Select the "Update Claim" control from the upper ribbon bar.
        /// </summary>
        public void ClickUpdateClaim()
        {
            ClickControl(XPath.Ribbon.UpdateClaim);
            DependenciesTree.WaitForUpdateClaimMode();
        }

        public void OpenClaimEventLog()
        {
            var endtime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
            var success = false;

            do
            {
                try
                {
                    ClickControl(XPath.Ribbon.ActionsMenu);
                    if (_driver.TryWaitForElementToBeVisible(By.XPath(XPath.Ribbon.MenuViewEvents), WaitTimes.T5SEC, out IWebElement menuItem))
                    {
                        Thread.Sleep(SleepTimes.T1SEC);
                        success = true;
                        menuItem.Click();
                        break;
                    }
                }
                catch (NoSuchElementException ex) { Reporting.Log($"Unable to find element: {ex}"); }
                finally
                {
                    Thread.Sleep(SleepTimes.T1SEC);
                }
            } while (DateTime.Now < endtime);

            if (!success) { Reporting.Error("Failed to drive Shield claims Action dropdown menu."); }
        }

        /// <summary>
        /// Click the "Select" button
        /// </summary>
        public void ClickSelect()
        {
            ClickControl(XPath.Footer.Select);
        }

        /// <summary>
        /// Click 'OK' in the Validation error dialog if it appears
        /// </summary>
        public void AcknowledgeValidationErrorDialog()
        {
            if (IsBasicNotificationPresent())
            {
                ClickControl(XPath.Dialog.ButtonOK);
            }
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

        /// <summary>
        /// When completing an update on a Home Claim, Shield sometimes provides a 
        /// notice of claim classification, which after dismissing, reloads the claim 
        /// details, requiring the user to click Finish a second time.
        /// </summary>
        public void FinishChangesAndDismissHomeClaimsClassificationDialog()
        {
            ClickFinish();
            if (DismissHomeClaimsClassificationDialog())
            {
                // Sleep as page refreshes
                Thread.Sleep(SleepTimes.T2SEC);

                if (IsDisplayed())
                { ClickFinish(); }
            }
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
