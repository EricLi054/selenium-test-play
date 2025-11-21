using Rac.TestAutomation.Common;
using System;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.Endorsements;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.Endorsements;
using static Rac.TestAutomation.Common.Constants.Endorsements.Cancellations;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using System.Text.RegularExpressions;

namespace Tests.ActionsAndValidations.Endorsements
{
    public static class VerifyCancelPolicy
    {
        /// <summary>
        /// Asserts that the date in the Last day of cover field
        /// matches expectation.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="expectedDate">DateTime for the last day of the policy</param>
        public static void VerifyCancellationLastDayOfCoverIsCorrectDate(Browser browser, DateTime expectedDate)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            { 
                Reporting.Log("Check last day of cover date", browser.Driver.TakeSnapshot());
                Reporting.AreEqual(cancellationPage.LastDayOfCover.Date, expectedDate.Date, "cancellation date is correctly set");
            }
        }

        /// <summary>
        /// Assert that the date picker is disabled. 
        /// When a member says the policy does not meet their needs (also known as 28 days free),
        /// then we cancel to the start of the policy term.
        /// </summary>
        /// <param name="browser"></param>
        public static void VerifyDatePickerIsDisabledForUpdates(Browser browser)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            {
                Reporting.IsFalse(cancellationPage.IsDatePickerEnabled, "date picker is locked and disabled for input");
            }
        }

        /// <summary>
        /// Asserts that the correct feedback is displayed to the member
        /// when invalid information is supplied on the initial details page.
        /// </summary>
        /// <param name="browser"></param>
        public static void VerifyErrorMessagesOnCancellationDetailsPage(Browser browser)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            {
                Reporting.Log("Check error messages when information is not supplied", 
                    browser.Driver.TakeSnapshot());
                Reporting.IsTrue(cancellationPage.IsReasonValidationMessagePresent,
                    "error message displayed when cancellation reason is not given");
                Reporting.IsTrue(cancellationPage.IsEmailValidationMessagePresent,
                    "error message displayed when email address is invalid");
            }
        }

        /// <summary>
        /// On the policy cancellation page, confirm the correct details for a policy are shown.
        /// 
        /// Date for cancellation is generally today's date but can differ due to policy 
        /// particulars.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="vehiclePolicy"></param>
        /// <param name="expectedDate">Expected date for cancellation</param>
        public static void VerifyMotorPolicyDetailsPopulated(Browser browser, EndorseCar vehiclePolicy,
            DateTime expectedDate)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                Reporting.Log("Cancellation Details Page", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(cancellationPage.IsDisplayed(), "Cancellation Page has loaded with title and cancel policy button");
                Reporting.AreEqual($"{Cancellations.DetailsPage.Title}", cancellationPage.PageTitle, false);
                Reporting.AreEqual($"{Cancellations.DetailsPage.SubTitle} {vehiclePolicy.ActivePolicyHolder.FirstName}.",
                    cancellationPage.PageSubTitle, false);
                Reporting.AreEqual(expectedDate.Date, cancellationPage.LastDayOfCover.Date, "cancellation date is correctly set");

                // FUTURE Card Component details are correct eg Rego and car description
            }
        }

        /// <summary>
        /// Check the contents of the roadside assistance information card.
        /// Includes roadside assistance telephone number
        /// 
        /// NOTE: This is only valid for PCO where Mock is enabled!
        /// </summary>
        /// <param name="browser"></param>
        public static void VerifyRoadsideMessage(Browser browser)
        {
            // Current PCO tests only force RSA in mocked environments.
            if (!Rac.TestAutomation.Common.Config.Get().IsMCMockEnabled())
            { return; }

            using (var cancellationConfirmationPage = new CancellationConfirmationPage(browser))
            {
                // This error message is so detailed because it has occurred a few times now and can
                // be caused by systems outside of the experience of the RACI team.
                var roadsideErrorMessage = "additional information for roadside assistance is displayed. If the roadside component does " +
                                           "NOT display, and you are in a connected MC environment (i.e. not a Mock), then you may need" +
                                           "to liaise with the Member Central team to ensure that their systems are sending the RSA " +
                                           "information through. An easy way to check whether MC is the problem, is to repeat this test" +
                                           "in a MC Mock environment (ensuring that the member you are using in the test has a RSA set " +
                                           "in the Mock). If it passes with the Mock, then the issue is with Member Central and FinOps";
                Reporting.IsTrue(cancellationConfirmationPage.IsRoadsideAssistanceCardDisplayed, roadsideErrorMessage);
                Reporting.AreEqual(CancellationConfirmationPage.Constants.Roadside.Title, cancellationConfirmationPage.RoadsideAssistanceTitle,
                    false, "roadside assistance title is displayed.");
                Reporting.AreEqual(CancellationConfirmationPage.Constants.Roadside.ExplantoryText, cancellationConfirmationPage.RoadsideAssistanceFirstParagraph,
                    false, "message prompting member to evaulate Roadside Assistance coverage.");
                Reporting.AreEqual(CancellationConfirmationPage.Constants.Roadside.ActionText, cancellationConfirmationPage.RoadsideAssistanceSecondParagraph,
                    false, "prompt to call about roadside options exists.");
                Reporting.IsTrue(cancellationConfirmationPage.IsRoadsideAssistanceCallLinkPresent, "link to call roadside assistance exists.");
            }
        }

        /// <summary>
        /// On the policy cancellation page, confirm the correct details for a Home policy are shown.
        /// 
        /// Date for cancellation is generally today's date but can differ due to policy 
        /// particulars.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="homePolicy"></param>
        /// <param name="expectedDate">Expected date</param>
        public static void VerifyHomePolicyDetailsPopulated(Browser browser, EndorseHome homePolicy,
             DateTime expectedDate)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                Reporting.Log("Cancellation Details Page", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(cancellationPage.IsDisplayed(), "Cancellation Page has loaded with title and cancel policy button");
                Reporting.AreEqual($"{Cancellations.DetailsPage.Title}", cancellationPage.PageTitle, false);
                Reporting.AreEqual($"{Cancellations.DetailsPage.SubTitle} {homePolicy.ActivePolicyHolder.FirstName}.",
                    cancellationPage.PageSubTitle, ignoreCase: true);
                Reporting.AreEqual(expectedDate.Date, cancellationPage.LastDayOfCover.Date, "cancellation date is correctly set");

                // FUTURE Card Component details are correct eg Rego and car description
            }
        }

        /// <summary>
        /// Confirm correct details are displayed for a member wishing to 
        /// cancel their boat policy. Will include information about the default 
        /// last day of cover and policy information.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="boatPolicy"></param>
        /// <param name="expectedDate"></param>
        public static void VerifyCancelBoatPolicyPage(Browser browser, EndorseBoat boatPolicy,
            DateTime expectedDate)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                Reporting.Log("Cancellation Details Page", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(cancellationPage.IsDisplayed(), "Cancellation Page has loaded with title and cancel policy button");
                Reporting.AreEqual($"{Cancellations.DetailsPage.Title}", cancellationPage.PageTitle, false);
                Reporting.AreEqual($"{Cancellations.DetailsPage.SubTitle} {boatPolicy.ActivePolicyHolder.FirstName}.",
                    cancellationPage.PageSubTitle, false);
                Reporting.AreEqual(expectedDate.Date, cancellationPage.LastDayOfCover.Date, "cancellation date is correctly set");
            }

            VerifyBoatPolicyInformationDisplayed(browser, boatPolicy, false);
        }

        /// <summary>
        /// Verifies the cancellation modal is displayed and contains the necessary 
        /// information.  The modal seeks confirmation that the member wants to
        /// cancel their policy.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="policyType"></param>
        /// <param name="policyNumber"></param>
        public static void VerifyConfirmationModalContents(Browser browser, string policyType, string policyNumber, DateTime policyCancellationDate)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            {
                Reporting.IsTrue(cancellationPage.IsConfirmationModalDisplayed,
                    "confirmation modal is displayed");
                Reporting.Log("Cancellation modal", browser.Driver.TakeSnapshot());
                Reporting.AreEqual($"Cancelling your {policyType}",
                    cancellationPage.ConfirmationModalTitle, false);
                Reporting.IsTrue(cancellationPage.IsConfirmationModalBackButtonPresent,
                    "confirmation modal has back button");
                Reporting.IsTrue(cancellationPage.IsConfirmationModalCloseButtonPresent,
                    "confirmation modal has close button");
                Reporting.IsTrue(cancellationPage.IsConfirmationModalConfirmCancellationButtonPresent,
                    "confirmation modal has confirm cancellation button");
                Reporting.IsTrue(cancellationPage.ConfirmationModalAgreementText.Contains(policyNumber),
                    "confirmation modal refers to the policy number");
                Reporting.IsTrue(cancellationPage.ConfirmationModalSubTitle.Contains(policyCancellationDate.ToString(DataFormats.DATE_FULL_DATE_WITH_DAY)),
                    "confirmation modal sub title includes correct cancellation date in long format");
                Reporting.IsTrue(cancellationPage.ConfirmationModalAgreementText.Contains(policyCancellationDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH)),
                    "confirmation modal text includes correct cancellation date in short format");
            }
        }

        /// <summary>
        /// When a member selects financial hardship as their reason for cancelling,
        /// we need to ensure extra information on help for financial hardship is displayed.
        /// </summary>
        /// <param name="browser"></param>
        public static void ValidateExtraInformationFinancialHardship(Browser browser)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            {
                Reporting.Log("Check extra information display for financial hardship", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(cancellationPage.IsExtraInformationFinancialHardshipDisplayed, "Extra information is now displayed for financial hardship");
                Reporting.AreEqual(CancellationDetailsPage.Constants.AdditionalInformationTitle.FinancialHardship,
                    cancellationPage.FinancialHardshipExtraInfoTitle,
                    "Extra information for financial hardship has correct title");
                Reporting.AreEqual(CancellationDetailsPage.Constants.AdditionalInformationText.FinancialHardship,
                    cancellationPage.FinancialHardshipExtraInfoText,
                    "Extra information for financial hardship has correct content");
            }
        }

        /// <summary>
        /// Financial hardship help information is only to be displayed when it is the reason
        /// for cancellation.  For other cancellation reasons, financial hardship should not be displayed.
        /// </summary>
        /// <param name="browser"></param>
        public static void ValidateExtraInformationFinancialHardshipRemoved(Browser browser)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            {
                Reporting.Log("Check extra information display for financial hardship has been removed", browser.Driver.TakeSnapshot());
                Reporting.IsFalse(cancellationPage.IsExtraInformationDemolishedHouseDisplayed, "Extra information is NOT displayed for financial hardship");
            }
        }

        /// <summary>
        /// Extra information should be displayed when a member chooses house demolished
        /// as their reason for cancelling. Check the displaying of and the content of 
        /// this extra section.
        /// </summary>
        /// <param name="browser"></param>
        public static void ValidateExtraInformationHouseDemolished(Browser browser)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            {
                Reporting.Log("Check extra information displayed for house demolished", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(cancellationPage.IsExtraInformationDemolishedHouseDisplayed, "Extra information is now displayed for house demolished");
                Reporting.AreEqual(CancellationDetailsPage.Constants.AdditionalInformationTitle.DemolishedHouse,
                    cancellationPage.HouseDemolishedExtraInfoTitle,
                    "Extra information for house demolished has correct title");
                Reporting.AreEqual(CancellationDetailsPage.Constants.AdditionalInformationText.DemolishedHouse,
                    cancellationPage.HouseDemolishedExtraInfoText,
                    "Extra information for house demolished has correct content");
            }
        }

        /// <summary>
        /// Extra information should be displayed when a member chooses House sold
        /// as their reason for cancelling. Check the displaying of and the content of 
        /// this extra section.
        /// </summary>
        /// <param name="browser"></param>
        public static void ValidateExtraInformationHouseSold(Browser browser)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            {
                Reporting.Log("Check extra information displayed for house sold", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(cancellationPage.IsExtraInformationSoldHouseDisplayed, "Extra information is now displayed for house sold");
                Reporting.AreEqual(CancellationDetailsPage.Constants.AdditionalInformationTitle.SoldHouse, 
                    cancellationPage.HouseSoldExtraInfoTitle, "Extra information for house sold has correct title");
                Reporting.AreEqual(CancellationDetailsPage.Constants.AdditionalInformationText.SoldHouse, cancellationPage.HouseSoldExtraInfoText, "Extra information for house sold has correct content");
            }
        }

        public static void ValidateRefundProcessingMessage(Browser browser)
        {
            using (var cancellationPage = new CancellationDetailsPage(browser))
            {
                Reporting.Log("Check message about processing refunds.", browser.Driver.TakeSnapshot());
                Reporting.AreEqual($"{CancellationDetailsPage.Constants.RefundProcessing.PartOne} {CancellationDetailsPage.Constants.RefundProcessing.LinkText}.",
                    cancellationPage.GetProcessingInformationFirstParagraph, "first paragraph about processing refunds displays correctly.");
                Reporting.IsTrue(cancellationPage.IsProcessingInformationSecondParagraphPresent, "second paragraph about processing refunds displays correctly"); 
                Reporting.IsTrue(cancellationPage.IsProcessingInformationLinkPresent, "link to insurance policy document exists");
            }
        }

        /// <summary>
        /// Confirmation Page is the final page shown to a member.
        /// Make sure the necessary content is displayed.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="policyNumber">Policy number is used for element ids</param>
        public static void VerifyPolicyCancellationConfirmationPage(Browser browser, string policyNumber, string memberFirstName)
        {
            using (var confirmationPage = new CancellationConfirmationPage(browser))
            using (var policyCard = new PolicyInformationComponent(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                Reporting.Log("Display confirmation of policy cancellation", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(confirmationPage.IsDisplayed(policyNumber), "cancellation confirmation page has loaded correctly");                
                Reporting.IsTrue(confirmationPage.PageTitle().Contains(Cancellations.ConfirmationPage.LeavingMessage),
                    "confirmation page contains correct page title");
                Reporting.IsTrue(confirmationPage.PageSubTitle().Contains($"{Cancellations.ConfirmationPage.ExitMessage.SorryToSeeYouGo}" +
                    $"{memberFirstName}. {Cancellations.ConfirmationPage.ExitMessage.ConfirmingEmail}"),
                    "confirmation page contains correct page sub title");
                Reporting.IsTrue(policyCard.IsDisplayed(policyNumber), "Policy card is present");
                Reporting.IsTrue(policyCard.IsCancelledRibbonDisplayed(policyNumber), "policy details has cancellation ribbon");
                Reporting.IsTrue(policyCard.PolicyDetailsCardProperties(policyNumber).Contains($"Policy number: {policyNumber}"),
                    "confirmation page displays correct policy number in the policy details card");
            }
        }

        /// <summary>
        /// Confirmation Page is the final page shown to a member.
        /// Make sure the necessary content is displayed with a tailored message
        /// for a bad claims experience.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="policyNumber"></param>
        /// <param name="memberFirstName"></param>
        public static void VerifyBadClaimExperiencePolicyCancellationConfirmationPage(Browser browser, string policyNumber, string memberFirstName)
        {
            using (var confirmationPage = new CancellationConfirmationPage(browser))
            using (var policyCard = new PolicyInformationComponent(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(waitTimeSeconds: WaitTimes.T150SEC);
                Reporting.Log("Display confirmation of policy cancellation", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(confirmationPage.IsDisplayed(policyNumber), "cancellation confirmation page has loaded correctly");
                Reporting.IsTrue(confirmationPage.PageTitle().Contains(Cancellations.ConfirmationPage.LeavingMessage),
                    "confirmation page contains correct page title");
                Reporting.IsTrue(confirmationPage.PageSubTitle().Contains($"{Cancellations.ConfirmationPage.ExitMessage.BadClaimsExperience}" +
                    $"{memberFirstName}. {Cancellations.ConfirmationPage.ExitMessage.ConfirmingEmail}"),
                    "confirmation page contains correct page sub title");
            }
        }

        /// <summary>
        /// Provides an additional check for home policy to make
        /// sure confirmation includes the asset address.
        /// Makes use of existing check
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="policyNumber"></param>
        /// <param name="address"></param>
        public static void VerifyHomePolicyCancellationConfirmationPage(Browser browser, string policyNumber, string memberFirstName, Address address)
        {
            VerifyPolicyCancellationConfirmationPage(browser, policyNumber, memberFirstName);

            using (var confirmationPage = new CancellationConfirmationPage(browser))
            using (var policyCard = new PolicyInformationComponent(browser))
            {
                Reporting.IsTrue(policyCard.PolicyDetailsCardProperties(policyNumber).Contains($"{address.StreetNumber} {address.StreetOrPOBox}"),
                    "confirmation page include the street name and number");
            }
        }

        /// <summary>
        /// Make sure the correct information is displayed for a boat policy.
        /// Will check for the appropriateness of displaying the cancelled ribbon
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testData"></param>
        /// <param name="cancelRibbonExpected">Boolean for the Cancelled ribbon to be displayed</param>
        public static void VerifyBoatPolicyInformationDisplayed(Browser browser, EndorseBoat testData, bool cancelRibbonExpected)
        {
            using (var policyCard = new PolicyInformationComponent(browser))
            {
                Reporting.IsTrue(policyCard.IsDisplayed(testData.PolicyNumber), "Policy card is present");
                Reporting.IsTrue(policyCard.PolicyDetailsCardTitle(testData.PolicyNumber).Contains($"Boat insurance"), "title includes boat insurance");
                Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 0, "type").Contains($"Type: {testData.Type.GetDescription()}"),
                    "policy card displays correct type of boat");
                Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber,1,"policy-number").Contains($"Policy number: {testData.PolicyNumber}"),
                    "confirmation page displays correct policy number in the policy details card");
                Reporting.AreEqual(cancelRibbonExpected, policyCard.IsCancelledRibbonDisplayed(testData.PolicyNumber), "checking display of Cancelled Ribbon");
            }
        }

        /// <summary>
        /// Once a policy is cancelled, confirm that the database has been updated as 
        /// intended.
        /// </summary>
        /// <param name="dbQueryResults">Results from database query</param>
        /// <param name="effectiveDate">Expected date from which endorsement (cancellation) is effective</param>
        /// <param name="reasonCode">External code for cancellation reading</param>
        /// <param name="updatedEmailAddress">Email address supplied with with cancellation</param>
        /// <param name="checkTime">Whether to include the time component when checking</param>
        public static void VerifyShieldUpdated(PolicyEndorsementCancellation dbQueryResults, DateTime effectiveDate, string reasonCode, string updatedEmailAddress, bool checkTime = false )
        {
            Reporting.AreEqual((int)ShieldPolicyStatusId.Cancelled, dbQueryResults.PolicyStatusId, "policy registered as cancelled in Shield");
            Reporting.AreEqual((int)ShieldCancellationInitiator.PolicyHolder, dbQueryResults.InitiatorId, "cancellation recorded as initiated by policy holder");
            Reporting.AreEqual(reasonCode, dbQueryResults.PolicyEndorsementExternalCode, "cancellation reason recorded correctly in Shield");
            Reporting.IsTrue(dbQueryResults.PrintEventGenerated, "cancellation email print event generated");
            Reporting.AreEqual(effectiveDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
                dbQueryResults.CancellationEffectiveDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), "cancellation effective date matches member intentions");
            if(checkTime)
            {
                Reporting.AreEqual(effectiveDate.ToString(DataFormats.TIME_FORMAT_24HR_WITH_SECONDS),
                    dbQueryResults.CancellationEffectiveDate.ToString(DataFormats.TIME_FORMAT_24HR_WITH_SECONDS), "cancellation effective time is correct");
            }
            // FUTURE work.  For current day policy cancellation, work out acceptable margins for time differences
            Reporting.AreEqual(updatedEmailAddress, dbQueryResults.Email, true, "email address update correctly");
        }

        /// <summary>
        /// Checking that the correct information is shown to a member when they are entitled to a refund.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="refundAmount">The refund amount</param>
        /// <param name="refundDestinationType"></param>
        public static void VerifyRefundInformation(Browser browser, decimal refundAmount, RefundDestinationType refundDestinationType)
        {
            refundAmount = refundAmount < 0 ? -1 * refundAmount : refundAmount;
            using (var confirmationPage = new CancellationConfirmationPage(browser))
            {
                Reporting.AreEqual(CancellationConfirmationPage.Constants.RefundInformation.Heading, confirmationPage.RefundHeading(), "refund heading is displayed");
                Reporting.IsTrue(confirmationPage.IsMoneyIconDisplayed, "money note icon is displayed");
                Reporting.AreEqual(CancellationConfirmationPage.Constants.RefundInformation.AmountLabel,
                    confirmationPage.RefundLabel(), "refund amount label is displayed");

                // When whole dollar amount, then no cents displayed
                if(refundAmount % 1 == 0)
                {
                    Reporting.AreEqual(String.Format("${0:0}", refundAmount), confirmationPage.RefundAmount(), "refund amount is displayed correctly (whole dollar only)");
                } else
                {
                    Reporting.AreEqual(String.Format("${0:0.00}", refundAmount), confirmationPage.RefundAmount(), "refund amount is displayed correctly");
                }

                // Display for refund definition, card / account number will have partial masking 
                var accountDestinationRegex = refundDestinationType == RefundDestinationType.BankAccount ?
                              new Regex($"^{CancellationConfirmationPage.Constants.RefundInformation.DestinationLabelAccount} {FixedTextRegex.BANK_ACCOUNT_MASKING}") :
                              new Regex($"^{CancellationConfirmationPage.Constants.RefundInformation.DestinationLabelCard} {FixedTextRegex.CREDIT_CARD_MASKING}");

                var accountDestinationText = confirmationPage.RefundDestination();
                var accountDestinationMatch = accountDestinationRegex.Match(accountDestinationText);
                Reporting.IsTrue(accountDestinationMatch.Success, "refund account details displayed with partial masking and a label");

                Reporting.AreEqual(CancellationConfirmationPage.Constants.RefundInformation.ProcessingTime, confirmationPage.RefundProcessingTime(), false);
            }
        }
    }
}