using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using UIDriver.Helpers;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;

namespace Spark.NewBusiness
{
    [TestFixture]
    [Property("Functional", "Motorcycle tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class MotorcycleAnonymous : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Regression tests for motorcycle new business.");
        }

        /// <summary>
        /// MCO: Issue Policy: No match attempted - Random data
        /// 
        /// To confirm is a MCO policy where there is no match attempted can be successfully purchased 
        /// via the automation tool when completely random data is selected.
        /// </summary>
        [Test, Description("MCO IssuePolicy No Match Attempted RandomData")]
        [Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Spark), Category(TestCategory.Motorcycle)]
        public void INSU_T194_MCO_IssuePolicy_No_Match_Attempted_RandomData()
        {
            // Setup test data
            Contact  mainPH;
            QuoteMotorcycle quoteInputs;
            BuildTestDataTest001(out mainPH, out quoteInputs);

            ActionsQuoteMotorcycle.PurchaseMotorcyclePolicy(_browser, quoteInputs);
        }

        /// <summary>
        //To confirm a user can take out a policy with a payment method of Direct debit and a payment frequency of Annual.
        //Also confirms:
        //-Authorisation message is correct
        //-Validate BSB scenarios for invalid, no match and valid bsb numbers
        //-New bank account can be added to a contact
        //-Verify in Shield that the policy is created
        //However this test does not verify that the Computershare document has been created correctly.
        /// </summary>
        [Test, Description("MCO IssuePolicy Annual DirectDebit ContactDoesNotExist")]
        [Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Spark), Category(TestCategory.Motorcycle), Category(TestCategory.InsuranceContactService), Category(TestCategory.VisualTest)]

        public void INSU_T193_MCO_IssuePolicy_Annual_DirectDebit_ContactDoesNotExist()
        {
            // Setup test data
            var quoteInputs = BuildTestDataForIssuePolicyAnnualDirectDebitContactDoesNotExist();

            ActionsQuoteMotorcycle.FetchNewMotorCycleQuote(_browser, quoteInputs);
            var policyNumber = ActionsQuoteMotorcycle.ProceedWithQuoteToPurchase(_browser, quoteInputs, detailUIChecking:true);

            VerifyPolicyMotorcycle.VerifyPolicyDetailsInShieldDB(policyNumber, quoteInputs);
        }

        /// <summary>
        //To confirm a user can take out a quote when they have not matched upfront.
        //Also confirms:
        //Mapping for no match contact - Anonymous contact is created in Shield correctly
        //Premium is correctly displayed in MCO
        //Quote breakdown is displayed
        //Dynamic question values are mapped
        //Dynamic page name for Tell us more about your<Bike>
        //NOT AUTOMATED: Display and layout for the above page
        /// </summary>
        [Test, Description("MCO CreateQuote No Match Attempted")]
        [Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Spark), Category(TestCategory.Motorcycle), Category(TestCategory.InsuranceContactService)]

        public void INSU_T188_MCO_CreateQuote_No_Match_Attempted()
        {
            var quoteInputs = BuildTestDataFor_CreateQuoteNoMatchAttempted();

            ActionsQuoteMotorcycle.FetchNewMotorCycleQuote(_browser, quoteInputs);
            var quoteNumber = ActionsQuoteMotorcycle.VerifyQuoteBreakdownTextAndGetQuoteNumber(_browser, quoteInputs);

            Contact expectedContact = ShieldAPIVerification.BuildExpectedContact(Vehicle.Motorcycle, SparkBasePage.QuoteStage.AFTER_QUOTE, quoteInputs.Drivers[0].Details);

            VerifyPolicyMotorcycle.VerifyQuoteContactAndVehicleDetailsInShield(quoteInputs, quoteNumber, expectedContact, SparkBasePage.QuoteStage.AFTER_QUOTE);
        }

        private void BuildTestDataTest001(out Contact mainPH, out QuoteMotorcycle motorcycleQuote)
        {
            // Setup test data
            mainPH = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true).Build();
            motorcycleQuote = new MotorCycleBuilder().InitialiseMotorCycleQuoteWithRandomData(mainPH, true).Build();
            
            // Logging out test data for easier debugging and auditing.
            Reporting.Log(mainPH.ToString());
            Reporting.Log(motorcycleQuote.ToString());
        }

        private QuoteMotorcycle BuildTestDataForIssuePolicyAnnualDirectDebitContactDoesNotExist()
        {
            // Setup test data
            var mainPH = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true)
                .WithRandomDateOfBirth(20, 99)
                .Build();
            var motorcycleQuote = new MotorCycleBuilder().InitialiseMotorCycleQuoteWithRandomData(mainPH, true)
                .WithImmobiliser()
                .WithAnnualKms(AnnualKms.LessThan5000)
                .WithAnnualPaymentFrequency()
                .WithCover(MotorCovers.MFCO)
                .WithDashcam(true)
                .WithoutFinancier()
                .WithIsModified(false)
                .WithIsGaraged(true)
                .WithTracker()
                .WithUsage(MotorcycleUsage.Private)
                .Build();
            motorcycleQuote.PayMethod.IsPaymentByBankAccount = true;
            // Logging out test data for easier debugging and auditing.
            Reporting.Log(motorcycleQuote.Drivers[0].Details.ToString());
            Reporting.Log(motorcycleQuote.ToString());

            return motorcycleQuote;
        }

        private QuoteMotorcycle BuildTestDataFor_CreateQuoteNoMatchAttempted()
        {
            // Setup test data
            var mainPH = new ContactBuilder().InitialiseRandomIndividual().WithoutDeclaringMembership(true)
                .Build();
            var motorcycleQuote = new MotorCycleBuilder().InitialiseMotorCycleQuoteWithRandomData(mainPH, true)
                .WithDashcam(true)
                .WithoutFinancier()
                .WithIsGaraged(true)
                .WithUsage(MotorcycleUsage.FoodDelivery)
                .WithRandomVehicle(minValue: 20000)
                .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.Log(mainPH.ToString());
            Reporting.Log(motorcycleQuote.ToString());

            return motorcycleQuote;
        }
    }
}
