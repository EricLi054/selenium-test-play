using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using static Rac.TestAutomation.Common.Constants;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.DatabaseCalls.Policies.ShieldPolicyDB;

namespace Rac.TestAutomation.Common.DatabaseCalls.Policies
{
    public class ShieldMotorDB
    {
        /// <summary>
        /// Used to denote which variant of SQL to use when searching for
        /// Motor Policies for the "Change My Car" scenarios.
        /// </summary>
        public enum ChangeMyCarScenario
        {
            LowValueForIncreasePremium,
            HighValueForDecreasePremium,
            AnyValueForNoChange
        };

        /// <summary>
        /// Fetch a reference for a member by a motor policy number.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <returns></returns>
        public static MotorPolicy FetchMotorPolicyDetail(string policyNumber)
        {
            MotorPolicy result = null;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\MotorPolicyByNumber.sql");
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        result = new MotorPolicy()
                        {
                            PolicyNumber    = policyNumber,
                            PolicyStartDate = DateTime.Parse(reader.GetDbValue(0)),
                            CoverType       = reader.GetDbValue(1),
                            Excess          = int.Parse(reader.GetDbValue(2)),
                            SumInsured      = int.Parse(reader.GetDbValue(3)),
                            AnnualPremium   = decimal.Parse(reader.GetDbValue(4))
                        };
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException ) 
            { Reporting.Log("FetchMotorPolicyDetail Exception occurs querying DB: " + ex.Message); }

            return result;
        }

        /// <summary>
        /// Returns all the entries found for contact against a given motor policy.
        /// For use to check on the various roles assigned to that contact and stuff.
        /// </summary>
        /// <param name="policyNum">The Motor policy to look up</param>
        /// <param name="firstName">The first name of the contact to look up</param>
        /// <returns>A list of matches for that customer. This is to assist in validating that they don't have unexpected roles.</returns>
        public static List<Contact> FetchContactFromMotorQuoteByFirstName(string policyNum, string firstName)
        {
            List<Contact> result = new List<Contact>();

            try
            {

                string query = ShieldDB.ReadSQLFromFile("Contacts\\GetDriverByNameAndMotorPolicy.sql");
                var queryParams = new Dictionary<string, string>()
                {
                    { "policynumber", policyNum },
                    { "firstname", firstName }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParams);
                    while (reader.Read())
                    {
                        var item = new Contact()
                        {
                            Title          = DataHelper.GetValueFromDescription<Title>(reader.GetDbValueFromColumnName("Title")),
                            FirstName      = reader.GetDbValueFromColumnName("FirstName"),
                            Surname        = reader.GetDbValueFromColumnName("Surname"),
                            DateOfBirth    = DateTime.Parse(reader.GetDbValueFromColumnName("DateOfBirth")),
                            IsPolicyHolder = reader.GetDbValue(4) == "1" ? true : false,
                            MailingAddress = new Address()
                            {
                                StreetOrPOBox = reader.GetDbValue(5) == null ?
                                                       $"{reader.GetDbValueFromColumnName("Street")}" :
                                                       $"{reader.GetDbValueFromColumnName("HouseNumber")} {reader.GetDbValueFromColumnName("Street")}",
                                Suburb      = reader.GetDbValueFromColumnName("Suburb"),
                                PostCode    = reader.GetDbValueFromColumnName("Postcode")
                            },
                            MobilePhoneNumber = reader.GetDbValueFromColumnName("MobilePhoneNumber"),
                            HomePhoneNumber   = reader.GetDbValueFromColumnName("HomePhoneNumber")
                        };

                        item.MailingAddress = ReviewAddressFormat(item.MailingAddress);

                        result.Add(item);
                    }
                }
            }
            catch(Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            { Reporting.Log("GetDriverByNameAndMotorPolicy Exception occurs querying DB: " + ex.Message); }

            return result;
        }

        /// <summary>
        /// Returns the vehicle details from a given motor policy number
        /// </summary>
        /// <param name="policyNum"></param>
        /// <returns></returns>
        public static Car FetchVehicleFromMotorPolicy(string policyNum)
        {
            Car result = null;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\VehicleDetailsFromMotorPolicy.sql");
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        result = new Car()
                        {
                            Year         = decimal.Parse(reader.GetDbValue(0)),
                            Make         = reader.GetDbValue(1),
                            Model        = reader.GetDbValue(2),
                            Body         = reader.GetDbValue(3),
                            Transmission = reader.GetDbValue(4),
                            Registration = reader.GetDbValue(5),
                            // Fuel type 4000001 for the electric vehicle only, checking if the value is 4000001
                            // then set the IsElectricVehicle flag as true otherwise false
                            IsElectricVehicle = reader.GetDbValue(6) == "4000001" ? true : false,
                        };
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            { Reporting.Log("VehicleDetailsFromMotorPolicy Exception occurs querying DB: " + ex.Message); }

            return result;
        }

        /// <summary>
        /// Find a motor policy that can be cancelled, or used in
        /// Update How I Pay tests. We also make sure that there
        /// are no open claims, as this will block some cancellation
        /// cases.
        /// </summary>
        /// <param name="policyCreatedinLast28days">Boolean for a new or a newly renewed policy</param>
        /// <returns>Contact id for primary policy holder and policy number</returns>
        public static EndorseCar FindMotorPolicyNotInRenewal(bool policyCreatedinLast28days)
        {
            EndorseCar policyToUse = null;
            var candidates = new List<EndorseCar>();

            try
            {
                string query = null;
                if (policyCreatedinLast28days)
                {
                   query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyMotorWithMontlyCCFirst28Days.sql");
                } else
                {
                    query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyMotorWithAnnualDDByBankAccountAndNotInRenewal.sql");
                }


                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query,null);
                    while (reader.Read())
                    {
                        var result = new EndorseCar()
                        { PolicyNumber = reader.GetDbValue(1) };

                        try
                        {
                            result.ActivePolicyHolder = new ContactBuilder(reader.GetDbValue(0)).Build();
                        }
                        catch
                        {
                            // Contact cannot be used, go to next data item.
                            continue;
                        }

                        if(policyCreatedinLast28days)
                        {
                            result.StartDate = DateTime.ParseExact(reader.GetDbValue(4), $"{DataFormats.DATE_FORMAT_REVERSE_HYPHENS} {DataFormats.TIME_FORMAT_24HR_WITH_SECONDS}", System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            result.PayMethod = new Payment().BankAccount().Annual();
                            result.PayMethod.Payer = result.ActivePolicyHolder;
                            var existingBankDetails = new BankAccount();                           
                            existingBankDetails.Bsb = reader.GetDbValue(5);
                            existingBankDetails.AccountNumber = reader.GetDbValue(6);
                            existingBankDetails.AccountName = reader.GetDbValue(7);
                            result.ActivePolicyHolder.BankAccounts.Add(existingBankDetails);                           
                        }

                        // When the mobile phone number is not present, then the land line
                        // number needs to be valid and included the area code
                        if (DataHelper.IsValidAustralianPhoneNumber(reader.GetDbValue(2)))
                        {
                            candidates.Add(result);
                        } else if (string.IsNullOrEmpty(reader.GetDbValue(2)) &&
                           DataHelper.IsValidAustralianPhoneNumber(reader.GetDbValue(3)))
                        {
                            candidates.Add(result);
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            foreach (var candidate in candidates)
            {
                var contact = DataHelper.MapContactWithPersonAPI(candidate.ActivePolicyHolder.Id, candidate.ActivePolicyHolder.ExternalContactNumber);
                
                if (IsPolicySuitableForEndorsements(candidate.PolicyNumber) &&
                    ShieldClaimDB.GetOpenClaimCountForPolicy(candidate.PolicyNumber) == 0 &&
                    contact != null && contact.MobilePhoneNumber != null &&
                    !DataHelper.ContactHasBadBankAccountName(candidate.ActivePolicyHolder.Id))
                {
                    policyToUse = candidate;
                    policyToUse.ActivePolicyHolder = contact;
                    break;
                }
            }

            Reporting.IsNotNull(policyToUse, "that we found a motor policy to use for endorsement (cancellation)");
            return policyToUse;
        }

        /// <summary>
        /// Find a Motor Policy to be used in a Change May Car endorsement scenario.
        /// LowValueForIncreasePremium - MFCO low value policy paid based on payment scenario given
        /// HighValueForDecreasePremium - MFCO high value policy paid on payment scenario given
        /// AnyValueForNoChange - Any cover type motor policy paid on payment scenario given
        /// </summary>
        /// <param name="requestedScenario"></param>
        /// <param name="paymentScenarios"></param>
        /// <returns>Policy for endorsement and its payment method</returns>
        public static EndorseCar FindMotorPolicyForChangeMyCar(ChangeMyCarScenario requestedScenario,
                                                               SparkCommonConstants.RefundToSource refundDestination, 
                                                                bool qasAddressPolicies = false, 
                                                                params PaymentScenario[] paymentScenarios)
        {
            EndorseCar testData = null;
            List<EndorseCar> candidates = new List<EndorseCar>();
            var paymentScenario             = DataHelper.GetRandomPaymentScenario(paymentScenarios);

            try
            {
                string query = null;

                switch (requestedScenario)
                {
                    case ChangeMyCarScenario.LowValueForIncreasePremium:
                        query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyMotorLowValueForChangeMyCar.sql");
                        break;
                    case ChangeMyCarScenario.HighValueForDecreasePremium:
                        query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyMotorHighValueForChangeMyCar.sql");
                        break;
                    case ChangeMyCarScenario.AnyValueForNoChange:
                    default:
                        query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyMotorAnyPolicyChangeMyCar.sql");
                        break;
                }

                var sqlParameters = ShieldPolicyDB.GetShieldPaymentMethodParameters(paymentScenario);
                sqlParameters.Add("needsQAS", qasAddressPolicies ? "1" : "0");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, sqlParameters);
                    while (reader.Read())
                    {
                        var result = new EndorseCar()
                        {
                            PolicyNumber = reader.GetDbValueFromColumnName("PolicyNumber"),
                            ActivePolicyHolder = new Contact(contactId: reader.GetDbValueFromColumnName("ContactId")),
                            CurrentProductVersionNumber = int.Parse(reader.GetDbValueFromColumnName("ProductVersionNumber"))
                        };

                        var coverString = reader.GetDbValueFromColumnName("Cover_Type");
                        if (coverString.StartsWith(MotorCoverNameMappings[MotorCovers.MFCO].TextShield))
                        {
                            result.CoverType = MotorCovers.MFCO;
                        }
                        else if (coverString.StartsWith(MotorCoverNameMappings[MotorCovers.TFT].TextShield))
                        {
                            result.CoverType = MotorCovers.TFT;
                        }
                        else if (coverString.StartsWith(MotorCoverNameMappings[MotorCovers.TPO].TextShield))
                        {
                            result.CoverType = MotorCovers.TPO;
                        }
                        else
                        { Reporting.Log($"Encountered policy {result.PolicyNumber} which had a parent cover that we don't recognise; {coverString}. We'll skip it."); }

                        candidates.Add(result);
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            foreach(var candidate in candidates)
            {
                try
                {
                    if (refundDestination != SparkCommonConstants.RefundToSource.None)
                    {
                        // We want to test refund to source, check that policy has right accounting info.
                        // If it doesn't, then skip this candidate and check next one. 
                        if (!HasDesiredAccountingInfoForRefundToSource(candidate.PolicyNumber, refundDestination))
                        { continue; }
                    }

                    if (ShieldPolicyDB.IsPolicySuitableForEndorsements(candidate.PolicyNumber))
                    {
                        candidate.ActivePolicyHolder = DataHelper.MapContactWithPersonAPI(candidate.ActivePolicyHolder.Id);                       
                        if (candidate.ActivePolicyHolder != null &&
                            candidate.ActivePolicyHolder!.MobilePhoneNumber != null &&
                            !DataHelper.ContactHasBadBankAccountName(candidate.ActivePolicyHolder.Id))
                        {
                            candidate.PayMethod = FetchPolicyPaymentDetails(candidate.PolicyNumber, candidate.ActivePolicyHolder);
                            candidate.ParkingAddress = candidate.ActivePolicyHolder.MailingAddress;                           
                            testData = candidate;
                            break;
                        }
                    }
                }
                // Some data from Shield have unsupported values for Gender and other
                // contact info, causing errors when we try to parse them. We'll skip
                // those instances for test data.
                catch(Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException) 
                { Reporting.Log($"Attempting to fetch added data for {candidate.PolicyNumber} encountered an exception. Skipping policy."); }
            }
            
            Reporting.IsNotNull(testData, "that a valid motor policy was found to use for endorsement.");
            return testData;
        }

        /// <summary>
        /// Finds a motor policy that is up for renewal based from parameters provided - motor cover and payment scenario/s list
        /// </summary>
        /// <param name="motorCover"></param>
        /// <param name="paymentScenarios"></param>
        /// <returns>Policy for endorsement and its payment method</returns>
        public static EndorseCar FindMotorPolicyForRenewal(MotorCovers motorCover, bool hasRiskAddress = false, params PaymentScenario[] paymentScenarios)
        {
            EndorseCar policy     = null;
            var candidates        = new List<EndorseCar>();
            var paymentScenario   = DataHelper.GetRandomPaymentScenario(paymentScenarios);

            try
            {
                string query = null;
                query        = hasRiskAddress ? ShieldDB.ReadSQLFromFile("Policies\\FindPolicyMotorRenewalQASValidatedWithGNAFID.sql") 
                                              : ShieldDB.ReadSQLFromFile("Policies\\FindPolicyMotorRenewal.sql");

                var queryPaymentMethodMotorCover = GetShieldPaymentMethodAndMotorCoverQuery(motorCover, paymentScenario);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPaymentMethodMotorCover);
                    while (reader.Read())
                    {
                        var result = new EndorseCar()
                        {
                            PolicyNumber = reader.GetDbValueFromColumnName("PolicyNumber"),
                            ActivePolicyHolder = new Contact(contactId: reader.GetDbValueFromColumnName("ContactId")),
                            CurrentProductVersionNumber = int.Parse(reader.GetDbValueFromColumnName("ProductVersionNumber"))
                        };

                        var coverString = reader.GetDbValueFromColumnName("CoverType");
                        if (coverString.StartsWith(MotorCoverNameMappings[MotorCovers.MFCO].TextShield))
                        {
                            result.CoverType = MotorCovers.MFCO; 
                        }
                        else if (coverString.StartsWith(MotorCoverNameMappings[MotorCovers.TFT].TextShield))
                        {
                            result.CoverType = MotorCovers.TFT; 
                        }
                        else if (coverString.StartsWith(MotorCoverNameMappings[MotorCovers.TPO].TextShield))
                        { 
                            result.CoverType = MotorCovers.TPO;  
                        }
                        else
                        { Reporting.Log($"Encountered policy {result.PolicyNumber} which had a parent cover that we don't recognise; {coverString}. We'll skip it."); }

                        candidates.Add(result);
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            foreach (var candidate in candidates)
            {
                if (ShieldPolicyDB.IsPolicySuitableForEndorsements(candidate.PolicyNumber))
                {
                    // Some contacts and policies have odd data. e.g.: Unknown Gender, etc
                    // Rather than wrestling with those, we'll skip them.
                    try
                    {                        
                        candidate.ActivePolicyHolder = DataHelper.MapContactWithPersonAPI(candidate.ActivePolicyHolder.Id);
                        //We check the mobile phone number because MFA required one
                        if (candidate.ActivePolicyHolder != null && 
                            candidate.ActivePolicyHolder!.MobilePhoneNumber != null &&
                            !DataHelper.ContactHasBadBankAccountName(candidate.ActivePolicyHolder.Id))
                        {
                            candidate.PayMethod = FetchPolicyPaymentDetails(candidate.PolicyNumber, candidate.ActivePolicyHolder);
                            policy = candidate;
                            break;
                        }               
                    }
                    catch (SqlException ex) 
                    { 
                        Reporting.Log($"SQL error encountered: {ex.Message}"); 
                    }
                }
            }

            Reporting.IsNotNull(policy, "that a suitable motor policy was found to use for endorsement.");
            return policy;
        }

        /// <summary>
        /// Returns the primary cover on a motor policy from its previous policy version.
        /// </summary>
        public static string GetCoverForPreviousPolicyVersion(string policyNumber)
        {
            string previousCover = null;

            try
            {
                string query = null;
                query = ShieldDB.ReadSQLFromFile("Policies\\CheckMotorPolicyForCoverOnPreviousVersion.sql");
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        previousCover = reader.GetDbValueFromColumnName("Cover");
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            return previousCover;
        }

        private static Address ReviewAddressFormat(Address addr)
        {
            var regex = new Regex(@"^(\d[0-9a-z/]*)\s(.*)$");
            Match match = regex.Match(addr.StreetOrPOBox.ToLower());
            if (addr.StreetNumber == null && match.Success && match.Groups.Count == 3)
            {
                addr.StreetNumber = match.Groups[1].Value;
                addr.StreetOrPOBox = match.Groups[2].Value;
            }

            return addr;
        }

        private static Payment FetchPolicyPaymentDetails(string policyNumber, Contact payer)
        {
            // We fetch the payment details values as these inform test framework behaviours during the endorsement.
            var policyPaymentDetailsData = ShieldPolicyDB.FetchPaymentDetailsForPolicy(policyNumber);

            // We don't care about the payee, only the payment terms.
            return new Payment(payer)
            {
                IsPaymentByBankAccount = policyPaymentDetailsData.PaymentMethod.Equals(DIRECT_DEBIT),
                PaymentFrequency       = policyPaymentDetailsData.PaymentFrequency
            };
        }

        private static Dictionary<string, string> GetShieldPaymentMethodAndMotorCoverQuery(MotorCovers motorCover, PaymentScenario paymentScenario)
        {
            string motorCoverId               = MotorCoverIdMappings[motorCover];

            return new Dictionary<string, string>()
            {
                { "paymentterm",      PaymentScenarioIdMappings[paymentScenario].PaymentTerm },
                { "collectionmethod", PaymentScenarioIdMappings[paymentScenario].CollectionMethod },
                { "motorcover", motorCoverId }
            };
        }
    }
}