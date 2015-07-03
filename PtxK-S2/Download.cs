using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace PtxK_S2
{
    public class Download
    {
        private string url;
        private Dictionary<string, string> filelist;

        private Thread thread = null;
        private ManualResetEvent stopEvent = null;
        private ManualResetEvent reloadEvent = null;

        private int filesReceived;

        // new frame event
        public event DownloadEventHandler DownloadNotify;

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
            get { return filelist.Count; }
        }

        public int FilesReceived
        {
            get { return filesReceived; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ip">Url to load from something like "192.168.0.1"</param>
        /// <param name="filelist">filelist as object</param>
        public Download(string url, Dictionary<string,string> filelist)
        {
            this.url = url;
            this.filelist = filelist;

            this.filesReceived = 0;
            
        }

        // Start work
        public void Start()
        {
            if (thread == null)
            {
                filesReceived = 0;

                // create events
                stopEvent = new ManualResetEvent(false);
                reloadEvent = new ManualResetEvent(false);

                // create and start new thread
                thread = new Thread(new ThreadStart(WorkerThread));
                thread.Name = "DownloadThread";
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
            // reset reload event
            reloadEvent.Reset();
            try
            {
                // loop
                filesReceived = 0;
                foreach(KeyValuePair<string,string> kvp in filelist)
                {
                    filesReceived++;
                    if ((stopEvent.WaitOne(0, true)) || (reloadEvent.WaitOne(0, true)))
                    {
                        break; //Skip remaining files
                    }

                    System.Diagnostics.Debug.WriteLine("Get file: " + kvp.Key);

                    if (File.Exists(kvp.Value)) //skip existing files
                    {
                        DownloadNotify(this, new DownloadEventArgs(kvp.Key, filesReceived, filelist.Count,
                            string.Format("{0} skiped. ({1}/{2})", kvp.Key, filesReceived, filelist.Count))); 
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(url)) //Simulation
                        {
                            // notify client
                            Thread.Sleep(20); //Simulate loading of file
                            DownloadNotify(this, new DownloadEventArgs(kvp.Key, filesReceived, filelist.Count, 
                                string.Format("{0} downloaded. ({1}/{2})", kvp.Key, filesReceived, filelist.Count)));
                        }
                        else
                        {
                            DownloadNotify(this, new DownloadEventArgs(kvp.Key, filesReceived, filelist.Count,
                                string.Format("Downloading {0}. ({1}/{2})", kvp.Key, filesReceived, filelist.Count))); 

                            string urlGetFile = string.Format("{0}/{1}", url, kvp.Key);
                            if (http.DownloadRemoteImageFile(urlGetFile, kvp.Value))
                            {
                                DownloadNotify(this, new DownloadEventArgs(kvp.Key, filesReceived, filelist.Count, 
                                    string.Format("{0} downloaded. ({1}/{2})", kvp.Key, filesReceived, filelist.Count))); 
                            }
                            else
                            {
                                DownloadNotify(this, new DownloadEventArgs(kvp.Key, filesReceived, filelist.Count, 
                                    string.Format("Download {0} failed. ({1}/{2})", kvp.Key, filesReceived, filelist.Count)));
                            }
                        }
                    }
                    
                }
                DownloadNotify(this, new DownloadEventArgs("", filesReceived, filelist.Count, 
                    "Download files done."));
            }
            catch (WebException ex)
            {
                // wait for a while before the next try
                Thread.Sleep(250);
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
                System.Diagnostics.Debug.WriteLine("Download finally");
            }
        }
    }
}
