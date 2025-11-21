using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Text.RegularExpressions;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace UIDriver.Pages.B2C
{
    public class PetQuoteConfirmation : BasePage
    {
        #region XPATHS
        private const string BASE            = "/html/body/div[@id='wrapper']";
        private const string CONTENT         = BASE + "//form";

        private const string XP_HEADER       = BASE + "/div[contains(@class,'pet-header')]//span[@class='action-heading']/span";

        // General Summary fields
        private const string XP_POLICY_NUM = "id('PolicyNumber_Answer')";
        private const string XP_PET_DETAIL = "id('InsuredPet_Answer')";
        private const string XP_POL_START  = "id('PolicyStartDate_Answer')";
        private const string XP_POL_END    = "id('PolicyEndDate_Answer')";

        // PolicyHolder(s)
        private const string XP_POLICYHOLDER = "id('Contacts_0__Name_Answer')";

        // Payment details
        private const string XP_PAYMENT_FREQ_LBL   = "//label[@for='PaymentFrequency']";
        private const string XP_PAYMENT_FREQ       = "id('PaymentFrequency_Answer')";
        private const string XP_PAYMENT_AMNT_LBL   = "//label[@for='AmountPaid']";
        private const string XP_PAYMENT_AMNT       = "id('AmountPaid_Answer')";
        private const string XP_RECEIPT_NUMBER     = "//div[@id='ReceiptNumber_Answer']";
        private const string XP_RECEIPT_NUMBER_LBL = "//label[@for='ReceiptNumber']";
        #endregion

        #region Settable properties and controls
        public string PolicyNumber => GetInnerText(XP_POLICY_NUM);

        public string PetDetail => GetInnerText(XP_PET_DETAIL);

        public string PolicyStartDate => GetInnerText(XP_POL_START);

        public string PolicyEndDate => GetInnerText(XP_POL_END);

        public string PolicyHolder => GetInnerText(XP_POLICYHOLDER);

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

        public bool IsPaymentFrequencyLabelDisplayed => IsControlDisplayed(XP_PAYMENT_FREQ_LBL);

        public string PaymentFrequencyLabelText => GetInnerText(XP_PAYMENT_FREQ_LBL);

        public bool IsAmountLabelDisplayed => IsControlDisplayed(XP_PAYMENT_AMNT_LBL);

        public string AmountLabelText => GetInnerText(XP_PAYMENT_AMNT_LBL);

        public string ReceiptNumber => GetInnerText(XP_RECEIPT_NUMBER);

        public string ReceiptNumberLabelText => GetInnerText(XP_RECEIPT_NUMBER_LBL);

        public bool IsReceiptNumberLabelDisplayed => IsControlDisplayed(XP_RECEIPT_NUMBER_LBL);

        public bool IsReceiptNumberDisplayed => IsControlDisplayed(XP_RECEIPT_NUMBER);
        #endregion

        public PetQuoteConfirmation(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                var field = GetElement(XP_HEADER);
                if (field.Text != "Pet insurance policy")
                {
                    Reporting.Log("Not the expected page header.");
                    return false;
                }
                GetElement(XP_POLICY_NUM);
                GetElement(XP_PAYMENT_AMNT);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Pet Quote - Policy issued confirmation page");
            return true;
        }

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
    }
}