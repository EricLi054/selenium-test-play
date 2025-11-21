using Rac.TestAutomation.Common;
using System;
using UIDriver.Pages;
using UIDriver.Pages.B2C;

namespace UIDriver.Helpers
{
    public static class HelpersQuotePet
    {
        public static void LaunchPageBeginNewPetQuote(this Browser browser)
        {
            using (var launchPage = new LaunchPage(browser))
            {
                launchPage.ClickNewPetQuote();
            }

            using (var quotePage1 = new PetQuote1Details(browser))
            {
                quotePage1.WaitForPage();
            }
        }
    }
}
