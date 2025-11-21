using Rac.TestAutomation.Common;
using OpenQA.Selenium;

namespace UIDriver.Pages
{
    public class WestpacQuickStream : BasePage
    {
        #region XPATHS
        private const string XP_CREDIT_CARD_PAYMENT_IFRAME = "//iframe[@id='trustedFrame-creditCard']";
        private const string XP_NAME_ON_CARD               = "id('cardholderName')";
        private const string XP_CARD_NUMBER                = "id('cardNumber')";
        private const string XP_EXPIRY_MONTH               = "id('expiryDateMonth')";
        private const string XP_MONTH_OPTIONS              = "id('expiryDateMonth')/option";
        private const string XP_EXPIRY_YEAR                = "id('expiryDateYear')";
        private const string XP_YEAR_OPTIONS               = "id('expiryDateYear')/option";
        private const string XP_CVC                        = "id('cvn')";

        private const string XP_VISA_ICON                  = "//div[@id='cardNumberField']/label[@for='cardNumber']/following-sibling::div[@id='accepted-cards']/div[@title='VISA accepted']";
        private const string XP_MASTERCARD_ICON            = "//div[@id='cardNumberField']/label[@for='cardNumber']/following-sibling::div[@id='accepted-cards']/div[@title='MasterCard accepted']";
        private const string XP_AMEX_ICON                  = "//div[@id='cardNumberField']/label[@for='cardNumber']/following-sibling::div[@id='accepted-cards']/div[@title='American Express accepted']";
        private const string XP_DINERS_ICON                = "//div[@id='cardNumberField']/label[@for='cardNumber']/following-sibling::div[@id='accepted-cards']/div[@title='Diners Card accepted']";
        private const string XP_UNIONPAY_ICON              = "//div[@id='cardNumberField']/label[@for='cardNumber']/following-sibling::div[@id='accepted-cards']/div[@title='UnionPay accepted']";

        private const string XP_CARDHOLDERNAME_LBL         = "//div[@id='cardholderNameField']/label";
        private const string XP_CARDNUMBER_LBL             = "//div[@id='cardNumberField']/label";
        private const string XP_EXPIRYDATE_LBL             = "//div[@id='expiryDateField']/label";
        private const string XP_CVN_LBL                    = "//div[@id='cvnField']/label";
        #endregion

        #region Settable properties and controls
        public string CCNumber
        {
            get => GetElement(XP_CARD_NUMBER).GetAttribute("value");
            set => WaitForTextFieldAndEnterText(XP_CARD_NUMBER, value, false);
        }

        public string CCName
        {
            get => GetElement(XP_NAME_ON_CARD).GetAttribute("value");
            set => WaitForTextFieldAndEnterText(XP_NAME_ON_CARD, value, false);
        }

        public string ExpMonth
        {
            get => GetSelectedTextFromDropDown(XP_EXPIRY_MONTH);
            set => WaitForSelectableAndPickFromDropdown(XP_EXPIRY_MONTH, XP_MONTH_OPTIONS, value);
        }

        public string ExpYear
        {
            get => GetSelectedTextFromDropDown(XP_EXPIRY_YEAR);
            set => WaitForSelectableAndPickFromDropdown(XP_EXPIRY_YEAR, XP_YEAR_OPTIONS, value);
        }

        public string CVNNumber
        {
            get => GetElement(XP_CVC).GetAttribute("value");
            set => WaitForTextFieldAndEnterText(XP_CVC, value, false);
        }
        #endregion

        public WestpacQuickStream(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_CREDIT_CARD_PAYMENT_IFRAME);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }

            return isDisplayed;
        }

        public void EnterCardDetails(Payment payment)
        {
            WaitForPage();

            var ccDetails = payment.CreditCardDetails;

            // Check Westpac trusted iFrame elements are initially cleared-out
            Reporting.Log("Verify iFrame fields are cleared-out initially.");
            CheckWestpacIframeAreCleared(payment);

            // Verify payment field labels and icons:
            CheckWestpacIframeElements(payment);

            FillOutCardDetails(payment, ccDetails);
        }

        /// <summary>
        /// Verifies Westpac trusted iFrame elements - labels and icons
        /// </summary>
        private void CheckWestpacIframeElements(Payment payment)
        {
            SwitchToWestpacIframe();

            Reporting.Log("Verify payment icons: VISA, MC, AMEX, DINERS, UNIONPAY and field labels.");
            Reporting.AreEqual("Name on card", GetInnerText(XP_CARDHOLDERNAME_LBL),
                            "Name on card".IsExpectedLabelText());
            Reporting.AreEqual("Card number", GetInnerText(XP_CARDNUMBER_LBL),
                            "Card number".IsExpectedLabelText());
            Reporting.AreEqual("Expiry date", GetInnerText(XP_EXPIRYDATE_LBL),
                            "Expiry date".IsExpectedLabelText());

            if (payment.IsAnnual)
            {
                Reporting.IsTrue(IsControlDisplayed(XP_CVC),
                                "CVC".IsDisplayed());
                Reporting.AreEqual("CVC", GetInnerText(XP_CVN_LBL),
                                "CVC".IsExpectedLabelText());
            }
            else
                Reporting.IsFalse(IsControlDisplayed(XP_CVC), "CVC".IsNotDisplayed());

            Reporting.IsTrue(IsControlDisplayed(XP_VISA_ICON),
                            "Visa icon".IsDisplayed());
            Reporting.IsFalse(GetClass(XP_VISA_ICON).Contains("active"),
                            "Visa icon".IsDisabled());
            Reporting.IsTrue(IsControlDisplayed(XP_MASTERCARD_ICON),
                            "Mastercard icon".IsDisplayed());
            Reporting.IsFalse(GetClass(XP_MASTERCARD_ICON).Contains("active"),
                            "Mastercard icon".IsDisabled());
            Reporting.IsTrue(IsControlDisplayed(XP_AMEX_ICON),
                            "Amex icon".IsDisplayed());
            Reporting.IsFalse(GetClass(XP_AMEX_ICON).Contains("active"),
                            "Amex icon".IsDisabled());
            Reporting.IsFalse(IsControlDisplayed(XP_DINERS_ICON),
                            "Diners icon".IsNotDisplayed());
            Reporting.IsFalse(IsControlDisplayed(XP_UNIONPAY_ICON),
                            "Union Pay icon".IsNotDisplayed());

            SwitchToParentFrame();
        }

        /// <summary>
        /// Verifies Westpac trusted iFrame elements are cleared-out initially
        /// </summary>
        private void CheckWestpacIframeAreCleared(Payment payment)
        {
            SwitchToWestpacIframe();

            Reporting.AreEqual(string.Empty, CCName);
            Reporting.AreEqual(string.Empty, CCNumber);
            Reporting.AreEqual("Month", ExpMonth);
            Reporting.AreEqual("Year", ExpYear);

            if (payment.IsAnnual)
                Reporting.AreEqual(string.Empty, CVNNumber);

            SwitchToParentFrame();
        }

        private void FillOutCardDetails(Payment payment, CreditCard ccDetails)
        {
            //Allows Selenium to access the objects inside the Credit Card payment iFrame
            SwitchToWestpacIframe();

            Reporting.Log("Enter credit card details.");
            CCName   = ccDetails.CardholderName;
            CCNumber = ccDetails.CardNumber;
            ExpMonth = ccDetails.CardExpiryDate.ToString("MM");
            ExpYear  = ccDetails.CardExpiryDate.ToString("yyyy");

            if (payment.IsAnnual)
            {
                Reporting.Log($"CreditCardCvc = '{ccDetails.CVNNumber}'");
                CVNNumber = ccDetails.CVNNumber;
            }

            Reporting.Log($"CreditCardName = {ccDetails.CardholderName}, CreditCardNumber = '{ccDetails.CardNumber}', " +
                $"CreditCardExpiryDateMonth = '{ccDetails.CardExpiryDate.ToString("MM")}', CreditCardExpiryDateYear = '{ccDetails.CardExpiryDate.ToString("yyyy")}", _browser.Driver.TakeSnapshot());

            SwitchToParentFrame();
        }

        private void SwitchToWestpacIframe()
        {
            _driver.SwitchTo().Frame(_driver.FindElement(By.XPath(XP_CREDIT_CARD_PAYMENT_IFRAME)));
        }

        private void SwitchToParentFrame()
        {
            _driver.SwitchTo().ParentFrame();
        }
    }
}