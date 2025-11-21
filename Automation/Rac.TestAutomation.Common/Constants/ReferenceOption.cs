namespace Rac.TestAutomation.Common
{
    public class ReferenceOption
    {
        public string Id { get; set; }

        public string ExternalCode { get; set; }

        public string Description { get; set; }
    }

    /// <summary>
    /// Shield Endorsement Reason
    /// Source from:
    /// SELECT ID, EXTERNAL_CODE, [DESCRIPTION] FROM T_ENDORSMENT_REASON WHERE EXTERNAL_CODE IS NOT NULL ORDER BY ID
    /// </summary>
    public static class EndorsementReasonOption
    {
        public static readonly ReferenceOption GeneralChanges = new ReferenceOption
        {
            Id = "10",
            ExternalCode = "GeneralChanges",
            Description = "General Changes"
        };

        public static readonly ReferenceOption EndorseForRenewal = new ReferenceOption
        {
            Id = "1000071",
            ExternalCode = "EndorseForRenewal",
            Description = "Endorsement For Renewal"
        };
    }

    public static class EventType
    {
        public static readonly string GeneralChange = "General change";

        public static readonly string PolicyEndorsementCertificatePrint = "Policy Endorsement Certificate Print";
    }
}
