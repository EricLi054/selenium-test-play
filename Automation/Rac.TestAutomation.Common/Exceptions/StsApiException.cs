using System;

namespace Rac.TestAutomation.Common.Exceptions
{
    [Serializable]
    public class StsApiException : Exception
    {
        public StsApiException(string apiStatusCode)
        {
            Reporting.Error($"Secure Token Service (STS) API did not return the expected 'OK' status code. Received: {apiStatusCode}");
        }
    }
}
