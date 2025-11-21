using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Contacts;
using Tests.ActionsAndValidations;

using static Rac.TestAutomation.Common.Constants.General;

namespace B2C.MemberServices
{
    [Property("Functional", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class UpdateContact : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C/PCM mandatory regression tests for updating preferred delivery method for policy documents.");
        }

        /// <summary>
        /// Mandatory regression test case B2C-T434.
        /// Previously was "B2C Logged In - Change My Details - TC04 - Update Contact"
        /// Member is able to update their preferred delivery method for policy documents
        /// via PCM. Other contact details are now updated via myRAC only.
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Endorsement),
            Category(TestCategory.Mock_Member_Central_Support), Category(TestCategory.B2CPCM)]
        public void INSU_T40_B2CLoggedIn_ChangeMailingPreferences()
        {
            var testData = BuildTestDataForMailingPreferences();
            Reporting.LogMinorSectionHeading("Change Member Mailing Preferences via PCM.");

            ActionsUpdateMailingPreferences.PerformUpdateToMailingPreferences(_browser, testData);

            ActionsUpdateMailingPreferences.VerificationOfConfirmationMessage(_browser);

            Reporting.LogMinorSectionHeading("Verify updated Contact details in Shield database.");
            VerifyContactDetails.VerifyUpdatedContactDetailsFromShield(testData);
        }

        private ChangeContactDetails BuildTestDataForMailingPreferences()
        {
            var testData = ShieldContactDetailsDB.FindContactForChangeMailingPreferences();


            return testData;
        }
    }
}
