using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace Rac.TestAutomation.Common.DatabaseCalls.Contacts
{
    public class ShieldContactDetailsDB
    {

        public static ChangeContactDetails FindContactForChangeMailingPreferences()
        {
            ChangeContactDetails result = new ChangeContactDetails();
            try
            {
                Reporting.Log($"Running FindContactWithCurrentPolicyNoMailPreferences.sql to find candidate members");
                string query = ShieldDB.ReadSQLFromFile("Contacts\\FindContactWithCurrentPolicyNoMailPreferences.sql");


                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, null);

                    while (reader.Read())
                    {
                        var contactId = reader.GetDbValueFromColumnName("ContactId");
                        var externalContactNumber = reader.GetDbValueFromColumnName("ExternalContactNumber");
                        var contact = DataHelper.MapContactWithPersonAPI(contactId, externalContactNumber);
                        if (contact != null)
                        {
                            result.Contact = contact;
                            result.PolicyNumber = reader.GetDbValueFromColumnName("PolicyNumber");
                            result.Contact.PreferredDeliveryMethod = reader.GetDbValueFromColumnName("PreferredDeliveryMethod");
                            break;
                        }                        
                    }
                    Reporting.Log($"Identified candidate Shield Contact {result.Contact.Id} to use for Update Mail Preferences test");
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

        public static ContactEvent GetLatestContactShieldEventForContactID(string contactId)
        {
            ContactEvent result = null;

            try
            {
                string query = ShieldDB.ReadSQLFromFile("Contacts\\GetUpdateContactEventByContactID.sql");

                var queryContactId = ShieldDB.SetSqlParameterForContactID(contactId);

                using (var db = ShieldDB.GetDatabaseHandle())
                {
                    var reader = db.ExecuteQuery(query, queryContactId);

                    while (reader.Read())
                    {
                        result = new ContactEvent()
                        {
                            EventType = reader.GetDbValue(0),
                            Username = reader.GetDbValue(1)
                        };

                        break;
                    }
                    Reporting.Log($"We get Event Type and Username for the Change Contact Details");
                }
            }
            catch(Exception e) when(e is ArgumentException || e is UnauthorizedAccessException || e is FileNotFoundException
                                        || e is NotSupportedException || e is IndexOutOfRangeException || e is NullReferenceException
                                        || e is InvalidDataException || e is SqlException || e is ArgumentNullException || e is FormatException)
            {
                Reporting.Log($"Exception Occured:{e.Message}");
            }

            return result;
        }

        public static Contact GetContactByContactID(string contactId)
        {
            Contact contact = null;
            string query = ShieldDB.ReadSQLFromFile("Contacts\\GetContactByContactID.sql");
            var queryContactId = ShieldDB.SetSqlParameterForContactID(contactId);

            using (var db = ShieldDB.GetDatabaseHandle())
            {
                var reader = db.ExecuteQuery(query, queryContactId);
                while (reader.Read())
                {
                    contact = new PolicyContactDB()
                    {
                        TitleString = reader.GetDbValueFromColumnName("Title"),
                        FirstName = reader.GetDbValueFromColumnName("first_name"),
                        MiddleName = reader.GetDbValueFromColumnName("middle_name"),
                        Surname = reader.GetDbValueFromColumnName("Surname"),
                        DateOfBirthString = reader.GetDbValueFromColumnName("DateOfBirth"),
                        GenderString = reader.GetDbValueFromColumnName("Gender"),
                        MailingAddress = new Address()
                        {
                            StreetNumber = reader.GetDbValueFromColumnName("House_nr"),
                            StreetOrPOBox = reader.GetDbValueFromColumnName("Street"),
                            Suburb = reader.GetDbValueFromColumnName("Suburb"),
                            PostCode = reader.GetDbValueFromColumnName("Postcode")
                        },
                        Id = reader.GetDbValueFromColumnName("ContactId")
                    };
                    contact.PrivateEmail = new Email(reader.GetDbValueFromColumnName("PrivateEmail"));
                    contact.MobilePhoneNumber = reader.GetDbValueFromColumnName("MobilePhone");
                    contact.HomePhoneNumber = reader.GetDbValueFromColumnName("HomePhone");
                    contact.WorkPhoneNumber = reader.GetDbValueFromColumnName("WorkPhone");
                    contact.MembershipTierString = reader.GetDbValueFromColumnName("MemberStatus");
                    contact.MembershipNumber = reader.GetDbValueFromColumnName("MemberNo");
                    contact.PreferredDeliveryMethod = reader.GetDbValueFromColumnName("PreferredDeliveryMethod");
                }
            }

            return contact;
        }

        public class ContactEvent
        {
            public string EventType { get; set; }
            public string Username { get; set; }
        }

    }
    
}
