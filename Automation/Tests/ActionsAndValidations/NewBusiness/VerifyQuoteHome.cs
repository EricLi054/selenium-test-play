using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using System;
using System.Linq;
using UIDriver.Pages.B2C;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace Tests.ActionsAndValidations
{
    public static class VerifyQuoteHome
    {
        // OVS and OVU covers have a fixed excess of $200.
        private const string EXCESS_OUTSIDE_VALUABLES = "200";

        /// <summary>
        /// Verifies the retrieved quote values against the expected values.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testData"></param>
        /// <param name="expectedQuoteValues"></param>
        /// <exception cref="NotSupportedException">Thrown if its not a valid Home Occupancy.</exception>
        public static void VerifyQuoteAfterRetrieve(Browser browser, QuoteHome testData, QuoteData expectedQuoteValues)
        {
            using (var quotePage2 = new HomeQuote2Quote(browser))
            {
                Reporting.Log($"Quote {expectedQuoteValues.QuoteNumber} retrieved. Begin verifying values.", browser.Driver.TakeSnapshot());
                Reporting.AreEqual(expectedQuoteValues.QuoteNumber, quotePage2.QuoteReference, "Quote number");
                Reporting.AreEqual(expectedQuoteValues.AnnualPremium, quotePage2.QuotePriceAnnual, "Annual Premium");
                Reporting.AreEqual(expectedQuoteValues.MonthlyPremium, quotePage2.QuotePriceMonthly, "Monthly Premium");

                if (testData.BuildingValue.HasValue)
                {
                    Reporting.AreEqual(expectedQuoteValues.HomeInsuredValueAndExcess.ExcessBuilding,
                                       quotePage2.QuoteBuildingExcess, "Building Excess");
                    Reporting.AreEqual(expectedQuoteValues.HomeInsuredValueAndExcess.SumInsuredBuilding,
                                       quotePage2.QuoteBuildingSumInsured, "Building Sum Insured");
                }
                else
                {
                    Reporting.IsFalse(quotePage2.IsBuildingSumInsuredDisplayed, "Building sum insured should not be displayed as we didn't request this cover");
                    Reporting.IsFalse(quotePage2.IsBuildingExcessDisplayed, "Building excess should not be displayed as we didn't request this cover");
                }
                
                if (testData.ContentsValue.HasValue)
                {
                    Reporting.AreEqual(expectedQuoteValues.HomeInsuredValueAndExcess.ExcessContents,
                                       quotePage2.QuoteContentsExcess, "Contents Excess");
                    Reporting.AreEqual(expectedQuoteValues.HomeInsuredValueAndExcess.SumInsuredContents,
                                       quotePage2.QuoteContentsSumInsured, "Contents Sum Insured");

                    switch(testData.Occupancy)
                    {
                        case HomeOccupancy.OwnerOccupied:
                            VerifyQuoteHome.VerifySpecifiedContentsRetrievedHomeQuoteDetailsOnPage2(browser, testData);
                            Reporting.AreEqual(testData.UnspecifiedValuablesInsuredAmount, quotePage2.UnspecifiedValuablesCover, "Unspecified valuables covers is equal");
                            break;
                        case HomeOccupancy.HolidayHome:
                            VerifyQuoteHome.VerifySpecifiedContentsRetrievedHomeQuoteDetailsOnPage2(browser, testData);
                            Reporting.IsFalse(quotePage2.ArePersonalValuablesDisplayed, "whether personal valuable options are available - should not be when there is no contents cover");
                            break;
                        case HomeOccupancy.InvestmentProperty:
                            Reporting.IsFalse(quotePage2.ArePersonalValuablesDisplayed, "whether personal valuable options are available - should not be when there is no contents cover");
                            Reporting.IsFalse(quotePage2.AreSpecifiedContentsDisplayed, "whether specified contents options are available - should not be when there is no contents cover");
                            break;
                        case HomeOccupancy.Tenant:
                            Reporting.AreEqual(testData.UnspecifiedValuablesInsuredAmount, quotePage2.UnspecifiedValuablesCover, "Unspecified valuables covers is equal");
                            Reporting.IsFalse(quotePage2.AreSpecifiedContentsDisplayed, "whether specified contents options are available - should not be when there is no contents cover");
                            break;
                        default:
                            throw new NotSupportedException($"Does not support {testData.Occupancy.GetDescription()} type requested occupancy yet.");
                    }
                }
                else
                {
                    Reporting.IsFalse(quotePage2.IsContentsSumInsuredDisplayed, "Contents sum insured should not be displayed as we didn't request this cover");
                    Reporting.IsFalse(quotePage2.IsContentsExcessDisplayed, "Contents excess should not be displayed as we didn't request this cover");
                    Reporting.IsFalse(quotePage2.ArePersonalValuablesDisplayed, "whether personal valuable options are available - should not be when there is no contents cover");
                    Reporting.IsFalse(quotePage2.AreSpecifiedContentsDisplayed, "whether specified contents options are available - should not be when there is no contents cover");
                }
            }
        }

        public static void VerifyQuoteSummaryPage(Browser browser, QuoteHome quoteDetails)
        {
            using (var quotePage3  = new HomeQuote3Summary(browser))
            using (var paymentPage = new QuotePayments(browser))
            using (var spinner     = new RACSpinner(browser))
            {
                Reporting.Log("Begin verify policy home details");

                quotePage3.VerifyPolicyholderDetails(quoteDetails.PolicyHolders[0]);
                quotePage3.VerifySumInsuredAndExcessDetails(quoteDetails);
            }
        }

        public static string VerifyHomeConfirmationPage(Browser browser, QuoteHome quoteDetails, QuoteData expectedPremiumValues, out string receiptNumber)
        {
            var policyNumber = string.Empty;
            receiptNumber    = string.Empty;

            if (quoteDetails.Occupancy == HomeOccupancy.OwnerOccupied &&
                (quoteDetails.AlarmSystem == Alarm.NoAlarm ||
                 quoteDetails.AlarmSystem == Alarm.NonMonitoredAlarm))
            {
                using (var confirmation = new HomeQuoteConfirmation(browser))
                {
                    Reporting.Log("Verify RAC alarm offer pop up dialog.");

                    Reporting.Log($"Dialog found is: {confirmation.CheckForRACAlarmQuoteOfferAndDismiss()}");
                }
            }

            using (var confirmation = new HomeQuoteConfirmation(browser))
            {
                confirmation.WaitForPage();
                Reporting.Log("Verifying details on confirmation screen.");

                policyNumber = confirmation.PolicyNumber;
                Reporting.Log($"Policy number is: {policyNumber}", browser.Driver.TakeSnapshot());

                Reporting.AreEqual(quoteDetails.StartDate.ToString("d MMMM yyyy"), confirmation.PolicyStartDate, "Home policy start date");
                Reporting.AreEqual(quoteDetails.StartDate.AddYears(1).ToString("d MMMM yyyy"), confirmation.PolicyEndDate, "Home policy end date");

                var expectedCoverText = quoteDetails.GetRequestedCoverAsString();

                // On summary page, cover generally reflects what the user picked from the dropdown, with
                // the following exceptions:
                // - Renters policies where B2C overrides it with the term "Basic Contents"
                // - Landlord policies where B2C overrides it with the term "Landlord's"
                switch (quoteDetails.Occupancy)
                {
                    case HomeOccupancy.Tenant:
                        expectedCoverText = "Basic Contents";
                        break;
                    case HomeOccupancy.InvestmentProperty:
                        expectedCoverText = "Landlord's";
                        break;
                    case HomeOccupancy.OwnerOccupied:
                    case HomeOccupancy.HolidayHome:
                        if (expectedCoverText == HomeCover.BuildingOnly.GetDescription())
                        {
                            expectedCoverText = "Building";
                        }

                        if (expectedCoverText == HomeCover.ContentsOnly.GetDescription())
                        {
                            expectedCoverText = "Contents";
                        }
                        break;
                    default:
                        break;
                }

                Reporting.AreEqual(expectedCoverText, confirmation.Cover, true, "Home cover type");

                Reporting.IsTrue(quoteDetails.PropertyAddress.IsEqualToString(confirmation.PropertyAddress), $"risk address {confirmation.PropertyAddress} is as expected {quoteDetails.PropertyAddress.QASStreetAddress()}.");

                if (quoteDetails.BuildingValue.HasValue)
                {
                    Reporting.AreEqual(expectedPremiumValues.HomeInsuredValueAndExcess.SumInsuredBuilding, confirmation.SumInsuredBuilding, "building sum insured value");
                    Reporting.AreEqual(expectedPremiumValues.HomeInsuredValueAndExcess.ExcessBuilding, confirmation.ExcessBuilding, "building excess");
                }

                if (quoteDetails.ContentsValue.HasValue)
                {
                    Reporting.AreEqual(expectedPremiumValues.HomeInsuredValueAndExcess.SumInsuredContents, confirmation.SumInsuredContents, "contents sum insured value");
                    Reporting.AreEqual(expectedPremiumValues.HomeInsuredValueAndExcess.ExcessContents, confirmation.ExcessContents, "contents excess");
                }

                if (quoteDetails.IsEligibleForAccidentalDamage && quoteDetails.AddAccidentalDamage)
                {
                    Reporting.IsTrue(confirmation.IsAccidentalDamageLabelDisplayed, "Accidental damage label".IsDisplayed());
                    Reporting.AreEqual("Accidental Damage:", confirmation.AccidentalDamageLabelText, "Accidental Damage:".IsExpectedLabelText());
                    Reporting.IsTrue(confirmation.IsAccidentalDamageSumInsuredDisplayed, "Accidental damage sum insured".IsDisplayed());
                    Reporting.AreEqual(HOME_ACCIDENTAL_DAMAGE_SI_CONTENTS_COVERED, confirmation.SumInsuredAccidentalDamage, "accidental damage contents covered");

                    Reporting.IsTrue(confirmation.IsAccidentalDamageExcessLabelDisplayed, "Accidental damage excess label".IsDisplayed());
                    Reporting.AreEqual("Accidental Damage excess:", confirmation.AccidentalDamageExcessLabelText, "Accidental Damage excess:".IsExpectedLabelText());
                    Reporting.IsTrue(confirmation.IsAccidentalDamageExcessDisplayed, "Accidental damage excess".IsDisplayed());

                    Reporting.AreEqual(HOME_ACCIDENTAL_DAMAGE_EXCESS, confirmation.ExcessAccidentalDamage, "accidental damage excess");
                }
                else 
                {
                    Reporting.IsFalse(confirmation.IsAccidentalDamageLabelDisplayed, "Accidental damage label".IsNotDisplayed());
                    Reporting.IsFalse(confirmation.IsAccidentalDamageSumInsuredDisplayed, "Accidental damage sum insured".IsNotDisplayed());

                    Reporting.IsFalse(confirmation.IsAccidentalDamageExcessLabelDisplayed, "Accidental damage excess label".IsNotDisplayed());
                    Reporting.IsFalse(confirmation.IsAccidentalDamageExcessDisplayed, "Accidental damage excess".IsNotDisplayed());
                }

                Reporting.IsTrue(confirmation.IsPaymentFrequencyLabelDisplayed, "Payment frequency label".IsDisplayed());
                Reporting.AreEqual("Payment frequency:", confirmation.PaymentFrequencyLabelText, "Payment frequency:".IsExpectedLabelText());
                Reporting.AreEqual(quoteDetails.PayMethod.PaymentFrequency, confirmation.PaymentFrequency, "Payment Frequency on confirmation page");

                var expectedPrice = quoteDetails.PayMethod.IsAnnual ? expectedPremiumValues.AnnualPremium : expectedPremiumValues.MonthlyPremium;
                Reporting.IsTrue(confirmation.IsAmountLabelDisplayed, "Amount label".IsDisplayed());
                Reporting.AreEqual("Amount:", confirmation.AmountLabelText, "Amount:".IsExpectedLabelText());
                Reporting.AreEqual(expectedPrice, confirmation.AmountPaid, "recorded amount as expected");

                if (quoteDetails.PayMethod.Scenario == PaymentScenario.AnnualCash)
                    receiptNumber = confirmation.VerifyReceiptNumberIsDisplayedCorrectly();
                else
                {
                    Reporting.IsFalse(confirmation.IsReceiptNumberLabelDisplayed, "Receipt number label".IsNotDisplayed());
                    Reporting.IsFalse(confirmation.IsReceiptNumberDisplayed, "Receipt number".IsNotDisplayed());
                }
            }

            return policyNumber;
        }

        public static void VerifyHomePolicyInShield(QuoteHome quoteDetails, string policyNumber)
        {
            // Verify General Policy details via database query
            Reporting.LogTestShieldValidations("policy", policyNumber);
            var policyInfo = ShieldHomeDB.FetchHomePolicyDetails(policyNumber);

            Reporting.AreEqual(quoteDetails.StartDate.ToString("d MMMM yyyy"), policyInfo.PolicyStartDate.ToString("d MMMM yyyy"), "Policy start date");
            Reporting.IsTrue(quoteDetails.Occupancy == policyInfo.Occupancy, "Verifying occupancy status in database.");

            // Verify covers
            switch (quoteDetails.Occupancy)
            {
                case HomeOccupancy.OwnerOccupied:
                    if (quoteDetails.ContentsValue.HasValue)
                        PullCoverFromList(policyInfo, HomeCoverCodes.HCN, quoteDetails.ExcessContents, quoteDetails.ContentsValue.Value);

                    if (quoteDetails.BuildingValue.HasValue)
                        PullCoverFromList(policyInfo, HomeCoverCodes.HB, quoteDetails.ExcessBuilding, quoteDetails.BuildingValue.Value);

                    if (quoteDetails.IsEligibleForAccidentalDamage && quoteDetails.AddAccidentalDamage)
                    {
                        PullCoverFromList(policyInfo, HomeCoverCodes.AD, HOME_ACCIDENTAL_DAMAGE_EXCESS, HOME_ACCIDENTAL_DAMAGE_SI_AMOUNT);
                    }
                    break;
                case HomeOccupancy.HolidayHome:
                    if (quoteDetails.ContentsValue.HasValue)
                        PullCoverFromList(policyInfo, HomeCoverCodes.HCN, quoteDetails.ExcessContents, quoteDetails.ContentsValue.Value);

                    if (quoteDetails.BuildingValue.HasValue)
                        PullCoverFromList(policyInfo, HomeCoverCodes.HB, quoteDetails.ExcessBuilding, quoteDetails.BuildingValue.Value);
                    break;
                case HomeOccupancy.Tenant:
                    PullCoverFromList(policyInfo, HomeCoverCodes.RCN, quoteDetails.ExcessContents, quoteDetails.ContentsValue.Value);
                    break;
                case HomeOccupancy.InvestmentProperty:
                    if (quoteDetails.ContentsValue.HasValue)
                        PullCoverFromList(policyInfo, HomeCoverCodes.LCN, quoteDetails.ExcessContents, quoteDetails.ContentsValue.Value);

                    if (quoteDetails.BuildingValue.HasValue)
                        PullCoverFromList(policyInfo, HomeCoverCodes.LB, quoteDetails.ExcessBuilding, quoteDetails.BuildingValue.Value);
                    break;
                default:
                    throw new NotImplementedException("Test Framework does not support requested occupancy yet.");

            }

            if (quoteDetails.UnspecifiedValuablesInsuredAmount != UnspecifiedPersonalValuables.None)
            {
                PullCoverFromList(policyInfo, HomeCoverCodes.OVU, EXCESS_OUTSIDE_VALUABLES, (int)quoteDetails.UnspecifiedValuablesInsuredAmount);
            }
            else
            {
                Reporting.IsFalse(policyInfo.Covers.Any(x => x.CoverCode == HomeCoverCodes.OVU), "There should be no outside specified valuables");
            }

            if (quoteDetails.SpecifiedValuablesOutside?.Any() == true)
            {
                PullCoverFromList(policyInfo, HomeCoverCodes.OVS, EXCESS_OUTSIDE_VALUABLES, quoteDetails.SpecifiedValuablesOutside.Sum(x => x.Value));
                // Verify personal valuables details
                foreach (var expectedItem in quoteDetails.SpecifiedValuablesOutside)
                {
                    var found = policyInfo.SpecifiedValuablesAndContents.FirstOrDefault(x => x.Category == expectedItem.Category &&
                                                                                             x.Description == expectedItem.Description &&
                                                                                             x.Value == expectedItem.Value);
                    Reporting.IsNotNull(found, $"Personal Valuable {expectedItem.Description} in Shield record of policy.");
                }
            }

            if (quoteDetails.SpecifiedValuablesInside?.Any() == true)
            {
                // Verify specified contents
                foreach (var expectedItem in quoteDetails.SpecifiedValuablesInside)
                {
                    var expectedContentsText = $"{expectedItem.CategoryB2CStringForSpecifiedContents} | {expectedItem.Description}";
                    var found = policyInfo.SpecifiedValuablesAndContents.FirstOrDefault(x => x.Category == (int)SpecifiedValuables.Contents &&
                                                                                             x.Description == expectedContentsText &&
                                                                                             x.Value == expectedItem.Value);
                    Reporting.IsNotNull(found, $"Specified Contents item {expectedContentsText} in Shield record of policy.");
                }
            }

            if (policyInfo.Covers.Any() == true)
            {
                string unexpectedCovers = "";
                foreach (var cover in policyInfo.Covers)
                    unexpectedCovers += $"{cover.CoverDescription},";
                Reporting.Error($"The following additional covers were found on the policy: {unexpectedCovers}");
            }

            // Verify Landlord specific elements
            if (quoteDetails.Occupancy == HomeOccupancy.InvestmentProperty)
            {
                Reporting.AreEqual(quoteDetails.WeeklyRental, policyInfo.WeeklyRental, "weekly rental is set correctly");
                Reporting.AreEqual(quoteDetails.PropertyManager, policyInfo.PropertyManager.Value, "Property manager is set correctly");
            }

            // Verify contacts
            for (int i = 0; i < quoteDetails.PolicyHolders.Count; i++)
            {
                var expectedContact = quoteDetails.PolicyHolders[i];
                // First Contact will always be the PH.
                var expectedContactRole = i == 0 ?
                            ContactRole.PolicyHolder :
                            ContactRole.CoPolicyHolder;

                var actualContact = policyInfo.PolicyHolders.FirstOrDefault(x => x.FirstName.Equals(expectedContact.FirstName, StringComparison.InvariantCultureIgnoreCase));

                //TODO: Introduce conditional validation based on .WithMultiMatchContact flag in Test Data to assert & verify whether these should match.
                Reporting.Log($"Original ContactId identified (if applicable) = {expectedContact.Id}, Actual ContactId used = {actualContact.Id}. When both values exist they should ONLY differ for Multi-match tests.");

                Reporting.IsNotNull(actualContact, $"{expectedContact.FirstName} {expectedContact.Surname} as policy holder on policy");

                Reporting.AreEqual(expectedContact.Surname, actualContact.Surname, true, "surname matches expected value");
                Reporting.AreEqual(expectedContact.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
                                   actualContact.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), "date of birth matches expected value");
                Reporting.AreEqual(expectedContact.MailingAddress.QASStreetAddress(), actualContact.MailingAddress.QASStreetAddress(), ignoreCase: true, "mailing address is as expected");
                VerifyContactTelephoneNumber(expectedContact, actualContact);
                Reporting.IsTrue(actualContact.ContactRoles.Contains(expectedContactRole), $"expected Contact role ({expectedContactRole}) exists for contact number {i + 1}");

                // Verify Preferred Delivery Method for Main PolicyHolder
                var actualDeliveryMethod = DataHelper.GetPreferredDeliveryMethodForMainPolicyholder(policyNumber);
                var expectedDeliveryMethod = quoteDetails.PolicyHolders[0].IsEmailPreferredDeliveryMethod() ? PreferredDeliveryMethod.Email : PreferredDeliveryMethod.Mail;
                Reporting.AreEqual(expectedDeliveryMethod, actualDeliveryMethod, "the actual preferred delivery method");
            }

            // Verify payment details
            VerifyHomePolicyPaymentDetailsInShield(quoteDetails: quoteDetails, policyNumber: policyNumber);
            // Verify contact multi match details
            VerifyPolicy.VerifyPolicyMultiMatchDetailsInShield(quoteDetails.PolicyHolders, policyNumber);

            // Verify policy details via API:
            if (Config.Get().IsCycloneEnabled())
            {
                var policyInfoByApiCall = DataHelper.GetPolicyDetails(policyNumber);
                Reporting.AreEqual(quoteDetails.IsACycloneAddress,  policyInfoByApiCall.HomeAsset.IsCycloneProneArea, "that property is correctly flagged for cyclone risk");

                if (quoteDetails.IsACycloneAddress)
                {
                    Reporting.AreEqual(quoteDetails.IsPropertyElevated, policyInfoByApiCall.HomeAsset.GetIsPropertyElevated, "that property elevation status is correctly set");
                    Reporting.AreEqual(quoteDetails.IsCycloneShuttersFitted, policyInfoByApiCall.HomeAsset.GetHasCycloneShutters, "that property cyclone shutter status is correctly reported");
                    var expectedGarageDoorStatus = quoteDetails.YearBuilt < 2012 ?
                                                   quoteDetails.GarageDoorsCycloneStatus :
                                                   GarageDoorsUpgradeStatus.ReplacedToCyclone;

                    var expectedRoofImprovementStatus = quoteDetails.YearBuilt < 1982 ?
                                                        quoteDetails.RoofImprovementCycloneStatus.ToString() :
                                                        RoofImprovementStatus.CompleteRoofReplacement.ToString();
                    Reporting.AreEqual(expectedGarageDoorStatus.ToString(), policyInfoByApiCall.HomeAsset.GetGarageDoorUpgradeStatus.ToString(), "the garage door upgrade status is as expected");
                    Reporting.AreEqual(expectedRoofImprovementStatus.ToString(), policyInfoByApiCall.HomeAsset.GetRoofImprovementStatus.ToString(), "the roof improvement status is as expected");
                }
                else
                {
                    Reporting.IsNull(policyInfoByApiCall.HomeAsset.IsPropertyElevated, "property elevated status is null as not a cyclone address");
                    Reporting.IsNull(policyInfoByApiCall.HomeAsset.HasCycloneShutters, "property cyclone shutters value is null as not a cyclone address");
                    Reporting.IsNull(policyInfoByApiCall.HomeAsset.GarageDoorUpgraded, "garage door status is null as not a cyclone address");
                    Reporting.IsNull(policyInfoByApiCall.HomeAsset.RoofImprovement, "roof improvement status is null as not a cyclone address");
                }
            }

            var policyDetails = DataHelper.GetPolicyDetails(policyNumber);
            Reporting.AreEqual(quoteDetails.IsFinanced, policyDetails.HomeAsset.IsFinanced, "IsFinanced flag correctly set in Shield");

            if (quoteDetails.IsFinanced)
            {
                Reporting.AreEqual(quoteDetails.Financier, policyDetails.GetFinancierNameViaShieldAPI(), ignoreCase: true, $"Expected financier name against the actual name displayed");
            }
        }

        /// <summary>
        /// If a Mobile telephone number was generated for this contact, then the actual contact should have that Mobile
        /// telephone number.
        /// If no Mobile telephone number was generated for this contact, then the actual Mobile telephone number value 
        /// returned from Shield should be null, as we would have input a Home telephone number instead. So we check for 
        /// IsNullOrEmpty regarding mobile, then confirm that the Home telephone number (AKA: Private telephone number) 
        /// matches the value generated.
        /// </summary>
        /// <param name="expectedContact">The test data generated for this contact in this test.</param>
        /// <param name="actualContact">The actual contact data returned from Shield for the contact in this test</param>
        public static void VerifyContactTelephoneNumber(Contact expectedContact, Contact actualContact)
        {
            if(!string.IsNullOrEmpty(expectedContact.MobilePhoneNumber))
            {
                Reporting.AreEqual(expectedContact.MobilePhoneNumber, actualContact.MobilePhoneNumber, "expected mobile phone number matches actual value");
            }
            else
            {
                Reporting.IsTrue(string.IsNullOrEmpty(actualContact.MobilePhoneNumber), "Mobile Telephone number is null or empty to match Expected Mobile Telephone number");

                if (!string.IsNullOrEmpty(expectedContact.HomePhoneNumber))
                {
                    Reporting.AreEqual(expectedContact.HomePhoneNumber, actualContact.HomePhoneNumber, "expected home phone number matches actual value");
                }
                else
                {
                    Reporting.IsTrue(string.IsNullOrEmpty(expectedContact.HomePhoneNumber), "Home Telephone number is null or empty to match Expected Home Telephone number");
                }
            }
        }

        public static void VerifySpecifiedContentsRetrievedHomeQuoteDetailsOnPage2(Browser browser, QuoteHome quoteDetails)
        {

            using (var quotePage2 = new HomeQuote2Quote(browser))
            {
                var displayedSpecifiedContents = quotePage2.GetSpecifiedContentItems();
                var expectedCountSpecifiedContents = quoteDetails.SpecifiedValuablesInside != null ?
                                                     quoteDetails.SpecifiedValuablesInside.Count :
                                                     0;

                var expectedSpecifiedContentsTotalValue = 0;

                Reporting.AreEqual(expectedCountSpecifiedContents, displayedSpecifiedContents.Count, "Number of specified contents items displayed as expected.");

                for (int index = 0; index < expectedCountSpecifiedContents; index++)
                {
                    var expectedItem = quoteDetails.SpecifiedValuablesInside[index];
                    var displayedItem = displayedSpecifiedContents[index];

                    Reporting.AreEqual(expectedItem.Category, displayedItem.Category, $"Specified Content #{index}; Category enum code");
                    Reporting.AreEqual(expectedItem.Description, displayedItem.Description, $"Specified Content #{index}; Description");
                    Reporting.AreEqual(expectedItem.Value, displayedItem.Value, $"Specified Content #{index}; item value");

                    expectedSpecifiedContentsTotalValue += expectedItem.Value;
                }

                Reporting.AreEqual(expectedSpecifiedContentsTotalValue, quotePage2.GetSpecifiedContentsTotalValue(), "Total value displayed for specified contents");
            }

        }

        private static void PullCoverFromList(HomePolicy shieldDBValues, HomeCoverCodes expectedCover, string expectedExcess, int expectedSumInsured)
        {
            var foundCover = shieldDBValues.Covers.FirstOrDefault(x => x.CoverCode == expectedCover);
            Reporting.IsNotNull(foundCover, $"{expectedCover.GetDescription()} cover was found");
            Reporting.AreEqual(expectedExcess, foundCover.Excess, $"{expectedCover.GetDescription()} Excess");
            Reporting.AreEqual(expectedSumInsured, foundCover.SumInsured, $"{expectedCover.GetDescription()} Sum Insured");
            shieldDBValues.Covers.Remove(foundCover);
        }

        /// <summary>
        /// Verifies new business home policy's payment details against Shield DB
        /// </summary>
        /// <param name="testConfig"></param>
        /// <param name="quoteDetails"></param>
        /// <param name="policyNumber"></param>
        public static void VerifyHomePolicyPaymentDetailsInShield(QuoteHome quoteDetails, string policyNumber)
        {
            Reporting.Log($"Begin verify payment details from Shield DB. Param/s = {policyNumber}");
            var dbPaymentDetails      = ShieldPolicyDB.FetchPaymentDetailsForPolicy(policyNumber);
            var recentEvents          = ShieldPolicyDB.FetchRecentEventsOnPolicy(policyNumber);

            Reporting.IsTrue(quoteDetails.PayMethod.PaymentFrequency == dbPaymentDetails.PaymentFrequency, "payment frequency is expected value in DB");

            if (quoteDetails.PayMethod.IsPaymentByBankAccount)
                Reporting.AreEqual(DIRECT_DEBIT, dbPaymentDetails.PaymentMethod);
            else
            {
                if (quoteDetails.PayMethod.IsAnnual)
                {
                    Reporting.AreEqual(CASH, dbPaymentDetails.PaymentMethod);
                    VerifyPolicy.VerifyNewBusinessEventForAnnualCashPayment(recentEvents);
                }
                else
                    Reporting.AreEqual(CREDIT_CARD, dbPaymentDetails.PaymentMethod);
            }

            if (quoteDetails.PayMethod.Scenario != PaymentScenario.AnnualCash)
                VerifyPolicy.VerifyNewBusinessEventForDirectDebitInstalmentPayment(recentEvents);

            var expectedPaymentFrequency = quoteDetails.PayMethod.NumberOfPayments;
            Reporting.AreEqual(expectedPaymentFrequency, dbPaymentDetails.PaymentCount, "count of payments in DB is expected value");

            VerifyPolicy.VerifyNewPolicyInstalments(policyNumber);
        }
    }
}