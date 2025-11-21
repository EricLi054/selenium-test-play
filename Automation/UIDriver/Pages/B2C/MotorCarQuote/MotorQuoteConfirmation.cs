using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using System.Text.RegularExpressions;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace UIDriver.Pages.B2C
{
    public class MotorQuoteConfirmation : BasePage
    {
        #region XPATHS
        private const string BASE            = "/html/body/div[@id='wrapper']";
        private const string CONTENT         = BASE + "//form";
        private const string SUMMARY_GENERAL = CONTENT + "//div[@class='summary-container']//div[@class='summary-items'][1]";
        private const string SUMMARY_DRIVERS = CONTENT + "//div[@class='summary-container']//div[@class='summary-items'][2]";
        private const string SUMMARY_HOLDERS = CONTENT + "//div[@class='summary-container']//div[@class='summary-items'][3]";
        private const string SUMMARY_PAYMENT = CONTENT + "//div[@class='summary-container']//div[@class='summary-items'][4]";

        private const string XP_HEADER       = BASE + "/div[contains(@class,'pet-header')]//span[@class='action-heading']/span";

        // General Summary fields
        private const string XP_POLICY_NUM = SUMMARY_GENERAL + "//div[@id='PolicyNumber_Answer']";
        private const string XP_COVER_TYPE = SUMMARY_GENERAL + "//div[@id='ProductType_Answer']";
        private const string XP_CAR_DETAIL = SUMMARY_GENERAL + "//div[@id='VehicleDescription_Answer']";
        private const string XP_CAR_REGO   = SUMMARY_GENERAL + "//div[@id='RegistrationNumber_Answer']";
        private const string XP_POL_START  = SUMMARY_GENERAL + "//div[@id='PolicyStartDate_Answer']";
        private const string XP_POL_END    = SUMMARY_GENERAL + "//div[@id='PolicyEndDate_Answer']";

        // Driver(s)
        private const string XP_DRIVER_X     = SUMMARY_DRIVERS + "//div[@class='question-label']";

        // PolicyHolder(s)
        private const string XP_POL_HOLDER_X = SUMMARY_HOLDERS + "//div[@class='question-label']";

        // Payment details
        private const string XP_PAYMENT_FREQ_LBL               = SUMMARY_PAYMENT + "//label[@for='PaymentFrequency']";
        private const string XP_PAYMENT_FREQ                   = SUMMARY_PAYMENT + "//div[@id='PaymentFrequency_Answer']";
        private const string XP_PAYMENT_AMNT_LBL               = SUMMARY_PAYMENT + "//label[@for='AmountPaid']";
        private const string XP_PAYMENT_AMNT                   = SUMMARY_PAYMENT + "//div[@id='AmountPaid_Answer']";

        private const string XP_INSURANCE_PREMIUM_LBL          = SUMMARY_PAYMENT + "//label[@for='AmountPaidInsurancePremium']";
        private const string XP_INSURANCE_PREMIUM              = SUMMARY_PAYMENT + "//div[@id='AmountPaidInsurancePremium_Answer']";
        private const string XP_ROADSIDE_ASSISTANCE_AMOUNT_LBL = SUMMARY_PAYMENT + "//label[@for='RoadSideAmount']";
        private const string XP_ROADSIDE_ASSISTANCE_AMOUNT     = SUMMARY_PAYMENT + "//div[@id='RoadSideAmount_Answer']";

        // Controls
        private const string XP_BTN_PRINT = CONTENT + "//a[@id='print-quote']";

        // Marketing
        private const string XP_ROADSIDE_STEP1_HDR      = CONTENT + "//div[@id='confirmationStepOne']/h2";
        private const string XP_ROADSIDE_STEP2_HDR      = CONTENT + "//div[@id='confirmationStepTwo']/h2";
        private const string XP_ROADSIDE_STEP2_BTN      = CONTENT + "//form[@id='roadside-form']/button/div[@class='btn-text']";
        private const string XP_ROADSIDE_ASSISTANCE_LBL = CONTENT + "//div[contains(text(), 'Roadside Assistance')]/following-sibling::div/div/i/following-sibling::span[1]";
        private const string XP_RAC_MEMBER_NUMBER       = "//span[@id='memberNumber']";

        private const string XP_RECEIPT_NUMBER_LBL      = "//label[@for='ReceiptNumber']";
        private const string XP_RECEIPT_NUMBER          = "//div[@id='ReceiptNumber_Answer']";
        #endregion

        #region Settable properties and controls
        public string PolicyNumber => GetInnerText(XP_POLICY_NUM);

        public string CoverType => GetInnerText(XP_COVER_TYPE);


        public string CarDetail => GetInnerText(XP_CAR_DETAIL);

        public string CarRegistration => GetInnerText(XP_CAR_REGO);

        public string PolicyStartDate => GetInnerText(XP_POL_START);

        public string PolicyEndDate => GetInnerText(XP_POL_END);

        public List<string> Drivers
        {
            get
            {
                var results = new List<string>();
                var drivers = _driver.FindElements(By.XPath(XP_DRIVER_X));
                foreach(var driver in drivers)
                {
                    results.Add(driver.Text);
                }
                return results;
            }
        }

        public List<string> PolicyHolders
        {
            get
            {
                var results = new List<string>();
                var holders = _driver.FindElements(By.XPath(XP_POL_HOLDER_X));
                foreach (var holder in holders)
                {
                    results.Add(holder.Text);
                }
                return results;
            }
        }

        public PaymentFrequency PaymentFrequency => DataHelper.GetValueFromDescription<PaymentFrequency>(
                                                        GetInnerText(XP_PAYMENT_FREQ));


        public decimal AmountPaid
        {
            get
            {
                var element = GetElement(XP_PAYMENT_AMNT);
                var price = decimal.Parse(element.Text.StripMoneyNotations());
                return price;
            }
        }

        public decimal InsurancePremiumAmount
        {
            get
            {
                var element = GetElement(XP_INSURANCE_PREMIUM);
                var amount = decimal.Parse(element.Text.StripMoneyNotations());
                return amount;
            }
        }

        public decimal RoadsideAssistanceAmount
        {
            get
            {
                var element = GetElement(XP_ROADSIDE_ASSISTANCE_AMOUNT);
                var amount = decimal.Parse(element.Text.StripMoneyNotations());
                return amount;
            }
        }

        /// <summary>
        /// Gets the header text of the right-side Roadside
        /// Step 1 panel. Should indicate if Step 1 is now
        /// complete or if still pending
        /// </summary>
        public string RoadsideStep1Header => GetInnerText(XP_ROADSIDE_STEP1_HDR);

        /// <summary>
        /// Gets the header text of the right-side Roadside
        /// Step 2 panel. Should always indicate pending at
        /// this point of transaction.
        /// </summary>
        public string RoadsideStep2Header => GetInnerText(XP_ROADSIDE_STEP2_HDR);

        /// <summary>
        /// Returns true/false if this button can be found
        /// which allows user to proceed to Step 2, for
        /// paying for their roadside assistance.
        /// </summary>
        public bool IsRoadsideStep2ButtonPresent
        {
            get
            {
                IWebElement element = null;
                bool isPresent = false;
                _driver.TryFindElement(By.XPath(XP_ROADSIDE_STEP2_BTN), out element);
                isPresent = (element != null);
                return isPresent;
            }
        }

        /// <summary>
        /// Gets the text of the left-side Roadside Assistance header
        /// Should be shown when Roadside assistance is added and paid by annual cash
        /// </summary>
        public string RoadsideAssistanceLabel => GetInnerText(XP_ROADSIDE_ASSISTANCE_LBL);

        public bool IsClassicRoadsideAssistanceTextDisplayed => IsControlDisplayed(XP_ROADSIDE_ASSISTANCE_LBL);

        public bool IsPaymentFrequencyLabelDisplayed => IsControlDisplayed(XP_PAYMENT_FREQ_LBL);

        public string PaymentFrequencyLabelText => GetInnerText(XP_PAYMENT_FREQ_LBL);

        public bool IsAmountLabelDisplayed => IsControlDisplayed(XP_PAYMENT_AMNT_LBL);

        public string AmountLabelText => GetInnerText(XP_PAYMENT_AMNT_LBL);

        public bool IsAmountDisplayed => IsControlDisplayed(XP_PAYMENT_AMNT);

        public bool IsInsurancePremiumLabelDisplayed => IsControlDisplayed(XP_INSURANCE_PREMIUM_LBL);

        public string InsurancePremiumLabelText => GetInnerText(XP_INSURANCE_PREMIUM_LBL);

        public bool IsInsurancePremiumDisplayed => IsControlDisplayed(XP_INSURANCE_PREMIUM);

        public bool IsRoadsideAssistanceLabelDisplayed => IsControlDisplayed(XP_ROADSIDE_ASSISTANCE_AMOUNT_LBL);

        public string RoadsideAssistanceLabelText => GetInnerText(XP_ROADSIDE_ASSISTANCE_AMOUNT_LBL);

        public bool IsRoadsideAssistanceDisplayed => IsControlDisplayed(XP_ROADSIDE_ASSISTANCE_AMOUNT);

        public string RoadsideMemberNumber => GetInnerText(XP_RAC_MEMBER_NUMBER);

        /// <summary>
        /// Gets the actual receipt number that Westpac generates from confirmation page
        /// </summary>
        public string ReceiptNumber => GetInnerText(XP_RECEIPT_NUMBER);

        public bool IsReceiptNumberLabelDisplayed => IsControlDisplayed(XP_RECEIPT_NUMBER_LBL);

        public string ReceiptNumberLabelText => GetInnerText(XP_RECEIPT_NUMBER_LBL);

        public bool IsReceiptNumberDisplayed => IsControlDisplayed(XP_RECEIPT_NUMBER);
        #endregion

        public MotorQuoteConfirmation(Browser browser) : base(browser) { }

        public string VerifyReceiptNumberIsDisplayedCorrectly()
        {
            Regex receiptNumberRegex = new Regex(@"(?=1)\d{10}");
            var actualReceiptNumber  = ReceiptNumber;
            Match match              = receiptNumberRegex.Match(actualReceiptNumber);
            Reporting.IsTrue(match.Success, "Receipt number generated");
            Reporting.AreEqual("Receipt number:", ReceiptNumberLabelText, "Receipt number label as expected");
            Reporting.Log($"Receipt number: {actualReceiptNumber}", _browser.Driver.TakeSnapshot());

            return actualReceiptNumber;
        }

        public override bool IsDisplayed()
        {
            try
            {
                var element = GetElement(XP_HEADER);
                if (element.Text != "Car insurance policy")
                {
                    Reporting.Log("Not the expected page header.");
                    return false;
                }
                GetElement(XP_POLICY_NUM);
                GetElement(XP_PAYMENT_FREQ);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Motor Quote - Policy issued confirmation page");
            return true;
        }
    }
}