using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.B2C
{
    public class MotorQuote3Policy : BasePage
    {
        /// <summary>
        /// Flag for tracking when we have set the first policyholder while entering
        /// driver details. This is important as only the first policyholder gets
        /// prompted for their preferred delivery method (PDM).
        /// We need this to know when to look for the PDM toggle controls.
        /// </summary>
        private bool _haveSetFirstPolicyHolder = false;

        #region XPATHS
        private const string BASE = "/html/body/div[@id='wrapper']";
        private const string MAIN = BASE + "/div/div[contains(@class,'body-content')]/form";

        private const string XP_PAGE_HEADING           = BASE + "//span[@class='action-heading']/span";
        private const string XP_PAGE_STICKY_HEADING    = "/html/body/div[contains(@class,'b2c-quoteheader')]";

        private const string XPR_YES = "//span[text()='Yes']/..";
        private const string XPR_NO = "//span[text()='No']/..";

        private const string XP_VEHICLE_HEADING        = MAIN + "/div/div[@data-accordion-id='Policy']";
        private const string XP_DRIVER_X_HEADING       = MAIN + "/div/div[starts-with(@data-accordion-id,'Policy_Contacts_')]";
        private const string XP_PANEL_BODY_X           = MAIN + "/div/div[starts-with(@class,'accordion-panel')]";


        private const string XPR_MODIFICATIONS_YES = "/div[@id='modifications']//div[@id='Policy_HasModifications_True_Label']";
        private const string XPR_MODIFICATIONS_NO  = "/div[@id='modifications']//div[@id='Policy_HasModifications_False_Label']";

        private const string XPR_REGO_NUM_TEXT     = "//input[@id='Policy_VehicleRegistration_Number']";
        private const string XPR_REGO_DONT_KNOW    = "//div[@class='checkbox']";
        private const string XPR_REGO_NOTIFICATION = "//div[@class='info-box vehicle-registration-isunknown-box']";
        private const string XPR_CURRENT_INSURER   = "//span[@aria-owns='Policy_CurrentInsurer_listbox']";
        private const string XPR_FINANCIER         = "//span[@aria-owns='Policy_FinanceCompany_listbox']";

        private const string XPR_MEMBERSHIP_NUM    = "//input[@id='Policy_Contacts_0__MembershipNumber']";

        private const string XPR_DRIVER_TITLE       = "//span[@aria-owns='Policy_Contacts_{0}__Name_Title_listbox']";
        private const string XPR_DRIVER_FIRSTNAME   = "//input[@id='Policy_Contacts_{0}__Name_FirstName']";
        private const string XPR_DRIVER_MIDDLENAME  = "//input[@id='Policy_Contacts_{0}__Name_MiddleName']";
        private const string XPR_DRIVER_SURNAME     = "//input[@id='Policy_Contacts_{0}__Name_LastName']";
        private const string XPR_DRIVER_DOB         = "//div[@data-wrapper-for='Policy_Contacts_{0}__Dob']//div[@class='display-answer']";
        private const string XPR_IS_ADDRESS_THE_SAME_YN = "//div[contains(@id,'__IsMailingAddressSameAsLocationAddress')]";
        private const string XPR_POLICYHOLDER_YES   = "//div[@id='Policy_Contacts_{0}__IsPolicyHolder_True_Label']";
        private const string XPR_POLICYHOLDER_NO    = "//div[@id='Policy_Contacts_{0}__IsPolicyHolder_False_Label']";
        private const string XPR_MAILINGADDRESS     = "//input[@id='Policy_Contacts_{0}__MailingAddress_qasautocomplete']";
        private const string XPR_MAILADDR_COMPLETE  = "//div[@id='Policy_Contacts_{0}__MailingAddress']//div[contains(@class,'selectedAddressDescription')]";
        private const string XPR_ADDR_SUGGESTION_X  = "//div[@id='Policy_Contacts_{0}__MailingAddress']//table[@class='address-find-table']//tr/td[1]";
        private const string XPR_DRIVER_PHONE       = "//input[@id='Policy_Contacts_{0}__PhoneNumber']";
        private const string XPR_DRIVER_EMAIL       = "//input[@id='Policy_Contacts_{0}__EmailAddress']";
        private const string XPR_ADDRESS_SAME_YES   = "//div[@id='Policy_Contacts_{0}__IsMailingAddressSameAsDriver1_True_Label']";
        private const string XPR_ADDRESS_SAME_NO    = "//div[@id='Policy_Contacts_{0}__IsMailingAddressSameAsDriver1_False_Label']";
        private const string XPR_DRIVER_PDM_TOGGLE  = "//div[@id='Policy_Contacts_{0}__RenewPolicyUsingEmail']";
        private const string XPR_CONTINUE_BTN = "//button[contains(@class,'accordion-button')]";

        private const string XPR_EMAIL = "//span[text()='Email']/..";
        private const string XPR_POST  = "//span[text()='Post']/..";
        #endregion

        #region Settable properties and controls
        public bool IsCarModified
        {
            get => GetBinaryToggleState($"{XP_PANEL_BODY_X}[1]", XPR_MODIFICATIONS_YES, XPR_MODIFICATIONS_NO);
            set => ClickBinaryToggle($"{XP_PANEL_BODY_X}[1]", XPR_MODIFICATIONS_YES, XPR_MODIFICATIONS_NO, value);
        }

        public string VehicleRegistration
        {
            get
            {
                var element = GetElement(XP_PANEL_BODY_X + "[1]" + XPR_REGO_NUM_TEXT);
                // If empty, return placeholder text.
                if (String.IsNullOrEmpty(element.Text)) { return element.GetAttribute("value"); }

                return element.Text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Reporting.Log($"VehicleRegistration setting value = Don't Know");
                    ClickDontKnowRego(true);
                }
                else
                {
                    Reporting.Log($"VehicleRegistration setting value = {value}");
                    ClickDontKnowRego(false);
                    Thread.Sleep(500);  // The rego field needs to transition to editable
                    WaitForTextFieldAndEnterText(XP_PANEL_BODY_X + "[1]" + XPR_REGO_NUM_TEXT, value);
                }
            }
        }

        public string RACMembershipNumber
        {
            get
            {
                var element = GetElement(XP_PANEL_BODY_X + XPR_MEMBERSHIP_NUM);

                // If empty, return placeholder text.
                if (String.IsNullOrEmpty(element.Text)) return element.GetAttribute("value");

                return element.Text;
            }
            set => WaitForTextFieldAndEnterText(XP_PANEL_BODY_X + XPR_MEMBERSHIP_NUM, value);
        }

        public string CurrentRecentInsurer
        {
            get => GetInnerText(XP_PANEL_BODY_X + "[1]" + XPR_CURRENT_INSURER + XPEXT_DROPDOWN_VALUE);
            set => WaitForSelectableAndPickByTyping(XP_PANEL_BODY_X + "[1]" + XPR_CURRENT_INSURER, value+Keys.ArrowDown);
        }

        public string Financier
        {
            get => GetInnerText(XP_PANEL_BODY_X + "[1]" + XPR_FINANCIER + XPEXT_DROPDOWN_VALUE);
            set => WaitForSelectableAndPickByTyping(XP_PANEL_BODY_X + "[1]" + XPR_FINANCIER, value);
        }
        #endregion

        public MotorQuote3Policy(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_PAGE_HEADING);
                if (!heading.Text.ToLower().StartsWith("your quote:"))
                {
                    Reporting.Log("Wrong heading text for second page of Motor Quote process.");
                    return false;
                }
                GetElement(XP_VEHICLE_HEADING);
                // Vehicle panel is first accordion
                string offset = "[1]";
                GetElement(XP_PANEL_BODY_X + offset + XPR_REGO_DONT_KNOW);
                GetElement(XP_PANEL_BODY_X + offset + XPR_CURRENT_INSURER);
                GetElement(XP_PANEL_BODY_X + offset + XPR_CONTINUE_BTN);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Motor Quote page 3 - additional vehicle and policyholder details");
            return true;
        }

        /// <summary>
        /// Returns whether the information text box is visible in the browser which informs the user
        /// that they need to provide their registration info. Is present if the user declares that they
        /// don't know their vehicle rego.
        /// </summary>
        /// <returns></returns>
        public bool IsRegistrationNotificationVisible()
        {
            try
            {
                // Vehicle details is first accordion (0-based index)
                _driver.WaitForElement(By.XPath(XP_PANEL_BODY_X + "[1]" + XPR_REGO_NOTIFICATION), WaitTimes.T30SEC);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Click to toggle the checkbox for user not knowing their vehicle rego at time of policy
        /// purchase.
        /// </summary>
        /// <param name="isChecked">Desired state of the checkbox</param>
        public void ClickDontKnowRego(bool isChecked)
        {
            var checkBoxXpath = $"{XP_PANEL_BODY_X}[1]{XPR_REGO_DONT_KNOW}";
            var statusXPath = $"{checkBoxXpath}//i";

            var checkboxStatus = !GetElement(statusXPath).GetAttribute("style")?.Contains("display: none");
            if (checkboxStatus != isChecked)
            {
                // Vehicle details is first accordion (1-based index)
                ClickControl($"{XP_PANEL_BODY_X}[1]{XPR_REGO_DONT_KNOW}");
            }
        }

        /// <summary>
        /// Helper method to take vehicle quote information and populate all relevant fields on this
        /// particular motor quote page.
        /// </summary>
        /// <param name="vehicle"></param>
        public bool FillInAddedVehicleDetails(QuoteCar vehicle)
        {
            Reporting.Log($"After Retrieve Quote \"Don't Know\" is checked by default for Car registration number, so we un-check it to get back to baseline.");
            var success = true;
            IsCarModified = vehicle.IsModified;

            VehicleRegistration = vehicle.Registration;
            if (string.IsNullOrEmpty(vehicle.Registration))
            {
                success = IsRegistrationNotificationVisible() && VehicleRegistration.Equals("To be advised");
            }

            CurrentRecentInsurer = vehicle.CurrentInsurer;

            if (vehicle.IsFinanced)
            {
                Financier = vehicle.Financier;
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">0-based index for which driver has details provided.</param>
        /// <param name="inputDrivers">List of all drivers from test data. Code will find the first driver to match the displayed DoB and use that to enter details.</param>
        /// <param name="vehicleParkingAddress">This is the address where the vehicle is parked. Used in relation to main driver questions.</param>
        public void FillInDriverDetails(int index, List<Driver> inputDrivers, Address vehicleParkingAddress, Browser browser)
        {
            var shownDoB = DateTime.ParseExact(GetInnerText(string.Format(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_DOB, index))),DataFormats.DATE_ABBREVIATED_MONTH_FORWARD_WHITESPACE, CultureInfo.InvariantCulture);
            var details = inputDrivers.FirstOrDefault(x => x.Details.DateOfBirth.Date == shownDoB.Date);
            if (details == null) 
            { Reporting.Error("Motor quote page 3 showing date of birth that does not match any expected inputs."); }
            inputDrivers.Remove(details);

            WaitForSelectableAndPickByTyping(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_TITLE, index), details.Details.Title.GetDescription());
            WaitForTextFieldAndEnterText(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_FIRSTNAME, index), details.Details.FirstName);
            if (!string.IsNullOrEmpty(details.Details.MiddleName))
                WaitForTextFieldAndEnterText(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_MIDDLENAME, index), details.Details.MiddleName);
            WaitForTextFieldAndEnterText(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_SURNAME, index), details.Details.Surname);

            // Set whether driver will be a policy holder, and set email
            var element = details.IsPolicyHolderDriver ? XPR_POLICYHOLDER_YES : XPR_POLICYHOLDER_NO;
            ClickControl(string.Format(XP_PANEL_BODY_X + element, index));

            if (details.IsPolicyHolderDriver)
            {
                // Added an explicit and easily configurable wait for the email text field to be displayed
                // as the 5 seconds allowed for in WaitForTextFieldAndEnterText has been insufficient
                if (_driver.TryWaitForElementToBeVisible(By.XPath(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_EMAIL, index)), WaitTimes.T10SEC, out IWebElement emailAddressField))
                {
                    Reporting.Log($"Finished waiting for email field to appear after saying 'Yes' to being a policyholder.", _browser.Driver.TakeSnapshot());
                    // Enter email value and set policy delivery method to 'Email'
                    WaitForTextFieldAndEnterText(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_EMAIL, index), details.Details.GetEmail());
                }
                if (!_haveSetFirstPolicyHolder)
                {
                    ClickBinaryToggle(string.Format(XPR_DRIVER_PDM_TOGGLE, index), XPR_EMAIL, XPR_POST, details.Details.IsEmailPreferredDeliveryMethod());
                    _haveSetFirstPolicyHolder = true;
                }
            }
            Reporting.Log($"index = {index}");
            Reporting.Log($"testConfig.IsMotorRiskAddressEnabled() = {Config.Get().IsMotorRiskAddressEnabled()}");
            bool doQASAddressSearch = false;
            /* Answering YES/NO toggle for if address is the same */
            if (index == 0)
            {
                var isDriverAddressSameAsVehicle = vehicleParkingAddress.StreetSuburbPostcode(expandUnitAddresses: true).Equals(details.Details.MailingAddress.StreetSuburbPostcode(expandUnitAddresses: true));
                // TODO: B2C-4561 Remove MotorRiskAddress Toggle when removing toggle from B2C/PCM Functional code leaving !haveRetrievedQuote.
                // Main driver is asked if addess is the same as vehicle address only if not a Retrieved Quote.
                if (Config.Get().IsMotorRiskAddressEnabled())
                {
                    ClickBinaryToggle($"{XPR_IS_ADDRESS_THE_SAME_YN}", XPR_YES, XPR_NO, isDriverAddressSameAsVehicle);
                    Reporting.Log($"Answered isDriverAddressSameAsVehicle = {isDriverAddressSameAsVehicle}", _browser.Driver.TakeSnapshot());
                }
                // TODO: B2C-4561 Remove MotorRiskAddress Toggle when removing toggle from B2C/PCM Functional code
                if (!isDriverAddressSameAsVehicle ||
                    !Config.Get().IsMotorRiskAddressEnabled())
                {
                    Reporting.Log($"doQASAddressSearch = {doQASAddressSearch}");
                    doQASAddressSearch = true;
                }
            }
            else
            {
                // For all additional drivers, indicate if they live at same location as main driver.
                element = details.Details.MailingAddress == null ? XPR_ADDRESS_SAME_YES : XPR_ADDRESS_SAME_NO;
                ClickControl(string.Format(XP_PANEL_BODY_X + element, index));

                doQASAddressSearch = details.Details.MailingAddress != null;
            }

            if (doQASAddressSearch)
            {
                QASSearchForAddress(string.Format(XP_PANEL_BODY_X + XPR_MAILINGADDRESS, index),
                                    string.Format($"{XP_PANEL_BODY_X}{XPR_ADDR_SUGGESTION_X}", index),
                                    details.Details.MailingAddress.StreetSuburbState());
            }

            WaitForTextFieldAndEnterText(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_PHONE, index), details.Details.GetPhone());
            Reporting.Log($"Fill In Driver Details complete for driver #{index + 1}", browser.Driver.TakeSnapshot());
        }

        public string GetMainDriverEmail()
        {
            // Accordion for main drivers panel starts at index 2 (if there is no membership panel)
            string offset_panel = "[2]";
            string email = null;

            var panel = GetElement(XP_PANEL_BODY_X + offset_panel);
            if (panel.GetAttribute("class").Contains("opened"))
            {
                email = GetInnerText(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_EMAIL, 0));
            }
            else
            {
                Reporting.Log("Cannot retrieve email as main driver details not visible.");
            }
            return email;
        }

        public void ClickCarDetailsContinueButton()
        {
            ClickControl($"{XP_PANEL_BODY_X}[1]{XPR_CONTINUE_BTN}");

            // Animation from Car Details panel to Membership/Driver Details is hard to detect. Using fixed sleep.
            Thread.Sleep(2000);
        }

        public void ClickRACMembershipContinueButton()
        {
            ClickControl($"{XP_PANEL_BODY_X}[2]{XPR_CONTINUE_BTN}");

            // Animation from Membership Details panel to Driver Details is hard to detect. Using fixed sleep.
            Thread.Sleep(2000);
        }

        public void ClickDriverContinueButton(int index)
        {
            // Determining accordion offsets.
            // Offset=2 if only vehicle details and driver details
            // Offset=3 if RAC membership is also declared.
            string offset = $"[{index + 2}]";
            ClickControl($"{XP_PANEL_BODY_X}{offset}{XPR_CONTINUE_BTN}");

            // Animation from Car Details panel to Driver Details is hard to detect. Using fixed sleep.
            Thread.Sleep(2000);
        }

        public void WaitForDriverDetails(int index)
        {
            // Accordion for drivers heading start at index 1 (as they are unique from vehicle/membership headings)
            var offset_heading = string.Format("[{0}]", index + 1);

            // Accordion for drivers panel   start at index 2 (as first index, 1, is vehicle details panel)
            string offset_panel = $"[{index + 2}]";

            var loaded = false;
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
            do
            {
                try
                {
                    var heading = GetElement(XP_DRIVER_X_HEADING + offset_heading);
                    var panel   = GetElement(XP_PANEL_BODY_X + offset_panel);
                    if (heading.GetAttribute("class").Contains("opened") &&
                        panel.GetAttribute("class").Contains("opened"))
                    {
                        loaded = true;
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
                catch (NoSuchElementException)
                {
                    Reporting.Log("Element not found when waiting for driver details.");
                    loaded = false;
                }
            } while (loaded == false && DateTime.Now < endTime);

            if (!loaded)
            { Reporting.Error("Driver Policy Details (" + index + ") accordian did not open in time."); }
        }

        /// <summary>
        /// Performs verifications on prepopulated values to support PPQ.
        /// These validations are not valid for anon motor quotes.
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public bool VerifyPPQPrePopulatedMainDriverDetails(Driver details)
        {
            var index = 0;  // Main driver only
            var success = true;

            if (!AssertReadOnlyFieldText(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_TITLE, index),
                                     "aria-disabled",
                                     details.Details.Title.GetDescription(),
                                     "Driver's title"))
                return false;

            if (!AssertReadOnlyFieldValue(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_FIRSTNAME, index),
                                     "readonly",
                                     details.Details.FirstName,
                                     "Driver's first name"))
                return false;

            if (!AssertReadOnlyFieldValue(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_SURNAME, index),
                                     "readonly",
                                     details.Details.Surname,
                                     "Driver's last name"))
                return false;

            /* TODO: determine resolution for handling Benang having a different email than Shield
             * if (!assertReadOnlyFieldValue(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_EMAIL, index),
             *                         "readonly",
             *                         details.Details.GetEmail(),
             *                         "Driver's email"))
             *    return false;
             */

            if (!AssertReadOnlyFieldValue(string.Format(XP_PANEL_BODY_X + XPR_DRIVER_PHONE, index),
                                     "readonly",
                                     details.Details.GetPhone(),
                                     "Driver's phone number"))
                return false;

            var field = GetElement(string.Format(XP_PANEL_BODY_X + XPR_POLICYHOLDER_YES + "/input", index));
            if (field.GetAttribute("readonly") != "true" || field.GetAttribute("checked") != "true")
                return false;

            // Expect main driver address to be pre-populated if Motor Risk Address not enabled,
            if (!Config.Get().IsMotorRiskAddressEnabled())
            {
                // Does not use assertReadOnlyFieldText() as this uses "contains" instead of equals.
                field = GetElement(string.Format(XP_PANEL_BODY_X + XPR_MAILADDR_COMPLETE, index));
                if (!details.Details.MailingAddress.IsEqualToString(field.Text.ToLower()))
                    return false;
            }
            else
            {
                // Where Motor Risk Address is enabled, set to true (for PPQ).
                ClickBinaryToggle($"{XPR_IS_ADDRESS_THE_SAME_YN}", XPR_YES, XPR_NO, true);
                Reporting.Log($"Answered isDriverAddressSameAsVehicle = true", _browser.Driver.TakeSnapshot());
            }

            return success;
        }

        private bool AssertReadOnlyFieldValue(string xPath, string readOnlyAttribute, string expectedValue, string fieldName)
        {
            var field = GetElement(xPath);
            if (field.GetAttribute(readOnlyAttribute) != "true")
            {
                Reporting.Log($"Failed validation of {fieldName}, did not have expected read-only attribute.");
                return false;
            }
            if (!field.GetAttribute("value").ToLower().Equals(expectedValue.ToLower()))
            {
                Reporting.Log($"Failed validation of {fieldName}, did not have expected value; {expectedValue}");
                return false;
            }
            return true;
        }

        private bool AssertReadOnlyFieldText(string xPath, string readOnlyAttribute, string expectedValue, string fieldName)
        {
            var field = GetElement(xPath);
            if (!string.IsNullOrEmpty(readOnlyAttribute) && field.GetAttribute(readOnlyAttribute) != "true")
            {
                Reporting.Log($"Failed validation of {fieldName}, did not have expected read-only attribute.");
                return false;
            }
            if (!field.Text.ToLower().Equals(expectedValue.ToLower()))
            {
                Reporting.Log($"Failed validation of {fieldName}, did not have expected value; {expectedValue}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Supports checking for the UI error notification screen.
        /// Expects that the calling test has triggered the error
        /// notification for all 3 name input fields.
        /// 
        /// Checks all 3 fields before setting assert result.
        /// </summary>
        public void VerifyNameFieldLengthValidationErrors()
        {
            var         success = true;
            IWebElement element = null;

            Reporting.Log("Checking for validation messages on Name fields");
            if (!_driver.TryWaitForElementToBeVisible(By.XPath("//*[contains(text(),'First name is too long. (Maximum is 45 characters).')]"), WaitTimes.T5SEC, out element))
            {
                Reporting.Log("Did not find expected length validation error message on First Name");
                success = false;
            }
            if (!_driver.TryWaitForElementToBeVisible(By.XPath("//*[contains(text(),'Middle name is too long. (Maximum is 50 characters).')]"), WaitTimes.T5SEC, out element))
            {
                Reporting.Log("Did not find expected length validation error message on Middle Name");
                success = false;
            }
            if (!_driver.TryWaitForElementToBeVisible(By.XPath("//*[contains(text(),'Last name is too long. (Maximum is 50 characters).')]"), WaitTimes.T5SEC, out element))
            {
                Reporting.Log("Did not find expected length validation error message on Last Name");
                success = false;
            }

            Reporting.IsTrue(success, $"VerifyNameFieldLengthValidationErrors should display error on all 3 name input fields for contact, taking snapshot. {_driver.TakeSnapshot()}");
        }
    }
}
