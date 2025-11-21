using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class ClaimsGeneral
        {
            public enum AgendaStepNames
            {
                [Description("Claim Lodged")]
                ClaimLodged,
                [Description("Building - Quote Authorised")]
                BuildingQuoteAuthorised,
                [Description("Contents - Quote Authorised")]
                ContentsQuoteAuthorised,
                [Description("Motor - Quote Authorised")]
                MotorQuoteAuthorised,
                [Description("Valuables - Quote Authorised")]
                ValuablesQuoteAuthorised,
                [Description("Building - Repairs Complete")]
                RepairsComplete,
                [Description("Contents - Items Replaced or Paid Out")]
                ContentsReplacedOrPaidOut,
                [Description("Valuables - Items Replaced or Paid Out")]
                ValuablesReplacedOrPaidOut,
                [Description("Invoice Received")]
                InvoiceReceived,
                [Description("Payments Settled")]
                PaymentsSettled
            };

            public readonly static IReadOnlyDictionary<AgendaStepNames, Int32> AgendaStepIdentifiers = new Dictionary<AgendaStepNames, Int32>()
            {
                { AgendaStepNames.ClaimLodged, 3000000 },
                { AgendaStepNames.BuildingQuoteAuthorised, 3000001 },
                { AgendaStepNames.ContentsQuoteAuthorised, 3000002 },
                { AgendaStepNames.ValuablesQuoteAuthorised, 3000003 },
                { AgendaStepNames.MotorQuoteAuthorised,    3000005 },
                { AgendaStepNames.RepairsComplete, 3000006 },
                { AgendaStepNames.ContentsReplacedOrPaidOut, 3000007 },
                { AgendaStepNames.ValuablesReplacedOrPaidOut, 3000008 },
                { AgendaStepNames.InvoiceReceived, 3000010 },
                { AgendaStepNames.PaymentsSettled, 3000011 }
            };

            public enum AgendaStepStatus
            {
                [Description("Select")]
                Select,
                [Description("Done")]
                Done,
                [Description("Not Applicable")]
                NotApplicable,
                [Description("Current")]
                Current,
                [Description("Pending")]
                Pending
            }

            public enum ClaimantSide
            {
                PolicyHolder,
                ThirdParty
            };

            public class ClaimContactRole
            {
                public static readonly string PolicyHolder = "PH";
                public static readonly string ThirdParty = "TP";
                public static readonly string PolicyCoOwner = "POLCOOWNER";
                public static readonly string Informant = "INFORMANT";
                public static readonly string Driver = "3D";
                public static readonly string ServiceProvider = "FN";
                public static readonly string Witness = "WT";
            };

            public readonly static IReadOnlyDictionary<ClaimantSide, int> ClaimantSideIdentifiers = new Dictionary<ClaimantSide, int>()
            {
                { ClaimantSide.PolicyHolder, 1 },
                { ClaimantSide.ThirdParty, 2 }
            };

            public const string Yes = nameof(Yes);
            public const string No = nameof(No);
            public const string Unsure = nameof(Unsure);
            public const string Unknown = nameof(Unknown);
            public const string EFT = nameof(EFT);
            public const string Unrecovered = "Unrecovered (Theft)";

            public readonly static IReadOnlyDictionary<string, string> MappedShieldAnswer = new Dictionary<string, string>()
            {
                { "0", No },
                { "1", Yes },
                // Is vehicle driveable
                { "1000005", Yes },
                { "1000006", No },
                { "1000007", Unrecovered },
                // Was vehicle towed
                { "1000008", Yes },
                { "1000009", No },
                { "1000010", Unsure },
                // Are repairs to be completed in WA
                { "3000025", Yes },
                { "3000026", No },
                // Member needs to provide payment info
                { "4000002", No},
                { "4000003", EFT}
            };

            public enum LiabilityPercent
            {
                [Description("Unknown")]
                Unknown,
                [Description("0%")]
                Percent0,
                [Description("10%")]
                Percent10,
                [Description("25%")]
                Percent25,
                [Description("50%")]
                Percent50,
                [Description("75%")]
                Percent75,
                [Description("90%")]
                Percent90,
                [Description("100%")]
                Percent100
            }

            public enum LiabilityWithOutPercent
            {
                [Description("Unknown")]
                Unknown,
                [Description("0")]
                Percent0,
                [Description("10")]
                Percent10,
                [Description("25")]
                Percent25,
                [Description("50")]
                Percent50,
                [Description("75")]
                Percent75,
                [Description("90")]
                Percent90,
                [Description("100")]
                Percent100
            }

            public enum LoginWith
            {
                PolicyNumber,
                ContactId
            }
            public enum ClaimEventType
            {
                [Description("ShieldHomeClaimRepairScopeDelayed")]
                HomeClaimRepairScopeDelayed,
                [Description("ShieldClaimProcessingDelayed")]
                ClaimProcessingDelayed,
                [Description("ShieldClaimAutomaticCashSettlementFactSheet")]
                ClaimAutomaticCSFS,
                [Description("ShieldClaimClosure")]
                ClaimClosure,
                [Description("ShieldInvoiceReminder")]
                InvoiceReminder,
                [Description("ShieldReminderClaimClosure")]
                ReminderClaimClosure
            };

            public enum MemberNotificationServiceType
            {
                [Description("ShieldClaim")]
                ShieldClaim,
                [Description("ShieldPolicy")]
                ShieldPolicy,
                [Description("ShieldPolicyholderClaimant")]
                ShieldPolicyholderClaimant
            };

            public static readonly int MaxNumberOfFile = 5;
        }
    }
}
