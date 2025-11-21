using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rac.TestAutomation.Common.DatabaseCalls.Maintenance
{
    public class ScheduledJobs
    {
        /// <summary>
        /// Interrogate the Shield database and return the existance and trigger_state (or lack thereof) 
        /// for each job listed in the batchjoblist.json file. 
        /// </summary>
        public static List<string> GetStatusOfScheduledJob(string jobName)
        {
            var jobInstanceStatuses = new List<string>();
            string query = ShieldDB.ReadSQLFromFile("Maintenance\\GetShieldBatchJobCount.sql");

            var queryJobName = new Dictionary<string, string>()
                {
                    { "JobName", $"%{jobName}%" }
                };

            using (var db = ShieldDB.GetDatabaseHandle())
            {
                var reader = db.ExecuteQuery(query, queryJobName);

                while (reader.Read())
                {
                    jobInstanceStatuses.Add(reader.GetDbValue(1));
                    break;
                }
            }

            return jobInstanceStatuses;
        }
    }
}
