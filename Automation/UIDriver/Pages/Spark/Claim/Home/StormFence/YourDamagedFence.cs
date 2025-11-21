using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;
using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class YourDamagedFence : SparkBasePage
    {
        #region Constants
        public class Constants
        {
            public class Field
            {
                public class Label
                {
                    public static readonly string TypeOfFence   = "What type of fence was damaged?";
                    public static readonly string DamagedLength = "How many metres have been damaged?\r\nPlease round up to the nearest metre.";
                    public static readonly string PaintedLength = "How many metres of the damaged fence are painted on your side?\r\nPlease round up to the nearest metre.";
                    public static readonly string CannotMeasure = "I can't measure my fence.";
                    public static readonly string DamagedSides  = "If you're outside your property facing the front, which fence has been damaged?\r\nYou can choose more than one option.";
                }
                public class Helptext
                {
                    public static readonly string DamagedTitle    = "Metres damaged";
                    public static readonly string DamagedBody     = "We use this figure to estimate your fence repair cost. If you make a mistake, you can contact us later and correct it.";
                    public static readonly string PaintedTitle    = "Metres painted on your side";
                    public static readonly string PaintedBody     = "This figure helps us estimate your fence repair cost.";
                }
            }
        }
        #endregion
        #region XPATHS
        public class XPath
        { 
            public static readonly string ClaimNumber = "id('claimNumberDisplay')";
            public static readonly string NextButton = "id('submit-button')";
            public class Field
            { 
                public static readonly string DamagedFenceType = "id('fenceType')";
                public static readonly string DamagedFenceTypeOptions = "id('menu-fenceType')//li";

                public static readonly string DamagedFenceLength = "id('fenceLength-textbox')";
                public static readonly string TroubleMeasuringFence = "//span[@data-testid='fenceLength-checkbox']";
                public static readonly string PaintedFenceLength = "id('fencePaintedLength-textbox')";

                public static readonly string LeftFence     = "id('fenceDamagedSides-Left')/span";
                public static readonly string RightFence    = "id('fenceDamagedSides-Right')/span";
                public static readonly string FrontFence    = "id('fenceDamagedSides-Front')/span";
                public static readonly string RearFence     = "id('fenceDamagedSides-Rear')/span";
                public static readonly string PoolFence     = "id('fenceDamagedSides-Pool-fence')/span"; 
                public class Label
                    {
                        public static readonly string TypeOfFence   = "//label[contains(text(),'fence was damaged?')]";
                        public static readonly string DamagedLength = "//label[contains(text(),'have been damaged?')]";
                        public static readonly string PaintedLength = "//label[contains(text(),'painted on your side?')]";
                        public static readonly string CannotMeasure = "//span[contains(text(),'measure my fence.')]";
                        public static readonly string DamagedSides  = "id('fenceDamagedSides-label')";
                    }
                    public class Helptext
                    {
                        public static readonly string DamagedShow     = "id('tooltip-fenceLength-textbox-show-button')";
                        public static readonly string DamagedTitle    = "id('tooltip-fenceLength-textbox-title')";
                        public static readonly string DamagedBody     = "id('tooltip-fenceLength-textbox-message-text')";
                        public static readonly string DamagedClose    = "id('tooltip-fenceLength-textbox-close')";
                        public static readonly string PaintedShow     = "id('tooltip-fencePaintedLength-textbox-show-button')";
                        public static readonly string PaintedTitle    = "id('tooltip-fencePaintedLength-textbox-title')";
                        public static readonly string PaintedBody     = "id('tooltip-fencePaintedLength-textbox-message-text')";
                        public static readonly string PaintedClose    = "id('tooltip-fencePaintedLength-textbox-close')";
                    }
            }
        }



        #endregion

        #region Settable properties and controls

        public string DamagedFenceType
        {
            get => GetInnerText(XPath.Field.DamagedFenceType);

            set => WaitForSelectableAndPickFromDropdown(XPath.Field.DamagedFenceType, XPath.Field.DamagedFenceTypeOptions, value);
        }


        public string DamagedFenceLength
        {
            get => GetInnerText(XPath.Field.DamagedFenceLength);

            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    WaitForTextFieldAndEnterText(XPath.Field.DamagedFenceLength, value, false);
                }
                else 
                {
                    ClickControl(XPath.Field.TroubleMeasuringFence);
                }
            }
        }

        public string DamagedPaintedFenceLength
        {
            get => GetInnerText(XPath.Field.PaintedFenceLength);

            set => WaitForTextFieldAndEnterText(XPath.Field.PaintedFenceLength, value, false);
        }

        #endregion

        public YourDamagedFence(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Field.DamagedFenceType);
                GetElement(XPath.NextButton);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Spark Fence Claim Page - Your Damaged Fence");
            Reporting.Log($"Capturing screenshot of page as shown on arrival", _driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// This method sets the values for this page to match the test data set up for the scenario.
        /// 
        /// NOTE: Length of damaged/painted fence fields remain hidden the following Fence Types: 
        /// BrickWall, Wooden, Glass, Other, MoreThanOneFence & NotSure
        /// 
        /// If detailUiChecking = true we also verify expected text for field labels, helptext etc.
        /// encountered during this flow.
        /// </summary>
        /// <param name="detailUiChecking">Optional parameter, if set to true will investigate field labels, validation errors etc</param>
        public void FillFenceDamageDetails(ClaimHome claim, bool detailUiChecking = false)
        {
            if (detailUiChecking)
            {
                Reporting.LogMinorSectionHeading("Detailed UI Checking for Fence Damage Details");

                Reporting.AreEqual(Constants.Field.Label.TypeOfFence, GetInnerText(XPath.Field.Label.TypeOfFence), 
                    "expected label text for Type of fence damaged with actual value displayed");
                
                Reporting.AreEqual(Constants.Field.Label.DamagedLength, GetInnerText(XPath.Field.Label.DamagedLength), 
                    "expected label text for Damaged Length with actual value displayed");

                Reporting.Log($"Setting damaged fence type to SuperSix to enable validation of the painted fence field.");
                DamagedFenceType = FenceType.Hardifence.ToString();
                Reporting.AreEqual(Constants.Field.Label.PaintedLength, GetInnerText(XPath.Field.Label.PaintedLength), 
                    "expected label text for Painted Length with actual value displayed");

                Reporting.Log($"Selecting Helptext button for Damaged Length");
                ClickControl(XPath.Field.Helptext.DamagedShow);
                Reporting.AreEqual(Constants.Field.Helptext.DamagedTitle, GetInnerText(XPath.Field.Helptext.DamagedTitle), 
                    "expected title copy for Damaged Length help text with actual value displayed");
                Reporting.AreEqual(Constants.Field.Helptext.DamagedBody, GetInnerText(XPath.Field.Helptext.DamagedBody), 
                    "expected body copy for Damaged Length help text with actual value displayed");
                Reporting.Log("Capturing Helptext for Damaged Length before closing.", _driver.TakeSnapshot());
                ClickControl(XPath.Field.Helptext.DamagedClose);

                Reporting.Log($"Selecting Helptext button for Painted Length");
                ClickControl(XPath.Field.Helptext.PaintedShow);
                Reporting.AreEqual(Constants.Field.Helptext.PaintedTitle, GetInnerText(XPath.Field.Helptext.PaintedTitle), 
                    "expected title copy for Painted Length help text with actual value displayed");
                Reporting.AreEqual(Constants.Field.Helptext.PaintedBody, GetInnerText(XPath.Field.Helptext.PaintedBody), 
                    "expected body copy for Painted Length help text with actual value displayed");
                Reporting.Log("Capturing Helptext for Painted Length before closing.", _driver.TakeSnapshot());
                ClickControl(XPath.Field.Helptext.PaintedClose);

                Reporting.AreEqual(Constants.Field.Label.CannotMeasure, GetInnerText(XPath.Field.Label.CannotMeasure), 
                    "expected label text for the 'I can't measure' checkbox with the actual value displayed");
                Reporting.AreEqual(Constants.Field.Label.DamagedSides, GetInnerText(XPath.Field.Label.DamagedSides),
                    "expected label text for the 'Which fence has been damaged' options with the actual value displayed");
            }
            
            Reporting.LogMinorSectionHeading("Inputting Fence Damage Details from test scenario data");
            DamagedFenceType = FenceTypeNames[claim.FenceDamage.FenceMaterial].TextSpark;

            if (claim.FenceDamage.FenceMaterial != FenceType.BrickWall
             && claim.FenceDamage.FenceMaterial != FenceType.Wooden
             && claim.FenceDamage.FenceMaterial != FenceType.Glass
             && claim.FenceDamage.FenceMaterial != FenceType.Other
             && claim.FenceDamage.FenceMaterial != FenceType.MoreThanOneFence
             && claim.FenceDamage.FenceMaterial != FenceType.NotSure)
            {
                _driver.WaitForElementToBeVisible(By.XPath(XPath.Field.DamagedFenceType), WaitTimes.T5SEC);
                DamagedFenceLength = claim.FenceDamage.MetresPanelsDamaged.ToString();
                Reporting.Log("Capturing completed fields so far.", _driver.TakeSnapshot());
                if (claim.FenceDamage.MetresPanelsDamaged != null &&
                   (claim.FenceDamage.FenceMaterial == FenceType.Hardifence ||
                    claim.FenceDamage.FenceMaterial == FenceType.SuperSix ||
                    claim.FenceDamage.FenceMaterial == FenceType.Asbestos))
                {
                    DamagedPaintedFenceLength = claim.FenceDamage.MetresPanelsPainted.ToString();
                }
            }
            else
            {
                Reporting.Log($"Fence material = '{claim.FenceDamage.FenceMaterial}' so capturing snapshot as Damaged/Painted length inputs are hidden.", _driver.TakeSnapshot());

                Reporting.IsFalse(IsControlDisplayed(XPath.Field.DamagedFenceLength),
                    $"Confirmed '{Constants.Field.Label.DamagedLength}' is not visible");
                Reporting.IsFalse(IsControlDisplayed(XPath.Field.PaintedFenceLength), 
                    $"Confirmed '{Constants.Field.Label.PaintedLength}' is not visible");
            }

            SetCheckBox(XPath.Field.LeftFence,  claim.FenceDamage.AffectedBoundaryLeft);
            SetCheckBox(XPath.Field.RightFence, claim.FenceDamage.AffectedBoundaryRight);
            SetCheckBox(XPath.Field.FrontFence, claim.FenceDamage.AffectedBoundaryFront);
            SetCheckBox(XPath.Field.RearFence,  claim.FenceDamage.AffectedBoundaryRear);
            if (claim.FenceDamage.FenceMaterial == FenceType.Glass)
            {
                Reporting.Log($"Fence material = {claim.FenceDamage.FenceMaterial} so 'Pool fence' checkbox is available for selection.");
                SetCheckBox(XPath.Field.PoolFence, claim.FenceDamage.AffectedPoolFence);
            }
            else
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.Field.PoolFence), $"Fence material = {claim.FenceDamage.FenceMaterial} so Pool Fence should not be available for selection.");
            }

            Reporting.Log("Capturing 'Your damaged fence' page before attempting to progress to next page", _driver.TakeSnapshot());
            ClickContinueButton();
        }

        private void SetCheckBox(string xpath, bool setAsChecked)
        {
            var checkboxTick = GetElement(xpath);
            if (checkboxTick.Selected != setAsChecked)
            {
                ClickControl(xpath);
            }
        }
        public void ClickContinueButton()
        {
            ClickControl(XPath.NextButton);
        }
    }
}
