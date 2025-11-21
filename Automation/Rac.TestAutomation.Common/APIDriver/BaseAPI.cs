using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Rac.TestAutomation.Common
{
    public class BaseAPI
    {
        protected string _baseUrl;
        private static readonly HttpClient ApiClient = new HttpClient
        {
            // The long value of this timeout is to primarily support the parallel
            // member search operation that we do in Member Central, as it is quite
            // a slow operation.
            Timeout = TimeSpan.FromSeconds(1200)
        };

        public BaseAPI()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// Generate a POST request.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="body"></param>
        /// <param name="responseContentFormat"></param>
        /// <exception cref="Exception">Thrown if request fails.</exception>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> Post_Request(string endpoint, 
                                                   string body = null, 
                                                   string responseContentFormat = "application/json",
                                                   Dictionary<string,string> addedHeaders = null,
                                                   Dictionary<string,string> addedParams = null)
        {
            StringContent content = null;
            var url = string.Format($"{_baseUrl}{endpoint}");
            HttpResponseMessage response = null;

            if (addedParams != null && addedParams.Count > 0)
            {
                var paramString = "";
                foreach (var paramName in addedParams.Keys)
                {
                    paramString += $"&{paramName}={addedParams[paramName]}";
                }
                url += $"?{paramString.Substring(1)}";
            }            

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {               
                if (addedHeaders != null)
                {
                    foreach (var headerName in addedHeaders.Keys)
                    {
                        request.Headers.Add(headerName, addedHeaders[headerName]);
                    }
                }
                if (body != null)
                {
                    request.Headers.Add("Accept", responseContentFormat);
                    content = new StringContent(body, Encoding.UTF8, responseContentFormat);
                    request.Content = content;
                }
                response = await ApiClient.SendAsync(request);
            }

            return response;
        }

        /// <summary>
        /// Generate a PUT request.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="body"></param>
        /// <param name="responseContentFormat"></param>
        /// <exception cref="Exception">Thrown if request fails.</exception>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> Put_Request(string endpoint,
                                                   string body,
                                                   string responseContentFormat = "application/json",
                                                   Dictionary<string, string> addedHeaders = null,
                                                   Dictionary<string, string> addedParams = null)
        {
            var url = string.Format($"{_baseUrl}{endpoint}");
            HttpResponseMessage response = null;
            if (addedParams?.Any() == true)
            {
                var paramString = "";
                foreach (var paramName in addedParams.Keys)
                {
                    paramString += $"&{paramName}={addedParams[paramName]}";
                }
                url += $"?{paramString.Substring(1)}";
            }

            using (var request = new HttpRequestMessage(HttpMethod.Put, url))
            using (var content = new StringContent(body, Encoding.UTF8, responseContentFormat))
            {
                {
                    request.Headers.Add("Accept", responseContentFormat);
                    if (addedHeaders != null)
                    {
                        foreach (var headerName in addedHeaders.Keys)
                        {
                            request.Headers.Add(headerName, addedHeaders[headerName]);
                        }
                    }
                    request.Content = content;
                    response = await ApiClient.SendAsync(request);
                }
            }

            return response;
        }

        /// <summary>
        /// Generate a GET request.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="body"></param>
        /// <param name="responseContentFormat"></param>
        /// <exception cref="Exception">Thrown if request fails.</exception>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> Get_Request(string endpoint,
                                                   string responseContentFormat = "application/json",
                                                   Dictionary<string, string> addedHeaders = null,
                                                   Dictionary<string, string> addedParams = null)
        {
            var url = string.Format($"{_baseUrl}{endpoint}");
            HttpResponseMessage response = null;

            if (addedParams != null && addedParams.Count > 0)
            {
                var paramString = "";
                foreach (var paramName in addedParams.Keys)
                {
                    paramString += $"&{paramName}={addedParams[paramName]}";
                }
                url += $"?{paramString.Substring(1)}";
            }

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Add("Accept", responseContentFormat);
                if (addedHeaders != null)
                {
                    foreach (var headerName in addedHeaders.Keys)
                    {
                        request.Headers.Add(headerName, addedHeaders[headerName]);
                    }
                }
                response = await ApiClient.SendAsync(request);
            }

            return response;
        }

        /// <summary>
        /// Generate a PATCH request
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="body"></param>
        /// <param name="responseContentFormat"></param>
        /// <param name="addedHeaders"></param>
        /// <param name="addedParams"></param>
        /// <exception cref="Exception">Thrown if request fails.</exception>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> Patch_Request(string endpoint,
                                                   string body,
                                                   string responseContentFormat = "application/json",
                                                   Dictionary<string, string> addedHeaders = null,
                                                   Dictionary<string, string> addedParams = null)
        {
            var url = string.Format($"{_baseUrl}{endpoint}");
            HttpResponseMessage response = null;

            if (addedParams != null && addedParams.Count > 0)
            {
                var paramString = "";
                foreach (var paramName in addedParams.Keys)
                {
                    paramString += $"&{paramName}={addedParams[paramName]}";
                }
                url += $"?{paramString.Substring(1)}";
            }

            using (var request = new HttpRequestMessage(new HttpMethod("PATCH"), url))
            using (var content = new StringContent(body, Encoding.UTF8, responseContentFormat))
            {
                {
                    request.Headers.Add("Accept", responseContentFormat);
                    if (addedHeaders != null)
                    {
                        foreach (var headerName in addedHeaders.Keys)
                        {
                            request.Headers.Add(headerName, addedHeaders[headerName]);
                        }
                    }
                    request.Content = content;
                    response = await ApiClient.SendAsync(request);
                }
            }

            return response;
        }
    }
}
