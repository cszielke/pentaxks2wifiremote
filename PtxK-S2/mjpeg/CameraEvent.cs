using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PtxK_S2
{
 	using System;
	using System.Drawing.Imaging;

	// NewFrame delegate
	public delegate void CameraEventHandler(object sender, CameraEventArgs e);

	/// <summary>
	/// Camera event arguments
	/// </summary>
    public class CameraEventArgs : EventArgs
    {
		private System.Drawing.Bitmap bmp;

		// Constructor
		public CameraEventArgs(System.Drawing.Bitmap bmp)
		{
			this.bmp = bmp;
		}

		// Bitmap property
		public System.Drawing.Bitmap Bitmap
		{
			get { return bmp; }
		}
	}
}
