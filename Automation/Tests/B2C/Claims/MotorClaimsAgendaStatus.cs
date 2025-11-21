using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Claim;
using Tests.ActionsAndValidations;
using UIDriver.Pages.PCM;
using static Rac.TestAutomation.Common.Constants.General;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using System.Collections.Generic;
using static Rac.TestAutomation.Common.AzureStorage.AzureTableOperation;
using Rac.TestAutomation.Common.API;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.Contacts;
using System.Linq;
using Tests.ActionsAndValidations.Claims;
using UIDriver.Pages;
using System;

namespace B2C.Claims
{
    [Property("Functional", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class MotorClaimsAgendaStatus : BaseUITest
    {
        private List<MotorPolicyEntity> _motorPoliciesForClaims;
        private readonly string _azureTableName = Config.Get().Shield.Environment + DataType.Policy + DataPurpose.ForClaim + ProductType.Motor;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C mandatory regression tests for verifying claim status details in PCM as a claim progresses through its lifecycle.");
            _motorPoliciesForClaims = DataHelper.AzureTableGetAllRecords(_azureTableName).FindAll(x => (x.CoverType == "MFCO") && (!x.IsEV) && (x.IsRegistrationValid)).PickRandom(5);
        }

        /// <summary>
        /// Mandatory regression test case:
        /// "B2C Logged In - Claim Status - Motor - AutoLodgement - True"
        /// Covers the scenario of a B2C lodged single vehicle motor claim, and
        /// tracking Claim Status in PCM as the claim is updated through its life
        /// cycle via Shield.
        /// Claim will follow one-step logic due to damage type (should auto-lodge
        /// into ASSESS state).
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Claim), Category(TestCategory.Motor),
            Category(TestCategory.Mock_Member_Central_Support),Category(TestCategory.B2CPCM)]
        public void INSU_T35_B2CLoggedIn_ClaimStatus_Motor()
        {
            var claim = BuildTestDataSingleVehicleCollision(MotorClaimScenario.AccidentWithYourOwnProperty, TravelDirection.Forward, DriverRole.PolicyHolder,
                isDrivable: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenMotorTriageClaimURL(_browser, claim);
            ActionsClaimMotor.TriageMotorClaim(_browser, claim.DamageType);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPage(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claim.ClaimNumber);
            // Single vehicle is a one-step claim scenario, so expect to be in Assess state.
            VerifyClaimMotor.VerifyClaimStatusInPCM(_browser, claim.ClaimNumber, claim.Claimant.Id, PortfolioSummary.CLAIM_PROGRESS_STATE.Assess);

            ActionsClaimMotor.MotorClaimShieldAuthoriseQuote(_browser, claim.ClaimNumber, claim.DamageType);
            VerifyClaimMotor.VerifyClaimStatusInPCM(_browser, claim.ClaimNumber, claim.Claimant.Id, PortfolioSummary.CLAIM_PROGRESS_STATE.Repair);

            ActionsClaimMotor.MotorClaimShieldAcceptRepairerInvoice(_browser, claim.ClaimNumber);
            VerifyClaimMotor.VerifyClaimStatusInPCM(_browser, claim.ClaimNumber, claim.Claimant.Id, PortfolioSummary.CLAIM_PROGRESS_STATE.Complete);
        }

        /// <summary>
        /// Mandatory regression test case:
        /// "B2C Logged In - Claim Status - Motor - AutoLodgement - False"
        /// Covers the scenario of a B2C lodged multi-vehicle motor claim under a
        /// merging lane situation, and tracking Claim Status in PCM as the claim
        /// is updated through its life cycle via Shield.
        /// Claim will follow basic claim logic due to damage type and scenario to
        /// require a claims specialist to review.
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Claim), Category(TestCategory.Motor),
            Category(TestCategory.Mock_Member_Central_Support), Category(TestCategory.B2CPCM)]
        public void INSU_T24_B2CLoggedIn_ClaimStatus_Motor()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileDrivingMyCarHitAnotherCarWhenChangingLanes, TravelDirection.Forward,
                DriverRole.PolicyHolder, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPage(_browser, claim);

            Reporting.LogTestShieldValidations("claim", claim.ClaimNumber);
            VerifyClaimMotor.VerifyClaimStatusInPCM(_browser, claim.ClaimNumber, claim.Claimant.Id, PortfolioSummary.CLAIM_PROGRESS_STATE.Lodge);

            ActionsClaimMotor.MotorClaimShieldAssignClaimsHandler(_browser, claim.ClaimNumber);
            VerifyClaimMotor.VerifyClaimStatusInPCM(_browser, claim.ClaimNumber, claim.Claimant.Id, PortfolioSummary.CLAIM_PROGRESS_STATE.Assess);

            ActionsClaimMotor.MotorClaimShieldAuthoriseQuote(_browser, claim.ClaimNumber, claim.DamageType);
            VerifyClaimMotor.VerifyClaimStatusInPCM(_browser, claim.ClaimNumber, claim.Claimant.Id, PortfolioSummary.CLAIM_PROGRESS_STATE.Repair);

            ActionsClaimMotor.MotorClaimShieldAcceptRepairerInvoice(_browser, claim.ClaimNumber);
            VerifyClaimMotor.VerifyClaimStatusInPCM(_browser, claim.ClaimNumber, claim.Claimant.Id, PortfolioSummary.CLAIM_PROGRESS_STATE.Complete);
        }

        #region Test cases helper methods        
        private class ClaimPolicy
        {
            public MotorPolicy motorPolicy { get; set; }
            public PolicyContactDB claimant { get; set; }
            public Contact driver { get; set; }
            public List<PolicyDetail> linkedMotorPolicies { get; set; }
        }

        private ClaimCar BuildTestDataSingleVehicleCollision(MotorClaimScenario motorClaimScenario, TravelDirection travelDirection, DriverRole driverRole,
            bool? isDrivable)
        {
            // Choosing repairer location to avoid getting auto-approved service provider.
            var preferredRepairLocation = new Address()
            {
                StreetNumber = "9",
                StreetOrPOBox = "Woodard Avenue",
                Suburb = "Margaret River",
                State = "WA",
                PostCode = "6285"
            };

            ClaimPolicy motorClaimPolicy = GetEligiblePolicy(_motorPoliciesForClaims, driverRole);
            var claimCar = new MotorClaimBuilder().InitialiseMotorClaimWithBasicData(motorClaimPolicy.motorPolicy, MotorClaimDamageType.SingleVehicleCollision)
                                                        .WithRiskAddressAndHireCarCover(motorClaimPolicy.motorPolicy.PolicyNumber)
                                                        .LoginWith(DataHelper.GetRandomEnum<LoginWith>())
                                                        .WithClaimant(contact: motorClaimPolicy.claimant, changeEmailAddress: false, changeMobileNumber: false)
                                                        .WithRandomEventDateInLast7Days(roundDown15Mins: true)
                                                        .WithNumberOfVehiclesInvolved(numberOfVehiclesInvolved: MotorCollisionNumberOfVehiclesInvolved.NoOtherVehiclesInvolved)
                                                        .WithClaimScenario(motorClaimScenario)
                                                        .OnlyClaimDamageToTPInClaim(false)
                                                        .WithTravelDirection(travelDirection)
                                                        .WithEventLocation(preferredRepairLocation.StreetSuburbState())
                                                        .WithRandomAccountOfAccident()
                                                        .WithDriver(motorClaimPolicy.driver, wasDrunk: false, wasSuspended: false, isLicensedMoreThan2Years: true)
                                                        .WithNoThirdParty()
                                                        .WithRandomPoliceDetails()
                                                        .WithRandomDescriptionOfDamageToPHVehicle()
                                                        .WithVehicleTowedDetails(false, MotorClaimTowedTo.None)
                                                        .WithIsVehicleDriveable(isDrivable)
                                                        .WithRepairerOption(RepairerOption.First)
                                                        .WithPreferredRepairerLocation(preferredRepairLocation)
                                                        .WithLinkedMotorPoliciesForClaimant(motorClaimPolicy.linkedMotorPolicies);

            _motorPoliciesForClaims.RemoveAll(x => x.PolicyNumber == motorClaimPolicy.motorPolicy.PolicyNumber);
            DataHelper.AzureTableDeleteEntityBasedOnPolicyNumber(motorClaimPolicy.motorPolicy.PolicyNumber, _azureTableName);
            return claimCar.Build();
        }

        private ClaimCar BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario motorClaimScenario, TravelDirection travelDirection, DriverRole driverRole,
            bool? isTPAssetDamage = false, bool isOnlyClaimDamageToTP = false)
        {
            var preferredRepairLocation = new Address()
            {
                Suburb = "Meekatharra",
                PostCode = "6642"
            };

            ClaimPolicy motorClaimPolicy = GetEligiblePolicy(_motorPoliciesForClaims, driverRole);
            var claimCar = new MotorClaimBuilder().InitialiseMotorClaimWithBasicData(motorClaimPolicy.motorPolicy, MotorClaimDamageType.MultipleVehicleCollision)
                                                        .WithRiskAddressAndHireCarCover(motorClaimPolicy.motorPolicy.PolicyNumber)
                                                        .LoginWith(DataHelper.GetRandomEnum<LoginWith>())
                                                        .WithClaimant(motorClaimPolicy.claimant, DataHelper.RandomBoolean(), DataHelper.RandomBoolean())
                                                        .WithRandomEventDateInLast7Days(roundDown15Mins: true)
                                                        .WithNumberOfVehiclesInvolved(numberOfVehiclesInvolved: MotorCollisionNumberOfVehiclesInvolved.OneOtherVehicle)
                                                        .WithClaimScenario(motorClaimScenario)
                                                        .IsTPAssestDamaged(isTPAssetDamage)
                                                        .OnlyClaimDamageToTPInClaim(isOnlyClaimDamageToTP)
                                                        .WithTravelDirection(travelDirection)
                                                        .WithRandomEventLocation()
                                                        .WithRandomAccountOfAccident()
                                                        .WithDriver(motorClaimPolicy.driver, wasDrunk: false, wasSuspended: false, isLicensedMoreThan2Years: true)
                                                        .WithNoThirdParty()
                                                        .WithRandomPoliceDetails()
                                                        .WithRandomDescriptionOfDamageToPHVehicle()
                                                        .WithVehicleTowedDetails(false, MotorClaimTowedTo.None)
                                                        .WithIsVehicleDriveable(true)
                                                        .WithRepairerOption(RepairerOption.First)
                                                        .WithPreferredRepairerLocation(preferredRepairLocation)
                                                        .WithLinkedMotorPoliciesForClaimant(motorClaimPolicy.linkedMotorPolicies);

            if (isOnlyClaimDamageToTP)
            {
                claimCar.AddRandomThirdParty(isKnownToClaimant: DataHelper.RandomBoolean(), thirdPartyInsurer: DataHelper.GetInsuranceCompany().PickRandom(), wasDriverTheOwner: DataHelper.RandomBoolean());
            }

            _motorPoliciesForClaims.RemoveAll(x => x.PolicyNumber == motorClaimPolicy.motorPolicy.PolicyNumber);
            DataHelper.AzureTableDeleteEntityBasedOnPolicyNumber(motorClaimPolicy.motorPolicy.PolicyNumber, _azureTableName);
            return claimCar.Build();
        }

        /// <summary>
        /// Get an eligible motor policy based on the Contact role,
        /// Driver role and if required multiple motor policy linked
        /// </summary>    
        /// <param name="driverRole">The Driver role i.e. Policy Holder/ Co Policy Holder/ Additional driver/ New Contact</param>        
        /// <returns>Return an eligible motor policy for claim</returns>    
        private ClaimPolicy GetEligiblePolicy(List<MotorPolicyEntity> motorPolicyEntities, DriverRole driverRole)
        {
            ClaimPolicy claimPolicy = new ClaimPolicy();
            foreach (var entity in motorPolicyEntities)
            {
                var policy = DataHelper.GetMotorPolicyDetailsFromEntity(entity, _azureTableName);
                if (policy == null) { continue; }

                var claimant = policy.PolicyHolders.PickRandom();
                claimant.UpdateEmailIfNotDefined();

                var driver = GetDriver(driverRole, policy);
                if (driver != null &&
                    ShieldClaimDB.GetOpenClaimCountForPolicy(policy.PolicyNumber) == 0)
                {
                    var linkedMotorPolicies = DataHelper.GetAllPoliciesOfTypeForPolicyHolder(claimant.Id, "Motor");

                        claimPolicy.motorPolicy = policy;
                        claimPolicy.claimant = claimant;
                        claimPolicy.driver = driver;
                        claimPolicy.linkedMotorPolicies = linkedMotorPolicies;
                        break;

                }
            }

            Reporting.IsNotNull(claimPolicy.motorPolicy, "suitable test data has been found");

            return claimPolicy;
        }

        /// <summary>
        /// Get the driver contact based on the policy and driver role
        /// PolicyHolder: It assign the main policy holder as a driver
        /// CoPolicyHolder: It assign the Co Policy Owner as a driver
        /// AdditionalDriver: It assign a contact who is an additional driver but not a policy holder
        /// NewContact: It assign a new contact as driver
        /// </summary>
        /// <param name="driverRole"></param>
        /// <param name="policy"></param>
        /// <returns>Returns a Contact object, if could not found a driver based on the driver role then it will return null</returns>
        /// <exception cref="NotSupportedException"></exception>
        private Contact GetDriver(DriverRole driverRole, MotorPolicy policy)
        {
            Contact driver = null;

            switch (driverRole)
            {
                case DriverRole.PolicyHolder:
                    driver = policy.PolicyHolders.Find(x => x.ContactRoles.Contains(ContactRole.PolicyHolder));
                    break;

                case DriverRole.CoPolicyHolder:
                    driver = policy.PolicyHolders.Find(x => x.ContactRoles.Contains(ContactRole.CoPolicyHolder));
                    break;

                case DriverRole.NewContact:
                    driver = new ContactBuilder().InitialiseRandomIndividual().Build();
                    break;

                default:
                    throw new NotSupportedException($"{driverRole.GetDescription()} is not a valid driver role");
            }

            return driver;
        }

        #endregion
    }
}
