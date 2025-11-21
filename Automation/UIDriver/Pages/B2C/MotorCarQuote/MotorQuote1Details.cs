using Rac.TestAutomation.Common;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using System.Linq;

namespace UIDriver.Pages.B2C
{
    public class MotorQuote1Details : BasePage
    {
        // Max past claims allowed (given longest previous insurance history)
        private const int MAXIMUM_CLAIMS_HISTORY = 5;
        private const string DECLINE_NOTICE_HEADER_TEXT = "Cover declined";

        #region XPATHS
        // Segments
        private const string BASE            = "/html/body/div[@id='wrapper']";
        private const string FORM            = BASE + "//form[@class='insuranceForm questionForm formTop']";
        private const string SEC_VEHICLE     = FORM + "/div[@class='row accordion']";
        private const string SEC_DRIVER_MAIN = FORM + "//div[@id='Question_Driver_Contact_0__row']";
        private const string SEC_DRIVER_ADD1 = FORM + "//div[@id='Question_Driver_Contact_1__row']";
        private const string SEC_DRIVER_ADD2 = FORM + "//div[@id='Question_Driver_Contact_2__row']";
        private const string HEADER          = "/div[starts-with(@class,'accordion-heading')]";
        private const string PANEL           = "/div[starts-with(@class,'accordion-panel')]";

        // General
        private const string XP_PAGE_HEADING        = BASE + "//span[@class='action-heading']/span";
        private const string XP_HEADING_VEHICLE     = SEC_VEHICLE + HEADER;

        private const string XPR_YES                = "//span[text()='Yes']/..";
        private const string XPR_NO                 = "//span[text()='No']/..";

        // Vehicle details
        private const string PANEL_VEHICLE        = SEC_VEHICLE + PANEL;
        private const string SUBPANEL_CAR_SEARCH  = SEC_VEHICLE + PANEL + "//div[@data-search-type='ByMakeModel']";
        private const string XP_REGO_TEXT         = PANEL_VEHICLE + "//input[@id='Question_Vehicle_Vehicle_RegistrationNumber']";
        private const string XP_REGO_FIND_BUTTON  = PANEL_VEHICLE + "//a[contains(@class,'find-button')]";
        private const string XP_CAR_SEARCH_BUTTON = PANEL_VEHICLE + "//a[contains(@class,'search-button')]";
        private const string XP_CAR_SELECTED      = PANEL_VEHICLE + "//div[@class='selectedVehicleDescription']";

        private const string XP_CAR_MAKE_TEXT     = SUBPANEL_CAR_SEARCH + "//input[@name='Question.Vehicle.Vehicle.Make_input']";
        private const string XP_CAR_MAKE_DROPDOWN = "id('Question_Vehicle_Vehicle_Make_listbox')";
        private const string XP_CAR_YEAR_TEXT     = SUBPANEL_CAR_SEARCH + "//span[@aria-owns='Question_Vehicle_Vehicle_Year_listbox']";
        private const string XP_CAR_YEAR_DROPDOWN = "id('Question_Vehicle_Vehicle_Year_listbox')/li";
        private const string XP_CAR_MODEL_TEXT    = SUBPANEL_CAR_SEARCH + "//span[@aria-owns='Question_Vehicle_Vehicle_Model_listbox']";
        private const string XP_CAR_BODY_TEXT     = SUBPANEL_CAR_SEARCH + "//span[@aria-owns='Question_Vehicle_Vehicle_BodyType_listbox']";
        private const string XP_CAR_TRANS_TEXT    = SUBPANEL_CAR_SEARCH + "//span[@aria-owns='Question_Vehicle_Vehicle_TransmissionText_listbox']";
        private const string XP_CAR_SEARCH_CANCEL = SUBPANEL_CAR_SEARCH + "//a[@class='link cancel-change-car']";
        private const string XP_CAR_RESULTS_TABLE = SUBPANEL_CAR_SEARCH + "//table[@class='vehicle-description-table']";
        private const string XP_CAR_SEARCH_RESULT_X  = SUBPANEL_CAR_SEARCH + "//table[@class='vehicle-description-table']/tbody/tr";

        private const string XP_REGO_SEARCH_RESULT = SEC_VEHICLE + "//div[@id='VehicleContainer']//div[@class='selectedVehicleDescription']";

        private const string XP_CAR_USAGE          = PANEL_VEHICLE + "//span[starts-with(@aria-owns,'Question_Vehicle_VehicleUsage')]";
        private const string XP_CAR_USAGE_OPTIONS  = "id('Question_Vehicle_VehicleUsage_listbox')/li";
        private const string XP_INCOME_FROM_CAR_YN = "id('Question_Vehicle_IsIncomeDerived')";
        private const string XP_CAR_KM             = PANEL_VEHICLE + "//span[starts-with(@aria-owns,'Question_Vehicle_KmPerYear')]";
        private const string XP_CAR_SUBURB         = "id('Question_Vehicle_Suburb_Suburb')";
        private const string XP_STREETADDRESS      = "id('Question_Vehicle_RiskLocation_qasautocomplete')";
        private const string XP_ADDR_SUGGESTION    = "//div[@id='Question_Vehicle_RiskLocation']//table[@class='address-find-table']//tr/td[1]";
        private const string XP_ADDR_SELECTED      = "//div[@class='selectedAddressDescription']//div[contains(@class,'selectedAddressDescription-text')]";
        private const string XP_IS_FINANCED_YN     = "id('Question_Vehicle_IsFinanced')";

        // Driver Details (prefix with required SEC_DRIVER_xxx)
        private const string GENDER_FEMALE   = "//div[contains(@id,'_Female_Label') or contains(@id,'_1_Label')]";
        private const string GENDER_MALE     = "//div[contains(@id,'_Male_Label') or contains(@id,'_2_Label')]";
        private const string DOB_DAY         = "//input[contains(@id,'_Dob_Day')]";
        private const string DOB_MONTH       = "//input[contains(@id,'_Dob_Month')]";
        private const string DOB_YEAR        = "//input[contains(@id,'_Dob_Year')]";
        private const string MAIN_DRIVER_LICENSE_YEARS   = "//span[@aria-owns='Question_Driver_Contact_0__NumberOfYearsLicensed_listbox']";
        private const string ADDITIONAL_DRIVER1_LICENSE_YEARS = "//span[@aria-owns='Question_Driver_Contact_1__NumberOfYearsLicensed_listbox']";
        private const string ADDITIONAL_DRIVER2_LICENSE_YEARS = "//span[@aria-owns='Question_Driver_Contact_2__NumberOfYearsLicensed_listbox']";
        private const string MEMBERSHIP_LEVEL= "//span[starts-with(@aria-owns,'Question_Driver_Contact_0__MembershipLevel')]";     // only relevant for main driver
        private const string EMAIL_ADDRESS   = "//input[@id='Question_Driver_Contact_0__EmailAddress']";                        // only relevant for main driver
        private const string MEMBER_YN       = "//div[@id='Question_Driver_Contact_0__IsMember']";  // only relevant for main driver
        private const string ADD_DRIVER_BTN  = "//div[starts-with(@class,'addPolicyHolder')]";
        private const string CONTINUE_QUOTE_BTN    = "//button[contains(@class,'accordion-button')]";

        // Driver Declaration (prefix with required SEC_DRIVER_xxx)
        private const string LIC_DISQUAL_YES = "//div[contains(@id,'_HadLicenseDisqualified_True_Label')]";
        private const string LIC_DISQUAL_NO  = "//div[contains(@id,'_HadLicenseDisqualified_False_Label')]";
        private const string DISQUAL_ROOT    = "//div[@id='trafficConvictionsContainer']";

        private const string PAST_CRASHES_YES = "//div[contains(@id,'_HadAnyAccidents_True_Label')]";
        private const string PAST_CRASHES_NO  = "//div[contains(@id,'_HadAnyAccidents_False_Label')]";
        private const string CRASH_ROOT       = "//div[@id='accidentClaimContainer']";

        // Cover Declined dialog
        private const string XP_DECLINED_DISCLOSURES_HEADER = "//*[@id='simple-dialog_wnd_title']";
        private const string XP_DECLINED_VEHICLE_USE_HEADER = "//*[@id='knockout-dialog_wnd_title']";
        private const string XP_DECLINED_NOTICE_TEXT        = "//div[contains(@class,'k-window-content k-content')]";
        private const string XP_DECLINED_NOTICE_DISMISS     = "//div[@class='cluetip-close']";
        #endregion

        #region Settable properties and controls
        public VehicleUsage Usage
        {
            get
            {
                var dropdownText = GetInnerText($"{XP_CAR_USAGE}{XPEXT_DROPDOWN_VALUE}");
                Reporting.IsTrue(VehicleUsageNameMappings.Any(x => x.Value.TextB2C.Equals(dropdownText)), $"that text in dropdown was recognised; {dropdownText}");
                
                return VehicleUsageNameMappings.First(x => x.Value.TextB2C.Equals(dropdownText)).Key;
            }
            set => WaitForSelectableAndPickFromDropdown(XP_CAR_USAGE, XP_CAR_USAGE_OPTIONS, VehicleUsageNameMappings[value].TextB2C);
        }

        public bool IncomeDirectFromCar
        {
            get => GetBinaryToggleState(XP_INCOME_FROM_CAR_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_INCOME_FROM_CAR_YN, XPR_YES, XPR_NO, value);
        }

        public AnnualKms AnnualKm
        {
            get => DataHelper.GetValueFromDescription<AnnualKms>(GetInnerText($"{XP_CAR_KM}{XPEXT_DROPDOWN_VALUE}"));
            set => WaitForSelectableAndPickByTyping(XP_CAR_KM, value.GetDescription());
        }

        /// <summary>
        /// TODO: (B2C-4561) Remove this method when removing Motor Risk
        /// Address toggles.
        /// </summary>
        public string ParkedSuburb
        {
            get => GetValue(XP_CAR_SUBURB);
            set => WaitForTextFieldAndEnterText(XP_CAR_SUBURB, value);
        }

        /// <summary>
        /// Only applicable for where Motor Risk Address is toggled on.
        /// If using GET, then empty string indicates we're yet to select an QAS address
        /// </summary>
        public string RiskAddress
        {
            get
            {
                // If QAS search field is shown, return empty string.
                if (_driver.TryWaitForElementToBeVisible(By.XPath(XP_STREETADDRESS), Constants.General.WaitTimes.T5SEC, out IWebElement searchTextField))
                {
                    return string.Empty;
                }
                // If we don't see the QAS search field, then we expect to see a selected address.
                if (!_driver.TryWaitForElementToBeVisible(By.XPath(XP_ADDR_SELECTED), Constants.General.WaitTimes.T5SEC, out IWebElement selectedAddressField))
                { Reporting.Error("Attempt to fetch risk address status, but expected field(s) not found."); }

                return selectedAddressField.Text;
            }
            set
            {
                QASSearchForAddress(XP_STREETADDRESS, XP_ADDR_SUGGESTION, value);
                Reporting.Log($"Address {value} provided.", _browser.Driver.TakeSnapshot());
            }
        }

        public bool CarFinance
        {
            get => GetBinaryToggleState(XP_IS_FINANCED_YN, XPR_YES, XPR_NO);
            set => ClickBinaryToggle(XP_IS_FINANCED_YN, XPR_YES, XPR_NO, value);
        }
        #endregion

        public MotorQuote1Details(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            try
            {
                var heading = GetElement(XP_PAGE_HEADING);
                if (!heading.Text.ToLower().Equals("car quote"))
                {
                    Reporting.Log("Wrong heading text for Car Quote page.");
                    return false;
                }
                GetElement(XP_CAR_USAGE);
                GetElement(XP_CAR_KM);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Motor Quote page 1 - Rating values");
            return true;
        }

        public void KnockBackMessage()
        {
            var declinedNoticeHead = GetElement(XP_DECLINED_DISCLOSURES_HEADER);
            var declinedNoticeText = GetElement(XP_DECLINED_NOTICE_TEXT);
            var declinedNoticeDismiss = GetElement(XP_DECLINED_NOTICE_DISMISS);
        }

        public void WaitForCarDetailsToDisplay()
        {
            var loaded = false;
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
            do
            {
                try
                {
                    var heading = GetElement(XP_HEADING_VEHICLE);
                    var carSection = GetElement(PANEL_VEHICLE);
                    if (heading.GetAttribute("class").Contains("opened") &&
                        carSection.GetAttribute("class").Contains("opened") &&
                        carSection.Displayed)
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
                    loaded = false;
                }
            } while (loaded == false && DateTime.Now < endTime);
        }

        public void WaitForDriverDetailsToDisplay(int driverIndex)
        {
            By xp_heading, xp_panel;

            switch(driverIndex)
            {
                case 1:
                    xp_heading = By.XPath(SEC_DRIVER_MAIN + HEADER);
                    xp_panel   = By.XPath(SEC_DRIVER_MAIN + PANEL);
                    break;
                case 2:
                    xp_heading = By.XPath(SEC_DRIVER_ADD1 + HEADER);
                    xp_panel   = By.XPath(SEC_DRIVER_ADD1 + PANEL);
                    break;
                case 3:
                    xp_heading = By.XPath(SEC_DRIVER_ADD2 + HEADER);
                    xp_panel   = By.XPath(SEC_DRIVER_ADD2 + PANEL);
                    break;
                default:
                    Reporting.Error(string.Format("Driver's are 1-based indexed. 1='main', 2='1 additional', 3='2 additional'. The value of {0} is unsupported.", driverIndex));
                    return;
            }

            var loaded = false;
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
            do
            {
                try
                {
                    var heading = _driver.FindElement(xp_heading);
                    var panel = _driver.FindElement(xp_panel);
                    if (heading.GetAttribute("class").Contains("opened") &&
                        panel.GetAttribute("class").Contains("opened") &&
                        panel.Displayed)
                    {
                        loaded = true;
                    }
                    else
                    {
                        Thread.Sleep(500);
                    }
                }
                catch
                {
                    loaded = false;
                }
            } while (loaded == false && DateTime.Now < endTime);
            if (!loaded)
            { Reporting.Error("Driver Details (" + driverIndex + ") accordian did not open in time."); }
        }

        public void SearchForCar(Car vehicle)
        {
            var collapsibleSearchSection = GetElement(SUBPANEL_CAR_SEARCH);
            var style = collapsibleSearchSection.GetAttribute("style");
            if (style.Contains("display: none"))
            {
                ClickControl(XP_CAR_SEARCH_BUTTON);
            }

            // Search for manufacturer. This search is non-standard as Shield can
            // respond quote slow the for the first vehicle search, and alot of
            // relevant data may no longer be in the API cache. Hence UI dropdowns
            // can lag significantly waiting for data.
            SendKeyPressesToField(XP_CAR_MAKE_TEXT, vehicle.Make);
            Thread.Sleep(2000);
            ClickControl($"{XP_CAR_MAKE_DROPDOWN}/li[text()='{vehicle.Make}']",
                         waitTimeSeconds: WaitTimes.T30SEC, skipJSScrollLogic: true);
            WaitForMakeDropDownToClose();

            // Search for year, model, body and transmission. These dropdown fields
            // don't suffer from the lag that Make does.
            WaitForSelectableAndPickFromDropdown(XP_CAR_YEAR_TEXT, XP_CAR_YEAR_DROPDOWN, vehicle.Year.ToString());
            WaitForSelectableAndPickByTyping(XP_CAR_MODEL_TEXT, vehicle.Model);
            WaitForSelectableAndPickByTyping(XP_CAR_BODY_TEXT,  vehicle.Body);
            WaitForSelectableAndPickByTyping(XP_CAR_TRANS_TEXT, vehicle.Transmission);
        }

        private void SearchForRego(Vehicles vehicle)
        {
            WaitForTextFieldAndEnterText(XP_REGO_TEXT, vehicle.Registration);

            ClickControl(XP_CAR_SEARCH_BUTTON);

            _driver.WaitForElementToBeVisible(By.XPath(XP_REGO_SEARCH_RESULT), WaitTimes.T60SEC);
        }

        /// <summary>
        /// Because there can be many variants based on the search criteria, make
        /// a choice based on vehicle ID (if we don't have a vehicle ID, we'll
        /// choose first result from list).
        /// </summary>
        /// <returns></returns>
        private string PickCarSearchResult(string vehicleId)
        {
            var endTime = DateTime.Now.AddSeconds(5);
            var success = false;
            var carText = string.Empty;
            do
            {
                try
                {
                    var xpathChosenResult = string.IsNullOrEmpty(vehicleId) ?
                        $"{XP_CAR_SEARCH_RESULT_X}[1]/td" :
                        $"{XP_CAR_SEARCH_RESULT_X}[@data-id='{vehicleId}']/td";
                    ClickControl(xpathChosenResult, hasFailOver: true);
                    success = true;
                }
                catch
                {
                    // It's possible that there was only a single match which B2C just
                    // picks automatically.
                    success = _driver.TryFindElement(By.XPath(XP_CAR_SELECTED), out IWebElement element);
                    carText = success ? element.Text.Trim() : string.Empty;
                }
            } while (!success && DateTime.Now < endTime);
            if (success)
            {
                var selectedVehicle = _driver.WaitForElement(By.XPath(XP_CAR_SELECTED), WaitTimes.T30SEC);
                carText = selectedVehicle.Text.Trim();
            }
            else
                Reporting.Error("Could not click desired row, either not found, or did not become visible in time.");
            Reporting.Log($"Selected vehicle: is '{carText}'", _driver.TakeSnapshot());
            return carText;
        }

        /// <summary>
        /// Fills the first accordion of page 1 of Motor Quote.
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <param name="isPPQ">Is this test a Pre-Populated Quote test?</param>
        /// <returns>Text of vehicle matched from rego/search</returns>
        public string FillQuoteDetailsFirstAccordion(QuoteCar quoteDetails, bool isPPQ = false)
        {
            var matchedVehicle = string.Empty;
            if (quoteDetails.Make != null)
            {
                SearchForCar(quoteDetails);
                matchedVehicle = PickCarSearchResult(quoteDetails.VehicleId);
            }
            else
            {
                SearchForRego(quoteDetails);
            }

            Usage = quoteDetails.UsageType;

            // We get an additional question when usage is "Business"
            // Always answer no to allow quote to proceed.
            if (quoteDetails.UsageType == VehicleUsage.Business)
            {
                IncomeDirectFromCar = false;
            }

            AnnualKm = quoteDetails.AnnualKm;
            if (Config.Get().IsMotorRiskAddressEnabled()) // TODO: B2C-4561 Remove toggle and old Risk Suburb references as appropriate when removing toggle from B2C/PCM Functional code
            {
                // If PPQ, this will be populated, but if not then we'll need to fill this field.
                if (!isPPQ)
                {
                    RiskAddress = quoteDetails.ParkingAddress.StreetSuburbState();
                }
            }
            else
            {
                ParkedSuburb = quoteDetails.ParkingAddress.SuburbAndCode();
            }
            CarFinance = quoteDetails.IsFinanced;
            Reporting.Log("Completed vehicle rating details accordion", _driver.TakeSnapshot());
            ClickContinueFromCarDetails();
            if (quoteDetails.VehicleUsageDeclineThenDismiss)
            {
                MotorWaitForVehicleUsageDeclinedCoverNoticeAndDismiss(quoteDetails);
            }
            return matchedVehicle;
        }

        /// <summary>
        /// Fills the second accordion of page 1 of Motor Quote.
        /// Will add additional drivers as defined.
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <param name="submit">Should we attempt to navigate to Page 2 after filling information?</param>
        /// <param name="isPPQ">Is this test a Pre-Populated Quote test?</param>
        /// <param name="isModifyingExistingDrivers">If true will skip attempts to ClickAddDriver, but will use the Continue button to navigate to subsequent drivers as they already exist.</param>
        public void FillQuoteDetailsSecondAccordion(QuoteCar quoteDetails, 
                                                             bool submit = true, 
                                                             bool isPPQ = false, 
                                                             bool isModifyingExistingDrivers = false)
        {
            int i = 0;
            for (; i < quoteDetails.Drivers.Count; i++)
            {
                Reporting.Log($"Populating driver details for driver index reference {i}. See 'Customer data' & 'Vehicle quote data' for expected details.");
                if (i > 0)
                {
                    if (isModifyingExistingDrivers)
                    {
                        // We're clicking continue from previous driver.
                        // NOTE: this method treats 'i' as 1 based.
                        ClickContinueFromCurrentDriver(i);
                    }
                    else
                    {
                        ClickAddDriver(i);
                    }
                }
                // i+1 for index, as it is 1-based.
                if (i == 0 && isPPQ)
                    SetPPQMainDriverDetails(quoteDetails.Drivers[i]);
                else
                    SetDriverDetails(i + 1, quoteDetails.Drivers[i]);
            }
            if (submit)
                ClickContinueFromCurrentDriver(i);
        }

        /// <summary>
        /// Wrapper around FillQuoteDetails..() methods for first
        /// and second accordions.
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <param name="submit"></param>
        /// <returns></returns>
        public string FillQuoteDetails(QuoteCar quoteDetails, bool submit = true)
        {
            var matchedVehicle = FillQuoteDetailsFirstAccordion(quoteDetails);
            Thread.Sleep(1000);
            FillQuoteDetailsSecondAccordion(quoteDetails, submit);
            return matchedVehicle;
        }

        /// <summary>
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <returns></returns>
        public bool VerifyMainDriverPrefilledFields(QuoteCar quoteDetails)
        {
            var errorMessage = "";

            try
            {
                string xp_base = GetDriverPanelXPathBaseFromIndex(1); // 1-based index.

                errorMessage = "Gender";
                IWebElement genderCheckbox = null;
                if (quoteDetails.Drivers[0].Details.Gender == Gender.Male)
                    genderCheckbox = GetElement(xp_base + GENDER_MALE + "/input");
                else
                    genderCheckbox = GetElement(xp_base + GENDER_FEMALE + "/input");

                if (genderCheckbox.GetAttribute("checked") != "true" ||
                    genderCheckbox.GetAttribute("readonly") != "true")
                {
                    Reporting.Log($"The expected {errorMessage} field did not have the correct value or was not readonly.");
                    return false;
                }

                errorMessage = "RAC Membership";
                IWebElement membershipCheckbox = null;
                if (quoteDetails.Drivers[0].Details.IsRACMember)
                {
                    membershipCheckbox = GetElement($"{xp_base}{MEMBER_YN}{XPR_YES}/input");
                    if (membershipCheckbox.GetAttribute("checked") != "true" ||
                        membershipCheckbox.GetAttribute("readonly") != "true")
                    {
                        Reporting.Log($"The expected {errorMessage} field did not have the correct value or was not readonly.");
                        return false;
                    }

                    var membershipLevel = GetElement(xp_base + MEMBERSHIP_LEVEL);
                    if (membershipLevel.GetAttribute("aria-disabled") != "true" ||
                        !membershipLevel.Text.Equals(MembershipTierText[quoteDetails.Drivers[0].Details.MembershipTier]))
                    {
                        Reporting.Log($"The expected {errorMessage} field did not have the correct value or was not readonly.");
                        return false;
                    }
                }
                else
                {
                    membershipCheckbox = GetElement($"{xp_base}{MEMBER_YN}{XPR_NO}/input");
                    if (!string.IsNullOrEmpty(membershipCheckbox.GetAttribute("checked")) ||
                        !string.IsNullOrEmpty(membershipCheckbox.GetAttribute("readonly")))
                    {
                        Reporting.Log($"The expected {errorMessage} field did not have the correct value or was not readonly.");
                        return false;
                    }
                }

                errorMessage = "Date of Birth";
                IWebElement dobDay   = GetElement(xp_base + DOB_DAY);
                IWebElement dobMonth = GetElement(xp_base + DOB_MONTH);
                IWebElement dobYear  = GetElement(xp_base + DOB_YEAR);
                if (dobDay.GetAttribute("readonly") != "true" ||
                    dobDay.GetAttribute("value") != quoteDetails.Drivers[0].Details.DateOfBirth.Day.ToString() ||
                    dobMonth.GetAttribute("readonly") != "true" ||
                    dobMonth.GetAttribute("value") != quoteDetails.Drivers[0].Details.DateOfBirth.Month.ToString() ||
                    dobYear.GetAttribute("readonly") != "true" ||
                    dobYear.GetAttribute("value") != quoteDetails.Drivers[0].Details.DateOfBirth.Year.ToString())
                {
                    Reporting.Log($"The expected {errorMessage} field did not have the correct value or was not readonly.");
                    return false;
                }
            }
            catch
            {
                Reporting.Log($"Failure occurred in attempting to access field for {errorMessage}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Click continue to complete entered vehicle details
        /// </summary>
        public void ClickContinueFromCarDetails()
        {
            ClickControl($"{PANEL_VEHICLE}{CONTINUE_QUOTE_BTN}");
            
            // Animation from Car Details panel to Driver Details is hard to detect. Using fixed sleep.
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Click contiue for the given visible driver.
        /// </summary>
        /// <param name="driverIndex"></param>
        public void ClickContinueFromCurrentDriver(int driverIndex)
        {
            var xp_base = GetDriverPanelXPathBaseFromIndex(driverIndex);
            Reporting.Log($"Completed driver {driverIndex}.", _driver.TakeSnapshot());
            ClickControl($"{xp_base}{CONTINUE_QUOTE_BTN}", WaitTimes.T10SEC);
            // Animation from one accordion to the next is hard to detect. Using fixed sleep.
            Thread.Sleep(1000);
        }

        private void SetDriverDetails(int driverIndex, Driver details)
        {
            string xp_base = GetDriverPanelXPathBaseFromIndex(driverIndex);

            // Set gender
            string xp_gender = null;
            switch (details.Details.Gender)
            {
                case Gender.Female:
                    xp_gender = GENDER_FEMALE;
                    break;
                case Gender.Male:
                    xp_gender = GENDER_MALE;
                    break;
                default:
                    Reporting.Error("Test does not recognise the gender value: " + details.Details.Gender);
                    break;
            }
            ClickControl($"{xp_base}{xp_gender}");

            // Set DoB
            WaitForTextFieldAndEnterText(xp_base + DOB_DAY, details.Details.DateOfBirth.Day.ToString(), false);
            WaitForTextFieldAndEnterText(xp_base + DOB_MONTH, details.Details.DateOfBirth.Month.ToString(), false);
            WaitForTextFieldAndEnterText(xp_base + DOB_YEAR, details.Details.DateOfBirth.Year.ToString(), false);

            // Applicable only for main driver:
            if (driverIndex == 1)
            {
                // Set RAC membership status
                ClickBinaryToggle($"{xp_base}{MEMBER_YN}", XPR_YES, XPR_NO, details.Details.IsRACMember);

                if (details.Details.IsRACMember)
                {
                    WaitForSelectableAndPickByTyping(xp_base + MEMBERSHIP_LEVEL, MembershipTierText[details.Details.MembershipTier]);
                }

                // Set Email
                if (!string.IsNullOrEmpty(details.Details.GetEmail()))
                {
                    WaitForTextFieldAndEnterText(xp_base + EMAIL_ADDRESS, details.Details.GetEmail());
                }
            }

            switch (driverIndex)
            {
                case 1:
                    WaitForSelectableAndPickByTyping(xp_base + MAIN_DRIVER_LICENSE_YEARS, details.LicenseTime);
                    break;
                case 2:
                    WaitForSelectableAndPickByTyping(xp_base + ADDITIONAL_DRIVER1_LICENSE_YEARS, details.LicenseTime);
                    break;
                case 3:
                    WaitForSelectableAndPickByTyping(xp_base + ADDITIONAL_DRIVER2_LICENSE_YEARS, details.LicenseTime);
                    break;
                default:
                    throw new ArgumentException("Invalid driver index");
            }

            // Declarations
            if (details.LicenseConvictions == null || details.LicenseConvictions.Count == 0)
            {
                ClickControl($"{xp_base}{LIC_DISQUAL_NO}");
            }
            else
            {
                ClickControl($"{xp_base}{LIC_DISQUAL_YES}");

                for (int i = 0; i < details.LicenseConvictions.Count; i++)
                {
                    if (i > 0)
                        ClickAddTrafficConvictionInstance(driverIndex);

                    SetTrafficConviction(driverIndex, i, details.LicenseConvictions[i]);
                }
            }

            if (details.HistoricalAccidents == null || details.HistoricalAccidents.Count == 0)
            {
                ClickControl($"{xp_base}{PAST_CRASHES_NO}");
            }
            else
            {
                ClickControl($"{xp_base}{PAST_CRASHES_YES}");

                for (int i = 0; i < details.HistoricalAccidents.Count; i++)
                {
                    if (i > 0)
                        ClickAddHistoricalAccidentInstance(driverIndex);

                    SetAccidentHistory(driverIndex, i, details.HistoricalAccidents[i]);
                }
            }
        }

        public void ClearDriversDisclosureDetails(Driver details)
        {
            details.LicenseConvictions = null;
            details.HistoricalAccidents = null;
        }

        private void SetPPQMainDriverDetails(Driver details)
        {
            int driverIndex = 1; // As we're only dealing with main driver in this method.
            string xp_base = GetDriverPanelXPathBaseFromIndex(driverIndex);

            // Set number of years driver has held a driver's license.
            if (!string.IsNullOrEmpty(details.LicenseTime))
            {
                WaitForSelectableAndPickByTyping(xp_base + MAIN_DRIVER_LICENSE_YEARS, details.LicenseTime);
            }

            // Declarations
            if (details.LicenseConvictions == null || details.LicenseConvictions.Count == 0)
            {
                ClickControl($"{xp_base}{LIC_DISQUAL_NO}");
            }
            else
            {
                ClickControl($"{xp_base}{LIC_DISQUAL_YES}");

                for (int i = 0; i < details.LicenseConvictions.Count; i++)
                {
                    if (i > 0)
                        ClickAddTrafficConvictionInstance(driverIndex);

                    SetTrafficConviction(driverIndex, i, details.LicenseConvictions[i]);
                }
            }

            if (details.HistoricalAccidents == null || details.HistoricalAccidents.Count == 0)
            {
                ClickControl($"{xp_base}{PAST_CRASHES_NO}");
            }
            else
            {
                ClickControl($"{xp_base}{PAST_CRASHES_YES}");
                for (int i = 0; i < details.HistoricalAccidents.Count; i++)
                {
                    if (i > 0)
                        ClickAddHistoricalAccidentInstance(driverIndex);

                    SetAccidentHistory(driverIndex, i, details.HistoricalAccidents[i]);
                }
            }
        }

        private void ClickAddDriver(int driverIndex)
        {
            var xp_base = GetDriverPanelXPathBaseFromIndex(driverIndex);
            ClickControl($"{xp_base}{ADD_DRIVER_BTN}");

            // We use "driverIndex+1" as we're waiting for NEXT driver to display
            WaitForDriverDetailsToDisplay(driverIndex + 1);
        }

        private void ClickAddTrafficConvictionInstance(int driverIndex)
        {
            var xp_base = GetDriverPanelXPathBaseFromIndex(driverIndex);
            var xp_button = string.Format("//div[@id='Question_Driver_Contact_{0}__Declaration_TrafficConvictions_addButton']", driverIndex - 1);
            ClickControl($"{xp_base}{xp_button}");

            // Allow new control time to render.
            Thread.Sleep(1000);
        }

        private void ClickAddHistoricalAccidentInstance(int driverIndex)
        {
            var xp_base = GetDriverPanelXPathBaseFromIndex(driverIndex);
            var xp_button = string.Format("//div[@id='Question_Driver_Contact_{0}__Declaration_AccidentClaims_addButton']", driverIndex - 1);
            ClickControl($"{xp_base}{xp_button}");

            // Allow new control time to render.
            Thread.Sleep(1000);
        }

        private void SetTrafficConviction(int driverIndex, int eventIndex, Declaration item)
        {
            var xp_base = GetDriverPanelXPathBaseFromIndex(driverIndex);
            var xp_month  = string.Format("//span[@aria-owns='Question_Driver_Contact_{0}__Declaration_TrafficConvictions_{1}__DateOfOffence_Month_listbox']", driverIndex-1, eventIndex);
            var xp_year   = string.Format("//span[@aria-owns='Question_Driver_Contact_{0}__Declaration_TrafficConvictions_{1}__DateOfOffence_Year_listbox']", driverIndex-1, eventIndex);
            var xp_detail = string.Format("//span[@aria-owns='Question_Driver_Contact_{0}__Declaration_TrafficConvictions_{1}__ConvictionType_listbox']", driverIndex-1, eventIndex);

            _driver.WaitForElement(By.XPath(xp_base + xp_month), WaitTimes.T30SEC);

            WaitForSelectableAndPickByTyping(xp_base + xp_month, item.Month);
            WaitForSelectableAndPickByTyping(xp_base + xp_year, item.Year);
            WaitForSelectableAndPickByTyping(xp_base + xp_detail, item.Description);
            Reporting.Log($"Traffic Conviction {item.Description} added effective {item.Year}-{item.Month}, taking snapshot", _driver.TakeSnapshot());
        }

        private void SetAccidentHistory(int driverIndex, int eventIndex, Declaration item)
        {
            var xp_base = GetDriverPanelXPathBaseFromIndex(driverIndex);
            var xp_month   = string.Format("//span[@aria-owns='Question_Driver_Contact_{0}__Declaration_AccidentClaims_{1}__DateOfEvent_Month_listbox']", driverIndex-1, eventIndex);
            var xp_year    = string.Format("//span[@aria-owns='Question_Driver_Contact_{0}__Declaration_AccidentClaims_{1}__DateOfEvent_Year_listbox']", driverIndex-1, eventIndex);
            var xp_detail  = string.Format("//span[@aria-owns='Question_Driver_Contact_{0}__Declaration_AccidentClaims_{1}__AccidentClaimType_listbox']", driverIndex-1, eventIndex);
            var xp_insurer = string.Format("//span[@aria-owns='Question_Driver_Contact_{0}__Declaration_AccidentClaims_{1}__InsurerCode_listbox']", driverIndex-1, eventIndex);

            _driver.WaitForElement(By.XPath(xp_base + xp_month), WaitTimes.T30SEC);

            WaitForSelectableAndPickByTyping(xp_base + xp_month, item.Month);
            WaitForSelectableAndPickByTyping(xp_base + xp_year, item.Year);
            if (item.Description != null)   WaitForSelectableAndPickByTyping(xp_base + xp_detail, item.Description);
            if (item.InsurerAtTime != null) WaitForSelectableAndPickByTyping(xp_base + xp_insurer, item.InsurerAtTime);
            Reporting.Log($"Accident/Loss {item.Description} with {item.InsurerAtTime} added effective {item.Year}-{item.Month}, taking snapshot", _driver.TakeSnapshot());
        }

        private string GetDriverPanelXPathBaseFromIndex(int driverIndex)
        {
            string xp_base = null;
            switch (driverIndex)
            {
                case 1:
                    xp_base = SEC_DRIVER_MAIN + PANEL;
                    break;
                case 2:
                    xp_base = SEC_DRIVER_ADD1 + PANEL;
                    break;
                case 3:
                    xp_base = SEC_DRIVER_ADD2 + PANEL;
                    break;
                default:
                    Reporting.Error("Driver index is out of valid range.");
                    break;
            }
            return xp_base;
        }

        private bool IsDeclineNoticeExpectedFromMotorClaimsHistory(Driver driver)
        {
            var disclosedClaimsCount = driver?.PastClaims?.Count ?? 0;

            return disclosedClaimsCount > MAXIMUM_CLAIMS_HISTORY;
        }

        public void MotorWaitForDeclinedCoverNoticeAndDismiss(Browser browser, QuoteCar quoteDetails)
        {
            Reporting.Log($"Checking that Decline Notice is displayed.", browser.Driver.TakeSnapshot());
            _driver.WaitForElementToBeVisible(By.XPath(XP_DECLINED_DISCLOSURES_HEADER), WaitTimes.T30SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_DECLINED_NOTICE_TEXT), WaitTimes.T5SEC);
            Reporting.Log("Cover declined notice triggered as expected.", _driver.TakeSnapshot());
            Reporting.AreEqual(DECLINE_NOTICE_HEADER_TEXT, GetInnerText(XP_DECLINED_DISCLOSURES_HEADER), "- Decline Notice Header Text");
            
            Reporting.Log("Looking to match the cover declined dialog text");
            var dialogText = GetInnerText(XP_DECLINED_NOTICE_TEXT).StripLineFeedAndCarriageReturns();
            Regex declineDialogRegEx = new Regex(FixedTextRegex.QUOTE_COVER_DECLINED_TEXT);
            Match match = declineDialogRegEx.Match(dialogText);
            Reporting.IsTrue(match.Success, $"the cover declined dialog text matches.");
            
            ClickControl(XP_DECLINED_NOTICE_DISMISS);
            Thread.Sleep(2000);
            Reporting.Log("Clicked to dismiss declined notice.", browser.Driver.TakeSnapshot());
            ClickControl(SEC_DRIVER_MAIN);
            for (int i = 0; i < quoteDetails.Drivers.Count; i++)
            {
                Reporting.Log($"Resetting disclosures to be added for driver index ref {i} to null and will go back over disclosure fields.", browser.Driver.TakeSnapshot());
                ClearDriversDisclosureDetails(quoteDetails.Drivers[i]);
            }
            FillQuoteDetailsSecondAccordion(quoteDetails, true, false, isModifyingExistingDrivers: true);
        }

        public void MotorWaitForVehicleUsageDeclinedCoverNoticeAndDismiss(QuoteCar quoteDetails)
        {
            Reporting.Log($"Checking that Decline Notice is displayed.", _driver.TakeSnapshot());
            _driver.WaitForElementToBeVisible(By.XPath(XP_DECLINED_VEHICLE_USE_HEADER), WaitTimes.T10SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_DECLINED_NOTICE_TEXT), WaitTimes.T5SEC);
            Reporting.Log("Cover declined notice triggered as expected.", _driver.TakeSnapshot());
            Reporting.AreEqual(DECLINE_NOTICE_HEADER_TEXT, GetInnerText(XP_DECLINED_VEHICLE_USE_HEADER), "- Decline Notice Header Text");

            Reporting.Log("Looking to match the cover declined dialog text for vehicle usage");
            var dialogText = GetInnerText(XP_DECLINED_NOTICE_TEXT).StripLineFeedAndCarriageReturns();
            Regex declineDialogRegEx = new Regex(FixedTextRegex.QUOTE_COVER_DECLINED_VEH_USAGE_TEXT);
            Match match = declineDialogRegEx.Match(dialogText);
            Reporting.IsTrue(match.Success, $"the cover declined dialog text for vehicle usage matches.");

            ClickControl(XP_DECLINED_NOTICE_DISMISS);
            Thread.Sleep(2000);
            Reporting.Log("Clicked to dismiss declined notice.", _driver.TakeSnapshot());
            Usage = VehicleUsage.Private;
            quoteDetails.UsageType = VehicleUsage.Private;
            Reporting.Log($"Set acceptable vehicle usage.", _driver.TakeSnapshot());
            ClickContinueFromCarDetails();
        }

        /// <summary>
        /// Essentially a sleep for about 10 seconds waiting for the vehicle make dropdown
        /// to collapse. If we detect it hiding, then we'll exit early.
        /// 
        /// No error is thrown, as a non-collapsed dropdown will break the test when the
        /// other fields are attempted.
        /// </summary>
        private void WaitForMakeDropDownToClose()
        {
            var endTime = DateTime.Now.AddSeconds(20);
            var dropdown = GetElement(XP_CAR_MAKE_DROPDOWN);
            do
            {
                if (dropdown.GetAttribute("aria-hidden").Equals("true"))
                    break;
                Thread.Sleep(500);
            } while (DateTime.Now < endTime);
        }
    }
}
