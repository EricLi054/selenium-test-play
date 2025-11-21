using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using System;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace Tests.ActionsAndValidations
{
    public static class VerifyQuoteCaravan
    {
        public const int CARAVAN_DEFAULT_ANNEXE_COVER = 3000;
        private const string CARAVAN_DEFAULT_REGISTRATION_NUMBER = "TBA";
        private const string CARAVAN_CAMPAIGN_CODE = "2000001";
        private const string CARAVAN_ONSITE_COVER_TYPE = "ACAO";
        private const string CARAVAN_CONTENT_COVER_TYPE = "AOCO";
        private const string CARAVAN_EMAIL_TYPE = "1";
        private const string CARAVAN_INSTALLMENT_NUMBER = "1";
        private const string CARAVAN_INSTALLMENT_TYPE = "1";

        /// <summary>
        /// This method uses the following Shield API calls to verify the quote parameters.
        /// 1. Build the expected contact
        /// 2. Get Quote API to verify quote details.
        /// 3. Person API to get Policyholder information.
        /// 4. Get Vehicle API to verify caravan details.
        /// </summary>
        /// <param name="testConfig"></param>
        /// <param name="quote"></param>
        public static void VerifyQuoteViaShieldAPI(QuoteCaravan quote, SparkBasePage.QuoteStage quoteStage)
        {
            Reporting.Log($"Verifying quote details in Shield {quoteStage}");
            Contact expectedContact = ShieldAPIVerification.BuildExpectedContact(Constants.PolicyGeneral.Vehicle.Caravan, quoteStage, quote.PolicyHolders[0]);
            var quoteDetailsFromAPI = DataHelper.GetQuoteDetails(quote.QuoteData.QuoteNumber);
            VerifyQuoteAndCoverDetailsViaShieldAPI(quote, quoteDetailsFromAPI, quoteStage);
            VerifyPolicy.VerifyPHDetailsWithAPIResponse(expectedContact, quoteDetailsFromAPI.Policyholder.Id.ToString(), quoteStage: quoteStage);
            VerifyCaravanDetailsViaShieldAPI(quote, quoteDetailsFromAPI);
        }

        private static void VerifyQuoteAndCoverDetailsViaShieldAPI(QuoteCaravan quote, GetQuotePolicy_Response quoteDetailsFromAPI, SparkBasePage.QuoteStage quoteStage)
        {
            //Verify quote breakdown
            if (quote.PayMethod.PaymentFrequency == PaymentFrequency.Annual)
            { Reporting.AreEqual(quote.QuoteData.AnnualPremium, quoteDetailsFromAPI.AnnualPremium.Total, "Total Annual Premium in Shield"); }
            else
            { Reporting.AreEqual(quote.QuoteData.MonthlyPremium, quoteDetailsFromAPI.Installments[0].Amount.Total, "Monthly Premium in Shield"); }

            Reporting.AreEqual(quote.QuoteData.PremiumBreakdownBasic, quoteDetailsFromAPI.AnnualPremium.BaseAmount, "Premium breakdown: Basic Amount in Shield");
            Reporting.AreEqual(quote.QuoteData.PremiumBreakdownStamp, quoteDetailsFromAPI.AnnualPremium.StampDuty, "Premium breakdown: Stamp Duty in Shield");
            Reporting.AreEqual(quote.QuoteData.PremiumBreakdownGST, quoteDetailsFromAPI.AnnualPremium.Gst, "Premium breakdown: GST in Shield");

            //Verify covers
            if (quote.ParkLocation == CaravanParkLocation.OnSite)
            { VerifyCoverInQuoteAPI("Caravan On-Site", quote.SumInsuredValue, quote.Excess.Value, quoteDetailsFromAPI); }
            else
            { VerifyCoverInQuoteAPI("Caravan Trailed", quote.SumInsuredValue, quote.Excess.Value, quoteDetailsFromAPI); }

            VerifyCoverInQuoteAPI("Caravan Contents", quote.ContentsSumInsured, quote.Excess.Value, quoteDetailsFromAPI);
            VerifyCoverInQuoteAPI("Caravan Annexe", CARAVAN_DEFAULT_ANNEXE_COVER, quote.Excess.Value, quoteDetailsFromAPI);

            //Verify Product Type, Status, Policy End Date and Proposal Valid Date
            Reporting.AreEqual(ShieldProductType.MGV, quoteDetailsFromAPI.ProductType, "Product Type in Shield");
            Reporting.AreEqual(Status.Proposal.GetDescription(), quoteDetailsFromAPI.Status, "Status in Shield");

            var expectedStartDate = quoteStage == SparkBasePage.QuoteStage.AFTER_QUOTE ?
                                    DateTime.Today : quote.StartDate;
            Reporting.AreEqual(expectedStartDate.AddYears(1).Date, quoteDetailsFromAPI.PolicyEndDate.Date, "Policy End Date in Shield");
            Reporting.AreEqual(expectedStartDate.Date, quoteDetailsFromAPI.PolicyStartDate.Date, "Policy Start Date in Shield");
            Reporting.AreEqual(DateTime.Now.AddDays(29).Date, quoteDetailsFromAPI.ProposalValidDate.Date, "Proposal Valid Date in Shield");

            //Verify Discount Group
            var contact = quote.PolicyHolders[0];
            if ((!contact.SkipDeclaringMembership) || (contact.SkipDeclaringMembership && (quoteStage == SparkBasePage.QuoteStage.AFTER_PERSONAL_INFO)))
            { Reporting.AreEqual(APIDiscountCode[quote.GetHighestTier()], quoteDetailsFromAPI.DiscountGroup, "Discount Group in Shield"); }
            else
            { Reporting.AreEqual(APIDiscountCode[MembershipTier.None], quoteDetailsFromAPI.DiscountGroup, "Discount Group in Shield"); }
        }

        /// <summary>
        /// Verifies if the given sum insured and exces values are found under the corresponding cover name,
        /// for the given API message text.
        /// </summary>
        /// <param name="coverName"></param>
        /// <param name="expectedSI"></param>
        /// <param name="expectedExcess"></param>
        /// <param name="apiMessage"></param>
        private static void VerifyCoverInQuoteAPI(string coverName, decimal expectedSI, decimal expectedExcess, GetQuotePolicy_Response apiMessage)
        {
            var matchingCover = apiMessage.Covers.FirstOrDefault(x => x.CoverTypeDescription.Equals(coverName, StringComparison.InvariantCultureIgnoreCase));
            Reporting.IsTrue(matchingCover != null, "Matching cover name was found in the API response");
            Reporting.AreEqual(expectedSI, matchingCover?.SumInsured, $"Sum insured for {coverName} cover is set to ${expectedSI}");
            Reporting.AreEqual(expectedExcess, matchingCover?.StandardExcess, $"Excess for {coverName} cover is set to ${expectedExcess}");
        }

        private static void VerifyCaravanDetailsViaShieldAPI(QuoteCaravan quote, GetQuotePolicy_Response quoteDetailsFromAPI)
        {
            Reporting.AreEqual(VehicleUsage.Private.ToString(), quoteDetailsFromAPI.CaravanAsset.VehicleUsage, "Vehicle Usage in Shield");
            Reporting.AreEqual(quote.ParkingAddress.Suburb, quoteDetailsFromAPI.CaravanAsset.Suburb, true);
            Reporting.IsFalse(quoteDetailsFromAPI.CaravanAsset.IsFinanced, "IsFinanced in Shield is not supposed to set to TRUE");

            var vehicleDetailsFromShieldAPI = DataHelper.GetVehicleDetails(quoteDetailsFromAPI.CaravanAsset.VehicleId);

            Reporting.AreEqual(ShieldMainVehicleTypeDesc.CARAVAN.GetDescription(), vehicleDetailsFromShieldAPI.Vehicles[0].MainVehicleTypeDescription, "Main Vehicle Type Description in Shield");
            Reporting.AreEqual(quote.Make, vehicleDetailsFromShieldAPI.Vehicles[0].MakeDescription, "Caravan Make in Shield");

            var modelFromShieldAPI = vehicleDetailsFromShieldAPI.Vehicles[0].ModelFamily == null ? vehicleDetailsFromShieldAPI.Vehicles[0].ModelDescription : vehicleDetailsFromShieldAPI.Vehicles[0].ModelFamily;

            Reporting.AreEqual(quote.Model, modelFromShieldAPI, "Caravan Model in Shield");
            Reporting.AreEqual(quote.Year, vehicleDetailsFromShieldAPI.Vehicles[0].ModelYear, "Caravan Year in Shield");
        }

        public static void VerifyCaravanPolicyInShield(Browser browser, QuoteCaravan caravanQuote, string policyNumber)
        {
            CaravanPolicy policyInfo = ShieldCaravanDB.FetchCaravanPolicyDetail(policyNumber, caravanQuote.Type);
            VerifyCaravanPolicyInShieldBasicCoverDetails(caravanQuote, policyInfo);
            VerifyCaravanPolicyPaymentDetailsInShield(browser, caravanQuote, policyNumber, policyInfo);
            VerifyPolicy.VerifyPolicyMultiMatchDetailsInShield(caravanQuote.PolicyHolders, policyNumber);
        }

        private static void VerifyCaravanPolicyInShieldBasicCoverDetails(QuoteCaravan caravanQuote, CaravanPolicy policyInfo)
        {

            // Verify General Policy details
            Reporting.AreEqual(caravanQuote.ParkLocation.GetDescription(), policyInfo.CaravanPolicyOutput.CaravanLocation, ignoreCase: true, "Caravan parking location in Shield DB");
            Reporting.AreEqual(CARAVAN_DEFAULT_REGISTRATION_NUMBER, policyInfo.CaravanPolicyOutput.RegistrationValue, "Caravan registration number in Shield DB");
            Reporting.AreEqual(CARAVAN_CAMPAIGN_CODE, policyInfo.CaravanPolicyOutput.Campaign_Code, "Caravan campaign code in Shield DB");

            Reporting.IsNull(policyInfo.CaravanPolicyOutput.NCBLevel, "Caravan NCB level should be null for policies bought after August 2024");

            if (caravanQuote.ParkLocation == CaravanParkLocation.OnSite)
            {
                Reporting.AreEqual(CARAVAN_ONSITE_COVER_TYPE, policyInfo.CaravanPolicyInput.CoverType, "Caravan cover type in Shield DB");
            }
            else
            {
                Reporting.AreEqual(CARAVAN_CONTENT_COVER_TYPE, policyInfo.CaravanPolicyInput.CoverType, "Caravan cover type in Shield DB");
            }
            
            Reporting.IsTrue(policyInfo.CaravanPolicyInput.CaravanModel.ToUpper().Contains(caravanQuote.Model.ToUpper()), $" ({policyInfo.CaravanPolicyInput.CaravanModel}) to match or contain model {caravanQuote.Model}");
            Reporting.AreEqual(CARAVAN_EMAIL_TYPE,  policyInfo.CaravanPolicyOutput.EmailType, "Email Type from Shield DB");            
        }

        private static void VerifyCaravanPolicyPaymentDetailsInShield(Browser browser, QuoteCaravan caravanQuote, string policyNumber, CaravanPolicy policyInfo)
        {
            Reporting.Log($"Begin verify policy payment details from Shield DB. Param/s = {policyNumber}");
            Reporting.AreEqual(caravanQuote.StartDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
                               policyInfo.PolicyStartDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), "Policy Start date");
            Reporting.AreEqual(caravanQuote.PayMethod.IsPaymentByBankAccount, policyInfo.CaravanPolicyInput.IsDirectDebit, "Payment method by bank account");
            Reporting.AreEqual(caravanQuote.PayMethod.PaymentFrequency, policyInfo.CaravanPolicyInput.PaymentFrequency, "Policy Payment Frequency from Shield DB");
            Reporting.AreEqual(CARAVAN_INSTALLMENT_NUMBER, policyInfo.CaravanPolicyOutput.InstallmentNumber, "Policy installment number in Shield DB");

            // Default to expecting bank account installment type
            var expectedPaymentScenario = caravanQuote.PayMethod.Scenario;
            var expectedCollectionMethod = PaymentScenarioIdMappings[expectedPaymentScenario].CollectionMethod;
            var expectedPaymentTerms = PaymentScenarioIdMappings[expectedPaymentScenario].PaymentTerm;

            Reporting.AreEqual(CARAVAN_INSTALLMENT_TYPE, policyInfo.CaravanPolicyOutput.InstallmentType, "Policy installment type in Shield DB");
            Reporting.AreEqual(expectedCollectionMethod, policyInfo.CaravanPolicyOutput.PolicyCollectionMethod, "Policy collection method in Shield DB");
            Reporting.AreEqual(expectedCollectionMethod, policyInfo.CaravanPolicyOutput.InstallmentCollectionMethod, "Policy installment collection method in Shield DB");

            Reporting.AreEqual(expectedPaymentTerms, policyInfo.CaravanPolicyOutput.PolicyPaymentTerms, "Policy payment terms in Shield DB");
            Reporting.AreEqual(expectedPaymentTerms, policyInfo.CaravanPolicyOutput.InstallmentPaymentTerms, "Policy installment payment terms in Shield DB");

            if (caravanQuote.PayMethod.IsAnnual)
            {
                Reporting.AreEqual(caravanQuote.QuoteData.AnnualPremium, policyInfo.PremiumAmount, "Total Premium Amount from Shield DB");
            }
            else
            {
                Reporting.AreEqual(caravanQuote.QuoteData.MonthlyPremium, policyInfo.InstallmentAmount, "First Installment Amount from Shield DB");
            }

            var recentEvents = ShieldPolicyDB.FetchRecentEventsOnPolicy(policyNumber);

            if (caravanQuote.PayMethod.Scenario == PaymentScenario.AnnualCash)
            {
                VerifyPolicy.VerifyNewBusinessEventForAnnualCashPayment(recentEvents);
            }
            else
            {
                VerifyPolicy.VerifyNewBusinessEventForDirectDebitInstalmentPayment(recentEvents);
            }

            VerifyPolicy.VerifyNewPolicyInstalments(policyNumber);
        }
    }
}
