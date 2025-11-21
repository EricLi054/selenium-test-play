using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using System;
using System.Linq;
using System.Threading;
using UIDriver.Pages.PCM;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace Tests.ActionsAndValidations
{
    public static class VerifyEndorseHome
    {
        /// <summary>
        /// Verify the policy information in the PCM Portfolio Summary after policy renewal.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testData"></param>
        public static void VerifyPolicyStateAfterRenewal(Browser browser, EndorseHome testData)
        {
            // Static sleep, as some payment processing events, particularly annual credit card
            // can take time to appear in Shield.
            Thread.Sleep(SleepTimes.T30SEC);

            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                pcmHomePage.WaitForPage();
                Reporting.Log("Verify policy values in portfolio summary after renewal.", browser.Driver.TakeSnapshot());
                Reporting.AreEqual(pcmHomePage.CurrentPolicyPolicyNumber, testData.PolicyNumber, "Policy Number in Portfolio Summary page matched the original policy number");

                // We had been forced to provide answers to mandatory cyclone questions.
                // So the premium will not match what Shield initially provided.
                if (!testData.AreWeChangingCycloneAnswers())
                {
                    Reporting.AreEqual(pcmHomePage.PolicyAnnualPremium, testData.OriginalPolicyData.AnnualPremium.Total, $"Annual Premium on the 'PortfolioSummary: Payment details' section matched the current Premium: {testData.OriginalPolicyData.AnnualPremium.Total.ToString("0.00")}");
                }
                else
                {
                    Reporting.Log("NOTE: Not all cyclone questions were pre-filled, so premium has changed with given answers.");
                }

                if (testData.OriginalPolicyData.StatusRenewalDate.Date == DateTime.Now.Date)
                {
                    Reporting.IsFalse(pcmHomePage.IsMakeAPaymentButtonShown, "'Make a Payment' button should no longer be visible.");
                }
                else
                {
                    Reporting.IsFalse(pcmHomePage.IsRenewalNoticeShown, "Renewal notice should no longer be visible.");
                    Reporting.IsFalse(pcmHomePage.IsRenewalButtonShown, "'Renew My Policy' button should no longer be visible.");
                    Reporting.IsTrue(pcmHomePage.IsRenewalAccepted(), "'Your policy is currently in renewal' message got displayed");
                }
            }
        }

        /// <summary>
        /// Validate the policy information in the PCM Portfolio Summary against the Shield Database after the Policy renewal.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testData"></param>
        /// <param name="annualPremium"></param>
        public static void VerifyPolicyDetailsInShieldAfterRenewal(Browser browser, EndorseHome testData, decimal annualPremium)
        {
            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                string policyNumber = testData.PolicyNumber;

                //Verify renewed policy data against the Shield DB
                Reporting.Log($"Querying Shield DB to verify details. Policy Number: {policyNumber}");

                var isRenewingToday = testData.RenewalDate.Date == DateTime.Now.Date;
                //Check if this is a PayNow Renewal Sceanrio
                HomePolicy policyInfoFromDB = isRenewingToday ? ShieldHomeDB.FetchHomePolicyDetailsAfterPayNowRenewal(policyNumber) : ShieldHomeDB.FetchHomePolicyDetailsAfterRenewal(testData);

                if (isRenewingToday || (testData.PayMethod.IsAnnual))
                {
                    Reporting.AreEqual(DateTime.Now.Date, policyInfoFromDB.PaymentDetails.PaymentDate, "Verifying PaymentDate in database.");
                    Reporting.AreEqual("Paid", policyInfoFromDB.PaymentDetails.PaymentStatus, "Verifying PaymentStatus in database.");
                    Reporting.AreEqual(annualPremium, policyInfoFromDB.PaymentDetails.PaymentTotal, "Verifying PaymentTotal in database.");
                    Reporting.AreEqual("Receipt", policyInfoFromDB.TransactionType, "Verifying TransactionType in database.");
                }
                if (!isRenewingToday) //If it is not a PayNow situation
                {
                    Reporting.AreEqual(1, policyInfoFromDB.EndorsementID, "Verifying EndorsementID in database.");
                    Reporting.AreEqual(DateTime.Now.Date, policyInfoFromDB.EventDetails.EventDate, "Verifying EventDate in database.");
                    Reporting.AreEqual("Policy Endorsement Certificate Print", policyInfoFromDB.EventDetails.EventType, "Verifying EventType in database.");

                    if (testData.PayMethod.IsAnnual)
                    {
                        decimal yearlyPremium = policyInfoFromDB.YearlyPremium;
                        Reporting.AreEqual(annualPremium, policyInfoFromDB.PaymentDetails.PaymentTotal, $"Verifying Current Premium: {annualPremium} value against the Yearly Premium in database: {yearlyPremium}");
                        Reporting.AreEqual(pcmHomePage.PolicyRenewalDate, policyInfoFromDB.MainRenewalDate.Date, $"Verifying Renewal Date in pcmHome page: {pcmHomePage.PolicyRenewalDate} against the database: {policyInfoFromDB.MainRenewalDate.Date}");
                        Reporting.AreEqual("Renewal - Cash", policyInfoFromDB.EventDetails.EventDocType, "Verifying EventDocType in database.");
                        Reporting.AreEqual(DateTime.Now.Date, policyInfoFromDB.PaymentDetails.PaymentDate, "Verifying PaymentDate in database.");
                    }
                    else if (testData.PayMethod.IsMonthly)
                    {
                        Reporting.AreEqual("Renewal - Direct Debit", policyInfoFromDB.EventDetails.EventDocType, "Verifying EventDocType in database.");
                    }
                }

                //Querying Shield DB to get Expected Premium for Policy Number
                if ((testData.PayMethod.IsAnnual) || (testData.RenewalDate.Date == DateTime.Now.Date))
                {
                    Reporting.Log($"Querying Shield DB to get Expected Premium for Policy Number: {policyNumber}");
                    decimal expectedPremium = ShieldHomeDB.FetchExpectedPremiumAfterRenewal(policyNumber);
                    Reporting.AreEqual(annualPremium, expectedPremium, "Verifying Current Premium value against the Expected Premium in database.");
                }

                // Verify endorsement details of policy from Shield API
                ValidateHomePolicyValuesAfterEndorsmentViaAPI(testData);
            }
        }

        /// <summary>
        /// Verify the policy premium and the updated home details in the PCM Portfolio
        /// Summary after a "Change my home details" endorsement.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testData"></param>
        /// <param name="origPolicyValues">Original values of the policy before endorsement</param>
        public static void VerifyPolicyPremiumAndHomeDetailsAfterEndorsement(Browser browser, EndorseHome testData, PolicyData origPolicyValues)
        {
            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                pcmHomePage.WaitForPage();
                pcmHomePage.ViewSpecificPolicy(testData.PolicyNumber);

                switch (testData.ExpectedImpactOnPremium)
                {
                    case PremiumChange.PremiumIncrease:
                        Reporting.IsTrue(origPolicyValues.AnnualPremium < pcmHomePage.PolicyAnnualPremium, $"New premium ({pcmHomePage.PolicyAnnualPremium}) is greater than original premium ({origPolicyValues.AnnualPremium})");
                        break;
                    case PremiumChange.PremiumDecrease:
                        Reporting.IsTrue(origPolicyValues.AnnualPremium > pcmHomePage.PolicyAnnualPremium, $"New premium ({pcmHomePage.PolicyAnnualPremium}) is lower than original premium ({origPolicyValues.AnnualPremium})");
                        break;
                    case PremiumChange.NoChange:
                        Reporting.IsTrue(origPolicyValues.AnnualPremium == pcmHomePage.PolicyAnnualPremium, $"New premium ({pcmHomePage.PolicyAnnualPremium}) is the same as original premium ({origPolicyValues.AnnualPremium})");
                        break;
                    default:
                        // nothing to do in regards to premium.
                        break;
                }

                // If non-null, we expect building cover
                if (testData.NewAssetValues.BuildingValue.HasValue)
                {
                    Reporting.AreEqual(testData.NewAssetValues.BuildingValue.Value, pcmHomePage.BuildingSumInsured, "Building sum insured value");

                    // If non-null, we would have changed the Building excess
                    if (testData.ExcessBuilding != null)
                        Reporting.AreEqual(testData.ExcessBuilding, pcmHomePage.BuildingExcess, "Building excess value");
                    else
                        Reporting.AreEqual(origPolicyValues.HomeCovers.BuildingExcess, pcmHomePage.BuildingExcess, "Building excess value");
                }

                // If non-null, we expect contents cover
                if (testData.NewAssetValues.ContentsValue.HasValue)
                {
                    Reporting.AreEqual(testData.NewAssetValues.ContentsValue.Value, pcmHomePage.ContentsSumInsured, "Contents sum insured value");

                    // If non-null, we would have changed the Contents excess
                    if (testData.ExcessContents != null)
                        Reporting.AreEqual(testData.ExcessContents, pcmHomePage.ContentsExcess, "Contents excess value");
                    else
                        Reporting.AreEqual(origPolicyValues.HomeCovers.ContentsExcess, pcmHomePage.ContentsExcess, "Contents excess value");
                }

                // If non-null, we would have changed the Address
                if (testData.NewAssetValues.PropertyAddress != null)
                { Reporting.AreEqual(testData.NewAssetValues.PropertyAddress.QASStreetAddress(), pcmHomePage.HomeAddress.StripAddressDelimiters(), ignoreCase: true, "Home Address"); }
                else
                { Reporting.AreEqual(origPolicyValues.HomeAddress, pcmHomePage.HomeAddress, ignoreCase: true, "Home Address"); }

                // If defined, we would have changed the Building type
                if (testData.NewAssetValues.TypeOfBuilding != HomeType.Undefined)
                { Reporting.AreEqual(testData.NewAssetValues.TypeOfBuilding, pcmHomePage.BuildingType, "Building type"); }

                // If defined, we would have changed the Alarm system
                if (testData.NewAssetValues.AlarmSystem != Alarm.Undefined)
                { Reporting.AreEqual(testData.NewAssetValues.AlarmSystem, pcmHomePage.AlarmSystem, "Alarm system"); }

                // If non-null, we would have changed the Financier
                if (testData.Financier != null)
                { Reporting.AreEqual(testData.Financier, pcmHomePage.Financier, "Financier"); }
            }
        }

        /// <summary>
        /// Verify the policy and home details against SHIELD DB
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="origPolicyValues">Original values of the policy before endorsement</param>
        public static void VerifyPolicyAndHomeDetailsInShield(EndorseHome testData, PolicyData origPolicyValues)
        {
            // Verify endorsement type and reason against SHIELD DB
            Reporting.Log($"Begin verify endorsement type and reason from Shield DB. Param/s = {testData.PolicyNumber}");

            var shieldEndorsementEvents = ShieldPolicyDB.FetchEndorsementReasonDetailsOnPolicy(testData.PolicyNumber);
            // We only care about the most recent event.
            var recordedEndorsementEvent = shieldEndorsementEvents.FirstOrDefault();
            Reporting.IsTrue(recordedEndorsementEvent != null, "We expected an endorsement event, but did not get any.");

            Reporting.AreEqual("General Changes", recordedEndorsementEvent.EndorsementType, ignoreCase: true);
            Reporting.AreEqual("Change home details", recordedEndorsementEvent.Description, ignoreCase: true);

            // Verify endorsement home building details against SHIELD DB
            Reporting.Log($"Begin verifying home endorsement details from Shield DB. Param/s = {testData.PolicyNumber}");
            var policyDataFromDB = ShieldHomeDB.FetchHomePolicyDetails(testData.PolicyNumber);

            ValidateBuildingSumInsuredAndExcess(policyDataFromDB, testData, origPolicyValues);
            ValidateContentsSumInsuredAndExcess(policyDataFromDB, testData, origPolicyValues);
            ValidatePropertyAddress(policyDataFromDB, testData, origPolicyValues);

            Reporting.AreEqual(testData.NewAssetValues.TypeOfBuilding, policyDataFromDB.PropertyDetails.TypeOfBuilding, "Verify Building type in DB");
            Reporting.AreEqual(testData.NewAssetValues.AlarmSystem, policyDataFromDB.PropertyDetails.AlarmSystem, "Verify Alarm system in DB");
            Reporting.AreEqual(testData.NewAssetValues.SecurityWindowsSecured.Equals(true), policyDataFromDB.PropertyDetails.SecurityWindowsSecured, "Verify Window security in DB");
            Reporting.AreEqual(testData.NewAssetValues.SecurityDoorsSecured.Equals(true), policyDataFromDB.PropertyDetails.SecurityDoorsSecured, "Verify Door security in DB.");
            Reporting.AreEqual(testData.NewAssetValues.WallMaterial, policyDataFromDB.PropertyDetails.WallMaterial, "Verify Home material in DB");
            Reporting.AreEqual(testData.NewAssetValues.YearBuilt, policyDataFromDB.PropertyDetails.YearBuilt, "Verify Year built in DB");

            // Roof material question is only applicable for Home Policies which
            // are pre-Cyclone (less than version ID 68000008), or Cyclone is disabled.
            // NOTE: Roof material is not asked, even if property is not at cyclone risk.
            if (!Config.Get().IsCycloneEnabled() || testData.OriginalPolicyData.ProductVersionAsInteger < HomeProductVersionCycloneReinsurance)
            {
                Reporting.AreEqual(testData.NewAssetValues.RoofMaterial, policyDataFromDB.PropertyDetails.RoofMaterial, "Verify Roof material in DB");
            }

            // Verify endorsement details of policy from Shield API
            ValidateHomePolicyValuesAfterEndorsmentViaAPI(testData);
        }

        public static void VerifyPolicyPaymentDetailsInShield(EndorseHome testData, PolicyData endorsementData)
        {
            Thread.Sleep(3000); // Allow some time before fetching updated values from Shield DB

            // Verify Payment details
            Reporting.Log($"Begin verify payment details from Shield DB. Param/s = {testData.PolicyNumber}");

            var dbPaymentDetails = ShieldPolicyDB.FetchPaymentDetailsForPolicy(testData.PolicyNumber);
            var recentEvents     = ShieldPolicyDB.FetchRecentEventsOnPolicy(testData.PolicyNumber);

            Reporting.IsTrue(testData.PayMethod.PaymentFrequency == dbPaymentDetails.PaymentFrequency, "payment frequency is expected value in DB");

            if (testData.PayMethod.IsPaymentByBankAccount)
                Reporting.AreEqual(DIRECT_DEBIT, dbPaymentDetails.PaymentMethod);
            else
            {
                if (testData.PayMethod.IsAnnual)
                {
                    Reporting.AreEqual(CASH, dbPaymentDetails.PaymentMethod);
                    VerifyEndorsePolicy.VerifyEndorsementEventForAnnualCashPayment(recentEvents, testData.ExpectedImpactOnPremium, endorsementData.RenewalDate);
                }
                else
                    Reporting.AreEqual(CREDIT_CARD, dbPaymentDetails.PaymentMethod);
            }

            if (testData.PayMethod.Scenario != PaymentScenario.AnnualCash)
                VerifyEndorsePolicy.VerifyEndorsementEventForNonAnnualCashPayment(recentEvents);

            var expectedPaymentFrequency = testData.PayMethod.NumberOfPayments;
            Reporting.AreEqual(expectedPaymentFrequency, dbPaymentDetails.PaymentCount, "count of payments in DB is expected value");

            VerifyPolicy.VerifyPolicyEndorsementInstalments(testData, renewalDate: endorsementData.RenewalDate);
        }

        private static void ValidateBuildingSumInsuredAndExcess(HomePolicy policyDataFromDB,
                                                                EndorseHome testData, 
                                                                PolicyData origPolicyValues)
        {
            // Verify the Building Sum Insured and Excess values.
            // If test data is not null, we expect Building Cover to be present and of an expected value
            if (testData.NewAssetValues.BuildingValue.HasValue)
            {
                var actualBuildingSI = policyDataFromDB.GetBuildingCover()?.SumInsured;
                Reporting.AreEqual(testData.NewAssetValues.BuildingValue.Value, actualBuildingSI, "Verify Building sum insured in DB.");

                var expectedBuildingExcess = string.IsNullOrEmpty(testData.ExcessBuilding) ?
                                             origPolicyValues.HomeCovers.BuildingExcess :
                                             testData.ExcessBuilding;
                var actualBuildingExcess = policyDataFromDB.GetBuildingCover()?.Excess;
                Reporting.AreEqual(expectedBuildingExcess, actualBuildingExcess, "Verify Building excess insured in DB.");
            }
            else
            {
                Reporting.IsNull(policyDataFromDB.GetBuildingCover(), "We expect no building cover to be on this policy.");
            }
        }

        private static void ValidateContentsSumInsuredAndExcess(HomePolicy policyDataFromDB,
                                                                EndorseHome testData,
                                                                PolicyData origPolicyValues)
        {
            // Verify the Contents Sum Insured and Excess values.
            // If non-null, we expect Contents Cover to be present and of an expected value
            if (testData.NewAssetValues.ContentsValue.HasValue)
            {
                var actualContentsSI = policyDataFromDB.GetContentsCover()?.SumInsured;
                Reporting.AreEqual(testData.NewAssetValues.ContentsValue.Value, actualContentsSI, "Verify Contents sum insured in DB.");

                var expectedContentsExcess = string.IsNullOrEmpty(testData.ExcessContents) ?
                                             origPolicyValues.HomeCovers.ContentsExcess :
                                             testData.ExcessContents;
                var actualContentsExcess = policyDataFromDB.GetContentsCover()?.Excess;
                Reporting.AreEqual(expectedContentsExcess, actualContentsExcess, "Verify Contents excess insured in DB.");
            }
            else
            {
                Reporting.IsNull(policyDataFromDB.GetContentsCover(), "We expect no contents cover to be on this policy.");
            }

        }

        private static void ValidatePropertyAddress(HomePolicy policyDataFromDB,
                                                    EndorseHome testData,
                                                    PolicyData origPolicyValues)
        {
            var actualHomeAddress = policyDataFromDB.PropertyDetails.PropertyAddress.PCMFormattedAddressString();

            // If non-null, we would have changed the Address
            if (testData.NewAssetValues.PropertyAddress != null)
            {
                var expectedHomeAddress = testData.NewAssetValues.PropertyAddress.PCMFormattedAddressString();

                Reporting.AreEqual(expectedHomeAddress, actualHomeAddress, ignoreCase: true);
            }
            else
            {
                Reporting.IsTrue(origPolicyValues.HomeAddress.StartsWith(actualHomeAddress, StringComparison.InvariantCultureIgnoreCase), "Verify Home address in DB");
            }
        }

        /// <summary>
        /// Verify home policy values via API, after endorsement (mid-term or renewal)
        /// has completed.
        /// </summary>
        private static void ValidateHomePolicyValuesAfterEndorsmentViaAPI(EndorseHome testData)
        {
            // Verify endorsement details of policy from Shield API
            if (Config.Get().IsCycloneEnabled() &&
                testData.OriginalPolicyData.ProductVersionAsInteger >= HomeProductVersionCycloneReinsurance)
            {
                var latestPolicyStateFromApi = DataHelper.GetPolicyDetails(testData.PolicyNumber);
                Reporting.AreEqual(testData.NewAssetValues.IsACycloneAddress, latestPolicyStateFromApi.HomeAsset.IsCycloneProneArea, "that property is correctly flagged for cyclone risk");

                if (testData.NewAssetValues.IsACycloneAddress)
                {
                    Reporting.AreEqual(testData.NewAssetValues.IsPropertyElevated, latestPolicyStateFromApi.HomeAsset.GetIsPropertyElevated, "that property elevation status is correctly set");
                    Reporting.AreEqual(testData.NewAssetValues.IsCycloneShuttersFitted, latestPolicyStateFromApi.HomeAsset.GetHasCycloneShutters, "that property cyclone shutter status is correctly reported");

                    var expectedGarageDoorUpgradeStatus = testData.NewAssetValues.YearBuilt > 2012 ?
                                                          GarageDoorsUpgradeStatus.NotSure :
                                                          testData.NewAssetValues.GarageDoorsCycloneStatus;
                    Reporting.AreEqual(expectedGarageDoorUpgradeStatus.ToString(),
                                       latestPolicyStateFromApi.HomeAsset.GetGarageDoorUpgradeStatus.ToString(),
                                       "the garage door upgrade status is as expected");
                    var expectedRoofImprovementStatus = testData.NewAssetValues.YearBuilt > 1982 ?
                                                          RoofImprovementStatus.NotSure :
                                                          testData.NewAssetValues.RoofImprovementCycloneStatus;
                    Reporting.AreEqual(expectedRoofImprovementStatus.ToString(),
                                       latestPolicyStateFromApi.HomeAsset.GetRoofImprovementStatus.ToString(),
                                       "the roof improvement status is as expected");
                }
                else
                {
                    Reporting.IsNull(latestPolicyStateFromApi.HomeAsset.IsPropertyElevated, "property elevated status is null as not a cyclone address");
                    Reporting.IsNull(latestPolicyStateFromApi.HomeAsset.HasCycloneShutters, "property cyclone shutters value is null as not a cyclone address");
                    Reporting.IsNull(latestPolicyStateFromApi.HomeAsset.GarageDoorUpgraded, "garage door status is null as not a cyclone address");
                    Reporting.IsNull(latestPolicyStateFromApi.HomeAsset.RoofImprovement, "roof improvement status is null as not a cyclone address");
                }
            }
        }
    }
}