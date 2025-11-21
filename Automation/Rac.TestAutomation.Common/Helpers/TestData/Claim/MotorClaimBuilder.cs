using Rac.TestAutomation.Common.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.Contacts;

namespace Rac.TestAutomation.Common.TestData.Claim
{
    public class MotorClaimBuilder : EntityBuilder<ClaimCar, MotorClaimBuilder>
    {
        private Config _config;

        public MotorClaimBuilder()
        {
            _config = Config.Get();
        }

        public MotorClaimBuilder WithPolicyDetails(MotorPolicy policyDetails)
        {
            Set(x => x.Policy, policyDetails);
            return this;
        }

        public MotorClaimBuilder WithRiskAddressAndHireCarCover(string policyNumber)
        {
            var policyDetails = Get(x => x.Policy);            
            var getPolicy = DataHelper.GetPolicyDetails(policyNumber);
            var address = new Address();

            if (getPolicy.MotorAsset.Address == null)
            {
                address.Suburb = getPolicy.MotorAsset.Suburb;
            }
            else
            {
                address = getPolicy.MotorAsset.Address;
            }
            policyDetails.RiskAddress = address;
            policyDetails.HasHireCarCover = getPolicy.HasHireCarCover;
            Set(x => x.Policy, policyDetails);
            return this;
        }

        public MotorClaimBuilder WithExistingClaimDetails(string claimNumber)
        {
            var claimDeatils = DataHelper.GetClaimDetails(claimNumber);
            Set(x => x.ExistingClaim, claimDeatils);
            return this;
        }

        /// <summary>
        /// This method populates MC Mock with the latest Contact details for the Contact object passed 
        /// in which we will use as the informant for a Motor claim. 
        /// 
        /// A new value will be generated for mobile telephone or email if there is not one present in 
        /// the Contact object OR if a param demands it as long as excludedFromContactMFA is also true.
        /// </summary>
        /// <param name="contact">The contact selected to be the informant for this Motor claim.</param>
        /// <param name="changeEmailAddress">If TRUE, generate an email we will set for this contact during the claim.</param>
        /// <param name="changeMobileNumber">If TRUE, generate a mobile telephone we will set for this contact during the claim.</param>
        /// <param name="excludedFromContactMFA">If TRUE, allow email/mobile values to be generated, otherwise prevent it.</param>
        /// <returns></returns>
        public MotorClaimBuilder WithClaimant(PolicyContactDB contact, bool changeEmailAddress = false, bool changeMobileNumber = false, bool excludedFromContactMFA = false)
        {
            MemberCentral.PopulateMockMemberCentralWithLatestContactIdInfo(contact.Id);
            
            if (excludedFromContactMFA)
            {
                Reporting.LogMinorSectionHeading($"This test is not impacted by Multi-Factor Authentication changes and so we will generate telephone " +
                    $"and email values to update to if they are null or empty on the existing record.");
            
                if (contact.PrivateEmail == null)
			    {
                    contact.PrivateEmail = new Email(); 
                }

                if (changeEmailAddress || string.IsNullOrEmpty(contact.PrivateEmail?.Address))
                {
                    contact.PrivateEmail.NewAddress = DataHelper.RandomEmail(contact.FirstName, contact.Surname, domain: _config.Email?.Domain).Address;
                }

                if (changeMobileNumber || string.IsNullOrEmpty(contact.MobilePhoneNumber))
                {
                    contact.NewMobilePhoneNumber = DataHelper.RandomMobileNumber();
                }
            }
            else
            {
                Reporting.LogMinorSectionHeading($"This test is impacted by Multi-Factor Authentication changes and so we will not generate telephone " +
                    $"and email values to update to if they are null or empty on the existing record.");
            }

            Set(x => x.Claimant, contact);

            return this;
        }

        public MotorClaimBuilder WithRandomPolicyHoderAsClaimant(List<PolicyContactDB> contact)
        {
            var claimant = contact.PickRandom();
            Set(x => x.Claimant, claimant);
            return this;
        }

        public MotorClaimBuilder WithEventDateAndTime(DateTime dateAndTimeofAccident, bool roundDown15Mins = false)
        {            
            if (roundDown15Mins)
            {
                var roundedMinute = ((dateAndTimeofAccident.Minute / 15) * 15);
                dateAndTimeofAccident = dateAndTimeofAccident.AddMinutes(-dateAndTimeofAccident.Minute).AddMinutes(roundedMinute).AddSeconds(-dateAndTimeofAccident.Second);
            }
            Set(x => x.EventDateTime, dateAndTimeofAccident);
            return this;
        }

        public MotorClaimBuilder WithRandomEventDateInLast7Days(bool roundDown15Mins = false)
        {
            DateTime randomDateAndTimeOfAccident = DataHelper.GenerateRandomDate(DateTime.Now, DateTime.Now.AddDays(-7));
            DateTime dateAndTimeOfAccident = randomDateAndTimeOfAccident.AddTicks(-(randomDateAndTimeOfAccident.Ticks % TimeSpan.TicksPerSecond));
            if (roundDown15Mins)
            {
                var roundedMinute = ((dateAndTimeOfAccident.Minute / 15) * 15);
                dateAndTimeOfAccident = dateAndTimeOfAccident.AddMinutes(-dateAndTimeOfAccident.Minute).AddMinutes(roundedMinute).AddSeconds(-dateAndTimeOfAccident.Second);
            }

            //If the incident time is between from 10 PM to 6 AM, then shield put a payment block
            //If the calculated time is between 10 PM to 6 AM, then it's change the time to 10 AM
            TimeSpan start = new TimeSpan(6, 0, 0); //6 AM
            TimeSpan end = new TimeSpan(22, 0, 0); //10 PM
            if ((dateAndTimeOfAccident.TimeOfDay < start) || (dateAndTimeOfAccident.TimeOfDay > end))
            {
                TimeSpan ts = new TimeSpan(10, 00, 0);
                dateAndTimeOfAccident = dateAndTimeOfAccident.Date + ts;
            }

                Set(x => x.EventDateTime, dateAndTimeOfAccident);
            return this;
        }

        public MotorClaimBuilder WithEventDateSameAsPreviousClaimEventDate()
        {
            var claimEventDate = Get(x => x.ExistingClaim).EventDate;

            var roundedMinute = ((claimEventDate.Minute / 15) * 15);
            var dateAndTimeOfAccident = claimEventDate.AddMinutes(-claimEventDate.Minute).AddMinutes(roundedMinute).AddSeconds(-claimEventDate.Second);

            Set(x => x.EventDateTime, dateAndTimeOfAccident);
            return this;
        }

        public MotorClaimBuilder WithEventDateWithIn14DaysFromPreviousClaimEventDate()
        {
            var claimEventDate = Get(x => x.ExistingClaim).EventDate;

            var claimEventDateWithIn14days = DataHelper.GenerateRandomDate(claimEventDate.AddDays(-14), claimEventDate.AddDays(14));
            claimEventDateWithIn14days = claimEventDateWithIn14days > DateTime.Now ? DateTime.Now.AddHours(-1) : claimEventDateWithIn14days;

            var roundedMinute = ((claimEventDateWithIn14days.Minute / 15) * 15);
            var dateAndTimeOfAccident = claimEventDateWithIn14days.AddMinutes(-claimEventDateWithIn14days.Minute).AddMinutes(roundedMinute).AddSeconds(-claimEventDateWithIn14days.Second);

            Set(x => x.EventDateTime, dateAndTimeOfAccident);
            return this;
        }

        public MotorClaimBuilder WithNumberOfVehiclesInvolved(MotorCollisionNumberOfVehiclesInvolved numberOfVehiclesInvolved)
        {
            Set(x => x.NumberOfVehiclesInvolved, numberOfVehiclesInvolved);
            return this;
        }

        public MotorClaimBuilder WithDamageType(MotorClaimDamageType damageType)
        {
            Set(x => x.DamageType, damageType);
            return this;
        }

        public MotorClaimBuilder WithRandomEventLocation()
        {
            var address = new AddressBuilder().InitialiseRandomMailingAddress().Build();
            Set(x => x.EventLocation, address.StreetSuburbState());
            return this;
        }

        public MotorClaimBuilder WithEventLocation(string eventLocation)
        {
            Set(x => x.EventLocation, eventLocation);
            return this;
        }

        public MotorClaimBuilder OnlyClaimDamageToTPInClaim(bool onlyClaimDamageToTP)
        {
            Set(x => x.OnlyClaimDamageToTP, onlyClaimDamageToTP);
            return this;
        }

        public MotorClaimBuilder IsTPAssestDamaged(bool? isTPAssestDamage)
        {
            Set(x => x.IsTPPropertyDamage, isTPAssestDamage);
            return this;
        }

        public MotorClaimBuilder WithIsVehicleDriveable(bool? isVehicleDriveable)
        {
            Set(x => x.IsVehicleDriveable, isVehicleDriveable);
            return this;
        }
       
        public MotorClaimBuilder WithVehicleTowedDetails(bool? wasTowed, MotorClaimTowedTo towedTo)
        {

            TowDetails towDetails = new TowDetails
            {
                WasVehicleTowed = wasTowed,
                TowedTo = towedTo,
                BusinessDetails = new BusinessDetails { 
                    BusinessName = DataHelper.RandomLetters(6).FirstCharToUpper(),
                    ContactNumber = DataHelper.RandomMobileNumber(),
                    Address = new AddressBuilder().InitialiseRandomMailingAddress().Build().StreetSuburbStateShortened(true)
                },
                CarLocation = GetRandomMultiLineText()
            };

            Set(x => x.TowedVehicleDetails, towDetails);
            return this;
        }

        public MotorClaimBuilder WithRepairerOption(RepairerOption repairerOption)
        {
            Set(x => x.RepairerOption, repairerOption);
            return this;
        }

        public MotorClaimBuilder WithRandomPreferredRepairerLocation()
        {
            var address = new AddressBuilder().InitialiseRandomMailingAddress().Build();
            Set(x => x.PreferredRepairerSuburb, address);
            return this;
        }

        public MotorClaimBuilder WithPreferredRepairerLocation(Address location)
        {
            Set(x => x.PreferredRepairerSuburb, location);
            return this;
        }

        public MotorClaimBuilder WithRepairerAllocationExhausted(bool repairerAllocation)
        {
            Set(x => x.IsRepairerAllocationExhausted, repairerAllocation);
            return this;
        }

        public MotorClaimBuilder WithTheftDetails(MotorClaimTheftDetails stolenItem, bool keysStolen = false, bool financed = false, bool forSale = false)
        {
            var theftDetails = new TheftDetails()
            {
                WhatWasStolen     = stolenItem,
                WereKeysStolen    = keysStolen,
                WasFinanceOwing   = financed,
                WasVehicleForSale = forSale
            };

            Set(x => x.TheftDetails, theftDetails);

            return this;
        }

        public MotorClaimBuilder WithGlassDamageDetails(bool isWindscreenOnly, bool otherWindowGlass, GlassDamageType typeOfDamage, bool isAlreadyRepaired)
        {
            var damageDetails = new GlassDamageDetails()
            {
                FrontWindscreenOnly = isWindscreenOnly,
                OtherWindowGlass = otherWindowGlass,
                TypeOfDamage = typeOfDamage,
                HasBeenRepaired = isAlreadyRepaired                
            };

            Set(x => x.GlassDamageDetails, damageDetails);

            return this;
        }
        
        public MotorClaimBuilder WithTravelDirection(TravelDirection direction)
        {
            Set(x => x.DirectionBeingTravelled, direction);
            return this;
        }

        public MotorClaimBuilder WithClaimScenario(MotorClaimScenario scenario)
        {
            Set(x => x.ClaimScenario, scenario);
            return this;
        }

        public MotorClaimBuilder WithRandomDescriptionOfDamageToPHVehicle()
        {
            Set(x => x.DamageToPHVehicle, GetRandomMultiLineText());
            return this;
        }

        public MotorClaimBuilder WithDescriptionOfDamageToPHVehicle(string damageDescription)
        {
            Set(x => x.DamageToPHVehicle, damageDescription);
            return this;
        }

        public MotorClaimBuilder WithRandomAccountOfAccident()
        {
            Set(x => x.AccountOfAccident, GetRandomMultiLineText());
            return this;
        }

        public MotorClaimBuilder WithAccountOfAccident(string accidentDescription)
        {
            Set(x => x.AccountOfAccident, accidentDescription);
            return this;
        }

        public MotorClaimBuilder WithNoPoliceReport()
        {
            Set(x => x.PoliceReportNumber, null);
            return this;
        }

        public MotorClaimBuilder WithPoliceReport(string reportNumber, DateTime reportDate)
        {
            Set(x => x.PoliceReportNumber, reportNumber);
            Set(x => x.PoliceReportDate, reportDate);
            return this;
        }

        public MotorClaimBuilder WithRandomPoliceDetails()
        {
            var randomBoolean = DataHelper.RandomBoolean();
            Set(x => x.IsPoliceInvolved, randomBoolean);
            if (randomBoolean)
            {
                if (DataHelper.RandomBoolean())
                {
                    Set(x => x.PoliceReportNumber, DataHelper.RandomAlphanumerics(5, 15));
                }
            }
            return this;
        }

        public MotorClaimBuilder WithRandomPoliceReport()
        {
            Set(x => x.PoliceReportNumber, DataHelper.RandomAlphanumerics(5,15));
            Set(x => x.PoliceReportDate, DateTime.Now);
            return this;
        }

        public MotorClaimBuilder AddWitness(Contact contact)
        {
            var witnesses = GetOrDefault(x => x.Witness, new List<Contact>()).ToList();

            witnesses.Add(contact);

            Set(x => x.Witness, witnesses);
            return this;
        }

        public MotorClaimBuilder WithNoWitnesses()
        {
            Set(x => x.Witness, null);
            return this;
        }

        public MotorClaimBuilder AddRandomWitness(int witnessCount = 1)
        {
            var witnesses = GetOrDefault(x => x.Witness, new List<Contact>()).ToList();

            for (int i = 0; i < witnessCount; i++)
            {
                var newWitness = new ContactBuilder().WithRandomFirstName().WithRandomSurname();
                switch(DataHelper.RandomNumber(1,5))
                {
                    case 1:
                        newWitness.WithPrivateEmailAddressFromName();
                        break;
                    case 2:
                        newWitness.WithRandomAustralianHomePhoneNumber();
                        break;
                    case 3:
                        newWitness.WithRandomMobileNumber();
                        break;
                    default:
                        newWitness.WithRandomMobileNumber()
                            .WithPrivateEmailAddressFromName();                            
                        break;
                }
                witnesses.Add(newWitness.Build());
            }

            Set(x => x.Witness, witnesses);
            return this;
        }

        public MotorClaimBuilder WithClaimantIsDriver(bool wasDrunk = false, bool wasSuspended = false, bool isLicensedMoreThan2Years = true)
        {
            return WithDriver(driver: null, wasDrunk: wasDrunk, wasSuspended: wasSuspended, isLicensedMoreThan2Years: isLicensedMoreThan2Years);
        }

        public MotorClaimBuilder LoginWith(LoginWith loginWith)
        {
            Set(x => x.LoginWith, loginWith);
            return this;
        }

        public MotorClaimBuilder WithDriver(Contact driver, bool wasDrunk = false, bool wasSuspended = false, bool isLicensedMoreThan2Years = true)
        {
            var driverDetails = new ContactClaimDriver()
            {
                DriverDetails                = driver,
                DriverLicensedMoreThan2Years = isLicensedMoreThan2Years,
                WasDriverDrunk               = wasDrunk,
                WasDriverLicenceSuspended    = wasSuspended,
                AdditionalInformation        = DataHelper.RandomAlphanumerics(10, 80)
            };

            Set(x => x.Driver, driverDetails);
            return this;
        }

        public MotorClaimBuilder WithNoThirdParty()
        {
            Set(x => x.ThirdParty, null);
            return this;
        }

        /// <summary>
        /// Adds a single third party contact
        /// </summary>
        /// <param name="isKnownToClaimant">boolean for Yes/No question for whether TP was previously known to claimant</param>
        /// <param name="thirdPartyInsurer">null = UNKNOWN, (constant)NO_PREVIOUS_INSURANCE for NO, any other value for an insurer</param>
        /// <returns></returns>
        public MotorClaimBuilder AddRandomThirdParty(bool isKnownToClaimant = false, API_InsuranceCompanies thirdPartyInsurer = null, bool wasDriverTheOwner = true)
        {
            var thirdParty = GetOrDefault(x => x.ThirdParty, new List<ContactClaimMotorTP>()).ToList();

            var thirdPartyContact = new ContactBuilder();
            switch (DataHelper.RandomNumber(1, 5))
            {
                case 1:
                    thirdPartyContact.WithRandomFirstName()
                        .WithRandomSurname()
                        .WithPrivateEmailAddressFromName()
                        .WithRandomMailingAddress();
                    break;
                case 2:
                    thirdPartyContact.WithRandomFirstName()
                    .WithRandomMobileNumber();
                    break;
                case 3:
                    thirdPartyContact.WithRandomSurname()
                        .WithRandomAustralianHomePhoneNumber()                        
                        .WithRandomMailingAddress();
                    break;
                case 4:
                    thirdPartyContact.WithRandomSurname()
                         .WithRandomMobileNumber()
                        .WithPrivateEmail(DataHelper.RandomEmail());
                    break;
                default:
                    thirdPartyContact.WithRandomMobileNumber()
                        .WithPrivateEmail(DataHelper.RandomEmail());
                    break;
            }
            ContactClaimMotorTP newThirdParty = new ContactClaimMotorTP(thirdPartyContact.Build());
                                    
            newThirdParty.Insurer = new InsuranceDetails();
            newThirdParty.IsKnownToClaimant = isKnownToClaimant;

            newThirdParty.WasDriverTheOwner = wasDriverTheOwner;
            newThirdParty.Title = DataHelper.GetRandomTitleForGender(newThirdParty.Gender, excludeDrTitle: true);

            if (DataHelper.RandomBoolean()) 
            {
                newThirdParty.Rego = DataHelper.RandomAlphanumerics(7);
            } 

            if (thirdPartyInsurer != null && DataHelper.RandomBoolean())
            {
                newThirdParty.Insurer.Name = thirdPartyInsurer.Name;
                newThirdParty.Insurer.ExternalContactNumber = thirdPartyInsurer.ExternalContactNumber;
            }

            if (DataHelper.RandomBoolean())
            {
                newThirdParty.DescriptionOfDamageToVehicle = GetRandomMultiLineText();
            }

            if (!wasDriverTheOwner && DataHelper.RandomBoolean())
            {
                newThirdParty.AdditionalInfo = DataHelper.RandomAlphanumerics(90);
            }

            if (DataHelper.RandomBoolean())
            {
                newThirdParty.ClaimNumber = DataHelper.RandomNumbersAsString(10);
            }
                
            thirdParty.Add(newThirdParty);

            Set(x => x.ThirdParty, thirdParty);
            return this;
        }

        public MotorClaimBuilder WithLinkedMotorPoliciesForClaimant(List<PolicyDetail> motorPolicies)
        {
            var policies = GetOrDefault(x => x.LinkedMotorPoliciesForClaimant, new List<MotorPolicy>()).ToList();

            foreach(var policy in motorPolicies)
            {
                var motorPolicy = new MotorPolicy { Vehicle = new Car() };
                motorPolicy.PolicyNumber = policy.policyNumber;
                motorPolicy.Vehicle.Make = policy.motorAsset.manufacturer;
                motorPolicy.Vehicle.Registration = policy.motorAsset.registrationNumber;
                motorPolicy.Vehicle.Year = policy.motorAsset.year;
                motorPolicy.Vehicle.Model = policy.motorAsset.modelDescription;
                motorPolicy.CoverType = policy.cover.FirstOrDefault().coverType;

                policies.Add(motorPolicy);
            }

            Set(x => x.LinkedMotorPoliciesForClaimant, policies);
            return this;
            
        }

        public MotorClaimBuilder WithClaimUploadFiles(string claimNumber, string persoonId, string claimantFirstName, List<string> files)
        {
            ClaimUploadFile glassClaimUploadFile = new ClaimUploadFile(claimNumber, persoonId, claimantFirstName, files);
            Set(x => x.ClaimUploadFile, glassClaimUploadFile);
            return this;

        }

        public MotorClaimBuilder WithPaymentBlock()
        {
            Set(x => x.expectClaimPaymentBlock, true);
            return this;
        }

        public MotorClaimBuilder InitialiseMotorClaimWithBasicData(MotorPolicy policyDetailsToUse, MotorClaimDamageType damageType)
        {
            var motorClaimBuilder = WithPolicyDetails(policyDetailsToUse)
                                   .WithEventDateAndTime(DateTime.Now.Date)
                                   .WithDamageType(damageType)
                                   .WithRandomEventLocation()
                                   .OnlyClaimDamageToTPInClaim(false)
                                   .WithRandomPreferredRepairerLocation()
                                   .WithTravelDirection(TravelDirection.Parked)                                   
                                   .WithRandomDescriptionOfDamageToPHVehicle()
                                   .WithRandomAccountOfAccident()
                                   .WithNoPoliceReport()
                                   .WithClaimantIsDriver()
                                   .WithNoWitnesses()
                                   .WithNoThirdParty();

            return motorClaimBuilder;
        }

        protected override ClaimCar BuildEntity()
        {
            return new ClaimCar
            {
                Policy          = GetOrDefault(x => x.Policy),
                ExistingClaim   = GetOrDefault(x => x.ExistingClaim),
                Claimant        = GetOrDefault(x => x.Claimant),
                EventDateTime   = GetOrDefault(x => x.EventDateTime),
                DamageType      = GetOrDefault(x => x.DamageType),
                EventLocation   = GetOrDefault(x => x.EventLocation),
                OnlyClaimDamageToTP         = GetOrDefault(x => x.OnlyClaimDamageToTP),
                IsTPPropertyDamage          = GetOrDefault(x => x.IsTPPropertyDamage),                
                IsVehicleDriveable     = GetOrDefault(x => x.IsVehicleDriveable),                
                TowedVehicleDetails    = GetOrDefault(x => x.TowedVehicleDetails),   
                PreferredRepairerSuburb     = GetOrDefault(x => x.PreferredRepairerSuburb),
                RepairerOption              = GetOrDefault(x => x.RepairerOption),
                AssignedRepairer            = GetOrDefault(x => x.AssignedRepairer, new BusinessDetails()),
                IsRepairerAllocationExhausted = GetOrDefault(x => x.IsRepairerAllocationExhausted, false),
                ExpectCompleteLodgementDone = GetOrDefault(x => x.ExpectCompleteLodgementDone, false),
                TheftDetails            = GetOrDefault(x => x.TheftDetails),
                GlassDamageDetails      = GetOrDefault(x => x.GlassDamageDetails),
                DirectionBeingTravelled = GetOrDefault(x => x.DirectionBeingTravelled),                
                ClaimScenario           = GetOrDefault(x => x.ClaimScenario),
                DamageToPHVehicle       = GetOrDefault(x => x.DamageToPHVehicle),
                AccountOfAccident  = GetOrDefault(x => x.AccountOfAccident),
                PoliceReportNumber = GetOrDefault(x => x.PoliceReportNumber),
                PoliceReportDate   = GetOrDefault(x => x.PoliceReportDate),
                Driver             = GetOrDefault(x => x.Driver),
                ThirdParty         = GetOrDefault(x => x.ThirdParty),
                Witness            = GetOrDefault(x => x.Witness),
                LoginWith          = GetOrDefault(x => x.LoginWith),
                LinkedMotorPoliciesForClaimant  = GetOrDefault(x => x.LinkedMotorPoliciesForClaimant),
                ClaimUploadFile                 = GetOrDefault(x => x.ClaimUploadFile),                
                expectClaimPaymentBlock         = GetOrDefault(x => x.expectClaimPaymentBlock),
                NumberOfVehiclesInvolved        = GetOrDefault(x => x.NumberOfVehiclesInvolved),
                IsPoliceInvolved                = GetOrDefault(x => x.IsPoliceInvolved),
                IsPoliceReportNumberAvailable   = GetOrDefault(x => x.IsPoliceReportNumberAvailable),
                ShieldDamageType                = GetOrDefault(x => x.ShieldDamageType),               
            };
        }

        private string GetRandomMultiLineText()
        {
            var description = new System.Text.StringBuilder();
            int max = Randomiser.Get.Next(1, 8);

            for (int i = 0; i < max; i++)
            {
                description.Append($"{DataHelper.RandomAlphanumerics(10, 80)}.\r\n");
            }
            var text = Regex.Replace(description.ToString(), "0x", "", RegexOptions.IgnoreCase);
            return text;
        }
    }
}
