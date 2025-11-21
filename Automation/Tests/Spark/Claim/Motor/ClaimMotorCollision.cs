using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System;
using System.Collections.Generic;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using Tests.ActionsAndValidations.Claims;
using UIDriver.Pages;
using Rac.TestAutomation.Common.TestData.Claim;
using Tests.ActionsAndValidations;
using Rac.TestAutomation.Common.API;
using static Rac.TestAutomation.Common.AzureStorage.AzureTableOperation;


namespace Spark.Claim.Motor
{
    [Property("Functional", "Single Motor Collision Claim")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class ClaimMotorCollision : BaseUITest
    {
        private List<MotorPolicyEntity> _fullCoverMotorPoliciesForClaims;
        private List<MotorPolicyEntity> _thirdPartyCoverMotorPoliciesForClaims;
        private List<MotorPolicyEntity> _evMotorPoliciesForClaims;

        private readonly string _azureTableName = Config.Get().Shield.Environment + DataType.Policy + DataPurpose.ForClaim + ProductType.Motor;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Single Motor Collision Claim");
            _fullCoverMotorPoliciesForClaims = DataHelper.AzureTableGetAllRecords(_azureTableName).FindAll(x => (x.CoverType == "MFCO") && (!x.IsEV) && (x.IsRegistrationValid)).PickRandom(25);
            _thirdPartyCoverMotorPoliciesForClaims = DataHelper.AzureTableGetAllRecords(_azureTableName).FindAll(x => (x.CoverType != "MFCO") && (!x.IsEV) && (x.IsRegistrationValid)).PickRandom(10);
            _evMotorPoliciesForClaims = DataHelper.AzureTableGetAllRecords(_azureTableName).FindAll(x => (x.CoverType == "MFCO") && (x.IsEV) && (x.IsRegistrationValid)).PickRandom(5);
        }

        #region Test Cases
        /// <summary>        
        /// Claim Scenario:     Accident With Your Own Property
        /// Travel Direction:   Reversing     
        /// Towed:              Yes, Repairer
        /// Drivable:           Yes
        /// TP Details:         No
        /// Repairer:           Repairer option available
        /// </summary>       
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Single vehicle motor collision claim accident with wildlife while driving forward and car is not drivable and towed to repairer")]
        public void INSU_T240_SingleVehicleCollision_AccidentWithYourOwnProperty_Reversing_Drivable_GetYourOwnQuote()
        {
            var claim = BuildRandomTestDataSingleVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.AccidentWithYourOwnProperty,
                TravelDirection.Reversing, isDrivable: true, hasTPDetails: false, repairerOption: RepairerOption.GetQuote);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenMotorTriageClaimURL(_browser, claim);
            ActionsClaimMotor.TriageMotorClaim(_browser, claim.DamageType);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>       
        /// Claim Scenario:     Accident With Wildlife
        /// Travel Direction:   Forward
        /// Towed:              Yes
        /// Drivable:           No
        /// TP Details:         No
        /// Repairer:           No
        /// </summary>        
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Single vehicle motor collision claim accident with wildlife while driving forward and car is not drivable and towed")]
        public void INSU_T241_SingleVehicleCollision_AccidentWithWildlife_Forward_NotDrivable_TowedToRepairer()
        {
            var claim = BuildRandomTestDataSingleVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.AccidentWithWildlife,
                TravelDirection.Forward, isDrivable: false, hasTPDetails: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>
        /// Detail UI Checking: False     
        /// Claim Scenario:     Accident With Someone Elses Property
        /// Travel Direction:   Reversing     
        /// Towed:              No
        /// Drivable:           Yes
        /// TP Details:         No
        /// Repairer:           Not Available
        /// This test case is not automated as we can't trigger no repairer option without shield intervention
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim accident with something else while reversing and car is drivable and no repairers are available")]
        public void INSU_T349_SingleVehicleCollision_AccidentWithSomeoneElsesProperty_Reversing_Drivable_NoTP_NoReapirers()
        {
            var claim = BuildRandomTestDataSingleVehicleCollision(_evMotorPoliciesForClaims, MotorClaimScenario.AccidentWithSomeoneElseProperty,
                TravelDirection.Reversing, isDrivable: true, hasTPDetails: false, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>       
        /// Claim Scenario:     Accident With Someone's Pet
        /// Travel Direction:   Forward      
        /// Towed:              Yes
        /// Drivable:           No
        /// TP Details:         Yes
        /// Repairer:           No
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Single vehicle motor collision claim accident with wildlife while driving forward and car is not drivable")]
        public void INSU_T239_SingleVehicleCollision_AccidentWithSomeonesPet_Forward_NotDrivable()
        {
            var claim = BuildRandomTestDataSingleVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.AccidentWithSomeonesPet,
               TravelDirection.Forward, isDrivable: false, hasTPDetails: true, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>        
        /// Claim Scenario:     Accident With Something Else
        /// Travel Direction:   Parked        
        /// Towed:              No
        /// Drivable:           Yes
        /// TP Details:         No
        /// Repairer:           Yes
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Single vehicle motor collision claim accident with wildlife while driving forward and car is drivable")]
        public void INSU_T238_SingleVehicleCollision_AccidentWithSomethingElse_Forward_Drivable_SelectRepairer()
        {
            var claim = BuildRandomTestDataSingleVehicleCollision(_evMotorPoliciesForClaims, MotorClaimScenario.AccidentWithSomethingElse,
             TravelDirection.Forward, isDrivable: true, hasTPDetails: false, isTPAssetDamage: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>        
        /// Claim Scenario:     Accident With Someone Else Property
        /// Travel Direction:   Reversing 
        /// Towed:              Yes
        /// Drivable:           No
        /// TP Details:         Yes
        /// Repairer:           No       
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Single vehicle motor collision claim accident with someone else property while reversing and car is not drivable and towed")]
        public void INSU_T237_SingleVehicleCollision_AccidentWithSomeoneElseProperty_Reversing_NotDrivable_TowedToUnknown()
        {
            var claim = BuildRandomTestDataSingleVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.AccidentWithSomeoneElseProperty,
             TravelDirection.Reversing, isDrivable: false, hasTPDetails: true, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>        
        /// Claim Scenario:     Accident With Someones Pet
        /// Travel Direction:   Reversing
        /// Towed:              No
        /// Drivable:           Yes
        /// TP Details:         Yes
        /// TP Only Claim:      Yes
        /// </summary>       
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Single vehicle motor collision claim accident with pet while reversing and car is not drivable and only claiming for TP asset")]
        public void INSU_T244_SingleVehicleCollision_AccidentWithSomeonesPet_Reversing_Drivable_TPOnlyClaim()
        {
            var claim = BuildRandomTestDataSingleVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.AccidentWithSomeonesPet,
            TravelDirection.Reversing, isDrivable: true, hasTPDetails: true, isTPAssetDamage: true, isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>        
        /// Claim Scenario:     Accident With Something Else
        /// Travel Direction:   Parked
        /// Towed:              No
        /// Drivable:           Yes
        /// TP Details:         No
        /// Repairer:           Select repairer
        /// </summary>       
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Single vehicle motor collision claim accident with pet while reversing and car is not drivable and only claiming for TP asset")]
        public void INSU_T230_SingleVehicleCollision_AccidentWithSomethingElse_Parked_Drivable_SelectRepairer()
        {
            var claim = BuildRandomTestDataSingleVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.AccidentWithSomethingElse,
            TravelDirection.Parked, isDrivable: true, hasTPDetails: false, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>        
        /// Claim Scenario:     Accident With Someone Else Property
        /// Travel Direction:   Forward
        /// Towed:              No
        /// Drivable:           Yes
        /// TP Details:         Yes
        /// Repairer:           Select repairer
        /// </summary>       
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Single vehicle motor collision claim accident with something else while driving forward and car is drivable and repairer is selected")]
        public void INSU_T229_SingleVehicleCollision_AccidentWithSomeoneElseProperty_Forward_Drivable_SelectRepairer()
        {
            var claim = BuildRandomTestDataSingleVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.AccidentWithSomeoneElseProperty,
            TravelDirection.Forward, isDrivable: true, hasTPDetails: true, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>        
        /// Claim Scenario:     Another Car Hit My Car
        /// Travel Direction:   Parked        
        /// Towed:              No
        /// Drivable:           Yes
        /// TP Details:         Yes
        /// Repairer:           Select repairer
        /// </summary>       
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while parked another car hit my car and car is drivable and repairer is selected")]
        public void INSU_T236_MultiVehicleCollision_FullCover_Parked_AnotherCarHitMyCar_Drivable_SelectRepairer()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileParkedAnotherCarHitMyCar,
                TravelDirection.Parked, isDrivable: true, hasTPDetails: true, isTPOnlyClaim: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>        
        /// Claim Scenario:     My Car Hit Rear Of Another Car
        /// Travel Direction:   Forward        
        /// Towed:              No
        /// Drivable:           Yes
        /// TP Details:         Yes
        /// Repairer:           No repairer options available
        /// This test case is not automated as we can't trigger no repairer option without shield intervention
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit rear of another car and car is drivable and repairer option is not available")]
        public void INSU_T348_MultipleVehicleCollision_FullCover_DrivingForward_MyCarHitRearOfAnotherCar_Drivable_NoRepairerOptionsAvailable()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingMyCarHitRearOfAnotherCar,
                TravelDirection.Forward, isDrivable: true, hasTPDetails: true, isTPOnlyClaim: false, repairerAllocationExhausted: true );
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>        
        /// Claim Scenario:     My Car Hit A Parked Car
        /// Travel Direction:   Forward        
        /// Towed:              Yes
        /// Drivable:           No
        /// TP Details:         No        
        /// </summary>       
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another parked car and car is not drivable and Towed")]
        public void INSU_T235_MultipleVehicleCollision_FullCover_DrivingForward_MyCarHitAParkedCar_NotDrivable_Towed()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingMyCarHitAParkedCar,
                TravelDirection.Forward, isDrivable: false, hasTPDetails: false, isTPOnlyClaim: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenMotorTriageClaimURL(_browser, claim);
            ActionsClaimMotor.TriageMotorClaim(_browser, claim.DamageType);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>        
        /// Claim Scenario:     My Car Hit A Parked Car
        /// Travel Direction:   Reversing
        /// Towed:              Yes
        /// Drivable:           No
        /// TP Details:         Yes
        /// TP Only Claim:      Yes
        /// </summary>       
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another parked car and car is not drivable and Towed")]
        public void INSU_T234_MultipleVehicleCollision_TPCover_Reversing_HitParkedCar_TPOnlyClaim()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_thirdPartyCoverMotorPoliciesForClaims, MotorClaimScenario.WhileReversingHitParkedCar,
                TravelDirection.Reversing, isDrivable: false, hasTPDetails: true, isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>        
        /// Claim Scenario:     My car hit another car while car while changing lanes
        /// Travel Direction:   Forward
        /// Towed:              No
        /// Drivable:           Yes
        /// TP Details:         Yes
        /// TP Only Claim:      Yes
        /// </summary>       
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another car while changing lanes and third party only claim")]
        public void INSU_T233_MultipleVehicleCollision_FullCover_DrivingForward_MyCarHitAnotherCarWhenChangingLanes_Drivable_TPOnlyClaim()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingMyCarHitAnotherCarWhenChangingLanes,
                TravelDirection.Forward, isDrivable: true, hasTPDetails: true, isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>        
        /// Claim Scenario:     Another car hit my car
        /// Travel Direction:   Stationary
        /// Towed:              Yes
        /// Drivable:           No
        /// TP Details:         Yes
        /// TP Only Claim:      No
        /// </summary>       
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while reversing hit another car and car is not drivable and towed")]
        public void INSU_T232_MultipleVehicleCollision_FullCover_Stationary_AnotherCarHitRearOfMyCar_NotDrivable_Towed()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileStationaryAnotherCarHitRearOfMyCar,
                TravelDirection.Stationary, isDrivable: false, hasTPDetails: true, isTPOnlyClaim: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>        
        /// Claim Scenario:     Another car hit my car when failed to give way
        /// Travel Direction:   Forward
        /// Towed:              No
        /// Drivable:           Yes
        /// TP Details:         No
        /// TP Only Claim:      No
        /// </summary>       
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision), Category(TestCategory.Regression)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward another car hit my car when failed to give way and car is drivable and repairer is selected")]
        public void INSU_T231_MultipleVehicleCollision_FullCover_DrivingForward_AnotherVehicleHitMyCarWhenFailToGiveWay_Drivable_SelectRepairer()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingOtherVehicleFailToGiveWayAndHitMyCar,
                TravelDirection.Forward, isDrivable: true, hasTPDetails: false, isTPOnlyClaim: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }
        #endregion

        #region Test cases helper methods
        private class ClaimPolicy
        {
            public MotorPolicy motorPolicy { get; set; }
            public PolicyContactDB claimant { get; set; }
            public Contact driver { get; set; }
            public List<PolicyDetail> linkedMotorPolicies { get; set; }
        }

        private ClaimCar BuildRandomTestDataSingleVehicleCollision(List<MotorPolicyEntity> motorPolicies, MotorClaimScenario motorClaimScenario, TravelDirection travelDirection,
            bool isDrivable, bool hasTPDetails, bool isTPAssetDamage = false, bool isTPOnlyClaim = false, bool repairerAllocationExhausted = false, RepairerOption repairerOption = RepairerOption.None)
        {
            ClaimPolicy motorClaimPolicy;
            var driverRole = DataHelper.GetRandomEnum<DriverRole>();

            motorClaimPolicy = GetEligiblePolicy(motorPolicies, driverRole);
            var claimCar = new MotorClaimBuilder().InitialiseMotorClaimWithBasicData(motorClaimPolicy.motorPolicy, MotorClaimDamageType.SingleVehicleCollision)
                                                        .WithRiskAddressAndHireCarCover(motorClaimPolicy.motorPolicy.PolicyNumber)
                                                        .LoginWith(DataHelper.GetRandomEnum<LoginWith>())
                                                        .WithClaimant(motorClaimPolicy.claimant, DataHelper.RandomBoolean(), DataHelper.RandomBoolean())
                                                        .WithRandomEventDateInLast7Days(roundDown15Mins: true)
                                                        .WithNumberOfVehiclesInvolved(numberOfVehiclesInvolved: MotorCollisionNumberOfVehiclesInvolved.NoOtherVehiclesInvolved)
                                                        .IsTPAssestDamaged(isTPAssetDamage)
                                                        .OnlyClaimDamageToTPInClaim(isTPOnlyClaim)
                                                        .WithClaimScenario(motorClaimScenario)
                                                        .WithTravelDirection(travelDirection)
                                                        .WithRandomEventLocation()
                                                        .WithRandomAccountOfAccident()
                                                        .WithDriver(motorClaimPolicy.driver, wasDrunk: false, wasSuspended: false, isLicensedMoreThan2Years: true)
                                                        .WithNoThirdParty()
                                                        .WithRandomPoliceDetails()
                                                        .WithRandomDescriptionOfDamageToPHVehicle()
                                                        .WithLinkedMotorPoliciesForClaimant(motorClaimPolicy.linkedMotorPolicies);

            if (!isDrivable)
            {
                var towedTo = DataHelper.GetRandomEnum<MotorClaimTowedTo>(1);

                if (towedTo == MotorClaimTowedTo.HomeAddress &&
                    motorClaimPolicy.motorPolicy.RiskAddress.StreetOrPOBox == null)
                {
                    towedTo = MotorClaimTowedTo.HoldingYard;
                }

                claimCar.WithVehicleTowedDetails(true, towedTo)
                    .WithIsVehicleDriveable(false)
                    .WithRepairerOption(RepairerOption.None);
            }
            else
            {
                claimCar.WithVehicleTowedDetails(false, MotorClaimTowedTo.None)
                   .WithIsVehicleDriveable(true)
                   .WithRepairerOption(DataHelper.GetRandomEnum<RepairerOption>(1))
                   .WithRandomPreferredRepairerLocation();

                if (repairerOption == RepairerOption.GetQuote)
                {
                    claimCar.WithRepairerOption(RepairerOption.GetQuote);
                }
                else
                {
                    claimCar.WithRepairerOption(DataHelper.GetRandomEnum<RepairerOption>(1));
                }
            }

            //Add random witness between 1 to 3
            if (DataHelper.RandomBoolean())
            {
                claimCar.AddRandomWitness(DataHelper.RandomNumber(1, 3));
            }

            if (hasTPDetails)
            {
                claimCar.AddRandomThirdParty(isKnownToClaimant: DataHelper.RandomBoolean(), thirdPartyInsurer: DataHelper.GetInsuranceCompany().PickRandom(), wasDriverTheOwner: DataHelper.RandomBoolean());
            }

            if (repairerAllocationExhausted)
            {
                //For Deepdene suburb, currently there is not any repairer available
                //This will trigger the repairer not available scenario
                Address address = new Address
                {
                    Suburb = "Deepdene",
                    PostCode = "6290"
                };
                claimCar.WithPreferredRepairerLocation(address)
                    .WithRepairerAllocationExhausted(true); ;
            }


            motorPolicies.RemoveAll(x => x.PolicyNumber == motorClaimPolicy.motorPolicy.PolicyNumber);
            DataHelper.AzureTableDeleteEntityBasedOnPolicyNumber(motorClaimPolicy.motorPolicy.PolicyNumber, _azureTableName);
            return claimCar.Build();
        }


        private ClaimCar BuildRandomTestDataMultiVehicleCollision(List<MotorPolicyEntity> motorPolicies, MotorClaimScenario motorClaimScenario, TravelDirection travelDirection,
            bool isDrivable, bool hasTPDetails, bool isTPOnlyClaim = false, bool repairerAllocationExhausted = false, RepairerOption repairerOption = RepairerOption.None)
        {
            ClaimPolicy motorClaimPolicy;
            var driverRole = DataHelper.GetRandomEnum<DriverRole>();

            motorClaimPolicy = GetEligiblePolicy(motorPolicies, driverRole);
            var claimCar = new MotorClaimBuilder().InitialiseMotorClaimWithBasicData(motorClaimPolicy.motorPolicy, MotorClaimDamageType.MultipleVehicleCollision)
                                                        .WithRiskAddressAndHireCarCover(motorClaimPolicy.motorPolicy.PolicyNumber)
                                                        .LoginWith(DataHelper.GetRandomEnum<LoginWith>())
                                                        .WithClaimant(motorClaimPolicy.claimant, DataHelper.RandomBoolean(), DataHelper.RandomBoolean())
                                                        .WithRandomEventDateInLast7Days(roundDown15Mins: true)
                                                        .WithNumberOfVehiclesInvolved(numberOfVehiclesInvolved: MotorCollisionNumberOfVehiclesInvolved.OneOtherVehicle)
                                                        .IsTPAssestDamaged(true)
                                                        .OnlyClaimDamageToTPInClaim(isTPOnlyClaim)
                                                        .WithClaimScenario(motorClaimScenario)
                                                        .WithTravelDirection(travelDirection)
                                                        .WithRandomEventLocation()
                                                        .WithRandomAccountOfAccident()
                                                        .WithDriver(motorClaimPolicy.driver, wasDrunk: false, wasSuspended: false, isLicensedMoreThan2Years: true)
                                                        .WithNoThirdParty()
                                                        .WithRandomPoliceDetails()
                                                        .WithRandomDescriptionOfDamageToPHVehicle()
                                                        .WithLinkedMotorPoliciesForClaimant(motorClaimPolicy.linkedMotorPolicies);

            if (!isDrivable)
            {
                var towedTo = DataHelper.GetRandomEnum<MotorClaimTowedTo>(1);

                if (towedTo == MotorClaimTowedTo.HomeAddress &&
                    motorClaimPolicy.motorPolicy.RiskAddress.StreetOrPOBox == null)
                {
                    towedTo = MotorClaimTowedTo.HoldingYard;
                }

                claimCar.WithVehicleTowedDetails(true, towedTo)
                    .WithIsVehicleDriveable(false)
                    .WithRepairerOption(RepairerOption.None);
            }
            else
            {
                claimCar.WithVehicleTowedDetails(false, MotorClaimTowedTo.None)
                   .WithIsVehicleDriveable(true)
                   .WithRandomPreferredRepairerLocation();

                if (repairerOption == RepairerOption.GetQuote)
                {
                    claimCar.WithRepairerOption(RepairerOption.GetQuote);
                }
                else
                {
                    claimCar.WithRepairerOption(DataHelper.GetRandomEnum<RepairerOption>(1));
                }
            }

            //Add random witness between 1 to 3
            if (DataHelper.RandomBoolean())
            {
                claimCar.AddRandomWitness(DataHelper.RandomNumber(1, 3));
            }

            if (hasTPDetails)
            {
                claimCar.AddRandomThirdParty(isKnownToClaimant: DataHelper.RandomBoolean(), thirdPartyInsurer: DataHelper.GetInsuranceCompany().PickRandom(), wasDriverTheOwner: DataHelper.RandomBoolean());
            }

            if (repairerAllocationExhausted)
            {
                //For Deepdene suburb, currently there is not any repairer available
                //This will trigger the repairer not available scenario
                Address address = new Address
                {
                    Suburb = "Deepdene",
                    PostCode = "6290"
                };
                claimCar.WithPreferredRepairerLocation(address)
                    .WithRepairerAllocationExhausted(true);                
            }

            motorPolicies.RemoveAll(x => x.PolicyNumber == motorClaimPolicy.motorPolicy.PolicyNumber);
            DataHelper.AzureTableDeleteEntityBasedOnPolicyNumber(motorClaimPolicy.motorPolicy.PolicyNumber, _azureTableName);
            return claimCar.Build();
        }


        /// <summary>
        /// Get an eligible motor policy based on the Contact role,
        /// Driver role and if required multiple motor policy linked
        /// </summary>       
        /// <param name="driverRole">The Driver role i.e. Policy Holder/ Co Policy Holder/ Additional driver/ New Contact</param>       
        /// <returns>Return an eligible motor policy for claim</returns>
        /// <exception cref="NotFoundException"></exception>
        private ClaimPolicy GetEligiblePolicy(List<MotorPolicyEntity> motorPolicyEntities, DriverRole driverRole, MotorClaimTowedTo towedTo = MotorClaimTowedTo.None)
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
                    if (towedTo != MotorClaimTowedTo.HomeAddress ||
                        towedTo == MotorClaimTowedTo.HomeAddress && policy.RiskAddress.StreetOrPOBox != null)
                    {
                        var linkedMotorPolicies = DataHelper.GetAllPoliciesOfTypeForPolicyHolder(claimant.Id, "Motor");

                        claimPolicy.motorPolicy = policy;
                        claimPolicy.claimant    = claimant;
                        claimPolicy.driver      = driver;
                        claimPolicy.linkedMotorPolicies = linkedMotorPolicies;
                        break;
                    }

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
