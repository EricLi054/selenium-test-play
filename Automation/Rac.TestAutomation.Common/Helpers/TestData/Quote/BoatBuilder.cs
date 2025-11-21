using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;

namespace Rac.TestAutomation.Common.TestData.Quote
{
    public class BoatBuilder : EntityBuilder<QuoteBoat, BoatBuilder>
    {
        [SuppressMessage("SonarLint", "S4487", Justification = "Safely unused variable, follows pattern of other ProductBuilders")]
        private Config _config;

        public BoatBuilder() 
        { 
            _config = Config.Get(); 
        }

        public BoatBuilder InitialiseBoatWithRandomData(List<Contact> candidatePolicyHolders)
        {
            var BoatBuilder = WithType(DataHelper.GetRandomEnum<SparkBoatTypeExternalCode>())
                .WithMake(DataHelper.GetRandomEnum<BoatMake>())
                .WithBoatHullMaterial(DataHelper.GetRandomEnum<BoatHullMaterial>())
                .WithPolicyStartDate(DateTime.Now.AddDays(DataHelper.RandomNumber(0, 29)).Date)
                .WithSkippersTicketHeld(DataHelper.GetRandomEnum<SkippersTicketYearsHeld>())
                .WithClaimsHistory(DataHelper.GetRandomEnum<BoatClaimsInLastThreeYears>())
                .WithRandomYearBuilt()
                .WithRandomBoatValue()
                .WithBasicExcess(DataHelper.RandomNumber(0,21)*100)
                .WithRandomFinancier()
                .WithRiskSuburbFromAddress(candidatePolicyHolders[0].MailingAddress)
                .WithRandomIsGaraged(candidatePolicyHolders[0].MailingAddress.PostCode)
                .WithRandomMotorType(DataHelper.GetRandomEnum<MotorType>())
                .WithRandomSecurity()
                .WithRandomWaterskiingAndFlotationDeviceCover()
                .WithRandomRacingCover()
                .WithCandidatePolicyHolder(candidatePolicyHolders)
                .WithRandomBoatRego()
                .WithRandomBoatTrailerRego()
                .WithPayer(candidatePolicyHolders[0])
                .WithRandomPaymentMethod();
            return BoatBuilder;
        }

        public BoatBuilder WithType(SparkBoatTypeExternalCode boatType)
        {
            Set(x => x.BoatTypeExternalCode, boatType);
            return this;
        }
        public BoatBuilder WithMake(BoatMake boatMake)
        {
            Set(x => x.BoatMake, boatMake);
            return this;
        }
        public BoatBuilder WithBoatHullMaterial(BoatHullMaterial boatHull)
        {
            Set(x => x.SparkBoatHull, boatHull);
            return this;
        }
        public BoatBuilder WithCandidatePolicyHolder(List<Contact> candidatePolicyHolders)
        {
            Set(x => x.CandidatePolicyHolders, candidatePolicyHolders);
            return this;
        }
        public BoatBuilder WithSkippersTicketHeld(SkippersTicketYearsHeld skippersTicketYearsHeld)
        {
            Set(x => x.SkippersTicketHeld, skippersTicketYearsHeld);
            return this;
        }
        public BoatBuilder WithClaimsHistory(BoatClaimsInLastThreeYears claimHistory)
        {
            Set(x => x.HistoricBoatClaims, claimHistory);
            return this;
        }
        public BoatBuilder WithPolicyStartDate(DateTime date)
        {
            Set(x => x.PolicyStartDate, date);
            return this;
        }

        public BoatBuilder WithRiskSuburb(string suburb, string postcode)
        {
            var address = new Address()
            {
                Suburb = suburb,
                PostCode = postcode
            };
            return WithRiskSuburbFromAddress(address);
        }

        public BoatBuilder WithRiskSuburbFromAddress(Address address)
        {
            Set(x => x.ParkingAddress, address);
            return this;
        }

        public BoatBuilder WithIsBoatGaraged(bool isGaraged)
        {
            Set(x => x.IsGaraged, isGaraged);
            return this;
        }

        public BoatBuilder WithRandomIsGaraged(string postcode)
        {
            var value = Randomiser.Get.Next(2) == 0 ? false : true;
            
            //Always set IsGaraged to "True" if the 'Parking Suburb' is above the 26th Parallel.
            //This is done to avoid user getting knocked out randomly.
            if (DataHelper.IsPostcodeAbove26thParallel(postcode)) 
            {
                Reporting.Log($"NOTE: Parking Suburb is above 26th Parallel so IsGaraged has been set to " +
                                $"True to avoid random Decline Cover.");
                value = true;
            }
            return WithIsBoatGaraged(value);
        }

        public BoatBuilder WithYearBuilt(int yearBuilt)
        {
            Set(x => x.BoatYearBuilt, yearBuilt);
            return this;
        }

        public BoatBuilder WithRandomYearBuilt()
        {
            return WithYearBuilt(Randomiser.Get.Next(1800, DateTime.Now.AddYears(1).Year));
        }

        public BoatBuilder WithBoatValue(int value)
        {
            Set(x => x.InsuredAmount, value);
            return this;
        }

        public BoatBuilder WithRandomBoatValue()
        {
            return WithBoatValue(Randomiser.Get.Next(3000, BOAT_MAXIMUM_INSURED_VALUE_ONLINE));
        }

        public BoatBuilder WithBasicExcess(int value)
        {
            Set(x => x.BasicExcess, value);
            return this;
        }

        public BoatBuilder WithFinancier(string financier)
        {
            Set(x => x.Financier, financier);
            return this;
        }

        public BoatBuilder WithoutFinancier()
        {
            Set(x => x.Financier, null);
            return this;
        }
        public BoatBuilder WithRandomFinancier()
        {
            return WithFinancier(FinancierOptions.OrderBy(t => Guid.NewGuid()).First());
        }
        
        public BoatBuilder WithWaterSkiingAndFlotationDeviceCover(bool hasCover)
        {
            Set(x => x.HasWaterSkiingAndFlotationDeviceCover, hasCover);
            return this;
        }

        public BoatBuilder WithRandomWaterskiingAndFlotationDeviceCover()
        {
            var randValue = Randomiser.Get.Next(2) == 0 ? false : true;

            return WithWaterSkiingAndFlotationDeviceCover(randValue);
        }

        public BoatBuilder WithRacingCover(bool hasCover)
        {
            Set(x => x.HasRacingCover, hasCover);
            return this;
        }

        public BoatBuilder WithRandomRacingCover()
        {
            var randValue = Randomiser.Get.Next(2) == 0 ? false : true;

            return WithRacingCover(randValue);
        }

        public BoatBuilder WithRandomMotorType(MotorType boatMotorType)
        {
            Set(x => x.SparkBoatMotorType, boatMotorType);
            return this;
        }




        /// <summary>
        /// Randomly assigns true/false flags for the four options available for selection under Security.
        /// 
        /// Begins with a 50% chance of leaving the three products defaulted to "false" and selecting 
        /// "No security".
        /// 
        /// If it is determined that there's a possibility of some security options being selected,
        /// each product gets its own probability of being selected. These percentages aren't indicative
        /// of market research, just an arbitrary better than 50% chance (as they already had to "win" 
        /// a coin-toss to be considered eligible).
        /// </summary>
        /// <returns></returns>
        public BoatBuilder WithRandomSecurity()
        {
            var fullRange = 100;
            var percentageSecurity = 50;
            var percentageHaveNebo = 60;
            var percentageHaveGPS = 75;
            var percentageHaveHitch = 90;

            Set(x => x.SecurityAlarmGps, false);
            Set(x => x.SecurityNebo, false);
            Set(x => x.SecurityHitch, false);
            
            var value = Randomiser.Get.Next(fullRange) < percentageSecurity;
            if (value)
            {
                Set(x => x.SecurityAlarmGps, Randomiser.Get.Next(fullRange) < percentageHaveGPS);
                Set(x => x.SecurityNebo, Randomiser.Get.Next(fullRange) < percentageHaveNebo);
                Set(x => x.SecurityHitch, Randomiser.Get.Next(fullRange) < percentageHaveHitch);
            }
            return this;
        }

        public BoatBuilder WithRandomBoatRego()
        {
            var fullRange = 100;
            var percentageBoatRego = 50;
            
            var result = Randomiser.Get.Next(fullRange) < percentageBoatRego;
            if (result)
            {
                WithBoatRego(DataHelper.RandomAlphanumerics(6, 10));
            }
            else
            {
                Set(x => x.BoatRego, null);
            }
            return this;
        }

        public BoatBuilder WithBoatRego(string boatRego)
        {
            Set(x => x.BoatRego, boatRego);
            return this;
        }
        public BoatBuilder WithRandomBoatTrailerRego()
        {
            var fullRange = 100;
            var percentageTrailerRego = 50;
                
            var result = Randomiser.Get.Next(fullRange) < percentageTrailerRego;
            if (result)
            {
                WithBoatTrailerRego(DataHelper.RandomAlphanumerics(7, 10));
            }
            else
            {
                Set(x => x.BoatTrailerRego, null);
            }
            return this;
        }
        public BoatBuilder WithBoatTrailerRego(string boatTrailerRego)
        {
            Set(x => x.BoatTrailerRego, boatTrailerRego);
            return this;
        }

        public BoatBuilder WithPaymentMethod(Payment paymentMethod)
        {
            Set(x => x.PayMethod, paymentMethod);
            return this;
        }

        /// <summary>
        /// Sets the provided contact as payer, and randomizes the
        /// payment terms. Will override any previously set payment
        /// details.
        /// </summary>
        /// <returns></returns>
        public BoatBuilder WithPayer(Contact contact)
        {
            Set(x => x.PayMethod, new Payment(contact));
            return this;
        }

        public BoatBuilder WithRandomPaymentMethod()
        {
            Set(x => x.PayMethod, new Payment(Get(x => x.CandidatePolicyHolders)));
            return this;
        }
        protected override QuoteBoat BuildEntity()
        {
            return new QuoteBoat
            {
                CandidatePolicyHolders = GetOrDefault(x => x.CandidatePolicyHolders),
                ParkingAddress = GetOrDefault(x => x.ParkingAddress),
                SkippersTicketHeld = GetOrDefault(x => x.SkippersTicketHeld),
                HistoricBoatClaims = GetOrDefault(x => x.HistoricBoatClaims),
                BoatTypeExternalCode = GetOrDefault(x => x.BoatTypeExternalCode),
                BoatMake = GetOrDefault(x => x.BoatMake),
                BoatYearBuilt = GetOrDefault(x => x.BoatYearBuilt),
                SparkBoatHull = GetOrDefault(x => x.SparkBoatHull),
                SecurityAlarmGps = GetOrDefault(x => x.SecurityAlarmGps),
                SecurityNebo = GetOrDefault(x => x.SecurityNebo),
                SecurityHitch = GetOrDefault(x => x.SecurityHitch),
                InsuredAmount = GetOrDefault(x => x.InsuredAmount),
                BasicExcess = GetOrDefault(x => x.BasicExcess),
                Financier = GetOrDefault(x => x.Financier),
                IsGaraged = GetOrDefault(x => x.IsGaraged),
                HasWaterSkiingAndFlotationDeviceCover = GetOrDefault(x => x.HasWaterSkiingAndFlotationDeviceCover),
                HasRacingCover = GetOrDefault(x => x.HasRacingCover),
                PolicyStartDate = GetOrDefault(x => x.PolicyStartDate),
                BoatRego = GetOrDefault(x => x.BoatRego),
                BoatTrailerRego = GetOrDefault(x => x.BoatTrailerRego),
                PayMethod = GetOrDefault(x => x.PayMethod)
            };
        }
    }
}
