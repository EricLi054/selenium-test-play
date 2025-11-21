using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Globalization;
using System.Threading;
using UIDriver.Pages.Spark;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;


public class SparkBoatYourQuote : SparkBasePage
{
    #region XPATHS
    private class XPath
    {
        public const string PageHeader = "//*[@id='header']";
        public const string ActiveStepper = "//button[@aria-selected='true']";
        public const string QuoteNumberPreface  = "//span[contains(text(),'Your quote number is')]";
        public const string QuoteNumber         = "id('quoteNumber')";
        public const string BoatOnlineDiscount  = "id('discount-0')";
        public const string TermsParagraph1     = "id('terms-1')";
        public const string TermsParagraph2     = "id('terms-2')";
        public const string TermsParagraph3     = "id('terms-3')";
        public class Field
        {
            public const string Value           = "id('agreed-value-input')";
            public const string BasicExcess     = "id('excess-input')";
            public const string ExcessOptions   = "//ul[@role='listbox']" + "//li";
            public const string EmailAddress    = "id('email')";
        }
        public class Button
        {
            public const string FlotationOption                 = "id('flotation-cover')";
            public const string RacingOption                    = "id('racing-cover')";
            public const string ExcessHelpTextButton            = "//button[@data-testid='excess-tooltip']";
            public const string ExcessHelpTextClose             = "id('excess-tooltip-close')";
            public const string AgreedValueHelpTextButton       = "//button[@data-testid='agreed-value-tooltip']";
            public const string AgreedValueHelpTextClose        = "id('agreed-value-tooltip-close')";
            public const string FlotationOptionHelpTextButton   = "//button[@data-testid='flotation-tooltip']";
            public const string FlotationOptionHelpTextClose    = "id('flotation-tooltip-close')";
            public const string RacingOptionHelpTextButton      = "//button[@data-testid='racing-tooltip']";
            public const string RacingOptionHelpTextClose       = "id('racing-tooltip-close')";
            public const string Yes                             = "//button[@aria-label='Yes']";
            public const string No                              = "//button[@aria-label='No']";
            public const string ShowEmailQuote                  = "id('button-toggle-email-quote-form')";
            public const string SendEmailQuote                  = "id('button-email-quote')";
            public const string NextPage                        = FORM + "//button[@type='submit']";
        }
        public class AdviseUser
        { 
            public class Helptext
            {
                public const string ExcessTitle             = "id('excess-tooltip-title')";
                public const string ExcessBody              = "id('excess-tooltip-message')";
                public const string AgreedValueTitle        = "id('agreed-value-tooltip-title')";
                public const string AgreedValueBody         = "id('agreed-value-tooltip-message')"; 
                public const string FlotationOptionTitle    = "id('flotation-tooltip-title')";
                public const string FlotationOptionBody     = "id('flotation-tooltip-message')";
                public const string RacingOptionTitle       = "id('racing-tooltip-title')";
                public const string RacingOptionBody        = "id('racing-tooltip-message')";
            }
            public class FieldValidation
            {
                public const string Value       = "//p[contains(text(),'enter a value')]";
                public const string YesNoToggle = "//p[contains(text(),'select Yes or No')]";
            }
            public class DeclinedCover
            {
                public const string ValueTooHighTitle   = "id('amount-notification-title')";
                public const string ValueTooHighBody    = "id('amount-notification-body-1')";
                public const string ValueTooHighLink    = "id('link-1')";
            }
        }
        public class PaymentFrequency
        {
            public const string SavingsInformationText = "id('payment-frequency-savings-information-text')";
            public const string CurrentPaymentFrequency = "//button[@aria-pressed='true']";
        }
        public class PremiumBreakdown
        {
            public const string ToggleControl       = "id('payment-frequency-premium-breakdown')";
            public const string Basic               = "id('payment-frequency-premium-breakdown-basic')";
            public const string GovernmentCharges   = "id('payment-frequency-premium-breakdown-government-charges')";
            public const string GST                 = "id('payment-frequency-premium-breakdown-gst')";
        }
        public class WhatsIncludedList
        {
            public const string Item1   = "//p[contains(text(),'Cover for accidental damage')]";
            public const string Item2   = "//p[contains(text(),'200 nautical miles')]";
            public const string Item3   = "//p[contains(text(),'personal belongings up to $1500')]";
            public const string Item4   = "//p[contains(text(),'bodily injury or accidental death')]";
            public const string Item5   = "//p[contains(text(),'replacement cover.')]";
            public const string Item6   = "//p[contains(text(),'available seven days a week.')]";
            public const string Item7   = "//p[contains(text(),'more!')]";
            public const string Summary = "//p[contains(text(),'This is a summary. See the')]";
            public const string PdsLink = "//a[contains(text(),'Product Disclosure Statement')]";
        }
        public static class PopUp
        {
            public const string Dialog      = "//div[@aria-labelledby='formotiv-popup-dialog']";
            public const string Header      = Dialog + "//h2[@id='formotiv-popup-dialog' and text() = 'Did you know?']";
            public const string Message     = Dialog + "//strong[text()=\"We'll cover your personal belongings up to $1500 if they're damaged, stolen or lost in a boat accident.\"]";
            public const string CloseButton = Dialog + "//button[@id='btn-close']";
        }
    }
    #endregion

    #region Constants
    private class Constants
    {
        public const string PageHeader          = "Here's your quote";
        public const string ActiveStepperLabel  = "Your quote";
        public const string QuoteNumberPreface  = "Your quote number is";
        public const string TermsParagraph1     = "Please remember that this is a quote only and does not provide cover. " +
                                                "The final information you provide to us may affect the terms and premium we offer.";
        public const string TermsParagraph2     = "It's important that you read and understand the Product Disclosure Statement (PDS) " +
                                                "to ensure the cover meets your needs. Please contact us if you have any difficulty " +
                                                "understanding the PDS or your quote.";
        public class AdviseUser
        {
            public class Helptext
            {
                public const string ExcessTitle             = "Excess";
                public const string ExcessBody              = "The excess is the amount you may need to pay towards settlement of any claim." +
                                                            " If you adjust your excess, the amount of premium you pay will change.";
                public const string AgreedValueTitle        = "Agreed value";
                public const string AgreedValueBody         = "The agreed value is the amount we agree to insure your boat for. It includes" +
                                                            " GST and registration. If you adjust the agreed value, the amount of premium you" +
                                                            " pay will change.";
                public const string FlotationOptionTitle    = "Waterskiing and flotation devices";
                public const string FlotationOptionBody     = "We'll cover your legal liability for accidental death or " +
                                                            "bodily injury to a water skier towed by your boat. We also " +
                                                            "cover damage to another person's property caused by the water " +
                                                            "skier. Waterskiing includes barefoot waterskiing, skurfing, " +
                                                            "wakeboarding and using a flotation device being towed by your boat " +
                                                            "(and only when using recognised and commercially manufactured " +
                                                            "equipment). Limits apply. See the Product Disclosure Statement " +
                                                            "for more information.";
                public const string RacingOptionTitle       = "Racing cover";
                public const string RacingOptionBody        = "Racing cover insures your boat for loss or damage while you take " +
                                                            "part in an organised sailing competition with other boats over a " +
                                                            "designated distance or route.\r\nWe don't cover:\r\nAny racing outside " +
                                                            "200 nautical miles from the Australian coastline.\r\nParticipation in " +
                                                            "an ocean race that's more than 200 nautical miles from start to finish.";
                
            }
            public class DeclinedCover
            {
                public const string ValueTooHighTitle   = "Please enter a value under $150,000";
                public const string ValueTooHighBody    = "If your boat is valued over $150,000, please call us on 13 17 03.";
                public const string ValueTooHighLink    = "tel:131703";
            }
        }
        public class WhatsIncludedList
        {
            public const string Item1   = "Cover for accidental damage, malicious damage, fire, theft, storm and flood.";
            public const string Item2   = "Cover anywhere in Australia and up to 200 nautical miles from the Australian coastline.";
            public const string Item3   = "Cover for personal belongings up to $1500 if this is part of an eligible claim.";
            public const string Item4   = "Cover for bodily injury or accidental death to another person or damage to their property.";
            public const string Item5   = "New boat replacement cover.";
            public const string Item6   = "Local claims team, available seven days a week.";
            public const string Item7   = "And much more!";
            public const string Summary = "This is a summary. See the Product Disclosure Statement for more information.";
            public const string PdsLink = "https://cdvnetd.ractest.com.au/products/insurance/policy-documents/boat-insurance";
        }
    }
    #endregion

    #region Settable properties and controls
    public string Value
    {
        get => GetElement(XPath.Field.Value).GetAttribute("value");
        set => SendKeyPressesAfterClearingExistingTextInField(XPath.Field.Value, $"{value}{Keys.Tab}");
    }
    public string BasicExcess
    {
        get => GetSelectedTextFromDropDown(XPath.Field.BasicExcess);
        set => WaitForSelectableAndPickFromDropdown(XPath.Field.BasicExcess, XPath.Field.ExcessOptions, value);
    }
    public bool AnswerFlotationOption
    {
        get => GetBinaryToggleState(XPath.Button.FlotationOption, XPath.Button.Yes, XPath.Button.No);
        set => ClickBinaryToggle(XPath.Button.FlotationOption, XPath.Button.Yes, XPath.Button.No, value);
    }
    public bool AnswerRacingOption
    {
        get => GetBinaryToggleState(XPath.Button.RacingOption, XPath.Button.Yes, XPath.Button.No);
        set => ClickBinaryToggle(XPath.Button.RacingOption, XPath.Button.Yes, XPath.Button.No, value);
    }
    public string EmailQuote
    {
        get => GetElement(XPath.Field.EmailAddress).GetAttribute("value");
        set => SendKeyPressesToField(XPath.Field.EmailAddress, $"{value}");
    }
    #endregion
    public SparkBoatYourQuote(Browser browser) : base(browser)
    { }

    public override bool IsDisplayed()
    {
        var isDisplayed = false;
        try
        {
            isDisplayed = string.Equals(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
            GetElement(XPath.QuoteNumber);
            GetElement(XPath.Button.NextPage);
        }
        catch (NoSuchElementException)
        {
            return false;
        }

        if (isDisplayed)
        { Reporting.LogPageChange("Spark Boat Quote page ? - YourQuote"); }

        return isDisplayed;
    }

    /// <summary>
    /// Confirm that the Agreed value on Your Quote has been pre-populated based on the test data input on the
    /// Your Boat screen.
    /// </summary>
    /// <param name="browser"></param>
    /// <param name="insuredAmount">The expected insured value for this test.</param>
    public void VerifyAgreedValueMatch(Browser browser, int insuredAmount)
    {
        Reporting.LogMinorSectionHeading("Verify Agreed Value");
        Reporting.AreEqual(insuredAmount.ToString(CultureInfo.InvariantCulture),
            DataHelper.StripMoneyNotations(GetElement(XPath.Field.Value).GetAttribute("value")),
            "initial Agreed value field displayed on this page against expected value for this test.");
    }

    /// <summary>
    /// Collect Details from the Your quote page for later use.
    /// </summary>
    /// <param name="quoteBoat"></param>
    public void YourQuoteComparisonDetails(QuoteBoat quoteBoat)
    {
        quoteBoat.QuoteData = new QuoteData();

        YourQuoteNumber(quoteBoat);
        PremiumBreakdown(quoteBoat);
        VerifyQuoteDetailsInShield(quoteBoat);
    }

    /// <summary>
    /// Log and retain the Quote number displayed on the Your quote step.
    /// </summary>
    /// <param name="quoteBoat"></param>
    public void YourQuoteNumber(QuoteBoat quoteBoat)
    {
        var quoteData = quoteBoat.QuoteData;
        quoteData.QuoteNumber = GetInnerText(XPath.QuoteNumber);
        Reporting.Log($"Quote Number value = {GetInnerText(XPath.QuoteNumber)}");
    }

    /// <summary>
    /// Log and retain the Premium Breakdown displayed on the Your quote step.
    /// </summary>
    /// <param name="quoteBoat"></param>
    public void PremiumBreakdown(QuoteBoat quoteBoat)
    {
        if (IsPremiumBreakdownDisplayed())
        {
            Reporting.Log("Premium Breakdown is already displayed.");
        }
        else
        {
            Reporting.Log("Selecting toggle control to expand Premium Breakdown.");
            ClickControl(XPath.PremiumBreakdown.ToggleControl);
        }
        var quoteData = quoteBoat.QuoteData;
        Reporting.LogMinorSectionHeading("Premium Breakdown for Annual payment frequency");

        ClickControl(XP_PAYMENT_FREQUENCY_ANNUAL);
        var annualPaymentFrequency = false;
        var annualEndTime = DateTime.Now.AddSeconds(WaitTimes.T5SEC);
        do
        {
            annualPaymentFrequency = (GetInnerText(XPath.PaymentFrequency.CurrentPaymentFrequency) == "Annually");
            Thread.Sleep(SleepTimes.T1SEC);
        } while (!annualPaymentFrequency && DateTime.Now < annualEndTime);
        Reporting.AreEqual("Annually", GetInnerText(XPath.PaymentFrequency.CurrentPaymentFrequency),
            " active payment frequency should now be Annually");

        ScrollElementIntoView(XPath.PremiumBreakdown.ToggleControl);
        Reporting.Log($"Scrolling Premium breakdown fields into view to capture snapshot of values before continuing.", _browser.Driver.TakeSnapshot());
        quoteData.AnnualPremium = QuoteAmount;

        Reporting.Log($"Total annual premium = {quoteData.AnnualPremium}");

        quoteData.PremiumBreakdownBasic = decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PremiumBreakdown.Basic)));
        Reporting.Log($"Basic premium = {quoteData.PremiumBreakdownBasic}");
        
        quoteData.PremiumBreakdownStamp = decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PremiumBreakdown.GovernmentCharges)));
        Reporting.Log($"Government charges = {quoteData.PremiumBreakdownStamp}");
        
        quoteData.PremiumBreakdownGST = decimal.Parse(DataHelper.StripMoneyNotations(GetInnerText(XPath.PremiumBreakdown.GST)));
        Reporting.Log($"GST = {quoteData.PremiumBreakdownGST}");

        Reporting.LogMinorSectionHeading("Changing to Monthly payment frequency");
        ClickControl(XP_PAYMENT_FREQUENCY_MONTHLY);
        // Confirm change of payment frequency before continuing
        var monthlyPaymentFrequency = false;
        var endTime = DateTime.Now.AddSeconds(WaitTimes.T5SEC);
        do
        {
            monthlyPaymentFrequency = (GetInnerText(XPath.PaymentFrequency.CurrentPaymentFrequency) == "Monthly");
            Thread.Sleep(SleepTimes.T1SEC);
        } while (!monthlyPaymentFrequency && DateTime.Now < endTime);
        
        Reporting.AreEqual("Monthly", GetInnerText(XPath.PaymentFrequency.CurrentPaymentFrequency), 
            "current active payment frequency should now be Monthly");

        quoteData.MonthlyPremium = QuoteAmount;

        Reporting.Log($"Monthly premium = {quoteData.MonthlyPremium}", _browser.Driver.TakeSnapshot());

        ClickControl(XP_PAYMENT_FREQUENCY_ANNUAL);
        var resetToAnnualEndTime = DateTime.Now.AddSeconds(WaitTimes.T5SEC);
        do
        {
            annualPaymentFrequency = (GetInnerText(XPath.PaymentFrequency.CurrentPaymentFrequency) == "Annually");
            Thread.Sleep(SleepTimes.T1SEC);
        } while (!annualPaymentFrequency && DateTime.Now < resetToAnnualEndTime);
        Reporting.AreEqual("Annually", GetInnerText(XPath.PaymentFrequency.CurrentPaymentFrequency),
            " active payment frequency should now be Annually");
    }

    /// <summary>
    /// As the Shield API does not return a value matching the Description of the enum
    /// in some cases, starts by assuming the Description and if one of the two other 
    /// values uses the enum value.
    /// </summary>
    /// <returns>The actual value we expect to see from Shield API related to Skipper's Ticket</returns>
    public static string TranslateTestDataToExpectedApiValueForSkippersTicket(QuoteBoat quoteBoat)
    {
        string apiAlias = quoteBoat.SkippersTicketHeld.GetDescription();
        
        if (quoteBoat.SkippersTicketHeld.GetDescription() == "I don't have a skipper's ticket" 
            || quoteBoat.SkippersTicketHeld.GetDescription() == "Less than 1 year")
        {
            apiAlias = quoteBoat.SkippersTicketHeld.ToString();
        }
        return apiAlias;
    }
    
    /// <summary>
    /// Use the Shield API via GetQuoteDetails, GetContactDetailsViaContactId and (sometimes) GetContactDetailsViaExternalContactNumber
    /// to fetch information about this quote and compare with expected results.
    /// </summary>
    /// <param name="quoteBoat">Quote data generated for this test.</param>
    /// <param name="isYourQuoteScreenUpdated">If flagged true we expect optional covers and basic excess to match test data. If false we bypass those checks.</param>
    /// <param name="isStartDateProvided">If flagged true, we compare the start date with the value set in quoteBoat. If false, the date is assumed to be today.</param>
    /// <param name="isPersonalInfoProvided">If flagged true, we verify personal information input by the user. If false we bypass those checks.</param>
    /// <param name="isRegistrationProvided">If flagged true, we verify the Boat/Trailer registration against quoteBoat. If false we bypass those checks.</param>
    public static void VerifyQuoteDetailsInShield(QuoteBoat quoteBoat, bool isYourQuoteScreenUpdated = false, bool isStartDateProvided = false, bool isPersonalInfoProvided = false, bool isRegistrationProvided = false)
    {
        Reporting.LogMinorSectionHeading("Verify Quote values in Shield");
        var apiQuoteResponse = DataHelper.GetQuoteDetails(quoteBoat.QuoteData.QuoteNumber);
        var apiContactResponse = DataHelper.GetContactDetailsViaContactId(apiQuoteResponse.Policyholder.Id.ToString());
        var dataQuoteResponse = ShieldBoatDB.FetchBoatQuoteDetails(quoteBoat.QuoteData.QuoteNumber);

        Reporting.IsNull(apiQuoteResponse.PolicyNumber,
            $"NULL returned for PolicyNumber from Shield API related to {quoteBoat.QuoteData.QuoteNumber} (as expected)");
        Reporting.AreEqual(Status.Proposal.GetDescription(), apiQuoteResponse.Status, "Status in Shield");

        Reporting.LogMinorSectionHeading("About you");

        Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), apiContactResponse.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), 
            $"expected Date of Birth against the value returned from Shield Contact API related to {apiQuoteResponse.Policyholder.Id.ToString()} on {quoteBoat.QuoteData.QuoteNumber}");
        
        string skippersTicketExpectedApiValue = TranslateTestDataToExpectedApiValueForSkippersTicket(quoteBoat);
        Reporting.AreEqual(skippersTicketExpectedApiValue, apiQuoteResponse.BoatAsset.SkipperExperience, 
            $"expected SkipperExperience value against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
        // TODO SPK-3809/SPK-4234: Confirmation of null value in Shield Database for quote/policy when claims/disclosures are provided.

        Reporting.LogMinorSectionHeading("Boat type");

        Reporting.AreEqual(quoteBoat.BoatTypeExternalCode.ToString(), apiQuoteResponse.BoatAsset.BoatType,
            $"expected EXTERNAL CODE for {quoteBoat.BoatTypeExternalCode.GetDescription()} Boat Type against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");

        Reporting.LogMinorSectionHeading("Your boat");

        Reporting.AreEqual(quoteBoat.BoatMake.GetDescription(), apiQuoteResponse.BoatAsset.Make,
            $"expected Make against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
        if (quoteBoat.BoatMake == BoatMake.Other)
        {
            Reporting.AreEqual(UnlistedInputs.OtherBoatMake.ToString(), apiQuoteResponse.BoatAsset.OtherMake,
            $"expected OtherMake against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} when 'Other' is selected for boat Make.");
        }
        Reporting.AreEqual(quoteBoat.BoatYearBuilt.ToString(), apiQuoteResponse.BoatAsset.BuiltYear,
            $"expected BuiltYear against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
        Reporting.AreEqual(quoteBoat.SparkBoatHull.ToString(), apiQuoteResponse.BoatAsset.HullConstruction.ToString(),
            $"expected EXTERNAL CODE for Hull Construction material against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
        Reporting.AreEqual(quoteBoat.InsuredAmount, apiQuoteResponse.Covers[0].SumInsured,
            $"expected Sum Insured against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
        Reporting.AreEqual(quoteBoat.IsFinanced.ToString(), apiQuoteResponse.BoatAsset.IsFinanced, ignoreCase: true,
            $"expected IsFinanced against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} (NOT case-sensitive)");
        if (quoteBoat.IsFinanced)
        {
            var apiFinancierContactResponse = DataHelper.GetContactDetailsViaExternalContactNumber(apiQuoteResponse.BoatAsset.FinancierExternalContactId[0]);
            if (string.Equals(quoteBoat.Financier, UnlistedInputs.FinancierNotFound))
            {
                Reporting.AreEqual("Other financier", apiFinancierContactResponse.LegalEntityName.ToString(), ignoreCase: true,
                    $"expected Financier when financier is not available for selection from our list against the value returned from Shield Contact API related to Contact Id {apiQuoteResponse.BoatAsset.FinancierExternalContactId[0].ToString()} on {quoteBoat.QuoteData.QuoteNumber} (NOT case-sensitive)");
            }
            else
            {
                Reporting.AreEqual(quoteBoat.Financier.ToString(), apiFinancierContactResponse.LegalEntityName.ToString(), ignoreCase: true,
                    $"expected Financier against the value returned from Shield Contact API related to Contact Id {apiQuoteResponse.BoatAsset.FinancierExternalContactId[0].ToString()} on {quoteBoat.QuoteData.QuoteNumber} (NOT case-sensitive)");
            }
        }
        else
        {
            Reporting.IsNull(apiQuoteResponse.BoatAsset.FinancierExternalContactId, 
                $"no value for Financier returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} as Financier was not set");
        }

        Reporting.LogMinorSectionHeading("More about your boat");

        Reporting.AreEqual("West Perth", apiQuoteResponse.BoatAsset.Risk_suburb, ignoreCase: true, //TODO: SPK-3809 generate random data and make this conditional
            $"expected Risk_suburb against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
        Reporting.AreEqual("6005", apiQuoteResponse.BoatAsset.Risk_postcode,
            $"expected Risk_postcode against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
        Reporting.AreEqual(quoteBoat.IsGaraged.ToString(), apiQuoteResponse.BoatAsset.IsGaraged, ignoreCase: true,
            $"expected IsGaraged against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} (NOT case-sensitive)");
        Reporting.AreEqual(quoteBoat.SparkBoatMotorType.ToString(), apiQuoteResponse.BoatAsset.MotorType,
            $"expected MotorType against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
        Reporting.AreEqual(quoteBoat.SecurityAlarmGps.ToString(), apiQuoteResponse.BoatAsset.IsSecurityAlarmGps, ignoreCase: true,
            $"expected IsSecurityAlarmGps against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} (NOT case-sensitive)");
        Reporting.AreEqual(quoteBoat.SecurityNebo.ToString(), apiQuoteResponse.BoatAsset.IsSecurityNebo, ignoreCase: true,
            $"expected IsSecurityNebo against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} (NOT case-sensitive)");
        Reporting.AreEqual(quoteBoat.SecurityHitch.ToString(), apiQuoteResponse.BoatAsset.IsSecurityHitch, ignoreCase: true,
            $"expected IsSecurityHitch against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} (NOT case-sensitive)");




        Reporting.LogMinorSectionHeading("Your quote");

        Reporting.AreEqual(quoteBoat.QuoteData.AnnualPremium, apiQuoteResponse.AnnualPremium.Total,
            $"expected Total Annual Premium against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");

        Reporting.AreEqual(quoteBoat.QuoteData.PremiumBreakdownBasic, apiQuoteResponse.AnnualPremium.BaseAmount,
            $"expected Base Amount for the Annual Premium against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");

        Reporting.AreEqual(quoteBoat.QuoteData.PremiumBreakdownGST, apiQuoteResponse.AnnualPremium.Gst,
            $"expected GST for the Annual Premium against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");

        Reporting.AreEqual(quoteBoat.QuoteData.PremiumBreakdownStamp, apiQuoteResponse.AnnualPremium.StampDuty,
            $"expected Stamp Duty for the Annual Premium against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");


        if (isYourQuoteScreenUpdated)
        {
            Reporting.AreEqual(quoteBoat.BasicExcess.ToString(), dataQuoteResponse.BasicExcess,
            $"expected Basic Excess for the boat against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");

            Reporting.AreEqual(quoteBoat.HasWaterSkiingAndFlotationDeviceCover.ToString(), dataQuoteResponse.HasWaterSkiingAndFlotationDeviceCover.ToString(),
            $"expected WaterSkiingAndFlotationDeviceCover flag against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");

            if (quoteBoat.BoatTypeExternalCode.Equals(SparkBoatTypeExternalCode.L))
            {
                Reporting.AreEqual(quoteBoat.HasRacingCover.ToString(), dataQuoteResponse.HasRacingCover.ToString(),
                    $"expected Racing Cover flag against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
            }
            else
            {
                Reporting.AreEqual("False", dataQuoteResponse.HasRacingCover.ToString(),
                    $"expected Racing Cover flag returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} is false as boat is not a Sailboat");
            }
        }
        
        Reporting.AreEqual(ShieldProductType.BGP, apiQuoteResponse.ProductType,
            $"expected Product Type against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");

        Reporting.LogMinorSectionHeading("Start Date");

        if (isStartDateProvided)
        {
            Reporting.AreEqual(quoteBoat.PolicyStartDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
                apiQuoteResponse.PolicyStartDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
                $"expected Policy Start against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
        }
        else
        {
            Reporting.AreEqual(DateTime.Now.Date.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
                apiQuoteResponse.PolicyStartDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
                $"expected Policy Start Date before member input (Today) against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
        }

        if (isPersonalInfoProvided)
        {
            Reporting.LogMinorSectionHeading("Your details");
            Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].Title.ToString(), apiContactResponse.Title.ToString(),
                $"expected Policyholder Title against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");

            Reporting.AreEqual("_B2C_" + quoteBoat.CandidatePolicyHolders[0].FirstName.ToString(), apiContactResponse.FirstName.ToString(),
                ignoreCase: true, $"expected Policyholder FirstName against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} (NOT case-sensitive)");

            Reporting.AreEqual("_B2C_" + quoteBoat.CandidatePolicyHolders[0].Surname.ToString(), apiContactResponse.Surname.ToString(),
                ignoreCase: true, $"expected Policyholder Surname against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} (NOT case-sensitive)");

            Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].MobilePhoneNumber.ToString(), apiContactResponse.MobilePhoneNumber.ToString(),
                $"expected Policyholder MobilePhoneNumber against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");

            Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].PrivateEmail.ToString(), apiContactResponse.PrivateEmail.ToString(),
                $"expected Policyholder PrivateEmail against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");

            Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].MailingAddress.StreetNumber.ToString(), apiContactResponse.MailingAddress.StreetNumber.ToString(),
                ignoreCase: true, $"expected Mailing Address StreetNumber against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");

            Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].MailingAddress.Suburb.ToString(), apiContactResponse.MailingAddress.Suburb.ToString(),
                ignoreCase: true, $"expected Mailing Address Suburb against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber} (NOT case-sensitive)");

            Reporting.AreEqual(quoteBoat.CandidatePolicyHolders[0].MailingAddress.PostCode.ToString(), apiContactResponse.MailingAddress.PostCode.ToString(),
                $"expected Mailing Address PostCode against value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
        }

        if (isRegistrationProvided)
        {
            Reporting.LogMinorSectionHeading("Registration");
            if (quoteBoat.BoatRego == null)
            {
                Reporting.IsNullOrEmptyString(apiQuoteResponse.BoatAsset.BoatRegistration, 
                    $"no Boat Registration value returned in the Shield API response related to {quoteBoat.QuoteData.QuoteNumber} as Boat Registration was not set");
            }
            else
            {
                Reporting.AreEqual(quoteBoat.BoatRego.ToUpper(), apiQuoteResponse.BoatAsset.BoatRegistration.ToString(),
                    $"expected Boat Registration against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
            }
            if (quoteBoat.BoatTrailerRego == null)
            {
                Reporting.IsNullOrEmptyString(apiQuoteResponse.BoatAsset.TrailerRegistration,
                    $"no Trailer Registration value returned in the Shield API response related to {quoteBoat.QuoteData.QuoteNumber} as Trailer Registration was not set");
            }
            else
            {
                Reporting.AreEqual(quoteBoat.BoatTrailerRego.ToUpper(), apiQuoteResponse.BoatAsset.TrailerRegistration.ToString(),
                    $"expected Trailer Registration against the value returned from Shield API related to {quoteBoat.QuoteData.QuoteNumber}");
            }
        }
    }
    public bool IsPremiumBreakdownDisplayed()
    {
        var isBreakdownDisplayed = false;
        try
        {
            GetElement(XPath.PremiumBreakdown.Basic);
            isBreakdownDisplayed = true;
        }
        catch (NoSuchElementException)
        {
            Reporting.Log("Premium Breakdown is not already displayed.");
        }
        return isBreakdownDisplayed;
    }
    public void VerifyPageContent(QuoteBoat quoteBoat)
    {
        Reporting.LogMinorSectionHeading("Begin General Page Content checks");

        Reporting.AreEqual(Constants.PageHeader,
                GetElement(XPath.PageHeader).Text, "page header content against expected value");

        Reporting.AreEqual(Sidebar.Link.PdsUrl,
            GetElement(XPaths.Sidebar.PdsLink).GetAttribute("href"), "NPE Sidebar PDS URL");

        VerifyStandardHeaderAndFooterContent();

        Reporting.AreEqual(Constants.QuoteNumberPreface,
            GetElement(XPath.QuoteNumberPreface).Text, "Quote Number preface");

        VerifyWhatsIncludedList();
        VerifyIfOnlineDiscountInformationDisplayed();
        VerifyTermsAndConditions();
        VerifyHelpText();
        FieldValidationAgreedValue(quoteBoat.InsuredAmount);
        VerifyDeclineCover(quoteBoat);
    }
    /// <summary>
    /// Boat NB - Remove online discount https://rac-wa.atlassian.net/browse/SPK-6100
    /// Verifies if elements with Online Discount information is displayed or not.
    /// </summary>
    private void VerifyIfOnlineDiscountInformationDisplayed()
    {
            Reporting.IsFalse(_driver.TryFindElement(By.XPath(XPath.BoatOnlineDiscount),
                                 out IWebElement onlineDiscount), "we do not see boat online discount at top of screen");
            Reporting.IsFalse(_driver.TryFindElement(By.XPath(XPath.TermsParagraph3),
                                 out IWebElement discountDisclosure), "we do not see boat online discount disclosure at bottom of screen");
    }   
    /// <summary>
    /// Verifies the content of the bullet-pointed list items in the "What's included" section beneath 
    /// the "Get policy" button on this page.
    /// </summary>
    /// <param name="browser"></param>
    private void VerifyWhatsIncludedList()
    {
        ScrollElementIntoView(XPath.WhatsIncludedList.Item4);
        Reporting.Log($"Scrolled 'What's included' list into view ", _browser.Driver.TakeSnapshot());

        Reporting.AreEqual(Constants.WhatsIncludedList.Item1,
            GetInnerText(XPath.WhatsIncludedList.Item1), "What's included list item 1");
        Reporting.AreEqual(Constants.WhatsIncludedList.Item2,
            GetInnerText(XPath.WhatsIncludedList.Item2), "What's included list item 2");
        Reporting.AreEqual(Constants.WhatsIncludedList.Item3,
            GetInnerText(XPath.WhatsIncludedList.Item3), "What's included list item 3");
        Reporting.AreEqual(Constants.WhatsIncludedList.Item4,
            GetInnerText(XPath.WhatsIncludedList.Item4), "What's included list item 4");
        Reporting.AreEqual(Constants.WhatsIncludedList.Item5,
            GetInnerText(XPath.WhatsIncludedList.Item5), "What's included list item 5");
        Reporting.AreEqual(Constants.WhatsIncludedList.Item6,
            GetInnerText(XPath.WhatsIncludedList.Item6), "What's included list item 6");
        Reporting.AreEqual(Constants.WhatsIncludedList.Item7,
            GetInnerText(XPath.WhatsIncludedList.Item7), "What's included list item 7");
        Reporting.AreEqual(Constants.WhatsIncludedList.Summary,
            GetInnerText(XPath.WhatsIncludedList.Summary), "What's included list Summary statement");
        Reporting.AreEqual(Constants.WhatsIncludedList.PdsLink,
            GetElement(XPath.WhatsIncludedList.PdsLink).GetAttribute("href"), "PDS link address");
    }

    private void VerifyTermsAndConditions()
    {
        ScrollElementIntoView(XPath.TermsParagraph2);
        Reporting.AreEqual(Constants.TermsParagraph1,
            GetElement(XPath.TermsParagraph1).Text, "Terms & Conditions paragraph 1 text content");
        Reporting.AreEqual(Constants.TermsParagraph2,
            GetElement(XPath.TermsParagraph2).Text, "Terms & Conditions paragraph 2 text content");
        Reporting.Log($"Scrolled Terms & Conditions into view ", _browser.Driver.TakeSnapshot());
    }

    private void VerifyHelpText()
    {
        Reporting.LogMinorSectionHeading("Begin Help Text verification");

        ClickControl(XPath.Button.ExcessHelpTextButton);
        _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.Helptext.ExcessTitle), WaitTimes.T5SEC);

        Reporting.AreEqual(Constants.AdviseUser.Helptext.ExcessTitle,
            GetInnerText(XPath.AdviseUser.Helptext.ExcessTitle), "title of help text for Excess field against expected value");
        Reporting.AreEqual(Constants.AdviseUser.Helptext.ExcessBody,
            GetInnerText(XPath.AdviseUser.Helptext.ExcessBody), "message of help text for Excess field against expected value");
        Reporting.Log("Capturing snapshot of Help Text for Excess field before closing it", _browser.Driver.TakeSnapshot());
        ClickControl(XPath.Button.ExcessHelpTextClose);

        ClickControl(XPath.Button.AgreedValueHelpTextButton);
        _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.Helptext.AgreedValueTitle), WaitTimes.T5SEC);

        Reporting.AreEqual(Constants.AdviseUser.Helptext.AgreedValueTitle,
            GetInnerText(XPath.AdviseUser.Helptext.AgreedValueTitle), "title of help text for Agreed Value field against expected value");
        Reporting.AreEqual(Constants.AdviseUser.Helptext.AgreedValueBody,
            GetInnerText(XPath.AdviseUser.Helptext.AgreedValueBody), "message of help text for Agreed Value field against expected value");
        Reporting.Log("Capturing snapshot of Help Text for Agreed Value field before closing it", _browser.Driver.TakeSnapshot());
        ClickControl(XPath.Button.AgreedValueHelpTextClose); 
        
        ClickControl(XPath.Button.FlotationOptionHelpTextButton);
        _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.Helptext.FlotationOptionTitle), WaitTimes.T5SEC);

        Reporting.AreEqual(Constants.AdviseUser.Helptext.FlotationOptionTitle,
            GetInnerText(XPath.AdviseUser.Helptext.FlotationOptionTitle), "title of help text for Waterskiing/Flotation Device field against expected value");
        Reporting.AreEqual(Constants.AdviseUser.Helptext.FlotationOptionBody,
            GetInnerText(XPath.AdviseUser.Helptext.FlotationOptionBody), "message of help text for Waterskiing/Flotation Device field against expected value");
        Reporting.Log("Capturing snapshot of Help Text for waterskiing/flotation device before closing it", _browser.Driver.TakeSnapshot());
        ClickControl(XPath.Button.FlotationOptionHelpTextClose);

        ClickControl(XPath.Button.RacingOptionHelpTextButton);
        _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.Helptext.RacingOptionTitle), WaitTimes.T5SEC);

        Reporting.AreEqual(Constants.AdviseUser.Helptext.RacingOptionTitle,
            GetInnerText(XPath.AdviseUser.Helptext.RacingOptionTitle), "title of help text for Racing Cover field against expected value");
        Reporting.AreEqual(Constants.AdviseUser.Helptext.RacingOptionBody,
            GetInnerText(XPath.AdviseUser.Helptext.RacingOptionBody), "message of help text for Racing Cover field against expected value");
        Reporting.Log("Capturing snapshot of Help Text for Racing Cover before closing it", _browser.Driver.TakeSnapshot());
        ClickControl(XPath.Button.RacingOptionHelpTextClose);
    }

    private void VerifyDeclineCover(QuoteBoat quoteBoat)
    {
        Reporting.LogMinorSectionHeading("Begin Decline Cover verification");
        double moreThanMax = (BOAT_MAXIMUM_INSURED_VALUE_ONLINE + 1);
        Reporting.Log($"Setting Sum Insured to {moreThanMax}");
        Value = moreThanMax.ToString(CultureInfo.InvariantCulture);
        using (var spinner = new SparkSpinner(_browser))
        {
            spinner.WaitForSpinnerToFinish();
        }
        _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.DeclinedCover.ValueTooHighTitle), WaitTimes.T5SEC);
        Reporting.Log("Capturing snapshot of advice to user that they must call to discuss a quote for a high value boat.", _browser.Driver.TakeSnapshot());

        Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.ValueTooHighTitle,
            GetInnerText(XPath.AdviseUser.DeclinedCover.ValueTooHighTitle), "title of element displayed to advise must call for quote due to high value boat");
        Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.ValueTooHighBody,
            GetInnerText(XPath.AdviseUser.DeclinedCover.ValueTooHighBody), "body of element displayed to advise must call for quote due to high value boat");
        Reporting.AreEqual(Constants.AdviseUser.DeclinedCover.ValueTooHighLink,
            GetElement(XPath.AdviseUser.DeclinedCover.ValueTooHighLink).GetAttribute("href"), "URL of link on the telephone number provided when we advise the user to call us");

        Reporting.Log($"Setting Sum Insured to {BOAT_MAXIMUM_INSURED_VALUE_ONLINE} exactly");
        Value = BOAT_MAXIMUM_INSURED_VALUE_ONLINE.ToString(CultureInfo.InvariantCulture);
        using (var spinner = new SparkSpinner(_browser))
        {
            spinner.WaitForSpinnerToFinish();
        }
        _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.DeclinedCover.ValueTooHighTitle), WaitTimes.T5SEC);
        Reporting.Log("Capturing snapshot to show message has been dismissed.", _browser.Driver.TakeSnapshot());

        Reporting.Log($"Setting Sum Insured back to value generated for this test: Value = {quoteBoat.InsuredAmount}");
        Value = quoteBoat.InsuredAmount.ToString(CultureInfo.InvariantCulture);
        using (var spinner = new SparkSpinner(_browser))
        {
            spinner.WaitForSpinnerToFinish();
        }
    }
    
    /// <summary>
    /// Verify the field validation error for the Agreed Value field is displayed as expected
    /// if the user populates it with a value less than $1.
    /// </summary>
    /// <param name="browser"></param>
    /// <param name="insuredAmount">Used to restore the Sum Insured to the desired value after triggering validation errors</param>
    private void FieldValidationAgreedValue(int insuredAmount)
    {
        Reporting.LogMinorSectionHeading("Begin Field Validation Error verification");
        Reporting.Log($"Setting Sum Insured to $0");
        Value = "0";
        using (var spinner = new SparkSpinner(_browser))
        {
            spinner.WaitForSpinnerToFinish();
        }
        _driver.WaitForElementToBeVisible(By.XPath(XPath.AdviseUser.FieldValidation.Value), WaitTimes.T5SEC);
        Reporting.Log("Capturing snapshot of field validation error for 'Agreed value' = 0.", _browser.Driver.TakeSnapshot());

        Reporting.AreEqual(AdviseUser.FieldValidation.EnterAValue,
            GetInnerText(XPath.AdviseUser.FieldValidation.Value), "element displayed to advise user we need a positive sum insured");

        Reporting.Log($"Setting Sum Insured to $1");
        Value = "1";
        using (var spinner = new SparkSpinner(_browser))
        {
            spinner.WaitForSpinnerToFinish();
        }
        _driver.WaitForElementToBeInvisible(By.XPath(XPath.AdviseUser.FieldValidation.Value), WaitTimes.T5SEC);
        Reporting.Log("Capturing snapshot showing field validation error has been dismissed once a positive value has been input.", _browser.Driver.TakeSnapshot());

        Reporting.Log($"Setting Sum Insured back to value generated for this test: Value = {insuredAmount}");
        Value = insuredAmount.ToString(CultureInfo.InvariantCulture);
        using (var spinner = new SparkSpinner(_browser))
        {
            spinner.WaitForSpinnerToFinish();
        }
        Reporting.Log("Capturing snapshot showing value for this test has been restored.", _browser.Driver.TakeSnapshot());
    }

    public void UpdatePageFields(QuoteBoat quoteBoat)
    {
        Reporting.LogMinorSectionHeading("Begin Update fields");
        Reporting.Log($"Setting Basic Excess to ${quoteBoat.BasicExcess.ToString(CultureInfo.InvariantCulture)}");
        BasicExcess = "$" + quoteBoat.BasicExcess.ToString(CultureInfo.InvariantCulture);
        using (var spinner = new SparkSpinner(_browser))
        {
            spinner.WaitForSpinnerToFinish();
        }
        Reporting.Log($"Setting WaterskiingCover = {quoteBoat.HasWaterSkiingAndFlotationDeviceCover}");
        AnswerFlotationOption = quoteBoat.HasWaterSkiingAndFlotationDeviceCover;
        using (var spinner = new SparkSpinner(_browser))
        {
            spinner.WaitForSpinnerToFinish();
        }
        if (quoteBoat.BoatTypeExternalCode.Equals(SparkBoatTypeExternalCode.L))
        {
            Reporting.Log($"Setting RacingCover (Sailboat only) = {quoteBoat.HasRacingCover}");
            AnswerRacingOption = quoteBoat.HasRacingCover;
        }
        using (var spinner = new SparkSpinner(_browser))
        {
            spinner.WaitForSpinnerToFinish();
        }
        Reporting.LogMinorSectionHeading("Recording up to date premium data.");
        PremiumBreakdown(quoteBoat);
        ScrollElementIntoView(XPath.Button.FlotationOption);
        Reporting.Log($"Scrolled optional cover field/s into view to capture snapshot of final optional cover input before continuing.", _browser.Driver.TakeSnapshot());
        RequestEmailQuote(quoteBoat.CandidatePolicyHolders[0].PrivateEmail.Address);
    }

    /// <summary>
    /// Display the field to input an email address for the quote to be sent to, 
    /// then input the Private Email generated for this test.
    /// </summary>
    /// <param name="emailAddress">The email address generated for the member in this test.</param>
    public void RequestEmailQuote(string emailAddress)
    {
        ClickControl(XPath.Button.ShowEmailQuote);
        EmailQuote = emailAddress;
        ClickControl(XPath.Button.SendEmailQuote);
        Reporting.Log($"Requesting email of quote to {emailAddress}", _browser.Driver.TakeSnapshot());
        using (var spinner = new SparkSpinner(_browser))
            {
                spinner.WaitForSpinnerToFinish();
            }
    }

    public void ContinueToStartDate()
    {
        ClickControl(XPath.Button.NextPage);
        using (var spinner = new SparkSpinner(_browser))
        {
            spinner.WaitForSpinnerToFinish();
        }
    }

    /// <summary>
    /// Formotiv runs its own algorithm to determine whether
    /// an additional promotional pop-up should be shown. As
    /// we don't always know, then automation should check
    /// for it on entry into the quote page.
    /// </summary>
    public void CloseFormotivPopupIfDisplayed()
    {
        // Formotive can have a delay in appearing.
        if (_driver.TryWaitForElementToBeVisible(By.XPath(XPath.PopUp.Dialog), WaitTimes.T5SEC, out IWebElement dialogPanel))
        {
            Reporting.IsTrue(IsControlDisplayed(XPath.PopUp.Header),  "'Here's your quote' page: 'Formotiv' text: Did you know? should display");
            Reporting.IsTrue(IsControlDisplayed(XPath.PopUp.Message), "'Here's your quote' page: 'Formotiv' text: We'll cover your personal belongings... should display");
            ClickControl(XPath.PopUp.CloseButton);
        }
    }

}
