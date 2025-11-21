using System;
using System.Linq;
using System.Text.RegularExpressions;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using UIDriver.Pages.PCM;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Tests.ActionsAndValidations
{
    public static class VerifyEndorseCaravan
    {
        /// <summary>
        /// Verify the policy premium and parking suburb in the PCM Portfolio
        /// Summary after a Change Where I Keep My Car endorsement.
        /// </summary>
        public static void VerifyPolicyPremiumAndParkingSuburb(Browser browser, EndorseCaravan testData)
        {
            var originalPremium = testData.OriginalPolicyData.TotalInstallmentPremium.Total;
            var changedPremiumAmount = testData.PremiumChangesAfterEndorsement.Total;

            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                pcmHomePage.WaitForPage();
                pcmHomePage.ViewSpecificPolicy(testData.PolicyNumber);

                Reporting.Log($"The Original Annual Premium before endorsement is ${originalPremium} and change in premium amount due to endorsement is ${changedPremiumAmount}");

                switch (testData.ExpectedImpactOnPremium)
                {
                    case PremiumChange.PremiumIncrease:                        
                        Reporting.IsTrue(testData.OriginalPolicyData.AnnualPremium.Total < pcmHomePage.PolicyAnnualPremium, 
                            $"New premium ({pcmHomePage.PolicyAnnualPremium}) is greater than original premium ({testData.OriginalPolicyData.AnnualPremium.Total})");
                        Reporting.Log($"The expected new Annual Premium after endorsement should be equal to Old Annual Premium(before endorsement) ${originalPremium} + increased Premium amount ${changedPremiumAmount} = {originalPremium + changedPremiumAmount}.");
                        Reporting.AreEqual(originalPremium + changedPremiumAmount, pcmHomePage.PolicyAnnualPremium,
                            $"New annual premium (${pcmHomePage.PolicyAnnualPremium}) is matched with expected (${originalPremium + changedPremiumAmount})");
                        if (testData.SparkExpandedPayment.PaymentFrequency == PaymentFrequency.Monthly)
                        {
                            Reporting.AreEqual(pcmHomePage.PolicyNextInstalmentAmount, testData.PremiumChangesAfterEndorsement.TotalPremiumMonthly,
                                $"New monthly installment amount (${pcmHomePage.PolicyNextInstalmentAmount}) is matched with expected (${testData.PremiumChangesAfterEndorsement.TotalPremiumMonthly})");
                        }
                        break;
                    case PremiumChange.PremiumDecrease:
                        Reporting.IsTrue(testData.OriginalPolicyData.AnnualPremium.Total > pcmHomePage.PolicyAnnualPremium, 
                            $"New premium ({pcmHomePage.PolicyAnnualPremium}) is lower than original premium ({testData.OriginalPolicyData.AnnualPremium.Total})");
                        Reporting.Log($"The expected new Annual Premium after endorsement should be equal to Old Annual Premium(before endorsement) ${originalPremium} - decreased Premium amount ${changedPremiumAmount} = {originalPremium - changedPremiumAmount}.");
                        Reporting.AreEqual(originalPremium - changedPremiumAmount, pcmHomePage.PolicyAnnualPremium,
                           $"New annual premium (${pcmHomePage.PolicyAnnualPremium}) is matched with expected (${originalPremium - changedPremiumAmount})");
                        break;
                    default:
                        Reporting.IsTrue(testData.OriginalPolicyData.AnnualPremium.Total == pcmHomePage.PolicyAnnualPremium, 
                            $"New premium ({pcmHomePage.PolicyAnnualPremium}) is the same as original premium ({testData.OriginalPolicyData.AnnualPremium.Total})");
                        break;
                }

                var policyDetails = DataHelper.GetPolicyDetails(testData.PolicyNumber);

                // TODO: B2C-4561 Remove toggle and old Risk Suburb references as
                // appropriate when removing toggle from B2C/PCM Functional code.
                // If non-null address, we would have changed parking address:
                if (testData.ParkingAddress != null)
                {
                    if (Config.Get().IsMotorRiskAddressEnabled() && policyDetails.MotorAsset.Address != null)
                    {
                        Reporting.AreEqual($"{testData.ParkingAddress.StreetSuburbStateShortened(longStreetType: false)} {testData.ParkingAddress.PostCode}", 
                            pcmHomePage.MotorPolicyParkingRiskAddress, ignoreCase: true, "expected value for 'Street address where your car is parked overnight' with displayed value");
                    }
                    else
                    {
                        Reporting.AreEqual(testData.ParkingAddress.Suburb, pcmHomePage.MotorPolicyParkingSuburb, true);
                    }
                }
                else
                {
                    var mailingAddress = testData.ActivePolicyHolder.MailingAddress;
                    Reporting.IsTrue(mailingAddress.IsEqualToString(pcmHomePage.GetMailingAddressFromPolicySummary), $"member mailing address {pcmHomePage.GetMailingAddressFromPolicySummary} is as expected {mailingAddress.QASStreetAddress()}.");
                }

                // Checking Agreed Value, Content Cover             
                Reporting.AreEqual(DataHelper.StripMoneyNotations(testData.NewAgreedValue), ((int)policyDetails.Covers[0].SumInsured).ToString(), "expected 'Agreed value'");

                if (testData.OriginalPolicyData.Covers[1].CoverTypeDescription.Equals("Caravan Contents"))
                {
                   Reporting.AreEqual(DataHelper.StripMoneyNotations(testData.ContentCover), ((int)policyDetails.Covers[1].SumInsured).ToString(), "Content Cover is displayed");
                }

            }
        }

        public static void VerifyPolicyStateInPcmAfterRenewal(Browser browser, EndorseCaravan testData)
        {
            Reporting.Log("Opening PCM to verify the details of the renewed policy");
            browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);

            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                pcmHomePage.WaitForPage();
                pcmHomePage.ViewSpecificPolicy(testData.PolicyNumber);

                Reporting.IsFalse(pcmHomePage.IsRenewalNoticeShown, "Renewal notice should no longer be visible in PCM.");
                if (testData.SparkExpandedPayment.PaymentFrequency == PaymentFrequency.Monthly)
                {
                    Reporting.AreEqual(testData.PremiumChangesAfterEndorsement.TotalPremiumMonthly, pcmHomePage.PolicyAnnualPremium, 
                        "expected value for Total Annual Premium for Monthly instalments matches the value displayed in PCM");
                }
                else
                {
                    Reporting.AreEqual(testData.PremiumChangesAfterEndorsement.Total, pcmHomePage.PolicyAnnualPremium, 
                        "expected annual premium reflects new premium value in PCM.");
                }

                VerifyPolicyDetailsViaApi(testData.PolicyNumber, testData, pcmHomePage);
            }
        }

        public static void VerifyPolicyPaymentDetailsInShield(EndorseCaravan testData)
        {
            // Verify Payment details
            Reporting.Log($"Begin verify payment details from Shield DB. Param/s = {testData.PolicyNumber}");

            var dbPaymentDetails = ShieldPolicyDB.FetchPaymentDetailsForPolicy(testData.PolicyNumber);
            var recentEvents = ShieldPolicyDB.FetchRecentEventsOnPolicy(testData.PolicyNumber);

            Reporting.IsTrue(testData.PayMethod.PaymentFrequency == dbPaymentDetails.PaymentFrequency, "payment frequency is expected value in DB");

            if (testData.PayMethod.IsPaymentByBankAccount)
            {
                Reporting.AreEqual(DIRECT_DEBIT, dbPaymentDetails.PaymentMethod);
            }
            else
            {
                if (testData.PayMethod.IsAnnual)
                {
                    Reporting.AreEqual(CASH, dbPaymentDetails.PaymentMethod);
                    VerifyEndorsePolicy.VerifyEndorsementEventForAnnualCashPayment(recentEvents, testData.ExpectedImpactOnPremium);
                }
                else
                {
                    Reporting.AreEqual(CREDIT_CARD, dbPaymentDetails.PaymentMethod);
                }
            }

            if (testData.PayMethod.Scenario != PaymentScenario.AnnualCash)
            {
                VerifyEndorsePolicy.VerifyEndorsementEventForNonAnnualCashPayment(recentEvents);
            }

            var expectedPaymentFrequency = testData.PayMethod.NumberOfPayments;
            Reporting.AreEqual(expectedPaymentFrequency, dbPaymentDetails.PaymentCount, "count of payments in DB is expected value");

            VerifyPolicy.VerifyPolicyEndorsementInstalments(testData, renewalDate: null);
        }

        /// <summary>
        /// Verify the policy details in Shield
        /// </summary>
        /// <param name="isFailedPayment">Set to true in case of failed payment scenario to verify the generated event. </param>
        public static void VerifyPolicyDetailsInShield(EndorseCaravan testData, bool isFailedPayment)
        {
            Reporting.Log($"Begin verify policy details from Shield. Param/s = {testData.PolicyNumber}");

            // Verify events
            var recentEvents = ShieldPolicyDB.FetchRecentEventsOnPolicy(testData.PolicyNumber);

            // Generated events are different for failed payments eventhough it is annual cash.
            if (testData.SparkExpandedPayment.PaymentOption == PaymentOptionsSpark.AnnualCash && !isFailedPayment)
            {
                VerifyEndorsePolicy.VerifyEndorsementEventForAnnualCashPayment(recentEvents);
            }
            else
            {
                VerifyEndorsePolicy.VerifyEndorsementEventForNonAnnualCashPayment(recentEvents);
            }

            // Verify Contact logged
            ShieldPolicyDB.FetchShieldEventDetailsOnPolicy(testData.PolicyNumber,
                                                           out var _,
                                                           out var shieldEventName,
                                                           out var loggedInB2CContactName);
            var expectedLoggedContact = DataHelper.GetContactDetailsViaContactId(testData.ActivePolicyHolder.Id);

            Reporting.AreEqual(expectedLoggedContact.GetLoggedContact(), loggedInB2CContactName, "logged contact");
            Reporting.AreEqual("General change", shieldEventName);

            // Verify endorsement reason
            var policyEndorsementReasons = ShieldPolicyDB.FetchEndorsementReasonDetailsOnPolicy(testData.PolicyNumber);
            var endorseForRenewal = policyEndorsementReasons.Single(s => s.Id == EndorsementReasonOption.EndorseForRenewal.Id);
            Reporting.IsNotNull(endorseForRenewal, $"Endorse for renewal reasons found");

            // Verify payments
            var dbPaymentDetails = ShieldPolicyDB.FetchPaymentDetailsForPolicy(testData.PolicyNumber);
            Reporting.IsTrue(testData.SparkExpandedPayment.PaymentFrequency == dbPaymentDetails.PaymentFrequency, "payment frequency is expected value in DB");

            var expectedNumberOfPayments = testData.SparkExpandedPayment.NumberOfPayments;
            Reporting.AreEqual(expectedNumberOfPayments, dbPaymentDetails.PaymentCount, "count of payments in DB is expected value");
            
            var policyDetails = DataHelper.GetPolicyDetails(testData.PolicyNumber);

            //Verify premium details Shield
            if (testData.SparkExpandedPayment.IsAnnual)
            {
                Reporting.AreEqual(testData.PremiumChangesAfterEndorsement.Total, (decimal)policyDetails.AnnualPremium.Total, "Premium total amount");
            }
            else
            {
                Reporting.AreEqual(testData.PremiumChangesAfterEndorsement.Total, (decimal)policyDetails.NextPayableInstallment.OutstandingAmount, "Premium total amount");
            }
            Reporting.AreEqual(testData.PremiumChangesAfterEndorsement.BaseAmount, (decimal)policyDetails.AnnualPremium.BaseAmount, "Premium base amount");
            Reporting.AreEqual(testData.PremiumChangesAfterEndorsement.StampDuty, (decimal)policyDetails.AnnualPremium.StampDuty, "Premium stamp duty");
            Reporting.AreEqual(testData.PremiumChangesAfterEndorsement.Gst, (decimal)policyDetails.AnnualPremium.Gst, "Premium GST amount");

            // Verify installments
            // Motor renewal flow allow member to change PaymentFrequency.
            // PaymentFrequency OR CollectionMethod changes willtrigger re-create payment installments
            if ((testData.PayMethod.PaymentFrequency != testData.SparkExpandedPayment.PaymentFrequency) ||
                (testData.PayMethod.IsPaymentByBankAccount && testData.SparkExpandedPayment.CreditCardDetails != null && testData.SparkExpandedPayment.BankAccountDetails == null) ||
                (!testData.PayMethod.IsPaymentByBankAccount && testData.SparkExpandedPayment.CreditCardDetails == null && testData.SparkExpandedPayment.BankAccountDetails != null))
            {

                // Verify number of installments been sets to cancelled
                var cancelledInstallmentsCount = policyDetails.Installments.Count(x => x.Status == Status.Cancelled.GetDescription());
                Reporting.AreEqual(testData.PayMethod.NumberOfPayments, cancelledInstallmentsCount, "count of installments with status cancelled");

                // Verify number of installments created and sets to Pending.
                var pendingInstallmentsCount = policyDetails
                    .Installments
                    .Count(x => x.Status == Status.Pending.GetDescription()
                                && x.Type == "Recurring"
                                && x.CollectionDate > DateTime.Now.Date);
                Reporting.AreEqual(testData.SparkExpandedPayment.NumberOfPayments, pendingInstallmentsCount, "count of installments with status pending");
            }

            // Verify payment account
            // Spark input bank account details to the payment
            if (testData.SparkExpandedPayment.BankAccountDetails != null)
            {
                VerifyEndorsePolicy.VerifyUpdateOccursInShieldWithBankAccountAsSource(testData.PolicyNumber, testData.SparkExpandedPayment.BankAccountDetails);
            }

            // Verify payment account
            // Spark input credit card details to the payment
            if (testData.SparkExpandedPayment.CreditCardDetails != null && testData.SparkExpandedPayment.PaymentOption != PaymentOptionsSpark.AnnualCash)
            {
                VerifyEndorsePolicy.VerifyUpdateOccursInShieldWithCreditCardAsSource(testData.PolicyNumber, testData.SparkExpandedPayment.CreditCardDetails);
            }   
        }

        private static void VerifyPolicyDetailsViaApi(string policyNumber, EndorseCaravan testData, PortfolioSummary pcmHomePage)
        {
            var policyDetails = DataHelper.GetPolicyDetails(policyNumber);

            Reporting.Log("Verifying policy details saved in Shield after endorsement via API...");
            // Rego
            var expectedRego = testData.NewInsuredAsset?.Registration;
            if (string.IsNullOrEmpty(expectedRego))
            {
                // IF we hadn't defined a new rego in "NewInsuredAsset" then we'll check the original policy value
                expectedRego = string.IsNullOrEmpty(testData.OriginalPolicyData.CaravanAsset.RegistrationNumber) ?
                                              "TBA" : testData.OriginalPolicyData.CaravanAsset.RegistrationNumber;
            }
            Reporting.AreEqual(expectedRego, policyDetails.CaravanAsset.RegistrationNumber, ignoreCase: true, "Vehicle registration has correct value saved in Shield after endorsement");

            if (DataHelper.IsRegistrationNumberConsideredValid(expectedRego))
            {
                Reporting.AreEqual(expectedRego, pcmHomePage.Registration, ignoreCase: true, "Vehicle registration is as expected after endorsement from PCM");
            }

            // Financier
            var expectedFinancier = testData.Financier == null ?
                                    testData.OriginalPolicyData.GetFinancierNameViaShieldAPI() :
                                    testData.Financier;

            if (string.IsNullOrEmpty(expectedFinancier))
            {
                Reporting.IsNull(testData.OriginalPolicyData.CaravanAsset.FinancierExternalContactId, "This policy has no financier");
            }
            else
            {
                Reporting.AreEqual(expectedFinancier, policyDetails.GetFinancierNameViaShieldAPI(), ignoreCase: true, "Financier has correct value saved in Shield after endorsement");

                if (!string.IsNullOrEmpty(pcmHomePage.Financier))
                {
                    Reporting.AreEqual(expectedFinancier, pcmHomePage.Financier, ignoreCase: true, "Financier is as expected after endorsement from PCM");
                }
                else
                {
                    Reporting.Log("PCM Home Page does not display a financier.");
                }
            }


            // VehicleID and Vehicle Description
            var expectedVehicle = testData.ChangeMakeAndModel ? 
                                       testData.NewInsuredAsset : 
                                       testData.InsuredAsset;        
            Reporting.AreEqual(expectedVehicle.VehicleId, policyDetails.GetVehicleId(), ignoreCase: true, "VehicleID has correct value saved in Shield after endorsement");

            var vehicle = DataHelper.GetVehicleDetails(policyDetails.CaravanAsset.VehicleId).Vehicles[0];

            Reporting.AreEqual(vehicle.MakeDescription.Trim(), pcmHomePage.CaravanMake, ignoreCase: true, "Vehicle make is as expected after endorsement from PCM");
            Reporting.AreEqual(vehicle.ModelDescription.Trim(), pcmHomePage.CaravanModel.RemoveDuplicateWhiteSpaceAndTrim(), ignoreCase: true, "Vehicle model is as expected after endorsement from PCM");

            // Vehicle Excess
            string excessFromShield = policyDetails.Excess().FirstOrDefault().Value.ToString();
            string originalExcess = testData.OriginalPolicyData.Excess().FirstOrDefault().Value.ToString();

            string expectedExcess = string.IsNullOrEmpty(testData.Excess) ?
                                         originalExcess :
                                         testData.Excess.Replace("$", "");
            Reporting.AreEqual(expectedExcess, excessFromShield, ignoreCase: true, "Vehicle excess has correct value saved in Shield after endorsement");

            // Vehicle Sum Insured
            var originalSumInsuredValue = testData.OriginalPolicyData.Covers.First().SumInsured;

            var expectedSumInsuredValue = testData.ChangeMakeAndModel ?
                                          testData.NewInsuredAsset.MarketValue :
                                          originalSumInsuredValue;

            var sumInsuredValue = policyDetails.Covers.First().SumInsured.ToString().Replace(".0", "").Trim();

            Reporting.AreEqual(expectedSumInsuredValue.ToString().Replace(".0", "").Trim(), sumInsuredValue, "Vehicle sum insured has correct value saved in Shield after endorsement");
            if (pcmHomePage.IsSumInsuredDisplayed)
            {
                Reporting.AreEqual(Math.Floor(expectedSumInsuredValue), pcmHomePage.SumInsured, "Vehicle sum insured is as expected after endorsement from PCM");
            }
        }
    }
}
