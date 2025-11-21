using System;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace Rac.TestAutomation.Common
{
    public class EndorsementBase
    {
        public string PolicyNumber { get; set; }
        public Contact ActivePolicyHolder { get; set; }
        public DateTime RenewalDate { get; set; }
        /// <summary>
        /// Used for "Update How I Pay" to set the next instalment date.
        /// Should be initialised to match the current next instalment.
        /// </summary>
        public DateTime NextPaymentDate { get; set; }
        public Payment PayMethod { get; set; }
        public string PremiumLabelText { get; set; }
        public int PreferredCollectionDay { get; set; }
        public API.GetQuotePolicy_Response OriginalPolicyData { get; set; }

        /// <summary>
        /// Desired start date of endorsement
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Current product version of policy. This was introduced for the
        /// Motor Risk Address changes and how they impacted Motor Policy
        /// endorsements (B2C-4610)
        /// </summary>

        /// <summary>
        /// Applicable for expanded payment options offered in Spark policy endorsement
        /// applications which offer increased options including BPay, Pay Later, and
        /// re-using existing account details on record.
        /// </summary>
        public PaymentV2 SparkExpandedPayment { get; set; }

        public int CurrentProductVersionNumber { get; set; }
        public PremiumChange ExpectedImpactOnPremium { get; set; }

        /// <summary>
        /// Applicable for Spark Caravan endorsement
        /// application can pass the expected refund destination to be used on test flow
        /// </summary>
        public RefundToSource RefundDestination { get; set; }

        /// <summary>
        /// Used to drive failed payment scenarios
        /// </summary>
        public bool isFailedPayment { get; set; }
        public string Financier { get; set; }

        /// <summary>
        /// Endorsements will require member to enter credit card details
        /// for a one-off payment if they have paid via Credit Card Annual.
        /// </summary>
        /// <returns></returns>
        public bool IsExpectedToMakeOneOffPayment() => ExpectedImpactOnPremium == PremiumChange.PremiumIncrease && IsAnnualCreditCardPaymentMethod();

        public bool IsExpectedToReceiveRefund() => ExpectedImpactOnPremium == PremiumChange.PremiumDecrease && PayMethod.IsAnnual;

        public bool IsAnnualCreditCardPaymentMethod() => PayMethod.Scenario == PaymentScenario.AnnualCash;

    }
}
