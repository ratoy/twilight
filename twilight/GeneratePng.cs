using System;
using System.Drawing;
using System.Windows.Forms;

namespace twilight
{
	public class GeneratePng
	{
		public GeneratePng ()
		{
		}

		public void Do ()
		{
			//get screen resolution
			Size Resolution = GetScreenRes ();
			 
			//write to png
			PngWriter pw = new PngWriter (Resolution.Width, Resolution.Height);
			pw.SavePng ();
		}

		Size GetScreenRes ()
		{
			Rectangle resolution = Screen.PrimaryScreen.Bounds;
			return new Size (resolution.Width, resolution.Height);
		}
	}
}

