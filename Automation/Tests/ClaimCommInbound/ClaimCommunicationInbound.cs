using AventStack.ExtentReports.Utils;
using Mailosaur.Models;
using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.API;
using Rac.TestAutomation.Common.APIDriver;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using Rac.TestAutomation.Common.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tests.ActionsAndValidations;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;

namespace Integration
{
    [Property("Integration", "Claim Communication Inbound (CCI) Integration Tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class ClaimCommunicationInbound : BaseNonUITest
    {

        static List<EmailDocTypeMapping> emails;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Claim Communication Inbound (CCI) Integration Tests");
            emails = EmailCollection();
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.ClaimCommunicationInbound)]
        public void INSU_T548_Member_MultipleClaimNumbersInEmailSubject_EnquiryEmail()
        {
            var testData = BuildTestDataForCCI(EmailScenario.MultipleClaimNumbersInSubject, EmailSenderType.Member, DocumentType.MemberEnquiryEmail);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.ClaimMailbox);
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.ClaimMailbox, CCIMailbox.Folder.Processed);
            VerifyCCI.VerifyClaimCorrespondence(testData);
            VerifyCCI.VerifyShieldEvent(testData);
            VerifyCCI.VerifyEmailResponse();
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.ClaimCommunicationInbound)]
        public void INSU_T545_Member_ClaimNumberInAttachmentName_InvoiceEmail()
        {
            var testData = BuildTestDataForCCI(EmailScenario.ClaimNumberInAttachmentName, EmailSenderType.Member, DocumentType.MemberInvoiceEmail);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.ClaimMailbox);
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.ClaimMailbox, CCIMailbox.Folder.Processed);
            VerifyCCI.VerifyClaimCorrespondence(testData);
            VerifyCCI.VerifyShieldEvent(testData);
            VerifyCCI.VerifyEmailResponse();
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.ClaimCommunicationInbound)]
        public void INSU_T534_Supplier_ClaimNumberInEmailBody_EnquiryEmail()
        {
            var testData = BuildTestDataForCCI(EmailScenario.ClaimNumberInBody, EmailSenderType.Supplier, DocumentType.SupplierEnquiry);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.ClaimMailbox);
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.ClaimMailbox, CCIMailbox.Folder.Processed);
            VerifyCCI.VerifyClaimCorrespondence(testData);
            VerifyCCI.VerifyShieldEvent(testData);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.ClaimCommunicationInbound)]
        public void INSU_T540_CreditHireCo_ClaimNumberInEmailSubject_CreditHireEmail()
        {
            var testData = BuildTestDataForCCI(EmailScenario.ClaimNumberInSubject, EmailSenderType.CreditHireCo, DocumentType.CreditHireEmail);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.ClaimMailbox);
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.ClaimMailbox, CCIMailbox.Folder.Processed);
            VerifyCCI.VerifyClaimCorrespondence(testData);
            VerifyCCI.VerifyShieldEvent(testData);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.ClaimCommunicationInbound)]
        public void INSU_T530_CreditHireCo_ClaimNumberNotInEmail_NotCatalogued()
        {
            var testData = BuildTestDataForCCI(EmailScenario.NoClaimNumber, EmailSenderType.CreditHireCo, DocumentType.None);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.ClaimMailbox);
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.ClaimMailbox, CCIMailbox.Folder.Processed);
            Reporting.LogMinorSectionHeading($"Verifying that email is forwarded to '{CCIMailbox.Folder.Inbox}' folder of '{_testConfig.CCI.InsurerMailbox}' mailbox when claim number is not found");
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.InsurerMailbox, CCIMailbox.Folder.Inbox);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.ClaimCommunicationInbound)]
        public void INSU_T537_CreditHire_ClaimNumberInEmailSubject_Motor_ProofOfLoss()
        {
            var testData = BuildTestDataForCCI(EmailScenario.ClaimNumberInSubject, EmailSenderType.CreditHireCo, DocumentType.CreditHireProofOfLoss,false,ShieldProductType.MGP);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.ProofOfLossMailbox);
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.ClaimMailbox, CCIMailbox.Folder.Processed);
            VerifyCCI.VerifyClaimCorrespondence(testData);
            VerifyCCI.VerifyShieldEvent(testData);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.ClaimCommunicationInbound)]
        public void INSU_T531_Supplier_ClaimNumberNotInEmail_NotCatalogued()
        {
            var testData = BuildTestDataForCCI(EmailScenario.NoClaimNumber, EmailSenderType.Supplier, DocumentType.None,isEmailAttachRequired:true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.ClaimMailbox);
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.ClaimMailbox, CCIMailbox.Folder.Processed);
            Reporting.LogMinorSectionHeading($"Verifying that email is forwarded to '{CCIMailbox.Folder.Inbox}' folder of '{_testConfig.CCI.SupplierMailbox}' mailbox when claim number is not found");
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.SupplierMailbox, CCIMailbox.Folder.Inbox);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.TCUInboundEmail)]
        public void INSU_T736_TCU_ClaimNumberWithCaseInSubject_Catalogued()
        {
            var testData = BuildTestDataForCCI(EmailScenario.ClaimNumberInSubject, EmailSenderType.Supplier, DocumentType.TCUIncomingCorrespondence, isEmailAttachRequired: false, isClaimHavingCase:true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.TCUMailbox);
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.TCUMailbox, CCIMailbox.Folder.Processed);
            VerifyCCI.VerifyClaimCorrespondence(testData);
            VerifyCCI.VerifyShieldEvent(testData);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.TCUInboundEmail)]
        public void INSU_T738_TCU_ClaimNumberWithCaseInBody_Catalogued()
        {
            var testData = BuildTestDataForCCI(EmailScenario.ClaimNumberInBody, EmailSenderType.Member, DocumentType.TCUIncomingCorrespondence, isEmailAttachRequired: true, isClaimHavingCase: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.TCUMailbox);
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.TCUMailbox, CCIMailbox.Folder.Processed);
            VerifyCCI.VerifyClaimCorrespondence(testData);
            VerifyCCI.VerifyShieldEvent(testData);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.TCUInboundEmail)]
        public void INSU_T739_TCU_ClaimNumberWithCaseInAttachmentName_Catalogued()
        {
            var testData = BuildTestDataForCCI(EmailScenario.ClaimNumberInAttachmentName, EmailSenderType.CreditHireCo, DocumentType.TCUIncomingCorrespondence, isEmailAttachRequired: true, isClaimHavingCase: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.TCUMailbox);
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.TCUMailbox, CCIMailbox.Folder.Processed);
            VerifyCCI.VerifyClaimCorrespondence(testData);
            VerifyCCI.VerifyShieldEvent(testData);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.TCUInboundEmail)]
        public void INSU_T740_TCU_ClaimNumberWithCaseInAttachmentContent_Catalogued()
        {
            var testData = BuildTestDataForCCI(EmailScenario.ClaimNumberInAttachmentContent, EmailSenderType.Member, DocumentType.TCUIncomingCorrespondence, isEmailAttachRequired: true, isClaimHavingCase: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.TCUMailbox);
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.TCUMailbox, CCIMailbox.Folder.Processed);
            VerifyCCI.VerifyClaimCorrespondence(testData);
            VerifyCCI.VerifyShieldEvent(testData);
        }


        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.TCUInboundEmail)]
        public void INSU_T745_TCU_ClaimNumberNotInEmail_NotCataloguedMoveToExceptionFolder()
        {
            var testData = BuildTestDataForCCI(EmailScenario.NoClaimNumber, EmailSenderType.Member, DocumentType.TCUIncomingCorrespondence);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.TCUMailbox);
            Reporting.LogMinorSectionHeading($"Verifying that email is moved to '{CCIMailbox.Folder.Exceptions}' folder of '{_testConfig.CCI.TCUMailbox}' mailbox when claim number is not found");
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.TCUMailbox, CCIMailbox.Folder.Exceptions);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.TCUInboundEmail)]
        public void INSU_T742_TCU_ClaimNumberWithNoCaseInBody_NotCataloguedMoveToExceptionFolder()
        {
            var testData = BuildTestDataForCCI(EmailScenario.ClaimNumberInBody, EmailSenderType.Supplier, DocumentType.TCUIncomingCorrespondence);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.TCUMailbox);
            Reporting.LogMinorSectionHeading($"Verifying that email is moved to '{CCIMailbox.Folder.Exceptions}' folder of '{_testConfig.CCI.TCUMailbox}' mailbox when claim number is not found");
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.TCUMailbox, CCIMailbox.Folder.Exceptions);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.TCUInboundEmail)]
        public void INSU_T741_TCU_ClaimNumberWithNoCaseInSubject_NotCataloguedMoveToExceptionFolder()
        {
            var testData = BuildTestDataForCCI(EmailScenario.ClaimNumberInSubject, EmailSenderType.Member, DocumentType.TCUIncomingCorrespondence);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.TCUMailbox);
            Reporting.LogMinorSectionHeading($"Verifying that email is moved to '{CCIMailbox.Folder.Exceptions}' folder of '{_testConfig.CCI.TCUMailbox}' mailbox when claim number is not found");
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.TCUMailbox, CCIMailbox.Folder.Exceptions);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.TCUInboundEmail)]
        public void INSU_T743_TCU_ClaimNumberWithNoCaseInAttachmentName_NotCataloguedMoveToExceptionFolder()
        {
            var testData = BuildTestDataForCCI(EmailScenario.ClaimNumberInAttachmentName, EmailSenderType.CreditHireCo, DocumentType.TCUIncomingCorrespondence);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.TCUMailbox);
            Reporting.LogMinorSectionHeading($"Verifying that email is moved to '{CCIMailbox.Folder.Exceptions}' folder of '{_testConfig.CCI.TCUMailbox}' mailbox when claim number is not found");
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.TCUMailbox, CCIMailbox.Folder.Exceptions);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.TCUInboundEmail)]
        public void INSU_T744_TCU_ClaimNumberWithNoCaseInAttachmentContent_NotCataloguedMoveToExceptionFolder()
        {
            var testData = BuildTestDataForCCI(EmailScenario.ClaimNumberInAttachmentContent, EmailSenderType.Member, DocumentType.TCUIncomingCorrespondence);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.TCUMailbox);
            Reporting.LogMinorSectionHeading($"Verifying that email is moved to '{CCIMailbox.Folder.Exceptions}' folder of '{_testConfig.CCI.TCUMailbox}' mailbox when claim number is not found");
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.TCUMailbox, CCIMailbox.Folder.Exceptions);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.TCUInboundEmail)]
        public void INSU_T746_TCU_MultipleClaimNumberWithCaseInSubject_MoveToExceptions()
        {
            var testData = BuildTestDataForCCI(EmailScenario.MultipleClaimNumbersInSubject, EmailSenderType.Member, DocumentType.TCUIncomingCorrespondence, isEmailAttachRequired: false, isClaimHavingCase: true);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.TCUMailbox);
            Reporting.LogMinorSectionHeading($"Verifying that email is moved to '{CCIMailbox.Folder.Exceptions}' folder of '{_testConfig.CCI.TCUMailbox}' mailbox when claim number is not found");
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.TCUMailbox, CCIMailbox.Folder.Exceptions);
        }

        [Test, Category(TestCategory.Claim), Category(TestCategory.Integration), Category(TestCategory.Regression), Category(TestCategory.TCUInboundEmail)]
        public void INSU_T753_TCU_MultipleClaimNumberWithNoCaseInAttachmentContent_MoveToExceptions()
        {
            var testData = BuildTestDataForCCI(EmailScenario.MultipleClaimNumberInAttachmentContent, EmailSenderType.Member, DocumentType.TCUIncomingCorrespondence);
            Reporting.LogTestData(TestContext.CurrentContext.Test.Name, testData.ToString());
            Reporting.LogTestStart();

            GenerateCCIEmail(testData, _testConfig.CCI.TCUMailbox);
            Reporting.LogMinorSectionHeading($"Verifying that email is moved to '{CCIMailbox.Folder.Exceptions}' folder of '{_testConfig.CCI.TCUMailbox}' mailbox when claim number is not found");
            VerifyCCI.VerifyEmailInCCIMailbox(testData, _testConfig.CCI.TCUMailbox, CCIMailbox.Folder.Exceptions);
        }

        /// <summary>
        /// Build test data for CCI scenarios
        /// </summary>
        /// <param name="emailScenario">Email sending scenario</param>
        /// <param name="emailSenderType">Type of email sender</param>
        /// <param name="documentType">Document type on which the email is tagged on Shield</param>
        /// <param name="isEmailAttachRequired">Setting this to True will attach a pdf in the email.Default option false will send email with no attachment</param>
        /// <param name="productType">Specifying the proudct type will find claim belong to that product.default option Null will return claim from any product</param>
        /// <param name="isClaimHavingCase">Setting this to True will return Claim having a TCU investigation linked , Default false will return claim with no TCU investigation linked</param>
        private CCIClaim BuildTestDataForCCI(EmailScenario emailScenario, EmailSenderType emailSenderType, DocumentType documentType, bool isEmailAttachRequired= false, ShieldProductType? productType = null, bool isClaimHavingCase = false)
        {
            var emailSubject = string.Empty;
            var emailBody = string.Empty;
            Attachment attachment = null;
            EmailDocTypeMapping docTypeEmail = null;
            var _cciClaim = new CCIClaim();

            if(isSingleClaimNumberScenario(emailScenario))
            {
                if (documentType==DocumentType.TCUIncomingCorrespondence)
                {
                    var polClaimCase = ShieldClaimDB.FindListOfClaimsForTCU(isClaimHavingCase).ConsumeRandom<PolicyClaimCasePair>();
                    _cciClaim.PolicyClaimAndCase.Add(polClaimCase);
                }
                else
                {
                    var polClaimCase = ShieldClaimDB.FindListOfOpenClaimsFromAllProducts(productType).ConsumeRandom<PolicyClaimCasePair>();
                    _cciClaim.PolicyClaimAndCase.Add(polClaimCase);
                }
            }else if(!emailScenario.Equals(EmailScenario.NoClaimNumber))
            {

                if (documentType == DocumentType.TCUIncomingCorrespondence)
                {
                    _cciClaim.PolicyClaimAndCase = ShieldClaimDB.FindListOfClaimsForTCU(isClaimHavingCase);
                }
                else
                {
                    _cciClaim.PolicyClaimAndCase = ShieldClaimDB.FindListOfOpenClaimsFromAllProducts(productType);
                }                   
            }

            docTypeEmail = emails.FirstOrDefault(x => x.DocumentType.Equals(documentType));
            emailSubject = docTypeEmail.Subject;
            emailBody = docTypeEmail.Body;
            attachment = isEmailAttachRequired ? GenerateEmailAttachment(DataHelper.RandomLetters(10), DataHelper.RandomAlphanumerics(50)) : null;

            switch (emailScenario)
            {
                case EmailScenario.NoClaimNumber:
                    // nothing to do.
                    break;
                case EmailScenario.ClaimNumberInAttachmentName:
                    isEmailAttachRequired = true;
                    attachment = GenerateEmailAttachment(_cciClaim.PolicyClaimAndCase.FirstOrDefault().ClaimNumber, DataHelper.RandomLetters(50));
                    break;
                case EmailScenario.ClaimNumberInAttachmentContent:
                    isEmailAttachRequired = true;
                    attachment = GenerateEmailAttachment(DataHelper.RandomLetters(8), $"{DataHelper.RandomLetters(25)} {_cciClaim.PolicyClaimAndCase.FirstOrDefault().ClaimNumber} {DataHelper.RandomLetters(25)}");
                    break;
                case EmailScenario.MultipleClaimNumberInAttachmentContent:
                    isEmailAttachRequired = true;
                    attachment = GenerateEmailAttachment(DataHelper.RandomLetters(8), $"{DataHelper.RandomLetters(25)} {String.Join(" & ", _cciClaim.PolicyClaimAndCase.Select(e => e.ClaimNumber).ToList())} {DataHelper.RandomLetters(25)}");
                    break;
                case EmailScenario.ClaimNumberInSubject:
                    emailSubject = $"{emailSubject} - {_cciClaim.PolicyClaimAndCase.FirstOrDefault().ClaimNumber}";
                    break;
                case EmailScenario.MultipleClaimNumbersInSubject:
                    emailSubject = emailSubject + String.Join(" & ", _cciClaim.PolicyClaimAndCase.Select(e => e.ClaimNumber).ToList());
                    break;
                case EmailScenario.ClaimNumberInBody:
                    emailBody = $"{emailBody} - {_cciClaim.PolicyClaimAndCase.FirstOrDefault().ClaimNumber}";
                    break;
                default:
                    throw new NotImplementedException("Claim scenario is not implemented!");
            }

            _cciClaim.EmailPayload = new EmailPayload(
                
                to: _testConfig.CCI.ClaimMailbox,
                from: $"{emailSenderType}@{_testConfig.Email.Domain}",
                subject: $"{ emailSubject} - {DataHelper.RandomAlphanumerics(10)}",
                plainTextBody : emailBody
            );
            _cciClaim.EmailScenario = emailScenario;
            _cciClaim.EmailSenderType = emailSenderType;
            _cciClaim.DocumentType = documentType;
            _cciClaim.Attachment = attachment;
            _cciClaim.HasAttachment = isEmailAttachRequired;
            return _cciClaim;
        }

        /// <summary>
        /// Returns TRUE if scenario is for a email which has only one claim number.
        /// If there is no claim number, or there are multiple; return FALSE.
        /// </summary>
        private bool isSingleClaimNumberScenario(EmailScenario scenario) => (scenario != EmailScenario.NoClaimNumber &&
                                                                             scenario != EmailScenario.MultipleClaimNumbersInSubject &&
                                                                             scenario != EmailScenario.MultipleClaimNumberInAttachmentContent);


        /// <summary>
        /// Send email from Mailosaur to CCI mailbox
        /// </summary>
        /// <param name="testData">Test data</param>
        /// <param name="toEmail"> To email address</param>
        private void GenerateCCIEmail(CCIClaim testData, string toEmail)
        {
            var _messageHandler = new MailosaurEmailHandler();
            _messageHandler.SendEmail(toEmail, testData.EmailPayload.Subject, testData.EmailPayload.Body.First().Value, testData.EmailPayload.From.Email,testData.Attachment);
            testData.EmailSentOn = DateTime.Now;
        }

        /// <summary>
        /// Create pdf file attachment that can send along with the CCI email
        /// </summary>
        /// <param name="fileName">file name of the pdf attachment</param>
        /// <param name="attachmentContent">content of the pdf file attachment</param>
        /// <returns>attachment</returns>
        private Attachment GenerateEmailAttachment(string fileName,string attachmentContent)
        {
            var _messageHandler = new MailosaurEmailHandler();
            return _messageHandler.GeneratePDFEmailAttachment(fileName, attachmentContent);
        }

        private static List<EmailDocTypeMapping> EmailCollection()
        {
            var emails = new List<EmailDocTypeMapping>();

            emails.Add(new EmailDocTypeMapping(DocumentType.SupplierEnquiry,        CCIEmails.SupplierEnquiryEmail_Subject, CCIEmails.SupplierEnquiryEmail_Body));
            emails.Add(new EmailDocTypeMapping(DocumentType.CreditHireEmail,        CCIEmails.CreditHireEmail_Subject,      CCIEmails.CreditHireEmail_Body));
            emails.Add(new EmailDocTypeMapping(DocumentType.CreditHireProofOfLoss,  CCIEmails.ProofOfLossEmail_Subject,     CCIEmails.ProofOfLossEmail_Subject));
            emails.Add(new EmailDocTypeMapping(DocumentType.MemberEnquiryEmail,     CCIEmails.MemberEnquiryEmail_Subject,   CCIEmails.MemberEnquiryEmail_Body));
            emails.Add(new EmailDocTypeMapping(DocumentType.MemberInvoiceEmail,     CCIEmails.MemberInvoiceEmail_Subject,   CCIEmails.MemberInvoiceEmail_Body));
            emails.Add(new EmailDocTypeMapping(DocumentType.TCUIncomingCorrespondence,CCIEmails.TCUEmail_Subject,             CCIEmails.TCUEmail_Body));
            emails.Add(new EmailDocTypeMapping(DocumentType.None,                   CCIEmails.ProofOfLossEmail_Subject,     CCIEmails.ProofOfLossEmail_Subject));

            return emails;
        }
    }

    public class EmailDocTypeMapping
    {
        public EmailDocTypeMapping(DocumentType docType, string subject, string body)
        {
            DocumentType = docType;
            Subject = subject;
            Body = body;
        }

        public DocumentType DocumentType { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class CCIClaim
    {
        public List<PolicyClaimCasePair> PolicyClaimAndCase { get; set; } = new List<PolicyClaimCasePair>();
        public EmailPayload EmailPayload { get; set; }
        public EmailSenderType EmailSenderType { get; set; }
        public EmailScenario EmailScenario { get; set; }
        public DocumentType DocumentType { get; set; }
        public Attachment Attachment { get; set; }
        public bool HasAttachment { get; set; }
        public DateTime EmailSentOn { get; set; }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"    Email Test Scenario: {EmailScenario}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Email Sender Type: {EmailSenderType}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            if (PolicyClaimAndCase.IsNullOrEmpty())
            {
                formattedString.AppendLine($"    No Claim number or policy number associated with email.{Reporting.HTML_NEWLINE}");
            }
            else
            {
                foreach (var polClaimAndCase in PolicyClaimAndCase)
                {
                    formattedString.AppendLine($"    Policy Number: {polClaimAndCase.PolicyNumber}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($"    Claim Number:  {polClaimAndCase.ClaimNumber}{Reporting.HTML_NEWLINE}");
                    formattedString.AppendLine($"    Case Number: {polClaimAndCase.CaseNumber}{Reporting.HTML_NEWLINE}");
                }
            }
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"    Email From: {EmailPayload.From.Email}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Email Subject: {EmailPayload.Subject}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Email Body: {EmailPayload.Body.First().Value}{Reporting.HTML_NEWLINE}");
            if (Attachment != null)
            {
                formattedString.AppendLine($"    Attachment File Name: {Attachment.FileName}{Reporting.HTML_NEWLINE}");
                formattedString.AppendLine($"    Attachment Content: {System.Text.Encoding.Default.GetString(Convert.FromBase64String(Attachment.Content))}{Reporting.HTML_NEWLINE}");
            }
            else
            {
                formattedString.AppendLine($"    No attachment is included with the email.{Reporting.HTML_NEWLINE}");
            }
            formattedString.AppendLine($"    Document Type: {DataHelper.GetDescription(DocumentType)}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            return formattedString.ToString();
        }
    }

    public enum EmailSenderType
    {
        [System.ComponentModel.Description("Subrogation")]
        CreditHireCo,
        [System.ComponentModel.Description("Member")]
        Member,
        [System.ComponentModel.Description("Supplier")]
        Supplier
    }

    public enum EmailScenario
    {
        NoClaimNumber,
        ClaimNumberInAttachmentName,
        ClaimNumberInAttachmentContent,
        ClaimNumberInSubject,
        MultipleClaimNumbersInSubject,
        MultipleClaimNumberInAttachmentContent,
        ClaimNumberInBody
    }

    public enum DocumentType
    {
        [System.ComponentModel.Description("None")]
        None,
        [System.ComponentModel.Description("Subrogation - Credit Hire Email")]
        CreditHireEmail,
        [System.ComponentModel.Description("Subrogation - Proof of Loss Email")]
        CreditHireProofOfLoss,
        [System.ComponentModel.Description("Supplier - Enquiry Email")]
        SupplierEnquiry,
        [System.ComponentModel.Description("Member - Enquiry Email")]
        MemberEnquiryEmail,
        [System.ComponentModel.Description("Member - Invoice Email")]
        MemberInvoiceEmail,
        [System.ComponentModel.Description("TCU Incoming Correspondence")]
        TCUIncomingCorrespondence,
    }
}
