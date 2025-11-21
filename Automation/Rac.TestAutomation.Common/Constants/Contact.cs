using System.Collections.Generic;
using System.ComponentModel;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class Contacts
        {
            public const int MIN_PH_AGE_VEHICLES = 16;  //The minimum allowed age for Policyholders for Boat, Caravan, Motor and Motorcycle.
            public const int MIN_PH_AGE_HOME_PET = 17;  //The minimum allowed age for Policyholders for Home and Pet.
            public const int MIN_PH_AGE_BOAT_SKIPPERTICKET_MORETHANTHREE = 17; //The minimum allowed age for a Skipper's Ticket for 3 or more years.
            public const int MAX_PH_AGE = 100; //Maximum age allowed age for Policyholders.

            public static readonly string QUOTE_ROLE_IN_SHIELD = "Prospect";

            public static readonly string QUOTE_B2C_IN_SHIELD = "_B2C";

            public static readonly string NPE_EMAIL_DOMAIN_RACTEST   = "ractest.com.au";
            public static readonly string NPE_EMAIL_DOMAIN_MAILOSAUR = "mailosaur.net";

            /// <summary>
            /// This enumerated list includes all valid values for Title available 
            /// in the Shield database (see T_TITLE) listed alphabetically, as well as 
            /// a "None" entry for an empty Title.
            /// 
            /// At time of writing we only allow selection of Dr, Mr, Mrs, Ms, Miss & Mx
            /// in our member-facing applications so our tests will often invoke 
            /// Contact.IsValidB2CTitle to allow substitution of other values.
            /// </summary>
            public enum Title
            {
                [Description("Bishop")]
                Bishop,
                [Description("Brigadier")]
                Brigadier,
                [Description("Canon")]
                Canon,
                [Description("Captain")]
                Captain,
                [Description("Colonel")]
                Colonel,
                [Description("Dame")]
                Dame,
                [Description("Doctor")]
                Dr,
                [Description("Father")]
                Father,
                [Description("General")]
                General,
                [Description("Judge")]
                Judge,
                [Description("Lady")]
                Lady,
                [Description("Lieutenant")]
                Lieutenant,
                [Description("Lieutenant Colonel")]
                LieutenantColonel,
                [Description("Lieutenant General")]
                LieutenantGeneral,
                [Description("Lord")]
                Lord,
                [Description("Madam")]
                Madam,
                [Description("Major")]
                Major,
                [Description("Master")]
                Master,
                [Description("Miss")]
                Miss,
                [Description("Mr")]
                Mr,
                [Description("Mrs")]
                Mrs,
                [Description("Ms")]
                Ms,
                [Description("Mx")]
                Mx,
                [Description("")]
                None,
                [Description("Pastor")]
                Pastor,
                [Description("Pltoff")]
                Pltoff,
                [Description("Private")]
                Private,
                [Description("Professor")]
                Professor,
                [Description("Reverend")]
                Reverend,
                [Description("Senator")]
                Senator,
                [Description("Sir")]
                Sir,
                [Description("Sister")]
                Sister,
                [Description("Ven")]
                Ven,
                [Description("Wing Commander")]
                WingCommander,
            };

            public enum Gender
            {
                [Description("Male")]
                Male,
                [Description("Female")]
                Female
            };

            public enum PhonePrefix
            {
                [Description("02")]
                NSW,
                [Description("03")]
                VIC,
                [Description("07")]
                QLD,
                [Description("08")]
                WA_SA,
                [Description("04")]
                Mobile
            }

            public enum PreferredDeliveryMethod
            {
                Email,
                Mail
            }

            /// <summary>
            /// Assigned values are to assist in determining higher value of
            /// Membership tier. Their actual values are a little arbitrary
            /// and simply chosen to allow for any future changes
            /// </summary>
            public enum MembershipTier
            {
                None = 0,
                Gold = 600,
                Silver = 500,
                Bronze = 400,
                Blue = 300,
                Red = 200,
                Free2Go = 100
            };

            public readonly static IReadOnlyDictionary<MembershipTier, string> MembershipTierText = new Dictionary<MembershipTier, string>()
            {
                { MembershipTier.Gold,    "Gold member" },
                { MembershipTier.Silver,  "Silver member" },
                { MembershipTier.Bronze,  "Bronze member" },
                { MembershipTier.Red,     "Red member" },
                { MembershipTier.Blue,    "Blue member" },
                { MembershipTier.Free2Go, "Free2Go" }
            };

            public readonly static IReadOnlyDictionary<MembershipTier, string> APIDiscountCode = new Dictionary<MembershipTier, string>()
            {
                { MembershipTier.Gold,    "U" },
                { MembershipTier.Silver,  "V" },
                { MembershipTier.Bronze,  "W" },
                { MembershipTier.None,    "N" }
            };

            /// Checks contact details against Member Central Person v2 Match API.
            /// based on one of the following Rules:
            public enum MemberMatchRule
            {
                /// <summary>
                /// This rule matches on first name, DOB and mobile number of the contact.
                /// Number reflects assessment order by Member Central.
                /// </summary>
                Rule1, //First name, Date of birth and Mobile
                /// <summary>
                /// This rule matches on first name, DOB and email of the contact.
                /// Number reflects assessment order by Member Central.
                /// </summary>
                Rule2, //First name, Date of birth and Email
                /// <summary>
                /// This rule matches on first name, DOB and street address of the contact.
                /// Number reflects assessment order by Member Central.
                /// </summary>
                Rule3, //First name, Date of birth and Postal Address
                None   //None of the above
            }

            public enum ContactRole
            {
                [Description("Claimant")]
                Claimant,
                [Description("Driver")]
                Driver,
                [Description("Witness")]
                Witness,
                [Description("Service Provider")]
                ServiceProvider,
                [Description("Policyholder")]
                PolicyHolder,
                [Description("Policy co-owner")]
                CoPolicyHolder,
                [Description("Authorized Party")]
                AuthParty,
                [Description("Prospect")]
                Prospect,
                [Description("Staff Member")]
                StaffMember
            };

            public enum DriverRole
            {           
                [Description("Policyholder")]
                PolicyHolder,
                [Description("Policy co-owner")]
                CoPolicyHolder,               
                [Description("NewContact")]
                NewContact
            };

            public readonly static IReadOnlyDictionary<ContactRole, int> ContactRoleIdentifiers = new Dictionary<ContactRole, int>()
            {
                { ContactRole.Claimant, 1 },
                { ContactRole.Witness, 2 },
                { ContactRole.ServiceProvider, 5 },
                { ContactRole.Driver, 9 },
                { ContactRole.Prospect, 27 },
                { ContactRole.StaffMember, 1000007 }
            };
        }
    }
}
