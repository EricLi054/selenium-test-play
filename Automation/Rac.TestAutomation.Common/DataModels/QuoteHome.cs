using System;
using System.Collections.Generic;
using System.Text;

using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace Rac.TestAutomation.Common
{
    public class QuoteHome : Home
    {
        public HomePreviousInsuranceTime PreviousInsuranceTime { get; set; }
        public string CurrentInsurer { get; set; }
        public List<Contact> PolicyHolders { get; set; }
        public Payment PayMethod { get; set; }
        public DateTime StartDate { get; set; }
        public HomeOccupancy Occupancy { get; set; }
        public bool IsUsedForShortStay { get; set; }
        /// <summary>
        /// Sets answer for question of whether property is
        /// being renovated, has multiple unrelated occupants
        /// or being used for a business other than home office.
        /// </summary>
        public bool IsHomeUsageUnacceptable { get; set; }
        public int WeeklyRental { get; set; }
        public HomePropertyManager PropertyManager { get; set; }
        /// <summary>
        /// Relates to disclosure whether any policyholder has
        /// been previously convicted of theft, drug, fraud, or
        /// criminal damage.
        /// 
        /// If TRUE, Test framework will answer yes to verify
        /// decline pop-up, but then it will answer no to
        /// allow policy purchase to proceeed.
        /// </summary>
        public bool HasPastConvictions { get; set; }
        public List<ClaimHistory> PastClaims { get; set; }
        /// <summary>
        /// The desired excess for the requested policy.
        /// If a null value, then just used the Shield provided default.
        /// </summary>
        public string ExcessBuilding { get; set; }
        public string ExcessContents { get; set; }
        public bool AddAccidentalDamage { get; set; }

        public override string ToString()
        {
            int buildingSI = BuildingValue.HasValue ? BuildingValue.Value : 0;
            int contentsSI = ContentsValue.HasValue ? ContentsValue.Value : 0;
            string buildingExcess = string.IsNullOrEmpty(ExcessBuilding) ? "default" : ExcessBuilding;
            string contentsExcess = string.IsNullOrEmpty(ExcessContents) ? "default" : ExcessContents;

            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(string.Empty);
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Home quote data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Address:      {PropertyAddress.StreetSuburbPostcode(expandUnitAddresses: false)}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"    Occupancy:    {Occupancy}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Building SI:   {buildingSI}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"   Contents SI:   {contentsSI}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($" Building Excess: {buildingExcess}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($" Contents Excess: {contentsExcess}{Reporting.HTML_NEWLINE}");
            formattedString.Append($"--- Policy holders:{Reporting.HTML_NEWLINE}");
            foreach (var policyholder in PolicyHolders)
            {
                formattedString.Append(policyholder.ToString());
            }
            return formattedString.ToString();
        }

        /// <summary>
        /// Derives the request home cover from the values of
        /// building and contents sum insured values.
        /// </summary>
        /// <returns></returns>
        public string GetRequestedCoverAsString()
        {
            var derivedCover = HomeCover.BuildingAndContents;
            if (!BuildingValue.HasValue)
                derivedCover = HomeCover.ContentsOnly;
            else if (!ContentsValue.HasValue)
                derivedCover = HomeCover.BuildingOnly;

            return derivedCover.GetDescription();
        }

        /// <summary>
        /// For properties with contents cover, if the minimum security is not
        /// met (locks on windows and doors) then Contents SI not exceed $200k
        /// </summary>
        /// <returns></returns>
        public bool IsContentsAboveSecurityLimit() => !SecurityDoorsSecured &&
                                                      !SecurityWindowsSecured &&
                                                       ContentsValue.HasValue && 
                                                       ContentsValue.Value > HOME_CONTENTS_SI_MAX_NO_SECURITY;

        public bool HasContentsCover => GetRequestedCoverAsString() == HomeCover.BuildingAndContents.GetDescription() || GetRequestedCoverAsString() == HomeCover.ContentsOnly.GetDescription();
        public bool HasContentsOnlyCover => GetRequestedCoverAsString() == HomeCover.ContentsOnly.GetDescription();
        public bool HasBuildingAndContentsCover => GetRequestedCoverAsString() == HomeCover.BuildingAndContents.GetDescription();

        public bool IsEligibleForAccidentalDamage => Occupancy == HomeOccupancy.OwnerOccupied && HasContentsCover;

        /// <summary>
        /// Flag to be set during code execution if the quote is retrieved via B2C 
        /// as the field for Policyholder 1 to select "Is the policyholder’s mailing 
        /// address the same as the insured property address?" (which is offered for
        /// Home quotes) is not presented for retrieved Quotes.
        /// </summary>
        public bool QuoteHasBeenRetrieved { get; set; }
    }

    public class ClaimHistory
    {
        public ClaimsHistory ClaimType { get; set; }
        public int Year { get; set; }
    }
}
