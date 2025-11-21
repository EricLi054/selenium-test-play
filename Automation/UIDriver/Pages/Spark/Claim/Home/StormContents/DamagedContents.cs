using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;


namespace UIDriver.Pages.Spark.Claim.Home
{
    public class DamagedContents : SparkBasePage
    {
        #region Constants
        public class Constants
        {
            public class Field
            {
                public class Label
                {
                    public static readonly string IsCarpetWaterDamaged = "Is there water damage to any carpet in the home?";
                    public static readonly string IsCarpetTooWet = "Are the carpets so badly soaked that you can't dry them?";
                }
                public class Button
                {
                    public static readonly string IsCarpetWaterDamaged = "Yes\r\nNo";
                    public static readonly string IsCarpetTooWet = "Yes\r\nNo\r\nI'm not sure";
                }
                public class Validation
                {
                    public static readonly string CarpetWaterDamaged    = "Please select Yes or No";
                    public static readonly string CarpetTooWet          = "Please select an answer";
                }
            }
        }
        #endregion
        #region XPaths
        private class XPath
        {
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";
            public static readonly string NextButton = "id('submit-button')";
            public class Field
            {
                public static readonly string IsCarpetWaterDamaged  = "id('waterDamageToCarpets')";
                public static readonly string IsCarpetTooWet        = "id('cantDryCarpets')";
                public class Button
                {
                    public static readonly string Yes = "//button[text()='Yes']";
                    public static readonly string No = "//button[text()='No']";
                    public static readonly string TooWetNotSure = "//button[contains(text(),'not sure')]";
                }
                public class Label
                {
                    public static readonly string CarpetWaterDamaged  = "id('label-waterDamageToCarpets')";
                    public static readonly string CarpetTooWet        = "id('label-cantDryCarpets')";
                }
                public class Validation
                {
                    public static readonly string CarpetWaterDamaged    = "id('helper-text-waterDamageToCarpets')";
                    public static readonly string CarpetTooWet          = "id('helper-text-cantDryCarpets')";
                }
            }
            public class Helptext
            {
                public static readonly string NotificationCardTitle = "id('notification-card-title')";
                public static readonly string NotificationCardBody  = "id('notification-card-paragraph-0')";
            }
        }
        #endregion
        #region Settable properties and controls
        private bool isCarpetWaterDamaged
        {
            get => GetBinaryToggleState(XPath.Field.IsCarpetWaterDamaged, XPath.Field.Button.Yes, XPath.Field.Button.No);
            set => ClickBinaryToggle(XPath.Field.IsCarpetWaterDamaged, XPath.Field.Button.Yes, XPath.Field.Button.No, value);
        }
        private bool? isCarpetTooWet
        {
            get => GetNullableBinaryForTriStateToggle(XPath.Field.IsCarpetTooWet, XPath.Field.Button.Yes, XPath.Field.Button.No, XPath.Field.Button.TooWetNotSure);
            set => ClickTriStateToggleWithNullableInput(XPath.Field.IsCarpetTooWet, XPath.Field.Button.Yes, XPath.Field.Button.No, XPath.Field.Button.TooWetNotSure, value);
        }
        #endregion

        public DamagedContents(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Field.IsCarpetWaterDamaged);
                GetElement(XPath.NextButton);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Spark Storm Claim - Damaged Contents");
            Reporting.Log($"Capturing screenshot of page as shown on arrival", _driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// This method sets the answers for the questions regarding damage to contents.
        /// 
        /// If detailedUiChecking flag is active then it will also confirm copy of field labels etc.
        /// 
        /// Note: As 'No' and 'I'm not sure' are both handled the same way by the app, we randomly determine whether 
        /// to answer 'No' or 'I'm not sure' when either of those answers are appropriate.
        /// </summary>
        /// <param name="detailedUiChecking">Flag to indicate if we should check the text on UI elements during this test</param>
        public void AnswerContentsDamageQuestions(Browser browsing, ClaimHome claim, bool detailedUiChecking = false)
        {
            if(detailedUiChecking)
            {
                Reporting.LogMinorSectionHeading("Detailed UI Checking for Contents stormwater damage questions");
                TriggerValidation();
                Reporting.AreEqual(Constants.Field.Label.IsCarpetWaterDamaged, 
                    GetInnerText(XPath.Field.Label.CarpetWaterDamaged), "any water damage field expected text against text on page");
                Reporting.AreEqual(Constants.Field.Button.IsCarpetWaterDamaged, 
                    GetInnerText(XPath.Field.IsCarpetWaterDamaged), "any water damage expected response options against text on page");
                Reporting.AreEqual(Constants.Field.Validation.CarpetWaterDamaged, 
                    GetInnerText(XPath.Field.Validation.CarpetWaterDamaged), "field validation error for this field");
            }

            isCarpetWaterDamaged = claim.ContentsDamage.IsWaterDamagedCarpets;

            if (claim.ContentsDamage.IsWaterDamagedCarpets)
            {
                if(detailedUiChecking)
                {
                    TriggerValidation();
                    Reporting.AreEqual(Constants.Field.Label.IsCarpetTooWet, 
                        GetInnerText(XPath.Field.Label.CarpetTooWet), "can't dry carpets field expected text against text on page");
                    Reporting.AreEqual(Constants.Field.Button.IsCarpetTooWet, 
                        GetInnerText(XPath.Field.IsCarpetTooWet), "can't dry carpet expected response options against text on page");
                    Reporting.AreEqual(Constants.Field.Validation.CarpetTooWet,
                        GetInnerText(XPath.Field.Validation.CarpetTooWet), "field validation error for this field");
                }

                bool selectNotSureIfNo = DataHelper.RandomBoolean();
                
                if (selectNotSureIfNo 
                 && !claim.ContentsDamage.IsCarpetTooWet)
                {
                    Reporting.Log($"'Is Carpet Too Wet' = '{claim.ContentsDamage.IsCarpetTooWet}' and 'I'm not sure' is treated the same as 'No', " +
                        $"and this test run was randomly selected to select 'I'm not sure' instead of 'No'.");
                    ClickControl(XPath.Field.Button.TooWetNotSure);
                }
                else
                {
                    isCarpetTooWet = claim.ContentsDamage.IsCarpetTooWet;
                }
            }
            

            Reporting.Log($"Capturing state of page after fill", _driver.TakeSnapshot());
            ClickNextButton();
        }

        private void TriggerValidation()
        {
            Reporting.Log($"Selecting Next to trigger field validaton error.");
            ClickControl(XPath.NextButton);
        }

        public void ClickNextButton()
        {
            ClickControl(XPath.NextButton);
        }
    }
}

