using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace Rac.TestAutomation.Common.DatabaseCalls.Claims
{
    public class ShieldHomeClaimDB
    {       

        /// <summary>
        /// Retrieve the Damage code, liability reserve amount and status of 
        /// the given Home Claim 
        /// </summary>
        /// <returns></returns>
        public static List<ClaimHomeDamageDetails> GetAffectedCoversAndReserves(string claimNumber)
        {
            var result = new List<ClaimHomeDamageDetails>();
            try
            {
                var query = ShieldDB.ReadSQLFromFile("Claims\\GetBasicClaimDetails.sql");

                var queryClaimDetails = new Dictionary<string, string>()
                {
                    { "claimNumber", claimNumber }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryClaimDetails);
                    while (reader.Read())
                    {
                        result.Add(new ClaimHomeDamageDetails
                        {
                            DamageCode = reader.GetDbValue(3),
                            ReserveAmount = reader.GetDbValue(7),
                            DamageStatus = reader.GetDbValue(4)

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Reporting.Log(ex.Message);
            }

            return result;
        }


        /// <summary>
        /// Retrieve the Damage code, Step and status of 
        /// the Home Claim Agenda type
        /// </summary>
        /// <param name="step">Agenda step for which the status is to be retrieved</param>
        /// <param name="damagedCover">string representation of the damage code (damage type and affected cover) to get for the given agenda step</param>
        /// <returns></returns>
        public static ClaimAgendaStatus GetClaimHomeAgendaStatus(string claimNumber, AgendaStepNames step, string damagedCover)
        {
            ClaimAgendaStatus result = null;

            try
            {
                var query = ShieldDB.ReadSQLFromFile("Claims\\GetClaimHomeAgendaStatus.sql");
            
                var queryClaimDetails = new Dictionary<string, string>()

                {
                    { "claimNumber" , claimNumber },
                    { "agendaStep"  , AgendaStepIdentifiers[step].ToString() },
                    { "damagedCover", damagedCover }
                };
               
                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryClaimDetails);
                    while (reader.Read())
                    {
                        result = new ClaimAgendaStatus()
                        {
                            DamageCode = reader.GetDbValue(0),
                            Step = reader.GetDbValue(1),
                            Status = reader.GetDbValue(2)
                        };
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Reporting.Log(ex.Message);
            }

            return result;
        }


        /// <summary>
        /// Retrieve fence claim agenda steps and status
        /// </summary>
        /// <param name="claimNumber">Claim Number        
        /// <returns></returns>
        public static Dictionary<string,string> GetHomeStormClaimAgendaStatus(string claimNumber)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {
                var query = ShieldDB.ReadSQLFromFile("Claims\\GetHomeStormClaimAgendaStatus.sql");

                var queryClaimDetails = new Dictionary<string, string>()

                {
                    { "claimNumber" , claimNumber }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryClaimDetails);
                    while (reader.Read())
                    {
                        result.Add(reader.GetDbValue(0), reader.GetDbValue(1));
                    }
                }
            }
            catch (Exception ex)
            {
                Reporting.Log(ex.Message);
            }

            return result;
        }


        /// <summary>
        /// Returns a list of home policies for use in a home claim
        /// scenario. Will return details for use in a member-match
        /// claim case (i.e. member does not know policy number).
        /// Requries Home cover was present in previous endorsement.
        /// </summary>
        /// <returns>list of policy numbers matching criteria.</returns>
        public static List<string> FindHomePoliciesWithBuildingAndContentsForNewClaim()
        {
            var candidates = new List<string>();
            
            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyHomeWithBuildingAndContentsForNewClaim.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        candidates.Add(reader.GetDbValueFromColumnName("PolicyNumber"));
                    }
                }
            }
            catch (Exception ex) { Reporting.Log("FindPolicyHomeWithBuildingAndContentsForNewClaim.sql Exception occurs querying DB: " + ex.Message); }

            return candidates;
        }

        private static List<string> FindHomePolicyWithPersonalValuablesCoverForClaim()
        {
            var policyNumbers = new List<string>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\FindHomePolicyWithPersonalValuablesCoverForClaim.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        policyNumbers.Add(reader.GetDbValueFromColumnName("PolicyNumber"));
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Log("FindHomePolicyWithPersonalValuablesCoverForClaim Exception occurs querying DB: " + ex.Message);
            }

            return policyNumbers;
        }

        public static List<HomePolicy> ReturnHomeOwnerPoliciesSuitableForClaimsPersonalValuables()
        {
            var policyNumbers = FindHomePolicyWithPersonalValuablesCoverForClaim();
            List<HomePolicy> result = new List<HomePolicy>();
            foreach (var policyNumber in policyNumbers)
            {
                var validPolicy = ShieldHomeDB.FetchHomePolicyDetailsForClaim(policyNumber);
                result.Add(validPolicy);
            }
            return result;
        }


        /// <summary>
        /// Returns all the valuables and contents attached to the
        /// identified home claim.
        /// </summary>
        public static List<ContentItem> GetContentsItemsFromClaim(string claimNumber)
        {
            var result = new List<ContentItem>();

            try
            {
                var query = ShieldDB.ReadSQLFromFile("Claims\\GetClaimHomeContentsItems.sql");

                var queryClaimDetails = new Dictionary<string, string>()
                {
                    { "ClaimNumber" , claimNumber }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryClaimDetails);
                    while (reader.Read())
                    {
                        var categoryString = reader.GetDbValue(2);
                        var categoryEnum   = SpecifiedPersonalValuablesDisplayedText.First(x => x.Value.TextShield == categoryString).Key;

                        result.Add(new ContentItem()
                        {
                            Value = int.Parse(reader.GetDbValue(1)),
                            Description = reader.GetDbValue(0),
                            Category = (int)categoryEnum
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Reporting.Log(ex.Message);
            }

            return result;
        }

        public static string GetClaimantEventDescriptionFromShield(string claimNumber)
        {
            string claimantEventDescription = "";

            try
            {
                var query = ShieldDB.ReadSQLFromFile("Claims\\GetHomeStormClaimantEventDescription.sql");

                var queryClaimDetails = new Dictionary<string, string>()

                {
                    { "claimNumber" , claimNumber }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryClaimDetails);
                    while (reader.Read())
                    {
                        claimantEventDescription = reader.GetDbValueFromColumnName("CLAIMANT_EVENT_DESCRIPTION");
                    }
                }
            }
            catch (Exception ex)
            {
                Reporting.Log("GetHomeStormClaimantEventDescription.sql Exception occurs querying DB: " + ex.Message);
            }
            return claimantEventDescription;
        }

        /// <summary>
        /// To support tests where an initial query has retrieved an initial list
        /// of policies to use in a storm related home claim. This query will pick
        /// a random policy and do some eligibility checks to see if its usable.
        /// Eligibility checks include:
        /// * no current claims
        /// * storm cover for current and previous endorsements of policy
        /// * has no bad instalments that may block payments
        /// 
        /// If it is not suitable, then that entry will be discarded and it will
        /// move to the next item.
        /// 
        /// Once a suitable policy has been found, the additional policy details
        /// will be fetched from the database to support test activities.
        /// </summary>
        public static string ReturnFirstPolicySuitableForStormClaims(List<string> policies)
        {
            string policyNumber = null;
            do
            {
                var candidate = policies.ConsumeRandom();

                try
                {
                    if (ShieldPolicyDB.IsPolicySuitableForClaims(candidate) &&
                        ShieldHomeDB.HomePolicyHadStormCoverPriorToLastEndorsement(candidate))
                    {
                        policyNumber = candidate;
                    }
                }
                // if there's an error processing this policy, then we don't use it, we'll go to the next item.
                catch
                {
                    Reporting.Log($"An issue occurred in attempting to verify state of {candidate}. Skipping.");
                }
            } while ((policies.Count > 0) && string.IsNullOrEmpty(policyNumber));

            Reporting.IsNotNull(policyNumber, "that a suitable policy was identified from policy list for claims test.");
            return policyNumber;
        }

        /// <summary>
        /// To support tests where an initial query has retrieved an initial list
        /// of policies to use in general home claim. This query will pick a
        /// random policy and do some eligibility checks to see if its usable.
        /// Eligibility checks include:
        /// * no current claims
        /// * has no bad instalments that may block payments
        /// 
        /// If it is not suitable, then that entry will be discarded and it will
        /// move to the next item.
        /// 
        /// The policy number of the first suitable policy found is returned.
        /// </summary>
        public static string ReturnFirstPolicySuitableForClaims(List<string> policyList)
        {
            string policyNumber = null;
            do
            {
                var candidatePolicyNumber = policyList.ConsumeRandom();

                try
                {
                    if (ShieldPolicyDB.IsPolicySuitableForClaims(candidatePolicyNumber))
                    {
                        policyNumber = candidatePolicyNumber;
                    }
                }
                // if there's an error processing this policy, then we don't use it, we'll go to the next item.
                catch
                {
                    Reporting.Log($"An issue occurred in attempting to get details for {candidatePolicyNumber}. Skipping.");
                }
            } while ((policyList.Count > 0) && string.IsNullOrEmpty(policyNumber));

            Reporting.IsNotNull(policyNumber, $"that a suitable policy ({policyNumber}) was identified from policy list for claims test.");
            return policyNumber;
        }

        /// <summary>
        /// To support tests where an initial query has retrieved an initial list
        /// of policies to use in general home claim. This query will do some 
        /// eligibility checks return all the home owner and landlord policies 
        /// which are eligible for an online storm claim.
        /// * no current claims
        /// * has no bad instalments that may block payments
        /// 
        /// If it is not suitable, then that entry will be discarded and it will
        /// move to the next item.
        /// 
        /// Once a suitable policy has been found, the additional policy details
        /// will be fetched from the database to support test activities.
        /// </summary>
        public static List<string> ReturnLandlordAndHomeOwnerPoliciesSuitableForClaims()
        {
            Reporting.Log($"Calling FindListOfHomePolicyNumbersLandlordAndHomeownerBuilding method");
            var policyList = ShieldHomeDB.FindListOfHomePolicyNumbersLandlordAndHomeownerBuilding();

            List<string> result = new List<string>();
            Reporting.Log($"DB query is complete and we have a list of candidate policy numbers");
            foreach (var policy in policyList) 
            {
                result.Add(policy);
            }
            
            return result;
        }

        /// <summary>
        /// To support tests where an initial query has retrieved a list
        /// of policies to use in general home claim. This query will do some 
        /// eligibility checks return all the home owner policies which are eligible 
        /// for online claim
        /// * no current claims
        /// * has no bad instalments that may block payments
        /// 
        /// If it is not suitable, then that entry will be discarded and it will
        /// move to the next item.
        /// 
        /// Once a suitable policy has been found, the additional policy details
        /// will be fetched from the database to support test activities.
        /// </summary>
        public static List<HomePolicy> ReturnHomeOwnerPoliciesSuitableForClaims()
        {
            var policyList = FindHomePoliciesWithBuildingAndContentsForNewClaim();
            List<HomePolicy> result = new List<HomePolicy>();
            foreach (var policy in policyList)
            {
                var validPolicy = ShieldHomeDB.FetchHomePolicyDetailsForClaim(policy);
                result.Add(validPolicy);                
            }
            return result;
        }
    

        /// <summary>
        /// For a home policy this method will return
        /// the number of claims against this policy that 
        /// meet the following criteria:
        /// *  damage type Storm & Tempest Damage to Fence Only
        /// *  event date is within the past 12 months
        /// </summary>
        /// <param name="policyNumber">The policy number to be investigated</param>
        /// <returns>claim count matching the above criteria.</returns>
        public static int GetFenceClaimCountForLastYear(string policyNumber)
        {
            int count = 0;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\GetFenceDamageStormClaimLastYear.sql");

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
            catch (Exception ex)
            {
                Reporting.Error(ex.Message);
            }
            if (count > 0)
            {
                Reporting.Log($"Found {count} fence claims for {policyNumber} in the last year, disqualifying it for online settlement.");
            }
            else
            {
                Reporting.Log($"Found {count} fence claims for {policyNumber} in the last year.");
            }
            return count;
        }
    }
}
