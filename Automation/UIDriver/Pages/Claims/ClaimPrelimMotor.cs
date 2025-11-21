using Rac.TestAutomation.Common;
using System;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.Contacts;

namespace UIDriver.Pages.Claims
{
    public class ClaimPrelimMotor : BaseClaimsPrelimDetails
    {

        private class XPath
        {
            public class Input
            {
                public const string FindByRego = "id('PrelimDetails_PolicyDetails_LicensePlate')";
                public const string EventLocation = "id('PrelimDetails_ContactDetails_AccidentLocation')";
            }
        }

        #region Settable properties and controls
        public string VehicleRego
        {
            get => throw new NotImplementedException("Need to implement means to navigate shadow DOM to get this value.");
            set => WaitForTextFieldAndEnterText(XPath.Input.FindByRego, value, false);
        }

        public MotorClaimDamageType DamageType
        {
            get => throw new NotImplementedException("Not yet implemented.");
            set
            {
                string damageTypeText = MotorClaimDamageTypeNames[value].TextB2C;
                WaitForSelectableAndPickFromDropdown(BaseXPath.DamageType.Dropdown, BaseXPath.DamageType.Options, damageTypeText);
            }
        }

        public string EventLocation
        {
            get => throw new NotImplementedException("Need to implement means to navigate shadow DOM to get this value.");
            set => WaitForTextFieldAndEnterText(XPath.Input.EventLocation, value, false);
        }
        #endregion

        public ClaimPrelimMotor(Browser browser) : base(browser)
        { }

        /// <summary>
        /// Completes the first (of two) accordions on the Prelim Questions
        /// page. This first accordion is to find the policy that the user
        /// is looking to record a claim against.
        /// </summary>
        /// <param name="claimData"></param>
        public void FillFormForFindMyPolicy(ClaimCar claimData, bool findPolicyByPolicyNumber = false)
        {
            if (!IsMemberLoggedIntoPCM())
            {
                KnowsPolicyNumber = findPolicyByPolicyNumber;

                if (findPolicyByPolicyNumber)
                {
                    PolicyNumber = claimData.Policy.PolicyNumber;
                }
                else
                {
                    FindPolicySurname = claimData.Claimant.Surname;
                    FindPolicyDateOfBirth = claimData.Claimant.DateOfBirth;
                    VehicleRego = claimData.Policy.Vehicle.Registration;
                }
            }

            EventDate = claimData.EventDateTime;
            EventTime = claimData.EventDateTime;

            Reporting.Log("First accordion of claim preliminary details completed.", _driver.TakeSnapshot());
            ClickControl(BaseXPath.Button.ContinueFirstAccordion);
        }

        /// <summary>
        /// Completes the second (of two) accordions on the Prelim Questions
        /// page. This second accordion will be to confirm which policyholder
        /// is looking to lodge the claim, as well as the initial damage type
        /// for the claim.
        /// </summary>
        /// <param name="claimData"></param>
        public void FillFormForDamageTypeAndInitialEventDetails(ClaimCar claimData)
        {
            DamageType         = claimData.DamageType;            
            if (!IsMemberLoggedIntoPCM())
            {
                ConfirmSurname = claimData.Claimant.Surname;
                ConfirmDateOfBirth = claimData.Claimant.DateOfBirth;
            }
            Email              = claimData.Claimant.GetEmail() ?? "NPE.b2cadmin@rac.com.au";
            PhoneNumber        = claimData.Claimant.GetPhone() ?? $"04{DataHelper.RandomNumbersAsString(8)}";

            EventLocation = claimData.EventLocation;
            SetPoliceReportDetails(claimData.PoliceReportNumber, claimData.PoliceReportDate);
            
            Reporting.Log("Second accordion of claim preliminary details completed.", _driver.TakeSnapshot());
        }

        /// <summary>
        /// Even though the submission of the first accordion involves a query
        /// to Shield, there is no spinner presented. The transition to the
        /// second accordion only occurs once the related policy has been found.
        /// We wait for our key desired question controls to be visible to know
        /// when the test can proceed.
        /// </summary>
        public void WaitForDamageTypeQuestionsToBecomeVisible()
        {
            _driver.WaitForElementToBeVisible(By.XPath(BaseXPath.DamageType.Dropdown), WaitTimes.T30SEC);
            _driver.WaitForElementToBeVisible(By.XPath(BaseXPath.Button.ContinueSecondAccordion), WaitTimes.T30SEC);
        }

        private bool IsMemberLoggedIntoPCM()
        {
            return _browser.Driver.Url.Contains("/Secure/");
        }
    }
}
