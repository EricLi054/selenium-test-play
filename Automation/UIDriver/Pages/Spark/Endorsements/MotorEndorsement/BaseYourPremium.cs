using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.PolicyGeneral;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace UIDriver.Pages.Spark.Endorsements
{
    abstract public class BaseYourPremium : SparkBasePage
    {
        #region CONSTANTS
        private class Constants
        {
            public class ToolTip
            {
                public class Title
                {
                    public const string Excess = "Excess";
                    public const string AgreedValue = "Agreed value";
                    public const string HireCar = "Hire car after accident";
                    public const string NCB = "No claim bonus protection";
                }

                public class Content
                {
                    public class Excess
                    {
                        public class Paragraph
                        {
                            public const string One = "The excess is the amount you will need to pay towards settlement of any claim.";
                            public const string Two = "If you adjust your excess, your premium will change.";
                            public const string Three = "The excess for windscreen and window glass replacement is $100.";
                            // TODO: INSU-283 to remove ThreeForProductVersion45AndPrior when story is actioned, and re-name ThreeForProductVersion46Onwards as all the policies will be on version 46
                            public const string ThreeForProductVersion45AndPrior = "The excess for windscreen and window glass replacement is $100.";
                            public const string ThreeForProductVersion46Onwards = "The excess for windscreen and window glass replacement is $150.";
                            public const string Four = "See the Premium, Excess and Discount Guide for more information.";
                        }

                        public class ExcessOption
                        {
                            public const string Heading = "Extra excesses may apply:";
                            // TODO: SPK-6704 to remove OneForProductVersion45AndPrior when the story is actioned, and re-name OneForProductVersion46Onwards as all the policies will be on version 46
                            public const string OneForProductVersion45AndPrior = "Inexperienced driver: $300";
                            public const string OneForProductVersion46Onwards = "Undisclosed driver age excess: $1000";
                            public const string Two = "Driver under 19: $650";
                            public const string Three = "Driver under 21: $550";
                            public const string Four = "Driver under 24: $450";
                            public const string Five = "Driver under 26: $300";
                            public const string Six = "Special excess: will be stated in your policy documents if applicable";
                        }
                    }               
                    public const string AgreedValue = "The agreed value is the amount we agree to insure your car for. The agreed value includes GST, registration, on-road costs and accessories fitted to your car.";
                    public const string HireCar = "Your policy includes a hire car after fire, theft and attempted theft. You can also pay extra to include a hire car after accident.";
                    // TODO: SPK-6704 to remove the NCB constant when the story is actioned 
                    public const string NCB = "You can pay extra so you can make one at-fault claim per period of insurance, without this affecting your no claim bonus.";
                }
            }
        }
        #endregion

        #region XPATHS
        private class XPath
        {
            public class Button
            {
                public const string Yes = "//button[@aria-label='Yes']";
                public const string No = "//button[@aria-label='No']";
            }

            public class AdjustHowYouPaySection
            {
                public const string Excess = "id('excess-dropdown')";
                public const string ExcessOptions = "//ul[@role='listbox']//li";
                public const string AgreedValue = "id('agreed-value-input')";
                public const string HireCar = "id('hire-car-after-accident-toggle-button')";
                public const string NCB = "id('ncb-protection-toggle-button')";
            }

            public class ToolTip
            {
                public class AgreedValue
                {
                    public class Button
                    {
                        public const string Show  = "id('agreed-value-tooltipButton')";
                        public const string Close = "id('agreed-value-tooltip-close')";
                    }
                    public const string Title   = "id('agreed-value-tooltip-title')";
                    public const string Content = "id('agreed-value-tooltip-message-text')";
                }
                public class Excess
                {
                    public const string Title = "id('excess-dialog-title')";

                    public class Button
                    {
                        public const string Show  = "id('excess-tooltip-idButton')";
                        public const string Close = "//button[@aria-label='close']";
                    }

                    public class Paragraph
                    {
                        public const string One = "id('excess-dialog-paragraph-one')";
                        public const string Two = "id('excess-dialog-paragraph-two')";
                        public const string Three = "id('excess-dialog-paragraph-three')";
                        public const string Four = "id('excess-dialog-paragraph-four')";
                    }

                    public class ExcessOption
                    {
                        public const string Heading = "id('excess-dialog-extra-excesses')";
                        public const string One = "id('excess-dialog-extra-excesses-one')";
                        public const string Two = "id('excess-dialog-extra-excesses-two')";
                        public const string Three = "id('excess-dialog-extra-excesses-three')";
                        public const string Four = "id('excess-dialog-extra-excesses-four')";
                        public const string Five = "id('excess-dialog-extra-excesses-five')";
                        public const string Six = "id('excess-dialog-extra-excesses-six')";
                    }                  
                }
                public class HireCar
                {
                    public class Button
                    {
                        public const string Show  = "id('hire-car-after-accident-toggle-button-tooltipButton')";
                        public const string Close = "id('hire-car-after-accident-toggle-button-tooltip-close')";
                    }
                    public const string Title   = "id('hire-car-after-accident-toggle-button-tooltip-title')";
                    public const string Content = "id('hire-car-after-accident-toggle-button-tooltip-message-text')";
                }
                public class NCB
                {
                    public class Button
                    {
                        public const string Show  = "id('ncb-protection-toggle-button-tooltipButton')";
                        public const string Close = "id('ncb-protection-toggle-button-tooltip-close')";
                    }
                    public const string Title   = "id('ncb-protection-toggle-button-tooltip-title')";
                    public const string Content = "id('ncb-protection-toggle-button-tooltip-message-text')";
                    public const string NCBToggleState = "//*[@data-testid='ncb-protection-toggle-button']";
                }
            }
        }
        #endregion

        #region Settable properties and controls
        private bool HasHireCarAfterAccident
        {
            get => GetBinaryToggleState(XPath.AdjustHowYouPaySection.HireCar, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.AdjustHowYouPaySection.HireCar, XPath.Button.Yes, XPath.Button.No, value);
        }

        private bool NoClaimBonus
        {
            get => GetBinaryToggleState(XPath.AdjustHowYouPaySection.NCB, XPath.Button.Yes, XPath.Button.No);
            set => ClickBinaryToggle(XPath.AdjustHowYouPaySection.NCB, XPath.Button.Yes, XPath.Button.No, value);
        }

        public string Excess
        {
            get => DataHelper.StripMoneyNotations(GetInnerText(XPath.AdjustHowYouPaySection.Excess));
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    WaitForSelectableAndPickFromDropdown(XPath.AdjustHowYouPaySection.Excess, XPath.AdjustHowYouPaySection.ExcessOptions, value);

                    using (var spinner = new SparkSpinner(_browser))
                    {
                        spinner.WaitForSpinnerToFinish(WaitTimes.T150SEC);
                    }
                }
            }
        }

        private int AgreedValue
        {
            get => DataHelper.ConvertMonetaryStringToInt(GetValue(XPath.AdjustHowYouPaySection.AgreedValue));
            set => WaitForTextFieldAndEnterText(XPath.AdjustHowYouPaySection.AgreedValue, value.ToString("0"), false);
        }
        #endregion

        protected BaseYourPremium(Browser browser) : base(browser) { }

        public void VerifyAdjustYourAmountSection(EndorseCar endorseCar)
        {
            Reporting.AreEqual(endorseCar.OriginalPolicyData.Excess().FirstOrDefault().Value.ToString(), Excess, "Verifying Excess");
            switch (endorseCar.CoverType)
            {
                case MotorCovers.MFCO:
                    if (!endorseCar.ChangeMakeAndModel)
                    {
                        VerifyAgreedValue(endorseCar.OriginalPolicyData.MotorAsset.TotalInsuredValue);
                    }                        
                    VerifyHireCar(endorseCar.OriginalPolicyData.HasHireCarCover);
                    VerifyNCB(endorseCar.OriginalPolicyData.MotorAsset.HasNcbProtection, endorseCar.OriginalPolicyData.MotorAsset.NcbLevel, endorseCar.OriginalPolicyData.MotorAsset.IsMaxNcbLevel);
                    break;
                case MotorCovers.TFT:
                    if (!endorseCar.ChangeMakeAndModel)
                    {
                        VerifyAgreedValue(endorseCar.OriginalPolicyData.MotorAsset.TotalInsuredValue);
                    }                        
                    break;
                default:
                    //Nothing to do
                    break;
            }
        }

        /// <summary>
        /// The agreed value from shield is decimal but we only as per SPK-4141
        /// Agreed Value will have the cents truncated
        /// </summary>
        private void VerifyAgreedValue(Decimal expectedAgreedValue)
        {
            var expected = (int)Math.Floor(expectedAgreedValue);
            Reporting.AreEqual(expected, AgreedValue, $"Verifying Agreed Value: Expected : ${expected} Actual: ${AgreedValue}");
        }

        private void VerifyHireCar(bool expectedHireCar)
        {
            Reporting.AreEqual(expectedHireCar, HasHireCarAfterAccident, $"Verifying Hire Car: Expected : {expectedHireCar} Actual: {HasHireCarAfterAccident}");
        }

        /// <summary>
        /// Open, verify and close Excess tool tip
        /// </summary>
        public void VerifyExcessToolTip(EndorseCar endorseCar)
        {
            ClickControl(XPath.ToolTip.Excess.Button.Show);
            Reporting.AreEqual(Constants.ToolTip.Title.Excess, GetInnerText(XPath.ToolTip.Excess.Title), $"expected title of Excess tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.Excess.Paragraph.One, GetInnerText(XPath.ToolTip.Excess.Paragraph.One), $"expected content of Excess tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.Excess.Paragraph.Two, GetInnerText(XPath.ToolTip.Excess.Paragraph.Two), $"expected content of Excess tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.Excess.ExcessOption.Heading, GetInnerText(XPath.ToolTip.Excess.ExcessOption.Heading), $"expected content of Excess tool tip against actual");
            
            string expectedExcessOptionOne = endorseCar.IsMotorPolicyWithExcessChanges() ? Constants.ToolTip.Content.Excess.ExcessOption.OneForProductVersion46Onwards : Constants.ToolTip.Content.Excess.ExcessOption.OneForProductVersion45AndPrior;
            Reporting.AreEqual(expectedExcessOptionOne, GetInnerText(XPath.ToolTip.Excess.ExcessOption.One), $"expected content of Excess tool tip against actual");
          
            Reporting.AreEqual(Constants.ToolTip.Content.Excess.ExcessOption.Two, GetInnerText(XPath.ToolTip.Excess.ExcessOption.Two), $"expected content of Excess tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.Excess.ExcessOption.Three, GetInnerText(XPath.ToolTip.Excess.ExcessOption.Three), $"expected content of Excess tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.Excess.ExcessOption.Four, GetInnerText(XPath.ToolTip.Excess.ExcessOption.Four), $"expected content of Excess tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.Excess.ExcessOption.Five, GetInnerText(XPath.ToolTip.Excess.ExcessOption.Five), $"expected content of Excess tool tip against actual");
            Reporting.AreEqual(Constants.ToolTip.Content.Excess.ExcessOption.Six, GetInnerText(XPath.ToolTip.Excess.ExcessOption.Six), $"expected content of Excess tool tip against actual");
            
            VerifyWindscreenExcessToolTip(endorseCar);

            Reporting.AreEqual(Constants.ToolTip.Content.Excess.Paragraph.Four, GetInnerText(XPath.ToolTip.Excess.Paragraph.Four), $"expected content of Excess tool tip against actual");

            ClickControl(XPath.ToolTip.Excess.Button.Close);
        }

        // TODO: SPK-6704 remove conditions around "IsSparkExcessChangesEnabled" and "IsMotorPolicyWithExcessChanges()" when SPK-6704 is actioned as all policies will be on version 46
        /// <summary>
        /// Verifies the Windscreen Excess tool tip text when the Excess and NCB changes are in toggled ON/OFF state
        /// </summary>
        public void VerifyWindscreenExcessToolTip(EndorseCar endorseCar)
        {
            if (endorseCar.CoverType == MotorCovers.MFCO)
            {
                Reporting.Log($"Checking Windscreen excess for Comprehensive policy when Spark Excess and NCB Changes are enabled");
                string expectedWindscreenExcess = null;
                if (endorseCar.IsMotorPolicyWithExcessChanges())
                {
                    expectedWindscreenExcess = Constants.ToolTip.Content.Excess.Paragraph.ThreeForProductVersion46Onwards;
                }
                else
                {
                    expectedWindscreenExcess = Constants.ToolTip.Content.Excess.Paragraph.ThreeForProductVersion45AndPrior;
                }
                Reporting.AreEqual(expectedWindscreenExcess, GetInnerText(XPath.ToolTip.Excess.Paragraph.Three), $"expected content of Excess tool tip against actual");
            }
            else
            {
                Reporting.Log($"Checking NCB Protection fields when Cover type is TPO or TFT.");
                Reporting.IsFalse(IsControlDisplayed(XPath.ToolTip.Excess.Paragraph.Three), "'Windscreen tool tip' is not displayed.");
            }
        }

        /// <summary>
        /// Open, verify and close Agreed value tool tip
        /// only applicable to MFCO and TFT cover type
        /// </summary>
        public void VerifyAgreedValueToolTip(EndorseCar endorseCar)
        {
            if(endorseCar.CoverType == MotorCovers.MFCO || endorseCar.CoverType == MotorCovers.TFT)
            {
                ClickControl(XPath.ToolTip.AgreedValue.Button.Show);
                Reporting.AreEqual(Constants.ToolTip.Title.AgreedValue, GetInnerText(XPath.ToolTip.AgreedValue.Title), $"expected title of Agreed value tool tip against actual");
                Reporting.AreEqual(Constants.ToolTip.Content.AgreedValue, GetInnerText(XPath.ToolTip.AgreedValue.Content), $"expected content of a agreed value tool tip against actual");
                ClickControl(XPath.ToolTip.AgreedValue.Button.Close);
            }
            else
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.ToolTip.AgreedValue.Button.Show), "'Agreed value tool tip' is not displayed for TPO Cover type");
            }
        }

        /// <summary>
        /// Open, verify and close Hire car after accident tool tip
        /// only applicable to MFCO cover type
        /// </summary>
        public void VerifyHireCarAfterAccidentToolTip(EndorseCar endorseCar)
        {
            if(endorseCar.CoverType == MotorCovers.MFCO)
            {
                ClickControl(XPath.ToolTip.HireCar.Button.Show);
                Reporting.AreEqual(Constants.ToolTip.Title.HireCar, GetInnerText(XPath.ToolTip.HireCar.Title), $"expected title of Hire car after accident tool tip against actual");
                Reporting.AreEqual(Constants.ToolTip.Content.HireCar, GetInnerText(XPath.ToolTip.HireCar.Content), $"expected content of a Hire car after accident tool tip against actual");
                ClickControl(XPath.ToolTip.HireCar.Button.Close);
            }
            else
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.ToolTip.HireCar.Button.Show), "'Hire car after accident tool tip' is not displayed for TFT and TPO Cover type");
            }             
        }

        // TODO: SPK-6704 Remove the isNcbRelevantToThisPolicy condition from this method when this story is actioned as all the comprehensive policies will be on version 46
        /// <summary>
        /// Open, verify and close No claim bonus protection tool tip
        /// </summary>
        public void VerifyNCBToolTip(bool isNcbRelevantToThisPolicy)
        {
            if (!isNcbRelevantToThisPolicy)
            {
                ClickControl(XPath.ToolTip.NCB.Button.Show);
                Reporting.AreEqual(Constants.ToolTip.Title.NCB, GetInnerText(XPath.ToolTip.NCB.Title), $"expected title of No claim bonus protection tool tip against actual");
                Reporting.AreEqual(Constants.ToolTip.Content.NCB, GetInnerText(XPath.ToolTip.NCB.Content), $"expected content of No claim bonus protection tool tip against actual");
                ClickControl(XPath.ToolTip.NCB.Button.Close);
            }
            else
            {
                Reporting.Log($"Checking NCB Protection fields when Spark Excess and NCB Changes are enabled.");
                Reporting.IsFalse(IsControlDisplayed(XPath.ToolTip.NCB.Title), "No claim bonus protection label is not displayed.");
                Reporting.IsFalse(IsControlDisplayed(XPath.ToolTip.NCB.Button.Show), "No claim bonus protection tool tip icon is not displayed.");
                Reporting.IsFalse(IsControlDisplayed(XPath.ToolTip.NCB.NCBToggleState), "Yes/No option for No claim bonus protection is not displayed.");
            }
        }

        // TODO: SPK-6704 Remove this logic when NCB toggle is removed from the UI
        private void VerifyNCB(bool? hasNcbProtection, string NCBLevel, bool? isMaxNCB)
        {
            if (NCBLevel != null && isMaxNCB == true)
            {
                Reporting.AreEqual(hasNcbProtection, NoClaimBonus, "no Claim Bonus");
            }
        }

        /// <summary>
        /// This method is checking the default UI behaviour for different cover type
        /// Method not making any changes to the UI element
        /// </summary>
        public void VerifyUIForCoverType(EndorseCar endorseCar)
        {
            switch (endorseCar.CoverType)
            {
                case MotorCovers.MFCO:
                    if(!endorseCar.ChangeMakeAndModel)
                    {
                        VerifyAgreedValue(endorseCar.OriginalPolicyData.MotorAsset.TotalInsuredValue);
                    }                    
                    VerifyHireCar(endorseCar.OriginalPolicyData.HasHireCarCover);
                    VerifyNCB(endorseCar.OriginalPolicyData.MotorAsset.HasNcbProtection, endorseCar.OriginalPolicyData.MotorAsset.NcbLevel, endorseCar.OriginalPolicyData.MotorAsset.IsMaxNcbLevel);
                    break;
                case MotorCovers.TFT:
                    if (!endorseCar.ChangeMakeAndModel)
                    {
                        VerifyAgreedValue(endorseCar.OriginalPolicyData.MotorAsset.TotalInsuredValue);
                    }
                    Reporting.IsFalse(IsControlDisplayed(XPath.AdjustHowYouPaySection.HireCar), "'Hire car after accident' section is not displayed.");
                    Reporting.IsFalse(IsControlDisplayed(XPath.AdjustHowYouPaySection.NCB), "'No claim bonus protection' section is not displayed.");
                    break;
                case MotorCovers.TPO:
                    Reporting.IsFalse(IsControlDisplayed(XPath.AdjustHowYouPaySection.AgreedValue), "'Agreed value' section is not displayed.");
                    Reporting.IsFalse(IsControlDisplayed(XPath.AdjustHowYouPaySection.HireCar), "'Hire car after accident' section is not displayed.");
                    Reporting.IsFalse(IsControlDisplayed(XPath.AdjustHowYouPaySection.NCB), "'No claim bonus protection' section is not displayed.");
                    break;
                default:
                    throw new NotImplementedException($"{endorseCar.CoverType} not supported");                    
            }
        }

    }
}
