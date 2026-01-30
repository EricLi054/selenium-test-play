using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rac.TestAutomation.Common.API
{
    public class ICSContactPayload
    {
        [JsonProperty("bankAccounts")]
        public List<BankAccount> BankAccounts { get; set; }
        [JsonProperty("creditCards")]
        public List<CreditCard> CreditCards { get; set; }
        [JsonProperty("linkedIds")]
        public List<string> LinkedIds { get; set; }
        [JsonProperty("shieldExternalNumber")]
        public string ShieldExternalNumber { get; set; }
        [JsonProperty("dateOfBirth")]
        public string DateOfBirth { get; set; }
        [JsonProperty("gender")]
        public string Gender { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty("membership")]
        public ContactServiceMembership Membership { get; set; }
        [JsonProperty("postalAddress")]
        public ContactServicePostalAddress PostalAddress { get; set; }
        [JsonProperty("personId")]
        public string PersonId { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("middleName")]
        public string MiddleName { get; set; }
        [JsonProperty("surname")]
        public string Surname { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Takes a object where the non-null values are to override this
        /// instance's values. The intent of this method is to support
        /// update contact tests.
        /// 
        /// Core assumptions are:
        /// * Insurance apps never delete values, they only update with a new value.
        /// * Banking/Credit Card info is never erased. We only add new payment info.
        /// </summary>
        public void ApplyNonNullChanges(ICSContactPayload updateValues)
        {
            FirstName   = ICSContactPayload.CheckForUpdatedValue(FirstName, updateValues.FirstName, "First Name");
            MiddleName  = ICSContactPayload.CheckForUpdatedValue(MiddleName, updateValues.MiddleName, "Middle Name");
            Surname     = ICSContactPayload.CheckForUpdatedValue(Surname, updateValues.Surname, "Surname");
            PhoneNumber = ICSContactPayload.CheckForUpdatedValue(PhoneNumber, updateValues.PhoneNumber, "Phone Number");
            Title       = ICSContactPayload.CheckForUpdatedValue(Title, updateValues.Title, "Title");
            Email       = ICSContactPayload.CheckForUpdatedValue(Email, updateValues.Email, "Email");
            Gender      = ICSContactPayload.CheckForUpdatedValue(Gender, updateValues.Gender, "Gender");
            DateOfBirth = ICSContactPayload.CheckForUpdatedValue(DateOfBirth, updateValues.DateOfBirth, "Date of Birth");

            if (updateValues.PostalAddress != null)
            {
                Reporting.Log($"UPDATING Postal Address from '{PostalAddress.StreetName}/{PostalAddress.Suburb}/{PostalAddress.PostCode}' to "+
                              $"'{updateValues.PostalAddress.StreetName}/{updateValues.PostalAddress.Suburb}/{updateValues.PostalAddress.PostCode}'");
                PostalAddress = updateValues.PostalAddress;
            }

            if (updateValues.BankAccounts != null)
            { BankAccounts.AddRange(updateValues.BankAccounts); }
        }

        /// <summary>
        /// Compare two instances of ICSContactPayload. If one of the objects is
        /// the test data used to create a contact record, then it should be
        /// passed as `payloadInstanceOne`
        /// </summary>
        public static bool Compare(ICSContactPayload payloadInstanceOne, ICSContactPayload payloadInstanceTwo)
        {
            var isEqual = true;
            Reporting.LogMinorSectionHeading("Comparing ICS contact payloads - General personal information.");

            if ((payloadInstanceOne == null && payloadInstanceTwo != null) || (payloadInstanceOne != null && payloadInstanceTwo == null))
            {
                Reporting.Log("ContactServicePayload: one is defined while the other is not.");
                return false;
            }

            if (payloadInstanceOne == null && payloadInstanceTwo == null) { return true; }

            isEqual &= CompareStringProperty(payloadInstanceOne.Title,       payloadInstanceTwo.Title,       "Title", ignoreCase: true);
            isEqual &= CompareStringProperty(payloadInstanceOne.FirstName,   payloadInstanceTwo.FirstName,   "First name");
            isEqual &= CompareStringProperty(payloadInstanceOne.MiddleName,  payloadInstanceTwo.MiddleName,  "Middle name");
            isEqual &= CompareStringProperty(payloadInstanceOne.Surname,     payloadInstanceTwo.Surname,     "Surname");
            isEqual &= CompareStringProperty(payloadInstanceOne.DateOfBirth, payloadInstanceTwo.DateOfBirth, "Date of birth");
            isEqual &= CompareStringProperty(payloadInstanceOne.PhoneNumber, payloadInstanceTwo.PhoneNumber, "Phone number");
            isEqual &= CompareStringProperty(payloadInstanceOne.Email,       payloadInstanceTwo.Email,       "Email");
            isEqual &= CompareStringProperty(payloadInstanceOne.Gender,      payloadInstanceTwo.Gender,      "Gender");

            Reporting.LogMinorSectionHeading("Comparing ICS contact payloads - Postal address information.");
            if ((payloadInstanceOne.PostalAddress != null && payloadInstanceTwo.PostalAddress == null) ||
                (payloadInstanceOne.PostalAddress == null && payloadInstanceTwo.PostalAddress != null))
            {
                Reporting.Log("One payload had an address recorded while the other did not.");
                isEqual = false;
            }
            if (payloadInstanceOne.PostalAddress != null && payloadInstanceTwo.PostalAddress != null)
            {
                var addressOne = payloadInstanceOne.PostalAddress;
                var addressTwo = payloadInstanceTwo.PostalAddress;
                isEqual &= CompareStringProperty(addressOne.UnitNumber,  addressTwo.UnitNumber,  "Unit number");
                isEqual &= CompareStringProperty(addressOne.HouseNumber, addressTwo.HouseNumber, "House number");
                isEqual &= CompareStringProperty(addressOne.StreetName,  addressTwo.StreetName,  "Street name");
                isEqual &= CompareStringProperty(addressOne.POBox,       addressTwo.POBox,       "PO Box");
                isEqual &= CompareStringProperty(addressOne.Suburb,      addressTwo.Suburb,      "Suburb");
                isEqual &= CompareStringProperty(addressOne.State,       addressTwo.State,       "State");
                isEqual &= CompareStringProperty(addressOne.PostCode,    addressTwo.PostCode,    "Postcode");
                isEqual &= CompareStringProperty(addressOne.Country,     addressTwo.Country,     "Australia");
            }

            Reporting.LogMinorSectionHeading("Comparing ICS contact payloads - RAC Membership.");
            var membershipOne = payloadInstanceOne.Membership ?? new ContactServiceMembership() { Tier = "None" };
            var membershipTwo = payloadInstanceTwo.Membership;

            if (payloadInstanceTwo.Membership == null)
            {
                Reporting.Log("We always expect a value for Membership to always be returned, even for anonymous contacts.");
                isEqual = false;
            }
            else
            {
                isEqual &= CompareNullableIntProperty(membershipOne.Tenure, membershipTwo.Tenure, "Tenure");
                isEqual &= CompareStringProperty(membershipOne.Tier,   membershipTwo.Tier,   "Tier", ignoreCase: true);
                isEqual &= CompareStringProperty(membershipOne.Number, membershipTwo.Number, "Membership number");
            }

            return isEqual;
        }

        public static bool CompareBankAccounts(ICSContactPayload payloadInstanceOne, ICSContactPayload payloadInstanceTwo)
        {
            var expectedBankAccounts = payloadInstanceOne.BankAccounts;
            var actualBankAccounts   = payloadInstanceTwo.BankAccounts;
            int expectedAccountCount = expectedBankAccounts == null ? 0 : expectedBankAccounts.Count;
            int actualAccountCount   = actualBankAccounts == null ? 0 : actualBankAccounts.Count;
            var areEqual = CompareNullableIntProperty(expectedAccountCount, actualAccountCount, "count of bank accounts");

            if (expectedBankAccounts != null && actualBankAccounts != null)
            {
                foreach(var providedAccount in expectedBankAccounts)
                {
                    Reporting.Log($"====> Checking Bank Account BSB:{providedAccount.Bsb}, ACC:{providedAccount.AccountNumber}");
                    var foundAccount = actualBankAccounts.FirstOrDefault(x => string.Equals(providedAccount.Bsb, x.Bsb) && string.Equals(providedAccount.AccountNumber, x.AccountNumber));
                    if (foundAccount == null)
                    {
                        areEqual = false;
                        Reporting.Log("Not found.");
                        continue;
                    }
                    areEqual &= CompareStringProperty(providedAccount.Bsb, foundAccount.Bsb, "BSB number");
                    areEqual &= CompareStringProperty(providedAccount.AccountNumber, foundAccount.AccountNumber, "Account number");
                    areEqual &= CompareStringProperty(providedAccount.AccountName, foundAccount.AccountName, "Account name");
                }
            }

            return areEqual;
        }

        /// <summary>
        /// Does not copy:
        /// * banking and credit card information
        /// 
        /// Does not copy Member Central only information
        /// * linked Shield Ids
        /// * Person Id
        /// 
        /// Does not copy Shield specific information
        /// * Shield External Number
        /// 
        /// </summary>
        /// <param name="originalVersion">ICS contact payload that we wish to duplicate</param>
        public static ICSContactPayload Duplicate(ICSContactPayload originalVersion)
        {
            var copy = DuplicateFull(originalVersion);
            // We clear these due to complexities around copying these elements.
            copy.BankAccounts  = null;
            copy.CreditCards   = null;
            // These are cleared as they are MC and Shield controlled properties.
            copy.PersonId      = null;
            copy.ShieldExternalNumber = null;
            return copy;
        }

        public static ICSContactPayload DuplicateFull(ICSContactPayload originalVersion)
        {
            var copy = JsonConvert.DeserializeObject<ICSContactPayload>(JsonConvert.SerializeObject(originalVersion));
            // We clear the LinkedIds as it is a property that is wholly controlled by MC.
            copy.LinkedIds = null;
            return copy;
        }

        private static bool CompareStringProperty(string expectedValue, string actualValue, string propertyName)
        {
            return CompareStringProperty(expectedValue, actualValue, propertyName, ignoreCase: false);
        }

        private static bool CompareStringProperty(string expectedValue, string actualValue, string propertyName, bool ignoreCase)
        {
            var caseSensitivity = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
            var expectedValueWithDelimiters = $"'{expectedValue}'";
            var equality = false;
            // The following handling is because on some fields Member Central will
            // set undefined properties to "null" and for others it will have them
            // as empty strings. "poBox" is one of these weird ones.
            if (string.IsNullOrEmpty(expectedValue))
            {
                equality = string.IsNullOrEmpty(actualValue);
            }
            else
            {
                equality = string.Equals(expectedValue, actualValue, caseSensitivity);
            }
            Reporting.Log($"Comparing property - {propertyName,15}: {equality,5} - {expectedValueWithDelimiters,20} == '{actualValue}'.");

            return equality;
        }

        private static bool CompareNullableIntProperty(int? expectedValue, int? actualValue, string propertyName)
        {
            if ((expectedValue.HasValue  && !actualValue.HasValue) ||
                (!expectedValue.HasValue && actualValue.HasValue))
            {
                Reporting.Log($"Comparing property - {propertyName,15}: don't match, one is null while the other is not");
                return false;
            }

            if (!expectedValue.HasValue && !actualValue.HasValue)
            {
                Reporting.Log($"Comparing property - {propertyName,15}: both are null");
                return true;
            }

            var expectedValueWithDelimiters = $"'{expectedValue.Value}'";
            var equality = expectedValue.Value == actualValue.Value;
            Reporting.Log($"Comparing property - {propertyName,15}: {equality,5} - {expectedValueWithDelimiters,20} == '{actualValue.Value}'.");

            return equality;
        }

        private static string CheckForUpdatedValue(string currentValue, string newValue, string propertyName)
        {
            // We only test against NULL, because empty strings are valid for removing a value.
            if (newValue != null)
            {
                Reporting.Log($"UPDATING {propertyName,15} from '{currentValue}' to '{newValue}'");
                return newValue;
            }
            return currentValue;
        }
    }

    public class ContactServiceMembership
    {
        [JsonProperty("tenure")]
        public int? Tenure { get; set; }
        [JsonProperty("tier")]
        public string Tier { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }

        public static ContactServiceMembership Duplicate(ContactServiceMembership originalVersion)
        {
            return originalVersion == null ? null :
                new ContactServiceMembership()
                {
                    Tenure = originalVersion.Tenure,
                    Tier   = originalVersion.Tier,
                    Number = originalVersion.Number
                };
        }
    }

    public class ContactServicePostalAddress
    {
        private const string DEFAULT_COUNTRY = "AUSTRALIA";
        [JsonProperty("houseNumber")]
        public string HouseNumber { get; set; }
        [JsonProperty("unitNumber")]
        public string UnitNumber { get; set; }
        [JsonProperty("blockNumber")]
        public string BlockNumber { get; set; }
        [JsonProperty("buildingName")]
        public string BuildingName { get; set; }
        [JsonProperty("isAddressValidated")]
        public bool IsAddressValidated { get; set; }
        [JsonProperty("streetName")]
        public string StreetName { get; set; }
        [JsonProperty("streetType")]
        public string StreetType { get; set; }
        [JsonProperty("streetTypeSuffix")]
        public string StreetTypeSuffix { get; set; }
        [JsonProperty("suburb")]
        public string Suburb { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("postcode")]
        public string PostCode { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("poBox")]
        public string POBox { get; set; }
        [JsonProperty("dpid")]
        public string Dpid { get; set; }
        [JsonProperty("isPreferredDeliveryMethod")]
        public bool IsPreferredDeliveryMethod { get; set; }
        [JsonProperty("formattedAddress")]
        public string FormattedAddress { get; set; }

        public ContactServicePostalAddress()
        {
            Country = DEFAULT_COUNTRY;
        }

        public ContactServicePostalAddress(Address shieldAddress)
        {
            HouseNumber = shieldAddress.StreetNumber;
            StreetName  = shieldAddress.StreetOrPOBox;
            PostCode    = shieldAddress.PostCode;
            Suburb      = shieldAddress.Suburb;
            Country     = shieldAddress.Country;
            State       = shieldAddress.State;
            IsPreferredDeliveryMethod = shieldAddress.IsPreferredDeliveryMethod;
        }

        public static ContactServicePostalAddress Duplicate(ContactServicePostalAddress originalVersion)
        {
            return originalVersion == null ? null :
                new ContactServicePostalAddress()
                {
                    HouseNumber        = originalVersion.HouseNumber,
                    UnitNumber         = originalVersion.UnitNumber,
                    BlockNumber        = originalVersion.BlockNumber,
                    BuildingName       = originalVersion.BuildingName,
                    IsAddressValidated = originalVersion.IsAddressValidated,
                    StreetName         = originalVersion.StreetName,
                    StreetType         = originalVersion.StreetType,
                    StreetTypeSuffix   = originalVersion.StreetTypeSuffix,
                    Suburb             = originalVersion.Suburb,
                    State              = originalVersion.State,
                    PostCode           = originalVersion.PostCode,
                    Country            = originalVersion.Country,
                    POBox              = originalVersion.POBox,
                    Dpid               = originalVersion.Dpid,
                    IsPreferredDeliveryMethod = originalVersion.IsPreferredDeliveryMethod,
                    FormattedAddress   = originalVersion.FormattedAddress
                };
        }
    }

    public class ICSMemberMatchPayload
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("middleName")]
        public string MiddleName { get; set; }
        [JsonProperty("surname")]
        public string Surname { get; set; }
        [JsonProperty("dateOfBirth")]
        public string DateOfBirth { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }
        [JsonProperty("streetName")]
        public string StreetName { get; set; }
        [JsonProperty("suburb")]
        public string Suburb { get; set; }
        [JsonProperty("poBox")]
        public string POBox { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
