using System;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace Rac.TestAutomation.Common.TestData.Endorsements
{
    public class MotorCycleEndorsementBuilder : EntityBuilder<EndorseMotorCycle, MotorCycleEndorsementBuilder>
    {
        /// <summary>
        /// Sets up core fields of the Endorsement data object based on the
        /// provided motor cycle policy number. Will default active policyholder
        /// to be the main policyholder, and payment method to be that
        /// member with the current payment method and frequency.
        /// </summary>
        private MotorCycleEndorsementBuilder WithPolicy(string policyNumber, Contact policyHolder)
        {
            Contact activePH = null;
            var policyDetails = DataHelper.GetPolicyDetails(policyNumber);
            if (policyHolder == null)
            {
                activePH = new ContactBuilder(DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber)).Build();
            }
            else
            {
                activePH = new ContactBuilder(policyHolder).Build();
            }
            var currentPaymentDetails = new Payment(payer: activePH)
            {
                IsPaymentByBankAccount = policyDetails.IsPaidByBankAccount(),
                PaymentFrequency = policyDetails.GetPaymentFrequency()
            };

            Set(x => x.PolicyNumber, policyNumber);
            Set(x => x.OriginalPolicyData, policyDetails);
            Set(x => x.ActivePolicyHolder, activePH);
            Set(x => x.PayMethod, currentPaymentDetails);

            if (policyDetails.GetPaymentFrequency() != PaymentFrequency.Annual)
            {
                Set(x => x.NextPaymentDate, policyDetails.NextPendingInstallment().CollectionDate);
            }

            return this;
        }

        /// <summary>
        /// Change the active policyholder (member that test will be logged in as)
        /// to the contact provided. Only necessary if test does not want to use
        /// default, which is primary policyholder.
        /// </summary>
        public MotorCycleEndorsementBuilder WithActivePolicyHolder(Contact policyHolder)
        {
            Set(x => x.ActivePolicyHolder, policyHolder);
            return this;
        }

        public MotorCycleEndorsementBuilder WithPaymentMethod(Payment paymentMethod)
        {
            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        /// <summary>
        /// sets the payer to a specific contact. MotorCycleBuilder initialisation
        /// methods will always set a PaymentMethod which defaults to the
        /// main policyholder and the existing payment method and frequency.
        /// This method allows tests to just change the payer.
        /// </summary>
        public MotorCycleEndorsementBuilder WithPayer(Contact payer)
        {
            var paymentMethod = Get(x => x.PayMethod);
            paymentMethod.Payer = payer;

            return WithPaymentMethod(paymentMethod);
        }

        public MotorCycleEndorsementBuilder WithRandomPaymentMethod(Contact payer)
        {
            var paymentMethod = new Payment(payer);
            paymentMethod.PaymentFrequency = DataHelper.GetRandomEnum<PaymentFrequency>(startIndex: (int)PaymentFrequency.Monthly);
            paymentMethod.IsPaymentByBankAccount = Randomiser.Get.Next(2) == 0;

            return WithPaymentMethod(paymentMethod);
        }

        /// <summary>
        /// To support "Update How I Pay" tests, by setting the next instalment
        /// date relative to the current next instalment.
        /// </summary>
        /// <param name="days">integer of how many days (relative to current next instalment) to use in test</param>
        public MotorCycleEndorsementBuilder WithNextPaymentDate(int days)
        {
            if (days == 0) { return this; }

            var currentPolicy = Get(x => x.OriginalPolicyData);
            if (currentPolicy.NextPendingInstallment() != null)
            {
                Set(x => x.NextPaymentDate, currentPolicy.NextPendingInstallment().CollectionDate.AddDays(days));
            }
            return this;
        }

        public MotorCycleEndorsementBuilder InitialiseMotorCycleWithDefaultData(string policynumber, Contact policyHolder = null)
        {
            return WithPolicy(policynumber, policyHolder);
        }

        /// <summary>
        /// For enhanced Spark payment options where member can
        /// choose BPay or delayed payment options, as well as
        /// conventional payment options
        /// </summary>
        /// TODO AUNT-166
        public MotorCycleEndorsementBuilder WithSparkPaymentChoice(PaymentV2 payment)
        {
            Set(x => x.SparkExpandedPayment, payment);
            return this;
        }

        protected override EndorseMotorCycle BuildEntity()
        {
            return new EndorseMotorCycle
            {
                PolicyNumber              = GetOrDefault(x => x.PolicyNumber),
                ActivePolicyHolder        = GetOrDefault(x => x.ActivePolicyHolder),
                PayMethod                 = GetOrDefault(x => x.PayMethod),
                NextPaymentDate           = GetOrDefault(x => x.NextPaymentDate),
                OriginalPolicyData        = GetOrDefault(x => x.OriginalPolicyData),
                SparkExpandedPayment      = GetOrDefault(x => x.SparkExpandedPayment),
            };
        }
    }
}