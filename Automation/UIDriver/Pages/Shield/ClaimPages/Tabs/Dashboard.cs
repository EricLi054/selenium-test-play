using Rac.TestAutomation.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    /// <summary>
    /// It is intended that this class is not used directly by tests,
    /// but instead is invoked indirectly via the reference in
    /// ShieldClaimDetailsPage.cs
    /// </summary>
    public class Dashboard : BaseShieldPage
    {
        private class XPath
        {
            public const string StatusAndReason = "id('IDITForm@stateAndReason')";

            public class Tasks
            {
                public const string Table = "id('openTaskList|')";
                public const string RowTitle = Table + "//li//div[@class='mainLine']";
            }
            public class Correspondence
            {
                public const string Table = "id('correspondenceList|')";
                public const string RowByTitle = Table + "//li//span[@title='{0}']";
            }
        }

        public string ClaimStatusAndReason => GetInnerText(XPath.StatusAndReason);

        public Dashboard(Browser browser) : base(browser) {}

        public override bool IsDisplayed()
        {
            return true;
        }

        /// <summary>
        /// Looks for a task that contains a specific block of text
        /// on the dashboard. Text search will be case sensitive.
        /// </summary>
        /// <param name="taskText">Task text string</param>
        /// <returns>TRUE if found</returns>
        public bool IsTaskPresent(string taskText)
        {
            return _driver.TryWaitForElementToBeVisible(By.XPath($"{XPath.Tasks.RowTitle}//span[@title='{taskText}']"), WaitTimes.T5SEC, out IWebElement taskRow);
        }

        /// <summary>
        /// Looks for a task that contains a specific block of text
        /// on the dashboard. Text search will be case sensitive.
        /// </summary>
        /// <param name="taskText">Task text string</param>
        /// <returns>TRUE if found</returns>
        public void OpenTaskItem(string taskText)
        {
            ClickControl($"{XPath.Tasks.RowTitle}//span[@title='{taskText}']");
        }

        public void OpenCorrespondenceItemByTitle(string correspondenceText)
        {
            ClickControl(string.Format(XPath.Correspondence.RowByTitle, correspondenceText));
        }
    }
}
