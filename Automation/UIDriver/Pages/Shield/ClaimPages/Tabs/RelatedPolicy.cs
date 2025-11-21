using Rac.TestAutomation.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;

namespace UIDriver.Pages.Shield
{
    /// <summary>
    /// It is intended that this class is not used directly by tests,
    /// but instead is invoked indirectly via the reference in
    /// ShieldClaimDetailsPage.cs
    /// </summary>
    public class RelatedPolicy : BaseShieldPage
    {
        private class XPath
        {
            public class PaymentsTable
            {
                /// <summary>
                /// Will match all displayed collection dates in the table.
                /// </summary>
                public const string CollectionDate = "id('flattendListpaymentSchedulerTree|2@innerVO@installmentVO@collectionDate')";
            }
        }

        public RelatedPolicy(Browser browser) : base(browser) {}

        public override bool IsDisplayed()
        {
            return true;
        }

        /// <summary>
        /// Returns the collection date of the first entry in the payments table.
        /// </summary>
        /// <returns>Displayed date</returns>
        public string GetFirstShownCollectionDate()
        {
            return GetValue(XPath.PaymentsTable.CollectionDate);
        }
    }
}
