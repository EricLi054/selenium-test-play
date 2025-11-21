using System;
using System.Linq;
using System.Collections.Generic;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.TestData.Claim;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.AzureStorage.AzureTableOperation;
using Tests.ActionsAndValidations.Claims;
using UIDriver.Pages;
using OpenQA.Selenium;
using NUnit.Framework;
using Tests.ActionsAndValidations;

namespace Spark.Claim.Motor
{
    [Property("Functional", "Multiple Motor Collision Claim")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class ClaimMultipleMotorCollision : BaseUITest
    {
        private List<MotorPolicyEntity> _fullCoverMotorPoliciesForClaims;
        private List<MotorPolicyEntity> _thirdPartyCoverMotorPoliciesForClaims;

        private readonly string _azureTableName = Config.Get().Shield.Environment + DataType.Policy + DataPurpose.ForClaim + ProductType.Motor;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Multiple Motor Collision Claim");

            _fullCoverMotorPoliciesForClaims = DataHelper.AzureTableGetAllRecords(_azureTableName).FindAll(x => (x.CoverType == "MFCO") && (!x.IsEV) && (x.IsRegistrationValid)).PickRandom(40);
            _thirdPartyCoverMotorPoliciesForClaims = DataHelper.AzureTableGetAllRecords(_azureTableName).FindAll(x => (x.CoverType != "MFCO") && (!x.IsEV) && (x.IsRegistrationValid)).PickRandom(18);
        }

        #region Test Cases
        /// <summary>     
        /// Claim Scenario: Another car hit my car
        /// Cover Type: Full Cover
        /// Travel Direction: Parked
        /// Drivable: true
        /// Towed: false
        /// Repairer: GetQuote
        /// TPAssetDamaged: false
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while parked another car hit my car")]
        public void MultipleVehicleCollision_FullCover_Parked_AnotherCarHitMyCar_Drivable_GetQuote()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileParkedAnotherCarHitMyCar, TravelDirection.Parked,
                 DriverRole.PolicyHolder, isDrivable: true, wasTowed: false, repairer: RepairerOption.GetQuote);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }


        /// <summary>     
        /// Claim Scenario: My car hit rear of another car
        /// Cover Type: Full Cover
        /// Travel Direction: Forward
        /// Driver: PolicyHolder
        /// Drivable: True
        /// Towed: False
        /// Repairer: First repairer
        /// TPAssetDamaged: Yes
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit rear of another car")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_MyCarHitRearOfAnotherCar_Drivable_SelectFirstRepairer()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileDrivingMyCarHitRearOfAnotherCar, TravelDirection.Forward,
                DriverRole.PolicyHolder, isDrivable: true, wasTowed: false, repairer: RepairerOption.First, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim, detailUiChecking: true);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim, detailUiChecking: true);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }


        /// <summary>     
        /// Claim Scenario: My car hit rear of another car
        /// Cover Type: Full Cover
        /// Travel Direction: Forward
        /// Driver: NewDriver
        /// Drivable: false
        /// Towed: False        
        /// TPAssetDamaged: Yes
        /// TOOnlyClaim: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit rear of another car")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_MyCarHitRearOfAnotherCar_Drivable_TPOnlyClaim()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileDrivingMyCarHitRearOfAnotherCar, TravelDirection.Forward,
                  DriverRole.NewContact, isDrivable: false, wasTowed: false, isTPAssetDamage: true, isOnlyClaimDamageToTP: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim, detailUiChecking: true);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: My car hit another parked car
        /// Cover Type: Full Cover
        /// Travel Direction: Forward
        /// Driver: Policy Holder
        /// Drivable: false
        /// Towed: true
        /// Towed To: Holding Yard
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another parked car")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_MyCarHitAParkedCar_NotDrivable_TowedToHoldingYard()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileDrivingMyCarHitAParkedCar, TravelDirection.Forward,
                DriverRole.PolicyHolder, isDrivable: false, wasTowed: true, towedTo: MotorClaimTowedTo.HoldingYard);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Hit a parked car
        /// Cover Type: Full Cover
        /// Travel Direction: Reversing
        /// Driver: Co Policy Owner
        /// Towed: Not sure
        /// Drivable: false
        /// TPAssetDamaged: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while reversing hit a parked car")]
        public void MultipleVehicleCollision_FullCover_Reversing_HitParkedCar_TowedNotSure()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileReversingHitParkedCar, TravelDirection.Reversing,
                DriverRole.CoPolicyHolder, isDrivable: false, wasTowed: null, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Hit a parked car
        /// Cover Type: Full Cover
        /// Travel Direction: Reversing  
        /// Driver: NewDriver
        /// Drivable: false
        /// Towed: false        
        /// TPAssetDamaged: true
        /// TOOnlyClaim: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while reversing hit a parked car")]
        public void MultipleVehicleCollision_FullCover_Reversing_HitParkedCar_TPOOnlyClaim()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileReversingHitParkedCar, TravelDirection.Reversing,
                DriverRole.NewContact, isDrivable: false, wasTowed: false, isTPAssetDamage: true, isOnlyClaimDamageToTP: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: My car hit another parked car
        /// Cover Type: Third Party Cover
        /// Travel Direction: Forward 
        /// Driver: Policy Holder
        /// Drivable: false
        /// Towed: false        
        /// TPAssetDamaged: true
        /// TPOnlyClaim: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another parked car")]
        public void MultipleVehicleCollision_TPCover_DrivingForward_MyCarHitAParkedCar_TPOnlyClaim()
        {
            var claim = BuildTestDataThirdPartyCoverMultiVehicleCollision(MotorClaimScenario.WhileDrivingMyCarHitAParkedCar, TravelDirection.Forward,
                DriverRole.PolicyHolder, isDrivable: false, wasTowed: false, isTPAssetDamage: true, isOnlyClaimDamageToTP: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim, detailUiChecking: true);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Another car hit rear of my car
        /// Cover Type: Full Cover
        /// Travel Direction: Forward   
        /// Driver: Policy Holder
        /// Drivable: false
        /// Towed: false        
        /// TPAssetDamaged: false
        /// TPOnlyClaim: false
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim with full policy cover accident while driving forward another car hit rear of my car")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_AnotherVehicleHitRearOfMyCar_NotDrivable_TowedToHome()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileDrivingOtherVehicleHitRearOfMyCar, TravelDirection.Forward,
                 DriverRole.PolicyHolder, isDrivable: false, wasTowed: true, towedTo: MotorClaimTowedTo.HomeAddress);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Another car hit my car
        /// Cover Type: Full Cover
        /// Travel Direction: Stationary 
        /// Driver: Co Policy Owner
        /// Drivable: true
        /// Towed: false
        /// Repairer: Second option
        /// TPAssetDamaged: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while stationary another car hit my car")]
        public void MultipleVehicleCollision_FullCover_Stationary_AnotherCarHitRearOfMyCar_Drivable_TPAssetDamaged()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileStationaryAnotherCarHitRearOfMyCar, TravelDirection.Stationary,
                DriverRole.CoPolicyHolder, isDrivable: true, wasTowed: false, repairer: RepairerOption.Second, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Another car hit my car when failed to give way
        /// Cover Type: Full Cover
        /// Travel Direction: Forward
        /// Driver: Policy Holder
        /// Drivable: false
        /// Towed: true
        /// Towed To: Repairer
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward another car hit my car when failed to give way")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_AnotherVehicleHitMyCarWhenFailToGiveWay_NotDrivable_TowedToRepairer()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileDrivingOtherVehicleFailToGiveWayAndHitMyCar, TravelDirection.Forward,
                DriverRole.PolicyHolder, isDrivable: false, wasTowed: true, towedTo: MotorClaimTowedTo.Repairer);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Hit another car
        /// Cover Type: Full Cover
        /// Travel Direction: Reversing
        /// Driver: Policy Holder
        /// Drivable: true
        /// Towed: false
        /// Repairer: Get Quote
        /// TPAssetDamaged: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while reversing hit another car")]
        public void MultipleVehicleCollision_FullCover_Reversing_HitAnotherCar_Drivable_GetQuote_TPAssetDamaged()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileReversingHitAnotherCar, TravelDirection.Reversing,
                DriverRole.PolicyHolder, isDrivable: true, wasTowed: false, repairer: RepairerOption.GetQuote, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Hit another car
        /// Cover Type: Third Party Cover
        /// Travel Direction: Reversing
        /// Driver: New Driver
        /// Drivable: Not sure
        /// Towed: false
        /// TPAssetDamaged: true
        /// TpOnlyClaim: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while reversing hit another car")]
        public void MultipleVehicleCollision_TPCover_Reversing_HitAnotherCar_DrivableNotSure_TPOnlyClaim()
        {
            var claim = BuildTestDataThirdPartyCoverMultiVehicleCollision(MotorClaimScenario.WhileReversingHitAnotherCar, TravelDirection.Reversing,
                 DriverRole.NewContact, isDrivable: null, wasTowed: false, isTPAssetDamage: true, isOnlyClaimDamageToTP: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Another car reverse into my car
        /// Cover Type: Full Cover
        /// Travel Direction: Stationary
        /// Driver: Policy Holder
        /// Drivable: true
        /// Towed: false
        /// Repairer: First option        
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while stationary another car reversed into my car")]
        public void MultipleVehicleCollision_FullCover_Stationary_AnotherCarReversedIntoMyCar_Drivable_SelectFirstRepairer()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileStationaryAnotherCarReversedIntoMyCar, TravelDirection.Stationary,
                DriverRole.PolicyHolder, isDrivable: true, wasTowed: false, repairer: RepairerOption.First);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>
        /// Claim Scenario: My car hit another car while failed to give way
        /// Cover Type: Full Cover
        /// Travel Direction: Forward
        /// Driver: Co Policy Owner
        /// Drivable: false 
        /// Towed: true
        /// Towed to: Unknown
        /// TPAssetDamaged: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another car while failed to give way")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_MyCarHitAnotherCarFailToGiveWay_TowedToUnknown_TPAssetDamaged()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileDrivingMyCarHitAnotherCarFailToGiveWay, TravelDirection.Forward,
                DriverRole.CoPolicyHolder, isDrivable: false, wasTowed: true, towedTo: MotorClaimTowedTo.Unknown, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }


        /// <summary>     
        /// Claim Scenario: My car hit another car and something else happened
        /// Cover Type: Full Cover
        /// Travel Direction: Forward
        /// Driver: Co Policy Owner
        /// Drivable: false 
        /// Towed: true
        /// Towed to: Holding yard
        /// TPAssetDamaged: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another car and something else happened")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_MyCarHitAnotherCarSomethingElseHappened_NotDrivable_TowedToHoldingYard_TPAssetDaaged()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileDrivingMyCarHitAnotherCarSomethingElseHappened, TravelDirection.Forward,
                DriverRole.CoPolicyHolder, isDrivable: false, wasTowed: true, towedTo: MotorClaimTowedTo.HoldingYard, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim, detailUiChecking: true);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Full Cover
        /// Travel Direction: Reversing
        /// Driver: Policy Holder
        /// Drivable: true
        /// Towed: false
        /// Repairer: Get Quote
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while reversing something else happened")]
        public void MultipleVehicleCollision_FullCover_Reversing_SomethingElseHappened_Drivable_GetQuote()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileReversingSomethingElseHappened, TravelDirection.Reversing,
                DriverRole.PolicyHolder, isDrivable: true, wasTowed: false, repairer: RepairerOption.GetQuote);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);

            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Full Cover
        /// Travel Direction: Stationary    
        /// Driver: New Driver
        /// Drivable: true
        /// Towed: false
        /// Repairer: First repairer
        /// IsTPAssetDamaged: true
        /// IsTPOnlyClaim: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while stationary something else happened")]
        public void MultipleVehicleCollision_FullCover_Stationary_OtherMultipleVehicleCollision_TPAssetDamaged_TPOnlyClaim()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.NotApplicable, TravelDirection.Stationary,
                DriverRole.NewContact, isDrivable: false, wasTowed: false, isTPAssetDamage: true, isOnlyClaimDamageToTP: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Third Party Cover
        /// Travel Direction: Parked
        /// Driver: Policy Cover
        /// Drivable: false
        /// Towed: false
        /// IsTPAssetDamaged: true
        /// IsTPOnlyClaim: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while parked something else happened")]
        public void MultipleVehicleCollision_TPCover_Parked_OtherMultipleVehicleCollision_TPAssetDamaged_TPOnlyClaim()
        {
            var claim = BuildTestDataThirdPartyCoverMultiVehicleCollision(MotorClaimScenario.NotApplicable, TravelDirection.Parked,
                DriverRole.PolicyHolder, isDrivable: false, wasTowed: false, isTPAssetDamage: true, isOnlyClaimDamageToTP: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Another car hit my car when changing lanes
        /// Cover Type: Full Cover
        /// Travel Direction: Forward
        /// Driver: Co Policy Owner
        /// Drivable: false 
        /// Towed: true
        /// Towed to: Home
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward another car hit my car when changing lanes")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_AnotherVehicleHitMyCarWhenChangingLanes_TowedToHome()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileDrivingOtherVehicleHitMyCarWhenChangingLanes, TravelDirection.Forward,
                 DriverRole.CoPolicyHolder, isDrivable: false, wasTowed: true, towedTo: MotorClaimTowedTo.HomeAddress);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Hit into another reversing car
        /// Cover Type: Full Cover
        /// Travel Direction: Reversing
        /// Driver: New Driver
        /// Drivable: true
        /// Towed: false
        /// Repairer option: Get Quote
        /// IsTPAssetDamaged: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while reversing hit another reversing car")]
        public void MultipleVehicleCollision_FullCover_Reversing_HitAnotherReversingCar_Drivable_GetQuote_TPAssetDamaged()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileReversingHitByAnotherReversingCar, TravelDirection.Reversing,
                DriverRole.NewContact, isDrivable: true, wasTowed: false, repairer: RepairerOption.GetQuote, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);

            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }


        /// <summary>     
        /// Claim Scenario: My car hit another car when changing lanes
        /// Cover Type: Full Cover
        /// Travel Direction: Forward
        /// Driver: Co Policy Owner
        /// Drivable: true
        /// Towed: false
        /// Repairer option: Second
        /// IsTPAssetDamaged: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another car when changing lanes")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_MyCarHitAnotherCarWhenChangingLanes_SelectSecondRepairer_TPAssetDamaged()
        {
            var claim = BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario.WhileDrivingMyCarHitAnotherCarWhenChangingLanes, TravelDirection.Forward,
                DriverRole.CoPolicyHolder, isDrivable: true, wasTowed: false, repairer: RepairerOption.Second, isTPAssetDamage: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);

            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: My car hit another car when changing lanes
        /// Cover Type: Third Party Cover
        /// Travel Direction: Forward     
        /// Driver: Policy Cover
        /// Drivable: false
        /// Towed: false
        /// IsTPAssetDamaged: true
        /// IsTPOnlyClaim: true
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another car when changing lanes")]
        public void MultipleVehicleCollision_TPCover_DrivingForward_MyCarHitAnotherCarWhenChangingLanes_TPAssetDamaged_TPOnlyClaim()
        {
            var claim = BuildTestDataThirdPartyCoverMultiVehicleCollision(MotorClaimScenario.WhileDrivingMyCarHitAnotherCarWhenChangingLanes, TravelDirection.Forward,
                DriverRole.PolicyHolder, isDrivable: false, wasTowed: false, isTPAssetDamage: true, isOnlyClaimDamageToTP: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }


        /// <summary>     
        /// Claim Scenario: Another car hit my car and something else happened
        /// Cover Type: Full Cover
        /// Travel Direction: Forward
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward another car hit my car and something else happened")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_AnotherVehicleHitMyCarSomethingElseHappened()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingOtherVehicleHitMyCarSomethingElseHappened, TravelDirection.Forward);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);

            if (claim.RepairerOption != RepairerOption.None && !claim.OnlyClaimDamageToTP)
            {
                ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            }

            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Our car hit another car when both changing lanes
        /// Cover Type: Full Cover
        /// Travel Direction: Forward       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward our car hit another car when both changing lanes")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_OurCarHitOneAnotherCarWhenBothChangingLanes()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingOurCarHitOneAnotherCarWhenChangingLanes, TravelDirection.Forward);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            if (claim.RepairerOption != RepairerOption.None && !claim.OnlyClaimDamageToTP)
            {
                ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            }
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Our car hit another car when both failed to give way
        /// Cover Type: Full Cover
        /// Travel Direction: Forward       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward our car hit another car when both failed to give way")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_OurCarHitOneAnotherCarBothFailedToGiveWay()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingOurCarHitOneAnotherCarBothFailedToGiveWay, TravelDirection.Forward);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);

            if (claim.RepairerOption != RepairerOption.None && !claim.OnlyClaimDamageToTP)
            {
                ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            }
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Our car hit another car when something else happened
        /// Cover Type: Full Cover
        /// Travel Direction: Forward       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward our car hit another car when something else happened")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_OurCarHitOneAnotherCarSomethingElseHappened()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingOurCarHitOneAnotherCarSomethingElseHappened, TravelDirection.Forward);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);

            if (claim.RepairerOption != RepairerOption.None && !claim.OnlyClaimDamageToTP)
            {
                ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            }
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Full Cover
        /// Travel Direction: Forward       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward something else happened")]
        public void MultipleVehicleCollision_FullCover_DrivingForward_SomethingElseHappened()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingSomethingElseHappened, TravelDirection.Forward);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);

            if (claim.RepairerOption != RepairerOption.None && !claim.OnlyClaimDamageToTP)
            {
                ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            }
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Hit by another car
        /// Cover Type: Full Cover
        /// Travel Direction: Reversing       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while reversing hit by another car")]
        public void MultipleVehicleCollision_FullCover_Reversing_HitByAnotherCar()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileReversingHitByAnotherCar, TravelDirection.Reversing);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);

            if (claim.RepairerOption != RepairerOption.None && !claim.OnlyClaimDamageToTP)
            {
                ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            }
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Full Cover
        /// Travel Direction: Parked       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while parked something else happened")]
        public void MultipleVehicleCollision_FullCover_Parked_SomethingElseHappened()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileParkedSomethingElseHappened, TravelDirection.Parked);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);

            if (claim.RepairerOption != RepairerOption.None && !claim.OnlyClaimDamageToTP)
            {
                ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            }
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Full Cover
        /// Travel Direction: Stationary       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while stationary something else happened")]
        public void MultipleVehicleCollision_FullCover_Stationary_SomethingElseHappened()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileStationarySomethingElseHappened, TravelDirection.Stationary);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);

            if (claim.RepairerOption != RepairerOption.None && !claim.OnlyClaimDamageToTP)
            {
                ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            }
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: My car hit another car when changing lanes
        /// Cover Type: Full Cover
        /// Travel Direction: Forward       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another car when changing lanes")]
        public void MultipleVehicleCollision_FullCover_TPO_DrivingForward_MyCarHitAnotherCarWhenChangingLanes()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingMyCarHitAnotherCarWhenChangingLanes, TravelDirection.Forward,
                 isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }


        /// <summary>     
        /// Claim Scenario: My car hit another parked car
        /// Cover Type: Full Cover
        /// Travel Direction: Forward       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another parked car")]
        public void MultipleVehicleCollision_FullCover_TPO_DrivingForward_MyCarHitAParkedCar()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingMyCarHitAParkedCar, TravelDirection.Forward,
                isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }


        /// <summary>     
        /// Claim Scenario: My car hit another car and something else happened
        /// Cover Type: Full Cover
        /// Travel Direction: Forward       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another car and something else happened")]
        public void MultipleVehicleCollision_FullCover_TPO_DrivingForward_MyCarHitAnotherCarSomethingElseHappened()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingMyCarHitAnotherCarSomethingElseHappened, TravelDirection.Forward,
                isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Full Cover
        /// Travel Direction: Forward       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward something else happened")]
        public void MultipleVehicleCollision_FullCover_TPO_DrivingForward_SomethingElseHappened()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingSomethingElseHappened, TravelDirection.Forward,
                isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Hit another car
        /// Cover Type: Full Cover
        /// Travel Direction: Reversing       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while reversing hit another car")]
        public void MultipleVehicleCollision_FullCover_TPO_Reversing_HitAnotherCar()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileReversingHitAnotherCar, TravelDirection.Reversing,
                 isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Full Cover
        /// Travel Direction: Reversing       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while reversing something else happened")]
        public void MultipleVehicleCollision_FullCover_TPO_Reversing_OtherMultipleVehicleCollision()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileReversingSomethingElseHappened, TravelDirection.Reversing,
                isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Full Cover
        /// Travel Direction: Parked       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while parked something else happened")]
        public void MultipleVehicleCollision_FullCover_TPO_Parked_OtherMultipleVehicleCollision()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.NotApplicable, TravelDirection.Parked,
                isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: My car hit rear of another car
        /// Cover Type: Third Party Cover
        /// Travel Direction: Forward       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit rear of another car")]
        public void MultipleVehicleCollision_ThirdPartyCover_DrivingForward_MyCarHitRearOfAnotherCar()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_fullCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingMyCarHitRearOfAnotherCar, TravelDirection.Forward,
                isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: My car hit another car and something else happened
        /// Cover Type: Third Party Cover
        /// Travel Direction: Forward       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward my car hit another car and something else happened")]
        public void MultipleVehicleCollision_ThirdPartyCover_DrivingForward_MyCarHitAnotherCarSomethingElseHappened()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_thirdPartyCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingMyCarHitAnotherCarSomethingElseHappened,
                TravelDirection.Forward, isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Third Party Cover
        /// Travel Direction: Forward       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while driving forward something else happened")]
        public void MultipleVehicleCollision_ThirdPartyCover_DrivingForward_SomethingElseHappened()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_thirdPartyCoverMotorPoliciesForClaims, MotorClaimScenario.WhileDrivingSomethingElseHappened,
                TravelDirection.Forward, isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Hit a parked car
        /// Cover Type: Third Party Cover
        /// Travel Direction: Reversing       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while reversing hit a parked car")]
        public void MultipleVehicleCollision_ThirdPartyCover_Reversing_HitParkedCar()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_thirdPartyCoverMotorPoliciesForClaims, MotorClaimScenario.WhileReversingHitParkedCar, TravelDirection.Reversing,
                isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Third Party Cover
        /// Travel Direction: Reversing       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while reversing something else happened")]
        public void MultipleVehicleCollision_ThirdPartyCover_Reversing_OtherMultipleVehicleCollision()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_thirdPartyCoverMotorPoliciesForClaims, MotorClaimScenario.WhileReversingSomethingElseHappened, TravelDirection.Reversing,
                isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Third Party Cover
        /// Travel Direction: Parked       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while parked something else happened")]
        public void MultipleVehicleCollision_ThirdPartyCover_Parked_OtherMultipleVehicleCollision()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_thirdPartyCoverMotorPoliciesForClaims, MotorClaimScenario.NotApplicable, TravelDirection.Parked,
                isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>     
        /// Claim Scenario: Something else happened
        /// Cover Type: Third Party Cover
        /// Travel Direction: Stationary       
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.MultiVehicleCollision)]
        [Test(Description = "Multiple vehicle motor collision claim accident with while stationary something else happened")]
        public void MultipleVehicleCollision_ThirdPartyCover_Stationary_OtherMultipleVehicleCollision()
        {
            var claim = BuildRandomTestDataMultiVehicleCollision(_thirdPartyCoverMotorPoliciesForClaims, MotorClaimScenario.NotApplicable, TravelDirection.Stationary,
                isTPOnlyClaim: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
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


        private ClaimCar BuildTestDataFullCoverMultiVehicleCollision(MotorClaimScenario motorClaimScenario, TravelDirection travelDirection, DriverRole driverRole,
            bool? isDrivable, bool? wasTowed, MotorClaimTowedTo towedTo = MotorClaimTowedTo.None, RepairerOption repairer = RepairerOption.None,
            bool? isTPAssetDamage = false, bool isOnlyClaimDamageToTP = false)
        {
            ClaimPolicy motorClaimPolicy;

            motorClaimPolicy = GetEligiblePolicy(_fullCoverMotorPoliciesForClaims, driverRole, towedTo);
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
                                                        .WithVehicleTowedDetails(wasTowed, towedTo)
                                                        .WithIsVehicleDriveable(isDrivable)
                                                        .WithRepairerOption(repairer)
                                                        .WithRandomPreferredRepairerLocation()
                                                        .WithLinkedMotorPoliciesForClaimant(motorClaimPolicy.linkedMotorPolicies);

            //Add random witness between 1 to 3
            if (DataHelper.RandomBoolean())
            {
                claimCar.AddRandomWitness(DataHelper.RandomNumber(1, 3));
            }

            if (isOnlyClaimDamageToTP)
            {
                claimCar.AddRandomThirdParty(isKnownToClaimant: DataHelper.RandomBoolean(), thirdPartyInsurer: DataHelper.GetInsuranceCompany().PickRandom(), wasDriverTheOwner: DataHelper.RandomBoolean());
            }

            _fullCoverMotorPoliciesForClaims.RemoveAll(x => x.PolicyNumber == motorClaimPolicy.motorPolicy.PolicyNumber);
            DataHelper.AzureTableDeleteEntityBasedOnPolicyNumber(motorClaimPolicy.motorPolicy.PolicyNumber, _azureTableName);
            return claimCar.Build();
        }


        private ClaimCar BuildTestDataThirdPartyCoverMultiVehicleCollision(MotorClaimScenario motorClaimScenario, TravelDirection travelDirection, DriverRole driverRole,
            bool? isDrivable, bool? wasTowed, MotorClaimTowedTo towedTo = MotorClaimTowedTo.None, RepairerOption repairer = RepairerOption.None,
            bool? isTPAssetDamage = false, bool isOnlyClaimDamageToTP = false)
        {
            ClaimPolicy motorClaimPolicy;

            motorClaimPolicy = GetEligiblePolicy(_thirdPartyCoverMotorPoliciesForClaims, driverRole, towedTo);
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
                                                        .AddRandomThirdParty(isKnownToClaimant: DataHelper.RandomBoolean(), thirdPartyInsurer: DataHelper.GetInsuranceCompany().PickRandom(), wasDriverTheOwner: DataHelper.RandomBoolean())
                                                        .WithRandomPoliceDetails()
                                                        .WithRandomDescriptionOfDamageToPHVehicle()
                                                        .WithVehicleTowedDetails(wasTowed, towedTo)
                                                        .WithIsVehicleDriveable(isDrivable)
                                                        .WithRepairerOption(repairer)
                                                        .WithRandomPreferredRepairerLocation()
                                                        .WithLinkedMotorPoliciesForClaimant(motorClaimPolicy.linkedMotorPolicies);


            //Add random witness between 1 to 3
            if (DataHelper.RandomBoolean())
            {
                claimCar.AddRandomWitness(DataHelper.RandomNumber(1, 3));
            }

            _thirdPartyCoverMotorPoliciesForClaims.RemoveAll(x => x.PolicyNumber == motorClaimPolicy.motorPolicy.PolicyNumber);
            DataHelper.AzureTableDeleteEntityBasedOnPolicyNumber(motorClaimPolicy.motorPolicy.PolicyNumber, _azureTableName);
            return claimCar.Build();
        }

        private ClaimCar BuildRandomTestDataMultiVehicleCollision(List<MotorPolicyEntity> motorPolicies, MotorClaimScenario motorClaimScenario, TravelDirection travelDirection, bool isTPOnlyClaim = false)
        {
            ClaimPolicy motorClaimPolicy;
            var driverRole = DataHelper.GetRandomEnum<DriverRole>();

            motorClaimPolicy = GetEligiblePolicy(motorPolicies, driverRole, MotorClaimTowedTo.None);
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

            if (DataHelper.RandomBoolean())
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
            }

            //Add random witness between 1 to 3
            if (DataHelper.RandomBoolean())
            {
                claimCar.AddRandomWitness(DataHelper.RandomNumber(1, 3));
            }

            if (isTPOnlyClaim)
            {
                claimCar.AddRandomThirdParty(isKnownToClaimant: DataHelper.RandomBoolean(), thirdPartyInsurer: DataHelper.GetInsuranceCompany().PickRandom(), wasDriverTheOwner: DataHelper.RandomBoolean());
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
        private ClaimPolicy GetEligiblePolicy(List<MotorPolicyEntity> motorPolicyEntities, DriverRole driverRole, MotorClaimTowedTo towedTo)
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
                        claimPolicy.claimant = claimant;
                        claimPolicy.driver = driver;
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