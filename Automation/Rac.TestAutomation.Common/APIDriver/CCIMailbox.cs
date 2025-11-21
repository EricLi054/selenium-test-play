using static Rac.TestAutomation.Common.Constants.General;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Azure.Identity;
using Microsoft.Graph;
using System.Linq;
using System;

namespace Rac.TestAutomation.Common.APIDriver
{
    public class CCIMailbox
    {
        private Config _config;
        private ClientSecretCredential _clientSecretCredential;
        private GraphServiceClient _graphClient;

        public CCIMailbox() 
        {
            _config = Config.Get();
            if (string.IsNullOrEmpty(_config.CCI?.ClaimMailbox) ||
                string.IsNullOrEmpty(_config.CCI?.InsurerMailbox) ||
                string.IsNullOrEmpty(_config.CCI?.SupplierMailbox) ||
                string.IsNullOrEmpty(_config.CCI?.TenantId) ||
                string.IsNullOrEmpty(_config.CCI?.ClientID) ||
                string.IsNullOrEmpty(_config.CCI?.ClientSecret))
            { Reporting.Error("CCI exchange Server configuration is incomplete. Cannot reach exchange server."); }

            _clientSecretCredential = new ClientSecretCredential(_config.CCI?.TenantId, _config.CCI.ClientID, _config.CCI.ClientSecret);
            _graphClient = new GraphServiceClient(_clientSecretCredential);
        }


        /// <summary>
        /// Will contact the email server to search for an email by subject line.
        /// </summary>
        /// <param name="mailbox">Mail box name in which search to be performed</param>
        /// <param name="folderName">Name of the folder in the mailbox search to be performed</param>
        /// <param name="subjectLine">Subject line to search for</param>
        /// <returns>return all emails containing the subject; return null if no match available</returns>
        public async Task<List<Message>> FindEmailsFromFolderBySubject(string mailbox, Folder folderName, string subjectLine)
        {
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T150SEC);

            try
            {
                var folders = await _graphClient.Users[mailbox].MailFolders.GetAsync();
                var folder = folders.Value.FirstOrDefault(f => f.DisplayName.Equals(folderName.ToString(), StringComparison.OrdinalIgnoreCase));

                if (folder == null)
                {
                    throw new InvalidOperationException($"Folder '{folderName}' not found.check the folder name is correct and present on '{mailbox}'.");
                }

                do
                {
                    var messages = await _graphClient.Users[mailbox].MailFolders[folder.Id].Messages.GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Filter = $"contains(subject, '{subjectLine}')";
                        requestConfiguration.QueryParameters.Top = 25;
                    });
                    if (messages.Value.Count == 0)
                    {
                        await Task.Delay(1000);
                    }
                    else
                    {
                        return messages.Value;
                    }
                } while (DateTime.Now < endTime);
            }
            catch (Exception ex) when (ex is ServiceException || ex is AuthenticationFailedException)
            {   
               throw new InvalidOperationException($"Unable to fetch email from the exchange server {mailbox} ; check tenant id, client id and client secrets on config.json: {ex.Message}");
            }
            return null;
        }

        public enum Folder
        {
            Inbox,
            Processed,
            Exceptions
        }
    }
}

