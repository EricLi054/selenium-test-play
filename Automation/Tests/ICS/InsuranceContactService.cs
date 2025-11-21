using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.APIDriver;
using System.Collections.Generic;
using System.Net.Http;
using static Rac.TestAutomation.Common.Constants.General;

namespace Integration
{
    [Property("Integration", "Insurance Contact Service Integration Tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class InsuranceContactService : BaseNonUITest
    {
        #region supporting data classes
        public class ICSMemberMatchTestData
        {
            public ICSMemberMatchPayload requestData { get; set; }
            public string expectedPersonId { get; set; }
            public override string ToString()
            {
                var logInfo = $"Match criteria - Given name:{requestData.FirstName} DoB:{requestData.DateOfBirth} " +
                              $"Mobile:{requestData.MobileNumber} Email:{requestData.Email}\r\n" +
                              $"Expecting match on PersonId:{expectedPersonId}";
                return logInfo;
            }
        }
        #endregion

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Insurance Contact Service integration tests");
        }

        /// <summary>
        /// Verify Claim Closure Event is triggered
        /// and the email content is correct
        /// </summary>
        [Test, TestCaseSource("ValidMinimumAnonymousContactScenarios"), Category(TestCategory.Integration), Category(TestCategory.InsuranceContactService)]
        public void INSU_Txxx_CreateAnonymousContact_SuccessCase(ICSContactPayload testdata)
        {
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testdata.ToString());

            var icsApiCreateContactResult = ContactService.GetInstance().POST_Anonymous(testdata).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiCreateContactResult, isSuccessExpected: true, "ICS Create Anonymous");
            var anonContactCreated = GetResponseContentByType<ICSContactPayload>(icsApiCreateContactResult);
            Reporting.IsNotNull(anonContactCreated, "that a contact has been created");
            Reporting.Log($"Created contact Id is {anonContactCreated.ShieldExternalNumber}");

            var icsApiFetchContactResult = ContactService.GetInstance().GET_Anonymous(anonContactCreated.ShieldExternalNumber).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiFetchContactResult, isSuccessExpected: true, "ICS Get Anonymous");
            var anonContactRetrieved = GetResponseContentByType<ICSContactPayload>(icsApiFetchContactResult);
            Reporting.IsNotNull(anonContactRetrieved, "that we could successfully retrieve the new contact");
            Reporting.IsTrue(ICSContactPayload.Compare(anonContactCreated, anonContactRetrieved), "that saved contact matches what we sent");
        }

        [Test]
        public void INSU_Txxx_IsAlive_HealthCheck()
        {
            var icsHealthCheckResponse = ContactService.GetInstance().GET_HealthCheck().GetAwaiter().GetResult();
            AssertHttpResponseCode(icsHealthCheckResponse, isSuccessExpected: true, "ICS health check");
        }

        /// <summary>
        /// Verify Claim Closure Event is triggered
        /// and the email content is correct
        /// </summary>
        [Test, TestCaseSource("MemberMatchScenariosSuccess"), Category(TestCategory.Integration), Category(TestCategory.InsuranceContactService)]
        public void INSU_Txxx_MemberMatch_SuccessCase(ICSMemberMatchTestData testdata)
        {
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testdata.ToString());

            var icsApiMemberMatchResult = ContactService.GetInstance().POST_MemberMatch(testdata.requestData).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiMemberMatchResult, isSuccessExpected: true, "ICS Member Match");
            var memberFoundByMatch = GetResponseContentByType<ICSContactPayload>(icsApiMemberMatchResult);
            Reporting.IsNotNull(memberFoundByMatch, "that a contact has been found");
            Reporting.AreEqual(testdata.expectedPersonId, memberFoundByMatch.PersonId, "matched person record is expected PersonId");
            Reporting.Log($"Matched member person Id is {memberFoundByMatch.PersonId}");

            var icsApiFetchMemberByIdResult = ContactService.GetInstance().GET_Person(testdata.expectedPersonId).GetAwaiter().GetResult();
            AssertHttpResponseCode(icsApiFetchMemberByIdResult, isSuccessExpected: true, "ICS Get Person By ID");
            var memberByPersonId = GetResponseContentByType<ICSContactPayload>(icsApiFetchMemberByIdResult);
            Reporting.IsNotNull(memberByPersonId, "that the expected member is retrievable by Person ID");
            Reporting.IsTrue(ICSContactPayload.Compare(memberByPersonId, memberFoundByMatch), "that the matched contact is the expected person");
        }

        private static IEnumerable<TestCaseData> ValidMinimumAnonymousContactScenarios()
        {
            var contact = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().Build();
            yield return new TestCaseData(contact).SetName("INSU_Txxx_CreateAnonymousContact_SuccessCase_001");

            contact = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().Build();
            yield return new TestCaseData(contact).SetName("INSU_Txxx_CreateAnonymousContact_SuccessCase_002");

            contact = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().Build();
            yield return new TestCaseData(contact).SetName("INSU_Txxx_CreateAnonymousContact_SuccessCase_003");

            contact = new ICSPayloadBuilder().InitialiseWithMinimalFields().WithAnonymousContactPrefixes().Build();
            yield return new TestCaseData(contact).SetName("INSU_Txxx_CreateAnonymousContact_SuccessCase_004");
        }

        private static IEnumerable<TestCaseData> MemberMatchScenariosSuccess()
        {

            var testDataInstance = new ICSMemberMatchTestData();
            testDataInstance.expectedPersonId = "1809b4b4-b501-e911-a968-000d3ad24077";
            testDataInstance.requestData = new ICSMemberMatchPayload();
            testDataInstance.requestData.FirstName = "Kaylah";
            testDataInstance.requestData.DateOfBirth = "2001-11-22";
            testDataInstance.requestData.MobileNumber = "0443495532";
            testDataInstance.requestData.Email = "Kaylah.Lagun@qlmpuxmd.mailosaur.net";
            yield return new TestCaseData(testDataInstance).SetName("INSU_Txxx_MemberMatch_SuccessCase_001");
        }

        private void AssertHttpResponseCode(HttpResponseMessage httpResponse, bool isSuccessExpected, string apiContextString)
        {
            string responseCodeString = $"{(int)httpResponse.StatusCode} {httpResponse.StatusCode}";
            Reporting.IsTrue(httpResponse.IsSuccessStatusCode == isSuccessExpected, $"{apiContextString} successful. Received '{responseCodeString}' from request");
        }

        private static T GetResponseContentByType<T>(HttpResponseMessage httpResponse)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        }
    }
}
