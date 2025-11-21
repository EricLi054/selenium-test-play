using Newtonsoft.Json;
using System.Collections.Generic;

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

            isEqual &= CompareStringProperty(payloadInstanceOne.Title,       payloadInstanceTwo.Title,       "Title");
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
            if ((payloadInstanceOne.Membership != null && payloadInstanceTwo.Membership == null) ||
                (payloadInstanceOne.Membership == null && payloadInstanceTwo.Membership != null))
            {
                Reporting.Log("One payload had RAC Membership details recorded while the other did not.");
                isEqual = false;
            }
            if (payloadInstanceOne.Membership != null && payloadInstanceTwo.Membership != null)
            {
                var membershipOne = payloadInstanceOne.Membership;
                var membershipTwo = payloadInstanceTwo.Membership;
                isEqual &= CompareNullableIntProperty(membershipOne.Tenure, membershipTwo.Tenure, "Tenure");
                isEqual &= CompareStringProperty(membershipOne.Tier,   membershipTwo.Tier,   "Tier");
                isEqual &= CompareStringProperty(membershipOne.Number, membershipTwo.Number, "Membership number");
            }

            return isEqual;
        }

        private static bool CompareStringProperty(string expectedValue, string actualValue, string propertyName)
        {
            var expectedValueWithDelimiters = $"'{expectedValue}'";
            Reporting.Log($"Comparing property - {propertyName, 15}: {expectedValueWithDelimiters, 20} == '{actualValue}'.");

            return string.Equals(expectedValue, actualValue);
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
            Reporting.Log($"Comparing property - {propertyName,15}: {expectedValueWithDelimiters,20} == '{actualValue.Value}'.");

            return expectedValue.Value == actualValue.Value;
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
        public string buildingName { get; set; }
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
        public string formattedAddress { get; set; }

        public ContactServicePostalAddress()
        {
            Country = DEFAULT_COUNTRY;
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

    }
}
