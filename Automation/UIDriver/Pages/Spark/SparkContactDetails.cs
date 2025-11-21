using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Collections.Generic;

using static Rac.TestAutomation.Common.Constants.Contacts;

namespace UIDriver.Pages.Spark
{
    public class SparkContactDetails : SparkBasePage
    {
        #region Constants
        private class Constants
        {
            public static readonly string SubHeader = "We'll update your contact details if you change them.";
            public static readonly string ActiveStepperLabel = "Contact details";
            public static string PageHeader(string firstName) => $"{firstName}, please confirm your contact details";
            public class Fields
            {
                public static readonly string PhoneTypeLabelMobile = "Mobile";
                public static readonly string PhoneTypeLabelHome = "Home";
                public static readonly string PhoneTypeLabelWork = "Work";
                public static readonly string ContactEmailLabel = "Contact email";
            }
            public class Dialog
            {
                public static readonly string Title = "Update your contact details";
                public static readonly string ContentPrefix = "If you no longer have access to ";
                public static readonly string ContentSuffix = " call us on 13 17 03.";
                public class MFAButtons
                {
                    public static readonly string Cancel = "Cancel";
                    public static readonly string UpdateDetails = "Update details";
                }
            }
            public class Buttons
            {
                public static readonly string YesThisIsCorrect = "Yes, this is correct";
                public static readonly string UpdateContactDetailsMyRac = "Update contact details in myRAC";
            }
        }
        #endregion
        #region XPATHS
        public class XPath
        {
            public static readonly string SubHeader = "//*[@data-testid='please-confirm-details']";
            public static readonly string PageHeader = "id('please-confirm-details')";
            public class Fields
            {
                public static readonly string PhoneTypeLabel        = "id('phone-type')";
                public static readonly string PhoneNumberMasked     = "id('masked-phone-number')";
                public static readonly string ContactEmailLabel     = "id('contact-email-header')";
                public static readonly string ContactEmailMasked    = "id('contact-email')";
            }
            public class Dialog
            {
                public static readonly string Title     = "id('dialog-title')";
                public static readonly string Content   = "id('dialog-content')";
                public class MFAButtons
                {
                    public static readonly string Cancel        = "//button[@data-testid='dialog-cancel-button']";
                    public static readonly string UpdateDetails = "//button[@data-testid='dialog-confirm-button']";
                }
            }
            public class Buttons
            {
                public static readonly string YesThisIsCorrect          = "id('yes-this-is-correct-button')";
                public static readonly string UpdateContactDetailsMyRac = "//button[contains(text(),'contact details in myRAC')]";
            }
        }
        #endregion
       
        public SparkContactDetails(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.SubHeader);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Contact details page");
            Reporting.Log("Contact details page", _browser.Driver.TakeSnapshot());
            return true;
        }

        public void UpdateMyDetailsInMyRAC()
        {
            Reporting.Log($"Selecting the 'Update contact details in myRAC' button.");
            ClickControl(XPath.Buttons.UpdateContactDetailsMyRac);
        }

        public void HandleTelephoneNumberWarningDialog()
        {
            Reporting.Log($"Capturing pop-up dialog", _browser.Driver.TakeSnapshot());
            Reporting.AreEqual(Constants.Dialog.Title, GetInnerText(XPath.Dialog.Title), "expected dialog Title matches actual display");
            ClickControl(XPath.Dialog.MFAButtons.UpdateDetails);
        }

        /// <summary>
        /// If a test includes a detailUiChecking flag set to True then confirm the content of these elements on the 
        /// Contact details page.
        /// </summary>
        public void DetailedUiChecking()
        {
            Reporting.AreEqual(Constants.ActiveStepperLabel, GetInnerText(XPaths.ActiveStepper), "label of active stepper with the displayed value");
            
            Reporting.AreEqual(Constants.Fields.PhoneTypeLabelMobile, GetInnerText(XPath.Fields.PhoneTypeLabel), "Phone type label");
            Reporting.AreEqual(Constants.Fields.ContactEmailLabel, GetInnerText(XPath.Fields.ContactEmailLabel), "Email label");
        }

        public void VerifyExistingContactDetails(string firstName, string phoneNumber = null, string emailAddress = null)
        {
            Reporting.AreEqual(Constants.PageHeader(firstName), GetInnerText(XPath.PageHeader), ignoreCase: true, "Page heading text");

            Reporting.IsNotNull(phoneNumber, "mobile phone number. Test data error, automation does not support handling members without a mobile phone number");

            Reporting.AreEqual(DataHelper.MaskPhoneNumber(phoneNumber), GetInnerText(XPath.Fields.PhoneNumberMasked),
                "Masked phone number on Contact details page");

            if (!string.IsNullOrEmpty(emailAddress))
            {
                Reporting.AreEqual(DataHelper.MaskEmailAddress(emailAddress), GetInnerText(XPath.Fields.ContactEmailMasked),
                    ignoreCase: true, "Masked Email on Contact details page");
            }
            else
            {
                Reporting.Error($"Email Address in test data for contact is null or empty, cannot compare with on-screen value '{GetInnerText(XPath.Fields.ContactEmailMasked)}'.");
            }
        }

        public void AssertDetailsAreCorrect()
        {
            Reporting.Log($"Capturing 'Contact details' page before attempting to progress to the next page by selecting 'Yes, this is correct'.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Buttons.YesThisIsCorrect);
        }

        public List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
               "#ConfirmDetails-container h2",
               "#contactNumber",
               "#email"
          };
    }
}
