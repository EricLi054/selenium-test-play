using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class Confirmation : SparkBasePage
    {
        #region CONSTANTS
        private const string SETTLEMENT_MESSAGE = "Your claim has been settled, ";
        private const string THANKYOU_FOR_SUBMITTING = "Thanks for submitting your claim, ";
        private const string CLAIM_RECEIVED_MESSAGE = "Claim received - thanks, ";
        private const string DECLINE_OR_INELIGIBLE_FOR_ONLINE_CLAIM_RECEIVED_MESSAGE = "Claim received - over to you, ";
        private const string SORRY_EXCESS_HIGHER_THAN_REPAIR = "Sorry, you can't claim";

        private const string CONTACT_YOU_TEXT = "We'll contact you within one business day.";
        private const string CONFIRMATION_EMAIL_TEXT = "You'll receive a confirmation email shortly.";
        private const string INVOICE_EMAIL_TEXT = "You'll receive an email shortly.";
        private const string WOODBRICK_SHARED_EMAIL_TEXT = "We'll send you an email within one business day.";
        private const string WOODBRICK_NONSHARED_EMAIL_TEXT = "We'll send you an email shortly.";
        private const string TAKETIME_EMAIL_TEXT = "We'll send you an email shortly.";
        private const string NEXT_STEP_EMAIL_TEXT = "We'll send an email shortly with your next steps.";
        private const string AGENDA_LODGED_DONE_EMAIL_TEXT = "We'll send an email shortly with the next steps.";
        private const string SEND_EMAIL_TEXT_INELIGIBLE = "We'll send you an email within one business day.";
        private const string REPAIR_COST_LESS_EXCESS_TEXT = @"Your excess of ${0} is more than your estimated repairs of ${1}, so we'll withdraw the claim.We'll confirm this by email within one business day. If you think there's been a mistake, we'll provide next steps in the email.";

        private const string CLAIM_NUMBER_TEXT = "Your claim number is ";

        private const string SETTLEMENT_AMOUNT_TEXT = "Your settlement $";
        private const string TO_ACCOUNT_TEXT = "To account: *** ";
        private const string PROCESSING_TIME_TEXT = "Processing time: up to seven business days";
        private const string EFT_EMAIL_LINK_TEXT = "Simply follow the link in the email to enter your bank details and get your cash settlement.";
        private const string BANK_DETAILS_RECEIVED_TEXT = "Once we've received your bank details, it may take up to seven business days to process.";

        private const string NEXT_STEPS_REPAIR_QUOTE_TEXT = "Your next steps\r\nWe'll leave you to get a quote for your fence. Then you can send it to us.\r\nOnce we've received your quote, we'll let you know the next steps within ten business days.";
        private const string NEXT_STEPS_INVOICE_READY_TEXT = "Your next steps\r\nIf you haven't uploaded your invoice, you'll need to do so. The email will explain how.\r\nIf you have, we'll contact you within ten business days to let you know the next steps.";
        private const string NEXT_STEPS_QUOTE_READY_TEXT = "Your next steps\r\nIf you haven't uploaded your quote, you'll need to do so. The email will explain how.\r\nIf you have, we'll contact you within ten business days to let you know the next steps.";
        private const string NEXT_STEPS_SETTLED_ONLINE = "Your next steps\r\nAfter you receive the cash, you can organise your fence repairs.\r\nIf the cash amount doesn't cover your costs, please send us your quote. We'll review the amount and discuss your options.";
        private const string GET_YOUR_TEMP_FENCE_TEXT = "Your temporary fence\r\nOne of our allocated builders will call you within one to two business days to organise your temporary fence.";
        private const string GET_BRICK_WOOD_TEMP_FENCE_TEXT = "Your temporary fence\r\nThe builder will also organise your temporary fence.";
        private const string NEXT_STEPS_BRICK_WOODEN_ASSESS_TEXT = "Your next steps\r\nOne of our allocated builders will contact you within two business days to assess your fence.";
        private const string BRICK_WOODEN_OWN_REPAIRS_TEXT = "\r\nYou'll still need to organise your own fence repairs after this.";
        private const string SERIOUS_STORM_NOTIFICATION_TEXT = "When there's been a serious storm, the timeframes can be a bit longer than indicated";
        #endregion

        #region XPATHS
        private class XPath
        {
            public static readonly string Header = "id('header')";
            public static readonly string ConfirmationEmail = "id('subHeader')";
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";
            public static readonly string SeriousStormNotification = "id('notification-card-title')";
            public class Button
            {
                public static readonly string GoToHomePage = "id('racHomePageLinkButton')";
            }
            public class Settlement
            {
                public static readonly string Amount = "//div[@data-testid='your-settlement-container']/div/div/span/h3";
                public static readonly string Paragraph1 = "//div[@data-testid='your-settlement-paragraph-0']/p";
                public static readonly string Paragraph2 = "//div[@data-testid='your-settlement-paragraph-1']/p";
            }
            public class Text
            {
                public static readonly string YourNextSteps = "//div[@data-testid='next-steps-container']";
                public static readonly string TemporaryFixed = "//div[@data-testid='temporary-fence-container']";
            }
        }
        #endregion

        #region Settable properties and controls

        private string HeaderMessage => GetInnerText(XPath.Header);
        private string ConfirmationEmailText => GetInnerText(XPath.ConfirmationEmail);
        private string ClaimNumberText => GetInnerText(XPath.ClaimNumber);
        private string SettlementText => GetInnerText(XPath.Settlement.Amount);
        private string SettlementParagraphOneText => GetInnerText(XPath.Settlement.Paragraph1);
        private string SettlementParagraphTwoText => GetInnerText(XPath.Settlement.Paragraph2);
        private string TemporaryFenceText => GetInnerText(XPath.Text.TemporaryFixed);
        private string YourNextStepTexts => GetInnerText(XPath.Text.YourNextSteps);
        #endregion

        public Confirmation(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.ClaimNumber);
                GetElement(XPath.Button.GoToHomePage);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Spark Fence Claim Page 7 - Confirmation");
            Reporting.Log("Confirmation Page :", _browser.Driver.TakeSnapshot());
            return true;
        }

        public void VerifyConfimationMessage(ClaimHome claim)
        {
            VerifyHeader(claim);

            if (claim.DamagedCovers == AffectedCovers.FenceOnly)
            {
                switch (claim.ExpectedOutcome)
                {
                    case ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails:
                    case ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails:
                        VerifySettlementDetails(claim);
                        VerifyYourNextStepsText(claim);
                        break;
                    case ExpectedClaimOutcome.GetRepairQuoteFirst:
                    case ExpectedClaimOutcome.AlreadyHaveRepairQuote:
                    case ExpectedClaimOutcome.RepairsCompleted:
                    case ExpectedClaimOutcome.NotEligibleForOnlineSettlement 
                            when !claim.IsRepairCostLessThanExcess 
                            && claim.EligibilityForOnlineSettlement != SettleFenceOnline.IneligibleCannotMeasureFence:
                        VerifyYourNextStepsText(claim);
                        break;
                    case ExpectedClaimOutcome.ClaimLodged:
                        break;
                    default:
                        break;
                }
            }
            if (claim.FenceDamage != null && 
                !claim.FenceDamage.IsAreaSafe &&
                (claim.DamagedCovers == AffectedCovers.FenceOnly ||
                claim.DamagedCovers == AffectedCovers.BuildingOnly ||
                claim.DamagedCovers == AffectedCovers.BuildingAndFence ||
                claim.DamagedCovers == AffectedCovers.ContentsAndFence ||
                claim.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence))
            {
                VerifyTemporaryFenceText(claim.FenceDamage.FenceMaterial);
            }

            VerifySevereStormNotification();
        }

        private void VerifyHeader(ClaimHome claim)
        {
            if (claim.DamagedCovers == AffectedCovers.FenceOnly)
            {
                switch (claim.ExpectedOutcome)
                {
                    case ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails:
                        Reporting.AreEqual($"{SETTLEMENT_MESSAGE}{claim.Claimant.FirstName}", HeaderMessage, ignoreCase: true,
                            "expected Header message for an online settlement with the actual value displayed on the Confirmation page");
                        Reporting.AreEqual(CONFIRMATION_EMAIL_TEXT, ConfirmationEmailText,
                            "expected 'Confirmation email message' for an online settlement with the actual value displayed on the Confirmation page");
                        break;
                    case ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails:
                        Reporting.AreEqual($"{THANKYOU_FOR_SUBMITTING}{claim.Claimant.FirstName}", HeaderMessage, ignoreCase: true,
                            "expected Header message when bank account details are not provided for an Online settlement with the actual value displayed on the Confirmation page");
                        Reporting.AreEqual(CONFIRMATION_EMAIL_TEXT, ConfirmationEmailText,
                            "expected 'EFT email' message when bank account details are not provided for an Online settlement with the actual value displayed on the Confirmation page");
                        break;
                    case ExpectedClaimOutcome.GetRepairQuoteFirst:
                        Reporting.AreEqual($"{DECLINE_OR_INELIGIBLE_FOR_ONLINE_CLAIM_RECEIVED_MESSAGE}{claim.Claimant.FirstName}!", HeaderMessage, ignoreCase: true,
                            "expected Header message when the member has opted to get a quote for repairs first with the actual value displayed on the Confirmation page");
                        Reporting.AreEqual(NEXT_STEP_EMAIL_TEXT, ConfirmationEmailText,
                            "expected 'Next steps email' message when the member has opted to get a quote for repairs first with the actual value displayed on the Confirmation page");
                        break;
                    case ExpectedClaimOutcome.OnlineSettlementTakeMoreTimeToDecide:
                        Reporting.AreEqual($"{CLAIM_RECEIVED_MESSAGE}{claim.Claimant.FirstName}", HeaderMessage, ignoreCase: true,
                                                    "expected Header message when the member was eligible for online settlement but has opted to take more time to decide with the actual value displayed on the Confirmation page");
                        Reporting.AreEqual(TAKETIME_EMAIL_TEXT, ConfirmationEmailText,
                                                    "expected 'send email' text when the member was eligible for online settlement but has opted to take more time to decide with the actual value displayed on the Confirmation page");
                        break;
                    case ExpectedClaimOutcome.NotEligibleForOnlineSettlement when claim.FenceDamage.FenceMaterial == FenceType.BrickWall && claim.FenceDamage.IsDividingFence:
                    case ExpectedClaimOutcome.NotEligibleForOnlineSettlement when claim.FenceDamage.FenceMaterial == FenceType.Wooden && claim.FenceDamage.IsDividingFence:
                        Reporting.AreEqual($"{CLAIM_RECEIVED_MESSAGE}{claim.Claimant.FirstName}", HeaderMessage, ignoreCase: true,
                            "expected Header message when ineligible due to construction material with the actual value displayed on the Confirmation page");
                        Reporting.AreEqual(WOODBRICK_SHARED_EMAIL_TEXT, ConfirmationEmailText,
                            "expected 'send email' text when ineligible due to construction material with the actual value displayed on the Confirmation page. If a different message is shown, then Shield may have failed to have assigned a service provider which is outside CHG's control");
                        break;
                    case ExpectedClaimOutcome.NotEligibleForOnlineSettlement when claim.FenceDamage.FenceMaterial == FenceType.BrickWall && !claim.FenceDamage.IsDividingFence:
                    case ExpectedClaimOutcome.NotEligibleForOnlineSettlement when claim.FenceDamage.FenceMaterial == FenceType.Wooden && !claim.FenceDamage.IsDividingFence:
                        Reporting.AreEqual($"{CLAIM_RECEIVED_MESSAGE}{claim.Claimant.FirstName}", HeaderMessage, ignoreCase: true,
                            "expected Header message when ineligible due to construction material with the actual value displayed on the Confirmation page");
                        Reporting.AreEqual(WOODBRICK_NONSHARED_EMAIL_TEXT, ConfirmationEmailText,
                            "expected 'send email' text when ineligible due to construction material with the actual value displayed on the Confirmation page");
                        break;
                    case ExpectedClaimOutcome.NotEligibleForOnlineSettlement when claim.IsRepairCostLessThanExcess:
                        Reporting.AreEqual(SORRY_EXCESS_HIGHER_THAN_REPAIR, HeaderMessage,
                            "expected Header message when repair cost estimate is less than the policy excess with the actual value displayed on the Confirmation page");
                        Reporting.AreEqual(String.Format(REPAIR_COST_LESS_EXCESS_TEXT, claim.FenceSettlementBreakdown.CurrentExcess.ToString("G29"), claim.FenceSettlementBreakdown.RepairCostBeforeExcess.ToString("G29")), ConfirmationEmailText.Replace("\r\n", ""),
                            "expected 'Repair cost' message when repair cost estimate is less than the policy excess with the actual value displayed on the Confirmation page");
                        break;
                    case ExpectedClaimOutcome.NotEligibleForOnlineSettlement when claim.FenceDamage.MetresPanelsDamaged == null:
                        Reporting.AreEqual($"{CLAIM_RECEIVED_MESSAGE}{claim.Claimant.FirstName}", HeaderMessage, ignoreCase: true,
                            "expected Header message when ineligible for online settlment as claimant could not provide a measurement for the damaged section of their fence");
                        Reporting.AreEqual($"{CONTACT_YOU_TEXT}", ConfirmationEmailText,
                            "expected 'We'll contact you' copy presented when ineligible for online settlment as claimant could not provide a measurement for the damaged section of their fence");
                        break;
                    case ExpectedClaimOutcome.RepairsCompleted:
                        Reporting.AreEqual($"{CLAIM_RECEIVED_MESSAGE}{claim.Claimant.FirstName}", HeaderMessage, ignoreCase: true,
                            "expected Header message when ineligible/repairs complete/already have a quote with the actual value displayed on the Confirmation page");
                        Reporting.AreEqual(INVOICE_EMAIL_TEXT, ConfirmationEmailText,
                            "expected 'send email' text when fence repairs are complete for a Fence Only claim with the actual value displayed on the Confirmation page");
                        break;
                    case ExpectedClaimOutcome.AlreadyHaveRepairQuote:
                        Reporting.AreEqual($"{CLAIM_RECEIVED_MESSAGE}{claim.Claimant.FirstName}", HeaderMessage, ignoreCase: true,
                            "expected Header message when the member already has a quote, with the actual value displayed on the Confirmation page");
                        Reporting.AreEqual(SEND_EMAIL_TEXT_INELIGIBLE, ConfirmationEmailText,
                            "expected 'send email' text when ineligible/repairs complete/already have a quote with the actual value displayed on the Confirmation page");
                        break;
                    case ExpectedClaimOutcome.NotEligibleForOnlineSettlement:
                        Reporting.AreEqual($"{DECLINE_OR_INELIGIBLE_FOR_ONLINE_CLAIM_RECEIVED_MESSAGE}{claim.Claimant.FirstName}", HeaderMessage, ignoreCase: true,
                            "expected Header message when ineligible/repairs complete/already have a quote with the actual value displayed on the Confirmation page");
                        Reporting.AreEqual(SEND_EMAIL_TEXT_INELIGIBLE, ConfirmationEmailText,
                            "expected 'send email' text when ineligible/repairs complete/already have a quote with the actual value displayed on the Confirmation page");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Refer to https://rac-wa.atlassian.net/wiki/spaces/ISP/pages/3323691614/Confirmation+page+card+variations+2.0
                var agendaStatus = ShieldHomeClaimDB.GetHomeStormClaimAgendaStatus(claim.ClaimNumber);

                Reporting.Log($"agendaStatus[AgendaStepNames.ClaimLodged.GetDescription() = {agendaStatus[AgendaStepNames.ClaimLodged.GetDescription()]}");

                if (agendaStatus[AgendaStepNames.ClaimLodged.GetDescription()] == "Done")
                {
                    Reporting.AreEqual($"{CLAIM_RECEIVED_MESSAGE}{claim.Claimant.FirstName}", HeaderMessage, ignoreCase: true,
                    "expected Header message when a claim is not Fence Only with the actual value displayed on the Confirmation page");
                    Reporting.AreEqual(AGENDA_LODGED_DONE_EMAIL_TEXT, ConfirmationEmailText,
                        "expected 'send email' text when a claim is not Fence Only and the status of Lodged step in Claim Agenda = 'Done' " +
                        "with the actual value displayed on the Confirmation page");
                }
                else
                {
                    Reporting.AreEqual($"{CLAIM_RECEIVED_MESSAGE}{claim.Claimant.FirstName}", HeaderMessage, ignoreCase: true,
                    "expected Header message when a claim is not Fence Only with the actual value displayed on the Confirmation page");
                    Reporting.AreEqual(CONTACT_YOU_TEXT, ConfirmationEmailText,
                        "expected 'send email' text when a claim is not Fence Only and the status of Lodged step in Claim Agenda = 'Current' " +
                        "with the actual value displayed on the Confirmation page");
                }
            }

            if (string.IsNullOrEmpty(claim.ClaimNumber))
            {
                claim.ClaimNumber = new String(ClaimNumberText.Where(x => Char.IsDigit(x)).ToArray());
            }
            Reporting.AreEqual($"{CLAIM_NUMBER_TEXT}{claim.ClaimNumber}", ClaimNumberText, 
                "expected Claim number text against the actual value displayed on the Confirmation page");
        }

        private void VerifySettlementDetails(ClaimHome claim)
        {
            switch (claim.ExpectedOutcome)
            {
                case ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails:
                    var lastThreeAccountNumber = claim.AccountForSettlement.AccountNumber.Substring(claim.AccountForSettlement.AccountNumber.Length - 3);
                    Reporting.AreEqual($"{TO_ACCOUNT_TEXT}{lastThreeAccountNumber}", SettlementParagraphOneText, "Account number on the Confirmation page");
                    Reporting.AreEqual(PROCESSING_TIME_TEXT, SettlementParagraphTwoText, "Processing time on the Confirmation page");                    
                    break;
                case ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails:
                    Reporting.AreEqual(EFT_EMAIL_LINK_TEXT, SettlementParagraphOneText, "EFT Email link text on the Confirmation page");
                    Reporting.AreEqual(BANK_DETAILS_RECEIVED_TEXT, SettlementParagraphTwoText, "Bank details received text on the Confirmation page");                    
                    break;
                default:
                    break;
            }            
            Reporting.AreEqual($"{SETTLEMENT_AMOUNT_TEXT}{Decimal.Round(claim.FenceSettlementBreakdown.TotalRepairCost)}", SettlementText, "Settlement amount on the Confirmation page");
        }

        private void VerifyYourNextStepsText(ClaimHome claim)
        {
            switch (claim.ExpectedOutcome)
            {
                case ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails:
                case ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails:
                    Reporting.AreEqual(NEXT_STEPS_SETTLED_ONLINE, YourNextStepTexts, 
                                               "the expected Next Steps copy (when online settlement is accepted) with the actual value displayed on the Confirmation page");
                    break;
                case ExpectedClaimOutcome.NotEligibleForOnlineSettlement when claim.FenceDamage.FenceMaterial == FenceType.BrickWall && claim.FenceDamage.IsDividingFence:
                case ExpectedClaimOutcome.NotEligibleForOnlineSettlement when claim.FenceDamage.FenceMaterial == FenceType.Wooden && claim.FenceDamage.IsDividingFence:
                    Reporting.AreEqual($"{NEXT_STEPS_BRICK_WOODEN_ASSESS_TEXT}{BRICK_WOODEN_OWN_REPAIRS_TEXT}", YourNextStepTexts, 
                        "the expected Next Steps copy (when online settlement is not available due to the construction material of the shared fence) with the actual value displayed on the Confirmation page");
                    break;
                case ExpectedClaimOutcome.NotEligibleForOnlineSettlement when claim.FenceDamage.FenceMaterial == FenceType.BrickWall && !claim.FenceDamage.IsDividingFence:
                case ExpectedClaimOutcome.NotEligibleForOnlineSettlement when claim.FenceDamage.FenceMaterial == FenceType.Wooden && !claim.FenceDamage.IsDividingFence:
                    Reporting.AreEqual($"{NEXT_STEPS_BRICK_WOODEN_ASSESS_TEXT}", YourNextStepTexts,
                        "the expected Next Steps copy (when online settlement is not available due to the construction material of the non-shared fence) with the actual value displayed on the Confirmation page");
                    break;
                case ExpectedClaimOutcome.GetRepairQuoteFirst:
                case ExpectedClaimOutcome.NotEligibleForOnlineSettlement:
                    Reporting.AreEqual(NEXT_STEPS_REPAIR_QUOTE_TEXT, YourNextStepTexts, 
                        "the expected Next Steps copy (when online settlement is declined or not available and the member needs to get a quote) with the actual value displayed on the Confirmation page");
                    break;
                case ExpectedClaimOutcome.RepairsCompleted:
                    Reporting.AreEqual(NEXT_STEPS_INVOICE_READY_TEXT, YourNextStepTexts, 
                        "the expected Next Steps copy (when repairs have already been completed) with the actual value displayed on the Confirmation page");
                    break;
                case ExpectedClaimOutcome.AlreadyHaveRepairQuote:
                    Reporting.AreEqual(NEXT_STEPS_QUOTE_READY_TEXT, YourNextStepTexts, 
                        "the expected Next Steps copy (when the member already has a quote for repairs) with the actual value displayed on the Confirmation page");
                    break;
                default:
                    break;
            }
        }


        private void VerifyTemporaryFenceText(FenceType fenceType)
        {
            switch(fenceType)
            {
                case FenceType.BrickWall:
                case FenceType.Wooden:
                    Reporting.AreEqual(GET_BRICK_WOOD_TEMP_FENCE_TEXT, TemporaryFenceText, "Temporary Fence Fixed text on the Confirmation page");
                    break;
                default:
                    Reporting.AreEqual(GET_YOUR_TEMP_FENCE_TEXT, TemporaryFenceText, "Temporary Fence Fixed text on the Confirmation page");
                    break;

            }

            
        }

        private void VerifySevereStormNotification()
        {
            Reporting.AreEqual(SERIOUS_STORM_NOTIFICATION_TEXT, GetInnerText(XPath.SeriousStormNotification),
                "expected Serious Storm text with that displayed in the Notification Card on screen");
        }
    }
}
