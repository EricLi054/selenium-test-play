using Rac.TestAutomation.Common;
using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.General;


namespace UIDriver.Pages.Claims
{
    public class ClaimMotorPage2DamageEventDetails : BasePage
    {
        #region XPATHS
        private const string XP_HEADING      = "//span[@class='action-heading']";
        private const string XP_CLAIM_NUMBER = "id('quote-number')";

        // Vehicle damage details:        
        private const string XPR_YES                   = "//span[text()='Yes']/..";
        private const string XPR_NO                    = "//span[text()='No']/..";        

        private const string XP_WHAT_WAS_STOLEN         = "//span[@aria-owns='MotorDamageDetails_WhatHasBeenStolen_listbox']";
        private const string XP_WHAT_WAS_STOLEN_OPTIONS = "id('MotorDamageDetails_WhatHasBeenStolen_listbox')/li";
        private const string XP_WERE_KEYS_STOLEN_YN     = "id('MotorDamageDetails_VehicleKeysStolen')";
        private const string XP_IS_FINANCE_OWING_YN     = "id('MotorDamageDetails_FinanceOwingOnVehicle')";
        private const string XP_WAS_CAR_FOR_SALE_YN     = "id('MotorDamageDetails_TriedToSellVehicle')";        

        private const string XP_CONTINUE_PAGE_BTN       = "//button[contains(@id,'accordion_0')]";

        // Event details:       
        private const string XP_ACCIDENT_DESCRIPTION     = "id('MotorDamageDetails_AccidentDescription')";

        private const string XP_SUBMIT_PAGE_BTN = "//button[contains(@id,'submit-action')]";
        #endregion

        #region Settable properties and controls
        
        public MotorClaimTheftDetails WhatHasBeenStolen
        {
            get => throw new NotImplementedException("Reading the 'What has been stolen' dropdown has not been implemented yet.");
            set => WaitForSelectableAndPickFromDropdown(XP_WHAT_WAS_STOLEN, XP_WHAT_WAS_STOLEN_OPTIONS, value.GetDescription());
        }

        public bool WereKeysStolen
        {
            get => GetBinaryToggleState(XP_WERE_KEYS_STOLEN_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_WERE_KEYS_STOLEN_YN, XPR_YES, XPR_NO, value);
        }

        public bool IsFinanceOwingOnVehicle
        {
            get => GetBinaryToggleState(XP_IS_FINANCE_OWING_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_IS_FINANCE_OWING_YN, XPR_YES, XPR_NO, value);
        }

        public bool HaveYouTriedToSellYourCar
        {
            get => GetBinaryToggleState(XP_WAS_CAR_FOR_SALE_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_WAS_CAR_FOR_SALE_YN, XPR_YES, XPR_NO, value);
        }

        public string DescriptionOfAccidentEvent
        {
            get => throw new NotImplementedException("Not yet implemented.");
            set => WaitForTextFieldAndEnterText(XP_ACCIDENT_DESCRIPTION, value, false);
        }
        
        #endregion

        public ClaimMotorPage2DamageEventDetails(Browser browser) : base(browser)
        {
        }

        public override bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XP_HEADING);
                GetElement(XP_CLAIM_NUMBER);
                // First accordion button.
                // No other first accordion controls are listed here as they
                // vary significantly based on the Damage Type that the member
                // is claiming for.
                GetElement(XP_CONTINUE_PAGE_BTN);

                Reporting.LogPageChange("Motor claim event damage details page");
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }

        public void FillTheftDetailsAndSubmit(ClaimCar claimData)
        {
            FillVehicleDamageDetailsTheft(claimData);
            
            Reporting.Log("Vehicle event details completed.", _browser.Driver.TakeSnapshot());
            ClickControl(XP_CONTINUE_PAGE_BTN);

            DescriptionOfAccidentEvent = claimData.AccountOfAccident;
        }

        public void SubmitPage2ClaimForm()
        {
            ClickControl(XP_SUBMIT_PAGE_BTN);
        }

        private void FillVehicleDamageDetailsTheft(ClaimCar claimData)
        {
            if (claimData.TheftDetails == null)
            { Reporting.Error("Test has not provided sufficient details for theft scenario."); }

            WhatHasBeenStolen = claimData.TheftDetails.WhatWasStolen;
            FillVehicleDamageDetailsTheftDeclarations(claimData);            
        }

        private void FillVehicleDamageDetailsTheftDeclarations(ClaimCar claimData)
        {
            WereKeysStolen            = claimData.TheftDetails.WereKeysStolen;
            IsFinanceOwingOnVehicle   = claimData.TheftDetails.WasFinanceOwing;
            HaveYouTriedToSellYourCar = claimData.TheftDetails.WasVehicleForSale;
        }
        
    }
}
