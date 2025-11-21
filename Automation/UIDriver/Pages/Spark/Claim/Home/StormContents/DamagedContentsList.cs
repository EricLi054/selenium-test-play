using OpenQA.Selenium;
using Rac.TestAutomation.Common;
using System;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;

namespace UIDriver.Pages.Spark.Claim.Home
{
    public class DamagedContentsList : SparkBasePage
    {
        #region Constants
        private class Constants
        {
            public static readonly string Header = "Tell us about your damaged contents";
            public class Field
            {
                public class Label
                {
                    public static readonly string AnyOtherDamagedContents   = "Do you have any other damaged contents?";
                    public static readonly string AnyDamagedContents        = "Do you have any damaged contents?";
                    public static readonly string ListEachItem              = "List each item that's been damaged";
                    public static readonly string NameAndDescription        = "Name and description of item";
                    public static readonly string EstimatedValue            = "Estimated value";
                }
                public class Button
                {
                    public static readonly string Yes               = "Yes";
                    public static readonly string No                = "No";
                    public static readonly string AddAnotherItem    = "Add another item";
                }
                public class Validation
                {
                    public static readonly string AnyOtherDamaged   = "Please select Yes or No";
                    public static readonly string Description       = "Please enter a description of the item";
                    public static readonly string Value             = "Please enter an estimated value";
                }
            }
            public class Alert
            {
                public static readonly string NoDamageTitleSingle = "Looks like there's no damage to claim for";
                public static readonly string NoDamageBodySingle  = "Please check your answers.";
                public static readonly string NoDamageTitleCombo  = "Looks like there's no contents damage to claim for";
                public static readonly string NoDamageBodyCombo   = "Please check your answers or select 'Next' to continue.";
            }
        }
        #endregion
        #region XPaths
        private class XPath
        {
            public static readonly string Header        = "id('header')";
            public static readonly string ClaimNumber   = "id('claimNumberDisplay')";
            public static readonly string NextButton    = "id('submit-button')";
            public class Field
            {
                public static readonly string AnyOtherDamagedContents   = "id('anyOtherDamagedContentsQuestion')";
                public class Button
                {
                    public static readonly string Yes               = "//button[text()='Yes']";
                    public static readonly string No                = "//button[text()='No']";
                    public static readonly string AddAnotherItem    = "id('contentsListQuestion-add-button')";
                }
                public class Label
                {
                    public static readonly string OtherDamagedContents  = "id('label-anyOtherDamagedContentsQuestion')";
                    public static readonly string ListEachItem          = "id('contentsListQuestion-label')";
                    public static readonly string NameAndDescription    = "id('contentsList-0-name-label')";
                    public static readonly string EstimatedValue        = "id('contentsList-0-value-label')";
                }
                public class Validation
                {
                    public static readonly string AnyOtherDamaged   = "id('helper-text-anyOtherDamagedContentsQuestion')";
                    public static readonly string Description       = "id('contentsList-0-name-field-helper-text')";
                    public static readonly string Value             = "id('contentsList-0-value-field-helper-text')";
                }
            }
            public class Alert
            {
                public static readonly string NoDamageTitle = "id('notification-card-title')";
                public static readonly string NoDamageBody  = "id('notification-card-paragraph-0')";
            }
        }
        #endregion
        #region Settable properties and controls
        /// <summary>
        /// When affected covers are Contents only or Contents & Fence.
        /// </summary>
        public bool anyOtherDamagedContents
        {
            get => GetBinaryToggleState(XPath.Field.AnyOtherDamagedContents, XPath.Field.Button.Yes, XPath.Field.Button.No);
            set => ClickBinaryToggle(XPath.Field.AnyOtherDamagedContents, XPath.Field.Button.Yes, XPath.Field.Button.No, value);
        }

        /// <summary>
        /// When affected covers are Building & Contents, or Building, Contents & Fence.
        /// </summary>
        public bool anyDamagedContents
        {
            get => GetBinaryToggleState(XPath.Field.AnyOtherDamagedContents, XPath.Field.Button.Yes, XPath.Field.Button.No);
            set => ClickBinaryToggle(XPath.Field.AnyOtherDamagedContents, XPath.Field.Button.Yes, XPath.Field.Button.No, value);
        }
        #endregion

        public DamagedContentsList(Browser browser) : base(browser)
        { }

        public override bool IsDisplayed()
        {
            try
            {
                GetElement(XPath.Field.AnyOtherDamagedContents);
                GetElement(XPath.NextButton);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            Reporting.LogPageChange("Spark Storm Claim - Damaged Contents List");
            Reporting.Log($"Capturing screenshot of page as shown on arrival", _driver.TakeSnapshot());
            return true;
        }

        /// <summary>
        /// Answer the key question on this page Yes or No depending on the test data set up for this scenario. 
        /// If optional flag detailedUiChecking = true then validation of the field labels etc will be included.
        /// 
        /// Note: Where checks are dependant on the value of 'claim.ContentsDamage.IsCarpetTooWet' you may wonder 
        /// if we should also be checking the value of 'claim.StormWaterDamageCheckboxes.BadlySoakedCarpets', 
        /// however the latter is determined by the value of the former so this is unnecessary.
        /// </summary>
        /// <param name="detailedUiChecking">Optional parameter, if set to true will investigate field labels, validation errors etc</param>
        public void AreAnyOtherItemsDamaged(Browser browser, ClaimHome claim, bool detailedUiChecking = false)
        {
            if (detailedUiChecking)
            {
                Reporting.LogMinorSectionHeading("Detailed UI Checking for Damaged Contents List");
                
                if(claim.DamagedCovers == AffectedCovers.ContentsOnly ||
                   claim.DamagedCovers == AffectedCovers.ContentsAndFence)
                {
                    Reporting.AreEqual(Constants.Field.Label.AnyOtherDamagedContents,
                        GetInnerText(XPath.Field.Label.OtherDamagedContents), "expected text of any other damaged contents items field label against text on page");
                    Reporting.AreEqual(Constants.Field.Button.Yes, GetInnerText(XPath.Field.Button.Yes), 
                        "expected copy on button for setting damaged contents items TRUE against text on page");
                    Reporting.AreEqual(Constants.Field.Button.No, GetInnerText(XPath.Field.Button.No), 
                        "expected copy on button for setting damaged contents items FALSE against text on page");
                    TriggerFieldValidation();
                    Reporting.AreEqual(Constants.Field.Validation.AnyOtherDamaged,
                        GetInnerText(XPath.Field.Validation.AnyOtherDamaged), "expected field validation message for 'any other damaged contents?'");
                    
                    if (!claim.ContentsDamage.IsWaterDamagedCarpets
                     && !claim.ContentsDamage.IsCarpetTooWet)
                    {
                        NoContentsDamageReportedAlert(claim);
                    }
                }
                else if(claim.DamagedCovers == AffectedCovers.BuildingAndContents ||
                        claim.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
                {
                    Reporting.AreEqual(Constants.Field.Label.AnyDamagedContents,
                        GetInnerText(XPath.Field.Label.OtherDamagedContents), "expected text of any damaged contents items field label against text on page");
                    Reporting.AreEqual(Constants.Field.Button.Yes, GetInnerText(XPath.Field.Button.Yes),
                        "expected copy on button for setting damaged contents items TRUE against text on page");
                    Reporting.AreEqual(Constants.Field.Button.No, GetInnerText(XPath.Field.Button.No),
                        "expected copy on button for setting damaged contents items FALSE against text on page");
                    TriggerFieldValidation();
                    Reporting.AreEqual(Constants.Field.Validation.AnyOtherDamaged,
                        GetInnerText(XPath.Field.Validation.AnyOtherDamaged), "expected field validation message for 'any damaged contents?'");

                    if (!claim.ContentsDamage.IsWaterDamagedCarpets
                     && !claim.ContentsDamage.IsCarpetTooWet
                     && claim.StormWaterDamageCheckboxes.NoWaterDamage)
                    {
                        NoContentsDamageReportedAlert(claim);
                    }
                }
                else
                {
                    throw new NotImplementedException($"An answer for a claim scenario where damage is to '{claim.DamagedCovers}' " +
                        $"has not been accounted for, investigation of this automated test is required.");
                }
            }

            AnswerQuestion(claim.DamagedCovers, claim.ContentsDamage);

            if (!claim.ContentsDamage.IsCarpetTooWet
                && claim.ContentsDamage.IsOtherStormDamagedContents)
            {
                if (detailedUiChecking)
                {
                    TriggerFieldValidation();
                    Reporting.AreEqual(Constants.Field.Label.ListEachItem, 
                        GetInnerText(XPath.Field.Label.ListEachItem), "instructions when there are other items damaged");
                    Reporting.AreEqual(Constants.Field.Label.NameAndDescription, 
                        GetInnerText(XPath.Field.Label.NameAndDescription), "expected label of item description field");
                    Reporting.AreEqual(Constants.Field.Label.EstimatedValue, 
                        GetInnerText(XPath.Field.Label.EstimatedValue), "expected label of item value field");
                    Reporting.AreEqual(Constants.Field.Validation.Description, 
                        GetInnerText(XPath.Field.Validation.Description), "expected field validation message for item description field");
                    Reporting.AreEqual(Constants.Field.Validation.Value, 
                        GetInnerText(XPath.Field.Validation.Value), "expected field validation message for item value field");
                }
                PopulateDamagedContentsList(browser, claim);
            }
        }

        /// <summary>
        /// This method checks if the scenario will allow triggering of the alert displayed 
        /// when the user has not provided any information about damage to contents on a 
        /// claim where contents are included. If so, trigger and verify the alert text before 
        /// continuing.
        /// </summary>
        /// <param name="contentsDamage">The damage details for Contents generated for this test scenario</param>
        private void NoContentsDamageReportedAlert(ClaimHome claim)
        {
            Reporting.Log($"Water damaged carpets = {claim.ContentsDamage.IsWaterDamagedCarpets} " +
                $"+ Too Wet to Dry = {claim.ContentsDamage.IsCarpetTooWet} + answering 'No' here will trigger alert " +
                $"due to no contents damage being reported. Selecting 'No'.");
                
            ClickControl(XPath.Field.Button.No);
                
            Reporting.Log($"Capturing 'No contents damage' alert", _browser.Driver.TakeSnapshot());
            if (claim.DamagedCovers == AffectedCovers.ContentsOnly)
            {
                Reporting.AreEqual(Constants.Alert.NoDamageTitleSingle, GetInnerText(XPath.Alert.NoDamageTitle),
                "expected Title of alert against displayed");
                Reporting.AreEqual(Constants.Alert.NoDamageBodySingle, GetInnerText(XPath.Alert.NoDamageBody),
                    "expected Body of alert against displayed");
            }
            else
            {
                Reporting.AreEqual(Constants.Alert.NoDamageTitleCombo, GetInnerText(XPath.Alert.NoDamageTitle),
                "expected Title of alert against displayed");
                Reporting.AreEqual(Constants.Alert.NoDamageBodyCombo, GetInnerText(XPath.Alert.NoDamageBody),
                    "expected Body of alert against displayed");
            }
        }

        /// <summary>
        /// Actually answer the yes/no question regarding any contents items damaged.
        /// Split out from the main AreAnyOtherItemsDamaged method in an attempt to keep things tidier.
        /// </summary>
        /// <param name="affectedCovers">The affected covers for this claim scenario determine which id values to expect</param>
        /// <param name="contentsDamageDetails">The answer to the question</param>
        /// <exception cref="NotImplementedException"></exception>
        private void AnswerQuestion(AffectedCovers affectedCovers, ContentsDamageDetails contentsDamageDetails)
        {
            if (affectedCovers == AffectedCovers.ContentsOnly ||
                affectedCovers == AffectedCovers.ContentsAndFence)
            {
                Reporting.Log($"Answering '{Constants.Field.Label.AnyOtherDamagedContents}': {contentsDamageDetails.IsOtherStormDamagedContents}");
                anyOtherDamagedContents = contentsDamageDetails.IsOtherStormDamagedContents;
            }
            else if (affectedCovers == AffectedCovers.BuildingAndContents ||
                     affectedCovers == AffectedCovers.BuildingAndContentsAndFence)
            {
                Reporting.Log($"Answering '{Constants.Field.Label.AnyDamagedContents}': {contentsDamageDetails.IsOtherStormDamagedContents}");
                anyDamagedContents = contentsDamageDetails.IsOtherStormDamagedContents;
            }
            else
            {
                throw new NotImplementedException($"An answer for a claim scenario where damage is to '{affectedCovers}' " +
                    $"has not been accounted for, investigation of this automated test is required.");
            }
        }

        /// <summary>
        /// Iterates through StormDamagedItems and adds their details to the next available card
        /// on screeen, then it will select the "Add another item" button to make a new card 
        /// available if the number of cards filled is less than the total count of items in the 
        /// test data for this claim.
        /// </summary>
        private void PopulateDamagedContentsList(Browser browser, ClaimHome claim)
        {
            var itemsInClaim = claim.ContentsDamage.StormDamagedItems;
            int i = 0;
            foreach (var itemInClaim in itemsInClaim)
            {
                Reporting.Log($"Adding item: {itemInClaim.Description} valued ${itemInClaim.Value.ToString()}");
                string xPathItemName = $"id('contentsList-{i}-name-field')";
                string xPathItemValue = $"id('contentsList-{i}-value-field')";

                WaitForTextFieldAndEnterText(xPathItemName, itemInClaim.Description, false);
                WaitForTextFieldAndEnterText(xPathItemValue, itemInClaim.Value.ToString(), false);

                i++;
                if(i < itemsInClaim.Count)
                {
                    ClickControl(XPath.Field.Button.AddAnotherItem);
                }
            }
        }

        private void TriggerFieldValidation()
        {
            ClickControl(XPath.NextButton);
        }

        public void ContinueToNextPage()
        {
            Reporting.Log($"Capturing state of page before continuing", _driver.TakeSnapshot());
            ClickControl(XPath.NextButton);
        }
    }
}
