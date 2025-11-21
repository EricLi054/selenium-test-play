using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;

namespace Rac.TestAutomation.Common.DatabaseCalls.Policies
{
    public class ShieldBoatDB
    {
        public static EndorseBoat FetchBoatPolicyForCancellation()
        {
            EndorseBoat usablePolicy = null;
            var candidates = new List<EndorseBoat>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\FindPolicyBoatInRenewalForCancellation.sql");
                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);
                    while (reader.Read())
                    {
                        var result = new EndorseBoat()
                        {
                            PolicyNumber = reader.GetDbValue(0),
                            ActivePolicyHolder = new Contact(contactId: reader.GetDbValue(1)),
                            RenewalDate = DateTime.ParseExact(reader.GetDbValue(2),
                            $"{DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH} {DataFormats.TIME_FORMAT_24HR_WITH_SECONDS}",
                            System.Globalization.CultureInfo.InvariantCulture),
                            Type = DataHelper.GetValueFromDescription<BoatType>(reader.GetDbValue(3))
                        };


                        // When the mobile phone number is not present, then the land line
                        // number needs to be valid and included the area code
                        if (DataHelper.IsValidAustralianPhoneNumber(reader.GetDbValue(4)))
                        {
                            candidates.Add(result);
                        }
                        else if (string.IsNullOrEmpty(reader.GetDbValue(4)) &&
                         DataHelper.IsValidAustralianPhoneNumber(reader.GetDbValue(5)))
                        {
                            candidates.Add(result);
                        }
                    }
                }
            }
            catch (Exception e) when (e is ArgumentException || e is UnauthorizedAccessException || e is FileNotFoundException
                                       || e is NotSupportedException || e is IndexOutOfRangeException || e is NullReferenceException
                                       || e is InvalidDataException || e is SqlException || e is ArgumentNullException || e is FormatException)
            {
                Reporting.Error($"SQL error encountered: {e.Message}");
            }

            foreach (var candidate in candidates)
            {
                var contact = DataHelper.MapContactWithPersonAPI(candidate.ActivePolicyHolder.Id);               
                if (contact != null &&
                    ShieldPolicyDB.IsPolicySuitableForEndorsements(candidate.PolicyNumber) &&
                     !DataHelper.ContactHasBadBankAccountName(candidate.ActivePolicyHolder.Id))
                {
                    usablePolicy = candidate;
                    // Some contacts and policies have odd data. e.g.: Unknown Gender etc
                    // Rather than wrestling with those, we'll skip them.
                    try
                    {
                        contact.UpdateEmailIfNotDefined();
                        usablePolicy.ActivePolicyHolder = contact;                       
                        break;
                    }
                    catch(Exception e) when(e  is NullReferenceException) {
                        Reporting.Log($"Exception Occured:{e.Message}");
                    }
                }
            }
            Reporting.IsNotNull(usablePolicy, "that we found a boat policy to use for endorsement (cancellation)");
            return usablePolicy;
        }

        /// <summary>
        /// Fetch details from an existing Boat Quote to verify stored properties.
        /// </summary>
        /// <param name="quoteNumber">The quote number to fetch details for.</param>
        public static BoatQuoteValues FetchBoatQuoteDetails(string quoteNumber)
        {
            var databaseResponse = new BoatQuoteValues();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\Boat_Quote_Details_from_Database_using_Quote_number.sql");
                var queryQteNum = ShieldDB.SetSqlParameterForQuoteNumber(quoteNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryQteNum);

                    while (reader.Read())
                    {
                        //TODO Verify via Shield API where possible
                        databaseResponse.PolicyUpdateUser                      = reader.GetDbValueFromColumnName("PolicyUpdateUser");
                        databaseResponse.QuoteNumber                           = reader.GetDbValueFromColumnName("QuoteNumber");
                        databaseResponse.Status                                = reader.GetDbValueFromColumnName("Status");
                        databaseResponse.BasicExcess                           = reader.GetDbValueFromColumnName("BasicExcess");
                        databaseResponse.HasWaterSkiingAndFlotationDeviceCover = reader.GetDbValueFromColumnName("HasWaterSkiingAndFlotationDeviceCover");
                        databaseResponse.HasRacingCover                        = reader.GetDbValueFromColumnName("HasRacingCover");
                        databaseResponse.OldHasAlarmShouldBeNull               = reader.GetDbValueFromColumnName("OldHasAlarmShouldBeNull");
                        databaseResponse.OldHasGpsShouldBeNull                 = reader.GetDbValueFromColumnName("OldHasGpsShouldBeNull");
                        databaseResponse.VehicleUsageDscShouldBeNull           = reader.GetDbValueFromColumnName("VehicleUsageDscShouldBeNull");
                    }
                }
            }
            catch (Exception e) when (e is ArgumentException || e is UnauthorizedAccessException || e is FileNotFoundException
                                        || e is NotSupportedException || e is IndexOutOfRangeException || e is NullReferenceException
                                        || e is InvalidDataException || e is SqlException || e is ArgumentNullException || e is FormatException)
            {
                Reporting.Error($"SQL error encountered: in FetchBoatQuoteDetails ({quoteNumber}) {e.Message}");
            }
            return databaseResponse;
        }

        /// <summary>
        /// Fetch details from an existing Boat Policy to verify stored properties.
        /// </summary>
        /// <param name="policyNumber">The policy number to fetch details for.</param>
        public static BoatQuoteValues FetchBoatPolicyDetails(string policyNumber)
        {
            var databaseResponse = new BoatQuoteValues();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Policies\\Boat_Policy_Details_from_Database_using_Policy_number.sql");
                var queryQteNum = ShieldDB.SetSqlParameterForPolicyNumber(policyNumber);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryQteNum);

                    while (reader.Read())
                    {
                        //TODO Verify via Shield API where possible
                        databaseResponse.PolicyUpdateUser                      = reader.GetDbValueFromColumnName("PolicyUpdateUser");
                        databaseResponse.PolicyNumber                          = reader.GetDbValueFromColumnName("PolicyNumber");
                        databaseResponse.Status                                = reader.GetDbValueFromColumnName("Status");
                        databaseResponse.BasicExcess                           = reader.GetDbValueFromColumnName("BasicExcess");
                        databaseResponse.HasWaterSkiingAndFlotationDeviceCover = reader.GetDbValueFromColumnName("HasWaterSkiingAndFlotationDeviceCover");
                        databaseResponse.HasRacingCover                        = reader.GetDbValueFromColumnName("HasRacingCover");
                        databaseResponse.OriginalChannel                       = reader.GetDbValueFromColumnName("OriginalChannel");
                        databaseResponse.Discount                              = reader.GetDbValueFromColumnName("Discount");
                        databaseResponse.Ncb                                   = reader.GetDbValueFromColumnName("Ncb");
                        databaseResponse.OldHasAlarmShouldBeNull               = reader.GetDbValueFromColumnName("OldHasAlarmShouldBeNull");
                        databaseResponse.OldHasGpsShouldBeNull                 = reader.GetDbValueFromColumnName("OldHasGpsShouldBeNull");
                        databaseResponse.VehicleUsageDscShouldBeNull           = reader.GetDbValueFromColumnName("VehicleUsageDscShouldBeNull");
                    }
                }
            }
            catch (Exception e) when (e is ArgumentException || e is UnauthorizedAccessException || e is FileNotFoundException
                                        || e is NotSupportedException || e is IndexOutOfRangeException || e is NullReferenceException
                                        || e is InvalidDataException || e is SqlException || e is ArgumentNullException || e is FormatException)
            {
                Reporting.Error($"SQL error encountered: in FetchBoatQuoteDetails ({policyNumber}) {e.Message}");
            }
            return databaseResponse;
        }
    }
}
