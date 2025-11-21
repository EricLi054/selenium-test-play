using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using static Rac.TestAutomation.Common.Constants;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace UIDriver.Pages.Spark.Endorsements
{
    public class ReviewYourPolicy : SparkPaymentPage
    {
        #region CONSTANTS
        private class Constants
        {
            public const string PageHeading = "Review your policy";
            public const string MonthlyInstalmentAmountSubtitle = "Monthly amounts may vary. Check your policy documents for the exact amounts.";
            public const int QualifiesForNoClaimBonusProtection = 50;

            public class CarCard
            {
                public const string Title = "Car insurance";
            }

            public class YourPolicySummary
            {
                public const string Title = "Your policy summary";
                public const string SubTitle = "Please ensure all details are correct and that the policy suits your needs.";
                public const string InfoSummaryShow = "Show your policy information";
                public const string YourCar = "Your car";
                public const string CarDetails = "Car details";
                public const string CarUsage = "Car usage";
                public const string StartDate = "Start date for this change";
                public const string Premium = "Premium";
                public const string InfoSummaryHide = "Hide your policy information";
            }

            public class Email
            {
                public const string Title = "Confirm your email address";
                public const string Text = "We'll update your contact details (not your login email) if you provide a new email.";
                public const string Validation = "Please enter a valid email";
            }

            public class RefundTitle
            {
                public const string KnownCardRefund = "We'll pay your refund into your card:";
                public const string UnknownCardRefund = "We'll pay your refund into the credit card you made payment from";
                public const string RefundPdsText = "We'll process your refund once the policy change takes place. See the Product Disclosure Statement for more information.";
                public const string BankAccountRefund = "We'll pay your refund into your account:";
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public class Title
            {
                public const string PageHeading = "//h2[text()='" + Constants.PageHeading + "']";
                public const string CarCard = "id('policy-card-content-policy-details-header-title-policy-details')";
            }

            public class Label
            {
                public const string Email = "//label[@for='email']";
                public const string CarCardPolicyCover     = "id('policy-card-content-policy-details-header-subtitle-policy-details')";
                public const string CarCardModel           = "id('policy-card-content-policy-details-property-0-model-policy-details')";
                public const string CarCardCarRegistration = "id('policy-card-content-policy-details-property-1-registration-policy-details')";
                public const string CarCardPolicyNumber    = "id('policy-card-content-policy-details-property-2-policy-number-policy-details')";
                public const string PolicyStateDate = "id('start-date-value')";
                public const string RefundAmount   = "id('refund-amount-value')";
                public const string Amount = "id('payment-amount-label')";
                public const string IncreaseAmount = "id('payment-amount-value')";
                public const string MonthlyInstallmentAmount = "id('instalment-amount-value')";
            }

            public class InputField
            {
                public const string Email = "id('email')";
            }

            public class Button
            {
                public const string Confirm = "id('mid-term-review-confirm-button')";
                public const string Back = "id('back-link')";
            }

            public class RefundTitle
            {
                public const string SingleAccountRefund = "id('single-account-refund-title')";
                public const string CreditCardRefund = "id('credit-card-refund-title')";
                public const string SingleAccountRefundPdsText = "id('single-account-refund-pds-text')";
                public const string CreditCardRefundPdsText = "id('credit-card-refund-pds-text')";
                public const string AddAccountRefundPdsText = "id('add-account-pds-text')";
                public const string MonthlyInstalmentAmountSubtitle = "id('instalment-amount-subtitle')";

            }

            public class YourPolicySummary
            {          
                public class Heading
                {
                    public const string Main = "id('info-summary-title')";
                    public const string Sub = "id('info-summary-subtitle')";
                    public const string YourCar = "id('info-summary-your-car-section-title')";
                    public const string CarDetails = "id('info-summary-car-details-title')";
                    public const string CarUsage = "id('info-summary-car-usage-title')";
                    public const string StartDate = "id('info-summary-start-date-title')";
                    public const string Premium = "id('info-summary-premium-title')";
                }

                public class ButtonLinks
                {
                    public const string InfoSummaryShow = "id('info-summary-expand')";
                    public const string InfoSummaryHide = "id('info-summary-collapse')";
                }

                public class Text
                {
                    public class CarDetails
                    {
                        public const string Registration = "id('info-summary-car-details-car-details-subsection-0')";
                        public const string Modification = "id('info-summary-car-details-car-details-subsection-1')";
                        public const string Finance = "id('info-summary-car-details-car-details-subsection-2')";
                        public const string Financier = "id('info-summary-car-details-car-details-subsection-3')";
                    }
                    public class CarUsage
                    {

                        public const string Section1 = "id('info-summary-car-usage-car-usage-subsection-0')";
                        public const string Section2 = "id('info-summary-car-usage-car-usage-subsection-1')";
                        public const string Section3 = "id('info-summary-car-usage-car-usage-subsection-2')";
                        public const string Section4 = "id('info-summary-car-usage-car-usage-subsection-3')";
                    }

                    public class Premium
                    {

                        public const string Excess = "id('info-summary-premium-premium-subsection-0')";
                        public const string AgreedValue = "id('info-summary-premium-premium-subsection-1')";
                        public const string HireCar = "id('info-summary-premium-premium-subsection-2')";
                        public const string NCB = "id('info-summary-premium-premium-subsection-3')";
                    }
                }             
            }

            public const string AddPaymentMethod = "id('select-payment-method-add-payment-method')";
            public const string AddBankAccount = "//button[starts-with(@id, 'add-payment-method-input-payment-method-Bank')]";
            public const string AddCreditCard = "id('add-payment-method-input-payment-method-Card')";

        }
        #endregion

        public ReviewYourPolicy(Browser browser) : base(browser) { }

        #region Settable properties and controls
        private string PolicyCardTitle => GetInnerText(XPath.Title.CarCard);
        private string PolicyCardCover => GetInnerText(XPath.Label.CarCardPolicyCover);
        private string PolicyCardModel => GetInnerText(XPath.Label.CarCardModel);
        private string PolicyCardRego => GetInnerText(XPath.Label.CarCardCarRegistration);
        private string PolicyNumber => GetInnerText(XPath.Label.CarCardPolicyNumber);
        private string PolicyStartDate => GetInnerText(XPath.Label.PolicyStateDate);
        private string PremiumChangeForRefund   => GetInnerText(XPath.Label.RefundAmount);
        private string PremiumChangeForIncrease => GetInnerText(XPath.Label.IncreaseAmount);
        private string MonthlyInstallmentAmount => GetInnerText(XPath.Label.MonthlyInstallmentAmount);

        private string IncreaseAmount => GetInnerText(XPath.Label.IncreaseAmount);

        private string EmailAddress
        {
            get => GetValue(XPath.InputField.Email);
            set => WaitForTextFieldAndEnterText(XPath.InputField.Email, value, false);
        }
        #endregion
        public override bool IsDisplayed()
        {
            try
            {              
                GetElement(XPath.Title.CarCard);
                GetElement(XPath.YourPolicySummary.ButtonLinks.InfoSummaryShow);
                GetElement(XPath.Label.Email);
                GetElement(XPath.InputField.Email);
                GetElement(XPath.Button.Back);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Review your Policy");
            Reporting.Log("Review your Policy", _browser.Driver.TakeSnapshot());

            return true;
        }

        /// <summary>
        /// Select the "Next" button to navigate to the next page in the flow.
        /// If "Yes, it is" has been selected the user will be navigated to the "Your car details" step.
        /// If "No, it isn't" has been selected then the user will be diverted to the "Lets update your car" page instead.
        /// </summary>
        public void ClickConfirm() => ClickControl(XPath.Button.Confirm);

        /// <summary>
        /// Verify start date notification card displayed
        /// </summary>
        public void VerifyPolicyCard(EndorseCar endorseCar)
        {

            string policyCover = null;
            Car insuredCar = new Car();
            insuredCar = endorseCar.ChangeMakeAndModel ? endorseCar.NewInsuredAsset : endorseCar.InsuredAsset;

            switch (endorseCar.CoverType)
            {
                case MotorCovers.MFCO:
                    policyCover = "Comprehensive";
                    break;
                case MotorCovers.TFT:
                    policyCover = "Third party fire & theft";
                    break;
                case MotorCovers.TPO:                
                default:
                    policyCover = "Third party";
                    break;

            }
            Reporting.AreEqual(Constants.CarCard.Title, PolicyCardTitle, "Policy card title");
            Reporting.AreEqual(policyCover, PolicyCardCover, "Policy card cover type");
            Reporting.Log($"Expecting the following to be contained in the Model field: '{insuredCar.Make} {insuredCar.Model}'");
            Reporting.IsTrue(PolicyCardModel.Contains($"{insuredCar.Make} {insuredCar.Model}", StringComparison.InvariantCultureIgnoreCase), $"Policy card '{PolicyCardModel}' contained correct car make and model '{insuredCar.Make} {insuredCar.Model}'");

            if (!string.IsNullOrEmpty(endorseCar.NewInsuredAsset.Registration) && !endorseCar.ChangeMakeAndModel)
            {
                Reporting.AreEqual($"Registration: {endorseCar.NewInsuredAsset.Registration}", PolicyCardRego, "Policy card car registration number");
            }
            else
            {
                Reporting.AreEqual($"Registration: {insuredCar.Registration}", PolicyCardRego, "Policy card car registration number");
            }

            Reporting.AreEqual($"Policy number: {endorseCar.PolicyNumber}", PolicyNumber, "Policy card policy number");
        }


        public void VerifyPremiumChange(EndorseCar endorseCar)
        {
            switch (endorseCar.ExpectedImpactOnPremium)
            {
                case PremiumChange.PremiumDecrease:
                    if (endorseCar.PayMethod.IsMonthly)
                    {
                        Reporting.AreEqual($"${DataHelper.GetCorrectedPremium(endorseCar.PremiumChangesAfterEndorsement.TotalPremiumMonthly)}", MonthlyInstallmentAmount, 
                            "expected monthly premium amount against value displayed on page");
                        Reporting.AreEqual(Constants.MonthlyInstalmentAmountSubtitle, GetInnerText(XPath.RefundTitle.MonthlyInstalmentAmountSubtitle), 
                            "expected copy for monthly installment subtitle against the value displayed on page");
                    }
                    else
                    {
                        Reporting.AreEqual($"${DataHelper.GetCorrectedPremium(endorseCar.PremiumChangesAfterEndorsement.Total)}", PremiumChangeForRefund, 
                            "expected refund amount difference against value displayed on page");
                    }
                    break;
                case PremiumChange.PremiumIncrease:
                    if (endorseCar.PayMethod.IsMonthly)
                    {
                        Reporting.AreEqual($"${DataHelper.GetCorrectedPremium(endorseCar.PremiumChangesAfterEndorsement.TotalPremiumMonthly)}", MonthlyInstallmentAmount, 
                            "expected monthly premium amount against value displayed on page");
                        Reporting.AreEqual(Constants.MonthlyInstalmentAmountSubtitle, GetInnerText(XPath.RefundTitle.MonthlyInstalmentAmountSubtitle), 
                            "expected monthly installment subtitle is displayed on page");
                    }
                    else
                    {
                        Reporting.AreEqual($"${DataHelper.GetCorrectedPremium(endorseCar.PremiumChangesAfterEndorsement.Total)}", PremiumChangeForIncrease, 
                            "expected increase premium amount against value displayed on page");
                    }
                    break;
                default:
                    // this deals with no premium change
                    if (endorseCar.PayMethod.IsMonthly)
                    {
                        Reporting.AreEqual($"${DataHelper.GetCorrectedPremium(endorseCar.PremiumChangesAfterEndorsement.TotalPremiumMonthly)}", MonthlyInstallmentAmount,
                            "expected monthly premium amount against value displayed on page");
                        Reporting.AreEqual(Constants.MonthlyInstalmentAmountSubtitle, GetInnerText(XPath.RefundTitle.MonthlyInstalmentAmountSubtitle),
                            "expected monthly installment subtitle is displayed on page");
                    }
                    else
                    {
                        Reporting.IsFalse(IsControlDisplayed(XPath.Label.Amount), "'Amount' label is not displayed for 'no change in premium'");
                        Reporting.IsFalse(IsControlDisplayed(XPath.Label.MonthlyInstallmentAmount), "'Amount value' is not displayed for 'no change in premium'");
                    }

                    break;
            }
        }

        public void VerifyYourPolicySummary(EndorseCar endorseCar, bool isRenewal)
        {
            Car insuredCar = new Car();
            insuredCar = endorseCar.ChangeMakeAndModel ? endorseCar.NewInsuredAsset : endorseCar.InsuredAsset;

            // Title
            Reporting.AreEqual(Constants.YourPolicySummary.Title,GetInnerText(XPath.YourPolicySummary.Heading.Main), " Title of the Your policy summary");
            Reporting.AreEqual(Constants.YourPolicySummary.SubTitle, GetInnerText(XPath.YourPolicySummary.Heading.Sub), " sub title of the Your policy summary");

            ClickControl(XPath.YourPolicySummary.ButtonLinks.InfoSummaryShow);
            Reporting.Log("Policy Summary", _browser.Driver.TakeSnapshot());

            // Your Car
            Reporting.AreEqual(Constants.YourPolicySummary.YourCar, GetInnerText(XPath.YourPolicySummary.Heading.YourCar), " 'Your Car' sub title of the 'Your policy summary'");
            Reporting.Log($"Expecting the following to be contained in the Model field: '{insuredCar.Make} {insuredCar.Model}'");
            Reporting.IsTrue(PolicyCardModel.Contains($"{insuredCar.Make} {insuredCar.Model}"), "make and model of the 'Your policy summary'");

            //Car details
            Reporting.AreEqual(Constants.YourPolicySummary.CarDetails, GetInnerText(XPath.YourPolicySummary.Heading.CarDetails), " 'Car details' sub title of the 'Your policy summary'");
            if (!string.IsNullOrEmpty(endorseCar.NewInsuredAsset.Registration) && !endorseCar.ChangeMakeAndModel)
            {
                Reporting.AreEqual($"Registration: {endorseCar.NewInsuredAsset.Registration}", GetInnerText(XPath.YourPolicySummary.Text.CarDetails.Registration), "'Registration' details on 'Car Details' section of  'Your policy summary'");
            }
            else
            {
                Reporting.AreEqual($"Registration: {insuredCar.Registration}", GetInnerText(XPath.YourPolicySummary.Text.CarDetails.Registration), "'Registration' details on 'Car Details' section of  'Your policy summary'");
            }

            var IsModified = endorseCar.ChangeMakeAndModel ? endorseCar.NewInsuredAsset.IsModified : endorseCar.OriginalPolicyData.MotorAsset.IsVehicleModified();
            var modified = IsModified ? "Yes" : "No";
            Reporting.AreEqual($"Modification: {modified}", GetInnerText(XPath.YourPolicySummary.Text.CarDetails.Modification), " 'Modification' details on 'Car Details' section of  'Your policy summary'");

            var financed = insuredCar.IsFinanced ? "Yes" : "No";
            Reporting.AreEqual($"Finance: {financed}", GetInnerText(XPath.YourPolicySummary.Text.CarDetails.Finance), " 'Finance' details on 'Car Details' section of  'Your policy summary'");
            if (insuredCar.IsFinanced)
            {
                Reporting.AreEqual($"Financier: {insuredCar.Financier}", GetInnerText(XPath.YourPolicySummary.Text.CarDetails.Financier),ignoreCase:true, " 'Financier' details on 'Car Details' section of  'Your policy summary'");

            }

            //Car Usage
            Reporting.AreEqual(Constants.YourPolicySummary.CarUsage, GetInnerText(XPath.YourPolicySummary.Heading.CarUsage), " 'Car Usage' sub title of the 'Your policy summary'");

            var vehicleUsage = endorseCar.UsageType == VehicleUsage.Undefined ? endorseCar.OriginalPolicyData.MotorAsset.VehicleUsage: VehicleUsageNameMappings[endorseCar.UsageType].TextSpark;
            Reporting.AreEqual($"Use: {vehicleUsage}", GetInnerText(XPath.YourPolicySummary.Text.CarUsage.Section1), " 'Use' details on 'Car Usage' section of  'Your policy summary'");

            // Car address:
            var xpathActualAddress = vehicleUsage.Equals(VehicleUsageNameMappings[VehicleUsage.Private].TextSpark) ?
                                     XPath.YourPolicySummary.Text.CarUsage.Section2 :
                                     XPath.YourPolicySummary.Text.CarUsage.Section3;
            Reporting.Log($"Vehicle address displayed (not including field label) is: {GetInnerText(xpathActualAddress).Substring(13)}");
            Reporting.IsTrue(endorseCar.OriginalPolicyData.MotorAsset.Address.IsEqualToString(GetInnerText(xpathActualAddress).Substring(13)), "'Car address' details in 'Car Usage' section of 'Your policy summary' match expected value");

            // Annual Kms
            var xpathAnnualKms = vehicleUsage.Equals(VehicleUsageNameMappings[VehicleUsage.Private].TextSpark) ?
                                     XPath.YourPolicySummary.Text.CarUsage.Section3 :
                                     XPath.YourPolicySummary.Text.CarUsage.Section4;
            Reporting.AreEqual($"Kilometres travelled annually: {endorseCar.AnnualKm.GetDescription()}kms", GetInnerText(xpathAnnualKms), "'Kilometres travelled annually' details on 'Car Usage' section of  'Your policy summary'");

            //Start Date- This section is not displayed for renewals
            if (!isRenewal)
            {
                Reporting.AreEqual(Constants.YourPolicySummary.StartDate, GetInnerText(XPath.YourPolicySummary.Heading.StartDate), "'Start Date' sub title of the 'Your policy summary'");
                Reporting.AreEqual(endorseCar.StartDate.ToString(DateTimeTextFormat.ddMMyyyy), PolicyStartDate, "policy start date shown correctly");
            }

            //Premium
            Reporting.AreEqual(Constants.YourPolicySummary.Premium, GetInnerText(XPath.YourPolicySummary.Heading.Premium), " 'Premium' sub title of the 'Your policy summary'");
            Reporting.AreEqual($"Excess: ${int.Parse(DataHelper.StripMoneyNotations(endorseCar.Excess)):n0}", GetInnerText(XPath.YourPolicySummary.Text.Premium.Excess), "'Excess' details on 'Premium' section of  'Your policy summary'");

            if (!endorseCar.CoverType.Equals(MotorCovers.TPO))
            {
                if (endorseCar.ChangeMakeAndModel)
                {
                    var expectedSIValue = (int)((1 + ((decimal)endorseCar.InsuredVariance / 100)) * endorseCar.NewInsuredAsset.MarketValue);
                    Reporting.AreEqual($"Agreed value: ${expectedSIValue:n0}", GetInnerText(XPath.YourPolicySummary.Text.Premium.AgreedValue), "'Agreed Value' details on 'Premium' section of 'Your policy summary'");
                }
                else
                {
                    Reporting.AreEqual($"Agreed value: ${Math.Floor(endorseCar.OriginalPolicyData.MotorAsset.TotalInsuredValue):n0}", GetInnerText(XPath.YourPolicySummary.Text.Premium.AgreedValue), "'Agreed Value' details on 'Premium' section of 'Your policy summary'");
                }
            }

            if (endorseCar.CoverType.Equals(MotorCovers.MFCO))
            {
                var hireCar = endorseCar.OriginalPolicyData.HasHireCarCover ? "Yes" : "No";
                Reporting.AreEqual($"Hire car: {hireCar}", GetInnerText(XPath.YourPolicySummary.Text.Premium.HireCar), "'Hire Car' details on 'Car Usage' section of 'Your policy summary'");
                // TODO: SPK-6704 Remove code around No Claims Bonus details being rendered when this story is actioned as all the comprehensive policies will be on version 46 and NCB will not be relevant
                if (!endorseCar.IsMotorPolicyWithExcessChanges() && int.Parse(endorseCar.OriginalPolicyData.MotorAsset.NcbLevel) > Constants.QualifiesForNoClaimBonusProtection)
                {
                    var NCB = endorseCar.OriginalPolicyData.MotorAsset.HasNcbProtection ? "Yes" : "No";
                    Reporting.AreEqual($"No claim bonus protection: {NCB}", GetInnerText(XPath.YourPolicySummary.Text.Premium.NCB), "'No Claim bonus' details on 'Car Usage' section of 'Your policy summary'");
                }
                else
                {
                    Reporting.IsFalse(IsControlDisplayed(XPath.YourPolicySummary.Text.Premium.NCB), "'No claim bonus protection' section is not displayed.");
                }
            }
        }

        public void UpdateEmail(Contact contact)
        {
            //Generate a random email address if member don't have one previously
            if (string.IsNullOrEmpty(contact.GetEmail()))
            {
                EmailAddress = DataHelper.RandomEmail(contact.FirstName, contact.Surname, Config.Get().Email.Domain).Address;
            }
            else
            {
                EmailAddress = contact.GetEmail();
            }
        }

        /// <summary>
        /// Verify the refund destination section displayed on the Review your Policy
        /// </summary>
        /// <param name="refundDestination">Set this for the expected refund destination</param>
        public void VerifyPaymentMethodAndMakePayment(EndorseCar endorseCar, SparkCommonConstants.RefundToSource refundDestination)
        {
            // TODO- Refatoring of this function -AUNT-119

            // Scroll base of page into view for screenshot
            ScrollElementIntoView(XPath.InputField.Email);
            Reporting.Log("Payment Method", _browser.Driver.TakeSnapshot());
            switch (endorseCar.ExpectedImpactOnPremium)
            {
                case PremiumChange.PremiumDecrease:

                    if (refundDestination.Equals(SparkCommonConstants.RefundToSource.RefundToBankAccount))
                    {
                        if (endorseCar.PayMethod.Scenario.Equals(PaymentScenario.AnnualCash))
                        {
                            EnterBankDetails(endorseCar.SparkExpandedPayment.BankAccountDetails);
                            VerifyBSBDetails(endorseCar.SparkExpandedPayment.BankAccountDetails);
                            Reporting.AreEqual(Constants.RefundTitle.RefundPdsText, GetInnerText(XPath.RefundTitle.AddAccountRefundPdsText), "refund pds text against the actual text displayed");
                        }

                        if (endorseCar.PayMethod.Scenario.Equals(PaymentScenario.AnnualBank))
                        {
                            Reporting.AreEqual(Constants.RefundTitle.BankAccountRefund, GetInnerText(XPath.RefundTitle.SingleAccountRefund), "refund text against the actual text displayed");
                            Reporting.AreEqual(Constants.RefundTitle.RefundPdsText, GetInnerText(XPath.RefundTitle.SingleAccountRefundPdsText), "refund pds text against the actual text displayed");
                        }
                    }
                    if (refundDestination.Equals(SparkCommonConstants.RefundToSource.RefundToKnownCreditCard))
                    {
                        if (endorseCar.PayMethod.IsMonthly)
                            {
                            ClickControl(XPath.AddPaymentMethod);
                            ClickControl(XPath.AddCreditCard);
                            FillCreditCardDetailsIn(endorseCar.SparkExpandedPayment.CreditCardDetails, isCvnRequired: false);
                        }
                        else
                        {
                            Reporting.AreEqual(Constants.RefundTitle.KnownCardRefund, GetInnerText(XPath.RefundTitle.SingleAccountRefund), "refund text against the actual text displayed");
                            Reporting.AreEqual(Constants.RefundTitle.RefundPdsText, GetInnerText(XPath.RefundTitle.SingleAccountRefundPdsText), "refund pds text against the actual text displayed");
                        }
                    }

                    if (refundDestination.Equals(SparkCommonConstants.RefundToSource.RefundToUnknownCreditCard))
                    {
                        if(endorseCar.PayMethod.IsMonthly)
                        {
                            ClickControl(XPath.AddPaymentMethod);
                            ClickControl(XPath.AddCreditCard);
                            FillCreditCardDetailsIn(endorseCar.SparkExpandedPayment.CreditCardDetails, isCvnRequired: false);
                        }
                        else 
                        { 
                        Reporting.AreEqual(Constants.RefundTitle.UnknownCardRefund, GetInnerText(XPath.RefundTitle.CreditCardRefund), "refund text against the actual text displayed");
                        Reporting.AreEqual(Constants.RefundTitle.RefundPdsText, GetInnerText(XPath.RefundTitle.CreditCardRefundPdsText), "refund pds text against the actual text displayed");
                        }
                    }
                    break;

                case PremiumChange.PremiumIncrease:

                    if (refundDestination.Equals(SparkCommonConstants.RefundToSource.RefundToKnownCreditCard) || refundDestination.Equals(SparkCommonConstants.RefundToSource.RefundToUnknownCreditCard))
                    {
                        if (endorseCar.PayMethod.IsMonthly || endorseCar.PayMethod.Scenario.Equals(PaymentScenario.AnnualBank))
                        {
                            ClickControl(XPath.AddPaymentMethod);
                            ClickControl(XPath.AddCreditCard);
                        }
                        FillCreditCardDetailsIn(endorseCar.SparkExpandedPayment.CreditCardDetails, isCvnRequired: true);                        
                    }

                    if (refundDestination.Equals(SparkCommonConstants.RefundToSource.RefundToBankAccount))
                    {
                        if(endorseCar.PayMethod.IsMonthly || endorseCar.PayMethod.Scenario.Equals(PaymentScenario.AnnualBank))
                        {
                            ClickControl(XPath.AddPaymentMethod);
                            ClickControl(XPath.AddBankAccount);
                        }
                        EnterBankDetails(endorseCar.SparkExpandedPayment.BankAccountDetails);
                        VerifyBSBDetails(endorseCar.SparkExpandedPayment.BankAccountDetails);
                    }
                    break;

                default:
                    // This deals with no change in premium
                    Reporting.IsFalse(IsControlDisplayed(XPath.RefundTitle.SingleAccountRefund), "refund text is not displayed");
                    Reporting.IsFalse(IsControlDisplayed(XPath.RefundTitle.SingleAccountRefundPdsText), "refund pds text is not displayed");
                    Reporting.IsFalse(IsControlDisplayed(XPath.RefundTitle.CreditCardRefund), "refund CC text is not dusplayed");
                    Reporting.IsFalse(IsControlDisplayed(XPath.RefundTitle.CreditCardRefundPdsText), "refund CC pds text is not displayed");
                    break;
            }

            Reporting.Log($"Capturing screen state card input", _browser.Driver.TakeSnapshot());
        }
    }
}
