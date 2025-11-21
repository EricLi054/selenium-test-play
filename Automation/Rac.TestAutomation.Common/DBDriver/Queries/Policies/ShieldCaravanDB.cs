using System;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using static Rac.TestAutomation.Common.DatabaseCalls.Policies.ShieldPolicyDB;
using static Rac.TestAutomation.Common.Constants;
using System.Linq;

namespace Rac.TestAutomation.Common.DatabaseCalls.Policies
{
    public class ShieldCaravanDB
    {
        /// <summary>
        /// Shield DB value to differentiate Caravan and Trailer policies
        /// </summary>
        public enum ProgramType
        {
            Caravan = 1000002,
            Trailer = 1000003
        };

        /// <summary>
        /// Fetch an existing Caravan/Trailer Policy to verify stored properties.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <returns></returns>
        public static CaravanPolicy FetchCaravanPolicyDetail(string policyNumber, CaravanType caravanType)
        {
            CaravanPolicy result = null;
            
            try
            {
                string query = caravanType == CaravanType.Caravan ?
                               ShieldDB.ReadSQLFromFile("Policies\\CaravanPolicyByNumber.sql") :
                               ShieldDB.ReadSQLFromFile("Policies\\TrailerPolicyByNumber.sql");
                
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                  
                    while (reader.Read())
                    {

                        result = new CaravanPolicy()
                        {
                            PolicyNumber = policyNumber,
                            PolicyStartDate = DateTime.Parse(reader.GetDbValue(17)),
                            PolicyEndDate = DateTime.Parse(reader.GetDbValue(18)),
                            CaravanPolicyOutput = new CaravanPolicyOutput()
                                {
                                RegistrationValue = reader.GetDbValue(23),
                                NCBLevel = reader.GetDbValue(20),
                                CaravanLocation = reader.GetDbValue(22),
                                Campaign_Code = reader.GetDbValue(24),
                                InstallmentNumber = reader.GetDbValue(29),
                                InstallmentType = reader.GetDbValue(31),
                                InstallmentPaymentTerms = reader.GetDbValue(33),
                                PolicyCollectionMethod = reader.GetDbValue(38),
                                InstallmentCollectionMethod = reader.GetDbValue(34),
                                PolicyPaymentTerms = reader.GetDbValue(37),
                                MemberNumber = reader.GetDbValue(7),
                                EmailType = reader.GetDbValue(14)
                            },
                            CaravanPolicyInput=new CaravanPolicyInput()
                            {
                                CaravanModel = reader.GetDbValue(26),
                                CoverType = reader.GetDbValue(27),
                                IsDirectDebit = reader.GetDbValue(36).Equals(DIRECT_DEBIT),
                                PaymentFrequency = DataHelper.GetValueFromDescription<PaymentFrequency>(reader.GetDbValue(39)),
                                PolicyholderDOB = DateTime.Parse(reader.GetDbValue(5))
                            },
                            PremiumAmount = decimal.Parse(reader.GetDbValue(19)),
                            InstallmentAmount = decimal.Parse(reader.GetDbValue(30)),
                            
                           

                        };
                        break; // We will only take the first row of returned data.
                    }
                }
            }
            catch (Exception e) when (e is ArgumentException || e is UnauthorizedAccessException || e is FileNotFoundException
                                      || e is NotSupportedException || e is IndexOutOfRangeException || e is NullReferenceException
                                      || e is InvalidDataException || e is SqlException || e is ArgumentNullException || e is FormatException)
            { Reporting.Log("FetchPetPolicyDetail Exception occurs querying DB: " + e.Message); }

            return result;
        }

        /// <summary>
        /// Fetch key information about the caravan/trailer asset. Including model, make
        /// and registration.
        /// </summary>
        /// <param name="policyNum"></param>
        /// <returns>Returns details of Caravan policy asset, otherwise null if any error occurs</returns>
        public static Caravan FetchCaravanDetailsForPolicyCard(string policyNum)
        {
            Caravan result = null;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\CaravanDetailsForPolicyCardCheck.sql");
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        result = new Caravan()
                        {
                            Year  = decimal.Parse(reader.GetDbValue(0)),
                            Make  = reader.GetDbValue(1),
                            Model = reader.GetDbValue(2),
                            Registration = reader.GetDbValue(3).ToUpper(),
                            Type  =  DataHelper.GetValueFromDescription<CaravanType>(reader.GetDbValue(4))
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

        /// <summary>
        /// Finds a caravan policy that is up for renewal based on payment scenario/s list
        /// </summary>
        /// <param name="paymentScenarios">policy in expected payment method</param>
        /// <returns>Policy for endorsement and its payment method</returns>
        public static EndorseCaravan FindCaravanPolicyForRenewal(params PaymentScenario[] paymentScenarios)
        {
            EndorseCaravan policy = null;
            var candidates = new List<EndorseCaravan>();
            var paymentScenario = DataHelper.GetRandomPaymentScenario(paymentScenarios);

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyCaravanRenewal.sql");

                var queryParameter = new Dictionary<string, string>()
                {
                    { "paymentterm",      PaymentScenarioIdMappings[paymentScenario].PaymentTerm },
                    { "collectionmethod", PaymentScenarioIdMappings[paymentScenario].CollectionMethod },
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParameter);
                    while (reader.Read())
                    {
                        var result = new EndorseCaravan()
                        {
                            ActivePolicyHolder = new Contact(contactId: reader.GetDbValue(0)),
                            PolicyNumber = reader.GetDbValue(1),
                        };

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
                candidate.ActivePolicyHolder = DataHelper.MapContactWithPersonAPI(candidate.ActivePolicyHolder.Id);

                if (candidate.ActivePolicyHolder != null &&
                    IsPolicySuitableForEndorsements(candidate.PolicyNumber) &&
                    !PolicyHasUnsupportedCriteria(candidate.PolicyNumber))
                {
                    // Some contacts and policies have odd data. e.g.: Unknown Gender, etc
                    // Rather than wrestling with those, we'll skip them.
                    try
                    {
                        candidate.PayMethod = FetchPolicyPaymentDetails(candidate.PolicyNumber, candidate.ActivePolicyHolder);
                        policy = candidate;
                        break;
                    }
                    catch (SqlException ex)
                    {
                        Reporting.Log($"SQL error encountered: {ex.Message}");
                    }
                }
            }

            Reporting.IsNotNull(policy, "that a suitable caravan policy was found to use for endorsement.");
            return policy;
        }

        private static Payment FetchPolicyPaymentDetails(string policyNumber, Contact payer)
        {
            // We fetch the payment details values as these inform test framework behaviours during the endorsement.
            var policyPaymentDetailsData = ShieldPolicyDB.FetchPaymentDetailsForPolicy(policyNumber);

            // We don't care about the payee, only the payment terms.
            return new Payment(payer)
            {
                IsPaymentByBankAccount = policyPaymentDetailsData.PaymentMethod.Equals(DIRECT_DEBIT),
                PaymentFrequency = policyPaymentDetailsData.PaymentFrequency
            };
        }

        /// <summary>
        /// Find a Caravan Policy to be used in a caravan endorsement scenario.
        /// Pass the valueMin and valueMax to get the policy whose sum insured between those value. 
        /// This will be helpfull for the increase, decrease and no change in premium scenarios
        /// Pass the ProgramType to get the Caravan or Trailer policies
        /// </summary>
        /// <returns>Policy for endorsement and its payment method</returns>
        public static EndorseCaravan FindCaravanPolicyForEndorsement(SparkCommonConstants.RefundToSource refundDestination, ProgramType programType,  int valueMin, 
                                                                            int valueMax, params PaymentScenario[] paymentScenarios)
        {
            EndorseCaravan testData = null;
            List<EndorseCaravan> candidates = new List<EndorseCaravan>();
            var paymentScenario = DataHelper.GetRandomPaymentScenario(paymentScenarios);

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyCaravanEndorsement.sql");
                var queryParameter = new Dictionary<string, string>()
                {
                    { "paymentterm",      PaymentScenarioIdMappings[paymentScenario].PaymentTerm },
                    { "collectionmethod", PaymentScenarioIdMappings[paymentScenario].CollectionMethod },
                    { "vehicleType", ((int)programType).ToString()},
                    { "valueMin", valueMin.ToString()},
                    { "valueMax", valueMax.ToString()},
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParameter);
                    while (reader.Read())
                    {
                        var result = new EndorseCaravan()
                        {
                            PolicyNumber = reader.GetDbValueFromColumnName("policy_number"),
                            ActivePolicyHolder = new Contact(contactId: reader.GetDbValueFromColumnName("contactid")),
                        };
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
                try
                {
                    if (refundDestination != SparkCommonConstants.RefundToSource.None)
                    {
                        // We want to test refund to source, check that policy has right accounting info.
                        // If it doesn't, then skip this candidate and check next one. 
                        if (!HasDesiredAccountingInfoForRefundToSource(candidate.PolicyNumber, refundDestination))
                        { continue; }
                    }

                    if (PolicyHasUnsupportedCriteria(candidate.PolicyNumber))
                    { continue; }

                    if (IsPolicySuitableForEndorsements(candidate.PolicyNumber))
                    {
                        candidate.ActivePolicyHolder = new ContactBuilder(candidate.ActivePolicyHolder.Id).Build();
                        candidate.PayMethod = FetchPolicyPaymentDetails(candidate.PolicyNumber, candidate.ActivePolicyHolder);
                        candidate.ParkingAddress = candidate.ActivePolicyHolder.MailingAddress;
                        testData = candidate;
                        break;
                    }
                }
                // Some data from Shield have unsupported values for Gender and other
                // contact info, causing errors when we try to parse them. We'll skip
                // those instances for test data.
                catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
                { Reporting.Log($"Attempting to fetch added data for {candidate.PolicyNumber} encountered an exception. Skipping policy."); }
            }

            Reporting.IsNotNull(testData, "that a valid caravan policy was found to use for endorsement.");
            return testData;
        }

        /// <summary>
        /// Caravan Endorsements, including renewals, are blocked if:
        /// 1. the sum insured is more than +/- 30% market value
        /// 2. the contents SI is not rounded to thousands
        /// </summary>
        /// <returns>TRUE - if a condition prevents us from using it.</returns>
        private static bool PolicyHasUnsupportedCriteria(string policyNumber)
        {
            Reporting.Log($"Starting with {policyNumber}");
            var policyDetails = DataHelper.GetPolicyDetails(policyNumber);
            if (policyDetails.SumInsured(ShieldProductType.MGV).Any(x => x.Key == "AOCO"))
            {
                var contentsSI = policyDetails.SumInsured(ShieldProductType.MGV).First(x => x.Key == "AOCO").Value;
                if (contentsSI % 1000 > 0)
                {
                    Reporting.Log($"{policyNumber} had a Contents SI (${contentsSI})that is not a product of $1000. We cannot use.");
                    return true;
                }
            }

            if (string.Equals(policyDetails.CaravanAsset.PolicyType, "TR"))
            {
                Reporting.Log($"{policyNumber} is not eligible for online self service: Unapproved irregularities found: Camper trailer is unacceptable under trailer.");
                return true;
            }

            // ACAT == Caravan trailer (stored at home).  ACAO == Caravan On-Site (stays at caravan park). ATRO = Trailer pop-up
            var trailerSI = policyDetails.SumInsured(ShieldProductType.MGV).First(x => x.Key == "ACAT" || x.Key == "ACAO" || x.Key == "ATRO").Value;
            var marketValue = DataHelper.GetVehicleDetails(policyDetails.CaravanAsset.VehicleId).Vehicles[0].Price;
            if (marketValue > 0 &&   // Exception to the +/-30% is where we have no market value.
                (trailerSI > (decimal)(marketValue * 1.3) || trailerSI < (decimal)(marketValue * 0.7)))
            {
                Reporting.Log($"{policyNumber} had a insured value (${trailerSI}) that is more than +/-30% market value (${marketValue}). We cannot use.");
                return true;
            }

            return false;
        }
    }
}
