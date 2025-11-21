using Rac.TestAutomation.Common.DataModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Rac.TestAutomation.Common.DatabaseCalls.Claims
{
    public class ShieldClaimDB
    {
        /// <summary>
        /// Retrieve all the events for the associated claim.
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <returns></returns>
        public static List<string> GetClaimEvents(string claimNumber)
        {
            var result = new List<string>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\ClaimGetEvent.sql");

                var queryClaimDetails = new Dictionary<string, string>()
                {
                    { "claimNumber", claimNumber }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryClaimDetails);
                    while (reader.Read())
                    {
                        result.Add(reader.GetDbValue(0));
                    }
                }
            }
            catch (Exception ex) when (ex is SqlException || ex is ArgumentNullException || ex is FormatException)
            {
                Reporting.Log(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Retrieve all the events for the associated Case.
        /// </summary>
        /// <param name="caseNumber"></param>
        /// <returns>All events created under the case in last 24 hr</returns>
        public static List<string> GetCaseEvents(string caseNumber)
        {
            var result = new List<string>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\CaseGetEvent.sql");

                var queryCaseDetails = new Dictionary<string, string>()
                {
                    { "caseNumber", caseNumber }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryCaseDetails);
                    while (reader.Read())
                    {
                        result.Add(reader.GetDbValue(0));
                    }
                }
            }
            catch (Exception ex) when (ex is SqlException || ex is ArgumentNullException || ex is FormatException)
            {
                Reporting.Log(ex.Message);
            }

            return result;
        }


        /// <summary>
        /// Fetching list of claim not in closed state from all product types
        /// </summary>
        /// <returns>List of claims created on NPE(claim number starting with '2')</returns>
        public static List<PolicyClaimCasePair> FindListOfOpenClaimsFromAllProducts(ShieldProductType? productType=null)
        {
            var candidates = new List<PolicyClaimCasePair>();
            var queryParam = new Dictionary<string, string>();

            if (productType == null)
            {
                queryParam.Add("AnyProductType" , "1");
                queryParam.Add("productType" , "0");
            }
            else
            {
                queryParam.Add("AnyProductType", "0");
                queryParam.Add("productType" , ((int)productType).ToString());
            }

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\SearchForOpenClaimNumbers.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParam);
                    while (reader.Read())
                    {
                        candidates.Add(new PolicyClaimCasePair() 
                            { 
                                PolicyNumber= reader.GetDbValueFromColumnName("EXTERNAL_POLICY_NUMBER"),
                                ClaimNumber= reader.GetDbValueFromColumnName("claim_number"),
                                CaseNumber = reader.GetDbValueFromColumnName("LITIGATION_ID")
                            }
                        );
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Log("Exception occurs querying DB for finding list of claims: " + ex.Message);
            }

            if (candidates == null)
            {
                Reporting.Error("Unable to find any NPE claims number(claim number starting with '2'). Please create any claim to continue with test automation.");
            }

            return candidates;
        }

        /// <summary>
        /// Fetching list of claim from all product types having case linked to it or not using the flag isClaimHavingCase
        /// </summary>
        /// <returns>List of claims created on NPE(claim number starting with '2')</returns>
        public static List<PolicyClaimCasePair> FindListOfClaimsForTCU(bool isClaimHavingCase = false)
        {
            var candidates = new List<PolicyClaimCasePair>();
            var queryParam = new Dictionary<string, string>();
            
            int isCaseLinkedClaim = isClaimHavingCase ? 0 : 1;
            queryParam.Add("isCaseLinkedClaim", isCaseLinkedClaim.ToString());

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\SearchForClaimAndCase.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParam);
                    while (reader.Read())
                    {
                        candidates.Add(new PolicyClaimCasePair()
                        {
                            PolicyNumber = reader.GetDbValueFromColumnName("PolicyNumber"),
                            ClaimNumber = reader.GetDbValueFromColumnName("ClaimNumber"),
                            CaseNumber = reader.GetDbValueFromColumnName("CaseNumber"),
                        }
                        );
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Log("Exception occurs querying DB for finding list of claims: " + ex.Message);
            }

            if (candidates == null)
            {
                Reporting.Error("Unable to find any NPE claims number(claim number starting with '2'). Please create any claim to continue with test automation.");
            }

            return candidates;
        }
        /// <summary>
        /// This method returns all correspondences linked to a Claim in Shield within the last 24 hours
        /// based on the Claim Number.
        /// </summary>
        /// <param name="claimNumber">Claim number for which correspondence is requested</param>
        /// <returns>list of attached correspondences from past 24 hours</returns>
        public static List<InboundClaimCorrespondence> GetClaimCorrespondence(string claimNumber)
        {
            var candidates = new List<InboundClaimCorrespondence>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\ClaimCorrespondence.sql");
                var queryParameter = new Dictionary<string, string>()
                {
                    { "claimNumber",  claimNumber}
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParameter);
                    while (reader.Read())
                    {
                        var correspondence = new InboundClaimCorrespondence()
                        {

                            FileName = reader.GetDbValueFromColumnName("FILE_NAME"),
                            DocType = reader.GetDbValueFromColumnName("DOC_TYPE"),
                            FileType = reader.GetDbValueFromColumnName("FILE_TYPE"),
                            CreationDate = DateTime.Parse(reader.GetDbValueFromColumnName("CREATION_DATE")),
                            IsActionable = Boolean.Parse(reader.GetDbValueFromColumnName("ISACTIONABLE")),
                            Remarks = reader.GetDbValueFromColumnName("REMARKS"),

                        };
                        candidates.Add(correspondence);
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Log("Exception occurs querying DB for getting claim correspondence: " + ex.Message);
            }

            return candidates;
        }


        /// <summary>
        /// Fetch the Shield description text for the Claim Scenario of
        /// a given claim. Suitable for a claim against any product type.
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <returns></returns>
        public static ClaimDetails GetClaimScenario(string claimNumber)
        {
            ClaimDetails result = null;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\GetClaimScenarioForLodgedClaim.sql");

                var queryClaimDetails = new Dictionary<string, string>()
                {
                    { "claimNumber", claimNumber }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryClaimDetails);
                    while (reader.Read())
                    {
                        result = new ClaimDetails()
                        {
                            ClaimType = reader.GetDbValue(1),
                            ClaimScenario = reader.GetDbValue(3),
                            EventDateAndTime = DateTime.ParseExact(reader.GetDbValue(6),
                                                                   $"{DataFormats.DATE_FORMAT_REVERSE_HYPHENS} {DataFormats.TIME_FORMAT_24HR}",
                                                                   CultureInfo.InvariantCulture),
                            PoliceReportNumber = reader.IsDBNull(4) ? null : reader.GetDbValue(4),
                            PoliceReportDate = reader.IsDBNull(5) ? 
                                               DateTime.Now : 
                                               DateTime.ParseExact(reader.GetDbValue(5),
                                                           DataFormats.DATE_FORMAT_REVERSE_HYPHENS,
                                                           CultureInfo.InvariantCulture)
                        };
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is SqlException || ex is ArgumentNullException || ex is FormatException)
            {
                Reporting.Log(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// For a policy of any product type, this method will return
        /// the number of claims against this policy that are either:
        /// *  currently not closed, OR
        /// *  have an event date within the past 12 months
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <returns>claim count matching the above criteria.</returns>
        public static int GetOpenClaimCountForPolicy(string policyNumber)
        {
            int count = 0;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\GetOpenClaimCountForAPolicy.sql");

                var queryClaimDetails = new Dictionary<string, string>()
                {
                    { "policynumber", policyNumber }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryClaimDetails);
                    while (reader.Read())
                    {
                        count = int.Parse(reader.GetDbValue(0));
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is SqlException || ex is ArgumentNullException || ex is FormatException)
            {
                Reporting.Error(ex.Message);
            }

            return count;
        }

        public static List<ContactClaimDB> GetClaimWitnessDetails(string claimNumber)
        {
            return GetContactsOnClaimByRole(claimNumber, ClaimantSide.PolicyHolder, ContactRole.Witness);
        }

        public static List<ContactClaimDB> GetClaimOffenderDetails(string claimNumber)
        {
            return GetContactsOnClaimByRole(claimNumber, ClaimantSide.ThirdParty, ContactRole.Claimant);
        }

        private static List<ContactClaimDB> GetContactsOnClaimByRole(string claimNumber, ClaimantSide sideId, ContactRole roleId)
        {
            var results = new List<ContactClaimDB>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\GetContactFromClaim.sql");
                var queryClaimDetails = new Dictionary<string, string>()
                {
                    { "claimantSideID", ClaimantSideIdentifiers[sideId].ToString() },
                    { "roleID",         ContactRoleIdentifiers[roleId].ToString() },
                    { "claimNumber",    claimNumber }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryClaimDetails);
                    while (reader.Read())
                    {
                        results.Add(new ContactClaimDB()
                        {
                            FirstName = reader.GetDbValue(0),
                            Surname = reader.GetDbValue(1),
                            DBTPRole = reader.GetDbValue(2),
                            DBTPEMailStatus = reader.GetDbValue(3),
                            DBTPPhoneStatus = reader.GetDbValue(4),
                            DBTPAddressStatus = reader.GetDbValue(5),
                            MailingAddress = new Address()
                            {
                                StreetOrPOBox = reader.GetDbValue(6),
                                Suburb = reader.GetDbValue(7)
                            }
                        });
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is SqlException || ex is ArgumentNullException || ex is FormatException)
            {
                Reporting.Log(ex.Message);
            }

            return results;
        }

        /// <summary>
        /// Retrieve claim number and contact id
        /// For fence damage open claim
        /// </summary>      
        /// <returns></returns>
        public static List<ClaimContact> GetOpenClaimContactForStormDamageToFenceOnly()
        {
            var results = new List<ClaimContact>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\GetOpenFenceOnlyClaim.sql");
                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        var claimNumber = reader.GetDbValue(0);
                        var beneficiary = new Contact(reader.GetDbValue(1));
                        results.Add(new ClaimContact(claimNumber, beneficiary));
                    }
                }
            }
            catch (Exception ex) when (ex is SqlException || ex is ArgumentNullException || ex is FormatException)
            { Reporting.Error("Can't get any claim"); }
            return results;
        }


        /// <summary>
        /// Retrieve all the uploaded file names for the claim.
        /// </summary>        
        public static List<string> GetUploadedInvoiceFileNames(string claimNumber)
        {
            var result = new List<string>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\GetUploadedInvoiceFileNames.sql");

                var queryClaimDetails = new Dictionary<string, string>()
                {
                    { "claimNumber", claimNumber }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryClaimDetails);
                    while (reader.Read())
                    {
                        result.Add(reader.GetDbValue(0));
                    }
                }
            }
            catch (Exception ex) when (ex is SqlException || ex is ArgumentNullException || ex is FormatException)
            {
                Reporting.Log(ex.Message);
            }

            return result;
        }
    }
}
