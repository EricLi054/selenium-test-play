using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Rac.TestAutomation.Common.DatabaseCalls.Policies
{
    public class ShieldBankDetailsDB
    {
        public static BankAccount GetDirectDebitDetailsForPolicy(String policyNum)
        {
            BankAccount result = null;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\GetPolicyBankAccountDetails.sql");

                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);

                     while (reader.Read())
                     {
                         result = new BankAccount()
                         {
                             Id = reader.GetDbValue(3),
                             Bsb = reader.GetDbValue(0),
                             AccountNumber = reader.GetDbValue(1),
                             AccountName = reader.GetDbValue(2)
                         };

                         break;
                     }  
                }
            }
            catch (Exception e) when (e is ArgumentException || e is UnauthorizedAccessException || e is FileNotFoundException
                                        || e is NotSupportedException || e is IndexOutOfRangeException || e is NullReferenceException
                                        || e is InvalidDataException || e is SqlException || e is ArgumentNullException || e is FormatException)
            {
                Reporting.Log($"Exception Occured:{e.Message}");
            }

            return result;
        }
    }
    
}
