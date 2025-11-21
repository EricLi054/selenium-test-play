using System;
using System.Data.SqlClient;
using System.IO;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyPet;

namespace Rac.TestAutomation.Common.DatabaseCalls.Policies
{
    public class ShieldPetDB
    {
        /// <summary>
        /// Fetch an existing Pet Policy to verify stored properties.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <returns></returns>
        public static PetPolicy FetchPetPolicyDetail(string policyNumber)
        {
            PetPolicy result = null;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\PetPolicyByNumber.sql");
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        result = new PetPolicy()
                        {
                            PolicyNumber  = policyNumber,
                            PolicyStartDate  = DateTime.Parse(reader.GetDbValue(11)),
                            HasTLCCover   = reader.GetDbValue(12).Equals("Tender Loving Care"),
                            PetType       = reader.GetDbValue(9).Equals("Dog") ? PetType.Dog : PetType.Cat,
                            PetBreed      = reader.GetDbValue(10),
                            PetName       = reader.GetDbValue(8),
                            IsDirectDebit = reader.GetDbValue(4).Equals(DIRECT_DEBIT),
                            PaymentFrequency = DataHelper.GetValueFromDescription<PaymentFrequency>(reader.GetDbValue(5)),
                            PolicyholderDOB  = DateTime.Parse(reader.GetDbValue(2)),
                            PolicyholderAge  = int.Parse(reader.GetDbValue(7)),
                            InstallmentCount = int.Parse(reader.GetDbValue(13)),
                            HasPreExistIllness = bool.Parse(reader.GetDbValue(14))
                        };
                        break; // We will only take the first row of returned data.
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException) 
            { Reporting.Log("FetchPetPolicyDetail Exception occurs querying DB: " + ex.Message); }

            return result;
        }
    }
}
