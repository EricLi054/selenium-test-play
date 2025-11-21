using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static Rac.TestAutomation.Common.Constants.Endorsements.Cancellations;

namespace Rac.TestAutomation.Common.API
{
    public class SystemIds
    {
        public string System { get; set; }
        public string SystemId { get; set; }
        public bool IsSynchronised { get; set; }
    }

    public class API_MemberCentralPersonV1_Response
    {
        public string MemberNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string Tier { get; set; }
    }

    public class API_MemberCentralPersonV2Address
    {
        public string BuildingName { get; set; }
        public string SubBuildingNumber { get; set; }
        public string BlockNumber { get; set; }
        public string UnitNumber { get; set; }
        public string LotNumber { get; set; }
        public string HouseNumber { get; set; }
        public string StreetName { get; set; }
        public string POBox { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public bool QASValidated { get; set; }
        public string Dpid { get; set; }
        public string FormattedAddress { get; set; }
    }

    public class API_MemberCentralPersonV2Response
    {
        // Member Central code to identify person IDs
        // linking them to a Shield contact record
        private const string SYSTEM_ID_SHIELD = "SHIELD";
        public string PersonId { get; set; }
        public string RacId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string DateOfBirth { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string PersonalEmailAddress { get; set; }
        public string Gender { get; set; }
        public string Title { get; set; }
        public string Tier { get; set; }
        public string PreferredShieldContactId { get; set; }
        public List<SystemIds> PersonSystemIds { get; set; }
        public API_MemberCentralPersonV2Address PostalAddress { get; set; }

        /// <summary>
        /// Checks the PersonSystemIds, and returns where the provided Shield
        /// Contact Id is amongst the linked system IDs. This assists us in
        /// checking that the retrieved MC record is one that the test was 
        /// expecting.
        /// </summary>
        /// <param name="id">Desired Shield ID to match against.</param>
        public bool IsExpectedShieldIdLinkedWithThisCRMRecord(string id) =>
                    PersonSystemIds != null ?
                    PersonSystemIds.Any(x => string.Equals(x.System, SYSTEM_ID_SHIELD, StringComparison.InvariantCultureIgnoreCase) && x.SystemId == id) :
                    false;

        public List<string> GetAllLinkedShieldIds() => PersonSystemIds.FindAll(x => string.Equals(x.System, SYSTEM_ID_SHIELD, StringComparison.InvariantCultureIgnoreCase)).Select(record => record.SystemId).ToList();

        public Contact ConvertToShieldContactRecord()
        {
            Contact mcContact = null;
            if (!string.IsNullOrEmpty(DateOfBirth))
            {
                mcContact = new Contact();
                mcContact.PersonId   = PersonId;
                mcContact.Id         = PreferredShieldContactId;
                mcContact.FirstName  = FirstName;
                mcContact.MiddleName = MiddleName;
                mcContact.Surname    = Surname;
                mcContact.MembershipTier    = DataHelper.ConvertMembershipTierStringToEnum(Tier);
                mcContact.MembershipNumber  = RacId;
                mcContact.TitleString       = Title;
                mcContact.GenderString      = Gender;
                mcContact.DateOfBirth       = DateTime.Parse(DateOfBirth);
                mcContact.MobilePhoneNumber = MobilePhone;
                mcContact.HomePhoneNumber   = HomePhone;

                mcContact.PrivateEmail = new Email(PersonalEmailAddress);

                mcContact.MailingAddress = new Address()
                {
                    StreetNumber = string.IsNullOrEmpty(PostalAddress.UnitNumber) ?
                                           PostalAddress.HouseNumber : 
                                           $"{PostalAddress.UnitNumber}/{PostalAddress.HouseNumber}",
                    StreetOrPOBox = string.IsNullOrEmpty(PostalAddress.StreetName) ?
                                           PostalAddress.POBox : 
                                           PostalAddress.StreetName,
                    Suburb   = PostalAddress.Suburb,
                    State    = PostalAddress.State,
                    Country  = PostalAddress.Country,
                    PostCode = PostalAddress.Postcode,
                    isAddressValidated = PostalAddress.QASValidated,
                    Dpid     = PostalAddress.Dpid,
                };
            }

            return mcContact;
        }
    }

    public class API_MemberCentralPersonV2Products
    {
        [JsonProperty("productHoldings")]
        public List<MemberCentralProduct> ProductHoldings { get; set; }

        /// <summary>
        /// Supporting method that returns TRUE if the person has a
        /// currently active RSA product.
        /// </summary>
        /// <returns></returns>
        public bool HasCurrentRSA() => ProductHoldings == null ? false :
                                       ProductHoldings.Any(x => x.ProductBusinessType == "RSA" && x.productStatus == "Active");
    }

    public class MemberCentralProduct
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("product")]
        public string Product { get; set; }
        [JsonProperty("productBusinessType")]
        public string ProductBusinessType { get; set; }
        [JsonProperty("productStatus")]
        public string productStatus { get; set; }
        [JsonProperty("productStatusReason")]
        public string productStatusReason { get; set; }
        [JsonProperty("startDate")]
        public DateTime startDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime endDate { get; set; }
    }
}
