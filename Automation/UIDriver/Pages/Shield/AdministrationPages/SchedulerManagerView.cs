using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Shield
{
    public class SchedulerManagerView : BaseShieldPage
    {

        #region XPATHS
        private const string XP_TRIGGERED_JOBS_CONTAINER = "id('gbox_idit-grid-table-flattendListjobsList_pipe_')";
        private const string XP_TRIGGERED_JOBS_TABLE     = "id('idit-grid-table-flattendListjobsList_pipe_')";
        private const string XP_ADD_JOB_BUTTON           = "id('flattendListjobsList|New')";
        #endregion

        public SchedulerManagerView(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_TRIGGERED_JOBS_CONTAINER);
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
        /// Click the '+' button above the triggered jobs table
        /// to begin process of adding a Shield Batch job to the
        /// scheduler.
        /// </summary>
        public void ClickAddJob()
        {
            ClickControl(XP_ADD_JOB_BUTTON);

        }
    }
}
