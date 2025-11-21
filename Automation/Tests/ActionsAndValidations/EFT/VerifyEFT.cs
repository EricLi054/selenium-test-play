using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UIDriver.Pages.MicrosoftAD;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.EFT;
using static Rac.TestAutomation.Common.ClaimContact;

namespace Tests.ActionsAndValidations
{
    public class VerifyEFT
    {
        /// <summary>
        /// Verify the confirmation page for Cash Settlement Option selection and EFT page.
        /// Check for claim number included and confirmation page matches expectations.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="claimData"></param>       
        public static void VerifyConfirmationPage(Browser browser, ClaimContact claimData)
        {
            using (var confirmation = new Confirmation(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish();
                confirmation.WaitForPage();
                string expectedRegexText;
                browser.PercyScreenCheck(Constants.VisualTest.ClaimsEFT.VerifyItsYou);
                Reporting.Log("Confirmation Page", browser.Driver.TakeSnapshot());
                Reporting.AreEqual(claimData.ClaimNumber, confirmation.ClaimNumber, "Claim Number on Confirmation page");
                
                switch(claimData.ExpectedOutcome)
                {
                    case ExpectedCSFSOutcome.Accepted:
                        browser.PercyScreenCheck(Constants.VisualTest.ClaimsEFT.AcceptCashSettlementConfirmation);
                        expectedRegexText = FixedTextRegex.EFT_CONFIRMATION_MESSAGE_REGEX;
                        break;
                    case ExpectedCSFSOutcome.Declined:
                        browser.PercyScreenCheck(Constants.VisualTest.ClaimsEFT.DeclineCashSettlementConfirmation);
                        expectedRegexText = FixedTextRegex.CSFS_DECLINE_CASH_CONFIRMATION_MESSAGE_REGEX;
                        break;
                    case ExpectedCSFSOutcome.EFT:
                        browser.PercyScreenCheck(Constants.VisualTest.ClaimsEFT.EFTConfirmation);
                        expectedRegexText = FixedTextRegex.EFT_CONFIRMATION_MESSAGE_REGEX;
                        break;
                    default:
                        throw new NotImplementedException($"{claimData.ExpectedOutcome.GetDescription()} is not an valid claim settlement option for EFT");                        
                }

                var expectedTextRegex = new Regex(expectedRegexText);
                var displayedText = confirmation.ConfirmationMessage;
                var match = expectedTextRegex.Match(displayedText);
                Reporting.IsTrue(match.Success, $"confirmation message text. Received: {displayedText}");
            }
            browser.CloseBrowser();
        }

        /// <summary>
        /// Verify correct bank details added in the Shield
        /// </summary>
        /// <param name="claimData"></param>
        public static void VerifyBankDetailsInShield(ClaimContact claimData)
        {

            var contactDetails = DataHelper.GetContactDetailsViaContactId(claimData.Beneficiary.Id);
            var resultCount = contactDetails.BankAccounts.Count(x => x.Bsb == claimData.Beneficiary.BankAccounts.First().Bsb &&
                                                            x.AccountNumber == claimData.Beneficiary.BankAccounts.First().AccountNumber &&
                                                            x.AccountName == claimData.Beneficiary.BankAccounts.First().AccountName);

            switch (resultCount)
            {
                case 0:
                    Reporting.Error("Provided bank account detail is not added in Shield");
                    break;
                case 1:
                    Reporting.IsTrue(true, "Provided bank account details added in Shield");
                    break;
                default:
                    Reporting.Error("Duplicate bank account details added in Shield");
                    break;
            }
        }

        /// <summary>
        /// Applicable for CSFS Flow, where we wish to verify that
        /// Shield has correctly recorded the events
        /// </summary>
        /// <param name="claimData"></param>
        public static void VerifyCSFSEventInShield(ClaimContact claimData)
        {
            var shieldEvent = ShieldClaimDB.GetClaimEvents(claimData.ClaimNumber);

            switch (claimData.ExpectedOutcome)
            {
                case ClaimContact.ExpectedCSFSOutcome.Accepted:
                    Reporting.IsTrue(shieldEvent.Contains(ClaimContact.ExpectedCSFSOutcome.Accepted.GetDescription()), "Shield event displayed: Cash Settlement Accepted");
                    break;
                case ClaimContact.ExpectedCSFSOutcome.Declined:
                    Reporting.IsTrue(shieldEvent.Contains(ClaimContact.ExpectedCSFSOutcome.Declined.GetDescription()), "Shield event displayed: Cash Settlement Offer Rejected");
                    break;
                case ClaimContact.ExpectedCSFSOutcome.EFT:
                    Reporting.IsTrue(shieldEvent.Contains(ClaimContact.ExpectedCSFSOutcome.EFT.GetDescription()), "Shield event displayed: EFT Confirmation Received");
                    break;
                default:
                    throw new NotImplementedException($"CSFS Flow [{claimData.ExpectedOutcome}] has not been implemented as yet");

            }
        }

        /// <summary>
        /// Applicable for CSFS Flow, where we wish to verify that
        /// The CSFS link is expired after STS the token is consumed
        /// </summary>
        /// <param name="claimData"></param>

        public static void VerifyCSFSLinkExpired(Browser browser, string csfsLink)
        {
            browser.OpenUrl(csfsLink);
            using (var errorPage = new LinkExpired(browser))
            using (var authPage = new Authentication(browser))
            {
                authPage.WaitForADLoginPageOrDesiredLandingPage(errorPage);
                errorPage.WaitForPage();
                Reporting.Log("Link Expired Page:", browser.Driver.TakeSnapshot());

                var expectedTextRegex = new Regex(FixedTextRegex.CSFS_ERROR_HEADING_REGEX);
                var displayedText = errorPage.ErrorHeading;
                var match = expectedTextRegex.Match(displayedText);
                Reporting.IsTrue(match.Success, $"Error Message Heading: {displayedText}");

                expectedTextRegex = new Regex(FixedTextRegex.CSFS_ERROR_MESSAGE_REGEX);
                displayedText = errorPage.ErrorMessage;
                match = expectedTextRegex.Match(displayedText);
                Reporting.IsTrue(match.Success, $"Error Message: {displayedText}");
            }
        }

    }
}
