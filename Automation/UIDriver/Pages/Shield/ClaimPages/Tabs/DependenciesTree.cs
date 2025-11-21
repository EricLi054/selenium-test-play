using Rac.TestAutomation.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using static Rac.TestAutomation.Common.Constants.General;
using static Rac.TestAutomation.Common.Constants.ClaimsHome;

namespace UIDriver.Pages.Shield
{
    /// <summary>
    /// It is intended that this class is not used directly by tests,
    /// but instead is invoked indirectly via the reference in
    /// ShieldClaimDetailsPage.cs
    /// </summary>
    public class DependenciesTree : BaseShieldPage
    {
        private class Constants
        {
            public const string ServiceProviderSuffix = " - Motor Repairers";
            public const string TotallossSpecialistSuffix = " - Total Loss Specialist";
        }

        public enum Action
        {
            Select,
            Edit,
            View
        }

        private class XPath
        {
            public class Text
            {
                public const string ClaimHandler = Input.ClaimHandler + "//span[@id='select2-chosen-6']";
            }
            public class Input
            {
                public const string ClaimHandler = "id('s2id_handlerCompanyTreeId')";
            }
            public class Dropdown
            {
                public const string ClaimHandlerOptions = "id('select2-drop')//li/div";
            }
            public class DependencyTree
            {
                public const string Table = "id('t_idit-grid-table-flattendListclaimTree_pipe_')";
                public class Button
                {
                    public const string Edit = "id('flattendListclaimTree|Update')";
                    public const string View = "id('flattendListclaimTree|View')";
                    public const string Delete = "id('flattendListclaimTree|Delete')";
                    public const string Payment = "id('flattendListclaimTree|createPayment')";
                    public const string ManualActions = "id('flattendListclaimTree|createManualEventForClaimTreeElememt')";
                    public const string AssignServiceProvider = "id('flattendListclaimTree|addClaimantAssetReferral')";
                    public const string More = Table + "//i[@class='fa fa-angle-double-right']";
                }
                public class Row
                {
                    private const string DescriptionCellIditCode = "@aria-describedby='idit-grid-table-flattendListclaimTree_pipe__innerVO@presentationValue'";
                    public const string Table = "id('idit-grid-table-flattendListclaimTree_pipe_')";

                    public class Policyholder
                    {
                        public const string Claimant = Table + "//tr[contains(@class,'level1')]/td[" + DescriptionCellIditCode + " and contains(@title,'- Claimant')]/following-sibling::td[text()='Policyholder']";
                        public const string Asset    = Claimant + "/../following-sibling::tr[contains(@class,'level2')]/td[contains(@title,'.00')]";
                        public const string Scenario = Claimant + "/../following-sibling::tr[contains(@class,'level3')]";
                        public const string ServiceProvider = Claimant + "/../following-sibling::tr[contains(@class,'level2')]/td[" + DescriptionCellIditCode + " and contains(@title,'" + Constants.ServiceProviderSuffix + "')]";
                        public const string TotalLossSpecialist = Claimant + "/../following-sibling::tr[contains(@class,'level2')]/td[" + DescriptionCellIditCode + " and contains(@title,'" + Constants.TotallossSpecialistSuffix + "')]";
                    }
                    public class ThirdParty
                    {
                        public const string Claimant = Table + "//tr[contains(@class,'level1')]/td[" + DescriptionCellIditCode + " and contains(@title,'- Claimant')]/following-sibling::td[text()='Third party']";
                        public const string Asset    = Claimant + "/../following-sibling::tr[contains(@class,'level2')]/td[contains(@title,'.00')]";
                        public const string Scenario = Claimant + "/../following-sibling::tr[contains(@class,'level3')]";
                        public const string ServiceProvider = Claimant + "/../following-sibling::tr[contains(@class,'level2')]/td[" + DescriptionCellIditCode + " and contains(@title,'" + Constants.ServiceProviderSuffix + "')]";
                    }
                }
            }
            public class DamagedItems
            {
                public class Button
                {
                    public const string Add = Items.Table + "//a[@id='IDITForm@policyDamagedItemsList|New']";
                }
                public class Items
                {
                    public const string Table = "id('idit-grid-IDITForm_at_policyDamagedItemsList_pipe_')";
                    public const string Asset = "//label[@for='assetItemsList|1@isSelected']";
                }
            }
            public class Popup
            {
                public const string Basic = "//div[@aria-describedby='BasicNotificationDialog']";
                public class Button
                {
                    public const string OK = Basic + "//button[@id='DialogOK']";
                }
            }
        }

        public bool IsTotalLossSpecialistAssigned => _driver.TryFindElement(By.XPath(XPath.DependencyTree.Row.Policyholder.TotalLossSpecialist), out IWebElement row);
        public bool IsServiceProviderAssignedForPH => _driver.TryFindElement(By.XPath(XPath.DependencyTree.Row.Policyholder.ServiceProvider), out IWebElement row);
        public List<string> PHServiceProviderName
        {
            get
            {
                var listOfAssignedServiceProviderNames = new List<string>();
                var serviceProviderEntries = _driver.FindElements(By.XPath(XPath.DependencyTree.Row.Policyholder.ServiceProvider));
                foreach(var provider in serviceProviderEntries)
                {
                    listOfAssignedServiceProviderNames.Add(provider.GetAttribute("title").Replace(Constants.ServiceProviderSuffix, string.Empty));
                }
                
                return listOfAssignedServiceProviderNames;
            }
        }

        public List<string> PHTotalLossSpecialistName
        {
            get
            {
                var listOfAssignedTotalLossSpecialists = new List<string>();
                var totalLossSpecialistEntries = _driver.FindElements(By.XPath(XPath.DependencyTree.Row.Policyholder.TotalLossSpecialist));
                foreach (var provider in totalLossSpecialistEntries)
                {
                    listOfAssignedTotalLossSpecialists.Add(provider.GetAttribute("title").Replace(Constants.TotallossSpecialistSuffix, string.Empty));
                }

                return listOfAssignedTotalLossSpecialists;
            }
        }

        public string ClaimHandler
        {
            get => GetInnerText(XPath.Text.ClaimHandler);
            set => WaitForSelectableAndPickFromDropdown(XPath.Input.ClaimHandler, XPath.Dropdown.ClaimHandlerOptions, value);
        }

        public DependenciesTree(Browser browser) : base(browser) {}

        public override bool IsDisplayed()
        {
            return true;
        }

        /// <summary>
        /// Select the "Payment" button from the top of the table.
        /// </summary>
        public void ClickPayment() => ClickControl(XPath.DependencyTree.Button.Payment);

        /// <summary>
        /// Clicks ManualActions button if visible, else clicks More to reveal it.
        /// Throws exception if ManualActions is still not visible after clicking More.
        /// Directly clicks ManualActions if already displayed.
        /// </summary>
        public void ClickManualAction()
        {
            if (!IsControlDisplayed(XPath.DependencyTree.Button.ManualActions))
            {
                ClickControl(XPath.DependencyTree.Button.More);
                ClickControl(XPath.DependencyTree.Button.ManualActions);
            }
            else
            {
                ClickControl(XPath.DependencyTree.Button.ManualActions);
            }  
        }       

        /// <summary>
        /// Select the "Assign Service Provider" button from the top of the table.
        /// </summary>
        public void ClickAssignServiceProvider() => ClickControl(XPath.DependencyTree.Button.AssignServiceProvider);

        /// <summary>
        /// Select the claimant directly from Dependencies table and
        /// open for either edit or read only.
        /// Allows for checking NCB and excess status for the claimant
        /// </summary>
        /// <param name="editMode"></param>
        public void ActOnPolicyholderRow(Action action)
        {
            ClickControl(XPath.DependencyTree.Row.Policyholder.Claimant);
            PerformActionOnSelectedDependencyTreeRow(action);
        }

        /// <summary>
        /// Select the claimant's asset from the dependencies tree.
        /// It will open into the Claim Asset detail page
        /// </summary>
        public void ActOnPolicyholdersInsuredAsset(Action action)
        {
            ClickControl(XPath.DependencyTree.Row.Policyholder.Asset);
            PerformActionOnSelectedDependencyTreeRow(action);
        }

        /// <summary>
        /// Select the claim damage scenario (under the policyholder's asset) 
        /// directly from Dependencies table and open for either edit or
        /// read only.
        /// </summary>
        public void ActOnPolicyholdersDamageScenario(Action action)
        {
            ClickControl(XPath.DependencyTree.Row.Policyholder.Scenario);
            PerformActionOnSelectedDependencyTreeRow(action);
        }

        /// <summary>
        /// Select the third party directly from Dependencies table and
        /// open for either edit or read only.
        /// Allows for checking NCB and excess status for the claimant
        /// </summary>
        public void ActOnThirdPartyRow(Action action)
        {
            ClickControl(XPath.DependencyTree.Row.ThirdParty.Claimant);
            PerformActionOnSelectedDependencyTreeRow(action);
        }

        /// <summary>
        /// Select the third party's asset from the dependencies tree.
        /// It will open into the Claim Asset detail page
        /// </summary>
        public void ActOnThirdPartyAsset(Action action)
        {
            ClickControl(XPath.DependencyTree.Row.ThirdParty.Asset);
            PerformActionOnSelectedDependencyTreeRow(action);
        }

        /// <summary>
        /// Select the claim damage scenario (under the third party's assert) 
        /// directly from Dependencies table and open for either edit or
        /// read only.
        /// </summary>
        /// <param name="editMode"></param>
        public void ActOnThirdPartyDamageScenario(Action action)
        {
            ClickControl(XPath.DependencyTree.Row.ThirdParty.Scenario);
            PerformActionOnSelectedDependencyTreeRow(action);
        }

        /// <summary>
        /// Removes the current assigned service provider, but returns
        /// the provider's name for caller's reference.
        /// </summary>
        /// <returns></returns>
        public string RemoveExistingServiceProvider()
        {
            string originalRepairer = null;

            if (IsServiceProviderAssignedForPH)
            {
                if (PHServiceProviderName.Count > 1)
                {
                    Reporting.Error("We're getting multiple service providers for a claimant at a point in the scenario when this is not expected.");
                }

                originalRepairer = PHServiceProviderName[0]; // There should be just one and we'll remove it

                ClickControl(XPath.DependencyTree.Row.Policyholder.ServiceProvider);
                ClickControl(XPath.DependencyTree.Button.Delete);

                DismissDeleteConfirmationDialog();

                // Brief sleep as Shield UI can lag while it processes operations.
                Thread.Sleep(1000);
                WaitForUpdateClaimMode();
            }

            return originalRepairer;
        }

        /// <summary>
        /// Click the "+" (Add) option under Damaged Items From Policy section in "Damages" tab
        /// </summary>
        public void ClickAddDamagedItemsFromPolicy()
        {
            ClickControl(XPath.DamagedItems.Button.Add);
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Select the specified contents/personal valuables
        /// to add to the claim.
        /// </summary>
        public void SelectDamagedItem()
        {
            ClickControl(XPath.DamagedItems.Items.Asset);
        }

        /// <summary>
        /// As there can be a variety of covers on a policy (e.g. in Home), this
        /// method returns the list of strings representing the different damage
        /// covers on the policy that relate to the conditions of the claim.
        /// </summary>
        /// <param name="damageType"></param>
        /// <returns>List of damage covers. Empty list if nothing found.</returns>
        public List<string> GetListOfDamageCoversForProperty(HomeClaimDamageType damageType)
        {
            var rowNames = new List<string>();

            var damageTypeText = HomeClaimDamageTypeNames[damageType].TextShield;
            var rowElements = _driver.FindElements(By.XPath($"{XPath.DependencyTree.Row.Table}//td[contains(@title,'{damageTypeText}')]"));
            foreach (var rowElement in rowElements)
            {
                rowNames.Add(rowElement.GetAttribute("title"));
            }
            return rowNames;
        }

        /// <summary>
        /// Under the Dependencies Tree, for the impacted asset, there is a row
        /// for the damage type for the claim. This method will select that row
        /// and click the "edit" control. (equivalent to double clicking that
        /// row).
        /// </summary>
        /// <param name="damageCoverToEdit">Must match the complete damage cover text</param>
        public void EditAssetDamageTypeByName(string damageCoverToEdit)
        {
            // Click cover to select.
            ClickControl($"{XPath.DependencyTree.Row.Table}//td[@title='{damageCoverToEdit}']");
            // Click to edit cover and add liability
            ClickControl(XPath.DependencyTree.Button.Edit);
        }

        private void PerformActionOnSelectedDependencyTreeRow(Action action)
        {
            switch(action)
            {
                case Action.Edit:
                    ClickControl(XPath.DependencyTree.Button.Edit);
                    break;
                case Action.View:
                    ClickControl(XPath.DependencyTree.Button.View);
                    break;
                case Action.Select:
                    // Do nothing
                    break;
                default:
                    throw new NotImplementedException("Requesting unsupported action on Shield Claim Dependency tree.");
            }
        }

        public void WaitForUpdateClaimMode(int waitTime = WaitTimes.T30SEC)
        {
            var endTime = DateTime.Now.AddSeconds(waitTime);
            var success = false;

            do
            {
                try
                {
                    GetElement(XPath.DependencyTree.Row.Table);
                    GetElement(XPath.DependencyTree.Button.Delete); // This is only visible in Update Claim
                    GetElement(XPath.Input.ClaimHandler); // This is only visible in Update Claim
                    success = true;
                    break;
                }
                catch (NoSuchElementException)
                {
                    Thread.Sleep(1000);
                }
            } while (DateTime.Now < endTime);

            if (!success)
            {
                Reporting.Error("Claim did not appear to go into expected edit mode with Dependencies tab.");
            }
        }

        /// <summary>
        /// The Shield dialog used for this prompt is odd in that clicks sent
        /// to it sometimes don't get consumed, and so the user or automation
        /// need to retry clicking a button to dismiss.
        /// </summary>
        private void DismissDeleteConfirmationDialog()
        {
            var endTime = DateTime.Now.AddSeconds(WaitTimes.T30SEC);
            do
            {
                if (_driver.TryWaitForElementToBeVisible(By.XPath(XPath.Popup.Basic), WaitTimes.T5SEC, out IWebElement dialogFrame))
                {
                    ClickControl(XPath.Popup.Button.OK);
                }
                else
                {
                    // Dialog is cleared.
                    break;
                }

                Thread.Sleep(1000);

            } while (DateTime.Now < endTime);
        }
    }
}
