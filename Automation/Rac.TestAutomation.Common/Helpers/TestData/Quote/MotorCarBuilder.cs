using System;
using System.Collections.Generic;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace Rac.TestAutomation.Common.TestData.Quote
{
    public class MotorCarBuilder : EntityBuilder<QuoteCar, MotorCarBuilder>
    {
        private Config _config;

        public MotorCarBuilder()
        {
            _config = Config.Get();
        }

        public MotorCarBuilder WithUsage(VehicleUsage usage)
        {
            Set(x => x.UsageType, usage);
            return this;
        }

        public MotorCarBuilder WithRandomUsage(Contact mainDriver)
        {
            // For main drivers who are 25 or younger, we can get cover declined
            // if we get a usage of Business or Ridesharing. So we'll force
            // Private usage for young drivers. If a test wants to apply a
            // different usage, it can by using the "WithUsage(<usage type>)"
            // method.
            // Declined usage options are not included. Hence we use "VehicleUsageAccepted", and then cast to "VehicleUsage".
            var vehicleUsage = mainDriver.GetContactAge() <= 25 ? VehicleUsage.Private : (VehicleUsage)DataHelper.GetRandomEnum<VehicleUsageAccepted>();

            return WithUsage(vehicleUsage);
        }

        /// <summary>
        /// Selects from either Private or Ridesharing Part-time, to support
        /// TFT cover. As it does not permit the Business usage option.
        /// </summary>
        /// <returns></returns>
        public MotorCarBuilder WithLimitedValidRandomVehicleUsage() => WithUsage(Randomiser.Get.Next(2) > 0 ? VehicleUsage.Private : VehicleUsage.Ridesharing);

        public MotorCarBuilder WithAnnualKms(AnnualKms annualKms)
        {
            Set(x => x.AnnualKm, annualKms);
            return this;
        }

        public MotorCarBuilder WithRandomAnnualKms() => WithAnnualKms(DataHelper.GetRandomEnum<AnnualKms>(startIndex: 1));

        /// <summary>
        ///  With introduction of Motor Risk Address changes a full QAS-validated
        ///  address must be provided by the user instead of just the Suburb where
        ///  they keep their vehicle. If motor risk address is toggled off, automation
        ///  will just use the suburb and postcode.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public MotorCarBuilder WithParkingAddress(Address address)
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
        
        public MotorCarBuilder WithIsModified(bool isModified)
        {
            Set(x => x.IsModified, isModified);
            return this;
        }

        public MotorCarBuilder WithInsurer(string insurer)
        {
            Set(x => x.CurrentInsurer, insurer);
            return this;
        }

        public MotorCarBuilder WithoutInsurer()
        {
            Set(x => x.CurrentInsurer, null);
            return this;
        }

        public MotorCarBuilder WithRandomInsurer()
        {
            return WithInsurer(GetRandomInsurer());
        }

        public MotorCarBuilder WithFinancier(string financier)
        {
            Set(x => x.Financier, financier);
            return this;
        }

        public MotorCarBuilder WithoutFinancier()
        {
            Set(x => x.Financier, null);
            return this;
        }

        public MotorCarBuilder WithRandomFinancier()
        {
            return WithFinancier(FinancierOptions.OrderBy(t => Guid.NewGuid()).First());
        }

        public MotorCarBuilder WithRego(string rego)
        {
            Set(x => x.Registration, rego);
            return this;
        }

        public MotorCarBuilder WithoutRego()
        {
            Set(x => x.Registration, null);
            return this;
        }

        public MotorCarBuilder WithRandomRego()
        {
            return WithRego(DataHelper.RandomAlphanumerics(5,10));
        }

        public MotorCarBuilder WithVehicle(string make, decimal year, string model, string body, string transmission, int marketValue, string vehicleId = null)
        {
            Set(x => x.Make, make);
            Set(x => x.Year, year);
            Set(x => x.Model, model);
            Set(x => x.Body, body);
            Set(x => x.Transmission, transmission);
            Set(x => x.MarketValue, marketValue);
            Set(x => x.VehicleId, vehicleId);
            return this;
        }

        public MotorCarBuilder WithRandomVehicle(int minValue = MOTOR_COVER_MIN_INSURABLE_VALUE, int maxValue = MOTOR_COVER_MAX_INSURABLE_VALUE)
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

        public MotorCarBuilder WithoutDrivers()
        {
            Set(x => x.Drivers, null);
            return this;
        }

        /// <summary>
        /// Adds a driver to the test data. If no drivers have been added yet
        /// it will initialise a new list and add this contact as the first
        /// driver. A random value will be assigned for the time they have
        /// had their license, determined by their age.
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="isPolicyHolder"></param>
        /// <returns></returns>
        public MotorCarBuilder WithDriver(Contact contact, bool isPolicyHolder)
        {
            var drivers                 = GetOrDefault(x => x.Drivers, new List<Driver>()).ToList();
            var licenseTimeOptions      = new Dictionary<int, string>()
            {
                {17, "Less than 1 year" },
                {18, "1 year" },
                {19, "2 years" },
                {20, "3 years" },
                {21, "4 years" },
                {22, "5 years" },
                {23, "6 years" },
                {24, "7 years" },
                {25, "8 years or more" }
            };
            var chosenlicenseTimeOption = licenseTimeOptions.OrderBy(t => Guid.NewGuid()).First();
            var licenseTime             = chosenlicenseTimeOption.Value;
            var driverAge               = contact.GetContactAge();

            if (driverAge < 17)
            {
                licenseTime = licenseTimeOptions[17];
            }
            else if (driverAge < chosenlicenseTimeOption.Key)
            {
                licenseTime = licenseTimeOptions[driverAge];
            }

            var driver = new Driver()
            {
                Details = contact,
                IsPolicyHolderDriver = isPolicyHolder,
                LicenseConvictions = null,
                HistoricalAccidents = null,
                LicenseTime = licenseTime
            };

            drivers.Add(driver);
            Set(x => x.Drivers, drivers);
            return this;
        }

        /// <summary>
        /// NCB calculation depends partially on the number of years 
        /// the MainPh has been licensed, combined with the accident
        /// disclosures they have.
        /// </summary>
        /// <param name="licenseTime">String matching an option in the drop-down list (see licenseTimeOptions in MotorCarBuilder.cs).</param>
        public MotorCarBuilder WithMainDriverLicenseTime(string licenseTime)
        {
            var drivers = Get(x => x.Drivers);
            if (drivers == null || drivers.Count < 1)
            {
                Reporting.Error("No drivers have been added yet.");
            }

            drivers[0].LicenseTime = licenseTime;
            Set(x => x.Drivers, drivers);
            return this;
        }

        public MotorCarBuilder WithDriverConvictions(int index, List<Declaration> convictions)
        {
            var drivers = Get(x => x.Drivers);
            if (drivers == null || drivers.Count < (index+1))
            {
                Reporting.Error("No drivers have been added yet, or index out of range.");
            }

            drivers[index].LicenseConvictions = convictions;
            Set(x => x.Drivers, drivers);
            return this;
        }

        /// <summary>
        /// Generates between 1-4 random Driver Conviction disclosures for the contact 
        /// OR min/max can be defined to achieve desired number of entires.
        /// </summary>
        /// <param name="driverIndex">Which driver to assign disclosures to? NOTE 0 is first driver, 1 is second etc</param>
        /// <param name="min">(optional) define minimum number of disclosures randomly generated (if not provided 1).</param>
        /// <param name="max">(optional) define maximum number of disclosures randomly generated (if not provided 4).</param>
        /// <returns></returns>
        public MotorCarBuilder WithRandomDriverConvictions(int driverIndex, int min = 1, int max = 4)
        {
            var drivers = Get(x => x.Drivers);
            if (drivers == null || drivers.Count < (driverIndex + 1))
            {
                Reporting.Error("No drivers have been added yet, or index out of range.");
            }

            var convictions = new List<Declaration>();
            var count = DataHelper.RandomNumber(min, max);
            for (int i=0; i < count; i++)
            {
                var convictionDate = DataHelper.RandomDateInPastXYears(3);

                var conviction = new Declaration()
                {
                    Description = GetRandomConvictionText(),
                    Month = convictionDate.ToString("MMM"),
                    Year = convictionDate.Year.ToString()
                };
                convictions.Add(conviction);
            }

            drivers[driverIndex].LicenseConvictions = convictions;
            Set(x => x.Drivers, drivers);
            return this;
        }

        public MotorCarBuilder WithDriverAccidentHistory(int index, List<Declaration> history)
        {
            var drivers = Get(x => x.Drivers);
            if (drivers == null || drivers.Count < (index + 1))
            {
                Reporting.Error("No drivers have been added yet, or index out of range.");
            }

            drivers[index].HistoricalAccidents = history;
            Set(x => x.Drivers, drivers);
            return this;
        }

        /// <summary>
        /// Generates between 1-4 random Accident/Loss disclosures for the contact 
        /// OR min/max can be defined to achieve desired number of entires.
        /// </summary>
        /// <param name="driverIndex">Which driver to assign disclosures to? NOTE 0 is first driver, 1 is second etc</param>
        /// <param name="min">(optional) define minimum number of disclosures randomly generated.</param>
        /// <param name="max">(optional) define maximum number of disclosures randomly generated.</param>
        /// <returns></returns>
        /// <returns></returns>
        public MotorCarBuilder WithRandomDriverAccidentHistory(int driverIndex, int min = 1, int max = 4)
        {
            var drivers = Get(x => x.Drivers);
            if (drivers == null || drivers.Count < (driverIndex + 1))
            {
                Reporting.Error("No drivers have been added yet, or index out of range.");
            }

            var history = new List<Declaration>();
            var count = DataHelper.RandomNumber(min, max);
            for (int i = 0; i < count; i++)
            {
                var accidentDate = DataHelper.RandomDateInPastXYears(3);

                var accident = new Declaration()
                {
                    Description   = GetRandomAccidentText(),
                    InsurerAtTime = GetRandomInsurer(),
                    Month = accidentDate.ToString("MMM"),
                    Year  = accidentDate.Year.ToString()
                };
                history.Add(accident);
            }

            drivers[driverIndex].HistoricalAccidents = history;
            Set(x => x.Drivers, drivers);
            return this;
        }
        public MotorCarBuilder WithDisclosureDeclineNoticeThenDismiss(bool TriggerDeclineThenDismiss)
        {
            Set(x => x.DisclosureDeclineThenDismiss, TriggerDeclineThenDismiss);
            return this;
        }
        public MotorCarBuilder WithVehicleUsageDeclineThenDismiss(bool TriggerDeclineThenDismiss)
        {
            Set(x => x.VehicleUsageDeclineThenDismiss, TriggerDeclineThenDismiss);
            return this;
        }
        public MotorCarBuilder WithPaymentMethod(Payment paymentMethod)
        {
            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        /// <summary>
        /// Sets the provided contact as payer, and randomizes the
        /// payment terms. Will override any previously set payment
        /// details.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public MotorCarBuilder WithPayer(Contact contact)
        {
            Set(x => x.PayMethod, new Payment(contact));
            return this;
        }

        public MotorCarBuilder WithRandomPaymentMethod()
        {
            var drivers = Get(x => x.Drivers);
            Set(x => x.PayMethod, new Payment(drivers));
            return this;
        }

        public MotorCarBuilder WithAnnualPaymentFrequency()
        {
            var paymentMethod = Get(x => x.PayMethod);
            paymentMethod.PaymentFrequency = PaymentFrequency.Annual;
            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        public MotorCarBuilder WithMonthlyPaymentFrequency()
        {
            var paymentMethod = Get(x => x.PayMethod);
            paymentMethod.PaymentFrequency = PaymentFrequency.Monthly;
            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        public MotorCarBuilder WithPolicyStartDate(DateTime date)
        {
            Set(x => x.StartDate, date);
            return this;
        }

        public MotorCarBuilder WithPurchaseRoadsideAssistanceMembershipBundle(bool addRoadsideMembership)
        {
            Set(x => x.AddRoadside, addRoadsideMembership);
            return this;
        }

        public MotorCarBuilder WithHireCarAfterAccidentCover(bool addHireCarCover)
        {
            Set(x => x.AddHireCarAfterAccident, addHireCarCover);
            return this;
        }

        /// <summary>
        /// This sets the excess to "null" which tells the test to accept
        /// whatever Shield defaults the quote to based on rating.
        /// </summary>
        /// <returns></returns>
        public MotorCarBuilder WithDefaultExcess()
        {
            Set(x => x.Excess, null);
            return this;
        }

        public MotorCarBuilder WithExcess(string excess)
        {
            Set(x => x.Excess, excess);
            return this;
        }

        /// <summary>
        /// Takes a signed integer to reflect the percentage change in
        /// sum insured from market value for the insured vehicle.
        /// </summary>
        /// <param name="variance"></param>
        /// <returns></returns>
        public MotorCarBuilder WithPercentageInsuredValueChangeFromMarketValue(int variance)
        {
            Set(x => x.InsuredVariance, variance);
            return this;
        }

        public MotorCarBuilder WithCover(MotorCovers cover)
        {
            AmmendVehicle(cover);

            Set(x => x.CoverType, cover);
            return this;
        }

        public MotorCarBuilder WithRandomCover()
        {
            var randomCover = DataHelper.GetRandomEnum<MotorCovers>();
            AmmendVehicle(randomCover);
            
            return WithCover(randomCover);
        }

        public MotorCarBuilder InitialiseMotorCarWithRandomData(Contact mainDriver, bool isPolicyHolder)
        {
            var quoteMotorBuilder = WithRandomUsage(mainDriver)
                .WithRandomAnnualKms()
                .WithIsModified(false)
                .WithRandomInsurer()
                .WithRandomFinancier()
                .WithRandomVehicle()
                .WithRandomRego()
                .WithPolicyStartDate(DateTime.Now)
                .WithPurchaseRoadsideAssistanceMembershipBundle(false)
                .WithHireCarAfterAccidentCover(false)
                .WithDefaultExcess()
                .WithPercentageInsuredValueChangeFromMarketValue(0)
                .WithCover(MotorCovers.MFCO)
                .WithDriver(mainDriver, isPolicyHolder)
                .WithParkingAddress(mainDriver.MailingAddress)
                .WithRandomPaymentMethod()
                .WithDisclosureDeclineNoticeThenDismiss(false)
                .WithVehicleUsageDeclineThenDismiss(false)
                ;

            return quoteMotorBuilder;
        }

        protected override QuoteCar BuildEntity()
        {
            return new QuoteCar
            {
                UsageType      = GetOrDefault(x => x.UsageType),
                AnnualKm       = GetOrDefault(x => x.AnnualKm),
                ParkingAddress = GetOrDefault(x => x.ParkingAddress),
                CurrentInsurer = GetOrDefault(x => x.CurrentInsurer),
                Financier      = GetOrDefault(x => x.Financier),
                Drivers        = GetOrDefault(x => x.Drivers),
                PayMethod      = GetOrDefault(x => x.PayMethod),
                IsModified     = GetOrDefault(x => x.IsModified),
                Make           = GetOrDefault(x => x.Make),
                Year           = GetOrDefault(x => x.Year),
                Model          = GetOrDefault(x => x.Model),
                Body           = GetOrDefault(x => x.Body),
                Transmission   = GetOrDefault(x => x.Transmission),
                MarketValue    = GetOrDefault(x => x.MarketValue),
                VehicleId      = GetOrDefault(x => x.VehicleId),
                Registration   = GetOrDefault(x => x.Registration),
                InsuredVariance = GetOrDefault(x => x.InsuredVariance),
                StartDate      = GetOrDefault(x => x.StartDate),
                AddRoadside    = GetOrDefault(x => x.AddRoadside),
                AddHireCarAfterAccident = GetOrDefault(x => x.AddHireCarAfterAccident),
                Excess         = GetOrDefault(x => x.Excess),
                CoverType      = GetOrDefault(x => x.CoverType),
                DisclosureDeclineThenDismiss = GetOrDefault(x => x.DisclosureDeclineThenDismiss),
                VehicleUsageDeclineThenDismiss = GetOrDefault(x => x.VehicleUsageDeclineThenDismiss)
            };
        }

        private string GetRandomConvictionText()
        {
            var convictionOptions = new[]
            {
                "Alcohol 0.02% - 0.049%",
                "Alcohol 0.05% - 0.079%",
                "Alcohol 0.08% - 0.149%",
                "Alcohol excess 0.149% ",
                "Careless driving",
                "Create false belief",
                "Dangerous driving",
                "Dangerous driving causing bodily harm",
                "Dangerous driving causing death",
                "Dangerous driving causing gbh",
                "Demerit point suspension",
                "Driving under the influence of drugs",
                "Driving whilst under suspension",
                "Driving without a licence",
                "Driving without due care and attention",
                "Failing to report an accident",
                "Failing to stop at the scene of an accident",
                "Negligent driving",
                "Reckless driving",
                "Refusing a blood test",
                "Refusing a breath test",
                "Refusing a urine test",
                "Speeding 30-40 km/h over the limit",
                "Speeding more than 40 km/h over the limit",
                "Unauthorised use of a motor vehicle",
                "Wilfully mislead"
            };

            return convictionOptions.OrderBy(t => Guid.NewGuid()).First();
        }

        private string GetRandomAccidentText()
        {
            var accidentOptions = new[]
            {
                "Collision - At Fault",
                "Collision - Not At Fault",
                "Fire",
                "Flood",
                "Hit While Parked",
                "Malicious Damage",
                "Other",
                "Storm",
                "Theft/Attempted Theft"
            };

            return accidentOptions.OrderBy(t => Guid.NewGuid()).First();
        }

        private string GetRandomInsurer()
        {
            var insurerOptions = new[]
            {
                "ALLIANZ",
                "BUDGET DIRECT",
                "CGU",
                NO_PREVIOUS_INSURANCE,
                "RAC WA",
                "RAC WA",
                "RAC WA",
                "RACQ",
                "OTHER"
            };

            return insurerOptions.OrderBy(t => Guid.NewGuid()).First();
        }

        private void AmmendVehicle(MotorCovers cover)
        {
            if (cover == MotorCovers.TFT)
            {
                // Setting upper limit to $5k less the current sum insured for TFT leaving enough buffer for higher spec variants that may appear in vehicle search list.
                var vehicle = GetRandomVehicle(maxValue: MOTOR_COVER_TFT_MAXVALUE - 5000);
                WithVehicle(make: vehicle.Make,
                                   year: vehicle.Year,
                                   model: vehicle.Model,
                                   body: vehicle.Body,
                                   transmission: vehicle.Transmission,
                                   marketValue: vehicle.MarketValue);
            }
        }

        private static Car GetRandomVehicle(int minValue = MOTOR_COVER_MIN_INSURABLE_VALUE, int maxValue = MOTOR_COVER_MAX_INSURABLE_VALUE)
        {
            return DataHelper.GetRandomInsurableCar(minValue, maxValue);
        }
    }
}
