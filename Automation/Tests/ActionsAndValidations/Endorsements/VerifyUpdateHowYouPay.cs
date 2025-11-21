using Rac.TestAutomation.Common;
using System;
using System.Linq;
using UIDriver.Pages.Spark;
using UIDriver.Pages.Spark.Endorsements;
using Rac.TestAutomation.Common.DatabaseCalls.Policies;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using System.Globalization;
using System.Text.RegularExpressions;
using UIDriver.Pages.Spark.Endorsements.UpdateHowYouPay;

namespace Tests.ActionsAndValidations.Endorsements
{
    public class VerifyUpdateHowYouPay
    {
        /// <summary>
        /// Checks the initial page content on loading the Update How You Pay page
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testData"></param>
        public static void VerifyDetailsPageLoaded(Browser browser, EndorsementBase testData)
        {
            using (var updateHowYouPayDetails = new UpdateHowYouPayDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(WaitTimes.T60SEC);
                Reporting.Log("Check current payment details are displayed", browser.Driver.TakeSnapshot());

                Reporting.IsTrue(updateHowYouPayDetails.IsDisplayed(), "Update How You Pay page has loaded");
                if (testData.GetType() == typeof(EndorseCar))
                {
                    VerifyMotorPolicyCardIsDisplayed(browser, (EndorseCar)testData);
                }
                else if (testData.GetType() == typeof(EndorseHome))
                {
                    VerifyHomePolicyCardIsDisplayed(browser, (EndorseHome)testData);
                }
                else if (testData.GetType() == typeof(EndorseCaravan))
                {
                    VerifyCaravanPolicyCardIsDisplayed(browser, (EndorseCaravan)testData);
                }
                else if (testData.GetType() == typeof(EndorseMotorCycle))
                {
                    VerifyMotorCyclePolicyCardIsDisplayed(browser, (EndorseMotorCycle)testData);
                }

                var paymentFrequency = (testData.PayMethod.PaymentFrequency == PaymentFrequency.Annual) ? "Annually" : "Monthly";

                Reporting.AreEqual(paymentFrequency, updateHowYouPayDetails.Frequency,"payment frequency is reported correctly");

                //installment amount and collection date doesn't display if existing payment ype is Annual direct debit with bank account
                if (!(testData.PayMethod.IsAnnual && testData.PayMethod.IsPaymentByBankAccount))
                {

                    VerifyInstalmentAmount(browser, testData.OriginalPolicyData.NextPendingInstallment().Amount.Total);

                    // Date displayed is the latter of Current Due Date and Collection Due Date 
                    var currentDay = DateTime.Now.Date;
                    if (testData.OriginalPolicyData.NextPendingInstallment().CollectionDate.Date <= currentDay.Date)
                    {
                        Reporting.AreEqual(currentDay, updateHowYouPayDetails.NextPaymentDate, "default date is today as collection date is today or in the past");
                    }
                    else
                    {
                        Reporting.AreEqual(testData.OriginalPolicyData.NextPendingInstallment().CollectionDate, updateHowYouPayDetails.NextPaymentDate, "future collection date is the default for next payment date");
                    }
                }                

                updateHowYouPayDetails.VerifyAccountSelectorPreamble();
                if (testData.PayMethod.IsPaymentByBankAccount)
                {
                    updateHowYouPayDetails.VerifySelectedBankAccount(testData.ActivePolicyHolder.BankAccounts[1]);
                } else
                {
                    updateHowYouPayDetails.VerifySelectedCreditCard(testData.ActivePolicyHolder.CreditCards[1]);
                }

                VerifyAuthorisationText(browser);
                updateHowYouPayDetails.VerifyTermsAndConditionsInitialState();
            }
        }

        /// <summary>
        /// Check the authorisation message displayed on the Page.
        /// Regardless of the Payment method/frequency ; 'How Update I Pay' Page is showing same message
        /// </summary>
        /// <param name="browser"></param>
        public static void VerifyAuthorisationText(Browser browser)
        {
            using (var updateHowYouPayDetails = new UpdateHowYouPayDetailsPage(browser))
            {
                var paymentAuthorisationRegex = new Regex(FixedTextRegex.PAYMENT_ANNUAL_BANK_MONTHLY_CREDIT_CARD_BANK_AUTHORISATION_REGEX);
                var paymentAuthorisationText = updateHowYouPayDetails.AuthorisationText;
                Match match = paymentAuthorisationRegex.Match(paymentAuthorisationText);

                Reporting.IsTrue(match.Success, $"Payment Authorisation Message Displayed. Actual Result: {paymentAuthorisationText}");
                Reporting.Log($"Capturing Authorisation Text Card", browser.Driver.TakeSnapshot());
            }            
        }

        private static void VerifyDetailsPageAfterUpdate(Browser browser, EndorsementBase testData)
        {
            using (var updateHowYouPayDetails = new UpdateHowYouPayDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish(WaitTimes.T60SEC);
                Reporting.Log("Check current payment details are displayed", browser.Driver.TakeSnapshot());

                Reporting.IsTrue(updateHowYouPayDetails.IsDisplayed(), "Update How You Pay page has loaded after updated");
                if (testData.GetType() == typeof(EndorseCar))
                {
                    VerifyMotorPolicyCardIsDisplayed(browser, (EndorseCar)testData);
                }
                else if (testData.GetType() == typeof(EndorseHome))
                {
                    VerifyHomePolicyCardIsDisplayed(browser, (EndorseHome)testData);
                }

                Reporting.AreEqual(testData.PayMethod.PaymentFrequency.GetDescription(), updateHowYouPayDetails.Frequency,
                    "payment frequency is reported correctly");

                VerifyInstalmentAmount(browser, testData.OriginalPolicyData.NextPendingInstallment().Amount.Total);
                Reporting.AreEqual(testData.NextPaymentDate, updateHowYouPayDetails.NextPaymentDate, "next collection date has been updated correctly");

                Reporting.AreEqual(testData.ActivePolicyHolder.GetEmail().ToLower(), updateHowYouPayDetails.EmailAddress.ToLower(), "email address update has updated correctly");

                updateHowYouPayDetails.VerifyAccountSelectorPreamble();
                updateHowYouPayDetails.VerifyTermsAndConditionsInitialState();
            }
        }

        public static void VerifyDetailsPageAfterUpdate(Browser browser, EndorsementBase testData, BankAccount bankAccount)
        {
            VerifyDetailsPageAfterUpdate(browser, testData);

            using (var updateHowYouPayDetails = new UpdateHowYouPayDetailsPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                updateHowYouPayDetails.VerifySelectedBankAccount(bankAccount);
            }
        }

        public static void VerifyDetailsPageAfterUpdate(Browser browser, EndorsementBase testData, CreditCard creditCard)
        {
                VerifyDetailsPageAfterUpdate(browser, testData);

                using (var updateHowYouPayDetails = new UpdateHowYouPayDetailsPage(browser))
                using (var spinner = new SparkSpinner(browser))
                {
                    updateHowYouPayDetails.VerifySelectedCreditCard(creditCard);
                }
            }



        /// <summary>
        /// Check the installment amount is shown correctly, making allowances for when it is a whole dollar amount
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="amount"></param>
        public static void VerifyInstalmentAmount(Browser browser, Decimal amount)
        {
            using (var updateHowYouPayDetails = new UpdateHowYouPayDetailsPage(browser))
            {
                // When whole dollar amount, then no cents displayed
                if (amount % 1 == 0)
                {
                    Reporting.AreEqual(String.Format("${0:0}", amount), updateHowYouPayDetails.ScheduledAmount, "payment amount is displayed correctly (whole dollar only)");
                }
                else
                {
                    Reporting.AreEqual(String.Format("${0:0.00}", amount), updateHowYouPayDetails.ScheduledAmount, "payment amount is displayed correctly");
                }
            }
        }

        public static void VerifyConfirmationPageWhenUsingCreditCard(Browser browser, EndorsementBase testData)
        {
            using (var updateHowYouPayConfirmation = new UpdateHowYouPayConfirmationPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish();
                Reporting.Log("Confirmation Page loaded, ready to check confirmation text", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(updateHowYouPayConfirmation.IsDisplayed(), "Confirmation Page has loaded");
                updateHowYouPayConfirmation.VerifyUpdateConfirmationMessage(testData, isNewPaymentBankAccount: false);
                updateHowYouPayConfirmation.VerifyBackToMyPolicy(testData.PolicyNumber);
            }
        }

        public static void VerifyConfirmationPageWhenUsingBankAccount(Browser browser, EndorsementBase testData)
        {
            using (var updateHowYouPayConfirmation = new UpdateHowYouPayConfirmationPage(browser))
            using (var spinner = new SparkSpinner(browser))
            {
                spinner.WaitForSpinnerToFinish();
                Reporting.Log("Confirmation Page loaded, ready to check confirmation text", browser.Driver.TakeSnapshot());
                Reporting.IsTrue(updateHowYouPayConfirmation.IsDisplayed(), "Confirmation Page has loaded");
                updateHowYouPayConfirmation.VerifyUpdateConfirmationMessage(testData, isNewPaymentBankAccount: true);
                updateHowYouPayConfirmation.VerifyBackToMyPolicy(testData.PolicyNumber);
            }
        }

        /// <summary>
        /// For a motor policy, check the policy card contains the correct details.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testData"></param>
        public static void VerifyMotorPolicyCardIsDisplayed(Browser browser, EndorseCar testData)
        {
            using (var policyCard = new PolicyInformationComponent(browser))
            {
                var vehicle = DataHelper.GetVehicleDetails(testData.OriginalPolicyData.MotorAsset.VehicleId).Vehicles[0];

                Reporting.IsTrue(policyCard.IsDisplayed(testData.PolicyNumber), "Policy card is present");
                Reporting.IsTrue(policyCard.PolicyDetailsCardTitle(testData.PolicyNumber).Contains($"Car insurance"), "title includes 'Car insurance'");

                Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 0, "model").Contains(vehicle.MakeDescription), "information about the car make and model is present");

                if(DataHelper.IsRegistrationNumberConsideredValid(testData.OriginalPolicyData.MotorAsset.RegistrationNumber))
                {
                    Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 1, "registration").Contains($"Registration: {testData.OriginalPolicyData.MotorAsset.RegistrationNumber}"), "car registration is displayed");
                    Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 2, "policy-number").Contains($"Policy number: {testData.PolicyNumber}"), "displays correct policy number in the policy details card");
                }
                else
                {
                    // Registration number is not to be shown and policy number is to moves up by one slot in the display
                    Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 1, "policy-number").Contains($"Policy number: {testData.PolicyNumber}"), "displays correct policy number in the policy details card");
                }
            }
        }

        public static void  VerifyCaravanPolicyCardIsDisplayed(Browser browser, EndorseCaravan testData)
        {
            using (var policyCard = new PolicyInformationComponent(browser))
            {
                var caravan = DataHelper.GetVehicleDetails(testData.OriginalPolicyData.CaravanAsset.VehicleId).Vehicles[0];

                Reporting.IsTrue(policyCard.IsDisplayed(testData.PolicyNumber), "Policy card is present");
                Reporting.IsTrue(policyCard.PolicyDetailsCardTitle(testData.PolicyNumber).Contains($"Caravan and trailer insurance"), "title includes 'Caravan and trailer insurance'");

                Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 0, "model").Contains(caravan.MakeDescription), "information about the caravan make and model is present");

                if (DataHelper.IsRegistrationNumberConsideredValid(testData.OriginalPolicyData.CaravanAsset.RegistrationNumber))
                {
                    Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 1, "registration").Contains($"Registration: {testData.OriginalPolicyData.CaravanAsset.RegistrationNumber}"), "caravan/trailer registration is displayed");
                    Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 2, "policy-number").Contains($"Policy number: {testData.PolicyNumber}"), "displays correct policy number in the policy details card");
                }
                else
                {
                    // Registration number is not to be shown and policy number is to moves up by one slot in the display
                    Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 1, "policy-number").Contains($"Policy number: {testData.PolicyNumber}"), "displays correct policy number in the policy details card");
                }
            }
        }

        public static void VerifyMotorCyclePolicyCardIsDisplayed(Browser browser, EndorseMotorCycle testData)
        {
            using (var policyCard = new PolicyInformationComponent(browser))
            {
                var motorCycle = DataHelper.GetVehicleDetails(testData.OriginalPolicyData.MotorcycleAsset.VehicleId).Vehicles[0];

                Reporting.IsTrue(policyCard.IsDisplayed(testData.PolicyNumber), "Policy card is present");
                Reporting.IsTrue(policyCard.PolicyDetailsCardTitle(testData.PolicyNumber).Contains($"Motorcycle insurance"), "title includes 'Motorcycle insurance'");

                Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 0, "model").Contains(motorCycle.MakeDescription), "information about the motor cycle make and model is present");

                if (DataHelper.IsRegistrationNumberConsideredValid(testData.OriginalPolicyData.MotorcycleAsset.RegistrationNumber))
                {
                    Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 1, "registration").Contains($"Registration: {testData.OriginalPolicyData.MotorcycleAsset.RegistrationNumber}"), "motor cycle registration is displayed");
                    Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 2, "policy-number").Contains($"Policy number: {testData.PolicyNumber}"), "displays correct policy number in the policy details card");
                }
                else
                {
                    // Registration number is not to be shown and policy number is to moves up by one slot in the display
                    Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 1, "policy-number").Contains($"Policy number: {testData.PolicyNumber}"), "displays correct policy number in the policy details card");
                }
            }
        }

        public static void VerifyHomePolicyCardIsDisplayed(Browser browser, EndorseHome testData)
        {
            using (var policyCard = new PolicyInformationComponent(browser))
            {
                Reporting.IsTrue(policyCard.IsDisplayed(testData.PolicyNumber), "Policy card is present");
                Reporting.IsTrue(policyCard.PolicyDetailsCardTitle(testData.PolicyNumber).Contains("Home insurance"), "title includes home insurance");
                Reporting.IsTrue(policyCard.PolicyDetailsCardProperty(testData.PolicyNumber, 1, "policy-number").Contains($"Policy number: {testData.PolicyNumber}"), "displays correct policy number in the policy details card");
                Reporting.IsTrue(policyCard.PolicyDetailsCardProperties(testData.PolicyNumber).Contains($"{testData.OriginalPolicyData.HomeAsset.Address.StreetNumber} {testData.OriginalPolicyData.HomeAsset.Address.StreetOrPOBox}"),
                    "policy card includes the street name and number");
            }
        }

        /// <summary>
        /// Check the details when the next instalment is updated and the payment source is a bank account
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="testData"></param>
        /// <param name="bankAccount"></param>
        public static void VerifyUpdateOccursInShieldWithBankAccountAsSource(Browser browser, EndorsementBase testData, BankAccount bankAccount)
        {
            var policyInstalmentDetails = ShieldPolicyDB.FetchNextInstalmentDetails(testData.PolicyNumber);
            var endorsementDetails = ShieldPolicyDB.FetchPaymentUpdateDetailsForPolicy(testData.PolicyNumber, testData.ActivePolicyHolder.Id);

            // Instalment Details
            Reporting.AreEqual(testData.OriginalPolicyData.NextPendingInstallment().Amount.Total, policyInstalmentDetails.AmountDue, "instalment amount has remained the same");
            Reporting.AreEqual(bankAccount.Bsb, policyInstalmentDetails.BankAccount.Bsb, "BSB updated");
            Reporting.AreEqual(bankAccount.AccountNumber, policyInstalmentDetails.BankAccount.AccountNumber, "account number updated coorectly");
            Reporting.AreEqual(bankAccount.AccountName, policyInstalmentDetails.BankAccount.AccountName, "account name updated");
            Reporting.AreEqual(testData.NextPaymentDate, policyInstalmentDetails.CurrentCollectionDate, "correct instalment date shown");

            // Email updated
            Reporting.AreEqual(testData.ActivePolicyHolder.GetEmail().ToLower(), endorsementDetails.Email.ToLower(),
                "email updated correctly");

            // Shield Events
            Reporting.IsTrue(endorsementDetails.PaymentChangeEventGenerated, "policy change event registered ");
            Reporting.IsTrue(endorsementDetails.PrintEventGenerated, "policy print invoked ");
            Reporting.IsTrue(endorsementDetails.EndorsementForDebitAdmendment, "endorsement for debit admendment");
            Reporting.AreEqual(1, endorsementDetails.EventCount, "only one event created in Shield");
        }

        /// <summary>
        /// When the next instalment using a credit card is update, check the details are correct.
        /// </summary>        
        /// <param name="testData"></param>
        /// <param name="creditCard"></param>
        public static void VerifyUpdateOccursInShieldWithCreditCardAsSource(EndorsementBase testData, CreditCard creditCard)
        {
            var policyInstalmentDetails = ShieldPolicyDB.FetchNextInstalmentDetails(testData.PolicyNumber);
            var endorsementDetails = ShieldPolicyDB.FetchPaymentUpdateDetailsForPolicy(testData.PolicyNumber, testData.ActivePolicyHolder.Id);

            //not required when there is no current installement amount in policy 
            if (testData.OriginalPolicyData.NextPendingInstallment() != null)
            {
                Reporting.AreEqual(testData.OriginalPolicyData.NextPendingInstallment().Amount.Total, policyInstalmentDetails.AmountDue, "instalment amount has remained the same");
            }
            
            Reporting.AreEqual(creditCard.CardIssuer, policyInstalmentDetails.CreditCard.CardIssuer, "card issuer is correct");
            Reporting.AreEqual(creditCard.CardNumber.Substring(creditCard.CardNumber.Length - 4), policyInstalmentDetails.CreditCard.CardNumber.Substring(policyInstalmentDetails.CreditCard.CardNumber.Length - 4), "card token is correct");
            Reporting.AreEqual(creditCard.CardholderName, policyInstalmentDetails.CreditCard.CardholderName, "card holder name is correct");
            Reporting.AreEqual(creditCard.CardExpiryDate.ToString(DataFormats.DATE_MONTH_YEAR_FORWARDSLASH, CultureInfo.InvariantCulture), policyInstalmentDetails.CreditCard.CardExpiryDate.ToString(DataFormats.DATE_MONTH_YEAR_FORWARDSLASH, CultureInfo.InvariantCulture),
                "credit card expiry date");
            Reporting.AreEqual(testData.NextPaymentDate, policyInstalmentDetails.CurrentCollectionDate, "correct instalment date shown");

            // Email updated
            Reporting.AreEqual(testData.ActivePolicyHolder.GetEmail().ToLower(), endorsementDetails.Email.ToLower(),
                "email updated correctly");

            // Shield Events
            Reporting.IsTrue(endorsementDetails.PaymentChangeEventGenerated, "policy change event registered");
            Reporting.IsTrue(endorsementDetails.PrintEventGenerated, "policy print invoked");
            Reporting.IsTrue(endorsementDetails.EndorsementForDebitAdmendment, "endorsement for debit admendment");
            Reporting.AreEqual(1, endorsementDetails.EventCount, "only one event created in Shield");
        }

        /// <summary>
        /// Verify the Uh oh error page content
        /// </summary>       
        public static void VerifyNotEligibleForOnlineUpdate(Browser browser, EndorsementBase endoresement)
        {
            using (var errorPage = new UhohErrorPage(browser))
            {
                errorPage.VerifyErrorPage(endoresement.OriginalPolicyData.EndorsementStartDate);
            }
        }
    }
}
