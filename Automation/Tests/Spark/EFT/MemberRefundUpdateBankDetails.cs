using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.AzureStorage;
using Rac.TestAutomation.Common.DatabaseCalls.Contacts;
using Rac.TestAutomation.Common.DataModels;
using System;
using Tests.ActionsAndValidations;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;

namespace Spark.EFT
{
    public class MemberRefundUpdateBankDetails : BaseUITest
    {

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Spark Memeber Refund Online test");
        }

        #region Test Cases
        /// <summary>
        /// Open the MRO refund link, enter refund id, enter OTP and provide a new bank details
        /// Verify Bank details are added in the Shield
        /// Verify correct shield event is created
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.EFT), Category(TestCategory.MRO), Category(TestCategory.Regression)]
        [Test(Description = "MRO Flow: Provide Bank Details for Refund")]
        public void MRO_Enter_Bank_Details()
        {
            var testData = BuildTestDataForMROFlow();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionMemberRefund.OpenMRO(_browser);
            ActionMemberRefund.CompleteRefundDetailsEntry(browser: _browser, testData, detailUiCheck:false);
            ActionMemberRefund.EnterAndVerifyOTP(browser: _browser, testData, detailUiCheck: false);
            ActionMemberRefund.EnterRefundBankDetails(browser: _browser, testData, detailUiCheck: false);
            ActionMemberRefund.VerifyConfirmationPage(_browser);
            VerifyMRO.VerifyBankDetailsInShield(testData);

            CleanUpTestData(testData);
        }

        /// <summary>
        /// Checking the field validation for each of the UI fields
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.EFT), Category(TestCategory.MRO), Category(TestCategory.Regression)]
        [Test(Description = "MRO Flow: Provide Bank Details for Refund with Field validation")]
        public void MRO_Enter_Bank_Details_FieldValidation()
        {
            var testData = BuildTestDataForMROFlow();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionMemberRefund.OpenMRO(_browser);
            ActionMemberRefund.CompleteRefundDetailsEntry(browser: _browser, testData, detailUiCheck:true);
            ActionMemberRefund.EnterAndVerifyOTP(browser: _browser, testData, detailUiCheck: true);
            ActionMemberRefund.EnterRefundBankDetails(browser: _browser, testData, detailUiCheck: true);
            ActionMemberRefund.VerifyConfirmationPage(_browser);
            VerifyMRO.VerifyBankDetailsInShield(testData);

            CleanUpTestData(testData);
        }
        #endregion Test Cases

        #region Test cases helper methods
        private RefundDetails BuildTestDataForMROFlow()
        {
            var contactCandidate = ShieldContacts.FetchAContactWithRACMembershipTier(membershipTiers: [MembershipTier.Gold, MembershipTier.Silver, MembershipTier.Bronze, MembershipTier.Red, MembershipTier.Blue]);

            if (!string.Equals(contactCandidate.Id, contactCandidate.ExternalContactNumber))
            {
                // It is possible that during MC Sync, the Contact ID was updated to the Preferred Shield ID
                // so we will ensure that we update the external contact number to match.
                var contactFromShield = DataHelper.GetContactDetailsViaContactId(contactCandidate.Id);
                contactCandidate.ExternalContactNumber = contactFromShield.ExternalContactNumber;
            }

            var azureTable = new AzureTableOperation(Config.Get().Azure.StorageMemberRefund, "refunds");

            var entry = new MemberRefundEntity()
            {
                PartitionKey     = DateTime.Now.ToString("yyyyMMddHHmmss"),
                RowKey           = string.Empty,
                ContactId        = contactCandidate.Id,
                ExternalContactNumber = contactCandidate.ExternalContactNumber,
                RefundAmount     = (double)DataHelper.RandomNumber(1, 99999)/100,
                EventDescription = "Selenium automation test data",
                FileName         = "CameFromAzureApi.json"
            };

            azureTable.AddEntity(entry);

            var testData = new RefundDetails()
            {
                ContactId   = entry.ContactId,
                RefundID    = entry.PartitionKey,
                Dob         = contactCandidate.DateOfBirth,
                LastName    = contactCandidate.Surname,
                RefundAmount     = string.Format("{0:N2}", entry.RefundAmount),
                RefundBankAmount = new BankAccount().InitWithRandomValues()
            };

            return testData;
        }

        private void CleanUpTestData(RefundDetails testData)
        {
            var azureTable = new AzureTableOperation(Config.Get().Azure.StorageMemberRefund, "refunds");

            azureTable.DeleteTableRow(testData.RefundID, string.Empty);
        }
        #endregion Test cases helper methods
    }
}
