using System.ComponentModel;

namespace Rac.TestAutomation.Common
{
    public partial class Constants
    {
        public class Endorsements
        {
            public class Cancellations
            {
                public class DetailsPage
                {
                    public const string Title = "Cancelling your policy";
                    public const string SubTitle = "Thanks for being a valued RAC member";
                }

                public class ConfirmationPage
                {
                    public const string LeavingMessage = "Your policy is now cancelled";
                    public class ExitMessage
                    {
                        public const string BadClaimsExperience = "We're sorry about your claim experience, ";
                        public const string SorryToSeeYouGo = "We’re sorry to see you go, "; // TODO: SPK-2149 Update apostrophe character (PCO)
                        public const string ConfirmingEmail = "Look out for an email from us confirming this cancellation.";
                    }
                }

                /// <summary>
                /// Cancellation reasons displayed to the member and the corresponding external code 
                /// in SHIELD are specified at:
                /// https://rac-wa.atlassian.net/wiki/spaces/ISP/pages/2740551801/Cancellation+reason+behaviour
                /// </summary>
                public class Reason
                {
                    public class BadClaimsExperience
                    {
                        public const string Text = "I’m not happy with a claim experience";
                        public const string ExternalCode = "10";
                    }

                    public class ChangingInsurer
                    {
                        public const string Text = "I'm changing to another insurer";
                        public const string ExternalCode = "2";
                    }

                    public class FinanacialHardship
                    {
                        public const string Text = "I'm experiencing financial hardship";
                        public const string ExternalCode = "20";
                    }

                    public class CancelWithin28DayFreeLookPeriod
                    {
                        public const string Text = "This policy doesn't suit my needs";
                        public const string ExternalCode = "13";
                    }

                    public class HouseDemolished
                    {
                        public const string Text = "My house has been demolished";
                        public const string ExternalCode = "25";
                    }

                    public class HouseSold
                    {
                        public const string Text = "I sold my house";
                        public const string ExternalCode = "11";
                    }
                }

                public enum RefundDestinationType
                {
                    [Description("Bank Account")]
                    BankAccount,
                    [Description("Card")]
                    Card
                }
            }
        }
    }
}
