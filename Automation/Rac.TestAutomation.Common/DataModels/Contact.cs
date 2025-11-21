using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Rac.TestAutomation.Common
{
    public class Contact
    {
        private string _homePhoneNumber;
        private string _workPhoneNumber;

        public Contact()
        {
            IsLegalEntity = false;
            Title = Title.None;
        }
        public Contact(string contactId) : this()
        {
            Id = contactId;
        }

        [JsonProperty("PersonId")]
        public string PersonId { get; set; }
        [JsonProperty("id")]
        public string Id                    { get; set; }
        [JsonProperty("externalContactNumber")]
        public string ExternalContactNumber { get; set; }
        [JsonIgnore]
        public Title Title        { get; set; }        
        /// <summary>
        /// Supports usage of this class in API comms where
        /// it is serialized and deserialised into JSON
        /// </summary>
        [JsonProperty("title")]
        public string TitleString
        {
            get
            {
                if (Title == Title.None)
                { 
                    return Title.None.GetDescription(); 
                }
                return Title.GetDescription();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Title = Title.None; 
                }
                else
                {
                    try
                    { 
                        Title = DataHelper.GetValueFromDescription<Title>(value); 
                    }
                    catch
                    {
                        Title = Title.None;
                    }
                }
            }
        }
        [JsonProperty("initial")]
        public string Initial               { get; set; }
        [JsonProperty("firstName")]
        public string FirstName             { get; set; }
        [JsonProperty("middleName")]
        public string MiddleName            { get; set; }
        [JsonProperty("surname")]
        public string Surname               { get; set; }
        [JsonIgnore]
        public Gender Gender      { get; set; }
        /// <summary>
        /// Supports usage of this class in API comms where
        /// it is serialized and deserialised into JSON
        /// </summary>
        [JsonProperty("gender")]
        public string GenderString
        {
            get => Gender.GetDescription();
            set 
            {
                if (string.IsNullOrEmpty(value)) return;

                try
                {
                    Gender = DataHelper.GetValueFromDescription<Gender>(value);
                }
                catch
                {
                    return;
                }
            }
        }
        [JsonProperty("mobilePhoneNumber")]
        public string MobilePhoneNumber     { get; set; }
        [JsonProperty("newMobilePhoneNumber")]
        public string NewMobilePhoneNumber { get; set; }
        [JsonProperty("homePhoneNumber")]
        public string HomePhoneNumber
        { 
            get => _homePhoneNumber;
            set => _homePhoneNumber = SetLandlinePhoneNumber(value);
        }
        [JsonProperty("workPhoneNumber")]
        public string WorkPhoneNumber
        {
            get => _workPhoneNumber;
            set => _workPhoneNumber = SetLandlinePhoneNumber(value);
        }
        [JsonProperty("privateEmail")]
        public Email PrivateEmail           { get; set; }
        [JsonProperty("loginEmail")]
        public string LoginEmail            { get; set; }
        [JsonIgnore]
        public DateTime DateOfBirth         { get; set; }
        [JsonProperty("dateOfBirth")]
        public string DateOfBirthString
        {
            get => DateOfBirth.ToString(DataFormats.DATE_FORMAT_REVERSE_HYPHENS);
            set => DateOfBirth = string.IsNullOrEmpty(value) ?
                                 DateTime.MinValue :
                                 DateTime.Parse(value);
        }
        [JsonProperty("contactRoles")]
        public List<ContactRoleDetails> Roles { get; set; }
        [JsonProperty("mailingAddress")]
        public Address  MailingAddress { get; set; }
        [JsonProperty("isLegalEntity")]
        public bool IsLegalEntity { get; set; }
        [JsonProperty("legalEntityName")]
        public string LegalEntityName { get; set; }
        [JsonProperty("isCrmPreferred")]
        public bool IsCrmPreferred { get; set; }
        [JsonProperty("preferredDeliveryMethod")]
        public string PreferredDeliveryMethod { get; set; }
        [JsonProperty("isCrmManaged")]
        public bool IsCrmManaged { get; set; }

        /// <summary>
        /// Check both membership tier and number. Especially as MC will 
        /// always provide a "membership number" even if there is no RSA.
        /// </summary>
        public bool IsRACMember => MembershipTier != MembershipTier.None && !string.IsNullOrEmpty(MembershipNumber);

        [JsonProperty("membershipNumber")]
        public string MembershipNumber        { get; set; }
        /// <summary>
        /// Used to specify if user intends to click 'Skip' for 'Are you an RAC member?' question
        /// in 'Before we get started' page while starting a Motorcycle insurance quote process
        /// </summary>
        [JsonIgnore]
        public bool SkipDeclaringMembership  { get; set; }
        [JsonIgnore]
        public MembershipTier MembershipTier        { get; set; }
        /// <summary>
        /// Supports usage of this class in API comms where
        /// it is serialized and deserialised into JSON
        /// </summary>
        [JsonProperty("membershipTier")]
        public string MembershipTierString
        {
            get
            {
                if (MembershipTier == MembershipTier.None)
                    return null;
                return MembershipTier.GetDescription();
            }
            set
            {
                MembershipTier = string.IsNullOrEmpty(value) ? MembershipTier.None :
                                                               DataHelper.GetValueFromDescription<MembershipTier>(value);
            }
        }

        [JsonProperty("bankAccounts")]
        public List<BankAccount> BankAccounts { get; set; }
        [JsonProperty("creditCards")]
        public List<CreditCard> CreditCards   { get; set; }

        /// <summary>
        /// Relevant in DB validations only. Not test data input.
        /// </summary>
        [JsonIgnore]
        public bool IsPolicyHolder { get; set; }
        public bool IsMultiMatchRSAMember { get; set; } //Used to identify if the RSA member is a Multi-Match in Member Central.

        public MemberMatchRule MemberMatchRule { get; set; }

        /// <summary>
        /// To be used as an input driver in method ChangeNameCase().
        /// </summary>
        public enum CaseOptions
        {
            UpperCase,
            TitleCase,
            CamelCase
        };

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            if (IsLegalEntity)
            {
                formattedString.AppendLine($"    LegalEntityName:    {LegalEntityName}{Reporting.HTML_NEWLINE}");
            }
            else
            {
                formattedString.AppendLine($"    Name:    {GetFullTitleAndName()}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Gender:  {Gender}<br>");
                formattedString.AppendLine($"    DoB:     {DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH)}{Reporting.HTML_NEWLINE}");
            }
            if (MailingAddress != null)
            { formattedString.AppendLine($"    Address: {MailingAddress.StreetSuburbPostcode(expandUnitAddresses: false)}{Reporting.HTML_NEWLINE}"); }
            formattedString.AppendLine($"    Member:  {IsRACMember} / {MembershipTier}({MembershipNumber}){Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Mobile:  {MobilePhoneNumber}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Home ph: {HomePhoneNumber}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Email:   {GetEmail()}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            return formattedString.ToString();
        }

        /*******************************************************************
        B2C-2707
        If transformation of — & ’ occurs before presenting Policy Summary
        then will need to change prior to invoking this.
        If not prior to Summary must be After this is complete.
        *******************************************************************/
        /// <summary>
        /// Returns the full title and name of the contact.
        /// Supports:
        /// - B2C UI supported titles (Mr, Miss, Mrs, Ms, Mx, Dr)
        /// - No title
        /// - Any other title outside the above will be changed to "Mx"
        /// If the title is "None", the returned name will not have a leading whitespace.
        /// </summary>
        /// <param name="shortenedTitle">Indicates whether to use a shortened title.</param>
        /// <returns>The full title and name of the contact.</returns>
        public string GetFullTitleAndName(bool shortenedTitle = true)
        {
            string titlePrefix = !DataHelper.IsValidTitle(Title)
                ? $"{Title.Mx.GetDescription()} "
                : $"{Title.GetDescription()} ";

            return (titlePrefix + GetFullName()).Trim();
        }

        /// <summary>
        /// Returns the full name including middle name if it's available
        /// </summary>        
        public string GetFullName()
        {
            string name = "";
            name += string.IsNullOrEmpty(FirstName) ? "" : FirstName + " ";
            name += string.IsNullOrEmpty(MiddleName) ? "" : MiddleName + " ";
            name += string.IsNullOrEmpty(Surname) ? "" : Surname;
            return name.TrimEnd();
        }

        /// <summary>
        /// Helper to compare the Contact's full name against static strings
        /// that we may be getting back from B2C/Spark UI.
        /// </summary>
        /// <returns></returns>
        public bool EqualsFullName(string providedNameString)
        {
            var isAMatch = GetFullTitleAndName()
                           .Equals(providedNameString, StringComparison.InvariantCultureIgnoreCase);

            if (!isAMatch)
            {
                isAMatch = GetFullTitleAndName(shortenedTitle: false)
                           .Equals(providedNameString, StringComparison.InvariantCultureIgnoreCase);
            }

            return isAMatch;
        }

        /// <summary>
        /// Return current age of this contact.
        /// </summary>
        /// <returns></returns>
        public int GetContactAge()
        {
            var age = DateTime.Now.Year - DateOfBirth.Year;
            if (age == 0) { return age; }

            // We're checking whether we've passed their birthday yet. If not, then decrement estimated age.
            if ((DateOfBirth.Month > DateTime.Now.Month) ||
                (DateOfBirth.Month == DateTime.Now.Month && DateOfBirth.Day > DateTime.Now.Day))
            { age--; }

            return age;
        }

        /// <summary>
        /// For a contact to be eligible to be a policyholder, they
        /// must be within a valid age range.
        /// </summary>
        public bool IsValidAgeAsPolicyholder(ShieldProductType productOfPolicy = ShieldProductType.MGP)
        {
            if (productOfPolicy == ShieldProductType.HGP ||
                productOfPolicy == ShieldProductType.PET)
            {
                return DateOfBirth != DateTime.MinValue &&
                       GetContactAge() >= Constants.Contacts.MIN_PH_AGE_HOME_PET &&
                       GetContactAge() < Constants.Contacts.MAX_PH_AGE;

            }
            return DateOfBirth != DateTime.MinValue &&
                   GetContactAge() >= Constants.Contacts.MIN_PH_AGE_VEHICLES &&
                   GetContactAge() < Constants.Contacts.MAX_PH_AGE;
        }

        /// <summary>
        /// Returns a defined email. In place, in case we support more than
        /// one email down the track.
        /// </summary>
        /// <returns></returns>
        public string GetEmail()
        {
            var email = PrivateEmail != null ? PrivateEmail.Address : null;
            return email;
        }

        /// <summary>
        /// Returns the preferred delivery method. Note Email is the default, so
        /// if nothing is defined, or if both are set as preferred, then TRUE (email)
        /// will be returned.
        /// </summary>
        /// <returns>TRUE to indicate email is PDM, otherwise FALSE for mail.</returns>
        public bool IsEmailPreferredDeliveryMethod()
        {
            var emailIsPreferred = false;
            if (PrivateEmail != null && PrivateEmail.IsPreferredDeliveryMethod.HasValue)
                emailIsPreferred = PrivateEmail.IsPreferredDeliveryMethod.Value;

            if (!emailIsPreferred)
            {
                emailIsPreferred = MailingAddress != null ? !MailingAddress.IsPreferredDeliveryMethod : true;
            }
            return emailIsPreferred;
        }

        /// <summary>
        /// Returns this contact's defined phone number. If multiple
        /// phone numbers are defined, then order of preference is:
        /// - mobile
        /// - home
        /// </summary>
        /// <returns></returns>
        public string GetPhone()
        {
            var phoneNumber = MobilePhoneNumber != null ? MobilePhoneNumber : HomePhoneNumber;
            return phoneNumber;
        }

        /// <summary>
        /// Method to allow changing the case of the contact's name
        /// to support desired variations for UAT and other test
        /// scenarios.
        /// </summary>
        /// <param name="desiredCase"></param>
        public void ChangeNameCase(CaseOptions desiredCase)
        {
            var textInfo = new CultureInfo("en-AU").TextInfo;
            switch(desiredCase)
            {
                case CaseOptions.UpperCase:
                    FirstName  = FirstName.ToUpper();
                    MiddleName = MiddleName?.ToUpper();
                    Surname    = Surname.ToUpper();
                    break;
                case CaseOptions.CamelCase:
                    FirstName  = RandomiseCharacterCase(FirstName);
                    MiddleName = string.IsNullOrEmpty(MiddleName) ? MiddleName : RandomiseCharacterCase(MiddleName);
                    Surname    = RandomiseCharacterCase(Surname);
                    // Do nothing for the moment.
                    break;
                case CaseOptions.TitleCase:
                    FirstName  = textInfo.ToTitleCase(FirstName.ToLower());
                    MiddleName = string.IsNullOrEmpty(MiddleName) ? MiddleName : textInfo.ToTitleCase(MiddleName.ToLower());
                    Surname    = textInfo.ToTitleCase(Surname.ToLower());
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Returns Logged in B2C contact name format that being in E_EVENT_RACI
        /// </summary>
        /// <returns>
        /// General format:
        /// With Middle name: firstname (middlename) surname
        /// Without Middle name: firstname surname
        /// </returns>
        public string GetLoggedContact()
        {
            var firstName = string.IsNullOrWhiteSpace(FirstName) ? string.Empty : $"{FirstName} ";
            var middleName = string.IsNullOrWhiteSpace(MiddleName) ? string.Empty : $"({MiddleName}) ";
            var surname = string.IsNullOrWhiteSpace(Surname) ? string.Empty : $"{Surname}";

            return $"{firstName}{middleName}{surname}";
        }

        /// <summary>
        /// If no email has been defined, this will create an email based on the
        /// member name, and then ensure it has been sync'ed to MC/MCMock.
        /// </summary>
        public void UpdateEmailIfNotDefined()
        {
            if (string.IsNullOrEmpty(PrivateEmail?.Address))
            {
                PrivateEmail = DataHelper.RandomEmail(FirstName, Surname, Config.Get().Email.Domain);
                DataHelper.OverrideMemberEmailInMemberCentralByContactID(Id, PrivateEmail.Address);
            }
        }

        private string RandomiseCharacterCase(string inputStr)
        {
            char[] inputCharArray = inputStr.ToCharArray();

            for (int i = 0; i < inputCharArray.Length; i++)
            {
                inputCharArray[i] = Randomiser.Get.Next(0, 2) == 1 ? Char.ToUpper(inputCharArray[i]) : Char.ToLower(inputCharArray[i]);
            }

            return string.Join("", inputCharArray);
        }

        private string SetLandlinePhoneNumber(string phoneNumber) => phoneNumber?.Length < 10 ? $"{PhonePrefix.WA_SA.GetDescription()}{phoneNumber}" : phoneNumber;

        /// <summary>
        /// Initialise a Contact object from member central.
        /// </summary>
        /// <param name="contactId">shield contact id and it's mandatory field</param>
        /// <param name="externalContactNumber">shield external contact number and it's optional field</param>
        public static Contact InitFromMCByShieldId(string contactId, string externalContactNumber = null)
        {
            var person = DataHelper.GetPersonFromMemberCentralByContactId(contactId);

            if (person == null)
            { return null; }

            Contact mcContact = person.ConvertToShieldContactRecord();

            // We want to keep the Shield Id that the automation found (for policy/claim references)
            mcContact.Id                    = contactId;
            mcContact.ExternalContactNumber = externalContactNumber;
            return mcContact;
        }

        /// <summary>
        /// Initialise a Contact object from member central.
        /// </summary>
        /// <param name="contactId">shield contact id and it's mandatory field</param>
        public static Contact InitFromMCByPersonId(string personId)
        {

            var person = Task.Run(() => MemberCentral.GetInstance().GET_PersonByPersonId(personId)).GetAwaiter().GetResult();

            if (person == null)
            { return null; }

            return person.ConvertToShieldContactRecord();
        }
    }

    public class Email
    {
        private readonly int MAX_EMAIL_LENGTH = 50;
        private string _emailAddress;

        public Email() { }

        public Email(string address)
        {
            Address = address;
            IsPreferredDeliveryMethod = false;
            NewAddress = null;
        }

        [JsonProperty("address")]
        public string Address
        {
            get => _emailAddress;
            set
            {
                _emailAddress = value;
                if (value != null && value.Length > MAX_EMAIL_LENGTH)
                {
                    var atIndex = value.IndexOf('@');
                    var domain = value.Substring(atIndex);
                    _emailAddress = $"{value.Substring(0, MAX_EMAIL_LENGTH - domain.Length)}{domain}";
                }
            }
        }
        [JsonProperty("newAddress")]
        public string NewAddress { get; set; }
        [JsonProperty("isPreferredDeliveryMethod")]
        public bool? IsPreferredDeliveryMethod { get; set; }
    }

    public class BankAccount
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("bsb")]
        public string Bsb { get; set; }
        [JsonProperty("accountName")]
        public string AccountName { get; set; }
        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }
        [JsonProperty("discontinueDate")]
        public DateTime DiscontinueDate { get; set; }
        [JsonProperty("bankBranchState")]
        public string BankBranchState { get; set; }

        public BankAccount InitWithRandomValues()
        {
            var bankBranchState = GetValidBSBDetails();

            Bsb = bankBranchState.BsbNo;
            BankBranchState = bankBranchState.BranchDetails;
            AccountNumber = DataHelper.RandomNumber(1, 1000000000).ToString("D6");
            AccountName = $"{DataHelper.RandomLetters(6)} {DataHelper.RandomLetters(6)}";

            return this;
        }

        /// <summary>
        /// Function to get a random valid BSB number and branch details
        /// </summary>
        private BankBranchDetails GetValidBSBDetails()
        {
            var bankBranchState = new List<BankBranchDetails>(){
                                  new BankBranchDetails() {BsbNo = "066135", BranchDetails=  "CBA, Kwinana Town Centre WA"},
                                  new BankBranchDetails() {BsbNo = "306471", BranchDetails=  "BWA, Perth WA"},
                                  new BankBranchDetails() {BsbNo = "766526", BranchDetails=  "CBA, York WA"},
                                  new BankBranchDetails() {BsbNo = "086479", BranchDetails=  "NAB, East Victoria Park WA"},
                                  new BankBranchDetails() {BsbNo = "944007", BranchDetails=  "MEB, Melbourne VIC"},
                                  new BankBranchDetails() {BsbNo = "640000", BranchDetails=  "HUM, Albury NSW"},
                                  new BankBranchDetails() {BsbNo = "156147", BranchDetails=  "T&C, South Melbourne VIC"},
                                  new BankBranchDetails() {BsbNo = "802155", BranchDetails=  "CRU, Sydney NSW"},
                                  new BankBranchDetails() {BsbNo = "016016", BranchDetails=  "ANZ, Success WA"},
                                  new BankBranchDetails() {BsbNo = "016610", BranchDetails=  "ANZ, Carnarvon WA"},
                                  new BankBranchDetails() {BsbNo = "036024", BranchDetails=  "WBC, Mount Lawley WA"},
                                  new BankBranchDetails() {BsbNo = "036054", BranchDetails=  "WBC, Claremont WA"},
            };

            int index = Randomiser.Get.Next(bankBranchState.Count);
            var randomBankBranchState = bankBranchState.ElementAt(index);

            return randomBankBranchState;
        }

        private class BankBranchDetails
        {
            public string BsbNo { get; set; }
            public string BranchDetails { get; set; }
        }
    }

    public class CreditCard
    {
        [JsonProperty("cardNumber")]
        public string CardNumber { get; set; }
        [JsonProperty("cardExpiryDate")]
        public DateTime CardExpiryDate { get; set; }
        [JsonProperty("cardholderName")]
        public string CardholderName { get; set; }
        [JsonProperty("cvnNumber")]
        public string CVNNumber { get; set; }
        [JsonProperty("externalNumber")]
        public string ExternalNumber { get; set; }
        public CreditCardIssuer CardIssuer { get; set; }
    }

    public class ContactRoleDetails
    {
        [JsonProperty("externalCode")]
        public string ExternalCode { get; set; }
    }
}
