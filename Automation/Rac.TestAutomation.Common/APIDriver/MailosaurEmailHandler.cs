using Mailosaur;
using Mailosaur.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.Constants.General;
using Attachment = Mailosaur.Models.Attachment;
using Message = Mailosaur.Models.Message;

namespace Rac.TestAutomation.Common
{
    public class MailosaurEmailHandler : BaseAPI
    {
        private const string EFT = "EFT";
        private const string RetrieveQuote = "RetrieveQuote";
        private Config _config;

        public MailosaurEmailHandler() : base()
        {
            _config = Config.Get();
            if (string.IsNullOrEmpty(_config.Email?.Domain) ||
                string.IsNullOrEmpty(_config.Email?.ServerID) ||
                string.IsNullOrEmpty(_config.Email?.APIKey))
            { Reporting.Error("Email configuration is incomplete. Cannot reach Mailosaur server."); }
        }

        /// <summary>
        /// Will send email from Mailosaur
        /// </summary>
        /// <param name="toEmail"> to email address</param>
        /// <param name="subject">subject of the email</param>
        /// <param name="body">body of the email</param>
        /// <param name="fromEmail">email for the 'from' address, if null a random email prefix is generated</param>
        /// <param name="attachment">Optional list of attachment that can be attached to email</param>
        public void SendEmail(string toEmail, string subject, string body, string fromEmail = null, Attachment attachment = null)
        {
            var mailosaur = new MailosaurClient(_config.Email.APIKey);
            var fromEmailAddress = fromEmail == null ? $"{DataHelper.RandomLetters(10)}@{_config.Email.Domain}" : fromEmail;
            var attachmentCollection = attachment == null ? null : new Attachment[] { attachment };

            mailosaur.Messages.Create
            (
                _config.Email.ServerID, new MessageCreateOptions()
                {
                    To = toEmail,
                    From = fromEmailAddress,
                    Subject = subject,
                    Html = $"<p>{body}</p>",
                    Send = true,
                    Attachments = attachmentCollection
                }
            );

            Reporting.Log($"Attempted to send email to '{toEmail}' from '{fromEmailAddress}'");
        }

        /// <summary>
        /// Will contact the email server to search for an email or SMS by search criteria.
        /// To assist in limiting the chance of finding emails or SMS from another test, the window
        /// of time to check back through past emails or SMS can be set.
        /// NOTE: Mailosaur will not search back in time more than 1 hour.
        /// </summary>
        /// <param name="criteria">Criteria to search for</param>
        /// <param name="withinPastSeconds">Number of seconds in the past to search up to</param>
        /// <param name="retrySeconds">To allow for delayed email delivery, the method can be told to retry for a period of time</param>
        /// <returns></returns>
        /// 
        private async Task<Message> Search(SearchCriteria criteria, int withinPastSeconds = 300, int retrySeconds = WaitTimes.T30SEC)
        {
            var mailosaur = new MailosaurClient(_config.Email.APIKey);
            var endTime = DateTime.Now.AddSeconds(retrySeconds);
            do
            {
                try
                {
                    return mailosaur.Messages.Get(_config.Email.ServerID, criteria, receivedAfter: DateTime.Now.ToUniversalTime().AddSeconds(-withinPastSeconds));
                }
                catch
                {
                    await Task.Delay(1000); // 1 second wait before polling Mailosaur again.
                }
            } while (DateTime.Now < endTime);

            if (criteria.Subject != null)
            {
                throw new MailosaurException($"Could not find matching message using email subject line {criteria.Subject} even after {retrySeconds}seconds. "+
                                              "If there are consistent failures, check the function app's email override setting, and that the automation is "+
                                              "configured to look in the correct Mailosaur inbox (usually @u8pbw776.mailosaur.net). " +
                                             $"Current domain configured is '{Config.Get().Email.Domain}'",
                                              "Email not found using Subject Line filter");
            }
            else if (criteria.SentTo != null)
            {
                throw new MailosaurException($"Could not find matching message using recipient email {criteria.SentTo} even after {retrySeconds}seconds. "+
                                              "If there are consistent failures, check the function app's email override setting, and that the automation is "+
                                              "configured to look in the correct Mailosaur inbox (usually @u8pbw776.mailosaur.net). " +
                                             $"Current domain configured for automation is '{Config.Get().Email.Domain}'", "Email not found using Sent To filter");
            }
            else
            {
                throw new MailosaurException($"Could not find matching message using after {retrySeconds}seconds. "+
                                              "If there are consistent failures, check the function app's email override setting, and that the automation is "+
                                              "configured to look in the correct Mailosaur inbox (usually @u8pbw776.mailosaur.net). " +
                                             $"Current domain configured for automation is '{Config.Get().Email.Domain}'", "Email not found");
            }
        }

        /// <summary>
        /// Will contact the email server to search for an email by recipient email address.
        /// To assist in limiting the chance of finding emails from another test, the window
        /// of time to check back through past emails can be set.
        /// NOTE: Mailosaur will not search back in time more than 1 hour.
        /// </summary>
        /// <param name="emailRecipient">Recipient email address to search for</param>
        /// <param name="withinPastSeconds">Number of seconds in the past to search up to</param>
        /// <param name="retrySeconds">To allow for delayed email delivery, the method can be told to retry for a period of time</param>
        /// <returns></returns>
        /// 
        public Task<Message> FindEmailByRecipient(string emailRecipient, int withinPastSeconds = 300, int retrySeconds = WaitTimes.T30SEC)
        {
            if (!emailRecipient.EndsWith(_config.Email.Domain))
            { Reporting.Error($"Provided email address ({emailRecipient}) is not a valid Mailosaur address."); }

            var criteria = new SearchCriteria() { SentTo = emailRecipient };

            return Search(criteria, withinPastSeconds, retrySeconds);
        }

        /// <summary>
        /// Will contact the email server to search for an email by subject line.
        /// To assist in limiting the chance of finding emails from another test, the window
        /// of time to check back through past emails can be set.
        /// NOTE: Mailosaur will not search back in time more than 1 hour.
        /// </summary>
        /// <param name="subjectLine">Subject line to search for</param>
        /// <param name="withinPastSeconds">Number of seconds in the past to search up to</param>
        /// <param name="retrySeconds">To allow for delayed email delivery, the method can be told to retry for a period of time</param>
        /// <returns></returns>
        public Task<Message> FindEmailBySubject(string subjectLine, int withinPastSeconds = 300, int retrySeconds = WaitTimes.T30SEC)
        {
            var criteria = new SearchCriteria() { Subject = subjectLine };
            return Search(criteria, withinPastSeconds, retrySeconds);
        }

        /// <summary>
        /// Connect mailosaur server to find the SMS by specific body text
        /// To assist in limiting the chance of finding SMS from another test, the window
        /// of time to check back through past SMS can be set.
        /// **NOTE: The Config on the AUT is forcing all outbound SMS to Mailosaur test mobile number on NPE, 
        /// regardless of what number is on the member's contact info. Any failure here may due to config is broken
        /// or the Mailosaur test mobile number is not added on Mailosaur server
        /// </summary>
        /// <param name="body">SMS body line to search for</param>
        /// <param name="withinPastSeconds">Number of seconds in the past to search up to</param>
        /// <param name="retrySeconds">To allow for delayed sms delivery, the method can be told to retry for a period of time</param>
        /// <returns></returns>
        public Task<Message> FindSMSByBody(string body, int withinPastSeconds = 300, int retrySeconds = WaitTimes.T30SEC)
        {

            Task<Message> task;
            var criteria = new SearchCriteria() { Body = body };
            try
            {
                task = Search(criteria, withinPastSeconds, retrySeconds);
                task.WaitAndUnwrapException();
            }
            catch (MailosaurException)
            {
                throw new MailosaurException($"Could not find matching SMS on Mailosaur server {_config.Email.ServerID} after {retrySeconds}seconds. " +
                                              "Check the function app's SMS override configuration in Azure that it is the right number. Also be " +
                                              "aware that some services still route through ICPS which will further override the SMS number used. " +
                                              "Lastly, double check the automation configuration that it is looking in the correct Mailosaur inbox.",
                                             $"Time out, criteria not found, or mobile number not added onto server {_config.Email.ServerID}. ");
            }
            return task;
        }

        /// <summary>
        /// Find the CSFS link in the email
        /// </summary>
        /// <param name="message">Email message</param>
        public string FindCSFSLink(Message message)
        {
            //Find the Cash Settlement Fact Sheet links or throw an exception if not found.
            var csfsLinks = message?.Html.Links?.Where(x => x.Href.Contains(EFT));
            Reporting.IsNotNull(csfsLinks, "Cash Settlement Fact Sheet link was found in email.");
            
            var csfsLink = csfsLinks.FirstOrDefault(x => x.Href.Contains(EFT));
            var csfsLinkAlt = csfsLinks.LastOrDefault(x => x.Href.Contains(EFT));
               
            //TODO ISE-8261 - If this bug is fixed, we can remove the `Trim()` from this comparison and return to raw comparison of the exact URLs.
            Reporting.AreEqual(csfsLink.Href.Trim(), csfsLinkAlt.Href.Trim(), "the EFT link on the image and text link 'Here' is the same (after Trim() is applied to allow for superflous spaces)");
            return csfsLink.Href;
        }

        /// <summary>
        /// Find the Retrieve quote link in the email
        /// </summary>
        /// <param name="message">Email message</param>
        public string FindRetrieveQuoteLink(Message message)
        {
            var retrieveQuoteLink = message?.Html.Links?.FirstOrDefault(x => x.Href.Contains(RetrieveQuote));
            Reporting.IsNotNull(retrieveQuoteLink, "Retrieve Quote link was found in email");
            return retrieveQuoteLink?.Href;
        }

        /// <summary>
        /// Find the OTP from the  SMS
        /// </summary>
        /// <param name="message">OTP Number</param>
        /// <returns>null if no OTP code found.</returns>
        public string FindOTP(Message message)
        {
            var matches = Regex.Matches(message.Text.Body, "([0-9]{6})", RegexOptions.IgnoreCase);
            return matches == null ? null : matches[0].ToString();
        }

        /// <summary>
        /// Create PDF attachment that can send via Mailosaur
        /// </summary>
        /// <param name="fileName">file name of the pdf attachment</param>
        /// <param name="attachmentContent">content of the pdf file attachment</param>
        /// <returns>attachment</returns>
        public Attachment GeneratePDFEmailAttachment(string fileName, string attachmentContent)
        {
            string fileNameUpdated = $"{fileName}.pdf";
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.Content().Text(attachmentContent);
                });
            }).GeneratePdf(fileNameUpdated);
            byte[] pdfBytes = File.ReadAllBytes(fileNameUpdated);
            string base64Pdf = Convert.ToBase64String(pdfBytes);

            var attachment = new Attachment()
            {
                FileName = fileNameUpdated,
                ContentType = "application/pdf",
                Content = base64Pdf
            };
            return attachment;
        }
    }
}
