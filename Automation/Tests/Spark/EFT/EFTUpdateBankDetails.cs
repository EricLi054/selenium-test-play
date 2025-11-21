using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tests.ActionsAndValidations;
using static Rac.TestAutomation.Common.Constants.General;

namespace Spark.EFT
{
    public class EFTUpdateBankDetails : BaseUITest
    {
        private List<ClaimContact> _claimBeneficiaryContact;
        private List<ContactAndMobile> _modifiedContacts;

        /// <summary>
        /// Small class to capture contacts that we
        /// modify to support OTP.
        /// </summary>
        private class ContactAndMobile
        {
            public string ContactId { get; set; }
            public string OriginalMobile { get; set; }
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Spark EFT test");
            _claimBeneficiaryContact = ShieldClaimDB.GetOpenClaimContactForStormDamageToFenceOnly();
            _modifiedContacts = new List<ContactAndMobile>();
        }

        [OneTimeTearDown]
        public void OneTimeCleanup()
        {
            // Reverting any mobile numbers we had changed.
            foreach(var modifiedContact in _modifiedContacts)
            {
                Reporting.Log($"Attempting to revert contact {modifiedContact.ContactId} mobile phone back to '{modifiedContact.OriginalMobile}'");
                DataHelper.OverrideMemberMobileInMemberCentralByContactID(modifiedContact.ContactId, modifiedContact.OriginalMobile);
            }
            _modifiedContacts.Clear();
        }

        #region Test Cases

        /// <summary>
        /// Generate the EFT link for a random existing claim
        /// Open the EFT link and provide a new bank details
        /// Verify Bank details are added in the Shield
        /// Verify correct shield event is created
        /// Verify EFT link has expired
        /// </summary>
        [Category(TestCategory.CHaaFS), Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.EFT), Category(TestCategory.VisualTest)]
        [Test(Description = "EFT Flow: Provide New Bank Details")]
        public void INSU_T206_EFT_New_Bank_Details()
        {
            var claimData = BuildTestDataForCSFSFlow(ClaimContact.ExpectedCSFSOutcome.EFT, ClaimContact.ClaimsEFTFLow.CSFS);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claimData.ToString());

            Reporting.LogTestStart();
            ActionEFT.CreateEFTPaymentInShield(_browser, claimData.ClaimNumber, ClaimContact.ClaimsEFTFLow.EFT);
            var EFTLink = ActionEFT.RetrieveEFTLinkFromEmailAndOpenInBrowser(_browser, claimData);
            ActionEFT.EnterOneTimePasscodeAndProvideBankDetails(_browser, claimData.Beneficiary.BankAccounts.First());

            VerifyEFT.VerifyConfirmationPage(_browser, claimData);
            Reporting.LogTestShieldValidations("Claim", claimData.ClaimNumber.ToString());
            VerifyEFT.VerifyBankDetailsInShield(claimData);
            VerifyEFT.VerifyCSFSEventInShield(claimData);
            VerifyEFT.VerifyCSFSLinkExpired(_browser, EFTLink);
        }

        /// <summary>
        /// Create Cash Settlement payment
        /// Read the claim lodgement email
        /// Open the CSFS link and accept the cash settlement
        /// provide a new bank details
        /// Verify Bank details are added in the Shield
        /// Verify correct shield event is created
        /// Verify CSFS link has expired
        /// </summary>
        [Category(TestCategory.CHaaFS), Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.EFT), Category(TestCategory.VisualTest)]
        [Test(Description = "CHaaFS Flow: Accept offer for cash settlement")]
        public void INSU_T205_CSFS_Accept_Cash_Settlement_New_Bank_Details()
        {
            var claimData = BuildTestDataForCSFSFlow(ClaimContact.ExpectedCSFSOutcome.Accepted, ClaimContact.ClaimsEFTFLow.CSFS);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claimData.ToString());

            Reporting.LogTestStart();
            ActionEFT.UpdateLiabilityCreateCSFSPaymentInShield(_browser, claimData.ClaimNumber, ClaimContact.ClaimsEFTFLow.CSFS);
            var CSFSLink = ActionEFT.RetrieveEFTLinkFromEmailAndOpenInBrowser(_browser, claimData);
            ActionEFT.AcceptCashSettlement(_browser);
            ActionEFT.EnterOneTimePasscodeAndProvideBankDetails(_browser, claimData.Beneficiary.BankAccounts.First(), detailUiChecking:true);
         
            VerifyEFT.VerifyConfirmationPage(_browser, claimData);
            Reporting.LogTestShieldValidations("Claim", claimData.ClaimNumber.ToString());
            VerifyEFT.VerifyBankDetailsInShield(claimData);
            VerifyEFT.VerifyCSFSEventInShield(claimData);
            VerifyEFT.VerifyCSFSLinkExpired(_browser, CSFSLink);
        }

        /// <summary>
        /// Create Cash Settlement payment
        /// Read the claim lodgement email
        /// Open the CSFS link and reject the cash settlement
        /// Verify correct shield event is created
        /// Verify CSFS link has expired
        /// </summary>
        [Category(TestCategory.CHaaFS), Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.EFT), Category(TestCategory.VisualTest)]
        [Test(Description = "CHaaFS Flow: Decline offer for cash settlement")]
        public void INSU_T207_CSFS_Decline_Cash_Settlement()
        {
            var claimData = BuildTestDataForCSFSFlow(ClaimContact.ExpectedCSFSOutcome.Declined, ClaimContact.ClaimsEFTFLow.CSFS);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claimData.ToString());

            Reporting.LogTestStart();
            ActionEFT.UpdateLiabilityCreateCSFSPaymentInShield(_browser, claimData.ClaimNumber, ClaimContact.ClaimsEFTFLow.CSFS);
            var CSFSLink = ActionEFT.RetrieveEFTLinkFromEmailAndOpenInBrowser(_browser, claimData);
            ActionEFT.DeclineCashSettlement(_browser);
         
            VerifyEFT.VerifyConfirmationPage(_browser, claimData);
            Reporting.LogTestShieldValidations("Claim", claimData.ClaimNumber.ToString());
            VerifyEFT.VerifyCSFSEventInShield(claimData);
            VerifyEFT.VerifyCSFSLinkExpired(_browser, CSFSLink);
        }
        #endregion Test Cases


        #region Test cases helper methods

        private ClaimContact BuildTestDataForCSFSFlow(ClaimContact.ExpectedCSFSOutcome expectedCSFSOutcome, ClaimContact.ClaimsEFTFLow claimsEFTFlow)
        {
            var claimData = _claimBeneficiaryContact.ConsumeRandom();
            var Beneficiary = new ContactBuilder(claimData.Beneficiary.Id).Build();
            var contactDetails = DataHelper.GetContactDetailsViaContactId(claimData.Beneficiary.Id);
            var newEmail = new Email()
            {
                Address = DataHelper.RandomEmail(contactDetails.FirstName, contactDetails.Surname, _testConfig.Email.Domain).Address,
                IsPreferredDeliveryMethod = contactDetails.PrivateEmail?.IsPreferredDeliveryMethod
            };
            UpdateContactMobileAndEmailForOTPInMemberCentral(claimData.Beneficiary.Id, newEmail.Address);

            return new ClaimContact(claimData.ClaimNumber, Beneficiary)
                .WithExpectedOutcomeForTest(expectedCSFSOutcome)
                .WithClaimsEFTFlow(claimsEFTFlow);
        }

        /// <summary>
        /// In this method we are:
        /// * changing the mobile number as ClaimsEFT validates whether it's sending
        ///   OTP codes to a mobile in a valid range. Due to PII obfuscation, we
        ///   will have to modify the member mobile to ensure it will not block
        ///   the test. We're using a valid number from ACMA's own site:
        ///   https://www.acma.gov.au/phone-numbers-use-tv-shows-films-and-creative-works
        ///   The reason why we're just using one number, is because we expect to
        ///   revert the member's record at the end of the test.
        /// * changing the email to a Mailosaur email in order to get the EFT
        ///   link in Mailosaur.
        /// </summary>
        private void UpdateContactMobileAndEmailForOTPInMemberCentral(string contactId, string email)
        {
            var getPersonResponse = Task.Run(() => MemberCentral.GetInstance().GET_PersonByShieldContactId(contactId)).GetAwaiter().GetResult();

            if (getPersonResponse == null)
            {
                Reporting.Error($"Unable to get contact {contactId} in Member Central/Mock");
                return;
            }

            // If we don't have OTP Bypass on, then we need to make sure
            // that the member's mobile is in a valid segment.
            if (!_testConfig.IsBypassOTPEnabled())
            {
                _modifiedContacts.Add(new ContactAndMobile()
                {
                    ContactId = contactId,
                    OriginalMobile = getPersonResponse.MobilePhone
                });
                Task.Run(() => MemberCentral.GetInstance().PUT_UpdateMemberMobile(getPersonResponse, Constants.EFT.SafeTestMobileNumber)).GetAwaiter().GetResult();
            }

            Task.Run(() => MemberCentral.GetInstance().PUT_UpdateMemberEmailAddress(getPersonResponse, email)).GetAwaiter().GetResult();

            Reporting.Log($"Updated Contact {contactId} to use mobile {Constants.EFT.SafeTestMobileNumber} (original {getPersonResponse.MobilePhone}), and email {email}");
        }
        #endregion Test cases helper methods
    }
}