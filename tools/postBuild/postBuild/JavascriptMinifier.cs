using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace postBuild
{
    public static class JavaScripMinifier
    {
        private const string URL_JS_MINIFIER = "https://javascript-minifier.com/raw";
        private const string POST_PAREMETER_NAME = "input";

        public static async Task<String> MinifyJs(string inputJs)
        {
            List<KeyValuePair<String, String>> contentData = new List<KeyValuePair<String, String>>
            {
                new KeyValuePair<String, String>(POST_PAREMETER_NAME, inputJs)
            };

            using (HttpClient httpClient = new HttpClient())
            {
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(contentData))
                {
                    using (HttpResponseMessage response = await httpClient.PostAsync(URL_JS_MINIFIER, content))
                    {
                        response.EnsureSuccessStatusCode();
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }

        public static string MinifyJs2(string inputJs)
        {
            string res = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://javascript-minifier.com/raw");
            request.Method = "POST";
            string formContent = "input=" + inputJs;
            byte[] byteArray = Encoding.UTF8.GetBytes(formContent);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            Stream str = request.GetRequestStream();
            str.Write(byteArray, 0, byteArray.Length);
            str.Close();

            WebResponse response = request.GetResponse();
            str = response.GetResponseStream();
            if (str != null)
            {
                StreamReader reader = new StreamReader(str);
                res = reader.ReadToEnd();
                reader.Close();
                str.Close();
            }
            response.Close();

            return res;
        }

        public static string MinifyJs3(string inputJS) {
            
            Microsoft.Ajax.Utilities.Minifier m = new Microsoft.Ajax.Utilities.Minifier();
            string res = m.MinifyJavaScript(inputJS);
            if (m.Errors.Count > 0)
            {

                return inputJS; // no se pudo reducir, se lo deja como estaba
            }
            return res;
        }

    }
}
