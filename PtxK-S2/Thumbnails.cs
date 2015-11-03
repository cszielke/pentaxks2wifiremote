using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace PtxK_S2
{
    public class Thumbnails
    {
        private string ip;
        private List<string> urllist;

        private filelist filelist;

        private Thread thread = null;
        private ManualResetEvent stopEvent = null;
        private ManualResetEvent reloadEvent = null;

        private int thumbsReceived;
        private int thumbsCount;
        // new frame event
        public event ThumbnailEventHandler NewFrame;

        // Get state of the video source thread
        public bool Running
        {
            get
            {
                if (thread != null)
                {
                    if (thread.Join(0) == false)
                        return true;

                    // the thread is not running, so free resources
                    Free();
                }
                return false;
            }
        }

        public int Count
        {
            get { return thumbsCount; }
        }

        public int ThumbsReceived
        {
            get { return thumbsReceived; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ip">Url to load from something like "192.168.0.1"</param>
        /// <param name="filelist">filelist as object</param>
        public Thumbnails(string ip, ref filelist filelist)
        {
            this.ip = ip;
            this.urllist = new List<string>();
            this.filelist = filelist;

            this.thumbsReceived = 0;
            this.thumbsCount = 0;

            //Count all files to fetch
            foreach (dirs d in filelist.dirs)
            {
                foreach (string s in d.files)
                {
                    string filepath = d.name + "/" + s;
                    urllist.Add(filepath);
                }
            }
            this.thumbsCount = urllist.Count;
        }

        // Start work
        public void Start()
        {
            if (thread == null)
            {
                thumbsReceived = 0;

                // create events
                stopEvent = new ManualResetEvent(false);
                reloadEvent = new ManualResetEvent(false);

                // create and start new thread
                thread = new Thread(new ThreadStart(WorkerThread));
                thread.Name = "ThumbnailsThread";
                thread.Start();
            }
        }

        // Signal thread to stop work
        public void SignalToStop()
        {
            // stop thread
            if (thread != null)
            {
                // signal to stop
                stopEvent.Set();
            }
        }

        // Wait for thread stop
        public void WaitForStop()
        {
            if (thread != null)
            {
                // wait for thread stop
                thread.Join();

                Free();
            }
        }

        // Abort thread
        public void Stop()
        {
            if (this.Running)
            {
                thread.Abort();
                WaitForStop();
            }
        }

        // Free resources
        private void Free()
        {
            thread = null;

            // release events
            stopEvent.Close();
            stopEvent = null;
            reloadEvent.Close();
            reloadEvent = null;
        }

        // Thread entry point
        public void WorkerThread()
        {
            //K_S2 ks2 = new K_S2(ip);
            HttpWebRequest request = null;
            WebResponse response = null;
            Stream stream = null;
            // reset reload event
            reloadEvent.Reset();
            try
            {
                // loop
                int n = 0;
                while( (n < urllist.Count) && (!stopEvent.WaitOne(0, true)) && (!reloadEvent.WaitOne(0, true)))
				{
                    string fn = urllist[n];
                    System.Diagnostics.Debug.WriteLine("Get Tumbnail for: " + fn);
                    string ext = Path.GetExtension(fn).ToUpper();
                    if (ext == ".JPG")
                    {
                        Bitmap bmp;
                        if (filelist.thumbcache.TryGetValue(fn,out bmp)) //is in cache
                        {
                            NewFrame(this, new ThumbnailEventArgs(bmp, fn, thumbsReceived, thumbsCount));
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(ip)) //Simulation
                            {
                                bmp = new Bitmap(@"simfiles\thumbimg.jpg");
                                // notify client
                                Thread.Sleep(20); //Simulate loading of file
                                filelist.thumbcache.Add(fn, bmp);
                                NewFrame(this, new ThumbnailEventArgs(bmp, fn, thumbsReceived, thumbsCount));
                            }
                            else
                            {
                                string urlGetFile = "http://{0}/v1/photos/{1}?size={2}";
                                string url = String.Format(urlGetFile, ip, fn, "thumb");

                                request = (HttpWebRequest)WebRequest.Create(url);
                                request.ConnectionGroupName = GetHashCode().ToString() + fn;

                                request.Timeout = 3000;

                                using (response = request.GetResponse())
                                {
                                    using (stream = response.GetResponseStream())
                                    {
                                        if (NewFrame != null)
                                        {
                                            bmp = new Bitmap(stream);
                                            filelist.thumbcache.Add(fn, bmp);
                                            // notify client
                                            NewFrame(this, new ThumbnailEventArgs(bmp, fn, thumbsReceived, thumbsCount));
                                        }
                                    }
                                }
                                request.Abort();
                                request = null;
                            }
                        }
                        // Don't release the image... its in cache!!
                        //bmp.Dispose();
                        //bmp = null;
                    }

                    this.thumbsReceived++;
                    n++;
                }
                NewFrame(this, new ThumbnailEventArgs(null, "", thumbsCount, thumbsCount));
            }
            catch (WebException ex)
            {
                // wait for a while before the next try
                Thread.Sleep(250);
                System.Diagnostics.Debug.WriteLine("Sleep 250" + ex.Message);
            }
            catch (ApplicationException ex)
            {
                System.Diagnostics.Debug.WriteLine("=============: " + ex.Message);
                // wait for a while before the next try
                Thread.Sleep(250);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=============: " + ex.Message);
            }
            finally
            {
                // abort request
                if (request != null)
                {
                    request.Abort();
                    request = null;
                }
                // close response stream
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
                // close response
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }
    }
}
