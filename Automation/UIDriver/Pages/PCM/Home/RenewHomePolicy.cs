using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Threading;
using UIDriver.Pages.B2C;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace UIDriver.Pages.PCM.Home
{
    public class RenewHomePolicy : BaseEndorsementPage
    {
        #region XPATHS
        // General
        private const string XPR_RADIO_YES = "//span[text()='Yes']";
        private const string XPR_RADIO_NO = "//span[text()='No']";

        // Renew My Policy
        private const string XP_UPDATEHOMEPOLICY_CONTINUE_BUTTON   = "id('accordion_0_submit-action')";
        private const string XP_WEEKLY_RENTAL_AMOUNT               = "id('WeeklyRentalAmount')";
        private const string XP_PROPERTY_MANAGEMENT                = "//span[@aria-owns='PropertyManagerType_listbox']";
        private const string XP_PROPERTY_MANAGEMENT_OPTIONS        = "id('PropertyManagerType_listbox')/li";
        private const string XP_BUILDING_TYPE                      = "//span[@aria-owns='Building_DwellingType_listbox']";
        private const string XP_ALARM_TYPE                         = "//span[@aria-owns='Building_AlarmTypeId_listbox']";
        private const string XP_IS_ELEVATED_YN                     = "id('IsPropertyElevated')";
        private const string XP_HAS_CYCLONE_SHUTTERS_YN            = "id('HasCycloneShutters')";
        private const string XP_GARAGE_DOOR_UPGRADES               = "//span[@aria-owns='GarageDoorUpgradePost2012_listbox']";
        private const string XP_GARAGE_DOOR_UPGRADES_OPTION        = "id('GarageDoorUpgradePost2012_listbox')/li";
        private const string XP_ROOF_IMPROVEMENTS                  = "//span[@aria-owns='RoofRepairsPost1982_listbox']";
        private const string XP_ROOF_IMPROVEMENTS_OPTION           = "id('RoofRepairsPost1982_listbox')/li";

        // Changes to the premium
        private const string XP_PREMIUM_LABEL_TEXT                 = "//div[@class='price-label']";
        private const string XP_PREMIUM_AMOUNT_TEXT                = "id('AdjustmentPremium')";
        private const string XP_INSTALMENT_AMOUNT_TEXT             = "//div[@class='next-installment-text']";
        private const string XP_EMAIL_TEXT                         = "id('Email')";
        private const string XP_UPDATECOVER_CONFIRM_BUTTON         = "id('accordion_1_submit-action')";

        // Payment details
        private const string XP_PAYMENT_AMOUNT_TEXT                = "id('PaymentAmount_Answer')";
        private const string XP_PAYMENT_OPTIONS_RADIO_BUTTON_GROUP = "id('IsPayingNow')";
        private const string XPR_RADIO_PAYNOW                      = "//span[text()='Pay now']";
        private const string XPR_RADIO_PAYLATER                    = "//span[text()='Pay later']";
        private const string XP_WESTPAC_PAY_NOW_BUTTON             = "//span[@class='btn-text' and text()='Pay now']";
        #endregion

        #region Settable properties and controls

        public string Email
        {
            get => throw new NotImplementedException("Not yet implemented access to shadow DOM.");
            set => WaitForTextFieldAndEnterText(XP_EMAIL_TEXT, value, false);
        }

        /// <summary>
        /// Question regarding whether property is elevated, conditional based on whether
        /// property is in a cyclone risk area
        /// </summary>
        /// <exception cref="InvalidElementStateException">If attempt to read state, but no option has been selected yet.</exception>
        public bool IsPropertyElevated
        {
            get
            {
                var radioButton = $"{XP_IS_ELEVATED_YN}{XPR_RADIO_YES}/../span[contains(@class,'rb-radio')]";
                if (GetClass(radioButton).Contains("checked"))
                { return true; }

                radioButton = $"{XP_IS_ELEVATED_YN}{XPR_RADIO_NO}/../span[contains(@class,'rb-radio')]";
                if (GetClass(radioButton).Contains("checked"))
                { return false; }

                throw new InvalidElementStateException("No radio button has been selected for is property elevated question.");
            }
            set => ClickControl(value ? $"{XP_IS_ELEVATED_YN}{XPR_RADIO_YES}" : $"{XP_IS_ELEVATED_YN}{XPR_RADIO_NO}");
        }

        /// <summary>
        /// Question regarding whether property has cyclone shutters, conditional based on 
        /// whether property is in a cyclone risk area
        /// </summary>
        /// <exception cref="InvalidElementStateException">If attempt to read state, but no option has been selected yet.</exception>
        public bool HasCycloneWindowShutters
        {
            get
            {
                var radioButton = $"{XP_HAS_CYCLONE_SHUTTERS_YN}{XPR_RADIO_YES}/../span[contains(@class,'rb-radio')]";
                if (GetClass(radioButton).Contains("checked"))
                { return true; }

                radioButton = $"{XP_HAS_CYCLONE_SHUTTERS_YN}{XPR_RADIO_NO}/../span[contains(@class,'rb-radio')]";
                if (GetClass(radioButton).Contains("checked"))
                { return false; }

                throw new InvalidElementStateException("No radio button has been selected for window cyclone shutters question.");
            }
            set => ClickControl(value ? $"{XP_HAS_CYCLONE_SHUTTERS_YN}{XPR_RADIO_YES}" : $"{XP_HAS_CYCLONE_SHUTTERS_YN}{XPR_RADIO_NO}");
        }

        /// <summary>
        /// Question regarding whether the garage door has had any upgrades. This question
        /// is a conditional question based on whether the property is in a cyclone risk
        /// area, and was built prior to 2012.
        /// </summary>
        /// <exception cref="InvalidElementStateException">If attempt to read state, but control is not visible or has an unrecognised value.</exception>
        public GarageDoorsUpgradeStatus GarageDoorStatus
        {
            get
            {
                var dropdownText = GetInnerText(XP_GARAGE_DOOR_UPGRADES);
                try
                {
                    return DataHelper.GetValueFromDescription<GarageDoorsUpgradeStatus>(dropdownText);
                }
                catch
                {
                    throw new InvalidElementStateException($"Unrecognised text in dropdown, received: {dropdownText}");
                }
            }
            set => WaitForSelectableAndPickFromDropdown(XP_GARAGE_DOOR_UPGRADES,
                                                       XP_GARAGE_DOOR_UPGRADES_OPTION,
                                                       value.GetDescription());
        }

        /// <summary>
        /// Question regarding whether the roof has had any improvements. This question
        /// is a conditional question based on whether the property is in a cyclone risk
        /// area, and was built prior to 1982.
        /// </summary>
        /// <exception cref="InvalidElementStateException">If attempt to read state, but control is not visible or has an unrecognised value.</exception>
        public RoofImprovementStatus RoofImprovements
        {
            get
            {
                var dropdownText = GetInnerText(XP_ROOF_IMPROVEMENTS);
                try
                {
                    return DataHelper.GetValueFromDescription<RoofImprovementStatus>(dropdownText);
                }
                catch
                {
                    throw new InvalidElementStateException($"Unrecognised text in dropdown, received: {dropdownText}");
                }
            }
            set => WaitForSelectableAndPickFromDropdown(XP_ROOF_IMPROVEMENTS,
                                                       XP_ROOF_IMPROVEMENTS_OPTION,
                                                       value.GetDescription());
        }
        #endregion

        public RenewHomePolicy(Browser browser) : base(browser)
        {
        }

        public override bool IsDisplayed()
        {
            var rendered = false;
            try
            {
                GetElement(XP_UPDATEHOMEPOLICY_CONTINUE_BUTTON);
                GetElement(XP_BUILDING_TYPE);
                GetElement(XP_ALARM_TYPE);
                Reporting.LogPageChange("Renew home policy page");
                rendered = true;
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }

        /// <summary>
        /// If the weekly rent text field is present, then enter the weekly rent and the property manager details
        /// Click 'Continue' button in the 'Renew your policy' accordion
        /// </summary>
        /// <param name="testData"></param>
        public void UpdateHomePolicyDetails(EndorseHome testData)
        {
            if (IsWeeklyRentalAmountPresent())
            {
                WaitForTextFieldAndEnterText(XP_WEEKLY_RENTAL_AMOUNT, testData.WeeklyRent, false);
                Thread.Sleep(500);
                SendKeyPressesToField(XP_WEEKLY_RENTAL_AMOUNT, Keys.Tab);
                Thread.Sleep(500);
                WaitForSelectableAndPickFromDropdown(XP_PROPERTY_MANAGEMENT, XP_PROPERTY_MANAGEMENT_OPTIONS, testData.HomePropertyManager.ToString());
            }

            if (Config.Get().IsCycloneEnabled() &&
                testData.NewAssetValues.IsACycloneAddress)
            {
                IsPropertyElevated = testData.NewAssetValues.IsPropertyElevated;
                HasCycloneWindowShutters = testData.NewAssetValues.IsCycloneShuttersFitted;

                if ((testData.NewAssetValues.YearBuilt == 0 && testData.OriginalPolicyData.HomeAsset.ConstructionYear < 2012) ||
                    (testData.NewAssetValues.YearBuilt != 0 && testData.NewAssetValues.YearBuilt < 2012))
                {
                    GarageDoorStatus = testData.NewAssetValues.GarageDoorsCycloneStatus;
                }

                if ((testData.NewAssetValues.YearBuilt == 0 && testData.OriginalPolicyData.HomeAsset.ConstructionYear < 1982) ||
                    (testData.NewAssetValues.YearBuilt != 0 && testData.NewAssetValues.YearBuilt < 1982))
                {
                    RoofImprovements = testData.NewAssetValues.RoofImprovementCycloneStatus;
                }
            }

            Reporting.Log("Policy details updated before submitting for final premium", _browser.Driver.TakeSnapshot());

            ClickControl(XP_UPDATEHOMEPOLICY_CONTINUE_BUTTON);

            using (var spinner = new RACSpinner(_browser))
            {
                spinner.WaitForSpinnerToFinish();
            }

        }

        /// <summary>
        /// Verify Premium Label Text, Verify Original Premium Amount, Enter 'Email' and Get the new Premium
        /// Click 'Confirm' button in the 'Your annual premium' accordion
        /// </summary>
        /// <param name="testData"></param>
        public decimal UpdateCoversAndGetPremium(Browser browser, EndorseHome testData)
        {
            Reporting.AreEqual("Annual premium", GetInnerText(XP_PREMIUM_LABEL_TEXT).StripLineFeedAndCarriageReturns(), "Verifying Premium Label Text");

            // We have been forced to provide answers to mandatory cyclone questions.
            // So the premium will not match what Shield initially provided.
            if (!testData.AreWeChangingCycloneAnswers())
            {
                Reporting.AreEqual(testData.OriginalPolicyData.AnnualPremium.Total, decimal.Parse(GetValue(XP_PREMIUM_AMOUNT_TEXT).StripMoneyNotations()), "Verifying Current Premium Amount Text");

                if (testData.PayMethod.PaymentFrequency == PaymentFrequency.Monthly)
                {
                        Reporting.AreEqual(testData.OriginalPolicyData.NextPendingInstallment().Amount.Total, decimal.Parse(DataHelper.SplitStringAndReturnAnElementFromArray(GetInnerText(XP_INSTALMENT_AMOUNT_TEXT), char.Parse(" "), 0).StripMoneyNotations()), "Verifying Instalment Amount");
                }
            }
            else
            {
                Reporting.Log("NOTE: Not all cyclone questions were pre-filled, so premium has changed with given answers.");
            }

            Email = testData.PayMethod.Payer.GetEmail();

            decimal newPremium = decimal.Parse(GetValue(XP_PREMIUM_AMOUNT_TEXT).StripMoneyNotations());

            Reporting.Log("About to Confirm the cover changes and accept the new Premium.", browser.Driver.TakeSnapshot());

            ClickControl(XP_UPDATECOVER_CONFIRM_BUTTON);

            using (var spinner = new RACSpinner(browser))
            { spinner.WaitForSpinnerToFinish(); }

            return newPremium;
        }

        /// <summary>
        /// Verify the Payment options radio buttons in 'Payment details' page
        /// </summary>
        /// <returns></returns>
        public void VerifyPaymentOptionsRadioButtons(PaymentOptions paymentOption)
        {
            var radioButtonPayNow = $"{XP_PAYMENT_OPTIONS_RADIO_BUTTON_GROUP}{XPR_RADIO_PAYNOW}/../span[contains(@class,'rb-radio')]";
            var radioButtonPayLater = $"{XP_PAYMENT_OPTIONS_RADIO_BUTTON_GROUP}{XPR_RADIO_PAYLATER}/../span[contains(@class,'rb-radio')]";
            if (paymentOption == PaymentOptions.PayNow)
            {
                Reporting.IsTrue(GetClass(radioButtonPayNow).Contains("checked"), "Pay now radio button is checked.");
                Reporting.IsFalse(GetClass(radioButtonPayLater).Contains("checked"), "Pay later radio button is not checked.");
            }
            else if (paymentOption == PaymentOptions.PayLater)
            {
                Reporting.IsFalse(GetClass(radioButtonPayNow).Contains("checked"), "Pay now radio button is not checked.");
                Reporting.IsTrue(GetClass(radioButtonPayLater).Contains("checked"), "Pay later radio button is checked.");
            }
            else
            {
                Reporting.Error("Payment option not supported");
            }
        }

        /// <summary>
        /// Checks to see if a payment is needed (Annual Cash only), and if
        /// so it navigates the Westpac credit card portal.
        /// 
        /// All other payment options will just fall through to the confirmation
        /// page.
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="expectedAmount"></param>
        public void HandlePaymentPrompt(Payment paymentDetails, decimal expectedAmount)
        {
            // Only Annual Cash policies will need to provide their credit card details
            // in order to finish renewing. All others will use details on file.
            if (paymentDetails.IsPaymentByBankAccount ||
                paymentDetails.IsMonthly)
                return;

            _driver.WaitForElementToBeVisible(By.XPath(XP_PAYMENT_AMOUNT_TEXT), WaitTimes.T30SEC);
            _driver.WaitForElementToBeVisible(By.XPath(XP_WESTPAC_PAY_NOW_BUTTON), WaitTimes.T5SEC);

            using (var westpacIframe = new WestpacQuickStream(_browser))
            using (var spinner       = new RACSpinner(_browser))
            {
                VerifyUIElementsInPaymentDetailsAccordionAreCorrect(expectedAmount);
                westpacIframe.EnterCardDetails(paymentDetails);
                Reporting.IsTrue(IsPayNowButtonEnabled, "Pay now button".IsEnabled());

                ClickControl(XP_WESTPAC_PAY_NOW_BUTTON);
                spinner.WaitForSpinnerToFinish();
            }
        }

        /// <summary>
        /// Checks if the Weekly Rent Text Field is Present in the screen
        /// </summary>
        /// <returns>true/false</returns>
        public bool IsWeeklyRentalAmountPresent()
        {
            IWebElement element;
            if (_driver.TryFindElement(By.XPath(XP_WEEKLY_RENTAL_AMOUNT), out element))
            {
                return element.Displayed;
            }
            return false;
        }

    }
}