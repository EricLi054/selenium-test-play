using Rac.TestAutomation.Common;
using OpenQA.Selenium;
using System;
using System.ComponentModel;

using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    public class TreeItemDamagesQuestionnairesAgenda : BaseShieldPage
    {
        public enum CLAIM_TABS
        {
            [Description("Related Policy")]
            RelatedPolicy,
            [Description("Damages")]
            Damages,
            [Description("Questionnaires")]
            Questionnaires,
            [Description("Agenda")]
            Agenda,
            [Description("Payments & Reserves")]
            PaymentsAndReserves
        }

        public enum AGENDA_STEPS
        {
            ClaimLodged,
            InvoiceReceived,
            PaymentsSettled,
            RepairsComplete,
            ItemsReplacedOrPaidOut
        }

        public enum AGENDA_STATUS_REASON
        {
            [Description("Select")]
            Select,
            [Description("Claimant status changed to Closed or Cancelled")]
            ClaimantClosedOrCancelled,
            [Description("Claim Closed")]
            ClaimClosed,
            [Description("Claim Denied")]
            ClaimDenied,
            [Description("Damage Cancelled")]
            DamageCancelled,
            [Description("Fraud Suspected")]
            FraudSuspected,
            [Description("Not Relevant for this Claim")]
            NotRelevant,
            [Description("Step Completed")]
            StepCompleted
        }

        #region XPATHS
        private const string XP_BUTTON_END_DAMAGES = "//button[@id='Finish' or @id='nextFromClaimantAssetTabsScreen']";

        // Tabs
        private const string XP_TAB_CURRENT_TAB = "//ul[contains(@class,'idit-tabs-nav')]/li[@aria-selected='true']";

        // General
        private const string XP_DROPDOWN_OPTIONS = "id('select2-drop')//li/div";

        // Damages:
        private const string XP_DAMAGES_TABLE               = "id('gview_idit-grid-table-flattendListclaimantAssetTree_pipe_')";
        private const string XP_DAMAGES_NOT_COVERED_TABLE   = "id('gview_idit-grid-table-IDITForm_at_damagedItemsList_pipe_')";

        // Agenda:
        private const string XP_BUTTON_EDIT_AGENDA_ITEM  = "id('flattendListagendaTree|Update')";
        private const string XP_BASE_AGENDA_TABLE        = "id('idit-grid-table-flattendListagendaTree_pipe_')";
        private const string XP_ROW_AGENDA_STEP_INVOICED = XP_BASE_AGENDA_TABLE + "//td[@title='Motor - Invoice Received or Claim Paid Out']";
        private const string XP_ROW_AGENDA_STEP_CONTENTS_ITEMS_PAID = XP_BASE_AGENDA_TABLE + "//td[@title='Contents - Items Replaced or Paid Out']";
        private const string XP_ROW_AGENDA_STEP_BUILDING_REPAIRS_COMPLETE = XP_BASE_AGENDA_TABLE + "//td[@title='Building - Repairs Complete']";

        // Agenda edit step controls:
        private const string XP_BUTTON_EDIT_STEP_STATUS  = "id('s2id_agendaStepStatus')";

        private const string XP_BUTTON_EDIT_STEP_REASON = "id('s2id_agendaStepStatusReason')";

        //Home Claim Circumstances Questionnaires
        private const string XP_MEMBER_PROVIDING_OWN_REPORT         = "id('s2id_4000013@4000043')";
        private const string XP_BUILDER_REQUIRED                    = "id('s2id_68000001@68000016')";
        private const string XP_RESTORER_REQUIRED_FOR_BUILDING      = "id('s2id_68000001@68000022')";
        private const string XP_RESTORER_REQUIRED_FOR_CONTENTS      = "id('s2id_68000004@68000011')";

        //Motor Claim Accident Circumstances Questionnaires
        private const string XP_ACTION_REQUIRED_FOR_HOLD_QUESTION   = "id('TR4000010@4000040')";
        private const string XP_ACTION_REQUIRED_FOR_HOLD_DROPDOWN   = "id('s2id_4000010@4000040')";

        private const string XP_BUTTON_EDIT_STEP_FINISH = "id('OKagendaStepContainer')";
        #endregion

        #region Settable properties and controls
        public CLAIM_TABS CurrentTab
        {
            get => DataHelper.GetValueFromDescription<CLAIM_TABS>(GetElement(XP_TAB_CURRENT_TAB).GetAttribute("title"));
            set
            {
                string tabTitle = value.GetDescription();
                var tabControl = GetElement($"//li[@title='{tabTitle}']");
                if (tabControl.GetAttribute("aria-selected") != "true")
                    ClickControl($"//li[@title='{tabTitle}']");
            }
        }

        /// <summary>
        /// Only applicable if in the sub-screen for editing an Agenda Step
        /// </summary>
        public AgendaStepStatus AgendaStepEditStatus
        {
            get => DataHelper.GetValueFromDescription<AgendaStepStatus>(GetInnerText($"{XP_BUTTON_EDIT_STEP_STATUS}//span[@class='select2-chosen']"));
            set => WaitForSelectableAndPickFromDropdown(XP_BUTTON_EDIT_STEP_STATUS,
                                                        XP_DROPDOWN_OPTIONS,
                                                        value.GetDescription());
        }

        /// <summary>
        /// Only applicable if in the sub-screen for editing an Agenda Step
        /// </summary>
        public AGENDA_STATUS_REASON AgendaStepEditReason
        {
            get => DataHelper.GetValueFromDescription<AGENDA_STATUS_REASON>(GetInnerText($"{XP_BUTTON_EDIT_STEP_REASON}//span[@class='select2-chosen']"));
            set => WaitForSelectableAndPickFromDropdown(XP_BUTTON_EDIT_STEP_REASON,
                                                        XP_DROPDOWN_OPTIONS,
                                                        value.GetDescription());
        }
        #endregion

        public TreeItemDamagesQuestionnairesAgenda(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                switch (CurrentTab)
                {
                    case CLAIM_TABS.Damages:
                        GetElement(XP_DAMAGES_TABLE);
                        GetElement(XP_DAMAGES_NOT_COVERED_TABLE);
                        break;
                    case CLAIM_TABS.Agenda:
                        GetElement(XP_BASE_AGENDA_TABLE);
                        break;
                    default:
                        break;
                }
                GetElement(XP_TAB_CURRENT_TAB);
                GetElement(XP_BUTTON_END_DAMAGES);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Even though this is sort of a separate screen, it is quite small and
        /// only accessible through the claim tree agenda, so for now its here,
        /// but we can look to move it if later test activities show a need.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="status"></param>
        /// <param name="reason"></param>
        public void EditAgendaStep(AGENDA_STEPS step, AgendaStepStatus status, AGENDA_STATUS_REASON reason)
        {
            var xPathToStepToOpen = string.Empty;
            switch(step)
            {
                case AGENDA_STEPS.InvoiceReceived:
                    xPathToStepToOpen = XP_ROW_AGENDA_STEP_INVOICED;
                    break;
                case AGENDA_STEPS.ItemsReplacedOrPaidOut:
                    xPathToStepToOpen = XP_ROW_AGENDA_STEP_CONTENTS_ITEMS_PAID;
                    break;
                case AGENDA_STEPS.RepairsComplete:
                    xPathToStepToOpen = XP_ROW_AGENDA_STEP_BUILDING_REPAIRS_COMPLETE;
                    break;
                default:
                    throw new NotImplementedException("Haven't built support to edit your requested Agenda Step at this stage.");
            }

            ClickControl(xPathToStepToOpen);
            ClickControl(XP_BUTTON_EDIT_AGENDA_ITEM);

            _driver.WaitForElementToBeVisible(By.XPath(XP_BUTTON_EDIT_STEP_STATUS), WaitTimes.T30SEC);
            AgendaStepEditStatus = status;
            AgendaStepEditReason = reason;

            ClickControl(XP_BUTTON_EDIT_STEP_FINISH);
        }

        /// <summary>
        /// Click the "End Damages" button for the form relating to the claim
        /// Agenda, Questionnaires and Damages.
        /// </summary>
        public void ClickEndDamages()
        {
            ClickControl(XP_BUTTON_END_DAMAGES);
        }

        /// <summary>
        /// Because we can no longer detect the Shield confirmation pop
        /// up dialogs, this method will monitor the "End Damages" button
        /// disappearing as a means to determine that we have exited
        /// Update mode successfully.
        /// </summary>
        public void WaitForEndDamagesButtonToDisappear()
        {
            _driver.WaitForElementToBeInvisible(By.XPath(XP_BUTTON_END_DAMAGES), WaitTimes.T30SEC);
        }

        /// <summary>
        /// In the Accident Circumstances Questionnaires section there is a question
        /// called 'is the member providing their own report'.
        /// This method allows to set the drop down value for that
        /// </summary>
        /// <param name="memberProvidingOwnReport">Dropdown text option: Select/Yes/No</param>
        public void SelectMemberProvidingOwnReport(string memberProvidingOwnReport) =>
            WaitForSelectableAndPickFromDropdown(XP_MEMBER_PROVIDING_OWN_REPORT, XP_DROPDOWN_OPTIONS, memberProvidingOwnReport);

        public bool IsQuestionShownActionNeededFor21DayHold() =>
            _driver.TryWaitForElementToBeVisible(By.XPath(XP_ACTION_REQUIRED_FOR_HOLD_QUESTION), WaitTimes.T5SEC, out IWebElement element);

        public void AnswerActionNeededFor21DayHold(string action) =>
            WaitForSelectableAndPickFromDropdown(XP_ACTION_REQUIRED_FOR_HOLD_DROPDOWN, XP_DROPDOWN_OPTIONS, action);

        public void SelectIsBuilderRequired(string isBuilderRequired) =>
            WaitForSelectableAndPickFromDropdown(XP_BUILDER_REQUIRED, XP_DROPDOWN_OPTIONS, isBuilderRequired);

        public void SelectIsRestorerRequiredForBuilding(string isRestorerRequired) =>
            WaitForSelectableAndPickFromDropdown(XP_RESTORER_REQUIRED_FOR_BUILDING, XP_DROPDOWN_OPTIONS, isRestorerRequired);

        public void SelectIsRestorerRequiredForContents(string isRestorerRequired) =>
            WaitForSelectableAndPickFromDropdown(XP_RESTORER_REQUIRED_FOR_CONTENTS, XP_DROPDOWN_OPTIONS, isRestorerRequired);
    }
}
