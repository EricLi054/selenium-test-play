using System;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.SparkCommonConstants;

namespace Rac.TestAutomation.Common.TestData.Endorsements
{
    public class CaravanEndorsementBuilder : EntityBuilder<EndorseCaravan, CaravanEndorsementBuilder>
    {
        /// <summary>
        /// Sets up core fields of the Endorsement data object based on the
        /// provided home policy number. Will default active policyholder
        /// to be the main policyholder, and payment method to be that
        /// member with the current payment method and frequency.
        /// </summary>
        private CaravanEndorsementBuilder WithPolicy(string policyNumber, Contact policyHolder)
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

            var identifiedVehicles = DataHelper.GetVehicleDetails(policyDetails.CaravanAsset.VehicleId);
            if (identifiedVehicles?.Vehicles?.Count < 1)
            { Reporting.Error($"Test data creation failed as could not get details of vehicle identified by id={policyDetails.MotorAsset.VehicleId}"); }

            // Just use the first vehicle that matched the ID. Should almost always be only one match.
            var vehicleDetails = identifiedVehicles.Vehicles[0];
            var vehicle = new Caravan()
            {
                Make  = vehicleDetails.MakeDescription,
                Year  = vehicleDetails.ModelYear,
                Model = vehicleDetails.ModelDescription,
                MarketValue = vehicleDetails.Price,
                VehicleId = policyDetails.CaravanAsset.VehicleId,
                Registration = policyDetails.CaravanAsset.RegistrationNumber,
            };

            var currentPaymentDetails = new Payment(payer: activePH)
            {
                IsPaymentByBankAccount = policyDetails.IsPaidByBankAccount(),
                PaymentFrequency = policyDetails.GetPaymentFrequency()
            };

            Set(x => x.PolicyNumber, policyNumber);
            Set(x => x.OriginalPolicyData, policyDetails);
            Set(x => x.ActivePolicyHolder, activePH);
            Set(x => x.PayMethod, currentPaymentDetails);
            Set(x => x.InsuredAsset, vehicle);

            if (policyDetails.GetPaymentFrequency() != PaymentFrequency.Annual)
            {
                Set(x => x.NextPaymentDate, policyDetails.NextPendingInstallment().CollectionDate);
            }

            return this;
        }

        public CaravanEndorsementBuilder WithExpectedImpactOnPremium(PremiumChange change)
        {
            Set(x => x.ExpectedImpactOnPremium, change);
            return this;
        }

        public CaravanEndorsementBuilder WithRefundDestination(RefundToSource refundDestination)
        {
            Set(x => x.RefundDestination, refundDestination);
            return this;
        }

        public CaravanEndorsementBuilder WithExcess(string excess)
        {
            Set(x => x.Excess, excess);
            return this;
        }

        public CaravanEndorsementBuilder WithContentCover(string contentCover)
        {
            Set(x => x.ContentCover, contentCover);
            return this;
        }

        public CaravanEndorsementBuilder WithFailedPayment(bool isFailedPayment)
        {
            Set(x => x.isFailedPayment, isFailedPayment);
            return this;
        }

        public CaravanEndorsementBuilder WithRandomCaravanRego()
        {
            var vehicle = GetOrDefault(x => x.NewInsuredAsset, defaultValue: new Caravan());
            vehicle.Registration = DataHelper.RandomAlphanumerics(5, 10);
            Set(x => x.NewInsuredAsset, vehicle);
            return this;
        }

        public CaravanEndorsementBuilder WithRandomParkLocation()
        {
            var vehicle = GetOrDefault(x => x.NewInsuredAsset, defaultValue: new Caravan());
            vehicle.ParkLocation = DataHelper.GetRandomEnum<CaravanParkLocation>();
            vehicle.Registration = string.IsNullOrEmpty(vehicle.Registration) ? DataHelper.RandomAlphanumerics(5, 10) : vehicle.Registration;
            Set(x => x.NewInsuredAsset, vehicle);
            return this;
        }

        /// <summary>
        /// Change the active policyholder (member that test will be logged in as)
        /// to the contact provided. Only necessary if test does not want to use
        /// default, which is primary policyholder.
        /// </summary>
        public CaravanEndorsementBuilder WithActivePolicyHolder(Contact policyHolder)
        {
            Set(x => x.ActivePolicyHolder, policyHolder);
            return this;
        }

        public CaravanEndorsementBuilder WithEndorsementStartDate(DateTime endorsementStartDate)
        {
            Set(x => x.StartDate, endorsementStartDate);
            return this;
        }

        public CaravanEndorsementBuilder WithPaymentMethod(Payment paymentMethod)
        {
            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        /// <summary>
        /// sets the payer to a specific contact. HomeBuilder initialisation
        /// methods will always set a PaymentMethod which defaults to the
        /// main policyholder and the existing payment method and frequency.
        /// This method allows tests to just change the payer.
        /// </summary>
        public CaravanEndorsementBuilder WithPayer(Contact payer)
        {
            var paymentMethod = Get(x => x.PayMethod);
            paymentMethod.Payer = payer;

            return WithPaymentMethod(paymentMethod);
        }

        public CaravanEndorsementBuilder WithRandomPaymentMethod(Contact payer)
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
        public CaravanEndorsementBuilder WithNextPaymentDate(int days)
        {
            if (days == 0) { return this; }

            var currentPolicy = Get(x => x.OriginalPolicyData);
            if (currentPolicy.NextPendingInstallment() != null)
            {
                Set(x => x.NextPaymentDate, currentPolicy.NextPendingInstallment().CollectionDate.AddDays(days));
            }
            return this;
        }

        /// <summary>
        /// For enhanced Spark payment options where member can
        /// choose BPay or delayed payment options, as well as
        /// conventional payment options
        /// </summary>
        /// TODO AUNT-166
        public CaravanEndorsementBuilder WithSparkPaymentChoice(PaymentV2 payment)
        {
            Set(x => x.SparkExpandedPayment, payment);
            return this;
        }

        public CaravanEndorsementBuilder WithParked(CaravanParkLocation parked)
        {
            Set(x => x.Parked, parked);
            return this;
        }

        public CaravanEndorsementBuilder InitialiseCaravanWithDefaultData(string policynumber, Contact policyHolder = null)
        {
            return WithPolicy(policynumber, policyHolder);
        }

        public CaravanEndorsementBuilder WithRandomNewCaravan(int minValue = 1000, int maxValue = 150000)
        {
            var vehicle = GetRandomVehicle(minValue, maxValue);
            Set(x => x.ChangeMakeAndModel, true);
            Set(x => x.NewInsuredAsset, vehicle);
            return this;
        }

        public CaravanEndorsementBuilder WithNewCaravan(string make, decimal year, string model, int marketValue, string vehicleId = null)
        {
            var vehicle = new Caravan()
            {
                Make = make,
                Year = year,
                Model = model,
                MarketValue = marketValue,
                VehicleId = vehicleId,
               
            };
            Set(x => x.ChangeMakeAndModel, true);
            Set(x => x.NewInsuredAsset, vehicle);
            return this;
        }

        private static Caravan GetRandomVehicle(int minValue = 1000, int maxValue = 200000)
        {
            var randomCaravan = DataHelper.GetRandomInsurableCaravan(minValue, maxValue);
            if (randomCaravan.VehicleId == null)
            {
                Reporting.Error($"Error creating a random caravan object. VehicleId is null for {randomCaravan.MakeDescription}:{randomCaravan.ModelYear}:{randomCaravan.ModelFamily}:{randomCaravan.ModelDescription}");
            }
            return new Caravan()
            {
                Make = randomCaravan.MakeDescription,
                Year = randomCaravan.ModelYear,
                Model = randomCaravan.ModelFamily ?? randomCaravan.ModelDescription,
                VehicleId = randomCaravan.VehicleId,
                MarketValue = randomCaravan.Price
            };
        }

        protected override EndorseCaravan BuildEntity()
        {
            return new EndorseCaravan
            {
                PolicyNumber              = GetOrDefault(x => x.PolicyNumber),
                ActivePolicyHolder        = GetOrDefault(x => x.ActivePolicyHolder),
                ChangeMakeAndModel        = GetOrDefault(x => x.ChangeMakeAndModel),
                Parked                    = GetOrDefault(x => x.Parked),
                PayMethod                 = GetOrDefault(x => x.PayMethod),
                InsuredAsset              = GetOrDefault(x => x.InsuredAsset),
                NextPaymentDate           = GetOrDefault(x => x.NextPaymentDate),
                OriginalPolicyData        = GetOrDefault(x => x.OriginalPolicyData),
                SparkExpandedPayment      = GetOrDefault(x => x.SparkExpandedPayment),
                NewInsuredAsset           = GetOrDefault(x => x.NewInsuredAsset),
                StartDate                 = GetOrDefault(x => x.StartDate),
                ExpectedImpactOnPremium   = GetOrDefault(x => x.ExpectedImpactOnPremium),
                RefundDestination         = GetOrDefault(x => x.RefundDestination),
                isFailedPayment           = GetOrDefault(x => x.isFailedPayment),
                Excess                    = GetOrDefault(x => x.Excess),
                ContentCover              = GetOrDefault(x => x.ContentCover)
            };
        } 
    }
}