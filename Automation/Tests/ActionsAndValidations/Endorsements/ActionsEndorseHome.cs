using Rac.TestAutomation.Common;
using System;
using System.Text;
using System.Text.RegularExpressions;
using UIDriver.Pages.PCM;
using UIDriver.Pages.B2C;
using UIDriver.Pages.PCM.Home;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Tests.ActionsAndValidations
{
    public static class ActionsEndorseHome
    {
        /// <summary>
        /// Performs a Renewal endorsement on a given Home policy.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testData"></param>
        /// <returns>Policy Data</returns>
        public static PolicyData PerformHomePolicyRenewal(Browser browser, EndorseHome testData)
        {
            var currentState     = new PolicyData();

            browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);

            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                pcmHomePage.ViewSpecificPolicy(testData.PolicyNumber);

                currentState.AnnualPremium = pcmHomePage.PolicyAnnualPremium;
                currentState.RenewalDate   = pcmHomePage.PolicyRenewalDate.Date;

                Reporting.Log($"Policy {testData.PolicyNumber} shows a current premium of ${currentState.AnnualPremium} and a Renewal Date of {currentState.RenewalDate} in the pcmHome Page");

                Reporting.AreEqual(testData.OriginalPolicyData.AnnualPremium.Total, currentState.AnnualPremium, "Verifying AnnualPremium in pcmHome Page against the database.");

                if (currentState.RenewalDate.Date == DateTime.Now.Date)
                {
                    ActionsPCM.BeginHomePayPolicy(browser, testData.PolicyNumber);
                }
                else
                {
                    if (testData.PayMethod.IsMonthly)
                    {
                        var nextInstalment = testData.OriginalPolicyData.NextPendingInstallment();
                        Reporting.AreEqual(testData.OriginalPolicyData.RenewalDate.Date, currentState.RenewalDate, "Verifying RenewalDate in pcmHome Page against the database.");
                        Reporting.AreEqual(nextInstalment.Amount.Total, pcmHomePage.PolicyNextInstalmentAmount, "Verifying Next Instalment Amount in pcmHome Page against the database.");
                        Reporting.AreEqual(nextInstalment.CollectionDate.Date, pcmHomePage.PolicyNextInstalmentDate.Date, "Verifying Next Instalment Date in pcmHome Page against the database.");
                    }

                    ActionsPCM.BeginHomeRenewal(browser);
                }
            }

            var receiptNumber = RenewPolicyAndReturnToPorfolioSummary(browser, testData, currentState);

            if (testData.PayMethod.Scenario == PaymentScenario.AnnualCash)
            {
                Payment.VerifyWestpacQuickStreamDetailsAreCorrect<QuotePet>(cardDetails: testData.PayMethod.CreditCardDetails,
                                                                policyNumber: testData.PolicyNumber,
                                                                expectedPrice: currentState.AnnualPremium,
                                                                expectedReceiptNumber: receiptNumber);
            }

            return currentState;
        }
        private static string RenewPolicyAndReturnToPorfolioSummary(Browser browser, EndorseHome testData, PolicyData currentState)
        {
            var receiptNumber = string.Empty;

            using (var spinner         = new RACSpinner(browser))
            using (var endorsementPage = new RenewHomePolicy(browser))
            {
                if (currentState.RenewalDate.Date == DateTime.Now.Date)
                {
                    Reporting.Log("About to Pay for the Policy.", browser.Driver.TakeSnapshot());
                    endorsementPage.HandlePaymentPrompt(testData.PayMethod, expectedAmount: currentState.AnnualPremium);
                }
                else
                {
                    endorsementPage.UpdateHomePolicyDetails(testData);
                    currentState.AnnualPremium = endorsementPage.UpdateCoversAndGetPremium(browser, testData);

                    if (testData.PayMethod.Scenario == PaymentScenario.AnnualCash)
                    {
                        endorsementPage.VerifyPaymentOptionsRadioButtons(PaymentOptions.PayNow);
                        endorsementPage.HandlePaymentPrompt(testData.PayMethod, expectedAmount: currentState.AnnualPremium);
                    }
                }

                if (testData.PayMethod.Scenario == PaymentScenario.AnnualCash)
                    receiptNumber = endorsementPage.VerifyConfirmationAccordionDetails(testData.PayMethod, expectedAmount: currentState.AnnualPremium, expectedPolicyNumber: testData.PolicyNumber, isRenewalScenario: true);

                //If the policy renewal date is today then it shows paid your policy instead of renew your policy
                Regex thankYouTextRegex = currentState.RenewalDate.Date == DateTime.Now.Date ?
                    new Regex(@"^\s*Thank you\s*You've successfully paid your policy.$") :
                    new Regex(@"^\s*Thank you\s*You've successfully renewed your policy.$");
                var thankYouText = endorsementPage.WaitForConfirmationAndReturnToPolicyView();
                var match = thankYouTextRegex.Match(thankYouText);
                Reporting.IsTrue(match.Success, $"Policy renewal thank you text. Received: {thankYouText}");
            }

            return receiptNumber;
        }

        /// <summary>
        /// Performs the complete action of logging into PCM and viewing a
        /// given home policy, then completes the "Change my home details" endorsement
        /// on that policy, returning to portfolio summary at the end.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testData"></param>
        /// <returns>The current values of the policy before endorsement.</returns>
        public static PolicyData PerformChangeMyHomeDetailsEndorsement(Browser browser, EndorseHome testData)
        {
            var currentValue     = new PolicyData();

            browser.LoginMemberToPCMAndDisplayPolicy(testData.ActivePolicyHolder.Id, testData.PolicyNumber);

            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                pcmHomePage.ViewSpecificPolicy(testData.PolicyNumber);
                currentValue.AnnualPremium = pcmHomePage.PolicyAnnualPremium;
                currentValue.HomeCovers = new CoverHome()
                {
                    CoverType              = pcmHomePage.HomeCover,
                    BuildingSumInsured     = testData.NewAssetValues.BuildingValue.HasValue ? pcmHomePage.BuildingSumInsured : 0,
                    BuildingExcess         = testData.NewAssetValues.BuildingValue.HasValue ? pcmHomePage.BuildingExcess : null,
                    ContentsSumInsured     = testData.NewAssetValues.ContentsValue.HasValue ? pcmHomePage.ContentsSumInsured : 0,
                    ContentsExcess         = testData.NewAssetValues.ContentsValue.HasValue ? pcmHomePage.ContentsExcess : null
                };

                currentValue.HomeAddress = pcmHomePage.HomeAddress;

                var logMessage = new StringBuilder($"Policy {testData.PolicyNumber} has current premium ${currentValue.AnnualPremium} and current home address {currentValue.HomeAddress}");
                if (testData.NewAssetValues.BuildingValue.HasValue)
                {
                    logMessage.AppendLine($"Current Building sum insured: {currentValue.HomeCovers.BuildingSumInsured}");
                    logMessage.AppendLine($"Current Building excess:      {currentValue.HomeCovers.BuildingExcess}");
                }
                if (testData.NewAssetValues.ContentsValue.HasValue)
                {
                    logMessage.AppendLine($"Current Contents sum insured: {currentValue.HomeCovers.ContentsSumInsured}");
                    logMessage.AppendLine($"Current Contents excess:      {currentValue.HomeCovers.ContentsExcess}");
                }
                Reporting.Log(logMessage.ToString());
                pcmHomePage.VerifyExcessText(testData);
            }

            ActionsPCM.BeginChangeMyHomeDetailsEndorsement(browser, testData.PolicyNumber);
            var endorsedData = CompleteChangeMyHomeDetailsAndReturnToPorfolioSummary(browser, testData);

            if (testData.IsExpectedToMakeOneOffPayment())
            {
                Payment.VerifyWestpacQuickStreamDetailsAreCorrect<QuotePet>(cardDetails: testData.PayMethod.CreditCardDetails,
                                                                policyNumber: testData.PolicyNumber,
                                                                expectedPrice: endorsedData.premiumChange,
                                                                expectedReceiptNumber: endorsedData.receiptNumber);
            }

            return currentValue;
        }

        private static (decimal premiumChange, string receiptNumber) CompleteChangeMyHomeDetailsAndReturnToPorfolioSummary(Browser browser, EndorseHome testData)
        {
            decimal premiumChange     = 0;
            var receiptNumber         = string.Empty;

            using (var endorsementPage = new ChangeMyHomeDetails(browser))
            {
                endorsementPage.UpdateMyHomeDetails(testData);
                endorsementPage.WaitForPremiumChangeAccordion();
                endorsementPage.VerifyExcessMsg(testData);
                premiumChange = endorsementPage.UpdatePolicyExcessAndSIAndProceed(testData);
                
                VerifyPremiumChange(testData, premiumChange);

                if (testData.IsExpectedToMakeOneOffPayment() ||
                    testData.IsExpectedToReceiveRefund())
                {
                    endorsementPage.WaitForPaymentDetails(testData.ExpectedImpactOnPremium);
                    endorsementPage.HandlePaymentOrRefundPrompt(testData, expectedAmount: premiumChange);
                }

                if (testData.IsExpectedToMakeOneOffPayment())
                    receiptNumber = endorsementPage.VerifyConfirmationAccordionDetails(testData.PayMethod, expectedAmount: premiumChange, expectedPolicyNumber: testData.PolicyNumber, isRenewalScenario: false);

                Regex thankYouTextRegex = new Regex(FixedTextRegex.POLICY_CHANGE_MY_HOME_DETAILS_SUCCESS_REGEX);
                var thankYouText        = endorsementPage.WaitForConfirmationAndReturnToPolicyView();
                Match match             = thankYouTextRegex.Match(thankYouText);
                Reporting.IsTrue(match.Success, $"endorsement thank you text. Received: {thankYouText}");
            }

            return (premiumChange, receiptNumber);
        }

        private static void VerifyPremiumChange(EndorseHome testData, decimal premiumChangeValue)
        {
            switch (testData.ExpectedImpactOnPremium)
            {
                case PremiumChange.PremiumIncrease:
                    Reporting.IsTrue(premiumChangeValue > 0, $"observed premium change ({premiumChangeValue}) should be greater than zero to reflect an increase in premium.");
                    break;
                case PremiumChange.PremiumDecrease:
                    Reporting.IsTrue(premiumChangeValue < 0, $"observed premium change ({premiumChangeValue}) should be less than zero to reflect an decrease in premium.");
                    break;
                case PremiumChange.NoChange:
                    Reporting.IsTrue(premiumChangeValue == 0, $"observed premium change ({premiumChangeValue}) should be zero to reflect no change in premium.");
                    break;
                case PremiumChange.NotApplicable:
                default:
                    // Nothing to do
                    break;
            }
        }
        
    }
}