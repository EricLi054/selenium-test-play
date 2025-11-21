using System;
using System.Collections.Generic;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace Rac.TestAutomation.Common.TestData.Endorsements
{
    public class HomeEndorsementBuilder : EntityBuilder<EndorseHome, HomeEndorsementBuilder>
    {
        /// <summary>
        /// Sets up core fields of the Endorsement data object based on the
        /// provided home policy number. Will default active policyholder
        /// to be the main policyholder, and payment method to be that
        /// member with the current payment method and frequency.
        /// </summary>
        private HomeEndorsementBuilder WithPolicy(string policyNumber, Contact policyHolder)
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
            activePH.UpdateEmailIfNotDefined();
            
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

            var newAssetDetails = new Home()
            {
                TypeOfBuilding = HomeType.Undefined,
                RoofMaterial = HomeRoof.Undefined,
                WallMaterial = HomeMaterial.Undefined,
                YearBuilt    = policyDetails.HomeAsset.ConstructionYear,
                SecurityWindowsSecured = policyDetails.HomeAsset.IsWindowSecurityApplied,
                SecurityDoorsSecured   = policyDetails.HomeAsset.IsDoorSecurityApplied,
                AlarmSystem  = Alarm.Undefined
            };
            if (policyDetails.HasBuildingCover())
            { newAssetDetails.BuildingValue = (int)Math.Round(policyDetails.HomeSumInsuredBuilding(), 0); }
            if (policyDetails.HasContentsCover())
            { newAssetDetails.ContentsValue = (int)Math.Round(policyDetails.HomeSumInsuredContents(), 0); }

            if (Config.Get().IsCycloneEnabled() && policyDetails.ProductVersionAsInteger >= HomeProductVersionCycloneReinsurance)
            {
                newAssetDetails.IsACycloneAddress = policyDetails.HomeAsset.IsCycloneProneArea;

                // For subsequent four questionnaire items, we will initialise a value if
                // a value is not already defined in the current policy (policyDetails.HomeAsset).
                // We choose random values as they don't affect the flow from the user's view.
                newAssetDetails.IsPropertyElevated = string.IsNullOrEmpty(policyDetails.HomeAsset.IsPropertyElevated) ?
                                                     DataHelper.RandomNumber(0, 2) == 1 :
                                                     policyDetails.HomeAsset.GetIsPropertyElevated;
                newAssetDetails.IsCycloneShuttersFitted = string.IsNullOrEmpty(policyDetails.HomeAsset.HasCycloneShutters) ?
                                                          DataHelper.RandomNumber(0, 2) == 1 :
                                                          policyDetails.HomeAsset.GetHasCycloneShutters;

                // For properties built 2012+, Shield will set this to replaced to cyclone standards.
                // Member is not prompted for input.
                if (policyDetails.HomeAsset.ConstructionYear >= 2012)
                { newAssetDetails.GarageDoorsCycloneStatus = GarageDoorsUpgradeStatus.ReplacedToCyclone; }
                else
                {
                    // If policy already has a value in existing policy, we'll use that,
                    // otherwise we use a randomised value in test.
                    newAssetDetails.GarageDoorsCycloneStatus = string.IsNullOrEmpty(policyDetails.HomeAsset.GarageDoorUpgraded) ?
                                                               DataHelper.GetRandomEnum<GarageDoorsUpgradeStatus>(startIndex: 1) :
                                                               policyDetails.HomeAsset.GetGarageDoorUpgradeStatus;
                }

                // For properties built 1982+, Shield will set this to complete roof replacement.
                // Member is not prompted for input.
                if (policyDetails.HomeAsset.ConstructionYear >= 1982)
                { newAssetDetails.RoofImprovementCycloneStatus = RoofImprovementStatus.NotSure; }
                else
                {
                    // If policy already has a value in existing policy, we'll use that,
                    // otherwise we use a randomised value in test.
                    newAssetDetails.RoofImprovementCycloneStatus = string.IsNullOrEmpty(policyDetails.HomeAsset.RoofImprovement) ?
                                                                   DataHelper.GetRandomEnum<RoofImprovementStatus>(startIndex: 1) :
                                                                   policyDetails.HomeAsset.GetRoofImprovementStatus;
                }
            }
            Set(x => x.NewAssetValues, newAssetDetails);

            return this;
        }

        /// <summary>
        /// Change the active policyholder (member that test will be logged in as)
        /// to the contact provided. Only necessary if test does not want to use
        /// default, which is primary policyholder.
        /// </summary>
        public HomeEndorsementBuilder WithActivePolicyHolder(Contact policyHolder)
        {
            Set(x => x.ActivePolicyHolder, policyHolder);
            return this;
        }

        public HomeEndorsementBuilder WithEndorsementStartDate(DateTime date)
        {
            Set(x => x.StartDate, date);
            return this;
        }

        /// <summary>
        /// With a change of address, the member will be prompted for
        /// new property rating values. Some items are not initialised
        /// here (such as rental and SI) as they are occupancy and
        /// test scenario specific.
        /// </summary>
        /// <param name="propertyAddress"></param>
        /// <returns></returns>
        public HomeEndorsementBuilder WithPropertyAddress(Address propertyAddress)
        {
            var home = Get(x => x.NewAssetValues);
            home.PropertyAddress= propertyAddress;
            Set(x => x.NewAssetValues, home);
            return this
                .WithRandomBuildingType()
                .WithRandomWallMaterial()
                .WithRandomYearBuilt()
                .WithRandomAlarmSystem()
                .WithoutFinancier()
                .WithWeeklyRentalRate(null)
                .WithDefaultExcesses()
                .WithoutContentsSumInsured()
                .WithoutBuildingSumInsured();
        }

        public HomeEndorsementBuilder WithRandomPropertyAddress()
        {
            var propertyAddress = new AddressBuilder().InitialiseRandomMailingAddress().Build();
            return WithPropertyAddress(propertyAddress);
        }

        public HomeEndorsementBuilder WithBuildingType(HomeType typeOfBuilding)
        {
            var home = Get(x => x.NewAssetValues);
            home.TypeOfBuilding = typeOfBuilding;
            Set(x => x.NewAssetValues, home);
            return this;
        }

        public HomeEndorsementBuilder WithRandomBuildingType() => WithBuildingType(DataHelper.GetRandomEnum<HomeType>(startIndex: (int)HomeType.House));

        public HomeEndorsementBuilder WithWallMaterial(HomeMaterial wallMaterial)
        {
            var home = Get(x => x.NewAssetValues);
            home.WallMaterial = wallMaterial;
            Set(x => x.NewAssetValues, home);
            return this;
        }

        public HomeEndorsementBuilder WithRandomWallMaterial() => WithWallMaterial(DataHelper.GetRandomEnum<HomeMaterial>(startIndex: 2));

        public HomeEndorsementBuilder WithYearBuilt(int yearBuilt)
        {
            var home = Get(x => x.NewAssetValues);
            home.YearBuilt = yearBuilt;
            Set(x => x.NewAssetValues, home);
            return this;
        }

        public HomeEndorsementBuilder WithRandomYearBuilt()
        {
            return WithYearBuilt(Randomiser.Get.Next(1901, DateTime.Now.Year));
        }

        public HomeEndorsementBuilder WithAreWindowsSecured(bool areWindowsSecured)
        {
            var home = Get(x => x.NewAssetValues);
            home.SecurityWindowsSecured = areWindowsSecured;
            Set(x => x.NewAssetValues, home);
            return this;
        }

        public HomeEndorsementBuilder WithAreDoorsSecured(bool areDoorsSecured)
        {
            var home = Get(x => x.NewAssetValues);
            home.SecurityDoorsSecured = areDoorsSecured;
            Set(x => x.NewAssetValues, home);
            return this;
        }

        public HomeEndorsementBuilder WithAlarmSystem(Alarm alarm)
        {
            var home = Get(x => x.NewAssetValues);
            home.AlarmSystem = alarm;
            Set(x => x.NewAssetValues, home);
            return this;
        }

        public HomeEndorsementBuilder WithRandomAlarmSystem() => WithAlarmSystem(DataHelper.GetRandomEnum<Alarm>(startIndex: 1));

        public HomeEndorsementBuilder WithFinancier(string financier)
        {
            Set(x => x.Financier, financier);
            return this;
        }

        public HomeEndorsementBuilder WithoutFinancier()
        {
            return WithFinancier(null);
        }

        public HomeEndorsementBuilder WithRandomFinancier()
        {
            return WithFinancier(FinancierOptions.OrderBy(t => Guid.NewGuid()).First());
        }

        public HomeEndorsementBuilder WithWeeklyRentalRate(string amount)
        {
            Set(x => x.WeeklyRent, amount);
            return this;
        }

        public HomeEndorsementBuilder WithRandomRentalRate()
        {
            return WithWeeklyRentalRate(DataHelper.RandomNumber(50, 5000).ToString());
        }

        public HomeEndorsementBuilder WithPropertyManager(HomePropertyManager propertyManager)
        {
            Set(x => x.HomePropertyManager, propertyManager);
            return this;
        }

        public HomeEndorsementBuilder WithRandomPropertyManager()
        {
            return WithPropertyManager(DataHelper.GetRandomEnum<HomePropertyManager>(startIndex: 1));
        }

        /// <summary>
        /// This sets the excess to "null" which tells the test to accept
        /// whatever Shield defaults the quote to based on rating.
        /// </summary>
        /// <returns></returns>
        public HomeEndorsementBuilder WithDefaultExcesses()
        {
            return WithExcess(null, null);
        }

        public HomeEndorsementBuilder WithExcess(string buildingExcess, string contentsExcess)
        {
            Set(x => x.ExcessBuilding, buildingExcess);
            Set(x => x.ExcessContents, contentsExcess);
            return this;
        }

        /// <summary>
        /// This effectively removes building cover from the quote.
        /// </summary>
        /// <returns></returns>
        public HomeEndorsementBuilder WithoutBuildingSumInsured()
        {
            return WithBuildingSumInsured(null);
        }

        public HomeEndorsementBuilder WithBuildingSumInsured(int? sumInsured)
        {
            var home = Get(x => x.NewAssetValues);
            home.BuildingValue = sumInsured;
            Set(x => x.NewAssetValues, home);
            return this;
        }

        public HomeEndorsementBuilder WithRandomBuildingSumInsured(int minValue = HOME_BUILDING_SI_MIN, int maxValue = HOME_BUILDING_SI_MAX)
        {
            return WithBuildingSumInsured(DataHelper.RandomNumber(minValue, maxValue));
        }

        /// <summary>
        /// This effectively leaves contents sum insured as is.
        /// </summary>
        /// <returns></returns>
        public HomeEndorsementBuilder WithoutContentsSumInsured()
        {
            return WithContentsSumInsured(null);
        }

        public HomeEndorsementBuilder WithContentsSumInsured(int? sumInsured)
        {
            var home = Get(x => x.NewAssetValues);
            home.ContentsValue = sumInsured;
            Set(x => x.NewAssetValues, home);
            return this;
        }

        public HomeEndorsementBuilder WithRandomContentsSumInsured(int minValue = 30000, int maxValue = 300000)
        {
            return WithContentsSumInsured(DataHelper.RandomNumber(minValue, maxValue));
        }

        public HomeEndorsementBuilder WithPaymentMethod(Payment paymentMethod)
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
        public HomeEndorsementBuilder WithPayer(Contact payer)
        {
            var paymentMethod = Get(x => x.PayMethod);
            paymentMethod.Payer = payer;

            return WithPaymentMethod(paymentMethod);
        }

        public HomeEndorsementBuilder WithRandomPaymentMethod(Contact payer)
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
        public HomeEndorsementBuilder WithNextPaymentDate(int days)
        {
            if (days == 0) { return this; }

            var currentPolicy = Get(x => x.OriginalPolicyData);
            if (currentPolicy.NextPendingInstallment() != null)
            {
                Set(x => x.NextPaymentDate, currentPolicy.NextPendingInstallment().CollectionDate.AddDays(days));
            }
            return this;
        }

        public HomeEndorsementBuilder WithExpectedImpactOnPremium(PremiumChange change)
        {
            Set(x => x.ExpectedImpactOnPremium, change);
            return this;
        }

        /// <summary>
        /// For enhanced Spark payment options where member can
        /// choose BPay or delayed payment options, as well as
        /// conventional payment options
        /// </summary>
        /// TODO AUNT-166
        public HomeEndorsementBuilder WithSparkPaymentChoice(PaymentV2 payment)
        {
            Set(x => x.SparkExpandedPayment, payment);
            return this;
        }

        public HomeEndorsementBuilder InitialiseHomeWithDefaultData(string policynumber, Contact policyHolder = null)
        {
            var homeEndorsementBuilder = WithPolicy(policynumber, policyHolder)
                                        .WithEndorsementStartDate(DateTime.Now)
                                        .WithoutFinancier()
                                        .WithWeeklyRentalRate(null)
                                        .WithPropertyManager(HomePropertyManager.Undefined)
                                        .WithDefaultExcesses()
                                        .WithExpectedImpactOnPremium(PremiumChange.NotApplicable);
            
            return homeEndorsementBuilder;
        }

        protected override EndorseHome BuildEntity()
        {
            return new EndorseHome
            {
                PolicyNumber                      = GetOrDefault(x => x.PolicyNumber),
                ActivePolicyHolder                = GetOrDefault(x => x.ActivePolicyHolder),
                NewAssetValues                    = GetOrDefault(x => x.NewAssetValues),
                WeeklyRent                        = GetOrDefault(x => x.WeeklyRent),
                HomePropertyManager               = GetOrDefault(x => x.HomePropertyManager),
                ExcessBuilding                    = GetOrDefault(x => x.ExcessBuilding),
                ExcessContents                    = GetOrDefault(x => x.ExcessContents),
                Financier                         = GetOrDefault(x => x.Financier),
                PayMethod                         = GetOrDefault(x => x.PayMethod),
                StartDate                         = GetOrDefault(x => x.StartDate),
                ExpectedImpactOnPremium           = GetOrDefault(x => x.ExpectedImpactOnPremium),
                NextPaymentDate                   = GetOrDefault(x => x.NextPaymentDate),
                PremiumLabelText                  = GetOrDefault(x => x.PremiumLabelText),
                OriginalPolicyData                = GetOrDefault(x => x.OriginalPolicyData),
                SparkExpandedPayment              = GetOrDefault(x => x.SparkExpandedPayment),
            };
        }
    }
}