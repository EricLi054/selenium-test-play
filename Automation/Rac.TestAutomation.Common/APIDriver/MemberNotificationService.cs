using AventStack.ExtentReports.Configuration;
using Newtonsoft.Json;
using NUnit.Framework.Internal;
using Rac.TestAutomation.Common.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common.APIDriver
{
    
    public class MemberNotificationService : BaseAPI
    {
        private static class Header
        {
            public const string CorrelationID = "correlationId";
            public const string ApiKey = "Ocp-Apim-Subscription-Key";
            public const string Source = "sourceSystem";
            public const string Host = "Host";
        }


        public static MemberNotificationService Instance { get; private set; }

        private Dictionary<string, string> _apiHeaders;
        private static readonly object _lock = new object();

        private static class Paths
        {
            public const string Events = "member-events/api/v1/events";
        }


        public MemberNotificationService() : base()
        {
            var config = Config.Get();
            _baseUrl = string.Format($"https://az-api-{config.Azure.MemberNotificationService.APIEnv}.ractest.com.au/");

            _apiHeaders = new Dictionary<string, string>()
            {
                { Header.CorrelationID, $"mns{DateTime.Now.ToString("s")}" },
                { Header.Source, "IntegrationTests" },
                { Header.ApiKey, config.Azure.MemberNotificationService.APIKey },
                { Header.Host, $"az-api-{config.Azure.MemberNotificationService.APIEnv}.ractest.com.au" }               
            };

           
        }

        public static MemberNotificationService GetInstance()
        {
            if (Instance == null)
            {
                lock (_lock)
                {
                    Instance = new MemberNotificationService();                   
                }
            }

            return Instance;
        }

        /// <summary>
        /// Create member event in Member Notification Service
        /// to support test scenario.
        /// </summary>
        /// <param name="memberEvent">Payload to send to MNS</param>
        /// <exception cref="NUnitException">Thrown if not successful or any error occurs.</exception>
        public async Task POST_MemberEvent(MemberEvent memberEvent)
        {
            string body = JsonConvert.SerializeObject(memberEvent);
            var createMemberEvent = await Post_Request(endpoint: Paths.Events, body: body, addedHeaders: _apiHeaders);

            if (createMemberEvent.StatusCode != HttpStatusCode.Created)
            {
                throw new NUnitException($"Failed to create event in Member Notification Service, error: {createMemberEvent.StatusCode}");
            }
        }
    }
}
