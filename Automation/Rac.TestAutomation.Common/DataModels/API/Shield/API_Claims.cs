using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;

using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;

namespace Rac.TestAutomation.Common.API
{
    public class QuestionnaireLine
    {
        [JsonProperty("answerId")]
        public string AnswerId { get; set; }

        [JsonProperty("lineId")]
        public string LineId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("answerValue")]
        public string AnswerValue { get; set; }

        public TEnum GetAnswerIDAsEnum<TEnum>() where TEnum : Enum
        {
            var idAsInt = int.Parse(AnswerId);
            return (TEnum)Enum.ToObject(typeof(TEnum), idAsInt);
        }

        /// <summary>
        /// Convert AnswerID to meangingful string based on
        /// MappedShieldAnswer dictionary.
        /// </summary>
        /// <returns></returns>
        public string GetAnswerIDAsMappedString()
        {
            if (string.IsNullOrEmpty(AnswerId))
            { return No; }  // Shield default answer is No if AnswerId is not returned

            return MappedShieldAnswer[AnswerId];
        }
    }

    public class DamagedAssetQuestionnaire
    {
        [JsonProperty("questionnaire")]
        public string Questionnaire { get; set; }

        [JsonProperty("questionnaireLines")]
        public List<QuestionnaireLine> QuestionnaireLines { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public class DamagedAsset
    {
        [JsonProperty("damages")]
        public List<Damage> Damages { get; set; }
        [JsonProperty("damagedAssetQuestionnaires")]
        public List<DamagedAssetQuestionnaire> DamagedAssetQuestionnaires { get; set; }
        [JsonProperty("driver")]
        public Driver Driver { get; set; }
        [JsonProperty("claimantInsurer")]
        public string ClaimantInsurer { get; set; }
        [JsonProperty("claimantClaimNumber")]
        public string ClaimantClaimNumber { get; set; }
        [JsonProperty("claimantLiability")]
        public string ClaimantLiability { get; set; }
    }

    public class ClaimContact
    {
        [JsonProperty("calculatedExcess")]
        public double CalculatedExcess { get; set; }
        [JsonProperty("outstandingExcess")]
        public double OutstandingExcess { get; set; }
        [JsonProperty("claimantStatusReason")]
        public string ClaimantStatusReason { get; set; }
        [JsonProperty("claimantSide")]
        public string ClaimantSide { get; set; }
        [JsonProperty("claimContactRole")]
        public string ClaimContactRole { get; set; }
        [JsonProperty("contactExternalNumber")]
        public string ContactExternalNumber { get; set; }
        [JsonProperty("relatedClaimantExternalNumber")]
        public string RelatedClaimantExternalNumber { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("witnessLocationType")]
        public string WitnessLocationType { get; set; }
        [JsonProperty("witnessNote")]
        public string WitnessNote { get; set; }
        [JsonProperty("damagedAssets")]
        public List<DamagedAsset> DamagedAssets { get; set; }
        [JsonProperty("claimantEventDescription")]
        public string ClaimantEventDescription { get; set; }
        [JsonProperty("claimantLiability")]
        public string ClaimantLiability { get; set; }
    }

    public class Damage
    {
        [JsonProperty("coverExternalNumber")]
        public string CoverExternalNumber { get; set; }

        [JsonProperty("damageCode")]
        public string DamageCode { get; set; }

        [JsonProperty("damageStatus")]
        public string DamageStatus { get; set; }

        [JsonProperty("damageStatusReason")]
        public string DamageStatusReason { get; set; }
       
    }

    public class EventLocation
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("streetName")]
        public string StreetName { get; set; }

        [JsonProperty("isAddressValidated")]
        public bool IsAddressValidated { get; set; }
    }

    public class Driver
    {
        [JsonProperty("hasLicenceSuspended")]
        public string HasLicenceSuspended { get; set; }

        [JsonProperty("hasTwoYearsDriverLicence")]
        public string HasTwoYearsDriverLicence { get; set; }

        [JsonProperty("contactExternalNumber")]
        public string ContactExternalNumber { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("wasDriverUnderTheInfluence")]
        public string WasDriverUnderTheInfluence { get; set; }
    }

    public class GetClaimResponse
    {
        private class Role
        {
            public const string ServiceProvider = "FN";
            public const string Policyholder = "PH";
            public const string CoPolicyholder = "POLCOOWNER";
            public const string MotorAssessingOfficer = "MOTASOFC";
            public const string Informant = "INFORMANT";
            public const string InternalHomeAssessor = "IHA";
        }

        [JsonProperty("claimContacts")]
        public List<ClaimContact> ClaimContacts { get; set; }
        
        /// <summary>
        /// Returns the questionnaire line that matches the provided questionnaire line Id.
        /// throws an exception if no matching questionnaire is found.
        /// </summary>
        public QuestionnaireLine GetQuestionnaireLine(int questionLineId)
        {
            // Done in two steps to assist with debugging in the event that the claim response is incomplete.
            var damagedAssets = ClaimContacts.Where(x=>x.DamagedAssets != null && x.DamagedAssets.Any()).SelectMany(x => x.DamagedAssets);
            return damagedAssets.SelectMany(x => x.DamagedAssetQuestionnaires).SelectMany(y => y.QuestionnaireLines).First(x => x.LineId == questionLineId.ToString());
        }

        /// <summary>
        /// Returns the name of the assigned service provider. If no service
        /// provider has been assigned, it will return null.
        /// </summary>
        public string ServiceProviderName()
        {
            return ClaimContacts.FirstOrDefault(x => x.ClaimContactRole.Equals(Role.ServiceProvider))?.Name;
        }
        /// <summary>
        /// Returns the name of the assigned Internal Home Assessor (relevant to Home Claims ONLY).
        /// If no Internal Home Assessor has been assigned, it will return null.
        /// </summary>
        /// <returns></returns>
        public string ReturnInternalHomeAssessor()
        {
            return ClaimContacts.FirstOrDefault(x => x.ClaimContactRole.Equals(Role.InternalHomeAssessor))?.Name;
        }
        [JsonProperty("eventLocation")]
        public EventLocation EventLocation { get; set; }        
        [JsonProperty("isBlockedPayments")]
        public bool IsBlockedPayments { get; set; }
        [JsonProperty("policyNumber")]
        public string PolicyNumber { get; set; }
        [JsonProperty("claimNumber")]
        public string ClaimNumber { get; set; }
        [JsonProperty("claimInsertDate")]
        public DateTime ClaimInsertDate { get; set; }
        [JsonProperty("eventDate")]
        public DateTime EventDate { get; set; }
        [JsonProperty("claimScenario")]
        public string ClaimScenario { get; set; }
        [JsonProperty("isPoliceInvolved")]
        public bool IsPoliceInvolved { get; set; }
        [JsonProperty("policeReportNumber")]
        public string PoliceReportNumber { get; set; }
        [JsonProperty("claimType")]
        public string ClaimType { get; set; }
        [JsonProperty("eventDescription")]
        public string EventDescription { get; set; }
    }
   

    public class GetFenceSettlementBreakdownCost_Response
    {
        private decimal RawTotalRepairCost;
        private decimal RawSubTotalBeforeExcess;
        private decimal RawSubTotalBeforeAdjustments;
        private decimal RawDividingFenceAdjustment;

        /// <summary>
        /// !!! IMPORTANT !!!
        /// DisposalContribution is no longer expected to be utilised as it was effectively replaced 
        /// by asbestosInspectionFee. It remains here because it IS still included in the API.
        /// </summary>
        [JsonProperty("disposalContribution")]
        public decimal DisposalContribution { get; set; }

        [JsonProperty("asbestosInspectionFee")]
        public decimal AsbestosInspectionFee { get; set; }

        [JsonProperty("totalRepairCost")]
        public decimal TotalRepairCost
        {
            get => Math.Round(RawTotalRepairCost, 2);  // Midpoint rounding to two decimal to support asserts against onscreen values.
            set => RawTotalRepairCost = value;
        }

        [JsonProperty("fenceType")]
        public string FenceType { get; set; }

        [JsonProperty("subTotalBeforeExcess")]
        public decimal SubTotalBeforeExcess
        {
            get => Math.Round(RawSubTotalBeforeExcess, 2);  // Midpoint rounding to two decimal to support asserts against onscreen values.
            set => RawSubTotalBeforeExcess = value;
        }

        [JsonProperty("numberOfMetresClaimed")]
        public decimal NumberOfMetresClaimed { get; set; }

        [JsonProperty("currentExcess")]
        public decimal CurrentExcess { get; set; }

        [JsonProperty("paintingCost")]
        public decimal PaintingCost { get; set; }

        [JsonProperty("diggingCost")]
        public double DiggingCost { get; set; }

        [JsonProperty("subTotalBeforeAdjustments")]
        public decimal SubTotalBeforeAdjustments
        {
            get => Math.Round(RawSubTotalBeforeAdjustments, 2);  // Midpoint rounding to two decimal to support asserts against onscreen values.
            set => RawSubTotalBeforeAdjustments = value;
        }

        [JsonProperty("dividingFenceAdjustment")]
        public decimal DividingFenceAdjustment
        {
            get => Math.Round(RawDividingFenceAdjustment, 2);  // Midpoint rounding to two decimal to support asserts against onscreen values.
            set => RawDividingFenceAdjustment = value;
        }

        [JsonProperty("costPerMetre")]
        public decimal CostPerMetre { get; set; }
    }
}
