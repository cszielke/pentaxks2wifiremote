using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace PtxK_S2
{
    public class http
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool useDefaultProxy = false;

        public void HttpSend(string strUrl, string Methode, string PostContent, out string strContent, out string strErrCode)
        {
            strErrCode = "";
            StringBuilder strBuildContent = new StringBuilder();
            WebRequest WebReq = WebRequest.Create(strUrl);

            if (useDefaultProxy)
            {
                WebReq.Proxy = WebRequest.DefaultWebProxy;
                WebReq.Credentials = CredentialCache.DefaultCredentials;
            }
            else
            {
                WebRequest.DefaultWebProxy = null;
                WebReq.Proxy = System.Net.WebRequest.DefaultWebProxy;
            }

            try
            {
                WebReq.Method = Methode;
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(PostContent);
                WebReq.ContentLength = byteArray.Length;
                WebReq.ContentType = "text/xml";
                WebReq.Timeout = 3000;
                WebReq.ConnectionGroupName = GetHashCode().ToString()+strUrl;

                //Bei POST und PUT Request Content setzen
                if ((Methode.ToUpper() == "POST")||(Methode.ToUpper() == "PUT"))
                {
                    Stream newStream = WebReq.GetRequestStream();
                    newStream.Write(byteArray, 0, byteArray.Length);
                    newStream.Close();
                }
                WebResponse WebResp = WebReq.GetResponse();

                StreamReader MyStrmR = new StreamReader(WebResp.GetResponseStream(), Encoding.UTF8);

                while (!MyStrmR.EndOfStream)
                {
                    //while (-1 != MyStrmR.Peek())
                    //{
                    strBuildContent.Append(MyStrmR.ReadToEnd());
                    //}
                }
            }
            catch (Exception e)
            {
                strErrCode = e.ToString();
            }
            strContent = strBuildContent.ToString();
        }

        public bool DownloadRemoteImageFile(string uri, string fileName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ConnectionGroupName = GetHashCode().ToString() + uri;
            request.Timeout = 3000;
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                return false;
            }

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {

                // if the remote file was found, download it
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(fileName))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }
                return true;
            }
            else
                return false;
        }
        
        public String GetStringInBetween(String strSource, ref int StartPos,
            String strBegin, String strEnd,
            bool includeBegin, bool includeEnd)
        {

            String ret = "";
            int iIndexOfBegin = strSource.IndexOf(strBegin, StartPos);

            if (iIndexOfBegin != -1)
            {
                // include the Begin string if desired
                StartPos = iIndexOfBegin;
                if (includeBegin)
                    iIndexOfBegin -= strBegin.Length;
                strSource = strSource.Substring(iIndexOfBegin
                    + strBegin.Length);
                int iEnd = strSource.IndexOf(strEnd);
                if (iEnd != -1)
                {
                    // include the End string if desired
                    StartPos += iEnd + strEnd.Length; //remember last Pos
                    if (includeEnd) iEnd += strEnd.Length;
                    ret = strSource.Substring(0, iEnd);
                }
            }

            return ret;
        }

        public String ReplaceStringInBetween(String strSource, String strBegin, String strEnd, String NewStr)
        {
            int pos = 0;
            String tmp = "dummy";
            while (tmp != "")
            {
                tmp = GetStringInBetween(strSource, ref pos,
                            strBegin, strEnd, true, true);
                if (tmp != "")
                {
                    strSource = strSource.Replace(tmp, NewStr);
                    pos -= tmp.Length;
                    pos += NewStr.Length;
                }
            }

            return strSource;
        }

        public String StripHtmlTags(String strContent)
        {
            Regex rexStripHtml = new Regex("<([^!>]([^>]|\n)*)>", RegexOptions.IgnoreCase);
            return rexStripHtml.Replace(strContent, "").Replace("&nbsp;", "");
        }

        public String StripWhiteSpaces(String strContent)
        {
            //Regex rexStripHtml = new Regex("<(.|\n)+?>", RegexOptions.IgnoreCase);
            Regex rexStripWhSpace = new Regex(" * ", RegexOptions.IgnoreCase);
            return rexStripWhSpace.Replace(strContent, " ");
        }

        /// <summary>
        /// JSON Serialization
        /// </summary>
        public string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return jsonString;
        }

        /// <summary>
        /// JSON Deserialization
        /// </summary>
        public T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }


    }
}
