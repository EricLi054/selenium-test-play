using Rac.TestAutomation.Common;
using System;
using System.Threading;
using OpenQA.Selenium;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyMotorcycle;

namespace UIDriver.Pages.Spark.MotorcycleQuote
{
    public class NowABitAboutYou : SparkBasePage
    {
        #region XPATHS
        private class XPath
        {
            public static class General
            {
                public const string Header = "//h2[@id='header' and text()='Now, a bit about you']";
            }
            public static class Button
            {
                public const string Next = "//button[@data-testid='riderDetailsSubmit']";
            }
            public static class AboutYou
            {
                public const string DOB = "id('dateOfBirth')";
                public static class YearsOfExperience
                {
                    public const string LessThanOne = "id('selectedRiderExperience')/button[@value='0-1']";
                    public const string OneToTwo = "id('selectedRiderExperience')/button[@value='1-2']";
                    public const string TwoToThree = "id('selectedRiderExperience')/button[@value='2-3']";
                    public const string ThreePlus = "id('selectedRiderExperience')/button[@value='3+']";
                }
            }
            public static class UseofMotorCycle
            {
                public const string Input = "id('select-bike-usage')";
                public const string DropdownOptions = "//ul[@role='listbox']/li";
            }
            public const string BirthdayAnimation = "id('birthdayConfettiContainer')";
        }
        #endregion

        #region Settable properties and controls
        public string DateOfBirth
        {
            get => GetInnerText(XPath.AboutYou.DOB);

            // Using return key to force blur event and birthday evaluation.
            set => WaitForTextFieldAndEnterText(XPath.AboutYou.DOB, value, true);

        }

        public string RiderExperience
        {
            get
            {
                return (IsChecked(XPath.AboutYou.YearsOfExperience.LessThanOne)) ? "0-1" :
                    (IsChecked(XPath.AboutYou.YearsOfExperience.OneToTwo)) ? "1-2" :
                    (IsChecked(XPath.AboutYou.YearsOfExperience.TwoToThree)) ? "2-3" :
                    (IsChecked(XPath.AboutYou.YearsOfExperience.ThreePlus)) ? "3+" : "None of them are selected";
            }
            set
            {
                switch (value)
                {
                    case "0-1":
                        ClickControl(XPath.AboutYou.YearsOfExperience.LessThanOne);
                        break;
                    case "1-2":
                        ClickControl(XPath.AboutYou.YearsOfExperience.OneToTwo);
                        break;
                    case "2-3":
                        ClickControl(XPath.AboutYou.YearsOfExperience.TwoToThree);
                        break;
                    case "3+":
                        ClickControl(XPath.AboutYou.YearsOfExperience.ThreePlus);
                        break;
                    default:
                        Reporting.Error("Requested option for motorcycle rider experience was not recognised.");
                        break;
                }
            }
        }

        public string BikeUsageType
        {
            get => GetInnerText(XPath.UseofMotorCycle.Input);
            set => WaitForSelectableAndPickFromDropdown(XPath.UseofMotorCycle.Input, XPath.UseofMotorCycle.DropdownOptions, value);
        }
        #endregion

        public NowABitAboutYou(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.Header);
                GetElement(XPath.UseofMotorCycle.Input);
                GetElement(XPath.Button.Next);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Motorcycle Quote page - Now A Bit About You");
            return true;
        }

        /// <summary>
        /// Wrapper around NowABitAboutYou..() methods for first
        /// and second accordions.
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <param name="contact"></param>        
        /// <returns></returns>
        public void FillAboutYou(QuoteMotorcycle quoteDetails)
        {
            var rider = quoteDetails.Drivers[0];

            // Date of Birth only needed if member was not an existing member
            // NOTE: This will not accomodate test scenarios of an existing
            //       member who did not match from MC.
            if (!rider.Details.IsRACMember || rider.Details.SkipDeclaringMembership)
            {
                DateOfBirth = rider.Details.DateOfBirth.ToString("ddMMyyyy");

                if (DateTime.Now.Day == rider.Details.DateOfBirth.Day &&
                    DateTime.Now.Month == rider.Details.DateOfBirth.Month)
                {
                    Reporting.IsTrue(WaitForBirthdayNotificationToAppearAndClear(), "'happy birthday' animation shown");
                }
            }

            RiderExperience = rider.LicenseTime;

            BikeUsageType = MotorcycleUsageMappings[quoteDetails.UsageType].TextB2C;
            Reporting.Log($"Capturing Screenshot after filling 'A Bit About You'", _browser.Driver.TakeSnapshot());
            ClickNext();
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }

        private bool WaitForBirthdayNotificationToAppearAndClear()
        {
            var birthdayNotificationSeen = _driver.WaitForElementToBeVisible(By.XPath(XPath.BirthdayAnimation), WaitTimes.T5SEC) != null;

            // Now wait for dialog to disappear.
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T10SEC);
            
            do
            {
                IWebElement birthdayPopUp = null;
                var isBirthdayDialogStillPresent = _driver.TryFindElement(By.XPath(XPath.BirthdayAnimation), out birthdayPopUp);

                if (isBirthdayDialogStillPresent)
                    Thread.Sleep(500);
                else
                    break;
            } while (DateTime.Now < endTime);

            return birthdayNotificationSeen;
        }
    }
}
