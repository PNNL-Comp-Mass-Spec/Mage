using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MageFilePackager
{
    internal static class DMSHttp
    {

        public enum HTTPMethod
        {
            HTTPGet = 0,
            HTTPPost = 1
        }

        /// <summary>
        /// Post web service request and receive response 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataList"></param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> postDataList)
        {

            // format the data to be posted
            var postData = FormatPostData(postDataList);

            // Prepare Request Object
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Credentials = CredentialCache.DefaultCredentials;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;

            // Send Request
            StreamWriter sw = null;
            StreamReader sr = null;
            String responseData;
            try
            {
                sw = new StreamWriter(request.GetRequestStream());
                sw.Write(postData);
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }

            // Receive Response
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                sr = new StreamReader(response.GetResponseStream());
                responseData = sr.ReadToEnd();
            }
            catch (WebException wex)
            {
                sr = new StreamReader(wex.Response.GetResponseStream());
                responseData = sr.ReadToEnd();
                throw new Exception(responseData);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
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
            foreach (KeyValuePair<string, string> item in postDataList)
            {
                sb.Append(string.Format("&{0}={1}", item.Key, item.Value));
            }
            return sb.ToString();
        }

    }
}
