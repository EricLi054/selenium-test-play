using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class ContactDetails : SparkBasePage
    {
        #region Constants
        public class Constants
        {
            public const string SubHeader       = "We'll update your contact details if you change them.";
            public const string MobilePreferred = "Mobile preferred so we can SMS you.";
            public const string NotLoginEmail   = "If you change this, it won't affect your login email.";
        }
        #endregion
        #region XPATHS
        public class XPath
        {
            public const string SubHeader           = "//p[contains(text(),'update your contact details if you change them.')]";
            public const string MobilePreferred     = "//p[contains(text(),'we can SMS you.')]";
            public const string NotLoginEmail       = "//p[contains(text(),'your login email')]";
            public class Field
            {
                public const string ContactTelephone = "id('contactNumber')";
                public const string ContactEmail = "id('email')";
            }
            public class Button
            {
                public const string ConfirmDetails = "id('submit-button')";
            }
        }
        #endregion
        
        #region Settable properties and controls
        public string ContactNumber
        {
            get => GetElementValue(XPath.Field.ContactTelephone).Replace(" ", "");

            set => WaitForTextFieldAndEnterText(XPath.Field.ContactTelephone, value, false);
        }

        public string ContactEmail
        {
            get => GetElementValue(XPath.Field.ContactEmail);

            set => WaitForTextFieldAndEnterText(XPath.Field.ContactEmail, value, false);
        }
        #endregion

        public ContactDetails(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.SubHeader);
            }
            catch (Exception e) when (e is NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Home Claim Page - Contact details");
            return true;
        }

        /// <summary>
        /// </summary>
        public void DetailedUiChecking()
        {
            Reporting.AreEqual(Constants.SubHeader, GetInnerText(XPath.SubHeader), "Sub-heading text");
            Reporting.AreEqual(Constants.MobilePreferred, GetInnerText(XPath.MobilePreferred), "Telephone number explanatory text");
            Reporting.AreEqual(Constants.NotLoginEmail, GetInnerText(XPath.NotLoginEmail), "Email explanatory text");
        }

        /// <summary>
        /// If no new mobile number or email address has been provided, confirm displayed information is correct.
        /// If a new value has been provided for input, update that field.
        /// </summary>
        /// <param name="claim">The test data generated for this test.</param>
        public void EnterAndVerifyContactDetails(ClaimHome claim)
        {
            _browser.PercyScreenCheck(DividingFenceClaim.ContactDetails, GetPercyIgnoreCSS());
            Reporting.Log("Capturing Home Claim - Contact Details as displayed when arriving.", _browser.Driver.TakeSnapshot());

            if (!string.IsNullOrEmpty(claim.Claimant.MobilePhoneNumber))
            {
                Reporting.AreEqual(claim.Claimant.MobilePhoneNumber, ContactNumber, "existing Mobile number on Contact details page");
            }
            if (!string.IsNullOrEmpty(claim.Claimant.PrivateEmail?.Address))
            {
                Reporting.AreEqual(claim.Claimant.PrivateEmail.Address, ContactEmail, "existing Email on Contact details page");
            }

            if (claim.IsMobileNumberChanged)
            {
                Reporting.Log($"Updating mobile number to {claim.Claimant.NewMobilePhoneNumber}");
                ContactNumber = claim.Claimant.NewMobilePhoneNumber;
            }
            if (claim.IsEmailAddressChanged)
            {
                Reporting.Log($"Updating email address to {claim.Claimant.PrivateEmail.NewAddress}");
                ContactEmail = claim.Claimant.PrivateEmail.NewAddress;
            }
        }
        
        /// <summary>
        /// Capture a screenshot of the page for Extent Report, then select the button to confirm details
        /// and progress to the next page.
        /// </summary>
        public void ClickConfirm()
        {
            Reporting.Log("Capturing Home Claim - Contact Details before continuing.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Button.ConfirmDetails);
        }

        private List<string> GetPercyIgnoreCSS() =>
          new List<string>
          {
               "#ConfirmDetails-container h2",
               "#contactNumber",
               "#email"
          };
    }
}
