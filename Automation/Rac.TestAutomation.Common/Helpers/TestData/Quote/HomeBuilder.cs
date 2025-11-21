using System;
using System.Collections.Generic;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace Rac.TestAutomation.Common.TestData.Quote
{
    public class HomeBuilder : EntityBuilder<QuoteHome, HomeBuilder>
    {
        public HomeBuilder() { }

        public HomeBuilder WithOccupancy(HomeOccupancy occupancy)
        {
            Set(x => x.Occupancy, occupancy);
            return this;
        }

        public HomeBuilder WithIsUsedForShortStay(bool isUsedForShortStay)
        {
            Set(x => x.IsUsedForShortStay, isUsedForShortStay);
            return this;
        }

        public HomeBuilder WithBuildingType(HomeType typeOfBuilding)
        {
            Set(x => x.TypeOfBuilding, typeOfBuilding);
            return this;
        }

        public HomeBuilder WithRandomBuildingType() => WithBuildingType(DataHelper.GetRandomEnum<HomeType>(startIndex: (int)HomeType.House));

        public HomeBuilder WithWallMaterial(HomeMaterial wallMaterial)
        {
            Set(x => x.WallMaterial, wallMaterial);
            return this;
        }

        public HomeBuilder WithRandomWallMaterial() => WithWallMaterial(DataHelper.GetRandomEnum<HomeMaterial>(startIndex: 2));

        public HomeBuilder WithRandomYearBuilt()
        {
            return WithYearBuilt(Randomiser.Get.Next(1901, DateTime.Now.Year + 1));
        }

        /// <summary>
        /// Random year between 1901 and given year. Upper limit is non-inclusive.
        /// </summary>
        public HomeBuilder WithRandomYearBuiltBeforeGivenYear(int upperYearLimit)
        {
            return WithYearBuilt(Randomiser.Get.Next(1901, upperYearLimit));
        }

        public HomeBuilder WithYearBuilt(int year)
        {
            var cycloneFlag = GetOrDefault(x => x.IsACycloneAddress, false);
            if (cycloneFlag) { throw new DataMisalignedException("You cannot change year built if you have already marked as cyclone risk due to conditional questions."); }

            Set(x => x.YearBuilt, year);
            return this;
        }

        public HomeBuilder WithPropertyAddress(Address insuredPropertyAddress)
        {
            Set(x => x.PropertyAddress, insuredPropertyAddress);
            return this;
        }

        public HomeBuilder WithRandomPropertyAddress()
        {
            var address = new AddressBuilder().InitialiseRandomMailingAddress().Build();
            return WithPropertyAddress(address);
        }

        public HomeBuilder WithWeeklyRentalRate(int amount)
        {
            Set(x => x.WeeklyRental, amount);
            return this;
        }

        public HomeBuilder WithRandomRentalRate()
        {
            return WithWeeklyRentalRate(DataHelper.RandomNumber(HOME_RENTAL_MINIMUM, HOME_RENTAL_MAXIMUM));
        }

        public HomeBuilder WithPropertyManager(HomePropertyManager propertyManager)
        {
            Set(x => x.PropertyManager, propertyManager);
            return this;
        }

        public HomeBuilder WithRandomPropertyManager()
        {
            return WithPropertyManager(DataHelper.GetRandomEnum<HomePropertyManager>(startIndex: 1));
        }

        public HomeBuilder WithAreDoorsSecured(bool areDoorsSecured)
        {
            Set(x => x.SecurityDoorsSecured, areDoorsSecured);
            return this;
        }

        public HomeBuilder WithAreWindowsSecured(bool areWindowsSecured)
        {
            Set(x => x.SecurityWindowsSecured, areWindowsSecured);
            return this;
        }

        /// <summary>
        /// This method does not check that the given property address is actually
        /// marked as a cyclone risk in Shield. It trusts that the calling test
        /// knows. It sets "IsACycloneAddress" to true, and takes the answers
        /// to the four related Cyclone questions.
        /// </summary>
        /// <param name="isElevated">boolean for if the property is elevated. Mandatory cyclone question.</param>
        /// <param name="hasShutters">boolean for if the property has cyclone shutters. Mandatory cyclone question.</param>
        /// <param name="garageDoorStatus">Dropdown answer for whether the garage door has been upgraded. Only relevant for properties built before 2012.</param>
        /// <param name="roofStatus">Dropdown answer for any roof upgrades. Only relevant for properties built before 1982.</param>
        public HomeBuilder WithCycloneResponses(bool isElevated, bool hasShutters,
                                                 GarageDoorsUpgradeStatus garageDoorStatus,
                                                 RoofImprovementStatus roofStatus)
        {
            var yearBuilt = GetOrDefault(x => x.YearBuilt);
            Set(x => x.IsACycloneAddress, true);
            Set(x => x.IsPropertyElevated, isElevated);
            Set(x => x.IsCycloneShuttersFitted, hasShutters);

            // This can only be answered by the test if year built is before 2012
            Set(x => x.GarageDoorsCycloneStatus, yearBuilt < 2012 ? garageDoorStatus : GarageDoorsUpgradeStatus.ReplacedToCyclone);

            // This can only be answered by the test if year built is before 1982
            Set(x => x.RoofImprovementCycloneStatus, yearBuilt < 1982 ? roofStatus : RoofImprovementStatus.CompleteRoofReplacement);
            return this;
        }

        public HomeBuilder WithAlarmSystem(Alarm alarm)
        {
            Set(x => x.AlarmSystem, alarm);
            return this;
        }

        public HomeBuilder WithRandomAlarmSystem() => WithAlarmSystem(DataHelper.GetRandomEnum<Alarm>(startIndex: 1));

        /// <summary>
        /// Sets data so that the answer will be NO for both
        /// past convictions as well as past claims.
        /// </summary>
        /// <returns></returns>
        public HomeBuilder WithOutDisclosures()
        {
            Set(x => x.HasPastConvictions, false);
            Set(x => x.PastClaims, null);
            return this;
        }

        public HomeBuilder WithPolicyholderHasPastConvictions()
        {
            Set(x => x.HasPastConvictions, true);
            return this;
        }

        /// <summary>
        /// Will generate the requested number of historical claims to
        /// be disclosed in the home quote. The claim types are all
        /// randomly chosen and may repeat a claim type, because there
        /// should be no business rules around specific claim types.
        /// 
        /// Any previous claims history will be thrown away.
        /// </summary>
        /// <param name="desiredNumber">The number of historical claims to add</param>
        /// <returns></returns>
        public HomeBuilder WithRandomClaimsHistory(int desiredNumber)
        {
            var history = new List<ClaimHistory>();

            for(int i = 0; i < desiredNumber; i++)
            {
                history.Add(new ClaimHistory()
                {
                    ClaimType = DataHelper.GetRandomEnum<ClaimsHistory>(),
                    Year = DataHelper.RandomDateInPastXYears(3).Year
                });

            }

            Set(x => x.PastClaims, history);
            return this;
        }

        /// <summary>
        /// To define a non-zero insurance history with a randomised insurer name
        /// pass NULL for the insurerName.
        /// </summary>
        /// <param name="previousInsuranceYears"></param>
        /// <param name="insurerName"></param>
        /// <returns></returns>
        public HomeBuilder WithInsuranceHistory(HomePreviousInsuranceTime previousInsuranceYears, string insurerName = null)
        {
            Set(x => x.PreviousInsuranceTime, previousInsuranceYears);
            if (previousInsuranceYears != HomePreviousInsuranceTime.Zero &&
                string.IsNullOrEmpty(insurerName))
            {
                insurerName = GetRandomInsurer();
            }
            Set(x => x.CurrentInsurer, insurerName);
            return this;
        }

        public HomeBuilder WithRandomInsuranceHistory()
        {
            Array values = Enum.GetValues(typeof(HomePreviousInsuranceTime));
            HomePreviousInsuranceTime previousInsuranceTime = (HomePreviousInsuranceTime)values.GetValue(Randomiser.Get.Next(values.Length));
            return WithInsuranceHistory(previousInsuranceTime, null);
        }

        /// <summary>
        /// This effectively removes building cover from the quote.
        /// </summary>
        /// <returns></returns>
        public HomeBuilder WithoutBuildingSumInsured()
        {
            return WithBuildingSumInsured(null);
        }

        public HomeBuilder WithBuildingSumInsured(int? sumInsured)
        {
            Set(x => x.BuildingValue, sumInsured);
            return this;
        }

        public HomeBuilder WithRandomBuildingSumInsured(int minValue = HOME_BUILDING_SI_MIN, int maxValue = HOME_BUILDING_SI_MAX)
        {
            return WithBuildingSumInsured(DataHelper.RandomNumber(minValue, maxValue));
        }

        /// <summary>
        /// This effectively removes contents cover from the quote.
        /// </summary>
        /// <returns></returns>
        public HomeBuilder WithoutContentsSumInsured()
        {
            return WithContentsSumInsured(null);
        }

        public HomeBuilder WithContentsSumInsured(int? sumInsured)
        {
            Set(x => x.ContentsValue, sumInsured);
            return this;
        }

        public HomeBuilder WithRandomContentsSumInsured(int minValue = HOME_CONTENTS_SI_MIN, int maxValue = HOME_CONTENTS_SI_MAX)
        {
            return WithContentsSumInsured(DataHelper.RandomNumber(minValue, maxValue));
        }

        public HomeBuilder WithRandomRentersContentsSumInsured()
        {
            var rentersContentsOptions = new[]
            {
                10000,
                15000,
                20000,
                25000
            };

            return WithContentsSumInsured(rentersContentsOptions.OrderBy(t => Guid.NewGuid()).First());
        }

        public HomeBuilder WithoutUnspecifiedPersonalValuableCover()
        {
            return WithUnspecifiedPersonalValuablesCover(UnspecifiedPersonalValuables.None);
        }

        public HomeBuilder WithUnspecifiedPersonalValuablesCover(UnspecifiedPersonalValuables amount)
        {
            Set(x => x.UnspecifiedValuablesInsuredAmount, amount);
            return this;
        }

        public HomeBuilder WithRandomUnspecifiedPersonalValuablesCover() => WithUnspecifiedPersonalValuablesCover(DataHelper.GetRandomEnum<UnspecifiedPersonalValuables>(startIndex: 1));

        public HomeBuilder AddSpecifiedPersonalValuable(SpecifiedValuables typeOfValuable, string description, int value)
        {
            var valuables = GetOrDefault(x => x.SpecifiedValuablesOutside);

            if (valuables == null)
                valuables = new List<ContentItem>();

            valuables.Add(new ContentItem()
            {
                Category    = (int)typeOfValuable,
                Description = description,
                Value       = value
            });

            Set(x => x.SpecifiedValuablesOutside, valuables);
            return this;
        }

        public HomeBuilder AddSpecifiedContentsItem(SpecifiedContents typeOfValuable, string description, int value)
        {
            var valuables = GetOrDefault(x => x.SpecifiedValuablesInside);

            if (valuables == null)
                valuables = new List<ContentItem>();

            valuables.Add(new ContentItem()
            {
                Category    = (int)typeOfValuable,
                Description = description,
                Value       = value
            });

            Set(x => x.SpecifiedValuablesInside, valuables);
            return this;
        }

        public HomeBuilder WithFinancier(string financier)
        {
            Set(x => x.Financier, financier);
            return this;
        }

        public HomeBuilder WithoutFinancier()
        {
            Set(x => x.Financier, null);
            return this;
        }

        public HomeBuilder WithRandomFinancier()
        {
            return WithFinancier(FinancierOptions.OrderBy(t => Guid.NewGuid()).First());
        }

        public HomeBuilder IsHomeUsageUnacceptable(bool isUnacceptable)
        {
            Set(x => x.IsHomeUsageUnacceptable, isUnacceptable);
            return this;
        }

        public HomeBuilder WithPolicyHolders(List<Contact> policyHolders)
        {
            Set(x => x.PolicyHolders, policyHolders);
            return this;
        }

        public HomeBuilder WithPaymentMethod(Payment paymentMethod)
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
        public HomeBuilder WithPayer(Contact contact)
        {
            Set(x => x.PayMethod, new Payment(contact));
            return this;
        }

        public HomeBuilder WithRandomPaymentMethod()
        {
            var policyHolders = GetOrDefault(x => x.PolicyHolders);

            Set(x => x.PayMethod, new Payment(policyHolders));
            return this;
        }

        /// <summary>
        /// Overrides any pre-existing payment method. Selects a random
        /// policyholder from the current known list, and sets a
        /// random payment frequency for the provided payment method.
        /// </summary>
        /// <param name="isBankAccount"></param>
        /// <returns></returns>
        public HomeBuilder WithPaymentMethod(bool isBankAccount)
        {
            var policyHolders = GetOrDefault(x => x.PolicyHolders);

            var newPaymentMethod = new Payment(policyHolders);
            newPaymentMethod.IsPaymentByBankAccount = isBankAccount;

            Set(x => x.PayMethod, newPaymentMethod);
            return this;
        }

        public HomeBuilder WithPolicyStartDate(DateTime date)
        {
            Set(x => x.StartDate, date);
            return this;
        }

        /// <summary>
        /// This sets the excess to "null" which tells the test to accept
        /// whatever Shield defaults the quote to based on rating.
        /// </summary>
        /// <returns></returns>
        public HomeBuilder WithDefaultExcesses()
        {
            return WithExcess(null, null);
        }

        public HomeBuilder WithExcess(string buildingExcess, string contentsExcess)
        {
            Set(x => x.ExcessBuilding, buildingExcess);
            Set(x => x.ExcessContents, contentsExcess);
            return this;
        }

        public HomeBuilder WithPurchaseAccidentalDamageBundle(bool addAccidentalDamage)
        {
            Set(x => x.AddAccidentalDamage, addAccidentalDamage);
            return this;
        }

        public HomeBuilder InitialiseHomeWithRandomData(HomeOccupancy occupancy, List<Contact> policyHolders)
        {
            var quoteHomeBuilder = WithOccupancy(occupancy)
                .WithRandomPropertyAddress()
                .WithPolicyHolders(policyHolders)
                .WithRandomBuildingSumInsured()
                .WithRandomContentsSumInsured()
                .WithDefaultExcesses()
                .WithRandomBuildingType()
                .WithRandomWallMaterial()
                .WithRandomYearBuilt()
                .WithRandomAlarmSystem()
                .WithAreDoorsSecured(true)
                .WithAreWindowsSecured(true)
                .WithIsUsedForShortStay(false)
                .IsHomeUsageUnacceptable(false)
                .WithRandomInsuranceHistory()
                .WithoutUnspecifiedPersonalValuableCover()
                .WithPolicyStartDate(DateTime.Now)
                .WithRandomFinancier()
                .WithRandomPaymentMethod()
                .WithOutDisclosures()
                .WithPurchaseAccidentalDamageBundle(false);

            return quoteHomeBuilder;
        }

        protected override QuoteHome BuildEntity()
        {
            return new QuoteHome
            {
                Occupancy = GetOrDefault(x => x.Occupancy),
                IsUsedForShortStay = GetOrDefault(x => x.IsUsedForShortStay),
                TypeOfBuilding = GetOrDefault(x => x.TypeOfBuilding),
                WallMaterial = GetOrDefault(x => x.WallMaterial),
                RoofMaterial = GetOrDefault(x => x.RoofMaterial),
                YearBuilt    = GetOrDefault(x => x.YearBuilt),
                PropertyAddress = GetOrDefault(x => x.PropertyAddress),
                WeeklyRental = GetOrDefault(x => x.WeeklyRental),
                PropertyManager = GetOrDefault(x => x.PropertyManager),
                SecurityWindowsSecured = GetOrDefault(x => x.SecurityWindowsSecured),
                SecurityDoorsSecured   = GetOrDefault(x => x.SecurityDoorsSecured),
                AlarmSystem    = GetOrDefault(x => x.AlarmSystem),
                IsHomeUsageUnacceptable = GetOrDefault(x => x.IsHomeUsageUnacceptable),
                PreviousInsuranceTime   = GetOrDefault(x => x.PreviousInsuranceTime),
                CurrentInsurer = GetOrDefault(x => x.CurrentInsurer),
                BuildingValue  = GetOrDefault(x => x.BuildingValue),
                ContentsValue  = GetOrDefault(x => x.ContentsValue),
                ExcessBuilding = GetOrDefault(x => x.ExcessBuilding),
                ExcessContents = GetOrDefault(x => x.ExcessContents),
                SpecifiedValuablesInside  = GetOrDefault(x => x.SpecifiedValuablesInside),
                SpecifiedValuablesOutside = GetOrDefault(x => x.SpecifiedValuablesOutside),
                UnspecifiedValuablesInsuredAmount = GetOrDefault(x => x.UnspecifiedValuablesInsuredAmount),
                Financier = GetOrDefault(x => x.Financier),
                PayMethod = GetOrDefault(x => x.PayMethod),
                StartDate = GetOrDefault(x => x.StartDate),
                PolicyHolders      = GetOrDefault(x => x.PolicyHolders),
                HasPastConvictions = GetOrDefault(x => x.HasPastConvictions),
                PastClaims         = GetOrDefault(x => x.PastClaims),
                AddAccidentalDamage = GetOrDefault(x => x.AddAccidentalDamage),
                IsACycloneAddress  = GetOrDefault(x => x.IsACycloneAddress),
                IsPropertyElevated = GetOrDefault(x => x.IsPropertyElevated),
                IsCycloneShuttersFitted = GetOrDefault(x => x.IsCycloneShuttersFitted),
                GarageDoorsCycloneStatus = GetOrDefault(x => x.GarageDoorsCycloneStatus),
                RoofImprovementCycloneStatus = GetOrDefault(x => x.RoofImprovementCycloneStatus)
            };
        }

        private string GetRandomInsurer()
        {
            var insurerOptions = new[]
            {
                "AAMI",
                "ALLIANZ",
                "APIA",
                "BINGLE",
                "BUDGET DIRECT",
                "CGU",
                "COLES",
                "COMMINSURE",
                "GIO",
                "HBF",
                "MEDIBANK",
                "NRMA",
                "PROGRESSIVE",
                "QBE",
                "RAC WA",
                "RACQ",
                "RACV",
                "REAL",
                "SGIO",
                "SHANNONS",
                "ST GEORGE",
                "SUNCORP",
                "WOOLWORTHS",
                "YOUI",
                "OTHER"
            };

            return insurerOptions.OrderBy(t => Guid.NewGuid()).First();
        }
    }
}