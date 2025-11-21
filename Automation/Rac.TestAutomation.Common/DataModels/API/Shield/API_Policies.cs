using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;


namespace Rac.TestAutomation.Common.API
{
    public class Quote
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    /// <summary>
    /// Model used for both Quotes and Policies.
    /// </summary>
    public class GetQuotePolicy_Response
    {
        [JsonProperty("quoteNumber")]
        public string QuoteNumber { get; set; }
        [JsonProperty("policyNumber")]
        public string PolicyNumber { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("annualPremium")]
        public PremiumDetails AnnualPremium { get; set; }
        [JsonProperty("totalInstallmentPremium")]
        public PremiumDetails TotalInstallmentPremium { get; set; }
        [JsonProperty("isPaidInFull")]
        public bool IsPaidInFull { get; set; }
        [JsonProperty("nextPayableInstallment")]
        public NextPayableInstallment NextPayableInstallment { get; set; }
        [JsonProperty("realTimePaymentsDetails")]
        public RealTimePaymentDetails RealTimePaymentDetails { get; set; }
        [JsonProperty("installments")]
        public List<InstallmentDetails> Installments { get; set; }
        [JsonProperty("motorAsset")]
        public MotorcarDetails MotorAsset { get; set; }
        [JsonProperty("motorcycleAsset")]
        public MotorcycleDetails MotorcycleAsset { get; set; }
        [JsonProperty("caravanAsset")]
        public CaravanDetails CaravanAsset { get; set; }
        [JsonProperty("petAsset")]
        public PetDetails PetAsset { get; set; }
        [JsonProperty("boatAsset")]
        public BoatDetails BoatAsset { get; set; }
        [JsonProperty("homeAsset")]
        public HomeDetails HomeAsset { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("policyholder")]
        public PolicyholderDetails Policyholder { get; set; }
        [JsonProperty("policyCoOwners")]
        public List<PolicyholderDetails> PolicyCoOwners { get; set; }
        [JsonProperty("covers")]
        public List<ParentCoverDetails> Covers { get; set; }
        [JsonProperty("discountGroup")]
        public string DiscountGroup { get; set; }
        [JsonProperty("productType")]
        public ShieldProductType ProductType { get; set; }
        [JsonProperty("policyEndDate")]
        public DateTime PolicyEndDate { get; set; }
        [JsonProperty("policyStartDate")]
        public DateTime PolicyStartDate { get; set; }
        [JsonProperty("proposalDate")]
        public DateTime InitialPolicyStartDate { get; set; }
        [JsonProperty("proposalValidDate")]
        public DateTime ProposalValidDate { get; set; }
        [JsonProperty("endorsementStartDate")]
        public DateTime EndorsementStartDate { get; set; }
        [JsonProperty("endorsementEndDate")]
        public DateTime EndorsementEndDate { get; set; }
        [JsonProperty("statusRenewalDate")]
        public DateTime StatusRenewalDate { get; set; }
        [JsonProperty("renewalDate")]
        public DateTime RenewalDate { get; set; }
        [JsonProperty("paymentFrequency")]
        public string PaymentFrequency { get; set; }
        [JsonProperty("isInRenewal")]
        public bool IsInRenewal { get; set; }
        [JsonProperty("paymentMethod")]
        public string PaymentMethod { get; set; }
        [JsonProperty("preferredCollectionDay")]
        public int PreferredCollectionDay { get; set; }
        [JsonProperty("productVersionId")]
        public string ProductVersionId { get; set; }

        public int ProductVersionAsInteger => int.Parse(ProductVersionId);

        public bool HasHireCarCover => Covers[0].ChildCovers.Any(x => x.CoverType == Constants.PolicyMotor.ChildCovers.HireCarAfterAccident);

        /// <summary>
        /// returns enumeration representation of payment frequency.
        /// We generally expect it to be either annual or monthly.
        /// If Shield has returned an unrecognised value, we assume
        /// semi-annual.
        /// </summary>
        /// <exception cref="NotImplementedException">Thrown if the payment frequency returned from Shield is not implemented for B2C/Spark cases.</exception>
        public PaymentFrequency GetPaymentFrequency()
        {
            PaymentFrequency paymentFrequency;
            switch (PaymentFrequency)
            { 
                case "1":
                    paymentFrequency = Constants.PolicyGeneral.PaymentFrequency.Annual;
                    break;
                case "12":
                    paymentFrequency = Constants.PolicyGeneral.PaymentFrequency.Monthly;
                    break;
                default:
                    throw new NotImplementedException(PaymentFrequency + " is not implemented yet.");
             }
            return paymentFrequency;
        }

        /// <summary>
        /// Returns whether the policy is paid by bank account
        /// or not
        /// </summary>
        /// <returns></returns>
        public bool IsPaidByBankAccount()
        {
            var isBankAccount = true;
            switch(PaymentMethod)
            {
                // NOTE: I've left these hard coded as they're only used here for the moment.
                case "DD":
                    // do nothing, we've defaulted to true.
                    break;
                case "1":
                case "4":
                    isBankAccount = false;
                    break;
                default:
                    Reporting.Error("This method has errored because your test has queried whether the policy is paid by bank account, but the Shield returned code does not match the expected options of Bank Account/Cash/Credit Card. Please check if automation needs to be updated.");
                    break;
            }
            return isBankAccount;
        }

        /// <summary>
        /// Returns the human readable name of the financier,
        /// pulled from the relevant asset.
        /// </summary>
        /// <returns>Legal name of financier, or null if no financier information found</returns>
        public string GetFinancierNameViaShieldAPI()
        {
            string financierId = null;

            if (BoatAsset != null)
            { financierId = BoatAsset.FinancierExternalContactId.FirstOrDefault(); }
            if (CaravanAsset != null && CaravanAsset.IsFinanced)
            { financierId = CaravanAsset.FinancierExternalContactId.FirstOrDefault(); }
            if (HomeAsset != null && HomeAsset.IsFinanced)
            { financierId = HomeAsset.FinancierExternalContactId.FirstOrDefault(); }
            if (MotorAsset != null && MotorAsset.IsFinanced)
            { financierId = MotorAsset.FinancierExternalContactId.FirstOrDefault(); }
            if (MotorcycleAsset != null && MotorcycleAsset.IsFinanced)
            { financierId = MotorcycleAsset.FinancierExternalContactId.FirstOrDefault(); }

            return string.IsNullOrEmpty(financierId) ? null :
                   DataHelper.GetContactDetailsViaExternalContactNumber(financierId).LegalEntityName;
        }

        /// <summary>
        /// Returns vehichleID of the asset
        /// </summary>
        /// 
        public string GetVehicleId()
        {
            string vehicleId = null;

            if (MotorAsset != null)
            { vehicleId = MotorAsset.VehicleId; }
            if (MotorcycleAsset != null)
            { vehicleId = MotorcycleAsset.VehicleId; }
            if (CaravanAsset != null)
            { vehicleId = CaravanAsset.VehicleId; }

            return string.IsNullOrEmpty(vehicleId) ? null : vehicleId;
        }

        public List<KeyValuePair<string, decimal>> SumInsured(ShieldProductType type)
        {
            if (Covers == null || Covers.Count < 1)
            { Reporting.Error("Requested quote/policy has no covers to link sum insured amounts to."); }

            var insuredAmounts = new List<KeyValuePair<string, decimal>>();
            switch(type)
            {
                case ShieldProductType.MGP:
                    if (MotorAsset == null)
                    { Reporting.Error("Requested insured amount for a motor car policy, but the asset is missing from payload."); }
                    insuredAmounts.Add(new KeyValuePair<string, decimal>(Covers[0].CoverType, MotorAsset.TotalInsuredValue));
                    break;
                case ShieldProductType.MGC:
                    if (MotorcycleAsset == null)
                    { Reporting.Error("Requested insured amount for a motorcycle policy, but the asset is missing from payload."); }
                    insuredAmounts.Add(new KeyValuePair<string, decimal>(Covers[0].CoverType, MotorcycleAsset.TotalInsuredValue));
                    break;
                case ShieldProductType.MGV:
                    if (CaravanAsset == null)
                    { Reporting.Error("Requested insured amount for a Caravan policy, but the asset is missing from payload."); }
                    foreach(var cover in Covers)
                    {
                        insuredAmounts.Add(new KeyValuePair<string, decimal>(cover.CoverType, cover.SumInsured));
                    }
                    break;
                default:
                    Reporting.Error($"Retrieving sum insured for type ${type.GetDescription()} is not yet implemented.");
                    break;
            }
            return insuredAmounts;
        }

        public List<KeyValuePair<string, int>> Excess()
        {
            if (Covers == null || Covers.Count < 1)
            { Reporting.Error("Requested quote/policy has no covers to link excess amounts to."); }

            var excesses = new List<KeyValuePair<string, int>>();
            foreach(var parentCover in Covers)
            {
                excesses.Add(new KeyValuePair<string, int>(parentCover.CoverType, Decimal.ToInt32(parentCover.StandardExcess)));
            }
            return excesses;
        }

        public InstallmentDetails NextPendingInstallment()
        {
            var installmentList = new List<InstallmentDetails>(Installments.OrderBy(x => x.CollectionDate));
            var selectedInstallment = installmentList.Find(p => p.Status.Equals(Constants.PolicyGeneral.Status.Pending.GetDescription()));

            return selectedInstallment;
        }

        #region Home Cover related searches
        public bool HasBuildingCover()
        {
            return Covers.Any(x => x.CoverType == HomeCoverCodes.HB.GetDescription() ||
                                   x.CoverType == HomeCoverCodes.LB.GetDescription());
        }

        public bool HasContentsCover()
        {
            return Covers.Any(x => x.CoverType == HomeCoverCodes.HCN.GetDescription() ||
                                   x.CoverType == HomeCoverCodes.LCN.GetDescription() ||
                                   x.CoverType == HomeCoverCodes.RCN.GetDescription());
        }

        public bool HasOwnerOccupyCover()
        {
            return Covers.Any(x => x.CoverType == HomeCoverCodes.HB.GetDescription() ||
                                   x.CoverType == HomeCoverCodes.HCN.GetDescription());
        }

        public bool HasLandLordCover()
        {
            return Covers.Any(x => x.CoverType == HomeCoverCodes.LB.GetDescription() ||
                                   x.CoverType == HomeCoverCodes.LCN.GetDescription());
        }

        public bool HasAccidentalDamageCover()
        {
            return Covers.Any(x => x.CoverType == HomeCoverCodes.AD.GetDescription());
        }

        // OVS - Personal Valuables Specified when creating Home policy
        public bool HasOVSCover()
        {
            return Covers.Any(x => x.CoverType == HomeCoverCodes.OVS.GetDescription());
        }
        // OVU - Personal Valuables Unspecified when creating Home policy
        public bool HasOVUCover()
        {
            return Covers.Any(x => x.CoverType == HomeCoverCodes.OVU.GetDescription());
        }

        public decimal HomeSumInsuredBuilding()
        {
            var cover = Covers.FirstOrDefault(x => x.CoverType == HomeCoverCodes.HB.GetDescription() ||
                                                   x.CoverType == HomeCoverCodes.LB.GetDescription());
            if (cover == null) { Reporting.Error($"{PolicyNumber} does not have any building covers. Check that test data is correct."); }
            return cover.SumInsured;
        }

        public decimal HomeSumInsuredContents()
        {
            var cover = Covers.FirstOrDefault(x => x.CoverType == HomeCoverCodes.HCN.GetDescription() ||
                                                   x.CoverType == HomeCoverCodes.LCN.GetDescription() ||
                                                   x.CoverType == HomeCoverCodes.RCN.GetDescription());
            if (cover == null) { Reporting.Error($"{PolicyNumber} does not have any contents covers. Check that test data is correct."); }
            return cover.SumInsured;
        }
        #endregion
    }

    public class RealTimePaymentDetails
    {
        [JsonProperty("totalPayableAmount")]
        public decimal TotalPayableAmount { get; set; }
        [JsonProperty("isEligibleForRealTime")]
        public bool IsEligibleForRealTime { get; set; }

    }

    public class PremiumDetails
    {
        [JsonProperty("totalPremiumMonthly")]
        public decimal TotalPremiumMonthly { get; set; }
        [JsonProperty("total")]
        public decimal Total { get; set; }
        [JsonProperty("stampDuty")]
        public decimal StampDuty { get; set; }
        [JsonProperty("gst")]
        public decimal Gst { get; set; }
        [JsonProperty("baseAmount")]
        public decimal BaseAmount { get; set; }
    }

    public class AmountDetails
    {
        [JsonProperty("total")]
        public decimal Total { get; set; }
    }

    public class MotorcarDetails
    {
        [JsonProperty("totalInsuredValue")]
        public decimal TotalInsuredValue { get; set; }
        [JsonProperty("sumInsured")]
        public decimal SumInsured { get; set; }
        [JsonProperty("vehicleUsage")]
        public string VehicleUsage { get; set; }
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }
        [JsonProperty("suburb")]
        public string Suburb { get; set; }
        [JsonProperty("isMaxNcbLevel")]
        public bool IsMaxNcbLevel { get; set; }
        [JsonProperty("hasNcbProtection")]
        public bool HasNcbProtection { get; set; }
        [JsonProperty("ncbLevel")]
        public string NcbLevel { get; set; }
        [JsonProperty("isFinanced")]
        public bool IsFinanced { get; set; }
        [JsonProperty("financeCompanies")]
        public List<string> FinancierExternalContactId { get; set; }
        [JsonProperty("modificationType")]
        public string ModificationType { get; set; }
        [JsonProperty("drivers")]
        public List<DriversDetails> Drivers { get; set; }
        [JsonProperty("address")]
        public Address Address { get; set; }
        [JsonProperty("registrationNumber")]
        public string RegistrationNumber { get; set; }
        [JsonProperty("annualKms")]
        public string AnnualKms { get; set; }
        
        public VehicleUsage GetVehicleUsage() => VehicleUsageNameMappings.First(x => x.Value.TextShield.Equals(VehicleUsage, StringComparison.OrdinalIgnoreCase)).Key;

        /// <summary>
        /// This will return the AnnualKms description
        /// </summary>
        public string GetAnnualKms
        {
            get
            {
                switch (AnnualKms)
                {
                    case "LT5000":
                        return Constants.PolicyGeneral.AnnualKms.LessThan5000.GetDescription();
                    case "10000":
                        return Constants.PolicyGeneral.AnnualKms.UpTo10000.GetDescription();
                    case "15000":
                        return Constants.PolicyGeneral.AnnualKms.UpTo15000.GetDescription();
                    case "20000":
                        return Constants.PolicyGeneral.AnnualKms.UpTo20000.GetDescription();
                    case "GT20000":
                        return Constants.PolicyGeneral.AnnualKms.MoreThan20000.GetDescription();
                    default:
                        return Constants.PolicyGeneral.AnnualKms.UpTo10000.GetDescription();
                }
            }
        }

        public bool IsVehicleModified()
        {
            var hasModification = true;
            if (string.IsNullOrEmpty(ModificationType) ||
                string.Equals(ModificationType, "n", StringComparison.InvariantCultureIgnoreCase))
            { hasModification = false; }
            return hasModification;
        }

        /// <summary>
        /// Method that returns the vehicle Address, but converted into a string
        /// that can be used in onscreen verification. For policies where a full
        /// QAS address is not known (pre-MRA policies), then just the suburb
        /// is returned.
        /// </summary>
        /// <param name="expandUnitAddresses">If TRUE, will provide unit addresses as "unit A B Main St". Otherwise they are returned as "A/B Main St"</param>
        public string RiskLocation(bool expandUnitAddresses)
        {
            if (Address == null)
            { return Suburb; }

            return expandUnitAddresses ? Address.StreetSuburbState() :
                                         Address.StreetSuburbStateShortened(longStreetType: false);
        }
    }

    public class MotorcycleDetails
    {
        [JsonProperty("totalInsuredValue")]
        public decimal TotalInsuredValue { get; set; }
        [JsonProperty("vehicleUsage")]
        public string VehicleUsage { get; set; }
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }
        [JsonProperty("suburb")]
        public string Suburb { get; set; }
        [JsonProperty("isGaraged")]
        public bool IsGaraged { get; set; }
        [JsonProperty("isDashcamUsed")]
        public bool IsDashcamUsed { get; set; }
        [JsonProperty("isFinanced")]
        public bool IsFinanced { get; set; }
        [JsonProperty("financeCompanies")]
        public List<string> FinancierExternalContactId { get; set; }
        [JsonProperty("annualKms")]
        public AnnualKms AnnualKms { get; set; }
        [JsonProperty("modificationType")]
        public string ModificationType { get; set; }
        [JsonProperty("Security")]
        public string Security { get; set; }
        [JsonProperty("drivers")]
        public List<DriversDetails> Drivers { get; set; }
        [JsonProperty("registrationNumber")]
        public string RegistrationNumber { get; set; }
        public bool IsModified() => !string.IsNullOrEmpty(ModificationType) && ModificationType != SHIELD_API_MOTORCYCLE_NOT_MODIFIED;
    }

    public class CaravanDetails
    {
        [JsonProperty("totalInsuredValue")]
        public decimal TotalInsuredValue { get; set; }
        [JsonProperty("vehicleUsage")]
        public string VehicleUsage { get; set; }
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }
        [JsonProperty("suburb")]
        public string Suburb { get; set; }
        [JsonProperty("postcode")]
        public string Postcode { get; set; }
        [JsonProperty("isFinanced")]
        public bool IsFinanced { get; set; }
        [JsonProperty("financeCompanies")]
        public List<string> FinancierExternalContactId { get; set; }
        [JsonProperty("drivers")]
        public List<DriversDetails> Drivers { get; set; }
        [JsonProperty("registrationNumber")]
        public string RegistrationNumber { get; set; }
        [JsonProperty("policyType")]
        public string PolicyType { get; set; }
    }

    public class PetDetails
    {
        [JsonProperty("isAnotherPetInsured")]
        public bool IsAnotherPetInsured { get; set; }
        [JsonProperty("petGender")]
        public string PetGender { get; set; }
        [JsonProperty("assetTypeId")]
        public string AssetTypeId { get; set; }
        [JsonProperty("updateVersion")]
        public string UpdateVersion { get; set; }
        [JsonProperty("isVetRegistered")]
        public bool IsVetRegistered { get; set; }
        [JsonProperty("petType")]
        public string PetType { get; set; }
        [JsonProperty("postcode")]
        public string Postcode { get; set; }
        [JsonProperty("isPreExistingIllness")]
        public bool IsPreExistingIllness { get; set; }
        [JsonProperty("vetDetails")]
        public string VetDetails { get; set; }
        [JsonProperty("petName")]
        public string PetName { get; set; }
        [JsonProperty("isSterilized")]
        public bool IsSterilized { get; set; }
        [JsonProperty("assetExternalNumber")]
        public string AssetExternalNumber { get; set; }
        [JsonProperty("possessionDate")]
        public string PossessionDate { get; set; }
        [JsonProperty("petBreed")]
        public string PetBreed { get; set; }
        [JsonProperty("suburb")]
        public string Suburb { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("petDob")]
        public string PetDOB { get; set; }
    }


    public class BoatDetails
    {
        [JsonProperty("skipperExperience")]
        public string SkipperExperience { get; set; }
        [JsonProperty("boatType")]
        public string BoatType { get; set; }
        [JsonProperty("make")]
        public string Make { get; set; }
        [JsonProperty("othermake")]
        public string OtherMake { get; set; }
        [JsonProperty("builtYear")]
        public string BuiltYear { get; set; }
        [JsonProperty("hullConstruction")]
        public string HullConstruction { get; set; }
        [JsonProperty("agreedValue")]
        public string AgreedValue { get; set; }
        [JsonProperty("isFinanced")]
        public string IsFinanced { get; set; }
        [JsonProperty("financeCompanies")]
        public List<string> FinancierExternalContactId { get; set; }
        [JsonProperty("suburb")]
        public string Risk_suburb { get; set; }
        [JsonProperty("postcode")]
        public string Risk_postcode { get; set; }
        [JsonProperty("isGaraged")]
        public string IsGaraged { get; set; }
        [JsonProperty("motorType")]
        public string MotorType { get; set; }
        [JsonProperty("isSecurityAlarmGps")]
        public string IsSecurityAlarmGps { get; set; }
        [JsonProperty("isSecurityNebo")]
        public string IsSecurityNebo { get; set; }
        [JsonProperty("isSecurityHitch")]
        public string IsSecurityHitch { get; set; }
        [JsonProperty("boatRegistration")]
        public string BoatRegistration { get; set; }
        [JsonProperty("trailerRegistration")]
        public string TrailerRegistration { get; set; }
    }

    public class HomeDetails
    {
        [JsonProperty("constructionType")]
        public string ConstructionType { get; set; }
        [JsonProperty("isWindowSecurityApplied")]
        public bool   IsWindowSecurityApplied { get; set; }
        [JsonProperty("isDoorSecurityApplied")]
        public bool   IsDoorSecurityApplied { get; set; }
        [JsonProperty("constructionYear")]
        public int    ConstructionYear { get; set; }
        [JsonProperty("isLargeProperty")]
        public bool   IsLargeProperty { get; set; }
        [JsonProperty("isFinanced")]
        public bool   IsFinanced { get; set; }
        [JsonProperty("financeCompanies")]
        public List<string> FinancierExternalContactId { get; set; }
        [JsonProperty("occupancyType")]
        public string OccupancyType { get; set; }
        [JsonProperty("roofMaterial")]
        public string RoofMaterial { get; set; }
        [JsonProperty("valuableType")]
        public string ValuableType { get; set; }
        [JsonProperty("alarmType")]
        public string AlarmType { get; set; }
        [JsonProperty("policyType")]
        public string PolicyType { get; set; }
        [JsonProperty("buildingType")]
        public string BuildingType { get; set; }
        [JsonProperty("address")]
        public Address Address { get; set; }
        [JsonProperty("isCycloneProneArea")]
        public bool IsCycloneProneArea { get; set; }
        [JsonProperty("isPropertyElevated")]
        public string IsPropertyElevated { get; set; }
        [JsonProperty("hasCycloneShutters")]
        public string HasCycloneShutters { get; set; }
        [JsonProperty("garageDoorUpgraded")]
        public string GarageDoorUpgraded { get; set; }
        [JsonProperty("roofImprovement")]
        public string RoofImprovement { get; set; }
        [JsonProperty("assetItems")]
        public List<AssetItem> AssetItems { get; set; }

        public bool GetIsPropertyElevated => string.Equals(IsPropertyElevated, "Y");

        public bool GetHasCycloneShutters => string.Equals(HasCycloneShutters, "Y");

        public GarageDoorsUpgradeStatus GetGarageDoorUpgradeStatus
        {
            get
            {
                switch (GarageDoorUpgraded)
                {
                    case "CYLGDS":
                        return GarageDoorsUpgradeStatus.ReplacedToCyclone;
                    case "CYLGDBU":
                        return GarageDoorsUpgradeStatus.BracingUpgrade;
                    case "CYLGDNU":
                        return GarageDoorsUpgradeStatus.NoUpgrade;
                    case "CYLNGD":
                        return GarageDoorsUpgradeStatus.NoGarageDoor;
                    case "CYLGDUN":
                        return GarageDoorsUpgradeStatus.NotSure;
                    default:
                        return GarageDoorsUpgradeStatus.NoInformation;
                }
            }
        }

        public RoofImprovementStatus GetRoofImprovementStatus
        {
            get
            {
                switch (RoofImprovement)
                {
                    case "CLYRIRRTDU":
                        return RoofImprovementStatus.CompleteRoofReplacement;
                    case "CLYRITDU":
                        return RoofImprovementStatus.TiedownUpgrade;
                    case "CLYNRI":
                        return RoofImprovementStatus.NoImprovement;
                    case "CLYRIUN":
                        return RoofImprovementStatus.NotSure;
                    default:
                        return RoofImprovementStatus.NoInformation;
                }
            }
        }
    }

    public class PolicyholderDetails
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("contactExternalNumber")]
        public string ContactExternalNumber { get; set; }
    }

    public class NextPayableInstallment
    {
        [JsonProperty("installmentNumber")]
        public int InstallmentNumber { get; set; }

        [JsonProperty("outstandingAmount")]
        public double OutstandingAmount { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("collectionDate")]
        public DateTime CollectionDate { get; set; }
    }

    public class ParentCoverDetails : CoverDetails
    {
        [JsonProperty("coverTypeDescription")]
        public string CoverTypeDescription { get; set; }
        [JsonProperty("standardExcess")]
        public decimal StandardExcess { get; set; }
        [JsonProperty("childCovers")]
        public List<CoverDetails> ChildCovers { get; set; }
    }

    public class CoverDetails
    {
        [JsonProperty("coverType")]
        public string CoverType { get; set; }
        [JsonProperty("sumInsured")]
        public decimal SumInsured { get; set; }
    }

    public class InstallmentDetails
    {
        [JsonProperty("amount")]
        public AmountDetails Amount { get; set; }
        [JsonProperty("collectionDate")]
        public DateTime CollectionDate { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
    }

    public class DriversDetails
    {
        [JsonProperty("driverExperience")]
        public string DriverExperience { get; set; }
        [JsonProperty("systemImposedExcess")]
        public double SystemImposedExcess { get; set; }
        [JsonProperty("contactExternalNumber")]
        public string ContactExternalNumber { get; set; }
    }
    public class AssetItem
    {
        [JsonProperty("itemAmount")]
        public double ItemAmount { get; set; }
        [JsonProperty("assetItemType")]
        public int AssetItemType { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
