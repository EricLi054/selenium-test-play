using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using static Rac.TestAutomation.Common.Constants.Contacts;

namespace Rac.TestAutomation.Common.DatabaseCalls.Contacts
{
    public class ShieldContacts
    {
        private class Constants
        {
            // Maximum RAC Roadside Membership tiers allowed in a SQL search.
            public const int MaxTiersInSearch = 5;
            public class SQL
            {
                // Shield codes representing contact gender
                public const string Male = "2";
                public const string Female = "1";
            }

            /*
             * due to the length of the service account names, the SAM account
             * name had to be truncated, hence the difference between AD and SAM account names.
             */
            public const string ShieldUserPrefix = "svc.shieldauto";
            public static Dictionary<string, string> ShieldTestUsers = new Dictionary<string, string>()
            {
                { ShieldUserPrefix + "1", "svc.shieldautomation1@rac.com.au" },
                { ShieldUserPrefix + "2", "svc.shieldautomation2@rac.com.au" },
                { ShieldUserPrefix + "3", "svc.shieldautomation3@rac.com.au" }
            };
        }
        

        /// <summary>
        /// Looks first in Shield for contacts with RSA, then checks them
        /// against Member Central to ensure that the RSA details match any Member Match Rule,
        /// and that they are also a "single match" to make sure they're
        /// suitable to use in B2C/Spark test cases.
        /// </summary>
        /// <param name="minAge">The minimum age to search for (defaults to 18yo)</param>
        /// <param name="maxAge">The maximum age to search for (defaults to 100yo)</param>
        /// <param name="gender">if NULL, then any gender, otherwise restrict search to given gender</param>
        /// <param name="membershipTiers">This is a list of RSA memberships</param>
        /// <returns>Returns the first contact that matches the given criteria and is a single match in MC</returns>
        /// <exception cref="Exception">If no data is found or if there was an error connecting to the database.</exception>
        public static Contact FetchAContactWithRACMembershipTier(int minAge = 18, int maxAge = 100, Gender? gender = null, params MembershipTier[] membershipTiers)
        {
            var validContact = ReturnAContactWithRACMembership(minAge, maxAge, gender, membershipTiers: membershipTiers);

            Reporting.IsNotNull(validContact, "that a suitable contact was found");
            return validContact;
        }

        /// <summary>
        /// Searches for all Contacts that have a specific first and last name.
        /// Intended to support maintenance tasks when creating automation
        /// users in Shield, this assists in determining whether the specific
        /// user already exists and is a staff member.
        /// 
        /// For multiple returned results, they are grouped by contact ID. 
        /// Multiple results occur when there is more than one role on that
        /// contact.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="surname"></param>
        /// <returns></returns>
        public static List<PolicyContactDB> FindContactByName(string firstName, string lastName)
        {
            var matchingContacts = new List<PolicyContactDB>();

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Contacts\\FindByName.sql");

                var queryNameDetails = new Dictionary<string, string>()
                {
                    { "firstName", firstName },
                    { "lastName",  lastName }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryNameDetails);

                    while (reader.Read())
                    {
                        var contactId = reader.GetDbValue(0);
                        var foundContact = matchingContacts.Where(x => x.Id == contactId).FirstOrDefault();
                        var role = ContactRole.StaffMember;

                        try
                        {
                            role = ContactRoleIdentifiers.First(x => x.Value == int.Parse(reader.GetDbValue(4))).Key;
                        }
                        catch(Exception e) when(e is ArgumentNullException || e is OverflowException || e is FormatException)
                        {
                            Reporting.Error($"When parsing policy contacts, failed to process role: {reader.GetDbValue(4)} for contact: {firstName} {lastName}" +
                                $"Exception Occured :{e.Message}");                           
                        }

                        if (foundContact == null)
                        {
                            foundContact = new PolicyContactDB()
                            {
                                Id = reader.GetDbValue(0),
                                FirstName = reader.GetDbValue(1),
                                Surname = reader.GetDbValue(2),
                                DateOfBirth = DateTime.Parse(reader.GetDbValue(3)),
                                ContactRoles = new List<ContactRole>()
                            };
                            foundContact.ContactRoles.Add(role);
                            matchingContacts.Add(foundContact);
                        }
                        else
                        {
                            matchingContacts.Remove(foundContact);
                            foundContact.ContactRoles.Add(role);
                            matchingContacts.Add(foundContact);
                        }
                    }
                }
            }
            catch (Exception e) when (e is ArgumentException || e is UnauthorizedAccessException || e is FileNotFoundException
                                       || e is NotSupportedException || e is IndexOutOfRangeException || e is NullReferenceException
                                       || e is InvalidDataException || e is SqlException || e is ArgumentNullException || e is FormatException)
            {
                Reporting.Log($"Exception Occured:{e.Message}");
            }

            return matchingContacts;
        }

        /// <summary>
        /// This method searches for the first available test automation
        /// Shield user login. It uses the coded SSO search criteria to
        /// find an available Shield login.
        /// </summary>
        /// <returns></returns>
        public static string FindAvailableShieldLogin()
        {
            string firstIdleUsername = null;
            var possibleUsernames = new List<string>();

            // Search for all configured Staff users matching the given prefix.
            try
            {
                string query = ShieldDB.ReadSQLFromFile("Contacts\\FindAllStaffContactsByUsernamePrefix.sql");

                var queryNameDetails = new Dictionary<string, string>()
                {
                    { "usernamePrefix", $"{Constants.ShieldUserPrefix}%" }
                };

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryNameDetails);

                    while (reader.Read())
                    {
                        var username = reader.GetDbValue(0);
                        if (Constants.ShieldTestUsers.ContainsKey(username))
                        possibleUsernames.Add(username);
                    }
                }
            }
            catch (Exception e) when (e is ArgumentException || e is UnauthorizedAccessException || e is FileNotFoundException
                                      || e is NotSupportedException || e is IndexOutOfRangeException || e is NullReferenceException
                                      || e is InvalidDataException || e is SqlException || e is ArgumentNullException || e is FormatException)
            {
                Reporting.Log($"Exception Occured:{e.Message}");
            }

            Reporting.IsTrue(possibleUsernames.Any(), $"Shield users (by {Constants.ShieldUserPrefix}) for automation have been created in Shield");

            // Find the first username that has not had a log event in the
            // past hour, or the most recent activity was to log out.
            foreach (var username in possibleUsernames)
            {
                try
                {
                    string query = ShieldDB.ReadSQLFromFile("Contacts\\GetLatestUserLogonSessionMessage.sql");

                    var queryNameDetails = new Dictionary<string, string>()
                    {
                        { "usernameRegex", $"%{username}" }
                    };

                    using (var db = ShieldDB.GetDatabaseHandle())
                    {
                        var reader = db.ExecuteQuery(query, queryNameDetails);
                        string status = null;
                        while (reader.Read())
                        {
                            status = reader.GetDbValue(1);
                        }
                        if (status == null ||
                            status.Equals($"User logged out {username}"))
                        {
                            firstIdleUsername = username;
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
            }

            if (string.IsNullOrEmpty(firstIdleUsername))
            { Reporting.Error("Unable to login to Shield Web UI as could not find free login."); }

            return Constants.ShieldTestUsers[firstIdleUsername];
        }

        /// <summary>
        /// This method searches for a Shield User login related to 
        /// the Contact Id provided.
        /// </summary>
        public static void VerifyContactHasUserAccountWithRole(string contactId)
        {
            string username = null;
            string query = ShieldDB.ReadSQLFromFile("Contacts\\FindShieldUserWithMasterRoleByContactId.sql");

            var queryContactId = ShieldDB.SetSqlParameterForContactID(contactId);

            using (var db = ShieldDB.GetDatabaseHandle())
            {
                var reader = db.ExecuteQuery(query, queryContactId);
                while (reader.Read())
                {
                    username = reader.GetDbValueFromColumnName("username");
                }
            }
            Reporting.IsTrue(!string.IsNullOrEmpty(username), $"whether an actual Shield Username with Master Role exists linked to Contact Id '{contactId}'");
        }

        private static Contact ReturnAContactWithRACMembership(int minAge = 18, int maxAge = 100, Gender? gender = null, MembershipTier[] membershipTiers = null)
        {
            var candidates = FetchRACMembers(minAge, maxAge, gender, membershipTiers);
            return PopulateContactDetailsFromMemberCentral(candidates);
        }

        private static List<Contact> FetchRACMembers( int minAge = 18, int maxAge = 100, Gender? gender = null, MembershipTier[] membershipTiers = null)
            => PerformShieldDBQueryToGetMembers("Contacts\\FindRSAMembers.sql", minAge, maxAge, gender, membershipTiers: membershipTiers);

        private static List<Contact> PerformShieldDBQueryToGetMembers(string sqlFile, int minAge = 18, int maxAge = 100, Gender? gender = null, MembershipTier[] membershipTiers = null)
        {
            List<Contact> foundMembers = new List<Contact>();

            if (membershipTiers.Length > Constants.MaxTiersInSearch)
            { Reporting.Error($"Cannot search for more tiers than {Constants.MaxTiersInSearch}"); }

            string membershipTierCriteria = string.Empty;
            foreach (var tier in membershipTiers)
            {
                if (string.IsNullOrEmpty(membershipTierCriteria))
                { membershipTierCriteria = $"'{tier.GetDescription()}'"; }
                else
                { membershipTierCriteria += $",'{tier.GetDescription()}'"; }
            }

            var query = ShieldDB.ReadSQLFromFile(sqlFile);

            try
            {
                var queryParameters = new Dictionary<string, string>();
                for(int i = 1; i <= Constants.MaxTiersInSearch; i++)
                {
                    if (i <= membershipTiers.Length)
                        queryParameters.Add($"membershipTier{i}", membershipTiers[i - 1].GetDescription());
                    else
                        queryParameters.Add($"membershipTier{i}", "IGNORE_ME");
                };

                var genderCode1 = Constants.SQL.Male;
                var genderCode2 = Constants.SQL.Female;
                if (gender.HasValue)
                {
                    if (gender.Value == Gender.Male)
                    { genderCode2 = Constants.SQL.Male; }

                    if (gender.Value == Gender.Female)
                    { genderCode1 = Constants.SQL.Female; }
                }
                queryParameters.Add($"Gender1", genderCode1);
                queryParameters.Add($"Gender2", genderCode2);

                SetBirthDateRangeInSqlParameters(queryParameters, minAge, maxAge);

                // MSSQL does not have a boolean type.
                int includeContactsWithNoMembership = membershipTiers.Contains(MembershipTier.None) ? 1 : 0;
                queryParameters.Add($"IncludeNoMembership", includeContactsWithNoMembership.ToString());

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryParameters);
                    while (reader.Read())
                    {
                        var dob = $"{reader.GetDbValueFromColumnName("DoBDay")}/{reader.GetDbValueFromColumnName("DoBMonth")}/{reader.GetDbValueFromColumnName("DoBYear")}";
                        var thisMember = new Contact()
                        {
                            Id = reader.GetDbValueFromColumnName("id"),
                            ExternalContactNumber = reader.GetDbValueFromColumnName("EXTERNAL_CONTACT_NUMBER"),
                            Title = DataHelper.GetValueFromDescription<Title>(reader.GetDbValueFromColumnName("title")),
                            FirstName = reader.GetDbValueFromColumnName("first_name"),
                            MiddleName = reader.GetDbValueFromColumnName("middle_name"),
                            Surname = reader.GetDbValueFromColumnName("last_name"),
                            Gender = DataHelper.ConvertGenderStringToEnum(reader.GetDbValueFromColumnName("Gender")),
                            DateOfBirth = DateTime.ParseExact(dob, DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH, System.Globalization.CultureInfo.InvariantCulture),
                            MailingAddress = new Address()
                            {
                                StreetNumber = reader.GetDbValueFromColumnName("house_nr"),
                                StreetOrPOBox = reader.GetDbValueFromColumnName("street_name"),
                                Suburb = reader.GetDbValueFromColumnName("city_name"),
                                PostCode = reader.GetDbValueFromColumnName("zip")
                            },
                            MembershipTier = DataHelper.ConvertMembershipTierStringToEnum(reader.GetDbValueFromColumnName("MemberStatus")),
                            MembershipNumber = reader.GetDbValueFromColumnName("MemberNo"),
                            PrivateEmail = new Email(reader.GetDbValueFromColumnName("email")),
                            MobilePhoneNumber = reader.GetDbValueFromColumnName("MobilePhone")
                        };

                        if (membershipTiers.Contains(thisMember.MembershipTier))
                        { foundMembers.Add(thisMember); }
                        else
                        { Reporting.Log($"FYI: Contact ID {thisMember.Id} was returned with tier {thisMember.MembershipTier} instead of requested tier value(s) '{membershipTierCriteria}'."); }
                    }
                }
            }
            catch (Exception ex) when (ex is InvalidDataException || ex is SqlException ||
                                       ex is ArgumentNullException || ex is FormatException ||
                                       ex is ArgumentException)
            { Reporting.Log("FetchRACMembers Exception occurs querying DB: " + ex.Message); }

            return foundMembers;
        }

        private static Contact PopulateContactDetailsFromMemberCentral(List<Contact> shieldContacts)
        {            
            Contact validSingleMatchContact = null;

            foreach (var candidate in shieldContacts)
            {
                try
                {
                    var contact = DataHelper.MapContactWithPersonAPI(candidate.Id, candidate.ExternalContactNumber);
                    if (contact != null && (contact.MembershipTier == candidate.MembershipTier))
                    {
                        if (HasUsableMailingAddress(candidate) && 
                            (Config.Get().IsMCMockEnabled() || // DPID is only kept in real MC, not Mock
                            !string.IsNullOrEmpty(contact.MailingAddress.Dpid)))
                        {
                            validSingleMatchContact = contact;
                            // Set PreferredShieldContactId as the MC Contact ID
                            var mcRecord = DataHelper.GetPersonFromMemberCentralByContactId(candidate.Id);
                            validSingleMatchContact.Id = mcRecord.PreferredShieldContactId;
                            break;
                        }
                    }
                }
                catch
                {
                    // Failed with MC for this contact, skip to next.
                }
            }

            return validSingleMatchContact;
        }

        private static bool HasUsableMailingAddress(Contact contact)
        {
            // TODO: Address class needs support for parsing unit/apartment syntax
            // We're skipping ones with "unit" or "/" in the street number as I haven't
            // built an algorithm to handle the variants that those cause when
            // reconciling human entered and QAS formatted versions.
            var streetNum = contact.MailingAddress.StreetNumber.ToLower();
            return !streetNum.StartsWith("unit ") && !streetNum.Contains("/");
        }

        private static void SetBirthDateRangeInSqlParameters(Dictionary<string,string> queryParams, int youngestAge, int oldestAge)
        {
            // Setup the birthdate range to search between. Maximum age, means up to 1 day before their next birthday.
            var birthFrom = DateTime.Now.AddYears(-(oldestAge + 1)).AddDays(1);
            var birthTo = DateTime.Now.AddYears(-youngestAge);
            queryParams.Add($"DoBFromYear", birthFrom.Year.ToString());
            queryParams.Add($"DoBFromMonth", birthFrom.Month.ToString());
            queryParams.Add($"DoBFromDay", birthFrom.Day.ToString());
            queryParams.Add($"DoBToYear", birthTo.Year.ToString());
            queryParams.Add($"DoBToMonth", birthTo.Month.ToString());
            queryParams.Add($"DoBToDay", birthTo.Day.ToString());
        }
    }
}
