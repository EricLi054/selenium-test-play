using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using Rac.TestAutomation.Common.TestData.Claim;
using Tests.ActionsAndValidations;
using System;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.General;
using System.Linq;


namespace B2C.Claims
{
    [Property("Functional", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class ClaimsMotorAnonymousTheft : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C mandatory regression tests for verifying criteria around lodging a motor claim via the B2C anonymous flow.");            
        }

        /// <summary>
        /// "B2C Anonymous - Motor Claim - MemberMatch - Theft of Vehicle"
        /// Lodgement of a online motor claim, where the policy is found by
        /// asset and PH/coPH details, rather than policy number.
        /// Claim scenario is theft of vehicle.
        /// Event date is set to occur in previous endorsement period
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Claim), Category(TestCategory.Motor),
            Category(TestCategory.Mock_Member_Central_Support), Category(TestCategory.B2CPCM)]
        public void INSU_T33_B2CAnon_SuccessfulLodgementOfMotorClaimMemberMatchVehicleTheft()
        {
            var claim = BuildTestDataForB2CAnonMotorClaimTheftOfVehicle();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            _browser.LaunchPageBeginNewAnonymousMotorClaim();
            ActionsClaimMotor.TriageMotorClaim(_browser, claim.DamageType);
            ActionsClaimMotor.MotorClaimCompletePrelimDetails(_browser, claim, false);
            ActionsClaimMotor.MotorClaimCompleteCommonQuestionnaires(_browser, claim);

            Reporting.Log("Verify claim confirmation details on final B2C claims page.");
            var claimNumber = VerifyClaimMotor.VerifyClaimConfirmationScreen(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claimNumber);
            VerifyClaimMotor.VerifyMotorClaimInshield( claimNumber, claim);
            VerifyClaimMotor.VerifyClaimsQuestionnairesInShield(claimNumber, claim);
        }


        private ClaimCar BuildTestDataForB2CAnonMotorClaimTheftOfVehicle()
        {
            var policiesToUse = ShieldMotorClaimDB.FindMotorPolicyWithMFCOInPrevEndorsementAnyPH();
            ClaimCar claimCar = null;

            foreach (var policyToUse in policiesToUse) 
            {
                var claimant = policyToUse.PolicyHolders.PickRandom();
                claimant = DataHelper.MapPolicyContactWithPersonAPI(claimant.Id, claimant.ExternalContactNumber, claimant.ContactRoles.FirstOrDefault());

                if (claimant != null)
                {
                    // Setup event date and time.
                    // - If the last endorsement is in the past, start there.
                    // - If not, start with today.
                    // - Then we want to rewind 3 days, and then set the day to the 1st of whatever month that is.
                    // The SQL gathering data already guaranteed that the PRIOR endorsement was over 
                    // 1 month ago and we take advantage of a NPE rule that sidesteps any pre-existing
                    // claim prompts when the Event Date is the 1st day of the month.
                    var dateToUse = policyToUse.LastEndorsementDate.Date <= DateTime.Now.Date ?
                    policyToUse.LastEndorsementDate.Date :
                    DateTime.Now.Date;
                    var eventDateAndTime = dateToUse.AddDays(-3).AddHours(15).AddMinutes(15);
                    eventDateAndTime = eventDateAndTime.AddDays(-eventDateAndTime.Day + 1);

                    claimCar = new MotorClaimBuilder().InitialiseMotorClaimWithBasicData(policyToUse, MotorClaimDamageType.Theft)
                                                       .WithClaimant(claimant, excludedFromContactMFA: true)
                                                       .WithTheftDetails(MotorClaimTheftDetails.VehicleUnrecovered)
                                                       .AddRandomWitness()
                                                       .AddRandomThirdParty()
                                                       .WithEventDateAndTime(eventDateAndTime)
                                                       .Build();
                }
            }

            Reporting.IsNotNull(claimCar, "suitable test data has been found");
            return claimCar;            

        }
    }
}
