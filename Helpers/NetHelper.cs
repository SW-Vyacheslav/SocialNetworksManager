using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

namespace Helpers
{
    public static class NetHelper
    {
        public static Dictionary<String, String> GetUriFields(Uri uri)
        {
            Dictionary<String, String> fields = new Dictionary<String, String>();

            Regex uriRegex = new Regex("([?&][^=]+=[^&]+)");

            MatchCollection matches = uriRegex.Matches(uri.ToString());

            foreach (Match item in matches)
            {
                String matchValue = item.Value;
                String key = matchValue.Substring(1, matchValue.IndexOf('=') - 1);
                String value = matchValue.Substring(matchValue.IndexOf('=') + 1);
                fields.Add(key, value);
            }

            return fields;
        }

        public static String PostRequest(Uri link, String post_data, out Uri response_uri)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(link);
            HttpWebResponse webResponse = null;
            String responseString = null;

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = Encoding.UTF8.GetBytes(post_data).Length;

            using (StreamWriter writer = new StreamWriter(webRequest.GetRequestStream()))
            {
                writer.Write(post_data);
            }

            webResponse = (HttpWebResponse)webRequest.GetResponse();

            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
            {
                responseString = reader.ReadToEnd();
            }

            response_uri = webResponse.ResponseUri;

            return responseString;
        }

        public static String PostRequest(Uri link, String post_data)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(link);
            HttpWebResponse webResponse = null;
            String responseString = null;

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = Encoding.UTF8.GetBytes(post_data).Length;

            using (StreamWriter writer = new StreamWriter(webRequest.GetRequestStream()))
            {
                writer.Write(post_data);
            }

            webResponse = (HttpWebResponse)webRequest.GetResponse();

            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
            {
                responseString = reader.ReadToEnd();
            }

            return responseString;
        }

        public static String GetRequest(Uri link, out Uri response_uri)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(link);
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            String responseString = null;

            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
            {
                responseString = reader.ReadToEnd();
            }

            response_uri = webResponse.ResponseUri;

            return responseString;
        }

        public static String GetRequest(Uri link)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(link);
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            String responseString = null;

            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
            {
                responseString = reader.ReadToEnd();
            }

            return responseString;
        }

        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        public extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
    }
}
