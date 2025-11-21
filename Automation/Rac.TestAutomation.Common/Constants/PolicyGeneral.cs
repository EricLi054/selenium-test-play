using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class PolicyGeneral
        {
            public static readonly string NO_PREVIOUS_INSURANCE = "NO PREVIOUS INSURANCE";
            public static readonly string ONLINE_DISCOUNT = "$50 First year online discount*"; //https://rac-wa.atlassian.net/browse/SPK-432

            /// <summary>
            /// This is to handle different Product Types from Shield GetQuote API
            /// </summary>
            public enum ShieldProductType
            {
                MGP = 1000000,    //Motor (Motor car)
                HGP = 1000001,    //Home
                MGE = 1000007,    //Electric Mobility
                MGV = 1000008,    //Caravan/Trailer
                MGO = 1000009,    //Truck
                MGT = 1000018,    //Tractor
                MGC = 1000032,    //Motor Cycle
                BGP = 1000033,    //Boat
                PET = 4000000     //Pet
            }

            /// <summary>
            /// This is to handle different Main Vehicle Type Descriptions from Shield GetVehicle API
            /// </summary>
            public enum ShieldMainVehicleTypeDesc
            {
                [Description("Motorcycles")]
                MOTORCYCLES,
                [Description("Van")]
                VAN,
                [Description("Trucks < 12 Tons")]
                TRUCKS_LESS_THAN_12_TONS,
                [Description("Trucks >=12 Tons")]
                TRUCKS_MORE_THAN_OR_EQUAL_TO_12_TONS,
                [Description("Moped")]
                MOPED,
                [Description("Special Vehicles")]
                SPECIAL_VEHICLES,
                [Description("Electric Mobility")]
                ELECTRIC_MOBILITY,
                [Description("Tractor")]
                TRACTOR,
                [Description("Truck")]
                TRUCK,
                [Description("Caravan")]
                CARAVAN,
                [Description("Unknown")]
                UNKNOWN
            }

            public enum Vehicle
            {
                Motorcycle,
                Car,
                Caravan,
                Boat,
                Bicycle
            }

            /// <summary>
            /// General statuses
            /// </summary>
            public enum Status
            {
                [Description("Current")]
                Current,
                [Description("Done")]
                Done,
                [Description("Pending")]
                Pending,
                [Description("Proposal")]
                Proposal,
                [Description("Policy")]
                Policy,
                [Description("Open")]
                Open,
                [Description("Processed")]
                Processed,
                [Description("Booked")]
                Booked,
                [Description("Paid")]
                Paid,
                [Description("Submitted")]
                Submitted,
                [Description("Cancelled")]
                Cancelled,
                [Description("Not Applicable")]
                NotApplicable
            }

            /// <summary>
            /// Values obtained from Shield table T_STATUS_CODE
            /// </summary>
            public enum ShieldPolicyStatusId
            {
                Policy = 20,
                Cancelled = 40
            }

            /// <summary>
            /// Values obtained from Shield table T_CANCEL_INITIATOR
            /// </summary>
            public enum ShieldCancellationInitiator
            {
                Raci = 1,
                PolicyHolder = 2
            }

            public enum PaymentFrequency
            {
                /* Indexes added to allow randomisation to skip
                 * Semi-Annual, as public front ends don't allow it.
                 */
                [Description("Semi-Annual")]
                SemiAnnual = 0,
                [Description("Monthly")]
                Monthly = 1,
                [Description("Annual")]
                Annual = 2
            }

            // Used when checking for fixed strings in UI and DB operations.
            public static readonly string BPAY_BILLER_CODE = "879254";
            public static readonly string DIRECT_DEBIT = "Direct Debit";
            public static readonly string CASH = "Cash";
            public static readonly string CREDIT_CARD = "Credit card";
            public static readonly string BANK_ACCOUNT = "Bank account";

            public enum CreditCardIssuer
            {
                [Description("Amex")]
                Amex,
                [Description("MASTERCARD")]
                Mastercard,
                [Description("VISA")]
                Visa,
            }
            public enum PaymentOptions
            {
                PayNow,
                PayLater
            }

            public enum PaymentScenario
            {
                AnnualCash,
                MonthlyBank,
                AnnualBank,
                MonthlyCard,
                AnnualCard
            };

            /// <summary>
            /// Spark - Motor Endorsements (2023+) only
            /// </summary>
            public enum PaymentOptionsSpark
            {
                /// <summary>
                /// This is for any recurring payment from CC or bank account
                /// Description has "unused" added, as we don't currently look
                /// for this in the UI, so its just to let people know that they
                /// can freely change it if their app requires it.
                /// </summary>
                [Description("Direct Debit - unused")]
                DirectDebit,
                /// <summary>
                /// This is a once off annual payment from CC
                /// </summary>
                [Description("Card")]
                AnnualCash,
                /// <summary>
                /// We provide BPay details to member for them to pay via their own institution
                /// </summary>
                [Description("BPAY")]
                BPay,
                /// <summary>
                /// We will email payment details for member to organise payment later
                /// </summary>
                [Description("Pay later")]
                PayLater
            }

            /// <summary>
            /// These are the corresponding payment term and collection method Shield DB Id mapping based from the payment method/scenario
            /// </summary>
            public readonly static IReadOnlyDictionary<PaymentScenario, PaymentMethodAndTerm> PaymentScenarioIdMappings = new Dictionary<PaymentScenario, PaymentMethodAndTerm>()
            {
                { PaymentScenario.AnnualCash,  new PaymentMethodAndTerm() { CollectionMethod = "1", PaymentTerm = "6"} },
                { PaymentScenario.MonthlyBank, new PaymentMethodAndTerm() { CollectionMethod = "2", PaymentTerm = "4"} },
                { PaymentScenario.AnnualBank,  new PaymentMethodAndTerm() { CollectionMethod = "2", PaymentTerm = "1"} },
                { PaymentScenario.MonthlyCard, new PaymentMethodAndTerm() { CollectionMethod = "4", PaymentTerm = "1000002"} }
            };

            public enum WestpacHttpStatusCodes
            {
                [Description("201")]
                Created
            };

            public enum AnnualKms
            {
                Undefined = 0,   // Where test case is not setting a value
                [Description("Less than 5,000")]
                LessThan5000,
                [Description("Up to 10,000")]
                UpTo10000,
                [Description("Up to 15,000")]
                UpTo15000,
                [Description("Up to 20,000")]
                UpTo20000,
                [Description("More than 20,000")]
                MoreThan20000
            }

            public enum PremiumChange
            {
                NotApplicable,
                NoChange,
                PremiumDecrease,
                PremiumIncrease
            };

            /// <summary>
            /// Financiers for Boat, Home, Motor, Motorcycle and Caravan.
            /// </summary>
            public readonly static string[] FinancierOptions = new[]
            {
                "360 FINANCE PTY LTD",
                "ACCESS FINANCE PARTNERS PTY LTD",
                "B & E LTD",
                "COLLIE MINERS CREDIT UNION LTD",
                "DNISTER UKRAINIAN CREDIT CO-OPERATIVE LTD",
                "ERINSTONE PTY LTD AS TRUSTEE FOR MACK SUPERANNUATION FUND",
                "FINLEASE",
                "G&C MUTUAL BANK",
                "HUNTER UNITED EMPLOYEE CREDIT UNION LTD",
                "INDUSTRY FUNDS MANAGEMENT (NOMINEES 2) PTY LTD",
                "JACARANDA CO-OPERATIVE HOUSING SOCIETY LTD",
                "KEYSTART & DEPARTMENT OF HOUSING",
                "LOAN WA",
                "ME BANK",
                "NCO GROUP",
                "P & N BANK",
                "QBANK",
                "RESIDENTIAL MORTGAGE GROUP AUSTRALIA PTY LTD",
                "SENIOR MASTER OF THE SUPREME COURT OF VICTORIA",
                "TLC B/S AS AGENT FOR KEYSTART LOANS LTD",
                "UBANK",
                "VICTORIA TEACHERS MUTUAL BANK",
                "W.H.S. AGENTS FOR KEYSTART LOANS",
                "YES LOANS"
        };

            /// <summary>
            /// To handle different member 'Discount Toasts' shown as a result of
            /// member matching on Page 1: 'Before we get started: Are you an RAC member?'
            /// page for Caravan/Motorcycle, and
            /// different member discount messages in the quote options page for Motorcycle and Caravan
            /// Motorcycle discount percentages: https://rac-wa.atlassian.net/browse/MO-573
            /// Caravan discount percentages: https://rac-wa.atlassian.net/browse/SPK-225
            /// </summary>
            public readonly static IReadOnlyDictionary<Contacts.MembershipTier, ProductTypes> MemberDiscountMappings = new Dictionary<Contacts.MembershipTier, ProductTypes>()
            {
                { Contacts.MembershipTier.Gold,    new ProductTypes() { Motorcycle = "10%", Caravan = "10%" } },
                { Contacts.MembershipTier.Silver,  new ProductTypes() { Motorcycle = "5%",  Caravan = "7.5%" } },
                { Contacts.MembershipTier.Bronze,  new ProductTypes() { Motorcycle = "4%",  Caravan = "5%" } }
            };

            /// <summary>
            /// These are different retrieve quote types
            /// </summary>
            public enum RetrieveQuoteType
            {
                Email,
                Website
            };

            /// <summary>
            /// This is to handle different installment status on Shield
            /// Assigned values must not be changed as they match Shield DB values used to represent these installment statuses.
            /// </summary>
            public enum InstallmentStatus
            {
                Pending = 1,
                PendingApproval = 2,
                Paid = 3,
                Cancelled = 4,
                Rejected = 5,
                Submitted = 6, 
            }

            /// <summary>
            /// This is to handle different product type for Make A Payment
            /// </summary>
            public enum MakePaymentScenarioType
            {
                ANY,
                RENEWAL,
                NEW_BUSINESS,
                MID_TERM,
            }
        }
    }
}
