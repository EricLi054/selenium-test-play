using System;

namespace Rac.TestAutomation.Common.Exceptions
{
    [Serializable]
    public class ShieldApiException : Exception
    {
        public ShieldApiException(string message) : base(message) {}

        public ShieldApiException(string message, Exception inner) : base(message, inner) { }
    }
}