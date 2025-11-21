using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Spark.Endorsements.MakePayment
{
    public class PaymentPage : SparkPaymentPage
    {
        #region CONSTANTS
        private static class Constants
        {
            public static readonly string ApplicationName = "Make a payment";
            public static readonly string PageHeading = "Payment";

            public static class Label
            {
                public static readonly string AmountDue = "Amount due";

                public static class Stepper
                {
                    public static readonly string Payment = "Payment";
                    public static readonly string Confirmation = "Confirmation";
                }

                public static class PolicyCard
                {
                    public static readonly string BoatTitle = "Boat insurance";

                    //Car
                    public static readonly string CarTitle = "Car insurance";
                    public static readonly string CarCoverTypeSubMFCO = "Comprehensive";
                    public static readonly string CarCoverTypeSubTFT = "Third party fire & theft";
                    public static readonly string CarCoverTypeSubTPO = "Third party";
                    
                    //Home
                    public static readonly string HomeTitle = "Home insurance";
                    public static readonly string HomeCoverType = "Building & content";

                    //MotorCycle
                    public static readonly string MotorcycleTitle = "Motorcycle insurance";
                    public static readonly string MotorCycleCoverTypeSubTPPD = "Third party property damage";

                    public static readonly string PetTitle = "Pet insurance";
                    public static readonly string CaravanTitle = "Caravan and trailer insurance";
                    public static readonly string ElectricMobilityTitle = "Electric mobility insurance";
                }

                public static class FailedPayment
                {
                    public static readonly string Title = "We couldn't process your payment";
                }

                public static class CardEntry
                {
                    public static readonly string Title = "Pay by card";
                    public static readonly string SubTitle = "This will be a one-off payment";

                    public static readonly string Name = "Name on card";
                    public static readonly string CardNumber = "Card number";
                    public static readonly string ExpiryDate = "Expiry date";
                    public static readonly string CVC = "CVC";
                }
            }

            public static class Button
            {
                public static readonly string Confirm = "Confirm";
                public static readonly string Back = "Back";
            }
        }
        #endregion

        #region XPATHS
        private static class XPath
        {
            public static readonly string ApplicationName = "//h1[text()='" + Constants.ApplicationName + "']";
            public static readonly string PageHeading = "//h2[text()='" + Constants.PageHeading + "']";

            public static class Label
            {
                public static readonly string AmountDueLabel = "id('amount-label')";
                public static readonly string AmountDue = "id('amount-value')";
                public static readonly string CardPaymentAuthText = "";

                public static class Stepper
                {
                    public static readonly string Payment = "id('payment-step')";
                    public static readonly string Confirmation = "id('confirmation-step')";
                }
                public static class PolicyCard
                {
                    public static readonly string Title = "id('policy-card-content-policy-details-header-title-policy-details')";
                    public static readonly string CoverType = "id('policy-card-content-policy-details-header-subtitle-policy-details')";
                    public static readonly string Model = "id('policy-card-content-policy-details-property-0-model-policy-details')";
                    public static readonly string Type = "id('policy-card-content-policy-details-property-0-type-policy-details')";
                    public static readonly string Address = "id('policy-card-content-policy-details-property-0-address-policy-details')";
                    public static readonly string Registration = "id('policy-card-content-policy-details-property-1-registration-policy-details')";
                    public static readonly string PolicyNumber = "//p[contains(@id,'policy-number-policy-details') and contains(@id,'policy-card')]";
                    public static readonly string PetName = "id('policy-card-content-policy-details-property-0-name-policy-details')";
                    public static readonly string PetBreed = "id('policy-card-content-policy-details-property-1-breed-policy-details')";
                }

                public static class CardEntry
                {
                    public static readonly string Title = "//div[@id='stacked-card-container-content']/h2";
                    public static readonly string SubTitle = "//div[@id='stacked-card-container-content']/p";
                    public static readonly string Name = "//div[@id='cardholderNameField']";
                    public static readonly string CardNumber = "//div[@id='cardNumberField']";
                    public static readonly string ExpiryDate = "//div[@id='expiryDateField']/label";
                    public static readonly string CVC = "//div[@id='cvnField']";
                } 
                
                public static class FailedPayment
                {
                    public static readonly string Title = "id('westpac-realtime-payment-retry-dialog-title')";
                }
            }

            public static class EmailEntry
            {
                public static readonly string Email = "id('email')";
            }

            public static class Button
            {
                public static readonly string Confirm = "//button[text()='Confirm']";
                public static readonly string Back = "id('pcm-back-link')";
                public static readonly string BackToMyPolicy = "id('westpac-realtime-payment-retry-dialogback-to-my-policy')";
                public static readonly string TryAgain = "//button[@data-testid='westpac-realtime-payment-retry-dialog-try-again-button']";
            }
        }
        #endregion

        public PaymentPage(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.ApplicationName);
                GetElement(XPath.PageHeading);
                GetElement(XPath.Button.Confirm);
                GetElement(XPath.Button.Back);
                Reporting.Log($"Payment page is Displayed");
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.Log($"Page 1: Payment page state.", _browser.Driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// Checking the name & active/inactive stepper on the current page
        /// </summary>
        public void VerifyStepperNameAndState()
        {
            Reporting.AreEqual(Constants.Label.Stepper.Payment, GetInnerText(XPath.Label.Stepper.Payment), $"the expected stepper name '{Constants.Label.Stepper.Payment}' against the value displayed on the page.");
            Reporting.AreEqual(Constants.Label.Stepper.Confirmation, GetInnerText(XPath.Label.Stepper.Confirmation), $"the expected stepper name '{Constants.Label.Stepper.Confirmation}' against the value displayed on the page.");
            Reporting.IsTrue(bool.Parse(GetElement(XPath.Label.Stepper.Payment).GetAttribute("aria-selected")), $"the stepper {Constants.Label.Stepper.Payment} is active on the page.");
            Reporting.IsFalse(bool.Parse(GetElement(XPath.Label.Stepper.Confirmation).GetAttribute("aria-selected")), $"the stepper {Constants.Label.Stepper.Confirmation} is disabled on the page.");
        }

        /// <summary>
        /// Verify the Amount due and Card entry field names displayed on the page
        /// </summary>
        public void VerifyUILabels()
        {
            Reporting.AreEqual(Constants.Label.AmountDue, GetInnerText(XPath.Label.AmountDueLabel), $"the text '{Constants.Label.AmountDue}' displayed");

            // Card entry section
            Reporting.AreEqual(Constants.Label.CardEntry.Title, GetInnerText(XPath.Label.CardEntry.Title), $"the '{Constants.Label.CardEntry.Title}' label is displayed on the page");
            Reporting.AreEqual(Constants.Label.CardEntry.SubTitle, GetInnerText(XPath.Label.CardEntry.SubTitle), $"the '{Constants.Label.CardEntry.SubTitle}' label is displayed on the page");
           
            _driver.WaitForElementToBeVisible(By.XPath(XPathPayment.CreditCard.PaymentIframe), WaitTimes.T30SEC);
            _driver.SwitchTo().Frame(_driver.FindElement(By.XPath(XPathPayment.CreditCard.PaymentIframe)));  //Allows Selenium to access the objects inside the Credit Card payment iFrame
            Reporting.AreEqual(Constants.Label.CardEntry.Name, GetInnerText(XPath.Label.CardEntry.Name), $"the '{Constants.Label.CardEntry.Name}' label is displayed on the page");
            Reporting.AreEqual(Constants.Label.CardEntry.CardNumber, GetInnerText(XPath.Label.CardEntry.CardNumber), $"the '{Constants.Label.CardEntry.CardNumber}' label is displayed on the page");
            Reporting.AreEqual(Constants.Label.CardEntry.ExpiryDate, GetInnerText(XPath.Label.CardEntry.ExpiryDate), $"the '{Constants.Label.CardEntry.ExpiryDate}' label is displayed on the page");
            Reporting.AreEqual(Constants.Label.CardEntry.CVC, GetInnerText(XPath.Label.CardEntry.CVC), $"the '{Constants.Label.CardEntry.CVC}' label is displayed on the page");
            _driver.SwitchTo().ParentFrame();
        }


        /// <summary>
        /// Verify the current email address displayed and updating the email address
        /// </summary>
        public void VerifyCurrentEmailAddressAndUpdate(EndorsementBase testData)
        {
            var contactDetails = DataHelper.GetContactDetailsViaExternalContactNumber(testData.OriginalPolicyData.Policyholder.ContactExternalNumber);
            ScrollElementIntoView(XPath.EmailEntry.Email);
            Reporting.Log("Current Email", _browser.Driver.TakeSnapshot());
            var expectedEmail = string.IsNullOrEmpty(contactDetails.GetEmail()) ? string.Empty : contactDetails.GetEmail();
            Reporting.AreEqual(expectedEmail, GetElementValue(XPath.EmailEntry.Email), ignoreCase: true, $" the current email address is matching with Shield.");

            WaitForTextFieldAndEnterText(XPath.EmailEntry.Email, testData.ActivePolicyHolder.PrivateEmail.Address, false);
            Reporting.Log("Updated Email", _browser.Driver.TakeSnapshot());
        }

        /// <summary>
        /// Verify the policy card displayed for all 7 products
        /// </summary>
        public void VerifyPolicyCard(EndorsementBase testData)
        {
            using (var spinner = new SparkSpinner(_browser))
            {
                Reporting.Log("Check current payment details are displayed", _browser.Driver.TakeSnapshot());

                if (testData.GetType() == typeof(EndorseCar))
                {
                    VerifyMotorCarPolicyCard((EndorseCar)testData);
                }
                else if (testData.GetType() == typeof(EndorseHome))
                {
                    VerifyHomePolicyCard((EndorseHome)testData);
                }
                else if (testData.GetType() == typeof(EndorseCaravan))
                {
                    VerifyCaravanPolicyCard((EndorseCaravan)testData);
                }
                else if (testData.GetType() == typeof(EndorseMotorCycle))
                {
                    VerifyMotorCyclePolicyCard((EndorseMotorCycle)testData);                    
                }
                else if (testData.GetType() == typeof(EndorseBoat)) { 
                    VerifyBoatPolicyCard((EndorseBoat)testData);
                }
                else if (testData.GetType() == typeof(EndorsePet))
                {
                    VerifyPetPolicyCard((EndorsePet)testData);
                }
                else if (testData.GetType() == typeof(EndorseElectricMobility))
                {
                    VerifyElectricMobilityPolicyCard((EndorseElectricMobility)testData);
                }
                else
                {
                    throw new NotSupportedException($"{testData.GetType().Name} not supported for MAP");
                }
            }
        }

        /// <summary>
        /// Verify the Amount Due displayed matches the expected amount.
        /// If the Amount Due is a whole number, then ".00" will not be 
        /// displayed here so we adjust the expected value accordingly.
        /// </summary>
        public void VerifyAmountDue(EndorsementBase testData)
        {
            var amountDue = GetAmountDue(testData);
            Reporting.AreEqual(amountDue.ToString().Replace(".00", "").Trim(), DataHelper.StripMoneyNotations(GetInnerText(XPath.Label.AmountDue)), 
                $"the Amount due '{amountDue}' is displayed (without decimal point if a whole number)");
        }

        /// <summary>
        /// Return Amount Due
        /// Display the amount due from the Shield Policy API NextPayableInstalment for Pending Installment Status.
        /// For Instalments in Submitted status this will be null so get the value from OutstandingPayableInstallment.Amount.Total.
        /// 
        /// If the amount has zero cents then MAP displays only whole dollars, so remove any zero cents notations.
        /// </summary>
        public static string GetAmountDue(EndorsementBase testData)
        {
            var amountDue = testData.OriginalPolicyData.NextPayableInstallment is null ?
                           testData.OriginalPolicyData.AnnualPremium.Total.ToString() :
                           testData.OriginalPolicyData.NextPayableInstallment.OutstandingAmount.ToString();

            // If zero cents, then MAP removes the cents notation altogether on screen.
            amountDue = Double.Parse(amountDue).ToString("0.00").Replace(".00", string.Empty);
            return amountDue;
        }

        /// <summary>
        /// Enter the credit card details passed from test data
        /// </summary>
        ///<param name = "creditCardDetails" > Credit card holder name, number, expiry date and card verification number for input</param>
        public void FillCreditCard(CreditCard creditCardDetails)
        {
            FillCreditCardDetailsIn(creditCardDetails, isCvnRequired: true);
        }

        /// <summary>
        /// Verify the credit card authentication text, authorisation checkbox text,  click authorisation checkbox 
        /// and Submit payment page
        /// </summary>
        public void VerifyAuthTextAndSubmitPayment()
        {
            Reporting.Log($"GetInnerText(XP_AUTHORISATION_TEXT).StripLineFeedAndCarriageReturns() = {GetInnerText(XPathPayment.Detail.AuthorisationText).StripLineFeedAndCarriageReturns()}");
            Match matchTerms = new Regex(FixedTextRegex.ANNUAL_POLICY_ENDORSEMENT_CARD_PAYMENT_AUTHORISATION_REGEX).Match(PaymentAuthorisationText);
            Reporting.IsTrue(matchTerms.Success, $"authorisation terms for credit card payment are present. Actual Result: {PaymentAuthorisationText}");

            Reporting.Log($"GetInnerText(XP_AUTHORISATION_ACKNOWLEDGEMENT) = {GetInnerText(XPathPayment.Detail.AuthorisationAcknowledgement)}");
            Match matchAuthorisation = new Regex(FixedTextRegex.CARD_PAYMENT_AUTHORISATION_TERMS_AGREE_REGEX).Match(PaymentAcknowledgementtext);
            Reporting.IsTrue(matchAuthorisation.Success, $"acknowledgement text for checkbox is present for credit card payment. Actual Result: {PaymentAcknowledgementtext}");

            ClickReadAgreeAuthorisationTerms();
            ClickSubmitButton();
        }

        /// <summary>
        /// Verify the policy card displayed for Motor Car
        /// </summary>
        private void VerifyMotorCarPolicyCard(EndorseCar testData)
        {
            string coverType = null;

            switch (testData.OriginalPolicyData.Covers.First().CoverTypeDescription)
            {
                case "Full Cover":
                    coverType = Constants.Label.PolicyCard.CarCoverTypeSubMFCO;
                    break;
                case "TFT":
                    coverType = Constants.Label.PolicyCard.CarCoverTypeSubTFT;
                    break;
                case "TPO":
                    coverType = Constants.Label.PolicyCard.CarCoverTypeSubTPO;
                    break;
                default:
                    throw new NotSupportedException($"The cover type is not supported or invalid");                
            }

            Reporting.AreEqual(Constants.Label.PolicyCard.CarTitle, GetInnerText(XPath.Label.PolicyCard.Title), $"the '{Constants.Label.PolicyCard.CarTitle}' label is displayed on the Policy Card");
            Reporting.AreEqual(coverType, GetInnerText(XPath.Label.PolicyCard.CoverType), $"the '{coverType}' label is displayed on the Policy Card");
            Reporting.IsTrue(GetInnerText(XPath.Label.PolicyCard.Model).Contains($"{testData.InsuredAsset.Make} {testData.InsuredAsset.Model}"), $"Policy card car model - " +
                $"Actual Result: {GetInnerText(XPath.Label.PolicyCard.Model).Replace("Model.","").Trim()} " +
                $"Expected Result:{testData.InsuredAsset.Make} {testData.InsuredAsset.Model}");
            Reporting.AreEqual($"Registration: {testData.InsuredAsset.Registration}", GetInnerText(XPath.Label.PolicyCard.Registration), "Policy card car registration number");
            Reporting.AreEqual($"Policy number: {testData.PolicyNumber}", GetInnerText(XPath.Label.PolicyCard.PolicyNumber), "Policy card policy number");
        }

        /// <summary>
        /// Verify the policy card displayed for Home
        /// </summary>
        private void VerifyHomePolicyCard(EndorseHome testData)
        {
            // Make the following Cover Type deriving part of code into centralised location for resuability after series of testing
            string coverTypeName = null;

            if (testData.OriginalPolicyData.Covers.Count == 1)
            {
                if (testData.OriginalPolicyData.Covers[0].CoverTypeDescription.Contains("Landlord"))
                {
                    var splitCoverType = testData.OriginalPolicyData.Covers[0].CoverTypeDescription.Split(new char[0]);
                    coverTypeName = $"{splitCoverType[0]}'s {splitCoverType[1]}";
                }
                else
                {
                    coverTypeName = testData.OriginalPolicyData.Covers[0].CoverTypeDescription.Split(new char[0]).Last();
                }
            }
            else
            {

                if (testData.OriginalPolicyData.Covers[0].CoverTypeDescription.Equals("Homeowner Building") && testData.OriginalPolicyData.Covers[1].CoverTypeDescription.Equals("Homeowners Contents"))
                {
                    coverTypeName = "Building & contents";
                }
                else if (testData.OriginalPolicyData.Covers[0].CoverTypeDescription.Equals("Landlord's Building") && testData.OriginalPolicyData.Covers[1].CoverTypeDescription.Equals("Landlord's Contents"))
                {
                    coverTypeName = "Landlord's building & contents";
                }
                else if (testData.OriginalPolicyData.Covers[0].CoverTypeDescription.Equals("Homeowners Contents") && 
                    (testData.OriginalPolicyData.Covers[1].CoverTypeDescription.Equals("Valuables Specified") || testData.OriginalPolicyData.Covers[1].CoverTypeDescription.Equals("Valuables Unspecified")))
                {
                    coverTypeName = "Contents";
                }
            }
            Reporting.AreEqual(Constants.Label.PolicyCard.HomeTitle, GetInnerText(XPath.Label.PolicyCard.Title), $"the '{Constants.Label.PolicyCard.HomeTitle}' label is displayed on the Policy Card");
            Reporting.AreEqual(coverTypeName, GetInnerText(XPath.Label.PolicyCard.CoverType), ignoreCase: true, $"the '{testData.OriginalPolicyData.Covers[0].CoverTypeDescription}' is displayed on the Policy Card");
            Reporting.IsTrue(GetInnerText(XPath.Label.PolicyCard.Address).Contains($"{testData.OriginalPolicyData.HomeAsset.Address.StreetNumber} {testData.OriginalPolicyData.HomeAsset.Address.StreetOrPOBox}"),
                                                                                    "policy card includes the street name and number");
            Reporting.AreEqual($"Policy number: {testData.PolicyNumber}", GetInnerText(XPath.Label.PolicyCard.PolicyNumber), "Policy card policy number");      
        }

        /// <summary>
        /// Verify the policy card displayed for Caravan
        /// </summary>
        private void VerifyCaravanPolicyCard(EndorseCaravan testData)
        {
            var caravan = DataHelper.GetVehicleDetails(testData.OriginalPolicyData.CaravanAsset.VehicleId).Vehicles[0];

            Reporting.AreEqual(Constants.Label.PolicyCard.CaravanTitle, GetInnerText(XPath.Label.PolicyCard.Title), $"the '{Constants.Label.PolicyCard.CaravanTitle}' label is displayed on the Policy Card");

            Reporting.IsTrue(GetInnerText(XPath.Label.PolicyCard.Model).Contains($"{caravan.MakeDescription} {caravan.ModelDescription}"), $"information about the caravan make and model is present" +
                $"Expected Result :{caravan.MakeDescription} {caravan.ModelDescription} " +
                $"Actual Result:{GetInnerText(XPath.Label.PolicyCard.Model).Replace("Model:","").Trim()} ");

            if (DataHelper.IsRegistrationNumberConsideredValid(testData.OriginalPolicyData.CaravanAsset.RegistrationNumber))
            {
                Reporting.AreEqual($"Registration: {testData.OriginalPolicyData.CaravanAsset.RegistrationNumber}", GetInnerText(XPath.Label.PolicyCard.Registration), "caravan/trailer registration is displayed");
            }
            Reporting.AreEqual($"Policy number: {testData.PolicyNumber}", GetInnerText(XPath.Label.PolicyCard.PolicyNumber), "Policy card policy number");
        }

        /// <summary>
        /// Verify the policy card displayed for Motor Cycle
        /// </summary>
        private void VerifyMotorCyclePolicyCard(EndorseMotorCycle testData)
        {
            string coverType = null;
            var motorCycle = DataHelper.GetVehicleDetails(testData.OriginalPolicyData.MotorcycleAsset.VehicleId).Vehicles[0];

            switch (testData.OriginalPolicyData.Covers.First().CoverTypeDescription)
            {
                case "Full Cover":
                    coverType = Constants.Label.PolicyCard.CarCoverTypeSubMFCO;
                    break;
                case "TPF&T":
                    coverType = Constants.Label.PolicyCard.CarCoverTypeSubTFT;
                    break;
                case "TPPD":
                    coverType = Constants.Label.PolicyCard.MotorCycleCoverTypeSubTPPD;
                    break;
                default:
                    throw new NotSupportedException($"The cover type is not supported or invalid");
            }

            Reporting.AreEqual(Constants.Label.PolicyCard.MotorcycleTitle, GetInnerText(XPath.Label.PolicyCard.Title), $"the '{Constants.Label.PolicyCard.MotorcycleTitle}' label is displayed on the Policy Card");
            Reporting.AreEqual(coverType, GetInnerText(XPath.Label.PolicyCard.CoverType), $"the 'Comprehensive' cover type label is displayed on the Policy Card");
            Reporting.IsTrue(GetInnerText(XPath.Label.PolicyCard.Model).Contains($"{motorCycle.MakeDescription} {motorCycle.ModelDescription}"), "information about the Motorcycle make and model is present");

            if (DataHelper.IsRegistrationNumberConsideredValid(testData.OriginalPolicyData.MotorcycleAsset.RegistrationNumber))
            {
                Reporting.AreEqual($"Registration: {testData.OriginalPolicyData.MotorcycleAsset.RegistrationNumber}", GetInnerText(XPath.Label.PolicyCard.Registration), "motorcycle registration is displayed");
            }
            Reporting.AreEqual($"Policy number: {testData.PolicyNumber}", GetInnerText(XPath.Label.PolicyCard.PolicyNumber), "Policy card policy number");
        }

        /// <summary>
        /// Verify the policy card displayed for Boat
        /// </summary>
        private void VerifyBoatPolicyCard(EndorseBoat testData)
        {
            string boatType = null;

            if (testData.OriginalPolicyData.BoatAsset.BoatType.Equals("P"))
            {
                boatType = "Power Boat";
            }
            else if (testData.OriginalPolicyData.BoatAsset.BoatType.Equals("L"))
            {
                boatType = "Sail Boat";
            }
            else
            {
                throw new NotSupportedException($"The boat type '{testData.OriginalPolicyData.BoatAsset.BoatType}' is not supported or invalid");
            }

            Reporting.AreEqual(Constants.Label.PolicyCard.BoatTitle, GetInnerText(XPath.Label.PolicyCard.Title), $"the '{Constants.Label.PolicyCard.BoatTitle}' label is displayed on the Policy Card");
            Reporting.AreEqual($"Type: {boatType}", GetInnerText(XPath.Label.PolicyCard.Type), "information about the boat type is present");
            Reporting.AreEqual($"Policy number: {testData.PolicyNumber}", GetInnerText(XPath.Label.PolicyCard.PolicyNumber), "Policy card policy number");
        }

        /// <summary>
        /// Verify the policy card displayed for Pet
        /// </summary>
        private void VerifyPetPolicyCard(EndorsePet testData)
        {
            Reporting.AreEqual(Constants.Label.PolicyCard.PetTitle, GetInnerText(XPath.Label.PolicyCard.Title), $"the '{Constants.Label.PolicyCard.BoatTitle}' label is displayed on the Policy Card");
            Reporting.AreEqual(testData.OriginalPolicyData.PetAsset.PetType, GetInnerText(XPath.Label.PolicyCard.CoverType), "information about the boat type is present");
            if (testData.OriginalPolicyData.PetAsset.PetName != null)
            {
                Reporting.AreEqual($"Name: {testData.OriginalPolicyData.PetAsset.PetName}", GetInnerText(XPath.Label.PolicyCard.PetName), "pet name is present");
            }
            Reporting.AreEqual($"Policy number: {testData.PolicyNumber}", GetInnerText(XPath.Label.PolicyCard.PolicyNumber), "Policy card policy number");
        }

        /// <summary>
        /// Verify the policy card displayed for Electric Mobility
        /// </summary>
        private void VerifyElectricMobilityPolicyCard(EndorseElectricMobility testData)
        {
            Reporting.AreEqual(Constants.Label.PolicyCard.ElectricMobilityTitle, GetInnerText(XPath.Label.PolicyCard.Title), 
                $"the '{Constants.Label.PolicyCard.ElectricMobilityTitle}' label is displayed on the Policy Card");
            Reporting.AreEqual($"Policy number: {testData.PolicyNumber}", GetInnerText(XPath.Label.PolicyCard.PolicyNumber), "Policy card policy number");
        }

        /// <summary>
        /// Handle Failed payment scenario pop
        /// Click on the Try again button twice and navigate to the confirmation page for failed payment
        /// </summary>
        public void VerifyFailedPaymentPopUpAndTryAgain()
        {
            int popUpOccurance = 1;

            using (var spinner = new SparkSpinner(_browser))
            {
                do
                {
                    spinner.WaitForSpinnerToFinish();
                    Reporting.Log($"Waiting for the 'Try Again' button for the {popUpOccurance} st/nd time", _browser.Driver.TakeSnapshot());
                    Reporting.AreEqual(Constants.Label.FailedPayment.Title, GetInnerText(XPath.Label.FailedPayment.Title), $"the '{Constants.Label.FailedPayment.Title}' label is displayed on the Failed Payment PopUp");
                    Reporting.IsTrue(IsControlDisplayed(XPath.Button.BackToMyPolicy), "the 'Back to Policy' button is displayed on the Failed Payment PopUp");
                    Reporting.IsTrue(IsControlDisplayed(XPath.Button.TryAgain), "the 'Try' button is displayed on the Failed Payment PopUp");

                    if (IsControlEnabled(XPath.Button.TryAgain))
                    { 
                        ClickControl(XPath.Button.TryAgain); 
                    }
                    else
                    { 
                        throw new NoSuchElementException("Button is currently disabled and not clickable. Check input values."); 
                    }
                    ClickSubmitButton();
                    spinner.WaitForSpinnerToFinish();
                    popUpOccurance++;
                } while (popUpOccurance<=2);               
            }
        }
    }
}
