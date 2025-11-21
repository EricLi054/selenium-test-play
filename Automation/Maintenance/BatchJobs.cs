using Newtonsoft.Json;
using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Maintenance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UIDriver.Pages.Shield;
using UIDriver.Helpers;

namespace Maintenance
{
    [Property("Functional", "Shield Maintenance tasks")]
    public class BatchJobs : BaseUITest
    {
        private const string BATCH_JOB_PAUSED_STATE = "PAUSED";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Scheduling of the required daily Shield batch jobs for data health.");
        }
        /// <summary>
        /// Automated Maintenance script to ensure that the "basic hygeine" jobs
        /// are scheduled in Shield.
        /// These are important to maintain a simulacrum of production-like
        /// function, keeping data from becoming too stale to recover from.
        /// </summary>
        [Test, Category("MaintenanceShieldBatchJobs")]
        public void ScheduleBatchJobs()
        {
            var requestedJobs  = ReadBatchJobList();
            Reporting.Log($"Running Job Scheduler checks against {_testConfig.Shield.Web}");
            var jobsToSchedule = VerifyRequestedJobsInShieldDatabase(requestedJobs, out List<BatchJobConfigElement> jobsInError);
            
            if (jobsToSchedule.Count > 0)
            {
                _browser.OpenShieldAndLogin();

                using (var homePage = new ShieldSearchPage(_browser))
                using (var JobViewPage = new SchedulerManagerView(_browser))
                using (var addJobPage = new SchedulerManagerAddJob(_browser))
                {
                    homePage.OpenJobScheduler();
                    JobViewPage.WaitForPage();

                    foreach (var job in jobsToSchedule)
                    {
                        Reporting.Log($"Adding batch job: {job.Name} for time: {job.Time} Excluded days: {job.ExcludedWeekDays}");
                        JobViewPage.ClickAddJob();

                        addJobPage.AddBatchJob(job.Name, job.ParsedTime, job.ExcludedWeekDays);
                        JobViewPage.WaitForAndClearGenericConfirmationDialog();
                    }
                }
                _browser.LogoutShieldAndCloseBrowser();
                var failedToScheduleList = VerifyRequestedJobsInShieldDatabase(requestedJobs, out List<BatchJobConfigElement> jobsFailedToSchedule);

                Reporting.IsFalse(failedToScheduleList.Any(), "All jobs should have been scheduled, we had some jobs not appearing to be recorded. Check logging.");
            }

            if (jobsInError.Count > 0)
                Reporting.Error($"See earlier log statements, as {jobsInError.Count} jobs were found with duplicate scheduling and/or paused jobs.");
        }

        /// <summary>
        /// Read in the batchjoblist.json file. This file defines the list
        /// of Shield batch jobs that we wish to ensure are scheduled
        /// and the time at which they should run each day.
        /// </summary>
        /// <returns></returns>
        public static List<BatchJobConfigElement> ReadBatchJobList()
        {
            var jobs = new List<BatchJobConfigElement>();
            string file = string.Format("{0}\\batchjoblist.json", TestContext.CurrentContext.TestDirectory);
            if (File.Exists(file))
            {
                jobs = JsonConvert.DeserializeObject<List<BatchJobConfigElement>>(File.ReadAllText(file));
            }
            return jobs;
        }

        /// <summary>
        /// Verifies if the requested job is already scheduled in Shield.
        /// Does not care if the time matches, but just looks to see if
        /// it is already there and fetches the trigger_state if it is.
        /// 
        /// If the trigger_state 'PAUSED' is found, the job is flagged for
        /// investigation as paused jobs don't help anyone.
        /// 
        /// Will also look for the erroneous case of a job being scheduled
        /// more than once as this would be seen as a problematic scenario.
        /// </summary>
        /// <param name="jobList">List of jobs to verify from batchjoblist.json</param>
        /// <param name="jobsWithErrors">Returned list of jobs that are paused OR have multiple entries scheduled.</param>
        /// <returns>List of jobs which are not currently scheduled and thus need to be added</returns>
        public List<BatchJobConfigElement> VerifyRequestedJobsInShieldDatabase(List<BatchJobConfigElement> jobList, out List<BatchJobConfigElement> jobsWithErrors)
        {
            var jobsToSchedule = new List<BatchJobConfigElement>();
            jobsWithErrors = new List<BatchJobConfigElement>();

            // the time part can be parsed:
            foreach (var job in jobList)
            {
                var foundJobCount = ScheduledJobs.GetStatusOfScheduledJob(job.Name);
                if (foundJobCount.Count > 0)
                {
                    if (foundJobCount.Contains(BATCH_JOB_PAUSED_STATE))
                    {
                        Reporting.Log($"Job ({job.Name}) was found with at least one paused instance. NEEDS INVESTIGATION.");
                        jobsWithErrors.Add(job);
                    }
                    else if (foundJobCount.Count > 1)
                    {
                        Reporting.Log($"Job ({job.Name}) is scheduled multiple times ({foundJobCount}). NEEDS INVESTIGATION.");
                        jobsWithErrors.Add(job);
                    }
                    else
                        Reporting.Log($"Job ({job.Name}) is scheduled appropriately. No action needed.");
                }
                else
                {
                    Reporting.Log($"Job ({job.Name}) is not scheduled to run daily at {job.Time} (Excluding {job.ExcludedWeekDays}) as expected.");
                    jobsToSchedule.Add(job);
                }
            }

            return jobsToSchedule;
        }

        public class BatchJobConfigElement
        {
            private string _excludeWeekDays;
            private readonly string[] _acceptedDays = { "sat", "sun", "mon", "tue", "wed", "thu", "fri" };

            public string Name { get; set; }
            public string ExcludedWeekDays
            {
                get => _excludeWeekDays;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        var days = value.Split(',');
                        foreach (var day in days)
                        {
                            if (!_acceptedDays.Any(x => x.Equals(day.ToLower())))
                                Reporting.Error($"Unrecognised value provided in ExcludedWeekDays for {Name}. Check value: {value}. Bad item: {day}");
                        }
                    }

                    _excludeWeekDays = value;
                }
            }
            public string Time { get; set; }
            public DateTime ParsedTime
            {
                get
                {
                    if (!DateTime.TryParse(Time, out DateTime parsedTime))
                        Reporting.Error($"Failed to parse batch job list. Error reading time {Time} for job {Name}");
                    return parsedTime;
                }
            }
        }
    }
}
