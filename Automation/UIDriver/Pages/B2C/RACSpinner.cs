using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.B2C
{
    public class RACSpinner : BasePage
    {
        private const string XP_PAGE_LOADING = "id('block-message-block')/div[@class='block-message']";

        public RACSpinner(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                var spinner = GetElement(XP_PAGE_LOADING);
                isDisplayed = spinner.Displayed || (spinner.Size.Height > 0);
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            catch (StaleElementReferenceException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Loading spinner may take a brief moment to render. So this method waits first
        /// for the spinner to be observed and then waits for it to disappear. There is a
        /// overarching timeout which will keep us from being stuck perpetually.
        /// 
        /// To also avoid the case of having missed the spinner, and being stuck for 90s,
        /// there is also a 5sec check on seeing the spinner start. If the spinner is not
        /// seen to start, then we will exit under the assumption the spinner has come
        /// and gone already.
        /// </summary>
        /// <param name="waitTimeSeconds"></param>
        /// <param name="nextPage"></param>
        public void WaitForSpinnerToFinish(int waitTimeSeconds = WaitTimes.T90SEC, BasePage nextPage = null)
        {
            var startTime = DateTime.Now; // Recorded to assist in exception message for recording time waited.
            var endTime   = DateTime.Now.AddSeconds(waitTimeSeconds);
            var spinnerStartTimeout = DateTime.Now.AddSeconds(WaitTimes.T5SEC);
            var spinnerSeen    = false;
            var spinnerCleared = false;
            var errorOccurred  = false;

            Reporting.Log("Begin waiting for spinner.");

            using (var errorPage = new ErrorB2C(browser: _browser))
            {
                do
                {
                    if (errorPage.IsDisplayed())
                    {
                        errorOccurred = true;
                        break;
                    }

                    if (!spinnerSeen)
                    {
                        spinnerSeen = IsDisplayed();

                        // If we've still not seen the spinner start, then
                        // we may have missed it altogether (operation completed
                        // before automation began checking).
                        if (!spinnerSeen &&
                            nextPage == null &&
                            DateTime.Now > spinnerStartTimeout)
                            break;
                    }
                    else
                    {
                        spinnerCleared = !IsDisplayed() && (nextPage == null || nextPage.IsDisplayed());
                    }

                    if (!spinnerSeen && nextPage != null)
                    {
                        spinnerCleared = nextPage.IsDisplayed();
                    }
                    Thread.Sleep(200);
                } while (DateTime.Now < endTime && !spinnerCleared);
            }

            var totalWaitedTime = DateTime.Now.Subtract(startTime);
            if (errorOccurred)
            { Reporting.Error($"B2C error screen encountered after {totalWaitedTime.Minutes}min{totalWaitedTime.Seconds}sec."); }

            if (!spinnerCleared && spinnerSeen)
            { Reporting.Error($"B2C timed out processing. {totalWaitedTime.Minutes}min{totalWaitedTime.Seconds}sec has elapsed but spinner hasn't finished yet."); }

            Reporting.Log("Completed waiting for spinner");

            // Sometimes the DOM isn't truly updated, or is still refreshing.
            Thread.Sleep(2000);
        }
    }
}