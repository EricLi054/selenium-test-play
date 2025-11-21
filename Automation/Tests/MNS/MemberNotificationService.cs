using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.APIDriver;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System.Collections.Generic;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.General;
using Tests.ActionsAndValidations;
using System.Text;
using Environment = Rac.TestAutomation.Common.Constants.General.Environment;

namespace Integration
{
    [Property("Integration", "Member Notification Service Integration Tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class MemberNotificationService : BaseNonUITest
    {
        private List<string> _claims;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Member notification service integration tests");
            _claims = ShieldMotorClaimDB.GetOpenMotorGlassClaims();
        }

        /// <summary>
        /// Verify Claim Closure Event is triggered
        /// and the email content is correct
        /// </summary>
        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.MemberNotificationServiceEmail)]
        public void CreateEvent_Email_ClaimClosureEmail()
        {
            var testData = BuildTestDataForShieldClaimEvent(ClaimEventType.ClaimClosure);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            Reporting.LogTestStart();
            VerifyMNS.VerifyClaimClosureEmail(testData);

        }

        /// <summary>
        /// Verify Claim RepairScope Delayed Event is triggered
        /// and the SMS content is correct
        /// </summary>
        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.MemberNotificationServiceSms)]
        public void CreateEvent_SMS_ApologyReminder()
        {
            if (_testConfig.Azure.MemberNotificationService.APIEnv == Environment.uat)
            {
                var testData = BuildTestDataForShieldClaimEvent(ClaimEventType.HomeClaimRepairScopeDelayed);
                Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

                Reporting.LogTestStart();
                VerifyMNS.VerifySMSClaimRepairScopeDelayed(testData);
            }
            else
            {
                Reporting.SkipLog("This test can not be run in SIT. This is because the Twilio service for sending SMS is disabled in our SIT environments.");
            }
        }


        /// <summary>
        /// Verify Claim Processing Delayed Event is triggered
        /// and the SMS content is correct
        /// </summary>
        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.MemberNotificationServiceSms)]
        public void CreateEvent_SMS_ClaimProcessingDelayed()
        {
            if (_testConfig.Azure.MemberNotificationService.APIEnv == Environment.uat)
            {
                var testData = BuildTestDataForShieldClaimEvent(ClaimEventType.ClaimProcessingDelayed);
                Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

                Reporting.LogTestStart();
                VerifyMNS.VerifySMSClaimProcessingDelayed();
            }
            else
            {
                Reporting.SkipLog("This test can not be run in SIT. This is because the Twilio service for sending SMS is disabled in our SIT environments.");
            }
        }

        /// <summary>
        /// Verify Claim CashSettlementFactSheet Event is triggered
        /// and the SMS content is correct
        /// </summary>
        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.MemberNotificationServiceSms)]
        public void CreateEvent_SMS_ClaimCashSettlementFactSheet()
        {
            if (_testConfig.Azure.MemberNotificationService.APIEnv == Environment.uat)
            {
                var testData = BuildTestDataForShieldClaimEvent(ClaimEventType.ClaimAutomaticCSFS);
                Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

                Reporting.LogTestStart();
                VerifyMNS.VerifySMSClaimSettlementFactSheet(testData);
            }
            else
            {
                Reporting.SkipLog("This test can not be run in SIT. This is because the Twilio service for sending SMS is disabled in our SIT environments.");
            }
        }

        /// <summary>
        /// Verify Invoice Remainder Event is triggered
        ///the Email content is correct
        /// </summary>
        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.MemberNotificationServiceEmail)]
        public void CreateEvent_Email_InvoiceRemainder()
        {
            var testData = BuildTestDataForShieldClaimEvent(ClaimEventType.InvoiceReminder);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            Reporting.LogTestStart();
            VerifyMNS.VerifyInvoiceRemainderEmail(testData);
        }

        /// <summary>
        /// Verify Reminder Claim Closure Event is triggered
        /// the Email content is correct
        /// </summary>
        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.MemberNotificationServiceEmail)]
        public void CreateEvent_Email_ReminderClaimClosure()
        {
            var testData = BuildTestDataForShieldClaimEvent(ClaimEventType.ReminderClaimClosure);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());

            Reporting.LogTestStart();
            VerifyMNS.VerifyReminderClaimClosureEmail(testData);
        }


        private ClaimEvent BuildTestDataForShieldClaimEvent(ClaimEventType claimEventType)
        {
            var foundData   = false;
            var claimNumber = string.Empty;
            var claimEvent  = new ClaimEvent();
            Contact claimantContactSynced  = null;
            GetClaimResponse claimDetails  = null;
            GetQuotePolicy_Response policy = null;
            
            do
            {
                claimNumber = _claims.ConsumeRandom();

                claimDetails = DataHelper.GetClaimDetails(claimNumber);
                policy = DataHelper.GetPolicyDetails(claimDetails.PolicyNumber);

                var contactExternalNumber = claimDetails.ClaimContacts.Find(x => x.ClaimContactRole == ClaimContactRole.Informant).ContactExternalNumber;
                var informantFromShield = DataHelper.GetContactDetailsViaExternalContactNumber(contactExternalNumber);
                claimantContactSynced   = DataHelper.MapContactWithPersonAPI(informantFromShield.Id, informantFromShield.ExternalContactNumber);

                if (claimantContactSynced == null)
                {
                    Reporting.Log($"Member Central sync for contact {contactExternalNumber} (claim number {claimNumber}) failed as it likely does not exist in MC.");
                }
                else
                { foundData = true; }
            } while (!foundData && _claims.Count > 0);

            if (!foundData)
            { Reporting.Error("We exhausted the claims we had pulled from Shield and had not found any usable data."); }

            var entity = new List<Entity>();
            entity.Add(new Entity { Type = MemberNotificationServiceType.ShieldClaim.GetDescription(), Id = claimNumber, ExternalId = claimNumber });
            if (claimEventType == ClaimEventType.ClaimClosure)
            {
                entity.Add(new Entity { Type = MemberNotificationServiceType.ShieldPolicyholderClaimant.GetDescription(), Id = claimantContactSynced.Id, ExternalId = claimantContactSynced.ExternalContactNumber });
            }
            if (claimEventType == ClaimEventType.InvoiceReminder || claimEventType == ClaimEventType.ReminderClaimClosure)
            {
                entity.Add(new Entity { Type = MemberNotificationServiceType.ShieldPolicy.GetDescription(), Id = policy.Id.ToString(), ExternalId = policy.PolicyNumber });
            }

            var testData = new MemberEvent()
            {
                Type = claimEventType.GetDescription(),
                Entities = entity
            };

            Rac.TestAutomation.Common.APIDriver.MemberNotificationService.GetInstance().POST_MemberEvent(testData).GetAwaiter().GetResult();

            claimEvent.ClaimNumber  = claimNumber;
            claimEvent.PolicyNumber = policy.PolicyNumber;
            claimEvent.ClaimantName = claimantContactSynced.FirstName;
            claimEvent.ClaimCreationDate = claimDetails.ClaimInsertDate.ToString("d/MM/yyyy");
            claimEvent.ClaimEventDate    = claimDetails.EventDate.ToString("d/MM/yyyy");
            claimEvent.OutstandingExcess = ClaimEvent.GetOutstandingExcess(claimDetails.ClaimContacts);

            return claimEvent;
        }

        public class ClaimEvent
        {
            public string ClaimNumber { get; set; }
            public string PolicyNumber { get; set; }           
            public string ClaimantName { get; set;}
            public string ClaimCreationDate { get; set; }
            public string ClaimEventDate { get; set; }
            public double OutstandingExcess { get; set; }

            public override string ToString()
            {
                StringBuilder formattedString = new StringBuilder();
                formattedString.AppendLine(string.Empty);
                formattedString.AppendLine(Reporting.SEPARATOR_BAR);                
                formattedString.AppendLine($"    Claim Number:  {ClaimNumber}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Policy Number: {PolicyNumber}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Claimant Name: {ClaimantName}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Event Date:    {ClaimEventDate}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Outstanding excess:    {OutstandingExcess}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine(Reporting.SEPARATOR_BAR);
                return formattedString.ToString();
            }

            public static double GetOutstandingExcess(List<Rac.TestAutomation.Common.API.ClaimContact> claimContacts)
            {
                double outstandingExcess = 0;
                foreach(Rac.TestAutomation.Common.API.ClaimContact claimContact in claimContacts)
                {
                    if (claimContact.ClaimContactRole.Equals("IN"))
                    {
                        outstandingExcess = claimContact.OutstandingExcess;
                        break;
                    }
                }
                return outstandingExcess;
            }
        }
    }
}
