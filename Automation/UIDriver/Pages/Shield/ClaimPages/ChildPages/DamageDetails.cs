using Rac.TestAutomation.Common;
using OpenQA.Selenium;
using System.ComponentModel;

namespace UIDriver.Pages.Shield
{
    public class DamageDetails : BaseShieldPage
    {

        public enum CLAIM_TABS
        {
            [Description("Damage Details")]
            DamageDetails,
            [Description("Damage History")]
            DamageHistory
        }

        private class XPath
        {
            public class Tab
            {
                public const string Current = "//ul[contains(@class,'idit-tabs-nav')]/li[@aria-selected='true']";
            }
            public class DamageStatus
            {
                public const string Label = "id('s2id_stateVO')/../label";
            }
            public class DamageStatusReason
            {
                public const string Label = "id('s2id_stateReason')/../label";
            }
            public class LiabilityReserves
            {
                public class Button
                {
                    public const string Add  = "id('IDITForm@liabilityReservesList|New')";
                    public const string View = "id('IDITForm@liabilityReservesList|View')";
                }
                public class Table
                {
                    public const string Row = "id('idit-grid-table-IDITForm_at_liabilityReservesList_pipe_')//tr//td[@aria-describedby='idit-grid-table-IDITForm_at_liabilityReservesList_pipe__claimReserveReasonVO']";
                }
            }
            public class RecoveryReserves
            {
                public class Button
                {
                    public const string View = "id('IDITForm@recoveryReservesList|View')";
                }
                public class Table
                {
                    public const string Row = "id('idit-grid-table-IDITForm_at_recoveryReservesList_pipe_')//tr//td[@aria-describedby='idit-grid-table-IDITForm_at_recoveryReservesList_pipe__claimReserveReasonVO']";
                }
            }
            public class ClaimPayments
            {
                public class Button
                {
                    public const string Add = "id('IDITForm@paymentsList|New')";
                }
            }
            public class Button
            {
                public const string OK     = "id('OK')";
                public const string Return = "id('Return')";
            }
        }

        #region Settable properties and controls
        public CLAIM_TABS CurrentTab
        {
            get => DataHelper.GetValueFromDescription<CLAIM_TABS>(GetElement(XPath.Tab.Current).GetAttribute("title"));
            set
            {
                var tabControl = GetElement($"//li[@title='{value.GetDescription()}']");
                if (tabControl.GetAttribute("aria-selected") != "true")
                    tabControl.Click();
            }
        }

        public string DamageStatus => GetElement(XPath.DamageStatus.Label).GetAttribute("title");
        public string DamageStatusReason => GetElement(XPath.DamageStatusReason.Label).GetAttribute("title");
        #endregion

        public DamageDetails(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XPath.DamageStatus.Label);
                GetElement(XPath.DamageStatusReason.Label);
                GetElement(XPath.LiabilityReserves.Button.View);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Clicks the "+" icon on the liabilities reserve table to begin the
        /// process of defining a new reserve on the claim.
        /// </summary>
        public void AddLiabilityReserve()
        {
            ClickControl(XPath.LiabilityReserves.Button.Add);
        }
        
        /// <summary>
        /// Click "OK" to complete updates to damage details on the claim.
        /// </summary>
        public void ClickOK()
        {
            ClickControl(XPath.Button.OK);
        }

        /// <summary>
        /// Because we can no longer detect the Shield confirmation pop
        /// up dialogs, this method will monitor the "OK" button
        /// disappearing as a means to determine that we have exited
        /// Update mode successfully.
        /// </summary>
        public void WaitForOkToDisappear()
        {
            _driver.WaitForElementToBeInvisible(By.XPath(XPath.Button.OK), Constants.General.WaitTimes.T30SEC);
        }

        public void ClickReturn()
        {
            ClickControl(XPath.Button.Return);
        }
    }
}
