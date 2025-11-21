using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using System;
using System.Collections.Generic;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyPet;

namespace B2C.NewBusiness
{
    [Property("Functional", "B2C tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class Pet : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "B2C mandatory regression tests for pet anonymous new business.");
        }

        /// <summary>
        /// Automated version of mandatory regression test:
        /// "B2C Anonymous - Pet Policy - TC01 - Dog Annual DD"
        /// Test emulates the workflow of a anonymous user attempting to obtain a Pet Policy via B2C for a dog.
        /// Bank Account - Annual with BSB validation
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Pet), Category(TestCategory.New_Business), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.B2CPCM)]
        public void INSU_T246_B2C_NewPetPolicy_Dog()
        {
            var petQuote = BuildTestDataDogPolicy();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, petQuote.ToString());

            Reporting.LogTestStart();
            string policyNumber = ActionsQuotePet.PurchasePetPolicy(_browser, petQuote, detailUIChecking: true);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuotePet.VerifyPetPolicyInShield(petQuote, policyNumber);
        }

        /// <summary>
        /// Automated version of mandatory regression test:
        /// "B2C Anonymous - Pet Policy - TC02 - Cat Monthly CC"
        /// Test emulates the workflow of a anonymous user attempting to obtain a Pet Policy via B2C for a cat.
        /// Credit Card - Monthly
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Pet), Category(TestCategory.New_Business), 
            Category(TestCategory.Westpac_Payment), Category(TestCategory.B2CPCM)]
        public void INSU_T247_B2C_NewPetPolicy_Cat()
        {
            var petQuote = BuildTestDataCatPolicy();
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, petQuote.ToString());

            Reporting.LogTestStart();
            string policyNumber = ActionsQuotePet.PurchasePetPolicy(_browser, petQuote);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuotePet.VerifyPetPolicyInShield(petQuote, policyNumber);
        }

        private QuotePet BuildTestDataDogPolicy()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual().Build();

            var petQuote = new PetBuilder().InitialisePetWithRandomData(mainPH)
                                             .WithTypeAndRandomBreed(PetType.Dog)
                                             .WithTLCCover(true)
                                             .WithPaymentMethod(new Payment(mainPH).BankAccount().Annual())
                                             .Build();

            return petQuote;
        }

        private QuotePet BuildTestDataCatPolicy()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual().Build();

            var petQuote = new PetBuilder().InitialisePetWithRandomData(mainPH)
                                             .WithTypeAndRandomBreed(PetType.Cat)
                                             .WithTLCCover(false)
                                             .WithPaymentMethod(new Payment(mainPH).CreditCard().Monthly())
                                             .Build();
    
            return petQuote;
        }
    }
}