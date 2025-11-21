using System;
using System.Collections.Generic;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace Rac.TestAutomation.Common.TestData.Endorsements
{
    public class MotorEndorsementBuilder : EntityBuilder<EndorseCar, MotorEndorsementBuilder>
    {
        private Config _config;

        public MotorEndorsementBuilder()
        {
            _config = Config.Get();
        }

        /// <summary>
        /// Not used to change the policy cover, but in supporting validation
        /// that cover is correctly displayed and maintained in PCM.
        /// </summary>
        /// <param name="coverType"></param>
        /// <returns></returns>
        public MotorEndorsementBuilder WithCover(MotorCovers coverType)
        {
            Set(x => x.CoverType, coverType);
            return this;
        }

        public MotorEndorsementBuilder WithUsage(VehicleUsage usage)
        {
            Set(x => x.UsageType, usage);
            return this;
        }

        public MotorEndorsementBuilder WithRandomUsage()
        {
            // Declined usage options are not included. Hence we use "VehicleUsageAccepted", and then cast to "VehicleUsage".
            Array values = Enum.GetValues(typeof(VehicleUsageAccepted));
            VehicleUsage vehicleUsage = (VehicleUsage)values.GetValue(Randomiser.Get.Next(values.Length));

            return WithUsage(vehicleUsage);
        }

        public MotorEndorsementBuilder WithAnnualKms(AnnualKms annualKms)
        {
            Set(x => x.AnnualKm, annualKms);
            return this;
        }

        public MotorEndorsementBuilder WithPreferredCollectionDay(int dayNumber)
        {
            Set(x => x.PreferredCollectionDay, dayNumber);
            return this;
        }

        public MotorEndorsementBuilder WithRandomAnnualKms() => WithAnnualKms(DataHelper.GetRandomEnum<AnnualKms>(startIndex: 1));

        /// <summary>
        /// Method to set where the vehicle is parked overnight. Usable for where
        /// Motor endorsements only require a suburb and postcode, but also where
        /// a full street address is required.
        /// 
        /// If the provided address is determined to be a PO box and a full street
        /// address is required, then the code will default to the "Generic" risk
        /// address (See MotorRiskAddress.Generic constant).
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public MotorEndorsementBuilder WithVehicleParkingAddress(Address address)
        {
            if (_config.IsMotorRiskAddressEnabled() &&
                (address.QASStreetAddress().ToLower().Contains("po box") || address.QASStreetAddress().ToLower().Contains("locked bag")))
            {
                Set(x => x.ParkingAddress, MotorRiskAddress.Generic);
            }
            else
            {
                Set(x => x.ParkingAddress, address);
            }
            return this;
        }

        public MotorEndorsementBuilder WithIsModified(bool isModified)
        {
            var vehicle = Get(x => x.InsuredAsset);
            vehicle.IsModified = isModified;
            Set(x => x.InsuredAsset, vehicle);
            return this;
        }

        public MotorEndorsementBuilder WithFinancier(string financier)
        {
            // We set the financier in both locations to
            // keep them in sync.
            var vehicle = GetOrDefault(x => x.NewInsuredAsset);
            if (vehicle != null)
            {
                vehicle.Financier = financier;
                Set(x => x.NewInsuredAsset, vehicle);
            }
            Set(x => x.Financier, financier);
            return this;
        }

        public MotorEndorsementBuilder WithoutFinancier()
        {
            return WithFinancier(null);
        }

        public MotorEndorsementBuilder WithRandomFinancier()
        {
            return WithFinancier(FinancierOptions.OrderBy(t => Guid.NewGuid()).First());
        }

        public MotorEndorsementBuilder WithRego(string rego)
        {
            var vehicle = Get(x => x.InsuredAsset);
            vehicle.Registration = rego;
            Set(x => x.InsuredAsset, vehicle);
            return this;
        }

        public MotorEndorsementBuilder WithoutRego()
        {
            return WithRego(null);
        }

        public MotorEndorsementBuilder WithRandomRego()
        {
            return WithRego(DataHelper.RandomAlphanumerics(5, 10));
        }

        public MotorEndorsementBuilder ChangeCarMakeAndModel(bool changeCarMakeAndModel)
        {
            Set(x => x.ChangeMakeAndModel, changeCarMakeAndModel);
            return this;
        }

        public MotorEndorsementBuilder WithVehicle(string make, decimal year, string model, string body, string transmission, int marketValue, string vehicleId = null)
        {
            var vehicle = Get(x => x.InsuredAsset);
            vehicle.Make         = make;
            vehicle.Year         = year;
            vehicle.Model        = model;
            vehicle.Body         = body;
            vehicle.Transmission = transmission;
            vehicle.MarketValue  = marketValue;
            vehicle.VehicleId    = vehicleId;
            Set(x => x.InsuredAsset, vehicle);
            return this;
        }

        public MotorEndorsementBuilder WithRandomVehicle(int minValue = 1000, int maxValue = 200000)
        {
            var vehicle = GetRandomVehicle(minValue, maxValue);
            return WithVehicle(make: vehicle.Make,
                               year: vehicle.Year,
                               model: vehicle.Model,
                               body: vehicle.Body,
                               transmission: vehicle.Transmission,
                               marketValue: vehicle.MarketValue,
                               vehicleId: vehicle.VehicleId);
        }

        public MotorEndorsementBuilder WithRandomMotorWebTestVehicle()
        {
            var vehicle = MotorWebTestVehicles.PickRandom();
            return WithNewCar(make: vehicle.Make,
                               year: vehicle.Year,
                               model: vehicle.Model,
                               body: vehicle.Body,
                               transmission: vehicle.Transmission,
                               marketValue: vehicle.MarketValue,
                               vehicleId: vehicle.VehicleId);
        }

        public MotorEndorsementBuilder WithNewCar(string make, decimal year, string model, string body, string transmission, int marketValue, string vehicleId = null)
        {
            var vehicle = new Car();
            vehicle.Make = make;
            vehicle.Year = year;
            vehicle.Model = model;
            vehicle.Body = body;
            vehicle.Transmission = transmission;
            vehicle.MarketValue = marketValue;
            vehicle.VehicleId = vehicleId;
            Set(x => x.NewInsuredAsset, vehicle);
            return this;
        }

        public MotorEndorsementBuilder WithNewCar(Car newCar)
        {
            Set(x => x.NewInsuredAsset, newCar);
            return this;
        }

        public MotorEndorsementBuilder WithRandomNewCar()
        {
            var vehicle = GetRandomVehicle(1000, 200000);
            return WithNewCar(make: vehicle.Make,
                               year: vehicle.Year,
                               model: vehicle.Model,
                               body: vehicle.Body,
                               transmission: vehicle.Transmission,
                               marketValue: vehicle.MarketValue,
                               vehicleId: vehicle.VehicleId);
        }

        public MotorEndorsementBuilder WithRandomLowValueCar()
        {
            return WithNewCar(DataHelper.FindRandomLowValueLowRatingVehicle());
        }

        public MotorEndorsementBuilder WithRandomHighValueCar()
        {
            return WithNewCar(DataHelper.FindRandomHighValueHighRatingVehicle());
        }

        public MotorEndorsementBuilder WithRandomNewCarRego()
        {
            var vehicle = GetOrDefault(x => x.NewInsuredAsset);
            if (vehicle == null)
            {
                Set(x => x.NewInsuredAsset, new Car());
            }
            vehicle.Registration = DataHelper.RandomAlphanumerics(5, 10);
            Set(x => x.NewInsuredAsset, vehicle);
            return this;
        }

        public MotorEndorsementBuilder WithNewCarIsModified(bool isModified)
        {
            var vehicle = Get(x => x.NewInsuredAsset);
            vehicle.IsModified = isModified;
            Set(x => x.NewInsuredAsset, vehicle);
            return this;
        }

        public MotorEndorsementBuilder WithRandomNewCarFinancier()
        {
            return WithFinancier(FinancierOptions.OrderBy(t => Guid.NewGuid()).First());
        }

        public MotorEndorsementBuilder WithoutVehicle()
        {
            Set(x => x.InsuredAsset, new Car());
            return this;
        }

        /// <summary>
        /// For enhanced Spark payment options where member can
        /// choose BPay or delayed payment options, as well as
        /// conventional payment options
        /// </summary>
        /// TODO AUNT-166
        public MotorEndorsementBuilder WithSparkPaymentChoice(PaymentV2 payment)
        {
            Set(x => x.SparkExpandedPayment, payment);
            return this;
        }

        public MotorEndorsementBuilder WithPaymentMethod(Payment paymentMethod)
        {
            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        public MotorEndorsementBuilder WithRandomPaymentMethod(Contact payer)
        {
            var paymentMethod = new Payment(payer);
            paymentMethod.PaymentFrequency = DataHelper.GetRandomEnum<PaymentFrequency>(startIndex: (int)PaymentFrequency.Monthly);
            paymentMethod.IsPaymentByBankAccount = Randomiser.Get.Next(2) == 0;

            return WithPaymentMethod(paymentMethod);
        }

        /// <summary>
        /// Sets the provided contact as payer, and randomizes the
        /// payment terms. Will override any previously set payment
        /// details.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public MotorEndorsementBuilder WithPayer(Contact contact)
        {
            Set(x => x.PayMethod, new Payment(contact));
            return this;
        }

        public MotorEndorsementBuilder WithAnnualPaymentFrequency()
        {
            var paymentMethod = Get(x => x.PayMethod);

            if (paymentMethod == null)
            {
                Reporting.Error("Payment method has not been initialised");
            }

            paymentMethod.PaymentFrequency = PaymentFrequency.Annual;

            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        public MotorEndorsementBuilder WithMonthlyPaymentFrequency()
        {
            var paymentMethod = Get(x => x.PayMethod);

            if (paymentMethod == null)
            {
                Reporting.Error("Payment method has not been initialised");
            }

            paymentMethod.PaymentFrequency = PaymentFrequency.Monthly;

            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        /// <summary>
        /// To support "Update How I Pay" tests, by setting the next instalment
        /// date relative to the current next instalment.
        /// </summary>
        /// <param name="days">integer of how many days (relative to current next instalment) to use in test</param>
        public MotorEndorsementBuilder WithNextPaymentDate(int days)
        {
            if (days == 0) { return this; }

            var currentPolicy = Get(x => x.OriginalPolicyData);
            if (currentPolicy.NextPendingInstallment() != null)
            {
                Set(x => x.NextPaymentDate, currentPolicy.NextPendingInstallment().CollectionDate.AddDays(days));
            }
            return this;
        }

        public MotorEndorsementBuilder WithPolicyStartDate(DateTime date)
        {
            Set(x => x.StartDate, date);
            return this;
        }

        /// <summary>
        /// This sets the excess to "null" which tells the test to accept
        /// whatever Shield defaults the quote to based on rating.
        /// </summary>
        /// <returns></returns>
        public MotorEndorsementBuilder WithDefaultExcess()
        {
            Set(x => x.Excess, null);
            return this;
        }

        public MotorEndorsementBuilder WithExcess(string excess)
        {
            int requestedExcess = DataHelper.ConvertMonetaryStringToInt(excess);
            /* TODO INSU-286: once this refenced JIRA ticket is actioned we can remove the
             * following code. In the meantime, we need to do an alignment of excess values
             * based on product version, because they have different supported excess values
             * based on whether it is an older version, or newer, relative to the excess/NCB
             * project.
             * NOTE: When removing this code, look at users of this method and adjust the values
             * they are passing in. e.g.: no more $0 excess.
             * Estimated that this work should be done after August 2025.
             */
            List<int> acceptableExcess;
            var currentPolicy = Get(x => x.OriginalPolicyData);
            if (currentPolicy.ProductVersionAsInteger >= MotorProductVersionIdWithExcessNcbChanges)
            {
                acceptableExcess = new List<int>() { 400, 700, 800, 900, 1000, 1250, 1500, 1750, 2000 };
            }
            else
            {
                acceptableExcess = new List<int>() { 0, 500, 600, 650, 700, 750, 800, 1000, 1250, 1500, 2000 };
            }
            int matchedExcess = acceptableExcess.OrderBy(xs => Math.Abs(requestedExcess - xs)).First();
            Reporting.Log($"Revised excess from {excess} to nearest ${matchedExcess} base on policy version, {currentPolicy.ProductVersionId}.");

            Set(x => x.Excess, $"${matchedExcess}");
            return this;
        }

        /// <summary>
        /// Sum insured variance is a signed integer reflecting the change that
        /// is desired to the presented sum insured during endorsement.
        /// For example:
        ///   A value of 14 equates to increasing by 14%
        ///   A value of -5 equates to reducing by 5%
        ///   
        /// This avoids the need to have whole numbers in the test data, but
        /// user needs to be mindful of the absolute bounds for allowed SI.
        /// </summary>
        /// <param name="variance"></param>
        /// <returns></returns>
        public MotorEndorsementBuilder WithSumInsuredVariance(int variance)
        {
            Set(x => x.InsuredVariance, variance);
            return this;
        }

        public MotorEndorsementBuilder WithPolicyAndContact(string policyNumber, Contact policyholder = null)
        {
            WithPolicy(policyNumber, policyholder);            
            return this;
        }

        public MotorEndorsementBuilder WithExpectedImpactOnPremium(PremiumChange change)
        {
            Set(x => x.ExpectedImpactOnPremium, change);
            return this;
        }

        public MotorEndorsementBuilder WithCurrentProductVersionNumber(int version)
        {
            Set(x => x.CurrentProductVersionNumber, version);
            return this;
        }

        private MotorEndorsementBuilder WithPolicy(string policyNumber, Contact policyHolder)
        {
            var policyDetails = DataHelper.GetPolicyDetails(policyNumber);
            Contact activePH = null;
            if (policyHolder == null)
            {
                activePH = new ContactBuilder(DataHelper.MapContactWithPersonAPI(policyDetails.Policyholder.Id.ToString(), policyDetails.Policyholder.ContactExternalNumber)).Build();
            }
            else
            {
                activePH = new ContactBuilder(policyHolder).Build();
            }

            var identifiedVehicles = DataHelper.GetVehicleDetails(policyDetails.MotorAsset.VehicleId);
            if (identifiedVehicles?.Vehicles?.Count < 1)
            { Reporting.Error($"Test data creation failed as could not get details of vehicle identified by id={policyDetails.MotorAsset.VehicleId}"); }

            // Just use the first vehicle that matched the ID. Should almost always be only one match.
            var vehicleDetails = identifiedVehicles.Vehicles[0];
            var vehicle = new Car()
            {
                Make  = vehicleDetails.MakeDescription,
                Year  = vehicleDetails.ModelYear,
                Model = vehicleDetails.ModelDescription,
                Body  = vehicleDetails.VehicleSubTypeDescription,
                Transmission = vehicleDetails.TransmissionDescription,
                MarketValue  = vehicleDetails.Price,
                VehicleId    = policyDetails.MotorAsset.VehicleId,
                Registration = policyDetails.MotorAsset.RegistrationNumber.Trim(),
                IsModified   = policyDetails.MotorAsset.IsVehicleModified(),
            };

            var currentPaymentDetails = new Payment(payer: activePH)
            {
                IsPaymentByBankAccount = policyDetails.IsPaidByBankAccount(),
                PaymentFrequency = policyDetails.GetPaymentFrequency()
            };

            Set(x => x.InsuredAsset, vehicle);
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

        public MotorEndorsementBuilder WithPolicyData(EndorseCar endorseCar)
        {
            var policyData = DataHelper.GetPolicyDetails(endorseCar.PolicyNumber);
            var identifiedVehicles = DataHelper.GetVehicleDetails(policyData.MotorAsset.VehicleId);
            if (identifiedVehicles?.Vehicles?.Count < 1)
            { Reporting.Error($"Test data creation failed as could not get details of vehicle identified by id={policyData.MotorAsset.VehicleId}"); }

            // Just use the first vehicle that matched the ID. Should almost always be only one match.
            var vehicleDetails = identifiedVehicles.Vehicles[0];
            var vehicle = new Car()
            {
                Make = vehicleDetails.MakeDescription,
                Year = vehicleDetails.ModelYear,
                Model = vehicleDetails.ModelDescription,
                Body = vehicleDetails.VehicleSubTypeDescription,
                Transmission = vehicleDetails.TransmissionDescription,
                MarketValue = vehicleDetails.Price,
                VehicleId = policyData.MotorAsset.VehicleId,
                Registration = policyData.MotorAsset.RegistrationNumber,
                IsModified = policyData.MotorAsset.IsVehicleModified(),
                Financier = policyData.GetFinancierNameViaShieldAPI()
            };

            Set(x => x.InsuredAsset, vehicle);
            Set(x => x.PolicyNumber, endorseCar.PolicyNumber);
            Set(x => x.ActivePolicyHolder, endorseCar.ActivePolicyHolder);
            Set(x => x.CoverType, endorseCar.CoverType);
            Set(x => x.PayMethod, endorseCar.PayMethod);
            Set(x => x.OriginalPolicyData, policyData);
            Set(x => x.CurrentProductVersionNumber, endorseCar.CurrentProductVersionNumber);
            Set(x => x.ExpectedImpactOnPremium, PremiumChange.NotApplicable);

            return this;
        }

        /// <summary>
        /// Minimal initialisation of endorsement class, which just loads
        /// existing policy information, and defaults contact reference
        /// to primary policyholder.
        /// </summary>
        /// <param name="policyNumber">PolicyNumber to be endorsed.</param>
        public MotorEndorsementBuilder InitialiseMotorCarWithDefaultData(string policyNumber, Contact policyHolder = null)
        {
            var motorEndorsementBuilder = WithPolicy(policyNumber, policyHolder);

            return motorEndorsementBuilder;
        }

        protected override EndorseCar BuildEntity()
        {
            return new EndorseCar
            {
                PolicyNumber    = GetOrDefault(x => x.PolicyNumber),
                ActivePolicyHolder = GetOrDefault(x => x.ActivePolicyHolder),
                ChangeMakeAndModel = GetOrDefault(x => x.ChangeMakeAndModel),
                CoverType       = GetOrDefault(x => x.CoverType),
                UsageType       = GetOrDefault(x => x.UsageType),
                AnnualKm        = GetOrDefault(x => x.AnnualKm),
                ParkingAddress  = GetOrDefault(x => x.ParkingAddress),
                Financier       = GetOrDefault(x => x.Financier),
                PayMethod       = GetOrDefault(x => x.PayMethod),
                InsuredAsset    = GetOrDefault(x => x.InsuredAsset),
                NewInsuredAsset = GetOrDefault(x => x.NewInsuredAsset),
                InsuredVariance = GetOrDefault(x => x.InsuredVariance),
                StartDate       = GetOrDefault(x => x.StartDate),
                Excess          = GetOrDefault(x => x.Excess),
                ExpectedImpactOnPremium = GetOrDefault(x => x.ExpectedImpactOnPremium),
                CurrentProductVersionNumber   = GetOrDefault(x => x.CurrentProductVersionNumber),
                OriginalPolicyData      = GetOrDefault(x => x.OriginalPolicyData),
                SparkExpandedPayment    = GetOrDefault(x => x.SparkExpandedPayment),
                NextPaymentDate         = GetOrDefault(x => x.NextPaymentDate)
            };
        }

        private static Car GetRandomVehicle(int minValue = 1000, int maxValue = 200000)
        {
            return DataHelper.GetRandomInsurableCar(minValue, maxValue);
        }
    }
}