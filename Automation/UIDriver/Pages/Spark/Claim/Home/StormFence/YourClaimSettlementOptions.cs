using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Linq;
using System.Threading;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class YourClaimSettlementOptions : SparkBasePage
    {
        #region XPATHS
        public class XPath
        {
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";
            public class OnlineSettlementBreakdown
            {
                public static readonly string CashSettlementAmount      = "id('cash-settlement-total')";
                public static readonly string DamagedFenceMeter         = "id('number-of-metres-claimed-cell')";
                public static readonly string EstimatedRepairCost       = "id('estimated-repair-cost-cell')";
                public static readonly string AsbestosContribution      = "id('asbestos-inspection-fee-cell')";
                public static readonly string SharedFenceAdjustment     = "id('shared-fence-adjustment-50-cell')";
                public static readonly string PaintCost                 = "id('paint-cost-cell')";
                public static readonly string ExcessPayment             = "id('less-your-excess-cell')";
                public static readonly string TotalPayment              = "id('total-cell')";
            }
            public class SettlementOptionButton
            {
                public static readonly string RepairByRac               = "id('settlementOption-1-label')";
                public static readonly string SeeAllOptions             = "id('settlementOption-2-label')";
                public static readonly string Next                      = "id('submit-button')";
            }
            public class Button
            {
                public static readonly string LinkSeeCashSettlementBreakdown    = "//div[@data-testid='form-section-accordion-title']/p";
                public static readonly string RepairQuote                       = "//button[@data-testid='repairQuoteButton']";
                public static readonly string ChooseRacRepairer                 = "//button[@data-testid='chooseRacRepairer']";
                public static readonly string ConfirmRacRepair                  = "//button[@data-testid='rac-repairer-confirmation-dialog-confirm-button']";
                public static readonly string CancelRacRepair                   = "//button[@data-testid='rac-repairer-confirmation-dialog-cancel-button']";
                public static readonly string TakeTimeToDecide                  = "//button[@data-testid='takeTimeToDecide']";
                public static readonly string CallFromRac                       = "//button[@data-testid='callFromRac']";
                public static readonly string AcceptSettlement                  = "//button[@data-testid='acceptSettlementButton']";
                public static readonly string ConfirmDialog                     = "//button[@data-testid='cash-settlement-confirmation-dialog-confirm-button']";
                public static readonly string CancelDialog                      = "//button[@data-testid='cash-settlement-confirmation-dialog-cancel-button']";
            }
        }

        #endregion

        #region Settable properties and controls

        public string ClaimNumber
        {
            get
            {
                var claimNumber = new String(GetElement(XPath.ClaimNumber).Text.
                    Where(x => Char.IsDigit(x)).ToArray());
                return claimNumber;
            }
        }

        public decimal CashSettlementAmount
        {
            get
            {
                var cashSettlementAmount = new String(GetElement(XPath.OnlineSettlementBreakdown.CashSettlementAmount).Text.
                    Where(x => x == '.' || Char.IsDigit(x)).ToArray());
                return Convert.ToDecimal(cashSettlementAmount);
            }
        }

        public decimal DamageFenceInMeters
        {
            get
            {
                var damagedMeter = new String(GetElement(XPath.OnlineSettlementBreakdown.DamagedFenceMeter).Text.
                    Where(x => x == '.' || Char.IsDigit(x)).ToArray());
                return Convert.ToDecimal(damagedMeter);
            }
        }

        public decimal EstimatedRepairCost
        {
            get
            {
                var estimatedRepairCost = new String(GetElement(XPath.OnlineSettlementBreakdown.EstimatedRepairCost).Text.
                    Where(x => x == '.' || Char.IsDigit(x)).ToArray());
                return Convert.ToDecimal(estimatedRepairCost);
            }
        }

        public decimal AsbestosContribution
        {
            get
            {
                var estimatedRepairCost = new String(GetElement(XPath.OnlineSettlementBreakdown.AsbestosContribution).Text.
                    Where(x => x == '.' || Char.IsDigit(x)).ToArray());
                return Convert.ToDecimal(estimatedRepairCost);
            }
        }

        public decimal SharedFenceAdjustment
        {
            get
            {
                var sharedFenceAdjustment = new String(GetElement(XPath.OnlineSettlementBreakdown.SharedFenceAdjustment).Text.
                    Where(x => x == '.' || x == '-' || Char.IsDigit(x)).ToArray());
                return Convert.ToDecimal(sharedFenceAdjustment);
            }
        }

        public decimal PaintCost
        {
            get
            {
                var paintCost = new String(GetElement(XPath.OnlineSettlementBreakdown.PaintCost).Text.
                    Where(x => x == '.' || Char.IsDigit(x)).ToArray());
                return Convert.ToDecimal(paintCost);
            }
        }

        public decimal ExcessPayment
        {
            get
            {
                var excessPayment = new String(GetElement(XPath.OnlineSettlementBreakdown.ExcessPayment).Text.
                    Where(x => x == '.' || Char.IsDigit(x)).ToArray());
                return Convert.ToDecimal(excessPayment);
            }
        }
        public decimal TotalPayment
        {
            get
            {
                var totalPayment = new String(GetElement(XPath.OnlineSettlementBreakdown.TotalPayment).Text.
                    Where(x => x == '.' || Char.IsDigit(x)).ToArray());
                return Convert.ToDecimal(totalPayment);
            }
        }
        #endregion

        public YourClaimSettlementOptions(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.ClaimNumber);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Spark Fence Claim - Your Claim Settlement Options");

            return true;
        }

        public void VerifySettlementBreakdownCostAndSelectAnOption(ClaimHome claim)
        {
            if(claim.FenceDamage.IsDividingFence)
            {
                VerifyFenceBreakdownSettlementCost(claim);
                Reporting.Log("Your Claim Settlement Options Page", _driver.TakeSnapshot());
                if (claim.ExpectedOutcome == ExpectedClaimOutcome.GetRepairQuoteFirst)
                {
                    ClickRepairQuoteButton();
                }
                else
                {
                    ClickAcceptSettlement();
                }
            }
            else //Non-dividing fence
            {
                VerifyFenceBreakdownSettlementCost(claim);
                Reporting.Log("Your Claim Settlement Options Page", _driver.TakeSnapshot());
                switch (claim.ExpectedOutcome)
                {
                    case ExpectedClaimOutcome.OnlineSettlementAcceptWithBankDetails:
                    case ExpectedClaimOutcome.OnlineSettlementAcceptWithExistingBankDetails:
                    case ExpectedClaimOutcome.OnlineSettlementAcceptWithOutBankDetails:
                        ClickAcceptSettlement();
                        break;
                    case ExpectedClaimOutcome.OnlineSettlementTakeMoreTimeToDecide:
                        ClickTakeTimeToDecide();
                        break;
                    case ExpectedClaimOutcome.OnlineSettlementContactMe:
                        ClickCallFromRACButton();
                        break;
                    case ExpectedClaimOutcome.OnlineSettlementRepairByRAC:
                        SelectRepairByRACFromCashSettlementFactSheet();
                        break;
                    default:
                        throw new NotImplementedException($"Claim Outcome {claim.ExpectedOutcome} has not been accounted in the Claim Settlment Options Page switch cases, investigation required.");
                }
            }
        }

        /// <summary>
        /// If the expected outcome for a home non-shared fence claim is to have repairs completed by RAC,
        /// we'll select that here and move on to the Confirmation page.
        /// 
        /// If not we'll select 'I'd like to see all my options' and go on to the Cash Settlement Fact Sheet.
        /// </summary>
        public void VerifyNonDividingSettlementOption(ClaimHome claim)
        {
            Reporting.Log("Your Claim Settlement Options Page", _driver.TakeSnapshot());
            if (claim.ExpectedOutcome == ExpectedClaimOutcome.OnlineSettlementRepairByRAC)
            {
                ClickRepairByRacButtonOnSettlementOptionsAndConfirm();
            }
            else
            {
                ClickSeeMyOptionsButtonOnSettlementOptions();
            }
        }

        public void SelectRepairByRACFromCashSettlementFactSheet()
        {
            ScrollElementIntoView(XPath.Button.ChooseRacRepairer);
            Reporting.Log($"Cash Settlement Fact Sheet - about to select 'Choose RAC Repairer' button", _driver.TakeSnapshot());
            ClickControl(XPath.Button.ChooseRacRepairer);
            
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Button.ConfirmRacRepair), WaitTimes.T5SEC);
            ClickControl(XPath.Button.ConfirmRacRepair);
        }

        private void ClickAcceptSettlement()
        {
            ScrollElementIntoView(XPath.Button.AcceptSettlement);
            ClickControl(XPath.Button.AcceptSettlement);
            Reporting.Log($"Accepting online settlement", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.ConfirmDialog);
        }

        private void ClickRepairQuoteButton()
        {
            ScrollElementIntoView(XPath.Button.RepairQuote);
            Reporting.Log($"Rejecting online settlement", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.RepairQuote);
        }

        private void ClickTakeTimeToDecide()
        {
            ScrollElementIntoView(XPath.Button.TakeTimeToDecide);
            Reporting.Log($"Selecting 'Take time to decide'", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.TakeTimeToDecide);
        }

        private void ClickCallFromRACButton()
        {
            ScrollElementIntoView(XPath.Button.CallFromRac);
            Reporting.Log($"Selecting 'Choose a call from RAC'", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.CallFromRac);
        }

        private void ClickRepairByRacButtonOnSettlementOptionsAndConfirm()
        {
            ClickControl(XPath.SettlementOptionButton.RepairByRac);
            Reporting.Log($"Selected 'Fix my fence for me'", _browser.Driver.TakeSnapshot());
            ClickNextButtonOnSettlementOptions();
            
            _driver.WaitForElementToBeVisible(By.XPath(XPath.Button.ConfirmRacRepair), WaitTimes.T5SEC);
            ClickControl(XPath.Button.ConfirmRacRepair);
            
        }

        private void ClickSeeMyOptionsButtonOnSettlementOptions()
        {
            ClickControl(XPath.SettlementOptionButton.SeeAllOptions);
            Reporting.Log($"Selected 'I'd like to see all my options'", _browser.Driver.TakeSnapshot());
            ClickNextButtonOnSettlementOptions();
        }

        private void ClickNextButtonOnSettlementOptions()
        {
            ClickControl(XPath.SettlementOptionButton.Next);
        }

        private void ClickSeeCashSettlementBreakdownButton()
        {
            ClickControl(XPath.Button.LinkSeeCashSettlementBreakdown);
            // This has been added to allow time for JS to populate with data
            // before subsequent test logic attempts to read fields.
            Thread.Sleep(Constants.General.SleepTimes.T2SEC);
        }

        private void VerifyFenceBreakdownSettlementCost(ClaimHome claim)
        {           
            ClickSeeCashSettlementBreakdownButton();           
            Reporting.AreEqual(claim.FenceSettlementBreakdown.TotalRepairCost, CashSettlementAmount, "Cash Settlement amount from Shield API against the amount displayed");
            Reporting.AreEqual(claim.FenceSettlementBreakdown.NumberOfMetresClaimed, DamageFenceInMeters, "Fence Number of meters value from Shield API against the amount displayed");
            Reporting.AreEqual(claim.FenceSettlementBreakdown.RepairCostBeforeAdjustments, EstimatedRepairCost, "Estimated repair cost amount from Shield API against the amount displayed");
            if (claim.FenceDamage.FenceMaterial == FenceType.Asbestos && claim.FenceDamage.MetresPanelsDamaged > 5)
            {
                Reporting.AreEqual(claim.FenceSettlementBreakdown.AsbestosInspectionFee, AsbestosContribution, "expected Asbestos Inspection Fee amount from Shield API against the amount displayed");
            }

            if(claim.FenceDamage.IsDividingFence)
            {
                Reporting.AreEqual(claim.FenceSettlementBreakdown.DividingFenceAdjustment, SharedFenceAdjustment, "Shared fence adjustment amount from Shield API against the amount displayed");
            }
            
            if (claim.FenceDamage.MetresPanelsPainted > 0)
            {
                Reporting.AreEqual(claim.FenceSettlementBreakdown.PaintingCost, PaintCost, "Paint cost amount from Shield API against the amount displayed");
            }
            Reporting.AreEqual(claim.FenceSettlementBreakdown.CurrentExcess, ExcessPayment, "Your excess payment amount from Shield API against the amount displayed");
            Reporting.AreEqual(claim.FenceSettlementBreakdown.TotalRepairCost, TotalPayment, "Total Fence repair cost amount from Shield API against the amount displayed");
            
        }
    }
}
