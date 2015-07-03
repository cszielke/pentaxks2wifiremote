using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PtxK_S2
{
    // NewFrame delegate
    public delegate void DownloadEventHandler(object sender, DownloadEventArgs e);

    /// <summary>
    /// Thumbnail event arguments
    /// </summary>
    public class DownloadEventArgs : EventArgs
    {
        private string filename;
        private int count;
        private int totalCount;
        private string message;

        // Constructor
        public DownloadEventArgs(string filename, int count, int totalcount,string message)
        {
            this.filename = filename;
            this.count = count;
            this.totalCount = totalcount;
            this.message = message;
        }

        // Bitmap property
        public string  Message
        {
            get { return message; }
        }

        // Bitmap property
        public string Filename
        {
            get { return filename; }
        }
        // Count property
        public int Count
        {
            get { return count; }
        }
        // TotalCount property
        public int TotalCount
        {
            get { return totalCount; }
        }
    }
}

