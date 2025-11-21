using Newtonsoft.Json;
using System.Collections.Generic;

namespace Rac.TestAutomation.Common.API
{
    /// <summary>
    /// JSON Web Token (JWT) as a string
    /// </summary>
    public class STS_Token
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }

    /// <summary>
    /// Contains the details to included in the
    /// JSON web token.
    /// </summary>
    public class STS_TokenParameters
    {
        [JsonProperty("TokenData")]
        public STS_TokenData TokenData { get; set; }
        [JsonProperty("ExpiryDays")]
        public string ExpiryDays { get; set; }
    }

    /// <summary>
    /// Secure Token Service (STS) controls tokens which are then used  
    /// to give to members to update claim details
    /// </summary>
    public class STS_TokenData
    {
        [JsonProperty("Data")]
        public STS_TokenIdentifiers Data { get; set; }
    }

    /// <summary>
    /// Provide ContactId, ClaimId and array of Transaction Numbers
    /// to be include in the create token request
    /// </summary>
    public class STS_TokenIdentifiers
    {
        [JsonProperty("ContactId")]
        public string ContactId { get; set; }
        [JsonProperty("ClaimId")]
        public string ClaimId { get; set; }
        [JsonProperty("externalContactNumber")]
        public string ExternalContactNumber { get; set; }
        [JsonProperty("ExternalClaimTransactionNumbers", Required = Required.DisallowNull)]
        public List<string> ExternalClaimTranactionNumbers { get; set; }
    }

    /// <summary>
    /// Contact, claim and transaction numbers in a comma separated string.
    /// </summary>
    public class STS_TokenResponseIdentifiers
    {
        [JsonProperty("contact")]
        public string ContactId { get; set; }
        [JsonProperty("claim")]
        public string ClaimId { get; set; }
        [JsonProperty("externalClaimTransactionNumbers")]
        public string ExternalClaimTranactionNumbers { get; set; }
    }

    /// <summary>
    /// Accesing information on what went wrong when validating a token
    /// </summary>
    public class STS_ErrorDetails
    {
        [JsonProperty("statusCode")]
        public string StatusCode { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }


}
