using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System.Collections.Generic;
using System;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using System.Data.SqlClient;
using System.IO;
using static Rac.TestAutomation.Common.Constants;

namespace Rac.TestAutomation.Common.DatabaseCalls.Policies
{
    public class ShieldPolicyDB
    {


        /// <summary>
        /// Returns basic payment details, such as method and frequency, for the given policy.
        /// </summary>
        /// <param name="policyNum"></param>
        /// <returns></returns>
        public static PolicyPaymentDetails FetchPaymentDetailsForPolicy(string policyNum)
        {
            PolicyPaymentDetails result = new PolicyPaymentDetails();

            try
            {
                string query          = ShieldDB.ReadSQLFromFile("Policies\\PolicyGetPaymentDetails.sql");
                var policyNumberQuery = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, policyNumberQuery);
                    while (reader.Read())
                    {
                        result.PaymentMethod    = reader.GetDbValue(0);
                        result.PaymentFrequency = DataHelper.GetValueFromDescription<PaymentFrequency>(reader.GetDbValue(1));
                        result.PaymentCount     = int.Parse(reader.GetDbValue(2));
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException) 
            { Reporting.Log("PolicyGetPaymentDetails Exception occurs querying DB: " + ex.Message); }

            return result;
        }

        public static bool PolicyHasExistingInstallmentDueToday(string policyNum)
        {
            bool foundInstallmentsDueToday = false;
            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\CheckCandidatePolicyForInstallmentsDueToday.sql");
                var policyNumberQuery = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, policyNumberQuery);
                    while (reader.Read())
                    {
                        foundInstallmentsDueToday = (int.Parse(reader.GetDbValue(0)) > 0);
                        break;
                    }
                }
            }
            catch (Exception e) when (e is SqlException || e is FormatException)
            {
                Reporting.Error($"SQL Exception: {e.Message}");
            }
            if (foundInstallmentsDueToday)
            {
                Reporting.Log($"Candidate {policyNum} disqualified for this test as an instalment with collection date matching current date was found.");
            }
            return foundInstallmentsDueToday;
        }

        /// <summary>
        /// Return details about a cancellation endorsement, including the email address
        /// submitted by the member for the policy during the cancellation process.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="contactId"></param>
        /// <returns>PolicyEndorsementCancellation</returns>
        public static PolicyEndorsementCancellation FetchCancellationDetailsForPolicy(string policyNumber, string contactId)
        {
            PolicyEndorsementCancellation result = new PolicyEndorsementCancellation();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\PolicyGetCancellationDetails.sql");

                // Combine the policyNumber and contact parameters together
                var queryPolicyNumber = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);
                var queryContactId = ShieldDB.SetSqlParameterForContactID(contactId);
                var queryParameters = queryPolicyNumber.Union(queryContactId).ToDictionary(k => k.Key, v => v.Value);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParameters);
                    while (reader.Read())
                    {
                        result.PolicyStatusId = int.Parse(reader.GetDbValue(2));
                        result.PolicyEndorsementExternalCode = reader.GetDbValue(5);
                        result.FinalInstallment = decimal.Parse(reader.GetDbValue(8));
                        result.CancellationEffectiveDate = DateTime.ParseExact(reader.GetDbValue(12),$"{DataFormats.DATE_FORMAT_REVERSE_HYPHENS} {DataFormats.TIME_FORMAT_24HR_WITH_SECONDS}", System.Globalization.CultureInfo.InvariantCulture);
                        result.InitiatorId = int.Parse(reader.GetDbValue(13));
                        result.Email = reader.GetDbValue(18);
                        result.PrintEventGenerated = bool.Parse(reader.GetDbValue(19));
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            { Reporting.Log("PolicyGetCancellationDetails Exception occurs querying DB: " + ex.Message); }

            return result;
        }

        /// <summary>
        /// Returns details about an endorsement when a member updates how they pay for 
        /// a policy. Includes the email address.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public static PolicyEndorsementInstalmentUpdate FetchPaymentUpdateDetailsForPolicy(string policyNumber, string contactId)
        {
            PolicyEndorsementInstalmentUpdate result = new PolicyEndorsementInstalmentUpdate();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\GetPaymentEndorsementDetails.sql");

                // Combine the policyNumber and contact parameters together
                var queryPolicyNumber = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);
                var queryContactId = ShieldDB.SetSqlParameterForContactID(contactId);
                var queryParameters = queryPolicyNumber.Union(queryContactId).ToDictionary(k => k.Key, v => v.Value);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParameters);
                    while (reader.Read())
                    {
                        result.PolicyEndorsementIdentifier = reader.GetDbValue(4);
                        result.PolicyEndorsementExternalCode = reader.GetDbValue(5);
                        result.Email = reader.GetDbValue(10);
                        result.PrintEventGenerated = bool.Parse(reader.GetDbValue(11));
                        result.PaymentChangeEventGenerated = bool.Parse(reader.GetDbValue(14));
                        result.EndorsementForDebitAdmendment = bool.Parse(reader.GetDbValue(17));
                        result.EventCount = int.Parse(reader.GetDbValue(20));
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            { Reporting.Log("GetPaymentEndorsementDetails Exception occurs querying DB: " + ex.Message); }

            return result;
        }

        /// <summary>
        /// Returns the details about the next pending instalment for the supplied policy number
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <returns>PolicyInstalmentDetails with amount due, current and original collection date</returns>
        public static PolicyInstalmentDetails FetchNextInstalmentDetails(string policyNumber)
        {
            var result = new PolicyInstalmentDetails();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\GetNextInstalmentDetails.sql");
                var queryParameters = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParameters);
                    while (reader.Read())
                    {
                        result.AmountDue = reader.GetDbValue(1) != null ? decimal.Parse(reader.GetDbValue(1)) : 0;
                        result.CurrentCollectionDate = reader.GetDbValue(2) != null ? DateTime.ParseExact(reader.GetDbValue(2), DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH, System.Globalization.CultureInfo.InvariantCulture) : DateTime.MinValue;
                        result.OriginalCollectionDate = reader.GetDbValue(3) != null ? DateTime.ParseExact(reader.GetDbValue(3), DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH, System.Globalization.CultureInfo.InvariantCulture) : DateTime.MinValue;

                        // Based on type of account, populate either bank account or credit card 
                        if (reader.GetDbValue(4) == "1")
                        {
                            
                            result.BankAccount = new BankAccount() {
                                Bsb = reader.GetDbValue(5),
                                AccountNumber = reader.GetDbValue(6),
                                AccountName = reader.GetDbValue(7)
                            };
                        }
                        else 
                        {
                            result.CreditCard = new CreditCard()
                            {
                                CardNumber = reader.GetDbValue(6),
                                CardIssuer = DataHelper.GetValueFromDescription<CreditCardIssuer>(reader.GetDbValue(8)),
                                CardholderName = reader.GetDbValue(9),
                                CardExpiryDate = DateTime.ParseExact(reader.GetDbValue(10),
                                DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH, System.Globalization.CultureInfo.InvariantCulture)
                            };
                        }

                        break;
                    }
                }
            }
            catch(Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Checks the account names linked to a contact.  Should an account name contain 
        /// invalid characters such as brackets, be null or an incorrrect length, then 
        /// method returns true.
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public static bool HasInvalidCharactersOrIncorrectLengthInContactAccountNames(string contactId)
        {
            var hasLinkedAccountWithInvalidAccountName = false;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\CheckForLinkedActiveAccountsWithInvalidName.sql");
                var queryParameters = ShieldDB.SetSqlParameterForContactID(contactId);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParameters);
                    while (reader.Read())
                    {
                        hasLinkedAccountWithInvalidAccountName = true;
                        break;
                    }
                }
            }
            catch (Exception e) when (e is ArgumentException || e is IOException || e is SqlException )
            {
                Reporting.Error($"SQL error encountered: {e.Message}");
            }
            return hasLinkedAccountWithInvalidAccountName;

        }

        public static void FetchShieldEventDetailsOnPolicy(string policyNum, out string shieldUser, out string eventName, out string loggedInB2CContactName)
        {
            shieldUser = null;
            eventName = null;
            loggedInB2CContactName = null;

            try
            {
                string query          = ShieldDB.ReadSQLFromFile("Policies\\GetShieldEventDetailsOnPolicy.sql");
                var policyNumberQuery = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, policyNumberQuery);
                    while (reader.Read())
                    {
                        shieldUser             = reader.GetDbValue(0);
                        loggedInB2CContactName = reader.GetDbValue(1);
                        eventName              = reader.GetDbValue(2);
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            { Reporting.Log("GetShieldEventDetailsOnPolicy Exception occurs querying DB: " + ex.Message); }
        }

        /// <summary>
        /// Returns list of policies for the given product type that have monthly credit card instalments.
        /// It provides details about the amount, the credit card details and the next instalment date.
        /// Phone number are returned to help excluded potential problems with bad data.
        /// </summary>
        /// <param name="productType">Type of policy</param>
        public static List<string> FindPolicyPaidMonthlyViaCreditCardForChangeHowYouPay(ShieldProductType productType)
        {
            var candidatePolicies      = new List<string>();
            var candidatePolicyHolders = new List<string>();
            var result  = new List<string>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyDetailsForMidTermMonthlyCreditCard.sql");
                var queryProductIdParam = new Dictionary<string, string>()
                {
                    { "productId", ((int)productType).ToString()}
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryProductIdParam);
                    while (reader.Read())
                    {
                        try
                        {
                            candidatePolicies.Add(reader.GetDbValueFromColumnName("PolicyNumber"));
                            candidatePolicyHolders.Add(reader.GetDbValueFromColumnName("ContactID"));

                            // Must have valid phone number set. Which is a mobile number OR a home phone number
                            // with an empty/null mobile.  Partial mobile number cause problems
                            if (!DataHelper.HasValidPhoneNumberSet(mobile: reader.GetDbValueFromColumnName("MobilePhone"),
                                                                   homePhone: reader.GetDbValueFromColumnName("HomePhone")))
                            {
                                continue;
                            }
                        }
                        // We're getting more bad data items in Shield (e.g. Contacts without mandatory gender)
                        // and they result in parsing exceptions. Skip and go to next candidate.
                        catch (Exception ex) when ( ex is SqlException || ex is IOException || ex is ArgumentException)
                        { Reporting.Log("Error getting value from column: " + ex.Message); }
                    }
                }
            }
            catch  (Exception e) when (e is ArgumentException || e is IOException || e is SqlException )
            {
                Reporting.Error($"SQL error encountered: {e.Message}");
            }

            for (int i = 0; i < candidatePolicies.Count; i++)
            {
                // Exclude candidate where there is a linked active bank account that has an invalid account name.
                // For example brackets in the account name when submitted to the Insurance Contact Service (ICS) to throw errors
                if (HasInvalidCharactersOrIncorrectLengthInContactAccountNames(candidatePolicyHolders[i]))
                {
                    continue;
                }
                result.Add(candidatePolicies[i]);
            }

            Reporting.IsTrue(result.Count > 0, "that we found at least one suitable policy matching test criteria");
            return result;
        }

        /// <summary>
        /// Returns list of policies for the given product type that have monthly credit card instalments.
        /// It provides details about the amount, the credit card details and the next instalment date.
        /// Phone number are returned to help excluded potential problems with bad data.
        /// </summary>
        /// <param name="productType"></param>
        public static List<string> FindPolicyPaidMonthlyViaBankDebit(ShieldProductType productType)
        {
            var candidatePolicies      = new List<string>();
            var candidatePolicyHolders = new List<string>();
            var result = new List<string>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyDetailsForMidTermMonthlyBankDebit.sql");
                var queryProductIdParam = new Dictionary<string, string>()
                {
                    { "productId", ((int)productType).ToString()}
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryProductIdParam);
                    while (reader.Read())
                    {
                        try
                        {
                            // Must have valid phone number set. Which is a mobile number OR a home phone number
                            // with an empty/null mobile.  Partial mobile number cause problems
                            if (!DataHelper.HasValidPhoneNumberSet(mobile: reader.GetDbValueFromColumnName("MobilePhone"),
                                                                   homePhone: reader.GetDbValueFromColumnName("HomePhone")))
                            {
                                continue;
                            }

                            candidatePolicies.Add(reader.GetDbValueFromColumnName("PolicyNumber"));
                            candidatePolicyHolders.Add(reader.GetDbValueFromColumnName("ContactID"));
                        }
                        // In the event of a parsing exception of data from Shield (e.g. missing gender)
                        // then skip and go to next candidate.
                        catch (SqlException ex)
                        { Reporting.Log("Error getting value from column: " + ex.Message); }
                    }
                }
            }
            catch (Exception e) when (e is SqlException || e is ArgumentException || e is IOException )
            {
                Reporting.Error($"SQL error encountered: {e.Message}");
            }

            for (int i = 0; i < candidatePolicies.Count; i++)
            {
                // Exclude candidate where there is a linked active bank account that has an invalid account name.
                // For example brackets in the account name when submitted to the Insurance Contact Service (ICS) to throw errors
                if (HasInvalidCharactersOrIncorrectLengthInContactAccountNames(candidatePolicyHolders[i]))
                {
                    continue;
                }
                result.Add(candidatePolicies[i]);
            }

            Reporting.IsTrue(result.Count > 0, "that we found at least one suitable policy matching test criteria");
            return result;
        }

        /// <summary>
        /// Returns TRUE if policy has no "bad" instalments (blocked,
        /// on hold, rejected), has no current open claims, and has
        /// no submitted instalments.
        /// </summary>
        /// <returns></returns>
        public static bool IsPolicySuitableForClaims(string policyNumber) =>
            !PolicyHasBadInstallments(policyNumber) &&
            !PolicyHasSubmittedInstallments(policyNumber) &&
            !PolicyHasPendingInstallmentsRelativeToDate(policyNumber, DateTime.Now) &&
            ShieldClaimDB.GetOpenClaimCountForPolicy(policyNumber) == 0;

        /// <summary>
        /// Returns TRUE if policy has no "bad" instalments (blocked,
        /// on hold, rejected) and has no submitted instalments.
        /// </summary>
        /// <returns></returns>
        public static bool IsPolicySuitableForEndorsements(string policyNumber) =>
            !PolicyHasBadInstallments(policyNumber) &&
            !PolicyHasPendingInstallmentsRelativeToDate(policyNumber, DateTime.Now) &&
            !PolicyHasSubmittedInstallments(policyNumber) &&
            !PolicyHasExistingInstallmentDueToday(policyNumber) &&
            PolicyIsAnnualCashOrPayerIsAPolicyholder(policyNumber) // TODO ISE-9723: Review the need for this step when ISE-9723 is resolved.
            ;

        /// <summary>
        /// Returns TRUE if policy has no "bad" instalments (blocked,
        /// on hold, rejected), has no fence claim lodged in last 1 year, and has
        /// no submitted instalments.
        /// 
        /// Note that even when we our expected outcome is that the CLAIM will not 
        /// be eligible for online settlement, we still need to start with a 
        /// Policy which would be eligible so we know that the reason it was ruled
        /// out is in the detail of the Claim itself.
        /// </summary>
        /// <returns></returns>
        public static bool IsPolicySuitableForFenceClaims(string policyNumber) =>
            !PolicyHasBadInstallments(policyNumber) &&
            !PolicyHasSubmittedInstallments(policyNumber) &&
            !PolicyHasPendingInstallmentsRelativeToDate(policyNumber, DateTime.Now) &&
            ShieldHomeClaimDB.GetFenceClaimCountForLastYear(policyNumber) == 0;

        /// <summary>
        /// Return TRUE if the policy has at least one existing bank account which
        /// is being used to pay a current policy so we know that the claimant who
        /// is reporting a claim will have options to select from on the Bank 
        /// Account page (e.g. Home Storm claims).
        /// </summary>
        /// <param name="claimant">The Contact ID of the claimant</param>
        /// <param name="policyNumber">The candidate policy number under consideration</param>
        /// <returns></returns>
        public static bool ClaimantHasActiveBankAccount(PolicyContactDB claimant, string policyNumber)
        { 
            bool foundActiveBankAccounts = false;
            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\PolicyContactHasActiveBankAccount.sql");
                var queryParameters = new Dictionary<string, string>();
                queryParameters.Add($"contactId", claimant.Id);
                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParameters);
                    while (reader.Read())
                    {
                        foundActiveBankAccounts = (int.Parse(reader.GetDbValue(0)) > 0);
                        break;
                    }
                }
            }
            catch (SqlException e)
            {
                Reporting.Error($"SQL Exception: {e.Message}");
            }
            if (!foundActiveBankAccounts)
            {
                Reporting.Log($"Candidate claimant {claimant.Id} for {policyNumber} disqualified for this test as the claimant does not possess any Bank Accounts which are actively paying for policies.");
            }
            else
            {
                Reporting.Log($"Candidate claimant {claimant.Id} for {policyNumber} qualifies for this test as they have at least one active bank account which is paying for a policy.");
            }
            return foundActiveBankAccounts;
        }

        /// <summary>
        /// Method to query whether the policy has any installments
        /// on the policy which are in a undesirable state. These
        /// are installments which are "rejected", "account blocked" 
        /// or "On Hold".
        /// 
        /// Any undesirable installment will affect the ability to
        /// carry out processes in PCM such as one-step claims,
        /// endorsements and renewals.
        /// </summary>
        /// <param name="policyNum"></param>
        /// <returns>true if any bad installments found.</returns>
        public static bool PolicyHasBadInstallments(string policyNum)
        {
            var foundBadInstallments = false;

            string query          = ShieldDB.ReadSQLFromFile("Policies\\GetPolicyBadInstallmentsCount.sql");
            var policyNumberQuery = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

            using (var db = ShieldDB.GetDatabaseHandle())
            {
                var reader = db.ExecuteQuery(query, policyNumberQuery);
                while (reader.Read())
                {
                    foundBadInstallments = ((int.Parse(reader.GetDbValue(0)) +
                                             int.Parse(reader.GetDbValue(1))) > 0);
                    break;
                }
            }
            if (foundBadInstallments)
            {
                Reporting.Log($"Candidate {policyNum} disqualified for this test as an instalment was found with a status of: Pending, Rejected, Booked or Account Blocked.");
            }
            return foundBadInstallments;
        }

        /// <summary>
        /// Mostly to support claims scenarios, this allows us to check whether the given
        /// policy has any instalments in PENDING status prior to a given date.
        /// This will assist in avoiding unexpected claims irregularities which block
        /// one-step claims scenarios.
        /// </summary>
        /// <param name="policyNum"></param>
        /// <param name="cutoffDate"></param>
        /// <returns></returns>
        public static bool PolicyHasPendingInstallmentsRelativeToDate(string policyNum, DateTime cutoffDate)
        {
            var hasOverdueInstalments = false;

            string query = ShieldDB.ReadSQLFromFile("Policies\\GetPolicyHistoricalPendingInstalmentCounts.sql");
            var queryParameters = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);
            queryParameters.Add("datestring", cutoffDate.ToString("yyyy/M/d"));

            using (var db = ShieldDB.GetDatabaseHandle())
            {
                var reader = db.ExecuteQuery(query, queryParameters);
                while (reader.Read())
                {
                    hasOverdueInstalments = ((int.Parse(reader.GetDbValue(0)) +
                                             int.Parse(reader.GetDbValue(1))) > 0);
                    break;
                }
            }
            if (hasOverdueInstalments)
            {
                Reporting.Log($"Candidate {policyNum} disqualified for this test as an overdue PENDING instalment was found.");
            }
            return hasOverdueInstalments;
        }

        /// <summary>
        /// Method to query whether the policy has any installments
        /// on the policy which are in "submitted" state (typical
        /// of policies which are currently on the collection day of
        /// an unpaid cash instalment).
        /// 
        /// For any test scenario which is NOT last day renewal for
        /// cash policies, this is considered an undesiable instalment.
        /// </summary>
        /// <param name="policyNum"></param>
        /// <returns>true if any submitted installments found.</returns>
        private static bool PolicyHasSubmittedInstallments(string policyNum)
        {
            var foundSubmittedInstallments = false;

            string query          = ShieldDB.ReadSQLFromFile("Policies\\GetPolicySubmittedInstallmentsCount.sql");
            var policyNumberQuery = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

            using (var db = ShieldDB.GetDatabaseHandle())
            {
                var reader = db.ExecuteQuery(query, policyNumberQuery);
                while (reader.Read())
                {
                    foundSubmittedInstallments = (int.Parse(reader.GetDbValue(0)) > 0);
                    break;
                }
            }
            if(foundSubmittedInstallments)
            {
                Reporting.Log($"Candidate {policyNum} disqualified for this test as at least one Submitted instalment was found.");
            }
            return foundSubmittedInstallments;
        }

        /// <summary>
        /// Method to query whether the linked bank account (or credit card)
        /// to the given policy, is owned by a policyholder. If there is no
        /// linked bank account/credit card, which occurs for Annual Cash,
        /// then we also return true
        /// TODO (ISE-9723): Review the need for this method when ISE-9723 is resolved.
        /// </summary>
        private static bool PolicyIsAnnualCashOrPayerIsAPolicyholder(string policyNum)
        {
            var isPayerAPolicyholder = false;
            var isAnnualCash = false;

            string query = ShieldDB.ReadSQLFromFile("Policies\\GetPolicyPayerRoleStatus.sql");
            var policyNumberQuery = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

            using (var db = ShieldDB.GetDatabaseHandle())
            {
                var reader = db.ExecuteQuery(query, policyNumberQuery);
                while (reader.Read())
                {
                    isPayerAPolicyholder = (int.Parse(reader.GetDbValueFromColumnName("IsPayeePolicyholder")) > 0);
                    isAnnualCash = (int.Parse(reader.GetDbValueFromColumnName("IsAnnualCash")) > 0);
                    break;
                }
            }
            return isPayerAPolicyholder || isAnnualCash;
        }

        /// <summary>
        /// Retrieve in list the recent events of a given policy number
        /// </summary>
        /// <param name="policyNum"></param>
        /// <param name="fetchRecentEvents"></param>
        /// <returns></returns>
        public static List<string> FetchRecentEventsOnPolicy( string policyNum)
        {
            var results  = new List<string>();
            string query = ShieldDB.ReadSQLFromFile("Policies\\GetPolicyRecentEvents.sql");

            var policyNumberQuery = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

            using (var db = ShieldDB.GetDatabaseHandle())
            {
                var reader = db.ExecuteQuery(query, policyNumberQuery);
                while (reader.Read())
                {
                    results.Add(reader.GetDbValue(0));
                }
            }

            return results;
        }

        /// <summary>
        /// Retrieve in list the endorsement reason of a given policy number
        /// </summary>
        public static List<PolicyEndorsementReason> FetchEndorsementReasonDetailsOnPolicy(string policyNumber)
        {
            var results = new List<PolicyEndorsementReason>();

            try
            {
                var query = ShieldDB.ReadSQLFromFile("Policies\\GetPolicyEndorsementDetails.sql");
                var queryParams = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParams);

                    while (reader.Read())
                    {
                        var reason = new PolicyEndorsementReason
                        {
                            Id = reader.GetDbValueFromColumnName("ENDORSMENT_REASON_ID"),
                            Description = reader.GetDbValueFromColumnName("ENDORSMENT_REASON"),
                            Remarks = reader.GetDbValueFromColumnName("REMARKS"),
                            EndorsementType = reader.GetDbValueFromColumnName("ENDORSMENT_TYPE")
                        };

                        results.Add(reason);
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            { Reporting.Error($"GetPolicyEndorsementDetails Exception occurs querying DB for the policy number {policyNumber}: " + ex.Message); }

            return results;
        }

        /// <summary>
        /// Retrieves all instalments payment status (Annual/Monthly) of a given policy number
        /// from today onwards.
        /// </summary>
        /// <param name="policyNum"></param>
        /// <returns></returns>
        public static List<PolicyInstalmentDB> FetchPaymentStatusOnPolicy(string policyNum)
        {
            var results  = new List<PolicyInstalmentDB>();
            string query = ShieldDB.ReadSQLFromFile("Policies\\GetPolicyPaymentStatusFromToday.sql");

            var policyNumberQuery = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

            using (var db = ShieldDB.GetDatabaseHandle())
            {
                var reader = db.ExecuteQuery(query, policyNumberQuery);
                while (reader.Read())
                {
                    var instalment = new PolicyInstalmentDB()
                    {
                        Status = DataHelper.GetValueFromDescription<Status>(reader.GetDbValue(3)),
                        Amount = decimal.Parse(reader.GetDbValue(2)),
                        CollectionDate = DateTime.ParseExact(reader.GetDbValue(1),
                                                             DataFormats.DATE_ABBREVIATED_MONTH_FORWARD_WHITESPACE,
                                                             System.Globalization.CultureInfo.InvariantCulture),
                        IsRecurring = reader.GetDbValue(4).Equals("true", StringComparison.InvariantCultureIgnoreCase)
                    };

                    results.Add(instalment);
                }
            }

            return results;
        }

        /// <summary>
        /// Returns the list of policy holders and co-policy holders on a policy.
        /// 
        /// NOTE: List is not ordered to match sequence of entry into B2C.
        /// </summary>
        /// <param name="policyNum"></param>
        /// <returns>Empty list if there are no results</returns>
        public static List<PolicyContactDB> FetchPolicyContacts(string policyNum)
        {
            List<PolicyContactDB> result = new List<PolicyContactDB>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\PolicyGetPolicyHolders.sql");
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        var contact = new PolicyContactDB()
                        {
                            TitleString = reader.GetDbValueFromColumnName("Title"),
                            FirstName = reader.GetDbValueFromColumnName("FirstName"),
                            MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null
                                        : reader.GetDbValueFromColumnName("MiddleName"),
                            Surname = reader.GetDbValueFromColumnName("Surname"),
                            DateOfBirth = DateTime.Parse(reader.GetDbValueFromColumnName("DOB")),
                            MailingAddress = new Address()
                            {
                                StreetNumber = reader.GetDbValueFromColumnName("HouseNumber"),
                                StreetOrPOBox = reader.GetDbValueFromColumnName("Street"),
                                Suburb = reader.GetDbValueFromColumnName("Suburb"),
                                PostCode = reader.GetDbValueFromColumnName("Postcode")
                            },
                            Id = reader.GetDbValueFromColumnName("ContactId"),
                            ExternalContactNumber = reader.GetDbValueFromColumnName("ContactExternalNumber"),
                            ContactRoles = new List<ContactRole>()
                        };

                        contact.PrivateEmail = reader.IsDBNull(reader.GetOrdinal("PrivateEmail")) ? null
                            : new Email() { Address = reader.GetDbValueFromColumnName("PrivateEmail") };

                        contact.MobilePhoneNumber = reader.IsDBNull(reader.GetOrdinal("MobilePhone")) ? null
                            : reader.GetDbValueFromColumnName("MobilePhone");
                        contact.HomePhoneNumber  = reader.IsDBNull(reader.GetOrdinal("HomePhone")) ? null
                            : reader.GetDbValueFromColumnName("HomePhone");

                        var roleString = reader.GetDbValueFromColumnName("PolicyContactRole");

                        try
                        {
                            contact.ContactRoles.Add(DataHelper.GetValueFromDescription<ContactRole>(roleString));
                        }
                        catch(Exception ex) when (ex is ArgumentException || ex is NotSupportedException)
                        {
                            Reporting.Error($"When parsing policy contacts, failed to process role: {roleString} for contact: {contact.FirstName}:{ex.Message}");
                        }

                        result.Add(contact);
                    }
                }
            }
            catch (Exception e) when (e is ArgumentException || e is IOException || e is NotSupportedException || e is SqlException || e is FormatException)
            {
                Reporting.Error($"SQL error encountered: in FetchPolicyContacts for {policyNum} {e.Message}");
            }

            return result;
        }

        public static List<string> FindRejectedInstallmentForPayNow(ShieldProductType productId, PaymentScenario paymentScenario)
        {
            var policyNumbers = new List<string>();

            var collectionMethod = paymentScenario == PaymentScenario.AnnualBank ?
                                   (int)PaymentScenario.AnnualBank :
                                   (int)PaymentScenario.AnnualCard;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindRejectedInstallmentForPayNow.sql");
                var queryParameter = ShieldDB.SetSqlParameterForProductId(((int)productId).ToString());
                queryParameter.Add("collectionMethod", collectionMethod.ToString());

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParameter);
                    while (reader.Read())
                    {
                        var policyNum = reader.GetDbValue(reader.GetOrdinal("POLICY_NUMBER"));
                        policyNumbers.Add(policyNum);
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            Reporting.IsTrue(policyNumbers.Count > 0, "that we found at least one suitable policy matching test criteria");
            return policyNumbers;
        }

        /// <summary>
        /// Returns list of eligible policies for "PayNow" in renewal or midterm state
        /// And Policy is required to be payed using "PayNow" option with Annual Cash
        /// Pass the required set of installment status on 'installmentStatus' param as List. The query will filter those list of policies from the DB
        /// </summary>
        /// <param name="isRenewal">Set this true if renewal policy is required, else false for midterm</param>
        /// <param name="productId">Set the required product type</param>
        /// <param name="installmentStatus">Set the return policy comes under this installment status</param>
        /// <returns>Policy number</returns>
        public static List<string> FindPolicyForPayNow(ShieldProductType productId, MakePaymentScenarioType productType, List<InstallmentStatus> installmentStatus)
        {
            var policyNumbers = new List<string>();

            var paymentType1 = installmentStatus.Contains(InstallmentStatus.Pending) ? (int)InstallmentStatus.Pending : 0;
            var paymentType2 = installmentStatus.Contains(InstallmentStatus.Submitted) ? (int)InstallmentStatus.Submitted : 0;
            var paymentType3 = installmentStatus.Contains(InstallmentStatus.Rejected) ? (int)InstallmentStatus.Rejected : 0;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindAnnualCashPoliciesInPayNowState.sql");
                var queryParameter = ShieldDB.SetSqlParameterForProductId(((int)productId).ToString());
                queryParameter.Add("paymentType1", paymentType1.ToString());
                queryParameter.Add("paymentType2", paymentType2.ToString());
                queryParameter.Add("paymentType3", paymentType3.ToString());

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParameter);
                    while (reader.Read())
                    {
                        var policyNum = reader.GetDbValue(reader.GetOrdinal("POLICY_NUMBER"));
                        var productState = reader.GetDbValue(reader.GetOrdinal("POLICY_STATE"));

                        // Add data that matches our criteria.
                        if (productType.Equals(MakePaymentScenarioType.RENEWAL) && productState == "RENEWAL")
                        {
                            policyNumbers.Add(policyNum);
                        }else if(productType.Equals(MakePaymentScenarioType.MID_TERM) && productState == "MID_TERM")
                        {
                            policyNumbers.Add(policyNum);
                        }else if(productType.Equals(MakePaymentScenarioType.NEW_BUSINESS) && productState == "NEW_BUSINESS")
                        {
                            policyNumbers.Add(policyNum);
                        }else if (productType.Equals(MakePaymentScenarioType.ANY))// This will get pol in any state eligible for MAP
                        {
                            policyNumbers.Add(policyNum);
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            Reporting.IsTrue(policyNumbers.Count > 0, "that we found at least one suitable policy matching test criteria");
            return policyNumbers;
        }

        /// <summary>
        /// Returns the Shield IDs to use in a parameterised SQL query for a given
        /// policy payment method.
        /// </summary>
        /// <param name="paymentScenario"></param>
        /// <returns>Dictionary with keys: "paymentterm" and "collectionmethod"</returns>
        public static Dictionary<string, string> GetShieldPaymentMethodParameters(PaymentScenario paymentScenario)
        {
            return new Dictionary<string, string>()
            {
                { "paymentterm",      PaymentScenarioIdMappings[paymentScenario].PaymentTerm },
                { "collectionmethod", PaymentScenarioIdMappings[paymentScenario].CollectionMethod }
            };
        }

        public static bool HasDesiredAccountingInfoForRefundToSource(string policyNum, SparkCommonConstants.RefundToSource desiredRefundType)
        {
            bool willSupportDesiredRefundToSourceType = false;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\PolicyGetRefundToSourceInfo.sql");
                var policyNumberQuery = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, policyNumberQuery);
                    while (reader.Read())
                    {
                        switch (desiredRefundType)
                        {
                            case SparkCommonConstants.RefundToSource.RefundToBankAccount:
                                willSupportDesiredRefundToSourceType = reader.GetDbValueFromColumnName("RefBank").Equals("1");
                                break;
                            case SparkCommonConstants.RefundToSource.RefundToUnknownCreditCard:
                                willSupportDesiredRefundToSourceType = reader.GetDbValueFromColumnName("RefUnknCC").Equals("1");
                                break;
                            case SparkCommonConstants.RefundToSource.RefundToKnownCreditCard:
                                willSupportDesiredRefundToSourceType = reader.GetDbValueFromColumnName("RefKnwnCC").Equals("1");
                                break;
                            default:
                                Reporting.Error($"Invalid desired refund type: {desiredRefundType}");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            { Reporting.Log($"PolicyGetRefundToSourceInfo Exception occurs querying DB for the policy number {policyNum}: " + ex.Message); }

            return willSupportDesiredRefundToSourceType;
        }

    }
}
