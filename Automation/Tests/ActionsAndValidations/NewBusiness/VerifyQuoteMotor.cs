using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;
using System.Collections.Generic;
using System;
using UIDriver.Pages.B2C;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.Contacts;
using System.Linq;

namespace Tests.ActionsAndValidations
{
    public static class VerifyQuoteMotor
    {
        public static void VerifyMotorVehiclePolicyInShield(QuoteCar vehicleQuote, string policyNumber)
        {
            Reporting.LogTestShieldValidations("policy", policyNumber);
            VerifyMotorVehiclePolicyInShieldBasicCoverDetails(vehicleQuote: vehicleQuote, policyNumber: policyNumber);
            VerifyMotorVehiclePolicyInShieldPaymentDetails(vehicleQuote: vehicleQuote, policyNumber: policyNumber);
            VerifyMotorVehiclePolicyInShieldDriverDetails(vehicleQuote: vehicleQuote, policyNumber: policyNumber);
            VerifyMotorVehiclePolicyInShieldVehicleDetails(vehicleQuote: vehicleQuote, policyNumber: policyNumber);
            VerifyPolicy.VerifyPolicyMultiMatchDetailsInShield(vehicleQuote.Drivers, policyNumber);
        }

        private static bool B2C4147registrationBug(QuoteCar vehicleQuote)
        {
            bool skipRegoCheck = false;
            DateTime thisDay = DateTime.Today;
            if (vehicleQuote.Registration != null)
            {
                if (vehicleQuote.StartDate.Date != DateTime.Now.Date)
                {
                    skipRegoCheck = true;
                    Reporting.Log($"skipRegoCheck evaluated as {skipRegoCheck} so registration will not be examined on the Confirmation screen (due to B2C-4147)");
                }
            }
            return skipRegoCheck;
        }

        public static void VerifyMotorVehiclePolicyInShieldBasicCoverDetails(QuoteCar vehicleQuote, string policyNumber)
        {
            // Verify General Policy details
            Reporting.Log($"Begin verify policy details from Shield DB. Param/s = {policyNumber}");
            var policyInfo = ShieldMotorDB.FetchMotorPolicyDetail(policyNumber);
            // Using "StartsWith()" as March 2019 product version has 'cloned' text in cover description
            Reporting.IsTrue(policyInfo.CoverType.StartsWith(MotorCoverNameMappings[vehicleQuote.CoverType].TextShield), "Verify Policy Cover Type in Shield DB");

            Reporting.AreEqual(vehicleQuote.StartDate.ToString("d/MMM/yy"), policyInfo.PolicyStartDate.ToString("d/MMM/yy"), " Policy Start Date Expected vs Actual");
            Reporting.AreEqual(vehicleQuote.Excess.StripMoneyNotations(), policyInfo.Excess.ToString(), " Excess Expected vs Actual");
        }

        /// <summary>
        /// Verifies new business motor policy's payment details against Shield DB
        /// </summary>
        /// <param name="testConfig"></param>
        /// <param name="vehicleQuote"></param>
        /// <param name="policyNumber"></param>
        public static void VerifyMotorVehiclePolicyInShieldPaymentDetails(QuoteCar vehicleQuote, string policyNumber)
        {
            Reporting.Log($"Begin verify payment details from Shield DB. Param/s = {policyNumber}");
            var dbPaymentDetails      = ShieldPolicyDB.FetchPaymentDetailsForPolicy(policyNumber);
            var recentEvents          = ShieldPolicyDB.FetchRecentEventsOnPolicy(policyNumber);

            Reporting.IsTrue(vehicleQuote.PayMethod.PaymentFrequency == dbPaymentDetails.PaymentFrequency, "payment frequency is expected value in DB");

            if (vehicleQuote.PayMethod.IsPaymentByBankAccount)
                Reporting.AreEqual(DIRECT_DEBIT, dbPaymentDetails.PaymentMethod);
            else
            {
                if (vehicleQuote.PayMethod.IsAnnual)
                {
                    Reporting.AreEqual(CASH, dbPaymentDetails.PaymentMethod);
                    VerifyPolicy.VerifyNewBusinessEventForAnnualCashPayment(recentEvents);
                }
                else
                    Reporting.AreEqual(CREDIT_CARD, dbPaymentDetails.PaymentMethod);
            }

            if (vehicleQuote.PayMethod.Scenario != PaymentScenario.AnnualCash)
                VerifyPolicy.VerifyNewBusinessEventForDirectDebitInstalmentPayment(recentEvents);

            var expectedPaymentFrequency = vehicleQuote.PayMethod.NumberOfPayments;
            Reporting.AreEqual(expectedPaymentFrequency, dbPaymentDetails.PaymentCount, "count of payments in DB is expected value");

            VerifyPolicy.VerifyNewPolicyInstalments(policyNumber);
        }

        public static void VerifyMotorVehiclePolicyInShieldDriverDetails(QuoteCar vehicleQuote, string policyNumber)
        {
            // Verify Financier Name via API:
            var policyDetails = DataHelper.GetPolicyDetails(policyNumber);
            Reporting.AreEqual(vehicleQuote.IsFinanced, policyDetails.MotorAsset.IsFinanced, "IsFinanced flag correctly set in Shield");
            if (vehicleQuote.IsFinanced)
            {
                Reporting.AreEqual(vehicleQuote.Financier, policyDetails.GetFinancierNameViaShieldAPI(), ignoreCase: true, $"Expected financier name against the actual name displayed");
            }

            // Verify Contacts - iterate through each driver
            for (int j = 0; j < vehicleQuote.Drivers.Count; j++)
            {
                Reporting.LogMinorSectionHeading($"Begin verify driver #{j + 1} from Shield DB. Params = {policyNumber}, {vehicleQuote.Drivers[j].Details.FirstName}");
                Reporting.Log("Sanitizing customer.SanitizeFirstNameValue() as sanitizedFirstNamePrimaryDriver.");
                string sanitizedFirstName = vehicleQuote.Drivers[j].Details.FirstName.SanitizeString();
                Reporting.Log($"sanitizedFirstNamePrimaryDriver = {sanitizedFirstName}");
                Reporting.Log($"Querying Shield DB for primary driver details.. Param/s = {policyNumber}, {sanitizedFirstName}");
                var contact = ShieldMotorDB.FetchContactFromMotorQuoteByFirstName(policyNumber, sanitizedFirstName);
                AssertDriverDetailsFromDatabase(vehicleQuote, j, contact);
            }
            Reporting.LogMinorSectionHeading($"Verify Preferred Delivery Method and email address for Main Policyholder");
            var actualDeliveryMethod = DataHelper.GetPreferredDeliveryMethodForMainPolicyholder(policyNumber);
            var mainPolicyHolder = vehicleQuote.Drivers.First(x => x.IsPolicyHolderDriver == true);
            var expectedDeliveryMethod = mainPolicyHolder.Details.IsEmailPreferredDeliveryMethod() ? PreferredDeliveryMethod.Email : PreferredDeliveryMethod.Mail;
            Reporting.AreEqual(expectedDeliveryMethod, actualDeliveryMethod, "the preferred delivery method");

            var mainPHContactFromShield = DataHelper.GetContactDetailsViaContactId(policyDetails.Policyholder.Id.ToString());
            Reporting.AreEqual(mainPolicyHolder.Details.PrivateEmail.Address, mainPHContactFromShield.GetEmail(),
                ignoreCase: true, "expected value for email address was successfully saved.");
        }

        /// <summary>
        /// If a Mobile telephone number was generated for this contact, then the actual contact should have that Mobile
        /// telephone number.
        /// If no Mobile telephone number was generated for this contact, then the actual Mobile telephone number value 
        /// returned from Shield should be null, as we would have input a Home telephone number instead. So we check for 
        /// IsNullOrEmpty regarding mobile, then confirm that the Home telephone number (AKA: Private telephone number) 
        /// matches the value generated.
        /// </summary>
        /// <param name="expectedContact">The test data generated for this contact in this test.</param>
        /// <param name="actualContact">The actual contact data returned from Shield for the contact in this test</param>
        public static void VerifyContactTelephoneNumber(Driver expectedContact, Contact actualContact)
        {
            if (!string.IsNullOrEmpty(expectedContact.Details.MobilePhoneNumber))
            {
                Reporting.AreEqual(expectedContact.Details.MobilePhoneNumber, actualContact.MobilePhoneNumber, "expected mobile phone number matches actual value");
            }
            else
            {
                Reporting.IsTrue(string.IsNullOrEmpty(actualContact.MobilePhoneNumber), "Mobile Telephone number is null or empty to match Expected Mobile Telephone number");

                if (!string.IsNullOrEmpty(expectedContact.Details.HomePhoneNumber))
                {
                    Reporting.AreEqual(expectedContact.Details.HomePhoneNumber, actualContact.HomePhoneNumber, "expected home phone number matches actual value");
                }
                else
                {
                    Reporting.IsTrue(string.IsNullOrEmpty(expectedContact.Details.HomePhoneNumber), "Home Telephone number is null or empty to match Expected Home Telephone number");
                }
            }
        }

        public static void VerifyMotorVehiclePolicyInShieldVehicleDetails(QuoteCar vehicleQuote, string policyNumber)
        {
            // Verify vehicle details.
            Reporting.Log($"Begin verify insured vehicle details from Shield DB. Params = {policyNumber}");
            var recordedVehicle = ShieldMotorDB.FetchVehicleFromMotorPolicy(policyNumber);
            Reporting.AreEqual(vehicleQuote.Make, recordedVehicle.Make, "vehicle make");
            Reporting.AreEqual(vehicleQuote.Year, recordedVehicle.Year, "vehicle year");
            Reporting.AreEqual(vehicleQuote.Model, recordedVehicle.Model, "vehicle model");
            // B2C modifies body type with door count.
            Reporting.IsTrue(recordedVehicle.Body.Contains(vehicleQuote.Body), "vehicle body");
            Reporting.AreEqual(vehicleQuote.Transmission, recordedVehicle.Transmission, "vehicle transmission");
        }

        /// <summary>
        /// Verifies the details on the Motor Quote Summary page.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="vehicleQuote"></param>
        /// <param name="insuredVehicle"></param>
        /// <param name="isPPQ"></param>
        public static void VerifyQuoteSummaryPage(Browser browser, QuoteCar vehicleQuote, string insuredVehicle, bool isPPQ = false)
        {
            using (var quotePage3  = new MotorQuote3Summary(browser))
            using (var paymentPage = new QuotePayments(browser))
            using (var spinner = new RACSpinner(browser))
            {
                Reporting.Log("Begin verify policy vehicle details", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(quotePage3.InsuredVehicle.Contains(insuredVehicle), "quote summary is displaying expected vehicle");
                Reporting.AreEqual(vehicleQuote.CoverType, quotePage3.CoverType, "requested motor vehicle cover type (enum) is as expected on summary page.");

                if (vehicleQuote.CoverType == MotorCovers.MFCO)
                        Reporting.AreEqual(vehicleQuote.AddHireCarAfterAccident ? "Included" : "Not included", quotePage3.HireCarStatus, "for Hire Car After Accident");

                if (vehicleQuote.CoverType == MotorCovers.MFCO)
                {
                    quotePage3.VerifyNCBProtectionIsNotDisplayed();
                }

                // B2C may interchange some drivers, so we use a temporary list
                // to search and eliminate from.
                var workingDriverList = new List<Driver>(vehicleQuote.Drivers);
                for (int i = 0; i < vehicleQuote.Drivers.Count; i++)
                {
                    Reporting.Log($"Begin verify driver on Policy Summary:{i}");
                    Reporting.IsTrue(quotePage3.VerifyDriverDetails(i, workingDriverList, vehicleQuote.Drivers[0].Details.MailingAddress, isPPQ), "driver details on policy summary view");
                }
            }
        }

        public static string VerifyMotorVehicleConfirmationPage(Browser browser, QuoteCar vehicleQuote, decimal expectedPrice, string insuredVehicle, out string receiptNumber)
        {
            var policyNumber = string.Empty;
            receiptNumber    = string.Empty;
            var regoSkip     = B2C4147registrationBug(vehicleQuote);

            using (var confirmation = new MotorQuoteConfirmation(browser))
            {
                confirmation.WaitForPage();
                Reporting.Log("Verifying details on confirmation screen.");

                policyNumber = confirmation.PolicyNumber;
                Reporting.Log($"Policy number is: {policyNumber}", browser.Driver.TakeSnapshot());

                switch (vehicleQuote.CoverType)
                {
                    case MotorCovers.MFCO:
                        Reporting.AreEqual("Comprehensive", confirmation.CoverType, "Motor cover type");
                        break;
                    case MotorCovers.TFT:
                        Reporting.AreEqual("Third Party Fire & Theft", confirmation.CoverType, "Motor cover type");
                        break;
                    case MotorCovers.TPO:
                        Reporting.AreEqual("Third Party Property", confirmation.CoverType, "Motor cover type");
                        break;
                    default:
                        Reporting.Error($"Unrecognised cover type of {vehicleQuote.CoverType}");
                        break;
                }
                
                if (regoSkip == false)
                {
                    Reporting.Log($"regoSkip = {regoSkip} - checking registration plate details");
                    if (vehicleQuote.Registration == null)
                        Reporting.AreEqual(REGISTRATION_NOT_PROVIDED, confirmation.CarRegistration);
                    else
                        Reporting.AreEqual(vehicleQuote.Registration, confirmation.CarRegistration);
                }


                Reporting.AreEqual(vehicleQuote.StartDate.ToString("d MMMM yyyy"), confirmation.PolicyStartDate, "Motor policy start date");
                Reporting.AreEqual(vehicleQuote.StartDate.AddYears(1).ToString("d MMMM yyyy"), confirmation.PolicyEndDate, "Motor policy end date");

                Reporting.Log("Policy Purchase Confirmation page, establishing sanitizedName from customer.SanitizeNameValues().");
                Reporting.AreEqual(vehicleQuote.Drivers.Count, confirmation.Drivers.Count, "count of drivers is as expected");
                foreach (var driver in vehicleQuote.Drivers)
                {
                    var sanitizedName = driver.Details.GetFullTitleAndName().SanitizeString();
                    Reporting.IsTrue(confirmation.Drivers.Contains(sanitizedName), "driver's name is correctly shown with restricted characters removed");
                    if (driver.IsPolicyHolderDriver)
                    {
                        Reporting.IsTrue(confirmation.PolicyHolders.Contains(sanitizedName), "policy holder's name is correctly shown with restricted characters removed");
                    }
                }

                Reporting.AreEqual(insuredVehicle, confirmation.CarDetail, true);

                if (vehicleQuote.AddRoadside)
                {
                    
                    if (vehicleQuote.PayMethod.Scenario != PaymentScenario.AnnualCash)
                    {
                        Reporting.AreEqual("Step 1 - Completed", confirmation.RoadsideStep1Header, "Roadside step 1 header text");
                        Reporting.AreEqual("Step 2 - Pending", confirmation.RoadsideStep2Header, "Roadside step 2 header text");
                        Reporting.IsTrue(confirmation.IsRoadsideStep2ButtonPresent, "visibility state of roadside assistance payment button");
                    }
                    else
                    {
                        // Verify that the new RSA fields should be displayed when there's an RSA added for payment methods which are paid annual cash (B2C-3831)
                        Reporting.IsTrue(confirmation.IsRoadsideAssistanceLabelDisplayed, "Roadside assistance label".IsDisplayed());
                        Reporting.AreEqual("Roadside Assistance:", confirmation.RoadsideAssistanceLabelText, "Roadside Assistance:".IsExpectedLabelText());
                        Reporting.AreEqual(MOTOR_ROADSIDE_AMOUNT, confirmation.RoadsideAssistanceAmount, "recorded RSA amount as expected");
                        Reporting.IsFalse(confirmation.IsClassicRoadsideAssistanceTextDisplayed, "Classic Roadside Assistance".IsNotDisplayed());
                        Reporting.Log($"RAC member number is: {confirmation.RoadsideMemberNumber}");
                        if (Config.Get().Azure.RoadsideBundling.HasLiveSync)
                        {
                            Reporting.IsFalse(confirmation.RoadsideMemberNumber == MEMBER_NUMBER_NOT_RETURNED, "RoadsideMemberNumber is not \"T.B.A.\"");
                        }
                        else
                        {
                            Reporting.Log($"Azure - Roadside Bundling - HasLiveSync (see config.json) is not true, so environment under test has no integration to produce RSA Member Number. Bypassing check, please ensure testing of RSA Bundling is completed in an integrated environment before code under test is deployed to Production.");
                        }
                    }
                }
                else
                {
                    if (vehicleQuote.PayMethod.Scenario != PaymentScenario.AnnualCash)
                    {
                        Reporting.IsFalse(confirmation.IsRoadsideAssistanceLabelDisplayed, "Roadside assistance label".IsNotDisplayed());

                        // Verify that no old RSA fields should be displayed when there's no RSA added for payment methods which are not paid annual cash (B2C-3831)
                        Reporting.IsFalse(confirmation.IsRoadsideAssistanceDisplayed, "Roadside assistance amount".IsNotDisplayed());
                        Reporting.IsFalse(confirmation.IsClassicRoadsideAssistanceTextDisplayed, "Classic Roadside Assistance".IsNotDisplayed());
                    }
                }

                // Verify Payment details section
                Reporting.IsTrue(confirmation.IsPaymentFrequencyLabelDisplayed, "Payment frequency label".IsDisplayed());
                Reporting.AreEqual("Payment frequency:", confirmation.PaymentFrequencyLabelText, "Payment frequency:");
                Reporting.AreEqual(vehicleQuote.PayMethod.PaymentFrequency, confirmation.PaymentFrequency, "Payment Frequency on confirmation page");

                if (vehicleQuote.PayMethod.IsAnnual)
                {
                    Reporting.IsTrue(confirmation.IsInsurancePremiumLabelDisplayed, "Insurance Premium label".IsDisplayed());
                    Reporting.AreEqual("Insurance Premium:", confirmation.InsurancePremiumLabelText, "Insurance Premium:");
                    Reporting.AreEqual(expectedPrice, confirmation.InsurancePremiumAmount, "recorded insurance premium as expected");

                    Reporting.IsFalse(confirmation.IsAmountLabelDisplayed, "Amount label".IsNotDisplayed());
                    Reporting.IsFalse(confirmation.IsAmountDisplayed, "Amount".IsNotDisplayed());
                }
                else
                {
                    Reporting.IsTrue(confirmation.IsAmountLabelDisplayed, "Amount label".IsDisplayed());
                    Reporting.AreEqual("Amount:", confirmation.AmountLabelText, "Amount:");
                    Reporting.AreEqual(expectedPrice, confirmation.AmountPaid, "recorded amount as expected");

                    Reporting.IsFalse(confirmation.IsInsurancePremiumLabelDisplayed, "Insurance Premium label".IsNotDisplayed());
                    Reporting.IsFalse(confirmation.IsInsurancePremiumDisplayed, "Insurance premium".IsNotDisplayed());
                }

                if (vehicleQuote.PayMethod.Scenario == PaymentScenario.AnnualCash)
                    receiptNumber = confirmation.VerifyReceiptNumberIsDisplayedCorrectly();
                else
                {
                    Reporting.IsFalse(confirmation.IsReceiptNumberLabelDisplayed, "Receipt number label".IsNotDisplayed());
                    Reporting.IsFalse(confirmation.IsReceiptNumberDisplayed, "Receipt number".IsNotDisplayed());
                }

            return policyNumber;
            }
        }

        private static void AssertDriverDetailsFromDatabase(QuoteCar testData, int sourceIndex, List<Contact> dbContacts)
        {
            var expectedDriver = testData.Drivers[sourceIndex];

            Reporting.IsTrue(dbContacts.Count > 0, $"policy contact {sourceIndex} was found with driver role");

            foreach (var contact in dbContacts)
            {
                Reporting.AreEqual(expectedDriver.Details.FirstName.SanitizeString(), contact.FirstName, true, "first name matches expected value");
                Reporting.AreEqual(expectedDriver.Details.Surname.SanitizeString(), contact.Surname, true, "surname matches expected value");
                Reporting.AreEqual(expectedDriver.Details.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH),
                                   contact.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH), "date of birth matches expected value");
                Reporting.AreEqual(expectedDriver.IsPolicyHolderDriver, contact.IsPolicyHolder, "current contact was attached ");

                if (sourceIndex == 0 || expectedDriver.Details.MailingAddress != null)
                {
                    Reporting.AreEqual(expectedDriver.Details.MailingAddress.StreetSuburbPostcode(expandUnitAddresses: true), contact.MailingAddress.StreetSuburbPostcode(expandUnitAddresses: true), true, "mailing address is as expected");
                }
                else
                {
                    var expectedAddr = testData.Drivers[0].Details.MailingAddress;
                    // Due to variations in unit/apartment/suite notations and how Shield presents them
                    // we just check the suburb and postcode. An annoyance with the way we get referenced mailing addresses.
                    Reporting.AreEqual(expectedAddr.SuburbAndCode(), contact.MailingAddress.SuburbAndCode(), true, "Mailing address");
                }
                VerifyContactTelephoneNumber(expectedDriver, contact);
            }
        }
    }
}