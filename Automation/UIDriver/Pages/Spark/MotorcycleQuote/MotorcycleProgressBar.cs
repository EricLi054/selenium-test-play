using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace UIDriver.Pages.Spark.MotorcycleQuote
{
    public class MotorcycleProgressBar : SparkBasePage
    {
        // PAGE enums must be maintained in the order the pages are encountered.
        public enum PAGE_QUOTE  { MemberDetails, YourBike, BikeUsage, AboutYou, YourQuote };
        public enum PAGE_POLICY { MemberDetails, YourBike, BikeUsage, AboutYou, YourQuote, ConfirmDetails, PersonalInformation, Payment, Confirmation };

        private static Dictionary<PAGE_QUOTE, string> progressBarQuoteText = new Dictionary<PAGE_QUOTE, string>()
        {
            { PAGE_QUOTE.MemberDetails, "Member details" },
            { PAGE_QUOTE.YourBike,      "Your bike" },
            { PAGE_QUOTE.BikeUsage,     "Bike usage" },
            { PAGE_QUOTE.AboutYou,      "About you" },
            { PAGE_QUOTE.YourQuote,     "Your quote" }
        };

        private static Dictionary<PAGE_POLICY, string> progressBarPolicyText = new Dictionary<PAGE_POLICY, string>()
        {
            { PAGE_POLICY.MemberDetails,        "Member details" },
            { PAGE_POLICY.YourBike,             "Your bike" },
            { PAGE_POLICY.BikeUsage,            "Bike usage" },
            { PAGE_POLICY.AboutYou,             "About you" },
            { PAGE_POLICY.YourQuote,            "Your quote" },
            { PAGE_POLICY.ConfirmDetails,       "Confirm policy details" },
            { PAGE_POLICY.PersonalInformation,  "Personal information" },
            { PAGE_POLICY.Payment,              "Payment" },
            { PAGE_POLICY.Confirmation,         "Confirmation" }
        };

        // Approx pixels before mobile view is emulated in browser.
        private const int MIN_WIDTH_BEFORE_MOBILE_VIEW = 770;

        #region XPATHS
        private static class XPath
        {
            public static class General
            {
                public const string Header = "//h2[text()='Motorcycle insurance']";
                public const string PDS = "//a[@id='pdsLink']";
            }
            public static class Progress
            {
                public const string Container = "//div[contains(@class,'MuiStepper-vertical')]";
                public const string Step = Container + "//div[contains(@class,'MuiStep-root')]";
                public const string StepLabel = "//span[contains(@class,'MuiStepLabel-labelContainer')]/span";
                public const string Mobile = "//div[@id='root']//header/div[2]/div[2]//span";
            }
        }
        #endregion
        public MotorcycleProgressBar(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.Header);
                GetElement(XPath.General.PDS);
                _driver.FindElements(By.XPath(XPath.Progress.Step));
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns an enumerator indicating which steo is currently marked
        /// as the active step while still getting quote.
        /// </summary>
        public PAGE_QUOTE CurrentStepDuringQuoteStages()
        {
            if (IsInResponsiveMode())
            {
                return GetCurrentProgressStepForQuoteInMobileMode();
            }

            return GetCurrentProgressStepForQuoteInDesktopMode();
        }

        /// <summary>
        /// Returns an enumerator indicating which steo is currently marked
        /// as the active step while proceeding to purchase policy.
        /// </summary>
        public PAGE_POLICY CurrentStepDuringPolicyStages()
        {
            if (IsInResponsiveMode())
            {
                return GetCurrentProgressStepForPolicyInMobileMode();
            }

            return GetCurrentProgressStepForPolicyInDesktopMode();
        }

        /// <summary>
        /// Attempt to determine if browser will be displaying the progress
        /// sode bar or not, based on width of the DOM. Appears that any
        /// width under ~770 pixels results in emulating a phone screen in
        /// portrait view, and progress side bar becomes hidden.
        /// </summary>
        /// <returns></returns>
        private bool IsInResponsiveMode()
        {
            var currentWidth = GetElement("//body").Size.Width;

            return currentWidth < MIN_WIDTH_BEFORE_MOBILE_VIEW;
        }

        private PAGE_QUOTE GetCurrentProgressStepForQuoteInMobileMode()
        {
            return (PAGE_QUOTE)GetPageNumberInMobileMode();
        }

        private PAGE_QUOTE GetCurrentProgressStepForQuoteInDesktopMode()
        {
            var currentStep = PAGE_QUOTE.MemberDetails;

            var progressStepsVisible = _driver.FindElements(By.XPath(XPath.Progress.Step));

            if (progressStepsVisible.Count != progressBarQuoteText.Count)
            { Reporting.Error($"Unexpected number of progress bar steps returned. Received {progressStepsVisible.Count}, but expected {progressBarQuoteText.Count}."); }

            var countOfActiveSteps = 0;
            try
            {
                for (int i = 1; i <= progressStepsVisible.Count; i++)
                {
                    var elementStepLabel = GetElement($"{XPath.Progress.Step}[{i}]{XPath.Progress.StepLabel}");

                    var thisStepText = elementStepLabel.Text;

                    var stepKey = progressBarQuoteText.First(x => x.Value == thisStepText).Key;

                    if (elementStepLabel.GetAttribute("class").Contains("Mui-active"))
                    {
                        countOfActiveSteps++;
                        currentStep = stepKey;
                    }
                }
            }
            catch (NoSuchElementException)
            {
                Reporting.Error("Error occurred trying to verify the state of the current progress bar step.");
            }

            if (countOfActiveSteps != 1)
            {
                Reporting.Error($"Expected to have only one active step, but found: {countOfActiveSteps}");
            }

            return currentStep;
        }

        private PAGE_POLICY GetCurrentProgressStepForPolicyInMobileMode()
        {
            return (PAGE_POLICY)GetPageNumberInMobileMode();
        }

        private PAGE_POLICY GetCurrentProgressStepForPolicyInDesktopMode()
        {
            var currentStep = PAGE_POLICY.ConfirmDetails;

            var progressStepsVisible = _driver.FindElements(By.XPath(XPath.Progress.Step));

            if (progressStepsVisible.Count != progressBarPolicyText.Count)
            { Reporting.Error($"Unexpected number of progress bar steps returned. Received {progressStepsVisible.Count}, but expected {progressBarPolicyText.Count}."); }

            var countOfActiveSteps = 0;
            try
            {
                for (int i = 1; i <= progressStepsVisible.Count; i++)
                {
                    var elementStepLabel = GetElement($"{XPath.Progress.Step}[{i}]{XPath.Progress.StepLabel}");

                    var thisStepText = elementStepLabel.Text;

                    var stepKey = progressBarPolicyText.First(x => x.Value == thisStepText).Key;

                    if (elementStepLabel.GetAttribute("class").Contains("Mui-active"))
                    {
                        countOfActiveSteps++;
                        currentStep = stepKey;
                    }
                }
            }
            catch
            {
                Reporting.Error("Error occurred trying to verify the state of the current progress bar step.");
            }

            if (countOfActiveSteps != 1)
            {
                Reporting.Error($"Expected to have only one active step, but found: {countOfActiveSteps}");
            }

            return currentStep;
        }

        private int GetPageNumberInMobileMode()
        {
            var progressText = GetElement(XPath.Progress.Mobile).Text;
            var stepIndex = 0;
            var stepMax = 0;

            var regex = new Regex(@"^Step (\d) of (\d)$");
            Match match = regex.Match(progressText);

            // First group is original text, second group is expected match.
            if (match.Success && match.Groups.Count == 3)
            {
                stepIndex = int.Parse(match.Groups[1].Value);
                stepMax = int.Parse(match.Groups[2].Value);
            }
            else
            { throw new NotImplementedException($"Header did not contain expected text format. Saw: {progressText}"); }

            if (stepIndex > stepMax)
            { Reporting.Error($"Observed step index {stepIndex} exceeded expected maximum of {stepMax}"); }

            // Convert to 0-based int.
            return (stepIndex - 1);
        }
    }
}
