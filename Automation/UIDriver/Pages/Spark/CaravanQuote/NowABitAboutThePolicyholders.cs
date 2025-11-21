using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.CaravanQuote
{
    public class NowABitAboutThePolicyholders : SparkBasePage
    {

        private class XPath
        {
            public static class General
            {
                public static class Section
                {
                    public const string Header = "//h2[contains(text(),'Now, a bit about the policyholders')]";
                    public const string ClaimInPastThreeYears = "//h3[@data-testid='inThePastThreeYears']";
        }
                public const string Yes = "//button[text()='Yes']";
                public const string No = "//button[text()='No']";
            }
            public static class Button
            {
                public const string Next = FORM + "//button[@data-testid='submit']";
            }
            public static class MainPHDoB
            {
                public const string Input = "id('yourDateOfBirth')";
                public const string Label = "//div[@data-testid='yourDateOfBirth']/../label";
            }
            public static class AddPH
            {
                public const string Toggle = "id('input-addAdditionalPolicyholder')";
                public const string Label = "//legend[@id='label-addAdditionalPolicyholder']";
            }
            public static class JointPHDoB
            {
                public const string Input = "id('additionalPolicyholderDateOfBirth')";
                public const string Label = "//div[@data-testid='additionalPolicyholderDateOfBirth']/../label";
            }
            public static class HadAccidents
            {
                public const string Toggle = "id('input-anyAccident')";
                public const string Label = "//legend[@id='label-anyAccident']";
            }
            public static class HadConvictions
            {
                public const string Toggle = "id('input-hadLicenceCancelled')";
                public const string Label = "//legend[@id='label-hadLicenceCancelled']";
            }
            public static class MembershipLevel
            {
                public const string Input = "id('select-membership-level')";
                public const string DropdownOptions = "//ul[@role='listbox']/li";
            }
            public static class ImportantInfo
            {
                public const string Title = "//b[text()='Important information']";
                public const string Paragraph1 = "//p[text()='When answering our questions you have a duty to answer them honestly, accurately and to the best of your knowledge. The duty applies to you and anyone else insured under the policy. If you answer for another person, we will treat your answers as theirs. Your duty continues until we insure you.']";
                public const string Paragraph2 = "//p[text()='If you do not meet the duty your policy may be cancelled, or treated as if it never existed and your claim may be rejected or not paid in full. This insurance is a consumer insurance contract.']";
            }
        }

        private class Expected
        {
            //Expected Labels Text
            public const string LABEL_PH1_DOB                   = "Your date of birth";
            public const string LABEL_PH2_DOB                   = "Great, what's their date of birth?";
            public const string LABEL_ADD_PH2                   = "Do you want to add a joint policyholder?\r\nThis could be someone who tows the caravan regularly or manages the policy.";
            public const string LABEL_VIEW_QUOTE                = "View quote";

            //Expected Info Text
            public const string INFO_INFORMATION_TITLE          = "Important information";
            public const string INFO_INFORMATION_P1             = "When answering our questions you have a duty to answer them honestly, accurately and to the best of your knowledge. The duty applies to you and anyone else insured under the policy. If you answer for another person, we will treat your answers as theirs. Your duty continues until we insure you.";
            public const string INFO_INFORMATION_P2             = "If you do not meet the duty your policy may be cancelled, or treated as if it never existed and your claim may be rejected or not paid in full. This insurance is a consumer insurance contract.";

            //Expected In the past three years, have you or any policyholders had... questions labels
            public const string HEADER_INTHEPAST3YEARS          = "In the past three years, have you or any policyholders had...";
            public const string LABEL_ANYACCIDENT               = "Any accidents or made any claims, to any vehicle, regardless of blame?";
            public const string LABEL_HADLICENCECANCELLED       = "Their driver's licence cancelled, suspended or any special conditions applied?";
        }

        public NowABitAboutThePolicyholders(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.General.Section.Header);
                GetElement(XPath.AddPH.Toggle);
                GetElement(XPath.HadAccidents.Toggle);
                GetElement(XPath.HadConvictions.Toggle);
                GetElement(XPath.Button.Next);
            }
            catch (NoSuchElementException)
            {
                return false;
            }

            Reporting.LogPageChange("Caravan Quote page - Now A Bit About The Policyholders");
            return true;
        }

        #region Settable properties and controls
        public string MainPHDOB
        {
            get => GetValue(XPath.MainPHDoB.Input);
            set => WaitForTextFieldAndEnterText(XPath.MainPHDoB.Input, value);
        }

        public string MembershipLevel
        {
            get => GetInnerText(XPath.MembershipLevel.Input);
            set => WaitForSelectableAndPickFromDropdown(XPath.MembershipLevel.Input, XPath.MembershipLevel.DropdownOptions, value);
        }

        public bool IsWithJointPolicyholder
        {
            get => GetBinaryToggleState(XPath.AddPH.Toggle, XPath.General.Yes, XPath.General.No);
            set => ClickBinaryToggle(XPath.AddPH.Toggle, XPath.General.Yes, XPath.General.No, value);
        }
        public string JointPHDOB
        {
            get => GetValue(XPath.JointPHDoB.Input);
            set => WaitForTextFieldAndEnterText(XPath.JointPHDoB.Input, value);
        }

        public bool IsWithAccidents
        {
            get => GetBinaryToggleState(XPath.HadAccidents.Toggle, XPath.General.Yes, XPath.General.No);
            set => ClickBinaryToggle(XPath.HadAccidents.Toggle, XPath.General.Yes, XPath.General.No, value);
        }

        public bool IsWithConvictions
        {
            get => GetBinaryToggleState(XPath.HadConvictions.Toggle, XPath.General.Yes, XPath.General.No);
            set => ClickBinaryToggle(XPath.HadConvictions.Toggle, XPath.General.Yes, XPath.General.No, value);
        }
        #endregion

        /// <summary>
        /// Supports Spark version of caravan B2C
        /// Fills details about the Policyholders(DOB, Joint policyholder DOB, Past accidents and conviction information)
        /// </summary>
        /// <param name="quoteCaravan"></param>
        public void FillPolicyholderInformation(QuoteCaravan quoteCaravan)
        {
            VerifyPageLabels();

            var mainPH = quoteCaravan.PolicyHolders[0];

            //If contact Skipped declaring membership on Page 1, then we show DOB field
            if (mainPH.SkipDeclaringMembership)
            {
                //Verify PH1 DOB Label
                Reporting.AreEqual(Expected.LABEL_PH1_DOB, GetInnerText(XPath.MainPHDoB.Label), "main policy holder dob text");
                MainPHDOB = mainPH.DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_HYPHENS);
            }

            //If contact declared membership on Page 1, and if they are a MultiMatch,
            //then we do not show the DOB field, but we show the 'membership tier' selection dropdown
            //IsMultiMatchRSAMember: This is used to identify if the RSA member is a Multi-Match in Member Central.
            //                       This will be set as part of quote input test data
            else if (mainPH.IsMultiMatchRSAMember && !(mainPH.SkipDeclaringMembership))
            {
                MembershipLevel = mainPH.MembershipTier.GetDescription();
            }

            //If the above 2 conditions were not met, the contact is a single match and we do not show DOB field and Membership Level question
            else
            {
                Reporting.IsFalse(IsControlDisplayed(XPath.MainPHDoB.Input), "Date of Birth field is not displayed");
                Reporting.IsFalse(IsControlDisplayed(XPath.MembershipLevel.Input), "Membership Level question is not displayed");
            }

            bool hasJointPolicyholder = quoteCaravan.PolicyHolders.Count > 1;
            IsWithJointPolicyholder = hasJointPolicyholder;

            //Right now spark caravan allows only 1 joint policy holder
            if (hasJointPolicyholder)
            {
                //Verify PH2 DOB Label Great, what's their date of birth?
                Reporting.AreEqual(Expected.LABEL_PH2_DOB, GetInnerText(XPath.JointPHDoB.Label), "joint policy holder dob text");
                JointPHDOB = quoteCaravan.PolicyHolders[1].DateOfBirth.ToString(DataFormats.DATE_FORMAT_FORWARD_HYPHENS);
            }
            Reporting.Log("Capturing PH/Joint PH information before answering disclosure questions:", _browser.Driver.TakeSnapshot());
            IsWithAccidents = false;
            IsWithConvictions = false;
        }

        public void ClickNext()
        {
            ClickControl(XPath.Button.Next);
        }

        private void VerifyPageLabels()
        {

            //Verify the Do you want to add a joint policyholder?
            Reporting.AreEqual(Expected.LABEL_ADD_PH2, GetInnerText(XPath.AddPH.Label), "add joint policyholder text");

            //Verify the important information text
            Reporting.AreEqual(Expected.INFO_INFORMATION_TITLE, GetInnerText(XPath.ImportantInfo.Title), "Important Information text");
            Reporting.IsTrue(IsControlDisplayed(XPath.ImportantInfo.Paragraph1), $"Expected: {Expected.INFO_INFORMATION_P1}");
            Reporting.IsTrue(IsControlDisplayed(XPath.ImportantInfo.Paragraph2), $"Expected: {Expected.INFO_INFORMATION_P2}");

            //Validate the In the past three years, have you or any policyholders had... questions Labels
            Reporting.AreEqual(Expected.HEADER_INTHEPAST3YEARS, GetInnerText(XPath.General.Section.ClaimInPastThreeYears), "In the past three years header");
            Reporting.AreEqual(Expected.LABEL_ANYACCIDENT, GetInnerText(XPath.HadAccidents.Label), "Any accident question label");
            Reporting.AreEqual(Expected.LABEL_HADLICENCECANCELLED, GetInnerText(XPath.HadConvictions.Label), "Had licence cancelled question label");

            //Verify View quote Button
            Reporting.AreEqual(Expected.LABEL_VIEW_QUOTE, GetInnerText(XPath.Button.Next), "button text");
        }
    }
}