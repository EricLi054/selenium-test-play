using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ClaimSingleMotorCollision : BaseUITest
    {
        private List<MotorPolicyEntity> _fullCoverMotorPoliciesForClaims;
        private List<MotorPolicyEntity> _evMotorPoliciesForClaims;

        private List<MotorPolicyWithExistingClaim> _motorPoliciesWithExistingClaims;       
        private readonly string _azureTableName = Config.Get().Shield.Environment + DataType.Policy + DataPurpose.ForClaim + ProductType.Motor;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Single Motor Collision Claim");
            _fullCoverMotorPoliciesForClaims = DataHelper.AzureTableGetAllRecords(_azureTableName).FindAll(x => (x.CoverType == "MFCO") && (!x.IsEV) && (x.IsRegistrationValid)).PickRandom(10);            
            _evMotorPoliciesForClaims = DataHelper.AzureTableGetAllRecords(_azureTableName).FindAll(x => (x.CoverType == "MFCO") && (x.IsEV) && (x.IsRegistrationValid)).PickRandom(5);

            _motorPoliciesWithExistingClaims = ShieldMotorClaimDB.FindMotorPoliciesWithExistingClaim();
        }

        #region Test Cases      
        /// <summary>
        /// Detail UI Checking: False     
        /// Claim Scenario: Accident With Your Own Property
        /// Travel Direction: Reversing
        /// Driver : Policyholder
        /// Towed: No
        /// Drivable: No
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim accident with own property while reversing and car is not drivable and not towed")]
        public void SingleVehicleCollision_AccidentWithYourOwnProperty_Reversing_NotDrivable_NotTowed()
        {
            var claim = BuildTestDataSingleVehicleCollision(MotorClaimScenario.AccidentWithYourOwnProperty, TravelDirection.Reversing, DriverRole.PolicyHolder, 
                isDrivable: false, wasTowed: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenMotorTriageClaimURL(_browser, claim);
            ActionsClaimMotor.TriageMotorClaim(_browser, claim.DamageType);            
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>
        /// Detail UI Checking: True      
        /// Claim Scenario: Accident With Your Own Property
        /// Travel Direction: Parked
        /// Driver : Co Policy Holder
        /// Towed: Yes, To Holding yard
        /// Drivable: No
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim accident with own property while parked and car is towed to holding yard")]
        public void SingleVehicleCollision_AccidentWithYourOwnProperty_Parked_NotDrivable_TowedToHoldingYard_DetailUIChecking()
        {
            var claim = BuildTestDataSingleVehicleCollision(MotorClaimScenario.AccidentWithYourOwnProperty, TravelDirection.Parked, DriverRole.CoPolicyHolder,
                 isDrivable: false, wasTowed: true, towedTo: MotorClaimTowedTo.HoldingYard);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim, detailUiChecking: true);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>
        /// Detail UI Checking: False
        /// Claim Scenario: Accident With Someone Else Property
        /// Travel Direction: Reversing
        /// Driver : Co Policy Holder
        /// Towed: Yes, To Repairer
        /// Drivable: No
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim accident with someone else property while reversing and car is not drivable and towed to repairer")]
        public void SingleVehicleCollision_AccidentWithSomeoneElseProperty_Reversing_NotDrvable_TowedToRepairer()
        {
            var claim = BuildTestDataSingleVehicleCollision(MotorClaimScenario.AccidentWithSomeoneElseProperty, TravelDirection.Reversing, DriverRole.CoPolicyHolder,
                isDrivable: false, wasTowed: true, towedTo: MotorClaimTowedTo.Repairer, isTPAssetDamage: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>
        /// Detail UI Checking: true        
        /// Claim Scenario: Accident With Someone Else Property
        /// Travel Direction: Stationary
        /// Driver : Co Policy Holder
        /// Towed: No
        /// Drivable: Yes
        /// Repairer: First Repairer
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim accident with someone else property while stationary and car is drivable, it's a liability only claim with TP details")]
        public void SingleVehicleCollision_TPOnlyClaim_AccidentWithSomeoneElseProperty_Stationary_Drivable_SelectRepairer_DetailUIChecking()
        {
            var claim = BuildTestDataSingleVehicleCollisionWithTPDetails(MotorClaimScenario.AccidentWithSomeoneElseProperty, TravelDirection.Stationary, DriverRole.CoPolicyHolder,
                isDrivable: true, wasTowed: false, repairer: RepairerOption.First, isTPAssetDamage: true, isOnlyClaimDamageToTP: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim, true);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>
        /// Detail UI Checking: true        
        /// Claim Scenario: Accident With Something Else
        /// Travel Direction: Forward
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim accident with someone else property while driving forward, " +
            "it's liability only claim without any TP details, so it's block the online claim submission and show can't claim online error message")]
        public void SingleVehicleCollision_TPOnlyClaim_AccidentWithSomethingElse_Forward_NoTPDetails_CantClaimOnline_ErrorMessage()
        {
            var claim = BuildTestDataSingleVehicleCollision(MotorClaimScenario.AccidentWithSomethingElse, TravelDirection.Forward, DriverRole.PolicyHolder,
                isDrivable: true, wasTowed: false, repairer: RepairerOption.First, isTPAssetDamage: true, isOnlyClaimDamageToTP: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.BeforeYouStart(_browser);
            ActionSparkMotorCollisionClaim.ConfirmContactDetails(_browser, claim);
            if (claim.LoginWith == LoginWith.ContactId && claim.LinkedMotorPoliciesForClaimant.Count > 1)
            {
                ActionSparkMotorCollisionClaim.SelectPolicy(_browser, claim);
            }
            ActionSparkMotorCollisionClaim.StartYourClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AboutTheAccident(_browser, claim);
            ActionSparkMotorCollisionClaim.MoreAboutTheAccident(_browser, claim);
            ActionSparkMotorCollisionClaim.EnterWhereAndHowAccidentHappened(_browser, claim);

            VerifySparkMotorCollisionClaim.VerifyInitialClaimDetailsInShield(claim);

            ActionSparkMotorCollisionClaim.ChooseDriverOfYourCar(_browser, claim);
            ActionSparkMotorCollisionClaim.EnterDriverHistory(_browser, claim);
            ActionSparkMotorCollisionClaim.EnterThirdPartyAndPoliceDetails(_browser, claim);

            VerifySparkMotorCollisionClaim.VerifyCantClaimOnlineErrorMessageOnThirdPartyDetailsPage(_browser);
        }

        /// <summary>
        /// Detail UI Checking: true    
        /// Claim Scenario: Accident With Someone Else Property
        /// Travel Direction: Stationary
        /// Driver : New Contact
        /// Towed: No
        /// Drivable: Yes
        /// Repairer: First Repairer
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single Electric vehicle motor collision claim accident with wildlife while driving forward and car is drivable and select the first repairer option")]
        public void SingleVehicleCollision_AccidentWithWildlife_Forward_Drivable_SelectRepairer_DetailUIChecking()
        {
            var claim = BuildTestDataSingleVehicleCollisionWithEVPolicy(MotorClaimScenario.AccidentWithWildlife, 
                TravelDirection.Forward, DriverRole.NewContact, isDrivable: true, wasTowed: false, repairer: RepairerOption.First);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim, true);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim, true);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>
        /// Detail UI Checking: False       
        /// Claim Scenario: Accident With your own property
        /// Travel Direction: Forward
        /// Driver : Policyholder
        /// Towed: No
        /// Drivable: Yes
        /// Repairer: Get Quote
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim accident with your own property while driving forward and car is drivable and select get quote")]
        public void SingleVehicleCollision_AccidentWithYourOwnProperty_Forward_Drivable_GetQuote()
        {
            var claim = BuildTestDataSingleVehicleCollision(MotorClaimScenario.AccidentWithYourOwnProperty, TravelDirection.Forward, DriverRole.PolicyHolder,
                isDrivable: true, wasTowed: false, repairer: RepairerOption.GetQuote);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AssignServiceProvider(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>
        /// Detail UI Checking: True       
        /// Claim Scenario: Accident With wildlife
        /// Travel Direction: Forward
        /// Driver : New contact
        /// Towed: Yes, To other
        /// Drivable: No
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim accident with wildlife while driving and car is not drivable and towed to Unknown")]
        public void SingleVehicleCollision_AccidentWithWildlife_DrivingForward_NotDrivable_TowedToUnknown_DetailUIChecking()
        {
            var claim = BuildTestDataSingleVehicleCollision(MotorClaimScenario.AccidentWithWildlife, TravelDirection.Forward, DriverRole.NewContact,
                isDrivable: false, wasTowed: true, towedTo: MotorClaimTowedTo.Unknown);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim, detailUiChecking: true);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>
        /// Detail UI Checking: False
        /// Claim Scenario: Accident With Someone's Pet
        /// Travel Direction: Reversing
        /// Driver : Policy Holder
        /// Towed: No
        /// Drivable: Not sure
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim accident with someones pet while revershing and don't know car is drivable")]
        public void SingleVehicleCollision_SomeoneElsePet_Reversing_DrivableNotSure_NotTowed()
        {
            var claim = BuildTestDataSingleVehicleCollisionWithTPDetails(MotorClaimScenario.AccidentWithSomeonesPet, TravelDirection.Reversing, DriverRole.PolicyHolder,
                isDrivable: null, wasTowed: false, isTPAssetDamage: true, isOnlyClaimDamageToTP: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>
        /// Detail UI Checking: False        
        /// Claim Scenario: Something else
        /// Travel Direction: Forward
        /// Driver : Co Policy Holder
        /// Towed: Not sure        
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim accident with something else while driving forward and not sure about the car towed")]
        public void SingleVehicleCollision_SomethingElse_DrivingForward_TowedNotSure()
        {
            var claim = BuildTestDataSingleVehicleCollision(MotorClaimScenario.AccidentWithSomethingElse, TravelDirection.Forward, DriverRole.CoPolicyHolder,
                isDrivable: null, wasTowed: null, isTPAssetDamage: null, isOnlyClaimDamageToTP: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.LodgeMotorCollisionClaim(_browser, claim);
            VerifySparkMotorCollisionClaim.VerifyConfirmationPageAndDetailsInShield(_browser, claim);
        }

        /// <summary>
        /// Detail UI Checking: False        
        /// Validating Duplicate claim error message
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim accident with event date same as previous one and expect duplicate claim error message")]
        public void SingleVehicleCollision_DuplicateClaim_ErrorMessage()
        {
            var claim = BuildTestDataWithExistingClaim(eventDateSameAsPreviousClaimEventDate: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.BeforeYouStart(_browser);
            ActionSparkMotorCollisionClaim.ConfirmContactDetails(_browser, claim);           
            ActionSparkMotorCollisionClaim.StartYourClaim(_browser, claim);

            VerifySparkMotorCollisionClaim.VerifyDuplicateClaimErrorMessage(_browser, claim.ExistingClaim.ClaimNumber);
        }

        /// <summary>
        /// Detail UI Checking: False       
        /// Validating Policy not active error message
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim with event date before policy activation date and expect policy not active error message")]
        public void SingleVehicleCollision_PolicyNotActive_ErrorMessage()
        {
            var claim = BuildTestDataEventDateBeforePolicyActivationDate();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.BeforeYouStart(_browser);
            ActionSparkMotorCollisionClaim.ConfirmContactDetails(_browser, claim);           
            ActionSparkMotorCollisionClaim.StartYourClaim(_browser, claim);

            VerifySparkMotorCollisionClaim.VerifyPolicyNotActiveErrorMessage(_browser);
        }

        /// <summary>
        /// Detail UI Checking: False       
        /// Validating similar claim waring message
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision claim accident with event date with in 14 days from previous claim event date and expect similar claim waring message")]
        public void SingleVehicleCollision_SimilarClaim_Dialog()
        {
            var claim = BuildTestDataWithExistingClaim(eventDateSameAsPreviousClaimEventDate: false);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.BeforeYouStart(_browser);
            ActionSparkMotorCollisionClaim.ConfirmContactDetails(_browser, claim);
            ActionSparkMotorCollisionClaim.StartYourClaim(_browser,claim);

            VerifySparkMotorCollisionClaim.VerifySimilarClaimDialogMessage(_browser, claim.ExistingClaim.EventDate.ToString("dd/MM/yyyy"), claim.ExistingClaim.ClaimNumber);
        }
        
        /// <summary>
        /// Detail UI Checking: False       
        /// Number Of Vehicles Involved: No Other Vehicles Involved
        /// Validating policy cover can't claim online error message
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision with no vehicles involved for policy with TPO/TPFT cover and expect can't claim online error message")]
        public void SingleVehicleCollision_CantClaim_ThirdPartyCover_ErrorMessage()
        {
            var claim = BuildTestDataWithThirdPartyCover();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.BeforeYouStart(_browser);
            ActionSparkMotorCollisionClaim.ConfirmContactDetails(_browser, claim);
            ActionSparkMotorCollisionClaim.StartYourClaim(_browser, claim);
            ActionSparkMotorCollisionClaim.AboutTheAccident(_browser, claim);

            VerifySparkMotorCollisionClaim.VerifyCantClaimOnlineErrorMessageOnAboutTheAccidentPage(_browser);
        }

        /// <summary>
        /// Detail UI Checking: False       
        /// Number Of Vehicles Involved: No Other Vehicles Involved
        /// Validating  Rego invalid  can't claim online error message
        /// </summary>
        [Category(TestCategory.Claim), Category(TestCategory.Motor), Category(TestCategory.Spark), Category(TestCategory.SingleVehicleCollision)]
        [Test(Description = "Single vehicle motor collision with no vehicles involved for policy with TPO/TPFT cover and expect can't claim online error message")]
        public void SingleVehicleCollision_CantClaim_RegistrationNumberInvalid_ErrorMessage()
        {
            var claim = BuildTestDataRegistrationNumberInvalid();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, claim.ToString());

            Reporting.LogTestStart();
            LaunchPage.OpenClaimMotorCollisionURL(_browser, claim);
            ActionSparkMotorCollisionClaim.BeforeYouStart(_browser);
            ActionSparkMotorCollisionClaim.ConfirmContactDetails(_browser, claim);
            ActionSparkMotorCollisionClaim.StartYourClaim(_browser, claim);           

            VerifySparkMotorCollisionClaim.VerifyCantClaimOnlineErrorMessageOnStartYourClaimPage(_browser);
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

        private ClaimCar BuildTestDataSingleVehicleCollision(MotorClaimScenario motorClaimScenario, TravelDirection travelDirection, DriverRole driverRole, 
            bool? isDrivable, bool? wasTowed, MotorClaimTowedTo towedTo = MotorClaimTowedTo.None, RepairerOption repairer = RepairerOption.None, bool? isTPAssetDamage = false, bool isOnlyClaimDamageToTP = false)
        {
            ClaimPolicy motorClaimPolicy;
            motorClaimPolicy = GetEligiblePolicy(_fullCoverMotorPoliciesForClaims, driverRole);           
            var claimCar = new MotorClaimBuilder().InitialiseMotorClaimWithBasicData(motorClaimPolicy.motorPolicy, MotorClaimDamageType.SingleVehicleCollision)
                                                        .WithRiskAddressAndHireCarCover(motorClaimPolicy.motorPolicy.PolicyNumber)
                                                        .LoginWith(DataHelper.GetRandomEnum<LoginWith>())
                                                        .WithClaimant(motorClaimPolicy.claimant, DataHelper.RandomBoolean(), DataHelper.RandomBoolean())
                                                        .WithRandomEventDateInLast7Days(roundDown15Mins: true)
                                                        .WithNumberOfVehiclesInvolved(numberOfVehiclesInvolved: MotorCollisionNumberOfVehiclesInvolved.NoOtherVehiclesInvolved)
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

            _fullCoverMotorPoliciesForClaims.RemoveAll(x => x.PolicyNumber == motorClaimPolicy.motorPolicy.PolicyNumber);
            DataHelper.AzureTableDeleteEntityBasedOnPolicyNumber(motorClaimPolicy.motorPolicy.PolicyNumber, _azureTableName);
            return claimCar.Build();
        }

        private ClaimCar BuildTestDataSingleVehicleCollisionWithTPDetails(MotorClaimScenario motorClaimScenario, TravelDirection travelDirection, DriverRole driverRole,
            bool? isDrivable, bool? wasTowed, MotorClaimTowedTo towedTo = MotorClaimTowedTo.None, RepairerOption repairer = RepairerOption.None, bool? isTPAssetDamage = false, bool isOnlyClaimDamageToTP = false)
        {
            ClaimPolicy motorClaimPolicy;
            motorClaimPolicy = GetEligiblePolicy(_fullCoverMotorPoliciesForClaims, driverRole);
            var claimCar = new MotorClaimBuilder().InitialiseMotorClaimWithBasicData(motorClaimPolicy.motorPolicy, MotorClaimDamageType.SingleVehicleCollision)
                                                        .WithRiskAddressAndHireCarCover(motorClaimPolicy.motorPolicy.PolicyNumber)
                                                        .LoginWith(DataHelper.GetRandomEnum<LoginWith>())
                                                        .WithClaimant(motorClaimPolicy.claimant, DataHelper.RandomBoolean(), DataHelper.RandomBoolean())
                                                        .WithRandomEventDateInLast7Days(roundDown15Mins: true)
                                                        .WithNumberOfVehiclesInvolved(numberOfVehiclesInvolved: MotorCollisionNumberOfVehiclesInvolved.NoOtherVehiclesInvolved)
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

            _fullCoverMotorPoliciesForClaims.RemoveAll(x => x.PolicyNumber == motorClaimPolicy.motorPolicy.PolicyNumber);
            DataHelper.AzureTableDeleteEntityBasedOnPolicyNumber(motorClaimPolicy.motorPolicy.PolicyNumber, _azureTableName);
            return claimCar.Build();
        }

        private ClaimCar BuildTestDataSingleVehicleCollisionWithEVPolicy(MotorClaimScenario motorClaimScenario, TravelDirection travelDirection, DriverRole driverRole,
            bool? isDrivable, bool? wasTowed, MotorClaimTowedTo towedTo = MotorClaimTowedTo.None, RepairerOption repairer = RepairerOption.None, bool? isTPAssetDamage = false, bool isOnlyClaimDamageToTP = false)
        {
            ClaimPolicy motorClaimPolicy;

            motorClaimPolicy = GetEligiblePolicy(_evMotorPoliciesForClaims, driverRole);
            
            var claimCar = new MotorClaimBuilder().InitialiseMotorClaimWithBasicData(motorClaimPolicy.motorPolicy, MotorClaimDamageType.SingleVehicleCollision)
                                                        .WithRiskAddressAndHireCarCover(motorClaimPolicy.motorPolicy.PolicyNumber)
                                                        .LoginWith(DataHelper.GetRandomEnum<LoginWith>())
                                                        .WithClaimant(motorClaimPolicy.claimant, DataHelper.RandomBoolean(), DataHelper.RandomBoolean())
                                                        .WithRandomEventDateInLast7Days(roundDown15Mins: true)
                                                        .WithNumberOfVehiclesInvolved(numberOfVehiclesInvolved: MotorCollisionNumberOfVehiclesInvolved.NoOtherVehiclesInvolved)
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
                                                        .WithPreferredRepairerLocation(new Address()
                                                        {
                                                            Suburb = "Albany",
                                                            PostCode = "6330"
                                                        })
                                                        .WithLinkedMotorPoliciesForClaimant(motorClaimPolicy.linkedMotorPolicies);
            //Add random witness between 1 to 3
            if (DataHelper.RandomBoolean())
            {
                claimCar.AddRandomWitness(DataHelper.RandomNumber(1, 3));
            }

            _evMotorPoliciesForClaims.RemoveAll(x => x.PolicyNumber == motorClaimPolicy.motorPolicy.PolicyNumber);
            DataHelper.AzureTableDeleteEntityBasedOnPolicyNumber(motorClaimPolicy.motorPolicy.PolicyNumber, _azureTableName);
            return claimCar.Build();
        }

        private ClaimCar BuildTestDataWithExistingClaim(bool eventDateSameAsPreviousClaimEventDate)
        {
            ClaimCar testData = null;

            foreach (var claimPolicy in _motorPoliciesWithExistingClaims)
            {
                if (!eventDateSameAsPreviousClaimEventDate && ShieldClaimDB.GetOpenClaimCountForPolicy(claimPolicy.MotorPolicy.PolicyNumber) != 1)
                { continue; }

                var policyDetails = DataHelper.GetPolicyDetails(claimPolicy.MotorPolicy.PolicyNumber);
                claimPolicy.MotorPolicy.PolicyHolders = DataHelper.FetchPolicyContacts(policyDetails);

                if (claimPolicy.MotorPolicy.PolicyHolders.Count == 0)
                {
                    Reporting.Log($"Policy {claimPolicy.MotorPolicy.PolicyNumber} and has no contacts in MC");
                    continue;
                }

                var claimant = claimPolicy.MotorPolicy.PolicyHolders.PickRandom();
                var mcRecord = DataHelper.GetPersonFromMemberCentralByContactId(claimant.Id);
                if (mcRecord == null)
                {
                    Reporting.Log($"Policy {claimPolicy.MotorPolicy.PolicyNumber} and contact {claimant.Id} not in MC");
                    continue;
                }

                // If the conditions are met, start building the claim
                var claimBuilder = new MotorClaimBuilder()
                    .InitialiseMotorClaimWithBasicData(claimPolicy.MotorPolicy, MotorClaimDamageType.SingleVehicleCollision)
                    .WithExistingClaimDetails(claimPolicy.ClaimNumber)
                    .LoginWith(LoginWith.PolicyNumber)
                    .WithClaimant(claimant, false, false)                    
                    .WithClaimScenario(MotorClaimScenario.AccidentWithSomethingElse);

                if (eventDateSameAsPreviousClaimEventDate)
                {
                    claimBuilder.WithEventDateSameAsPreviousClaimEventDate();
                }
                else
                {
                    claimBuilder.WithEventDateWithIn14DaysFromPreviousClaimEventDate();
                }

                testData = claimBuilder.Build();
                break;
            }

            Reporting.IsNotNull(testData, "suitable test data has been found");
            return testData;
        }

        private ClaimCar BuildTestDataWithThirdPartyCover()
        {
            ClaimCar testData = null;
            var policy = DataHelper.AzureTableGetAllRecords(_azureTableName).FindAll(x => (x.CoverType != "MFCO") && (!x.IsEV) && (x.IsRegistrationValid)).First();
            var claimPolicy = DataHelper.GetSingleMotorPolicyDetail(policy, _azureTableName);

            var claimant = claimPolicy.PolicyHolders.PickRandom();
            testData = new MotorClaimBuilder().InitialiseMotorClaimWithBasicData(claimPolicy, MotorClaimDamageType.SingleVehicleCollision)
                                           .LoginWith(LoginWith.PolicyNumber)
                                           .WithClaimant(claimant, false, false)
                                           .WithRandomEventDateInLast7Days(true)
                                           .WithNumberOfVehiclesInvolved(numberOfVehiclesInvolved: MotorCollisionNumberOfVehiclesInvolved.NoOtherVehiclesInvolved)
                                           .WithClaimScenario(MotorClaimScenario.AccidentWithSomethingElse)
                                           .Build();

            Reporting.IsNotNull(testData, "suitable test data has been found");
            DataHelper.AzureTableDeleteEntityBasedOnPolicyNumber(testData.Policy.PolicyNumber, _azureTableName);

            return testData;
        }

        private ClaimCar BuildTestDataEventDateBeforePolicyActivationDate()
        {
            ClaimCar testData = null;

            var motorPolicy = DataHelper.AzureTableGetAllRecords(_azureTableName).Where(x => (x.CoverType == "MFCO") && (!x.IsEV) && (x.IsRegistrationValid))
                .OrderByDescending(t => t.PolicyStartDate).First();
            var policy = DataHelper.GetSingleMotorPolicyDetail(motorPolicy, _azureTableName);

            var claimant = policy.PolicyHolders.Count() > 1 ? policy.PolicyHolders.PickRandom() : policy.PolicyHolders.FirstOrDefault();

            testData = new MotorClaimBuilder()
                    .InitialiseMotorClaimWithBasicData(policy, MotorClaimDamageType.SingleVehicleCollision)
                    .LoginWith(LoginWith.PolicyNumber)
                    .WithClaimant(claimant, false, false)
                    .WithEventDateAndTime(motorPolicy.PolicyStartDate.AddDays(-1), true)
                    .Build();

            Reporting.IsNotNull(testData, "suitable test data has been found");

            return testData;
        }

        private ClaimCar BuildTestDataRegistrationNumberInvalid()
        {
            ClaimCar testData = null;

            var motorPolicy = DataHelper.AzureTableGetAllRecords(_azureTableName).Where(x => !x.IsRegistrationValid).First();
            var policy = DataHelper.GetSingleMotorPolicyDetail(motorPolicy, _azureTableName);

            var claimant = policy.PolicyHolders.PickRandom();

            testData = new MotorClaimBuilder()
                    .InitialiseMotorClaimWithBasicData(policy, MotorClaimDamageType.SingleVehicleCollision)
                    .LoginWith(LoginWith.PolicyNumber)
                    .WithClaimant(claimant, false, false)
                    .WithRandomEventDateInLast7Days(true)
                    .Build();

            Reporting.IsNotNull(testData, "suitable test data has been found");

            return testData;
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
