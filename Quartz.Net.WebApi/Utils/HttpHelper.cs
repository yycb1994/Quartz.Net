using RestSharp;
using System.Net;

namespace Quartz.Net.WebApi.Utils
{
    public class HttpHelper
    {
        public static string HttpRequest(string baseUrl, Method method, RestRequest requestBody)
        {
            try
            {
                var client = new RestClient(baseUrl);
                requestBody.Method = method;
                var response = client.Execute(requestBody);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return response.Content;
                }

                throw new Exception("HTTP request result error: " + response.ErrorMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("HTTP request failed: " + ex.Message);
            }
        }
    }
}
