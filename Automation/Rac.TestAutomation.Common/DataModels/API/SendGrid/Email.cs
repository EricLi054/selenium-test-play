using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

using static Rac.TestAutomation.Common.Constants.ClaimsGeneral;

namespace Rac.TestAutomation.Common.API
{
    /// <summary>
    /// This is not a comprehensive list, just the ones which
    /// we're using. So if you need something new, please add
    /// it.
    /// </summary>
    public enum ContentType
    {
        //text/plain
        [Description("text/plain")]
        TextPlain,
        [Description("application/xml")]
        ApplicationXML,
        [Description("application/json")]
        ApplicationJSON,
        Unknown
    }

    public class EmailPayload
    {
        public EmailPayload(string to, string from, string subject, string plainTextBody)
        {
            Addressees = new List<EmailRecipients>();
            Addressees.Add(new EmailRecipients(to));

            From = new EmailAddress(from);
            Subject = subject;
            Body = new List<EmailContent>();
            Body.Add(new EmailContent(ContentType.TextPlain, plainTextBody));
        }

        public void AddAttachment(string base64Content, ContentType type, string filename)
        {
            if (Attachments == null)
            {
                Attachments = new List<EmailAttachment>();
            }
            Attachments.Add(new EmailAttachment(base64Content, type, filename));
        }

        [JsonProperty("personalizations")]
        public List<EmailRecipients> Addressees { get; set; }
        [JsonProperty("from")]
        public EmailAddress From { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("content")]
        public List<EmailContent> Body { get; set; }
        [JsonProperty("attachments")]
        public List<EmailAttachment> Attachments { get; set; }
    }

    public class EmailAddress
    {
        public EmailAddress(string address, string name)
        {
            Email = address;
            Name = name;
        }

        /// <summary>
        /// shorthand constructor which will build the name from the email
        /// address given.
        /// </summary>
        /// <param name="address"></param>
        public EmailAddress(string address)
        {
            Email = address;
            Name = address.Substring(0, address.IndexOf('@')).Replace('.',' ');
        }

        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class EmailRecipients
    {
        /// <summary>
        /// Shorthard constructor to create an email recipient
        /// list where we just have one email to be put in the
        /// "To" list, and no others.
        /// </summary>
        /// <param name="addressee"></param>
        public EmailRecipients(string addressee)
        {
            To = new List<EmailAddress>()
            {
                new EmailAddress(addressee)
            };
        }

        [JsonProperty("to")]
        public List<EmailAddress> To { get; set; }
    }

    public class EmailContent
    {
        public EmailContent(ContentType type, string content)
        {
            Type = type;
            // SendGrid will not permit empty message bodies. So use a whitespace.
            Value = string.IsNullOrEmpty(content) ? " " : content;
        }

        public ContentType Type
        {
            get
            {
                var type = ContentType.Unknown;
                try
                { type = DataHelper.GetValueFromDescription<ContentType>(TypeString); }
                catch
                {
                    // Unknown type on exception
                }

                return type;
            }
            set => TypeString = value.GetDescription();
        }
        [JsonProperty("type")]
        public string TypeString { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class EmailAttachment
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64Content">base64 encoded string as this will be going via a JSON payload</param>
        /// <param name="type"></param>
        /// <param name="filename">the filename to present to the recipinet for this provided attachment</param>
        public EmailAttachment(string base64Content, ContentType type, string filename)
        {
            Content = base64Content;
            Type = type;
            Filename = filename;
        }

        [JsonProperty("content")]
        public string Content { get; set; }
        public ContentType Type
        {
            get
            {
                var type = ContentType.Unknown;
                try
                { type = DataHelper.GetValueFromDescription<ContentType>(TypeString); }
                catch { }
                return type;
            }
            set => TypeString = value.GetDescription();
        }
        [JsonProperty("type")]
        public string TypeString { get; set; }
        [JsonProperty("filename")]
        public string Filename { get; set; }
    }
}
