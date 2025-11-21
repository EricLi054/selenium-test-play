using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DBDriver;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Rac.TestAutomation.Common.DatabaseCalls
{
    public static class ShieldDB
    {
        private static string SQL_BASEFOLDER = "DBDriver\\SQL\\";

        public static Dictionary<string, string> SetSqlParameterForQuoteNumber(string value)
        {
            return new Dictionary<string, string>()
            {
                    { "quotenumber", value }
            };
        }

        public static Dictionary<string, string> SetSqlParameterForPolicyNumber(string value)
        {
            return new Dictionary<string, string>()
            {
                    { "policynumber", value }
            };
        }

        public static Dictionary<string, string> SetSqlParameterForProductId(string value)
        {
            return new Dictionary<string, string>()
            {
                    { "productid", value }
            };
        }

        public static Dictionary<string, string> SetSqlParameterForContactID(string value)
        {
            return new Dictionary<string, string>()
            {
                    { "ContactId", value }
            };
        }

        public static string ReadSQLFromFile(string file)
        {
            var subPath = SQL_BASEFOLDER;
            if (!IsAzureDb())
            { Reporting.Error("Only Azure MSSQL database connections supported."); }

            string query = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{subPath}{file}"));

            return query.Trim().TrimEnd(';');
        }

        public static IDatabase GetDatabaseHandle()
        {
            if (!IsAzureDb())
            { Reporting.Error("Only Azure MSSQL database connections supported."); }
            return new DbMsSqlDriver();
        }

        public static bool IsAzureDb()
        {
            var whatAmI = Config.Get().Shield.GetEnvironmentAsEnum();
            if(!DbMsSqlDriver.IsKnown(whatAmI))
            { Reporting.Error($"Shield database name ({whatAmI}) requested in config.json was not recognised."); }
            
            return true;
        }

        public static Dictionary<string, string> SetSqlParameterForShieldToggleReference(string value)
        {
            return new Dictionary<string, string>()
            {
                { "toggleReference", value }
            };
        }

        /// <summary>
        /// Get the value returned by a database query by providing the ordinal number of the
        /// columns returned.
        /// 
        /// If DBNull values are encountered they will be returned as a literal null/empty 
        /// value to avoid triggering an exception.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="index">The ordinal number of the column you wish to interrogate.</param>
        /// <returns></returns>
        public static string GetDbValue(this IDataReader reader, int index)
        {
            if (reader.IsDBNull(index)) return null;

            return reader.GetValue(index).ToString();
        }

        /// <summary>
        /// Get the value returned by a database query using the column name as a reference
        /// instead of the ordinal number (as is the case with GetDbValue).
        /// 
        /// This method obtains the ordinal number for the purposes of checking against IsDBNull
        /// to allow null values to be returned without triggering an exception.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName">The name of the column you wish to interrogate.</param>
        /// <returns></returns>
        public static string GetDbValueFromColumnName(this IDataReader reader, string columnName)
        {
            if (reader.IsDBNull(reader.GetOrdinal(columnName)))
            { 
                return null; 
            }

            return reader[columnName].ToString();
        }
    }
}
