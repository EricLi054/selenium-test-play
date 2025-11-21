using NUnit.Framework;
using System;
using Rac.TestAutomation.Common;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.Endorsements.Cancellations;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using Tests.ActionsAndValidations.Endorsements;
using UIDriver.Pages;
using Rac.TestAutomation.Common.TestData.Endorsements;
using Tests.ActionsAndValidations;
using Rac.TestAutomation.Common.DatabaseCalls.Queries.Environment;

namespace Spark.Cancellations
{
    [Property("Functional", "B2C tests")]
    public class CancelPolicy : BaseUITest
    {

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Policy Cancellation tests");
        }

        #region Test Cases
        /// <summary>
        /// Test emulates a member that wishes to cancel their motor policy.
        /// 1. Launch policy cancellation test page.
        /// 2. Confirm policy details and default day is current day
        /// 3. Check error messages for no input
        /// 4. Fill in form with cancellation reason of "I'm changing to another insurer
        /// 5. Click [Cancel policy] button. Verify modal dialog
        /// 6. Click [Confirm cancellation] button
        /// 7. Check details on policy cancellation confirmation page
        /// 8. Verify policy has been updated in Shield
        /// </summary>
        [Test(Description = "Cancel Motor Policy Via Spark Test Launch Page"),
            Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.PolicyCancellation),
            Category(TestCategory.Endorsement), Category(TestCategory.Motor), Category(TestCategory.MultiFactorAuthentication)]
        public void INSU_T105_Policy_Cancellations_Motor_Midterm_CurrentDay_RefundToBankAccount()
        { 
            ShieldParametersDB.RequireShieldFeatureToggleForRefundIsTrue();
            var testData = BuildTestDataForCancelMotorPolicy();

            Reporting.LogTestStart();
            LaunchPage.OpenCancelPolicyURL(_browser, testData);

            DateTime currentDay = DateTime.Now.Date;
            VerifyCancelPolicy.VerifyMotorPolicyDetailsPopulated(_browser, testData, currentDay);

            ActionsCancelPolicy.SubmitCancellationWithErrorsAndCheckFieldValidationMessages(_browser);

            ActionsCancelPolicy.CompleteCancellationDetails(_browser, testData.ActivePolicyHolder,
                Reason.ChangingInsurer.Text);

            ActionsCancelPolicy.SubmitCancellationRequest(_browser);

            ActionsCancelPolicy.CheckConfirmationModalAndThenConfirmCancellation(_browser,
                "car insurance", testData.PolicyNumber, DateTime.Now.Date);

            ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber, detailUiChecking: true);

            VerifyCancelPolicy.VerifyPolicyCancellationConfirmationPage(_browser, testData.PolicyNumber, testData.ActivePolicyHolder.FirstName);

            var shieldUpdatedValues = ShieldPolicyDB.FetchCancellationDetailsForPolicy(testData.PolicyNumber, testData.ActivePolicyHolder.Id);
            VerifyCancelPolicy.VerifyShieldUpdated(shieldUpdatedValues, currentDay, Reason.ChangingInsurer.ExternalCode, testData.ActivePolicyHolder.GetEmail(), false);
            VerifyCancelPolicy.VerifyRefundInformation(_browser, shieldUpdatedValues.FinalInstallment, RefundDestinationType.BankAccount);
        }

        /// <summary>
        /// Test emulates a member cancelling their policy in the first 28 days and
        /// that they receive a prompt to call about their Roadside Assistance product.
        /// 1. Launch policy cancellation test page
        /// 2. Confirm policy details and default day is current day
        /// 3. Check field validation messages
        /// 4. Change reason to "This policy doesn't suit my needs" AKA 28 day free look
        /// 5. Confirm last day of cover changes to first day of policy and field is no longer editable
        /// 6. Confirm refund to source text
        /// 7. Upon clicking [Cancel policy] button modal with correct details appears 
        /// 8. Upon confirmation, policy cancellation page loads
        /// 9. Prompt for existing roadside assistance product appears as well as refund destination
        /// 10. Verify policy has been updated in Shield
        /// </summary>
        [Test(Description = "Cancel Motor Policy with Roadside Assistance Prompt Via Spark Test Launch Page"),
            Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.PolicyCancellation),
            Category(TestCategory.Endorsement), Category(TestCategory.Motor), Category(TestCategory.SparkB2CRegressionForMemberCentralReleases)]
        public void INSU_T106_Policy_Cancellations_Motor_First28Days_RoadsideAssistance_RefundToCard()
        {
            ShieldParametersDB.RequireShieldFeatureToggleForRefundIsTrue();
            var testData = BuildTestDataForCancelMotorFirst28DaysWithRoadsidePolicy();

            Reporting.LogTestStart();
            LaunchPage.OpenCancelPolicyURL(_browser, testData);

            DateTime currentDay = DateTime.Now.Date;
            VerifyCancelPolicy.VerifyMotorPolicyDetailsPopulated(_browser, testData, currentDay);

            ActionsCancelPolicy.SubmitCancellationWithErrorsAndCheckFieldValidationMessages(_browser);

            ActionsCancelPolicy.CompleteCancellationDetails(_browser, testData.ActivePolicyHolder,
                Reason.CancelWithin28DayFreeLookPeriod.Text);

            VerifyCancelPolicy.VerifyDatePickerIsDisabledForUpdates(_browser);
            VerifyCancelPolicy.ValidateRefundProcessingMessage(_browser);

            ActionsCancelPolicy.SubmitCancellationRequest(_browser);

            ActionsCancelPolicy.CheckConfirmationModalAndThenConfirmCancellation(_browser,
                "car insurance", testData.PolicyNumber, testData.StartDate);

            ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber, retryOTP: true);

            VerifyCancelPolicy.VerifyPolicyCancellationConfirmationPage(_browser, testData.PolicyNumber, testData.ActivePolicyHolder.FirstName);

            var shieldUpdatedValues = ShieldPolicyDB.FetchCancellationDetailsForPolicy(testData.PolicyNumber, testData.ActivePolicyHolder.Id);

            VerifyCancelPolicy.VerifyShieldUpdated(shieldUpdatedValues, testData.StartDate,
                Reason.CancelWithin28DayFreeLookPeriod.ExternalCode, testData.ActivePolicyHolder.GetEmail(), false);
            VerifyCancelPolicy.VerifyRoadsideMessage(_browser);
            VerifyCancelPolicy.VerifyRefundInformation(_browser, shieldUpdatedValues.FinalInstallment, RefundDestinationType.Card);
        }

        /// <summary>
        /// Test emulates a member wishing to cancel their home policy during 
        /// the renewal period and allows them to choose the renewal date.
        /// Check that extra information is displayed for specific reasons.
        /// 1. Launch Policy Cancellation test page
        /// 2. Confirm policy details 
        /// 3. Check error messages for no input
        /// 4. Change reason to "I sold my house". Check for additional information.
        /// 5. Change reason to "My house has been demolished"
        /// 6. Update renewal date to the last day of the policy
        /// 7. Verify confirmation modal
        /// 8. Verify cancellation confirmation page
        /// 9. Confirm details have been updated in SHIELD
        /// </summary>
        [Test(Description = "Cancel Home Policy Via Spark Test Launch Page"),
            Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.PolicyCancellation),
            Category(TestCategory.Endorsement), Category(TestCategory.Home)]
        public void INSU_T107_Policy_Cancellations_Home_RenewalLastDay_ChangingReasons()
        {
            ShieldParametersDB.RequireShieldFeatureToggleForRefundIsTrue();
            var testData = BuildTestDataForCancelHomePolicyInRenewal();

            Reporting.LogTestStart();
            LaunchPage.OpenCancelPolicyURL(_browser, testData);
            VerifyCancelPolicy.VerifyHomePolicyDetailsPopulated(_browser, testData, DateTime.Now.Date);
            ActionsCancelPolicy.SubmitCancellationWithErrorsAndCheckFieldValidationMessages(_browser);

            ActionsCancelPolicy.UpdateEmailAddress(_browser, testData.ActivePolicyHolder.GetEmail());

            ActionsCancelPolicy.UpdateCancellationForSellingHouseAndCheckExtraInformationDisplayed(_browser);
            ActionsCancelPolicy.UpdateCancellationForHouseDemolishedAndCheckExtraInformationDisplayed(_browser);

            // Policy is in renewal (so Shield has all its dates for the next term already).
            // We want to cancel on the end of THIS term, and so use "Status Renewal Date"
            // as the date that the policy will go through that renewal from current to next.
            ActionsCancelPolicy.UpdateLastDayOfCoverDate(_browser, testData.OriginalPolicyData.StatusRenewalDate);

            ActionsCancelPolicy.SubmitCancellationRequest(_browser);
            ActionsCancelPolicy.CheckConfirmationModalAndThenConfirmCancellation(_browser,
                "home insurance", testData.PolicyNumber, testData.OriginalPolicyData.StatusRenewalDate);

            ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber);

            VerifyCancelPolicy.VerifyHomePolicyCancellationConfirmationPage(_browser, testData.PolicyNumber,
                testData.ActivePolicyHolder.FirstName, testData.OriginalPolicyData.HomeAsset.Address);

            VerifyCancelPolicy.VerifyShieldUpdated(
                ShieldPolicyDB.FetchCancellationDetailsForPolicy(testData.PolicyNumber, testData.ActivePolicyHolder.Id),
                testData.OriginalPolicyData.StatusRenewalDate, Reason.HouseDemolished.ExternalCode, testData.ActivePolicyHolder.GetEmail(), true);
        }

        /// <summary>
        /// Test emulates a member wishing to cancel their boat policy during
        /// the renewal period and allows then to back date the last day of
        /// cover by 14 days.
        /// 1. Launch policy cancellation test page
        /// 2. Select cancellation reason as "I'm experiencing financial hardship". Check for additional information.
        /// 3. Back date cancellation to 14 days ago
        /// 4. Provide email address
        /// 5. Change cancellation reason to "I'm not happy with a claims experience"
        /// 6. Click [Cancel policy] button. Verify details in modal dialog
        /// 7. Choose the [back] button on modal dialog
        /// 8. Click [Cancel policy] button again, this time clicking the [Confirm cancellation] button
        /// 9. Check details on policy cancellation confirmation page
        /// 10. Verify policy has been updated in Shield 
        /// </summary>
        [Test(Description = "Cancel Boat Policy Via Spark Test Launch Page"),
             Category(TestCategory.Regression), Category(TestCategory.Spark), Category(TestCategory.PolicyCancellation),
             Category(TestCategory.Endorsement), Category(TestCategory.Boat), Category(TestCategory.InsuranceContactService)]
        public void INSU_T104_Policy_Cancellations_Boat_In_Renewal_PastDate_ReasonChangedFromFinancialHardshipToBadClaimsExperience()
        {
            ShieldParametersDB.RequireShieldFeatureToggleForRefundIsTrue();
            var testData = BuildTestDateForCancelBoatPolicy();
            Reporting.LogTestStart();

            LaunchPage.OpenCancelPolicyURL(_browser, testData);

            VerifyCancelPolicy.VerifyCancelBoatPolicyPage(_browser, testData, DateTime.Now.Date);

            ActionsCancelPolicy.UpdateCancellationForFinancialHardshipAndCheckExtraInformationDisplayed(_browser);

            DateTime backDatedCancellation = DateTime.Now.AddDays(-14).Date;
            ActionsCancelPolicy.UpdateLastDayOfCoverDate(_browser, backDatedCancellation);

            ActionsCancelPolicy.CompleteCancellationDetails(_browser, testData.ActivePolicyHolder,
                Reason.BadClaimsExperience.Text);
            VerifyCancelPolicy.ValidateExtraInformationFinancialHardshipRemoved(_browser);

            ActionsCancelPolicy.SubmitCancellationRequest(_browser);
            ActionsCancelPolicy.CheckConfirmationModalAndThenRejectCancellation(_browser, "boat insurance", testData.PolicyNumber, backDatedCancellation);

            ActionsCancelPolicy.SubmitCancellationRequest(_browser);
            ActionsCancelPolicy.CheckConfirmationModalAndThenConfirmCancellation(_browser, "boat insurance", testData.PolicyNumber, backDatedCancellation);

            ActionMFA.RequestAndEnterOTP(_browser, testData.ActivePolicyHolder.MobilePhoneNumber);                        

            VerifyCancelPolicy.VerifyBadClaimExperiencePolicyCancellationConfirmationPage(_browser, testData.PolicyNumber, testData.ActivePolicyHolder.FirstName);
            VerifyCancelPolicy.VerifyBoatPolicyInformationDisplayed(_browser, testData, true);

            VerifyCancelPolicy.VerifyShieldUpdated(
                ShieldPolicyDB.FetchCancellationDetailsForPolicy(testData.PolicyNumber, testData.ActivePolicyHolder.Id),
                backDatedCancellation, Reason.BadClaimsExperience.ExternalCode, testData.ActivePolicyHolder.GetEmail(), true);
        }
        #endregion

        #region Test Case Helper Methods
        private EndorseBoat BuildTestDateForCancelBoatPolicy()
        {
            var testData = ShieldBoatDB.FetchBoatPolicyForCancellation();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            return testData;
        }

        private EndorseCar BuildTestDataForCancelMotorPolicy()
        {
            var testData = ShieldMotorDB.FindMotorPolicyNotInRenewal(false);
            testData.OriginalPolicyData = DataHelper.GetPolicyDetails(testData.PolicyNumber);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            return testData;
        }

        private EndorseCar BuildTestDataForCancelMotorFirst28DaysWithRoadsidePolicy()
        {
            var testData = ShieldMotorDB.FindMotorPolicyNotInRenewal(true);
            testData.OriginalPolicyData = DataHelper.GetPolicyDetails(testData.PolicyNumber);
            MemberCentral.SetMockContactRoadsideAssistanceLevel(testData.ActivePolicyHolder.Id, Constants.MemberCentraL.Roadside.Classic);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            return testData;
        }

        private EndorseHome BuildTestDataForCancelHomePolicyInRenewal()
        {
            var policyNumber = ShieldHomeDB.FindPolicyForCancellation();

            var testData = new HomeEndorsementBuilder().InitialiseHomeWithDefaultData(policyNumber).Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            return testData;
        }
        #endregion
    }
}
