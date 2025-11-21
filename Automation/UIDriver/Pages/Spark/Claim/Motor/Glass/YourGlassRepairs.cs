using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.Constants.ClaimsMotor;
using static Rac.TestAutomation.Common.Constants.VisualTest;

namespace UIDriver.Pages.Spark.Claim.Motor.Glass
{
    public class YourGlassRepairs : SparkBasePage
    {
        #region CONSTANTS
        private class Constants
        {
            public const string Header = "Your glass repairs";

            public class InfoTitle
            {
                public const string RepairRequired = "Time to get your glass fixed!";
                public const string RepairBooked = "Great to hear";
                public const string RepairCompleted = "Great to hear";
            }

            public class InfoMessage
            {
                public const string RepairRequired = "Our repairers provide services Australia-wide.";
                public const string RepairBooked = "All you need to do is give your repairer the claim number then submit your invoice to us.";
                public const string RepairCompleted = "All you need to do is submit your claim and then your repair invoice.";
            }
        }

        #endregion

        #region XPATHS

        private const string XP_HEADER = "id('header')";
        private const string XP_CLAIM_NUMBER = "id('claimNumberDisplay')";

        private const string XP_OPTION_NOT_FIXED = "//input[@value='glassNotRepaired']/parent::span";
        private const string XP_OPTION_REPAIRS_BOOKED = "//input[@value='glassRepairBooked']/parent::span";
        private const string XP_OPTION_YES_FIXED = "//input[@value='glassRepaired']/parent::span";

        private const string XP_OPTION_FRONT_WINDSCREEN = "//input[@value='FrontWindscreen']/parent::span";
        private const string XP_OPTION_OTHER_WINDOW_GLASS = "//input[@value='Other']/parent::span";

        private const string XP_INFO_TITLE = "id('notification-card-title')";
        private const string XP_INFO_MSG = "id('notification-card-content')";

        private const string XP_BTN_NEXT = "//button[text()='Next']";

        #endregion

        #region Settable properties and controls

        public string Header => GetInnerText(XP_HEADER);

        public string ClaimNumber
        {
            get
            {
                var claimNumber = new String(GetElement(XP_CLAIM_NUMBER).Text.
                    Where(x => Char.IsDigit(x)).ToArray());
                return claimNumber;
            }
        }

        public string InformationTitle => GetInnerText(XP_INFO_TITLE);

        public string InformationMessage => GetInnerText(XP_INFO_MSG);

        #endregion


        public YourGlassRepairs(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XP_HEADER);
                GetElement(XP_CLAIM_NUMBER);
                GetElement(XP_OPTION_NOT_FIXED);
                GetElement(XP_OPTION_REPAIRS_BOOKED);
                GetElement(XP_OPTION_YES_FIXED);
                GetElement(XP_BTN_NEXT);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Motor Glass Claim Page - Your Glass Claim");
            return true;
        }

        public void SelectGlassFixedOption(ClaimCar claim)
        {

            _browser.PercyScreenCheck(ClaimMotorGlass.YourGlassRepairs, GetPercyIgnoreCSS());
            ClickNext();
            _browser.PercyScreenCheck(ClaimMotorGlass.YourGlassRepairsErrorPage, GetPercyIgnoreCSS());

            Reporting.AreEqual(Constants.Header, Header, "Header on Your Glass Repair page");

            claim.ClaimNumber = ClaimNumber;

            switch (claim.ClaimScenario)
            {
                case MotorClaimScenario.GlassDamageNotFixed:                    
                    ClickControl(XP_OPTION_NOT_FIXED);
                    if (claim.GlassDamageDetails.FrontWindscreenOnly)
                    {
                        ClickControl(XP_OPTION_FRONT_WINDSCREEN);
                    }                        
                    if (claim.GlassDamageDetails.OtherWindowGlass)
                    {
                        ClickControl(XP_OPTION_OTHER_WINDOW_GLASS);
                    }
                    _browser.PercyScreenCheck(ClaimMotorGlass.YourGlassRepairs_NotFixed, GetPercyIgnoreCSS());
                    break;
                case MotorClaimScenario.GlassDamageRepairsBooked:
                    ClickControl(XP_OPTION_REPAIRS_BOOKED);
                    _browser.PercyScreenCheck(ClaimMotorGlass.YourGlassRepairs_RepairBooked, GetPercyIgnoreCSS());
                    break;
                case MotorClaimScenario.GlassDamageAlreadyFixed:
                    ClickControl(XP_OPTION_YES_FIXED);
                    _browser.PercyScreenCheck(ClaimMotorGlass.YourGlassRepairs_AlreadyFixed, GetPercyIgnoreCSS());
                    break;
                default:
                    throw new NotImplementedException($"{claim.ClaimScenario} is not supported");
            }
            VerifyNotificationText(claim.ClaimScenario);
            Reporting.Log("Motor Glass Claim - Your Glass Claim", _browser.Driver.TakeSnapshot());
            ClickNext();
        }

        private void ClickNext()
        {
            ClickControl(XP_BTN_NEXT);
        }

        private void VerifyNotificationText(MotorClaimScenario claimScenario)
        {
            switch(claimScenario)
            {
                case MotorClaimScenario.GlassDamageNotFixed:
                    Reporting.AreEqual(Constants.InfoTitle.RepairRequired, InformationTitle, "Notification title");
                    Reporting.AreEqual(Constants.InfoMessage.RepairRequired, InformationMessage, "Notification title");                    
                    break;
                case MotorClaimScenario.GlassDamageRepairsBooked:
                    Reporting.AreEqual(Constants.InfoTitle.RepairBooked, InformationTitle, "Notification title");
                    Reporting.AreEqual(Constants.InfoMessage.RepairBooked, InformationMessage, "Notification title");
                    break;
                case MotorClaimScenario.GlassDamageAlreadyFixed:
                    Reporting.AreEqual(Constants.InfoTitle.RepairCompleted, InformationTitle, "Notification title");
                    Reporting.AreEqual(Constants.InfoMessage.RepairCompleted, InformationMessage, "Notification title");
                    break;
                default:
                    throw new NotImplementedException($"{claimScenario} is not supported");
            }
            
        }

        private List<string> GetPercyIgnoreCSS() =>
          new List<string>()
          {
               "#claimNumberDisplay span"              
          };
    }
}
