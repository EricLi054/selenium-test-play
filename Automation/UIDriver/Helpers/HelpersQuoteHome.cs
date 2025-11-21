using Rac.TestAutomation.Common;
using System;
using UIDriver.Pages;
using UIDriver.Pages.B2C;

namespace UIDriver.Helpers
{
    public static class HelpersQuoteHome
    {
        public static void LaunchPageBeginNewHomeQuote(this Browser browser)
        {
            using (var launchPage = new LaunchPage(browser))
            {
                launchPage.ClickNewHomeQuote();
            }

            using (var quotePage1 = new HomeQuote1Details(browser))
            {
                quotePage1.WaitForPage();
            }
        }
    }
}
