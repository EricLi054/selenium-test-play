using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Linq;
using static Rac.TestAutomation.Common.Constants;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace UIDriver.Pages.Spark.Endorsements
{
    public class ReviewYourCaravanPolicy : SparkPaymentPage
    {

        #region CONSTANTS
        private static class Constants
        {
            public const string PageHeading = "Review your policy";
            public const string MonthlyInstalmentAmountSubtitle = "Monthly amounts may vary. Check your policy documents for the exact amounts.";

            public static class PolicyCard
            {
                public static readonly string Title = "Caravan and trailer insurance";
            }

            public static class YourPolicySummary
            {
                public static readonly string Title = "Your policy summary";
                public static readonly string TitleFooter = "Please ensure all details are correct and that the policy suits your needs.";

                public static readonly string StartDateTitle = "Start date for this change";

                public static readonly string TitleYourCaravanOrTrailer = "Your caravan or trailer";

                public static readonly string TitleCaravanOrTrailerDetails = "Storage and use";
                public static readonly string Rego = "Caravan or trailer registration: ";
                public static readonly string IsBusinessUse = "Your caravan or trailer is used for business purposes or earning income: No";
                public static readonly string Suburb = "Suburb where your caravan or trailer is usually kept: ";
                public static readonly string KeptPlace = "Your caravan or trailer is kept in one place and isn’t towed: ";
                public static readonly string Parked = "Place your caravan or trailer is usually parked: ";

                public static readonly string TitleYourPremium = "Your premium";
                public static readonly string BasicExcess = "Basic excess: ";
                public static readonly string AgreedValue = "Agreed value: ";
                public static readonly string ContentCover = "Contents cover: ";
            }

            public static class Email
            {
                public const string Title = "Confirm your email address";
                public const string Text = "We'll update your contact details (not your login email) if you provide a new email.";
                public const string Validation = "Please enter a valid email";
            }

            public static class RefundTitle
            {
                public const string KnownCardRefund = "We'll pay your refund into your card:";
                public const string UnknownCardRefund = "We'll pay your refund into the credit card you made payment from";
                public const string RefundPdsText = "We'll process your refund once the policy change takes place. See the Product Disclosure Statement for more information.";
                public const string BankAccountRefund = "We'll pay your refund into your account:";
            }
        }
        #endregion

        #region XPATHS
        private static class XPath
        {
            public static class Title
            {
                public const string PageHeading = "//h2[text()='" + Constants.PageHeading + "']";
                public const string CarCard = "id('policy-card-content-policy-details-header-title-policy-details')";
            }

            public static class Label
            {
                public const string Email = "//label[@for='email']";
                public const string CarCardPolicyCover = "id('policy-card-content-policy-details-header-subtitle-policy-details')";
                public const string CarCardModel = "id('policy-card-content-policy-details-property-0-model-policy-details')";
                public const string CarCardCarRegistration = "//p[contains(@id,'policy-card-content-policy-details') and contains(@id,'registration-policy-details')]";
                public const string CarCardPolicyNumber = "//p[contains(@id,'policy-card-content-policy-details') and contains(@id,'policy-number-policy-details')]";
                public const string PolicyStateDate = "id('start-date-value')";
                public const string RefundAmount = "id('refund-amount-value')";
                public const string Amount = "id('payment-amount-label')";
                public const string IncreaseAmount = "id('payment-amount-value')";
                public const string MonthlyInstallmentAmount = "id('instalment-amount-value')";
            }

            public static class PolicyCard
            {
                public static readonly string PolicyCardTitle = "id('policy-card-content-policy-details-header-title-policy-details')";
                public static readonly string PolicyCover = "id('policy-card-content-policy-details-header-subtitle-policy-details')";
                public static readonly string CarModel = "id('policy-card-content-policy-details-property-0-model-policy-details')";
                public static readonly string CarRegistration = "//p[contains(@id,'policy-card-content-policy-details') and contains(@id,'registration-policy-details')]";
                public static readonly string PolicyNumber = "//p[contains(@id,'policy-number-policy-details') and contains(@id,'policy-card-content-policy-details-property')]";
            }
            public static class InputField
            {
                public const string Email = "id('email')";
            }

            public static class Button
            {
                public const string Confirm = "id('midterm-review-confirm-button')";
                public const string Back = "id('back-link')";
            }

            public static class RefundTitle
            {
                public const string SingleAccountRefund = "id('single-account-refund-title')";
                public const string CreditCardRefund = "id('credit-card-refund-title')";
                public const string SingleAccountRefundPdsText = "id('single-account-refund-pds-text')";
                public const string CreditCardRefundPdsText = "id('credit-card-refund-pds-text')";
                public const string AddAccountRefundPdsText = "id('add-account-pds-text')";
                public const string MonthlyInstalmentAmountSubtitle = "id('instalment-amount-subtitle')";
            }

            public static class YourPolicySummary
            {
                public static readonly string ShowYourPolicyInfo = "id('info-summary-expand')";
                public static readonly string InfoTitle = "id('info-summary-title')";
                public static readonly string TitleFooter = "id('info-summary-subtitle')";

                public static readonly string StartDateTitle = "id('info-summary-start-date-section-title')";
                public static readonly string StartDate = "id('info-summary-start-date-section-start-date-subsection-0')";

                public static readonly string TitleYourCaravanOrTrailer = "id('info-summary-your-caravan-section-title')";
                public static readonly string Model = "id('info-summary-your-caravan-section-your-caravan-subsection-0')";

                public static readonly string TitleCaravanOrTrailerDetails = "id('info-summary-caravan-details-title')";
                public static readonly string Rego = "id('info-summary-caravan-details-caravan-details-subsection-0')";
                public static readonly string IsBusinessUse = "id('info-summary-caravan-details-caravan-details-subsection-1')";
                public static readonly string Suburb = "id('info-summary-caravan-details-caravan-details-subsection-2')";
                public static readonly string KeptPlace = "id('info-summary-caravan-details-caravan-details-subsection-3')";
                public static readonly string Parked = "id('info-summary-caravan-details-caravan-details-subsection-4')";

                public static readonly string TitleYourPremium = "id('info-summary-your-premium-section-title')";
                public static readonly string BasicExcess = "id('info-summary-your-premium-section-premium-subsection-0')";
                public static readonly string AgreedValue = "id('info-summary-your-premium-section-premium-subsection-1')";
                public static readonly string ContentCover = "id('info-summary-your-premium-section-premium-subsection-2')";
            }

            public const string AddPaymentMethod = "id('select-payment-method-add-payment-method')";
            public const string AddBankAccount = "//button[starts-with(@id, 'add-payment-method-input-payment-method-Bank')]";
            public const string AddCreditCard = "id('add-payment-method-input-payment-method-Card')";
        }
        #endregion

        public ReviewYourCaravanPolicy(Browser browser) : base(browser) { }

        #region Settable properties and controls
        //Policy Summary
        private string Title => GetInnerText(XPath.YourPolicySummary.InfoTitle);
        private string TitleFooter => GetInnerText(XPath.YourPolicySummary.TitleFooter);
        private string StartDateTitle => GetInnerText(XPath.YourPolicySummary.StartDateTitle);
        private string StartDate => GetInnerText(XPath.YourPolicySummary.StartDate);
        private string TitleYourCaravanOrTrailer => GetInnerText(XPath.YourPolicySummary.TitleYourCaravanOrTrailer);
        private string Model => GetInnerText(XPath.YourPolicySummary.Model);
        private string TitleCaravanOrTrailerDetails => GetInnerText(XPath.YourPolicySummary.TitleCaravanOrTrailerDetails);
        private string Rego => GetInnerText(XPath.YourPolicySummary.Rego);
        private string IsBusinessUse => GetInnerText(XPath.YourPolicySummary.IsBusinessUse);
        private string Suburb => GetInnerText(XPath.YourPolicySummary.Suburb);
        private string KeptPlace => GetInnerText(XPath.YourPolicySummary.KeptPlace);
        private string Parked => GetInnerText(XPath.YourPolicySummary.Parked);
        private string TitleYourPremium => GetInnerText(XPath.YourPolicySummary.TitleYourPremium);
        private string BasicExcess => GetInnerText(XPath.YourPolicySummary.BasicExcess);
        private string AgreedValue => GetInnerText(XPath.YourPolicySummary.AgreedValue);
        private string ContentCover => GetInnerText(XPath.YourPolicySummary.ContentCover);
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
                GetElement(XPath.PolicyCard.PolicyCardTitle);
                GetElement(XPath.YourPolicySummary.ShowYourPolicyInfo);
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

            public void VerifyPolicyCard(EndorseCaravan endorseCaravan)
            {
                Caravan insuredCaravan = new Caravan();
                insuredCaravan = endorseCaravan.ChangeMakeAndModel ? endorseCaravan.NewInsuredAsset : endorseCaravan.InsuredAsset;

                // This will update only the Rego of the Insured caravan. Usefull in a scenario where there is no
                // change to the make and model but only rego is changed
                if (!string.IsNullOrEmpty(endorseCaravan.NewInsuredAsset?.Registration) && !endorseCaravan.ChangeMakeAndModel)
                {
                    Reporting.AreEqual($"Registration: {endorseCaravan.NewInsuredAsset.Registration}", PolicyCardRego, "caravan/trailer registration is displayed");

                }
                else if(DataHelper.IsRegistrationNumberConsideredValid(insuredCaravan.Registration))
                {
                    Reporting.AreEqual($"Registration: {insuredCaravan.Registration}", PolicyCardRego, "caravan/trailer registration is displayed");
                }

                Reporting.AreEqual(Constants.PolicyCard.Title, PolicyCardTitle, "Policy card title");
                Reporting.Log($"Expecting the following to be contained in the Model field: '{insuredCaravan.Model.Trim()}'");
                Reporting.IsTrue(PolicyCardModel.Contains($"{insuredCaravan.Model.Trim()}"), "Policy card caravan make and model");
                Reporting.AreEqual($"Policy number: {endorseCaravan.OriginalPolicyData.PolicyNumber}", PolicyNumber, "Policy card policy number");
            }


            public void VerifyPremiumChange(EndorseCaravan endorseCar)
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
        public void VerifyYourPolicySummary(EndorseCaravan endorseCaravan)
        {
            Caravan insuredCaravan = new Caravan();
            insuredCaravan = endorseCaravan.ChangeMakeAndModel ? endorseCaravan.NewInsuredAsset : endorseCaravan.InsuredAsset;

            ClickControl(XPath.YourPolicySummary.ShowYourPolicyInfo);
            Reporting.Log("Your policy summary expanded", _browser.Driver.TakeSnapshot());

            Reporting.AreEqual(Constants.YourPolicySummary.StartDateTitle, StartDateTitle, "'Start date' title is displayed");
            Reporting.AreEqual(StartDate, endorseCaravan.StartDate.ToString(DateTimeTextFormat.ddMMyyyy), "start date is displayed");

            Reporting.AreEqual(Constants.YourPolicySummary.Title, Title, "'Your policy summary' title is displayed");
            Reporting.AreEqual(Constants.YourPolicySummary.TitleFooter, TitleFooter, "Your policy summary' sub title is displayed");
            Reporting.AreEqual(Constants.YourPolicySummary.TitleYourCaravanOrTrailer, TitleYourCaravanOrTrailer, $"caravan/trailer details title matches expected value");
            Reporting.AreEqual($"{insuredCaravan.Year} {insuredCaravan.Make}", Model, "caravan model displayed");

            Reporting.AreEqual(Constants.YourPolicySummary.TitleCaravanOrTrailerDetails, TitleCaravanOrTrailerDetails, $"section title '{Constants.YourPolicySummary.TitleCaravanOrTrailerDetails}' displayed");

            // This will update only the Rego of the Insured caravan. Usefull in a scenario where there is no
            // change to the make and model but only rego is changed
            if (!string.IsNullOrEmpty(endorseCaravan.NewInsuredAsset?.Registration) && !endorseCaravan.ChangeMakeAndModel)
            {
                Reporting.AreEqual($"Registration: {endorseCaravan.NewInsuredAsset.Registration}", PolicyCardRego, "caravan/trailer registration is displayed");

            }
            else if (DataHelper.IsRegistrationNumberConsideredValid(insuredCaravan.Registration))
            {
                Reporting.AreEqual($"Registration: {insuredCaravan.Registration}", PolicyCardRego, "caravan/trailer registration is displayed");
            }

            Reporting.AreEqual(Constants.YourPolicySummary.IsBusinessUse, IsBusinessUse, $"'{Constants.YourPolicySummary.IsBusinessUse}' is displayed");
            Reporting.IsTrue(Suburb.ToLower().StartsWith($"{Constants.YourPolicySummary.Suburb}{endorseCaravan.OriginalPolicyData.CaravanAsset.Suburb}".ToLower()), "Suburb were the caravan is kept is displayed");

            var isKeptInOnePlace = endorseCaravan.OriginalPolicyData.Covers.First().CoverTypeDescription.Contains("On-Site") ? "Yes" : "No";
            if (endorseCaravan.ChangeMakeAndModel)
            {
                isKeptInOnePlace = string.IsNullOrEmpty(endorseCaravan.Parked.GetDescription()) ? "Yes" : "No";
            }
            Reporting.AreEqual($"{Constants.YourPolicySummary.KeptPlace}{isKeptInOnePlace}", KeptPlace, $"'{Constants.YourPolicySummary.KeptPlace}' label is displayed");

            Reporting.AreEqual(Constants.YourPolicySummary.TitleYourPremium, TitleYourPremium, $"title {Constants.YourPolicySummary.TitleYourPremium} is displayed");

            var excess = endorseCaravan.OriginalPolicyData.Covers.First().StandardExcess.ToString().Replace(".0", "").Trim();
            Reporting.AreEqual($"{Constants.YourPolicySummary.BasicExcess}{DataHelper.ConvertIntToMonetaryString(Int32.Parse(excess), applyThousandsSeparator: false)}", 
                BasicExcess, "Basic Excess is displayed (without comma-separator if it is four figures)");

            Reporting.AreEqual($"{Constants.YourPolicySummary.AgreedValue}{endorseCaravan.NewAgreedValue}", AgreedValue, "Agreed Value is displayed");

            if (endorseCaravan.OriginalPolicyData.Covers.First().CoverTypeDescription.Equals("Trailer"))
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.YourPolicySummary.ContentCover), "Content cover option is not displayed for Trailer");
            }
            else if (endorseCaravan.OriginalPolicyData.Covers[1].CoverTypeDescription.Equals("Caravan Contents"))
            {
                Reporting.AreEqual($"{Constants.YourPolicySummary.ContentCover}{endorseCaravan.ContentCover}", ContentCover, "Content Cover is displayed");
            }
        }

        public void UpdateEmail(string newEmailAddress)
        {
            EmailAddress = newEmailAddress;
        }

        /// <summary>
        /// Verify the refund destination section displayed on the Review your Policy
        /// </summary>
        /// <param name="refundDestination">Set this for the expected refund destination</param>
        public void VerifyPaymentMethodAndMakePayment(EndorseCaravan endorseCar)
        {
            // TODO- Refatoring of this function -AUNT-119

            // Scroll base of page into view for screenshot
            ScrollElementIntoView(XPath.InputField.Email);
            Reporting.Log("Payment Method", _browser.Driver.TakeSnapshot());
            var refundDestination = endorseCar.RefundDestination;
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
