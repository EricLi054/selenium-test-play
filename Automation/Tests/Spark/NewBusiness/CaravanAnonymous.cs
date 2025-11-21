using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using System.Collections.Generic;
using UIDriver.Pages.Spark.CaravanQuote;
using System;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using System.Linq;

namespace Spark.NewBusiness
{
    [Property("Functional", "Caravan tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class CaravanAnonymous : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Spark mandatory regression tests for Spark Caravan new business.");
        }

        #region Test Cases
        /// <summary>
        /// Test emulates the workflow of an Anonymous user who is younger than 50 year old (age 16 to 49),
        /// along with a joint Policyholder who is older than 50 years old (age 50 to 100), gets a Caravan Quote via Spark Caravan:
        /// 1. Get Excess Waiver (Excess toggle not shown in the Quote Page) and discount message on quote page.
        /// 2. Select a random payment frequency.
        /// 3. Customise the Quote by changing 'Insured value' and 'Contents cover' to maximum allowed.
        /// 4. Get Quote
        /// 5. Verify the Quote (including quote breakdown), Cover, Policyholder and Caravan details in Shield
        /// 6. Verify the bsb validation triggered
        /// 7. Purchase the policy, payment method - Bank, payment frequency - Annual
        /// 8. Verify the policy details in Shield
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.Smoke), Category(TestCategory.InsuranceContactService)]
        [Test(Description = "Caravan: AnonymousUser: Create And Customise Quote: 2 Policyholders: Zero Excess: BSB Validation")]
        public void INSU_T146_Caravan_AnonymousUser_CreatePolicy_2PH_ZeroExcessAndCustomise_Annual_BankAccount()
        {
            var quoteInputs = BuildTestDataZeroExcessMaxSIMaxContSIRandomIndividualsAndCaravan();

            Reporting.LogTestStart();
            ActionsQuoteCaravan.CreateNewCaravanQuote(_browser, quoteInputs);
            ActionsQuoteCaravan.ProceedWithQuoteToPurchase(_browser, quoteInputs, detailUIChecking: true);
            string policyNumber = ActionsQuoteCaravan.GetPolicyNumberFromConfirmationPage(_browser);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteCaravan.VerifyCaravanPolicyInShield(_browser, quoteInputs, policyNumber);
        }

        /// <summary>
        /// Test emulates the workflow of an Anonymous user who is 20 years old and turning 21 years the next day (Positive rate change),
        /// gets a Caravan Policy via Spark Caravan:
        /// 1. Get the $50 Excess by default (Excess toggle shown in the Quote page).
        /// 2. Change the Excess to the minimum allowed.
        /// 3. Customise the Quote by changing 'Insured value' and 'Contents cover' to the maximum allowed.
        /// 4. Select Annual payment frequency.
        /// 5. Get Quote
        /// 6. Sets the Policy start date to the next day, to trigger the Positive 'Premium change' pop-up (due to rate change based on age factor)
        /// 7. Verify the Quote (including quote breakdown), Cover, Policyholder and Caravan details in Shield
        /// 8. Purchase the policy, payment method - Credit Card, payment frequency - Monthly
        /// 9. Verify the policy details in Shield
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark), Category(TestCategory.Regression)
            , Category(TestCategory.SparkB2CRegressionForMemberCentralReleases), Category(TestCategory.VisualTest)]
        [Test(Description = "Caravan: AnonymousUser: Create And Customise Quote: $50 Excess: Annaul Frequency: Positive Premium change based on age")]
        public void INSU_T147_Caravan_AnonymousUser_CreatePolicy_ChangeExcessAndCustomise_AgeBasedPositivePremiumChangePopup_Monthly_CreditCard()
        {
            var quoteInputs = BuildTestDataWithMinExcessMaxSIMaxContSIPositivePremChngPayAnnualRandom20OldIndividual();

            Reporting.LogTestStart();
            ActionsQuoteCaravan.CreateNewCaravanQuote(_browser, quoteInputs);
            ActionsQuoteCaravan.ProceedWithQuoteToPurchase(_browser, quoteInputs);
            string policyNumber = ActionsQuoteCaravan.GetPolicyNumberFromConfirmationPage(_browser);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteCaravan.VerifyCaravanPolicyInShield(_browser, quoteInputs, policyNumber);
            //TODO AUNT-196 Add further confirmation of member information to match manual SPK-T281 test.
        }

        /// <summary>
        /// Test emulates the workflow of an Anonymous user who is 75 years old and turning 76 years the next day (Negative rate change),
        /// gets a Caravan Policy via Spark Caravan:
        /// 1. Gets the $0 Excess by default (Excess toggle not shown in the Quote page).
        /// 2. Customise the Quote by changing 'Contents cover' to the maximum allowed.
        /// 3. Select Monthly payment frequency
        /// 4. Get Quote
        /// 5. Sets the Policy start date to the next day, to trigger the Negative 'Premium change' pop-up (due to rate change based on age factor)
        /// 6. Verify the Quote (including quote breakdown), Cover, Policyholder and Caravan details in Shield
        /// 7. Purchase the policy, payment method - Credit Card, payment frequency - Annual
        /// 8. Verify the policy details in Shield
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark)]
        [Test(Description = "Caravan: AnonymousUser: Create Quote: $0 Excess: Monthly Frequency: Negative Premium change based on age")]
        public void INSU_T316_Caravan_AnonymousUser_CreatePolicy_NoExcess_AgeBasedNegativePremiumChangePopup_Annual_CreditCard()
        {
            var quoteInputs = BuildTestDataNegativePremChngPayMonthlyRandom75OldIndividual();

            Reporting.LogTestStart();
            ActionsQuoteCaravan.CreateNewCaravanQuote(_browser, quoteInputs);
            ActionsQuoteCaravan.ProceedWithQuoteToPurchase(_browser, quoteInputs);
            string policyNumber = ActionsQuoteCaravan.GetPolicyNumberFromConfirmationPage(_browser);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteCaravan.VerifyCaravanPolicyInShield(_browser, quoteInputs, policyNumber);
        }

        /// <summary>
        /// Test emulates the workflow of an Anonymous user who is younger than 50 years old (age 16 to 49),
        /// gets a Caravan Policy via Spark Caravan for a Caravan which has a market value of less than $1000:
        /// 1. Caravan market value is less than $1000 ($0 to $999), so user is asked to enter a value between $1000 and $150,000
        /// 2. Gets the $50 Excess by default (Excess toggle shown in the Quote page)
        /// 3. Change the Excess to the minimum allowed.
        /// 4. Customise the Quote by changing 'Insured value' to the maximum allowed and 'Contents cover' to a random value between $6,000 ~ $15,000.
        /// 5. Select Monthly payment frequency
        /// 6. Get Quote
        /// 7. Verify the Quote (including quote breakdown), Cover, Policyholder and Caravan details in Shield
        /// 8. Purchase the policy, payment method - Bank, payment frequency - Monthly
        /// 9. Verify the policy details in Shield
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark), Category(TestCategory.Smoke), Category(TestCategory.Regression)]
        [Test(Description = "Caravan: AnonymousUser: Create And Customise Quote: Market Value less than 1000: $50 Excess")]
        public void INSU_T150_Caravan_AnonymousUser_EmailRetrieveQuote_MarketValueLessThan1000_WithExcessAndCustomise_Monthly_BankAccount()
        {
            var quoteInputs = BuildTestDataWithMinExcessRandomIndividualAndCaravanLessThan1000();

            Reporting.LogTestStart();

            ActionsQuoteCaravan.CreateNewCaravanQuote(_browser, quoteInputs);
            ActionsQuote.RetrieveSparkQuoteFromEmail(_browser, quoteInputs.ParkingAddress, quoteInputs.PolicyHolders[0].PrivateEmail.Address, ShieldProductType.MGV);

            ActionsQuoteCaravan.VerifyHeresYourQuoteAfterRetrieve(_browser, quoteInputs);

            ActionsQuoteCaravan.ProceedWithQuoteToPurchase(_browser, quoteInputs);
            string policyNumber = ActionsQuoteCaravan.GetPolicyNumberFromConfirmationPage(_browser);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteCaravan.VerifyCaravanPolicyInShield(_browser, quoteInputs, policyNumber);
        }

        /// <summary>
        /// This test emulates the workflow of an anonymous user who is
        /// PH1: >= 16 and <= 49 PH2 : >= 16 and <= 49 | Caravan Market Value : > 30000 | Excess: 0 | Content Insured: 15000
        /// 1. Before we get started:                   Quote flow No
        /// 2. Let's Start with your caravan:           Random caravan with market value >= 30000
        /// 3. Tell us more about your <Make>
        /// 4. Now, a bit about the policyholders:      PH1 >= 16 and <= 49 PH2 : >= 16 and <= 49 
        /// 5. Here's your quote:                       Payment Frequency: Monthly | Excess: 0 | Content Insured: 15000
        /// 6. Retrieve Quote
        /// 7: Verify Here's your quote Values
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark)]
        [Test(Description = "Caravan: AnonymousUser: Create And Retrieve Quote: Payment Frequency: Monthly | Excess: 0 | Content insured : 15000| caravan Value >= 30000 | PH1 & PH2 below 50")]
        public void INSU_T317_Caravan_AnonymousUser_RetrieveQuote_Monthly_ContentInsured15000()
        {
            var quoteInputs = BuildTestDataWith0ExcessAndContentInsured15000Monthly();

            Reporting.LogTestStart();
            ActionsQuoteCaravan.CreateNewCaravanQuote(_browser, quoteInputs);
            ActionsQuote.RetrieveSparkQuoteByQuoteNumber(_browser, quoteInputs.ParkingAddress, quoteInputs.QuoteData.QuoteNumber, ShieldProductType.MGV);

            ActionsQuoteCaravan.VerifyHeresYourQuoteAfterRetrieve(_browser, quoteInputs);

            ActionsQuoteCaravan.ProceedWithQuoteToPurchase(_browser, quoteInputs);
            string policyNumber = ActionsQuoteCaravan.GetPolicyNumberFromConfirmationPage(_browser);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteCaravan.VerifyCaravanPolicyInShield(_browser, quoteInputs, policyNumber);
        }
        #endregion

        #region Test cases helper methods


        private QuoteCaravan BuildTestDataZeroExcessMaxSIMaxContSIRandomIndividualsAndCaravan()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true)
                                .WithRandomDateOfBirth(MIN_PH_AGE_VEHICLES, MIN_AGE_FOR_EXCESS_WAIVER - 1).Build();
            var jointPH = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true)
                                .WithRandomDateOfBirth(MIN_AGE_FOR_EXCESS_WAIVER, MAX_PH_AGE).Build();
            var caravan = new CaravanBuilder().InitialiseCaravanWithRandomData(new List<Contact>() { mainPH, jointPH })
                                .WithExcess(CARAVAN_MIN_EXCESS_VALUE)
                                .WithPaymentMethod(new Payment(mainPH).BankAccount().Annual())
                                .WithInsuredVariance(ActionsQuoteCaravan.MAX_SUM_INSURED_PERCENTAGE)
                                .WithContentsCoverValue(MAX_CONTENT_INSURANCE_VALUE)
                                .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, caravan.ToString());

            return caravan;
        }

        private QuoteCaravan BuildTestDataWithMinExcessMaxSIMaxContSIPositivePremChngPayAnnualRandom20OldIndividual()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true)
                                .WithDateOfBirth(DateTime.Now.AddYears(-PremiumChangePopup.DRIVER_AGE_FACTOR_RATE_GROUP2_MIN_AGE).AddDays(1)) //To trigger the Positive Premium change pop-up (based on the driver age factor), when the Policy start date is set to the next day.
                                .Build();
            var caravan = new CaravanBuilder().InitialiseCaravanWithRandomData(new List<Contact>() { mainPH })
                                .WithRandomCaravan(20000) //Increasing the minimum caravan value to a significant value, so that we can expect a premium change
                                .WithPaymentMethod(new Payment(mainPH).CreditCard().Monthly())
                                //In Shield, On-Site caravans have different rate groups compared to 'Trailed' caravans.
                                //For this test we specify a 'Trailed' caravan type.
                                .WithRandomParkLocation(mainPH.MailingAddress.PostCode, excludeOnSite: true)
                                .WithExcess(CARAVAN_MIN_EXCESS_VALUE)
                                .WithInsuredVariance(ActionsQuoteCaravan.MAX_SUM_INSURED_PERCENTAGE)
                                .WithContentsCoverValue(MAX_CONTENT_INSURANCE_VALUE) //To makesure the premium is high enough, to trigger an age based rate change.
                                .WithPolicyStartDate(DateTime.Now.AddDays(1)) //To trigger the Positive Premium change pop-up, based on the driver age factor
                                .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, caravan.ToString());

            return caravan;
        }

        private QuoteCaravan BuildTestDataNegativePremChngPayMonthlyRandom75OldIndividual()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true)
                                .WithDateOfBirth(DateTime.Now.AddYears(-PremiumChangePopup.DRIVER_AGE_FACTOR_RATE_GROUP6_MIN_AGE).AddDays(1)) //To trigger the Negative Premium change pop-up (based on the driver age factor), when the Policy start date is set to the next day.
                                .Build();
            var caravan = new CaravanBuilder().InitialiseCaravanWithRandomData(new List<Contact>() { mainPH })
                                .WithRandomCaravan(100000) //Increasing the minimum caravan value to a significant value, so that we can expect a premium change                                
                                .WithPaymentMethod(new Payment(mainPH).CreditCard().Annual())
                                //In Shield, On-Site caravans have different rate groups compared to 'Trailed' caravans.
                                //For this test we specify a 'Trailed' caravan type.
                                .WithRandomParkLocation(mainPH.MailingAddress.PostCode, excludeOnSite: true)
                                .WithPolicyStartDate(DateTime.Now.AddDays(3)) //To trigger the Negative Premium change pop-up, based on the driver age factor
                                .Build();           

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, caravan.ToString());

            return caravan;
        }

        private QuoteCaravan BuildTestDataWithMinExcessRandomIndividualAndCaravanLessThan1000()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true)
                                .WithRandomDateOfBirth(MIN_PH_AGE_VEHICLES, MIN_AGE_FOR_EXCESS_WAIVER - 1).Build();
            var caravan = new CaravanBuilder().InitialiseCaravanWithRandomData(new List<Contact>() { mainPH })
                                .WithRandomCaravan(0, CARAVAN_MIN_SUM_INSURED_VALUE - 1)
                                .WithPaymentMethod(new Payment(mainPH).BankAccount().Monthly())
                                .WithRandomParkLocation(mainPH.MailingAddress.PostCode)
                                .WithExcess(CARAVAN_MIN_EXCESS_VALUE)
                                .WithAgreedSumInsured(CARAVAN_MAX_SUM_INSURED_VALUE)
                                .WithContentsCoverValue(Randomiser.Get.Next(1, MAX_CONTENT_INSURANCE_VALUE/1000) * 1000)
                                .WithRetrieveQuote(RetrieveQuoteType.Email)
                                .Build();
            
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, caravan.ToString());

            return caravan;
        }

        /// <summary>
        /// This get the data with following condition:
        /// PH1:                >= 16 and <= 49
        /// PH2:                >= 16 and <= 49
        /// Frequency:          Monthly
        /// Caravan Value:      >= 30000
        /// Content Insured:    15000
        /// </summary>
        /// <returns></returns>
        private QuoteCaravan BuildTestDataWith0ExcessAndContentInsured15000Monthly()
        {
            var mainPH = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true)
                                .WithRandomDateOfBirth(MIN_PH_AGE_VEHICLES, MIN_AGE_FOR_EXCESS_WAIVER - 1).Build();
            var jointPH = new ContactBuilder().InitialiseRandomIndividual()
                                .WithRandomDateOfBirth(MIN_PH_AGE_VEHICLES, MIN_AGE_FOR_EXCESS_WAIVER - 1).Build();

            var caravan = new CaravanBuilder().InitialiseCaravanWithRandomData(new List<Contact>() { mainPH, jointPH })
                                .WithRandomCaravan(30000)
                                .WithPaymentMethod(new Payment(mainPH))
                                .WithExcess(CARAVAN_MIN_EXCESS_VALUE)
                                .WithContentsCoverValue(MAX_CONTENT_INSURANCE_VALUE)
                                .WithRetrieveQuote(RetrieveQuoteType.Website)
                                .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, caravan.ToString());

            return caravan;
        }

        #endregion
    }
}
