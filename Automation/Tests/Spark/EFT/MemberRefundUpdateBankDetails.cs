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
        [Category(TestCategory.Spark), Category(TestCategory.EFT)]
        [Test(Description = "MRO Flow: Provide Bank Details for Refund")]
        public void MRO_Enter_Bank_Details()
        {
            if (Config.Get().Shield.IsUatEnvironment())
            {
                Reporting.SkipLog("Member Refund tests cannot be run in UAT as we don't have write permissions to the Refund storage table.");
            }

            var testData = BuildTestDataForMROFlow();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionMemberRefund.OpenMRO(_browser);
            ActionMemberRefund.CompleteRefundDetailsEntry(browser: _browser, testData, detailUiCheck:false);
            ActionMemberRefund.EnterAndVerifyOTP(browser: _browser, testData, detailUiCheck: false);
            ActionMemberRefund.EnterRefundBankDetails(browser: _browser, testData, detailUiCheck: false);
            ActionMemberRefund.VerifyConfirmationPage(_browser);

            CleanUpTestData(testData);
        }

        /// <summary>
        /// Checking the field validation for each of the UI fields
        /// </summary>
        [Category(TestCategory.Spark), Category(TestCategory.EFT)]
        [Test(Description = "MRO Flow: Provide Bank Details for Refund with Field validation")]
        public void MRO_Enter_Bank_Details_FieldValidation()
        {
            if (Config.Get().Shield.IsUatEnvironment())
            {
                Reporting.SkipLog("Member Refund tests cannot be run in UAT as we don't have write permissions to the Refund storage table.");
            }

            var testData = BuildTestDataForMROFlow();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            ActionMemberRefund.OpenMRO(_browser);
            ActionMemberRefund.CompleteRefundDetailsEntry(browser: _browser, testData, detailUiCheck:true);
            ActionMemberRefund.EnterAndVerifyOTP(browser: _browser, testData, detailUiCheck: true);
            ActionMemberRefund.EnterRefundBankDetails(browser: _browser, testData, detailUiCheck: true);
            ActionMemberRefund.VerifyConfirmationPage(_browser);

            CleanUpTestData(testData);
        }
        #endregion Test Cases

        #region Test cases helper methods
        private RefundDetails BuildTestDataForMROFlow()
        {
            var contactCandidate = ShieldContacts.FetchAContactWithRACMembershipTier(membershipTiers: [MembershipTier.Gold, MembershipTier.Silver, MembershipTier.Bronze, MembershipTier.Red, MembershipTier.Blue]);

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
                RefundID = entry.PartitionKey,
                Dob      = contactCandidate.DateOfBirth,
                LastName = contactCandidate.Surname,
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
