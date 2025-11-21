using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace UIDriver.Pages.Spark.CaravanQuote
{
    public class HeresYourQuote : SparkBasePage
    {
        #region XPATHS
        private class XPath
        {
            public static class General
            {
                public const string QuoteNumber = "//strong[@data-testid='heading-quote-number']";

                public const string Header = FORM + "//h3[contains(text(),'Here')  and contains(text(),'s your quote')]";
                public const string GetPolicy = "//button[@id='button-caravan-get-quote'][1]";
                public static class Help
                {
                    public static class Button
                    {
                        public const string Open = "//button[@id='whatsIncludedTooltip-Button']";
                        public const string Close = "//h2[@id='Cover summary-cover-dialogue']/button";
                    }
                    public const string Dialog = "//div[@aria-labelledby='Cover summary-cover-dialogue']";
                    public const string Sub = Dialog + "//h2[text()='Relax with local caravan insurance you can rely on']";
                    public const string C1 = Dialog + "//p[text()='Cover for accidental damage, malicious damage, fire, theft, storm and flood.']";
                    public const string C2 = Dialog + "//p[text()='Interstate and off-road cover.']";
                    public const string C3 = Dialog + "//p[text()='Cover for personal items (including sports equipment and electronic devices) when inside or attached to your caravan or annexe.']";
                    public const string C4 = Dialog + "//p[text()='Local claims team, available seven days a week.']";
                    public const string C5 = Dialog + "//p[text()='Cover for anyone towing your caravan.']";
                    public const string C6 = Dialog + "//p[text()=\"Towing your caravan to an approved repairer or safe location if it isn't safe to use.\"]";
                    public const string C7 = Dialog + "//p[text()='Cover up to $3000 for your annexe.']";
                    public const string C8 = Dialog + "//p[text()=\"If your caravan is written off and it's less than two years old, and you're the first registered owner, we'll replace it with a new caravan.\"]";
                    public const string C9 = Dialog + "//p[text()='And much more!']";
                }
            }
            public static class Discount
            {
                public const string CaravanMemberDiscount = "//span[@id='discount-Caravan-0']"; // Displays the membership discount if applicable
                public const string CaravanOnlineDiscount = "//span[contains(text(),'online discount')]";
                public const string ZeroExcess = "id('discount-caravan-excess')";
            }
            public static class PremiumBreakdown
            {
                public const string Link = "//a[@id='link-caravan-premium-breakdown']";
                public const string Basic = "//span[@id='caravan-premium-breakdown-basic']";
                public const string Stamp = "//span[@id='caravan-premium-breakdown-government-charges']";
                public const string GST = "//span[@id='caravan-premium-breakdown-gst']";
            }
            public static class Excess
            {
                public const string Label = "id('excess-label')";
                public const string Input = "id('excess-dropdown')";
                public const string Options = "//ul[@aria-labelledby='excess-label']//li[@role='option']";

                public static class Help
                {
                    public const string ButtonOpen = "//button[@data-testid='excess-tooltip-idButton']";
                    public const string ButtonClose = "//button[@aria-label='close']";

                    public const string Title    = "id('excess-dialog-title')";
                    public const string Section1 = "//div[@aria-labelledby='excess-dialog-title']/div/div[1]";
                    public const string Section2 = "//div[@aria-labelledby='excess-dialog-title']/div/div[2]";
                    public const string Section3 = "//div[@aria-labelledby='excess-dialog-title']/div/div[3]";
                    public const string Section4 = "//div[@aria-labelledby='excess-dialog-title']/div/div[4]";
                    public const string LinkPDS  = "//a[@id='excess-dialog-link']";
                }
            }
            public static class SumInsured
            {
                public const string Label = "//div[@id='label-caravan-sumInsured']/div[2]";
                public const string Input = "//input[@id='input-caravan-sumInsured']";
                public static class Help
                {
                    public static class Button
                    {
                        public const string Open = "//button[@id='chooseAgreedValueTooltipButton']";
                        public const string Close = "//button[@id='chooseAgreedValueTooltip-close']";
                    }
                    public const string Text = "//p[text()=\"The agreed value is the amount we agree to insure your caravan for. It includes accessories, GST, rego and any other 'on-road' costs. If you adjust the agreed value your policy premium will change.\"]";
                }
            }
            public static class Contents
            {
                public const string Label = "//div[@id='label-caravan-contents']/div[2]";
                public const string Input = "//div[@id='select-caravan-contents']";
                public const string Options = "/html/body/div[@id='menu-contents']//li[@role='option']";
                public static class Help
                {
                    public static class Button
                    {
                        public const string Open = "//button[@id='chooseContentCoverTooltipButton']";
                        public const string Close = "//button[@id='chooseContentCoverTooltip-close']";
                    }
                    public const string P1 = "//p[text()='We provide $1000 of complimentary contents cover against loss or damage caused by fire, storm, collision, theft or malicious damage.']";
                    public const string P2 = "//p[text()='Contents includes clothing, personal belongings, furniture, sports equipment and electrical devices when inside or attached to your caravan or annexe.']";
                    public const string P3 = "//p[text()='For an additional premium, you can increase your cover up to $15,000.']";
                }
            }
            public static class Email
            {
                public const string ShowHideForm = "//button[@id='button-caravan-toggle-email-quote-form']";
                public const string Input = "//input[@id='email']";
                public const string SendButton = "//button[@id='button-caravan-email-quote']/span";
            }

            public static class PopUp
            {
                public const string Dialog = "//div[@aria-labelledby='formotiv-popup-dialog']";
                public const string Header = Dialog + "//h2[@id='formotiv-popup-dialog' and text() = 'Did you know?']";
                public const string Message = Dialog + "//strong[text()=\"We'll cover your caravan or camper trailer throughout Australia.\"]";
                public const string CloseButton = Dialog + "//button[@id='btn-close']";
            }

            public const string CaravanOnlineDiscountFooterText = "//strong[text()='*Discount applies to new policyholders for the first year of insurance only and is subject to a minimum premium.']";
        }
        #endregion

        #region Constants
        private class PageConstants
        {
            public class ExcessHelp
            {
                public const string PDSLink  = "https://([a-zA-Z]+(\\.[a-zA-Z]+)+)/products/insurance/policy-documents/caravan-trailer-insurance";
                public const string Section1 = "The excess is the amount you may need to pay towards settlement of any claim.";
                public const string Section2 = "If you adjust your excess, your premium will change.";
                public const string Section3 = "Extra excesses may apply:\r\nDriver under 19: $650\r\nDriver under 21: $550\r\nDriver under 24: $450\r\nDriver under 26: $300\r\nSpecial excess: will be stated in your policy documents if applicable";
                public const string Section4 = "See the Premium, Excess and Discount Guide for more information.";
            }
        }
        #endregion

        public HeresYourQuote(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.Header);
                GetElement(XPath.General.QuoteNumber);
                GetElement(XP_PAYMENT_AMOUNT_QUOTE);
                GetElement(XP_PAYMENT_FREQUENCY_ANNUAL);
                GetElement(XP_PAYMENT_FREQUENCY_MONTHLY);
                GetElement(XPath.PremiumBreakdown.Link);
                GetElement(XPath.SumInsured.Input);
                GetElement(XPath.Contents.Input);
                GetElement(XPath.General.GetPolicy);
                GetElement(XPath.Email.ShowHideForm);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Caravan Quote page - Heres Your Quote");
            return true;
        }

        #region Settable properties and controls

        private int? Excess
        {
            get => int.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.Excess.Input)));

            set
            {
                if (value == Excess)
                { return; }   // Requested value is already set. Nothing to do.

                WaitForSelectableAndPickFromDropdown(XPath.Excess.Input, XPath.Excess.Options, DataHelper.ConvertIntToMonetaryString(value.Value, applyThousandsSeparator: false));

                using (var spinner = new SparkSpinner(_browser))
                    spinner.WaitForSpinnerToFinish();
            }
        }

        private int SumInsured
        {
            get => int.Parse(DataHelper.StripMoneyNotations(GetValue(XPath.SumInsured.Input)));
            set => WaitForTextFieldAndEnterText(XPath.SumInsured.Input, value.ToString());
        }

        private int ContentsSumInsured
        {
            get => int.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.Contents.Input)));
            set => WaitForSelectableAndPickFromDropdown(XPath.Contents.Input, XPath.Contents.Options, DataHelper.ConvertIntToMonetaryString(value, minValueForThousandsSeparator: 10000));
        }

        private string QuoteNumber => GetInnerText(XPath.General.QuoteNumber);

        private string Email
        {
            get => GetInnerText(XPath.Email.Input);
            set => WaitForTextFieldAndEnterText(XPath.Email.Input, value, hasTypeAhead: false);
        }
        #endregion

        /// <summary>
        /// Supports Spark version of caravan
        /// Verify static text labels in the quote page
        /// </summary>
        /// <param name="quoteCaravan"></param>
        public void VerifyQuotePageLabels(QuoteCaravan quoteCaravan)
        {
            string quoteNumberText = GetInnerText($"{XPath.General.QuoteNumber}/..");
            Reporting.AreEqual("Your quote number is", quoteNumberText.Remove(quoteNumberText.LastIndexOf(' ')).TrimEnd(), "Quote number label display");
            Reporting.AreEqual("See quote breakdown", GetInnerText(XPath.PremiumBreakdown.Link), "Quote breakdown link display");

            Reporting.AreEqual("Agreed value", GetInnerText(XPath.SumInsured.Label), "Agreed value display");
            Reporting.AreEqual(DataHelper.ConvertIntToMonetaryString(CARAVAN_DEFAULT_CONTENT_INSURANCE_VALUE, minValueForThousandsSeparator: 10000), GetInnerText(XPath.Contents.Input), "Contents sum agreed value defaults to the correct value");
            Reporting.AreEqual("Contents cover", GetInnerText(XPath.Contents.Label), "Contents cover display");

            ValidateHelpText();
            ValidateExcessLabel(quoteCaravan);
            ValidateMembershipDiscount(quoteCaravan);
            ValidateWhatsIncludedText();
        }

        private void ValidateHelpText()
        {
            VerifyWhatsIncludedHelpText();
            VerifyAgreedValueHelpText();
            VerifyContentCoverHelpText();
        }

        public void CloseOffRoadCoverPopupIfDisplayed()
        {
            // Formotive can have a delay in appearing.
            if (_driver.TryWaitForElementToBeVisible(By.XPath(XPath.PopUp.Dialog), Constants.General.WaitTimes.T5SEC, out IWebElement dialogPanel))
            {
                Reporting.IsTrue(IsControlDisplayed(XPath.PopUp.Header), "'Here's your quote' page: 'Formotiv' text: Did you know? should display");
                Reporting.IsTrue(IsControlDisplayed(XPath.PopUp.Message), "'Here's your quote' page: 'Formotiv' text: We'll cover your caravan or camper trailer throughout Australia... should display");
                ClickControl(XPath.PopUp.CloseButton);
            }
        }

        private void ValidateWhatsIncludedText()
        {
            Reporting.IsTrue(IsControlDisplayed("//p[text()='Cover for accidental damage, malicious damage, fire, theft, storm and flood.']"), "'Here's your quote' page: 'What's included' text: Cover for accidental damage... display");
            Reporting.IsTrue(IsControlDisplayed("//p[text()='Interstate and off-road cover.']"), "'Here's your quote' page: 'What's included' text: Interstate and off-road... display");
            Reporting.IsTrue(IsControlDisplayed("//p[text()='Cover for personal items (including sports equipment and electronic devices) when inside or attached to your caravan or annexe.']"), "'Here's your quote' page: 'What's included' text: Cover for personal items... display");
            Reporting.IsTrue(IsControlDisplayed("//p[text()='Local claims team, available seven days a week.']"), "'Here's your quote' page: 'What's included' text: Local claims team, available display");
            Reporting.IsTrue(IsControlDisplayed("//p[text()='Cover for anyone towing your caravan.']"), "'Here's your quote' page: 'What's included' text: Local claims team... display");
            Reporting.IsTrue(IsControlDisplayed("//p[text()=\"Towing your caravan to an approved repairer or safe location if it isn't safe to use.\"]"), "'Here's your quote' page: 'What's included' text: Towing your caravan... display");
            Reporting.IsTrue(IsControlDisplayed("//p[text()='Cover up to $3000 for your annexe.']"), "'Here's your quote' page: 'What's included' text: Cover up to... display");
            Reporting.IsTrue(IsControlDisplayed("//p[text()=\"If your caravan is written off and it's less than two years old, and you're the first registered owner, we'll replace it with a new caravan.\"]"), "'Here's your quote' page: 'What's included' text: If your caravan... display");
            Reporting.IsTrue(IsControlDisplayed("//strong[text()='Please remember that this is a quote only and does not provide cover. The final information you provide to us may affect the terms and premium we offer.']"), "'Here's your quote' page: 'What's included' text: Please remember that this is a quote only... display");
        }

        private void ValidateExcessLabel(QuoteCaravan quoteCaravan)
        {
            Reporting.IsTrue(IsControlDisplayed(XPath.Excess.Input), "Excess field display");
            Reporting.AreEqual("Basic excess", GetInnerText(XPath.Excess.Label), "Choose your basic excess label is displayed");
            VerifyExcessHelpText();
        }

        /// <summary>
        /// Checks if the member is eligible for member discount then correct membership discount is shown. Else, no member discount is shown.
        /// Also, checks elements showing Online Discount information are not displayed.
        /// Caravan NB - remove online discount (UI) https://rac-wa.atlassian.net/browse/SPK-6098
        /// </summary>
        /// <param name="quoteCaravan"></param>
        private void ValidateMembershipDiscount(QuoteCaravan quoteCaravan)
        {
            var mainPH = quoteCaravan.PolicyHolders[0];
            if (IsEligibleForDiscount(mainPH.MembershipTier) && !mainPH.SkipDeclaringMembership)
            {
                var mainPHTier = mainPH.MembershipTier;
                var expectedMembershipDiscountLabel = $"{MemberDiscountMappings[mainPHTier].Caravan} {mainPHTier.GetDescription()} member discount";

                Reporting.AreEqual(expectedMembershipDiscountLabel, GetInnerText(XPath.Discount.CaravanMemberDiscount), $"Expected: {expectedMembershipDiscountLabel} Actual: {GetInnerText(XPath.Discount.CaravanMemberDiscount)}");
            }
            else
            {
                Reporting.IsFalse(_driver.TryFindElement(By.XPath(XPath.Discount.CaravanMemberDiscount), out IWebElement memberDiscount), "Member Discount should not be shown.");
            }

            Reporting.IsFalse(_driver.TryFindElement(By.XPath(XPath.Discount.CaravanOnlineDiscount),
                              out IWebElement onlineDiscount), "Caravan online Discount should not be shown on the top of screen");
            Reporting.IsFalse(_driver.TryFindElement(By.XPath(XPath.CaravanOnlineDiscountFooterText),
                               out IWebElement onlineDiscountFooterText), "Online Discount footer text should not be shown");
        }

        /// <summary>
        /// Supports Spark version of caravan
        /// Change Excess if the option is available for the user.
        /// </summary>
        /// <param name="quoteCaravan"></param>
        public void ChangeExcess(QuoteCaravan quoteCaravan)
        {
            if (quoteCaravan.Excess.HasValue)
            {
                Excess = quoteCaravan.Excess;
            }
        }

        /// <summary>
        /// Supports Spark version of caravan
        /// Change agreed value based on the given percentage increase/decrease (InsuredVariance from quote input)
        /// However, the new agreed value you get after applying the percentage increase/decrease,
        /// is subjected to be cut-off at a different discount level, based on the minimum and maximum
        /// allowed insurable limits of the application.
        /// </summary>
        /// <param name="marketValue"></param>
        /// <param name="insuredVariance"></param>
        public void ChangeInsuredValueBasedOnPercentage(int marketValue, int insuredVariance)
        {
            if (insuredVariance == 0)
            { return; }

            int newSumInsured = SumInsured + (SumInsured * insuredVariance / 100);

            newSumInsured = DataHelper.RoundUpOrDownToNearestHundred(newSumInsured, insuredVariance);

            // MIN bound check:
            var finalDesiredSI = newSumInsured < CARAVAN_MIN_SUM_INSURED_VALUE ? CARAVAN_MIN_SUM_INSURED_VALUE : newSumInsured;

            finalDesiredSI = newSumInsured > CARAVAN_MAX_SUM_INSURED_VALUE ? CARAVAN_MAX_SUM_INSURED_VALUE : newSumInsured;

            SumInsured = finalDesiredSI;

            using (var spinner = new SparkSpinner(_browser))
                spinner.WaitForSpinnerToFinish();

            Reporting.Log("Quote Page: Changed Agreed value", _browser.Driver.TakeSnapshot());
        }

        /// <summary>
        /// Supports Spark version of caravan
        /// Change Contents Sum Agreed value based on the given value. ($1000 to $15,000 allowed)
        /// </summary>
        /// <param name="amount"></param>
        public void ChangeContentsInsuredValue(int amount)
        {
            if (amount == CARAVAN_DEFAULT_CONTENT_INSURANCE_VALUE)
                return;

            ContentsSumInsured = amount;

            using (var spinner = new SparkSpinner(_browser))
                spinner.WaitForSpinnerToFinish();

            Reporting.Log("Quote Page: Changed Contents Agreed value", _browser.Driver.TakeSnapshot());
        }

        /// <summary>
        /// Supports Spark version of caravan
        /// This method is used to:
        /// 1. Save quote details such as 'quote number', 'premium details' and 'quote breakdown'
        /// 2. Verifies that annualized quote breakdown for Monthly payment frequency is,
        /// greater than or equal to Annual values.
        /// 3. Annual premium is greater than the Monthly premium
        /// </summary>
        /// <param name="quoteCaravan"></param>
        public void SaveQuoteDetailsAndComparePremiums(QuoteCaravan quoteCaravan)
        {
            var quoteData = new QuoteData();

            quoteData.QuoteNumber = QuoteNumber;

            PaymentFrequency = PaymentFrequency.Annual;

            quoteData.AnnualPremium = QuoteAmount;

            ClickPremiumBreakdownLink(); //Expand quote breakdown

            var premiumBreakdownBasicAnnual = GetPremiumBreakdownBasic();
            var premiumBreakdownStampAnnual = GetPremiumBreakdownStamp();
            var premiumBreakdownGSTAnnual = GetPremiumBreakdownGST();

            PaymentFrequency = PaymentFrequency.Monthly;

            quoteData.MonthlyPremium = QuoteAmount;

            var premiumBreakdownBasicMonthly = GetPremiumBreakdownBasic();
            var premiumBreakdownStampMonthly = GetPremiumBreakdownStamp();
            var premiumBreakdownGSTMonthly = GetPremiumBreakdownGST();

            //A new quote version is not created in Shield, when we select "Monthly" in this page.
            //This makes it difficult to verify the Annualized premium breakdown for Monthly payment frequency on this Page.
            //As a workaround we perform the following verification on this page.
            bool areMonthlyPremiumBreakdownTextsGreaterOrEqual = ((premiumBreakdownBasicAnnual <= premiumBreakdownBasicMonthly)
                && (premiumBreakdownStampAnnual <= premiumBreakdownStampMonthly) && (premiumBreakdownGSTAnnual <= premiumBreakdownGSTMonthly));
            Reporting.IsTrue(areMonthlyPremiumBreakdownTextsGreaterOrEqual, "Premium Breakdown texts for Monthly payment frequency, are greater than or equal to the Annual values");

            //A new quote version is not created in Shield, when we select "Monthly" in this page.
            //This makes it difficult to verify the Monthly premium value on this Page.
            //As a workaround we perform the following verification on this page.
            Reporting.IsTrue(quoteData.AnnualPremium > quoteData.MonthlyPremium, "Annual Premium is greater than the Monthly Premium");

            PaymentFrequency = quoteCaravan.PayMethod.PaymentFrequency;

            quoteData.PremiumBreakdownBasic = GetPremiumBreakdownBasic();
            quoteData.PremiumBreakdownStamp = GetPremiumBreakdownStamp();
            quoteData.PremiumBreakdownGST = GetPremiumBreakdownGST();

            ClickPremiumBreakdownLink(); //Collapse quote breakdown

            quoteCaravan.Excess = Excess;

            quoteCaravan.SumInsuredValue = SumInsured;
            quoteCaravan.ContentsSumInsured = ContentsSumInsured;
            quoteCaravan.QuoteData = quoteData;

            LogQuoteDataInToTestReport(quoteCaravan, quoteData);

            Reporting.Log("Quote Page: Saved Quote details", _browser.Driver.TakeSnapshot());
        }

        public void ClickPremiumBreakdownLink()
        {
            ClickControl(XPath.PremiumBreakdown.Link);
        }

        /// <summary>
        /// Used to get Basic premium for any payment frequency
        /// </summary>
        /// <returns></returns>
        public decimal GetPremiumBreakdownBasic()
        {
            return decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PremiumBreakdown.Basic)));
        }

        /// <summary>
        /// Used to get Stamp duty value for any payment frequency
        /// </summary>
        /// <returns></returns>
        public decimal GetPremiumBreakdownStamp()
        {
            return decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PremiumBreakdown.Stamp)));
        }

        /// <summary>
        /// Used to get GST value for any payment frequency
        /// </summary>
        /// <returns></returns>
        public decimal GetPremiumBreakdownGST()
        {
            return decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PremiumBreakdown.GST)));
        }

        public void ClickGetPolicy()
        {
            ClickControl(XPath.General.GetPolicy);
        }

        /// <summary>
        /// Logging out quote data for easier debugging and auditing.
        /// </summary>
        private static void LogQuoteDataInToTestReport(QuoteCaravan quoteCaravan, QuoteData quoteData)
        {
            Reporting.Log(quoteData.ToString());
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine($"    Excess:    {quoteCaravan.Excess.Value}<br>");
            formattedString.AppendLine($"    Caravan Sum Insured:    {quoteCaravan.SumInsuredValue}<br>");
            formattedString.AppendLine($"    Contents Sum Insured:    {quoteCaravan.ContentsSumInsured}<br>");
            formattedString.AppendLine("-------------------------------------<br>");
            Reporting.Log(formattedString.ToString());
        }

        private void VerifyWhatsIncludedHelpText()
        {
            //Click to open the what's included help text box
            ClickControl(XPath.General.Help.Button.Open);

            //To wait for the popup to appear before verifying
            WaitForPage(2);
            Reporting.IsTrue(IsControlDisplayed(XPath.General.Help.Sub), $"EXPECTED : 'Relax with local caravan insurance you can rely on' ACTUAL: {GetInnerText(XPath.General.Help.Sub)}");

            //what's included checklist
            Reporting.IsTrue(IsControlDisplayed(XPath.General.Help.C1), "CHECK #1: EXPECTED: 'Cover for accidental damage, malicious damage, fire, theft, storm and flood.'");
            Reporting.IsTrue(IsControlDisplayed(XPath.General.Help.C2), "CHECK #2: EXPECTED: 'Interstate and off-road cover.'");
            Reporting.IsTrue(IsControlDisplayed(XPath.General.Help.C3), "CHECK #3: EXPECTED: 'Cover for personal items (including sports equipment and electronic devices) when inside or attached to your caravan or annexe.'");
            Reporting.IsTrue(IsControlDisplayed(XPath.General.Help.C4), "CHECK #4: EXPECTED: 'Local claims team, available seven days a week.'");
            Reporting.IsTrue(IsControlDisplayed(XPath.General.Help.C5), "CHECK #5: EXPECTED: 'Cover for anyone towing your caravan.");
            Reporting.IsTrue(IsControlDisplayed(XPath.General.Help.C6), "CHECK #6: EXPECTED: 'Towing your caravan to an approved repairer or safe location if it isn't safe to use.'");
            Reporting.IsTrue(IsControlDisplayed(XPath.General.Help.C7), "CHECK #7: EXPECTED: 'Cover up to $3000 for your annexe.'");
            Reporting.IsTrue(IsControlDisplayed(XPath.General.Help.C8), "CHECK #8: EXPECTED: 'If your caravan is written off and it's less than two years old, and you're the first registered owner, we'll replace it with a new caravan.'");
            Reporting.IsTrue(IsControlDisplayed(XPath.General.Help.C9), "CHECK #9: EXPECTED: 'And much more!'");

            //Close the dialog box
            ClickControl(XPath.General.Help.Button.Close);
        }

        /// <summary>
        /// Verifies the content and disclosures for excess
        /// help text pop-up.
        /// </summary>
        private void VerifyExcessHelpText()
        {
            //Click to open the Excess help text box
            Reporting.IsTrue(IsControlDisplayed(XPath.Excess.Help.ButtonOpen), "Excesses help icon must be visible on screen");
            ClickControl(XPath.Excess.Help.ButtonOpen);
            //To wait for the popup to appear before verifying
            WaitForPage(2);
            ScrollElementIntoView(XPath.Excess.Help.Title);

            Reporting.IsTrue(IsControlDisplayed(XPath.Excess.Help.Title), $"EXPECTED: Excesses ACTUAL: {GetInnerText(XPath.Excess.Help.Title)}");
            Reporting.AreEqual(PageConstants.ExcessHelp.Section1, GetInnerText(XPath.Excess.Help.Section1), "paragraph 1 of Excess help text");
            Reporting.AreEqual(PageConstants.ExcessHelp.Section2, GetInnerText(XPath.Excess.Help.Section2), "paragraph 2 of Excess help text");
            Reporting.AreEqual(PageConstants.ExcessHelp.Section3, GetInnerText(XPath.Excess.Help.Section3), "paragraph 3 of Excess help text");
            Reporting.AreEqual(PageConstants.ExcessHelp.Section4, GetInnerText(XPath.Excess.Help.Section4), "paragraph 4 of Excess help text");

            var expectedURLRegex = new Regex(PageConstants.ExcessHelp.PDSLink);
            var actualURL = GetElement($"{XPath.Excess.Help.Section4}//a").GetDomProperty("href");
            var match = expectedURLRegex.Match(actualURL);
            Reporting.IsTrue(match.Success, $"PED Guide link on Here Your Quote page: {actualURL}");

            //Close the dialog box
            ClickControl(XPath.Excess.Help.ButtonClose);
        }

        private void VerifyAgreedValueHelpText()
        {

            //Click to open the what's included help text box
            ClickControl(XPath.SumInsured.Help.Button.Open);
            //To wait for the popup to appear before verifying
            WaitForPage(2);
            Reporting.IsTrue(IsControlDisplayed(XPath.SumInsured.Help.Text), "EXPECTED: 'The agreed value is the amount we agree to insure your caravan for. It includes accessories, GST, rego and any other 'on-road' costs. If you adjust the agreed value your policy premium will change.'");

            //Close the dialog box
            ClickControl(XPath.SumInsured.Help.Button.Close);
        }

        private void VerifyContentCoverHelpText()
        {

            //Click to open the what's included help text box
            ClickControl(XPath.Contents.Help.Button.Open);
            //To wait for the popup to appear before verifying
            WaitForPage(2);
            Reporting.IsTrue(IsControlDisplayed(XPath.Contents.Help.P1), $"EXPECTED: We provide $1000 of complimentary contents cover against loss or damage caused by fire, storm, collision, theft or malicious damage.");
            Reporting.IsTrue(IsControlDisplayed(XPath.Contents.Help.P2), $"EXPECTED: Contents includes clothing, personal belongings, furniture, sports equipment and electrical devices when inside or attached to your caravan or annexe.");
            Reporting.IsTrue(IsControlDisplayed(XPath.Contents.Help.P3), $"EXPECTED: For an additional premium, you can increase your cover up to $15,000.");

            //Close the dialog box
            ClickControl(XPath.Contents.Help.Button.Close);
        }

        public void VerifyQuoteValuesAfterRetrieve(QuoteCaravan quoteCaravan, Browser browser)
        {
            Reporting.Log($"Quote {quoteCaravan.QuoteData.QuoteNumber} is retrieved.", browser.Driver.TakeSnapshot());

            Reporting.AreEqual(quoteCaravan.QuoteData.QuoteNumber, QuoteNumber, $"Quote Number > Expected: {quoteCaravan.QuoteData.QuoteNumber} Actual: {QuoteNumber} ");
            Reporting.AreEqual(quoteCaravan.PayMethod.PaymentFrequency, PaymentFrequency,  $"Frequency > Expected {quoteCaravan.PayMethod.PaymentFrequency} Actual {PaymentFrequency}");
            Reporting.AreEqual(quoteCaravan.SumInsuredValue, SumInsured, $"Sum Insured > Expected: {quoteCaravan.SumInsuredValue}  Actual: {SumInsured}");
            Reporting.AreEqual(quoteCaravan.ContentsSumInsured, ContentsSumInsured, $"Content Insured > Expected: {quoteCaravan.ContentsSumInsured} Actual: {ContentsSumInsured}");

            ValidateMembershipDiscount(quoteCaravan);
            ValidateExcessLabel(quoteCaravan);
            ValidatePremiumAmount(quoteCaravan);
        }

        private void ValidatePremiumAmount(QuoteCaravan quoteCaravan)
        {
            if (PaymentFrequency == PaymentFrequency.Annual)
            { Reporting.AreEqual(quoteCaravan.QuoteData.AnnualPremium, QuoteAmount, $"Payment Amount should be {quoteCaravan.QuoteData.AnnualPremium}");} 
            else
            { Reporting.AreEqual(quoteCaravan.QuoteData.MonthlyPremium, QuoteAmount, $"Payment Amount should be {quoteCaravan.QuoteData.MonthlyPremium}");}
        }

        public void SendEmailQuote(QuoteCaravan quoteCaravan)
        {
            ClickControl(XPath.Email.ShowHideForm);
            Email = quoteCaravan.PolicyHolders[0].PrivateEmail.Address;
            ClickControl(XPath.Email.SendButton);
        }

        /// <summary>
        /// The dynamic CSS that needs to be ignored from visual testing.
        /// </summary>
        public List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
              "#heading-quote-number", // Quote number in header
              "#label-payment-amount", // premium amount
              "div[class*='css-dsartl']", // "paying annually is cheaper..." text
              "#excess-dropdown",
              "#input-caravan-sumInsured"
          };
    }
}