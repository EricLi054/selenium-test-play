using System;
using static Rac.TestAutomation.Common.Constants.General;

namespace Rac.TestAutomation.Common.DatabaseCalls.Queries.Environment
{
    public class ShieldParametersDB
    {
        /// <summary>
        /// Obtain the current state of a toggle for this Shield environment via the
        /// application parameters table.
        /// 
        /// I expect most of the time the value will be sufficient, but I've included 
        /// return of other useful values related to the record as we may wish to use
        /// these if we need to fetch values for more than one Shield Toggle.
        /// </summary>
        /// <param name="toggleReference">The ShieldToggle ENUM related to the DESCRIPTION value from T_APPLICATION_PARAMS in Shield for the toggle you need to interrogate.</param>
        public static ShieldEnvironmentToggle FetchShieldToggleForEnvironment(string toggleReference)
        {
            ShieldEnvironmentToggle result = new ShieldEnvironmentToggle();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Environment\\FetchShieldToggles.sql");
                var queryToggleReference = ShieldDB.SetSqlParameterForShieldToggleReference(toggleReference);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryToggleReference);
                    while (reader.Read())
                    {
                        result.Value = reader.GetDbValueFromColumnName("value");
                        result.Id = reader.GetDbValueFromColumnName("id");
                        result.Param_desc = reader.GetDbValueFromColumnName("param_desc");
                        result.Param_dev_desc = reader.GetDbValueFromColumnName("param_dev_desc");
                        result.Parameter_type = reader.GetDbValueFromColumnName("parameter_type");
                        result.Parameter_dev_desc = reader.GetDbValueFromColumnName("parameter_dev_desc");
                    }
                }
            }
            catch (AggregateException ex) { Reporting.Log("Exception occurred querying DB: " + ex.Message); }

            return result;
        }

        /// <summary>
        /// Check the value of "Toogle_for_RefundFW_B2C" (sic) is 'true' before beginning tests 
        /// as if it is not then the environment setup is invalid. Refer to INSU-1121.
        /// </summary>
        public static void RequireShieldFeatureToggleForRefundIsTrue()
        {
            var  shieldToggle_for_RefundFW_B2C = ShieldParametersDB.FetchShieldToggleForEnvironment(ShieldToggle.Toogle_for_RefundFW_B2C.GetDescription());

            if (shieldToggle_for_RefundFW_B2C.Value == "true")
            {
                Reporting.LogMinorSectionHeading($"Shield Toggle '{shieldToggle_for_RefundFW_B2C.Param_desc}' value = '{shieldToggle_for_RefundFW_B2C.Value}' and test can continue.");
            }
            else
            {
                Reporting.Error($"Shield Toggle '{ShieldToggle.Toogle_for_RefundFW_B2C.GetDescription()}' value = '{shieldToggle_for_RefundFW_B2C.Value}' and should be corrected before re-running this test.");
            }
        }
    }
}
