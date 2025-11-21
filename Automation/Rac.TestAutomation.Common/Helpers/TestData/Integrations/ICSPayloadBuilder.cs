using Rac.TestAutomation.Common.API;
using System;
using System.Collections.Generic;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.Contacts;

namespace Rac.TestAutomation.Common
{
    public class ICSPayloadBuilder : EntityBuilder<ICSContactPayload, ICSPayloadBuilder>
    {
        private Config _config;
        private static string _AnonymousPrefix = "_B2C_";

        public ICSPayloadBuilder()
        {
            _config = Config.Get();
        }

        public ICSPayloadBuilder InitialiseWithRandomInidividual()
        {
            var icsBuilder = new ICSPayloadBuilder()
                .WithRandomDateOfBirth(18, 80)
                .WithRandomGender()
                .WithRandomTitleFromGender()
                .WithRandomFirstName()
                .WithRandomMiddleName()
                .WithRandomSurname()
                .WithRandomMobileNumber()
                .WithEmailAddressFromName()
                .WithRandomMailingAddress()
                .WithRandomBankAccount()
                .WithRandomCreditCard();

            return icsBuilder;
        }

        public ICSPayloadBuilder InitialiseWithMinimalFields()
        {
            var icsBuilder = new ICSPayloadBuilder()
                .WithRandomDateOfBirth(18, 80)
                .WithRandomFirstName()
                .WithRandomSurname()
                .WithMinimumMailingAddress();

            return icsBuilder;
        }

        public ICSPayloadBuilder WithMembershipTier(MembershipTier tier)
        {
            var membership = GetOrDefault(x => x.Membership) ?? new ContactServiceMembership();

            membership.Tier = tier.GetDescription();
            Set(x => x.Membership, membership);
            return this;
        }

        public ICSPayloadBuilder WithMembershipNumber(string number)
        {
            var membership = GetOrDefault(x => x.Membership) ?? new ContactServiceMembership();

            membership.Number = number;
            Set(x => x.Membership, membership);
            return this;
        }

        public ICSPayloadBuilder WithMembership(MembershipTier tier, string number)
        {
            return WithMembershipTier(tier)
                .WithMembershipNumber(number);
        }

        public ICSPayloadBuilder WithoutMembershipTier()
        {
            Set(x => x.Membership, null);
            return this;
        }

        public ICSPayloadBuilder WithRandomMembershipTier()
        {
            return WithMembershipTier((MembershipTier)Randomiser.Get.Next((Enum.GetValues(typeof(MembershipTier))).Length));
        }

        public ICSPayloadBuilder WithoutMembershipNumber()
        {
            return WithMembershipNumber(null);
        }

        public ICSPayloadBuilder WithoutMembership()
        {
            return WithoutMembershipTier()
                .WithoutMembershipNumber();
        }

        public ICSPayloadBuilder WithTitle(Title title)
        {
            Set(x => x.Title, title.GetDescription());
            return this;
        }

        public ICSPayloadBuilder WithTitle(string title)
        {
            return WithTitle(DataHelper.GetValueFromDescription<Title>(title));
        }

        public ICSPayloadBuilder WithRandomTitleFromGender()
        {
            var genderEnum = DataHelper.GetValueFromDescription<Gender>(Get(x => x.Gender));
            return WithTitle(DataHelper.GetRandomTitleForGender(genderEnum));
        }

        public ICSPayloadBuilder WithFirstName(string firstName)
        {
            Set(x => x.FirstName, firstName);
            return this;
        }

        public ICSPayloadBuilder WithRandomFirstName()
        {
            return WithFirstName(DataHelper.RandomShieldWhiteListedCharacters(4, 45).FirstCharToUpper());
        }

        public ICSPayloadBuilder WithMiddleName(string middleName)
        {
            Set(x => x.MiddleName, middleName);
            return this;
        }

        public ICSPayloadBuilder WithRandomMiddleName()
        {
            return WithMiddleName(DataHelper.RandomShieldWhiteListedCharacters(4, 45).FirstCharToUpper());
        }

        public ICSPayloadBuilder WithoutMiddleName()
        {
            Set(x => x.MiddleName, null);
            return this;
        }

        public ICSPayloadBuilder WithSurname(string surname)
        {
            Set(x => x.Surname, surname);
            return this;
        }

        public ICSPayloadBuilder WithoutSurname()
        {
            return WithSurname(null);
        }

        public ICSPayloadBuilder WithRandomSurname()
        {
            return WithSurname(DataHelper.RandomShieldWhiteListedCharacters(4, 50).FirstCharToUpper());
        }

        public ICSPayloadBuilder WithAnonymousContactPrefixes()
        {
            var firstName = GetOrDefault(x => x.FirstName);
            if (string.IsNullOrEmpty(firstName)) { WithRandomFirstName(); }

            if (!firstName.StartsWith(_AnonymousPrefix))
            { 
                firstName = $"{_AnonymousPrefix}{firstName}";
            }

            var surname = GetOrDefault(x => x.Surname) ?? DataHelper.RandomShieldWhiteListedCharacters(4, 50).FirstCharToUpper();
            if (!surname.StartsWith(_AnonymousPrefix))
            {
                surname = $"{_AnonymousPrefix}{surname}";
            }

            WithFirstName(firstName);
            WithSurname(surname);
            return this;    
        }

        public ICSPayloadBuilder WithDateOfBirth(DateTime dateOfBirth)
        {
            Set(x => x.DateOfBirth, dateOfBirth.ToString(DataFormats.DATE_FORMAT_REVERSE_HYPHENS));
            return this;
        }

        public ICSPayloadBuilder WithRandomDateOfBirth(int age)
        {
            return WithDateOfBirth(DataHelper.RandomDoB(age));
        }

        public ICSPayloadBuilder WithRandomDateOfBirth(int minAge, int maxAge)
        {
            return WithDateOfBirth(DataHelper.RandomDoB(minAge, maxAge));
        }

        /// <summary>
        /// Set gender
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public ICSPayloadBuilder WithGender(Gender gender)
        {
            Set(x => x.Gender, gender.GetDescription());
            return this;
        }

        public ICSPayloadBuilder AsMale()
        {
            return WithGender(Gender.Male);
        }

        public ICSPayloadBuilder AsFemale()
        {
            return WithGender(Gender.Female);
        }

        public ICSPayloadBuilder WithRandomGender()
        {
            var genderOptions = new[]
            {
                Gender.Male,
                Gender.Female
            };

            return WithGender(genderOptions.OrderBy(t => Guid.NewGuid()).First());
        }

        public ICSPayloadBuilder WithPhoneNumber(string phoneNumber)
        {
            Set(x => x.PhoneNumber, phoneNumber);
            return this;
        }

        public ICSPayloadBuilder WithRandomMobileNumber()
        {
            var firstTwoDigits = DataHelper.RandomNumbersAsString(2);
            var secondDigits = DataHelper.RandomNumbersAsString(3);
            var thirdDigits = DataHelper.RandomNumbersAsString(3);

            return WithPhoneNumber($"04{firstTwoDigits}{secondDigits}{thirdDigits}");
        }

        public ICSPayloadBuilder WithoutPhoneNumber()
        {
            return WithPhoneNumber(null);
        }

        public ICSPayloadBuilder WithRandomAustralianHomePhoneNumber()
        {
            return WithPhoneNumber(GetRandomAustralianLandlineNumber());
        }

        public ICSPayloadBuilder WithRandomWesternAustralianHomePhoneNumber()
        {
            return WithPhoneNumber($"{PhonePrefix.WA_SA.GetDescription()}{GenerateRandomPhoneNumber()}");
        }

        public ICSPayloadBuilder WithEmail(string email)
        {
            Set(x => x.Email, email);
            return this;
        }

        public ICSPayloadBuilder WithEmailAddressFromName()
        {
            return WithEmail(DataHelper.RandomEmail(Get(x => x.FirstName), Get(x => x.Surname), domain: _config.Email?.Domain).Address);
        }

        public ICSPayloadBuilder WithoutPrivateEmailAddress()
        {
            return WithEmail(null);
        }

        public ICSPayloadBuilder WithMailingAddress(ContactServicePostalAddress postalAddress)
        {
            Set(x => x.PostalAddress, postalAddress);
            return this;
        }

        public ICSPayloadBuilder WithRandomMailingAddress()
        {
            // TODO: need to do some translation thing here.
            var mailingAddress = new AddressBuilder().InitialiseRandomMailingAddress().Build();
            //return WithMailingAddress(mailingAddress);
            return this;
        }

        public ICSPayloadBuilder WithoutMailingAddress()
        {
            return WithMailingAddress(null);
        }

        public ICSPayloadBuilder WithMinimumMailingAddress()
        {
            return WithMailingAddress(new ContactServicePostalAddress());
        }

        public ICSPayloadBuilder WithoutBankAccount()
        {
            Set(x => x.BankAccounts, null);
            return this;
        }

        public ICSPayloadBuilder WithBankAccount(BankAccount account)
        {
            var bankAccounts = GetOrDefault(x => x.BankAccounts, new List<BankAccount>()).ToList();
            bankAccounts.Add(account);
            Set(x => x.BankAccounts, bankAccounts);
            return this;
        }

        public ICSPayloadBuilder WithBankAccounts(List<BankAccount> accounts)
        {
            Set(x => x.BankAccounts, accounts);
            return this;
        }

        public ICSPayloadBuilder WithRandomBankAccount()
        {
            return WithBankAccount(new BankAccount().InitWithRandomValues());
        }

        public ICSPayloadBuilder WithCreditCard(CreditCard card)
        {
            var cards = GetOrDefault(x => x.CreditCards, new List<CreditCard>()).ToList();
            cards.Add(card);
            Set(x => x.CreditCards, cards);
            return this;
        }

        public ICSPayloadBuilder WithCreditCards(List<CreditCard> cards)
        {
            Set(x => x.CreditCards, cards);
            return this;
        }

        public ICSPayloadBuilder WithRandomCreditCard()
        {
            return WithCreditCard(DataHelper.RandomCreditCard());
        }

        protected override ICSContactPayload BuildEntity()
        {
            var createdContact = new ICSContactPayload
            {
                Title         = GetOrDefault(x => x.Title),
                FirstName     = GetOrDefault(x => x.FirstName),
                MiddleName    = GetOrDefault(x => x.MiddleName),
                Surname       = GetOrDefault(x => x.Surname),
                DateOfBirth   = GetOrDefault(x => x.DateOfBirth),
                Gender        = GetOrDefault(x => x.Gender),
                Membership    = GetOrDefault(x => x.Membership),
                PhoneNumber   = GetOrDefault(x => x.PhoneNumber),
                Email         = GetOrDefault(x => x.Email),
                PostalAddress = GetOrDefault(x => x.PostalAddress),
                BankAccounts  = GetOrDefault(x => x.BankAccounts),
                CreditCards   = GetOrDefault(x => x.CreditCards),
                PersonId      = GetOrDefault(x => x.PersonId)
            };

            return createdContact;
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
    }
}