using OpenQA.Selenium;
using Rac.TestAutomation.Common;

namespace UIDriver.Pages.Spark.Endorsements
{
    /// <summary>
    /// The Policy Information Component is shared component in Spark.
    /// It provides details about a product, with some flexibility on 
    /// the exact details displayed.  Typically it will describe the 
    /// product, cover, policy number and asset information
    /// </summary>
    public class  PolicyInformationComponent : SparkBasePage
    {
        #region XPATHS
        private class XPath
        {
            public class PolicyDetailCard
            {
                public static string Root(string policyNumber) => $"id('policy-card-{policyNumber}')";
                public static string Icon(string policyNumber) => $"id('policy-card-content-product-icon-{policyNumber}')";
                public static string Title(string policyNumber) => $"id('policy-card-content-policy-details-header-title-{policyNumber}')";
                public static string Properties(string policyNumber) =>
                    $"id('policy-card-content-policy-details-properties-{policyNumber}')";
                // A zero based index for a collection of information elements about the policy
                public static string Property(string policyNumber, int index) =>
                    $"id('policy-card-content-policy-details-property-{index}-policy-number-{policyNumber}')";
                public static string Property(string policyNumber, int index, string indexDescriptor) =>
                    $"id('policy-card-content-policy-details-property-{index}-{indexDescriptor}-{policyNumber}')";
                public static string ProductDisclosureLink(string policyNumber) => $"//a[@id='policy-card-content-policy-details-link-{policyNumber}']";
                public static string CancelledRibbon(string policyNumber) => $"id('policy-card-ribbon-{policyNumber}')";
            }
        }
        #endregion


        public string PolicyDetailsCardTitle(string policyNumber) =>
            GetInnerText(XPath.PolicyDetailCard.Title(policyNumber));
        public string PolicyDetailsCardProperties(string policyNumber) =>
            GetElement(XPath.PolicyDetailCard.Properties(policyNumber) + "/div").Text;
        public string PolicyDetailsCardProperty(string policyNumber, int index) =>
            GetElement(XPath.PolicyDetailCard.Property(policyNumber, index)).Text;
        public string PolicyDetailsCardProperty(string policyNumber, int index, string indexDescriptor) =>
            GetElement(XPath.PolicyDetailCard.Property(policyNumber, index, indexDescriptor)).Text;
        public bool IsCancelledRibbonDisplayed(string policyNumber) =>
            IsControlDisplayed(XPath.PolicyDetailCard.CancelledRibbon(policyNumber));


        public PolicyInformationComponent(Browser browser) : base(browser) { }

        public override bool IsDisplayed()
        {
            // Please use IsDisplayed(policyNumber) as all components include
            // the policy number in their ID
            return true;
        }

        public bool IsDisplayed(string policyNumber)
        {
            var rendered = false;
            try
            {
                if (IsDisplayed())
                {
                    GetElement(XPath.PolicyDetailCard.Root(policyNumber));
                    GetElement(XPath.PolicyDetailCard.Icon(policyNumber));
                    GetElement(XPath.PolicyDetailCard.Title(policyNumber));
                    GetElement(XPath.PolicyDetailCard.Properties(policyNumber));
                    GetElement(XPath.PolicyDetailCard.ProductDisclosureLink(policyNumber));
                    rendered = true;
                }
            }
            catch (NoSuchElementException)
            {
                rendered = false;
            }
            return rendered;
        }
    }
}
