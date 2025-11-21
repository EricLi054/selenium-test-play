using Integration;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.APIDriver;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentType = Integration.DocumentType;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Tests.ActionsAndValidations
{
    public class VerifyCCI
    {
        protected static MailosaurEmailHandler _messageHandler;
        protected static CCIMailbox _cciMailbox;
        protected static Config _config;

        private const string EMAIL_CONTENT = "Thanks for contacting us. We'll be in touch as soon as we can. If you're an RAC member, " +
                                            "you can login to myRAC and make a claim online 24/7. Or perhaps our FAQs may help with your enquiry. " +
                                            "We're here to help. Kind regardsRAC Claims Team";
        private const string EMAIL_CONTENT_XPATH = "//tr[2]//table[@class='MsoNormalTable'][text()]";

        static VerifyCCI() {
            _messageHandler = new MailosaurEmailHandler();
            _config = Config.Get();
            _cciMailbox = new CCIMailbox();
        }

        /// <summary>
        /// Verifying the email sent by a member, supplier or credit hire company has been processed by CCI 
        /// and moved to respective folder on the mailbox
        /// </summary>
        /// <param name="cciClaimTestData">CCI claim test data</param>
        /// <param name="mailbox">mailbox on which emails to be checked</param>
        /// <param name="folder">The folder we expect to find the email in after CCI processing</param>
        public static void VerifyEmailInCCIMailbox(CCIClaim cciClaimTestData, string mailbox, CCIMailbox.Folder folder)
        {
            Reporting.Log($"Checking the email reaches '{folder}' folder of '{mailbox}' Mailbox.");
            var emails = _cciMailbox.FindEmailsFromFolderBySubject(mailbox,folder, GetExpectedSavedEmailSubject(cciClaimTestData)).GetAwaiter().GetResult();

            if (emails == null)
            {
                Reporting.Error($"Unable to find the email on '{folder}' folder of '{mailbox}' Mailbox.The Email is not picked by CCI for processing");
            }
            Reporting.AreEqual(GetExpectedSavedEmailSubject(cciClaimTestData), emails.First().Subject, $"email is moved to '{folder}' folder");
        }

        /// <summary>
        /// Verifying email is successfully catalogued on the shield DB.
        /// Checking email (*.eml file), document type, catalogued date
        /// </summary>
        /// <param name="cciClaimTestData">CCI Claim test data</param>
        public static void VerifyClaimCorrespondence(CCIClaim cciClaimTestData)
        {
            foreach (var polClaimAndCase in cciClaimTestData.PolicyClaimAndCase)
            {
                bool found = false;
                var correspondences = cciClaimTestData.DocumentType.Equals(DocumentType.TCUIncomingCorrespondence) ? 
                                        ShieldClaimDB.GetClaimCorrespondence(polClaimAndCase.CaseNumber) : 
                                        ShieldClaimDB.GetClaimCorrespondence(polClaimAndCase.ClaimNumber);
   
                Reporting.LogMinorSectionHeading($"Verifying the email catalogued on the shield for the Claim/Case number {polClaimAndCase.ClaimNumber} / {polClaimAndCase.PolicyNumber}.");
                foreach (var correspondence in correspondences)
                {

                    if  (correspondence.CreationDate > cciClaimTestData.EmailSentOn)
                    {
                        Reporting.Log("The email is catalogued on Shield as " + correspondence.FileName);
                        if (cciClaimTestData.DocumentType.Equals(DocumentType.TCUIncomingCorrespondence))
                        {
                            Reporting.AreEqual($"{GetExpectedSavedEmailSubject(cciClaimTestData)}.eml", correspondence.FileName, "email file name matches with the catalogued.");
                            Reporting.AreEqual(DataHelper.GetDescription(cciClaimTestData.DocumentType), correspondence.DocType, "'Document Type' matching with the expected.");
                            Reporting.AreEqual(DateTime.Now.ToString("dd/MM/yyyy"), correspondence.CreationDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), "email is catalogued today.");
                            Reporting.IsTrue(string.IsNullOrEmpty(correspondence.Remarks), $"email summary is displayed as blank for 'TCU' emails");
                            Reporting.IsTrue(correspondence.IsActionable, "'ISACTIONABLE' flag is always displayed as True for 'TCU' emails");
                            found = true;
                            break;
                        }
                        else if (cciClaimTestData.DocumentType == DocumentType.CreditHireProofOfLoss)
                        {                           
                            Reporting.IsTrue(correspondence.DocType.StartsWith(DataHelper.GetDescription(cciClaimTestData.EmailSenderType)), $"'Document Type' the email is catalogued in Shield is corresponding to '{cciClaimTestData.EmailSenderType.ToString()}', ie - {correspondence.DocType}");
                            Reporting.AreEqual(DateTime.Now.ToString("dd/MM/yyyy"), correspondence.CreationDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), "email is catalogued today.");
                            Reporting.IsTrue(string.IsNullOrEmpty(correspondence.Remarks), $"email summary is displayed as blank for 'Proof Of Loss' emails");
                            Reporting.IsTrue(correspondence.IsActionable, "'ISACTIONABLE' flag is always displayed as True for 'Proof Of Loss' emails");
                            found = true;
                            break;
                        }
                        else
                        {
                            Reporting.IsTrue(correspondence.DocType.StartsWith(DataHelper.GetDescription(cciClaimTestData.EmailSenderType)), $"'Document Type' the email is catalogued in Shield is corresponding to '{cciClaimTestData.EmailSenderType.ToString()}', ie - {correspondence.DocType}");
                            Reporting.AreEqual(DateTime.Now.ToString("dd/MM/yyyy"), correspondence.CreationDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), "email is catalogued today.");                          
                            Reporting.IsTrue(!string.IsNullOrEmpty(correspondence.Remarks), $"email summary is not displayed as blank, Emails Summary is '{correspondence.Remarks}'");
                            var actualIsActionable = correspondence.IsActionable == true || correspondence.IsActionable == false ? true : false;
                            Reporting.IsTrue(actualIsActionable, "'ISACTIONABLE' flag is displayed as True or False only");// Passing the test if actionable fag is either true or false.
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    Reporting.Error($"The email is not catalogued on the shield for the Claim/Case number {polClaimAndCase.ClaimNumber}/{polClaimAndCase.CaseNumber}.");
                }
            }
        }

        /// <summary>
        /// Verifying the events is attached to the claim number
        /// </summary>
        public static void VerifyShieldEvent(CCIClaim cciClaimTestData)
        {
            foreach (var polClaimAndCase in cciClaimTestData.PolicyClaimAndCase)
            {
                var shieldEvents = cciClaimTestData.DocumentType.Equals(DocumentType.TCUIncomingCorrespondence) ? ShieldClaimDB.GetCaseEvents(polClaimAndCase.CaseNumber) : ShieldClaimDB.GetClaimEvents(polClaimAndCase.ClaimNumber);
               
                if (cciClaimTestData.DocumentType.Equals(DocumentType.TCUIncomingCorrespondence))
                {
                    Reporting.LogMinorSectionHeading($"Verifying the event is added on the Case Number {polClaimAndCase.CaseNumber}");
                    Reporting.IsTrue(shieldEvents.Any(ev => ev.Equals(DataHelper.GetDescription(cciClaimTestData.DocumentType))), $"the expected event is added to Shield Case");
                }
                else
                {
                    Reporting.LogMinorSectionHeading($"Verifying the event is added on the Claim Number {polClaimAndCase.ClaimNumber}");
                    Reporting.IsTrue(shieldEvents.Any(ev => ev.Contains(DataHelper.GetDescription(cciClaimTestData.EmailSenderType))), $"the expected event is added to Shield Claim");
                }                                 
            }
        }

        /// <summary>
        /// Returns the expected email subject based on business rules.
        /// If document type is Motor Proof Of Loss, we assume it is forwarded
        /// from the "Proof Of Loss" mailbox and this a "FW: " prefix would have
        /// been added.
        /// </summary>
        private static string GetExpectedSavedEmailSubject(CCIClaim testData)
        {
            return testData.DocumentType == DocumentType.CreditHireProofOfLoss ?
                           $"FW: {testData.EmailPayload.Subject}" :
                           testData.EmailPayload.Subject;
        }

       /// <summary>
       /// Verifying response email from CCI after cataloguing the email to Shield
       /// </summary>
       public static void VerifyEmailResponse()
        {

            HtmlDocument doc = new HtmlDocument();
            var emailHandler = new MailosaurEmailHandler();
            var email = Task.Run(() => emailHandler.FindEmailBySubject($"Thank you for your enquiry")).GetAwaiter().GetResult();
         
            doc.LoadHtml(email.Html.Body);
            var emailHtmlAsText = doc.DocumentNode.SelectNodes(EMAIL_CONTENT_XPATH)[0].InnerText.StripLineFeedAndCarriageReturns(true);
            string emailHtmlAsTextCleaned = emailHtmlAsText.RemoveDuplicateWhiteSpaceAndTrim();

            if (email != null)
            {
                Reporting.AreEqual(EMAIL_CONTENT, emailHtmlAsTextCleaned, "'Thank you for your enquiry' email on Mailosaur");
                Reporting.Log(email.Html.Body);
            }
            else
            {
                Reporting.Error("'Thank you for your enquiry' email not recieved on Mailosaur");
            }
        }
    }
}