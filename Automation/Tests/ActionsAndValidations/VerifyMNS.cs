using Mailosaur.Models;
using Rac.TestAutomation.Common;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using static Integration.MemberNotificationService;
using static Rac.TestAutomation.Common.Constants.General;
using static System.Net.WebRequestMethods;

namespace Tests.ActionsAndValidations
{
    public class VerifyMNS
    {

        protected static MailosaurEmailHandler _messageHandler;
        protected static string _zone;

        private static string submitInvoiceURL;
        private static string privacyPolicyURL;
        private static string claimsFAQURL;
        private static string homePageURL;
        private static string contactUsURL;


        static VerifyMNS()
        {
            _messageHandler = new MailosaurEmailHandler();

            var config = Config.Get();
            _zone = "sit";
            if (config.Shield.IsUatEnvironment())
            { _zone = "uat"; }
            else if (config.Shield.IsDevEnvironment())
            { _zone = "dev"; }

            submitInvoiceURL = $"{config.Spark.Applications.ClaimsServicing}/claims/servicing/invoice-or-quote?claimNumber=";

            if (_zone == "uat")
            {                
                privacyPolicyURL = "https://rac.com.au/about-us/about-this-site/privacy";
                claimsFAQURL = "https://rac.com.au/insurance/make-a-claim/insurance-claims-faq";
                homePageURL = "https://rac.com.au";
                contactUsURL = "https://rac.com.au/about-us/contact-us";                
            }
            else
            {               
                privacyPolicyURL = "https://cdvnets.ractest.com.au/about-us/about-this-site/privacy";
                claimsFAQURL = "https://cdvnets.ractest.com.au/insurance/make-a-claim/insurance-claims-faq";
                homePageURL = "https://cdvnets.ractest.com.au";
                contactUsURL = "https://cdvnets.ractest.com.au/about-us/contact-us";
            }
        }

        private class EmailText
        {
            public static string ClaimClosure(string firstName) => $"Hi {firstName},Thank you for starting your claim online. We noticed you haven’t finished making your claim.To complete your claim, please reply to this email and we’ll contact you. You can also call us on 13 17 03 ( tel:131703 ).Please have your claim number handy if you get in touch.If you don’t want to proceed with your claim, please ignore this email.Kind regards,The RAC Claims TeamView our privacy policy ( https://rac.com.au/about-us/about-this-site/privacy )  Claims FAQ ( https://rac.com.au/insurance/make-a-claim/insurance-claims-faq )  rac.com.au ( https://rac.com.au )  Contact us ( https://rac.com.au/about-us/contact-us )RAC Insurance Pty Limited (ABN 59 094 685 882), GPO Box C140, Perth, WA, 6839";
            public static string RemainderInvoice(ClaimEvent claimDetails) => $"Hi {claimDetails.ClaimantName},We’re following up on your Car insurance claim.Submit your invoice or quoteThis is a reminder for you to please submit your invoice or quote for us to review.Submit now ( {submitInvoiceURL}{claimDetails.ClaimNumber} )Your claim detailsPolicy number {claimDetails.PolicyNumber} Claim number {claimDetails.ClaimNumber} Date of incident {claimDetails.ClaimEventDate} Outstanding excess ${claimDetails.OutstandingExcess}We’re here to help. If you have any questions, please call us on 13 17 03 ( tel:131703 ) or reply to this email.Kind regardsThe RAC Claims TeamView our privacy policy ( {privacyPolicyURL} )  Claims FAQ ( {claimsFAQURL} )  rac.com.au ( {homePageURL} )  Contact us ( {contactUsURL} )RAC Insurance Pty Limited (ABN 59 094 685 882), GPO Box C140, Perth, WA, 6839";
            public static string ReminderClaimClosure(ClaimEvent claimDetails) => $"Hi {claimDetails.ClaimantName},We’re following up on your Car insurance claim.Claim closedWe’ve asked you to submit your invoice or quote to us. We can’t process your claim without this, so we’ve closed your claim.To reopen your claimPlease submit your invoice or quote, and we’ll reopen your claim.Submit now ( {submitInvoiceURL}{claimDetails.ClaimNumber} )Your claim detailsPolicy number {claimDetails.PolicyNumber} Claim number {claimDetails.ClaimNumber} Date of incident {claimDetails.ClaimEventDate} Outstanding excess ${claimDetails.OutstandingExcess}We’re here to help. If you have any questions, please call us on 13 17 03 ( tel:131703 ) or reply to this email.Kind regardsThe RAC Claims TeamView our privacy policy ( {privacyPolicyURL} )  Claims FAQ ( {claimsFAQURL} )  rac.com.au ( {homePageURL} )  Contact us ( {contactUsURL} )RAC Insurance Pty Limited (ABN 59 094 685 882), GPO Box C140, Perth, WA, 6839";

        }

        private class SMSText
        {            
            public static string ClaimRepairScopeDelayed(string firstName) => $"Hi {firstName}, another update on your RAC claim. Unfortunately we haven’t been able to get to your report yet. We’ll review it as soon as we can. Thanks for your patience.";

            public static string ClaimSettlementFactSheet(string firstName) => $"Hi {firstName}, it’s the RAC here. Just a reminder about the email we sent about your claim. Thanks if you've already responded.";

            public const string ClaimProcessingDelayed = "We are currently processing your insurance claim. Please be advised we are experiencing a high volume of claims at present and appreciate your patience during this busy time. We will be in touch as soon as we can with an update on your claim progress. Regards, RAC Insurance";

        }

        public static void VerifyClaimClosureEmail(ClaimEvent claimDetails)
        {
            var email = Task.Run(() => _messageHandler.FindEmailBySubject($"Your incomplete claim number {claimDetails.ClaimNumber}", retrySeconds: WaitTimes.T150SEC)).GetAwaiter().GetResult();

            if (email != null)
            {
                Reporting.AreEqual(EmailText.ClaimClosure(claimDetails.ClaimantName), HttpUtility.HtmlDecode(email.Text.Body.StripLineFeedAndCarriageReturns(false)), "Incomplete claim closure email");
            }
        }

        public static void VerifyInvoiceRemainderEmail(ClaimEvent claimDetails)
        {
            var email = Task.Run(() => _messageHandler.FindEmailBySubject($"Your  claim {claimDetails.ClaimNumber}", retrySeconds: WaitTimes.T150SEC)).GetAwaiter().GetResult();

            if (email != null)
            {
                Reporting.AreEqual(EmailText.RemainderInvoice(claimDetails), HttpUtility.HtmlDecode(email.Text.Body.StripLineFeedAndCarriageReturns(false)), "Invoice Remainder email");
            }
        }


        public static void VerifyReminderClaimClosureEmail(ClaimEvent claimDetails)
        {
            var email = Task.Run(() => _messageHandler.FindEmailBySubject($"Your  claim {claimDetails.ClaimNumber}", retrySeconds: WaitTimes.T150SEC)).GetAwaiter().GetResult();

            if (email != null)
            {
                Reporting.AreEqual(EmailText.ReminderClaimClosure(claimDetails), HttpUtility.HtmlDecode(email.Text.Body.StripLineFeedAndCarriageReturns(false)), "Invoice Remainder email");
            }
        }

        public static void VerifySMSClaimRepairScopeDelayed(ClaimEvent claimDetails)
        {
            if (Config.Get().MNSInMailosaur())
            {
                Reporting.Log($"Feature toggle indicates that we {TelephoneFeatureToggles.SMSForMNSInMailosaur.GetDescription()} " +
                    $"so should be able to complete this test entirely within the automation.");
                Reporting.LogMinorSectionHeading("Searching Mailosaur inbox for SMS message");
                FindMemberNotificationSMS(SMSText.ClaimRepairScopeDelayed(claimDetails.ClaimantName), "Claim Repair Scope Delayed sms");
            }
            else
            {
                Reporting.Log($"Feature toggle indicates that we do NOT {TelephoneFeatureToggles.SMSForMNSInMailosaur.GetDescription()}, " +
                    $"so this test must be completed manually. Refer to the <a href=\"https://rac-wa.atlassian.net/wiki/spaces/ISP/pages/2899837049/Testing+MNS\">Testing MNS article on confluence</a> " +
                    $"regarding this workaround. " +
                    $"<P>Expected SMS content follows <i><b>for you to manually compare against.</i></b> <font color=\"red\">Test is <i>not complete</i> without that manual check.</font>");
                Reporting.LogMinorSectionHeading($"{SMSText.ClaimRepairScopeDelayed(claimDetails.ClaimantName)}");
                Reporting.SkipLog($"Test marked as 'skipped' to prompt investigation of these logs.");
            }
        }

        public static void VerifySMSClaimSettlementFactSheet(ClaimEvent claimDetails)
        {
            if (Config.Get().MNSInMailosaur())
            {
                Reporting.Log($"Feature toggle indicates that we {TelephoneFeatureToggles.SMSForMNSInMailosaur.GetDescription()} " +
                    $"so should be able to complete this test entirely within the automation.");
                Reporting.LogMinorSectionHeading("Searching Mailosaur inbox for SMS message");
                FindMemberNotificationSMS(SMSText.ClaimSettlementFactSheet(claimDetails.ClaimantName), "Claim Settlement Fact Sheet sms");
            }
            else
            {
                Reporting.Log($"Feature toggle indicates that we do NOT {TelephoneFeatureToggles.SMSForMNSInMailosaur.GetDescription()}, " +
                    $"so this test must be completed manually. Refer to the <a href=\"https://rac-wa.atlassian.net/wiki/spaces/ISP/pages/2899837049/Testing+MNS\">Testing MNS article on confluence</a> " +
                    $"regarding this workaround. " +
                    $"<P>Expected SMS content follows <i><b>for you to manually compare against.</i></b> <font color=\"red\">Test is <i>not complete</i> without that manual check.</font>");
                Reporting.LogMinorSectionHeading($"{SMSText.ClaimSettlementFactSheet(claimDetails.ClaimantName)}");
                Reporting.SkipLog($"Test marked as 'skipped' to prompt investigation of these logs.");
            }
        }

        public static void VerifySMSClaimProcessingDelayed()
        {
            if (Config.Get().MNSInMailosaur())
            {
                Reporting.Log($"Feature toggle indicates that we {TelephoneFeatureToggles.SMSForMNSInMailosaur.GetDescription()} " +
                    $"so should be able to complete this test entirely within the automation.");
                Reporting.LogMinorSectionHeading("Searching Mailosaur inbox for SMS message");
                FindMemberNotificationSMS(SMSText.ClaimProcessingDelayed, "Claim Processing Delayed sms");
            }
            else
            {
                Reporting.Log($"Feature toggle indicates that we do NOT {TelephoneFeatureToggles.SMSForMNSInMailosaur.GetDescription()}, " +
                    $"so this test must be completed manually. Refer to the <a href=\"https://rac-wa.atlassian.net/wiki/spaces/ISP/pages/2899837049/Testing+MNS\">Testing MNS article on confluence</a> " +
                    $"regarding this workaround. " +
                    $"<P>Expected SMS content follows <i><b>for you to manually compare against.</i></b> <font color=\"red\">Test is <i>not complete</i> without that manual check.</font>");
                Reporting.LogMinorSectionHeading($"{SMSText.ClaimProcessingDelayed}");
                Reporting.SkipLog($"Test marked as 'skipped' to prompt investigation of these logs.");
            }
        }

        private static void FindMemberNotificationSMS(string bodyText, string assertText)
        {
            try
            {
                var sms = Task.Run(() => _messageHandler.FindSMSByBody(bodyText, 120, WaitTimes.T150SEC)).GetAwaiter().GetResult();
                Reporting.IsTrue(sms != null, assertText);
            }
            catch (MailosaurException)
            {
                Reporting.Error("Getting an exception means we didn't find the SMS. Member Notification Service application " +
                    "uses a hardcoded SMS override number in NPE. Check that the value of AppSettings_SmsSettings_OverrideRecipient " +
                    "matches a number in the Mailosaur mailbox you're using." +
                    "IMPORTANT: This hardcoded mobile telephone number MUST NOT have a USA (+1) country code. " +
                    "Refer to the <a href=\"https://rac-wa.atlassian.net/wiki/spaces/ISP/pages/2899837049/Testing+MNS\">Testing MNS article on confluence</a>");
            }
        }
    }
}
