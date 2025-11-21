using Rac.TestAutomation.Common;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.General;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace UIDriver.Pages.Spark.CaravanQuote
{
    public class PaymentDetails : SparkPaymentPage
    {
        #region XPATHS
        private class XPath
        {
            public const string ShowHideAdditionalInformation = "//a[@data-testid='toggle_quote_information']";

            public static class Core
            {
                public const string Caravan = "//p[@data-testid='policySummary-caravanDetails-caravanInformation']";
                public const string AgreedValue = "//p[@data-testid='policySummary-caravanDetails-agreedValue']";
                public const string BasicExcess = "//p[@data-testid='policySummary-caravanDetails-basicExcess']";
                public const string ContentsCover = "//p[@data-testid='policySummary-caravanDetails-contentsCover']";
            }
            public static class Usage
            {
                public const string Purpose = "//p[@data-testid='policySummary-caravanUsage-noBusinessUse']";
                public const string Suburb = "//p[@data-testid='policySummary-caravanUsage-keptIn']";
                public const string Parked = "//p[@data-testid='policySummary-caravanUsage-overnightParking']";
            }
            public static class Driver
            {
                public const string AccidentHistory = "//p[@data-testid='policySummary-drivingHistory-noAccidents']";
                public const string CancellationHistory = "//p[@data-testid='policySummary-drivingHistory-noSuspension']";
            }
            public static class Policy
            {
                public const string StartDate = "//p[@data-testid='policySummary-policyStartDate-date']";
            }
            public static class MainPH
            {
                public const string Name = "//p[@data-testid='policySummary-moreAboutYou-fullName']";
                public const string Gender = "//p[@data-testid='policySummary-moreAboutYou-gender']";
                public const string Phone = "//p[@data-testid='policySummary-moreAboutYou-contactNumber']";
                public const string Email = "//p[@data-testid='policySummary-moreAboutYou-email']";
                public const string Address = "//p[@data-testid='policySummary-moreAboutYou-mailingAddress']";
                public const string DOB = "//p[@data-testid='policySummary-moreAboutYou-dateOfBirth']";
            }
            public static class JointPH
            {
                public const string Name = "//p[@data-testid='policySummary-aboutJointPolicyHolder-fullName']";
                public const string Gender = "//p[@data-testid='policySummary-aboutJointPolicyHolder-gender']";
                public const string Phone = "//p[@data-testid='policySummary-aboutJointPolicyHolder-contactNumber']";
                public const string Email = "//p[@data-testid='policySummary-aboutJointPolicyHolder-email']";
                public const string Address = "//p[@data-testid='policySummary-aboutJointPolicyHolder-mailingAddress']";
                public const string DOB = "//p[@data-testid='policySummary-aboutJointPolicyHolder-dateOfBirth']";
            }
        }
        #endregion

        #region Settable properties and controls
        public string CaravanModel => GetInnerText(XPath.Core.Caravan);
        public int AgreedValue
        {
            get
            {
                var agreedValue = GetInnerText(XPath.Core.AgreedValue);
                var val = Regex.Match(agreedValue.Replace(",", ""), @"\d+$").Value;
                return int.Parse(val);
            }
        }

        public int BasicExcess
        {
            get
            {
                var basicExcess = GetInnerText(XPath.Core.BasicExcess);
                var val = Regex.Match(basicExcess, @"\d+$").Value;
                return int.Parse(val);
            }
        }
        public int ContentsCover
        {
            get
            {
                var contentsCover = GetInnerText(XPath.Core.ContentsCover);
                var val = Regex.Match(contentsCover.Replace(",", ""), @"\d+$").Value;
                return int.Parse(val);
            }
        }
        public string CaravanUsage => GetInnerText(XPath.Usage.Purpose);

        public string CaravanKept
        {
            get
            {
                var text = GetInnerText(XPath.Usage.Suburb);
                //Striped out 'Kept in'
                return text.Substring(8);
            }
        }

        public string CaravanParked => GetInnerText(XPath.Usage.Parked);

        public string DriverAccidentHistory => GetInnerText(XPath.Driver.AccidentHistory);
        public string DriverLicenceHistory => GetInnerText(XPath.Driver.CancellationHistory);

        public string PolicyStartDate
        {
            get
            {
                var text = GetInnerText(XPath.Policy.StartDate);
                Match match = Regex.Match(text, @"\d{2}\/\d{2}\/\d{4}");
                return match.Value;
            }
        }

        public string PolicyHolderName => GetInnerText(XPath.MainPH.Name);
        public string PolicyHolderGender => GetInnerText(XPath.MainPH.Gender);
        public string PolicyHolderPhoneNumber => GetInnerText(XPath.MainPH.Phone);
        public string PolicyHolderEmail => GetInnerText(XPath.MainPH.Email);
        public string PolicyHolderAddress => GetInnerText(XPath.MainPH.Address);
        public string PolicyHolderDateOfBirth
        {
            get
            {
                var text = GetInnerText(XPath.MainPH.DOB);
                Match match = Regex.Match(text, @"\d{2}\/\d{2}\/\d{4}");
                return match.Value;
            }
        }

        public string JointPolicyHolderName => GetInnerText(XPath.JointPH.Name);
        public string JointPolicyHolderGender => GetInnerText(XPath.JointPH.Gender);
        public string JointPolicyHolderPhoneNumber => GetInnerText(XPath.JointPH.Phone);
        public string JointPolicyHolderEmail => GetInnerText(XPath.JointPH.Email);
        public string JointPolicyHolderAddress => GetInnerText(XPath.JointPH.Address);
        public string JointPolicyHolderDateOfBirth
        {
            get
            {
                var text = GetInnerText(XPath.JointPH.DOB);
                Match match = Regex.Match(text, @"\d{2}\/\d{2}\/\d{4}");
                return match.Value;
            }
        }
        #endregion

        public PaymentDetails(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                //TODO: Enable ones we have a header on this page
                //GetElement(XP_HEADER);
                GetElement(XPathPayment.Button.Submit);
                GetElement(XPathPayment.Detail.FrequencyLabel);
                GetElement(XP_PAYMENT_FREQUENCY_ANNUAL);
                GetElement(XP_PAYMENT_FREQUENCY_MONTHLY);
                GetElement(XPathPayment.Detail.MethodLabel);
                GetElement(XPathPayment.Button.MethodCard);
                GetElement(XPathPayment.Button.MethodBankAccount);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Caravan Quote page - Payment");
            return true;
        }

        //Verify policy summary details
        public void VerifyPolicySummary(QuoteCaravan quoteCaravan)
        {
            //Verify policy summary details Caravan Model, Agreed Value, Basic Excess and Content Cover in the payment screen
            Reporting.Log("Verifying Policy Summary details on the payment page");
            VerifyCaravanDetails(quoteCaravan);
            ClickExpandQuoteInformation();
            VerifyCaravanUsage(quoteCaravan);
            VerifyDrivingHistory(quoteCaravan);
            Reporting.AreEqual(quoteCaravan.StartDate.ToString(DateTimeTextFormat.ddMMyyyy), PolicyStartDate, "Policy Start Date in Policy Summary Screen");
            VerifyPolicyHolderDetails(quoteCaravan);

        }

        //Verify caravan details
        private void VerifyCaravanDetails(QuoteCaravan quoteCaravan)
        {
            Reporting.AreEqual($"{quoteCaravan.Year} {quoteCaravan.Make} {quoteCaravan.ModelDescription}", CaravanModel, ignoreCase: true, comparisonContext: "Your Caravan in Policy Summary Screen");
            Reporting.AreEqual(quoteCaravan.SumInsuredValue, AgreedValue, "Agreed Value in Policy Summary Screen");
            Reporting.AreEqual(quoteCaravan.Excess, BasicExcess, "Basic Excess Value in Policy Summary Screen");
            Reporting.AreEqual(quoteCaravan.ContentsSumInsured, ContentsCover, "Contents Cover Value in Policy Summary Screen");
        }

        //Verify caravan usage
        private void VerifyCaravanUsage(QuoteCaravan quoteCaravan)
        {
            Reporting.AreEqual(Constants.PolicyCaravan.CARAVAN_PERSONAL_USAGE_TEXT, CaravanUsage, "Caravan usage in Policy Summary Screen");
            Reporting.AreEqual($"{quoteCaravan.ParkingAddress.Suburb.ToTitleCase()} {quoteCaravan.ParkingAddress.PostCode}", CaravanKept, "Caravan kept in Policy Summary Screen");
            Reporting.AreEqual(CaravanParkedText[quoteCaravan.ParkLocation].ToString(), CaravanParked, "Caravan Parked in Policy Summary Screen");
        }

        //Verify driver history
        private void VerifyDrivingHistory(QuoteCaravan quoteCaravan)
        {
            Reporting.AreEqual(Constants.PolicyCaravan.DRIVING_HISTORY_NO_ACCIDENT_TEXT, DriverAccidentHistory, "Accident history in Policy Summary Screen");
            Reporting.AreEqual(Constants.PolicyCaravan.DRIVING_HISTORY_NO_CANCELLATION_TEXT, DriverLicenceHistory, "Licence history in Policy Summary Screen");
        }


        //Verify policyholder details
        private void VerifyPolicyHolderDetails(QuoteCaravan quoteCaravan)
        {
            Reporting.AreEqual($"{quoteCaravan.PolicyHolders[0].Title} {quoteCaravan.PolicyHolders[0].FirstName} {quoteCaravan.PolicyHolders[0].Surname}", PolicyHolderName, ignoreCase: true, "Policy holder name on policy summary screen (NOT CASE SENSITIVE)");
            if (quoteCaravan.PolicyHolders[0].Title == Constants.Contacts.Title.Dr || quoteCaravan.PolicyHolders[0].Title == Constants.Contacts.Title.Mx)
            {
                Reporting.AreEqual(quoteCaravan.PolicyHolders[0].GenderString, PolicyHolderGender, "Policy holder gender on policy summary screen");
            }
            Reporting.AreEqual(quoteCaravan.PolicyHolders[0].MobilePhoneNumber, PolicyHolderPhoneNumber, "Policy holder phone number on policy summary screen");
            Reporting.AreEqual(quoteCaravan.PolicyHolders[0].PrivateEmail.Address, PolicyHolderEmail, ignoreCase: true, "Policy holder email on policy summary screen");
            Reporting.IsTrue(quoteCaravan.PolicyHolders[0].MailingAddress.IsEqualToString(PolicyHolderAddress), $"Policy holder address {quoteCaravan.PolicyHolders[0].MailingAddress.QASStreetAddress()} matches the shown address of {PolicyHolderAddress} on policy summary screen");
            Reporting.AreEqual(quoteCaravan.PolicyHolders[0].DateOfBirth.ToString(Constants.General.DateTimeTextFormat.ddMMyyyy), PolicyHolderDateOfBirth, "Policy holder date of birth on policy summary screen");

            //Verify additional policy holder details
            if (quoteCaravan.PolicyHolders.Count > 1)
            {
                Reporting.AreEqual($"{quoteCaravan.PolicyHolders[1].Title} {quoteCaravan.PolicyHolders[1].FirstName} {quoteCaravan.PolicyHolders[1].Surname}", JointPolicyHolderName, ignoreCase: true, comparisonContext: "Joint Policy holder name on policy summary screen (NOT CASE SENSITIVE)");
                if (quoteCaravan.PolicyHolders[1].Title == Constants.Contacts.Title.Dr || quoteCaravan.PolicyHolders[1].Title == Constants.Contacts.Title.Mx)
                {
                    Reporting.AreEqual(quoteCaravan.PolicyHolders[1].GenderString, JointPolicyHolderGender, "Joint Policy holder gender on policy summary screen");
                }
                Reporting.AreEqual(quoteCaravan.PolicyHolders[1].MobilePhoneNumber, JointPolicyHolderPhoneNumber, "Joint Policy holder phone number on policy summary screen");
                Reporting.AreEqual(quoteCaravan.PolicyHolders[1].PrivateEmail.Address, JointPolicyHolderEmail, ignoreCase: true, "Joint Policy holder email on policy summary screen");
                Reporting.IsTrue(quoteCaravan.PolicyHolders[1].MailingAddress.IsEqualToString(JointPolicyHolderAddress), $"Joint Policy holder address {quoteCaravan.PolicyHolders[1].MailingAddress.QASStreetAddress()} matches the shown address of {JointPolicyHolderAddress} on policy summary screen");
                Reporting.AreEqual(quoteCaravan.PolicyHolders[1].DateOfBirth.ToString(Constants.General.DateTimeTextFormat.ddMMyyyy), JointPolicyHolderDateOfBirth, "Joint Policy holder date of birth on policy summary screen");
            }
        }

        public void VerifyPaymentAmount(QuoteCaravan quoteCaravan)
        {
            //Verify if Payment Amount is same as what we selected on 'Here's your quote' page
            if (quoteCaravan.PayMethod.PaymentFrequency == PaymentFrequency.Annual)
            { Reporting.AreEqual(quoteCaravan.QuoteData.AnnualPremium, PaymentAmount, "Annual Payment Amount has the correct value"); }
            else
            { Reporting.AreEqual(quoteCaravan.QuoteData.MonthlyPremium, PaymentAmount, "Monthly Payment Amount has the correct value"); }
        }

        private void ClickExpandQuoteInformation()
        {
            ClickControl(XPath.ShowHideAdditionalInformation);
        }

        /// <summary>
        /// Ignore CSS from visual testing
        /// </summary>
        public List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
              "p[data-testid='policySummary-caravanDetails-caravanInformation']", // make and model
              "p[data-testid='policySummary-caravanDetails-agreedValue']",   // SI
              "p[data-testid='policySummary-caravanDetails-basicExcess']",   // excess
              "p[data-testid='policySummary-caravanDetails-contentsCover']", // SI Contents
              "[data-testid='label-payment-amount']", // premium
              "div[class*='css-dsartl']"              // "paying annually is cheaper..." text
          };
    }
}