using Azure.Data.Tables;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common.AzureStorage
{
    public class AzureTableOperation
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly TableClient _tableClient;

        public AzureTableOperation(string tableName)
        {
            _tableServiceClient = new TableServiceClient(new Uri(Config.Get().Azure.StorageMotorClaims.URI), new TableSharedKeyCredential(Config.Get().Azure.StorageMotorClaims.AccountName, Config.Get().Azure.StorageMotorClaims.AccountKey));
            _tableServiceClient.CreateTableIfNotExists(tableName);
            _tableClient = _tableServiceClient.GetTableClient(tableName);
        }

        public AzureTableOperation(AzureTable storageConfig, string tableName)
        {
            _tableServiceClient = new TableServiceClient(new Uri(storageConfig.URI), new TableSharedKeyCredential(storageConfig.AccountName, storageConfig.AccountKey));           
            _tableServiceClient.CreateTableIfNotExists(tableName);
            _tableClient = _tableServiceClient.GetTableClient(tableName);
        }

        public enum DataType
        {
            Policy,
            Contact,
        }

        public enum DataPurpose
        {
            ForClaim,
            ForRenewal,
            ForEndorsement,
        }

        public enum ProductType
        {
            Home,
            Motor,
            Motorcycle,
            Caravan,
            Boat,
            Pet,
        }

        /// <summary>
        /// Get All the data from the azure storage table
        /// </summary>
        public async Task<List<T>> GetAllEntries<T>() where T : class, ITableEntity
        {
            List<T> allTableEntries = new List<T>();
            var rows = _tableClient.QueryAsync<T>();
            await foreach (var row in rows)
            {
                allTableEntries.Add(row);
            }
            return allTableEntries;
        }

        /// <summary>
        ///Delete record from the azure storage table
        /// </summary>
        /// <param name="pKey">unique partition key for record</param>
        /// <param name="rKey">unique row key for record</param>
        public void DeleteTableRow(string pKey, string rKey)
        {
            _tableClient.DeleteEntity(pKey, rKey);
        }

        /// <summary>
        /// Delete policy record from the azure table, usually because we have selected it for use in a test.
        /// This method is specific to Azure tables with the "MotorPolicyEntity".
        /// </summary>
        /// <param name="policyNumber">The policy number to find and erase. If not found, then just returns.</param>
        public void DeleteEntityBasedOnThePolicyNumber(string policyNumber)
        {
            var policyRow = _tableClient.Query<MotorPolicyEntity>(x => x.PolicyNumber == policyNumber);
            foreach (var policy in policyRow)
            {
                DeleteTableRow(policy.PartitionKey, policy.RowKey);
            }            
        }

        /// <summary>
        /// Delete all existing records and
        /// then add new records in the azure storage table
        /// </summary>
        public void DeleteAndAddNewEntityList<T>(List<T> newTableData) where T : class, ITableEntity
        {
            var rows = Task.Run(() => GetAllEntries<TableEntity>()).GetAwaiter().GetResult();
            foreach (var row in rows)
            {
                DeleteTableRow(row.PartitionKey, row.RowKey);
            }
            foreach (var newItem in newTableData)
            {
                _tableClient.AddEntity(newItem);
            }
        }

        public void AddEntity<T>(T newRow) where T : ITableEntity
        {
            _tableClient.AddEntity(newRow);
        }
    }
}
