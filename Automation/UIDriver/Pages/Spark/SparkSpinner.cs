using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Spark
{
    public class SparkSpinner : SparkBasePage
    {
        public static class XPath
        {
            public static readonly string SparkPageLoading = "//*[@data-icon='spinner']";
        }

        public SparkSpinner(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                var spinner = GetElement(XPath.SparkPageLoading);
                isDisplayed = spinner.Displayed && (spinner.Size.Height > 0);
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Loading spinner may take a brief moment to render. So this method waits first
        /// for the spinner to be observed and then waits for it to disappear. There is a
        /// overarching timeout which will keep us from being stuck perpetually.
        /// To also avoid the case of having missed the spinner, and being stuck for 90s,
        /// there is also a 5sec check on seeing the spinner start. If the spinner is not
        /// seen to start, then we will exit under the assumption the spinner has come
        /// and gone already.
        /// </summary>
        /// <param name="waitTimeSeconds"></param>
        /// <param name="nextPage"></param>
        public void WaitForSpinnerToFinish(int waitTimeSeconds = WaitTimes.T150SEC, BasePage nextPage = null, bool waitAfterSpinnerFinished = true)
        {
            var endTime = DateTime.Now.AddSeconds(waitTimeSeconds);
            var spinnerStartTimeout = DateTime.Now.AddSeconds(WaitTimes.T5SEC);
            var spinnerSeen = false;
            var spinnerCleared = false;

            do
            {
                if (!spinnerSeen)
                {
                    spinnerSeen = IsDisplayed();
                    // If we've still not seen the spinner start, then
                    // we may have missed it altogether (operation completed before automation began checking).
                    if (!spinnerSeen && nextPage == null &&
                        DateTime.Now > spinnerStartTimeout)
                    { break; }
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

            if (!spinnerCleared && spinnerSeen)
            { Reporting.Error($"{DateTime.Now.ToString(DataFormats.TIME_FORMAT_24HR_WITH_SECONDS)} Spark spinner timed-out waiting for page. Waited {waitTimeSeconds}s"); }

            // Sometimes the DOM isn't truly updated, or is still refreshing.
            if (waitAfterSpinnerFinished)
            {
                Thread.Sleep(2000);
            }
        }
    }
}
