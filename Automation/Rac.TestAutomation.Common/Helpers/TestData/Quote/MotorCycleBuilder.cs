using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;

namespace Rac.TestAutomation.Common.TestData.Quote
{
    public class MotorCycleBuilder : EntityBuilder<QuoteMotorcycle, MotorCycleBuilder>
    {
        // Used for the API connections for fetching details of a random vehicle
        [SuppressMessage("SonarLint", "S4487", Justification = "Safely unused variable, follows pattern of other ProductBuilders")]
        private Config _config;


        public MotorCycleBuilder()
        {
            _config = Config.Get();
        }

        public MotorCycleBuilder WithUsage(MotorcycleUsage usage)
        {
            Set(x => x.UsageType, usage);
            return this;
        }

        public MotorCycleBuilder WithRandomUsage() => WithUsage((MotorcycleUsage)DataHelper.GetRandomEnum<MotorcycleUsageValid>());

        public MotorCycleBuilder WithAnnualKms(AnnualKms annualKms)
        {
            Set(x => x.AnnualKm, annualKms);
            return this;
        }

        public MotorCycleBuilder WithRandomAnnualKms() => WithAnnualKms(DataHelper.GetRandomEnum<AnnualKms>(startIndex: 1));

        public MotorCycleBuilder WithImmobiliser()
        {
            Set(x => x.HasImmobiliser, true);
            return this;
        }

        public MotorCycleBuilder WithTracker()
        {
            Set(x => x.HasTracker, true);
            return this;
        }

        public MotorCycleBuilder WithParkingSuburb(string suburb, string postcode)
        {
            var address = new Address()
            {
                Suburb = suburb,
                PostCode = postcode
            };
            return WithParkingSuburbFromAddress(address);
        }

        public MotorCycleBuilder WithParkingSuburbFromAddress(Address address)
        {
            Set(x => x.ParkingAddress, address);
            return this;
        }

        public MotorCycleBuilder WithIsModified(bool isModified)
        {
            Set(x => x.IsModified, isModified);
            return this;
        }

        public MotorCycleBuilder WithInsurer(string insurer)
        {
            Set(x => x.CurrentInsurer, insurer);
            return this;
        }

        public MotorCycleBuilder WithoutInsurer()
        {
            Set(x => x.CurrentInsurer, null);
            return this;
        }

        public MotorCycleBuilder WithRandomInsurer()
        {
            return WithInsurer(GetRandomInsurer());
        }

        public MotorCycleBuilder WithDashcam(bool hasDashcam)
        {
            Set(x => x.HasDashcam, hasDashcam);
            return this;
        }

        public MotorCycleBuilder WithRandomDashcam()
        {
            var randValue = Randomiser.Get.Next(2) == 0 ? false : true;

            return WithDashcam(randValue);
        }

        public MotorCycleBuilder WithIsGaraged(bool isGaraged)
        {
            Set(x => x.IsGaraged, isGaraged);
            return this;
        }

        public MotorCycleBuilder WithIsPremiumChangeExpected(bool isPremiumChangeExpected)
        {
            Set(x => x.IsPremiumChangeExpected, isPremiumChangeExpected);
            return this;
        }

        public MotorCycleBuilder WithRandomIsGaraged()
        {
            var randValue = Randomiser.Get.Next(2) == 0 ? false : true;

            return WithIsGaraged(randValue);
        }

        public MotorCycleBuilder WithFinancier(string financier)
        {
            Set(x => x.Financier, financier);
            return this;
        }

        public MotorCycleBuilder WithoutFinancier()
        {
            Set(x => x.Financier, null);
            return this;
        }

        public MotorCycleBuilder WithRandomFinancier()
        {
             return WithFinancier(FinancierOptions.OrderBy(t => Guid.NewGuid()).First());
        }

        public MotorCycleBuilder WithRego(string rego)
        {
            Set(x => x.Registration, rego);
            return this;
        }

        public MotorCycleBuilder WithoutRego()
        {
            Set(x => x.Registration, null);
            return this;
        }

        public MotorCycleBuilder WithRandomRego()
        {
            return WithRego(DataHelper.RandomAlphanumerics(5, 10));
        }

        public MotorCycleBuilder WithVehicle(string make, decimal year, string model, string engine, string vehicleid)
        {
            Set(x => x.Make, make);
            Set(x => x.Year, year);
            Set(x => x.Model, model);
            Set(x => x.EngineCC, engine);
            Set(x => x.VehicleId, vehicleid);

            return this;
        }

        public MotorCycleBuilder WithRandomVehicle(int minValue = 500, int maxValue = 200000)
        {
            var vehicle = GetRandomVehicle(minMarketValue: minValue, maxMarketValue: maxValue);
            return WithVehicle(make: vehicle.Make,
                               year: vehicle.Year,
                               model: vehicle.Model,
                               engine: vehicle.EngineCC,
                               vehicleid: vehicle.VehicleId);
        }

        public MotorCycleBuilder WithoutDrivers()
        {
            Set(x => x.Drivers, null);
            return this;
        }

        public MotorCycleBuilder WithDriver(Contact contact, bool isPolicyHolder)
        {
            var drivers = GetOrDefault(x => x.Drivers, new List<Driver>()).ToList();
            var driver = new Driver()
            {
                Details = contact,
                IsPolicyHolderDriver = isPolicyHolder,
                LicenseConvictions = null,
                HistoricalAccidents = null,
                LicenseTime = null
            };

            drivers.Add(driver);
            Set(x => x.Drivers, drivers);
            return this;
        }

        public MotorCycleBuilder WithMainDriverLicenseTime(string licenseTime)
        {
            var drivers = Get(x => x.Drivers);
            if (drivers == null || drivers.Count < 1)
            {
                Reporting.Error("No drivers have been added yet.");
            }

            Set(x => x.Drivers[0].LicenseTime, licenseTime);
            return this;
        }

        public MotorCycleBuilder WithRandomMainDriverLicenseTime()
        {
            var drivers = Get(x => x.Drivers);
            if (drivers == null || drivers.Count < 1)
            {
                Reporting.Error("No drivers have been added yet.");
            }

            var licenseTimeOptions = new Dictionary<int, string>()
            {
                {17, "0-1" },
                {18, "1-2" },
                {19, "2-3" },
                {20, "3+" }
            };

            var randomLicenseTime = licenseTimeOptions.OrderBy(t => Guid.NewGuid()).First();
            var finalLicenseTime = randomLicenseTime.Value;
            var mainDriverAge = drivers[0].Details.GetContactAge();

            if (mainDriverAge < 17)
            {
                finalLicenseTime = licenseTimeOptions[17];
            }
            else if (mainDriverAge < randomLicenseTime.Key)
            {
                finalLicenseTime = licenseTimeOptions[mainDriverAge];
            }

            drivers[0].LicenseTime = finalLicenseTime;

            Set(x => x.Drivers, drivers);
            return this;
        }

        public MotorCycleBuilder WithDriverConvictions(int index, List<Declaration> convictions)
        {
            var drivers = Get(x => x.Drivers);
            if (drivers == null || drivers.Count < (index + 1))
            {
                Reporting.Error("No drivers have been added yet, or index out of range.");
            }

            drivers[index].LicenseConvictions = convictions;
            Set(x => x.Drivers, drivers);
            return this;
        }

        public MotorCycleBuilder WithRandomDriverConvictions(int index)
        {
            var drivers = Get(x => x.Drivers);
            if (drivers == null || drivers.Count < (index + 1))
            {
                Reporting.Error("No drivers have been added yet, or index out of range.");
            }

            var convictions = new List<Declaration>();
            var count = Randomiser.Get.Next(1, 4);
            for (int i = 0; i < count; i++)
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

            drivers[index].LicenseConvictions = convictions;
            Set(x => x.Drivers, drivers);
            return this;
        }

        public MotorCycleBuilder WithDriverAccidentHistory(int index, List<Declaration> history)
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

        public MotorCycleBuilder WithRandomDriverAccidentHistory(int index)
        {
            var drivers = Get(x => x.Drivers);
            if (drivers == null || drivers.Count < (index + 1))
            {
                Reporting.Error("No drivers have been added yet, or index out of range.");
            }

            var history = new List<Declaration>();
            var count = Randomiser.Get.Next(1, 4);
            for (int i = 0; i < count; i++)
            {
                var accidentDate = DataHelper.RandomDateInPastXYears(3);

                var accident = new Declaration()
                {
                    Description = GetRandomAccidentText(),
                    InsurerAtTime = GetRandomInsurer(),
                    Month = accidentDate.ToString("MMM"),
                    Year = accidentDate.Year.ToString()
                };
                history.Add(accident);
            }

            drivers[index].HistoricalAccidents = history;
            Set(x => x.Drivers, drivers);
            return this;
        }

        public MotorCycleBuilder WithPaymentMethod(Payment paymentMethod)
        {
            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        public MotorCycleBuilder WithPayer(Contact contact)
        {
            var paymentMethod = Get(x => x.PayMethod);

            if (paymentMethod == null)
                paymentMethod = new Payment(contact);

            paymentMethod.Payer = contact;

            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        public MotorCycleBuilder WithRandomPaymentMethod()
        {
            var drivers = Get(x => x.Drivers);
            var payment = new Payment(drivers);

            Set(x => x.PayMethod, payment);
            return this;
        }

        public MotorCycleBuilder WithAnnualPaymentFrequency()
        {
            Set(x => x.PayMethod.PaymentFrequency, PaymentFrequency.Annual);
            return this;
        }

        public MotorCycleBuilder WithMonthlyPaymentFrequency()
        {
            Set(x => x.PayMethod.PaymentFrequency, PaymentFrequency.Monthly);
            return this;
        }

        public MotorCycleBuilder WithPolicyStartDate(DateTime date)
        {
            Set(x => x.StartDate, date);
            return this;
        }

        public MotorCycleBuilder WithRandomPolicyStartDate()
        {
            return WithPolicyStartDate(DateTime.Now.AddDays(DataHelper.RandomNumber(0, 30)));
        }

        /// <summary>
        /// This sets the excess to "null" which tells the test to accept
        /// whatever Shield defaults the quote to based on rating.
        /// </summary>
        /// <returns></returns>
        public MotorCycleBuilder WithDefaultExcess()
        {
            Set(x => x.Excess, null);
            return this;
        }

        public MotorCycleBuilder WithExcess(string excess)
        {
            Set(x => x.Excess, excess);
            return this;
        }

        public MotorCycleBuilder WithDefaultSumInsured()
        {
            Set(x => x.InsuredVariance, 0);
            return this;
        }

        /// <summary>
        /// For sum insured (as code doesn't know the market value), then
        /// test defines a percentage variance from market value which is
        /// retrieved during runtime. The variance is defined as a
        /// +/- variation. e.g. "-10" is 10% under, and "10" is 10% over.
        /// </summary>
        /// <param name="percentageVariance"></param>
        /// <returns></returns>
        public MotorCycleBuilder WithSumInsuredVariance(int percentageVariance)
        {
            Set(x => x.InsuredVariance, percentageVariance);
            return this;
        }

        public MotorCycleBuilder WithCover(MotorCovers cover)
        {
            Set(x => x.CoverType, cover);
            return this;
        }

        public MotorCycleBuilder InitialiseMotorCycleQuoteWithRandomData(Contact mainDriver, bool isPolicyHolder)
        {
            var MotorCycleBuilder = WithRandomUsage()
                .WithRandomAnnualKms()
                .WithIsModified(false)
                .WithRandomInsurer()
                .WithRandomFinancier()
                .WithRandomVehicle()
                .WithImmobiliser()
                .WithTracker()
                .WithRandomDashcam()
                .WithRandomIsGaraged()
                .WithRandomRego()
                .WithDriver(mainDriver, isPolicyHolder)
                .WithParkingSuburbFromAddress(mainDriver.MailingAddress)
                .WithRandomMainDriverLicenseTime()
                .WithRandomPaymentMethod()
                .WithRandomPolicyStartDate()
                .WithDefaultExcess()
                .WithDefaultSumInsured()
                .WithIsPremiumChangeExpected(false)
                .WithCover(MotorCovers.MFCO);

            return MotorCycleBuilder;
        }

        protected override QuoteMotorcycle BuildEntity()
        {
            return new QuoteMotorcycle
            {
                UsageType      = GetOrDefault(x => x.UsageType),
                AnnualKm       = GetOrDefault(x => x.AnnualKm),
                ParkingAddress = GetOrDefault(x => x.ParkingAddress),
                CurrentInsurer = GetOrDefault(x => x.CurrentInsurer),
                Financier      = GetOrDefault(x => x.Financier),
                Drivers        = GetOrDefault(x => x.Drivers),
                PayMethod      = GetOrDefault(x => x.PayMethod),
                IsModified     = GetOrDefault(x => x.IsModified),
                IsGaraged      = GetOrDefault(x => x.IsGaraged),
                HasDashcam     = GetOrDefault(x => x.HasDashcam),
                HasImmobiliser = GetOrDefault(x => x.HasImmobiliser),
                HasTracker     = GetOrDefault(x => x.HasTracker),
                InsuredVariance = GetOrDefault(x => x.InsuredVariance),
                Make           = GetOrDefault(x => x.Make),
                Year           = GetOrDefault(x => x.Year),
                Model          = GetOrDefault(x => x.Model),
                EngineCC       = GetOrDefault(x => x.EngineCC),
                VehicleId      = GetOrDefault(x => x.VehicleId),
                Registration   = GetOrDefault(x => x.Registration),
                Excess         = GetOrDefault(x => x.Excess),
                StartDate      = GetOrDefault(x => x.StartDate),
                CoverType      = GetOrDefault(x => x.CoverType),
                IsPremiumChangeExpected = GetOrDefault(x => x.IsPremiumChangeExpected)
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

        /// <summary>
        /// Method to assist in getting a random motorcycle vehicle from Shield.
        /// </summary>
        /// <param name="testConfig">The test config with the details for connecting to Shield via the API.</param>
        /// <param name="minMarketValue"></param>
        /// <param name="maxMarketValue"></param>
        /// <returns></returns>
        public static Motorcycle GetRandomVehicle(int minMarketValue = 500, int maxMarketValue = 200000)
        {
            var randomVehicle = DataHelper.GetRandomInsurableMotorcycle(minMarketValue, maxMarketValue);

            return new Motorcycle()
            {
                Make        = randomVehicle.MakeDescription.Trim(),
                Year        = randomVehicle.ModelYear,
                Model       = randomVehicle.ModelDescription.Trim(),
                EngineCC    = randomVehicle.ModelFamily.Trim(),
                VehicleId   = randomVehicle.VehicleId,
                MarketValue = randomVehicle.Price
            };            
        }
    }
}
