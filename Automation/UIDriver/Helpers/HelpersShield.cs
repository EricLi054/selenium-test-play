using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Contacts;
using System;
using UIDriver.Pages.MicrosoftAD;
using UIDriver.Pages.Shield;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Helpers
{
    public static class HelpersShield
    {
        // Strings to use for role search operations
        private const string ROLE_SEARCH_STRING_MASTER       = "Master";
        private const string ROLE_SEARCH_STRING_SYSTEM_ADMIN = "System Administrator";

        /// <summary>
        /// Opens Shield URL (from config.json) and logs in
        /// with the configured Shield user credentials.
        /// </summary>
        /// <param name="browser"></param>       
        public static void OpenShieldAndLogin(this Browser browser)
        {
            // Logic checks the format of URL and that it is a "https" URL.
            Uri uriResult;
            var isValidHttpsURI = Uri.TryCreate(Config.Get().Shield.Web.Url, UriKind.Absolute, out uriResult) &&
                                  uriResult.Scheme == Uri.UriSchemeHttps;

            if (!isValidHttpsURI)
            {
                throw new InvalidOperationException("Need a URL defined for Shield Web, including https prefix.");
            }

            // Shield only officially supports Edge browser.
            browser.OpenUrl(TargetBrowser.Edge, Config.Get().Shield.Web.Url, TargetDevice.Windows11);

            using (var authPage = new Authentication(browser))
            using (var loginPage = new ShieldLogin(browser))
            using (var homePage = new ShieldSearchPage(browser))
            {
                if (string.IsNullOrEmpty(Config.Get().Shield.Web?.User))
                {
                    var userLogin = new Credentials
                    {
                        User = ShieldContacts.FindAvailableShieldLogin(),
                        Pwd = Config.Get().Shield.Web.Pwd
                    };
                    authPage.WaitForADLoginPageOrDesiredLandingPage(homePage, userLogin);
                }
                else
                {
                    loginPage.WaitForPage(WaitTimes.T10SEC);
                    loginPage.LoginUser(Config.Get().Shield.Web.User, Config.Get().Shield.Web.Pwd);
                }
                homePage.WaitForPage();
            }
        }

        /// <summary>
        /// Opens Shield URL (from config.json) and waits up to 5 minutes for a
        /// user to log in with their AD account so it can create the Shield
        /// users for selenium automation test suites based off the data in the
        /// AutomationUsers.json file.
        ///
        /// IMPORTANT: User running this must have the following roles:
        ///             - Master Role
        ///             - User Administrator
        /// </summary>
        /// <param name="browser"></param>
        public static void OpenShieldAndAwaitManualLogin(this Browser browser)
        {
            // Logic checks the format of URL and that it is a "https" URL.
            Uri uriResult;
            var isValidHttpsURI = Uri.TryCreate(Config.Get().Shield.Web.Url, UriKind.Absolute, out uriResult) &&
                                  uriResult.Scheme == Uri.UriSchemeHttps;

            if (!isValidHttpsURI)
            {
                throw new InvalidOperationException("Need a URL defined for Shield Web, including https prefix.");
            }

            // Shield only officially supports Edge browser.
            browser.OpenUrl(TargetBrowser.Edge, Config.Get().Shield.Web.Url, TargetDevice.Windows11);

            using (var homePage = new ShieldSearchPage(browser))
            {
                homePage.WaitForPage(300);
            }
        }

        public static void LogoutShieldAndCloseBrowser(this Browser browser)
        {
            var success = false;

            using (var homePage = new ShieldSearchPage(browser))
            using (var loginPage = new ShieldLogin(browser))
            using (var microsoftPage = new MyApplications(browser))
            {
                var endtime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
                
                do
                {
                    // Attempt log out. We trap the exception as we sometimes might be
                    // retrying a logout operation, thinking Shield didn't process it
                    // but it actually was just delayed.
                    try { homePage.Logout(); }
                    catch (NoSuchElementException)
                    {
                        Reporting.Log("Logout button not found.");
                    }

                    // Check for URL is a workaround documented in https://rac-wa.atlassian.net/browse/RAI-323
                    System.Threading.Thread.Sleep(SleepTimes.T1SEC);
                    Reporting.Log($"Attempting to confirm logout. Browser URL = {browser.Driver.Url}");
                    success = loginPage.IsDisplayed() || microsoftPage.IsDisplayed() || browser.Driver.Url.Contains("microsoftonline.com");
                } while (!success && endtime > DateTime.Now);
            }
            if (!success)
            {
                Reporting.Error("Unable to log out of Shield within expected time.");
            }
            else
            {
                Reporting.Log("Logged out of Shield, closing browser.");
            }
            browser.CloseBrowser();
        }

        /// <summary>
        /// Convenience method to login to Shield and find a claim
        /// via quick search. The method will complete with the
        /// Dependencies Tree tab selected.
        /// </summary>
        public static void LoginToShieldAndFindClaim(this Browser browser, string claimNumber)
        {
            browser.OpenShieldAndLogin();

            using (var pageHome = new ShieldSearchPage(browser))
            using (var pageClaimDetails = new ShieldClaimDetailsPage(browser))
            {
                // TODO AUNT-224 are we able to expect/support QuickSearch in Shield? 
                pageClaimDetails.SlowSearchByClaimNumber(claimNumber);
                pageClaimDetails.WaitForPage();

                pageClaimDetails.CurrentTab = ShieldClaimDetailsPage.CLAIM_TABS.DependenciesTree;
            }
        }        

        /// <summary>
        /// Navigates to the Organizational Structure manager view in
        /// Shield and adds the given contact to orginzational unit
        /// idenitified by the search criteria of org type and org name.
        /// 
        /// User will have a username defined by their combined first
        /// and last names, and the given password. They will be assigned
        /// System Administrator and Master roles.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="organizationType"></param>
        /// <param name="organizationName"></param>
        /// <param name="contact"></param>
        /// <param name="pasword"></param>
        public static void AddContactAsStaffMember(Browser browser,
                                                   string organizationType,
                                                   string organizationName,
                                                   Contact contact,
                                                   string password)
        {
            using (var homePage = new ShieldSearchPage(browser))
            {
                homePage.OpenSecurityManager();
            }

            // Navigate to Maintain Organization Structure page
            using (var securityManager = new SecurityManager(browser))
            using (var securityManagerPromotion = new SecurityManagerPromotion(browser))
            {
                securityManager.SelectTestPromotionAndClickNext();
                securityManagerPromotion.FindAgencyAndBeginAddEmployeeProcess(organizationType,
                                                                              organizationName);

                AddContactToAgencyAndSetLogin(browser, contact, password);

                securityManagerPromotion.ReturnToPreviousPage();
                securityManager.ReturnToPreviousPage();
            }
        }

        private static void AddContactToAgencyAndSetLogin(Browser browser, Contact contact, string password)
        {
            // Add Employee, then search for automation contact by name
            using (var newEmployee = new NewEmployee(browser))
            using (var searchPage = new ShieldSearchPage(browser))
            using (var searchResultsPage = new ContactSearchResults(browser))
            using (var findRoles = new FindEmployeeRoles(browser))
            {
                newEmployee.SetPositionToStaffMemberAndBeginFindContact();
                searchPage.FindContactByName(contact);
                searchResultsPage.SelectFirstResult();

                // Set username and password in the new user card
                newEmployee.CompleteNewShieldUserFields(contact, password);

                // Add Master role.
                newEmployee.InitiateAddingRoleToUserAccount();
                findRoles.GrantRequestedRole(ROLE_SEARCH_STRING_MASTER);
                // Add System Administrator role
                newEmployee.InitiateAddingRoleToUserAccount();
                findRoles.GrantRequestedRole(ROLE_SEARCH_STRING_SYSTEM_ADMIN);

                // Save all changes to organization structure
                newEmployee.SaveEmployeeDetails();
                newEmployee.WaitForAndClearGenericConfirmationDialog();
            }
        }
    }
}
