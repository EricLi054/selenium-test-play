using NUnit.Framework;
using Rac.TestAutomation.Common;
using Rac.TestAutomation.Common.DatabaseCalls.Claims;
using static Rac.TestAutomation.Common.AzureStorage.AzureTableOperation;

namespace Maintenance
{
    public class AzureTestDataSetup : BaseUITest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ExtentTestManager.CreateParentTest(this.GetType().Name, "Schedule batch job for storing test data in azure table");
        }

        [Test, Category("TestDataSetup")]
        public void LoadMotorPolicyForClaimTestDataInAzureTable()
        {
            var motorPolicies = ShieldMotorClaimDB.GetMotorPoliciesForClaim();
            var tableName = FormTableName(DataType.Policy, DataPurpose.ForClaim, ProductType.Motor);
            DataHelper.AzureTableDeleteAndAddNewEntity(Config.Get().Azure.StorageMotorClaims, tableName, motorPolicies);
        }


        #region DataSetupHelper
        public string FormTableName(DataType dataType, DataPurpose dataPurpose, ProductType productType)
        {
            var environment = Config.Get().Shield.Environment;
            return $"{environment}{dataType}{dataPurpose}{productType}";
    }
        #endregion
    }
}
