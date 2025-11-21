using Rac.TestAutomation.Common;
using UIDriver.Pages;
using UIDriver.Pages.B2C;
using UIDriver.Pages.PCM;
using UIDriver.Pages.PCM.CertificateOfCurrency;
using UIDriver.Pages.PCM.Home;

using static Rac.TestAutomation.Common.Constants.General;
using System.Threading;

namespace Tests.ActionsAndValidations
{
    public static class ActionsPCM
    {
        public static void LoginMemberToPCM(this Browser browser, string contactId)
        {
            browser.CloseBrowser();
            Thread.Sleep(SleepTimes.T5SEC);

            /***********************************************************
             * Open B2C launch page.
             ***********************************************************/
            LaunchPage.OpenB2CLandingPageAndFeatureToggles(browser);

            using (var launchPage = new LaunchPage(browser))
            {
                launchPage.GotoPCMLoginOptions();
            }

            using (var pcmLoginPage = new PCMLogon(browser))
            {
                pcmLoginPage.LogInToPortfolioSummary(contactId);
            }
        }

        public static void LoginMemberToPCMAndBeginNewMotorQuote(this Browser browser, Contact contact)
        {
            /***********************************************************
             * Open B2C launch page.
             ***********************************************************/
            LaunchPage.OpenB2CLandingPageAndFeatureToggles(browser);

            using (var launchPage = new LaunchPage(browser))
            {
                launchPage.GotoPCMLoginOptions();
            }

            using (var pcmLoginPage = new PCMLogon(browser))
            {
                pcmLoginPage.LogInToPortfolioSummaryAndStartNewQuote(contact.Id);
            }

            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                pcmHomePage.WaitForPage();
                System.Threading.Thread.Sleep(3000);  // Done as rendering of page is slow, even though DOM is populated.
                pcmHomePage.NewQuotePromptClickCarInsurance();
                System.Threading.Thread.Sleep(3000);  // Done as rendering of page is slow, even though DOM is populated.
                pcmHomePage.NewQuotePromptAnswerRadioButtons(true, true);
                System.Threading.Thread.Sleep(3000);  // Done as rendering of page is slow, even though DOM is populated.
                var prefillCheck = pcmHomePage.VerifyPrefillDetails(contact);
                Reporting.Log("Check of prefill is: " + prefillCheck, browser.Driver.TakeSnapshot());
                Reporting.IsTrue(prefillCheck, "Member central returned expected values.");
                pcmHomePage.ClickPrefillDetailsCorrect();
            }
        }

        /// <summary>
        /// Logs into PCM for a given contact, and then navigates the
        /// portfolio summary carousel to find a specific policy.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testConfig"></param>
        /// <param name="contactId"></param>
        /// <param name="policyNumber"></param>
        public static void LoginMemberToPCMAndDisplayPolicy(this Browser browser, string contactId, string policyNumber)
        {
            // Log into PCM
            browser.LoginMemberToPCM(contactId);

            NavigateCarouselToSpecificPolicy(browser, policyNumber);
            Reporting.Log("Navigated to correct policy.", browser.Driver.TakeSnapshot());
        }

        public static void BeginClaimOnPolicy(Browser browser, string policyNumber)
        {
            NavigateCarouselToSpecificPolicy(browser, policyNumber);

            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                pcmHomePage.ClickMakeAClaim();
            }
        }

        /// <summary>
        /// Check whether the policy is eligible for making a payment. 
        /// The policy should be eligible for making a payment if the payment failed previously
        /// </summary>
        /// <param name="isFailedPayment">Pass True if the scenario is for Failed payment.</param>
        public static void CheckMakePaymentEligibility(Browser browser, bool isFailedPayment)
        {
            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                if (isFailedPayment)
                {
                    Reporting.IsTrue(pcmHomePage.IsMakeAPaymentButtonShown, "'Make A Payment ' button is displayed");
                    Reporting.IsTrue(pcmHomePage.IsRenewalButtonShown, "'Renew Policy' button is displayed");
                }
                else
                {
                    Reporting.IsFalse(pcmHomePage.IsMakeAPaymentButtonShown, "'Make A Payment ' button is NOT displayed");
                    Reporting.IsFalse(pcmHomePage.IsRenewalButtonShown, "'Renew Policy' button is NOT displayed");
                }
            }
        }

        public static void BeginHomeRenewal(Browser browser)
        {
            using (var pcmHomePage = new PortfolioSummary(browser))
            using (var spinner = new RACSpinner(browser))
            using (var renewMyPolicyPage = new RenewHomePolicy(browser))
            {
                pcmHomePage.ClickRenewMyPolicy();

                spinner.WaitForSpinnerToFinish(nextPage: pcmHomePage);

                pcmHomePage.ClickNoContinueToPolicy();

                spinner.WaitForSpinnerToFinish(nextPage: renewMyPolicyPage);
            }
        }

        public static void BeginHomePayPolicy(Browser browser, string policyNumber)
        {
            using (var pcmHomePage = new PortfolioSummary(browser))
            using (var spinner = new RACSpinner(browser))
            using (var westpacQuickStream = new WestpacQuickStream(browser))
            {
                pcmHomePage.ClickMakeAPayment();

                spinner.WaitForSpinnerToFinish(nextPage: westpacQuickStream);
            }
        }

        public static void NavigateCarouselToSpecificPolicy(Browser browser, string policyNumber)
        {
            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                pcmHomePage.WaitForPage();
                pcmHomePage.ViewSpecificPolicy(policyNumber);
            }
        }


        public static void ViewExistingClaim(Browser browser, string claimNumber)
        {
            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                pcmHomePage.WaitForPage();

                pcmHomePage.CurrentPCMTab = PortfolioSummary.PCM_TAB.CLAIMS;

                pcmHomePage.ViewSpecificClaim(claimNumber);
            }
        }

        /// <summary>
        /// For the given policy, launch the cancellation page for the member's policy.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="policyNumber"></param>
        public static void BeginCancelPolicy(Browser browser, string policyNumber)
        {
            NavigateCarouselToSpecificPolicy(browser, policyNumber);

            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                Reporting.IsTrue(pcmHomePage.IsCancelMyPolicyButtonShown, "Expect button to cancel a policy to be present");
                pcmHomePage.ClickCancelMyPolicy();
            }
        }

        public static void ViewMailPreferences(Browser browser)
        {
            using (var pcmHomePage = new PortfolioSummary(browser))
            {
                pcmHomePage.WaitForPage();
                pcmHomePage.CurrentPCMTab = PortfolioSummary.PCM_TAB.MAILPREFERENCES;
            }
        }

        public static void BeginChangeMyHomeDetailsEndorsement(Browser browser, string policyNumber)
        {
            NavigateCarouselToSpecificPolicy(browser, policyNumber);

            using (var pcmHomePage = new PortfolioSummary(browser))
            using (var spinner = new RACSpinner(browser))
            using (var changeHomeDetailsPage = new ChangeMyHomeDetails(browser))
            {
                pcmHomePage.ClickChangeMyHomeDetails();
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T30SEC);

                pcmHomePage.ClickNoContinueToPolicy();
                spinner.WaitForSpinnerToFinish(nextPage: changeHomeDetailsPage);
            }
        }

        /// <summary>
        /// Logs into PCM to find a specified policy and attempt to
        /// have its Certificate of Currency emailed to a specified email address.
        /// NOTE: Only certain policies products and covers support provision
        /// of a certificate of currency.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="policyHolder"></param>
        /// <param name="policyNumber"></param>
        public static void FetchCertificateOfCurrencyForPolicy(Browser browser, Contact policyHolder, string policyNumber)
        {
            LoginMemberToPCMAndDisplayPolicy(browser, policyHolder.Id, policyNumber);

            using (var portfolioSummary = new PortfolioSummary(browser))
            using (var cocPage = new RetrieveCertificateOfCurrency(browser))
            {
                portfolioSummary.ClickGetCertificateOfCurrency();
                cocPage.WaitForPage();
                cocPage.RequestCertificateOfCurrencyByEmailAndReturnToPortfolioSummary(policyHolder.GetEmail());
            }
        }
    }
}
