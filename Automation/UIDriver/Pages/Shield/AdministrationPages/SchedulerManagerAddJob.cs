using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    public class SchedulerManagerAddJob : BaseShieldPage
    {

        #region XPATHS
        private const string XP_JOB_TYPE_FIELD = "id('s2id_jobType')";
        private const string XP_JOB_TYPE_OPTIONS = "id('select2-results-1')/li/div";

        private const string XP_TRIGGER_TYPE_FIELD = "id('s2id_triggerType')";
        private const string XP_TRIGGER_TYPE_OPTIONS = "id('select2-results-2')/li/div";

        private const string XP_PRIORITY_FIELD = "id('priority')";
        private const string XP_DATE_FROM_FIELD = "id('fromDate')";
        private const string XP_DATE_TO_FIELD = "id('toDate')";
        private const string XP_TRIGGER_TIME_FIELD = "id('hour')";
        private const string XP_EXCLUDE_DATES_HEADER = "id('panel-2000022')";
        private const string XP_EXCLUDED_DAYS_OF_WEEK = "id('excludedDaysWeek')";

        private const string XP_ADD_JOB_BUTTON = "id('addJob')";
        #endregion

        public SchedulerManagerAddJob(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_JOB_TYPE_FIELD);
                GetElement(XP_DATE_FROM_FIELD);
                GetElement(XP_DATE_TO_FIELD);
                GetElement(XP_ADD_JOB_BUTTON);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Configures the requested batch job to run daily for the next
        /// year. Start date is tomorrow as that guarantees we haven't
        /// passed the requested start time for today.
        /// 
        /// We leave priority at the default value provided by Shield.
        /// </summary>
        /// <param name="jobName">Name of the batch job. Should match Shield text exactly</param>
        /// <param name="dailyStartTime"></param>
        public void AddBatchJob(string jobName, DateTime dailyStartTime, string excludedWeekDays)
        {
            WaitForPage(WaitTimes.T30SEC);
            WaitForSelectableAndPickFromDropdown(XP_JOB_TYPE_FIELD, XP_JOB_TYPE_OPTIONS, jobName);
            // Shield does a screen refresh on choice of job, so sleep to
            // allow that refresh before operating on DOM again.
            Thread.Sleep(3000);

            WaitForSelectableAndPickFromDropdown(XP_TRIGGER_TYPE_FIELD, XP_TRIGGER_TYPE_OPTIONS, "Daily-en");
            // These date fields all accept format "dd/MM/yyyy"
            WaitForTextFieldAndEnterText(XP_DATE_FROM_FIELD, DateTime.Now.AddDays(1).ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), false);
            WaitForTextFieldAndEnterText(XP_DATE_TO_FIELD, DateTime.Now.AddDays(366).ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), false);
            WaitForTextFieldAndEnterText(XP_TRIGGER_TIME_FIELD, dailyStartTime.ToString(DataFormats.TIME_FORMAT_24HR), false);
            if (!string.IsNullOrEmpty(excludedWeekDays))
            {
                // Please note that the date fields in the Exclude dates section each require a different format of date.
                ClickControl(XP_EXCLUDE_DATES_HEADER);
                WaitForTextFieldAndEnterText(XP_EXCLUDED_DAYS_OF_WEEK, excludedWeekDays, false);
            }
            Reporting.Log("Screenshot of Job Scheduling Screen prior to selecting Add Job button", _browser.Driver.TakeSnapshot());
            ClickControl(XP_ADD_JOB_BUTTON);
        }
    }
}