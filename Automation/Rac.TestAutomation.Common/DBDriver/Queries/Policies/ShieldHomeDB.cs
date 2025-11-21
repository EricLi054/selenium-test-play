using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;
using static Rac.TestAutomation.Common.Constants.Contacts;

namespace Rac.TestAutomation.Common.DatabaseCalls.Policies
{
    public class ShieldHomeDB
    {
        /// <summary>
        /// Used to denote which variant of SQL to use when searching for
        /// Home Policies for the "Change my home details" scenarios.
        /// </summary>
        public enum ChangeMyHomeDetailsScenario
        {
            LowRiskForIncreasePremium,
            HighRiskForDecreasePremium,
            AnyRiskForNoChange
        };

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static List<string> FindListOfHomePolicyNumbersOwnerOccupiedBuildingAndContents()
        {
            var candidates = new List<string>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyHomeOwnerBuildingAndContents.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        candidates.Add(reader.GetDbValue(0));
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            { 
                Reporting.Log("FindHomePolicy Exception occurs querying DB: " + ex.Message); 
            }

            return candidates;
        }

        /// <summary>       
        /// Find Landlord and Home Building Policies for spark fence claim flow.
        /// </summary>     
        public static List<string> FindListOfHomePolicyNumbersLandlordAndHomeownerBuilding()
        {
            var candidates = new List<string>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindHomeownerAndLandlordBuildingPolicy.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        candidates.Add(reader.GetDbValue(0));
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            { 
                Reporting.Log("FindHomePolicy Exception occurs querying DB: " + ex.Message); 
            }

            return candidates;
        }

        /// <summary>
        /// Fetch information about an existing Home Policy details
        /// restricted to only policy number, cover types and policy holder
        /// to verify our assertions against.
        /// </summary>        
        public static HomePolicy FetchHomePolicyDetailsForClaim(string policyNumber)
        {
            var result = new HomePolicy();

            var policyDetails = DataHelper.GetPolicyDetails(policyNumber);

            result.PolicyNumber = policyDetails.PolicyNumber;
            result.Covers = new List<PolicyCoverDetails>();            
            foreach (var item in policyDetails.Covers) 
            {
                var cover = new PolicyCoverDetails();
                cover.CoverDescription = item.CoverTypeDescription;
                cover.CoverCode = DataHelper.GetValueFromDescription<HomeCoverCodes>(item.CoverType);                
                result.Covers.Add(cover);
            }
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
            result.PolicyHolders = policyHolders;         
            return result;
        }

        /// <summary>
        /// Fetch an existing Home Policy to verify stored properties.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <returns></returns>
        public static HomePolicy FetchHomePolicyDetails(string policyNumber)
        {
            var result = new HomePolicy();            
            result.PropertyDetails               = FetchHomePropertyDetails(policyNumber);
            result.PaymentDetails                = ShieldPolicyDB.FetchPaymentDetailsForPolicy(policyNumber);
            result.SpecifiedValuablesAndContents = FetchAllSpecifiedValuablesAndContents(policyNumber);
            result.PolicyHolders                 = ShieldPolicyDB.FetchPolicyContacts(policyNumber);
            result.DisclosedClaimsHistory        = FetchDisclosedClaimsHistory(policyNumber);

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\HomePolicyGetBasicPolicyDetails.sql");
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    result.Covers = new List<PolicyCoverDetails>();

                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        result.PolicyNumber        = policyNumber;
                        result.PolicyStartDate     = DateTime.Parse(reader.GetDbValue(0));
                        result.Occupancy           = DataHelper.GetValueFromDescription<HomeOccupancy>(reader.GetDbValue(1));
                       
                        var cover                  = new PolicyCoverDetails();

                        // We use regex to strip out the "- cloned" that may appear on cover descriptions
                        var regex   = new Regex(@"^((\w+\s+)*\w+)\s*-*");
                        Match match = regex.Match(reader.GetDbValue(2));

                        if(!match.Success || match.Groups.Count < 1)
                        {
                            Reporting.Error($"Cover description for home policy was not successfully parsed.");
                        }
                        cover.CoverDescription = match.Groups[1].Value;

                        cover.CoverCode  = DataHelper.GetValueFromDescription<HomeCoverCodes>(reader.GetDbValue(5));
                        cover.Excess     = reader.GetDbValue(3);
                        cover.SumInsured = int.Parse(reader.GetDbValue(4));
                        result.Covers.Add(cover);
                        result.WeeklyRental = reader.IsDBNull(6) ? 0 : int.Parse(reader.GetDbValue(6));
                        result.PropertyManager = reader.IsDBNull(7) ? result.PropertyManager : DataHelper.GetValueFromDescription<HomePropertyManager>(reader.GetDbValue(7));
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered: in FetchHomePolicyDetails ({policyNumber}) {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Returns the list of all specified personal valuables and
        /// contents on a home policy. The Category will distinguish
        /// between contents and SPV.
        /// </summary>
        /// <param name="policyNum"></param>
        /// <returns>Empty list if there are no results</returns>
        private static List<ContentItem> FetchAllSpecifiedValuablesAndContents(string policyNum)
        {
            List<ContentItem> result = new List<ContentItem>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\HomePolicyGetSpecifiedValuablesAndContents.sql");
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        var shieldCategoryName = reader.GetDbValue(0);

                        // If we don't recognise the category, then its not a valid specified valuable.
                        if (!SpecifiedPersonalValuablesDisplayedText.Any(x => x.Value.TextShield == shieldCategoryName))
                            continue;

                        var item = new ContentItem()
                        {
                            Category = (int)SpecifiedPersonalValuablesDisplayedText.First(x => x.Value.TextShield == shieldCategoryName).Key,
                            Description = reader.GetDbValue(1),
                            Value = int.Parse(reader.GetDbValue(2))
                        };
                        result.Add(item);
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered: in FetchAllSpecifiedValuablesAndContents for {policyNum} {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Returns the list of disclosed claims history from time of
        /// policy purchase.
        /// </summary>
        /// <returns>List of claims. Empty list if no claims were disclosed.</returns>
        private static List<ClaimHistory> FetchDisclosedClaimsHistory(string policyNum)
        {
            List<ClaimHistory> result = new List<ClaimHistory>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\HomePolicyGetDisclosedClaims.sql");
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        // Have seen Shield put in empty rows for claims history when there are no disclosures.
                        if (reader.IsDBNull(0))
                            continue;

                        // Only want to get history for quotes/policies that use current disclosure format.
                        // (Year of claim, and not where we get a claim count)
                        if (reader.IsDBNull(1))
                            continue;

                        var claimItem = new ClaimHistory()
                        {
                            ClaimType = DataHelper.GetValueFromDescription<ClaimsHistory>(reader.GetDbValue(0)),
                            Year = int.Parse(reader.GetDbValue(1))
                        };

                        result.Add(claimItem);
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered: in FetchDisclosedClaimsHistory for {policyNum} {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Returns a Home Policy details where the renewal is due. 
        /// The policy selected checks that the active policy holder
        /// has one and only one non credit card account associated with them.
        /// After executing the query the home policy information is returned.
        /// </summary>
        /// <returns></returns>
        public static string FindPolicyForCancellation()
        {
            string policyNumber = null;
            List<EndorseHome> candidates = new List<EndorseHome>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyHomeWithBuildingAndContentsInRenewalForCancellation.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        var result = new EndorseHome();
                        result.PolicyNumber = reader.GetDbValue(0);
                        result.ActivePolicyHolder = new Contact(contactId: reader.GetDbValue(1));

                        // When the mobile phone number is not present, then the land line
                        // number needs to be valid and included the area code
                        if (DataHelper.IsValidAustralianPhoneNumber(reader.GetDbValue(9)))
                        {
                            candidates.Add(result);
                        }
                        else if (string.IsNullOrEmpty(reader.GetDbValue(9)) &&
                         DataHelper.IsValidAustralianPhoneNumber(reader.GetDbValue(10)))
                        {
                            candidates.Add(result);
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            foreach (var candidate in candidates)
            {
                var contact = DataHelper.MapContactWithPersonAPI(candidate.ActivePolicyHolder.Id);                
                if (contact != null && ShieldPolicyDB.IsPolicySuitableForEndorsements(candidate.PolicyNumber) &&
                     !DataHelper.ContactHasBadBankAccountName(candidate.ActivePolicyHolder.Id))
                {
                    policyNumber = candidate.PolicyNumber;                   
                    break;  // We return the first candidate that meets our criteria.
                }
            }
            Reporting.IsNotNull(policyNumber, "that a suitable policy was found for endorsment");
            return policyNumber;
        }

        /// <summary>
        /// Returns a Home Policy details where the renewal is due.
        /// The payment frequency for a renewal could be either Annual Cash of paid Monthly
        /// Following method executes the relevant DB query based on the 'payment frequency'
        /// and returns Policy information
        /// </summary>
        /// <param name="paymentFrequency"></param>
        /// <returns>Policy information</returns>
        /// <exception cref="NotSupportedException">Thrown if policies returned from Shield database is not supported for B2C/Spark cases.</exception>
        public static List<string> FindHomePolicyForRenewal(PaymentFrequency paymentFrequency)
        {
            List<string> candidates = new List<string>();

            try
            {
                string query = null;

                switch (paymentFrequency)
                {
                    case PaymentFrequency.Annual:
                        query = ShieldDB.ReadSQLFromFile("Policies\\FindAnnualCashHomePolicyInRenewalState.sql");
                        break;
                    case PaymentFrequency.Monthly:
                        query = ShieldDB.ReadSQLFromFile("Policies\\FindInstalmentPaidHomePolicyInRenewalState.sql");
                        break;
                    default:
                        throw new NotSupportedException($"Policies for payment frequency {paymentFrequency} is not supported yet.");
                }

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        candidates.Add(reader.GetDbValue(0));                       
                    }                   
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            Reporting.IsTrue(candidates.Count > 0, "that we found at least one suitable policy matching test criteria");
            return candidates;
        }

        /// <summary>
        /// Returns a Home Policy where the renewal is due for the current date
        /// And Policy is required to be renewed/payed using "PayNow" option with Annual Cash
        /// </summary>
        /// <returns>Policy number</returns>
        public static string FindHomePolicyForPayNow()
        {
            string policyNumber = null;
            var candidates = new List<EndorseHome>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindAnnualCashPaidHomePolicyInPayNowState.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        var result = new EndorseHome()
                        {
                            PolicyNumber = reader.GetDbValue(0),
                            ActivePolicyHolder = new Contact(contactId: reader.GetDbValue(1))
                        };
                        candidates.Add(result);
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            foreach(var candidate in candidates)
            {
                if (!ShieldPolicyDB.PolicyHasBadInstallments(candidate.PolicyNumber) &&
                    !ShieldPolicyDB.PolicyHasPendingInstallmentsRelativeToDate(candidate.PolicyNumber, DateTime.Now))
                {
                    policyNumber = candidate.PolicyNumber;
                    break;  // We return the first candidate that meets our criteria.
                }
            }

            Reporting.IsNotNull(policyNumber, "that a suitable policy was found matching test criteria");
            return policyNumber;
        }

        /// <summary>
        /// Returns the Sum of the Annual Premiums From Instalments
        /// </summary>
        /// <param name="testData"></param>
        /// <returns>Annual Premium Sum</returns>
        public static decimal FindHomePolicyAnnualPremiumSumFromInstalmentsForRenewal(string policyNumber)
        {
            Decimal currentPremium = 0;
            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindAnnualPremiumSumFromInstallmentsForHomePolicyInRenewalState.sql");
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        currentPremium = decimal.Parse(reader.GetDbValue(1));
                        break;
                    }
                    Reporting.Log($"Fetched Annual premium Sum from Installments");
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            return currentPremium;
        }


        /// <summary>
        /// Get the details of the Home Policy after the renewal of a payment due policy
        /// </summary>
        /// <param name="testData"></param>
        /// <returns>policyInfoFromDB</returns>
        /// <exception cref="NotSupportedException">Thrown if policy information test data returned from Shield databaseis not supported for B2C/Spark cases.</exception>
        public static HomePolicy FetchHomePolicyDetailsAfterRenewal(EndorseHome testData)
        {
            HomePolicy policyInfoFromDB = new HomePolicy();
            policyInfoFromDB.EventDetails = new PolicyEventDetails();
            policyInfoFromDB.PaymentDetails = new PolicyPaymentDetails();
            try
            {
                string query = null;

                switch (testData.PayMethod.PaymentFrequency)
                {
                    case PaymentFrequency.Annual:
                        query = ShieldDB.ReadSQLFromFile("Policies\\HomePolicyGetBasicPolicyDetailsAfterPolicyRenewalAnnualCash.sql");
                        break;
                    case PaymentFrequency.Monthly:
                        query = ShieldDB.ReadSQLFromFile("Policies\\HomePolicyGetBasicPolicyDetailsAfterPolicyRenewalInstalments.sql");
                        break;
                    default:
                        throw new NotSupportedException($"Details for the Home Policies for Payment Frequency {testData.PayMethod.PaymentFrequency} is not supported yet.");
                }

                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(testData.PolicyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        policyInfoFromDB.EndorsementID = int.Parse(reader.GetDbValue(4));
                        policyInfoFromDB.MainRenewalDate = DateTime.Parse(reader.GetDbValue(2));
                        policyInfoFromDB.EventDetails.EventDate = DateTime.Parse(reader.GetDbValue(6));
                        policyInfoFromDB.EventDetails.EventType = reader.GetDbValue(7);
                        policyInfoFromDB.EventDetails.EventDocType = reader.GetDbValue(8);
                        switch (testData.PayMethod.PaymentFrequency)
                        {
                            case PaymentFrequency.Annual:
                                policyInfoFromDB.TransactionType = reader.GetDbValue(12);
                                policyInfoFromDB.PaymentDetails.PaymentStatus = reader.GetDbValue(10);
                                policyInfoFromDB.YearlyPremium = decimal.Parse(reader.GetDbValue(5));
                                policyInfoFromDB.PaymentDetails.PaymentTotal = decimal.Parse(reader.GetDbValue(11));
                                policyInfoFromDB.PaymentDetails.PaymentDate = DateTime.Parse(reader.GetDbValue(9));
                                break;
                            case PaymentFrequency.Monthly:
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            return policyInfoFromDB;
        }

        /// <summary>
        /// Get the details of the Home Policy after the renewal of a PayNow policy
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="policyInfoFromDB"></param>
        /// <returns>Empty list if there are no results</returns>
        public static HomePolicy FetchHomePolicyDetailsAfterPayNowRenewal(string policyNumber)
        {
            HomePolicy policyInfoFromDB = new HomePolicy();
            policyInfoFromDB.EventDetails = new PolicyEventDetails();
            policyInfoFromDB.PaymentDetails = new PolicyPaymentDetails();
            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\HomePolicyGetBasicPolicyDetailsAfterPolicyRenewalPayMyPolicy.sql");

                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        policyInfoFromDB.MainRenewalDate = DateTime.Parse(reader.GetDbValue(2));
                        policyInfoFromDB.YearlyPremium = decimal.Parse(reader.GetDbValue(4));
                        policyInfoFromDB.PaymentDetails.PaymentDate = DateTime.Parse(reader.GetDbValue(5));
                        policyInfoFromDB.PaymentDetails.PaymentStatus = reader.GetDbValue(6);
                        policyInfoFromDB.PaymentDetails.PaymentTotal = decimal.Parse(reader.GetDbValue(7));
                        policyInfoFromDB.TransactionType = reader.GetDbValue(8);
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            return policyInfoFromDB;
        }

        /// <summary>
        /// Get the details of the Expected Premium after the renewal for the given Policy Number
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <returns>expected premium</returns>
        public static decimal FetchExpectedPremiumAfterRenewal(string policyNumber)
        {
            decimal result = 0;
            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\HomePolicyGetExpectedPremiumForPolicy.sql");
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        result = decimal.Parse(reader.GetDbValue(1));
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Find a Home Policy to be used in Change my home details endorsement scenario.
        /// LowRiskForIncreasePremium - Low risk suburb that is not in renewal policy paid annually.
        /// HighRiskForDecreasePremium - High risk suburb that is not in renewal policy paid monthly.
        /// </summary>
        /// <param name="requestedScenario">Determines the specific SQL query that is used which returns Home Policies that suit the related scenario</returns>
        public static List<string> FindHomePolicyForChangeMyHomeDetails(ChangeMyHomeDetailsScenario requestedScenario)
        {
            var candidates = new List<string>();

            try
            {
                string query = null;
                switch (requestedScenario)
                {
                    case ChangeMyHomeDetailsScenario.LowRiskForIncreasePremium:
                        query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyHomeLowRiskSuburbForChangeMyHomeDetails.sql");
                        break;

                    case ChangeMyHomeDetailsScenario.HighRiskForDecreasePremium:
                        query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyHomeHighRiskSuburbForChangeMyHomeDetails.sql");
                        break;

                    default:
                        throw new NotImplementedException(requestedScenario + " is not implemented yet.");
                }

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        candidates.Add(reader.GetDbValueFromColumnName("Policy_Number"));
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            {
                Reporting.Error($"SQL error encountered: {ex.Message}");
            }

            Reporting.IsTrue(candidates.Count > 0, "that we found at least one suitable policy matching test criteria");

            return candidates;
        }

        /// <summary>
        /// Get the endorsement details for Building/Contents cover
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <returns>Home object with details of physical characteristics of insured property.</returns>
        private static Home FetchHomePropertyDetails(string policyNumber)
        {
            Home propertyDetails = null;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\HomePolicyGetPropertyDetails.sql");
                var queryPolNumCover = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNumCover);
                    while (reader.Read())
                    {
                        propertyDetails = new Home()
                        {
                            PropertyAddress = new Address()
                            {
                                StreetNumber  = reader.GetDbValueFromColumnName("house_nr"),
                                StreetOrPOBox = reader.GetDbValueFromColumnName("street"),
                                Suburb = reader.GetDbValueFromColumnName("Suburb")
                            },
                            TypeOfBuilding = DataHelper.ConvertBuildingTypeStringToEnum(reader.GetDbValueFromColumnName("building_type"), isShieldText: true),
                            AlarmSystem    = DataHelper.GetValueFromDescription<Alarm>(reader.GetDbValueFromColumnName("alarm_type")),
                            SecurityWindowsSecured = reader.GetDbValueFromColumnName("Window_Security") == "Y" ? true : false,
                            SecurityDoorsSecured   = reader.GetDbValueFromColumnName("Door_Security") == "Y" ? true : false,
                            WallMaterial   = DataHelper.GetValueFromDescription<HomeMaterial>(reader.GetDbValueFromColumnName("Construction_Type")),
                            YearBuilt      = int.Parse(reader.GetDbValueFromColumnName("Year"))
                        };

                        // Automation wants these to be empty strings, not null, if they have no value in Shield.
                        if (string.IsNullOrEmpty(propertyDetails.PropertyAddress.StreetNumber))
                        { propertyDetails.PropertyAddress.StreetNumber = string.Empty; }
                        if (string.IsNullOrEmpty(propertyDetails.PropertyAddress.StreetOrPOBox))
                        { propertyDetails.PropertyAddress.StreetOrPOBox = string.Empty; }

                        // As per B2C-4951, product versions after 68000008 no longer require roof type.
                        var roofMaterialCode = reader.GetDbValueFromColumnName("Roof_Material");
                        propertyDetails.RoofMaterial = string.IsNullOrEmpty(roofMaterialCode) ?
                                                       HomeRoof.Undefined :
                                                       DataHelper.GetValueFromDescription<HomeRoof>(roofMaterialCode);
                        // Only using first returned data set.
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            {
                Reporting.Error($"SQL error encountered for policy number: {policyNumber}: {ex.Message}");
            }

            return propertyDetails;
        }

        /// <summary>
        /// Looks for a home policy (with no current claims) which has cover for Building, Contents and 
        /// Specified Personal Valuables to be used in B2C_T425_TC01_B2CLoggedIn_ClaimStatus_Home.
        /// </summary>
        public static string FindHomePersonalValuablesPolicyForHomeClaimAgendaTest()
        {
            var candidates = new List<string>();
            Reporting.Log($"Begin query for policy list in Shield Database");
            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyHomeForNewClaim.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        candidates.Add(reader.GetDbValueFromColumnName("EXTERNAL_POLICY_NUMBER"));
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            { Reporting.Log("FindHomePersonalValuablesPolicyForHomeClaimAgendaTest Exception occurs querying DB: " + ex.Message); }
            Reporting.Log($"Initial policy list acquired");
            return ShieldHomeClaimDB.ReturnFirstPolicySuitableForClaims(candidates);
        }

        /// <summary
        /// Find a Home Policy with active Building cover so we can request
        /// a certificate of currency.
        /// </summary>
        public static List<string> FindPolicyHomeActiveBuildingCoverForCertificateOfCurrency()
        {
            Reporting.Log($"Looking for home policy suitable for CoC");
            
            var candidates = new List<string>();
            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyHomeActiveBuildingCoverForCertificateOfCurrency.sql");

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        candidates.Add(reader.GetDbValue(0));
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException)
            { Reporting.Log("FindPolicyHomeActiveBuildingCoverForCertificateOfCurrency Exception occurs querying DB: " + ex.Message); }

            return candidates;
        }

        /// <summary>
        /// For the given Home Policy, checks the version of the policy
        /// prior to the most recent endorsement, and checks that
        /// storm cover for both building and contents was present.
        /// </summary>
        /// <param name="policyNum"></param>
        /// <returns>TRUE if storm cover for home owner building and contents was found for the previous version.</returns>
        public static bool HomePolicyHadStormCoverPriorToLastEndorsement(string policyNum)
        {
            var hadStormCover = false;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\QueryHomePolicyPreviousVersionHaveStormCoverForBuildingAndContents.sql");
                var queryPolNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNum);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryPolNum);
                    while (reader.Read())
                    {
                        hadStormCover = (int.Parse(reader.GetDbValue(0)) > 0);
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is SqlException || ex is FormatException)
            { Reporting.Log("QueryHomePolicyPreviousVersionHaveStormCoverForBuildingAndContents Exception occurs querying DB: " + ex.Message); }

            return hadStormCover;
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
    }
}