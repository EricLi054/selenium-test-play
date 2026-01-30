using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.APIDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;

namespace Integration
{
    [Property("Integration", "Insurance Contact Service Integration Tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class InsuranceContactService : BaseNonUITest
    {
        private List<string> _CreatedPersonPool;

        #region supporting data classes
        public class ICSMemberMatchTestData
        {
            public ICSMemberMatchPayload requestData { get; set; }
            public string expectedPersonId { get; set; }

            public override string ToString() =>$"Expecting match on PersonId:{expectedPersonId}\r\n\r\n{requestData.ToString()}";
        }
        #endregion

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Insurance Contact Service integration tests");
            _CreatedPersonPool = new List<string>();
        }

        [Test, TestCaseSource("ValidAnonymousCreateContactScenarios"), Category(TestCategory.Integration), Category(TestCategory.InsuranceContactService)]
        public void INSU_T787_CreateAnonymousContact_SuccessCase(ICSContactPayload testdata)
        {
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testdata.ToString());

            var anonContactCreated = ContactService.CreateAnonymousInShield(testdata, isSuccessExpected: true);
            Reporting.IsNotNull(anonContactCreated, "that a contact has been created");
            Reporting.Log($"Created contact Id is {anonContactCreated.ShieldExternalNumber}");
            Reporting.LogMinorSectionHeading("Comparing created contact against test data object.");
            Reporting.IsTrue(ICSContactPayload.Compare(testdata, anonContactCreated), "that created contact matches what we sent");
            Reporting.IsNull(anonContactCreated.BankAccounts, "that bank account details are not added for anonymous contacts");
            Reporting.IsNull(anonContactCreated.CreditCards,  "that credit cards details are not added for anonymous contacts");

            var anonContactRetrieved = ContactService.GetAnonymousFromShield(anonContactCreated.ShieldExternalNumber, isSuccessExpected: true);
            Reporting.IsNotNull(anonContactRetrieved, "that we could successfully retrieve the new contact");
            Reporting.LogMinorSectionHeading("Comparing created contact response against retrieved contact response.");
            Reporting.IsTrue(ICSContactPayload.Compare(anonContactCreated, anonContactRetrieved), "that response from creating contact matches what we later retrieve");
            Reporting.IsNull(anonContactRetrieved.BankAccounts, "that bank account details are not retrieved for anonymous contacts");
            Reporting.IsNull(anonContactRetrieved.CreditCards,  "that credit cards details are not retrieved for anonymous contacts");
        }

        [Test, TestCaseSource("InvalidAnonymousCreateContactScenarios"), Category(TestCategory.Integration), Category(TestCategory.InsuranceContactService)]
        public void INSU_T788_CreateAnonymousContact_FailureCase(ICSContactPayload testdata)
        {
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testdata.ToString());

            var anonContactCreated = ContactService.CreateAnonymousInShield(testdata, isSuccessExpected: false);
            Reporting.IsNull(anonContactCreated, "that a contact was not created");
        }

        [Test, TestCaseSource("ValidAnonymousUpdateContactScenarios"), Category(TestCategory.Integration), Category(TestCategory.InsuranceContactService)]
        public void INSU_T787_UpdateAnonymousContact_SuccessCase(ICSContactPayload initialCreate, ICSContactPayload updateData)
        {
            Reporting.LogMinorSectionHeading("Creating initial anonymous contact");
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, initialCreate.ToString());

            var anonContactCreated = ContactService.CreateAnonymousInShield(initialCreate, isSuccessExpected: true);
            Reporting.IsNotNull(anonContactCreated, "that a contact has been created");
            Reporting.Log($"Created contact Id is {anonContactCreated.ShieldExternalNumber}");
            Reporting.IsTrue(ICSContactPayload.Compare(initialCreate, anonContactCreated), "that created contact matches what we sent");

            Reporting.LogMinorSectionHeading($"Updating anonymous contact: {anonContactCreated.ShieldExternalNumber}");
            updateData.ShieldExternalNumber = anonContactCreated.ShieldExternalNumber;
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, updateData.ToString());
            var anonContactUpdated = ContactService.UpdateAnonymousInShield(updateData, anonContactCreated.ShieldExternalNumber, isSuccessExpected: true);
            Reporting.IsNotNull(anonContactUpdated, $"that the contact ({anonContactCreated.ShieldExternalNumber}) has been updated");
            Reporting.IsTrue(ICSContactPayload.Compare(updateData, anonContactUpdated), "that updated contact matches what we sent");

            var anonContactRetrieved = ContactService.GetAnonymousFromShield(anonContactCreated.ShieldExternalNumber, isSuccessExpected: true);
            Reporting.IsNotNull(anonContactRetrieved, "that we could successfully retrieve the updated contact");
            Reporting.LogMinorSectionHeading("Comparing updated contact response against retrieved contact response.");
            Reporting.IsTrue(ICSContactPayload.Compare(anonContactUpdated, anonContactRetrieved), "that response from updating the contact matches what we later retrieve");
        }

        [Test, TestCaseSource("ValidMCCreateContactScenarios"), Category(TestCategory.Integration), Category(TestCategory.InsuranceContactService), Order(1)]
        public void INSU_T801_CreateMCContact_SuccessCase(ICSContactPayload testdata)
        {
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testdata.ToString());

            var mcContactCreated = ContactService.CreateContactInMemberCentral(testdata, isSuccessExpected: true);
            Reporting.IsNotNull(mcContactCreated,                      "that a contact has been created");
            Reporting.IsNotNull(mcContactCreated.PersonId,             "that the created contact has a PersonId");
            Reporting.IsNotNull(mcContactCreated.ShieldExternalNumber, "that the created contact has a Shield Contact Id");
            Reporting.IsNotNull(mcContactCreated.Membership,           "that the created contact has Membership details");
            Reporting.IsNotNull(mcContactCreated.Membership.Number,    "that the created contact has a Membership Number");
            Reporting.AreEqual(MembershipTier.None.GetDescription(), mcContactCreated.Membership.Tier, ignoreCase: true, "that the created contact has Membership Tier of 'None'");

            // Buffering created person ID for use in Update Contact tests
            _CreatedPersonPool.Add(mcContactCreated.PersonId);

            // Member Central will always assign a membership even if one is not provided, with a Tier of 'None'.
            testdata.Membership = mcContactCreated.Membership;

            Reporting.Log($"Created Person Id is {mcContactCreated.PersonId}, with Shield Contact Id {mcContactCreated.ShieldExternalNumber}, and RAC ID: {mcContactCreated.Membership.Number}");
            Reporting.LogMinorSectionHeading("Comparing created contact against test data object.");
            Reporting.IsTrue(ICSContactPayload.Compare(testdata, mcContactCreated), "that created contact matches what we sent");
            Reporting.AreEqual(0, mcContactCreated.CreditCards.Count, "that we can't add credit cards (Shield uses a different mechanism for this)");
            Reporting.IsTrue(ICSContactPayload.CompareBankAccounts(testdata, mcContactCreated), "that any provided bank accounts are added to the member record");

            var mcContactRetrieved = ContactService.GetContactFromMemberCentral(mcContactCreated.PersonId, isSuccessExpected: true);
            Reporting.IsNotNull(mcContactRetrieved, "that we could successfully retrieve the new contact");
            Reporting.LogMinorSectionHeading("Comparing created contact response against retrieved contact response.");
            Reporting.IsTrue(ICSContactPayload.Compare(mcContactCreated, mcContactRetrieved), "that response from creating contact matches what we later retrieve");
            Reporting.AreEqual(0, mcContactCreated.CreditCards.Count, "that no credit card records are present on a new member");
            Reporting.IsTrue(ICSContactPayload.CompareBankAccounts(testdata, mcContactRetrieved), "that the provided bank accounts persist on record");
        }

        [Test, TestCaseSource("InvalidMCCreateContactScenarios"), Category(TestCategory.Integration), Category(TestCategory.InsuranceContactService)]
        public void INSU_T801_CreateMCContact_FailureCase(ICSContactPayload testdata)
        {
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testdata.ToString());

            var mcContactCreated = ContactService.CreateContactInMemberCentral(testdata, isSuccessExpected: false);
            Reporting.IsNull(mcContactCreated, "that a contact was not created");
        }

        [Test, TestCaseSource("ValidMCUpdateContactScenarios"), Category(TestCategory.Integration), Category(TestCategory.InsuranceContactService), Order(2)]
        public void INSU_T801_UpdateMCContact_SuccessCase(ICSContactPayload testdata)
        {
            if (_CreatedPersonPool == null || _CreatedPersonPool.Count < 1)
            { Reporting.SkipLog("We don't have any manufactured person records to operate on. Ensure 'INSU_T801_CreateMCContact_SuccessCase' is running properly."); }

            Reporting.Log($"Test is going to perform updates against this Person record: {_CreatedPersonPool[0]}");
            var personRecordOriginal = ContactService.GetContactFromMemberCentral(_CreatedPersonPool[0], isSuccessExpected: true);
            Reporting.IsNotNull(personRecordOriginal, "that we could successfully retrieve the contact for the test");
            Reporting.LogMinorSectionHeading("Original Person Record:");
            Reporting.Log(personRecordOriginal.ToString());

            var updatePayload = ICSContactPayload.DuplicateFull(personRecordOriginal);
            Reporting.LogMinorSectionHeading("Values being updated in test:");
            updatePayload.ApplyNonNullChanges(testdata);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, updatePayload.ToString());

            Reporting.LogMinorSectionHeading($"Updating anonymous contact: {updatePayload.PersonId}");
            var updateResponse = ContactService.UpdateContactInMemberCentral(updatePayload, isSuccessExpected: true);
            Reporting.IsNotNull(updateResponse, $"that the contact ({updateResponse.PersonId}) has been updated");
            Reporting.IsTrue(ICSContactPayload.Compare(updatePayload, updateResponse), "that updated contact matches what we sent");
            Reporting.IsTrue(ICSContactPayload.CompareBankAccounts(updatePayload, updateResponse), "that bank account updates were applied correctly");

            var mcContactRetrieved = ContactService.GetContactFromMemberCentral(updateResponse.PersonId, isSuccessExpected: true);
            Reporting.IsNotNull(mcContactRetrieved, "that we could successfully retrieve the updated contact");
            Reporting.LogMinorSectionHeading("Comparing updated contact response against retrieved contact response.");
            Reporting.IsTrue(ICSContactPayload.Compare(updateResponse, mcContactRetrieved), "that response from updating the contact, matches what we later retrieve");
            Reporting.IsTrue(ICSContactPayload.CompareBankAccounts(updateResponse, mcContactRetrieved), "that bank account were retained correctly since update");
        }

        [Test, TestCaseSource("InvalidMCUpdateContactScenarios"), Category(TestCategory.Integration), Category(TestCategory.InsuranceContactService), Order(3)]
        public void INSU_T801_UpdateMCContact_FailureCase(ICSContactPayload testdata)
        {
            if (_CreatedPersonPool == null || _CreatedPersonPool.Count < 1)
            { Reporting.SkipLog("We don't have any manufactured person records to operate on. Ensure 'INSU_T801_CreateMCContact_FailureCase' is running properly."); }

            Reporting.Log($"Test is going to perform updates against this Person record: {_CreatedPersonPool[0]}");
            var personRecordOriginal = ContactService.GetContactFromMemberCentral(_CreatedPersonPool[0], isSuccessExpected: true);
            Reporting.IsNotNull(personRecordOriginal, "that we could successfully retrieve the contact for the test");

            var updatePayload = ICSContactPayload.DuplicateFull(personRecordOriginal);
            Reporting.LogMinorSectionHeading("Values being updated in test:");
            updatePayload.ApplyNonNullChanges(testdata);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, updatePayload.ToString());

            Reporting.LogMinorSectionHeading($"Updating anonymous contact: {updatePayload.PersonId}");
            var updateResponse = ContactService.UpdateContactInMemberCentral(updatePayload, isSuccessExpected: false);
            Reporting.IsNull(updateResponse, $"that the contact ({updatePayload.PersonId}) was NOT updated");

            var mcContactRetrieved = ContactService.GetContactFromMemberCentral(updatePayload.PersonId, isSuccessExpected: true);
            Reporting.IsNotNull(mcContactRetrieved, "that we could successfully retrieve the updated contact");
            Reporting.LogMinorSectionHeading("Comparing updated contact response against retrieved contact response.");
            Reporting.IsTrue(ICSContactPayload.Compare(personRecordOriginal, mcContactRetrieved), "that the contact was not changed after failed update");
        }

        [Test, Category(TestCategory.Integration), Category(TestCategory.InsuranceContactService)]
        public void INSU_T789_IsAlive_HealthCheck()
        {
            Reporting.IsTrue(ContactService.ICSHealthCheck(), "ICS Health check is returning success");
        }

        [Test, TestCaseSource("MemberMatchScenariosSuccess"), Category(TestCategory.Integration), Category(TestCategory.InsuranceContactService)]
        public void INSU_T802_MemberMatch_SuccessCase(ICSMemberMatchTestData testdata)
        {
            if (Config.Get().IsMCMockEnabled())
            { Reporting.SkipLog("Current ICS Member Match scenarios are based on real Member Central data. Cannot run against mocked environment."); }

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testdata.ToString());

            var memberFoundByMatch = ContactService.MemberMatch(testdata.requestData, isSuccessExpected: true);
            Reporting.IsNotNull(memberFoundByMatch, "that a contact has been found");
            Reporting.AreEqual(testdata.expectedPersonId, memberFoundByMatch.PersonId, "matched person record is expected PersonId");
            Reporting.Log($"Matched member person Id is {memberFoundByMatch.PersonId}");

            var memberByPersonId = ContactService.GetPersonFromMemberCentral(testdata.expectedPersonId, isSuccessExpected: true);
            Reporting.IsNotNull(memberByPersonId, "that the expected member is retrievable by Person ID");
            Reporting.IsTrue(ICSContactPayload.Compare(memberByPersonId, memberFoundByMatch), "that the matched contact is the expected person");
        }

        [Test, TestCaseSource("MemberMatchScenariosFailure"), Category(TestCategory.Integration), Category(TestCategory.InsuranceContactService)]
        public void INSU_T802_MemberMatch_FailureCase(ICSMemberMatchTestData testdata)
        {
            if (Config.Get().IsMCMockEnabled())
            { Reporting.SkipLog("Current ICS Member Match scenarios are based on real Member Central data. Cannot run against mocked environment."); }

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testdata.ToString());

            var memberMatchFailure = ContactService.MemberMatch(testdata.requestData, isSuccessExpected: false);
            Reporting.IsNull(memberMatchFailure, "that no member was found");
        }

        private static IEnumerable<TestCaseData> ValidAnonymousCreateContactScenarios()
        {
            var contact = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().Build();
            yield return new TestCaseData(contact).SetName("INSU_T787_CreateAnonymousContact_SuccessCase_Minimum");

            contact = new ICSPayloadBuilder().InitialiseWithBasicProspectContact().Build();
            yield return new TestCaseData(contact).SetName("INSU_T787_CreateAnonymousContact_SuccessCase_BasicProspect");

            contact = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().WithEmailAddressFromName().Build();
            yield return new TestCaseData(contact).SetName("INSU_T787_CreateAnonymousContact_SuccessCase_WithEmail");

            contact = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().WithRandomMobileNumber().Build();
            yield return new TestCaseData(contact).SetName("INSU_T787_CreateAnonymousContact_SuccessCase_WithMobile");

            contact = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().WithRandomAustralianHomePhoneNumber().Build();
            yield return new TestCaseData(contact).SetName("INSU_T787_CreateAnonymousContact_SuccessCase_WithLandline");

            contact = new ICSPayloadBuilder().InitialiseWithRandomInidividualNoFinancials().WithAnonymousContactPrefixes().Build();
            yield return new TestCaseData(contact).SetName("INSU_T787_CreateAnonymousContact_SuccessCase_FullAnonymous");

            contact = new ICSPayloadBuilder().InitialiseWithRandomInidividual().WithAnonymousContactPrefixes().Build();
            yield return new TestCaseData(contact).SetName("INSU_T787_CreateAnonymousContact_SuccessCase_FullAnonymousWithFinancials");
        }

        private static IEnumerable<TestCaseData> InvalidAnonymousCreateContactScenarios()
        {
            var contact = new ICSPayloadBuilder().InitialiseWithMinimalFields().Build();
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_NoAnonymousPrefixes");

            var baseGoodContact = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().Build();
            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.FirstName = null;
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_NoFirstName");

            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.Surname = null;
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_NoSurname");

            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.DateOfBirth = null;
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_NoDateOfBirth");

            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.PostalAddress = null;
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_NoPostalAddress");

            baseGoodContact = new ICSPayloadBuilder().InitialiseWithRandomInidividualNoFinancials().WithAnonymousContactPrefixes().Build();
            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.FirstName = $"{ICSPayloadBuilder.AnonymousPrefix}_{DataHelper.RandomLetters(46)}";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_FirstNameExceeds50Characters");

            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.MiddleName = $"{ICSPayloadBuilder.AnonymousPrefix}_{DataHelper.RandomLetters(46)}";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_MiddleNameExceeds50Characters");

            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.Surname = $"{ICSPayloadBuilder.AnonymousPrefix}_{DataHelper.RandomLetters(51)}";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_SurnameExceeds55Characters");

            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.PhoneNumber = "0154326234";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_PhoneNumberWithBadAreaCode");

            contact.PhoneNumber = "54326234";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_PhoneNumberWithMissingAreaCode");

            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.Email = "@ractest.com.au";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_BadEmailDomainOnly");

            contact.Email = "testemail";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_MissingEmailDomain");

            contact.Email = "123@ractest";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_IncompleteEmail");

            contact.Email = "bad'characters@ractest.com.au";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_InvalidCharacter");

            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.Title = "Overlord";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_InvalidTitle");

            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.DateOfBirth = "1970/07";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_BirthdateMissingDay");

            contact.DateOfBirth = "1970/21/07";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_BirthdateInvalidSequence");

            contact.DateOfBirth = "12/07/1970";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_BirthdateReversedSequence");

            contact.DateOfBirth = "1971/02/29";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_BirthdateInvalidDate");

            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.Gender = "Undisclosed";
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_UnsupportedGender");

            contact = ICSContactPayload.Duplicate(baseGoodContact);
            contact.Membership = new ContactServiceMembership()
            {
                Tier = "Orange"
            };
            yield return new TestCaseData(contact).SetName("INSU_T788_CreateAnonymousContact_FailureCase_UnsupportedMembershipTier");
        }

        private static IEnumerable<TestCaseData> ValidAnonymousUpdateContactScenarios()
        {
            var baseMinimalContact = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().Build();
            var updatedContact     = ICSContactPayload.Duplicate(baseMinimalContact);
            updatedContact.MiddleName = DataHelper.RandomLetters(minLength: 5, maxLength: 50);
            yield return new TestCaseData(new object[] {baseMinimalContact, updatedContact}).SetName("INSU_T787_UpdateAnonymousContact_SuccessCase_AddMiddleName");

            baseMinimalContact = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().Build();
            updatedContact     = ICSContactPayload.Duplicate(baseMinimalContact);
            var genderOptions = new[]
            {
                Gender.Male,
                Gender.Female
            };
            var genderEnumValue   = genderOptions.OrderBy(t => Guid.NewGuid()).First();
            updatedContact.Gender = genderEnumValue.GetDescription();
            updatedContact.Title  = DataHelper.GetRandomTitleForGender(genderEnumValue).GetDescription();
            yield return new TestCaseData(new object[] { baseMinimalContact, updatedContact }).SetName("INSU_T787_UpdateAnonymousContact_SuccessCase_AddGenderAndTitle");

            baseMinimalContact    = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().Build();
            updatedContact        = ICSContactPayload.Duplicate(baseMinimalContact);
            updatedContact.Email  = DataHelper.RandomEmail(baseMinimalContact.FirstName, baseMinimalContact.Surname, Config.Get().Email.Domain).Address.ToLower();
            yield return new TestCaseData(new object[] { baseMinimalContact, updatedContact }).SetName("INSU_T787_UpdateAnonymousContact_SuccessCase_AddEmail");

            baseMinimalContact    = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().Build();
            updatedContact        = ICSContactPayload.Duplicate(baseMinimalContact);
            updatedContact.PhoneNumber = DataHelper.RandomMobileNumber();
            yield return new TestCaseData(new object[] { baseMinimalContact, updatedContact }).SetName("INSU_T787_UpdateAnonymousContact_SuccessCase_AddMobile");

            baseMinimalContact    = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().Build();
            updatedContact        = ICSContactPayload.Duplicate(baseMinimalContact);
            updatedContact.Membership = new ContactServiceMembership() 
            { 
                Number = DataHelper.RandomNumber(1000000, 8000000).ToString(),
                Tier   = "Silver",
                Tenure = DataHelper.RandomNumber(1, 10)
            };
            yield return new TestCaseData(new object[] { baseMinimalContact, updatedContact }).SetName("INSU_T787_UpdateAnonymousContact_SuccessCase_AddMembership");

            baseMinimalContact = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().Build();
            updatedContact     = new ICSPayloadBuilder().InitialiseWithRandomInidividual().WithRandomMembership().WithAnonymousContactPrefixes().Build();
            yield return new TestCaseData(new object[] { baseMinimalContact, updatedContact }).SetName("INSU_T787_UpdateAnonymousContact_SuccessCase_UpdateAllFields");
        }

        /// <summary>
        /// There is a limited set of data here because we're reluctant to create too many
        /// contacts in Member Central, and also rules in Insurance applications mean that
        /// most member records will be in a complete state. So even though it is possible
        /// to create member records with varied combinations, Insurance will always be
        /// sending records that have full details.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<TestCaseData> ValidMCCreateContactScenarios()
        {
            var contact = new ICSPayloadBuilder().InitialiseWithRandomInidividualNoFinancials().Build();
            yield return new TestCaseData(contact).SetName("INSU_T801_CreateMCContact_SuccessCase_NoFinancials");

            var poBoxAddress = new ContactServicePostalAddress()
            {
                POBox              = "GPO Box A123",
                IsAddressValidated = false,
                Suburb             = "WEST PERTH",
                PostCode           = "6872",
                State              = "WA",
                Country            = "AUSTRALIA",
                FormattedAddress   = "GPO Box A123 WEST PERTH, WA 6872"
            };
            contact = new ICSPayloadBuilder().InitialiseWithRandomInidividual().WithMailingAddress(poBoxAddress).Build();
            yield return new TestCaseData(contact).SetName("INSU_T801_CreateMCContact_SuccessCase_POBox");

            contact = new ICSPayloadBuilder().InitialiseWithRandomInidividual().WithoutPhoneNumber().WithRandomAustralianHomePhoneNumber().Build();
            yield return new TestCaseData(contact).SetName("INSU_T801_CreateMCContact_SuccessCase_LandlineNumber");
        }

        private static IEnumerable<TestCaseData> InvalidMCCreateContactScenarios()
        {
            var contact = new ICSPayloadBuilder().InitialiseWithRandomInidividualNoFinancials().WithAnonymousContactPrefixes().Build();

            yield return new TestCaseData(contact).SetName("INSU_T801_CreateMCContact_FailureCase_AnonymousPrefixes");

            contact = new ICSPayloadBuilder().InitialiseWithRandomInidividual().WithoutSurname().Build();
            yield return new TestCaseData(contact).SetName("INSU_T801_CreateMCContact_FailureCase_WithoutSurname");

            contact = new ICSPayloadBuilder().InitialiseWithRandomInidividual().WithoutFirstName().Build();
            yield return new TestCaseData(contact).SetName("INSU_T801_CreateMCContact_FailureCase_WithoutFirstName");

            contact = new ICSPayloadBuilder().InitialiseWithRandomInidividual().WithoutPrivateEmailAddress().WithoutPhoneNumber().WithoutMailingAddress().Build();
            yield return new TestCaseData(contact).SetName("INSU_T801_CreateMCContact_FailureCase_NoContactInfo");
        }

        private static IEnumerable<TestCaseData> ValidMCUpdateContactScenarios()
        {
            var contact = new ICSContactPayload();
            contact.FirstName = DataHelper.RandomLetters(minLength:10, maxLength:40);
            yield return new TestCaseData(contact).SetName("INSU_T801_UpdateMCContact_SuccessCase_ChangeGivenName");

            contact = new ICSContactPayload();
            contact.MiddleName = DataHelper.RandomLetters(minLength: 10, maxLength: 40);
            yield return new TestCaseData(contact).SetName("INSU_T801_UpdateMCContact_SuccessCase_ChangeMiddleName");

            contact = new ICSContactPayload();
            contact.Surname = DataHelper.RandomLetters(minLength: 10, maxLength: 40);
            yield return new TestCaseData(contact).SetName("INSU_T801_UpdateMCContact_SuccessCase_ChangeSurname");

            contact = new ICSContactPayload();
            contact.PhoneNumber = DataHelper.RandomMobileNumber();
            yield return new TestCaseData(contact).SetName("INSU_T801_UpdateMCContact_SuccessCase_ChangeMobileNumber");

            contact = new ICSContactPayload();
            contact.Email = DataHelper.RandomEmail(DataHelper.RandomLetters(6), DataHelper.RandomLetters(6), Config.Get().Email.Domain).Address.ToLower();
            yield return new TestCaseData(contact).SetName("INSU_T801_UpdateMCContact_SuccessCase_ChangeEmail");

            contact = new ICSContactPayload();
            contact.BankAccounts = new List<BankAccount>() { new BankAccount().InitWithRandomValues() };
            yield return new TestCaseData(contact).SetName("INSU_T801_UpdateMCContact_SuccessCase_AddBankAccount");
        }

        private static IEnumerable<TestCaseData> InvalidMCUpdateContactScenarios()
        {
            var contact = new ICSContactPayload();
            contact.FirstName = ICSPayloadBuilder.AnonymousPrefix + DataHelper.RandomLetters(minLength: 10, maxLength: 40);
            yield return new TestCaseData(contact).SetName("INSU_T801_UpdateMCContact_FailureCase_AnonymousPrefixGivenName");

            contact = new ICSContactPayload();
            contact.Surname = ICSPayloadBuilder.AnonymousPrefix + DataHelper.RandomLetters(minLength: 10, maxLength: 40);
            yield return new TestCaseData(contact).SetName("INSU_T801_UpdateMCContact_FailureCase_AnonymousPrefixSurname");

            contact = new ICSContactPayload();
            contact.FirstName = string.Empty;
            yield return new TestCaseData(contact).SetName("INSU_T801_UpdateMCContact_FailureCase_DeleteGivenName");

            contact = new ICSContactPayload();
            contact.Surname = string.Empty;
            yield return new TestCaseData(contact).SetName("INSU_T801_UpdateMCContact_FailureCase_DeleteSurname");

            contact = new ICSContactPayload();
            contact.Email = string.Empty;
            yield return new TestCaseData(contact).SetName("INSU_T801_UpdateMCContact_FailureCase_DeleteEmail");

            contact = new ICSContactPayload();
            contact.PhoneNumber = string.Empty;
            yield return new TestCaseData(contact).SetName("INSU_T801_UpdateMCContact_FailureCase_DeletePhone");
        }

        private static IEnumerable<TestCaseData> MemberMatchScenariosSuccess()
        {
            var testDataInstance = new ICSMemberMatchTestData();
            testDataInstance.expectedPersonId         = "1809b4b4-b501-e911-a968-000d3ad24077";
            testDataInstance.requestData = new ICSMemberMatchPayload();
            testDataInstance.requestData.FirstName    = "Kaylah";
            testDataInstance.requestData.Surname      = DataHelper.RandomLetters(minLength: 4, maxLength: 45);
            testDataInstance.requestData.DateOfBirth  = "2001-11-22";
            testDataInstance.requestData.MobileNumber = "0443495532";
            testDataInstance.requestData.Email        = DataHelper.RandomEmail(domain: Config.Get().Email.Domain).Address.ToLower();
            testDataInstance.requestData.StreetName   = DataHelper.RandomLetters(minLength: 5, maxLength: 30);
            testDataInstance.requestData.Suburb       = DataHelper.RandomLetters(minLength: 5, maxLength: 30);
            yield return new TestCaseData(testDataInstance).SetName("INSU_T802_MemberMatch_SuccessCase_MobileOnlyMatches");

            testDataInstance = new ICSMemberMatchTestData();
            testDataInstance.expectedPersonId         = "b6593f25-98de-7df4-f6d7-118a4bc6acb4";
            testDataInstance.requestData = new ICSMemberMatchPayload();
            testDataInstance.requestData.FirstName    = "Renier";
            testDataInstance.requestData.Surname      = DataHelper.RandomLetters(minLength: 4, maxLength: 45);
            testDataInstance.requestData.DateOfBirth  = "1950-10-08";
            testDataInstance.requestData.MobileNumber = DataHelper.RandomMobileNumber();
            testDataInstance.requestData.Email        = "Renier.Dickens@qlmpuxmd.mailosaur.net";
            testDataInstance.requestData.StreetName   = DataHelper.RandomLetters(minLength: 5, maxLength: 30);
            testDataInstance.requestData.Suburb       = DataHelper.RandomLetters(minLength: 5, maxLength: 30);
            yield return new TestCaseData(testDataInstance).SetName("INSU_T802_MemberMatch_SuccessCase_EmailOnlyMatches");

            testDataInstance = new ICSMemberMatchTestData();
            testDataInstance.expectedPersonId         = "f1d8db04-09fa-9ff0-eee9-5b283e678c9d";
            testDataInstance.requestData = new ICSMemberMatchPayload();
            testDataInstance.requestData.FirstName    = "Koral";
            testDataInstance.requestData.Surname      = DataHelper.RandomLetters(minLength: 4, maxLength: 45);
            testDataInstance.requestData.DateOfBirth  = "1990-05-28";
            testDataInstance.requestData.MobileNumber = DataHelper.RandomMobileNumber();
            testDataInstance.requestData.Email        = DataHelper.RandomEmail(domain: Config.Get().Email.Domain).Address.ToLower();
            testDataInstance.requestData.StreetName   = "Hamersley";
            testDataInstance.requestData.Suburb       = "Esperance";
            yield return new TestCaseData(testDataInstance).SetName("INSU_T802_MemberMatch_SuccessCase_AddressOnlyMatches");

            testDataInstance = new ICSMemberMatchTestData();
            testDataInstance.expectedPersonId         = "7b1c4e9f-6a63-b209-346f-0001065422f0";
            testDataInstance.requestData = new ICSMemberMatchPayload();
            testDataInstance.requestData.FirstName    = "Rhonda";
            testDataInstance.requestData.DateOfBirth  = "1987-07-23";
            testDataInstance.requestData.MobileNumber = "0443586844";
            yield return new TestCaseData(testDataInstance).SetName("INSU_T802_MemberMatch_SuccessCase_MinimumData");

            testDataInstance = new ICSMemberMatchTestData();
            testDataInstance.expectedPersonId         = "e4789aa3-603a-b72d-4d63-bf637d0317cb";
            testDataInstance.requestData = new ICSMemberMatchPayload();
            testDataInstance.requestData.FirstName    = "Mirjana";
            testDataInstance.requestData.Surname      = "Cope";
            testDataInstance.requestData.DateOfBirth  = "1969-10-26";
            testDataInstance.requestData.MobileNumber = "0442227367";
            testDataInstance.requestData.Email        = "Mirjana.Cope@qlmpuxmd.mailosaur.net";
            testDataInstance.requestData.StreetName   = "Butchart";
            testDataInstance.requestData.Suburb       = "AUBIN GROVE";
            yield return new TestCaseData(testDataInstance).SetName("INSU_T802_MemberMatch_SuccessCase_AllDataMatches");
        }

        private static IEnumerable<TestCaseData> MemberMatchScenariosFailure()
        {
            var testDataInstance = new ICSMemberMatchTestData();
            testDataInstance.expectedPersonId         = "1809b4b4-b501-e911-a968-000d3ad24077";
            testDataInstance.requestData = new ICSMemberMatchPayload();
            testDataInstance.requestData.FirstName    = "Kaylah";
            testDataInstance.requestData.Surname      = "Lagun";
            testDataInstance.requestData.DateOfBirth  = "2001-11-22";
            testDataInstance.requestData.MobileNumber = DataHelper.RandomMobileNumber();
            testDataInstance.requestData.Email        = DataHelper.RandomEmail(domain: Config.Get().Email.Domain).Address.ToLower();
            testDataInstance.requestData.StreetName   = DataHelper.RandomLetters(minLength: 5, maxLength: 30);
            testDataInstance.requestData.Suburb       = DataHelper.RandomLetters(minLength: 5, maxLength: 30);
            yield return new TestCaseData(testDataInstance).SetName("INSU_T802_MemberMatch_FailureCase_AllFieldsPresentNoMatch");

            testDataInstance = new ICSMemberMatchTestData();
            testDataInstance.expectedPersonId         = "1809b4b4-b501-e911-a968-000d3ad24077";
            testDataInstance.requestData = new ICSMemberMatchPayload();
            testDataInstance.requestData.FirstName    = "Kaylah";
            testDataInstance.requestData.Surname      = null;
            testDataInstance.requestData.DateOfBirth  = null;
            testDataInstance.requestData.MobileNumber = "0443495532";
            testDataInstance.requestData.Email        = "Kaylah.Lagun@qlmpuxmd.mailosaur.net";
            testDataInstance.requestData.StreetName   = "Foss";
            testDataInstance.requestData.Suburb       = "BICTON";
            yield return new TestCaseData(testDataInstance).SetName("INSU_T802_MemberMatch_FailureCase_MissingDateOfBirthAndSurname");

            testDataInstance = new ICSMemberMatchTestData();
            testDataInstance.expectedPersonId         = "1809b4b4-b501-e911-a968-000d3ad24077";
            testDataInstance.requestData = new ICSMemberMatchPayload();
            testDataInstance.requestData.FirstName    = null;
            testDataInstance.requestData.Surname      = "Lagun";
            testDataInstance.requestData.DateOfBirth  = "2001-11-22";
            testDataInstance.requestData.MobileNumber = "0443495532";
            testDataInstance.requestData.Email        = "Kaylah.Lagun@qlmpuxmd.mailosaur.net";
            testDataInstance.requestData.StreetName   = "Foss";
            testDataInstance.requestData.Suburb       = "BICTON";
            yield return new TestCaseData(testDataInstance).SetName("INSU_T802_MemberMatch_FailureCase_MissingFirstName");

            testDataInstance = new ICSMemberMatchTestData();
            testDataInstance.expectedPersonId         = "1809b4b4-b501-e911-a968-000d3ad24077";
            testDataInstance.requestData = new ICSMemberMatchPayload();
            testDataInstance.requestData.FirstName    = "Kaylah";
            testDataInstance.requestData.DateOfBirth  = "2001-11-22";
            yield return new TestCaseData(testDataInstance).SetName("INSU_T802_MemberMatch_FailureCase_MissingSearchCriteria");
        }
    }
}
