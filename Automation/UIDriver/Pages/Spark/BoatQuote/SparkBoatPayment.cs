using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;


namespace UIDriver.Pages.Spark.BoatQuote
{
    public class SparkBoatPayment : SparkPaymentPage
    {
        #region Constants
        private class Constants
        {
            public class PolicySummary
            {
                public static readonly string PleaseCheckDetailsMessage = "Please ensure all details are correct and that the policy suits your needs.";
                public class Cover
                {
                    public static readonly string AgreedValue      = "Agreed value ";
                    public static readonly string BasicExcess      = "Basic excess ";
                    public static readonly string WaterskiingCover = " waterskiing and flotation device cover";
                    public static readonly string RacingCover      = " racing cover";

                }
                public class Boat
                {
                    public static readonly string StoredInWater           = "Not usually stored in the water";
                    public static readonly string BusinessOrCommercial    = "No business or commercial use";
                    public static readonly string HouseBoatOrPersonal     = "Not houseboat or personal watercraft";
                    public static readonly string ValuedUnder150k         = "Boat valued under $150,000";
                    public static readonly string FinancierPre            = "Financed with ";
                    public static readonly string NoFinance               = "No boat finance";
                    public static readonly string KeptInSuburb            = "Kept in ";
                    public static readonly string KeptInGarage            = "Kept in a garage";
                    public static readonly string NotKeptInGarage         = "Not kept in a garage";
                    public static readonly string MotorType               = " motor";
                    public static readonly string NoSecurity              = "No security";
                    public static readonly string SecurityAlarmOrGps      = "Alarm or GPS tracker";
                    public static readonly string SecurityNeboLink        = "nebolink";
                    public static readonly string SecurityTrailerDevice   = "Trailer security device";

                }
                public class Registration
                {
                    public static readonly string BoatRego                = "Boat rego ";
                    public static readonly string BoatRegoNotProvided     = "No boat rego provided";
                    public static readonly string TrailerRego             = "Trailer rego ";
                    public static readonly string TrailerRegoNotProvided  = "No trailer rego provided";
                }
                public class PolicyStart
                {
                    public static readonly string Date = "Policy start date ";
                }
                public class YourDetails
                {
                    public static readonly string DateOfBirth             = "Date of birth ";
                    public static readonly string SkippersTicket          = "Skipper's ticket held for ";
                    public static readonly string NoSkippersTicket        = "No Skipper's ticket held";
                    public static readonly string ClaimsCount             = " claims in the last 3 years";
                    public static readonly string ClaimsCountZero         = "No claims in the last 3 years";
                }
            }
            public static readonly string ActiveStepperLabel = "Payment";
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public static readonly string PageHeader = "//*[@id='header']";
            public static readonly string ActiveStepper = "//button[@aria-selected='true']";
            public static class PaymentFrequency
            {
                public static readonly string CurrentPaymentFrequency = "//button[@aria-pressed='true']";
            }
            public class Button
            {
                public static readonly string MoreQuoteInformationAccordion   = "id('show-quote-information-accordion')";
                public static readonly string PaymentMethodCreditCard         = "id('input-payment-method-Card')";
                public static readonly string PaymentMethodBankAccount        = "//button[@id='input-payment-method-Bank account']";
                public static readonly string AcceptTermsCheckbox             = "//span[@data-test-id='payment-disclaimer-input']";
            }
            public class PolicySummary
            {
                public static readonly string PleaseCheckDetailsMessage = "//p[contains(text(),'ensure all details are correct and that the policy suits your needs.')]";
                public static readonly string MakeHullYear              = "id('your-boat-make-hull-year-text')";
                public static readonly string BoatType                  = "id('boat-type-text')";
                public class Cover
                {                
                    public static readonly string AgreedValue                     = "id('your-cover-agreed-value-text')";
                    public static readonly string BasicExcess                     = "id('your-cover-excess-text')";
                    public static readonly string WaterskiingCover                = "id('your-cover-waterskiing-cover-text')";
                    public static readonly string RacingCover                     = "id('your-cover-racing-cover-text')";
                }
                public class Boat
                {
                    public static readonly string StoredInWater                   = "id('more-about-your-boat-storage-text')";
                    public static readonly string BusinessOrCommercial            = "id('more-about-your-boat-commercial-use-text')";
                    public static readonly string HouseBoatOrPersonalWatercraft   = "id('more-about-your-boat-personal-watercraft-text')";
                    public static readonly string ValuedUnder150k                 = "id('more-about-your-boat-value-text')";
                    public static readonly string Financier                       = "id('more-about-your-boat-finance-text')";
                    public static readonly string KeptInSuburb                    = "id('more-about-your-boat-suburb-text')";
                    public static readonly string GaragedOrNot                    = "id('more-about-your-boat-garage-text')";
                    public static readonly string MotorType                       = "id('more-about-your-boat-motor-type-text')";
                    public static readonly string NoSecurity                      = "id('more-about-your-boat-no-security-text')";
                    public static readonly string SecurityAlarmOrGps              = "id('more-about-your-boat-alarm-or-gps-tracker-text')";
                    public static readonly string SecurityNeboLink                = "id('more-about-your-boat-nebolink-text')";
                    public static readonly string SecurityTrailerDevice           = "id('more-about-your-boat-trailer-security-device-text')";
                }
                public class Registration
                {
                    public static readonly string BoatRego                        = "id('your-registration-boat-rego-text')";
                    public static readonly string TrailerRego                     = "id('your-registration-trailer-rego-text')";
                }
                public class PolicyStart
                {
                    public static readonly string Date                 = "id('start-date-text')";
                }
                public class YourDetails
                {
                    public static readonly string TitleAndNames                   = "id('your-details-full-name-text')";
                    public static readonly string ContactTelephone                = "id('your-details-contact-number-text')";
                    public static readonly string Email                           = "id('your-details-email-text')";
                    public static readonly string MailingAddress                  = "id('your-details-address-text')";
                    public static readonly string DateOfBirth                     = "id('your-details-dob-text')";
                    public static readonly string SkippersTicket                  = "id('your-details-skippers-experience-text')";
                    public static readonly string ClaimsCount                     = "id('your-details-claims-history-text')";
                }
            }
        }
        #endregion

        #region Settable properties and controls

        #endregion
        public SparkBoatPayment(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                isDisplayed = string.Equals(Constants.ActiveStepperLabel, GetInnerText(XPath.ActiveStepper));
                isDisplayed = isDisplayed && string.Equals(Sidebar.Link.PdsUrl, GetElement(XPaths.Sidebar.PdsLink).GetAttribute("href"));
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            if (isDisplayed)
            { Reporting.LogPageChange("Spark Boat Quote page ? - Payment"); }
            return isDisplayed;
        }

        public void CheckPaymentFieldValidation(Browser browser)
        {
            // Placeholder 
        }

        public void PremiumCheck(QuoteBoat quoteBoat)
        {
            var quoteData = quoteBoat.QuoteData;
            Reporting.LogMinorSectionHeading("Annual Premium");
            Reporting.Log($"Current recorded quoteData.AnnualPremium value = {quoteData.AnnualPremium}");
            ClickControl(XP_PAYMENT_FREQUENCY_ANNUAL);
            
            var annualPaymentFrequency = false;
            var annualEndTime = DateTime.Now.AddSeconds(WaitTimes.T5SEC);
            do
            {
                annualPaymentFrequency = (GetInnerText(XPath.PaymentFrequency.CurrentPaymentFrequency) == "Annually");
                Thread.Sleep(SleepTimes.T1SEC);
            } while (!annualPaymentFrequency && DateTime.Now < annualEndTime);

            Reporting.AreEqual("Annually", GetInnerText(XPath.PaymentFrequency.CurrentPaymentFrequency),
                "current active payment frequency should now be Annually");
            quoteData.AnnualPremium = PaymentAmount;
            Reporting.Log($"Annual premium from Payment step = {quoteData.AnnualPremium}", _browser.Driver.TakeSnapshot());

            Reporting.LogMinorSectionHeading("Monthly Premium");
            Reporting.Log($"Current recorded quoteData.MonthlyPremium value = {quoteData.MonthlyPremium}");
            ClickControl(XP_PAYMENT_FREQUENCY_MONTHLY);
            
            var monthlyPaymentFrequency = false;
            var monthlyEndTime = DateTime.Now.AddSeconds(WaitTimes.T5SEC);
            do
            {
                monthlyPaymentFrequency = (GetInnerText(XPath.PaymentFrequency.CurrentPaymentFrequency) == "Monthly");
                Thread.Sleep(SleepTimes.T1SEC);
            } while (!monthlyPaymentFrequency && DateTime.Now < monthlyEndTime);

            Reporting.AreEqual("Monthly", GetInnerText(XPath.PaymentFrequency.CurrentPaymentFrequency),
                "current active payment frequency should now be Monthly");
            quoteData.MonthlyPremium = PaymentAmount;
            Reporting.Log($"Monthly premium = {quoteData.MonthlyPremium}");
        }

        public void CheckSummaryContent(Browser browser, QuoteBoat quoteBoat)
        {
            Reporting.LogMinorSectionHeading("Begin Policy Summary Validation");

            ClickControl(XPath.Button.MoreQuoteInformationAccordion);
            Reporting.Log($"Selecting 'Show your quote information' and capturing a screenshot of the top half of the Policy Summary.", _browser.Driver.TakeSnapshot());

            _driver.WaitForElementToBeVisible(By.XPath(XPath.PolicySummary.PolicyStart.Date), WaitTimes.T5SEC);
            ScrollElementIntoView(XPath.PolicySummary.PolicyStart.Date);
            Reporting.Log($"Capturing screenshot after scrolling to show the bottom half of the Policy Summary.", _browser.Driver.TakeSnapshot());

            Reporting.LogMinorSectionHeading("Begin checking Policy Summary values");

            Reporting.AreEqual(Constants.PolicySummary.PleaseCheckDetailsMessage, 
                GetInnerText(XPath.PolicySummary.PleaseCheckDetailsMessage), "expected message to user advising check details against the value displayed on the policy summary");

            var boatMakePresented = quoteBoat.BoatMake.GetDescription().Equals("Other") ? UnlistedInputs.OtherBoatMake : quoteBoat.BoatMake.GetDescription();

            Reporting.AreEqual(boatMakePresented + ", " + quoteBoat.SparkBoatHull.GetDescription() + ", " + quoteBoat.BoatYearBuilt.ToString(),
                GetInnerText(XPath.PolicySummary.MakeHullYear), ignoreCase: true, "expected Make/Hull Material/Year for this test with the value displayed on the policy summary");
            
            Reporting.AreEqual(quoteBoat.BoatTypeExternalCode.GetDescription().ToString(),
                GetInnerText(XPath.PolicySummary.BoatType), ignoreCase: true, "expected boat type selected for this test with the value displayed on the policy summary");

            Reporting.AreEqual(Constants.PolicySummary.Cover.AgreedValue + quoteBoat.InsuredAmount.ToString(CultureInfo.InvariantCulture),
                DataHelper.StripMoneyNotations(GetInnerText(XPath.PolicySummary.Cover.AgreedValue)), "insured value generated for this test against the 'Agreed value' of the boat displayed in the policy summary");

            Reporting.AreEqual(Constants.PolicySummary.Cover.BasicExcess + quoteBoat.BasicExcess.ToString(CultureInfo.InvariantCulture),
                DataHelper.StripMoneyNotations(GetInnerText(XPath.PolicySummary.Cover.BasicExcess)), "basic excess generated for this test against the value displayed in the policy summary");
            
            var prefixForWaterSkiing = quoteBoat.HasWaterSkiingAndFlotationDeviceCover ? "Includes" : "No";

            Reporting.AreEqual(prefixForWaterSkiing + Constants.PolicySummary.Cover.WaterskiingCover,
                GetInnerText(XPath.PolicySummary.Cover.WaterskiingCover), "expected policy summary text for Waterskiing and flotation device cover against the value in the summary");

            if (quoteBoat.BoatTypeExternalCode.Equals(SparkBoatTypeExternalCode.L))
            {
                var prefixForRacing  = quoteBoat.HasRacingCover ? "Includes" : "No";

            Reporting.AreEqual(prefixForRacing + Constants.PolicySummary.Cover.RacingCover,
                GetInnerText(XPath.PolicySummary.Cover.RacingCover), "expected policy summary text for Racing cover against the value in the summary" +
                $"as the Type of Boat is a {quoteBoat.BoatTypeExternalCode.GetDescription()}");
            }
            else 
            {
                Reporting.Log($"Racing cover should only be available for Sailboats. This boat type = {quoteBoat.BoatTypeExternalCode.GetDescription()}");
                Reporting.IsFalse(IsRacingCoverDisplayed, "Racing cover entry in policy summary".IsNotDisplayed());
            }

            Reporting.AreEqual(Constants.PolicySummary.PolicyStart.Date + quoteBoat.PolicyStartDate.ToString(DateTimeTextFormat.ddMMyyyy),
                GetInnerText(XPath.PolicySummary.PolicyStart.Date), "Policy Start Date generated for this test compared to the value in the summary");

            Reporting.LogMinorSectionHeading("Continue evaluation of Policy Summary details with 'Let's start'");

            Reporting.AreEqual(Constants.PolicySummary.Boat.StoredInWater,
                GetInnerText(XPath.PolicySummary.Boat.StoredInWater), "expected value for 'Stored in water' text displayed with the value in the summary");

            Reporting.AreEqual(Constants.PolicySummary.Boat.BusinessOrCommercial,
                GetInnerText(XPath.PolicySummary.Boat.BusinessOrCommercial), "expected value for 'Business or commercial' text displayed with the value in the summary");

            Reporting.AreEqual(Constants.PolicySummary.Boat.HouseBoatOrPersonal,
                GetInnerText(XPath.PolicySummary.Boat.HouseBoatOrPersonalWatercraft), "expected value for 'Houseboat or personal watercraft' text displayed with the value in the summary");

            Reporting.AreEqual(Constants.PolicySummary.Boat.ValuedUnder150k,
                GetInnerText(XPath.PolicySummary.Boat.ValuedUnder150k), "expected value for 'Valued under $150k' text displayed with the value in the summary");

            Reporting.Log($"IsFinanced = {quoteBoat.IsFinanced}");
            if (quoteBoat.IsFinanced)
            {
                if (string.Equals(quoteBoat.Financier, UnlistedInputs.FinancierNotFound))
                {
                    Reporting.AreEqual(Constants.PolicySummary.Boat.FinancierPre + "Other financier",
                        GetInnerText(XPath.PolicySummary.Boat.Financier), ignoreCase: true, "expected text when financier is not available for selection from our list with the text displayed on the policy summary (NOT case-sensitive)");
                }
                else
                {
                    Reporting.AreEqual(Constants.PolicySummary.Boat.FinancierPre + quoteBoat.Financier,
                        GetInnerText(XPath.PolicySummary.Boat.Financier), ignoreCase: true, "expected financier randomly selected for this test with the text displayed on the policy summary (NOT case-sensitive)");
                }
            }
            else
            {
                Reporting.Log($"{Constants.PolicySummary.Boat.NoFinance}");
                Reporting.AreEqual(Constants.PolicySummary.Boat.NoFinance,
                    GetInnerText(XPath.PolicySummary.Boat.Financier), "expected text when no financier is present with the text displayed on the policy summary");
            }

            Reporting.LogMinorSectionHeading("More about your boat");

            if (quoteBoat.IsGaraged)
            {
                Reporting.AreEqual(Constants.PolicySummary.Boat.KeptInGarage,
                    GetInnerText(XPath.PolicySummary.Boat.GaragedOrNot), "expected text when boat is garaged against the text displayed on the Policy Summary");
            }
            else
            {
                Reporting.AreEqual(Constants.PolicySummary.Boat.NotKeptInGarage,
                    GetInnerText(XPath.PolicySummary.Boat.GaragedOrNot), "expected text for when boat is not garaged against the text displayed on the Policy Summary");
            }

            Reporting.AreEqual(quoteBoat.SparkBoatMotorType.GetDescription() + Constants.PolicySummary.Boat.MotorType,
                GetInnerText(XPath.PolicySummary.Boat.MotorType), "expected text for Motor Type against the text displayed on the Policy Summary");

            if (quoteBoat.SecurityAlarmGps)
            {
                Reporting.AreEqual(Constants.PolicySummary.Boat.SecurityAlarmOrGps, GetInnerText(XPath.PolicySummary.Boat.SecurityAlarmOrGps),
                    "expected text when Alarm or GPS security option is selected, against the text displayed on the Policy Summary");
            }

            if (quoteBoat.SecurityNebo)
            {
                Reporting.AreEqual(Constants.PolicySummary.Boat.SecurityNeboLink, GetInnerText(XPath.PolicySummary.Boat.SecurityNeboLink),
                    "expected text when neboLink security option is selected, against the text displayed on the Policy Summary");
            }

            if (quoteBoat.SecurityHitch)
            {
                Reporting.AreEqual(Constants.PolicySummary.Boat.SecurityTrailerDevice, GetInnerText(XPath.PolicySummary.Boat.SecurityTrailerDevice),
                    "expected text when Trailer security device option is selected, against the text displayed on the Policy Summary");
            }

            if (!quoteBoat.SecurityAlarmGps && !quoteBoat.SecurityNebo && !quoteBoat.SecurityHitch)
            {
                Reporting.AreEqual(Constants.PolicySummary.Boat.NoSecurity, GetInnerText(XPath.PolicySummary.Boat.NoSecurity),
                    "expected text when 'No security' option is selected, against the text displayed on the Policy Summary");
            }


            if (quoteBoat.SkippersTicketHeld == SkippersTicketYearsHeld.Noskippersticket) 
            {
                Reporting.AreEqual(Constants.PolicySummary.YourDetails.NoSkippersTicket, 
                    GetInnerText(XPath.PolicySummary.YourDetails.SkippersTicket), 
                    "expected text when 'I don't have a skipper's ticket' has been selected by the member");
            }
            else if (quoteBoat.SkippersTicketHeld == SkippersTicketYearsHeld.Lessthan1)
            {
                Reporting.AreEqual(Constants.PolicySummary.YourDetails.SkippersTicket + quoteBoat.SkippersTicketHeld.GetDescription(), 
                    GetInnerText(XPath.PolicySummary.YourDetails.SkippersTicket), ignoreCase: true,
                    "expected text when the member has had a Skipper's ticket for less than one year");
            }
            else
            {
                Reporting.AreEqual(Constants.PolicySummary.YourDetails.SkippersTicket + quoteBoat.SkippersTicketHeld.GetDescription() + " years", 
                    GetInnerText(XPath.PolicySummary.YourDetails.SkippersTicket), 
                    "expected text when the member has a Skipper's ticket");
            }
        }

        public bool IsRacingCoverDisplayed => IsControlDisplayed(XPath.PolicySummary.Cover.RacingCover);

        public void InputPaymentInformation(Browser browser, QuoteBoat quoteBoat, bool detailedUiChecking = false)
        {

            if (quoteBoat.PayMethod.IsAnnual)
            {
                ClickControl(XP_PAYMENT_FREQUENCY_ANNUAL);
            }
            else
            {
                ClickControl(XP_PAYMENT_FREQUENCY_MONTHLY);
            }

            if (detailedUiChecking)
            {
                ClickControl(XPath.Button.PaymentMethodBankAccount);
                EnterInvalidNoMatchBSBAndCheckErrorMessage(quoteBoat.PayMethod);
            }

            if (quoteBoat.PayMethod.IsPaymentByBankAccount) 
            {
                ClickControl(XPath.Button.PaymentMethodBankAccount);
            }
            else
            {
                ClickControl(XPath.Button.PaymentMethodCreditCard);
            }
            EnterPaymentDetailsAndPurchasePolicy(Vehicle.Boat, quoteBoat.PayMethod);
        }
    }
}
