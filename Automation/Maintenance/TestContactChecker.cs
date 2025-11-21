using NUnit.Framework;
using NUnit.Framework.Internal;
using Rac.TestAutomation.Common;
using System;

namespace Maintenance
{
    [Property("Support", "Test automation support utility")]
    public class TestContactChecker : BaseNonUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Support utility for verify coded test contact data.");
        }

        /// <summary>
        /// This test is a support test to allow checking of the hardcoded
        /// multi-match contacts in the Contact Builder to ensure that
        /// they are still listed as multi-match contacts in member central.
        /// 
        /// This test should be run against the real member central in SIT
        /// and UAT. All flagged contact items should be removed from the
        /// hardcoded list and eventually replaced with new multi-match
        /// contacts.
        /// </summary>
        [Test]
        public void CheckContactBuilderMultiMatchContactList()
        {
            var badContactsFound   = 0;
            var multiMatchContacts = ContactBuilder.ListOfMultiMatchContactOptions();

            if (Config.Get().IsMCMockEnabled())
            {
                Reporting.Error("This utility is intended to verify coded multimatch contacts against a real MC.");
            }

            var mcApi = MemberCentral.GetInstance();

            Console.WriteLine($"We have {multiMatchContacts.Count} total contacts to check.");

            foreach (var contactPartial in multiMatchContacts)
            {
                try
                {
                    var contactWhole = Contact.InitFromMCByPersonId(contactPartial.PersonId);
                    if (!mcApi.IsMultiMatch(contactWhole))
                    {
                        badContactsFound++;
                        Reporting.Log($"TEST DATA ERROR with Multi Match Contacts. Coded contact is no longer a " +
                            $"multi-match suitable, review CRM:{contactPartial.PersonId} Name:{contactWhole.FirstName} " +
                            $"Mobile:{contactWhole.MobilePhoneNumber} DoB:{contactWhole.DateOfBirthString}");
                    }
                }
                catch(NUnitException ex)
                {
                    badContactsFound++;
                    Reporting.Log($"TEST DATA ERROR with {contactPartial.PersonId}. Most likely contact is in bad state, such as 299 sync error. {ex.Message}. {ex.InnerException}");
                }
            }

            Reporting.Log($"Counted {badContactsFound} (of {multiMatchContacts.Count}) contacts which are not in the expected multi-match state in MC.");
            Reporting.AreEqual(0, badContactsFound, $"if this has errored, check logs for contacts which are no longer multimatch in {Config.Get().Azure.MemberCentral.APIEnv}");
        }
    }
}
