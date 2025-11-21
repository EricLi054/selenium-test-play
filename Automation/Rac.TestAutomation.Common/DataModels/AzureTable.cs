using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rac.TestAutomation.Common.Constants.PolicyMotor;

namespace Rac.TestAutomation.Common
{
    public class MemberRefundEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag   ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
        public string ContactId { get; set; }
        public string ExternalContactNumber { get; set; }
        public string EventDescription { get; set; }
        public string FileName { get; set; }
        public double RefundAmount { get; set; }
    }

    public class MotorPolicyEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string PolicyNumber { get; set; }
        public string CoverType { get; set; }
        public bool IsEV { get; set; }
        public bool IsRegistrationValid { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
        public MotorCovers GetCoverType()
        {
            return MotorCoverIdMappings.FirstOrDefault(x => x.Value == CoverType).Key;
        }
    }
}
