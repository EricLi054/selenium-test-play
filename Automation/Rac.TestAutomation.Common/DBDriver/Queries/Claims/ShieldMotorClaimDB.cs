using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace Rac.TestAutomation.Common.DatabaseCalls.Claims
{
    public class ShieldMotorClaimDB
    {
        /// <summary>
        /// Find a generic MFCO motor policy for motor claim scenario.
        /// Will include details of the primary PH.
        /// </summary>     
        /// <returns></returns>
        public static List<MotorPolicy> FindMotorPolicyWithNoExistingClaimsNoHireCar()
        {
            var candidates = new List<MotorPolicy>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyMotorForNewClaimByPHNoHireCar.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        candidates.Add(new MotorPolicy()
                        {
                            PolicyNumber = reader.GetDbValue(0),
                            PolicyHolders = ShieldPolicyDB.FetchPolicyContacts(reader.GetDbValue(0))
                        });
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Log("FindMotorPolicyWithNoExistingClaims Exception occurs querying DB: " + ex.Message);
            }

            return candidates;
        }

        /// <summary>
        /// Returns a motor policy for use in a motor claim scenario.
        /// Will return details for use in a member-match claim case
        /// (i.e. member does not know policy number).
        /// Requries MFCO cover was present in previous endorsement.
        /// </summary>
        /// <returns></returns>
        public static List<MotorPolicy> FindMotorPolicyWithMFCOInPrevEndorsementAnyPH()
        {
            var candidates = new List<MotorPolicy>(); // temp working list
            var result     = new List<MotorPolicy>(); // list to hold final results.

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyMotorForNewClaimByPHcoPH.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        var candidate = new MotorPolicy()
                        {
                            PolicyNumber = reader.GetDbValue(1)
                        };
                        var policyDetails = DataHelper.GetPolicyDetails(candidate.PolicyNumber);

                        var policyHolders = new List<PolicyContactDB>();
                        var policyHolder = new PolicyContactDB()
                        {
                            Id = policyDetails.Policyholder.Id.ToString(),
                            ExternalContactNumber = policyDetails.Policyholder.ContactExternalNumber,
                            ContactRoles = new List<ContactRole> { ContactRole.PolicyHolder }
                        };
                        policyHolders.Add(policyHolder);

                        if (policyDetails.PolicyCoOwners != null)
                        {
                            foreach (var coOwner in policyDetails.PolicyCoOwners)
                            {
                                var policyCoOwner = new PolicyContactDB();
                                policyCoOwner.Id = coOwner.Id.ToString();
                                policyCoOwner.ExternalContactNumber = coOwner.ContactExternalNumber;
                                policyCoOwner.ContactRoles = new List<ContactRole> { ContactRole.CoPolicyHolder };
                                policyHolders.Add(policyCoOwner);
                            }
                        }
                        candidate.PolicyHolders = policyHolders;
                        candidate.LastEndorsementDate = DateTime.Parse(reader.GetDbValue(6));
                        candidates.Add(candidate);                        
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            { 
                Reporting.Log("FindPolicyMotorForNewClaimByPHcoPHInPrevEndorsement Exception occurs querying DB: " + ex.Message);
            }

            foreach (var candidate in candidates)
            {
                if (ShieldPolicyDB.IsPolicySuitableForEndorsements(candidate.PolicyNumber) &&
                    string.Equals("MFCO", ShieldMotorDB.GetCoverForPreviousPolicyVersion(candidate.PolicyNumber)))
                {
                    candidate.Vehicle = ShieldMotorDB.FetchVehicleFromMotorPolicy(candidate.PolicyNumber);
                    result.Add(candidate);                    
                }
            }

            return result;
        }

        /// <summary>
        /// Looks for a motor policy to be used in motor claims.
        /// Finds motor policies paid annually and returns
        /// reference to either a PH or coPH on the policy to use as claimant.
        /// </summary>
        public static List<MotorPolicyEntity> GetMotorPoliciesForClaim()
        {
            var candidates = new List<MotorPolicyEntity>();
            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\GetMotorPoliciesForNewClaim.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    var i = 0;
                    while (reader.Read())
                    {
                        var coverType = reader.GetDbValueFromColumnName("motorCover");
                        candidates.Add(new MotorPolicyEntity
                        {
                            PartitionKey = $"pkey{i}",
                            RowKey       = $"rkey{i}",
                            PolicyNumber = reader.GetDbValueFromColumnName("policyNumber"),
                            CoverType    = MotorCoverIdMappings.FirstOrDefault(x => x.Value == coverType).Key.GetDescription(),
                            IsEV         = reader.GetDbValueFromColumnName("fuelType") == "4000001" ? true : false,
                            IsRegistrationValid = DataHelper.IsRegistrationNumberConsideredValid(reader.GetDbValueFromColumnName("Rego")) ? true : false,
                            PolicyStartDate = DateTime.Parse(reader.GetDbValueFromColumnName("PolicyStartDate")).ToUniversalTime()
                        });
                        i++;
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is NullReferenceException)
            {
                Reporting.Log("GetMotorPoliciesForNewClaim Exception occurs querying DB: " + ex.Message);
            }

            for (int i = candidates.Count; i > 0;)
            {
                i--;
                if (ShieldClaimDB.GetOpenClaimCountForPolicy(candidates[i].PolicyNumber) > 0)
                { candidates.RemoveAt(i); }
            }

            Reporting.IsTrue(candidates.Count > 0, "No suitable motor policies are found for lodging new motor claim");
            return candidates;
        }

        /// <summary>
        /// Looks for a motor policy to be used in motor claims.
        /// Finds motor policy with MFCO cover and had existing open claim lodged in last 1 month
        /// reference to either a PH or coPH on the policy to use as claimant.
        /// </summary>
        /// <returns></returns>
        public static List<MotorPolicyWithExistingClaim> FindMotorPoliciesWithExistingClaim()
        {
            var candidates = new List<MotorPolicyWithExistingClaim>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyMotorWithOpenClaim.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        candidates.Add(new MotorPolicyWithExistingClaim()
                        {
                            MotorPolicy = new MotorPolicy()
                            {
                                PolicyNumber = reader.GetDbValue(0),
                                PolicyHolders = ShieldPolicyDB.FetchPolicyContacts(reader.GetDbValue(0)),
                                Vehicle = ShieldMotorDB.FetchVehicleFromMotorPolicy(reader.GetDbValue(0))
                            },
                            ClaimNumber = reader.GetDbValue(1)
                        });
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Log("FindMotorPoliciesWithExistingClaim Exception occurs querying DB: " + ex.Message);
            }

            Reporting.IsTrue(candidates.Count > 0, "that a suitable motor policy, with an existing claim in th past month, was found for claim test");
            return candidates;
        }

        /// <summary>
        /// Looks for a motor policy to be used in motor claims.
        /// Finds motor policies are open and have unpaid installements
        /// lodge a claim, it will trigger payment block
        /// </summary>
        /// <returns></returns>
        public static List<MotorPolicy> FindMotorPoliciesHaveUnpaidInstallments()
        {
            var tempCandidates = new List<MotorPolicy>(); // temp working list
            var finalCandidates = new List<MotorPolicy>(); // list to hold final results.

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyMotorUnpaidInstallments.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        var candidate = new MotorPolicy()
                        {
                            PolicyNumber = reader.GetDbValue(0)
                        };
                        var policyDetails = DataHelper.GetPolicyDetails(candidate.PolicyNumber);

                        var policyHolders = new List<PolicyContactDB>();
                        var policyHolder = new PolicyContactDB()
                        {
                            Id = policyDetails.Policyholder.Id.ToString(),
                            ExternalContactNumber = policyDetails.Policyholder.ContactExternalNumber,
                            ContactRoles = new List<ContactRole> { ContactRole.PolicyHolder }
                        };
                        policyHolders.Add(policyHolder);

                        if (policyDetails.PolicyCoOwners != null)
                        {
                            foreach (var coOwner in policyDetails.PolicyCoOwners)
                            {
                                var policyCoOwner = new PolicyContactDB();
                                policyCoOwner.Id = coOwner.Id.ToString();
                                policyCoOwner.ExternalContactNumber = coOwner.ContactExternalNumber;
                                policyCoOwner.ContactRoles = new List<ContactRole> { ContactRole.CoPolicyHolder };
                                policyHolders.Add(policyCoOwner);
                            }
                        }
                        candidate.PolicyHolders = policyHolders;
                        candidate.Vehicle = ShieldMotorDB.FetchVehicleFromMotorPolicy(candidate.PolicyNumber);                       
                        tempCandidates.Add(candidate);
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Log("FindPolicyMotorForNewClaimByPHcoPHInPrevEndorsement Exception occurs querying DB: " + ex.Message);
            }

            foreach (var candidate in tempCandidates)
            {
                if (ShieldPolicyDB.IsPolicySuitableForEndorsements(candidate.PolicyNumber))
                {
                    finalCandidates.Add(candidate);
                }
            }

            return finalCandidates;
        }

        /// <summary>
        /// To support tests where an initial query has retrieved an initial list
        /// of policies to use in a motor claim. This query will pick a random
        /// policy and do some eligibility checks to see if its usable.
        /// Eligibility checks include:
        /// * no current claims
        /// * has no bad instalments that may block payments
        /// 
        /// If it is not suitable, then that entry will be discarded and it will
        /// move to the next item.
        /// 
        /// Once a suitable policy has been found, the additional policy details
        /// will be fetched from the database to support test activities.
        /// </summary>
        /// <param name="randomisePolicyHolder">
        /// If FALSE, use the policyholder details that are in the original policy
        /// items. If TRUE, look to randomly select a policyholder from the policy
        /// </param>
        public static MotorPolicy ReturnFirstPolicySuitableForClaims(List<MotorPolicy> policyList)
        {
            MotorPolicy policyToUse = null;
            do
            {
                var candidate = policyList.ConsumeRandom();

                if (ShieldPolicyDB.IsPolicySuitableForClaims(candidate.PolicyNumber))
                {
                    policyToUse = candidate;
                }
            } while ((policyList.Count > 0) && (policyToUse == null));
            
            Reporting.IsNotNull(policyToUse, "that a usable policy was found to use in claims");
            return policyToUse;
        }

        /// <summary>
        /// For a motor claim with an assigned repairer, this method will return
        /// flags indicating the various skills that the repairer has; hire car,
        /// fast track repair, etc.
        /// 
        /// Intended to support verification of information displayed to the claimant.
        /// </summary>
        /// <param name="claimNumber">Claim number of the motor claim.</param>
        /// <returns>NULL if no repairer assigned.</returns>
        public static MotorServiceProvider GetMotorRepairerSkillsViaClaim(string claimNumber)
        {
            MotorServiceProvider result = null;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\ClaimMotorGetRepairerSkills.sql");
                var queryClaimDetails = new Dictionary<string, string>()
                {
                    { "claimNumber", claimNumber }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryClaimDetails);
                    while (reader.Read())
                    {
                        result = new MotorServiceProvider()
                        {
                            ContactId    = int.Parse(reader.GetDbValue(0)),
                            ExternalContactNumber = reader.GetDbValue(1),
                            ProviderName = reader.GetDbValue(2),
                            ServiceArea  = reader.GetDbValue(3),
                            HasReadyDriveSkill    = int.Parse(reader.GetDbValue(5)),
                            HasFreeHirecar        = int.Parse(reader.GetDbValue(6)),
                            IsAutoAuthoriseRepairer = int.Parse(reader.GetDbValue(7)) > 0
                        };
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Log("ClaimMotorGetRepairerSkills Exception occurs querying DB: " + ex.Message);
            }

            return result;
        }

        public static ClaimAgendaStatus GetClaimAgendaStatus(string claimNumber, AgendaStepNames step)
        {
            ClaimAgendaStatus result = null;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Claims\\ClaimMotorGetAgendaStatus.sql");

                var queryClaimDetails = new Dictionary<string, string>()
                {
                    { "claimNumber", claimNumber },
                    { "agendaStep" , AgendaStepIdentifiers[step].ToString() }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryClaimDetails);
                    while (reader.Read())
                    {
                        result = new ClaimAgendaStatus()
                        {
                            DamageCode = reader.GetDbValue(0),
                            Step       = reader.GetDbValue(1),
                            Status     = reader.GetDbValue(2)
                        };
                        break;
                    }
                }
            }
            catch(Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Log(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Find open motor glass claims
        /// with policyholder claimant side
        /// and claim is lodged in last 90 days
        /// </summary>
        /// <returns></returns>
        public static List<string> GetOpenMotorGlassClaims()        
        {
            var result = new List<string>();
            string query = ShieldDB.ReadSQLFromFile("Claims\\GetOpenMotorGlassClaim.sql");

            using (var db = ShieldDB.GetDatabaseHandle())
            {
                var reader = db.ExecuteQuery(query, null);
                while (reader.Read())
                {
                    result.Add(reader.GetDbValue(0));
                }
            }
            return result;
        }

        /// <summary>
        /// Find closed motor glass claims
        /// with policyholder claimant side
        /// and claim is created 6 months before
        /// </summary>
        /// <returns></returns>
        public static List<string> GetClosedMotorGlassClaimsMoreThan6MonthsOld()
        {
            var result = new List<string>();
            string query = ShieldDB.ReadSQLFromFile("Claims\\GetCloseMotorGlassClaims.sql");

            using (var db = ShieldDB.GetDatabaseHandle())
            {
                var reader = db.ExecuteQuery(query, null);
                while (reader.Read())
                {
                    result.Add(reader.GetDbValue(0));
                }
            }
            return result;
        }

    }
}
