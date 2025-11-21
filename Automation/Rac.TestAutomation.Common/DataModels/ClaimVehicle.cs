using Rac.TestAutomation.Common.API;
using System;
using System.Text;
using System.Collections.Generic;
using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using System.Linq;

namespace Rac.TestAutomation.Common
{
    public class ClaimCar
    {
        public MotorPolicy Policy { get; set; }
        public GetClaimResponse ExistingClaim { get; set; }
        public DateTime    EventDateTime { get; set; }
        public MotorClaimDamageType DamageType { get; set; }
        /// <summary>
        /// PolicyHolder / Representative who will lodge claim.
        /// </summary>
        public PolicyContactDB Claimant { get; set; }
        /// <summary>
        /// Number of vehicles involved in the accident to support Single/Multiple collision motor claim scenarios
        /// </summary>
        public MotorCollisionNumberOfVehiclesInvolved NumberOfVehiclesInvolved { get; set; }

        public string ClaimNumber { get; set; }
        public string      EventLocation { get; set; }
        public bool?       IsTPPropertyDamage { get; set; }
        public bool        OnlyClaimDamageToTP { get; set; }       
        public bool?       IsVehicleDriveable { get; set; }       
        /// <summary>
        /// The vehicle tow details
        /// </summary>      
        public TowDetails TowedVehicleDetails { get; set; }
        public Address PreferredRepairerSuburb { get; set; }        
        public RepairerOption RepairerOption { get; set; }
        /// <summary>
        /// AssignedRepairer will hold the selected repairer's name/phone when 
        /// that stage of the claims flow is reached and the automation chooses it.
        /// </summary>
        public BusinessDetails AssignedRepairer { get; set; }
        /// <summary>
        /// If the reapirer allocation tool exhausted for the claim 
        /// We set it true if we had no repairers returned by the AssignServiceProvider call for this 
        /// claim/suburb, as we have to conclude that no repairers are available for allocation. 
        /// </summary>
        public bool IsRepairerAllocationExhausted { get; set; }
        /// <summary>
        /// ExpectCompleteLodgementDone is determined after claim is completed 
        /// and is a product of whether the vehicle was driveable after the accident, 
        /// and if there are any payment blocks on the policy.
        /// </summary>
        public bool ExpectCompleteLodgementDone { get; set; }
        public TheftDetails TheftDetails { get; set; }
        public GlassDamageDetails           GlassDamageDetails { get; set; }
        public TravelDirection    DirectionBeingTravelled { get; set; }
        public MotorClaimScenario ClaimScenario       { get; set; }
        public string DamageToPHVehicle   { get; set; }
        public string AccountOfAccident   { get; set; }       
        /// <summary>
        /// To set no police involvement, leave this null
        /// </summary>
        public bool?    IsPoliceInvolved { get; set; }
        public bool     IsPoliceReportNumberAvailable { get; set; }
        public string   PoliceReportNumber { get; set; }
        public DateTime PoliceReportDate { get; set; }
        public ContactClaimDriver        Driver { get; set; }
        public List<ContactClaimMotorTP> ThirdParty { get; set; }
        public List<Contact>             Witness { get; set; }
        public LoginWith                 LoginWith { get; set; }        
        public List<MotorPolicy>         LinkedMotorPoliciesForClaimant { get; set; }
        public ClaimUploadFile ClaimUploadFile { get; set; }       
        /// <summary>
        /// When policy have an unpaid installment 
        /// we expect the claim should have a payment block
        /// </summary>
        public bool expectClaimPaymentBlock { get; set; }
        public ShieldClaimScenario ShieldDamageType { get; set; }
        /// <summary>
        /// For Motor claims, its check if new mobile number provided
        /// </summary>
        public bool IsMobileNumberChanged
        {
            get
            {
                if (string.IsNullOrEmpty(Claimant.NewMobilePhoneNumber)) { return false; }

                if (!string.IsNullOrEmpty(Claimant.MobilePhoneNumber) &&
                    Claimant.MobilePhoneNumber.Equals(Claimant.NewMobilePhoneNumber))
                { return false; }

                return true;
            }
        }

        /// <summary>
        /// For Motor claims, its check if new email provided
        /// </summary>      
        public bool IsEmailAddressChanged
        {
            get
            {
                if (string.IsNullOrEmpty(Claimant.PrivateEmail?.NewAddress)) { return false; }

                if (Claimant.PrivateEmail != null)
                {
                    if (!string.IsNullOrEmpty(Claimant.PrivateEmail.Address) &&
                    (Claimant.PrivateEmail.Address.Equals(Claimant.PrivateEmail.NewAddress)))
                    { return false; }
                }

                return true;
            }
        }

        /// <summary>
        /// If the driver and claimant both are same, then will return true 
        /// </summary>      
        public bool IsClaimantDriver
        {
            get
            {
                if ((Claimant.FirstName == Driver.DriverDetails.FirstName) && (Claimant.Surname == Driver.DriverDetails.Surname))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        // Driver History screen is not displayed on hit whilst parked
        /// </summary>      
        public bool IsQualifiedForDriverHistoryQuestionnaire
        {
            get
            {
                if (ClaimScenario == MotorClaimScenario.WhileParkedAnotherCarHitMyCar && !OnlyClaimDamageToTP)
                {
                    return false;
                }

                return true;
            }
        }

        public override string ToString()
        {
            string fullNameString = Claimant.GetFullTitleAndName();
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"-------------Motor Claim data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"                Policy number: {Policy.PolicyNumber}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"          Claimant Contact ID: {Claimant.Id}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"                Claimant Name: {fullNameString}{Reporting.HTML_NEWLINE}");
            if (IsMobileNumberChanged)
            {
                formattedString.AppendLine($"             New Phone number: {Claimant.NewMobilePhoneNumber}{Reporting.HTML_NEWLINE}");
            }
            if (IsEmailAddressChanged)
            {
                formattedString.AppendLine($"            New Email address: {Claimant.PrivateEmail.NewAddress}{Reporting.HTML_NEWLINE}");
            }
            formattedString.AppendLine($"               Event datetime: {EventDateTime.ToString(DataFormats.DATE_FORMAT_FORWARD_FORWARDSLASH)} {EventDateTime.ToString(DataFormats.TIME_FORMAT_24HR)}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"                 Claim Damage: {MotorClaimDamageTypeNames[DamageType].TextB2C}{Reporting.HTML_NEWLINE}");            
            formattedString.AppendLine($"               Claim Scenario: {ClaimScenario}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Third Party Asset Damaged: {IsTPPropertyDamage}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Only Claim TP Asset Damage: {OnlyClaimDamageToTP}{Reporting.HTML_NEWLINE}");
            if (Driver.DriverDetails != null)
            {
                formattedString.AppendLine($"---------------Driver details:{Reporting.HTML_NEWLINE}");                
                formattedString.AppendLine($"            Driver Contact ID: {Driver.DriverDetails.Id}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"                  Driver Name: {Driver.DriverDetails.FirstName}  {Driver.DriverDetails.Surname}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"               Driver Address: {Driver.DriverDetails.MailingAddress.StreetSuburbStatePostcode()}{Reporting.HTML_NEWLINE}");
            }

            if (TowedVehicleDetails != null)
            {
                var vehicleTowed = TowedVehicleDetails.WasVehicleTowed == true ? true : false;
                formattedString.AppendLine($"------------Was vehicle towed: {vehicleTowed}{Reporting.HTML_NEWLINE}");
                if (TowedVehicleDetails.WasVehicleTowed == true)
                {
                    formattedString.AppendLine($"-------------Vehicle towed to: {TowedVehicleDetails.TowedTo.GetDescription()}{Reporting.HTML_NEWLINE}");
                }
            }

            var isSparkVehicleDriveable = IsVehicleDriveable == true ? true : false;
            formattedString.AppendLine($"---------Was vehicle drivable: {isSparkVehicleDriveable}{Reporting.HTML_NEWLINE}");

            if (GlassDamageDetails != null)
            {
                formattedString.AppendLine($"-----------------Glass damage: Windscreen only? {GlassDamageDetails.FrontWindscreenOnly} - {GlassDamageDetails.TypeOfDamage.GetDescription()}{Reporting.HTML_NEWLINE}");
            }

            var thirdPartyCount = ThirdParty != null ? ThirdParty.Count : 0;
            formattedString.AppendLine($"------------------Third Party Count: {thirdPartyCount}{Reporting.HTML_NEWLINE}");
            if (thirdPartyCount == 1)
            {
                var thirdPartyDetails = ThirdParty.FirstOrDefault();
                formattedString.AppendLine($"------------------Third Party Details (single TP involved){Reporting.HTML_NEWLINE}");
                string thirdPartyFullNameString = thirdPartyDetails.GetFullTitleAndName();
                formattedString.AppendLine($"                Third Party First Name: {thirdPartyDetails.FirstName}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"                Third Party Last Name: {thirdPartyDetails.Surname}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"                Third Party Full Title & Name: {thirdPartyFullNameString}{Reporting.HTML_NEWLINE}");
                if (thirdPartyDetails.MobilePhoneNumber != null)
                {
                    formattedString.AppendLine($"                Third Party Mobile Number: {thirdPartyDetails.MobilePhoneNumber}{Reporting.HTML_NEWLINE}");
                }
                if (thirdPartyDetails.PrivateEmail.Address != null)
                {
                    formattedString.AppendLine($"                Third Party Email: {thirdPartyDetails.PrivateEmail.Address}{Reporting.HTML_NEWLINE}");
                }
                if (thirdPartyDetails.MailingAddress != null)
                {
                    formattedString.AppendLine($"                Third Party Mailing Address: {thirdPartyDetails.MailingAddress.StreetSuburbState()}{Reporting.HTML_NEWLINE}");
                }
                else
                { formattedString.AppendLine($"                Third Party Mailing Address: - {Reporting.HTML_NEWLINE}"); }
                if (thirdPartyDetails.Insurer != null)
                {
                    formattedString.AppendLine($"                Third Party Insurer: {thirdPartyDetails.Insurer.Name}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($"                Third Party Insurer External Contact Number: {thirdPartyDetails.Insurer.ExternalContactNumber}{Reporting.HTML_NEWLINE}");
                }
                if (thirdPartyDetails.ClaimNumber != null)
                {
                    formattedString.AppendLine($"                Third Party Claim Number: {thirdPartyDetails.ClaimNumber}{Reporting.HTML_NEWLINE}");
                }
                if (thirdPartyDetails.DescriptionOfDamageToVehicle != null)
                {
                    formattedString.AppendLine($"                Third Party Damage Description: {thirdPartyDetails.DescriptionOfDamageToVehicle}{Reporting.HTML_NEWLINE}");
                }
                formattedString.AppendLine($"                Third Party known to Policyholder before accident?: {thirdPartyDetails.IsKnownToClaimant}{Reporting.HTML_NEWLINE}");

                


                formattedString.AppendLine($"                Third Party Full Name: {thirdPartyFullNameString}{Reporting.HTML_NEWLINE}");
            }
            var witnessCount = Witness != null ? Witness.Count : 0;
            formattedString.AppendLine($"--------------------Witnesses Count: {witnessCount}{Reporting.HTML_NEWLINE}");

            if (isSparkVehicleDriveable)
            {
                formattedString.AppendLine($"-Preferred suburb for repairs: {PreferredRepairerSuburb.SuburbAndCode()}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"-------------Repairer options: {RepairerOption}{Reporting.HTML_NEWLINE}");
            }
            return formattedString.ToString();
        }
    }

    public class TowDetails
    {
        /// <summary>
        /// If NULL then this equates to "Unsure"
        /// If FALSE then this equates to "No"
        /// If TRUE then this equates to "Yes"
        /// </summary>
        public bool? WasVehicleTowed { get; set; }
        /// <summary>
        /// Applicable if WasVehicleTowed.Value == true
        /// </summary>
        public MotorClaimTowedTo TowedTo { get; set; }
        /// <summary>
        /// Applicable if WasVehicleTowed.Value == true
        /// </summary>
        public BusinessDetails BusinessDetails { get; set; }
        /// <summary>
        /// Applicable if WasVehicleTowed.Value == true
        /// </summary>
        public string CarLocation { get; set; }
    }

    public class BusinessDetails
    {
        public string BusinessName { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
    }

    public class TheftDetails
    {
        public MotorClaimTheftDetails WhatWasStolen { get; set; }
        public bool                             WereKeysStolen { get; set; }
        public bool                             WasFinanceOwing { get; set; }
        public bool                             WasVehicleForSale { get; set; }
    }

    public class GlassDamageDetails
    {
        public bool                      FrontWindscreenOnly { get; set; }
        public bool                      OtherWindowGlass { get; set; }
        public GlassDamageType TypeOfDamage { get; set; }
        public bool                      HasBeenRepaired { get; set; }
    }

    public class ContactClaimDriver
    {
        /// <summary>
        /// To set driver as claimant, leave DriverDetails null.
        /// </summary>
        public Contact DriverDetails { get; set; }
        public bool WasDriverLicenceSuspended { get; set; }
        public bool WasDriverDrunk { get; set; }
        public bool DriverLicensedMoreThan2Years { get; set; }
        public string AdditionalInformation { get; set; }
        //Return true if the driver is a new Contact, otherwise it returns false.
        public bool isNewContact() => DriverDetails.ExternalContactNumber == null;
        public List<string> expectedClaimDrivers { get; set; }
    }
       

    public class ContactClaimMotorTP : Contact
    {
        public ContactClaimMotorTP(Contact contact)
        {
            Id = contact.Id;
            ExternalContactNumber = contact.ExternalContactNumber;
            Title = contact.Title;
            Initial = contact.Initial;
            FirstName = contact.FirstName;
            MiddleName = contact.MiddleName;
            Surname = contact.Surname;
            Gender = contact.Gender;
            MobilePhoneNumber = contact.MobilePhoneNumber;
            HomePhoneNumber = contact.HomePhoneNumber;
            PrivateEmail = contact.PrivateEmail;
            DateOfBirth = contact.DateOfBirth;
            MailingAddress = contact.MailingAddress;
            MembershipNumber = contact.MembershipNumber;
            MembershipTier = contact.MembershipTier;
            BankAccounts = contact.BankAccounts;
            CreditCards = contact.CreditCards;
            IsPolicyHolder = contact.IsPolicyHolder;
        }

        public bool   IsKnownToClaimant { get; set; }
        public bool WasDriverTheOwner { get; set; }
        public string Rego { get; set; }
        public InsuranceDetails Insurer { get; set; }
        public string DescriptionOfDamageToVehicle { get; set; }
        public string AdditionalInfo { get; set; }
        ///<summary>
        /// This is an optional value for where the Third Party in a multi-vehicle
        /// collision has made a claim and provides their own claim number
        /// to the claimant. This can be a random alphanumeric string for tests
        /// </summary>        
        public string ClaimNumber { get; set; }
        /// <summary>
        /// Set by Shield DB when retrieving TP from lodged claim
        /// </summary>
        public string DBTPRole { get; set; }
        public string DBTPEMailStatus { get; set; }
        public string DBTPPhoneStatus { get; set; }
        public string DBTPAddressStatus { get; set; }
    }

    public class InsuranceDetails
    {
        public string Name { get; set; }
        public string ExternalContactNumber { get; set; }
    }
}
