using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using static Rac.TestAutomation.Common.Constants.Contacts;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;

namespace Tests.ActionsAndValidations
{
    public static class VerifyClaimGeneral
    {
        private class Constants
        {
            public class ClaimantEventDescription
            {
                public static readonly string TempFenceRequired = "^^TEMPORARY FENCE REQUIRED^^";
            }
        }

        private static int MAX_WITNESS_COUNT    = 2;
        private static int MAX_THIRDPARTY_COUNT = 1;

        /// <summary>
        /// Retrieves all the contacts attached to the claim as witnesses and
        /// verifies that their names match.
        /// </summary>
        public static void VerifyClaimWitnessDetails(string claimNumber, List<Contact> expectedWitnesses)
        {
            var dbWitnesses = ShieldClaimDB.GetClaimWitnessDetails(claimNumber);
            // B2C limits the number of witnesses to be added to a motor claim.
            var expectedWitnessCount = expectedWitnesses == null ? 0 :
                expectedWitnesses.Count > MAX_WITNESS_COUNT ? MAX_WITNESS_COUNT : expectedWitnesses.Count;

            Reporting.Log("================================================================");
            Reporting.Log($"Verify Witness details for Claim: {claimNumber}");
            Reporting.AreEqual(dbWitnesses.Count, expectedWitnessCount, "count of witnesses");

            for (int i = 0; i < expectedWitnessCount; i++)
            {
                var witness = expectedWitnesses[i];
                var matchingDBWitness = dbWitnesses.First(x => x.FirstName.Equals(witness.FirstName, StringComparison.InvariantCultureIgnoreCase) &&
                                                               x.Surname.Equals(witness.Surname, StringComparison.InvariantCultureIgnoreCase));

                Reporting.IsNotNull(matchingDBWitness, $"witness {witness.FirstName} {witness.Surname} logged against claim in Shield DB.");
            }
        }

        /// <summary>
        /// Retrieves all the contacts attached to the claim as a third party and
        /// verifies that their names match.
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <param name="expectedThirdParty"></param>
        /// <param name="dbThirdParties"></param>
        public static void VerifyClaimThirdPartyDetails(string claimNumber, List<Contact> expectedThirdParties, List<ContactClaimDB> dbThirdParties)
        {
            if (expectedThirdParties == null || expectedThirdParties.Count == 0)
            { return; } // Nothing to do.

            // B2C limits the number of third parties/offenders to be added to a motor claim.
            var expectedTPCount = expectedThirdParties.Count > MAX_THIRDPARTY_COUNT ? MAX_THIRDPARTY_COUNT : expectedThirdParties.Count;

            Reporting.Log("================================================================");
            Reporting.Log($"Verify Third Party details for Claim: {claimNumber}");
            Reporting.AreEqual(dbThirdParties.Count, expectedTPCount, "count of third party contacts");

            for (int i = 0; i < expectedTPCount; i++)
            {
                var thirdParty = expectedThirdParties[i];
                ContactClaimDB matchingDBThirdParty = null;
                if (!string.IsNullOrEmpty(thirdParty.FirstName))
                {
                    matchingDBThirdParty = dbThirdParties.First(x => x.FirstName.Equals(thirdParty.FirstName, StringComparison.InvariantCultureIgnoreCase));
                }
                else if (!string.IsNullOrEmpty(thirdParty.Surname))
                {
                    matchingDBThirdParty = dbThirdParties.First(x => x.Surname.Equals(thirdParty.Surname, StringComparison.InvariantCultureIgnoreCase));
                }
                else
                {
                    matchingDBThirdParty = dbThirdParties.FirstOrDefault(x => x.MobilePhoneNumber.Equals(thirdParty.MobilePhoneNumber, StringComparison.InvariantCultureIgnoreCase));
                }

                Reporting.IsNotNull(matchingDBThirdParty, $"third party {thirdParty.FirstName} {thirdParty.Surname} logged against claim in Shield DB.");

                //Verify Surname
                var expectedSurname = string.IsNullOrEmpty(thirdParty.Surname) ? "." : thirdParty.Surname;
                Reporting.AreEqual(expectedSurname, matchingDBThirdParty.Surname);

                // Verify email status - Tests do not currently enter an email for TP. So should always be "Not Provided"
                Reporting.AreEqual("Not Provided", matchingDBThirdParty.DBTPEMailStatus);

                // Verify entered phone number status
                var expectedPhoneStatus = string.IsNullOrEmpty(thirdParty.GetPhone()) ? "Not Provided" : thirdParty.GetPhone();
                if (!string.IsNullOrEmpty(thirdParty.GetPhone()))
                {
                    expectedPhoneStatus = expectedPhoneStatus.StartsWith(PhonePrefix.Mobile.GetDescription()) ? "Mobile" : "Home telephone";
                }                
                Reporting.AreEqual(expectedPhoneStatus, matchingDBThirdParty.DBTPPhoneStatus);

                // Verify TP's address status. This should always be "Mailing address"
                if (thirdParty.MailingAddress != null)
                {
                    Reporting.AreEqual("Mailing address", matchingDBThirdParty.DBTPAddressStatus);
                    Reporting.AreEqual(thirdParty.MailingAddress.QASStreetNameOnly(), matchingDBThirdParty.MailingAddress.StreetOrPOBox,
                                       ignoreCase: true, "Third Party's mailing address street name is as entered");
                    Reporting.AreEqual(thirdParty.MailingAddress.Suburb, matchingDBThirdParty.MailingAddress.Suburb,
                                       ignoreCase: true, "Third Party's mailing address suburb is as entered");
                }
            }
        }

        /// <summary>
        /// This method should be expanded to include other verifications regarding the content in the 
        /// Claimant Event Description field in Shield.
        /// To begin with, we're checking for the appearance of "^^TEMPORARY FENCE REQUIRED^^" when a 
        /// claim includes damage to a fence and a make-safe has been indicated by the member. There 
        /// should be no mention of temporary fence in any other scenario.
        /// </summary>
        public static void VerifyClaimantEventDescription(ClaimHome claimData)
        {
            string claimantEventDescription = ShieldHomeClaimDB.GetClaimantEventDescriptionFromShield(claimData.ClaimNumber);

            Reporting.LogMinorSectionHeading("Beginning verification of Claimant Event Description");
            Reporting.Log($"Beginning by logging the raw Claimant Event Description text:");
            Reporting.Log($"{claimantEventDescription}");

            if (claimData.DamagedCovers == AffectedCovers.FenceOnly
             || claimData.DamagedCovers == AffectedCovers.ContentsAndFence
             || claimData.DamagedCovers == AffectedCovers.BuildingAndFence
             || claimData.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
            {
                if (claimData.FenceDamage.IsAreaSafe)
                {
                    Reporting.IsFalse(claimantEventDescription.Contains(Constants.ClaimantEventDescription.TempFenceRequired),
                        $"Claimant Event Description should not include any mention of '{Constants.ClaimantEventDescription.TempFenceRequired}' " +
                        $"as the member has not indicated that a make-safe is required");
                }
                else
                {
                    Reporting.IsTrue(claimantEventDescription.Contains(Constants.ClaimantEventDescription.TempFenceRequired),
                        $"Claimant Event description should contain '{Constants.ClaimantEventDescription.TempFenceRequired}' " +
                        $"as the member has indicated that a make-safe is required");
                }
            }
            else
            {
                Reporting.IsFalse(claimantEventDescription.Contains(Constants.ClaimantEventDescription.TempFenceRequired),
                    $"Claimant Event Description should not include any mention of {Constants.ClaimantEventDescription.TempFenceRequired} " +
                    $"as no Fence damage should be involved in a '{claimData.DamagedCovers.GetDescription()}' claim.");
            }

            if (claimData.DamagedCovers == AffectedCovers.BuildingOnly
                || claimData.DamagedCovers == AffectedCovers.BuildingAndContents
                || claimData.DamagedCovers == AffectedCovers.BuildingAndFence
                || claimData.DamagedCovers == AffectedCovers.BuildingAndContentsAndFence)
            {
                VerifyWaterDamageSelectionsInEventDescription(claimData.StormWaterDamageCheckboxes, claimantEventDescription);
                // TODO: INSU-818: Once gone live, we can remove the toggle and treat as always true.
                if (Config.Get().IsClaimHomeMoreAboutYourDamageScreenEnabled())
                {
                    Reporting.IsTrue(DataHelper.StripLineFeedAndCarriageReturns(claimantEventDescription,false).Contains(DataHelper.StripLineFeedAndCarriageReturns(claimData.AccountOfEvent,false)),
                        $"Claimant Event Description should include mention of 'More about your damage' as '{claimData.AccountOfEvent}' ");
                }                 
            }    
        }

        /// <summary>
        /// For each checkbox selected on the Water Damage screen (if it was included in the claim flow), we should see 
        /// a corresponding line in the Claimant Event Description.
        /// If the Water Damage screen was not presented (e.g. Contents only or Fence only claims) then this will not be relevant and 
        /// should not be invoked.
        /// </summary>
        /// <param name="expectedStormWaterDmgCheckboxes">The test data as to which checkboxes should have been selected</param>
        /// <param name="claimantEventDescription">The full text of the Claimant Event Description pulled from the Shield Database</param>
        public static void VerifyWaterDamageSelectionsInEventDescription(StormWaterDamageCheckboxes expectedStormWaterDmgCheckboxes, string claimantEventDescription)
        {
            VerifyClaimantEventDescriptionContains(expectedStormWaterDmgCheckboxes.NoWaterDamage, 
                StormWaterDamageCheckboxesOptions.NoWaterDamage, claimantEventDescription);

            VerifyClaimantEventDescriptionContains(expectedStormWaterDmgCheckboxes.DampPatchesOrDripping,
                StormWaterDamageCheckboxesOptions.DampPatchesOrDripping, claimantEventDescription);

            VerifyClaimantEventDescriptionContains(expectedStormWaterDmgCheckboxes.SolidTimberFloorIsWet,
                StormWaterDamageCheckboxesOptions.SolidTimberFloorIsWet, claimantEventDescription);

            VerifyClaimantEventDescriptionContains(expectedStormWaterDmgCheckboxes.BadlySoakedCarpets,
                StormWaterDamageCheckboxesOptions.BadlySoakedCarpets, claimantEventDescription);

            VerifyClaimantEventDescriptionContains(expectedStormWaterDmgCheckboxes.HouseIsFlooded,
                StormWaterDamageCheckboxesOptions.HouseIsFlooded, claimantEventDescription);

            VerifyClaimantEventDescriptionContains(expectedStormWaterDmgCheckboxes.SewageOrDrainWaterInTheHouse,
                StormWaterDamageCheckboxesOptions.SewageOrDrainWaterInTheHouse, claimantEventDescription);

            VerifyClaimantEventDescriptionContains(expectedStormWaterDmgCheckboxes.WaterInTheElectrics,
                StormWaterDamageCheckboxesOptions.WaterInTheElectrics, claimantEventDescription);

            VerifyClaimantEventDescriptionContains(expectedStormWaterDmgCheckboxes.OtherWaterDamage,
                StormWaterDamageCheckboxesOptions.OtherWaterDamage, claimantEventDescription);
        }

        private static void VerifyClaimantEventDescriptionContains(bool isExpected, StormWaterDamageCheckboxesOptions waterDamageOption, string eventDescription)
        {
            Reporting.AreEqual(isExpected, eventDescription.Contains(waterDamageOption.GetDescription()),
                  $"the presence of '{waterDamageOption.GetDescription()}' in Claimant Event Description is expected " +
                  $"to be evaluated as {isExpected}");
        }
    }
}

