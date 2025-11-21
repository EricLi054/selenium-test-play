using System.Text;

using static Rac.TestAutomation.Common.Constants.PolicyHome;

namespace Rac.TestAutomation.Common
{
    public class EndorseHome : EndorsementBase
    {
        public Home     NewAssetValues { get; set; }
        public string   WeeklyRent   { get; set; }
        public HomePropertyManager HomePropertyManager { get; set; }

        /// <summary>
        /// The desired building excess for the requested policy.
        /// If a null value, then just used the Shield provided default.
        /// </summary>
        public string ExcessBuilding { get; set; }

        /// <summary>
        /// The desired contents excess for the requested policy.
        /// If a null value, then just used the Shield provided default.
        /// </summary>
        public string ExcessContents { get; set; }

        public EndorseHome()
        {
            // As a key element, needs to at least be initialised.
            NewAssetValues = new Home();
        }

        /// <summary>
        /// For properties which are a cyclone risk, which was introduced Sep 2023,
        /// it is possible that we may endorse policies where these questions have
        /// not been answered and the process of the member answering causes
        /// premium changes which will affect automation expectations.
        /// </summary>
        /// <returns>TRUE indicating we'll be updating at least one unanswered question</returns>
        public bool AreWeChangingCycloneAnswers()
        {
            bool areChanging = false;

            if (OriginalPolicyData.HomeAsset.IsCycloneProneArea &&
                Config.Get().IsCycloneEnabled())
            {
                if (OriginalPolicyData.HomeAsset.GetGarageDoorUpgradeStatus !=
                    NewAssetValues.GarageDoorsCycloneStatus)
                { areChanging = true; }
                if (OriginalPolicyData.HomeAsset.GetRoofImprovementStatus !=
                    NewAssetValues.RoofImprovementCycloneStatus)
                { areChanging = true; }
                if (string.IsNullOrEmpty(OriginalPolicyData.HomeAsset.IsPropertyElevated) ||
                    string.IsNullOrEmpty(OriginalPolicyData.HomeAsset.HasCycloneShutters))
                { areChanging = true; }
            }

            return areChanging;
        }

        public override string ToString()
        {
            StringBuilder formattedString = new StringBuilder();
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            formattedString.AppendLine($"--- Endorsement data:{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"     Policy/Contact:  {PolicyNumber} / {ActivePolicyHolder.Id}{Reporting.HTML_NEWLINE}");
            formattedString.AppendLine($"     Annual Premium:  ${OriginalPolicyData.AnnualPremium.Total.ToString("0.00")}{Reporting.HTML_NEWLINE}");
            if (OriginalPolicyData.GetPaymentFrequency() != Constants.PolicyGeneral.PaymentFrequency.Annual)
            {
                formattedString.AppendLine($"     Current Instalment:  ${OriginalPolicyData.NextPendingInstallment().Amount.Total.ToString("0.00")}{Reporting.HTML_NEWLINE}");
            }
            if (NewAssetValues != null)
            {
                if (NewAssetValues.PropertyAddress != null)
                { formattedString.AppendLine($"     New Address:      {NewAssetValues.PropertyAddress.QASStreetAddress()}{Reporting.HTML_NEWLINE}"); }
                if (!string.IsNullOrEmpty(ExcessBuilding))
                { formattedString.AppendLine($"     New Building Excess:  {ExcessBuilding}{Reporting.HTML_NEWLINE}"); }
                if (NewAssetValues.BuildingValue.HasValue)
                { formattedString.AppendLine($"     Change Building SI:  {NewAssetValues.BuildingValue.Value}{Reporting.HTML_NEWLINE}"); }
                if (!string.IsNullOrEmpty(ExcessContents))
                { formattedString.AppendLine($"     New Contents Excess:  {ExcessContents}{Reporting.HTML_NEWLINE}"); }
                if (NewAssetValues.ContentsValue.HasValue)
                { formattedString.AppendLine($"     Change Contents SI:  {NewAssetValues.ContentsValue.Value}{Reporting.HTML_NEWLINE}"); }
            }
            formattedString.AppendLine(Reporting.SEPARATOR_BAR);
            return formattedString.ToString();
        }
    }
}