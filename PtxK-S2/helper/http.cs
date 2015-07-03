using System;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;

namespace PtxK_S2
{
    public static class http
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool useDefaultProxy = false;

        /// <summary>
        /// Send a Request to a webserver
        /// </summary>
        /// <param name="strUrl">URL</param>
        /// <param name="Methode">GET, POST, PUT,DELETE, etc...</param>
        /// <param name="PostContent">The Content, that will be sent to the webserver</param>
        /// <param name="strContent">The Content, that the webserver will return</param>
        /// <param name="strErrCode">The Error, that the webserver will return (or empty for no error)</param>
        public static void HttpSend(string strUrl, string Methode, string PostContent, out string strContent, out string strErrCode)
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
                WebReq.ConnectionGroupName = "HttpSend "+strUrl;

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

        /// <summary>
        /// Download a image file and save it on disk
        /// </summary>
        /// <param name="uri">Source URI</param>
        /// <param name="fileName">Filename to save to</param>
        /// <returns></returns>
        public static bool DownloadRemoteImageFile(string uri, string fileName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ConnectionGroupName = "DownloadRemoteImageFile " + uri;
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
                if (!Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(fileName))))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(fileName)));
                }
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

        /// <summary>
        /// JSON Serialization
        /// </summary>
        public static string JsonSerializer<T>(T t)
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
        public static T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }


    }
}
