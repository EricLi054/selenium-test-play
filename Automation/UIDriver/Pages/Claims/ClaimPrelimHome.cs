using Rac.TestAutomation.Common;
using System;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Claims
{
    public class ClaimPrelimHome : BaseClaimsPrelimDetails
    {
        private class XPath
        {
            public class Text
            {
                public const string CoversAffectedHelp = "id('damageAffectedCoverTypeHelpTextContainer')";
            }
            public class Button
            {
                public const string CoversAffected = "//span[contains(text(),'{0}')]";
            }
            public class ValidationMessage
            {
                public const string EventDate = "//span[contains(text(),'Please enter a valid event date in the format dd/mm/yyyy.')]";
                public const string EventTime = "//span[contains(text(),'Please enter a valid event time.')]";
                public const string Address   = "//span[contains(text(),'Please enter a valid address.')]";
                public const string BirthDate = "//span[contains(text(),'Please enter a valid Date of birth.')]";
                public const string Surname   = "//span[contains(text(),'Surname must be entered.')]";
            }
        }
        #region XPATHS
        // Find my policy
        private const string XP_HOME_ADDRESS = "id('PrelimDetails_PolicyDetails_InsuredAddress_qasautocomplete')";
        private const string XP_ADDRESS_QAS_OPTIONS = "id('PrelimDetails_PolicyDetails_InsuredAddress')//table[@class='address-find-table']//tr/td[1]";


        #endregion

        #region Settable properties and controls
        public bool IsSurnameValidationPresent => HasDriverElement(XPath.ValidationMessage.Surname);
        public bool IsDateOfBirthValidationPresent => HasDriverElement(XPath.ValidationMessage.BirthDate);
        public bool IsAddressValidationPresent => HasDriverElement(XPath.ValidationMessage.Address);
        public bool IsEventDateValidationPresent => HasDriverElement(XPath.ValidationMessage.EventDate);
        public bool IsEventTimeValidationPresent => HasDriverElement(XPath.ValidationMessage.EventTime);
        #endregion

        public ClaimPrelimHome(Browser browser) : base(browser)
        { }

        /// <summary>
        /// Completes the first (of two) accordions on the Prelim Questions
        /// page. This first accordion is to find the policy that the user
        /// is looking to record a claim against.
        /// </summary>
        /// <param name="claimData"></param>
        public void FillFormForFindMyPolicy(ClaimHome claimData, bool findPolicyByPolicyNumber = false)
        {
            if (!IsMemberLoggedIntoPCM())
            {
                KnowsPolicyNumber = findPolicyByPolicyNumber;

                if (findPolicyByPolicyNumber)
                {
                    PolicyNumber = claimData.PolicyDetails.PolicyNumber;
                }
                else
                {
                    FindPolicySurname = claimData.Claimant.Surname;
                    FindPolicyDateOfBirth = claimData.Claimant.DateOfBirth;
                    QASSearchForAddress(XP_HOME_ADDRESS, XP_ADDRESS_QAS_OPTIONS,
                                        claimData.PolicyDetails.HomeAsset.Address.StreetSuburbState());
                }
            }
            
            EventDate = claimData.EventDateTime;
            EventTime = claimData.EventDateTime;

            Reporting.Log("First accordion of claim preliminary details completed.", _driver.TakeSnapshot());
            ClickContinueButton();
        }

        public void ClickContinueButton()
        {
            ClickControl(BaseXPath.Button.ContinueFirstAccordion);
        }

        /// <summary>
        /// Completes the second (of two) accordions on the Prelim Questions
        /// page. This second accordion will be to confirm which policyholder
        /// is looking to lodge the claim, as well as the initial damage type
        /// for the claim.
        /// </summary>
        /// <param name="claimData"></param>
        public void FillFormForDamageTypeAndInitialEventDetails(ClaimHome claimData)
        {
            WaitForSelectableAndPickFromDropdown(BaseXPath.DamageType.Dropdown, BaseXPath.DamageType.Options,
                                                 HomeClaimDamageTypeNames[claimData.DamageType].TextB2C);

            if (claimData.DamageType != HomeClaimDamageType.AccidentalDamage)
            {
                SetAffectedCovers(claimData);
            }
            
            if (!IsMemberLoggedIntoPCM())
            {
                ConfirmSurname     = claimData.Claimant.Surname;
                ConfirmDateOfBirth = claimData.Claimant.DateOfBirth;
            }
            if (string.IsNullOrEmpty(claimData.Claimant.GetEmail()) ||
                !claimData.Claimant.GetEmail().EndsWith(Config.Get().Email.Domain))
            {
                claimData.Claimant.PrivateEmail = DataHelper.RandomEmail(firstName: claimData.Claimant.FirstName, 
                                                                            surname: claimData.Claimant.Surname, 
                                                                            domain: Config.Get().Email.Domain);
                Reporting.Log($"As existing email address on record was not a @{Config.Get().Email.Domain} address " +
                    $"have generated a new email address: {claimData.Claimant.PrivateEmail.Address}");
            }
            Email = claimData.Claimant.GetEmail();
           
            PhoneNumber = claimData.Claimant.GetPhone() ?? $"04{DataHelper.RandomNumbersAsString(8)}";
            if (claimData.DamageType == HomeClaimDamageType.Theft ||
                claimData.DamageType == HomeClaimDamageType.MaliciousDamage ||
                claimData.DamageType == HomeClaimDamageType.ImpactOfVehicle ||
                claimData.DamageType == HomeClaimDamageType.Fire)
            {
                SetPoliceReportDetails(claimData.PoliceReportNumber, claimData.PoliceReportDate);
            }

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

        private bool HasDriverElement(string elementPath) => _driver.TryFindElement(By.XPath(elementPath), out IWebElement _);

        private void SetAffectedCovers(ClaimHome claim)
        {
            // Only need to action if the policy has both building and contents covers
            // We don't need to check for invalid combinations as our HomeClaimBuilder
            // will have blocked those.
            if (!claim.PolicyDetails.HasBuildingCover() ||
                !claim.PolicyDetails.HasContentsCover())
            { return; }

            ClickControl(string.Format(XPath.Button.CoversAffected, claim.DamagedCovers.GetDescription()));

            _driver.WaitForElementToBeVisible(By.XPath(XPath.Text.CoversAffectedHelp), WaitTimes.T5SEC);
        }
    }
}
