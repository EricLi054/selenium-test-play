using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.PolicyBoat;
using static Rac.TestAutomation.Common.Constants.PolicyCaravan;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyHome;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;
using static Rac.TestAutomation.Common.Constants.PolicyPet;

namespace Rac.TestAutomation.Common
{
    public class PolicyCoverDetails
    {
        /// <summary>
        /// Homeowner Building, Landlords Contents, etc
        /// </summary>
        public string CoverDescription { get; set; }
        /// <summary>
        /// HB, LCN, etc. These are Shield internal codes
        /// for cover type.
        /// </summary>
        public HomeCoverCodes CoverCode { get; set; }
        public string Excess { get; set; }
        public int SumInsured { get; set; }
    }

    public class PolicyPaymentDetails
    {
        public string PaymentMethod { get; set; }
        public PaymentFrequency PaymentFrequency { get; set; }
        public int    PaymentCount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        public decimal PaymentTotal { get; set; }
    }

    public class PolicyEndorsementCancellation
    {
        public int PolicyStatusId { get; set; }
        public DateTime CancellationEffectiveDate { get; set; }
        public int InitiatorId { get; set; }
        public string PolicyEndorsementExternalCode { get; set; }
        public decimal FinalInstallment { get; set; }
        public string Email { get; set; }
        public bool PrintEventGenerated { get; set; }
    }

    public class PolicyEndorsementInstalmentUpdate
    {
        public string PolicyEndorsementIdentifier { get; set; }
        public string PolicyEndorsementExternalCode { get; set; }
        public string Email { get; set; }
        public bool PrintEventGenerated { get; set; }
        public bool PaymentChangeEventGenerated { get; set; }
        public bool EndorsementForDebitAdmendment { get; set; }
        public int EventCount { get; set; }
    }

    public class PolicyEventDetails
    {
        public DateTime EventDate { get; set; }
        public string EventType { get; set; }
        public string EventDocType { get; set; }
    }

     public class PolicyInstalmentDetails
    {
        public decimal AmountDue { get; set; }
        public DateTime OriginalCollectionDate { get; set; }
        public DateTime CurrentCollectionDate { get; set; }
        public BankAccount BankAccount { get; set; }
        public CreditCard CreditCard { get; set; }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine("-------------------------------------<br>");
            formattedString.AppendLine("--- Instalment Information:<br>");
            formattedString.AppendLine($"    Original Collection Date:  {OriginalCollectionDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH)}<br>");
            formattedString.AppendLine($"    Current Collection Date:   {CurrentCollectionDate.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH)}<br>");

            if(BankAccount != null)
            {
                formattedString.AppendLine($"--- Bank Account<br>");
                formattedString.AppendLine($"    Name:             {BankAccount.AccountName}<br>");
                formattedString.AppendLine($"    BSB:              {BankAccount.Bsb}<br>");
                formattedString.AppendLine($"    Number:           {BankAccount.AccountNumber}<br>");
                formattedString.AppendLine("-------------------------------------<br>");
            }

            if(CreditCard != null)
            {
                formattedString.AppendLine($"--- Credit Card<br>");
                formattedString.AppendLine($"    Card Holder Name: {CreditCard.CardholderName}<br>");
                formattedString.AppendLine($"    Last Four Digits: {CreditCard.CardNumber.Substring(12)}<br>");
                formattedString.AppendLine($"    Expiry Date:      {CreditCard.CardExpiryDate.ToString(DataFormats.DATE_MONTH_YEAR_FORWARDSLASH)}<br>");
                formattedString.AppendLine($"    Issuer:           {CreditCard.CardIssuer.GetDescription()}");
            }

            return formattedString.ToString();
        }

    }

    /// <summary>
    /// Data structure for DB query results relating to Motor Policies.
    /// </summary>
    public class MotorPolicy
    {
        public string   PolicyNumber { get; set; }
        public List <PolicyContactDB>  PolicyHolders { get; set; }
        public string   CoverType { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime LastEndorsementDate { get; set; }
        public int      Excess { get; set; }
        public int      SumInsured { get; set; }
        public decimal  AnnualPremium { get; set; }
        public Car      Vehicle { get; set; }
        public Address RiskAddress { get; set; }
        public bool HasHireCarCover { get; set; }

        public MotorCovers GetCoverType()
        {
            if (CoverType == "MFCO")
            {
                return MotorCovers.MFCO;
            }
            else if (CoverType == "TPO")
            {
                return MotorCovers.TPO;
            }
            else if (CoverType == "TFT")
            {
                return MotorCovers.TFT;
            }
            else
            {
                throw new NotSupportedException($"{CoverType} is not a valid motor cover type");
            }
        }
        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine("-------------------------------------<br>");
            formattedString.AppendLine("--- Policy data for claim test:<br>");
            formattedString.AppendLine($"    Policy:  {PolicyNumber} with {PolicyHolders.Count} contacts<br>");
            formattedString.AppendLine($"    Risk address:  {RiskAddress.StreetSuburbStatePostcode()}");
            foreach (var contact in PolicyHolders)
            {
                formattedString.AppendLine($"    PH/coPH: {contact.FirstName}<br>");
                formattedString.AppendLine($"    PH/coPH: {contact.Surname}<br>");
                formattedString.AppendLine($"    DoB:     {contact.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH)}<br>");
                formattedString.AppendLine($"    CID:     {contact.Id}<br>");
            }
            formattedString.AppendLine("-------------------------------------<br>");

            return formattedString.ToString();
        }
    }

    /// <summary>
    /// Data structure for DB query results relating to Motor Policies with previous claim number.
    /// </summary>
    public class MotorPolicyWithExistingClaim
    {
        public MotorPolicy MotorPolicy { get; set; }
        public string ClaimNumber { get; set; }
    }

    /// <summary>
    /// Data structure for DB query results relating to Home Policies.
    /// </summary>
    public class HomePolicy
    {
        public string PolicyNumber { get; set; }
        public List<PolicyContactDB>    PolicyHolders { get; set; }
        public List<ContentItem>        SpecifiedValuablesAndContents { get; set; }
        public HomeOccupancy            Occupancy { get; set; }
        public DateTime                 PolicyStartDate { get; set; }
        public List<PolicyCoverDetails> Covers { get; set; }
        public PolicyPaymentDetails     PaymentDetails { get; set; }
        public PolicyEventDetails       EventDetails { get; set; }
        public int                      WeeklyRental { get; set; }
        public HomePropertyManager?     PropertyManager { get; set; }
        public List<ClaimHistory>       DisclosedClaimsHistory { get; set; }
        public DateTime                 MainRenewalDate { get; set; }
        public decimal                  YearlyPremium { get; set; }
        public DateTime                 LastEndorsementDate { get; set; }
        public int                      EndorsementID { get; set; }
        public string                   TransactionType { get; set; }
        public decimal                  ExpectedPremium { get; set; }
        public string                   EndorsementType { get; set; }
        public string                   EndorsementReason { get; set; }
        public Home                     PropertyDetails { get; set; }

        public PolicyCoverDetails GetBuildingCover() =>
                            Covers.FirstOrDefault(x => x.CoverCode == HomeCoverCodes.HB ||
                                                       x.CoverCode == HomeCoverCodes.LB);

        public PolicyCoverDetails GetContentsCover() =>
                            Covers.FirstOrDefault(x => x.CoverCode == HomeCoverCodes.HCN ||
                                                       x.CoverCode == HomeCoverCodes.LCN ||
                                                       x.CoverCode == HomeCoverCodes.RCN);

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine("-------------------------------------<br>");
            formattedString.AppendLine("--- Policy data for claim test:<br>");
            formattedString.AppendLine($"    Policy:  {PolicyNumber} with {PolicyHolders.Count} contacts<br>");
            foreach (var contact in PolicyHolders)
            {
                formattedString.AppendLine($"    PH/coPH: {contact.Surname}<br>");
                formattedString.AppendLine($"    DoB:     {contact.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH)}<br>");
                formattedString.AppendLine($"    CID:     {contact.Id}<br>");
            }
            formattedString.AppendLine("-------------------------------------<br>");

            return formattedString.ToString();
        }
    }

    /// <summary>
    /// Collection of data that we retrieve from Shield DB to support
    /// validation of new Pet policies
    /// </summary>
    public class PetPolicy
    {
        public string PolicyNumber { get; set; }
        public DateTime PolicyholderDOB { get; set; }
        public int PolicyholderAge { get; set; }
        public PetType PetType { get; set; }
        public string PetBreed { get; set; }
        public string PetName { get; set; }
        public int ContactId { get; set; }
        public bool HasTLCCover { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public bool IsDirectDebit { get; set; }
        public PaymentFrequency PaymentFrequency { get; set; }
        public int InstallmentCount { get; set; }
        public bool HasPreExistIllness { get; set; }
    }

    public class BoatPolicy
    {
        public string PolicyNumber { get; set;}
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyEndDate { get; set; }
        public BoatType BoatType { get; set; }
    }

    public class CaravanPolicy
    {
        public string PolicyNumber { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyEndDate { get; set; }
        public Decimal PremiumAmount { get; set; }
        public Decimal InstallmentAmount { get; set; }
        public CaravanPolicyOutput CaravanPolicyOutput { get; set; }
        public CaravanPolicyInput CaravanPolicyInput { get; set; }
    }
    public class CaravanPolicyInput
    {
        public DateTime PolicyholderDOB { get; set; }
        public int PolicyholderAge { get; set; }
        public CaravanType CaravanType { get; set; }
        public string CaravanYear { get; set; }
        public string CaravanMake { get; set; }
        public string CaravanModel { get; set; }
        public string CoverType { get; set; }
        public int ContactId { get; set; }
        public bool HasCoveryMyAnnexe { get; set; }
        public bool IsDirectDebit { get; set; }
        public PaymentFrequency PaymentFrequency { get; set; }
    }

    public class CaravanPolicyOutput
    {
        public string RegistrationValue { get; set; }
        public string CaravanLocation { get; set; }
        public string Campaign_Code { get; set; }
        public string NCBLevel { get; set; }
        public string MemberNumber { get; set; }
        public string InstallmentType { get; set; }
        public string InstallmentPaymentTerms { get; set; }
        public string InstallmentNumber { get; set; }
        public string InstallmentCollectionMethod { get; set; }
        public string PolicyPaymentTerms { get; set; }
        public string PolicyCollectionMethod { get; set; }
        public string EmailType { get; set; }
    }

    public class PolicyContactDB : Contact
    {
        /// <summary>
        /// Initialise a PolicyContactDB object from a Contact object.
        /// </summary>
        public static PolicyContactDB Init(Contact contact)
        {
            PolicyContactDB policyContactDB = null;
            if (contact != null)
            {
                var inputObjectAsString = JsonConvert.SerializeObject(contact);
                policyContactDB = JsonConvert.DeserializeObject<PolicyContactDB>(inputObjectAsString);
            }
            return policyContactDB;
        }

        public List<ContactRole> ContactRoles { get; set; }
    }

    public class PolicyInstalmentDB
    {
        public Status   Status         { get; set; }
        public decimal  Amount         { get; set; }
        public DateTime CollectionDate { get; set; }
        public bool     IsRecurring    { get; set; }
    }

    public class MotorServiceProvider
    {
        public int ContactId { get; set; }
        public string ExternalContactNumber { get; set; }
        public string ProviderName { get; set; }
        public string ServiceArea { get; set; }
        /// <summary>
        /// 1 == Provider has this skill
        /// 0 == Provider does not have
        /// </summary>
        public int HasReadyDriveSkill { get; set; }
        /// <summary>
        /// 1 == Provider has this skill
        /// 0 == Provider does not have
        /// </summary>
        public int HasFreeHirecar { get; set; }
        /// <summary>
        /// Indicates if the service provider has the
        /// "Auto Authorise Repairer" role, which affects
        /// how claim agenda status is processed.
        /// </summary>
        public bool IsAutoAuthoriseRepairer { get; set; }
    }

    public class ClaimAgendaStatus
    {
        public string DamageCode { get; set; }
        public string Step       { get; set; }
        public string Status     { get; set; }
    }

    public class ClaimHomeDamageDetails
    {
        public string DamageCode { get; set; }
        public string ReserveAmount { get; set; }
        public string DamageStatus { get; set; }
    }

    /// <summary>
    /// Specific to Motor Glass Only claims, where a glazier
    /// has been auto allocated. Expected 4 event items to
    /// be returned and a general event description
    /// </summary>
    public class ClaimEventDetails
    {
        public string Event1 { get; set; }
        public string Event2 { get; set; }
        public string Event3 { get; set; }
        public string Event4 { get; set; }
        public string EventDescription { get; set; }
    }

    public class ClaimDetails
    {
        public string ClaimType { get; set; }
        public string ClaimScenario { get; set; }
        public DateTime EventDateAndTime { get; set; }
        public string PoliceReportNumber { get; set; }
        public DateTime PoliceReportDate { get; set; }
    }

    public class InboundClaimCorrespondence
    {
        public string FileName { get; set; }
        public string DocType { get; set; }
        public string FileType { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActionable { get; set; }
        public string Remarks { get; set; }
    }

    public class PolicyEndorsementReason
    {
        public string Id { get; set; }

        /// <summary>
        /// Also known as the endorsement reason from Shield.
        /// </summary>
        public string Description { get; set; }

        public string Remarks { get; set; }

        public string EndorsementType { get; set; }
    }

    public class ShieldEnvironmentToggle
    {
        public string Value { get; set; }
        public string Id { get; set; }
        public string Param_desc { get; set; }
        public string Param_dev_desc { get; set; }
        public string Parameter_type { get; set; }
        public string Parameter_dev_desc { get; set; }
    }
}