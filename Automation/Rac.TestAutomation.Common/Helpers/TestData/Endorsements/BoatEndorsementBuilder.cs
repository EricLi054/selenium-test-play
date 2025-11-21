using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Rac.TestAutomation.Common.TestData.Endorsements
{
    public class BoatEndorsementBuilder : EntityBuilder<EndorseBoat, BoatEndorsementBuilder>
    {
        /// <summary>
        /// Sets up core fields of the Endorsement data object based on the
        /// provided Boat policy number. Will default active policyholder
        /// to be the main policyholder, and payment method to be that
        /// member with the current payment method and frequency.
        /// </summary>
        private BoatEndorsementBuilder WithPolicy(string policyNumber, Contact policyHolder)
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
        public BoatEndorsementBuilder WithActivePolicyHolder(Contact policyHolder)
        {
            Set(x => x.ActivePolicyHolder, policyHolder);
            return this;
        }

        public BoatEndorsementBuilder WithPaymentMethod(Payment paymentMethod)
        {
            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        /// <summary>
        /// sets the payer to a specific contact. BoatBuilder initialisation
        /// methods will always set a PaymentMethod which defaults to the
        /// main policyholder and the existing payment method and frequency.
        /// This method allows tests to just change the payer.
        /// </summary>
        public BoatEndorsementBuilder WithPayer(Contact payer)
        {
            var paymentMethod = Get(x => x.PayMethod);
            paymentMethod.Payer = payer;

            return WithPaymentMethod(paymentMethod);
        }

        public BoatEndorsementBuilder WithRandomPaymentMethod(Contact payer)
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
        public BoatEndorsementBuilder WithNextPaymentDate(int days)
        {
            if (days == 0) { return this; }

            var currentPolicy = Get(x => x.OriginalPolicyData);
            if (currentPolicy.NextPendingInstallment() != null)
            {
                Set(x => x.NextPaymentDate, currentPolicy.NextPendingInstallment().CollectionDate.AddDays(days));
            }
            return this;
        }

        public BoatEndorsementBuilder InitialiseBoatWithDefaultData(string policynumber, Contact policyHolder = null)
        {
            return WithPolicy(policynumber, policyHolder);
        }

        /// <summary>
        /// For enhanced Spark payment options where member can
        /// choose BPay or delayed payment options, as well as
        /// conventional payment options
        /// </summary>
        /// TODO AUNT-166 - Merge into a single central method.
        public BoatEndorsementBuilder WithSparkPaymentChoice(PaymentV2 payment)
        {
            Set(x => x.SparkExpandedPayment, payment);
            return this;
        }

        protected override EndorseBoat BuildEntity()
        {
            return new EndorseBoat
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