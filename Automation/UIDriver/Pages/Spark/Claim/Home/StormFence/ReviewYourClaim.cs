using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System.Security.Claims;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;

namespace UIDriver.Pages.Spark.Claim.Home.DividingFence
{
    public class ReviewYourClaim : SparkBasePage
    {
        #region Constants
        public class Constants
        {
            public static readonly string Header                        = "Review and submit your claim";
            public static readonly string ActiveStepper                 = "Review your claim";
            public static readonly string NotificationCard              = "After this, you won't be able to make any changes online";
            public class Summary
            {
                public static readonly string Title                     = "Your claim summary";
                public class SectionHeading
                {
                    public static readonly string YourClaim             = "Your claim";
                    public static readonly string ContactDetails        = "Contact details";
                    public static readonly string WhenItHappened        = "When it happened";
                    public static readonly string MoreAboutDamage       = "More about your building damage";
                    public static readonly string MoreDamageDescribe    = "Describe what happened";
                    public static readonly string WaterDamageInside     = "Water damage inside the home";
                    public static readonly string SafetyChecks          = "Safety checks";
                    public static readonly string YourContents          = "Your contents";
                    public static readonly string DamagedContents       = "Damaged contents";
                }
                public class Prompt
                {
                    public static readonly string DamageType            = "Damage type";
                    public static readonly string Excess                = "Your excess";
                    public static readonly string ContactNumber         = "Contact number";
                    public static readonly string Email                 = "Email";
                    public static readonly string EventDate             = "When did the damage happen?";
                    public static readonly string ApproxTime            = "Approximate time";
                    public static readonly string WaterDamageSelectAll  = "Select all the problems your home has";
                    public static readonly string SafetyChecks          = "Select all the problems your home has";
                    public static readonly string CarpetWaterDamaged    = "Is there water damage to any carpet in the home?";
                    public static readonly string CarpetTooWet          = "Are the carpets so badly soaked that you can't dry them?";
                    public static readonly string OtherDamagedContents  = "Do you have any other damaged contents?";
                    public static readonly string DamagedContents       = "Do you have any damaged contents?";
                    public static readonly string ListEachItem          = "List each item that has been damaged";
                }
            }
        }
        #endregion
        #region XPATHS
        public class XPath
        {
            public static readonly string Header                        = "id('header')";
            public static readonly string ActiveStepper                 = "//button[@aria-selected='true']";
            public static readonly string NotificationCard              = "id('notification-card-title')";
            public class Control
            {
                public static readonly string ExpandSummary             = "id('claim-summary-card-expand')";
                public static readonly string CollapseSummary           = "id('claim-summary-card-collapse')";
                public static readonly string EditContactDetails        = "id('claim-summary-card-contact-details-section-contact-details-subsection-edit')";
                public static readonly string EditFenceRepairs          = "id('claim-summary-card-fence-repairs-section-fence-repairs-subsection-edit')";
                public static readonly string EditYourDamagedFence      = "id('claim-summary-card-your-damaged-fence-section-your-damaged-fence-subsection-edit')";
                public static readonly string EditSecurity              = "id('claim-summary-card-your-propertys-security-section-your-propertys-security-subsection-edit-container')";
                public static readonly string SubmitClaim               = "id('submit-button')";
            }
            public class Summary
            {
                public static readonly string Title              = "id('claim-summary-card-title')";
                public class SectionHeading
                {
                    public static readonly string YourClaim             = "id('claim-summary-card-your-claim-section-title')";
                    public static readonly string ContactDetails        = "id('claim-summary-card-contact-details-section-title')";
                    public static readonly string WhenItHappened        = "id('claim-summary-card-when-it-happened-section-title')";
                    public static readonly string MoreAboutDamage       = "id('claim-summary-card-more-about-your-building-damage-section-title')";                 
                    public static readonly string WaterDamageInside     = "id('claim-summary-card-water-damage-inside-the-home-section-title')";
                    public static readonly string SafetyChecks          = "id('claim-summary-card-safety-checks-section-title')";
                    public static readonly string YourContents          = "id('claim-summary-card-your-contents-section-title')";
                    public static readonly string DamagedContents       = "id('claim-summary-card-damaged-contents-section-title')";
                }
                public class Prompt
                {
                    public static readonly string DamageType            = "id('damage-type-label')";
                    public static readonly string Excess                = "id('your-excess-label')";
                    public static readonly string ContactNumber         = "id('contact-number-label')";
                    public static readonly string Email                 = "id('email-label')";
                    public static readonly string EventDate             = "id('when-did-the-damage-happen-label')";
                    public static readonly string ApproxTime            = "id('approximate-time-label')";
                    public static readonly string MoreDamageDescribe    = "id('more-about-your-building-damage')";
                    public static readonly string WaterDamageSelectAll  = "id('water-damage')";
                    public static readonly string SafetyChecks          = "id('safety-checks')";
                    public static readonly string CarpetWaterDamaged    = "id('is-there-water-damage-to-any-carpet-in-the-home-label')";
                    public static readonly string CarpetTooWet          = "id('are-the-carpets-so-badly-soaked-that-you-cant-dry-them-label')";
                    public static readonly string OtherDamagedContents  = "id('do-you-have-any-other-damaged-contents-label')";
                    public static readonly string DamagedContents       = "id('do-you-have-any-damaged-contents-label')";
                    public static readonly string ListEachItem          = "id('list-damaged-items-label')";
                }
                public class Response
                {
                    public static readonly string DamageType            = "id('damage-type-content')";
                    public static readonly string Excess                = "id('your-excess-content')";
                    public static readonly string ContactNumber         = "id('contact-number-content')";
                    public static readonly string Email                 = "id('email-content')";
                    public static readonly string EventDate             = "id('when-did-the-damage-happen-content')";
                    public static readonly string ApproxTime            = "id('approximate-time-content')";
                    public static readonly string MoreAboutDamage       = "//p[@id='more-about-your-building-damage']/../p[2]";
                    public static readonly string CarpetWaterDamaged    = "id('is-there-water-damage-to-any-carpet-in-the-home-content')";
                    public static readonly string CarpetTooWet          = "id('are-the-carpets-so-badly-soaked-that-you-cant-dry-them-content')";
                    public static readonly string OtherDamagedContents  = "id('do-you-have-any-other-damaged-contents-content')";
                    public static readonly string DamagedContents       = "id('do-you-have-any-damaged-contents-content')";
                    public class WaterDamage
                    {
                        public static readonly string NoWaterDamage                = "//p[contains(text(),'No water dam')]";
                        public static readonly string DampPatchesOrDripping        = "//p[contains(text(),'Damp patches')]";
                        public static readonly string SolidTimberFloorIsWet        = "//p[contains(text(),'Solid timber')]";
                        public static readonly string BadlySoakedCarpets           = "//p[contains(text(),'Carpet is so')]";
                        public static readonly string HouseIsFlooded               = "//p[contains(text(),'House is')]";
                        public static readonly string SewageOrDrainWaterInTheHouse = "//p[contains(text(),'sewage or drain')]";
                        public static readonly string WaterInTheElectrics          = "//p[contains(text(),'electrics')]";
                        public static readonly string OtherWaterDamage             = "//p[contains(text(),'Other water')]";
                    }
                    public class SafetyChecks
                    {
                        public static readonly string Insecure = "//p[contains(text(),'secure my home')]";
                        public static readonly string DangerousLooseItems = "//p[contains(text(),'Dangerous or loose')]";
                        public static readonly string NoPower = "//p[contains(text(),'power to the')]";
                        public static readonly string NoWater = "//p[contains(text(),'water supply to')]";
                        public static readonly string NoAccessKitchenBath = "//p[contains(text(),'kitchen or bathroom')]";
                        public static readonly string NoneOfThese = "//p[contains(text(),'of these')]";
                    }
                }
            }
        }
        #endregion

        public ReviewYourClaim(Browser browser) : base(browser)
        { }

        override public bool IsDisplayed()
        {
            var isDisplayed = false;
            try
            {
                isDisplayed = string.Equals(Constants.ActiveStepper, GetInnerText(XPath.ActiveStepper));
                GetElement(XPath.Control.SubmitClaim);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            if(isDisplayed)
            {
                Reporting.LogPageChange("Spark Claim Page - Review your claim");
            }
            return isDisplayed;
        }

        /// <summary>
        /// Captures the details displayed in the claim summary then compares with the expected values.
        /// </summary>
        public void VerifyDetailedContent(ClaimHome claimHome)
        {
            ExpandClaimDetailsAndCapture(claimHome);
            ValidateClaimSummary(claimHome);
        }

        /// <summary>
        /// Selects the control to Expand the Claim details accordion, then scrolls to ensure 
        /// that the bottom of the claim summary details are visible and captures a screenshot 
        /// for the logs.
        /// This method also verifies the page heading and the notification card content before 
        /// passing off to the ValidateClaimSummary method.
        /// </summary>
        public void ExpandClaimDetailsAndCapture(ClaimHome claimHome)
        {
            ClickControl(XPath.Control.ExpandSummary);
            if (claimHome.DamagedCovers == AffectedCovers.FenceOnly)
            {
                ScrollElementIntoView(XPath.Control.EditSecurity);
            }
            else
            {
                ScrollElementIntoView(XPath.NotificationCard);
            }
            Reporting.Log("Capturing snapshot of expanded claim summary for this Storm claim", _browser.Driver.TakeSnapshot());
            
            Reporting.AreEqual(Constants.Header, GetInnerText(XPath.Header), 
                "expected page heading with displayed heading");
            Reporting.AreEqual(Constants.NotificationCard, GetInnerText(XPath.NotificationCard), 
                "expected copy for notification card against actual value");
        }
        
        /// <summary>
        /// Validates detailed content from the Claim Summary against the expected values including 
        /// those input during this test scenario.
        /// </summary>
        public void ValidateClaimSummary(ClaimHome claimHome)
        {
            Reporting.AreEqual(Constants.Summary.Title, GetInnerText(XPath.Summary.Title), "title of summary");
            

            
            if (claimHome.DamagedCovers == AffectedCovers.ContentsOnly ||
                claimHome.DamagedCovers == AffectedCovers.ContentsAndFence)
            {
                Reporting.AreEqual(Constants.Summary.SectionHeading.YourContents, GetInnerText(XPath.Summary.SectionHeading.YourContents), 
                    "Your contents section title");
                Reporting.AreEqual(Constants.Summary.Prompt.CarpetWaterDamaged, GetInnerText(XPath.Summary.Prompt.CarpetWaterDamaged),
                    "label");
                Reporting.AreEqual(DataHelper.BooleanToStringYesNo(claimHome.ContentsDamage.IsWaterDamagedCarpets), 
                    GetInnerText(XPath.Summary.Response.CarpetWaterDamaged), "response displayed");
                if(claimHome.ContentsDamage.IsWaterDamagedCarpets)
                {
                    Reporting.AreEqual(Constants.Summary.Prompt.CarpetTooWet, GetInnerText(XPath.Summary.Prompt.CarpetTooWet),
                    "label");
                    Reporting.AreEqual(DataHelper.BooleanToStringYesNo(claimHome.ContentsDamage.IsCarpetTooWet),
                        GetInnerText(XPath.Summary.Response.CarpetTooWet), "response displayed");
                }
            }

            if ((claimHome.ContentsDamage != null && 
                 claimHome.ContentsDamage.IsOtherStormDamagedContents) 
               && 
                (claimHome.DamagedCovers == AffectedCovers.ContentsOnly ||
                claimHome.DamagedCovers == AffectedCovers.ContentsAndFence || 
                claimHome.DamagedCovers == AffectedCovers.BuildingAndContents || 
                claimHome.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence))
            {
                Reporting.AreEqual(Constants.Summary.SectionHeading.DamagedContents, GetInnerText(XPath.Summary.SectionHeading.DamagedContents), 
                    "section title");
                
                if ((claimHome.ContentsDamage != null && 
                     claimHome.ContentsDamage.IsOtherStormDamagedContents) 
                   && 
                    (claimHome.DamagedCovers == AffectedCovers.ContentsOnly ||
                     claimHome.DamagedCovers == AffectedCovers.ContentsAndFence))
                {
                    Reporting.AreEqual(Constants.Summary.Prompt.OtherDamagedContents, 
                        GetInnerText(XPath.Summary.Prompt.OtherDamagedContents),
                        "expected prompt for damaged contents items");
                    Reporting.AreEqual(DataHelper.BooleanToStringYesNo(claimHome.ContentsDamage.IsOtherStormDamagedContents),
                        GetInnerText(XPath.Summary.Response.OtherDamagedContents), 
                        "expected response for damaged contents items");
                }
                
                if(claimHome.DamagedCovers == AffectedCovers.BuildingAndContents ||
                claimHome.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
                {
                    Reporting.AreEqual(Constants.Summary.Prompt.DamagedContents, 
                        GetInnerText(XPath.Summary.Prompt.DamagedContents), 
                        "expected prompt for damaged contents items");
                    Reporting.AreEqual(DataHelper.BooleanToStringYesNo(claimHome.ContentsDamage.IsOtherStormDamagedContents),
                        GetInnerText(XPath.Summary.Response.DamagedContents), 
                        "expected response for damaged contents items");
                }
                Reporting.AreEqual(Constants.Summary.Prompt.ListEachItem, GetInnerText(XPath.Summary.Prompt.ListEachItem),
                    "label");

                if (claimHome.DamagedCovers == AffectedCovers.BuildingOnly
                    || claimHome.DamagedCovers == AffectedCovers.BuildingAndContents
                    || claimHome.DamagedCovers == AffectedCovers.BuildingAndFence
                    || claimHome.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
                {
                    // TODO: INSU-818: Once gone live, we can remove the toggle and treat as always true.
                    if (Config.Get().IsClaimHomeMoreAboutYourDamageScreenEnabled())
                    {
                        Reporting.AreEqual(Constants.Summary.SectionHeading.MoreAboutDamage,
                          GetInnerText(XPath.Summary.SectionHeading.MoreAboutDamage), "expected section heading copy is displayed");
                        Reporting.AreEqual(Constants.Summary.SectionHeading.MoreDamageDescribe,
                        GetInnerText(XPath.Summary.Prompt.MoreDamageDescribe), "expected section sub heading copy is displayed");
                        Reporting.AreEqual(DataHelper.StripLineFeedAndCarriageReturns(claimHome.AccountOfEvent, false),
                            DataHelper.StripLineFeedAndCarriageReturns(GetInnerText(XPath.Summary.Response.MoreAboutDamage), false), "expected response for more about damage is displayed");
                    }
                }

                if (claimHome.StormWaterDamageCheckboxes != null &&
                    (claimHome.DamagedCovers == AffectedCovers.BuildingOnly
                    || claimHome.DamagedCovers == AffectedCovers.BuildingAndContents
                    || claimHome.DamagedCovers == AffectedCovers.BuildingAndFence
                    || claimHome.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence))
                {
                    Reporting.AreEqual(Constants.Summary.SectionHeading.WaterDamageInside, 
                        GetInnerText(XPath.Summary.SectionHeading.WaterDamageInside), "expected section heading copy is displayed");
                    Reporting.AreEqual(Constants.Summary.Prompt.WaterDamageSelectAll,
                        GetInnerText(XPath.Summary.Prompt.WaterDamageSelectAll), "expected section prompt copy is displayed");
                    ValidateWaterDamageCheckboxes(claimHome.StormWaterDamageCheckboxes);
                }

                if (claimHome.StormSafetyCheckboxes != null && 
                    (claimHome.DamagedCovers == AffectedCovers.BuildingOnly ||
                    claimHome.DamagedCovers == AffectedCovers.BuildingAndContents ||
                    claimHome.DamagedCovers == AffectedCovers.BuildingAndFence ||
                    claimHome.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence))
                {
                    Reporting.AreEqual(Constants.Summary.SectionHeading.SafetyChecks,
                        GetInnerText(XPath.Summary.SectionHeading.SafetyChecks), "expected section heading copy is displayed");
                    Reporting.AreEqual(Constants.Summary.Prompt.SafetyChecks,
                        GetInnerText(XPath.Summary.Prompt.SafetyChecks), "expected section prompt copy is displayed");
                    ValidateSafetyChecks(claimHome.StormSafetyCheckboxes);
                }
            }
        }

        /// <summary>
        /// If a checkbox was selected on the 'Water damage' page, then we verify that it should be represented here
        /// on the claim summary.
        /// </summary>
        /// <param name="expectedWaterDamageCheckboxes"></param>
        public void ValidateWaterDamageCheckboxes(StormWaterDamageCheckboxes expectedWaterDamageCheckboxes)
        {
            if (expectedWaterDamageCheckboxes.NoWaterDamage)
            {
                Reporting.AreEqual(StormWaterDamageCheckboxesOptions.NoWaterDamage.GetDescription(),
                    GetInnerText(XPath.Summary.Response.WaterDamage.NoWaterDamage), "expected value is displayed for selected checkbox .");
            }

            if (expectedWaterDamageCheckboxes.DampPatchesOrDripping)
            {
                Reporting.AreEqual(StormWaterDamageCheckboxesOptions.DampPatchesOrDripping.GetDescription(), 
                    GetInnerText(XPath.Summary.Response.WaterDamage.DampPatchesOrDripping), "expected value is displayed for selected checkbox .");
            }

            if (expectedWaterDamageCheckboxes.SolidTimberFloorIsWet)
            {
                Reporting.AreEqual(StormWaterDamageCheckboxesOptions.SolidTimberFloorIsWet.GetDescription(), 
                    GetInnerText(XPath.Summary.Response.WaterDamage.SolidTimberFloorIsWet), "expected value is displayed for selected checkbox .");
            }

            if (expectedWaterDamageCheckboxes.BadlySoakedCarpets)
            {
                Reporting.AreEqual(StormWaterDamageCheckboxesOptions.BadlySoakedCarpets.GetDescription(), 
                    GetInnerText(XPath.Summary.Response.WaterDamage.BadlySoakedCarpets), "expected value is displayed for selected checkbox .");
            }

            if (expectedWaterDamageCheckboxes.HouseIsFlooded)
            {
                Reporting.AreEqual(StormWaterDamageCheckboxesOptions.HouseIsFlooded.GetDescription(), 
                    GetInnerText(XPath.Summary.Response.WaterDamage.HouseIsFlooded), "expected value is displayed for selected checkbox .");
            }

            if (expectedWaterDamageCheckboxes.SewageOrDrainWaterInTheHouse)
            {
                Reporting.AreEqual(StormWaterDamageCheckboxesOptions.SewageOrDrainWaterInTheHouse.GetDescription(),
                    GetInnerText(XPath.Summary.Response.WaterDamage.SewageOrDrainWaterInTheHouse), "expected value is displayed for selected checkbox .");
            }

            if (expectedWaterDamageCheckboxes.WaterInTheElectrics)
            {
                Reporting.AreEqual(StormWaterDamageCheckboxesOptions.WaterInTheElectrics.GetDescription(),
                    GetInnerText(XPath.Summary.Response.WaterDamage.WaterInTheElectrics), "expected value is displayed for selected checkbox .");
            }

            if (expectedWaterDamageCheckboxes.OtherWaterDamage)
            {
                Reporting.AreEqual(StormWaterDamageCheckboxesOptions.OtherWaterDamage.GetDescription(),
                    GetInnerText(XPath.Summary.Response.WaterDamage.OtherWaterDamage), "expected value is displayed for selected checkbox .");
            }
        }

        public void ValidateSafetyChecks(StormSafetyCheckboxes expectedSafetyChecksSelected)
        {
            if (expectedSafetyChecksSelected.Insecure)
            {
                Reporting.AreEqual(StormSafetyCheckOptions.Insecure.GetDescription(),
                    GetInnerText(XPath.Summary.Response.SafetyChecks.Insecure), "expected value is displayed for selected checkbox");
            }

            if (expectedSafetyChecksSelected.DangerousLooseItems)
            {
                Reporting.AreEqual(StormSafetyCheckOptions.DangerousLooseItems.GetDescription(),
                    GetInnerText(XPath.Summary.Response.SafetyChecks.DangerousLooseItems), "expected value is displayed for selected checkbox");
            }

            if (expectedSafetyChecksSelected.NoPower)
            {
                Reporting.AreEqual(StormSafetyCheckOptions.NoPower.GetDescription(),
                    GetInnerText(XPath.Summary.Response.SafetyChecks.NoPower), "expected value is displayed for selected checkbox");
            }

            if (expectedSafetyChecksSelected.NoWater)
            {
                Reporting.AreEqual(StormSafetyCheckOptions.NoWater.GetDescription(),
                    GetInnerText(XPath.Summary.Response.SafetyChecks.NoWater), "expected value is displayed for selected checkbox");
            }

            if (expectedSafetyChecksSelected.NoAccessKitchenBath)
            {
                Reporting.AreEqual(StormSafetyCheckOptions.NoAccessKitchenBath.GetDescription(),
                    GetInnerText(XPath.Summary.Response.SafetyChecks.NoAccessKitchenBath), "expected value is displayed for selected checkbox");
            }

            if (expectedSafetyChecksSelected.NoneOfThese)
            {
                Reporting.AreEqual(StormSafetyCheckOptions.NoneOfThese.GetDescription(),
                    GetInnerText(XPath.Summary.Response.SafetyChecks.NoneOfThese), "expected value is displayed for selected checkbox");
            }
        }

        /// <summary>
        /// Capture a screenshot of the page for Extent Report, then select the button to submit the claim.
        /// </summary>
        public void ConfirmAndSubmitClaim()
        {
            Reporting.Log("Capturing Home Claim - Review your claim page before continuing.", _browser.Driver.TakeSnapshot());
            ClickControl(XPath.Control.SubmitClaim);
        }
    }
}
