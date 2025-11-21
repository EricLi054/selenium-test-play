//TODO: AUNT-163 - This entire class file should be removed as Online Settlement is not handled in B2C anymore.
using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Claims
{
    public class ClaimHomePage3OnlineSettlement : BasePage
    {
        private enum B2COpenAccordion
        {
            [Description("Your claim has been fast-tracked")]
            SettlementOptions_FirstAccordion,
            [Description("Your claim settlement options (Cash Settlement Fact Sheet)")]
            SettlementOptions_SecondAccordion,
        }

        private class XPath
        {
            public const string Header        = "//span[@class='action-heading']";
            public const string ClaimNumber   = "id('quote-number')";
            public const string OpenAccordion = "//div[@class='accordion-heading opened']/div";

            public class ClaimSettlementFactSheet
            {
                public class Options
                {
                    public const string CashSettle       = "//div[@id='FenceCashSettlement_ClaimantSettlementDecisionChoice_63000002_Label']/span";
                    public const string Repair           = "//div[@id='FenceCashSettlement_ClaimantSettlementDecisionChoice_63000003_Label']/span";
                    public const string ContactMe        = "//div[@id='FenceCashSettlement_ClaimantSettlementDecisionChoice_63000004_Label']/span";
                    public const string TakeTimeToDecide = "//div[@id='FenceCashSettlement_ClaimantSettlementDecisionChoice_63000005_Label']/span";
                }
                public class Button
                {
                    public const string ConsentYes = "id('FenceCashSettlement_ViewFactSheetConsent_True_Label')";
                    public const string ConsentNo  = "id('FenceCashSettlement_ViewFactSheetConsent_False_Label')";
                }
            }

            public class Settlement
            {
                public class Text
                {
                    public class NonDividedFence
                    {
                        public const string FenceUnits  = "id('RepairCosts_MetresClaimed_Answer')";
                        public const string CostPerUnit = "id('RepairCosts_CostPerMetre_Answer')";
                    }
                    public class DividedFence
                    {
                        public const string DamagedFence = "//label[@for='RepairCosts_SubTotalBeforeAdjustments']";
                    }
                    
                    public const string SubTotal    = "id('RepairCosts_SubTotalBeforeExcess_Answer')";
                    public const string Excess      = "//div[contains(@id, 'Excess_Answer') and not(contains(@id,'SubTotal'))]";
                    public const string FinalAmount = "//div[contains(@id, 'RepairCosts_Total')]";
                }
                public class Button
                {
                    public const string Decline = "id('decline')";
                    public const string Accept  = "//button[contains(@class,'accordion-button-alt')]";
                }
            }

            public class BankDetails
            {
                public class Checkbox
                {
                    public const string DontHaveBankDetails = "//div[contains(@data-wrapper-for, '_BankAccountDetailsUnknown')]//div[@class='checkbox']";
                }
                public class Input
                {
                    public const string BsbNumber = "//input[contains(@id, 'BankAccountDetailsViewModel_BSB')]";
                    public const string AccountNumber = "//input[contains(@id, 'BankAccountDetailsViewModel_AccountNumber')]";
                    public const string AccountName = "//input[contains(@id, 'BankAccountDetailsViewModel_AccountName')]";
                }
            }

            public class Button
            {
                public const string Continue = "//button[starts-with(@id,'accordion_') and contains(@id,'_submit-action')]";
            }
        }

        #region Settable properties and controls

        public string OpenAccordion => GetInnerText(XPath.OpenAccordion);

        public string ClaimNumber => GetElement(XPath.ClaimNumber).Text;

        public decimal SettlementSubTotal => decimal.Parse(GetInnerText(XPath.Settlement.Text.SubTotal).StripMoneyNotations());
        public decimal Excess => decimal.Parse(GetInnerText(XPath.Settlement.Text.Excess).StripMoneyNotations().Replace("-", string.Empty));

        public decimal SettlementFinalAmount => decimal.Parse(GetInnerText(XPath.Settlement.Text.FinalAmount).StripMoneyNotations());
        #endregion Settable properties and controls

        public ClaimHomePage3OnlineSettlement(Browser browser) : base(browser)
        {
        }

        public override bool IsDisplayed()
        {
            bool rendered;
            try
            {
                GetElement(XPath.Header);
                GetElement(XPath.ClaimNumber);

                Reporting.LogPageChange("Home claim online settlement page");
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }

        public void ProcessOfferedSettlement(ClaimHome claimData)
        {
            Reporting.Log("Online settlement of fence claim", _driver.TakeSnapshot());

            //Checking it's a non dividing fence
            if (!claimData.FenceDamage.IsDividingFence)

                CompleteNonDividingFenceOnlineSettlementFlow(claimData);
            else
            {
                CompleteDividingFenceOnlineSettlementFlow(claimData);
            }
        }

        private void CompleteDividingFenceOnlineSettlementFlow(ClaimHome claimData)
        {
            VerifyFenceBreakdownSettlementCost(claimData.FenceDamage.IsDividingFence);

            switch (claimData.ExpectedOutcome)
            {
                case ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails:
                case ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails:
                    {
                        ClickControl(XPath.Settlement.Button.Accept);
                        EnterBankDetails(claimData);
                        Reporting.Log("Enter the bank details", _driver.TakeSnapshot());
                        ClickControl(XPath.Button.Continue);

                        break;
                    }
                default:
                    {
                        Reporting.Error($"{claimData.ExpectedOutcome} is not supported for online settlement");
                        break;
                    }
            }
        }

        private void CompleteNonDividingFenceOnlineSettlementFlow(ClaimHome claimData)
        {
            //Verify the new B2C CHaaFS Consent accordion is showing
            Reporting.AreEqual(OpenAccordion, B2COpenAccordion.SettlementOptions_FirstAccordion.GetDescription(), "Open accordion text");
            
            if (claimData.ExpectedOutcome != ExpectedClaimOutcome.OnlineSettlementNoConsent)
            {
                ProvideCSFSConsentVerifyFenceCost();
            }

            switch (claimData.ExpectedOutcome)
            {
                case ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails:
                case ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails:
                    {
                        ClickControl(XPath.ClaimSettlementFactSheet.Options.CashSettle);
                        ClickControl(XPath.Button.Continue);
                        EnterBankDetails(claimData);
                        break;
                    }
                case ExpectedClaimOutcome.OnlineSettlementRepairByRAC:
                    {
                        ClickControl(XPath.ClaimSettlementFactSheet.Options.Repair);
                        Reporting.Log("Provide the CSFS Consent", _driver.TakeSnapshot());
                        break;
                    }

                case ExpectedClaimOutcome.OnlineSettlementContactMe:
                    {
                        ClickControl(XPath.ClaimSettlementFactSheet.Options.ContactMe);
                        Reporting.Log("Settlement Options", _driver.TakeSnapshot());
                        break;
                    }
                case ExpectedClaimOutcome.OnlineSettlementTakeMoreTimeToDecide:
                    {
                        ClickControl(XPath.ClaimSettlementFactSheet.Options.TakeTimeToDecide);
                        Reporting.Log("Settlement Options", _driver.TakeSnapshot());
                        break;
                    }
                case ExpectedClaimOutcome.OnlineSettlementNoConsent:
                    {
                        ClickControl(XPath.ClaimSettlementFactSheet.Button.ConsentNo);
                        Reporting.Log("Provide the CSFS Consent", _driver.TakeSnapshot());
                        break;
                    }
                default:
                    {
                        Reporting.Error($"{claimData.ExpectedOutcome} is not supported for online settlement");
                        break;
                    }
            }
            ClickControl(XPath.Button.Continue);
        }

        private void EnterBankDetails(ClaimHome claimData)
        {
            _driver.WaitForElementToBeVisible(By.XPath(XPath.BankDetails.Input.BsbNumber), WaitTimes.T5SEC);

            if (claimData.AccountForSettlement == null)
            {
                ClickControl(XPath.BankDetails.Checkbox.DontHaveBankDetails);

            }
            else
            {
                WaitForTextFieldAndEnterText(XPath.BankDetails.Input.BsbNumber, claimData.AccountForSettlement.Bsb, false);
                WaitForTextFieldAndEnterText(XPath.BankDetails.Input.AccountNumber, claimData.AccountForSettlement.AccountNumber, false);
                WaitForTextFieldAndEnterText(XPath.BankDetails.Input.AccountName, claimData.AccountForSettlement.AccountName, false);
            }

            Reporting.Log("Responded to Payment details question", _driver.TakeSnapshot());

        }

        private void ProvideCSFSConsentVerifyFenceCost()
        {
            ClickControl(XPath.ClaimSettlementFactSheet.Button.ConsentYes);
            ClickControl(XPath.Button.Continue);

            _driver.WaitForElementToBeVisible(By.XPath(XPath.ClaimSettlementFactSheet.Options.CashSettle), WaitTimes.T5SEC);

            //Verify the new B2C CHaaFS Fact Sheet accordion is showing
            Reporting.AreEqual(OpenAccordion, B2COpenAccordion.SettlementOptions_SecondAccordion.GetDescription(), "Open accordion text");
            Reporting.Log("Online Cash Settlement Fact Sheet", _driver.TakeSnapshot());
            VerifyFenceBreakdownSettlementCost(isFenceDividing: false);
        }

        /// <summary>
        /// Returns the number of panels/metres of fence being claimed.
        /// 
        /// The table with the settlement details uses a different format
        /// based on whether it involves a fence shared with a neighbour.
        /// Ref: B2C-4412
        /// </summary>
        private int GetUnitsOfFenceClaimed(bool isDividingFence)
        {
            int fenceUnits = 0;
            if (isDividingFence)
            {
                Regex fenceUnitsRegex = new Regex(FixedTextRegex.CLAIM_FENCE_SETTLEMENT_LENGTH);
                var fenceUnitsText = GetInnerText(XPath.Settlement.Text.DividedFence.DamagedFence);
                Match match = fenceUnitsRegex.Match(fenceUnitsText);
                Reporting.IsTrue(match.Success, $"Fence online settlement shown with claimed fence amount. Received: {fenceUnitsText}");
                fenceUnits = int.Parse(match.Groups[1].Value);
            }
            else
            {
                fenceUnits = int.Parse(GetInnerText(XPath.Settlement.Text.NonDividedFence.FenceUnits));
            }

            return fenceUnits;
        }

        /// <summary>
        /// Cost value used by RAC per panel/metre for this claim.
        /// 
        /// The table with the settlement details uses a different format
        /// based on whether it involves a fence shared with a neighbour.
        /// Ref: B2C-4412
        /// </summary>
        private decimal GetCostPerUnit(bool isDividingFence)
        {
            decimal unitRate = 0;
            if (isDividingFence)
            {

                Regex fenceRateRegex = new Regex(FixedTextRegex.CLAIM_FENCE_SETTLEMENT_COVER_PER_UNIT);
                var fenceRateText = GetInnerText(XPath.Settlement.Text.DividedFence.DamagedFence);
                Match match = fenceRateRegex.Match(fenceRateText);
                Reporting.IsTrue(match.Success, $"Fence online settlement shown with fence cover rate. Received: ${fenceRateText}");

                unitRate = decimal.Parse(match.Groups[1].Value);
            }
            else
            {
                unitRate = decimal.Parse(GetInnerText(XPath.Settlement.Text.NonDividedFence.CostPerUnit).StripMoneyNotations());
            }
            return unitRate;
        }

        private void VerifyFenceBreakdownSettlementCost(bool isFenceDividing)
        {
            var fenceBreakdownCost = DataHelper.GetFenceSettlementBreakdownCost(ClaimNumber);

            Reporting.AreEqual(((decimal)fenceBreakdownCost.NumberOfMetresClaimed), GetUnitsOfFenceClaimed(isFenceDividing), "Fence Number of meter from Shield API");
            Reporting.AreEqual(((decimal)fenceBreakdownCost.CostPerMetre), GetCostPerUnit(isFenceDividing), "Fence Cost per meter from Shield API");
            Reporting.AreEqual(((decimal)fenceBreakdownCost.SubTotalBeforeExcess), SettlementSubTotal, "Fence Settlement Sub Total amount from Shield API");
            Reporting.AreEqual(((decimal)fenceBreakdownCost.CurrentExcess), Excess, "Building Excess from Shield API");
            Reporting.AreEqual(((decimal)fenceBreakdownCost.TotalRepairCost), SettlementFinalAmount, "Fence Total repair cost from Shield API");
        }
    }
}