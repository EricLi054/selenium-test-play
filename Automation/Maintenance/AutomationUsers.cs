using Newtonsoft.Json;
using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Contacts;
using System;
using System.IO;
using System.Linq;
using UIDriver.Helpers;

using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.General;

namespace Maintenance
{
    [Property("Functional", "Test automation support tasks")]
    public class AutomationUsers : BaseUITest
    {
        // Search criteria to find "Claims = Total Loss" organizational unit
        // to add new test users to.
        private const string SHIELD_ORGANIZATION_TYPE_FOR_TEST_USERS = "Agency";
        private const string SHIELD_ORGANIZATION_NAME_FOR_TEST_USERS = "Total Loss";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Scheduling of the required daily Shield batch jobs for data health.");
        }

        [Test, Category("MaintenanceAutomationUsers")]
        public void CreateAutomationUsersInShield()
        {
            var usersToCreate = ReadAutomationUserConfig();

            _browser.OpenShieldAndAwaitManualLogin();

            for (int i = usersToCreate.StartIndexInclusive; i <= usersToCreate.EndIndexInclusive; i++)
            {
                var firstName = usersToCreate.FirstName;
                var lastName  = $"{usersToCreate.Surname}{i}";
                var automationUser = AutomationUserAsContact(firstName, lastName);

                // Check if user already exists in Shield (SQL query)
                var databaseRecord = ShieldContacts.FindContactByName(firstName, lastName);

                if (databaseRecord.Count == 0)
                {
                    automationUser.Id = DataHelper.CreateContactInShieldViaAPI(automationUser);
                    Reporting.IsTrue(!string.IsNullOrEmpty(automationUser.Id), "contact ID is not null, to check that contact was successfully created in Shield.");
                    Reporting.Log($"Created new contact for {firstName} {lastName}, ID: {automationUser.Id}");
                }
                else if (databaseRecord.Count > 1)
                {
                    Reporting.Error($"This user ({firstName} {lastName}) has {databaseRecord.Count} instances and is unusable.");
                }
                else
                {
                    if (databaseRecord[0].ContactRoles.Where(x => x == ContactRole.StaffMember).Any())
                    {
                        Reporting.Log($"User {firstName} {lastName} is already set as a staff member.");
                        continue;
                    }
                }

                HelpersShield.AddContactAsStaffMember(_browser,
                                                      SHIELD_ORGANIZATION_TYPE_FOR_TEST_USERS,
                                                      SHIELD_ORGANIZATION_NAME_FOR_TEST_USERS,
                                                      automationUser,
                                                      usersToCreate.Password);
            }

            _browser.LogoutShieldAndCloseBrowser();
        }

        /// <summary>
        /// This script is to be run to check that all of the expected Test Automation users 
        /// already exist in a given Shield environment. If it fails, then someone should run 
        /// CreateAutomationUsersInShield from the Debug VM.
        /// </summary>
        [Test, Category(TestCategory.CheckAutomationUsers)]
        public void CheckForAutomationUsersInShield()
        {
            var usersToCheck = ReadAutomationUserConfig();

            for (int i = usersToCheck.StartIndexInclusive; i <= usersToCheck.EndIndexInclusive; i++)
            {
                var firstName = usersToCheck.FirstName;
                var lastName = $"{usersToCheck.Surname}{i}";
                var automationUser = AutomationUserAsContact(firstName, lastName);

                // Check if Contact already exists in Shield (SQL query)
                var databaseRecord = ShieldContacts.FindContactByName(firstName, lastName);
                var contactId = databaseRecord?.FirstOrDefault()?.Id;

                if (databaseRecord.Count > 1)
                {
                    Reporting.Error($"Found multiple ({databaseRecord.Count}) contacts with expected name ({firstName} {lastName}) " +
                        $"which will cause problems running CreateAutomationUsersInShield. Please investigate manually for this envirnonment.");
                }
                else if (databaseRecord.Count == 1)
                {
                    Reporting.Log($"Contact for Shield Automation Test User exists as expected. " +
                        $"Contact Id =  '{contactId}'");
                    if (databaseRecord[0].ContactRoles.Where(x => x == ContactRole.StaffMember).Any())
                    {
                        Reporting.Log($"Contact {firstName} {lastName} has the Staff Member role as expected.");
                    }

                    Reporting.Log($"Checking for actual Shield User related to this Contact ID");
                    ShieldContacts.VerifyContactHasUserAccountWithRole(contactId);
                }
                else
                {
                    Reporting.Error($"Unable to find Contact for Shield Automation Test User named {firstName} {lastName}. " +
                        $"Please run the CreateAutomationUsersInShield script via the debug VM");
                }
            }
        }
        
        /// <summary>
        /// Read in the batchjoblist.json file. This file defines the list
        /// of Shield batch jobs that we wish to ensure are scheduled
        /// and the time at which they should run each day.
        /// </summary>
        /// <returns></returns>
        public AutomationUsersConfigElement ReadAutomationUserConfig()
        {
            var config = new AutomationUsersConfigElement();
            string file = $"{TestContext.CurrentContext.TestDirectory}\\AutomationUsers.json";
            if (File.Exists(file))
            {
                config = JsonConvert.DeserializeObject<AutomationUsersConfigElement>(File.ReadAllText(file));
            }

            if (string.IsNullOrEmpty(config.Password))
            {
                Reporting.Error("A password must be provided for these Shield users. Do not leave this field blank.");
            }

            return config;
        }

        public Contact AutomationUserAsContact(string firstName, string surname)
        {
            return new ContactBuilder().InitialiseRandomIndividual()
                                       .WithGender(Gender.Male)
                                       .WithTitle(Title.Mr)
                                       .WithFirstName(firstName)
                                       .WithoutMiddleName()
                                       .WithSurname(surname)
                                       .Build();
        }
    }
}