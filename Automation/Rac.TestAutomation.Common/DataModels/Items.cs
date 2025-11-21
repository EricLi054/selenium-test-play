using System;
using System.Collections.Generic;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyHome;
using static Rac.TestAutomation.Common.Constants.PolicyPet;


namespace Rac.TestAutomation.Common
{
    /// <summary>
    /// For any given Shield identifier, it will have a description text
    /// that will be more meaningful that the plain ID. That description
    /// may not be identical across B2C and Shield. This class is used
    /// by dictionaries to allow for an easy reference to these values.
    /// </summary>
    public class IdDescriptions
    {
        public string TextB2C { get; set; }
        public string TextSpark { get; set; }
        public string TextShield { get; set; }
    }

    public class ShieldApi
    {
        public string Code { get; set; }        
    }
    public class FenceDescriptions
    {
        public string TextB2C { get; set; }
        public string TextSpark { get; set; }
        public string ShieldAnswerId { get; set; }
    }

    public class SparkShieldQuestionnaireDescriptions
    {
        public string TextSpark { get; set; }
        public string TextShield { get; set; }
        public string ShieldAnswerId { get; set; }
    }

    public class ProductTypes
    {
        public string Motorcycle { get; set; }
        public string Caravan { get; set; }
    }

    abstract public class Vehicles
    {
        private string _make;
        private string _model;
        public string Make
        {
            get => _make;
            set => _make = string.IsNullOrEmpty(value) ? null : value.RemoveDuplicateWhiteSpaceAndTrim();
        }
        public string Model
        {
            get => _model;
            set => _model = string.IsNullOrEmpty(value) ? null : value.RemoveDuplicateWhiteSpaceAndTrim();
        }

        public decimal Year     { get; set; }
        public string VehicleId { get; set; }
        public string Financier { get; set; }
        public bool IsFinanced
        {
            get
            {
                if (string.IsNullOrEmpty(Financier)) { return false; }
                return true;
            }
        }
        public bool   IsModified { get; set; }
        public string Registration { get; set; }
        public bool HasImmobiliser { get; set; }
        public bool HasTracker { get; set; }
        public int MarketValue { get; set; }
        // If the vehicle is an electric vehicle
        public bool IsElectricVehicle { get; set; }
    }

    /// <summary>
    /// Asset object for "cars", or general motor vehicles.
    /// (There are some light buses and vans in this category).
    /// </summary>
    public class Car : Vehicles
    {
        public string Body { get; set; }
        public string Transmission { get; set; }
    }

    public class Motorcycle : Vehicles
    {
        public string EngineCC { get; set; }
        public bool HasDashcam { get; set; }
        public bool IsGaraged { get; set; }
        /// <summary>
        /// For IsPremiumChangeExpected to be 'true' following needs to be satisfied:
        /// -Test case needs to be choosing SKIP/NO on Are you a member? page
        /// -Contact information must exist in MC as a single match with a current gold/silver/bronze RSA membership
        /// -vehicle value, excess and usage inputs must result in a premium that is above minimum premium and satisfy the 'Golden Rule'
        /// </summary>
        public bool IsPremiumChangeExpected { get; set; }
        /// <summary>
        /// Returns a formatted string that is expected in the public portal
        /// for the chosen motorcycle.
        /// 
        /// Format:
        ///    Year Make Model Engine
        /// </summary>
        /// <returns></returns>
        public string GetFullMotorcycleName()
        {
            // We can expect that "Make" and "Year" will always have a value.
            var description = $"{Year} {Make}";

            // Some motorcycles, like 2016 Hunter Scooter, do not have a model.
            if (!string.IsNullOrWhiteSpace(Model))
                description = $"{description} {Model}".Trim();

            // Engine should generally be populated, but there may be exceptions.
            if (!string.IsNullOrWhiteSpace(EngineCC))
                description = $"{description} {EngineCC}";

            return description;
        }
    }

    public class Pet
    {
        /// <summary>
        /// If TRUE, then treated as a dog, otherwise treated as a cat.
        /// </summary>
        public PetType Type { get; set; }
        public string Breed { get; set; }
        public string Name  { get; set; }
        public DateTime DateOfBirth     { get; set; }
        /// <summary>
        /// If TRUE, then treated as a female animal, otherwise treated
        /// as male.
        /// </summary>
        public Gender Gender     { get; set; }
        public bool IsSterilised           { get; set; }
        public bool HasPrexistingCondition { get; set; }
        public bool IsBusinessOwned        { get; set; }
        public bool IsWorkingAnimal        { get; set; }
        public bool HasVet
        {
            get
            {
                return !(string.IsNullOrEmpty(VetName) && string.IsNullOrEmpty(VetAddress));
            }
        }
        public string VetName    { get; set; }
        public string VetAddress { get; set; }
    }


    public class Boat : Vehicles
    {
        public BoatType Type { get; set; }
        public BoatMake BoatMake { get; set; }
        public BoatHullMaterial SparkBoatHull { get; set; }
        public SkippersTicketYearsHeld SkippersTicketHeld { get; set; }
        public BoatClaimsInLastThreeYears HistoricBoatClaims { get; set; }
        public MotorType SparkBoatMotorType { get; set; }
        public bool SecurityAlarmGps {get; set;}
        public bool SecurityNebo { get; set; }
        public bool SecurityHitch { get; set; }
        public bool IsGaraged { get; set; }
        public int BoatYearBuilt { get; set; }
        public int InsuredAmount { get; set; }
        public bool HasWaterSkiingAndFlotationDeviceCover { get; set; }
        public bool HasRacingCover { get; set; }
    }

    public class Caravan : Vehicles
    {
        public string ModelDescription { get; set; } //Applicable for Spark only
        public CaravanType Type { get; set; }
        public CaravanParkLocation ParkLocation { get; set; }
    }

    public class ContentItem
    {
        public ContentItem() { }

        public ContentItem(string description, int value)
        {
            Description = description;
            Value = value;
        }

        /// <summary>
        /// Should be cast to either Constants.SpecifiedContents
        /// or Constants.SpecifiedValuables when being referenced.
        /// </summary>
        public int    Category { get; set; }
        public string Description { get; set; }
        public int    Value { get; set; }

        /// <summary>
        /// Returns the Category as the B2C dropdown string for
        /// Specified Personal Valuables.
        /// </summary>
        public string CategoryB2CStringForSpecifiedValuable
        {
            get
            {
                return SpecifiedPersonalValuablesDisplayedText[(SpecifiedValuables)Category].TextB2C;
            }
        }

        /// <summary>
        /// Returns the Category as the B2C dropdown string for
        /// Specified Contents.
        /// </summary>
        public string CategoryB2CStringForSpecifiedContents
        {
            get
            {
                return SpecifiedContentsDisplayedText[(SpecifiedContents)Category].TextB2C;
            }
        }
    }

    public class Home
    {
        public Address PropertyAddress { get; set; }
        /// <summary>
        /// For new business home scenarios, if this parameter has a non-null value,
        /// then building cover will be added to the policy.
        /// For Home Endorsements, a non-null value will indicate that the policy
        /// should already have building cover and it will be kept/modified to this
        /// value.
        /// </summary>
        public int? BuildingValue { get; set; }
        /// <summary>
        /// For new business home scenarios, if this parameter has a non-null value,
        /// then general contents cover will be added to the policy.
        /// For Home Endorsements, a non-null value will indicate that the policy
        /// should already have contents cover and it will be kept/modified to this
        /// value.
        /// </summary>
        public int? ContentsValue { get; set; }
        public UnspecifiedPersonalValuables UnspecifiedValuablesInsuredAmount { get; set; }
        public List<ContentItem> SpecifiedValuablesInside { get; set; }
        public List<ContentItem> SpecifiedValuablesOutside { get; set; }
        public HomeType     TypeOfBuilding { get; set; }
        public HomeMaterial WallMaterial { get; set; }
        public HomeRoof     RoofMaterial { get; set; }
        public int YearBuilt { get; set; }
        public Alarm AlarmSystem { get; set; }
        public bool SecurityWindowsSecured { get; set; }
        public bool SecurityDoorsSecured { get; set; }
        /// <summary>
        /// Denotes whether property address is expected to be in a cyclone risk area.
        /// </summary>
        public bool IsACycloneAddress { get; set; }
        public bool IsPropertyElevated { get; set; }
        public bool IsCycloneShuttersFitted { get; set; }
        public GarageDoorsUpgradeStatus GarageDoorsCycloneStatus { get; set; }
        public RoofImprovementStatus    RoofImprovementCycloneStatus { get; set; }
        public string Financier { get; set; }
        public bool IsFinanced
        {
            get
            {
                if (string.IsNullOrEmpty(Financier)) { return false; }
                return true;
            }
        }
    }

    public class DateNumeric
    {
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public DateTime ToDateTime()
        {
            return new DateTime(year, month, day);
        }
    }
}