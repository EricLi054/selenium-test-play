using Rac.TestAutomation.Common.DatabaseCalls.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

using static Rac.TestAutomation.Common.Constants.Contacts;

namespace Rac.TestAutomation.Common
{
    public class ContactBuilder : EntityBuilder<Contact, ContactBuilder>
    {
        private Config _config;

        public ContactBuilder()
        {
            _config = Config.Get();
        }

        public ContactBuilder(Contact contact)
        {
            _config = Config.Get();

            WithId(contact.Id);
            WithExternalContactNumber(contact.ExternalContactNumber);
            WithTitle(contact.Title);
            WithInitial(contact.Initial);
            WithFirstName(contact.FirstName);
            WithMiddleName(contact.MiddleName);
            WithSurname(contact.Surname);
            WithDateOfBirth(contact.DateOfBirth);
            WithGender(contact.Gender);
            WithMembershipTier(contact.MembershipTier);
            WithMembershipNumber(contact.MembershipNumber);
            WithMobileNumber(contact.MobilePhoneNumber);
            WithHomePhoneNumber(contact.HomePhoneNumber);
            WithWorkPhoneNumber(contact.WorkPhoneNumber);
            WithPrivateEmail(contact.PrivateEmail);
            WithMailingAddress(contact.MailingAddress);
            WithoutDeclaringMembership(contact.SkipDeclaringMembership);
            WithMemberMatchRule(MemberMatchRule.None);
            WithMultiMatchStatus(false);

            // If bank or CC provided, we'll add them over, but a random
            // bank and CC are also still added anyways.
            if (contact.BankAccounts != null)
                WithBankAccounts(contact.BankAccounts.ToList());
            if (contact.CreditCards != null)
                WithCreditCards(contact.CreditCards.ToList());

            if (contact.PersonId != null)
            {
                WithPersonId(contact.PersonId);
            }

            WithRandomBankAccount();
            WithRandomCreditCard();
        }

        public ContactBuilder(string id)
        {
            _config     = Config.Get();
            var contact = ShieldContactDetailsDB.GetContactByContactID(id);

            WithId(contact.Id);
            WithExternalContactNumber(contact.ExternalContactNumber);
            WithGender(contact.Gender);

            if (contact.Title != Title.None)
            { WithTitle(contact.Title); }
            else
            { WithRandomTitleFromGender(); }

            if (!string.IsNullOrEmpty(contact.FirstName))
                WithFirstName(contact.FirstName);
            else
                WithRandomFirstName();
            
            WithInitial(string.IsNullOrEmpty(contact.Initial) ? contact.FirstName.Substring(0,1) : contact.Initial);
            WithMiddleName(contact.MiddleName);

            if (!string.IsNullOrEmpty(contact.Surname))
                WithSurname(contact.Surname);
            else
                WithRandomSurname();

            WithDateOfBirth(contact.DateOfBirth);
            WithMembershipTier(contact.MembershipTier);
            WithMembershipNumber(contact.MembershipNumber);

            if (!string.IsNullOrEmpty(contact.MobilePhoneNumber))
                WithMobileNumber(contact.MobilePhoneNumber);
            else
                WithRandomMobileNumber();

            if (!string.IsNullOrEmpty(contact.HomePhoneNumber))
                WithHomePhoneNumber(contact.HomePhoneNumber);
            else
                WithRandomWesternAustralianHomePhoneNumber();

            if (!string.IsNullOrEmpty(contact.WorkPhoneNumber))
                WithWorkPhoneNumber(contact.WorkPhoneNumber);

            if (contact.PrivateEmail != null)
                WithPrivateEmail(contact.PrivateEmail);
            else
                WithPrivateEmailAddressFromName();

            if (contact.MailingAddress != null)
                WithMailingAddress(contact.MailingAddress);
            else
                WithRandomMailingAddress();

            WithoutDeclaringMembership(contact.SkipDeclaringMembership);
            WithMemberMatchRule(MemberMatchRule.None);
            WithMultiMatchStatus(false);

            // If bank or CC provided, we'll add them over, but a random
            // bank and CC are also still added anyways.
            if (contact.BankAccounts != null)
                WithBankAccounts(contact.BankAccounts.ToList());
            if (contact.CreditCards != null)
                WithCreditCards(contact.CreditCards.ToList());

            WithRandomBankAccount();
            WithRandomCreditCard();
        }

        public ContactBuilder WithId(string id)
        {
            Set(x => x.Id, id);
            return this;
        }

        public ContactBuilder WithExternalContactNumber(string externalContactNumber)
        {
            Set(x => x.ExternalContactNumber, externalContactNumber);
            return this;
        }

        public ContactBuilder WithPersonId(string personId)
        {
            Set(x => x.PersonId, personId);
            return this;
        }

        public ContactBuilder WithMembershipTier(MembershipTier tier)
        {
            Set(x => x.MembershipTier, tier);
            return this;
        }

        /// <summary>
        /// Used to set with what member match rule the contact should be built
        /// To makesure member match uses only the relevent parameters for the rule,
        /// other parameters are forced to have invalid values.
        /// e.g:- If contact trying to match using Rule 1: (First name, Date of birth and Mobile),
        /// we deliberately provide a different email id here to test if only Rule 1 was actually triggered during contact matching.
        /// </summary>
        /// <param name="memberMatchRule"></param>
        /// <returns></returns>
        public ContactBuilder WithMemberMatchRule(MemberMatchRule memberMatchRule)
        {
            switch (memberMatchRule)
            {
                //If contact trying to match using Rule 1: (First name, Date of birth and Mobile),
                //we deliberately provide a different email id here to test if only Rule 1 was actually triggered during contact matching.
                case MemberMatchRule.Rule1:
                    WithPrivateEmailAddressFromName();
                    break;
                //If contact trying to match using Rule 2: (First name, Date of birth and Email),
                //we deliberately provide a different phone number here to test if only Rule 2 was actually triggered during contact matching.
                case MemberMatchRule.Rule2:
                    WithMobileNumber("0400000000");
                    break;
                //If contact trying to match using Rule 3: (First name, Date of birth and Postal Address),
                //we deliberately provide a different phone number and an email here to test if only Rule 3 was actually triggered during contact matching.
                case MemberMatchRule.Rule3:
                    WithMobileNumber("0400000000");
                    WithPrivateEmailAddressFromName();
                    break;
                default:
                    break;
            }

            Set(x => x.MemberMatchRule, memberMatchRule);
            return this;
        }

        public ContactBuilder WithMembershipNumber(string number)
        {
            Set(x => x.MembershipNumber, number);
            return this;
        }

        public ContactBuilder WithoutDeclaringMembership(bool isDeclaringMembershipSkipped)
        {
            Set(x => x.SkipDeclaringMembership, isDeclaringMembershipSkipped);
            return this;
        }

        public ContactBuilder WithMembership(MembershipTier tier, string number)
        {
            return WithMembershipTier(tier)
                .WithMembershipNumber(number);
        }

        public ContactBuilder WithoutMembershipTier()
        {
            return WithMembershipTier(MembershipTier.None);
        }

        public ContactBuilder WithRandomMembershipTier()
        {
            return WithMembershipTier((MembershipTier)Randomiser.Get.Next((Enum.GetValues(typeof(MembershipTier))).Length));
        }

        public ContactBuilder WithoutMembershipNumber()
        {
            return WithMembershipNumber(null);
        }

        public ContactBuilder WithoutMembership()
        {
            return WithoutMembershipTier()
                .WithoutMembershipNumber();
        }

        public ContactBuilder WithTitle(Title title)
        {
            Set(x => x.Title, title);
            return this;
        }

        public ContactBuilder WithTitle(string title)
        {
            return WithTitle(DataHelper.GetValueFromDescription<Title>(title));
        }

        public ContactBuilder WithRandomTitleFromGender()
        {
            return WithTitle(DataHelper.GetRandomTitleForGender(Get(x => x.Gender), excludeMxTitle: false));
        }

        public ContactBuilder WithInitial(string initial)
        {
            Set(x => x.Initial, initial);
            return this;
        }

        public ContactBuilder WithoutInitial()
        {
            return WithInitial(null);
        }

        public ContactBuilder WithInitialFromFirstName()
        {
            return WithInitial(GetOrDefault(x => x.FirstName)?[0].ToString());
        }

        public ContactBuilder WithFirstName(string firstName)
        {
            Set(x => x.FirstName, firstName);
            return this;
        }

        public ContactBuilder WithRandomFirstName()
        {
            return WithFirstName(DataHelper.RandomLetters(4, 15).FirstCharToUpper());
        }

        public ContactBuilder WithMiddleName(string middleName)
        {
            Set(x => x.MiddleName, middleName);
            return this;
        }

        public ContactBuilder WithRandomMiddleName()
        {
            return WithMiddleName(DataHelper.RandomLetters(4, 15).FirstCharToUpper());
        }

        public ContactBuilder WithoutMiddleName()
        {
            Set(x => x.MiddleName, null);
            return this;
        }

        public ContactBuilder WithSurname(string surname)
        {
            Set(x => x.Surname, surname);
            return this;
        }

        public ContactBuilder WithoutSurname()
        {
            return WithSurname(null);
        }

        public ContactBuilder WithRandomSurname()
        {
            return WithSurname(DataHelper.RandomLetters(4, 15).FirstCharToUpper());
        }

        public ContactBuilder WithDateOfBirth(DateTime dateOfBirth)
        {
            Set(x => x.DateOfBirth, dateOfBirth);
            return this;
        }

        public ContactBuilder WithRandomDateOfBirth(int age)
        {
            return WithDateOfBirth(DataHelper.RandomDoB(age));
        }

        public ContactBuilder WithRandomDateOfBirth(int minAge, int maxAge)
        {
            return WithDateOfBirth(DataHelper.RandomDoB(minAge, maxAge));
        }

        /// <summary>
        /// Set gender
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public ContactBuilder WithGender(Gender gender)
        {
            Set(x => x.Gender, gender);
            return this;
        }

        public ContactBuilder AsMale()
        {
            return WithGender(Gender.Male);
        }

        public ContactBuilder AsFemale()
        {
            return WithGender(Gender.Female);
        }

        public ContactBuilder WithRandomGender()
        {
            var genderOptions = new[]
            {
                Gender.Male,
                Gender.Female
            };

            return WithGender(genderOptions.OrderBy(t => Guid.NewGuid()).First());
        }

        public ContactBuilder WithMobileNumber(string mobileNumber)
        {
            Set(x => x.MobilePhoneNumber, mobileNumber);
            Set(x => x.NewMobilePhoneNumber, null);
            return this;
        }

        public ContactBuilder WithRandomMobileNumber()
        {
            var firstTwoDigits = DataHelper.RandomNumbersAsString(2);
            var secondDigits   = DataHelper.RandomNumbersAsString(3);
            var thirdDigits    = DataHelper.RandomNumbersAsString(3);

            return WithMobileNumber($"04{firstTwoDigits}{secondDigits}{thirdDigits}");
        }

        public ContactBuilder WithoutMobileNumber()
        {
            return WithMobileNumber(null);
        }

        public ContactBuilder WithHomePhoneNumber(string phoneNumber)
        {
            Set(x => x.HomePhoneNumber, phoneNumber);
            return this;
        }

        public ContactBuilder WithRandomAustralianHomePhoneNumber()
        {
            return WithHomePhoneNumber(GetRandomAustralianLandlineNumber());
        }

        public ContactBuilder WithRandomWesternAustralianHomePhoneNumber()
        {
            return WithHomePhoneNumber($"{PhonePrefix.WA_SA.GetDescription()}{GenerateRandomPhoneNumber()}");
        }

        public ContactBuilder WithoutHomePhoneNumber()
        {
            return WithHomePhoneNumber(null);
        }

        public ContactBuilder WithWorkPhoneNumber(string phoneNumber)
        {
            Set(x => x.WorkPhoneNumber, phoneNumber);
            return this;
        }

        public ContactBuilder WithRandomWesternAustralianWorkPhoneNumber()
        {
            return WithWorkPhoneNumber($"{PhonePrefix.WA_SA.GetDescription()}{GenerateRandomPhoneNumber()}");
        }
        public ContactBuilder WithPrivateEmail(Email email)
        {
            Set(x => x.PrivateEmail, email);
            return this;
        }

        public ContactBuilder WithPrivateEmailAddress(string email)
        {
            var privateEmail = GetOrDefault(x => x.PrivateEmail);
            if (privateEmail != null)
            {
                privateEmail.Address = email;
                return WithPrivateEmail(privateEmail);
            }

            return WithPrivateEmail(new Email
            {
                Address = email
            });
        }

        public ContactBuilder WithPrivateEmailAddressFromName()
        {
            return WithPrivateEmailAddress(DataHelper.RandomEmail(Get(x => x.FirstName), Get(x => x.Surname), domain: _config.Email?.Domain).Address);
        }

        public ContactBuilder WithoutPrivateEmailAddress()
        {
            return WithPrivateEmailAddress(null);
        }

        public ContactBuilder WithoutPrivateEmail()
        {
            Set(x => x.PrivateEmail, null);
            return this;
        }

        private ContactBuilder SetPrivateEmailPreferredDeliveryMethod(bool? isPreferredMethod)
        {
            var privateEmail = GetOrDefault(x => x.PrivateEmail);
            if (privateEmail != null)
            {
                privateEmail.IsPreferredDeliveryMethod = isPreferredMethod;
                return WithPrivateEmail(privateEmail);
            }

            return WithPrivateEmail(new Email
            {
                IsPreferredDeliveryMethod = isPreferredMethod
            });
        }

        public ContactBuilder WithPrivateEmailAsTheOnlyPreferredDeliveryMethod()
        {
            return SetAllPreferredDeliveryMethodAsFalse().SetPrivateEmailPreferredDeliveryMethod(true);
        }

        public ContactBuilder WithPrivateEmailAsPreferredDeliveryMethod(bool isPreferredMethod)
        {
            return SetPrivateEmailPreferredDeliveryMethod(isPreferredMethod);
        }

        public ContactBuilder WithMailingAddress(Address mailingAddress)
        {
            Set(x => x.MailingAddress, mailingAddress);
            return this;
        }

        public ContactBuilder WithRandomMailingAddress()
        {
            var mailingAddress = new AddressBuilder().InitialiseRandomMailingAddress().Build();
            return WithMailingAddress(mailingAddress);
        }

        public ContactBuilder WithoutMailingAddress()
        {
            return WithMailingAddress(null);
        }

        private ContactBuilder SetMailingAddressPreferredDeliveryMethod(bool isPreferredMethod)
        {
            var address = GetOrDefault(x => x.MailingAddress);
            if (address == null)
            {
                // Nothing to do.
                return this;
            }

            address.IsPreferredDeliveryMethod = isPreferredMethod;
            return WithMailingAddress(address);
        }

        public ContactBuilder WithMailingAddressAsTheOnlyPreferredDeliveryMethod()
        {
            SetAllPreferredDeliveryMethodAsFalse();
            SetMailingAddressPreferredDeliveryMethod(true);

            return this;
        }

        public ContactBuilder WithoutBankAccount()
        {
            Set(x => x.BankAccounts, null);
            return this;
        }

        public ContactBuilder WithBankAccount(BankAccount account)
        {
            var bankAccounts = GetOrDefault(x => x.BankAccounts, new List<BankAccount>()).ToList();
            bankAccounts.Add(account);
            Set(x => x.BankAccounts, bankAccounts);
            return this;
        }

        public ContactBuilder WithBankAccounts(List<BankAccount> accounts)
        {
            Set(x => x.BankAccounts, accounts);
            return this;
        }

        public ContactBuilder UpdateBankAccount(BankAccount accountToUpdate)
        {
            var bankAccounts = GetOrDefault(x => x.BankAccounts, new List<BankAccount>()).ToList();
            var account = bankAccounts.First(x => string.Equals(x.Id, accountToUpdate.Id));
            account.Bsb = accountToUpdate.Bsb;
            account.AccountName = accountToUpdate.AccountName;
            account.AccountNumber = accountToUpdate.AccountNumber;
            account.DiscontinueDate = accountToUpdate.DiscontinueDate;

            Set(x => x.BankAccounts, bankAccounts);
            return this;
        }

        public ContactBuilder WithRandomBankAccount()
        {
            return WithBankAccount(new BankAccount().InitWithRandomValues());
        }

        public ContactBuilder WithCreditCard(CreditCard card)
        {
            var cards = GetOrDefault(x => x.CreditCards, new List<CreditCard>()).ToList();
            cards.Add(card);
            Set(x => x.CreditCards, cards);
            return this;
        }

        public ContactBuilder WithCreditCards(List<CreditCard> cards)
        {
            Set(x => x.CreditCards, cards);
            return this;
        }

        public ContactBuilder WithRandomCreditCard()
        {
            return WithCreditCard(DataHelper.RandomCreditCard());
        }

        public ContactBuilder SetAllPreferredDeliveryMethodAsFalse()
        {
            if (Has(x => x.PrivateEmail))
            {
                SetPrivateEmailPreferredDeliveryMethod(false);
            }

            if (Has(x => x.MailingAddress))
            {
                SetMailingAddressPreferredDeliveryMethod(false);
            }

            return this;
        }

        public ContactBuilder WithMultiMatchStatus(bool multiMatchStatus)
        {
            Set(x => x.IsMultiMatchRSAMember, multiMatchStatus);
            return this;
        }

        /// <summary>
        /// Begins contact creation by pulling randomly from an internally
        /// coded list of known multi-match contacts. Allows some
        /// filtering on selection based on a age range, gender or
        /// whether they qualify for a membership discount.
        /// </summary>
        /// <param name="gender">nullable filter of gender. If NULL, then no filtering</param>
        /// <param name="minimumAge">Defaults to the minimum age allowed for a PH</param>
        /// <param name="maximumAge">Defaults to maximum age allowed for a PH</param>
        /// <param name="withDiscountTier">If true, then member must have a Bronze or higher tier in one of their records</param>
        /// <returns></returns>
        public ContactBuilder WithMultiMatchContact(Gender? gender = null,
                                                    int minimumAge = MIN_PH_AGE_HOME_PET,
                                                    int maximumAge = MAX_PH_AGE,
                                                    bool withDiscountTier = false)
        {
            var mmContact = GetMultiMatchContact(gender, minimumAge, maximumAge, withDiscountTier);

            return this.WithId(mmContact.Id)
                       .WithPersonId(mmContact.PersonId)
                       .WithFirstName(mmContact.FirstName)
                       .WithMiddleName(mmContact.MiddleName)
                       .WithSurname(mmContact.Surname)
                       .WithDateOfBirth(mmContact.DateOfBirth)
                       .WithMobileNumber(mmContact.MobilePhoneNumber)
                       .WithPrivateEmail(mmContact.PrivateEmail)
                       .WithGender(mmContact.Gender)
                       .WithRandomTitleFromGender()
                       .WithMultiMatchStatus(true);
        }

        public ContactBuilder InitialiseRandomIndividual()
        {
            var contactBuilder = WithRandomGender()
                .WithRandomTitleFromGender()
                .WithRandomFirstName()
                .WithRandomMiddleName()
                .WithRandomSurname()
                .WithInitialFromFirstName()
                .WithRandomDateOfBirth(18, 80)
                .WithRandomMobileNumber()
                .WithRandomWesternAustralianHomePhoneNumber()
                .WithRandomWesternAustralianWorkPhoneNumber()
                .WithPrivateEmailAddressFromName()
                .WithRandomBankAccount()
                .WithRandomCreditCard()
                .WithRandomMailingAddress()
                .WithPrivateEmailAsTheOnlyPreferredDeliveryMethod()
                .WithoutMembership()
                .WithoutDeclaringMembership(false)
                .WithMemberMatchRule(MemberMatchRule.None)
                .WithMultiMatchStatus(false);
            return contactBuilder;
        }


        protected override Contact BuildEntity()
        {
            var createdContact = new Contact
            {
                Id = GetOrDefault(x => x.Id),
                ExternalContactNumber = GetOrDefault(x => x.ExternalContactNumber),
                Title = GetOrDefault(x => x.Title),
                Initial = GetOrDefault(x => x.Initial),
                FirstName = GetOrDefault(x => x.FirstName),
                MiddleName = GetOrDefault(x => x.MiddleName),
                Surname = GetOrDefault(x => x.Surname),
                DateOfBirth = GetOrDefault(x => x.DateOfBirth),
                Gender = GetOrDefault(x => x.Gender),
                MembershipTier = GetOrDefault(x => x.MembershipTier),
                MembershipNumber = GetOrDefault(x => x.MembershipNumber),
                MobilePhoneNumber = GetOrDefault(x => x.MobilePhoneNumber),
                NewMobilePhoneNumber = GetOrDefault(x => x.NewMobilePhoneNumber),
                HomePhoneNumber = GetOrDefault(x => x.HomePhoneNumber),
                WorkPhoneNumber = GetOrDefault(x => x.WorkPhoneNumber),
                PrivateEmail = GetOrDefault(x => x.PrivateEmail),
                LoginEmail = GetOrDefault(x => x.LoginEmail),
                MailingAddress = GetOrDefault(x => x.MailingAddress),
                BankAccounts = GetOrDefault(x => x.BankAccounts),
                CreditCards = GetOrDefault(x => x.CreditCards),
                SkipDeclaringMembership = GetOrDefault(x => x.SkipDeclaringMembership),
                MemberMatchRule = GetOrDefault(x => x.MemberMatchRule),
                IsMultiMatchRSAMember = GetOrDefault(x => x.IsMultiMatchRSAMember),
                PersonId = GetOrDefault(x => x.PersonId),
            };

            return DataAlignment(createdContact);
        }

        private static string GetRandomAustralianLandlineNumber(bool? includeMobilePrefix = false)
        {
            var statePrefixes = new List<string> { PhonePrefix.NSW.GetDescription(),
                                                   PhonePrefix.VIC.GetDescription(),
                                                   PhonePrefix.QLD.GetDescription(),
                                                   PhonePrefix.WA_SA.GetDescription() };

            if (includeMobilePrefix.GetValueOrDefault())
            {
                statePrefixes.Add(PhonePrefix.Mobile.GetDescription());
            }

            return $"{statePrefixes.OrderBy(o => Guid.NewGuid()).First()}{GenerateRandomPhoneNumber()}";
        }

        private static string GenerateRandomPhoneNumber()
        {
            return DataHelper.RandomNumbersAsString(8);
        }

        /// <summary>
        /// Method intended to review created contact values and make last minute adjustments
        /// to remove conflicting values or invalid combinations
        /// </summary>
        /// <returns></returns>
        private Contact DataAlignment(Contact createdContact)
        {
            // NPE environments will only send to whitelisted domains.
            // If a contact is not relying on MC member match rule 2, then
            // check the email address and revise if necessary.
            if (createdContact.MemberMatchRule != MemberMatchRule.Rule2 &&
                createdContact.PrivateEmail != null &&
                !EmailHasWhitelistedDomain(createdContact.PrivateEmail.Address))
            {
                createdContact.PrivateEmail.Address = DataHelper.RandomEmail(firstName: createdContact.FirstName,
                                                                             surname: createdContact.Surname,
                                                                             domain: _config.Email?.Domain).Address;
            }

            // We should always have an email address for every contact
            if (createdContact.PrivateEmail == null)
                createdContact.PrivateEmail = DataHelper.RandomEmail(firstName: createdContact.FirstName,
                                                                     surname: createdContact.Surname,
                                                                     domain: _config.Email?.Domain);

            return createdContact;
        }

        private bool EmailHasWhitelistedDomain(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return email.EndsWith(NPE_EMAIL_DOMAIN_RACTEST) || email.EndsWith(NPE_EMAIL_DOMAIN_MAILOSAUR);
        }

        /// <summary>
        /// Returns the hardcoded list of contact search values
        /// for 409 (multi-match) contacts in Member Central.
        /// 
        /// This list of CRM IDs have been provided to the Member Central
        /// team to be explicitly maintained as multi-match. Some may still
        /// get touched by other staff during testing and no longer return
        /// 409, but they can be reset on request by reaching out to MC team.
        /// </summary>
        /// <returns></returns>
        public static List<Contact> ListOfMultiMatchContactOptions()
        {
            return new List<Contact>()
            {
                new Contact() { Id = "18588276", PersonId = "7de94ec4-6aaf-ed11-83ff-00224893bd7a", DateOfBirth = new DateTime(2006, 01, 17), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "19652617", PersonId = "d9edac60-1e8f-ee11-be36-00224812bb83", DateOfBirth = new DateTime(2006, 01, 16), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "19811451", PersonId = "2fb5c54f-ceaa-ee11-be37-000d3acbf676", DateOfBirth = new DateTime(2006, 11, 03), Gender = Gender.Male, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "18340628", PersonId = "74eeac1f-f1c6-ed11-b597-00224893d32a", DateOfBirth = new DateTime(2005, 12, 20), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "19206522", PersonId = "5b2dac11-e44a-ee11-be6f-00224893bd7a", DateOfBirth = new DateTime(2006, 02, 09), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "19703392", PersonId = "d8a820ec-df97-ee11-be37-000d3acbf676", DateOfBirth = new DateTime(2007, 01, 09), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "20468284", PersonId = "8422dd9d-a77a-e911-a972-000d3ad24a0d", DateOfBirth = new DateTime(2007, 02, 01), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "20426913", PersonId = "0b914d65-b606-ef11-9f89-6045bdc38fe4", DateOfBirth = new DateTime(2006, 12, 15), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "19802835", PersonId = "db12b7c3-d5a9-ee11-be37-000d3acbf676", DateOfBirth = new DateTime(2007, 04, 21), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17700448", PersonId = "b3a741eb-0c59-ed11-9562-00224893b422", DateOfBirth = new DateTime(2006, 03, 17), Gender = Gender.Male, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "19175574", PersonId = "c4de9e58-1846-ee11-be6e-002248933b96", DateOfBirth = new DateTime(2005, 12, 07), Gender = Gender.Male, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "18755725", PersonId = "df582834-b606-ee11-8f6e-000d3acac3fc", DateOfBirth = new DateTime(2006, 02, 17), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "19450305", PersonId = "80bf97a9-cd70-ee11-8179-000d3acb3862", DateOfBirth = new DateTime(2006, 05, 03), Gender = Gender.Female, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "18942653", PersonId = "5911d6d5-ec22-ee11-9966-00224893bd7a", DateOfBirth = new DateTime(2005, 12, 05), Gender = Gender.Female, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "18801773", PersonId = "e07e251f-ce0d-ee11-8f6e-000d3acbf676", DateOfBirth = new DateTime(2006, 02, 20), Gender = Gender.Female, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "20107056", PersonId = "d2dbe0f6-d7d5-ee11-904c-6045bde4c518", DateOfBirth = new DateTime(2006, 06, 11), Gender = Gender.Female, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "20325143", PersonId = "d050755f-dff6-ee11-a1fe-000d3acb10d2", DateOfBirth = new DateTime(2006, 09, 22), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17892298", PersonId = "5fd6fe11-ee6d-83bd-133c-b0edf6768d9c", DateOfBirth = new DateTime(2006, 11, 09), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "17943952", PersonId = "a6577b53-eccc-eb11-bacc-000d3a6aec49", DateOfBirth = new DateTime(2005, 12, 28), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "19701656", PersonId = "b90fcc5f-d99a-96f7-8c45-a7b2ee268186", DateOfBirth = new DateTime(2006, 12, 01), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "20042645", PersonId = "93b97aaa-5acc-ee11-9079-00224893b958", DateOfBirth = new DateTime(2006, 10, 29), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "19384457", PersonId = "85eb5863-6e66-ee11-9ae7-00224893bb52", DateOfBirth = new DateTime(2006, 06, 30), Gender = Gender.Female, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "18960895", PersonId = "7b50ac22-d525-ee11-9965-00224893b958", DateOfBirth = new DateTime(2004, 07, 22), Gender = Gender.Male, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "14731776", PersonId = "2e9aade9-3a4c-eb11-a812-000d3ad1c2b4", DateOfBirth = new DateTime(2003, 11, 20), Gender = Gender.Male, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "19539723", PersonId = "59895a09-347e-ee11-8179-00224893bd7a", DateOfBirth = new DateTime(2004, 12, 17), Gender = Gender.Male, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "17927109", PersonId = "5eb9c336-aa82-ed11-81ad-000d3acb2824", DateOfBirth = new DateTime(2004, 09, 10), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "16658714", PersonId = "dc22165f-b59d-ec11-b400-00224812d1d1", DateOfBirth = new DateTime(2005, 01, 28), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17269850", PersonId = "d6b3d773-2907-ed11-82e5-002248944f9f", DateOfBirth = new DateTime(2005, 07, 24), Gender = Gender.Male, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "16876310", PersonId = "9904a167-d5e5-eb11-bacb-0022481012fd", DateOfBirth = new DateTime(2005, 04, 21), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "16613684", PersonId = "3067ae2b-1396-ec11-b400-00224811d8b2", DateOfBirth = new DateTime(2004, 08, 14), Gender = Gender.Male, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "16529418", PersonId = "f9b37f5f-6989-ec11-8d20-000d3ad23ffa", DateOfBirth = new DateTime(2004, 12, 06), Gender = Gender.Male, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "14861865", PersonId = "56b8e590-b662-eb11-a812-000d3a6a65ee", DateOfBirth = new DateTime(2003, 12, 28), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "19987256", PersonId = "2fbe9459-e7c3-ee11-9079-00224893b958", DateOfBirth = new DateTime(2004, 09, 23), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17663598", PersonId = "de9e6ae2-a851-ed11-9562-000d3acac3fc", DateOfBirth = new DateTime(2005, 06, 16), Gender = Gender.Female, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "15128683", PersonId = "5e551322-bb0b-ec11-b6e6-00224810b25c", DateOfBirth = new DateTime(2004, 03, 30), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "17052993", PersonId = "d28b72df-f2e2-ec11-bb3d-002248944ee0", DateOfBirth = new DateTime(2005, 01, 29), Gender = Gender.Female, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "19732401", PersonId = "fdbbbc3a-9757-ee11-be6f-00224893bb52", DateOfBirth = new DateTime(2003, 12, 07), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "17722014", PersonId = "3eec8838-ef19-ed11-b83e-00224812b000", DateOfBirth = new DateTime(2005, 01, 23), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "19809458", PersonId = "b8b81649-abaa-ee11-be37-00224893b958", DateOfBirth = new DateTime(2005, 04, 26), Gender = Gender.Female, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "15854607", PersonId = "7b2bb3e9-ce00-d02c-37ad-d439a4f3434d", DateOfBirth = new DateTime(2004, 07, 23), Gender = Gender.Female, MembershipTier = MembershipTier.Free2Go },
                new Contact() { Id = "15591424", PersonId = "78c63aae-21e5-eb11-bacb-0022481012fd", DateOfBirth = new DateTime(2004, 04, 26), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "17774895", PersonId = "7b3b98c5-3b66-ed11-9561-000d3ad0b1e9", DateOfBirth = new DateTime(2004, 06, 03), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "16390151", PersonId = "2e723863-d101-e911-a968-000d3ad24077", DateOfBirth = new DateTime(2004, 11, 29), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "15192621", PersonId = "4bff796c-589e-eb11-b1ac-000d3a6ae80e", DateOfBirth = new DateTime(2004, 03, 19), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "9914089", PersonId = "6dd200ab-cac8-ed77-76aa-237af9f3dd33", DateOfBirth = new DateTime(2000, 12, 27), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "14949541", PersonId = "e40a99ab-5e72-eb11-a812-000d3acba220", DateOfBirth = new DateTime(2000, 12, 12), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "13284778", PersonId = "280fa931-0644-ea11-a812-000d3ad1cd5b", DateOfBirth = new DateTime(2001, 07, 23), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "13401394", PersonId = "e9156c86-7158-ea11-a811-000d3ad1cd5b", DateOfBirth = new DateTime(2000, 11, 26), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "17187017", PersonId = "09e4ce50-28fc-ec11-82e6-002248944a4e", DateOfBirth = new DateTime(2001, 04, 16), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "13340864", PersonId = "3e5890cc-594e-ea11-a812-000d3ad1caaa", DateOfBirth = new DateTime(2002, 10, 03), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "18116093", PersonId = "fe68dde6-c5f4-7604-64e2-ddb217712775", DateOfBirth = new DateTime(2002, 10, 07), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "19069112", PersonId = "6a9f07f4-e235-ee11-bdf4-00224811425f", DateOfBirth = new DateTime(2002, 03, 01), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17861770", PersonId = "b1f23ba3-3976-ed11-81ab-00224892bbf7", DateOfBirth = new DateTime(2001, 03, 06), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "17239673", PersonId = "6bb1478d-5806-ed11-82e5-002248944297", DateOfBirth = new DateTime(2001, 07, 20), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "15685225", PersonId = "7ca4c987-c9f4-eb11-94ef-000d3acb8913", DateOfBirth = new DateTime(2000, 11, 29), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "16049751", PersonId = "b7b24642-a633-ec11-b6e6-00224810ee1a", DateOfBirth = new DateTime(2003, 01, 09), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "15342613", PersonId = "a3015f5a-8aa8-e911-a97b-000d3ad24282", DateOfBirth = new DateTime(2003, 02, 27), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "14358161", PersonId = "41cd7248-6001-eb11-a813-000d3ad1c2b4", DateOfBirth = new DateTime(2003, 07, 15), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "15152136", PersonId = "2201cb2c-b196-eb11-b1ac-000d3acc230b", DateOfBirth = new DateTime(2002, 02, 25), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "14489021", PersonId = "7043d2b7-a119-eb11-a813-000d3ad1caaa", DateOfBirth = new DateTime(2002, 06, 19), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "14114053", PersonId = "efa1a139-e6d5-ea11-a813-000d3acb05bc", DateOfBirth = new DateTime(2002, 09, 23), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "13093674", PersonId = "ea98e8c4-9720-ea11-a810-000d3ad1c2b4", DateOfBirth = new DateTime(2002, 03, 26), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "17923519", PersonId = "5ac2f7b2-e281-ed11-81ad-00224893bd39", DateOfBirth = new DateTime(2002, 04, 04), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "15675073", PersonId = "1b0052a5-6bf3-eb11-94ef-000d3acbb602", DateOfBirth = new DateTime(2002, 10, 14), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "19227047", PersonId = "844b489f-424d-ee11-be6f-000d3a6b66ed", DateOfBirth = new DateTime(2001, 10, 28), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "20022349", PersonId = "7d42277c-57c9-ee11-9079-00224893bfba", DateOfBirth = new DateTime(1999, 08, 24), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "18604772", PersonId = "5913c3d8-a8ef-ed11-8849-00224810ab99", DateOfBirth = new DateTime(1999, 04, 10), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "16857108", PersonId = "71dc6465-9dc1-ec11-983e-000d3ad1188c", DateOfBirth = new DateTime(2000, 03, 14), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "15044948", PersonId = "e2aa8ac1-ce0f-e911-a968-000d3ad244fd", DateOfBirth = new DateTime(1999, 03, 11), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "8217092", PersonId = "af8557b6-590c-c279-717a-43250dc3d6fa", DateOfBirth = new DateTime(1999, 06, 30), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "19373721", PersonId = "8af9b70d-3b64-ee11-8df0-00224892bbf7", DateOfBirth = new DateTime(2000, 03, 10), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "19178409", PersonId = "23375efe-5e46-ee11-be6e-00224893bb52", DateOfBirth = new DateTime(1999, 04, 02), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "14534259", PersonId = "09ffc1db-e126-eb11-a813-000d3acb05bc", DateOfBirth = new DateTime(1999, 12, 16), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "17048670", PersonId = "9fdcc29e-67df-9a6b-0965-6c3cd58c8919", DateOfBirth = new DateTime(1999, 02, 17), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "16583786", PersonId = "17dd7d34-a792-ec11-b400-00224811d07d", DateOfBirth = new DateTime(2000, 06, 22), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17960081", PersonId = "e91865aa-788a-ed11-81ad-000d3acc0e54", DateOfBirth = new DateTime(2000, 07, 14), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "19961655", PersonId = "73b1d09d-d83e-c31f-1ca4-ba6745d86ec0", DateOfBirth = new DateTime(1998, 12, 19), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "15773713", PersonId = "d292df42-ac04-ec11-b6e6-00224810b59f", DateOfBirth = new DateTime(1999, 09, 08), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "14087211", PersonId = "474c16c7-cfd0-ea11-a813-000d3ad1ce3a", DateOfBirth = new DateTime(1999, 02, 08), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "16780347", PersonId = "eba4b1da-24b0-ec11-983f-0022489363ae", DateOfBirth = new DateTime(2000, 07, 08), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "20073059", PersonId = "643285f1-78d0-ee11-9079-00224892bbf7", DateOfBirth = new DateTime(1999, 11, 06), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "14843126", PersonId = "ceba22b4-ed94-e788-203d-e213ea3125bd", DateOfBirth = new DateTime(2000, 04, 12), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "17134962", PersonId = "cfa5eba7-e3f2-ec11-bb3d-002248944efe", DateOfBirth = new DateTime(1999, 12, 28), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17969162", PersonId = "f07f0d6b-0153-f0c3-fb81-cc8f755218dc", DateOfBirth = new DateTime(1999, 10, 22), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "16829568", PersonId = "be6aae37-c7b8-ec11-983f-0022489417a5", DateOfBirth = new DateTime(1999, 01, 05), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "14930469", PersonId = "9bfd70e4-646f-eb11-a812-000d3acba220", DateOfBirth = new DateTime(2000, 05, 13), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "15415022", PersonId = "204221ff-31c7-eb11-bacc-000d3a6b5662", DateOfBirth = new DateTime(2000, 01, 25), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "14863266", PersonId = "f25c7ff1-0663-eb11-a812-000d3ad296a5", DateOfBirth = new DateTime(1992, 01, 28), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "15339776", PersonId = "e6b54745-2eba-eb11-8236-000d3a6a6f07", DateOfBirth = new DateTime(1991, 07, 07), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "13922742", PersonId = "eda7bec6-97b6-ea11-a812-000d3ad1ce3a", DateOfBirth = new DateTime(1995, 12, 11), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "19546063", PersonId = "3519933a-287f-ee11-8179-002248933b96", DateOfBirth = new DateTime(1997, 01, 11), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "16086383", PersonId = "75d22c24-063b-ec11-8c62-00224811092e", DateOfBirth = new DateTime(1998, 05, 11), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "15348251", PersonId = "f2e0ae67-48bc-eb11-bacc-000d3a6ac2c3", DateOfBirth = new DateTime(1990, 08, 31), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "19577218", PersonId = "6e195858-ba83-ee11-8179-000d3acbf676", DateOfBirth = new DateTime(1991, 05, 04), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "15690849", PersonId = "3f15b8c0-b5f5-eb11-94ef-000d3acb8913", DateOfBirth = new DateTime(1995, 05, 17), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "13343235", PersonId = "926e7dda-ea4e-ea11-a812-000d3ad1caaa", DateOfBirth = new DateTime(1992, 08, 21), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "20260016", PersonId = "31a0fc86-49ec-ee11-a1fd-00224892556d", DateOfBirth = new DateTime(1998, 04, 06), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "18756033", PersonId = "d479912d-c406-ee11-8f6e-000d3acbf676", DateOfBirth = new DateTime(1998, 01, 01), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "18236466", PersonId = "299cb37b-edb4-ed11-b597-000d3acac3fc", DateOfBirth = new DateTime(1992, 12, 01), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "16731446", PersonId = "3a1430cc-fba8-ec11-9840-002248936914", DateOfBirth = new DateTime(1991, 10, 12), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "18443268", PersonId = "542f64c3-52d7-ed11-a7c7-00224811b992", DateOfBirth = new DateTime(1996, 04, 10), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17742555", PersonId = "60d5918c-aa99-eb11-b1ac-000d3acc2bfb", DateOfBirth = new DateTime(1994, 12, 07), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "18462331", PersonId = "034137ad-ffd9-ed11-a7c7-00224893bd7a", DateOfBirth = new DateTime(1998, 03, 16), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "15573599", PersonId = "bb9eb344-25e2-eb11-bacb-00224810188f", DateOfBirth = new DateTime(1995, 08, 26), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "15578868", PersonId = "9f059be5-f8e2-eb11-bacb-00224810188f", DateOfBirth = new DateTime(1994, 02, 25), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "7674817", PersonId = "ef9ce7f6-8a75-c5b1-27c3-fbe1979c950c", DateOfBirth = new DateTime(1993, 04, 30), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "18096546", PersonId = "32a628c0-9da0-ed11-aad1-000d3acc0e54", DateOfBirth = new DateTime(1996, 10, 20), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "15835208", PersonId = "c48d62df-200f-ec11-b6e6-00224810db66", DateOfBirth = new DateTime(1992, 05, 31), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "15716478", PersonId = "f6b23da6-89fa-eb11-94ef-00224810df2e", DateOfBirth = new DateTime(1993, 06, 02), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "16079018", PersonId = "ac2d266e-5639-ec11-8c64-000d3a6a1381", DateOfBirth = new DateTime(1985, 05, 02), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "2276198", PersonId = "86d01afb-9bea-98b4-7466-b3aed0016a83", DateOfBirth = new DateTime(1989, 02, 01), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "8682395", PersonId = "7fc0b234-de8b-ec11-8d20-000d3ad23da4", DateOfBirth = new DateTime(1987, 01, 08), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "11113086", PersonId = "6cafc6d7-261e-7eba-0713-28ab0658c2d3", DateOfBirth = new DateTime(1985, 02, 13), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "16890157", PersonId = "7574ceac-d7c5-ec11-a7b5-000d3ad14768", DateOfBirth = new DateTime(1988, 01, 29), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "18461114", PersonId = "46bc83d9-c5c0-ed11-83fe-00224893b422", DateOfBirth = new DateTime(1987, 09, 10), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "17912833", PersonId = "3512c5b4-1980-ed11-81ac-000d3a6b66ed", DateOfBirth = new DateTime(1984, 06, 05), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "7129996", PersonId = "9f8d98ad-b6ab-dbfe-5341-016af926065b", DateOfBirth = new DateTime(1984, 07, 27), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "15713093", PersonId = "5b299578-e5f9-eb11-94ef-00224810d3fe", DateOfBirth = new DateTime(1988, 03, 15), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "13682565", PersonId = "e99aace2-c58d-ea11-a811-000d3acb05bc", DateOfBirth = new DateTime(1985, 07, 28), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "13742332", PersonId = "687a645a-d498-ea11-a812-000d3ad1ce3a", DateOfBirth = new DateTime(1987, 10, 11), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "14679865", PersonId = "42f2627a-d23e-eb11-a813-000d3ad1ce3a", DateOfBirth = new DateTime(1983, 09, 22), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "17519952", PersonId = "5825a045-e838-ed11-9db0-00224893b5c4", DateOfBirth = new DateTime(1984, 01, 16), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "14370268", PersonId = "111e3416-c203-eb11-a813-000d3ad1caaa", DateOfBirth = new DateTime(1985, 12, 07), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "17522441", PersonId = "72ffb196-6b39-ed11-9db0-00224893b5c4", DateOfBirth = new DateTime(1987, 06, 28), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "18644484", PersonId = "17c48f94-c78a-b307-4f3a-94d0cda3d250", DateOfBirth = new DateTime(1986, 08, 13), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17052175", PersonId = "78ecea86-e2e2-ec11-bb3c-002248944a7b", DateOfBirth = new DateTime(1986, 07, 27), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "8448060", PersonId = "fac451cd-f380-87fa-1336-9a9822556b4a", DateOfBirth = new DateTime(1988, 11, 21), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "16965858", PersonId = "237568eb-afd2-ec11-a7b5-002248944193", DateOfBirth = new DateTime(1985, 06, 19), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "18781935", PersonId = "d0767d74-c20a-ee11-8f6e-000d3a6b66ed", DateOfBirth = new DateTime(1984, 02, 01), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17972872", PersonId = "9797a2d5-968c-ed11-81ad-000d3acc0e54", DateOfBirth = new DateTime(1984, 07, 11), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17457049", PersonId = "d982550d-b6b3-2847-4a35-fc2b726123f4", DateOfBirth = new DateTime(1986, 07, 23), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "14696654", PersonId = "992e6b48-3043-eb11-a812-000d3acae853", DateOfBirth = new DateTime(1976, 01, 05), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "16218644", PersonId = "796144fa-8b52-ec11-8f8e-00224811c192", DateOfBirth = new DateTime(1981, 06, 03), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "16469477", PersonId = "94023941-607f-ec11-8d20-00224891d929", DateOfBirth = new DateTime(1981, 03, 18), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "19512920", PersonId = "53f81f31-0d7a-ee11-8179-00224893d32a", DateOfBirth = new DateTime(1975, 11, 24), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "15656588", PersonId = "2de036f8-33f0-eb11-94ef-000d3acc2b61", DateOfBirth = new DateTime(1981, 11, 04), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "19909347", PersonId = "c08fcd9b-cdb8-ee11-9078-00224893b065", DateOfBirth = new DateTime(1976, 03, 27), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "15451275", PersonId = "02062e8a-a9cd-eb11-bacc-000d3a6aa211", DateOfBirth = new DateTime(1979, 04, 23), Gender = Gender.Male, MembershipTier = MembershipTier.None },
                new Contact() { Id = "19601901", PersonId = "e04d0979-7487-ee11-be36-00224893bd7a", DateOfBirth = new DateTime(1982, 06, 10), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "883145", PersonId = "dc02c779-5964-ea11-a811-000d3ad1c2b4", DateOfBirth = new DateTime(1979, 08, 26), Gender = Gender.Male, MembershipTier = MembershipTier.Silver },
                new Contact() { Id = "18745488", PersonId = "bd62a8ed-3605-ee11-8f6e-00224893b065", DateOfBirth = new DateTime(1978, 09, 19), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "13667313", PersonId = "85f83be4-0e12-05ef-2250-d5fb06ab2e9f", DateOfBirth = new DateTime(1975, 05, 30), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "10558970", PersonId = "b219b5c1-7d17-caec-d395-a17d2b287f99", DateOfBirth = new DateTime(1974, 12, 22), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "15338535", PersonId = "0a0607a0-fab9-eb11-8236-000d3a6a6f07", DateOfBirth = new DateTime(1981, 12, 15), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "14910025", PersonId = "52774c62-616b-eb11-a812-000d3acb9f9b", DateOfBirth = new DateTime(1975, 09, 21), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17389624", PersonId = "46dcd4c9-2921-ed11-9db1-00224812bc4e", DateOfBirth = new DateTime(1981, 12, 28), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "10857780", PersonId = "e269b35a-19b7-e811-a962-000d3ad24a0d", DateOfBirth = new DateTime(1978, 10, 29), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "13713068", PersonId = "cd5eff11-6793-ea11-a812-000d3acae853", DateOfBirth = new DateTime(1976, 03, 02), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "7472153", PersonId = "670e704a-c6e6-40df-9e92-e9e630e5a931", DateOfBirth = new DateTime(1977, 11, 29), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "17993151", PersonId = "69186b5d-5f71-2dff-b2c2-87f8c86bce89", DateOfBirth = new DateTime(1981, 05, 29), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "18277853", PersonId = "cba65d2f-f5bb-ed11-83ff-00224893362a", DateOfBirth = new DateTime(1977, 12, 02), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "1536840", PersonId = "463dd7cd-dd01-ec52-6d63-14fbab82d1a8", DateOfBirth = new DateTime(1978, 09, 16), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "17140829", PersonId = "5f6732dd-edf5-ec11-82e7-002248944f2b", DateOfBirth = new DateTime(1980, 10, 16), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "11458209", PersonId = "4ccf779f-50e7-ea11-a817-000d3ad1cf4f", DateOfBirth = new DateTime(1974, 01, 05), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "14190952", PersonId = "b6fd1e4d-57e3-ea11-a813-000d3ad1cf4f", DateOfBirth = new DateTime(1962, 10, 14), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "15946647", PersonId = "5613df1f-7f22-ec11-b6e6-00224810efdc", DateOfBirth = new DateTime(1953, 01, 01), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "16574214", PersonId = "4decde3f-7f90-ec11-8d20-002248923e0c", DateOfBirth = new DateTime(1968, 09, 10), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "15041295", PersonId = "e53d1340-61dd-ea11-a813-000d3acb05bc", DateOfBirth = new DateTime(1962, 04, 04), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "11481154", PersonId = "4cb071e2-6615-e911-a966-000d3ad24c60", DateOfBirth = new DateTime(1972, 05, 31), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "14427054", PersonId = "3c627386-64fd-ea11-a813-000d3acbb312", DateOfBirth = new DateTime(1967, 01, 26), Gender = Gender.Male, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "18946580", PersonId = "6f44a0fa-8d23-ee11-9965-00224893bd39", DateOfBirth = new DateTime(1962, 10, 17), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "9796730", PersonId = "c50d6640-8d49-ea11-a812-000d3ad1c2b4", DateOfBirth = new DateTime(1964, 03, 05), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "19435004", PersonId = "2b897515-2e6e-ee11-9ae7-00224893bd7a", DateOfBirth = new DateTime(1967, 11, 19), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "13589102", PersonId = "b6b33f71-c278-ea11-a811-000d3ad1cd5b", DateOfBirth = new DateTime(1958, 09, 22), Gender = Gender.Male, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "13834721", PersonId = "2bc2559e-11a9-ea11-a812-000d3ad1cd5b", DateOfBirth = new DateTime(1959, 12, 12), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "15100349", PersonId = "6d4f9ce3-1192-eb11-b1ac-000d3acbd05d", DateOfBirth = new DateTime(1970, 04, 28), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "18902238", PersonId = "cfcd321e-761c-ee11-8f6d-00224893362a", DateOfBirth = new DateTime(1974, 05, 30), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "16400179", PersonId = "c55f10b0-d4c5-132c-1422-8fce5cdbba3c", DateOfBirth = new DateTime(1966, 10, 05), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "13022455", PersonId = "ca508fe6-8c13-ea11-a811-000d3ad1ce3a", DateOfBirth = new DateTime(1953, 08, 08), Gender = Gender.Female, MembershipTier = MembershipTier.None },
                new Contact() { Id = "17578887", PersonId = "f5b7084a-cf42-ed11-bba3-00224893bd31", DateOfBirth = new DateTime(1973, 07, 21), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "738495", PersonId = "6951c5f8-7f36-ed11-9db1-00224893bb52", DateOfBirth = new DateTime(1962, 12, 08), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "3230749", PersonId = "2f9ddc2e-6e08-e1bc-49fb-937fe4fa9926", DateOfBirth = new DateTime(1967, 05, 03), Gender = Gender.Female, MembershipTier = MembershipTier.Silver },
                new Contact() { Id = "1693870", PersonId = "b85c2b8a-f322-d22a-29af-f74f1f079b8a", DateOfBirth = new DateTime(1953, 08, 18), Gender = Gender.Female, MembershipTier = MembershipTier.Red },
                new Contact() { Id = "19170395", PersonId = "a75f9500-6145-ee11-be6e-002248933d22", DateOfBirth = new DateTime(1959, 09, 18), Gender = Gender.Female, MembershipTier = MembershipTier.Blue },
                new Contact() { Id = "15940546", PersonId = "12aa58da-9021-ec11-b6e6-00224810eda9", DateOfBirth = new DateTime(1950, 10, 26), Gender = Gender.Female, MembershipTier = MembershipTier.Blue } 
            };
        }

        /// <summary>
        /// Returns a Multi-Match contact from a static list
        /// A static list is used due to the complexity of getting a Multi-Match contact dynamically
        /// </summary>
        /// <returns></returns>
        // TODO: Add Tear-Down operations to consider "cleaning up" the added duplicate contacts that result.
        private static Contact GetMultiMatchContact(Gender? gender = null,
                                                    int minimumAge = MIN_PH_AGE_HOME_PET,
                                                    int maximumAge = MAX_PH_AGE,
                                                    bool withDiscountTier = false)
        {
            Contact candidate = null;
            var multiMatchContact = ListOfMultiMatchContactOptions();
            var haveFoundSuitableContact = false;
            var retryCounter = 3;

            do
            {

                candidate = multiMatchContact.Where(x => x.GetContactAge() >= minimumAge)
                                             .Where(x => x.GetContactAge() <= maximumAge)
                                             .Where(x => (!gender.HasValue || (x.Gender == gender.Value)))
                                             .Where(x => !withDiscountTier || (x.MembershipTier == MembershipTier.Bronze ||
                                                                               x.MembershipTier == MembershipTier.Silver ||
                                                                               x.MembershipTier == MembershipTier.Gold))
                                             .PickRandom();

                if (Config.Get().IsMCMockEnabled())
                {
                    Reporting.IsTrue(MemberCentral.IsMCMockAlive(), "status of MC Mock isAlive = True");

                    // We update "candidate" with full contact info
                    candidate = MemberCentral.PopulateMockMemberCentralWithLatestContactIdInfo(candidate.Id);
                    MemberCentral.SetMultiMatchResponseByShieldContactId(candidate.Id, (int)HttpStatusCode.Conflict);
                    Reporting.Log($"Member Central Mock has been populated with Shield Contact Id {candidate.Id} and is simulating Multi-match (HTTP 409: Conflict) response for that member.");

                    haveFoundSuitableContact = true;
                }
                else
                {
                    var mcApi = MemberCentral.GetInstance();
                    candidate = Contact.InitFromMCByShieldId(candidate.Id);
                    if (!mcApi.IsMultiMatch(candidate))
                    {
                        Reporting.Log($"---> Info: Multi-match contact with CRM ID: {candidate.PersonId} did not return as a multi-match.");
                    }
                    else
                    {
                        haveFoundSuitableContact = true;
                    }
                }

                retryCounter--;
            } while (!haveFoundSuitableContact && retryCounter > 0);

            if (!haveFoundSuitableContact)
            {
                Reporting.Error("Have attempted multiple times to find a multi-match contact but failed. Review logs and get in touch with MC team to rectify affected contacts.");
            }

            return candidate;
        }
    }
}