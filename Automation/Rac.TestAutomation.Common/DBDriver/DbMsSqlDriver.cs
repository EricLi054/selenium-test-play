using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

using static Rac.TestAutomation.Common.Constants.General;
using System.IO;

namespace Rac.TestAutomation.Common.DBDriver
{
    public class DbMsSqlDriver : IDatabase
    {
        private string _connString;

        private bool _isBusy;

        private SqlCommand    _cmd = null;
        private SqlConnection _con = null;
        private SqlDataReader _rdr = null;


        private static Dictionary<ENV, string> ENVIRONMENTS = new Dictionary<ENV, string>()
        {
            { ENV.SHIELDINT2, "dcc-shdsql-s193.rac.com.au" },
            { ENV.SHIELDSIT5, "dcc-shdsql-s195.rac.com.au" },
            { ENV.SHIELDSIT6, "dcc-shdsql-s196.rac.com.au" },
            { ENV.SHIELDSIT7, "dcc-shdsql-s197.rac.com.au" },
            { ENV.SHIELDSIT8, "dcc-shdsql-s198.rac.com.au" },
            { ENV.SHIELDUAT5, "dcc-shdsql-u195.rac.com.au" },
            { ENV.SHIELDUAT6, "dcc-shdsql-u196.rac.com.au" },
            { ENV.SHIELDUAT7, "dcc-shdsql-u197.rac.com.au" }

        };

        private static readonly string[] _Shieldv19EnvironmentsNeedSqlAuth = { "SHIELDINT2", "SHIELDDEV2", "SHIELDSIT5", "SHIELDSIT6", "SHIELDSIT7", "SHIELDSIT8", "SHIELDUAT5", "SHIELDUAT6", "SHIELDUAT7" };

        /// <summary>
        /// Returns true/false if environment name exists in
        /// this class' dictionary of supported databases.
        /// </summary>
        /// <param name="environmentName"></param>
        /// <returns></returns>
        public static bool IsKnown(ENV environmentName)
        {
            if (ENVIRONMENTS.ContainsKey(environmentName))
                return true;

            return false;
        }

        /// <summary>
        /// Construct the connection string for a MSSQL database.
        /// Shield v17 environments simply use SSPI but v19 environments can't currently support that as our 
        /// PA accounts are based in racnpe.local rather than rac.com.au. As such we use SQL Server Authentication 
        /// via a username and password.
        /// TODO AUNT-223 Update this summary when removing support for SQL Server Authentication.
        /// </summary>
        public DbMsSqlDriver()
        {
            var usr = Config.Get().Shield.Database.User; //TODO AUNT-223 Remove support for sql Server Authentication
            var pwd = Config.Get().Shield.Database.Pwd;  //TODO AUNT-223 Remove support for sql Server Authentication
            var env = Config.Get().Shield.GetEnvironmentAsEnum();
            var v19dbName = "SHIELD"; //TODO AUNT-223 Remove support for sql Server Authentication
            var datasource = MatchDBConnection(env);

            if (_Shieldv19EnvironmentsNeedSqlAuth.Contains(env.ToString()))
            {
                //TODO AUNT-223 When Shield v19 environments do not require special treatment this condition can be removed
                Reporting.Log($"Shield v19 environment detected, setting Initial Catalog = 'SHIELD' for connection string and " +
                    $"using SQL Server Authentication with username and password from config.");
                if (usr == null || pwd == null)
                {
                    Reporting.Error($"SQL Server Authentication is required for Shield v19 environments but the username and/or password " +
                        $"are missing from the config file. Please add them and try again.");
                }
                _connString = String.Format($"Server={datasource};Initial Catalog={v19dbName};User id={usr};Password={pwd}");
            }
            else
            {
                //TODO AUNT-223 only the _connString line below should be necessary; consider removing this logging
                Reporting.Log($"Shield v17 environment detected, setting Initial Catalog = '{env}' and using SSPI for DB " +
                    $"login credentials.");
                _connString = String.Format($"Server={datasource};Initial Catalog={env};Integrated Security=SSPI");
            }

            _isBusy = false;
        }

        public void Dispose()
        {
            FreeDBResource();
            if (_rdr != null)
            {
                _rdr.Close();
                _rdr.Dispose();
            }
            if (_cmd != null)
            {
                _cmd.Cancel();
                _cmd.Dispose();
            }
            if (_con != null)
            {
                _con.Close();
                _con.Dispose();
            }
        }

        /// <summary>
        /// Executes the provided MSSQL query, and returns the reader.
        /// The caller is responsible of disposing of this class once
        /// the call has been completed.
        /// 
        /// SQL paraneters are supported. Query should use the ":variable"
        /// syntax in its query string.
        /// </summary>
        /// <param name="query">Plain string query, can contain line feeds and comment syntax.</param>
        /// <param name="queryParams">Optional dictionery of parameters for SqlCommand to substitute at runtime.</param>
        /// <returns></returns>
        public IDataReader ExecuteQuery(string query, Dictionary<string,string> queryParams)
        {
            GetDBResource();

            try
            {
                _con = new SqlConnection(_connString);
                _con.Open();

                //disabling, this takes parameters but the queries are loaded from a text file and codacy can't prove that this is safe
                #pragma warning disable csharp_injection_rule-SQLInjection
                _cmd = new SqlCommand(query);
                #pragma warning restore csharp_injection_rule-SQLInjection
                _cmd.Connection = _con;
                _cmd.CommandType = CommandType.Text;
				_cmd.CommandTimeout = 0;

                if (queryParams != null)
                {
                    foreach (var key in queryParams.Keys)
                    {
                        _cmd.Parameters.Add(new SqlParameter(key, queryParams[key]));
                    }
                }

                _rdr = _cmd.ExecuteReader();
            }
            catch (Exception ex) when (ex is SqlException || ex is InvalidCastException || ex is InvalidOperationException || ex is IOException)
            { Reporting.Log("Exception occured querying DB: " + ex.Message); }
            finally
            {
                FreeDBResource();
            }
            return _rdr;
        }

        private void GetDBResource()
        {
            // Slightly arbitrary chosen value. Within the scope of
            // motor vehicle policies, 10minutes should be plenty.
            // This may need to be revised as other tests move to .Net 
            var timeoutLimit = DateTime.Now.AddSeconds(WaitTimes.T600SEC);

            while (_isBusy && (DateTime.Now < timeoutLimit))
            {
                System.Threading.Thread.Sleep(2000);
            }

            if (_isBusy)
            { Reporting.Error("Timeout while waiting for the DB resource to free up from a previously executing query."); }

            _isBusy = true;
        }

        private void FreeDBResource()
        {
            _isBusy = false;
        }

        private string MatchDBConnection(ENV envName)
        {
            if (!IsKnown(envName))
            { Reporting.Error($"The environment {envName} is not recognised for Azure MSSQL databases."); }

            return ENVIRONMENTS[envName];
        }
    }
}
