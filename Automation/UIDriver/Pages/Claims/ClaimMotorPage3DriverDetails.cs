using Rac.TestAutomation.Common;
using System;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Claims
{
    public class ClaimMotorPage3DriverDetails : BasePage
    {
        #region XPATHS
        private const string XP_HEADING = "//span[@class='action-heading']";
        private const string XP_CLAIM_NUMBER = "id('quote-number')";

        // Driver details:
        private const string XP_WERE_YOU_DRIVING_YN           = "id('DriverDetails_WereYouTheDriver')";
        private const string XP_DRIVER_LICENSE_CANCELLED_YNU  = "id('DriverDetails_HasBeenSuspendedInLast3Years')";
        private const string XP_DRIVER_UNDER_INFLUENCE_YNU    = "id('DriverDetails_UnderInflenceAtTimeOfAccident')";
        private const string XP_DRIVER_HAD_LIC_2PLUSYEARS_YNU = "id('DriverDetails_HasValidLicenseLongerThan2years')";
        private const string XP_DETAILS_OF_OFFENCES_TEXT      = "id('DriverDetails_LicenseOrOffenceDetails')";
        private const string XPR_YES          = "//span[text()='Yes']/..";
        private const string XPR_NO           = "//span[text()='No']/..";
        private const string XPR_UNKNOWN      = "//span[text()='Unknown']/..";
        private const string XP_DRIVER_TITLE         = "//span[@aria-owns='DriverDetails_DriverTitle_listbox']";
        private const string XP_DRIVER_TITLE_OPTIONS = "id('DriverDetails_DriverTitle_listbox')/li";
        private const string XP_DRIVER_GENDER        = "//span[@aria-owns='DriverDetails_DriverGender_listbox']";
        private const string XP_DRIVER_GENDER_OPTIONS = "id('DriverDetails_DriverGender_listbox')/li";
        private const string XP_DRIVER_NAME_FIRST    = "id('DriverDetails_DriverFirstName')";
        private const string XP_DRIVER_NAME_LAST     = "id('DriverDetails_DriverLastName')";
        private const string XP_DRIVER_PHONENUMBER   = "id('DriverDetails_TelephoneNumber_PhoneNumber')";
        private const string XP_ADDRESS_SEARCH_FIELD = "id('DriverDetails_DriverAddress_qasautocomplete')";
        private const string XP_ADDRESS_QAS_OPTIONS  = "id('DriverDetails_DriverAddress')//table[@class='address-find-table']//tr/td[1]";
        private const string XP_DRIVER_DOB_DAY       = "id('DriverDetails_DriverDOB_Day')";
        private const string XP_DRIVER_DOB_MONTH     = "id('DriverDetails_DriverDOB_Month')";
        private const string XP_DRIVER_DOB_YEAR      = "id('DriverDetails_DriverDOB_Year')";

        private const string XP_SUBMIT_PAGE_BTN = "id('accordion_0_submit-action')";
        #endregion

        #region Settable properties and controls
        public bool WasClaimantDriving
        {
            get => GetBinaryToggleState(XP_WERE_YOU_DRIVING_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_WERE_YOU_DRIVING_YN, XPR_YES, XPR_NO, value);
        }

        public bool? DriverHasSuspendedLicense
        {
            get => GetNullableBinaryForTriStateToggle(
                       XP_DRIVER_LICENSE_CANCELLED_YNU, XPR_YES, XPR_NO, XPR_UNKNOWN);
            set => ClickTriStateToggleWithNullableInput(
                       XP_DRIVER_LICENSE_CANCELLED_YNU, XPR_YES, XPR_NO, XPR_UNKNOWN, value);

        }

        public bool? DriverWasUnderInfluence
        {
            get => GetNullableBinaryForTriStateToggle(
                       XP_DRIVER_UNDER_INFLUENCE_YNU, XPR_YES, XPR_NO, XPR_UNKNOWN);
            set => ClickTriStateToggleWithNullableInput(
                       XP_DRIVER_UNDER_INFLUENCE_YNU, XPR_YES, XPR_NO, XPR_UNKNOWN, value);
        }

        public bool? DriverHadLicenseOver2Years
        {
            get => GetNullableBinaryForTriStateToggle(
                       XP_DRIVER_HAD_LIC_2PLUSYEARS_YNU, XPR_YES, XPR_NO, XPR_UNKNOWN);
            set => ClickTriStateToggleWithNullableInput(
                       XP_DRIVER_HAD_LIC_2PLUSYEARS_YNU, XPR_YES, XPR_NO, XPR_UNKNOWN, value);
        }

        public string DetailsOfSuspensionsOrOffences
        {
            get => throw new NotImplementedException("Not yet implemented.");
            set => WaitForTextFieldAndEnterText(XP_DETAILS_OF_OFFENCES_TEXT, value, false);
        }
        #endregion

        public ClaimMotorPage3DriverDetails(Browser browser) : base(browser)
        {
        }

        public override bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XP_HEADING);
                GetElement(XP_CLAIM_NUMBER);
                GetElement(XP_WERE_YOU_DRIVING_YN);
                GetElement(XP_DRIVER_LICENSE_CANCELLED_YNU);
                GetElement(XP_DRIVER_UNDER_INFLUENCE_YNU);

                Reporting.LogPageChange("Motor claim claimant driver details page");
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }

        public void CompleteDetailsOfDriver(ClaimCar claimData)
        {
            // If driver details is NULL, then we assume that that claimant was driving.
            WasClaimantDriving = claimData.Driver.DriverDetails == null;

            EnterNonClaimantDriver(claimData.Driver.DriverDetails);

            DriverHasSuspendedLicense  = claimData.Driver.WasDriverLicenceSuspended;
            DriverWasUnderInfluence    = claimData.Driver.WasDriverDrunk;
            Reporting.IsFalse(_driver.TryFindElement(By.XPath(XP_DRIVER_HAD_LIC_2PLUSYEARS_YNU), out IWebElement element), "after excess changes release, question about driver having license more than 2 years is not shown");

            if (claimData.Driver.WasDriverLicenceSuspended ||
                claimData.Driver.WasDriverDrunk ||
                !claimData.Driver.DriverLicensedMoreThan2Years)
            {
                DetailsOfSuspensionsOrOffences = claimData.Driver.AdditionalInformation;
            }

            ClickControl(XP_SUBMIT_PAGE_BTN);
        }

        /// <summary>
        /// Enters driver details where test has set driver is not the claimant.
        /// </summary>
        /// <param name="driverInAccident"></param>
        private void EnterNonClaimantDriver(Contact driverInAccident)
        {
            if (WasClaimantDriving)
                return;  // Nothing to do if it was the claimant.

            if (driverInAccident.Title != Title.None)
            {
                _driver.WaitForElementToBeVisible(By.XPath(XP_DRIVER_TITLE), WaitTimes.T5SEC);
                WaitForSelectableAndPickFromDropdown(XP_DRIVER_TITLE, XP_DRIVER_TITLE_OPTIONS, driverInAccident.Title.GetDescription());

                // Claim form is happy to assume gender unless the titles of 'Dr' or 'Mx' are chosen.
                if (driverInAccident.Title == Title.Dr ||
                    driverInAccident.Title == Title.Mx)
                {
                    _driver.WaitForElementToBeVisible(By.XPath(XP_DRIVER_GENDER), WaitTimes.T5SEC);
                    WaitForSelectableAndPickFromDropdown(XP_DRIVER_GENDER, XP_DRIVER_GENDER_OPTIONS, driverInAccident.Gender.GetDescription());
                }
            }

            if (!string.IsNullOrEmpty(driverInAccident.FirstName))
                WaitForTextFieldAndEnterText(XP_DRIVER_NAME_FIRST, driverInAccident.FirstName, false);

            if (!string.IsNullOrEmpty(driverInAccident.Surname))
                WaitForTextFieldAndEnterText(XP_DRIVER_NAME_LAST, driverInAccident.Surname, false);

            if (driverInAccident.DateOfBirth != DateTime.MinValue)
            {
                WaitForTextFieldAndEnterText(XP_DRIVER_DOB_DAY,   driverInAccident.DateOfBirth.Day.ToString(), false);
                WaitForTextFieldAndEnterText(XP_DRIVER_DOB_MONTH, driverInAccident.DateOfBirth.Month.ToString(), false);
                WaitForTextFieldAndEnterText(XP_DRIVER_DOB_YEAR,  driverInAccident.DateOfBirth.Year.ToString(), false);
            }

            if (!string.IsNullOrEmpty(driverInAccident.GetPhone()))
                WaitForTextFieldAndEnterText(XP_DRIVER_PHONENUMBER, driverInAccident.GetPhone(), false);

            if (driverInAccident.MailingAddress != null)
                QASSearchForAddress(XP_ADDRESS_SEARCH_FIELD,
                                    XP_ADDRESS_QAS_OPTIONS,
                                    driverInAccident.MailingAddress.StreetSuburbState());
        }
    }
}
