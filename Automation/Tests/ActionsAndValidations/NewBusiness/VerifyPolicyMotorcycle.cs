using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using System;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;
using static Rac.TestAutomation.Common.SparkBasePage;

namespace Tests.ActionsAndValidations
{
    public static class VerifyPolicyMotorcycle
    {
        public const string QUOTE_SECURITY_IMMOBILISER_IN_SHIELD = "Immobiliser";

        public static void VerifyPolicyDetailsInShieldDB(string policyNumber, QuoteMotorcycle policyData)
        {
            Reporting.Log($"Begin verify policy details from Shield DB for: {policyNumber}");
            var policyInfo = ShieldMotorDB.FetchMotorPolicyDetail(policyNumber);

            Reporting.AreEqual(int.Parse(policyData.Excess), policyInfo.Excess, "Policy Excess value in Shield");
            if (policyData.CoverType != MotorCovers.TPO)
            {
                Reporting.AreEqual(policyData.SumInsuredFromQuotePage, policyInfo.SumInsured, "Policy Sum Insured value in Shield");
            }
            if (policyData.PayMethod.IsAnnual)
            {
                Reporting.AreEqual(policyData.PremiumAnnualFromQuotePage, policyInfo.AnnualPremium, "Policy Annual Premium value in Shield");
            }

            VerifyMotorcyclePolicyInShieldBasicCoverDetails(policyData, policyNumber);
            VerifyMotorcyclePolicyInShieldPaymentDetails(policyData, policyNumber);
            VerifyPolicy.VerifyPolicyMultiMatchDetailsInShield(policyData.Drivers, policyNumber);
        }

        public static void VerifyMotorcyclePolicyInShieldBasicCoverDetails(QuoteMotorcycle quoteInputs, string policyNumber)
        {
            Reporting.Log($"Begin verify policy basic cover details from Shield DB for: {policyNumber}");
            var policyInfo = ShieldMotorDB.FetchMotorPolicyDetail(policyNumber);
            // Using "StartsWith()" as March 2019 product version has 'cloned' text in cover description
            Reporting.IsTrue(policyInfo.CoverType.StartsWith(MotorcycleCoverNameMappings[quoteInputs.CoverType].TextShield), "Verify Policy Cover Type in Shield DB");

            Reporting.AreEqual(quoteInputs.StartDate.ToString("d/MMM/yy"), policyInfo.PolicyStartDate.ToString("d/MMM/yy"));

            var policyDetails = DataHelper.GetPolicyDetails(policyNumber);
            Reporting.AreEqual(quoteInputs.IsFinanced, policyDetails.MotorcycleAsset.IsFinanced, "IsFinanced flag correctly set in Shield");

            if (quoteInputs.IsFinanced)
            {
                Reporting.AreEqual(quoteInputs.Financier, policyDetails.GetFinancierNameViaShieldAPI(), ignoreCase: true, $"Expected financier name against the actual name displayed");
            }
        }

        public static void VerifyMotorcyclePolicyInShieldPaymentDetails(QuoteMotorcycle quoteInputs, string policyNumber)
        {
            Reporting.Log($"Begin verify payment details from Shield DB for {policyNumber}");
            var dbPaymentDetails = ShieldPolicyDB.FetchPaymentDetailsForPolicy(policyNumber);

            Reporting.IsTrue(quoteInputs.PayMethod.PaymentFrequency == dbPaymentDetails.PaymentFrequency, "Payment frequency matched the Shield DB value");

            if (quoteInputs.PayMethod.IsPaymentByBankAccount)
                Reporting.AreEqual(DIRECT_DEBIT, dbPaymentDetails.PaymentMethod, "Payment method Direct Debit, matched the Shield DB value");
            else
            {
                if (quoteInputs.PayMethod.IsAnnual)
                    Reporting.AreEqual("Cash", dbPaymentDetails.PaymentMethod, "Payment method Cash, matched the Shield DB value");
                else
                    Reporting.AreEqual("Credit card", dbPaymentDetails.PaymentMethod, "Payment method Credit card, matched the Shield DB value");
            }

            var expectedPaymentFrequency = quoteInputs.PayMethod.NumberOfPayments;
            Reporting.AreEqual(expectedPaymentFrequency, dbPaymentDetails.PaymentCount, "Count of payments in Shield DB is the expected value");
        }

        /// <summary>
        /// Verification of a policy contact in Member Central. This is to ensure that details
        /// entered for a contact (whether new or pre-existing) have been correctly sync-ed with
        /// Member Central. However, as this sync is not always immediate, this method will retry
        /// for a period of time if a match is not found, or a key data item like email, is not
        /// as expected.
        /// </summary>
        /// <param name="policyHolder">The contact details from the test, that we are expecting to find in Member Central</param>
        public static void VerifyUpdatedEmailAddressInMemberCentral(Contact policyHolder)
        {
            API_MemberCentralPersonV2Response mcMatch = null;
            bool emailMatched = false;
            var waitTime = WaitTimes.T150SEC; // Using a variable as we put this time into some logging as well.
            var endtime = DateTime.Now.AddSeconds(waitTime);

            if (Config.Get().IsMCMockEnabled())
            {
                Reporting.Log($"PLEASE NOTE: This test run is using MC Mock in place of Member Central.");
            }

            if (Config.Get().Azure.MemberCentral.HasLiveSync)
            {
                do
                {
                    Thread.Sleep(10000);

                    var mcApi = MemberCentral.GetInstance();
                    mcMatch = mcApi.GET_MemberByMemberMatch(policyHolder).GetAwaiter().GetResult();

                    if (mcMatch == null)
                    { Reporting.Log($"Checking in Member Central, did not find member. May be new contact that has not yet synced, will wait before retry."); }
                    else if (mcMatch.PersonalEmailAddress == null)
                    { Reporting.Log($"Checking in Member Central, member has no private email address, will wait before retry."); }

                    else if (mcMatch.PersonalEmailAddress.Equals(policyHolder.PrivateEmail.Address, StringComparison.InvariantCultureIgnoreCase))
                    {
                        emailMatched = true;
                        break;
                    }
                    else
                    { Reporting.Log($"Checking in Member Central found member, but the email is different. Will allow time for sync. Expected {policyHolder.PrivateEmail.Address} but got {mcMatch.PersonalEmailAddress}"); }

                } while (DateTime.Now < endtime);

                Reporting.IsNotNull(mcMatch, $"found a contact in Member Central within {waitTime} seconds");

                if (emailMatched)
                    Reporting.Log($"Private Email Address '{policyHolder.PrivateEmail.Address.ToLower()}' matched '{mcMatch.PersonalEmailAddress.ToLower()}' in Member Central within {waitTime} seconds");
                else
                {
                    Reporting.IsNotNull(mcMatch, $"revealed that the returned Member Central record Private Email Address in MC did not match the expected address provided by this test.");
                    Reporting.AreEqual(policyHolder.PrivateEmail.Address.ToLower(), mcMatch.PersonalEmailAddress.ToLower());
                }
            }
        }

        /// <summary>
        /// This method Verifies the following Quote Details against the Shield DB (via the Shield APIs)
        /// -Quote parameters like 'Status', Premiums etc
        /// -Driver details like Name, DOB, Gender etc
        /// -Vehicle details (Description, Make, Model and Year)
        /// This method is intended to be used for the following scenarios
        /// -For Comprehensive or TFT cover types
        /// -Have not yet progressed past the "Tell Us More About You" page, hence the policy is in Proposal state
        /// </summary>
        /// <param name="quoteInputs"></param>
        /// <param name="quoteNumber"></param>
        /// <param name="quoteStage"></param>
        public static void VerifyQuoteContactAndVehicleDetailsInShield(QuoteMotorcycle quoteInputs, string quoteNumber, Contact expectedContact, QuoteStage quoteStage)
        {
            var quoteDetailsFromShieldAPI = VerifyQuoteDetailsInShield(quoteInputs, quoteNumber);

            // Should always exclude mailing address as we're dealing with quote stage at this point,
            // so no detailed PH details should have been entered.
            VerifyPolicy.VerifyPHDetailsWithAPIResponse(expectedContact, quoteDetailsFromShieldAPI.Policyholder.Id.ToString(), includeMailingAddress: false, quoteStage: quoteStage);

            VerifyVehicleDetailsInShield(quoteInputs, quoteDetailsFromShieldAPI);
        }

        private static GetQuotePolicy_Response VerifyQuoteDetailsInShield(QuoteMotorcycle quoteInputs, string quoteNumber)
        {
            var quoteDetailsFromShieldAPI = DataHelper.GetQuoteDetails(quoteNumber);

            VerifyQuoteMotorcycleAssetDetailsInShield(quoteInputs, quoteDetailsFromShieldAPI);

            Reporting.IsTrue(quoteDetailsFromShieldAPI.Covers[0].CoverTypeDescription.StartsWith(MotorcycleCoverNameMappings[quoteInputs.CoverType].TextShield), "Cover Type in Shield");
            Reporting.AreEqual(Status.Proposal.GetDescription(), quoteDetailsFromShieldAPI.Status, "Status in Shield");
            Reporting.AreEqual(quoteInputs.PremiumAnnualFromQuotePage, quoteDetailsFromShieldAPI.AnnualPremium.Total, "Annual Premium in Shield");
            Reporting.AreEqual(quoteInputs.SumInsuredFromQuotePage, quoteDetailsFromShieldAPI.Covers[0].SumInsured, "Sum Insured in Shield");
            Reporting.AreEqual(ShieldProductType.MGC, quoteDetailsFromShieldAPI.ProductType, "Product Type in Shield");
            Reporting.AreEqual(DateTime.Now.AddYears(1).ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), quoteDetailsFromShieldAPI.PolicyEndDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), "Policy End Date in Shield");
            Reporting.AreEqual(DateTime.Now.AddDays(29).ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), quoteDetailsFromShieldAPI.ProposalValidDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), "Proposal Valid Date in Shield");

            if (!quoteInputs.Drivers[0].Details.SkipDeclaringMembership)
                Reporting.AreEqual(APIDiscountCode[quoteInputs.Drivers[0].Details.MembershipTier], quoteDetailsFromShieldAPI.DiscountGroup, "Discount Group in Shield");
            else
                Reporting.AreEqual(APIDiscountCode[MembershipTier.None], quoteDetailsFromShieldAPI.DiscountGroup, "Discount Group in Shield");
            return quoteDetailsFromShieldAPI;
        }

        private static void VerifyQuoteMotorcycleAssetDetailsInShield(QuoteMotorcycle quoteInputs, GetQuotePolicy_Response quoteDetailsFromShieldAPI)
        {
            string security = null;

            Reporting.AreEqual(MotorcycleUsageMappings[quoteInputs.UsageType].TextShield, quoteDetailsFromShieldAPI.MotorcycleAsset.VehicleUsage.ToLower(), "Vehicle Usage in Shield");
            Reporting.AreEqual(quoteInputs.Drivers[0].LicenseTime, quoteDetailsFromShieldAPI.MotorcycleAsset.Drivers[0].DriverExperience, "Driver Experience in Shield");
            Reporting.AreEqual(quoteInputs.ParkingAddress.Suburb, quoteDetailsFromShieldAPI.MotorcycleAsset.Suburb, true, "Suburb in Shield");
            Reporting.AreEqual(quoteInputs.IsGaraged, quoteDetailsFromShieldAPI.MotorcycleAsset.IsGaraged, "IsGaraged in Shield");
            Reporting.AreEqual(quoteInputs.HasDashcam, quoteDetailsFromShieldAPI.MotorcycleAsset.IsDashcamUsed, "IsDashcamUsed in Shield");
            Reporting.AreEqual(quoteInputs.AnnualKm, quoteDetailsFromShieldAPI.MotorcycleAsset.AnnualKms, "AnnualKms in Shield");
            Reporting.AreEqual(quoteInputs.IsFinanced, quoteDetailsFromShieldAPI.MotorcycleAsset.IsFinanced, "IsFinanced in Shield");
            Reporting.AreEqual(quoteInputs.IsModified, quoteDetailsFromShieldAPI.MotorcycleAsset.IsModified(), "Motorcycle modified flag in Shield");

            if (quoteInputs.HasImmobiliser)
            {
                security = QUOTE_SECURITY_IMMOBILISER_IN_SHIELD;
            }

            Reporting.AreEqual(security, quoteDetailsFromShieldAPI.MotorcycleAsset.Security, "Security in Shield");

            if (quoteInputs.IsFinanced)
            {
                Reporting.AreEqual(quoteInputs.Financier, quoteDetailsFromShieldAPI.GetFinancierNameViaShieldAPI(), ignoreCase: true, $"Expected financier name against the actual name displayed");
            }
        }

        private static void VerifyVehicleDetailsInShield(QuoteMotorcycle quoteInputs, Rac.TestAutomation.Common.API.GetQuotePolicy_Response quoteDetailsFromShieldAPI)
        {
            var vehicleDetailsFromShieldAPI = DataHelper.GetVehicleDetails(quoteDetailsFromShieldAPI.MotorcycleAsset.VehicleId);
            Reporting.AreEqual(ShieldMainVehicleTypeDesc.MOTORCYCLES.GetDescription(), vehicleDetailsFromShieldAPI.Vehicles[0].MainVehicleTypeDescription, "Main Vehicle Type Description in Shield");
            Reporting.AreEqual(quoteInputs.Make, vehicleDetailsFromShieldAPI.Vehicles[0].MakeDescription, "Motorcycle Make in Shield");
            Reporting.AreEqual(quoteInputs.Model, vehicleDetailsFromShieldAPI.Vehicles[0].ModelDescription.Trim(), "Motorcycle Model in Shield");
            Reporting.AreEqual(quoteInputs.Year, vehicleDetailsFromShieldAPI.Vehicles[0].ModelYear, "Motorcycle Year in Shield");
        }
    }
}
