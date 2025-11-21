using NUnit.Framework;
using NUnit.Framework.Internal;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Quote;
using Tests.ActionsAndValidations;
using Rac.TestAutomation.Common.DatabaseCalls.Contacts;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UIDriver.Pages;
using UIDriver.Pages.B2C;
using UIDriver.Pages.PCM;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace B2C.NewBusiness
{
    [TestFixture]
    [Property("Functional", "B2C prepopulated quote tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class PrePopulatedQuoteMotor : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Mandatory motor pre-populated quote tests.");
        }

        /// <summary>
        /// Automated version of mandatory regression test:
        /// "B2C- Logged in - member takes out pre-populated quote"
        /// 
        /// Test emulates the scenario of a prep-populated motor quote
        /// initiated from a member's logged in MyRAC session.
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Motor), Category(TestCategory.B2CPCM),
            Category(TestCategory.New_Business), Category(TestCategory.Westpac_Payment),
            Category(TestCategory.Mock_Member_Central_Support)]
        public void INSU_T251_LoggedInMemberTakesOutPrePopulatedQuote()
        {
            decimal firstPayment   = 0;
            string  insuredVehicle = null;
            string  policyNumber   = null;
            string receiptNumber   = string.Empty;

            var testVehicle = BuildTestDataMCValidContactMotorPPQB2CT2013();
            var testMember = testVehicle.Drivers[0].Details;

            Reporting.IsNotNull(testMember, "valid test data of a member with current RAC membership.");

            _browser.LoginMemberToPCMAndBeginNewMotorQuote(testMember);

            insuredVehicle = VerifyAndSubmitPage1InitialDetails(testVehicle);

            ActionsQuoteMotor.UpdateAndSubmitInitialQuotePage(browser: _browser, vehicleQuote: testVehicle, agreedQuotePrice: out firstPayment);

            VerifyAndSubmitPage3PHDetails(testVehicle);

            VerifyQuoteMotor.VerifyQuoteSummaryPage(browser: _browser,
                                                            vehicleQuote: testVehicle,
                                                            insuredVehicle: insuredVehicle,
                                                            isPPQ: true);
            ActionsQuoteMotor.AcceptQuoteSummary(_browser);

            ActionsQuoteMotor.SubmitPayment(browser: _browser, vehicleQuote: testVehicle, expectedPrice: firstPayment);

            policyNumber = VerifyQuoteMotor.VerifyMotorVehicleConfirmationPage(browser: _browser,
                                                                               vehicleQuote: testVehicle,
                                                                               expectedPrice: firstPayment,
                                                                               insuredVehicle: insuredVehicle, 
                                                                               receiptNumber: out receiptNumber);

            if (testVehicle.PayMethod.Scenario == PaymentScenario.AnnualCash)
                Payment.VerifyWestpacQuickStreamDetailsAreCorrect<QuoteCar>(cardDetails: testVehicle.PayMethod.CreditCardDetails,
                                                            policyNumber: policyNumber,
                                                            expectedPrice: firstPayment,
                                                            expectedReceiptNumber: receiptNumber,
                                                            isMotorQuoteIncludingRoadside: testVehicle.AddRoadside);

            // Verify policy against Shield
            VerifyQuoteMotor.VerifyMotorVehiclePolicyInShield(testVehicle, policyNumber);
        }

        /// <summary>
        /// Automated version of mandatory regression test:
        /// "B2C- Logged in - member takes out pre-populated quote"
        /// 
        /// Test verifies the mandatory state of the radio buttons in the
        /// initial prompt when a PPQ motor quote is begun from PCM.
        /// </summary>
        [Test, Category(TestCategory.Regression), Category(TestCategory.Motor), Category(TestCategory.B2CPCM),
            Category(TestCategory.New_Business), Category(TestCategory.Mock_Member_Central_Support)]
        public void INSU_T253_LoggedInMemberPPQ_MandatoryErrorButtons()
        {
            /* For test expediency, we're going to cheat a little here.
             * Reason why I think we can get away with this, is because:
             * a) the test does not get to the point to fetching contact from MC
             * b) we just checking the validation on the radio buttons, these don't care if the contact even exists
             */
            var testMember = new Contact(contactId: "3310812");

            MemberCentral.PopulateMockMemberCentralWithLatestContactIdInfo(testMember.Id);
            
            Reporting.IsNotNull(testMember, "valid test data of a member with current RAC membership.");
            /***********************************************************
             * Open B2C launch page.
             ***********************************************************/
            LaunchPage.OpenB2CLandingPageAndFeatureToggles(_browser);

            using (var launchPage = new LaunchPage(_browser))
            {
                launchPage.GotoPCMLoginOptions();
            }

            using (var pcmLoginPage = new PCMLogon(_browser))
            {
                pcmLoginPage.LogInToPortfolioSummaryAndStartNewQuote(testMember.Id.ToString());
            }

            using (var pcmHomePage = new PortfolioSummary(_browser))
            {
                pcmHomePage.WaitForPage();
                pcmHomePage.NewQuotePromptClickCarInsurance();
                System.Threading.Thread.Sleep(3000);
                pcmHomePage.NewQuotePromptAnswerRadioButtons(null, null);
                System.Threading.Thread.Sleep(3000);
                Reporting.IsTrue(pcmHomePage.VerifyNewQuoteRadioButtonValidations(), "PPQ prelim question validations");
            }
        }

        private QuoteCar BuildTestDataMCValidContactMotorPPQB2CT2013()
        {
            var mainPH = new ContactBuilder(ShieldContacts.FetchAContactWithRACMembershipTier(
                                                                            membershipTiers: new MembershipTier[] { MembershipTier.Gold, MembershipTier.Silver, MembershipTier.Bronze }))
                                             .Build();
            // As we are getting an existing contact from MC/MCMock which may be obfuscated, but
            // ContactBuilder will override to the RACI test Mailosaur domain, we need to sync
            // that over-ridden email back in MC/MCMock.
            DataHelper.OverrideMemberEmailInMemberCentralByContactID(mainPH.Id, mainPH.PrivateEmail.Address);
            var vehicle = new MotorCarBuilder().InitialiseMotorCarWithRandomData(mainPH, true)
                                             .WithParkingAddress(mainPH.MailingAddress)
                                             .WithLimitedValidRandomVehicleUsage()  // As we may opt for TFT cover
                                             .WithRandomCover()
                                             .Build();

            // Logging out test data for easier debugging and auditing.
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, vehicle.ToString());

            return vehicle;
        }

        /// <summary>
        /// Could not leverage ActionsMotorVehicles methods as this has
        /// prefill validations sewn into each accordion.
        /// </summary>
        private string VerifyAndSubmitPage1InitialDetails(QuoteCar testVehicle)
        {
            Contact testMember = testVehicle.Drivers[0].Details;
            var insuredVehicle = string.Empty;

            using (var quotePage1 = new MotorQuote1Details(_browser))
            using (var quotePage2 = new MotorQuote2Quote(_browser))
            using (var spinner = new RACSpinner(_browser))
            {
                quotePage1.WaitForCarDetailsToDisplay();
                // Verify PPQ defaults.
                Reporting.IsTrue(quotePage1.AnnualKm == AnnualKms.UpTo10000, "Annual KM should default to 'Up To 10,000' for pre-populated quotes");
                Reporting.IsTrue(quotePage1.Usage == VehicleUsage.Private, "Vehicle usage should be Private");
                if (Config.Get().IsMotorRiskAddressEnabled()) // TODO: B2C-4561 Remove toggle and old Risk Suburb references as appropriate when removing toggle from B2C/PCM Functional code
                {
                    /* KNOWN ISSUE:
                     * RAI-316: PPQ combined with MRA creates a test data issue. As we are drawing from a large
                     * pool of existing members, we don't currently have any filtering on their mailing address,
                     * meaning that some of the members attempted may have a Lot number address or other unusable
                     * address without a GNAF. These type of addresses cause execution and validation issues for
                     * the automation.
                     */
                    if (testVehicle.Drivers[0].Details.MailingAddress.StreetOrPOBox.ToLower().Contains("po box"))
                    {
                        Reporting.IsTrue(string.Empty.Equals(quotePage1.RiskAddress), "Motor risk address should not be pre-filled as PPQ member had a PO Box mailing address.");
                    }
                    else
                    {
                        var expectedAddress = testVehicle.ParkingAddress.QASStreetAddress();
                        var foundAddress = quotePage1.RiskAddress.StripAddressDelimiters();
                        if (!expectedAddress.Equals(foundAddress, StringComparison.InvariantCultureIgnoreCase))
                            // Modifying street type, as non-QAS verified addresses can present unabbreviated street types.
                            expectedAddress = testVehicle.ParkingAddress.QASStreetAddress(true);
                        Reporting.AreEqual(expectedAddress, foundAddress, ignoreCase: true, "motor risk address");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(testMember.MailingAddress.StreetNumber))
                        Reporting.AreEqual($"{testMember.MailingAddress.Suburb.ToLower()} - {testMember.MailingAddress.PostCode}", 
                            quotePage1.ParkedSuburb.ToLower(), "Member Mailing address");
                }
                insuredVehicle = Regex.Replace(quotePage1.FillQuoteDetailsFirstAccordion(testVehicle, isPPQ: true), ",", "");
                System.Threading.Thread.Sleep(2000);

                Reporting.IsTrue(quotePage1.VerifyMainDriverPrefilledFields(testVehicle), "Prefilled fills were not correct, or not read-only");
                quotePage1.FillQuoteDetailsSecondAccordion(testVehicle, true, true);

                spinner.WaitForSpinnerToFinish(nextPage: quotePage2);
            }

            return insuredVehicle;
        }

        /// <summary>
        /// Could not leverage ActionsMotorVehicles methods as this has
        /// prefill validations sewn into each accordion.
        /// </summary>
        private void VerifyAndSubmitPage3PHDetails(QuoteCar testVehicle)
        {
            /***********************************************************
             * Complete Page 3 details
             ***********************************************************/
            using (var quotePage3 = new MotorQuote3Policy(_browser))
            using (var quoteSummary = new MotorQuote3Summary(_browser))
            using (var spinner = new RACSpinner(_browser))
            {
                quotePage3.WaitForPage();

                // Complete the additional car details
                Reporting.IsTrue(quotePage3.FillInAddedVehicleDetails(testVehicle), "successfully completed additional vehicle details");
                quotePage3.ClickCarDetailsContinueButton();

                Reporting.IsTrue(quotePage3.VerifyPPQPrePopulatedMainDriverDetails(testVehicle.Drivers[0]), "Main driver details were populated as expected.");
                Reporting.Log("Capturing image of Main Driver Details immediately before Continue button is selected.", _browser.Driver.TakeSnapshot());
                quotePage3.ClickDriverContinueButton(0);

                // B2C may interchange some drivers, so we use a temporary list
                // to search and eliminate from.
                var workingDriversList = new List<Driver>(testVehicle.Drivers);
                // Complete the additional driver details
                for (int i = 1; i < testVehicle.Drivers.Count; i++)
                {
                    quotePage3.WaitForDriverDetails(i);
                    quotePage3.FillInDriverDetails(i, workingDriversList, testVehicle.ParkingAddress, _browser);
                    quotePage3.ClickDriverContinueButton(i);
                }
                spinner.WaitForSpinnerToFinish(nextPage: quoteSummary);
            }
        }
    }
}