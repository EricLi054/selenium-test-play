using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.TestData.Claim;
using System;
using UIDriver.Helpers;
using UIDriver.Pages;
using UIDriver.Pages.B2C;
using UIDriver.Pages.Claims;

namespace Tests.ActionsAndValidations
{
    public static class ActionsClaimAnonymous
    {
        public static void LaunchPageBeginNewAnonymousMotorClaim(this Browser browser)
        {
            LaunchPage.OpenB2CLandingPageAndFeatureToggles(browser);
            using (var launchPage = new LaunchPage(browser))
            using (var preamblePage = new ClaimPreamble(browser))
            {
                launchPage.ClickNewAnonymousClaim();
                preamblePage.WaitForPage();
                preamblePage.ClickNewMotorCarClaim();
            }

        }

        public static void LaunchPageBeginNewAnonymousHomeClaim(this Browser browser)
        {
            LaunchPage.OpenB2CLandingPageAndFeatureToggles(browser);
            using (var launchPage = new LaunchPage(browser))
            using (var preamblePage = new ClaimPreamble(browser))
            {
                launchPage.ClickNewAnonymousClaim();
                preamblePage.WaitForPage();
                preamblePage.ClickNewHomeAndContentsClaim();
            }

        }

        public static void LaunchPageBeginNewPersonalValuablesClaim(this Browser browser)
        {
            LaunchPage.OpenB2CLandingPageAndFeatureToggles(browser);
            using (var launchPage = new LaunchPage(browser))
            using (var preamblePage = new ClaimPreamble(browser))
            {
                launchPage.ClickNewAnonymousClaim();
                preamblePage.WaitForPage();
                preamblePage.ClickNewPersonalValuablesClaim();
            }

        }
    }
}
