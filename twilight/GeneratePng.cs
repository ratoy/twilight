using System;
using System.Drawing;

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
			//read world map

			//calculate sun position and twilight
			SunPos sunpos = new SunPos ();
			Point p=sunpos.GetSunPos ();
			Console.WriteLine (p.X +","+p.Y );
			//calculate moon position
			MoonPos moonpos = new MoonPos ();

			//transform coordinates

			//write to png
			PngWriter pw = new PngWriter ();
			pw.SavePng ();
		}

		Size GetScreenRes()
		{

			return new Size (1280, 800);
		}
	}
}

