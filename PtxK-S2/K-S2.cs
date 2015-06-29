using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Net;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace PtxK_S2
{
    public class K_S2
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string CameraIP;
        public string LastErrorStr;
        public string Content;
        public string ErrCode;

        public parameter Parameter;
        public filelist Filelist;
        public photoinfo PhotoInfo;

        private bool simCamera;

        const string urlShoot       = "http://{0}/v1/camera/shoot"; //ip
        const string urlGetParam    = "http://{0}/v1/props"; //ip
        const string urlSetParam    = "http://{0}/v1/params/camera"; //ip
        const string urlGetFilelist = "http://{0}/v1/photos"; //ip
        const string urlGetFile     = "http://{0}/v1/photos/{1}?size={2}"; //ip,dir+/+filename,resolution thumb,view or full
        const string urlGetFileInfo = "http://{0}/v1/photos/{1}/info"; //ip,dir+/+filename
        const string urlLiveView = "http://{0}/v1/liveview";//ip

        #region constructor
        public K_S2()
        {
            K_S2_Init();
        }

        public K_S2(string ip)
        {
            K_S2_Init();

            this.CameraIP = ip;
            log.DebugFormat("CameraIP={0}", this.CameraIP);

            if (String.IsNullOrEmpty(this.CameraIP)) simCamera = true;

        }

        private void K_S2_Init()
        {
            log.Debug("K_S2_Init");

            Filelist = new filelist();
            Parameter = new parameter();
            CameraIP = "192.168.0.1";
            simCamera = false;
        }
        #endregion #region constructor

        /// <summary>
        /// Gets the filelist from K-S2
        /// </summary>
        /// <returns>success or not</returns>
        public bool GetFilelist()
        {
            bool ret = false;

            try
            {
                string url = String.Format(urlGetFilelist, CameraIP);

                PtxK_S2.http cam = new PtxK_S2.http();

                if (simCamera)
                {
                    Content = File.ReadAllText(@"simfiles\filelist.txt");
                    ErrCode = "";
                }
                else
                {
                    cam.HttpSend(url, "GET", "", out Content, out ErrCode);
                }

                if (!String.IsNullOrEmpty(ErrCode))
                {
                    throw new Exception(ErrCode);
                }
                else
                {
                    Filelist = cam.JsonDeserialize<filelist>(Content);
                }

                ret = true;
                LastErrorStr = "";
            }
            catch (Exception ex)
            {
                if(ex.InnerException == null)
                    LastErrorStr = String.Format("Error GetFileList: {0}", ex.Message);
                else
                    LastErrorStr = String.Format("Error GetFileList: {0}, InnerEx: {1}", ex.Message, ex.InnerException.Message);

                log.Error(LastErrorStr);
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// Get all Parameter from K-S2. Includes List of possible Parameters
        /// </summary>
        /// <returns>success or not</returns>
        public bool GetParameter()
        {
            bool ret = false;

            try
            {
                string url = String.Format(urlGetParam, CameraIP);

                PtxK_S2.http cam = new PtxK_S2.http();

                if (simCamera)
                {
                    Content = File.ReadAllText(@"simfiles\parameter.txt");
                    ErrCode = "";
                }
                else
                {
                    cam.HttpSend(url, "GET", "", out Content, out ErrCode);
                }

                if (!String.IsNullOrEmpty(ErrCode))
                {
                    throw new Exception(ErrCode);
                }
                else
                {
                    Parameter = cam.JsonDeserialize<parameter>(Content);
                }

                ret = true;
                LastErrorStr = "";
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    LastErrorStr = String.Format("Error GetParameter: {0}", ex.Message);
                else
                    LastErrorStr = String.Format("Error GetParameter: {0}, InnerEx: {1}", ex.Message, ex.InnerException.Message);

                log.Error(LastErrorStr);
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// Set a parameter at K-S2
        /// </summary>
        /// <param name="param">empty string for green Button or something like av=4.0, tv=1.160 etc... look at class parameter </param>
        /// <returns>success or not</returns>
        public bool SetParameter(string param ="") //Default = Green Button
        {
            bool ret = false;

            try
            {
                string url = String.Format(urlSetParam, CameraIP);
                string postContent = param;

                PtxK_S2.http cam = new PtxK_S2.http();

                if (simCamera)
                {
                    Content = File.ReadAllText(@"simfiles\setparam.txt");
                    ErrCode = "";
                }
                else
                {
                    cam.HttpSend(url, "PUT", postContent, out Content, out ErrCode);
                }

                if (!String.IsNullOrEmpty(ErrCode))
                {
                    throw new Exception(ErrCode);
                }
                else
                {
                    parameter pm = cam.JsonDeserialize<parameter>(Content);
                    SetParameterIfNotNull(pm);
                }

                ret = true;
                LastErrorStr = "";
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    LastErrorStr = String.Format("Error SetParameter: {0}", ex.Message);
                else
                    LastErrorStr = String.Format("Error SetParameter: {0}, InnerEx: {1}", ex.Message, ex.InnerException.Message);

                log.Error(LastErrorStr);
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// Set the focus on a specific Position //TODO: Not implemented yet
        /// </summary>
        /// <returns>success or not</returns>
        public bool SetFocus()
        {
            bool ret = false;

            try
            {
                ErrCode = "";

                ret = true;
                LastErrorStr = "";
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    LastErrorStr = String.Format("Error SetFocus: {0}", ex.Message);
                else
                    LastErrorStr = String.Format("Error SetFocus: {0}, InnerEx: {1}", ex.Message, ex.InnerException.Message);

                log.Error(LastErrorStr);
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// Takes a picture with the K-S2
        /// </summary>
        /// <param name="af">auto, on or off</param>
        /// <returns></returns>
        public bool Shoot(string af ="af=auto")
        {
            bool ret = false;

            try
            {
                string url = String.Format(urlShoot, CameraIP);
                string postContent = af;

                PtxK_S2.http cam = new PtxK_S2.http();

                if (simCamera)
                {
                    Content = File.ReadAllText(@"simfiles\shoot.txt");
                    ErrCode = "";
                }
                else
                {
                    cam.HttpSend(url, "POST", postContent, out Content, out ErrCode);
                }

                if (!String.IsNullOrEmpty(ErrCode))
                {
                    throw new Exception(ErrCode);
                }

                ret = true;
                LastErrorStr = "";
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    LastErrorStr = String.Format("Error Shoot: {0}", ex.Message);
                else
                    LastErrorStr = String.Format("Error Shoot: {0}, InnerEx: {1}", ex.Message, ex.InnerException.Message);

                log.Error(LastErrorStr);
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// Get an Image from K-S2
        /// </summary>
        /// <param name="dir">Dirname</param>
        /// <param name="filename">Filename</param>
        /// <param name="resolution">thumb,view or full</param>
        /// <returns></returns>
        public Bitmap GetImage(string dir, string filename, string resolution = "full")
        {
            Bitmap bmp = null;
            
            try
            {
                string filepath = dir + "/" + filename;
                string url = String.Format(urlGetFile, CameraIP, filepath, resolution);

                if (simCamera)
                {
                    bmp = new Bitmap(@"simfiles\" + resolution + "img.jpg");
                }
                else
                {
                    var request = WebRequest.Create(url);
                    request.Timeout = 3000;

                    using (var response = request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        bmp = new Bitmap(stream);
                    }
                }
                LastErrorStr = "";
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    LastErrorStr = String.Format("Error GetImage: {0}", ex.Message);
                else
                    LastErrorStr = String.Format("Error GetImage: {0}, InnerEx: {1}", ex.Message, ex.InnerException.Message);

                log.Error(LastErrorStr);
            }
            return bmp;
        }

        /// <summary>
        /// Get Info for an Image
        /// </summary>
        /// <param name="dir">Dirname</param>
        /// <param name="filename">Filename</param>
        /// <returns>Json-String with Imageinformation</returns>
        public bool GetImageInfo(string dir, string filename)
        {
            bool ret = false;

            try
            {
                string filepath = dir + "/" + filename;
                if (dir == "latest") filepath = dir;

                string url;
                url = String.Format(urlGetFileInfo, CameraIP, filepath);

                PtxK_S2.http cam = new PtxK_S2.http();

                if (simCamera)
                {
                    Content = File.ReadAllText(@"simfiles\info.txt");
                    ErrCode = "";
                }
                else
                {
                    cam.HttpSend(url, "GET", "", out Content, out ErrCode);
                }

                if (!String.IsNullOrEmpty(ErrCode))
                {
                    throw new Exception(ErrCode);
                }
                else
                {
                    PhotoInfo = cam.JsonDeserialize<photoinfo>(Content);
                }

                ret = true;
                LastErrorStr = "";
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    LastErrorStr = String.Format("Error GetImageInfo: {0}", ex.Message);
                else
                    LastErrorStr = String.Format("Error GetImageInfo: {0}, InnerEx: {1}", ex.Message, ex.InnerException.Message);

                log.Error(LastErrorStr);
            }
            return ret;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public Bitmap ResizeImage(Image image, int width, int height)
        {
            if (image == null) return null;

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        #region Helper
        private void SetParameterIfNotNull(parameter param)
        {
            if (param.av != null) Parameter.av = param.av;
            if (param.tv != null) Parameter.tv = param.tv;
            if (param.sv != null) Parameter.sv = param.sv;
            if (param.xv != null) Parameter.xv = param.xv;
            //TODO: alles implementieren
        }
        #endregion Helper
    }
}
