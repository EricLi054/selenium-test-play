using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;

namespace Rac.TestAutomation.Common.TestData.Quote
{
    public class CaravanBuilder : EntityBuilder<QuoteCaravan, CaravanBuilder>
    {
        [SuppressMessage("SonarLint", "S4487", Justification = "Safely unused variable, follows pattern of other ProductBuilders")]
        private Config _config;

        public CaravanBuilder() 
        { 
            _config = Config.Get(); 
        }

        public CaravanBuilder InitialiseCaravanWithRandomData(List<Contact> policyHolders)
        {
            var CaravanBuilder = WithType(CaravanType.Caravan)
                .WithRandomCaravan()
                .WithUseOfBusinessOrCommercial(false)
                .WithParkingSuburbFromAddress(policyHolders[0].MailingAddress)
                .WithRandomParkLocation(policyHolders[0].MailingAddress.PostCode)
                .WithRandomFinancier()
                .WithPolicyHolders(policyHolders)
                .WithPolicyStartDate(DateTime.Now)
                .WithDefaultExcess()
                .WithDefaultSumInsuredValue()
                .WithDefaultContentCoverValue()
                .WithRandomPaymentMethod();

            return CaravanBuilder;
        }

        public CaravanBuilder WithParkingSuburb(string suburb, string postcode)
        {
            var address = new Address()
            {
                Suburb = suburb,
                PostCode = postcode
            };
            return WithParkingSuburbFromAddress(address);
        }

        public CaravanBuilder WithParkingSuburbFromAddress(Address address)
        {
            Set(x => x.ParkingAddress, address);
            return this;
        }

        public CaravanBuilder WithParkLocation(CaravanParkLocation parkLocation)
        {
            Set(x => x.ParkLocation, parkLocation);
            return this;
        }

        public CaravanBuilder WithType(CaravanType type)
        {
            Set(x => x.Type, type);
            return this;
        }

        public CaravanBuilder WithUseOfBusinessOrCommercial(bool isUsedByBusiness)
        {
            Set(x => x.IsForBusinessOrCommercialUse, isUsedByBusiness);
            return this;
        }

        public CaravanBuilder WithoutFinancier()
        {
            Set(x => x.Financier, null);
            return this;
        }

        public CaravanBuilder WithRandomFinancier()
        {
            Set(x => x.Financier, FinancierOptions.OrderBy(t => Guid.NewGuid()).First());
            return this;
        }

        /// <summary>
        /// Randomly selects a parking location for the caravan (garage,
        /// driveway, on-site, etc). Allows caller to exclude on-site
        /// as there are some rating variations that are different for
        /// that case. So tests checking age-related rating, may want to
        /// avoid on-site.
        /// </summary>
        /// <param name="postcode"></param>
        /// <param name="excludeOnSite">If TRUE, then will not pick on-site</param>
        /// <returns></returns>
        public CaravanBuilder WithRandomParkLocation(string postcode, bool excludeOnSite = false)
        {
            CaravanParkLocation parkLocation = DataHelper.GetRandomEnum<CaravanParkLocation>();

            if (excludeOnSite && parkLocation == CaravanParkLocation.OnSite)
            {
                parkLocation = CaravanParkLocation.Carport;
            }

            //Update 'Parking Location' to 'Garage', if the 'Parking Suburb' is above the 26th Parallel.
            //This is done to avoid user getting knocked out.
            if (DataHelper.IsPostcodeAbove26thParallel(postcode))
                parkLocation = CaravanParkLocation.Garage;
            Set(x => x.ParkLocation, parkLocation);
            return this;
        }

        public CaravanBuilder WithPolicyHolders(List<Contact> policyHolders)
        {
            Set(x => x.PolicyHolders, policyHolders);
            return this;
        }

        public CaravanBuilder WithPaymentMethod(Payment paymentMethod)
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
        public CaravanBuilder WithPayer(Contact contact)
        {
            Set(x => x.PayMethod, new Payment(contact));
            return this;
        }

        public CaravanBuilder WithRandomPaymentMethod()
        {
            Set(x => x.PayMethod, new Payment(Get(x => x.PolicyHolders)));
            return this;
        }

        public CaravanBuilder WithPolicyStartDate(DateTime date)
        {
            Set(x => x.StartDate, date);
            return this;
        }

        /// <summary>
        /// This sets the excess to "null" which tells the test to accept
        /// whatever Shield defaults the quote to based on rating.
        /// </summary>
        /// <returns></returns>
        public CaravanBuilder WithDefaultExcess()
        {
            Set(x => x.Excess, null);
            return this;
        }

        public CaravanBuilder WithExcess(int? excess)
        {
            Set(x => x.Excess, excess);
            return this;
        }

        public CaravanBuilder WithInsuredVariance(int insuredVariance)
        {
            Set(x => x.InsuredVariance, insuredVariance);
            return this;
        }

        //If SumInsuredValue is 0, then in the subsequent caravan pages,
        //use the sum insured value provided by the application.
        public CaravanBuilder WithDefaultSumInsuredValue()
        {
            Set(x => x.SumInsuredValue, 0);
            return this;
        }

        public CaravanBuilder WithAgreedSumInsured(int agreedSumInsured)
        {
            Set(x => x.SumInsuredValue, agreedSumInsured);
            return this;
        }

        public CaravanBuilder WithDefaultContentCoverValue()
        {
            Set(x => x.ContentsSumInsured, CARAVAN_DEFAULT_CONTENT_INSURANCE_VALUE);
            return this;
        }

        public CaravanBuilder WithContentsCoverValue(int contentsCoverValue)
        {
            Set(x => x.ContentsSumInsured, contentsCoverValue);
            return this;
        }

        public CaravanBuilder WithRetrieveQuote(RetrieveQuoteType retrieveQuoteType)
        {
            Set(x => x.RetrieveQuote, retrieveQuoteType);
            return this;
        }

        protected override QuoteCaravan BuildEntity()
        {
            return new QuoteCaravan
            {
                Type = GetOrDefault(x => x.Type),
                Make = GetOrDefault(x => x.Make),
                Year = GetOrDefault(x => x.Year),
                Model = GetOrDefault(x => x.Model),
                MarketValue = GetOrDefault(x => x.MarketValue),
                IsForBusinessOrCommercialUse = GetOrDefault(x => x.IsForBusinessOrCommercialUse),
                ParkingAddress = GetOrDefault(x => x.ParkingAddress),
                PolicyHolders = GetOrDefault(x => x.PolicyHolders),
                StartDate = GetOrDefault(x => x.StartDate),
                CoverMyAnnexe = GetOrDefault(x => x.CoverMyAnnexe),
                Excess = GetOrDefault(x => x.Excess),
                InsuredVariance = GetOrDefault(x => x.InsuredVariance),
                SumInsuredValue = GetOrDefault(x => x.SumInsuredValue),
                ContentsSumInsured = GetOrDefault(x => x.ContentsSumInsured),
                PayMethod = GetOrDefault(x => x.PayMethod),
                VehicleId = GetOrDefault(x => x.VehicleId),
                Financier = GetOrDefault(x => x.Financier),
                ParkLocation = GetOrDefault(x => x.ParkLocation),
                RetrieveQuote = GetOrDefault(x => x.RetrieveQuote)
            };
        }

        public CaravanBuilder WithRandomCaravan(int minValue = CARAVAN_MIN_SUM_INSURED_VALUE, int maxValue = CARAVAN_MAX_SUM_INSURED_VALUE)
        {
            var caravan = GetRandomCaravan(minMarketValue: minValue, maxMarketValue: maxValue);
            return WithCaravan(make: caravan.Make,
                               year: caravan.Year,
                               model: caravan.Model,
                               modelDescription: caravan.ModelDescription,
                               marketValue: caravan.MarketValue,
                               vehicleid: caravan.VehicleId);
        }

        public CaravanBuilder WithCaravan(string make, decimal year, string model, string modelDescription, int marketValue, string vehicleid)
        {
            Set(x => x.Make, make);
            Set(x => x.Year, year);
            Set(x => x.Model, model);
            Set(x => x.ModelDescription, modelDescription);
            Set(x => x.MarketValue, marketValue);
            Set(x => x.VehicleId, vehicleid);
            return this;
        }

        /// <summary>
        /// Method to assist in getting a random caravan from Shield.
        /// </summary>
        /// <param name="testConfig">The test config with the details for connecting to Shield via the API.</param>
        /// <param name="minMarketValue"></param>User is required to manually enter the value of caravan, if its market value is less than $1000
        /// <param name="maxMarketValue"></param>
        /// <returns></returns>
        public static Caravan GetRandomCaravan(int minMarketValue, int maxMarketValue)
        {
            var randomCaravan = DataHelper.GetRandomInsurableCaravan(minMarketValue, maxMarketValue);
            if (randomCaravan.VehicleId==null)
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
    }
}
