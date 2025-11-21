using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Rac.TestAutomation.Common.DatabaseCalls.Contacts;
using Tests.ActionsAndValidations;
using System.Collections.Generic;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;

namespace Spark.NewBusiness
{
    [TestFixture]
    [Property("Functional", "Motorcycle tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class MotorcycleMemberMatch : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Regression tests for motorcycle new business.");
        }

        /// <summary>
        /// Placeholder for Motorcycle Online mandatory regression test.
        /// 
        /// Test emulates the workflow of an existing RAC member successfully
        /// being match after declaring they are a member, and proceeding to
        /// purchase a motorcycle policy.
        /// </summary>
        [Test, Description("MCO IssuePolicy Upfront Single Match Random Discount")]
        [Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Spark), Category(TestCategory.Motorcycle),
            Category(TestCategory.Mock_Member_Central_Support), Category(TestCategory.InsuranceContactService)]
        public void INSU_T195_MCO_IssuePolicy_Upfront_Single_Match_Random_Discount()
        {
            var quoteInputs = BuildTestDataTest002();

            ActionsQuoteMotorcycle.PurchaseMotorcyclePolicy(_browser, quoteInputs);
        }

        /// <summary>
        //Confirms:
        //-User can future date a policy
        //-Rego is TBA when user does not enter one in the UI
        //-User can match against MC rule 1
        //-Discount pop up is displayed after Tell us more about your page
        //-Pay Annually using credit card
        //-Authorisation message is correct
        //-Verify the policy in Shield
        //-NOT AUTOMATED: Computershare document and receipt is sent
        //-NOT AUTOMATED: CCS document is successfully received via email
        //Member Central has been updated with the new email
        /// </summary>
        [Test, Description("MCO IssuePolicy Comprehensive Annual CreditCard_FutureDatedMatchRule1")]
        [Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Spark), Category(TestCategory.Motorcycle),
            Category(TestCategory.Mock_Member_Central_Support), Category(TestCategory.InsuranceContactService), Category(TestCategory.SparkB2CRegressionForMemberCentralReleases)]
        public void INSU_T191_MCO_IssuePolicy_Comprehensive_Annual_CreditCard_FutureDatedMatchRule1()
        {
            var quoteInputs = BuildTestDataForIssuePolicyComprehensiveAnnualCreditCardFutureDatedMatchRule1();

            var policyNumber = ActionsQuoteMotorcycle.PurchaseMotorcyclePolicy(_browser, quoteInputs);

            VerifyPolicyMotorcycle.VerifyPolicyDetailsInShieldDB(policyNumber, quoteInputs);

            VerifyPolicyMotorcycle.VerifyUpdatedEmailAddressInMemberCentral(quoteInputs.Drivers[0].Details);
            //TODO AUNT-195 Add further confirmation of member information to match manual SPK-T14 test.
        }

        /// <summary>
        //To confirm a policy can be successfully issued for Monthly direct debit.
        //Also confirms:
        //Correct cover is retained
        //User can match against MC rule 3 in Tell us more about you
        //Authorisation message is correct
        //When the bank account already exists against the Shield contact a new bank account is not created
        //NOT AUTOMATED: CCS document is successfully received via email
        // This test is expected to fail when feature toggle "Use MC Mock" is toggled On, since MC Mock supports only Rule #1 at the moment
        /// </summary>
        [Test, Description("MCO IssuePolicy TPPD Monthly DirectDebit PurchaseFlowMatch Rule3")]
        [Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Spark), Category(TestCategory.Motorcycle)]
        public void INSU_T192_MCO_IssuePolicy_TPPD_Monthly_DirectDebit_PurchaseFlowMatch_Rule3()
        {
            if (_testConfig.IsMCMockEnabled())
            {
                Reporting.SkipLog("SPK_T17_MCO_IssuePolicy_TPPD_Monthly_DirectDebit_PurchaseFlowMatch_Rule3 skipped as MC Mock does not support Rule 3 match.");
            }
            var quoteInputs = BuildTestDataForIssuePolicyTPPDMonthlyDirectDebitMatchRule3Gold();

            ActionsQuoteMotorcycle.FetchNewMotorCycleQuote(_browser, quoteInputs);
            ActionsQuoteMotorcycle.VerifyCoverHelpTipAndGetQuoteNumber(_browser, quoteInputs);
            var policyNumber = ActionsQuoteMotorcycle.ProceedWithQuoteToPurchase(_browser, quoteInputs);
            
            VerifyPolicyMotorcycle.VerifyPolicyDetailsInShieldDB(policyNumber, quoteInputs);
            VerifyPaymentDetails.VerifyPolicyPaymentDetails(policyNumber, quoteInputs.PayMethod.Payer.BankAccounts[0]);
        }

        /// <summary>
        //To confirm the mapping for a quote with a match upfront from a contact in Member central
        //is correctly created in Shield and displays the correct premium for each cover.
        //Also confirms:
        //Quote summary display
        //Mapping for single match contact with Gold membership
        //Mapping for quote for UI inputs to Shield
        //Dynamic question values are mapped
        //Premium is correctly displayed in MCO
        //Checks member discount is displayed based on the Membership Tier
        //Checks Online discount is not displayed
        //Excess can be amended for Comprehensive damage
        //NOT AUTOMATED: User can update the contact and quote in Shield
        /// </summary>
        [Test, Description("MCO CreateQuote Upfront Single Match Gold Discount")]
        [Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Spark), Category(TestCategory.Motorcycle),
            Category(TestCategory.Mock_Member_Central_Support)]
        public void INSU_T189_MCO_CreateQuote_Upfront_Single_Match_Gold_Discount()
        {
            var quoteInputs = BuildTestDataForMCOCreateQuoteUpfrontSingleMatchGoldDiscount();

            ActionsQuoteMotorcycle.FetchNewMotorCycleQuote(_browser, quoteInputs);
            var quoteNumber = ActionsQuoteMotorcycle.VerifyCoverHelpTipAndGetQuoteNumber(_browser, quoteInputs);

            //Change Excess (expecting premiums to be updated)
            ActionsQuoteMotorcycle.AdjustQuoteParametersRemainOnQuotePage(_browser, quoteInputs);

            //Verify Quote details (To verify the updated premiums)
            Contact expectedContact = ShieldAPIVerification.BuildExpectedContact(Vehicle.Motorcycle, SparkBasePage.QuoteStage.AFTER_QUOTE, quoteInputs.Drivers[0].Details);

            VerifyPolicyMotorcycle.VerifyQuoteContactAndVehicleDetailsInShield(quoteInputs, quoteNumber, expectedContact, SparkBasePage.QuoteStage.AFTER_QUOTE);

        }

        /// <summary>
        //To confirm the mapping for a MFCO quote with a match upfront against rule 1
        //in Member central is correctly created in Shield and displays the correct premium for each cover.
        //Also confirms:
        //Mapping for single match contact with Silver membership
        //Mapping for quote for UI inputs in Shield is correct
        //Premium is correctly displayed in MCO
        //Excess and Sum insured can be amended for Comprehensive cover
        //Shield holds the updated excess and sum insured values
        /// </summary>
        [Test, Description("MCO CreateQuote Upfront Single Match MFCO Rule1 Silver Discount")]
        [Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Spark), Category(TestCategory.Motorcycle),
            Category(TestCategory.Mock_Member_Central_Support)]
        public void INSU_T190_MCO_CreateQuote_Upfront_Single_Match_MFCO_Rule1_Silver_Discount()
        {
            var quoteInputs = BuildTestDataForMCOCreateQuoteUpfrontSingleMatchMFCORule1SilverDiscount();

            ActionsQuoteMotorcycle.FetchNewMotorCycleQuote(_browser, quoteInputs);

            ActionsQuoteMotorcycle.AdjustQuoteParametersRemainOnQuotePage(_browser, quoteInputs);

            var quoteNumber = ActionsQuoteMotorcycle.SaveQuotePremiumDetailsAndGetQuoteNumber(_browser, quoteInputs);

            Contact expectedContact = ShieldAPIVerification.BuildExpectedContact(Vehicle.Motorcycle, SparkBasePage.QuoteStage.AFTER_QUOTE, quoteInputs.Drivers[0].Details);

            VerifyPolicyMotorcycle.VerifyQuoteContactAndVehicleDetailsInShield(quoteInputs, quoteNumber, expectedContact, SparkBasePage.QuoteStage.AFTER_QUOTE);
        }

        /// <summary>
        //To confirm when the cover selected in MCO is TFTP the quote is correct in Shield
        //Also confirms:
        //Premium is correctly displayed in MCO
        /// </summary>
        [Test, Description("MCO CreateQuote Upfront Single Match TFTP Gold Discount")]
        [Category(TestCategory.Regression), Category(TestCategory.New_Business), Category(TestCategory.Spark), Category(TestCategory.Motorcycle),
            Category(TestCategory.Mock_Member_Central_Support)]

        public void INSU_T324_MCO_CreateQuote_Upfront_Single_Match_TFTP_Gold_Discount()
        {
            var quoteInputs = BuildTestDataForMCOCreateQuoteUpfrontSingleMatchTFTPGoldMembership();

            ActionsQuoteMotorcycle.FetchNewMotorCycleQuote(_browser, quoteInputs);
            var quoteNumber = ActionsQuoteMotorcycle.VerifyCoverHelpTipAndGetQuoteNumber(_browser, quoteInputs);
            ActionsQuoteMotorcycle.SelectCover(_browser, quoteInputs);

            Contact expectedContact = ShieldAPIVerification.BuildExpectedContact(Vehicle.Motorcycle, SparkBasePage.QuoteStage.AFTER_QUOTE, quoteInputs.Drivers[0].Details);

            VerifyPolicyMotorcycle.VerifyQuoteContactAndVehicleDetailsInShield(quoteInputs, quoteNumber, expectedContact, SparkBasePage.QuoteStage.AFTER_QUOTE);
        }

        private QuoteMotorcycle BuildTestDataTest002()
        {
            // Setup test data
            var contactCandidate = ShieldContacts.FetchAContactWithRACMembershipTier(membershipTiers: new MembershipTier[] { MembershipTier.Gold,
                                                                                            MembershipTier.Silver,
                                                                                            MembershipTier.Bronze });
            var mainPH          = new ContactBuilder(contactCandidate).Build();
            var motorcycleQuote = new MotorCycleBuilder().InitialiseMotorCycleQuoteWithRandomData(mainPH, true)
                                                                    .WithRandomVehicle(minValue: 30000)
                                                                    .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.Log(mainPH.ToString());
            Reporting.Log(motorcycleQuote.ToString());

            return motorcycleQuote;
        }

        private QuoteMotorcycle BuildTestDataForIssuePolicyComprehensiveAnnualCreditCardFutureDatedMatchRule1()
        {
            // Setup test data
            var contactCandidate = ShieldContacts.FetchAContactWithRACMembershipTier(membershipTiers: new MembershipTier[] { MembershipTier.Gold,
                                                                                            MembershipTier.Silver,
                                                                                            MembershipTier.Bronze });

            var mainPH = new ContactBuilder(contactCandidate)
                .WithoutDeclaringMembership(true)
                //This test is supposed to use a different email address and
                //the following statement does that.
                .WithMemberMatchRule(MemberMatchRule.Rule1)
                .Build();
            var motorcycleQuote = new MotorCycleBuilder().InitialiseMotorCycleQuoteWithRandomData(mainPH, true)
                .WithAnnualPaymentFrequency()
                .WithCover(MotorCovers.MFCO)
                .WithoutFinancier()
                .WithIsModified(false)
                .WithIsGaraged(true)
                .WithTracker()
                .WithUsage(MotorcycleUsage.Private)
                .WithRandomVehicle(minValue: 30000) //Selecting a motorcycle which has a higher premium, so that when the gold/silver/bronze RSA member
                                                    //get a quote for a new motorcycle, the premium change popup will appear.
                .WithPaymentMethod(new Payment(mainPH).CreditCard().Annual())
                .WithIsPremiumChangeExpected(true)
                .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.Log(mainPH.ToString());
            Reporting.Log(motorcycleQuote.ToString());

            return motorcycleQuote;
        }

        private QuoteMotorcycle BuildTestDataForIssuePolicyTPPDMonthlyDirectDebitMatchRule3Gold()
        {
            // Setup test date
            var contactCandidate = ShieldContacts.FetchAContactWithRACMembershipTier(membershipTiers: MembershipTier.Gold);

            var mainPH = new ContactBuilder(contactCandidate)
                .WithoutDeclaringMembership(true)
                .WithMemberMatchRule(MemberMatchRule.Rule3)
                .Build();
            var motorcycleQuote = new MotorCycleBuilder().InitialiseMotorCycleQuoteWithRandomData(mainPH, true)
                .WithAnnualPaymentFrequency()
                .WithCover(MotorCovers.TPO)
                .WithoutFinancier()
                .WithIsModified(false)
                .WithIsGaraged(true)
                .WithTracker()
                .WithUsage(MotorcycleUsage.Private)
                .WithRandomVehicle(minValue: 30000) //Selecting a motorcycle which has a higher premium, so that when the gold RSA member
                                                    //get a quote for a new motorcycle, the premium change popup will appear.
                .WithPaymentMethod(new Payment(mainPH).BankAccount().Monthly())
                .WithIsPremiumChangeExpected(true) // Premium change/discount should be applied against the matched contact (mainPH) as we are expecting Rule 3 to be satisfied
                .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.Log(mainPH.ToString());
            Reporting.Log(motorcycleQuote.ToString());

            return motorcycleQuote;
        }

        private QuoteMotorcycle BuildTestDataForMCOCreateQuoteUpfrontSingleMatchGoldDiscount()
        {
            // Setup test date
            var contactCandidate = ShieldContacts.FetchAContactWithRACMembershipTier(membershipTiers: MembershipTier.Gold);

            var mainPH = new ContactBuilder(contactCandidate)
                .WithoutDeclaringMembership(false)
                //TODO: This test case is supposed to use Rule2, but untill the folowing PR is merged we have set as Rule1
                //https://bitbucket.org/racwa/dotnet_b2c_automation/pull-requests/222
                .WithMemberMatchRule(MemberMatchRule.Rule1)
                .Build();
            var motorcycleQuote = new MotorCycleBuilder().InitialiseMotorCycleQuoteWithRandomData(mainPH, true)
                .WithExcess("0")
                .WithoutFinancier()
                .WithUsage(MotorcycleUsage.Private)
                .WithPaymentMethod(new Payment(mainPH).BankAccount().Monthly())
                .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.Log(mainPH.ToString());
            Reporting.Log(motorcycleQuote.ToString());

            return motorcycleQuote;
        }

        private QuoteMotorcycle BuildTestDataForMCOCreateQuoteUpfrontSingleMatchTFTPGoldMembership()
        {
            // Setup test date
            var contactCandidate = ShieldContacts.FetchAContactWithRACMembershipTier(membershipTiers: MembershipTier.Gold);

            var mainPH = new ContactBuilder(contactCandidate)
                .WithoutDeclaringMembership(false)
                .WithMemberMatchRule(MemberMatchRule.Rule1)
                .WithRandomBankAccount()
                .WithRandomCreditCard()
                .Build();
            var vehicle = new MotorCycleBuilder().InitialiseMotorCycleQuoteWithRandomData(mainPH, true)
                .WithUsage(MotorcycleUsage.Private)
                .WithCover(MotorCovers.TFT)
                .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.Log(mainPH.ToString());
            Reporting.Log(vehicle.ToString());

            return vehicle;
        }

        private QuoteMotorcycle BuildTestDataForMCOCreateQuoteUpfrontSingleMatchMFCORule1SilverDiscount()
        {
            // Setup test date
            var contactCandidate = ShieldContacts.FetchAContactWithRACMembershipTier(membershipTiers: MembershipTier.Silver);
            var mainPH = new ContactBuilder(contactCandidate)
                .WithoutDeclaringMembership(false)
                .WithMemberMatchRule(MemberMatchRule.Rule1)
                .Build();
            var motorcycleQuote = new MotorCycleBuilder().InitialiseMotorCycleQuoteWithRandomData(mainPH, true)
                .WithExcess("0")
                .WithoutFinancier()
                .WithUsage(MotorcycleUsage.Private)
                .WithPaymentMethod(new Payment(mainPH).BankAccount().Monthly())
                .WithSumInsuredVariance(5)
                .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.Log(mainPH.ToString());
            Reporting.Log(motorcycleQuote.ToString());

            return motorcycleQuote;
        }
    }
}
