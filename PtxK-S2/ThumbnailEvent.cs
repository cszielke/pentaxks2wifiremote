using System;
using System.Drawing.Imaging;

namespace PtxK_S2
{
	// NewFrame delegate
	public delegate void ThumbnailEventHandler(object sender, ThumbnailEventArgs e);

    /// <summary>
    /// Thumbnail event arguments
    /// </summary>
    public class ThumbnailEventArgs : EventArgs
    {
        private System.Drawing.Bitmap bmp;
        private string dir;
        private string filename;
        private int count;
        private int totalCount;

		// Constructor
		public ThumbnailEventArgs(System.Drawing.Bitmap bmp,string dir, string filename,int count, int totalcount)
		{
			this.bmp = bmp;
            this.dir = dir;
            this.filename = filename;
            this.count = count;
            this.totalCount = totalcount;
		}

		// Bitmap property
		public System.Drawing.Bitmap Bitmap
		{
			get { return bmp; }
		}

        // Bitmap property
        public string Dir
        {
            get { return dir; }
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
