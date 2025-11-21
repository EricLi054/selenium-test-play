using Rac.TestAutomation.Common;
using OpenQA.Selenium;

namespace UIDriver.Pages.Shield
{
    public class TreeItemDriverDetails : BaseShieldPage
    {
        #region XPATHS
        private const string XP_TEXT_HEADER = "//div[@title='Driver Contact Details']";
        private const string XP_BUTTON_NEXT = "id('Next')";

        // Disclosures:
        private const string XP_BUTTON_DISCLOSURE_LICENSE_SUSPENSION = "id('s2id_hasLicenceSuspendedId')";
        private const string XP_OPTIONS_DISCLOSURE_LICENSE_SUSPENSION = "id('select2-results-1')/li/div";

        private const string XP_BUTTON_DISCLOSURE_UNDER_INFLUENCE = "id('s2id_IDITForm@driverVO@faildOnBloodTest@id')";
        private const string XP_OPTIONS_DISCLOSURE_UNDER_INFLUENCE = "id('select2-results-2')/li/div";

        private const string XP_BUTTON_DISCLOSURE_LICENSE_OVER_2YEARS = "id('s2id_IDITForm@driverVO@hasTowYearsDriverLience@id')";
        private const string XP_OPTIONS_DISCLOSURE_LICENSE_OVER_2YEARS = "id('select2-results-3')/li/div";
        #endregion

        #region Settable properties and controls
        public bool? HasLicenseBeenSuspendedPast3Years
        {
            get => DataHelper.StringYesNoToNullableBool(GetInnerText($"{XP_BUTTON_DISCLOSURE_LICENSE_SUSPENSION}//span[@class='select2-chosen']"));
            set
            {
                if (value.HasValue)
                {
                    WaitForSelectableAndPickFromDropdown(XP_BUTTON_DISCLOSURE_LICENSE_SUSPENSION, XP_OPTIONS_DISCLOSURE_LICENSE_SUSPENSION, DataHelper.BooleanToStringYesNo(value.Value));
                }
            }
        }

        public bool? WasDrivingUnderTheInfluence
        {
            get => DataHelper.StringYesNoToNullableBool(GetInnerText($"{XP_BUTTON_DISCLOSURE_UNDER_INFLUENCE}//span[@class='select2-chosen']"));
            set
            {
                if (value.HasValue)
                {
                    WaitForSelectableAndPickFromDropdown(XP_BUTTON_DISCLOSURE_UNDER_INFLUENCE, XP_OPTIONS_DISCLOSURE_UNDER_INFLUENCE, DataHelper.BooleanToStringYesNo(value.Value));
                }
            }
        }

        public bool? HasHadLicenseMoreThan2Years
        {
            get => DataHelper.StringYesNoToNullableBool(GetInnerText($"{XP_BUTTON_DISCLOSURE_LICENSE_OVER_2YEARS}//span[@class='select2-chosen']"));
            set
            {
                if (value.HasValue)
                {
                    WaitForSelectableAndPickFromDropdown(XP_BUTTON_DISCLOSURE_LICENSE_OVER_2YEARS, XP_OPTIONS_DISCLOSURE_LICENSE_OVER_2YEARS, DataHelper.BooleanToStringYesNo(value.Value));
                }
            }
        }
        #endregion

        public TreeItemDriverDetails(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                GetElement(XP_TEXT_HEADER);
                GetElement(XP_BUTTON_NEXT);
                isDisplayed = true;
            }
            catch (NoSuchElementException)
            {
                isDisplayed = false;
            }
            return isDisplayed;
        }

        /// <summary>
        /// Click the "Next" button, which should take the user to the
        /// Damages view for the claim.
        /// </summary>
        public void ClickNext()
        {
            ClickControl(XP_BUTTON_NEXT);
        }
    }
}
