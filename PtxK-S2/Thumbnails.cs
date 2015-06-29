using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace PtxK_S2
{
    public class Thumbnails
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private string ip;
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
        public Thumbnails(string ip, filelist filelist)
        {
            this.ip = ip;
            this.filelist = filelist;

            this.thumbsReceived = 0;
            this.thumbsCount = 0;

            //Count all files to fetch
            foreach (dirs d in filelist.dirs)
            {
                foreach (string s in d.files)
                {
                    this.thumbsCount++;
                }
            }

            //log.Debug("Thumnailthread created");
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
                //log.Debug("Thumnailthread started");
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
                //log.Debug("Thumnailthread set stop event");
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
                //log.Debug("Thumnailthread wait for stop");
            }
        }

        // Abort thread
        public void Stop()
        {
            if (this.Running)
            {
                thread.Abort();
                WaitForStop();
                //log.Debug("Thumnailthread stopped");
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
            K_S2 ks2 = new K_S2(ip);

            // reset reload event
            reloadEvent.Reset();

            try
            {

                // loop
                foreach (dirs d in filelist.dirs)
                {
                    foreach (string fn in d.files)
                    {
                        //log.DebugFormat("Fetching Thumbnail {0} from {1} ({2}/{3}",thumbsReceived,thumbsCount, d,fn);
                        string ext = Path.GetExtension(fn).ToUpper();
                        if (ext == ".JPG")
                        {
                            Bitmap bmp = (Bitmap)ks2.GetImage(d.name, fn, "thumb");
                            // notify client
                            NewFrame(this, new ThumbnailEventArgs(bmp, d.name, fn, thumbsReceived, thumbsCount));
                            // release the image
                            bmp.Dispose();
                            bmp = null;
                        }
                        this.thumbsReceived++;
                        if( (stopEvent.WaitOne(0, true)) || (reloadEvent.WaitOne(0, true)) )
                        {
                            break;
                        }
                    }
                }

            }
            catch (WebException ex)
            {
                log.Error("Thumbnail Error: " + ex.Message);
                // wait for a while before the next try
                Thread.Sleep(250);
            }
            catch (ApplicationException ex)
            {
                log.Error("Thumbnail Error: " + ex.Message);
                // wait for a while before the next try
                Thread.Sleep(250);
            }
            catch (Exception ex)
            {
                log.Error("Thumbnail Error: " + ex.Message);
            }
            finally
            {
                //log.Debug("Thumbnailthread finally");
            }

            //log.Debug("Thumbnailthread end");
        }
    }
}
