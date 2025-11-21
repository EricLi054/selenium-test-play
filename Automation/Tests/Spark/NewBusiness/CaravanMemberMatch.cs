using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using System.Collections.Generic;
using UIDriver.Pages.Spark.CaravanQuote;
using Rac.TestAutomation.Common.DatabaseCalls.Contacts;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;


namespace Spark.NewBusiness
{
    [Property("Functional", "Caravan tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class CaravanMemberMatch : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Mandatory regression tests for Spark Caravan new business.");         
        }

        #region Test Cases
        
        /// <summary>
        /// Test emulates the workflow of a member, getting a Caravan Policy via Spark Caravan:
        /// 1. Member matched upfront on 'Page 1: Are you an RAC Member'
        /// 2. Member matched as Gold Member using Rule 1 (First name, Date of birth and Mobile)
        /// 3. Verify 'Discount Toaster' display for Gold membership.
        /// 4. Verify if online discount icons are displayed correctly in quote page.
        /// 5. Get Quote.
        /// 6. Verify if 'Tell us more about you' page displays only the Mailing address input.
        /// 7. Verify the Quote (including quote breakdown), Cover, Policyholder and Caravan details in Shield.
        /// 8. Purchase the policy
        /// 9. Verify the policy details in Shield
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark)]
        [Test(Description = "Caravan: Upfront Single Match: Rule1: Gold discount: Create Quote")]
        public void INSU_T322_Caravan_PH1SingleGold_Policy()
        {
            var quoteInputs = BuildTestDataUpfrontSingleMatchRule1GoldDiscountCreateQuote();

            Reporting.LogTestStart();
            ActionsQuoteCaravan.CreateNewCaravanQuote(_browser, quoteInputs);
            ActionsQuoteCaravan.ProceedWithQuoteToPurchase(_browser, quoteInputs);
            string policyNumber = ActionsQuoteCaravan.GetPolicyNumberFromConfirmationPage(_browser);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteCaravan.VerifyCaravanPolicyInShield(_browser, quoteInputs, policyNumber);
        }
        /// <summary>
        /// Test emulates the workflow of a single match silver member along with a single match Gold joint policyholder, getting a Caravan Policy via Spark Caravan:
        /// 1. Member matched upfront on 'Page 1: Are you an RAC Member' as a Silver member
        /// 2. Member matched as Silver Member using Rule 1 (First name, Date of birth and Mobile)
        /// 3. Verify 'Discount Toaster' display for Silver membership.
        /// 4. Verify if online discount icons are displayed correctly in quote page.
        /// 5. Get Quote.
        /// 6. Verify if 'Tell us more about you' page displays only the Mailing address input.
        /// 7. Verify if 'Tell us more about your joint policyholder' page displays.
        /// 8. Enter a Gold member joint policyholder details.
        /// 9. A premium change popup should get triggered, since the Joint policyholder is having a higher discount tier than the main policyholder.
        /// 10. Verify the Quote (including quote breakdown), Cover, Policyholder and Caravan details in Shield.
        /// 11. Purchase the policy
        /// 12. Verify the policy details in Shield
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark)]
        [Test(Description = "Caravan: Main PH Upfront Single Match: Rule1: Silver discount: Joint PH Gold discount: Issue Policy")]
        public void INSU_T318_Caravan_PH1SingleSilver_PH2SingleGold_Policy()
        {
            var quoteInputs = BuildTestDataMainPHUpfrontSingleMatchRule1SilverDiscountJointPHGoldMatchIssuePolicy();

            Reporting.LogTestStart();
            ActionsQuoteCaravan.CreateNewCaravanQuote(_browser, quoteInputs);
            ActionsQuoteCaravan.ProceedWithQuoteToPurchase(_browser, quoteInputs);
            string policyNumber = ActionsQuoteCaravan.GetPolicyNumberFromConfirmationPage(_browser);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteCaravan.VerifyCaravanPolicyInShield(_browser, quoteInputs, policyNumber);
        }
        /// <summary>
        /// Test emulates the workflow of a single match Gold member along with a single match silver joint policyholder,  who fails to declare the membership on Page 1 (Are you an RAC member):
        /// gets Single Matched with Rule 1 (First name, Date of birth and Mobile), on 'Tell us more about you' page, and receives applicable discounts.
        /// 1. Click NO to Are you a RAC member
        /// 2. Obtain a Caravan Quote via Spark Caravan as an Anonymous user.
        /// 3. Enter Single Matched  Gold on 'Tell us more about you' page.
        /// 4. Enter Single Matched  Silver on 'Tell us more about your joint policy holder' page.
        /// 5. Verify that the price reduction pop-up discount is Gold
        /// 6. Verify the Quote (including quote breakdown), Cover, Policyholder and Caravan details in Shield.
        /// 6. Purchase the policy
        /// 7. Verify the policy details in Shield
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark), Category(TestCategory.Smoke)]
        [Test(Description = "Caravan: Upfront membership not declared: Create Policy: Annual Frequency: Premium change based on Single Match")]
        public void Caravan_PH1SingleGold_PH2SingleSilver_Policy()
        {
            var quoteInputs = BuildTestDataPurchaseFlowPH1SingleMatchGoldPH2SingleMatchSilver();

            Reporting.LogTestStart();
            ActionsQuoteCaravan.CreateNewCaravanQuote(_browser, quoteInputs);
            ActionsQuoteCaravan.ProceedWithQuoteToPurchase(_browser, quoteInputs);
            string policyNumber = ActionsQuoteCaravan.GetPolicyNumberFromConfirmationPage(_browser);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteCaravan.VerifyCaravanPolicyInShield(_browser, quoteInputs, policyNumber);
        }
        /// <summary>
        /// Test emulates the workflow of a Multi Match silver member along with a single Gold joint policyholder, who fails to declare the membership on Page 1 (Are you an RAC member),
        /// gets Multi Matched on 'Tell us more about you' page, and prompted with membership tier selection in the next page.
        /// 1. Click NO to Are you a RAC member
        /// 2. Obtain a Caravan Quote via Spark Caravan as an Anonymous user.
        /// 3. Enter Multi Matched on 'Tell us more about you' page.
        /// 4. Verify that One more question is displayed
        /// 5. Select Silver on (What RAC membership level is PH1 name) 
        /// 6. Enter Single Matched  Gold on 'Tell us more about your joint policy holder' page.
        /// 7. Verify that the price reduction pop-up discount is Gold
        /// 8. Verify the Quote (including quote breakdown), Cover, Policyholder and Caravan details in Shield.
        /// 9. Purchase the policy
        /// 10. Verify the policy details in Shield
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark), Category(TestCategory.Regression), Category(TestCategory.InsuranceContactService)]
        [Test(Description = "Caravan: Upfront membership not declared: Create Policy: Monthly Frequency: Premium change based on Silver Multi Match")]
        public void INSU_T149_Caravan_PH1MultiSilver_PH2SingleGold_Policy()
        {
            var quoteInputs = BuildTestDataPurchaseFlowPH1MultiMatchSilverPH2SingleMatchGold();

            Reporting.LogTestStart();
            ActionsQuoteCaravan.CreateNewCaravanQuote(_browser, quoteInputs);
            ActionsQuoteCaravan.ProceedWithQuoteToPurchase(_browser, quoteInputs);
            string policyNumber = ActionsQuoteCaravan.GetPolicyNumberFromConfirmationPage(_browser);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteCaravan.VerifyCaravanPolicyInShield(_browser, quoteInputs, policyNumber);
        }
        /// <summary>
        /// This test emulates the workflow of:
        /// PH1: With Membership discount | Caravan Market Value : > 30000 | Excess: 0 | Content Insured: 15000
        /// 1. Before we get started:                   YES
        /// 2. Let's Start with your caravan:           Random caravan with market value >= 30000
        /// 3. Tell us more about your <Make>
        /// 4. Now, a bit about the policyholders:      PH1 >= 16 and <= 49 PH2 : >= 16 and <= 49 
        /// 5. Here's your quote:                       Payment Frequency: Monthly | Excess: 0 | Content Insured: 15000
        /// 6. Retrieve Quote
        /// 7: Verify Here's your quote Values
        /// 8. Purchase the policy
        /// 9. Verify the policy details in Shield
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark)]
        [Test(Description = "Caravan: Upfront membership: Create And Retrieve Quote:Membership: Gold | Payment Frequency: Monthly | Excess: 0 | Content insured : 15000| caravan Value >= 30000 | PH1 & PH2 below 50")]        
        public void INSU_T321_Caravan_MemberWithDiscount_RetrieveQuote_Monthly_ContentInsured15000()
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

        /// <summary>
        /// Test emulates the workflow of a gold member, getting a Caravan Policy via Spark Caravan No upfront member match:
        /// 1. Member not matched upfront on 'Page 1: Are you an RAC Member' - Select No
        /// 2. Get Quote.
        /// 3. Verify if 'Tell us more about you' page displays.
        /// 4. Member matched as Gold Member using Rule 1 (First name, Date of birth and Mobile)
        /// 5. Verify you receive a discount pop up 
        /// 6. Verify the Quote (including quote breakdown), Cover, Policyholder and Caravan details in Shield.
        /// 7. Purchase the policy
        /// 8. Verify the policy details in Shield
        /// </summary>
        [Category(TestCategory.New_Business), Category(TestCategory.Caravan), Category(TestCategory.Spark), Category(TestCategory.Regression)]
        [Test(Description = "Caravan: No Upfront Single Match: Rule1: Gold discount: Policy")]
        public void INSU_T148_Caravan_NotUpfrontSingleMatch_Rule1_SilverDiscount()
        {
            var quoteInputs = BuildTestDataNoUpfrontSingleMatchRule1SilverDiscountCreateQuote();

            Reporting.LogTestStart();
            ActionsQuoteCaravan.CreateNewCaravanQuote(_browser, quoteInputs);
            ActionsQuoteCaravan.ProceedWithQuoteToPurchase(_browser, quoteInputs);
            string policyNumber = ActionsQuoteCaravan.GetPolicyNumberFromConfirmationPage(_browser);

            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyQuoteCaravan.VerifyCaravanPolicyInShield(_browser, quoteInputs, policyNumber);
        }
        #endregion

        #region Test cases helper methods        

        private QuoteCaravan BuildTestDataUpfrontSingleMatchRule1GoldDiscountCreateQuote()
        {
            var mainPH = CreateMemberMatchContact(matchRule: MemberMatchRule.Rule1, true,
                                                             membershipTiers: new MembershipTier[] { MembershipTier.Gold });
            var caravanQuote = new CaravanBuilder()
                .InitialiseCaravanWithRandomData(new List<Contact>() { mainPH })
                .WithRandomCaravan(minValue: 30000)
                .Build();
            
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, caravanQuote.ToString());

            return caravanQuote;
        }
        private QuoteCaravan BuildTestDataMainPHUpfrontSingleMatchRule1SilverDiscountJointPHGoldMatchIssuePolicy()
        {
            var mainPH = CreateMemberMatchContact(matchRule: MemberMatchRule.Rule1, true,
                                                             membershipTiers: new MembershipTier[] { MembershipTier.Silver });
            var jointPH = CreateMemberMatchContact(matchRule: MemberMatchRule.Rule1, false,
                                                             membershipTiers: new MembershipTier[] { MembershipTier.Gold });
            var caravanQuote = new CaravanBuilder()
                .InitialiseCaravanWithRandomData(new List<Contact>() { mainPH, jointPH })
                .WithRandomCaravan(minValue: 30000)
                .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, caravanQuote.ToString());

            return caravanQuote;
        }
        private QuoteCaravan BuildTestDataPurchaseFlowPH1SingleMatchGoldPH2SingleMatchSilver()
        {
            var mainPH = CreateMemberMatchContact(matchRule: MemberMatchRule.Rule1, false,
                                                             membershipTiers: new MembershipTier[] { MembershipTier.Gold });
            var joinPH = CreateMemberMatchContact(matchRule: MemberMatchRule.Rule1, false,
                                                             membershipTiers: new MembershipTier[] { MembershipTier.Silver });
            var caravanQuote = new CaravanBuilder()
                                                .InitialiseCaravanWithRandomData(new List<Contact>() { mainPH, joinPH })
                                                .WithRandomCaravan(minValue: 30000) //Increasing the minimum caravan value to a significant value, so that we can expect a premium change
                                                .Build();           

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, caravanQuote.ToString());

            return caravanQuote;
        } 
        private QuoteCaravan BuildTestDataPurchaseFlowPH1MultiMatchSilverPH2SingleMatchGold()
        {
            var mainPH = CreateMultiMatchContact(declareMembership: true, MembershipTier.Silver);
            var joinPH = CreateMemberMatchContact(matchRule: MemberMatchRule.Rule1, false,
                                                             membershipTiers: new MembershipTier[] { MembershipTier.Gold });

            var caravanQuote = new CaravanBuilder()
                                                .InitialiseCaravanWithRandomData(new List<Contact>() { mainPH, joinPH })
                                                .WithRandomCaravan(minValue: 30000) //Increasing the minimum caravan value to a significant value, so that we can expect a premium change
                                                .Build();           

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, caravanQuote.ToString());

            return caravanQuote;
        }
        private QuoteCaravan BuildTestDataWith0ExcessAndContentInsured15000Monthly()
        {
            var mainPH = CreateMemberMatchContact(
                                 MemberMatchRule.Rule1,
                                 true,
                                 new MembershipTier[] {MembershipTier.Gold, MembershipTier.Silver, MembershipTier.Bronze});
             
             var jointPH = new ContactBuilder().InitialiseRandomIndividual()
                                .WithRandomDateOfBirth(MIN_PH_AGE_VEHICLES, MIN_AGE_FOR_EXCESS_WAIVER - 1).Build();

            var caravanQuote = new CaravanBuilder()
                                .InitialiseCaravanWithRandomData(new List<Contact>() { mainPH, jointPH })
                                .WithRandomCaravan(minValue: 30000)
                                .WithPaymentMethod(new Payment(mainPH).Monthly())
                                .WithExcess(CARAVAN_MIN_EXCESS_VALUE)
                                .WithContentsCoverValue(MAX_CONTENT_INSURANCE_VALUE)
                                .WithRetrieveQuote(RetrieveQuoteType.Website)
                                .Build();           

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, caravanQuote.ToString());

            return caravanQuote;
        }

        private QuoteCaravan BuildTestDataNoUpfrontSingleMatchRule1SilverDiscountCreateQuote()
        {
            var mainPH = CreateMemberMatchContact(           matchRule: MemberMatchRule.Rule1, false,
                                                             membershipTiers: new MembershipTier[] { MembershipTier.Silver });
            var caravanQuote = new CaravanBuilder()
                .InitialiseCaravanWithRandomData(new List<Contact>() { mainPH })
                .WithRandomCaravan(minValue: 30000)
                .Build();

            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, caravanQuote.ToString());

            return caravanQuote;
        }
        /// <summary>
        /// Creates a policyHolder contact with the given:
        /// 1. matchRule: Member match Rule (e.g:-MemberMatchRule.Rule1)
        /// 2. declareMembership:   true:   if member is selecting "Yes" for the "Are you an RAC member question"
        ///                         false:  if memeber is selecting "No" for the "Are you an RAC member question"
        /// 3. membershipTiers: One or more tiers: Gold/Silver/Bronze etc.. (e.g:- new[] {MembershipTier.Gold, MembershipTier.Silver, MembershipTier.Bronze})                         
        /// </summary>
        public Contact CreateMemberMatchContact(MemberMatchRule matchRule, bool declareMembership, params MembershipTier[] membershipTiers)
        {
            var contactCandidate = ShieldContacts.FetchAContactWithRACMembershipTier(membershipTiers: membershipTiers);
            return new ContactBuilder(contactCandidate)
                                                               .WithoutDeclaringMembership(!declareMembership)
                                                               .WithMemberMatchRule(matchRule)
                                                               .Build();
        }

        /// <summary>
        /// Creates a policyHolder Multi Match contact with the given:
        /// </summary>
        /// <param name="declareMembership">TRUE; if member is selecting "Yes" for the "Are you an RAC member question", FALSE; if member is selecting "No" for the "Are you an RAC member question"</param>
        /// <param name="membershipTier">One or more tiers: Gold/Silver/Bronze etc.. (e.g:- new[] {MembershipTier.Gold, MembershipTier.Silver, MembershipTier.Bronze})</param>
        public Contact CreateMultiMatchContact(bool declareMembership, MembershipTier membershipTier)
        {
            var requireDiscountTier = membershipTier == MembershipTier.Bronze ||
                                      membershipTier == MembershipTier.Silver ||
                                      membershipTier == MembershipTier.Gold;
            var policyHolder = new ContactBuilder().InitialiseRandomIndividual()
                                                   .WithMultiMatchContact(withDiscountTier: requireDiscountTier)
                                                   .WithMembershipTier(membershipTier)
                                                   .WithoutDeclaringMembership(!declareMembership)
                                                   .Build();
            return policyHolder;
        }
        #endregion
    }
}
