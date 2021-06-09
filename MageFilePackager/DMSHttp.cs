using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MageFilePackager
{
    internal static class DMSHttp
    {
        // Ignore Spelling: urlencoded, www

        /// <summary>
        /// Post web service request and receive response
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataList"></param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> postDataList)
        {
            // Format the data to be posted
            var postData = FormatPostData(postDataList);

            // Prepare Request Object
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Credentials = CredentialCache.DefaultCredentials;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;

            // Send Request
            string responseData;

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(postData);
            }

            // Receive Response
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();

                if (responseStream == null)
                {
                    responseData = string.Empty;
                }
                else
                {
                    using var reader = new StreamReader(responseStream);
                    responseData = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                var responseStream = ex.Response.GetResponseStream();
                if (responseStream == null)
                {
                    responseData = string.Empty;
                }
                else
                {
                    using var reader = new StreamReader(responseStream);
                    responseData = reader.ReadToEnd();
                }
            }

            return responseData;
        }

        /// <summary>
        /// Convert set of Key/Value pairs to POST data string
        /// </summary>
        /// <param name="postDataList"></param>
        /// <returns></returns>
        private static string FormatPostData(Dictionary<string, string> postDataList)
        {
            var sb = new StringBuilder();
            foreach (var item in postDataList)
            {
                sb.AppendFormat("&{0}={1}", item.Key, item.Value);
            }
            return sb.ToString();
        }
    }
}
